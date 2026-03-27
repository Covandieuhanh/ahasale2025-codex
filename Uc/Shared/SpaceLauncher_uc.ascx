<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SpaceLauncher_uc.ascx.cs" Inherits="Uc_Shared_SpaceLauncher_uc" %>
<asp:PlaceHolder ID="phLauncher" runat="server" Visible="false">
    <style>
        .aha-space-launcher {
            position: relative;
            display: flex;
            align-items: center;
            flex: 0 0 auto;
        }

        .aha-space-launcher__toggle {
            position: relative;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            border: 0;
            background: transparent;
            color: inherit;
            cursor: pointer;
            transition: background-color .2s ease, color .2s ease, transform .2s ease;
        }

        .aha-space-launcher__toggle:hover {
            transform: translateY(-1px);
        }

        .aha-space-launcher__toggle:focus-visible,
        .aha-space-launcher__close:focus-visible {
            outline: 2px solid rgba(37, 99, 235, .45);
            outline-offset: 2px;
        }

        .aha-space-launcher__toggle-bars {
            width: 20px;
            height: 14px;
            display: inline-flex;
            flex-direction: column;
            justify-content: space-between;
        }

        .aha-space-launcher__toggle-bars span {
            display: block;
            width: 100%;
            height: 2px;
            border-radius: 999px;
            background: currentColor;
        }

        .aha-space-launcher__toggle.app-bar-item {
            min-width: 52px;
            height: 52px;
            padding: 0 14px;
        }

        .aha-space-launcher__toggle.storefront-icon-button {
            margin-right: 8px;
        }

        .aha-space-launcher__backdrop {
            position: fixed;
            inset: 0;
            margin: 0;
            padding: 0;
            border: 0;
            background: rgba(15, 23, 42, .46);
            opacity: 0;
            pointer-events: none;
            transition: opacity .24s ease;
            z-index: 1600;
        }

        .aha-space-launcher__drawer {
            position: fixed;
            inset: 0 auto 0 0;
            width: min(380px, 92vw);
            max-width: 100%;
            background: #ffffff;
            box-shadow: 0 24px 60px rgba(15, 23, 42, .24);
            transform: translateX(-104%);
            transition: transform .24s ease;
            z-index: 1601;
            display: flex;
            flex-direction: column;
        }

        .aha-space-launcher.is-open .aha-space-launcher__backdrop,
        .aha-space-launcher__backdrop.is-open {
            opacity: 1;
            pointer-events: auto;
        }

        .aha-space-launcher.is-open .aha-space-launcher__drawer,
        .aha-space-launcher__drawer.is-open {
            transform: translateX(0);
        }

        body.aha-space-launcher-open {
            overflow: hidden;
        }

        .aha-space-launcher__head {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 12px;
            padding: 18px 18px 14px;
            border-bottom: 1px solid #e2e8f0;
            background: linear-gradient(135deg, #eff6ff 0%, #f8fafc 100%);
        }

        .aha-space-launcher__eyebrow {
            margin: 0 0 4px;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: .04em;
            text-transform: uppercase;
            color: #64748b;
        }

        .aha-space-launcher__title {
            margin: 0;
            font-size: 22px;
            line-height: 1.25;
            font-weight: 800;
            color: #0f172a;
        }

        .aha-space-launcher__subtitle {
            margin: 6px 0 0;
            font-size: 13px;
            line-height: 1.45;
            color: #475569;
        }

        .aha-space-launcher__close {
            width: 40px;
            height: 40px;
            border-radius: 999px;
            border: 1px solid #dbe6ef;
            background: #ffffff;
            color: #0f172a;
            font-size: 24px;
            line-height: 1;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            flex: 0 0 40px;
        }

        .aha-space-launcher__body {
            flex: 1 1 auto;
            overflow-y: auto;
            padding: 16px;
            display: flex;
            flex-direction: column;
            gap: 14px;
        }

        .aha-space-launcher__account {
            border: 1px solid #dbe6ef;
            background: #f8fbff;
            border-radius: 16px;
            padding: 14px;
            display: flex;
            flex-direction: column;
            gap: 4px;
        }

        .aha-space-launcher__account strong {
            font-size: 18px;
            line-height: 1.3;
            color: #102a43;
        }

        .aha-space-launcher__account span {
            font-size: 13px;
            color: #64748b;
        }

        .aha-space-launcher__section-title {
            font-size: 12px;
            font-weight: 800;
            letter-spacing: .04em;
            text-transform: uppercase;
            color: #64748b;
            padding: 0 2px;
        }

        .aha-space-launcher__list {
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .aha-space-launcher__link {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            text-decoration: none;
            color: #102a43;
            border: 1px solid #dbe6ef;
            background: #ffffff;
            border-radius: 16px;
            padding: 12px 14px;
            transition: border-color .2s ease, box-shadow .2s ease, transform .2s ease;
        }

        .aha-space-launcher__link:hover {
            border-color: #93c5fd;
            box-shadow: 0 12px 30px rgba(37, 99, 235, .1);
            transform: translateY(-1px);
        }

        .aha-space-launcher__link.is-active {
            border-color: #16a34a;
            background: #f0fdf4;
        }

        .aha-space-launcher__link.is-request {
            border-color: #fed7aa;
            background: #fff7ed;
        }

        .aha-space-launcher__copy {
            display: flex;
            flex-direction: column;
            min-width: 0;
            gap: 3px;
        }

        .aha-space-launcher__copy strong {
            font-size: 15px;
            line-height: 1.35;
            color: #102a43;
        }

        .aha-space-launcher__copy span {
            font-size: 12px;
            line-height: 1.45;
            color: #64748b;
        }

        .aha-space-launcher__chevron {
            font-size: 22px;
            line-height: 1;
            color: #94a3b8;
            flex: 0 0 auto;
        }

        .aha-space-launcher__sr {
            position: absolute;
            width: 1px;
            height: 1px;
            padding: 0;
            margin: -1px;
            overflow: hidden;
            clip: rect(0, 0, 0, 0);
            white-space: nowrap;
            border: 0;
        }

        @media (max-width: 767px) {
            .aha-space-launcher__head {
                padding: 16px;
            }

            .aha-space-launcher__body {
                padding: 14px;
            }

            .aha-space-launcher__title {
                font-size: 20px;
            }
        }
    </style>

    <div id="<%=LauncherRootId %>" class="aha-space-launcher<%=WrapperCssClassResolved %>" data-aha-space-launcher>
        <button
            type="button"
            class="<%=ToggleButtonCssClass %>"
            aria-controls="<%=DrawerClientId %>"
            aria-expanded="false"
            aria-label="Không gian hoạt động"
            title="Không gian hoạt động">
            <span class="aha-space-launcher__toggle-bars" aria-hidden="true">
                <span></span>
                <span></span>
                <span></span>
            </span>
            <span class="aha-space-launcher__sr">Không gian hoạt động</span>
        </button>

        <button type="button" class="aha-space-launcher__backdrop" aria-label="Đóng không gian hoạt động"></button>

        <aside id="<%=DrawerClientId %>" class="aha-space-launcher__drawer" aria-hidden="true">
            <div class="aha-space-launcher__head">
                <div>
                    <div class="aha-space-launcher__eyebrow">Chuyển không gian</div>
                    <h3 class="aha-space-launcher__title">Không gian hoạt động</h3>
                    <p class="aha-space-launcher__subtitle">Ở đâu cũng có thể chuyển nhanh sang đúng không gian của tài khoản đang đăng nhập.</p>
                </div>
                <button type="button" class="aha-space-launcher__close" aria-label="Đóng">&times;</button>
            </div>

            <div class="aha-space-launcher__body">
                <div class="aha-space-launcher__account">
                    <strong><asp:Literal ID="litAccountName" runat="server" /></strong>
                    <span><asp:Literal ID="litAccountKey" runat="server" /></span>
                </div>

                <div class="aha-space-launcher__section-title">Danh sách không gian</div>
                <div class="aha-space-launcher__list">
                    <asp:Literal ID="litItems" runat="server" />
                </div>
            </div>
        </aside>
    </div>

    <script>
        (function () {
            function bootLaunchers() {
                var roots = document.querySelectorAll("[data-aha-space-launcher]");
                if (!roots || !roots.length) return;

                function getDrawer(root) {
                    return root ? (root.__ahaSpaceDrawer || root.querySelector(".aha-space-launcher__drawer")) : null;
                }

                function getBackdrop(root) {
                    return root ? (root.__ahaSpaceBackdrop || root.querySelector(".aha-space-launcher__backdrop")) : null;
                }

                function portalOverlay(root, node, kind) {
                    if (!root || !node || node.getAttribute("data-aha-space-portaled") === "1") return node;
                    node.setAttribute("data-aha-space-portaled", "1");
                    node.setAttribute("data-aha-space-owner", root.id || "");
                    node.classList.add("aha-space-launcher__" + kind + "--portal");
                    document.body.appendChild(node);
                    return node;
                }

                function closeRoot(root) {
                    if (!root) return;
                    root.classList.remove("is-open");
                    var toggle = root.querySelector(".aha-space-launcher__toggle");
                    var drawer = getDrawer(root);
                    var backdrop = getBackdrop(root);
                    if (toggle) toggle.setAttribute("aria-expanded", "false");
                    if (drawer) drawer.setAttribute("aria-hidden", "true");
                    if (drawer) drawer.classList.remove("is-open");
                    if (backdrop) backdrop.classList.remove("is-open");
                    if (!document.querySelector("[data-aha-space-launcher].is-open")) {
                        document.body.classList.remove("aha-space-launcher-open");
                    }
                }

                roots.forEach(function (root) {
                    if (!root || root.getAttribute("data-aha-space-launcher-bound") === "1") return;
                    root.setAttribute("data-aha-space-launcher-bound", "1");

                    var toggle = root.querySelector(".aha-space-launcher__toggle");
                    var drawer = root.querySelector(".aha-space-launcher__drawer");
                    var closeButton = root.querySelector(".aha-space-launcher__close");
                    var backdrop = root.querySelector(".aha-space-launcher__backdrop");

                    drawer = portalOverlay(root, drawer, "drawer");
                    backdrop = portalOverlay(root, backdrop, "backdrop");
                    root.__ahaSpaceDrawer = drawer;
                    root.__ahaSpaceBackdrop = backdrop;

                    function openRoot() {
                        roots.forEach(function (otherRoot) {
                            if (otherRoot !== root) closeRoot(otherRoot);
                        });

                        root.classList.add("is-open");
                        document.body.classList.add("aha-space-launcher-open");
                        if (toggle) toggle.setAttribute("aria-expanded", "true");
                        if (drawer) drawer.setAttribute("aria-hidden", "false");
                        if (drawer) drawer.classList.add("is-open");
                        if (backdrop) backdrop.classList.add("is-open");
                    }

                    if (toggle) {
                        toggle.addEventListener("click", function (event) {
                            event.preventDefault();
                            event.stopPropagation();
                            if (root.classList.contains("is-open")) closeRoot(root);
                            else openRoot();
                        });
                    }

                    if (closeButton) {
                        closeButton.addEventListener("click", function (event) {
                            event.preventDefault();
                            closeRoot(root);
                        });
                    }

                    if (backdrop) {
                        backdrop.addEventListener("click", function () {
                            closeRoot(root);
                        });
                    }

                    if (drawer) {
                        drawer.addEventListener("click", function (event) {
                            var link = event.target.closest("a");
                            if (link) closeRoot(root);
                        });
                    }
                });

                if (!window.__ahaSpaceLauncherEscBound) {
                    window.__ahaSpaceLauncherEscBound = true;
                    document.addEventListener("keydown", function (event) {
                        if (event.key !== "Escape") return;
                        document.querySelectorAll("[data-aha-space-launcher].is-open").forEach(function (root) {
                            root.classList.remove("is-open");
                            var toggle = root.querySelector(".aha-space-launcher__toggle");
                            var drawer = getDrawer(root);
                            var backdrop = getBackdrop(root);
                            if (toggle) toggle.setAttribute("aria-expanded", "false");
                            if (drawer) drawer.setAttribute("aria-hidden", "true");
                            if (drawer) drawer.classList.remove("is-open");
                            if (backdrop) backdrop.classList.remove("is-open");
                        });
                        document.body.classList.remove("aha-space-launcher-open");
                    });
                }
            }

            if (document.readyState === "loading") {
                document.addEventListener("DOMContentLoaded", bootLaunchers);
            } else {
                bootLaunchers();
            }

            if (window.Sys && Sys.WebForms) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    bootLaunchers();
                });
            }
        })();
    </script>
</asp:PlaceHolder>
