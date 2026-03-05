<%@ control language="C#" autoeventwireup="true" inherits="home_uc_menu_top_home_uc, App_Web_243tslvg" %>

<div data-role="charms" data-position="right" id="thongbao-charms" style="width: 320px; background-color: #fff; overflow: auto;" class="p-0 m-0 shadow-1">
    <div style="height: 52px; line-height: 55px" class="bg-orange fg-white">
        <div style="float: left"><span class="  ml-4 fg-white">THÔNG BÁO</span></div>
        <div style="float: right"><a href="#" class="fg-white" title="Đóng" onclick="Metro.getPlugin('#thongbao-charms', 'charms').close()"><span class="mif mif-cross mr-4"></span></a></div>
        <div style="clear: both"></div>
    </div>
    <%--<div style="position: absolute; top: 68px; right: 14px"><a href="#" class="fg-red fg-darkRed-hover"><small>Quản lý thông báo</small></a></div>--%>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="text-left p-3">
                <asp:Button ID="but_sapxep_moinhat" OnClick="but_sapxep_moinhat_Click" runat="server" Text="Mới nhất" CssClass="light small rounded" />
                <asp:Button ID="but_sapxep_chuadoc" OnClick="but_sapxep_chuadoc_Click" runat="server" Text="Chưa đọc" CssClass="light small rounded" />
                <a href="/home/quan-ly-thong-bao/default.aspx" class="button warning small rounded">Quản lý</a>
            </div>
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>

                    <div class="thongbao-div pt-2 pb-2 pl-3 pr-3">
                        <a href="<%#Eval("link").ToString()%>idtb=<%#Eval("id").ToString() %>">
                            <div class="thongbao-avt">
                                <img src="<%#Eval("avt_nguoithongbao") %>" width="50" height="50" class="img-cover-vuongtron" />
                            </div>
                            <div class="thongbao-noidung">
                                <div class="thongbao-noidungchinh">
                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("daxem").ToString()=="True" %>'>
                                        <%#Eval("noidung").ToString() %>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("daxem").ToString()=="False" %>'>
                                        <span class="fg-orange"><%#Eval("noidung").ToString() %></span>
                                    </asp:PlaceHolder>
                                </div>
                                <div class="thongbao-thoigian"><%#Eval("thoigian","{0:dd/MM/yyyy HH:mm}") %>'</div>
                            </div>
                        </a>
                        <div class="thongbao-hanhdong">
                            <div class="dropdown-button bg-transparent">
                                <span class="button <%--dropdown-toggle--%> bg-transparent"><span class="text-bold" style="font-size: 18px">...</span></span>
                            </div>
                            <ul class="d-menu place-right" data-role="dropdown">
                                <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("daxem").ToString()=="False" %>'>
                                    <li>
                                        <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="but_dadoc" OnClick="but_dadoc_Click" runat="server" Text="Đánh dấu đã đọc"></asp:LinkButton>
                                    </li>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("daxem").ToString()=="True" %>'>
                                    <li>
                                        <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="but_chuadoc" OnClick="but_chuadoc_Click" runat="server" Text="Đánh dấu chưa đọc"></asp:LinkButton>
                                    </li>
                                </asp:PlaceHolder>
                                <li>
                                    <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="but_xoathongbao" OnClick="but_xoathongbao_Click" runat="server" Text="Xóa thông báo này"></asp:LinkButton>
                                </li>
                            </ul>
                        </div>

                        <div class="clr-bc"></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <div class="text-center pt-3 pb-10">
                <a href="/home/quan-ly-thong-bao/default.aspx" class="button warning small rounded">Xem tất cả</a>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>


    <asp:PlaceHolder ID="PlaceHolder7" runat="server">
        <div class="text-left p-3 fg-black">Vui lòng đăng nhập để xem thông báo.</div>
    </asp:PlaceHolder>
</div>


<asp:UpdatePanel ID="up_doimatkhau" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pn_doimatkhau" runat="server" Visible="false" DefaultButton="but_doimatkhau">
            <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                <div style='top: 0; left: 0px; margin: 0 auto; max-width: 440px; opacity: 1;'>
                    <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                        <a href='#' class='fg-white d-inline' id="A5" runat="server" onserverclick="but_close_doimatkhau_Click" title='Đóng'>
                            <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                        </a>
                    </div>
                    <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                        <div class="pt-4 text-upper text-bold">
                            Đổi mật khẩu
                        </div>
                        <hr />
                    </div>
                </div>
            </div>
            <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                <div style='top: 0; left: 0; margin: 0 auto; max-width: 446px; opacity: 1;'>
                    <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                        <div class="mt-3 ">
                            <div class="fw-600">Mật khẩu hiện tại</div>
                            <asp:TextBox ID="TextBox1" runat="server" data-role="input" TextMode="Password"></asp:TextBox>
                        </div>
                        <div class="mt-3 ">
                            <div class="fw-600">Mật khẩu mới</div>
                            <asp:TextBox ID="TextBox2" runat="server" data-role="input" TextMode="Password"></asp:TextBox>
                        </div>
                        <div class="mt-3 ">
                            <div class="fw-600">Nhập lại mật khẩu mới</div>
                            <asp:TextBox ID="TextBox3" runat="server" data-role="input" TextMode="Password"></asp:TextBox>
                        </div>
                        <div class="mt-3 ">
                            <div class="fw-600 fg-red"><i>Bạn sẽ phải đăng nhập lại sau khi đổi mật khẩu.</i></div>
                        </div>
                        <div class="mt-6 mb-20 text-right">
                            <asp:Button ID="but_doimatkhau" runat="server" Text="ĐỔI MẬT KHẨU" CssClass="button dark" OnClick="but_doimatkhau_Click" />
                        </div>
                        <div class="mb-20"></div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="UpdateProgress12" runat="server" AssociatedUpdatePanelID="up_doimatkhau">
    <ProgressTemplate>
        <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
            <div style="padding-top: 45vh;">
                <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
            </div>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>

