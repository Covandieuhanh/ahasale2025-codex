<%@ Page Title="Trang chủ" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <%--title & meta--%>
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="pt-3 pl-3 pr-3 pb-20">
        <div class="bridge-page-title">Theo dõi Token Bridge (tách riêng)</div>

        <asp:Panel ID="pn_bridge_summary" runat="server" CssClass="bridge-summary-card">
            <div class="row">
                <div class="cell-lg-7">
                    <div class="bridge-kv">Tài khoản tổng: <b><asp:Label ID="lb_bridge_treasury_account" runat="server" /></b></div>
                    <div class="bridge-kv">Tên tài khoản: <asp:Label ID="lb_bridge_treasury_name" runat="server" /></div>
                    <div class="bridge-kv">Ví BSC theo dõi: <asp:Label ID="lb_bridge_deposit_address" runat="server" /></div>
                    <div class="bridge-kv">Token contract: <asp:Label ID="lb_bridge_token_contract" runat="server" /></div>
                </div>
                <div class="cell-lg-5 text-right-lg text-left">
                    <div class="bridge-kv">Bridge: <span class="bridge-inline-badge"><asp:Label ID="lb_bridge_enabled" runat="server" /></span></div>
                    <div class="bridge-kv">Tỷ lệ quy đổi: <b><asp:Label ID="lb_bridge_point_rate" runat="server" /></b> điểm / 1 token</div>
                    <div class="bridge-kv">Xác nhận tối thiểu: <b><asp:Label ID="lb_bridge_min_confirmations" runat="server" /></b> block</div>
                    <div class="bridge-kv mt-2">Số dư token blockchain (đã xác nhận): <b><asp:Label ID="lb_bridge_token_balance_now" runat="server" /></b></div>
                    <div class="bridge-kv">Safe block watcher: <b><asp:Label ID="lb_bridge_safe_block" runat="server" /></b></div>
                    <div class="bridge-kv mt-2">Điểm A hiển thị theo blockchain:</div>
                    <div class="bridge-points"><asp:Label ID="lb_bridge_points_now" runat="server" /></div>
                    <div class="bridge-kv">Điểm A nội bộ DB: <asp:Label ID="lb_bridge_points_db" runat="server" /></div>
                    <div class="mt-2">
                        <asp:LinkButton ID="but_refresh_bridge" runat="server" CssClass="button small light" OnClick="but_refresh_bridge_Click">Làm mới dữ liệu bridge</asp:LinkButton>
                    </div>
                </div>
            </div>

            <div class="mt-3">
                <div class="text-bold mb-1">Giao dịch bridge gần nhất</div>
                <div class="bcorn-fix-title-table-container">
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
            <div class="text-bold mb-2">Cập nhật ví/token (root admin)</div>
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
                    <asp:TextBox ID="txt_bridge_treasury_account" runat="server" data-role="input" Style="width: 100%;"></asp:TextBox>
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

        <div class="mt-3">
            <a href="/admin/lich-su-chuyen-diem/default.aspx" class="button small light">
                Mở trang Lịch sử chuyển điểm
            </a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>

