<%@ Control Language="C#" ClassName="uc_bds_nav" %>
<%@ Import Namespace="System.Linq" %>
<script runat="server">
    private string _accountAvatarUrl = "";
    private string _headerLogoUrl = "";
    private string _currentAccount = "";
    private string _currentDisplayName = "";
    private string _currentRoleLabel = "KhÃ¡ch hÃ ng";
    private int _unreadNotificationCount;

    private bool? _showTopbar;
    private bool? _showBanner;
    private bool? _showNav;
    private bool? _includeAssets;

    public string Current { get; set; }
    public bool ShowTopbar
    {
        get { return _showTopbar ?? true; }
        set { _showTopbar = value; }
    }
    public bool ShowBanner
    {
        get { return _showBanner ?? true; }
        set { _showBanner = value; }
    }
    public bool ShowNav
    {
        get { return _showNav ?? true; }
        set { _showNav = value; }
    }
    public bool IncludeAssets
    {
        get { return _includeAssets ?? true; }
        set { _includeAssets = value; }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        ResolveBranding();
        ResolveCurrentAccountUi();
    }

    protected string NavClass(string key, string activeClass, string defaultClass)
    {
        return string.Equals((Current ?? "").Trim(), key, StringComparison.OrdinalIgnoreCase)
            ? activeClass
            : defaultClass;
    }

    protected string ThemeCssUrl()
    {
        return Helper_cl.VersionedUrl("~/bat-dong-san/bds-theme.css") + "&bds_rev=20260404a";
    }

    protected string BannerImageUrl()
    {
        return Helper_cl.VersionedUrl("~/uploads/images/bat-dong-san/bds-banner-top.png");
    }

    protected string NavJsUrl()
    {
        return Helper_cl.VersionedUrl("~/bat-dong-san/bds-nav.js") + "&bds_rev=20260404a";
    }

    protected string HeaderLogoUrl()
    {
        if (!string.IsNullOrWhiteSpace(_headerLogoUrl))
            return _headerLogoUrl;

        return Helper_cl.VersionedUrl("~/uploads/images/logo-aha-trang.png");
    }

    protected string HeaderAccountAvatarUrl()
    {
        if (!string.IsNullOrWhiteSpace(_accountAvatarUrl))
            return _accountAvatarUrl;

        return Helper_cl.VersionedUrl("~/uploads/images/guest-avatar-mobile.png");
    }

    protected bool HeaderIsLoggedIn()
    {
        return !string.IsNullOrWhiteSpace(_currentAccount);
    }

    protected string HeaderDisplayName()
    {
        if (!string.IsNullOrWhiteSpace(_currentDisplayName))
            return _currentDisplayName;
        return _currentAccount;
    }

    protected string HeaderUnreadBadgeCss()
    {
        return _unreadNotificationCount > 0 ? "bds-topbar-badge" : "bds-topbar-badge d-none";
    }

    protected string HeaderUnreadBadgeText()
    {
        if (_unreadNotificationCount <= 0)
            return "0";
        if (_unreadNotificationCount > 99)
            return "99+";
        return _unreadNotificationCount.ToString();
    }

    protected string HeaderReturnUrl()
    {
        string raw = (Request == null ? "" : (Request.RawUrl ?? ""));
        if (string.IsNullOrWhiteSpace(raw))
            raw = "/bat-dong-san";
        return System.Web.HttpUtility.UrlEncode(raw);
    }

    protected string BuildTopbarAccountTitle()
    {
        if (!HeaderIsLoggedIn())
            return "TÃ i khoáº£n khÃ¡ch";
        string name = HeaderDisplayName();
        return string.IsNullOrWhiteSpace(name) ? _currentAccount : name;
    }

    protected string HeaderRoleLabel()
    {
        if (string.IsNullOrWhiteSpace(_currentRoleLabel))
            return "KhÃ¡ch hÃ ng";
        return _currentRoleLabel;
    }

    protected string HeaderProfileName()
    {
        if (!string.IsNullOrWhiteSpace(_currentDisplayName))
            return _currentDisplayName;
        if (!string.IsNullOrWhiteSpace(_currentAccount))
            return "Home " + _currentAccount;
        return "KhÃ¡ch";
    }

    protected string HeaderReferralLink()
    {
        string tk = (_currentAccount ?? "").Trim();
        if (string.IsNullOrWhiteSpace(tk))
            return "";
        return "https://ahasale.vn/home/page/gioi-thieu-nguoi-dung.aspx?u=" + tk;
    }

    protected string SpaceLinkCss(string key)
    {
        string current = "bds";
        if (string.Equals(current, key, StringComparison.OrdinalIgnoreCase))
            return "list-group-item list-group-item-action active";
        return "list-group-item list-group-item-action";
    }

    private void ResolveCurrentAccountUi()
    {
        _currentAccount = (PortalRequest_cl.GetCurrentAccount() ?? "").Trim();
        _currentDisplayName = "";
        _currentRoleLabel = "KhÃ¡ch hÃ ng";
        _unreadNotificationCount = 0;
        _accountAvatarUrl = Helper_cl.VersionedUrl("~/uploads/images/guest-avatar-mobile.png");

        string tk = _currentAccount;
        if (string.IsNullOrWhiteSpace(tk))
            return;

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
                if (account == null)
                    return;

                _currentDisplayName = (account.hoten ?? "").Trim();
                _currentRoleLabel = (account.phanloai ?? "").Trim();
                if (string.IsNullOrWhiteSpace(_currentRoleLabel))
                    _currentRoleLabel = "KhÃ¡ch hÃ ng";
                string avatar = (account.anhdaidien ?? "").Trim();
                if (!string.IsNullOrWhiteSpace(avatar))
                    _accountAvatarUrl = avatar;

                _unreadNotificationCount = db.ThongBao_tbs.Count(p =>
                    p.nguoinhan == tk &&
                    p.daxem == false &&
                    p.bin == false);
            }
        }
        catch
        {
            _unreadNotificationCount = 0;
        }
    }

    private void ResolveBranding()
    {
        _headerLogoUrl = Helper_cl.VersionedUrl("~/uploads/images/logo-aha-trang.png");

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                PortalBranding_cl.ScopeBrandingSnapshot branding = PortalBranding_cl.LoadScopeBranding(
                    db,
                    PortalBranding_cl.ScopeBatDongSan,
                    true);

                _headerLogoUrl = PortalBranding_cl.ResolveHeaderLogoPath(
                    branding,
                    PortalBranding_cl.ScopeBatDongSan);
            }
        }
        catch
        {
        }
    }
