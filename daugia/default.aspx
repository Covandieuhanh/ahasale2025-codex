<%@ Page Title="Đấu giá ngược" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="daugia_default" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .daugia-page {
            padding-bottom: 16px;
        }

        .daugia-thumb {
            width: 100%;
            aspect-ratio: 16/9;
            border-radius: 12px;
            overflow: hidden;
            background: #f1f5f9;
            border: 1px solid var(--tblr-border-color, #e2e8f0);
        }

        .daugia-thumb img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            display: block;
        }

        .daugia-stat-value {
            font-size: 22px;
            line-height: 1.1;
            font-weight: 700;
        }

        .daugia-summary-card {
            overflow: hidden;
            position: relative;
        }

        .daugia-summary-card::after {
            content: "";
            position: absolute;
            inset: auto -24px -24px auto;
            width: 96px;
            height: 96px;
            border-radius: 999px;
            background: radial-gradient(circle, rgba(22,163,74,.12) 0%, rgba(22,163,74,0) 70%);
            pointer-events: none;
        }

        .daugia-summary-card .card-body {
            padding: 14px 16px;
        }

        .daugia-title {
            color: inherit;
            text-decoration: none;
            display: block;
            line-height: 1.45;
        }

        .daugia-title:hover {
            text-decoration: underline;
        }

        .daugia-mobile-btns {
            width: 100%;
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .daugia-mobile-btns .btn {
            width: 100%;
            justify-content: center;
        }

        .daugia-live-card .card-body {
            padding: 12px;
        }

        .daugia-live-card .card-footer {
            padding: 12px;
        }

        .daugia-section-card {
            overflow: hidden;
        }

        .daugia-section-card .card-header {
            background: linear-gradient(180deg, rgba(22,163,74,.06) 0%, rgba(255,255,255,0) 100%);
        }

        @media (max-width: 767.98px) {
            .page-header .page-title {
                font-size: 1.35rem;
            }

            .card-actions.daugia-mobile-btns {
                margin-left: 0;
                margin-top: 8px;
            }
        }

        @media (min-width: 768px) {
            .daugia-page {
                padding-bottom: 24px;
            }

            .daugia-stat-value {
                font-size: 26px;
            }

            .daugia-mobile-btns {
                width: auto;
                flex-direction: row;
                flex-wrap: wrap;
                align-items: center;
            }

            .daugia-mobile-btns .btn {
                width: auto;
            }

            .card-actions.daugia-mobile-btns {
                margin-left: auto;
                margin-top: 0;
            }

            .daugia-summary-card .card-body {
                padding: 18px;
            }

            .daugia-live-card {
                transition: transform .18s ease, box-shadow .18s ease, border-color .18s ease;
            }

            .daugia-live-card:hover {
                transform: translateY(-3px);
                border-color: rgba(21,128,61,.22);
                box-shadow: 0 18px 34px rgba(16,42,67,.12);
            }

            .daugia-live-card .card-body,
            .daugia-live-card .card-footer {
                padding: 16px;
            }

            .daugia-title {
                min-height: 48px;
                font-size: 1rem;
            }
        }

        @media (min-width: 1400px) {
            .daugia-page-header .container-xl,
            .daugia-page .container-xl {
                max-width: 1440px;
            }

            .daugia-live-grid > [class*='col-'] {
                flex: 0 0 25%;
                width: 25%;
                max-width: 25%;
            }

            .daugia-live-card .card-body,
            .daugia-live-card .card-footer {
                padding: 18px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none daugia-page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">AhaSale / Đấu giá</div>
                    <h2 class="page-title">Đấu giá ngược</h2>
                    <div class="text-muted">Không gian hiển thị tổng hợp các phiên đấu giá công khai. Khu quản lý vận hành tách riêng tại `/daugia/admin`.</div>
                </div>
                <div class="col-12 col-md-auto ms-md-auto d-print-none">
                    <div class="btn-list daugia-mobile-btns">
                        <a class="btn btn-outline-primary btn-sm" href="/daugia/da-ket-thuc">Phiên đã kết thúc</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body daugia-page">
        <div class="container-xl">
            <div class="row row-cards mb-3">
                <div class="col-sm-6 col-lg-3">
                    <div class="card daugia-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Đang diễn ra</div>
                            <div class="daugia-stat-value"><%=Summary.LiveCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3">
                    <div class="card daugia-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Chờ duyệt</div>
                            <div class="daugia-stat-value"><%=Summary.PendingCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3">
                    <div class="card daugia-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Đã lịch</div>
                            <div class="daugia-stat-value"><%=Summary.ScheduledCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3">
                    <div class="card daugia-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Đã hoàn tất</div>
                            <div class="daugia-stat-value"><%=Summary.CompletedCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="card daugia-section-card">
                <div class="card-header">
                    <h3 class="card-title mb-0">Phiên đang diễn ra</h3>
                    <div class="card-actions daugia-mobile-btns">
                        <a class="btn btn-outline-secondary btn-sm" href="/daugia/da-ket-thuc">Xem lịch sử</a>
                    </div>
                </div>

                <div class="card-body">
                    <asp:PlaceHolder ID="phLive" runat="server">
                        <div class="row row-cards daugia-live-grid">
                            <asp:Repeater ID="rptLive" runat="server">
                                <ItemTemplate>
                                    <div class="col-md-6 col-xl-4 col-xxl-3">
                                        <article class="card h-100 daugia-live-card">
                                            <div class="card-body d-flex flex-column gap-2">
                                                <a class="daugia-thumb" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>">
                                                    <img src="<%# ResolveImage(Eval("SnapshotImage")) %>" alt="Ảnh phiên đấu giá" />
                                                </a>
                                                <div>
                                                    <span class='<%# BuildStatusBadgeCss(Eval("TrangThai")) %>'><%# BuildStatusLabel(Eval("TrangThai")) %></span>
                                                </div>
                                                <div class="fw-semibold">
                                                    <a class="daugia-title" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>"><%# Eval("SnapshotTitle") %></a>
                                                </div>
                                                <div class="text-muted small">Shop: <%# Eval("SellerAccount") %></div>
                                                <div class="text-muted small">Kết thúc: <%# FormatDate(Eval("PhienKetThuc")) %></div>
                                                <div class="text-muted small">Lượt đấu: <%# Eval("SoLuotBid") %></div>
                                                <div class="fw-bold text-success mt-1"><%# FormatPoint(Eval("GiaHienTai")) %> Quyền</div>
                                            </div>
                                            <div class="card-footer bg-transparent">
                                                <div class="daugia-mobile-btns">
                                                    <a class="btn btn-primary btn-sm" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>">Vào phiên</a>
                                                </div>
                                            </div>
                                        </article>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phLiveEmpty" runat="server" Visible="false">
                        <div class="alert alert-secondary mb-0">Hiện chưa có phiên nào đang mở.</div>
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
