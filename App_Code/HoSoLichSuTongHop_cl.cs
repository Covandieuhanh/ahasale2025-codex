using System;
using System.Collections.Generic;
using System.Linq;

public class HoSoLichSuItem_cl
{
    public long id { get; set; }
    public DateTime? ngay { get; set; }
    public decimal? dongA { get; set; }
    public bool? CongTru { get; set; }
    public string ghichu { get; set; }
    public string id_donhang { get; set; }
    public int? KyHieu9HanhVi_1_9 { get; set; }
    public string TenHanhVi { get; set; }
}

public static class HoSoLichSuTongHop_cl
{
    public static List<HoSoLichSuItem_cl> LayLichSuTongHop(
        dbDataContext db,
        string taiKhoan,
        int loaiHoSo,
        int? kyHieuHanhVi = null)
    {
        string tk = (taiKhoan ?? "").Trim();
        if (db == null || tk == "")
            return new List<HoSoLichSuItem_cl>();

        var records = db.LichSu_DongA_tbs
            .Where(p => p.taikhoan == tk && p.LoaiHoSo_Vi == loaiHoSo)
            .OrderByDescending(p => p.ngay)
            .ThenByDescending(p => p.id)
            .ToList();

        if (kyHieuHanhVi.HasValue)
            records = records.Where(p => p.KyHieu9HanhVi_1_9 == kyHieuHanhVi.Value).ToList();

        var list = records.Select(p => new HoSoLichSuItem_cl
        {
            id = p.id,
            ngay = p.ngay,
            dongA = p.dongA,
            CongTru = p.CongTru,
            ghichu = p.ghichu,
            id_donhang = p.id_donhang,
            KyHieu9HanhVi_1_9 = p.KyHieu9HanhVi_1_9
        }).ToList();

        foreach (var item in list)
            item.TenHanhVi = LayTenHanhVi(item.KyHieu9HanhVi_1_9);

        return list;
    }

    public static string LayTenHanhVi(int? kyHieu)
    {
        string text = HanhVi9Cap_cl.GetTenHanhViTheoLoai(kyHieu);
        return text == "-" ? "" : text;
    }
}
