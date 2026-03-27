using System;
using System.Collections.Generic;
using System.Linq;

public static class AccountVisibility_cl
{
    public static bool IsBlocked(taikhoan_tb account)
    {
        return account != null && account.block == true;
    }

    public static bool CanToggleHomeLock(taikhoan_tb account)
    {
        if (account == null)
            return false;
        if (PermissionProfile_cl.IsRootAdmin(account.taikhoan))
            return false;
        return PortalScope_cl.CanLoginHome(account.taikhoan, account.phanloai, account.permission);
    }

    public static bool CanLoginHomeByPhone(taikhoan_tb account)
    {
        if (account == null)
            return false;
        if (!PortalScope_cl.CanLoginHome(account.taikhoan, account.phanloai, account.permission))
            return false;

        bool byPhoneColumn = AccountAuth_cl.IsValidPhone(account.dienthoai);
        bool byLegacyUsername = AccountAuth_cl.IsValidPhone(account.taikhoan);
        return byPhoneColumn || byLegacyUsername;
    }

    public static bool IsSellerVisible(dbDataContext db, string taiKhoan)
    {
        if (db == null)
            return false;

        string tk = (taiKhoan ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(tk))
            return false;

        return db.taikhoan_tbs.Any(p => p.taikhoan == tk && p.block != true);
    }

    public static IQueryable<BaiViet_tb> FilterVisibleProducts(dbDataContext db, IQueryable<BaiViet_tb> source)
    {
        if (db == null || source == null)
            return source;

        return source.Where(p =>
            p.phanloai == "sanpham"
            && p.bin == false
            && db.taikhoan_tbs.Any(acc => acc.taikhoan == p.nguoitao && acc.block != true));
    }

    public static BaiViet_tb FindVisibleProductById(dbDataContext db, string idsp)
    {
        if (db == null)
            return null;

        string id = (idsp ?? "").Trim();
        if (string.IsNullOrEmpty(id))
            return null;

        return db.BaiViet_tbs.FirstOrDefault(p =>
            p.id.ToString() == id
            && p.phanloai == "sanpham"
            && p.bin == false
            && db.taikhoan_tbs.Any(acc => acc.taikhoan == p.nguoitao && acc.block != true));
    }

    public static int LockLegacyHomeAccountsWithoutPhone(dbDataContext db, out int hiddenPostCount)
    {
        hiddenPostCount = 0;
        if (db == null)
            return 0;

        List<taikhoan_tb> allAccounts = db.taikhoan_tbs.ToList();
        List<string> targets = allAccounts
            .Where(CanToggleHomeLock)
            .Where(p => p.block != true)
            .Where(p => !CanLoginHomeByPhone(p))
            .Select(p => (p.taikhoan ?? "").Trim().ToLowerInvariant())
            .Where(p => p != "")
            .Distinct()
            .ToList();

        if (targets.Count == 0)
            return 0;

        hiddenPostCount = db.BaiViet_tbs.Count(p =>
            p.phanloai == "sanpham"
            && p.bin == false
            && targets.Contains((p.nguoitao ?? "").Trim().ToLower()));

        foreach (taikhoan_tb acc in allAccounts)
        {
            string tk = (acc.taikhoan ?? "").Trim().ToLowerInvariant();
            if (targets.Contains(tk))
                acc.block = true;
        }

        db.SubmitChanges();
        return targets.Count;
    }
}
