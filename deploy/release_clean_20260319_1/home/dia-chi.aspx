<%@ Page Title="Địa chỉ của bạn" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="dia-chi.aspx.cs" Inherits="home_dia_chi" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head_truoc" runat="Server">
    <style>
        :root {
            --addr-bg: #f6f8fc;
            --addr-card: #ffffff;
            --addr-border: rgba(98, 105, 118, 0.2);
            --addr-text: #182433;
            --addr-muted: #64748b;
            --addr-primary: #ff5b2e;
            --addr-success: #16a34a;
            --addr-success-soft: rgba(22, 163, 74, 0.12);
            --addr-danger: #d63939;
        }

        html[data-bs-theme="dark"] {
            --addr-bg: #0b1220;
            --addr-card: #0f172a;
            --addr-border: #223246;
            --addr-text: #e5e7eb;
            --addr-muted: #94a3b8;
            --addr-primary: #ff7a47;
            --addr-success: #22c55e;
            --addr-success-soft: rgba(34, 197, 94, 0.14);
            --addr-danger: #f87171;
        }

        .address-page {
            background: var(--addr-bg);
            border: 1px solid var(--addr-border);
            border-radius: 20px;
            padding: 16px;
        }

        .address-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            margin-bottom: 16px;
        }

        .address-title {
            font-size: 1.4rem;
            font-weight: 700;
            color: var(--addr-text);
        }

        .address-grid {
            display: grid;
            grid-template-columns: minmax(0, 1.15fr) minmax(0, 1fr);
            gap: 14px;
        }

        .address-card {
            border: 1px solid var(--addr-border);
            border-radius: 16px;
            background: var(--addr-card);
            box-shadow: 0 10px 24px rgba(15, 23, 42, 0.06);
            overflow: hidden;
        }

        .address-card .card-header {
            padding: 12px 14px;
            border-bottom: 1px solid var(--addr-border);
            font-weight: 700;
        }

        .address-card .card-body {
            padding: 14px;
        }

        .addr-row {
            display: flex;
            gap: 12px;
            justify-content: space-between;
            padding: 12px;
            border: 1px solid var(--addr-border);
            border-radius: 12px;
            margin-bottom: 10px;
        }

        .addr-row:last-child {
            margin-bottom: 0;
        }

        .addr-row-title {
            font-weight: 600;
            color: var(--addr-text);
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .addr-row-text {
            color: var(--addr-muted);
            white-space: pre-line;
            font-size: 0.95rem;
        }

        .addr-badge {
            display: inline-flex;
            align-items: center;
            padding: 2px 8px;
            border-radius: 999px;
            font-size: 11px;
            font-weight: 700;
            background: var(--addr-success-soft);
            color: var(--addr-success);
        }

        .addr-actions {
            display: flex;
            flex-direction: column;
            gap: 6px;
            align-items: flex-end;
        }

        .addr-action {
            border: none;
            background: transparent;
            font-size: 0.85rem;
            font-weight: 600;
            text-decoration: underline;
            cursor: pointer;
        }

        .addr-action.default {
            color: var(--addr-primary);
        }

        .addr-action.delete {
            color: var(--addr-danger);
        }

        .addr-empty {
            color: var(--addr-muted);
            font-style: italic;
        }

        @media (max-width: 991px) {
            .address-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl py-3">
        <div class="address-page">
            <div class="address-header">
                <div class="address-title">Địa chỉ của bạn</div>
                <a class="btn btn-outline-primary" href="/home/taikhoan.aspx">Quay lại</a>
            </div>

            <div class="address-grid">
                <div class="address-card">
                    <div class="card-header">Danh sách địa chỉ</div>
                    <div class="card-body">
                        <asp:Repeater ID="rpt_address_book" runat="server" OnItemCommand="AddressBook_ItemCommand">
                            <ItemTemplate>
                                <div class="addr-row">
                                    <div>
                                        <div class="addr-row-title">
                                            <asp:Label runat="server" Text='<%# Eval("DisplayTitle") %>'></asp:Label>
                                            <asp:Label runat="server" CssClass="addr-badge" Text="Mặc định"
                                                Visible='<%# (Eval("IsDefault") != null && Convert.ToBoolean(Eval("IsDefault"))) %>'></asp:Label>
                                        </div>
                                        <div class="addr-row-text">
                                            <asp:Label runat="server" Text='<%# Eval("DisplayAddress") %>'></asp:Label>
                                        </div>
                                    </div>
                                    <div class="addr-actions">
                                        <asp:LinkButton runat="server" CssClass="addr-action default" CommandName="set-default" CommandArgument='<%# Eval("Id") %>'
                                            Visible='<%# !(Eval("IsDefault") != null && Convert.ToBoolean(Eval("IsDefault"))) %>'>Đặt mặc định</asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="addr-action" CommandName="edit" CommandArgument='<%# Eval("Id") %>'>Sửa</asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="addr-action delete" CommandName="delete" CommandArgument='<%# Eval("Id") %>'
                                            OnClientClick="return confirm('Xoá địa chỉ này?');">Xoá</asp:LinkButton>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Label ID="lbl_address_empty" runat="server" CssClass="addr-empty" Visible="false">Chưa có địa chỉ nào.</asp:Label>
                    </div>
                </div>

                <div class="address-card">
                    <div class="card-header">Thêm / Sửa địa chỉ</div>
                    <div class="card-body">
                        <asp:HiddenField ID="hf_edit_id" runat="server" />
                        <div class="mb-3">
                            <label class="form-label">Người nhận</label>
                            <asp:TextBox ID="txt_hoten" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Điện thoại</label>
                            <asp:TextBox ID="txt_sdt" runat="server" CssClass="form-control" inputmode="numeric"></asp:TextBox>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Tỉnh/Thành - Quận/Huyện - Phường/Xã</label>
                            <div class="row g-2">
                                <div class="col-md-4">
                                    <select id="addr_tinh" class="form-select"></select>
                                </div>
                                <div class="col-md-4">
                                    <select id="addr_quan" class="form-select"></select>
                                </div>
                                <div class="col-md-4">
                                    <select id="addr_phuong" class="form-select"></select>
                                </div>
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Địa chỉ chi tiết</label>
                            <asp:TextBox ID="txt_diachi_chitiet" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="mb-3">
                            <asp:CheckBox ID="chk_default" runat="server" Text="Đặt làm mặc định" />
                        </div>
                        <div class="d-flex gap-2 flex-wrap">
                            <asp:Button ID="btn_save" runat="server" CssClass="btn btn-primary" Text="Lưu địa chỉ" OnClick="btn_save_Click" />
                            <asp:Button ID="btn_reset" runat="server" CssClass="btn btn-outline-secondary" Text="Làm mới" OnClick="btn_reset_Click" />
                        </div>

                        <asp:HiddenField ID="hf_tinh" runat="server" />
                        <asp:HiddenField ID="hf_quan" runat="server" />
                        <asp:HiddenField ID="hf_phuong" runat="server" />
                        <asp:HiddenField ID="hf_address_raw" runat="server" />
                        <asp:TextBox ID="txt_diachi_full" runat="server" CssClass="form-control d-none"></asp:TextBox>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentFoot" ContentPlaceHolderID="foot_sau" runat="Server">
    <script src="<%= Helper_cl.VersionedUrl("~/js/aha-address-picker.js") %>"></script>
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            if (window.AhaAddressPicker) {
                window.AhaAddressPicker.init({
                    provinceSelectId: "addr_tinh",
                    districtSelectId: "addr_quan",
                    wardSelectId: "addr_phuong",
                    detailInputId: "<%= txt_diachi_chitiet.ClientID %>",
                    hiddenAddressId: "<%= txt_diachi_full.ClientID %>",
                    hiddenProvinceId: "<%= hf_tinh.ClientID %>",
                    hiddenDistrictId: "<%= hf_quan.ClientID %>",
                    hiddenWardId: "<%= hf_phuong.ClientID %>",
                    rawAddressId: "<%= hf_address_raw.ClientID %>"
                });
            }
        });
    </script>
</asp:Content>
