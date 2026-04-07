<%@ Page Title="Hồ sơ quyền tiêu dùng" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true" CodeFile="lich-su-giao-dich.aspx.cs" Inherits="home_lich_su_giao_dich" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        /* Overlay loading (UpdateProgress) */
        .tblr-overlay {
            position: fixed;
            inset: 0;
            background: rgba(0,0,0,.65);
            z-index: 99999;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        /* Make modal scrollable if content long */
        .modal-body {
            max-height: calc(100vh - 220px);
            overflow: auto;
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="main" runat="Server">

    <!-- ===================== MODAL: RÚT ĐIỂM ===================== -->
    <asp:UpdatePanel ID="up_add" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_add" runat="server" Visible="false" DefaultButton="but_add_edit">

                <div class="modal modal-blur show" style="display:block;" tabindex="-1" role="dialog" aria-modal="true">
                    <div class="modal-dialog modal-md modal-dialog-centered" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Rút Quyền tiêu dùng</h5>
                                <a href="#" class="btn-close" aria-label="Close" id="A1" runat="server" onserverclick="but_close_form_add_Click"></a>
                            </div>
                            <div class="modal-body">
                                <div class="mb-3">
                                    <label class="form-label text-danger">Số Quyền tiêu dùng muốn rút</label>
                                    <asp:TextBox ID="txt_dongA_chuyen" runat="server" CssClass="form-control" placeholder="Nhập số Quyền tiêu dùng"></asp:TextBox>
                                    <div class="form-hint">Ví dụ: 100 hoặc 250.5</div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <a href="#" class="btn btn-outline-secondary" id="A3" runat="server" onserverclick="but_close_form_add_Click">Đóng</a>
                                <asp:Button ID="but_add_edit" runat="server" Text="XÁC NHẬN RÚT" CssClass="btn btn-primary" OnClick="but_add_edit_Click" />
                            </div>
                        </div>
                    </div>
                </div>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_add">
        <ProgressTemplate>
            <div class="tblr-overlay">
                <div class="text-center">
                    <div class="spinner-border" role="status"></div>
                    <div class="mt-3 text-white">Đang xử lý...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>


    <!-- ===================== MODAL: NẠP ĐIỂM ===================== -->
    <asp:UpdatePanel ID="up_nap" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_nap" runat="server" Visible="false" DefaultButton="but_nap">

                <div class="modal modal-blur show" style="display:block;" tabindex="-1" role="dialog" aria-modal="true">
                    <div class="modal-dialog modal-xl modal-dialog-centered" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Nạp gói quyền tiêu dùng</h5>
                                <a href="#" class="btn-close" aria-label="Close" id="A2" runat="server" onserverclick="but_close_form_nap_Click"></a>
                            </div>

                            <div class="modal-body">
                                <div class="row g-3">
                                    <div class="col-lg-8">
                                        <div class="card">
                                            <div class="card-body">
                                                <div class="mb-3">
                                                    <label class="form-label">Chọn gói</label>
                                                    <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-select">
                                                        <asp:ListItem Text="Chọn" Value=""></asp:ListItem>
                                                        <asp:ListItem Text="100 Quyền tiêu dùng" Value="1"></asp:ListItem>
                                                        <asp:ListItem Text="200 Quyền tiêu dùng" Value="2"></asp:ListItem>
                                                        <asp:ListItem Text="500 Quyền tiêu dùng" Value="3"></asp:ListItem>
                                                        <asp:ListItem Text="1.000 Quyền tiêu dùng" Value="4"></asp:ListItem>
                                                        <asp:ListItem Text="2.000 Quyền tiêu dùng" Value="5"></asp:ListItem>
                                                        <asp:ListItem Text="5.000 Quyền tiêu dùng" Value="6"></asp:ListItem>
                                                        <asp:ListItem Text="10.000 Quyền tiêu dùng" Value="7"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>

                                                <div class="alert alert-info">
                                                    Quy đổi: <b>1 Quyền tiêu dùng tương đương 1000 VNĐ (Việt Nam đồng)</b>
                                                </div>

                                                <div class="row g-3">
                                                    <div class="col-md-7">
                                                        <div class="card" style="background-image:url('/uploads/images/banner-atm.jpg');background-size:cover;background-position:top;">
                                                            <div class="card-body" style="min-height:240px; background: rgba(0,0,0,.35); color:#fff;">
                                                                <div class="fw-bold mb-2">Thông tin chuyển khoản</div>
                                                                <div>Số tài khoản: <b>613991413</b></div>
                                                                <div>Ngân hàng: <b>MB Bank</b></div>
                                                                <div>Tên TK: <b>CÔNG TY CỔ PHẦN ĐÀO TẠO AHASALE</b></div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-5">
                                                        <div class="card">
                                                            <div class="card-body text-center">
                                                                <div class="fw-bold mb-2">QR chuyển khoản</div>
                                                                <img src="/uploads/images/QrThanhToan.jpg" style="width: 220px; height: auto;" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="mt-3">
                                                    <asp:CheckBox ID="CheckBox1" runat="server" Text="Tôi đã chuyển khoản" />
                                                </div>
                                            </div>
                                            <div class="card-footer text-end">
                                                <asp:Button ID="but_nap" runat="server" Text="NẠP ĐIỂM" CssClass="btn btn-primary" OnClick="but_nap_Click" />
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-4">
                                        <div class="card">
                                            <div class="card-body d-flex justify-content-between align-items-center">
                                                <div class="fw-bold">Hồ sơ quyền tiêu dùng</div>
                                                <a href="#" class="text-decoration-none">Xem tất cả</a>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="modal-footer">
                                <a href="#" class="btn btn-outline-secondary" id="A4" runat="server" onserverclick="but_close_form_nap_Click">Đóng</a>
                            </div>
                        </div>
                    </div>
                </div>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="up_nap">
        <ProgressTemplate>
            <div class="tblr-overlay">
                <div class="text-center">
                    <div class="spinner-border" role="status"></div>
                    <div class="mt-3 text-white">Đang xử lý...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>


    <!-- ===================== MODAL: CHI TIẾT ĐƠN ===================== -->
    <asp:UpdatePanel ID="up_chitiet" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_chitiet" runat="server" Visible="false">

                <div class="modal modal-blur show" style="display:block;" tabindex="-1" role="dialog" aria-modal="true">
                    <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Chi tiết đơn hàng</h5>
                                <a href="#" class="btn-close" aria-label="Close" id="close_add" runat="server" onserverclick="but_close_form_chitiet_Click"></a>
                            </div>

                            <div class="modal-body">
                                <div class="row g-3">
                                    <div class="col-md-6">
                                        <div class="card">
                                            <div class="card-body">
                                                <div class="fw-bold mb-2">Thông tin Gian hàng đối tác</div>
                                                <div class="text-muted">Tên gian hàng đối tác</div>
                                                <div class="fw-semibold"><asp:Label ID="Label100" runat="server" Text=""></asp:Label></div>
                                                <div class="text-muted mt-2">Điện thoại</div>
                                                <div class="fw-semibold"><asp:Label ID="Label101" runat="server" Text=""></asp:Label></div>
                                                <div class="text-muted mt-2">Địa chỉ</div>
                                                <div class="fw-semibold"><asp:Label ID="Label102" runat="server" Text=""></asp:Label></div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="card">
                                            <div class="card-body">
                                                <div class="fw-bold mb-2">Thông tin khách hàng</div>
                                                <div class="text-muted">Tên khách</div>
                                                <div class="fw-semibold"><asp:Label ID="Label103" runat="server" Text=""></asp:Label></div>
                                                <div class="text-muted mt-2">Điện thoại</div>
                                                <div class="fw-semibold"><asp:Label ID="Label104" runat="server" Text=""></asp:Label></div>
                                                <div class="text-muted mt-2">Địa chỉ</div>
                                                <div class="fw-semibold"><asp:Label ID="Label105" runat="server" Text=""></asp:Label></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="card mt-3">
                                    <div class="table-responsive">
                                        <table class="table table-vcenter card-table">
                                            <thead>
                                                <tr>
                                                    <th style="width:1px;">ID</th>
                                                    <th style="width:1px;">Ảnh</th>
                                                    <th>Tên sản phẩm</th>
                                                    <th class="text-end">Giá (Quyền tiêu dùng)</th>
                                                    <th class="text-center">Số lượng</th>
                                                    <th class="text-end">Trao đổi</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:Repeater ID="Repeater2" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td class="text-center"><%# Eval("id") %></td>
                                                            <td class="text-center">
                                                                <img src="<%# Eval("image") %>" width="60" height="60" style="object-fit:cover;border-radius:8px;" />
                                                            </td>
                                                         <td>
    <a href="/<%# Eval("name_en") %>-<%# Eval("id") %>.html" class="text-decoration-none">
        <%# Eval("name") %>
    </a>
</td>

                                                            <td class="text-end">
                                                                <%# Eval("giaban","{0:#,##0.##}") %>
                                                                <img src="/uploads/images/dong-a.png" style="width:18px" class="ms-1" />
                                                            </td>
                                                            <td class="text-center"><%# Eval("soluong") %></td>
                                                            <td class="text-end">
                                                                <%# Eval("thanhtien","{0:#,##0.##}") %>
                                                                <img src="/uploads/images/dong-a.png" style="width:18px" class="ms-1" />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>

                            </div>

                            <div class="modal-footer">
                                <a href="#" class="btn btn-outline-secondary" id="A5" runat="server" onserverclick="but_close_form_chitiet_Click">Đóng</a>
                            </div>
                        </div>
                    </div>
                </div>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="up_chitiet">
        <ProgressTemplate>
            <div class="tblr-overlay">
                <div class="text-center">
                    <div class="spinner-border" role="status"></div>
                    <div class="mt-3 text-white">Đang xử lý...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>


    <!-- ===================== MAIN ===================== -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="page-header d-print-none">
                <div class="container-xl">
                    <div class="row g-2 align-items-center">
                        <div class="col">
                            <%--<div class="page-pretitle">Lịch sử hồ sơ tiêu dùng</div>--%>
                            <h2 class="page-title">Hồ sơ quyền tiêu dùng</h2>
                        </div>
                        <div class="col-auto ms-auto d-print-none">
                            <div class="btn-list">
                                <asp:LinkButton ID="but_show_form_nap" OnClick="but_show_form_nap_Click" runat="server" CssClass="btn btn-primary">
                                    <i class="ti ti-plus"></i>&nbsp;Nạp
                                </asp:LinkButton>
                                <asp:LinkButton ID="but_show_form_add" OnClick="but_show_form_add_Click" runat="server" CssClass="btn btn-outline-primary">
                                    <i class="ti ti-arrow-bar-to-up"></i>&nbsp;Rút
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>

                    <div class="row g-2 align-items-center mt-2">
                        <div class="col-auto text-muted">
                            <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                                    <asp:Literal ID="litPager" runat="server"></asp:Literal>
                        </div>
                        <div class="col-auto ms-auto">
                            <div class="btn-list">
                                <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                                    <i class="ti ti-chevron-left"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                                    <i class="ti ti-chevron-right"></i>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>

                </div>
            </div>

            <div class="page-body">
                <div class="container-xl">

                    <div class="alert alert-info">
                        <small>
                            Quyền tiêu dùng là đơn vị quy ước nội bộ được hiển thị và sử dụng trên nền tảng ahasale.vn, đóng vai trò làm phương tiện trao đổi giá trị.
<br />Tỷ lệ tham chiếu: 1 Quyền tiêu dùng tương đương 1000 VNĐ (Việt Nam đồng).
<br />Lưu ý: Quyền tiêu dùng không phải là tiền tệ hợp pháp và chỉ có hiệu lực trong phạm vi nền tảng.
                        </small>
                    </div>

                    <!-- Search (giữ control, hiện tại code đang comment phần lọc) -->
                    <div class="row g-2 mb-3">
                        <div class="col-md-6">
                            <asp:TextBox ID="txt_timkiem1" runat="server" MaxLength="50" CssClass="form-control"
                                placeholder="Nhập từ khóa" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                        </div>
                        <div class="col-md-6" style="display:none">
                            <asp:TextBox ID="txt_timkiem" runat="server" MaxLength="50" CssClass="form-control"
                                placeholder="Nhập từ khóa" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                        </div>
                        <div class="d-none">
                            <asp:Label ID="lb_show_md" runat="server" Text=""></asp:Label>
                            <asp:LinkButton ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" />
                            <asp:LinkButton ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" />
                        </div>
                    </div>

                    <div class="card">
                        <div class="table-responsive">
                            <table class="table table-vcenter card-table">
                                <thead>
                                    <tr>
                                        <th style="width:1px;">ID</th>
                                        <th style="width:140px;">Ngày</th>
                                        <th class="text-end" style="min-width:140px;">Quyền tiêu dùng</th>
                                        <th>Nội dung</th>
                                        <th style="width:120px;">Đơn</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="text-center"><%# Eval("id") %></td>
                                                <td><%# Eval("ngay","{0:dd/MM/yyyy}") %></td>

                                                <td class="text-end">
                                                    <asp:PlaceHolder ID="PlaceHolder19" runat="server" Visible='<%# Eval("CongTru").ToString()=="True" %>'>
                                                        <span class="badge bg-green-lt">+ <%# Eval("dongA","{0:#,##0.##}") %></span>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# Eval("CongTru").ToString()=="False" %>'>
                                                        <span class="badge bg-yellow-lt text-yellow">- <%# Eval("dongA","{0:#,##0.##}") %></span>
                                                    </asp:PlaceHolder>
                                                    <img src="/uploads/images/dong-a.png" style="width:18px" class="ms-1" />
                                                </td>

                                                <td><%# Eval("ghichu") %></td>

                                                <td>
                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("id_donhang").ToString()!="" %>'>
                                                        <asp:LinkButton ID="LinkButton1" OnClick="LinkButton1_Click" CommandArgument='<%# Eval("id_donhang") %>'
                                                            runat="server" CssClass="btn btn-sm btn-outline-primary">
                                                            Chi tiết
                                                        </asp:LinkButton>
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
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="tblr-overlay">
                <div class="text-center">
                    <div class="spinner-border" role="status"></div>
                    <div class="mt-3 text-white">Đang tải dữ liệu...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="foot_sau" runat="Server">
</asp:Content>
