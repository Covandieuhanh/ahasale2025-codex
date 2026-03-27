<%@ Page Title="Tạo giao dịch gian hàng" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="tao-giao-dich.aspx.cs" Inherits="gianhang_admin_gianhang_tao_giao_dich" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-createflow {
            display: grid;
            gap: 18px;
        }

        .gh-createflow__hero,
        .gh-createflow__context,
        .gh-createflow__card {
            border: 1px solid #ecdcdc;
            border-radius: 20px;
            background: #fff;
            box-shadow: 0 18px 34px rgba(15, 23, 42, 0.06);
        }

        .gh-createflow__hero {
            padding: 20px 22px;
            background:
                radial-gradient(720px 220px at 0% 0%, rgba(255, 104, 62, .16), transparent 60%),
                linear-gradient(180deg, #fff9f8 0%, #ffffff 100%);
            display: flex;
            justify-content: space-between;
            gap: 16px;
            align-items: flex-start;
            flex-wrap: wrap;
        }

        .gh-createflow__kicker,
        .gh-createflow__tag {
            display: inline-flex;
            align-items: center;
            min-height: 30px;
            padding: 0 12px;
            border-radius: 999px;
            background: #fff1ea;
            border: 1px solid #ffd3c4;
            color: #c2410c;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: .03em;
            text-transform: uppercase;
        }

        .gh-createflow__title {
            margin: 10px 0 6px;
            color: #7f1d1d;
            font-size: 30px;
            line-height: 1.1;
            font-weight: 900;
        }

        .gh-createflow__sub,
        .gh-createflow__context p,
        .gh-createflow__card p,
        .gh-createflow__hint {
            margin: 0;
            color: #8d5d5d;
            font-size: 14px;
            line-height: 1.6;
        }

        .gh-createflow__actions,
        .gh-createflow__card-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }

        .gh-createflow__btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 42px;
            padding: 0 16px;
            border-radius: 14px;
            border: 1px solid transparent;
            text-decoration: none !important;
            font-size: 13px;
            font-weight: 800;
        }

        .gh-createflow__btn--primary {
            color: #fff !important;
            background: linear-gradient(135deg, #d73a31, #ef6b41);
            box-shadow: 0 14px 28px rgba(215, 58, 49, .18);
        }

        .gh-createflow__btn--soft {
            color: #7f1d1d !important;
            background: #fff;
            border-color: #f0d7d9;
        }

        .gh-createflow__context {
            padding: 16px 18px;
            display: grid;
            gap: 10px;
        }

        .gh-createflow__context-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 10px;
        }

        .gh-createflow__chip {
            border: 1px solid #ebeff5;
            border-radius: 16px;
            background: #fff;
            padding: 12px 14px;
        }

        .gh-createflow__chip small {
            display: block;
            color: #8d5d5d;
            font-size: 11px;
            text-transform: uppercase;
            font-weight: 700;
        }

        .gh-createflow__chip strong {
            display: block;
            margin-top: 5px;
            color: #111827;
            font-size: 16px;
            line-height: 1.35;
        }

        .gh-createflow__grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 16px;
        }

        .gh-createflow__card {
            padding: 18px;
            display: grid;
            gap: 14px;
        }

        .gh-createflow__card--recommended {
            border-color: #ffd3c4;
            background: linear-gradient(180deg, #fff9f6 0%, #fff 100%);
            box-shadow: 0 20px 36px rgba(215, 58, 49, .10);
        }

        .gh-createflow__card h3 {
            margin: 0;
            color: #1f2937;
            font-size: 22px;
            line-height: 1.2;
        }

        .gh-createflow__list {
            margin: 0;
            padding-left: 18px;
            color: #4b5563;
            line-height: 1.7;
        }

        @media (max-width: 767px) {
            .gh-createflow__title {
                font-size: 26px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-createflow">
        <section class="gh-createflow__hero">
            <div>
                <div class="gh-createflow__kicker">Entry Flow Thống Nhất</div>
                <h1 class="gh-createflow__title">Tạo giao dịch gian hàng</h1>
                <p class="gh-createflow__sub">
                    Chọn đúng luồng tạo giao dịch theo mục đích sử dụng. Từ đây mình gom cả
                    <code>/gianhang</code> native và <code>/gianhang/admin</code> vận hành vào cùng một điểm vào để sau level 2 thao tác đỡ bị tách đôi.
                </p>
            </div>
            <div class="gh-createflow__actions">
                <a class="gh-createflow__btn gh-createflow__btn--soft" href="<%=BackUrl %>">Quay lại</a>
                <a class="gh-createflow__btn gh-createflow__btn--soft" href="<%=WorkspaceHubUrl %>">Không gian /gianhang</a>
                <a class="gh-createflow__btn gh-createflow__btn--soft" href="<%=UtilityUrl %>">Tiện ích /gianhang</a>
            </div>
        </section>

        <section class="gh-createflow__context">
            <div class="gh-createflow__tag"><%=ContextTag %></div>
            <p><%=ContextDescription %></p>
            <div class="gh-createflow__context-grid">
                <div class="gh-createflow__chip">
                    <small>Khách hàng</small>
                    <strong><%=CustomerDisplay %></strong>
                </div>
                <div class="gh-createflow__chip">
                    <small>Điện thoại</small>
                    <strong><%=PhoneDisplay %></strong>
                </div>
                <asp:PlaceHolder ID="ph_booking" runat="server" Visible="false">
                    <div class="gh-createflow__chip">
                        <small>Lịch hẹn</small>
                        <strong>#<%=BookingId %></strong>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="ph_product" runat="server" Visible="false">
                    <div class="gh-createflow__chip">
                        <small>Sản phẩm khởi tạo</small>
                        <strong>#<%=ProductId %> · SL <%=QuantityText %></strong>
                    </div>
                </asp:PlaceHolder>
            </div>
        </section>

        <div class="gh-createflow__grid">
            <section class="gh-createflow__card<%=InvoiceCardCss %>">
                <div class="gh-createflow__tag"><%=InvoiceTag %></div>
                <h3>Hóa đơn admin</h3>
                <p>
                    Dùng khi cần vận hành nội bộ, giữ bối cảnh CRM, lịch hẹn, nhân viên chăm sóc,
                    hoặc đang tạo giao dịch từ khách hàng/lịch hẹn ngay trong <code>/gianhang/admin</code>.
                </p>
                <ul class="gh-createflow__list">
                    <li>Prefill được tên khách, số điện thoại và liên kết lịch hẹn.</li>
                    <li>Phù hợp cho thao tác tại quầy, chăm sóc khách, xử lý nghiệp vụ nội bộ.</li>
                    <li>Kết nối trực tiếp với hóa đơn, lịch hẹn, thẻ dịch vụ và CRM hiện có.</li>
                </ul>
                <div class="gh-createflow__card-actions">
                    <a class="gh-createflow__btn gh-createflow__btn--primary" href="<%=AdminInvoiceCreateUrl %>">Đi vào hóa đơn admin</a>
                </div>
            </section>

            <section class="gh-createflow__card<%=OrderCardCss %>">
                <div class="gh-createflow__tag"><%=OrderTag %></div>
                <h3>Đơn gian hàng native</h3>
                <p>
                    Dùng khi muốn tạo đơn theo đúng luồng native <code>/gianhang</code>, giữ trải nghiệm storefront,
                    đơn bán và bước chờ trao đổi/thanh toán của không gian gian hàng.
                </p>
                <ul class="gh-createflow__list">
                    <li>Phù hợp khi thao tác theo logic đơn gian hàng native.</li>
                    <li>Đi qua lớp bridge để sau đó vẫn vận hành được trong admin.</li>
                    <li><%=OrderHint %></li>
                </ul>
                <div class="gh-createflow__card-actions">
                    <a class="gh-createflow__btn <%=OrderButtonCss %>" href="<%=NativeOrderCreateUrl %>">Đi vào đơn gian hàng</a>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
