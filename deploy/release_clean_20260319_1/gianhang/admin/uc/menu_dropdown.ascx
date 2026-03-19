<%@ Control Language="C#" AutoEventWireup="true" CodeFile="menu_dropdown.ascx.cs" Inherits="badmin_uc_menu_dropdown" %>
<div class="gh-admin-dropdown">
    <ul class="navview-menu" id="side-menu">
        <li class="<%=a0 %>">
            <a href="/gianhang/admin">
                <span class="icon"><span class="mif-home"></span></span>
                <span class="caption">Trang chủ</span>
            </a>
        </li>
        <li class="item-header">VẬN HÀNH NHANH</li>
        <li class="<%=atv_ql_hoadon %>">
            <a href="/gianhang/admin/quan-ly-hoa-don/Default.aspx">
                <span class="icon"><span class="mif-open-book"></span></span>
                <span class="caption">Bán hàng · Hóa đơn</span>
            </a>
        </li>
        <li class="<%=atv_kh_dtkh %>">
            <a href="/gianhang/admin/quan-ly-khach-hang/Default.aspx">
                <span class="icon"><span class="mif-database"></span></span>
                <span class="caption">Khách hàng</span>
            </a>
        </li>
        <li class="<%=b0 %>">
            <a href="/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx">
                <span class="icon"><span class="mif-calendar"></span></span>
                <span class="caption">Lịch hẹn</span>
            </a>
        </li>
        <li class="<%=b3 %>">
            <a href="/gianhang/admin/quan-ly-kho-hang/Default.aspx">
                <span class="icon"><span class="mif-shop"></span></span>
                <span class="caption">Kho hàng</span>
            </a>
        </li>
        <li class="<%=a7 %>">
            <a href="/gianhang/admin/quan-ly-thu-chi/Default.aspx">
                <span class="icon"><span class="mif-dollars"></span></span>
                <span class="caption">Thu chi</span>
            </a>
        </li>
        <li class="<%=tk1 %>">
            <a href="/gianhang/admin/quan-ly-hoa-don/thong-ke-dich-vu.aspx">
                <span class="icon"><span class="mif-chart-bars2"></span></span>
                <span class="caption">Báo cáo nhanh</span>
            </a>
        </li>
        <li class="item-header">QUẢN LÝ HỆ THỐNG</li>

        <%if (bcorn_class.check_quyen(user, "q16_0") == "")
            { %>
        <li class="<%=b20 %> fg-red">
            <a href="/gianhang/admin/quan-ly-he-thong/chi-nhanh.aspx">
                <span class="icon"><span class="mif-location"></span></span>
                <span class="caption">Quản lý chi nhánh</span>
            </a>
        </li>
        <%} %>

        <li class="<%=b18 %>">
            <a href="/gianhang/admin/quan-ly-he-thong/nganh.aspx">
                <span class="icon"><span class="mif-joomla"></span></span>
                <span class="caption">Quản lý ngành</span>
            </a>
        </li>
        <li class="<%=b19 %>">
            <a href="/gianhang/admin/quan-ly-he-thong/phong-ban.aspx">
                <span class="icon"><span class="mif-stack"></span></span>
                <span class="caption">Quản lý phòng ban</span>
            </a>
        </li>

        <li class="item-header">QUẢN LÝ TRANG CHỦ</li>

        <%if (Session["chinhanh"] != null && Session["chinhanh"].ToString() == "1")
                { %>
        <li class="<%=a1 %>">
            <a class="dropdown-toggle">
                <span class="icon"><span class="mif-cogs"></span></span>
                <span class="caption">Cấu hình chung</span>
            </a>
            <ul class="navview-menu" data-role="dropdown">
                <%if (bcorn_class.check_quyen(user, "q1_1") == "")
                { %>
                <li class="<%=a11 %>">
                    <a href="/gianhang/admin/cau-hinh-chung/tao-lien-ket-chia-se.aspx">
                        <span class="icon"><span class="mif-link"></span></span>
                        <span class="caption">Tạo liên kết chia sẻ</span>
                    </a>
                </li>
                <%} %>
                <%if (bcorn_class.check_quyen(user, "q1_2") == "")
                { %>
                <li class="<%=a12 %>">
                    <a href="/gianhang/admin/cau-hinh-chung/nhung-ma-vao-website.aspx">
                        <span class="icon"><span class="mif-embed2"></span></span>
                        <span class="caption">Nhúng mã vào website</span>
                    </a>
                </li>
                <%} %>
                <%if (bcorn_class.check_quyen(user, "q1_3") == "")
                { %>
                <li class="<%=a13 %>">
                    <a href="/gianhang/admin/cau-hinh-chung/cap-nhat-thong-tin.aspx">
                        <span class="icon"><span class="mif-info"></span></span>
                        <span class="caption">Cập nhật thông tin</span>
                    </a>
                </li>
                <%} %>
                <%if (bcorn_class.check_quyen(user, "q1_4") == "")
                { %>
                <li class="<%=a14 %>">
                    <a href="/gianhang/admin/cau-hinh-chung/link-social-media.aspx">
                        <span class="icon"><span class="mif-earth"></span></span>
                        <span class="caption">Link social media</span>
                    </a>
                </li>
                <%} %>
                <%--<li class="<%=a15 %>">
                    <a href="/gianhang/admin/cau-hinh-chung/cai-dat-bao-tri.aspx">
                        <span class="icon"><span class="mif-tools"></span></span>
                        <span class="caption">Cài đặt bảo trì</span>
                    </a>
                </li>--%>
            </ul>
        </li>
        <li class="<%=a5 %>">
            <a class="dropdown-toggle">
                <span class="icon"><span class="mif-dashboard"></span></span>
                <span class="caption">Quản lý module</span>
            </a>
            <ul class="navview-menu" data-role="dropdown">
                <%if (bcorn_class.check_quyen(user, "q5_1") == "")
                    { %>
                <li class="<%=a51 %>">
                    <a href="/gianhang/admin/quan-ly-module/slide-anh/default.aspx">
                        <span class="icon"><span class="mif-images"></span></span>
                        <span class="caption">Quản lý slide ảnh</span>
                    </a>
                </li>
                <%} %>
            </ul>
        </li>
        <%if (bcorn_class.check_quyen(user, "q6_1") == "")
            { %>
        <li class="<%=a6 %>">
            <a href="/gianhang/admin/yeu-cau-tu-van/default.aspx">
                <span class="icon"><span class="mif-contacts-mail"></span></span>
                <span class="caption">Yêu cầu tư vấn</span>
            </a>
        </li>
        <%} %>
        <%} %>

        <li class="<%=a3 %>">
            <a href="/gianhang/admin/quan-ly-menu/Default.aspx">
                <span class="icon"><span class="mif-flow-tree"></span></span>
                <span class="caption">Quản lý menu</span>
            </a>
        </li>
        <li class="<%=a4 %>">
            <a href="/gianhang/admin/quan-ly-bai-viet/Default.aspx">
                <span class="icon"><span class="mif-news"></span></span>
                <span class="caption">Quản lý bài viết</span>
            </a>
        </li>
        <li class="<%=atv_ql_dichvu %>">
            <a href="/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=dv">
                <span class="icon"><span class="mif-list"></span></span>
                <span class="caption">Quản lý dịch vụ</span>
                <span class="badges ml-auto mr-3">
                    <span class="badge inline bg-cyan fg-white"><%=post_class.dem_soluong_dv().ToString("#,##0") %></span>
                </span>
            </a>
        </li>
        <li class="<%=atv_ql_sanpham %>">
            <a href="/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=sp">
                <span class="icon"><span class="mif-shop"></span></span>
                <span class="caption">Quản lý sản phẩm</span>
                <span class="badges ml-auto mr-3">
                    <span class="badge inline bg-green fg-white"><%=post_class.dem_soluong_sp().ToString("#,##0") %></span>
                </span>
            </a>
        </li>
        



        <li class="item-header">QUẢN LÝ ĐÀO TẠO</li>
        <li class="<%=b14 %>">
            <a href="/gianhang/admin/quan-ly-giang-vien/Default.aspx">
                <span class="icon"><span class="mif-user-secret"></span></span>
                <span class="caption">Danh sách Chuyên gia</span>
            </a>
        </li>
        <li class="<%=b15 %>">
            <a href="/gianhang/admin/quan-ly-giang-vien/add.aspx">
                <span class="icon"><span class="mif-plus"></span></span>
                <span class="caption">Thêm Chuyên gia</span>
            </a>
        </li>
        <li class="<%=b16 %>">
            <a href="/gianhang/admin/quan-ly-hoc-vien/Default.aspx">
                <span class="icon"><span class="mif-users"></span></span>
                <span class="caption">Danh sách thành viên</span>
            </a>
        </li>
        <li class="<%=b17 %>">
            <a href="/gianhang/admin/quan-ly-hoc-vien/add.aspx">
                <span class="icon"><span class="mif-plus"></span></span>
                <span class="caption">Thêm thành viên</span>
            </a>
        </li>


        <li class="item-header">TÀI KHOẢN NHÂN VIÊN</li>
        <%--<li class="<%=atv_caidatchung%>">
            <a href="/gianhang/admin/cai-dat-chung/default.aspx">
                <span class="icon"><span class="mif-cogs"></span></span>
                <span class="caption">Cài đặt chung</span>
            </a>
        </li>--%>
        <li class="<%=a2 %>">
            <a href="/gianhang/admin/quan-ly-tai-khoan/Default.aspx">
                <span class="icon"><span class="mif-users"></span></span>
                <span class="caption">Quản lý tài khoản</span>
            </a>
        </li>
        <li class="<%=atv_doanhso_nv %>">
            <a href="/gianhang/admin/quan-ly-tai-khoan/doanh-so-nhan-vien.aspx">
                <span class="icon"><span class="mif-chart-bars2"></span></span>
                <span class="caption">Doanh số nhân viên</span>
            </a>
        </li>
        <li class="<%=atv_bangchamcong_nv %>">
            <a href="/gianhang/admin/quan-ly-tai-khoan/bang-cham-cong.aspx">
                <span class="icon"><span class="mif-table"></span></span>
                <span class="caption">Xem bảng chấm công</span>
            </a>
        </li>
        <li class="<%=atv_chamcong_nv %>">
            <a href="/gianhang/admin/quan-ly-tai-khoan/cham-cong-nhan-vien.aspx">
                <span class="icon"><span class="mif-checkmark"></span></span>
                <span class="caption">Chấm công</span>
            </a>
        </li>

        <li class="<%=atv_tinhluong_nv %>">
            <a href="/gianhang/admin/quan-ly-tai-khoan/tinh-luong-nhan-vien.aspx">
                <span class="icon"><span class="mif-money"></span></span>
                <span class="caption">Tính lương nhân viên</span>
            </a>
        </li>

        <li class="item-header">Kho</li>
        <li class="<%=b3 %>">
            <a href="/gianhang/admin/quan-ly-kho-hang/Default.aspx">
                <span class="icon"><span class="mif-shop"></span></span>
                <span class="caption">Kho</span>
            </a>
        </li>
        <li class="<%=b4 %>">
            <a href="/gianhang/admin/quan-ly-kho-hang/nhap-hang.aspx?q=nh">
                <span class="icon"><span class="mif-plus"></span></span>
                <span class="caption">Nhập hàng</span>
            </a>
        </li>
        <li class="<%=b5 %>">
            <a href="/gianhang/admin/quan-ly-kho-hang/don-nhap-hang.aspx">
                <span class="icon"><span class="mif-open-book"></span></span>
                <span class="caption">Đơn đã nhập</span>
            </a>
        </li>
        <li class="<%=b6 %>">
            <a href="/gianhang/admin/quan-ly-kho-hang/nha-cung-cap.aspx">
                <span class="icon"><span class="mif-library"></span></span>
                <span class="caption">Nhà cung cấp</span>
            </a>
        </li>

        <li class="item-header">KHO VẬT TƯ</li>
        <li class="<%=b12 %>">
            <a href="/gianhang/admin/quan-ly-vat-tu/Default.aspx">
                <span class="icon"><span class="mif-tools"></span></span>
                <span class="caption">Quản lý vật tư</span>
            </a>
        </li>
        <li class="<%=b13 %>">
            <a href="/gianhang/admin/quan-ly-vat-tu/add.aspx">
                <span class="icon"><span class="mif-plus"></span></span>
                <span class="caption">Thêm vật tư</span>
            </a>
        </li>
        <li class="<%=b10 %>">
            <a href="/gianhang/admin/quan-ly-vat-tu/kho-vat-tu.aspx">
                <span class="icon"><span class="mif-shop"></span></span>
                <span class="caption">Kho vật tư</span>
            </a>
        </li>
        <li class="<%=b11 %>">
            <a href="/gianhang/admin/quan-ly-vat-tu/nhap-vat-tu.aspx?q=nh">
                <span class="icon"><span class="mif-plus"></span></span>
                <span class="caption">Nhập vật tư</span>
            </a>
        </li>
        <li class="<%=b9 %>">
            <a href="/gianhang/admin/quan-ly-vat-tu/vat-tu-da-nhap.aspx">
                <span class="icon"><span class="mif-open-book"></span></span>
                <span class="caption">Vật tư đã nhập</span>
            </a>
        </li>
        <li class="<%=b8 %>">
            <a href="/gianhang/admin/quan-ly-vat-tu/nhom-vat-tu.aspx">
                <span class="icon"><span class="mif-dashboard"></span></span>
                <span class="caption">Quản lý nhóm vật tư</span>
            </a>
        </li>

        <li class="item-header">QUẢN LÝ BÁN HÀNG</li>
        <li class="<%=atv_ql_hoadon %>">
            <a href="/gianhang/admin/quan-ly-hoa-don/Default.aspx">
                <span class="icon"><span class="mif-open-book"></span></span>
                <span class="caption">Quản lý hóa đơn</span>
                <%--<span class="badges ml-auto mr-3">
                    <span class="badge inline bg-orange fg-white"><%=hoadon_class.return_soluong_hoadon(user_parent) %></span>
                </span>--%>
            </a>
        </li>
        <li class="<%=atv_ql_lstt %>">
            <a href="/gianhang/admin/quan-ly-hoa-don/lich-su-thanh-toan.aspx">
                <span class="icon"><span class="mif-calendar"></span></span>
                <span class="caption">Lịch sử thanh toán</span>
            </a>
        </li>
        <li class="<%=atv_ql_lsbh %>">
            <a href="/gianhang/admin/quan-ly-hoa-don/lich-su-ban-hang.aspx">
                <span class="icon"><span class="mif-calendar"></span></span>
                <span class="caption">Lịch sử bán hàng</span>
            </a>
        </li>

        <li class="item-header">KHÁCH HÀNG & LỊCH HẸN</li>
        <li class="<%=atv_kh_dtkh %>">
            <a href="/gianhang/admin/quan-ly-khach-hang/Default.aspx">
                <span class="icon"><span class="mif-database"></span></span>
                <span class="caption">Data khách hàng</span>
                <span class="badges ml-auto mr-3">
                    <span class="badge inline bg-red fg-white"><%=data_khachhang_class.return_soluong_khachhang(user_parent) %></span>
                </span>
            </a>
        </li>
        <li class="<%=b0 %>">
            <a href="/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx">
                <span class="icon"><span class="mif-calendar"></span></span>
                <span class="caption">Danh sách lịch hẹn</span>
                <span class="badges ml-auto mr-3">
                    <span class="badge inline bg-cyan fg-white"><%=datlich_class.return_lich_chauxacnhan() %></span>
                </span>
            </a>
        </li>
        <li class="<%=b1 %>">
            <a href="/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx?q=add">
                <span class="icon"><span class="mif-plus"></span></span>
                <span class="caption">Đặt lịch hẹn</span>
                <%--<span class="caption">Thống kê đặt lịch</span>--%>
            </a>
        </li>
        <li class="<%=b2 %>">
            <a href="/gianhang/admin/quan-ly-khach-hang/nhom-khach-hang.aspx">
                <span class="icon"><span class="mif-dashboard"></span></span>
                <span class="caption">Quản lý nhóm khách hàng</span>
            </a>
        </li>
        <%--<li class="<%=b2 %>">
            <a href="/gianhang/admin/quan-ly-khach-hang/bang-lich-hen.aspx">
                <span class="icon"><span class="mif-table"></span></span>
                <span class="caption">Xem bảng lịch hẹn</span>
            </a>
        </li>--%>

        <li class="item-header">THẺ DỊCH VỤ</li>
        <li class="<%=b7 %>">
            <a href="/gianhang/admin/quan-ly-the-dich-vu/Default.aspx">
                <span class="icon"><span class="mif-credit-card"></span></span>
                <span class="caption">Quản lý thẻ dịch vụ</span>
            </a>
        </li>
        <li class="">
            <a href="/gianhang/admin/quan-ly-the-dich-vu/Default.aspx?q=add">
                <span class="icon"><span class="mif-plus"></span></span>
                <span class="caption">Bán thẻ dịch vụ</span>
                <%--<span class="caption">Thống kê thẻ dịch vụ</span>--%>
            </a>
        </li>


        <li class="item-header">QUẢN LÝ THU CHI</li>
        <li class="<%=a7 %>">
            <a href="/gianhang/admin/quan-ly-thu-chi/Default.aspx">
                <span class="icon"><span class="mif-dollars"></span></span>
                <span class="caption">Danh sách thu chi</span>
            </a>
        </li>
        <li class="<%=a8 %>">
            <a href="/gianhang/admin/quan-ly-thu-chi/add.aspx">
                <span class="icon"><span class="mif-plus"></span></span>
                <span class="caption">Tạo thu chi</span>
            </a>
        </li>
        <li class="<%=a9 %>">
            <a href="/gianhang/admin/quan-ly-thu-chi/nhom-thu-chi.aspx">
                <span class="icon"><span class="mif-dashboard"></span></span>
                <span class="caption">Quản lý loại thu chi</span>
            </a>
        </li>



        <li class="item-header">THỐNG KÊ</li>
        <li class="<%=tk1 %>">
            <a href="/gianhang/admin/quan-ly-hoa-don/thong-ke-dich-vu.aspx">
                <span class="icon"><span class="mif-chevron-right"></span></span>
                <span class="caption">Thống kê dịch vụ</span>
            </a>
        </li>
        <li class="<%=tk2 %>">
            <a href="/gianhang/admin/quan-ly-hoa-don/thong-ke-san-pham.aspx">
                <span class="icon"><span class="mif-chevron-right"></span></span>
                <span class="caption">Thống kê sản phẩm</span>
            </a>
        </li>

        <%--<li class="<%=atv_ql_nhanvien %>">
            <a href="/gianhang/admin/quan-ly-tai-khoan/Default.aspx">
                <span class="icon"><span class="mif-users"></span></span>
                <span class="caption">Quản lý nhân viên</span>
                <span class="badges ml-auto mr-3">
                    <span class="badge inline bg-red fg-white"><%=taikhoan_class.return_soluong_nhanvien(user_parent) %></span>
                </span>
            </a>
        </li>--%>
        <%--<li class="<%=atv_ql_thuchi %>">
            <a href="/gianhang/admin/quan-ly-thu-chi/Default.aspx">
                <span class="icon"><span class="mif-dollars"></span></span>
                <span class="caption">Quản lý thu chi</span>
            </a>
        </li>
        <li class="<%=atv_ql_lstt %>">
            <a href="/gianhang/admin/lich-su-thanh-toan/default.aspx">
                <span class="icon"><span class="mif-calendar"></span></span>
                <span class="caption">Lịch sử thanh toán</span>
            </a>
        </li>
        <li class="<%=atv_ql_lsbh %>">
            <a href="/gianhang/admin/lich-su-ban-hang/default.aspx">
                <span class="icon"><span class="mif-calendar"></span></span>
                <span class="caption">Lịch sử bán hàng</span>
            </a>
        </li>--%>
        <%--<li class="item-header">KHÁCH HÀNG</li>
        <li class="<%=atv_kh_dtkh %>">
            <a href="/gianhang/admin/khach-hang/default.aspx">
                <span class="icon"><span class="mif-database"></span></span>
                <span class="caption">Data khách hàng</span>
                <span class="badges ml-auto mr-3">
                    <span class="badge inline bg-red fg-white"><%=data_khachhang_class.return_soluong_khachhang(user_parent) %></span>
                </span>
            </a>
        </li>
        <li class="">
            <a href="#">
                <span class="icon"><span class="mif-calendar"></span></span>
                <span class="caption">Đặt lịch hẹn</span>
            </a>
        </li>--%>
        <%--<li class="item-header">NHÂN VIÊN</li>
        <li class="<%=atv_chamcong_nv %>">
            <a href="/gianhang/admin/quan-ly-tai-khoan/cham-cong-nhan-vien.aspx">
                <span class="icon"><span class="mif-checkmark"></span></span>
                <span class="caption">Chấm công nhân viên</span>
            </a>
        </li>
        <li class="<%=atv_bangchamcong_nv %>">
            <a href="/gianhang/admin/quan-ly-tai-khoan/bang-cham-cong.aspx">
                <span class="icon"><span class="mif-table"></span></span>
                <span class="caption">Bảng chấm công</span>
            </a>
        </li>
        <li class="<%=atv_doanhso_nv %>">
            <a href="/gianhang/admin/quan-ly-tai-khoan/doanh-so-nhan-vien.aspx">
                <span class="icon"><span class="mif-chart-bars2"></span></span>
                <span class="caption">Doanh số nhân viên</span>
            </a>
        </li>
        <li class="<%=atv_tinhluong_nv %>">
            <a href="/gianhang/admin/quan-ly-tai-khoan/tinh-luong-nhan-vien.aspx">
                <span class="icon"><span class="mif-money"></span></span>
                <span class="caption">Tính lương nhân viên</span>
            </a>
        </li>
        <li class="item-header">DỊCH VỤ</li>
        <li class="<%=atv_thongke_dichvu %>">
            <a href="/gianhang/admin/quan-ly-dich-vu/thong-ke-dich-vu.aspx">
                <span class="icon"><span class="mif-chart-bars"></span></span>
                <span class="caption">Thống kê dịch vụ</span>
            </a>
        </li>

        <li class="item-header">SẢN PHẨM</li>
        <li class="<%=atv_thongke_bansp %>">
            <a href="/gianhang/admin/quan-ly-san-pham/thong-ke-ban-hang.aspx">
                <span class="icon"><span class="mif-chart-dots"></span></span>
                <span class="caption">Thống kê bán sản phẩm</span>
            </a>
        </li>--%>

        <%--<li>
            <a href="#" class="dropdown-toggle">
                <span class="icon"><span class="mif-add-shopping-cart"></span></span>
                <span class="caption">Заявки</span>
            </a>
            <ul class="navview-menu" data-role="dropdown">
                <li class="item-header">Заявки</li>
                <li>
                    <a href="#">
                        <span class="icon"><span class="mif-folder-plus"></span></span>
                        <span class="caption">Ожидают регистрацию</span>
                    </a>
                </li>
            </ul>
        </li>--%>
    </ul>
</div>
