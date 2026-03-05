
<%@ page title="Lịch sử Trao đổi" language="C#" masterpagefile="~/home/MasterPageHome.master" autoeventwireup="true" inherits="home_lich_su_giao_dich, App_Web_md1cs0my" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="up_add" runat="server" UpdateMode="Conditional">
        <%--<Triggers>
     <asp:AsyncPostBackTrigger ControlID="but_show_form_add" EventName="Click" />
 </Triggers>--%>
        <ContentTemplate>
            <asp:Panel ID="pn_add" runat="server" Visible="false" DefaultButton="but_add_edit">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 600px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' id="A1" runat="server" onserverclick="but_close_form_add_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                RÚT ĐIỂM
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 606px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <%--pl-4 pl-8-md pr-8-md pr-4--%>
                            <div class="row">
                                <div class="cell-lg-12">
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Số Quyền tiêu dùng muốn rút</label>
                                        <div>
                                            <asp:TextBox ID="txt_dongA_chuyen" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_add_edit" runat="server" Text="XÁC NHẬN RÚT ĐIỂM" CssClass="button dark" OnClick="but_add_edit_Click" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_add">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="up_nap" runat="server" UpdateMode="Conditional">

        <ContentTemplate>
            <asp:Panel ID="pn_nap" runat="server" Visible="false" DefaultButton="but_nap">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 1200px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' id="A2" runat="server" onserverclick="but_close_form_nap_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                NẠP ĐIỂM
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 1206px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <div class="row mt-3 mb-5" style="justify-content: space-around">
                                <div class="cell-lg-7 mt-3" style="box-shadow: 7px 6px 20px 1px gray;
                                    border-radius: 10px;
                                    height: auto;
                                    padding: 20px 10px 10px 10px;">
                                    <label class="fw-700">Chi tiết tài khoản</label>
                                    <div>
                                        <asp:DropDownList ID="DropDownList1" runat="server" data-role="select">
                                            <asp:ListItem Text="Chọn" Value=""></asp:ListItem>
                                            <asp:ListItem Text="100 Quyền tiêu dùng" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="200 Quyền tiêu dùng" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="500 Quyền tiêu dùng" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="1.000 Quyền tiêu dùng" Value="4"></asp:ListItem>
                                            <asp:ListItem Text="2.000 Quyền tiêu dùng" Value="5"></asp:ListItem>
                                            <asp:ListItem Text="5.000 Quyền tiêu dùng" Value="6"></asp:ListItem>
                                            <asp:ListItem Text="10.000 Quyền tiêu dùng" Value="7"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div>
                                        <div class="mt-3">
                                            Quy đổi: 1 Quyền tiêu dùng = 1.000 VNĐ<br />
                                            Ví dụ: Muốn nạp 100 Quyền tiêu dùng, bạn sẽ phải chuyển khoản 100.000 VNĐ<br />
                                            <hr />
                                            <div style="
                                                background-image: url('/uploads/images/banner-atm.jpg');
                                                background-size: cover;
                                                background-position: top;
                                                height: 400px;
                                                position: relative;
                                                border-radius: 10px;
                                                overflow: hidden;
                                                color: white;
                                            ">
                                                <div style="
                                                    position: absolute;
                                                    bottom: 20px;
                                                    width: 100%;
                                                    text-align: center;
                                                    text-shadow: 1px 1px 4px black;
                                                ">
                                                    Số tài khoản xxx.xxxx.xxx<br />
                                                    Ngân hàng XXX Bank<br />
                                                    Tên tài khoản: XXX XXX
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <asp:CheckBox ID="CheckBox1" runat="server" Text="Tôi đã chuyển khoản" />
                                    </div>
                                     <div class="mt-6 text-right">
                                         <asp:Button ID="but_nap" runat="server" Text="NẠP ĐIỂM" CssClass="button dark" OnClick="but_nap_Click" />
                                     </div>
                                </div>
                                <div class="cell-lg-4 mt-3" style="box-shadow: 7px 6px 20px 1px gray;
                                    border-radius: 10px;
                                    height: 70px;
                                    padding: 20px 10px 10px 10px;
                                    display: flex;
                                    justify-content: space-between;">
                                    <label class="fw-700">Lịch sử Trao đổi</label>
                                    <a href="">Xem tất cả</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="up_nap">
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
                            <div class="mt-3">
                                <small>
                                    <div><b>Thông tin Gian hàng đối tác</b></div>
                                    <div>Tên shop</div>
                                    <div class="fw-600">
                                        <asp:Label ID="Label100" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div>Điện thoại</div>
                                    <div class="fw-600">
                                        <asp:Label ID="Label101" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div>Địa chỉ</div>
                                    <div class="fw-600">
                                        <asp:Label ID="Label102" runat="server" Text=""></asp:Label>
                                    </div>
                                </small>
                            </div>
                            <div class="mt-3">
                                <small>
                                    <div><b>Thông tin khách hàng</b></div>
                                    <div>Tên khách</div>
                                    <div class="fw-600">
                                        <asp:Label ID="Label103" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div>Điện thoại</div>
                                    <div class="fw-600">
                                        <asp:Label ID="Label104" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div>Địa chỉ</div>
                                    <div class="fw-600">
                                        <asp:Label ID="Label105" runat="server" Text=""></asp:Label>
                                    </div>
                                </small>
                            </div>
                            <div class="bcorn-fix-title-table-container mt-3">
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
                            <a><small>Lịch sử Trao đổi</small></a>
                        </li>
                        <li>
                            <asp:LinkButton ID="but_show_form_nap" OnClick="but_show_form_nap_Click" runat="server"><small>Nạp</small></asp:LinkButton>
                        </li>
                        <li>
                            <asp:LinkButton ID="but_show_form_add" OnClick="but_show_form_add_Click" runat="server"><small>Rút</small></asp:LinkButton>
                        </li>
                        <li class="bd-gray border bd-default mt-2 d-block-lg d-none" style="height: 24px"></li>


                        <li class="d-block-lg d-none">
                            <a data-role="hint" data-hint-position="top" data-hint-text="Hiển thị">
                                <small>
                                    <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                                </small></a>
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
                            <asp:Label ID="lb_show_md" runat="server" Text=""></asp:Label>
                        </small>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Lùi" ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" CssClass="button small light"><span class="mif-chevron-left"></span></asp:LinkButton>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Tới" ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" CssClass="button small light"><span class="mif-chevron-right"></span></asp:LinkButton>
                    </div>
                    <div class="clr-bc"></div>
                </div>
                <div class="fg-white">
                    <small>Quyền tiêu dùng là đơn vị quy ước nội bộ được hiển thị và sử dụng trên nền tảng ahasale.vn, đóng vai trò làm phương tiện trao đổi giá trị giữa các sản phẩm và dịch vụ thuộc hệ sinh thái Aha Sale.

