<%@ Control Language="C#" AutoEventWireup="true" CodeFile="menutop_webcon_uc.ascx.cs" Inherits="webcon_uc_menutop_webcon_uc" %>
<div class="container-fluid pos-fixed fixed-top  bg-darkIndigo " data-gradient-colors="#008a00, #003d00" data-role-gradientbox="true" style="z-index: 1000!important; background: linear-gradient(#008a00, #003d00);">
    <div class="fg-white container bg-transparent pl-0 pr-0" data-role="app-bar" data-expand-point="lg">
        <a href="/gianhang/webcon/Default.aspx?tkchinhanh=<%=Session["ten_tk_chinhanh"].ToString() %>" class="brand text-bold no-hover">
            <img src="<%=logo %>" height="30"></a>

        <div class="app-bar-container">
            <%if (Session["user_home_webcon"].ToString() != "")
                { %>
            <a href="#" class="app-bar-item  marker-light">
                <img src="<%=avt %>" class="img-cover-vuongtron w-h-28">
                <%--<span class="ml-2 app-bar-name d-none d-block-lg">admin</span>--%>
            </a>
            <div class="user-block shadow-1" data-role="collapse" data-collapsed="true" ><%--style="right: unset!important; left: 0px!important"--%>
                <%--style="right: unset!important; left: 0px!important" nằm bên trái--%>
                <div class="fg-white p-2 text-center bg-emerald">
                    <img src="<%=avt %>" class="img-cover-vuongtron w-h-100 border bg-white border-size-2 mt-2">
                    <div class="h5 mb-1 mt-1"><%=hoten %></div>
                    <div><%=Session["user_home_webcon"].ToString() %></div>
                </div>
                <div class="bg-white d-flex flex-justify-between flex-equal-items p-2 pb-2 fg-black">
                    <div class="text-center">
                        <div>
                            <img src="https://ahasale.vn/uploads/Images/e-aha.png" width="50" />
                        </div>
                        <div class=" mt-2">E-AHA</div>
                        <div class="mt-0 text-bold"><%=sodiem_eaha %></div>
                    </div>
                </div>
                <div class="bg-white d-flex flex-justify-between flex-equal-items p-2">
                    <%--<a href="/tai-khoan/edit.aspx" class="button light fg-black flat-button mr-1">Chỉnh sửa</a>--%>
                    <a href="/gianhang/webcon/doi-mat-khau.aspx?tkchinhanh=<%=Session["ten_tk_chinhanh"].ToString() %>" class="button light fg-black flat-button ml-1">Đổi mật khẩu</a>
                </div>
                <div class="bg-white d-flex flex-justify-between flex-equal-items p-2 bg-light fg-black">
                    <a href="/gianhang/webcon/ho-so.aspx?tkchinhanh=<%=Session["ten_tk_chinhanh"].ToString() %>" class="button success fg-white  mr-1">Hồ sơ</a>
                    <a runat="server" onserverclick="but_dangxuat_Click" class="button alert fg-white ml-1">Đăng xuất</a>
                </div>
            </div>
            <%}
                else
                { %>
            <div class="pos-relative">
                <a class="app-bar-item c-pointer <%-- dropdown-toggle marker-light--%>">
                    <span class="mif-user"></span>
                </a>
                <ul class="d-menu" data-role="dropdown">
                    <li><a class="c-pointer" onclick="show_hide_id_form_dangnhap()">Đăng nhập</a></li>
                    <li><a class="c-pointer" onclick="show_hide_id_form_dangky()">Đăng ký</a></li>
                </ul>
            </div>
            <%} %>
            <a class="app-bar-item" href="/gianhang/webcon/datlich.aspx?tkchinhanh=<%=Session["ten_tk_chinhanh"].ToString() %>" data-role="hint" data-hint-position="bottom" data-hint-text="Đặt lịch">
                <span class="mif mif-calendar"></span>
            </a>
            <div class="pos-relative" data-role="bottom" data-hint-position="top" data-hint-text="Giỏ hàng">
                <a class="app-bar-item c-pointer <%-- dropdown-toggle marker-light--%>">
                    <span class="mif-cart"></span>
                    <span class="badge bg-green fg-white mt-2 mr-1"><%=sl_hangtronggio %></span>
                </a>
                <ul class="d-menu" data-role="dropdown">
                    <li><a href="/gianhang/webcon/giohang.aspx?tkchinhanh=<%=Session["ten_tk_chinhanh"].ToString() %>">Giỏ hàng của bạn</a></li>
                    <%--<li class="divider"></li>--%>
                    <%--<li><a href="#">Tra cứu đơn hàng</a></li>--%>
                </ul>
            </div>

        </div>

        <ul class="app-bar-menu bg-transparent ml-auto text-upper">
            <%--<%=kq %>--%>
            <%--<li><a class="c-pointer"  onclick="show_hide_id_form_dangkytuvan()">Tư vấn</a></li>
            <li><a class="c-pointer"  onclick="show_hide_id_form_dangkybaogia()">Báo giá</a></li>--%>
        </ul>







        <%--<a class="app-bar-item fg-orange-hover" href="#">Đăng nhập</a>
        <a class="app-bar-item fg-orange-hover" href="#">Đăng ký</a>--%>
        <!--<div class="app-bar-container">
            <a class="app-bar-item dropdown-toggle marker-light" href="#">
                <span class="mif-user"></span>
            </a>
            <ul class="d-menu place-left" data-role="dropdown" style="margin-left: 130px">
                <li><a href="/dangnhap">Đăng nhập</a></li>
                <%--<li class="divider"></li>--%>
                <li><a href="/dangky">Đăng ký tài khoản</a></li>
            </ul>
        </div>
        <div class="app-bar-container">
            <a class="app-bar-item <%--dropdown-toggle marker-light--%>" href="#">
                <span class="mif-search"></span>
            </a>
            <ul class="d-menu place-left" data-role="collapse">
                <%--collapse dropdown--%>
                <li>
                    <asp:TextBox ID="TextBox1" runat="server" data-role="input" Width="224" placeholder="Tìm kiếm" ></asp:TextBox>
                </li>
            </ul>
        </div>
        <div class="app-bar-item <%--dropdown-toggle marker-light--%>" href="#">            
            <asp:TextBox ID="TextBox2" runat="server"  placeholder="Tìm kiếm" CssClass="app-bar-search"></asp:TextBox>
        </div>-->

    </div>
</div>
