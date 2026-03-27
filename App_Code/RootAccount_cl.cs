using System;
using System.Collections.Generic;
using System.Linq;

public static class RootAccount_cl
{
    public sealed class RootAccountInfo
    {
        public string AccountKey { get; set; }
        public string FullName { get; set; }
        public string AccountType { get; set; }
        public string LegacyPermission { get; set; }
        public string Email { get; set; }
        public string ShopEmail { get; set; }
        public string ShopName { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool CanAccessHome { get; set; }
        public bool CanAccessGianHang { get; set; }
        public bool CanAccessShop { get; set; }
        public bool CanAccessAdmin { get; set; }
        public bool CanAccessGianHangAdmin { get; set; }
        public bool CanAccessDauGia { get; set; }
        public bool CanAccessEvent { get; set; }
        public string DefaultSpace { get; set; }
    }

    public static string GetCurrentAccountKey()
    {
        return (PortalRequest_cl.GetCurrentAccount() ?? "").Trim();
    }

    public static taikhoan_tb GetCurrentEntity()
    {
        string accountKey = GetCurrentAccountKey();
        if (accountKey == "")
            return null;

        return CoreDb_cl.Use(db => GetByAccountKey(db, accountKey));
    }

    public static taikhoan_tb GetCurrentEntity(dbDataContext db)
    {
        string accountKey = GetCurrentAccountKey();
        if (accountKey == "")
            return null;

        return GetByAccountKey(db, accountKey);
    }

    public static taikhoan_tb GetByAccountKey(dbDataContext db, string accountKey)
    {
        if (db == null)
            return null;

        string key = (accountKey ?? "").Trim();
        if (key == "")
            return null;

        return db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == key);
    }

    public static RootAccountInfo GetCurrentInfo()
    {
        string accountKey = GetCurrentAccountKey();
        if (accountKey == "")
        {
            return new RootAccountInfo
            {
                AccountKey = "",
                IsAuthenticated = false,
                DefaultSpace = ModuleSpace_cl.Home
            };
        }

        return CoreDb_cl.Use(db => GetInfo(db, accountKey));
    }

    public static RootAccountInfo GetInfo(string accountKey)
    {
        return CoreDb_cl.Use(db => GetInfo(db, accountKey));
    }

    public static RootAccountInfo GetInfo(dbDataContext db, string accountKey)
    {
        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        taikhoan_tb account = GetByAccountKey(db, accountKey);
        if (account == null)
        {
            return new RootAccountInfo
            {
                AccountKey = (accountKey ?? "").Trim(),
                IsAuthenticated = false,
                DefaultSpace = ModuleSpace_cl.Home
            };
        }

        CoreAccessBootstrap_cl.EnsureAccountSeeded(db, account);

        bool canHome = SpaceAccess_cl.CanAccessHome(db, account);
        bool canGianHang = SpaceAccess_cl.CanAccessGianHang(db, account);
        bool canShop = SpaceAccess_cl.CanAccessShop(db, account);
        bool canAdmin = SpaceAccess_cl.CanAccessAdmin(db, account);
        bool canGianHangAdmin = SpaceAccess_cl.CanAccessGianHangAdmin(db, account);
        if (!canGianHangAdmin && canHome)
            canGianHangAdmin = GianHangAdminWorkspace_cl.HasAnyWorkspace(db, account.taikhoan);
        bool canDauGia = SpaceAccess_cl.CanAccessDauGia(db, account);
        bool canEvent = SpaceAccess_cl.CanAccessEvent(db, account);

        return new RootAccountInfo
        {
            AccountKey = (account.taikhoan ?? "").Trim(),
            FullName = (account.hoten ?? "").Trim(),
            AccountType = (account.phanloai ?? "").Trim(),
            LegacyPermission = (account.permission ?? "").Trim(),
            Email = (account.email ?? "").Trim(),
            ShopEmail = (account.email_shop ?? "").Trim(),
            ShopName = (account.ten_shop ?? "").Trim(),
            IsAuthenticated = true,
            CanAccessHome = canHome,
            CanAccessGianHang = canGianHang,
            CanAccessShop = canShop,
            CanAccessAdmin = canAdmin,
            CanAccessGianHangAdmin = canGianHangAdmin,
            CanAccessDauGia = canDauGia,
            CanAccessEvent = canEvent,
            DefaultSpace = ResolveDefaultSpace(canHome, canGianHang, canShop, canAdmin, canGianHangAdmin, canDauGia, canEvent)
        };
    }

    public static string ResolveContactEmail(RootAccountInfo info)
    {
        if (info == null)
            return "";

        string email = (info.Email ?? "").Trim();
        if (email != "")
            return email;

        return (info.ShopEmail ?? "").Trim();
    }

    public static IList<string> GetAvailableSpaces(RootAccountInfo info)
    {
        List<string> spaces = new List<string>();
        if (info == null)
            return spaces;

        if (info.CanAccessHome) spaces.Add(ModuleSpace_cl.Home);
        if (info.CanAccessGianHang) spaces.Add(ModuleSpace_cl.GianHang);
        if (info.CanAccessShop) spaces.Add(ModuleSpace_cl.Shop);
        if (info.CanAccessAdmin) spaces.Add(ModuleSpace_cl.Admin);
        if (info.CanAccessGianHangAdmin) spaces.Add(ModuleSpace_cl.GianHangAdmin);
        if (info.CanAccessDauGia) spaces.Add(ModuleSpace_cl.DauGia);
        if (info.CanAccessEvent) spaces.Add(ModuleSpace_cl.Event);
        return spaces;
    }

    private static string ResolveDefaultSpace(bool canHome, bool canGianHang, bool canShop, bool canAdmin, bool canGianHangAdmin, bool canDauGia, bool canEvent)
    {
        if (canHome) return ModuleSpace_cl.Home;
        if (canGianHang) return ModuleSpace_cl.GianHang;
        if (canShop) return ModuleSpace_cl.Shop;
        if (canDauGia) return ModuleSpace_cl.DauGia;
        if (canEvent) return ModuleSpace_cl.Event;
        if (canGianHangAdmin) return ModuleSpace_cl.GianHangAdmin;
        if (canAdmin) return ModuleSpace_cl.Admin;
        return ModuleSpace_cl.Home;
    }
}
