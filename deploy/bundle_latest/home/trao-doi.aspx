<%@ Page Title="Trao đổi sản phẩm" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="trao-doi.aspx.cs" Inherits="home_trao_doi" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        :root {
            --pay-page-bg: #f5f7fb;
            --pay-card: #ffffff;
            --pay-border: rgba(98, 105, 118, 0.2);
            --pay-text: #182433;
            --pay-muted: #64748b;
            --pay-danger: #d63939;
            --pay-primary: #ff5b2e;
            --pay-success: #16a34a;
            --pay-success-soft: #eafaf0;
        }

        html[data-bs-theme="dark"] {
            --pay-page-bg: #0b1220;
            --pay-card: #0f172a;
            --pay-border: #223246;
            --pay-text: #e5e7eb;
            --pay-muted: #94a3b8;
            --pay-danger: #f87171;
            --pay-primary: #ff7a47;
            --pay-success: #22c55e;
            --pay-success-soft: rgba(34, 197, 94, 0.14);
        }

        .exchange-page {
            background: var(--pay-page-bg);
            border: 1px solid var(--pay-border);
            border-radius: 20px;
            padding: 16px;
        }

        .exchange-top {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 12px;
            margin-bottom: 14px;
            flex-wrap: wrap;
        }

        .exchange-step {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            font-size: 12px;
            font-weight: 700;
            padding: 6px 11px;
            border-radius: 999px;
            color: #6b3700;
            background: #fff3db;
            border: 1px solid #ffd38a;
            margin-bottom: 8px;
        }

        html[data-bs-theme="dark"] .exchange-step {
            color: #fcd34d;
            background: rgba(245, 158, 11, 0.16);
            border-color: rgba(245, 158, 11, 0.4);
        }

        .exchange-title {
            margin: 0;
            font-size: 30px;
            line-height: 1.2;
            font-weight: 800;
            color: var(--pay-text);
        }

        .exchange-sub {
            margin: 6px 0 0;
            color: var(--pay-muted);
            font-size: 14px;
        }

        .exchange-layout {
            display: grid;
            grid-template-columns: minmax(0, 1.05fr) minmax(0, 1.25fr);
            gap: 14px;
        }

        .exchange-card {
            border: 1px solid var(--pay-border);
            border-radius: 16px;
            background: var(--pay-card);
            box-shadow: 0 10px 25px rgba(15, 23, 42, 0.06);
            overflow: hidden;
        }

        .exchange-card .card-header {
            background: #fff;
            border-bottom: 1px solid var(--pay-border);
            padding: 12px 14px;
        }

        .exchange-card .card-body {
            padding: 14px;
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
            border: 1px solid var(--pay-border);
            border-radius: 12px;
            background: var(--pay-card);
            transition: border-color 0.2s ease, box-shadow 0.2s ease;
        }

        .saved-address-item.is-selected {
            border-color: var(--pay-primary);
            box-shadow: 0 0 0 2px rgba(255, 91, 46, 0.15);
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
            color: var(--pay-text);
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .saved-address-text {
            font-size: 0.95rem;
            color: var(--pay-muted);
            white-space: pre-line;
        }

        .saved-address-new .saved-address-name {
            color: var(--pay-primary);
        }

        .saved-address-badge {
            display: inline-flex;
            align-items: center;
            padding: 2px 8px;
            border-radius: 999px;
            font-size: 11px;
            font-weight: 700;
            background: var(--pay-success-soft);
            color: var(--pay-success);
        }

        .saved-address-delete {
            border: none;
            background: transparent;
            color: var(--pay-danger);
            font-size: 0.85rem;
            font-weight: 600;
            text-decoration: underline;
            cursor: pointer;
        }

        .saved-address-set-default {
            border: none;
            background: transparent;
            color: var(--pay-primary);
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
            color: var(--pay-primary);
            text-decoration: none;
        }

        .product-media {
            width: 100%;
            max-height: 340px;
            object-fit: cover;
            border-radius: 14px;
            border: 1px solid var(--pay-border);
            background: #fff;
        }

        .product-tag {
            margin-top: 12px;
            display: inline-flex;
            align-items: center;
            font-size: 12px;
            font-weight: 700;
            color: #0f172a;
            background: #eef2ff;
            border: 1px solid #dbe2ff;
            border-radius: 999px;
            padding: 4px 10px;
        }

        .product-title {
            margin: 8px 0 0;
            font-size: 28px;
            line-height: 1.2;
            color: var(--pay-text);
            font-weight: 800;
        }

        .summary-box {
            border: 1px solid var(--pay-border);
            border-radius: 14px;
            background: #fdfefe;
            padding: 10px 12px;
            margin-top: 12px;
        }

        .summary-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            padding: 8px 0;
            font-size: 14px;
            color: var(--pay-muted);
            border-bottom: 1px dashed rgba(100, 116, 139, 0.25);
        }

        .summary-row:last-child {
            border-bottom: 0;
        }

        .summary-row .value {
            font-size: 20px;
            line-height: 1.1;
            font-weight: 800;
            color: var(--pay-text);
            white-space: nowrap;
        }

        .summary-row .value.vnd {
            color: var(--pay-danger);
        }

        .summary-row .value.a {
            color: #0f4a84;
        }

        .summary-row .value.percent {
            color: #b45309;
        }

        .qty-label,
        .receiver-label {
            font-size: 14px;
            font-weight: 700;
            color: var(--pay-text);
            margin-bottom: 8px;
        }

        .qty-input {
            border-radius: 0;
            font-size: 26px;
            font-weight: 800;
            padding: 0 !important;
            height: 40px;
            border: 0 !important;
            box-shadow: none !important;
            background: transparent !important;
            color: #111827 !important;
            text-align: center;
        }

        .qty-stepper {
            display: inline-flex;
            align-items: center;
            gap: 7px;
            border: 1px solid rgba(98, 105, 118, 0.3);
            border-radius: 999px;
            background: #fff;
            padding: 4px 8px;
            min-width: 146px;
        }

        .qty-step-btn {
            width: 32px;
            height: 32px;
            border-radius: 999px;
            border: 1px solid rgba(98, 105, 118, 0.35);
            background: #f8fafc;
            color: #182433;
            font-size: 20px;
            font-weight: 800;
            line-height: 1;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            justify-content: center;
        }

        .receiver-input {
            border-radius: 12px;
            padding: 11px 12px;
            font-size: 16px;
            font-weight: 600;
        }

        .receiver-input.is-invalid {
            border-color: rgba(220, 38, 38, 0.65) !important;
            box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.1) !important;
        }

        .field-error {
            margin-top: 6px;
            font-size: 12px;
            color: #dc2626;
            line-height: 1.35;
            display: none;
        }

        .field-error.show {
            display: block;
        }

        .exchange-paybar {
            border: 1px solid var(--pay-border);
            border-radius: 14px;
            background: #fff;
            padding: 10px 12px;
            margin-top: 14px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
            position: sticky;
            bottom: 8px;
            z-index: 18;
            box-shadow: 0 12px 30px rgba(15, 23, 42, 0.14);
        }

        .exchange-paybar .meta {
            color: var(--pay-muted);
            font-size: 13px;
            font-weight: 600;
        }

        .exchange-paybar .trust {
            margin-top: 3px;
            color: #065f46;
            font-size: 12px;
            font-weight: 600;
        }

        .exchange-paybar .totals {
            margin-top: 4px;
            display: flex;
            align-items: center;
            flex-wrap: wrap;
            gap: 10px;
        }

        .exchange-paybar .total-vnd {
            font-size: 16px;
            font-weight: 800;
            color: #d63939;
        }

        .exchange-paybar .total {
            font-size: 28px;
            line-height: 1.1;
            color: var(--pay-success);
            font-weight: 800;
            display: inline-flex;
            align-items: center;
            gap: 7px;
            background: var(--pay-success-soft);
            border: 1px solid #bbf0cd;
            border-radius: 12px;
            padding: 4px 10px;
        }

        html[data-bs-theme="dark"] .exchange-card {
            box-shadow: 0 12px 30px rgba(0, 0, 0, 0.45);
        }

        html[data-bs-theme="dark"] .exchange-card .card-header {
            background: #111d34;
            color: var(--pay-text);
        }

        html[data-bs-theme="dark"] .product-media {
            background: #0b1220;
        }

        html[data-bs-theme="dark"] .product-tag {
            color: #e5e7eb;
            background: rgba(59, 130, 246, 0.16);
            border-color: rgba(59, 130, 246, 0.3);
        }

        html[data-bs-theme="dark"] .summary-box {
            background: #111d34;
        }

        html[data-bs-theme="dark"] .summary-row .value.a {
            color: #7dd3fc;
        }

        html[data-bs-theme="dark"] .summary-row .value.percent {
            color: #fcd34d;
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

        html[data-bs-theme="dark"] .exchange-paybar {
            background: #0f172a;
            box-shadow: 0 12px 30px rgba(0, 0, 0, 0.45);
        }

        html[data-bs-theme="dark"] .exchange-paybar .trust {
            color: #6ee7b7;
        }

        html[data-bs-theme="dark"] .exchange-paybar .total {
            border-color: rgba(34, 197, 94, 0.4);
        }

        .exchange-paybar .btn-primary,
        .exchange-paybar .btn-primary:focus {
            background: linear-gradient(135deg, var(--pay-primary), #ff7a47);
            border-color: #ff6a39;
            font-weight: 700;
        }

        .exchange-paybar .btn-primary.is-loading {
            opacity: .92;
            pointer-events: none;
            cursor: wait;
        }

        .exchange-paybar .btn-primary.is-loading::after {
            content: "";
            width: 14px;
            height: 14px;
            border-radius: 50%;
            border: 2px solid rgba(255, 255, 255, 0.45);
            border-top-color: #fff;
            display: inline-block;
            margin-left: 8px;
            vertical-align: -2px;
            animation: exchangeSpin .75s linear infinite;
        }

        .exchange-link-cancel {
            font-size: 14px;
            font-weight: 600;
            color: var(--pay-muted);
            text-decoration: none;
        }

        .exchange-link-cancel:hover {
            text-decoration: underline;
        }

        @keyframes exchangeSpin {
            to { transform: rotate(360deg); }
        }

        @media (max-width: 991.98px) {
            .exchange-page {
                padding: 12px;
                border-radius: 14px;
            }

            .exchange-title {
                font-size: 24px;
            }

            .exchange-layout {
                grid-template-columns: 1fr;
            }

            .product-title {
                font-size: 22px;
            }

            .summary-row .value {
                font-size: 18px;
            }

            .qty-input {
                font-size: 22px;
                width: 54px;
            }

            .qty-stepper {
                min-width: 130px;
                gap: 5px;
                padding: 3px 6px;
            }

            .qty-step-btn {
                width: 28px;
                height: 28px;
            }

            .exchange-paybar {
                bottom: max(10px, env(safe-area-inset-bottom));
                box-shadow: 0 12px 30px rgba(15, 23, 42, 0.16);
            }

            .exchange-paybar .total {
                font-size: 22px;
            }

            .exchange-paybar .total-vnd {
                font-size: 14px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-body py-4">
        <div class="container-xl">
            <div class="exchange-page">
                <div class="exchange-top">
                    <div>
                        <div class="exchange-step"><asp:Label ID="lb_page_step" runat="server" Text="Bước cuối: Xác nhận trao đổi"></asp:Label></div>
                        <h2 class="exchange-title"><asp:Label ID="lb_page_title" runat="server" Text="Trao đổi đơn mua"></asp:Label></h2>
                        <p class="exchange-sub"><asp:Label ID="lb_page_sub" runat="server" Text="Trình bày ưu tiên rõ thông tin như Shopee, hành động xác nhận nổi bật như TikTok."></asp:Label></p>
                    </div>
                    <asp:HyperLink ID="hl_back_top" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                        <i class="ti ti-arrow-left"></i>&nbsp;Quay lại
                    </asp:HyperLink>
                </div>

                <div class="exchange-layout">
                    <div class="exchange-card">
                        <div class="card-header fw-bold"><asp:Label ID="lb_item_card_header" runat="server" Text="Sản phẩm"></asp:Label></div>
                        <div class="card-body">
                            <img id="img_product" runat="server" class="product-media" alt="Sản phẩm" src="/uploads/images/macdinh.jpg" />
                            <div class="product-tag"><asp:Label ID="lb_item_tag" runat="server" Text="Đang mua ngay"></asp:Label></div>
                            <h3 class="product-title">
                                <asp:Label ID="lb_ten_sp" runat="server" Text="-"></asp:Label>
                            </h3>

                            <div class="summary-box">
                                <div class="summary-row">
                                    <span>Giá sản phẩm</span>
                                    <span class="value vnd"><asp:Label ID="lb_gia_vnd" runat="server" Text="0"></asp:Label> đ</span>
                                </div>
                                <div class="summary-row">
                                    <span>Giá quy đổi</span>
                                    <span class="value a"><asp:Label ID="lb_gia_a" runat="server" Text="0"></asp:Label> A</span>
                                </div>
                                <div class="summary-row">
                                    <span>Ưu đãi eVoucher</span>
                                    <span class="value percent"><asp:Label ID="lb_uu_dai" runat="server" Text="0"></asp:Label>%</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="d-flex flex-column gap-3">
                        <div class="exchange-card">
                            <div class="card-header fw-bold">Thông tin đơn hàng</div>
                            <div class="card-body">
                                <asp:PlaceHolder ID="ph_qty_section" runat="server">
                                    <div class="mb-3">
                                        <label class="form-label qty-label">Số lượng</label>
                                        <div class="qty-stepper">
                                            <button type="button" class="qty-step-btn" onclick="ahaQtyAdjust(this,-1)" aria-label="Giảm số lượng">-</button>
                                            <asp:TextBox ID="txt_soluong" runat="server" CssClass="form-control qty-input js-exchange-qty" MaxLength="3"
                                                inputmode="numeric"
                                                onfocus="AutoSelect(this)" oninput="ahaQtySanitize(this)"></asp:TextBox>
                                            <button type="button" class="qty-step-btn" onclick="ahaQtyAdjust(this,1)" aria-label="Tăng số lượng">+</button>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>

                                <div class="summary-box mb-0">
                                    <div class="summary-row">
                                        <span>Tổng tiền (VNĐ)</span>
                                        <span class="value vnd"><asp:Label ID="lb_tong_vnd" runat="server" Text="0"></asp:Label> đ</span>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="exchange-card">
                            <div class="card-header fw-bold"><asp:Label ID="lb_receiver_card_title" runat="server" Text="Thông tin nhận hàng"></asp:Label></div>
                            <div class="card-body">
                                <asp:PlaceHolder ID="ph_service_booking" runat="server" Visible="false">
                                    <div class="mb-3">
                                        <label class="form-label receiver-label">Chọn lịch trống</label>
                                        <asp:TextBox ID="txt_service_datetime" runat="server" CssClass="form-control receiver-input"></asp:TextBox>
                                    </div>
                                    <div class="mb-3">
                                        <label class="form-label receiver-label">Thời lượng (phút)</label>
                                        <asp:TextBox ID="txt_service_duration" runat="server" CssClass="form-control receiver-input" MaxLength="3"></asp:TextBox>
                                        <div class="form-hint">Tối thiểu 15 phút, tối đa 480 phút.</div>
                                    </div>
                                    <div class="mb-3">
                                        <label class="form-label receiver-label">Cơ sở phục vụ</label>
                                        <asp:DropDownList ID="ddl_service_branch" runat="server" CssClass="form-select receiver-input"></asp:DropDownList>
                                    </div>
                                    <div class="mb-0">
                                        <label class="form-label receiver-label">Ghi chú cho gian hàng đối tác (tuỳ chọn)</label>
                                        <asp:TextBox ID="txt_service_note" runat="server" CssClass="form-control receiver-input" TextMode="MultiLine" Rows="2"></asp:TextBox>
                                    </div>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="ph_product_address" runat="server">
                                    <asp:Panel ID="pnl_saved_address" runat="server" CssClass="saved-address-panel" Visible="false">
                                        <div class="d-flex align-items-center justify-content-between mb-2">
                                            <label class="form-label receiver-label mb-0">Chọn địa chỉ đã lưu</label>
                                            <a class="saved-address-manage" href="/home/dia-chi.aspx">Quản lý địa chỉ</a>
                                        </div>
                                        <div class="saved-address-list">
                                            <asp:Repeater ID="rpt_saved_address" runat="server" OnItemCommand="SavedAddress_ItemCommand">
                                                <ItemTemplate>
                                                    <div class="saved-address-item">
                                                        <label class="saved-address-choice">
                                                            <input type="radio" name="exchange_saved_address" class="saved-address-radio js-addr-pick-exchange" <%# (Eval("IsDefault") != null && Convert.ToBoolean(Eval("IsDefault"))) ? "data-default=\"1\"" : "" %> />
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
                                                    <input type="radio" name="exchange_saved_address" class="saved-address-radio js-addr-pick-exchange" data-new="1" checked="checked" />
                                                    <div class="saved-address-content">
                                                        <div class="saved-address-name">Nhập địa chỉ mới</div>
                                                        <div class="saved-address-text">Bạn có thể chỉnh sửa các ô bên dưới.</div>
                                                    </div>
                                                </label>
                                            </div>
                                        </div>
                                    </asp:Panel>

                                    <div class="mb-3">
                                        <label class="form-label receiver-label">Người nhận</label>
                                        <asp:TextBox ID="txt_hoten_nguoinhan" runat="server" CssClass="form-control receiver-input"></asp:TextBox>
                                        <div class="field-error" id="err_exchange_hoten"></div>
                                    </div>

                                    <div class="mb-3">
                                        <label class="form-label receiver-label">Điện thoại</label>
                                        <asp:TextBox ID="txt_sdt_nguoinhan" runat="server" CssClass="form-control receiver-input" inputmode="numeric"></asp:TextBox>
                                        <div class="field-error" id="err_exchange_sdt"></div>
                                    </div>

                                    <div class="mb-3">
                                        <label class="form-label receiver-label">Tỉnh/Thành - Quận/Huyện - Phường/Xã</label>
                                        <div class="row g-2">
                                            <div class="col-md-4">
                                                <select id="exchange_tinh" class="form-select receiver-input"></select>
                                                <div class="field-error" id="err_exchange_tinh"></div>
                                            </div>
                                            <div class="col-md-4">
                                                <select id="exchange_quan" class="form-select receiver-input"></select>
                                                <div class="field-error" id="err_exchange_quan"></div>
                                            </div>
                                            <div class="col-md-4">
                                                <select id="exchange_phuong" class="form-select receiver-input"></select>
                                                <div class="field-error" id="err_exchange_phuong"></div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="mb-0">
                                        <label class="form-label receiver-label">Địa chỉ chi tiết</label>
                                        <asp:TextBox ID="txt_diachi_chitiet" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control receiver-input"></asp:TextBox>
                                        <div class="field-error" id="err_exchange_chitiet"></div>
                                    </div>

                                    <asp:HiddenField ID="hf_tinh" runat="server" />
                                    <asp:HiddenField ID="hf_quan" runat="server" />
                                    <asp:HiddenField ID="hf_phuong" runat="server" />
                                    <asp:HiddenField ID="hf_address_raw" runat="server" />
                                    <asp:TextBox ID="txt_diachi_nguoinhan" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control receiver-input d-none"></asp:TextBox>
                                </asp:PlaceHolder>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="exchange-paybar">
                    <div>
                        <div class="meta">Tổng cần trao đổi</div>
                        <div class="trust">Đơn xác nhận thành công sẽ được ghi nhận trạng thái <b>Đã đặt</b>.</div>
                        <div class="totals">
                            <div class="total-vnd"><asp:Label ID="lb_tong_vnd_footer" runat="server" Text="0"></asp:Label> đ</div>
                            <div class="total"><asp:Label ID="lb_tong_a" runat="server" Text="0"></asp:Label> A</div>
                        </div>
                    </div>
                    <div class="d-flex align-items-center gap-2">
                        <asp:HyperLink ID="hl_back_bottom" runat="server" CssClass="exchange-link-cancel">Quay lại</asp:HyperLink>
                        <asp:Button ID="but_xacnhan" runat="server" CssClass="btn btn-primary" Text="Xác nhận trao đổi" OnClick="but_xacnhan_Click" OnClientClick="return ahaBeforeExchangeSubmit(this);" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hf_is_service_mode" runat="server" />
</asp:Content>

<asp:Content ID="ContentFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server"></asp:Content>
<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="Server">
    <script src="<%= Helper_cl.VersionedUrl("~/js/aha-address-picker.js") %>"></script>
    <script>
        function ahaParseVndInt(str) {
            var digits = String(str || "").replace(/\D+/g, "");
            var num = parseInt(digits || "0", 10);
            return isNaN(num) ? 0 : num;
        }

        function ahaFormatVnd(value) {
            if (!isFinite(value)) return "0";
            return Math.round(value).toLocaleString("vi-VN");
        }

        function ahaFormatA(value) {
            if (!isFinite(value)) return "0";
            return value.toLocaleString("vi-VN", { minimumFractionDigits: 0, maximumFractionDigits: 2 });
        }

        function ahaToAFromVnd(vnd) {
            if (!isFinite(vnd) || vnd <= 0) return 0;
            return Math.ceil((vnd / 1000) * 100) / 100;
        }

        function ahaExchangeRecalc() {
            var qtyInput = document.getElementById("<%= txt_soluong.ClientID %>");
            var unitVndLabel = document.getElementById("<%= lb_gia_vnd.ClientID %>");
            var totalVndLabel = document.getElementById("<%= lb_tong_vnd.ClientID %>");
            var totalVndFooterLabel = document.getElementById("<%= lb_tong_vnd_footer.ClientID %>");
            var totalALabel = document.getElementById("<%= lb_tong_a.ClientID %>");
            if (!qtyInput || !unitVndLabel || !totalVndLabel || !totalALabel) return;

            var qty = parseInt(String(qtyInput.value || "").replace(/\D+/g, ""), 10);
            if (isNaN(qty) || qty < 1) qty = 1;
            if (qty > 999) qty = 999;
            qtyInput.value = qty;

            var unitVnd = ahaParseVndInt(unitVndLabel.textContent);
            var totalVnd = unitVnd * qty;
            var totalA = ahaToAFromVnd(totalVnd);

            totalVndLabel.textContent = ahaFormatVnd(totalVnd);
            if (totalVndFooterLabel) totalVndFooterLabel.textContent = ahaFormatVnd(totalVnd);
            totalALabel.textContent = ahaFormatA(totalA);
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

        function ahaIsServiceMode() {
            var hf = document.getElementById("<%= hf_is_service_mode.ClientID %>");
            return hf && hf.value === "1";
        }

        function ahaValidateExchange() {
            if (ahaIsServiceMode()) return true;

            var hotenInput = document.getElementById("<%= txt_hoten_nguoinhan.ClientID %>");
            var sdtInput = document.getElementById("<%= txt_sdt_nguoinhan.ClientID %>");
            var detailInput = document.getElementById("<%= txt_diachi_chitiet.ClientID %>");
            var tinhSelect = document.getElementById("exchange_tinh");
            var quanSelect = document.getElementById("exchange_quan");
            var phuongSelect = document.getElementById("exchange_phuong");
            if (!hotenInput || !sdtInput || !detailInput || !tinhSelect || !quanSelect || !phuongSelect) return true;

            var hoten = hotenInput.value.trim();
            var sdt = sdtInput.value.trim();
            var detail = detailInput.value.trim();

            var ok = true;
            if (hoten.length < 2) {
                ahaSetFieldState(hotenInput, document.getElementById("err_exchange_hoten"), "Vui lòng nhập họ tên người nhận.");
                ok = false;
            } else {
                ahaSetFieldState(hotenInput, document.getElementById("err_exchange_hoten"), "");
            }

            if (!ahaValidatePhone(sdt)) {
                ahaSetFieldState(sdtInput, document.getElementById("err_exchange_sdt"), "Số điện thoại không hợp lệ.");
                ok = false;
            } else {
                ahaSetFieldState(sdtInput, document.getElementById("err_exchange_sdt"), "");
            }

            if (!tinhSelect.value) {
                ahaSetFieldState(tinhSelect, document.getElementById("err_exchange_tinh"), "Chọn Tỉnh/Thành.");
                ok = false;
            } else {
                ahaSetFieldState(tinhSelect, document.getElementById("err_exchange_tinh"), "");
            }

            if (!quanSelect.value) {
                ahaSetFieldState(quanSelect, document.getElementById("err_exchange_quan"), "Chọn Quận/Huyện.");
                ok = false;
            } else {
                ahaSetFieldState(quanSelect, document.getElementById("err_exchange_quan"), "");
            }

            if (!phuongSelect.value) {
                ahaSetFieldState(phuongSelect, document.getElementById("err_exchange_phuong"), "Chọn Phường/Xã.");
                ok = false;
            } else {
                ahaSetFieldState(phuongSelect, document.getElementById("err_exchange_phuong"), "");
            }

            if (detail.length < 4) {
                ahaSetFieldState(detailInput, document.getElementById("err_exchange_chitiet"), "Vui lòng nhập địa chỉ chi tiết.");
                ok = false;
            } else {
                ahaSetFieldState(detailInput, document.getElementById("err_exchange_chitiet"), "");
            }

            return ok;
        }

        function ahaBeforeExchangeSubmit(btn) {
            if (!ahaValidateExchange()) return false;
            if (!btn) return true;
            if (btn.dataset.loading === "1") return false;

            btn.dataset.loading = "1";
            btn.classList.add("is-loading");
            btn.dataset.originalText = btn.value;
            btn.value = "Đang xử lý...";
            return true;
        }

        function ahaBindExchangeValidation() {
            if (ahaIsServiceMode()) return;

            var hotenInput = document.getElementById("<%= txt_hoten_nguoinhan.ClientID %>");
            var sdtInput = document.getElementById("<%= txt_sdt_nguoinhan.ClientID %>");
            var detailInput = document.getElementById("<%= txt_diachi_chitiet.ClientID %>");
            var tinhSelect = document.getElementById("exchange_tinh");
            var quanSelect = document.getElementById("exchange_quan");
            var phuongSelect = document.getElementById("exchange_phuong");
            if (!hotenInput || !sdtInput || !detailInput || !tinhSelect || !quanSelect || !phuongSelect) return;
            if (hotenInput.dataset.boundValidation === "1") return;

            hotenInput.dataset.boundValidation = "1";
            sdtInput.dataset.boundValidation = "1";
            detailInput.dataset.boundValidation = "1";

            hotenInput.addEventListener("input", function () {
                if (hotenInput.value.trim().length >= 2) {
                    ahaSetFieldState(hotenInput, document.getElementById("err_exchange_hoten"), "");
                }
            });
            sdtInput.addEventListener("input", function () {
                if (ahaValidatePhone(sdtInput.value.trim())) {
                    ahaSetFieldState(sdtInput, document.getElementById("err_exchange_sdt"), "");
                }
            });
            detailInput.addEventListener("input", function () {
                if (detailInput.value.trim().length >= 4) {
                    ahaSetFieldState(detailInput, document.getElementById("err_exchange_chitiet"), "");
                }
            });
            tinhSelect.addEventListener("change", function () {
                if (tinhSelect.value) {
                    ahaSetFieldState(tinhSelect, document.getElementById("err_exchange_tinh"), "");
                }
            });
            quanSelect.addEventListener("change", function () {
                if (quanSelect.value) {
                    ahaSetFieldState(quanSelect, document.getElementById("err_exchange_quan"), "");
                }
            });
            phuongSelect.addEventListener("change", function () {
                if (phuongSelect.value) {
                    ahaSetFieldState(phuongSelect, document.getElementById("err_exchange_phuong"), "");
                }
            });
        }

        function ahaQtySanitize(input) {
            if (!input) return;
            var raw = String(input.value || "").replace(/\D+/g, "");
            var qty = parseInt(raw || "1", 10);
            if (isNaN(qty) || qty < 1) qty = 1;
            if (qty > 999) qty = 999;
            input.value = qty;
            ahaExchangeRecalc();
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

        function ahaBindSavedAddressPicker() {
            if (ahaIsServiceMode()) return;

            var radios = document.querySelectorAll(".js-addr-pick-exchange");
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
                            provinceSelectId: "exchange_tinh",
                            districtSelectId: "exchange_quan",
                            wardSelectId: "exchange_phuong",
                            detailInputId: "<%= txt_diachi_chitiet.ClientID %>",
                            rawAddressId: "<%= hf_address_raw.ClientID %>"
                        }, rawAddress);
                    } else if (detailInput && rawAddress) {
                        detailInput.value = rawAddress;
                    }

                    if (typeof ahaSetFieldState === "function") {
                        if (hotenInput) ahaSetFieldState(hotenInput, document.getElementById("err_exchange_hoten"), "");
                        if (sdtInput) ahaSetFieldState(sdtInput, document.getElementById("err_exchange_sdt"), "");
                        if (detailInput) ahaSetFieldState(detailInput, document.getElementById("err_exchange_chitiet"), "");
                        var tinhSelect = document.getElementById("exchange_tinh");
                        var quanSelect = document.getElementById("exchange_quan");
                        var phuongSelect = document.getElementById("exchange_phuong");
                        if (tinhSelect) ahaSetFieldState(tinhSelect, document.getElementById("err_exchange_tinh"), "");
                        if (quanSelect) ahaSetFieldState(quanSelect, document.getElementById("err_exchange_quan"), "");
                        if (phuongSelect) ahaSetFieldState(phuongSelect, document.getElementById("err_exchange_phuong"), "");
                    }
                });
            });

            var defaultRadio = document.querySelector(".js-addr-pick-exchange[data-default='1']");
            if (defaultRadio) {
                defaultRadio.checked = true;
                defaultRadio.dispatchEvent(new Event("change", { bubbles: true }));
                return;
            }

            var checked = document.querySelector(".js-addr-pick-exchange:checked");
            if (checked) {
                var item = checked.closest(".saved-address-item");
                if (item) item.classList.add("is-selected");
            }
        }

        document.addEventListener("DOMContentLoaded", function () {
            ahaExchangeRecalc();
            ahaBindExchangeValidation();
            if (window.AhaAddressPicker) {
                window.AhaAddressPicker.init({
                    provinceSelectId: "exchange_tinh",
                    districtSelectId: "exchange_quan",
                    wardSelectId: "exchange_phuong",
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
    </script>
</asp:Content>
