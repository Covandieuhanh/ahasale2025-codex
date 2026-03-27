<%@ Page Language="C#" AutoEventWireup="true" CodeFile="1.aspx.cs" Inherits="dt" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PHÚT VÀNG BẮT MẠCH AI</title>
    <meta http-equiv="content-language" content="vi" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=yes" />
    <meta name="robots" content="noodp,index,follow" />
    <meta name="revisit-after" content="1 days" />
    <meta name="google" content="nositelinkssearchbox" />

    <%--css nguồn--%>
    <link href="/Metro-UI-CSS-master/css/metro-all.min.css" rel="stylesheet" />
    <link href="/Metro-UI-CSS-master/css/icons.css" rel="stylesheet" />
    <%--dành riêng cho trang admin--%>
    <link href="/assetscss/home-style.css" rel="stylesheet" />
    <%--viết thêm dựa vào metro--%>
    <link href="/assetscss/bcorn-with-metro.css" rel="stylesheet" />
    <%--sửa lại css metro theo ý mình--%>
    <link href="/assetscss/fix-metro.css" rel="stylesheet" />

    <asp:Literal ID="literal_fav_icon" runat="server"></asp:Literal>


    <style>
        body {
            background-image: url('/getdata/bg2.jpg');
            background-repeat: no-repeat;
            background-size: cover;
            background-position: center;
            background-attachment: fixed;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True"></asp:ScriptManager>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div style="max-width: 400px; margin: 0 auto" class="mt-0 mb-6 p-5">
                    <div class="text-center mb-6">
                        <div class="fg-white">HỘI THẢO</div>
                        <div style="font-size: 20px" class="text-bold fg-white">DẪN ĐẦU KỶ NGUYÊN CÔNG NGHỆ</div>
                        <div style="display: flex; justify-content: center; align-items: center; ">
                            <img src="/GETDATA/habio.png" width="150" />
                            <img src="/GETDATA/ahashine1.png" width="68" class="ml-4 mt-4-minus" />
                        </div>


                        <div style="font-size: 18px" class="text-bold fg-white mt-2-minus">PHÚT VÀNG BẮT MẠCH AI</div>
                        <%--<div class="fg-white">NHẬN BIẾT GỐC BỆNH</div>--%>
                    </div>

                    <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
                        <div class="mt-3">
                            <asp:TextBox ID="TextBox1" runat="server" data-role="input" data-prepend="<span class='mif-user'>" placeholder="Họ và tên"></asp:TextBox>
                        </div>
                        <div class="mt-3">
                            <asp:TextBox ID="TextBox2" runat="server" data-role="input" data-prepend="<span class='mif-phone'>" placeholder="Số điện thoại"></asp:TextBox>
                        </div>
                        <%--<div class="mt-3">
                            <asp:TextBox ID="TextBox3" runat="server" data-role="input" data-prepend="<span class='mif-info'>" placeholder="Số Zalo"></asp:TextBox>
                        </div>
                          <div class="mt-3">
                            <asp:TextBox ID="txt_ngaysinh" data-prepend="<span class='mif-calendar'>" placeholder="Ngày sinh" runat="server" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                        </div>
                          <div class="mt-3">
                            <asp:DropDownList ID="DropDownList2" data-role="select" runat="server" data-prepend="<span class='mif-female'>" data-filter="false">
                                <asp:ListItem Text="Giới tính Nữ" Value="Nữ"></asp:ListItem>
                                <asp:ListItem Text="Giới tính Nam" Value="Nam"></asp:ListItem>
                                <asp:ListItem Text="Khác" Value="Khác"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="mt-3">
                            <asp:DropDownList ID="DropDownList1" data-role="select" runat="server" data-prepend="<span class='mif-location'>"></asp:DropDownList>
                        </div>
                        --%>
                        <%-- <div class="mt-3">
                            <asp:RadioButton ID="RadioButton1" runat="server" GroupName="GioiTinh" Text="Nam" />
                             <asp:RadioButton ID="RadioButton2" runat="server" GroupName="GioiTinh" Text="Nữ" Checked />
                            </div>--%>
                        <%--<div class="mt-3">
                            <asp:TextBox ID="TextBox4" runat="server" data-role="input" data-prepend="<span class='mif-info'>" placeholder="Thông tin 2"></asp:TextBox>
                        </div>
                        <div class="mt-3">
                            <asp:TextBox ID="TextBox5" runat="server" data-role="input" data-prepend="<span class='mif-mail'>" placeholder="Email"></asp:TextBox>
                        </div>--%>
                        <div class="mt-3">
                            <asp:Button ID="Button1" runat="server" Text="ĐĂNG KÝ NGAY" OnClick="Button1_Click" CssClass="success w-100" />

                        </div>
                        <div class="fg-white text-center mt-3"><small>Nhận ngay <span style="font-size: 20px" class="text-bold">500 POINT</span> vào tài khoản AhaSale</small></div>
                        <div class="fg-white text-center "><small>và tham gia nhóm sau khi đăng ký thành công.</small></div>
                    </asp:Panel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
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
    <script src="/js/bcorn.js"></script>
</body>
</html>
