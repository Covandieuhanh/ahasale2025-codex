<%@ WebHandler Language="C#" Class="AppUiRuntimeBenefitRequestDetailHandler" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

public class AppUiRuntimeBenefitRequestDetailHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        string profileKey = ResolveProfileKey(context.Request["profile"]);
        Guid requestId;
        if (!Guid.TryParse((context.Request["request_id"] ?? "").Trim(), out requestId))
        {
            context.Response.Write(serializer.Serialize(new
            {
                ok = false,
                state = "invalid",
                message = "Mã yêu cầu không hợp lệ."
            }));
            return;
        }

        try
        {
            RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetCurrentInfo();
            if (info == null || !info.IsAuthenticated || string.IsNullOrWhiteSpace(info.AccountKey))
            {
                context.Response.Write(serializer.Serialize(new
                {
                    ok = false,
                    state = "guest",
                    message = "Bạn cần đăng nhập để xem chi tiết yêu cầu."
                }));
                return;
            }

            object payload = CoreDb_cl.Use(db => BuildPayload(db, info, profileKey, requestId));
            context.Response.Write(serializer.Serialize(payload));
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "app_ui_runtime_benefit_request_detail", ex.StackTrace);
            context.Response.StatusCode = 500;
            context.Response.Write(serializer.Serialize(new
            {
                ok = false,
                state = "error",
                message = "Không tải được chi tiết yêu cầu."
            }));
        }
    }

    public bool IsReusable { get { return false; } }

    private static object BuildPayload(dbDataContext db, RootAccount_cl.RootAccountInfo info, string profileKey, Guid requestId)
    {
        taikhoan_tb account = ResolveAccount(db, info);
        if (account == null || string.IsNullOrWhiteSpace(account.taikhoan))
        {
            return new { ok = false, state = "guest", message = "Không xác định được tài khoản Home." };
        }

        string taiKhoan = (account.taikhoan ?? "").Trim();
        int profileCode = ResolveProfileCode(profileKey);

        YeuCauRutQuyen_tb request = db.YeuCauRutQuyen_tbs.FirstOrDefault(p => p.IdYeuCauRut == requestId && p.TaiKhoan == taiKhoan);
        if (request == null)
        {
            return new
            {
                ok = false,
                state = "not_found",
                message = "Không tìm thấy yêu cầu cần xem."
            };
        }

        if (profileCode <= 0)
            profileCode = request.LoaiHanhVi;
        int loaiHoSo = HanhViGhiNhanHoSo_cl.GetLoaiHoSoViByProfile(profileCode);
        profileKey = GetProfileKeyByCode(profileCode);

        if ((profileCode == HanhViGhiNhanHoSo_cl.Profile_LaoDong || profileCode == HanhViGhiNhanHoSo_cl.Profile_GanKet)
            && !TierHome_cl.CanViewHoSo(TierHome_cl.TinhTierHome(db, taiKhoan), loaiHoSo))
        {
            return new
            {
                ok = false,
                state = "blocked",
                message = profileCode == HanhViGhiNhanHoSo_cl.Profile_LaoDong
                    ? "Bạn cần đạt tầng cộng tác phát triển để xem chi tiết yêu cầu này."
                    : "Bạn cần đạt tầng đồng hành hệ sinh thái để xem chi tiết yêu cầu này."
            };
        }

        decimal paidAmount = 0m;
        if (HanhViGhiNhanHoSo_cl.IsProfileChiTraQuanTri(profileCode))
            paidAmount = HanhViGhiNhanHoSo_cl.TinhTongDaChiTraByYeuCau(db, request.IdYeuCauRut, profileCode);

        string statusCode = HanhViGhiNhanHoSo_cl.NormalizeTrangThai(request.TrangThai);
        string statusText = HanhViGhiNhanHoSo_cl.GetTrangThaiText(request.TrangThai);
        if (HanhViGhiNhanHoSo_cl.IsProfileChiTraQuanTri(profileCode)
            && statusCode == HanhViGhiNhanHoSo_cl.TrangThaiDaDuyet
            && (request.TongQuyen - paidAmount) <= 0m)
        {
            statusCode = "3";
            statusText = "Đã chi trả";
        }

        HanhViHoSoSummary_cl summary = HanhViGhiNhanHoSo_cl.TinhTongHop(db, taiKhoan, profileCode);
        HanhViHoSoRowBalance_cl row = summary.Rows.FirstOrDefault(p => p.KyHieu9HanhVi_1_9 == request.KyHieu9HanhVi_1_9);

        List<object> relatedHistory = HoSoLichSuTongHop_cl.LayLichSuTongHop(db, taiKhoan, loaiHoSo, request.KyHieu9HanhVi_1_9)
            .Take(4)
            .Select(p => new
            {
                title = string.IsNullOrWhiteSpace(p.TenHanhVi) ? "Biến động hồ sơ" : HanhVi9Cap_cl.EnsureDisplayWithoutPercent(p.KyHieu9HanhVi_1_9, p.TenHanhVi),
                date_text = p.ngay.HasValue ? p.ngay.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa cập nhật",
                amount_text = ((p.CongTru.HasValue && p.CongTru.Value) ? "-" : "+") + (p.dongA ?? 0m).ToString("#,##0.##") + " A",
                note = (p.ghichu ?? "").Trim()
            })
            .Cast<object>()
            .ToList();

        return new
        {
            ok = true,
            state = "ready",
            profile = new
            {
                key = profileKey,
                title = ResolveProfileTitle(profileKey),
                history_url = ResolveHistoryUrl(profileKey)
            },
            request = new
            {
                id = request.IdYeuCauRut.ToString("N"),
                short_id = request.IdYeuCauRut.ToString("N").Substring(0, 8).ToUpper(),
                title = HanhVi9Cap_cl.GetTenHanhViKhongPhanTram(request.KyHieu9HanhVi_1_9),
                behavior_code = request.KyHieu9HanhVi_1_9,
                period_text = HanhViGhiNhanHoSo_cl.GetThoiHanTextByHanhVi(request.KyHieu9HanhVi_1_9),
                amount_text = request.TongQuyen.ToString("#,##0.##") + " A",
                status_code = statusCode,
                status_text = statusText,
                created_at = request.NgayTao.ToString("dd/MM/yyyy HH:mm"),
                updated_at = request.NgayCapNhat.HasValue ? request.NgayCapNhat.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa cập nhật",
                reviewer = string.IsNullOrWhiteSpace(request.NguoiDuyet) ? "Đang chờ hệ thống xử lý" : request.NguoiDuyet.Trim(),
                note = string.IsNullOrWhiteSpace(request.GhiChu) ? "Chưa có ghi chú thêm cho yêu cầu này." : request.GhiChu.Trim(),
                paid_text = paidAmount > 0m ? paidAmount.ToString("#,##0.##") + " A" : "0 A"
            },
            stats = new object[]
            {
                new { label = "Số điểm yêu cầu", value = request.TongQuyen.ToString("#,##0.##") + " A", tone = "accent" },
                new { label = "Hành vi hợp lệ hiện tại", value = ((row != null ? row.SoDuHanhViHopLe : 0m)).ToString("#,##0.##") + " A", tone = "positive" },
                new { label = "Đã chi trả", value = paidAmount.ToString("#,##0.##") + " A", tone = "neutral" },
                new { label = "Chu kỳ áp dụng", value = HanhViGhiNhanHoSo_cl.GetThoiHanTextByHanhVi(request.KyHieu9HanhVi_1_9), tone = "default" }
            },
            timeline = new object[]
            {
                new { title = "Yêu cầu được tạo", text = request.NgayTao.ToString("dd/MM/yyyy HH:mm"), tone = "default" },
                new { title = "Trạng thái hiện tại", text = statusText, tone = statusCode },
                new { title = "Cập nhật gần nhất", text = request.NgayCapNhat.HasValue ? request.NgayCapNhat.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa cập nhật", tone = "default" }
            },
            related_history = relatedHistory
        };
    }

    private static taikhoan_tb ResolveAccount(dbDataContext db, RootAccount_cl.RootAccountInfo info)
    {
        if (db == null || info == null)
            return null;
        taikhoan_tb byKey = RootAccount_cl.GetByAccountKey(db, info.AccountKey);
        if (byKey != null)
            return byKey;
        string accountKey = (info.AccountKey ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(accountKey))
            return null;
        return db.taikhoan_tbs.FirstOrDefault(x => (x.taikhoan ?? "").Trim().ToLower() == accountKey);
    }

    private static string ResolveProfileKey(string raw)
    {
        string key = (raw ?? "").Trim().ToLowerInvariant();
        if (key == "offers" || key == "labor" || key == "engagement")
            return key;
        return "";
    }

    private static int ResolveProfileCode(string profileKey)
    {
        if (profileKey == "offers") return HanhViGhiNhanHoSo_cl.Profile_UuDai;
        if (profileKey == "labor") return HanhViGhiNhanHoSo_cl.Profile_LaoDong;
        if (profileKey == "engagement") return HanhViGhiNhanHoSo_cl.Profile_GanKet;
        return 0;
    }

    private static string GetProfileKeyByCode(int profileCode)
    {
        if (profileCode == HanhViGhiNhanHoSo_cl.Profile_UuDai) return "offers";
        if (profileCode == HanhViGhiNhanHoSo_cl.Profile_LaoDong) return "labor";
        if (profileCode == HanhViGhiNhanHoSo_cl.Profile_GanKet) return "engagement";
        return "";
    }

    private static string ResolveProfileTitle(string profileKey)
    {
        if (profileKey == "offers") return "Hồ sơ quyền ưu đãi";
        if (profileKey == "labor") return "Hồ sơ hành vi lao động";
        if (profileKey == "engagement") return "Hồ sơ chỉ số gắn kết";
        return "Hồ sơ";
    }

    private static string ResolveHistoryUrl(string profileKey)
    {
        if (profileKey == "offers") return "/app-ui/home/benefit-offers.aspx?ui_mode=app";
        if (profileKey == "labor") return "/app-ui/home/benefit-labor.aspx?ui_mode=app";
        if (profileKey == "engagement") return "/app-ui/home/benefit-engagement.aspx?ui_mode=app";
        return "/app-ui/home/benefits.aspx?ui_mode=app";
    }
}
