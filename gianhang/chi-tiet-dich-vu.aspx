<%@ Page Title="Chi tiết dịch vụ" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="chi-tiet-dich-vu.aspx.cs" Inherits="gianhang_chi_tiet_dich_vu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl">
        <asp:Panel ID="pn_not_found" runat="server" Visible="false" CssClass="alert alert-warning">
            Dịch vụ không tồn tại hoặc đã bị ẩn.
        </asp:Panel>

        <asp:Panel ID="pn_content" runat="server" Visible="false">
            <div class="row">
                <div class="col-lg-6 mb-4">
                    <asp:Image ID="img_service" runat="server" CssClass="img-fluid rounded" />
                </div>
                <div class="col-lg-6">
                    <h2 class="mb-2"><asp:Label ID="lb_name" runat="server"></asp:Label></h2>
                    <div class="text-muted mb-3"><asp:Label ID="lb_desc" runat="server"></asp:Label></div>
                    <div class="h3 text-primary mb-4">Giá: <asp:Label ID="lb_price" runat="server"></asp:Label> VNĐ</div>

                    <asp:Button ID="but_book" runat="server" Text="Đặt lịch ngay" CssClass="btn btn-primary mb-4" OnClick="but_book_Click" />

                    <div class="card">
                        <div class="card-body">
                            <asp:Literal ID="lit_content" runat="server"></asp:Literal>
                        </div>
                    </div>

                    <div class="mt-3">
                        <asp:HyperLink ID="lnk_back" runat="server" CssClass="btn btn-outline-secondary">Quay lại gian hàng đối tác</asp:HyperLink>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
