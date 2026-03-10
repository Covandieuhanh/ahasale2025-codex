<%@ Page Title="Đặt lại mật khẩu" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true" CodeFile="dat-lai-mat-khau.aspx.cs" Inherits="shop_dat_lai_mat_khau" %>

<asp:Content ID="cHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>
<asp:Content ID="cHeadSau" ContentPlaceHolderID="head_sau" runat="Server"></asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="main" runat="Server">
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

                        <div class="mb-3">
                            <label class="form-label">Mật khẩu mới</label>
                            <div class="aha-password-field">
                                <asp:TextBox ID="txt_pass" runat="server"
                                    CssClass="form-control js-password"
                                    MaxLength="120"
                                    TextMode="Password"
                                    placeholder="Nhập mật khẩu mới"></asp:TextBox>
                                <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện mật khẩu">
                                    <span class="aha-password-toggle-label">Hiện</span>
                                </button>
                            </div>
                        </div>

                        <div class="mb-3">
                            <label class="form-label">Nhập lại mật khẩu</label>
                            <div class="aha-password-field">
                                <asp:TextBox ID="txt_pass_confirm" runat="server"
                                    CssClass="form-control js-password"
                                    MaxLength="120"
                                    TextMode="Password"
                                    placeholder="Nhập lại mật khẩu"></asp:TextBox>
                                <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện mật khẩu">
                                    <span class="aha-password-toggle-label">Hiện</span>
                                </button>
                            </div>
                        </div>

                        <div class="d-flex align-items-center justify-content-between">
                            <asp:HyperLink ID="hl_back" runat="server" CssClass="link-secondary">Quay lại đăng nhập</asp:HyperLink>
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
</asp:Content>
