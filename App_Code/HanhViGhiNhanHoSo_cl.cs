using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

public class HanhViHoSoRowBalance_cl
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

public class HanhViHoSoSummary_cl
{
    public int ProfileCode { get; set; }
    public decimal SoDuTrongHoSo { get; set; }
    public decimal TongSoDuDiemNhan { get; set; }
    public decimal TongSoDuHanhViHopLe { get; set; }
    public decimal TongSoDuDangChoDuyet { get; set; }
    public decimal TongSoDaGhiNhan { get; set; }
    public int SoPhutChoHopLe { get; set; }
    public DateTime MocHopLe { get; set; }
    public List<HanhViHoSoRowBalance_cl> Rows { get; set; }

    public HanhViHoSoSummary_cl()
    {
        Rows = new List<HanhViHoSoRowBalance_cl>();
    }
}

public static class HanhViGhiNhanHoSo_cl
{
    public const int Profile_UuDai = 1;
    public const int Profile_LaoDong = 2;
    public const int Profile_GanKet = 3;

    public const string TrangThaiChoDuyet = "0";
    public const string TrangThaiDaDuyet = "1";
    public const string TrangThaiTuChoi = "2";

    public const string DbTrangThaiChoDuyet = "Đang xử lý";
    public const string DbTrangThaiDaDuyet = "Thành công";
    public const string DbTrangThaiTuChoi = "Thất bại";

    public static string NormalizeTrangThai(string trangThai)
    {
        string s = (trangThai ?? "").Trim().ToLowerInvariant();
        if (s == ""
            || s == TrangThaiChoDuyet
            || s == "cho duyet"
            || s == "chờ duyệt"
            || s == "dang xu ly"
            || s == "đang xử lý")
            return TrangThaiChoDuyet;
        if (s == TrangThaiDaDuyet
            || s == "da duyet"
            || s == "đã duyệt"
            || s == "thanh cong"
            || s == "thành công")
            return TrangThaiDaDuyet;
        if (s == TrangThaiTuChoi
            || s == "tu choi"
            || s == "từ chối"
            || s == "that bai"
            || s == "thất bại")
            return TrangThaiTuChoi;
        return TrangThaiTuChoi;
    }

    public static string GetTrangThaiText(string trangThai)
    {
        string s = NormalizeTrangThai(trangThai);
        if (s == TrangThaiDaDuyet) return "Đã duyệt";
        if (s == TrangThaiTuChoi) return "Từ chối";
        return "Chờ duyệt";
    }

    public static bool TryGetHanhViRange(int profileCode, out int fromKy, out int toKy)
    {
        fromKy = 0;
        toKy = 0;
        if (profileCode == Profile_UuDai) { fromKy = 1; toKy = 3; return true; }
        if (profileCode == Profile_LaoDong) { fromKy = 4; toKy = 6; return true; }
        if (profileCode == Profile_GanKet) { fromKy = 7; toKy = 9; return true; }
        return false;
    }

    public static bool IsHanhViThuocHoSo(int profileCode, int kyHieu9HanhVi)
    {
        int fromKy, toKy;
        if (!TryGetHanhViRange(profileCode, out fromKy, out toKy))
            return false;
        return kyHieu9HanhVi >= fromKy && kyHieu9HanhVi <= toKy;
    }

    public static int GetLoaiHoSoViByProfile(int profileCode)
    {
        if (profileCode == Profile_UuDai) return 2;
        if (profileCode == Profile_LaoDong) return 3;
        return 4;
    }

    public static string GetTenHoSoByProfile(int profileCode)
    {
        if (profileCode == Profile_UuDai) return "hồ sơ quyền ưu đãi";
        if (profileCode == Profile_LaoDong) return "hồ sơ hành vi lao động";
        if (profileCode == Profile_GanKet) return "hồ sơ chỉ số gắn kết";
        return "hồ sơ";
    }

    public static string GetHomeUrlByProfile(int profileCode)
    {
        if (profileCode == Profile_UuDai) return "/home/lich-su-quyen-uu-dai.aspx";
        if (profileCode == Profile_LaoDong) return "/home/lich-su-quyen-lao-dong.aspx";
        if (profileCode == Profile_GanKet) return "/home/lich-su-quyen-gan-ket.aspx";
        return "/home/default.aspx";
    }

    public static int GetSoPhutChoHopLe(int profileCode)
    {
        string key;
        if (profileCode == Profile_LaoDong) key = "HanhViGhiNhan.LaoDong.MinMinutes";
        else if (profileCode == Profile_GanKet) key = "HanhViGhiNhan.GanKet.MinMinutes";
        else key = "HanhViGhiNhan.UuDai.MinMinutes";

        int soPhut;
        string raw = ConfigurationManager.AppSettings[key];
        if (!int.TryParse((raw ?? "").Trim(), out soPhut))
            soPhut = 0;
        if (soPhut < 0)
            soPhut = 0;
        return soPhut;
    }

