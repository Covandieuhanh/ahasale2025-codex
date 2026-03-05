<%@ page title="Giỏ hàng của bạn" language="C#" masterpagefile="~/home/MasterPageHome.master" autoeventwireup="true" inherits="home_gio_hang, App_Web_md1cs0my" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:UpdatePanel ID="up_dathang" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_dathang" runat="server" Visible="false" DefaultButton="but_dathang">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 700px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' id="A1" runat="server" onserverclick="but_close_form_dathang_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                Xác nhận trao đổi
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 706px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <%--pl-4 pl-8-md pr-8-md pr-4--%>
                            <div class="row">
                                <div class="cell-lg-12">

                                    <div class="bcorn-fix-title-table-container">
                                        <table class="bcorn-fix-title-table">
                                            <thead>
                                                <tr class="">
                                                    <th style="width: 1px;">ID</th>
                                                    <th style="width: 1px;">Ảnh</th>
                                                    <th class="text-left" style="min-width: 130px;">Tên sản phẩm</th>
                                                    <th style="min-width: 90px;">Giá</th>
                                                    <th style="width: 1px;">Số lượng</th>
                                                    <th style="min-width: 90px;">Trao đổi</th>
                                                    <th style="min-width: 130px;">Shop</th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                                <asp:Repeater ID="Repeater2" runat="server">
                                                    <ItemTemplate>
                                                        <span style="display: none">
                                                            <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                                        </span>
                                                        <tr>
                                                            <td class="text-center">
                                                                <%# Eval("id") %>
                                                            </td>
                                                            <td class="text-center">
                                                                <div data-role="lightbox" class="c-pointer">
                                                                    <img src="<%# Eval("image") %>" class="img-cover-vuong" width="60" height="60" />
                                                                </div>
                                                            </td>
                                                            <td class="text-left">
                                                                <%#Eval("name") %>
                                                            </td>
                                                            <td class="text-right"><%#Eval("giaban","{0:#,##0}") %>
                                                                <img src="/uploads/images/dong-a.png" style="width: 20px!important" class="pl-1">
                                                            </td>
                                                            <td>
                                                               <%#Eval("soluong") %>
                                                            </td>
                                                            <td class="text-right"><%#Eval("ThanhTien","{0:#,##0}") %>
                                                                <img src="/uploads/images/dong-a.png" style="width: 20px!important" class="pl-1">
                                                            </td>
                                                            <td class="text-left">
                                                                <%#Eval("TenShop") %>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>

                                    <div class="mt-1">
                                        <label class="fw-600 fg-red"><small>Người nhận</small></label>
                                        <div>
                                            <asp:TextBox ID="txt_hoten_nguoinhan" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-1">
                                        <label class="fw-600 fg-red"><small>Điện thoại</small></label>
                                        <div>
                                            <asp:TextBox ID="txt_sdt_nguoinhan" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-1">
                                        <label class="fw-600 fg-red"><small>Địa chỉ</small></label>
                                        <div>
                                            <asp:TextBox ID="txt_diachi_nguoinhan" runat="server" data-role="textarea" TextMode="MultiLine"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_dathang" OnClick="but_dathang_Click" runat="server" Text="Xác nhận trao đổi" CssClass="button dark" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_dathang">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>


    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="container">
                <div class="fg-ahasale mb-2"><b>Giỏ hàng của bạn</b></div>
                <div>
                    <asp:Button ID="Button1" runat="server" Text="Xóa" CssClass="mini  alert" OnClick="Button1_Click" />
                    <asp:Button ID="Button2" runat="server" Text="Trao đổi ngay" CssClass="mini yellow" OnClick="Button2_Click" />
                    <asp:Button ID="Button3" runat="server" Text="Lưu chỉnh sửa" CssClass="mini info" OnClick="Button3_Click" />
                </div>
                <div class="row mt-2">
                    <div class="cell-lg-12">
                        <div class="bcorn-fix-title-table-container">
                            <table class="bcorn-fix-title-table">
                                <thead>
                                    <tr class="">
                                        <th style="width: 1px;">ID</th>
                                        <th style="width: 1px;">
                                            <%--data-role="checkbox" data-style="2"--%>
                                            <input data-role="hint" data-hint-position="top" data-hint-text="Chọn/Bỏ chọn" type="checkbox" onkeypress="if (event.keyCode==13) return false;" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </th>
                                        <th style="width: 1px;">Ảnh</th>
                                        <th class="text-left" style="min-width: 130px;">Tên sản phẩm</th>
                                        <th style="min-width: 90px;">Giá</th>
                                        <th style="width: 1px;">Số lượng</th>
                                        <th style="min-width: 90px;">Trao đổi</th>
                                        <th style="min-width: 130px;">Shop</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <span style="display: none">
                                                <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                            </span>
                                            <tr>
                                                <td class="text-center">
                                                    <%# Eval("id") %>
                                                </td>
                                                <td class="checkbox-table">
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>
                                                <td class="text-center">
                                                    <div data-role="lightbox" class="c-pointer">
                                                        <img src="<%# Eval("image") %>" class="img-cover-vuong" width="60" height="60" />
                                                    </div>
                                                </td>
                                                <td class="text-left">
                                                    <%#Eval("name") %>
                                                </td>
                                                <td class="text-right"><%#Eval("giaban","{0:#,##0}") %>
                                                    <img src="/uploads/images/dong-a.png" style="width: 20px!important" class="pl-1">
                                                </td>
                                                <td>
                                                    <asp:TextBox onfocus="AutoSelect(this)" data-role="input" CssClass="input-small" data-clear-button="false" oninput="format_sotien_new(this)" ID="txt_sl_1" Width="50" MaxLength="3" runat="server" Text='<%#Eval("soluong") %>' onkeypress="if (event.keyCode==13) return false;"></asp:TextBox>
                                                </td>
                                                <td class="text-right"><%#Eval("ThanhTien","{0:#,##0}") %>
                                                    <img src="/uploads/images/dong-a.png" style="width: 20px!important" class="pl-1">
                                                </td>
                                                <td class="text-left">
                                                    <%#Eval("TenShop") %>
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

        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>

