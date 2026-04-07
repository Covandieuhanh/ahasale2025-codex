using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class gianhang_person_hub_default : System.Web.UI.Page
{
    private IList<GianHangAdminPersonHub_cl.PersonSummary> _items = new List<GianHangAdminPersonHub_cl.PersonSummary>();
    protected int summary_total = 0;
    protected int summary_active = 0;
    protected int summary_pending = 0;
    protected int summary_none = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!GianHangSystemAdminGuard_cl.EnsurePageAccess(this))
            return;

        if (!AdvancedAdminAccessGate_cl.EnsurePageAccess(this))
            return;

        if (!IsPostBack)
            txt_keyword.Text = (Request.QueryString["keyword"] ?? "").Trim();

        LoadData();
    }

    private void LoadData()
    {
        string ownerKey = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();
        using (dbDataContext db = new dbDataContext())
        {
            IList<GianHangAdminPersonHub_cl.PersonSummary> snapshot = GianHangAdminPersonHub_cl.GetPeople(
                db,
                ownerKey,
                txt_keyword.Text,
                "all",
                Request.QueryString["source"]);
            summary_total = snapshot.Count;
            summary_active = snapshot.Count(p => p != null && p.LinkInfo != null && string.Equals(p.LinkInfo.Status, "active", StringComparison.OrdinalIgnoreCase));
            summary_pending = snapshot.Count(p => p != null && p.LinkInfo != null && string.Equals(p.LinkInfo.Status, "pending", StringComparison.OrdinalIgnoreCase));
            summary_none = snapshot.Count - summary_active - summary_pending;

            _items = ApplyLifecycleFilter(GianHangAdminPersonHub_cl.GetPeople(
                db,
                ownerKey,
                txt_keyword.Text,
                Request.QueryString["status"],
                Request.QueryString["source"]),
                Request.QueryString["lifecycle"]);
        }

        Repeater1.DataSource = _items;
        Repeater1.DataBind();
        ph_empty.Visible = _items.Count == 0;
    }

    protected void but_search_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildFilterUrl(Request.QueryString["status"], Request.QueryString["source"], Request.QueryString["lifecycle"]), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected string BuildFilterUrl(string status)
    {
        return BuildFilterUrl(status, Request.QueryString["source"], Request.QueryString["lifecycle"]);
    }

    protected string BuildFilterUrl(string status, string source)
    {
        return BuildFilterUrl(status, source, Request.QueryString["lifecycle"]);
    }

    protected string BuildFilterUrl(string status, string source, string lifecycle)
    {
        string normalizedStatus = NormalizeStatus(status);
        string normalizedSource = NormalizeSource(source);
        string normalizedLifecycle = NormalizeLifecycle(lifecycle);
        var query = HttpUtility.ParseQueryString(string.Empty);
        if (normalizedStatus != "all")
            query["status"] = normalizedStatus;
        if (normalizedSource != "all")
            query["source"] = normalizedSource;
        if (normalizedLifecycle != "all")
            query["lifecycle"] = normalizedLifecycle;

        string keyword = (txt_keyword.Text ?? "").Trim();
        if (keyword != "")
            query["keyword"] = keyword;

        string queryString = query.ToString();
        return "/gianhang/admin/quan-ly-con-nguoi/Default.aspx" + (queryString == "" ? "" : "?" + queryString);
    }

    protected string FilterButtonCss(string status)
    {
        return string.Equals(NormalizeStatus(Request.QueryString["status"]), NormalizeStatus(status), StringComparison.OrdinalIgnoreCase)
            ? "primary"
            : "light";
    }

    protected string SourceButtonCss(string source)
    {
        return string.Equals(NormalizeSource(Request.QueryString["source"]), NormalizeSource(source), StringComparison.OrdinalIgnoreCase)
            ? "success"
            : "light";
    }

    protected string LifecycleButtonCss(string lifecycle)
    {
        return string.Equals(NormalizeLifecycle(Request.QueryString["lifecycle"]), NormalizeLifecycle(lifecycle), StringComparison.OrdinalIgnoreCase)
            ? "warning"
            : "light";
    }

    protected string RenderRoleChips(string rolesSummary)
    {
        List<string> chips = (rolesSummary ?? "")
            .Split(new[] { "•" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => (p ?? "").Trim())
            .Where(p => p != "")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (chips.Count == 0)
            chips.Add("Hồ sơ người");

        return string.Join("", chips.Select(p => "<span class='personhub-role-chip'>" + HttpUtility.HtmlEncode(p) + "</span>"));
    }

    protected string RenderSourceCountHtml(GianHangAdminPersonHub_cl.PersonSummary item)
    {
        if (item == null)
            return "<span class='data-wrapper'><code class='bg-gray fg-white'>0</code></span>";

        string html = "<span class='data-wrapper'><code class='bg-grayBlue fg-white'>" + HttpUtility.HtmlEncode(item.SourceCount.ToString("#,##0")) + "</code></span>";
        if (item.RemovedSourceCount > 0)
            html += " <span class='data-wrapper'><code class='bg-orange fg-white'>-" + HttpUtility.HtmlEncode(item.RemovedSourceCount.ToString("#,##0")) + "</code></span>";
        return html;
    }

    protected string RenderLinkMeta(GianHangAdminPersonHub_cl.PersonSummary item)
    {
        if (item == null || item.LinkInfo == null)
            return "<small class='fg-gray'>Chưa có thông tin liên kết.</small>";

        if (item.SourceCount <= 0)
        {
            if (item.RemovedSourceCount > 0)
                return "<small class='fg-gray'>Hiện chưa còn vai trò nguồn trong không gian. Hồ sơ trung tâm vẫn được giữ và có <strong>" + HttpUtility.HtmlEncode(item.RemovedSourceCount.ToString("#,##0")) + "</strong> vai trò đã bị gỡ khỏi nguồn để bạn tra cứu lịch sử.</small>";
            return "<small class='fg-gray'>Hiện chưa còn vai trò nguồn trong không gian, nhưng liên kết Home trung tâm vẫn được giữ để bạn tiếp tục quản lý.</small>";
        }

        if (item.RemovedSourceCount > 0)
            return "<small class='fg-gray'>Hiện có <strong>" + HttpUtility.HtmlEncode(item.RemovedSourceCount.ToString("#,##0")) + "</strong> vai trò đã bị gỡ khỏi nguồn. Mở hồ sơ để xem chi tiết lịch sử.</small>";

        var link = item.LinkInfo;
        if (link.LinkedHomeAccount != null)
        {
            string display = string.IsNullOrWhiteSpace(link.LinkedHomeAccount.hoten)
                ? (link.LinkedHomeAccount.taikhoan ?? "")
                : link.LinkedHomeAccount.hoten;
            return "<small class='fg-gray'>Home: <strong>" + HttpUtility.HtmlEncode(display) + "</strong> • " + HttpUtility.HtmlEncode(link.LinkedHomeAccount.taikhoan) + "</small>";
        }

        if (!string.IsNullOrWhiteSpace(link.PendingPhone))
            return "<small class='fg-gray'>Chờ tài khoản Home của số <strong>" + HttpUtility.HtmlEncode(link.PendingPhone) + "</strong>.</small>";

        return "<small class='fg-gray'>Mở hồ sơ để gắn Home hoặc tạo chờ liên kết.</small>";
    }

    private static string NormalizeStatus(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        if (value == "active" || value == "pending" || value == "none")
            return value;
        return "all";
    }

    private static string NormalizeSource(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        if (value == "staff" || value == "customer" || value == "lecturer" || value == "member")
            return value;
        return "all";
    }

    private static string NormalizeLifecycle(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        if (value == "removed" || value == "orphaned")
            return value;
        return "all";
    }

    private static IList<GianHangAdminPersonHub_cl.PersonSummary> ApplyLifecycleFilter(IList<GianHangAdminPersonHub_cl.PersonSummary> items, string lifecycleRaw)
    {
        IList<GianHangAdminPersonHub_cl.PersonSummary> safeItems = items ?? new List<GianHangAdminPersonHub_cl.PersonSummary>();
        string lifecycle = NormalizeLifecycle(lifecycleRaw);
        if (lifecycle == "removed")
            return safeItems.Where(p => p != null && p.RemovedSourceCount > 0).ToList();
        if (lifecycle == "orphaned")
            return safeItems.Where(p => p != null && p.SourceCount <= 0).ToList();
        return safeItems;
    }
}