</script>

<% if (IncludeAssets) { %>
<link rel="stylesheet" type="text/css" href="<%= ThemeCssUrl() %>" />
<script src="<%= NavJsUrl() %>" defer></script>
<% } %>

<% if (ShowTopbar) { %>
<section class="bds-topbar-wrap" aria-label="Thanh Ä‘iá»u hÆ°á»›ng báº¥t Ä‘á»™ng sáº£n">
    <header class="bds-topbar">
        <div class="bds-topbar-inner">
            <div class="bds-topbar-side bds-topbar-side--left">
                <button class="bds-topbar-circle" type="button"
                    data-bs-toggle="offcanvas" data-bs-target="#bdsMobileMenuCanvas"
                    aria-controls="bdsMobileMenuCanvas" aria-label="Menu">
                    <i class="ti ti-menu-2"></i>
                </button>
            </div>

            <a class="bds-topbar-brand" href="/bat-dong-san" aria-label="Aha Sale báº¥t Ä‘á»™ng sáº£n">
                <img src="<%= HeaderLogoUrl() %>" alt="Aha Sale" class="bds-topbar-brand-logo" />
            </a>

            <div class="bds-topbar-side bds-topbar-side--right">
                <button type="button"
                    class="bds-topbar-circle bds-topbar-circle--notif"
                    data-bs-toggle="offcanvas"
                    data-bs-target="#bdsNotifCanvas"
                    aria-controls="bdsNotifCanvas"
                    aria-label="ThÃ´ng bÃ¡o">
                    <i class="ti ti-bell"></i>
                    <span id="bdsNotifBadge" class="<%= HeaderUnreadBadgeCss() %>"><%= HeaderUnreadBadgeText() %></span>
                </button>
                <button type="button"
                    class="bds-topbar-account"
                    data-bs-toggle="offcanvas"
                    data-bs-target="#accountMenuCanvas"
                    aria-controls="accountMenuCanvas"
                    aria-label="TÃ i khoáº£n">
                    <% if (HeaderIsLoggedIn()) { %>
                    <img src="<%= HeaderAccountAvatarUrl() %>" alt="" class="bds-topbar-account-avatar" id="bdsTopbarAccountAvatar" />
                    <% } else { %>
                    <span class="bds-topbar-account-avatar bds-topbar-account-avatar--guest" id="bdsTopbarAccountAvatar" aria-hidden="true">TÃ i khoáº£n</span>
                    <% } %>
                </button>
            </div>
        </div>
    </header>
    <div class="bds-topbar-spacer" aria-hidden="true"></div>
</section>

<div class="offcanvas offcanvas-end" tabindex="-1" id="accountMenuCanvas" aria-labelledby="accountMenuCanvasLabel" data-bs-scroll="true" data-bs-backdrop="false">
    <div class="offcanvas-header">
        <h5 class="offcanvas-title fw-bold" id="accountMenuCanvasLabel">TÃ i khoáº£n</h5>
        <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>
    <div class="offcanvas-body p-3">
        <div id="accountMenuHostMobile" class="account-dropdown">
            <div class="account-menu-shell">
        <% if (HeaderIsLoggedIn()) { %>
                <div class="account-menu-body">
                    <div class="card border-0 bg-light mb-3" style="border-radius:14px;overflow:hidden;">
                        <div class="p-3 d-flex align-items-center gap-3">
                            <span class="avatar avatar-lg rounded-circle home-dropdown-avatar" style="background-image:url('<%= HeaderAccountAvatarUrl() %>')"></span>
                            <div class="flex-fill">
                                <div class="fw-bold" style="font-size:1.05rem;"><%= HeaderProfileName() %></div>
                                <div class="text-secondary small"><%= _currentAccount %></div>
                                <div class="mt-1"><%= HeaderRoleLabel() %></div>
                            </div>
                            <a href="/home/edit-info.aspx" class="btn btn-sm btn-outline-secondary" style="border-radius:10px;">
                                <i class="ti ti-pencil"></i>
                            </a>
                        </div>
                        <div class="list-group list-group-flush bg-white" style="border-top:1px solid rgba(0,0,0,.06);">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/default.aspx?tk=<%= System.Web.HttpUtility.UrlEncode(_currentAccount) %>"><span class="d-flex align-items-center gap-2"><i class="ti ti-user-circle text-secondary"></i><span class="fw-medium">Trang cÃ¡ nhÃ¢n</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/tao-yeu-cau.aspx"><span class="d-flex align-items-center gap-2"><i class="ti ti-file-plus text-secondary"></i><span class="fw-medium">Táº¡o Ä‘á» xuáº¥t</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="javascript:void(0);" data-copy-ref="1" data-ref-link="<%= System.Web.HttpUtility.HtmlAttributeEncode(HeaderReferralLink()) %>" onclick="return copyReferralLink(this);"><span class="d-flex align-items-center gap-2"><i class="ti ti-link text-secondary"></i><span class="fw-medium">Sao chÃ©p Link giá»›i thiá»‡u</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/DoiPin.aspx"><span class="d-flex align-items-center gap-2"><i class="ti ti-key text-secondary"></i><span class="fw-medium">Äá»•i mÃ£ pin tháº»</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/DoiMatKhau.aspx"><span class="d-flex align-items-center gap-2"><i class="ti ti-lock text-secondary"></i><span class="fw-medium">Äá»•i máº­t kháº©u tÃ i khoáº£n</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/don-mua.aspx"><span class="d-flex align-items-center gap-2"><i class="ti ti-shopping-bag text-secondary"></i><span class="fw-medium">ÄÆ¡n mua</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/lich-hen.aspx"><span class="d-flex align-items-center gap-2"><i class="ti ti-calendar-event text-secondary"></i><span class="fw-medium">Lá»‹ch háº¹n cá»§a tÃ´i</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/lich-su-giao-dich.aspx"><span class="d-flex align-items-center gap-2"><i class="ti ti-clock text-secondary"></i><span class="fw-medium">Lá»‹ch sá»­ Trao Ä‘á»•i</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                        </div>
                    </div>

                    <div class="text-secondary small fw-semibold px-1 mb-2">Tiá»‡n Ã­ch</div>
                    <div class="card border-0 bg-light mb-3" style="border-radius:14px;">
                        <div class="list-group list-group-flush" style="border-radius:14px;overflow:hidden;">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/quan-ly-tin/tin-da-luu.aspx"><span class="d-flex align-items-center gap-2"><i class="ti ti-heart text-secondary"></i><span class="fw-medium">Tin Ä‘Äƒng Ä‘Ã£ lÆ°u</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/quan-ly-tin/lich-su-xem-tin.aspx"><span class="d-flex align-items-center gap-2"><i class="ti ti-history text-secondary"></i><span class="fw-medium">Lá»‹ch sá»­ xem tin</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/quan-ly-tin/danh-gia-tu-toi.aspx"><span class="d-flex align-items-center gap-2"><i class="ti ti-star text-secondary"></i><span class="fw-medium">ÄÃ¡nh giÃ¡ tá»« tÃ´i</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                        </div>
                    </div>

                    <div class="text-secondary small fw-semibold px-1 mb-2">Æ¯u Ä‘Ã£i, khuyáº¿n mÃ£i</div>
                    <div class="card border-0 bg-light mb-3" style="border-radius:14px;">
                        <div class="list-group list-group-flush" style="border-radius:14px;overflow:hidden;">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/lich-su-giao-dich.aspx"><span class="d-flex align-items-center gap-2"><i class="ti ti-coin text-secondary"></i><span class="fw-medium">Há»“ sÆ¡ quyá»n tiÃªu dÃ¹ng</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/lich-su-quyen-uu-dai.aspx"><span class="d-flex align-items-center gap-2"><i class="ti ti-ticket text-secondary"></i><span class="fw-medium">Há»“ sÆ¡ quyá»n Æ°u Ä‘Ã£i</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/lich-su-quyen-lao-dong.aspx"><span class="d-flex align-items-center gap-2"><i class="ti ti-briefcase text-secondary"></i><span class="fw-medium">Há»“ sÆ¡ hÃ nh vi lao Ä‘á»™ng</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/lich-su-quyen-gan-ket.aspx"><span class="d-flex align-items-center gap-2"><i class="ti ti-heart-handshake text-secondary"></i><span class="fw-medium">Há»“ sÆ¡ chá»‰ sá»‘ gáº¯n káº¿t</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                        </div>
                    </div>

                    <div class="text-secondary small fw-semibold px-1 mb-2">KhÃ¡c</div>
                    <div class="card border-0 bg-light mb-3" style="border-radius:14px;">
                        <div class="list-group list-group-flush" style="border-radius:14px;overflow:hidden;">
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/edit-info.aspx"><span class="d-flex align-items-center gap-2"><i class="ti ti-settings text-secondary"></i><span class="fw-medium">CÃ i Ä‘áº·t tÃ i khoáº£n</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="#"><span class="d-flex align-items-center gap-2"><i class="ti ti-help-circle text-secondary"></i><span class="fw-medium">Trá»£ giÃºp</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                            <a class="list-group-item d-flex align-items-center justify-content-between py-3" href="/home/dong-gop-y-kien.aspx"><span class="d-flex align-items-center gap-2"><i class="ti ti-message-report text-secondary"></i><span class="fw-medium">ÄÃ³ng gÃ³p Ã½ kiáº¿n</span></span><i class="ti ti-chevron-right text-secondary"></i></a>
                        </div>
                    </div>
                </div>
                <div class="account-logout-sticky">
                    <a href="/home/logout.aspx?return_url=<%= HeaderReturnUrl() %>" class="account-logout-btn">
                        <i class="ti ti-logout"></i><span>ÄÄƒng xuáº¥t</span>
                    </a>
                </div>
        <% } else { %>
                <div class="account-menu-body">
                    <div class="card border-0 shadow-sm mb-3" style="border-radius:14px;">
                        <div class="card-body p-3">
                            <div class="fw-bold" style="font-size:1.05rem;">Mua thÃ¬ há»i, bÃ¡n thÃ¬ lá»i.</div>
                            <div class="text-secondary">ÄÄƒng nháº­p cÃ¡i Ä‘Ã£!</div>
                            <div class="row g-2 mt-3">
                                <div class="col-6">
                                    <a href="/dang-nhap" class="btn btn-success w-100" style="border-radius:12px;">ÄÄƒng nháº­p</a>
                                </div>
                                <div class="col-6">
                                    <a href="/home/dangky.aspx" class="btn btn-outline-success w-100" style="border-radius:12px;">ÄÄƒng kÃ½</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
        <% } %>
        </div>
    </div>
</div>
</div>

<script>
    function copyReferralLink(el) {
        try {
            var link = el ? (el.getAttribute('data-ref-link') || '') : '';
            if (!link) return false;
            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(link);
            } else {
                var ta = document.createElement('textarea');
                ta.value = link;
                document.body.appendChild(ta);
                ta.select();
                document.execCommand('copy');
                document.body.removeChild(ta);
            }
            return false;
        } catch (e) {
            return false;
        }
    }
