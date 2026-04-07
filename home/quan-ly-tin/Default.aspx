<%@ Page Title="Quản lý tin" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="home_quan_ly_bai_Default" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server">
    <style>
        .page-shell-wide { max-width: 1200px; }
        .editor-dialog-wide { max-width: 1200px; margin: 0 auto; }
        .overlay-loading{
            position:fixed; inset:0; background:rgba(0,0,0,.6);
            z-index:99999; display:flex; align-items:center; justify-content:center;
        }

        /* overlay panel add/edit (giữ cơ chế pn_add.Visible) */
        .editor-overlay{
            position:fixed; inset:0; z-index:2000;
            background:rgba(0,0,0,.55);
            overflow:auto; padding:16px 12px;
        }
        .table thead th{ white-space:nowrap; }
        .table td,.table th{ vertical-align:middle; }
        .td-img img{ border-radius:10px; border:1px solid rgba(98,105,118,.18); object-fit:cover; }

        /* topbar tools */
        .toolbar-sticky{
            position: sticky; top: 64px; /* dưới header */
            z-index: 10;
        }
        @media (max-width: 767px){
            .toolbar-sticky{ top: 58px; }
        }

        /* ✅ FIX: Modal/Toast nổi lên trên overlay add/edit */
        .modal{ z-index: 3005 !important; }
        .modal-backdrop{ z-index: 3000 !important; }
        .toast-container, .toast, .tabler-toast, .tabler-notify{ z-index: 3010 !important; }
        .bds-posting-panel{
            border:1px solid rgba(22,163,74,.16);
            border-radius:18px;
            padding:18px;
            background:linear-gradient(180deg, rgba(240,253,244,.95), rgba(255,255,255,.98));
        }
        .bds-posting-title{
            font-size:15px;
            font-weight:800;
            color:#166534;
            margin-bottom:4px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:HiddenField ID="hf_anhphu" runat="server" />
    <asp:HiddenField ID="hf_bds_category_ids" runat="server" />

    <!-- ===== FORM ADD/EDIT (overlay) ===== -->
    <asp:UpdatePanel ID="up_add" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_add" runat="server" Visible="false" DefaultButton="but_add_edit">
                <div class="editor-overlay">
                    <div class="editor-dialog editor-dialog-wide">
                        <div class="card shadow-sm">
                            <div class="card-header d-flex align-items-center justify-content-between">
                                <div class="card-title fw-bold">
                                    <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                                </div>

                                <a href="#" id="close_add" runat="server" onserverclick="but_close_form_add_Click" title="Đóng"
                                   class="btn btn-ghost-danger btn-icon">
                                    <i class="ti ti-x"></i>
                                </a>
                            </div>

                            <div class="card-body">
                                <div class="row g-3">
                                    <div class="col-lg-6">
                                        <label class="form-label">Tên bài viết</label>
                                        <asp:TextBox ID="txt_name" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-6">
                                        <label class="form-label">Ảnh chính</label>
                                        <input type="file" id="fileInput" onchange="uploadFile()" class="form-control" />
                                        <div id="message" class="text-danger small mt-1"></div>
                                        <div id="uploadedFilePath" class="mt-2">
                                            <asp:Literal ID="lit_uploaded_main" runat="server"></asp:Literal>
                                        </div>
                                        <div style="display:none">
                                            <asp:TextBox ID="txt_link_fileupload" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-6">
                                        <label class="form-label">Danh mục</label>
                                        <asp:DropDownList ID="ddl_DanhMuc" runat="server" CssClass="form-select"></asp:DropDownList>
                                    </div>

                                    <div class="col-lg-6">
                                        <label class="form-label">Danh sách ảnh</label>
                                        <input type="file" id="fileListInput" multiple onchange="uploadMultipleFiles()" class="form-control" />
                                        <div id="multiMessage" class="text-danger small mt-1"></div>
                                        <div id="uploadedFileList" class="mt-2">
                                            <asp:Literal ID="lit_uploaded_list" runat="server"></asp:Literal>
                                        </div>
                                    </div>

                                    <div class="col-lg-6">
                                        <label class="form-label">Giá bán (VNĐ)</label>
                                        <asp:TextBox ID="txt_giaban" runat="server"
                                            onfocus="AutoSelect(this)" MaxLength="14" oninput="format_sotien_new(this)"
                                            CssClass="form-control" Text="0"></asp:TextBox>
                                    </div>

                                    <div id="bdsPostingPanel" class="col-12" style="display:none;">
                                        <div class="bds-posting-panel">
                                            <div class="bds-posting-title">Thuộc tính riêng cho tin Bất động sản</div>
                                            <div class="text-muted small mb-3">Nhóm field này giúp giữ metadata BĐS đúng chuẩn để listing và detail trong <code>/bat-dong-san</code> hiển thị đúng logic.</div>
                                            <div class="row g-3">
                                                <div class="col-md-3">
                                                    <label class="form-label">Nhu cầu</label>
                                                    <asp:DropDownList ID="ddl_bds_purpose" runat="server" CssClass="form-select">
                                                        <asp:ListItem Value="sale">Mua bán</asp:ListItem>
                                                        <asp:ListItem Value="rent">Cho thuê</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-md-3">
                                                    <label class="form-label">Loại hình BĐS</label>
                                                    <asp:DropDownList ID="ddl_bds_property_type" runat="server" CssClass="form-select">
                                                        <asp:ListItem Value="apartment">Căn hộ</asp:ListItem>
                                                        <asp:ListItem Value="house">Nhà phố</asp:ListItem>
                                                        <asp:ListItem Value="land">Đất nền</asp:ListItem>
                                                        <asp:ListItem Value="office">Văn phòng</asp:ListItem>
                                                        <asp:ListItem Value="business-premises">Mặt bằng</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-md-2">
                                                    <label class="form-label">Diện tích (m²)</label>
                                                    <asp:TextBox ID="txt_bds_area" runat="server" CssClass="form-control" Text="0" MaxLength="10"></asp:TextBox>
                                                </div>
                                                <div class="col-md-2">
                                                    <label class="form-label">Phòng ngủ</label>
                                                    <asp:TextBox ID="txt_bds_bedrooms" runat="server" CssClass="form-control" Text="0" MaxLength="2"></asp:TextBox>
                                                </div>
                                                <div class="col-md-2">
                                                    <label class="form-label">Phòng tắm</label>
                                                    <asp:TextBox ID="txt_bds_bathrooms" runat="server" CssClass="form-control" Text="0" MaxLength="2"></asp:TextBox>
                                                </div>
                                                <div id="bdsRentDepositWrap" class="col-md-3">
                                                    <label class="form-label">Tiền cọc (VNĐ)</label>
                                                    <asp:TextBox ID="txt_bds_deposit" runat="server" CssClass="form-control" Text="0" MaxLength="14" oninput="format_sotien_new(this)"></asp:TextBox>
                                                </div>
                                                <div id="bdsRentTermWrap" class="col-md-2">
                                                    <label class="form-label">Kỳ hạn thuê (tháng)</label>
                                                    <asp:TextBox ID="txt_bds_rental_term" runat="server" CssClass="form-control" Text="0" MaxLength="3"></asp:TextBox>
                                                </div>
                                                <div id="bdsHouseFloorWrap" class="col-md-2">
                                                    <label class="form-label">Số tầng</label>
                                                    <asp:TextBox ID="txt_bds_floor_count" runat="server" CssClass="form-control" Text="0" MaxLength="2"></asp:TextBox>
                                                </div>
                                                <div id="bdsLandWidthWrap" class="col-md-2">
                                                    <label class="form-label">Ngang (m)</label>
                                                    <asp:TextBox ID="txt_bds_land_width" runat="server" CssClass="form-control" Text="0" MaxLength="10"></asp:TextBox>
                                                </div>
                                                <div id="bdsLandLengthWrap" class="col-md-2">
                                                    <label class="form-label">Dài (m)</label>
                                                    <asp:TextBox ID="txt_bds_land_length" runat="server" CssClass="form-control" Text="0" MaxLength="10"></asp:TextBox>
                                                </div>
                                                <div class="col-md-4">
                                                    <label class="form-label">Pháp lý</label>
                                                    <asp:DropDownList ID="ddl_bds_legal" runat="server" CssClass="form-select">
                                                        <asp:ListItem Value="Sổ hồng riêng">Sổ hồng riêng</asp:ListItem>
                                                        <asp:ListItem Value="Sổ đỏ">Sổ đỏ</asp:ListItem>
                                                        <asp:ListItem Value="HĐMB / đang chờ sổ">HĐMB / đang chờ sổ</asp:ListItem>
                                                        <asp:ListItem Value="Hợp đồng thuê thương mại">Hợp đồng thuê thương mại</asp:ListItem>
                                                        <asp:ListItem Value="Chưa cập nhật">Chưa cập nhật</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-md-4">
                                                    <label class="form-label">Nội thất / bàn giao</label>
                                                    <asp:DropDownList ID="ddl_bds_furnishing" runat="server" CssClass="form-select">
                                                        <asp:ListItem Value="Hoàn thiện">Hoàn thiện</asp:ListItem>
                                                        <asp:ListItem Value="Nội thất cơ bản">Nội thất cơ bản</asp:ListItem>
                                                        <asp:ListItem Value="Full nội thất">Full nội thất</asp:ListItem>
                                                        <asp:ListItem Value="Bàn giao thô">Bàn giao thô</asp:ListItem>
                                                        <asp:ListItem Value="Chưa cập nhật">Chưa cập nhật</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-md-4">
                                                    <label class="form-label">Hướng</label>
                                                    <asp:DropDownList ID="ddl_bds_direction" runat="server" CssClass="form-select">
                                                        <asp:ListItem Value="">Chưa chọn</asp:ListItem>
                                                        <asp:ListItem Value="Đông">Đông</asp:ListItem>
                                                        <asp:ListItem Value="Tây">Tây</asp:ListItem>
                                                        <asp:ListItem Value="Nam">Nam</asp:ListItem>
                                                        <asp:ListItem Value="Bắc">Bắc</asp:ListItem>
                                                        <asp:ListItem Value="Đông Nam">Đông Nam</asp:ListItem>
                                                        <asp:ListItem Value="Đông Bắc">Đông Bắc</asp:ListItem>
                                                        <asp:ListItem Value="Tây Nam">Tây Nam</asp:ListItem>
                                                        <asp:ListItem Value="Tây Bắc">Tây Bắc</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-md-4">
                                                    <label class="form-label">Tên dự án</label>
                                                    <asp:TextBox ID="txt_bds_project" runat="server" CssClass="form-control" MaxLength="250"></asp:TextBox>
                                                </div>
                                                <div class="col-md-4">
                                                    <label class="form-label">Quận/Huyện</label>
                                                    <asp:TextBox ID="txt_bds_district" runat="server" CssClass="form-control" MaxLength="150"></asp:TextBox>
                                                </div>
                                                <div class="col-md-4">
                                                    <label class="form-label">Phường/Xã</label>
                                                    <asp:TextBox ID="txt_bds_ward" runat="server" CssClass="form-control" MaxLength="150"></asp:TextBox>
                                                </div>
                                                <div class="col-md-4">
                                                    <label class="form-label">Địa chỉ hiển thị</label>
                                                    <asp:TextBox ID="txt_bds_address_line" runat="server" CssClass="form-control" MaxLength="500"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- ✅ NEW: Phần trăm ưu đãi -->
                                    <div class="col-lg-6">
                                        <label class="form-label">Phần trăm ưu đãi (0 - 50%)</label>
                                        <asp:TextBox ID="txt_phantram_uu_dai" runat="server"
                                            CssClass="form-control" Text="0" MaxLength="2"
                                            oninput="clamp_percent_0_50(this)"></asp:TextBox>
                                        <div class="text-muted small mt-1">Mặc định 0%. Sẽ được trừ tại hồ sơ ưu đãi của người mua.</div>
                                    </div>

                                    <asp:PlaceHolder ID="ph_company_shop_options" runat="server" Visible="false">
                                        <div class="col-lg-6">
                                            <label class="form-label">Kênh hiển thị sản phẩm</label>
                                            <asp:DropDownList ID="ddl_kenh_hienthi" runat="server" CssClass="form-select">
                                                <asp:ListItem Value="public">Công khai (hiển thị ngoài Home)</asp:ListItem>
                                                <asp:ListItem Value="internal">Nội bộ (chỉ shop công ty tự bán)</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-lg-6">
                                            <label class="form-label">% chiết khấu cho sàn (0 - 100%)</label>
                                            <asp:TextBox ID="txt_phantram_san" runat="server"
                                                CssClass="form-control" Text="0" MaxLength="3"
                                                oninput="clamp_percent_0_100(this)"></asp:TextBox>
                                            <div class="text-muted small mt-1">Áp dụng cho luồng bán sản phẩm của shop công ty.</div>
                                        </div>
                                    </asp:PlaceHolder>

                                    <div class="col-lg-6">
                                        <label class="form-label">Thành phố</label>
                                        <asp:DropDownList ID="DanhSachTP" runat="server" CssClass="form-select"></asp:DropDownList>
                                    </div>

                                    <div class="col-lg-12">
                                        <label class="form-label">Link google map</label>
                                        <asp:TextBox ID="LinkMap" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-12">
                                        <label class="form-label">Mô tả ngắn</label>
                                        <asp:TextBox ID="txt_description" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-12">
                                        <label class="form-label">Nội dung</label>
                                        <CKEditor:CKEditorControl ID="txt_noidung" runat="server" Height="300px" Width="100%" CustomConfig="/ckeditor/config-basic.js"></CKEditor:CKEditorControl>
                                    </div>
                                </div>
                            </div>

                            <div class="card-footer d-flex align-items-center justify-content-between">
                                <a href="#" id="close_add2" runat="server" onserverclick="but_close_form_add_Click"
                                   class="btn btn-link text-decoration-none">Hủy</a>

                                <asp:Button ID="but_add_edit" runat="server" Text="" CssClass="btn btn-success px-4" OnClick="but_add_edit_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="up_add">
        <ProgressTemplate>
            <div class="overlay-loading">
                <div class="spinner-border" role="status" aria-label="loading"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <!-- ===== MAIN LIST ===== -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="container-xl page-shell-wide py-4">

                <!-- toolbar -->
                <div class="toolbar-sticky mb-3">
                    <div class="card shadow-sm">
                        <div class="card-body d-flex align-items-center justify-content-between gap-2 flex-wrap">

                            <div class="d-flex gap-2 flex-wrap">
                                <asp:HyperLink ID="lnk_add_new" runat="server" CssClass="btn btn-primary">
                                    <i class="ti ti-plus me-1"></i> Thêm
                                </asp:HyperLink>

                                <asp:LinkButton ID="LinkButton1" OnClick="LinkButton1_Click" runat="server"
                                    CssClass="btn btn-outline-success">
                                    Đang bán
                                </asp:LinkButton>

                                <asp:LinkButton ID="LinkButton2" OnClick="LinkButton2_Click" runat="server"
                                    CssClass="btn btn-outline-danger">
                                    Ngưng bán
                                </asp:LinkButton>
                            </div>

                            <div class="d-flex align-items-center gap-2 flex-wrap">
                                <span class="text-muted small d-none d-lg-inline">
                                    <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                                    <asp:Literal ID="litPager" runat="server"></asp:Literal>
                                </span>

                                <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server" CssClass="btn btn-outline-secondary btn-icon d-none d-lg-inline-flex" title="Lùi">
                                    <i class="ti ti-chevron-left"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server" CssClass="btn btn-outline-secondary btn-icon d-none d-lg-inline-flex" title="Tới">
                                    <i class="ti ti-chevron-right"></i>
                                </asp:LinkButton>

                                <div class="input-icon">
                                    <span class="input-icon-addon"><i class="ti ti-search"></i></span>
                                    <asp:TextBox MaxLength="50" ID="txt_timkiem" runat="server"
                                        placeholder="Nhập từ khóa"
                                        CssClass="form-control"
                                        AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

                <!-- mobile paging line (giữ controls md) -->
                <div class="d-lg-none d-block mb-3">
                    <div class="d-flex align-items-center justify-content-between">
                        <small class="text-muted">
                            <asp:Label ID="lb_show_md" runat="server" Text=""></asp:Label>
                        </small>
                        <div class="d-flex gap-2">
                            <asp:LinkButton ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" CssClass="btn btn-outline-secondary btn-icon" title="Lùi">
                                <i class="ti ti-chevron-left"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" CssClass="btn btn-outline-secondary btn-icon" title="Tới">
                                <i class="ti ti-chevron-right"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>

                <!-- search fallback textbox (giữ txt_timkiem1 cho logic cũ) -->
                <div class="d-none d-lg-block mb-3">
                    <asp:TextBox MaxLength="50" ID="txt_timkiem1" runat="server"
                        placeholder="Nhập từ khóa"
                        CssClass="form-control"
                        AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                </div>

                <div class="card shadow-sm">
                    <div class="card-body p-0">
                        <div class="table-responsive">
                            <table class="table table-vcenter card-table">
                                <thead>
    <tr>
        <th style="width: 1px;">ID</th>
        <th style="width: 1px;" class="text-center">
            <input type="checkbox" aria-label="Chọn/Bỏ chọn"
                   onkeypress="if (event.keyCode==13) return false;"
                   onclick="document.querySelectorAll('.checkbox-table input[type=checkbox]').forEach(cb=>cb.checked=this.checked);" />
        </th>
        <th style="width: 1px;">Ảnh</th>
        <th class="text-start" style="min-width: 180px;">Tên sản phẩm</th>
        <th class="text-start" style="min-width: 180px;">Mô tả</th>

        <th class="text-end" style="min-width: 120px;">Giá (VNĐ)</th>
        <!-- ✅ NEW -->
        <th class="text-end" style="width: 110px; min-width: 110px;">Ưu đãi (%)</th>
        <th class="text-end" style="width: 140px; min-width: 140px;">Chiết khấu sàn (%)</th>
        <th class="text-center" style="width: 110px; min-width: 110px;">Kênh</th>

        <th class="text-start" style="min-width: 220px;">Danh mục</th>
        <th style="width: 110px; min-width: 110px;">Ngày tạo</th>
        <th class="text-center" style="width: 110px; min-width: 110px;">Cập nhật</th>
    </tr>
</thead>


                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <span style="display: none">
                                                <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                            </span>
                                            <tr>
                                                <td class="text-center"><%# Eval("id") %></td>

                                                <td class="checkbox-table text-center">
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>

                                                <td class="text-center td-img">
                                                    <img src="<%# Eval("image") %>" width="60" height="60" alt="" />
                                                </td>

                                                <td class="text-start">
                                                    <%# Eval("name") %>
                                                    <div class="mt-2">
                                                        <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("bin").ToString()=="True" %>'>
                                                            <span class="badge bg-red-lt">Ngưng bán</span>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("bin").ToString()=="False" %>'>
                                                            <span class="badge bg-green-lt">Đang bán</span>
                                                        </asp:PlaceHolder>
                                                    </div>
                                                </td>

                                                <td class="text-start">
                                                    <%# Eval("description") %>
                                                </td>

                                               <td class="text-end">
    <%# Eval("giaban","{0:#,##0}") %>
</td>

<!-- ✅ NEW: ưu đãi % (null coi như 0) -->
<td class="text-end">
    <%# (Eval("PhanTram_GiamGia_ThanhToan_BangEvoucher") == DBNull.Value || Eval("PhanTram_GiamGia_ThanhToan_BangEvoucher") == null)
        ? "0"
        : Eval("PhanTram_GiamGia_ThanhToan_BangEvoucher").ToString() %>
</td>

<td class="text-end">
    <%# (Eval("PhanTram_ChiTra_ChoSan") == DBNull.Value || Eval("PhanTram_ChiTra_ChoSan") == null)
        ? "0"
        : Eval("PhanTram_ChiTra_ChoSan").ToString() %>
</td>

<td class="text-center">
    <%# ((Eval("KenhRaw") ?? "").ToString().ToLower() == "dichvu")
        ? "<span class='badge bg-indigo-lt'>Dịch vụ</span>"
        : (((Eval("KenhRaw") ?? "").ToString().ToLower() == "sanpham_noibo")
            ? "<span class='badge bg-orange-lt'>Nội bộ</span>"
            : "<span class='badge bg-blue-lt'>Công khai</span>") %>
</td>


                                                <td class="text-start">
                                                    <%# Eval("TenMenu") %><br />
                                                    <%# Eval("TenMenu2") %>
                                                </td>

                                                <td>
                                                    <%# Eval("ngaytao","{0:dd/MM/yyyy}") %>
                                                </td>
                                                <td class="text-center">
                                                    <asp:HyperLink ID="btn_edit" runat="server" CssClass="btn btn-sm btn-outline-primary"
                                                        NavigateUrl='<%# GetEditPostUrl(Eval("id")) %>'>
                                                        Cập nhật
                                                    </asp:HyperLink>
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
            <div class="overlay-loading">
                <div class="spinner-border" role="status" aria-label="loading"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot_sau" runat="Server">
    <script>
        function clamp_percent_0_50(el) {
            // chỉ cho số
            el.value = (el.value || "").replace(/[^\d]/g, "");
            if (el.value === "") return;

            var v = parseInt(el.value, 10);
            if (isNaN(v)) { el.value = ""; return; }
            if (v < 0) v = 0;
            if (v > 50) v = 50; // UI clamp, server vẫn validate & báo
            el.value = v.toString();
        }

        function clamp_percent_0_100(el) {
            el.value = (el.value || "").replace(/[^\d]/g, "");
            if (el.value === "") return;
            var v = parseInt(el.value, 10);
            if (isNaN(v)) { el.value = ""; return; }
            if (v < 0) v = 0;
            if (v > 100) v = 100;
            el.value = v.toString();
        }

        function uploadFile() {
            var fileInput = document.getElementById("fileInput");
            var messageDiv = document.getElementById("message");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath");

            messageDiv.innerHTML = "";
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
                            "<img class='img-fluid rounded' style='max-width:160px' src='" + xhr.responseText + "' />";
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

        function uploadMultipleFiles() {
            var fileInput = document.getElementById("fileListInput");
            var messageDiv = document.getElementById("multiMessage");
            var uploadedListDiv = document.getElementById("uploadedFileList");

            messageDiv.innerHTML = "";
            uploadedListDiv.innerHTML = "";

            var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
            var maxFileSize = 10 * 1024 * 1024;

            if (fileInput.files.length === 0) {
                messageDiv.innerHTML = "Vui lòng chọn ít nhất một file.";
                return;
            }

            for (let i = 0; i < fileInput.files.length; i++) {
                let file = fileInput.files[i];
                let fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();

                if (!allowedExtensions.includes(fileExtension)) {
                    messageDiv.innerHTML += `<div>File "${file.name}" có định dạng không hợp lệ.</div>`;
                    continue;
                }

                if (file.size > maxFileSize) {
                    messageDiv.innerHTML += `<div>File "${file.name}" vượt quá dung lượng cho phép.</div>`;
                    continue;
                }

                let formData = new FormData();
                formData.append("file", file);

                let xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        uploadedListDiv.innerHTML +=
                            "<div style='display:inline-block;margin:6px'>" +
                            "<img class='rounded' style='width:100px;height:100px;object-fit:cover;border:1px solid rgba(98,105,118,.18)' src='" + xhr.responseText + "' />" +
                            "</div>";
                        document.getElementById('<%= hf_anhphu.ClientID %>').value += xhr.responseText + "|";
                    } else {
                        messageDiv.innerHTML += `<div>Lỗi upload ảnh "${file.name}".</div>`;
                    }
                };
                xhr.send(formData);
            }
        }

        function isBdsCategorySelected() {
            var ddl = document.getElementById("<%= ddl_DanhMuc.ClientID %>");
            var hidden = document.getElementById("<%= hf_bds_category_ids.ClientID %>");
            if (!ddl) return false;

            var selectedValue = ddl.value || "";
            var selectedText = "";
            if (ddl.selectedIndex >= 0 && ddl.options[ddl.selectedIndex]) {
                selectedText = (ddl.options[ddl.selectedIndex].text || "").toLowerCase();
            }

            var knownIds = (hidden && hidden.value ? hidden.value.split(",") : []).map(function (x) { return (x || "").trim(); }).filter(Boolean);
            if (knownIds.indexOf(selectedValue) >= 0) return true;

            return selectedText.indexOf("bất động sản") >= 0
                || selectedText.indexOf("bat dong san") >= 0
                || selectedText.indexOf("nhà đất") >= 0
                || selectedText.indexOf("nha dat") >= 0;
        }

        function syncBdsPostingPanel() {
            var panel = document.getElementById("bdsPostingPanel");
            if (!panel) return;
            var isVisible = isBdsCategorySelected();
            panel.style.display = isVisible ? "" : "none";
            if (!isVisible) return;

            var purpose = document.getElementById("<%= ddl_bds_purpose.ClientID %>");
            var propertyType = document.getElementById("<%= ddl_bds_property_type.ClientID %>");
            var furnishing = document.getElementById("<%= ddl_bds_furnishing.ClientID %>");
            var bedrooms = document.getElementById("<%= txt_bds_bedrooms.ClientID %>");
            var bathrooms = document.getElementById("<%= txt_bds_bathrooms.ClientID %>");
            var wrapRentDeposit = document.getElementById("bdsRentDepositWrap");
            var wrapRentTerm = document.getElementById("bdsRentTermWrap");
            var wrapHouseFloor = document.getElementById("bdsHouseFloorWrap");
            var wrapLandWidth = document.getElementById("bdsLandWidthWrap");
            var wrapLandLength = document.getElementById("bdsLandLengthWrap");
            var purposeValue = purpose ? (purpose.value || "") : "";
            var propertyTypeValue = propertyType ? (propertyType.value || "") : "";
            var isRent = purposeValue === "rent";
            var isLand = propertyTypeValue === "land";
            var isHouse = propertyTypeValue === "house";
            var noBedroomTypes = ["land", "office", "business-premises"];

            if (wrapRentDeposit) wrapRentDeposit.style.display = isRent ? "" : "none";
            if (wrapRentTerm) wrapRentTerm.style.display = isRent ? "" : "none";
            if (wrapHouseFloor) wrapHouseFloor.style.display = isHouse ? "" : "none";
            if (wrapLandWidth) wrapLandWidth.style.display = isLand ? "" : "none";
            if (wrapLandLength) wrapLandLength.style.display = isLand ? "" : "none";

            if (furnishing && isLand) furnishing.value = "Chưa cập nhật";
            if (bedrooms && noBedroomTypes.indexOf(propertyTypeValue) >= 0) bedrooms.value = "0";
            if (bathrooms && propertyTypeValue === "land") bathrooms.value = "0";
        }

        function bootBdsPostingPanel() {
            var ddl = document.getElementById("<%= ddl_DanhMuc.ClientID %>");
            var purpose = document.getElementById("<%= ddl_bds_purpose.ClientID %>");
            var propertyType = document.getElementById("<%= ddl_bds_property_type.ClientID %>");
            if (ddl) ddl.addEventListener("change", syncBdsPostingPanel);
            if (purpose) purpose.addEventListener("change", syncBdsPostingPanel);
            if (propertyType) propertyType.addEventListener("change", syncBdsPostingPanel);
            syncBdsPostingPanel();
        }

        document.addEventListener("DOMContentLoaded", bootBdsPostingPanel);
        if (window.Sys && Sys.WebForms) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                bootBdsPostingPanel();
            });
        }
    </script>
</asp:Content>
