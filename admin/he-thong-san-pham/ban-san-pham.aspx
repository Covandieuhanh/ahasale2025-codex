<%@ Page Title="Bán sản phẩm" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true"
    CodeFile="ban-san-pham.aspx.cs" Inherits="admin_he_thong_san_pham_ban_the" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <!-- ======================= POPUP BÁN SẢN PHẨM (NEW) ======================= -->
    <asp:UpdatePanel ID="up_banthe" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_banthe" runat="server" Visible="false" DefaultButton="but_tao_banthe">

                <!-- header fixed -->
                <div style="position: fixed; width: 100%; height: 52px; top: 0; left: 0; z-index: 1041!important;">
                    <div style='margin: 0 auto; max-width: 520px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' runat="server" id="close_banthe" onserverclick="but_close_form_banthe_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                BÁN SẢN PHẨM
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>

                <!-- overlay -->
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='margin: 0 auto; max-width: 526px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pr-4" style="padding-top: 52px">

                            <div class="row">
                                <div class="cell-lg-12">

                                    <!-- chọn tài khoản Đồng hành hệ sinh thái -->
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Bán cho tài khoản nào?</label>
                                        <div>
                                            <asp:DropDownList ID="ddl_taikhoan_nhadautu" runat="server" data-role="select"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <!-- chọn sản phẩm -->
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Chọn sản phẩm</label>
                                        <div>
                                            <asp:DropDownList ID="ddl_sanpham" runat="server"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddl_sanpham_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <!-- số lượng -->
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Số lượng</label>
                                        <div>
                                            <asp:TextBox ID="txt_soluong"
                                                runat="server"
                                                data-role="input"
                                                MaxLength="10"
                                                placeholder="Nhập số lượng"
                                                AutoPostBack="true"
                                                OnTextChanged="txt_soluong_TextChanged"
                                                oninput="onlyNumber1to100(this)">
                                            </asp:TextBox>
                                        </div>
                                    </div>

                                    <!-- % cho sàn -->
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">% chiết khấu cho sàn</label>
                                        <div class="p-2 border bd-gray rounded bg-light">
                                            <asp:Label ID="lb_phantram_san" runat="server" CssClass="fw-600"></asp:Label>
                                        </div>
                                    </div>

                                    <!-- VNĐ -->
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Giá bán (VNĐ)</label>
                                        <div class="p-2 border bd-gray rounded bg-light">
                                            <asp:Label ID="lb_giatri" runat="server" CssClass="fw-600"></asp:Label>
                                        </div>
                                    </div>

                                    <!-- Quyền tiêu dùng -->
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Quyền tiêu dùng</label>
                                        <div class="p-2 border bd-gray rounded bg-light">
                                            <asp:Label ID="lb_dongA" runat="server" CssClass="fw-600"></asp:Label>
                                        </div>
                                        <small class="fg-gray">1A = 1.000 VNĐ</small>
                                    </div>

                                </div>
                            </div>

                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_tao_banthe" runat="server" Text="BÁN SẢN PHẨM"
                                    CssClass="button success" OnClick="but_tao_banthe_Click" OnClientClick="this.disabled=true; this.value='Đang xử lý...'; __doPostBack(this.name,''); return false;"
 />
                            </div>

                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_banthe">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true">
                        <span class="electron"></span><span class="electron"></span><span class="electron"></span>
                    </div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <!-- ======================= POPUP CHI TIẾT GIAO DỊCH ======================= -->
    <asp:UpdatePanel ID="up_chitiet" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_chitiet" runat="server" Visible="false">

                <!-- header fixed -->
                <div style="position: fixed; width: 100%; height: 52px; top: 0; left: 0; z-index: 1041!important;">
                    <div style='margin: 0 auto; max-width: 860px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' runat="server" id="close_chitiet" onserverclick="but_close_chitiet_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
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
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='margin: 0 auto; max-width: 866px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pr-4" style="padding-top: 52px; padding-bottom: 30px">

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
                <div id="menutop-tool-bc" style="position: fixed; top: 52px; width: 100%; z-index: 4">
                    <ul class="h-menu bg-white">

                        <li data-role="hint" data-hint-position="top" data-hint-text="Bán sản phẩm">
                            <asp:HyperLink ID="but_show_form_banthe" runat="server">
                                <span class="mif-plus"></span>
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

            <div class="p-3">
                <div class="row">
                    <div class="cell-lg-12">
                     <div class="bg-white p-4 shadow-2 rounded">

    <div class="d-flex flex-align-center mb-3">
        <h5 class="m-0 text-bold"> LỊCH SỬ BÁN SẢN PHẨM</h5>
        <span class="ml-auto fg-gray">
            <asp:Label ID="Label1" runat="server" />
        </span>
    </div>

    <div class="table-responsive">
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
    <script>
        function onlyNumber1to100(input) {
            input.value = input.value.replace(/[^0-9]/g, '');
            if (input.value === '') return;
            var num = parseInt(input.value);
            if (num < 1) input.value = 1;
            if (num > 100) input.value = 100;
        }
    </script>
</asp:Content>
