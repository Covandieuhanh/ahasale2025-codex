using System;
using System.Web;

public static class GianHangAdminRouteMap_cl
{
    private static string NormalizePath(string path)
    {
        return (path ?? "").Trim().ToLowerInvariant();
    }

    public static string ResolveTitle(HttpRequest request)
    {
        if (request == null || request.Url == null)
            return "Trang chủ";

        return ResolveTitle(request.Url.AbsolutePath, request);
    }

    public static string ResolveTitle(string path, HttpRequest request)
    {
        string normalized = NormalizePath(path);
        if (normalized == "")
            return "Trang chủ";

        if (normalized == "/gianhang/admin/default.aspx"
            || normalized == "/gianhang/admin/"
            || normalized == "/gianhang/admin")
            return "Trang chủ";

        if (normalized == "/gianhang/admin/cau-hinh-chung/default.aspx")
            return "Cài đặt chung";
        if (normalized == "/gianhang/admin/cau-hinh-chung/tao-lien-ket-chia-se.aspx")
            return "Tạo liên kết chia sẻ";
        if (normalized == "/gianhang/admin/cau-hinh-chung/nhung-ma-vao-website.aspx")
            return "Nhúng mã";
        if (normalized == "/gianhang/admin/cau-hinh-chung/cap-nhat-thong-tin.aspx")
            return "Cập nhật thông tin";
        if (normalized == "/gianhang/admin/cau-hinh-chung/link-social-media.aspx")
            return "Link social media";
        if (normalized == "/gianhang/admin/cau-hinh-chung/cai-dat-bao-tri.aspx")
            return "Cài đặt bảo trì";

        if (normalized == "/gianhang/admin/cau-hinh-storefront/default.aspx")
            return "Trang công khai /gianhang";
        if (normalized == "/gianhang/admin/cau-hinh-storefront/edit-section.aspx")
            return "Chỉnh sửa block trang công khai";

        if (normalized == "/gianhang/admin/quan-ly-tai-khoan/default.aspx")
        {
            string scope = ((request == null ? "" : request.QueryString["scope"]) ?? "").Trim().ToLowerInvariant();
            if (scope == "admin")
                return "Quản lý tài khoản admin";
            if (scope == "home")
                return "Quản lý tài khoản home";
            if (scope == "shop")
                return "Quản lý tài khoản gian hàng đối tác";
            return "Quản lý tài khoản";
        }
        if (normalized == "/gianhang/admin/quan-ly-tai-khoan/phan-quyen.aspx")
            return "Phân quyền";
        if (normalized == "/gianhang/admin/quan-ly-tai-khoan/doi-mat-khau.aspx")
            return "Đổi mật khẩu";
        if (normalized == "/gianhang/admin/quan-ly-tai-khoan/edit.aspx")
            return "Chỉnh sửa tài khoản";
        if (normalized == "/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx")
            return "Chi tiết tài khoản";
        if (normalized == "/gianhang/admin/quan-ly-tai-khoan/add.aspx")
            return "Tạo tài khoản";
        if (normalized == "/gianhang/admin/quan-ly-tai-khoan/doanh-so-nhan-vien.aspx")
            return "Doanh số nhân viên";
        if (normalized == "/gianhang/admin/quan-ly-tai-khoan/chi-tiet-doanh-so.aspx")
            return "Chi tiết doanh số";
        if (normalized == "/gianhang/admin/quan-ly-tai-khoan/cham-cong-nhan-vien.aspx")
            return "Chấm công";
        if (normalized == "/gianhang/admin/quan-ly-tai-khoan/bang-cham-cong.aspx")
            return "Bảng chấm công";
        if (normalized == "/gianhang/admin/quan-ly-tai-khoan/tinh-luong-nhan-vien.aspx")
            return "Lương nhân viên";

        if (normalized == "/gianhang/admin/quan-ly-menu/default.aspx")
            return "Quản lý menu";
        if (normalized == "/gianhang/admin/quan-ly-menu/add.aspx")
            return "Thêm menu";
        if (normalized == "/gianhang/admin/quan-ly-menu/edit.aspx")
            return "Chỉnh sửa menu";
        if (normalized == "/gianhang/admin/quan-ly-menu/cay-menu.aspx")
            return "Cây menu";
        if (normalized == "/gianhang/admin/quan-ly-menu/xem-truoc.aspx")
            return "Xem trước menu";

        if (normalized == "/gianhang/admin/quan-ly-bai-viet/default.aspx")
        {
            string pl = ((request == null ? "" : request.QueryString["pl"]) ?? "").Trim().ToLowerInvariant();
            if (pl == "ctsp")
                return "Quản lý sản phẩm";
            if (pl == "ctdv")
                return "Quản lý dịch vụ";
            return "Quản lý bài viết";
        }
        if (normalized == "/gianhang/admin/quan-ly-bai-viet/add.aspx")
            return "Thêm bài viết";
        if (normalized == "/gianhang/admin/quan-ly-bai-viet/edit.aspx")
            return "Chỉnh sửa bài viết";
        if (normalized == "/gianhang/admin/quan-ly-bai-viet/in/a4.aspx")
            return "In bài viết";

        if (normalized == "/gianhang/admin/quan-ly-module/slide-anh/default.aspx")
            return "Quản lý slide ảnh";
        if (normalized == "/gianhang/admin/quan-ly-module/slide-anh/add.aspx")
            return "Thêm ảnh slide";
        if (normalized == "/gianhang/admin/quan-ly-module/slide-anh/edit.aspx")
            return "Chỉnh sửa ảnh slide";
        if (normalized == "/gianhang/admin/quan-ly-module/slide-anh/xemtruoc.aspx")
            return "Xem trước slide";

        if (normalized == "/gianhang/admin/yeu-cau-tu-van/default.aspx")
            return "Yêu cầu tư vấn";
        if (normalized == "/gianhang/admin/quan-ly-thong-bao/default.aspx")
            return "Quản lý thông báo";

        if (normalized == "/gianhang/admin/quan-ly-hoa-don/default.aspx")
            return "Quản lý hóa đơn";
        if (normalized == "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx")
            return "Chi tiết hóa đơn";
        if (normalized == "/gianhang/admin/quan-ly-hoa-don/lich-su-thanh-toan.aspx")
            return "Lịch sử thanh toán";
        if (normalized == "/gianhang/admin/quan-ly-hoa-don/lich-su-ban-hang.aspx")
            return "Lịch sử bán hàng";
        if (normalized == "/gianhang/admin/quan-ly-hoa-don/edit-cthd.aspx")
            return "Chỉnh sửa hóa đơn";
        if (normalized == "/gianhang/admin/quan-ly-hoa-don/thong-ke-dich-vu.aspx")
            return "Thống kê dịch vụ";
        if (normalized == "/gianhang/admin/quan-ly-hoa-don/thong-ke-san-pham.aspx")
            return "Thống kê sản phẩm";

        if (normalized == "/gianhang/admin/quan-ly-khach-hang/default.aspx")
            return "Data khách hàng";
        if (normalized == "/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx")
            return "Info khách hàng";
        if (normalized == "/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx")
        {
            string q = ((request == null ? "" : request.QueryString["q"]) ?? "").Trim().ToLowerInvariant();
            return q == "add" ? "Đặt lịch hẹn" : "Lịch hẹn";
        }
        if (normalized == "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx")
            return "Sửa lịch hẹn";
        if (normalized == "/gianhang/admin/quan-ly-khach-hang/nhom-khach-hang.aspx")
            return "Nhóm khách hàng";

        if (normalized == "/gianhang/admin/quan-ly-con-nguoi/default.aspx")
            return "Hồ sơ người";
        if (normalized == "/gianhang/admin/quan-ly-con-nguoi/chi-tiet.aspx")
            return "Chi tiết hồ sơ người";

        if (normalized == "/gianhang/admin/quan-ly-thu-chi/default.aspx")
            return "Danh sách thu chi";
        if (normalized == "/gianhang/admin/quan-ly-thu-chi/nhom-thu-chi.aspx")
            return "Loại thu chi";
        if (normalized == "/gianhang/admin/quan-ly-thu-chi/add.aspx")
            return "Tạo thu chi";
        if (normalized == "/gianhang/admin/quan-ly-thu-chi/edit.aspx")
            return "Sửa thu chi";
        if (normalized == "/gianhang/admin/quan-ly-thu-chi/so-quy-tien-mat.aspx")
            return "Sổ quỹ tiền mặt";

        if (normalized == "/gianhang/admin/quan-ly-kho-hang/default.aspx")
            return "Kho";
        if (normalized == "/gianhang/admin/quan-ly-kho-hang/nhap-hang.aspx")
            return "Nhập mỹ phẩm";
        if (normalized == "/gianhang/admin/quan-ly-kho-hang/don-nhap-hang.aspx")
            return "Mỹ phẩm đã nhập";
        if (normalized == "/gianhang/admin/quan-ly-kho-hang/chi-tiet-nhap-hang.aspx")
            return "Chi tiết đơn nhập";
        if (normalized == "/gianhang/admin/quan-ly-kho-hang/edit-cthd.aspx")
            return "Sửa đơn nhập";
        if (normalized == "/gianhang/admin/quan-ly-kho-hang/nha-cung-cap.aspx")
            return "Nhà cung cấp";

        if (normalized == "/gianhang/admin/quan-ly-vat-tu/default.aspx")
            return "Quản lý vật tư";
        if (normalized == "/gianhang/admin/quan-ly-vat-tu/add.aspx")
            return "Thêm vật tư";
        if (normalized == "/gianhang/admin/quan-ly-vat-tu/edit.aspx")
            return "Sửa vật tư";
        if (normalized == "/gianhang/admin/quan-ly-vat-tu/kho-vat-tu.aspx")
            return "Kho vật tư";
        if (normalized == "/gianhang/admin/quan-ly-vat-tu/nhap-vat-tu.aspx")
            return "Nhập vật tư";
        if (normalized == "/gianhang/admin/quan-ly-vat-tu/vat-tu-da-nhap.aspx")
            return "Vật tư đã nhập";
        if (normalized == "/gianhang/admin/quan-ly-vat-tu/chi-tiet-nhap-hang.aspx")
            return "Đơn nhập vật tư";
        if (normalized == "/gianhang/admin/quan-ly-vat-tu/edit-cthd.aspx")
            return "Sửa đơn vật tư";
        if (normalized == "/gianhang/admin/quan-ly-vat-tu/nhom-vat-tu.aspx")
            return "Nhóm vật tư";

        if (normalized == "/gianhang/admin/quan-ly-the-dich-vu/default.aspx")
            return "Thẻ dịch vụ";
        if (normalized == "/gianhang/admin/quan-ly-the-dich-vu/chi-tiet.aspx")
            return "Sửa thẻ dịch vụ";

        if (normalized == "/gianhang/admin/quan-ly-giang-vien/default.aspx")
            return "Chuyên gia";
        if (normalized == "/gianhang/admin/quan-ly-giang-vien/edit.aspx")
            return "Sửa Chuyên gia";
        if (normalized == "/gianhang/admin/quan-ly-giang-vien/add.aspx")
            return "Thêm Chuyên gia";

        if (normalized == "/gianhang/admin/quan-ly-hoc-vien/default.aspx")
            return "Thành viên";
        if (normalized == "/gianhang/admin/quan-ly-hoc-vien/edit.aspx")
            return "Sửa thành viên";
        if (normalized == "/gianhang/admin/quan-ly-hoc-vien/add.aspx")
            return "Thêm thành viên";

        if (normalized == "/gianhang/admin/quan-ly-he-thong/chi-nhanh.aspx")
            return "Quản lý chi nhánh";
        if (normalized == "/gianhang/admin/quan-ly-he-thong/nganh.aspx")
            return "Quản lý ngành";
        if (normalized == "/gianhang/admin/quan-ly-he-thong/phong-ban.aspx")
            return "Quản lý phòng ban";

        return "Trang chủ";
    }
}
