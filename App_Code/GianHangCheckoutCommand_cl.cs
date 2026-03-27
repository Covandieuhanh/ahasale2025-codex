using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web.SessionState;

public static class GianHangCheckoutCommand_cl
{
    private sealed class ExchangeContext
    {
        public string SellerAccount { get; set; }
        public string OrderId { get; set; }
        public string InternalOrderId { get; set; }
        public string SellerDonBanUrl { get; set; }
        public string ChoThanhToanUrl { get; set; }
        public string BuyerDonMuaUrl { get; set; }
        public string LoginUrl { get; set; }
        public string ClientIp { get; set; }
        public string InputPin { get; set; }
        public int LinkContextTtlMinutes { get; set; }

        public GianHangCheckoutCore_cl.PaymentContext PaymentContext { get; set; }
        public GianHangOrderRuntime_cl.OrderRuntime WaitingRuntime { get; set; }
        public taikhoan_tb PayerAccount { get; set; }
        public The_PhatHanh_tb Card { get; set; }
        public int CardType { get; set; }
        public string WaitingOrderId { get; set; }

        public GianHangOrderRuntime_cl.OrderRuntime Runtime { get; set; }
        public string PublicOrderId { get; set; }
        public GH_HoaDon_tb RuntimeInvoice { get; set; }
        public decimal TotalVnd { get; set; }
        public decimal TotalRights { get; set; }
        public GianHangOrderDetail_cl.DiscountSummary DiscountSummary { get; set; }
        public decimal DiscountRights { get; set; }
        public bool HasDiscount { get; set; }
        public DateTime Now { get; set; }
        public string SellerCreditAccount { get; set; }
    }

    public sealed class ExchangeResult
    {
        public bool Success { get; set; }
        public bool ShouldRedirect { get; set; }
        public bool UseOnloadDialog { get; set; }
        public string RedirectUrl { get; set; }
        public string DialogTitle { get; set; }
        public string DialogMessage { get; set; }
        public string DialogType { get; set; }
        public string SuccessNotice { get; set; }
    }

    public static ExchangeResult ExecuteExchange(
        dbDataContext db,
        HttpSessionState session,
        string sellerAccount,
        string orderId,
        string inputPin,
        string sellerDonBanUrl,
        string choThanhToanUrl,
        string buyerDonMuaUrl,
        string loginUrl,
        string clientIp,
        int linkContextTtlMinutes)
    {
        ExchangeContext context = BuildContext(
            sellerAccount,
            orderId,
            inputPin,
            sellerDonBanUrl,
            choThanhToanUrl,
            buyerDonMuaUrl,
            loginUrl,
            clientIp,
            linkContextTtlMinutes);

        if (db == null || session == null)
            return FailDialog("Thông báo", "Có lỗi hệ thống. Vui lòng thử lại.", "alert");

        if (string.IsNullOrWhiteSpace(context.SellerAccount))
            return FailOnload("Thông báo", "Phiên của bạn đã hết hạn. Vui lòng đăng nhập lại.", context.LoginUrl, "alert");

        ExchangeResult validation = ValidateSessionAndCard(db, session, context);
        if (validation != null)
            return validation;

        db.Connection.Open();
        DbTransaction transaction = (DbTransaction)db.Connection.BeginTransaction(IsolationLevel.Serializable);
        db.Transaction = transaction;
        bool committed = false;

        try
        {
            ExchangeResult runtimeValidation = ValidatePinAndLoadRuntime(db, session, context);
            if (runtimeValidation != null)
                return runtimeValidation;

            if (context.CardType == 1)
            {
                ExchangeResult discountResult = ExecuteDiscountCardExchange(db, context);
                transaction.Commit();
                committed = true;
                WalletPaymentSession_cl.Clear(session);
                return discountResult;
            }

            if (context.CardType == 2)
            {
                ExchangeResult consumerResult = ExecuteConsumerCardExchange(db, context);
                transaction.Commit();
                committed = true;
                WalletPaymentSession_cl.Clear(session);
                return consumerResult;
            }

            return FailDialog("Thông báo", "Loại thẻ không hợp lệ.", "alert");
        }
        finally
        {
            if (!committed)
                SafeRollback(transaction);

            db.Transaction = null;
            SafeCloseConnection(db);
        }
    }

