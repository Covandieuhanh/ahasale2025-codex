using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_uc_menu_top_uc : System.Web.UI.UserControl
{
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

    private void BuildMenuPermissionFlags(taikhoan_tb account)
    {
        string tk = account != null ? (account.taikhoan ?? "").Trim() : "";
        bool isRoot = account != null && AdminRolePolicy_cl.IsSuperAdmin(tk);
        SetFlag("menu_is_root", isRoot);

        bool showTransferHistory = false;
        bool showAdminDashboard = false;
        bool showAdminAccount = false;
        bool showAdminOtp = false;
        bool showAdminTokenWallet = false;
        bool showAdminGopY = false;
        bool showAdminThongBao = false;
        bool showAdminTuVan = false;
        bool showAdminCompanyShopSync = false;
        bool showAdminReindexBaiViet = false;
        bool showHomeAccount = false;
        bool showApproveHanhVi = false;
        bool showIssueCard = false;
        bool showTierDescription = false;
        bool showSellProduct = false;
        bool showShopWorkspace = false;
        bool showShopAccount = false;
        bool showShopApprove = false;
        bool showShopLevel2 = false;
        bool showShopEmailTemplate = false;
        bool showShopPointApproval = false;
        bool showHomeContent = false;
        bool showHomeTextContent = false;
        bool showContentMenu = false;
        bool showContentBaiViet = false;
        bool showContentBanner = false;
        bool showDauGiaWorkspace = false;
        bool showEventWorkspace = false;
        bool showLegacySystemProductMenu = !CompanyShop_cl.HideLegacyAdminSystemProduct();

        if (account != null && !string.IsNullOrWhiteSpace(tk))
        {
            using (dbDataContext db = new dbDataContext())
            {
                string phanloai = account != null ? (account.phanloai ?? "") : "";
                string permissionRaw = account != null ? (account.permission ?? "") : "";
                var tabKeys = new HashSet<string>(
                    AdminRolePolicy_cl.BuildAdminHomeTabs(db, tk, phanloai, permissionRaw)
                        .Select(item => (item.Key ?? "").Trim()),
                    StringComparer.OrdinalIgnoreCase);

                Func<string, bool> hasKey = key => tabKeys.Contains((key ?? "").Trim());

                showAdminDashboard = hasKey("admin_dashboard");
                showAdminAccount = hasKey("admin_accounts");
                showAdminOtp = hasKey("admin_otp");
                showAdminTokenWallet = hasKey("token_wallet");
                showAdminGopY = hasKey("admin_feedback");
                showAdminThongBao = hasKey("notifications");
                showAdminTuVan = hasKey("consulting");
                showAdminCompanyShopSync = hasKey("admin_company_shop_sync");
                showAdminReindexBaiViet = hasKey("admin_reindex_baiviet");
                showTransferHistory = hasKey("core_assets");
                showHomeAccount = hasKey("home_accounts");
                showApproveHanhVi = hasKey("home_point_approval")
                    || hasKey("home_points_customer")
                    || hasKey("home_points_development")
                    || hasKey("home_points_ecosystem");
                showIssueCard = showLegacySystemProductMenu && hasKey("issue_cards");
                showTierDescription = hasKey("tier_reference");
                showSellProduct = showLegacySystemProductMenu && hasKey("system_products");
                showShopWorkspace = hasKey("shop_workspace");
                showShopAccount = hasKey("shop_accounts");
                showShopApprove = hasKey("home_gianhang_space") || hasKey("shop_partner");
                showShopLevel2 = hasKey("shop_level2");
                showShopEmailTemplate = hasKey("shop_email");
                showShopPointApproval = hasKey("shop_points");
                showHomeContent = hasKey("home_config");
                showHomeTextContent = hasKey("home_content");
                showContentMenu = hasKey("home_menu");
                showContentBaiViet = hasKey("home_posts");
                showContentBanner = hasKey("home_banner");
                showDauGiaWorkspace = hasKey("daugia_workspace");
                showEventWorkspace = hasKey("event_workspace");
            }
        }

        SetFlag("menu_admin_dashboard", showAdminDashboard);
        SetFlag("menu_admin_account", showAdminAccount);
        SetFlag("menu_admin_otp", showAdminOtp);
        SetFlag("menu_admin_token_wallet", showAdminTokenWallet);
        SetFlag("menu_admin_gopy", showAdminGopY);
        SetFlag("menu_admin_thongbao", showAdminThongBao);
        SetFlag("menu_admin_tuvan", showAdminTuVan);
        SetFlag("menu_admin_company_shop_sync", showAdminCompanyShopSync);
        SetFlag("menu_admin_reindex_baiviet", showAdminReindexBaiViet);
        SetFlag("menu_transfer_history", showTransferHistory);
        SetFlag("menu_home_account", showHomeAccount);
        SetFlag("menu_home_approve_hanhvi", showApproveHanhVi);
        SetFlag("menu_home_issue_card", showIssueCard);
        SetFlag("menu_home_tier_desc", showTierDescription);
        SetFlag("menu_home_sell_product", showSellProduct);
        SetFlag("menu_shop_workspace", showShopWorkspace);
        SetFlag("menu_shop_account", showShopAccount);
        SetFlag("menu_shop_approve", showShopApprove);
        SetFlag("menu_shop_level2", showShopLevel2);
        SetFlag("menu_shop_email_template", showShopEmailTemplate);
        SetFlag("menu_shop_point_approval", showShopPointApproval);
        SetFlag("menu_content_home", showHomeContent);
        SetFlag("menu_content_home_text", showHomeTextContent);
        SetFlag("menu_content_menu", showContentMenu);
        SetFlag("menu_content_baiviet", showContentBaiViet);
        SetFlag("menu_content_banner", showContentBanner);
        SetFlag("menu_daugia_workspace", showDauGiaWorkspace);
        SetFlag("menu_event_workspace", showEventWorkspace);
    }

    private string GetCurrentManagementSpace()
    {
        object value = ViewState["admin_management_space"];
        string space = value == null ? "" : value.ToString();
        space = AdminManagementSpace_cl.NormalizeSpace(space);
        return string.IsNullOrWhiteSpace(space) ? AdminManagementSpace_cl.SpaceAdmin : space;
    }

    public bool IsManagementSpaceAdmin() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceAdmin; }
    public bool IsManagementSpaceHome() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceHome; }
    public bool IsManagementSpaceGianHang() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceGianHang; }
    public bool IsManagementSpaceDauGia() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceDauGia; }
    public bool IsManagementSpaceEvent() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceEvent; }
    public bool IsManagementSpaceContent() { return GetCurrentManagementSpace() == AdminManagementSpace_cl.SpaceContent; }

    public bool ShowMenuAdminDashboard() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_dashboard"); }
    public bool ShowMenuAdminAccount() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_account"); }
    public bool ShowMenuAdminOtp() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_otp"); }
    public bool ShowMenuAdminTokenWallet() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_token_wallet"); }
    public bool ShowMenuAdminGopY() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_gopy"); }
    public bool ShowMenuAdminThongBao() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_thongbao"); }
    public bool ShowMenuAdminTuVan() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_tuvan"); }
    public bool ShowMenuAdminCompanyShopSync() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_company_shop_sync"); }
    public bool ShowMenuAdminReindexBaiViet() { return IsManagementSpaceAdmin() && GetFlag("menu_admin_reindex_baiviet"); }
    public bool ShowMenuTransferHistory() { return IsManagementSpaceHome() && GetFlag("menu_transfer_history"); }
    public bool ShowMenuHomeAccount() { return IsManagementSpaceHome() && GetFlag("menu_home_account"); }
    public bool ShowMenuApproveHanhVi() { return IsManagementSpaceHome() && GetFlag("menu_home_approve_hanhvi"); }
    public bool ShowMenuIssueCard() { return IsManagementSpaceHome() && GetFlag("menu_home_issue_card"); }
    public bool ShowMenuTierDescription() { return IsManagementSpaceHome() && GetFlag("menu_home_tier_desc"); }
    public bool ShowMenuSellProduct() { return IsManagementSpaceHome() && GetFlag("menu_home_sell_product"); }
    public bool ShowMenuShopWorkspace() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_workspace"); }
    public bool ShowMenuShopAccount() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_account"); }
    public bool ShowMenuShopApprove() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_approve"); }
    public bool ShowMenuShopLevel2() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_level2"); }
    public bool ShowMenuShopEmailTemplate() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_email_template"); }
    public bool ShowMenuShopPointApproval() { return IsManagementSpaceGianHang() && GetFlag("menu_shop_point_approval"); }
    public bool ShowMenuHomeContent() { return IsManagementSpaceContent() && GetFlag("menu_content_home"); }
    public bool ShowMenuHomeTextContent() { return IsManagementSpaceContent() && GetFlag("menu_content_home_text"); }
    public bool ShowMenuContentMenu() { return IsManagementSpaceContent() && GetFlag("menu_content_menu"); }
    public bool ShowMenuContentBaiViet() { return IsManagementSpaceContent() && GetFlag("menu_content_baiviet"); }
    public bool ShowMenuContentBanner() { return IsManagementSpaceContent() && GetFlag("menu_content_banner"); }
    public bool ShowMenuDauGiaWorkspace() { return IsManagementSpaceDauGia() && GetFlag("menu_daugia_workspace"); }
    public bool ShowMenuEventWorkspace() { return IsManagementSpaceEvent() && GetFlag("menu_event_workspace"); }

    public bool ShowMenuGroupAdmin()
    {
        return ShowMenuAdminDashboard() || ShowMenuAdminAccount() || ShowMenuAdminOtp() || ShowMenuAdminTokenWallet()
            || ShowMenuAdminGopY() || ShowMenuAdminThongBao() || ShowMenuAdminTuVan()
            || ShowMenuAdminCompanyShopSync() || ShowMenuAdminReindexBaiViet();
    }

    public bool ShowMenuGroupHomeSpace()
    {
        return ShowMenuTransferHistory() || ShowMenuHomeAccount() || ShowMenuApproveHanhVi()
            || ShowMenuIssueCard() || ShowMenuTierDescription() || ShowMenuSellProduct();
    }

    public bool ShowMenuGroupShop() { return ShowMenuShopWorkspace() || ShowMenuShopAccount() || ShowMenuShopApprove() || ShowMenuShopLevel2() || ShowMenuShopEmailTemplate() || ShowMenuShopPointApproval(); }
    public bool ShowMenuGroupContent() { return ShowMenuHomeContent() || ShowMenuHomeTextContent() || ShowMenuContentMenu() || ShowMenuContentBaiViet() || ShowMenuContentBanner(); }
    public bool ShowMenuGroupDauGia() { return ShowMenuDauGiaWorkspace(); }
    public bool ShowMenuGroupEvent() { return ShowMenuEventWorkspace(); }

    public bool ShowQuickCreateAccounts()
    {
        if (IsManagementSpaceAdmin())
            return ShowMenuAdminAccount();

        if (IsManagementSpaceHome())
            return ShowMenuHomeAccount();

        if (IsManagementSpaceGianHang())
            return ShowMenuShopAccount();

        return false;
    }

    public bool ShowQuickCreateOperations()
    {
        return IsManagementSpaceHome() && (ShowMenuIssueCard() || ShowMenuSellProduct());
    }

    public bool ShowQuickCreateContent()
    {
        return IsManagementSpaceContent() && (ShowMenuContentMenu() || ShowMenuContentBaiViet() || ShowMenuContentBanner());
    }

    public bool ShowQuickCreateMenu()
    {
        return ShowQuickCreateAccounts() || ShowQuickCreateOperations() || ShowQuickCreateContent();
    }

    public string GetAdminHomeUrl()
    {
        string taiKhoan = GetCurrentAdminAccount();
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            using (dbDataContext db = new dbDataContext())
            {
                taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == taiKhoan);
                if (account != null)
                    return AdminManagementSpace_cl.ResolveCurrentLandingUrl(db, account, Request);
            }
        }

        return "/admin/login.aspx";
    }

    private string GetScopedUrl(string url, string space)
    {
        return AdminManagementSpace_cl.AppendSpaceToUrl(url, space);
    }

    public string GetApproveHomePointUrl()
    {
        string taiKhoan = GetCurrentAdminAccount();
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            using (dbDataContext db = new dbDataContext())
            {
                return GetScopedUrl(AdminRolePolicy_cl.ResolveHomePointApprovalUrl(db, taiKhoan), AdminManagementSpace_cl.SpaceHome);
            }
        }

        return GetScopedUrl("/admin/duyet-yeu-cau-len-cap.aspx", AdminManagementSpace_cl.SpaceHome);
    }

    public string GetAdminAccountManagementUrl()
    {
        string taiKhoan = GetCurrentAdminAccount();
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            using (dbDataContext db = new dbDataContext())
            {
                return GetScopedUrl(AdminRolePolicy_cl.ResolveAdminAccountManagementUrl(db, taiKhoan), AdminManagementSpace_cl.SpaceAdmin);
            }
        }

        return GetScopedUrl("/admin/quan-ly-tai-khoan/Default.aspx?scope=admin&fscope=admin", AdminManagementSpace_cl.SpaceAdmin);
    }

    public string GetAdminDashboardUrl()
    {
        return GetScopedUrl("/admin/default.aspx", AdminManagementSpace_cl.SpaceAdmin);
    }

    public string GetAdminOtpUrl()
    {
        return GetScopedUrl("/admin/otp/default.aspx?scope=home", AdminManagementSpace_cl.SpaceAdmin);
    }

    public string GetAdminTokenWalletUrl()
    {
        return GetScopedUrl("/admin/vi-token-diem/default.aspx", AdminManagementSpace_cl.SpaceAdmin);
    }

    public string GetAdminGopYUrl()
    {
        return GetScopedUrl("/admin/quan-ly-gop-y/default.aspx", AdminManagementSpace_cl.SpaceAdmin);
    }

    public string GetAdminThongBaoUrl()
    {
        return GetScopedUrl("/admin/quan-ly-thong-bao/default.aspx", AdminManagementSpace_cl.SpaceAdmin);
    }

    public string GetAdminTuVanUrl()
    {
        return GetScopedUrl("/admin/yeu-cau-tu-van/default.aspx", AdminManagementSpace_cl.SpaceAdmin);
    }

    public string GetAdminCompanyShopSyncUrl()
    {
        return GetScopedUrl("/admin/tools/company-shop-sync.aspx", AdminManagementSpace_cl.SpaceAdmin);
    }

    public string GetAdminReindexBaiVietUrl()
    {
        return GetScopedUrl("/admin/tools/reindex-baiviet.aspx", AdminManagementSpace_cl.SpaceAdmin);
    }

    public string GetAdminAccountCreateUrl()
    {
        return GetScopedUrl("/admin/quan-ly-tai-khoan/them-moi.aspx?scope=admin&fscope=admin", AdminManagementSpace_cl.SpaceAdmin);
    }

    public string GetHomeAccountManagementUrl()
    {
        string taiKhoan = GetCurrentAdminAccount();
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            using (dbDataContext db = new dbDataContext())
            {
                return GetScopedUrl(AdminRolePolicy_cl.ResolveHomeAccountManagementUrl(db, taiKhoan), AdminManagementSpace_cl.SpaceHome);
            }
        }

        return GetScopedUrl("/admin/quan-ly-tai-khoan/Default.aspx?scope=home&fscope=home", AdminManagementSpace_cl.SpaceHome);
    }

    public string GetShopWorkspaceUrl()
    {
        return GetScopedUrl("/gianhang/admin/default.aspx?system=1", AdminManagementSpace_cl.SpaceGianHang);
    }

    public string GetShopAccountManagementUrl()
    {
        string taiKhoan = GetCurrentAdminAccount();
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            using (dbDataContext db = new dbDataContext())
            {
                return GetScopedUrl(AdminRolePolicy_cl.ResolveShopAccountManagementUrl(db, taiKhoan), AdminManagementSpace_cl.SpaceGianHang);
            }
        }

        return GetScopedUrl("/admin/quan-ly-tai-khoan/Default.aspx?scope=shop&fscope=shop&frole=shop_partner", AdminManagementSpace_cl.SpaceGianHang);
    }

    public string GetDauGiaAdminUrl()
    {
        return GetScopedUrl("/daugia/admin/default.aspx?system=1", AdminManagementSpace_cl.SpaceDauGia);
    }

    public string GetEventAdminUrl()
    {
        return GetScopedUrl("/event/admin/default.aspx?system=1", AdminManagementSpace_cl.SpaceEvent);
    }

    public string GetHomeAccountCreateUrl()
    {
        return GetScopedUrl("/admin/quan-ly-tai-khoan/them-moi.aspx?scope=home&fscope=home", AdminManagementSpace_cl.SpaceHome);
    }

    public string GetShopAccountCreateUrl()
    {
        return GetScopedUrl("/admin/quan-ly-tai-khoan/them-moi.aspx?scope=shop&fscope=shop&frole=shop_partner", AdminManagementSpace_cl.SpaceGianHang);
    }

    public string GetHomeTransferHistoryUrl() { return GetScopedUrl("/admin/lich-su-chuyen-diem/default.aspx", AdminManagementSpace_cl.SpaceHome); }
    public string GetHomeIssueCardUrl() { return GetScopedUrl("/admin/phat-hanh-the/them-moi.aspx", AdminManagementSpace_cl.SpaceHome); }
    public string GetHomeTierDescriptionUrl() { return GetScopedUrl("/admin/MoTaCapBac.aspx", AdminManagementSpace_cl.SpaceHome); }
    public string GetHomeSellProductUrl() { return GetScopedUrl("/admin/he-thong-san-pham/ban-the.aspx", AdminManagementSpace_cl.SpaceHome); }
    public string GetShopPointApprovalUrl() { return GetScopedUrl("/admin/lich-su-chuyen-diem/default.aspx?tab=shop-only", AdminManagementSpace_cl.SpaceGianHang); }
    public string GetShopEmailTemplateUrl() { return GetScopedUrl("/admin/quan-ly-email-shop/default.aspx", AdminManagementSpace_cl.SpaceGianHang); }
    public string GetGianHangApprovalUrl() { return GetScopedUrl("/admin/duyet-gian-hang-doi-tac.aspx", AdminManagementSpace_cl.SpaceGianHang); }
    public string GetShopPartnerApprovalUrl() { return GetScopedUrl("/admin/duyet-shop-doi-tac.aspx", AdminManagementSpace_cl.SpaceGianHang); }
    public string GetShopLevel2ApprovalUrl() { return GetScopedUrl("/admin/duyet-nang-cap-level2.aspx", AdminManagementSpace_cl.SpaceGianHang); }
    public string GetContentSettingsUrl() { return GetScopedUrl("/admin/cai-dat-trang-chu/default.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string GetContentHomeTextUrl() { return GetScopedUrl("/admin/quan-ly-noi-dung-home/default.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string GetContentMenuUrl() { return GetScopedUrl("/admin/quan-ly-menu/default.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string GetContentMenuCreateUrl() { return GetScopedUrl("/admin/quan-ly-menu/them-moi.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string GetContentBaiVietUrl() { return GetScopedUrl("/admin/quan-ly-bai-viet/default.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string GetContentBaiVietCreateUrl() { return GetScopedUrl("/admin/quan-ly-bai-viet/them-moi.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string GetContentBannerUrl() { return GetScopedUrl("/admin/quan-ly-banner/default.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string GetContentBannerCreateUrl() { return GetScopedUrl("/admin/quan-ly-banner/them-moi.aspx", AdminManagementSpace_cl.SpaceContent); }
    public string MenuActive(params string[] urls)
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
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

    public string MenuActiveTokenWallet()
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        if (currentUrl == "/admin/vi-token-diem/default.aspx")
            return "active";
        if (currentUrl != "/admin/default.aspx")
            return "";

        string section = (Request.QueryString["section"] ?? "").Trim().ToLowerInvariant();
        return section == "vi-token-diem" ? "active" : "";
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

    private string GetCurrentAdminAccount()
    {
        string taiKhoanMaHoa = Session["taikhoan"] as string;
        if (string.IsNullOrEmpty(taiKhoanMaHoa))
            return "";

        return mahoa_cl.giaima_Bcorn(taiKhoanMaHoa);
    }

    protected string BuildCurrentPageUrl()
    {
        var query = HttpUtility.ParseQueryString(Request.QueryString.ToString());
        query.Remove("topview");
        query.Remove("return_url");
        string queryString = query.ToString();
        return Request.Url.AbsolutePath + (string.IsNullOrEmpty(queryString) ? "" : "?" + queryString);
    }

    private string BuildChangePasswordUrl()
    {
        string currentUrl = AdminFullPageRoute_cl.SanitizeAdminReturnUrl(BuildCurrentPageUrl(), "/admin/default.aspx");
        return ResolveUrl("~/admin/doi-mat-khau/default.aspx?return_url=" + HttpUtility.UrlEncode(currentUrl));
    }

    public string GetNotificationListUrl()
    {
        return ResolveUrl("~/admin/quan-ly-thong-bao/default.aspx");
    }

    private void RedirectTo(string url)
    {
        ScriptManager sm = ScriptManager.GetCurrent(Page);
        if (sm != null && sm.IsInAsyncPostBack)
        {
            string safeUrl = (url ?? "").Replace("'", "\\'");
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "window.location='" + safeUrl + "';", true);
            return;
        }

        Response.Redirect(url, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void ApplyAccountRouteLinks()
    {
        but_show_form_doimatkhau.NavigateUrl = BuildChangePasswordUrl();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                #region vô hiệu hóa timer trên một số trang có CKEditor
                ApplyMenuTimerSafe();
                #endregion


                if (Session["title"] != null)
                    ViewState["title"] = Session["title"].ToString();

                ViewState["sapxep_thongbao"] = "1";//mặc định sx thông báo theo mới nhất lên đầu
                but_sapxep_moinhat.CssClass = "info small rounded";

                using (dbDataContext db = new dbDataContext())
                {
                    show_soluong_thongbao(db);
                    lay_thongtin_nguoidung(db);
                }

                ApplyAccountRouteLinks();

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

    private void ApplyMenuTimerSafe()
    {
        try
        {
            string url = (Request.Url != null ? Request.Url.AbsolutePath : "").ToLowerInvariant();
            bool disableTimer = url == "/admin/quan-ly-bai-viet/default.aspx"
                                || url == "/admin/motacapbac.aspx"
                                || url == "/admin/MoTaCapBac.aspx".ToLowerInvariant();

            Timer timer = FindControl("Timer1") as Timer;
            if (timer != null)
                timer.Enabled = !disableTimer;
        }
        catch
        {
            // ignore timer failures (Mono doesn't implement Timer.Visible)
        }
    }

    protected void Timer1_Tick(object sender, EventArgs e)
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                show_soluong_thongbao(db);
            }
        }
        catch
        {
            // ignore timer refresh failures
        }
    }

    public void lay_thongtin_nguoidung(dbDataContext db)
    {
        string _tk = Session["taikhoan"] as string;
        if (!string.IsNullOrEmpty(_tk))
        {
            _tk = mahoa_cl.giaima_Bcorn(_tk);
            try
            {
                var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
                if (q == null) return;

                ViewState["hoten"] = q.hoten;
                ViewState["anhdaidien"] = q.anhdaidien;
                ViewState["email"] = q.email;
                ViewState["taikhoan"] = _tk;
                ViewState["admin_role_label"] = AdminRolePolicy_cl.GetAdminRoleLabel(db, q.taikhoan, q.phanloai, q.permission);
                ViewState["admin_role_description"] = AdminRolePolicy_cl.GetAdminRoleDescription(db, q.taikhoan, q.phanloai, q.permission);
                string currentSpace = AdminManagementSpace_cl.GetCurrentSpace(db, q, Request);
                ViewState["admin_management_space"] = currentSpace;
                ViewState["admin_scope_label"] = AdminManagementSpace_cl.GetTitle(currentSpace);

                // Hiển thị nhãn theo hệ đăng nhập admin.
                if (AccountType_cl.IsTreasury(q.phanloai))
                {
                    ViewState["phanloai"] = "Tài khoản tổng";
                }
                else
                {
                    ViewState["phanloai"] = "Nhân viên admin";
                }


                ViewState["DongA"] = (q.DongA ?? 0).ToString("#,##0");
                BuildMenuPermissionFlags(q);
                // =============================
            }
            catch (Exception _ex)
            {
                Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            }
        }
    }


    #region thông báo
    public void show_soluong_thongbao(dbDataContext db)
    {
        try
        {
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
            {
                lb_sl_thongbao.Text = "0";
                return;
            }

            // Đếm số lượng thông báo chưa đọc
            int soLuongThongBaoChuaDoc = db.ThongBao_tbs.Count(p => p.nguoinhan == taiKhoan && p.daxem == false && p.bin == false);

            // Cập nhật nhãn hiển thị số lượng thông báo
            if (soLuongThongBaoChuaDoc < 100)
                lb_sl_thongbao.Text = soLuongThongBaoChuaDoc.ToString();
            else
                lb_sl_thongbao.Text = "99+";
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

    public void show_noidung_thongbao(dbDataContext db)
    {
        try
        {
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
            {
                Repeater1.DataSource = new object[0];
                Repeater1.DataBind();
                ph_empty_thongbao.Visible = true;
                return;
            }

            var list_all = (from ob1 in db.ThongBao_tbs
                            join ob2 in db.taikhoan_tbs on ob1.nguoithongbao equals ob2.taikhoan into senderGroup
                            from ob2 in senderGroup.DefaultIfEmpty()
                            where ob1.nguoinhan == taiKhoan && ob1.bin == false
                            select new
                            {
                                ob1.id, // id thông báo
                                avt_nguoithongbao = (ob2 == null || ob2.anhdaidien == null || ob2.anhdaidien == "")
                                    ? "/uploads/images/macdinh.jpg"
                                    : ob2.anhdaidien,
                                daxem = ob1.daxem,
                                noidung = ob1.noidung ?? "",
                                thoigian = ob1.thoigian,
                                link = (ob1.link == null || ob1.link == "")
                                    ? "/admin/default.aspx?"
                                    : (ob1.link.Contains("?") ? ob1.link + "&" : ob1.link + "?")
                            }).AsQueryable();

            if (Convert.ToString(ViewState["sapxep_thongbao"]) == "2")//lọc ra chưa đọc, mới nhất lên đầu
                list_all = list_all.Where(p => p.daxem == false).OrderByDescending(p => p.thoigian).Take(20);
            else//sx theo mới nhất lên đầu
                list_all = list_all.OrderByDescending(p => p.thoigian).Take(20);

            var result = list_all.ToList();
            // Gán dữ liệu cho Repeater
            Repeater1.DataSource = result;
            Repeater1.DataBind();
            ph_empty_thongbao.Visible = result.Count == 0;
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            Repeater1.DataSource = new object[0];
            Repeater1.DataBind();
            ph_empty_thongbao.Visible = true;
        }
    }
    protected void but_sapxep_moinhat_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["sapxep_thongbao"] = "1";
            but_sapxep_moinhat.CssClass = "info small rounded";
            but_sapxep_chuadoc.CssClass = "light small rounded";
            using (dbDataContext db = new dbDataContext())
            {
                show_noidung_thongbao(db);
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

    protected void but_sapxep_chuadoc_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["sapxep_thongbao"] = "2";
            but_sapxep_moinhat.CssClass = "light small rounded";
            but_sapxep_chuadoc.CssClass = "info small rounded";
            using (dbDataContext db = new dbDataContext())
            {
                show_noidung_thongbao(db);
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

    protected void but_show_form_thongbao_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            using (dbDataContext db = new dbDataContext())
            {
                show_noidung_thongbao(db);
            }
            UpdatePanel2.Update();
            // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString()), "1000", "warning"), true);
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
    protected void but_chuadoc_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == taiKhoan && p.bin == false);
                if (q == null)
                    return;
                q.daxem = false;
                db.SubmitChanges();
                show_noidung_thongbao(db);
                show_soluong_thongbao(db);
                UpdatePanel1.Update();
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

    protected void but_dadoc_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == taiKhoan && p.bin == false);
                if (q == null)
                    return;
                q.daxem = true;
                db.SubmitChanges();
                show_noidung_thongbao(db);
                show_soluong_thongbao(db);
                UpdatePanel1.Update();
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
    protected void but_xoathongbao_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == taiKhoan && p.bin == false);
                if (q == null)
                    return;
                q.bin = true;
                db.SubmitChanges();
                show_noidung_thongbao(db);
                show_soluong_thongbao(db);
                UpdatePanel1.Update();
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
    #endregion

    protected void but_dangxuat_Click(object sender, EventArgs e)
    {
        Session["taikhoan"] = "";
        Session["matkhau"] = "";
        if (Request.Cookies["cookie_userinfo_admin_bcorn"] != null)
            Response.Cookies["cookie_userinfo_admin_bcorn"].Expires = AhaTime_cl.Now.AddDays(-1);
        Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng xuất thành công.", "2000", "warning");
        Response.Redirect("/admin/login.aspx");
    }
    #region ĐỔI MẬT KHẨU
    #endregion 

}
