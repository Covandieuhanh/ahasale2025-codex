using System;
using System.Linq;

public static class GianHangLedger_cl
{
    public const string TagRoot = "|GIANHANG|";
    public const string TagCreditSeller = "|GIANHANG|CREDIT_SELLER|";
    public const string TagWithdraw = "|GIANHANG|WITHDRAW|";

    public sealed class BalanceResult
    {
        public decimal TieuDung { get; set; }
        public decimal UuDai { get; set; }
        public bool Updated { get; set; }
    }

    public static bool HasCreditForOrder(dbDataContext db, string seller, string orderId, int loaiVi)
    {
        if (db == null)
            return false;

        string sellerKey = NormalizeAccount(seller);
        string safeOrderId = NormalizeText(orderId);
        if (sellerKey == string.Empty || safeOrderId == string.Empty)
            return false;

        return db.LichSu_DongA_tbs.Any(x =>
            x.taikhoan == sellerKey
            && x.id_donhang == safeOrderId
            && x.CongTru == true
            && x.LoaiHoSo_Vi == loaiVi);
    }

    public static bool AddSellerCreditFromOrder(dbDataContext db, string seller, string orderId, decimal amountA, int loaiVi, string note)
    {
        if (db == null || amountA <= 0m)
            return false;

        string sellerKey = NormalizeAccount(seller);
        string safeOrderId = NormalizeText(orderId);
        if (sellerKey == string.Empty || safeOrderId == string.Empty)
            return false;

        if (HasCreditForOrder(db, sellerKey, safeOrderId, loaiVi))
            return false;

        LichSu_DongA_tb item = new LichSu_DongA_tb();
        item.taikhoan = sellerKey;
        item.dongA = amountA;
        item.ngay = AhaTime_cl.Now;
        item.CongTru = true;
        item.id_donhang = safeOrderId;
        item.LoaiHoSo_Vi = loaiVi;
        item.ghichu = string.Format("{0} {1}", TagCreditSeller, NormalizeText(note));
        db.LichSu_DongA_tbs.InsertOnSubmit(item);

        ApplyBalanceDelta(db, sellerKey, loaiVi, amountA);
        return true;
    }

    public static void AddSellerDebit(dbDataContext db, string seller, decimal amountA, int loaiVi, string note, string orderId)
    {
        if (db == null || amountA <= 0m)
            return;

        string sellerKey = NormalizeAccount(seller);
        if (sellerKey == string.Empty)
            return;

        LichSu_DongA_tb item = new LichSu_DongA_tb();
        item.taikhoan = sellerKey;
        item.dongA = amountA;
        item.ngay = AhaTime_cl.Now;
        item.CongTru = false;
        item.id_donhang = NormalizeText(orderId);
        item.LoaiHoSo_Vi = loaiVi;
        item.ghichu = string.Format("{0} {1}", TagRoot, NormalizeText(note));
        db.LichSu_DongA_tbs.InsertOnSubmit(item);

        ApplyBalanceDelta(db, sellerKey, loaiVi, -amountA);
    }

    public static BalanceResult RecalculateBalances(dbDataContext db, string seller, bool updateDb)
    {
        BalanceResult result = new BalanceResult();
        if (db == null)
            return result;

        string sellerKey = NormalizeAccount(seller);
        if (sellerKey == string.Empty)
            return result;

        IQueryable<LichSu_DongA_tb> entries = QueryGianHangEntries(db, sellerKey);

        decimal addTieuDung = entries.Where(x => x.LoaiHoSo_Vi == 1 && x.CongTru == true)
            .Select(x => (decimal?)x.dongA).Sum() ?? 0m;
        decimal subTieuDung = entries.Where(x => x.LoaiHoSo_Vi == 1 && x.CongTru == false)
            .Select(x => (decimal?)x.dongA).Sum() ?? 0m;
        decimal addUuDai = entries.Where(x => x.LoaiHoSo_Vi == 2 && x.CongTru == true)
            .Select(x => (decimal?)x.dongA).Sum() ?? 0m;
        decimal subUuDai = entries.Where(x => x.LoaiHoSo_Vi == 2 && x.CongTru == false)
            .Select(x => (decimal?)x.dongA).Sum() ?? 0m;

        result.TieuDung = addTieuDung - subTieuDung;
        result.UuDai = addUuDai - subUuDai;

        if (!updateDb)
            return result;

        taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == sellerKey);
        if (account == null)
            return result;

        bool changed = false;
        if ((account.HoSo_TieuDung_ShopOnly ?? 0m) != result.TieuDung)
        {
            account.HoSo_TieuDung_ShopOnly = result.TieuDung;
            changed = true;
        }

        if ((account.HoSo_UuDai_ShopOnly ?? 0m) != result.UuDai)
        {
            account.HoSo_UuDai_ShopOnly = result.UuDai;
            changed = true;
        }

