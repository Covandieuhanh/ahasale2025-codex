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

    public static IQueryable<GH_SanPham_tb> QueryByShop(dbDataContext db, string shopTaiKhoan)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        return db.GetTable<GH_SanPham_tb>().Where(p => (p.shop_taikhoan ?? "").Trim().ToLower() == tk);
    }

    public static IQueryable<GH_SanPham_tb> QueryPublicByShop(dbDataContext db, string shopTaiKhoan)
    {
        return QueryByShop(db, shopTaiKhoan).Where(p => p.bin != true);
    }

    public static GH_SanPham_tb GetById(dbDataContext db, int id, string shopTaiKhoan)
    {
        return QueryByShop(db, shopTaiKhoan).FirstOrDefault(p => p.id == id);
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
        string loai,
        string idDanhMuc,
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
            gh = table.FirstOrDefault(p => p.id == id.Value && (p.shop_taikhoan ?? "").Trim().ToLower() == tk);
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
        gh.bin = isHidden;
        gh.ngay_cap_nhat = AhaTime_cl.Now;
        gh.so_luong_ton = soLuongTon ?? gh.so_luong_ton ?? 0;
        if (gh.so_luong_da_ban == null) gh.so_luong_da_ban = 0;
        if (gh.luot_truy_cap == null) gh.luot_truy_cap = 0;

        db.SubmitChanges();
        SyncToHome(db, gh);
        return gh;
    }

    public static bool ToggleVisibility(dbDataContext db, int id, string shopTaiKhoan)
    {
        EnsureSchema(db);
        string tk = (shopTaiKhoan ?? "").Trim().ToLowerInvariant();
        var gh = db.GetTable<GH_SanPham_tb>()
            .FirstOrDefault(p => p.id == id && (p.shop_taikhoan ?? "").Trim().ToLower() == tk);
        if (gh == null)
            return false;

        gh.bin = !(gh.bin ?? false);
        gh.ngay_cap_nhat = AhaTime_cl.Now;
        db.SubmitChanges();
        SyncToHome(db, gh);
        return true;
    }

    public static bool SyncToHome(dbDataContext db, GH_SanPham_tb gh)
    {
        if (db == null || gh == null)
            return false;

        string loai = NormalizeLoai(gh.loai);
        string phanLoai = loai == LoaiDichVu ? AccountVisibility_cl.PostTypeService : AccountVisibility_cl.PostTypeProduct;
        BaiViet_tb bv = null;

        if (gh.id_baiviet.HasValue)
        {
            int idBv = gh.id_baiviet.Value;
            bv = db.BaiViet_tbs.FirstOrDefault(p => p.id == idBv);
        }

        if (bv == null)
        {
            bv = new BaiViet_tb();
            db.BaiViet_tbs.InsertOnSubmit(bv);
        }

        String_cl str = new String_cl();
        string ten = (gh.ten ?? "").Trim();

        bv.name = ten;
        bv.name_en = string.IsNullOrEmpty(ten) ? "" : str.replace_name_to_url(ten);
        bv.id_DanhMuc = gh.id_danhmuc;
        bv.content_post = gh.noi_dung;
        bv.description = gh.mo_ta;
        bv.image = gh.hinh_anh;
        bv.bin = gh.bin ?? false;
        bv.ngaytao = gh.ngay_tao ?? AhaTime_cl.Now;
        bv.nguoitao = gh.shop_taikhoan;
        bv.noibat = false;
        bv.giaban = gh.gia_ban;
        bv.giavon = gh.gia_von;
        bv.soluong_tonkho = gh.so_luong_ton ?? 0;
        bv.soluong_daban = gh.so_luong_da_ban ?? 0;
        bv.LuotTruyCap = gh.luot_truy_cap ?? 0;
        bv.phanloai = phanLoai;

        db.SubmitChanges();

        if (!gh.id_baiviet.HasValue && bv.id > 0)
        {
            gh.id_baiviet = bv.id;
            db.SubmitChanges();
        }

        return true;
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
}
