<%@ page language="C#" autoeventwireup="true" inherits="home_cho_thanh_toan, App_Web_ofdqyxp3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AHASALE.VN</title>
    <meta charset="UTF-8" />
    <meta http-equiv="content-language" content="vi" />
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=yes" />
    <%--css nguồn--%>
    <link href="/Metro-UI-CSS-master/css/metro-all.min.css" rel="stylesheet" />
    <link href="/css/home-style.css" rel="stylesheet" />
    <%--dành riêng cho trang login--%>
    <link href="/css/login.css" rel="stylesheet" />
    <%--viết thêm dựa vào metro--%>
    <link href="/css/bcorn-with-metro.css" rel="stylesheet" />
    <%--sửa lại css metro theo ý mình--%>
    <link href="/css/fix-metro.css" rel="stylesheet" />
    <style>
        .body-login-bcorn1 {
            background-color: #1e2329; /*màu nhạt*/
            overflow: hidden
        }

        .bg-login-bcorn1 {
            background: linear-gradient(#181a20,#1e2329); /*đậm nhạt*/
        }

        .fg-ahasale {
            color: #eaecef; /*trắng*/
        }

        .fg-ahasale1 {
            color: #848e9c; /*xám xanh*/
        }

        .bg-ahasale1 {
            background-color: #1e2329; /*nền tối nhạt*/
        }

        .bg-ahasale {
            background-color: #181a20; /*nền tối đậm*/
        }
    </style>
</head>
<body class="body-login-bcorn1">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True"></asp:ScriptManager>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Timer ID="Timer1" runat="server" Interval="5000" OnTick="Timer1_Tick"></asp:Timer>
                <div class="bg-login-bcorn1">
                    <div class="fg-white text-center">
                        <div class="text-bold" style="font-size: 16px">
                            <div class="text-center" style="padding-top: 40px; padding-bottom: 20px;">
                                <img src="/uploads/images/logo-aha-trang.png" width="70" />
                            </div>
                            <div>
                                CHỜ Trao đổi ĐƠN HÀNG SỐ
                                <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                            </div>
                            <div><small class="fg-yellow">Quy đổi giá trị tương đương 1000 Quyền tiêu dùng = 1000 VNĐ</small></div>
                        </div>
                        <%--<div>
           Bạn chưa có tài khoản? 
          <a href="tel:0842359155" class="fg-white fg-lightOrange" title="Nhấn để gọi"><span class="mif-phone pl-1"></span>Trợ giúp</a>
       </div>--%>
                    </div>
                    <div style="margin: 0 auto; max-width: 420px; z-index: 0;" class="pl-4 pr-4 pl-0-md pr-0-md">

                        

                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="false">
                            <div class="bg-ahasale fg-ahasale text-center mt-4 p-4" style="border-radius: 20px">
                                <div>Vui lòng chạm thẻ khách hàng để Trao đổi.</div>
                                <div class="mt-1 mb-1">
                                    <img src="/uploads/images/dong-a.png" width="20" />
                                    <div class="button mini light rounded text-bold">
                                        <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="mt-2">
                                    <asp:Button ID="Button1" OnClick="Button1_Click" runat="server" Text="Hủy chờ Trao đổi" CssClass="small alert rounded" />
                                </div>
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="false">
                            <div class="bg-ahasale fg-ahasale text-center mt-4 p-4" style="border-radius: 20px">
                                <div>
                                    Xin chào,
                                    <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                                </div>
                                <div class="mt-1 mb-1">
                                    <small>Bạn sắp trao đổi 
                                        <span class="button mini bg-amber fg-black bg-darkAmber-hover rounded text-bold">
                                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                        </span>
                                        Quyền tiêu dùng với
                                        <asp:Label ID="lb_tenshop" runat="server" Text=""></asp:Label>
                                    </small>
                                </div>
                                <div class="mb-1 fg-red">
                                    <small>Nếu không phải là bạn, vui lòng nhấn nút <b>Hủy</b></small>
                                </div>
                                <div class="mt-2">
                                    <asp:TextBox MaxLength="50" TextMode="Password" ID="txt_mapin" runat="server" data-role="input" data-prepend="<span class='mif-key'>" placeholder="Nhập mã pin để hoàn tất Trao đổi"></asp:TextBox>
                                </div>
                                <div class="mt-2  d-flex flex-justify-between flex-equal-item">
                                    <asp:Button ID="Button2" OnClick="Button2_Click" runat="server" Text="Hủy" CssClass=" alert mr-1 flat-button" Width="100%" />
                                    <asp:Button ID="Button3" OnClick="Button3_Click" runat="server" Text="Trao đổi" CssClass="  bg-amber fg-black bg-darkAmber-hover ml-1 flat-button" Width="100%" />
                                </div>
                            </div>
                        </asp:PlaceHolder>

                        <div class="bg-ahasale fg-ahasale text-left mt-4 p-4" style="border-radius: 20px">
    <div class="mb-1">Chi tiết đơn hàng</div>
    <div class="bcorn-fix-title-table-container">
        <table class="bcorn-fix-title-table" style="width:100%!important">
            <thead>
                <tr class="">
                    <th style="width: 1px;">ID</th>
                    <th style="width: 1px;">Ảnh</th>
                    <th class="text-left" style="min-width: 150px;">Sản phẩm</th>
                    <th style="min-width: 1px;">SL</th>
                    <th style="min-width: 1px;">Tổng</th>
                </tr>
            </thead>

            <tbody>
                <asp:Repeater ID="Repeater2" runat="server">
                    <ItemTemplate>
                        <tr class=" fg-black">
                            <td class="text-center">
                                <%# Eval("id") %>
                            </td>
                            <td class="text-center">
                                <div data-role="lightbox" class="c-pointer">
                                    <img src="<%# Eval("image") %>" class="img-cover-vuong" width="60" height="60" />
                                </div>
                            </td>
                            <td class="text-left">
                                <%#Eval("name") %>
                                <div><%#Eval("giaban","{0:#,##0}") %><img src="/uploads/images/dong-a.png" style="width: 20px!important" class="pl-1" /></div>
                            </td>
                           
                            <td>
                                <%#Eval("soluong") %>
                            </td>
                            <td class="text-right"><%#Eval("thanhtien","{0:#,##0}") %><img src="/uploads/images/dong-a.png" style="width: 20px!important" class="pl-1" /></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>
</div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
            <ProgressTemplate>
                <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                    <div style="padding-top: 45vh;">
                        <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </form>
    <script src="/Metro-UI-CSS-master/js/metro.min.js"></script>
</body>
</html>
