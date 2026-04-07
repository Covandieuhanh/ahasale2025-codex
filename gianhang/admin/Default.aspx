<%@ Page Title="AHASHINE" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="badmin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="/js/chart.3.9.1.js"></script>
    <style>
        .admin-ops-snapshot {
            border: 1px solid #d9e4e8;
            border-radius: 14px;
            background: #f8fbfc;
            padding: 18px;
        }

        .admin-ops-snapshot__head {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
        }

        .admin-ops-snapshot__kpis {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
            gap: 12px;
            margin-top: 16px;
        }

        .admin-ops-snapshot__card {
            border: 1px solid #e2ecef;
            background: #fff;
            border-radius: 12px;
            padding: 14px;
        }

        .admin-ops-snapshot__card small {
            display: block;
            color: #67808b;
            margin-bottom: 6px;
        }

        .admin-ops-snapshot__card strong {
            display: block;
            color: #17323f;
            font-size: 24px;
        }

        .admin-ops-snapshot__sub {
            color: #67808b;
            margin-top: 4px;
        }

        .admin-ops-snapshot__grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 14px;
            margin-top: 16px;
        }

        .admin-ops-snapshot__panel {
            border: 1px solid #e2ecef;
            background: #fff;
            border-radius: 12px;
            padding: 14px;
        }

        .admin-ops-snapshot__title {
            color: #17323f;
            font-size: 15px;
            font-weight: 700;
        }

        .admin-ops-snapshot__caption {
            color: #67808b;
            font-size: 13px;
            margin-top: 2px;
        }

        .admin-ops-list {
            margin-top: 12px;
        }

        .admin-ops-list__item {
            border-top: 1px dashed #e2ecef;
            padding: 12px 0;
        }

        .admin-ops-list__item:first-child {
            border-top: 0;
            padding-top: 0;
        }

        .admin-ops-chip {
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

        .admin-ops-link {
            color: #0f6c8d;
            font-weight: 600;
        }

        .admin-ops-empty {
            color: #6f8691;
            margin-top: 10px;
        }

        .admin-flow {
            border: 1px solid #eadfe2;
            background: #fff7f7;
            border-radius: 16px;
            padding: 18px;
            margin-bottom: 18px;
            box-shadow: 0 12px 24px rgba(175, 32, 32, 0.08);
        }

        .admin-flow__head {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            flex-wrap: wrap;
        }

        .admin-flow__title {
            font-size: 18px;
            font-weight: 800;
            color: #7f1d1d;
        }

        .admin-flow__subtitle {
            color: #9b5b5b;
            font-size: 13px;
        }

        .admin-flow__grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(170px, 1fr));
            gap: 12px;
            margin-top: 16px;
        }

        .admin-flow__card {
            display: grid;
            gap: 6px;
            padding: 12px 14px;
            border-radius: 14px;
            border: 1px solid #f0d7d9;
            background: #fff;
            color: #581818;
            text-decoration: none !important;
            transition: transform .15s ease, box-shadow .15s ease, border-color .15s ease;
        }

        .admin-flow__card small {
            color: #9b5b5b;
            font-weight: 600;
        }

        .admin-flow__card strong {
            font-size: 15px;
            color: #7f1d1d;
        }

        .admin-flow__card:hover {
            border-color: #eab0b5;
            box-shadow: 0 10px 20px rgba(175, 32, 32, 0.12);
            transform: translateY(-2px);
        }

        @media (max-width: 767px) {
            .admin-ops-snapshot__grid {
                grid-template-columns: 1fr;
            }
        }
    
        .kv-kpi-wrap {
            border: 1px solid #ead6d6;
            background: #fff8f8;
            border-radius: 18px;
            padding: 18px;
            margin: 16px 0 18px;
            box-shadow: 0 12px 26px rgba(182, 32, 32, 0.08);
        }
        .kv-kpi-head {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
        }
        .kv-kpi-title {
            font-size: 18px;
            font-weight: 800;
            color: #7f1d1d;
        }
        .kv-kpi-sub {
            color: #8b5b5b;
            font-size: 13px;
        }
        .kv-kpi-actions .button {
            border-radius: 999px !important;
            font-weight: 700;
        }
        .kv-kpi-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(170px, 1fr));
            gap: 12px;
            margin-top: 16px;
        }
        .kv-kpi-card {
            border: 1px solid #f0d7d9;
            background: #fff;
            border-radius: 14px;
            padding: 12px 14px;
            display: grid;
            gap: 6px;
        }
        .kv-kpi-card small {
            color: #8b5b5b;
            font-weight: 600;
        }
        .kv-kpi-card strong {
            font-size: 18px;
            color: #7f1d1d;
        }
        .kv-kpi-card .kv-kpi-note {
            font-size: 12px;
            color: #8b5b5b;
        }
        @media (max-width: 767px) {
            .kv-kpi-actions {
                width: 100%;
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
                        <%--<li><a href="#_data">Dữ liệu</a></li>--%>
                        <li><a href="#_time">Thời gian</a></li>
                        <%--<li><a href="#_sort">Sắp xếp</a></li>--%>
                    </ul>
                    <div class="">
                        <div id="_data">
                            <%--<div class="mt-3">
                                <div class="fw-600">Số lượng hiển thị mỗi trang</div>
                                <asp:TextBox ID="txt_show" MaxLength="6" runat="server" data-role="input" data-clear-button="true"></asp:TextBox>
                            </div>--%>
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
                            <%-- <div class="mt-3">
                                <div class="fw-600">Sắp xếp theo</div>
                                <asp:DropDownList ID="DropDownList2" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Ngày tạo (Tăng dần)" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Ngày tạo (Giảm dần)" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                            </div>--%>
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



    <div id="main-content" class="mb-10">
        <div class="admin-flow">
            <div class="admin-flow__head">
                <div>
                    <div class="admin-flow__title">Luồng vận hành chuẩn Aha Shine</div>
                    <div class="admin-flow__subtitle">Đi theo thứ tự thao tác để vận hành nhanh, ít sai sót.</div>
                </div>
            </div>
            <div class="admin-flow__grid">
                <a class="admin-flow__card" href="/gianhang/admin/quan-ly-hoa-don/Default.aspx">
                    <small>1. Bán hàng</small>
                    <strong>Tạo & quản lý hóa đơn</strong>
                </a>
                <a class="admin-flow__card" href="/gianhang/admin/quan-ly-khach-hang/Default.aspx">
                    <small>2. Khách hàng</small>
                    <strong>Quản lý khách + lịch sử</strong>
                </a>
                <a class="admin-flow__card" href="/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx">
                    <small>3. Lịch hẹn</small>
                    <strong>Đặt lịch & theo dõi</strong>
                </a>
                <a class="admin-flow__card" href="/gianhang/admin/quan-ly-kho-hang/Default.aspx">
                    <small>4. Kho hàng</small>
                    <strong>Nhập hàng · tồn kho</strong>
                </a>
                <a class="admin-flow__card" href="/gianhang/admin/quan-ly-thu-chi/Default.aspx">
                    <small>5. Thu chi</small>
                    <strong>Ghi nhận dòng tiền</strong>
                </a>
                <a class="admin-flow__card" href="/gianhang/admin/quan-ly-hoa-don/thong-ke-dich-vu.aspx">
                    <small>6. Báo cáo</small>
                    <strong>Xem doanh thu nhanh</strong>
                </a>
            </div>
        </div>
        <div class="kv-kpi-wrap">
            <div class="kv-kpi-head">
                <div>
                    <div class="kv-kpi-title">Báo cáo nhanh (chuẩn Aha Shine)</div>
                    <div class="kv-kpi-sub">Từ <%=Session["tungay_thongke_home"] %> đến <%=Session["denngay_thongke_home"] %></div>
                </div>
                <div class="kv-kpi-actions">
                    <a class="button info outline" href="/gianhang/admin/quan-ly-hoa-don/thong-ke-dich-vu.aspx">Báo cáo dịch vụ</a>
                    <a class="button info outline ml-2" href="/gianhang/admin/quan-ly-hoa-don/thong-ke-san-pham.aspx">Báo cáo sản phẩm</a>
                </div>
            </div>
            <div class="kv-kpi-grid">
                <div class="kv-kpi-card">
                    <small>Doanh số (SP/DV/Thẻ/HV)</small>
                    <strong><%=tongdoanhso.ToString("#,##0") %> đ</strong>
                    <div class="kv-kpi-note">Tổng sau chiết khấu</div>
                </div>
                <div class="kv-kpi-card">
                    <small>Thực thu (Thu/Chi)</small>
                    <strong><%=tongthu_tu_thuchi.ToString("#,##0") %> đ</strong>
                    <div class="kv-kpi-note">Tổng phiếu Thu tự động</div>
                </div>
                <div class="kv-kpi-card">
                    <small>Tổng chi</small>
                    <strong><%=tongchi_tu_thuchi.ToString("#,##0") %> đ</strong>
                    <div class="kv-kpi-note">Bao gồm nhập kho & chi khác</div>
                </div>
                <div class="kv-kpi-card">
                    <small>Lợi nhuận tạm tính</small>
                    <strong class="<%=tongloinhuan_css %>"><%=tongloinhuan.ToString("#,##0") %> đ</strong>
                    <div class="kv-kpi-note">Thu - Chi</div>
                </div>
                <div class="kv-kpi-card">
                    <small>Công nợ</small>
                    <strong><%=tongcongno.ToString("#,##0") %> đ</strong>
                </div>
                <div class="kv-kpi-card">
                    <small>Hóa đơn</small>
                    <strong><%=tonghoadon.ToString("#,##0") %></strong>
                    <div class="kv-kpi-note">Số đơn trong kỳ</div>
                </div>
                <div class="kv-kpi-card">
                    <small>Lịch hẹn</small>
                    <strong><%=tonglichhen.ToString("#,##0") %></strong>
                    <div class="kv-kpi-note">Phát sinh trong kỳ</div>
                </div>
                <div class="kv-kpi-card">
                    <small>Khách mới</small>
                    <strong><%=tongkhachmoi.ToString("#,##0") %></strong>
                    <div class="kv-kpi-note">Khách hàng tạo mới</div>
                </div>
            </div>
        </div>

        <div class="row mt-1-minus <%--mt-0-lg-minus mt-12-minus--%>">
            <div class="cell-md-6 order-2 order-md-1 mt-0">
                <h6>Tổng quan</h6>
            </div>
            <div class="cell-md-6 order-1 order-md-2 mt-0">
                <div class="place-right">
                    <ul class="h-menu">
                        <li data-role="hint" data-hint-position="top" data-hint-text="Lọc" onclick="show_hide_id_form_1()">
                            <a class="button"><span class="mif mif-filter"></span></a></li>
                        <%--<li class="bd-gray border bd-default mt-1" style="height: 28px"></li>--%>
                    </ul>
                </div>
                <div class="clr-float"></div>
            </div>
        </div>

        <div class="admin-ops-snapshot mt-4">
            <div class="admin-ops-snapshot__head">
                <div>
                    <div class="text-bold">Snapshot lịch hẹn hôm nay</div>
                    <div class="fg-gray">Mang điều phối trong ngày lên dashboard để staff vào admin là thấy ngay phần cần xử lý.</div>
                </div>
                <div>
                    <span class="admin-ops-chip bg-dark"><%=dieu_phoi_homnay.ngay.ToString("dd/MM/yyyy") %></span>
                    <a class="button info outline ml-2" href="/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx">Mở lịch hẹn</a>
                </div>
            </div>

            <div class="admin-ops-snapshot__kpis">
                <div class="admin-ops-snapshot__card">
                    <small>Tổng lịch</small>
                    <strong><%=dieu_phoi_homnay.tong_lich.ToString("#,##0") %></strong>
                    <div class="admin-ops-snapshot__sub"><%=dieu_phoi_homnay.so_lich_da_ghi_nhan_giaodich.ToString("#,##0") %> lịch đã phát sinh giao dịch</div>
                </div>
                <div class="admin-ops-snapshot__card">
                    <small>Cần xác nhận</small>
                    <strong><%=dieu_phoi_homnay.so_lich_cho_xacnhan.ToString("#,##0") %></strong>
                    <div class="admin-ops-snapshot__sub"><%=dieu_phoi_homnay.so_lich_sap_toi_2gio.ToString("#,##0") %> lịch sắp tới trong 2 giờ</div>
                </div>
                <div class="admin-ops-snapshot__card">
                    <small>Quá giờ</small>
                    <strong><%=dieu_phoi_homnay.so_lich_qua_gio_chua_xuly.ToString("#,##0") %></strong>
                    <div class="admin-ops-snapshot__sub"><%=dieu_phoi_homnay.so_lich_da_den.ToString("#,##0") %> lịch đã được chốt đến</div>
                </div>
                <div class="admin-ops-snapshot__card">
                    <small>Chưa phân công</small>
                    <strong><%=dieu_phoi_homnay.so_lich_chua_phan_cong.ToString("#,##0") %></strong>
                    <div class="admin-ops-snapshot__sub"><%=dieu_phoi_homnay.so_lich_chua_co_hoso.ToString("#,##0") %> lịch chưa có hồ sơ CRM</div>
                </div>
            </div>

            <div class="admin-ops-snapshot__grid">
                <div class="admin-ops-snapshot__panel">
                    <div class="admin-ops-snapshot__title">Nhân viên cần nhìn trước</div>
                    <div class="admin-ops-snapshot__caption">Ưu tiên người có lịch quá giờ hoặc lịch sắp tới cần xử lý.</div>
                    <% if (dieu_phoi_homnay.list_nhanvien.Count == 0) { %>
                    <div class="admin-ops-empty">Chưa có lịch nào được gán nhân viên trong ngày.</div>
                    <% } else { %>
                    <div class="admin-ops-list">
                        <% foreach (var item in return_top_nhanvien_dieu_phoi()) { %>
                        <div class="admin-ops-list__item">
                            <div class="d-flex flex-justify-between flex-align-center flex-wrap">
                                <div class="text-bold"><%=item.hoten %></div>
                                <span class="admin-ops-chip <%=item.muc_tai_css %>"><%=item.muc_tai %></span>
                            </div>
                            <div class="fg-gray mt-1"><%=item.ten_bophan %> | <%=item.tong_lich.ToString("#,##0") %> lịch | <%=item.so_lich_cho_xacnhan.ToString("#,##0") %> chờ xác nhận</div>
                            <% if (item.id_lich_gan_nhat != "") { %>
                            <div class="mt-1"><a class="admin-ops-link" href="/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=<%=item.id_lich_gan_nhat %>">Lịch gần nhất #<%=item.id_lich_gan_nhat %></a></div>
                            <% } %>
                        </div>
                        <% } %>
                    </div>
                    <% } %>
                </div>

                <div class="admin-ops-snapshot__panel">
                    <div class="admin-ops-snapshot__title">Cảnh báo nóng</div>
                    <div class="admin-ops-snapshot__caption">Mở nhanh đúng lịch cần xử lý thay vì phải lọc lại từ đầu.</div>
                    <% if (dieu_phoi_homnay.list_canhbao.Count == 0) { %>
                    <div class="admin-ops-empty">Hiện không có cảnh báo nóng trong ngày.</div>
                    <% } else { %>
                    <div class="admin-ops-list">
                        <% foreach (var item in return_top_canhbao_dieu_phoi()) { %>
                        <div class="admin-ops-list__item">
                            <div class="d-flex flex-justify-between flex-align-center flex-wrap">
                                <span class="admin-ops-chip <%=item.loai_css %>"><%=item.loai %></span>
                                <div class="fg-gray"><%=item.ngaydat.HasValue ? item.ngaydat.Value.ToString("HH:mm") : "" %></div>
                            </div>
                            <div class="mt-1"><a class="admin-ops-link" href="<%=item.url %>"><%=item.tieu_de %></a></div>
                            <div class="fg-gray mt-1"><%=item.mo_ta %></div>
                        </div>
                        <% } %>
                    </div>
                    <% } %>
                </div>
            </div>
        </div>

        <%if (bcorn_class.check_quyen(user, "q0_1") == "")
            { %>
        <asp:Repeater ID="Repeater4" runat="server">
            <ItemTemplate>

                <div class="mt-10">
                    <div data-role="panel" data-collapsible="true"
                        data-title-caption="Ngành <%#Eval("ten") %>"
                        data-title-icon="<span class='mif-joomla'></span>">
                        <div class="p-2">
                            <div class="row">
                                <div class="cell-lg-12">
                                    <%--<div class="text-bold">Tổng doanh số: 10.000.000</div>
                                    <div data-role="progress" data-type="buffer" data-cls-back="bg-orange" data-cls-bar="bg-cyan" data-value="70" data-buffer="30"></div>
                                    <div class="text-small">
                                        <div class="place-left">Đã thu: 7.000.000</div>
                                        <div class="place-right">Công nợ: 3.000.000</div>
                                        <div class="clr-bc"></div>
                                    </div>--%>
                                    <table class="table row-hover <%--table-border--%> compact striped mt-4">
                                        <thead>
                                            <tr>
                                                <td>Hạng mục</td>
                                                <td class="text-right fg-cyan">Doanh số</td>
                                                <td class="text-right fg-green">Doanh thu</td>
                                                <td class="text-right fg-orange">Công nợ</td>
                                                <%--<td class="text-right">Công nợ</td>--%>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td>Sản phẩm - Dịch vụ</td>
                                                <td class="text-right fg-cyan"><%#tinhdoanhso_hoadon_nganh(Eval("id").ToString()).ToString("#,##0")%></td>
                                                <td class="text-right fg-green"><%#tinhdoanhthu_hoadon_nganh(Eval("id").ToString()).ToString("#,##0")%></td>
                                                <td class="text-right fg-orange"><%#tinhcongno_hoadon_nganh(Eval("id").ToString()).ToString("#,##0")%></td>
                                            </tr>
                                            <tr>
                                                <td>Thẻ dịch vụ</td>
                                                <td class="text-right fg-cyan"><%#tinhdoanhso_thedichvu_nganh(Eval("id").ToString()).ToString("#,##0") %></td>
                                                <td class="text-right fg-green"><%#tinhdoanhthu_thedichvu_nganh(Eval("id").ToString()).ToString("#,##0") %></td>
                                                <td class="text-right fg-orange"><%#tinhcongno_thedichvu_nganh(Eval("id").ToString()).ToString("#,##0") %></td>
                                            </tr>
                                            <tr>
                                                <td>thành viên</td>
                                                <td class="text-right fg-cyan"><%#tinhdoanhso_hocvien_nganh(Eval("id").ToString()).ToString("#,##0") %></td>
                                                <td class="text-right fg-green"><%#tinhdoanhthu_hocvien_nganh(Eval("id").ToString()).ToString("#,##0") %></td>
                                                <td class="text-right fg-orange"><%#tinhcongno_hocvien_nganh(Eval("id").ToString()).ToString("#,##0") %></td>
                                            </tr>

                                        </tbody>
                                        <tfoot>
                                            <tr>
                                                <td class="text-right"></td>
                                                <td class="text-right fg-cyan"><%#(tinhdoanhso_hoadon_nganh(Eval("id").ToString()) +tinhdoanhso_thedichvu_nganh(Eval("id").ToString())+tinhdoanhso_hocvien_nganh(Eval("id").ToString())).ToString("#,##0") %></td>
                                                <td class="text-right fg-green"><%#(tinhdoanhthu_hoadon_nganh(Eval("id").ToString()) +tinhdoanhthu_thedichvu_nganh(Eval("id").ToString())+tinhdoanhthu_hocvien_nganh(Eval("id").ToString())).ToString("#,##0") %></td>
                                                <td class="text-right fg-orange"><%#(tinhcongno_hoadon_nganh(Eval("id").ToString()) +tinhcongno_thedichvu_nganh(Eval("id").ToString())+tinhcongno_hocvien_nganh(Eval("id").ToString())).ToString("#,##0") %></td>

                                            </tr>
                                        </tfoot>
                                    </table>
                                </div>
                                <%--<div class="cell-lg-5 mt-0-lg mt-3">
                                    <div style="max-width: 260px; margin: 0 auto;">
                                        <canvas id="myChart<%#Eval("id") %>"></canvas>
                                        <div class="text-center text-bold">Tổng doanh số: 10.000.000</div>
                                    </div>
                                </div>--%>
                            </div>
                        </div>
                    </div>
                </div>
                <%--<script>
                    const data<%#Eval("id") %> = {
                        labels: [
                            'Bán SP-DV',
                            'Bán thẻ DV',
                            'thành viên'
                        ],
                        datasets: [{
                            label: 'My First Dataset',
                            data: [3000000, 2000000, 5000000],
                            backgroundColor: [
                                'rgb(255, 99, 100)',
                                'rgb(154, 162, 178)',
                                'rgb(100, 205, 86)'
                            ],
                            hoverOffset: 4
                        }]
                    };
                    const config<%#Eval("id") %> = {
                        type: 'doughnut',
                        data: data<%#Eval("id") %>,

                    };
                    const myChart<%#Eval("id") %> = new Chart(
                        document.getElementById('myChart<%#Eval("id") %>'),
                        config<%#Eval("id") %>
                    );
                </script>--%>
            </ItemTemplate>
        </asp:Repeater>
        <%} %>

        <hr />


        <hr />

        <div class="row">
            <%if (bcorn_class.check_quyen(user, "q0_2") == "")
                { %>
            <div class="cell-lg-4 mt-8">
                <div class="skill-box">
                    <div class="header bg-cyan fg-white">
                        <%-- <img src="images/jek_vorobey.jpg" class="avatar">--%>
                        <div class="title" style="margin-left: -4px">Dịch vụ nổi bật</div>
                        <div style="position: absolute; right: 7px; top: 27px"><a href="/gianhang/admin/quan-ly-hoa-don/thong-ke-dich-vu.aspx" data-role="hint" data-hint-position="top" data-hint-text="Xem thêm"><span class="mif mif-chevron-thin-right fg-white"></span></a></div>
                        <%--<div class="subtitle">Pirate captain</div>--%>
                    </div>
                    <ul class="skills">
                        <asp:Repeater ID="Repeater1" runat="server">
                            <ItemTemplate>
                                <li>
                                    <span><%#Eval("tendvsp") %></span>
                                    <span class="badge bg-cyan fg-white"><%#Eval("tongsl","{0:#,##0}") %></span>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>

                    </ul>
                </div>
            </div>
            <%} %>
            <%if (bcorn_class.check_quyen(user, "q0_3") == "")
                { %>
            <div class="cell-lg-4  mt-8">
                <div class="skill-box">
                    <div class="header bg-green fg-white">
                        <%--<img src="images/jek_vorobey.jpg" class="avatar">--%>
                        <div class="title" style="margin-left: -4px">Sản phẩm bán chạy</div>
                        <div style="position: absolute; right: 7px; top: 27px"><a href="/gianhang/admin/quan-ly-hoa-don/thong-ke-san-pham.aspx" data-role="hint" data-hint-position="top" data-hint-text="Xem thêm"><span class="mif mif-chevron-thin-right fg-white"></span></a></div>
                        <%--<div class="subtitle">Pirate captain</div>--%>
                    </div>
                    <ul class="skills">
                        <asp:Repeater ID="Repeater2" runat="server">
                            <ItemTemplate>
                                <li>
                                    <span><%#Eval("tendvsp") %></span>
                                    <span class="badge bg-green fg-white"><%#Eval("tongsl","{0:#,##0}") %></span>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>
            </div>
            <%} %>
            <%if (bcorn_class.check_quyen(user, "q0_4") == "")
                { %>
            <div class="cell-lg-4  mt-8">
                <div class="skill-box">
                    <div class="header bg-orange fg-white">
                        <%--<img src="images/jek_vorobey.jpg" class="avatar">--%>
                        <div class="title" style="margin-left: -4px">Top làm dịch vụ</div>
                        <div style="position: absolute; right: 7px; top: 27px"><a href="#" data-role="hint" data-hint-position="top" data-hint-text="Xem thêm"><span class="mif mif-chevron-thin-right fg-white"></span></a></div>
                        <%--<div class="subtitle">Pirate captain</div>--%>
                    </div>
                    <ul class="skills">
                        <asp:Repeater ID="Repeater3" runat="server">
                            <ItemTemplate>
                                <li>
                                    <span><%#return_fullname(Eval("username").ToString()) %></span>
                                    <span class="badge bg-orange fg-white">
                                        <%#Eval("soluong","{0:#,##0}") %>
                                        <%--<a href="/gianhang/admin/quan-ly-dich-vu/lich-su.aspx?nl=<%#Eval("username") %>" class="fg-white" data-role="hint" data-hint-position="top" data-hint-text="Chi tiết">
                                            <%#Eval("soluong","{0:#,##0}") %>
                                        </a>--%>
                                    </span>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>
            </div>
            <%} %>
        </div>

    </div>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">


    <%--<%=notifi %>--%>
</asp:Content>
