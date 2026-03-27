<%@ Page Title="Đặt lịch" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="dat-lich.aspx.cs" Inherits="gianhang_dat_lich" %>

<asp:Content ID="Content1" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl">
        <div class="page-header d-print-none">
            <div class="row align-items-center">
                <div class="col">
                    <div class="page-pretitle">Gian hàng</div>
                    <h2 class="page-title">Đặt lịch hẹn</h2>
                </div>
            </div>
        </div>

        <asp:Panel ID="pn_not_found" runat="server" Visible="false" CssClass="alert alert-warning">
            Gian hàng không tồn tại hoặc chưa sẵn sàng nhận đặt lịch.
        </asp:Panel>

        <asp:Panel ID="pn_form" runat="server" Visible="false" CssClass="card">
            <div class="card-body">
                <div class="mb-3">
                    <asp:Label ID="lb_gianhang_name" runat="server" CssClass="fw-bold"></asp:Label>
                    <div class="text-muted"><asp:Label ID="lb_gianhang_desc" runat="server"></asp:Label></div>
                </div>

                <div class="row">
                    <div class="col-md-6 mb-3">
                        <label class="form-label">Họ tên</label>
                        <asp:TextBox ID="txt_ten" runat="server" CssClass="form-control" placeholder="Nhập họ tên"></asp:TextBox>
                    </div>
                    <div class="col-md-6 mb-3">
                        <label class="form-label">Số điện thoại</label>
                        <asp:TextBox ID="txt_sdt" runat="server" CssClass="form-control" placeholder="Nhập SĐT"></asp:TextBox>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-6 mb-3">
                        <label class="form-label">Chọn dịch vụ</label>
                        <asp:DropDownList ID="ddl_dichvu" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="col-md-6 mb-3">
                        <label class="form-label">Dịch vụ khác (tuỳ chọn)</label>
                        <asp:TextBox ID="txt_dichvu_khac" runat="server" CssClass="form-control" placeholder="Nhập dịch vụ nếu chưa có trong danh sách"></asp:TextBox>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-6 mb-3">
                        <label class="form-label">Ngày hẹn</label>
                        <asp:TextBox ID="txt_ngay" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-md-6 mb-3">
                        <label class="form-label">Giờ hẹn</label>
                        <asp:TextBox ID="txt_gio" runat="server" CssClass="form-control" TextMode="Time"></asp:TextBox>
                    </div>
                </div>

                <div class="mb-3">
                    <label class="form-label">Ghi chú</label>
                    <asp:TextBox ID="txt_ghichu" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                </div>

                <asp:Button ID="but_submit" runat="server" Text="Gửi lịch hẹn" CssClass="btn btn-primary" OnClick="but_submit_Click" />
            </div>
        </asp:Panel>
    </div>
</asp:Content>
