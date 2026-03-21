<%@ Page Title="Phiên đấu giá đã kết thúc" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="da-ket-thuc.aspx.cs" Inherits="daugia_da_ket_thuc" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .daugia-ended-shell {
            padding: 20px 0 40px;
        }

        .daugia-ended-card {
            background: #fff;
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 18px;
            padding: 16px;
        }

        .daugia-ended-list {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 12px;
        }

        .daugia-item {
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 12px;
            padding: 12px;
            background: #fff;
        }

        .daugia-item h3 {
            margin: 0 0 8px;
            font-size: 16px;
            min-height: 44px;
            line-height: 1.35;
        }

        .daugia-meta {
            color: #475569;
            font-size: 13px;
            margin-top: 4px;
        }

        .daugia-price {
            margin-top: 8px;
            font-size: 17px;
            font-weight: 700;
            color: #0f766e;
        }

        .daugia-empty {
            color: #64748b;
            font-size: 14px;
        }

        @media (max-width: 980px) {
            .daugia-ended-list {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }

        @media (max-width: 640px) {
            .daugia-ended-list {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl daugia-ended-shell">
        <div class="daugia-ended-card">
            <div class="d-flex justify-content-between align-items-center mb-2">
                <h1 class="h5 mb-0">Phiên đấu giá đã kết thúc</h1>
                <a href="/daugia" class="btn btn-outline-primary btn-sm">Quay lại danh sách live</a>
            </div>

            <asp:PlaceHolder ID="phList" runat="server">
                <div class="daugia-ended-list">
                    <asp:Repeater ID="rptEnded" runat="server">
                        <ItemTemplate>
                            <article class="daugia-item">
                                <h3>
                                    <a class="text-decoration-none text-dark" href="<%# BuildAuctionUrl(Eval("Slug"), Eval("ID")) %>"><%# Eval("SnapshotTitle") %></a>
                                </h3>
                                <div class="daugia-meta">Shop: <%# Eval("SellerAccount") %></div>
                                <div class="daugia-meta">Trạng thái: <%# BuildStatusLabel(Eval("TrangThai")) %></div>
                                <div class="daugia-meta">Kết thúc: <%# FormatDate(Eval("PhienKetThuc")) %></div>
                                <div class="daugia-meta">Người mua: <%# Eval("WinnerAccount") %></div>
                                <div class="daugia-price"><%# FormatPoint(Eval("GiaHienTai")) %> E-AHA</div>
                            </article>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
                <div class="daugia-empty">Chưa có phiên đấu giá kết thúc.</div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
