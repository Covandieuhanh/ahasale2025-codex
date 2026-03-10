<%@ Page Title="Khôi phục mật khẩu" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true" CodeFile="khoi-phuc-mat-khau.aspx.cs" Inherits="home_khoi_phuc_mat_khau" %>

<asp:Content ID="cHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>
<asp:Content ID="cHeadSau" ContentPlaceHolderID="head_sau" runat="Server"></asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="page page-center">
                <div class="container container-tight py-4">
                    <div class="card card-md">
                        <div class="card-body">
                            <h2 class="h2 text-center mb-4">Khôi phục mật khẩu</h2>

                            <div class="mb-3">
                                <label class="form-label">Số điện thoại đã đăng ký</label>
                                <div class="input-group input-group-flat">
                                    <span class="input-group-text">
                                        <i class="ti ti-phone"></i>
                                    </span>
                                    <asp:TextBox ID="txt_phone" runat="server"
                                        CssClass="form-control"
                                        MaxLength="20"
                                        placeholder="Nhập số điện thoại"
                                        autocomplete="tel"></asp:TextBox>
                                </div>
                                <div class="text-secondary mt-2">
                                    <small>Mã OTP sẽ hết hạn sau 5 phút.</small>
                                </div>
                            </div>

                            <div class="d-flex align-items-center justify-content-between">
                                <a href="/dang-nhap" class="link-secondary">Quay lại đăng nhập</a>
                                <asp:Button ID="btnSendOtp" runat="server"
                                    Text="Gửi OTP"
                                    CssClass="btn btn-success px-4"
                                    OnClick="btnSendOtp_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="0">
        <ProgressTemplate>
            <div class="page-loading active">
                <div class="page-loading-card">
                    <span class="spinner-border"></span>
                    <div class="text-secondary">Loading...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
