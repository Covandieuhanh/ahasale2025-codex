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
        GianHangAdminMenuPolicy_cl.MenuVisibility model = GianHangAdminMenuPolicy_cl.Build(account, false);
        SetFlag("left_is_root", model.IsRoot);
        SetFlag("left_admin_dashboard", model.Dashboard);
        SetFlag("left_admin_account", model.AdminAccount);
        SetFlag("left_transfer_history", model.TransferHistory);
        SetFlag("left_home_account", model.HomeAccount);
        SetFlag("left_home_approve_hanhvi", model.ApproveHanhVi);
        SetFlag("left_home_issue_card", model.IssueCard);
        SetFlag("left_home_tier_desc", model.TierDescription);
        SetFlag("left_home_sell_product", model.SellProduct);
        SetFlag("left_shop_account", model.ShopAccount);
        SetFlag("left_shop_approve", model.ShopApprove);
        SetFlag("left_shop_email_template", model.ShopEmailTemplate);
        SetFlag("left_content_home", model.ContentHome);
        SetFlag("left_content_home_text", model.ContentHomeText);
        SetFlag("left_content_menu", model.ContentMenu);
        SetFlag("left_content_baiviet", model.ContentBaiViet);
        SetFlag("left_content_banner", model.ContentBanner);
        SetFlag("left_content_gopy", model.ContentGopY);
        SetFlag("left_content_thongbao", model.ContentThongBao);
        SetFlag("left_content_tuvan", model.ContentTuVan);
        SetFlag("left_group_admin", model.GroupAdmin);
        SetFlag("left_group_home", model.GroupHome);
        SetFlag("left_group_shop", model.GroupShop);
        SetFlag("left_group_content", model.GroupContent);
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
        if (currentUrl != "/gianhang/admin/quan-ly-tai-khoan/default.aspx")
            return "";

        string currentScope = (Request.QueryString["scope"] ?? "").Trim().ToLowerInvariant();
        string targetScope = (scope ?? "").Trim().ToLowerInvariant();
        if (currentScope == targetScope)
            return "active";
        return "";
    }

    private void BindSpaceAccessSummary(dbDataContext db)
    {
        if (ph_space_access_summary == null || lit_space_access_summary == null)
            return;

        ph_space_access_summary.Visible = false;
        lit_space_access_summary.Text = "";

        GlobalSpaceLauncher_cl.LauncherModel model = GlobalSpaceLauncher_cl.BuildCurrent(db, Request == null ? "" : Request.RawUrl);
        if (model == null || !model.Visible || string.IsNullOrWhiteSpace(model.ItemsHtml))
            return;

        lit_space_access_summary.Text = model.ItemsHtml;
        ph_space_access_summary.Visible = true;
    }

    private static string ResolveTitle(string url)
    {
        switch ((url ?? "").ToLower().Trim())
        {
            case "/gianhang/admin/quan-ly-menu/default.aspx":
                return "Quản lý menu";
            case "/gianhang/admin/quan-ly-bai-viet/default.aspx":
            case "/gianhang/admin/quan-ly-bai-viet/in.aspx":
                return "Quản lý bài viết";
            case "/gianhang/admin/quan-ly-banner/default.aspx":
                return "Quản lý banner";
            case "/gianhang/admin/quan-ly-gop-y/default.aspx":
                return "Quản lý góp ý";
            case "/gianhang/admin/quan-ly-thong-bao/default.aspx":
            case "/gianhang/admin/quan-ly-thong-bao/in.aspx":
                return "Quản lý thông báo";
            case "/gianhang/admin/yeu-cau-tu-van/default.aspx":
                return "Yêu cầu tư vấn";
            case "/gianhang/admin/lich-su-chuyen-diem/default.aspx":
                return "Lịch sử chuyển điểm";
            case "/gianhang/admin/cai-dat-trang-chu/default.aspx":
                return "Cài đặt trang chủ";
            case "/gianhang/admin/quan-ly-noi-dung-home/default.aspx":
                return "Nội dung trang chủ Home";
            case "/gianhang/admin/quan-ly-tai-khoan/default.aspx":
                return "Quản lý tài khoản";
            case "/gianhang/admin/duyet-yeu-cau-len-cap.aspx":
                return "Duyệt yêu cầu xác nhận hành vi";
            case "/gianhang/admin/duyet-gian-hang-doi-tac.aspx":
                return "Duyệt gian hàng đối tác";
            case "/gianhang/admin/phat-hanh-the.aspx":
                return "Phát hành thẻ";
            case "/gianhang/admin/motacapbac.aspx":
                return "Mô tả cấp bậc";
            case "/gianhang/admin/he-thong-san-pham/ban-san-pham.aspx":
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
                string title = GianHangAdminRouteMap_cl.ResolveTitle(Request);
                if (string.IsNullOrWhiteSpace(title))
                    title = ResolveTitle(_url);
                Session["title"] = title;

                using (dbDataContext db = new dbDataContext())
                {
                    taikhoan_tb account = GianHangAdminContext_cl.ResolveDisplayAccount(db);
                    BuildLeftMenuPermissionFlags(account);
                    BindSpaceAccessSummary(db);

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
                Log_cl.Add_Log(_ex.Message, GianHangAdminContext_cl.ResolveDisplayAccountKey(), _ex.StackTrace);
            }
        }
    }
}
