<%@ Page Title="Duyệt nâng cấp Level 2" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master"
    AutoEventWireup="true" CodeFile="duyet-nang-cap-level2.aspx.cs"
    Inherits="admin_duyet_nang_cap_level2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="p-3 aha-admin-section">
                <h4 class="mb-3">DUYỆT NÂNG CẤP LEVEL 2</h4>

                <div class="row">
                    <div class="cell-md-4 mb-2">
                        <asp:TextBox ID="txt_timkiem" runat="server" CssClass="input-custom" placeholder="Tìm ID hoặc tài khoản"></asp:TextBox>
                    </div>
                    <div class="cell-md-3 mb-2">
                        <asp:DropDownList ID="ddl_trangthai" runat="server" CssClass="input-custom" AutoPostBack="true" OnSelectedIndexChanged="ddl_trangthai_SelectedIndexChanged">
                            <asp:ListItem Value="" Text="Tất cả trạng thái"></asp:ListItem>
                            <asp:ListItem Value="0" Text="Chờ duyệt"></asp:ListItem>
                            <asp:ListItem Value="1" Text="Đã duyệt"></asp:ListItem>
                            <asp:ListItem Value="2" Text="Từ chối"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="cell-md-5 mb-2">
                        <asp:Button ID="btn_timkiem" runat="server" CssClass="button primary mr-2" Text="Tìm kiếm" OnClick="btn_timkiem_Click" />
                        <asp:Button ID="btn_reset" runat="server" CssClass="button" Text="Reset" OnClick="btn_reset_Click" />
                    </div>
                </div>

                <div class="bcorn-fix-title-table-container aha-admin-grid">
                    <table class="bcorn-fix-title-table">
                        <thead>
                            <tr>
                                <th style="min-width: 70px;">ID</th>
                                <th style="min-width: 160px;">Tài khoản</th>
                                <th style="min-width: 170px;">Ngày tạo</th>
                                <th style="min-width: 140px;">Trạng thái</th>
                                <th style="min-width: 220px;">Ghi chú</th>
                                <th style="min-width: 170px;">Duyệt bởi</th>
                                <th style="min-width: 220px;" class="text-center">Thao tác</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rp_level2" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("ID") %></td>
                                        <td class="fw-600"><%# Eval("TaiKhoan") %></td>
                                        <td><%# Eval("NgayTao","{0:dd/MM/yyyy HH:mm}") %></td>
                                        <td>
                                            <asp:PlaceHolder runat="server" Visible='<%# Eval("TrangThai").ToString()=="0" %>'>
                                                <span class="status-pill">Chờ duyệt</span>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder runat="server" Visible='<%# Eval("TrangThai").ToString()=="1" %>'>
                                                <span class="status-pill">Đã duyệt</span>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder runat="server" Visible='<%# Eval("TrangThai").ToString()=="2" %>'>
                                                <span class="status-pill">Từ chối</span>
                                            </asp:PlaceHolder>
                                        </td>
                                        <td><%# Eval("GhiChuAdmin") %></td>
                                        <td>
                                            <%# Eval("AdminDuyet") %>
                                            <div><small><%# Eval("NgayDuyet","{0:dd/MM/yyyy HH:mm}") %></small></div>
                                        </td>
                                        <td class="text-center">
                                            <asp:LinkButton
                                                ID="btn_duyet"
                                                runat="server"
                                                CssClass="button success small"
                                                CommandArgument='<%# Eval("ID") %>'
                                                OnClick="btn_duyet_Click"
                                                Visible='<%# Eval("TrangThai").ToString()=="0" %>'>
                                                Duyệt
                                            </asp:LinkButton>

                                            <asp:LinkButton
                                                ID="btn_tuchoi"
                                                runat="server"
                                                CssClass="button alert small"
                                                CommandArgument='<%# Eval("ID") %>'
                                                OnClick="btn_tuchoi_Click"
                                                Visible='<%# Eval("TrangThai").ToString()=="0" %>'>
                                                Từ chối
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>

                <asp:Panel ID="pn_empty" runat="server" Visible="false" CssClass="mt-3">
                    <div class="text-center">Chưa có yêu cầu nâng cấp Level 2.</div>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
