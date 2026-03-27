<%@ Page Title="Khách hàng gian hàng" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="khach-hang.aspx.cs" Inherits="gianhang_khach_hang" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="server">
    <style>
        :root {
            --gh-orange: #f97316;
            --gh-ink: #102a43;
            --gh-muted: #627d98;
            --gh-line: #d9e2ec;
            --gh-bg: #f5f8fb;
            --gh-card: #ffffff;
        }

        .gh-customer-shell {
            min-height: 100vh;
            padding: 24px 16px 40px;
            background:
                radial-gradient(1000px 280px at 16% -4%, rgba(249,115,22,.20), transparent 65%),
                radial-gradient(1000px 320px at 86% -6%, rgba(251,146,60,.18), transparent 62%),
                var(--gh-bg);
        }

        .gh-customer-wrap {
            max-width: 1180px;
            margin: 0 auto;
        }

        .gh-customer-card,
        .gh-customer-hero {
            background: var(--gh-card);
            border: 1px solid var(--gh-line);
            border-radius: 18px;
            box-shadow: 0 16px 36px rgba(16, 42, 67, .08);
        }

        .gh-customer-hero {
            padding: 24px;
            margin-bottom: 18px;
        }

        .gh-customer-eyebrow {
            color: #ea580c;
            font-weight: 800;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: .08em;
        }

        .gh-customer-title {
            margin: 10px 0 8px;
            color: var(--gh-ink);
            font-size: 32px;
            line-height: 1.15;
            font-weight: 900;
        }

        .gh-customer-sub {
            color: var(--gh-muted);
            font-size: 15px;
            line-height: 1.6;
            max-width: 760px;
        }

        .gh-customer-toolbar {
            display: flex;
            flex-wrap: wrap;
            justify-content: space-between;
            gap: 12px;
            align-items: center;
            margin-bottom: 14px;
        }

        .gh-customer-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
        }

        .gh-link-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 40px;
            padding: 0 16px;
            border-radius: 12px;
            border: 1px solid var(--gh-line);
            color: var(--gh-ink);
            text-decoration: none;
            font-weight: 800;
            background: #fff;
        }

        .gh-filter-box {
            display: grid;
            grid-template-columns: 1fr auto auto;
            gap: 10px;
            margin-bottom: 16px;
        }

        .gh-filter-input {
            min-height: 42px;
            border-radius: 12px;
            border: 1px solid var(--gh-line);
            padding: 0 14px;
        }

        .gh-filter-btn {
            min-height: 42px;
            border-radius: 12px;
            border: 1px solid transparent;
            padding: 0 16px;
            font-weight: 800;
            background: linear-gradient(135deg, #ffb26c, #f97316);
            color: #fff;
        }

        .gh-filter-btn-soft {
            background: #fff;
            color: var(--gh-ink);
            border-color: var(--gh-line);
        }

        .gh-customer-card {
            padding: 18px;
        }

        .gh-customer-table-wrap {
            overflow: auto;
        }

        .gh-customer-table {
            width: 100%;
            border-collapse: collapse;
        }

        .gh-customer-table th,
        .gh-customer-table td {
            padding: 12px 10px;
            border-bottom: 1px solid #e6eef7;
            text-align: left;
            vertical-align: top;
            color: var(--gh-ink);
        }

        .gh-customer-table th {
            font-size: 12px;
            text-transform: uppercase;
            color: var(--gh-muted);
        }

        .gh-empty {
            padding: 28px 18px;
            text-align: center;
            color: var(--gh-muted);
            font-weight: 700;
        }

        .gh-muted { color: var(--gh-muted); }

        @media (max-width: 760px) {
            .gh-filter-box {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
    <div class="gh-customer-shell">
        <div class="gh-customer-wrap">
            <section class="gh-customer-hero">
                <div class="gh-customer-eyebrow">Khách hàng</div>
                <h1 class="gh-customer-title">Khách hàng gian hàng</h1>
                <div class="gh-customer-sub">Danh sách khách hàng phát sinh từ đơn bán và lịch hẹn của gian hàng.</div>
            </section>

            <section class="gh-customer-card">
                <div class="gh-customer-toolbar">
                    <div>
                        <strong>Danh sách khách hàng</strong>
                        <div class="gh-muted">Tìm theo số điện thoại, tài khoản mua hoặc tên khách.</div>
                    </div>
                    <div class="gh-customer-actions">
                        <a href="/gianhang/default.aspx" class="gh-link-btn">Trang công khai</a>
                        <a href="/gianhang/don-ban.aspx" class="gh-link-btn">Đơn bán</a>
                        <a href="/gianhang/quan-ly-lich-hen.aspx" class="gh-link-btn">Lịch hẹn</a>
                    </div>
                </div>

                <div class="gh-filter-box">
                    <asp:TextBox ID="txt_search" runat="server" CssClass="gh-filter-input" MaxLength="100" placeholder="Tìm theo tên, số điện thoại hoặc tài khoản mua"></asp:TextBox>
                    <asp:Button ID="btn_filter" runat="server" CssClass="gh-filter-btn" Text="Lọc" OnClick="btn_filter_Click" />
                    <asp:Button ID="btn_reset" runat="server" CssClass="gh-filter-btn gh-filter-btn-soft" Text="Làm mới" OnClick="btn_reset_Click" CausesValidation="false" />
                </div>

                <div class="gh-customer-table-wrap">
                    <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                        <div class="gh-empty">Chưa có khách hàng nào phù hợp.</div>
                    </asp:PlaceHolder>

                    <asp:Repeater ID="rp_customers" runat="server">
                        <HeaderTemplate>
                            <table class="gh-customer-table">
                                <thead>
                                    <tr>
                                        <th>Khách hàng</th>
                                        <th>Điện thoại</th>
                                        <th>Tài khoản mua</th>
                                        <th>Đơn bán</th>
                                        <th>Lịch hẹn</th>
                                        <th>Doanh thu</th>
                                        <th>Tương tác gần nhất</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <a href="<%# BuildDetailUrl(Eval("CustomerKey")) %>" style="font-weight:800;color:#102a43;text-decoration:none;">
                                        <%# Eval("DisplayName") %>
                                    </a>
                                </td>
                                <td><%# Eval("Phone") %></td>
                                <td><span class="gh-muted"><%# string.IsNullOrWhiteSpace((Eval("BuyerAccount") ?? "").ToString()) ? "--" : Eval("BuyerAccount") %></span></td>
                                <td><%# Eval("OrderCount") %></td>
                                <td><%# Eval("BookingCount") %></td>
                                <td><%# GianHangReport_cl.FormatCurrency(Convert.ToDecimal(Eval("RevenueTotal"))) %> đ</td>
                                <td><%# GianHangReport_cl.FormatDateTime((DateTime?)Eval("LastInteractionAt")) %></td>
                                <td>
                                    <a href="<%# BuildDetailUrl(Eval("CustomerKey")) %>" class="gh-link-btn" style="min-height:34px;padding:0 12px;">Chi tiết</a>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