    private static void SafeRollback(DbTransaction transaction)
    {
        if (transaction == null)
            return;

        try
        {
            transaction.Rollback();
        }
        catch
        {
        }
    }

    private static void SafeCloseConnection(dbDataContext db)
    {
        if (db == null || db.Connection == null)
            return;

        try
        {
            db.Connection.Close();
        }
        catch
        {
        }
    }

    private static ExchangeContext BuildContext(
        string sellerAccount,
        string orderId,
        string inputPin,
        string sellerDonBanUrl,
        string choThanhToanUrl,
        string buyerDonMuaUrl,
        string loginUrl,
        string clientIp,
        int linkContextTtlMinutes)
    {
        return new ExchangeContext
        {
            SellerAccount = (sellerAccount ?? string.Empty).Trim(),
            OrderId = (orderId ?? string.Empty).Trim(),
            InputPin = (inputPin ?? string.Empty).Trim(),
            SellerDonBanUrl = string.IsNullOrWhiteSpace(sellerDonBanUrl) ? "/gianhang/don-ban.aspx" : sellerDonBanUrl.Trim(),
            ChoThanhToanUrl = string.IsNullOrWhiteSpace(choThanhToanUrl) ? "/gianhang/cho-thanh-toan.aspx" : choThanhToanUrl.Trim(),
            BuyerDonMuaUrl = string.IsNullOrWhiteSpace(buyerDonMuaUrl) ? GianHangRoutes_cl.BuildBuyerOrdersUrl() : buyerDonMuaUrl.Trim(),
            LoginUrl = string.IsNullOrWhiteSpace(loginUrl) ? GianHangCheckoutPortal_cl.LoginUrl() : loginUrl.Trim(),
            ClientIp = clientIp ?? string.Empty,
            LinkContextTtlMinutes = linkContextTtlMinutes
        };
    }

    private static ExchangeResult ValidateSessionAndCard(dbDataContext db, HttpSessionState session, ExchangeContext context)
    {
        context.PaymentContext = GianHangCheckoutCore_cl.LoadPaymentContext(
            db,
            session,
            context.SellerAccount,
            true,
            context.LinkContextTtlMinutes);

        if (!string.IsNullOrEmpty(context.PaymentContext.ErrorCode))
        {
            WalletPaymentSession_cl.Clear(session);
            return FailOnload(
                "Thông báo",
                GianHangCheckoutCore_cl.BuildContextErrorMessage(context.PaymentContext.ErrorCode),
                context.ChoThanhToanUrl,
                "alert");
        }

        context.WaitingRuntime = context.PaymentContext.WaitingRuntime;
        context.WaitingOrderId = (context.PaymentContext.WaitingOrderId ?? string.Empty).Trim();
        context.PayerAccount = context.PaymentContext.PayerAccount;
        context.Card = context.PaymentContext.Card;
        context.CardType = context.PaymentContext.CardType;

        if (context.Card == null || !context.Card.TrangThai)
            return FailDialog("Thông báo", "Thẻ đã bị khóa. Vui lòng liên hệ quản trị hoặc quét thẻ khác.", "alert");

        if (context.CardType != 1 && context.CardType != 2)
            return FailDialog("Thông báo", "Thẻ này không hợp lệ để Trao đổi.", "alert");

        if (context.WaitingRuntime == null
            || string.IsNullOrWhiteSpace(context.OrderId)
            || !string.Equals(context.OrderId, context.WaitingOrderId, StringComparison.Ordinal))
            return FailDialog("Thông báo", "Đơn chờ trao đổi đã thay đổi. Vui lòng quét lại link trao đổi.", "alert");

        if (context.PayerAccount == null)
            return FailDialog("Thông báo", "Tài khoản trao đổi không tồn tại. Vui lòng kiểm tra lại thẻ.", "alert");

        return ValidatePinAttempt(db, context);
    }