    public static decimal GetSoDuTrongHoSoThat(taikhoan_tb acc, int profileCode)
    {
        if (acc == null) return 0m;
        if (profileCode == Profile_UuDai) return acc.Vi1That_Evocher_30PhanTram ?? 0m;
        if (profileCode == Profile_LaoDong) return acc.Vi2That_LaoDong_50PhanTram ?? 0m;
        return acc.Vi3That_GanKet_20PhanTram ?? 0m;
    }

    public static void CongSoDuTrongHoSoThat(taikhoan_tb acc, int profileCode, decimal amount)
    {
        if (acc == null || amount <= 0m) return;
        if (profileCode == Profile_UuDai)
            acc.Vi1That_Evocher_30PhanTram = (acc.Vi1That_Evocher_30PhanTram ?? 0m) + amount;
        else if (profileCode == Profile_LaoDong)
            acc.Vi2That_LaoDong_50PhanTram = (acc.Vi2That_LaoDong_50PhanTram ?? 0m) + amount;
        else
            acc.Vi3That_GanKet_20PhanTram = (acc.Vi3That_GanKet_20PhanTram ?? 0m) + amount;
    }

    public static HanhViHoSoSummary_cl TinhTongHop(dbDataContext db, string taiKhoan, int profileCode, DateTime? nowOverride)
    {
        HanhViHoSoSummary_cl summary = new HanhViHoSoSummary_cl();
        summary.ProfileCode = profileCode;

        if (db == null) return summary;
        string tk = (taiKhoan ?? "").Trim();
        if (tk == "") return summary;

        int fromKy, toKy;
        if (!TryGetHanhViRange(profileCode, out fromKy, out toKy))
            return summary;

        DateTime now = nowOverride.HasValue ? nowOverride.Value : AhaTime_cl.Now;
        int soPhutCho = GetSoPhutChoHopLe(profileCode);
        DateTime mocHopLe = now.AddMinutes(-soPhutCho);

        summary.SoPhutChoHopLe = soPhutCho;
        summary.MocHopLe = mocHopLe;

        var acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
        summary.SoDuTrongHoSo = GetSoDuTrongHoSoThat(acc, profileCode);

        var detailRows = db.ViLoiNhuan_LichSuBanHang_ChiTiet_tbs
            .Where(p => p.TaiKhoan_Nhan == tk
                && p.LoaiHanhVi.HasValue
                && p.LoaiHanhVi.Value >= fromKy
                && p.LoaiHanhVi.Value <= toKy)
            .Select(p => new
            {
                Ky = p.LoaiHanhVi.Value,
                SoDiem = p.DongANhanDuoc,
                p.ThoiGian
            })
            .ToList();

        var requestRows = db.YeuCauRutQuyen_tbs
            .Where(p => p.TaiKhoan == tk
                && p.LoaiHanhVi == profileCode
                && p.KyHieu9HanhVi_1_9 >= fromKy
                && p.KyHieu9HanhVi_1_9 <= toKy)
            .Select(p => new
            {
                p.KyHieu9HanhVi_1_9,
                p.TongQuyen,
                p.TrangThai
            })
            .ToList();

        for (int ky = fromKy; ky <= toKy; ky++)
        {
            decimal tongNhan = detailRows
                .Where(p => p.Ky == ky && p.ThoiGian > mocHopLe)
                .Sum(p => p.SoDiem);

            decimal tongDaDatMoc = detailRows
                .Where(p => p.Ky == ky && p.ThoiGian <= mocHopLe)
                .Sum(p => p.SoDiem);

            decimal tongChoDuyet = requestRows
                .Where(p => p.KyHieu9HanhVi_1_9 == ky && NormalizeTrangThai(p.TrangThai) == TrangThaiChoDuyet)
                .Sum(p => p.TongQuyen);

            decimal tongDaGhiNhan = requestRows
                .Where(p => p.KyHieu9HanhVi_1_9 == ky && NormalizeTrangThai(p.TrangThai) == TrangThaiDaDuyet)
                .Sum(p => p.TongQuyen);

            decimal tongDaTruKhoiHopLe = tongChoDuyet + tongDaGhiNhan;
            decimal hopLeConLai = tongDaDatMoc - tongDaTruKhoiHopLe;
            if (hopLeConLai < 0m) hopLeConLai = 0m;

            HanhViHoSoRowBalance_cl row = new HanhViHoSoRowBalance_cl();
            row.KyHieu9HanhVi_1_9 = ky;
            row.TenHanhVi = HanhVi9Cap_cl.GetTenHanhViTheoLoai(ky);
            row.SoDuDiemNhan = tongNhan;
            row.SoDuHanhViHopLe = hopLeConLai;
            row.SoDuDangChoDuyet = tongChoDuyet;
            row.SoDaGhiNhan = tongDaGhiNhan;
            summary.Rows.Add(row);
        }

        summary.TongSoDuDiemNhan = summary.Rows.Sum(p => p.SoDuDiemNhan);
        summary.TongSoDuHanhViHopLe = summary.Rows.Sum(p => p.SoDuHanhViHopLe);
        summary.TongSoDuDangChoDuyet = summary.Rows.Sum(p => p.SoDuDangChoDuyet);
        summary.TongSoDaGhiNhan = summary.Rows.Sum(p => p.SoDaGhiNhan);

        return summary;
    }

