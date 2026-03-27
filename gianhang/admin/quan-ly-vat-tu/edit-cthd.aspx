<%@ Page Title="Chỉnh sửa đơn nhập vật tư" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="edit-cthd.aspx.cs" Inherits="badmin_Default" %><asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <div id="main-content" class="mb-10">
        <div class="row border-bottom bd-light">
            <div class="cell-6">
                <ul class="h-menu">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Quay lại">
                        <a class="button" href="<%=url_back %>"><span class="mif mif-arrow-left"></span></a></li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                </ul>
            </div>
            <div class="cell-6">
            </div>
        </div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>
            </Triggers>
            <ContentTemplate>
                <div>
                    <ul data-role="tabs" data-expand="true">
                        <li><a href="#themsanpham">Vật tư</a></li>
                    </ul>

                    <div class="border bd-default no-border-top p-2 pl-4 pr-4">

                        <div id="themsanpham">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="Panel2" runat="server" DefaultButton="but_form_themsanpham">
                                        <div class="row ">
                                            <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                                <div class="">
                                                    <label class="fw-600">Ngày nhập</label>

                                                    <asp:TextBox ID="txt_ngayban_sanpham" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Vật tư</label>
                                                    <%--<asp:DropDownList ID="ddl_sanpham" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_sanpham_SelectedIndexChanged"></asp:DropDownList>--%>
                                                    <asp:TextBox ID="txt_tensanpham" runat="server" data-role="input" placeholder="Nhập và chọn tên sản phẩm"></asp:TextBox></div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Đơn giá</label>
                                                    <asp:TextBox ID="txt_gia_sanpham" MaxLength="13" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Số lượng</label>
                                                    <asp:TextBox ID="txt_soluong_sanpham" data-role="input" runat="server" MaxLength="5" Text="1"></asp:TextBox><%--autocomplete="off" --%>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Chiết khấu sản phẩm</label>
                                                    <span class="place-right">
                                                        <asp:RadioButton ID="ck_sp_phantram" runat="server" Text="%" GroupName="ck_sp" Checked="true" />
                                                        <asp:RadioButton ID="ck_sp_tienmat" runat="server" Text="Tiền" GroupName="ck_sp" />
                                                    </span>
                                                    <asp:TextBox ID="txt_chietkhau_sanpham" data-role="input" runat="server" MaxLength="10" Text="0" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                                </div>
                                            </div>
                                            <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                                <div>
                                                    <label class="fw-600">Ngày nhập</label>
                                                    <asp:TextBox ID="txt_nsx" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Hạn bảo hành</label>
                                                    <asp:TextBox ID="txt_hsd" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Số lô</label>
                                                    <asp:TextBox ID="txt_solo" data-role="input" runat="server" Text=""></asp:TextBox>
                                                </div>
                                                <div class="mt-3">
                                                    <label class="fw-600">Đơn vị tính</label>
                                                    <asp:TextBox ID="txt_dvt" data-role="input" runat="server" Text=""></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="mt-6 mb-6 text-right">
                                            <div style="float: left">
                                                <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                            </div>
                                            <div style="float: right">
                                                <asp:Button ID="but_form_themsanpham" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="but_form_themsanpham_Click" />
                                            </div>
                                            <div style="clear: both"></div>
                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="UpdatePanel3">
                                <ProgressTemplate>
                                    <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                                        <div style="padding-top: 50vh;">
                                            <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                                        </div>
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
            <ProgressTemplate>
                <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                    <div style="padding-top: 50vh;">
                        <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <script>
        function show_saochep() {
            Metro.notify.create("Sao chép link hóa đơn thành công.", "Thông báo", {});
        }
    </script>
    <%--<%=notifi %>--%>

    <script src="/js/gianhang-invoice-fast.js?v=20260326a"></script>
    <script>
        (function () {
            function bindFastUi() {
                if (!window.ahaInvoiceFast) return;
                window.ahaInvoiceFast.initItemLookup({
                    endpoint: "/gianhang/admin/quan-ly-vat-tu/lookup-data.ashx",
                    mode: "supply-item",
                    inputId: "<%=txt_tensanpham.ClientID %>",
                    priceId: "<%=txt_gia_sanpham.ClientID %>",
                    unitId: "<%=txt_dvt.ClientID %>"
                });
            }
            bindFastUi();
            if (window.Sys && Sys.Application) {
                Sys.Application.add_load(bindFastUi);
            }
        })();
    </script>
</asp:Content>

