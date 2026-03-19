<%@ Page Title="Đặt lại mật khẩu" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true" CodeFile="dat-lai-mat-khau.aspx.cs" Inherits="home_dat_lai_mat_khau" %>

<asp:Content ID="cHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>
<asp:Content ID="cHeadSau" ContentPlaceHolderID="head_sau" runat="Server"></asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="page page-center">
                <div class="container container-tight py-4">
                    <div class="card card-md">
                        <div class="card-body">
                            <h2 class="h2 text-center mb-4">Đặt lại mật khẩu</h2>

                            <asp:Panel ID="pn_error" runat="server" Visible="false">
                                <div class="alert alert-warning" role="alert">
                                    <asp:Literal ID="lit_error" runat="server"></asp:Literal>
                                </div>
                            </asp:Panel>

                            <asp:Panel ID="pn_form" runat="server" Visible="false">
                                <asp:HiddenField ID="hd_request_id" runat="server" />

                                <div class="mb-2 text-secondary">
                                    Xác thực thành công cho <strong><asp:Literal ID="lit_phone" runat="server"></asp:Literal></strong>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Mật khẩu mới</label>
                                    <div class="input-group input-group-flat">
                                        <asp:TextBox ID="txt_new_pass" runat="server"
                                            CssClass="form-control js-password"
                                            MaxLength="50"
                                            TextMode="Password"
                                            placeholder="Nhập mật khẩu mới"></asp:TextBox>
                                        <span class="input-group-text">
                                            <a href="javascript:void(0);" class="link-secondary js-toggle-password" aria-label="Hiện mật khẩu">
                                                <i class="ti ti-eye"></i>
                                            </a>
                                        </span>
                                    </div>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Nhập lại mật khẩu mới</label>
                                    <div class="input-group input-group-flat">
                                        <asp:TextBox ID="txt_new_pass_repeat" runat="server"
                                            CssClass="form-control js-password"
                                            MaxLength="50"
                                            TextMode="Password"
                                            placeholder="Nhập lại mật khẩu mới"></asp:TextBox>
                                        <span class="input-group-text">
                                            <a href="javascript:void(0);" class="link-secondary js-toggle-password" aria-label="Hiện mật khẩu">
                                                <i class="ti ti-eye"></i>
                                            </a>
                                        </span>
                                    </div>
                                </div>

                                <div class="d-flex align-items-center justify-content-between">
                                    <a href="/dang-nhap" class="link-secondary">Quay lại đăng nhập</a>
                                    <asp:Button ID="btnReset" runat="server"
                                        Text="Đặt lại mật khẩu"
                                        CssClass="btn btn-success px-4"
                                        OnClick="btnReset_Click" />
                                </div>
                            </asp:Panel>
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
