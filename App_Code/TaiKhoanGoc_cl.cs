using System;
using System.Linq;

public static class TaiKhoanGoc_cl
{
    public sealed class ThongTinTaiKhoanGoc
    {
        public string TaiKhoan { get; set; }
        public string HoTen { get; set; }
        public string PhanLoai { get; set; }
        public string Quyen { get; set; }
        public string Email { get; set; }
        public string EmailShop { get; set; }
        public string TenShop { get; set; }
        public bool DangDangNhap { get; set; }
        public bool CoTheVaoHome { get; set; }
        public bool CoTheVaoShop { get; set; }
        public bool CoTheVaoAdmin { get; set; }
        public bool CoTheVaoDauGia { get; set; }
        public bool CoTheVaoGianHangAdmin { get; set; }
        public string KhongGianMacDinh { get; set; }
    }

    public static string LayTaiKhoanHienTai()
    {
        return (PortalRequest_cl.GetCurrentAccount() ?? "").Trim();
    }

    public static taikhoan_tb LayBanGhiTaiKhoanHienTai()
    {
        string taiKhoan = LayTaiKhoanHienTai();
        if (taiKhoan == "")
            return null;

        return NguonDuLieuGoc_cl.ThucThi(db => LayBanGhiTaiKhoan(db, taiKhoan));
    }

    public static taikhoan_tb LayBanGhiTaiKhoan(dbDataContext db, string taiKhoan)
    {
        if (db == null)
            return null;

        string key = (taiKhoan ?? "").Trim();
        if (key == "")
            return null;

        return db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == key);
    }

    public static ThongTinTaiKhoanGoc LayThongTinHienTai()
    {
        string taiKhoan = LayTaiKhoanHienTai();
        if (taiKhoan == "")
            return new ThongTinTaiKhoanGoc
            {
                TaiKhoan = "",
                DangDangNhap = false,
                KhongGianMacDinh = PhanQuyenTruyCap_cl.KhongGianHome
            };

        return NguonDuLieuGoc_cl.ThucThi(db => LayThongTin(db, taiKhoan));
    }

    public static ThongTinTaiKhoanGoc LayThongTin(string taiKhoan)
    {
        return NguonDuLieuGoc_cl.ThucThi(db => LayThongTin(db, taiKhoan));
    }

    public static ThongTinTaiKhoanGoc LayThongTin(dbDataContext db, string taiKhoan)
    {
        taikhoan_tb banGhi = LayBanGhiTaiKhoan(db, taiKhoan);
        if (banGhi == null)
            return new ThongTinTaiKhoanGoc
            {
                TaiKhoan = (taiKhoan ?? "").Trim(),
                DangDangNhap = false,
                KhongGianMacDinh = PhanQuyenTruyCap_cl.KhongGianHome
            };

        bool coHome = PhanQuyenTruyCap_cl.CoTheVaoHome(banGhi);
        bool coShop = PhanQuyenTruyCap_cl.CoTheVaoShop(banGhi);
        bool coAdmin = PhanQuyenTruyCap_cl.CoTheVaoAdmin(banGhi);
        bool coDauGia = PhanQuyenTruyCap_cl.CoTheVaoDauGia(banGhi);
        bool coGianHangAdmin = PhanQuyenTruyCap_cl.CoTheVaoGianHangAdmin(banGhi);

        return new ThongTinTaiKhoanGoc
        {
            TaiKhoan = (banGhi.taikhoan ?? "").Trim(),
            HoTen = (banGhi.hoten ?? "").Trim(),
            PhanLoai = (banGhi.phanloai ?? "").Trim(),
            Quyen = (banGhi.permission ?? "").Trim(),
            Email = (banGhi.email ?? "").Trim(),
            EmailShop = (banGhi.email_shop ?? "").Trim(),
            TenShop = (banGhi.ten_shop ?? "").Trim(),
            DangDangNhap = true,
            CoTheVaoHome = coHome,
            CoTheVaoShop = coShop,
            CoTheVaoAdmin = coAdmin,
            CoTheVaoDauGia = coDauGia,
            CoTheVaoGianHangAdmin = coGianHangAdmin,
            KhongGianMacDinh = XacDinhKhongGianMacDinh(coHome, coShop, coAdmin, coDauGia, coGianHangAdmin)
        };
    }

    public static string LayEmailLienHe(ThongTinTaiKhoanGoc thongTin)
    {
        if (thongTin == null)
            return "";

        string email = (thongTin.Email ?? "").Trim();
        if (email != "")
            return email;

        return (thongTin.EmailShop ?? "").Trim();
    }

    private static string XacDinhKhongGianMacDinh(bool coHome, bool coShop, bool coAdmin, bool coDauGia, bool coGianHangAdmin)
    {
        if (coHome) return PhanQuyenTruyCap_cl.KhongGianHome;
        if (coShop) return PhanQuyenTruyCap_cl.KhongGianShop;
        if (coDauGia) return PhanQuyenTruyCap_cl.KhongGianDauGia;
        if (coGianHangAdmin) return PhanQuyenTruyCap_cl.KhongGianGianHangAdmin;
        if (coAdmin) return PhanQuyenTruyCap_cl.KhongGianAdmin;
        return PhanQuyenTruyCap_cl.KhongGianHome;
    }
}

