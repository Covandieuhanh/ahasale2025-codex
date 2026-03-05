<%@ Page Language="C#" AutoEventWireup="true" CodeFile="khoi-phuc-mat-khau.aspx.cs" Inherits="admin_khoi_phuc_mat_khau" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <%--Basic --%>
    <title>Đăng nhập hệ thống</title>
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
        .body-khoiphuc-bcorn {
            background-color: #00aba9; /*màu nhạt*/
            overflow: hidden
        }

        .bg-khoiphuc-bcorn {
            background: linear-gradient(#005e5d,#00aba9); /*đậm nhạt*/
        }
    </style>
</head>
<body class="body-khoiphuc-bcorn">

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True"></asp:ScriptManager>


        <div class="bg-khoiphuc-bcorn">

            <div class="fg-white text-center" style="padding-top: 50px">
                <div class="text-bold" style="font-size: 17px">
                    KHÔI PHỤC MẬT KHẨU
                </div>
                <%--<div>
                    Bạn chưa có tài khoản? 
                   <a href="tel:0842359155" class="fg-white fg-lightOrange" title="Nhấn để gọi"><span class="mif-phone pl-1"></span>Trợ giúp</a>
                </div>--%>
            </div>

            <div style="margin: 0 auto; max-width: 360px; z-index: 0;" class="pl-4 pr-4 pl-0-md pr-0-md">
                <div>
                    <div class="text-center" style="padding-top: 30px; padding-bottom: 40px;">
                        <span class="mif-replay mif-8x fg-white"></span>
                    </div>

                    <div>
                        <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                            <div class="text-center fg-white">
                                <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                <div class="mt-3"><a href="/admin/login.aspx" class="button small dark rounded">Quay lại trang đăng nhập</a></div>
                            </div>

                        </asp:PlaceHolder>

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" Visible="false">
                            <ContentTemplate>
                                <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
                                    <div class="mt-0 fg-white">
                                        <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div class="mt-5">
                                        <asp:TextBox MaxLength="50" TextMode="Password" ID="txt_pass" runat="server" data-role="input" data-prepend="<span class='mif-key'>" placeholder="Nhập mật khẩu mới"></asp:TextBox>
                                    </div>
                                    <div class="mt-5">
                                        <div style="float: left">
                                            <small><a href="/admin/login.aspx" class="fg-white fg-black-hover">Quay lại đăng nhập</a></small>
                                        </div>
                                        <div style="float: right">

                                            <asp:Button ID="Button1" runat="server" Text="ĐẶT LẠI MẬT KHẨU" CssClass="button dark ml-2 " OnClick="Button1_Click" />

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
    <script src="/js/aha-ui-refresh.js?v=2026-03-02.1"></script>

    <%=ViewState["thongbao"] %>
</body>
</html>