</script>

<div class="offcanvas offcanvas-start bds-topbar-canvas" tabindex="-1" id="bdsMobileMenuCanvas" aria-labelledby="bdsMobileMenuCanvasLabel" data-bs-scroll="true" data-bs-backdrop="false">
    <div class="offcanvas-header">
        <h5 class="offcanvas-title fw-bold" id="bdsMobileMenuCanvasLabel">Danh má»¥c báº¥t Ä‘á»™ng sáº£n</h5>
        <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>
    <div class="offcanvas-body p-2">
        <div class="small text-uppercase fw-bold text-secondary px-3 py-2">Chuyá»ƒn khÃ´ng gian</div>
        <div class="list-group list-group-flush mb-2">
            <a href="/home/default.aspx" class="<%= SpaceLinkCss("home") %>">Trang chá»§ Aha Sale</a>
            <a href="/gianhang/default.aspx" class="<%= SpaceLinkCss("gianhang") %>">Gian hÃ ng Aha Shop</a>
            <a href="/bat-dong-san" class="<%= SpaceLinkCss("bds") %>">Báº¥t Ä‘á»™ng sáº£n Aha Land</a>
            <a href="/gianhang/admin/" class="<%= SpaceLinkCss("ahashine") %>">CÃ´ng cá»¥ quáº£n trá»‹ váº­n hÃ nh Aha Shine</a>
        </div>
        <div class="small text-uppercase fw-bold text-secondary px-3 py-2">Danh má»¥c báº¥t Ä‘á»™ng sáº£n</div>
        <div class="list-group list-group-flush">
            <a href="/bat-dong-san" class="list-group-item list-group-item-action">Tá»•ng quan</a>
            <a href="/bat-dong-san/mua-ban.aspx" class="list-group-item list-group-item-action">Mua bÃ¡n</a>
            <a href="/bat-dong-san/cho-thue.aspx" class="list-group-item list-group-item-action">Cho thuÃª</a>
            <a href="/bat-dong-san/du-an.aspx" class="list-group-item list-group-item-action">Dá»± Ã¡n</a>
            <a href="/bat-dong-san/tham-khao-gia.aspx" class="list-group-item list-group-item-action">Tham kháº£o giÃ¡</a>
            <a href="/bat-dong-san/vay-mua-nha.aspx" class="list-group-item list-group-item-action">Vay mua nhÃ </a>
            <a href="/bat-dong-san/kinh-nghiem.aspx" class="list-group-item list-group-item-action">Kinh nghiá»‡m</a>
            <% if (!HeaderIsLoggedIn()) { %>
            <a href="/dang-nhap?return_url=<%= HeaderReturnUrl() %>" class="list-group-item list-group-item-action text-success">ÄÄƒng nháº­p</a>
            <% } %>
        </div>
    </div>
