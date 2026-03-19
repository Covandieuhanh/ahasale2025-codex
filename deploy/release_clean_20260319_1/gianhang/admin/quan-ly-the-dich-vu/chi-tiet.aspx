<%@ Page Title="Chi tiết thẻ dịch vụ" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="chi-tiet.aspx.cs" Inherits="badmin_Default" %><asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id='form_thanhtoan' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 600px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a class='fg-white d-inline c-pointer' onclick='show_hide_id_form_thanhtoan()' title='Đóng'>
                    <span class='mif mif-cross mif-lg fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Thanh toán</h5>
                <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel4" runat="server" DefaultButton="but_thanhtoan">
                            <div class="mt-7">
                                Ngày
                            </div>
                            <div>
                                <asp:TextBox ID="txt_ngaythanhtoan" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Hình thức giao dịch</label>
                                <asp:DropDownList ID="ddl_hinhthuc_thanhtoan" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Tiền mặt" Value="Tiền mặt"></asp:ListItem>
                                    <asp:ListItem Text="Chuyển khoản" Value="Chuyển khoản"></asp:ListItem>
                                    <asp:ListItem Text="Quẹt thẻ" Value="Quẹt thẻ"></asp:ListItem>
                                    <asp:ListItem Text="Voucher giấy" Value="Voucher giấy"></asp:ListItem>
                                    <asp:ListItem Text="E-Voucher (điểm)" Value="E-Voucher (điểm)"></asp:ListItem>
                                    <asp:ListItem Text="Ví điện tử" Value="Ví điện tử"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Nhập số tiền thanh toán</label>
                                <asp:TextBox ID="txt_sotien_thanhtoan_congno" data-role="input" runat="server" data-clear-button="true" onchange="format_sotien(this);"></asp:TextBox>
                            </div>

                            <div class="mt-6 mb-10">
                                <div style="float: left">
                                    <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                </div>
                                <div style="float: right" onclick='show_hide_id_form_thanhtoan()'>
                                    <asp:Button ID="but_thanhtoan" runat="server" Text="THANH TOÁN" CssClass="button success" OnClick="but_thanhtoan_Click" />
                                </div>
                                <div style="clear: both"></div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

        </div>
    </div>
    <script>
        function show_hide_id_form_thanhtoan() {
            var x = document.getElementById("form_thanhtoan");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>


    <div id="main-content" class="mb-10">
        <ul data-role="tabs" data-expand="true">
            <li><a href="#thongtinthe">Thông tin thẻ</a></li>
            <li><a href="#thanhtoan">Thanh toán</a></li>
            <li><a href="#nhatky">Nhật ký</a></li>
        </ul>
        <div class="border bd-default no-border-top p-2 pl-4 pr-4 ">
            <div id="thongtinthe">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" DefaultButton="Button3">
                            <div class="row">
                                <div class="cell-lg-6 pr-2-lg">
                                    <div class="mt-3">
                                        <label class="fw-600">Mã thẻ</label>
                                        <asp:TextBox ID="txt_mathe" runat="server" data-role="input" Text="" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Ngày bán</label>
                                        <asp:TextBox ID="txt_ngaytao" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Tên khách hàng</label>
                                        <asp:TextBox ID="txt_tenkhachhang" runat="server" data-role="input" OnTextChanged="txt_tenkhachhang_TextChanged" AutoPostBack="true"></asp:TextBox></div>
                                    <div class="mt-3">
                                        <label class="fw-600">Điện thoại</label>
                                        <%--<asp:TextBox ID="txt_sdt" runat="server" data-role="input" OnTextChanged="txt_sdt_TextChanged" AutoPostBack="true"></asp:TextBox>--%>
                                        <asp:TextBox ID="txt_sdt" runat="server" data-role="input" OnTextChanged="txt_sdt_TextChanged" AutoPostBack="true"></asp:TextBox></div>
                                    <%--<div class="mt-3">
                                        <label class="fw-600">Ngày sinh</label>
                                        <asp:TextBox ID="txt_ngaysinh" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                    </div>--%>
                                    <div class="mt-3">
                                        <label class="fw-600">Địa chỉ</label>
                                        <asp:TextBox ID="txt_diachi" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Ghi chú</label>
                                        <asp:TextBox ID="txt_ghichu" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="cell-lg-6 pl-2-lg">
                                    <div class="mt-3">
                                        <label class="fw-600">Tên thẻ</label>
                                        <asp:TextBox ID="txt_tenthe" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Dịch vụ thẻ</label>
                                        <asp:DropDownList ID="DropDownList1" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Số buổi</label>
                                        <asp:TextBox ID="txt_sobuoi" runat="server" data-role="input" Text="1"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Giá trị thẻ</label>
                                        <asp:TextBox ID="txt_giatrithe" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Chiết khấu thẻ</label>
                                        <span class="place-right">
                                            <asp:RadioButton ID="ck_phantram_giathe" runat="server" Text="%" GroupName="ck_the" Checked="true" />
                                            <asp:RadioButton ID="ck_tienmat_giathe" runat="server" Text="Tiền" GroupName="ck_the" />
                                        </span>
                                        <asp:TextBox ID="txt_chietkhau_giatrithe" data-role="input" runat="server" MaxLength="10" Text="0" onchange="format_sotien(this);"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Hạn sử dụng</label>
                                        <asp:TextBox ID="txt_hsd" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Nhân viên chốt sale</label>
                                        <asp:DropDownList ID="ddl_nhanvien_chotsale" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Chiết khấu NV chốt sale</label>
                                        <span class="place-right">
                                            <asp:RadioButton ID="ck_phantram_chotsale" runat="server" Text="%" GroupName="ck_hd" Checked="true" />
                                            <asp:RadioButton ID="ck_tienmat_chotsale" runat="server" Text="Tiền" GroupName="ck_hd" />
                                        </span>
                                        <asp:TextBox ID="txt_chietkhau_chotsale" data-role="input" runat="server" MaxLength="10" Text="0" onchange="format_sotien(this);"></asp:TextBox>
                                    </div>


                                </div>
                            </div>

                            <div class="mt-10 mb-20 text-center">
                                <asp:Button ID="Button3" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="Button3_Click" />
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanel2">
                    <ProgressTemplate>
                        <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                            <div style="padding-top: 50vh;">
                                <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                            </div>
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </div>
            <div id="thanhtoan">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="but_thanhtoan" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <div class="bg-white pl-2 pr-2">
                            <div>
                                <div style="float: left" class="">
                                    <ul class="h-menu ">
                                        <%if (bcorn_class.check_quyen(user, "q12_5") == "" || bcorn_class.check_quyen(user, "n12_5") == "")
                                            { %>
                                        <li data-role="hint" data-hint-position="top" data-hint-text="Thanh toán"><a class="button" onclick='show_hide_id_form_thanhtoan()'><span class="mif mif-paypal"></span></a></li>
                                        <%} %>
                                        <%if (bcorn_class.check_quyen(user, "q12_5") == ""|| bcorn_class.check_quyen(user, "n12_5") == "")
                                            { %>
                                        <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                        <li data-role="hint" data-hint-position="top" data-hint-text="Xóa thanh toán">
                                            <asp:ImageButton ID="but_xoathanhtoan" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" OnClick="but_xoathanhtoan_Click" />
                                        </li>
                                        <%} %>
                                    </ul>
                                </div>
                                <div style="float: right" class="">
                                    <ul class="h-menu ">
                                    </ul>
                                </div>
                                <div class="clr-float"></div>
                            </div>
                            <hr />
                            <table class="table row-hover striped subcompact mt-3">
                                <tbody>
                                    <asp:Repeater ID="Repeater2" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td style="width: 1px">
                                                    <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_lichsu_thanhtoan_<%#Eval("id").ToString() %>">
                                                </td>

                                                <%--<td style="width: 50px">Lần <%=stt_tt %></td>--%>
                                                <td class="text-right" style="width: 118px"><%#Eval("thoigian","{0:dd/MM/yyyy HH:mm}").ToString() %></td>
                                                <td style="min-width: 70px"><%#Eval("hinhthuc_thanhtoan").ToString() %></td>
                                                <td class="text-right" style="width: 1px"><%#Eval("sotienthanhtoan","{0:#,##0}").ToString() %></td>
                                            </tr>
                                            <%--<%stt_tt = stt_tt + 1; %>--%>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                            <div class="p-2 text-right">
                                <asp:Label ID="Label1" runat="server" Text=""></asp:Label>

                            </div>
                        </div>
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
            <div id="nhatky">
                <div style="overflow: auto" class=" mt-3 mb-3">
                    <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                        <thead>
                            <tr style="background-color: #f5f5f5">

                                <td class="text-bold" style="width: 1px; min-width: 1px">Mã</td>
                                <td class="text-bold" style="width: 100px;">Ngày SD</td>
                                <%--<td class="text-bold text-center" style="width: 50px; min-width: 50px">Ảnh</td>--%>
                                <td class="text-bold" style="min-width: 150px">Dịch vụ</td>


                                <td class="text-bold" style="min-width: 170px">Nhân viên làm dịch vụ</td>
                                <td class="text-bold" style="width: 170px; min-width: 170px">Đánh giá người làm</td>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="Repeater1" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="text-center text-bold">
                                            <div><%=id%></div>
                                        </td>
                                        <td class="text-right">
                                            <div><%#Eval("ngayban","{0:dd/MM/yyyy HH:mm}").ToString() %></div>
                                        </td>

                                        <td>

                                            <span class="fg-blue"><%#Eval("ten_dichvu_sanpham").ToString() %></span>


                                            <asp:PlaceHolder ID="PlaceHolder10" runat="server" Visible='<%#Eval("id_thedichvu")!=null%>'>
                                                <div><span class="data-wrapper"><code class="bg-orange fg-white">Thẻ dịch vụ</code></span></div>
                                            </asp:PlaceHolder>
                                        </td>

                                        <td>
                                            <div><%#Eval("tennguoilam")%></div>

                                            <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("phantramlam").ToString()=="0" %>'>
                                                <div class="text-right text-bold fg-emerald"><%#Eval("tongtien_lam","{0:#,##0}").ToString() %></div>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("phantramlam").ToString()!="0" %>'>
                                                <div class="text-right text-bold fg-emerald"><%#Eval("phantramlam")%>%</div>
                                            </asp:PlaceHolder>
                                        </td>
                                        <td>
                                            <asp:PlaceHolder ID="PlaceHolder9" runat="server" Visible='<%#Eval("kyhieu").ToString()=="dichvu" %>'>
                                                <%#Eval("danhgia_nguoilam_dichvu")%>
                                                <div>
                                                    <input data-role="rating" data-value="<%#Eval("danhgia_5sao_dv")%>" name="danhgia_5sao_dv_<%#Eval("id")%>" data-static="true">
                                                </div>
                                            </asp:PlaceHolder>
                                        </td>
                                    </tr>
                                    <%--  <%stt = stt + 1; %>--%>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>

                    </table>
                </div>
            </div>
        </div>

    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%--<%=notifi %>--%>
</asp:Content>

