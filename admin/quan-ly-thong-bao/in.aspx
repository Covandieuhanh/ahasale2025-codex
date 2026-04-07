<%@ Page Language="C#" AutoEventWireup="true" CodeFile="in.aspx.cs" Inherits="admin_quan_ly_thong_bao_in" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>In thử</title>
    <meta charset="UTF-8" />
    <meta http-equiv="content-language" content="vi" />
    <style>
        body {
            font-family: "Be Vietnam Pro", "Segoe UI", Arial, sans-serif !important;
            font-size: 16px !important;
        }

        @media print {
            .page-break {
                page-break-before: always; /* Luôn bắt đầu trang mới trước phần tử có class này */
            }

            .avoid-break {
                page-break-inside: avoid; /* Tránh ngắt trang giữa phần tử này */
            }
        }
    </style>
</head>
<body style="margin: 0; padding: 0">
    <form id="form1" runat="server">
        <div style="margin-top: 0mm; width: 210mm; margin: 0 auto">
            In theo nhu cầu của bạn
            <br />
            Liên hệ: 0842 359 155 (Bôn Bắp)

            <%--<div class="page-break"></div>--%>
            <%--Sử dụng class này để bắt đầu trang mới--%>
        </div>
    </form>
    <script>window.onload = function () { window.print(); };</script>
</body>
</html>
