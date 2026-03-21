<%@ Page Title="Ví token điểm" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_vi_token_diem_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="admin-role-home-shell pt-3 pl-3 pr-3 pb-20">
        <section class="admin-role-home-hero">
            <div class="admin-role-home-hero-main">
                <div class="admin-role-home-eyebrow">TÀI SẢN LÕI / SUPER ADMIN</div>
                <h1 class="admin-role-home-title">Ví token điểm</h1>
                <div class="admin-role-home-pill-row">
                    <span class="admin-role-home-pill admin-role-home-pill-primary">Chỉ Super Admin</span>
                    <span class="admin-role-home-pill admin-role-home-pill-scope">Bridge / blockchain / đối chiếu điểm A</span>
                </div>
                <p class="admin-role-home-description">
                    Trang này dùng để theo dõi số dư token blockchain, đối chiếu điểm A nội bộ và mở cấu hình ví/token bridge.
                </p>
            </div>
            <aside class="admin-role-home-sidecard">
                <div class="admin-role-home-side-title">Phạm vi thao tác</div>
                <div class="admin-role-home-side-line">
                    <span class="admin-role-home-side-label">Loại quyền</span>
                    <span class="admin-role-home-side-value">Tài sản lõi</span>
                </div>
                <div class="admin-role-home-side-line">
                    <span class="admin-role-home-side-label">Phạm vi</span>
                    <span class="admin-role-home-side-value">Toàn hệ thống</span>
                </div>
                <div class="admin-role-home-side-alert">
                    Chỉ Super Admin mới được theo dõi và chỉnh cấu hình ví token điểm.
                </div>
            </aside>
        </section>

        <section class="admin-role-home-section admin-role-home-root-section">
            <div class="admin-role-home-section-head">
                <h2>Ví token điểm</h2>
                <p>Khối này dùng để theo dõi bridge và cập nhật cấu hình ví/token trên hệ thống.</p>
            </div>

            <asp:Panel ID="pn_bridge_summary" runat="server" CssClass="bridge-summary-card">
                <div class="bridge-kv mt-2">Điểm A hiển thị theo blockchain:</div>
                <div class="bridge-points"><asp:Label ID="lb_bridge_points_now" runat="server" /></div>
                <div class="bridge-kv">Điểm A nội bộ DB: <asp:Label ID="lb_bridge_points_db" runat="server" /></div>
                <div class="mt-2">
                    <asp:LinkButton ID="but_refresh_bridge" runat="server" CssClass="button small light" OnClick="but_refresh_bridge_Click">Làm mới dữ liệu bridge</asp:LinkButton>
                </div>
                <asp:Panel ID="pn_bridge_hidden_meta" runat="server" Visible="false">
                    <asp:Label ID="lb_bridge_treasury_account" runat="server" />
                    <asp:Label ID="lb_bridge_treasury_name" runat="server" />
                    <asp:Label ID="lb_bridge_deposit_address" runat="server" />
                    <asp:Label ID="lb_bridge_token_contract" runat="server" />
                    <asp:Label ID="lb_bridge_enabled" runat="server" />
                    <asp:Label ID="lb_bridge_point_rate" runat="server" />
                    <asp:Label ID="lb_bridge_min_confirmations" runat="server" />
                    <asp:Label ID="lb_bridge_token_balance_now" runat="server" />
                    <asp:Label ID="lb_bridge_safe_block" runat="server" />
                </asp:Panel>

                <div class="mt-3">
                    <div class="text-bold mb-1">Giao dịch bridge gần nhất</div>
                    <div class="bcorn-fix-title-table-container aha-admin-grid">
                        <table class="bcorn-fix-title-table">
                            <thead>
                                <tr>
                                    <th style="width: 1px;">ID</th>
                                    <th class="text-center" style="min-width: 120px;">Thời gian</th>
                                    <th class="text-center" style="min-width: 80px;">Chiều</th>
                                    <th class="text-center" style="min-width: 120px;">Số token</th>
                                    <th class="text-center" style="min-width: 120px;">Điểm</th>
                                    <th class="text-center" style="min-width: 110px;">Trạng thái</th>
                                    <th class="text-center" style="min-width: 170px;">TX Hash</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="RepeaterBridge" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="text-center"><%# Eval("id") %></td>
                                            <td class="text-center"><%# Eval("time_text") %></td>
                                            <td class="text-center">
                                                <asp:PlaceHolder ID="ph_in" runat="server" Visible='<%# Eval("direction").ToString()=="IN" %>'>
                                                    <span class="mini rounded button success">IN</span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="ph_out" runat="server" Visible='<%# Eval("direction").ToString()=="OUT" %>'>
                                                    <span class="mini rounded button warning">OUT</span>
                                                </asp:PlaceHolder>
                                            </td>
                                            <td class="text-right"><%# Eval("token_amount", "{0:#,##0.######}") %></td>
                                            <td class="text-right"><%# Eval("points_credited", "{0:#,##0.##}") %></td>
                                            <td class="text-center"><%# Eval("status") %></td>
                                            <td class="text-center" title='<%# Eval("tx_hash") %>'><%# Eval("tx_hash_short") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div class="bridge-note"><asp:Label ID="lb_bridge_status_note" runat="server" /></div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pn_bridge_config" runat="server" CssClass="bridge-summary-card mt-3">
                <div class="text-bold mb-2">Cập nhật ví/token (Super Admin)</div>

                <asp:Panel ID="pn_bridge_config_gate" runat="server">
                    <div class="bridge-kv">Nhập mật khẩu để mở cấu hình</div>
                    <div class="row">
                        <div class="cell-lg-6">
                            <asp:TextBox ID="txt_bridge_config_password" runat="server" TextMode="Password" data-role="input" Style="width: 100%;" placeholder="Mật khẩu admin"></asp:TextBox>
                        </div>
                        <div class="cell-lg-6 mt-2">
                            <asp:LinkButton ID="but_unlock_bridge_config" runat="server" CssClass="button small warning" OnClick="but_unlock_bridge_config_Click">Mở cấu hình</asp:LinkButton>
                            <span class="text-secondary ml-2">Chỉ mở trong phiên đăng nhập hiện tại.</span>
                        </div>
                    </div>
                    <div class="mt-1">
                        <asp:Label ID="lb_bridge_gate_notice" runat="server"></asp:Label>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pn_bridge_config_fields" runat="server">
                    <div class="row">
                        <div class="cell-lg-6">
                            <div class="bridge-kv">Ví BSC theo dõi</div>
                            <asp:TextBox ID="txt_bridge_deposit_address" runat="server" data-role="input" Style="width: 100%;"></asp:TextBox>
                        </div>
                        <div class="cell-lg-6">
                            <div class="bridge-kv">Token contract (BEP20)</div>
                            <asp:TextBox ID="txt_bridge_token_contract" runat="server" data-role="input" Style="width: 100%;"></asp:TextBox>
                        </div>
                        <div class="cell-lg-6 mt-2">
                            <div class="bridge-kv">Tài khoản ví tổng (cộng điểm)</div>
                            <asp:DropDownList ID="ddl_bridge_treasury_account" runat="server" data-role="select" Style="width: 100%;"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="mt-2">
                        <asp:LinkButton ID="but_save_bridge_config" runat="server" CssClass="button small success" OnClick="but_save_bridge_config_Click">Lưu cấu hình</asp:LinkButton>
                        <span class="text-secondary ml-2">Lưu xong cần khởi động lại watcher để áp dụng ví/token mới.</span>
                        <div class="mt-1">
                            <asp:Label ID="lb_bridge_config_notice" runat="server"></asp:Label>
                        </div>
                    </div>
                </asp:Panel>
            </asp:Panel>
        </section>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>
