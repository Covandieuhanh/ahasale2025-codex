<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="admin_Default2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>GIANHANG ADMIN - AHASALE</title>
    <meta charset="UTF-8" />
    <meta http-equiv="content-language" content="vi" />
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=yes" />
    <meta property="og:type" content="website" />

    <asp:Literal ID="literal_fav_icon" runat="server"></asp:Literal>

    <link href="/Metro-UI-CSS-master/css/metro-all.min.css" rel="stylesheet" />
    <link href="/assetscss/aha-ui-refresh.css?v=2026-03-06.1" rel="stylesheet" />
    <link href="/assetscss/bcorn-with-metro.css" rel="stylesheet" />
    <link href="/assetscss/fix-metro.css" rel="stylesheet" />
    <link href="/assetscss/login.css?v=2026-03-06.2" rel="stylesheet" />

    <style>
        .body-login-bcorn1 {
            background-color: #7f1d1d;
            overflow: hidden;
        }

        .bg-login-bcorn1 {
            background: linear-gradient(140deg, #7f1d1d 0%, #c81e1e 55%, #ef4444 100%);
        }

        .admin-login-submit {
            background: #c81e1e !important;
            border-color: #9f1239 !important;
            color: #ffffff !important;
        }

        .admin-login-submit:hover {
            background: #b91c1c !important;
        }
    </style>

    <link rel='icon' href='/uploads/images/favicon.png' sizes='16x16' type='image/x-icon' />
    <link rel='icon' href='/uploads/images/favicon.png' sizes='32x32' type='image/x-icon' />
    <link rel='icon' href='/uploads/images/favicon.png' sizes='48x48' type='image/x-icon' />

    <link rel='apple-touch-icon' href='/uploads/images/avatar/avt-aha.jpg' sizes='180x180' />
    <link rel='apple-touch-icon' href='/uploads/images/avatar/avt-aha.jpg' sizes='167x167' />
    <link rel='apple-touch-icon' href='/uploads/images/avatar/avt-aha.jpg' sizes='152x152' />
    <link rel='apple-touch-icon' href='/uploads/images/avatar/avt-aha.jpg' sizes='120x120' />

    <link rel='icon' href='/uploads/images/avatar/avt-aha.jpg' sizes='192x192' />
    <link rel='icon' href='/uploads/images/avatar/avt-aha.jpg' sizes='144x144' />
    <meta property='og:title' content='GIANHANG ADMIN - AHASALE' />
    <meta property='og:image' content='/uploads/images/config/ac087060-8cf4-4fd7-bf20-a5fff4677952.jpg' />
</head>
<body class="body-login-bcorn1 admin-login-page admin-login-gianhang">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True"></asp:ScriptManager>

        <div class="bg-login-bcorn1">
            <div class="fg-white text-center" style="padding-top: 50px">
                <div class="text-bold" style="font-size: 17px">
                    ĐĂNG NHẬP /GIANHANG/ADMIN
                </div>
            </div>

            <div class="admin-login-shell pl-4 pr-4 pl-0-md pr-0-md">
                <div>
                    <div class="text-center" style="padding-top: 30px; padding-bottom: 40px;">
                        <img src="/uploads/images/logo-aha-trang.png" width="100" />
                    </div>

                    <div class="admin-login-form-wrap">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="Panel1" runat="server" DefaultButton="but_login">
                                    <div class="admin-login-input-group mt-0">
                                        <span class="admin-login-input-icon mif-location"></span>
                                        <asp:DropDownList ID="ddl_chinhanh" runat="server" CssClass="admin-login-input"></asp:DropDownList>
                                    </div>
                                    <div class="admin-login-input-group mt-4">
                                        <span class="admin-login-input-icon mif-user"></span>
                                        <asp:TextBox MaxLength="50" ID="txt_user" runat="server" CssClass="admin-login-input" placeholder="Tài khoản"></asp:TextBox>
                                    </div>
                                    <div class="admin-login-input-group mt-4">
                                        <span class="admin-login-input-icon mif-key"></span>
                                        <asp:TextBox MaxLength="50" TextMode="Password" ID="txt_pass" runat="server" CssClass="admin-login-input" placeholder="Mật khẩu"></asp:TextBox>
                                        <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện mật khẩu">
                                            <span class="aha-password-toggle-label">Hiện</span>
                                        </button>
                                    </div>
                                    <div class="admin-login-actions mt-5">
                                        <div class="admin-login-forgot">
                                            <small><a href="/gianhang/admin/quen-mat-khau/default.aspx" class="fg-white fg-light-hover">Quên mật khẩu?</a></small>
                                        </div>
                                        <div class="admin-login-submit-wrap">
                                            <asp:Button ID="but_login" runat="server" Text="ĐĂNG NHẬP" CssClass="button dark admin-login-submit" OnClick="but_login_Click" />
                                        </div>
                                    </div>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                            <ProgressTemplate>
                                <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                                    <div style="padding-top: 50vh;">
                                        <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                                    </div>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </div>
                </div>
            </div>
        </div>
    </form>
    <script src="/Metro-UI-CSS-master/js/metro.min.js"></script>
    <script src="/js/aha-ui-refresh.js?v=2026-03-07.2"></script>
    <%=notifi %>
</body>
</html>
