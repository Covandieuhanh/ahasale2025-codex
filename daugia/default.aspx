<%@ Page Title="Đấu giá ngược" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="daugia_default" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .daugia-shell {
            padding: 20px 0 40px;
        }

        .daugia-hero {
            border-radius: 24px;
            padding: 24px;
            background: linear-gradient(125deg, #f8fffe, #eff8ff);
            border: 1px solid rgba(15, 23, 42, .08);
            box-shadow: 0 18px 40px rgba(15, 23, 42, .08);
            margin-bottom: 16px;
        }

        .daugia-grid {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 12px;
            margin-bottom: 16px;
        }

        .daugia-stat {
            background: #fff;
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 14px;
            padding: 14px 16px;
        }

        .daugia-stat__label {
            color: #64748b;
            font-size: 12px;
        }

        .daugia-stat__value {
            margin-top: 6px;
            font-size: 22px;
            font-weight: 700;
            line-height: 1;
            color: #0f172a;
        }

        .daugia-block {
            background: #fff;
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 18px;
            padding: 18px;
            margin-bottom: 16px;
        }

        .daugia-list {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 12px;
        }

        .daugia-card {
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 14px;
            padding: 12px;
            background: #fff;
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .daugia-thumb {
            width: 100%;
            aspect-ratio: 16/9;
            border-radius: 10px;
            overflow: hidden;
            background: #f1f5f9;
        }

        .daugia-thumb img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            display: block;
        }

        .daugia-title {
            margin: 0;
            font-size: 16px;
            line-height: 1.35;
            min-height: 44px;
        }

        .daugia-meta {
            color: #475569;
            font-size: 13px;
        }

        .daugia-price {
            font-weight: 700;
            color: #0f766e;
            font-size: 18px;
        }

        .daugia-badge {
            border-radius: 999px;
            font-size: 11px;
            padding: 3px 10px;
            display: inline-block;
            font-weight: 600;
            letter-spacing: .02em;
        }

        .daugia-badge--live {
            background: #dcfce7;
            color: #166534;
        }

        .daugia-badge--scheduled {
            background: #dbeafe;
            color: #1d4ed8;
        }

        .daugia-badge--pending {
            background: #fef3c7;
            color: #92400e;
        }

        .daugia-badge--done {
            background: #e2e8f0;
            color: #334155;
        }

        .daugia-empty {
            color: #64748b;
            font-size: 14px;
        }

        @media (max-width: 980px) {
            .daugia-grid {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }

            .daugia-list {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }

        @media (max-width: 640px) {
            .daugia-grid {
                grid-template-columns: 1fr;
            }

            .daugia-list {
                grid-template-columns: 1fr;
            }

            .daugia-hero {
                padding: 16px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl daugia-shell">
        <div class="daugia-hero">
            <div class="text-muted small">AhaSale / Business Module</div>
            <h1 class="mb-2">Đấu Giá Ngược</h1>
            <p class="mb-2 text-muted">Module độc lập của hệ AhaSale. Dùng chung tài khoản, ví E-AHA, thông báo và luồng admin duyệt.</p>
            <div class="d-flex gap-2 flex-wrap">
                <a class="btn btn-warning btn-sm" href="/home/dau-gia">Quản lý phiên của tôi</a>
                <a class="btn btn-success btn-sm" href="/daugia/tao">Tạo phiên đấu giá</a>
                <a class="btn btn-outline-success btn-sm" href="/home/quan-ly-tin/them.aspx">Tạo tài sản nguồn</a>
                <a class="btn btn-outline-primary btn-sm" href="/daugia/da-ket-thuc">Phiên đã kết thúc</a>
                <a class="btn btn-outline-dark btn-sm" href="/admin/quan-ly-dau-gia">Quản trị đấu giá</a>
            </div>
        </div>

        <div class="daugia-grid">
            <div class="daugia-stat">
                <div class="daugia-stat__label">Đang diễn ra</div>
                <div class="daugia-stat__value"><%=Summary.LiveCount.ToString("#,##0") %></div>
            </div>
            <div class="daugia-stat">
                <div class="daugia-stat__label">Chờ duyệt</div>
                <div class="daugia-stat__value"><%=Summary.PendingCount.ToString("#,##0") %></div>
            </div>
            <div class="daugia-stat">
                <div class="daugia-stat__label">Đã lịch</div>
                <div class="daugia-stat__value"><%=Summary.ScheduledCount.ToString("#,##0") %></div>
            </div>
            <div class="daugia-stat">
                <div class="daugia-stat__label">Đã hoàn tất</div>
                <div class="daugia-stat__value"><%=Summary.CompletedCount.ToString("#,##0") %></div>
            </div>
        </div>

        <div class="daugia-block">
            <div class="d-flex justify-content-between align-items-center mb-2">
                <h2 class="h5 mb-0">Phiên Đang Diễn Ra</h2>
                <a class="small text-decoration-none" href="/daugia/da-ket-thuc">Xem lịch sử</a>
            </div>
            <asp:PlaceHolder ID="phLive" runat="server">
                <div class="daugia-list">
                    <asp:Repeater ID="rptLive" runat="server">
                        <ItemTemplate>
                            <article class="daugia-card">
                                <a class="daugia-thumb" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>">
                                    <img src="<%# ResolveImage(Eval("SnapshotImage")) %>" alt="Ảnh phiên đấu giá" />
                                </a>
                                <div>
                                    <span class='<%# BuildStatusBadgeCss(Eval("TrangThai")) %>'><%# BuildStatusLabel(Eval("TrangThai")) %></span>
                                </div>
                                <h3 class="daugia-title">
                                    <a class="text-decoration-none text-dark" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>"><%# Eval("SnapshotTitle") %></a>
                                </h3>
                                <div class="daugia-meta">Shop: <%# Eval("SellerAccount") %></div>
                                <div class="daugia-meta">Kết thúc: <%# FormatDate(Eval("PhienKetThuc")) %></div>
                                <div class="daugia-meta">Lượt đấu: <%# Eval("SoLuotBid") %></div>
                                <div class="daugia-price"><%# FormatPoint(Eval("GiaHienTai")) %> E-AHA</div>
                                <a class="btn btn-primary btn-sm" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>">Vào phiên</a>
                            </article>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phLiveEmpty" runat="server" Visible="false">
                <div class="daugia-empty">Hiện chưa có phiên nào đang mở.</div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
