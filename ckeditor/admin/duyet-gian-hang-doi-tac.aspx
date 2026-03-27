<%@ Page Title="Duyệt gian hàng đối tác" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master"
    AutoEventWireup="true" CodeFile="duyet-gian-hang-doi-tac.aspx.cs"
    Inherits="admin_duyet_gian_hang_doi_tac" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="p-3">
                <h4 class="mb-3">DUYỆT ĐĂNG KÝ GIAN HÀNG ĐỐI TÁC</h4>

                <div class="bcorn-fix-title-table-container">
                    <table class="bcorn-fix-title-table">
                        <thead>
                            <tr>
                                <th style="min-width: 70px;">ID</th>
                                <th style="min-width: 170px;">Tài khoản</th>
                                <th style="min-width: 170px;">Ngày tạo</th>
                                <th style="min-width: 130px;">Trạng thái</th>
                                <th style="min-width: 240px;">Ghi chú</th>
                                <th style="min-width: 220px;" class="text-center">Thao tác</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rp_dangky" runat="server">
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
                                        <td class="text-center">

                                            <!-- Chỉ hiện khi đang chờ duyệt -->
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

                                            <!-- Chỉ hiện khi đã duyệt: cho phép Hủy duyệt -->
                                            <asp:LinkButton
                                                ID="btn_huyduyet"
                                                runat="server"
                                                CssClass="button warning small"
                                                CommandArgument='<%# Eval("ID") %>'
                                                OnClick="btn_huyduyet_Click"
                                                Visible='<%# Eval("TrangThai").ToString()=="1" %>'>
                                                Hủy duyệt
                                            </asp:LinkButton>

                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>

            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
