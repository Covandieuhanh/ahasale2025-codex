<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Footer_uc.ascx.cs" Inherits="Uc_Home_Footer_uc" %>

<style>
    .theme-toggle .theme-btn {
        width: 34px;
        height: 34px;
        border-radius: 50%;
        border: 1px solid #e6e8eb;
        background: #fff;
        color: #6c757d;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        padding: 0;
        transition: all .15s ease;
    }

    .theme-toggle .theme-btn i {
        font-size: 18px;
        line-height: 1;
    }

    .theme-toggle .theme-btn:hover {
        background: rgba(0,0,0,.04);
        color: #111;
    }

    .theme-toggle .theme-btn.active {
        background: var(--tblr-primary);
        border-color: var(--tblr-primary);
        color: #fff;
    }

    [data-bs-theme="dark"] .theme-toggle .theme-btn {
        background: transparent;
        border-color: rgba(255,255,255,.2);
        color: rgba(255,255,255,.7);
    }

    [data-bs-theme="dark"] .theme-toggle .theme-btn.active {
        background: #fff;
        color: #000;
    }

    .footer .footer-top {
        border-top: 0 !important;
    }

    .footer .contact-text,
    .footer ul li a {
        font-size: .875rem;
        line-height: 1.5;
    }

    .footer .fw-bold {
        font-size: 1rem;
    }

    .footer a.link-secondary {
        transition: color .15s ease;
    }

    .footer a.link-secondary:hover {
        color: var(--tblr-success) !important;
        text-decoration: none;
    }

    .footer .btn.btn-icon:hover {
        filter: brightness(1.1);
    }
</style>

<footer class="footer mt-5">

    <div class="footer-top">
        <div class="container py-5">
            <div class="row g-4 align-items-start">

                <div class="col-12 col-md-3">
                    <div class="fw-bold mb-3 mt-3">Giao diện</div>

                    <div class="d-flex align-items-center gap-2 theme-toggle">
                        <button type="button" class="theme-btn" data-theme="light" title="Giao diện sáng">
                            <i class="ti ti-sun"></i>
                        </button>
                        <button type="button" class="theme-btn" data-theme="dark" title="Giao diện tối">
                            <i class="ti ti-moon"></i>
                        </button>
                    </div>
                </div>

                <div class="col-12 col-md-3">
                    <div class="fw-bold mb-3">Hỗ trợ khách hàng</div>
                    <ul class="list-unstyled mb-0">
                        <asp:Repeater ID="rpt_support_links" runat="server">
                            <ItemTemplate>
                                <li class="mb-2">
                                    <a href="<%# Eval("Url") %>" class="link-secondary text-decoration-none"><%# Eval("DisplayName") %></a>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>

                <div class="col-12 col-md-3">
                    <div class="fw-bold mb-3">Về AhaSale</div>
                    <ul class="list-unstyled mb-0">
                        <asp:Repeater ID="rpt_about_links" runat="server">
                            <ItemTemplate>
                                <li class="mb-2">
                                    <a href="<%# Eval("Url") %>" class="link-secondary text-decoration-none"><%# Eval("DisplayName") %></a>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>

                <div class="col-12 col-md-3">
                    <div class="fw-bold mb-3">Liên kết</div>

                    <div class="d-flex gap-2 mb-3">
                        <asp:HyperLink ID="lnk_social_linkedin" runat="server" class="btn btn-icon" style="background: #0a66c2; color: #fff; border: 0" title="LinkedIn" Target="_blank">
                            <i class="ti ti-brand-linkedin"></i>
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnk_social_youtube" runat="server" class="btn btn-icon" style="background: #ff0000; color: #fff; border: 0" title="YouTube" Target="_blank">
                            <i class="ti ti-brand-youtube"></i>
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnk_social_facebook" runat="server" class="btn btn-icon" style="background: #1877f2; color: #fff; border: 0" title="Facebook" Target="_blank">
                            <i class="ti ti-brand-facebook"></i>
                        </asp:HyperLink>
                    </div>

                    <div class="text-secondary contact-text">
                        <div class="mb-2">
                            Email:
                            <asp:HyperLink ID="lnk_contact_email" runat="server" class="link-secondary text-decoration-none"></asp:HyperLink>
                        </div>
                        <div class="mb-2">CSKH: <asp:Literal ID="lit_contact_hotline" runat="server"></asp:Literal></div>
                        <div>Địa chỉ: <asp:Literal ID="lit_contact_address" runat="server"></asp:Literal></div>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <div class="border-top">
        <div class="container py-4">
            <div class="row align-items-center g-3">
                <div class="col-12 col-lg-9">
                    <div class="text-secondary small">
                        <asp:Literal ID="lit_footer_legal_line" runat="server"></asp:Literal><br />
                        <asp:HyperLink ID="lnk_policy_usage" runat="server" class="link-secondary text-success text-decoration-none"></asp:HyperLink>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        (function () {
            const ROOT = document.documentElement;
            const STORAGE_KEY = 'theme-preference';
            const buttons = document.querySelectorAll('[data-theme]');

            function applyTheme(theme) {
                localStorage.setItem(STORAGE_KEY, theme);
                ROOT.setAttribute('data-bs-theme', theme);
                buttons.forEach(function (b) { b.classList.toggle('active', b.dataset.theme === theme); });
            }

            const saved = localStorage.getItem(STORAGE_KEY);
            const theme = (saved === 'dark' || saved === 'light') ? saved : 'light';
            applyTheme(theme);

            buttons.forEach(function (btn) {
                btn.addEventListener('click', function () { applyTheme(btn.dataset.theme); });
            });
        })();
    </script>
</footer>