<asp:UpdatePanel ID="up_doipin" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pn_doipin" runat="server" Visible="false" DefaultButton="but_doipin">
            <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                <div style='top: 0; left: 0px; margin: 0 auto; max-width: 440px; opacity: 1;'>
                    <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                        <a href='#' class='fg-white d-inline' id="A1" runat="server" onserverclick="but_close_doipin_Click" title='Đóng'>
                            <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                        </a>
                    </div>
                    <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                        <div class="pt-4 text-upper text-bold">
                            Đổi pin
                        </div>
                        <hr />
                    </div>
                </div>
            </div>
            <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                <div style='top: 0; left: 0; margin: 0 auto; max-width: 446px; opacity: 1;'>
                    <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                        <div class="mt-3 ">
                            <div class="fw-600">Mã pin hiện tại</div>
                            <asp:TextBox ID="TextBox4" placeholder="Nhập mã gồm 4 ký tự" runat="server" data-role="input" TextMode="Password" MaxLength="4"></asp:TextBox>
                        </div>
                        <div class="mt-3 ">
                            <div class="fw-600">Mã pin mới</div>
                            <asp:TextBox ID="TextBox5" placeholder="Nhập mã gồm 4 ký tự" runat="server" data-role="input" TextMode="Password" MaxLength="4"></asp:TextBox>
                        </div>
                        <div class="mt-3 ">
                            <div class="fw-600">Nhập lại mã pin mới</div>
                            <asp:TextBox ID="TextBox6" placeholder="Nhập mã gồm 4 ký tự" runat="server" data-role="input" TextMode="Password" MaxLength="4"></asp:TextBox>
                        </div>
                        <div class="mt-6 mb-20 text-right">
                            <asp:Button ID="but_restpin" runat="server" Text="QUÊN MÃ PIN" CssClass="button" OnClick="but_restpin_Click" />
                            <asp:Button ID="but_doipin" runat="server" Text="ĐỔI PIN" CssClass="button dark" OnClick="but_doipin_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_doipin">
    <ProgressTemplate>
        <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
            <div style="padding-top: 45vh;">
                <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
            </div>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>

