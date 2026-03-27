<%@ Page Title="Đăng ký gian hàng đối tác"
    Language="C#"
    MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true"
    CodeFile="dang-ky-gian-hang-doi-tac.aspx.cs"
    Inherits="home_dang_ky_gian_hang_doi_tac" %>

<asp:Content ContentPlaceHolderID="main" runat="server">
<style>
    .gianhang-onboarding-shell {
        max-width: 1100px;
        margin: 0 auto;
        padding: 24px 12px 40px;
    }

    .gianhang-onboarding-card {
        background: #fff;
        border: 1px solid #edf2f7;
        border-radius: 22px;
        box-shadow: 0 20px 60px rgba(15, 23, 42, 0.08);
        overflow: hidden;
    }

    .gianhang-onboarding-header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 16px;
        padding: 22px 28px;
        border-bottom: 1px solid #edf2f7;
    }

    .gianhang-onboarding-brand {
        display: inline-flex;
        align-items: center;
        gap: 12px;
        font-size: 18px;
        font-weight: 700;
        color: #1f2937;
    }

    .gianhang-onboarding-logo {
        width: 42px;
        height: 42px;
        border-radius: 12px;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        color: #fff;
        background: linear-gradient(135deg, #ee4d2d 0%, #ff7a45 100%);
        font-size: 20px;
        font-weight: 800;
    }

    .gianhang-stepbar {
        display: grid;
        grid-template-columns: repeat(2, minmax(0, 1fr));
        gap: 0;
        padding: 24px 28px 8px;
    }

    .gianhang-step {
        position: relative;
        text-align: center;
        color: #9ca3af;
    }

    .gianhang-step:before {
        content: "";
        position: absolute;
        top: 13px;
        left: 50%;
        width: 100%;
        height: 2px;
        background: #e5e7eb;
        z-index: 0;
    }

    .gianhang-step:last-child:before {
        display: none;
    }

    .gianhang-step-dot {
        position: relative;
        z-index: 1;
        width: 26px;
        height: 26px;
        margin: 0 auto 12px;
        border-radius: 999px;
        border: 2px solid #e5e7eb;
        background: #fff;
    }

    .gianhang-step.gianhang-step--active,
    .gianhang-step.gianhang-step--done {
        color: #111827;
    }

    .gianhang-step.gianhang-step--active .gianhang-step-dot,
    .gianhang-step.gianhang-step--done .gianhang-step-dot {
        border-color: #ee4d2d;
        background: #ee4d2d;
        box-shadow: 0 0 0 6px rgba(238, 77, 45, 0.12);
    }

    .gianhang-step.gianhang-step--done .gianhang-step-dot:after {
        content: "";
        position: absolute;
        inset: 7px;
        border-radius: 999px;
        background: #fff;
    }

    .gianhang-onboarding-body {
        padding: 12px 28px 28px;
    }

    .gianhang-section-title {
        margin: 0 0 4px;
        font-size: 30px;
        font-weight: 800;
        color: #111827;
    }

    .gianhang-section-subtitle {
        margin: 0;
        color: #6b7280;
        font-size: 15px;
    }

    .gianhang-grid {
        margin-top: 28px;
        display: grid;
        gap: 18px;
        grid-template-columns: 240px minmax(0, 1fr);
        align-items: start;
    }

    .gianhang-grid-label {
        padding-top: 14px;
        font-size: 15px;
        color: #111827;
        font-weight: 600;
    }

    .gianhang-required {
        color: #ee4d2d;
    }

    .gianhang-input,
    .gianhang-static-box {
        border-radius: 14px !important;
        border: 1px solid #dbe3ee !important;
        min-height: 52px;
    }

    .gianhang-static-box {
        padding: 14px 16px;
        background: #f8fafc;
        color: #334155;
    }

    .gianhang-static-box strong {
        color: #111827;
        display: block;
        margin-bottom: 4px;
    }

    .gianhang-inline-link {
        color: #ee4d2d;
        font-weight: 600;
        text-decoration: none;
    }

    .gianhang-inline-link:hover {
        text-decoration: underline;
    }

    .gianhang-field-hint {
        margin-top: 8px;
        font-size: 13px;
        color: #6b7280;
    }

    .gianhang-footer {
        display: flex;
        justify-content: flex-end;
        gap: 12px;
        padding: 22px 28px 28px;
        border-top: 1px solid #edf2f7;
        background: #fff;
    }

    .gianhang-btn-primary {
        background: linear-gradient(135deg, #ee4d2d 0%, #ff7a45 100%) !important;
        border: none !important;
        color: #fff !important;
        border-radius: 14px !important;
        min-width: 170px;
        min-height: 48px;
        font-weight: 700;
    }

    .gianhang-summary-card {
        margin-top: 24px;
        border: 1px solid #e5e7eb;
        border-radius: 18px;
        padding: 20px;
        background: #fffaf8;
    }

    .gianhang-summary-chip {
        display: inline-flex;
        align-items: center;
        gap: 6px;
        padding: 6px 12px;
        border-radius: 999px;
        font-size: 12px;
        font-weight: 700;
        background: #fff1ec;
        color: #ee4d2d;
        border: 1px solid #ffd5c7;
    }

    .gianhang-history-table td,
    .gianhang-history-table th {
        vertical-align: top;
    }

    @media (max-width: 767.98px) {
        .gianhang-onboarding-header {
            padding: 18px 18px;
        }

        .gianhang-stepbar,
        .gianhang-onboarding-body,
        .gianhang-footer {
            padding-left: 18px;
            padding-right: 18px;
        }

        .gianhang-grid {
            grid-template-columns: 1fr;
            gap: 10px;
        }

        .gianhang-grid-label {
            padding-top: 0;
        }

        .gianhang-section-title {
            font-size: 24px;
        }

        .gianhang-footer {
            flex-direction: column-reverse;
        }

        .gianhang-btn-primary,
        .gianhang-footer .btn {
            width: 100%;
        }
    }
</style>

<asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="gianhang-onboarding-shell">
            <div class="gianhang-onboarding-card">
                <div class="gianhang-onboarding-header">
                    <div class="gianhang-onboarding-brand">
                        <span class="gianhang-onboarding-logo">A</span>
                        <span>Đăng ký trở thành Gian hàng đối tác AhaSale</span>
                    </div>
                    <div class="text-secondary small">
                        <asp:Literal ID="lit_account_root" runat="server"></asp:Literal>
                    </div>
                </div>

                <div class="gianhang-stepbar">
                    <div class='<%= GetStepCss(true) %>'>
                        <div class="gianhang-step-dot"></div>
                        <div>Thông tin Shop</div>
                    </div>
                    <div class='<%= GetStepCss(false) %>'>
                        <div class="gianhang-step-dot"></div>
                        <div>Hoàn tất</div>
                    </div>
                </div>

                <asp:PlaceHolder ID="ph_form" runat="server">
                    <div class="gianhang-onboarding-body">
                        <h1 class="gianhang-section-title">Thông tin Shop</h1>
                        <p class="gianhang-section-subtitle">Hoàn tất bước đầu tiên để hệ thống tạo yêu cầu mở quyền gian hàng cho chính tài khoản Home của bạn.</p>

                        <asp:PlaceHolder ID="ph_request_feedback" runat="server" Visible="false">
                            <div class="alert alert-warning mt-4 mb-0">
                                <strong><asp:Literal ID="lit_request_feedback_title" runat="server"></asp:Literal></strong><br />
                                <asp:Literal ID="lit_request_feedback_body" runat="server"></asp:Literal>
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="ph_profile_warning" runat="server" Visible="false">
                            <div class="alert alert-warning mt-4 mb-0">
                                <strong>Hồ sơ Home chưa đủ thông tin để mở gian hàng.</strong><br />
                                <asp:Literal ID="lit_profile_warning" runat="server"></asp:Literal><br />
                                <asp:HyperLink ID="lnk_edit_profile_warning" runat="server" CssClass="gianhang-inline-link" Text="Cập nhật hồ sơ Home ngay"></asp:HyperLink>
                            </div>
                        </asp:PlaceHolder>

                        <div class="gianhang-grid">
                            <div class="gianhang-grid-label"><span class="gianhang-required">*</span> Tên Shop</div>
                            <div>
                                <asp:TextBox ID="txt_shop_name" runat="server" CssClass="form-control gianhang-input" MaxLength="30"></asp:TextBox>
                                <div class="d-flex justify-content-between gianhang-field-hint">
                                    <span>Tên này sẽ dùng làm tên hiển thị mặc định cho storefront /gianhang.</span>
                                    <span><span id="shop_name_counter">0</span>/30</span>
                                </div>
                            </div>

                            <div class="gianhang-grid-label"><span class="gianhang-required">*</span> Địa chỉ lấy hàng</div>
                            <div>
                                <div class="gianhang-static-box">
                                    <strong><asp:Literal ID="lit_contact_name" runat="server"></asp:Literal> | <asp:Literal ID="lit_contact_phone" runat="server"></asp:Literal></strong>
                                    <asp:Literal ID="lit_pickup_address" runat="server"></asp:Literal>
                                </div>
                                <div class="gianhang-field-hint">
                                    Địa chỉ này đang lấy từ hồ sơ hiện tại của tài khoản Home.
                                    <asp:HyperLink ID="lnk_edit_profile_inline" runat="server" CssClass="gianhang-inline-link" Text="Chỉnh sửa hồ sơ Home"></asp:HyperLink>
                                </div>
                            </div>

                            <div class="gianhang-grid-label"><span class="gianhang-required">*</span> Email</div>
                            <div>
                                <div class="gianhang-static-box">
                                    <asp:Literal ID="lit_contact_email" runat="server"></asp:Literal>
                                </div>
                            </div>

                            <div class="gianhang-grid-label"><span class="gianhang-required">*</span> Số điện thoại</div>
                            <div>
                                <div class="gianhang-static-box">
                                    <asp:Literal ID="lit_contact_phone_dup" runat="server"></asp:Literal>
                                </div>
                            </div>
                        </div>

                        <div class="gianhang-summary-card">
                            <span class="gianhang-summary-chip">Luồng hiện tại: 1 bước</span>
                            <p class="mt-3 mb-0 text-secondary">
                                Sau khi hoàn tất bước này, hệ thống sẽ lưu thông tin shop, gửi yêu cầu mở quyền <strong>/gianhang</strong> và chờ admin duyệt.
                                Các bước vận chuyển, định danh và thuế sẽ được bổ sung ở các phiên bản sau.
                            </p>
                        </div>
                    </div>

                    <div class="gianhang-footer">
                        <asp:HyperLink ID="lnk_back_home_form" runat="server" CssClass="btn btn-outline-secondary" Text="Về Home"></asp:HyperLink>
                        <asp:Button ID="btn_submit" runat="server" CssClass="btn gianhang-btn-primary" Text="Hoàn tất đăng ký" OnClick="btn_submit_Click" />
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="ph_waiting" runat="server" Visible="false">
                    <div class="gianhang-onboarding-body">
                        <h1 class="gianhang-section-title">Đã gửi yêu cầu mở gian hàng</h1>
                        <p class="gianhang-section-subtitle">Admin sẽ duyệt và mở quyền /gianhang cho chính tài khoản Home của bạn.</p>
                        <div class="alert alert-info mt-4 mb-0">
                            <strong>Trạng thái hiện tại:</strong> <asp:Literal ID="lit_waiting_status" runat="server"></asp:Literal><br />
                            <asp:PlaceHolder ID="ph_waiting_time" runat="server" Visible="false">
                                Thời gian gửi: <asp:Literal ID="lit_waiting_time" runat="server"></asp:Literal><br />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="ph_waiting_note" runat="server" Visible="false">
                                Ghi chú admin: <asp:Literal ID="lit_waiting_note" runat="server"></asp:Literal>
                            </asp:PlaceHolder>
                        </div>
                    </div>

                    <div class="gianhang-footer">
                        <asp:HyperLink ID="lnk_back_home_waiting" runat="server" CssClass="btn btn-outline-secondary" Text="Về Home"></asp:HyperLink>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="ph_done" runat="server" Visible="false">
                    <div class="gianhang-onboarding-body">
                        <h1 class="gianhang-section-title">Gian hàng đã sẵn sàng</h1>
                        <p class="gianhang-section-subtitle">Tài khoản Home của bạn đã được mở quyền truy cập /gianhang.</p>
                        <div class="alert alert-success mt-4 mb-0">
                            <strong>Trạng thái:</strong> <asp:Literal ID="lit_done_status" runat="server"></asp:Literal>
                        </div>
                    </div>

                    <div class="gianhang-footer">
                        <asp:HyperLink ID="lnk_back_home_done" runat="server" CssClass="btn btn-outline-secondary" Text="Về Home"></asp:HyperLink>
                        <asp:HyperLink ID="lnk_open_gianhang" runat="server" CssClass="btn gianhang-btn-primary" Text="Vào /gianhang"></asp:HyperLink>
                    </div>
                </asp:PlaceHolder>
            </div>

            <div class="card mt-4">
                <div class="card-header">
                    <h3 class="card-title">Lịch sử đăng ký</h3>
                </div>
                <div class="table-responsive">
                    <table class="table table-vcenter gianhang-history-table">
                        <thead>
                            <tr>
                                <th>Thời gian gửi</th>
                                <th>Trạng thái</th>
                                <th>Thời gian xử lý</th>
                                <th>Ghi chú admin</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rp_history" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("RequestedAtText") %></td>
                                        <td><%# Eval("RequestStatusText") %></td>
                                        <td><%# Eval("ReviewedAtText") %></td>
                                        <td><%# Eval("ReviewNote") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:PlaceHolder ID="ph_history_empty" runat="server">
                                <tr>
                                    <td colspan="4" class="text-center text-secondary py-4">Chưa có lịch sử đăng ký gian hàng đối tác.</td>
                                </tr>
                            </asp:PlaceHolder>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
<script type="text/javascript">
    (function () {
        function bindCounter() {
            var input = document.getElementById('<%= txt_shop_name.ClientID %>');
            var counter = document.getElementById('shop_name_counter');
            if (!input || !counter) return;
            var update = function () {
                counter.textContent = (input.value || '').length.toString();
            };
            input.addEventListener('input', update);
            update();
        }
        if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', bindCounter);
        else bindCounter();
    })();
</script>
</asp:Content>
