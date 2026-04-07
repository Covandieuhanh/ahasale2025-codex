<%@ Control Language="C#" AutoEventWireup="true" CodeFile="menu-top-uc.ascx.cs" Inherits="admin_uc_menu_top_uc" %>
<%@ Register Src="~/admin/uc/admin-space-launcher-uc.ascx" TagPrefix="uc1" TagName="AdminSpaceLauncher" %>

<div data-role="charms" data-position="right" id="thongbao-charms" style="width: 320px; background-color: #fff; overflow: auto;" class="p-0 m-0 shadow-1 charms right-side">
    <div style="height: 52px; line-height: 55px" class="fg-white">
        <div style="float: left"><span class="ml-4 fg-white">THÔNG BÁO ADMIN</span></div>
        <div style="float: right"><a href="#" class="fg-white" title="Đóng" onclick="window.ahaAdminCharms('close'); return false;"><span class="mif mif-cross mr-4"></span></a></div>
        <div style="clear: both"></div>
    </div>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="text-left p-3">
                <asp:Button ID="but_sapxep_moinhat" runat="server" Text="Mới nhất" CssClass="light small rounded" OnClick="but_sapxep_moinhat_Click" CausesValidation="false" />
                <asp:Button ID="but_sapxep_chuadoc" runat="server" Text="Chưa đọc" CssClass="light small rounded" OnClick="but_sapxep_chuadoc_Click" CausesValidation="false" />
                <a href="<%=GetNotificationListUrl() %>" class="button warning small rounded">Quản lý</a>
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
                                <span class="button bg-transparent"><span class="text-bold" style="font-size: 18px">...</span></span>
                            </div>
                            <ul class="d-menu place-right" data-role="dropdown">
                                <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("daxem").ToString()=="False" %>'>
                                    <li>
                                        <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="but_dadoc" OnClick="but_dadoc_Click" runat="server" Text="Đánh dấu đã đọc" CausesValidation="false"></asp:LinkButton>
                                    </li>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("daxem").ToString()=="True" %>'>
                                    <li>
                                        <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="but_chuadoc" OnClick="but_chuadoc_Click" runat="server" Text="Đánh dấu chưa đọc" CausesValidation="false"></asp:LinkButton>
                                    </li>
                                </asp:PlaceHolder>
                                <li>
                                    <asp:LinkButton CommandArgument='<%# Eval("id") %>' ID="but_xoathongbao" OnClick="but_xoathongbao_Click" runat="server" Text="Xóa thông báo này" CausesValidation="false"></asp:LinkButton>
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
        <a href="<%=GetNotificationListUrl() %>" class="button warning small rounded">Xem tất cả</a>
    </div>
</div>