</div>

<div class="offcanvas offcanvas-end bds-topbar-canvas" tabindex="-1" id="bdsNotifCanvas" aria-labelledby="bdsNotifLabel" data-bs-scroll="true" data-bs-backdrop="false">
    <div class="offcanvas-header">
        <h5 class="offcanvas-title fw-bold" id="bdsNotifLabel">ThÃ´ng bÃ¡o</h5>
        <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>
    <div class="offcanvas-body p-3">
        <% if (HeaderIsLoggedIn()) { %>
        <div class="small text-secondary mb-2">Báº¡n cÃ³ <strong><%= HeaderUnreadBadgeText() %></strong> thÃ´ng bÃ¡o chÆ°a Ä‘á»c.</div>
        <a href="/home/quan-ly-thong-bao/default.aspx" class="btn btn-outline-primary btn-sm">Xem táº¥t cáº£ thÃ´ng bÃ¡o</a>
        <% } else { %>
        <div class="small text-secondary mb-2">ÄÄƒng nháº­p Ä‘á»ƒ xem thÃ´ng bÃ¡o má»›i nháº¥t.</div>
        <a href="/dang-nhap?return_url=<%= HeaderReturnUrl() %>" class="btn btn-success btn-sm">ÄÄƒng nháº­p</a>
        <% } %>
    </div>
