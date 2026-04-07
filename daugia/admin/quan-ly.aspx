<%@ Page Title="Quản lý đấu giá" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="quan-ly.aspx.cs" Inherits="daugia_admin_quan_ly" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .daugia-owner-page {
            padding-bottom: 16px;
        }

        .daugia-owner-stats .stat-value {
            font-size: 20px;
            font-weight: 700;
            line-height: 1.1;
        }

        .daugia-owner-summary-card {
            overflow: hidden;
            position: relative;
        }

        .daugia-owner-summary-card::after {
            content: "";
            position: absolute;
            inset: auto -24px -24px auto;
            width: 88px;
            height: 88px;
            border-radius: 999px;
            background: radial-gradient(circle, rgba(29,78,216,.11) 0%, rgba(29,78,216,0) 70%);
            pointer-events: none;
        }

        .daugia-owner-summary-card .card-body {
            padding: 14px 16px;
        }

        .daugia-owner-stats {
            margin-top: 4px;
        }

        .daugia-owner-list-card {
            overflow: hidden;
        }

        .daugia-owner-list-card .card-header {
            background: linear-gradient(180deg, rgba(29,78,216,.05) 0%, rgba(255,255,255,0) 100%);
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

            .daugia-owner-table th,
            .daugia-owner-table td {
                white-space: nowrap;
                font-size: 12px;
            }

            .daugia-owner-desktop {
                display: none;
            }
        }

        @media (min-width: 768px) {
            .daugia-owner-page {
                padding-bottom: 24px;
            }

            .daugia-owner-stats .stat-value {
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

            .daugia-owner-summary-card .card-body {
                padding: 18px;
            }

            .daugia-owner-table tbody tr:hover {
                background: rgba(29,78,216,.03);
            }

            .daugia-owner-mobile {
                display: none;
            }
        }

        @media (min-width: 768px) {
            .daugia-filter-bar {
                width: auto;
            }

            .daugia-filter-bar .form-control {
                width: 260px;
            }

            .daugia-filter-bar .form-select {
                width: 180px;
            }

            .daugia-filter-bar .btn {
                width: auto;
            }
        }

        @media (min-width: 1400px) {
            .daugia-owner-header .container-xl,
            .daugia-owner-page .container-xl {
                max-width: 1440px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none daugia-owner-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">AhaSale / Đấu giá / Admin Space</div>
                    <h2 class="page-title">Phiên đấu giá của tôi</h2>
                    <div class="text-muted">Quản lý toàn bộ phiên đã tạo từ tài sản nguồn của bạn trong khu vực `/daugia/admin/*`.</div>
                </div>
                <div class="col-12 col-lg-auto ms-lg-auto d-print-none">
                    <div class="btn-list daugia-mobile-btns">
                        <asp:HyperLink ID="lnkCreateAuction" runat="server" CssClass="btn btn-success btn-sm">Tạo phiên đấu giá</asp:HyperLink>
                        <a class="btn btn-outline-secondary btn-sm" href="/daugia/admin">Trung tâm quản lý</a>
                        <a class="btn btn-outline-dark btn-sm" href="/daugia">Xem khu đấu giá public</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body daugia-owner-page">
        <div class="container-xl">
            <div class="row row-cards mb-3 daugia-owner-stats">
                <div class="col-sm-6 col-lg-3">
                    <div class="card daugia-owner-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Chờ duyệt</div>
                            <div class="stat-value"><%=PendingCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3">
                    <div class="card daugia-owner-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Đang diễn ra</div>
                            <div class="stat-value"><%=LiveCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3">
                    <div class="card daugia-owner-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Đã giữ mua</div>
                            <div class="stat-value"><%=ReservedCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3">
                    <div class="card daugia-owner-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Hoàn tất</div>
                            <div class="stat-value"><%=CompletedCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="card daugia-owner-list-card">
                <div class="card-header d-flex align-items-center flex-wrap gap-2">
                    <h3 class="card-title mb-0">Danh sách phiên</h3>
                    <div class="ms-auto d-flex gap-2 flex-wrap daugia-filter-bar">
                        <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control form-control-sm" placeholder="Tìm theo ID, tiêu đề..." />
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select form-select-sm" />
                        <asp:LinkButton ID="butSearch" runat="server" CssClass="btn btn-outline-primary btn-sm" OnClick="butSearch_Click">Lọc</asp:LinkButton>
                    </div>
                </div>

                <asp:PlaceHolder ID="phList" runat="server">
                    <div class="daugia-owner-mobile">
                        <div class="daugia-mobile-list">
                            <asp:Repeater ID="rptAuctionCards" runat="server" OnItemCommand="rptAuctions_ItemCommand">
                                <ItemTemplate>
                                    <article class="daugia-mobile-card">
                                        <div class="d-flex justify-content-between align-items-start gap-2">
                                            <div>
                                                <div class="text-muted small">Phiên #<%# Eval("ID") %></div>
                                                <a class="text-decoration-none fw-semibold d-block mt-1" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>"><%# Eval("SnapshotTitle") %></a>
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
                                            <a class="btn btn-outline-primary btn-sm" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>">Mở</a>
                                            <asp:LinkButton runat="server"
                                                CssClass="btn btn-outline-dark btn-sm"
                                                CommandName="seller_confirm"
                                                CommandArgument='<%# Eval("ID") %>'
                                                OnClientClick="return confirm('Xác nhận bạn đã hoàn tất nghĩa vụ với người mua?');"
                                                Visible='<%# CanShowSellerConfirm(Eval("TrangThai")) %>'>
                                                Xác nhận
                                            </asp:LinkButton>
                                            <asp:LinkButton runat="server"
                                                CssClass="btn btn-outline-danger btn-sm"
                                                CommandName="cancel"
                                                CommandArgument='<%# Eval("ID") %>'
                                                OnClientClick="return confirm('Bạn chắc chắn muốn hủy phiên này?');"
                                                Visible='<%# CanShowSellerCancel(Eval("TrangThai")) %>'>
                                                Hủy
                                            </asp:LinkButton>
                                        </div>
                                    </article>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                    <div class="table-responsive daugia-owner-desktop">
                        <table class="table table-vcenter card-table daugia-owner-table">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Tiêu đề</th>
                                    <th>Trạng thái</th>
                                    <th>Giá hiện tại</th>
                                    <th>Lượt đấu</th>
                                    <th>Kết thúc</th>
                                    <th>Người mua</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptAuctions" runat="server" OnItemCommand="rptAuctions_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("ID") %></td>
                                            <td>
                                                <a class="text-decoration-none fw-semibold" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>"><%# Eval("SnapshotTitle") %></a>
                                            </td>
                                            <td><%# BuildStatusLabel(Eval("TrangThai")) %></td>
                                            <td><%# FormatPoint(Eval("GiaHienTai")) %> Quyền</td>
                                            <td><%# Eval("SoLuotBid") %></td>
                                            <td><%# FormatDate(Eval("PhienKetThuc")) %></td>
                                            <td><%# Eval("WinnerAccount") %></td>
                                            <td>
                                                <div class="btn-list">
                                                    <a class="btn btn-outline-primary btn-sm" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>">Mở</a>
                                                    <asp:LinkButton runat="server"
                                                        CssClass="btn btn-outline-dark btn-sm"
                                                        CommandName="seller_confirm"
                                                        CommandArgument='<%# Eval("ID") %>'
                                                        OnClientClick="return confirm('Xác nhận bạn đã hoàn tất nghĩa vụ với người mua?');"
                                                        Visible='<%# CanShowSellerConfirm(Eval("TrangThai")) %>'>
                                                        Xác nhận
                                                    </asp:LinkButton>
                                                    <asp:LinkButton runat="server"
                                                        CssClass="btn btn-outline-danger btn-sm"
                                                        CommandName="cancel"
                                                        CommandArgument='<%# Eval("ID") %>'
                                                        OnClientClick="return confirm('Bạn chắc chắn muốn hủy phiên này?');"
                                                        Visible='<%# CanShowSellerCancel(Eval("TrangThai")) %>'>
                                                        Hủy
                                                    </asp:LinkButton>
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
                        <div class="alert alert-secondary mb-0">Chưa có phiên nào phù hợp.</div>
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>
</asp:Content>
