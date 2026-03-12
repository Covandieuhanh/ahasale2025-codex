<%@ Page Title="Đơn mua" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="don-mua.aspx.cs" Inherits="home_don_mua" %>

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
        .w-240{ width:240px; }
        .money{ white-space:nowrap; }
        .addr{ min-width: 180px; }
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
    </style>
    <script>
        function confirmCancelOrder(orderId) {
            var msg = "Bạn có chắc chắn muốn huỷ đơn không ?";
            if (!orderId) return false;
            if (window.show_modal_2btn) {
                show_modal_2btn(
                    msg,
                    "Thông báo",
                    true,
                    "warning",
                    "Xác nhận",
                    "/home/don-mua.aspx?cancel=1&id=" + encodeURIComponent(orderId),
                    "Không",
                    ""
                );
                return false;
            }
            return confirm(msg);
        }
    </script>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">

    <!-- ===================== MAIN ===================== -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="page-header d-print-none">
                <div class="container-xl" >
                    <div class="row g-2 align-items-center">
                        <div class="col">
                            <div class="page-pretitle">Đơn hàng</div>
                            <h2 class="page-title">Đơn mua</h2>
                        </div>

                        <div class="col-auto ms-auto d-print-none">
                            <div class="text-muted small">
                                <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                                <asp:Label ID="lb_show_md" runat="server" Text="" CssClass="d-none"></asp:Label>
                            </div>
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
	                            <div class="btn-list">
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
                <div class="container-xl" >

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
                                        <th style="min-width:160px;">Tên Shop</th>
                                    
                                        <th class="text-end" style="min-width:120px;">Trao đổi (A)</th>
                                        <th style="min-width:140px;">Trạng thái</th>
                                        <th style="min-width:190px;">Thao tác</th>
                                        <th style="min-width:170px;">Người nhận</th>
                                        <th class="addr">Địa chỉ</th>
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
                                                    <div class="fw-semibold"><%# Eval("TenShop") %></div>
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
                                                        <span class="badge bg-red-lt text-red">Chờ Trao đổi</span>
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
                                                                OnClientClick='<%# "return confirmCancelOrder(" + Eval("id") + ");" %>'
                                                                CommandArgument='<%# Eval("id") %>'
                                                                CssClass="btn btn-outline-danger btn-sm">
                                                                Hủy đơn
                                                            </asp:LinkButton>
                                                        </asp:PlaceHolder>

                                                        <asp:PlaceHolder ID="ph_nhan_row" runat="server" Visible='<%# Convert.ToBoolean(Eval("show_danhan")) %>'>
                                                            <asp:LinkButton ID="but_danhanhang_row" runat="server"
                                                                OnClick="but_danhanhang_row_Click"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                CssClass="btn btn-primary btn-sm">
                                                                Đã nhận hàng
                                                            </asp:LinkButton>
                                                        </asp:PlaceHolder>
                                                    </div>
                                                </td>

                                                <td>
                                                    <div class="fw-semibold"><%# Eval("hoten_nguoinhan") %></div>
                                                    <div class="text-muted"><%# Eval("sdt_nguoinhan") %></div>
                                                </td>

                                                <td class="text-muted">
                                                    <%# Eval("diahchi_nguoinhan") %>
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
                    <div class="mt-3 text-white">Đang tải...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="ContentFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="Server">
</asp:Content>
