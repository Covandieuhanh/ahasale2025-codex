<%@ Page Title="Lịch sử chuyển điểm" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master"
    AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_lich_su_chuyen_diem_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:UpdatePanel ID="up_add" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_add" runat="server" Visible="false" DefaultButton="but_add_edit">
                <div class="admin-fullpage-head admin-route-panel-head">
                    <div class='admin-fullpage-shell admin-fullpage-head-shell admin-route-panel-shell admin-route-panel-head-shell'>
                        <div class="admin-route-panel-actions">
                            <asp:HyperLink ID="close_add" runat="server" CssClass="admin-route-back-link">Quay lại danh sách</asp:HyperLink>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">CHUYỂN ĐIỂM</div>
                            <hr />
                        </div>
                    </div>
                </div>

                <div class="admin-fullpage-body-wrap admin-route-panel-body-wrap">
                    <div class='admin-fullpage-shell admin-fullpage-dialog admin-route-panel-shell admin-route-panel-dialog'>
                        <div class="bg-white border bd-transparent admin-fullpage-body admin-route-panel-body pl-4 pl-8-md pr-8-md pr-4">
                            <div class="row">
                                <div class="cell-lg-12">
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Tài khoản nhận Quyền tiêu dùng</label>
                                        <div>
                                            <asp:DropDownList ID="DropDownList1" runat="server" data-role="select"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="mt-3">
                                        <label class="fw-600 fg-red">Số Quyền tiêu dùng muốn chuyển</label>
                                        <div>
                                            <asp:TextBox ID="txt_dongA_chuyen" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_add_edit" runat="server" Text="XÁC NHẬN CHUYỂN ĐIỂM"
                                    CssClass="button dark" OnClick="but_add_edit_Click" />
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

    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" class="admin-fullpage-toolbar admin-route-toolbar">
                    <ul class="h-menu bg-white">
                        <asp:PlaceHolder ID="ph_transfer_action" runat="server">
                            <li data-role="hint" data-hint-position="top" data-hint-text="Chuyển điểm">
                                <asp:HyperLink ID="but_show_form_add" runat="server"><small>Chuyển điểm</small></asp:HyperLink>
                            </li>
                        </asp:PlaceHolder>

                        <li data-role="hint" data-hint-position="top" data-hint-text="Tìm kiếm">
                            <asp:LinkButton ID="but_timkiem" ClientIDMode="Static" OnClick="but_timkiem_Click" runat="server"><span class="mif-search"></span></asp:LinkButton>
                        </li>

                        <li data-role="hint" data-hint-position="top" data-hint-text="Làm mới">
                            <asp:LinkButton ID="but_xoa_timkiem" OnClick="but_xoa_timkiem_Click" runat="server"><span class="mif-loop2"></span></asp:LinkButton>
                        </li>

                        <li class="bd-gray border bd-default mt-2 d-block-lg d-none" style="height: 24px"></li>
                        <li class="d-block-lg d-none">
                            <a data-role="hint" data-hint-position="top" data-hint-text="Hiển thị">
                                <small><asp:Label ID="lb_show" runat="server" Text=""></asp:Label></small>
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

                <div id="timkiem-fixtop-bc" class="aha-admin-toolbar-search admin-fullpage-searchbar admin-route-searchbar d-none d-block-sm">
                    <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>"
                        ID="txt_timkiem" runat="server" placeholder="Nhập từ khóa"
                        data-role="input" CssClass="input-small"
                        AutoPostBack="false" data-sync-key="lscd-search" data-enter-click="but_timkiem"></asp:TextBox>
                </div>
            </div>

            <div class="p-3">
                <div class="text-bold fg-cyan"><asp:Label ID="lb_tab_caption" runat="server" Text=""></asp:Label></div>
                <div class="profile-tabs">
                    <asp:HyperLink ID="hl_tab_tieudung" runat="server" CssClass="profile-tab">Hồ sơ quyền tiêu dùng</asp:HyperLink>
                    <asp:HyperLink ID="hl_tab_uudai" runat="server" CssClass="profile-tab">Hồ sơ quyền ưu đãi</asp:HyperLink>
                    <asp:HyperLink ID="hl_tab_laodong" runat="server" CssClass="profile-tab">Hồ sơ hành vi lao động</asp:HyperLink>
                    <asp:HyperLink ID="hl_tab_ganket" runat="server" CssClass="profile-tab">Hồ sơ chỉ số gắn kết</asp:HyperLink>
                    <asp:HyperLink ID="hl_tab_shoponly" runat="server" CssClass="profile-tab">Hồ sơ gian hàng đối tác</asp:HyperLink>
                </div>

                <asp:Panel ID="pn_bridge_summary" runat="server" CssClass="bridge-summary-card" Visible="false">
                    <div class="row">
                        <div class="cell-lg-7">
                            <div class="bridge-summary-title">Theo dõi Token Bridge (tài khoản tổng)</div>
                            <div class="bridge-kv">Tài khoản tổng: <b><asp:Label ID="lb_bridge_treasury_account" runat="server" /></b></div>
                            <div class="bridge-kv">Tên tài khoản: <asp:Label ID="lb_bridge_treasury_name" runat="server" /></div>
                            <div class="bridge-kv">Địa chỉ BSC theo dõi: <asp:Label ID="lb_bridge_deposit_address" runat="server" /></div>
                            <div class="bridge-kv">Token contract: <asp:Label ID="lb_bridge_token_contract" runat="server" /></div>
                        </div>
                        <div class="cell-lg-5 text-right-lg text-left">
                            <div class="bridge-kv">Bridge: <span class="bridge-inline-badge"><asp:Label ID="lb_bridge_enabled" runat="server" /></span></div>
                            <div class="bridge-kv">Tỷ lệ quy đổi: <b><asp:Label ID="lb_bridge_point_rate" runat="server" /></b> điểm / 1 token</div>
                            <div class="bridge-kv">Xác nhận tối thiểu: <b><asp:Label ID="lb_bridge_min_confirmations" runat="server" /></b> block</div>
                            <div class="bridge-kv mt-2">Điểm hiện tại trong tài khoản tổng:</div>
                            <div class="bridge-points"><asp:Label ID="lb_bridge_points_now" runat="server" /></div>
                            <div class="mt-2">
                                <asp:LinkButton ID="but_refresh_bridge" runat="server" CssClass="button small light" OnClick="but_refresh_bridge_Click">Làm mới dữ liệu bridge</asp:LinkButton>
                            </div>
                        </div>
                    </div>
                    <div class="mt-3">
                        <div class="text-bold mb-1">Giao dịch bridge gần nhất</div>
                        <div class="bcorn-fix-title-table-container aha-admin-grid">
                            <table class="bcorn-fix-title-table">
                                <thead>
                                    <tr>
                                        <th style="width: 1px;">ID</th>
                                        <th class="text-center" style="min-width: 120px;">Thời gian</th>
                                        <th class="text-center" style="min-width: 80px;">Chiều</th>
                                        <th class="text-center" style="min-width: 120px;">Số token</th>
                                        <th class="text-center" style="min-width: 120px;">Điểm</th>
                                        <th class="text-center" style="min-width: 110px;">Trạng thái</th>
                                        <th class="text-center" style="min-width: 170px;">TX Hash</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="RepeaterBridge" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="text-center"><%# Eval("id") %></td>
                                                <td class="text-center"><%# Eval("time_text") %></td>
                                                <td class="text-center">
                                                    <asp:PlaceHolder ID="ph_in" runat="server" Visible='<%# Eval("direction").ToString()=="IN" %>'>
                                                        <span class="mini rounded button success">IN</span>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="ph_out" runat="server" Visible='<%# Eval("direction").ToString()=="OUT" %>'>
                                                        <span class="mini rounded button warning">OUT</span>
                                                    </asp:PlaceHolder>
                                                </td>
                                                <td class="text-right"><%# Eval("token_amount", "{0:#,##0.######}") %></td>
                                                <td class="text-right"><%# Eval("points_credited", "{0:#,##0.##}") %></td>
                                                <td class="text-center"><%# Eval("status") %></td>
                                                <td class="text-center" title='<%# Eval("tx_hash") %>'><%# Eval("tx_hash_short") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                        <div class="bridge-note"><asp:Label ID="lb_bridge_status_note" runat="server" /></div>
                    </div>
                </asp:Panel>

                <div class="d-none-sm d-block">
                    <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>"
                        ID="txt_timkiem1" runat="server" placeholder="Nhập từ khóa"
                        data-role="input" AutoPostBack="false" data-sync-key="lscd-search" data-enter-click="but_timkiem"></asp:TextBox>
                </div>

                <div class="d-none-lg d-block mb-3 mt-0-lg mt-3">
                    <div class="place-left"></div>
                    <div class="place-right text-right">
                        <small class="pr-1"><asp:Label ID="lb_show_md" runat="server" Text=""></asp:Label></small>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Lùi"
                            ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" CssClass="button small light">
                            <span class="mif-chevron-left"></span>
                        </asp:LinkButton>
                        <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Tới"
                            ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" CssClass="button small light">
                            <span class="mif-chevron-right"></span>
                        </asp:LinkButton>
                    </div>
                    <div class="clr-bc"></div>
                </div>

                <asp:Panel ID="pn_tab_tieudung" runat="server">
                    <div class="row">
                        <div class="cell-lg-12">
                            <div class="bcorn-fix-title-table-container aha-admin-grid">
                                <table class="bcorn-fix-title-table">
                                    <thead>
                                        <tr>
                                            <th style="width: 1px;">ID</th>
                                            <th class="text-center" style="min-width: 1px;">Ngày</th>
                                            <th class="text-center" style="min-width: 1px;">Tài khoản</th>
                                            <th class="text-center" style="min-width: 1px;">Người nhận</th>
                                            <th class="text-center" style="min-width: 50px;">Quyền tiêu dùng</th>
                                            <th class="text-center" style="min-width: 120px;">Chuyển/Rút</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="Repeater1" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="text-center"><%# Eval("id") %></td>
                                                    <td><%#Eval("ngay","{0:dd/MM/yyyy}") %></td>
                                                    <td class="text-center"><%# Eval("taikhoan_chuyen") %></td>
                                                    <td class="text-center"><%# Eval("taikhoan_nhan") %></td>
                                                    <td class="text-left">
                                                        <img src="/uploads/images/dong-a.png" width="20" />
                                                        <div class="button mini light rounded"><%#Eval("DongA","{0:#,##0.##}") %></div>
                                                    </td>
                                                    <td class="text-center">
                                                        <asp:PlaceHolder ID="PlaceHolder19" runat="server" Visible='<%#Eval("nap_rut").ToString()=="True" %>'>
                                                            <div class="mini rounded button dark">Chuyển</div>
                                                        </asp:PlaceHolder>

                                                        <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("nap_rut").ToString()=="False" %>'>
                                                            <div class="mini rounded button warning">Rút</div>
                                                            <div class="mt-1">
                                                                <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("trangtrai_rut").ToString()=="Chờ xác nhận" %>'>
                                                                    <asp:Button ID="but_xacnhan_rutdiem"
                                                                        CommandArgument='<%# Eval("id") %>'
                                                                        OnClick="but_xacnhan_rutdiem_Click"
                                                                        runat="server"
                                                                        CssClass="rounded mini dark"
                                                                        Text="Xác nhận" />
                                                                    <asp:Button ID="but_huy_rutdiem"
                                                                        CommandArgument='<%# Eval("id") %>'
                                                                        OnClick="but_huy_rutdiem_Click"
                                                                        runat="server"
                                                                        CssClass="rounded mini alert mt-1"
                                                                        Text="Hủy" />
                                                                </asp:PlaceHolder>

                                                                <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("trangtrai_rut").ToString()=="Hoàn thành" %>'>
                                                                    <div class="mini rounded button success">Hoàn thành</div>
                                                                </asp:PlaceHolder>

                                                                <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("trangtrai_rut").ToString()=="Bị hủy" %>'>
                                                                    <div class="mini rounded button alert">Bị hủy</div>
                                                                </asp:PlaceHolder>
                                                            </div>
                                                        </asp:PlaceHolder>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pn_tab_hoso" runat="server" Visible="false">
                    <div class="row">
                        <div class="cell-lg-12">
                            <div class="bcorn-fix-title-table-container aha-admin-grid">
                                <table class="bcorn-fix-title-table">
                                    <thead>
                                        <tr>
                                            <th style="width: 1px;">ID</th>
                                            <th class="text-center">Ngày</th>
                                            <th class="text-center">Tài khoản</th>
                                            <th class="text-center">Hồ sơ</th>
                                            <th class="text-center">Hành vi</th>
                                            <th class="text-center">Biến động</th>
                                            <th class="text-center">Điểm</th>
                                            <th class="text-center">Nội dung</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="RepeaterHoSo" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="text-center"><%# Eval("id") %></td>
                                                    <td class="text-center"><%# Eval("ngay","{0:dd/MM/yyyy HH:mm}") %></td>
                                                    <td class="text-center"><%# Eval("taikhoan") %></td>
                                                    <td class="text-center"><%# Eval("HoSoName") %></td>
                                                    <td class="text-center"><%# Eval("KyHieu9Text") %></td>
                                                    <td class="text-center">
                                                        <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%# Eval("CongTru").ToString()=="True" %>'>
                                                            <span class="mini rounded button success">Cộng</span>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%# Eval("CongTru").ToString()=="False" %>'>
                                                            <span class="mini rounded button warning">Trừ</span>
                                                        </asp:PlaceHolder>
                                                    </td>
                                                    <td class="text-right">
                                                        <img src="/uploads/images/dong-a.png" width="18" />
                                                        <span class="button mini light rounded"><%# Eval("dongA", "{0:#,##0.##}") %></span>
                                                    </td>
                                                    <td><%# Eval("ghichu") %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                    <asp:Panel ID="pn_yeucau_hanhvi_admin" runat="server" Visible="false">
                        <div class="mt-4 text-bold fg-cyan"><asp:Label ID="lb_yc_heading" runat="server" Text="Yêu cầu ghi nhận hành vi"></asp:Label></div>
                        <div class="yc-kpi-wrap">
                            <div class="yc-kpi">Tổng: <asp:Label ID="lb_yc_tong" runat="server" Text="0" /></div>
                            <div class="yc-kpi">Chờ duyệt: <asp:Label ID="lb_yc_cho_duyet" runat="server" Text="0" /></div>
                            <div class="yc-kpi">Đã duyệt: <asp:Label ID="lb_yc_da_duyet" runat="server" Text="0" /></div>
                            <div class="yc-kpi">Từ chối: <asp:Label ID="lb_yc_tu_choi" runat="server" Text="0" /></div>
                        </div>
                        <div class="row">
                            <div class="cell-lg-12">
                                <div class="bcorn-fix-title-table-container aha-admin-grid">
                                    <table class="bcorn-fix-title-table">
                                        <thead>
                                            <tr>
                                                <th style="width: 1px;">Mã</th>
                                                <th class="text-center">Ngày tạo</th>
                                                <th class="text-center">Tài khoản</th>
                                                <th class="text-center">Hành vi</th>
                                                <th class="text-center">Điểm yêu cầu</th>
                                                <th class="text-center">Trạng thái</th>
                                                <th class="text-center">Người duyệt</th>
                                                <th class="text-center">Cập nhật</th>
                                                <th class="text-center">Ghi chú</th>
                                                <th class="text-center">Thao tác</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Repeater ID="RepeaterYeuCauHanhViAdmin" runat="server">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="text-center"><%# Eval("IdShort") %></td>
                                                        <td class="text-center"><%# Eval("NgayTao", "{0:dd/MM/yyyy HH:mm}") %></td>
                                                        <td class="text-center"><%# Eval("TaiKhoan") %></td>
                                                        <td class="text-center"><%# Eval("TenHanhVi") %></td>
                                                        <td class="text-right"><%# Eval("TongQuyen", "{0:#,##0.##}") %></td>
                                                        <td class="text-center">
                                                            <asp:PlaceHolder ID="ph_yc_status_pending" runat="server" Visible='<%# Eval("TrangThaiUiCode").ToString()=="0" %>'>
                                                                <span class="status-pill">Chờ duyệt</span>
                                                            </asp:PlaceHolder>
                                                            <asp:PlaceHolder ID="ph_yc_status_done" runat="server" Visible='<%# Eval("TrangThaiUiCode").ToString()=="1" %>'>
                                                                <span class="status-pill" style="background:#e9fbf2;border-color:#b9e6d1;color:#0f7a4f;">Đã duyệt</span>
                                                            </asp:PlaceHolder>
                                                            <asp:PlaceHolder ID="ph_yc_status_reject" runat="server" Visible='<%# Eval("TrangThaiUiCode").ToString()=="2" %>'>
                                                                <span class="status-pill" style="background:#fff0f0;border-color:#f3c1c1;color:#b83232;">Từ chối</span>
                                                            </asp:PlaceHolder>
                                                            <asp:PlaceHolder ID="ph_yc_status_paid" runat="server" Visible='<%# Eval("TrangThaiUiCode").ToString()=="3" %>'>
                                                                <span class="status-pill" style="background:#edf4ff;border-color:#c7d9ff;color:#1f4a93;">Đã chi trả</span>
                                                            </asp:PlaceHolder>
                                                        </td>
                                                        <td class="text-center"><%# Eval("NguoiDuyet") %></td>
                                                        <td class="text-center"><%# Eval("NgayCapNhat", "{0:dd/MM/yyyy HH:mm}") %></td>
                                                        <td><%# Eval("GhiChu") %></td>
                                                        <td class="text-center">
                                                            <asp:PlaceHolder ID="ph_action_pending" runat="server" Visible='<%# Eval("TrangThaiUiCode").ToString()=="0" %>'>
                                                                <asp:Button ID="but_duyet_yeucau_hanhvi"
                                                                    CommandArgument='<%# Eval("IdYeuCauRut") %>'
                                                                    OnClick="but_duyet_yeucau_hanhvi_Click"
                                                                    runat="server"
                                                                    CssClass="rounded mini dark"
                                                                    Text="Duyệt" />
                                                                <asp:Button ID="but_tuchoi_yeucau_hanhvi"
                                                                    CommandArgument='<%# Eval("IdYeuCauRut") %>'
                                                                    OnClick="but_tuchoi_yeucau_hanhvi_Click"
                                                                    runat="server"
                                                                    CssClass="rounded mini alert mt-1"
                                                                    Text="Từ chối" />
                                                            </asp:PlaceHolder>
                                                            <asp:PlaceHolder ID="ph_action_can_payout" runat="server" Visible='<%# Eval("CanChiTra").ToString()=="True" %>'>
                                                                <asp:Button ID="but_chitra_yeucau_hanhvi"
                                                                    CommandArgument='<%# Eval("IdYeuCauRut") %>'
                                                                    OnClick="but_chitra_yeucau_hanhvi_Click"
                                                                    runat="server"
                                                                    CssClass="rounded mini success"
                                                                    Text="Chi trả" />
                                                            </asp:PlaceHolder>
                                                            <asp:PlaceHolder ID="ph_action_paid" runat="server" Visible='<%# Eval("DaChiTra").ToString()=="True" %>'>
                                                                <span class="mini rounded button light">Đã chi trả</span>
                                                            </asp:PlaceHolder>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </asp:Panel>

            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="admin-inline-progress" role="status" aria-live="polite"><span class="admin-inline-progress-spinner"></span><span class="admin-inline-progress-text">Đang xử lý...</span></div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>
