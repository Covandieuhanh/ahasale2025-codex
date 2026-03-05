<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="admin_Default2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <%--Basic --%>
    <title>ADMIN AHASALE.VN</title>
    <meta charset="UTF-8" />
    <meta http-equiv="content-language" content="vi" />
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=yes" />
    <%-- <meta name="robots" content="index, follow" />
    <meta name="revisit-after" content="1 days" />--%>
    <meta property="og:type" content="website" />

    <%--Open Graph Meta Tags --%>
    <%--
<meta property="og:title" content="Mô tả ngắn và hấp dẫn về trang web của bạn" />
<meta property="og:url" content="URL_trang_web_cua_ban" />
<meta property="og:image" content="URL_hinh_anh_1200x630" />
<meta property="og:description" content="Mô tả ngắn và hấp dẫn về nội dung của trang web của bạn." />

<title></title>
<meta name="description" content="Mô tả ngắn và hấp dẫn về nội dung của trang web của bạn." />
<link rel="canonical" href="https://bcorn.net" />--%>


    <%--Favicon & icon Mobile & meta--%>
    <asp:Literal ID="literal_fav_icon" runat="server"></asp:Literal>

    <%--css nguồn--%>
    <link href="/Metro-UI-CSS-master/css/metro-all.min.css" rel="stylesheet" />
    <%--dành riêng cho trang login--%>
    <link href="/css/login.css?v=2026-03-02.1" rel="stylesheet" />
    <link href="/css/aha-ui-refresh.css?v=2026-03-02.1" rel="stylesheet" />
    <%--viết thêm dựa vào metro--%>
    <link href="/css/bcorn-with-metro.css" rel="stylesheet" />
    <%--sửa lại css metro theo ý mình--%>
    <link href="/css/fix-metro.css" rel="stylesheet" />
    <!-- jquery nên để trước các js khác -->
    <%--<script src="/js/jquery-3.7.1.min.js"></script>--%>
    <style>
        .body-login-bcorn1 {
            background-color: #00d600; /*màu nhạt*/
            overflow: hidden
        }

        .bg-login-bcorn1 {
            background: linear-gradient(#008a00,#00d600); /*đậm nhạt*/
        }
    </style>

     <link rel='icon' href='/uploads/images/favicon.png' sizes='16x16' type='image/x-icon' />
    <link rel='icon' href='/uploads/images/favicon.png' sizes='32x32' type='image/x-icon' />
    <link rel='icon' href='/uploads/images/favicon.png' sizes='48x48' type='image/x-icon' />

    <!-- Apple Touch Icon -->
    <link rel='apple-touch-icon' href='/uploads/images/avatar/avt-aha.jpg' sizes='180x180' />
    <link rel='apple-touch-icon' href='/uploads/images/avatar/avt-aha.jpg' sizes='167x167' />
    <link rel='apple-touch-icon' href='/uploads/images/avatar/avt-aha.jpg' sizes='152x152' />
    <link rel='apple-touch-icon' href='/uploads/images/avatar/avt-aha.jpg' sizes='120x120' />

    <!-- Android Icons -->
    <link rel='icon' href='/uploads/images/avatar/avt-aha.jpg' sizes='192x192' />
<link rel='icon' href='/uploads/images/avatar/avt-aha.jpg' sizes='144x144' />
    <meta property='og:title' content='ADMIN AHASALE.VN' />
    <meta property='og:image' content='/uploads/images/config/ac087060-8cf4-4fd7-bf20-a5fff4677952.jpg' />


</head>
<body class="body-login-bcorn1">

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True"></asp:ScriptManager>


        <asp:UpdatePanel ID="up_khoiphuc" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="pn_khoiphuc" runat="server" Visible="false" DefaultButton="but_nhanma">
                    <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                        <div style='top: 0; left: 0px; margin: 0 auto; max-width: 500px; opacity: 1;'>
                            <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                                <a href='#' class='fg-white d-inline' id="close_add" runat="server" onserverclick="but_close_form_quenmk_Click" title='Đóng'>
                                    <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                                </a>
                            </div>
                            <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                                <div class="pt-4 text-upper text-bold">
                                    KHÔI PHỤC MẬT KHẨU
                                </div>
                                <hr />
                            </div>
                        </div>
                    </div>
                    <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                        <div style='top: 0; left: 0; margin: 0 auto; max-width: 506px; opacity: 1;'>
                            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                                <%--pl-4 pl-8-md pr-8-md pr-4--%>
                                <div class="row">
                                    <div class="cell-lg-12">
                                        <div class="mt-3">
                                            <label class="fg-red fw-600">Nhập email khôi phục của bạn</label>
                                            <asp:TextBox ID="txt_email_khoiphuc" runat="server" CssClass="input" MaxLength="100"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="mt-6 mb-20 text-right">
                                    <asp:Button ID="but_nhanma" runat="server" Text="Nhận mã khôi phục" CssClass="button success" OnClick="but_nhanma_Click" />
                                </div>
                                <div class="mb-20"></div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="up_khoiphuc">
            <ProgressTemplate>
                <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                    <div style="padding-top: 45vh;">
                        <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>


        <div class="bg-login-bcorn1">

            <div class="fg-white text-center" style="padding-top: 50px">
                <div class="text-bold" style="font-size: 17px">
                    ADMIN AHASALE.VN
                </div>
                <%--<div>
                    Bạn chưa có tài khoản? 
                   <a href="tel:0842359155" class="fg-white fg-lightOrange" title="Nhấn để gọi"><span class="mif-phone pl-1"></span>Trợ giúp</a>
                </div>--%>
            </div>

            <div style="margin: 0 auto; max-width: 360px; z-index: 0;" class="pl-4 pr-4 pl-0-md pr-0-md">
                <div>
                    <div class="text-center" style="padding-top: 30px; padding-bottom: 40px;">
                        <img src="/uploads/images/logo-aha-trang.png" width="100" />
                    </div>

                    <div>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="Panel1" runat="server" DefaultButton="but_login">
                                    <div class="mt-0">
                                        <asp:TextBox MaxLength="50" ID="txt_user" runat="server" data-role="input" data-prepend="<span class='mif-user'>" placeholder="Tài khoản"></asp:TextBox><%--autocomplete="off" --%>
                                    </div>
                                    <div class="mt-5">
                                        <asp:TextBox MaxLength="50" TextMode="Password" ID="txt_pass" runat="server" data-role="input" data-prepend="<span class='mif-key'>" placeholder="Mật khẩu"></asp:TextBox>
                                    </div>
                                    <div class="mt-5">
                                        <div style="float: left">
                                            <small>
                                                <asp:LinkButton ID="but_show_form_quenmk" OnClick="but_show_form_quenmk_Click" CssClass="fg-white fg-light-hover" runat="server">Quên mật khẩu?</asp:LinkButton>
                                            </small>
                                        </div>
                                        <div style="float: right">

                                            <asp:Button ID="but_login" runat="server" Text="ĐĂNG NHẬP" CssClass="button dark ml-2 " OnClick="but_login_Click" />

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
                <%-- <div class="mb-20 pb-10"></div>--%>
                <%--<div class="border bd-default border-left-none border-right-none border-bottom-none mb-20 mt-10 text-center fg-white">
                <small class="mt-4">Hỗ trợ kỹ thuật: 0842 359 155</small>
            </div>--%>
            </div>
        </div>
    </form>
    <script src="/Metro-UI-CSS-master/js/metro.min.js"></script>
    <script src="/js/aha-ui-refresh.js?v=2026-03-02.2"></script>

    <%=ViewState["thongbao"] %>
</body>
</html>
