<%@ Page Language="C#" AutoEventWireup="true" CodeFile="cho-thanh-toan.aspx.cs" Inherits="home_cho_thanh_toan" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AHASALE.VN</title>
    <meta charset="UTF-8" />
    <meta http-equiv="content-language" content="vi" />
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=yes" />

    <link href="/Metro-UI-CSS-master/css/metro-all.min.css" rel="stylesheet" />
    <link href="/assetscss/home-style.css" rel="stylesheet" />
    <link href="/assetscss/login.css?v=2026-03-02.1" rel="stylesheet" />
    <link href="/assetscss/aha-ui-refresh.css?v=2026-03-02.1" rel="stylesheet" />
    <link href="/assetscss/bcorn-with-metro.css" rel="stylesheet" />
    <link href="/assetscss/fix-metro.css" rel="stylesheet" />

    <style>
        :root {
            --pay-bg-1: #0b1220;
            --pay-bg-2: #111b30;
            --pay-card: #121a2c;
            --pay-card-2: #0f1728;
            --pay-border: rgba(148, 163, 184, 0.2);
            --pay-text: #f1f5f9;
            --pay-muted: #c4d2e5;
            --pay-accent: #38bdf8;
            --pay-success: #22c55e;
            --pay-warn: #f59e0b;
            --pay-danger: #ef4444;
        }

        html, body {
            min-height: 100%;
            margin: 0;
        }

        .body-login-bcorn1 {
            background:
                radial-gradient(circle at 12% 8%, rgba(56, 189, 248, 0.18), transparent 35%),
                radial-gradient(circle at 78% 2%, rgba(59, 130, 246, 0.12), transparent 32%),
                linear-gradient(160deg, var(--pay-bg-1), var(--pay-bg-2));
            color: var(--pay-text);
            overflow-x: hidden;
            overflow-y: auto;
            -webkit-overflow-scrolling: touch;
        }

        .pay-shell,
        .pay-shell * {
            color: inherit;
        }

        .pay-shell {
            width: min(1180px, 100% - 28px);
            margin: 22px auto 36px;
        }

        .pay-hero {
            border: 1px solid var(--pay-border);
            border-radius: 24px;
            background: linear-gradient(135deg, rgba(15, 23, 42, 0.96), rgba(17, 24, 39, 0.88));
            box-shadow: 0 22px 60px rgba(2, 6, 23, 0.45);
            padding: 24px 24px 18px;
            margin-bottom: 16px;
            color: var(--pay-text) !important;
        }

        .pay-hero-top {
            display: flex;
            align-items: center;
            gap: 14px;
            flex-wrap: wrap;
        }

        .pay-logo {
            width: 56px;
            height: 56px;
            border-radius: 14px;
            object-fit: cover;
            border: 1px solid rgba(148, 163, 184, 0.28);
            background: rgba(15, 23, 42, 0.5);
            padding: 6px;
        }

        .pay-title {
            margin: 0;
            font-size: 28px;
            line-height: 1.18;
            font-weight: 800;
            letter-spacing: 0.2px;
            color: #f8fafc !important;
            text-shadow: 0 1px 0 rgba(2, 6, 23, 0.6);
        }

        .pay-sub {
            margin-top: 10px;
            color: var(--pay-muted);
            font-size: 13px;
            line-height: 1.52;
            color: #d3deee !important;
        }

        .pay-tag {
            display: inline-flex;
            align-items: center;
            border: 1px solid rgba(56, 189, 248, 0.35);
            border-radius: 999px;
            padding: 6px 12px;
            background: rgba(56, 189, 248, 0.12);
            color: #7dd3fc;
            font-size: 13px;
            font-weight: 700;
            color: #bfe7ff !important;
        }

        .pay-hero-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            margin-top: 14px;
        }

        .pay-grid {
            display: grid;
            grid-template-columns: minmax(0, 0.95fr) minmax(0, 1.45fr);
            gap: 14px;
        }

        .pay-card {
            border: 1px solid var(--pay-border);
            border-radius: 20px;
            background: linear-gradient(155deg, rgba(18, 26, 44, 0.96), rgba(15, 23, 40, 0.95));
            box-shadow: 0 16px 40px rgba(2, 6, 23, 0.35);
            padding: 18px;
            color: var(--pay-text) !important;
            position: relative;
            z-index: 1;
        }

        .pay-card + .pay-card {
            margin-top: 12px;
        }

        .pay-card-title {
            margin: 0;
            font-size: 17px;
            font-weight: 800;
            letter-spacing: 0.2px;
            color: #eaf2ff !important;
        }

        .pay-note {
            margin-top: 8px;
            color: var(--pay-muted);
            font-size: 13px;
            line-height: 1.45;
            color: #c9d6e8 !important;
        }

        .pay-amount {
            margin-top: 12px;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 8px 12px;
            border-radius: 12px;
            border: 1px solid rgba(34, 197, 94, 0.4);
            background: rgba(34, 197, 94, 0.14);
            color: #86efac;
            font-size: 20px;
            font-weight: 800;
            letter-spacing: 0.2px;
            color: #bbf7d0 !important;
        }

        .pay-actions {
            display: flex;
            gap: 8px;
            margin-top: 14px;
            flex-wrap: wrap;
            position: relative;
            z-index: 3;
        }

        .pay-btn,
        .pay-btn:visited {
            appearance: none;
            border: 1px solid transparent;
            border-radius: 12px;
            min-height: 42px;
            padding: 0 14px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 14px;
            font-weight: 700;
            text-decoration: none;
            cursor: pointer;
            transition: all .2s ease;
            color: #fff;
            background: rgba(51, 65, 85, 0.55);
            user-select: none;
        }

        .pay-btn:hover {
            transform: translateY(-1px);
        }

        .pay-btn-danger {
            background: linear-gradient(145deg, #ef4444, #dc2626);
            border-color: rgba(248, 113, 113, 0.45);
            color: #fff !important;
        }

        .pay-btn-primary {
            background: linear-gradient(145deg, #0ea5e9, #0284c7);
            border-color: rgba(56, 189, 248, 0.45);
            color: #fff !important;
        }

        .pay-btn-success {
            background: linear-gradient(145deg, #22c55e, #16a34a);
            border-color: rgba(74, 222, 128, 0.45);
            color: #fff !important;
        }

        .pay-btn-muted {
            background: rgba(51, 65, 85, 0.45);
            border-color: rgba(148, 163, 184, 0.3);
            color: #cbd5e1 !important;
        }

        .pay-pin-input {
            display: flex;
            align-items: center;
            width: 100%;
            min-height: 50px;
            border: 1px solid rgba(148, 163, 184, 0.28);
            border-radius: 14px;
            background: rgba(248, 250, 252, 0.98);
            overflow: hidden;
            margin-top: 10px;
        }

        .pay-pin-input .pin-icon {
            flex: 0 0 50px;
            text-align: center;
            color: #0f172a;
            font-size: 18px;
            line-height: 50px;
            border-right: 1px solid rgba(15, 23, 42, 0.12);
        }

        .pay-pin-input .pay-pin-control {
            width: 100%;
            min-width: 0;
            height: 50px !important;
            border: 0 !important;
            outline: 0;
            box-shadow: none !important;
            background: transparent !important;
            color: #0f172a;
            padding: 0 14px !important;
            border-radius: 0 !important;
            font-size: 16px;
            font-weight: 600;
            letter-spacing: 0.2px;
        }

        .pay-pin-input .pay-pin-control::placeholder {
            color: #64748b;
            font-weight: 500;
        }

        .pay-pin-input:focus-within {
            box-shadow: 0 0 0 3px rgba(56, 189, 248, 0.24);
            border-color: rgba(14, 165, 233, 0.6);
        }

        .pay-alert {
            margin-top: 10px;
            border-radius: 12px;
            padding: 10px 12px;
            border: 1px solid rgba(245, 158, 11, 0.35);
            background: rgba(245, 158, 11, 0.12);
            color: #fde68a;
            font-size: 13px;
            line-height: 1.45;
        }

        .pay-table-wrap {
            margin-top: 10px;
            border: 1px solid rgba(148, 163, 184, 0.22);
            border-radius: 16px;
            overflow: auto;
            background: rgba(2, 6, 23, 0.2);
        }

        .pay-table-wrap table {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0;
            min-width: 700px;
        }

        .pay-table-wrap thead th {
            position: sticky;
            top: 0;
            z-index: 1;
            background: #0b1324;
            color: #cbd5e1;
            font-size: 12px;
            letter-spacing: 0.15px;
            text-transform: uppercase;
            padding: 10px 8px;
            border-bottom: 1px solid rgba(148, 163, 184, 0.22);
            color: #dbe7f8 !important;
        }

        .pay-table-wrap tbody td {
            color: #e2e8f0;
            border-bottom: 1px solid rgba(148, 163, 184, 0.12);
            padding: 9px 8px;
            font-size: 13px;
            vertical-align: middle;
            color: #f1f5f9 !important;
        }

        .pay-table-wrap tbody tr:last-child td {
            border-bottom: 0;
        }

        .pay-product-img {
            width: 56px;
            height: 56px;
            border-radius: 12px;
            object-fit: cover;
            border: 1px solid rgba(148, 163, 184, 0.25);
            background: rgba(255, 255, 255, 0.03);
        }

        .pay-progress {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            height: 3px;
            background: rgba(2, 6, 23, 0.35);
            z-index: 9999;
            pointer-events: none;
            overflow: hidden;
        }

        .pay-progress::before {
            content: "";
            display: block;
            width: 42%;
            height: 100%;
            background: linear-gradient(90deg, #0ea5e9, #22c55e);
            animation: pay-progress-move 1.05s linear infinite;
            border-radius: 999px;
        }

        @keyframes pay-progress-move {
            0% { transform: translateX(-110%); }
            100% { transform: translateX(320%); }
        }

        @media (max-width: 980px) {
            .pay-shell {
                width: min(1180px, 100% - 18px);
                margin-top: 14px;
            }

            .pay-grid {
                grid-template-columns: 1fr;
            }

            .pay-title {
                font-size: 22px;
            }
        }
    </style>
</head>

<body class="body-login-bcorn1">
<form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True"></asp:ScriptManager>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <asp:Timer ID="Timer1" runat="server" Interval="12000" OnTick="Timer1_Tick"></asp:Timer>

            <div class="pay-shell">
                <section class="pay-hero">
                    <div class="pay-hero-top">
                        <img src="/uploads/images/logo-aha-trang.png" alt="AhaSale" class="pay-logo" />
                        <div>
                            <h1 class="pay-title">
                                Chờ Trao đổi Đơn #<asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                            </h1>
                            <span class="pay-tag">1 Quyền = 1000 VNĐ (quy đổi nội bộ)</span>
                        </div>
                    </div>
                    <div class="pay-sub">
                        Quyền tiêu dùng là đơn vị quy ước nội bộ được hiển thị và sử dụng trên nền tảng ahasale.vn. Quyền tiêu dùng không phải tiền tệ hợp pháp và chỉ có hiệu lực trong phạm vi nền tảng.
                    </div>
                    <asp:PlaceHolder ID="ph_shop_home" runat="server" Visible="false">
                        <div class="pay-hero-actions">
                            <a href="/shop/default.aspx" class="pay-btn pay-btn-primary">Trang chủ shop</a>
                            <a href="/shop/don-ban" class="pay-btn pay-btn-muted">Đơn bán</a>
                        </div>
                    </asp:PlaceHolder>
                </section>

                <section class="pay-grid">
                    <div>
                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="false">
                            <div class="pay-card">
                                <h2 class="pay-card-title">Sẵn sàng quét thẻ</h2>
                                <div class="pay-note">Vui lòng chạm thẻ khách hàng để bắt đầu phiên Trao đổi.</div>
                                <div class="pay-amount">
                                    <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                                </div>
                                <div class="pay-actions">
                                    <asp:Button ID="btn_huy_cho" runat="server" OnClick="btn_huy_cho_Click" CausesValidation="false" Text="Hủy chờ Trao đổi" CssClass="pay-btn pay-btn-danger" />
                                    <asp:HyperLink ID="lnk_refresh_wait" runat="server" CssClass="pay-btn pay-btn-muted">Làm mới</asp:HyperLink>
                                </div>
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="false">
                            <div class="pay-card">
                                <h2 class="pay-card-title">
                                    Xin chào, <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                                </h2>
                                <div class="pay-note">
                                    Bạn sắp trao đổi <b><asp:Label ID="Label2" runat="server" Text=""></asp:Label></b> với <b><asp:Label ID="lb_tenshop" runat="server" Text=""></asp:Label></b>.
                                </div>
                                <div class="pay-note" style="color:#fca5a5;">Nếu không phải bạn, vui lòng nhấn <b>Hủy</b>.</div>

                                <div class="pay-pin-input">
                                    <span class="mif-key pin-icon"></span>
                                    <asp:TextBox
                                        MaxLength="4"
                                        TextMode="Password"
                                        ID="txt_mapin"
                                        runat="server"
                                        Attributes="inputmode:numeric;pattern:[0-9]*"
                                        CssClass="pay-pin-control"
                                        placeholder="Nhập mã PIN 4 số để hoàn tất Trao đổi">
                                    </asp:TextBox>
                                </div>

                                <asp:PlaceHolder ID="ph_alert_the" runat="server">
                                    <div class="pay-alert" runat="server" id="box_alert_the" visible="false">
                                        <asp:Literal ID="lb_thongbao_the" runat="server" Visible="false"></asp:Literal>
                                    </div>
                                </asp:PlaceHolder>

                                <div class="pay-actions">
                                    <asp:Button ID="Button2" OnClick="Button2_Click" runat="server" Text="Hủy" CssClass="pay-btn pay-btn-muted" Width="100%" />
                                    <asp:Button ID="Button3" OnClick="Button3_Click" runat="server" Text="Trao đổi" CssClass="pay-btn pay-btn-success" Width="100%" />
                                </div>
                            </div>
                        </asp:PlaceHolder>
                    </div>

                    <div>
                        <div class="pay-card">
                            <h2 class="pay-card-title">Chi tiết đơn hàng</h2>
                            <div class="pay-note">Danh sách sản phẩm trong đơn đang chờ Trao đổi.</div>

                            <div class="pay-table-wrap">
                                <table>
                                    <thead>
                                        <tr>
                                            <th style="width:56px;">ID</th>
                                            <th style="width:72px;">Ảnh</th>
                                            <th class="text-left" style="min-width:180px;">Sản phẩm</th>
                                            <th style="width:56px;">SL</th>
                                            <th style="width:92px;">% Ưu đãi</th>
                                            <th style="min-width:120px;">Tổng</th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        <asp:Repeater ID="Repeater2" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="text-center"><%# Eval("id") %></td>

                                                    <td class="text-center">
                                                        <div data-role="lightbox" class="c-pointer">
                                                            <img src="<%# Eval("image") %>" class="pay-product-img" />
                                                        </div>
                                                    </td>

                                                    <td class="text-left">
                                                        <%# Eval("name") %>
                                                        <div style="color:#93c5fd; margin-top:2px;">
                                                            <%# FormatQuyen(Eval("giaban")) %> Quyền
                                                            <%# (Convert.ToInt32(Eval("PhanTramUuDai")) > 0
                                                                ? "<span style='display:inline-flex; margin-left:6px; padding:1px 6px; border-radius:999px; border:1px solid rgba(245,158,11,.45); background:rgba(245,158,11,.15); color:#fcd34d;'>-"
                                                                  + Eval("PhanTramUuDai") + "%</span>"
                                                                : "") %>
                                                        </div>
                                                    </td>

                                                    <td class="text-center"><%# Eval("soluong") %></td>

                                                    <td class="text-center">
                                                        <%# (Convert.ToInt32(Eval("PhanTramUuDai")) > 0 ? (Eval("PhanTramUuDai") + "%") : "-") %>
                                                    </td>

                                                    <td class="text-right">
                                                        <%# FormatQuyen(Eval("thanhtien")) %> Quyền
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </section>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btn_huy_cho" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="900">
        <ProgressTemplate>
            <div class="pay-progress"></div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</form>

<script src="/Metro-UI-CSS-master/js/metro.min.js"></script>
<script src="/js/aha-ui-refresh.js?v=2026-03-07.2"></script>
</body>
</html>
