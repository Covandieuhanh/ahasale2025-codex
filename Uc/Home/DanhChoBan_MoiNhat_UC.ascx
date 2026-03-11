<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DanhChoBan_MoiNhat_UC.ascx.cs" Inherits="Uc_Home_DanhChoBan_MoiNhat_UC" %>

<style>
    /* ===== Grid 5 (desktop), 3 (md), 1 (mobile) ===== */
    .grid-5 {
        display: flex;
        flex-wrap: wrap;
        gap: 16px;
    }

    .grid-5-item {
        width: 100%;
    }

    @media (max-width: 767.98px) {
        .grid-5-item {
            width: 100%;
        }
    }

    @media (min-width: 768px) and (max-width: 991.98px) {
        .grid-5-item {
            width: calc((100% - 16px*2) / 3);
        }
    }

    @media (min-width: 992px) {
        .grid-5-item {
            width: calc((100% - 16px*4) / 5);
        }
    }

    .sp-card {
        border: none;
        border-radius: 12px;
        background: #fff;
        height: 100%;
        display: flex;
        flex-direction: column;
        box-shadow: 0 2px 14px rgba(0,0,0,.06);
        transition: box-shadow .2s ease;
        overflow: visible;
    }

        .sp-card:hover {
            box-shadow: 0 8px 24px rgba(0,0,0,.10);
        }

    .sp-body {
        padding: 12px;
        display: flex;
        flex-direction: column;
        height: 100%;
    }

    .thumb-wrap {
        border-radius: 10px;
        overflow: hidden;
        background: #f6f8fb;
    }

    .sp-thumb {
        width: 100%;
        aspect-ratio: 1 / 1;
        object-fit: cover;
        display: block;
        transition: transform .25s ease;
    }
    .sp-thumb-video {
        width: 100%;
        aspect-ratio: 1 / 1;
        object-fit: cover;
        display: block;
        background: #000;
        transition: transform .25s ease;
        pointer-events: none;
    }

    .sp-card:hover .sp-thumb {
        transform: scale(1.04);
    }

    .sp-title {
        margin-top: 10px;
        font-weight: 600;
        color: #1d273b;
        line-height: 1.25rem;
        min-height: 2.5rem;
        display: -webkit-box;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
        overflow: hidden;
        transition: color .2s ease;
    }

    .sp-card:hover .sp-title {
        color: #2fb344;
    }

    .sp-desc {
        margin-top: 6px;
        color: #626976;
        font-size: .875rem;
        line-height: 1.15rem;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    }

    .sp-price {
        margin-top: 6px;
        color: #d63939;
        font-weight: 700;
    }

    .sp-meta {
        margin-top: 8px;
        font-size: .875rem;
        color: #626976;
    }

        .sp-meta .rowline {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 8px;
        }

    .sp-loc {
        display: flex;
        align-items: center;
        gap: 6px;
        min-width: 0;
        color: #626976;
        font-weight: 400;
    }

        .sp-loc .loc-text {
            min-width: 0;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

    .thumb-wrap {
        position: relative;
    }
    
    .heart-icon {
        position: absolute;
        top: 8px;
        right: 8px;
        font-size: 22px;
        cursor: pointer;
        color: #adb5bd;          
        background: #fff;
        border-radius: 50%;
        padding: 6px;
    }

    .heart-icon.active {
        color: #dc3545;          
    }

    .heart-btn {
        text-decoration: none;
        padding: 0;
        border: 0;
        background: transparent;
    }

    .heart-btn:focus,
    .heart-btn:hover {
        text-decoration: none;
    }

    .icon-18 {
        width: 18px;
        height: 18px;
        flex: 0 0 18px;
    }

    .sp-time {
        display: flex;
        align-items: center;
        gap: 6px;
        color: #626976;
        font-weight: 400;
        white-space: nowrap;
    }

    /* ===== Dropdown 3 chấm ===== */
    .kebab-wrap {
        position: relative;
        z-index: 60;
    }

    .kebab-btn {
        border: none;
        background: transparent;
        padding: 4px 6px;
        border-radius: 8px;
        cursor: pointer;
    }

        .kebab-btn:hover {
            background: rgba(98,105,118,.10);
        }

    .kebab-menu {
        position: absolute;
        right: 0;
        top: calc(100% + 6px);
        min-width: 220px;
        background: #fff;
        border: 1px solid rgba(98,105,118,.18);
        border-radius: 12px;
        box-shadow: 0 8px 24px rgba(0,0,0,.12);
        padding: 6px;
        display: none;
        z-index: 9999;
    }

        .kebab-menu.show {
            display: block;
        }

    .kebab-item {
        display: flex;
        align-items: center;
        gap: 10px;
        padding: 10px 12px;
        border-radius: 10px;
        color: #1d273b;
        text-decoration: none;
        font-size: 14px;
        cursor: pointer;
        user-select: none;
    }

        .kebab-item:hover {
            background: rgba(98,105,118,.10);
        }

    .sp-actions {
        margin-top: 12px;
        display: grid;
        grid-template-columns: 1fr;
        gap: 8px;
    }

    .sp-actions .btn {
        width: 100%;
        min-height: 34px;
        padding: 6px 10px;
        font-size: 13px;
            display: flex;
            align-items: center;
            justify-content: center;
        white-space: nowrap;
    }

    .sp-actions .btn-outline-info {
        background: #0f8f5d;
        border-color: #0f8f5d;
        color: #fff;
    }

    .sp-actions .btn-outline-info:hover,
    .sp-actions .btn-outline-info:focus {
        background: #0d7b50;
        border-color: #0d7b50;
        color: #fff;
    }

    .sp-actions .btn-outline-success {
        background: #eef6ff;
        border-color: #bfd7f5;
        color: #0f4c81;
    }

    .sp-actions .btn-outline-success:hover,
    .sp-actions .btn-outline-success:focus {
        background: #e1efff;
        border-color: #a9c8ee;
        color: #0b3f66;
    }

    .sp-card a, .sp-card a:hover {
        color: inherit;
        text-decoration: none;
    }

    /* ===== Modal z-index cao hơn topbar ===== */
    .modal {
        z-index: 2000;
    }

    .modal-backdrop {
        z-index: 1990;
    }

    /* Nút X giống trang Thưởng */
    #modalDatHang .modal-header,
    #modalAddCart .modal-header {
        position: relative;
        display: flex;
        align-items: center;
        padding-right: 52px;
    }

    .modal-xbtn-abs {
        position: absolute;
        right: 12px;
        top: 10px;
        border: 0;
        background: transparent;
        width: 34px;
        height: 34px;
        border-radius: 10px;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 0;
        cursor: pointer;
    }

    .modal-xbtn-abs:hover {
        background: rgba(98,105,118,.10);
    }

    .modal-xbtn-abs span {
        font-size: 22px;
        line-height: 1;
        color: #98a2b3;
    }

    .ct-guest-auth-wrap {
        display: none;
    }

    :root {
        --aha-home-root-search-height: 88px;
    }

    body.aha-home-root-pinned-search .page-wrapper {
        padding-top: var(--aha-home-root-search-height) !important;
    }

    .search-wrap.is-root-pinned.is-sticky-fixed {
        z-index: 1040;
        padding: 6px 0 8px;
        background: linear-gradient(180deg, rgba(248, 251, 255, .98) 0%, rgba(248, 251, 255, .90) 100%);
        border-bottom: 1px solid #dce8f4;
    }

    html[data-bs-theme="dark"] .sp-card {
        background: #0f172a;
        box-shadow: 0 10px 26px rgba(0,0,0,.4);
    }

    html[data-bs-theme="dark"] .thumb-wrap {
        background: #111d34;
    }

    html[data-bs-theme="dark"] .sp-title {
        color: #e5e7eb;
    }

    html[data-bs-theme="dark"] .sp-card:hover .sp-title {
        color: #4ade80;
    }

    html[data-bs-theme="dark"] .sp-desc,
    html[data-bs-theme="dark"] .sp-meta,
    html[data-bs-theme="dark"] .sp-loc,
    html[data-bs-theme="dark"] .sp-time {
        color: #94a3b8;
    }

    html[data-bs-theme="dark"] .heart-icon {
        background: #0f172a;
        border: 1px solid #223246;
        color: #94a3b8;
    }

    html[data-bs-theme="dark"] .kebab-btn:hover {
        background: rgba(148,163,184,.12);
    }

    html[data-bs-theme="dark"] .kebab-menu {
        background: #0f172a;
        border-color: #223246;
        box-shadow: 0 10px 26px rgba(0,0,0,.45);
    }

    html[data-bs-theme="dark"] .kebab-item {
        color: #e5e7eb;
    }

    html[data-bs-theme="dark"] .kebab-item:hover {
        background: rgba(148,163,184,.12);
    }

    html[data-bs-theme="dark"] .sp-actions .btn-outline-success {
        background: #1f2a3a;
        border-color: #334155;
        color: #e5e7eb;
    }

    html[data-bs-theme="dark"] .sp-actions .btn-outline-success:hover,
    html[data-bs-theme="dark"] .sp-actions .btn-outline-success:focus {
        background: #273449;
        border-color: #3b4a61;
        color: #ffffff;
    }

    html[data-bs-theme="dark"] .search-wrap.is-root-pinned.is-sticky-fixed {
        background: linear-gradient(180deg, rgba(15, 23, 42, .98) 0%, rgba(15, 23, 42, .90) 100%);
        border-bottom: 1px solid #223246;
    }

    html[data-bs-theme="dark"] .ct-guest-auth-btn.ct-login {
        background: #0f172a;
        border-color: #223246;
        color: #ffffff !important;
    }

    html[data-bs-theme="dark"] .ct-guest-auth-btn.ct-register {
        background: rgba(34, 197, 94, 0.16);
        border-color: rgba(34, 197, 94, 0.35);
        color: #86efac !important;
    }

    .search-wrap.is-root-pinned.is-sticky-fixed .ct-shell {
        padding: 0 12px;
    }

    @media (max-width: 991.98px) {
        :root {
            --aha-home-root-search-height: 98px;
        }

        .search-wrap.is-root-pinned.is-sticky-fixed .ct-shell {
            padding: 0 10px;
        }

        .search-wrap.is-root-pinned.is-sticky-fixed .ct-guest-auth-wrap {
            margin-bottom: 6px;
            padding-top: 1px;
        }

        .search-wrap .ct-guest-auth-wrap {
            display: flex;
            align-items: center;
            justify-content: flex-end;
            gap: 8px;
            max-width: 1000px;
            margin: 0 auto 8px;
            padding: 0 4px;
        }

        .search-wrap.is-sticky-fixed .ct-guest-auth-wrap {
            margin-bottom: 6px;
            padding-top: 2px;
        }

        .ct-guest-auth-btn {
            min-height: 36px;
            padding: 0 14px;
            border-radius: 999px;
            font-size: 13px;
            font-weight: 700;
            text-decoration: none !important;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            white-space: nowrap;
        }

        .ct-guest-auth-btn.ct-login {
            background: #0f172a;
            border: 1px solid #0f172a;
            color: #ffffff !important;
        }

        .ct-guest-auth-btn.ct-register {
            background: #e9f7ef;
            border: 1px solid rgba(25, 135, 84, .45);
            color: #146c43 !important;
        }

        .search-wrap.is-compact .ct-guest-auth-wrap {
            margin-bottom: 4px;
            padding-top: 0;
            gap: 6px;
        }

        .search-wrap.is-root-pinned.is-compact {
            padding: 4px 0 6px;
        }

        .search-wrap.is-compact .ct-guest-auth-btn {
            min-height: 32px;
            padding: 0 12px;
            font-size: 12px;
        }

        .search-wrap.is-compact .ct-bar {
            height: 54px !important;
            padding: 5px !important;
            border-radius: 16px !important;
        }

        .search-wrap.is-compact .ct-cat,
        .search-wrap.is-compact .ct-q,
        .search-wrap.is-compact .ct-bar .choices__inner,
        .search-wrap.is-compact .ct-input,
        .search-wrap.is-compact .ct-btn-icon {
            height: 40px !important;
            min-height: 40px !important;
        }

        .search-wrap.is-compact .ct-input {
            font-size: 13px !important;
        }
    }
