<%@ Page Title="Đăng nhập" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="home_login" %>

<asp:Content ID="cHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
    <style>
        /* Nền trắng-xanh giống trang chủ */
        .aha-login-wrap {
            min-height: calc(100vh - 56px); /* trừ header nếu master có */
            background: linear-gradient(180deg, rgba(32, 201, 151, .18) 0%, rgba(32, 201, 151, .06) 35%, rgba(255,255,255,1) 100%);
            position: relative;
            overflow: hidden;
        }

        /* hoạ tiết nhẹ kiểu banner xanh */
        .aha-login-wrap:before {
            content: "";
            position: absolute;
            inset: -120px -120px auto -120px;
            height: 320px;
            background: radial-gradient(circle at 30% 30%, rgba(32,201,151,.35), transparent 55%),
                        radial-gradient(circle at 70% 40%, rgba(34,197,94,.25), transparent 60%);
            filter: blur(0px);
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

    <asp:UpdatePanel ID="upAll" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="aha-login-wrap">
                <div class="container container-tight py-5">
                    <div class="text-center mb-4">
                        <div class="aha-brand mb-3">
                            <img src="/uploads/images/logo-aha-trang.png" alt="AhaSale" />
                        </div>
                        <div class="aha-title h2 mb-1">Đăng nhập AHASALE.VN</div>
                        <div class="aha-sub">Giá sale, gần bạn</div>
                    </div>

                    <div class="card card-md aha-login-card">
                        <div class="card-body">
                            <asp:PlaceHolder ID="ph_switch_home_mode" runat="server" Visible="false">
                                <div class="alert alert-warning" role="alert">
                                    Bạn đang ở chế độ gian hàng đối tác. Để dùng lại tài khoản cá nhân home, vui lòng chuyển chế độ.
                                    <div class="mt-2">
                                        <a href="/dang-nhap?switch=home" class="btn btn-sm btn-warning">Chuyển sang tài khoản cá nhân</a>
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                            <asp:Panel ID="pnLogin" runat="server" DefaultButton="but_login">
                                <asp:HiddenField ID="hid_return_url" runat="server" />
                                <input type="hidden" name="home_return_url_shadow" value="<%=HttpUtility.HtmlAttributeEncode(RenderRequestedReturnUrl()) %>" />
                                <div class="mb-3">
                                    <label class="form-label">Tài khoản / Số điện thoại / Email</label>
                                    <div class="input-group input-group-flat">
                                        <span class="input-group-text">
                                            <i class="ti ti-phone"></i>
                                        </span>
                                        <asp:TextBox ID="txt_user" runat="server"
                                            CssClass="form-control"
                                            MaxLength="120"
                                            placeholder="Nhập tài khoản hoặc số điện thoại hoặc email"
                                            autocomplete="username"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Mật khẩu</label>
                                    <div class="input-group input-group-flat">
                                        <span class="input-group-text">
                                            <i class="ti ti-key"></i>
                                        </span>

                                        <asp:TextBox ID="txt_pass" runat="server"
                                            CssClass="form-control js-password"
                                            MaxLength="50"
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
                                    <a href="/home/khoi-phuc-mat-khau.aspx" class="link-secondary">Quên mật khẩu?</a>
                                    <a href="/dang-ky" class="link-secondary">Đăng ký</a>
                                </div>

                                <div class="d-flex align-items-center justify-content-end mb-3">
                                    <asp:Button ID="but_login" runat="server"
                                        Text="ĐĂNG NHẬP"
                                        CssClass="btn btn-success px-4"
                                        OnClick="but_login_Click" />
                                </div>
                            </asp:Panel>

                        </div>
                    </div>

                    <div class="text-center text-secondary mt-4">
                        <small>© AHASALE.VN</small>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress runat="server" ID="upProg" AssociatedUpdatePanelID="upAll" DisplayAfter="0">
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

<asp:Content ID="cFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server">
</asp:Content>

<asp:Content ID="cFootSau" ContentPlaceHolderID="foot_sau" runat="Server">
    <script type="text/javascript">
        (function () {
            function resolveReturnUrl(form) {
                try {
                    var action = (form && form.action) ? form.action : '';
                    var fromAction = new URL(action, window.location.origin).searchParams.get('return_url')
                        || new URL(action, window.location.origin).searchParams.get('returnUrl');
                    if (fromAction) return fromAction;
                } catch (e) { }

                try {
                    var fromPage = new URL(window.location.href).searchParams.get('return_url')
                        || new URL(window.location.href).searchParams.get('returnUrl');
                    if (fromPage) return fromPage;
                } catch (e) { }

                return '';
            }

            function syncReturnUrlFields() {
                var form = document.getElementById('ctl00_form1');
                if (!form) return;

                var hidden = document.getElementById('<%= hid_return_url.ClientID %>');
                var shadow = form.querySelector('input[name="home_return_url_shadow"]');
                var value = resolveReturnUrl(form);
                if (!value) return;

                if (hidden) hidden.value = value;
                if (shadow) shadow.value = value;

                try {
                    var cookieValue = encodeURIComponent(value);
                    var cookie = 'save_url_home_aka=' + cookieValue + '; path=/; SameSite=Lax';
                    if (window.location.protocol === 'https:') cookie += '; Secure';
                    document.cookie = cookie;
                } catch (e) { }
            }

            syncReturnUrlFields();

            if (window.Sys && Sys.WebForms) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    var u = document.getElementById('<%= txt_user.ClientID %>');
                    if (u) u.focus();
                    syncReturnUrlFields();
                });
            }

            var form = document.getElementById('ctl00_form1');
            if (form) {
                form.addEventListener('submit', function () {
                    syncReturnUrlFields();
                });
            }
        })();
    </script>
</asp:Content>
