<%@ Page Title="Chi tiết đấu giá" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="chi-tiet.aspx.cs" Inherits="daugia_chi_tiet" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .daugia-detail-page {
            padding-bottom: 16px;
        }

        .daugia-image {
            width: 100%;
            aspect-ratio: 16/9;
            border-radius: 12px;
            overflow: hidden;
            background: #f1f5f9;
            border: 1px solid var(--tblr-border-color, #e2e8f0);
        }

        .daugia-image img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            display: block;
        }

        .daugia-detail-main,
        .daugia-history-card {
            overflow: hidden;
        }

        .daugia-detail-main .card-header,
        .daugia-history-card .card-header {
            background: linear-gradient(180deg, rgba(29,78,216,.05) 0%, rgba(255,255,255,0) 100%);
        }

        .daugia-status {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            border-radius: 999px;
            padding: 4px 10px;
            font-size: 12px;
            font-weight: 600;
            background: var(--tblr-blue-lt, #dbeafe);
            color: var(--tblr-blue, #1d4ed8);
        }

        .daugia-info {
            display: grid;
            grid-template-columns: 1fr;
            gap: 10px;
        }

        .daugia-info .item {
            border: 1px solid var(--tblr-border-color, #e2e8f0);
            border-radius: 12px;
            padding: 12px;
            background: var(--tblr-bg-surface, #ffffff);
        }

        .daugia-info .label {
            color: var(--tblr-muted, #64748b);
            font-size: 12px;
        }

        .daugia-info .value {
            margin-top: 4px;
            font-size: 15px;
            font-weight: 600;
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

        .daugia-detail-main .card-body {
            padding: 12px;
        }

        .daugia-detail-main .card-footer {
            padding: 12px;
        }

        .daugia-detail-side {
            display: flex;
            flex-direction: column;
            gap: 12px;
            height: 100%;
        }

        @media (max-width: 767.98px) {
            .page-header .page-title {
                font-size: 1.35rem;
            }

            .daugia-bid-table th,
            .daugia-bid-table td {
                white-space: nowrap;
                font-size: 12px;
            }
        }

        @media (min-width: 768px) {
            .daugia-detail-page {
                padding-bottom: 24px;
            }

            .daugia-info {
                grid-template-columns: 1fr 1fr;
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

            .daugia-detail-main .card-body,
            .daugia-detail-main .card-footer {
                padding: 18px;
            }

            .daugia-image {
                box-shadow: 0 20px 36px rgba(16,42,67,.12);
            }

            .daugia-info .item {
                min-height: 96px;
            }

            .daugia-history-card tbody tr:hover {
                background: rgba(29,78,216,.03);
            }
        }

        @media (min-width: 1400px) {
            .daugia-detail-header .container-xl,
            .daugia-detail-page .container-xl {
                max-width: 1440px;
            }

            .daugia-detail-main .card-body,
            .daugia-detail-main .card-footer {
                padding: 22px;
            }

            .daugia-image {
                aspect-ratio: 21/10;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none daugia-detail-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">AhaSale / Đấu giá</div>
                    <h2 class="page-title">Chi tiết phiên đấu giá</h2>
                </div>
                <div class="col-12 col-md-auto ms-md-auto d-print-none">
                    <div class="btn-list daugia-mobile-btns">
                        <a class="btn btn-outline-secondary btn-sm" href="/daugia">Quay lại danh sách</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body daugia-detail-page">
        <div class="container-xl">
            <asp:PlaceHolder ID="phNotFound" runat="server" Visible="false">
                <div class="card">
                    <div class="card-body">
                        <h1 class="h5 mb-1">Không tìm thấy phiên đấu giá</h1>
                        <p class="text-muted mb-2">Phiên có thể đã bị xóa hoặc không còn hiển thị.</p>
                        <a href="/daugia" class="btn btn-outline-primary btn-sm">Quay lại danh sách</a>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phDetail" runat="server">
                <div class="card mb-3 daugia-detail-main">
                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-lg-7">
                                <div class="daugia-image">
                                    <img src="<%=ImageUrl %>" alt="Ảnh đấu giá" />
                                </div>
                            </div>
                            <div class="col-lg-5">
                                <div class="daugia-detail-side">
                                    <div>
                                        <div class="mb-2">
                                            <span class="daugia-status"><%=StatusLabel %></span>
                                        </div>
                                        <h1 class="h4 mb-2"><%=AuctionTitle %></h1>
                                        <div class="text-muted mb-3"><%=AuctionDesc %></div>
                                    </div>

                                    <div class="daugia-info">
                                        <div class="item">
                                            <div class="label">Giá hiện tại</div>
                                            <div class="value text-success"><%=CurrentPrice %> Quyền</div>
                                        </div>
                                        <div class="item">
                                            <div class="label">Phí mỗi lượt</div>
                                            <div class="value"><%=BidFee %> Quyền</div>
                                        </div>
                                        <div class="item">
                                            <div class="label">Shop</div>
                                            <div class="value"><%=Seller %></div>
                                        </div>
                                        <div class="item">
                                            <div class="label">Người giữ mua</div>
                                            <div class="value"><%=Winner %></div>
                                        </div>
                                        <div class="item">
                                            <div class="label">Bắt đầu</div>
                                            <div class="value"><%=StartAt %></div>
                                        </div>
                                        <div class="item">
                                            <div class="label">Kết thúc</div>
                                            <div class="value"><%=EndAt %></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-footer bg-transparent">
                        <div class="btn-list daugia-mobile-btns">
                            <asp:LinkButton ID="butBid" runat="server" CssClass="btn btn-success btn-sm" Visible="false" OnClick="butBid_Click">Đấu giá</asp:LinkButton>
                            <asp:LinkButton ID="butReserve" runat="server" CssClass="btn btn-primary btn-sm" Visible="false" OnClick="butReserve_Click">Mua ngay</asp:LinkButton>
                            <asp:LinkButton ID="butBuyerConfirm" runat="server" CssClass="btn btn-outline-primary btn-sm" Visible="false" OnClick="butBuyerConfirm_Click">Khách xác nhận thanh toán</asp:LinkButton>
                            <asp:LinkButton ID="butSellerConfirm" runat="server" CssClass="btn btn-outline-dark btn-sm" Visible="false" OnClick="butSellerConfirm_Click">Shop xác nhận</asp:LinkButton>
                            <a class="btn btn-outline-secondary btn-sm" href="/daugia">Quay lại</a>
                        </div>
                    </div>
                </div>

                <div class="card daugia-history-card">
                    <div class="card-header">
                        <h3 class="card-title mb-0">Lịch sử lượt đấu giá</h3>
                    </div>
                    <asp:PlaceHolder ID="phBids" runat="server">
                        <div class="table-responsive">
                            <table class="table table-vcenter card-table daugia-bid-table">
                                <thead>
                                    <tr>
                                        <th>Thời gian</th>
                                        <th>Tài khoản</th>
                                        <th>Phí lượt</th>
                                        <th>Giá trước</th>
                                        <th>Giá sau</th>
                                        <th>Trạng thái</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptBids" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# FormatDate(Eval("CreatedAt")) %></td>
                                                <td><%# Eval("BidderAccount") %></td>
                                                <td><%# FormatPoint(Eval("BidFee")) %></td>
                                                <td><%# FormatPoint(Eval("PriceBefore")) %></td>
                                                <td><%# FormatPoint(Eval("PriceAfter")) %></td>
                                                <td><%# Eval("TrangThai") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phBidsEmpty" runat="server" Visible="false">
                        <div class="card-body">
                            <div class="alert alert-secondary mb-0">Phiên này chưa có lượt đấu giá nào.</div>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
