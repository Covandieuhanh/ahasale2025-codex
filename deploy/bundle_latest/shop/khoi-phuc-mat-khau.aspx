<%@ Page Language="C#" AutoEventWireup="true" CodeFile="khoi-phuc-mat-khau.aspx.cs" Inherits="shop_khoi_phuc_mat_khau" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <%--Basic --%>
    <title>Khôi phục mật khẩu gian hàng đối tác</title>
    <meta charset="UTF-8" />
    <meta http-equiv="content-language" content="vi" />
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=yes" />
    <meta property="og:type" content="website" />

    <%--Favicon & icon Mobile & meta--%>
    <asp:Literal ID="literal_fav_icon" runat="server"></asp:Literal>

    <%--css nguồn--%>
    <link href="/Metro-UI-CSS-master/css/metro-all.min.css" rel="stylesheet" />
    <%--dành riêng cho trang login--%>
    <link href="/assetscss/login.css?v=2026-03-02.1" rel="stylesheet" />
    <link href="/assetscss/aha-ui-refresh.css?v=2026-03-02.1" rel="stylesheet" />
    <%--viết thêm dựa vào metro--%>
    <link href="/assetscss/bcorn-with-metro.css" rel="stylesheet" />
    <%--sửa lại css metro theo ý mình--%>
    <link href="/assetscss/fix-metro.css" rel="stylesheet" />
    <style>
        .body-khoiphuc-bcorn {
            background-color: #ee4d2d;
            overflow: hidden
        }

        .bg-khoiphuc-bcorn {
            background: linear-gradient(#c63c1d,#ee4d2d);
        }
    </style>
</head>
<body class="body-khoiphuc-bcorn">

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True"></asp:ScriptManager>


        <div class="bg-khoiphuc-bcorn">

            <div class="fg-white text-center" style="padding-top: 50px">
                <div class="text-bold" style="font-size: 17px">
                    KHÔI PHỤC MẬT KHẨU GIAN HÀNG ĐỐI TÁC
                </div>
            </div>

            <div style="margin: 0 auto; max-width: 360px; z-index: 0;" class="pl-4 pr-4 pl-0-md pr-0-md">
                <div>
                    <div class="text-center" style="padding-top: 30px; padding-bottom: 40px;">
                        <span class="mif-replay mif-8x fg-white"></span>
                    </div>

                    <div>
                        <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="true">
                            <div class="text-center fg-white">
                                <asp:Label ID="Label2" runat="server" Text="Trang khôi phục bằng link đã được thay thế bằng OTP qua email."></asp:Label>
                                <div class="mt-3"><a href="/shop/login.aspx" class="button small dark rounded">Quay lại để nhận OTP</a></div>
                            </div>

                        </asp:PlaceHolder>

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" Visible="false">
                            <ContentTemplate>
                                <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
                                    <div class="mt-0 fg-white">
                                        <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div class="mt-5">
                                        <div class="aha-password-field">
                                            <asp:TextBox MaxLength="50" TextMode="Password" ID="txt_pass" runat="server" data-role="input" data-prepend="<span class='mif-key'>" placeholder="Nhập mật khẩu mới"></asp:TextBox>
                                            <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện mật khẩu">
                                                <span class="aha-password-toggle-label">Hiện</span>
                                            </button>
                                        </div>
                                    </div>
                                    <div class="mt-5">
                                        <div style="float: left">
                                            <small><a href="/shop/login.aspx" class="fg-white fg-black-hover">Quay lại đăng nhập</a></small>
                                        </div>
                                        <div style="float: right">

                                            <asp:Button ID="Button1" runat="server" Text="ĐẶT LẠI MẬT KHẨU" CssClass="button dark ml-2 " OnClientClick="return AhaPreventDoubleClick(this);" OnClick="Button1_Click" />

                                        </div>
                                        <div style="clear: both"></div>
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
    <script>
        window.AhaPreventDoubleClick = window.AhaPreventDoubleClick || function (btn) {
            try {
                if (!btn) return true;
                if (btn.dataset && btn.dataset.locked === "1") return false;
                if (btn.dataset) btn.dataset.locked = "1";
                btn.disabled = true;
                btn.classList.add("aha-btn-loading");
                return true;
            } catch (e) { return true; }
        };
    </script>

    <%=ViewState["thongbao"] %>
</body>
</html>
