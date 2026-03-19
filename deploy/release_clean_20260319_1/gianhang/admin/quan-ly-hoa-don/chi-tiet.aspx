<%@ Page Title="Chi tiết hóa đơn" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="chi-tiet.aspx.cs" Inherits="badmin_Default" %><asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .info-hoadon {
            font-weight: 600
        }

        .text-size-20 {
            font-size: 20px !important
        }

        .booking-flow-box {
            border: 1px solid #dbe7e5;
            border-radius: 14px;
            background: #f7fbfa;
            padding: 16px 18px;
        }

        .booking-flow-box__title {
            font-size: 16px;
            font-weight: 700;
            color: #145c54;
        }

        .booking-flow-box__meta {
            color: #4d6661;
            line-height: 1.7;
        }

        .booking-flow-box__hint {
            font-size: 13px;
            color: #4d6661;
        }
    </style>

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

    <%if (bcorn_class.check_quyen(user, "q7_3") == ""||bcorn_class.check_quyen(user, "n7_3") == "")
        { %>
    <div id='form_edithoadon' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 600px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a class='fg-white d-inline c-pointer' onclick='show_hide_id_form_edithoadon()' title='Đóng'>
                    <span class='mif mif-cross mif-lg fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Chỉnh sửa hóa đơn</h5>
                <hr />
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel3" runat="server" DefaultButton="but_form_edithoadon">
                            <div class="mt-0 fw-600">
                                Ngày
                            </div>
                            <div>
                                <asp:TextBox ID="txt_ngaytao" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                            </div>
                            <div class="mt-3 fw-600">Tên khách hàng</div>
                            <div>
                                <asp:TextBox ID="txt_tenkhachhang" runat="server" data-role="input" Width="100%"></asp:TextBox>
                            </div>
                            <div class="mt-3 fw-600">Điện thoại</div>
                            <div>
                                <asp:TextBox ID="txt_sdt" runat="server" data-role="input" Width="100%"></asp:TextBox>
                            </div>
                            <div class="mt-3 fw-600">Địa chỉ</div>
                            <div>
                                <asp:TextBox ID="txt_diachi" runat="server" data-role="textarea" Width="100%" TextMode="MultiLine"></asp:TextBox>
                            </div>
                            <div class="mt-3 fw-600">Ghi chú</div>
                            <div>
                                <asp:TextBox ID="txt_ghichu" runat="server" data-role="input" Width="100%"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Chiết khấu hóa đơn</label>
                                <span class="place-right">
                                    <asp:RadioButton ID="ck_hd_phantram" runat="server" Text="%" GroupName="ck_hd" />
                                    <asp:RadioButton ID="ck_hd_tienmat" runat="server" Text="Tiền" GroupName="ck_hd" />
                                </span>
                                <asp:TextBox ID="txt_chietkhau_hoadon" data-role="input" runat="server" MaxLength="13" Text="0" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                            </div>
                            <div class="mt-6 mb-10">
                                <div style="float: left">
                                    <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="but_form_edithoadon" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="but_form_edithoadon_Click" />
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
        function show_hide_id_form_edithoadon() {
            var x = document.getElementById("form_edithoadon");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>
    <%} %>

    <div id='form_themdvsp' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: <%=show_add%>; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 1110px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_themdvsp()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8 pb-8">
                <h5>Thêm dịch vụ / sản phẩm</h5>
                <ul data-role="tabs" data-expand="true">
                    <li><a href="#themdichvu">Dịch vụ</a></li>
                    <li><a href="#themsanpham">Sản phẩm</a></li>
                    <li><a href="#thedichvu">Thẻ dịch vụ</a></li>
                </ul>

                <div class="border bd-default no-border-top p-2 pl-4 pr-4 ">
                    <div id="themdichvu">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="Panel1" runat="server" DefaultButton="but_form_themdichvu">
                                    <div class="row ">
                                        <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                            <div class="">
                                                <label class="fw-600">Ngày bán</label>
                                                <asp:TextBox ID="txt_ngayban" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>

                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Dịch vụ</label>
                                                <%--<asp:DropDownList ID="ddl_dichvu" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_dichvu_SelectedIndexChanged"></asp:DropDownList>--%>
                                                <asp:TextBox ID="txt_tendichvu" runat="server" data-role="input" placeholder="Nhập và chọn tên dịch vụ" OnTextChanged="txt_tendichvu_TextChanged" AutoPostBack="true"></asp:TextBox></div>
                                            <div class="mt-3">
                                                <label class="fw-600">Giá</label>
                                                <asp:TextBox ID="txt_gia" MaxLength="13" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Số lượng</label>
                                                <asp:TextBox ID="txt_soluong" data-role="input" runat="server" MaxLength="5" Text="1"></asp:TextBox><%--autocomplete="off" --%>
                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Chiết khấu dịch vụ</label>
                                                <span class="place-right">
                                                    <asp:RadioButton ID="ck_dv_phantram" runat="server" Text="%" GroupName="ck_dv" Checked="true" />
                                                    <asp:RadioButton ID="ck_dv_tienmat" runat="server" Text="Tiền" GroupName="ck_dv" />
                                                </span>
                                                <asp:TextBox ID="txt_chietkhau" data-role="input" runat="server" MaxLength="13" Text="0" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                            </div>
                                        </div>
                                        <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                            <div>
                                                <label class="fw-600">Nhân viên chốt sale</label>
                                                <asp:DropDownList ID="ddl_nhanvien_chotsale" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Chiết khấu chốt sale</label>
                                                <span class="place-right">
                                                    <asp:RadioButton ID="ck_dv_phantram_chotsale" runat="server" Text="%" GroupName="ck_dv_chotsale" Checked="true" />
                                                    <asp:RadioButton ID="ck_dv_tienmat_chotsale" runat="server" Text="Tiền" GroupName="ck_dv_chotsale" />
                                                </span>
                                                <asp:TextBox ID="txt_chietkhau_chotsale" data-role="input" runat="server" MaxLength="13" Text="0" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Nhân viên làm dịch vụ</label>
                                                <asp:DropDownList ID="ddl_nhanvien_lamdichvu" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Chiết khấu làm dịch vụ</label>
                                                <span class="place-right">
                                                    <asp:RadioButton ID="ck_dv_phantram_chotsale_lamdv" runat="server" Text="%" GroupName="ck_dv_lamdv" Checked="true" />
                                                    <asp:RadioButton ID="ck_dv_tienmat_chotsale_lamdv" runat="server" Text="Tiền" GroupName="ck_dv_lamdv" />
                                                </span>
                                                <asp:TextBox ID="txt_chietkhau_lamdichvu" data-role="input" runat="server" MaxLength="13" Text="0" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Đánh giá nhân viên làm dịch vụ</label>
                                                <asp:TextBox ID="txt_danhgia_dichvu" data-role="input" runat="server"></asp:TextBox><%--autocomplete="off" --%>
                                            </div>
                                            <input data-role="rating" data-value="0" name="danhgia_5sao_nhanvien_dv">
                                        </div>
                                    </div>

                                    <div class="mt-6 mb-6 text-right">
                                        <div style="float: left">
                                            <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                        </div>
                                        <div style="float: right">
                                            <asp:Button ID="but_form_themdichvu" runat="server" Text="THÊM DỊCH VỤ" CssClass="button success" OnClick="but_form_themdichvu_Click" />
                                        </div>
                                        <div style="clear: both"></div>
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
                    <div id="themsanpham">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="Panel2" runat="server" DefaultButton="but_form_themsanpham">
                                    <div class="row ">
                                        <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                            <div class="">
                                                <label class="fw-600">Ngày bán</label>

                                                <asp:TextBox ID="txt_ngayban_sanpham" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Sản phẩm</label>
                                                <%--<asp:DropDownList ID="ddl_sanpham" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_sanpham_SelectedIndexChanged"></asp:DropDownList>--%>
                                                <asp:TextBox ID="txt_tensanpham" runat="server" data-role="input" placeholder="Nhập và chọn tên sản phẩm" OnTextChanged="txt_tensanpham_TextChanged" AutoPostBack="true"></asp:TextBox></div>
                                            <div class="mt-3">
                                                <label class="fw-600">Giá</label>
                                                <asp:TextBox ID="txt_gia_sanpham" MaxLength="13" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Số lượng</label>
                                                <asp:TextBox ID="txt_soluong_sanpham" data-role="input" runat="server" MaxLength="5" Text="1"></asp:TextBox><%--autocomplete="off" --%>
                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Chiết khấu sản phẩm</label>
                                                <span class="place-right">
                                                    <asp:RadioButton ID="ck_sp_phantram" runat="server" Text="%" GroupName="ck_sp" Checked="true" />
                                                    <asp:RadioButton ID="ck_sp_tienmat" runat="server" Text="Tiền" GroupName="ck_sp" />
                                                </span>
                                                <asp:TextBox ID="txt_chietkhau_sanpham" data-role="input" runat="server" MaxLength="13" Text="0" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                            </div>
                                        </div>
                                        <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                            <div>
                                                <label class="fw-600">Nhân viên chốt sale</label>
                                                <asp:DropDownList ID="ddl_nhanvien_chotsale_sanpham" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Chiết khấu chốt sale</label>
                                                <span class="place-right">
                                                    <asp:RadioButton ID="ck_sp_phantram_chotsale" runat="server" Text="%" GroupName="ck_sp_chotsale" Checked="true" />
                                                    <asp:RadioButton ID="ck_sp_tienmat_chotsale" runat="server" Text="Tiền" GroupName="ck_sp_chotsale" />
                                                </span>
                                                <asp:TextBox ID="txt_chietkhau_chotsale_sanpham" data-role="input" runat="server" MaxLength="13" onchange="format_sotien(this);" Text="0"></asp:TextBox><%--autocomplete="off" --%>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="mt-6 mb-6 text-right">
                                        <div style="float: left">
                                            <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                        </div>
                                        <div style="float: right">
                                            <asp:Button ID="but_form_themsanpham" runat="server" Text="THÊM SẢN PHẨM" CssClass="button success" OnClick="but_form_themsanpham_Click" />
                                        </div>
                                        <div style="clear: both"></div>
                                    </div>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="UpdatePanel3">
                            <ProgressTemplate>
                                <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                                    <div style="padding-top: 50vh;">
                                        <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                                    </div>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </div>
                    <div id="thedichvu">
                        <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="Panel5" runat="server" DefaultButton="Button1">
                                    <div class="row ">
                                        <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                            <div class="mt-3">
                                                <label class="fw-600">Ngày sử dụng</label>
                                                <asp:TextBox ID="txt_ngayban_thedv" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                            <div class="mt-3">
                                                <label class="fw-600">Nhân viên làm dịch vụ</label>
                                                <asp:DropDownList ID="ddl_nhanvien_lamdichvu_thedv" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                            <div class="mt-3">
                                                <label class="fw-600">Chiết khấu làm dịch vụ</label>
                                                <span class="place-right">
                                                    <asp:RadioButton ID="ck_dv_phantram_lamdv_thedv" runat="server" Text="%" GroupName="ck_dv_lamdv_thedv" Checked="true" />
                                                    <asp:RadioButton ID="ck_dv_tienmat_lamdv_thedv" runat="server" Text="Tiền" GroupName="ck_dv_lamdv_thedv" />
                                                </span>
                                                <asp:TextBox ID="txt_chietkhau_lamdichvu_thedv" data-role="input" runat="server" MaxLength="10" Text="0" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                            </div>
                                        </div>
                                        <div class="cell-lg-6 mt-3 pl-3-md pr-3-md pl-0 pr-0">
                                            <div class="mt-3">
                                                <label class="fw-600">Đánh giá nhân viên làm dịch vụ</label>
                                                <asp:TextBox ID="txt_danhgia_dichvu_lamdv" data-role="input" runat="server"></asp:TextBox><%--autocomplete="off" --%>
                                            </div>
                                            <input data-role="rating" data-value="0" name="danhgia_5sao_nhanvien_dv_lamdv">
                                        </div>

                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Chọn 1 thẻ dịch vụ</label>

                                    </div>
                                    <div style="overflow: auto" class="">
                                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                            <thead>
                                                <tr style="background-color: #f5f5f5">
                                                    <%--<td style="width: 1px;" class=" text-bold text-center">#</td>--%>
                                                    <td style="width: 1px;"></td>
                                                    <td class="text-bold" style="min-width: 1px; width: 1px">Mã</td>
                                                    <td class="text-bold" style="min-width: 90px; width: 90px">HSD</td>
                                                    <td class="text-bold" style="min-width: 130px">Người bán/CK</td>
                                                    <td class="text-bold" style="min-width: 120px;">Tên thẻ/DV</td>
                                                    <td class="text-bold" style="min-width: 100px;">Số buổi</td>
                                                    <td class="text-bold" style="min-width: 140px">Khách hàng</td>
                                                    <td class="text-bold" style="min-width: 130px">Tổng tiền/CK</td>

                                                    <td class="text-bold" style="min-width: 100px">Sau CK</td>
                                                    <td class="text-bold" style="min-width: 110px">T.Toán/C.Nợ</td>

                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:Repeater ID="Repeater3" runat="server">
                                                    <ItemTemplate>
                                                        <tr class="<%#return_thedv_row_class(Eval("id").ToString()) %>">
                                                            <%--<td class="text-right"><%=stt %></td>--%>
                                                            <td class="checkbox-table-tdv">
                                                                <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_thedv_<%#Eval("id").ToString() %>" <%#return_thedv_checked_attr(Eval("id").ToString()) %>>
                                                            </td>
                                                            <td class="text-center">
                                                                <a data-role="hint" data-hint-position="top" data-hint-text="Chỉnh sửa thẻ dịch vụ" href="/gianhang/admin/quan-ly-the-dich-vu/chi-tiet.aspx?id=<%#Eval("id").ToString() %>">
                                                                    <b><%#Eval("id").ToString() %></b>
                                                                </a>
                                                            </td>
                                                            <td class="text-right">
                                                                <small>
                                                                    <div><%#Eval("hsd","{0:dd/MM/yyyy}").ToString() %></div>

                                                                </small>
                                                            </td>
                                                            <td>
                                                                <small>
                                                                    <%#Eval("tennguoichot").ToString() %>
                                                                    <div class="text-bold">
                                                                        <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#Eval("phantramchot").ToString()=="0" %>'>
                                                                            <div class="">CK: <%#Eval("tongtien_chot","{0:#,##0}").ToString() %></div>
                                                                        </asp:PlaceHolder>
                                                                        <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible='<%#Eval("phantramchot").ToString()!="0" %>'>
                                                                            <div class="">CK: <%#Eval("phantramchot")%>%</div>
                                                                        </asp:PlaceHolder>

                                                                    </div>
                                                                </small>
                                                            </td>

                                                            <td><small>
                                                                <%#Eval("tenthe").ToString() %>
                                                                <div class="fg-green"><%#Eval("tendv").ToString() %></div>
                                                                <asp:PlaceHolder ID="PlaceHolderBookingMatch" runat="server" Visible='<%#list_id_thedv_goiy.Contains(Eval("id").ToString()) %>'>
                                                                <div><code class="bg-emerald fg-white">Phù hợp lịch hẹn</code></div>
                                                                </asp:PlaceHolder>
                                                            </small>
                                                            </td>
                                                            <td>
                                                                <small>
                                                                    <div>Số buổi: <%#Eval("sobuoi").ToString() %></div>
                                                                    <div>Đã làm: <%#Eval("sl_dalam").ToString() %></div>
                                                                    <div>Còn lại: <%#Eval("sl_conlai").ToString() %></div>
                                                                </small>
                                                            </td>
                                                            <td><%#Eval("tenkhachhang").ToString() %>
                                                                <div><a class="fg-orange" data-role="hint" data-hint-position="top" data-hint-text="Nhấn để gọi" href="tel:<%#Eval("sdt").ToString() %>"><%#Eval("sdt").ToString() %></a></div>
                                                            </td>


                                                            <td class="text-right">
                                                                <%#Eval("tongtien","{0:#,##0}").ToString() %>
                                                                <div class="text-bold">
                                                                    <small>
                                                                        <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("ck_hoadon").ToString()=="0" %>'>CK: <%#Eval("tongtien_ck","{0:#,##0}").ToString() %>
                                                                        </asp:PlaceHolder>
                                                                        <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("ck_hoadon").ToString()!="0" %>'>CK: <%#Eval("ck_hoadon")%>%
                                                                        </asp:PlaceHolder>
                                                                    </small>
                                                                </div>
                                                            </td>


                                                            <td class="text-right text-bold">

                                                                <div><%#Eval("tongsauchietkhau","{0:#,##0}").ToString() %></div>
                                                            </td>
                                                            <td class="text-right">
                                                                <div>
                                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("sotien_conlai").ToString()=="0" %>'>
                                                                        <span class="data-wrapper"><code class="bg-red  fg-white">Đã thanh toán</code></span>
                                                                    </asp:PlaceHolder>
                                                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("sotien_conlai").ToString()!="0" %>'>
                                                                        <%#Eval("sotien_dathanhtoan","{0:#,##0}").ToString() %>
                                                                    </asp:PlaceHolder>
                                                                </div>
                                                                <div>
                                                                    <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("sotien_conlai").ToString()=="0" %>'>
                                                                        <%#Eval("sotien_conlai","{0:#,##0}").ToString() %>
                                                                    </asp:PlaceHolder>
                                                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("sotien_conlai").ToString()!="0" %>'>
                                                                        <span class="data-wrapper"><code class="bg-orange fg-white"><%#Eval("sotien_conlai","{0:#,##0}").ToString() %></code></span>
                                                                    </asp:PlaceHolder>
                                                                </div>
                                                            </td>

                                                        </tr>

                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>
                                    <div class="mt-6 mb-6 text-right">
                                        <div style="float: left">
                                            <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                        </div>
                                        <div style="float: right">
                                            <asp:Button ID="Button1" runat="server" Text="THÊM VÀO HÓA ĐƠN" CssClass="button success" OnClick="Button1_Click" />
                                        </div>
                                        <div style="clear: both"></div>
                                    </div>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdateProgress ID="UpdateProgress4" runat="server" AssociatedUpdatePanelID="UpdatePanel6">
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

            </div>
        </div>
    </div>
    <script>
        function show_hide_id_form_themdvsp() {
            var x = document.getElementById("form_themdvsp");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>

    <div id="main-content" class="mb-10">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="but_form_themdichvu" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="but_form_themsanpham" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="but_form_edithoadon" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="but_thanhtoan" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
            </Triggers>
            <ContentTemplate>

                <%--NỘI DUNG CHÍNH--%>

                <div class="row">
                    <div class="cell-lg-6 pr-3-lg mt-3">
                        <div data-role="panel"
                            data-title-caption="Hóa đơn: <b><%=id %></b>"
                            <%--data-title-icon="<a data-role='hint' data-hint-position='top' data-hint-text='Quay lại' href='/gianhang/admin/quan-ly-hoa-don/Default.aspx'><span class='mif-arrow-left'></span></a>"--%>
                            data-title-icon="<span class='mif-info'></span>"
                            data-collapsible="true">
                            <div class="bg-white pl-2 pr-2">
                                <%--TIÊU ĐỀ - MENU - THANH CÔNG CỤ--%>
                                <div>
                                    <div style="float: left" class="">
                                        <%--<h4>Danh sách hóa đơn</h4>--%>
                                        <ul class="h-menu ">
                                            <%if (bcorn_class.check_quyen(user, "q7_3") == "" ||bcorn_class.check_quyen(user, "n7_3") == "")
                                                { %>
                                            <li data-role="hint" data-hint-position="top" data-hint-text="Chỉnh sửa"><a class="button" onclick='show_hide_id_form_edithoadon()'><span class="mif mif-pencil"></span></a></li>
                                            <%} %>
                                            <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                            <li data-role="hint" data-hint-position="top" data-hint-text="In bill"><a class="button" target="_blank" href="/gianhang/admin/quan-ly-hoa-don/in-bill.aspx?id=<%=id %>">
                                                <span class="mif mif-printer pr-1"></span>Bill</a></li>
                                            <li data-role="hint" data-hint-position="top" data-hint-text="In A5"><a class="button" target="_blank" href="/gianhang/admin/quan-ly-hoa-don/in-a5.aspx?id=<%=id %>">
                                                <span class="mif mif-printer pr-1"></span>A5</a></li>
                                            <%if (bcorn_class.check_quyen(user, "q7_4") == ""||bcorn_class.check_quyen(user, "n7_4") == "")
                                                { %>
                                            <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                            <li data-role="hint" data-hint-position="top" data-hint-text="Xóa hóa đơn">
                                                <asp:ImageButton ID="but_xoahoadon" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" OnClick="but_xoahoadon_Click" OnClientClick="return confirm('Bạn đã chắc chắn chưa?');" />
                                            </li>
                                            <%} %>
                                            <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                            <li>
                                                <a class="c-pointer js-textareacopybtn<%=id_guide %>" onclick="show_saochep();"><span class="mif mif-link" data-role="hint" data-hint-position="top" data-hint-text="Sao chép link hóa đơn"></span></a>

                                            </li>
                                            <%--<li data-role="hint" data-hint-position="top" data-hint-text="Xuất excel"><a href="#"><span class="mif mif-file-excel"></span></a></li>--%>
                                        </ul>
                                        <div style="opacity: 0; z-index: -999; position: absolute; bottom: 0px; left: 0">
                                            <input type="text" value="https://ahashine.vn/hoa-don-dien-tu.aspx?id=<%=id_guide %>" class="js-copytextarea<%=id_guide %>">
                                        </div>
                                        <script>        
                                            var copyTextareaBtn = document.querySelector('.js-textareacopybtn<%=id_guide %>');

                                            copyTextareaBtn.addEventListener('click', function (event) {
                                                var copyTextarea = document.querySelector('.js-copytextarea<%=id_guide %>');
                                                copyTextarea.focus();
                                                copyTextarea.select();
                                                try {
                                                    var successful = document.execCommand('copy');
                                                    var msg = successful ? 'successful' : 'unsuccessful';
                                                    console.log('Copying text command was ' + msg);
                                                } catch (err) {
                                                    console.log('Oops, unable to copy');
                                                }
                                            });
                                        </script>
                                    </div>
                                    <div style="float: right" class="">
                                        <ul class="h-menu ">
                                        </ul>
                                    </div>
                                    <div class="clr-float"></div>
                                </div>
                                <%--END TIÊU ĐỀ - MENU - THANH CÔNG CỤ--%>
                                <hr />
                                <div class="mt-3">
                                    <h5 class="pl-2 pr-2 fg-orange"><%=ten_kh %></h5>
                                    <% if (co_ngu_canh_datlich) { %>
                                    <div class="booking-flow-box mt-3">
                                        <div class="d-flex flex-justify-between flex-wrap flex-align-start">
                                            <div>
                                                <div class="booking-flow-box__title">Hóa đơn đang nối với lịch hẹn #<%=id_datlich_lienket %></div>
                                                <div class="booking-flow-box__meta mt-1">
                                                    Dịch vụ: <%=ten_dichvu_datlich != "" ? ten_dichvu_datlich : "Chưa có dịch vụ" %>
                                                    <% if (ngay_datlich_lienket != "") { %> | Giờ hẹn: <%=ngay_datlich_lienket %><% } %>
                                                    <% if (ten_nhanvien_datlich != "") { %> | Nhân viên: <%=ten_nhanvien_datlich %><% } %>
                                                    <% if (trangthai_datlich_lienket != "") { %> | Trạng thái: <%=trangthai_datlich_lienket %><% } %>
                                                </div>
                                                <div class="booking-flow-box__hint mt-2"><%=thongbao_datlich_thedv %></div>
                                            </div>
                                            <div class="mt-2">
                                                <a class="button info outline mr-1" href="<%=url_quay_lai_datlich %>">Quay lại lịch hẹn</a>
                                                <% if (url_ho_so_khach_datlich != "") { %>
                                                <a class="button secondary mr-1" href="<%=url_ho_so_khach_datlich %>">Hồ sơ khách</a>
                                                <% } %>
                                                <% if (url_sudung_thedv_datlich != "" && tongquan_datlich.so_thedv_phuhop_dichvu > 0) { %>
                                                <a class="button success mr-1" href="<%=url_sudung_thedv_datlich %>">Dùng thẻ ở hồ sơ</a>
                                                <% } %>
                                                <a class="button warning" href="<%=url_ban_thedv_datlich %>">Bán thẻ từ lịch</a>
                                            </div>
                                        </div>
                                    </div>
                                    <% } %>
                                    <table class="table row-hover <%--striped--%> subcompact mt-3">
                                        <tbody>
                                            <tr>
                                                <td style="width: 90px">Ngày tạo:
                                                </td>
                                                <td class="info-hoadon">
                                                    <%=ngaytao %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 90px">Điện thoại:
                                                </td>
                                                <td class="info-hoadon">
                                                    <%=sdt_kh %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 90px">Địa chỉ:
                                                </td>
                                                <td class="info-hoadon">
                                                    <%=diachi_kh %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 90px">Ghi chú:
                                                </td>
                                                <td class="info-hoadon">
                                                    <%=ghichu_kh %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 90px">Tổng tiền:
                                                </td>
                                                <td class="info-hoadon"><%=tongtien %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 90px">Chiết khấu:
                                                </td>
                                                <td class="info-hoadon">
                                                    <%=ck_hoadon %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="text-bold" style="width: 90px">Sau CK:
                                                </td>
                                                <td class="text-bold"><%=sauck.ToString("#,##0") %>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                    <hr />
                                    <div class="pl-2 pr-2">
                                        <small class="fg-red">
                                            <%=km1_ghichu %>
                                        </small>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="cell-lg-6 pl-3-lg mt-3">
                        <div data-role="panel"
                            data-title-caption="Thanh toán"
                            data-title-icon="<span class='mif-paypal'></span>"
                            data-collapsible="true">
                            <div class="bg-white pl-2 pr-2">
                                <div>
                                    <div style="float: left" class="">
                                        <ul class="h-menu ">
                                            <%if (bcorn_class.check_quyen(user, "q7_5") == ""||bcorn_class.check_quyen(user, "n7_5") == "")
                                                { %>
                                            <li data-role="hint" data-hint-position="top" data-hint-text="Thanh toán"><a class="button" onclick='show_hide_id_form_thanhtoan()'><span class="mif mif-paypal"></span></a></li>
                                            <%} %>
                                            <%if (bcorn_class.check_quyen(user, "q7_6") == ""||bcorn_class.check_quyen(user, "n7_6") == "")
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
                        </div>
                    </div>
                </div>


                <div>
                    <h5 class="pt-10">Chi tiết hóa đơn</h5>

                    <div class="row">
                        <div class="cell-md-6 order-2 order-md-1 mt-0">
                            <asp:TextBox ID="txt_search" runat="server" data-role="input" data-prepend="<span class='mif mif-search'></span>" placeholder="Tìm kiếm theo dịch vụ hoặc sản phẩm" OnTextChanged="txt_search_TextChanged" AutoPostBack="true"></asp:TextBox>

                        </div>
                        <div class="cell-md-6 order-1 order-md-2 mt-0">

                            <div class="place-right">
                                <ul class="h-menu">
                                    <%if (bcorn_class.check_quyen(user, "q7_2") == ""||bcorn_class.check_quyen(user, "n7_2") == "")
                                        { %>
                                    <li data-role="hint" data-hint-position="top" data-hint-text="Thêm mặt hàng"><a class="button" onclick='show_hide_id_form_themdvsp()'><span class="mif mif-plus"></span></a></li>

                                    <%} %>
                                    <%if (bcorn_class.check_quyen(user, "q7_4") == ""||bcorn_class.check_quyen(user, "n7_4") == "")
                                        { %>
                                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                    <li data-role="hint" data-hint-position="top" data-hint-text="Xóa">
                                        <asp:ImageButton ID="but_xoa" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" OnClick="but_xoa_Click" />
                                    </li>
                                    <%} %>
                                </ul>
                            </div>
                            <div class="clr-float"></div>

                        </div>
                    </div>

                    <%--TABLE CHÍNH--%>
                    <div id="table-main">
                        <div style="overflow: auto" class=" mt-3">
                            <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                <thead>
                                    <tr style="background-color: #f5f5f5">
                                        <%--<td style="width: 1px;" class=" text-bold text-center">#</td>--%>
                                        <td style="width: 1px;">
                                            <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </td>
                                        <td class="text-bold" style="width: 100px;">Ngày bán</td>
                                        <%--<td class="text-bold text-center" style="width: 50px; min-width: 50px">Ảnh</td>--%>
                                        <td class="text-bold" style="min-width: 150px">Mặt hàng</td>

                                        <td class="text-bold text-center" style="width: 1px;">Giá</td>
                                        <td class="text-bold" style="width: 1px;">SL</td>
                                        <td class="text-bold text-right" style="width: 102px; min-width: 102px">Thành tiền</td>
                                        <td class="text-bold text-right" style="width: 1px;">CK</td>
                                        <td class="text-bold text-right" style="min-width: 150px;">Sau CK</td>

                                        <td class="text-bold" style="min-width: 190px">Nhân viên chốt</td>
                                        <td class="text-bold" style="min-width: 190px">Nhân viên làm dịch vụ</td>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <%--<td class="text-right"><%=stt %></td>--%>
                                                <td class="checkbox-table">
                                                    <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_<%#Eval("id").ToString() %>">
                                                </td>
                                                <td class="text-right">
                                                    <div><%#Eval("ngayban","{0:dd/MM/yyyy HH:mm}").ToString() %></div>
                                                </td>
                                                <%--<td>
                                                    <a href="/gianhang/admin/quan-ly-hoa-don/edit-cthd.aspx?id=<%#Eval("id") %>">
                                                        <img src="<%#Eval("hinhanh") %>" class="img-cover-vuong w-h-50" style="max-width: none!important">
                                                    </a>
                                                </td>--%>
                                                <td>
                                                    <a href="/gianhang/admin/quan-ly-hoa-don/edit-cthd.aspx?id=<%#Eval("id") %>" data-role='hint' data-hint-position='top' data-hint-text='Nhấn để sửa'>
                                                        <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#Eval("kyhieu").ToString()=="dichvu" %>'>
                                                            <span class="fg-blue"><%#Eval("ten_dichvu_sanpham").ToString() %></span>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible='<%#Eval("kyhieu").ToString()=="sanpham" %>'>
                                                            <span class="fg-green"><%#Eval("ten_dichvu_sanpham").ToString() %></span>
                                                        </asp:PlaceHolder>
                                                    </a>
                                                    <asp:PlaceHolder ID="PlaceHolder10" runat="server" Visible='<%#Eval("id_thedichvu")!=null%>'>
                                                        <span class="data-wrapper"><code class="bg-orange fg-white">Thẻ DV số <%#Eval("id_thedichvu")%></code></span>
                                                    </asp:PlaceHolder>
                                                </td>
                                                <td class="text-right">
                                                    <asp:PlaceHolder ID="PlaceHolder11" runat="server" Visible='<%#Eval("id_thedichvu")!=null%>'>
                                                        <%#Eval("gia_hienthi_khidung_thedv","{0:#,##0}").ToString() %> 
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder12" runat="server" Visible='<%#Eval("id_thedichvu")==null%>'>
                                                        <%#Eval("gia","{0:#,##0}").ToString() %> 
                                                    </asp:PlaceHolder>

                                                </td>
                                                <td class="text-right">
                                                    <%#Eval("soluong","{0:#,##0}").ToString() %>
                                                </td>
                                                <td class="text-right">
                                                    <div><%#Eval("thanhtien","{0:#,##0}").ToString() %> </div>
                                                </td>
                                                <td class="text-right">
                                                    <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("chietkhau").ToString()=="0" %>'>
                                                        <%#Eval("tongtien_ck","{0:#,##0}").ToString() %>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("chietkhau").ToString()!="0" %>'>
                                                        <%#Eval("chietkhau")%>%
                                                    </asp:PlaceHolder>
                                                </td>
                                                <td class="text-right">
                                                    <div><b><%#Eval("sauck","{0:#,##0}").ToString() %></b></div>
                                                </td>

                                                <td>
                                                    <div><%#Eval("tennguoichot")%></div>

                                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("phantramchot").ToString()=="0" %>'>
                                                        <div class="text-right text-bold fg-emerald"><%#Eval("tongtien_chot","{0:#,##0}").ToString() %></div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("phantramchot").ToString()!="0" %>'>
                                                        <div class="text-right text-bold fg-emerald"><%#Eval("phantramchot")%>%</div>
                                                    </asp:PlaceHolder>
                                                    </span>
                                                </td>
                                                <td>
                                                    <div><%#Eval("tennguoilam")%></div>

                                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("phantramlam").ToString()=="0" %>'>
                                                        <div class="text-right text-bold fg-emerald"><%#Eval("tongtien_lam","{0:#,##0}").ToString() %></div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("phantramlam").ToString()!="0" %>'>
                                                        <div class="text-right text-bold fg-emerald"><%#Eval("phantramlam")%>%</div>
                                                    </asp:PlaceHolder>

                                                    <div>
                                                        <asp:PlaceHolder ID="PlaceHolder9" runat="server" Visible='<%#Eval("kyhieu").ToString()=="dichvu" %>'>
                                                            <div>
                                                                <input data-role="rating" data-value="<%#Eval("danhgia_5sao_dv")%>" name="danhgia_5sao_dv_<%#Eval("id")%>" data-static="true">
                                                            </div>
                                                            <small><%#Eval("danhgia_nguoilam_dichvu")%></small>
                                                        </asp:PlaceHolder>
                                                    </div>
                                                </td>

                                            </tr>
                                            <%--  <%stt = stt + 1; %>--%>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                                <tfoot>
                                    <tr class=" text-bold">
                                        <td colspan="7" class="text-right">Tổng cộng</td>
                                        <td class="text-right"><%=tongtien %></td>
                                        <td colspan="3"></td>
                                    </tr>
                                    <tr class="text-bold">
                                        <script type="text/javascript">
                                            function handleEnter(e) {
                                                if (e.keyCode == 13) {
                                                    e.preventDefault();
                                                    document.getElementById('<%=but_form_edithoadon1.ClientID %>').click();
                                                    return false;
                                                }
                                                return true;
                                            }
                                        </script>
                                        <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:Panel ID="Panel6" runat="server" DefaultButton="but_form_edithoadon1">
                                                    <td colspan="7" class="text-right">Chiết khấu hóa đơn
                                            <div>
                                                <asp:RadioButton ID="ck_hd_phantram1" runat="server" Text="%" GroupName="ck_hd1" />
                                                <asp:RadioButton ID="ck_hd_tienmat1" runat="server" Text="Tiền" GroupName="ck_hd1" />
                                            </div>
                                                    </td>
                                                    <td class="text-right">
                                                        <%--<%=ck_hoadon %>--%>
                                                        <asp:TextBox onkeydown="return handleEnter(event);" ID="txt_chietkhau_hoadon1" data-clear-button="false" data-role="input" runat="server" MaxLength="13" Text="0" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                                    </td>
                                                    <td colspan="3">
                                                        <asp:ImageButton ID="but_form_edithoadon1" runat="server" ImageUrl="~/uploads/images/no.png" OnClick="but_form_edithoadon1_Click" />
                                                    </td>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </tr>
                                    <tr class="">
                                        <td colspan="7" class="text-right text-leader text-bold fg-red text-size-20">Sau chiết khấu</td>
                                        <td class="text-leader text-right text-bold fg-red text-size-20"><%=sauck.ToString("#,##0") %></td>
                                        <td colspan="3"></td>
                                    </tr>
                                    <tr class="text-bold">
                                        <td colspan="8" class="text-right fg-red">Số tiền bằng chữ: <%=tienbangchu %> đồng.</td>
                                        <td colspan="3"></td>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>

                    </div>
                    <%--END TABLE CHÍNH--%>
                </div>


                <%--END DUNG CHÍNH--%>
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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <script>
        function show_saochep() {
            Metro.notify.create("Sao chép link hóa đơn thành công.", "Thông báo", {});
        }
    </script>
    <%--<%=notifi %>--%>
</asp:Content>

