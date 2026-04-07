<%@ Page Title="Thông tin khách hàng" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="chi-tiet.aspx.cs" Inherits="taikhoan_add" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .booking-link-box {
            border: 1px solid #b8d9e8;
            border-radius: 12px;
            background: #f4fbff;
            padding: 16px;
        }

        .booking-link-box__title {
            font-weight: 700;
            color: #163645;
        }

        .booking-link-box__meta {
            color: #56707d;
        }

        .crm-kpi-grid {
            display: flex;
            flex-wrap: wrap;
            margin: 18px -8px 0;
        }

        .crm-kpi-cell {
            width: 25%;
            padding: 8px;
        }

        .crm-kpi {
            height: 100%;
            border: 1px solid #d7e1e8;
            border-radius: 14px;
            background: #fff;
            padding: 18px;
            box-shadow: 0 12px 28px rgba(22, 54, 69, 0.06);
        }

        .crm-kpi__eyebrow {
            font-size: 11px;
            font-weight: 700;
            letter-spacing: .08em;
            text-transform: uppercase;
            color: #5f7987;
        }

        .crm-kpi__value {
            margin-top: 8px;
            font-size: 28px;
            font-weight: 700;
            line-height: 1.15;
            color: #163645;
        }

        .crm-kpi__desc {
            margin-top: 8px;
            color: #56707d;
            line-height: 1.55;
        }

        .crm-kpi--cyan {
            border-color: #b8d9e8;
            background: linear-gradient(180deg, #f4fbff 0%, #ffffff 100%);
        }

        .crm-kpi--green {
            border-color: #b8e5d0;
            background: linear-gradient(180deg, #f4fff8 0%, #ffffff 100%);
        }

        .crm-kpi--orange {
            border-color: #f3d6b3;
            background: linear-gradient(180deg, #fff9f2 0%, #ffffff 100%);
        }

        .crm-kpi--teal {
            border-color: #b8e0dd;
            background: linear-gradient(180deg, #f3fffe 0%, #ffffff 100%);
        }

        .crm-kpi--slate {
            border-color: #d7e1e8;
            background: linear-gradient(180deg, #fbfdff 0%, #ffffff 100%);
        }

        .crm-timeline {
            border: 1px solid #d7e1e8;
            border-radius: 16px;
            background: #fff;
            padding: 18px;
            box-shadow: 0 12px 28px rgba(22, 54, 69, 0.05);
        }

        .crm-timeline__head {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
        }

        .crm-timeline__title {
            font-size: 18px;
            font-weight: 700;
            color: #163645;
        }

        .crm-timeline__sub {
            color: #68808d;
        }

        .crm-timeline__list {
            margin-top: 10px;
        }

        .crm-timeline__item {
            display: flex;
            gap: 14px;
            padding: 14px 0;
            border-top: 1px solid #edf2f5;
        }

        .crm-timeline__item:first-child {
            border-top: 0;
        }

        .crm-timeline__icon {
            width: 42px;
            height: 42px;
            flex: 0 0 42px;
            border-radius: 50%;
            background: #f3f8fb;
            color: #23708a;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .crm-timeline__content {
            flex: 1;
            min-width: 0;
        }

        .crm-timeline__top {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
        }

        .crm-timeline__text {
            margin-top: 6px;
            color: #58717e;
            line-height: 1.55;
        }

        .crm-chip {
            display: inline-flex;
            align-items: center;
            padding: 3px 10px;
            border-radius: 999px;
            font-size: 11px;
            font-weight: 700;
            letter-spacing: .03em;
            text-transform: uppercase;
        }

        .crm-chip--cyan {
            background: #dff5ff;
            color: #14607c;
        }

        .crm-chip--green {
            background: #e2f8ea;
            color: #1d6a44;
        }

        .crm-chip--orange {
            background: #fff0de;
            color: #9a5a10;
        }

        .crm-chip--magenta {
            background: #f6e2f7;
            color: #893a90;
        }

        .crm-chip--red {
            background: #ffe3e3;
            color: #9a1e1e;
        }

        .crm-chip--teal {
            background: #def7f5;
            color: #126f69;
        }

        .crm-chip--violet {
            background: #ebe6ff;
            color: #5e46af;
        }

        .crm-chip--slate {
            background: #edf2f5;
            color: #506671;
        }

        .crm-mini-actions a {
            margin-right: 6px;
            margin-top: 6px;
            display: inline-block;
        }

        @media (max-width: 767px) {
            .crm-kpi-cell {
                width: 100%;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <%if (bcorn_class.check_quyen(user, "q8_3") == "")
        { %>
    <div id='form_add_ghichu' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_add_ghichu()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Tạo ghi chú</h5>
                <hr />
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel2" runat="server" DefaultButton="Button3">

                            <div class="row">
                                <div class="cell-lg-12">
                                    <div class="mt-3">
                                        <label class="fw-600">Nội dung ghi chú</label>
                                        <asp:TextBox ID="txt_noidung_ghichu" runat="server" data-role="textarea" TextMode="MultiLine"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="mt-6 mb-10">
                                <div style="float: left">
                                    <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="Button3" runat="server" Text="TẠO GHI CHÚ" CssClass="button success" OnClick="Button3_Click" />
                                </div>
                                <div style="clear: both"></div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanel4">
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
        function show_hide_id_form_add_ghichu() {
            var x = document.getElementById("form_add_ghichu");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>
    <%} %>

    <%if (bcorn_class.check_quyen(user, "q8_3") == "")
        { %>
    <div id='form_add_donthuoc' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 600px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_add_donthuoc()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Tạo đơn thuốc</h5>
                <hr />
                <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel3" runat="server" DefaultButton="Button4">

                            <div class="row">
                                <div class="cell-lg-12">
                                    <div class="mt-3">
                                        <label class="fw-600">Nội dung đơn thuốc</label>
                                        <CKEditor:CKEditorControl ID="txt_ghichu_donthuoc" runat="server" Height="120"></CKEditor:CKEditorControl>
                                        <script src="/ckeditor/ckeditor.js"></script>
                                        <script> 
                                            CKEDITOR.replace('txt_ghichu_donthuoc');
                                            CKEDITOR.config.toolbar = [['Bold', 'Italic', 'Underline']];
                                            CKEDITOR.config.height = 120;
                                    /*CKEDITOR.config.toolbar = [];*/
                                        </script>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600">Ngày tái khám</label>
                                        <asp:TextBox ID="txt_ngaytaikham" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Nơi tái khám</label>
                                        <asp:TextBox ID="txt_noitamkham" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Lời dặn bác sĩ</label>
                                        <asp:TextBox ID="txt_loidanbacsi" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="mt-6 mb-10">
                                <div style="float: left">
                                    <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                </div>
                                <div style="float: right">

                                    <asp:Button ID="Button4" runat="server" Text="TẠO ĐƠN THUỐC" CssClass="button success" OnClick="Button4_Click" />

                                </div>
                                <div style="clear: both"></div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel6">
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
        function show_hide_id_form_add_donthuoc() {
            var x = document.getElementById("form_add_donthuoc");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>
    <%} %>

    <%if (bcorn_class.check_quyen(user, "q8_3") == "")
        { %>
    <div id='form_add_hinhanh' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 600px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_add_hinhanh()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Thêm ảnh trước sau</h5>
                <hr />

                <asp:Panel ID="Panel4" runat="server" DefaultButton="Button4">

                    <div class="row">
                        <div class="cell-lg-12">
                            <div class="mt-3">
                                <label class="fw-600">Ảnh trước</label>
                                <asp:FileUpload ID="FileUpload1" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Ảnh sau</label>
                                <asp:FileUpload ID="FileUpload3" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Ghi chú</label>
                                <asp:TextBox ID="txt_ghichu_hinhanh_truocsau" runat="server" data-role="input"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="mt-6 mb-10">
                        <div style="float: left">
                            <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                        </div>
                        <div style="float: right">

                            <asp:Button ID="them_hinhanh_truocsau" runat="server" Text="THÊM HÌNH ẢNH" CssClass="button success" OnClick="them_hinhanh_truocsau_Click" />

                        </div>
                        <div style="clear: both"></div>
                    </div>
                </asp:Panel>

            </div>

        </div>
    </div>
    <script>
        function show_hide_id_form_add_hinhanh() {
            var x = document.getElementById("form_add_hinhanh");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>
    <%} %>

    <div id="main-content" class=" mb-10">
        <div class="row border-bottom bd-light">
            <div class="cell-6">
                <ul class="h-menu">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Quay lại">
                        <a class="button" href="/gianhang/admin/quan-ly-khach-hang/Default.aspx"><span class="mif mif-arrow-left"></span></a></li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>

                </ul>
            </div>
            <div class="cell-6 text-right">
                <ul class="h-menu place-right">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Đặt lịch">
                        <a class="button" href="/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx?q=add&amp;sdt=<%=sdt %>&amp;tenkh=<%=Server.UrlEncode(hoten) %>">
                            <span class="mif mif-calendar"></span>Đặt lịch</a>
                    </li>
                    <li data-role="hint" data-hint-position="top" data-hint-text="Tạo giao dịch">
                        <a class="button" href='/gianhang/admin/gianhang/tao-giao-dich.aspx?sdt=<%=sdt %>&amp;tenkh=<%=Server.UrlEncode(hoten) %>&amp;return_url=<%=Server.UrlEncode(Request.RawUrl ?? "/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx") %>'>
                            <span class="mif mif-open-book"></span>Tạo giao dịch</a>
                    </li>
                    <li data-role="hint" data-hint-position="top" data-hint-text="Bán thẻ dịch vụ">
                        <a class="button" href="/gianhang/admin/quan-ly-the-dich-vu/Default.aspx?q=add&amp;sdt=<%=sdt %>&amp;tenkh=<%=Server.UrlEncode(hoten) %>">
                            <span class="mif mif-credit-card"></span>Bán thẻ DV</a>
                    </li>
                </ul>
            </div>
        </div>

        <div class="row">
            <div class="cell-lg-12 mt-5">
                <div class="text-center">
                    <div style="">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>
                                <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="text-bold mt-1">
                        <%=hoten %>
                    </div>
                    <div class="text-bold">
                        <a data-role="hint" data-hint-position="top" data-hint-text="Nhấn để gọi" href="tel:<%=sdt %>">
                            <span class="mif mif-phone"></span><%=sdt %>
                        </a>
                    </div>
                </div>
                <% if (co_ngu_canh_datlich) { %>
                <div class="booking-link-box mt-5">
                    <div class="d-flex flex-justify-between flex-wrap flex-align-center">
                        <div>
                            <div class="booking-link-box__title">Khách này đang được xử lý từ lịch hẹn #<%=id_datlich_lienket %></div>
                            <div class="booking-link-box__meta mt-1">
                                Dịch vụ: <%=ten_dichvu_datlich != "" ? ten_dichvu_datlich : "Chưa có dịch vụ" %>
                                <% if (ngay_datlich_lienket != "") { %> | Giờ hẹn: <%=ngay_datlich_lienket %><% } %>
                                <% if (ten_nhanvien_datlich != "") { %> | Nhân viên gợi ý: <%=ten_nhanvien_datlich %><% } %>
                            </div>
                        </div>
                        <div class="mt-2">
                            <a class="button info outline" href="<%=url_quay_lai_datlich %>">Quay lại lịch hẹn</a>
                        </div>
                    </div>
                </div>
                <% } %>
                <div class="crm-kpi-grid">
                    <div class="crm-kpi-cell">
                        <div class="crm-kpi crm-kpi--cyan">
                            <div class="crm-kpi__eyebrow">Lịch hẹn</div>
                            <div class="crm-kpi__value"><%=tongquan_crm.so_lich_sap_toi.ToString("#,##0") %></div>
                            <div class="crm-kpi__desc">
                                <b><%=tongquan_crm.so_lich_chua_xacnhan.ToString("#,##0") %></b> lịch chưa xác nhận
                                <% if (tongquan_crm.id_lich_gannhat != "") { %>
                                <div class="mt-1">
                                    Gần nhất: <a class="fg-cyan" href="/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=<%=tongquan_crm.id_lich_gannhat %>">#<%=tongquan_crm.id_lich_gannhat %></a>
                                    <% if (tongquan_crm.ten_dichvu_lich_gannhat != "") { %> - <%=tongquan_crm.ten_dichvu_lich_gannhat %><% } %>
                                </div>
                                <% } %>
                            </div>
                        </div>
                    </div>
                    <div class="crm-kpi-cell">
                        <div class="crm-kpi crm-kpi--orange">
                            <div class="crm-kpi__eyebrow">Công nợ</div>
                            <div class="crm-kpi__value"><%=tongquan_crm.tong_congno.ToString("#,##0") %></div>
                            <div class="crm-kpi__desc">
                                <b><%=tongquan_crm.so_hoadon_congno.ToString("#,##0") %></b> hóa đơn chưa thu hết
                                <% if (tongquan_crm.id_hoadon_gannhat != "") { %>
                                <div class="mt-1">Hóa đơn gần nhất: <a class="fg-orange" href="/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=<%=tongquan_crm.id_hoadon_gannhat %>">#<%=tongquan_crm.id_hoadon_gannhat %></a></div>
                                <% } %>
                            </div>
                        </div>
                    </div>
                    <div class="crm-kpi-cell">
                        <div class="crm-kpi crm-kpi--teal">
                            <div class="crm-kpi__eyebrow">Thẻ Dịch Vụ</div>
                            <div class="crm-kpi__value"><%=tongquan_crm.tong_buoi_conlai.ToString("#,##0") %></div>
                            <div class="crm-kpi__desc"><b><%=tongquan_crm.so_thedv_hoatdong.ToString("#,##0") %></b> thẻ còn hiệu lực và còn buổi để dùng tiếp.</div>
                        </div>
                    </div>
                    <div class="crm-kpi-cell">
                        <div class="crm-kpi <%=tongquan_crm.hanh_dong_css %>">
                            <div class="crm-kpi__eyebrow">Hành động gợi ý</div>
                            <div class="crm-kpi__value" style="font-size: 20px;"><%=tongquan_crm.hanh_dong_goi_y %></div>
                            <div class="crm-kpi__desc">
                                Từ dữ liệu hiện có, đây là bước staff nên làm tiếp theo.
                                <% if (tongquan_crm.url_hanh_dong_goi_y != "") { %>
                                <div class="mt-3"><a class="button primary" href="<%=tongquan_crm.url_hanh_dong_goi_y %>">Mở nhanh</a></div>
                                <% } %>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="crm-timeline mt-4">
                    <div class="crm-timeline__head">
                        <div>
                            <div class="crm-timeline__title">Timeline vận hành khách hàng</div>
                            <div class="crm-timeline__sub">Gom lịch hẹn, hóa đơn, thẻ dịch vụ, dùng thẻ và ghi chú nội bộ theo đúng dữ liệu đang có.</div>
                        </div>
                        <div class="crm-chip crm-chip--slate"><%=tongquan_crm.list_timeline.Count.ToString("#,##0") %> mốc gần nhất</div>
                    </div>
                    <div class="crm-timeline__list">
                        <% if (tongquan_crm.list_timeline.Count == 0) { %>
                        <div class="crm-timeline__item">
                            <div class="crm-timeline__icon"><span class="mif mif-info"></span></div>
                            <div class="crm-timeline__content">
                                <div class="text-bold">Chưa có dữ liệu vận hành</div>
                                <div class="crm-timeline__text">Khách này hiện chưa phát sinh lịch hẹn, hóa đơn, thẻ dịch vụ hoặc ghi chú nội bộ.</div>
                            </div>
                        </div>
                        <% } %>
                        <% foreach (khachhang_vanhanh_timeline_item item in tongquan_crm.list_timeline) { %>
                        <div class="crm-timeline__item">
                            <div class="crm-timeline__icon"><span class="mif <%=item.icon_css %>"></span></div>
                            <div class="crm-timeline__content">
                                <div class="crm-timeline__top">
                                    <div>
                                        <span class="crm-chip <%=item.loai_css %>"><%=item.loai_hienthi %></span>
                                        <% if (item.trangthai != "") { %>
                                        <span class="crm-chip <%=item.trangthai_css %>"><%=item.trangthai %></span>
                                        <% } %>
                                    </div>
                                    <div class="fg-gray"><%=item.thoigian.HasValue ? item.thoigian.Value.ToString("dd/MM/yyyy HH:mm") : "" %></div>
                                </div>
                                <div class="text-bold mt-1"><%=item.tieude %></div>
                                <div class="crm-timeline__text"><%=item.mota %></div>
                                <% if (item.url_chitiet != "") { %>
                                <div class="crm-mini-actions">
                                    <a class="fg-cyan" href="<%=item.url_chitiet %>"><%=item.label_url != "" ? item.label_url : "Xem chi tiết" %></a>
                                </div>
                                <% } %>
                            </div>
                        </div>
                        <% } %>
                    </div>
                </div>
            </div>
            <div class="cell-lg-12 mt-5">
                <ul data-role="tabs" data-expand="true">
                    <li><a href="#_khachhang">Khách hàng</a></li>
                    <li><a href="#_hoadon">Hóa đơn</a></li>
                    <li><a href="#_dathen">Đặt hẹn</a></li>
                    <li><a href="#_ghichu">Ghi chú</a></li>
                    <li class="<%=act_hinhanh %>"><a href="#_hinhanh">Hình ảnh</a></li>
                    <li><a href="#_donthuoc">Đơn thuốc</a></li>
                    <li class="<%=act_thedv %>"><a href="#_thedichvu">Thẻ dịch vụ</a></li>
                </ul>
                <div class="">
                    <div id="_khachhang">
                        <asp:Panel ID="Panel1" runat="server" DefaultButton="button1">

                            <div class="row">

                                <div class="cell-lg-6 pr-3-lg">

                                    <div class="mt-3">
                                        <label class="fw-600">Tên khách hàng</label>
                                        <div>
                                            <asp:TextBox ID="txt_hoten" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Điện thoại</label>
                                        <div>
                                            <asp:TextBox ID="txt_dienthoai" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>

                                    <!-- BỔ SUNG EMAIL -->
                                    <div class="mt-3">
                                        <label class="fw-600">Email</label>
                                        <div>
                                            <asp:TextBox ID="txt_email" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <!-- /BỔ SUNG EMAIL -->

                                    <div class="booking-link-box mt-4">
                                        <div class="booking-link-box__title">Hồ sơ người AhaSale</div>
                                        <div class="booking-link-box__meta mt-1">
                                            Việc liên kết Home đã được chuyển sang module trung tâm Hồ sơ người. Chỉ cần gắn 1 lần ở đó là các vai trò cùng số điện thoại trong gian hàng này sẽ tự nhận liên kết.
                                        </div>
                                        <div class="mt-3">
                                            <span class="data-wrapper"><code class="<%=HttpUtility.HtmlAttributeEncode(personHubStatusCss) %>"><%=HttpUtility.HtmlEncode(personHubStatusLabel) %></code></span>
                                        </div>
                                        <div class="mt-2 fg-gray"><%=HttpUtility.HtmlEncode(personHubNote) %></div>
                                        <div class="mt-3">
                                            <span class="data-wrapper"><code class="<%=HttpUtility.HtmlAttributeEncode(personHubAdminAccessCss) %>"><%=HttpUtility.HtmlEncode(personHubAdminAccessLabel) %></code></span>
                                        </div>
                                        <div class="mt-2 fg-gray"><%=HttpUtility.HtmlEncode(personHubAdminAccessNote) %></div>
                                        <div class="mt-3">
                                            <span class="data-wrapper"><code class="<%=HttpUtility.HtmlAttributeEncode(sourceLifecycleCss) %>"><%=HttpUtility.HtmlEncode(sourceLifecycleLabel) %></code></span>
                                        </div>
                                        <div class="mt-2 fg-gray"><%=HttpUtility.HtmlEncode(sourceLifecycleNote) %></div>
                                        <div class="mt-3 p-3 border" style="border-radius: 12px; border-color: #cfe6ff!important; background: #f7fbff;">
                                            <div class="booking-link-box__title">Ngừng dùng an toàn</div>
                                            <div class="booking-link-box__meta mt-1">
                                                Ngừng dùng khách hàng sẽ không xóa lịch sử, không gỡ liên kết Home ở Hồ sơ người và không làm mất các vai trò khác cùng số điện thoại trong gian hàng này.
                                            </div>
                                            <div class="mt-3">
                                                <% if (!sourceLifecycleIsInactive) { %>
                                                    <asp:Button ID="but_ngung_khachhang" runat="server" Text="NGỪNG DÙNG KHÁCH HÀNG" CssClass="button warning" OnClick="but_ngung_khachhang_Click" />
                                                <% } else { %>
                                                    <asp:Button ID="but_molai_khachhang" runat="server" Text="MỞ LẠI KHÁCH HÀNG" CssClass="button success" OnClick="but_molai_khachhang_Click" />
                                                <% } %>
                                            </div>
                                        </div>
                                        <div class="mt-3 p-3 border" style="border-radius: 12px; border-color: #f3d6b3!important; background: #fff9f2;">
                                            <div class="booking-link-box__title"><%=HttpUtility.HtmlEncode(personHubImpactTitle) %></div>
                                            <div class="booking-link-box__meta mt-1"><%=HttpUtility.HtmlEncode(personHubImpactNote) %></div>
                                        </div>
                                        <div class="mt-3">
                                            <a class="button success" href="<%=HttpUtility.HtmlAttributeEncode(personHubUrl) %>">Mở hồ sơ người</a>
                                        </div>
                                        <% if (!string.IsNullOrWhiteSpace(personHubRelatedRolesHtml)) { %>
                                        <div class="mt-3">
                                            <div class="booking-link-box__title">Cùng số điện thoại này còn có thêm vai trò khác</div>
                                            <div class="booking-link-box__meta mt-1">
                                                Chỉ cần gắn Home ở Hồ sơ người là các hồ sơ dưới đây cũng sẽ nhận cùng trạng thái liên kết trong không gian này.
                                            </div>
                                            <div class="mt-2">
                                                <%=personHubRelatedRolesHtml %>
                                            </div>
                                        </div>
                                        <% } %>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600">Ngày sinh</label>
                                        <div>
                                            <asp:TextBox ID="txt_ngaysinh" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Địa chỉ</label>
                                        <asp:TextBox ID="txt_diachi" runat="server" data-role="textarea" TextMode="MultiLine"></asp:TextBox><%--autocomplete="off" --%>
                                    </div>
                                </div>
                                <div class="cell-lg-6 pl-3-lg">
                                    <div class="mt-3">
                                        <label class="fw-600">Ảnh đại diện</label>

                                        <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Kích thước chuẩn: 500x500 pixel.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>

                                        <div class="place-right">
                                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:Button ID="Button2" runat="server" Text="Xóa ảnh hiện tại" CssClass="alert small" Visible="false" OnClick="Button2_Click" />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                        <asp:FileUpload ID="FileUpload2" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />


                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Nhân viên chăm sóc</label>
                                        <asp:DropDownList ID="ddl_nhanvien_chamsoc" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600">Mã giới thiệu</label>
                                        <asp:TextBox ID="txt_magioithieu" runat="server" data-role="input"></asp:TextBox><%--autocomplete="off" --%>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Nhóm khách hàng</label>
                                        <asp:DropDownList ID="DropDownList1" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                            </div>

                            <div class="text-center mt-6 mb-6">
                                <asp:Button OnClientClick="Metro.activity.open({type:'cycle',overlayClickClose:false})" ID="button1" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="button1_Click" />
                            </div>
                        </asp:Panel>
                    </div>

                    <div id="_hoadon">
                        <div class="row mt-2">
                            <div class="cell-lg-4 mt-2">
                                <div class="icon-box border bd-cyan">
                                    <div class="icon bg-cyan fg-white"><span class="mif-open-book"></span></div>
                                    <div class="content p-4">
                                        <div class="text-upper">Số hóa đơn: <span class="text-bold"><%=tong_hoadon.ToString("#,##0") %></span></div>
                                        <%--<div class="text-upper text-bold text-lead"></div>--%>
                                        <div class="mt-3">
                                            <small>
                                                <span class="fg-cyan pr-3 text-bold"><%=tong_dv.ToString("#,##0") %> dịch vụ</span>
                                                <span class="fg-green pr-3 text-bold"><%=tong_sp.ToString("#,##0") %> sản phẩm</span>
                                            </small>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="cell-lg-4 mt-2">
                                <div class="icon-box border bd-red">
                                    <div class="icon bg-red fg-white"><span class="mif-dollars"></span></div>
                                    <div class="content p-4">
                                        <div class="text-upper">Tổng chi tiêu</div>
                                        <div class="text-upper text-bold text-lead"><%=tong_chitieu.ToString("#,##0") %></div>
                                    </div>
                                </div>
                            </div>

                            <div class="cell-lg-4 mt-2">
                                <div class="icon-box border bd-orange">
                                    <div class="icon bg-orange fg-white"><span class="mif-money"></span></div>
                                    <div class="content p-4">
                                        <div class="text-upper">Tổng công nợ</div>
                                        <div class="text-upper text-bold text-lead"><%=tong_congno.ToString("#,##0") %></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div>
                            <asp:Repeater ID="Repeater1" runat="server">
                                <ItemTemplate>
                                    <div class="text-bold mt-10 ">
                                        <a data-role="hint" data-hint-position="top" data-hint-text="Xem chi tiết" class="fg-orange" href="/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=<%#Eval("id") %>">Hóa đơn: <%#Eval("id") %></a>
                                    </div>
                                    <div style="overflow: auto" class=" mt-2">
                                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                            <thead>
                                                <tr class="bg-green">
                                                    <td class="text-bold fg-white" style="width: 1px; min-width: 1px;">Ngày tạo</td>
                                                    <td class="text-bold fg-white" style="min-width: 120px;">Người tạo</td>
                                                    <td class="text-bold fg-white" style="width: 100px; min-width: 100px">Tổng tiền</td>
                                                    <td class="text-bold fg-white" style="width: 1px; min-width: 1px">CK</td>
                                                    <td class="text-bold fg-white" style="width: 100px; min-width: 100px">Sau CK</td>
                                                    <td class="text-bold fg-white" style="width: 110px; min-width: 110px">Thanh toán</td>
                                                    <td class="text-bold fg-white" style="width: 86px; min-width: 86px">Công nợ</td>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td class="text-right">
                                                        <div><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></div>
                                                    </td>
                                                    <td><%#return_ten_nhanvien(Eval("nguoitao").ToString()) %></td>
                                                    <td class="text-right"><%#Eval("tongtien","{0:#,##0}").ToString() %></td>
                                                    <td class="text-right">
                                                        <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("chietkhau").ToString()=="0" %>'>
                                                            <%#Eval("tongtien_ck_hoadon","{0:#,##0}").ToString() %>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("chietkhau").ToString()!="0" %>'>
                                                            <%#Eval("chietkhau")%>%
                                                        </asp:PlaceHolder>
                                                    </td>
                                                    <td class="text-right">
                                                        <div><%#Eval("tongsauchietkhau","{0:#,##0}").ToString() %></div>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("sotien_conlai").ToString()=="0" %>'>
                                                            <span class="data-wrapper"><code class="bg-red  fg-white">Đã thanh toán</code></span>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("sotien_conlai").ToString()!="0" %>'>
                                                            <%#Eval("sotien_dathanhtoan","{0:#,##0}").ToString() %>
                                                        </asp:PlaceHolder>
                                                    </td>
                                                    <td class="text-right">
                                                        <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("sotien_conlai").ToString()=="0" %>'>
                                                            <%#Eval("sotien_conlai","{0:#,##0}").ToString() %>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("sotien_conlai").ToString()!="0" %>'>
                                                            <span class="data-wrapper"><code class="bg-orange fg-white"><%#Eval("sotien_conlai","{0:#,##0}").ToString() %></code></span>
                                                        </asp:PlaceHolder>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>

                                    <div data-role="accordion"
                                        data-one-frame="false"
                                        data-show-active="true"
                                        data-on-frame-open="console.log('frame was opened!', arguments[0])"
                                        data-on-frame-close="console.log('frame was closed!', arguments[0])">
                                        <div class="frame">
                                            <div class="heading">Dịch vụ - Sản phẩm</div>
                                            <div class="content">
                                                <div style="overflow: auto" class=" mt-2">
                                                    <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                                        <thead>
                                                            <tr style="background-color: #f5f5f5">

                                                                <td class="text-bold" style="width: 1px; min-width: 1px;">Ngày bán</td>
                                                                <td class="text-bold" style="min-width: 120px;">Người bán</td>
                                                                <td class="text-bold" style="min-width: 150px">Mặt hàng</td>

                                                                <td class="text-bold text-center" style="width: 1px;">Giá</td>
                                                                <td class="text-bold" style="width: 1px;">SL</td>
                                                                <td class="text-bold" style="width: 102px; min-width: 102px">Thành tiền</td>
                                                                <td class="text-bold" style="width: 1px;">CK</td>
                                                                <td class="text-bold" style="width: 100px;">Sau CK</td>


                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <asp:Repeater ID="Repeater2" runat="server" DataSource='<%#show_chitiet_hoadon(Eval("id").ToString()) %>'>
                                                                <ItemTemplate>
                                                                    <tr>

                                                                        <td class="text-right">
                                                                            <div><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></div>
                                                                        </td>
                                                                        <td><%#return_ten_nhanvien(Eval("nguoitao").ToString())%></td>
                                                                        <td>
                                                                            <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#Eval("kyhieu").ToString()=="dichvu" %>'>
                                                                                <span class="fg-blue"><%#Eval("ten_dvsp_taithoidiemnay").ToString() %></span>
                                                                            </asp:PlaceHolder>
                                                                            <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible='<%#Eval("kyhieu").ToString()=="sanpham" %>'>
                                                                                <span class="fg-green"><%#Eval("ten_dvsp_taithoidiemnay").ToString() %></span>
                                                                            </asp:PlaceHolder>
                                                                            <div>
                                                                                <asp:PlaceHolder ID="PlaceHolder10" runat="server" Visible='<%#Eval("id_thedichvu")!=null%>'>
                                                                                    <span class="data-wrapper"><code class="bg-orange fg-white">Thẻ DV số <%#Eval("id_thedichvu")%></code></span>
                                                                                </asp:PlaceHolder>
                                                                            </div>
                                                                        </td>
                                                                        <td class="text-right">
                                                                            <%#Eval("gia_dvsp_taithoidiemnay","{0:#,##0}").ToString() %>                                       
                                                                        </td>
                                                                        <td class="text-right">
                                                                            <%#Eval("soluong","{0:#,##0}").ToString() %>
                                                                        </td>
                                                                        <td class="text-right">
                                                                            <div><%#Eval("thanhtien","{0:#,##0}").ToString() %> </div>
                                                                        </td>
                                                                        <td class="text-right">
                                                                            <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("chietkhau").ToString()=="0" %>'>
                                                                                <%#Eval("tongtien_ck_dvsp","{0:#,##0}").ToString() %>
                                                                            </asp:PlaceHolder>
                                                                            <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("chietkhau").ToString()!="0" %>'>
                                                                                <%#Eval("chietkhau")%>%
                                                                            </asp:PlaceHolder>
                                                                        </td>
                                                                        <td class="text-right">
                                                                            <div><b><%#Eval("tongsauchietkhau","{0:#,##0}").ToString() %></b></div>
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
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                    <div id="_dathen">
                        <div style="overflow: auto" class="mt-3">
                            <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                <tbody>
                                    <tr style="background-color: #f5f5f5">
                                        <td class="text-bold" style="min-width: 1px; width: 1px">Mã</td>
                                        <td class="text-bold" style="min-width: 1px; width: 1px">Ngày đặt</td>
                                        <td class="text-bold" style="min-width: 140px">Khách hàng</td>

                                        <td class="text-bold" style="min-width: 140px">Dịch vụ</td>
                                        <td class="text-bold" style="min-width: 140px">NV thực hiện</td>
                                        <td class="text-bold" style="min-width: 140px">Ghi chú</td>
                                        <td class="text-bold" style="min-width: 140px">Người tạo</td>
                                        <td class="text-bold" style="min-width: 100px; width: 100px">Trạng thái</td>
                                    </tr>
                                    <asp:Repeater ID="Repeater3" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="text-center">
                                                    <a data-role="hint" data-hint-position="top" data-hint-text="Chỉnh sửa" href="/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=<%#Eval("id").ToString() %>">
                                                        <b><%#Eval("id").ToString() %></b>
                                                    </a>
                                                </td>
                                                <td class="text-right"><%#Eval("ngaydat","{0:dd/MM/yyyy HH:mm}").ToString() %></td>
                                                <td><%#Eval("tenkhachhang").ToString() %>
                                                    <div><a class="fg-cobalt" data-role="hint" data-hint-position="top" data-hint-text="Nhấn để gọi" href="tel:<%#Eval("sdt").ToString() %>"><%#Eval("sdt").ToString() %></a></div>

                                                    <!-- HIỂN THỊ EMAIL (NẾU CÓ) -->
                                                    <asp:PlaceHolder ID="PlaceHolderEmail" runat="server" Visible='<%#Eval("email").ToString()!="" %>'>
                                                        <div><a class="fg-teal" data-role="hint" data-hint-position="top" data-hint-text="Nhấn để gửi mail" href="mailto:<%#Eval("email").ToString() %>"><%#Eval("email").ToString() %></a></div>
                                                    </asp:PlaceHolder>
                                                    <!-- /HIỂN THỊ EMAIL -->

                                                </td>

                                                <td>
                                                    <%#Eval("tendv").ToString() %>
                                                    <div class="mt-1">
                                                        <asp:PlaceHolder ID="PlaceHolderLinkedInvoiceKh" runat="server" Visible='<%#return_so_hoadon_lienket_tu_ghichu(Eval("ghichu").ToString()) > 0 %>'>
                                                            <a class="fg-green" href='<%#return_url_hoadon_lienket_tu_ghichu(Eval("ghichu").ToString()) %>'>
                                                                <span class="crm-chip crm-chip--green"><%#return_so_hoadon_lienket_tu_ghichu(Eval("ghichu").ToString()) %> HĐ</span>
                                                            </a>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolderLinkedTheDvKh" runat="server" Visible='<%#return_so_thedv_lienket_tu_ghichu(Eval("ghichu").ToString()) > 0 %>'>
                                                            <a class="fg-teal" href='<%#return_url_tieu_buoi_tu_lich(Eval("id").ToString()) %>'>
                                                                <span class="crm-chip crm-chip--teal"><%#return_so_thedv_lienket_tu_ghichu(Eval("ghichu").ToString()) %> thẻ DV</span>
                                                            </a>
                                                        </asp:PlaceHolder>
                                                    </div>
                                                </td>
                                                <td><%#return_ten_nhanvien(Eval("nhanvien_thuchien").ToString()) %></td>
                                                <td><%#Eval("ghichu").ToString() %></td>
                                                <td>
                                                    <%#return_ten_nhanvien(Eval("nguoitao").ToString()) %>
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
                    </div>

                    <div id="_ghichu">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>
                                <div class="place-right">
                                    <ul class="h-menu">
                                        <%if (bcorn_class.check_quyen(user, "q8_3") == "")
                                            { %>
                                        <li data-role="hint" data-hint-position="top" data-hint-text="Tạo ghi chú" onclick="show_hide_id_form_add_ghichu()">
                                            <a class="button"><span class="mif mif-plus"></span></a></li>
                                        <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                        <li data-role="hint" data-hint-position="top" data-hint-text="Xóa ghi chú">
                                            <asp:ImageButton ID="but_xoa_ghichu" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" OnClick="but_xoa_ghichu_Click" />
                                        </li>
                                        <%} %>
                                    </ul>
                                </div>
                                <div class="clr-float"></div>
                                <div style="overflow: auto" class="mt-3">
                                    <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                        <tbody>
                                            <tr style="background-color: #f5f5f5">
                                                <td style="width: 1px;">
                                                    <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table-ghichu input[type=checkbox]').prop('checked', this.checked)">
                                                </td>
                                                <td class="text-bold" style="min-width: 1px; width: 1px">Ngày tạo</td>
                                                <td class="text-bold" style="min-width: 140px">Người tạo</td>
                                                <td class="text-bold" style="min-width: 180px">Nội dung ghi chú</td>
                                                <td class="text-bold" style="min-width: 80px; width: 80px">Mã đơn</td>
                                            </tr>
                                            <asp:Repeater ID="Repeater4" runat="server">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="checkbox-table-ghichu">
                                                            <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("kyhieu").ToString()=="table" %>'>
                                                                <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="ghichu_<%#Eval("id").ToString() %>">
                                                            </asp:PlaceHolder>
                                                        </td>
                                                        <td class="text-right"><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></td>
                                                        <td><%#return_ten_nhanvien(Eval("nguoitao").ToString()) %></td>
                                                        <td><%#Eval("ghichu").ToString() %></td>
                                                        <td class="text-bold text-center">
                                                            <a data-role="hint" data-hint-position="top" data-hint-text="Xem hóa đơn" href="/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=<%#Eval("id_hoadon").ToString() %>">
                                                                <%#Eval("id_hoadon").ToString() %>
                                                            </a>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <div id="_hinhanh">
                        <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                            <Triggers>
                            </Triggers>
                            <ContentTemplate>
                                <div class="place-right">
                                    <ul class="h-menu">
                                        <%if (bcorn_class.check_quyen(user, "q8_3") == "")
                                            { %>
                                        <li data-role="hint" data-hint-position="top" data-hint-text="Thêm hình ảnh" onclick="show_hide_id_form_add_hinhanh()">
                                            <a class="button"><span class="mif mif-plus"></span></a></li>
                                        <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                        <li data-role="hint" data-hint-position="top" data-hint-text="Xóa">
                                            <asp:ImageButton ID="but_xoa_hinhanh_truocsau" OnClick="but_xoa_hinhanh_truocsau_Click" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" />
                                        </li>
                                        <%} %>
                                    </ul>
                                </div>
                                <div class="clr-float"></div>
                                <div style="overflow: auto" class="mt-3">
                                    <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                        <tbody>
                                            <tr style="background-color: #f5f5f5">
                                                <td style="width: 1px;">
                                                    <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table-hinhanh input[type=checkbox]').prop('checked', this.checked)">
                                                </td>
                                                <td class="text-bold" style="min-width: 1px; width: 1px">Ngày tạo</td>
                                                <td class="text-bold" style="min-width: 140px">Người tạo</td>
                                                <td class="text-bold" style="min-width: 230px; width: 230px">Ảnh trước sau</td>
                                                <td class="text-bold" style="min-width: 140px">Ghi chú</td>

                                            </tr>
                                            <asp:Repeater ID="Repeater6" runat="server">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="checkbox-table-hinhanh">

                                                            <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="hinhanh_<%#Eval("id").ToString() %>">
                                                        </td>
                                                        <td class="text-right"><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></td>
                                                        <td><%#return_ten_nhanvien(Eval("nguoitao").ToString()) %></td>


                                                        <td class="text-center">
                                                            <div data-role="lightbox">
                                                                <%#Eval("anhtruoc").ToString() %>
                                                                <%#Eval("anhsau").ToString() %>
                                                            </div>
                                                        </td>



                                                        <td><%#Eval("ghichu").ToString() %></td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <div id="_donthuoc">
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="Button4" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>
                                <div class="place-right">
                                    <ul class="h-menu">
                                        <%if (bcorn_class.check_quyen(user, "q8_3") == "")
                                            { %>
                                        <li data-role="hint" data-hint-position="top" data-hint-text="Tạo đơn thuốc" onclick="show_hide_id_form_add_donthuoc()">
                                            <a class="button"><span class="mif mif-plus"></span></a></li>
                                        <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                        <li data-role="hint" data-hint-position="top" data-hint-text="Xóa đơn thuốc">
                                            <asp:ImageButton ID="but_xoa_donthuoc" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" OnClick="but_xoa_donthuoc_Click" />
                                        </li>
                                        <%} %>
                                    </ul>
                                </div>
                                <div class="clr-float"></div>
                                <div style="overflow: auto" class="mt-3">
                                    <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                        <tbody>
                                            <tr style="background-color: #f5f5f5">
                                                <td style="width: 1px;">
                                                    <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table-donthuoc input[type=checkbox]').prop('checked', this.checked)">
                                                </td>
                                                <td class="text-bold" style="min-width: 1px; width: 1px">Ngày tạo</td>
                                                <td class="text-bold" style="min-width: 140px">Người tạo</td>
                                                <td class="text-bold" style="min-width: 180px">Nội dung đơn thuốc</td>
                                                <td class="text-bold" style="min-width: 1px; width: 1px">Tái khám</td>
                                                <td class="text-bold" style="min-width: 140px">Nơi tái khám</td>
                                                <td class="text-bold" style="min-width: 180px">Lời dặn bác sĩ</td>
                                            </tr>
                                            <asp:Repeater ID="Repeater5" runat="server">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="checkbox-table-donthuoc">

                                                            <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="donthuoc_<%#Eval("id").ToString() %>">
                                                        </td>
                                                        <td class="text-right"><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></td>
                                                        <td><%#return_ten_nhanvien(Eval("nguoitao").ToString()) %></td>
                                                        <td><%#Eval("ghichu").ToString() %></td>
                                                        <td class="text-right"><%#Eval("ngaytaikham","{0:dd/MM/yyyy}").ToString() %></td>
                                                        <td><%#Eval("noitaikham").ToString() %></td>
                                                        <td><%#Eval("loidan").ToString() %></td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <div id="_thedichvu">
                        <% if (co_ngu_canh_datlich) { %>
                        <div class="booking-link-box mt-3">
                            <div class="d-flex flex-justify-between flex-wrap flex-align-center">
                                <div>
                                    <div class="booking-link-box__title">Đang tiêu buổi từ lịch hẹn #<%=id_datlich_lienket %></div>
                                    <div class="booking-link-box__meta mt-1">
                                        Dịch vụ: <%=ten_dichvu_datlich != "" ? ten_dichvu_datlich : "Chưa có dịch vụ" %>
                                        <% if (ngay_datlich_lienket != "") { %> | Giờ hẹn: <%=ngay_datlich_lienket %><% } %>
                                        <% if (ten_nhanvien_datlich != "") { %> | Nhân viên gợi ý: <%=ten_nhanvien_datlich %><% } %>
                                    </div>
                                </div>
                                <div class="mt-2">
                                    <a class="button info outline" href="<%=url_quay_lai_datlich %>">Quay lại lịch hẹn</a>
                                </div>
                            </div>
                            <div class="mt-2"><%=thongbao_datlich_thedv %></div>
                        </div>
                        <% } %>
                        <div class="row mt-2">
                            <div class="cell-lg-4 mt-2">
                                <div class="icon-box border bd-cyan">
                                    <div class="icon bg-cyan fg-white"><span class="mif-credit-card"></span></div>
                                    <div class="content p-4">
                                        <div class="text-upper">Thẻ đã mua</div>
                                        <div class="text-upper text-bold text-lead"><%=sl_thedv.ToString("#,##0") %></div>
                                    </div>
                                </div>
                            </div>
                            <div class="cell-lg-4 mt-2">
                                <div class="icon-box border bd-red">
                                    <div class="icon bg-red fg-white"><span class="mif-dollars"></span></div>
                                    <div class="content p-4">
                                        <div class="text-upper">Tổng giá trị thẻ</div>
                                        <div class="text-upper text-bold text-lead"><%=doanhso_tdv_sauck.ToString("#,##0") %></div>
                                    </div>
                                </div>
                            </div>

                            <div class="cell-lg-4 mt-2">
                                <div class="icon-box border bd-orange">
                                    <div class="icon bg-orange fg-white"><span class="mif-money"></span></div>
                                    <div class="content p-4">
                                        <div class="text-upper">Tổng công nợ</div>
                                        <div class="text-upper text-bold text-lead"><%=tong_congno_tdv.ToString("#,##0") %></div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="mt-4 bg-light">
                                    <div class="row">
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
                                    <div class="text-center">
                                        <asp:Button ID="Button5" CssClass="success mt-4 mb-4" runat="server" Text="Sử dụng thẻ đã chọn" OnClick="Button5_Click" />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="UpdatePanel8">
                            <ProgressTemplate>
                                <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                                    <div style="padding-top: 45vh;">
                                        <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                                    </div>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>

                        <div>
                            <asp:Repeater ID="Repeater7" runat="server">
                                <ItemTemplate>
                                    <div class="text-bold mt-4 ">
                                        <a data-role="hint" data-hint-position="top" data-hint-text="Xem chi tiết" class="fg-orange" href="/gianhang/admin/quan-ly-the-dich-vu/chi-tiet.aspx?id=<%#Eval("id") %>">Thẻ dịch vụ số <%#Eval("id") %></a>
                                        <asp:PlaceHolder ID="PlaceHolderBookingSuggested" runat="server" Visible='<%#check_thedv_goiy(Eval("id").ToString()) %>'>
                                            <span class="ml-2 data-wrapper"><code class="bg-green fg-white">Gợi ý cho lịch này</code></span>
                                        </asp:PlaceHolder>
                                    </div>
                                    <div style="overflow: auto" class=" mt-2">
                                        <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                            <thead>
                                                <tr style="background-color: #f5f5f5">
                                                    <td style="width: 1px;"></td>
                                                    <td class="text-bold" style="min-width: 90px; width: 90px">Thời hạn</td>
                                                    <td class="text-bold" style="min-width: 130px">Người bán/CK</td>
                                                    <td class="text-bold" style="min-width: 120px;">Tên thẻ/DV</td>
                                                    <td class="text-bold" style="min-width: 100px;">Số buổi</td>

                                                    <td class="text-bold" style="min-width: 130px">Tổng tiền/CK</td>

                                                    <td class="text-bold" style="min-width: 100px">Sau CK</td>
                                                    <td class="text-bold" style="min-width: 110px">T.Toán/C.Nợ</td>

                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr class="<%#return_thedv_row_class(Eval("id").ToString()) %>">
                                                    <td class="checkbox-table-tdv">
                                                        <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_thedv_<%#Eval("id").ToString() %>" <%#return_thedv_checked_attr(Eval("id").ToString()) %>>
                                                    </td>
                                                    <td class="text-right">
                                                        <small>
                                                            <div><%#Eval("ngaytao","{0:dd/MM/yyyy}").ToString() %></div>
                                                            <div><%#check_hsd(Eval("hsd","{0:dd/MM/yyyy}").ToString()) %></div>
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
                                                    </small>
                                                    </td>
                                                    <td>
                                                        <small>
                                                            <div>Số buổi: <%#Eval("sobuoi").ToString() %></div>
                                                            <div>Đã làm: <%#Eval("sl_dalam").ToString() %></div>
                                                            <div>Còn lại: <%#Eval("sl_conlai").ToString() %></div>
                                                        </small>
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

                                                <%--<%stt = stt + 1; %>--%>
                                            </tbody>
                                            <tfoot>
                                                <tr class="">
                                                    <td colspan="4"></td>
                                                    <td class="text-right text-bold"><%=doanhso_tdv.ToString("#,##0") %></td>

                                                    <td class="text-right text-bold fg-red"><%=doanhso_tdv_sauck.ToString("#,##0") %></td>
                                                    <td class="text-right text-bold">
                                                        <%--<div><small>TT: <%=tongtien_dathanhtoan.ToString("#,##0") %></small></div>--%>

                                                        <div class="fg-orange"><%=tong_congno.ToString("#,##0") %></div>
                                                    </td>

                                                </tr>
                                            </tfoot>

                                        </table>

                                    </div>
                                    <div data-role="accordion"
                                        data-one-frame="false"
                                        data-show-active="true"
                                        data-on-frame-open="console.log('frame was opened!', arguments[0])"
                                        data-on-frame-close="console.log('frame was closed!', arguments[0])">
                                        <div class="frame">
                                            <div class="heading">Nhật ký sử dụng</div>
                                            <div class="content">
                                                <div style="overflow: auto" class=" mt-3 mb-3">
                                                    <table class="table row-hover table-border cell-border compact normal-lg <%--striped--%> <%--compact normal-lg--%>">
                                                        <thead>
                                                            <tr style="background-color: #f5f5f5">

                                                                <td class="text-bold" style="width: 1px; min-width: 1px;">STT</td>
                                                                <td class="text-bold" style="width: 100px;">Ngày SD</td>
                                                                <%--<td class="text-bold text-center" style="width: 50px; min-width: 50px">Ảnh</td>--%>
                                                                <td class="text-bold" style="min-width: 150px">Dịch vụ</td>
                                                                <td class="text-bold" style="width: 1px">Đơn</td>
                                                                <td class="text-bold" style="min-width: 170px">Nhân viên làm dịch vụ</td>
                                                                <td class="text-bold" style="width: 170px; min-width: 170px">Đánh giá người làm</td>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <asp:Repeater ID="Repeater1" runat="server" DataSource='<%#show_chitiet_thedv(Eval("id").ToString()) %>'>
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td><%=stt_tdv %></td>
                                                                        <td class="text-right">
                                                                            <div><%#Eval("ngaytao","{0:dd/MM/yyyy HH:mm}").ToString() %></div>
                                                                        </td>

                                                                        <td>

                                                                            <span class="fg-blue"><%#Eval("ten_dvsp_taithoidiemnay").ToString() %></span>
                                                                        </td>
                                                                        <td class="text-bold text-center">
                                                                            <a data-role="hint" data-hint-position="top" data-hint-text="Xem hóa đơn" href="/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=<%#Eval("id_hoadon").ToString()%>">
                                                                                <%#Eval("id_hoadon").ToString()%>
                                                                            </a>
                                                                        </td>
                                                                        <td>
                                                                            <div><%#Eval("tennguoilam_hientai").ToString()%></div>

                                                                            <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("phantram_lamdichvu").ToString()=="0" %>'>
                                                                                <div class="text-right text-bold fg-emerald"><%#Eval("tongtien_lamdichvu","{0:#,##0}").ToString() %></div>
                                                                            </asp:PlaceHolder>
                                                                            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("phantram_lamdichvu").ToString()!="0" %>'>
                                                                                <div class="text-right text-bold fg-emerald"><%#Eval("phantram_lamdichvu")%>%</div>
                                                                            </asp:PlaceHolder>
                                                                        </td>
                                                                        <td>
                                                                            <%#Eval("danhgia_nhanvien_lamdichvu")%>
                                                                            <div>
                                                                                <input data-role="rating" data-value="<%#Eval("danhgia_5sao_dv")%>" name="danhgia_5sao_dv_<%#Eval("id")%>" data-static="true">
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                    <%stt_tdv = stt_tdv + 1; %>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </tbody>

                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                </div>
            </div>



        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>
