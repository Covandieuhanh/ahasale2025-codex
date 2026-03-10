<%@ Page Language="C#" AutoEventWireup="true" CodeFile="cho-thanh-toan.aspx.cs" Inherits="home_cho_thanh_toan" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AHASALE.VN</title>
    <meta charset="UTF-8" />
    <meta http-equiv="content-language" content="vi" />
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=yes" />

    <link href="/Metro-UI-CSS-master/css/metro-all.min.css" rel="stylesheet" />
    <link href="/assetscss/home-style.css" rel="stylesheet" />
    <link href="/assetscss/login.css?v=2026-03-02.1" rel="stylesheet" />
    <link href="/assetscss/aha-ui-refresh.css?v=2026-03-02.1" rel="stylesheet" />
    <link href="/assetscss/bcorn-with-metro.css" rel="stylesheet" />
    <link href="/assetscss/fix-metro.css" rel="stylesheet" />

    <style>
        :root {
            --wait-bg-1: #eef3fa;
            --wait-bg-2: #eef3fa;
            --wait-card: #ffffff;
            --wait-card-soft: #ffffff;
            --wait-border: rgba(98, 105, 118, 0.2);
            --wait-text: #182433;
            --wait-muted: #637085;
            --wait-primary: #ff5b2e;
            --wait-success: #06c167;
            --wait-danger: #dc2626;
            --wait-warn: #f59e0b;
        }

        html,
        body {
            min-height: 100%;
            margin: 0;
        }

        .body-login-bcorn1 {
            background: linear-gradient(180deg, var(--wait-bg-1), var(--wait-bg-2));
            color: var(--wait-text);
            overflow-x: hidden;
            -webkit-font-smoothing: antialiased;
            -webkit-overflow-scrolling: touch;
        }

        .wait-shell,
        .wait-shell * {
            color: inherit;
            box-sizing: border-box;
        }

        .wait-shell {
            width: min(1180px, 100% - 18px);
            margin: 12px auto 26px;
        }

        .wait-hero {
            border: 1px solid var(--wait-border);
            border-radius: 18px;
            background: #f6f8fc;
            box-shadow: 0 10px 26px rgba(15, 23, 42, 0.08);
            padding: 16px;
            margin-bottom: 12px;
        }

        .wait-hero-top {
            display: flex;
            align-items: center;
            gap: 11px;
        }

        .wait-logo {
            width: 50px;
            height: 50px;
            border-radius: 12px;
            object-fit: cover;
            border: 1px solid rgba(98, 105, 118, 0.2);
            background: #0f172a;
            padding: 5px;
        }

        .wait-stage {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 8px;
            padding: 6px 11px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 700;
            color: #6b3700;
            background: #fff3db;
            border: 1px solid #ffd38a;
        }

        .wait-title {
            margin: 0;
            font-size: 30px;
            line-height: 1.18;
            font-weight: 800;
            letter-spacing: .12px;
            color: var(--wait-text);
        }

        .wait-tag {
            margin-top: 8px;
            display: inline-flex;
            align-items: center;
            padding: 5px 11px;
            border-radius: 999px;
            border: 1px solid #b7f0ce;
            background: #e8fbf0;
            color: #005e30;
            font-size: 13px;
            font-weight: 700;
        }

        .wait-sub {
            margin-top: 10px;
            color: var(--wait-muted);
            font-size: 14px;
            line-height: 1.52;
        }

        .wait-hero-actions {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
            margin-top: 12px;
        }

        .wait-main {
            display: grid;
            grid-template-columns: 1fr;
            gap: 10px;
        }

        .wait-card {
            border: 1px solid var(--wait-border);
            border-radius: 16px;
            background: linear-gradient(160deg, var(--wait-card), var(--wait-card-soft));
            box-shadow: 0 8px 24px rgba(15, 23, 42, 0.06);
            padding: 14px;
            position: relative;
        }

        .wait-kicker {
            display: inline-flex;
            align-items: center;
            border: 1px solid #b7f0ce;
            background: #e8fbf0;
            color: #005e30;
            border-radius: 999px;
            padding: 5px 10px;
            font-size: 12px;
            font-weight: 700;
            margin-bottom: 8px;
        }

        .wait-card-title {
            margin: 0;
            font-size: 28px;
            line-height: 1.2;
            font-weight: 800;
            color: var(--wait-text);
        }

        .wait-note {
            margin-top: 8px;
            color: var(--wait-muted);
            font-size: 14px;
            line-height: 1.45;
        }

        .wait-note.warn {
            color: #b91c1c;
        }

        .wait-amount {
            margin-top: 12px;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 8px 12px;
            border-radius: 12px;
            border: 1px solid #b7f0ce;
            background: #f0fdf4;
            color: #166534;
            font-size: 36px;
            font-weight: 800;
            line-height: 1.1;
        }

        .wait-pin {
            margin-top: 10px;
            display: flex;
            align-items: center;
            width: 100%;
            min-height: 50px;
            border-radius: 14px;
            border: 1px solid rgba(98, 105, 118, 0.3);
            background: #ffffff;
            overflow: hidden;
        }

        .wait-pin-icon {
            flex: 0 0 48px;
            text-align: center;
            color: #0f172a;
            border-right: 1px solid rgba(15, 23, 42, 0.14);
            line-height: 50px;
            font-size: 18px;
        }

        .wait-pin-input {
            width: 100%;
            min-width: 0;
            border: 0 !important;
            box-shadow: none !important;
            background: transparent !important;
            color: #111827 !important;
            height: 50px !important;
            font-size: 16px;
            font-weight: 700;
            letter-spacing: .2px;
            padding: 0 12px !important;
        }

        .wait-pin-input::placeholder {
            color: #64748b;
            font-weight: 500;
        }

        .wait-pin .aha-password-toggle-label {
            font-size: 14px;
        }

        .wait-inline-error {
            margin-top: 6px;
            font-size: 12px;
            color: #dc2626;
            line-height: 1.4;
            display: none;
        }

        .wait-inline-error.show {
            display: block;
        }

        .wait-alert {
            margin-top: 10px;
            border-radius: 12px;
            padding: 10px 12px;
            border: 1px solid #fecaca;
            background: #fff5f5;
            color: #b91c1c;
            font-size: 13px;
            line-height: 1.45;
        }

        .wait-actions {
            margin-top: 12px;
            display: grid;
            gap: 8px;
        }

        .wait-card-active .wait-actions {
            position: sticky;
            bottom: max(8px, env(safe-area-inset-bottom));
            z-index: 6;
            background: linear-gradient(180deg, rgba(246, 248, 252, 0), rgba(246, 248, 252, 0.94) 26%, rgba(246, 248, 252, 1));
            padding-top: 10px;
        }

        .wait-btn,
        .wait-btn:visited {
            appearance: none;
            border: 1px solid transparent;
            min-height: 44px;
            border-radius: 12px;
            padding: 0 14px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            text-decoration: none;
            font-size: 15px;
            font-weight: 700;
            cursor: pointer;
            transition: all .18s ease;
            color: #fff !important;
            width: 100%;
        }

        .wait-btn:hover {
            transform: translateY(-1px);
        }

        .wait-btn.primary {
            background: linear-gradient(140deg, #ff5b2e, #ff7a47);
            border-color: rgba(255, 106, 57, 0.5);
        }

        .wait-btn.success {
            background: linear-gradient(140deg, #06c167, #05a75a);
            border-color: rgba(6, 193, 103, 0.45);
        }

        .wait-btn.danger {
            background: linear-gradient(145deg, #dc2626, #ef4444);
            border-color: rgba(239, 68, 68, 0.45);
        }

        .wait-btn.muted {
            background: #edf2f7;
            border-color: rgba(98, 105, 118, 0.28);
            color: #334155 !important;
        }

        .wait-btn.loading {
            opacity: .92;
            pointer-events: none;
            cursor: wait;
        }

        .wait-btn.loading::after {
            content: "";
            width: 14px;
            height: 14px;
            border-radius: 50%;
            border: 2px solid rgba(255, 255, 255, 0.45);
            border-top-color: #fff;
            display: inline-block;
            margin-left: 8px;
            vertical-align: -2px;
            animation: waitSpin .75s linear infinite;
        }

        .wait-trust {
            margin-top: 10px;
            font-size: 12px;
            color: #5b6b83;
            line-height: 1.45;
        }

        .wait-order-wrap {
            margin-top: 8px;
        }

        .wait-table-wrap {
            border: 1px solid var(--wait-border);
            border-radius: 14px;
            background: #fff;
            overflow-x: auto;
            overflow-y: hidden;
            -webkit-overflow-scrolling: touch;
        }

        .wait-table {
            width: 100%;
            min-width: 640px;
            border-collapse: separate;
            border-spacing: 0;
        }

        .wait-table thead {
            display: table-header-group;
        }

        .wait-table thead th {
            padding: 10px 8px;
            border-bottom: 1px solid var(--wait-border);
            color: #48566e;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: .16px;
            background: #f8fafc;
        }

        .wait-table tbody {
            display: table-row-group;
            padding: 0;
        }

        .wait-table tbody tr {
            display: table-row;
            border: 0;
            border-radius: 0;
            padding: 0;
            background: transparent;
        }

        .wait-table tbody td {
            padding: 9px 8px !important;
            border-bottom: 1px solid rgba(98, 105, 118, 0.14) !important;
            display: table-cell;
            vertical-align: middle;
            color: #1e293b;
            font-size: 13px;
        }

        .wait-table .cell-id {
            display: table-cell;
        }

        .wait-product-img {
            width: 56px;
            height: 56px;
            border-radius: 12px;
            object-fit: cover;
            border: 1px solid rgba(98, 105, 118, 0.24);
            background: #fff;
        }

        .wait-table .cell-product {
            font-weight: 700;
            line-height: 1.35;
        }

        .wait-table .cell-product .price-line {
            margin-top: 2px;
            color: #0f766e;
            font-weight: 600;
            font-size: 13px;
        }

        .wait-table .cell-qty::before,
        .wait-table .cell-uudai::before,
        .wait-table .cell-total::before {
            display: none;
        }

        .wait-table .cell-total {
            font-weight: 700;
            color: #166534;
        }

        .wait-progress {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            height: 3px;
            background: rgba(15, 23, 42, 0.08);
            z-index: 9999;
            pointer-events: none;
            overflow: hidden;
        }

        .wait-progress::before {
            content: "";
            display: block;
            width: 42%;
            height: 100%;
            background: linear-gradient(90deg, #ff5b2e, #06c167);
            animation: wait-progress-move 1.05s linear infinite;
            border-radius: 999px;
        }

        @keyframes wait-progress-move {
            0% { transform: translateX(-110%); }
            100% { transform: translateX(320%); }
        }

        @keyframes waitSpin {
            to { transform: rotate(360deg); }
        }

        @media (min-width: 740px) {
            .wait-shell {
                width: min(1180px, 100% - 26px);
                margin-top: 16px;
            }

            .wait-title {
                font-size: 32px;
            }

            .wait-card-title {
                font-size: 30px;
            }

            .wait-actions {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }

            .wait-card-active .wait-actions {
                position: static;
                background: transparent;
                padding-top: 0;
            }

            .wait-table thead {
                display: table-header-group;
            }

            .wait-table thead th {
                padding: 10px 8px;
                border-bottom: 1px solid var(--wait-border);
                color: #48566e;
                font-size: 12px;
                text-transform: uppercase;
                letter-spacing: .16px;
                background: #f8fafc;
            }

            .wait-table tbody {
                display: table-row-group;
                gap: 0;
                padding: 0;
            }

            .wait-table tbody tr {
                display: table-row;
                border: 0;
                border-radius: 0;
                padding: 0;
                background: transparent;
            }

            .wait-table tbody td {
                padding: 9px 8px !important;
                border-bottom: 1px solid rgba(98, 105, 118, 0.14) !important;
                font-size: 13px;
                display: table-cell;
                vertical-align: middle;
            }

            .wait-table .cell-id {
                display: table-cell;
            }

            .wait-table .cell-product .price-line {
                margin-top: 1px;
            }

            .wait-table .cell-qty::before,
            .wait-table .cell-uudai::before,
            .wait-table .cell-total::before {
                display: none;
            }
        }

        @media (min-width: 1020px) {
            .wait-main {
                grid-template-columns: minmax(0, 0.95fr) minmax(0, 1.35fr);
                gap: 14px;
            }

            .wait-title {
                font-size: 34px;
            }
        }
    </style>
</head>

<body class="body-login-bcorn1">
<form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True"></asp:ScriptManager>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <asp:Timer ID="Timer1" runat="server" Interval="12000" OnTick="Timer1_Tick"></asp:Timer>

            <div class="wait-shell">
                <section class="wait-hero">
                    <div class="wait-hero-top">
                        <img src="/uploads/images/logo-aha-trang.png" alt="AhaSale" class="wait-logo" />
                        <div>
                            <div class="wait-stage">Quy trình 2 bước: Kích hoạt và Trao đổi</div>
                            <h1 class="wait-title">Chờ Trao đổi Đơn #<asp:Label ID="Label4" runat="server" Text=""></asp:Label></h1>
                            <span class="wait-tag">1 Quyền = 1000 VNĐ (quy đổi nội bộ)</span>
                        </div>
                    </div>
                    <div class="wait-sub">
                        Quyền tiêu dùng là đơn vị quy ước nội bộ được hiển thị và sử dụng trên nền tảng ahasale.vn. Quyền tiêu dùng không phải tiền tệ hợp pháp và chỉ có hiệu lực trong phạm vi nền tảng.
                    </div>
                    <asp:PlaceHolder ID="ph_shop_home" runat="server" Visible="false">
                        <div class="wait-hero-actions">
                            <a href="/shop/default.aspx" class="wait-btn primary" style="width:auto; min-width:140px;">Trang chủ shop</a>
                            <a href="/shop/don-ban" class="wait-btn muted" style="width:auto; min-width:120px;">Đơn bán</a>
                        </div>
                    </asp:PlaceHolder>
                </section>

                <section class="wait-main">
                    <div>
                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="false">
                            <div class="wait-card">
                                <div class="wait-kicker">Bước 1: Chờ quét thẻ</div>
                                <h2 class="wait-card-title">Sẵn sàng Trao đổi</h2>
                                <div class="wait-note">Vui lòng chạm thẻ khách hàng để bắt đầu phiên Trao đổi.</div>
                                <div class="wait-amount"><asp:Label ID="Label1" runat="server" Text=""></asp:Label></div>
                                <div class="wait-actions">
                                    <asp:Button ID="btn_huy_cho" runat="server" OnClick="btn_huy_cho_Click" CausesValidation="false" Text="Hủy chờ Trao đổi" CssClass="wait-btn danger" />
                                    <asp:HyperLink ID="lnk_refresh_wait" runat="server" CssClass="wait-btn muted">Làm mới</asp:HyperLink>
                                </div>
                                <div class="wait-trust">Đơn sẽ giữ trạng thái chờ Trao đổi cho tới khi có thẻ hợp lệ.</div>
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="false">
                            <div class="wait-card wait-card-active">
                                <div class="wait-kicker">Bước 2: Xác thực PIN</div>
                                <h2 class="wait-card-title">Xin chào, <asp:Label ID="Label3" runat="server" Text=""></asp:Label></h2>
                                <div class="wait-note">Bạn sắp trao đổi <b><asp:Label ID="Label2" runat="server" Text=""></asp:Label></b> với <b><asp:Label ID="lb_tenshop" runat="server" Text=""></asp:Label></b>.</div>
                                <div class="wait-note warn">Nếu không phải bạn, vui lòng nhấn <b>Hủy</b>.</div>

                                <div class="wait-pin">
                                    <span class="mif-key wait-pin-icon"></span>
                                    <asp:TextBox
                                        MaxLength="4"
                                        TextMode="Password"
                                        ID="txt_mapin"
                                        runat="server"
                                        Attributes="inputmode:numeric;pattern:[0-9]*"
                                        CssClass="wait-pin-input"
                                        placeholder="Nhập mã PIN 4 số để hoàn tất Trao đổi">
                                    </asp:TextBox>
                                    <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện mã PIN">
                                        <span class="aha-password-toggle-label">Hiện</span>
                                    </button>
                                </div>
                                <div class="wait-inline-error" id="pin_inline_error"></div>

                                <asp:PlaceHolder ID="ph_alert_the" runat="server">
                                    <div class="wait-alert" runat="server" id="box_alert_the" visible="false">
                                        <asp:Literal ID="lb_thongbao_the" runat="server" Visible="false"></asp:Literal>
                                    </div>
                                </asp:PlaceHolder>

                                <div class="wait-actions">
                                    <asp:Button ID="Button2" OnClick="Button2_Click" runat="server" Text="Hủy" CssClass="wait-btn muted" />
                                    <asp:Button ID="Button3" OnClick="Button3_Click" runat="server" Text="Trao đổi" CssClass="wait-btn success" OnClientClick="return ahaWaitBeforeSubmit(this);" />
                                </div>
                                <div class="wait-trust">PIN thẻ gồm 4 số. Nếu bạn nhập sai quá số lần cho phép, hệ thống sẽ khóa thao tác để bảo mật.</div>
                            </div>
                        </asp:PlaceHolder>
                    </div>

                    <div class="wait-order-wrap">
                        <div class="wait-card">
                            <h2 class="wait-card-title" style="font-size:34px;">Chi tiết đơn hàng</h2>
                            <div class="wait-note">Danh sách sản phẩm trong đơn đang chờ Trao đổi.</div>

                            <div class="wait-table-wrap">
                                <table class="wait-table">
                                    <thead>
                                        <tr>
                                            <th style="width:56px;">ID</th>
                                            <th style="width:72px;">Ảnh</th>
                                            <th class="text-left" style="min-width:180px;">Sản phẩm</th>
                                            <th style="width:56px;">SL</th>
                                            <th style="width:92px;">% Ưu đãi</th>
                                            <th style="min-width:120px;">Tổng</th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        <asp:Repeater ID="Repeater2" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="text-center cell-id"><%# Eval("id") %></td>

                                                    <td class="text-center cell-image">
                                                        <div data-role="lightbox" class="c-pointer">
                                                            <img src="<%# Eval("image") %>" class="wait-product-img" />
                                                        </div>
                                                    </td>

                                                    <td class="text-left cell-product">
                                                        <%# Eval("name") %>
                                                        <div class="price-line">
                                                            <%# FormatQuyen(Eval("giaban")) %> Quyền
                                                            <%# (Convert.ToInt32(Eval("PhanTramUuDai")) > 0
                                                                ? "<span style='display:inline-flex; margin-left:6px; padding:1px 6px; border-radius:999px; border:1px solid rgba(245,180,1,.46); background:rgba(245,180,1,.16); color:#fde68a;'>-"
                                                                  + Eval("PhanTramUuDai") + "%</span>"
                                                                : "") %>
                                                        </div>
                                                    </td>

                                                    <td class="text-center cell-qty" data-label="SL"><%# Eval("soluong") %></td>

                                                    <td class="text-center cell-uudai" data-label="Ưu đãi">
                                                        <%# (Convert.ToInt32(Eval("PhanTramUuDai")) > 0 ? (Eval("PhanTramUuDai") + "%") : "-") %>
                                                    </td>

                                                    <td class="text-right cell-total" data-label="Tổng">
                                                        <%# FormatQuyen(Eval("thanhtien")) %> Quyền
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </section>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btn_huy_cho" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="900">
        <ProgressTemplate>
            <div class="wait-progress"></div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</form>

<script src="/Metro-UI-CSS-master/js/metro.min.js"></script>
<script src="/js/aha-ui-refresh.js?v=2026-03-07.2"></script>
<script>
    function ahaWaitValidatePin() {
        var pinInput = document.getElementById("<%= txt_mapin.ClientID %>");
        var inlineErr = document.getElementById("pin_inline_error");
        if (!pinInput || !inlineErr) return true;

        var digits = String(pinInput.value || "").replace(/\D+/g, "");
        if (!/^\d{4}$/.test(digits)) {
            inlineErr.textContent = "Vui lòng nhập mã PIN gồm đúng 4 số.";
            inlineErr.classList.add("show");
            pinInput.focus();
            return false;
        }

        inlineErr.textContent = "";
        inlineErr.classList.remove("show");
        return true;
    }

    function ahaWaitBeforeSubmit(btn) {
        if (!ahaWaitValidatePin()) return false;
        if (!btn) return true;
        if (btn.dataset.loading === "1") return false;

        btn.dataset.loading = "1";
        btn.dataset.originalText = btn.value;
        btn.value = "Đang trao đổi...";
        btn.classList.add("loading");
        return true;
    }

    function ahaWaitResetSubmitState() {
        var btn = document.getElementById("<%= Button3.ClientID %>");
        if (!btn) return;
        btn.dataset.loading = "0";
        btn.disabled = false;
        btn.classList.remove("loading");
        if (btn.dataset.originalText) {
            btn.value = btn.dataset.originalText;
        }
    }

    function ahaWaitBindPinInput() {
        var pinInput = document.getElementById("<%= txt_mapin.ClientID %>");
        var inlineErr = document.getElementById("pin_inline_error");
        if (!pinInput || !inlineErr) return;
        if (pinInput.dataset.boundValidation === "1") return;

        pinInput.dataset.boundValidation = "1";
        pinInput.addEventListener("input", function () {
            pinInput.value = String(pinInput.value || "").replace(/\D+/g, "").slice(0, 4);
            if (/^\d{4}$/.test(pinInput.value)) {
                inlineErr.textContent = "";
                inlineErr.classList.remove("show");
            }
        });
    }

    document.addEventListener("DOMContentLoaded", function () {
        ahaWaitBindPinInput();
    });

    if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            ahaWaitBindPinInput();
            ahaWaitResetSubmitState();
        });
    }
</script>
</body>
</html>