<div data-role="appbar" class="fg-white bg-nmenutop-bc admin-topbar" data-expand-point="lg" style="position: fixed; top: 0; z-index: 3">
    <uc1:AdminSpaceLauncher runat="server" ID="adminSpaceLauncher" ButtonCssClass="app-bar-item fg-white" />
    <a href="#" class="app-bar-item d-block d-none-lg" id="paneToggle"><span class="mif-menu"></span></a>

    <a class="app-bar-item fg-white admin-topbar-home" href="<%=GetAdminHomeUrl() %>"><span class="mif mif-home"></span></a>
    <a class="app-bar-item fg-white d-block-lg d-none fw-600 admin-topbar-title" style="z-index: 10!important" href="<%=GetAdminHomeUrl() %>"><%=ViewState["title"] %></a>
    <a class="app-bar-item d-block-lg d-none" href="#" id="admin-ui-density-toggle" title="Thu gọn menu">
        <span class="mif mif-list"></span>
    </a>
    <div class="app-bar-container ml-auto">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:LinkButton ID="but_show_form_thongbao" OnClick="but_show_form_thongbao_Click" OnClientClick="window.ahaAdminCharms('toggle');" CssClass="app-bar-item" runat="server" CausesValidation="false">
                    <span class="mif-notifications mif-lg"></span>
                    <span class="badge bg-orange fg-white mt-2 mr-1">
                        <asp:Label ID="lb_sl_thongbao" runat="server" Text="0"></asp:Label>
                    </span>
                </asp:LinkButton>
            </ContentTemplate>
        </asp:UpdatePanel>

        <% if (ShowQuickCreateMenu()) { %>
        <div class="app-bar-container admin-quick-dd" id="admin-quick-shell">
            <button type="button" class="app-bar-item admin-quick-btn" id="admin-quick-toggle" aria-haspopup="true" aria-expanded="false" title="Tạo nhanh">
                <span class="mif mif-plus"></span>
            </button>
            <div class="admin-quick-menu" id="admin-quick-dropdown">
                <ul class="admin-quick-list">
                    <% if (ShowQuickCreateAccounts()) { %>
                        <% if (ShowMenuAdminAccount()) { %>
                        <li><a href="<%= GetAdminAccountCreateUrl() %>">Tạo tài khoản admin</a></li>
                        <% } %>
                        <% if (ShowMenuHomeAccount()) { %>
                        <li><a href="<%= GetHomeAccountCreateUrl() %>">Tạo tài khoản Home</a></li>
                        <% } %>
                        <% if (ShowMenuShopAccount()) { %>
                        <li><a href="<%= GetShopAccountCreateUrl() %>">Tạo tài khoản gian hàng</a></li>
                        <% } %>
                    <% } %>

                    <% if (ShowQuickCreateAccounts() && (ShowQuickCreateOperations() || ShowQuickCreateContent())) { %>
                    <li class="admin-quick-divider"></li>
                    <% } %>

                    <% if (ShowQuickCreateOperations()) { %>
                        <% if (ShowMenuIssueCard()) { %>
                        <li><a href="<%= GetHomeIssueCardUrl() %>">Phát hành thẻ</a></li>
                        <% } %>
                        <% if (ShowMenuSellProduct()) { %>
                        <li><a href="<%= GetHomeSellProductUrl() %>">Bán sản phẩm</a></li>
                        <% } %>
                    <% } %>

                    <% if (ShowQuickCreateOperations() && ShowQuickCreateContent()) { %>
                    <li class="admin-quick-divider"></li>
                    <% } %>

                    <% if (ShowQuickCreateContent()) { %>
                        <% if (ShowMenuContentMenu()) { %>
                        <li><a href="<%= GetContentMenuCreateUrl() %>">Tạo menu</a></li>
                        <% } %>
                        <% if (ShowMenuContentBaiViet()) { %>
                        <li><a href="<%= GetContentBaiVietCreateUrl() %>">Tạo bài viết</a></li>
                        <% } %>
                        <% if (ShowMenuContentBanner()) { %>
                        <li><a href="<%= GetContentBannerCreateUrl() %>">Tạo banner</a></li>
                        <% } %>
                    <% } %>
                </ul>
            </div>
        </div>
        <% } %>

        <div class="app-bar-container admin-avatar-wrap admin-shop-dd" id="admin-avatar-shell" style="visibility:hidden; opacity:0;">
            <button type="button" class="app-bar-item admin-shop-avatar-btn" id="admin-avatar-toggle" aria-haspopup="true" aria-expanded="false">
                <img src="<%=ViewState["anhdaidien"] %>" class="admin-shop-avatar-image" alt="avatar" />
                <span class="app-bar-name admin-shop-avatar-name"><%=ViewState["taikhoan"] %></span>
                <span class="admin-shop-avatar-caret">&#9660;</span>
            </button>
            <div class="admin-shop-menu" id="admin-avatar-dropdown">
                <div class="admin-shop-menu-body">
                    <% if (ShowMenuGroupAdmin()) { %>
                    <div class="admin-shop-menu-group-title">Quản trị admin</div>
                    <% if (ShowMenuAdminDashboard()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("admin_dashboard") %>' href="<%= GetAdminDashboardUrl() %>">
                        <span>Trang chủ admin</span>
                        <span class="admin-shop-menu-badge">Dashboard</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuAdminAccount()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveTaiKhoanScope("admin") %>' href="<%= GetAdminAccountManagementUrl() %>">
                        <span>Quản lý tài khoản admin</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuAdminOtp()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("admin_otp") %>' href="<%= GetAdminOtpUrl() %>">
                        <span>Quản lý OTP</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuAdminTokenWallet()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveTokenWallet() %>' href="<%= GetAdminTokenWalletUrl() %>">
                        <span>Ví token điểm</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuAdminGopY()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("admin_feedback") %>' href="<%= GetAdminGopYUrl() %>">
                        <span>Quản lý góp ý</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuAdminThongBao()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("notifications") %>' href="<%= GetAdminThongBaoUrl() %>">
                        <span>Quản lý thông báo</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuAdminTuVan()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("consulting") %>' href="<%= GetAdminTuVanUrl() %>">
                        <span>Yêu cầu tư vấn</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuAdminCompanyShopSync()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("admin_company_shop_sync") %>' href="<%= GetAdminCompanyShopSyncUrl() %>">
                        <span>Đồng bộ shop công ty</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuAdminReindexBaiViet()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("admin_reindex_baiviet") %>' href="<%= GetAdminReindexBaiVietUrl() %>">
                        <span>Reindex bài viết</span>
                    </a>
                    <% } %>
                    <% } %>

                    <% if (ShowMenuGroupHomeSpace()) { %>
                    <div class="admin-shop-menu-group-title">Quản trị không gian Home</div>
                    <% if (ShowMenuTransferHistory()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveTransferHistory() %>' href="<%= GetHomeTransferHistoryUrl() %>">
                        <span>Lịch sử chuyển điểm</span>
                        <span class="admin-shop-menu-badge"><%=ViewState["DongA"] %></span>
                    </a>
                    <% } %>
                    <% if (ShowMenuHomeAccount()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveTaiKhoanScope("home") %>' href="<%= GetHomeAccountManagementUrl() %>">
                        <span>Quản lý tài khoản home</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuApproveHanhVi()) { %>
                    <a class='admin-shop-menu-item <%= MenuActivePointApproval() %>' href="<%= GetApproveHomePointUrl() %>">
                        <span>Duyệt yêu cầu điểm / hành vi</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuIssueCard()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("issue_cards") %>' href="<%= GetHomeIssueCardUrl() %>">
                        <span>Phát hành thẻ</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuTierDescription()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("tier_reference") %>' href="<%= GetHomeTierDescriptionUrl() %>">
                        <span>Mô tả cấp bậc</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuSellProduct()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("system_products") %>' href="<%= GetHomeSellProductUrl() %>">
                        <span>Bán sản phẩm</span>
                    </a>
                    <% } %>
                    <% } %>

                    <% if (ShowMenuGroupShop()) { %>
                    <div class="admin-shop-menu-group-title">Quản trị không gian Gian hàng đối tác</div>
                    <% if (ShowMenuShopWorkspace()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("shop_workspace") %>' href="<%= GetShopWorkspaceUrl() %>">
                        <span>Trung tâm quản trị gian hàng</span>
                        <span class="admin-shop-menu-badge">Portal</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopAccount()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveTaiKhoanScope("shop") %>' href="<%= GetShopAccountManagementUrl() %>">
                        <span>Quản lý tài khoản gian hàng đối tác</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopPointApproval()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveShopPointApproval() %>' href="<%= GetShopPointApprovalUrl() %>">
                        <span>Duyệt điểm / nghiệp vụ shop</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopEmailTemplate()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("shop_email") %>' href="<%= GetShopEmailTemplateUrl() %>">
                        <span>Nội dung email gian hàng đối tác</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopLegacyInvoices()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("shop_legacy_invoices") %>' href="<%= GetShopLegacyInvoicesUrl() %>">
                        <span>Vận hành hóa đơn gian hàng</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopLegacyCustomers()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("shop_legacy_customers") %>' href="<%= GetShopLegacyCustomersUrl() %>">
                        <span>Vận hành khách hàng gian hàng</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopLegacyContent()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("shop_legacy_content") %>' href="<%= GetShopLegacyContentUrl() %>">
                        <span>Vận hành nội dung gian hàng</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopLegacyFinance()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("shop_legacy_finance") %>' href="<%= GetShopLegacyFinanceUrl() %>">
                        <span>Vận hành thu chi gian hàng</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopLegacyInventory()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("shop_legacy_inventory") %>' href="<%= GetShopLegacyInventoryUrl() %>">
                        <span>Vận hành kho vật tư gian hàng</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopLegacyAccounts()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("shop_legacy_accounts") %>' href="<%= GetShopLegacyAccountsUrl() %>">
                        <span>Vận hành tài khoản gian hàng</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopLegacyOrg()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("shop_legacy_org") %>' href="<%= GetShopLegacyOrgUrl() %>">
                        <span>Vận hành cơ cấu gian hàng</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopLegacyTraining()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("shop_legacy_training") %>' href="<%= GetShopLegacyTrainingUrl() %>">
                        <span>Vận hành học viên giảng viên</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopLegacySupport()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("shop_legacy_support") %>' href="<%= GetShopLegacySupportUrl() %>">
                        <span>Vận hành hỗ trợ gian hàng</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuGianHangApproval()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("home_gianhang_space") %>' href="<%= GetGianHangApprovalUrl() %>">
                        <span>Duyệt không gian gian hàng</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopPartnerApproval()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("shop_partner") %>' href="<%= GetShopPartnerApprovalUrl() %>">
                        <span>Duyệt gian hàng đối tác (Shop)</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuShopLevel2()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("shop_level2") %>' href="<%= GetShopLevel2ApprovalUrl() %>">
                        <span>Duyệt nâng cấp Level 2</span>
                    </a>
                    <% } %>
                    <% } %>

                    <% if (ShowMenuGroupDauGia()) { %>
                    <div class="admin-shop-menu-group-title">Quản trị không gian Đấu giá</div>
                    <a class='admin-shop-menu-item' href="<%= GetDauGiaAdminUrl() %>">
                        <span>Trung tâm quản trị đấu giá</span>
                        <span class="admin-shop-menu-badge">Portal</span>
                    </a>
                    <% } %>

                    <% if (ShowMenuGroupEvent()) { %>
                    <div class="admin-shop-menu-group-title">Quản trị không gian Sự kiện</div>
                    <a class='admin-shop-menu-item' href="<%= GetEventAdminUrl() %>">
                        <span>Trung tâm quản trị sự kiện</span>
                        <span class="admin-shop-menu-badge">Portal</span>
                    </a>
                    <% } %>

                    <% if (ShowMenuGroupBatDongSan()) { %>
                    <div class="admin-shop-menu-group-title">Quản trị không gian Bất động sản</div>
                    <% if (ShowMenuBatDongSanWorkspace()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveBatDongSanWorkspace() %>' href="<%= GetBatDongSanWorkspaceUrl() %>">
                        <span>Trung tâm quản trị bất động sản</span>
                        <span class="admin-shop-menu-badge">Portal</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuBatDongSanLinked()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("home_bds_linked") %>' href="<%= GetBatDongSanLinkedUrl() %>">
                        <span>BĐS - Liên kết tin</span>
                    </a>
                    <% } %>
                    <% } %>

                    <% if (ShowMenuGroupContent()) { %>
                    <div class="admin-shop-menu-group-title">Quản trị nội dung Website</div>
                    <% if (ShowMenuHomeContent() || ShowMenuHomeTextContent()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("home_config") %>' href="<%= GetContentSettingsUrl() %>">
                        <span>Cài đặt trang chủ</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuHomeTextContent()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("home_content") %>' href="<%= GetContentHomeTextUrl() %>">
                        <span>Nội dung trang chủ Home</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuContentMenu()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("home_menu") %>' href="<%= GetContentMenuUrl() %>">
                        <span>Quản lý menu</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuContentBaiViet()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("home_posts") %>' href="<%= GetContentBaiVietUrl() %>">
                        <span>Quản lý bài viết</span>
                    </a>
                    <% } %>
                    <% if (ShowMenuContentBanner()) { %>
                    <a class='admin-shop-menu-item <%= MenuActiveFeature("home_banner") %>' href="<%= GetContentBannerUrl() %>">
                        <span>Quản lý banner</span>
                    </a>
                    <% } %>
                    <% } %>
                </div>
                <div class="admin-shop-menu-footer">
                    <asp:HyperLink ID="but_show_form_doimatkhau" runat="server" CssClass="admin-shop-btn admin-shop-btn-light">Đổi mật khẩu</asp:HyperLink>
                    <asp:LinkButton ID="but_dangxuat" runat="server" CssClass="admin-shop-btn admin-shop-btn-danger" OnClick="but_dangxuat_Click" CausesValidation="false">Đăng xuất</asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</div>
