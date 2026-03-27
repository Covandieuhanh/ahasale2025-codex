<%@ Page Language="C#" AutoEventWireup="true" CodeFile="hoa-don-dien-tu.aspx.cs" Inherits="gianhang_hoa_don_dien_tu" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Hóa đơn điện tử</title>

    <meta http-equiv="content-language" content="vi" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
  
    <link rel="shortcut icon" type="image/x-icon" href="/uploads/images/favicon.png" />
    <meta property="og:title" content="Hóa đơn điện tử" />
    <meta property="og:image" content="/uploads/images/hoa-don-dien-tu.jpg" />
    <meta name="description" content="" />
    <meta property="og:description" content="" />
    <asp:PlaceHolder runat="server">
        <%=meta %>
    </asp:PlaceHolder>
    <style>
        .text-big {
            font-size: 18px;
        }

        .text-small {
            font-size: 15px;
        }
        .text-small-1 {
            font-size: 14px;
        }

        .text-center {
            text-align: center;
        }

        .text-left {
            text-align: left;
        }

        .text-right {
            text-align: right;
        }

        .text-bold {
            font-weight: bold;
        }
    </style>
</head>
<body style="margin: 0!important; padding: 0!important">
    <form id="form1" runat="server">
        <div style="/*height: 148mm; */ width: 210mm; font-family: 'Times New Roman'; margin: 0 auto; /*border: 1px solid red; */ /*overflow: hidden*" />

        <div style="padding: 0 10px" class="text-small-1">
            <%if (logo_hoadon != "")
                { %>
            <div style="float: left; width: 60px; padding-right: 14px">
                <img src="<%=logo_hoadon %>" width="60" />
            </div>
            <%} %>
            <div style="float: left; width: calc(100% - 96px);">
                <div style="font-weight: bold; margin-top: 6px;font-size:17px">
                    <%=tencty %>
                </div>
                <div>
                    Địa chỉ: <%=diachi %>
                </div>
                <div>Điện thoại: <%=sdt %></div>
            </div>
            <div style="clear: both"></div>
        </div>

        <div style="width: 100%; height: 1px; border-top: 1px dashed black; margin-top: 6px; margin-bottom: 6px;"></div>
        <div class="text-small">
            Ngày xuất: <%=ngaytao %>
        </div>
        <div class="text-small">
            Người xuất: <%=nguoixuat %>
        </div>
        <div class="text-center text-bold text-big" style="margin-top: 0px">
            HÓA ĐƠN BÁN HÀNG<br />
            MÃ ĐƠN: <%=id %>
        </div>
        <div style="margin-top: 4px">
            Khách hàng: <%=ten_kh %>
        </div>
        <div>
            Điện thoại: <%=sdt_kh %>
        </div>
        <div>
            Địa chỉ: <%=diachi_kh %>
        </div>

        <table style="width: 100%; margin-top: 8px">
            <tr>
                <td class="text-bold">Dịch vụ - Sản phẩm</td>
                <td class="text-bold">Đơn giá</td>
                <td class="text-bold text-center">Số lượng</td>
                <td class="text-bold text-center">Chiết khấu</td>
                <td class="text-bold text-right">Thành tiền</td>
            </tr>
            <tr>
                <td colspan="5" style="width: 100%; height: 1px; border-top: 1px dashed black"></td>
            </tr>
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <tr>
                        <td><%#Eval("ten_dichvu_sanpham") %></td>
                        <td class=""><%#Eval("gia","{0:#,##0}") %></td>
                        <td class="text-center"><%#Eval("soluong") %></td>
                        <td class="text-center"><%#Eval("chietkhau") %></td>
                        <td class="text-right"><%#Eval("sauck","{0:#,##0}") %></td>
                    </tr>
                    <tr>
                        <td colspan="5" style="width: 100%; height: 1px; border-top: 1px dashed black"></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>

            <tr class="text-right">
                <td colspan="4">Tổng tiền:</td>
                <td><%=tongtien %></td>
            </tr>
            <tr class="text-right">
                <td colspan="4">Chiết khấu:</td>
                <td><%=ck %></td>
            </tr>
            <tr class="text-big text-bold text-right">
                <td colspan="3">Sau chiết khấu:</td>
                <td><%=sauck %></td>
            </tr>
            <tr class="text-right">
                <td colspan="4">Đã thanh toán:</td>
                <td><%=tien_dathanhtoan %></td>
            </tr>
            <tr class="text-right">
                <td colspan="4">Còn thiếu:</td>
                <td><%=tien_conlai %></td>
            </tr>
            <tr class="text-right">
                <td colspan="5">Tiền bằng chữ: <%=bangchu %> đồng.</td>
            </tr>
            <tr>
                <td colspan="5" style="width: 100%; height: 1px; border-top: 1px dashed black"></td>
            </tr>
        </table>
        <div><%=km1_ghichu %></div>
        <div class="text-center text-bold text-big" style="margin-top:4px">
            CÁM ƠN VÀ HẸN GẶP LẠI!
        </div>

    </form>
    <script src="/Pandora-master1.0.0/source/vendors/jquery/jquery-3.4.1.min.js"></script>
    <%=p %>
</body>
</html>
