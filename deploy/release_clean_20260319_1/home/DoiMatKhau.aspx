<%@ Page Title="Đổi mật khẩu" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="DoiMatKhau.aspx.cs" Inherits="home_DoiMatKhau" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head_sau" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="page page-center">
                <div class="container container-tight py-4">
                    <%--<div class="text-center mb-4">
                        <img src="/Img/HeThong/logo-hota-2.png" width="90" />
                    </div>--%>
                    <div class="card card-md">
                        <div class="card-body">
                            <h2 class="h2 text-center mb-4">Đổi mật khẩu</h2>

                            <div class="mb-3">
                                <label class="form-label">
                                    Mật khẩu hiện tại
                                    <%-- <span class="form-label-description">
                                         <a href="/admin/QuenMatKhau.aspx">Quên mật khẩu</a>
                                     </span>--%>
                                </label>
                                <div class="input-group input-group-flat">
                                    <asp:TextBox ID="txt_MatKhauCu" runat="server"
                                        CssClass="form-control js-password"
                                        placeholder="Nhập mật khẩu hiện tại"
                                        TextMode="Password" autocomplete="off">
                                    </asp:TextBox>
                                    <span class="input-group-text">
                                        <a href="javascript:void(0);"
                                            class="link-secondary js-toggle-password"
                                            aria-label="Hiện mật khẩu"
                                            data-bs-toggle="tooltip" data-bs-original-title="Hiện mật khẩu">
                                            <i class="ti ti-eye"></i>
                                        </a>
                                    </span>
                                </div>
                            </div>

                            <div class="mb-3">
                                <label class="form-label">
                                    Mật khẩu mới
                                   
                                </label>
                                <div class="input-group input-group-flat">
                                    <asp:TextBox ID="txt_MatKhauMoi" runat="server"
                                        CssClass="form-control js-password"
                                        placeholder="Nhập mật khẩu mới"
                                        TextMode="Password" autocomplete="off">
                                    </asp:TextBox>
                                    <span class="input-group-text">
                                        <a href="javascript:void(0);"
                                            class="link-secondary js-toggle-password"
                                            aria-label="Hiện mật khẩu"
                                            data-bs-toggle="tooltip" data-bs-original-title="Hiện mật khẩu">
                                            <i class="ti ti-eye"></i>
                                        </a>
                                    </span>
                                </div>
                            </div>

                            <div class="mb-3">
                                <label class="form-label">
                                    Nhập lại mật khẩu mới
                                </label>
                                <div class="input-group input-group-flat">
                                    <asp:TextBox ID="txt_NhapLaiMatKhauMoi" runat="server"
                                        CssClass="form-control js-password"
                                        placeholder="Nhập lại mật khẩu mới"
                                        TextMode="Password" autocomplete="off">
                                    </asp:TextBox>
                                    <span class="input-group-text">
                                        <a href="javascript:void(0);"
                                            class="link-secondary js-toggle-password"
                                            aria-label="Hiện mật khẩu"
                                            data-bs-toggle="tooltip" data-bs-original-title="Hiện mật khẩu">
                                            <i class="ti ti-eye"></i>
                                        </a>
                                    </span>
                                </div>
                            </div>

                            <div class="form-footer">
                                <asp:Button ID="btnDoiMatKhau" runat="server" Text="Đổi mật khẩu"
                                    OnClick="btnDoiMatKhau_Click"
                                    CssClass="btn btn-primary w-100" />
                            </div>
                           <%-- <div class="text-center mt-3">
                                <a href="/admin" class="">
                                    <i class="ti ti-arrow-left me-1"></i>Quay về trang chủ
                                </a>
                            </div>--%>


                        </div>
                    </div>

                   <%-- <div class="text-center text-secondary mt-5">
                        Design by <a href="https://Hotasoft.com" target="_blank" tabindex="-1">Hotasoft.com</a>
                    </div>--%>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress runat="server" ID="upProg" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="0">
        <ProgressTemplate>
            <div class="page-loading active">
                <div class="page-loading-card">
                    <div class="mb-2">
                        <span class="spinner-border" role="status" aria-hidden="true"></span>
                    </div>
                    <div class="text-secondary">Loading...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="foot_truoc" runat="Server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="foot_sau" runat="Server">
    <script type="text/javascript">
        // Khi partial postback (UpdatePanel) hoàn tất
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            // Focus về ô mật khẩu hiện tại
            const passBox = document.getElementById('<%= txt_MatKhauCu.ClientID %>');
            if (passBox) passBox.focus();
        });
    </script>

</asp:Content>

