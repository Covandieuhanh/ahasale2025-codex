using System;
using System.Collections.Generic;
using System.Linq;

public static class ShopOnlyLedger_cl
{
    public const string TagRoot = "|SHOPONLY|";
    public const string TagCreditSeller = "|SHOPONLY|CREDIT_SELLER|";
    public const string TagWithdraw = "|SHOPONLY|WITHDRAW|";

    public class BalanceResult
    {
        public decimal TieuDung { get; set; }
        public decimal UuDai { get; set; }
        public bool Updated { get; set; }
    }

    public static bool HasCreditForOrder(dbDataContext db, string seller, string orderId, int loaiVi)
    {
        if (db == null) return false;
        if (string.IsNullOrWhiteSpace(seller) || string.IsNullOrWhiteSpace(orderId)) return false;
        return db.LichSu_DongA_tbs.Any(x =>
            x.taikhoan == seller
            && x.id_donhang == orderId
            && x.CongTru == true
            && x.LoaiHoSo_Vi == loaiVi);
    }

    public static bool AddSellerCreditFromOrder(dbDataContext db, string seller, string orderId, decimal amountA, int loaiVi, string note)
    {
        if (db == null) return false;
        if (string.IsNullOrWhiteSpace(seller) || string.IsNullOrWhiteSpace(orderId)) return false;
        if (amountA <= 0m) return false;
        if (HasCreditForOrder(db, seller, orderId, loaiVi)) return false;

        LichSu_DongA_tb ls = new LichSu_DongA_tb();
        ls.taikhoan = seller;
        ls.dongA = amountA;
        ls.ngay = AhaTime_cl.Now;
        ls.CongTru = true;
        ls.id_donhang = orderId;
        ls.LoaiHoSo_Vi = loaiVi;
        ls.ghichu = string.Format("{0} {1}", TagCreditSeller, (note ?? "").Trim());
        db.LichSu_DongA_tbs.InsertOnSubmit(ls);

        ApplyBalanceDelta(db, seller, loaiVi, amountA);
        return true;
    }

    public static void AddSellerDebit(dbDataContext db, string seller, decimal amountA, int loaiVi, string note, string orderId)
    {
        if (db == null) return;
        if (string.IsNullOrWhiteSpace(seller)) return;
        if (amountA <= 0m) return;

        LichSu_DongA_tb ls = new LichSu_DongA_tb();
        ls.taikhoan = seller;
        ls.dongA = amountA;
        ls.ngay = AhaTime_cl.Now;
        ls.CongTru = false;
        ls.id_donhang = orderId ?? "";
        ls.LoaiHoSo_Vi = loaiVi;
        ls.ghichu = string.Format("{0} {1}", TagRoot, (note ?? "").Trim());
        db.LichSu_DongA_tbs.InsertOnSubmit(ls);

        ApplyBalanceDelta(db, seller, loaiVi, -amountA);
    }

    private static void ApplyBalanceDelta(dbDataContext db, string seller, int loaiVi, decimal delta)
    {
        if (db == null) return;
        var acc = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == seller);
        if (acc == null) return;

        if (loaiVi == 1)
            acc.HoSo_TieuDung_ShopOnly = (acc.HoSo_TieuDung_ShopOnly ?? 0m) + delta;
        else if (loaiVi == 2)
            acc.HoSo_UuDai_ShopOnly = (acc.HoSo_UuDai_ShopOnly ?? 0m) + delta;
    }

    private static IQueryable<LichSu_DongA_tb> QueryShopOnlyEntries(dbDataContext db, string seller)
    {
        return db.LichSu_DongA_tbs.Where(x =>
            x.taikhoan == seller
            && (
                (x.ghichu != null && x.ghichu.Contains(TagRoot))
                || (x.id_donhang != null && x.id_donhang != "")
            ));
    }

    public static BalanceResult RecalculateBalances(dbDataContext db, string seller, bool updateDb)
    {
        BalanceResult result = new BalanceResult { TieuDung = 0m, UuDai = 0m, Updated = false };
        if (db == null || string.IsNullOrWhiteSpace(seller)) return result;

        var entries = QueryShopOnlyEntries(db, seller);

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

        if (updateDb)
        {
            var acc = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == seller);
            if (acc != null)
            {
                bool changed = false;
                if ((acc.HoSo_TieuDung_ShopOnly ?? 0m) != result.TieuDung)
                {
                    acc.HoSo_TieuDung_ShopOnly = result.TieuDung;
                    changed = true;
                }
                if ((acc.HoSo_UuDai_ShopOnly ?? 0m) != result.UuDai)
                {
                    acc.HoSo_UuDai_ShopOnly = result.UuDai;
                    changed = true;
                }
                result.Updated = changed;
            }
        }

        return result;
    }

    public static int BackfillSellerCredits(dbDataContext db, string seller)
    {
        if (db == null || string.IsNullOrWhiteSpace(seller)) return 0;
        int added = 0;

        var orders = db.DonHang_tbs.Where(p => p.nguoiban == seller).ToList();
        foreach (var order in orders)
        {
            DonHangStateMachine_cl.EnsureStateFields(order);

            bool creditable = order.exchange_status == DonHangStateMachine_cl.Exchange_DaTraoDoi
                || order.order_status == DonHangStateMachine_cl.Order_DaNhan
                || order.trangthai == DonHangStateMachine_cl.Exchange_DaTraoDoi;
            if (!creditable) continue;

            string orderId = order.id.ToString();
            string buyer = (order.nguoimua ?? "").Trim();
            if (string.IsNullOrEmpty(buyer)) continue;

            var buyerDebits = db.LichSu_DongA_tbs
                .Where(x => x.id_donhang == orderId
                            && x.taikhoan == buyer
                            && x.CongTru == false)
                .ToList();
            if (buyerDebits.Count == 0) continue;

            decimal tieuDung = buyerDebits.Where(x => (x.LoaiHoSo_Vi ?? 1) == 1).Sum(x => x.dongA ?? 0m);
            decimal uuDai = buyerDebits.Where(x => (x.LoaiHoSo_Vi ?? 1) == 2).Sum(x => x.dongA ?? 0m);

            if (tieuDung > 0m)
            {
                bool ok = AddSellerCreditFromOrder(db, seller, orderId, tieuDung, 1,
                    string.Format("Bán đơn hàng số {0} (Hồ sơ tiêu dùng ShopOnly) [Backfill]", orderId));
                if (ok) added++;
            }
            if (uuDai > 0m)
            {
                bool ok = AddSellerCreditFromOrder(db, seller, orderId, uuDai, 2,
                    string.Format("Bán đơn hàng số {0} (Hồ sơ ưu đãi ShopOnly) [Backfill]", orderId));
                if (ok) added++;
            }
        }

        return added;
    }
}
