<%@ Page Title="Danh sách lịch hẹn" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="danh-sach-lich-hen.aspx.cs" Inherits="admin_quan_ly_khach_hang_danh_sach_lich_hen" %><asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .crm-mini-pill {
            display: inline-flex;
            align-items: center;
            padding: 3px 10px;
            border-radius: 999px;
            font-size: 11px;
            font-weight: 700;
            letter-spacing: .02em;
            text-transform: uppercase;
            color: #fff;
            margin-top: 6px;
        }

        .crm-list-links a {
            margin-right: 6px;
            margin-top: 6px;
            display: inline-block;
        }

        .crm-row-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 6px;
            margin-top: 10px;
        }

        .crm-row-action {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 28px;
            padding: 5px 10px;
            border-radius: 999px;
            border: 1px solid #d7e4e8;
            background: #fff;
            color: #17323f;
            font-size: 11px;
            font-weight: 700;
            letter-spacing: .02em;
            text-transform: uppercase;
            text-decoration: none;
            line-height: 1;
            transition: all .15s ease;
        }

        .crm-row-action:hover {
            text-decoration: none;
            transform: translateY(-1px);
        }

        .crm-row-action--confirm {
            border-color: #91d8bf;
            background: #eaf9f2;
            color: #0e7751;
        }

        .crm-row-action--arrived {
            border-color: #f2c37d;
            background: #fff5df;
            color: #935400;
        }

        .crm-row-action--invoice {
            border-color: #b9d6ea;
            background: #edf7fe;
            color: #0f5f85;
        }

        .crm-row-action--card {
            border-color: #9ad5d9;
            background: #ebfbfb;
            color: #0f6e72;
        }

        .crm-row-action--fallback {
            border-color: #dfe7ea;
            background: #f6f8f9;
            color: #667b86;
        }

        .ops-hub {
            border: 1px solid #d9e4e8;
            background: #f8fbfc;
            border-radius: 14px;
            padding: 18px;
        }

        .ops-hub__head {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
        }

        .ops-hub__kpis {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
            gap: 12px;
            margin-top: 16px;
        }

        .ops-hub__card {
            background: #fff;
            border: 1px solid #e1ecef;
            border-radius: 12px;
            padding: 14px;
        }

        .ops-hub__card small {
            display: block;
            color: #69808c;
            margin-bottom: 6px;
        }

        .ops-hub__card strong {
            display: block;
            color: #17323f;
            font-size: 24px;
            line-height: 1.1;
        }

        .ops-hub__sub {
            color: #69808c;
            margin-top: 4px;
        }

        .ops-hub__columns {
            display: grid;
            grid-template-columns: 1.2fr .9fr 1fr;
            gap: 14px;
            margin-top: 16px;
        }

        .ops-hub__panel {
            background: #fff;
            border: 1px solid #e1ecef;
            border-radius: 12px;
            padding: 14px;
        }

        .ops-hub__title {
            color: #17323f;
            font-size: 15px;
            font-weight: 700;
        }

        .ops-hub__caption {
            color: #69808c;
            font-size: 13px;
            margin-top: 2px;
        }

        .ops-list {
            margin-top: 12px;
        }

        .ops-list__item {
            border-top: 1px dashed #e3edf0;
            padding: 12px 0;
        }

        .ops-list__item:first-child {
            border-top: 0;
            padding-top: 0;
        }

        .ops-list__top {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 8px;
            flex-wrap: wrap;
        }

        .ops-list__meta {
            color: #6a818d;
            margin-top: 4px;
            line-height: 1.55;
        }

        .ops-chip {
            display: inline-flex;
            align-items: center;
            padding: 3px 10px;
            border-radius: 999px;
            font-size: 11px;
            font-weight: 700;
            letter-spacing: .02em;
            text-transform: uppercase;
            color: #fff;
        }

        .ops-alert-link {
            color: #0f6c8d;
            font-weight: 600;
        }

        .ops-empty {
            color: #6f8590;
            margin-top: 10px;
        }

        @media (max-width: 1199px) {
            .ops-hub__columns {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id='form_1' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_1()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Lọc dữ liệu</h5>
                <asp:Panel ID="Panel2" runat="server" DefaultButton="Button1">
                    <ul data-role="tabs" data-expand="true">
                        <li><a href="#_data">Dữ liệu</a></li>
                        <li><a href="#_time">Thời gian</a></li>
                        <li><a href="#_sort">Sắp xếp</a></li>
                    </ul>
                    <div class="">
                        <div id="_data">
                            <div class="mt-3">
                                <div class="fw-600">Số lượng hiển thị mỗi trang</div>
                                <asp:TextBox ID="txt_show" MaxLength="6" runat="server" data-role="input" data-clear-button="true"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Trạng thái</label>
                                <asp:DropDownList ID="DropDownList1" runat="server" data-role="select" data-filter="true">
                                    <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Chưa xác nhận" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Đã xác nhận" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Không đến" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Đã đến" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="Đã hủy" Value="5"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div id="_time">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="mt-3">
                                        <div class="row">
                                            <div class="cell-6 pr-1">
                                                <div class="fw-600">Từ ngày</div>
                                                <asp:TextBox ID="txt_tungay" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>

                                            </div>
                                            <div class="cell-6 pl-1">
                                                <div class="fw-600">Đến ngày</div>
                                                <asp:TextBox ID="txt_denngay" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>

                                            </div>
                                        </div>
                                        <div class="row mt-3">
                                            <div class="cell-12">
                                                <div class="fw-600">Chọn nhanh</div>
                                            </div>
                                            <div class="cell-6 pr-1">
                                                <div class="mt-1">
                                                    <asp:Button ID="but_homqua" runat="server" Text="Hôm qua" CssClass="mt-1 light" Width="100%" OnClick="but_homqua_Click" />
                                                    <asp:Button ID="but_tuantruoc" runat="server" Text="Tuần trước" CssClass="mt-1 light" Width="100%" OnClick="but_tuantruoc_Click" />
                                                    <asp:Button ID="but_thangtruoc" runat="server" Text="Tháng trước" CssClass="mt-1 light" Width="100%" OnClick="but_thangtruoc_Click" />
                                                    <asp:Button ID="but_quytruoc" runat="server" Text="Quý trước" CssClass="mt-1 light" Width="100%" OnClick="but_quytruoc_Click" />
                                                    <asp:Button ID="but_namtruoc" runat="server" Text="Năm trước" CssClass="mt-1 light" Width="100%" OnClick="but_namtruoc_Click" />
                                                </div>
                                            </div>
                                            <div class="cell-6 pl-1">
                                                <div class="mt-1">
                                                    <asp:Button ID="but_homnay" runat="server" Text="Hôm nay" CssClass="mt-1 light" Width="100%" OnClick="but_homnay_Click" />
                                                    <asp:Button ID="but_tuannay" runat="server" Text="Tuần này" CssClass="mt-1 light" Width="100%" OnClick="but_tuannay_Click" />
                                                    <asp:Button ID="but_thangnay" runat="server" Text="Tháng này" CssClass="mt-1 light" Width="100%" OnClick="but_thangnay_Click" />
                                                    <asp:Button ID="but_quynay" runat="server" Text="Quý này" CssClass="mt-1 light" Width="100%" OnClick="but_quynay_Click" />
                                                    <asp:Button ID="but_namnay" runat="server" Text="Năm này" CssClass="mt-1 light" Width="100%" OnClick="but_namnay_Click" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="UpdatePanel3">
                                <ProgressTemplate>
                                    <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                                        <div style="padding-top: 45vh;">
                                            <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                                        </div>
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>

                        </div>
                        <div id="_sort">
                            <div class="mt-3">
                                <div class="fw-600">Sắp xếp theo</div>
                                <asp:DropDownList ID="DropDownList2" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Ngày đặt (Tăng dần)" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Ngày đặt (Giảm dần)" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Ngày tạo (Tăng dần)" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Ngày tạo (Giảm dần)" Value="3"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="mt-6 mb-10">

                        <div style="float: left">
                            <asp:Button ID="Button2" runat="server" Text="Đặt lại mặc định" CssClass="button link small fg-orange-hover" OnClick="Button2_Click" />
                        </div>
                        <div style="float: right">
                            <asp:Button ID="Button1" runat="server" Text="BẮT ĐẦU LỌC" CssClass="button success" OnClick="Button1_Click" OnClientClick="show_hide_id_form_1()" />
                        </div>
                        <div style="clear: both"></div>

                    </div>
                </asp:Panel>
            </div>

        </div>
    </div>
    <script>
        function show_hide_id_form_1() {
            var x = document.getElementById("form_1");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>

    <%if (bcorn_class.check_quyen(user, "q10_2") == "")
        { %>
    <div id='form_2' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: <%=show_add%>; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 1200px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_2()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Đặt lịch hẹn</h5>
                <hr />
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" DefaultButton="Button3">
                            <div class="row">
                                <div class="cell-lg-6 pr-2-lg">
                                    <div class="mt-3">
                                        <label class="fw-600">Ngày đặt</label>
                                        <asp:TextBox ID="txt_ngaydat" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="false"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Giờ đặt</label>
                                        <div class="d-flex">
                                            <asp:DropDownList ID="ddl_giobatdau" runat="server" data-role="select" data-filter="flase" CssClass="mr-2"></asp:DropDownList>
                                            <asp:DropDownList ID="ddl_phutbatdau" runat="server" data-role="select" data-filter="flase" CssClass="ml-2"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Tên khách hàng</label>
                                        <asp:TextBox ID="txt_tenkhachhang" runat="server" data-role="input" OnTextChanged="txt_tenkhachhang_TextChanged" AutoPostBack="true"></asp:TextBox></div>
                                    <div class="mt-3">
                                        <label class="fw-600">Điện thoại</label>
                                        <%--<asp:TextBox ID="txt_sdt" runat="server" data-role="input" OnTextChanged="txt_sdt_TextChanged" AutoPostBack="true"></asp:TextBox>--%>
                                        <asp:TextBox ID="txt_sdt" runat="server" data-role="input" OnTextChanged="txt_sdt_TextChanged" AutoPostBack="true"></asp:TextBox></div>
                                    <div class="mt-3">
                                        <label class="fw-600">Ghi chú</label>
                                        <asp:TextBox ID="txt_ghichu" runat="server" data-role="textarea" TextMode="MultiLine"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Nguồn</label>
                                        <asp:TextBox ID="txt_nguon" runat="server" data-role="input"></asp:TextBox></div>
                                </div>
                                <div class="cell-lg-6 pl-2-lg">
                                    <div class="mt-3">
                                        <label class="fw-600">Dịch vụ</label>
                                        <asp:DropDownList ID="ddl_dichvu" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Nhân viên thực hiện</label>
                                        <asp:DropDownList ID="ddl_nhanvien" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600">Trạng thái</label>
                                        <asp:DropDownList ID="ddl_trangthai" runat="server" data-role="select" data-filter="true">
                                            <asp:ListItem Text="Chưa xác nhận" Value="Chưa xác nhận"></asp:ListItem>
                                            <asp:ListItem Text="Đã xác nhận" Value="Đã xác nhận"></asp:ListItem>
                                            <asp:ListItem Text="Không đến" Value="Không đến"></asp:ListItem>
                                            <asp:ListItem Text="Đã đến" Value="Đã đến"></asp:ListItem>
                                            <asp:ListItem Text="Đã hủy" Value="Đã hủy"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>

                                </div>
                            </div>

                            <div class="mt-6 mb-10">
                                <div style="float: left">
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="Button3" runat="server" Text="ĐẶT LỊCH HẸN" CssClass="button success" OnClick="Button3_Click" />
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

        </div>
    </div>
    <script>
        function show_hide_id_form_2() {
            var x = document.getElementById("form_2");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>
    <%} %>

    <div id="main-content" class="mb-10">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />
            </Triggers>
            <ContentTemplate>
                <div class="row mt-1-minus <%--mt-0-lg-minus mt-12-minus--%>">
                    <div class="cell-md-6 order-2 order-md-1 mt-0">
                        <asp:TextBox ID="txt_search" runat="server" data-role="input" data-prepend="<span class='mif mif-search'></span>" placeholder="Tìm kiếm" OnTextChanged="txt_search_TextChanged" AutoPostBack="true"></asp:TextBox>
                    </div>
                    <div class="cell-md-6 order-1 order-md-2 mt-0">

                        <div class="place-right">
                            <ul class="h-menu">
                                <%if (bcorn_class.check_quyen(user, "q10_2") == "")
                                    { %>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Thêm" onclick="show_hide_id_form_2()">
                                    <a class="button"><span class="mif mif-plus"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <%} %>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                                    <a class="button"><span class="mif mif-filter"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>

                                <%if (bcorn_class.check_quyen(user, "q10_4") == "")
                                    { %>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Xóa">
                                    <asp:ImageButton ID="Button4" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" OnClick="Button4_Click" OnClientClick="return confirm('Bạn đã chắc chắn chưa?');" />
                                </li>
                                <%} %>
                            </ul>
                        </div>
                        <div class="clr-float"></div>

                    </div>
                </div>

                <div class="ops-hub mt-4">
                    <div class="ops-hub__head">
                        <div>
                            <div class="text-bold">Điều phối vận hành hôm nay</div>
                            <div class="fg-gray">Chốt nhanh lịch trong ngày theo trạng thái, nhân viên, bộ phận và các cảnh báo cần xử lý.</div>
                        </div>
                        <div>
                            <span class="ops-chip bg-dark"><%=dieu_phoi_homnay.ngay.ToString("dd/MM/yyyy") %></span>
                        </div>
                    </div>

                    <div class="ops-hub__kpis">
                        <div class="ops-hub__card">
                            <small>Tổng lịch hôm nay</small>
                            <strong><%=dieu_phoi_homnay.tong_lich.ToString("#,##0") %></strong>
                            <div class="ops-hub__sub"><%=dieu_phoi_homnay.so_lich_da_ghi_nhan_giaodich.ToString("#,##0") %> lịch đã phát sinh giao dịch</div>
                        </div>
                        <div class="ops-hub__card">
                            <small>Cần xác nhận</small>
                            <strong><%=dieu_phoi_homnay.so_lich_cho_xacnhan.ToString("#,##0") %></strong>
                            <div class="ops-hub__sub"><%=dieu_phoi_homnay.so_lich_sap_toi_2gio.ToString("#,##0") %> lịch sắp tới trong 2 giờ</div>
                        </div>
                        <div class="ops-hub__card">
                            <small>Quá giờ chưa xử lý</small>
                            <strong><%=dieu_phoi_homnay.so_lich_qua_gio_chua_xuly.ToString("#,##0") %></strong>
                            <div class="ops-hub__sub"><%=dieu_phoi_homnay.so_lich_da_den.ToString("#,##0") %> lịch đã đến được chốt</div>
                        </div>
                        <div class="ops-hub__card">
                            <small>Chưa phân công</small>
                            <strong><%=dieu_phoi_homnay.so_lich_chua_phan_cong.ToString("#,##0") %></strong>
                            <div class="ops-hub__sub"><%=dieu_phoi_homnay.so_lich_chua_co_hoso.ToString("#,##0") %> lịch chưa có hồ sơ CRM</div>
                        </div>
                    </div>

                    <div class="ops-hub__columns">
                        <div class="ops-hub__panel">
                            <div class="ops-hub__title">Nhịp tải theo nhân viên</div>
                            <div class="ops-hub__caption">Ưu tiên người đang quá giờ hoặc có tải cao để staff điều phối lại ngay.</div>
                            <% if (dieu_phoi_homnay.list_nhanvien.Count == 0) { %>
                            <div class="ops-empty">Chưa có lịch nào được gán nhân viên trong ngày.</div>
                            <% } else { %>
                            <div class="ops-list">
                                <% foreach (var item in dieu_phoi_homnay.list_nhanvien) { %>
                                <div class="ops-list__item">
                                    <div class="ops-list__top">
                                        <div class="text-bold"><%=item.hoten %></div>
                                        <span class="ops-chip <%=item.muc_tai_css %>"><%=item.muc_tai %></span>
                                    </div>
                                    <div class="ops-list__meta">
                                        <%=item.ten_bophan %> | <b><%=item.tong_lich.ToString("#,##0") %></b> lịch
                                        | <%=item.so_lich_cho_xacnhan.ToString("#,##0") %> chờ xác nhận
                                        | <%=item.so_lich_da_den.ToString("#,##0") %> đã đến
                                        <% if (item.so_lich_qua_gio > 0) { %>| <span class="fg-red"><%=item.so_lich_qua_gio.ToString("#,##0") %> quá giờ</span><% } %>
                                    </div>
                                    <% if (item.id_lich_gan_nhat != "") { %>
                                    <div class="ops-list__meta">Lịch gần nhất: <a class="ops-alert-link" href="/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=<%=item.id_lich_gan_nhat %>">#<%=item.id_lich_gan_nhat %></a> lúc <%=return_ngaygio_ngan(item.gio_hen_gan_nhat) %></div>
                                    <% } %>
                                </div>
                                <% } %>
                            </div>
                            <% } %>
                        </div>

                        <div class="ops-hub__panel">
                            <div class="ops-hub__title">Tải theo bộ phận</div>
                            <div class="ops-hub__caption">Dùng lõi `id_bophan` hiện có để biết bộ phận nào đang chịu tải chính trong ngày.</div>
                            <% if (dieu_phoi_homnay.list_bophan.Count == 0) { %>
                            <div class="ops-empty">Chưa có dữ liệu bộ phận để điều phối hôm nay.</div>
                            <% } else { %>
                            <div class="ops-list">
                                <% foreach (var item in dieu_phoi_homnay.list_bophan) { %>
                                <div class="ops-list__item">
                                    <div class="ops-list__top">
                                        <div class="text-bold"><%=item.ten_bophan %></div>
                                        <span class="ops-chip <%=(item.so_lich_qua_gio > 0 ? "bg-red" : (item.so_lich_cho_xacnhan > 0 ? "bg-orange" : "bg-green")) %>"><%=item.tong_lich.ToString("#,##0") %> lịch</span>
                                    </div>
                                    <div class="ops-list__meta">
                                        <%=item.so_lich_cho_xacnhan.ToString("#,##0") %> chờ xác nhận
                                        | <%=item.so_lich_da_den.ToString("#,##0") %> đã đến
                                        <% if (item.so_lich_qua_gio > 0) { %>| <span class="fg-red"><%=item.so_lich_qua_gio.ToString("#,##0") %> quá giờ</span><% } %>
                                    </div>
                                </div>
                                <% } %>
                            </div>
                            <% } %>
                        </div>

                        <div class="ops-hub__panel">
                            <div class="ops-hub__title">Cảnh báo cần chạm ngay</div>
                            <div class="ops-hub__caption">Mỗi cảnh báo mở thẳng vào lịch tương ứng để xử lý nhanh trên một màn.</div>
                            <% if (dieu_phoi_homnay.list_canhbao.Count == 0) { %>
                            <div class="ops-empty">Hiện không có lịch nào cần can thiệp gấp trong ngày.</div>
                            <% } else { %>
                            <div class="ops-list">
                                <% foreach (var item in dieu_phoi_homnay.list_canhbao) { %>
                                <div class="ops-list__item">
                                    <div class="ops-list__top">
                                        <span class="ops-chip <%=item.loai_css %>"><%=item.loai %></span>
                                        <div class="fg-gray"><%=item.ngaydat.HasValue ? item.ngaydat.Value.ToString("HH:mm") : "" %></div>
                                    </div>
                                    <div class="mt-1"><a class="ops-alert-link" href="<%=item.url %>"><%=item.tieu_de %></a></div>
                                    <div class="ops-list__meta"><%=item.mo_ta %></div>
                                </div>
                                <% } %>
                            </div>
                            <% } %>
                        </div>
                    </div>
                </div>

                <div id="table-main">

                    <div style="overflow: auto" class=" mt-4">
                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                            <tbody>
                                <tr style="background-color: #f5f5f5">
                                    <td style="width: 1px;">
                                        <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                    </td>
                                    <td class="text-bold" style="min-width: 1px; width: 1px">Mã</td>
                                    <td class="text-bold" style="min-width: 130px; width: 130px">Ngày đặt</td>
                                    <td class="text-bold" style="min-width: 140px">Khách hàng</td>

                                    <td class="text-bold" style="min-width: 140px">Dịch vụ</td>
                                    <td class="text-bold" style="min-width: 140px">NV thực hiện</td>
                                    <td class="text-bold" style="min-width: 160px">Vận hành</td>
                                    <td class="text-bold" style="min-width: 140px">Ghi chú</td>
                                    <td class="text-bold" style="min-width: 140px">Người tạo</td>
                                    <td class="text-bold" style="min-width: 100px; width: 100px">Trạng thái</td>
                                </tr>
                                <asp:Repeater ID="Repeater1" runat="server" OnItemCommand="Repeater1_ItemCommand">
                                    <ItemTemplate>
                                        <tr>

                                            <td class="checkbox-table">
                                                <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_<%#Eval("id").ToString() %>">
                                            </td>
                                            <td class="text-center">
                                                <a data-role="hint" data-hint-position="top" data-hint-text="Chỉnh sửa" href="/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=<%#Eval("id").ToString() %>">
                                                    <b><%#Eval("id").ToString() %></b>
                                                </a>
                                            </td>
                                            <td>
                                                <small>
                                                    <%#Eval("ngaydat","{0:dd/MM/yyyy HH:mm}").ToString() %>
                                                    <div class="text-bold"><%#Eval("nguongoc").ToString() %></div>
                                                </small>
                                                

                                            </td>
                                            <td><%#Eval("tenkhachhang").ToString() %>
                                                <div><a class="fg-cobalt" data-role="hint" data-hint-position="top" data-hint-text="Nhấn để gọi" href="tel:<%#Eval("sdt").ToString() %>"><%#Eval("sdt").ToString() %></a></div>
                                                <asp:PlaceHolder ID="PlaceHolderProfile" runat="server" Visible='<%#co_hoso_tu_lich(Eval("id").ToString()) %>'>
                                                    <div><a class="fg-teal" href='<%#return_url_khachhang_tu_lich(Eval("id").ToString()) %>'>Mở hồ sơ khách</a></div>
                                                </asp:PlaceHolder>
                                            </td>

                                            <td><%#Eval("tendv").ToString() %></td>
                                            <td><%#Eval("nhanvien_thuchien").ToString() %></td>
                                            <td>
                                                <span class="crm-mini-pill <%#return_css_nhan_vanhanh_tu_lich(Eval("id").ToString()) %>"><%#return_nhan_vanhanh_tu_lich(Eval("id").ToString()) %></span>
                                                <div class="crm-list-links">
                                                    <asp:PlaceHolder ID="PlaceHolderHdLinked" runat="server" Visible='<%#return_so_hoadon_lienket_tu_lich(Eval("id").ToString()) > 0 %>'>
                                                        <a class="fg-green" href='<%#return_url_hoadon_lienket_tu_lich(Eval("id").ToString()) %>'>
                                                            <span class="data-wrapper"><code class="bg-green fg-white"><%#return_so_hoadon_lienket_tu_lich(Eval("id").ToString()) %> HĐ</code></span>
                                                        </a>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolderTdvLinked" runat="server" Visible='<%#return_so_thedv_lienket_tu_lich(Eval("id").ToString()) > 0 %>'>
                                                        <a class="fg-teal" href='<%#return_url_thedv_tu_lich(Eval("id").ToString()) %>'>
                                                            <span class="data-wrapper"><code class="bg-cyan fg-white"><%#return_so_thedv_lienket_tu_lich(Eval("id").ToString()) %> thẻ DV</code></span>
                                                        </a>
                                                    </asp:PlaceHolder>
                                                </div>
                                                <div class="crm-row-actions">
                                                    <asp:PlaceHolder ID="PlaceHolderQuickConfirm" runat="server" Visible='<%#hien_hanhdong_xacnhan_nhanh(Eval("id").ToString()) %>'>
                                                        <asp:PlaceHolder ID="PlaceHolderQuickConfirmReady" runat="server" Visible='<%#co_the_xacnhan_nhanh(Eval("id").ToString()) %>'>
                                                            <asp:LinkButton ID="LinkQuickConfirm" runat="server" CssClass="crm-row-action crm-row-action--confirm" CommandName="quick_confirm" CommandArgument='<%#Eval("id").ToString() %>' CausesValidation="false">Xác nhận</asp:LinkButton>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolderQuickConfirmFallback" runat="server" Visible='<%# !co_the_xacnhan_nhanh(Eval("id").ToString()) %>'>
                                                            <a class="crm-row-action crm-row-action--fallback" href='<%#return_url_chinhsua_lich(Eval("id").ToString()) %>'><%#return_nhan_xacnhan_nhanh(Eval("id").ToString()) %></a>
                                                        </asp:PlaceHolder>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolderQuickArrived" runat="server" Visible='<%#hien_hanhdong_da_den_nhanh(Eval("id").ToString()) %>'>
                                                        <asp:PlaceHolder ID="PlaceHolderQuickArrivedReady" runat="server" Visible='<%#co_the_da_den_nhanh(Eval("id").ToString()) %>'>
                                                            <asp:LinkButton ID="LinkQuickArrived" runat="server" CssClass="crm-row-action crm-row-action--arrived" CommandName="quick_arrived" CommandArgument='<%#Eval("id").ToString() %>' CausesValidation="false">Đã đến</asp:LinkButton>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolderQuickArrivedFallback" runat="server" Visible='<%# !co_the_da_den_nhanh(Eval("id").ToString()) %>'>
                                                            <a class="crm-row-action crm-row-action--fallback" href='<%#return_url_chinhsua_lich(Eval("id").ToString()) %>'><%#return_nhan_da_den_nhanh(Eval("id").ToString()) %></a>
                                                        </asp:PlaceHolder>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolderQuickInvoice" runat="server" Visible='<%#hien_hanhdong_hoa_don_nhanh(Eval("id").ToString()) %>'>
                                                        <a class="crm-row-action crm-row-action--invoice" href='<%#return_url_hoa_don_nhanh(Eval("id").ToString()) %>'><%#return_nhan_hoa_don_nhanh(Eval("id").ToString()) %></a>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolderQuickThedv" runat="server" Visible='<%#hien_hanhdong_thedv_nhanh(Eval("id").ToString()) %>'>
                                                        <a class="crm-row-action crm-row-action--card" href='<%#return_url_thedv_nhanh(Eval("id").ToString()) %>'>Dùng thẻ</a>
                                                    </asp:PlaceHolder>
                                                </div>
                                            </td>
                                            <td><%#Eval("ghichu").ToString() %></td>
                                            <td>
                                                <%#Eval("nguoitao").ToString() %>
                                                <div><small><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></small></div>
                                            </td>
                                            <td>
                                                <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("trangthai").ToString()=="Chưa xác nhận" %>'>
                                                    <span class="data-wrapper"><code class="bg-cyan  fg-white">Chưa xác nhận</code></span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã xác nhận" %>'>
                                                    <span class="data-wrapper"><code class="bg-green  fg-white">Đã xác nhận</code></span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("trangthai").ToString()=="Không đến" %>'>
                                                    <span class="data-wrapper"><code class="bg-orange  fg-white">Không đến</code></span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã đến" %>'>
                                                    <span class="data-wrapper"><code class="bg-magenta  fg-white">Đã đến</code></span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã hủy" %>'>
                                                    <span class="data-wrapper"><code class="bg-red  fg-white">Đã hủy</code></span>
                                                </asp:PlaceHolder>
                                            </td>
                                        </tr>

                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <div class="text-center mt-8 mb-20">
                        <asp:Button ID="but_quaylai" runat="server" Text="Lùi" CssClass="" OnClick="but_quaylai_Click" />
                        <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                        <asp:Button ID="but_xemtiep" runat="server" Text="Tiếp" CssClass="" OnClick="but_xemtiep_Click" />
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

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>

