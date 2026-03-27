using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class event_default : Page
{
    protected sealed class EventPublicSummaryInfo
    {
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int PausedCount { get; set; }
        public int SelectedScopeCount { get; set; }
    }

    protected EventPublicSummaryInfo Summary = new EventPublicSummaryInfo();

    protected void Page_Load(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            EventBootstrap_cl.EnsureSchemaSafe(db);
            if (!IsPostBack)
            {
                BindFilters();
                BindState(db);
            }
        }
    }

    private void BindFilters()
    {
        ddlStatusFilter.Items.Clear();
        ddlStatusFilter.Items.Add(new ListItem("Tất cả trạng thái", ""));
        ddlStatusFilter.Items.Add(new ListItem("Đang chạy", EventPolicy_cl.StatusActive));
        ddlStatusFilter.Items.Add(new ListItem("Tạm dừng", EventPolicy_cl.StatusPaused));

        ddlTypeFilter.Items.Clear();
        ddlTypeFilter.Items.Add(new ListItem("Tất cả loại", ""));
        ddlTypeFilter.Items.Add(new ListItem("Tích điểm voucher bậc thang", EventPolicy_cl.CampaignTypeVoucherTier));
        ddlTypeFilter.Items.Add(new ListItem("Trả lương / thưởng bậc thang", EventPolicy_cl.CampaignTypeSalaryBonusTier));
    }

    private void BindState(dbDataContext db)
    {
        List<EventService_cl.ProgramPublicRow> all = EventService_cl.LoadPublishedPrograms(db, "", "", 500);

        Summary = new EventPublicSummaryInfo
        {
            TotalCount = all.Count,
            ActiveCount = all.Count(p => string.Equals((p.Status ?? "").Trim(), EventPolicy_cl.StatusActive, StringComparison.OrdinalIgnoreCase)),
            PausedCount = all.Count(p => string.Equals((p.Status ?? "").Trim(), EventPolicy_cl.StatusPaused, StringComparison.OrdinalIgnoreCase)),
            SelectedScopeCount = all.Count(p => string.Equals((p.ShopScope ?? "").Trim(), "selected", StringComparison.OrdinalIgnoreCase))
        };

        string statusFilter = (ddlStatusFilter.SelectedValue ?? "").Trim().ToLowerInvariant();
        string typeFilter = (ddlTypeFilter.SelectedValue ?? "").Trim().ToLowerInvariant();
        string keyword = (txtKeyword.Text ?? "").Trim();

        IEnumerable<EventService_cl.ProgramPublicRow> query = all;

        if (statusFilter != "")
        {
            string status = EventPolicy_cl.NormalizeStatus(statusFilter);
            query = query.Where(p => string.Equals((p.Status ?? "").Trim(), status, StringComparison.OrdinalIgnoreCase));
        }

        if (typeFilter != "")
        {
            string type = EventPolicy_cl.NormalizeCampaignType(typeFilter);
            query = query.Where(p => string.Equals((p.CampaignType ?? "").Trim(), type, StringComparison.OrdinalIgnoreCase));
        }

        if (keyword != "")
        {
            long idSearch;
            bool byId = long.TryParse(keyword, out idSearch);
            query = query.Where(p =>
                (byId && p.ProgramID == idSearch)
                || ((p.CampaignCode ?? "").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                || ((p.CampaignName ?? "").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                || ((p.PublishedBy ?? "").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        List<EventService_cl.ProgramPublicRow> list = query
            .OrderByDescending(p => p.PublishedAt ?? DateTime.MinValue)
            .ThenByDescending(p => p.ProgramID)
            .ToList();

        rptPrograms.DataSource = list;
        rptPrograms.DataBind();
        phEmpty.Visible = list.Count == 0;
    }

    protected void butSearch_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            EventBootstrap_cl.EnsureSchemaSafe(db);
            BindState(db);
        }
    }

    protected string Safe(object value)
    {
        return HttpUtility.HtmlEncode(value == null ? "" : value.ToString());
    }

    protected string BuildTypeLabel(object typeObj)
    {
        string type = EventPolicy_cl.NormalizeCampaignType(typeObj == null ? "" : typeObj.ToString());
        if (type == EventPolicy_cl.CampaignTypeSalaryBonusTier)
            return "Lương / thưởng bậc thang";
        return "Voucher bậc thang";
    }

    protected string BuildTypeBadgeCss(object typeObj)
    {
        string type = EventPolicy_cl.NormalizeCampaignType(typeObj == null ? "" : typeObj.ToString());
        if (type == EventPolicy_cl.CampaignTypeSalaryBonusTier)
            return "badge bg-orange-lt text-orange";
        return "badge bg-blue-lt text-blue";
    }

    protected string BuildStatusLabel(object statusObj)
    {
        string status = EventPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        switch (status)
        {
            case EventPolicy_cl.StatusActive:
                return "Đang chạy";
            case EventPolicy_cl.StatusPaused:
                return "Tạm dừng";
            case EventPolicy_cl.StatusEnded:
                return "Kết thúc";
            case EventPolicy_cl.StatusArchived:
                return "Lưu trữ";
            default:
                return "Nháp";
        }
    }

    protected string BuildStatusBadgeCss(object statusObj)
    {
        string status = EventPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        if (status == EventPolicy_cl.StatusActive)
            return "badge bg-green-lt text-green";
        if (status == EventPolicy_cl.StatusPaused)
            return "badge bg-yellow-lt text-yellow";
        if (status == EventPolicy_cl.StatusEnded)
            return "badge bg-red-lt text-red";
        if (status == EventPolicy_cl.StatusArchived)
            return "badge bg-secondary-lt text-secondary";
        return "badge bg-primary-lt text-primary";
    }

    protected string BuildScopeLabel(object scopeObj, object targetCountObj)
    {
        string scope = (scopeObj == null ? "" : scopeObj.ToString()).Trim().ToLowerInvariant();
        int targetCount;
        if (!int.TryParse(targetCountObj == null ? "0" : targetCountObj.ToString(), out targetCount))
            targetCount = 0;

        if (scope == "selected")
            return "Scope: Chỉ định (" + targetCount.ToString("#,##0") + " shop)";

        return "Scope: Tất cả shop";
    }

    protected string BuildRangeLabel(object startObj, object endObj)
    {
        string start = FormatDate(startObj);
        string end = FormatDate(endObj);
        if (start == "-" && end == "-")
            return "Không giới hạn";
        return start + " -> " + end;
    }

    protected string BuildDefinitionSummary(object definitionObj, object typeObj)
    {
        string definition = definitionObj == null ? "" : definitionObj.ToString();
        string campaignType = EventPolicy_cl.NormalizeCampaignType(typeObj == null ? "" : typeObj.ToString());

        double step = ExtractDouble(definition, "step_percent", 5);
        double max = ExtractDouble(definition, "max_percent", 50);
        int cap = ExtractInt(definition, "cap_occurrence", 10);

        string rewardLabel = campaignType == EventPolicy_cl.CampaignTypeSalaryBonusTier
            ? "thưởng/lương"
            : "voucher";

        return "Cơ chế " + rewardLabel + ": +" + step.ToString("#,##0.##", CultureInfo.InvariantCulture)
            + "% mỗi lần, trần " + max.ToString("#,##0.##", CultureInfo.InvariantCulture)
            + "%, cap " + cap.ToString("#,##0") + " lần.";
    }

    protected string FormatDate(object dateObj)
    {
        if (dateObj == null)
            return "-";

        DateTime date;
        if (!DateTime.TryParse(dateObj.ToString(), out date))
            return "-";

        return date.ToString("dd/MM/yyyy HH:mm");
    }

    private double ExtractDouble(string json, string key, double fallback)
    {
        if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(key))
            return fallback;

        Match m = Regex.Match(
            json,
            "\\\"" + Regex.Escape(key) + "\\\"\\s*:\\s*([-+]?[0-9]*\\.?[0-9]+)",
            RegexOptions.IgnoreCase);

        if (!m.Success)
            return fallback;

        double value;
        if (!double.TryParse(m.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            return fallback;

        if (double.IsNaN(value) || double.IsInfinity(value))
            return fallback;

        return value;
    }

    private int ExtractInt(string json, string key, int fallback)
    {
        if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(key))
            return fallback;

        Match m = Regex.Match(
            json,
            "\\\"" + Regex.Escape(key) + "\\\"\\s*:\\s*([0-9]+)",
            RegexOptions.IgnoreCase);

        if (!m.Success)
            return fallback;

        int value;
        if (!int.TryParse(m.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            return fallback;

        if (value <= 0)
            return fallback;

        return value;
    }
}
