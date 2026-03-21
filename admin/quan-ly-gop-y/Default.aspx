<%@ Page Title="Quản lý góp ý" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <Triggers>
            <%--<asp:AsyncPostBackTrigger ControlID="but_add" EventName="Click" />--%>
        </Triggers>
        <ContentTemplate>

            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" class="aha-admin-toolbar">
                    <ul class="h-menu bg-white">
                        <li class="d-block-lg d-none" data-role="hint" data-hint-position="top" data-hint-text="Lùi">
                            <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server"><span class="mif-chevron-left"></span></asp:LinkButton>
                        </li>
                        <li class="d-block-lg d-none" data-role="hint" data-hint-position="top" data-hint-text="Tới">
                            <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server"><span class="mif-chevron-right"></span></asp:LinkButton>
                        </li>
                    </ul>
                </div>
            </div>

            <div class="<%--border-top bd-lightGray--%> <%--pt-3 pl-3-lg pl-0 pr-3-lg pr-0 pb-3--%>p-3 aha-admin-section">
                <div class="d-none-lg d-block mb-3 mt-0-lg mt-3">
                    <div class="place-left">
                        <%--<b><%=ViewState["title"] %></b> Nó k kịp lưu vì nó tải trang này trước khi load menu-left--%>
                    </div>
                    <div class="place-right text-right">

                        <small class="pr-1">
                            <asp:Label ID="lb_show_md" runat="server" Text=""></asp:Label></small>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Lùi" ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" CssClass="button small light"><span class="mif-chevron-left"></span></asp:LinkButton>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Tới" ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" CssClass="button small light"><span class="mif-chevron-right"></span></asp:LinkButton>
                    </div>
                    <div class="clr-bc"></div>
                </div>

                <div class="row">
                    <div class="cell-lg-12">
                        <div class="bcorn-fix-title-table-container aha-admin-grid">
                            <%--style="padding-bottom: 300px"--%>
                            <table class="bcorn-fix-title-table">
                                <thead>
                                    <tr class="">
                                        <th>ID</th>
                                        <th>Tài khoản</th>
                                        <th>Họ tên</th>
                                        <th>Loại vấn đề</th>
                                        <th>Nội dung</th>
                                        <th>SĐT</th>
                                        <th>Ảnh đính kèm</th>
                                        <th>Ngày gửi</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# Eval("id") %></td>
                                                <td><%# Eval("taikhoan") %></td>
                                                <td><%# Eval("HoTen") %></td>
                                                <td><%# Eval("LoaiVanDe") %></td>
                                                <td><%# Eval("ChiTietYKien") %></td>
                                                <td><%# Eval("SoDienThoai") %></td>
                                                <td style="max-width: 220px">
                                                    <asp:Repeater ID="rpImages" runat="server"
                                                        DataSource='<%# Eval("Images") %>'>
                                                        <ItemTemplate>
                                                            <a href='<%# Container.DataItem %>' data-role="lightbox">
                                                                <img src='<%# Container.DataItem %>'
                                                                    style="width: 60px; height: 60px; object-fit: cover; margin: 2px; border-radius: 4px;" />
                                                            </a>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </td>

                                                <td><%# Eval("NgayTao", "{0:dd/MM/yyyy HH:mm}") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>

            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="admin-inline-progress" role="status" aria-live="polite"><span class="admin-inline-progress-spinner"></span><span class="admin-inline-progress-text">Đang xử lý...</span></div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>
