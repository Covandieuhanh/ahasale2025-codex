using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public static class GianHangProduct_cl
{
    public const string LoaiSanPham = "sanpham";
    public const string LoaiDichVu = "dichvu";

    public static void EnsureSchema(dbDataContext db)
    {
        GianHangSchema_cl.EnsureSchemaSafe(db);
    }

    public static string NormalizeLoai(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        if (value == LoaiDichVu)
            return LoaiDichVu;
        return LoaiSanPham;
    }

    public static IQueryable<GH_SanPham_tb> QueryByStorefront(dbDataContext db, string storefrontAccount)
    {
        EnsureSchema(db);
        string tk = (storefrontAccount ?? "").Trim().ToLowerInvariant();
        return db.GetTable<GH_SanPham_tb>().Where(p => p.shop_taikhoan == tk);
    }

    public static IQueryable<GH_SanPham_tb> QueryPublicByStorefront(dbDataContext db, string storefrontAccount)
    {
        return QueryByStorefront(db, storefrontAccount).Where(p => p.bin == null || p.bin == false);
    }

    public static GH_SanPham_tb GetById(dbDataContext db, int id, string storefrontAccount)
    {
        return QueryByStorefront(db, storefrontAccount).FirstOrDefault(p => p.id == id);
    }

    // Compatibility aliases for older call sites during migration.
    public static IQueryable<GH_SanPham_tb> QueryByShop(dbDataContext db, string shopTaiKhoan)
    {
        return QueryByStorefront(db, shopTaiKhoan);
    }

    public static IQueryable<GH_SanPham_tb> QueryPublicByShop(dbDataContext db, string shopTaiKhoan)
    {
        return QueryPublicByStorefront(db, shopTaiKhoan);
    }

    public static GH_SanPham_tb Save(dbDataContext db,
        int? id,
        string shopTaiKhoan,
        string ten,
        string moTa,
        string noiDung,
        string hinhAnh,
        decimal? giaBan,
        long? giaVon,
        int? soLuongTon,
        int? phanTramUuDai,
        string loai,
        string idDanhMuc,
        string diaDiem,
        string diaChiTinh,
        string diaChiQuan,
        string diaChiPhuong,
        string diaChiChiTiet,
        bool isHidden)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(tk))
            return null;

        var table = db.GetTable<GH_SanPham_tb>();
        GH_SanPham_tb gh;
        if (id.HasValue && id.Value > 0)
        {
            gh = table.FirstOrDefault(p => p.id == id.Value && p.shop_taikhoan == tk);
            if (gh == null)
                return null;
        }
        else
        {
            gh = new GH_SanPham_tb
            {
                shop_taikhoan = tk,
                ngay_tao = AhaTime_cl.Now
            };
            table.InsertOnSubmit(gh);
        }

        gh.ten = (ten ?? "").Trim();
        gh.mo_ta = moTa ?? "";
        gh.noi_dung = noiDung ?? "";
        gh.hinh_anh = hinhAnh ?? "";
        gh.gia_ban = giaBan;
        gh.gia_von = giaVon;
        gh.loai = NormalizeLoai(loai);
        gh.id_danhmuc = string.IsNullOrWhiteSpace(idDanhMuc) ? null : idDanhMuc.Trim();
        gh.dia_diem = string.IsNullOrWhiteSpace(diaDiem) ? null : diaDiem.Trim();
        gh.dia_chi_tinh = string.IsNullOrWhiteSpace(diaChiTinh) ? null : diaChiTinh.Trim();
        gh.dia_chi_quan = string.IsNullOrWhiteSpace(diaChiQuan) ? null : diaChiQuan.Trim();
        gh.dia_chi_phuong = string.IsNullOrWhiteSpace(diaChiPhuong) ? null : diaChiPhuong.Trim();
        gh.dia_chi_chi_tiet = string.IsNullOrWhiteSpace(diaChiChiTiet) ? null : diaChiChiTiet.Trim();
        gh.bin = isHidden;
        gh.ngay_cap_nhat = AhaTime_cl.Now;
        gh.so_luong_ton = soLuongTon ?? gh.so_luong_ton ?? 0;
        if (gh.so_luong_da_ban == null) gh.so_luong_da_ban = 0;
        if (gh.luot_truy_cap == null) gh.luot_truy_cap = 0;
        gh.phan_tram_uu_dai = ClampDiscountPercent(phanTramUuDai);

        db.SubmitChanges();
        TryEnsureWorkspaceMirror(db, tk);
        return gh;
    }

    public static bool ToggleVisibility(dbDataContext db, int id, string shopTaiKhoan)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        var gh = db.GetTable<GH_SanPham_tb>()
            .FirstOrDefault(p => p.id == id && p.shop_taikhoan == tk);
        if (gh == null)
            return false;

        gh.bin = gh.bin != true;
        gh.ngay_cap_nhat = AhaTime_cl.Now;
        db.SubmitChanges();
        TryEnsureWorkspaceMirror(db, tk);
        return true;
    }

    public static bool TrySyncToHome(dbDataContext db, GH_SanPham_tb gh)
    {
        return false;
    }

    public static bool SyncToHome(dbDataContext db, GH_SanPham_tb gh)
    {
        return false;
    }

    public static decimal ParseGia(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return 0m;

        decimal value;
        if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.GetCultureInfo("vi-VN"), out value))
            return value;

        string cleaned = raw.Replace(".", "").Replace(",", "");
        decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        return value;
    }

    public static long ParseGiaVon(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return 0;

        long value;
        if (long.TryParse(raw.Replace(".", "").Replace(",", ""), out value))
            return value;

        return 0;
    }

    public static int ClampDiscountPercent(int? value)
    {
        int percent = value ?? 0;
        if (percent < 0) percent = 0;
        if (percent > 50) percent = 50;
        return percent;
    }

    public static int ResolveDiscountPercent(GH_SanPham_tb product)
    {
        return product == null ? 0 : ClampDiscountPercent(product.phan_tram_uu_dai);
    }

    private static void TryEnsureWorkspaceMirror(dbDataContext db, string shopTaiKhoan)
    {
        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        if (db == null || tk == "")
            return;

        try
        {
            GianHangWorkspaceLink_cl.EnsureWorkspaceLinked(db, tk, true);
        }
        catch
        {
            // Không làm gián đoạn luồng lưu tin nếu bước bridge sang Home gặp lỗi tạm thời.
        }
    }
}
