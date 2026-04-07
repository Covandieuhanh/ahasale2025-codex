<%@ Page Title="Hồ sơ quyền tiêu dùng gian hàng" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerGianHang.master" AutoEventWireup="true" CodeFile="ho-so-tieu-dung.aspx.cs" Inherits="gianhang_ho_so_tieu_dung" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="server">
    <style>
        .gh-wallet-shell {
            min-height: 100vh;
            padding: 18px 0 30px;
            background:
                radial-gradient(900px 240px at 12% -6%, rgba(249,115,22,.16), transparent 62%),
                radial-gradient(900px 260px at 88% -8%, rgba(251,146,60,.12), transparent 58%),
                #f7fafc;
        }

        .gh-wallet-wrap {
            max-width: 1180px;
            margin: 0 auto;
            padding: 0 16px;
            display: grid;
            gap: 16px;
        }

        .gh-wallet-hero,
        .gh-wallet-card {
            background: #fff;
            border: 1px solid #dbe4ee;
            border-radius: 22px;
            box-shadow: 0 14px 32px rgba(15, 23, 42, .06);
        }

        .gh-wallet-hero {
            overflow: hidden;
        }

        .gh-wallet-hero-top {
            padding: 22px 24px;
            background: linear-gradient(135deg, #ffb36f, #f97316 56%, #fb923c);
            color: #fff;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 16px;
            flex-wrap: wrap;
        }

        .gh-wallet-eyebrow {
            display: inline-flex;
            align-items: center;
            min-height: 34px;
            padding: 0 12px;
            border-radius: 999px;
            background: rgba(255,255,255,.16);
            border: 1px solid rgba(255,255,255,.28);
            font-size: 12px;
            font-weight: 800;
            margin-bottom: 10px;
        }

        .gh-wallet-hero h1 {
            margin: 0;
            font-size: 30px;
            line-height: 1.08;
            font-weight: 800;
            color: #fff !important;
        }

        .gh-wallet-hero p {
            margin: 8px 0 0;
            max-width: 720px;
            font-size: 14px;
            line-height: 1.6;
            color: rgba(255,255,255,.94) !important;
        }

        .gh-wallet-back {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 40px;
            padding: 0 16px;
            border-radius: 999px;
            border: 1px solid rgba(255,255,255,.36);
            background: rgba(255,255,255,.14);
            color: #fff !important;
            text-decoration: none;
            font-size: 13px;
            font-weight: 800;
        }

        .gh-wallet-hero-bottom {
            padding: 18px 24px 22px;
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 14px;
        }

        .gh-wallet-stat {
            padding: 16px 18px;
            border-radius: 18px;
            background: #fff7ed;
            border: 1px solid #fed7aa;
        }

        .gh-wallet-stat-label {
            font-size: 12px;
            font-weight: 700;
            color: #9a3412;
            text-transform: uppercase;
        }

        .gh-wallet-stat-value {
            margin-top: 6px;
            font-size: 28px;
            line-height: 1.08;
            font-weight: 900;
            color: #7c2d12;
        }

        .gh-wallet-stat-sub {
            margin-top: 6px;
            font-size: 12px;
            color: #9a3412;
        }

        .gh-wallet-card-head {
            padding: 16px 18px;
            border-bottom: 1px solid #e8eef5;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
        }

        .gh-wallet-card-title {
            margin: 0;
            font-size: 20px;
            line-height: 1.15;
            font-weight: 800;
            color: #0f172a;
        }

        .gh-wallet-card-sub {
            margin-top: 5px;
            font-size: 13px;
            color: #64748b;
        }

        .gh-wallet-card-body {
            padding: 18px;
        }

        .gh-wallet-search {
            display: grid;
            grid-template-columns: minmax(0, 1fr) auto auto;
            gap: 10px;
            align-items: center;
        }

        .gh-wallet-input {
            width: 100%;
            height: 42px;
            border-radius: 14px;
            border: 1px solid #dbe4ee;
            padding: 0 14px;
            font-size: 14px;
            color: #0f172a;
            background: #fff;
        }

        .gh-wallet-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 42px;
            padding: 0 16px;
            border-radius: 14px;
            border: 1px solid #ea580c;
            background: #f97316;
            color: #fff;
            font-size: 13px;
            font-weight: 800;
            cursor: pointer;
        }

        .gh-wallet-btn--soft {
            background: #fff;
            color: #c2410c;
            border-color: #fdba74;
        }

        .gh-wallet-list {
            display: grid;
            gap: 12px;
        }

        .gh-wallet-item {
            border: 1px solid #e2e8f0;
            border-radius: 18px;
            padding: 16px 18px;
            display: grid;
            grid-template-columns: auto 1fr auto;
            gap: 14px;
            align-items: start;
        }

        .gh-wallet-item-badge {
            min-width: 86px;
            min-height: 36px;
            border-radius: 999px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 12px;
            font-weight: 800;
            padding: 0 12px;
        }

        .gh-wallet-item-badge--credit {
            background: #dcfce7;
            color: #166534;
        }

        .gh-wallet-item-badge--debit {
            background: #fee2e2;
            color: #b91c1c;
        }

        .gh-wallet-item-note {
            font-size: 15px;
            line-height: 1.55;
            color: #0f172a;
            font-weight: 600;
        }

        .gh-wallet-item-meta {
            margin-top: 8px;
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
        }

        .gh-wallet-meta-chip {
            display: inline-flex;
            align-items: center;
            min-height: 28px;
            padding: 0 10px;
            border-radius: 999px;
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            font-size: 12px;
            color: #475569;
        }

        .gh-wallet-item-amount {
            text-align: right;
        }

        .gh-wallet-amount-value {
            font-size: 22px;
            line-height: 1.1;
            font-weight: 900;
        }

        .gh-wallet-amount-value--credit {
            color: #15803d;
        }

        .gh-wallet-amount-value--debit {
            color: #dc2626;
        }

        .gh-wallet-amount-date {
            margin-top: 6px;
            font-size: 12px;
            color: #64748b;
        }

        .gh-wallet-empty {
            padding: 30px 18px;
            text-align: center;
            color: #64748b;
            border: 1px dashed #cbd5e1;
            border-radius: 18px;
            background: #f8fafc;
        }

        .gh-wallet-pager {
            margin-top: 16px;
        }

        .gh-wallet-pager :is(a, span) {
            margin-right: 6px;
        }

        @media (max-width: 900px) {
            .gh-wallet-hero-bottom,
            .gh-wallet-item {
                grid-template-columns: 1fr;
            }

            .gh-wallet-item-amount {
                text-align: left;
            }
        }

        @media (max-width: 640px) {
            .gh-wallet-search {
                grid-template-columns: 1fr;
            }

            .gh-wallet-hero-top,
            .gh-wallet-hero-bottom,
            .gh-wallet-card-head,
            .gh-wallet-card-body {
                padding-left: 16px;
                padding-right: 16px;
            }

            .gh-wallet-hero h1 {
                font-size: 24px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <section class="gh-wallet-shell">
        <div class="gh-wallet-wrap">
            <section class="gh-wallet-hero">
                <div class="gh-wallet-hero-top">
                    <div>
                        <span class="gh-wallet-eyebrow">GIAN HÀNG TÁCH RIÊNG</span>
                        <h1>Hồ sơ quyền tiêu dùng</h1>
                        <p>Đây là hồ sơ nhận quyền riêng của không gian gian hàng, chỉ cộng từ các đơn bán trao đổi thành công của tin do chính gian hàng đăng tải.</p>
                    </div>
                    <a class="gh-wallet-back" href="/gianhang/default.aspx">Quay về dashboard</a>
                </div>
                <div class="gh-wallet-hero-bottom">
                    <div class="gh-wallet-stat">
                        <div class="gh-wallet-stat-label">Số dư hiện tại</div>
                        <div class="gh-wallet-stat-value"><asp:Literal ID="lit_balance_a" runat="server" /></div>
                        <div class="gh-wallet-stat-sub">Tổng quyền tiêu dùng của riêng gian hàng</div>
                    </div>
                    <div class="gh-wallet-stat">
                        <div class="gh-wallet-stat-label">Quy đổi tham chiếu</div>
                        <div class="gh-wallet-stat-value"><asp:Literal ID="lit_balance_vnd" runat="server" /></div>
                        <div class="gh-wallet-stat-sub">Hiển thị tham chiếu theo tỷ lệ nội bộ hiện tại</div>
                    </div>
                    <div class="gh-wallet-stat">
                        <div class="gh-wallet-stat-label">Tổng giao dịch</div>
                        <div class="gh-wallet-stat-value"><asp:Literal ID="lit_total_count" runat="server" /></div>
                        <div class="gh-wallet-stat-sub"><asp:Literal ID="lit_page_state" runat="server" /></div>
                    </div>
                </div>
            </section>

            <section class="gh-wallet-card">
                <div class="gh-wallet-card-head">
                    <div>
                        <h2 class="gh-wallet-card-title">Lịch sử ghi nhận quyền</h2>
                        <div class="gh-wallet-card-sub">Tìm theo mã đơn, ghi chú hoặc tài khoản khách để rà soát các giao dịch đã cộng/trừ của không gian gian hàng.</div>
                    </div>
                </div>
                <div class="gh-wallet-card-body">
                    <div class="gh-wallet-search">
                        <asp:TextBox ID="txt_search" runat="server" CssClass="gh-wallet-input" placeholder="Tìm theo mã đơn, khách hàng hoặc ghi chú..." />
                        <asp:LinkButton ID="btn_search" runat="server" CssClass="gh-wallet-btn" OnClick="btn_search_Click">Tìm kiếm</asp:LinkButton>
                        <asp:LinkButton ID="btn_clear" runat="server" CssClass="gh-wallet-btn gh-wallet-btn--soft" OnClick="btn_clear_Click">Xóa lọc</asp:LinkButton>
                    </div>

                    <div class="gh-wallet-list" style="margin-top:16px;">
                        <asp:Repeater ID="rp_history" runat="server">
                            <ItemTemplate>
                                <article class="gh-wallet-item">
                                    <div>
                                        <span class='<%# BuildBadgeCss(Eval("IsCredit")) %>'><%# BuildBadgeText(Eval("IsCredit")) %></span>
                                    </div>
                                    <div>
                                        <div class="gh-wallet-item-note"><%# CleanNote(Eval("Note")) %></div>
                                        <div class="gh-wallet-item-meta">
                                            <span class="gh-wallet-meta-chip">Đơn: <%# BuildOrderLabel(Eval("PublicOrderId"), Eval("OrderId")) %></span>
                                            <span class="gh-wallet-meta-chip">Khách: <%# BuildBuyerLabel(Eval("BuyerAccount")) %></span>
                                            <span class="gh-wallet-meta-chip">Mã bút toán: <%# Eval("Id") %></span>
                                        </div>
                                    </div>
                                    <div class="gh-wallet-item-amount">
                                        <div class='<%# BuildAmountCss(Eval("IsCredit")) %>'><%# BuildSignedAmount(Eval("AmountA"), Eval("IsCredit")) %></div>
                                        <div class="gh-wallet-amount-date"><%# FormatDate(Eval("CreatedAt")) %></div>
                                    </div>
                                </article>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                            <div class="gh-wallet-empty">Chưa có giao dịch quyền tiêu dùng nào cho gian hàng này.</div>
                        </asp:PlaceHolder>
                    </div>

                    <div class="gh-wallet-pager">
                        <asp:Literal ID="litPager" runat="server" />
                    </div>
                </div>
            </section>
        </div>
    </section>
</asp:Content>
