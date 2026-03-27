<%@ Page Title="Bán sản phẩm" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true"
    CodeFile="ban-san-pham.aspx.cs" Inherits="admin_he_thong_san_pham_ban_the" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/choices.js/public/assets/styles/choices.min.css" />
    <style>
        .sell-page {
            max-width: 920px;
            margin: 0 auto;
        }

        .sell-head {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
            margin-bottom: 16px;
        }

        .sell-head h2 {
            margin: 0;
            font-size: 26px;
            font-weight: 800;
        }

        .sell-head p {
            margin: 6px 0 0;
            color: #6b7280;
        }

        .sell-card {
            background: #fff;
            border-radius: 18px;
            padding: 18px;
            box-shadow: 0 10px 24px rgba(15, 23, 42, 0.08);
            border: 1px solid #eef2f7;
        }

        .sell-field {
            margin-bottom: 16px;
        }

        .sell-label {
            font-weight: 700;
            margin-bottom: 8px;
            display: block;
        }

        .sell-search {
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 10px 14px;
            border-radius: 14px;
            background: #f3f6f9;
            border: 1px solid #e5ebf0;
        }

        .sell-search .g-icon {
            color: #94a3b8;
            font-size: 18px;
            width: 20px;
            text-align: center;
            flex: 0 0 20px;
        }

        .sell-search .g-select {
            width: 100%;
            border: 0;
            background: transparent;
            font-size: 16px;
        }

        .sell-search .choices {
            width: 100%;
        }

        .sell-search .choices__inner {
            min-height: 40px;
            padding: 0;
            border: 0;
            background: transparent;
            box-shadow: none;
            display: flex;
            align-items: center;
        }

        .sell-search .choices__list--single {
            padding: 0;
        }

        .sell-search .choices[data-type*="select-one"]::after {
            right: 0;
            border-color: #9aa0a6 transparent transparent;
        }

        .sell-input {
            border-radius: 14px;
            padding: 10px 14px;
            font-size: 16px;
        }

        .sell-summary {
            background: #f8fafc;
            border-radius: 14px;
            padding: 12px 14px;
            border: 1px solid #e5ebf0;
            margin-bottom: 16px;
        }

        .sell-summary-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 8px 0;
            border-bottom: 1px dashed #e5ebf0;
            font-weight: 600;
        }

        .sell-summary-row:last-child {
            border-bottom: 0;
        }

        .sell-summary-row .value {
            font-weight: 800;
        }

        .sell-summary-note {
            font-size: 12px;
            color: #6b7280;
            margin-top: 4px;
        }

        .sell-actions {
            display: flex;
            justify-content: flex-end;
        }

        .sell-actions .button.success {
            border-radius: 14px;
            font-weight: 700;
            min-width: 180px;
        }

        .sell-alert {
            background: #fef2f2;
            color: #b91c1c;
            border: 1px solid #fecaca;
            border-radius: 14px;
            padding: 12px 14px;
            font-weight: 600;
            margin-bottom: 16px;
        }

        .sell-special-trace {
            background: #f8fafc;
            border: 1px solid #e5ebf0;
            border-radius: 14px;
            padding: 14px;
        }

        .sell-special-badge {
            display: inline-flex;
            align-items: center;
            padding: 4px 10px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 700;
            background: #dcfce7;
            color: #166534;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <!-- ======================= FULL-PAGE BÁN SẢN PHẨM (NEW) ======================= -->
    <asp:UpdatePanel ID="up_banthe" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:PostBackTrigger ControlID="but_tao_banthe" />
        </Triggers>
        <ContentTemplate>
            <asp:Panel ID="pn_banthe" runat="server" Visible="false" DefaultButton="but_tao_banthe">
                <div class="sell-page">
                    <div class="sell-head">
                        <div>
                            <h2>Bán thẻ</h2>
                            <p>Chọn tài khoản nhận và sản phẩm để bán nhanh.</p>
                        </div>
                        <asp:HyperLink ID="hl_back_sell" runat="server" CssClass="button mini">
                            <span class="mif-arrow-left"></span> Quay lại
                        </asp:HyperLink>
                    </div>

                    <div class="sell-card">
                        <asp:Panel ID="pn_sell_error" runat="server" CssClass="sell-alert" Visible="false">
                            <asp:Literal ID="lt_sell_error" runat="server"></asp:Literal>
                        </asp:Panel>
                        <asp:HiddenField ID="hf_sell_token" runat="server" />

                        <div class="sell-field">
                            <label class="sell-label">Chọn tài khoản mua</label>
                            <div class="sell-search">
                                <span class="g-icon"><i class="ti ti-user"></i></span>
                                <asp:DropDownList ID="ddl_taikhoan_nhadautu" runat="server"
                                    CssClass="g-select js-choices"
                                    data-choices-search="1"
                                    data-choices-placeholder="Tìm tài khoản">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="sell-field">
                            <label class="sell-label">Chọn sản phẩm</label>
                            <div class="sell-search">
                                <span class="g-icon"><i class="ti ti-box"></i></span>
                                <asp:DropDownList ID="ddl_sanpham" runat="server"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="ddl_sanpham_SelectedIndexChanged"
                                    CssClass="g-select js-choices"
                                    data-choices-search="1"
                                    data-choices-placeholder="Tìm sản phẩm">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="sell-field">
                            <label class="sell-label">Số lượng</label>
                            <asp:TextBox ID="txt_soluong"
                                runat="server"
                                CssClass="form-control sell-input"
                                MaxLength="10"
                                placeholder="Nhập số lượng"
                                AutoPostBack="true"
                                OnTextChanged="txt_soluong_TextChanged"
                                oninput="onlyNumber1to100(this)">
                            </asp:TextBox>
                        </div>

                        <div class="sell-summary">
                            <div class="sell-summary-row">
                                <span>% chiết khấu cho sàn</span>
                                <span class="value"><asp:Label ID="lb_phantram_san" runat="server" Text="0"></asp:Label>%</span>
                            </div>
                            <div class="sell-summary-row">
                                <span>Giá bán (VNĐ)</span>
                                <span class="value"><asp:Label ID="lb_giatri" runat="server" Text="0"></asp:Label></span>
                            </div>
                            <div class="sell-summary-row">
                                <span>Quyền tiêu dùng</span>
                                <span class="value"><asp:Label ID="lb_dongA" runat="server" Text="0.00"></asp:Label></span>
                            </div>
                            <div class="sell-summary-note">1A = 1.000 VNĐ</div>
                        </div>

                        <div class="sell-actions">
                            <asp:Button ID="but_tao_banthe" runat="server" Text="BÁN SẢN PHẨM"
                                CssClass="button success"
                                UseSubmitBehavior="true"
                                OnClientClick="return AhaSellSubmit(this);"
                                OnClick="but_tao_banthe_Click" />
                        </div>
                    </div>
                </div>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_banthe">
        <ProgressTemplate>
            <div class="admin-inline-progress" role="status" aria-live="polite"><span class="admin-inline-progress-spinner"></span><span class="admin-inline-progress-text">Đang xử lý...</span></div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <!-- ======================= FULL-PAGE CHI TIẾT GIAO DỊCH ======================= -->
    <asp:UpdatePanel ID="up_chitiet" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_chitiet" runat="server" Visible="false">

                <!-- header fixed -->
                <div class="admin-fullpage-head admin-route-panel-head">
                    <div class='admin-fullpage-shell admin-fullpage-head-shell admin-route-panel-shell admin-route-panel-head-shell'>
                        <div class="admin-route-panel-actions">
                            <asp:HyperLink ID="close_chitiet" runat="server" CssClass="admin-route-back-link">Quay lại danh sách</asp:HyperLink>
                        </div>
                        <div class="bg-white pl-4 pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                CHI TIẾT GIAO DỊCH BÁN SẢN PHẨM
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>

                <!-- overlay -->
                <div class="admin-fullpage-body-wrap admin-route-panel-body-wrap">
                    <div class='admin-fullpage-shell admin-fullpage-dialog admin-route-panel-shell admin-route-panel-dialog'>
                        <div class="bg-white border bd-transparent admin-fullpage-body admin-route-panel-body pl-4 pr-4" style="padding-bottom: 30px">

                            <!-- Header info -->
                            <div class="mt-3">
                                <div class="text-bold">Thông tin giao dịch</div>
                                <hr />

                                <div class="row">
                                    <div class="cell-lg-6">
                                        <div class="p-2 border bd-gray rounded bg-light mb-2">
                                            <div class="fg-gray">Mã giao dịch</div>
                                            <div class="text-bold"><asp:Label ID="lb_ct_id" runat="server" /></div>
                                        </div>

                                        <div class="p-2 border bd-gray rounded bg-light mb-2">
                                            <div class="fg-gray">Thời gian</div>
                                            <div class="text-bold"><asp:Label ID="lb_ct_thoigian" runat="server" /></div>
                                        </div>

                                        <div class="p-2 border bd-gray rounded bg-light mb-2">
                                            <div class="fg-gray">Tài khoản mua</div>
                                            <div class="text-bold"><asp:Label ID="lb_ct_tk_mua" runat="server" /></div>
                                        </div>

                                        <div class="p-2 border bd-gray rounded bg-light mb-2">
                                            <div class="fg-gray">Tài khoản bán</div>
                                            <div class="text-bold"><asp:Label ID="lb_ct_tk_ban" runat="server" /></div>
                                        </div>
                                    </div>

                                    <div class="cell-lg-6">
                                        <div class="p-2 border bd-gray rounded bg-light mb-2">
                                            <div class="fg-gray">Sản phẩm</div>
                                            <div class="text-bold"><asp:Label ID="lb_ct_sanpham" runat="server" /></div>
                                        </div>

                                        <div class="p-2 border bd-gray rounded bg-light mb-2">
                                            <div class="fg-gray">Số lượng</div>
                                            <div class="text-bold"><asp:Label ID="lb_ct_soluong" runat="server" /></div>
                                        </div>

                                        <div class="p-2 border bd-gray rounded bg-light mb-2">
                                            <div class="fg-gray">Tổng tiền (VNĐ)</div>
                                            <div class="text-bold"><asp:Label ID="lb_ct_tongvnd" runat="server" /></div>
                                        </div>

                                        <div class="p-2 border bd-gray rounded bg-light mb-2">
                                            <div class="fg-gray">Tổng Quyền tiêu dùng</div>
                                            <div class="text-bold"><asp:Label ID="lb_ct_tongdonga" runat="server" /></div>
                                        </div>

                                        <div class="p-2 border bd-gray rounded bg-light mb-2">
                                            <div class="fg-gray">% chi trả cho sàn</div>
                                            <div class="text-bold"><asp:Label ID="lb_ct_pt_san" runat="server" /></div>
                                        </div>
                                    </div>
                                </div>

                                <!-- Ví sàn -->
                                <div class="row mt-2">
                                    <div class="cell-lg-12">
                                        <div class="p-2 border bd-gray rounded bg-light">
                                            <div class="text-bold">Lợi nhuận của sàn chia 20/30/50 như sau</div>
                                            <div class="row mt-2">
                                                <div class="cell-md-3">
                                                    <div class="fg-gray">Tổng</div>
                                                    <div class="text-bold"><asp:Label ID="lb_ct_vitong" runat="server" /></div>
                                                </div>
                                                <div class="cell-md-3">
                                                    <div class="fg-gray">Hồ sơ quyền ưu đãi (30%)</div>
                                                    <div class="text-bold"><asp:Label ID="lb_ct_vi1" runat="server" /></div>
                                                </div>
                                                <div class="cell-md-3">
                                                    <div class="fg-gray">Hồ sơ hành vi lao động (50%)</div>
                                                    <div class="text-bold"><asp:Label ID="lb_ct_vi2" runat="server" /></div>
                                                </div>
                                                <div class="cell-md-3">
                                                    <div class="fg-gray">Hồ sơ chỉ số gắn kết (20%)</div>
                                                    <div class="text-bold"><asp:Label ID="lb_ct_vi3" runat="server" /></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>

                            <!-- Detail list -->
                            <div class="mt-6">
                                <div class="text-bold">Danh sách tài khoản nhận (Chi tiết chia)</div>
                                <hr />

                                <div class="table-responsive">
                                    <table class="table striped table-border cell-border">
                                        <thead>
                                            <tr>
                                                <th style="min-width:60px" class="text-center">#</th>
                                                <th style="min-width:160px">Tài khoản nhận</th>
                                                <th style="min-width:120px" class="text-center">Hành vi</th>
                                                <th style="min-width:90px" class="text-center">% nhận</th>
                                                <th style="min-width:140px" class="text-right">Quyền tiêu dùng nhận</th>
                                                <th style="min-width:160px" class="text-center">Thời gian</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Repeater ID="RepeaterChiTiet" runat="server">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="text-center"><%# Container.ItemIndex + 1 %></td>
                                                        <td class="text-bold"><%# Eval("TaiKhoan_Nhan") %></td>
                                                        <td class="text-center"><%# Eval("HanhVi_Text") %></td>
                                                        <td class="text-center">
    <%# Eval("PhanTramNhanDuoc_Text") %>
</td>

                                                        <td class="text-right text-bold"><%# Eval("DongANhanDuoc", "{0:#,##0.##}") %></td>
                                                        <td class="text-center"><%# Eval("ThoiGian_Text") %></td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </div>

                                <div class="mt-2 fg-gray">
                                    <small><asp:Label ID="lb_ct_note" runat="server"></asp:Label></small>
                                </div>
                            </div>

                            <asp:Panel ID="pn_ct_special" runat="server" Visible="false" CssClass="mt-6">
                                <div class="text-bold">Nghiệp vụ đặc biệt đã chạy</div>
                                <hr />

                                <div class="sell-special-trace">
                                    <div class="table-responsive">
                                        <table class="table striped table-border cell-border">
                                            <thead>
                                                <tr>
                                                    <th style="min-width:60px" class="text-center">#</th>
                                                    <th style="min-width:160px">Handler</th>
                                                    <th style="min-width:120px" class="text-center">Trạng thái</th>
                                                    <th>Mô tả</th>
                                                    <th style="min-width:170px">Dữ liệu trace</th>
                                                    <th style="min-width:160px" class="text-center">Thời gian chạy</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:Repeater ID="rpt_ct_special" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td class="text-center"><%# Container.ItemIndex + 1 %></td>
                                                            <td class="text-bold"><%# Eval("HandlerLabel") %></td>
                                                            <td class="text-center"><%# Eval("ExecutionStatus") %></td>
                                                            <td><%# Eval("ExecutionSummary") %></td>
                                                            <td class="fg-gray"><%# Eval("ExecutionData") %></td>
                                                            <td class="text-center"><%# Eval("ExecutedAtText") %></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </asp:Panel>

                        </div>
                    </div>
                </div>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- ======================= MAIN LIST ======================= -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" class="admin-fullpage-toolbar admin-route-toolbar">
                    <ul class="h-menu bg-white">

                        <li data-role="hint" data-hint-position="top" data-hint-text="Bán thẻ">
                            <asp:HyperLink ID="but_show_form_banthe" runat="server">
                                <span class="mif mif-credit-card"></span>
                                <span class="ml-1">Bán thẻ</span>
                            </asp:HyperLink>
                        </li>

                        <li class="bd-gray border bd-default mt-2 d-block-lg d-none" style="height: 24px"></li>

                        <li class="d-block-lg d-none">
                            <a data-role="hint" data-hint-position="top" data-hint-text="Hiển thị">
                                <small><asp:Label ID="lb_show" runat="server" Text=""></asp:Label></small>
                            </a>
                        </li>

                        <li class="ml-auto d-block-lg d-none">
                            <asp:LinkButton ID="but_quaylai" runat="server" CssClass="button mini" OnClick="but_quaylai_Click">
                                <span class="mif-arrow-left"></span> Quay lại
                            </asp:LinkButton>
                            <asp:LinkButton ID="but_xemtiep" runat="server" CssClass="button mini" OnClick="but_xemtiep_Click">
                                Xem tiếp <span class="mif-arrow-right"></span>
                            </asp:LinkButton>
                        </li>

                    </ul>
                </div>
            </div>

            <div class="p-3 aha-admin-section">
                <div class="row">
                    <div class="cell-lg-12">
                     <div class="aha-admin-card p-4">

    <div class="d-flex flex-align-center mb-3">
        <h5 class="m-0 text-bold"> LỊCH SỬ BÁN SẢN PHẨM</h5>
        <span class="ml-auto fg-gray">
            <asp:Label ID="Label1" runat="server" />
        </span>
    </div>

    <div class="table-responsive aha-admin-grid">
        <table class="table table-border striped cell-border compact">
            <thead class="bg-light">
                <tr>
                    <th class="text-center">ID</th>
                    <th>Thời gian</th>
                    <th>Tài khoản mua</th>
                    <th>Sản phẩm</th>
                    <th class="text-center">SL</th>

                    <th class="text-right fg-darkRed">Tổng VNĐ</th>
                    <th class="text-right fg-darkBlue">Tổng A</th>

                    <th class="text-center">% Sàn</th>

                    <th class="text-right fg-emerald">Lợi nhuận sàn</th>
            <%--        <th class="text-right">Ví1</th>
                    <th class="text-right">Ví2</th>
                    <th class="text-right">Ví3</th>--%>
                    <th class="text-center">Nghiệp vụ đặc biệt</th>

                    <th class="text-center">Thao tác</th>
                </tr>
            </thead>

            <tbody>
                <asp:Repeater ID="Repeater1" runat="server" OnItemCommand="Repeater1_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <td class="text-center text-bold"><%# Eval("id") %></td>
                            <td><%# Eval("ThoiGian_Text") %></td>
                            <td class="text-bold fg-darkBlue"><%# Eval("TaiKhoan_Mua") %></td>
                            <td><%# Eval("TenSanPham") %></td>
                            <td class="text-center"><%# Eval("SoLuong") %></td>

                            <td class="text-right text-bold fg-darkRed">
                                <%# Eval("TongVND", "{0:#,##0}") %>
                            </td>

                            <td class="text-right text-bold fg-darkBlue">
                                <%# Eval("TongDongA", "{0:#,##0.##}") %>
                            </td>

                            <td class="text-center">
                                <span class=" bg-cyan fg-white">
                                    <%# Eval("PhanTram_ChiTra_ChoSan") %>%
                                </span>
                            </td>

                            <td class="text-right text-bold fg-emerald">
                                <%# Eval("ViLoiNhuan_DongA_CuaSan_NhanDuoc_ViTong", "{0:#,##0.##}") %>
                            </td>
                            <%-- <td class="text-right"><%# Eval("Vi1_30PhanTram_NhanDuoc_ViEVoucher", "{0:#,##0.##}") %></td>
                                   <td class="text-right"><%# Eval("Vi2_50PhanTram_NhanDuoc_ViLaoDong", "{0:#,##0.##}") %></td>
                            <td class="text-right"><%# Eval("Vi3_20PhanTram_NhanDuoc_ViGanKet", "{0:#,##0.##}") %></td>--%>
                            <td class="text-center">
                                <asp:Literal ID="lt_special_badge" runat="server" Text='<%# BuildSpecialExecutionBadge(Eval("SpecialExecutionLabel")) %>'></asp:Literal>
                            </td>

                            <td class="text-center">
                                <asp:HyperLink runat="server"
                                    CssClass="button mini info"
                                    NavigateUrl='<%# BuildChiTietUrl(Eval("id")) %>'>
                                    🔍 Chi tiết
                                </asp:HyperLink>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>
</div>

                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <script src="https://cdn.jsdelivr.net/npm/choices.js/public/assets/scripts/choices.min.js"></script>
    <script>
        function onlyNumber1to100(input) {
            input.value = input.value.replace(/[^0-9]/g, '');
            if (input.value === '') return;
            var num = parseInt(input.value);
            if (num < 1) input.value = 1;
            if (num > 100) input.value = 100;
        }

        function initSellChoices() {
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

        document.addEventListener('DOMContentLoaded', initSellChoices);
        if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                initSellChoices();
            });
        }

        function AhaSellSubmit(btn) {
            if (!btn) return true;
            if (btn.dataset.busy === "1") return false;
            btn.dataset.busy = "1";
            setTimeout(function () {
                btn.disabled = true;
                btn.value = "Đang xử lý...";
            }, 50);
            return true;
        }
    </script>
</asp:Content>
