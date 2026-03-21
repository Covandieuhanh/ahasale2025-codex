<%@ Page Title="Quản lý đấu giá" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="quan-ly.aspx.cs" Inherits="daugia_quan_ly" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .daugia-owner-shell {
            padding: 20px 0 40px;
        }

        .daugia-owner-card {
            background: #fff;
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 18px;
            padding: 16px;
            margin-bottom: 12px;
        }

        .daugia-owner-stats {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 10px;
            margin-top: 10px;
        }

        .daugia-owner-stat {
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 10px;
            padding: 10px;
            background: #fff;
        }

        .daugia-owner-stat .label {
            color: #64748b;
            font-size: 12px;
        }

        .daugia-owner-stat .value {
            margin-top: 4px;
            font-size: 18px;
            font-weight: 700;
        }

        .daugia-owner-table {
            width: 100%;
            border-collapse: collapse;
        }

        .daugia-owner-table th,
        .daugia-owner-table td {
            border-bottom: 1px solid rgba(15, 23, 42, .08);
            padding: 8px;
            font-size: 13px;
            vertical-align: top;
        }

        .daugia-empty {
            color: #64748b;
            font-size: 14px;
        }

        @media (max-width: 980px) {
            .daugia-owner-stats {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl daugia-owner-shell">
        <div class="daugia-owner-card">
            <div class="text-muted small">AhaSale / Đấu giá</div>
            <h1 class="h5 mb-1">Phiên Đấu Giá Của Tôi</h1>
            <div class="text-muted">Quản lý toàn bộ phiên đã tạo từ tài sản nguồn của bạn.</div>
            <div class="d-flex gap-2 flex-wrap mt-2">
                <asp:HyperLink ID="lnkCreateAuction" runat="server" CssClass="btn btn-success btn-sm">Tạo phiên đấu giá</asp:HyperLink>
                <asp:HyperLink ID="lnkManagePost" runat="server" CssClass="btn btn-outline-primary btn-sm">Quản lý tài sản nguồn</asp:HyperLink>
                <a class="btn btn-outline-dark btn-sm" href="/daugia">Xem khu đấu giá public</a>
            </div>

            <div class="daugia-owner-stats">
                <div class="daugia-owner-stat"><div class="label">Chờ duyệt</div><div class="value"><%=PendingCount.ToString("#,##0") %></div></div>
                <div class="daugia-owner-stat"><div class="label">Đang diễn ra</div><div class="value"><%=LiveCount.ToString("#,##0") %></div></div>
                <div class="daugia-owner-stat"><div class="label">Đã giữ mua</div><div class="value"><%=ReservedCount.ToString("#,##0") %></div></div>
                <div class="daugia-owner-stat"><div class="label">Hoàn tất</div><div class="value"><%=CompletedCount.ToString("#,##0") %></div></div>
            </div>
        </div>

        <div class="daugia-owner-card">
            <div class="d-flex justify-content-between align-items-center mb-2 flex-wrap gap-2">
                <h2 class="h6 mb-0">Danh sách phiên</h2>
                <div class="d-flex gap-2">
                    <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control form-control-sm" placeholder="Tìm theo ID, tiêu đề..." />
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select form-select-sm" />
                    <asp:LinkButton ID="butSearch" runat="server" CssClass="btn btn-outline-primary btn-sm" OnClick="butSearch_Click">Lọc</asp:LinkButton>
                </div>
            </div>

            <asp:PlaceHolder ID="phList" runat="server">
                <div class="table-responsive">
                    <table class="daugia-owner-table">
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
                            <asp:Repeater ID="rptAuctions" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("ID") %></td>
                                        <td>
                                            <a class="text-decoration-none fw-semibold" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>"><%# Eval("SnapshotTitle") %></a>
                                        </td>
                                        <td><%# BuildStatusLabel(Eval("TrangThai")) %></td>
                                        <td><%# FormatPoint(Eval("GiaHienTai")) %> E-AHA</td>
                                        <td><%# Eval("SoLuotBid") %></td>
                                        <td><%# FormatDate(Eval("PhienKetThuc")) %></td>
                                        <td><%# Eval("WinnerAccount") %></td>
                                        <td>
                                            <a class="btn btn-outline-primary btn-sm" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>">Mở</a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
                <div class="daugia-empty">Chưa có phiên nào phù hợp.</div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
