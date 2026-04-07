<%@ Page Language="C#" AutoEventWireup="true" CodeFile="in-bill.aspx.cs" Inherits="gianhang_hoa_don_in_bill_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <%--<link href="/Metro-UI-CSS-master4.5.0/tests/metro/css/metro-all.min.css" rel="stylesheet" />--%>
    <style>
        .text-big {
            font-size: 15px;
        }

        .text-small {
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
        <div style="/*height: 148mm; */ width: 58mm; font-family: "Be Vietnam Pro", "Segoe UI", Arial, sans-serif; margin: 0 auto; /*border: 1px solid red; */ /*overflow: hidden*/" class="bg-white text-small">
            
            <div class="text-center text-bold text-big">
                <%if(logo_hoadon!=""){ %>
                <img src="<%=logo_hoadon %>" width="66" style="margin-bottom:6px" />
                <%} %>
                <div>
                <%=tencty %></div>
            </div>
            <div class="text-small" style="margin-bottom:4px">
                Đ/C: <%=diachi %>
                <div>ĐT: <%=sdt %></div>
            </div>
            <div  style="width: 100%; height: 1px; border-top: 1px dashed black;padding-top:4px;"></div>
            <div class="text-small">
                Ngày xuất: <%=ngaytao %>
            </div>
            <div class="text-small">
                Người xuất: <%=nguoixuat %>
            </div>
            <div class="text-center text-bold text-big" style="margin:4px">
                HÓA ĐƠN BÁN HÀNG<br />
                MÃ ĐƠN: <%=id %>
            </div>
            <div>
                Khách hàng: <%=ten_kh %>
            </div>
            <div>
                Điện thoại: <%=sdt_kh %>
            </div>
            <div>
                Địa chỉ: <%=diachi_kh %>
            </div>

            <table style="width: 100%" class="mt-2">
                <tr>
                    <td class="text-bold">Đơn giá</td>
                    <td class="text-bold text-center">SL</td>
                    <td class="text-bold text-center">CK<br />
                        (%)</td>
                    <td class="text-bold text-right">Thành<br />
                        tiền</td>
                </tr>
                <tr>
                    <td colspan="4" style="width: 100%; height: 1px; border-top: 1px dashed black"></td>
                </tr>
                <asp:Repeater ID="Repeater1" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td colspan="4"><%#Eval("ten_dichvu_sanpham") %></td>
                        </tr>
                        <tr>
                            <td class=""><%#Eval("gia","{0:#,##0}") %></td>
                            <td class="text-center"><%#Eval("soluong") %></td>
                            <td class="text-center"><%#Eval("chietkhau") %></td>
                            <td class="text-right"><%#Eval("sauck","{0:#,##0}") %></td>
                        </tr>
                        <tr>
                            <td colspan="4" style="width: 100%; height: 1px; border-top: 1px dashed black"></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                
                <tr class="text-right">
                    <td colspan="3">Tổng tiền:</td>
                    <td><%=tongtien %></td>
                </tr>
                <tr class="text-right">
                    <td colspan="3">Chiết khấu:</td>
                    <td><%=ck %></td>
                </tr>
                <tr class="text-big text-bold text-right">
                    <td colspan="3">Sau chiết khấu:</td>
                    <td><%=sauck %></td>
                </tr>
                <tr class="text-right">
                    <td colspan="3">Đã thanh toán:</td>
                    <td><%=tien_dathanhtoan %></td>
                </tr>
                <tr class="text-right">
                    <td colspan="3">Còn thiếu:</td>
                    <td><%=tien_conlai %></td>
                </tr>
                <tr class="text-right">
                    <td colspan="4">Tiền bằng chữ: <%=bangchu %> đồng.</td>
                </tr>
                <tr>
                    <td colspan="4" style="width: 100%; height: 1px; border-top: 1px dashed black"></td>
                </tr>                
            </table>
            <div><%=km1_ghichu %></div>
            <div class="text-center text-bold text-big mt-2">
                CÁM ƠN VÀ HẸN GẶP LẠI!
            </div>
        </div>
    </form>
    <script src="/Pandora-master1.0.0/source/vendors/jquery/jquery-3.4.1.min.js"></script>
    <%=p %>
</body>
</html>
