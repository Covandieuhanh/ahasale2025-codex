<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="admin_Default2" %>
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
    <title>Quên mật khẩu</title>

    <meta name="description" content="" />
    <meta property="og:title" content="Quên mật khẩu" />
    <meta property="og:image" content="/uploads/images/login/og-quen-mat-khau.jpg" />
    <meta property="og:description" content="" />

    <asp:PlaceHolder runat="server">
        <%=meta %>
    </asp:PlaceHolder>
    <style>
        .body-quen-mat-khau {
            margin-top: 0;
            background-color: #7f1d1d;
        }

        .bg-quen-mat-khau {
            background: linear-gradient(140deg, #7f1d1d 0%, #c81e1e 55%, #ef4444 100%);
            min-height: 100vh;
        }

        .gh-quen-topbar {
            height: 56px;
            line-height: 56px;
            border-bottom: 1px solid rgba(255, 255, 255, 0.18);
            backdrop-filter: blur(8px);
        }

        .gh-quen-shell {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 96px 16px 80px;
        }

        .gh-quen-card {
            width: min(460px, 100%);
            background: rgba(255, 255, 255, 0.12);
            border: 1px solid rgba(255, 255, 255, 0.18);
            border-radius: 18px;
            padding: 28px 26px 30px;
            box-shadow: 0 24px 70px rgba(0, 0, 0, 0.25);
        }

        .gh-quen-step {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 4px 12px;
            border-radius: 999px;
            font-size: 12px;
            letter-spacing: 0.6px;
            text-transform: uppercase;
            background: rgba(255, 255, 255, 0.18);
            color: #fee2e2;
            margin-bottom: 12px;
        }

        .gh-quen-title {
            font-size: 24px;
            font-weight: 700;
            color: #ffffff;
            margin: 0 0 8px;
        }

        .gh-quen-desc {
            color: rgba(255, 255, 255, 0.82);
            font-size: 14px;
            line-height: 1.6;
            margin-bottom: 18px;
        }

        .gh-quen-input {
            margin-top: 12px;
        }

        .gh-quen-input .input input {
            border-radius: 12px !important;
            border-color: rgba(255, 255, 255, 0.35) !important;
            background: rgba(255, 255, 255, 0.12) !important;
            color: #ffffff !important;
        }

        .gh-quen-input .input input::placeholder {
            color: rgba(255, 255, 255, 0.7) !important;
        }

        .gh-quen-action {
            margin-top: 18px;
        }

        .gh-quen-action .button {
            height: 48px;
            font-weight: 700;
            letter-spacing: 0.4px;
            border-radius: 12px;
        }

        .gh-quen-support {
            margin-top: 16px;
            font-size: 13px;
            color: rgba(255, 255, 255, 0.75);
            text-align: center;
        }

        .gh-quen-support a {
            color: #ffffff;
            text-decoration: underline;
        }

        @media (max-width: 767px) {
            .gh-quen-shell {
                padding: 84px 12px 60px;
            }

            .gh-quen-card {
                padding: 22px 20px 24px;
            }

            .gh-quen-title {
                font-size: 20px;
            }
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
<body class="body-quen-mat-khau">
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
      window.fbAsyncInit = function() {
        FB.init({
          xfbml            : true,
          version          : 'v15.0'
        });
      };

      (function(d, s, id) {
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
        <div class="bg-quen-mat-khau">
            <div class="container-fluid pos-fixed fixed-top bg-darkRed z-1 fg-white gh-quen-topbar">
                <div class="fg-white container bg-transparent pl-0 pr-0" data-role="app-bar" data-expand-point="lg">
                    <uc1:SpaceLauncher runat="server" ID="spaceLauncher" ButtonCssClass="app-bar-item" />
                    <a class="app-bar-item" href="<%=LegacyLoginUrl %>" title="Quay lại"><span class="mif-arrow-left mr-3"></span>Quay lại đăng nhập</a>
                </div>
            </div>

            <div class="gh-quen-shell">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
                    <div class="gh-quen-card">
                        <div class="gh-quen-step">
                            <span class="mif mif-lock"></span>
                            Bước 1
                        </div>
                        <div class="gh-quen-title">Khôi phục mật khẩu</div>
                        <div class="gh-quen-desc">
                            Nhập <b>tên tài khoản</b> hoặc <b>email</b> đã đăng ký.
                            Hệ thống sẽ gửi mã khôi phục để bạn tiếp tục đặt lại mật khẩu.
                        </div>
                        <div class="gh-quen-input">
                            <asp:TextBox ID="txt_taikhoan_email" runat="server" data-role="input" placeholder="Nhập tài khoản hoặc Email"></asp:TextBox>
                        </div>
                        <div class="gh-quen-action">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button1" runat="server" Text="NHẬN MÃ KHÔI PHỤC" CssClass="button warning" Width="100%" OnClick="Button1_Click" />
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
                        <div class="gh-quen-support">
                            Cần hỗ trợ nhanh? <a href="mailto:support@ahasale.vn">support@ahasale.vn</a>
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
