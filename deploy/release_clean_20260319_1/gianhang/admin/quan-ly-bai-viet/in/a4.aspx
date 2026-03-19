<%@ Page Language="C#" AutoEventWireup="true" CodeFile="a4.aspx.cs" Inherits="admin_quan_ly_menu_in_a4_doc" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Mẫu A4</title>
    <link href="/Metro-UI-CSS-master/tests/metro/css/metro-all.min.css" rel="stylesheet" />
    <style>
        .table.cell-border .td, .table.cell-border .th, .table.cell-border td, .table.cell-border th {
            border: 1px #000 solid !important;
        }
        body{
            font-size:14px
        }
    </style>
</head>
<body style="margin: 0!important; padding: 0!important">
    <form id="form1" runat="server">
        <div style="width: 210mm;  font-family: 'Times New Roman'; margin: 0 auto; /*border: 1px solid black; overflow: hidden*/" class="bg-white">
            <table class="table row-hover table-border cell-border <%--striped--%>  mt-2 ">
                <tbody>
                    <tr class="text-bold">
                        <td>Chức năng đang được cập nhật</td>
                    </tr>
                    <tr>
                        <td>Chức năng đang được cập nhật</td>
                    </tr>
                    <tr>
                        <td style="font-size:14px !important">Chức năng đang được cập nhật</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </form>
    <script>window.onload = function () {window.print();};</script>
</body>
</html>
