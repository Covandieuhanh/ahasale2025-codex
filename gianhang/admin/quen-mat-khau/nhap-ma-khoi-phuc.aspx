<%@ Page Language="C#" AutoEventWireup="true" CodeFile="nhap-ma-khoi-phuc.aspx.cs" Inherits="admin_Default2" %>
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
    <title>Nhập mã khôi phục</title>

    <meta name="description" content="" />
    <meta property="og:title" content="Nhập mã khôi phục" />
    <meta property="og:image" content="/uploads/images/login/og-ma-khoi-phuc.jpg" />
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
<body class="body-ma-xac-nhan">
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
            <div class="bg-ma-xac-nhan">
                <div class="container-fluid pos-fixed fixed-top bg-darkCrimson z-1 fg-white" style="height: 50px; line-height: 50px;">
                    <div class="fg-white container bg-transparent pl-0 pr-0" data-role="app-bar" data-expand-point="lg">
                        <uc1:SpaceLauncher runat="server" ID="spaceLauncher" ButtonCssClass="app-bar-item" />
                        <a class="app-bar-item" href="/gianhang/admin/quen-mat-khau/default.aspx" title="Quay lại"><span class="mif-arrow-left mr-3"></span>Quay lại</a>
                        <%--<a class="app-bar-item" href="#">Quên mật khẩu</a>--%>
                        <div class="app-bar-container ml-auto">
                        <a class="app-bar-item" href="/gianhang/admin/login.aspx">Đăng nhập ngay</a>
                    </div>
                </div>
            </div>
            <%--<div class="container-fluid pos-fixed fixed-bottom bg-crimson text-center z-1 fg-white" style="height: 40px; line-height: 40px;">
            <small>Email trợ giúp: <a href="" class="fg-white fg-gray-hover">support@bcorn.net</a></small>
        </div>--%>
            <div style="margin: 0 auto; max-width: 390px; z-index: 0;" class="pl-4 pr-4 pl-0-md pr-0-md">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
                    <div>
                        <div class="text-center" style="padding-top: 120px;">
                            <div class='pb-2 fg-white'>
                                <%--<img src="/uploads/images/avt-bcorn.jpg" class="login-logo" />--%>
                                <%--<img src="/uploads/images/logo-bcorn-text-white.png" width="90" />--%>
                                <div class="login-title-big mt-2 mb-2"><span class="fg-white">NHẬP MÃ KHÔI PHỤC</span></div>
                                <span class="mif mif-mail-read mif-8x fg-white"></span>
                                <div class="login-title-big mt-2"><span class="fg-white">Bước 2</span></div>
                                <div>
                                    Nhập <b>mã khôi phục</b> được gửi về email<br />
                                    <b><%=email %></b>
                                </div>
                            </div>
                        </div>
                        <div style="padding-bottom: 120px">
                            <div class="mt-5">
                                <asp:TextBox ID="txt_makhoiphuc" runat="server" data-role="input" placeholder="Nhập mã khôi phục"></asp:TextBox><%--autocomplete="off" --%>
                            </div>
                            <div class="mt-5">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Button ID="Button1" runat="server" Text="XÁC NHẬN" CssClass="button fg-white bg-amber bg-darkAmber-hover" Width="100%" OnClick="Button1_Click" />
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
