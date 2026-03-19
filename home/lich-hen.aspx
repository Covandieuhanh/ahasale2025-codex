<%@ Page Title="Lịch hẹn của tôi" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="lich-hen.aspx.cs" Inherits="home_lich_hen" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .booking-shell{
            padding: 18px 0 40px;
        }
        .booking-filter{
            display:flex;
            flex-wrap:wrap;
            gap:10px;
            align-items:center;
            margin: 10px 0 18px;
        }
        .booking-filter .form-control,
        .booking-filter .form-select{
            border-radius: 999px;
            min-height: 40px;
        }
        .booking-filter .btn{
            border-radius: 999px;
            font-weight: 700;
        }
        .booking-card{
            border: 1px solid #e2e8f0;
            border-radius: 18px;
            background: #fff;
            padding: 18px;
            box-shadow: 0 16px 36px rgba(15, 23, 42, .08);
            margin-bottom: 16px;
        }
        .booking-card-head{
            display:flex;
            align-items:flex-start;
            justify-content:space-between;
            gap:12px;
            margin-bottom:10px;
        }
        .booking-id{
            font-size: 20px;
            font-weight: 800;
            color: #0f172a;
        }
        .booking-label{
            font-size: 12px;
            color: #64748b;
            text-transform: uppercase;
            letter-spacing: .08em;
            font-weight: 700;
        }
        .booking-service{
            font-size: 18px;
            font-weight: 700;
            color: #0f172a;
            margin-bottom: 6px;
        }
        .booking-meta{
            display: grid;
            gap: 6px;
            color: #475569;
            font-size: 14px;
        }
        .booking-meta-line:empty{ display:none; }
        .booking-status{
            display:inline-flex;
            align-items:center;
            min-height: 28px;
            padding: 0 12px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 800;
            white-space: nowrap;
        }
        .booking-status-pending{ background:#fff7e6; color:#b45309; }
        .booking-status-confirmed{ background:#e0f2fe; color:#1d4ed8; }
        .booking-status-done{ background:#dcfce7; color:#166534; }
        .booking-status-cancelled{ background:#fee2e2; color:#b91c1c; }
        .booking-status-missed{ background:#f3f4f6; color:#475569; }
        .booking-action{
            margin-top: 12px;
            display:flex;
            flex-wrap:wrap;
            gap:10px;
        }
        .booking-action a{
            border-radius: 999px;
            font-weight: 700;
        }
        .booking-empty{
            border: 1px dashed #cbd5f5;
            border-radius: 16px;
            padding: 24px;
            text-align: center;
            color: #64748b;
            background: #f8fafc;
        }
        .booking-count{
            font-size: 13px;
            color: #64748b;
        }
        .booking-notice{
            border-radius: 14px;
            padding: 12px 16px;
            border: 1px solid #bbf7d0;
            background: #f0fdf4;
            color: #166534;
            font-weight: 600;
            margin-bottom: 14px;
        }
    </style>
    <script>
        function confirmCancelBooking() {
            return confirm("Bạn có chắc chắn muốn hủy lịch hẹn này không?");
        }
        function applyBookingFilter() {
            var q = document.getElementById("booking_filter_q");
            var st = document.getElementById("booking_filter_status");
            var time = document.getElementById("booking_filter_time");
            var params = [];
            if (q && q.value) params.push("q=" + encodeURIComponent(q.value));
            if (st && st.value) params.push("st=" + encodeURIComponent(st.value));
            if (time && time.value) params.push("time=" + encodeURIComponent(time.value));
            var url = "/home/lich-hen.aspx";
            if (params.length > 0) url += "?" + params.join("&");
            window.location.href = url;
        }
    </script>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Theo dõi lịch hẹn</div>
                    <h2 class="page-title">Lịch hẹn của tôi</h2>
                    <div class="booking-count">Đang hiển thị: <asp:Label ID="lb_count" runat="server" Text="0"></asp:Label> lịch hẹn</div>
                </div>
                <div class="col-auto ms-auto d-print-none">
                    <span class="text-muted small">SĐT: <asp:Label ID="lb_phone" runat="server" Text=""></asp:Label></span>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl booking-shell">
            <asp:PlaceHolder ID="ph_notice" runat="server" Visible="false">
                <div class="booking-notice">
                    <asp:Label ID="lb_notice" runat="server"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <div class="booking-filter">
                <input id="booking_filter_q" class="form-control" type="text" value="<%= Server.HtmlEncode(FilterQuery) %>" placeholder="Tìm theo dịch vụ, gian hàng, chi nhánh...">
                <select id="booking_filter_status" class="form-select">
                    <option value="all" <%= SelectedStatus("all") %>>Tất cả trạng thái</option>
                    <option value="pending" <%= SelectedStatus("pending") %>>Chưa xác nhận</option>
                    <option value="confirmed" <%= SelectedStatus("confirmed") %>>Đã xác nhận</option>
                    <option value="done" <%= SelectedStatus("done") %>>Đã đến</option>
                    <option value="cancelled" <%= SelectedStatus("cancelled") %>>Đã hủy</option>
                    <option value="missed" <%= SelectedStatus("missed") %>>Không đến</option>
                </select>
                <select id="booking_filter_time" class="form-select">
                    <option value="all" <%= SelectedTime("all") %>>Tất cả thời gian</option>
                    <option value="upcoming" <%= SelectedTime("upcoming") %>>Sắp tới</option>
                    <option value="past" <%= SelectedTime("past") %>>Đã qua</option>
                </select>
                <button class="btn btn-primary" type="button" onclick="applyBookingFilter()">Lọc</button>
            </div>

            <asp:PlaceHolder ID="ph_require_login" runat="server" Visible="false">
                <div class="booking-empty">
                    <div class="fw-bold mb-2">Bạn cần đăng nhập để xem lịch hẹn</div>
                    <div class="mb-3">Hãy đăng nhập tài khoản Home để xem toàn bộ lịch đã đặt.</div>
                    <a class="btn btn-primary" href="/home/login.aspx">Đăng nhập</a>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                <div class="booking-empty">
                    <div class="fw-bold mb-2">Chưa có lịch hẹn nào</div>
                    <div class="mb-3">Hãy đặt lịch dịch vụ để lịch hẹn của bạn hiển thị tại đây.</div>
                    <a class="btn btn-outline-secondary" href="/home/Default.aspx">Về trang chủ</a>
                </div>
            </asp:PlaceHolder>

            <asp:Repeater ID="rpt_bookings" runat="server">
                <ItemTemplate>
                    <div class="booking-card">
                        <div class="booking-card-head">
                            <div>
                                <div class="booking-label">Mã lịch hẹn</div>
                                <div class="booking-id">#<%# Eval("Id") %></div>
                            </div>
                            <span class='booking-status <%# Eval("StatusCss") %>'><%# Eval("StatusText") %></span>
                        </div>
                        <div class="booking-service"><%# Eval("ServiceName") %></div>
                        <div class="booking-meta">
                            <div class="booking-meta-line"><i class="ti ti-calendar-event"></i> <%# Eval("DateText") %></div>
                            <div class="booking-meta-line"><i class="ti ti-building-store"></i> Gian hàng: <%# Eval("ShopName") %></div>
                            <div class="booking-meta-line"><i class="ti ti-map-pin"></i> <%# Eval("BranchName") %></div>
                            <div class="booking-meta-line"><%# Eval("ShopPhoneLabel") %></div>
                            <div class="booking-meta-line"><%# Eval("BranchAddressLabel") %></div>
                            <div class="booking-meta-line"><%# Eval("NoteLabel") %></div>
                    </div>
                    <div class="booking-action">
                        <a class="btn btn-outline-secondary btn-sm" href="<%# Eval("ShopUrl") %>">Xem gian hàng</a>
                        <%# string.IsNullOrWhiteSpace((Eval("EditUrl") ?? "").ToString()) ? "" : "<a class=\\\"btn btn-outline-primary btn-sm\\\" href=\\\"" + Eval("EditUrl") + "\\\">Đổi lịch</a>" %>
                        <%# string.IsNullOrWhiteSpace((Eval("RebookUrl") ?? "").ToString()) ? "" : "<a class=\"btn btn-primary btn-sm\" href=\"" + Eval("RebookUrl") + "\">Đặt lại dịch vụ</a>" %>
                        <%# string.IsNullOrWhiteSpace((Eval("CancelUrl") ?? "").ToString()) ? "" : "<a class=\\\"btn btn-outline-danger btn-sm\\\" href=\\\"" + Eval("CancelUrl") + "\\\" onclick=\\\"return confirmCancelBooking();\\\">Hủy lịch</a>" %>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    </div>
</asp:Content>
