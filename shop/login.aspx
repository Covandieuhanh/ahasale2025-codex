<%@ Page Title="Đăng nhập shop" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="shop_login" %>

<asp:Content ID="cHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
    <style>
        .aha-login-wrap {
            min-height: calc(100vh - 56px);
            background: linear-gradient(180deg, rgba(32, 201, 151, .18) 0%, rgba(32, 201, 151, .06) 35%, rgba(255,255,255,1) 100%);
            position: relative;
            overflow: hidden;
        }

        .aha-login-wrap:before {
            content: "";
            position: absolute;
            inset: -120px -120px auto -120px;
            height: 320px;
            background: radial-gradient(circle at 30% 30%, rgba(32,201,151,.35), transparent 55%), radial-gradient(circle at 70% 40%, rgba(34,197,94,.25), transparent 60%);
            pointer-events: none;
        }

        .aha-login-card {
            border: 1px solid rgba(98, 105, 118, .16);
            box-shadow: 0 12px 36px rgba(0,0,0,.08);
        }

        .aha-brand {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
        }

        .aha-brand img {
            height: 56px;
            width: auto;
        }

        .aha-title {
            font-weight: 700;
            letter-spacing: .5px;
        }

        .aha-sub {
            color: #626976;
        }
    </style>
</asp:Content>

<asp:Content ID="cHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
</asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="main" runat="Server">
    <div class="aha-login-wrap">
        <div class="container container-tight py-5">
            <div class="text-center mb-4">
                <div class="aha-brand mb-3">
                    <img src="/uploads/images/logo-aha-trang.png" alt="AhaSale" />
                </div>
                <div class="aha-title h2 mb-1">Đăng nhập gian hàng AhaSale</div>
                <div class="aha-sub">Dành cho tài khoản shop quản lý sản phẩm và đơn hàng.</div>
            </div>

            <div class="card card-md aha-login-card">
                <div class="card-body">
                    <asp:PlaceHolder ID="ph_switch_shop_mode" runat="server" Visible="false">
                        <div class="alert alert-warning" role="alert">
                            Bạn đang ở chế độ tài khoản cá nhân.
                            <div class="mt-2">
                                <a href="/shop/login.aspx?switch=shop" class="btn btn-sm btn-warning">Chuyển sang gian hàng đối tác</a>
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <asp:Panel ID="pnLogin" runat="server" DefaultButton="but_login">
                        <div class="mb-3">
                            <label class="form-label">Email đăng nhập</label>
                            <div class="input-group input-group-flat">
                                <span class="input-group-text">
                                    <i class="ti ti-mail"></i>
                                </span>
                                <asp:TextBox ID="txt_user" runat="server"
                                    CssClass="form-control"
                                    MaxLength="120"
                                    placeholder="Nhập email tài khoản shop"
                                    autocomplete="email"></asp:TextBox>
                            </div>
                            <small class="text-secondary">Trang shop chỉ hỗ trợ đăng nhập bằng email.</small>
                        </div>

                        <div class="mb-3">
                            <label class="form-label">Mật khẩu</label>
                            <div class="input-group input-group-flat">
                                <span class="input-group-text">
                                    <i class="ti ti-key"></i>
                                </span>
                                <asp:TextBox ID="txt_pass" runat="server"
                                    CssClass="form-control js-password"
                                    MaxLength="120"
                                    TextMode="Password"
                                    placeholder="Nhập mật khẩu"
                                    autocomplete="current-password"></asp:TextBox>
                                <span class="input-group-text">
                                    <a href="javascript:void(0);" class="link-secondary js-toggle-password"
                                        aria-label="Hiện mật khẩu" data-bs-toggle="tooltip" data-bs-original-title="Hiện mật khẩu">
                                        <i class="ti ti-eye"></i>
                                    </a>
                                </span>
                            </div>
                        </div>

                        <div class="d-flex align-items-center justify-content-between mb-3">
                            <asp:LinkButton ID="but_show_form_quenmk" runat="server"
                                CssClass="link-secondary"
                                OnClick="but_show_form_quenmk_Click">
                                Quên mật khẩu?
                            </asp:LinkButton>
                        </div>

                        <asp:Button ID="but_login" runat="server"
                            Text="ĐĂNG NHẬP SHOP"
                            CssClass="btn btn-success w-100"
                            OnClick="but_login_Click" />

                        <div class="mt-3 text-danger fw-semibold">
                            <asp:Label ID="lb_msg" runat="server" Text=""></asp:Label>
                        </div>
                    </asp:Panel>

                    <div class="mt-4 text-center">
                        <a class="link-secondary" href="/shop/dang-ky.aspx">Đăng ký tài khoản shop</a>
                        <span class="text-secondary mx-2">|</span>
                        <a class="link-secondary" href="/dang-nhap">Đăng nhập tài khoản cá nhân</a>
                    </div>
                </div>
            </div>

            <div class="text-center text-secondary mt-4">
                <small>© AHASALE.VN</small>
            </div>
        </div>
    </div>

    <!-- Modal: Khôi phục mật khẩu shop -->
    <asp:Panel ID="pn_khoiphuc" runat="server" Visible="false">
        <div class="modal fade show" style="display:block;" tabindex="-1" role="dialog" aria-modal="true">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Khôi phục mật khẩu shop</h5>
                        <asp:LinkButton ID="but_close_form_quenmk" runat="server"
                            CssClass="btn-close"
                            OnClick="but_close_form_quenmk_Click"
                            aria-label="Close" />
                    </div>

                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">Nhập email đăng nhập của bạn</label>
                            <asp:TextBox ID="txt_email_khoiphuc" runat="server"
                                CssClass="form-control"
                                MaxLength="120"
                                placeholder="email@domain.com"
                                autocomplete="email"></asp:TextBox>
                            <div class="text-secondary mt-2">
                                <small>Hệ thống sẽ gửi OTP qua email (hết hạn sau 5 phút).</small>
                            </div>
                        </div>
                    </div>

                    <div class="modal-footer">
                        <asp:Button ID="but_nhanma" runat="server"
                            Text="Gửi OTP"
                            CssClass="btn btn-success"
                            OnClick="but_nhanma_Click" />
                    </div>
                </div>
            </div>
        </div>
        <div class="modal-backdrop fade show"></div>
    </asp:Panel>
</asp:Content>

<asp:Content ID="cFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server">
</asp:Content>

<asp:Content ID="cFootSau" ContentPlaceHolderID="foot_sau" runat="Server">
    <script type="text/javascript">
    </script>
</asp:Content>
