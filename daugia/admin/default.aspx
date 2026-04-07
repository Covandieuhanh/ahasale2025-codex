<%@ Page Title="Quản trị đấu giá" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="daugia_admin_default" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .daugia-admin-page {
            padding-bottom: 16px;
        }

        .daugia-admin-stats {
            margin-top: 4px;
        }

        .daugia-admin-stats .stat-value {
            font-size: 20px;
            line-height: 1.1;
            font-weight: 700;
        }

        .daugia-admin-summary-card {
            overflow: hidden;
            position: relative;
        }

        .daugia-admin-summary-card::after {
            content: "";
            position: absolute;
            inset: auto -24px -24px auto;
            width: 92px;
            height: 92px;
            border-radius: 999px;
            background: radial-gradient(circle, rgba(22,163,74,.1) 0%, rgba(22,163,74,0) 70%);
            pointer-events: none;
        }

        .daugia-admin-summary-card .card-body {
            padding: 14px 16px;
        }

        .daugia-admin-list-card {
            overflow: hidden;
        }

        .daugia-admin-list-card .card-header {
            background: linear-gradient(180deg, rgba(22,163,74,.05) 0%, rgba(255,255,255,0) 100%);
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

        .daugia-filter-bar {
            width: 100%;
        }

        .daugia-filter-bar .form-control,
        .daugia-filter-bar .form-select,
        .daugia-filter-bar .btn {
            width: 100%;
        }

        .daugia-mobile-list {
            display: grid;
            gap: 12px;
        }

        .daugia-mobile-card {
            border: 1px solid var(--tblr-border-color, #e2e8f0);
            border-radius: 14px;
            padding: 14px;
            background: var(--tblr-bg-surface, #ffffff);
            box-shadow: 0 10px 24px rgba(16,42,67,.06);
        }

        .daugia-mobile-card__meta {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 10px;
            margin-top: 12px;
        }

        .daugia-mobile-card__item {
            border: 1px solid var(--tblr-border-color, #e2e8f0);
            border-radius: 12px;
            padding: 10px;
            background: #fff;
        }

        .daugia-mobile-card__label {
            font-size: 11px;
            color: var(--tblr-muted, #64748b);
            text-transform: uppercase;
        }

        .daugia-mobile-card__value {
            margin-top: 4px;
            font-weight: 600;
            font-size: 13px;
            line-height: 1.4;
        }

        @media (max-width: 767.98px) {
            .page-header .page-title {
                font-size: 1.35rem;
            }

            .daugia-admin-table th,
            .daugia-admin-table td {
                white-space: nowrap;
                font-size: 12px;
            }

            .daugia-admin-desktop {
                display: none;
            }
        }

        @media (min-width: 768px) {
            .daugia-admin-page {
                padding-bottom: 24px;
            }

            .daugia-admin-stats .stat-value {
                font-size: 22px;
            }

            .daugia-mobile-btns {
                width: auto;
                flex-direction: row;
                flex-wrap: wrap;
            }

            .daugia-mobile-btns .btn {
                width: auto;
            }

            .daugia-admin-summary-card .card-body {
                padding: 18px;
            }

            .daugia-admin-table tbody tr:hover {
                background: rgba(22,163,74,.03);
            }

            .daugia-admin-mobile {
                display: none;
            }
        }

        @media (min-width: 768px) {
            .daugia-filter-bar {
                width: auto;
            }

            .daugia-filter-bar .form-control {
                width: 280px;
            }

            .daugia-filter-bar .form-select {
                width: 180px;
            }

            .daugia-filter-bar .btn {
                width: auto;
            }
        }

        @media (min-width: 1400px) {
            .daugia-system-header .container-xl,
            .daugia-admin-page .container-xl {
                max-width: 1440px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none daugia-system-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Admin / Business Module</div>
                    <h2 class="page-title">Quản trị đấu giá</h2>
                    <div class="text-muted">Kiểm duyệt phiên, kích hoạt, xử lý timeout và tất toán.</div>
                </div>
                <div class="col-12 col-lg-auto ms-lg-auto d-print-none">
                    <div class="btn-list daugia-mobile-btns">
                        <asp:LinkButton ID="butActivateScheduled" runat="server" CssClass="btn btn-primary btn-sm" OnClick="butActivateScheduled_Click">Kích hoạt phiên đã lịch</asp:LinkButton>
                        <asp:LinkButton ID="butRunAuto" runat="server" CssClass="btn btn-outline-dark btn-sm" OnClick="butRunAuto_Click">Chạy Auto Close</asp:LinkButton>
                        <a href="/admin/quan-ly-dau-gia" class="btn btn-outline-secondary btn-sm">Dashboard admin</a>
                        <a href="/daugia" class="btn btn-outline-success btn-sm">Xem public</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body daugia-admin-page">
        <div class="container-xl">
            <div class="row row-cards mb-3 daugia-admin-stats">
                <div class="col-sm-6 col-lg-3 col-xl-2">
                    <div class="card daugia-admin-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Chờ duyệt</div>
                            <div class="stat-value"><%=Summary.PendingCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3 col-xl-2">
                    <div class="card daugia-admin-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Đã lịch</div>
                            <div class="stat-value"><%=Summary.ScheduledCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3 col-xl-2">
                    <div class="card daugia-admin-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Đang live</div>
                            <div class="stat-value"><%=Summary.LiveCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3 col-xl-3">
                    <div class="card daugia-admin-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Chờ tất toán</div>
                            <div class="stat-value"><%=Summary.NeedSettleCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3 col-xl-3">
                    <div class="card daugia-admin-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Lỗi tất toán</div>
                            <div class="stat-value"><%=Summary.FailedCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="card daugia-admin-list-card">
                <div class="card-header d-flex align-items-center flex-wrap gap-2">
                    <h3 class="card-title mb-0">Danh sách phiên đấu giá</h3>
                    <div class="ms-auto d-flex gap-2 flex-wrap daugia-filter-bar">
                        <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control form-control-sm" placeholder="Tìm theo ID, tiêu đề, tài khoản..." />
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select form-select-sm" />
                        <asp:LinkButton ID="butSearch" runat="server" CssClass="btn btn-sm btn-outline-primary" OnClick="butSearch_Click">Lọc</asp:LinkButton>
                    </div>
                </div>

                <asp:PlaceHolder ID="phList" runat="server">
                    <div class="daugia-admin-mobile">
                        <div class="daugia-mobile-list">
                            <asp:Repeater ID="rptAuctionCards" runat="server" OnItemCommand="rptAuctions_ItemCommand">
                                <ItemTemplate>
                                    <article class="daugia-mobile-card">
                                        <div class="d-flex justify-content-between align-items-start gap-2">
                                            <div>
                                                <div class="text-muted small">Phiên #<%# Eval("ID") %></div>
                                                <a class="text-decoration-none fw-semibold d-block mt-1" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>" target="_blank"><%# Eval("SnapshotTitle") %></a>
                                                <div class="text-muted small mt-1">Shop: <%# Eval("SellerAccount") %></div>
                                            </div>
                                            <span class="badge bg-blue-lt text-blue"><%# BuildStatusLabel(Eval("TrangThai")) %></span>
                                        </div>

                                        <div class="daugia-mobile-card__meta">
                                            <div class="daugia-mobile-card__item">
                                                <div class="daugia-mobile-card__label">Giá hiện tại</div>
                                                <div class="daugia-mobile-card__value"><%# FormatPoint(Eval("GiaHienTai")) %> Quyền</div>
                                            </div>
                                            <div class="daugia-mobile-card__item">
                                                <div class="daugia-mobile-card__label">Lượt đấu</div>
                                                <div class="daugia-mobile-card__value"><%# Eval("SoLuotBid") %></div>
                                            </div>
                                            <div class="daugia-mobile-card__item">
                                                <div class="daugia-mobile-card__label">Kết thúc</div>
                                                <div class="daugia-mobile-card__value"><%# FormatDate(Eval("PhienKetThuc")) %></div>
                                            </div>
                                            <div class="daugia-mobile-card__item">
                                                <div class="daugia-mobile-card__label">Người mua</div>
                                                <div class="daugia-mobile-card__value"><%# Eval("WinnerAccount") %></div>
                                            </div>
                                        </div>

                                        <div class="btn-list daugia-mobile-btns mt-3">
                                            <asp:LinkButton runat="server" CssClass="btn btn-outline-success btn-sm" CommandName="approve" CommandArgument='<%# Eval("ID") %>' Visible='<%# CanShowApprove(Eval("TrangThai")) %>'>Duyệt</asp:LinkButton>
                                            <asp:LinkButton runat="server" CssClass="btn btn-outline-danger btn-sm" CommandName="reject" CommandArgument='<%# Eval("ID") %>' Visible='<%# CanShowReject(Eval("TrangThai")) %>'>Từ chối</asp:LinkButton>
                                            <asp:LinkButton runat="server" CssClass="btn btn-outline-primary btn-sm" CommandName="settle" CommandArgument='<%# Eval("ID") %>' Visible='<%# CanShowSettle(Eval("TrangThai")) %>'>Tất toán</asp:LinkButton>
                                        </div>
                                    </article>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                    <div class="table-responsive daugia-admin-desktop">
                        <table class="table table-vcenter card-table daugia-admin-table">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Phiên</th>
                                    <th>Shop</th>
                                    <th>Trạng thái</th>
                                    <th>Giá hiện tại</th>
                                    <th>Kết thúc</th>
                                    <th>Người mua</th>
                                    <th>Thao tác</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptAuctions" runat="server" OnItemCommand="rptAuctions_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("ID") %></td>
                                            <td>
                                                <a class="text-decoration-none fw-semibold" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>" target="_blank"><%# Eval("SnapshotTitle") %></a>
                                                <div class="text-muted small">Bid: <%# Eval("SoLuotBid") %></div>
                                            </td>
                                            <td><%# Eval("SellerAccount") %></td>
                                            <td><%# BuildStatusLabel(Eval("TrangThai")) %></td>
                                            <td><%# FormatPoint(Eval("GiaHienTai")) %></td>
                                            <td><%# FormatDate(Eval("PhienKetThuc")) %></td>
                                            <td><%# Eval("WinnerAccount") %></td>
                                            <td>
                                                <div class="btn-list">
                                                    <asp:LinkButton runat="server" CssClass="btn btn-outline-success btn-sm" CommandName="approve" CommandArgument='<%# Eval("ID") %>' Visible='<%# CanShowApprove(Eval("TrangThai")) %>'>Duyệt</asp:LinkButton>
                                                    <asp:LinkButton runat="server" CssClass="btn btn-outline-danger btn-sm" CommandName="reject" CommandArgument='<%# Eval("ID") %>' Visible='<%# CanShowReject(Eval("TrangThai")) %>'>Từ chối</asp:LinkButton>
                                                    <asp:LinkButton runat="server" CssClass="btn btn-outline-primary btn-sm" CommandName="settle" CommandArgument='<%# Eval("ID") %>' Visible='<%# CanShowSettle(Eval("TrangThai")) %>'>Tất toán</asp:LinkButton>
                                                </div>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
                    <div class="card-body">
                        <div class="alert alert-secondary mb-0">Không có dữ liệu phù hợp bộ lọc.</div>
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>
</asp:Content>