</style>

<asp:UpdatePanel ID="up_all" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <section class="hero-ct-like">
            <div class="hero-ct-inner">
                <h1 class="hero-ct-title">Giá Sale, gần bạn!</h1>
            </div>
        </section>

        <asp:Panel ID="pnlSearch" runat="server" DefaultButton="btn_Search">
            <section class="search-wrap">
                <asp:PlaceHolder ID="phGuestMobileQuickAuth" runat="server" Visible="false">
                    <div class="ct-guest-auth-wrap">
                        <a href="/dang-nhap" class="ct-guest-auth-btn ct-login">Đăng nhập</a>
                        <a href="/home/dangky.aspx" class="ct-guest-auth-btn ct-register">Đăng ký</a>
                    </div>
                </asp:PlaceHolder>
                <div class="ct-shell">
                    <div class="ct-bar">

                        <div class="ct-cell ct-cat">
                            <asp:DropDownList ID="ddl_Category" runat="server" CssClass="ct-select js-choices" />
                        </div>

                        <div class="ct-vline"></div>

                        <div class="ct-cell ct-q">
                            <span class="input-icon"><i class="ti ti-search"></i></span>
                            <asp:TextBox ID="txt_Search" runat="server"
                                CssClass="form-control ct-input"
                                placeholder="Tìm sản phẩm..." />
                        </div>

                        <div class="ct-cell ct-loc">
                            <i class="ti ti-map-pin ct-loc-icon"></i>
                            <asp:DropDownList ID="ddl_Location" runat="server" CssClass="ct-select js-choices" data-choices-search="1" data-choices-placeholder="Tìm tỉnh thành" />
                        </div>

                        <asp:LinkButton ID="btn_Search" runat="server"
                            CssClass="btn btn-success ct-btn"
                            OnClick="Loc_Click">
                    Tìm kiếm
                        </asp:LinkButton>

                    </div>
                </div>
            </section>
        </asp:Panel>


        <script>
            // Init Choices (safe for WebForms partial postback)
            function initChoices() {
                document.querySelectorAll('.js-choices').forEach(function (el) {
                    if (el.tagName !== 'SELECT') return;

                    if (el.dataset.choicesDone) return;
                    var enableSearch = (el.dataset.choicesSearch === '1');
                    var searchPlaceholder = el.dataset.choicesPlaceholder || 'Tìm kiếm...';
                    new Choices(el, {
                        searchEnabled: enableSearch,
                        searchPlaceholderValue: enableSearch ? searchPlaceholder : null,
                        shouldSort: false,
                        itemSelectText: '',
                        position: 'bottom'
                    });
                    el.dataset.choicesDone = "1";
                });
            }

            function ensureHomeSearchSticky() {
                if (window.__ahaHomeSearchStickyBound) {
                    return;
                }
                window.__ahaHomeSearchStickyBound = true;

                var ticking = false;
                var wrap = document.querySelector('.search-wrap');
                if (!wrap) return;
                var body = document.body;
                var html = document.documentElement;

                var spacer = document.createElement('div');
                spacer.className = 'search-wrap-spacer';
                wrap.parentNode.insertBefore(spacer, wrap.nextSibling);

                function setPinnedLayoutClass(enabled, wrapHeight) {
                    if (!body || !html) return;
                    body.classList.toggle('aha-home-root-pinned-search', !!enabled);
                    if (enabled && wrapHeight > 0) {
                        html.style.setProperty('--aha-home-root-search-height', (wrapHeight + 12) + 'px');
                    } else {
                        html.style.removeProperty('--aha-home-root-search-height');
                    }
                }

                function updateStickyState() {
                    ticking = false;
                    var header = document.querySelector('.site-header');
                    var headerHeight = header ? Math.round(header.getBoundingClientRect().height) : 56;
                    var stickyTop = Math.max(48, headerHeight + 6);
                    document.documentElement.style.setProperty('--aha-home-sticky-top', stickyTop + 'px');

                    var path = (window.location.pathname || '').toLowerCase();
                    var isRootHomePath = (path === '/' || path === '/default.aspx');
                    var isMobile = window.matchMedia('(max-width: 991.98px)').matches;
                    var forcePinnedAtTop = isRootHomePath;

                    var isFixed = wrap.classList.contains('is-sticky-fixed');
                    var triggerTop = isFixed
                        ? spacer.getBoundingClientRect().top
                        : wrap.getBoundingClientRect().top;
                    var shouldStick = forcePinnedAtTop || (triggerTop <= stickyTop);
                    var keepSpacer = shouldStick && !forcePinnedAtTop;
                    spacer.style.height = keepSpacer ? (wrap.offsetHeight + 'px') : '0px';

                    wrap.classList.toggle('is-sticky-fixed', shouldStick);
                    wrap.classList.toggle('is-sticky', shouldStick);
                    wrap.classList.toggle('is-root-pinned', forcePinnedAtTop && shouldStick);
                    wrap.classList.toggle('is-compact', shouldStick && isMobile && (window.scrollY > (forcePinnedAtTop ? 32 : 8)));
                    setPinnedLayoutClass(forcePinnedAtTop && shouldStick, wrap.offsetHeight);
                }

                function queueUpdate() {
                    if (ticking) return;
                    ticking = true;
                    window.requestAnimationFrame(updateStickyState);
                }

                window.addEventListener('scroll', queueUpdate, { passive: true });
                document.addEventListener('scroll', queueUpdate, true);
                window.addEventListener('resize', queueUpdate);
                document.addEventListener('visibilitychange', queueUpdate);
                queueUpdate();

                window.__ahaHomeSearchStickyRefresh = function () {
                    queueUpdate();
                };
            }

            document.addEventListener('DOMContentLoaded', initChoices);
            document.addEventListener('DOMContentLoaded', ensureHomeSearchSticky);

            if (window.Sys && Sys.WebForms) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    initChoices();
                    if (window.__ahaHomeSearchStickyRefresh) {
                        window.__ahaHomeSearchStickyRefresh();
                    } else {
                        ensureHomeSearchSticky();
                    }
                });
            }
        </script>

        <!-- ============ MODAL ĐẶT HÀNG ============ -->
        <div class="modal fade" id="modalDatHang"
            data-bs-backdrop="true" data-bs-keyboard="true"
            data-backdrop="true" data-keyboard="true"
            tabindex="-1" aria-hidden="true">

            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">

                    <asp:Panel ID="pnlModalDatHang" runat="server" DefaultButton="but_dathang">
                        <asp:UpdatePanel ID="up_dathang" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="txt_soluong2" EventName="TextChanged" />
                                <asp:AsyncPostBackTrigger ControlID="but_dathang" EventName="Click" />
                            </Triggers>

                            <ContentTemplate>

                                <div class="modal-header">
                                    <h5 class="modal-title fw-bold">Đặt hàng</h5>

                                    <button type="button"
                                        class="modal-xbtn-abs"
                                        aria-label="Đóng"
                                        onclick="ModalHelper.hide('modalDatHang')"
                                        data-bs-dismiss="modal" data-dismiss="modal">
                                        <span>&times;</span>
                                    </button>
                                </div>

                                <div class="modal-body">
                                    <div class="mb-2">
                                        <div class="text-muted small">Sản phẩm</div>
                                        <div class="fw-bold">
                                            <asp:Literal ID="Literal9" runat="server"></asp:Literal>
                                        </div>
                                    </div>

                                    <div class="row g-2 mb-3">
                                        <div class="col-6">
                                            <div class="text-muted small">Giá</div>
                                            <div class="fw-bold">
                                                <asp:Literal ID="Literal10" runat="server"></asp:Literal>
                                                đ
                                            </div>
                                        </div>
                                        <div class="col-6">
                                            <div class="text-muted small">Điểm cần trao đổi</div>
                                            <div class="fw-bold text-success">
                                                <asp:Literal ID="Literal11" runat="server"></asp:Literal>
                                                đ
                                            </div>
                                        </div>
                                    </div>
                                    <div class="mb-2">
  <div class="text-muted small">Ưu đãi</div>
  <div class="fw-bold text-warning">
    <asp:Literal ID="LiteralUuDaiPercent" runat="server"></asp:Literal>% 
  </div>
