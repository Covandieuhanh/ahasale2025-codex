<%@ Page Title="Phân quyền tài khoản admin" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="phan-quyen.aspx.cs" Inherits="admin_quan_ly_tai_khoan_phan_quyen" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .admin-permission-page {
            display: flex;
            flex-direction: column;
            gap: 16px;
        }

        .admin-permission-hero,
        .admin-permission-card {
            border: 1px solid #dce7f2;
            border-radius: 24px;
            background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
            box-shadow: 0 16px 34px rgba(15, 41, 64, 0.08);
        }

        .admin-permission-hero {
            padding: 22px 24px;
        }

        .admin-permission-eyebrow {
            margin: 0 0 8px;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: .14em;
            text-transform: uppercase;
            color: #c81e1e;
        }

        .admin-permission-title {
            margin: 0;
            font-size: 34px;
            line-height: 1.08;
            color: #123148;
        }

        .admin-permission-desc {
            margin: 10px 0 0;
            color: #688094;
            font-size: 15px;
            line-height: 1.7;
        }

        .admin-permission-meta {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 12px;
            margin-top: 18px;
        }

        .admin-permission-meta-card {
            padding: 14px 16px;
            border: 1px solid #e4edf5;
            border-radius: 18px;
            background: #fff;
        }

        .admin-permission-meta-label {
            margin: 0;
            color: #6c8194;
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: .06em;
        }

        .admin-permission-meta-value {
            margin: 6px 0 0;
            color: #123148;
            font-size: 18px;
            font-weight: 800;
            line-height: 1.4;
        }

        .admin-permission-card-head {
            display: flex;
            flex-wrap: wrap;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            padding: 18px 20px;
            border-bottom: 1px solid #e7eef5;
        }

        .admin-permission-card-head h2 {
            margin: 0;
            font-size: 24px;
            color: #123148;
        }

        .admin-permission-card-head p {
            margin: 6px 0 0;
            color: #6b8195;
            font-size: 13px;
            line-height: 1.6;
        }

        .admin-permission-card-body {
            padding: 20px;
        }

        .admin-permission-message {
            margin-bottom: 16px;
            padding: 14px 16px;
            border-radius: 18px;
            border: 1px solid #dce7f2;
            background: #f7fbff;
            color: #23435d;
            font-weight: 700;
        }

        .admin-permission-message.error {
            border-color: #fecaca;
            background: #fff1f2;
            color: #b91c1c;
        }

        .admin-permission-preset label {
            display: block;
            margin-bottom: 8px;
            font-size: 14px;
            font-weight: 800;
            color: #123148;
        }

        .admin-permission-preset-note {
            margin-top: 8px;
            color: #6b8195;
            font-size: 13px;
            line-height: 1.7;
        }

        .admin-permission-group {
            margin-top: 18px;
            padding: 18px;
            border: 1px solid #e4edf5;
            border-radius: 20px;
            background: #fff;
        }

        .admin-permission-scope-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 12px;
            margin-top: 16px;
        }

        .admin-permission-scope-card {
            padding: 14px 16px;
            border: 1px solid #e4edf5;
            border-radius: 18px;
            background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
        }

        .admin-permission-scope-card.super-admin {
            border-color: #fecaca;
            background: linear-gradient(180deg, #fff6f6 0%, #fff1f2 100%);
        }

        .admin-permission-scope-card h3 {
            margin: 0;
            font-size: 17px;
            color: #123148;
        }

        .admin-permission-scope-card p {
            margin: 8px 0 0;
            color: #5c7489;
            font-size: 13px;
            line-height: 1.7;
        }

        .admin-permission-scope-tag {
            display: inline-flex;
            align-items: center;
            min-height: 28px;
            padding: 0 10px;
            border-radius: 999px;
            background: #eef5fb;
            color: #23435d;
            font-size: 11px;
            font-weight: 800;
            letter-spacing: .04em;
            text-transform: uppercase;
        }

        .admin-permission-scope-card.super-admin .admin-permission-scope-tag {
            background: #fee2e2;
            color: #b91c1c;
        }

        .admin-permission-group .text-bold {
            font-size: 15px;
            color: #123148;
        }

        .admin-permission-group .checkBoxList label,
        .admin-permission-group .checkbox label {
            color: #23435d;
        }

        .admin-permission-actions {
            display: flex;
            flex-wrap: wrap;
            justify-content: flex-end;
            gap: 12px;
            margin-top: 24px;
        }

        .admin-permission-actions .button,
        .admin-permission-actions .admin-permission-back {
            min-height: 44px;
            padding: 0 18px;
            border-radius: 999px !important;
            font-weight: 800 !important;
        }

        .admin-permission-back {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            border: 1px solid #dce7f2;
            background: #fff;
            color: #123148 !important;
            text-decoration: none !important;
        }

        .admin-permission-back:hover {
            background: #f8fbff;
        }

        @media (max-width: 640px) {
            .admin-permission-hero,
            .admin-permission-card-body,
            .admin-permission-card-head {
                padding: 16px;
            }

            .admin-permission-title {
                font-size: 28px;
            }

            .admin-permission-actions > * {
                width: 100%;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="admin-permission-page">
        <section class="admin-permission-hero">
            <p class="admin-permission-eyebrow">Quản trị Admin</p>
            <h1 class="admin-permission-title">Phân quyền tài khoản</h1>
            <p class="admin-permission-desc">
                Màn này đã được tách thành URL full-page riêng để thao tác phân quyền mượt hơn và đồng nhất với toàn bộ cổng admin.
            </p>

            <div class="admin-permission-meta">
                <div class="admin-permission-meta-card">
                    <p class="admin-permission-meta-label">Tài khoản đang chỉnh</p>
                    <p class="admin-permission-meta-value"><asp:Literal ID="litAccount" runat="server"></asp:Literal></p>
                </div>
                <div class="admin-permission-meta-card">
                    <p class="admin-permission-meta-label">Vai trò hiện tại</p>
                    <p class="admin-permission-meta-value"><asp:Literal ID="litRole" runat="server"></asp:Literal></p>
                </div>
                <div class="admin-permission-meta-card">
                    <p class="admin-permission-meta-label">Mô tả phạm vi</p>
                    <p class="admin-permission-meta-value" style="font-size:15px; font-weight:700; line-height:1.7;"><asp:Literal ID="litRoleDesc" runat="server"></asp:Literal></p>
                </div>
            </div>
        </section>

        <section class="admin-permission-card">
            <div class="admin-permission-card-head">
                <div>
                    <h2>Chọn quyền cho tài khoản admin</h2>
                    <p>Chỉ Super Admin mới được chỉnh màn này. Các quyền tiền, điểm, quyền lõi vẫn chỉ Super Admin mới được thao tác trực tiếp.</p>
                </div>
                <asp:HyperLink ID="hlBackTop" runat="server" CssClass="admin-permission-back">Quay lại danh sách</asp:HyperLink>
            </div>
            <div class="admin-permission-card-body">
                <asp:Panel ID="pnMessage" runat="server" Visible="false" CssClass="admin-permission-message">
                    <asp:Literal ID="litMessage" runat="server"></asp:Literal>
                </asp:Panel>

                <div class="admin-permission-preset">
                    <label for="<%= ddl_admin_permission_preset.ClientID %>">Mẫu vai trò admin</label>
                    <asp:DropDownList ID="ddl_admin_permission_preset" runat="server" data-role="select">
                        <asp:ListItem Value="" Text="Tự chọn thủ công"></asp:ListItem>
                        <asp:ListItem Value="home_customer" Text="Admin khách hàng"></asp:ListItem>
                        <asp:ListItem Value="home_development" Text="Admin cộng tác phát triển"></asp:ListItem>
                        <asp:ListItem Value="home_ecosystem" Text="Admin đồng hành hệ sinh thái"></asp:ListItem>
                        <asp:ListItem Value="shop_partner" Text="Admin gian hàng đối tác"></asp:ListItem>
                        <asp:ListItem Value="home_content" Text="Admin nội dung web"></asp:ListItem>
                    </asp:DropDownList>
                    <div class="admin-permission-preset-note">
                        Mẫu này chỉ gán quyền nghiệp vụ theo đúng tầng. Các thao tác tiền, quyền, điểm lõi vẫn chỉ Super Admin mới được thao tác trực tiếp.
                    </div>
                </div>

                <div class="admin-permission-scope-grid">
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

                <div class="admin-permission-group">
                    <div class="mt-1">
                        <asp:CheckBox ID="check_all_quyen_quanlynhanvien" runat="server" CssClass="text-bold" Text="SUPER ADMIN: CẤU TRÚC CỔNG ADMIN / PHÂN QUYỀN" />
                    </div>
                    <asp:CheckBoxList ID="check_list_quyen_quanlynhanvien" runat="server">
                        <asp:ListItem Text="Super Admin: tạo tài khoản admin và gán 5 vai trò vận hành" Value="5" Selected="false"></asp:ListItem>
                        <asp:ListItem Text="Super Admin: các quyền cấu trúc admin mở rộng (tạm thời)" Value="1" Selected="false"></asp:ListItem>
                    </asp:CheckBoxList>
                </div>

                <div class="admin-permission-group">
                    <div class="mt-1">
                        <asp:CheckBox ID="check_all_quyen_1" runat="server" CssClass="text-bold" Text="SUPER ADMIN: TÀI SẢN LÕI / ĐIỀU PHỐI ĐIỂM" />
                    </div>
                    <asp:CheckBoxList ID="check_list_quyen_1" runat="server">
                        <asp:ListItem Text="Super Admin: tài khoản tài sản lõi / ví token điểm / bridge đối soát" Value="q2_1" Selected="false"></asp:ListItem>
                        <asp:ListItem Text="Super Admin: xem lịch sử chuyển điểm toàn hệ thống" Value="q1_6" Selected="false"></asp:ListItem>
                        <asp:ListItem Text="Super Admin: xem lịch sử chuyển điểm theo phân quyền vận hành" Value="q1_7" Selected="false"></asp:ListItem>
                        <asp:ListItem Text="Super Admin: chuyển điểm đến các tài khoản tổng" Value="q1_1" Selected="false"></asp:ListItem>
                        <asp:ListItem Text="Super Admin: điều phối điểm cho tài khoản tầng khách hàng" Value="q1_2" Selected="false"></asp:ListItem>
                        <asp:ListItem Text="Super Admin: điều phối điểm cho tài khoản gian hàng đối tác" Value="q1_3" Selected="false"></asp:ListItem>
                        <asp:ListItem Text="Super Admin: điều phối điểm cho tài khoản đồng hành hệ sinh thái" Value="q1_4" Selected="false"></asp:ListItem>
                        <asp:ListItem Text="Super Admin: điều phối điểm cho tài khoản cộng tác phát triển" Value="q1_5" Selected="false"></asp:ListItem>
                    </asp:CheckBoxList>
                </div>

                <div class="admin-permission-group">
                    <div class="mt-1">
                        <asp:CheckBox ID="check_all_quyen_home_customer" runat="server" CssClass="text-bold" Text="TÀI KHOẢN TẦNG KHÁCH HÀNG" />
                    </div>
                    <asp:CheckBoxList ID="check_list_quyen_home_customer" runat="server">
                        <asp:ListItem Text="Xem hồ sơ khách hàng, duyệt yêu cầu điểm theo rule hệ thống và chỉ xử lý đúng phạm vi tầng khách hàng" Value="q2_2" Selected="false"></asp:ListItem>
                    </asp:CheckBoxList>
                </div>

                <div class="admin-permission-group">
                    <div class="mt-1">
                        <asp:CheckBox ID="check_all_quyen_home_development" runat="server" CssClass="text-bold" Text="TÀI KHOẢN TẦNG CỘNG TÁC PHÁT TRIỂN" />
                    </div>
                    <asp:CheckBoxList ID="check_list_quyen_home_development" runat="server">
                        <asp:ListItem Text="Xem hồ sơ cộng tác phát triển, duyệt yêu cầu điểm đúng tầng và không tự ý can thiệp tài sản lõi" Value="q2_3" Selected="false"></asp:ListItem>
                    </asp:CheckBoxList>
                </div>

                <div class="admin-permission-group">
                    <div class="mt-1">
                        <asp:CheckBox ID="check_all_quyen_home_ecosystem" runat="server" CssClass="text-bold" Text="TÀI KHOẢN TẦNG ĐỒNG HÀNH HỆ SINH THÁI" />
                    </div>
                    <asp:CheckBoxList ID="check_list_quyen_home_ecosystem" runat="server">
                        <asp:ListItem Text="Xem hồ sơ đồng hành hệ sinh thái, duyệt yêu cầu điểm đúng tầng và chỉ vận hành trong phạm vi hệ sinh thái" Value="q2_4" Selected="false"></asp:ListItem>
                    </asp:CheckBoxList>
                </div>

                <div class="admin-permission-group">
                    <div class="mt-1">
                        <asp:CheckBox ID="check_all_quyen_shop_partner" runat="server" CssClass="text-bold" Text="TÀI KHOẢN GIAN HÀNG ĐỐI TÁC" />
                    </div>
                    <asp:CheckBoxList ID="check_list_quyen_shop_partner" runat="server">
                        <asp:ListItem Text="Quản trị tài khoản shop, duyệt nghiệp vụ shop và các yêu cầu điểm phát sinh trong phạm vi gian hàng đối tác" Value="q2_5" Selected="false"></asp:ListItem>
                    </asp:CheckBoxList>
                </div>

                <div class="admin-permission-group">
                    <div class="mt-1">
                        <asp:CheckBox ID="check_all_quyen_home_content" runat="server" CssClass="text-bold" Text="TÀI KHOẢN QUẢN LÝ NỘI DUNG WEB AHASALE.VN" />
                    </div>
                    <asp:CheckBoxList ID="check_list_quyen_home_content" runat="server">
                        <asp:ListItem Text="Chỉnh sửa nội dung văn bản hiển thị trên web Ahasale.vn; không quản lý menu, banner, bài viết tổng quan và không can thiệp dữ liệu tài sản lõi" Value="q3_1" Selected="false"></asp:ListItem>
                    </asp:CheckBoxList>
                </div>

                <div class="admin-permission-actions">
                    <asp:HyperLink ID="hlBackBottom" runat="server" CssClass="admin-permission-back">Quay lại danh sách</asp:HyperLink>
                    <asp:Button ID="but_phanquyen" runat="server" CssClass="button success" Text="Lưu phân quyền" OnClick="but_phanquyen_Click" />
                </div>
            </div>
        </section>
    </div>

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

            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', initAdminPermissionPage);
            } else {
                initAdminPermissionPage();
            }
        })();
    </script>

</asp:Content>
