using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_tai_khoan_phan_quyen : System.Web.UI.Page
{
    private string GetCurrentAdminUser()
    {
        string tkEnc = Session["taikhoan"] as string;
        if (string.IsNullOrEmpty(tkEnc))
            return "";

        try { return mahoa_cl.giaima_Bcorn(tkEnc); }
        catch { return tkEnc; }
    }

    private taikhoan_tb FindAccountByUsername(dbDataContext db, string taiKhoan)
    {
        if (db == null) return null;
        string tk = (taiKhoan ?? "").Trim();
        if (tk == "") return null;

        taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
        if (acc != null) return acc;

        return db.taikhoan_tbs
            .AsEnumerable()
            .FirstOrDefault(p => string.Equals((p.taikhoan ?? "").Trim(), tk, StringComparison.OrdinalIgnoreCase));
    }

    private string NormalizeFilterAccountType(string raw)
    {
        string type = (raw ?? "").Trim();
        return AccountType_cl.IsTreasury(type) ? AccountType_cl.Treasury : type;
    }

    private string NormalizeFilterScope(string raw)
    {
        return AdminDataScope_cl.NormalizeAccountScope(raw);
    }

    private string NormalizeFilterAdminRole(string raw)
    {
        string key = (raw ?? "").Trim();
        if (string.IsNullOrEmpty(key))
            return "";

        if (string.Equals(key, "super_admin", StringComparison.OrdinalIgnoreCase))
            return "super_admin";

        var preset = AdminRolePolicy_cl.GetScopedAdminPreset(key);
        return preset != null ? preset.Key : "";
    }

    private void RedirectTo(string url)
    {
        Response.Redirect(url, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private bool CanAccessPermissionTarget(dbDataContext db, taikhoan_tb account)
    {
        if (db == null || account == null)
            return false;

        string tkAdmin = GetCurrentAdminUser();
        if (string.IsNullOrWhiteSpace(tkAdmin))
            return false;

        string scope = PortalScope_cl.ResolveScope(account.taikhoan, account.phanloai, account.permission);
        return string.Equals(scope, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase)
            && AdminRolePolicy_cl.CanManageAdminAccounts(db, tkAdmin);
    }

    private string BuildListUrl()
    {
        string scope = AdminDataScope_cl.ResolveEffectiveAccountScope(
            Request.QueryString["scope"],
            Request.QueryString["fscope"]);
        string filterType = NormalizeFilterAccountType(Request.QueryString["ftype"]);
        string filterScope = NormalizeFilterScope(Request.QueryString["fscope"]);
        string filterAdminRole = NormalizeFilterAdminRole(Request.QueryString["frole"]);

        if (!string.IsNullOrEmpty(scope) && string.IsNullOrEmpty(filterScope))
            filterScope = scope;

        var query = new List<string>();
        if (!string.IsNullOrEmpty(scope))
            query.Add("scope=" + HttpUtility.UrlEncode(scope));
        if (!string.IsNullOrEmpty(filterType))
            query.Add("ftype=" + HttpUtility.UrlEncode(filterType));
        if (!string.IsNullOrEmpty(filterScope))
            query.Add("fscope=" + HttpUtility.UrlEncode(filterScope));
        if (!string.IsNullOrEmpty(filterAdminRole))
            query.Add("frole=" + HttpUtility.UrlEncode(filterAdminRole));

        return "/admin/quan-ly-tai-khoan/Default.aspx" + (query.Count > 0 ? "?" + string.Join("&", query) : "");
    }

    private void ShowMessage(string text, string cssModifier)
    {
        pnMessage.Visible = !string.IsNullOrWhiteSpace(text);
        pnMessage.CssClass = "admin-permission-message" + (string.IsNullOrWhiteSpace(cssModifier) ? "" : " " + cssModifier.Trim());
        litMessage.Text = Server.HtmlEncode(text ?? "");
    }

    private void SelectPermissions(CheckBoxList list, IEnumerable<string> permissionCodes)
    {
        var selected = new HashSet<string>((permissionCodes ?? Enumerable.Empty<string>()).Select(x => (x ?? "").Trim()), StringComparer.OrdinalIgnoreCase);
        foreach (ListItem item in list.Items)
            item.Selected = selected.Contains((item.Value ?? "").Trim());
    }

    private void SyncPermissionGroup(CheckBox groupCheckBox, CheckBoxList permissionList)
    {
        groupCheckBox.Checked = permissionList.Items.Count > 0 && permissionList.Items.Cast<ListItem>().All(i => i.Selected);
    }

    private void TogglePermissionGroup(CheckBox groupCheckBox, CheckBoxList permissionList)
    {
        bool isChecked = groupCheckBox.Checked;
        foreach (ListItem item in permissionList.Items)
            item.Selected = isChecked;
    }

    private void RefreshPermissionGroupChecks()
    {
        SyncPermissionGroup(check_all_quyen_quanlynhanvien, check_list_quyen_quanlynhanvien);
        SyncPermissionGroup(check_all_quyen_1, check_list_quyen_1);
        SyncPermissionGroup(check_all_quyen_home_customer, check_list_quyen_home_customer);
        SyncPermissionGroup(check_all_quyen_home_development, check_list_quyen_home_development);
        SyncPermissionGroup(check_all_quyen_home_ecosystem, check_list_quyen_home_ecosystem);
        SyncPermissionGroup(check_all_quyen_shop_partner, check_list_quyen_shop_partner);
        SyncPermissionGroup(check_all_quyen_home_content, check_list_quyen_home_content);
    }

    private void ApplyAdminPermissionPreset(string presetKey)
    {
        var preset = AdminRolePolicy_cl.GetScopedAdminPreset((presetKey ?? "").Trim());
        if (preset == null)
            return;

        SelectPermissions(check_list_quyen_quanlynhanvien, Enumerable.Empty<string>());
        SelectPermissions(check_list_quyen_1, Enumerable.Empty<string>());
        SelectPermissions(check_list_quyen_home_customer, Enumerable.Empty<string>());
        SelectPermissions(check_list_quyen_home_development, Enumerable.Empty<string>());
        SelectPermissions(check_list_quyen_home_ecosystem, Enumerable.Empty<string>());
        SelectPermissions(check_list_quyen_shop_partner, Enumerable.Empty<string>());
        SelectPermissions(check_list_quyen_home_content, Enumerable.Empty<string>());

        if (preset.PermissionCodes != null)
        {
            SelectPermissions(check_list_quyen_home_customer, preset.PermissionCodes);
            SelectPermissions(check_list_quyen_home_development, preset.PermissionCodes);
            SelectPermissions(check_list_quyen_home_ecosystem, preset.PermissionCodes);
            SelectPermissions(check_list_quyen_shop_partner, preset.PermissionCodes);
            SelectPermissions(check_list_quyen_home_content, preset.PermissionCodes);
        }

        RefreshPermissionGroupChecks();
    }

    private void SyncAdminPermissionPresetDropdown(string permissionRaw)
    {
        string presetKey = AdminRolePolicy_cl.MatchScopedAdminPresetKey(permissionRaw);
        ListItem item = ddl_admin_permission_preset.Items.FindByValue(presetKey);
        ddl_admin_permission_preset.SelectedValue = item != null ? presetKey : "";
    }

    private void ResetPermissionControls()
    {
        check_all_quyen_quanlynhanvien.Checked = false;
        check_list_quyen_quanlynhanvien.SelectedIndex = -1;
        check_all_quyen_1.Checked = false;
        check_list_quyen_1.SelectedIndex = -1;
        check_all_quyen_home_customer.Checked = false;
        check_list_quyen_home_customer.SelectedIndex = -1;
        check_all_quyen_home_development.Checked = false;
        check_list_quyen_home_development.SelectedIndex = -1;
        check_all_quyen_home_ecosystem.Checked = false;
        check_list_quyen_home_ecosystem.SelectedIndex = -1;
        check_all_quyen_shop_partner.Checked = false;
        check_list_quyen_shop_partner.SelectedIndex = -1;
        check_all_quyen_home_content.Checked = false;
        check_list_quyen_home_content.SelectedIndex = -1;
        if (ddl_admin_permission_preset.Items.Count > 0)
            ddl_admin_permission_preset.SelectedValue = "";
        ViewState["tk_phanquyen"] = null;
    }

    private void LoadPermissionForm(string taiKhoan)
    {
        ResetPermissionControls();
        string tk = (taiKhoan ?? "").Trim();
        if (string.IsNullOrWhiteSpace(tk))
        {
            ShowMessage("Không xác định được tài khoản cần phân quyền.", "error");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb account = FindAccountByUsername(db, tk);
            if (account == null)
            {
                ShowMessage("Không tìm thấy tài khoản cần phân quyền.", "error");
                return;
            }

            string targetScope = PortalScope_cl.ResolveScope(account.taikhoan, account.phanloai, account.permission);
            if (!string.Equals(targetScope, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase))
            {
                ShowMessage("Chỉ tài khoản thuộc hệ admin mới được phân quyền tại màn này.", "error");
                return;
            }

            if (!CanAccessPermissionTarget(db, account))
            {
                ShowMessage("Bạn không có quyền phân quyền cho tài khoản này.", "error");
                return;
            }

            if (PermissionProfile_cl.IsRootAdmin(account.taikhoan))
            {
                ShowMessage("Tài khoản admin gốc luôn giữ toàn quyền và không chỉnh ở màn hình này.", "error");
                return;
            }

            ViewState["tk_phanquyen"] = account.taikhoan;
            litAccount.Text = Server.HtmlEncode(account.taikhoan);
            litRole.Text = Server.HtmlEncode(AdminRolePolicy_cl.GetAdminRoleLabel(db, account.taikhoan, account.phanloai, account.permission));
            litRoleDesc.Text = Server.HtmlEncode(AdminRolePolicy_cl.GetAdminRoleDescription(db, account.taikhoan, account.phanloai, account.permission));

            string permissionRaw = account.permission ?? "";
            if (!string.IsNullOrWhiteSpace(permissionRaw))
            {
                string[] quyenArray = permissionRaw.Split(',');

                foreach (ListItem item in check_list_quyen_quanlynhanvien.Items)
                    item.Selected = quyenArray.Contains(item.Value);
                foreach (ListItem item in check_list_quyen_1.Items)
                    item.Selected = quyenArray.Contains(item.Value);
                foreach (ListItem item in check_list_quyen_home_customer.Items)
                    item.Selected = quyenArray.Contains(item.Value);
                foreach (ListItem item in check_list_quyen_home_development.Items)
                    item.Selected = quyenArray.Contains(item.Value);
                foreach (ListItem item in check_list_quyen_home_ecosystem.Items)
                    item.Selected = quyenArray.Contains(item.Value);
                foreach (ListItem item in check_list_quyen_shop_partner.Items)
                    item.Selected = quyenArray.Contains(item.Value);
                foreach (ListItem item in check_list_quyen_home_content.Items)
                    item.Selected = quyenArray.Contains(item.Value);

                RefreshPermissionGroupChecks();
                SyncAdminPermissionPresetDropdown(permissionRaw);
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri;
        AdminAccessGuard_cl.RequireFeatureAccess("admin_accounts", "/admin/default.aspx?mspace=admin");

        if (!IsPostBack)
        {
            string listUrl = BuildListUrl();
            hlBackTop.NavigateUrl = listUrl;
            hlBackBottom.NavigateUrl = listUrl;

            if (Session["thongbao"] != null)
            {
                ViewState["thongbao"] = Session["thongbao"].ToString();
                Session["thongbao"] = null;
            }

            LoadPermissionForm(Request.QueryString["tk"]);
        }
    }

    protected void ddl_admin_permission_preset_SelectedIndexChanged(object sender, EventArgs e)
    {
        AdminAccessGuard_cl.RequireFeatureAccess("admin_accounts", "/admin/default.aspx?mspace=admin");
        ApplyAdminPermissionPreset(ddl_admin_permission_preset.SelectedValue);
    }

    protected void check_all_quyen_quanlynhanvien_CheckedChanged(object sender, EventArgs e)
    {
        TogglePermissionGroup(check_all_quyen_quanlynhanvien, check_list_quyen_quanlynhanvien);
    }

    protected void check_list_quyen_quanlynhanvien_SelectedIndexChanged(object sender, EventArgs e)
    {
        SyncPermissionGroup(check_all_quyen_quanlynhanvien, check_list_quyen_quanlynhanvien);
    }

    protected void check_all_quyen_1_CheckedChanged(object sender, EventArgs e)
    {
        TogglePermissionGroup(check_all_quyen_1, check_list_quyen_1);
    }

    protected void check_list_quyen_1_SelectedIndexChanged(object sender, EventArgs e)
    {
        SyncPermissionGroup(check_all_quyen_1, check_list_quyen_1);
    }

    protected void check_all_quyen_home_customer_CheckedChanged(object sender, EventArgs e)
    {
        TogglePermissionGroup(check_all_quyen_home_customer, check_list_quyen_home_customer);
    }

    protected void check_list_quyen_home_customer_SelectedIndexChanged(object sender, EventArgs e)
    {
        SyncPermissionGroup(check_all_quyen_home_customer, check_list_quyen_home_customer);
    }

    protected void check_all_quyen_home_development_CheckedChanged(object sender, EventArgs e)
    {
        TogglePermissionGroup(check_all_quyen_home_development, check_list_quyen_home_development);
    }

    protected void check_list_quyen_home_development_SelectedIndexChanged(object sender, EventArgs e)
    {
        SyncPermissionGroup(check_all_quyen_home_development, check_list_quyen_home_development);
    }

    protected void check_all_quyen_home_ecosystem_CheckedChanged(object sender, EventArgs e)
    {
        TogglePermissionGroup(check_all_quyen_home_ecosystem, check_list_quyen_home_ecosystem);
    }

    protected void check_list_quyen_home_ecosystem_SelectedIndexChanged(object sender, EventArgs e)
    {
        SyncPermissionGroup(check_all_quyen_home_ecosystem, check_list_quyen_home_ecosystem);
    }

    protected void check_all_quyen_shop_partner_CheckedChanged(object sender, EventArgs e)
    {
        TogglePermissionGroup(check_all_quyen_shop_partner, check_list_quyen_shop_partner);
    }

    protected void check_list_quyen_shop_partner_SelectedIndexChanged(object sender, EventArgs e)
    {
        SyncPermissionGroup(check_all_quyen_shop_partner, check_list_quyen_shop_partner);
    }

    protected void check_all_quyen_home_content_CheckedChanged(object sender, EventArgs e)
    {
        TogglePermissionGroup(check_all_quyen_home_content, check_list_quyen_home_content);
    }

    protected void check_list_quyen_home_content_SelectedIndexChanged(object sender, EventArgs e)
    {
        SyncPermissionGroup(check_all_quyen_home_content, check_list_quyen_home_content);
    }

    protected void but_phanquyen_Click(object sender, EventArgs e)
    {
        AdminAccessGuard_cl.RequireFeatureAccess("admin_accounts", "/admin/default.aspx?mspace=admin");
        string tk = (ViewState["tk_phanquyen"] ?? "").ToString().Trim();
        if (string.IsNullOrWhiteSpace(tk))
        {
            ShowMessage("Không xác định được tài khoản cần lưu phân quyền.", "error");
            return;
        }

        if (PermissionProfile_cl.IsRootAdmin(tk))
        {
            ShowMessage("Tài khoản admin gốc luôn giữ toàn quyền và không chỉnh ở màn hình này.", "error");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb account = FindAccountByUsername(db, tk);
            if (account == null)
            {
                ShowMessage("Không tìm thấy tài khoản cần lưu phân quyền.", "error");
                return;
            }

            string existingScope = PortalScope_cl.ResolveScope(account.taikhoan, account.phanloai, account.permission);
            if (!string.Equals(existingScope, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase))
            {
                ShowMessage("Tài khoản này không thuộc hệ admin nên không áp dụng phân quyền admin.", "error");
                return;
            }

            if (!CanAccessPermissionTarget(db, account))
            {
                ShowMessage("Bạn không có quyền phân quyền cho tài khoản này.", "error");
                return;
            }

            var allPermissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string code in check_list_quyen_quanlynhanvien.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => (i.Value ?? "").Trim()))
                if (code != "") allPermissions.Add(code);
            foreach (string code in check_list_quyen_1.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => (i.Value ?? "").Trim()))
                if (code != "") allPermissions.Add(code);
            foreach (string code in check_list_quyen_home_customer.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => (i.Value ?? "").Trim()))
                if (code != "") allPermissions.Add(code);
            foreach (string code in check_list_quyen_home_development.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => (i.Value ?? "").Trim()))
                if (code != "") allPermissions.Add(code);
            foreach (string code in check_list_quyen_home_ecosystem.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => (i.Value ?? "").Trim()))
                if (code != "") allPermissions.Add(code);
            foreach (string code in check_list_quyen_shop_partner.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => (i.Value ?? "").Trim()))
                if (code != "") allPermissions.Add(code);
            foreach (string code in check_list_quyen_home_content.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => (i.Value ?? "").Trim()))
                if (code != "") allPermissions.Add(code);

            string selectedPermissions = string.Join(",", allPermissions.OrderBy(x => x));
            string targetScope = PortalScope_cl.ResolveScope(account.taikhoan, account.phanloai, account.permission);
            if (PortalScope_cl.ContainsAnyAdminPermission(selectedPermissions) || AccountType_cl.IsTreasury(account.phanloai) || PermissionProfile_cl.IsRootAdmin(account.taikhoan))
                targetScope = PortalScope_cl.ScopeAdmin;

            account.permission = PortalScope_cl.NormalizePermissionWithScope(selectedPermissions, targetScope);
            db.SubmitChanges();
        }

        Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đã lưu phân quyền cho tài khoản " + tk + ".", "1200", "success");
        RedirectTo(BuildListUrl());
    }
}
