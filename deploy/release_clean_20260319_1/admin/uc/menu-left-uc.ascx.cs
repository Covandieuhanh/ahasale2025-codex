using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_uc_menu_left_uc : System.Web.UI.UserControl
{
    public string loi, tuvan;
    private const string PermissionManageAdminAccounts = "5";
    private const string PermissionLegacyGeneralAdmin = "1";
    private const string PermissionHomeContent = "q3_1";

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

    private static HashSet<string> ParsePermissionTokens(string permissionRaw)
    {
        var tokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(permissionRaw))
            return tokens;

        string[] arr = permissionRaw
            .Split(new[] { ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string token in arr)
        {
            string t = (token ?? "").Trim();
            if (t != "")
                tokens.Add(t);
        }
        return tokens;
    }

    private void BuildLeftMenuPermissionFlags(taikhoan_tb account)
    {
        bool isRoot = account != null && PermissionProfile_cl.IsRootAdmin(account.taikhoan);
        SetFlag("left_is_root", isRoot);
        if (isRoot)
        {
            SetFlag("left_admin_dashboard", true);
            SetFlag("left_admin_account", true);
            SetFlag("left_transfer_history", true);
            SetFlag("left_home_account", true);
            SetFlag("left_home_approve_hanhvi", true);
            SetFlag("left_home_issue_card", true);
            SetFlag("left_home_tier_desc", true);
            SetFlag("left_home_sell_product", true);
            SetFlag("left_shop_account", true);
            SetFlag("left_shop_approve", true);
            SetFlag("left_shop_email_template", true);
            SetFlag("left_content_home", true);
            SetFlag("left_content_home_text", true);
            SetFlag("left_content_menu", true);
            SetFlag("left_content_baiviet", true);
            SetFlag("left_content_banner", true);
            SetFlag("left_content_gopy", true);
            SetFlag("left_content_thongbao", true);
            SetFlag("left_content_tuvan", true);
            SetFlag("left_group_admin", true);
            SetFlag("left_group_home", true);
            SetFlag("left_group_shop", true);
            SetFlag("left_group_content", true);
            return;
        }

        HashSet<string> tokens = ParsePermissionTokens(account != null ? account.permission : "");
        bool legacyGeneral = tokens.Contains(PermissionLegacyGeneralAdmin);
        bool canManageAdminAccounts = tokens.Contains(PermissionManageAdminAccounts);

        bool canLegacyTransfer = PermissionProfile_cl.LegacyTieuDungPermissions.Any(code => tokens.Contains(code));
        bool canTieuDung = tokens.Contains(PermissionProfile_cl.HoSoTieuDung);
        bool canUuDai = tokens.Contains(PermissionProfile_cl.HoSoUuDai);
        bool canLaoDong = tokens.Contains(PermissionProfile_cl.HoSoLaoDong);
        bool canGanKet = tokens.Contains(PermissionProfile_cl.HoSoGanKet);
        bool canShopOnly = tokens.Contains(PermissionProfile_cl.HoSoShopOnly);
        bool canHomeContent = tokens.Contains(PermissionHomeContent);

        bool canApproveHanhVi = canUuDai || canLaoDong || canGanKet;
        bool canHomeAccount = canTieuDung || canApproveHanhVi;
        bool canTransferHistory = canLegacyTransfer || canTieuDung;

        bool showAdminDashboard = false;
        bool showAdminAccount = canManageAdminAccounts;
        bool showTransferHistory = legacyGeneral || canTransferHistory;
        bool showHomeAccount = legacyGeneral || canHomeAccount;
        bool showApproveHanhVi = legacyGeneral || canApproveHanhVi;
        bool showIssueCard = legacyGeneral || canTieuDung;
        bool showTierDescription = legacyGeneral || canApproveHanhVi;
        bool showSellProduct = legacyGeneral || canTieuDung;
        bool showShopAccount = legacyGeneral || canShopOnly;
        bool showShopApprove = legacyGeneral || canShopOnly;
        bool showShopEmailTemplate = legacyGeneral || canShopOnly;
        bool showHomeSettings = false;
        bool showOtherContent = legacyGeneral || canHomeContent;

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
        SetFlag("left_content_home", showHomeSettings);
        SetFlag("left_content_home_text", showOtherContent);
        SetFlag("left_content_menu", showOtherContent);
        SetFlag("left_content_baiviet", showOtherContent);
        SetFlag("left_content_banner", showOtherContent);
        SetFlag("left_content_gopy", showOtherContent);
        SetFlag("left_content_thongbao", showOtherContent);
        SetFlag("left_content_tuvan", showOtherContent);

        SetFlag("left_group_admin", showAdminDashboard || showAdminAccount || showTransferHistory);
        SetFlag("left_group_home", showHomeAccount || showApproveHanhVi || showIssueCard || showTierDescription || showSellProduct);
        SetFlag("left_group_shop", showShopAccount || showShopApprove || showShopEmailTemplate);
        SetFlag("left_group_content", showHomeSettings || showOtherContent);
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
        string tkEnc = Session["taikhoan"] as string;
        if (string.IsNullOrEmpty(tkEnc))
            return false;

        string tk = "";
        try { tk = mahoa_cl.giaima_Bcorn(tkEnc); }
        catch { tk = tkEnc; }

        if (PermissionProfile_cl.IsRootAdmin(tk))
            return true;

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                return PermissionProfile_cl.HasPermission(db, tk, PermissionHomeContent);
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
    public bool ShowLeftContentMenu() { return GetFlag("left_content_menu"); }
    public bool ShowLeftContentBaiViet() { return GetFlag("left_content_baiviet"); }
    public bool ShowLeftContentBanner() { return GetFlag("left_content_banner"); }
    public bool ShowLeftContentGopY() { return GetFlag("left_content_gopy"); }
    public bool ShowLeftContentThongBao() { return GetFlag("left_content_thongbao"); }
    public bool ShowLeftContentTuVan() { return GetFlag("left_content_tuvan"); }

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

    public string MenuActiveTaiKhoanScope(string scope)
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        if (currentUrl != "/admin/quan-ly-tai-khoan/default.aspx")
            return "";

        string currentScope = (Request.QueryString["scope"] ?? "").Trim().ToLowerInvariant();
        string targetScope = (scope ?? "").Trim().ToLowerInvariant();
        if (currentScope == targetScope)
            return "active";
        return "";
    }

    private static string ResolveTitle(string url)
    {
        switch ((url ?? "").ToLower().Trim())
        {
            case "/admin/quan-ly-menu/default.aspx":
                return "Quản lý menu";
            case "/admin/quan-ly-bai-viet/default.aspx":
            case "/admin/quan-ly-bai-viet/in.aspx":
                return "Quản lý bài viết";
            case "/admin/quan-ly-banner/default.aspx":
                return "Quản lý banner";
            case "/admin/quan-ly-gop-y/default.aspx":
                return "Quản lý góp ý";
            case "/admin/quan-ly-thong-bao/default.aspx":
            case "/admin/quan-ly-thong-bao/in.aspx":
                return "Quản lý thông báo";
            case "/admin/yeu-cau-tu-van/default.aspx":
                return "Yêu cầu tư vấn";
            case "/admin/lich-su-chuyen-diem/default.aspx":
                return "Lịch sử chuyển điểm";
            case "/admin/cai-dat-trang-chu/default.aspx":
                return "Cài đặt trang chủ";
            case "/admin/quan-ly-noi-dung-home/default.aspx":
                return "Nội dung trang chủ Home";
            case "/admin/quan-ly-tai-khoan/Default.aspx":
                return "Quản lý tài khoản";
            case "/admin/duyet-yeu-cau-len-cap.aspx":
                return "Duyệt yêu cầu xác nhận hành vi";
            case "/admin/duyet-gian-hang-doi-tac.aspx":
                return "Duyệt gian hàng đối tác";
            case "/admin/phat-hanh-the.aspx":
                return "Phát hành thẻ";
            case "/admin/motacapbac.aspx":
                return "Mô tả cấp bậc";
            case "/admin/he-thong-san-pham/ban-san-pham.aspx":
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
