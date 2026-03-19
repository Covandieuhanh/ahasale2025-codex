using System;
using System.Collections.Generic;
using System.Linq;

public class HanhViUuDaiRowBalance_cl
{
    public int KyHieu9HanhVi_1_9 { get; set; }
    public string TenHanhVi { get; set; }
    public decimal SoDuDiemNhan { get; set; }
    public decimal SoDuHanhViHopLe { get; set; }
    public decimal SoDuDangChoDuyet { get; set; }
    public decimal SoDaGhiNhan { get; set; }
    public bool CoTheGui
    {
        get { return SoDuHanhViHopLe > 0m; }
    }
}

public class HanhViUuDaiSummary_cl
{
    public decimal SoDuTrongHoSo { get; set; }
    public decimal TongSoDuDiemNhan { get; set; }
    public decimal TongSoDuHanhViHopLe { get; set; }
    public decimal TongSoDuDangChoDuyet { get; set; }
    public decimal TongSoDaGhiNhan { get; set; }
    public int SoPhutChoHopLe { get; set; }
    public DateTime MocHopLe { get; set; }
    public List<HanhViUuDaiRowBalance_cl> Rows { get; set; }

    public HanhViUuDaiSummary_cl()
    {
        Rows = new List<HanhViUuDaiRowBalance_cl>();
    }
}

public static class HanhViGhiNhanUuDai_cl
{
    public const int ProfileCode_UuDai = HanhViGhiNhanHoSo_cl.Profile_UuDai;

    public const string TrangThaiChoDuyet = HanhViGhiNhanHoSo_cl.TrangThaiChoDuyet;
    public const string TrangThaiDaDuyet = HanhViGhiNhanHoSo_cl.TrangThaiDaDuyet;
    public const string TrangThaiTuChoi = HanhViGhiNhanHoSo_cl.TrangThaiTuChoi;

    public const string DbTrangThaiChoDuyet = HanhViGhiNhanHoSo_cl.DbTrangThaiChoDuyet;
    public const string DbTrangThaiDaDuyet = HanhViGhiNhanHoSo_cl.DbTrangThaiDaDuyet;
    public const string DbTrangThaiTuChoi = HanhViGhiNhanHoSo_cl.DbTrangThaiTuChoi;

    public static int GetSoPhutChoHopLe()
    {
        return HanhViGhiNhanHoSo_cl.GetSoPhutChoHopLe(ProfileCode_UuDai);
    }

    public static string NormalizeTrangThai(string trangThai)
    {
        return HanhViGhiNhanHoSo_cl.NormalizeTrangThai(trangThai);
    }

    public static string GetTrangThaiText(string trangThai)
    {
        return HanhViGhiNhanHoSo_cl.GetTrangThaiText(trangThai);
    }

    public static bool IsHanhViUuDai(int kyHieu9HanhVi)
    {
        return HanhViGhiNhanHoSo_cl.IsHanhViThuocHoSo(ProfileCode_UuDai, kyHieu9HanhVi);
    }

    private static HanhViUuDaiSummary_cl ToLegacySummary(HanhViHoSoSummary_cl source)
    {
        HanhViUuDaiSummary_cl legacy = new HanhViUuDaiSummary_cl();
        if (source == null)
            return legacy;

        legacy.SoDuTrongHoSo = source.SoDuTrongHoSo;
        legacy.TongSoDuDiemNhan = source.TongSoDuDiemNhan;
        legacy.TongSoDuHanhViHopLe = source.TongSoDuHanhViHopLe;
        legacy.TongSoDuDangChoDuyet = source.TongSoDuDangChoDuyet;
        legacy.TongSoDaGhiNhan = source.TongSoDaGhiNhan;
        legacy.SoPhutChoHopLe = source.SoPhutChoHopLe;
        legacy.MocHopLe = source.MocHopLe;
        legacy.Rows = source.Rows.Select(p => new HanhViUuDaiRowBalance_cl
        {
            KyHieu9HanhVi_1_9 = p.KyHieu9HanhVi_1_9,
            TenHanhVi = p.TenHanhVi,
            SoDuDiemNhan = p.SoDuDiemNhan,
            SoDuHanhViHopLe = p.SoDuHanhViHopLe,
            SoDuDangChoDuyet = p.SoDuDangChoDuyet,
            SoDaGhiNhan = p.SoDaGhiNhan
        }).ToList();
        return legacy;
    }

    public static HanhViUuDaiSummary_cl TinhTongHop(dbDataContext db, string taiKhoan, DateTime? nowOverride)
    {
        HanhViHoSoSummary_cl source = HanhViGhiNhanHoSo_cl.TinhTongHop(db, taiKhoan, ProfileCode_UuDai, nowOverride);
        return ToLegacySummary(source);
    }

    public static HanhViUuDaiSummary_cl TinhTongHop(dbDataContext db, string taiKhoan)
    {
        return TinhTongHop(db, taiKhoan, null);
    }

    public static bool TaoYeuCau(
        dbDataContext db,
        string taiKhoan,
        int kyHieu9HanhVi,
        decimal soDiem,
        out string message,
        out Guid newId)
    {
        return HanhViGhiNhanHoSo_cl.TaoYeuCau(
            db,
            taiKhoan,
            ProfileCode_UuDai,
            kyHieu9HanhVi,
            soDiem,
            out message,
            out newId);
    }

    public static bool DuyetYeuCau(
        dbDataContext db,
        Guid idYeuCau,
        string nguoiDuyet,
        out string message)
    {
        return HanhViGhiNhanHoSo_cl.DuyetYeuCau(
            db,
            idYeuCau,
            ProfileCode_UuDai,
            nguoiDuyet,
            out message);
    }

    public static bool TuChoiYeuCau(
        dbDataContext db,
        Guid idYeuCau,
        string nguoiDuyet,
        string ghiChuTuChoi,
        out string message)
    {
        return HanhViGhiNhanHoSo_cl.TuChoiYeuCau(
            db,
            idYeuCau,
            ProfileCode_UuDai,
            nguoiDuyet,
            ghiChuTuChoi,
            out message);
    }
}