        result.Updated = changed;
        return result;
    }

    public static int BackfillSellerCredits(dbDataContext db, string seller)
    {
        if (db == null)
            return 0;

        string sellerKey = NormalizeAccount(seller);
        if (sellerKey == string.Empty)
            return 0;

        int added = 0;
        var invoices = GianHangInvoice_cl.LoadStorefrontInvoicesWithRuntime(db, sellerKey, 500, 5000);

        foreach (GH_HoaDon_tb invoice in invoices)
        {
            if (!IsCreditableInvoice(invoice))
                continue;

            string orderId = ResolveBackfillOrderId(invoice);
            if (orderId == string.Empty)
                continue;

            string buyerKey = NormalizeAccount(invoice.buyer_account);
            if (buyerKey == string.Empty)
                continue;

            var buyerDebits = db.LichSu_DongA_tbs
                .Where(x => x.id_donhang == orderId
                            && x.taikhoan == buyerKey
                            && x.CongTru == false)
                .ToList();
            if (buyerDebits.Count == 0)
                continue;

            decimal tieuDung = buyerDebits.Where(x => (x.LoaiHoSo_Vi ?? 1) == 1).Sum(x => x.dongA ?? 0m);
            decimal uuDai = buyerDebits.Where(x => (x.LoaiHoSo_Vi ?? 1) == 2).Sum(x => x.dongA ?? 0m);
            string publicOrderId = GianHangInvoice_cl.ResolveOrderPublicId(invoice);
            if (publicOrderId == string.Empty)
                publicOrderId = orderId;

            if (tieuDung > 0m)
            {
                bool ok = AddSellerCreditFromOrder(
                    db,
                    sellerKey,
                    orderId,
                    tieuDung,
                    1,
                    string.Format("Ban don hang so {0} (Ho so tieu dung gian hang) [Backfill]", publicOrderId));
                if (ok)
                    added++;
            }

            if (uuDai > 0m)
            {
                bool ok = AddSellerCreditFromOrder(
                    db,
                    sellerKey,
                    orderId,
                    uuDai,
                    2,
                    string.Format("Ban don hang so {0} (Ho so uu dai gian hang) [Backfill]", publicOrderId));
                if (ok)
                    added++;
            }
        }

        return added;
    }

    private static bool IsCreditableInvoice(GH_HoaDon_tb invoice)
    {
        if (invoice == null)
            return false;

        string exchangeStatus = NormalizeText(invoice.exchange_status);
        string orderStatus = NormalizeText(invoice.order_status);
        string trangThai = NormalizeText(invoice.trang_thai);

        return string.Equals(exchangeStatus, DonHangStateMachine_cl.Exchange_DaTraoDoi, StringComparison.OrdinalIgnoreCase)
            || string.Equals(orderStatus, DonHangStateMachine_cl.Order_DaNhan, StringComparison.OrdinalIgnoreCase)
            || string.Equals(orderStatus, DonHangStateMachine_cl.Order_DaGiao, StringComparison.OrdinalIgnoreCase)
            || string.Equals(trangThai, GianHangInvoice_cl.TrangThaiDaThu, StringComparison.OrdinalIgnoreCase);
    }

    private static string ResolveBackfillOrderId(GH_HoaDon_tb invoice)
    {
        if (invoice == null)
            return string.Empty;

        string orderId = NormalizeText(GianHangInvoice_cl.ResolveRuntimeOrderKey(invoice));
        if (orderId != string.Empty)
            return orderId;

        return invoice.id > 0 ? invoice.id.ToString() : string.Empty;
    }

    private static IQueryable<LichSu_DongA_tb> QueryGianHangEntries(dbDataContext db, string sellerKey)
    {
        return db.LichSu_DongA_tbs.Where(x =>
            x.taikhoan == sellerKey
            && (
                (x.ghichu != null && x.ghichu.Contains(TagRoot))
                || (x.ghichu != null && x.ghichu.Contains(TagCreditSeller))
                || (x.id_donhang != null && x.id_donhang != "" && x.ghichu != null && x.ghichu.Contains("gian hang"))
            ));
    }

    private static void ApplyBalanceDelta(dbDataContext db, string sellerKey, int loaiVi, decimal delta)
    {
        taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == sellerKey);
        if (account == null)
            return;

        if (loaiVi == 1)
            account.HoSo_TieuDung_ShopOnly = (account.HoSo_TieuDung_ShopOnly ?? 0m) + delta;
        else if (loaiVi == 2)
            account.HoSo_UuDai_ShopOnly = (account.HoSo_UuDai_ShopOnly ?? 0m) + delta;
    }

    private static string NormalizeAccount(string value)
    {
        return (value ?? string.Empty).Trim().ToLowerInvariant();
    }

    private static string NormalizeText(string value)
    {
        return (value ?? string.Empty).Trim();
    }
}
