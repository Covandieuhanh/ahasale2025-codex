using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangOrderLifecycle_cl
{
    public sealed class RefundSummary
    {
        public decimal TotalRights { get; set; }
        public decimal ConsumerRights { get; set; }
        public decimal DiscountRights { get; set; }
    }

    public static void InsertBuyerNotice(
        dbDataContext db,
        string sellerAccount,
        string buyerAccount,
        string message,
        string link)
    {
        string buyerKey = (buyerAccount ?? string.Empty).Trim();
        if (db == null || string.IsNullOrWhiteSpace(buyerKey) || string.IsNullOrWhiteSpace(message))
            return;

        ThongBao_tb notice = new ThongBao_tb();
        notice.id = Guid.NewGuid();
        notice.daxem = false;
        notice.nguoithongbao = sellerAccount;
        notice.nguoinhan = buyerKey;
        notice.link = link ?? string.Empty;
        notice.noidung = message;
        notice.thoigian = AhaTime_cl.Now;
        notice.bin = false;
        db.ThongBao_tbs.InsertOnSubmit(notice);
    }

    public static GianHangOrderCommand_cl.CommandResult MarkDelivered(
        dbDataContext db,
        string sellerAccount,
        GianHangOrderRuntime_cl.OrderRuntime runtime,
        string publicOrderId)
    {
        string buyerAccount = GianHangOrderRuntime_cl.ResolveBuyerAccount(runtime);
        InsertBuyerNotice(
            db,
            sellerAccount,
            buyerAccount,
            GianHangOrderRuntime_cl.ResolveSellerDisplayName(db, runtime, sellerAccount, "Gian hàng") + " vừa xác nhận đã giao hàng. ID đơn hàng: " + publicOrderId,
            GianHangRoutes_cl.BuildBuyerOrdersUrl());
        GianHangOrderRuntime_cl.PersistDelivered(db, runtime);

        string emailErr;
        if (runtime != null && runtime.Invoice != null)
            GianHangEmailNotify_cl.TryNotifyInvoice(db, runtime.Invoice, GianHangEmailTemplate_cl.CodeOrderDelivered, "", out emailErr);

        return BuildSuccess("Đã cập nhật trạng thái giao hàng thành công.");
    }

    public static GianHangOrderCommand_cl.CommandResult CancelOrder(
        dbDataContext db,
        string sellerAccount,
        string publicOrderId,
        GianHangOrderRuntime_cl.OrderRuntime runtime)
    {
        string internalOrderId = runtime == null ? string.Empty : (runtime.OrderId ?? string.Empty).Trim();
        string buyerAccount = GianHangOrderRuntime_cl.ResolveBuyerAccount(runtime);
        string sellerCreditAccount = GianHangOrderRuntime_cl.ResolveSellerCreditAccount(runtime, sellerAccount);
        if (string.IsNullOrEmpty(buyerAccount))
        {
            GianHangLedger_cl.ReverseSellerCreditsForOrder(db, sellerCreditAccount, internalOrderId, publicOrderId, buyerAccount);
            GianHangOrderRuntime_cl.PersistCancelled(db, runtime);
            return BuildSuccess("Đã hủy đơn Offline trong /gianhang thành công.");
        }

        RefundSummary refund = RefundBuyerRights(db, internalOrderId, buyerAccount);
        if (refund == null)
            return BuildDanger("Không tìm thấy lịch sử trừ Quyền của đơn này. Không thể hoàn tự động.");

        GianHangLedger_cl.ReversalResult sellerReversal = GianHangLedger_cl.ReverseSellerCreditsForOrder(
            db,
            sellerCreditAccount,
            internalOrderId,
            publicOrderId,
            buyerAccount);

        InsertBuyerNotice(
            db,
            sellerAccount,
            buyerAccount,
            GianHangOrderRuntime_cl.ResolveSellerDisplayName(db, runtime, sellerAccount, "Gian hàng") + " vừa hủy đơn. ID đơn hàng: " + publicOrderId,
            GianHangRoutes_cl.BuildBuyerOrdersUrl());
        GianHangOrderRuntime_cl.PersistCancelled(db, runtime);

        string emailErr;
        if (runtime != null && runtime.Invoice != null)
            GianHangEmailNotify_cl.TryNotifyInvoice(db, runtime.Invoice, GianHangEmailTemplate_cl.CodeOrderCancelled, "Đơn hàng đã bị hủy.", out emailErr);

        string msg =
            "Hủy đơn thành công.<br/>" +
            string.Format("ID đơn: <b>{0}</b><br/>", publicOrderId) +
            string.Format("Đã hoàn tổng: <b>{0:#,##0.##} Quyền</b><br/>", refund.TotalRights) +
            string.Format("- Hồ sơ ưu đãi 30%: <b>+{0:#,##0.##} Quyền ưu đãi</b><br/>", refund.DiscountRights) +
            string.Format("- Hồ sơ tiêu dùng: <b>+{0:#,##0.##} Quyền tiêu dùng</b>", refund.ConsumerRights);

        if (sellerReversal != null && sellerReversal.Changed)
        {
            msg += "<br/><br/>Đã thu hồi hồ sơ quyền của gian hàng:<br/>"
                + string.Format("- Hồ sơ quyền ưu đãi: <b>-{0:#,##0.##} A</b><br/>", sellerReversal.UuDai)
                + string.Format("- Hồ sơ quyền tiêu dùng: <b>-{0:#,##0.##} A</b>", sellerReversal.TieuDung);
        }

        return new GianHangOrderCommand_cl.CommandResult
        {
            Success = true,
            Message = msg,
            AlertType = "success",
            UseHtmlMessage = true,
            OrderId = publicOrderId
        };
    }

    private static RefundSummary RefundBuyerRights(dbDataContext db, string orderId, string buyerAccount)
    {
        List<LichSu_DongA_tb> deducted = db.LichSu_DongA_tbs
            .Where(x => x.id_donhang == orderId && x.taikhoan == buyerAccount && x.CongTru == false)
            .ToList();
        if (deducted == null || deducted.Count == 0)
            return null;

        decimal consumerRights = deducted.Where(x => (x.LoaiHoSo_Vi ?? 1) == 1).Sum(x => x.dongA ?? 0m);
        decimal discountRights = deducted.Where(x => (x.LoaiHoSo_Vi ?? 1) == 2).Sum(x => x.dongA ?? 0m);
        decimal totalRights = consumerRights + discountRights;

        taikhoan_tb buyer = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == buyerAccount);
        if (buyer == null)
            return null;

        if (consumerRights > 0)
            buyer.DongA = (buyer.DongA ?? 0m) + consumerRights;
        if (discountRights > 0)
            buyer.Vi1That_Evocher_30PhanTram = (buyer.Vi1That_Evocher_30PhanTram ?? 0m) + discountRights;

        DateTime now = AhaTime_cl.Now;
        if (discountRights > 0)
        {
            LichSu_DongA_tb discountLog = new LichSu_DongA_tb();
            discountLog.taikhoan = buyerAccount;
            discountLog.dongA = discountRights;
            discountLog.ngay = now;
            discountLog.CongTru = true;
            discountLog.id_donhang = orderId;
            discountLog.LoaiHoSo_Vi = 2;
            discountLog.ghichu = string.Format("Hoàn Hồ sơ ưu đãi 30% đơn {0}: +{1:#,##0.##} Quyền ưu đãi", orderId, discountRights);
            db.LichSu_DongA_tbs.InsertOnSubmit(discountLog);
        }

        if (consumerRights > 0)
        {
            LichSu_DongA_tb consumerLog = new LichSu_DongA_tb();
            consumerLog.taikhoan = buyerAccount;
            consumerLog.dongA = consumerRights;
            consumerLog.ngay = now;
            consumerLog.CongTru = true;
            consumerLog.id_donhang = orderId;
            consumerLog.LoaiHoSo_Vi = 1;
            consumerLog.ghichu = string.Format("Hoàn Hồ sơ tiêu dùng đơn {0}: +{1:#,##0.##} Quyền tiêu dùng", orderId, consumerRights);
            db.LichSu_DongA_tbs.InsertOnSubmit(consumerLog);
        }

        return new RefundSummary
        {
            TotalRights = totalRights,
            ConsumerRights = consumerRights,
            DiscountRights = discountRights
        };
    }

    private static GianHangOrderCommand_cl.CommandResult BuildDanger(string message)
    {
        return new GianHangOrderCommand_cl.CommandResult
        {
            Success = false,
            Message = message,
            AlertType = "danger"
        };
    }

    private static GianHangOrderCommand_cl.CommandResult BuildSuccess(string message)
    {
        return new GianHangOrderCommand_cl.CommandResult
        {
            Success = true,
            Message = message,
            AlertType = "success"
        };
    }
}
