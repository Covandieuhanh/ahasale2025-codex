<%@ Page Title="Event Platform" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="event_default" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .event-public-page {
            padding-bottom: 24px;
        }

        .event-summary-value {
            font-size: 24px;
            line-height: 1.1;
            font-weight: 700;
        }

        .event-summary-card .card-body {
            padding: 16px;
        }

        .event-filter-card .card-body {
            padding: 14px;
        }

        .event-program-card {
            border: 1px solid rgba(15, 23, 42, .08);
            transition: transform .16s ease, box-shadow .16s ease, border-color .16s ease;
        }

        .event-program-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 16px 28px rgba(15, 23, 42, .08);
            border-color: rgba(37, 99, 235, .2);
        }

        .event-definition {
            border: 1px dashed rgba(100, 116, 139, .35);
            border-radius: 8px;
            padding: 8px 10px;
            font-size: 12px;
            color: #334155;
            background: rgba(248, 250, 252, .9);
        }

        .event-empty {
            border: 1px dashed rgba(100, 116, 139, .35);
            border-radius: 12px;
            padding: 22px;
            text-align: center;
            color: #64748b;
            background: rgba(248, 250, 252, .9);
        }

        @media (max-width: 767.98px) {
            .event-summary-value {
                font-size: 20px;
            }

            .event-filter-card .btn,
            .event-filter-card .form-select,
            .event-filter-card .form-control {
                width: 100%;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">AhaSale / Event</div>
                    <h2 class="page-title">Không gian chương trình tổng hợp</h2>
                    <div class="text-muted">Hiển thị tất cả chiến dịch đã publish từ Event Platform. Khu xây dựng và vận hành nằm ở <code>/event/admin</code>.</div>
                </div>
                <div class="col-12 col-md-auto ms-md-auto">
                    <a class="btn btn-outline-primary btn-sm" href="/event/admin">Đi tới Event Admin</a>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body event-public-page">
        <div class="container-xl">
            <div class="row row-cards mb-3">
                <div class="col-sm-6 col-lg-3">
                    <div class="card event-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Đã publish</div>
                            <div class="event-summary-value"><%= Summary.TotalCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3">
                    <div class="card event-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Đang chạy</div>
                            <div class="event-summary-value"><%= Summary.ActiveCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3">
                    <div class="card event-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Tạm dừng</div>
                            <div class="event-summary-value"><%= Summary.PausedCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-lg-3">
                    <div class="card event-summary-card">
                        <div class="card-body">
                            <div class="text-muted text-uppercase small">Scope chỉ định</div>
                            <div class="event-summary-value"><%= Summary.SelectedScopeCount.ToString("#,##0") %></div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="card event-filter-card mb-3">
                <div class="card-body">
                    <div class="row g-2 align-items-end">
                        <div class="col-12 col-md-3">
                            <label class="form-label">Trạng thái</label>
                            <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-select form-select-sm" />
                        </div>
                        <div class="col-12 col-md-3">
                            <label class="form-label">Loại chiến dịch</label>
                            <asp:DropDownList ID="ddlTypeFilter" runat="server" CssClass="form-select form-select-sm" />
                        </div>
                        <div class="col-12 col-md-4">
                            <label class="form-label">Tìm kiếm</label>
                            <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control form-control-sm" placeholder="ID, code, tên chiến dịch, người publish..." />
                        </div>
                        <div class="col-6 col-md-1">
                            <asp:LinkButton ID="butSearch" runat="server" CssClass="btn btn-primary btn-sm w-100" OnClick="butSearch_Click">Lọc</asp:LinkButton>
                        </div>
                        <div class="col-6 col-md-1">
                            <a class="btn btn-outline-secondary btn-sm w-100" href="/event">Reset</a>
                        </div>
                    </div>
                </div>
            </div>

            <asp:PlaceHolder ID="phList" runat="server">
                <div class="row row-cards">
                    <asp:Repeater ID="rptPrograms" runat="server">
                        <ItemTemplate>
                            <div class="col-md-6 col-xl-4">
                                <article class="card h-100 event-program-card">
                                    <div class="card-body d-flex flex-column gap-2">
                                        <div class="d-flex flex-wrap gap-1">
                                            <span class='<%# BuildTypeBadgeCss(Eval("CampaignType")) %>'><%# BuildTypeLabel(Eval("CampaignType")) %></span>
                                            <span class='<%# BuildStatusBadgeCss(Eval("Status")) %>'><%# BuildStatusLabel(Eval("Status")) %></span>
                                        </div>
                                        <div class="fw-semibold"><%# Safe(Eval("CampaignName")) %></div>
                                        <div class="text-muted small"><%# Safe(Eval("CampaignCode")) %></div>
                                        <div class="text-muted small"><%# BuildScopeLabel(Eval("ShopScope"), Eval("TargetShopCount")) %></div>
                                        <div class="text-muted small">Thời gian: <%# BuildRangeLabel(Eval("StartAt"), Eval("EndAt")) %></div>
                                        <div class="text-muted small">Phiên bản publish: v<%# Eval("PublishedVersionNo") %></div>
                                        <div class="event-definition"><%# BuildDefinitionSummary(Eval("DefinitionJson"), Eval("CampaignType")) %></div>
                                    </div>
                                    <div class="card-footer bg-transparent">
                                        <div class="text-muted small">Publish bởi: <%# Safe(Eval("PublishedBy")) %></div>
                                        <div class="text-muted small">Publish lúc: <%# FormatDate(Eval("PublishedAt")) %></div>
                                    </div>
                                </article>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
                <div class="event-empty">
                    Không có campaign public phù hợp bộ lọc hiện tại.
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
