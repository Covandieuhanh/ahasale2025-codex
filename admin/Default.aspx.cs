using System;
using System.Linq;
using System.Web;

public partial class admin_Default : System.Web.UI.Page
{
    private sealed class AdminHomeTabView
    {
        public string Title { get; set; }
        public string Meaning { get; set; }
        public string ActionSummary { get; set; }
        public string GuardrailSummary { get; set; }
        public string ScopeLabel { get; set; }
        public string ScopeMeaning { get; set; }
        public string Url { get; set; }
        public string CtaLabel { get; set; }
    }

    private sealed class AdminHomeNoteView
    {
        public string Text { get; set; }
    }

    private sealed class AdminObjectGroupView
    {
        public string Title { get; set; }
        public string ScopeLabel { get; set; }
        public string Meaning { get; set; }
        public string StatusLabel { get; set; }
        public string StatusHint { get; set; }
        public bool IsActive { get; set; }
    }

    private string ResolvePrimaryWorkspaceUrl(string taiKhoan)
    {
        if (string.IsNullOrWhiteSpace(taiKhoan))
            return "/admin/login.aspx";

        using (dbDataContext db = new dbDataContext())
            return AdminRolePolicy_cl.ResolvePrimaryWorkspaceUrl(db, taiKhoan);
    }

    private void DisablePageCaching()
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();
        Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        Response.Cache.SetValidUntilExpires(false);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        DisablePageCaching();

        if (!IsPostBack)
        {
            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLowerInvariant();
            check_login_cl.check_login_admin("none", "none");
            Session["title"] = "Trang chủ admin";
        }

        BindAdminHome();
    }

    private string GetCurrentAdminAccount()
    {
        string taiKhoanMaHoa = Session["taikhoan"] as string;
        if (string.IsNullOrWhiteSpace(taiKhoanMaHoa))
            return "";

        try
        {
            return mahoa_cl.giaima_Bcorn(taiKhoanMaHoa);
        }
        catch
        {
            return taiKhoanMaHoa;
        }
    }

    private void BindAdminHome()
    {
        string taiKhoan = GetCurrentAdminAccount();
        if (string.IsNullOrWhiteSpace(taiKhoan))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            var account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == taiKhoan);
            string phanloai = account != null ? (account.phanloai ?? "") : "";
            string permissionRaw = account != null ? (account.permission ?? "") : "";
            string scope = PortalScope_cl.ResolveScope(taiKhoan, phanloai, permissionRaw);
            string displayName = account != null && !string.IsNullOrWhiteSpace(account.hoten)
                ? account.hoten.Trim()
                : taiKhoan;
            string roleLabel = AdminRolePolicy_cl.GetAdminRoleLabel(db, taiKhoan, phanloai, permissionRaw);
            string roleDescription = AdminRolePolicy_cl.GetAdminRoleDescription(db, taiKhoan, phanloai, permissionRaw);
            string primaryWorkspaceUrl = ResolvePrimaryWorkspaceUrl(taiKhoan);

            var tabs = AdminRolePolicy_cl.BuildAdminHomeTabs(db, taiKhoan, phanloai, permissionRaw)
                .Select(item => new AdminHomeTabView
                {
                    Title = item.Title,
                    Meaning = item.Meaning,
                    ActionSummary = item.ActionSummary,
                    GuardrailSummary = item.GuardrailSummary,
                    ScopeLabel = item.ScopeLabel,
                    ScopeMeaning = item.ScopeMeaning,
                    Url = item.Url,
                    CtaLabel = "Mở tab"
                })
                .ToList();

            var notes = AdminRolePolicy_cl.BuildAdminPolicyNotes(db, taiKhoan, phanloai, permissionRaw)
                .Select(note => new AdminHomeNoteView { Text = note })
                .ToList();

            var objectGroups = AdminRolePolicy_cl.BuildAdminObjectGroups(db, taiKhoan, phanloai, permissionRaw)
                .Select(item => new AdminObjectGroupView
                {
                    Title = item.Title,
                    ScopeLabel = item.ScopeLabel,
                    Meaning = item.Meaning,
                    StatusLabel = item.StatusLabel,
                    StatusHint = item.StatusHint,
                    IsActive = item.IsActive
                })
                .ToList();
            string activeObjectGroups = string.Join(", ", objectGroups.Where(p => p.IsActive).Select(p => p.Title).ToList());
            if (string.IsNullOrWhiteSpace(activeObjectGroups))
                activeObjectGroups = "Chưa có nhóm nghiệp vụ cụ thể";

            lb_admin_home_account.Text = taiKhoan;
            lb_admin_home_name.Text = displayName;
            lb_admin_home_role.Text = roleLabel;
            lb_admin_home_scope.Text = AdminRolePolicy_cl.GetScopeDisplayLabel(scope);
            lb_admin_home_tab_count.Text = tabs.Count.ToString("#,##0");
            lb_admin_home_permission_summary.Text = string.IsNullOrWhiteSpace(AdminRolePolicy_cl.DescribeAdminRoleSummary(permissionRaw))
                ? roleLabel
                : AdminRolePolicy_cl.DescribeAdminRoleSummary(permissionRaw);
            lb_admin_home_object_scope.Text = activeObjectGroups;
            lb_admin_home_description.Text = roleDescription;
            lb_admin_home_primary_hint.Text = string.Equals(primaryWorkspaceUrl, "/admin/default.aspx", StringComparison.OrdinalIgnoreCase)
                ? "Trang này đang là điểm vào mặc định của tài khoản. Dùng danh sách tab bên dưới để vào đúng khu làm việc."
                : "Nút bên cạnh sẽ mở đúng khu làm việc chính theo vai trò của tài khoản này.";

            hl_primary_workspace.NavigateUrl = primaryWorkspaceUrl;
            hl_primary_workspace.Visible = !string.Equals(primaryWorkspaceUrl, "/admin/default.aspx", StringComparison.OrdinalIgnoreCase);
            hl_primary_workspace_inline.NavigateUrl = primaryWorkspaceUrl;
            hl_primary_workspace_inline.Text = primaryWorkspaceUrl;
            hl_primary_workspace_inline.ToolTip = "Mở khu làm việc chính";

            rpt_admin_home_notes.DataSource = notes;
            rpt_admin_home_notes.DataBind();
            rpt_admin_object_groups.DataSource = objectGroups;
            rpt_admin_object_groups.DataBind();
            rpt_admin_home_tabs.DataSource = tabs;
            rpt_admin_home_tabs.DataBind();
            pn_admin_home_empty.Visible = tabs.Count == 0;
            pn_core_asset_notice.Visible = !PermissionProfile_cl.IsRootAdmin(taiKhoan);
        }
    }
}
