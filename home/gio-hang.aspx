<%@ Page Title="Giỏ hàng của bạn" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true" CodeFile="gio-hang.aspx.cs" Inherits="home_gio_hang" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server">
    <style>
        :root {
            --checkout-bg: #f6f8fc;
            --checkout-card: #ffffff;
            --checkout-border: rgba(98, 105, 118, 0.2);
            --checkout-text: #182433;
            --checkout-muted: #637085;
            --checkout-primary: #ff5b2e;
            --checkout-primary-soft: #fff1ed;
            --checkout-tiktok: #06c167;
        }

        html[data-bs-theme="dark"] {
            --checkout-bg: #0b1220;
            --checkout-card: #0f172a;
            --checkout-border: #223246;
            --checkout-text: #e5e7eb;
            --checkout-muted: #94a3b8;
            --checkout-primary: #ff7a47;
            --checkout-primary-soft: rgba(255, 122, 71, 0.12);
            --checkout-tiktok: #22c55e;
        }

        .cart-wrap {
            max-width: 1160px;
        }

        .cart-shell {
            background: var(--checkout-bg);
            border: 1px solid var(--checkout-border);
            border-radius: 18px;
            padding: 16px;
        }

        .cart-stage {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 12px;
            padding: 6px 12px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 700;
            color: #6b3700;
            background: #fff3db;
            border: 1px solid #ffd38a;
        }

        html[data-bs-theme="dark"] .cart-stage {
            color: #fcd34d;
            background: rgba(245, 158, 11, 0.16);
            border-color: rgba(245, 158, 11, 0.4);
        }

        .cart-head {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
            margin-bottom: 14px;
        }

        .cart-head h2 {
            margin: 0;
            font-size: 28px;
            color: var(--checkout-text);
            font-weight: 800;
            line-height: 1.2;
        }

        .cart-head p {
            margin: 6px 0 0;
            color: var(--checkout-muted);
            font-size: 14px;
        }

        .cart-actions {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
        }

        .cart-actions .btn-warning {
            background: linear-gradient(135deg, var(--checkout-primary), #ff7a47);
            border-color: #ff6a39;
            color: #fff;
            font-weight: 700;
        }

        .cart-actions .btn-warning.is-loading {
            opacity: .92;
            cursor: wait;
            pointer-events: none;
        }

        .cart-actions .btn-warning.is-loading::after {
            content: "";
            width: 14px;
            height: 14px;
            border-radius: 50%;
            border: 2px solid rgba(255, 255, 255, 0.45);
            border-top-color: #fff;
            display: inline-block;
            margin-left: 8px;
            vertical-align: -2px;
            animation: checkoutSpin .75s linear infinite;
        }

        .cart-table-card,
        .checkout-card {
            border: 1px solid var(--checkout-border);
            border-radius: 16px;
            background: var(--checkout-card);
            box-shadow: 0 8px 24px rgba(15, 23, 42, 0.06);
        }

        .cart-table-card .card-header,
        .checkout-card .card-header {
            background: #fff;
            border-bottom: 1px solid var(--checkout-border);
        }

        html[data-bs-theme="dark"] .cart-table-card,
        html[data-bs-theme="dark"] .checkout-card {
            box-shadow: 0 12px 30px rgba(0, 0, 0, 0.45);
        }

        html[data-bs-theme="dark"] .cart-table-card .card-header,
        html[data-bs-theme="dark"] .checkout-card .card-header {
            background: #111d34;
            color: var(--checkout-text);
        }

        .saved-address-panel {
            margin-bottom: 12px;
        }

        .saved-address-list {
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .saved-address-item {
            display: flex;
            gap: 10px;
            align-items: center;
            padding: 10px 12px;
            border: 1px solid var(--checkout-border);
            border-radius: 12px;
            background: var(--checkout-card);
            transition: border-color 0.2s ease, box-shadow 0.2s ease;
        }

        .saved-address-item.is-selected {
            border-color: var(--checkout-primary);
            box-shadow: 0 0 0 2px rgba(255, 91, 46, 0.12);
        }

        .saved-address-radio {
            margin-top: 4px;
        }

        .saved-address-choice {
            display: flex;
            gap: 10px;
            align-items: flex-start;
            flex: 1;
            cursor: pointer;
        }

        .saved-address-content {
            display: flex;
            flex-direction: column;
            gap: 4px;
            flex: 1;
        }

        .saved-address-name {
            font-weight: 600;
            color: var(--checkout-text);
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .saved-address-text {
            font-size: 0.95rem;
            color: var(--checkout-muted);
            white-space: pre-line;
        }

        .saved-address-new .saved-address-name {
            color: var(--checkout-primary);
        }

        .saved-address-badge {
            display: inline-flex;
            align-items: center;
            padding: 2px 8px;
            border-radius: 999px;
            font-size: 11px;
            font-weight: 700;
            background: rgba(34, 197, 94, 0.15);
            color: #16a34a;
        }

        .saved-address-delete {
            border: none;
            background: transparent;
            color: #d63939;
            font-size: 0.85rem;
            font-weight: 600;
            text-decoration: underline;
            cursor: pointer;
        }

        .saved-address-set-default {
            border: none;
            background: transparent;
            color: var(--checkout-primary);
            font-size: 0.85rem;
            font-weight: 600;
            text-decoration: underline;
            cursor: pointer;
        }

        .saved-address-actions {
            display: flex;
            align-items: center;
            gap: 8px;
            flex-shrink: 0;
        }

        .saved-address-manage {
            font-size: 0.85rem;
            font-weight: 600;
            color: var(--checkout-primary);
            text-decoration: none;
        }

        .table thead th {
            white-space: nowrap;
        }

        .table td,
        .table th {
            vertical-align: middle;
        }

        .td-img img {
            border-radius: 10px;
            border: 1px solid rgba(98, 105, 118, .18);
            object-fit: cover;
        }

        .cart-product-inline {
            display: flex;
            align-items: center;
            gap: 12px;
            min-width: 0;
        }

        .cart-product-inline-thumb {
            display: none;
            width: 72px;
            min-width: 72px;
            height: 72px;
            border-radius: 12px;
            overflow: hidden;
            border: 1px solid rgba(98, 105, 118, .18);
            background: #f8fafc;
        }

        .cart-product-inline-thumb img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            display: block;
        }

        .cart-product-inline-content {
            min-width: 0;
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .cart-product-inline-name {
            color: inherit;
            text-decoration: none;
            font-weight: 700;
            line-height: 1.35;
        }

        .cart-product-inline-shop {
            display: none;
            font-size: 12px;
            color: var(--checkout-muted);
            line-height: 1.3;
        }

        .qty-input {
            width: 56px;
            min-width: 56px;
            border: 0 !important;
            box-shadow: none !important;
            padding: 0 !important;
            height: 30px;
            background: transparent !important;
            color: #111827 !important;
            font-weight: 800 !important;
            line-height: 30px;
        }

        .qty-stepper {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
            border: 1px solid rgba(98, 105, 118, 0.3);
            border-radius: 999px;
            padding: 3px 7px;
            background: #fff;
            min-width: 126px;
        }

        .qty-step-btn {
            width: 28px;
            height: 28px;
            border-radius: 999px;
            border: 1px solid rgba(98, 105, 118, 0.35);
            background: #f8fafc;
            color: #182433;
            font-size: 18px;
            font-weight: 800;
            line-height: 1;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            user-select: none;
            transition: all .18s ease;
        }

        html[data-bs-theme="dark"] .qty-input {
            color: #e5e7eb !important;
        }

        html[data-bs-theme="dark"] .qty-stepper {
            background: #111d34;
            border-color: #223246;
        }

        html[data-bs-theme="dark"] .qty-step-btn {
            background: #0f172a;
            border-color: #334155;
            color: #e5e7eb;
        }

        html[data-bs-theme="dark"] .qty-step-btn:hover {
            background: #1e2a3d;
            border-color: #4b5563;
            color: #f8fafc;
        }

        .qty-step-btn:hover {
            background: #e9f2ff;
            border-color: #60a5fa;
            color: #1d4ed8;
        }

        .qty-step-btn:active {
            transform: scale(.96);
        }

        .text-price {
            font-weight: 700;
            color: #d63939;
        }

        .text-line-total {
            font-weight: 700;
            color: #14532d;
        }

        .checkout-stage {
            border: 1px solid var(--checkout-border);
            border-radius: 18px;
            background: #fff;
            padding: 16px;
            margin-top: 16px;
        }

        html[data-bs-theme="dark"] .checkout-stage {
            background: var(--checkout-card);
        }

        .checkout-head {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            gap: 10px;
            flex-wrap: wrap;
            margin-bottom: 12px;
        }

        .checkout-pill {
            display: inline-flex;
            align-items: center;
            padding: 5px 10px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 700;
            color: #005e30;
            background: #e8fbf0;
            border: 1px solid #b7f0ce;
            margin-bottom: 6px;
        }

        html[data-bs-theme="dark"] .checkout-pill {
            color: #6ee7b7;
            background: rgba(16, 185, 129, 0.12);
            border-color: rgba(16, 185, 129, 0.35);
        }

        .checkout-head h3 {
            margin: 0;
            font-size: 24px;
            line-height: 1.2;
            font-weight: 800;
            color: var(--checkout-text);
        }

        .checkout-head p {
            margin: 6px 0 0;
            color: var(--checkout-muted);
            font-size: 14px;
        }

        .checkout-grid {
            display: grid;
            grid-template-columns: minmax(0, 1.5fr) minmax(0, 1fr);
            gap: 12px;
        }

        .checkout-summary {
            border: 1px solid var(--checkout-border);
            background: var(--checkout-primary-soft);
            border-radius: 14px;
            padding: 12px;
            margin-top: 14px;
        }

        .checkout-summary-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            padding: 7px 0;
            color: var(--checkout-muted);
            font-size: 14px;
        }

        .checkout-summary-row strong {
            color: #111827;
        }

        .checkout-summary-row.total {
            border-top: 1px dashed rgba(99, 112, 133, 0.32);
            margin-top: 4px;
            padding-top: 10px;
        }

        .checkout-summary-row.total strong {
            color: var(--checkout-tiktok);
            font-size: 21px;
        }

        .checkout-summary-card .card-body {
            padding-top: 10px;
        }

        .field-error {
            margin-top: 6px;
            font-size: 12px;
            line-height: 1.35;
            color: #dc2626;
            display: none;
        }

        .field-error.show {
            display: block;
        }

        .checkout-input.is-invalid {
            border-color: rgba(220, 38, 38, 0.65) !important;
            box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.1) !important;
        }

        .checkout-actionbar {
            margin-top: 12px;
            border: 1px solid var(--checkout-border);
            border-radius: 14px;
            padding: 10px 12px;
            background: #fff;
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 12px;
            flex-wrap: wrap;
            position: sticky;
            bottom: 8px;
            z-index: 12;
            box-shadow: 0 10px 24px rgba(15, 23, 42, 0.12);
        }

        html[data-bs-theme="dark"] .checkout-summary {
            background: rgba(255, 122, 71, 0.12);
        }

        html[data-bs-theme="dark"] .checkout-summary-row strong {
            color: #f8fafc;
        }

        html[data-bs-theme="dark"] .checkout-actionbar {
            background: #0f172a;
            box-shadow: 0 12px 30px rgba(0, 0, 0, 0.45);
        }

        html[data-bs-theme="dark"] .checkout-actionbar .cta-trust {
            color: #6ee7b7;
        }

        .checkout-actionbar .cta-meta {
            color: var(--checkout-muted);
            font-size: 13px;
            font-weight: 600;
        }

        .checkout-actionbar .cta-trust {
            margin-top: 4px;
            font-size: 12px;
            color: #065f46;
            font-weight: 600;
        }

        .checkout-actionbar .cta-total-wrap {
            margin-top: 4px;
            display: flex;
            align-items: center;
            gap: 10px;
            flex-wrap: wrap;
        }

        .checkout-actionbar .cta-vnd {
            font-size: 16px;
            font-weight: 800;
            color: #d63939;
        }

        .checkout-actionbar .cta-total {
            font-size: 24px;
            line-height: 1.1;
            font-weight: 800;
            color: var(--checkout-tiktok);
        }

        .checkout-actionbar .checkout-btn-main {
            min-width: 190px;
            font-weight: 700;
            position: relative;
        }

        .checkout-actionbar .checkout-btn-main.is-loading {
            opacity: .92;
            cursor: wait;
            pointer-events: none;
        }

        .checkout-actionbar .checkout-btn-main.is-loading::after {
            content: "";
            width: 14px;
            height: 14px;
            border-radius: 50%;
            border: 2px solid rgba(255, 255, 255, 0.45);
            border-top-color: #fff;
            display: inline-block;
            margin-left: 8px;
            vertical-align: -2px;
            animation: checkoutSpin .75s linear infinite;
        }

        .checkout-link-cancel {
            font-size: 14px;
            color: var(--checkout-muted);
            text-decoration: none;
            font-weight: 600;
        }

        .checkout-link-cancel:hover {
            text-decoration: underline;
        }

        .checkout-line.is-updated {
            animation: checkoutRowFlash .45s ease;
        }

        @keyframes checkoutSpin {
            to { transform: rotate(360deg); }
        }

        @keyframes checkoutRowFlash {
            0% { background: rgba(34, 197, 94, .12); }
            100% { background: transparent; }
        }

        .overlay-loading {
            position: fixed;
            inset: 0;
            background: rgba(0,0,0,.6);
            z-index: 99999;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        @media (max-width: 767.98px) {
            .cart-shell {
                padding: 12px;
                border-radius: 14px;
            }

            .cart-head h2 {
                font-size: 24px;
            }

            .checkout-grid {
                grid-template-columns: 1fr;
            }

            .checkout-actionbar {
                bottom: max(10px, env(safe-area-inset-bottom));
                box-shadow: 0 12px 26px rgba(15, 23, 42, 0.16);
            }

            .checkout-actionbar .cta-total {
                font-size: 20px;
            }

            .checkout-actionbar .cta-vnd {
                font-size: 14px;
            }

            .col-id,
            .col-shop,
            .col-image {
                display: none;
            }

            .qty-stepper {
                min-width: 112px;
                gap: 4px;
                padding: 2px 5px;
            }

            .qty-step-btn {
                width: 26px;
                height: 26px;
            }

            .qty-input {
                width: 48px;
                min-width: 48px;
            }

            .cart-product-inline-thumb {
                display: block;
            }

            .cart-product-inline-shop {
                display: block;
            }
        }

        @media (max-width: 767.98px) {
            .checkout-card .table-responsive {
                overflow: visible;
            }

            .checkout-table {
                width: 100%;
                min-width: 0 !important;
            }

            .checkout-table thead {
                display: none;
            }

            .checkout-table tbody {
                display: grid;
                gap: 10px;
                padding: 10px;
            }

            .checkout-table tbody tr.checkout-line {
                display: grid;
                grid-template-columns: 64px 1fr;
                gap: 8px 10px;
                border: 1px solid var(--checkout-border);
                border-radius: 12px;
                padding: 10px;
                background: #fff;
            }

            .checkout-table tbody td {
                border: 0 !important;
                padding: 0 !important;
                text-align: left !important;
                display: block;
            }

            .checkout-table .cell-image {
                grid-row: 1 / span 4;
            }

            .checkout-table .cell-image img {
                width: 60px;
                height: 60px;
            }

            .checkout-table .cell-product {
                font-size: 15px;
                line-height: 1.35;
                font-weight: 700;
            }

            .checkout-table .cell-price::before,
            .checkout-table .cell-qty::before {
                content: attr(data-label);
                display: inline-block;
                font-size: 12px;
                font-weight: 600;
                color: var(--checkout-muted);
                margin-right: 7px;
            }

            .checkout-table .cell-qty .qty-stepper {
                margin-top: 4px;
            }

            .checkout-table .cell-shop {
                grid-column: 1 / -1;
                padding-top: 6px !important;
                border-top: 1px dashed rgba(99, 112, 133, 0.3);
                font-size: 12px;
                color: var(--checkout-muted);
            }

            html[data-bs-theme="dark"] .checkout-table tbody tr.checkout-line {
                background: #111d34;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <div class="container-xl cart-wrap py-4">
        <div class="cart-shell">
                    <div class="cart-stage">Bước 1/2: Giỏ hàng và chọn sản phẩm</div>

                    <div class="cart-head">
                        <div>
                            <h2>Giỏ hàng của bạn</h2>
                            <p>Chọn sản phẩm muốn mua, cập nhật số lượng rồi bấm <b>Trao đổi ngay</b> để sang bước trao đổi.</p>
                        </div>

                        <div class="cart-actions">
                            <asp:Button ID="Button1" runat="server" Text="Xóa" CssClass="btn btn-outline-danger" OnClick="Button1_Click" />
                            <asp:Button ID="Button3" runat="server" Text="Lưu chỉnh sửa" CssClass="btn btn-outline-info" OnClick="Button3_Click" />
                            <button id="btn_go_step2" type="button" class="btn btn-warning" onclick="return ahaGoCheckoutStep2(this);">Trao đổi ngay</button>
                        </div>
                    </div>

                    <asp:Panel ID="pn_step1" runat="server">
                    <div class="card cart-table-card">
                        <div class="card-header">
                            <div class="fw-bold">Danh sách sản phẩm</div>
                        </div>

                        <div class="card-body p-0">
                            <div class="table-responsive">
                                <table class="table table-vcenter card-table mb-0">
                                    <thead>
                                        <tr>
                                            <th class="col-id" style="width:1px;">ID</th>
                                            <th style="width:1px;" class="text-center">
                                                <input type="checkbox"
                                                    aria-label="Chọn/Bỏ chọn"
                                                    onkeypress="if (event.keyCode==13) return false;"
                                                    onclick="document.querySelectorAll('.checkbox-table input[type=checkbox]').forEach(cb=>cb.checked=this.checked);" />
                                            </th>
                                            <th class="col-image" style="width:1px;">Ảnh</th>
                                            <th class="text-start" style="min-width: 180px;">Tên sản phẩm</th>
                                            <th class="text-end" style="min-width: 120px;">Giá</th>
                                            <th style="width:1px;" class="text-center">SL</th>
                                            <th class="text-end" style="min-width: 140px;">Thành tiền</th>
                                            <th class="text-start col-shop" style="min-width: 160px;">Shop</th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        <asp:Repeater ID="Repeater1" runat="server">
                                            <ItemTemplate>
                                                <span style="display:none">
                                                    <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                                </span>
                                                <tr class="js-cart-row" data-cart-id="<%# Eval("id") %>">
                                                    <td class="text-center col-id"><%# Eval("id") %></td>

                                                    <td class="checkbox-table text-center">
                                                        <asp:CheckBox ID="checkID" runat="server" CssClass="js-cart-check"
                                                            onkeypress="if (event.keyCode==13) return false;" />
                                                    </td>

                                                    <td class="text-center td-img col-image">
                                                        <img src="<%# Eval("image") %>" width="60" height="60" alt="" />
                                                    </td>

                                                    <td class="text-start">
                                                        <div class="cart-product-inline">
                                                            <div class="cart-product-inline-thumb">
                                                                <img src="<%# ResolveCardImage(Eval("image")) %>" alt="" />
                                                            </div>
                                                            <div class="cart-product-inline-content">
                                                                <a href="/<%# Eval("name_en") %>-<%# Eval("id") %>.html" class="cart-product-inline-name">
                                                                    <%# Eval("name") %>
                                                                </a>
                                                                <div class="cart-product-inline-shop">Shop: <%# Eval("TenShop") %></div>
                                                            </div>
                                                        </div>
                                                    </td>

                                                    <td class="text-end text-price">
                                                        <span class="me-1"><%# Eval("giaban","{0:#,##0}") %> đ</span>

                                                        <asp:PlaceHolder runat="server"
                                                            Visible='<%# Convert.ToInt32(Eval("PhanTramUuDai")) > 0 %>'>
                                                            <span class="badge bg-azure-lt">-<%# Eval("PhanTramUuDai") %>%</span>
                                                        </asp:PlaceHolder>
                                                    </td>

                                                    <td class="text-center">
                                                        <div class="qty-stepper">
                                                            <button type="button" class="qty-step-btn" onclick="ahaQtyAdjust(this,-1)" aria-label="Giảm số lượng">-</button>
                                                            <asp:TextBox onfocus="AutoSelect(this)"
                                                                CssClass="form-control form-control-sm text-center qty-input js-cart-qty"
                                                                oninput="ahaQtySanitize(this)"
                                                                ID="txt_sl_1" MaxLength="3" runat="server"
                                                                Text='<%#Eval("soluong") %>'
                                                                onkeypress="if (event.keyCode==13) return false;"></asp:TextBox>
                                                            <button type="button" class="qty-step-btn" onclick="ahaQtyAdjust(this,1)" aria-label="Tăng số lượng">+</button>
                                                        </div>
                                                    </td>

                                                    <td class="text-end text-line-total">
                                                        <%# Eval("ThanhTien","{0:#,##0}") %> đ
                                                    </td>

                                                    <td class="text-start col-shop"><%# Eval("TenShop") %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>

                                </table>
                            </div>
                        </div>
                    </div>
                    </asp:Panel>

                    <asp:Panel ID="pn_dathang" runat="server" Visible="false" DefaultButton="but_dathang">
                                <section class="checkout-stage">
                                    <div class="checkout-head">
                                        <div>
                                            <div class="checkout-pill">Bước 2/2: Xác nhận đơn hàng</div>
                                            <h3>Trao đổi đơn mua</h3>
                                            <p>Bố cục ưu tiên thông tin quan trọng như Shopee, và CTA nổi bật như TikTok.</p>
                                        </div>
                                        <a href="/home/gio-hang.aspx" class="btn btn-outline-secondary btn-sm" title="Đóng">
                                            <i class="ti ti-x"></i>&nbsp;Quay lại giỏ
                                        </a>
                                    </div>

                                    <div class="checkout-grid">
                                        <div class="card checkout-card">
                                            <div class="card-header d-flex align-items-center justify-content-between">
                                                <div class="fw-bold">Sản phẩm đã chọn</div>
                                                <span class="badge bg-primary-lt"><asp:Label ID="lb_checkout_items" runat="server" Text="0"></asp:Label> SP</span>
                                            </div>
                                            <div class="card-body p-0">
                                                <div class="table-responsive">
                                                    <table class="table table-vcenter card-table mb-0 checkout-table">
                                                        <thead>
                                                            <tr>
                                                                <th class="col-id" style="width:1px;">ID</th>
                                                                <th class="col-image" style="width:1px;">Ảnh</th>
                                                                <th class="text-start" style="min-width: 180px;">Sản phẩm</th>
                                                                <th class="text-end" style="min-width: 120px;">Giá</th>
                                                                <th class="text-center" style="width:1px;">SL</th>
                                                                <th class="text-start col-shop" style="min-width: 160px;">Shop</th>
                                                            </tr>
                                                        </thead>

                                                        <tbody>
                                                            <asp:Repeater ID="Repeater2" runat="server">
                                                                <ItemTemplate>
                                                                    <span style="display:none">
                                                                        <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                                                    </span>
                                                                    <tr class="checkout-line"
                                                                        data-unit-vnd="<%# (Eval("giaban") == null ? "0" : Convert.ToDecimal(Eval("giaban")).ToString(System.Globalization.CultureInfo.InvariantCulture)) %>">
                                                                        <td class="text-center col-id cell-id"><%# Eval("id") %></td>

                                                                        <td class="text-center td-img col-image cell-image">
                                                                            <img src="<%# Eval("image") %>" width="60" height="60" alt="" />
                                                                        </td>

                                                                        <td class="text-start cell-product">
                                                                            <a href="/<%# Eval("name_en") %>-<%# Eval("id") %>.html" class="text-decoration-none fw-semibold">
                                                                                <%# Eval("name") %>
                                                                            </a>
                                                                        </td>

                                                                        <td class="text-end text-price cell-price" data-label="Giá">
                                                                            <span class="me-1"><%# Eval("giaban","{0:#,##0}") %> đ</span>

                                                                            <asp:PlaceHolder runat="server"
                                                                                Visible='<%# Convert.ToInt32(Eval("PhanTramUuDai")) > 0 %>'>
                                                                                <span class="badge bg-azure-lt">-<%# Eval("PhanTramUuDai") %>%</span>
                                                                            </asp:PlaceHolder>
                                                                        </td>

                                                                        <td class="text-center cell-qty" data-label="SL">
                                                                            <div class="qty-stepper">
                                                                                <button type="button" class="qty-step-btn" onclick="ahaQtyAdjust(this,-1)" aria-label="Giảm số lượng">-</button>
                                                                                <asp:TextBox
                                                                                    CssClass="form-control form-control-sm text-center qty-input js-checkout-qty"
                                                                                    oninput="ahaQtySanitize(this)"
                                                                                    ID="txt_sl_2" MaxLength="3" runat="server"
                                                                                    Text='<%#Eval("soluong") %>'
                                                                                    onkeypress="if (event.keyCode==13) return false;"></asp:TextBox>
                                                                                <button type="button" class="qty-step-btn" onclick="ahaQtyAdjust(this,1)" aria-label="Tăng số lượng">+</button>
                                                                            </div>
                                                                        </td>

                                                                        <td class="text-start col-shop cell-shop">Shop: <%# Eval("TenShop") %></td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="card checkout-card">
                                            <div class="card-header fw-bold">Thông tin nhận hàng</div>
                                            <div class="card-body">
                                                <asp:Panel ID="pnl_saved_address" runat="server" CssClass="saved-address-panel" Visible="false">
                                                    <div class="d-flex align-items-center justify-content-between mb-2">
                                                        <label class="form-label mb-0">Chọn địa chỉ đã lưu</label>
                                                        <a class="saved-address-manage" href="/home/dia-chi.aspx">Quản lý địa chỉ</a>
                                                    </div>
                                                    <div class="saved-address-list">
                                                        <asp:Repeater ID="rpt_saved_address" runat="server" OnItemCommand="SavedAddress_ItemCommand">
                                                            <ItemTemplate>
                                                                <div class="saved-address-item">
                                                                    <label class="saved-address-choice">
                                                                        <input type="radio" name="checkout_saved_address" class="saved-address-radio js-addr-pick-checkout" <%# (Eval("IsDefault") != null && Convert.ToBoolean(Eval("IsDefault"))) ? "data-default=\"1\"" : "" %> />
                                                                        <div class="saved-address-content">
                                                                            <div class="saved-address-name">
                                                                                <asp:Label runat="server" Text='<%# Eval("DisplayTitle") %>'></asp:Label>
                                                                                <asp:Label runat="server" CssClass="saved-address-badge" Text="Mặc định"
                                                                                    Visible='<%# (Eval("IsDefault") != null && Convert.ToBoolean(Eval("IsDefault"))) %>'></asp:Label>
                                                                            </div>
                                                                            <div class="saved-address-text">
                                                                                <asp:Label runat="server" Text='<%# Eval("DisplayAddress") %>'></asp:Label>
                                                                            </div>
                                                                        </div>
                                                                        <input type="hidden" class="js-addr-hoten" value='<%# System.Web.HttpUtility.HtmlAttributeEncode((Eval("HoTen") ?? "").ToString()) %>' />
                                                                        <input type="hidden" class="js-addr-sdt" value='<%# System.Web.HttpUtility.HtmlAttributeEncode((Eval("Sdt") ?? "").ToString()) %>' />
                                                                        <input type="hidden" class="js-addr-diachi" value='<%# System.Web.HttpUtility.HtmlAttributeEncode((Eval("DiaChi") ?? "").ToString()) %>' />
                                                                    </label>
                                                                    <div class="saved-address-actions">
                                                                        <asp:LinkButton runat="server" CssClass="saved-address-set-default" CommandName="set-default" CommandArgument='<%# Eval("Id") %>'
                                                                            Visible='<%# !(Eval("IsDefault") != null && Convert.ToBoolean(Eval("IsDefault"))) %>'>Đặt mặc định</asp:LinkButton>
                                                                        <asp:LinkButton runat="server" CssClass="saved-address-delete" CommandName="delete" CommandArgument='<%# Eval("Id") %>'
                                                                            OnClientClick="return confirm('Xoá địa chỉ này?');">Xoá</asp:LinkButton>
                                                                    </div>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                        <div class="saved-address-item saved-address-new">
                                                            <label class="saved-address-choice">
                                                                <input type="radio" name="checkout_saved_address" class="saved-address-radio js-addr-pick-checkout" data-new="1" checked="checked" />
                                                                <div class="saved-address-content">
                                                                    <div class="saved-address-name">Nhập địa chỉ mới</div>
                                                                    <div class="saved-address-text">Bạn có thể chỉnh sửa các ô bên dưới.</div>
                                                                </div>
                                                            </label>
                                                        </div>
                                                    </div>
                                                </asp:Panel>

                                                <div class="mb-3">
                                                    <label class="form-label">Người nhận</label>
                                                    <asp:TextBox ID="txt_hoten_nguoinhan" runat="server" CssClass="form-control checkout-input"></asp:TextBox>
                                                    <div class="field-error" id="err_checkout_hoten"></div>
                                                </div>
                                                <div class="mb-3">
                                                    <label class="form-label">Điện thoại</label>
                                                    <asp:TextBox ID="txt_sdt_nguoinhan" runat="server" CssClass="form-control checkout-input" inputmode="numeric"></asp:TextBox>
                                                    <div class="field-error" id="err_checkout_sdt"></div>
                                                </div>
                                                <div class="mb-3">
                                                    <label class="form-label">Tỉnh/Thành - Quận/Huyện - Phường/Xã</label>
                                                    <div class="row g-2">
                                                        <div class="col-md-4">
                                                            <select id="checkout_tinh" class="form-select checkout-input"></select>
                                                            <div class="field-error" id="err_checkout_tinh"></div>
                                                        </div>
                                                        <div class="col-md-4">
                                                            <select id="checkout_quan" class="form-select checkout-input"></select>
                                                            <div class="field-error" id="err_checkout_quan"></div>
                                                        </div>
                                                        <div class="col-md-4">
                                                            <select id="checkout_phuong" class="form-select checkout-input"></select>
                                                            <div class="field-error" id="err_checkout_phuong"></div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="mb-0">
                                                    <label class="form-label">Địa chỉ chi tiết</label>
                                                    <asp:TextBox ID="txt_diachi_chitiet" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control checkout-input"></asp:TextBox>
                                                    <div class="field-error" id="err_checkout_chitiet"></div>
                                                </div>

                                                <asp:HiddenField ID="hf_tinh" runat="server" />
                                                <asp:HiddenField ID="hf_quan" runat="server" />
                                                <asp:HiddenField ID="hf_phuong" runat="server" />
                                                <asp:HiddenField ID="hf_address_raw" runat="server" />
                                                <asp:TextBox ID="txt_diachi_nguoinhan" runat="server" CssClass="form-control checkout-input d-none" TextMode="MultiLine" Rows="2"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="card checkout-card checkout-summary-card">
                                            <div class="card-header fw-bold">Tóm tắt trao đổi</div>
                                            <div class="card-body">
                                                <div class="checkout-summary m-0">
                                                    <div class="checkout-summary-row">
                                                        <span>Tổng tiền đơn hàng</span>
                                                        <strong><asp:Label ID="lb_checkout_total_vnd" runat="server" Text="0"></asp:Label> đ</strong>
                                                    </div>
                                                    <div class="checkout-summary-row total">
                                                        <span>Tổng cần trao đổi</span>
                                                        <strong><asp:Label ID="lb_checkout_total_a" runat="server" Text="0"></asp:Label> A</strong>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="checkout-actionbar">
                                        <div>
                                            <div class="cta-meta">Kiểm tra thông tin người nhận trước khi xác nhận trao đổi.</div>
                                            <div class="cta-trust">Đơn sau khi tạo sẽ vào trạng thái <b>Đã đặt</b> và có thể theo dõi trong Đơn mua.</div>
                                            <div class="cta-total-wrap">
                                                <div class="cta-vnd"><asp:Label ID="lb_checkout_total_vnd_footer" runat="server" Text="0"></asp:Label> đ</div>
                                                <div class="cta-total"><asp:Label ID="lb_checkout_total_a_footer" runat="server" Text="0"></asp:Label> A</div>
                                            </div>
                                        </div>
                                        <div class="d-flex gap-2 flex-wrap">
                                            <a href="/home/gio-hang.aspx" class="checkout-link-cancel">Quay lại giỏ</a>
                                            <asp:Button ID="but_dathang" OnClick="but_dathang_Click" runat="server"
                                                Text="Xác nhận trao đổi" CssClass="btn btn-success px-4 checkout-btn-main"
                                                OnClientClick="return ahaBeforeCheckoutSubmit(this);" />
                                        </div>
                                    </div>
                                </section>
                    </asp:Panel>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot_sau" runat="Server">
    <script src="<%= Helper_cl.VersionedUrl("~/js/aha-address-picker.js") %>"></script>
    <script>
        function ahaGetBtnText(btn) {
            if (!btn) return "";
            var tag = String(btn.tagName || "").toLowerCase();
            return (tag === "button") ? (btn.textContent || "") : (btn.value || "");
        }

        function ahaSetBtnText(btn, text) {
            if (!btn) return;
            var tag = String(btn.tagName || "").toLowerCase();
            if (tag === "button") {
                btn.textContent = text;
            } else {
                btn.value = text;
            }
        }

        function ahaToAFromVnd(vnd) {
            if (!isFinite(vnd) || vnd <= 0) return 0;
            return Math.ceil((vnd / 1000) * 100) / 100;
        }

        function ahaFormatVnd(value) {
            if (!isFinite(value)) return "0";
            return Math.round(value).toLocaleString("vi-VN");
        }

        function ahaFormatA(value) {
            if (!isFinite(value)) return "0";
            return value.toLocaleString("vi-VN", { minimumFractionDigits: 0, maximumFractionDigits: 2 });
        }

        function ahaCheckoutRecalc() {
            var rows = document.querySelectorAll("tr.checkout-line");
            if (!rows || rows.length === 0) return;

            var totalVnd = 0;
            var totalQty = 0;

            rows.forEach(function (row) {
                var qtyInput = row.querySelector("input.js-checkout-qty");
                if (!qtyInput) return;

                var qty = parseInt(String(qtyInput.value || "").replace(/\D+/g, ""), 10);
                if (isNaN(qty) || qty < 1) qty = 1;
                if (qty > 999) qty = 999;
                qtyInput.value = qty;

                var unitVnd = parseFloat(row.getAttribute("data-unit-vnd") || "0");
                if (isNaN(unitVnd) || unitVnd < 0) unitVnd = 0;

                var lineVnd = unitVnd * qty;
                totalVnd += lineVnd;
                totalQty += qty;

                var lineTotalEl = row.querySelector(".js-checkout-line-total-vnd");
                if (lineTotalEl) {
                    lineTotalEl.textContent = ahaFormatVnd(lineVnd) + " đ";
                }
            });

            var totalA = ahaToAFromVnd(totalVnd);

            var lbTotalVnd = document.getElementById("<%= lb_checkout_total_vnd.ClientID %>");
            if (lbTotalVnd) lbTotalVnd.textContent = ahaFormatVnd(totalVnd);

            var lbTotalA = document.getElementById("<%= lb_checkout_total_a.ClientID %>");
            if (lbTotalA) lbTotalA.textContent = ahaFormatA(totalA);

            var lbTotalAFooter = document.getElementById("<%= lb_checkout_total_a_footer.ClientID %>");
            if (lbTotalAFooter) lbTotalAFooter.textContent = ahaFormatA(totalA);

            var lbTotalVNDFooter = document.getElementById("<%= lb_checkout_total_vnd_footer.ClientID %>");
            if (lbTotalVNDFooter) lbTotalVNDFooter.textContent = ahaFormatVnd(totalVnd);

            var lbItems = document.getElementById("<%= lb_checkout_items.ClientID %>");
            if (lbItems) lbItems.textContent = Math.round(totalQty).toLocaleString("vi-VN");
        }

        function ahaSetFieldState(input, errorEl, message) {
            if (!input || !errorEl) return;
            var isError = !!message;
            if (isError) {
                input.classList.add("is-invalid");
                errorEl.classList.add("show");
                errorEl.textContent = message;
            } else {
                input.classList.remove("is-invalid");
                errorEl.classList.remove("show");
                errorEl.textContent = "";
            }
        }

        function ahaValidatePhone(phone) {
            var digits = String(phone || "").replace(/\D+/g, "");
            return /^0\d{9,10}$/.test(digits);
        }

        function ahaValidateCheckout() {
            var hotenInput = document.getElementById("<%= txt_hoten_nguoinhan.ClientID %>");
            var sdtInput = document.getElementById("<%= txt_sdt_nguoinhan.ClientID %>");
            var detailInput = document.getElementById("<%= txt_diachi_chitiet.ClientID %>");
            var tinhSelect = document.getElementById("checkout_tinh");
            var quanSelect = document.getElementById("checkout_quan");
            var phuongSelect = document.getElementById("checkout_phuong");

            if (!hotenInput || !sdtInput || !detailInput || !tinhSelect || !quanSelect || !phuongSelect) return true;

            var hoten = hotenInput.value.trim();
            var sdt = sdtInput.value.trim();
            var detail = detailInput.value.trim();

            var ok = true;
            var errHoten = document.getElementById("err_checkout_hoten");
            var errSdt = document.getElementById("err_checkout_sdt");
            var errTinh = document.getElementById("err_checkout_tinh");
            var errQuan = document.getElementById("err_checkout_quan");
            var errPhuong = document.getElementById("err_checkout_phuong");
            var errChiTiet = document.getElementById("err_checkout_chitiet");

            if (hoten.length < 2) {
                ahaSetFieldState(hotenInput, errHoten, "Vui lòng nhập họ tên người nhận.");
                ok = false;
            } else {
                ahaSetFieldState(hotenInput, errHoten, "");
            }

            if (!ahaValidatePhone(sdt)) {
                ahaSetFieldState(sdtInput, errSdt, "Số điện thoại không hợp lệ.");
                ok = false;
            } else {
                ahaSetFieldState(sdtInput, errSdt, "");
            }

            if (!tinhSelect.value) {
                ahaSetFieldState(tinhSelect, errTinh, "Chọn Tỉnh/Thành.");
                ok = false;
            } else {
                ahaSetFieldState(tinhSelect, errTinh, "");
            }

            if (!quanSelect.value) {
                ahaSetFieldState(quanSelect, errQuan, "Chọn Quận/Huyện.");
                ok = false;
            } else {
                ahaSetFieldState(quanSelect, errQuan, "");
            }

            if (!phuongSelect.value) {
                ahaSetFieldState(phuongSelect, errPhuong, "Chọn Phường/Xã.");
                ok = false;
            } else {
                ahaSetFieldState(phuongSelect, errPhuong, "");
            }

            if (detail.length < 4) {
                ahaSetFieldState(detailInput, errChiTiet, "Vui lòng nhập địa chỉ chi tiết.");
                ok = false;
            } else {
                ahaSetFieldState(detailInput, errChiTiet, "");
            }

            return ok;
        }

        function ahaQtySanitize(input) {
            if (!input) return;
            var raw = String(input.value || "").replace(/\D+/g, "");
            var qty = parseInt(raw || "1", 10);
            if (isNaN(qty) || qty < 1) qty = 1;
            if (qty > 999) qty = 999;
            input.value = qty;
            if (input.classList.contains("js-checkout-qty")) {
                ahaCheckoutRecalc();
                var row = input.closest("tr.checkout-line");
                if (row) {
                    row.classList.add("is-updated");
                    setTimeout(function () { row.classList.remove("is-updated"); }, 380);
                }
            }
        }

        function ahaQtyAdjust(btn, step) {
            var holder = btn ? btn.closest(".qty-stepper") : null;
            if (!holder) return;
            var input = holder.querySelector("input");
            if (!input) return;

            var current = parseInt(String(input.value || "").replace(/\D+/g, ""), 10);
            if (isNaN(current) || current < 1) current = 1;
            current += step;
            if (current < 1) current = 1;
            if (current > 999) current = 999;
            input.value = current;
            input.dispatchEvent(new Event("input", { bubbles: true }));
        }

        function ahaBindCheckoutInputValidation() {
            var hotenInput = document.getElementById("<%= txt_hoten_nguoinhan.ClientID %>");
            var sdtInput = document.getElementById("<%= txt_sdt_nguoinhan.ClientID %>");
            var detailInput = document.getElementById("<%= txt_diachi_chitiet.ClientID %>");
            var tinhSelect = document.getElementById("checkout_tinh");
            var quanSelect = document.getElementById("checkout_quan");
            var phuongSelect = document.getElementById("checkout_phuong");
            if (!hotenInput || !sdtInput || !detailInput || !tinhSelect || !quanSelect || !phuongSelect) return;

            if (hotenInput.dataset.boundValidation === "1") return;
            hotenInput.dataset.boundValidation = "1";
            sdtInput.dataset.boundValidation = "1";
            detailInput.dataset.boundValidation = "1";

            hotenInput.addEventListener("input", function () {
                if (hotenInput.value.trim().length >= 2) {
                    ahaSetFieldState(hotenInput, document.getElementById("err_checkout_hoten"), "");
                }
            });
            sdtInput.addEventListener("input", function () {
                if (ahaValidatePhone(sdtInput.value.trim())) {
                    ahaSetFieldState(sdtInput, document.getElementById("err_checkout_sdt"), "");
                }
            });
            detailInput.addEventListener("input", function () {
                if (detailInput.value.trim().length >= 4) {
                    ahaSetFieldState(detailInput, document.getElementById("err_checkout_chitiet"), "");
                }
            });
            tinhSelect.addEventListener("change", function () {
                if (tinhSelect.value) {
                    ahaSetFieldState(tinhSelect, document.getElementById("err_checkout_tinh"), "");
                }
            });
            quanSelect.addEventListener("change", function () {
                if (quanSelect.value) {
                    ahaSetFieldState(quanSelect, document.getElementById("err_checkout_quan"), "");
                }
            });
            phuongSelect.addEventListener("change", function () {
                if (phuongSelect.value) {
                    ahaSetFieldState(phuongSelect, document.getElementById("err_checkout_phuong"), "");
                }
            });
        }

        function ahaBeforeCheckoutSubmit(btn) {
            if (!ahaValidateCheckout()) {
                return false;
            }
            if (!btn) return true;
            if (btn.dataset.loading === "1") return false;

            btn.dataset.loading = "1";
            btn.classList.add("is-loading");
            btn.dataset.originalText = btn.value;
            btn.value = "Đang xử lý...";
            return true;
        }

        function ahaGoCheckoutStep2(btn) {
            var rows = document.querySelectorAll("tr.js-cart-row");
            var selectedIds = [];
            var qtyPairs = [];

            rows.forEach(function (row) {
                if (!row) return;
                var chk = row.querySelector("input.js-cart-check, input[type='checkbox']");
                if (!chk || !chk.checked) return;

                var cartId = String(row.getAttribute("data-cart-id") || "").trim();
                if (!/^\d+$/.test(cartId)) return;

                var qtyInput = row.querySelector("input.js-cart-qty");
                var qty = parseInt(String(qtyInput && qtyInput.value ? qtyInput.value : "").replace(/\D+/g, ""), 10);
                if (isNaN(qty) || qty < 1) qty = 1;
                if (qty > 999) qty = 999;
                if (qtyInput) qtyInput.value = qty;

                selectedIds.push(cartId);
                qtyPairs.push(cartId + "-" + qty);
            });

            if (selectedIds.length === 0) {
                alert("Không có mục nào được chọn.");
                return false;
            }

            if (btn) {
                if (btn.dataset.loading === "1") return false;
                btn.dataset.loading = "1";
                btn.dataset.originalText = ahaGetBtnText(btn);
                ahaSetBtnText(btn, "Đang mở bước 2...");
                btn.disabled = true;
                btn.classList.add("is-loading");
            }

            var url = "/home/gio-hang.aspx?step=2&sel="
                + encodeURIComponent(selectedIds.join(","))
                + "&qty=" + encodeURIComponent(qtyPairs.join(","));
            window.location.href = url;
            return false;
        }

        function ahaResetCheckoutSubmitState() {
            var btn = document.getElementById("<%= but_dathang.ClientID %>");
            if (!btn) return;
            btn.dataset.loading = "0";
            btn.disabled = false;
            btn.classList.remove("is-loading");
            if (btn.dataset.originalText) {
                ahaSetBtnText(btn, btn.dataset.originalText);
            }
        }

        function ahaResetOpenCheckoutButton() {
            var btn = document.getElementById("btn_go_step2");
            if (!btn) return;
            btn.dataset.loading = "0";
            btn.disabled = false;
            btn.classList.remove("is-loading");
            if (btn.dataset.originalText) {
                ahaSetBtnText(btn, btn.dataset.originalText);
            }
        }

        function ahaBindSavedAddressPicker() {
            var radios = document.querySelectorAll(".js-addr-pick-checkout");
            if (!radios.length) return;

            var hotenInput = document.getElementById("<%= txt_hoten_nguoinhan.ClientID %>");
            var sdtInput = document.getElementById("<%= txt_sdt_nguoinhan.ClientID %>");
            var detailInput = document.getElementById("<%= txt_diachi_chitiet.ClientID %>");
            var rawInput = document.getElementById("<%= hf_address_raw.ClientID %>");

            function clearSelected() {
                document.querySelectorAll(".saved-address-item.is-selected").forEach(function (el) {
                    el.classList.remove("is-selected");
                });
            }

            radios.forEach(function (radio) {
                radio.addEventListener("change", function () {
                    if (!radio.checked) return;
                    var item = radio.closest(".saved-address-item");
                    if (!item) return;

                    clearSelected();
                    item.classList.add("is-selected");

                    if (radio.getAttribute("data-new") === "1") {
                        return;
                    }

                    var hoten = item.querySelector(".js-addr-hoten");
                    var sdt = item.querySelector(".js-addr-sdt");
                    var diachi = item.querySelector(".js-addr-diachi");

                    if (hotenInput && hoten) hotenInput.value = hoten.value;
                    if (sdtInput && sdt) sdtInput.value = sdt.value;

                    var rawAddress = diachi ? diachi.value : "";
                    if (rawInput) rawInput.value = rawAddress;

                    if (window.AhaAddressPicker && rawAddress) {
                        window.AhaAddressPicker.applyRaw({
                            provinceSelectId: "checkout_tinh",
                            districtSelectId: "checkout_quan",
                            wardSelectId: "checkout_phuong",
                            detailInputId: "<%= txt_diachi_chitiet.ClientID %>",
                            rawAddressId: "<%= hf_address_raw.ClientID %>"
                        }, rawAddress);
                    } else if (detailInput && rawAddress) {
                        detailInput.value = rawAddress;
                    }

                    if (typeof ahaSetFieldState === "function") {
                        if (hotenInput) ahaSetFieldState(hotenInput, document.getElementById("err_checkout_hoten"), "");
                        if (sdtInput) ahaSetFieldState(sdtInput, document.getElementById("err_checkout_sdt"), "");
                        if (detailInput) ahaSetFieldState(detailInput, document.getElementById("err_checkout_chitiet"), "");
                        var tinhSelect = document.getElementById("checkout_tinh");
                        var quanSelect = document.getElementById("checkout_quan");
                        var phuongSelect = document.getElementById("checkout_phuong");
                        if (tinhSelect) ahaSetFieldState(tinhSelect, document.getElementById("err_checkout_tinh"), "");
                        if (quanSelect) ahaSetFieldState(quanSelect, document.getElementById("err_checkout_quan"), "");
                        if (phuongSelect) ahaSetFieldState(phuongSelect, document.getElementById("err_checkout_phuong"), "");
                    }
                });
            });

            var defaultRadio = document.querySelector(".js-addr-pick-checkout[data-default='1']");
            if (defaultRadio) {
                defaultRadio.checked = true;
                defaultRadio.dispatchEvent(new Event("change", { bubbles: true }));
                return;
            }

            var checked = document.querySelector(".js-addr-pick-checkout:checked");
            if (checked) {
                var item = checked.closest(".saved-address-item");
                if (item) item.classList.add("is-selected");
            }
        }

        document.addEventListener("DOMContentLoaded", function () {
            ahaCheckoutRecalc();
            ahaBindCheckoutInputValidation();
            ahaResetOpenCheckoutButton();
            if (window.AhaAddressPicker) {
                window.AhaAddressPicker.init({
                    provinceSelectId: "checkout_tinh",
                    districtSelectId: "checkout_quan",
                    wardSelectId: "checkout_phuong",
                    detailInputId: "<%= txt_diachi_chitiet.ClientID %>",
                    hiddenAddressId: "<%= txt_diachi_nguoinhan.ClientID %>",
                    hiddenProvinceId: "<%= hf_tinh.ClientID %>",
                    hiddenDistrictId: "<%= hf_quan.ClientID %>",
                    hiddenWardId: "<%= hf_phuong.ClientID %>",
                    rawAddressId: "<%= hf_address_raw.ClientID %>"
                });
            }
            ahaBindSavedAddressPicker();
        });

        if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                ahaCheckoutRecalc();
                ahaBindCheckoutInputValidation();
                ahaResetCheckoutSubmitState();
                ahaResetOpenCheckoutButton();
                if (window.AhaAddressPicker) {
                    window.AhaAddressPicker.init({
                        provinceSelectId: "checkout_tinh",
                        districtSelectId: "checkout_quan",
                        wardSelectId: "checkout_phuong",
                        detailInputId: "<%= txt_diachi_chitiet.ClientID %>",
                        hiddenAddressId: "<%= txt_diachi_nguoinhan.ClientID %>",
                        hiddenProvinceId: "<%= hf_tinh.ClientID %>",
                        hiddenDistrictId: "<%= hf_quan.ClientID %>",
                        hiddenWardId: "<%= hf_phuong.ClientID %>",
                        rawAddressId: "<%= hf_address_raw.ClientID %>"
                    });
                }
                ahaBindSavedAddressPicker();
            });
        }
    </script>
</asp:Content>
