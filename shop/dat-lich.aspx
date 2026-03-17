<%@ Page Title="Đặt lịch dịch vụ" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="dat-lich.aspx.cs" Inherits="shop_dat_lich" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="server">
    <style>
        :root {
            --shop-orange: #ee4d2d;
            --shop-orange-soft: #fff1ea;
            --shop-ink: #102a43;
            --shop-muted: #627d98;
            --shop-line: #d9e2ec;
            --shop-bg: #f5f8fb;
            --shop-card: #ffffff;
            --shop-radius: 18px;
        }

        .booking-shell {
            min-height: 100vh;
            padding: 24px 16px 40px;
            background:
                radial-gradient(1000px 280px at 16% -4%, rgba(238,77,45,.20), transparent 65%),
                radial-gradient(1000px 320px at 86% -6%, rgba(255,122,69,.18), transparent 62%),
                var(--shop-bg);
        }

        .booking-wrap {
            max-width: 960px;
            margin: 0 auto;
        }

        .booking-card,
        .booking-hero {
            background: var(--shop-card);
            border: 1px solid var(--shop-line);
            border-radius: var(--shop-radius);
            box-shadow: 0 16px 36px rgba(16, 42, 67, .08);
        }

        .booking-hero {
            padding: 24px;
            margin-bottom: 18px;
        }

        .booking-eyebrow {
            color: var(--shop-orange);
            font-weight: 800;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: .08em;
        }

        .booking-title {
            margin: 10px 0 8px;
            color: var(--shop-ink);
            font-size: 34px;
            line-height: 1.15;
            font-weight: 900;
        }

        .booking-sub {
            color: var(--shop-muted);
            font-size: 15px;
            line-height: 1.6;
            max-width: 720px;
        }

        .booking-meta {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 12px;
            margin-top: 18px;
        }

        .booking-meta-box {
            border: 1px solid var(--shop-line);
            border-radius: 14px;
            padding: 14px 16px;
            background: #fcfdff;
        }

        .booking-meta-label {
            color: var(--shop-muted);
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: .06em;
        }

        .booking-meta-value {
            margin-top: 6px;
            color: var(--shop-ink);
            font-size: 16px;
            font-weight: 800;
        }

        .booking-card {
            padding: 22px;
        }

        .booking-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 16px;
        }

        .booking-field {
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .booking-field-full {
            grid-column: 1 / -1;
        }

        .booking-label {
            color: var(--shop-ink);
            font-size: 14px;
            font-weight: 800;
        }

        .booking-input,
        .booking-select,
        .booking-textarea {
            width: 100%;
            min-height: 44px;
            border-radius: 12px;
            border: 1px solid var(--shop-line);
            padding: 0 14px;
            background: #fff;
            color: var(--shop-ink);
        }

        .booking-textarea {
            min-height: 110px;
            padding: 12px 14px;
            resize: vertical;
        }

        .booking-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            margin-top: 20px;
        }

        .booking-btn {
            min-height: 44px;
            padding: 0 18px;
            border-radius: 12px;
            border: 0;
            font-weight: 800;
            cursor: pointer;
        }

        .booking-btn-primary {
            background: linear-gradient(135deg, #ff9a3d, #ee4d2d);
            color: #fff;
        }

        .booking-btn-secondary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            background: #fff;
            color: var(--shop-ink);
            border: 1px solid var(--shop-line);
            text-decoration: none;
        }

        .booking-alert {
            border-radius: 14px;
            padding: 14px 16px;
            margin-bottom: 16px;
            font-weight: 700;
        }

        .booking-alert-success {
            background: #edfdf4;
            border: 1px solid #b7ebc6;
            color: #137333;
        }

        .booking-alert-warning {
            background: #fff8e1;
            border: 1px solid #ffe08a;
            color: #8a6116;
        }

        @media (max-width: 820px) {
            .booking-meta,
            .booking-grid {
                grid-template-columns: 1fr;
            }

            .booking-title {
                font-size: 28px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
    <div class="booking-shell">
        <div class="booking-wrap">
            <asp:PlaceHolder ID="ph_success" runat="server" Visible="false">
                <div class="booking-alert booking-alert-success">
                    <asp:Label ID="lb_success" runat="server"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="ph_warning" runat="server" Visible="false">
                <div class="booking-alert booking-alert-warning">
                    <asp:Label ID="lb_warning" runat="server"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <section class="booking-hero">
                <div class="booking-eyebrow">Shop Basic Booking</div>
                <h1 class="booking-title">Đặt lịch dịch vụ</h1>
                <div class="booking-sub">
                    Tạo lịch hẹn cơ bản ngay trong <b>/shop</b>. Khi nâng lên Level 2, toàn bộ lịch sử này sẽ tiếp tục dùng lại trong <b>/gianhang/admin</b>.
                </div>

                <div class="booking-meta">
                    <div class="booking-meta-box">
                        <div class="booking-meta-label">Gian hàng</div>
                        <div class="booking-meta-value"><asp:Label ID="lb_shop_name" runat="server" Text="--"></asp:Label></div>
                    </div>
                    <div class="booking-meta-box">
                        <div class="booking-meta-label">Dịch vụ</div>
                        <div class="booking-meta-value"><asp:Label ID="lb_service_name" runat="server" Text="Chọn dịch vụ"></asp:Label></div>
                    </div>
                    <div class="booking-meta-box">
                        <div class="booking-meta-label">Mức sử dụng</div>
                        <div class="booking-meta-value">Level 1 cơ bản</div>
                    </div>
                </div>
            </section>

            <asp:Panel ID="pn_form" runat="server" CssClass="booking-card">
                <div class="booking-grid">
                    <div class="booking-field">
                        <label class="booking-label" for="<%= ddl_dichvu.ClientID %>">Dịch vụ</label>
                        <asp:DropDownList ID="ddl_dichvu" runat="server" CssClass="booking-select" AutoPostBack="true" OnSelectedIndexChanged="ddl_dichvu_SelectedIndexChanged"></asp:DropDownList>
                    </div>

                    <div class="booking-field">
                        <label class="booking-label" for="<%= txt_ngay.ClientID %>">Ngày đặt</label>
                        <asp:TextBox ID="txt_ngay" runat="server" CssClass="booking-input" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="booking-field">
                        <label class="booking-label" for="<%= txt_hoten.ClientID %>">Tên khách hàng</label>
                        <asp:TextBox ID="txt_hoten" runat="server" CssClass="booking-input" MaxLength="80"></asp:TextBox>
                    </div>

                    <div class="booking-field">
                        <label class="booking-label" for="<%= txt_sdt.ClientID %>">Số điện thoại</label>
                        <asp:TextBox ID="txt_sdt" runat="server" CssClass="booking-input" MaxLength="20"></asp:TextBox>
                    </div>

                    <div class="booking-field">
                        <label class="booking-label" for="<%= ddl_gio.ClientID %>">Giờ</label>
                        <asp:DropDownList ID="ddl_gio" runat="server" CssClass="booking-select"></asp:DropDownList>
                    </div>

                    <div class="booking-field">
                        <label class="booking-label" for="<%= ddl_phut.ClientID %>">Phút</label>
                        <asp:DropDownList ID="ddl_phut" runat="server" CssClass="booking-select"></asp:DropDownList>
                    </div>

                    <div class="booking-field booking-field-full">
                        <label class="booking-label" for="<%= txt_ghichu.ClientID %>">Ghi chú</label>
                        <asp:TextBox ID="txt_ghichu" runat="server" CssClass="booking-textarea" TextMode="MultiLine"></asp:TextBox>
                    </div>
                </div>

                <div class="booking-actions">
                    <asp:Button ID="but_datlich" runat="server" CssClass="booking-btn booking-btn-primary" Text="Đặt lịch ngay" OnClientClick="return AhaPreventDoubleClick(this);" OnClick="but_datlich_Click" />
                    <asp:HyperLink ID="lnk_back" runat="server" CssClass="booking-btn booking-btn-secondary" Text="Quay lại"></asp:HyperLink>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
