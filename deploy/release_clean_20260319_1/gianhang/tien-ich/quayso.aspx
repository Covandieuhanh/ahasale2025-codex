<%@ Page Language="C#" AutoEventWireup="true" CodeFile="quayso.aspx.cs" Inherits="congcu_tienich_quayso" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Quay số ngẫu nhiên</title>
    <link href="/Metro-UI-CSS-master/tests/metro/css/metro-all.min.css" rel="stylesheet" />
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=yes" />
    <link rel="icon" href="/uploads/Images/favicon.png" type="image/x-icon" />
    <style>
        .title-1 {
            font-family: "Segoe UI",SegoeUI,"Helvetica Neue",Helvetica,Arial,sans-serif !important;
            font-weight: 600;
            font-size: 36px;
        }

        @media (max-width: 767px) { /*md*/
            .title-1 {
                font-size: 26px;
            }
        }
    </style>
</head>
<body class="h-vh-100" style="background-image: url('/uploads/Images/bg-quay-so.jpg'); background-position: center center; background-size: cover; background-repeat: no-repeat">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True"></asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Timer ID="Timer1" runat="server" OnTick="Timer1_Tick" Interval="1" Enabled="false"></asp:Timer>
                <div class="text-center">
                    <%--<a href="/" title="Trang chủ AppHalora">
                        <img src="/uploads/Images/logo-takara-trang.png" width="110" class="mt-10-md mt-5" /></a>--%>
                    <div class="text-bold fg-white title-1 mt-4 pl-3 pr-3">
                        QUAY SỐ NGẪU NHIÊN
                    </div>
                    <div class="text-bold mt-6 title-1">
                        <asp:Label ID="Label1" runat="server" Text="0" CssClass="fg-white button large alert rounded ani-flash text-bold"></asp:Label>
                    </div>
                </div>

            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
                <asp:AsyncPostBackTrigger ControlID="but_batdau" EventName="Click" />
               
            </Triggers>

        </asp:UpdatePanel>

        <div class="text-center mt-10">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="but_batdau" runat="server" Text="BẮT ĐẦU" OnClick="but_batdau_Click" CssClass="success large" />
                    <asp:Button ID="but_dunglai" runat="server" Text="DỪNG LẠI" OnClick="but_dunglai_Click" CssClass="warning large" Visible="false" />
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="mt-4">
                <asp:TextBox ID="txtNhanVien" runat="server" TextMode="MultiLine" Rows="13" Columns="40" placeholder="Nhập tên vào đây. Xuống hàng để thêm tên khác."></asp:TextBox>
            </div>
        </div>
        <div style="margin: 0 auto auto; max-width: 500px;" class="p-4">
        </div>
    </form>
    <script src="/Metro-UI-CSS-master/tests/metro/js/metro.min.js"></script>
    <script src="/js/bcorn.js"></script>
    <%=notifi %>
</body>
</html>
