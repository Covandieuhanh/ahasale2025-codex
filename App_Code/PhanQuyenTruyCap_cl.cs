using System;
using System.Collections.Generic;
using System.Linq;

public static class PhanQuyenTruyCap_cl
{
    public const string KhongGianHome = "home";
    public const string KhongGianShop = "shop";
    public const string KhongGianAdmin = "admin";
    public const string KhongGianDauGia = "daugia";
    public const string KhongGianGianHangAdmin = "gianhang_admin";

    public static string ChuanHoaKhongGian(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        if (value == KhongGianHome) return KhongGianHome;
        if (value == KhongGianShop) return KhongGianShop;
        if (value == KhongGianAdmin) return KhongGianAdmin;
        if (value == KhongGianDauGia) return KhongGianDauGia;
        if (value == KhongGianGianHangAdmin) return KhongGianGianHangAdmin;
        return "";
    }

    public static IEnumerable<string> TachMaQuyen(string quyenRaw)
    {
        return PortalScope_cl.SplitPermissionTokens(quyenRaw)
            .Select(p => (p ?? "").Trim().ToLowerInvariant())
            .Where(p => p != "");
    }

    public static bool CoMaQuyen(string quyenRaw, params string[] maQuyen)
    {
        HashSet<string> tap = new HashSet<string>(TachMaQuyen(quyenRaw), StringComparer.OrdinalIgnoreCase);
        if (tap.Contains("all"))
            return true;

        if (maQuyen == null || maQuyen.Length == 0)
            return false;

        for (int i = 0; i < maQuyen.Length; i++)
        {
            string ma = (maQuyen[i] ?? "").Trim();
            if (ma == "")
                continue;

            if (tap.Contains(ma))
                return true;
        }

        return false;
    }

    public static bool CoTheVaoHome(taikhoan_tb taiKhoan)
    {
        if (taiKhoan == null)
            return false;

        return PortalScope_cl.CanLoginHome(taiKhoan.taikhoan, taiKhoan.phanloai, taiKhoan.permission);
    }

    public static bool CoTheVaoShop(taikhoan_tb taiKhoan)
    {
        if (taiKhoan == null)
            return false;

        return PortalScope_cl.CanLoginShop(taiKhoan.taikhoan, taiKhoan.phanloai, taiKhoan.permission);
    }

    public static bool CoTheVaoAdmin(taikhoan_tb taiKhoan)
    {
        if (taiKhoan == null)
            return false;

        return PortalScope_cl.CanLoginAdmin(taiKhoan.taikhoan, taiKhoan.phanloai, taiKhoan.permission);
    }

    public static bool CoTheVaoDauGia(taikhoan_tb taiKhoan)
    {
        if (taiKhoan == null)
            return false;

        return CoMaQuyen(taiKhoan.permission, "portal_daugia", "daugia", "mod_daugia");
    }

    public static bool CoTheVaoGianHangAdmin(taikhoan_tb taiKhoan)
    {
        if (taiKhoan == null)
            return false;

        return CoMaQuyen(taiKhoan.permission, "portal_gianhang_admin", "gianhang_admin", "mod_gianhang_admin");
    }

    public static string DoanKhongGianTheoDuongDan(string duongDan)
    {
        string path = (duongDan ?? "").Trim();
        if (path == "")
            return "";

        if (path.StartsWith("/admin/", StringComparison.OrdinalIgnoreCase))
            return KhongGianAdmin;
        if (path.StartsWith("/shop/", StringComparison.OrdinalIgnoreCase))
            return KhongGianShop;
        if (path.StartsWith("/daugia/", StringComparison.OrdinalIgnoreCase))
            return KhongGianDauGia;
        if (path.StartsWith("/gianhang/admin/", StringComparison.OrdinalIgnoreCase))
            return KhongGianGianHangAdmin;
        if (path.StartsWith("/home/", StringComparison.OrdinalIgnoreCase))
            return KhongGianHome;

        return "";
    }

    public static bool CoTheVaoKhongGian(taikhoan_tb taiKhoan, string khongGian)
    {
        switch (ChuanHoaKhongGian(khongGian))
        {
            case KhongGianHome:
                return CoTheVaoHome(taiKhoan);
            case KhongGianShop:
                return CoTheVaoShop(taiKhoan);
            case KhongGianAdmin:
                return CoTheVaoAdmin(taiKhoan);
            case KhongGianDauGia:
                return CoTheVaoDauGia(taiKhoan);
            case KhongGianGianHangAdmin:
                return CoTheVaoGianHangAdmin(taiKhoan);
            default:
                return false;
        }
    }

    public static bool CoTheVaoTrang(taikhoan_tb taiKhoan, string duongDan)
    {
        string khongGian = DoanKhongGianTheoDuongDan(duongDan);
        if (khongGian == "")
            return false;

        return CoTheVaoKhongGian(taiKhoan, khongGian);
    }

    public static string LayThongDiepKhongDuQuyen(string khongGian)
    {
        switch (ChuanHoaKhongGian(khongGian))
        {
            case KhongGianShop:
                return "Tai khoan chua du quyen vao khong gian gian hang doi tac.";
            case KhongGianAdmin:
                return "Tai khoan chua du quyen vao khong gian quan tri.";
            case KhongGianDauGia:
                return "Tai khoan chua du quyen vao khong gian dau gia.";
            case KhongGianGianHangAdmin:
                return "Tai khoan chua du quyen vao khong gian quan tri gian hang.";
            case KhongGianHome:
                return "Tai khoan chua du quyen vao khong gian ca nhan.";
            default:
                return "Tai khoan chua du quyen truy cap.";
        }
    }
}

