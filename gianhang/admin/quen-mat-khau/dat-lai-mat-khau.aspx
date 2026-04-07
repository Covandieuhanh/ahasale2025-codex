<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dat-lai-mat-khau.aspx.cs" Inherits="admin_Default2" %>
<%@ Register Src="~/Uc/Shared/SpaceLauncher_uc.ascx" TagPrefix="uc1" TagName="SpaceLauncher" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <meta name="robots" content="noodp,index,follow" />
    <meta name="revisit-after" content="1 days" />
    <meta http-equiv="content-language" content="vi" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=yes" />

    <link href="/Metro-UI-CSS-master/tests/metro/css/metro-all.min.css" rel="stylesheet" />
    <link href="/css/fix-metro.css" rel="stylesheet" />
    <link href="/css/bcorn-with-metro.css" rel="stylesheet" />
    <link href="/css/login.css" rel="stylesheet" />

    <meta property="og:type" content="website" />
    <title>Đặt lại mật khẩu</title>

    <meta name="description" content="" />
    <meta property="og:title" content="Đặt lại mật khẩu" />
    <meta property="og:image" content="/uploads/images/login/og-dat-lai-mat-khau.jpg" />
    <meta property="og:description" content="" />

    <asp:PlaceHolder runat="server">
        <%=meta %>
    </asp:PlaceHolder>
    <style>
        .body-login {
            background-color: #7f1d1d;
        }

        .bg-login {
            background: linear-gradient(140deg, #7f1d1d 0%, #c81e1e 55%, #ef4444 100%);
        }

        .bg-darkRed,
        .bg-cyan,
        .bg-darkCyan {
            background: #b91c1c !important;
        }

        .button.warning {
            background: #c81e1e !important;
            border-color: #9f1239 !important;
            color: #ffffff !important;
        }

        .button.warning:hover {
            background: #b91c1c !important;
        }
    </style>
</head>
<body class="body-login">
    <%--fb chat hỗ trợ--%>
    <!-- Messenger Plugin chat Code -->
    <div id="fb-root"></div>

    <!-- Your Plugin chat code -->
    <div id="fb-customer-chat" class="fb-customerchat">
    </div>

    <script>
        var chatbox = document.getElementById('fb-customer-chat');
        chatbox.setAttribute("page_id", "104888439178698");
        chatbox.setAttribute("attribution", "biz_inbox");
    </script>

    <!-- Your SDK code -->
    <script>
        window.fbAsyncInit = function () {
            FB.init({
                xfbml: true,
                version: 'v15.0'
            });
        };

        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) return;
            js = d.createElement(s); js.id = id;
            js.src = 'https://connect.facebook.net/vi_VN/sdk/xfbml.customerchat.js';
            fjs.parentNode.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));
    </script>
    <%--fb chat hỗ trợ--%>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True"></asp:ScriptManager>
            <div class="bg-login">
                <div class="container-fluid pos-fixed fixed-top bg-darkCyan z-1 fg-white" style="height: 50px; line-height: 50px;">
                    <div class="fg-white container bg-transparent pl-0 pr-0" data-role="app-bar" data-expand-point="lg">
                        <uc1:SpaceLauncher runat="server" ID="spaceLauncher" ButtonCssClass="app-bar-item" />
                        <a class="app-bar-item" href="/gianhang/admin/quen-mat-khau/nhap-ma-khoi-phuc.aspx?user=<%=user %>" title="Quay lại"><span class="mif-arrow-left mr-3"></span>Quay lại</a>
                        <%--<a class="app-bar-item" href="#">Quên mật khẩu</a>--%>
                        <div class="app-bar-container ml-auto">
                        <a class="app-bar-item" href="<%=LegacyLoginUrl %>">Đăng nhập ngay</a>
                    </div>
                </div>
            </div>
            <%--<div class="container-fluid pos-fixed fixed-bottom bg-cyan text-center z-1 fg-white" style="height: 40px; line-height: 40px;">
            <small>Email trợ giúp: <a href="" class="fg-white fg-gray-hover">support@bcorn.net</a></small>
        </div>--%>
            <div style="margin: 0 auto; max-width: 390px; z-index: 0;" class="pl-4 pr-4 pl-0-md pr-0-md">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
                    <div>
                        <div class="text-center" style="padding-top: 120px;">
                            <div class='pb-2 fg-white'>
                                <%--<img src="/uploads/images/avt-bcorn.jpg" class="login-logo" />--%>
                                <%--<img src="/uploads/images/logo-bcorn-text-white.png" width="90" />--%>
                                <div class="login-title-big mt-2 mb-2"><span class="fg-white">ĐẶT LẠI MẬT KHẨU</span></div>
                                <span class="mif mif-lock mif-8x fg-white"></span>
                                <div class="login-title-big mt-2"><span class="fg-white">Bước 3</span></div>
                                <div>
                                    Đặt lại mật khẩu mới cho tài khoản của bạn.<br />
                                    Tài khoản: <b><%=user %></b>
                                </div>
                            </div>
                        </div>
                        <div style="padding-bottom: 120px">
                            <div class="mt-5">
                                <asp:TextBox TextMode="Password" ID="txt_pass" runat="server" data-role="input" data-prepend="<span class='mif-key'>" placeholder="Nhập mật khẩu mới"></asp:TextBox>
                            </div>
                            <div class="mt-5">
                                <asp:TextBox TextMode="Password" ID="txt_pass_1" runat="server" data-role="input" data-prepend="<span class='mif-key'>" placeholder="Nhập lại mật khẩu mới"></asp:TextBox>
                            </div>
                            <div class="mt-5">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Button ID="Button1" runat="server" Text="ĐẶT LẠI MẬT KHẨU" CssClass="button warning" Width="100%" OnClick="Button1_Click" />
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
                </asp:Panel>
            </div>
        </div>
    </form>
    <script src="/Metro-UI-CSS-master/tests/metro/js/metro.min.js"></script>
    <%=notifi %>
</body>
</html>
