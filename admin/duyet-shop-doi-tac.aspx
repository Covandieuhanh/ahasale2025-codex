<%@ Page Title="Duyệt gian hàng đối tác (Shop)" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master"
    AutoEventWireup="true" CodeFile="duyet-shop-doi-tac.aspx.cs"
    Inherits="admin_duyet_shop_doi_tac" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="p-3 aha-admin-section">
                <h4 class="mb-3">DUYỆT MỞ KHÔNG GIAN /SHOP ĐỐI TÁC CHIẾN LƯỢC</h4>
                <p class="mb-3 text-muted">Tài khoản Home gửi đăng ký mở <code>/shop</code> kèm % chiết khấu tại <code>/home/mo-khong-gian.aspx?space=shop</code>. Khi duyệt, hệ thống mở quyền truy cập <code>/shop</code> và lưu chính sách % mặc định cho toàn bộ tin đăng sau này của shop.</p>

                <div class="card p-3 mb-3">
                    <div class="row g-3 align-items-end">
                        <div class="col-md-9">
                            <label class="form-label fw-600">Ghi chú admin</label>
                            <asp:TextBox ID="txt_shop_admin_note" runat="server" CssClass="form-control" placeholder="Ghi chú duyệt, từ chối hoặc hủy duyệt"></asp:TextBox>
                        </div>
                        <div class="col-md-3 d-flex gap-2">
                            <asp:Button ID="btn_shop_reload" runat="server" CssClass="button secondary" Text="Làm mới" OnClick="btn_shop_reload_Click" />
                        </div>
                    </div>
                </div>

                <div class="bcorn-fix-title-table-container aha-admin-grid">
                    <table class="bcorn-fix-title-table">
                        <thead>
                            <tr>
                                <th style="min-width: 170px;">Tài khoản Home</th>
                                <th style="min-width: 180px;">Họ tên</th>
                                <th style="min-width: 220px;">Tên shop hiển thị</th>
                                <th style="min-width: 150px;">% đăng ký</th>
                                <th style="min-width: 160px;">% đang áp dụng</th>
                                <th style="min-width: 130px;">Trạng thái</th>
                                <th style="min-width: 170px;">Ngày gửi</th>
                                <th style="min-width: 170px;">Ngày xử lý</th>
                                <th style="min-width: 240px;">Ghi chú</th>
                                <th style="min-width: 220px;" class="text-center">Thao tác</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rp_shop_requests" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr>
                                        <td class="fw-600"><%# Eval("AccountKey") %></td>
                                        <td><%# Eval("FullName") %></td>
                                        <td><%# Eval("ShopName") %></td>
                                        <td><%# Eval("RequestedCommissionPercent") %>%</td>
                                        <td><%# Eval("PolicyCommissionText") %></td>
                                        <td><span class="status-pill"><%# Eval("RequestStatusText") %></span></td>
                                        <td><%# Eval("RequestedAtText") %></td>
                                        <td><%# Eval("ReviewedAtText") %></td>
                                        <td><%# Eval("ReviewNote") %></td>
                                        <td class="text-center">
                                            <asp:LinkButton ID="btn_shop_approve" runat="server" CssClass="button success small" CommandArgument='<%# Eval("RequestId") %>' OnClick="btn_shop_approve_Click" Visible='<%# Eval("CanApprove") %>'>Duyệt</asp:LinkButton>
                                            <asp:LinkButton ID="btn_shop_reject" runat="server" CssClass="button alert small" CommandArgument='<%# Eval("RequestId") %>' OnClick="btn_shop_reject_Click" Visible='<%# Eval("CanReject") %>'>Từ chối</asp:LinkButton>
                                            <asp:LinkButton ID="btn_shop_cancel" runat="server" CssClass="button warning small" CommandArgument='<%# Eval("RequestId") %>' OnClick="btn_shop_cancel_Click" Visible='<%# Eval("CanCancel") %>'>Hủy duyệt</asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:PlaceHolder ID="ph_shop_empty" runat="server" Visible="false">
                                <tr>
                                    <td colspan="10" class="text-center text-secondary py-4">
                                        Chưa có yêu cầu mở không gian /shop nào từ tài khoản Home.
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
