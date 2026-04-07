using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class GianHangLedger_cl
{
    private const string BackfillSessionPrefix = "gianhang_ledger_backfill_";
    public const string TagRoot = "|GIANHANG|";
    public const string TagCreditSeller = "|GIANHANG|CREDIT_SELLER|";
    public const string TagDebitSeller = "|GIANHANG|DEBIT_SELLER|";
    public const string TagWithdraw = "|GIANHANG|WITHDRAW|";

    public sealed class BalanceResult
    {
        public decimal TieuDung { get; set; }
        public decimal UuDai { get; set; }
        public bool Updated { get; set; }
    }

    public sealed class HistoryItem
    {
        public long Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public decimal AmountA { get; set; }
        public bool IsCredit { get; set; }
        public string OrderId { get; set; }
        public string PublicOrderId { get; set; }
        public string BuyerAccount { get; set; }
        public string Note { get; set; }
        public int WalletType { get; set; }
    }

    public sealed class HistoryPage
    {
        public BalanceResult Balance { get; set; }
        public int TotalCount { get; set; }
        public List<HistoryItem> Items { get; set; }
    }

    public sealed class ReversalResult
    {
        public decimal TieuDung { get; set; }
        public decimal UuDai { get; set; }
        public bool Changed { get; set; }
    }

    public static bool EnsureSellerLedgerReady(dbDataContext db, string seller)
    {
        if (db == null)
            return false;

        GianHangSchema_cl.EnsureSchemaSafe(db);

        string sellerKey = NormalizeAccount(seller);
        if (sellerKey == string.Empty)
            return false;
        string sessionKey = BackfillSessionPrefix + sellerKey;
        HttpContext context = HttpContext.Current;

        if (context != null && context.Session != null && context.Session[sessionKey] != null)
            return false;

        bool changed = BackfillSellerCredits(db, sellerKey) > 0;

        if (context != null && context.Session != null)
            context.Session[sessionKey] = true;

        return changed;
    }

    public static bool HasCreditForOrder(dbDataContext db, string seller, string orderId, int loaiVi)
    {
        if (db == null)
            return false;

        string sellerKey = NormalizeAccount(seller);
        string safeOrderId = NormalizeText(orderId);
        if (sellerKey == string.Empty || safeOrderId == string.Empty)
            return false;

        GianHangSchema_cl.EnsureSchemaSafe(db);

        return QueryBySeller(db, sellerKey).Any(x =>
            x.id_donhang == safeOrderId
            && x.cong_tru == true
            && x.loai_vi == loaiVi);
    }

    public static bool AddSellerCreditFromOrder(dbDataContext db, string seller, string orderId, decimal amountA, int loaiVi, string note, string publicOrderId, string buyerAccount)
    {
        if (db == null || amountA <= 0m)
            return false;

        string sellerKey = NormalizeAccount(seller);
        string safeOrderId = NormalizeText(orderId);
        if (sellerKey == string.Empty || safeOrderId == string.Empty)
            return false;

        GianHangSchema_cl.EnsureSchemaSafe(db);

        if (HasCreditForOrder(db, sellerKey, safeOrderId, loaiVi))
            return false;

        GH_HoSoQuyen_tb item = CreateEntry(
            sellerKey,
            loaiVi,
            amountA,
            true,
            safeOrderId,
            NormalizeText(publicOrderId) == string.Empty ? safeOrderId : NormalizeText(publicOrderId),
            NormalizeAccount(buyerAccount),
            string.Format("{0} {1}", TagCreditSeller, NormalizeText(note)));

        db.GetTable<GH_HoSoQuyen_tb>().InsertOnSubmit(item);
        return true;
    }

    public static void AddSellerDebit(dbDataContext db, string seller, decimal amountA, int loaiVi, string note, string orderId, string publicOrderId, string buyerAccount)
    {
        if (db == null || amountA <= 0m)
            return;

        string sellerKey = NormalizeAccount(seller);
        if (sellerKey == string.Empty)
            return;

        GianHangSchema_cl.EnsureSchemaSafe(db);

        GH_HoSoQuyen_tb item = CreateEntry(
            sellerKey,
            loaiVi,
            amountA,
            false,
            NormalizeText(orderId),
            NormalizeText(publicOrderId),
            NormalizeAccount(buyerAccount),
            string.Format("{0} {1}", TagDebitSeller, NormalizeText(note)));

        db.GetTable<GH_HoSoQuyen_tb>().InsertOnSubmit(item);
    }

    public static BalanceResult RecalculateBalances(dbDataContext db, string seller, bool updateDb)
    {
        BalanceResult result = new BalanceResult();
        if (db == null)
            return result;

        string sellerKey = NormalizeAccount(seller);
        if (sellerKey == string.Empty)
            return result;

        GianHangSchema_cl.EnsureSchemaSafe(db);

        IQueryable<GH_HoSoQuyen_tb> entries = QueryBySeller(db, sellerKey);

        decimal addTieuDung = entries.Where(x => x.loai_vi == 1 && x.cong_tru == true)
            .Select(x => (decimal?)x.so_quyen).Sum() ?? 0m;
        decimal subTieuDung = entries.Where(x => x.loai_vi == 1 && x.cong_tru == false)
            .Select(x => (decimal?)x.so_quyen).Sum() ?? 0m;
        decimal addUuDai = entries.Where(x => x.loai_vi == 2 && x.cong_tru == true)
            .Select(x => (decimal?)x.so_quyen).Sum() ?? 0m;
        decimal subUuDai = entries.Where(x => x.loai_vi == 2 && x.cong_tru == false)
            .Select(x => (decimal?)x.so_quyen).Sum() ?? 0m;

        result.TieuDung = addTieuDung - subTieuDung;
        result.UuDai = addUuDai - subUuDai;
        result.Updated = false;
        return result;
    }

    public static HistoryPage LoadHistoryPage(dbDataContext db, string seller, int loaiVi, string keyword, int page, int pageSize)
    {
        HistoryPage result = new HistoryPage
        {
            Balance = new BalanceResult(),
            Items = new List<HistoryItem>(),
            TotalCount = 0
        };

        if (db == null)
            return result;

        string sellerKey = NormalizeAccount(seller);
        if (sellerKey == string.Empty)
            return result;

        GianHangSchema_cl.EnsureSchemaSafe(db);

        bool changed = EnsureSellerLedgerReady(db, sellerKey);
        if (changed)
            db.SubmitChanges();

        result.Balance = RecalculateBalances(db, sellerKey, false);

        if (pageSize <= 0)
            pageSize = 30;
        if (page <= 0)
            page = 1;

        string safeKeyword = NormalizeText(keyword);
        IQueryable<GH_HoSoQuyen_tb> query = QueryBySeller(db, sellerKey).Where(x => x.loai_vi == loaiVi);

        if (safeKeyword != string.Empty)
        {
            query = query.Where(x =>
                (x.id_donhang != null && x.id_donhang.Contains(safeKeyword))
                || (x.public_order_id != null && x.public_order_id.Contains(safeKeyword))
                || (x.buyer_account != null && x.buyer_account.Contains(safeKeyword))
                || (x.ghi_chu != null && x.ghi_chu.Contains(safeKeyword)));
        }

        result.TotalCount = query.Count();
        result.Items = query
            .OrderByDescending(x => x.ngay_tao)
            .ThenByDescending(x => x.id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList()
            .Select(x => new HistoryItem
            {
                Id = x.id,
                CreatedAt = x.ngay_tao,
                AmountA = x.so_quyen ?? 0m,
                IsCredit = x.cong_tru ?? false,
                OrderId = x.id_donhang,
                PublicOrderId = x.public_order_id,
                BuyerAccount = x.buyer_account,
                Note = x.ghi_chu,
                WalletType = x.loai_vi ?? 0
            })
            .ToList();

        return result;
    }

    public static int BackfillSellerCredits(dbDataContext db, string seller)
    {
        if (db == null)
            return 0;

        string sellerKey = NormalizeAccount(seller);
        if (sellerKey == string.Empty)
            return 0;

        GianHangSchema_cl.EnsureSchemaSafe(db);

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

            if (tieuDung > 0m && !HasCreditForOrder(db, sellerKey, orderId, 1))
            {
                db.GetTable<GH_HoSoQuyen_tb>().InsertOnSubmit(CreateEntry(
                    sellerKey,
                    1,
                    tieuDung,
                    true,
                    orderId,
                    publicOrderId,
                    buyerKey,
                    string.Format("{0} Ban don hang so {1} (Ho so tieu dung gian hang) [Backfill]", TagCreditSeller, publicOrderId)));
                added++;
            }

            if (uuDai > 0m && !HasCreditForOrder(db, sellerKey, orderId, 2))
            {
                db.GetTable<GH_HoSoQuyen_tb>().InsertOnSubmit(CreateEntry(
                    sellerKey,
                    2,
                    uuDai,
                    true,
                    orderId,
                    publicOrderId,
                    buyerKey,
                    string.Format("{0} Ban don hang so {1} (Ho so uu dai gian hang) [Backfill]", TagCreditSeller, publicOrderId)));
                added++;
            }
        }

        return added;
    }

    public static ReversalResult ReverseSellerCreditsForOrder(dbDataContext db, string seller, string orderId, string publicOrderId, string buyerAccount)
    {
        ReversalResult result = new ReversalResult();
        if (db == null)
            return result;

        string sellerKey = NormalizeAccount(seller);
        string safeOrderId = NormalizeText(orderId);
        if (sellerKey == string.Empty || safeOrderId == string.Empty)
            return result;

        GianHangSchema_cl.EnsureSchemaSafe(db);

        List<GH_HoSoQuyen_tb> items = QueryBySeller(db, sellerKey)
            .Where(x => x.id_donhang == safeOrderId)
            .ToList();

        decimal creditTieuDung = items.Where(x => (x.loai_vi ?? 0) == 1 && x.cong_tru == true).Sum(x => x.so_quyen ?? 0m);
        decimal debitTieuDung = items.Where(x => (x.loai_vi ?? 0) == 1 && x.cong_tru == false).Sum(x => x.so_quyen ?? 0m);
        decimal creditUuDai = items.Where(x => (x.loai_vi ?? 0) == 2 && x.cong_tru == true).Sum(x => x.so_quyen ?? 0m);
        decimal debitUuDai = items.Where(x => (x.loai_vi ?? 0) == 2 && x.cong_tru == false).Sum(x => x.so_quyen ?? 0m);

        decimal needReverseTieuDung = creditTieuDung - debitTieuDung;
        decimal needReverseUuDai = creditUuDai - debitUuDai;
        string safePublicOrderId = NormalizeText(publicOrderId);
        if (safePublicOrderId == string.Empty)
            safePublicOrderId = safeOrderId;

        if (needReverseTieuDung > 0m)
        {
            AddSellerDebit(
                db,
                sellerKey,
                needReverseTieuDung,
                1,
                string.Format("Hoan tra ho so quyen tieu dung cho don {0} do huy giao dich", safePublicOrderId),
                safeOrderId,
                safePublicOrderId,
                buyerAccount);
            result.TieuDung = needReverseTieuDung;
            result.Changed = true;
        }

        if (needReverseUuDai > 0m)
        {
            AddSellerDebit(
                db,
                sellerKey,
                needReverseUuDai,
                2,
                string.Format("Hoan tra ho so quyen uu dai cho don {0} do huy giao dich", safePublicOrderId),
                safeOrderId,
                safePublicOrderId,
                buyerAccount);
            result.UuDai = needReverseUuDai;
            result.Changed = true;
        }

        return result;
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

    private static IQueryable<GH_HoSoQuyen_tb> QueryBySeller(dbDataContext db, string sellerKey)
    {
        return db.GetTable<GH_HoSoQuyen_tb>().Where(x => x.shop_taikhoan == sellerKey);
    }

    private static GH_HoSoQuyen_tb CreateEntry(string sellerKey, int loaiVi, decimal amountA, bool isCredit, string orderId, string publicOrderId, string buyerAccount, string note)
    {
        GH_HoSoQuyen_tb item = new GH_HoSoQuyen_tb();
        item.shop_taikhoan = sellerKey;
        item.loai_vi = loaiVi;
        item.so_quyen = amountA;
        item.cong_tru = isCredit;
        item.id_donhang = NormalizeText(orderId);
        item.public_order_id = NormalizeText(publicOrderId);
        item.buyer_account = NormalizeAccount(buyerAccount);
        item.ghi_chu = NormalizeText(note);
        item.ngay_tao = AhaTime_cl.Now;
        return item;
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
