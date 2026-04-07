<%@ Page Title="Không gian /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="gianhang_admin_gianhang_default" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-bridge-shell {
            display: grid;
            gap: 18px;
        }

        .gh-bridge-hero {
            border: 1px solid #f6d4c5;
            border-radius: 22px;
            padding: 20px 22px;
            background:
                radial-gradient(700px 220px at 0% 0%, rgba(255, 110, 53, .18), transparent 60%),
                linear-gradient(180deg, #fffdfc 0%, #ffffff 100%);
            box-shadow: 0 20px 36px rgba(127, 29, 29, 0.08);
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 18px;
            flex-wrap: wrap;
        }

        .gh-bridge-kicker {
            display: inline-flex;
            align-items: center;
            min-height: 30px;
            padding: 0 12px;
            border-radius: 999px;
            background: #fff3ee;
            border: 1px solid #ffd4c5;
            color: #b93815;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: .03em;
            text-transform: uppercase;
        }

        .gh-bridge-title {
            margin: 10px 0 6px;
            color: #7f1d1d;
            font-size: 30px;
            line-height: 1.12;
            font-weight: 900;
        }

        .gh-bridge-sub {
            margin: 0;
            color: #8d5d5d;
            font-size: 14px;
            line-height: 1.6;
            max-width: 760px;
        }

        .gh-bridge-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }

        .gh-bridge-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 42px;
            padding: 0 16px;
            border-radius: 14px;
            text-decoration: none !important;
            font-weight: 800;
            font-size: 13px;
            border: 1px solid transparent;
        }

        .gh-bridge-btn--primary {
            background: linear-gradient(135deg, #d73a31, #ef6b41);
            color: #fff !important;
            box-shadow: 0 14px 30px rgba(215, 58, 49, .18);
        }

        .gh-bridge-btn--soft {
            background: #fff;
            color: #7f1d1d !important;
            border-color: #f3cccc;
        }

        .gh-bridge-kpis {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(170px, 1fr));
            gap: 12px;
        }

        .gh-bridge-kpi {
            border: 1px solid #f0dfe0;
            border-radius: 18px;
            padding: 16px;
            background: #fff;
            box-shadow: 0 14px 28px rgba(15, 23, 42, 0.05);
        }

        .gh-bridge-kpi small {
            display: block;
            color: #8d5d5d;
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .gh-bridge-kpi strong {
            display: block;
            margin-top: 8px;
            color: #1f2937;
            font-size: 28px;
            line-height: 1.05;
            font-weight: 900;
        }

        .gh-bridge-kpi span {
            display: block;
            margin-top: 6px;
            color: #6b7280;
            font-size: 13px;
        }

        .gh-bridge-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
            gap: 14px;
        }

        .gh-bridge-card {
            border: 1px solid #ebeff5;
            border-radius: 18px;
            background: #fff;
            padding: 16px;
            box-shadow: 0 16px 30px rgba(15, 23, 42, 0.05);
        }

        .gh-bridge-card--accent {
            border-color: #ffd4c5;
            background: linear-gradient(180deg, #fff8f4 0%, #ffffff 100%);
        }

        .gh-bridge-card h3 {
            margin: 0 0 6px;
            font-size: 18px;
            line-height: 1.25;
            color: #1f2937;
        }

        .gh-bridge-card p {
            margin: 0;
            color: #6b7280;
            line-height: 1.6;
            font-size: 14px;
        }

        .gh-bridge-links {
            display: grid;
            gap: 8px;
            margin-top: 14px;
        }

        .gh-bridge-link {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            padding: 11px 12px;
            border-radius: 14px;
            background: #f8fafc;
            color: #1f2937;
            text-decoration: none !important;
            border: 1px solid #edf2f7;
            font-weight: 700;
        }

        .gh-bridge-link small {
            display: block;
            color: #8d5d5d;
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
            margin-bottom: 2px;
        }

        .gh-bridge-link span {
            color: #6b7280;
            font-size: 13px;
            font-weight: 600;
        }

        .gh-bridge-list {
            margin: 14px 0 0;
            padding-left: 18px;
            color: #4b5563;
            line-height: 1.7;
        }

        .gh-bridge-table {
            width: 100%;
            border-collapse: collapse;
        }

        .gh-bridge-table th,
        .gh-bridge-table td {
            padding: 11px 10px;
            border-bottom: 1px solid #eef2f7;
            vertical-align: top;
            text-align: left;
        }

        .gh-bridge-table th {
            color: #8d5d5d;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: .03em;
        }

        .gh-bridge-empty {
            color: #6b7280;
            margin-top: 12px;
        }

        @media (max-width: 767px) {
            .gh-bridge-title {
                font-size: 26px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-bridge-shell">
        <section class="gh-bridge-hero">
            <div>
                <div class="gh-bridge-kicker">Không gian /gianhang</div>
                <h1 class="gh-bridge-title"><%=WorkspaceDisplayName %></h1>
                <p class="gh-bridge-sub">
                    Đây là lớp đồng bộ để chủ không gian level 2 tiếp tục vận hành ngay trong <code>/gianhang/admin</code>,
                    nhưng vẫn dùng đúng dữ liệu gốc đang phát sinh từ <code>/gianhang</code>.
                </p>
            </div>
            <div class="gh-bridge-actions">
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=AccountCenterUrl %>">Trung tâm /gianhang</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=PresentationUrl %>">Trình bày storefront</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=ProductsUrl %>">Sản phẩm /gianhang</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=ServicesUrl %>">Dịch vụ /gianhang</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=ArticlesUrl %>">Bài viết /gianhang</a>
                <a class="gh-bridge-btn gh-bridge-btn--primary" href="<%=CreateFlowUrl %>">Tạo giao dịch</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=StorefrontUrl %>">Trang công khai</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=ContentUrl %>">Nội dung /gianhang</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=CustomersUrl %>">Khách hàng /gianhang</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=BookingsUrl %>">Lịch hẹn /gianhang</a>
                <a class="gh-bridge-btn gh-bridge-btn--primary" href="<%=OrdersUrl %>">Đơn gian hàng</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=WaitUrl %>">Chờ thanh toán</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=BuyerFlowUrl %>">Buyer-flow / Đơn mua</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=CartUrl %>">Giỏ hàng /gianhang</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=ElectronicInvoiceUrl %>">Hóa đơn điện tử</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=ReportUrl %>">Báo cáo gian hàng</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=LegacyInvoiceUrl %>">Hóa đơn admin</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=PublicUrl %>" target="_blank">Trang công khai</a>
                <a class="gh-bridge-btn gh-bridge-btn--soft" href="<%=SyncUrl %>">Làm mới bridge</a>
            </div>
        </section>

        <div class="gh-bridge-kpis">
            <div class="gh-bridge-kpi">
                <small>Sản phẩm</small>
                <strong><%=ProductCount.ToString("#,##0") %></strong>
                <span>Đồng bộ từ dữ liệu native /gianhang</span>
            </div>
            <div class="gh-bridge-kpi">
                <small>Dịch vụ</small>
                <strong><%=ServiceCount.ToString("#,##0") %></strong>
                <span>Dùng chung storefront và công cụ admin</span>
            </div>
            <div class="gh-bridge-kpi">
                <small>Đơn bán</small>
                <strong><%=TotalOrders.ToString("#,##0") %></strong>
                <span>Trong đó chờ trao đổi: <%=WaitingExchange.ToString("#,##0") %></span>
            </div>
            <div class="gh-bridge-kpi">
                <small>Lịch hẹn</small>
                <strong><%=BookingTotal.ToString("#,##0") %></strong>
                <span>Booking từ storefront đi vào vận hành</span>
            </div>
            <div class="gh-bridge-kpi">
                <small>Khách hàng</small>
                <strong><%=CustomerCount.ToString("#,##0") %></strong>
                <span>Gom theo CRM native của gian hàng</span>
            </div>
            <div class="gh-bridge-kpi">
                <small>Doanh thu gộp</small>
                <strong><%=GianHangReport_cl.FormatCurrency(RevenueGross) %> đ</strong>
                <span>Dữ liệu lấy từ lõi đơn hàng /gianhang</span>
            </div>
            <div class="gh-bridge-kpi">
                <small>Hóa đơn mirror</small>
                <strong><%=LegacyMirroredInvoiceCount.ToString("#,##0") %></strong>
                <span>Đã hiện ở công cụ hóa đơn admin để vận hành tiếp</span>
            </div>
        </div>

        <div class="gh-bridge-grid">
            <section class="gh-bridge-card gh-bridge-card--accent">
                <h3>Những gì đã đi thẳng vào /gianhang/admin</h3>
                <p>Các luồng còn thiếu lớn nhất của <code>/gianhang</code> giờ đã có điểm vào rõ ràng trong admin.</p>
                <div class="gh-bridge-links">
                    <a class="gh-bridge-link" href="<%=AccountCenterUrl %>">
                        <div>
                            <small>Tài khoản gốc của workspace</small>
                            Trung tâm /gianhang
                            <span>Xem trạng thái owner Home, storefront, quyền level 2 và các lối vào vận hành từ một chỗ.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=PresentationUrl %>">
                        <div>
                            <small>Bố cục & thương hiệu</small>
                            Trình bày storefront
                            <span>Gom menu, slider, section, branding và các lớp hiển thị public về cùng một hub.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=ProductsUrl %>">
                        <div>
                            <small>Catalog bán hàng</small>
                            Sản phẩm /gianhang
                            <span>Tách riêng lớp sản phẩm native để kiểm tra hiển thị, bridge admin và mở nhanh sang public.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=ServicesUrl %>">
                        <div>
                            <small>Catalog booking</small>
                            Dịch vụ /gianhang
                            <span>Tách riêng lớp dịch vụ native để vận hành booking, ưu đãi và bridge admin rõ ràng hơn.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=ArticlesUrl %>">
                        <div>
                            <small>Bài viết public</small>
                            Bài viết /gianhang
                            <span>Xem lớp bài tư vấn/SEO public và trạng thái mirror của chúng trong admin.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=OrdersUrl %>">
                        <div>
                            <small>Đơn bán · checkout</small>
                            Đơn gian hàng
                            <span>Nhìn, lọc, cập nhật trạng thái các đơn phát sinh từ storefront.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=CreateFlowUrl %>">
                        <div>
                            <small>Entry flow thống nhất</small>
                            Tạo giao dịch
                            <span>Điểm vào chung để chọn giữa đơn native /gianhang và hóa đơn admin vận hành.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=WaitUrl %>">
                        <div>
                            <small>Buyer-flow đang chờ</small>
                            Chờ thanh toán
                            <span>Tách riêng các đơn buyer đã vào bước trao đổi để vận hành nhanh hơn.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=BuyerFlowUrl %>">
                        <div>
                            <small>Giỏ hàng · đơn mua</small>
                            Buyer-flow / Đơn mua
                            <span>Nhìn toàn bộ hành trình buyer từ lúc tạo đơn đến khi hoàn tất hoặc hủy.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=CartUrl %>">
                        <div>
                            <small>Preview checkout</small>
                            Giỏ hàng /gianhang
                            <span>Xem nhanh giỏ hàng storefront theo browser session hiện tại ngay trong admin.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=ElectronicInvoiceUrl %>">
                        <div>
                            <small>Hóa đơn public</small>
                            Hóa đơn điện tử
                            <span>Tra cứu và mở bản hóa đơn điện tử của các đơn native /gianhang.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=ReportUrl %>">
                        <div>
                            <small>Báo cáo native</small>
                            Báo cáo gian hàng
                            <span>Dùng lại dashboard report của /gianhang ngay trong admin.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=UtilityUrl %>">
                        <div>
                            <small>Tiện ích workspace</small>
                            Tiện ích /gianhang
                            <span>Gom các công cụ bổ trợ như quay số và cơ cấu vào cùng ngữ cảnh workspace level 2.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=LegacyInvoiceUrl %>">
                        <div>
                            <small>Bridge sang vận hành</small>
                            Hóa đơn admin
                            <span>Mở lớp hóa đơn legacy đã nhận dữ liệu mirror từ /gianhang để thao tác sâu hơn.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=ContentUrl %>">
                        <div>
                            <small>Sản phẩm · Dịch vụ · Bài viết</small>
                            Nội dung /gianhang
                            <span>Nhìn toàn bộ sản phẩm và dịch vụ native, đồng thời mở lớp quản lý bài viết admin từ cùng workspace.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=StorefrontUrl %>">
                        <div>
                            <small>Preview public</small>
                            Trang công khai /gianhang
                            <span>Mở trực tiếp storefront công khai ngay trong admin để rà bố cục, nội dung và CTA của khách.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                </div>
            </section>

            <section class="gh-bridge-card">
                <h3>Map chỗ dùng giữa 2 mặt không gian</h3>
                <p>Từ giờ nên nhìn như một workspace duy nhất, chỉ khác mặt public và mặt vận hành.</p>
                <div class="gh-bridge-links">
                    <a class="gh-bridge-link" href="<%=ContentUrl %>">
                        <div>
                            <small>Nội dung</small>
                            Sản phẩm · dịch vụ /gianhang
                            <span>Xem toàn bộ catalog native và lớp bridge admin theo cùng workspace.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=ProductsUrl %>">
                        <div>
                            <small>Chi tiết catalog</small>
                            Sản phẩm public
                            <span>Đi sâu vào lớp sản phẩm native, tồn kho hiển thị và trạng thái public theo từng bản ghi.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=ServicesUrl %>">
                        <div>
                            <small>Chi tiết catalog</small>
                            Dịch vụ public
                            <span>Đi sâu vào lớp dịch vụ native, ưu đãi, hiển thị public và liên kết booking.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=CustomersUrl %>">
                        <div>
                            <small>Khách hàng</small>
                            CRM gian hàng
                            <span>Xem lớp khách native /gianhang và đi tiếp sang CRM admin khi cần chăm sóc sâu hơn.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=BookingsUrl %>">
                        <div>
                            <small>Lịch hẹn</small>
                            Booking vận hành
                            <span>Nhìn lớp booking native rồi mở tiếp sang danh sách lịch hẹn admin đã mirror.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=BuyerFlowUrl %>">
                        <div>
                            <small>Giỏ hàng buyer</small>
                            Buyer-flow trong admin
                            <span>Giỏ hàng vẫn là lớp cá nhân của buyer; admin tiếp nhận từ lúc đơn được tạo hoặc buyer vào bước chờ thanh toán.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=CartUrl %>">
                        <div>
                            <small>Session storefront</small>
                            Giỏ hàng preview
                            <span>Dùng để xem nhanh trạng thái checkout hiện tại của browser/session đang thao tác storefront.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=ElectronicInvoiceUrl %>">
                        <div>
                            <small>Chứng từ public</small>
                            Hóa đơn điện tử
                            <span>Từ admin có thể tra cứu và mở đúng bản hóa đơn public để gửi lại cho khách.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=UtilityUrl %>">
                        <div>
                            <small>Tiện ích</small>
                            Công cụ nội bộ /gianhang
                            <span>Quay số và cơ cấu giờ cũng nằm trong mặt vận hành admin của cùng workspace.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-bridge-link" href="<%=PersonHubUrl %>">
                        <div>
                            <small>Con người</small>
                            Hồ sơ người
                            <span>Điểm liên kết Home tập trung cho nhân sự, khách, chuyên gia, thành viên.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                </div>
            </section>
        </div>

        <div class="gh-bridge-grid">
            <section class="gh-bridge-card">
                <h3>Dữ liệu đang được bridge sang admin</h3>
                <ul class="gh-bridge-list">
                    <li>Sản phẩm và dịch vụ native <code>/gianhang</code> sang danh mục vận hành admin.</li>
                    <li>Lịch hẹn public sang lịch hẹn quản trị nội bộ.</li>
                    <li>Đơn và chi tiết đơn sang mô-đun hóa đơn legacy để xử lý tiếp.</li>
                    <li>Khách hàng được bám theo số điện thoại để CRM/admin nhận ra cùng một người.</li>
                </ul>
            </section>

            <section class="gh-bridge-card">
                <h3>Khách gần đây từ không gian /gianhang</h3>
                <asp:Repeater ID="rp_recent_customers" runat="server">
                    <HeaderTemplate>
                        <table class="gh-bridge-table">
                            <thead>
                                <tr>
                                    <th>Khách</th>
                                    <th>Đơn</th>
                                    <th>Lịch hẹn</th>
                                    <th>Doanh thu</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <strong><%# Eval("DisplayName") %></strong><br />
                                <span class="fg-gray"><%# Eval("Phone") %></span>
                            </td>
                            <td><%# Eval("OrderCount") %></td>
                            <td><%# Eval("BookingCount") %></td>
                            <td><%# GianHangReport_cl.FormatCurrency(Convert.ToDecimal(Eval("RevenueTotal"))) %> đ</td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:PlaceHolder ID="ph_no_customers" runat="server" Visible="false">
                    <div class="gh-bridge-empty">Chưa có khách phát sinh từ không gian <code>/gianhang</code>.</div>
                </asp:PlaceHolder>
            </section>
        </div>
    </div>
</asp:Content>
