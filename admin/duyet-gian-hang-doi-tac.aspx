<%@ Page Title="Duyệt không gian gian hàng" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master"
    AutoEventWireup="true" CodeFile="duyet-gian-hang-doi-tac.aspx.cs"
    Inherits="admin_duyet_gian_hang_doi_tac" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="p-3 aha-admin-section">
                <h4 class="mb-3">DUYỆT ĐĂNG KÝ MỞ KHÔNG GIAN /GIANHANG CHO TÀI KHOẢN HOME</h4>
                <p class="mb-3 text-muted">Tài khoản Home gửi đăng ký tại <code>/home/dang-ky-gian-hang-doi-tac.aspx</code>. Admin xử lý ngay trên bảng dưới bằng các trạng thái <strong>Duyệt</strong>, <strong>Từ chối</strong> hoặc <strong>Hủy duyệt</strong>. Không còn luồng mở quyền / khóa quyền thủ công riêng cho <code>/gianhang</code>.</p>

                <div class="card p-3 mb-3">
                    <div class="row g-3 align-items-end">
                        <div class="col-md-9">
                            <label class="form-label fw-600">Ghi chú admin</label>
                            <asp:TextBox ID="txt_home_admin_note" runat="server" CssClass="form-control" placeholder="Ghi chú duyệt, từ chối hoặc hủy duyệt"></asp:TextBox>
                        </div>
                        <div class="col-md-3 d-flex gap-2">
                            <asp:Button ID="btn_home_reload" runat="server" CssClass="button secondary" Text="Làm mới" OnClick="btn_home_reload_Click" />
                        </div>
                    </div>
                </div>

                <div class="bcorn-fix-title-table-container aha-admin-grid mb-4">
                    <table class="bcorn-fix-title-table">
                        <thead>
                            <tr>
                                <th style="min-width: 180px;">Tài khoản Home</th>
                                <th style="min-width: 180px;">Họ tên</th>
                                <th style="min-width: 260px;">Thông tin gian hàng</th>
                                <th style="min-width: 150px;">Yêu cầu</th>
                                <th style="min-width: 170px;">Ngày gửi</th>
                                <th style="min-width: 170px;">Ngày xử lý</th>
                                <th style="min-width: 260px;">Ghi chú</th>
                                <th style="min-width: 220px;" class="text-center">Thao tác</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rp_home_requests" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr>
                                        <td class="fw-600"><%# Eval("AccountKey") %></td>
                                        <td><%# Eval("FullName") %></td>
                                        <td>
                                            <strong><%# Eval("ShopName") %></strong><br />
                                            <span class="text-secondary"><%# Eval("ContactPhone") %></span><br />
                                            <span class="text-secondary"><%# Eval("ContactEmail") %></span><br />
                                            <span class="text-secondary"><%# Eval("PickupAddress") %></span>
                                        </td>
                                        <td><span class="status-pill"><%# Eval("RequestStatusText") %></span></td>
                                        <td><%# Eval("RequestedAtText") %></td>
                                        <td><%# Eval("ReviewedAtText") %></td>
                                        <td><%# Eval("ReviewNote") %></td>
                                        <td class="text-center">
                                            <asp:LinkButton ID="btn_home_approve" runat="server" CssClass="button success small" CommandArgument='<%# Eval("RequestId") %>' OnClick="btn_home_approve_Click" Visible='<%# Eval("CanApprove") %>'>Duyệt</asp:LinkButton>
                                            <asp:LinkButton ID="btn_home_reject" runat="server" CssClass="button alert small" CommandArgument='<%# Eval("RequestId") %>' OnClick="btn_home_reject_Click" Visible='<%# Eval("CanReject") %>'>Từ chối</asp:LinkButton>
                                            <asp:LinkButton ID="btn_home_cancel" runat="server" CssClass="button warning small" CommandArgument='<%# Eval("RequestId") %>' OnClick="btn_home_cancel_Click" Visible='<%# Eval("CanCancel") %>'>Hủy duyệt</asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:PlaceHolder ID="ph_home_empty" runat="server" Visible="false">
                                <tr>
                                    <td colspan="8" class="text-center text-secondary py-4">
                                        Chưa có yêu cầu mở không gian gian hàng nào từ tài khoản Home.
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                        </tbody>
                    </table>
                </div>

            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
