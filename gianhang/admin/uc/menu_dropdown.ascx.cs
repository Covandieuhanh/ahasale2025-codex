using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_uc_menu_dropdown : System.Web.UI.UserControl
{
    dbDataContext db = new dbDataContext();
    public string user_parent, user;
    public int sl_thu_chuadoc = 0;

    public string mn0_1, mn0_2, mn0_3;
    public string mn1_1, mn1_2, mn1_3, mn1_4, mn1_5;
    public string mn1_5_1;

    public string atv_caidatchung, atv_ql_hoadon, atv_ql_dichvu, atv_ql_sanpham, atv_ql_nhanvien, atv_ql_thuchi;
    public string atv_doanhso_nv, atv_chamcong_nv, atv_bangchamcong_nv, atv_tinhluong_nv, atv_ql_lstt, atv_ql_lsbh, atv_kh_dtkh, atv_ql_connguoi;
    public string atv_gh_space, atv_gh_center, atv_gh_present, atv_gh_public, atv_gh_content, atv_gh_products, atv_gh_services, atv_gh_articles, atv_gh_customers, atv_gh_bookings, atv_gh_create, atv_gh_orders, atv_gh_wait, atv_gh_buyer, atv_gh_cart, atv_gh_einvoice, atv_gh_utils, atv_gh_report;

    public string a0, a1, a11, a12, a13, a14, a15, a2, a3, a4, a5, a51, a6, a7, a8, a9;
    public string b0, b1, b2, b3, b4, b5, b6, b7, b8, b9, b10, b11, b12, b13, b14, b15, b16, b17, b18, b19,b20;
    public string tk1, tk2;

    protected void Page_Load(object sender, EventArgs e)
    {
        user = GianHangAdminContext_cl.ResolveDisplayAccountKey();
        user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();
        if (Session["index_loc_phanloai_baiviet"] == null) Session["index_loc_phanloai_baiviet"] = "1";
        if (!IsPostBack)
        {
        }
        string _url = HttpContext.Current.Request.Url.AbsolutePath.ToLower().Trim();
        if (_url.StartsWith("/gianhang/admin"))
            _url = _url.Replace("/gianhang", "");
        switch (_url)
        {
            //quản lý hệ thống
            // case ("/badmin/quan-ly-he-thong/cai-dat-tai-khoan/default.aspx"): mn0_1 = "active"; break;

            //DANH MỤC CHÍNH
            //cài đặt chung
            //old
            //case ("/admin/cai-dat-chung/default.aspx"): atv_caidatchung = "active"; Session["title"] = "Cài đặt chung"; break;
            //quản lý nhân viên
            //case ("/admin/quan-ly-tai-khoan/bo-phan.aspx"): atv_ql_nhanvien = "active"; break;



            //TỪ BCORN SANG
            //cấu hình chung
            case ("/admin/cau-hinh-chung/tao-lien-ket-chia-se.aspx"): a1 = "active"; a11 = "active"; Session["title"] = "Tạo liên kết chia sẻ"; break;
            case ("/admin/cau-hinh-chung/nhung-ma-vao-website.aspx"): a1 = "active"; a12 = "active"; Session["title"] = "Nhúng mã"; break;
            case ("/admin/cau-hinh-chung/cap-nhat-thong-tin.aspx"): a1 = "active"; a13 = "active"; Session["title"] = "Cập nhật thông tin"; break;
            case ("/admin/cau-hinh-chung/link-social-media.aspx"): a1 = "active"; a14 = "active"; Session["title"] = "Link social media"; break;
            case ("/admin/cau-hinh-chung/cai-dat-bao-tri.aspx"): a1 = "active"; a15 = "active"; Session["title"] = "Cài đặt bảo trì"; break;

            //quản lý tài khoản nhân viên
            case ("/admin/quan-ly-tai-khoan/default.aspx"): a2 = "active"; Session["title"] = "Quản lý tài khoản"; break;
            case ("/admin/quan-ly-tai-khoan/phan-quyen.aspx"): a2 = "active"; Session["title"] = "Phân quyền"; break;
            case ("/admin/quan-ly-tai-khoan/doi-mat-khau.aspx"): a2 = "active"; Session["title"] = "Đổi mật khẩu"; break;
            case ("/admin/quan-ly-tai-khoan/edit.aspx"): a2 = "active"; Session["title"] = "Chỉnh sửa tài khoản"; break;
            case ("/admin/quan-ly-tai-khoan/tai-khoan.aspx"): a2 = "active"; Session["title"] = "Chi tiết tài khoản"; break;
            case ("/admin/quan-ly-tai-khoan/add.aspx"): a2 = "active"; Session["title"] = "Tạo tài khoản"; break;
            case ("/admin/quan-ly-tai-khoan/doanh-so-nhan-vien.aspx"): atv_doanhso_nv = "active"; Session["title"] = "Doanh số nhân viên"; break;
            case ("/admin/quan-ly-tai-khoan/chi-tiet-doanh-so.aspx"): atv_doanhso_nv = "active"; Session["title"] = "Chi tiết doanh số"; break;
            case ("/admin/quan-ly-tai-khoan/cham-cong-nhan-vien.aspx"): atv_chamcong_nv = "active"; Session["title"] = "Chấm công"; break;
            case ("/admin/quan-ly-tai-khoan/bang-cham-cong.aspx"): atv_bangchamcong_nv = "active"; Session["title"] = "Bảng chấm công"; break;
            case ("/admin/quan-ly-tai-khoan/tinh-luong-nhan-vien.aspx"): atv_tinhluong_nv = "active"; Session["title"] = "Lương nhân viên"; break;

            //quản lý menu
            case ("/admin/quan-ly-menu/default.aspx"): a3 = "active"; Session["title"] = "Quản lý menu"; break;
            case ("/admin/quan-ly-menu/add.aspx"): a3 = "active"; Session["title"] = "Thêm menu"; break;
            case ("/admin/quan-ly-menu/edit.aspx"): a3 = "active"; Session["title"] = "Chỉnh sửa menu"; break;
            case ("/admin/quan-ly-menu/cay-menu.aspx"): a3 = "active"; Session["title"] = "Cây menu"; break;
            case ("/admin/quan-ly-menu/xem-truoc.aspx"): a3 = "active"; Session["title"] = "Xem trước menu"; break;

            //quản lý bài viết
            case ("/admin/quan-ly-bai-viet/default.aspx"):
                if (Session["index_loc_phanloai_baiviet"].ToString() == "3")
                {
                    atv_ql_dichvu = "active"; Session["title"] = "Quản lý dịch vụ";
                }
                else
                {
                    if (Session["index_loc_phanloai_baiviet"].ToString() == "2")
                    {
                        atv_ql_sanpham = "active"; Session["title"] = "Quản lý sản phẩm";
                    }
                    else
                    {
                        a4 = "active"; Session["title"] = "Quản lý bài viết";
                    }
                }
                break;
            case ("/admin/quan-ly-bai-viet/add.aspx"): a4 = "active"; Session["title"] = "Thêm bài viết"; break;
            case ("/admin/quan-ly-bai-viet/edit.aspx"): a4 = "active"; Session["title"] = "Chỉnh sửa bài viết"; break;

            //quản lý module
            case ("/admin/quan-ly-module/slide-anh/default.aspx"): a5 = "active"; a51 = "active"; Session["title"] = "Quản lý slide ảnh"; break;
            case ("/admin/quan-ly-module/slide-anh/add.aspx"): a5 = "active"; a51 = "active"; Session["title"] = "Thêm ảnh slide"; break;
            case ("/admin/quan-ly-module/slide-anh/edit.aspx"): a5 = "active"; a51 = "active"; Session["title"] = "Chỉnh sửa ảnh slide"; break;
            case ("/admin/quan-ly-module/slide-anh/xemtruoc.aspx"): a5 = "active"; a51 = "active"; Session["title"] = "Xem trước slide"; break;

            //data yêu cầu tư vấn
            case ("/admin/yeu-cau-tu-van/default.aspx"): a6 = "active"; Session["title"] = "Yêu cầu tư vấn"; break;

            //QUẢN LÝ BÁN HÀNG
            case ("/admin/gianhang/default.aspx"): atv_gh_space = "active"; Session["title"] = "Không gian /gianhang"; break;
            case ("/admin/gianhang/trung-tam.aspx"): atv_gh_center = "active"; Session["title"] = "Trung tâm /gianhang"; break;
            case ("/admin/gianhang/trinh-bay.aspx"): atv_gh_present = "active"; Session["title"] = "Trình bày storefront"; break;
            case ("/admin/gianhang/trang-cong-khai.aspx"): atv_gh_public = "active"; Session["title"] = "Trang công khai /gianhang"; break;
            case ("/admin/gianhang/quan-ly-noi-dung.aspx"): atv_gh_content = "active"; Session["title"] = "Nội dung /gianhang"; break;
            case ("/admin/gianhang/san-pham.aspx"): atv_gh_products = "active"; Session["title"] = "Sản phẩm /gianhang"; break;
            case ("/admin/gianhang/san-pham-chi-tiet.aspx"): atv_gh_products = "active"; Session["title"] = "Chi tiết sản phẩm /gianhang"; break;
            case ("/admin/gianhang/dich-vu.aspx"): atv_gh_services = "active"; Session["title"] = "Dịch vụ /gianhang"; break;
            case ("/admin/gianhang/dich-vu-chi-tiet.aspx"): atv_gh_services = "active"; Session["title"] = "Chi tiết dịch vụ /gianhang"; break;
            case ("/admin/gianhang/bai-viet.aspx"): atv_gh_articles = "active"; Session["title"] = "Bài viết /gianhang"; break;
            case ("/admin/gianhang/bai-viet-chi-tiet.aspx"): atv_gh_articles = "active"; Session["title"] = "Chi tiết bài viết /gianhang"; break;
            case ("/admin/gianhang/khach-hang.aspx"): atv_gh_customers = "active"; Session["title"] = "Khách hàng /gianhang"; break;
            case ("/admin/gianhang/khach-hang-chi-tiet.aspx"): atv_gh_customers = "active"; Session["title"] = "Chi tiết khách hàng /gianhang"; break;
            case ("/admin/gianhang/lich-hen.aspx"): atv_gh_bookings = "active"; Session["title"] = "Lịch hẹn /gianhang"; break;
            case ("/admin/gianhang/lich-hen-chi-tiet.aspx"): atv_gh_bookings = "active"; Session["title"] = "Chi tiết lịch hẹn /gianhang"; break;
            case ("/admin/gianhang/tao-giao-dich.aspx"): atv_gh_create = "active"; Session["title"] = "Tạo giao dịch"; break;
            case ("/admin/gianhang/don-ban.aspx"): atv_gh_orders = "active"; Session["title"] = "Đơn gian hàng"; break;
            case ("/admin/gianhang/cho-thanh-toan.aspx"): atv_gh_wait = "active"; Session["title"] = "Chờ thanh toán"; break;
            case ("/admin/gianhang/don-mua.aspx"): atv_gh_buyer = "active"; Session["title"] = "Buyer-flow / Đơn mua"; break;
            case ("/admin/gianhang/gio-hang.aspx"): atv_gh_cart = "active"; Session["title"] = "Giỏ hàng /gianhang"; break;
            case ("/admin/gianhang/hoa-don-dien-tu.aspx"): atv_gh_einvoice = "active"; Session["title"] = "Hóa đơn điện tử /gianhang"; break;
            case ("/admin/gianhang/tien-ich.aspx"): atv_gh_utils = "active"; Session["title"] = "Tiện ích /gianhang"; break;
            case ("/admin/gianhang/tien-ich-co-cau.aspx"): atv_gh_utils = "active"; Session["title"] = "Cơ cấu /gianhang"; break;
            case ("/admin/gianhang/tien-ich-quay-so.aspx"): atv_gh_utils = "active"; Session["title"] = "Quay số /gianhang"; break;
            case ("/admin/gianhang/bao-cao.aspx"): atv_gh_report = "active"; Session["title"] = "Báo cáo gian hàng"; break;
            case ("/admin/quan-ly-hoa-don/default.aspx"): atv_ql_hoadon = "active"; Session["title"] = "Quản lý hóa đơn"; break;
            case ("/admin/quan-ly-hoa-don/chi-tiet.aspx"): atv_ql_hoadon = "active"; Session["title"] = "Chi tiết hóa đơn"; break;
            case ("/admin/quan-ly-hoa-don/lich-su-thanh-toan.aspx"): atv_ql_lstt = "active"; Session["title"] = "Lịch sử thanh toán"; break;
            case ("/admin/quan-ly-hoa-don/lich-su-ban-hang.aspx"): atv_ql_lsbh = "active"; Session["title"] = "Lịch sử bán hàng"; break;
            case ("/admin/quan-ly-hoa-don/edit-cthd.aspx"): atv_ql_hoadon = "active"; Session["title"] = "Chỉnh sửa hóa đơn"; break;

            //THỐNG KÊ
            case ("/admin/quan-ly-hoa-don/thong-ke-dich-vu.aspx"): tk1 = "active"; Session["title"] = "Thống kê dịch vụ"; break;
            case ("/admin/quan-ly-hoa-don/thong-ke-san-pham.aspx"): tk2 = "active"; Session["title"] = "Thống kê sản phẩm"; break;


            //KHÁCH HÀNG
            //data khách hàng
            case ("/admin/quan-ly-khach-hang/default.aspx"): atv_kh_dtkh = "active"; Session["title"] = "Data khách hàng"; break;
            case ("/admin/quan-ly-khach-hang/chi-tiet.aspx"): atv_kh_dtkh = "active"; Session["title"] = "Info khách hàng"; break;
            case ("/admin/quan-ly-con-nguoi/default.aspx"): atv_ql_connguoi = "active"; Session["title"] = "Hồ sơ người"; break;
            case ("/admin/quan-ly-con-nguoi/chi-tiet.aspx"): atv_ql_connguoi = "active"; Session["title"] = "Chi tiết hồ sơ người"; break;
            case ("/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx"):
                if (!string.IsNullOrWhiteSpace(Request.QueryString["q"]) && Request.QueryString["q"].ToString().Trim().ToLower() == "add")
                {
                    b1 = "active";
                    Session["title"] = "Đặt lịch hẹn";
                }
                else
                {
                    b0 = "active";
                    Session["title"] = "Lịch hẹn";
                }
                break;
            case ("/admin/quan-ly-khach-hang/sua-lich-hen.aspx"): b0 = "active"; Session["title"] = "Sửa lịch hẹn"; break;
            case ("/admin/quan-ly-khach-hang/nhom-khach-hang.aspx"): b2 = "active"; Session["title"] = "Nhóm khách hàng"; break;
            //case ("/admin/quan-ly-khach-hang/bang-lich-hen.aspx"): b2 = "active"; Session["title"] = "Bảng lịch hẹn"; break;

            //THU CHI
            case ("/admin/quan-ly-thu-chi/default.aspx"): a7 = "active"; Session["title"] = "Danh sách thu chi"; break;
            case ("/admin/quan-ly-thu-chi/nhom-thu-chi.aspx"): a9 = "active"; Session["title"] = "Loại thu chi"; break;
            case ("/admin/quan-ly-thu-chi/add.aspx"): a8 = "active"; Session["title"] = "Tạo thu chi"; break;
            case ("/admin/quan-ly-thu-chi/edit.aspx"): a7 = "active"; Session["title"] = "Sửa thu chi"; break;
            case ("/admin/quan-ly-thu-chi/so-quy-tien-mat.aspx"): a7 = "active"; Session["title"] = "Sổ quỹ tiền mặt"; break;

            //KHO HÀNG
            case ("/admin/quan-ly-kho-hang/default.aspx"): b3 = "active"; Session["title"] = "Kho"; break;
            case ("/admin/quan-ly-kho-hang/nhap-hang.aspx"): b4 = "active"; Session["title"] = "Nhập mỹ phẩm"; break;
            case ("/admin/quan-ly-kho-hang/don-nhap-hang.aspx"): b5 = "active"; Session["title"] = "Mỹ phẩm đã nhập"; break;
            case ("/admin/quan-ly-kho-hang/chi-tiet-nhap-hang.aspx"): b5 = "active"; Session["title"] = "Chi tiết đơn nhập"; break;
            case ("/admin/quan-ly-kho-hang/edit-cthd.aspx"): b5 = "active"; Session["title"] = "Sửa đơn nhập"; break;
            case ("/admin/quan-ly-kho-hang/nha-cung-cap.aspx"): b6 = "active"; Session["title"] = "Nhà cung cấp"; break;

            //VẬT TƯ
            case ("/admin/quan-ly-vat-tu/default.aspx"): b12 = "active"; Session["title"] = "Quản lý vật tư"; break;
            case ("/admin/quan-ly-vat-tu/add.aspx"): b13 = "active"; Session["title"] = "Thêm vật tư"; break;
            case ("/admin/quan-ly-vat-tu/edit.aspx"): b12 = "active"; Session["title"] = "Sửa vật tư"; break;

            case ("/admin/quan-ly-vat-tu/kho-vat-tu.aspx"): b10 = "active"; Session["title"] = "Kho vật tư"; break;
            case ("/admin/quan-ly-vat-tu/nhap-vat-tu.aspx"): b11 = "active"; Session["title"] = "Nhập vật tư"; break;
            case ("/admin/quan-ly-vat-tu/vat-tu-da-nhap.aspx"): b9 = "active"; Session["title"] = "Vật tư đã nhập"; break;
            case ("/admin/quan-ly-vat-tu/chi-tiet-nhap-hang.aspx"): b9 = "active"; Session["title"] = "Đơn nhập vật tư"; break;
            case ("/admin/quan-ly-vat-tu/edit-cthd.aspx"): b9 = "active"; Session["title"] = "Sửa đơn vật tư"; break;
            case ("/admin/quan-ly-vat-tu/nhom-vat-tu.aspx"): b8 = "active"; Session["title"] = "Nhóm vật tư"; break;

            case ("/admin/quan-ly-thong-bao/default.aspx"): Session["title"] = "Quản lý thông báo"; break;

            //THẺ DỊCH VỤ
            case ("/admin/quan-ly-the-dich-vu/default.aspx"): b7 = "active"; Session["title"] = "Thẻ dịch vụ"; break;
            case ("/admin/quan-ly-the-dich-vu/chi-tiet.aspx"): b7 = "active"; Session["title"] = "Sửa thẻ dịch vụ"; break;

            //Chuyên gia
            case ("/admin/quan-ly-giang-vien/default.aspx"): b14 = "active"; Session["title"] = "Chuyên gia"; break;
            case ("/admin/quan-ly-giang-vien/edit.aspx"): b14 = "active"; Session["title"] = "Sửa Chuyên gia"; break;
            case ("/admin/quan-ly-giang-vien/add.aspx"): b15 = "active"; Session["title"] = "Thêm Chuyên gia"; break;

            //thành viên
            case ("/admin/quan-ly-hoc-vien/default.aspx"): b16 = "active"; Session["title"] = "thành viên"; break;
            case ("/admin/quan-ly-hoc-vien/edit.aspx"): b16 = "active"; Session["title"] = "Sửa thành viên"; break;
            case ("/admin/quan-ly-hoc-vien/add.aspx"): b17 = "active"; Session["title"] = "Thêm thành viên"; break;

            //QL HỆ THỐNG
            case ("/admin/quan-ly-he-thong/chi-nhanh.aspx"): b20 = "active"; Session["title"] = "Quản lý chi nhánh"; break;
            case ("/admin/quan-ly-he-thong/nganh.aspx"): b18 = "active"; Session["title"] = "Quản lý ngành"; break;
            case ("/admin/quan-ly-he-thong/phong-ban.aspx"): b19 = "active"; Session["title"] = "Quản lý phòng ban"; break;

            default: a0 = "active"; Session["title"] = "Trang chủ"; break;
        }

        Session["title"] = GianHangAdminRouteMap_cl.ResolveTitle(Request);
    }
}
