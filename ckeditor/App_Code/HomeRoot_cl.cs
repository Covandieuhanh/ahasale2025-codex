using System;
using System.Configuration;
using System.Linq;

public static class HomeRoot_cl
{
    public const string DefaultHomeRootAccount = "home_root";
    public const string DefaultHomeRootPassword = "123123";

    public static string GetConfiguredAccountName()
    {
        string raw = ConfigurationManager.AppSettings["Home.RootAccount"];
        string name = (raw ?? "").Trim();
        if (name == "") name = DefaultHomeRootAccount;
        return name;
    }

    public static bool IsHomeRoot(string taiKhoan)
    {
        return string.Equals(
            (taiKhoan ?? "").Trim(),
            GetConfiguredAccountName(),
            StringComparison.OrdinalIgnoreCase);
    }

    public static taikhoan_tb GetOrCreate(dbDataContext db, out bool created)
    {
        created = false;
        if (db == null) return null;

        string tk = GetConfiguredAccountName();
        taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
        if (acc != null) return acc;

        acc = new taikhoan_tb();
        acc.taikhoan = tk;
        acc.matkhau = DefaultHomeRootPassword;
        acc.hoten = "Home Root";
        acc.ten = "HOME ROOT";
        acc.hoten_khongdau = "home-root";
        acc.phanloai = "Khách hàng";
        acc.permission = PortalScope_cl.NormalizePermissionWithScope("", PortalScope_cl.ScopeHome);
        acc.block = false;
        acc.nguoitao = "system";
        acc.ngaytao = AhaTime_cl.Now;
        acc.hansudung = AhaTime_cl.Now.AddYears(50);
        acc.DongA = 0m;
        acc.HeThongSanPham_Cap123 = 0;
        acc.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 = null;
        acc.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 = null;
        acc.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 = null;

        db.taikhoan_tbs.InsertOnSubmit(acc);
        created = true;
        return acc;
    }
}
