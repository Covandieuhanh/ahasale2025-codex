using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public partial class gianhang_nang_cap_level2 : System.Web.UI.Page
{
    private const string NoticeSessionKey = "gianhang_level2_notice";

    private sealed class Level2StaffItem
    {
        public string TaiKhoan { get; set; }
        public string HoTen { get; set; }
        public string ChiNhanh { get; set; }
        public string TrangThai { get; set; }
        public string PermissionSummary { get; set; }
        public bool IsOwner { get; set; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (GianHangContext_cl.EnsureCurrentAccess(this) == null)
            return;

        if (!IsPostBack)
            BindPage();
    }

    protected void but_enable_level2_Click(object sender, EventArgs e)
    {
        string shopAccount = CurrentAccountKey();
        if (string.IsNullOrWhiteSpace(shopAccount))
        {
            Response.Redirect("/gianhang/default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            string message;
            if (!ShopLevel2Request_cl.TryCreateRequest(db, shopAccount, out message))
            {
                Session[NoticeSessionKey] = string.IsNullOrWhiteSpace(message)
                    ? "Không thể gửi yêu cầu mở quản trị nâng cao."
                    : message;
                Response.Redirect("/gianhang/nang-cap-level2.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            Session[NoticeSessionKey] = message;
        }

        Response.Redirect("/gianhang/nang-cap-level2.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void but_reset_owner_admin_Click(object sender, EventArgs e)
    {
        string shopAccount = CurrentAccountKey();
        if (string.IsNullOrWhiteSpace(shopAccount))
        {
            Response.Redirect("/gianhang/default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            if (!ShopLevel_cl.IsAdvancedEnabled(db, shopAccount))
            {
                Session[NoticeSessionKey] = "Gian hàng này chưa bật quản trị nâng cao nên chưa thể đặt lại mật khẩu quản trị viên chính.";
                Response.Redirect("/gianhang/nang-cap-level2.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            taikhoan_table_2023 ownerAdmin = AhaShineContext_cl.EnsureAdvancedAdminBootstrapForShop(db, shopAccount);
            if (ownerAdmin == null || string.IsNullOrWhiteSpace(ownerAdmin.taikhoan))
            {
                Session[NoticeSessionKey] = "Chưa tìm thấy tài khoản quản trị viên chính để đặt lại mật khẩu.";
                Response.Redirect("/gianhang/nang-cap-level2.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            Session["user_reset_pass_admin"] = ownerAdmin.taikhoan;
        }

        Response.Redirect("/gianhang/admin/quen-mat-khau/dat-lai-mat-khau.aspx?from=gianhang", false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void BindPage()
    {
        string shopAccount = CurrentAccountKey();
        if (string.IsNullOrWhiteSpace(shopAccount))
        {
            Response.Redirect("/gianhang/default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb shop = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == shopAccount);
            if (shop == null)
            {
                Response.Redirect("/gianhang/default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            bool hasAdvanced = ShopLevel_cl.IsAdvancedEnabled(db, shopAccount);
            taikhoan_table_2023 ownerAdmin = hasAdvanced
                ? AhaShineContext_cl.EnsureAdvancedAdminBootstrapForShop(db, shopAccount)
                : db.taikhoan_table_2023s.FirstOrDefault(p => p.taikhoan == ShopLevel_cl.ResolveOwnerAdminUsername(shopAccount));
            string chiNhanhId = AhaShineContext_cl.ResolveChiNhanhIdForShopAccount(db, shopAccount);
            chinhanh_table branch = db.chinhanh_tables.FirstOrDefault(p => p.id.ToString() == chiNhanhId);
            ShopLevel2Request_cl.Level2RequestInfo latestRequest = ShopLevel2Request_cl.GetLatestRequest(db, shopAccount);
            bool pendingRequest = !hasAdvanced && latestRequest != null && latestRequest.TrangThai == ShopLevel2Request_cl.StatusPending;
            bool rejectedRequest = !hasAdvanced && latestRequest != null && latestRequest.TrangThai == ShopLevel2Request_cl.StatusRejected;

            lb_shop_name.Text = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(shop.ten_shop) ? shop.taikhoan : shop.ten_shop.Trim());
            lb_shop_level.Text = hasAdvanced ? "Nâng cao" : "Cơ bản";
            lb_admin_owner.Text = HttpUtility.HtmlEncode(ownerAdmin == null ? ShopLevel_cl.ResolveOwnerAdminUsername(shopAccount) : ownerAdmin.taikhoan);
            lb_admin_owner_duplicate.Text = HttpUtility.HtmlEncode(ownerAdmin == null ? ShopLevel_cl.ResolveOwnerAdminUsername(shopAccount) : ownerAdmin.taikhoan);
            lb_branch.Text = HttpUtility.HtmlEncode(branch == null ? "Chi nhánh mặc định" : branch.ten);
            lb_admin_account_info.Text = "Tài khoản quản trị viên chính mặc định dùng tên đăng nhập " + HttpUtility.HtmlEncode(ShopLevel_cl.ResolveOwnerAdminUsername(shopAccount)) + ".";
            lb_password_info.Text = ownerAdmin == null
                ? "Khi được duyệt quản trị nâng cao, tài khoản quản trị viên chính mới sẽ có mật khẩu mặc định 123456."
                : "Nếu tài khoản quản trị viên chính đã tồn tại, hãy dùng mật khẩu hiện có hoặc đặt lại từ màn đăng nhập quản trị.";

            ph_enable_level2.Visible = !hasAdvanced && !pendingRequest;
            ph_level2_pending.Visible = !hasAdvanced && pendingRequest;
            ph_level2_ready.Visible = hasAdvanced;
            ph_level2_ready_detail.Visible = hasAdvanced;
            ph_request_reject.Visible = !hasAdvanced && rejectedRequest;

            if (pendingRequest)
            {
                lb_request_pending_time.Text = latestRequest.NgayTao.HasValue
                    ? latestRequest.NgayTao.Value.ToString("dd/MM/yyyy HH:mm")
                    : "--";
            }
            else
            {
                lb_request_pending_time.Text = "--";
            }

            if (rejectedRequest)
            {
                string note = string.IsNullOrWhiteSpace(latestRequest.GhiChuAdmin)
                    ? "Admin chưa ghi chú."
                    : latestRequest.GhiChuAdmin.Trim();
                lb_request_reject.Text = HttpUtility.HtmlEncode(note);
            }
            else
            {
                lb_request_reject.Text = "";
            }

            if (hasAdvanced)
            {
                string ownerTk = ownerAdmin == null ? ShopLevel_cl.ResolveOwnerAdminUsername(shopAccount) : ownerAdmin.taikhoan;
                lnk_open_advanced.NavigateUrl = "/gianhang/admin/login.aspx";
                lnk_add_staff.NavigateUrl = "/gianhang/admin/quan-ly-tai-khoan/add.aspx";
                lnk_owner_permission.NavigateUrl = "/gianhang/admin/quan-ly-tai-khoan/phan-quyen.aspx?user=" + HttpUtility.UrlEncode(ownerTk);
                BindLevel2Staff(db, shopAccount, ownerTk);
            }
        }

        string notice = (Session[NoticeSessionKey] ?? "").ToString();
        ph_notice.Visible = !string.IsNullOrWhiteSpace(notice);
        lb_notice.Text = HttpUtility.HtmlEncode(notice);
        Session[NoticeSessionKey] = "";
    }

    private string CurrentAccountKey()
    {
        RootAccount_cl.RootAccountInfo info = GianHangBootstrap_cl.GetCurrentInfo();
        return info == null ? string.Empty : (info.AccountKey ?? string.Empty).Trim().ToLowerInvariant();
    }

    private void BindLevel2Staff(dbDataContext db, string shopAccount, string ownerTk)
    {
        List<taikhoan_table_2023> staffRows = db.taikhoan_table_2023s
            .Where(p => p.user_parent == shopAccount)
            .OrderByDescending(p => p.taikhoan == ownerTk)
            .ThenBy(p => p.hoten)
            .ToList();

        List<string> branchIds = staffRows
            .Select(p => (p.id_chinhanh ?? "").Trim())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct()
            .ToList();

        Dictionary<string, string> branchLookup = branchIds.Count == 0
            ? new Dictionary<string, string>()
            : db.chinhanh_tables
                .Where(p => branchIds.Contains(p.id.ToString()))
                .ToList()
                .ToDictionary(p => p.id.ToString(), p => string.IsNullOrWhiteSpace(p.ten) ? "Chi nhánh" : p.ten);

        List<Level2StaffItem> items = staffRows
            .Select(p => new Level2StaffItem
            {
                TaiKhoan = p.taikhoan,
                HoTen = string.IsNullOrWhiteSpace(p.hoten) ? p.taikhoan : p.hoten.Trim(),
                ChiNhanh = ResolveBranchName(branchLookup, (p.id_chinhanh ?? "").Trim()),
                TrangThai = string.IsNullOrWhiteSpace(p.trangthai) ? "Đang hoạt động" : p.trangthai,
                PermissionSummary = ResolvePermissionSummary(p.permission),
                IsOwner = string.Equals(p.taikhoan ?? "", ownerTk ?? "", StringComparison.OrdinalIgnoreCase)
            })
            .ToList();

        lb_level2_staff_total.Text = items.Count.ToString("#,##0");
        ph_level2_staff_empty.Visible = items.Count == 0;
        rp_level2_staff.DataSource = items;
        rp_level2_staff.DataBind();
    }

    private string ResolvePermissionSummary(string permission)
    {
        string raw = (permission ?? "").Trim();
        if (string.IsNullOrWhiteSpace(raw))
            return "Chưa cấp quyền module";
        if (string.Equals(raw, "all", StringComparison.OrdinalIgnoreCase))
            return "Toàn quyền";

        string[] tokens = PortalScope_cl.SplitPermissionTokens(raw)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (tokens.Length == 0)
            return "Chưa cấp quyền module";

        return tokens.Length.ToString("#,##0") + " mã quyền";
    }

    private static string ResolveBranchName(Dictionary<string, string> branchLookup, string branchId)
    {
        if (branchLookup == null || string.IsNullOrWhiteSpace(branchId))
            return "Chi nhánh mặc định";

        string name;
        return branchLookup.TryGetValue(branchId, out name) ? name : "Chi nhánh mặc định";
    }
}
