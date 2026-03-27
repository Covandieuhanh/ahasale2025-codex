<%@ Page Title="Đơn mua gian hàng" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="don-mua.aspx.cs" Inherits="gianhang_don_mua" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .gh-buyer-shell{padding:18px 0 24px;}
        .gh-buyer-header{
            background:linear-gradient(135deg,#fff7f2,#ffffff);
            border:1px solid #ffd4c4;
            border-radius:20px;
            padding:18px;
            box-shadow:0 16px 40px rgba(15,23,42,.08);
        }
        .gh-buyer-chip{
            display:inline-flex;align-items:center;gap:8px;padding:7px 12px;border-radius:999px;
            border:1px solid #ffd4c4;background:#fff3eb;color:#c2410c;font-size:12px;font-weight:700;
        }
        .gh-buyer-title{margin:12px 0 4px;font-size:34px;line-height:1.1;font-weight:800;color:#182433;}
        .gh-buyer-sub{margin:0;color:#64748b;font-size:15px;}
        .gh-buyer-summary{display:grid;grid-template-columns:repeat(auto-fit,minmax(140px,1fr));gap:12px;margin-top:16px;}
        .gh-buyer-summary-card{
            border:1px solid rgba(98,105,118,.18);border-radius:16px;background:#fff;padding:14px 16px;
        }
        .gh-buyer-summary-label{font-size:12px;font-weight:700;letter-spacing:.04em;text-transform:uppercase;color:#64748b;}
        .gh-buyer-summary-value{margin-top:8px;font-size:28px;line-height:1;font-weight:800;color:#182433;}
        .gh-buyer-panel{
            margin-top:18px;background:#fff;border:1px solid rgba(98,105,118,.18);border-radius:20px;
            box-shadow:0 16px 40px rgba(15,23,42,.08);overflow:hidden;
        }
        .gh-buyer-panel-head{padding:16px 18px;border-bottom:1px solid rgba(98,105,118,.16);}
        .gh-buyer-panel-title{margin:0;font-size:22px;font-weight:800;color:#182433;}
        .gh-buyer-panel-sub{margin-top:4px;color:#64748b;font-size:14px;}
        .gh-buyer-filter{padding:16px 18px;border-bottom:1px solid rgba(98,105,118,.16);display:grid;grid-template-columns:minmax(0,1fr) 220px;gap:12px;}
        .gh-buyer-empty{padding:28px 18px;color:#64748b;text-align:center;}
        .gh-buyer-order{padding:18px;border-bottom:1px solid rgba(98,105,118,.14);}
        .gh-buyer-order:last-child{border-bottom:0;}
        .gh-buyer-order-head{display:flex;align-items:flex-start;justify-content:space-between;gap:12px;flex-wrap:wrap;}
        .gh-buyer-order-store{font-size:20px;font-weight:800;color:#182433;}
        .gh-buyer-order-meta{margin-top:6px;color:#64748b;font-size:13px;}
        .gh-buyer-order-status{
            display:inline-flex;align-items:center;justify-content:center;padding:8px 14px;border-radius:999px;
            font-size:13px;font-weight:700;border:1px solid transparent;
        }
        .gh-report-status-neutral{background:#f8fafc;color:#475569;border-color:#cbd5e1;}
        .gh-report-status-warning{background:#fff7ed;color:#c2410c;border-color:#fdba74;}
        .gh-report-status-info{background:#eff6ff;color:#1d4ed8;border-color:#93c5fd;}
        .gh-report-status-success{background:#ecfdf5;color:#047857;border-color:#6ee7b7;}
        .gh-report-status-danger{background:#fef2f2;color:#b91c1c;border-color:#fca5a5;}
        .gh-buyer-order-body{
            margin-top:14px;display:grid;grid-template-columns:72px minmax(0,1fr) auto;gap:14px;align-items:center;
        }
        .gh-buyer-order-image{width:72px;height:72px;border-radius:16px;object-fit:cover;background:#f8fafc;border:1px solid rgba(98,105,118,.16);}
        .gh-buyer-order-name{font-size:18px;font-weight:800;color:#182433;line-height:1.2;}
        .gh-buyer-order-detail{margin-top:4px;color:#64748b;font-size:14px;}
        .gh-buyer-order-total{text-align:right;}
        .gh-buyer-order-total-label{font-size:12px;font-weight:700;letter-spacing:.04em;text-transform:uppercase;color:#64748b;}
        .gh-buyer-order-total-value{margin-top:6px;font-size:24px;line-height:1;font-weight:800;color:#d63939;white-space:nowrap;}
        .gh-buyer-order-actions{margin-top:14px;display:flex;align-items:center;gap:10px;flex-wrap:wrap;}
        @media (max-width: 767.98px){
            .gh-buyer-title{font-size:28px;}
            .gh-buyer-filter{grid-template-columns:1fr;}
            .gh-buyer-order-body{grid-template-columns:72px minmax(0,1fr);}
            .gh-buyer-order-total{text-align:left;}
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl gh-buyer-shell">
        <div class="gh-buyer-header">
            <span class="gh-buyer-chip">Lịch sử mua hàng</span>
            <h1 class="gh-buyer-title">Đơn mua gian hàng</h1>
            <p class="gh-buyer-sub">Theo dõi các đơn bạn đã tạo và trạng thái xử lý mới nhất của từng đơn.</p>
            <div class="gh-buyer-summary">
                <div class="gh-buyer-summary-card">
                    <div class="gh-buyer-summary-label">Tổng đơn</div>
                    <div class="gh-buyer-summary-value"><asp:Literal ID="lit_summary_total" runat="server" /></div>
                </div>
                <div class="gh-buyer-summary-card">
                    <div class="gh-buyer-summary-label">Chờ trao đổi</div>
                    <div class="gh-buyer-summary-value"><asp:Literal ID="lit_summary_waiting" runat="server" /></div>
                </div>
                <div class="gh-buyer-summary-card">
                    <div class="gh-buyer-summary-label">Đã trao đổi</div>
                    <div class="gh-buyer-summary-value"><asp:Literal ID="lit_summary_exchanged" runat="server" /></div>
                </div>
                <div class="gh-buyer-summary-card">
                    <div class="gh-buyer-summary-label">Đã giao</div>
                    <div class="gh-buyer-summary-value"><asp:Literal ID="lit_summary_delivered" runat="server" /></div>
                </div>
                <div class="gh-buyer-summary-card">
                    <div class="gh-buyer-summary-label">Đã hủy</div>
                    <div class="gh-buyer-summary-value"><asp:Literal ID="lit_summary_cancelled" runat="server" /></div>
                </div>
            </div>
        </div>

        <asp:PlaceHolder ID="ph_notice" runat="server" Visible="false">
            <div class="alert alert-success mt-3 mb-0">
                <asp:Literal ID="lit_notice" runat="server" />
            </div>
        </asp:PlaceHolder>

        <div class="gh-buyer-panel">
            <div class="gh-buyer-panel-head">
                <h2 class="gh-buyer-panel-title">Danh sách đơn</h2>
                <div class="gh-buyer-panel-sub">Mỗi đơn hiển thị trạng thái hiện tại và tổng thanh toán.</div>
            </div>
            <div class="gh-buyer-filter">
                <asp:TextBox ID="txt_search" runat="server" CssClass="form-control" placeholder="Tìm theo gian hàng, mã đơn hoặc tên khách..." AutoPostBack="true" OnTextChanged="txt_search_TextChanged"></asp:TextBox>
                <asp:DropDownList ID="ddl_status" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddl_status_SelectedIndexChanged">
                    <asp:ListItem Value="all">Tất cả trạng thái</asp:ListItem>
                    <asp:ListItem Value="da-dat">Đã đặt</asp:ListItem>
                    <asp:ListItem Value="cho-trao-doi">Chờ trao đổi</asp:ListItem>
                    <asp:ListItem Value="da-trao-doi">Đã trao đổi</asp:ListItem>
                    <asp:ListItem Value="da-giao">Đã giao</asp:ListItem>
                    <asp:ListItem Value="da-huy">Đã hủy</asp:ListItem>
                </asp:DropDownList>
            </div>

            <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                <div class="gh-buyer-empty">Chưa có đơn mua nào.</div>
            </asp:PlaceHolder>

            <asp:Repeater ID="rp_orders" runat="server">
                <ItemTemplate>
                    <div class="gh-buyer-order">
                        <div class="gh-buyer-order-head">
                            <div>
                                <div class="gh-buyer-order-store"><%# Server.HtmlEncode(Convert.ToString(Eval("StoreName")) ?? "") %></div>
                                <div class="gh-buyer-order-meta">
                                    Mã đơn: <%# Server.HtmlEncode(Convert.ToString(Eval("OrderId")) ?? ("GH-" + Convert.ToString(Eval("InvoiceId")))) %>
                                    &nbsp;•&nbsp;
                                    <%# Server.HtmlEncode(FormatDate(Eval("CreatedAt"))) %>
                                </div>
                            </div>
                            <span class='<%# Convert.ToString(Eval("StatusCss")) %> gh-buyer-order-status'><%# Server.HtmlEncode(Convert.ToString(Eval("StatusText")) ?? "") %></span>
                        </div>
                        <div class="gh-buyer-order-body">
                            <img class="gh-buyer-order-image" src='<%# ResolveImageUrl(Eval("FirstItemImage")) %>' alt="">
                            <div>
                                <div class="gh-buyer-order-name"><%# Server.HtmlEncode(Convert.ToString(Eval("FirstItemName")) ?? "") %></div>
                                <div class="gh-buyer-order-detail">
                                    Số lượng đầu tiên: <%# Convert.ToString(Eval("FirstItemQty")) %> • Tổng món: <%# Convert.ToString(Eval("TotalItems")) %>
                                </div>
                            </div>
                            <div class="gh-buyer-order-total">
                                <div class="gh-buyer-order-total-label">Tổng thanh toán</div>
                                <div class="gh-buyer-order-total-value"><%# FormatCurrency(Eval("TotalAmount")) %> đ</div>
                            </div>
                        </div>
                        <div class="gh-buyer-order-actions">
                            <a class="btn btn-outline-primary btn-sm" href='<%# Convert.ToString(Eval("PublicUrl")) %>'>Xem gian hàng</a>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server"></asp:Content>
<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="Server"></asp:Content>
