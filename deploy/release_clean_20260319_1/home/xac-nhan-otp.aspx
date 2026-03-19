<%@ Page Title="Xác nhận OTP" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true" CodeFile="xac-nhan-otp.aspx.cs" Inherits="home_xac_nhan_otp" %>

<asp:Content ID="cHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>
<asp:Content ID="cHeadSau" ContentPlaceHolderID="head_sau" runat="Server"></asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="page page-center">
                <div class="container container-tight py-4">
                    <div class="card card-md">
                        <div class="card-body">
                            <h2 class="h2 text-center mb-4">Xác nhận OTP</h2>

                            <asp:Panel ID="pn_error" runat="server" Visible="false">
                                <div class="alert alert-warning" role="alert">
                                    <asp:Literal ID="lit_error" runat="server"></asp:Literal>
                                </div>
                            </asp:Panel>

                            <asp:Panel ID="pn_send_warning" runat="server" Visible="false">
                                <div class="alert alert-warning" role="alert">
                                    <asp:Literal ID="lit_send_warning" runat="server"></asp:Literal>
                                </div>
                            </asp:Panel>

                            <asp:Panel ID="pn_form" runat="server" Visible="false">
                                <asp:HiddenField ID="hd_request_id" runat="server" />
                                <asp:HiddenField ID="hd_request_type" runat="server" />

                                <div class="mb-2 text-secondary">
                                    OTP đã được tạo cho <strong><asp:Literal ID="lit_phone" runat="server"></asp:Literal></strong>
                                </div>
                                <div class="alert alert-info" role="alert">
                                    Email có thể đến sau 1–3 phút, vui lòng kiểm tra Hộp thư rác (Spam).
                                </div>

                                <asp:Panel ID="pn_dev_otp" runat="server" Visible="false">
                                    <div class="alert alert-info" role="alert">
                                        OTP DEV: <strong><asp:Literal ID="lit_dev_otp" runat="server"></asp:Literal></strong>
                                    </div>
                                </asp:Panel>

                                <div class="mb-3">
                                    <label class="form-label">Mã OTP</label>
                                    <asp:TextBox ID="txt_otp" runat="server"
                                        CssClass="form-control"
                                        MaxLength="6"
                                        placeholder="Nhập mã OTP"
                                        autocomplete="one-time-code"></asp:TextBox>
                                </div>

                                <div class="d-flex align-items-center justify-content-between">
                                    <asp:HyperLink ID="hl_resend" runat="server" CssClass="link-secondary">Gửi lại OTP</asp:HyperLink>
                                    <asp:Button ID="btnVerify" runat="server"
                                        Text="Xác nhận OTP"
                                        CssClass="btn btn-success px-4"
                                        OnClick="btnVerify_Click" />
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
