<%@ Page Title="Quản lý thông báo" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_quan_ly_thong_bao_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="mt-3">
        <div class="h5 mb-1">Quản lý thông báo</div>
        <div class="text-muted">Danh sách thông báo của tài khoản hiện tại.</div>
    </div>

    <asp:Panel ID="PanelEmpty" runat="server" Visible="false" CssClass="mt-4 p-4 border bd-default bg-white">
        Chưa có thông báo nào.
    </asp:Panel>

    <div class="mt-3">
        <table class="table row-hover striped border bd-default">
            <thead>
                <tr>
                    <th style="width: 70px;">Trạng thái</th>
                    <th>Nội dung</th>
                    <th style="width: 170px;">Thời gian</th>
                    <th style="width: 180px;">Người gửi</th>
                    <th style="width: 90px;">Liên kết</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="Repeater1" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# (bool)Eval("daxem") ? "<span class='fg-green'>Đã xem</span>" : "<span class='fg-red'>Chưa xem</span>" %>
                            </td>
                            <td><%# Eval("noidung") %></td>
                            <td><%# Eval("thoigian") %></td>
                            <td><%# Eval("nguoithongbao") %></td>
                            <td>
                                <a href="<%# Eval("link_full") %>" class="fg-cyan fg-darkCyan-hover">Mở</a>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>
</asp:Content>
