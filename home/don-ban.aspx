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
        .pos-create-search,
        .pos-create-cart{
            min-width:0;
        }
        .pos-create-search .table-responsive,
        .pos-create-cart .table-responsive{
            width:100%;
            overflow-x:auto;
            overflow-y:visible;
        }
        .pos-search-toolbar{
            display:grid;
            grid-template-columns:minmax(0,1fr) 220px;
            gap:10px;
            align-items:start;
        }
        .pos-search-meta{
            display:flex;
            align-items:center;
            justify-content:space-between;
            gap:10px;
            flex-wrap:wrap;
        }
        .pos-mobile-stepbar,
        .pos-mobile-cartbar{
            display:none;
        }
        .pos-mobile-stepbtn{
            width:100%;
            min-height:42px;
            border:1px solid var(--tblr-border-color,#e2e8f0);
            border-radius:14px;
            background:var(--tblr-bg-surface,#fff);
            color:var(--tblr-body-color,#0f172a);
            font-weight:700;
            padding:0 12px;
        }
        .pos-mobile-stepbtn.is-active{
            border-color:#ee4d2d;
            background:#fff1ea;
            color:#c2410c;
        }
        .pos-mobile-cartbar{
            align-items:center;
            justify-content:space-between;
            gap:12px;
            margin-top:12px;
            padding:12px 14px;
            border-radius:16px;
            background:#fff7f2;
            border:1px solid #ffd4c4;
            box-shadow:0 12px 24px rgba(15,23,42,.08);
        }
        .pos-mobile-cartbar strong{
            display:block;
            font-size:15px;
            line-height:1.2;
            color:#111827;
        }
        .pos-mobile-cartbar span{
            display:block;
            font-size:12px;
            color:#6b7280;
        }
        .pos-standalone-page .modal-content{
            border-radius:18px;
            overflow:hidden;
        }
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
        @media (min-width: 992px){
            .pos-create-layout{
                align-items:flex-start;
            }
            .pos-create-search .card,
            .pos-create-cart .card{
                height:100%;
            }
        }
        @media (min-width: 768px){
            .pos-create-search .table,
            .pos-create-cart .table{
                margin-bottom:0;
            }
            .pos-create-search th,
            .pos-create-search td,
            .pos-create-cart th,
            .pos-create-cart td{
                padding:.75rem .65rem;
            }
            .pos-create-search th:nth-child(1),
            .pos-create-search td:nth-child(1),
            .pos-create-cart th:nth-child(1),
            .pos-create-cart td:nth-child(1){
                width:72px;
            }
            .pos-create-search th:nth-child(3),
            .pos-create-search td:nth-child(3){
                width:120px;
            }
            .pos-create-search th:nth-child(4),
            .pos-create-search td:nth-child(4){
                width:120px;
            }
            .pos-create-search th:nth-child(5),
            .pos-create-search td:nth-child(5){
                width:86px;
            }
            .pos-create-cart th:nth-child(3),
            .pos-create-cart td:nth-child(3){
                width:126px;
            }
            .pos-create-cart th:nth-child(4),
            .pos-create-cart td:nth-child(4){
                width:164px;
            }
            .pos-create-cart th:nth-child(5),
            .pos-create-cart td:nth-child(5){
                width:118px;
            }
            .pos-create-cart th:nth-child(6),
            .pos-create-cart td:nth-child(6){
                width:126px;
            }
            .pos-create-cart td:nth-child(2) .fw-semibold,
            .pos-create-search td:nth-child(2) .fw-semibold{
                word-break:break-word;
            }
            .pos-create-search .img-60,
            .pos-create-cart .img-60{
                width:52px;
                height:52px;
            }
        }
        @media (max-width: 767.98px){
            .pos-standalone-page{
                padding:0 0 18px;
            }
            .pos-standalone-page .modal-dialog{
                margin:0 !important;
                max-width:none !important;
                width:100%;
            }
            .pos-standalone-page .modal-content{
                min-height:100vh;
                border-radius:0;
                border-left:0;
                border-right:0;
            }
            .pos-standalone-page .modal-header{
                position:sticky;
                top:0;
                z-index:8;
                background:var(--tblr-bg-surface,#fff);
            }
            .pos-standalone-page .modal-body{
                max-height:none;
                overflow:visible;
                padding:12px;
            }
            .pos-search-toolbar{
                grid-template-columns:1fr;
            }
            .pos-mobile-stepbar{
                display:grid;
                grid-template-columns:1fr 1fr;
                gap:8px;
                margin-bottom:12px;
                position:sticky;
                top:0;
                z-index:7;
                background:var(--tblr-bg-surface,#fff);
                padding-bottom:8px;
            }
            .pos-create-layout.pos-mobile-search .pos-create-cart{
                display:none;
            }
            .pos-create-layout.pos-mobile-cart .pos-create-search{
                display:none;
            }
            .pos-create-layout.pos-mobile-search + .pos-mobile-cartbar{
                display:flex;
            }
            .pos-create-layout.pos-mobile-cart + .pos-mobile-cartbar{
                display:none;
            }
            .pos-standalone-page .modal-footer{
                position:sticky;
                bottom:0;
                z-index:8;
                background:var(--tblr-bg-surface,#fff);
                border-top:1px solid var(--tblr-border-color,#e2e8f0);
            }
            .pos-create-search .table-responsive,
            .pos-create-cart .table-responsive{
                overflow:visible;
            }
            .pos-create-search table,
            .pos-create-cart table{
                min-width:0 !important;
            }
            .pos-create-search thead,
            .pos-create-cart thead{
                display:none;
            }
            .pos-create-search tbody,
            .pos-create-cart tbody{
                display:flex;
                flex-direction:column;
                gap:10px;
                padding:12px;
            }
            .pos-create-search tbody tr,
            .pos-create-cart tbody tr{
                display:block;
                border:1px solid var(--tblr-border-color,#e2e8f0);
                border-radius:14px;
                padding:12px;
                background:var(--tblr-bg-surface,#fff);
                box-shadow:0 10px 24px rgba(15,23,42,.06);
            }
            .pos-create-search tbody td,
            .pos-create-cart tbody td{
                display:block;
                width:100%;
                padding:0 0 8px !important;
                border:0 !important;
                text-align:left !important;
                background:transparent !important;
            }
            .pos-create-search tbody td:last-child,
            .pos-create-cart tbody td:last-child{
                padding-bottom:0 !important;
            }
            .pos-create-search .img-60,
            .pos-create-cart .img-60{
                width:72px;
                height:72px;
            }
            .pos-create-search .btn-group{
                display:flex;
                width:100%;
            }
            .pos-create-search .btn-group .btn,
            .pos-create-cart .btn-group .btn{
                flex:1;
            }
            .pos-create-cart .qty-input{
                width:100%;
                max-width:none;
            }
            .pos-create-search .money,
            .pos-create-cart .money{
                font-weight:700;
            }
            .pos-create-search .btn-primary{
                width:100%;
            }
        }
        :root{
            --pay-page-bg:#f5f7fb;
            --pay-card:#ffffff;
            --pay-border:rgba(98,105,118,.2);
            --pay-text:#182433;
            --pay-muted:#64748b;
            --pay-danger:#d63939;
            --pay-primary:#ff5b2e;
            --pay-success:#16a34a;
            --pay-success-soft:#eafaf0;
        }
        html[data-bs-theme="dark"]{
            --pay-page-bg:#0b1220;
            --pay-card:#0f172a;
            --pay-border:#223246;
            --pay-text:#e5e7eb;
            --pay-muted:#94a3b8;
            --pay-danger:#f87171;
            --pay-primary:#ff7a47;
            --pay-success:#22c55e;
            --pay-success-soft:rgba(34,197,94,.14);
        }
        .pos-standalone-page .modal-content{
            background:var(--pay-page-bg);
            border:1px solid var(--pay-border);
            box-shadow:0 20px 40px rgba(15,23,42,.12);
        }
        .pos-shell-header{
            background:transparent;
            border:0;
            padding:18px 18px 0;
            align-items:flex-start;
        }
        .pos-shell-head{
            display:flex;
            flex-direction:column;
            gap:8px;
        }
        .pos-shell-step{
            display:inline-flex;
            align-items:center;
            gap:8px;
            font-size:12px;
            font-weight:700;
            padding:6px 11px;
            border-radius:999px;
            color:#6b3700;
            background:#fff3db;
            border:1px solid #ffd38a;
        }
        .pos-shell-title{
            margin:0;
            font-size:30px;
            line-height:1.15;
            font-weight:800;
            color:var(--pay-text);
        }
        .pos-shell-sub{
            margin:0;
            color:var(--pay-muted);
            font-size:14px;
        }
        .pos-shell-body{
            padding:16px 18px;
        }
        .pos-create-surface{
            background:var(--pay-page-bg);
            border:1px solid var(--pay-border);
            border-radius:20px;
            padding:16px;
        }
        .pos-create-search .card,
        .pos-create-cart .card{
            border:1px solid var(--pay-border);
            border-radius:16px;
            background:var(--pay-card);
            box-shadow:0 10px 25px rgba(15,23,42,.06);
            overflow:hidden;
        }
        .pos-create-search .card-header,
        .pos-create-cart .card-header{
            background:#fff;
            border-bottom:1px solid var(--pay-border);
            padding:12px 14px;
        }
        .pos-create-search .card-footer,
        .pos-create-cart .card-footer{
            background:#fff;
            border-top:1px solid var(--pay-border);
        }
        .pos-section-tag{
            display:inline-flex;
            align-items:center;
            font-size:12px;
            font-weight:700;
            color:#6b3700;
            background:#fff3db;
            border:1px solid #ffd38a;
            border-radius:999px;
            padding:4px 10px;
            margin-bottom:8px;
        }
        .pos-section-title{
            margin:0;
            font-size:18px;
            line-height:1.2;
            font-weight:800;
            color:var(--pay-text);
        }
        .pos-section-sub{
            margin-top:4px;
            color:var(--pay-muted);
            font-size:14px;
        }
        .pos-create-search .table,
        .pos-create-cart .table{
            color:var(--pay-text);
        }
        .pos-create-search thead th,
        .pos-create-cart thead th{
            color:var(--pay-muted);
            font-weight:800;
            text-transform:uppercase;
            font-size:13px;
            letter-spacing:.02em;
            background:rgba(255,255,255,.75);
        }
        .pos-create-search tbody tr,
        .pos-create-cart tbody tr{
            background:#fff;
        }
        .pos-create-search tbody tr:hover,
        .pos-create-cart tbody tr:hover{
            background:#fffaf6;
        }
        .pos-create-search .btn-primary,
        .pos-create-footer .btn-primary{
            background:linear-gradient(135deg,var(--pay-primary),#ff7a47);
            border-color:#ff6a39;
            font-weight:700;
        }
        .pos-create-search .btn-outline-primary,
        .pos-create-cart .btn-outline-primary{
            border-color:#ffb79f;
            color:#c2410c;
        }
        .pos-create-cart .btn-outline-danger{
            color:#d63939;
            border-color:#f2b5b5;
        }
        .pos-cart-summary{
            padding:12px 14px;
            display:flex;
            flex-direction:column;
            gap:8px;
        }
        .pos-summary-row{
            display:flex;
            align-items:center;
            justify-content:space-between;
            gap:10px;
            padding:6px 0;
            border-bottom:1px dashed rgba(100,116,139,.25);
            color:var(--pay-muted);
            font-size:14px;
        }
        .pos-summary-row:last-child{
            border-bottom:0;
        }
        .pos-summary-value{
            font-size:20px;
            line-height:1.1;
            font-weight:800;
            white-space:nowrap;
            color:var(--pay-text);
        }
        .pos-summary-value.vnd{
            color:var(--pay-danger);
        }
        .pos-summary-value.a{
            color:#0f4a84;
        }
        .pos-cart-error{
            min-height:18px;
            font-size:12px;
        }
        .pos-create-footer{
            border-top:0;
            background:transparent;
            padding:0 18px 18px;
        }
        .pos-paybar{
            border:1px solid var(--pay-border);
            border-radius:14px;
            background:#fff;
            padding:12px 14px;
            display:flex;
            align-items:center;
            justify-content:space-between;
            gap:12px;
            flex-wrap:wrap;
            box-shadow:0 12px 30px rgba(15,23,42,.14);
        }
        .pos-paybar-meta{
            color:var(--pay-muted);
            font-size:13px;
            font-weight:600;
        }
        .pos-paybar-trust{
            margin-top:3px;
            color:#065f46;
            font-size:12px;
            font-weight:600;
        }
        .pos-paybar-actions{
            display:flex;
            align-items:center;
            gap:10px;
            flex-wrap:wrap;
        }
        .pos-link-cancel{
            font-size:14px;
            font-weight:600;
            color:var(--pay-muted);
            text-decoration:none;
        }
        .pos-link-cancel:hover{
            text-decoration:underline;
        }
        html[data-bs-theme="dark"] .pos-create-search .card,
        html[data-bs-theme="dark"] .pos-create-cart .card{
            box-shadow:0 12px 30px rgba(0,0,0,.45);
        }
        html[data-bs-theme="dark"] .pos-create-search .card-header,
        html[data-bs-theme="dark"] .pos-create-cart .card-header,
        html[data-bs-theme="dark"] .pos-create-search .card-footer,
        html[data-bs-theme="dark"] .pos-create-cart .card-footer{
            background:#111d34;
        }
        html[data-bs-theme="dark"] .pos-paybar{
            background:#0f172a;
            box-shadow:0 12px 30px rgba(0,0,0,.45);
        }
        html[data-bs-theme="dark"] .pos-paybar-trust{
            color:#6ee7b7;
        }
        @media (max-width: 991.98px){
            .pos-shell-title{
                font-size:24px;
            }
            .pos-create-surface{
                padding:12px;
                border-radius:14px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">

    <!-- ===================== MODAL: TẠO ĐƠN OFFLINE (POS) ===================== -->
    <asp:UpdatePanel ID="up_taodon" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_taodon" runat="server" Visible="false" DefaultButton="but_taodon">
                <asp:HiddenField ID="hf_pos_mobile_step" runat="server" Value="search" />

                <div class="<%= GetCreateOrderWrapperCssClass() %>" style="display:block;" tabindex="-1" role="dialog" aria-modal="true">
                    <div class="modal-dialog modal-xl modal-dialog-centered" role="document" style="<%= GetCreateOrderDialogStyle() %>">
                        <div class="modal-content">

                            <div class="modal-header pos-shell-header">
                                <div class="pos-shell-head">
                                    <span class="pos-shell-step">Quy trình 2 bước: Chọn tin và tạo đơn</span>
                                    <h5 class="modal-title pos-shell-title">Tạo đơn gian hàng đối tác</h5>
                                    <p class="pos-shell-sub">Chọn sản phẩm hoặc dịch vụ, thêm vào giỏ rồi chốt đơn nhanh ngay trên một màn hình.</p>
                                </div>
                                <a href="#" class="btn-close" aria-label="Close" id="A1" runat="server" onserverclick="but_close_form_taodon_Click"></a>
                            </div>

                            <div class="modal-body pos-shell-body">
                                <div class="pos-create-surface">
                                <div class="pos-mobile-stepbar d-md-none">
                                    <button type="button" class="pos-mobile-stepbtn" data-pos-step="search" onclick="ahaPosSetStep('search')">
                                        Chọn tin
                                    </button>
                                    <button type="button" class="pos-mobile-stepbtn" data-pos-step="cart" onclick="ahaPosSetStep('cart')">
                                        Giỏ hàng (<asp:Label ID="lb_cart_count_mobile_tab" runat="server" Text="0"></asp:Label>)
                                    </button>
                                </div>

                                <div id="posCreateLayout" class="row g-3 pos-create-layout pos-mobile-search">

                                    <!-- LEFT: SEARCH + RESULTS -->
                                    <div class="col-lg-5 pos-create-search">
                                        <div class="card">
                                            <div class="card-header">
                                                <div class="w-100">
                                                    <div class="pos-section-tag">Bước 1: Chọn tin</div>
                                                    <h6 class="pos-section-title">Danh sách tin bán hàng</h6>
                                                    <div class="pos-section-sub">Lọc nhanh theo loại tin, gõ tên để tìm rồi bấm Thêm.</div>
                                                    <div class="pos-search-toolbar">
                                                        <asp:TextBox ID="txt_sp_search" runat="server"
                                                            CssClass="form-control"
                                                            placeholder="Nhập tên sản phẩm hoặc dịch vụ..."
                                                            AutoPostBack="true"
                                                            OnTextChanged="txt_sp_search_TextChanged"
                                                            MaxLength="100"></asp:TextBox>
                                                        <asp:DropDownList ID="ddl_trade_type" runat="server"
                                                            CssClass="form-select"
                                                            AutoPostBack="true">
                                                            <asp:ListItem Text="Tất cả tin" Value="all"></asp:ListItem>
                                                            <asp:ListItem Text="Chỉ sản phẩm" Value="product"></asp:ListItem>
                                                            <asp:ListItem Text="Chỉ dịch vụ" Value="service"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>

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
                                                            <th style="min-width:220px;">Tên sản phẩm / dịch vụ</th>
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
                                                                        <div class="mt-2">
                                                                            <span class='badge <%# ((Eval("phanloai") ?? "").ToString() == "dichvu") ? "bg-orange-lt text-orange" : "bg-azure-lt text-azure" %>'>
                                                                                <%# ((Eval("phanloai") ?? "").ToString() == "dichvu") ? "Dịch vụ" : "Sản phẩm" %>
                                                                            </span>
                                                                        </div>
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
                                                                <td colspan="5" class="text-center text-muted py-4">Không có sản phẩm hoặc dịch vụ phù hợp.</td>
                                                            </tr>
                                                        </asp:PlaceHolder>
                                                    </tbody>

                                                </table>
                                            </div>
                                            <div class="card-footer pos-search-meta">
                                                <div class="text-muted small">
                                                    <asp:Label ID="lb_search_result_summary" runat="server" Text="Hiển thị các tin gần nhất."></asp:Label>
                                                </div>
                                                <asp:LinkButton ID="but_search_more" runat="server"
                                                    CssClass="btn btn-outline-primary btn-sm"
                                                    Visible="false">
                                                    Xem thêm 20 tin
                                                </asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- RIGHT: CART -->
                                    <div class="col-lg-7 pos-create-cart">
                                        <div class="card">
                                            <div class="card-header">
                                                <div class="d-flex align-items-center w-100">
                                                    <div>
                                                        <div class="pos-section-tag">Bước 2: Hoàn tất giỏ hàng</div>
                                                        <h6 class="pos-section-title">Giỏ hàng tạo đơn</h6>
                                                        <div class="pos-section-sub">Cộng dồn số lượng khi thêm trùng, cho phép chỉnh SL và % ưu đãi trước khi chốt.</div>
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

                                            <div class="card-footer pos-cart-summary">
                                                <div class="pos-summary-row">
                                                    <span>Tổng giá trị đơn</span>
                                                    <span class="pos-summary-value vnd"><asp:Label ID="lb_cart_total_vnd" runat="server" Text="0"></asp:Label> ₫</span>
                                                </div>
                                                <div class="pos-summary-row">
                                                    <span>Quy đổi nội bộ</span>
                                                    <span class="pos-summary-value a"><asp:Label ID="lb_cart_total_a" runat="server" Text="0.00"></asp:Label> A</span>
                                                </div>
                                                <asp:Label ID="lb_cart_err" runat="server" CssClass="text-danger pos-cart-error" Text=""></asp:Label>
                                            </div>

                                        </div>
                                    </div>

                                </div>
                                <div class="pos-mobile-cartbar d-md-none">
                                    <div>
                                        <strong><asp:Label ID="lb_cart_count_mobile" runat="server" Text="0"></asp:Label> món trong giỏ</strong>
                                        <span>Tạm tính <asp:Label ID="lb_cart_total_mobile" runat="server" Text="0"></asp:Label> ₫</span>
                                    </div>
                                    <button type="button" class="btn btn-primary" onclick="ahaPosSetStep('cart')">Xem giỏ</button>
                                </div>
                                </div>

                            </div>

                            <div class="modal-footer pos-create-footer">
                                <div class="pos-paybar">
                                    <div>
                                        <div class="pos-paybar-meta">Đơn sẽ được tạo trong gian hàng đối tác đang đăng nhập và xuất hiện ngay tại danh sách đơn bán.</div>
                                        <div class="pos-paybar-trust">Bạn có thể thêm trùng để cộng số lượng hoặc chỉnh ưu đãi trước khi xác nhận.</div>
                                    </div>
                                    <div class="pos-paybar-actions">
                                        <a href="#" class="pos-link-cancel" id="A2" runat="server" onserverclick="but_close_form_taodon_Click">Quay lại</a>
                                        <asp:Button ID="but_taodon" OnClick="but_taodon_Click" runat="server"
                                            Text="TẠO ĐƠN" CssClass="btn btn-primary" />
                                    </div>
                                </div>
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

                        <div class="col-12">
                            <div class="d-flex flex-wrap gap-2">
                                <span class="badge bg-muted-lt text-muted">Tổng: <asp:Label ID="lb_count_total" runat="server" Text="0"></asp:Label></span>
                                <span class="badge bg-azure-lt text-azure">Đã đặt/Chưa Trao đổi: <asp:Label ID="lb_count_pending" runat="server" Text="0"></asp:Label></span>
                                <span class="badge bg-red-lt text-red">Chờ Trao đổi: <asp:Label ID="lb_count_wait" runat="server" Text="0"></asp:Label></span>
                                <span class="badge bg-blue-lt text-blue">Đã Trao đổi: <asp:Label ID="lb_count_exchanged" runat="server" Text="0"></asp:Label></span>
                                <span class="badge bg-green-lt text-green">Đã nhận: <asp:Label ID="lb_count_received" runat="server" Text="0"></asp:Label></span>
                                <span class="badge bg-muted-lt text-muted">Đã hủy: <asp:Label ID="lb_count_cancelled" runat="server" Text="0"></asp:Label></span>
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
                                    <asp:PlaceHolder ID="ph_empty_donban" runat="server" Visible="false">
                                        <tr>
                                            <td colspan="10" class="text-center text-muted py-4">Chưa có đơn bán phù hợp.</td>
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
                    <div class="mt-3 text-white">Đang xử lý...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="ContentFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="Server">
    <script>
        (function () {
            function getLayout() {
                return document.getElementById('posCreateLayout');
            }

            function getStepField() {
                return document.getElementById('<%= hf_pos_mobile_step.ClientID %>');
            }

            function applyStep(step) {
                var layout = getLayout();
                var field = getStepField();
                if (!layout || !field) return;

                var finalStep = step === 'cart' ? 'cart' : 'search';
                field.value = finalStep;

                layout.classList.remove('pos-mobile-search', 'pos-mobile-cart');
                layout.classList.add(finalStep === 'cart' ? 'pos-mobile-cart' : 'pos-mobile-search');

                var buttons = document.querySelectorAll('[data-pos-step]');
                for (var i = 0; i < buttons.length; i++) {
                    var btn = buttons[i];
                    var isActive = btn.getAttribute('data-pos-step') === finalStep;
                    btn.classList.toggle('is-active', isActive);
                }
            }

            window.ahaPosSetStep = function (step) {
                applyStep(step);
            };

            function initStep() {
                var field = getStepField();
                applyStep(field && field.value === 'cart' ? 'cart' : 'search');
            }

            if (window.Sys && Sys.Application) {
                Sys.Application.add_load(initStep);
            } else {
                document.addEventListener('DOMContentLoaded', initStep);
            }
        })();
    </script>
</asp:Content>