</div>


                                    <div class="row g-2">
                                        <div class="col-12">
                                            <label class="form-label">Số lượng</label>
                                            <asp:TextBox ID="txt_soluong2" runat="server"
                                                CssClass="form-control"
                                                Text="1"
                                                AutoPostBack="true"
                                                OnTextChanged="txt_soluong2_TextChanged"
                                                MaxLength="6" />
                                        </div>

                                        <div class="col-12">
                                            <label class="form-label">Họ tên người nhận</label>
                                            <asp:TextBox ID="txt_hoten_nguoinhan" runat="server" CssClass="form-control" MaxLength="100" />
                                        </div>
                                        <div class="col-12">
                                            <label class="form-label">SĐT người nhận</label>
                                            <asp:TextBox ID="txt_sdt_nguoinhan" runat="server" CssClass="form-control" MaxLength="20" />
                                        </div>
                                        <div class="col-12">
                                            <label class="form-label">Địa chỉ người nhận</label>
                                            <asp:TextBox ID="txt_diachi_nguoinhan" runat="server" CssClass="form-control" MaxLength="200" />
                                        </div>
                                    </div>
                                </div>

                                <div class="modal-footer d-flex align-items-center justify-content-between">
                                    <button type="button"
                                        class="btn btn-link text-decoration-none"
                                        onclick="ModalHelper.hide('modalDatHang')"
                                        data-bs-dismiss="modal" data-dismiss="modal">
                                        Hủy
                                    </button>

                                    <asp:Button ID="but_dathang" runat="server"
    CssClass="btn btn-success px-4"
    Text="Xác nhận đặt hàng"
    OnClick="but_dathang_Click"
    OnClientClick="return preventDoubleClick(this);" />

                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>

                </div>
            </div>
        </div>

        <!-- ============ MODAL THÊM GIỎ ============ -->
        <div class="modal fade" id="modalAddCart"
            data-bs-backdrop="true" data-bs-keyboard="true"
            data-backdrop="true" data-keyboard="true"
            tabindex="-1" aria-hidden="true">

            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">

                    <asp:Panel ID="pnlModalAddCart" runat="server" DefaultButton="but_add_cart">
                        <asp:UpdatePanel ID="up_add_cart" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="but_add_cart" EventName="Click" />
                            </Triggers>

                            <ContentTemplate>

                                <div class="modal-header">
                                    <h5 class="modal-title fw-bold">Thêm vào giỏ hàng</h5>

                                    <button type="button"
                                        class="modal-xbtn-abs"
                                        aria-label="Đóng"
                                        onclick="ModalHelper.hide('modalAddCart')"
                                        data-bs-dismiss="modal" data-dismiss="modal">
                                        <span>&times;</span>
                                    </button>
                                </div>

                                <div class="modal-body">
                                    <div class="mb-2">
                                        <div class="text-muted small">Sản phẩm</div>
                                        <div class="fw-bold">
                                            <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                                        </div>
                                    </div>

                                    <div class="mb-2">
                                        <label class="form-label">Số lượng</label>
                                        <asp:TextBox ID="txt_soluong1" runat="server" CssClass="form-control" Text="1" MaxLength="6" />
                                    </div>
                                </div>

                                <div class="modal-footer d-flex align-items-center justify-content-between">
                                    <button type="button"
                                        class="btn btn-link text-decoration-none"
                                        onclick="ModalHelper.hide('modalAddCart')"
                                        data-bs-dismiss="modal" data-dismiss="modal">
                                        Hủy
                                    </button>

                                    <asp:Button ID="but_add_cart" runat="server"
                                        CssClass="btn btn-success px-4"
                                        Text="Thêm vào giỏ"
                                        OnClick="but_add_cart_Click" />
                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>

                </div>
            </div>
        </div>

        <!-- ============ LIST ============ -->
        <div class="container-xl mt-5">
            <div class="card">
                <div class="card-header">
                    <div class="card-title">
                        <asp:Label ID="lblTitle" runat="server" Text="Mới nhất"></asp:Label>
                    </div>

                </div>
                <div class="card-body">
                    <div class="grid-5">
                        <asp:Repeater ID="RepeaterTin" runat="server" OnItemDataBound="RepeaterTin_ItemDataBound">
                            <ItemTemplate>
                                <div class="grid-5-item">
                                    <div class="sp-card">
                                        <div class="sp-body">
                                            <div class="thumb-wrap position-relative">
                                                <a href="/<%# Eval("name_en") %>-<%# Eval("id") %>.html" class="text-decoration-none">
                                                    <img class="sp-thumb" src="<%# string.IsNullOrWhiteSpace((Eval("image") + "").Trim()) ? "/uploads/images/macdinh.jpg" : (Eval("image") + "").Trim() %>" alt="<%# Eval("name") %>" />
                                                </a>
                                             <asp:UpdatePanel ID="upHeart" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:LinkButton ID="btnHeart"
                                                        runat="server"
                                                        CssClass="heart-btn"
                                                        CommandArgument='<%# Eval("id") %>'
                                                        OnClick="btnHeart_Click">
                                                        <i class='<%# (bool)Eval("IsTheoDoi")
                                                            ? "ti ti-heart-filled heart-icon active"
                                                            : "ti ti-heart heart-icon" %>'></i>
                                                    </asp:LinkButton>

                                                </ContentTemplate>

                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="btnHeart" EventName="Click" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                            </div>
                                            <a href="/<%# Eval("name_en") %>-<%# Eval("id") %>.html" class="text-decoration-none">
                                                <div class="sp-title"><%# Eval("name") %></div>
                                            </a>
                                            <div class="sp-desc"><%# Eval("description") %></div>
                                            <div class="sp-price"><%# Eval("giaban", "{0:#,##0}") %> đ</div>

                                            <div class="sp-meta">
                                                <div class="sp-time text-muted">
                                                    <svg class="icon-18" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"
                                                        fill="none" stroke="currentColor" stroke-width="2"
                                                        stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                                                        <circle cx="12" cy="12" r="9"></circle>
                                                        <path d="M12 7v5l3 3"></path>
                                                    </svg>
                                                    <asp:Literal ID="lit_timeago" runat="server"></asp:Literal>
                                                </div>

                                                <div class="rowline mt-1">
                                                    <div class="sp-loc text-muted">
                                                        <svg class="icon-18" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"
                                                            fill="none" stroke="currentColor" stroke-width="2"
                                                            stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                                                            <path d="M12 21s-6-5.686-6-10a6 6 0 1 1 12 0c0 4.314-6 10-6 10z"></path>
                                                            <circle cx="12" cy="11" r="2"></circle>
                                                        </svg>
                                                        <span class="loc-text text-muted"><%# Eval("ThanhPhoDisplay") %></span>
                                                    </div>

                                                    <div class="kebab-wrap">
                                                        <button type="button" class="kebab-btn js-kebab-toggle" aria-label="menu">
                                                            <svg class="icon-18" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"
                                                                fill="currentColor" aria-hidden="true">
                                                                <circle cx="12" cy="5" r="2"></circle>
                                                                <circle cx="12" cy="12" r="2"></circle>
                                                                <circle cx="12" cy="19" r="2"></circle>
                                                            </svg>
                                                        </button>
                                                        <div class="kebab-menu">
                                                            <asp:LinkButton ID="btn1" runat="server"
                                                                CssClass="kebab-item"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                CommandName="khongdungnhucau"
                                                                OnClick="AddItemtoSession">
                                                                    <i class="ti ti-x"></i> Không đúng nhu cầu
                                                            </asp:LinkButton>

                                                            <asp:LinkButton ID="btn2" runat="server"
                                                                CssClass="kebab-item"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                CommandName="daxem"
                                                                OnClick="AddItemtoSession">
                                                                    <i class="ti ti-eye-off"></i> Tin đã xem rồi
                                                            </asp:LinkButton>

                                                            <asp:LinkButton ID="btn3" runat="server"
                                                                CssClass="kebab-item"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                CommandName="xakhuvuc"
                                                                OnClick="AddItemtoSession">
                                                                    <i class="ti ti-map-pin-off"></i> Xa khu vực đang sống
                                                            </asp:LinkButton>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <asp:PlaceHolder ID="ph_actions" runat="server">
                                                <div class="sp-actions">
                                                    <asp:Button ID="but_bansanphamnay" runat="server"
                                                        Text="Bán chéo"
                                                        CssClass="btn btn-sm btn-outline-warning"
                                                        OnClick="but_bansanphamnay_Click"
                                                        CommandArgument='<%# Eval("id") %>'
                                                        Visible="false" />

                                                    <asp:Button ID="but_traodoi" runat="server"
                                                        Text="Trao đổi"
                                                        CssClass="btn btn-sm btn-outline-info"
                                                        OnClick="but_traodoi_Click"
                                                        CommandArgument='<%# Eval("id") %>' />

                                                    <asp:Button ID="but_themvaogio" runat="server"
                                                        Text="Thêm vào giỏ hàng"
                                                        CssClass="btn btn-sm btn-outline-success"
                                                        OnClick="but_themvaogio_Click"
                                                        CommandArgument='<%# Eval("id") %>' />
                                                </div>
                                            </asp:PlaceHolder>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div class="mt-3 text-center">
                        <asp:Button ID="but_xemthem" runat="server"
                            CssClass="btn btn-outline-primary"
                            Text="Xem thêm"
                            OnClick="but_xemthem_Click" />
                    </div>

                </div>
            </div>
        </div>  
        <script>
            window.ModalHelper = (function () {

                function _el(id) { return document.getElementById(id); }

                function forceCleanup() {
                    document.querySelectorAll('.modal-backdrop').forEach(function (b) { b.remove(); });
                    document.body.classList.remove('modal-open');
                    document.body.style.paddingRight = '';
                    document.body.style.overflow = '';
                }

                function show(id) {
                    var el = _el(id); if (!el) return;

                    if (window.bootstrap && bootstrap.Modal) {
                        bootstrap.Modal.getOrCreateInstance(el, { backdrop: true, keyboard: true }).show();
                        return;
                    }

                    if (window.jQuery && typeof jQuery(el).modal === "function") {
                        jQuery(el).modal({ backdrop: true, keyboard: true, show: true });
                        return;
                    }

                    el.style.display = 'block';
                    el.classList.add('show');
                    el.removeAttribute('aria-hidden');
                    document.body.classList.add('modal-open');
                    document.body.style.overflow = 'hidden';

                    if (!document.querySelector('.modal-backdrop')) {
                        var bd = document.createElement('div');
                        bd.className = 'modal-backdrop fade show';
                        document.body.appendChild(bd);
                    }
                }

                function hide(id) {
                    var el = _el(id); if (!el) return;

                    if (window.bootstrap && bootstrap.Modal) {
                        try {
                            var ins = bootstrap.Modal.getOrCreateInstance(el);
                            ins.hide();
                        } catch (e) { }

                        forceCleanup();
                        setTimeout(forceCleanup, 50);
                        setTimeout(forceCleanup, 300);
                        return;
                    }

                    if (window.jQuery && typeof jQuery(el).modal === "function") {
                        try { jQuery(el).modal('hide'); } catch (e) { }

                        forceCleanup();
                        setTimeout(forceCleanup, 50);
                        setTimeout(forceCleanup, 300);
                        return;
                    }

                    el.classList.remove('show');
                    el.style.display = 'none';
                    el.setAttribute('aria-hidden', 'true');

                    forceCleanup();
                    setTimeout(forceCleanup, 50);
                    setTimeout(forceCleanup, 300);
                }

                function hookMsAjax() {
                    if (!window.Sys || !Sys.WebForms) return;
                    var prm = Sys.WebForms.PageRequestManager.getInstance();
                    prm.add_endRequest(function () {
                        if (!document.querySelector('.modal.show')) {
                            forceCleanup();
                            setTimeout(forceCleanup, 150);
                        }
                    });
                }

                hookMsAjax();
                return { show: show, hide: hide, cleanup: forceCleanup };
            })();
        </script>
        <script>
            (function () {
                function closeAllMenus(exceptMenu) {
                    document.querySelectorAll('.kebab-menu.show').forEach(function (m) {
                        if (exceptMenu && m === exceptMenu) return;
                        m.classList.remove('show');
                    });
                }

                document.addEventListener('click', function (e) {
                    if (!e.target.closest('.kebab-wrap')) closeAllMenus();
                });

                document.addEventListener('click', function (e) {
                    var btn = e.target.closest('.js-kebab-toggle');
                    if (!btn) return;

                    e.preventDefault();
                    e.stopPropagation();

                    var wrap = btn.closest('.kebab-wrap');
                    if (!wrap) return;

                    var menu = wrap.querySelector('.kebab-menu');
                    if (!menu) return;

                    var isOpen = menu.classList.contains('show');
                    closeAllMenus();
                    if (!isOpen) menu.classList.add('show');
                });
            })();
        </script>

    </ContentTemplate>
</asp:UpdatePanel>