    public static HanhViHoSoSummary_cl TinhTongHop(dbDataContext db, string taiKhoan, int profileCode)
    {
        return TinhTongHop(db, taiKhoan, profileCode, null);
    }

    private static decimal TinhHopLeConLaiTheoHanhVi(
        dbDataContext db,
        string taiKhoan,
        int profileCode,
        int kyHieu9HanhVi,
        Guid? excludeRequestId)
    {
        string tk = (taiKhoan ?? "").Trim();
        if (db == null || tk == "" || !IsHanhViThuocHoSo(profileCode, kyHieu9HanhVi))
            return 0m;

        DateTime mocHopLe = AhaTime_cl.Now.AddMinutes(-GetSoPhutChoHopLe(profileCode));

        decimal tongDaDatMoc = db.ViLoiNhuan_LichSuBanHang_ChiTiet_tbs
            .Where(p => p.TaiKhoan_Nhan == tk
                && p.LoaiHanhVi.HasValue
                && p.LoaiHanhVi.Value == kyHieu9HanhVi
                && p.ThoiGian <= mocHopLe)
            .Sum(p => (decimal?)p.DongANhanDuoc) ?? 0m;

        var rqQuery = db.YeuCauRutQuyen_tbs.Where(p =>
            p.TaiKhoan == tk
            && p.LoaiHanhVi == profileCode
            && p.KyHieu9HanhVi_1_9 == kyHieu9HanhVi);

        if (excludeRequestId.HasValue)
            rqQuery = rqQuery.Where(p => p.IdYeuCauRut != excludeRequestId.Value);

        decimal tongDaTruKhoiHopLe = rqQuery
            .ToList()
            .Where(p =>
            {
                string st = NormalizeTrangThai(p.TrangThai);
                return st == TrangThaiChoDuyet || st == TrangThaiDaDuyet;
            })
            .Sum(p => p.TongQuyen);

        decimal hopLeConLai = tongDaDatMoc - tongDaTruKhoiHopLe;
        if (hopLeConLai < 0m) hopLeConLai = 0m;
        return hopLeConLai;
    }

    public static bool TaoYeuCau(
        dbDataContext db,
        string taiKhoan,
        int profileCode,
        int kyHieu9HanhVi,
        decimal soDiem,
        out string message,
        out Guid newId)
    {
        message = "";
        newId = Guid.Empty;

        string tk = (taiKhoan ?? "").Trim();
        if (db == null || tk == "")
        {
            message = "Tài khoản không hợp lệ.";
            return false;
        }

        if (!IsHanhViThuocHoSo(profileCode, kyHieu9HanhVi))
        {
            message = "Hành vi không hợp lệ cho hồ sơ này.";
            return false;
        }

        decimal amount = Math.Round(soDiem, 2, MidpointRounding.AwayFromZero);
        if (amount <= 0m)
        {
            message = "Số điểm yêu cầu phải lớn hơn 0.";
            return false;
        }

        decimal hopLeConLai = TinhHopLeConLaiTheoHanhVi(db, tk, profileCode, kyHieu9HanhVi, null);
        if (amount > hopLeConLai)
        {
            message = "Số điểm yêu cầu vượt quá số dư hành vi hợp lệ.";
            return false;
        }

        newId = Guid.NewGuid();
        YeuCauRutQuyen_tb req = new YeuCauRutQuyen_tb();
        req.IdYeuCauRut = newId;
        req.TaiKhoan = tk;
        req.LoaiHanhVi = profileCode;
        req.KyHieu9HanhVi_1_9 = kyHieu9HanhVi;
        req.TongQuyen = amount;
        req.TrangThai = DbTrangThaiChoDuyet;
        req.NgayTao = AhaTime_cl.Now;
        req.NgayCapNhat = AhaTime_cl.Now;
        req.NguoiDuyet = "";
        req.GhiChu = "Yêu cầu ghi nhận hành vi " + HanhVi9Cap_cl.GetTenHanhViTheoLoai(kyHieu9HanhVi);

        db.YeuCauRutQuyen_tbs.InsertOnSubmit(req);
        return true;
    }

