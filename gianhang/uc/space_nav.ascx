<%@ Control Language="C#" AutoEventWireup="true" CodeFile="space_nav.ascx.cs" Inherits="gianhang_uc_space_nav" %>

<asp:PlaceHolder ID="ph_nav" runat="server" Visible="false">
    <div class="gh-space-nav-shell">
        <div class="gh-space-nav-card">
            <div class="gh-space-nav-brand">
                <span class="gh-space-nav-logo">
                    <img src="/uploads/images/favicon.png" alt="AhaSale" />
                </span>
                <div>
                    <div class="gh-space-nav-title">
                        <asp:Literal ID="lit_store_name" runat="server" />
                    </div>
                    <div class="gh-space-nav-sub">
                        Tài khoản gốc: <asp:Literal ID="lit_account_key" runat="server" />
                        • Trạng thái: <asp:Literal ID="lit_status" runat="server" />
                    </div>
                </div>
            </div>

            <div class="gh-space-nav-actions">
                <span class="gh-space-nav-pill">Không gian quản trị gian hàng</span>
                <asp:Image ID="img_avatar" runat="server" CssClass="gh-space-nav-avatar" />
                <asp:HyperLink ID="lnk_public" runat="server" CssClass="gh-space-nav-link" Target="_blank">Trang công khai</asp:HyperLink>
                <asp:HyperLink ID="lnk_home" runat="server" CssClass="gh-space-nav-link">Trung tâm tài khoản</asp:HyperLink>
                <asp:HyperLink ID="lnk_manage" runat="server" CssClass="gh-space-nav-link">Quản lý tin</asp:HyperLink>
                <asp:HyperLink ID="lnk_orders" runat="server" CssClass="gh-space-nav-link">Đơn bán</asp:HyperLink>
                <asp:HyperLink ID="lnk_booking" runat="server" CssClass="gh-space-nav-link">Lịch hẹn</asp:HyperLink>
                <asp:HyperLink ID="lnk_customers" runat="server" CssClass="gh-space-nav-link">Khách hàng</asp:HyperLink>
                <asp:HyperLink ID="lnk_report" runat="server" CssClass="gh-space-nav-link">Báo cáo</asp:HyperLink>
                <asp:HyperLink ID="lnk_level2" runat="server" CssClass="gh-space-nav-link">Level 2</asp:HyperLink>
                <asp:PlaceHolder ID="ph_admin" runat="server" Visible="false">
                    <asp:HyperLink ID="lnk_admin" runat="server" CssClass="gh-space-nav-link is-solid">Mở /gianhang/admin</asp:HyperLink>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>
</asp:PlaceHolder>
