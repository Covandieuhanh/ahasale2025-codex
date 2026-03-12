<%@ Page Title="Đơn bán" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="don-ban.aspx.cs" Inherits="home_don_ban" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .tblr-overlay{
            position:fixed; inset:0; background:rgba(0,0,0,.65);
            z-index:99999; display:flex; align-items:center; justify-content:center;
        }
        .table thead th{ white-space:nowrap; }
        .table td{ vertical-align:middle; }
        .modal-body{ max-height: calc(100vh - 220px); overflow:auto; }
        .img-60{ width:60px; height:60px; object-fit:cover; border-radius:12px; }
        .money{ white-space:nowrap; }
        .addr{ min-width:180px; }
        .qty-input{ width:70px; max-width:70px; }
        .badge-dot{
            display:inline-flex; align-items:center; gap:.4rem;
        }
        .badge-dot:before{
            content:""; width:.5rem; height:.5rem; border-radius:999px; background: currentColor;
            display:inline-block;
        }
        .status-filter-control{
            display:inline-flex;
            align-items:center;
            gap:10px;
            padding:6px 14px;
            border:1px solid var(--tblr-border-color,#e2e8f0);
            border-radius:999px;
            background:var(--tblr-bg-surface,#ffffff);
            color:var(--tblr-body-color,#0f172a);
            box-shadow:0 10px 22px rgba(15,23,42,.08);
            transition:box-shadow .2s ease,border-color .2s ease,transform .2s ease;
        }
        .status-filter-control:hover{
            border-color:rgba(99,102,241,.35);
            box-shadow:0 14px 30px rgba(15,23,42,.12);
            transform:translateY(-1px);
        }
        .status-filter-icon{
            width:28px;
            height:28px;
            border-radius:999px;
            display:inline-flex;
            align-items:center;
            justify-content:center;
            background:var(--tblr-primary-lt,#e0f2fe);
            color:var(--tblr-primary,#0284c7);
            font-size:16px;
        }
        .status-filter-select{
            border:0;
            background:transparent;
            font-weight:600;
            color:inherit;
            padding:4px 32px 4px 6px;
            appearance:none;
            -webkit-appearance:none;
            -moz-appearance:none;
            min-width:200px;
            background-image:url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' fill='none' stroke='%2363748b' stroke-width='2' viewBox='0 0 24 24'%3E%3Cpath d='m6 9 6 6 6-6'/%3E%3C/svg%3E");
            background-repeat:no-repeat;
            background-position:right 8px center;
            background-size:16px;
        }
        .status-filter-select:focus{
            outline:none;
            box-shadow:none;
        }
        .status-filter-badge{
            border-radius:999px;
            padding:6px 10px;
            font-weight:600;
        }
        @media (max-width: 575.98px){
            .status-filter-control{ width:100%; }
            .status-filter-select{ flex:1; min-width:0; }
        }
        .pos-standalone-page{
            padding: 8px 0 18px;
        }
        .pos-standalone-page .modal-dialog{
            margin: 0 auto;
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">

    <!-- ===================== MODAL: TẠO ĐƠN OFFLINE (POS) ===================== -->
    <asp:UpdatePanel ID="up_taodon" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_taodon" runat="server" Visible="false" DefaultButton="but_taodon">

                <div class="<%= GetCreateOrderWrapperCssClass() %>" style="display:block;" tabindex="-1" role="dialog" aria-modal="true">
                    <div class="modal-dialog modal-xl modal-dialog-centered" role="document" style="<%= GetCreateOrderDialogStyle() %>">
                        <div class="modal-content">

                            <div class="modal-header">
                                <h5 class="modal-title">Tạo đơn Offline (POS)</h5>
                                <a href="#" class="btn-close" aria-label="Close" id="A1" runat="server" onserverclick="but_close_form_taodon_Click"></a>
                            </div>

                            <div class="modal-body">

                                <div class="row g-3">

                                    <!-- LEFT: SEARCH + RESULTS -->
                                    <div class="col-lg-6">
                                        <div class="card">
                                            <div class="card-header">
                                                <div class="w-100">
                                                    <div class="fw-semibold mb-2">Tìm sản phẩm</div>

                                                    <asp:TextBox ID="txt_sp_search" runat="server"
                                                        CssClass="form-control"
                                                        placeholder="Nhập tên sản phẩm..."
                                                        AutoPostBack="true"
                                                        OnTextChanged="txt_sp_search_TextChanged"
                                                        MaxLength="100"></asp:TextBox>

                                                    <div class="text-muted small mt-2">
                                                        <asp:Label ID="lb_sp_search_note" runat="server" Text="Gõ tên để tìm, bấm Thêm để đưa vào giỏ."></asp:Label>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="table-responsive">
                                                <table class="table card-table table-vcenter">
                                                    <thead>
                                                        <tr>
                                                            <th style="width:90px;">Ảnh</th>
                                                            <th style="min-width:220px;">Tên sản phẩm</th>
                                                            <th class="text-end" style="min-width:120px;">Giá (VNĐ)</th>
                                                            <th class="text-end" style="min-width:90px;">Trao đổi (A)</th>
                                                            <th class="text-center" style="width:90px;">Thêm</th>
                                                        </tr>
                                                    </thead>

                                                    <tbody>
                                                        <asp:Repeater ID="RepeaterSearch" runat="server" OnItemCommand="RepeaterSearch_ItemCommand">
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td class="text-center">
                                                                        <a href="<%# Eval("image") %>" target="_blank" class="d-inline-block">
                                                                            <img src="<%# Eval("image") %>" class="img-60" />
                                                                        </a>
                                                                    </td>

                                                                    <td>
                                                                        <div class="fw-semibold"><%# Eval("name") %></div>
                                                                        <div class="text-muted small">ID: <%# Eval("id") %></div>
                                                                    </td>

                                                                    <td class="text-end money">
                                                                        <%# Eval("giaban","{0:#,##0}") %> ₫
                                                                    </td>

                                                                    <td class="text-end money">
                                                                        <%# (Convert.ToDecimal(Eval("giaban")) / 1000m).ToString("0.00") %> A
                                                                    </td>

                                                                    <td class="text-center">
                                                                        <asp:LinkButton ID="but_add" runat="server"
                                                                            CommandName="add"
                                                                            CommandArgument='<%# Eval("id") %>'
                                                                            CssClass="btn btn-primary btn-sm">
                                                                            Thêm
                                                                        </asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>

                                                        <asp:PlaceHolder ID="ph_no_search" runat="server" Visible="false">
                                                            <tr>
                                                                <td colspan="5" class="text-center text-muted py-4">Không có sản phẩm phù hợp.</td>
                                                            </tr>
                                                        </asp:PlaceHolder>
                                                    </tbody>

                                                </table>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- RIGHT: CART -->
                                    <div class="col-lg-6">
                                        <div class="card">
                                            <div class="card-header">
                                                <div class="d-flex align-items-center w-100">
                                                    <div>
                                                        <div class="fw-semibold">Giỏ hàng</div>
                                                        <div class="text-muted small">Cộng dồn SL khi thêm trùng. Có thể sửa SL và % ưu đãi.</div>
                                                    </div>
                                                    <div class="ms-auto">
                                                        <asp:LinkButton ID="but_clear_cart" runat="server" CssClass="btn btn-outline-danger btn-sm"
                                                            OnClick="but_clear_cart_Click"
                                                            OnClientClick="return confirm('Xóa toàn bộ giỏ hàng?');">
                                                            Xóa giỏ
                                                        </asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="table-responsive">
                                                <table class="table card-table table-vcenter">
                                                    <thead>
                                                        <tr>
                                                            <th style="width:90px;">Ảnh</th>
                                                            <th style="min-width:220px;">Tên</th>
                                                            <th class="text-end" style="min-width:110px;">Giá</th>
                                                            <th class="text-center" style="min-width:170px;">Số lượng</th>
                                                            <th class="text-center" style="min-width:120px;">Ưu đãi (%)</th>
                                                            <th class="text-end" style="min-width:130px;">Thành tiền</th>
                                                            <th style="width:1px;"></th>
                                                        </tr>
                                                    </thead>

                                                    <tbody>
                                                        <asp:Repeater ID="RepeaterCart" runat="server" OnItemCommand="RepeaterCart_ItemCommand">
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td class="text-center">
                                                                        <a href="<%# Eval("Image") %>" target="_blank" class="d-inline-block">
                                                                            <img src="<%# Eval("Image") %>" class="img-60" />
                                                                        </a>
                                                                    </td>

                                                                    <td>
                                                                        <div class="fw-semibold"><%# Eval("Name") %></div>
                                                                        <div class="text-muted small">ID: <%# Eval("ProductId") %></div>
                                                                    </td>

                                                                    <td class="text-end money">
                                                                      <%# Convert.ToDecimal(Eval("GiaBan")).ToString("#,##0") %> ₫
                                                                    </td>

                                                                    <td class="text-center">
                                                                        <div class="btn-group" role="group" aria-label="qty">
                                                                            <asp:LinkButton ID="but_minus" runat="server" CssClass="btn btn-outline-secondary btn-sm"
                                                                                CommandName="minus" CommandArgument='<%# Eval("ProductId") %>'>-</asp:LinkButton>

                                                                            <asp:TextBox ID="txt_qty" runat="server"
                                                                                Text='<%# Eval("SoLuong") %>'
                                                                                CssClass="form-control form-control-sm qty-input text-center"
                                                                                MaxLength="3"
                                                                                Attributes="type:number; min:0; max:999; inputmode:numeric"></asp:TextBox>

                                                                            <asp:LinkButton ID="but_plus" runat="server" CssClass="btn btn-outline-secondary btn-sm"
                                                                                CommandName="plus" CommandArgument='<%# Eval("ProductId") %>'>+</asp:LinkButton>
                                                                        </div>

                                                                        <div class="mt-2">
                                                                            <asp:LinkButton ID="but_update_qty" runat="server" CssClass="btn btn-outline-primary btn-sm"
                                                                                CommandName="updateqty" CommandArgument='<%# Eval("ProductId") %>'>
                                                                                Cập nhật
                                                                            </asp:LinkButton>
                                                                        </div>
                                                                    </td>

                                                                    <td class="text-center">
                                                                        <asp:TextBox ID="txt_discount" runat="server"
                                                                            Text='<%# Eval("PhanTramGiamGia") %>'
                                                                            CssClass="form-control form-control-sm text-center"
                                                                            MaxLength="2"
                                                                            Attributes="type:number; min:0; max:50; inputmode:numeric"></asp:TextBox>

                                                                        <div class="mt-2">
                                                                            <asp:LinkButton ID="but_update_discount" runat="server" CssClass="btn btn-outline-primary btn-sm"
                                                                                CommandName="updatediscount" CommandArgument='<%# Eval("ProductId") %>'>
                                                                                Lưu %
                                                                            </asp:LinkButton>
                                                                        </div>
                                                                    </td>

                                                                    <td class="text-end money">
                                                                      <%# (Convert.ToDecimal(Eval("GiaBan")) * Convert.ToInt32(Eval("SoLuong"))).ToString("#,##0") %> ₫
                                                                    </td>

                                                                    <td class="text-end">
                                                                        <asp:LinkButton ID="but_remove" runat="server"
                                                                            CommandName="remove" CommandArgument='<%# Eval("ProductId") %>'
                                                                            CssClass="btn btn-outline-danger btn-sm"
                                                                            OnClientClick="return confirm('Xóa sản phẩm này khỏi giỏ?');">
                                                                            Xóa
                                                                        </asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>

                                                        <asp:PlaceHolder ID="ph_no_cart" runat="server" Visible="false">
                                                            <tr>
                                                                <td colspan="7" class="text-center text-muted py-4">Giỏ hàng đang trống.</td>
                                                            </tr>
                                                        </asp:PlaceHolder>
                                                    </tbody>

                                                </table>
                                            </div>

                                            <div class="card-footer">
                                                <div class="d-flex align-items-center">
                                                    <div class="text-muted">
                                                        Tổng:
                                                        <span class="fw-semibold money"><asp:Label ID="lb_cart_total_vnd" runat="server" Text="0"></asp:Label> ₫</span>
                                                        &nbsp;|&nbsp;
                                                        <span class="fw-semibold money"><asp:Label ID="lb_cart_total_a" runat="server" Text="0.00"></asp:Label> A</span>
                                                    </div>
                                                    <div class="ms-auto">
                                                        <asp:Label ID="lb_cart_err" runat="server" CssClass="text-danger small" Text=""></asp:Label>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                </div>

                            </div>

                            <div class="modal-footer">
                                <asp:Button ID="but_taodon" OnClick="but_taodon_Click" runat="server"
                                    Text="TẠO ĐƠN" CssClass="btn btn-primary" />
                            </div>

                        </div>
                    </div>
                </div>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_taodon">
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
                            <div class="page-pretitle">Bán hàng</div>
                            <h2 class="page-title">Đơn bán</h2>
                        </div>
                        <div class="col-auto ms-auto d-print-none">
                            <asp:LinkButton ID="but_show_form_taodon" OnClick="but_show_form_taodon_Click" runat="server" CssClass="btn btn-primary btn-sm">
                                <i class="ti ti-plus"></i>&nbsp;Tạo đơn
                            </asp:LinkButton>
                        </div>
                    </div>

                    <div class="row g-2 align-items-center mt-3">
                        <div class="col-md-6">
                            <asp:TextBox MaxLength="50" ID="txt_timkiem1" runat="server"
                                placeholder="Tìm theo Shop / Người mua / ID..."
                                CssClass="form-control"
                                AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                        </div>
                        <div class="col-md-6" style="display:none">
                            <asp:TextBox MaxLength="50" ID="txt_timkiem" runat="server"
                                placeholder="Tìm nhanh..."
                                CssClass="form-control"
                                AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                        </div>

                        <div class="col-12">
                            <div class="d-flex align-items-center gap-2 flex-wrap">
                                <div class="text-muted small">
                                    <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                                    <asp:Label ID="lb_show_md" runat="server" Text="" CssClass="d-none"></asp:Label>
                                </div>

                                <div class="ms-auto btn-list">
                                    <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                                        <i class="ti ti-chevron-left"></i>&nbsp;Lùi
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                                        Tới&nbsp;<i class="ti ti-chevron-right"></i>
                                    </asp:LinkButton>

                                    <!-- giữ lại bản mobile cũ để không đụng code -->
                                    <div class="d-none">
                                        <asp:LinkButton ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" />
                                        <asp:LinkButton ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-12">
                            <div class="d-flex align-items-center gap-2 flex-wrap">
                                <div class="text-muted small">Lọc trạng thái:</div>
                                <div class="status-filter-control">
                                    <span class="status-filter-icon"><i class="ti ti-map-pin"></i></span>
                                    <asp:DropDownList ID="ddl_status_filter" runat="server"
                                        CssClass="status-filter-select"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="ddl_status_filter_SelectedIndexChanged">
                                        <asp:ListItem Text="Tất cả trạng thái" Value="all"></asp:ListItem>
                                        <asp:ListItem Text="Đã đặt/Chưa Trao đổi" Value="da-dat"></asp:ListItem>
                                        <asp:ListItem Text="Chờ Trao đổi" Value="cho-trao-doi"></asp:ListItem>
                                        <asp:ListItem Text="Đã Trao đổi" Value="da-trao-doi"></asp:ListItem>
                                        <asp:ListItem Text="Đã giao" Value="da-giao"></asp:ListItem>
                                        <asp:ListItem Text="Đã nhận" Value="da-nhan"></asp:ListItem>
                                        <asp:ListItem Text="Đã hủy" Value="da-huy"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <span class="badge bg-azure-lt text-azure status-filter-badge">
                                    Đang chọn: <asp:Label ID="lb_status_filter" runat="server" Text="Tất cả trạng thái"></asp:Label>
                                </span>
                            </div>
                        </div>

                    </div>

                </div>
            </div>

            <div class="page-body">
                <div class="container-xl">

                    <div class="card">
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
                                        <th style="min-width:160px;">Ngày</th>
                                        <th style="min-width:90px;">Loại</th>
                                        <th style="min-width:180px;">Người nhận</th>
                                        <th class="addr">Địa chỉ</th>
                                        <th class="text-end" style="min-width:140px;">Trao đổi (VNĐ)</th>
                                        <th class="text-end" style="min-width:120px;">Trao đổi (A)</th>
                                        <th style="min-width:220px;">Trạng thái</th>
                                        <th style="min-width:220px;">Thao tác</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <span style="display:none">
                                                <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                            </span>

                                            <tr>
                                                <td class="text-center"><%# Eval("id") %></td>

                                                <td class="checkbox-table text-center">
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>

                                                <td>
                                                    <div class="fw-semibold"><%#Eval("ngaydat","{0:dd/MM/yyyy}") %></div>
                                                    <div class="mt-2">
                                                        <asp:HyperLink ID="hl_chitiet" runat="server"
                                                            NavigateUrl='<%# BuildOrderDetailUrl(Eval("id")) %>'
                                                            CssClass="btn btn-outline-secondary btn-sm">
                                                            Chi tiết
                                                        </asp:HyperLink>
                                                    </div>
                                                </td>

                                                <td>
                                                    <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("online_offline").ToString()=="True" %>'>
                                                        <span class="badge bg-yellow-lt text-yellow">Online</span>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("online_offline").ToString()=="False" %>'>
                                                        <span class="badge bg-blue-lt text-blue">Offline</span>
                                                    </asp:PlaceHolder>
                                                </td>

                                                <td>
                                                    <div class="fw-semibold"><%# Eval("hoten_nguoinhan") %></div>
                                                    <div class="text-muted"><%# Eval("sdt_nguoinhan") %></div>
                                                </td>

                                                <td class="text-muted">
                                                    <%# Eval("diahchi_nguoinhan") %>
                                                </td>

                                                <td class="text-end money">
                                                    <%#Eval("tongtien","{0:#,##0}") %> ₫
                                                </td>

                                                <td class="text-end money">
                                                    <%# (Convert.ToDecimal(Eval("tongtien")) / 1000m).ToString("0.00") %> A
                                                </td>

                                                <td>
                                                    <asp:PlaceHolder ID="PlaceHolder19" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã đặt" %>'>
                                                        <span class="badge bg-azure-lt text-azure">Đã đặt</span>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã hủy" %>'>
                                                        <span class="badge bg-red-lt text-red">Đã hủy</span>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã giao" %>'>
                                                        <span class="badge bg-yellow-lt text-yellow">Đã giao</span>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã nhận" %>'>
                                                        <span class="badge bg-green-lt text-green">Đã nhận</span>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("trangthai").ToString()=="Chưa Trao đổi" %>'>
                                                        <span class="badge bg-orange-lt text-orange">Chưa Trao đổi</span>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#Eval("trangthai").ToString()=="Chờ Trao đổi" %>'>
                                                        <span class="badge bg-red-lt text-red badge-dot">Chờ Trao đổi</span>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã Trao đổi" %>'>
                                                        <span class="badge bg-blue-lt text-blue">Đã Trao đổi</span>
                                                    </asp:PlaceHolder>
                                                </td>

                                                <td>
                                                    <div class="btn-list">
                                                        <asp:PlaceHolder ID="ph_huy_row" runat="server" Visible='<%# Convert.ToBoolean(Eval("show_huydon")) %>'>
                                                            <asp:LinkButton ID="but_huydonhang_row" runat="server"
                                                                OnClick="but_huydonhang_row_Click"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                CssClass="btn btn-outline-danger btn-sm">
                                                                Hủy đơn
                                                            </asp:LinkButton>
                                                        </asp:PlaceHolder>

                                                        <asp:PlaceHolder ID="ph_dagiao_row" runat="server" Visible='<%# Convert.ToBoolean(Eval("show_dagiaohang")) %>'>
                                                            <asp:LinkButton ID="but_dagiaohang_row" runat="server"
                                                                OnClick="but_dagiaohang_row_Click"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                CssClass="btn btn-primary btn-sm">
                                                                Đã giao
                                                            </asp:LinkButton>
                                                        </asp:PlaceHolder>

                                                        <asp:PlaceHolder ID="ph_chotraodoi_row" runat="server" Visible='<%# Convert.ToBoolean(Eval("show_chothanhtoan")) %>'>
                                                            <asp:LinkButton ID="but_chothanhtoan" runat="server"
                                                                OnClick="but_chothanhtoan_Click"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                CssClass="btn btn-outline-danger btn-sm">
                                                                Kích hoạt chờ Trao đổi
                                                            </asp:LinkButton>
                                                        </asp:PlaceHolder>

                                                        <asp:PlaceHolder ID="ph_huychotraodoi_row" runat="server" Visible='<%# Convert.ToBoolean(Eval("show_huychothanhtoan")) %>'>
                                                            <asp:LinkButton ID="but_huychothanhtoan" runat="server"
                                                                OnClick="but_huychothanhtoan_Click"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                CssClass="btn btn-outline-warning btn-sm">
                                                                Hủy chờ Trao đổi
                                                            </asp:LinkButton>
                                                        </asp:PlaceHolder>
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
            <div class="tblr-overlay">
                <div class="text-center">
                    <div class="spinner-border" role="status"></div>
                    <div class="mt-3 text-white">Đang xử lý...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="ContentFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="Server">
</asp:Content>
