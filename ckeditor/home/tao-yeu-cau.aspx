<%@ Page Title="Đề xuất gửi yêu cầu ghi nhận hoạt động"
    Language="C#"
    MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true"
    CodeFile="tao-yeu-cau.aspx.cs"
    Inherits="home_tao_yeu_cau"
    ValidateRequest="false" %>

<asp:Content ContentPlaceHolderID="main" runat="server">
<asp:UpdatePanel ID="up_main" runat="server">
<ContentTemplate>
<style>
    .yc-status {
        display: inline-flex;
        align-items: center;
        min-height: 28px;
        padding: 0 10px;
        border-radius: 999px;
        border: 1px solid #d9e3ee;
        background: #f5f9fc;
        color: #35566e;
        font-weight: 700;
        font-size: 12px;
    }

    .yc-status.pending {
        background: #fff7e8;
        border-color: #f0d4a4;
        color: #8d6220;
    }

    .yc-status.approved {
        background: #ecf8f1;
        border-color: #b5dbca;
        color: #286b46;
    }

    .yc-status.rejected {
        background: #feeeee;
        border-color: #efc4c4;
        color: #943838;
    }
</style>

<div class="container-xl py-4" style="max-width:640px">

    <!-- ====== HIỆN TRẠNG ====== -->
    <div class="card mb-3">
        <div class="card-header">
            <h3 class="card-title">Trạng thái hiện tại</h3>
        </div>
        <div class="card-body">
            <div class="alert alert-info mb-2">
                <asp:Label ID="lb_hientai" runat="server" />
            </div>

            <!-- MÔ TẢ HIỆN TẠI -->
            <div class="alert alert-secondary mb-2">
                <asp:Label ID="lb_mota_hientai" runat="server" />
            </div>

            <!-- TRÁCH NHIỆM HIỆN TẠI (CHỈ HIỆN Ở ĐÂY) -->
            <asp:Panel ID="pn_trachnhiem_hientai" runat="server" Visible="false">
                <div class="alert alert-warning mb-0">
                    <div class="fw-bold mb-1">Trách nhiệm</div>
                    <asp:Label ID="lb_trachnhiem_hientai" runat="server" />
                </div>
            </asp:Panel>
        </div>
    </div>

    <!-- ====== CHỌN LEVEL ====== -->
    <div class="card mb-3">
        <div class="card-header">
            <h3 class="card-title">Chọn cấp độ muốn đề xuất</h3>
        </div>
        <div class="card-body">

            <div class="mb-3">
                <label class="fw-bold">Cấp độ đề xuất</label>
                <asp:DropDownList ID="ddl_level"
                    runat="server"
                    CssClass="form-select"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="ddl_level_SelectedIndexChanged" />
            </div>

            <div class="alert alert-success mb-2">
                <asp:Label ID="lb_yeucau" runat="server" />
            </div>

            <div class="alert alert-secondary mb-0">
                <asp:Label ID="lb_mota_yeucau" runat="server" />
            </div>

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
            <asp:Button ID="but_gui_yeucau" runat="server"
                CssClass="btn btn-success"
                Text="Gửi yêu cầu"
                OnClick="but_gui_yeucau_Click" />
        </div>
    </div>

    <!-- ====== LỊCH SỬ YÊU CẦU ====== -->
    <div class="card mb-3">
        <div class="card-header">
            <h3 class="card-title">Lịch sử yêu cầu đã gửi</h3>
        </div>
        <div class="table-responsive">
            <table class="table table-vcenter card-table">
                <thead>
                    <tr>
                        <th style="width:1px;">ID</th>
                        <th>Ngày gửi</th>
                        <th>Hiện tại</th>
                        <th>Yêu cầu</th>
                        <th>Trạng thái</th>
                        <th>Ngày duyệt</th>
                        <th>Người duyệt</th>
                        <th>Ghi chú admin</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rp_lichsu_yeucau" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("id") %></td>
                                <td><%# Eval("NgayTao", "{0:dd/MM/yyyy HH:mm}") %></td>
                                <td><%# Eval("HienTaiText") %></td>
                                <td><%# Eval("YeuCauText") %></td>
                                <td>
                                    <span class='yc-status <%# Eval("TrangThaiCss") %>'><%# Eval("TrangThaiText") %></span>
                                </td>
                                <td><%# Eval("NgayDuyet", "{0:dd/MM/yyyy HH:mm}") %></td>
                                <td><%# Eval("NguoiDuyet") %></td>
                                <td><%# Eval("GhiChuAdmin") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
        <asp:Panel ID="pn_lichsu_empty" runat="server" Visible="false" CssClass="p-3 text-secondary">
            Chưa có yêu cầu nào.
        </asp:Panel>
    </div>

</div>

<!-- MODAL -->
<div class="modal modal-blur fade" id="modal-dieukhoan" tabindex="-1">
  <div class="modal-dialog modal-lg modal-dialog-centered">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title">Điều khoản & chính sách</h5>
        <button class="btn-close" data-bs-dismiss="modal"></button>
      </div>
      <div class="modal-body">
        Điều khoản và chính sách... bổ sung sau
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
