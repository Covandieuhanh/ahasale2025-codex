<%@ Page Title="Đăng ký gian hàng đối tác"
    Language="C#"
    MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true"
    CodeFile="dang-ky-gian-hang-doi-tac.aspx.cs"
    Inherits="home_dang_ky_gian_hang_doi_tac" %>

<asp:Content ContentPlaceHolderID="main" runat="server">
<asp:UpdatePanel ID="up_main" runat="server">
<ContentTemplate>

<div class="container-xl py-4" style="max-width:640px">
    <!-- CARD ĐĂNG KÝ -->
    <div class="card">
        <div class="card-header">
            <h3 class="card-title">Đăng ký trở thành gian hàng đối tác</h3>
        </div>

        <div class="card-body">

            <!-- Trạng thái hiện tại -->
            <div class="mb-3">
                <label class="fw-bold">Trạng thái đăng ký</label>
                <asp:Literal ID="ltr_trangthai" runat="server" />
            </div>

            <!-- Box điều khoản -->
            <div class="mb-3">
                <label class="fw-bold">Điều khoản &amp; chính sách</label>
                <div class="alert alert-light mb-0" style="max-height:220px; overflow:auto;">
                    <div class="text-secondary">
                        <!-- Bạn thay nội dung thật ở đây -->
                        <p class="mb-2"><strong>1)</strong> Điều khoản tham gia gian hàng đối tác...</p>
                        <p class="mb-2"><strong>2)</strong> Chính sách duyệt, vận hành, trao đổi...</p>
                        <p class="mb-0"><strong>3)</strong> Quy định nội dung, sản phẩm, dịch vụ...</p>
                        <hr class="my-3" />
                        <a href="javascript:;" data-bs-toggle="modal" data-bs-target="#modal-dieukhoan">
                            Xem bản đầy đủ điều khoản &amp; chính sách
                        </a>
                    </div>
                </div>
            </div>

            <!-- Checkbox đồng ý -->
            <div class="form-check mt-3">
                <asp:CheckBox ID="chk_dongy" runat="server" CssClass="form-check-input" />
                <label class="form-check-label">
                    Tôi đã đọc và đồng ý với
                    <a href="javascript:;" data-bs-toggle="modal" data-bs-target="#modal-dieukhoan">
                        điều khoản và chính sách
                    </a>
                </label>
            </div>

        </div>

        <div class="card-footer text-end">
            <asp:Button ID="but_dangky" runat="server"
                CssClass="btn btn-success"
                Text="Đăng ký trở thành gian hàng đối tác"
                OnClick="but_dangky_Click" />
        </div>
    </div>

    <!-- CARD LỊCH SỬ -->
    <div class="card mt-4">
        <div class="card-header">
            <h4 class="card-title">Lịch sử đăng ký</h4>
        </div>

        <div class="table-responsive">
            <table class="table table-sm table-vcenter">
                <thead>
                    <tr>
                        <th>Thời gian</th>
                        <th>Trạng thái</th>
                        <th>Ghi chú</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rp_lichsu" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("NgayTao","{0:dd/MM/yyyy HH:mm}") %></td>
                                <td><%# Eval("TrangThaiText") %></td>
                                <td><%# Eval("GhiChuAdmin") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </div>
</div>

<!-- MODAL ĐIỀU KHOẢN -->
<div class="modal modal-blur fade" id="modal-dieukhoan" tabindex="-1">
  <div class="modal-dialog modal-lg modal-dialog-centered">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title">Điều khoản &amp; chính sách</h5>
        <button class="btn-close" data-bs-dismiss="modal"></button>
      </div>
      <div class="modal-body">
        <!-- Bạn thay nội dung thật ở đây -->
        <p><strong>Điều khoản &amp; chính sách (bản đầy đủ)</strong></p>
        <p class="mb-2">- Nội dung 1...</p>
        <p class="mb-2">- Nội dung 2...</p>
        <p class="mb-0">- Nội dung 3...</p>
      </div>
      <div class="modal-footer">
        <button class="btn btn-primary" data-bs-dismiss="modal">Đã hiểu</button>
      </div>
    </div>
  </div>
</div>

</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