<br />Người dùng có thể sử dụng Quyền tiêu dùng để thực hiện các Trao đổi trao đổi sản phẩm trên nền tảng theo đúng chính sách nội bộ.

<br />Tỷ lệ tham chiếu: 1.000 Quyền tiêu dùng = 1.000 VNĐ, nhằm giúp người dùng dễ dàng định giá và so sánh sản phẩm.

<br />Lưu ý: Quyền tiêu dùng không phải là tiền tệ hợp pháp và không có chức năng Trao đổi ngoài phạm vi nền tảng ahasale.vn. Việc sử dụng Quyền tiêu dùng chỉ có hiệu lực trong nội bộ hệ thống theo quy định của Aha Sale.</small>
                </div>
                <div class="row">
                    <div class="cell-lg-12">
                        <div class="bcorn-fix-title-table-container">
                            <table class="bcorn-fix-title-table">
                                <thead>
                                    <tr class="">
                                        <th style="width: 1px;">ID</th>
                                        <th style="width: 1px;">Ngày</th>
                                        <th style="min-width: 90px;">Quyền tiêu dùng</th>
                                        <th style="min-width: 150px;">Nội dung</th>
                                        <th style="width: 1px;">Đơn</th>
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
                                                <td><%#Eval("ngay","{0:dd/MM/yyyy}") %></td>
                                                <td class="text-right">
                                                    <asp:PlaceHolder ID="PlaceHolder19" runat="server" Visible='<%#Eval("CongTru").ToString()=="True" %>'>
                                                        <div class="mini rounded button info">+ <%#Eval("dongA","{0:#,##0}") %></div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("CongTru").ToString()=="False" %>'>
                                                        <div class="mini rounded button warning">- <%#Eval("dongA","{0:#,##0}") %></div>
                                                    </asp:PlaceHolder>
                                                    <img src="/uploads/images/dong-a.png" style="width: 20px!important" class="pl-1" />
                                                </td>
                                                <td class="text-left"><%# Eval("ghichu") %></td>
                                                <td>
                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("id_donhang").ToString()!="" %>'>
                                                        <div>
                                                            <asp:LinkButton ID="LinkButton1" OnClick="LinkButton1_Click" CommandArgument='<%# Eval("id_donhang") %>' runat="server" CssClass="mini rounded dark button">Chi tiết</asp:LinkButton>
                                                        </div>
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

