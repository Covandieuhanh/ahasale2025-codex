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
                            <div class="alert alert-info mb-4" role="alert">
                                Mã PIN chỉ được đặt bằng <strong>4 chữ số</strong>.
                            </div>

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
                                <a href="/home/khoi-phuc-ma-pin.aspx" class="link-secondary">Quên mã PIN?</a>
                                <asp:Button ID="btnDoiPin" runat="server"
                                    Text="Đổi mã PIN"
                                    OnClick="btnDoiPin_Click"
                                    CssClass="btn btn-primary px-4" />
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

<asp:Content ID="Content5" ContentPlaceHolderID="foot_sau" runat="Server">
    <script type="text/javascript">
        (function () {
            if (window.Sys && Sys.WebForms) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    const pinBox = document.getElementById('<%= txt_PinCu.ClientID %>');
                    if (pinBox) pinBox.focus();
                });
            }
        })();
    </script>
</asp:Content>
