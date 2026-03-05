<%@ Control Language="C#" AutoEventWireup="true" CodeFile="menu-top-uc.ascx.cs" Inherits="admin_uc_menu_top_uc" %>

<div data-role="charms" data-position="right" id="thongbao-charms" style="width: 320px; background-color: #fff; overflow: auto;" class="p-0 m-0 shadow-1 charms right-side">
    <div style="height: 52px; line-height: 55px" class="bg-orange fg-white">
        <div style="float: left"><span class="  ml-4 fg-white">THÔNG BÁO</span></div>
        <div style="float: right"><a href="#" class="fg-white" title="Đóng" onclick="window.ahaAdminCharms('close'); return false;"><span class="mif mif-cross mr-4"></span></a></div>
        <div style="clear: both"></div>
    </div>
    <%--<div style="position: absolute; top: 68px; right: 14px"><a href="#" class="fg-red fg-darkRed-hover"><small>Quản lý thông báo</small></a></div>--%>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="text-left p-3">
                <asp:Button ID="but_sapxep_moinhat" OnClick="but_sapxep_moinhat_Click" runat="server" Text="Mới nhất" CssClass="light small rounded" />
                <asp:Button ID="but_sapxep_chuadoc" OnClick="but_sapxep_chuadoc_Click" runat="server" Text="Chưa đọc" CssClass="light small rounded" />
                <a href="/admin/quan-ly-thong-bao/default.aspx" class="button warning small rounded">Quản lý</a>
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
                                <div class="thongbao-thoigian"><%#Eval("thoigian","{0:dd/MM/yyyy HH:mm}") %></div>
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
            <asp:PlaceHolder ID="ph_empty_thongbao" runat="server" Visible="false">
                <div class="px-3 pb-3 fg-gray">Không có thông báo nào.</div>
            </asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>


    <div class="text-center pt-3 pb-10">
        <a href="/admin/quan-ly-thong-bao/default.aspx" class="button warning small rounded">Xem tất cả</a>
    </div>
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
                            <asp:Button ID="but_doimatkhau" runat="server" Text="ĐỔI MẬT KHẨU" CssClass="button success" OnClick="but_doimatkhau_Click" />
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


