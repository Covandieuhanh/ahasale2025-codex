<%@ Page Title="Phiên đấu giá đã kết thúc" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="da-ket-thuc.aspx.cs" Inherits="daugia_da_ket_thuc" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .daugia-ended-page {
            padding-bottom: 16px;
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

        .daugia-ended-card .card-body {
            padding: 12px;
        }

        .daugia-ended-section {
            overflow: hidden;
        }

        .daugia-ended-section .card-header {
            background: linear-gradient(180deg, rgba(15,118,110,.06) 0%, rgba(255,255,255,0) 100%);
        }

        @media (max-width: 767.98px) {
            .page-header .page-title {
                font-size: 1.35rem;
            }
        }

        @media (min-width: 768px) {
            .daugia-ended-page {
                padding-bottom: 24px;
            }

            .daugia-mobile-btns {
                width: auto;
                flex-direction: row;
                flex-wrap: wrap;
            }

            .daugia-mobile-btns .btn {
                width: auto;
            }

            .daugia-ended-card {
                transition: transform .18s ease, box-shadow .18s ease, border-color .18s ease;
            }

            .daugia-ended-card:hover {
                transform: translateY(-3px);
                border-color: rgba(15,118,110,.22);
                box-shadow: 0 18px 34px rgba(16,42,67,.12);
            }

            .daugia-ended-card .card-body {
                padding: 16px;
            }

            .daugia-title {
                min-height: 48px;
                font-size: 1rem;
            }
        }

        @media (min-width: 1400px) {
            .daugia-ended-header .container-xl,
            .daugia-ended-page .container-xl {
                max-width: 1440px;
            }

            .daugia-ended-grid > [class*='col-'] {
                flex: 0 0 25%;
                width: 25%;
                max-width: 25%;
            }

            .daugia-ended-card .card-body {
                padding: 18px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none daugia-ended-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">AhaSale / Đấu giá</div>
                    <h2 class="page-title">Phiên đấu giá đã kết thúc</h2>
                </div>
                <div class="col-12 col-md-auto ms-md-auto d-print-none">
                    <div class="btn-list daugia-mobile-btns">
                        <a href="/daugia" class="btn btn-outline-primary btn-sm">Quay lại phiên live</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body daugia-ended-page">
        <div class="container-xl">
            <div class="card daugia-ended-section">
                <div class="card-header">
                    <h3 class="card-title mb-0">Lịch sử phiên đấu giá</h3>
                </div>
                <div class="card-body">
                    <asp:PlaceHolder ID="phList" runat="server">
                        <div class="row row-cards daugia-ended-grid">
                            <asp:Repeater ID="rptEnded" runat="server">
                                <ItemTemplate>
                                    <div class="col-md-6 col-xl-4 col-xxl-3">
                                        <article class="card h-100 daugia-ended-card">
                                            <div class="card-body">
                                                <div class="fw-semibold mb-2">
                                                    <a class="daugia-title" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>"><%# Eval("SnapshotTitle") %></a>
                                                </div>
                                                <div class="text-muted small mb-1">Shop: <%# Eval("SellerAccount") %></div>
                                                <div class="text-muted small mb-1">Trạng thái: <%# BuildStatusLabel(Eval("TrangThai")) %></div>
                                                <div class="text-muted small mb-1">Kết thúc: <%# FormatDate(Eval("PhienKetThuc")) %></div>
                                                <div class="text-muted small mb-2">Người mua: <%# Eval("WinnerAccount") %></div>
                                                <div class="fw-bold text-success"><%# FormatPoint(Eval("GiaHienTai")) %> Quyền</div>
                                            </div>
                                        </article>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
                        <div class="alert alert-secondary mb-0">Chưa có phiên đấu giá kết thúc.</div>
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
