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
                        <div class="exchange-step">Bước cuối: Xác nhận trao đổi</div>
                        <h2 class="exchange-title">Trao đổi đơn mua</h2>
                        <p class="exchange-sub">Trình bày ưu tiên rõ thông tin như Shopee, hành động xác nhận nổi bật như TikTok.</p>
                    </div>
                    <asp:HyperLink ID="hl_back_top" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                        <i class="ti ti-arrow-left"></i>&nbsp;Quay lại
                    </asp:HyperLink>
                </div>

                <div class="exchange-layout">
                    <div class="exchange-card">
                        <div class="card-header fw-bold">Sản phẩm</div>
                        <div class="card-body">
                            <img id="img_product" runat="server" class="product-media" alt="Sản phẩm" src="/uploads/images/macdinh.jpg" />
                            <div class="product-tag">Đang mua ngay</div>
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

                                <div class="summary-box mb-0">
                                    <div class="summary-row">
                                        <span>Tổng tiền (VNĐ)</span>
                                        <span class="value vnd"><asp:Label ID="lb_tong_vnd" runat="server" Text="0"></asp:Label> đ</span>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="exchange-card">
                            <div class="card-header fw-bold">Thông tin nhận hàng</div>
                            <div class="card-body">
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

                                <div class="mb-0">
                                    <label class="form-label receiver-label">Địa chỉ</label>
                                    <asp:TextBox ID="txt_diachi_nguoinhan" runat="server" TextMode="MultiLine" Rows="4" CssClass="form-control receiver-input"></asp:TextBox>
                                    <div class="field-error" id="err_exchange_diachi"></div>
                                </div>
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
</asp:Content>

<asp:Content ID="ContentFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server"></asp:Content>
<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="Server">
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

        function ahaValidateExchange() {
            var hotenInput = document.getElementById("<%= txt_hoten_nguoinhan.ClientID %>");
            var sdtInput = document.getElementById("<%= txt_sdt_nguoinhan.ClientID %>");
            var diachiInput = document.getElementById("<%= txt_diachi_nguoinhan.ClientID %>");
            if (!hotenInput || !sdtInput || !diachiInput) return true;

            var hoten = hotenInput.value.trim();
            var sdt = sdtInput.value.trim();
            var diachi = diachiInput.value.trim();

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

            if (diachi.length < 6) {
                ahaSetFieldState(diachiInput, document.getElementById("err_exchange_diachi"), "Vui lòng nhập địa chỉ nhận hàng chi tiết.");
                ok = false;
            } else {
                ahaSetFieldState(diachiInput, document.getElementById("err_exchange_diachi"), "");
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
            var hotenInput = document.getElementById("<%= txt_hoten_nguoinhan.ClientID %>");
            var sdtInput = document.getElementById("<%= txt_sdt_nguoinhan.ClientID %>");
            var diachiInput = document.getElementById("<%= txt_diachi_nguoinhan.ClientID %>");
            if (!hotenInput || !sdtInput || !diachiInput) return;
            if (hotenInput.dataset.boundValidation === "1") return;

            hotenInput.dataset.boundValidation = "1";
            sdtInput.dataset.boundValidation = "1";
            diachiInput.dataset.boundValidation = "1";

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
            diachiInput.addEventListener("input", function () {
                if (diachiInput.value.trim().length >= 6) {
                    ahaSetFieldState(diachiInput, document.getElementById("err_exchange_diachi"), "");
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

        document.addEventListener("DOMContentLoaded", function () {
            ahaExchangeRecalc();
            ahaBindExchangeValidation();
        });
    </script>
</asp:Content>