    private static ExchangeResult ValidatePinAttempt(dbDataContext db, ExchangeContext context)
    {
        string payer = (context.PayerAccount.taikhoan ?? string.Empty).Trim();
        string currentPin = (context.PayerAccount.mapin_thanhtoan ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(currentPin))
            return FailDialog("Thông báo", "Vui lòng cài đặt mã pin Trao đổi trước khi Trao đổi", "alert");

        if (!PinSecurity_cl.IsValidPinFormat(context.InputPin))
            return FailDialog("Mã pin không đúng", "Mã PIN phải gồm đúng 4 chữ số.", "alert");

        DateTime lockUntil;
        if (PinAttemptGuard_cl.IsLocked(payer, context.ClientIp, out lockUntil))
            return FailDialog("Thông báo", "Chức năng đang bị tạm khóa, vui lòng thử lại vào lúc " + lockUntil.ToString("dd/MM/yyyy HH:mm"), "alert");

        if (PinSecurity_cl.VerifyAndUpgrade(context.PayerAccount, context.InputPin))
        {
            PinAttemptGuard_cl.Reset(payer, context.ClientIp);
            return null;
        }

        DateTime? lockUntilOnFail;
        bool shouldBlockAccount;
        int failCount = PinAttemptGuard_cl.RegisterFailure(payer, context.ClientIp, out lockUntilOnFail, out shouldBlockAccount);

        if (shouldBlockAccount)
        {
            context.PayerAccount.block = true;
            db.SubmitChanges();
            PinAttemptGuard_cl.Reset(payer, context.ClientIp);
            return FailOnload(
                "Mã pin không đúng",
                "Bạn đã nhập sai " + failCount + " lần. Tài khoản trao đổi đã bị khóa. Vui lòng liên hệ với chúng tôi để xác thực.",
                context.LoginUrl,
                "alert");
        }

        if (lockUntilOnFail != null)
        {
            return FailDialog(
                "Mã pin không đúng",
                "Bạn đã nhập sai " + failCount + " lần. Chức năng sẽ bị khóa đến " + lockUntilOnFail.Value.ToString("dd/MM/yyyy HH:mm"),
                "alert");
        }

        return FailDialog("Mã pin không đúng", "Bạn đã nhập sai " + failCount + " lần.", "alert");
    }

    private static ExchangeResult ValidatePinAndLoadRuntime(dbDataContext db, HttpSessionState session, ExchangeContext context)
    {
        string payer = (context.PayerAccount.taikhoan ?? string.Empty).Trim();

        Guid keyGuid;
        if (!WalletPaymentSession_cl.TryGetCardKey(session, out keyGuid))
            return FailDialog("Thông báo", "Phiên trao đổi không hợp lệ. Vui lòng quét lại link trao đổi.", "alert");

        The_PhatHanh_tb currentCard = db.The_PhatHanh_tbs.FirstOrDefault(p => p.idGuide == keyGuid);
        if (currentCard == null || !currentCard.TrangThai)
            return FailDialog("Thông báo", "Thẻ không còn hợp lệ để Trao đổi.", "alert");

        if (!string.Equals((currentCard.taikhoan ?? string.Empty).Trim(), payer, StringComparison.OrdinalIgnoreCase))
            return FailDialog("Thông báo", "Thông tin tài khoản trao đổi đã thay đổi. Vui lòng quét lại link trao đổi.", "alert");

        context.CardType = currentCard.LoaiThe;
        context.PayerAccount = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == payer);
        if (context.PayerAccount == null || context.PayerAccount.block == true)
            return FailDialog("Thông báo", "Tài khoản trao đổi không hợp lệ hoặc đã bị khóa.", "alert");

        context.Runtime =
            context.WaitingRuntime != null
            && string.Equals(
                (context.WaitingRuntime.OrderId ?? string.Empty).Trim(),
                (context.OrderId ?? string.Empty).Trim(),
                StringComparison.OrdinalIgnoreCase)
                ? context.WaitingRuntime
                : GianHangOrderRuntime_cl.ResolveByOrderKeyForCommand(db, context.SellerAccount, context.OrderId);
        if (context.Runtime == null)
            return FailOnload("Thông báo", "Có lỗi xãy ra.", context.SellerDonBanUrl, "alert");