</div>

<% } %>

<% if (ShowBanner) { %>
<section class="bds-global-banner-wrap">
    <div class="bds-global-banner card border-0">
        <img src="<%= BannerImageUrl() %>" alt="Banner báº¥t Ä‘á»™ng sáº£n Aha Sale" class="bds-global-banner-image" />
    </div>
</section>
<% } %>

<% if (ShowNav) { %>
<div class="bds-market-nav">
    <a class="btn btn-sm <%= NavClass("overview", "btn-success", "btn-outline-success") %>" href="/bat-dong-san">Tá»•ng quan</a>
    <a class="btn btn-sm <%= NavClass("sale", "btn-success", "btn-outline-success") %>" href="/bat-dong-san/mua-ban.aspx">Mua bÃ¡n</a>
    <a class="btn btn-sm <%= NavClass("rent", "btn-azure", "btn-outline-azure") %>" href="/bat-dong-san/cho-thue.aspx">Cho thuÃª</a>
    <a class="btn btn-sm <%= NavClass("project", "btn-secondary", "btn-outline-secondary") %>" href="/bat-dong-san/du-an.aspx">Dá»± Ã¡n</a>
    <a class="btn btn-sm <%= NavClass("price", "btn-secondary", "btn-outline-secondary") %>" href="/bat-dong-san/tham-khao-gia.aspx">Tham kháº£o giÃ¡</a>
    <a class="btn btn-sm <%= NavClass("loan", "btn-secondary", "btn-outline-secondary") %>" href="/bat-dong-san/vay-mua-nha.aspx">Vay mua nhÃ </a>
    <a class="btn btn-sm <%= NavClass("guide", "btn-secondary", "btn-outline-secondary") %>" href="/bat-dong-san/kinh-nghiem.aspx">Kinh nghiá»‡m</a>
</div>
<% } %>
