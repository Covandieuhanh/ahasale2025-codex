<%@ Page Title="Đổi mã pin" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true" CodeFile="DoiPin.aspx.cs" Inherits="home_DoiPin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="head_sau" runat="Server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="page page-center">
                <div class="container container-tight py-4">
                    <div class="card card-md">
                        <div class="card-body">
                            <h2 class="h2 text-center mb-4">Đổi mã PIN</h2>

                            <!-- PIN hiện tại -->
                            <div class="mb-3">
                                <label class="form-label">Mã PIN hiện tại</label>
                                <div class="input-group input-group-flat">
                                    <asp:TextBox ID="txt_PinCu" runat="server"
                                        CssClass="form-control js-password"
                                        placeholder="Nhập mã PIN hiện tại"
                                        MaxLength="4"
                                        Attributes="inputmode:numeric;pattern:[0-9]*"
                                        TextMode="Password" autocomplete="off" />
                                    <span class="input-group-text">
                                        <a href="javascript:void(0);" class="link-secondary js-toggle-password" aria-label="Hiện PIN">
                                            <i class="ti ti-eye"></i>
                                        </a>
                                    </span>
                                </div>
                            </div>

                            <!-- PIN mới -->
                            <div class="mb-3">
                                <label class="form-label">Mã PIN mới</label>
                                <div class="input-group input-group-flat">
                                    <asp:TextBox ID="txt_PinMoi" runat="server"
                                        CssClass="form-control js-password"
                                        placeholder="Nhập mã PIN mới"
                                        MaxLength="4"
                                        Attributes="inputmode:numeric;pattern:[0-9]*"
                                        TextMode="Password" autocomplete="off" />
                                    <span class="input-group-text">
                                        <a href="javascript:void(0);" class="link-secondary js-toggle-password" aria-label="Hiện PIN">
                                            <i class="ti ti-eye"></i>
                                        </a>
                                    </span>
                                </div>
                            </div>

                            <!-- Nhập lại PIN mới -->
                            <div class="mb-3">
                                <label class="form-label">Nhập lại mã PIN mới</label>
                                <div class="input-group input-group-flat">
                                    <asp:TextBox ID="txt_NhapLaiPinMoi" runat="server"
                                        CssClass="form-control js-password"
                                        placeholder="Nhập lại mã PIN mới"
                                        MaxLength="4"
                                        Attributes="inputmode:numeric;pattern:[0-9]*"
                                        TextMode="Password" autocomplete="off" />
                                    <span class="input-group-text">
                                        <a href="javascript:void(0);" class="link-secondary js-toggle-password" aria-label="Hiện PIN">
                                            <i class="ti ti-eye"></i>
                                        </a>
                                    </span>
                                </div>
                            </div>

                            <div class="d-flex align-items-center justify-content-between mb-3">
                                <!-- Quên PIN -->
                                <asp:LinkButton ID="but_show_form_quenpin" runat="server"
                                    CssClass="link-secondary"
                                    OnClick="but_show_form_quenpin_Click">
                                    Quên mã PIN?
                                </asp:LinkButton>

                                <!-- Đổi PIN -->
                                <asp:Button ID="btnDoiPin" runat="server"
                                    Text="Đổi mã PIN"
                                    OnClick="btnDoiPin_Click"
                                    CssClass="btn btn-primary px-4" />
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <!-- Modal: Quên PIN (Tabler/Bootstrap) -->
            <asp:Panel ID="pn_quenpin" runat="server" Visible="false">
                <div class="modal fade show" style="display:block;" tabindex="-1" role="dialog" aria-modal="true">
                    <div class="modal-dialog modal-dialog-centered" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Quên mã PIN</h5>
                                <asp:LinkButton ID="but_close_form_quenpin" runat="server"
                                    CssClass="btn-close"
                                    OnClick="but_close_form_quenpin_Click"
                                    aria-label="Close" />
                            </div>

                            <div class="modal-body">
                                <div class="mb-3">
                                    <label class="form-label">Nhập email khôi phục của bạn</label>
                                    <asp:TextBox ID="txt_email_quenpin" runat="server"
                                        CssClass="form-control"
                                        MaxLength="100"
                                        placeholder="email@domain.com"></asp:TextBox>
                                    <div class="text-secondary mt-2">
                                        <small>Hệ thống sẽ gửi mã PIN mới vào email của bạn. (Giới hạn mỗi 5 phút 1 lần)</small>
                                    </div>
                                </div>
                            </div>

                            <div class="modal-footer">
                                <asp:Button ID="but_gui_pin_moi" runat="server"
                                    Text="Gửi PIN mới về email"
                                    CssClass="btn btn-success"
                                    OnClick="but_gui_pin_moi_Click" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-backdrop fade show"></div>
            </asp:Panel>

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

<asp:Content ID="Content5" ContentPlaceHolderID="foot_sau" runat="Server">
    <script type="text/javascript">
        // Toggle password / pin (giống login)
        (function () {
            function wireToggle() {
                document.querySelectorAll('.js-toggle-password').forEach(function (btn) {
                    btn.onclick = function () {
                        var input = btn.closest('.input-group').querySelector('.js-password');
                        if (!input) return;
                        input.type = (input.type === 'password') ? 'text' : 'password';
                    };
                });
            }

            wireToggle();

            if (window.Sys && Sys.WebForms) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    wireToggle();
                    const pinBox = document.getElementById('<%= txt_PinCu.ClientID %>');
                    if (pinBox) pinBox.focus();
                });
            }
        })();
    </script>
</asp:Content>
