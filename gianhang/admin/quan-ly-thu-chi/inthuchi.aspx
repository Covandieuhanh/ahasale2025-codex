<%@ Page Language="C#" AutoEventWireup="true" CodeFile="inthuchi.aspx.cs" Inherits="thong_ke_bao_cao_inbienban" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%=tenphieu %></title>
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
        .abc{padding-top:4px}
    </style>
</head>
<body style="margin: 0!important; padding: 0!important">
    <form id="form1" runat="server">
        <div style="/*height: 148mm; */ width: 210mm; font-family: "Be Vietnam Pro", "Segoe UI", Arial, sans-serif; margin: 0 auto; /*border: 1px solid red; */ /*overflow: hidden*" />
        <div style="padding: 0 10px" class="text-small-1">
            <%if (logo_hoadon != "")
                { %>
            <div style="float: left; width: 60px; padding-right: 14px">
                <img src="<%=logo_hoadon %>" width="60" />
            </div>
            <%} %>
            <div style="float: left; width: calc(100% - 96px);">
                <div style="font-weight: bold; margin-top: 6px; font-size: 17px">
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



        <div class="text-bold text-center">
            <div>Mã phiếu: <%=idtc %></div>
                <div>Loại thu chi: <%=nhom %></div>
            <div style="font-size: 26px;padding-top:6px" class="text-bold"><%=tenphieu %></div>
            <div>
                    <i><asp:Label ID="lb_ngaythuchi" runat="server" Text=""></asp:Label></i>
                </div>
        </div>

        <div class="row p-0 m-0" style="padding-top:8px">
            <div class="cell-12 mt-1-minus">
                <div class="abc">
                    <div style="float: left; min-width: 136px" class="text-bold">
                        Người tạo: 
                    </div>
                    <div style="float: left;">
                        <asp:Label ID="lb_hoten" runat="server" Text=""></asp:Label>
                    </div>
                    <div style="clear: both"></div>
                </div>
                <div class="abc">
                    <div style="float: left; min-width: 136px;" class="text-bold">
                        Nội dung:
                    </div>
                    <div style="float: left;">
                        <asp:Label ID="lb_noidung" runat="server" Text=""></asp:Label>
                    </div>
                    <div style="clear: both"></div>
                </div>
                <div class="abc">
                    <div style="float: left; min-width: 136px" class="text-bold">
                        Số tiền:
                    </div>
                    <div style="float: left;">
                        <asp:Label ID="lb_sotien" runat="server" Text=""></asp:Label>
                    </div>
                    <div style="clear: both"></div>
                </div>
                <div class="abc">
                    <div style="float: left; min-width: 136px" class="text-bold">
                        Số tiền bằng chữ:
                    </div>
                    <div style="float: left;">
                        <asp:Label ID="lb_tienbangchu" runat="server" Text=""></asp:Label>
                    </div>
                    <div style="clear: both"></div>
                </div>
                <div class="abc">
                    <div style="float: left; min-width: 136px" class="text-bold">
                        Người nhận tiền:
                    </div>
                    <div style="float: left;">
                        <asp:Label ID="lb_nguoinhan" runat="server" Text=""></asp:Label>
                    </div>
                    <div style="clear: both"></div>
                </div>
                <%-- <div class="mt-1">
                        <div style="float: left; min-width:130px" class="text-bold">
                            Kèm theo:
                        </div>
                        <div style="float: left;">
                            .................................chứng từ gốc.
                        </div>
                        <div style="clear: both"></div>
                    </div>--%>
            </div>
            
        </div>

        <div class=" text-right text-italic  mt-1-minus"style="padding-top:6px">
                Ngày .............. tháng .............. năm ..............
            </div>
        <div style="width: 100%;padding-top:12px">
            
            <%--<div style="width: 20%; float: left" class="text-center">
                    <div class="text-bold">Giám đốc</div>
                    <div class="text-italic">(Ký, họ tên, đóng dấu)</div>
                </div>--%>
            <%--<div style="width: 33.3%; float: left" class="text-center">
                    <div class="text-bold">Người nộp tiền</div>
                    <div class="text-italic">(Ký, họ tên)</div>
                </div>--%>
            <div style="width: 33.3%; float: left" class="text-center">
                <div class="text-bold">Người tạo phiếu</div>
                <div class="text-italic"><i>(Ký và ghi rõ họ tên)</i></div>
            </div>
            <div style="width: 33.3%; float: left" class="text-center">
                <div class="text-bold">Người nhận tiền</div>
                <div class="text-italic"><i>(Ký và ghi rõ họ tên)</i></div>
            </div>
            <div style="width: 33.3%; float: left" class="text-center">
                <div class="text-bold">Giám đốc cơ sở</div>
                <div class="text-italic"><i>(Ký và ghi rõ họ tên)</i></div>
            </div>
            <div style="clear: both"></div>
        </div>
        <%--<div style="margin-top:70px">
                Đã nhận đủ số tiền (viết bằng chữ):.............................................................................................................................................
            </div>--%>
       
    </form>
    <%=p %>
</body>
</html>
