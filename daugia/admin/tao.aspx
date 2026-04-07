<%@ Page Title="Tạo phiên đấu giá" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="tao.aspx.cs" Inherits="daugia_admin_tao" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .daugia-create-page {
            padding-bottom: 16px;
        }

        .daugia-form-grid {
            display: grid;
            grid-template-columns: 1fr;
            gap: 12px;
        }

        .daugia-create-summary {
            display: grid;
            grid-template-columns: 1fr;
            gap: 12px;
            margin-bottom: 16px;
        }

        .daugia-create-summary .summary-card {
            border: 1px solid var(--tblr-border-color, #e2e8f0);
            border-radius: 14px;
            padding: 14px;
            background: var(--tblr-bg-surface, #ffffff);
        }

        .daugia-create-summary .summary-label {
            font-size: 12px;
            color: var(--tblr-muted, #64748b);
            text-transform: uppercase;
            letter-spacing: .02em;
        }

        .daugia-create-summary .summary-value {
            margin-top: 6px;
            font-size: 20px;
            line-height: 1.1;
            font-weight: 700;
        }

        .daugia-create-summary .summary-note {
            margin-top: 6px;
            font-size: 13px;
            color: var(--tblr-muted, #64748b);
        }

        .daugia-create-card {
            overflow: hidden;
        }

        .daugia-create-card .card-body {
            padding: 14px;
        }

        .daugia-form-grid .full {
            grid-column: 1 / -1;
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

        @media (max-width: 767.98px) {
            .page-header .page-title {
                font-size: 1.35rem;
            }
        }

        @media (min-width: 768px) {
            .daugia-create-page {
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

            .daugia-create-card .card-body {
                padding: 20px;
            }
        }

        @media (min-width: 768px) {
            .daugia-create-summary {
                grid-template-columns: repeat(3, minmax(0, 1fr));
            }

            .daugia-form-grid {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }

        @media (min-width: 1400px) {
            .daugia-create-header .container-xl,
            .daugia-create-page .container-xl {
                max-width: 1360px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none daugia-create-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">AhaSale / Đấu giá / Admin Space</div>
                    <h2 class="page-title">Tạo phiên đấu giá mới</h2>
                    <div class="text-muted">Phiên sẽ bị giữ cọc theo cấu hình module và chuyển trạng thái chờ admin duyệt.</div>
                </div>
                <div class="col-12 col-md-auto ms-md-auto d-print-none">
                    <div class="btn-list daugia-mobile-btns">
                        <a class="btn btn-outline-secondary btn-sm" href="/daugia/admin/quan-ly">Quay lại quản lý</a>
                        <a class="btn btn-outline-dark btn-sm" href="/daugia/admin">Trung tâm quản lý</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body daugia-create-page">
        <div class="container-xl">
            <div class="daugia-create-summary">
                <section class="summary-card">
                    <div class="summary-label">Quyền ưu đãi hiện có</div>
                    <div class="summary-value text-primary"><%=SellerUuDaiBalance %></div>
                    <div class="summary-note">Sẽ bị trừ trước khi dùng đến quyền tiêu dùng.</div>
                </section>
                <section class="summary-card">
                    <div class="summary-label">Quyền tiêu dùng hiện có</div>
                    <div class="summary-value text-success"><%=SellerTieuDungBalance %></div>
                    <div class="summary-note">Tổng quyền khả dụng hiện tại: <strong><%=SellerTotalRights %></strong>.</div>
                </section>
                <section class="summary-card">
                    <div class="summary-label">Cọc tạo phiên</div>
                    <div class="summary-value"><span id="dgDepositPreview"><%=DepositPreview %></span> Quyền</div>
                    <div class="summary-note">Tỷ lệ cọc hiện tại là <strong><%=DepositPercentText %>%</strong> trên giá niêm yết.</div>
                </section>
            </div>

            <div class="card daugia-create-card">
                <div class="card-body">
                    <div class="daugia-form-grid">
                        <div class="full">
                            <label class="form-label">Tiêu đề phiên</label>
                            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" />
                        </div>

                        <div class="full">
                            <label class="form-label">Mô tả</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
                        </div>

                        <div class="full">
                            <label class="form-label">Ảnh đại diện (URL)</label>
                            <asp:TextBox ID="txtImage" runat="server" CssClass="form-control" />
                        </div>

                        <div>
                            <label class="form-label">Loại tài sản nguồn</label>
                            <asp:DropDownList ID="ddlSourceType" runat="server" CssClass="form-select">
                                <asp:ListItem Value="manual_asset">manual_asset</asp:ListItem>
                                <asp:ListItem Value="shop_post">shop_post</asp:ListItem>
                                <asp:ListItem Value="home_quyen_uu_dai">home_quyen_uu_dai</asp:ListItem>
                                <asp:ListItem Value="home_quyen_tieu_dung">home_quyen_tieu_dung</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div>
                            <label class="form-label">ID tài sản nguồn</label>
                            <asp:TextBox ID="txtSourceID" runat="server" CssClass="form-control" />
                            <div class="form-text">Có thể để trống khi dùng `manual_asset`.</div>
                        </div>

                        <div>
                            <label class="form-label">Giá niêm yết (Quyền)</label>
                            <asp:TextBox ID="txtGiaNiemYet" runat="server" CssClass="form-control" />
                        </div>

                        <div>
                            <label class="form-label">Phí mỗi lượt (Quyền)</label>
                            <asp:TextBox ID="txtPhiLuot" runat="server" CssClass="form-control" />
                        </div>

                        <div>
                            <label class="form-label">Bắt đầu</label>
                            <asp:TextBox ID="txtStartAt" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
                        </div>

                        <div>
                            <label class="form-label">Kết thúc</label>
                            <asp:TextBox ID="txtEndAt" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
                        </div>

                        <div>
                            <label class="form-label">Settlement mode</label>
                            <asp:DropDownList ID="ddlSettlementMode" runat="server" CssClass="form-select">
                                <asp:ListItem Value="manual_fulfillment">manual_fulfillment</asp:ListItem>
                                <asp:ListItem Value="system_transfer">system_transfer</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div>
                            <label class="form-label">Scope người bán</label>
                            <asp:DropDownList ID="ddlSellerScope" runat="server" CssClass="form-select">
                                <asp:ListItem Value="shop">shop</asp:ListItem>
                                <asp:ListItem Value="home">home</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="full">
                            <div class="btn-list daugia-mobile-btns">
                                <asp:LinkButton ID="butCreate" runat="server" CssClass="btn btn-success" OnClick="butCreate_Click">Tạo phiên đấu giá</asp:LinkButton>
                                <a class="btn btn-outline-secondary" href="/daugia/admin/quan-ly">Quay lại quản lý</a>
                                <a class="btn btn-outline-dark" href="/daugia/admin">Trung tâm quản lý</a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        (function () {
            function parseValue(raw) {
                var normalized = (raw || '').replace(/,/g, '').trim();
                var value = parseFloat(normalized);
                return isNaN(value) ? 0 : value;
            }

            function formatValue(value) {
                return value.toLocaleString('vi-VN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            }

            function bindDepositPreview() {
                var giaInput = document.getElementById('<%=txtGiaNiemYet.ClientID %>');
                var depositNode = document.getElementById('dgDepositPreview');
                if (!giaInput || !depositNode)
                    return;

                var depositPercent = parseFloat('<%=DepositPercentJs %>') || 0;
                var update = function () {
                    var gia = parseValue(giaInput.value);
                    var deposit = Math.max(0, gia) * depositPercent / 100;
                    depositNode.textContent = formatValue(deposit);
                };

                giaInput.addEventListener('input', update);
                update();
            }

            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', bindDepositPreview);
            } else {
                bindDepositPreview();
            }
        })();
    </script>
</asp:Content>
