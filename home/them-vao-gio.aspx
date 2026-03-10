<%@ Page Title="Thêm vào giỏ hàng" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="them-vao-gio.aspx.cs" Inherits="home_them_vao_gio" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .cart-add-page .product-media {
            width: 100%;
            max-height: 280px;
            object-fit: cover;
            border-radius: 12px;
            border: 1px solid rgba(98, 105, 118, .2);
            background: #fff;
        }
        .cart-add-page .metric {
            border: 1px solid rgba(98, 105, 118, .2);
            border-radius: 12px;
            padding: 12px;
            background: #fff;
        }
        .cart-add-page .metric .label {
            font-size: 12px;
            color: #6c7a91;
        }
        .cart-add-page .metric .value {
            font-size: 18px;
            font-weight: 600;
            margin-top: 6px;
            white-space: nowrap;
        }
        .cart-add-page .qty-stepper {
            display: inline-flex;
            align-items: center;
            gap: 7px;
            border: 1px solid rgba(98, 105, 118, 0.3);
            border-radius: 999px;
            background: #fff;
            padding: 4px 8px;
            min-width: 146px;
        }
        .cart-add-page .qty-step-btn {
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
        .cart-add-page .qty-input {
            width: 54px;
            min-width: 54px;
            border: 0 !important;
            box-shadow: none !important;
            background: transparent !important;
            color: #111827 !important;
            text-align: center;
            font-size: 24px;
            font-weight: 800;
            padding: 0 !important;
            height: 38px;
        }

        html[data-bs-theme="dark"] .cart-add-page .product-media {
            background: #0f172a;
            border-color: #223246;
        }

        html[data-bs-theme="dark"] .cart-add-page .metric {
            background: #0f172a;
            border-color: #223246;
        }

        html[data-bs-theme="dark"] .cart-add-page .metric .label {
            color: #94a3b8;
        }

        html[data-bs-theme="dark"] .cart-add-page .metric .value {
            color: #e5e7eb;
        }

        html[data-bs-theme="dark"] .cart-add-page .qty-stepper {
            background: #111d34;
            border-color: #223246;
        }

        html[data-bs-theme="dark"] .cart-add-page .qty-step-btn {
            background: #0f172a;
            border-color: #334155;
            color: #e5e7eb;
        }

        html[data-bs-theme="dark"] .cart-add-page .qty-step-btn:hover {
            background: #1e2a3d;
            border-color: #4b5563;
            color: #ffffff;
        }

        html[data-bs-theme="dark"] .cart-add-page .qty-input {
            color: #e5e7eb !important;
        }
        .cart-add-page .btn-primary.loading {
            opacity: .92;
            pointer-events: none;
            cursor: wait;
        }
        .cart-add-page .btn-primary.loading::after {
            content: "";
            width: 14px;
            height: 14px;
            border-radius: 50%;
            border: 2px solid rgba(255,255,255,.45);
            border-top-color: #fff;
            display: inline-block;
            margin-left: 8px;
            vertical-align: -2px;
            animation: addCartSpin .75s linear infinite;
        }
        @keyframes addCartSpin {
            to { transform: rotate(360deg); }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="cart-add-page">
        <div class="page-header d-print-none">
            <div class="container-xl">
                <div class="row g-2 align-items-center">
                    <div class="col">
                        <div class="page-pretitle">Giỏ hàng</div>
                        <h2 class="page-title">Thêm sản phẩm vào giỏ</h2>
                    </div>
                    <div class="col-auto ms-auto">
                        <asp:HyperLink ID="hl_back_top" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                            <i class="ti ti-arrow-left"></i>&nbsp;Quay lại
                        </asp:HyperLink>
                    </div>
                </div>
            </div>
        </div>

        <div class="page-body">
            <div class="container-xl">
                <div class="card">
                    <div class="card-body">
                        <div class="row g-4">
                            <div class="col-lg-5">
                                <img id="img_product" runat="server" class="product-media" alt="Sản phẩm" src="/uploads/images/macdinh.jpg" />
                                <div class="mt-3">
                                    <div class="text-muted small">Sản phẩm</div>
                                    <div class="h3 mb-0"><asp:Label ID="lb_ten_sp" runat="server" Text="-"></asp:Label></div>
                                </div>
                            </div>
                            <div class="col-lg-7">
                                <div class="row g-2 mb-3">
                                    <div class="col-md-6">
                                        <div class="metric">
                                            <div class="label">Giá (VNĐ)</div>
                                            <div class="value text-danger"><asp:Label ID="lb_gia_vnd" runat="server" Text="0"></asp:Label> đ</div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="metric">
                                            <div class="label">Giá (A)</div>
                                            <div class="value text-primary"><asp:Label ID="lb_gia_a" runat="server" Text="0"></asp:Label> A</div>
                                        </div>
                                    </div>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Số lượng</label>
                                    <div class="qty-stepper">
                                        <button type="button" class="qty-step-btn" onclick="ahaQtyAdjust(this,-1)" aria-label="Giảm số lượng">-</button>
                                        <asp:TextBox ID="txt_soluong" runat="server" CssClass="form-control qty-input js-cartadd-qty" MaxLength="3"
                                            onfocus="AutoSelect(this)" oninput="ahaQtySanitize(this)"></asp:TextBox>
                                        <button type="button" class="qty-step-btn" onclick="ahaQtyAdjust(this,1)" aria-label="Tăng số lượng">+</button>
                                    </div>
                                </div>

                                <div class="row g-2">
                                    <div class="col-md-6">
                                        <div class="metric">
                                            <div class="label">Tổng tiền (VNĐ)</div>
                                            <div class="value text-danger"><asp:Label ID="lb_tong_vnd" runat="server" Text="0"></asp:Label> đ</div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="metric">
                                            <div class="label">Tổng tương ứng (A)</div>
                                            <div class="value text-success"><asp:Label ID="lb_tong_a" runat="server" Text="0"></asp:Label> A</div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-footer d-flex align-items-center justify-content-between">
                        <asp:HyperLink ID="hl_back_bottom" runat="server" CssClass="btn btn-outline-secondary">Hủy</asp:HyperLink>
                        <asp:Button ID="but_xacnhan" runat="server" CssClass="btn btn-primary" Text="Thêm vào giỏ hàng" OnClick="but_xacnhan_Click" OnClientClick="return ahaBeforeCartAddSubmit(this);" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server"></asp:Content>
<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="Server">
    <script>
        function ahaParseNumber(str) {
            var cleaned = String(str || "").replace(/[^\d.-]/g, "");
            var num = parseFloat(cleaned);
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

        function ahaCartAddRecalc() {
            var qtyInput = document.getElementById("<%= txt_soluong.ClientID %>");
            var unitVndLabel = document.getElementById("<%= lb_gia_vnd.ClientID %>");
            var totalVndLabel = document.getElementById("<%= lb_tong_vnd.ClientID %>");
            var totalALabel = document.getElementById("<%= lb_tong_a.ClientID %>");
            if (!qtyInput || !unitVndLabel || !totalVndLabel || !totalALabel) return;

            var qty = parseInt(String(qtyInput.value || "").replace(/\D+/g, ""), 10);
            if (isNaN(qty) || qty < 1) qty = 1;
            if (qty > 999) qty = 999;
            qtyInput.value = qty;

            var unitVnd = ahaParseNumber(unitVndLabel.textContent);
            var totalVnd = unitVnd * qty;
            var totalA = ahaToAFromVnd(totalVnd);

            totalVndLabel.textContent = ahaFormatVnd(totalVnd);
            totalALabel.textContent = ahaFormatA(totalA);
        }

        function ahaQtySanitize(input) {
            if (!input) return;
            var raw = String(input.value || "").replace(/\D+/g, "");
            var qty = parseInt(raw || "1", 10);
            if (isNaN(qty) || qty < 1) qty = 1;
            if (qty > 999) qty = 999;
            input.value = qty;
            ahaCartAddRecalc();
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

        function ahaBeforeCartAddSubmit(btn) {
            ahaCartAddRecalc();
            if (!btn) return true;
            if (btn.dataset.loading === "1") return false;

            btn.dataset.loading = "1";
            btn.dataset.originalText = btn.value;
            btn.value = "Đang thêm...";
            btn.classList.add("loading");
            return true;
        }

        document.addEventListener("DOMContentLoaded", function () {
            ahaCartAddRecalc();
        });
    </script>
</asp:Content>
