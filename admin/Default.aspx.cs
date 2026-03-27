using System;
using System.Collections.Generic;
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

    private static string NormalizeManagementSpace(string space)
    {
        string normalized = AdminManagementSpace_cl.NormalizeSpace(space);
        return string.IsNullOrWhiteSpace(normalized) ? AdminManagementSpace_cl.SpaceAdmin : normalized;
    }

    private static string ResolveTabSpace(string url)
    {
        string raw = (url ?? "").Trim();
        if (string.IsNullOrWhiteSpace(raw))
            return "";

        string lower = raw.ToLowerInvariant();
        string path = lower;
        string query = "";
        int queryIndex = lower.IndexOf('?');
        if (queryIndex >= 0)
        {
            path = lower.Substring(0, queryIndex);
            if (queryIndex + 1 < lower.Length)
                query = lower.Substring(queryIndex + 1);
        }

        if (path == "/admin/default.aspx")
            return AdminManagementSpace_cl.SpaceAdmin;

        if (path.StartsWith("/gianhang/admin/", StringComparison.OrdinalIgnoreCase)
            || path == "/admin/duyet-gian-hang-doi-tac.aspx"
            || path == "/admin/duyet-shop-doi-tac.aspx"
            || path == "/admin/duyet-nang-cap-level2.aspx"
            || path.StartsWith("/admin/quan-ly-email-shop/", StringComparison.OrdinalIgnoreCase))
            return AdminManagementSpace_cl.SpaceGianHang;

        if (path.StartsWith("/daugia/admin/", StringComparison.OrdinalIgnoreCase))
            return AdminManagementSpace_cl.SpaceDauGia;

        if (path.StartsWith("/event/admin/", StringComparison.OrdinalIgnoreCase))
            return AdminManagementSpace_cl.SpaceEvent;

        if (path.StartsWith("/admin/quan-ly-noi-dung-home/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/admin/quan-ly-menu/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/admin/quan-ly-bai-viet/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/admin/quan-ly-banner/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/admin/cai-dat-trang-chu/", StringComparison.OrdinalIgnoreCase))
            return AdminManagementSpace_cl.SpaceContent;

        if (path == "/admin/quan-ly-tai-khoan/default.aspx")
        {
            if (query.Contains("scope=admin"))
                return AdminManagementSpace_cl.SpaceAdmin;
            if (query.Contains("scope=home"))
                return AdminManagementSpace_cl.SpaceHome;
            if (query.Contains("scope=shop"))
                return AdminManagementSpace_cl.SpaceGianHang;
        }

        if (path == "/admin/lich-su-chuyen-diem/default.aspx")
            return query.Contains("tab=shop-only")
                ? AdminManagementSpace_cl.SpaceGianHang
                : AdminManagementSpace_cl.SpaceHome;

        if (path == "/admin/duyet-yeu-cau-len-cap.aspx"
            || path == "/admin/motacapbac.aspx"
            || path.StartsWith("/admin/phat-hanh-the/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/admin/he-thong-san-pham/", StringComparison.OrdinalIgnoreCase))
            return AdminManagementSpace_cl.SpaceHome;

        if (path.StartsWith("/admin/otp/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/admin/quan-ly-gop-y/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/admin/quan-ly-thong-bao/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/admin/yeu-cau-tu-van/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/admin/vi-token-diem/", StringComparison.OrdinalIgnoreCase)
            || path == "/admin/tools/company-shop-sync.aspx"
            || path == "/admin/tools/reindex-baiviet.aspx")
            return AdminManagementSpace_cl.SpaceAdmin;

        return "";
    }

    private static bool ObjectGroupMatchesSpace(string currentSpace, string groupKey)
    {
        string normalizedSpace = NormalizeManagementSpace(currentSpace);
        string normalizedKey = (groupKey ?? "").Trim().ToLowerInvariant();

        if (normalizedSpace == AdminManagementSpace_cl.SpaceHome)
            return normalizedKey == "home_customer"
                || normalizedKey == "home_development"
                || normalizedKey == "home_ecosystem";

        if (normalizedSpace == AdminManagementSpace_cl.SpaceGianHang)
            return normalizedKey == "shop_partner";

        if (normalizedSpace == AdminManagementSpace_cl.SpaceContent)
            return normalizedKey == "home_content";

        return false;
    }

    private List<AdminHomeNoteView> BuildManagementSpaceNotes(dbDataContext db, string taiKhoan, string phanloai, string permissionRaw, string currentSpace)
    {
        var notes = new List<AdminHomeNoteView>();
        string normalizedSpace = NormalizeManagementSpace(currentSpace);
        bool isSuperAdmin = PermissionProfile_cl.IsRootAdmin(taiKhoan);

        if (normalizedSpace == AdminManagementSpace_cl.SpaceAdmin)
        {
            notes.Add(new AdminHomeNoteView { Text = "Đây là không gian quản trị admin. Chỉ các công cụ vận hành nội bộ của tài khoản admin mới được hiển thị tại đây." });
            notes.Add(new AdminHomeNoteView { Text = isSuperAdmin
                ? "Bạn đang dùng Super Admin nên có thể quản lý tài khoản admin, OTP, ví token điểm và các công cụ vận hành nội bộ khác."
                : "Tài khoản admin thường chỉ thấy các công cụ nội bộ đã được Super Admin cấp quyền." });
            notes.Add(new AdminHomeNoteView { Text = "Nếu mở URL thuộc không gian khác, hệ sẽ tự đưa bạn về đúng không gian quản trị đang chọn." });
            return notes;
        }

        if (normalizedSpace == AdminManagementSpace_cl.SpaceHome)
        {
            notes.Add(new AdminHomeNoteView { Text = "Không gian này chỉ hiển thị các tab quản trị hệ Home như tài khoản Home, lịch sử chuyển điểm, phát hành thẻ và các nghiệp vụ lõi của Home." });
            notes.Add(new AdminHomeNoteView { Text = "Mọi URL không thuộc phạm vi Home sẽ bị đưa về landing của không gian Home để tránh truy cập chéo." });
            return notes;
        }

        if (normalizedSpace == AdminManagementSpace_cl.SpaceGianHang)
        {
            notes.Add(new AdminHomeNoteView { Text = "Không gian này chỉ dành cho các tab quản trị gian hàng đối tác: duyệt không gian gian hàng, duyệt shop, quản lý shop và nghiệp vụ liên quan." });
            notes.Add(new AdminHomeNoteView { Text = "Tài khoản admin chỉ được nhìn thấy và thao tác trong phạm vi gian hàng đối tác đã được cấp." });
            return notes;
        }

        if (normalizedSpace == AdminManagementSpace_cl.SpaceDauGia)
        {
            notes.Add(new AdminHomeNoteView { Text = "Không gian này dành riêng cho quản trị đấu giá. Chỉ các tab và route của đấu giá được phép xuất hiện." });
            return notes;
        }

        if (normalizedSpace == AdminManagementSpace_cl.SpaceEvent)
        {
            notes.Add(new AdminHomeNoteView { Text = "Không gian này dành riêng cho quản trị sự kiện. Chỉ các tab và route của sự kiện được phép xuất hiện." });
            return notes;
        }

        if (normalizedSpace == AdminManagementSpace_cl.SpaceContent)
        {
            notes.Add(new AdminHomeNoteView { Text = "Không gian này chỉ hiển thị các tab chỉnh sửa nội dung Website như trang chủ, nội dung văn bản, menu, bài viết và banner." });
            notes.Add(new AdminHomeNoteView { Text = "Các tác vụ quản trị admin, Home, gian hàng, đấu giá và sự kiện sẽ không xuất hiện tại đây." });
            return notes;
        }

        notes.Add(new AdminHomeNoteView { Text = "Chỉ các tab thuộc không gian đang chọn mới được phép hiển thị và truy cập." });
        return notes;
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
            if (TryRedirectToSelectedManagementSpace())
                return;

            Session["title"] = "Trang chủ admin";
        }

        BindAdminHome();
    }

    private bool TryRedirectToSelectedManagementSpace()
    {
        string explicitSpace = AdminManagementSpace_cl.NormalizeSpace(Request.QueryString["mspace"]);
        if (!string.IsNullOrWhiteSpace(explicitSpace))
            return false;

        string taiKhoan = GetCurrentAdminAccount();
        if (string.IsNullOrWhiteSpace(taiKhoan))
            return false;

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == taiKhoan);
            if (account == null)
                return false;

            string target = AdminManagementSpace_cl.ResolveCurrentLandingUrl(db, account, Request);
            if (string.IsNullOrWhiteSpace(target))
                return false;

            string currentRaw = (Request.RawUrl ?? "").Trim();
            string targetRaw = (target ?? "").Trim();
            if (string.IsNullOrWhiteSpace(currentRaw) || string.IsNullOrWhiteSpace(targetRaw))
                return false;

            if (string.Equals(currentRaw, targetRaw, StringComparison.OrdinalIgnoreCase))
                return false;

            Response.Redirect(target, false);
            HttpContext currentContext = HttpContext.Current;
            if (currentContext != null && currentContext.ApplicationInstance != null)
                currentContext.ApplicationInstance.CompleteRequest();
            return true;
        }
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
            string currentSpace = NormalizeManagementSpace(AdminManagementSpace_cl.GetCurrentSpace(db, account, Request));
            string displayName = account != null && !string.IsNullOrWhiteSpace(account.hoten)
                ? account.hoten.Trim()
                : taiKhoan;
            string roleLabel = AdminRolePolicy_cl.GetAdminRoleLabel(db, taiKhoan, phanloai, permissionRaw);
            string roleDescription = AdminManagementSpace_cl.GetSubtitle(currentSpace);
            string primaryWorkspaceUrl = AdminManagementSpace_cl.ResolveLandingUrl(db, taiKhoan, currentSpace);

            var tabs = AdminRolePolicy_cl.BuildAdminHomeTabs(db, taiKhoan, phanloai, permissionRaw)
                .Where(item => string.Equals(ResolveTabSpace(item.Url), currentSpace, StringComparison.OrdinalIgnoreCase))
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

            var notes = BuildManagementSpaceNotes(db, taiKhoan, phanloai, permissionRaw, currentSpace);

            var objectGroups = AdminRolePolicy_cl.BuildAdminObjectGroups(db, taiKhoan, phanloai, permissionRaw)
                .Where(item => ObjectGroupMatchesSpace(currentSpace, item.Key))
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
                activeObjectGroups = "Không áp dụng cho không gian đang chọn";

            lb_admin_home_eyebrow.Text = "KHÔNG GIAN QUẢN TRỊ";
            lb_admin_home_title.Text = AdminManagementSpace_cl.GetTitle(currentSpace);
            lb_admin_home_account.Text = taiKhoan;
            lb_admin_home_name.Text = displayName;
            lb_admin_home_role.Text = roleLabel;
            lb_admin_home_scope.Text = AdminManagementSpace_cl.GetTitle(currentSpace);
            lb_admin_home_tab_count.Text = tabs.Count.ToString("#,##0");
            lb_admin_home_permission_summary.Text = string.IsNullOrWhiteSpace(AdminRolePolicy_cl.DescribeAdminRoleSummary(permissionRaw))
                ? roleLabel
                : AdminRolePolicy_cl.DescribeAdminRoleSummary(permissionRaw);
            lb_admin_home_object_scope.Text = activeObjectGroups;
            lb_admin_home_description.Text = roleDescription;
            lb_admin_home_primary_hint.Text = string.Equals(currentSpace, AdminManagementSpace_cl.SpaceAdmin, StringComparison.OrdinalIgnoreCase)
                ? "Đây là landing của Quản trị admin. Chỉ các tab nội bộ của admin được hiển thị bên dưới."
                : "Landing này luôn bám theo không gian quản trị đang chọn và sẽ khóa truy cập chéo sang không gian khác.";

            lb_admin_home_notes_title.Text = "Quy định đang áp dụng trong không gian này";
            lb_admin_home_notes_description.Text = "Các ghi chú dưới đây được sinh theo đúng không gian quản trị đang chọn, không trộn lẫn tab của không gian khác.";
            lb_admin_object_groups_title.Text = "Nhóm nghiệp vụ liên quan";
            lb_admin_object_groups_description.Text = "Chỉ hiển thị các nhóm nghiệp vụ thật sự liên quan tới không gian đang chọn.";
            lb_admin_home_tabs_title.Text = "Danh sách tab của không gian này";
            lb_admin_home_tabs_description.Text = "Chỉ các tab thuộc không gian quản trị đang chọn mới xuất hiện ở đây.";
            lb_admin_home_empty.Text = "Không gian này hiện chưa có tab nghiệp vụ nào được cấp cho tài khoản admin đang đăng nhập.";

            hl_primary_workspace.NavigateUrl = primaryWorkspaceUrl;
            string currentRaw = (Request.RawUrl ?? "").Trim();
            string targetRaw = (primaryWorkspaceUrl ?? "").Trim();
            hl_primary_workspace.Visible = !string.IsNullOrWhiteSpace(targetRaw)
                && !string.Equals(targetRaw, currentRaw, StringComparison.OrdinalIgnoreCase);
            hl_primary_workspace_inline.NavigateUrl = primaryWorkspaceUrl;
            hl_primary_workspace_inline.Text = primaryWorkspaceUrl;
            hl_primary_workspace_inline.ToolTip = "Mở khu làm việc chính";

            rpt_admin_home_notes.DataSource = notes;
            rpt_admin_home_notes.DataBind();
            rpt_admin_object_groups.DataSource = objectGroups;
            rpt_admin_object_groups.DataBind();
            pn_admin_object_groups_section.Visible = objectGroups.Count > 0;
            rpt_admin_home_tabs.DataSource = tabs;
            rpt_admin_home_tabs.DataBind();
            pn_admin_home_empty.Visible = tabs.Count == 0;
            pn_core_asset_notice.Visible = currentSpace == AdminManagementSpace_cl.SpaceAdmin && !PermissionProfile_cl.IsRootAdmin(taiKhoan);
        }
    }
}