        context.InternalOrderId = (context.Runtime.OrderId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(context.InternalOrderId))
            context.InternalOrderId = (context.OrderId ?? string.Empty).Trim();

        if (!GianHangOrderRuntime_cl.CanExecuteExchange(context.Runtime))
            return FailDialog("Thông báo", "Đơn hàng không còn ở trạng thái chờ Trao đổi.", "alert");

        context.PublicOrderId = GianHangOrderRuntime_cl.ResolvePublicOrderId(context.Runtime);
        if (string.IsNullOrWhiteSpace(context.PublicOrderId))
            context.PublicOrderId = context.OrderId;

        context.RuntimeInvoice = context.Runtime.Invoice;
        context.TotalVnd = GianHangOrderRuntime_cl.ResolveTotalAmount(context.Runtime);
        context.TotalRights = GianHangCheckoutCore_cl.ConvertVndToRights(context.TotalVnd);
        context.DiscountSummary = GianHangOrderDetail_cl.BuildDiscountSummary(
            db,
            context.SellerAccount,
            context.Runtime);
        context.DiscountRights = context.DiscountSummary.DiscountRights;
        context.HasDiscount = context.DiscountSummary.HasDiscount;
        context.Now = AhaTime_cl.Now;
        context.SellerCreditAccount = GianHangOrderRuntime_cl.ResolveSellerCreditAccount(context.Runtime, context.SellerAccount);
        return null;
    }

    private static ExchangeResult ExecuteDiscountCardExchange(dbDataContext db, ExchangeContext context)
    {
        if (!context.HasDiscount)
            return FailDialog("Thông báo", "Thẻ ưu đãi chỉ áp dụng cho đơn có ưu đãi. Đơn này không có ưu đãi nên không thể Trao đổi bằng thẻ ưu đãi.", "alert");

        decimal currentDiscountWallet = context.PayerAccount.Vi1That_Evocher_30PhanTram ?? 0m;
        if (currentDiscountWallet < context.DiscountRights)
            return FailDialog("Thông báo", "Ví ưu đãi không đủ để Trao đổi.\nCần: " + context.DiscountRights.ToString("#,##0.##") + " Quyền.", "alert");

        context.PayerAccount.Vi1That_Evocher_30PhanTram = currentDiscountWallet - context.DiscountRights;
        db.LichSu_DongA_tbs.InsertOnSubmit(new LichSu_DongA_tb
        {
            taikhoan = context.PayerAccount.taikhoan,
            dongA = context.DiscountRights,
            ngay = context.Now,
            CongTru = false,
            id_donhang = context.InternalOrderId,
            LoaiHoSo_Vi = 2,
            ghichu = "Ưu đãi đơn hàng số " + context.PublicOrderId + " (Thẻ ưu đãi)"
        });

        if (!string.IsNullOrWhiteSpace(context.SellerCreditAccount))
        {
            GianHangLedger_cl.AddSellerCreditFromOrder(
                db,
                context.SellerCreditAccount,
                context.InternalOrderId,
                context.DiscountRights,
                2,
                string.Format("Bán đơn hàng số {0} (Thẻ ưu đãi - Hồ sơ quyền ưu đãi gian hàng)", context.PublicOrderId));
        }

        InsertExchangeNotices(
            db,
            context.PayerAccount,
            context.SellerCreditAccount,
            context.SellerDonBanUrl,
            context.BuyerDonMuaUrl,
            BuildExchangeSuccessNotice(context.DiscountRights, 0m, context.PublicOrderId),
            context.Now);
        GianHangOrderRuntime_cl.PersistExchangeSuccess(db, context.SellerAccount, context.Runtime, context.PayerAccount);
        return SuccessRedirect(context.SellerDonBanUrl, BuildExchangeSuccessMessage(context.DiscountRights, 0m));
    }

    private static ExchangeResult ExecuteConsumerCardExchange(dbDataContext db, ExchangeContext context)
    {
        decimal currentRights = context.PayerAccount.DongA ?? 0m;
        decimal currentDiscountWallet = context.PayerAccount.Vi1That_Evocher_30PhanTram ?? 0m;

        bool applyDiscount = false;
        decimal discountPart = 0m;
        decimal consumerPart = 0m;

        if (context.HasDiscount)
        {
            discountPart = context.DiscountRights;
            applyDiscount = currentDiscountWallet >= discountPart;

            if (applyDiscount)
            {
                consumerPart = context.TotalRights - discountPart;
                if (consumerPart < 0m) consumerPart = 0m;

                if (currentRights < consumerPart)
                    return FailDialog("Thông báo", "Ví Quyền tiêu dùng không đủ cho phần còn lại.", "alert");
            }
            else if (currentRights < context.TotalRights)
            {
                return FailDialog("Thông báo", "Quyền tiêu dùng của bạn không đủ để Trao đổi.", "alert");
            }
        }
        else if (currentRights < context.TotalRights)
        {
            return FailDialog("Thông báo", "Quyền tiêu dùng của bạn không đủ để Trao đổi.", "alert");
        }

        if (applyDiscount)
        {
            context.PayerAccount.Vi1That_Evocher_30PhanTram = currentDiscountWallet - discountPart;
            db.LichSu_DongA_tbs.InsertOnSubmit(new LichSu_DongA_tb
            {
                taikhoan = context.PayerAccount.taikhoan,
                dongA = discountPart,
                ngay = context.Now,
                CongTru = false,
                id_donhang = context.InternalOrderId,
                LoaiHoSo_Vi = 2,
                ghichu = "Ưu đãi đơn hàng số " + context.PublicOrderId
            });

            context.PayerAccount.DongA = currentRights - consumerPart;
            db.LichSu_DongA_tbs.InsertOnSubmit(new LichSu_DongA_tb
            {
                taikhoan = context.PayerAccount.taikhoan,
                dongA = consumerPart,
                ngay = context.Now,
                CongTru = false,
                id_donhang = context.InternalOrderId,
                LoaiHoSo_Vi = 1,
                ghichu = "Trao đổi đơn hàng số " + context.PublicOrderId + " (phần còn lại)"
            });
        }
        else
        {
            context.PayerAccount.DongA = currentRights - context.TotalRights;
            db.LichSu_DongA_tbs.InsertOnSubmit(new LichSu_DongA_tb
            {
                taikhoan = context.PayerAccount.taikhoan,
                dongA = context.TotalRights,
                ngay = context.Now,
                CongTru = false,
                id_donhang = context.InternalOrderId,
                LoaiHoSo_Vi = 1,
                ghichu = "Trao đổi đơn hàng số " + context.PublicOrderId
            });
        }

        InsertSellerCredits(db, context, applyDiscount, discountPart, consumerPart);

        decimal successDiscountRights = applyDiscount ? discountPart : 0m;
        decimal successConsumerRights = applyDiscount ? consumerPart : context.TotalRights;
        InsertExchangeNotices(
            db,
            context.PayerAccount,
            context.SellerCreditAccount,
            context.SellerDonBanUrl,
            context.BuyerDonMuaUrl,
            BuildExchangeSuccessNotice(successDiscountRights, successConsumerRights, context.PublicOrderId),
            context.Now);
        GianHangOrderRuntime_cl.PersistExchangeSuccess(db, context.SellerAccount, context.Runtime, context.PayerAccount);
        return SuccessRedirect(context.SellerDonBanUrl, BuildExchangeSuccessMessage(successDiscountRights, successConsumerRights));
    }

    private static void InsertSellerCredits(
        dbDataContext db,
        ExchangeContext context,
        bool applyDiscount,
        decimal discountPart,
        decimal consumerPart)
    {
        if (string.IsNullOrWhiteSpace(context.SellerCreditAccount))
            return;

        if (applyDiscount)
        {
            GianHangLedger_cl.AddSellerCreditFromOrder(
                db,
                context.SellerCreditAccount,
                context.InternalOrderId,
                discountPart,
                2,
                string.Format("Bán đơn hàng số {0} (ưu đãi - Hồ sơ quyền ưu đãi gian hàng)", context.PublicOrderId));

            GianHangLedger_cl.AddSellerCreditFromOrder(
                db,
                context.SellerCreditAccount,
                context.InternalOrderId,
                consumerPart,
                1,
                string.Format("Bán đơn hàng số {0} (phần còn lại - Hồ sơ quyền tiêu dùng gian hàng)", context.PublicOrderId));
            return;
        }

        GianHangLedger_cl.AddSellerCreditFromOrder(
            db,
            context.SellerCreditAccount,
            context.InternalOrderId,
            context.TotalRights,
            1,
            string.Format("Bán đơn hàng số {0} (Hồ sơ quyền tiêu dùng gian hàng)", context.PublicOrderId));
    }

    private static void InsertExchangeNotices(
        dbDataContext db,
        taikhoan_tb payerAccount,
        string sellerAccount,
        string sellerDonBanUrl,
        string buyerDonMuaUrl,
        string successNotice,
        DateTime createdAt)
    {
        string buyerAccount = payerAccount == null ? string.Empty : (payerAccount.taikhoan ?? string.Empty).Trim();
        string buyerName = payerAccount == null ? buyerAccount : (!string.IsNullOrWhiteSpace(payerAccount.hoten) ? payerAccount.hoten : buyerAccount);

        db.ThongBao_tbs.InsertOnSubmit(new ThongBao_tb
        {
            id = Guid.NewGuid(),
            daxem = false,
            nguoithongbao = buyerAccount,
            nguoinhan = sellerAccount,
            link = sellerDonBanUrl,
            noidung = buyerName + " " + successNotice,
            thoigian = createdAt,
            bin = false
        });

        db.ThongBao_tbs.InsertOnSubmit(new ThongBao_tb
        {
            id = Guid.NewGuid(),
            daxem = false,
            nguoithongbao = sellerAccount,
            nguoinhan = buyerAccount,
            link = buyerDonMuaUrl,
            noidung = successNotice,
            thoigian = createdAt,
            bin = false
        });
    }

    private static ExchangeResult FailDialog(string title, string message, string dialogType)
    {
        return new ExchangeResult
        {
            Success = false,
            DialogTitle = title ?? "Thông báo",
            DialogMessage = message ?? string.Empty,
            DialogType = dialogType ?? "alert"
        };
    }

    private static ExchangeResult FailOnload(string title, string message, string redirectUrl, string dialogType)
    {
        return new ExchangeResult
        {
            Success = false,
            DialogTitle = title ?? "Thông báo",
            DialogMessage = message ?? string.Empty,
            DialogType = dialogType ?? "alert",
            UseOnloadDialog = true,
            ShouldRedirect = !string.IsNullOrWhiteSpace(redirectUrl),
            RedirectUrl = redirectUrl ?? string.Empty
        };
    }

    private static ExchangeResult SuccessRedirect(string redirectUrl, string successNotice)
    {
        return new ExchangeResult
        {
            Success = true,
            ShouldRedirect = !string.IsNullOrWhiteSpace(redirectUrl),
            RedirectUrl = redirectUrl ?? string.Empty,
            SuccessNotice = successNotice ?? string.Empty
        };
    }

    private static string BuildExchangeSuccessMessage(decimal discountRights, decimal consumerRights)
    {
        if (discountRights > 0m && consumerRights > 0m)
        {
            return string.Format(
                "Đã trao đổi thành công {0} Quyền ưu đãi và {1} Quyền tiêu dùng.",
                FormatQuyenValue(discountRights),
                FormatQuyenValue(consumerRights));
        }

        if (discountRights > 0m)
        {
            return string.Format(
                "Đã trao đổi thành công {0} Quyền ưu đãi.",
                FormatQuyenValue(discountRights));
        }

        return string.Format(
            "Đã trao đổi thành công {0} Quyền tiêu dùng.",
            FormatQuyenValue(consumerRights));
    }

    private static string BuildExchangeSuccessNotice(decimal discountRights, decimal consumerRights, string orderId)
    {
        return BuildExchangeSuccessMessage(discountRights, consumerRights) + " ID đơn hàng: " + orderId;
    }

    private static string FormatQuyenValue(decimal value)
    {
        return value.ToString("#,##0.##");
    }

}
