<%@ WebHandler Language="C#" Class="AppUiRuntimeBenefitHistoryHandler" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

public class AppUiRuntimeBenefitHistoryHandler : IHttpHandler
{
    private const int PageSize = 20;

    private sealed class BenefitMeta
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string Description { get; set; }
        public string BalanceLabel { get; set; }
        public string Accent { get; set; }
    }

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();

        string profileKey = ResolveProfileKey(context.Request["profile"]);
        BenefitMeta meta = GetMeta(profileKey);
        int page;
        if (!int.TryParse((context.Request["page"] ?? "").Trim(), out page))
            page = 1;
        page = Math.Max(1, page);
        string periodKey = ResolvePeriodKey(context.Request["period"]);

        try
        {
            RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetCurrentInfo();
            if (info == null || !info.IsAuthenticated || string.IsNullOrWhiteSpace(info.AccountKey))
            {
                context.Response.Write(ToJson(new
                {
                    ok = false,
                    reason = "unauthorized",
                    state = "guest",
                    profile_key = profileKey,
                    meta = BuildMetaPayload(meta)
                }));
                return;
            }

            object payload = CoreDb_cl.Use(db => BuildPayload(db, info, meta, page, periodKey));
            context.Response.Write(ToJson(payload));
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "app_ui_runtime_benefit_history", ex.StackTrace);
            context.Response.StatusCode = 500;
            context.Response.Write(ToJson(new
            {
                ok = false,
                reason = "error",
                state = "error",
                profile_key = profileKey,
                meta = BuildMetaPayload(meta)
            }));
        }
    }

    public bool IsReusable { get { return false; } }

    private static object BuildPayload(dbDataContext db, RootAccount_cl.RootAccountInfo info, BenefitMeta meta, int page, string periodKey)
    {
        taikhoan_tb account = ResolveAccount(db, info);
        if (account == null || string.IsNullOrWhiteSpace(account.taikhoan))
        {
            return new
            {
                ok = false,
                reason = "not_found",
                state = "guest",
                profile_key = meta.Key,
                meta = BuildMetaPayload(meta)
            };
        }

        string taiKhoan = (account.taikhoan ?? "").Trim();

        if (string.Equals(meta.Key, "consumer", StringComparison.OrdinalIgnoreCase))
            return BuildConsumerPayload(db, account, meta, page);

        int profileCode = ResolveProfileCode(meta.Key);
        int loaiHoSo = HanhViGhiNhanHoSo_cl.GetLoaiHoSoViByProfile(profileCode);
        int tierHome = TierHome_cl.TinhTierHome(db, taiKhoan);

        if ((meta.Key == "labor" || meta.Key == "engagement") && !TierHome_cl.CanViewHoSo(tierHome, loaiHoSo))
            return BuildBlockedPayload(meta, tierHome);

        return BuildBehaviorProfilePayload(db, account, meta, profileCode, loaiHoSo, page, periodKey);
    }

    private static object BuildConsumerPayload(dbDataContext db, taikhoan_tb account, BenefitMeta meta, int page)
    {
        string taiKhoan = (account.taikhoan ?? "").Trim();
        DateTime now = AhaTime_cl.Now;
        DateTime recentFrom = now.AddDays(-30);

        var historyQuery = db.LichSu_DongA_tbs.Where(p =>
            p.taikhoan == taiKhoan
            && p.LoaiHoSo_Vi == 1
            && (p.ghichu == null || p.ghichu == "" || !p.ghichu.Contains("|SHOPONLY|CREDIT_SELLER|")));

        int totalCount = historyQuery.Count();
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (decimal)PageSize));
        int currentPage = Math.Min(Math.Max(1, page), totalPages);

        var pageRows = historyQuery
            .OrderByDescending(p => p.ngay)
            .ThenByDescending(p => p.id)
            .Skip((currentPage - 1) * PageSize)
            .Take(PageSize)
            .Select(p => new
            {
                p.id,
                p.ngay,
                p.dongA,
                p.CongTru,
                p.ghichu,
                p.id_donhang
            })
            .ToList();

        decimal recentCredit = historyQuery
            .Where(p => p.ngay >= recentFrom && p.CongTru.HasValue && p.CongTru.Value == false)
            .Sum(p => (decimal?)p.dongA) ?? 0m;

        decimal recentDebit = historyQuery
            .Where(p => p.ngay >= recentFrom && p.CongTru.HasValue && p.CongTru.Value == true)
            .Sum(p => (decimal?)p.dongA) ?? 0m;

        List<object> history = pageRows.Select(p => BuildHistoryItem(
            p.id,
            p.ngay,
            p.dongA,
            p.CongTru,
            ResolveConsumerHistoryTitle(p.ghichu),
            NormalizeNote(p.ghichu),
            p.id_donhang,
            "Quyền tiêu dùng")).Cast<object>().ToList();

        return new
        {
            ok = true,
            state = "ready",
            profile_key = meta.Key,
            meta = BuildMetaPayload(meta),
            hero = new
            {
                balance_label = meta.BalanceLabel,
                balance_value = FormatDecimal(account.DongA ?? 0m),
                balance_suffix = "A",
                status_text = totalCount > 0 ? "Có " + totalCount.ToString("#,##0") + " biến động đã ghi nhận" : "Chưa có biến động nào"
            },
            summary = new object[]
            {
                BuildStat("Số dư hiện tại", FormatDecimal(account.DongA ?? 0m) + " A", "accent"),
                BuildStat("Phát sinh cộng 30 ngày", FormatDecimal(recentCredit) + " A", "positive"),
                BuildStat("Phát sinh trừ 30 ngày", FormatDecimal(recentDebit) + " A", "neutral"),
                BuildStat("Tổng giao dịch", totalCount.ToString("#,##0"), "default")
            },
            note = "Các biến động mới nhất được trình bày theo kiểu dòng thời gian của app để dễ theo dõi khi đang làm việc trên di động.",
            requests = new object[0],
            behaviors = new object[0],
            history = history,
            history_pagination = BuildPagination(currentPage, totalPages, totalCount),
            request_pagination = BuildPagination(1, 1, 0)
        };
    }

    private static object BuildBehaviorProfilePayload(dbDataContext db, taikhoan_tb account, BenefitMeta meta, int profileCode, int loaiHoSo, int page, string periodKey)
    {
        string taiKhoan = (account.taikhoan ?? "").Trim();
        HanhViHoSoSummary_cl summary = HanhViGhiNhanHoSo_cl.TinhTongHop(db, taiKhoan, profileCode);
        string note = (summary.DieuKienHopLeText ?? HanhViGhiNhanHoSo_cl.GetDieuKienHopLeText())
            + " Mốc hệ thống hiện tại: " + AhaTime_cl.Now.ToString("dd/MM/yyyy HH:mm:ss") + ".";

        if (profileCode == HanhViGhiNhanHoSo_cl.Profile_LaoDong)
            note += " Điểm đã duyệt của hồ sơ này được ghi nhận vào dữ liệu chờ chi trả lương.";
        else if (profileCode == HanhViGhiNhanHoSo_cl.Profile_GanKet)
            note += " Điểm đã duyệt của hồ sơ này được ghi nhận vào dữ liệu chờ chi trả thưởng.";

        List<HanhViHoSoRowBalance_cl> filteredRows = summary.Rows
            .Where(p => PeriodMatches(periodKey, p.KyHieu9HanhVi_1_9))
            .ToList();

        List<object> behaviors = filteredRows.Select(p => new
        {
            code = "HV" + p.KyHieu9HanhVi_1_9,
            behavior_code = p.KyHieu9HanhVi_1_9,
            period_key = GetPeriodKeyByBehavior(p.KyHieu9HanhVi_1_9),
            title = HanhVi9Cap_cl.GetTenHanhViKhongPhanTram(p.KyHieu9HanhVi_1_9),
            rule_text = "Điều kiện đủ hạn: " + HanhViGhiNhanHoSo_cl.GetThoiHanTextByHanhVi(p.KyHieu9HanhVi_1_9),
            earned_text = FormatDecimal(p.SoDuDiemNhan) + " điểm nhận",
            eligible_text = FormatDecimal(p.SoDuHanhViHopLe) + " hợp lệ",
            pending_text = FormatDecimal(p.SoDuDangChoDuyet) + " chờ duyệt",
            recorded_text = FormatDecimal(p.SoDaGhiNhan) + " đã ghi nhận",
            can_submit = p.CoTheGui
        }).Cast<object>().ToList();

        List<object> requests = BuildRequests(db, taiKhoan, profileCode, loaiHoSo, periodKey);

        List<HoSoLichSuItem_cl> allHistory = HoSoLichSuTongHop_cl.LayLichSuTongHop(db, taiKhoan, loaiHoSo)
            .Where(p => PeriodMatches(periodKey, p.KyHieu9HanhVi_1_9))
            .Select(p =>
            {
                p.TenHanhVi = HanhVi9Cap_cl.EnsureDisplayWithoutPercent(p.KyHieu9HanhVi_1_9, p.TenHanhVi);
                return p;
            })
            .ToList();

        int totalCount = allHistory.Count;
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (decimal)PageSize));
        int currentPage = Math.Min(Math.Max(1, page), totalPages);
        List<object> history = allHistory
            .Skip((currentPage - 1) * PageSize)
            .Take(PageSize)
            .Select(p => BuildHistoryItem(
                p.id,
                p.ngay,
                p.dongA,
                p.CongTru,
                string.IsNullOrWhiteSpace(p.TenHanhVi) ? meta.ShortTitle : p.TenHanhVi,
                NormalizeNote(p.ghichu),
                p.id_donhang,
                string.IsNullOrWhiteSpace(p.TenHanhVi) ? meta.ShortTitle : p.TenHanhVi))
            .Cast<object>()
            .ToList();

        decimal balance = HanhViGhiNhanHoSo_cl.GetSoDuTrongHoSoThat(account, profileCode);
        string heroStatus = requests.Count > 0
            ? requests.Count.ToString("#,##0") + " yêu cầu đã được ghi nhận"
            : "Chưa có yêu cầu nào trong hồ sơ này";
        decimal tongHopLe = filteredRows.Sum(p => p.SoDuHanhViHopLe);
        decimal tongNhan = filteredRows.Sum(p => p.SoDuDiemNhan);
        decimal tongCho = filteredRows.Sum(p => p.SoDuDangChoDuyet);
        decimal tongDaGhiNhan = filteredRows.Sum(p => p.SoDaGhiNhan);

        return new
        {
            ok = true,
            state = "ready",
            profile_key = meta.Key,
            meta = BuildMetaPayload(meta),
            hero = new
            {
                balance_label = meta.BalanceLabel,
                balance_value = FormatDecimal(balance),
                balance_suffix = "A",
                status_text = heroStatus
            },
            summary = new object[]
            {
                BuildStat("Số dư trong hồ sơ", FormatDecimal(summary.SoDuTrongHoSo) + " A", "accent"),
                BuildStat("Hành vi hợp lệ", FormatDecimal(tongHopLe), "positive"),
                BuildStat("Điểm nhận", FormatDecimal(tongNhan), "default"),
                BuildStat("Chờ duyệt", FormatDecimal(tongCho), "warning"),
                BuildStat("Đã ghi nhận", FormatDecimal(tongDaGhiNhan), "neutral")
            },
            note = note,
            requests = requests.Take(8).ToList(),
            behaviors = behaviors,
            filters = BuildPeriodFilters(periodKey),
            active_filter_label = GetPeriodLabel(periodKey),
            history = history,
            history_pagination = BuildPagination(currentPage, totalPages, totalCount),
            request_pagination = BuildPagination(1, 1, requests.Count)
        };
    }

    private static List<object> BuildRequests(dbDataContext db, string taiKhoan, int profileCode, int loaiHoSo, string periodKey)
    {
        int fromKy;
        int toKy;
        HanhViGhiNhanHoSo_cl.TryGetHanhViRange(profileCode, out fromKy, out toKy);

        var raw = db.YeuCauRutQuyen_tbs
            .Where(p => p.TaiKhoan == taiKhoan
                && p.LoaiHanhVi == profileCode
                && p.KyHieu9HanhVi_1_9 >= fromKy
                && p.KyHieu9HanhVi_1_9 <= toKy)
            .OrderByDescending(p => p.NgayTao)
            .Select(p => new
            {
                p.IdYeuCauRut,
                p.NgayTao,
                p.NgayCapNhat,
                p.KyHieu9HanhVi_1_9,
                p.TongQuyen,
                p.TrangThai,
                p.NguoiDuyet,
                p.GhiChu
            })
            .ToList();

        Dictionary<Guid, decimal> paidLookup = new Dictionary<Guid, decimal>();
        if (HanhViGhiNhanHoSo_cl.IsProfileChiTraQuanTri(profileCode) && raw.Count > 0)
        {
            Dictionary<string, Guid> refToId = raw.ToDictionary(
                p => HanhViGhiNhanHoSo_cl.GetPayoutRefId(p.IdYeuCauRut),
                p => p.IdYeuCauRut,
                StringComparer.OrdinalIgnoreCase);

            var payoutRows = db.LichSu_DongA_tbs
                .Where(p =>
                    p.taikhoan == taiKhoan
                    && p.LoaiHoSo_Vi == loaiHoSo
                    && p.CongTru.HasValue
                    && p.CongTru.Value == false
                    && p.id_rutdiem != null
                    && p.id_rutdiem.StartsWith(HanhViGhiNhanHoSo_cl.PayoutRefPrefix))
                .Select(p => new
                {
                    p.id_rutdiem,
                    p.dongA
                })
                .ToList();

            foreach (var payout in payoutRows)
            {
                Guid requestId;
                if (!refToId.TryGetValue((payout.id_rutdiem ?? "").Trim(), out requestId))
                    continue;

                decimal current = 0m;
                if (paidLookup.ContainsKey(requestId))
                    current = paidLookup[requestId];
                paidLookup[requestId] = current + (payout.dongA ?? 0m);
            }
        }

        return raw.Select(p =>
        {
            decimal daChiTra = paidLookup.ContainsKey(p.IdYeuCauRut) ? paidLookup[p.IdYeuCauRut] : 0m;
            string statusCode = HanhViGhiNhanHoSo_cl.NormalizeTrangThai(p.TrangThai);
            string statusText = HanhViGhiNhanHoSo_cl.GetTrangThaiText(p.TrangThai);
            if (HanhViGhiNhanHoSo_cl.IsProfileChiTraQuanTri(profileCode)
                && statusCode == HanhViGhiNhanHoSo_cl.TrangThaiDaDuyet
                && (p.TongQuyen - daChiTra) <= 0m)
            {
                statusCode = "3";
                statusText = "Đã chi trả";
            }

            return (object)new
            {
                id = p.IdYeuCauRut.ToString("N"),
                title = HanhVi9Cap_cl.GetTenHanhViKhongPhanTram(p.KyHieu9HanhVi_1_9),
                behavior_code = p.KyHieu9HanhVi_1_9,
                period_key = GetPeriodKeyByBehavior(p.KyHieu9HanhVi_1_9),
                created_at = FormatDateTime(p.NgayTao),
                updated_at = FormatDateTime(p.NgayCapNhat),
                amount_text = FormatDecimal(p.TongQuyen) + " A",
                status_code = statusCode,
                status_text = statusText,
                reviewer = CleanText(p.NguoiDuyet, "Đang chờ hệ thống xử lý"),
                note = NormalizeNote(p.GhiChu),
                detail_url = "/app-ui/home/benefit-request-detail.aspx?ui_mode=app&profile=" + HttpUtility.UrlEncode(GetProfileKeyByCode(profileCode)) + "&request_id=" + HttpUtility.UrlEncode(p.IdYeuCauRut.ToString("N"))
            };
        })
        .Where(p => PeriodMatches(periodKey, p.behavior_code))
        .Cast<object>()
        .ToList();
    }

    private static object BuildBlockedPayload(BenefitMeta meta, int tierHome)
    {
        return new
        {
            ok = false,
            reason = "tier_locked",
            state = "blocked",
            profile_key = meta.Key,
            meta = BuildMetaPayload(meta),
            notice = new
            {
                title = "Hồ sơ chưa mở quyền truy cập",
                message = meta.Key == "labor"
                    ? "Bạn cần đạt tầng cộng tác phát triển để xem hồ sơ hành vi lao động trên app."
                    : "Bạn cần đạt tầng đồng hành hệ sinh thái để xem hồ sơ chỉ số gắn kết trên app.",
                level = "warning",
                tier_text = "Tier Home hiện tại: " + tierHome.ToString("#,##0")
            }
        };
    }

    private static object BuildPagination(int currentPage, int totalPages, int totalItems)
    {
        return new
        {
            current_page = currentPage,
            total_pages = totalPages,
            total_items = totalItems,
            can_prev = currentPage > 1,
            can_next = currentPage < totalPages,
            label = totalItems > 0
                ? ("Trang " + currentPage.ToString("#,##0") + "/" + totalPages.ToString("#,##0") + " · " + totalItems.ToString("#,##0") + " mục")
                : "Chưa có dữ liệu"
        };
    }

    private static object BuildHistoryItem(long id, DateTime? ngay, decimal? amount, bool? congTru, string title, string note, string orderId, string contextTag)
    {
        bool isDebit = congTru.HasValue && congTru.Value;
        decimal absValue = Math.Abs(amount ?? 0m);
        return new
        {
            id = id,
            title = CleanText(title, "Biến động hồ sơ"),
            note = CleanText(note, "Hệ thống đã ghi nhận một biến động mới trong hồ sơ."),
            date_text = FormatDateTime(ngay),
            amount_text = (isDebit ? "-" : "+") + FormatDecimal(absValue) + " A",
            direction_code = isDebit ? "debit" : "credit",
            direction_text = isDebit ? "Trừ" : "Cộng",
            order_code = string.IsNullOrWhiteSpace(orderId) ? "" : orderId.Trim(),
            tag = CleanText(contextTag, "")
        };
    }

    private static object BuildStat(string label, string value, string tone)
    {
        return new
        {
            label = label,
            value = value,
            tone = tone
        };
    }

    private static object BuildMetaPayload(BenefitMeta meta)
    {
        return new
        {
            key = meta.Key,
            title = meta.Title,
            short_title = meta.ShortTitle,
            description = meta.Description,
            balance_label = meta.BalanceLabel,
            accent = meta.Accent
        };
    }

    private static BenefitMeta GetMeta(string profileKey)
    {
        switch ((profileKey ?? "").Trim().ToLowerInvariant())
        {
            case "offers":
                return new BenefitMeta
                {
                    Key = "offers",
                    Title = "Hồ sơ quyền ưu đãi",
                    ShortTitle = "Quyền ưu đãi",
                    Description = "Theo dõi số dư quyền ưu đãi, hành vi đủ điều kiện, các yêu cầu đã gửi và toàn bộ lịch sử ghi nhận trên app.",
                    BalanceLabel = "Số dư quyền ưu đãi",
                    Accent = "offers"
                };
            case "labor":
                return new BenefitMeta
                {
                    Key = "labor",
                    Title = "Hồ sơ hành vi lao động",
                    ShortTitle = "Hành vi lao động",
                    Description = "Màn hình app-native tập trung phần hành vi lao động, các điều kiện hợp lệ, yêu cầu chờ duyệt và lịch sử chi trả liên quan.",
                    BalanceLabel = "Số dư hồ sơ lao động",
                    Accent = "labor"
                };
            case "engagement":
                return new BenefitMeta
                {
                    Key = "engagement",
                    Title = "Hồ sơ chỉ số gắn kết",
                    ShortTitle = "Chỉ số gắn kết",
                    Description = "Theo dõi chỉ số gắn kết, trạng thái ghi nhận và lịch sử hệ sinh thái theo giao diện gọn cho app.",
                    BalanceLabel = "Số dư hồ sơ gắn kết",
                    Accent = "engagement"
                };
            default:
                return new BenefitMeta
                {
                    Key = "consumer",
                    Title = "Hồ sơ quyền tiêu dùng",
                    ShortTitle = "Quyền tiêu dùng",
                    Description = "Lịch sử quyền tiêu dùng trên app được trình bày lại theo kiểu làm việc nhanh: nhìn số dư, biến động và bối cảnh đơn hàng ngay trong một màn.",
                    BalanceLabel = "Số dư quyền tiêu dùng",
                    Accent = "consumer"
                };
        }
    }

    private static int ResolveProfileCode(string profileKey)
    {
        switch ((profileKey ?? "").Trim().ToLowerInvariant())
        {
            case "offers":
                return HanhViGhiNhanHoSo_cl.Profile_UuDai;
            case "labor":
                return HanhViGhiNhanHoSo_cl.Profile_LaoDong;
            case "engagement":
                return HanhViGhiNhanHoSo_cl.Profile_GanKet;
            default:
                return 0;
        }
    }

    private static string ResolveProfileKey(string raw)
    {
        string key = (raw ?? "").Trim().ToLowerInvariant();
        if (key == "offers" || key == "labor" || key == "engagement")
            return key;
        return "consumer";
    }

    private static string GetProfileKeyByCode(int profileCode)
    {
        if (profileCode == HanhViGhiNhanHoSo_cl.Profile_UuDai) return "offers";
        if (profileCode == HanhViGhiNhanHoSo_cl.Profile_LaoDong) return "labor";
        if (profileCode == HanhViGhiNhanHoSo_cl.Profile_GanKet) return "engagement";
        return "consumer";
    }

    private static string ResolvePeriodKey(string raw)
    {
        string key = (raw ?? "").Trim().ToLowerInvariant();
        if (key == "day" || key == "week" || key == "month")
            return key;
        return "all";
    }

    private static string GetPeriodKeyByBehavior(int? kyHieu)
    {
        if (!kyHieu.HasValue || kyHieu.Value <= 0)
            return "all";
        int nhom = ((kyHieu.Value - 1) % 3) + 1;
        if (nhom == 1) return "day";
        if (nhom == 2) return "week";
        return "month";
    }

    private static bool PeriodMatches(string periodKey, int? kyHieu)
    {
        string current = ResolvePeriodKey(periodKey);
        if (current == "all")
            return true;
        return string.Equals(GetPeriodKeyByBehavior(kyHieu), current, StringComparison.OrdinalIgnoreCase);
    }

    private static object[] BuildPeriodFilters(string activeKey)
    {
        string current = ResolvePeriodKey(activeKey);
        return new object[]
        {
            new { key = "all", label = "Tất cả", active = current == "all" },
            new { key = "day", label = "1 ngày", active = current == "day" },
            new { key = "week", label = "1 tuần", active = current == "week" },
            new { key = "month", label = "1 tháng", active = current == "month" }
        };
    }

    private static string GetPeriodLabel(string periodKey)
    {
        string current = ResolvePeriodKey(periodKey);
        if (current == "day") return "Mốc 1 ngày";
        if (current == "week") return "Mốc 1 tuần";
        if (current == "month") return "Mốc 1 tháng";
        return "Toàn bộ chu kỳ";
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

    private static string ResolveConsumerHistoryTitle(string note)
    {
        string text = (note ?? "").Trim();
        if (text == "")
            return "Biến động quyền tiêu dùng";
        if (text.Length <= 72)
            return text;
        return text.Substring(0, 72).Trim() + "...";
    }

    private static string NormalizeNote(string text)
    {
        string value = (text ?? "").Replace("|SHOPONLY|CREDIT_SELLER|", "").Trim();
        if (value == "")
            return "";
        return value;
    }

    private static string CleanText(string text, string fallback)
    {
        string value = (text ?? "").Trim();
        return value == "" ? fallback : value;
    }

    private static string FormatDecimal(decimal value)
    {
        return value.ToString("#,##0.##");
    }

    private static string FormatDateTime(DateTime? value)
    {
        if (!value.HasValue)
            return "Chưa cập nhật";
        return value.Value.ToString("dd/MM/yyyy HH:mm");
    }

    private static string ToJson(object payload)
    {
        return new JavaScriptSerializer().Serialize(payload);
    }
}
