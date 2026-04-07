using System;
using System.Collections.Generic;
using System.Linq;

public static class AdminFeatureRegistry_cl
{
    public sealed class FeatureDefinition
    {
        public string Key { get; set; }
        public string AccessGroupKey { get; set; }
        public string ManagementSpace { get; set; }
        public string Title { get; set; }
        public string Meaning { get; set; }
        public string ScopeLabel { get; set; }
        public string ScopeMeaning { get; set; }
        public string DefaultUrl { get; set; }
        public int SortOrder { get; set; }
    }

    private static readonly FeatureDefinition[] Definitions =
    {
        new FeatureDefinition
        {
            Key = "login",
            ManagementSpace = AdminManagementSpace_cl.SpaceAdmin,
            Title = "Đăng nhập admin",
            Meaning = "Trang đăng nhập cổng quản trị admin.",
            ScopeLabel = "Auth",
            ScopeMeaning = "Luồng xác thực vào cổng quản trị.",
            DefaultUrl = "/admin/login.aspx",
            SortOrder = 1
        },
        new FeatureDefinition
        {
            Key = "forgot_password",
            ManagementSpace = AdminManagementSpace_cl.SpaceAdmin,
            Title = "Quên mật khẩu",
            Meaning = "Luồng khôi phục mật khẩu cho tài khoản admin.",
            ScopeLabel = "Auth",
            ScopeMeaning = "Luồng hỗ trợ truy cập lại tài khoản quản trị.",
            DefaultUrl = "/admin/quen-mat-khau/default.aspx",
            SortOrder = 2
        },
        new FeatureDefinition
        {
            Key = "change_password",
            ManagementSpace = AdminManagementSpace_cl.SpaceAdmin,
            Title = "Đổi mật khẩu",
            Meaning = "Trang cập nhật mật khẩu của tài khoản admin đang đăng nhập.",
            ScopeLabel = "Auth",
            ScopeMeaning = "Bảo mật thông tin đăng nhập tài khoản quản trị.",
            DefaultUrl = "/admin/doi-mat-khau/default.aspx",
            SortOrder = 3
        },
        new FeatureDefinition
        {
            Key = "admin_dashboard",
            ManagementSpace = AdminManagementSpace_cl.SpaceAdmin,
            Title = "Trang chủ admin",
            Meaning = "Landing tổng của khối quản trị admin.",
            ScopeLabel = "Cổng Admin",
            ScopeMeaning = "Điểm vào chính của không gian quản trị admin.",
            DefaultUrl = "/admin/default.aspx",
            SortOrder = 5
        },
        new FeatureDefinition
        {
            Key = "admin_accounts",
            ManagementSpace = AdminManagementSpace_cl.SpaceAdmin,
            Title = "Quản lý tài khoản admin",
            Meaning = "Tạo tài khoản admin, gán 5 vai trò và kiểm soát đúng cổng vận hành.",
            ScopeLabel = "Cổng Admin",
            ScopeMeaning = "Toàn bộ tài khoản admin và cấu trúc phân quyền nội bộ.",
            DefaultUrl = "/admin/quan-ly-tai-khoan/Default.aspx?scope=admin&fscope=admin",
            SortOrder = 10
        },
        new FeatureDefinition
        {
            Key = "admin_otp",
            ManagementSpace = AdminManagementSpace_cl.SpaceAdmin,
            Title = "Quản lý OTP",
            Meaning = "Quản trị cấu hình OTP, nhật ký gửi mã và hỗ trợ xác thực cho các tài khoản trong hệ.",
            ScopeLabel = "Cổng Admin",
            ScopeMeaning = "Khối OTP nội bộ của hệ quản trị.",
            DefaultUrl = "/admin/otp/default.aspx",
            SortOrder = 15
        },
        new FeatureDefinition
        {
            Key = "admin_feedback",
            ManagementSpace = AdminManagementSpace_cl.SpaceAdmin,
            Title = "Quản lý góp ý",
            Meaning = "Theo dõi, lọc và xử lý các góp ý phát sinh gửi vào hệ quản trị.",
            ScopeLabel = "Cổng Admin",
            ScopeMeaning = "Toàn bộ luồng góp ý của nền tảng.",
            DefaultUrl = "/admin/quan-ly-gop-y/default.aspx",
            SortOrder = 18
        },
        new FeatureDefinition
        {
            Key = "admin_company_shop_sync",
            ManagementSpace = AdminManagementSpace_cl.SpaceAdmin,
            Title = "Đồng bộ shop công ty",
            Meaning = "Đồng bộ, làm sạch và rà soát dữ liệu shop công ty dùng cho các tác vụ nội bộ.",
            ScopeLabel = "Cổng Admin",
            ScopeMeaning = "Công cụ nội bộ dành cho vận hành admin.",
            DefaultUrl = "/admin/tools/company-shop-sync.aspx",
            SortOrder = 19
        },
        new FeatureDefinition
        {
            Key = "admin_reindex_baiviet",
            ManagementSpace = AdminManagementSpace_cl.SpaceAdmin,
            Title = "Reindex bài viết",
            Meaning = "Chạy lại chỉ mục nội dung để đồng bộ tìm kiếm và các feed bài viết trên website.",
            ScopeLabel = "Cổng Admin",
            ScopeMeaning = "Công cụ vận hành chỉ mục nội dung.",
            DefaultUrl = "/admin/tools/reindex-baiviet.aspx",
            SortOrder = 19
        },
        new FeatureDefinition
        {
            Key = "home_accounts",
            ManagementSpace = AdminManagementSpace_cl.SpaceHome,
            Title = "Quản lý tài khoản home",
            Meaning = "Rà soát hồ sơ Home, tìm kiếm và xử lý dữ liệu người dùng cá nhân.",
            ScopeLabel = "Hệ Home",
            ScopeMeaning = "Toàn bộ tài khoản Home trên các tầng khách hàng, cộng tác phát triển và đồng hành hệ sinh thái.",
            DefaultUrl = "/admin/quan-ly-tai-khoan/Default.aspx?scope=home&fscope=home",
            SortOrder = 20
        },
        new FeatureDefinition
        {
            Key = "home_point_approval",
            ManagementSpace = AdminManagementSpace_cl.SpaceHome,
            Title = "Duyệt yêu cầu xác nhận hành vi",
            Meaning = "Mở đúng tab duyệt hành vi/điểm của hệ Home theo vai trò hiện tại.",
            ScopeLabel = "Hệ Home",
            ScopeMeaning = "Khối duyệt yêu cầu hành vi và các hồ sơ điểm liên quan của hệ Home.",
            DefaultUrl = "/admin/duyet-yeu-cau-len-cap.aspx",
            SortOrder = 30
        },
        new FeatureDefinition
        {
            Key = "tier_reference",
            ManagementSpace = AdminManagementSpace_cl.SpaceHome,
            Title = "Mô tả cấp bậc",
            Meaning = "Tra cứu mô tả cấp bậc, tiêu chí hành vi và nội dung tham chiếu của hệ Home.",
            ScopeLabel = "Hệ Home",
            ScopeMeaning = "Khối tham chiếu cấp bậc và nội dung hành vi của hệ Home.",
            DefaultUrl = "/admin/MoTaCapBac.aspx",
            SortOrder = 52
        },
        new FeatureDefinition
        {
            Key = "shop_workspace",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Trung tâm quản trị gian hàng",
            Meaning = "Đi vào portal quản trị của không gian gian hàng đối tác.",
            ScopeLabel = "Hệ Gian hàng",
            ScopeMeaning = "Điểm vào chính của không gian quản trị gian hàng đối tác.",
            DefaultUrl = "/gianhang/admin/default.aspx?system=1",
            SortOrder = 55
        },
        new FeatureDefinition
        {
            Key = "shop_legacy_invoices",
            AccessGroupKey = "shop_legacy_invoice_ops",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Vận hành hóa đơn gian hàng",
            Meaning = "Truy cập module legacy hóa đơn của gian hàng trong system mode.",
            ScopeLabel = "Hệ Gian hàng",
            ScopeMeaning = "Khối quản trị hóa đơn, in ấn và đối soát đơn ở lớp legacy.",
            DefaultUrl = "/gianhang/admin/quan-ly-hoa-don/Default.aspx?workspace=gianhang&system=1",
            SortOrder = 56
        },
        new FeatureDefinition
        {
            Key = "shop_legacy_customers",
            AccessGroupKey = "shop_legacy_customer_ops",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Vận hành khách hàng gian hàng",
            Meaning = "Truy cập module legacy khách hàng và lịch hẹn của gian hàng trong system mode.",
            ScopeLabel = "Hệ Gian hàng",
            ScopeMeaning = "Khối khách hàng, lịch hẹn và hồ sơ người ở lớp legacy.",
            DefaultUrl = "/gianhang/admin/quan-ly-khach-hang/Default.aspx?system=1",
            SortOrder = 57
        },
        new FeatureDefinition
        {
            Key = "shop_legacy_content",
            AccessGroupKey = "shop_legacy_content_ops",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Vận hành nội dung gian hàng",
            Meaning = "Truy cập module legacy menu, bài viết và cấu hình hiển thị của gian hàng trong system mode.",
            ScopeLabel = "Hệ Gian hàng",
            ScopeMeaning = "Khối nội dung legacy của gian hàng: menu, bài viết, storefront config.",
            DefaultUrl = "/gianhang/admin/quan-ly-bai-viet/Default.aspx?system=1",
            SortOrder = 58
        },
        new FeatureDefinition
        {
            Key = "shop_legacy_finance",
            AccessGroupKey = "shop_legacy_finance_ops",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Vận hành thu chi gian hàng",
            Meaning = "Truy cập module legacy thu chi của gian hàng trong system mode.",
            ScopeLabel = "Hệ Gian hàng",
            ScopeMeaning = "Khối thu/chi và chứng từ tài chính nội bộ của gian hàng.",
            DefaultUrl = "/gianhang/admin/quan-ly-thu-chi/Default.aspx?system=1",
            SortOrder = 59
        },
        new FeatureDefinition
        {
            Key = "shop_legacy_inventory",
            AccessGroupKey = "shop_legacy_inventory_ops",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Vận hành kho vật tư gian hàng",
            Meaning = "Truy cập module legacy kho hàng, vật tư và nghiệp vụ nhập kho trong system mode.",
            ScopeLabel = "Hệ Gian hàng",
            ScopeMeaning = "Khối kho hàng/vật tư và các nghiệp vụ liên quan tồn kho của gian hàng.",
            DefaultUrl = "/gianhang/admin/quan-ly-kho-hang/Default.aspx?system=1",
            SortOrder = 59
        },
        new FeatureDefinition
        {
            Key = "shop_legacy_accounts",
            AccessGroupKey = "shop_legacy_accounts_ops",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Vận hành tài khoản gian hàng",
            Meaning = "Truy cập module legacy quản lý tài khoản/nhân sự và lịch sử chuyển điểm trong system mode.",
            ScopeLabel = "Hệ Gian hàng",
            ScopeMeaning = "Khối nhân sự, phân quyền và quản trị tài khoản nội bộ gian hàng.",
            DefaultUrl = "/gianhang/admin/quan-ly-tai-khoan/Default.aspx?system=1",
            SortOrder = 59
        },
        new FeatureDefinition
        {
            Key = "shop_legacy_org",
            AccessGroupKey = "shop_legacy_org_ops",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Vận hành cơ cấu hệ thống gian hàng",
            Meaning = "Truy cập module legacy quản lý chi nhánh, ngành, phòng ban trong system mode.",
            ScopeLabel = "Hệ Gian hàng",
            ScopeMeaning = "Khối cơ cấu tổ chức và tham số hệ thống của gian hàng.",
            DefaultUrl = "/gianhang/admin/quan-ly-he-thong/chi-nhanh.aspx?system=1",
            SortOrder = 59
        },
        new FeatureDefinition
        {
            Key = "shop_legacy_training",
            AccessGroupKey = "shop_legacy_training_ops",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Vận hành học viên giảng viên",
            Meaning = "Truy cập module legacy học viên/giảng viên trong system mode.",
            ScopeLabel = "Hệ Gian hàng",
            ScopeMeaning = "Khối học viên, giảng viên và theo dõi học tập trong gian hàng.",
            DefaultUrl = "/gianhang/admin/quan-ly-hoc-vien/Default.aspx?system=1",
            SortOrder = 59
        },
        new FeatureDefinition
        {
            Key = "shop_legacy_support",
            AccessGroupKey = "shop_legacy_support_ops",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Vận hành hỗ trợ gian hàng",
            Meaning = "Truy cập module legacy thông báo/tư vấn trong system mode.",
            ScopeLabel = "Hệ Gian hàng",
            ScopeMeaning = "Khối hỗ trợ khách hàng, thông báo và tiếp nhận yêu cầu tư vấn.",
            DefaultUrl = "/gianhang/admin/yeu-cau-tu-van/default.aspx?system=1",
            SortOrder = 59
        },
        new FeatureDefinition
        {
            Key = "shop_accounts",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Quản lý tài khoản gian hàng đối tác",
            Meaning = "Theo dõi hồ sơ shop, cấp vận hành và trạng thái gian hàng đối tác.",
            ScopeLabel = "Hệ Shop",
            ScopeMeaning = "Toàn bộ tài khoản shop và thông tin vận hành shop.",
            DefaultUrl = "/admin/quan-ly-tai-khoan/Default.aspx?scope=shop&fscope=shop",
            SortOrder = 60
        },
        new FeatureDefinition
        {
            Key = "shop_points",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Duyệt điểm / nghiệp vụ shop",
            Meaning = "Duyệt các yêu cầu điểm hoặc nghiệp vụ gắn với gian hàng đối tác.",
            ScopeLabel = "Hệ Shop",
            ScopeMeaning = "Chỉ các hồ sơ điểm và nghiệp vụ thuộc gian hàng đối tác.",
            DefaultUrl = "/admin/lich-su-chuyen-diem/default.aspx?tab=shop-only",
            SortOrder = 70
        },
        new FeatureDefinition
        {
            Key = "shop_level2",
            AccessGroupKey = "shop_level2",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Duyệt nâng cấp Level 2",
            Meaning = "Duyệt yêu cầu mở quyền dùng /gianhang/admin cho shop.",
            ScopeLabel = "Hệ Shop",
            ScopeMeaning = "Các shop yêu cầu nâng cấp lên bộ công cụ quản trị Level 2.",
            DefaultUrl = "/admin/duyet-nang-cap-level2.aspx",
            SortOrder = 80
        },
        new FeatureDefinition
        {
            Key = "home_gianhang_space",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Duyệt không gian gian hàng",
            Meaning = "Duyệt yêu cầu mở không gian /gianhang của tài khoản Home.",
            ScopeLabel = "Hệ Gian hàng",
            ScopeMeaning = "Các tài khoản Home gửi yêu cầu mở không gian /gianhang để tự sở hữu gian hàng của mình.",
            DefaultUrl = "/admin/duyet-gian-hang-doi-tac.aspx",
            SortOrder = 80
        },
        new FeatureDefinition
        {
            Key = "shop_partner",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Duyệt gian hàng đối tác (Shop)",
            Meaning = "Duyệt yêu cầu mở không gian /shop và áp dụng % chiết khấu mặc định cho shop.",
            ScopeLabel = "Hệ Shop",
            ScopeMeaning = "Các tài khoản Home gửi yêu cầu mở /shop kèm % chiết khấu để chạy chính sách chia % của nền tảng.",
            DefaultUrl = "/admin/duyet-shop-doi-tac.aspx",
            SortOrder = 85
        },
        new FeatureDefinition
        {
            Key = "shop_email",
            ManagementSpace = AdminManagementSpace_cl.SpaceGianHang,
            Title = "Nội dung email gian hàng đối tác",
            Meaning = "Sửa nội dung email hệ thống gửi tới shop trong luồng vận hành.",
            ScopeLabel = "Hệ Shop",
            ScopeMeaning = "Các mẫu email nội bộ liên quan đến shop và gian hàng.",
            DefaultUrl = "/admin/quan-ly-email-shop/default.aspx",
            SortOrder = 80
        },
        new FeatureDefinition
        {
            Key = "home_config",
            ManagementSpace = AdminManagementSpace_cl.SpaceContent,
            Title = "Cài đặt trang chủ",
            Meaning = "Cấu hình tổng cho Home như liên kết, bảo trì và khối hiển thị lõi.",
            ScopeLabel = "Nội dung website",
            ScopeMeaning = "Cấu hình hệ thống và các thiết lập lõi của khu vực Home.",
            DefaultUrl = "/admin/cai-dat-trang-chu/default.aspx",
            SortOrder = 88
        },
        new FeatureDefinition
        {
            Key = "home_content",
            ManagementSpace = AdminManagementSpace_cl.SpaceContent,
            Title = "Nội dung trang chủ Home",
            Meaning = "Sửa nội dung văn bản hiển thị trên Ahasale.vn.",
            ScopeLabel = "Nội dung văn bản",
            ScopeMeaning = "Các text, nhãn, CTA, mô tả và nội dung văn bản hiển thị trên Ahasale.vn; về sau có thể mở rộng cho /home, /shop, /gianhang/admin và /daugia.",
            DefaultUrl = "/admin/quan-ly-noi-dung-home/default.aspx",
            SortOrder = 90
        },
        new FeatureDefinition
        {
            Key = "home_bds_linked",
            ManagementSpace = AdminManagementSpace_cl.SpaceBatDongSan,
            Title = "BĐS - Liên kết tin",
            Meaning = "Quản lý feed tin bất động sản liên kết (RSS/đấu giá công khai) để bổ sung nội dung BĐS.",
            ScopeLabel = "Không gian BĐS",
            ScopeMeaning = "Quản trị nội dung BĐS liên kết hiển thị trong không gian Bất động sản.",
            DefaultUrl = "/admin/quan-ly-noi-dung-home/bds-lien-ket-tin.aspx",
            SortOrder = 92
        },
        new FeatureDefinition
        {
            Key = "daugia_workspace",
            ManagementSpace = AdminManagementSpace_cl.SpaceDauGia,
            Title = "Trung tâm quản trị đấu giá",
            Meaning = "Đi vào portal quản trị của không gian đấu giá.",
            ScopeLabel = "Hệ Đấu giá",
            ScopeMeaning = "Điểm vào chính của không gian quản trị đấu giá.",
            DefaultUrl = "/daugia/admin/default.aspx?system=1",
            SortOrder = 112
        },
        new FeatureDefinition
        {
            Key = "event_workspace",
            ManagementSpace = AdminManagementSpace_cl.SpaceEvent,
            Title = "Trung tâm quản trị sự kiện",
            Meaning = "Đi vào portal quản trị của không gian sự kiện.",
            ScopeLabel = "Hệ Sự kiện",
            ScopeMeaning = "Điểm vào chính của không gian quản trị sự kiện.",
            DefaultUrl = "/event/admin/default.aspx?system=1",
            SortOrder = 114
        },
        new FeatureDefinition
        {
            Key = "home_posts",
            ManagementSpace = AdminManagementSpace_cl.SpaceContent,
            Title = "Quản lý bài viết",
            Meaning = "Quản trị bài viết tổng quan cấp website.",
            ScopeLabel = "Tổng quan website",
            ScopeMeaning = "Kho bài viết, tin tức và nội dung công khai tổng quan của website.",
            DefaultUrl = "/admin/quan-ly-bai-viet/default.aspx",
            SortOrder = 130
        },
        new FeatureDefinition
        {
            Key = "home_menu",
            ManagementSpace = AdminManagementSpace_cl.SpaceContent,
            Title = "Quản lý menu",
            Meaning = "Cấu hình menu và điều hướng tổng quan của website.",
            ScopeLabel = "Tổng quan website",
            ScopeMeaning = "Các menu, danh mục và nhóm điều hướng tổng quan của website.",
            DefaultUrl = "/admin/quan-ly-menu/default.aspx",
            SortOrder = 140
        },
        new FeatureDefinition
        {
            Key = "home_banner",
            ManagementSpace = AdminManagementSpace_cl.SpaceContent,
            Title = "Quản lý banner",
            Meaning = "Quản lý banner và khối chiến dịch tổng quan của website.",
            ScopeLabel = "Tổng quan website",
            ScopeMeaning = "Các banner và vùng hiển thị chiến dịch tổng quan của website.",
            DefaultUrl = "/admin/quan-ly-banner/default.aspx",
            SortOrder = 150
        },
        new FeatureDefinition
        {
            Key = "notifications",
            ManagementSpace = AdminManagementSpace_cl.SpaceAdmin,
            Title = "Quản lý thông báo",
            Meaning = "Điều phối thông báo hệ thống và trạng thái gửi nhận.",
            ScopeLabel = "Cổng Admin",
            ScopeMeaning = "Thông báo hệ thống toàn nền tảng.",
            DefaultUrl = "/admin/quan-ly-thong-bao/default.aspx",
            SortOrder = 170
        },
        new FeatureDefinition
        {
            Key = "consulting",
            ManagementSpace = AdminManagementSpace_cl.SpaceAdmin,
            Title = "Yêu cầu tư vấn",
            Meaning = "Tổng hợp và xử lý luồng liên hệ, yêu cầu tư vấn từ người dùng.",
            ScopeLabel = "Cổng Admin",
            ScopeMeaning = "Các yêu cầu tư vấn phát sinh trên toàn nền tảng.",
            DefaultUrl = "/admin/yeu-cau-tu-van/default.aspx",
            SortOrder = 180
        },
        new FeatureDefinition
        {
            Key = "system_products",
            ManagementSpace = AdminManagementSpace_cl.SpaceHome,
            Title = "Bán sản phẩm",
            Meaning = "Thao tác bán các sản phẩm/thẻ của hệ thống từ cổng admin.",
            ScopeLabel = "Tài sản lõi",
            ScopeMeaning = "Các nghiệp vụ bán sản phẩm hệ thống có ảnh hưởng tới tài sản lõi.",
            DefaultUrl = "/admin/he-thong-san-pham/ban-the.aspx",
            SortOrder = 190
        },
        new FeatureDefinition
        {
            Key = "issue_cards",
            ManagementSpace = AdminManagementSpace_cl.SpaceHome,
            Title = "Phát hành thẻ",
            Meaning = "Phát hành thẻ theo đúng quy trình tài sản lõi của hệ thống.",
            ScopeLabel = "Tài sản lõi",
            ScopeMeaning = "Các giao dịch phát hành thẻ từ cổng admin.",
            DefaultUrl = "/admin/phat-hanh-the/them-moi.aspx",
            SortOrder = 200
        },
        new FeatureDefinition
        {
            Key = "core_assets",
            ManagementSpace = AdminManagementSpace_cl.SpaceHome,
            Title = "Lịch sử chuyển điểm",
            Meaning = "Rà soát chuyển điểm, tài sản lõi và các giao dịch nội bộ toàn hệ thống.",
            ScopeLabel = "Tài sản lõi",
            ScopeMeaning = "Tiền, quyền, điểm và toàn bộ giao dịch tài sản lõi.",
            DefaultUrl = "/admin/lich-su-chuyen-diem/default.aspx?tab=tieu-dung",
            SortOrder = 210
        },
        new FeatureDefinition
        {
            Key = "token_wallet",
            ManagementSpace = AdminManagementSpace_cl.SpaceAdmin,
            Title = "Ví token điểm",
            Meaning = "Theo dõi số dư blockchain, đối chiếu điểm A nội bộ và mở cấu hình bridge tài sản lõi.",
            ScopeLabel = "Tài sản lõi",
            ScopeMeaning = "Khối ví/token bridge chỉ dành cho Super Admin.",
            DefaultUrl = "/admin/vi-token-diem/default.aspx",
            SortOrder = 220
        }
    };

    public static IList<FeatureDefinition> GetAll()
    {
        return Definitions;
    }

    public static FeatureDefinition Get(string key)
    {
        string normalized = (key ?? "").Trim();
        if (normalized == "")
            return null;

        return Definitions.FirstOrDefault(item => string.Equals(item.Key, normalized, StringComparison.OrdinalIgnoreCase));
    }

    public static string ResolveManagementSpace(string key)
    {
        FeatureDefinition definition = Get(key);
        return definition == null ? "" : AdminManagementSpace_cl.NormalizeSpace(definition.ManagementSpace);
    }

    public static string ResolveAccessGroupKey(string key)
    {
        FeatureDefinition definition = Get(key);
        if (definition == null)
            return "";

        string groupKey = (definition.AccessGroupKey ?? "").Trim();
        if (groupKey == "")
            groupKey = definition.Key ?? "";

        return groupKey;
    }
}