<%--<div class="container-fluid" data-gradient-colors="#ffba00, #ffeb00" data-role-gradientbox="true" style="z-index: 1000!important; position: fixed; top: 0; background: linear-gradient(#ffba00, #ffeb00);">--%>
<div class="container-fluid" style="z-index: 1000!important; position: fixed; top: 0; background-color: #181a20">
    <div class="fg-black container bg-transparent pl-0 pr-0" data-role="app-bar" data-expand-point="lg">
        <a href="/" class="brand text-bold fg-yellow">
            <img src="/uploads/images/favicon.png" width="28" class="pr-1" />AHASALE</a>


        <div class="app-bar-container">

            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible="false">
                        <a href="#" class="app-bar-item">
                            <img src="<%=ViewState["anhdaidien"] %>" class="img-cover-vuongtron" width="28" height="28">
                            <%--<span class="ml-2 app-bar-name d-block-lg d-none"><%=ViewState["taikhoan"] %></span>--%>
                        </a>
                        <div class="user-block shadow-1" style="background-color: #1e2329" data-role="collapse" data-collapsed="true">
                            <div class=" fg-ahasale p-2 text-center ">
                                <%--style="background-color: #222d32"--%>
                                <img src="<%=ViewState["anhdaidien"] %>" width="100" height="100" class="img-cover-vuongtron border bg-white border-size-2 mt-2">
                                <div class="text-bold mt-1 mb-1"><%=ViewState["hoten"] %></div>
                                <div class=""><%=ViewState["taikhoan"] %>  <a href="/home/edit-info.aspx" class="button mini ml-1  bg-yellow fg-black bg-amber-hover  flat-button rounded"><span class="mif-pencil pr-1"></span>Chỉnh sửa</a></div>
                                <%-- <div><%=ViewState["email"] %></div>--%>
                            </div>
                            <div class=" d-flex flex-justify-between flex-equal-items p-2" style="background-color: #1e2329">
                                <%=ViewState["phanloai"] %>
                                <a href="/home/lich-su-giao-dich.aspx" class="button flat-button bg-gray  ml-1 rounded">
                                    <img src="/uploads/images/dong-a.png" />
                                    <span class="text-bold"><%=ViewState["DongA"] %></span>
                                </a>
                            </div>

                            <div class="d-flex flex-justify-between flex-equal-items p-2" style="background-color: #1e2329">
                                <a href="/<%=ViewState["taikhoan"] %>.info" class="button small mr-1  bg-lightGray bg-gray-hover fg-black  mr-1 flat-button rounded">Hồ sơ</a>
                                <asp:LinkButton ID="but_show_form_doipin" OnClick="but_show_form_doipin_Click" runat="server" Width="" CssClass="button small mr-1 ml-1 bg-lightGray bg-gray-hover fg-black flat-button rounded">Đổi pin</asp:LinkButton>
                                <asp:LinkButton ID="but_show_form_doimatkhau" OnClick="but_show_form_doimatkhau_Click" runat="server" Width="" CssClass="button small ml-1 bg-lightGray bg-gray-hover fg-black flat-button rounded">Đổi pass</asp:LinkButton>
                                <%--<button class="button flat-button">Sales</button>
                        <button class="button flat-button">Friends</button>--%>
                            </div>
                            <div class="fg-white text-center pb-2 pt-1" style="background-color: #1e2329">
                                <img src="<%=ViewState["qr_code"] %>" width="70" height="70">
                            </div>
                            <div class=" d-flex flex-justify-between flex-equal-items p-2" style="background-color: #1e2329">
                                <a href="/home/khach-hang.aspx" class="button small flat-button light rounded mr-1">Khách hàng</a>
                                <a href="/home/don-mua.aspx" class="button small flat-button light rounded ml-1 mr-1">Đơn mua
                                </a>
                                <a href="/home/don-ban.aspx" class="button small flat-button light rounded ml-1">Đơn bán
                                </a>
                            </div>
                            <div class="d-flex flex-justify-between flex-equal-items p-2 " style="background-color: #1e2329">
                                <a href="/home/quan-ly-tin/default.aspx" class="button mr-1  bg-yellow fg-black bg-amber-hover  flat-button rounded">Quản lý tin</a>
                                <asp:Button ID="but_dangxuat" runat="server" Text="Đăng xuất" CssClass="alert ml-1 flat-button rounded " OnClick="but_dangxuat_Click" />
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="PlaceHolder6" runat="server">
                        <div class="pos-relative">
                            <a href="/dang-nhap" style="color: #eaecef" class="app-bar-item c-pointer fg-yellow-hover"><span class="mif-user mif-lg"></span>Đăng nhập</a>
                        </div>
                    </asp:PlaceHolder>
                </ContentTemplate>
            </asp:UpdatePanel>


            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="app-bar-container">
                        <a class="app-bar-item" href="/home/gio-hang.aspx">
                            <span class="mif-cart  fg-ahasale"></span>
                            <span class="badge bg-yellow fg-black mt-2 mr-1">
                                <asp:Label ID="lb_sl_giohang" runat="server" Text="0"></asp:Label>
                            </span>
                        </a>

                        <asp:LinkButton ID="but_show_form_thongbao" OnClick="but_show_form_thongbao_Click" OnClientClick="Metro.getPlugin('#thongbao-charms', 'charms').toggle()" CssClass="app-bar-item" runat="server">
                            <span class="mif-notifications mif-lg fg-ahasale"></span>
                            <span class="badge bg-yellow fg-black mt-2 mr-1">
                                <asp:Label ID="lb_sl_thongbao" runat="server" Text="0"></asp:Label>
                            </span>
                        </asp:LinkButton>
                    </div>
                    <asp:Timer ID="Timer1" runat="server" Interval="5000" OnTick="Timer1_Tick"></asp:Timer>
                </ContentTemplate>
            </asp:UpdatePanel>


            <!--<div class="pos-relative">
                <a class="app-bar-item c-pointer <%-- dropdown-toggle marker-light--%>">
                    <span class="mif-cart"></span>
                    <span class="badge bg-white fg-black mt-2 mr-1">0</span>
                </a>
                <ul class="d-menu" data-role="dropdown">
                    <li><a href="#">Giỏ hàng của bạn</a></li>
                    <%--<li class="divider"></li>--%>
                    <li><a href="#">Tra cứu đơn hàng</a></li>
                </ul>
            </div>-->
        </div>

        <%--<div class="app-bar-container ml-auto">
            <a class="app-bar-item c-pointer ">
                <span class="mif-menu mif-2x"></span>
            </a>
            <ul class="d-menu place-right" data-role="dropdown" style="overflow:auto !important">
                <%=show_menu %>
            </ul>
        </div>--%>

        <ul class='app-bar-menu bg-transparent ml-auto'>
            <%=show_menu %>
            <%--<li><a href='#'>Home</a></li>
            <li><a href='#' class='dropdown-toggle marker-light'><span class="mif-menu"></span></a>
                <ul class='d-menu place-right' data-role='dropdown'>
                    <li><a href='#'>Windows 10</a></li>
                    <li class='divider bg-lightGray'></li>
                </ul>
            </li>--%>
        </ul>
    </div>
</div>
