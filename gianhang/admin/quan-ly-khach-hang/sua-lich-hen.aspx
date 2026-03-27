<%@ Page Title="Chỉnh sửa lịch hẹn" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="sua-lich-hen.aspx.cs" Inherits="admin_quan_ly_khach_hang_danh_sach_lich_hen" %><asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .booking-ops-box {
            border: 1px solid #d8e4e8;
            border-radius: 12px;
            background: #f8fbfc;
            padding: 18px;
        }

        .booking-ops-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 12px;
        }

        .booking-ops-stat {
            border-radius: 10px;
            background: #ffffff;
            padding: 14px;
            border: 1px solid #e4eef1;
        }

        .booking-ops-stat small {
            display: block;
            color: #5f7480;
            margin-bottom: 6px;
        }

        .booking-ops-stat strong {
            color: #17323f;
            font-size: 20px;
        }

        .booking-ops-list {
            margin: 0;
            padding-left: 18px;
        }

        .booking-ops-list li + li {
            margin-top: 8px;
        }

        .booking-flow-timeline {
            border-top: 1px solid #dde7eb;
            margin-top: 22px;
            padding-top: 18px;
        }

        .booking-flow-timeline__item {
            display: flex;
            gap: 12px;
            padding: 12px 0;
            border-top: 1px dashed #e6eef2;
        }

        .booking-flow-timeline__item:first-child {
            border-top: 0;
        }

        .booking-flow-timeline__icon {
            width: 38px;
            height: 38px;
            border-radius: 50%;
            background: #ffffff;
            border: 1px solid #dbe6ea;
            color: #2d6a84;
            display: flex;
            align-items: center;
            justify-content: center;
            flex: 0 0 38px;
        }

        .booking-flow-timeline__content {
            flex: 1;
            min-width: 0;
        }

        .booking-flow-timeline__meta {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            flex-wrap: wrap;
        }

        .booking-flow-chip {
            display: inline-flex;
            align-items: center;
            padding: 3px 10px;
            border-radius: 999px;
            font-size: 11px;
            font-weight: 700;
            letter-spacing: .02em;
            text-transform: uppercase;
        }

        .booking-flow-text {
            color: #5b7280;
            line-height: 1.55;
            margin-top: 4px;
        }

        .booking-workflow {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 12px;
        }

        .booking-workflow__step {
            border-radius: 12px;
            border: 1px solid #dce7eb;
            background: #ffffff;
            padding: 14px;
            min-height: 132px;
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .booking-workflow__top {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
        }

        .booking-workflow__icon {
            width: 34px;
            height: 34px;
            border-radius: 50%;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            border: 1px solid rgba(0,0,0,.08);
            background: rgba(255,255,255,.7);
        }

        .booking-workflow__state {
            display: inline-flex;
            align-items: center;
            padding: 4px 10px;
            border-radius: 999px;
            font-size: 11px;
            font-weight: 700;
            letter-spacing: .02em;
            text-transform: uppercase;
        }

        .booking-workflow__title {
            color: #17323f;
            font-size: 16px;
            font-weight: 700;
        }

        .booking-workflow__desc {
            color: #5b7280;
            line-height: 1.55;
            flex: 1;
        }

        .booking-workflow__step--done {
            border-color: #bfe3ce;
            background: #f2fbf6;
        }

        .booking-workflow__step--done .booking-workflow__state {
            background: #daf4e4;
            color: #17633d;
        }

        .booking-workflow__step--current {
            border-color: #b7d9ee;
            background: #f2f9fd;
        }

        .booking-workflow__step--current .booking-workflow__state {
            background: #dceffc;
            color: #0c5a86;
        }

        .booking-workflow__step--pending {
            border-color: #dbe4e8;
            background: #f8fbfc;
        }

        .booking-workflow__step--pending .booking-workflow__state {
            background: #edf3f5;
            color: #526873;
        }

        .booking-workflow__step--blocked {
            border-color: #e8c6c6;
            background: #fff6f6;
        }

        .booking-workflow__step--blocked .booking-workflow__state {
            background: #fde1e1;
            color: #9c3434;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <div id="main-content" class="mb-10">
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
                                <asp:TextBox ID="txt_tenkhachhang" runat="server" data-role="input"></asp:TextBox></div>
                            <div class="mt-3">
                                <label class="fw-600">Điện thoại</label>
                                <%--<asp:TextBox ID="txt_sdt" runat="server" data-role="input" OnTextChanged="txt_sdt_TextChanged" AutoPostBack="true"></asp:TextBox>--%>
                                <asp:TextBox ID="txt_sdt" runat="server" data-role="input"></asp:TextBox></div>
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

                    <div class="mt-4 booking-ops-box">
                        <div class="d-flex flex-justify-between flex-align-center flex-wrap">
                            <div>
                                <div class="fw-700">Điều phối vận hành từ lịch hẹn</div>
                                <div class="fg-gray">
                                    Dùng cùng một lõi khách hàng, thẻ dịch vụ và hóa đơn để staff xử lý lịch ngay tại đây.
                                </div>
                            </div>
                            <div class="mt-2">
                                <% if (url_ho_so_khach != "") { %>
                                <a class="button info outline mr-1" href="<%=url_ho_so_khach %>">Mở hồ sơ khách</a>
                                <% } %>
                                <% if (url_sudung_thedv != "" && tongquan_vanhanh.so_thedv_phuhop_dichvu > 0) { %>
                                <a class="button secondary mr-1" href="<%=url_sudung_thedv %>">Vào dùng thẻ</a>
                                <% } %>
                                <a class="button success mr-1" href="<%=url_tao_hoa_don %>">Tạo giao dịch</a>
                                <a class="button warning" href="<%=url_ban_thedv %>">Bán thẻ dịch vụ</a>
                            </div>
                        </div>

                        <div class="mt-4">
                            <div class="d-flex flex-justify-between flex-wrap flex-align-center">
                                <div>
                                    <div class="fw-600">Workflow vận hành</div>
                                    <div class="fg-gray">Hiển thị thẳng mạch xử lý từ đặt lịch tới thu tiền để staff không bỏ sót bước.</div>
                                </div>
                                <% if (goi_y_workflow != "") { %>
                                <div class="mt-2">
                                    <span class="booking-flow-chip bg-light fg-dark mr-1">Gợi ý tiếp theo</span>
                                    <% if (url_goi_y_workflow != "") { %>
                                    <a class="fg-cyan" href="<%=url_goi_y_workflow %>"><%=goi_y_workflow %></a>
                                    <% } else { %>
                                    <span class="fg-dark"><%=goi_y_workflow %></span>
                                    <% } %>
                                </div>
                                <% } %>
                            </div>

                            <div class="booking-workflow mt-3">
                                <% foreach (var step in list_buoc_vanhanh) { %>
                                <div class="booking-workflow__step <%=step.css %>">
                                    <div class="booking-workflow__top">
                                        <span class="booking-workflow__icon"><span class="mif <%=step.icon_css %>"></span></span>
                                        <span class="booking-workflow__state"><%=step.trang_thai %></span>
                                    </div>
                                    <div class="booking-workflow__title"><%=step.tieu_de %></div>
                                    <div class="booking-workflow__desc"><%=step.mo_ta %></div>
                                </div>
                                <% } %>
                            </div>
                        </div>

                        <div class="booking-ops-grid mt-4">
                            <div class="booking-ops-stat">
                                <small>Hồ sơ khách</small>
                                <strong><%=tongquan_vanhanh.co_hoso_khachhang ? "Đã có" : "Chưa có" %></strong>
                                <div class="mt-1">
                                    <%=tongquan_vanhanh.tenkhachhang != "" ? tongquan_vanhanh.tenkhachhang : "Khách mới từ lịch hẹn" %>
                                </div>
                            </div>
                            <div class="booking-ops-stat">
                                <small>Thẻ đang hoạt động</small>
                                <strong><%=tongquan_vanhanh.so_thedv_hoatdong %></strong>
                                <div class="mt-1"><%=tongquan_vanhanh.tong_buoi_conlai %> buổi còn lại</div>
                            </div>
                            <div class="booking-ops-stat">
                                <small>Phù hợp dịch vụ đang chọn</small>
                                <strong><%=tongquan_vanhanh.so_thedv_phuhop_dichvu %></strong>
                                <div class="mt-1">
                                    <%=(ten_dichvu_hien_tai != "" ? ten_dichvu_hien_tai : "Chưa chọn dịch vụ") %>
                                    <% if (ten_dichvu_hien_tai != "") { %>
                                    <span class="d-block mt-1"><%=tongquan_vanhanh.tong_buoi_conlai_phuhop %> buổi phù hợp còn lại</span>
                                    <% } %>
                                </div>
                            </div>
                            <div class="booking-ops-stat">
                                <small>Hóa đơn còn công nợ</small>
                                <strong><%=tongquan_vanhanh.so_hoadon_congno %></strong>
                                <div class="mt-1"><%=tongquan_vanhanh.tong_congno.ToString("#,##0") %> đ</div>
                            </div>
                        </div>

                        <div class="row mt-4">
                            <div class="cell-lg-6 pr-2-lg">
                                <div class="fw-600">Thông tin vận hành khách hàng</div>
                                <ul class="booking-ops-list mt-2">
                                    <li>Nhóm khách: <%=tongquan_vanhanh.ten_nhomkhachhang != "" ? tongquan_vanhanh.ten_nhomkhachhang : "Chưa gán nhóm" %></li>
                                    <li>Nhân viên chăm sóc: <%=tongquan_vanhanh.ten_nguoichamsoc != "" ? tongquan_vanhanh.ten_nguoichamsoc : "Chưa gán" %></li>
                                    <li>Địa chỉ: <%=tongquan_vanhanh.diachi != "" ? tongquan_vanhanh.diachi : "Chưa cập nhật" %></li>
                                </ul>
                            </div>
                            <div class="cell-lg-6 pl-2-lg">
                                <% if (list_hoadon_lienket.Count != 0) { %>
                                <div class="fw-600">Hóa đơn đã đi từ lịch này</div>
                                <ul class="booking-ops-list mt-2">
                                    <% foreach (var item in list_hoadon_lienket) { %>
                                    <li>
                                        <a class="fg-black" href="/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=<%=item.id %>&id_datlich=<%=id %>">Hóa đơn #<%=item.id %></a>
                                        <span class="d-block fg-gray"><%=item.tenkhachhang != "" ? item.tenkhachhang : "Khách hẹn" %>, tạo lúc <%=return_ngaygio_hienthi(item.ngaytao) %></span>
                                        <span class="d-block fg-gray">Sau CK <%=item.tongtien.ToString("#,##0") %> đ, còn nợ <%=item.sotien_conlai.ToString("#,##0") %> đ</span>
                                    </li>
                                    <% } %>
                                </ul>
                                <% } %>

                                <% if (list_thedv_lienket.Count != 0) { %>
                                <div class="fw-600 <%=(list_hoadon_lienket.Count != 0 ? "mt-4" : "") %>">Thẻ dịch vụ đã bán từ lịch này</div>
                                <ul class="booking-ops-list mt-2">
                                    <% foreach (var item in list_thedv_lienket) { %>
                                    <li>
                                        <a class="fg-black" href="/gianhang/admin/quan-ly-the-dich-vu/chi-tiet.aspx?id=<%=item.id %>"><%=item.tenthe != "" ? item.tenthe : "Thẻ DV #" + item.id %></a>
                                        <span class="d-block fg-gray"><%=item.tendichvu != "" ? item.tendichvu : "Chưa có dịch vụ" %>, còn <%=item.sl_conlai %> buổi</span>
                                        <span class="d-block fg-gray">Bán lúc <%=return_ngaygio_hienthi(item.ngaytao) %>, HSD <%=return_ngaygio_hienthi(item.hsd).Replace(" 00:00", "") %></span>
                                    </li>
                                    <% } %>
                                </ul>
                                <% } %>

                                <% if (list_hoadon_lienket.Count == 0 && list_thedv_lienket.Count == 0 && tongquan_vanhanh.list_thedv.Count != 0) { %>
                                <div class="fw-600">Thẻ dịch vụ còn hiệu lực</div>
                                <ul class="booking-ops-list mt-2">
                                    <% foreach (var item in tongquan_vanhanh.list_thedv) { %>
                                    <li>
                                        <a class="fg-black" href="/gianhang/admin/quan-ly-the-dich-vu/chi-tiet.aspx?id=<%=item.id %>"><%=item.tenthe != "" ? item.tenthe : item.tendichvu %></a>
                                        <span class="d-block fg-gray"><%=item.sl_conlai %>/<%=item.tongsoluong %> buổi còn lại, HSD <%=return_ngaygio_hienthi(item.hsd).Replace(" 00:00", "") %></span>
                                    </li>
                                    <% } %>
                                </ul>
                                <% } else if (list_hoadon_lienket.Count == 0 && list_thedv_lienket.Count == 0 && tongquan_vanhanh.list_hoadon_congno.Count != 0) { %>
                                <div class="fw-600">Hóa đơn còn công nợ</div>
                                <ul class="booking-ops-list mt-2">
                                    <% foreach (var item in tongquan_vanhanh.list_hoadon_congno) { %>
                                    <li>
                                        <a class="fg-black" href="/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=<%=item.id %>">Hóa đơn #<%=item.id %></a>
                                        <span class="d-block fg-gray">Còn nợ <%=item.sotien_conlai.ToString("#,##0") %> đ, tạo lúc <%=return_ngaygio_hienthi(item.ngaytao) %></span>
                                    </li>
                                    <% } %>
                                </ul>
                                <% } else if (list_hoadon_lienket.Count == 0 && list_thedv_lienket.Count == 0) { %>
                                <div class="fw-600">Khuyến nghị thao tác</div>
                                <ul class="booking-ops-list mt-2">
                                    <li>Nếu khách mua lẻ, tạo hóa đơn trực tiếp từ lịch này để tiếp tục xử lý dịch vụ.</li>
                                    <li>Nếu khách mua liệu trình, bán thẻ dịch vụ trước để theo dõi số buổi còn lại về sau.</li>
                                </ul>
                                <% } %>
                            </div>
                        </div>

                        <div class="booking-flow-timeline">
                            <div class="d-flex flex-justify-between flex-wrap flex-align-center">
                                <div>
                                    <div class="fw-600">Timeline nhanh của khách</div>
                                    <div class="fg-gray">Lấy thẳng từ lịch hẹn, hóa đơn, thẻ dịch vụ, dùng thẻ và ghi chú để staff quyết định ngay trên màn lịch.</div>
                                </div>
                                <div class="mt-2">
                                    <span class="booking-flow-chip bg-light fg-dark"><%=tongquan_crm.list_timeline.Count.ToString("#,##0") %> mốc gần nhất</span>
                                </div>
                            </div>

                            <% if (return_timeline_datlich().Count == 0) { %>
                            <div class="booking-flow-timeline__item">
                                <div class="booking-flow-timeline__icon"><span class="mif mif-info"></span></div>
                                <div class="booking-flow-timeline__content">
                                    <div class="text-bold">Chưa có lịch sử vận hành</div>
                                    <div class="booking-flow-text">Khách này hiện chưa có thêm hóa đơn, thẻ dịch vụ, dùng thẻ hoặc ghi chú nội bộ để staff tham chiếu.</div>
                                </div>
                            </div>
                            <% } %>

                            <% foreach (var item in return_timeline_datlich()) { %>
                            <div class="booking-flow-timeline__item">
                                <div class="booking-flow-timeline__icon"><span class="mif <%=item.icon_css %>"></span></div>
                                <div class="booking-flow-timeline__content">
                                    <div class="booking-flow-timeline__meta">
                                        <div>
                                            <span class="booking-flow-chip <%=item.loai_css %>"><%=item.loai_hienthi %></span>
                                            <% if (item.trangthai != "") { %>
                                            <span class="booking-flow-chip <%=item.trangthai_css %>"><%=item.trangthai %></span>
                                            <% } %>
                                        </div>
                                        <div class="fg-gray"><%=item.thoigian.HasValue ? item.thoigian.Value.ToString("dd/MM/yyyy HH:mm") : "" %></div>
                                    </div>
                                    <div class="text-bold mt-1"><%=item.tieude %></div>
                                    <div class="booking-flow-text"><%=item.mota %></div>
                                    <% if (item.url_chitiet != "") { %>
                                    <div class="mt-1"><a class="fg-cyan" href="<%=item.url_chitiet %>"><%=item.label_url != "" ? item.label_url : "Xem chi tiết" %></a></div>
                                    <% } %>
                                </div>
                            </div>
                            <% } %>
                        </div>
                    </div>

                    <div class="mt-6 mb-10">
                        <div style="float: left">
                        </div>
                        <div style="float: right">
                            <asp:Button ID="Button3" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="Button3_Click" />
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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <script src="/js/gianhang-invoice-fast.js?v=2026-03-26.2"></script>
    <script>
        (function () {
            function bindFastUi() {
                if (!window.ahaInvoiceFast) return;
                window.ahaInvoiceFast.initCustomerLookup({
                    endpoint: "/gianhang/admin/quan-ly-hoa-don/lookup-data.ashx",
                    phoneId: "<%=txt_sdt.ClientID %>",
                    nameId: "<%=txt_tenkhachhang.ClientID %>"
                });
            }
            bindFastUi();
            if (window.Sys && Sys.Application) {
                Sys.Application.add_load(bindFastUi);
            }
        })();
    </script>
</asp:Content>