<div data-role="appbar" class="fg-white bg-nmenutop-bc admin-topbar" data-expand-point="lg" style="position: fixed; top: 0; z-index: 3">

    <a href="#" class="app-bar-item d-block d-none-lg" id="paneToggle"><span class="mif-menu"></span></a>

    <a class="app-bar-item no-hover d-block-xl d-none"><span style="width: 250px" class="d-block-xl d-none"></span></a>
    <a class="app-bar-item fg-white" href="/admin/default.aspx"><span class="mif mif-home"></span></a>
    <a class="app-bar-item fg-white d-block-lg d-none fw-600" style="z-index: 10!important" href="/admin/default.aspx"><%=ViewState["title"] %></a>
    <a class="app-bar-item d-block-lg d-none" href="#" id="admin-ui-density-toggle" title="Thu gọn menu">
        <span class="mif mif-list"></span>
    </a>
    <div class="app-bar-container ml-auto">

        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:LinkButton ID="but_show_form_thongbao" OnClick="but_show_form_thongbao_Click" OnClientClick="window.ahaAdminCharms('toggle');" CssClass="app-bar-item" runat="server">
                    <span class="mif-notifications mif-lg"></span>
                    <span class="badge bg-orange fg-white mt-2 mr-1">
                        <asp:Label ID="lb_sl_thongbao" runat="server" Text="0"></asp:Label>
                    </span>
                </asp:LinkButton>
                <asp:Timer ID="Timer1" runat="server" Interval="5000" OnTick="Timer1_Tick"></asp:Timer>
            </ContentTemplate>
        </asp:UpdatePanel>

        <div class="app-bar-container admin-avatar-wrap admin-shop-dd" id="admin-avatar-shell" style="visibility:hidden; opacity:0;">
            <button type="button" class="app-bar-item admin-shop-avatar-btn" id="admin-avatar-toggle" aria-haspopup="true" aria-expanded="false">
                <img src="<%=ViewState["anhdaidien"] %>" class="admin-shop-avatar-image" alt="avatar" />
                <span class="app-bar-name admin-shop-avatar-name"><%=ViewState["taikhoan"] %></span>
                <span class="admin-shop-avatar-caret">&#9660;</span>
            </button>
            <div class="admin-shop-menu" id="admin-avatar-dropdown">
                <div class="admin-shop-menu-head">
                    <p class="admin-shop-menu-title"><%=ViewState["hoten"] %></p>
                    <div class="admin-shop-menu-sub"><%=ViewState["phanloai"] %> • <%=ViewState["taikhoan"] %></div>
                </div>
                <div class="admin-shop-menu-body">
                    <div class="admin-shop-menu-group-title">Trang chính</div>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/default.aspx") %>' href="/admin/default.aspx">
                        <span>Trang chủ admin</span>
                        <span class="admin-shop-menu-badge">Dashboard</span>
                    </a>

                    <div class="admin-shop-menu-group-title">Quản lý admin</div>
                    <a class='admin-shop-menu-item <%= MenuActiveTaiKhoanScope("admin") %>' href="/admin/quan-ly-tai-khoan/default.aspx?scope=admin">
                        <span>Quản lý tài khoản admin</span>
                    </a>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/lich-su-chuyen-diem/default.aspx") %>' href="/admin/lich-su-chuyen-diem/default.aspx">
                        <span>Lịch sử chuyển điểm</span>
                        <span class="admin-shop-menu-badge"><%=ViewState["DongA"] %></span>
                    </a>

                    <div class="admin-shop-menu-group-title">Quản lý home</div>
                    <a class='admin-shop-menu-item <%= MenuActiveTaiKhoanScope("home") %>' href="/admin/quan-ly-tai-khoan/default.aspx?scope=home">
                        <span>Quản lý tài khoản home</span>
                    </a>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/duyet-yeu-cau-len-cap.aspx") %>' href="/admin/duyet-yeu-cau-len-cap.aspx">
                        <span>Duyệt yêu cầu lên cấp</span>
                    </a>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/phat-hanh-the.aspx") %>' href="/admin/phat-hanh-the.aspx">
                        <span>Phát hành thẻ</span>
                    </a>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/motacapbac.aspx", "/admin/MoTaCapBac.aspx") %>' href="/admin/MoTaCapBac.aspx">
                        <span>Mô tả cấp bậc</span>
                    </a>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/he-thong-san-pham/ban-san-pham.aspx") %>' href="/admin/he-thong-san-pham/ban-san-pham.aspx">
                        <span>Bán sản phẩm</span>
                    </a>

                    <div class="admin-shop-menu-group-title">Quản lý shop</div>
                    <a class='admin-shop-menu-item <%= MenuActiveTaiKhoanScope("shop") %>' href="/admin/quan-ly-tai-khoan/default.aspx?scope=shop">
                        <span>Quản lý tài khoản shop</span>
                    </a>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/duyet-gian-hang-doi-tac.aspx") %>' href="/admin/duyet-gian-hang-doi-tac.aspx">
                        <span>Duyệt gian hàng đối tác</span>
                    </a>

                    <div class="admin-shop-menu-group-title">Quản lý nội dung</div>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/cai-dat-trang-chu/default.aspx") %>' href="/admin/cai-dat-trang-chu/default.aspx">
                        <span>Cài đặt trang chủ</span>
                    </a>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/quan-ly-menu/default.aspx") %>' href="/admin/quan-ly-menu/default.aspx">
                        <span>Quản lý menu</span>
                    </a>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/quan-ly-bai-viet/default.aspx", "/admin/quan-ly-bai-viet/in.aspx") %>' href="/admin/quan-ly-bai-viet/default.aspx">
                        <span>Quản lý bài viết</span>
                    </a>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/quan-ly-banner/default.aspx") %>' href="/admin/quan-ly-banner/default.aspx">
                        <span>Quản lý banner</span>
                    </a>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/quan-ly-gop-y/default.aspx") %>' href="/admin/quan-ly-gop-y/default.aspx">
                        <span>Quản lý góp ý</span>
                    </a>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/quan-ly-thong-bao/default.aspx", "/admin/quan-ly-thong-bao/in.aspx") %>' href="/admin/quan-ly-thong-bao/default.aspx">
                        <span>Quản lý thông báo</span>
                    </a>
                    <a class='admin-shop-menu-item <%= MenuActive("/admin/yeu-cau-tu-van/default.aspx") %>' href="/admin/yeu-cau-tu-van/default.aspx">
                        <span>Yêu cầu tư vấn</span>
                    </a>
                </div>
                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="admin-shop-menu-footer">
                            <asp:LinkButton ID="but_show_form_doimatkhau" OnClick="but_show_form_doimatkhau_Click" runat="server" CssClass="admin-shop-btn admin-shop-btn-light">Đổi mật khẩu</asp:LinkButton>
                            <asp:Button ID="but_dangxuat" runat="server" Text="Đăng xuất" CssClass="admin-shop-btn admin-shop-btn-danger" OnClick="but_dangxuat_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</div>


