using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_uc_menu_left_uc : System.Web.UI.UserControl
{
    public string loi, tuvan;

    private bool GetFlag(string key)
    {
        object v = ViewState[key];
        if (v == null) return false;
        bool b;
        return bool.TryParse(v.ToString(), out b) && b;
    }

    private void SetFlag(string key, bool value)
    {
        ViewState[key] = value ? "true" : "false";
    }

    private void BuildLeftMenuPermissionFlags(taikhoan_tb account)
    {
        string tk = account != null ? (account.taikhoan ?? "").Trim() : "";
        bool isRoot = account != null && AdminRolePolicy_cl.IsSuperAdmin(tk);
        SetFlag("left_is_root", isRoot);

        bool showAdminDashboard = account != null;
        bool showAdminAccount = false;
        bool showTransferHistory = false;
        bool showHomeAccount = false;
        bool showApproveHanhVi = false;
        bool showIssueCard = false;
        bool showTierDescription = false;
        bool showSellProduct = false;
        bool showShopAccount = false;
        bool showShopApprove = false;
        bool showShopEmailTemplate = false;
        bool showShopPointApproval = false;
        bool showHomeSettings = false;
        bool showHomeTextContent = false;
        bool showContentMenu = false;
        bool showContentBaiViet = false;
        bool showContentBanner = false;
        bool showContentGopY = false;
        bool showContentThongBao = false;
        bool showContentTuVan = false;

        using (dbDataContext db = new dbDataContext())
        {
            showAdminAccount = AdminRolePolicy_cl.CanManageAdminAccounts(db, tk);
            showTransferHistory = AdminRolePolicy_cl.CanAccessTransferHistory(db, tk);
            showHomeAccount = AdminRolePolicy_cl.CanManageHomeAccounts(db, tk);
            showApproveHanhVi = AdminRolePolicy_cl.CanReviewHomePointRequests(db, tk);
            showIssueCard = AdminRolePolicy_cl.CanIssueCards(db, tk);
            showTierDescription = AdminRolePolicy_cl.CanViewTierReference(db, tk);
            showSellProduct = AdminRolePolicy_cl.CanSellSystemProducts(db, tk);
            showShopAccount = AdminRolePolicy_cl.CanManageShopAccounts(db, tk);
            showShopApprove = AdminRolePolicy_cl.CanApproveShopPartnerRegistration(db, tk);
            showShopEmailTemplate = AdminRolePolicy_cl.CanManageShopOperations(db, tk);
            showShopPointApproval = AdminRolePolicy_cl.CanReviewShopPointRequests(db, tk) || AdminRolePolicy_cl.CanManageShopAccounts(db, tk);

            bool canManageHomeContent = AdminRolePolicy_cl.CanManageHomeContent(db, tk);
            showHomeSettings = false;
            showHomeTextContent = canManageHomeContent;
            showContentMenu = isRoot;
            showContentBaiViet = isRoot;
            showContentBanner = isRoot;
            showContentGopY = isRoot;
            showContentThongBao = isRoot;
            showContentTuVan = isRoot;
        }

        SetFlag("left_admin_dashboard", showAdminDashboard);
        SetFlag("left_admin_account", showAdminAccount);
        SetFlag("left_transfer_history", showTransferHistory);
        SetFlag("left_home_account", showHomeAccount);
        SetFlag("left_home_approve_hanhvi", showApproveHanhVi);
        SetFlag("left_home_issue_card", showIssueCard);
        SetFlag("left_home_tier_desc", showTierDescription);
        SetFlag("left_home_sell_product", showSellProduct);
        SetFlag("left_shop_account", showShopAccount);
        SetFlag("left_shop_approve", showShopApprove);
        SetFlag("left_shop_email_template", showShopEmailTemplate);
        SetFlag("left_shop_point_approval", showShopPointApproval);
        SetFlag("left_content_home", showHomeSettings);
        SetFlag("left_content_home_text", showHomeTextContent);
        SetFlag("left_content_menu", showContentMenu);
        SetFlag("left_content_baiviet", showContentBaiViet);
        SetFlag("left_content_banner", showContentBanner);
        SetFlag("left_content_gopy", showContentGopY);
        SetFlag("left_content_thongbao", showContentThongBao);
        SetFlag("left_content_tuvan", showContentTuVan);

        SetFlag("left_group_admin", showAdminDashboard || showAdminAccount || showTransferHistory);
        SetFlag("left_group_home", showHomeAccount || showApproveHanhVi || showIssueCard || showTierDescription || showSellProduct);
        SetFlag("left_group_shop", showShopAccount || showShopApprove || showShopEmailTemplate || showShopPointApproval);
        SetFlag("left_group_content", showHomeSettings || showHomeTextContent || showContentMenu || showContentBaiViet || showContentBanner || showContentGopY || showContentThongBao || showContentTuVan);
    }

    private bool IsRootAdminCurrent()
    {
        string tkEnc = Session["taikhoan"] as string;
        if (string.IsNullOrEmpty(tkEnc))
            return false;

        string tk = "";
        try { tk = mahoa_cl.giaima_Bcorn(tkEnc); }
        catch { tk = tkEnc; }
        return PermissionProfile_cl.IsRootAdmin(tk);
    }

    private bool CanManageHomeContentCurrent()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                return AdminRolePolicy_cl.CanManageHomeContent(db, AdminRolePolicy_cl.GetCurrentAdminUser());
            }
        }
        catch
        {
            return false;
        }
    }

    public bool ShowHomeLandingSettingsTab()
    {
        return GetFlag("left_content_home");
    }

    public bool ShowAdminDashboardTab()
    {
        return GetFlag("left_admin_dashboard");
    }

    public bool ShowHomeLandingContentTab()
    {
        return GetFlag("left_content_home_text");
    }

    public bool ShowLeftGroupAdmin() { return GetFlag("left_group_admin"); }
    public bool ShowLeftGroupHome() { return GetFlag("left_group_home"); }
    public bool ShowLeftGroupShop() { return GetFlag("left_group_shop"); }
    public bool ShowLeftGroupContent() { return GetFlag("left_group_content"); }

    public bool ShowLeftAdminAccount() { return GetFlag("left_admin_account"); }
    public bool ShowLeftTransferHistory() { return GetFlag("left_transfer_history"); }
    public bool ShowLeftHomeAccount() { return GetFlag("left_home_account"); }
    public bool ShowLeftApproveHanhVi() { return GetFlag("left_home_approve_hanhvi"); }
    public bool ShowLeftIssueCard() { return GetFlag("left_home_issue_card"); }
    public bool ShowLeftTierDescription() { return GetFlag("left_home_tier_desc"); }
    public bool ShowLeftSellProduct() { return GetFlag("left_home_sell_product"); }
    public bool ShowLeftShopAccount() { return GetFlag("left_shop_account"); }
    public bool ShowLeftShopApprove() { return GetFlag("left_shop_approve"); }
    public bool ShowLeftShopEmailTemplate() { return GetFlag("left_shop_email_template"); }
    public bool ShowLeftShopPointApproval() { return GetFlag("left_shop_point_approval"); }
    public bool ShowLeftContentMenu() { return GetFlag("left_content_menu"); }
    public bool ShowLeftContentBaiViet() { return GetFlag("left_content_baiviet"); }
    public bool ShowLeftContentBanner() { return GetFlag("left_content_banner"); }
    public bool ShowLeftContentGopY() { return GetFlag("left_content_gopy"); }
    public bool ShowLeftContentThongBao() { return GetFlag("left_content_thongbao"); }
    public bool ShowLeftContentTuVan() { return GetFlag("left_content_tuvan"); }

    private string GetCurrentAdminAccount()
    {
        string tkEnc = Session["taikhoan"] as string;
        if (string.IsNullOrEmpty(tkEnc))
            return "";

        try { return mahoa_cl.giaima_Bcorn(tkEnc); }
        catch { return tkEnc; }
    }

    public string GetApproveHomePointUrl()
    {
        string taiKhoan = GetCurrentAdminAccount();
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            using (dbDataContext db = new dbDataContext())
            {
                return AdminRolePolicy_cl.ResolveHomePointApprovalUrl(db, taiKhoan);
            }
        }

        return "/admin/duyet-yeu-cau-len-cap.aspx";
    }

    public string GetAdminAccountManagementUrl()
    {
        string taiKhoan = GetCurrentAdminAccount();
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            using (dbDataContext db = new dbDataContext())
            {
                return AdminRolePolicy_cl.ResolveAdminAccountManagementUrl(db, taiKhoan);
            }
        }

        return "/admin/quan-ly-tai-khoan/Default.aspx?scope=admin&fscope=admin";
    }

    public string GetHomeAccountManagementUrl()
    {
        string taiKhoan = GetCurrentAdminAccount();
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            using (dbDataContext db = new dbDataContext())
            {
                return AdminRolePolicy_cl.ResolveHomeAccountManagementUrl(db, taiKhoan);
            }
        }

        return "/admin/quan-ly-tai-khoan/Default.aspx?scope=home&fscope=home";
    }

    public string GetShopAccountManagementUrl()
    {
        string taiKhoan = GetCurrentAdminAccount();
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            using (dbDataContext db = new dbDataContext())
            {
                return AdminRolePolicy_cl.ResolveShopAccountManagementUrl(db, taiKhoan);
            }
        }

        return "/admin/quan-ly-tai-khoan/Default.aspx?scope=shop&fscope=shop&frole=shop_partner";
    }

    public string MenuActive(params string[] urls)
    {
        string currentUrl = HttpContext.Current.Request.Url.AbsolutePath.ToLower().Trim();
        foreach (string url in urls)
        {
            if (currentUrl == (url ?? "").ToLower().Trim())
                return "active";
        }
        return "";
    }

    public string MenuActiveTransferHistory()
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        if (currentUrl != "/admin/lich-su-chuyen-diem/default.aspx"
            && currentUrl != "/admin/lich-su-chuyen-diem/chuyen-diem.aspx")
            return "";

        string tab = (Request.QueryString["tab"] ?? "").Trim().ToLowerInvariant();
        if (tab == "uu-dai" || tab == "lao-dong" || tab == "gan-ket")
            return "";

        return "active";
    }

    public string MenuActivePointApproval()
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        if (currentUrl == "/admin/duyet-yeu-cau-len-cap.aspx")
            return "active";
        if (currentUrl != "/admin/lich-su-chuyen-diem/default.aspx"
            && currentUrl != "/admin/lich-su-chuyen-diem/chuyen-diem.aspx")
            return "";

        string tab = (Request.QueryString["tab"] ?? "").Trim().ToLowerInvariant();
        if (tab == "uu-dai" || tab == "lao-dong" || tab == "gan-ket")
            return "active";

        return "";
    }

    public string MenuActiveShopPointApproval()
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        if (currentUrl != "/admin/lich-su-chuyen-diem/default.aspx"
            && currentUrl != "/admin/lich-su-chuyen-diem/chuyen-diem.aspx")
            return "";

        string tab = (Request.QueryString["tab"] ?? "").Trim().ToLowerInvariant();
        return tab == "shop-only" ? "active" : "";
    }

    public string MenuActiveTaiKhoanScope(string scope)
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        if (currentUrl != "/admin/quan-ly-tai-khoan/default.aspx"
            && currentUrl != "/admin/quan-ly-tai-khoan/them-moi.aspx"
            && currentUrl != "/admin/quan-ly-tai-khoan/bo-loc.aspx"
            && currentUrl != "/admin/quan-ly-tai-khoan/chinh-sua.aspx"
            && currentUrl != "/admin/quan-ly-tai-khoan/phan-quyen.aspx")
            return "";

        string currentScope = (Request.QueryString["scope"] ?? "").Trim().ToLowerInvariant();
        string targetScope = (scope ?? "").Trim().ToLowerInvariant();
        if (currentScope == targetScope)
            return "active";
        return "";
    }

    private static string ResolveTitle(string url)
    {
        string normalized = (url ?? "").ToLower().Trim();
        switch (normalized)
        {
            case "/admin/quan-ly-menu/default.aspx":
            case "/admin/quan-ly-menu/them-moi.aspx":
            case "/admin/quan-ly-menu/bo-loc.aspx":
            case "/admin/quan-ly-menu/chinh-sua.aspx":
            case "/admin/quan-ly-menu/xuat-du-lieu.aspx":
            case "/admin/quan-ly-menu/ban-in.aspx":
                return "Quản lý menu";
            case "/admin/quan-ly-bai-viet/default.aspx":
            case "/admin/quan-ly-bai-viet/in.aspx":
            case "/admin/quan-ly-bai-viet/them-moi.aspx":
            case "/admin/quan-ly-bai-viet/bo-loc.aspx":
            case "/admin/quan-ly-bai-viet/chinh-sua.aspx":
            case "/admin/quan-ly-bai-viet/xuat-du-lieu.aspx":
            case "/admin/quan-ly-bai-viet/ban-in.aspx":
                return "Quản lý bài viết";
            case "/admin/quan-ly-banner/default.aspx":
            case "/admin/quan-ly-banner/them-moi.aspx":
                return "Quản lý banner";
            case "/admin/quan-ly-gop-y/default.aspx":
                return "Quản lý góp ý";
            case "/admin/quan-ly-thong-bao/default.aspx":
            case "/admin/quan-ly-thong-bao/in.aspx":
            case "/admin/quan-ly-thong-bao/bo-loc.aspx":
            case "/admin/quan-ly-thong-bao/xuat-du-lieu.aspx":
            case "/admin/quan-ly-thong-bao/ban-in.aspx":
                return "Quản lý thông báo";
            case "/admin/yeu-cau-tu-van/default.aspx":
            case "/admin/yeu-cau-tu-van/bo-loc.aspx":
            case "/admin/yeu-cau-tu-van/xuat-du-lieu.aspx":
            case "/admin/yeu-cau-tu-van/ban-in.aspx":
                return "Yêu cầu tư vấn";
            case "/admin/lich-su-chuyen-diem/default.aspx":
            case "/admin/lich-su-chuyen-diem/chuyen-diem.aspx":
                return "Lịch sử chuyển điểm";
            case "/admin/cai-dat-trang-chu/default.aspx":
                return "Cài đặt trang chủ";
            case "/admin/quan-ly-noi-dung-home/default.aspx":
                return "Nội dung trang chủ Home";
            case "/admin/quan-ly-tai-khoan/Default.aspx":
            case "/admin/quan-ly-tai-khoan/them-moi.aspx":
            case "/admin/quan-ly-tai-khoan/bo-loc.aspx":
            case "/admin/quan-ly-tai-khoan/chinh-sua.aspx":
            case "/admin/quan-ly-tai-khoan/phan-quyen.aspx":
                return "Quản lý tài khoản";
            case "/admin/duyet-yeu-cau-len-cap.aspx":
                return "Duyệt yêu cầu xác nhận hành vi";
            case "/admin/duyet-gian-hang-doi-tac.aspx":
                return "Duyệt gian hàng đối tác";
            case "/admin/phat-hanh-the.aspx":
            case "/admin/phat-hanh-the/them-moi.aspx":
                return "Phát hành thẻ";
            case "/admin/doi-mat-khau/default.aspx":
                return "Đổi mật khẩu";
            case "/admin/quen-mat-khau/default.aspx":
            case "/admin/khoi-phuc-mat-khau.aspx":
                return "Quên mật khẩu";
            case "/admin/vi-token-diem/default.aspx":
                return "Ví token điểm";
            case "/admin/motacapbac.aspx":
                return "Mô tả cấp bậc";
            case "/admin/he-thong-san-pham/ban-san-pham.aspx":
            case "/admin/he-thong-san-pham/ban-the.aspx":
            case "/admin/he-thong-san-pham/chi-tiet-giao-dich.aspx":
                return "Bán sản phẩm";
            default:
                return "Trang chủ admin";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string _url = HttpContext.Current.Request.Url.AbsolutePath.ToLower().Trim();
                string title = ResolveTitle(_url);
                if (_url == "/admin/quan-ly-tai-khoan/Default.aspx")
                {
                    string scope = (Request.QueryString["scope"] ?? "").Trim().ToLowerInvariant();
                    if (scope == "admin") title = "Quản lý tài khoản admin";
                    else if (scope == "home") title = "Quản lý tài khoản home";
                    else if (scope == "shop") title = "Quản lý tài khoản gian hàng đối tác";
                }
                else if (_url == "/admin/lich-su-chuyen-diem/default.aspx")
                {
                    string tab = (Request.QueryString["tab"] ?? "").Trim().ToLowerInvariant();
                    if (tab == "uu-dai") title = "Duyệt điểm hồ sơ Khách hàng";
                    else if (tab == "lao-dong") title = "Duyệt điểm hồ sơ Cộng tác phát triển";
                    else if (tab == "gan-ket") title = "Duyệt điểm hồ sơ Đồng hành hệ sinh thái";
                    else if (tab == "shop-only") title = "Duyệt điểm / nghiệp vụ shop";
                }
                Session["title"] = title;

                using (dbDataContext db = new dbDataContext())
                {
                    string tkEnc = Session["taikhoan"] as string;
                    string tk = "";
                    if (!string.IsNullOrEmpty(tkEnc))
                    {
                        try { tk = mahoa_cl.giaima_Bcorn(tkEnc); }
                        catch { tk = tkEnc; }
                    }
                    taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == tk);
                    BuildLeftMenuPermissionFlags(account);
                    if (account != null)
                    {
                        ViewState["admin_role_label"] = AdminRolePolicy_cl.GetAdminRoleLabel(db, account.taikhoan, account.phanloai, account.permission);
                        ViewState["admin_scope_label"] = AdminRolePolicy_cl.GetScopeDisplayLabel(PortalScope_cl.ResolveScope(account.taikhoan, account.phanloai, account.permission));
                    }
                    else
                    {
                        ViewState["admin_role_label"] = "Trang quản trị";
                        ViewState["admin_scope_label"] = "Cổng Admin";
                    }

                    #region ĐẾM LỖI HỆ THỐNG CHƯA XỬ LÝ
                    int q_loi = db.Log_tbs.Count(p => p.trangthai == "Chưa sửa" && p.bin == false);
                    if (q_loi < 100)
                        loi = q_loi.ToString();
                    else
                        loi = "99+";
                    #endregion
                }
            }
            catch (Exception _ex)
            {
                string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
                if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                    _tk = mahoa_cl.giaima_Bcorn(_tk);
                else
                    _tk = "";
                Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            }
        }
    }
}
