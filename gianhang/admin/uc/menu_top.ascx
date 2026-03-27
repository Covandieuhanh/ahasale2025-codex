<%@ Control Language="C#" AutoEventWireup="true" CodeFile="menu_top.ascx.cs" Inherits="admin_uc_menu_top_uc" %>
<%@ Register Src="~/gianhang/admin/uc/menu_dropdown.ascx" TagPrefix="uc1" TagName="menudropdown" %>
<%@ Register Src="~/Uc/Shared/SpaceLauncher_uc.ascx" TagPrefix="uc2" TagName="SpaceLauncher" %>

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
                <a href="/gianhang/admin/quan-ly-thong-bao/default.aspx" class="button warning small rounded">Quản lý</a>
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
        <a href="/gianhang/admin/quan-ly-thong-bao/default.aspx" class="button warning small rounded">Xem tất cả</a>
    </div>
</div>


<asp:UpdatePanel ID="up_doimatkhau" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pn_doimatkhau" runat="server" Visible="false" DefaultButton="but_doimatkhau">
            <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                <div style='top: 0; left: 0px; margin: 0 auto; max-width: 440px; opacity: 1;'>
                            <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                                <asp:LinkButton ID="but_close_doimatkhau" runat="server" CssClass="fg-white d-inline" CausesValidation="false" OnClick="but_close_doimatkhau_Click" ToolTip="Đóng">
                                    <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                                </asp:LinkButton>
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
                            <div class="aha-password-field">
                                <asp:TextBox ID="TextBox1" runat="server" data-role="input" TextMode="Password"></asp:TextBox>
                                <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện mật khẩu hiện tại">
                                    <span class="aha-password-toggle-label">Hiện</span>
                                </button>
                            </div>
                        </div>
                        <div class="mt-3 ">
                            <div class="fw-600">Mật khẩu mới</div>
                            <div class="aha-password-field">
                                <asp:TextBox ID="TextBox2" runat="server" data-role="input" TextMode="Password"></asp:TextBox>
                                <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện mật khẩu mới">
                                    <span class="aha-password-toggle-label">Hiện</span>
                                </button>
                            </div>
                        </div>
                        <div class="mt-3 ">
                            <div class="fw-600">Nhập lại mật khẩu mới</div>
                            <div class="aha-password-field">
                                <asp:TextBox ID="TextBox3" runat="server" data-role="input" TextMode="Password"></asp:TextBox>
                                <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện xác nhận mật khẩu mới">
                                    <span class="aha-password-toggle-label">Hiện</span>
                                </button>
                            </div>
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

    <uc2:SpaceLauncher runat="server" ID="spaceLauncher" ButtonCssClass="app-bar-item fg-white" />
    <a href="#" class="app-bar-item d-block d-none-lg" id="paneToggle"><span class="mif-menu"></span></a>

    <a class="app-bar-item fg-white admin-topbar-home" href="/gianhang/admin/default.aspx"><span class="mif mif-home"></span></a>
    <a class="app-bar-item fg-white d-block-lg d-none fw-600 admin-topbar-title" style="z-index: 10!important" href="/gianhang/admin/default.aspx"><%=ViewState["title"] %></a>
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
            </ContentTemplate>
        </asp:UpdatePanel>

        <div class="app-bar-container admin-quick-dd" id="admin-quick-shell">
            <button type="button" class="app-bar-item admin-quick-btn" id="admin-quick-toggle" aria-haspopup="true" aria-expanded="false" title="Tạo nhanh">
                <span class="mif mif-plus"></span>
            </button>
            <div class="admin-quick-menu" id="admin-quick-dropdown">
                <ul class="admin-quick-list">
                    <li><a href="/gianhang/admin/gianhang/default.aspx">Không gian /gianhang</a></li>
                    <li><a href="/gianhang/admin/gianhang/trung-tam.aspx">Trung tâm /gianhang</a></li>
                    <li><a href="/gianhang/admin/gianhang/trinh-bay.aspx">Trình bày storefront</a></li>
                    <li><a href="/gianhang/admin/gianhang/trang-cong-khai.aspx">Trang công khai</a></li>
                    <li><a href="/gianhang/admin/gianhang/quan-ly-noi-dung.aspx">Nội dung /gianhang</a></li>
                    <li><a href="/gianhang/admin/gianhang/san-pham.aspx">Sản phẩm /gianhang</a></li>
                    <li><a href="/gianhang/admin/gianhang/dich-vu.aspx">Dịch vụ /gianhang</a></li>
                    <li><a href="/gianhang/admin/gianhang/bai-viet.aspx">Bài viết /gianhang</a></li>
                    <li><a href="/gianhang/admin/gianhang/khach-hang.aspx">Khách hàng /gianhang</a></li>
                    <li><a href="/gianhang/admin/gianhang/lich-hen.aspx">Lịch hẹn /gianhang</a></li>
                    <li><a href="/gianhang/admin/gianhang/tao-giao-dich.aspx">Tạo giao dịch</a></li>
                    <li><a href="/gianhang/admin/gianhang/don-ban.aspx">Đơn gian hàng</a></li>
                    <li><a href="/gianhang/admin/gianhang/cho-thanh-toan.aspx">Chờ thanh toán</a></li>
                    <li><a href="/gianhang/admin/gianhang/don-mua.aspx">Buyer-flow / Đơn mua</a></li>
                    <li><a href="/gianhang/admin/gianhang/gio-hang.aspx">Giỏ hàng /gianhang</a></li>
                    <li><a href="/gianhang/admin/gianhang/hoa-don-dien-tu.aspx">Hóa đơn điện tử</a></li>
                    <li><a href="/gianhang/admin/gianhang/tien-ich.aspx">Tiện ích /gianhang</a></li>
                    <li><a href="/gianhang/admin/gianhang/bao-cao.aspx">Báo cáo gian hàng</a></li>
                    <li class="admin-quick-divider"></li>
                    <li><a href="/gianhang/admin/quan-ly-hoa-don/Default.aspx?q=add">Tạo hóa đơn</a></li>
                    <li><a href="/gianhang/admin/quan-ly-khach-hang/Default.aspx?q=add">Tạo khách hàng</a></li>
                    <li><a href="/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx?q=add">Đặt lịch hẹn</a></li>
                    <li><a href="/gianhang/admin/quan-ly-the-dich-vu/Default.aspx?q=add">Bán thẻ dịch vụ</a></li>
                    <li class="admin-quick-divider"></li>
                    <li><a href="/gianhang/admin/quan-ly-kho-hang/nhap-hang.aspx?q=nh">Nhập hàng</a></li>
                    <li><a href="/gianhang/admin/quan-ly-vat-tu/nhap-vat-tu.aspx?q=nh">Nhập vật tư</a></li>
                    <li class="admin-quick-divider"></li>
                    <li><a href="/gianhang/admin/quan-ly-thu-chi/add.aspx">Tạo thu chi</a></li>
                    <li class="admin-quick-divider"></li>
                    <li><a href="/gianhang/admin/quan-ly-bai-viet/add.aspx">Đăng bài viết</a></li>
                    <li><a href="/gianhang/admin/quan-ly-tai-khoan/add.aspx">Thêm nhân sự / chuyên gia</a></li>
                </ul>
            </div>
        </div>

        <details class="storefront-account admin-storefront-account" id="admin-avatar-shell">
            <summary class="storefront-account__summary admin-storefront-account__summary">
                <span class="storefront-account__avatar<%=HasAdminDropdownAvatar() ? "" : " storefront-account__avatar--empty" %>">
                    <% if (HasAdminDropdownAvatar()) { %>
                    <img src="<%=GetAdminDropdownAvatarUrl() %>" alt="<%=GetAdminDropdownDisplayName() %>" />
                    <% } else { %>
                    <span><%=GetAdminDropdownInitial() %></span>
                    <% } %>
                </span>
                <span class="storefront-account__meta admin-storefront-account__meta">
                    <strong><%=GetAdminDropdownAccountKey() %></strong>
                    <small><%=GetAdminDropdownStatusText() %></small>
                </span>
            </summary>
            <div class="storefront-account__menu admin-storefront-account__menu">
                <div class="storefront-account__profile admin-storefront-account__profile">
                    <span class="storefront-account__profile-avatar<%=HasAdminDropdownAvatar() ? "" : " storefront-account__profile-avatar--empty" %>">
                        <% if (HasAdminDropdownAvatar()) { %>
                        <img src="<%=GetAdminDropdownAvatarUrl() %>" alt="<%=GetAdminDropdownDisplayName() %>" />
                        <% } else { %>
                        <span><%=GetAdminDropdownInitial() %></span>
                        <% } %>
                    </span>
                    <div class="storefront-account__profile-copy admin-storefront-account__profile-copy">
                        <strong><%=GetAdminDropdownDisplayName() %></strong>
                        <small><%=GetAdminDropdownAccountKey() %></small>
                        <span class="storefront-account__profile-status admin-storefront-account__profile-status"><%=GetAdminDropdownStatusText() %></span>
                    </div>
                </div>
                <div class="storefront-account__space-list admin-storefront-account__space-list">
                    <asp:PlaceHolder ID="ph_workspace_switcher" runat="server" Visible="false">
                        <asp:Literal ID="lit_workspace_switcher" runat="server" />
                    </asp:PlaceHolder>
                    <uc1:menudropdown runat="server" ID="menudropdown" />
                    <asp:LinkButton ID="but_show_form_doimatkhau" runat="server" CssClass="storefront-account-space__link admin-storefront-account__link admin-storefront-account__link-secondary" CausesValidation="false" OnClick="but_show_form_doimatkhau_Click">Đổi mật khẩu</asp:LinkButton>
                </div>
                <div class="storefront-account__footer admin-storefront-account__footer">
                    <asp:LinkButton ID="but_dangxuat" runat="server" CssClass="storefront-account__logout admin-storefront-account__logout" OnClick="but_dangxuat_Click">
                        <span class="mif-exit"></span>
                        <span>Đăng xuất</span>
                    </asp:LinkButton>
                </div>
            </div>
        </details>
    </div>
</div>
