<%@ Page Title="Trang chủ" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="admin-role-home-shell pt-3 pl-3 pr-3 pb-20">
        <section class="admin-role-home-hero">
            <div class="admin-role-home-hero-main">
                <div class="admin-role-home-eyebrow">TRANG CHỦ TÀI KHOẢN ADMIN</div>
                <h1 class="admin-role-home-title">Bảng quyền truy cập của <asp:Label ID="lb_admin_home_name" runat="server" /></h1>
                <div class="admin-role-home-pill-row">
                    <span class="admin-role-home-pill admin-role-home-pill-primary">
                        Vai trò: <asp:Label ID="lb_admin_home_role" runat="server" />
                    </span>
                    <span class="admin-role-home-pill admin-role-home-pill-scope">
                        Phạm vi: <asp:Label ID="lb_admin_home_scope" runat="server" />
                    </span>
                    <span class="admin-role-home-pill admin-role-home-pill-count">
                        Số tab được cấp: <asp:Label ID="lb_admin_home_tab_count" runat="server" />
                    </span>
                </div>
                <p class="admin-role-home-description">
                    <asp:Label ID="lb_admin_home_description" runat="server" />
                </p>
                <div class="admin-role-home-cta-row">
                    <asp:HyperLink ID="hl_primary_workspace" runat="server" CssClass="button primary admin-role-home-cta">Mở khu làm việc chính</asp:HyperLink>
                    <span class="admin-role-home-primary-hint"><asp:Label ID="lb_admin_home_primary_hint" runat="server" /></span>
                </div>
            </div>
            <aside class="admin-role-home-sidecard">
                <div class="admin-role-home-side-title">Thông tin tài khoản</div>
                <div class="admin-role-home-side-line">
                    <span class="admin-role-home-side-label">Tài khoản</span>
                    <span class="admin-role-home-side-value"><asp:Label ID="lb_admin_home_account" runat="server" /></span>
                </div>
                <div class="admin-role-home-side-line">
                    <span class="admin-role-home-side-label">Quyền đang gán</span>
                    <span class="admin-role-home-side-value"><asp:Label ID="lb_admin_home_permission_summary" runat="server" /></span>
                </div>
                <div class="admin-role-home-side-line">
                    <span class="admin-role-home-side-label">Nhóm đang phụ trách</span>
                    <span class="admin-role-home-side-value"><asp:Label ID="lb_admin_home_object_scope" runat="server" /></span>
                </div>
                <div class="admin-role-home-side-line">
                    <span class="admin-role-home-side-label">URL khu làm việc chính</span>
                    <span class="admin-role-home-side-value"><asp:HyperLink ID="hl_primary_workspace_inline" runat="server" Target="_self"></asp:HyperLink></span>
                </div>
                <asp:Panel ID="pn_core_asset_notice" runat="server" CssClass="admin-role-home-side-alert">
                    Tài khoản này chỉ được xem tiền, quyền và điểm. Mọi can thiệp tài sản lõi phải qua Super Admin.
                </asp:Panel>
            </aside>
        </section>

        <section class="admin-role-home-section">
            <div class="admin-role-home-section-head">
                <h2>Quy định đang áp dụng cho tài khoản này</h2>
                <p>Các ghi chú dưới đây được sinh theo đúng vai trò và scope đang cấp.</p>
            </div>
            <div class="admin-role-home-note-list">
                <asp:Repeater ID="rpt_admin_home_notes" runat="server">
                    <ItemTemplate>
                        <div class="admin-role-home-note-item">
                            <span class="admin-role-home-note-dot"></span>
                            <span><%# Eval("Text") %></span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </section>

        <section class="admin-role-home-section">
            <div class="admin-role-home-section-head">
                <h2>5 nhóm đối tượng quản trị</h2>
                <p>Khối này giúp người dùng nhìn một lần là hiểu ngay mình đang phụ trách nhóm nào trong 5 nhóm admin chính.</p>
            </div>
            <div class="admin-role-home-grid">
                <asp:Repeater ID="rpt_admin_object_groups" runat="server">
                    <ItemTemplate>
                        <article class='admin-role-home-tab-card <%# (bool)Eval("IsActive") ? "admin-role-home-object-card-active" : "admin-role-home-object-card-inactive" %>'>
                            <div class="admin-role-home-tab-head">
                                <h3><%# Eval("Title") %></h3>
                                <span class="admin-role-home-tab-scope"><%# Eval("ScopeLabel") %></span>
                            </div>
                            <div class="admin-role-home-tab-body">
                                <div class="admin-role-home-tab-row">
                                    <span class="admin-role-home-tab-label">Ý nghĩa</span>
                                    <span class="admin-role-home-tab-text"><%# Eval("Meaning") %></span>
                                </div>
                                <div class="admin-role-home-tab-row">
                                    <span class="admin-role-home-tab-label">Trạng thái quyền</span>
                                    <span class="admin-role-home-tab-text"><%# Eval("StatusLabel") %></span>
                                </div>
                                <div class="admin-role-home-tab-row admin-role-home-tab-row-guardrail">
                                    <span class="admin-role-home-tab-label">Ghi chú</span>
                                    <span class="admin-role-home-tab-text"><%# Eval("StatusHint") %></span>
                                </div>
                            </div>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </section>

        <section class="admin-role-home-section">
            <div class="admin-role-home-section-head">
                <h2>Danh sách tab được phép truy cập</h2>
                <p>Mỗi tab đều ghi rõ tên tab, ý nghĩa, thao tác chính và phạm vi hoạt động để người dùng nắm quyền ngay khi đăng nhập.</p>
            </div>
            <asp:Panel ID="pn_admin_home_empty" runat="server" CssClass="admin-role-home-empty" Visible="false">
                Tài khoản này hiện chưa có tab nghiệp vụ nào được cấp. Nếu đây là tài khoản mới, Super Admin cần gán quyền trước khi sử dụng.
            </asp:Panel>
            <div class="admin-role-home-grid">
                <asp:Repeater ID="rpt_admin_home_tabs" runat="server">
                    <ItemTemplate>
                        <article class="admin-role-home-tab-card">
                            <div class="admin-role-home-tab-head">
                                <h3><%# Eval("Title") %></h3>
                                <span class="admin-role-home-tab-scope"><%# Eval("ScopeLabel") %></span>
                            </div>
                            <div class="admin-role-home-tab-body">
                                <div class="admin-role-home-tab-row">
                                    <span class="admin-role-home-tab-label">Ý nghĩa</span>
                                    <span class="admin-role-home-tab-text"><%# Eval("Meaning") %></span>
                                </div>
                                <div class="admin-role-home-tab-row">
                                    <span class="admin-role-home-tab-label">Thao tác chính</span>
                                    <span class="admin-role-home-tab-text"><%# Eval("ActionSummary") %></span>
                                </div>
                                <div class="admin-role-home-tab-row">
                                    <span class="admin-role-home-tab-label">Phạm vi hoạt động</span>
                                    <span class="admin-role-home-tab-text"><%# Eval("ScopeMeaning") %></span>
                                </div>
                                <div class="admin-role-home-tab-row admin-role-home-tab-row-guardrail">
                                    <span class="admin-role-home-tab-label">Giới hạn bắt buộc</span>
                                    <span class="admin-role-home-tab-text"><%# Eval("GuardrailSummary") %></span>
                                </div>
                            </div>
                            <div class="admin-role-home-tab-foot">
                                <asp:HyperLink ID="hl_open_tab" runat="server" NavigateUrl='<%# Eval("Url") %>' CssClass="button light admin-role-home-tab-cta"><%# Eval("CtaLabel") %></asp:HyperLink>
                            </div>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </section>

    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>
