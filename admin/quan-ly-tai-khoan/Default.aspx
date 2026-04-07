<%@ Page Title="Quản lý tài khoản" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        /* Các route them-moi/bo-loc/chinh-sua/phan-quyen đều render theo full-page. */
        body.admin-shell #qltk-page-root.qltk-standalone-view {
            padding: 0 !important;
        }

        body.admin-shell #qltk-page-root.qltk-standalone-view .admin-route-panel-head,
        body.admin-shell #qltk-page-root.qltk-standalone-view .admin-account-route-head {
            position: static !important;
            inset: auto !important;
            top: auto !important;
            left: auto !important;
            width: 100% !important;
            height: auto !important;
            z-index: auto !important;
            padding: 0 !important;
        }

        body.admin-shell #qltk-page-root.qltk-standalone-view .admin-route-panel-body-wrap,
        body.admin-shell #qltk-page-root.qltk-standalone-view .admin-account-route-body-wrap {
            position: static !important;
            inset: auto !important;
            top: auto !important;
            left: auto !important;
            width: 100% !important;
            height: auto !important;
            min-height: 0 !important;
            z-index: auto !important;
            overflow: visible !important;
            background: transparent !important;
            background-image: none !important;
            backdrop-filter: none !important;
            padding: 0 !important;
            margin: 0 !important;
        }

        body.admin-shell #qltk-page-root.qltk-standalone-view .admin-route-panel-head-shell,
        body.admin-shell #qltk-page-root.qltk-standalone-view .admin-route-panel-dialog,
        body.admin-shell #qltk-page-root.qltk-standalone-view .admin-account-route-shell {
            margin: 0 !important;
            max-width: none !important;
            width: 100% !important;
        }

        body.admin-shell #qltk-page-root.qltk-standalone-view .admin-route-panel-head-shell,
        body.admin-shell #qltk-page-root.qltk-standalone-view .admin-account-route-head-card {
            border-radius: 16px 16px 0 0 !important;
            box-shadow: none !important;
        }

        body.admin-shell #qltk-page-root.qltk-standalone-view .admin-route-panel-head-shell > .bg-white,
        body.admin-shell #qltk-page-root.qltk-standalone-view .admin-account-route-head-card > .bg-white {
            border-radius: 16px 16px 0 0 !important;
        }

        body.admin-shell #qltk-page-root.qltk-standalone-view .admin-route-panel-body,
        body.admin-shell #qltk-page-root.qltk-standalone-view .admin-account-route-body {
            border-radius: 0 0 16px 16px !important;
            box-shadow: var(--aha-sync-shadow) !important;
            padding: 12px 16px 24px !important;
            margin: 0 !important;
        }

        body.admin-shell #qltk-page-root.qltk-standalone-view #menutop-tool-bc,
        body.admin-shell #qltk-page-root.qltk-standalone-view #timkiem-fixtop-bc {
            display: none !important;
        }

        /* Cố định render dạng bảng, chặn mọi CSS global ép theo hàng */
        body.admin-shell #qltk-page-root .bcorn-fix-title-table {
            display: table !important;
            width: max-content !important;
            min-width: 100% !important;
            border-collapse: separate !important;
            border-spacing: 0 !important;
        }

        body.admin-shell #qltk-page-root .bcorn-fix-title-table thead {
            display: table-header-group !important;
        }

        body.admin-shell #qltk-page-root .bcorn-fix-title-table tbody {
            display: table-row-group !important;
        }

        body.admin-shell #qltk-page-root .bcorn-fix-title-table tr {
            display: table-row !important;
        }

        body.admin-shell #qltk-page-root .bcorn-fix-title-table th,
        body.admin-shell #qltk-page-root .bcorn-fix-title-table td,
        body.admin-shell #qltk-page-root .bcorn-fix-title-table th[class*="col-"],
        body.admin-shell #qltk-page-root .bcorn-fix-title-table td[class*="col-"] {
            display: table-cell !important;
            float: none !important;
            width: auto !important;
            max-width: none !important;
            min-width: 0 !important;
            flex: none !important;
        }

        /* Scope HOME: chỉ giữ Tài khoản, Điện thoại, Hành vi, Thao tác */
        body.admin-shell #qltk-page-root.scope-home .col-id,
        body.admin-shell #qltk-page-root.scope-home .col-check,
        body.admin-shell #qltk-page-root.scope-home .col-avatar,
        body.admin-shell #qltk-page-root.scope-home .col-balance,
        body.admin-shell #qltk-page-root.scope-home .col-fullname,
        body.admin-shell #qltk-page-root.scope-home .col-percent,
        body.admin-shell #qltk-page-root.scope-home .col-birthday,
        body.admin-shell #qltk-page-root.scope-home .col-email,
        body.admin-shell #qltk-page-root.scope-home .qltkc-id,
        body.admin-shell #qltk-page-root.scope-home .qltkc-check,
        body.admin-shell #qltk-page-root.scope-home .qltkc-avatar,
        body.admin-shell #qltk-page-root.scope-home .qltkc-balance,
        body.admin-shell #qltk-page-root.scope-home .qltkc-fullname,
        body.admin-shell #qltk-page-root.scope-home .qltkc-percent,
        body.admin-shell #qltk-page-root.scope-home .qltkc-birthday,
        body.admin-shell #qltk-page-root.scope-home .qltkc-email {
            display: none !important;
        }

        /* Scope SHOP: chỉ giữ Tài khoản, Email, % DV Cho sàn, Thao tác */
        body.admin-shell #qltk-page-root.scope-shop .col-id,
        body.admin-shell #qltk-page-root.scope-shop .col-check,
        body.admin-shell #qltk-page-root.scope-shop .col-avatar,
        body.admin-shell #qltk-page-root.scope-shop .col-balance,
        body.admin-shell #qltk-page-root.scope-shop .col-fullname,
        body.admin-shell #qltk-page-root.scope-shop .col-hanhvi,
        body.admin-shell #qltk-page-root.scope-shop .col-birthday,
        body.admin-shell #qltk-page-root.scope-shop .col-phone,
        body.admin-shell #qltk-page-root.scope-shop .qltkc-id,
        body.admin-shell #qltk-page-root.scope-shop .qltkc-check,
        body.admin-shell #qltk-page-root.scope-shop .qltkc-avatar,
        body.admin-shell #qltk-page-root.scope-shop .qltkc-balance,
        body.admin-shell #qltk-page-root.scope-shop .qltkc-fullname,
        body.admin-shell #qltk-page-root.scope-shop .qltkc-hanhvi,
        body.admin-shell #qltk-page-root.scope-shop .qltkc-birthday,
        body.admin-shell #qltk-page-root.scope-shop .qltkc-phone {
            display: none !important;
        }

        /* Nút tạo tài khoản admin chỉ hiển thị ở scope admin */
        body.admin-shell #qltk-page-root.scope-home #ctl00_main_but_show_form_add,
        body.admin-shell #qltk-page-root.scope-shop #ctl00_main_but_show_form_add {
            display: none !important;
        }

        .admin-create-account-btn {
            display: inline-flex !important;
            align-items: center;
            gap: 6px;
            font-weight: 800 !important;
            white-space: nowrap;
            padding: 0 14px !important;
        }

        .admin-create-account-btn .mif-plus {
            font-size: 15px;
        }

        .admin-role-quickview {
            margin-bottom: 16px;
            padding: 14px;
            border-radius: 18px;
            border: 1px solid #dce7f2;
            background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
            box-shadow: 0 12px 28px rgba(15, 41, 64, 0.06);
        }

        .admin-role-quickview-title {
            margin: 0 0 4px;
            font-size: 18px;
            font-weight: 800;
            color: #10314a;
        }

        .admin-role-quickview-note {
            margin: 0 0 12px;
            color: #688094;
            font-size: 13px;
        }

        .admin-role-quick-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 12px;
        }

        .admin-role-quick-card {
            display: block;
            padding: 14px;
            border-radius: 16px;
            border: 1px solid #dce7f2;
            background: #fff;
            text-decoration: none !important;
            transition: transform .18s ease, box-shadow .18s ease, border-color .18s ease;
        }

        .admin-role-quick-card:hover {
            transform: translateY(-1px);
            box-shadow: 0 10px 20px rgba(15, 41, 64, 0.08);
            border-color: #bfd4e6;
        }

        .admin-role-quick-card.active {
            border-color: #d61f1f;
            background: linear-gradient(180deg, #fff4f4 0%, #fff 100%);
            box-shadow: 0 14px 26px rgba(214, 31, 31, 0.12);
        }

        .admin-role-quick-head {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
        }

        .admin-role-quick-title {
            color: #123148;
            font-size: 14px;
            font-weight: 800;
        }

        .admin-role-quick-count {
            min-width: 42px;
            height: 32px;
            padding: 0 10px;
            border-radius: 999px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            background: #eef5fb;
            color: #14354f;
            font-weight: 800;
            font-size: 13px;
        }

        .admin-role-quick-card.active .admin-role-quick-count {
            background: #d61f1f;
            color: #fff;
        }

        .admin-role-quick-desc {
            margin-top: 8px;
            color: #6b8092;
            font-size: 12px;
            line-height: 1.5;
            min-height: 54px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id="qltk-page-root" class="qltk-page-root <%= IsStandaloneViewByQuery() ? "qltk-standalone-view" : "" %> <%= GetAccountListScopeCssClass() %>">
    <!-- ======================= FULL-PAGE PHÂN QUYỀN ======================= -->
    <asp:UpdatePanel ID="up_phanquyen" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_phanquyen" runat="server" Visible="false" DefaultButton="but_phanquyen">
                <div class="admin-account-route-head admin-route-panel-head">
                    <div class="admin-account-route-shell admin-account-route-head-card admin-route-panel-shell">
                        <div class="admin-route-panel-actions">
                            <asp:HyperLink ID="A1" runat="server" CssClass="admin-route-back-link">Quay lại danh sách</asp:HyperLink>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                PHÂN QUYỀN TÀI KHOẢN
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>

                <div class="admin-account-route-body-wrap admin-route-panel-body-wrap">
                    <div class="admin-account-route-shell admin-route-panel-shell">
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 admin-account-route-body admin-route-panel-body">
                            <small>
                                <div class="row">
                                    <div class="cell-lg-12">
                                        <div class="admin-permission-scope-grid mt-4">
                                            <div class="admin-permission-scope-card super-admin">
                                                <span class="admin-permission-scope-tag">Super Admin</span>
                                                <h3>Tài sản lõi và cấu trúc hệ thống</h3>
                                                <p>Chỉ Super Admin mới được cấu hình ví token điểm, điều phối điểm lõi, cấp quyền admin và thay đổi cấu trúc phân quyền.</p>
                                            </div>
                                            <div class="admin-permission-scope-card">
                                                <span class="admin-permission-scope-tag">Tầng Home</span>
                                                <h3>Tài khoản tầng khách hàng</h3>
                                                <p>Duyệt yêu cầu điểm và xử lý hồ sơ thuộc tầng khách hàng. Chỉ xem điểm, không thao tác trực tiếp tài sản lõi.</p>
                                            </div>
                                            <div class="admin-permission-scope-card">
                                                <span class="admin-permission-scope-tag">Tầng Home</span>
                                                <h3>Tài khoản tầng cộng tác phát triển</h3>
                                                <p>Duyệt yêu cầu điểm của hồ sơ lao động và cộng tác phát triển theo đúng phạm vi được giao.</p>
                                            </div>
                                            <div class="admin-permission-scope-card">
                                                <span class="admin-permission-scope-tag">Tầng Home</span>
                                                <h3>Tài khoản tầng đồng hành hệ sinh thái</h3>
                                                <p>Duyệt yêu cầu điểm của hồ sơ gắn kết hệ sinh thái. Không được tự cộng trừ tài sản lõi.</p>
                                            </div>
                                            <div class="admin-permission-scope-card">
                                                <span class="admin-permission-scope-tag">Hệ Shop</span>
                                                <h3>Tài khoản gian hàng đối tác</h3>
                                                <p>Quản trị tài khoản shop, duyệt nghiệp vụ shop và các yêu cầu điểm phát sinh trong phạm vi gian hàng đối tác.</p>
                                            </div>
                                            <div class="admin-permission-scope-card">
                                                <span class="admin-permission-scope-tag">Ahasale.vn</span>
                                                <h3>Tài khoản quản lý nội dung web Ahasale.vn</h3>
                                                <p>Chỉ chỉnh sửa nội dung văn bản hiển thị trên web Ahasale.vn. Không quản lý menu, banner, bài viết tổng quan.</p>
                                            </div>
                                        </div>

                                        <div class="mt-3">
                                            <label class="fw-600">Mẫu vai trò admin</label>
                                            <div>
                                                <asp:DropDownList ID="ddl_admin_permission_preset" runat="server" data-role="select">
                                                    <asp:ListItem Value="" Text="Tự chọn thủ công"></asp:ListItem>
                                                    <asp:ListItem Value="home_customer" Text="Admin khách hàng"></asp:ListItem>
                                                    <asp:ListItem Value="home_development" Text="Admin cộng tác phát triển"></asp:ListItem>
                                                    <asp:ListItem Value="home_ecosystem" Text="Admin đồng hành hệ sinh thái"></asp:ListItem>
                                                    <asp:ListItem Value="shop_partner" Text="Admin gian hàng đối tác"></asp:ListItem>
                                                    <asp:ListItem Value="home_content" Text="Admin nội dung web"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <small class="fg-gray">Mẫu này chỉ gán quyền nghiệp vụ theo đúng tầng. Các thao tác tiền, quyền, điểm lõi vẫn chỉ Super Admin mới được thao tác trực tiếp.</small>
                                        </div>

                                        <div class="mt-3">
                                            <div class="mt-1">
                                                <asp:CheckBox ID="check_all_quyen_quanlynhanvien" runat="server" CssClass="text-bold" Text="SUPER ADMIN: CẤU TRÚC CỔNG ADMIN / PHÂN QUYỀN" />
                                            </div>
                                            <asp:CheckBoxList ID="check_list_quyen_quanlynhanvien" runat="server">
                                                    <asp:ListItem Text="Super Admin: tạo tài khoản admin và gán 5 vai trò vận hành" Value="5" Selected="false"></asp:ListItem>
                                                    <asp:ListItem Text="Super Admin: các quyền cấu trúc admin mở rộng (tạm thời)" Value="1" Selected="false"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>

                                        <div class="mt-3">
                                            <div class="mt-1">
                                                <asp:CheckBox ID="check_all_quyen_1" runat="server" CssClass="text-bold" Text="SUPER ADMIN: TÀI SẢN LÕI / ĐIỀU PHỐI ĐIỂM" />
                                            </div>
                                            <asp:CheckBoxList ID="check_list_quyen_1" runat="server">
                                                    <asp:ListItem Text="Super Admin: tài khoản tài sản lõi / ví token điểm / bridge đối soát" Value="q2_1" Selected="false"></asp:ListItem>
                                                    <asp:ListItem Text="Super Admin: xem lịch sử chuyển điểm toàn hệ thống" Value="q1_6" Selected="false"></asp:ListItem>
                                                    <asp:ListItem Text="Super Admin: xem lịch sử chuyển điểm theo phân quyền vận hành" Value="q1_7" Selected="false"></asp:ListItem>
                                                    <asp:ListItem Text="Admin: quản trị Event Platform /event/admin (tạo chiến dịch tích điểm voucher, lương/thưởng bậc thang)" Value="event_admin" Selected="false"></asp:ListItem>
                                                    <asp:ListItem Text="Event: chủ không gian (owner)" Value="event_owner" Selected="false"></asp:ListItem>
                                                    <asp:ListItem Text="Event: thiết kế campaign (designer)" Value="event_designer" Selected="false"></asp:ListItem>
                                                    <asp:ListItem Text="Event: phê duyệt/publish campaign" Value="event_approver" Selected="false"></asp:ListItem>
                                                    <asp:ListItem Text="Event: vận hành campaign (đổi trạng thái, target)" Value="event_operator" Selected="false"></asp:ListItem>
                                                    <asp:ListItem Text="Event: chỉ xem dashboard/campaign" Value="event_viewer" Selected="false"></asp:ListItem>
                                                    <asp:ListItem Text="Super Admin: chuyển điểm đến các tài khoản tổng" Value="q1_1" Selected="false"></asp:ListItem>
                                                    <asp:ListItem Text="Super Admin: điều phối điểm cho tài khoản tầng khách hàng" Value="q1_2" Selected="false"></asp:ListItem>
                                                    <asp:ListItem Text="Super Admin: điều phối điểm cho tài khoản gian hàng đối tác" Value="q1_3" Selected="false"></asp:ListItem>
                                                    <asp:ListItem Text="Super Admin: điều phối điểm cho tài khoản đồng hành hệ sinh thái" Value="q1_4" Selected="false"></asp:ListItem>
                                                    <asp:ListItem Text="Super Admin: điều phối điểm cho tài khoản cộng tác phát triển" Value="q1_5" Selected="false"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>

                                        <div class="mt-3">
                                            <div class="mt-1">
                                                <asp:CheckBox ID="check_all_quyen_home_customer" runat="server" CssClass="text-bold" Text="TÀI KHOẢN TẦNG KHÁCH HÀNG" />
                                            </div>
                                            <asp:CheckBoxList ID="check_list_quyen_home_customer" runat="server">
                                                <asp:ListItem Text="Xem hồ sơ khách hàng, duyệt yêu cầu điểm theo rule hệ thống và chỉ xử lý đúng phạm vi tầng khách hàng" Value="q2_2" Selected="false"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>

                                        <div class="mt-3">
                                            <div class="mt-1">
                                                <asp:CheckBox ID="check_all_quyen_home_development" runat="server" CssClass="text-bold" Text="TÀI KHOẢN TẦNG CỘNG TÁC PHÁT TRIỂN" />
                                            </div>
                                            <asp:CheckBoxList ID="check_list_quyen_home_development" runat="server">
                                                <asp:ListItem Text="Xem hồ sơ cộng tác phát triển, duyệt yêu cầu điểm đúng tầng và không tự ý can thiệp tài sản lõi" Value="q2_3" Selected="false"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>

                                        <div class="mt-3">
                                            <div class="mt-1">
                                                <asp:CheckBox ID="check_all_quyen_home_ecosystem" runat="server" CssClass="text-bold" Text="TÀI KHOẢN TẦNG ĐỒNG HÀNH HỆ SINH THÁI" />
                                            </div>
                                            <asp:CheckBoxList ID="check_list_quyen_home_ecosystem" runat="server">
                                                <asp:ListItem Text="Xem hồ sơ đồng hành hệ sinh thái, duyệt yêu cầu điểm đúng tầng và chỉ vận hành trong phạm vi hệ sinh thái" Value="q2_4" Selected="false"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>

                                        <div class="mt-3">
                                            <div class="mt-1">
                                                <asp:CheckBox ID="check_all_quyen_shop_partner" runat="server" CssClass="text-bold" Text="TÀI KHOẢN GIAN HÀNG ĐỐI TÁC" />
                                            </div>
                                            <asp:CheckBoxList ID="check_list_quyen_shop_partner" runat="server">
                                                <asp:ListItem Text="Quản trị tài khoản shop, duyệt nghiệp vụ shop và các yêu cầu điểm phát sinh trong phạm vi gian hàng đối tác" Value="q2_5" Selected="false"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>

                                        <div class="mt-3">
                                            <div class="mt-1">
                                                <asp:CheckBox ID="check_all_quyen_home_content" runat="server" CssClass="text-bold" Text="TÀI KHOẢN QUẢN LÝ NỘI DUNG WEB AHASALE.VN" />
                                            </div>
                                            <asp:CheckBoxList ID="check_list_quyen_home_content" runat="server">
                                                <asp:ListItem Text="Chỉnh sửa nội dung văn bản hiển thị trên web Ahasale.vn; không quản lý menu, banner, bài viết tổng quan và không can thiệp dữ liệu tài sản lõi" Value="q3_1" Selected="false"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>
                                    </div>
                                </div>
                            </small>

                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_phanquyen" runat="server" CssClass="success" Text="Phân quyền" OnClick="but_phanquyen_Click" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress4" runat="server" AssociatedUpdatePanelID="up_phanquyen">
        <ProgressTemplate>
            <div class="admin-inline-progress" role="status" aria-live="polite"><span class="admin-inline-progress-spinner"></span><span class="admin-inline-progress-text">Đang xử lý...</span></div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <!-- ======================= FULL-PAGE ADD/EDIT ======================= -->
    <asp:UpdatePanel ID="up_add" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_add" runat="server" Visible="false" DefaultButton="but_add_edit">
                <div class="admin-account-route-head admin-route-panel-head">
                    <div class="admin-account-route-shell admin-account-route-head-card admin-route-panel-shell">
                        <div class="admin-route-panel-actions">
                            <asp:HyperLink ID="close_add" runat="server" CssClass="admin-route-back-link">Quay lại danh sách</asp:HyperLink>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>

                <div class="admin-account-route-body-wrap admin-route-panel-body-wrap">
                    <div class="admin-account-route-shell admin-route-panel-shell">
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 admin-account-route-body admin-route-panel-body">
                            <div class="row">
                                <div class="cell-lg-12">

                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Tài khoản</label>
                                        <div>
                                            <asp:TextBox ID="txt_taikhoan" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>

                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                                        <div class="mt-3">
                                            <label class="fw-600 fg-red">Mật khẩu</label>
                                            <div class="aha-password-field">
                                                <asp:TextBox ID="txt_matkhau" TextMode="Password" runat="server" data-role="input"></asp:TextBox>
                                                <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện mật khẩu">
                                                    <span class="aha-password-toggle-label">Hiện</span>
                                                </button>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>

                                    <asp:Panel ID="pn_loai_taikhoan" runat="server" CssClass="mt-3">
                                        <label class="fw-600 fg-red">Loại tài khoản</label>
                                        <div>
                                            <asp:DropDownList ID="DropDownList1" runat="server" data-role="select">
                                                <asp:ListItem Value="Nhân viên admin" Text="Nhân viên admin"></asp:ListItem>
                                                <asp:ListItem Value="Tài khoản tổng" Text="Tài khoản tổng"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </asp:Panel>

                                    <asp:Panel ID="pn_admin_create_preset" runat="server" CssClass="mt-3" Visible="false">
                                        <label class="fw-600">Vai trò admin khởi tạo</label>
                                        <div>
                                            <asp:DropDownList ID="ddl_admin_create_preset" runat="server" data-role="select">
                                                <asp:ListItem Value="" Text="Tự gán sau khi tạo"></asp:ListItem>
                                                <asp:ListItem Value="home_customer" Text="Admin khách hàng"></asp:ListItem>
                                                <asp:ListItem Value="home_development" Text="Admin cộng tác phát triển"></asp:ListItem>
                                                <asp:ListItem Value="home_ecosystem" Text="Admin đồng hành hệ sinh thái"></asp:ListItem>
                                                <asp:ListItem Value="shop_partner" Text="Admin gian hàng đối tác"></asp:ListItem>
                                                <asp:ListItem Value="home_content" Text="Admin nội dung web"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <small class="fg-gray">Nếu chọn mẫu, tài khoản admin sẽ được gán đúng 1 vai trò chính ngay khi tạo. Nếu để trống, Super Admin sẽ phân quyền thủ công ở bước sau.</small>
                                    </asp:Panel>

<asp:Panel ID="pn_tang_home" runat="server" CssClass="mt-3" Visible="false">
    <label class="fw-600">Tầng Home (chỉnh bởi admin có quyền)</label>
    <div>
        <asp:DropDownList ID="ddl_cap_sp" runat="server" data-role="select">
            <asp:ListItem Value="1" Text="Khách hàng"></asp:ListItem>
            <asp:ListItem Value="2" Text="Cộng tác phát triển"></asp:ListItem>
            <asp:ListItem Value="3" Text="Đồng hành hệ sinh thái"></asp:ListItem>
        </asp:DropDownList>
    </div>
</asp:Panel>

<asp:Panel ID="pn_home_event_builder" runat="server" CssClass="mt-3 p-2 border bd-default rounded" Visible="false">
    <div class="fw-600">Phân quyền theo không gian</div>
    <div class="fg-gray mt-1">
        <small>Bật hoặc tắt từng không gian mở rộng để tài khoản Home được phép truy cập đúng khu quản trị tương ứng.</small>
    </div>
    <div class="mt-2">
        <div class="mb-2">
            <div class="small fg-gray">
                Không gian Gian hàng (<code>/gianhang</code>) được mở theo luồng:
                tài khoản Home đăng ký mở không gian -> admin duyệt tại tab <strong>Duyệt không gian gian hàng</strong>.
                Quyền này không cấp trực tiếp ở màn hình chi tiết tài khoản.
            </div>
        </div>
        <div class="mb-2">
            <asp:CheckBox ID="cb_home_event_builder" runat="server" Text="Cho phép truy cập không gian Event (/event/admin)" />
        </div>
        <div class="mb-2">
            <asp:CheckBox ID="cb_home_daugia_admin" runat="server" Text="Cho phép truy cập không gian Đấu giá (/daugia/admin)" />
        </div>
        <div class="mb-2">
            <asp:CheckBox ID="cb_home_batdongsan_admin" runat="server" Text="Cho phép truy cập không gian Bất động sản (/admin/default.aspx?mspace=batdongsan)" />
        </div>
        <div>
            <asp:CheckBox ID="cb_home_gianhang_admin" runat="server" Text="Cho phép truy cập không gian quản trị Gian hàng (/gianhang/admin)" />
        </div>
    </div>
</asp:Panel>


                                    <asp:DropDownList ID="ddl_nguoi_gioi_thieu" runat="server" Visible="false"></asp:DropDownList>

                      




                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Email</label>
                                        <div>
                                            <asp:TextBox ID="txt_email" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600">Ảnh đại diện</label>
                                        <input type="file" id="fileInput" onchange="uploadFile()" data-role="file" data-button-title="<span class='mif-file-upload'></span>" />
                                        <div id="message" runat="server"></div>
                                        <div id="uploadedFilePath"></div>
                                        <div style="display: none">
                                            <asp:TextBox ID="txt_link_fileupload" runat="server"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div style='position: absolute; bottom: 0px; left: 100px'>
                                            <asp:Button ID="Button2" runat="server" Text="Xóa ảnh cũ" CssClass="alert small" Visible="false" OnClick="Button2_Click" />
                                        </div>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600">Họ tên</label>
                                        <div>
                                            <asp:TextBox ID="txt_hoten" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Ngày sinh</label>
                                        <div>
                                            <asp:TextBox ID="txt_ngaysinh" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Điện thoại</label>
                                        <div>
                                            <asp:TextBox ID="txt_dienthoai" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>

                                    <asp:Panel ID="pn_reset_security_actions" runat="server" CssClass="mt-4 p-3 border bd-default rounded" Visible="false">
                                        <div class="text-bold">Reset bảo mật tạm thời</div>
                                        <div class="fg-gray mt-1">
                                            <small>
                                                Admin nhập giá trị tạm thời để cấp lại cho người dùng.
                                                Sau khi đăng nhập, tài khoản sẽ bị bắt buộc đổi lại thông tin bảo mật tương ứng.
                                            </small>
                                        </div>
                                        <div class="mt-1">
                                            <small class="fg-blue">
                                                <asp:Label ID="lb_reset_scope_note" runat="server" Text=""></asp:Label>
                                            </small>
                                        </div>

                                        <asp:Panel ID="pn_reset_home_credentials" runat="server" CssClass="mt-3" Visible="false">
                                            <div class="mt-2">
                                                <label class="fw-600">Mật khẩu tạm thời (Home)</label>
                                                <div class="d-flex flex-wrap align-items-center">
                                                    <div class="aha-password-field">
                                                        <asp:TextBox ID="txt_reset_home_password" runat="server" data-role="input" TextMode="Password" placeholder="Nhập mật khẩu tạm thời mới"></asp:TextBox>
                                                        <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện mật khẩu tạm thời Home">
                                                            <span class="aha-password-toggle-label">Hiện</span>
                                                        </button>
                                                    </div>
                                                    <asp:Button ID="but_reset_home_password" runat="server" Text="Reset mật khẩu Home" CssClass="button warning ml-2 mt-2 mt-0-sm" CausesValidation="false" OnClick="but_reset_home_password_Click" />
                                                </div>
                                            </div>
                                            <div class="mt-3">
                                                <label class="fw-600">Mã PIN tạm thời (Home)</label>
                                                <div class="d-flex flex-wrap align-items-center">
                                                    <div class="aha-password-field">
                                                        <asp:TextBox ID="txt_reset_home_pin" runat="server" MaxLength="4" TextMode="Password" data-role="input" placeholder="4 chữ số"></asp:TextBox>
                                                        <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện mã PIN tạm thời Home">
                                                            <span class="aha-password-toggle-label">Hiện</span>
                                                        </button>
                                                    </div>
                                                    <asp:Button ID="but_reset_home_pin" runat="server" Text="Reset PIN Home" CssClass="button warning ml-2 mt-2 mt-0-sm" CausesValidation="false" OnClick="but_reset_home_pin_Click" />
                                                </div>
                                            </div>
                                        </asp:Panel>

                                        <asp:Panel ID="pn_reset_shop_credentials" runat="server" CssClass="mt-3" Visible="false">
                                            <div class="mt-2">
                                                <label class="fw-600">Mật khẩu tạm thời (Shop)</label>
                                                <div class="d-flex flex-wrap align-items-center">
                                                    <div class="aha-password-field">
                                                        <asp:TextBox ID="txt_reset_shop_password" runat="server" data-role="input" TextMode="Password" placeholder="Nhập mật khẩu tạm thời mới"></asp:TextBox>
                                                        <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện mật khẩu tạm thời Shop">
                                                            <span class="aha-password-toggle-label">Hiện</span>
                                                        </button>
                                                    </div>
                                                    <asp:Button ID="but_reset_shop_password" runat="server" Text="Reset mật khẩu Shop" CssClass="button warning ml-2 mt-2 mt-0-sm" CausesValidation="false" OnClick="but_reset_shop_password_Click" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </asp:Panel>

                                </div>
                            </div>

                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_add_edit" runat="server" Text="" CssClass="button success" OnClick="but_add_edit_Click" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="up_add">
        <ProgressTemplate>
            <div class="admin-inline-progress" role="status" aria-live="polite"><span class="admin-inline-progress-spinner"></span><span class="admin-inline-progress-text">Đang xử lý...</span></div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <!-- ======================= FULL-PAGE LỌC ======================= -->
    <asp:UpdatePanel ID="up_filter" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_filter" runat="server" Visible="false" DefaultButton="but_apdung_loc">
                <div class="admin-account-route-head admin-route-panel-head">
                    <div class="admin-account-route-shell admin-account-route-head-card admin-route-panel-shell">
                        <div class="admin-route-panel-actions">
                            <asp:HyperLink ID="close_filter" runat="server" CssClass="admin-route-back-link">Quay lại danh sách</asp:HyperLink>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                LỌC DANH SÁCH
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>

                <div class="admin-account-route-body-wrap admin-route-panel-body-wrap">
                    <div class="admin-account-route-shell admin-route-panel-shell">
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 admin-account-route-body admin-route-panel-body">
                            <div class="row">
                                <div class="cell-lg-12">

                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Phân loại tài khoản</label>
                                        <div>
                                            <asp:DropDownList ID="ddl_loc_phanloai" runat="server" data-role="select">
                                                <asp:ListItem Value="" Text="-- Tất cả --"></asp:ListItem>
                                                <asp:ListItem Value="Nhân viên admin" Text="Nhân viên admin"></asp:ListItem>
                                                <asp:ListItem Value="Cộng tác phát triển" Text="Cộng tác phát triển"></asp:ListItem>
                                                <asp:ListItem Value="Đồng hành hệ sinh thái" Text="Đồng hành hệ sinh thái"></asp:ListItem>
                                                <asp:ListItem Value="Gian hàng đối tác" Text="Gian hàng đối tác"></asp:ListItem>
                                                <asp:ListItem Value="Khách hàng" Text="Khách hàng"></asp:ListItem>
                                                <asp:ListItem Value="Tài khoản tổng" Text="Tài khoản tổng"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Phạm vi hệ</label>
                                        <div>
                                            <asp:DropDownList ID="ddl_loc_scope" runat="server" data-role="select">
                                                <asp:ListItem Value="" Text="-- Tất cả hệ --"></asp:ListItem>
                                                <asp:ListItem Value="admin" Text="Hệ admin"></asp:ListItem>
                                                <asp:ListItem Value="home" Text="Hệ home"></asp:ListItem>
                                                <asp:ListItem Value="shop" Text="Hệ shop"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <asp:Panel ID="pn_filter_admin_role" runat="server" CssClass="mt-3">
                                        <label class="fw-600 fg-red">Nhóm admin nghiệp vụ</label>
                                        <div>
                                            <asp:DropDownList ID="ddl_loc_admin_role" runat="server" data-role="select">
                                                <asp:ListItem Value="" Text="-- Tất cả nhóm admin --"></asp:ListItem>
                                                <asp:ListItem Value="super_admin" Text="Super Admin"></asp:ListItem>
                                                <asp:ListItem Value="home_customer" Text="Admin khách hàng"></asp:ListItem>
                                                <asp:ListItem Value="home_development" Text="Admin cộng tác phát triển"></asp:ListItem>
                                                <asp:ListItem Value="home_ecosystem" Text="Admin đồng hành hệ sinh thái"></asp:ListItem>
                                                <asp:ListItem Value="shop_partner" Text="Admin gian hàng đối tác"></asp:ListItem>
                                                <asp:ListItem Value="home_content" Text="Admin nội dung web"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <small class="fg-gray">Bộ lọc này giúp Super Admin tách riêng 5 nhóm admin chính như sơ đồ vận hành đã chốt.</small>
                                    </asp:Panel>

                                </div>
                            </div>

                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_apdung_loc" runat="server" Text="Áp dụng lọc" CssClass="button success" OnClick="but_apdung_loc_Click" />
                                <asp:Button ID="but_xoa_loc" runat="server" Text="Xóa lọc" CssClass="button alert" OnClick="but_xoa_loc_Click" />
                            </div>

                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress6" runat="server" AssociatedUpdatePanelID="up_filter">
        <ProgressTemplate>
            <div class="admin-inline-progress" role="status" aria-live="polite"><span class="admin-inline-progress-spinner"></span><span class="admin-inline-progress-text">Đang xử lý...</span></div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <!-- ======================= MAIN ======================= -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="pos-relative pb-11">
                <asp:HiddenField ID="hf_selected_taikhoan" runat="server" ClientIDMode="Static" />
                <div id="menutop-tool-bc" class="aha-admin-toolbar">
                    <ul class="h-menu bg-white">

                        <!-- nút tạo tài khoản admin -->
                        <li data-role="hint" data-hint-position="top" data-hint-text="Tạo tài khoản admin">
                            <asp:HyperLink ID="but_show_form_add" runat="server" CssClass="admin-create-account-btn">
                                <span class="mif-plus"></span>
                                <span>Tạo tài khoản admin</span>
                            </asp:HyperLink>
                        </li>

                        <!-- ✅ NEW: icon lọc -->
                        <li data-role="hint" data-hint-position="top" data-hint-text="Lọc">
                            <asp:HyperLink ID="but_show_filter" runat="server" NavigateUrl="<%# BuildFilterUrl() %>"><span class="mif-filter"></span></asp:HyperLink>
                        </li>

                        <li data-role="hint" data-hint-position="top" data-hint-text="Tìm kiếm">
                            <asp:LinkButton ID="but_timkiem" ClientIDMode="Static" OnClick="but_timkiem_Click" runat="server"><span class="mif-search"></span></asp:LinkButton>
                        </li>

                        <li data-role="hint" data-hint-position="top" data-hint-text="Làm mới">
                            <asp:LinkButton ID="but_xoa_timkiem" OnClick="but_xoa_timkiem_Click" runat="server"><span class="mif-loop2"></span></asp:LinkButton>
                        </li>

                        <li data-role="hint" data-hint-position="top" data-hint-text="Khóa hàng loạt Home không đủ số điện thoại">
                            <asp:LinkButton ID="but_lock_legacy_home" OnClick="but_lock_legacy_home_Click" runat="server"><span class="mif-lock"></span></asp:LinkButton>
                        </li>

                        <li class="bd-gray border bd-default mt-2 d-block-lg d-none" style="height: 24px"></li>

                        <li class="d-block-lg d-none">
                            <a data-role="hint" data-hint-position="top" data-hint-text="Hiển thị">
                                <small>
                                    <asp:Label ID="lb_show" runat="server" Text=""></asp:Label></small>
                            </a>
                        </li>

                        <li class="d-block-lg d-none" data-role="hint" data-hint-position="top" data-hint-text="Lùi">
                            <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server"><span class="mif-chevron-left"></span></asp:LinkButton>
                        </li>
                        <li class="d-block-lg d-none" data-role="hint" data-hint-position="top" data-hint-text="Tới">
                            <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server"><span class="mif-chevron-right"></span></asp:LinkButton>
                        </li>
                    </ul>
                </div>

                <div id="timkiem-fixtop-bc" class="aha-admin-toolbar-search d-none d-block-sm">
                    <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem" runat="server" placeholder="Tìm bất kỳ dữ liệu" data-role="input" CssClass="input-small" AutoPostBack="false" data-sync-key="qltk-search" data-enter-click="but_timkiem"></asp:TextBox>
                </div>
            </div>

            <div class="p-3">
                <asp:PlaceHolder ID="ph_admin_role_quickview" runat="server" Visible="false">
                    <div class="admin-role-quickview">
                        <div class="admin-role-quickview-title">5 nhóm admin nghiệp vụ</div>
                        <div class="admin-role-quickview-note">Từ đây Super Admin có thể nhìn nhanh số lượng tài khoản ở từng vai trò và bấm vào từng nhóm để lọc ngay.</div>
                        <asp:Literal ID="lit_admin_role_quickview" runat="server"></asp:Literal>
                    </div>
                </asp:PlaceHolder>

                <div class="d-none-sm d-block">
                    <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem1" runat="server" placeholder="Tìm bất kỳ dữ liệu" data-role="input" AutoPostBack="false" data-sync-key="qltk-search" data-enter-click="but_timkiem"></asp:TextBox>
                </div>

                <div class="d-none-lg d-block mb-3 mt-0-lg mt-3">
                    <div class="place-right text-right">
                        <small class="pr-1">
                            <asp:Label ID="lb_show_md" runat="server" Text=""></asp:Label></small>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Lùi" ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" CssClass="button small light"><span class="mif-chevron-left"></span></asp:LinkButton>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Tới" ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" CssClass="button small light"><span class="mif-chevron-right"></span></asp:LinkButton>
                    </div>
                    <div class="clr-bc"></div>
                </div>

                <div class="row">
                    <div class="cell-lg-12">
                        <div class="bcorn-fix-title-table-container aha-admin-grid">
                            <table class="bcorn-fix-title-table">
                                <thead>
                                    <tr class="">
                                        <th class="qltkc-id" style="width: 1px;">ID</th>
                                        <th class="qltkc-check" style="width: 1px;">
                                            <input data-role="hint" data-hint-position="top" data-hint-text="Chọn/Bỏ chọn" type="checkbox" onkeypress="if (event.keyCode==13) return false;" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </th>

                                        <th class="text-center qltkc-avatar" style="width: 60px; min-width: 60px;">Ảnh</th>
                                        <th class="text-center qltkc-account" style="min-width: 1px;">Tài khoản</th>
                                        <th class="text-center qltkc-balance" style="min-width: 50px;">Quyền tiêu dùng</th>

                                        <!-- ✅ NEW -->
                                       <%-- <th class="text-center" style="min-width: 90px;">Ví 1 (20%)</th>
                                        <th class="text-center" style="min-width: 90px;">Ví 2 (30%) EVoucher</th>
                                        <th class="text-center" style="min-width: 90px;">Ví 3 (50%)</th>--%>

                                        <th class="text-center qltkc-fullname" style="min-width: 140px;">Họ tên</th>
                                        <th class="text-center qltkc-hanhvi" style="min-width: 220px;">Hành vi</th>
                                        <th class="text-center qltkc-percent" style="width: 90px; min-width: 90px;">% DV Cho sàn</th>

                                        <th class="text-center qltkc-birthday" style="width: 60px; min-width: 60px;">Ngày sinh</th>
                                        <th class="text-center qltkc-phone" style="width: 60px; min-width: 60px;">Điện thoại</th>
                                        <th class="text-center qltkc-email" style="width: 60px; min-width: 60px;">Email</th>
                                        <th class="text-center qltkc-actions" style="min-width: 120px;">Thao tác</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server" EnableViewState="false" ViewStateMode="Disabled">
                                        <ItemTemplate>
                                            <span style="display: none">
                                                <asp:Label ID="lbID" runat="server" Text='<%#Eval("taikhoan") %>'></asp:Label>
                                            </span>
                                            <tr>
                                                <td class="text-center qltkc-id">
                                                    <%# Eval("id") %>
                                                </td>
                                                <td class="checkbox-table text-center qltkc-check">
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>
                                                <td class="text-center qltkc-avatar">
                                                    <img src='<%#Eval("anhdaidien") %>' class="img-cover-vuongtron" width="60" height="60" />
                                                </td>
                                                <td class="text-left qltkc-account" style="vertical-align: middle">
                                                    <div>
                                                        <%# Eval("taikhoan") %>
                                                    </div>

                                                    <asp:PlaceHolder ID="PlaceHolder19" runat="server" Visible='<%#Eval("phanloai").ToString()=="Cộng tác phát triển" %>'>
                                                        <div class="button mini dark rounded">Cộng tác phát triển</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder19b" runat="server" Visible='<%#Eval("phanloai").ToString()=="Nhân viên admin" %>'>
                                                        <div class="button mini dark rounded">Nhân viên admin</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("phanloai").ToString()=="Đồng hành hệ sinh thái" %>'>
                                                        <div class="button mini alert rounded">Đồng hành hệ sinh thái</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("phanloai").ToString()=="Gian hàng đối tác" %>'>
                                                        <div class="button mini yellow rounded">Gian hàng đối tác</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("phanloai").ToString()=="Khách hàng" %>'>
                                                        <div class="button mini success rounded">Khách hàng</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%# AccountType_cl.IsTreasury(Eval("phanloai").ToString()) %>'>
                                                        <div class="button mini bg-violet fg-white rounded">Tài khoản tổng</div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolderLockHome" runat="server" Visible='<%# Convert.ToBoolean(Eval("IsHomeScope")) %>'>
                                                        <div class='<%# Convert.ToBoolean(Eval("IsBlocked")) ? "button mini alert rounded" : "button mini success rounded" %>'>
                                                            <%# Convert.ToBoolean(Eval("IsBlocked")) ? "Đã khóa Home" : "Home hoạt động" %>
                                                        </div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolderAdminRoleSummary" runat="server" Visible='<%# Convert.ToBoolean(Eval("IsAdminScope")) && !string.IsNullOrEmpty(Eval("AdminVaiTroHienThi") as string) %>'>
                                                        <div class="mt-1 fg-gray"><small>Vai trò: <%# Eval("AdminVaiTroHienThi") %></small></div>
                                                    </asp:PlaceHolder>
                                                </td>
                                                <td class="text-left qltkc-balance">
                                                    <img src="/uploads/images/dong-a.png" width="20" />
                                                    <div class="button mini light rounded"><%# Eval("DongA","{0:#,##0.##}") %></div>
                                                </td>

                                                <!-- ✅ NEW: 3 ví (đã được code-behind đổi null => 0) -->
                                               <%-- <td class="text-left">
                                                    <img src="/uploads/images/dong-a.png" width="20" />
                                                    <div class="button mini light rounded"><%# Eval("Vi3_20PhanTram_ViGanKet","{0:#,##0.##}") %></div>
                                                </td>
                                                <td class="text-left">
                                                    <img src="/uploads/images/dong-a.png" width="20" />
                                                    <div class="button mini light rounded"><%# Eval("Vi1_30PhanTram_ViEVoucher","{0:#,##0.##}") %></div>
                                                </td>
                                                <td class="text-left">
                                                    <img src="/uploads/images/dong-a.png" width="20" />
                                                    <div class="button mini light rounded"><%# Eval("Vi2_50PhanTram_ViLaoDong","{0:#,##0.##}") %></div>
                                                </td>--%>

                                                <td class="text-left qltkc-fullname">
                                                    <div class="fw-600"><%#Eval("hoten") %></div>
                                                </td>
                                                <td class="text-center qltkc-hanhvi">
                                                    <div class="button mini light rounded">
                                                        <%# Eval("HanhVi_HienThi") %>
                                                    </div>
                                                </td>
                                                <td class="text-center qltkc-percent">
                                                    <div class="button mini light rounded">
                                                        <%# Eval("ChiPhanTram_BanDichVu_ChoSan") %>%
                                                    </div>
                                                </td>

                                                <td class="qltkc-birthday">
                                                    <%#Eval("ngaysinh","{0:dd/MM/yyyy}") %>
                                                </td>
                                                <td class="qltkc-phone">
                                                    <div><a title="Gọi" href="tel:<%#Eval("dienthoai") %>"><%#Eval("dienthoai") %></a></div>
                                                </td>
                                                <td class="text-left qltkc-email"><%#Eval("email") %></td>

                                                <td style="vertical-align: middle" class="text-center qltkc-actions">
                                                    <div class="d-flex flex-wrap justify-content-center align-items-center">
                                                        <asp:HyperLink ID="hl_chi_tiet" runat="server" CssClass="button small primary rounded mr-1" NavigateUrl='<%# BuildEditUrl(Eval("taikhoan")) %>' Text="Chi tiết"></asp:HyperLink>
                                                        <asp:HyperLink ID="hl_show_form_phanquyen" runat="server" CssClass="button small warning rounded" NavigateUrl='<%# BuildPermissionUrl(Eval("taikhoan")) %>' Visible='<%# Convert.ToBoolean(Eval("CanShowPhanQuyen")) %>'>Phân quyền</asp:HyperLink>
                                                        <asp:HyperLink ID="hl_toggle_home_lock" runat="server"
                                                            CssClass='<%# Convert.ToBoolean(Eval("IsBlocked")) ? "button small success rounded ml-1" : "button small alert rounded ml-1" %>'
                                                            Visible='<%# Convert.ToBoolean(Eval("CanToggleHomeLock")) %>'
                                                            NavigateUrl='<%# BuildToggleHomeLockUrl(Eval("taikhoan")) %>'>
                                                            <%# Convert.ToBoolean(Eval("IsBlocked")) ? "Mở khóa Home" : "Khóa Home" %>
                                                        </asp:HyperLink>
                                                    </div>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>

            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="admin-inline-progress" role="status" aria-live="polite"><span class="admin-inline-progress-spinner"></span><span class="admin-inline-progress-text">Đang xử lý...</span></div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <script>
        function uploadFile() {
            var fileInput = document.getElementById("fileInput");
            var messageDiv = document.getElementById("message");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath");

            if (fileInput.files.length > 0) {
                var file = fileInput.files[0];

                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) {
                    messageDiv.innerHTML = "Định dạng ảnh không hợp lệ.";
                    return;
                }

                var maxFileSize = 10 * 1024 * 1024;
                if (file.size > maxFileSize) {
                    messageDiv.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB.";
                    return;
                }

                var formData = new FormData();
                formData.append("file", file);

                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        uploadedFilePathDiv.innerHTML = "<div><small>Ảnh mới chọn<small></div><img width='100' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= txt_link_fileupload.ClientID %>').value = xhr.responseText;
                    } else {
                        messageDiv.innerHTML = "Lỗi upload.";
                    }
                };
                xhr.send(formData);
            } else {
                messageDiv.innerHTML = "Vui lòng chọn file.";
            }
        }
    </script>

    <script type="text/javascript">
        (function () {
            function toArray(nodes) {
                return Array.prototype.slice.call(nodes || []);
            }

            function getInputs(containerId) {
                var host = document.getElementById(containerId);
                if (!host) {
                    return [];
                }
                return toArray(host.querySelectorAll('input[type="checkbox"]'));
            }

            function syncMaster(masterId, listId) {
                var master = document.getElementById(masterId);
                var inputs = getInputs(listId);
                if (!master) {
                    return;
                }
                if (!inputs.length) {
                    master.checked = false;
                    master.indeterminate = false;
                    return;
                }
                var checkedCount = inputs.filter(function (item) { return item.checked; }).length;
                master.checked = checkedCount === inputs.length;
                master.indeterminate = checkedCount > 0 && checkedCount < inputs.length;
            }

            function bindGroup(masterId, listId) {
                var master = document.getElementById(masterId);
                var inputs = getInputs(listId);
                if (!master || master.getAttribute('data-admin-bound') === '1') {
                    syncMaster(masterId, listId);
                    return;
                }

                master.addEventListener('change', function () {
                    var items = getInputs(listId);
                    items.forEach(function (item) {
                        item.checked = master.checked;
                    });
                    syncMaster(masterId, listId);
                });

                inputs.forEach(function (item) {
                    item.addEventListener('change', function () {
                        syncMaster(masterId, listId);
                    });
                });

                master.setAttribute('data-admin-bound', '1');
                syncMaster(masterId, listId);
            }

            function bindPreset(selectId, pairs) {
                var select = document.getElementById(selectId);
                if (!select || select.getAttribute('data-admin-preset-bound') === '1') {
                    return;
                }

                select.addEventListener('change', function () {
                    var permissionMap = {
                        home_customer: ['q2_2'],
                        home_development: ['q2_3'],
                        home_ecosystem: ['q2_4'],
                        shop_partner: ['q2_5'],
                        home_content: ['q3_1']
                    };
                    var selected = permissionMap[select.value] || [];
                    pairs.forEach(function (pair) {
                        getInputs(pair.listId).forEach(function (item) {
                            item.checked = false;
                        });
                    });
                    selected.forEach(function (code) {
                        pairs.forEach(function (pair) {
                            getInputs(pair.listId).forEach(function (item) {
                                if ((item.value || '').trim() === code) {
                                    item.checked = true;
                                }
                            });
                        });
                    });
                    pairs.forEach(function (pair) {
                        syncMaster(pair.masterId, pair.listId);
                    });
                });

                select.setAttribute('data-admin-preset-bound', '1');
            }

            function initAdminPermissionPage() {
                var pairs = [
                    { masterId: '<%= check_all_quyen_quanlynhanvien.ClientID %>', listId: '<%= check_list_quyen_quanlynhanvien.ClientID %>' },
                    { masterId: '<%= check_all_quyen_1.ClientID %>', listId: '<%= check_list_quyen_1.ClientID %>' },
                    { masterId: '<%= check_all_quyen_home_customer.ClientID %>', listId: '<%= check_list_quyen_home_customer.ClientID %>' },
                    { masterId: '<%= check_all_quyen_home_development.ClientID %>', listId: '<%= check_list_quyen_home_development.ClientID %>' },
                    { masterId: '<%= check_all_quyen_home_ecosystem.ClientID %>', listId: '<%= check_list_quyen_home_ecosystem.ClientID %>' },
                    { masterId: '<%= check_all_quyen_shop_partner.ClientID %>', listId: '<%= check_list_quyen_shop_partner.ClientID %>' },
                    { masterId: '<%= check_all_quyen_home_content.ClientID %>', listId: '<%= check_list_quyen_home_content.ClientID %>' }
                ];

                pairs.forEach(function (pair) {
                    bindGroup(pair.masterId, pair.listId);
                    syncMaster(pair.masterId, pair.listId);
                });
                bindPreset('<%= ddl_admin_permission_preset.ClientID %>', pairs);
            }

            function syncAdminCreatePresetVisibility() {
                var accountTypeSelect = document.getElementById('<%= DropDownList1.ClientID %>');
                var presetPanel = document.getElementById('<%= pn_admin_create_preset.ClientID %>');
                var presetSelect = document.getElementById('<%= ddl_admin_create_preset.ClientID %>');
                if (!accountTypeSelect || !presetPanel) {
                    return;
                }

                var showPreset = (accountTypeSelect.value || '').trim() === 'Nhân viên admin';
                presetPanel.style.display = showPreset ? '' : 'none';
                presetPanel.setAttribute('aria-hidden', showPreset ? 'false' : 'true');
                if (!showPreset && presetSelect) {
                    presetSelect.value = '';
                }
            }

            function bindAdminCreatePresetVisibility() {
                var accountTypeSelect = document.getElementById('<%= DropDownList1.ClientID %>');
                if (!accountTypeSelect) {
                    return;
                }

                if (accountTypeSelect.getAttribute('data-admin-create-bound') !== '1') {
                    accountTypeSelect.addEventListener('change', syncAdminCreatePresetVisibility);
                    accountTypeSelect.setAttribute('data-admin-create-bound', '1');
                }

                syncAdminCreatePresetVisibility();
            }

            function initAdminAccountPage() {
                initAdminPermissionPage();
                bindAdminCreatePresetVisibility();
            }

            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', initAdminAccountPage);
            } else {
                initAdminAccountPage();
            }

            if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    initAdminAccountPage();
                });
            }
        })();
    </script>

</asp:Content>
