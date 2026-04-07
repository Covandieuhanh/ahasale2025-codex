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
        .desktop-status-tabs{
            display:flex;
            flex-wrap:wrap;
            gap:8px;
            margin-top:12px;
        }
        .status-tab{
            display:inline-flex;
            align-items:center;
            gap:8px;
            padding:8px 12px;
            border:1px solid var(--tblr-border-color,#e2e8f0);
            border-radius:999px;
            color:#334155;
            text-decoration:none;
            background:#fff;
            font-weight:600;
            transition:all .18s ease;
        }
        .status-tab:hover{
            border-color:#94a3b8;
            color:#0f172a;
            transform:translateY(-1px);
        }
        .status-tab.active{
            background:#0f172a;
            border-color:#0f172a;
            color:#fff;
            box-shadow:0 10px 24px rgba(15,23,42,.22);
        }
        .status-tab-count{
            min-width:22px;
            height:22px;
            padding:0 7px;
            border-radius:999px;
            display:inline-flex;
            align-items:center;
            justify-content:center;
            background:#f1f5f9;
            color:#334155;
            font-size:12px;
            font-weight:700;
        }
        .status-tab.active .status-tab-count{
            background:rgba(255,255,255,.18);
            color:#fff;
        }

        .order-drawer-backdrop{
            position:fixed;
            inset:0;
            background:rgba(15,23,42,.45);
            z-index:1045;
            display:none;
        }
        .order-drawer{
            position:fixed;
            top:0;
            right:-720px;
            width:min(720px, 96vw);
            height:100vh;
            background:#fff;
            z-index:1050;
            box-shadow:-18px 0 44px rgba(15,23,42,.18);
            transition:right .22s ease;
            display:flex;
            flex-direction:column;
        }
        .order-drawer.show{ right:0; }
        .order-drawer-backdrop.show{ display:block; }
        .order-drawer-head{
            padding:12px 14px;
            border-bottom:1px solid #e2e8f0;
            display:flex;
            align-items:center;
            justify-content:space-between;
            gap:10px;
        }
        .order-drawer-title{
            font-weight:700;
            font-size:16px;
            margin:0;
        }
        .order-drawer-frame{
            border:0;
            width:100%;
            height:100%;
            background:#f8fafc;
        }

        @media (min-width: 768px){
            .page-header .row.g-2.align-items-center.mt-3{
                position:sticky;
                top:70px;
                z-index:20;
                background:#fff;
                border:1px solid #e2e8f0;
                border-radius:14px;
                padding:12px;
                margin-top:14px !important;
                box-shadow:0 12px 26px rgba(15,23,42,.06);
            }
        }
        @media (max-width: 767.98px){
            .status-filter-control{ width:100%; }
            .status-filter-select{ flex:1; min-width:0; }
            .desktop-status-tabs{ display:none; }
        }

        .orders-mobile {
            display: none;
        }

        .order-card {
            border: 1px solid var(--tblr-border-color,#e2e8f0);
            border-radius: 16px;
            background: #fff;
            padding: 12px;
            box-shadow: 0 10px 24px rgba(15,23,42,.06);
            margin-bottom: 14px;
        }
        .order-card-head {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            font-weight: 700;
            margin-bottom: 8px;
        }
        .order-card-status {
            font-size: 12px;
            font-weight: 700;
            color: #2563eb;
        }
        .order-card-status .badge {
            font-size: 12px;
            font-weight: 700;
        }
        .order-card-item {
            display: grid;
            grid-template-columns: 64px 1fr;
            gap: 12px;
            padding: 8px;
            border-radius: 12px;
            background: #f8fafc;
        }
        .order-card-item img {
            width: 64px;
            height: 64px;
            border-radius: 12px;
            object-fit: cover;
        }
        .order-card-item-name {
            font-weight: 700;
            font-size: 14px;
            line-height: 1.2;
        }
        .order-card-item-meta {
            font-size: 12px;
            color: #64748b;
            margin-top: 4px;
        }
        .order-card-total {
            display: flex;
            justify-content: flex-end;
            font-weight: 700;
            margin-top: 8px;
        }
        .order-card-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            margin-top: 10px;
        }
        .order-card-actions .btn {
            flex: 1 1 auto;
            min-height: 34px;
            border-radius: 999px;
            font-weight: 700;
        }

        .mobile-toolbar {
            display: none;
            gap: 10px;
            align-items: center;
            margin-bottom: 12px;
        }
        .mobile-page-title{
            display:none;
            margin-bottom:10px;
        }
        .mobile-page-title .mobile-pretitle{
            font-size:12px;
            color:#64748b;
            font-weight:600;
            text-transform:uppercase;
            letter-spacing:.02em;
            margin-bottom:2px;
        }
        .mobile-page-title .mobile-title{
            font-size:24px;
            line-height:1.2;
            font-weight:800;
            color:#0f172a;
            margin:0;
        }

        .mobile-search {
            flex: 1 1 auto;
            display: flex;
            align-items: center;
            gap: 8px;
            padding: 8px 12px;
            border-radius: 999px;
            background: #f1f5f9;
            border: 1px solid #e2e8f0;
            min-height: 40px;
        }

        .mobile-search input {
            border: 0;
            background: transparent;
            width: 100%;
            font-size: 14px;
        }

        .mobile-search input:focus { outline: none; }

        .mobile-tabs {
            display: none;
            gap: 18px;
            overflow-x: auto;
            padding-bottom: 6px;
            margin-bottom: 12px;
            scrollbar-width: none;
        }
        .mobile-tabs::-webkit-scrollbar{ display:none; }

        .mobile-tab {
            white-space: nowrap;
            font-weight: 700;
            color: #64748b;
            position: relative;
            padding-bottom: 6px;
        }

        .mobile-tab.active {
            color: #0f172a;
        }

        .mobile-tab.active::after {
            content: "";
            position: absolute;
            left: 0;
            right: 0;
            bottom: 0;
            height: 3px;
            border-radius: 999px;
            background: #111827;
        }

        @media (max-width: 767.98px){
            .table-responsive { display: none; }
            .orders-mobile { display: block; }
            .mobile-page-title { display:block; }
            .mobile-toolbar { display: flex; }
            .mobile-tabs { display: flex; }
            .desktop-status-tabs { display: none; }
            .page-header .row.g-2.align-items-center { display: none; }
            .page-header .row.g-2.align-items-center.mt-3 { display: none; }
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

        function showReviewUpgradeNotice() {
            var msg = "Tính năng này đang trong quá trình nâng cấp.";
            if (window.show_modal) {
                show_modal(msg, "Thông báo", true, "info");
                return false;
            }
            alert(msg);
            return false;
        }

        function markOrderActionBusy(btn, text) {
            if (!btn) return true;
            if (btn.getAttribute('data-busy') === '1') return false;
            btn.setAttribute('data-busy', '1');
            btn.setAttribute('disabled', 'disabled');
            if (text) {
                btn.setAttribute('data-origin-text', btn.innerHTML);
                btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>' + text;
            }
            return true;
        }

        function openOrderDetailDrawer(url) {
            if (!url) return false;
            var mq = window.matchMedia ? window.matchMedia('(max-width: 767.98px)') : null;
            if (mq && mq.matches) return true;

            var drawer = document.getElementById('order_detail_drawer');
            var backdrop = document.getElementById('order_detail_backdrop');
            var frame = document.getElementById('order_detail_frame');
            if (!drawer || !backdrop || !frame) return true;

            frame.src = url;
            drawer.classList.add('show');
            backdrop.classList.add('show');
            document.body.style.overflow = 'hidden';
            return false;
        }

        function closeOrderDetailDrawer() {
            var drawer = document.getElementById('order_detail_drawer');
            var backdrop = document.getElementById('order_detail_backdrop');
            var frame = document.getElementById('order_detail_frame');
            if (drawer) drawer.classList.remove('show');
            if (backdrop) backdrop.classList.remove('show');
            if (frame) frame.src = 'about:blank';
            document.body.style.overflow = '';
            return false;
        }

        function goMobileSearch() {
            var input = document.getElementById('txt_search_mobile');
            if (!input) return;
            var q = (input.value || '').trim();
            var status = input.getAttribute('data-status') || 'all';
            var qs = '?status=' + encodeURIComponent(status);
            if (q.length > 0) qs += '&q=' + encodeURIComponent(q);
            window.location.href = '/home/don-mua.aspx' + qs;
        }

        document.addEventListener('DOMContentLoaded', function () {
            var input = document.getElementById('txt_search_mobile');
            if (input) {
                input.addEventListener('keypress', function (e) {
                    if (e.key === 'Enter') {
                        e.preventDefault();
                        goMobileSearch();
                    }
                });
            }

            document.addEventListener('keydown', function (e) {
                if (e.key === 'Escape') closeOrderDetailDrawer();
            });
        });
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
                                    <asp:Literal ID="litPager" runat="server"></asp:Literal>
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

                            <div class="col-12">
                                <div class="desktop-status-tabs">
                                    <a href="<%= BuildStatusTabUrl("all") %>" class="<%= GetDesktopTabClass("all") %>">
                                        Tất cả <span class="status-tab-count"><%= GetStatusTabCount("all") %></span>
                                    </a>
                                    <a href="<%= BuildStatusTabUrl("da-dat") %>" class="<%= GetDesktopTabClass("da-dat") %>">
                                        Đã đặt <span class="status-tab-count"><%= GetStatusTabCount("da-dat") %></span>
                                    </a>
                                    <a href="<%= BuildStatusTabUrl("cho-trao-doi") %>" class="<%= GetDesktopTabClass("cho-trao-doi") %>">
                                        Chờ trao đổi <span class="status-tab-count"><%= GetStatusTabCount("cho-trao-doi") %></span>
                                    </a>
                                    <a href="<%= BuildStatusTabUrl("da-trao-doi") %>" class="<%= GetDesktopTabClass("da-trao-doi") %>">
                                        Đã trao đổi <span class="status-tab-count"><%= GetStatusTabCount("da-trao-doi") %></span>
                                    </a>
                                    <a href="<%= BuildStatusTabUrl("da-giao") %>" class="<%= GetDesktopTabClass("da-giao") %>">
                                        Đã giao <span class="status-tab-count"><%= GetStatusTabCount("da-giao") %></span>
                                    </a>
                                    <a href="<%= BuildStatusTabUrl("da-nhan") %>" class="<%= GetDesktopTabClass("da-nhan") %>">
                                        Đã nhận <span class="status-tab-count"><%= GetStatusTabCount("da-nhan") %></span>
                                    </a>
                                    <a href="<%= BuildStatusTabUrl("da-huy") %>" class="<%= GetDesktopTabClass("da-huy") %>">
                                        Đã hủy <span class="status-tab-count"><%= GetStatusTabCount("da-huy") %></span>
                                    </a>
                                </div>
                            </div>
	                    </div>

                </div>
            </div>

            <div class="page-body">
                <div class="container-xl" >

                    <div class="mobile-page-title">
                        <div class="mobile-pretitle">Đơn hàng</div>
                        <h2 class="mobile-title">Đơn mua</h2>
                    </div>

                    <div class="mobile-toolbar">
                        <div class="mobile-search">
                            <i class="ti ti-search text-muted"></i>
                            <input id="txt_search_mobile" type="text"
                                placeholder="Tìm kiếm đơn hàng của bạn"
                                value="<%= GetCurrentSearchQuery() %>"
                                data-status="<%= GetCurrentStatusFilter() %>" />
                        </div>
                    </div>

                    <div class="mobile-tabs">
                        <a href="<%= BuildStatusTabUrl("all") %>" class="<%= GetMobileTabClass("all") %>">Tất cả</a>
                        <a href="<%= BuildStatusTabUrl("da-dat") %>" class="<%= GetMobileTabClass("da-dat") %>">Đã đặt</a>
                        <a href="<%= BuildStatusTabUrl("cho-trao-doi") %>" class="<%= GetMobileTabClass("cho-trao-doi") %>">Chờ trao đổi</a>
                        <a href="<%= BuildStatusTabUrl("da-trao-doi") %>" class="<%= GetMobileTabClass("da-trao-doi") %>">Đã trao đổi</a>
                        <a href="<%= BuildStatusTabUrl("da-giao") %>" class="<%= GetMobileTabClass("da-giao") %>">Đã giao</a>
                        <a href="<%= BuildStatusTabUrl("da-nhan") %>" class="<%= GetMobileTabClass("da-nhan") %>">Đã nhận</a>
                        <a href="<%= BuildStatusTabUrl("da-huy") %>" class="<%= GetMobileTabClass("da-huy") %>">Đã hủy</a>
                    </div>

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
                                                    <div class="btn-list justify-content-end">
                                                        <a class="btn btn-outline-secondary btn-sm"
                                                           href="<%# BuildOrderDetailUrl(Eval("id")) %>"
                                                           onclick='return openOrderDetailDrawer("<%# BuildOrderDetailUrl(Eval("id")) %>");'>
                                                            Chi tiết
                                                        </a>

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
                                                                OnClientClick="return markOrderActionBusy(this,'Đang xử lý...');"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                CssClass="btn btn-primary btn-sm">
                                                                Đã nhận hàng
                                                            </asp:LinkButton>
                                                        </asp:PlaceHolder>

                                                        <asp:PlaceHolder ID="ph_review_row" runat="server" Visible='<%# Convert.ToBoolean(Eval("show_review")) %>'>
                                                            <a class="btn btn-outline-primary btn-sm" href="javascript:void(0);" onclick="return showReviewUpgradeNotice();">Viết đánh giá</a>
                                                        </asp:PlaceHolder>

                                                        <asp:PlaceHolder ID="ph_rebuy_row" runat="server" Visible='<%# Convert.ToBoolean(Eval("show_rebuy")) %>'>
                                                            <a class="btn btn-primary btn-sm" href="<%# Eval("rebuy_url") %>"><%# Eval("rebuy_label") %></a>
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

                        <div class="orders-mobile">
                            <asp:Repeater ID="rp_orders_mobile" runat="server">
                                <ItemTemplate>
                                    <div class="order-card">
                                        <div class="order-card-head">
                                            <div><%# Eval("TenShop") %></div>
                                            <div class="order-card-status">
                                                <asp:PlaceHolder ID="ph_mobile_status_dat" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã đặt" %>'>
                                                    <span class="badge bg-azure-lt text-azure">Đã đặt</span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="ph_mobile_status_huy" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã hủy" %>'>
                                                    <span class="badge bg-red-lt text-red">Đã hủy</span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="ph_mobile_status_giao" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã giao" %>'>
                                                    <span class="badge bg-yellow-lt text-yellow">Đã giao</span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="ph_mobile_status_nhan" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã nhận" %>'>
                                                    <span class="badge bg-green-lt text-green">Đã nhận</span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="ph_mobile_status_chua_traodoi" runat="server" Visible='<%#Eval("trangthai").ToString()=="Chưa Trao đổi" %>'>
                                                    <span class="badge bg-orange-lt text-orange">Chưa Trao đổi</span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="ph_mobile_status_cho_traodoi" runat="server" Visible='<%#Eval("trangthai").ToString()=="Chờ Trao đổi" %>'>
                                                    <span class="badge bg-red-lt text-red">Chờ Trao đổi</span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="ph_mobile_status_da_traodoi" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã Trao đổi" %>'>
                                                    <span class="badge bg-blue-lt text-blue">Đã Trao đổi</span>
                                                </asp:PlaceHolder>
                                            </div>
                                        </div>

                                        <a class="order-card-item text-decoration-none" href="<%# BuildOrderDetailUrl(Eval("id")) %>">
                                            <img src="<%# Eval("first_item_image") %>" alt="" />
                                            <div>
                                                <div class="order-card-item-name"><%# Eval("first_item_name") %></div>
                                                <div class="order-card-item-meta">
                                                    x<%# Eval("first_item_qty") %>
                                                    <%# string.IsNullOrWhiteSpace((Eval("more_items_label") ?? "").ToString()) ? "" : (" · " + Eval("more_items_label")) %>
                                                </div>
                                                <div class="order-card-item-meta">
                                                    <%# Eval("first_item_price", "{0:#,##0}") %> ₫
                                                </div>
                                            </div>
                                        </a>

                                        <div class="order-card-total">
                                            Tổng: <%# FormatVnd(Eval("tongtien")) %> ₫
                                        </div>

                                        <div class="order-card-actions">
                                            <a class="btn btn-outline-secondary btn-sm" href="<%# BuildOrderDetailUrl(Eval("id")) %>">Chi tiết</a>
                                            <asp:PlaceHolder ID="ph_huy_mobile" runat="server" Visible='<%# Convert.ToBoolean(Eval("show_huydon")) %>'>
                                                <asp:LinkButton ID="but_huydonhang_mobile" runat="server"
                                                    OnClick="but_huydonhang_row_Click"
                                                    OnClientClick='<%# "return confirmCancelOrder(" + Eval("id") + ");" %>'
                                                    CommandArgument='<%# Eval("id") %>'
                                                    CssClass="btn btn-outline-danger btn-sm">
                                                    Huỷ đơn
                                                </asp:LinkButton>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="ph_nhan_mobile" runat="server" Visible='<%# Convert.ToBoolean(Eval("show_danhan")) %>'>
                                                <asp:LinkButton ID="but_danhanhang_mobile" runat="server"
                                                    OnClick="but_danhanhang_row_Click"
                                                    OnClientClick="return markOrderActionBusy(this,'Đang xử lý...');"
                                                    CommandArgument='<%# Eval("id") %>'
                                                    CssClass="btn btn-primary btn-sm">
                                                    Đã nhận
                                                </asp:LinkButton>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="ph_review_mobile" runat="server" Visible='<%# Convert.ToBoolean(Eval("show_review")) %>'>
                                                <a class="btn btn-outline-primary btn-sm" href="javascript:void(0);" onclick="return showReviewUpgradeNotice();">Viết đánh giá</a>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="ph_rebuy_mobile" runat="server" Visible='<%# Convert.ToBoolean(Eval("show_rebuy")) %>'>
                                                <a class="btn btn-primary btn-sm" href="<%# Eval("rebuy_url") %>"><%# Eval("rebuy_label") %></a>
                                            </asp:PlaceHolder>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                    <div id="order_detail_backdrop" class="order-drawer-backdrop" onclick="return closeOrderDetailDrawer();"></div>
                    <div id="order_detail_drawer" class="order-drawer" aria-hidden="true">
                        <div class="order-drawer-head">
                            <h3 class="order-drawer-title">Chi tiết đơn hàng</h3>
                            <button type="button" class="btn btn-outline-secondary btn-sm" onclick="return closeOrderDetailDrawer();">
                                Đóng
                            </button>
                        </div>
                        <iframe id="order_detail_frame" class="order-drawer-frame" src="about:blank"></iframe>
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
