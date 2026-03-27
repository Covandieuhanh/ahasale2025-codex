<%@ Page Title="Đồng bộ shop công ty" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master"
    AutoEventWireup="true" CodeFile="company-shop-sync.aspx.cs" Inherits="admin_tools_company_shop_sync" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .company-sync-card { max-width: 920px; }
        .company-sync-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 12px;
        }
        .company-sync-metric {
            border: 1px solid #dbe5f1;
            border-radius: 14px;
            padding: 14px 16px;
            background: #f8fbff;
        }
        .company-sync-metric__label {
            color: #64748b;
            font-size: 13px;
            margin-bottom: 6px;
        }
        .company-sync-metric__value {
            font-size: 26px;
            font-weight: 700;
            color: #0f172a;
        }
        .company-sync-note {
            color: #64748b;
        }
        .company-sync-error {
            padding: 14px 16px;
            border-radius: 12px;
            background: #fff1f2;
            color: #9f1239;
            white-space: pre-wrap;
        }
        .company-special-card {
            margin-top: 18px;
            border: 1px solid #dbe5f1;
            border-radius: 16px;
            overflow: hidden;
            background: #fff;
        }
        .company-special-head {
            padding: 16px 18px;
            border-bottom: 1px solid #e2e8f0;
            background: #f8fbff;
        }
        .company-special-list {
            display: flex;
            flex-direction: column;
        }
        .company-special-item {
            display: grid;
            grid-template-columns: minmax(220px, 1.4fr) minmax(260px, 1.3fr) minmax(220px, 1fr);
            gap: 16px;
            padding: 18px;
            border-bottom: 1px solid #eef2f7;
            align-items: start;
        }
        .company-special-item:last-child {
            border-bottom: 0;
        }
        .company-special-name {
            font-size: 17px;
            font-weight: 700;
            color: #0f172a;
            margin-bottom: 6px;
        }
        .company-special-meta {
            color: #64748b;
            font-size: 13px;
            line-height: 1.55;
        }
        .company-special-form {
            display: grid;
            gap: 10px;
        }
        .company-special-row {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 10px;
        }
        .company-special-field {
            display: flex;
            flex-direction: column;
            gap: 6px;
        }
        .company-special-field label {
            font-size: 12px;
            font-weight: 600;
            color: #475569;
        }
        .company-special-state {
            display: flex;
            flex-direction: column;
            gap: 8px;
            align-items: flex-start;
        }
        .company-special-badge {
            display: inline-flex;
            align-items: center;
            padding: 6px 10px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 700;
        }
        .company-special-badge.is-active {
            background: #dcfce7;
            color: #166534;
        }
        .company-special-badge.is-off {
            background: #e2e8f0;
            color: #475569;
        }
        .company-special-empty {
            padding: 18px;
            color: #64748b;
        }
        @media (max-width: 960px) {
            .company-special-item {
                grid-template-columns: 1fr;
            }
            .company-special-row {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="p-3">
                <div class="panel company-sync-card">
                    <div class="panel-header">
                        <h4 class="mb-0">Đồng bộ shop công ty</h4>
                    </div>
                    <div class="panel-content">
                        <p class="company-sync-note">
                            Công cụ này ép khởi tạo <strong>shop công ty</strong>, tạo policy mặc định và mirror sản phẩm hệ thống sang không gian nội bộ của <code>/shop</code>.
                        </p>
                        <div class="mb-3">
                            <asp:Label ID="lbl_status" runat="server" Font-Bold="true"></asp:Label>
                        </div>
                        <asp:Panel ID="pn_config_status" runat="server" Visible="false" CssClass="mb-3">
                            <asp:Label ID="lbl_config_status" runat="server" Font-Bold="true"></asp:Label>
                        </asp:Panel>
                        <div class="d-flex align-items-center mb-3">
                            <asp:Button ID="btn_run" runat="server" CssClass="button success" Text="Chạy đồng bộ ngay" OnClick="btn_run_Click" />
                        </div>
                        <asp:Panel ID="pn_error" runat="server" Visible="false" CssClass="company-sync-error mb-3">
                            <asp:Literal ID="lit_error" runat="server"></asp:Literal>
                        </asp:Panel>
                        <div class="company-sync-grid">
                            <div class="company-sync-metric">
                                <div class="company-sync-metric__label">Tài khoản shop công ty</div>
                                <div class="company-sync-metric__value">
                                    <asp:Literal ID="lit_account" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="company-sync-metric">
                                <div class="company-sync-metric__label">Sản phẩm hệ thống nguồn</div>
                                <div class="company-sync-metric__value">
                                    <asp:Literal ID="lit_system_count" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="company-sync-metric">
                                <div class="company-sync-metric__label">Sản phẩm nội bộ đã mirror</div>
                                <div class="company-sync-metric__value">
                                    <asp:Literal ID="lit_internal_count" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="company-sync-metric">
                                <div class="company-sync-metric__label">Bản ghi map nguồn</div>
                                <div class="company-sync-metric__value">
                                    <asp:Literal ID="lit_map_count" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="company-sync-metric">
                                <div class="company-sync-metric__label">Handler sản phẩm đặc biệt</div>
                                <div class="company-sync-metric__value">
                                    <asp:Literal ID="lit_special_count" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="company-sync-metric">
                                <div class="company-sync-metric__label">Policy % chiết khấu</div>
                                <div class="company-sync-metric__value">
                                    <asp:Literal ID="lit_policy" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="company-sync-metric">
                                <div class="company-sync-metric__label">Quyền không gian</div>
                                <div class="company-sync-metric__value" style="font-size:18px;">
                                    <asp:Literal ID="lit_access" runat="server"></asp:Literal>
                                </div>
                            </div>
                        </div>

                        <div class="company-special-card">
                            <div class="company-special-head">
                                <h5 class="mb-1">Cấu hình sản phẩm đặc biệt</h5>
                                <div class="company-sync-note">
                                    Dùng để bật/tắt handler riêng cho từng sản phẩm của <code>shop_cong_ty</code>. Hiện tại đã hỗ trợ handler <strong>Phát hành thẻ</strong>.
                                </div>
                            </div>
                            <asp:Panel ID="pn_special_empty" runat="server" Visible="false" CssClass="company-special-empty">
                                Chưa có sản phẩm nào trong shop công ty để cấu hình handler.
                            </asp:Panel>
                            <asp:Repeater ID="rpt_special_products" runat="server" OnItemCommand="rpt_special_products_ItemCommand" OnItemDataBound="rpt_special_products_ItemDataBound">
                                <HeaderTemplate>
                                    <div class="company-special-list">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="company-special-item">
                                        <div>
                                            <div class="company-special-name"><%# Eval("ProductName") %></div>
                                            <div class="company-special-meta">
                                                Post ID: <%# Eval("ShopPostId") %><br />
                                                System Product ID: <%# Eval("SystemProductId") %><br />
                                                Không gian: <%# Eval("ProductType") %>
                                            </div>
                                        </div>
                                        <div class="company-special-form">
                                            <asp:HiddenField ID="hf_post_id" runat="server" Value='<%# Eval("ShopPostId") %>' />
                                            <asp:HiddenField ID="hf_system_id" runat="server" Value='<%# Eval("SystemProductId") %>' />
                                            <div class="company-special-row">
                                                <div class="company-special-field">
                                                    <label>Handler</label>
                                                    <asp:DropDownList ID="ddl_handler" runat="server" CssClass="form-control">
                                                        <asp:ListItem Value="">Không áp dụng</asp:ListItem>
                                                        <asp:ListItem Value="issue_card">Phát hành thẻ</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="company-special-field">
                                                    <label>Loại thẻ khi bán</label>
                                                    <asp:DropDownList ID="ddl_card_type" runat="server" CssClass="form-control"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="company-special-row">
                                                <div class="company-special-field">
                                                    <label>Tùy chọn</label>
                                                    <asp:CheckBox ID="chk_reset_pin" runat="server" Text="Reset PIN về `6868` khi phát hành" />
                                                </div>
                                                <div class="company-special-field" style="justify-content:flex-end;">
                                                    <label>&nbsp;</label>
                                                    <asp:Button ID="btn_save_handler" runat="server" Text="Lưu cấu hình" CssClass="button primary" CommandName="save_handler" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="company-special-state">
                                            <asp:Literal ID="lit_handler_badge" runat="server"></asp:Literal>
                                            <div class="company-sync-note">
                                                <asp:Literal ID="lit_handler_note" runat="server"></asp:Literal>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </div>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
