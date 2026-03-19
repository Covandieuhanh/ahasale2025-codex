using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    private sealed class LichSuRawRow
    {
        public long id { get; set; }
        public DateTime? ngay { get; set; }
        public decimal? dongA { get; set; }
        public bool? CongTru { get; set; }
        public string ghichu { get; set; }
        public string id_donhang { get; set; }
        public int? KyHieu9HanhVi_1_9 { get; set; }
    }

    private static bool HasKyHieuHanhViColumn(dbDataContext db)
    {
        try
        {
            int hasCol = db.ExecuteQuery<int>(
                "SELECT CASE WHEN COL_LENGTH('dbo.LichSu_DongA_tb','KyHieu9HanhVi_1_9') IS NULL THEN 0 ELSE 1 END")
                .FirstOrDefault();
            return hasCol == 1;
        }
        catch
        {
            return false;
        }
    }

    private static List<HoSoLichSuItem_cl> QueryLichSuBySql(dbDataContext db, string taiKhoan, int loaiHoSo, int? kyHieuHanhVi, bool hasKyHieuColumn)
    {
        string selectKy = hasKyHieuColumn ? "CAST(KyHieu9HanhVi_1_9 AS INT)" : "CAST(NULL AS INT)";

        string sql;
        object[] args;
        if (hasKyHieuColumn && kyHieuHanhVi.HasValue)
        {
            sql = @"SELECT id, ngay, dongA, CongTru, ghichu, id_donhang, " + selectKy + @" AS KyHieu9HanhVi_1_9
                    FROM dbo.LichSu_DongA_tb
                    WHERE taikhoan = {0} AND LoaiHoSo_Vi = {1} AND KyHieu9HanhVi_1_9 = {2}
                    ORDER BY ngay DESC, id DESC";
            args = new object[] { taiKhoan, loaiHoSo, kyHieuHanhVi.Value };
        }
        else
        {
            sql = @"SELECT id, ngay, dongA, CongTru, ghichu, id_donhang, " + selectKy + @" AS KyHieu9HanhVi_1_9
                    FROM dbo.LichSu_DongA_tb
                    WHERE taikhoan = {0} AND LoaiHoSo_Vi = {1}
                    ORDER BY ngay DESC, id DESC";
            args = new object[] { taiKhoan, loaiHoSo };
        }

        List<LichSuRawRow> rows = db.ExecuteQuery<LichSuRawRow>(sql, args).ToList();

        return rows.Select(p => new HoSoLichSuItem_cl
        {
            id = p.id,
            ngay = p.ngay,
            dongA = p.dongA,
            CongTru = p.CongTru,
            ghichu = p.ghichu,
            id_donhang = p.id_donhang,
            KyHieu9HanhVi_1_9 = p.KyHieu9HanhVi_1_9,
            TenHanhVi = LayTenHanhVi(p.KyHieu9HanhVi_1_9)
        }).ToList();
    }

    public static List<HoSoLichSuItem_cl> LayLichSuTongHop(
        dbDataContext db,
        string taiKhoan,
        int loaiHoSo,
        int? kyHieuHanhVi = null)
    {
        string tk = (taiKhoan ?? "").Trim();
        if (db == null || tk == "")
            return new List<HoSoLichSuItem_cl>();

        bool hasKyHieuCol = HasKyHieuHanhViColumn(db);
        try
        {
            return QueryLichSuBySql(db, tk, loaiHoSo, kyHieuHanhVi, hasKyHieuCol);
        }
        catch (SqlException ex)
        {
            if (hasKyHieuCol || ex == null || string.IsNullOrEmpty(ex.Message) || !ex.Message.Contains("KyHieu9HanhVi_1_9"))
                throw;

            // Fallback an toàn cho DB cũ chưa nâng cột hanhvi.
            return QueryLichSuBySql(db, tk, loaiHoSo, kyHieuHanhVi, false);
        }
    }

    public static string LayTenHanhVi(int? kyHieu)
    {
        string text = HanhVi9Cap_cl.GetTenHanhViTheoLoai(kyHieu);
        return text == "-" ? "" : text;
    }
}
