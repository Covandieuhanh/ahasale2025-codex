<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true" CodeFile="gioi-thieu-nguoi-dung.aspx.cs"
    Inherits="home_page_gioi_thieu_nguoi_dung" %>

<asp:Content ID="cHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>

    <style>
        .aha-wrap {
            min-height: calc(100vh - 56px);
            background: linear-gradient(180deg,
                rgba(32,201,151,.14) 0%,
                rgba(32,201,151,.05) 35%,
                #fff 100%);
        }

        .aha-card {
            max-width: 620px;
            margin: 0 auto;
            border: 1px solid rgba(98,105,118,.16);
            box-shadow: 0 12px 36px rgba(0,0,0,.08);
            border-radius: 12px;
        }

        .aha-refbox {
            background: #f8f9fa;
            border: 1px solid rgba(98,105,118,.16);
            border-radius: 12px;
            padding: 16px;
        }

        .aha-note {
            font-size: 12px;
            color: #626976;
        }

        .js-readonly-select {
            background-color: #f8f9fa;
            cursor: not-allowed;
        }

        .aha-preview img {
            max-width: 120px;
            border-radius: 10px;
            border: 1px solid rgba(98,105,118,.16);
        }
    </style>
</asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="main" runat="Server">

    <asp:UpdatePanel ID="up_reg" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="aha-wrap">
                <div class="page page-center">
                    <div class="container py-5">

                        <div class="card card-md aha-card">
                            <div class="card-body">

                                <h2 class="h2 text-center mb-1">Đăng ký tài khoản mới</h2>
                                <div class="text-secondary text-center mb-4">
                                    Đăng ký thông qua đường dẫn giới thiệu
                                </div>

                                <!-- REF INFO -->
                                <div class="aha-refbox mb-4">
                                    <div class="mb-3">
                                        <label class="form-label">Người giới thiệu</label>
                                        <div class="fw-bold">
                                            <asp:Label ID="lb_ref_display" runat="server"></asp:Label>
                                        </div>
                                    </div>

                                    <div>
                                        <label class="form-label">Loại tài khoản mặc định</label>
                                        <div class="fw-bold">
                                            <asp:Label ID="lb_phanloai_display" runat="server"></asp:Label>
                                        </div>
                                        <div class="aha-note mt-2">
                                            Loại tài khoản tự động theo người giới thiệu, bạn không thể chọn.
                                        </div>
                                    </div>
                                </div>

                                <!-- FORM -->
                                <div class="mb-3">
                                    <label class="form-label text-danger fw-bold">Số điện thoại đăng nhập</label>
                                    <div class="input-group input-group-flat">
                                        <span class="input-group-text"><i class="ti ti-phone"></i></span>
                                        <asp:TextBox ID="txt_taikhoan" runat="server"
                                            CssClass="form-control" MaxLength="20" autocomplete="tel"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label text-danger fw-bold">Mật khẩu</label>
                                    <div class="input-group input-group-flat">
                                        <span class="input-group-text"><i class="ti ti-key"></i></span>
                                        <asp:TextBox ID="txt_matkhau" runat="server"
                                            CssClass="form-control js-password"
                                            TextMode="Password"></asp:TextBox>
                                        <span class="input-group-text">
                                            <a href="javascript:;" class="js-toggle-password link-secondary">
                                                <i class="ti ti-eye"></i>
                                            </a>
                                        </span>
                                    </div>
                                </div>

                                <!-- GIỮ DROPDOWN NHƯ CŨ -->
                                <div class="mb-3">
                                    <label class="form-label text-danger fw-bold">Loại tài khoản</label>
                                    <asp:DropDownList ID="DropDownList1" runat="server"
                                        CssClass="form-select js-readonly-select">
                                        <asp:ListItem Value="Cộng tác phát triển">Cộng tác phát triển</asp:ListItem>
                                        <asp:ListItem Value="Đồng hành hệ sinh thái">Đồng hành hệ sinh thái</asp:ListItem>
                                        <asp:ListItem Value="Gian hàng đối tác">Gian hàng đối tác</asp:ListItem>
                                        <asp:ListItem Value="Khách hàng">Khách hàng</asp:ListItem>
                                        <asp:ListItem Value="Tài khoản tổng">Tài khoản tổng</asp:ListItem>
                                    </asp:DropDownList>
                                    <div class="aha-note mt-1">
                                        Bạn không thể chọn loại tài khoản tại trang này.
                                    </div>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label text-danger fw-bold">Email</label>
                                    <asp:TextBox ID="txt_email" runat="server"
                                        CssClass="form-control"></asp:TextBox>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Ảnh đại diện</label>
                                    <input type="file" id="fileInput" class="form-control" onchange="uploadFile()" />
                                    <div id="message" runat="server" class="text-danger small mt-2"></div>
                                    <div id="uploadedFilePath" class="aha-preview mt-2"></div>
                                    <asp:TextBox ID="txt_link_fileupload" runat="server" Style="display:none" />
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Họ tên</label>
                                    <asp:TextBox ID="txt_hoten" runat="server"
                                        CssClass="form-control"></asp:TextBox>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Ngày sinh</label>
                                    <asp:TextBox ID="txt_ngaysinh" runat="server"
                                        CssClass="form-control"
                                        MaxLength="10"
                                        placeholder="dd/MM/yyyy"></asp:TextBox>
                                </div>

                                <asp:HiddenField ID="hdn_ref_taikhoan" runat="server" />
                                <asp:HiddenField ID="hdn_ref_phanloai" runat="server" />

                                <!-- ✅ Checkbox đồng ý điều khoản -->
                                <div class="mb-4">
                                    <div class="form-check">
                                        <asp:CheckBox ID="cb_dongy" runat="server" CssClass="form-check-input" />
                                        <label class="form-check-label" for="<%= cb_dongy.ClientID %>">
                                            Bằng việc Đăng ký, bạn đã đọc và đồng ý với
                                            <a href="javascript:void(0);" class="link-primary"
                                               data-bs-toggle="modal" data-bs-target="#modal-dieukhoan">Điều khoản sử dụng</a>
                                            và
                                            <a href="javascript:void(0);" class="link-primary"
                                               data-bs-toggle="modal" data-bs-target="#modal-chinhsach">Chính sách bảo mật</a>
                                            của AhaSale
                                        </label>
                                    </div>
                                </div>

                                <div class="form-footer">
                                    <asp:Button ID="but_dangky" runat="server"
                                        Text="ĐĂNG KÝ"
                                        CssClass="btn btn-success w-100"
                                        OnClick="but_dangky_Click" />
                                </div>

                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <!-- ✅ MODAL: Điều khoản sử dụng -->
            <div class="modal modal-blur fade" id="modal-dieukhoan" tabindex="-1" aria-hidden="true">
                <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Điều khoản sử dụng</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            ...
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ✅ MODAL: Chính sách bảo mật -->
            <div class="modal modal-blur fade" id="modal-chinhsach" tabindex="-1" aria-hidden="true">
                <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Chính sách bảo mật</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            ...
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress runat="server" DisplayAfter="0">
        <ProgressTemplate>
            <div class="page-loading active">
                <div class="page-loading-card">
                    <span class="spinner-border"></span>
                    <div class="text-secondary">Loading...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="cFootSau" ContentPlaceHolderID="foot_sau" runat="Server">
<script>
</script>
</asp:Content>
