<%@ WebHandler Language="C#" Class="AppUiRuntimeBenefitRequestHandler" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

public class AppUiRuntimeBenefitRequestHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();

        JavaScriptSerializer serializer = new JavaScriptSerializer();

        string profileKey = ResolveProfileKey(context.Request["profile"]);
        int profileCode = ResolveProfileCode(profileKey);
        int behaviorCode = ParseInt(context.Request["behavior"]);

        if (!string.Equals(context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetCurrentInfo();
                if (info == null || !info.IsAuthenticated || string.IsNullOrWhiteSpace(info.AccountKey))
                {
                    context.Response.Write(serializer.Serialize(new
                    {
                        ok = false,
                        state = "guest",
                        profile_key = profileKey,
                        behavior_code = behaviorCode
                    }));
                    return;
                }

                object payload = CoreDb_cl.Use(db => BuildGetPayload(db, info, profileKey, profileCode, behaviorCode));
                context.Response.Write(serializer.Serialize(payload));
            }
            catch (Exception ex)
            {
                Log_cl.Add_Log(ex.Message, "app_ui_runtime_benefit_request_get", ex.StackTrace);
                context.Response.StatusCode = 500;
                context.Response.Write(serializer.Serialize(new
                {
                    ok = false,
                    state = "error",
                    message = "Không tải được dữ liệu yêu cầu ghi nhận."
                }));
            }
            return;
        }

        try
        {
            RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetCurrentInfo();
            if (info == null || !info.IsAuthenticated || string.IsNullOrWhiteSpace(info.AccountKey))
            {
                context.Response.StatusCode = 401;
                context.Response.Write(serializer.Serialize(new
                {
                    ok = false,
                    message = "Bạn cần đăng nhập để gửi yêu cầu."
                }));
                return;
            }

            decimal amount = ParseDecimal(context.Request["amount"]);
            object payload = CoreDb_cl.Use(db => SubmitRequest(db, info, profileKey, profileCode, behaviorCode, amount));
            context.Response.Write(serializer.Serialize(payload));
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "app_ui_runtime_benefit_request_post", ex.StackTrace);
            context.Response.StatusCode = 500;
            context.Response.Write(serializer.Serialize(new
            {
                ok = false,
                message = "Không gửi được yêu cầu, vui lòng thử lại."
            }));
        }
    }

    public bool IsReusable { get { return false; } }

    private static object BuildGetPayload(dbDataContext db, RootAccount_cl.RootAccountInfo info, string profileKey, int profileCode, int behaviorCode)
    {
        if (profileCode <= 0 || !HanhViGhiNhanHoSo_cl.IsHanhViThuocHoSo(profileCode, behaviorCode))
        {
            return new
            {
                ok = false,
                state = "invalid",
                message = "Không xác định được hành vi cần ghi nhận."
            };
        }

        taikhoan_tb account = ResolveAccount(db, info);
        if (account == null || string.IsNullOrWhiteSpace(account.taikhoan))
        {
            return new
            {
                ok = false,
                state = "guest",
                message = "Không xác định được tài khoản Home."
            };
        }

        string taiKhoan = (account.taikhoan ?? "").Trim();
        int loaiHoSo = HanhViGhiNhanHoSo_cl.GetLoaiHoSoViByProfile(profileCode);
        int tierHome = TierHome_cl.TinhTierHome(db, taiKhoan);
        if ((profileCode == HanhViGhiNhanHoSo_cl.Profile_LaoDong || profileCode == HanhViGhiNhanHoSo_cl.Profile_GanKet)
            && !TierHome_cl.CanViewHoSo(tierHome, loaiHoSo))
        {
            return new
            {
                ok = false,
                state = "blocked",
                message = profileCode == HanhViGhiNhanHoSo_cl.Profile_LaoDong
                    ? "Bạn cần đạt tầng cộng tác phát triển để gửi yêu cầu cho hồ sơ hành vi lao động."
                    : "Bạn cần đạt tầng đồng hành hệ sinh thái để gửi yêu cầu cho hồ sơ chỉ số gắn kết.",
                tier_text = "Tier Home hiện tại: " + tierHome.ToString("#,##0")
            };
        }

        HanhViHoSoSummary_cl summary = HanhViGhiNhanHoSo_cl.TinhTongHop(db, taiKhoan, profileCode);
        HanhViHoSoRowBalance_cl row = summary.Rows.FirstOrDefault(p => p.KyHieu9HanhVi_1_9 == behaviorCode);
        if (row == null)
        {
            return new
            {
                ok = false,
                state = "invalid",
                message = "Không tìm thấy dữ liệu hành vi cần ghi nhận."
            };
        }

        var recentRequests = db.YeuCauRutQuyen_tbs
            .Where(p => p.TaiKhoan == taiKhoan
                && p.LoaiHanhVi == profileCode
                && p.KyHieu9HanhVi_1_9 == behaviorCode)
            .OrderByDescending(p => p.NgayTao)
            .Take(5)
            .Select(p => new
            {
                p.IdYeuCauRut,
                p.NgayTao,
                p.TongQuyen,
                p.TrangThai,
                p.NgayCapNhat,
                p.GhiChu
            })
            .ToList()
            .Select(p => new
            {
                id = p.IdYeuCauRut.ToString("N"),
                created_at = p.NgayTao.ToString("dd/MM/yyyy HH:mm"),
                updated_at = p.NgayCapNhat.HasValue ? p.NgayCapNhat.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa cập nhật",
                amount_text = p.TongQuyen.ToString("#,##0.##") + " A",
                status_code = HanhViGhiNhanHoSo_cl.NormalizeTrangThai(p.TrangThai),
                status_text = HanhViGhiNhanHoSo_cl.GetTrangThaiText(p.TrangThai),
                note = (p.GhiChu ?? "").Trim(),
                detail_url = "/app-ui/home/benefit-request-detail.aspx?ui_mode=app&profile=" + HttpUtility.UrlEncode(profileKey) + "&request_id=" + HttpUtility.UrlEncode(p.IdYeuCauRut.ToString("N"))
            })
            .Cast<object>()
            .ToList();

        return new
        {
            ok = true,
            state = "ready",
            profile_key = profileKey,
            behavior_code = behaviorCode,
            profile = new
            {
                title = ResolveProfileTitle(profileKey),
                short_title = ResolveProfileShortTitle(profileKey),
                history_url = ResolveHistoryUrl(profileKey),
                description = "Gửi yêu cầu ghi nhận theo đúng mốc " + HanhViGhiNhanHoSo_cl.GetThoiHanTextByHanhVi(behaviorCode) + " trên app."
            },
            behavior = new
            {
                code = "HV" + behaviorCode,
                title = HanhVi9Cap_cl.GetTenHanhViKhongPhanTram(behaviorCode),
                period_text = HanhViGhiNhanHoSo_cl.GetThoiHanTextByHanhVi(behaviorCode),
                rule_text = "Điểm đủ điều kiện sau " + HanhViGhiNhanHoSo_cl.GetThoiHanTextByHanhVi(behaviorCode),
                eligible_value = row.SoDuHanhViHopLe,
                eligible_text = row.SoDuHanhViHopLe.ToString("#,##0.##") + " A",
                earned_text = row.SoDuDiemNhan.ToString("#,##0.##") + " điểm nhận",
                pending_text = row.SoDuDangChoDuyet.ToString("#,##0.##") + " đang chờ duyệt",
                recorded_text = row.SoDaGhiNhan.ToString("#,##0.##") + " đã ghi nhận",
                can_submit = row.SoDuHanhViHopLe > 0m
            },
            note = (summary.DieuKienHopLeText ?? HanhViGhiNhanHoSo_cl.GetDieuKienHopLeText())
                + " Hành vi này đang áp dụng mốc " + HanhViGhiNhanHoSo_cl.GetThoiHanTextByHanhVi(behaviorCode) + ".",
            recent_requests = recentRequests
        };
    }

    private static object SubmitRequest(dbDataContext db, RootAccount_cl.RootAccountInfo info, string profileKey, int profileCode, int behaviorCode, decimal amount)
    {
        if (profileCode <= 0 || !HanhViGhiNhanHoSo_cl.IsHanhViThuocHoSo(profileCode, behaviorCode))
        {
            return new
            {
                ok = false,
                message = "Hành vi yêu cầu không hợp lệ."
            };
        }

        taikhoan_tb account = ResolveAccount(db, info);
        if (account == null || string.IsNullOrWhiteSpace(account.taikhoan))
        {
            return new
            {
                ok = false,
                message = "Không xác định được tài khoản gửi yêu cầu."
            };
        }

        string taiKhoan = (account.taikhoan ?? "").Trim();
        int loaiHoSo = HanhViGhiNhanHoSo_cl.GetLoaiHoSoViByProfile(profileCode);
        int tierHome = TierHome_cl.TinhTierHome(db, taiKhoan);
        if ((profileCode == HanhViGhiNhanHoSo_cl.Profile_LaoDong || profileCode == HanhViGhiNhanHoSo_cl.Profile_GanKet)
            && !TierHome_cl.CanViewHoSo(tierHome, loaiHoSo))
        {
            return new
            {
                ok = false,
                message = profileCode == HanhViGhiNhanHoSo_cl.Profile_LaoDong
                    ? "Bạn cần đạt tầng cộng tác phát triển để gửi yêu cầu cho hồ sơ hành vi lao động."
                    : "Bạn cần đạt tầng đồng hành hệ sinh thái để gửi yêu cầu cho hồ sơ chỉ số gắn kết."
            };
        }

        string message;
        Guid newId;
        bool ok = HanhViGhiNhanHoSo_cl.TaoYeuCau(
            db,
            taiKhoan,
            profileCode,
            behaviorCode,
            amount,
            out message,
            out newId);

        if (!ok)
        {
            return new
            {
                ok = false,
                message = message
            };
        }

        db.SubmitChanges();

        return new
        {
            ok = true,
            message = "Đã gửi yêu cầu ghi nhận hành vi. Mã yêu cầu: " + newId.ToString("N").Substring(0, 8).ToUpper(),
            request_id = newId.ToString("N"),
            redirect_url = ResolveHistoryUrl(profileKey)
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

    private static string ResolveProfileTitle(string profileKey)
    {
        if (profileKey == "offers") return "Hồ sơ quyền ưu đãi";
        if (profileKey == "labor") return "Hồ sơ hành vi lao động";
        if (profileKey == "engagement") return "Hồ sơ chỉ số gắn kết";
        return "Hồ sơ";
    }

    private static string ResolveProfileShortTitle(string profileKey)
    {
        if (profileKey == "offers") return "Quyền ưu đãi";
        if (profileKey == "labor") return "Hành vi lao động";
        if (profileKey == "engagement") return "Chỉ số gắn kết";
        return "Hồ sơ";
    }

    private static string ResolveHistoryUrl(string profileKey)
    {
        if (profileKey == "offers") return "/app-ui/home/benefit-offers.aspx?ui_mode=app";
        if (profileKey == "labor") return "/app-ui/home/benefit-labor.aspx?ui_mode=app";
        if (profileKey == "engagement") return "/app-ui/home/benefit-engagement.aspx?ui_mode=app";
        return "/app-ui/home/benefits.aspx?ui_mode=app";
    }

    private static int ParseInt(string raw)
    {
        int value;
        return int.TryParse((raw ?? "").Trim(), out value) ? value : 0;
    }

    private static decimal ParseDecimal(string raw)
    {
        decimal value;
        string safe = (raw ?? "").Replace(",", "").Trim();
        return decimal.TryParse(safe, out value) ? value : 0m;
    }
}
