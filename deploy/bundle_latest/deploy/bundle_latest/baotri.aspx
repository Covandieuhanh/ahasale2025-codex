<%@ Page Language="C#" AutoEventWireup="true" CodeFile="baotri.aspx.cs" Inherits="admin_Default2" %>

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
    <title>Bảo trì hệ thống</title>

    <meta name="description" content="Vui lòng quay lại sau" />
    <meta property="og:title" content="Bảo trì hệ thống" />
    <meta property="og:image" content="" />
    <meta property="og:description" content="Vui lòng quay lại sau" />

    <asp:PlaceHolder runat="server">
        <%=meta %>
    </asp:PlaceHolder>
</head>
<body class="body-bao-tri">
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
            <div class="container-fluid pos-fixed fixed-top bg-darkGrayBlue text-center z-1 fg-white" style="height: 50px; line-height: 50px;">
                <div class="text-bold">BẢO TRÌ HỆ THỐNG</div>
            </div>
            <%--<div class="container-fluid pos-fixed fixed-bottom bg-grayBlue text-center z-1 fg-white" style="height: 40px; line-height: 40px;">
            <small>Email trợ giúp: <a href="" class="fg-white fg-gray-hover">support@bcorn.net</a></small>
        </div>--%>

            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
                </Triggers>
            </asp:UpdatePanel>
            <asp:Timer ID="Timer1" runat="server" OnTick="Timer1_Tick" Interval="1000"></asp:Timer>
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                <ProgressTemplate>
                    <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                        <div style="padding-top: 45vh;">
                            <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                        </div>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <div style="margin: 0 auto; max-width: 390px; z-index: 0;" class="pl-4 pr-4 pl-0-md pr-0-md">
                <div>
                    <div class="text-center" style="padding-top: 70px;">
                        <div class='pb-2'>
                            <%--<img src="/uploads/images/avt-bcorn.jpg" class="login-logo" />--%>
                            <%--<img src="/uploads/images/logo-bcorn-text-white.png" width="90" />--%>
                            <span class="mif mif-tools mif-9x fg-white"></span>
                        </div>
                        <%--<div class="login-title-big mt-2"><span class="fg-white">LOGIN TO ADMIN</span></div>--%>
                    </div>

                    <div style="padding-bottom: 70px" class="text-center fg-white">
                        <div class="mt-5 login-title-big fg-yellow">
                            VUI LÒNG QUAY LẠI SAU
                        </div>
                        <div data-role="countdown" data-seconds="<%=tongsogiay %>" data-animate="slide" data-locale="vi-VN" style="font-size: 32px"></div>
                    </div>
                </div>

            </div>
        </div>
    </form>
    <script src="/Metro-UI-CSS-master/tests/metro/js/metro.min.js"></script>
    <%=notifi %>
</body>
</html>