    public static bool DuyetYeuCau(
        dbDataContext db,
        Guid idYeuCau,
        int profileCode,
        string nguoiDuyet,
        out string message)
    {
        message = "";
        if (db == null)
        {
            message = "Không có kết nối dữ liệu.";
            return false;
        }

        string reviewer = (nguoiDuyet ?? "").Trim();
        if (reviewer == "")
        {
            message = "Không xác định tài khoản duyệt.";
            return false;
        }

        YeuCauRutQuyen_tb req = db.YeuCauRutQuyen_tbs.FirstOrDefault(p => p.IdYeuCauRut == idYeuCau);
        if (req == null)
        {
            message = "Không tìm thấy yêu cầu.";
            return false;
        }

        if (req.LoaiHanhVi != profileCode || !IsHanhViThuocHoSo(profileCode, req.KyHieu9HanhVi_1_9))
        {
            message = "Yêu cầu không thuộc hồ sơ cần duyệt.";
            return false;
        }

        if (NormalizeTrangThai(req.TrangThai) != TrangThaiChoDuyet)
        {
            message = "Yêu cầu này không còn ở trạng thái chờ duyệt.";
            return false;
        }

        decimal hopLeConLai = TinhHopLeConLaiTheoHanhVi(db, req.TaiKhoan, profileCode, req.KyHieu9HanhVi_1_9, req.IdYeuCauRut);
        if (req.TongQuyen > hopLeConLai)
        {
            message = "Số dư hành vi hợp lệ hiện không đủ để duyệt yêu cầu này.";
            return false;
        }

        taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == req.TaiKhoan);
        if (acc == null)
        {
            message = "Không tìm thấy tài khoản nhận ghi nhận.";
            return false;
        }

        CongSoDuTrongHoSoThat(acc, profileCode, req.TongQuyen);

        string refId = "YCRQ:" + req.IdYeuCauRut.ToString("N");
        string ghiChu = "Duyệt ghi nhận hành vi " + HanhVi9Cap_cl.GetTenHanhViTheoLoai(req.KyHieu9HanhVi_1_9)
            + " (" + req.TongQuyen.ToString("#,##0.##") + " điểm)";

        Helper_DongA_cl.AddLedger(
            db,
            req.TaiKhoan,
            req.TongQuyen,
            true,
            ghiChu,
            refId,
            GetLoaiHoSoViByProfile(profileCode),
            req.KyHieu9HanhVi_1_9);

        req.TrangThai = DbTrangThaiDaDuyet;
        req.NgayCapNhat = AhaTime_cl.Now;
        req.NguoiDuyet = reviewer;
        req.GhiChu = "Đã duyệt ghi nhận";

        Helper_DongA_cl.AddNotify(
            db,
            reviewer,
            req.TaiKhoan,
            "Yêu cầu ghi nhận hành vi của bạn đã được duyệt.",
            GetHomeUrlByProfile(profileCode));

        return true;
    }

    public static bool TuChoiYeuCau(
        dbDataContext db,
        Guid idYeuCau,
        int profileCode,
        string nguoiDuyet,
        string ghiChuTuChoi,
        out string message)
    {
        message = "";
        if (db == null)
        {
            message = "Không có kết nối dữ liệu.";
            return false;
        }

        string reviewer = (nguoiDuyet ?? "").Trim();
        if (reviewer == "")
        {
            message = "Không xác định tài khoản duyệt.";
            return false;
        }

        YeuCauRutQuyen_tb req = db.YeuCauRutQuyen_tbs.FirstOrDefault(p => p.IdYeuCauRut == idYeuCau);
        if (req == null)
        {
            message = "Không tìm thấy yêu cầu.";
            return false;
        }

        if (req.LoaiHanhVi != profileCode || !IsHanhViThuocHoSo(profileCode, req.KyHieu9HanhVi_1_9))
        {
            message = "Yêu cầu không thuộc hồ sơ cần duyệt.";
            return false;
        }

        if (NormalizeTrangThai(req.TrangThai) != TrangThaiChoDuyet)
        {
            message = "Yêu cầu này không còn ở trạng thái chờ duyệt.";
            return false;
        }

        req.TrangThai = DbTrangThaiTuChoi;
        req.NgayCapNhat = AhaTime_cl.Now;
        req.NguoiDuyet = reviewer;
        req.GhiChu = string.IsNullOrWhiteSpace(ghiChuTuChoi) ? "Từ chối ghi nhận" : ghiChuTuChoi.Trim();

        Helper_DongA_cl.AddNotify(
            db,
            reviewer,
            req.TaiKhoan,
            "Yêu cầu ghi nhận hành vi của bạn đã bị từ chối.",
            GetHomeUrlByProfile(profileCode));

        return true;
    }
}
