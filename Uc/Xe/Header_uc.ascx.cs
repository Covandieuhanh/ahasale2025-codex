using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Uc_Xe_Header_uc : System.Web.UI.UserControl
{
    private const string DefaultHeaderCenterLogo = "/uploads/images/favicon.png";
    private const int HeaderMenuCacheSeconds = 300;

    private sealed class HeaderAccountSnapshot
    {
        public string TaiKhoan { get; set; }
        public string HoTen { get; set; }
        public string AnhDaiDien { get; set; }
        public string QrCode { get; set; }
        public string Email { get; set; }
        public string PhanLoai { get; set; }
        public string Permission { get; set; }
        public decimal DongA { get; set; }
        public decimal Vi1ThatEvocher30PhanTram { get; set; }
        public decimal Vi2ThatLaoDong50PhanTram { get; set; }
        public decimal Vi3ThatGanKet20PhanTram { get; set; }
        public decimal HoSoTieuDungShopOnly { get; set; }
        public decimal HoSoUuDaiShopOnly { get; set; }
    }

    private sealed class HeaderQuickAction
    {
        public string Url { get; set; }
        public string Label { get; set; }
        public string Note { get; set; }
        public string Badge { get; set; }
    }

    private sealed class HeaderDanhMucMarkup
    {
        public string NavHtml { get; set; }
        public string MobileHtml { get; set; }
    }

    public string show_danhmuc_nav = "";
    public string show_danhmuc_mobile = "";
    protected string HeaderCenterLogoUrl { get; private set; }
    private string _resolvedHeaderCenterLogoUrl = "";

    public Uc_Xe_Header_uc()
    {
        HeaderCenterLogoUrl = DefaultHeaderCenterLogo;
    }

    private string GetCurrentHomeAccount()
    {
        return PortalRequest_cl.GetCurrentAccount();
    }

    private static string NormalizeHeaderLogoUrl(string raw)
    {
        string value = (raw ?? "").Trim();
        if (string.IsNullOrEmpty(value))
            return "";
        if (value.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
            return "";

        Uri absolute;
        if (Uri.TryCreate(value, UriKind.Absolute, out absolute))
        {
            if (string.Equals(absolute.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                || string.Equals(absolute.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                return absolute.AbsoluteUri;
            return "";
        }

        if (value.StartsWith("~/", StringComparison.Ordinal))
            value = value.Substring(1);
        if (!value.StartsWith("/", StringComparison.Ordinal))
            value = "/" + value;

        return value;
    }

    protected string ResolveHeaderCenterLogoUrl()
    {
        if (!string.IsNullOrWhiteSpace(_resolvedHeaderCenterLogoUrl))
            return _resolvedHeaderCenterLogoUrl;

        string resolved = DefaultHeaderCenterLogo;
        HttpContext context = HttpContext.Current;
        if (context != null)
        {
            string contextLogo = NormalizeHeaderLogoUrl(Convert.ToString(context.Items["AhaHeaderCenterLogoUrl"]));
            if (string.IsNullOrEmpty(contextLogo))
                contextLogo = resolved;
            if (!string.IsNullOrWhiteSpace(contextLogo))
            {
                _resolvedHeaderCenterLogoUrl = contextLogo;
                HeaderCenterLogoUrl = contextLogo;
                return _resolvedHeaderCenterLogoUrl;
            }
        }

        _resolvedHeaderCenterLogoUrl = resolved;
        HeaderCenterLogoUrl = resolved;
        return _resolvedHeaderCenterLogoUrl;
    }

    private void BindHeaderCenterLogo(dbDataContext db)
    {
        HeaderCenterLogoUrl = ResolveHeaderCenterLogoUrl();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            HeaderCenterLogoUrl = ResolveHeaderCenterLogoUrl();
            BindDesktopSpaceBadge();

            // Luôn dựng lại trạng thái menu/account cho mọi request để tránh
            // lệch Visible của PlaceHolder sau postback/async postback trên mobile.
            BuildDanhMucTabler(1, 3, false, "web", "0");

            // Luôn nạp lại số dư hồ sơ ở mỗi request để không bị giữ ViewState cũ
            // (đặc biệt sau khi thay đổi logic từ DuVi* -> Vi*That).
            RefreshLoggedUserHeaderState();
        }
        catch (Exception ex)
        {
            HeaderCenterLogoUrl = DefaultHeaderCenterLogo;
            SafeFallbackForGuestHeader();
            Log_cl.Add_Log(ex.Message, "header_uc", ex.StackTrace);
        }
    }

    private void SafeFallbackForGuestHeader()
    {
        try
        {
            show_danhmuc_nav = "";
            show_danhmuc_mobile = "";
            if (phDangNhap != null) phDangNhap.Visible = true;
            if (PlaceHolder1 != null) PlaceHolder1.Visible = false;
            if (phTopDesktopAccount != null) phTopDesktopAccount.Visible = true;
            if (PlaceHolderLogged != null) PlaceHolderLogged.Visible = false;
            if (phAccountLogoutFooter != null) phAccountLogoutFooter.Visible = false;
            if (UpdatePanelGuestCard != null) UpdatePanelGuestCard.Visible = true;
            if (phMenuHomeYeuCau != null) phMenuHomeYeuCau.Visible = false;
            if (phMenuHomeCopyLink != null) phMenuHomeCopyLink.Visible = false;
            if (phMenuHomeDoiPin != null) phMenuHomeDoiPin.Visible = false;
            if (phMenuHomeKhachHang != null) phMenuHomeKhachHang.Visible = false;
            if (phMenuHomeDonMua != null) phMenuHomeDonMua.Visible = false;
            if (phMenuHomeLichSuTraoDoi != null) phMenuHomeLichSuTraoDoi.Visible = false;
            if (phMenuShopTinhNang != null) phMenuShopTinhNang.Visible = false;
            if (phShopLevel2Tools != null) phShopLevel2Tools.Visible = false;
            if (phShopLevel2Upgrade != null) phShopLevel2Upgrade.Visible = false;
            if (phShopLevel2Tools != null) phShopLevel2Tools.Visible = false;
            if (phShopLevel2Upgrade != null) phShopLevel2Upgrade.Visible = false;
            if (phUtilityHome != null) phUtilityHome.Visible = false;
            if (phMenuHomeExtra != null) phMenuHomeExtra.Visible = false;
            if (phHomeSpaceAccessSummary != null) phHomeSpaceAccessSummary.Visible = false;
            if (phHomeSpaceAccessSummaryMobile != null) phHomeSpaceAccessSummaryMobile.Visible = false;
            if (phTopDesktopHomeUtilities != null) phTopDesktopHomeUtilities.Visible = false;
            if (phTopNotificationDesktop != null) phTopNotificationDesktop.Visible = false;
            if (phTopNotificationDesktopShop != null) phTopNotificationDesktopShop.Visible = false;
            if (phTopMobileAccount != null) phTopMobileAccount.Visible = true;
            if (phTopMobileHomeUtilities != null) phTopMobileHomeUtilities.Visible = true;
            if (phTopMobileFavorite != null) phTopMobileFavorite.Visible = false;
            if (phTopNotificationMobile != null) phTopNotificationMobile.Visible = true;
            if (phTopMobileGuestAuth != null) phTopMobileGuestAuth.Visible = false;
            if (badgeThongBaoDesktop != null) badgeThongBaoDesktop.Visible = false;
            if (badgeThongBaoDesktopShop != null) badgeThongBaoDesktopShop.Visible = false;
            if (badgeThongBaoMobile != null) badgeThongBaoMobile.Visible = false;
        }
        catch
        {
        }
    }

    private void RefreshLoggedUserHeaderState()
    {
        try
        {
            string tkEnc = PortalRequest_cl.GetCurrentAccountEncrypted();
            if (string.IsNullOrEmpty(tkEnc))
                return;

            using (dbDataContext db = new dbDataContext())
            {
                lay_thongtin_nguoidung(db);
                show_soluong_thongbao(db);
            }

            if (phTopNotificationDesktop != null) phTopNotificationDesktop.Visible = true;
            if (phTopNotificationDesktopShop != null) phTopNotificationDesktopShop.Visible = false;
            if (phTopNotificationMobile != null) phTopNotificationMobile.Visible = true;
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "header_uc_refresh", ex.StackTrace);
        }
    }

    private void BindDesktopSpaceBadge()
    {
        if (phDesktopSpaceBadge == null || lb_desktop_space_badge == null)
            return;

        phDesktopSpaceBadge.Visible = false;
        lb_desktop_space_badge.Text = string.Empty;
    }

    protected string ResolveTopAccountLabel()
    {
        string taiKhoan = Convert.ToString(ViewState["taikhoan"]);
        if (IsCurrentSpaceGianHang() && !string.IsNullOrWhiteSpace(taiKhoan))
            return "Home " + taiKhoan.Trim();

        string hoTen = Convert.ToString(ViewState["hoten"]);
        if (!string.IsNullOrWhiteSpace(hoTen))
            return hoTen.Trim();

        if (!string.IsNullOrWhiteSpace(taiKhoan))
            return taiKhoan.Trim();

        return "Tài khoản";
    }

    protected string ResolveDropdownProfileName()
    {
        string taiKhoan = Convert.ToString(ViewState["taikhoan"]);
        if (IsCurrentSpaceGianHang() && !string.IsNullOrWhiteSpace(taiKhoan))
            return "Home " + taiKhoan.Trim();

        string hoTen = Convert.ToString(ViewState["hoten"]);
        if (!string.IsNullOrWhiteSpace(hoTen))
            return hoTen.Trim();

        if (!string.IsNullOrWhiteSpace(taiKhoan))
            return taiKhoan.Trim();

        return "Tài khoản";
    }

    protected bool IsHeaderLoggedIn()
    {
        return !string.IsNullOrEmpty(PortalRequest_cl.GetCurrentAccountEncrypted());
    }

    protected string ResolveMobileHeaderStateCssClass()
    {
        return IsHeaderLoggedIn() ? "site-header-mobile-logged" : "site-header-mobile-guest";
    }

    protected string ResolveHeaderCssClassTokens()
    {
        return (ResolveHeaderSpaceCssClass() + " " + ResolveMobileHeaderStateCssClass()).Trim();
    }

    private bool IsCurrentSpaceGianHang()
    {
        return string.Equals(ResolveCurrentSpaceCode(), ModuleSpace_cl.GianHang, StringComparison.OrdinalIgnoreCase);
    }

    private bool IsCurrentSpaceBatDongSan()
    {
        return string.Equals(ResolveCurrentSpaceCode(), ModuleSpace_cl.BatDongSan, StringComparison.OrdinalIgnoreCase);
    }

    private string ResolveGianHangActiveMenuKey()
    {
        HttpRequest request = HttpContext.Current == null ? null : HttpContext.Current.Request;
        string path = request == null || request.Url == null ? "" : (request.Url.AbsolutePath ?? "").Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(path))
            return "trang-cong-khai";

        if (path.StartsWith("/gianhang/admin/", StringComparison.OrdinalIgnoreCase)
            || path.Equals("/gianhang/admin", StringComparison.OrdinalIgnoreCase)
            || path.Equals("/gianhang/mo-cong-cu-quan-tri.aspx", StringComparison.OrdinalIgnoreCase))
            return "quan-tri";

        if (path.StartsWith("/gianhang/quan-ly-tin/", StringComparison.OrdinalIgnoreCase))
            return "quan-ly-tin";

        if (path.Equals("/gianhang/don-ban.aspx", StringComparison.OrdinalIgnoreCase)
            || path.Equals("/gianhang/cho-thanh-toan.aspx", StringComparison.OrdinalIgnoreCase)
            || path.Equals("/gianhang/hoa-don-dien-tu.aspx", StringComparison.OrdinalIgnoreCase))
            return "don-ban";

        if (path.Equals("/gianhang/quan-ly-lich-hen.aspx", StringComparison.OrdinalIgnoreCase)
            || path.Equals("/gianhang/dat-lich.aspx", StringComparison.OrdinalIgnoreCase)
            || path.Equals("/gianhang/datlich.aspx", StringComparison.OrdinalIgnoreCase))
            return "lich-hen";

        if (path.Equals("/gianhang/khach-hang.aspx", StringComparison.OrdinalIgnoreCase)
            || path.Equals("/gianhang/khach-hang-chi-tiet.aspx", StringComparison.OrdinalIgnoreCase))
            return "khach-hang";

        if (path.Equals("/gianhang/bao-cao.aspx", StringComparison.OrdinalIgnoreCase))
            return "bao-cao";

        if (path.Equals("/gianhang/nang-cap-level2.aspx", StringComparison.OrdinalIgnoreCase))
            return "nang-cap-level2";

        return "trang-cong-khai";
    }

    protected string ResolveGianHangMenuItemClass(string menuKey)
    {
        return string.Equals((menuKey ?? "").Trim(), ResolveGianHangActiveMenuKey(), StringComparison.OrdinalIgnoreCase)
            ? "is-active"
            : "";
    }

    private string ResolveCurrentSpaceCode()
    {
        HttpRequest request = HttpContext.Current == null ? null : HttpContext.Current.Request;
        string path = request == null || request.Url == null ? "" : (request.Url.AbsolutePath ?? "").Trim();
        if (string.IsNullOrEmpty(path))
            return ModuleSpace_cl.Home;

        if (path.Equals("/admin", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/admin/", StringComparison.OrdinalIgnoreCase))
            return ModuleSpace_cl.Admin;

        if (path.Equals("/shop", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/shop/", StringComparison.OrdinalIgnoreCase))
            return ModuleSpace_cl.Shop;

        if (path.Equals("/gianhang/admin", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/gianhang/admin/", StringComparison.OrdinalIgnoreCase))
            return ModuleSpace_cl.GianHangAdmin;

        if (path.Equals("/gianhang", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/gianhang/", StringComparison.OrdinalIgnoreCase))
            return ModuleSpace_cl.GianHang;

        if (path.Equals("/daugia", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/daugia/", StringComparison.OrdinalIgnoreCase))
            return ModuleSpace_cl.DauGia;

        if (path.Equals("/event", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/event/", StringComparison.OrdinalIgnoreCase))
            return ModuleSpace_cl.Event;

        if (path.Equals("/bat-dong-san", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/bat-dong-san/", StringComparison.OrdinalIgnoreCase))
            return ModuleSpace_cl.BatDongSan;

        return ModuleSpace_cl.Home;
    }

    private string ResolveCurrentSpaceLabel(string spaceCode)
    {
        switch ((spaceCode ?? "").Trim().ToLowerInvariant())
        {
            case ModuleSpace_cl.Shop:
                return "Không gian Gian hàng đối tác";
            case ModuleSpace_cl.GianHang:
                return "Không gian Gian hàng";
            case ModuleSpace_cl.GianHangAdmin:
                return "Không gian Gian hàng Admin";
            case ModuleSpace_cl.Admin:
                return "Không gian Admin";
            case ModuleSpace_cl.DauGia:
                return "Không gian Đấu giá";
            case ModuleSpace_cl.Event:
                return "Không gian Event";
            case ModuleSpace_cl.BatDongSan:
                return "Không gian Bất động sản";
            default:
                return "Không gian Home";
        }
    }

    private string ResolveCurrentSpaceCss(string spaceCode)
    {
        switch ((spaceCode ?? "").Trim().ToLowerInvariant())
        {
            case ModuleSpace_cl.Shop:
            case ModuleSpace_cl.GianHang:
                return "mode-shop";
            case ModuleSpace_cl.GianHangAdmin:
                return "mode-gianhang-admin";
            case ModuleSpace_cl.Admin:
                return "mode-admin";
            case ModuleSpace_cl.DauGia:
                return "mode-daugia";
            case ModuleSpace_cl.Event:
                return "mode-event";
            case ModuleSpace_cl.BatDongSan:
                return "mode-batdongsan";
            default:
                return "mode-home";
        }
    }

    protected string ResolveHeaderSpaceCssClass()
    {
        string spaceCode = ResolveCurrentSpaceCode();
        switch ((spaceCode ?? "").Trim().ToLowerInvariant())
        {
            case ModuleSpace_cl.Shop:
                return "site-header-space-shop site-header-shop";
            case ModuleSpace_cl.Admin:
                return "site-header-space-admin";
            case ModuleSpace_cl.GianHang:
                return "site-header-space-gianhang";
            case ModuleSpace_cl.GianHangAdmin:
                return "site-header-space-gianhang-admin";
            case ModuleSpace_cl.DauGia:
                return "site-header-space-daugia";
            case ModuleSpace_cl.Event:
                return "site-header-space-event";
            case ModuleSpace_cl.BatDongSan:
                return "site-header-space-batdongsan";
            default:
                return "site-header-space-home";
        }
    }

    protected string ResolveHeaderInlineStyle()
    {
        string spaceCode = ResolveCurrentSpaceCode();
        string gradient = "linear-gradient(110deg, #0f5a4a 0%, #198754 55%, #2fbf6d 100%)";
        string shadow = "0 8px 24px rgba(5, 28, 22, 0.2)";

        switch ((spaceCode ?? "").Trim().ToLowerInvariant())
        {
            case ModuleSpace_cl.Shop:
                gradient = "linear-gradient(110deg, #b63a1e 0%, #ee4d2d 56%, #ff8757 100%)";
                shadow = "0 8px 24px rgba(238, 77, 45, 0.25)";
                break;
            case ModuleSpace_cl.Admin:
                gradient = "linear-gradient(110deg, #7f1d1d 0%, #dc2626 58%, #ef4444 100%)";
                shadow = "0 8px 24px rgba(220, 38, 38, 0.25)";
                break;
            case ModuleSpace_cl.GianHang:
                gradient = "linear-gradient(110deg, #c2410c 0%, #f97316 56%, #fdba74 100%)";
                shadow = "0 8px 24px rgba(249, 115, 22, 0.24)";
                break;
            case ModuleSpace_cl.GianHangAdmin:
                gradient = "linear-gradient(110deg, #312e81 0%, #4f46e5 58%, #818cf8 100%)";
                shadow = "0 8px 24px rgba(79, 70, 229, 0.24)";
                break;
            case ModuleSpace_cl.DauGia:
                gradient = "linear-gradient(110deg, #9d174d 0%, #db2777 56%, #f472b6 100%)";
                shadow = "0 8px 24px rgba(219, 39, 119, 0.24)";
                break;
            case ModuleSpace_cl.Event:
                gradient = "linear-gradient(110deg, #155e75 0%, #0891b2 56%, #22d3ee 100%)";
                shadow = "0 8px 24px rgba(8, 145, 178, 0.24)";
                break;
            case ModuleSpace_cl.BatDongSan:
                gradient = "linear-gradient(110deg, #365314 0%, #65a30d 52%, #84cc16 100%)";
                shadow = "0 8px 24px rgba(101, 163, 13, 0.24)";
                break;
        }

        return "background: " + gradient + " !important; border-bottom: 1px solid rgba(255,255,255,0.2) !important; box-shadow: " + shadow + " !important;";
    }

    private bool IsCurrentDauGiaAdminSpace()
    {
        HttpRequest request = HttpContext.Current == null ? null : HttpContext.Current.Request;
        string path = request == null || request.Url == null ? "" : (request.Url.AbsolutePath ?? "").Trim();
        return path.StartsWith("/daugia/admin", StringComparison.OrdinalIgnoreCase);
    }

    private List<HeaderQuickAction> BuildHeaderQuickActions()
    {
        string spaceCode = ResolveCurrentSpaceCode();
        var items = new List<HeaderQuickAction>();

        if (string.Equals(spaceCode, ModuleSpace_cl.DauGia, StringComparison.OrdinalIgnoreCase))
        {
            bool adminSubspace = IsCurrentDauGiaAdminSpace();
            items.Add(new HeaderQuickAction { Url = "/daugia/default.aspx", Label = adminSubspace ? "Khu đấu giá public" : "Phiên live", Note = "Xem các phiên đang mở", Badge = "Live" });
            items.Add(new HeaderQuickAction { Url = "/daugia/da-ket-thuc.aspx", Label = "Phiên đã kết thúc", Note = "Tra cứu lịch sử đấu giá", Badge = "Lịch sử" });
            items.Add(new HeaderQuickAction { Url = "/daugia/admin/portal.aspx", Label = "Trung tâm quản lý", Note = "Điểm vào điều phối /daugia/admin", Badge = "Portal" });
            items.Add(new HeaderQuickAction { Url = "/daugia/admin/quan-ly.aspx", Label = "Quản lý phiên", Note = "Theo dõi toàn bộ phiên của bạn", Badge = "Admin" });
            items.Add(new HeaderQuickAction { Url = "/daugia/admin/tao.aspx", Label = "Tạo phiên mới", Note = "Khởi tạo nhanh một phiên đấu giá", Badge = "Tạo mới" });
        }
        else if (string.Equals(spaceCode, ModuleSpace_cl.Event, StringComparison.OrdinalIgnoreCase))
        {
            items.Add(new HeaderQuickAction { Url = "/event/default.aspx", Label = "Trang sự kiện", Note = "Theo dõi không gian event public", Badge = "Public" });
            items.Add(new HeaderQuickAction { Url = "/event/admin/default.aspx", Label = "Event admin", Note = "Điểm vào khu quản trị event", Badge = "Admin" });
        }
        else if (string.Equals(spaceCode, ModuleSpace_cl.BatDongSan, StringComparison.OrdinalIgnoreCase))
        {
            items.Add(new HeaderQuickAction { Url = "/bat-dong-san", Label = "Hub bất động sản", Note = "Cổng vào vertical BĐS", Badge = "Hub" });
            items.Add(new HeaderQuickAction { Url = "/bat-dong-san/mua-ban.aspx", Label = "Listing mua bán", Note = "Tập trung vào nhu cầu sale-first", Badge = "Sale" });
            items.Add(new HeaderQuickAction { Url = "/bat-dong-san/cho-thue.aspx", Label = "Listing cho thuê", Note = "Tách riêng logic rent-first", Badge = "Rent" });
        }
        return items;
    }

    protected bool ShowHeaderQuickActions()
    {
        return BuildHeaderQuickActions().Count > 0;
    }

    protected bool HideMobileSpaceBadge()
    {
        return IsCurrentSpaceBatDongSan();
    }

    protected bool HideMobileQuickActionButton()
    {
        return IsCurrentSpaceBatDongSan();
    }

    protected string ResolveHeaderQuickActionsTitle()
    {
        string spaceCode = ResolveCurrentSpaceCode();
        if (string.Equals(spaceCode, ModuleSpace_cl.DauGia, StringComparison.OrdinalIgnoreCase))
            return "Đi nhanh không gian đấu giá";
        if (string.Equals(spaceCode, ModuleSpace_cl.Event, StringComparison.OrdinalIgnoreCase))
            return "Đi nhanh không gian event";
        if (string.Equals(spaceCode, ModuleSpace_cl.BatDongSan, StringComparison.OrdinalIgnoreCase))
            return "Đi nhanh không gian bất động sản";
        return "Đi nhanh";
    }

    protected string RenderHeaderQuickActionsHtml()
    {
        var items = BuildHeaderQuickActions();
        if (items.Count == 0)
            return "";

        var html = new StringBuilder();
        foreach (HeaderQuickAction item in items)
        {
            html.Append("<a class=\"aha-header-quick-link\" href=\"")
                .Append(HttpUtility.HtmlAttributeEncode(item.Url ?? "#"))
                .Append("\">")
                .Append("<span class=\"aha-header-quick-copy\">")
                .Append("<strong>")
                .Append(HttpUtility.HtmlEncode(item.Label ?? ""))
                .Append("</strong>");

            if (!string.IsNullOrWhiteSpace(item.Note))
            {
                html.Append("<small>")
                    .Append(HttpUtility.HtmlEncode(item.Note))
                    .Append("</small>");
            }

            html.Append("</span>");

            if (!string.IsNullOrWhiteSpace(item.Badge))
            {
                html.Append("<span class=\"aha-header-quick-badge\">")
                    .Append(HttpUtility.HtmlEncode(item.Badge))
                    .Append("</span>");
            }

            html.Append("</a>");
        }

        return html.ToString();
    }

    private void BuildDanhMucTabler(int capBatDau, int capKetThuc, bool bin, string kyhieu, string idLoaiTru)
    {
        var li = new StringBuilder();
        var mobile = new StringBuilder();

        using (dbDataContext db = new dbDataContext())
        {
            BindHeaderCenterLogo(db);

            string _tk_enc = PortalRequest_cl.GetCurrentAccountEncrypted();
            string currentSpaceCode = ResolveCurrentSpaceCode();
            bool laKhongGianGianHang = string.Equals(currentSpaceCode, ModuleSpace_cl.GianHang, StringComparison.OrdinalIgnoreCase);
            bool isShopPortalRequest = PortalRequest_cl.IsShopPortalRequest();
            bool hasHomeCredential = PortalActiveMode_cl.HasHomeCredential();
            bool hasShopCredential = PortalActiveMode_cl.HasShopCredential();
            bool homeModeActive = PortalActiveMode_cl.IsHomeActive();
            bool shopModeActive = PortalActiveMode_cl.IsShopActive();
            bool showModeBadge = (homeModeActive && hasHomeCredential) || (shopModeActive && hasShopCredential);

            if (phModeBadge != null)
                phModeBadge.Visible = false;

            bool hideMobileModeBadge =
                string.Equals(currentSpaceCode, ModuleSpace_cl.Home, StringComparison.OrdinalIgnoreCase)
                || string.Equals(currentSpaceCode, ModuleSpace_cl.GianHang, StringComparison.OrdinalIgnoreCase);
            if (phModeBadgeMobile != null)
                phModeBadgeMobile.Visible = !hideMobileModeBadge;

            if (lb_mode_badge != null)
            {
                if (showModeBadge)
                {
                    bool isShopMode = shopModeActive;
                    lb_mode_badge.Text = isShopMode ? "Chế độ Shop" : "Chế độ Home";
                    lb_mode_badge.CssClass = "mode-badge " + (isShopMode ? "mode-shop" : "mode-home");
                }
                else
                {
                    lb_mode_badge.Text = "";
                }
            }

            if (lb_mode_badge_mobile != null)
            {
                lb_mode_badge_mobile.Text = ResolveCurrentSpaceLabel(currentSpaceCode);
                lb_mode_badge_mobile.CssClass = "mode-badge mode-mobile " + ResolveCurrentSpaceCss(currentSpaceCode);
            }

            if (!string.IsNullOrEmpty(_tk_enc)) // có đăng nhập
            {
                phDangNhap.Visible = false;
                PlaceHolder1.Visible = false;
                if (phTopDesktopAccount != null) phTopDesktopAccount.Visible = true;
                PlaceHolderLogged.Visible = true;
                if (phAccountLogoutFooter != null) phAccountLogoutFooter.Visible = true;
                UpdatePanelGuestCard.Visible = false;

                lay_thongtin_nguoidung(db);
                show_soluong_thongbao(db);

                int tierHome = Number_cl.Check_Int((ViewState["tier_home"] ?? "0").ToString());

                // Home và Shop tách cổng đăng nhập: chỉ scope shop mới thấy menu shop-only.
                bool laGianHangDoiTac = isShopPortalRequest || string.Equals(
                    (ViewState["portal_scope"] ?? "").ToString(),
                    PortalScope_cl.ScopeShop,
                    StringComparison.OrdinalIgnoreCase);

                if (laGianHangDoiTac)
                {
                    if (phHoSoHomeMacDinh != null) phHoSoHomeMacDinh.Visible = false;
                    if (phHoSoLaoDong != null) phHoSoLaoDong.Visible = false;
                    if (phHoSoGanKet != null) phHoSoGanKet.Visible = false;
                    if (phTopDesktopHomeUtilities != null) phTopDesktopHomeUtilities.Visible = false;
                    if (phTopMobileHomeUtilities != null) phTopMobileHomeUtilities.Visible = false;
                    if (phTopMobileFavorite != null) phTopMobileFavorite.Visible = false;
                    if (phTopNotificationDesktop != null) phTopNotificationDesktop.Visible = true;
                    if (phTopNotificationDesktopShop != null) phTopNotificationDesktopShop.Visible = false;
                    if (phTopNotificationMobile != null) phTopNotificationMobile.Visible = true;
                    if (phTopMobileAccount != null) phTopMobileAccount.Visible = true;
                }
                else
                {
                    if (phHoSoHomeMacDinh != null) phHoSoHomeMacDinh.Visible = true;
                    if (phHoSoLaoDong != null) phHoSoLaoDong.Visible = TierHome_cl.CanViewHoSo(tierHome, 3);
                    if (phHoSoGanKet != null) phHoSoGanKet.Visible = TierHome_cl.CanViewHoSo(tierHome, 4);
                    if (phTopDesktopHomeUtilities != null) phTopDesktopHomeUtilities.Visible = false;
                    if (phTopMobileHomeUtilities != null) phTopMobileHomeUtilities.Visible = true;
                    if (phTopMobileFavorite != null) phTopMobileFavorite.Visible = false;
                    if (phTopNotificationDesktop != null) phTopNotificationDesktop.Visible = true;
                    if (phTopNotificationDesktopShop != null) phTopNotificationDesktopShop.Visible = false;
                    if (phTopNotificationMobile != null) phTopNotificationMobile.Visible = true;
                    if (phTopMobileAccount != null) phTopMobileAccount.Visible = true;
                }

                if (phDonBan != null) phDonBan.Visible = laGianHangDoiTac;

                // ✅ NEW: chỉ hiện 2 hồ sơ shop khi là gian hàng đối tác
                if (phHoSoShopOnly != null) phHoSoShopOnly.Visible = laGianHangDoiTac;
                if (phMenuShopTinhNang != null) phMenuShopTinhNang.Visible = laGianHangDoiTac;
                if (!laGianHangDoiTac)
                {
                    if (phShopLevel2Tools != null) phShopLevel2Tools.Visible = false;
                    if (phShopLevel2Upgrade != null) phShopLevel2Upgrade.Visible = false;
                }
                if (phMenuHomeYeuCau != null) phMenuHomeYeuCau.Visible = !laGianHangDoiTac;
                if (phMenuHomeCopyLink != null) phMenuHomeCopyLink.Visible = !laGianHangDoiTac;
                if (phMenuHomeDoiPin != null) phMenuHomeDoiPin.Visible = !laGianHangDoiTac;
                if (phMenuHomeKhachHang != null) phMenuHomeKhachHang.Visible = !laGianHangDoiTac;
                if (phMenuHomeDonMua != null) phMenuHomeDonMua.Visible = !laGianHangDoiTac;
                if (phMenuHomeLichHen != null) phMenuHomeLichHen.Visible = !laGianHangDoiTac;
                if (phMenuHomeLichSuTraoDoi != null) phMenuHomeLichSuTraoDoi.Visible = !laGianHangDoiTac;
                if (phUtilityHome != null) phUtilityHome.Visible = !laGianHangDoiTac && !laKhongGianGianHang;
                if (phMenuHomeExtra != null) phMenuHomeExtra.Visible = !laGianHangDoiTac && !laKhongGianGianHang;
                if (phSwitchToShop != null) phSwitchToShop.Visible = !laGianHangDoiTac && hasShopCredential;
                if (phSwitchToHome != null) phSwitchToHome.Visible = laGianHangDoiTac && hasHomeCredential;
                if (phGuestSwitchHome != null) phGuestSwitchHome.Visible = false;
                if (phTopMobileGuestAuth != null) phTopMobileGuestAuth.Visible = false;

                if (phHomeSpaceAccessSummary != null && laKhongGianGianHang)
                    phHomeSpaceAccessSummary.Visible = false;
                if (phGianHangCompactMenu != null) phGianHangCompactMenu.Visible = laKhongGianGianHang;
                if (phDefaultAccountMenu != null) phDefaultAccountMenu.Visible = !laKhongGianGianHang;
                if (phGianHangCompactAdmin != null)
                {
                    RootAccount_cl.RootAccountInfo rootInfo = RootAccount_cl.GetInfo(db, GetCurrentHomeAccount());
                    phGianHangCompactAdmin.Visible = laKhongGianGianHang && rootInfo != null && rootInfo.CanAccessGianHangAdmin;
                }
                if (laKhongGianGianHang)
                {
                    if (phSwitchToShop != null) phSwitchToShop.Visible = false;
                    if (phSwitchToHome != null) phSwitchToHome.Visible = false;
                    if (phMenuShopTinhNang != null) phMenuShopTinhNang.Visible = false;
                }
            }
            else // chưa đăng nhập
            {
                phDangNhap.Visible = true;
                PlaceHolder1.Visible = false;
                if (phTopDesktopAccount != null) phTopDesktopAccount.Visible = !isShopPortalRequest;
                PlaceHolderLogged.Visible = false;
                if (phAccountLogoutFooter != null) phAccountLogoutFooter.Visible = false;
                UpdatePanelGuestCard.Visible = true;

                if (phDonBan != null) phDonBan.Visible = false;

                // ✅ NEW: chưa đăng nhập thì không hiện 2 hồ sơ shop
                if (phHoSoHomeMacDinh != null) phHoSoHomeMacDinh.Visible = false;
                if (phHoSoShopOnly != null) phHoSoShopOnly.Visible = false;
                if (phHoSoLaoDong != null) phHoSoLaoDong.Visible = false;
                if (phHoSoGanKet != null) phHoSoGanKet.Visible = false;
                if (phMenuShopTinhNang != null) phMenuShopTinhNang.Visible = false;
                if (phShopLevel2Tools != null) phShopLevel2Tools.Visible = false;
                if (phShopLevel2Upgrade != null) phShopLevel2Upgrade.Visible = false;
                if (phMenuHomeYeuCau != null) phMenuHomeYeuCau.Visible = false;
                if (phMenuHomeCopyLink != null) phMenuHomeCopyLink.Visible = false;
                if (phMenuHomeDoiPin != null) phMenuHomeDoiPin.Visible = false;
                if (phMenuHomeKhachHang != null) phMenuHomeKhachHang.Visible = false;
                if (phMenuHomeDonMua != null) phMenuHomeDonMua.Visible = false;
                if (phMenuHomeLichSuTraoDoi != null) phMenuHomeLichSuTraoDoi.Visible = false;
                if (phUtilityHome != null) phUtilityHome.Visible = false;
                if (phMenuHomeExtra != null) phMenuHomeExtra.Visible = false;
                if (phTopDesktopHomeUtilities != null) phTopDesktopHomeUtilities.Visible = false;
                if (phTopMobileHomeUtilities != null) phTopMobileHomeUtilities.Visible = false;
                if (phTopMobileFavorite != null) phTopMobileFavorite.Visible = false;
                if (phTopNotificationDesktop != null) phTopNotificationDesktop.Visible = false;
                if (phTopNotificationDesktopShop != null) phTopNotificationDesktopShop.Visible = false;
                if (phTopNotificationMobile != null) phTopNotificationMobile.Visible = false;
                if (phTopMobileAccount != null) phTopMobileAccount.Visible = !isShopPortalRequest;
                if (phTopMobileGuestAuth != null) phTopMobileGuestAuth.Visible = false;
                badgeThongBaoDesktop.Visible = false;
                if (badgeThongBaoDesktopShop != null) badgeThongBaoDesktopShop.Visible = false;
                badgeThongBaoMobile.Visible = false;
                if (phSwitchToShop != null) phSwitchToShop.Visible = false;
                if (phSwitchToHome != null) phSwitchToHome.Visible = false;
                if (phGuestSwitchHome != null) phGuestSwitchHome.Visible = (!homeModeActive && shopModeActive && hasHomeCredential);
                if (phGianHangCompactMenu != null) phGianHangCompactMenu.Visible = false;
                if (phDefaultAccountMenu != null) phDefaultAccountMenu.Visible = true;
                if (phGianHangCompactAdmin != null) phGianHangCompactAdmin.Visible = false;
            }

            // Shop portal chỉ hiển thị nút quay về trang chủ shop ở top-nav/mobile-nav.
            string portalScope = (ViewState["portal_scope"] ?? "").ToString();
            if (string.IsNullOrEmpty(portalScope))
                portalScope = isShopPortalRequest ? PortalScope_cl.ScopeShop : PortalScope_cl.ScopeHome;

            if (laKhongGianGianHang)
            {
                show_danhmuc_nav = "";
                show_danhmuc_mobile = "";
                return;
            }

            if (string.Equals(portalScope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
            {
                show_danhmuc_nav = "";
                show_danhmuc_mobile = @"<a href=""/dang-nhap?switch=home"" class=""list-group-item list-group-item-action fw-semibold"">Chuyển sang home</a>";
                return;
            }

            HeaderDanhMucMarkup cachedMarkup = GetCachedDanhMucMarkup(db, capKetThuc, bin, kyhieu, idLoaiTru);
            show_danhmuc_nav = cachedMarkup != null ? (cachedMarkup.NavHtml ?? "") : "";
            show_danhmuc_mobile = string.Equals(ResolveCurrentSpaceCode(), ModuleSpace_cl.Home, StringComparison.OrdinalIgnoreCase)
                ? ""
                : (cachedMarkup != null ? (cachedMarkup.MobileHtml ?? "") : "");
            return;

            // 1) tìm root "Danh mục" (level 1)
            var root = db.DanhMuc_tbs.FirstOrDefault(p =>
                p.id_level == 1 &&
                p.bin == bin &&
                p.kyhieu_danhmuc == kyhieu &&
                (
                    (p.name != null && p.name.Trim().ToLower() == "danh mục") ||
                    (p.name_en != null && (p.name_en.Trim().ToLower() == "danh-muc" || p.name_en.Trim().ToLower() == "danhmuc"))
                )
            );

            IQueryable<DanhMuc_tb> cap1;
            if (root != null)
            {
                // 2) chỉ lấy con trực tiếp của root Danh mục làm “cấp 1” hiển thị
                cap1 = db.DanhMuc_tbs.Where(p =>
                    p.id_parent == root.id.ToString() &&
                    p.bin == bin &&
                    p.kyhieu_danhmuc == kyhieu
                );
            }
            else
            {
                // Fallback: nếu chưa có root "Danh mục" thì lấy các mục level 2.
                cap1 = db.DanhMuc_tbs.Where(p =>
                    p.id_level == 2 &&
                    p.bin == bin &&
                    p.kyhieu_danhmuc == kyhieu
                );
            }

            if (idLoaiTru != "0")
                cap1 = cap1.Where(p => p.id.ToString() != idLoaiTru);

            if (capKetThuc != 0)
                cap1 = cap1.Where(p => p.id_level <= capKetThuc);

            // =========================
            // DESKTOP DROPDOWN: Danh mục
            // =========================
            li.Append(@"
                <li class=""nav-item dropdown"">
                  <a class=""nav-link dropdown-toggle fw-semibold"" href=""#"" data-bs-toggle=""dropdown"" data-bs-auto-close=""outside"">
                    Danh mục
                  </a>
                  <div class=""dropdown-menu dropdown-menu-arrow dm-scroll"" style=""min-width:280px; border-radius:14px;"">
                    <div class=""dm-scroll-host"">");

            foreach (var dm1 in cap1.OrderBy(p => p.rank))
            {
                bool isRootDanhMuc =
                    (dm1.name ?? "").Trim().ToLower() == "danh mục"
                    || (dm1.name_en ?? "").Trim().ToLower() == "danh-muc"
                    || (dm1.name_en ?? "").Trim().ToLower() == "danhmuc";

                // Lấy con cấp 2 của dm1
                var cap2 = db.DanhMuc_tbs.Where(p =>
                    p.id_parent == dm1.id.ToString()
                    && p.bin == bin
                    && p.kyhieu_danhmuc == kyhieu);

                if (idLoaiTru != "0")
                    cap2 = cap2.Where(p => p.id.ToString() != idLoaiTru);

                if (capKetThuc != 0)
                    cap2 = cap2.Where(p => p.id_level <= capKetThuc);

                // ✅ Nếu dm1 là root "Danh mục" -> không hiển thị dm1, đẩy con dm2 lên
                if (isRootDanhMuc)
                {
                    foreach (var dm2 in cap2.OrderBy(p => p.rank))
                    {
                        string url2 = string.IsNullOrEmpty(dm2.url_other)
                            ? ("/" + dm2.name_en + "-" + dm2.id)
                            : dm2.url_other;

                        // Lấy con cấp 3 của dm2
                        var cap3 = db.DanhMuc_tbs.Where(p =>
                            p.id_parent == dm2.id.ToString()
                            && p.bin == bin
                            && p.kyhieu_danhmuc == kyhieu);

                        if (idLoaiTru != "0")
                            cap3 = cap3.Where(p => p.id.ToString() != idLoaiTru);

                        if (cap3.Any())
                        {
                            li.AppendFormat(@"
                            <div class=""dropend dm-cap1"">
                              <a class=""dropdown-item d-flex align-items-center"" href=""{0}"">
                                <span class=""dropdown-item-icon"">{2}</span>
                                <span class=""flex-grow-1"">{1}</span>
                                <span class=""ms-auto text-muted""><i class=""ti ti-chevron-right""></i></span>
                              </a>

                              <div class=""dropdown-menu dm-submenu"" style=""min-width:280px; border-radius:14px;"">",
                                url2,
                                HttpUtility.HtmlEncode(dm2.name),
                                GetIcon(dm2.icon_html)
                            );

                            foreach (var dm3 in cap3.OrderBy(p => p.rank))
                            {
                                string url3 = string.IsNullOrEmpty(dm3.url_other)
                                    ? ("/" + dm3.name_en + "-" + dm3.id)
                                    : dm3.url_other;

                                li.AppendFormat(@"
                                    <a class=""dropdown-item"" href=""{0}"">
                                      <span class=""dropdown-item-icon"">{2}</span>
                                      {1}
                                    </a>",
                                    url3,
                                    HttpUtility.HtmlEncode(dm3.name),
                                    GetIcon(dm3.icon_html)
                                );
                            }

                            li.Append(@"
  </div>
</div>");
                        }
                        else
                        {
                            li.AppendFormat(@"
<a class=""dropdown-item d-flex align-items-center"" href=""{0}"">
  <span class=""dropdown-item-icon"">{2}</span>
  <span class=""flex-grow-1"">{1}</span>
</a>",
                                url2,
                                HttpUtility.HtmlEncode(dm2.name),
                                GetIcon(dm2.icon_html)
                            );
                        }
                    }

                    continue;
                }

                // ===== Trường hợp dm1 bình thường =====
                string url1 = string.IsNullOrEmpty(dm1.url_other)
                    ? ("/" + dm1.name_en + "-" + dm1.id)
                    : dm1.url_other;

                if (cap2.Any())
                {
                    li.AppendFormat(@"
<div class=""dropend dm-cap1"">
  <a class=""dropdown-item d-flex align-items-center"" href=""{0}"">
    <span class=""dropdown-item-icon"">{2}</span>
    <span class=""flex-grow-1"">{1}</span>
    <span class=""ms-auto text-muted""><i class=""ti ti-chevron-right""></i></span>
  </a>

  <div class=""dropdown-menu"" style=""min-width:280px; border-radius:14px;"">",
                        url1,
                        HttpUtility.HtmlEncode(dm1.name),
                        GetIcon(dm1.icon_html)
                    );

                    foreach (var dm2 in cap2.OrderBy(p => p.rank))
                    {
                        string url2 = string.IsNullOrEmpty(dm2.url_other)
                            ? ("/" + dm2.name_en + "-" + dm2.id)
                            : dm2.url_other;

                        li.AppendFormat(@"
    <a class=""dropdown-item"" href=""{0}"">
      <span class=""dropdown-item-icon"">{2}</span>
      {1}
    </a>",
                            url2,
                            HttpUtility.HtmlEncode(dm2.name),
                            GetIcon(dm2.icon_html)
                        );
                    }

                    li.Append(@"
  </div>
</div>");
                }
                else
                {
                    li.AppendFormat(@"
<a class=""dropdown-item d-flex align-items-center"" href=""{0}"">
  <span class=""dropdown-item-icon"">{2}</span>
  <span class=""flex-grow-1"">{1}</span>
</a>",
                        url1,
                        HttpUtility.HtmlEncode(dm1.name),
                        GetIcon(dm1.icon_html)
                    );
                }
            }

            // ✅ đóng dm-scroll-host + dropdown-menu + li
            li.Append(@"
    </div>
  </div>
</li>");

            // =========================
            // MOBILE OFFCANVAS
            // =========================
            mobile.Append(@"
<div class=""list-group-item fw-semibold"">Danh mục</div>");

            foreach (var dm1 in cap1.OrderBy(p => p.rank))
            {
                bool isRootDanhMuc =
                    (dm1.name ?? "").Trim().ToLower() == "danh mục"
                    || (dm1.name_en ?? "").Trim().ToLower() == "danh-muc"
                    || (dm1.name_en ?? "").Trim().ToLower() == "danhmuc";

                var cap2 = db.DanhMuc_tbs.Where(p =>
                    p.id_parent == dm1.id.ToString()
                    && p.bin == bin
                    && p.kyhieu_danhmuc == kyhieu);

                if (idLoaiTru != "0")
                    cap2 = cap2.Where(p => p.id.ToString() != idLoaiTru);

                if (capKetThuc != 0)
                    cap2 = cap2.Where(p => p.id_level <= capKetThuc);

                if (isRootDanhMuc)
                {
                    foreach (var dm2 in cap2.OrderBy(p => p.rank))
                    {
                        string url2 = string.IsNullOrEmpty(dm2.url_other)
                            ? ("/" + dm2.name_en + "-" + dm2.id)
                            : dm2.url_other;

                        mobile.AppendFormat(@"
<a href=""{0}"" class=""list-group-item list-group-item-action ps-4"">
  <span class=""me-2"">{2}</span>{1}
</a>",
                            url2,
                            HttpUtility.HtmlEncode(dm2.name),
                            GetIcon(dm2.icon_html)
                        );
                    }
                    continue;
                }

                string url1 = string.IsNullOrEmpty(dm1.url_other)
                    ? ("/" + dm1.name_en + "-" + dm1.id)
                    : dm1.url_other;

                mobile.AppendFormat(@"
<a href=""{0}"" class=""list-group-item list-group-item-action ps-4"">
  <span class=""me-2"">{2}</span>{1}
</a>",
                    url1,
                    HttpUtility.HtmlEncode(dm1.name),
                    GetIcon(dm1.icon_html)
                );

                if (cap2.Any())
                {
                    foreach (var dm2 in cap2.OrderBy(p => p.rank))
                    {
                        string url2 = string.IsNullOrEmpty(dm2.url_other)
                            ? ("/" + dm2.name_en + "-" + dm2.id)
                            : dm2.url_other;

                        mobile.AppendFormat(@"
<a href=""{0}"" class=""list-group-item list-group-item-action ps-5 text-muted"">
  <span class=""me-2"">{2}</span>{1}
</a>",
                            url2,
                            HttpUtility.HtmlEncode(dm2.name),
                            GetIcon(dm2.icon_html)
                        );
                    }
                }
            }
        }

        show_danhmuc_nav = li.ToString();
        show_danhmuc_mobile = string.Equals(ResolveCurrentSpaceCode(), ModuleSpace_cl.Home, StringComparison.OrdinalIgnoreCase)
            ? ""
            : mobile.ToString();
    }

    private HeaderDanhMucMarkup GetCachedDanhMucMarkup(dbDataContext db, int capKetThuc, bool bin, string kyhieu, string idLoaiTru)
    {
        string cacheKey = string.Format(
            "header:danhmuc:{0}:{1}:{2}:{3}",
            capKetThuc,
            bin ? "1" : "0",
            (kyhieu ?? "").Trim().ToLowerInvariant(),
            (idLoaiTru ?? "").Trim().ToLowerInvariant());

        return Helper_cl.RuntimeCacheGetOrAdd<HeaderDanhMucMarkup>(cacheKey, HeaderMenuCacheSeconds, () => BuildDanhMucMarkup(db, capKetThuc, bin, kyhieu, idLoaiTru));
    }

    private HeaderDanhMucMarkup BuildDanhMucMarkup(dbDataContext db, int capKetThuc, bool bin, string kyhieu, string idLoaiTru)
    {
        var li = new StringBuilder();
        var mobile = new StringBuilder();

        List<DanhMuc_tb> allDanhMuc = db.DanhMuc_tbs
            .Where(p => p.bin == bin && p.kyhieu_danhmuc == kyhieu && (capKetThuc == 0 || p.id_level <= capKetThuc))
            .ToList();

        Func<string, List<DanhMuc_tb>> getChildren = parentId =>
            allDanhMuc
                .Where(p => string.Equals(p.id_parent, parentId, StringComparison.OrdinalIgnoreCase)
                    && (idLoaiTru == "0" || p.id.ToString() != idLoaiTru))
                .OrderBy(p => p.rank)
                .ToList();

        Func<DanhMuc_tb, bool> isRootDanhMuc = dm =>
            dm != null && (
                (dm.name ?? "").Trim().ToLower() == "danh mục" ||
                (dm.name_en ?? "").Trim().ToLower() == "danh-muc" ||
                (dm.name_en ?? "").Trim().ToLower() == "danhmuc");

        Func<DanhMuc_tb, string> resolveUrl = dm =>
        {
            if (dm == null) return "#";
            return string.IsNullOrEmpty(dm.url_other) ? ("/" + dm.name_en + "-" + dm.id) : dm.url_other;
        };

        DanhMuc_tb root = allDanhMuc.FirstOrDefault(p =>
            p.id_level == 1 &&
            ((p.name ?? "").Trim().ToLower() == "danh mục"
             || (p.name_en ?? "").Trim().ToLower() == "danh-muc"
             || (p.name_en ?? "").Trim().ToLower() == "danhmuc"));

        List<DanhMuc_tb> cap1 = root != null
            ? getChildren(root.id.ToString())
            : allDanhMuc
                .Where(p => p.id_level == 2 && (idLoaiTru == "0" || p.id.ToString() != idLoaiTru))
                .OrderBy(p => p.rank)
                .ToList();

        li.Append(@"
                <li class=""nav-item dropdown"">
                  <a class=""nav-link dropdown-toggle fw-semibold"" href=""#"" data-bs-toggle=""dropdown"" data-bs-auto-close=""outside"">
                    Danh mục
                  </a>
                  <div class=""dropdown-menu dropdown-menu-arrow dm-scroll"" style=""min-width:280px; border-radius:14px;"">
                    <div class=""dm-scroll-host"">");

        mobile.Append(@"
<div class=""list-group-item fw-semibold"">Danh mục</div>");

        foreach (DanhMuc_tb dm1 in cap1)
        {
            if (isRootDanhMuc(dm1))
            {
                foreach (DanhMuc_tb dm2 in getChildren(dm1.id.ToString()))
                {
                    AppendDesktopDanhMucItem(li, dm2, getChildren(dm2.id.ToString()));
                    mobile.AppendFormat(@"
<a href=""{0}"" class=""list-group-item list-group-item-action ps-4"">
  <span class=""me-2"">{2}</span>{1}
</a>",
                        resolveUrl(dm2),
                        HttpUtility.HtmlEncode(dm2.name),
                        GetIcon(dm2.icon_html));
                }
                continue;
            }

            List<DanhMuc_tb> cap2 = getChildren(dm1.id.ToString());
            AppendDesktopDanhMucItem(li, dm1, cap2);

            mobile.AppendFormat(@"
<a href=""{0}"" class=""list-group-item list-group-item-action ps-4"">
  <span class=""me-2"">{2}</span>{1}
</a>",
                resolveUrl(dm1),
                HttpUtility.HtmlEncode(dm1.name),
                GetIcon(dm1.icon_html));

            foreach (DanhMuc_tb dm2 in cap2)
            {
                mobile.AppendFormat(@"
<a href=""{0}"" class=""list-group-item list-group-item-action ps-5 text-muted"">
  <span class=""me-2"">{2}</span>{1}
</a>",
                    resolveUrl(dm2),
                    HttpUtility.HtmlEncode(dm2.name),
                    GetIcon(dm2.icon_html));
            }
        }

        li.Append(@"
    </div>
  </div>
</li>");

        return new HeaderDanhMucMarkup
        {
            NavHtml = li.ToString(),
            MobileHtml = mobile.ToString()
        };
    }

    private void AppendDesktopDanhMucItem(StringBuilder html, DanhMuc_tb node, List<DanhMuc_tb> children)
    {
        string url = string.IsNullOrEmpty(node.url_other)
            ? ("/" + node.name_en + "-" + node.id)
            : node.url_other;

        if (children != null && children.Count > 0)
        {
            html.AppendFormat(@"
<div class=""dropend dm-cap1"">
  <a class=""dropdown-item d-flex align-items-center"" href=""{0}"">
    <span class=""dropdown-item-icon"">{2}</span>
    <span class=""flex-grow-1"">{1}</span>
    <span class=""ms-auto text-muted""><i class=""ti ti-chevron-right""></i></span>
  </a>

  <div class=""dropdown-menu dm-submenu"" style=""min-width:280px; border-radius:14px;"">",
                url,
                HttpUtility.HtmlEncode(node.name),
                GetIcon(node.icon_html));

            foreach (DanhMuc_tb child in children)
            {
                string childUrl = string.IsNullOrEmpty(child.url_other)
                    ? ("/" + child.name_en + "-" + child.id)
                    : child.url_other;

                html.AppendFormat(@"
    <a class=""dropdown-item"" href=""{0}"">
      <span class=""dropdown-item-icon"">{2}</span>
      {1}
    </a>",
                    childUrl,
                    HttpUtility.HtmlEncode(child.name),
                    GetIcon(child.icon_html));
            }

            html.Append(@"
  </div>
</div>");
            return;
        }

        html.AppendFormat(@"
<a class=""dropdown-item d-flex align-items-center"" href=""{0}"">
  <span class=""dropdown-item-icon"">{2}</span>
  <span class=""flex-grow-1"">{1}</span>
</a>",
            url,
            HttpUtility.HtmlEncode(node.name),
            GetIcon(node.icon_html));
    }

    public void lay_thongtin_nguoidung(dbDataContext db)
    {
        string _tk = PortalRequest_cl.GetCurrentAccountEncrypted();
        if (!string.IsNullOrEmpty(_tk))
        {
            _tk = mahoa_cl.giaima_Bcorn(_tk);

            HeaderAccountSnapshot q = db.ExecuteQuery<HeaderAccountSnapshot>(@"
SELECT TOP 1
    taikhoan AS TaiKhoan,
    hoten AS HoTen,
    anhdaidien AS AnhDaiDien,
    qr_code AS QrCode,
    email AS Email,
    phanloai AS PhanLoai,
    permission AS Permission,
    ISNULL(DongA, 0) AS DongA,
    ISNULL(Vi1That_Evocher_30PhanTram, 0) AS Vi1ThatEvocher30PhanTram,
    ISNULL(Vi2That_LaoDong_50PhanTram, 0) AS Vi2ThatLaoDong50PhanTram,
    ISNULL(Vi3That_GanKet_20PhanTram, 0) AS Vi3ThatGanKet20PhanTram,
    ISNULL(HoSo_TieuDung_ShopOnly, 0) AS HoSoTieuDungShopOnly,
    ISNULL(HoSo_UuDai_ShopOnly, 0) AS HoSoUuDaiShopOnly
FROM dbo.taikhoan_tb
WHERE taikhoan = {0}
", _tk).FirstOrDefault();
            if (q == null) return;

            ViewState["hoten"] = q.HoTen;
            ViewState["anhdaidien"] = q.AnhDaiDien;
            ViewState["qr_code"] = q.QrCode;
            ViewState["email"] = q.Email;
            ViewState["taikhoan"] = _tk;
            string scope = PortalScope_cl.ResolveScope(_tk, q.PhanLoai, q.Permission);
            ViewState["portal_scope"] = scope;
            ViewState["public_profile_link"] = string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase)
                ? ("/shop/public.aspx?user=" + HttpUtility.UrlEncode(_tk.Trim().ToLowerInvariant()))
                : ("/" + _tk.Trim().ToLowerInvariant() + ".info");
            int tierHome = TierHome_cl.GetTierFromPhanLoai(q.PhanLoai);
            if (tierHome <= TierHome_cl.Tier0)
                tierHome = TierHome_cl.Tier1;
            ViewState["tier_home"] = tierHome;
            try
            {
                BindHomeSpaceAccessSummary(db, _tk, scope);
            }
            catch
            {
                if (phHomeSpaceAccessSummary != null)
                    phHomeSpaceAccessSummary.Visible = false;
                if (phHomeSpaceAccessSummaryMobile != null)
                    phHomeSpaceAccessSummaryMobile.Visible = false;
            }

            // ✅ lưu raw để check nghiệp vụ
            ViewState["phanloai_raw"] = q.PhanLoai;

            ViewState["DongA"] = q.DongA.ToString("#,##0");

            // Dropdown hồ sơ phải hiển thị SỐ DƯ THẬT đã ghi nhận (Vi*That),
            // không hiển thị phần điểm nhận/chờ xử lý (DuVi*).
            string soDuUuDaiThat = q.Vi1ThatEvocher30PhanTram.ToString("#,##0.00");
            string soDuLaoDongThat = q.Vi2ThatLaoDong50PhanTram.ToString("#,##0.00");
            string soDuGanKetThat = q.Vi3ThatGanKet20PhanTram.ToString("#,##0.00");

            // Key mới (ưu tiên render)
            ViewState["HoSo_UuDai_Real"] = soDuUuDaiThat;
            ViewState["HoSo_LaoDong_Real"] = soDuLaoDongThat;
            ViewState["HoSo_GanKet_Real"] = soDuGanKetThat;

            // Giữ key cũ để tránh ảnh hưởng đoạn code đang dùng tên cũ.
            ViewState["DuVi1_Evocher_30PhanTram"] = soDuUuDaiThat;
            ViewState["DuVi2_LaoDong_50PhanTram"] = soDuLaoDongThat;
            ViewState["DuVi3_GanKet_20PhanTram"] = soDuGanKetThat;

            // ✅ NEW: 2 trường hồ sơ shop only (null -> 0)
            ViewState["HoSo_TieuDung_ShopOnly"] = q.HoSoTieuDungShopOnly.ToString("#,##0.00");
            ViewState["HoSo_UuDai_ShopOnly"] = q.HoSoUuDaiShopOnly.ToString("#,##0.00");

            if (scope == PortalScope_cl.ScopeShop)
            {
                ViewState["phanloai"] = "<span class=\"badge rounded-pill px-3 py-2 text-dark bg-warning\">Gian hàng đối tác</span>";
                bool hasAdvanced = ShopLevel_cl.IsAdvancedEnabled(db, _tk);
                bool pendingRequest = false;
                bool rejectedRequest = false;
                if (!hasAdvanced)
                {
                    ShopLevel2Request_cl.Level2RequestInfo req = ShopLevel2Request_cl.GetLatestRequest(db, _tk);
                    if (req != null)
                    {
                        pendingRequest = req.TrangThai == ShopLevel2Request_cl.StatusPending;
                        rejectedRequest = req.TrangThai == ShopLevel2Request_cl.StatusRejected;
                    }
                }

                if (phShopLevel2Tools != null) phShopLevel2Tools.Visible = hasAdvanced;
                if (phShopLevel2Upgrade != null) phShopLevel2Upgrade.Visible = !hasAdvanced;
                if (lb_shop_level2_cta != null)
                {
                    if (pendingRequest)
                        lb_shop_level2_cta.Text = "Đang chờ duyệt Level 2";
                    else if (rejectedRequest)
                        lb_shop_level2_cta.Text = "Gửi lại yêu cầu Level 2";
                    else
                        lb_shop_level2_cta.Text = "Yêu cầu nâng cấp Level 2";
                }
            }
            else
            {
                string tenTang = TierHome_cl.GetTenTangHome(tierHome);
                if (tenTang == "Đồng hành hệ sinh thái")
                    ViewState["phanloai"] = "<span class=\"badge rounded-pill px-3 py-2 text-dark\" style=\"background-color:#ce352c;\">Đồng hành hệ sinh thái</span>";
                else if (tenTang == "Cộng tác phát triển")
                    ViewState["phanloai"] = "<span class=\"badge rounded-pill px-3 py-2 text-dark\" style=\"background-color:#f6c945;\">Cộng tác phát triển</span>";
                else
                    ViewState["phanloai"] = "<span class=\"badge rounded-pill bg-success px-3 text-dark py-2\">Khách hàng</span>";

                if (phShopLevel2Tools != null) phShopLevel2Tools.Visible = false;
                if (phShopLevel2Upgrade != null) phShopLevel2Upgrade.Visible = false;
            }
        }
    }

    private void BindHomeSpaceAccessSummary(dbDataContext db, string accountKey, string scope)
    {
        bool hasMobileSummary = phHomeSpaceAccessSummaryMobile != null && litHomeSpaceAccessSummaryMobile != null;
        if (phHomeSpaceAccessSummary != null)
            phHomeSpaceAccessSummary.Visible = false;
        if (litHomeSpaceAccessSummary != null)
            litHomeSpaceAccessSummary.Text = "";

        if (!hasMobileSummary)
            return;

        phHomeSpaceAccessSummaryMobile.Visible = false;
        litHomeSpaceAccessSummaryMobile.Text = "";

        if (!string.Equals((scope ?? "").Trim(), PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase))
            return;

        RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetInfo(db, accountKey);
        if (info == null || !info.IsAuthenticated)
            return;

        StringBuilder html = new StringBuilder();
        AppendFixedShortcutEntry(html, "Truy cập Trang chủ", "/", "/home");
        AppendManagedShortcutEntry(
            db,
            html,
            accountKey,
            "Gian hàng đối tác",
            ModuleSpace_cl.GianHang,
            info.CanAccessGianHang,
            "/gianhang/",
            "/home/dang-ky-gian-hang-doi-tac.aspx?return_url=" + HttpUtility.UrlEncode("/gianhang/"));
        AppendFixedShortcutEntry(html, "Truy cập Đấu giá", "/daugia/", "/daugia");
        AppendFixedShortcutEntry(html, "Truy cập Sự kiện Ưu đãi", "/event/", "/event");
        AppendManagedShortcutEntry(
            db,
            html,
            accountKey,
            "Phần mềm Quản trị vận hành chuyên sâu",
            ModuleSpace_cl.GianHangAdmin,
            info.CanAccessGianHangAdmin,
            "/gianhang/admin/",
            "/home/mo-khong-gian.aspx?space=gianhang_admin&return_url=" + HttpUtility.UrlEncode("/gianhang/admin"));

        if (info.CanAccessGianHang)
        {
            AppendManagedShortcutEntry(
                db,
                html,
                accountKey,
                "Quản trị Đấu giá",
                ModuleSpace_cl.DauGia,
                info.CanAccessDauGia,
                "/daugia/admin/",
                HomeSpaceAccess_cl.BuildAccessPageUrl(ModuleSpace_cl.DauGia, "/daugia/admin/"));
        }

        if (info.CanAccessGianHang)
        {
            AppendManagedShortcutEntry(
                db,
                html,
                accountKey,
                "Quản trị sự kiện, ưu đãi",
                ModuleSpace_cl.Event,
                info.CanAccessEvent,
                "/event/admin/",
                HomeSpaceAccess_cl.BuildAccessPageUrl(ModuleSpace_cl.Event, "/event/admin/"));
        }

        if (html.Length == 0)
            return;

        string summaryHtml = html.ToString();
        litHomeSpaceAccessSummaryMobile.Text = summaryHtml;
        phHomeSpaceAccessSummaryMobile.Visible = true;
    }

    private void AppendSpaceLink(StringBuilder html, string label, string url, string routeText)
    {
        if (html == null)
            return;

        html.AppendFormat(@"
<a class=""home-space-access-link"" href=""{0}"">
    <span class=""home-space-access-copy"">
        <strong>{1}</strong>
        <span>{2}</span>
    </span>
    <i class=""ti ti-chevron-right text-secondary""></i>
</a>",
            HttpUtility.HtmlAttributeEncode(url),
            HttpUtility.HtmlEncode(label),
            HttpUtility.HtmlEncode(routeText));
    }

    private void AppendFixedShortcutEntry(StringBuilder html, string label, string url, string routeText)
    {
        if (html == null)
            return;

        AppendSpaceLink(html, label, url, routeText);
    }

    private void AppendManagedShortcutEntry(
        dbDataContext db,
        StringBuilder html,
        string accountKey,
        string displayName,
        string spaceCode,
        bool canAccess,
        string accessUrl,
        string registerUrl)
    {
        if (db == null || html == null || string.IsNullOrWhiteSpace(accountKey))
            return;

        string normalizedSpace = ModuleSpace_cl.Normalize(spaceCode);
        if (normalizedSpace == "")
            return;

        string safeAccessUrl = (accessUrl ?? "").Trim();
        string routeText = safeAccessUrl.TrimEnd('/');
        if (routeText == "")
            routeText = "/";

        if (canAccess)
        {
            AppendSpaceLink(
                html,
                "Truy cập " + displayName,
                safeAccessUrl,
                routeText);
            return;
        }

        string requestHint = BuildRequestStatusHint(
            db,
            accountKey,
            normalizedSpace,
            "Chưa duyệt");

        AppendSpaceLink(
            html,
            "Đăng ký " + displayName,
            registerUrl,
            requestHint);
    }

    private string BuildRequestStatusHint(dbDataContext db, string accountKey, string spaceCode, string fallback)
    {
        if (db == null || string.IsNullOrWhiteSpace(accountKey))
            return fallback;

        CoreSpaceRequest_cl.SpaceRequestInfo latestRequest =
            CoreSpaceRequest_cl.GetLatestRequest(db, accountKey, spaceCode);
        if (latestRequest == null)
            return fallback;

        string status = (latestRequest.RequestStatus ?? "").Trim();
        if (string.Equals(status, CoreSpaceRequest_cl.StatusPending, StringComparison.OrdinalIgnoreCase))
            return "Đang chờ duyệt · đã gửi yêu cầu";
        if (string.Equals(status, CoreSpaceRequest_cl.StatusRejected, StringComparison.OrdinalIgnoreCase))
            return "Yêu cầu gần nhất bị từ chối";
        if (string.Equals(status, CoreSpaceRequest_cl.StatusCancelled, StringComparison.OrdinalIgnoreCase))
            return "Yêu cầu gần nhất đã hủy";
        if (string.Equals(status, CoreSpaceRequest_cl.StatusApproved, StringComparison.OrdinalIgnoreCase))
            return "Yêu cầu gần nhất đã duyệt";

        return fallback;
    }

    private string GetIcon(string iconHtml)
    {
        if (string.IsNullOrWhiteSpace(iconHtml))
            return "<span class='ti ti-category'></span>";

        return iconHtml;
    }

    #region ✅ COPY LINK GIỚI THIỆU (NEW)
    protected void but_copy_link_gioithieu_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", false);

        string tk = Convert.ToString(ViewState["taikhoan"]) ?? "";
        if (string.IsNullOrEmpty(tk))
        {
            Helper_Tabler_cl.ShowToast(Page, "Bạn chưa đăng nhập", "warning");
            return;
        }

        string url = "https://ahasale.vn/home/page/gioi-thieu-nguoi-dung.aspx?u=" + tk;
        string safeUrl = HttpUtility.JavaScriptStringEncode(url);

        // copy clipboard (navigator.clipboard + fallback)
        string jsCopy = string.Format(@"
(function(){{
    var text = '{0}';
    function fallbackCopy(t) {{
        var ta = document.createElement('textarea');
        ta.value = t;
        ta.setAttribute('readonly', '');
        ta.style.position = 'fixed';
        ta.style.top = '-1000px';
        document.body.appendChild(ta);
        ta.select();
        try {{ document.execCommand('copy'); }} catch(e){{}}
        document.body.removeChild(ta);
    }}
    if (navigator.clipboard && navigator.clipboard.writeText) {{
        navigator.clipboard.writeText(text).catch(function(){{ fallbackCopy(text); }});
    }} else {{
        fallbackCopy(text);
    }}
}})();", safeUrl);

        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "copy_ref_link_" + Guid.NewGuid().ToString("N"), jsCopy, true);

        // toast "Đã copy" (đúng cơ chế updatepanel)
        Helper_Tabler_cl.ShowToast(Page, "Đã copy", "success", true, 3000, "Thông báo");
    }
    #endregion

    #region thông báo
    protected void Timer1_Tick(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string _tk = GetCurrentHomeAccount();
            if (!string.IsNullOrEmpty(_tk))
            {
                show_soluong_thongbao(db);
            }
        }
    }

    public void show_soluong_thongbao(dbDataContext db)
    {
        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk))
        {
            lb_sl_thongbao_desktop.Text = "0";
            lb_sl_thongbao_mobile.Text = "0";
            badgeThongBaoDesktop.Visible = false;
            badgeThongBaoMobile.Visible = false;
            if (lb_sl_thongbao_desktop_shop != null)
                lb_sl_thongbao_desktop_shop.Text = "0";
            if (badgeThongBaoDesktopShop != null)
                badgeThongBaoDesktopShop.Visible = false;
            lb_sl_giohang_desktop.Text = "0";
            lb_sl_giohang_mobile.Text = "0";
            badgeGioHangDesktop.Visible = false;
            badgeGioHangMobile.Visible = false;
            return;
        }

        int soLuongThongBaoChuaDoc = db.ThongBao_tbs.Count(p => p.nguoinhan == _tk && p.daxem == false && p.bin == false);
        string badgeText = soLuongThongBaoChuaDoc < 100 ? soLuongThongBaoChuaDoc.ToString() : "99+";
        bool coThongBao = soLuongThongBaoChuaDoc > 0;

        lb_sl_thongbao_desktop.Text = badgeText;
        lb_sl_thongbao_mobile.Text = badgeText;
        badgeThongBaoDesktop.Visible = coThongBao;
        badgeThongBaoMobile.Visible = coThongBao;
        if (lb_sl_thongbao_desktop_shop != null)
            lb_sl_thongbao_desktop_shop.Text = badgeText;
        if (badgeThongBaoDesktopShop != null)
            badgeThongBaoDesktopShop.Visible = coThongBao;

        int soLuongGioHang = db.GioHang_tbs.Count(p => p.taikhoan == _tk);
        string gioHangBadgeText = soLuongGioHang < 100 ? soLuongGioHang.ToString() : "99+";
        bool coGioHang = soLuongGioHang > 0;

        lb_sl_giohang_desktop.Text = gioHangBadgeText;
        lb_sl_giohang_mobile.Text = gioHangBadgeText;
        badgeGioHangDesktop.Visible = coGioHang;
        badgeGioHangMobile.Visible = coGioHang;
    }

    public void show_noidung_thongbao(dbDataContext db)
    {
        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk))
        {
            Repeater1.DataSource = new object[0];
            Repeater1.DataBind();
            ph_empty_thongbao.Visible = true;
            return;
        }

        var query = from ob1 in db.ThongBao_tbs
                    join ob2 in db.taikhoan_tbs
                        on ob1.nguoithongbao equals ob2.taikhoan into senderGroup
                    from ob2 in senderGroup.DefaultIfEmpty()
                    where ob1.nguoinhan == _tk
                          && ob1.bin == false
                    select new
                    {
                        ob1.id,
                        avt_nguoithongbao = (ob2 == null || ob2.anhdaidien == null || ob2.anhdaidien == "")
                            ? "/uploads/images/macdinh.jpg"
                            : ob2.anhdaidien,
                        daxem = ob1.daxem,
                        noidung = ob1.noidung ?? "",
                        thoigian = ob1.thoigian,
                        link = (ob1.link == null || ob1.link == "")
                            ? "/home/default.aspx?"
                            : (ob1.link.Contains("?") ? ob1.link + "&" : ob1.link + "?")
                    };

        if (Convert.ToString(ViewState["sapxep_thongbao"]) == "2")
        {
            query = query
                .Where(p => p.daxem == false)
                .OrderByDescending(p => p.thoigian);
        }
        else
        {
            query = query
                .OrderByDescending(p => p.thoigian);
        }

        var result = query.Take(20).ToList();

        Repeater1.DataSource = result;
        Repeater1.DataBind();
        ph_empty_thongbao.Visible = result.Count == 0;

        ScriptManager.RegisterStartupScript(
            this,
            GetType(),
            "openNotif",
            "showNotif();",
            true
        );
    }

    protected void but_sapxep_moinhat_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", false);
        ViewState["sapxep_thongbao"] = "1";
        but_sapxep_moinhat_desk.CssClass = "btn btn-sm btn-outline-secondary active";
        but_sapxep_chuadoc_desk.CssClass = "btn btn-sm btn-outline-secondary";

        using (dbDataContext db = new dbDataContext())
        {
            show_noidung_thongbao(db);
        }
    }

    protected void but_sapxep_chuadoc_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", false);
        ViewState["sapxep_thongbao"] = "2";
        but_sapxep_moinhat_desk.CssClass = "btn btn-sm btn-outline-secondary";
        but_sapxep_chuadoc_desk.CssClass = "btn btn-sm btn-outline-secondary active";

        using (dbDataContext db = new dbDataContext())
        {
            show_noidung_thongbao(db);
        }
    }

    protected void but_show_form_thongbao_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", false);

        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk)) return;

        using (dbDataContext db = new dbDataContext())
        {
            // đánh dấu đã xem
            var q = db.ThongBao_tbs.Where(p => p.nguoinhan == _tk && p.daxem == false && p.bin == false);
            foreach (var t in q) t.daxem = true;
            db.SubmitChanges();

            show_noidung_thongbao(db);
            show_soluong_thongbao(db);
        }

        UpdatePanel2.Update();

        ScriptManager.RegisterStartupScript(
            Page,
            Page.GetType(),
            "openNoti",
            "showNotif();",
            true
        );
    }

    protected void but_chuadoc_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", false);
        LinkButton button = (LinkButton)sender;
        string _id = button.CommandArgument;
        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == _tk && p.bin == false);
            if (q == null) return;

            q.daxem = false;
            db.SubmitChanges();

            show_noidung_thongbao(db);
            show_soluong_thongbao(db);
        }
    }

    protected void but_dadoc_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", false);
        LinkButton button = (LinkButton)sender;
        string _id = button.CommandArgument;
        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == _tk && p.bin == false);
            if (q == null) return;

            q.daxem = true;
            db.SubmitChanges();

            show_noidung_thongbao(db);
            show_soluong_thongbao(db);
        }
    }

    protected void but_xoathongbao_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", false);
        LinkButton button = (LinkButton)sender;
        string _id = button.CommandArgument;
        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == _tk && p.bin == false);
            if (q == null) return;

            q.bin = true;
            db.SubmitChanges();

            show_noidung_thongbao(db);
            show_soluong_thongbao(db);
        }
    }
    #endregion

    protected void dangxuat_Click(object sender, EventArgs e)
    {
        bool isShopPortal = PortalRequest_cl.IsShopPortalRequest();
        if (isShopPortal)
        {
            Session["taikhoan_shop"] = "";
            Session["matkhau_shop"] = "";
            if (Request.Cookies["cookie_userinfo_shop_bcorn"] != null)
                Response.Cookies["cookie_userinfo_shop_bcorn"].Expires = DateTime.Now.AddDays(-1);
            Session["ThongBao_Shop"] = "toast|Thông báo|Đăng xuất thành công.|warning|1000";
            Response.Redirect("/shop/login.aspx");
            return;
        }

        Session["taikhoan_home"] = "";
        Session["matkhau_home"] = "";

        if (Request.Cookies["cookie_userinfo_home_bcorn"] != null)
            Response.Cookies["cookie_userinfo_home_bcorn"].Expires = DateTime.Now.AddDays(-1);

        Session["thongbao_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng xuất thành công.", "1000", "warning");
        Response.Redirect("/");
    }
}
