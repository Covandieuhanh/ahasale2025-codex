<%@ Page Title="Khách hàng" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="khach-hang.aspx.cs" Inherits="home_khach_hang" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .tblr-overlay{
            position:fixed; inset:0; background:rgba(0,0,0,.65);
            z-index:99999; display:flex; align-items:center; justify-content:center;
        }
        .avatar-60{ width:60px; height:60px; border-radius:999px; object-fit:cover; }
        .table thead th{ white-space:nowrap; }
        .table td{ vertical-align:middle; }
        .w-240{ width:240px; }
        .modal-body{ max-height: calc(100vh - 220px); overflow:auto; }
        .small-muted{ color: rgba(98,105,118,.85); font-size: 12px; }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">

    <!-- ===================== MODAL ADD ===================== -->
    <asp:UpdatePanel ID="up_add" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_add" runat="server" Visible="false" DefaultButton="but_add_edit">

                <div class="modal modal-blur show" style="display:block;" tabindex="-1" role="dialog" aria-modal="true">
                    <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
                        <div class="modal-content">

                            <div class="modal-header">
                                <h5 class="modal-title">
                                    <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                                </h5>
                                <a href="#" class="btn-close" aria-label="Close" id="close_add" runat="server" onserverclick="but_close_form_add_Click"></a>
                            </div>

                            <div class="modal-body">

                                <div class="row g-3">

                                    <div class="col-md-6">
                                        <label class="form-label text-danger">Tài khoản</label>
                                        <asp:TextBox ID="txt_taikhoan" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>

                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                                        <div class="col-md-6">
                                            <label class="form-label text-danger">Mật khẩu</label>
                                            <div class="input-group input-group-flat">
                                                <asp:TextBox ID="txt_matkhau" TextMode="Password" runat="server" CssClass="form-control js-password"></asp:TextBox>
                                                <span class="input-group-text">
                                                    <a href="javascript:void(0);" class="link-secondary js-toggle-password" aria-label="Hiện mật khẩu">
                                                        <span class="aha-password-toggle-label">Hiện</span>
                                                    </a>
                                                </span>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>

                                    <div class="col-12 d-none">
                                        <label class="form-label text-danger">Loại tài khoản</label>
                                        <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-select">
                                            <asp:ListItem Value="Khách hàng" Text="Khách hàng"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>

                                    <!-- người giới thiệu -->
                                    <div class="col-12">
                                        <label class="form-label">Người giới thiệu</label>
                                        <div class="form-control-plaintext border rounded px-3 py-2 bg-muted-lt">
                                            <asp:Label ID="lb_nguoi_gioi_thieu" runat="server" CssClass="fw-semibold"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <label class="form-label text-danger">Email</label>
                                        <asp:TextBox ID="txt_email" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>

                                    <div class="col-md-6">
                                        <label class="form-label">Điện thoại</label>
                                        <asp:TextBox ID="txt_dienthoai" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>

                                    <div class="col-md-6">
                                        <label class="form-label">Họ tên</label>
                                        <asp:TextBox ID="txt_hoten" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>

                                    <div class="col-md-6">
                                        <label class="form-label">Ngày sinh</label>
                                        <!-- giữ textbox + format cũ, chỉ đổi class -->
                                        <asp:TextBox ID="txt_ngaysinh" runat="server" MaxLength="10"
                                            CssClass="form-control"
                                            data-role="calendar-picker"
                                            data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN"
                                            data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                        <div class="small-muted mt-1">Định dạng: dd/MM/yyyy</div>
                                    </div>

                                    <div class="col-12">
                                        <label class="form-label">Ảnh đại diện</label>

                                        <input type="file" id="fileInput" onchange="uploadFile()" class="form-control" />

                                        <div id="message" runat="server" class="text-danger mt-2"></div>
                                        <div id="uploadedFilePath" class="mt-2"></div>

                                        <div style="display:none">
                                            <asp:TextBox ID="txt_link_fileupload" runat="server"></asp:TextBox>
                                        </div>

                                        <div class="mt-2">
                                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                        </div>

                                        <div class="mt-2">
                                            <asp:Button ID="Button2" runat="server" Text="Xóa ảnh cũ" CssClass="btn btn-outline-danger btn-sm"
                                                Visible="false" OnClick="Button2_Click" />
                                        </div>
                                    </div>

                                </div>

                            </div>

                            <div class="modal-footer">
                                <a href="#" class="btn btn-outline-secondary" id="close_add2" runat="server" onserverclick="but_close_form_add_Click">Đóng</a>
                                <asp:Button ID="but_add_edit" runat="server" Text="" CssClass="btn btn-primary" OnClick="but_add_edit_Click" />
                            </div>

                        </div>
                    </div>
                </div>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="up_add">
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
                <div class="container-xl" style="max-width: 992px;">
                    <div class="row g-2 align-items-center">
                        <div class="col">
                            <div class="page-pretitle">Quản lý</div>
                            <h2 class="page-title">Khách hàng</h2>
                        </div>
                        <div class="col-auto ms-auto d-print-none">
                            <div class="btn-list">
                                <asp:LinkButton ID="but_show_form_add" OnClick="but_show_form_add_Click" runat="server" CssClass="btn btn-primary">
                                    <i class="ti ti-plus"></i>&nbsp;Thêm
                                </asp:LinkButton>
                                <a href="<%= PortalRequest_cl.IsShopPortalRequest() ? "/shop/ho-so-tieu-dung" : "/home/lich-su-chuyen-diem.aspx" %>" class="btn btn-outline-secondary">
                                    <i class="ti ti-history"></i>&nbsp;Lịch sử chuyển điểm
                                </a>
                            </div>
                        </div>
                    </div>

                    <div class="row g-2 align-items-center mt-3">
                        <div class="col-md-6">
                            <asp:TextBox MaxLength="50" ID="txt_timkiem1" runat="server"
                                placeholder="Nhập từ khóa (họ tên / tài khoản / email / điện thoại)"
                                CssClass="form-control"
                                AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                        </div>

                        <div class="col-md-6" style="display:none">
                            <asp:TextBox MaxLength="50" ID="txt_timkiem" runat="server"
                                placeholder="Nhập từ khóa..."
                                CssClass="form-control"
                                AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                        </div>
                    </div>

                    <asp:PlaceHolder ID="phShopNote" runat="server" Visible="false">
                        <div class="small text-muted mt-2">
                            * Số liệu tab Khách hàng chỉ tính các đơn đã ghi nhận điểm (Hồ sơ tiêu dùng ShopOnly).
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>

            <div class="page-body">
                <div class="container-xl" style="max-width: 992px;">

                    <div class="card">
                        <div class="card-header">
                            <div class="text-muted small">
                                <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                                <asp:Label ID="lb_show_md" runat="server" Text="" CssClass="d-none"></asp:Label>
                            </div>

                            <div class="ms-auto d-flex align-items-center gap-2">
                                <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                                    <i class="ti ti-chevron-left"></i>
                                </asp:LinkButton>

                                <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                                    <i class="ti ti-chevron-right"></i>
                                </asp:LinkButton>

                                <!-- mobile controls giữ lại để không đụng code -->
                                <div class="d-none">
                                    <asp:LinkButton ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" />
                                    <asp:LinkButton ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" />
                                </div>
                            </div>
                        </div>

                        <div class="table-responsive">
                            <table class="table card-table table-vcenter">
                                <thead>
                                    <tr>
                                        <th style="width:1px;">ID</th>
                                        <th style="width:1px;" class="text-center">
                                            <input type="checkbox"
                                                onkeypress="if (event.keyCode==13) return false;"
                                                onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </th>
                                        <th class="text-center" style="width:70px;">Ảnh</th>
                                        <th>Tài khoản</th>
                                        <asp:PlaceHolder ID="phHomeMetricHeader" runat="server">
                                            <th class="text-center" style="min-width:120px;">Quyền tiêu dùng</th>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="phShopMetricHeader" runat="server" Visible="false">
                                            <th class="text-center" style="min-width:110px;">Số đơn</th>
                                            <th class="text-center" style="min-width:140px;">Tổng tiêu dùng</th>
                                            <th style="min-width:150px;">Đơn gần nhất</th>
                                            <th style="width:120px;">Đơn</th>
                                        </asp:PlaceHolder>
                                        <th style="min-width:160px;">Họ tên</th>
                                        <th style="min-width:120px;">Ngày sinh</th>
                                        <th style="min-width:140px;">Điện thoại</th>
                                        <th style="min-width:180px;">Email</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <span style="display:none">
                                                <asp:Label ID="lbID" runat="server" Text='<%#Eval("taikhoan") %>'></asp:Label>
                                            </span>

                                            <tr>
                                                <td class="text-center"><%# Eval("id") %></td>

                                                <td class="checkbox-table text-center">
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>

                                                <td class="text-center">
                                                    <a href='<%#Eval("anhdaidien") %>' target="_blank" class="d-inline-block">
                                                        <img src='<%#Eval("anhdaidien") %>' class="avatar-60" />
                                                    </a>
                                                </td>

                                                <td>
                                                    <div class="fw-semibold"><%# Eval("taikhoan") %></div>

                                                    <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("phanloai").ToString()=="Khách hàng" %>'>
                                                        <span class="badge bg-green-lt text-green mt-1">Khách hàng</span>
                                                    </asp:PlaceHolder>
                                                </td>

                                                <asp:PlaceHolder ID="phHomeMetricRow" runat="server" Visible='<%# !IsShopPortalMode %>'>
                                                    <td class="text-center">
                                                        <div class="d-inline-flex align-items-center gap-2">
                                                            <img src="/uploads/images/dong-a.png" width="18" />
                                                            <span class="badge bg-muted-lt text-muted"><%#Eval("DongA","{0:#,##0}") %></span>
                                                        </div>
                                                    </td>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="phShopMetricRow" runat="server" Visible='<%# IsShopPortalMode %>'>
                                                    <td class="text-center">
                                                        <span class="badge bg-blue-lt text-blue fw-semibold"><%# Eval("TotalOrders") %></span>
                                                    </td>
                                                    <td class="text-center">
                                                        <span class="badge bg-green-lt text-green fw-semibold"><%# Eval("TotalAmount", "{0:#,##0}") %></span>
                                                    </td>
                                                    <td><%# Eval("LastOrder", "{0:dd/MM/yyyy HH:mm}") %></td>
                                                    <td>
                                                        <asp:HyperLink ID="lnk_view_orders" runat="server" CssClass="btn btn-sm btn-outline-primary"
                                                            NavigateUrl='<%# GetCustomerOrdersUrl(Eval("taikhoan")) %>'>
                                                            Xem đơn
                                                        </asp:HyperLink>
                                                    </td>
                                                </asp:PlaceHolder>

                                                <td><div class="fw-semibold"><%#Eval("hoten") %></div></td>

                                                <td><%#Eval("ngaysinh","{0:dd/MM/yyyy}") %></td>

                                                <td>
                                                    <a title="Gọi" href="tel:<%#Eval("dienthoai") %>"><%#Eval("dienthoai") %></a>
                                                </td>

                                                <td><%#Eval("email") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <asp:PlaceHolder ID="ph_empty_khach" runat="server" Visible="false">
                                        <tr>
                                            <td colspan="12" class="text-center text-muted py-4">Chưa có khách hàng phù hợp.</td>
                                        </tr>
                                    </asp:PlaceHolder>
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
                    <div class="mt-3 text-white">Đang tải...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <!-- ===================== MODAL: ĐƠN KHÁCH HÀNG (SHOP) ===================== -->
    <asp:UpdatePanel ID="up_orders" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_orders" runat="server" Visible="false">
                <div class="modal modal-blur show" style="display:block;" tabindex="-1" role="dialog" aria-modal="true">
                    <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">
                                    Đơn của khách:
                                    <asp:Label ID="lb_orders_customer" runat="server" Text=""></asp:Label>
                                </h5>
                                <a href="<%= GetCustomerListUrl() %>" class="btn-close" aria-label="Close" id="close_orders"></a>
                            </div>

                            <div class="modal-body">
                                <div class="table-responsive">
                                    <table class="table table-vcenter card-table">
                                        <thead>
                                            <tr>
                                                <th style="width:1px;">ID</th>
                                                <th style="min-width:140px;">Ngày đặt</th>
                                                <th style="min-width:140px;">Trạng thái</th>
                                                <th class="text-end" style="min-width:140px;">Tiêu dùng (VNĐ)</th>
                                                <th class="text-end" style="min-width:120px;">Trao đổi (A)</th>
                                                <th style="width:110px;"></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Repeater ID="rp_orders" runat="server">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="text-center"><%# Eval("id") %></td>
                                                        <td><%# Eval("ngaydat","{0:dd/MM/yyyy HH:mm}") %></td>
                                                        <td><%# Eval("trangthai") %></td>
                                                        <td class="text-end"><%# Eval("tongtien","{0:#,##0}") %></td>
                                                        <td class="text-end"><%# Eval("tongA","{0:#,##0.##}") %></td>
                                                        <td class="text-end">
                                                            <a href="<%# Eval("detail_url") %>" class="btn btn-sm btn-outline-primary">Chi tiết</a>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </div>

                                <div class="small text-muted mt-2">
                                    * Chỉ hiển thị đơn đã ghi nhận điểm (Hồ sơ tiêu dùng ShopOnly).
                                </div>

                                <asp:PlaceHolder ID="ph_orders_empty" runat="server" Visible="false">
                                    <div class="text-center text-muted py-3">Chưa có đơn phù hợp.</div>
                                </asp:PlaceHolder>
                            </div>

                            <div class="modal-footer">
                                <a href="<%= GetCustomerListUrl() %>" class="btn btn-outline-secondary" id="close_orders2">Đóng</a>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

<asp:Content ID="ContentFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="Server">

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
                        uploadedFilePathDiv.innerHTML =
                            "<div class='small text-muted mb-1'>Ảnh mới chọn</div>" +
                            "<img width='120' style='border-radius:12px' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= txt_link_fileupload.ClientID %>').value = xhr.responseText;
                        messageDiv.innerHTML = "";
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

</asp:Content>
