<%@ page title="Đơn bán" language="C#" masterpagefile="~/home/MasterPageHome.master" autoeventwireup="true" inherits="home_don_ban, App_Web_md1cs0my" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="up_taodon" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_taodon" runat="server" Visible="false" DefaultButton="but_taodon">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 650px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' id="A1" runat="server" onserverclick="but_close_form_taodon_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                Tạo đơn Offline
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 656px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">

                            <div class="bcorn-fix-title-table-container">
                                <table class="bcorn-fix-title-table">
                                    <thead>
                                        <tr class="">
                                            <th style="width: 1px;">ID</th>
                                            <th style="width: 1px;">Ảnh</th>
                                            <th class="text-left" style="min-width: 180px;">Tên sản phẩm</th>
                                            <th style="min-width: 90px;">Giá (Quyền tiêu dùng)</th>
                                            <th style="width: 1px;">Số lượng</th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        <asp:Repeater ID="Repeater3" runat="server">
                                            <ItemTemplate>
                                                <span style="display: none">
                                                    <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                                    <asp:Label ID="lbGiaBan" runat="server" Text='<%#Eval("giaban") %>'></asp:Label>
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
                                                    <td class="text-right"><%#Eval("giaban","{0:#,##0}") %><img src="/uploads/images/dong-a.png" style="width: 20px!important" class="pl-1" /></td>
                                                    <td>
                                                        <asp:TextBox onfocus="AutoSelect(this)" data-role="spinner" data-min-value="0" data-max-value="999" CssClass="input-small" data-clear-button="false" oninput="format_sotien_new(this)" ID="txt_sl_1" Width="40" MaxLength="3" runat="server" Text='0' onkeypress="if (event.keyCode==13) return false;"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_taodon" OnClick="but_taodon_Click" runat="server" Text="TẠO ĐƠN" CssClass="button dark small rounded" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_taodon">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>




    <asp:UpdatePanel ID="up_chitiet" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_chitiet" runat="server" Visible="false">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 650px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' id="close_add" runat="server" onserverclick="but_close_form_chitiet_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                Chi tiết đơn hàng
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 656px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <div class="bcorn-fix-title-table-container">
                                <table class="bcorn-fix-title-table">
                                    <thead>
                                        <tr class="">
                                            <th style="width: 1px;">ID</th>
                                            <th style="width: 1px;">Ảnh</th>
                                            <th class="text-left" style="min-width: 180px;">Tên sản phẩm</th>
                                            <th style="min-width: 90px;">Giá (Quyền tiêu dùng)</th>
                                            <th style="min-width: 80px;">Số lượng</th>
                                            <th style="min-width: 90px;">Trao đổi</th>
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
                                                    <td class="text-right"><%#Eval("giaban","{0:#,##0}") %><img src="/uploads/images/dong-a.png" style="width: 20px!important" class="pl-1" /></td>
                                                    <td>
                                                        <%#Eval("soluong") %>
                                                    </td>
                                                    <td class="text-right"><%#Eval("thanhtien","{0:#,##0}") %><img src="/uploads/images/dong-a.png" style="width: 20px!important" class="pl-1" /></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_dagiaohang" OnClick="but_dagiaohang_Click" runat="server" Text="Xác nhận đã giao hàng" CssClass="button dark small rounded" />
                                <asp:Button ID="but_huydonhang" OnClick="but_huydonhang_Click" runat="server" Text="Hủy đơn hàng" CssClass="button alert small rounded" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="up_chitiet">
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
            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" style="position: fixed; top: 52px; width: 100%; z-index: 4">
                    <ul class="h-menu bg-white">
                        <li>
                            <asp:LinkButton ID="but_show_form_taodon" OnClick="but_show_form_taodon_Click" runat="server"><small>Tạo đơn</small></asp:LinkButton>
                        </li>

                        <li class="bd-gray border bd-default mt-2 d-block-lg d-none" style="height: 24px"></li>

                        <li class="d-block-lg d-none">
                            <a data-role="hint" data-hint-position="top" data-hint-text="Hiển thị">
                                <small>
                                    <asp:Label ID="lb_show" runat="server" Text=""></asp:Label></small></a>
                        </li>
                        <li class="d-block-lg d-none" data-role="hint" data-hint-position="top" data-hint-text="Lùi">
                            <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server"><span class="mif-chevron-left"></span></asp:LinkButton>
                        </li>
                        <li class="d-block-lg d-none" data-role="hint" data-hint-position="top" data-hint-text="Tới">
                            <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server"><span class="mif-chevron-right"></span></asp:LinkButton>
                        </li>
                    </ul>
                </div>
                <div id="timkiem-fixtop-bc" style="position: fixed; right: 10px; top: 58px; width: 240px; z-index: 4" class="d-none d-block-sm">
                    <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem" runat="server" placeholder="Nhập từ khóa" data-role="input" CssClass="input-small" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                </div>
            </div>

            <div class="container">
                <div class="d-none-sm d-block">
                    <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem1" runat="server" placeholder="Nhập từ khóa" data-role="input" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                </div>
                <div class="d-none-lg d-block mb-3 mt-0-lg mt-3">
                    <div class="place-left">
                        <%--<b><%=ViewState["title"] %></b> Nó k kịp lưu vì nó tải trang này trước khi load menu-left--%>
                    </div>
                    <div class="place-right text-right">

                        <small class="pr-1">
                            <asp:Label ID="lb_show_md" runat="server" Text=""></asp:Label></small>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Lùi" ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" CssClass="button small light"><span class="mif-chevron-left"></span></asp:LinkButton>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Tới" ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" CssClass="button small light"><span class="mif-chevron-right"></span></asp:LinkButton>
                    </div>
                    <div class="clr-bc"></div>
                </div>
                <div class="rowe">
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
                                        <th style="width: 1px;">Ngày</th>
                                        <th style="width: 1px;">Loại</th>
                                        <th style="min-width: 80px;">Người nhận</th>
                                        <th style="min-width: 150px;">Địa chỉ</th>
                                        <th style="min-width: 90px;">Trao đổi</th>
                                        <th style="min-width: 80px;">Trạng thái</th>

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
                                                <td><%#Eval("ngaydat","{0:dd/MM/yyyy}") %>
                                                    <div>
                                                        <asp:LinkButton ID="LinkButton1" OnClick="LinkButton1_Click" CommandArgument='<%# Eval("id") %>' runat="server" CssClass="mini rounded dark button">Chi tiết</asp:LinkButton>
                                                    </div>
                                                </td>
                                                <td>
                                                    <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("online_offline").ToString()=="True" %>'>
                                                        <div class="button mini yellow rounded">Online</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("online_offline").ToString()=="False" %>'>
                                                        <div class="button mini primary rounded">Offline</div>
                                                    </asp:PlaceHolder>
                                                </td>
                                                <td class="text-left">
                                                    <div><%# Eval("hoten_nguoinhan") %></div>
                                                    <div><span class="mif-phone pr-1"></span><%# Eval("sdt_nguoinhan") %></div>
                                                </td>
                                                <td class="text-left">
                                                    <div><%# Eval("diahchi_nguoinhan") %></div>
                                                </td>
                                                <td class="text-right"><%#Eval("tongtien","{0:#,##0}") %><img src="/uploads/images/dong-a.png" style="width: 20px!important" class="pl-1" />
                                                </td>

                                                <td>
                                                    <asp:PlaceHolder ID="PlaceHolder19" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã đặt" %>'>
                                                        <div class="button mini info rounded">Đã đặt</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã hủy" %>'>
                                                        <div class="button mini alert rounded">Đã hủy</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã giao" %>'>
                                                        <div class="button mini yellow rounded">Đã giao</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã nhận" %>'>
                                                        <div class="button mini success rounded">Đã nhận</div>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("trangthai").ToString()=="Chưa Trao đổi" %>'>
                                                        <div class="button mini warning rounded">Chưa Trao đổi</div>
                                                        <div>
                                                            <asp:LinkButton ID="but_chothanhtoan" OnClick="but_chothanhtoan_Click" CommandArgument='<%# Eval("id") %>' runat="server" CssClass="button mini alert rounded">Kích hoạt chờ Trao đổi</asp:LinkButton>
                                                        </div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#Eval("trangthai").ToString()=="Chờ Trao đổi" %>'>
                                                        <div class="button mini alert rounded ani-flash">Chờ Trao đổi</div>
                                                        <div>
                                                            <asp:LinkButton ID="but_huychothanhtoan" CommandArgument='<%# Eval("id") %>' OnClick="but_huychothanhtoan_Click" runat="server" CssClass="button mini alert rounded">Hủy chờ Trao đổi</asp:LinkButton>
                                                        </div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã Trao đổi" %>'>
                                                        <div class="button mini primary rounded">Đã Trao đổi</div>
                                                    </asp:PlaceHolder>
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

