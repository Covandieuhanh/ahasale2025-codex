<%@ Page Title="Quản trị đấu giá" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="daugia_admin_default" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .daugia-admin-shell {
            padding: 20px 0 40px;
        }

        .daugia-admin-card {
            border-radius: 20px;
            padding: 18px;
            background: #fff;
            border: 1px solid rgba(15, 23, 42, .08);
            box-shadow: 0 18px 40px rgba(15, 23, 42, .08);
            margin-bottom: 14px;
        }

        .daugia-admin-stats {
            display: grid;
            grid-template-columns: repeat(5, minmax(0, 1fr));
            gap: 10px;
            margin-top: 12px;
        }

        .daugia-admin-stat {
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 12px;
            padding: 10px 12px;
            background: #fff;
        }

        .daugia-admin-stat .label {
            color: #64748b;
            font-size: 12px;
        }

        .daugia-admin-stat .value {
            margin-top: 4px;
            font-size: 19px;
            font-weight: 700;
        }

        .daugia-admin-table {
            width: 100%;
            border-collapse: collapse;
        }

        .daugia-admin-table th,
        .daugia-admin-table td {
            border-bottom: 1px solid rgba(15, 23, 42, .08);
            padding: 9px 8px;
            vertical-align: top;
            font-size: 13px;
        }

        .daugia-admin-table th {
            color: #475569;
            font-weight: 600;
            white-space: nowrap;
        }

        .daugia-actions {
            display: flex;
            gap: 6px;
            flex-wrap: wrap;
        }

        .daugia-actions .btn {
            padding: 2px 10px;
            font-size: 12px;
        }

        .daugia-empty {
            color: #64748b;
            font-size: 14px;
        }

        @media (max-width: 980px) {
            .daugia-admin-stats {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl daugia-admin-shell">
        <div class="daugia-admin-card">
            <div class="text-muted small">Admin / Business Module</div>
            <h1 class="h4 mb-1">Quản trị Đấu Giá</h1>
            <div class="text-muted">Kiểm duyệt phiên, kích hoạt, xử lý timeout và tất toán.</div>
            <div class="d-flex gap-2 flex-wrap mt-2">
                <asp:LinkButton ID="butActivateScheduled" runat="server" CssClass="btn btn-primary btn-sm" OnClick="butActivateScheduled_Click">Kích hoạt phiên đã lịch</asp:LinkButton>
                <asp:LinkButton ID="butRunAuto" runat="server" CssClass="btn btn-outline-dark btn-sm" OnClick="butRunAuto_Click">Chạy Auto Close</asp:LinkButton>
                <a href="/daugia" class="btn btn-outline-success btn-sm">Xem public</a>
            </div>

            <div class="daugia-admin-stats">
                <div class="daugia-admin-stat"><div class="label">Chờ duyệt</div><div class="value"><%=Summary.PendingCount.ToString("#,##0") %></div></div>
                <div class="daugia-admin-stat"><div class="label">Đã lịch</div><div class="value"><%=Summary.ScheduledCount.ToString("#,##0") %></div></div>
                <div class="daugia-admin-stat"><div class="label">Đang live</div><div class="value"><%=Summary.LiveCount.ToString("#,##0") %></div></div>
                <div class="daugia-admin-stat"><div class="label">Chờ tất toán</div><div class="value"><%=Summary.NeedSettleCount.ToString("#,##0") %></div></div>
                <div class="daugia-admin-stat"><div class="label">Lỗi tất toán</div><div class="value"><%=Summary.FailedCount.ToString("#,##0") %></div></div>
            </div>
        </div>

        <div class="daugia-admin-card">
            <div class="d-flex justify-content-between align-items-center mb-2">
                <h2 class="h6 mb-0">Danh sách phiên đấu giá</h2>
                <div class="d-flex gap-2">
                    <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control form-control-sm" placeholder="Tìm theo ID, tiêu đề, tài khoản..." />
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select form-select-sm" />
                    <asp:LinkButton ID="butSearch" runat="server" CssClass="btn btn-sm btn-outline-primary" OnClick="butSearch_Click">Lọc</asp:LinkButton>
                </div>
            </div>

            <asp:PlaceHolder ID="phList" runat="server">
                <div class="table-responsive">
                    <table class="daugia-admin-table">
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
                                            <div class="daugia-actions">
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
                <div class="daugia-empty">Không có dữ liệu phù hợp bộ lọc.</div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
