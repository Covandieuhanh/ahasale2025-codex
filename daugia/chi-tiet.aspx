<%@ Page Title="Chi tiết đấu giá" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="chi-tiet.aspx.cs" Inherits="daugia_chi_tiet" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .daugia-detail-shell {
            padding: 20px 0 40px;
        }

        .daugia-detail-card {
            background: #fff;
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 18px;
            padding: 16px;
            margin-bottom: 12px;
        }

        .daugia-detail-top {
            display: grid;
            grid-template-columns: 1.2fr 1fr;
            gap: 16px;
        }

        .daugia-image {
            width: 100%;
            aspect-ratio: 16/9;
            border-radius: 12px;
            overflow: hidden;
            background: #f1f5f9;
        }

        .daugia-image img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            display: block;
        }

        .daugia-status {
            display: inline-block;
            border-radius: 999px;
            padding: 4px 12px;
            font-size: 12px;
            font-weight: 600;
            background: #dbeafe;
            color: #1d4ed8;
            margin-bottom: 8px;
        }

        .daugia-info {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 10px;
            margin-top: 10px;
        }

        .daugia-info .item {
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 10px;
            padding: 10px 12px;
        }

        .daugia-info .label {
            color: #64748b;
            font-size: 12px;
        }

        .daugia-info .value {
            margin-top: 4px;
            font-size: 15px;
            font-weight: 600;
        }

        .daugia-actions {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
            margin-top: 12px;
        }

        .daugia-bids {
            width: 100%;
            border-collapse: collapse;
        }

        .daugia-bids th,
        .daugia-bids td {
            border-bottom: 1px solid rgba(15, 23, 42, .08);
            padding: 8px;
            font-size: 13px;
        }

        .daugia-empty {
            color: #64748b;
            font-size: 14px;
        }

        @media (max-width: 900px) {
            .daugia-detail-top {
                grid-template-columns: 1fr;
            }

            .daugia-info {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl daugia-detail-shell">
        <asp:PlaceHolder ID="phNotFound" runat="server" Visible="false">
            <div class="daugia-detail-card">
                <h1 class="h5 mb-1">Không tìm thấy phiên đấu giá</h1>
                <p class="text-muted mb-2">Phiên có thể đã bị xóa hoặc không còn hiển thị.</p>
                <a href="/daugia" class="btn btn-outline-primary btn-sm">Quay lại danh sách</a>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phDetail" runat="server">
            <div class="daugia-detail-card">
                <div class="daugia-detail-top">
                    <div>
                        <div class="daugia-image">
                            <img src="<%=ImageUrl %>" alt="Ảnh đấu giá" />
                        </div>
                    </div>
                    <div>
                        <span class="daugia-status"><%=StatusLabel %></span>
                        <h1 class="h4 mb-2"><%=AuctionTitle %></h1>
                        <div class="text-muted mb-2"><%=AuctionDesc %></div>

                        <div class="daugia-info">
                            <div class="item">
                                <div class="label">Giá hiện tại</div>
                                <div class="value text-success"><%=CurrentPrice %> E-AHA</div>
                            </div>
                            <div class="item">
                                <div class="label">Phí mỗi lượt</div>
                                <div class="value"><%=BidFee %> E-AHA</div>
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

                        <div class="daugia-actions">
                            <asp:LinkButton ID="butBid" runat="server" CssClass="btn btn-success btn-sm" Visible="false" OnClick="butBid_Click">Đấu giá</asp:LinkButton>
                            <asp:LinkButton ID="butReserve" runat="server" CssClass="btn btn-primary btn-sm" Visible="false" OnClick="butReserve_Click">Mua ngay</asp:LinkButton>
                            <asp:LinkButton ID="butBuyerConfirm" runat="server" CssClass="btn btn-outline-primary btn-sm" Visible="false" OnClick="butBuyerConfirm_Click">Khách xác nhận thanh toán</asp:LinkButton>
                            <asp:LinkButton ID="butSellerConfirm" runat="server" CssClass="btn btn-outline-dark btn-sm" Visible="false" OnClick="butSellerConfirm_Click">Shop xác nhận</asp:LinkButton>
                            <a class="btn btn-outline-secondary btn-sm" href="/daugia">Quay lại</a>
                        </div>
                    </div>
                </div>
            </div>

            <div class="daugia-detail-card">
                <h2 class="h6 mb-2">Lịch sử lượt đấu giá</h2>
                <asp:PlaceHolder ID="phBids" runat="server">
                    <div class="table-responsive">
                        <table class="daugia-bids">
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
                    <div class="daugia-empty">Phiên này chưa có lượt đấu giá nào.</div>
                </asp:PlaceHolder>
            </div>
        </asp:PlaceHolder>
    </div>
</asp:Content>
