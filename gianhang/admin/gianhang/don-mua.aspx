<%@ Page Title="Buyer-flow gian hàng" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="don-mua.aspx.cs" Inherits="gianhang_admin_gianhang_don_mua" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-admin-buyerflow{display:grid;gap:18px;}
        .gh-admin-buyerflow__hero,.gh-admin-buyerflow__card,.gh-admin-buyerflow__table-card{border:1px solid #eadfe2;border-radius:20px;background:#fff;box-shadow:0 18px 34px rgba(15,23,42,.06);}
        .gh-admin-buyerflow__hero{padding:20px 22px;background:radial-gradient(820px 240px at 100% 0%, rgba(255,132,84,.14), transparent 64%), linear-gradient(180deg,#fff9f7 0%,#ffffff 100%);display:flex;align-items:flex-start;justify-content:space-between;gap:16px;flex-wrap:wrap;}
        .gh-admin-buyerflow__kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#fff1ea;border:1px solid #ffd3c4;color:#c2410c;font-size:12px;font-weight:800;letter-spacing:.03em;text-transform:uppercase;}
        .gh-admin-buyerflow__title{margin:10px 0 6px;color:#7f1d1d;font-size:30px;line-height:1.1;font-weight:900;}
        .gh-admin-buyerflow__sub{margin:0;color:#8d5d5d;font-size:14px;line-height:1.6;max-width:760px;}
        .gh-admin-buyerflow__actions{display:flex;gap:10px;flex-wrap:wrap;}
        .gh-admin-buyerflow__btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;text-decoration:none !important;font-size:13px;font-weight:800;border:1px solid transparent;}
        .gh-admin-buyerflow__btn--primary{color:#fff !important;background:linear-gradient(135deg,#d73a31,#ef6b41);box-shadow:0 14px 28px rgba(215,58,49,.18);}
        .gh-admin-buyerflow__btn--soft{color:#7f1d1d !important;background:#fff;border-color:#f0d7d9;}
        .gh-admin-buyerflow__summary{display:grid;grid-template-columns:repeat(auto-fit,minmax(160px,1fr));gap:12px;}
        .gh-admin-buyerflow__summary-card{border:1px solid #ebeff5;border-radius:18px;background:#fff;padding:16px;}
        .gh-admin-buyerflow__summary-card small{display:block;color:#8d5d5d;font-size:12px;text-transform:uppercase;font-weight:700;}
        .gh-admin-buyerflow__summary-card strong{display:block;margin-top:8px;color:#1f2937;font-size:28px;line-height:1.05;font-weight:900;}
        .gh-admin-buyerflow__summary-card span{display:block;margin-top:6px;color:#6b7280;font-size:13px;line-height:1.6;}
        .gh-admin-buyerflow__map{display:grid;grid-template-columns:repeat(auto-fit,minmax(220px,1fr));gap:12px;}
        .gh-admin-buyerflow__map-card{border:1px solid #ebeff5;border-radius:18px;background:#fff;padding:16px;}
        .gh-admin-buyerflow__map-card small{display:block;color:#8d5d5d;font-size:12px;font-weight:700;text-transform:uppercase;}
        .gh-admin-buyerflow__map-card strong{display:block;margin-top:8px;font-size:20px;color:#182433;}
        .gh-admin-buyerflow__map-card p{margin:8px 0 0;color:#6b7280;font-size:14px;line-height:1.6;}
        .gh-admin-buyerflow__filters{padding:16px;display:grid;grid-template-columns:minmax(0,1fr) 220px auto auto;gap:12px;align-items:end;}
        .gh-admin-buyerflow__field{display:grid;gap:6px;}
        .gh-admin-buyerflow__field label{color:#8d5d5d;font-size:12px;font-weight:700;text-transform:uppercase;letter-spacing:.03em;}
        .gh-admin-buyerflow__input,.gh-admin-buyerflow__select{min-height:42px;border-radius:14px;border:1px solid #d9dee7;background:#fff;padding:0 14px;color:#1f2937;font-size:14px;outline:none;}
        .gh-admin-buyerflow__input:focus,.gh-admin-buyerflow__select:focus{border-color:#ef6b41;box-shadow:0 0 0 3px rgba(239,107,65,.12);}
        .gh-admin-buyerflow__submit,.gh-admin-buyerflow__reset{min-height:42px;border-radius:14px;padding:0 16px;font-size:13px;font-weight:800;}
        .gh-admin-buyerflow__submit{border:0;background:linear-gradient(135deg,#d73a31,#ef6b41);color:#fff;}
        .gh-admin-buyerflow__reset{border:1px solid #eadfe2;background:#fff;color:#7f1d1d;}
        .gh-admin-buyerflow__table-wrap{overflow:auto;}
        .gh-admin-buyerflow__table{width:100%;border-collapse:collapse;}
        .gh-admin-buyerflow__table th,.gh-admin-buyerflow__table td{padding:12px 10px;border-bottom:1px solid #edf1f6;text-align:left;vertical-align:top;}
        .gh-admin-buyerflow__table th{color:#8d5d5d;font-size:12px;text-transform:uppercase;letter-spacing:.03em;}
        .gh-admin-buyerflow__status{display:inline-flex;align-items:center;justify-content:center;padding:7px 12px;border-radius:999px;font-size:12px;font-weight:800;border:1px solid transparent;}
        .gh-admin-buyerflow__status--pending{background:#eff6ff;color:#1d4ed8;border-color:#bfdbfe;}
        .gh-admin-buyerflow__status--waiting{background:#fff1f2;color:#be123c;border-color:#fda4af;}
        .gh-admin-buyerflow__status--exchanged{background:#eff6ff;color:#1d4ed8;border-color:#bfdbfe;}
        .gh-admin-buyerflow__status--delivered{background:#ecfdf3;color:#15803d;border-color:#86efac;}
        .gh-admin-buyerflow__status--cancelled{background:#f3f4f6;color:#6b7280;border-color:#d1d5db;}
        .gh-admin-buyerflow__actions-list{display:flex;gap:8px;flex-wrap:wrap;}
        .gh-admin-buyerflow__action-link{display:inline-flex;align-items:center;justify-content:center;min-height:34px;padding:0 12px;border-radius:12px;border:1px solid #eadfe2;background:#fff;color:#7f1d1d !important;font-size:12px;font-weight:800;text-decoration:none !important;}
        .gh-admin-buyerflow__empty{padding:24px 12px;color:#6b7280;text-align:center;}
        @media (max-width:991px){.gh-admin-buyerflow__filters{grid-template-columns:1fr;} .gh-admin-buyerflow__title{font-size:26px;}}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-admin-buyerflow">
        <section class="gh-admin-buyerflow__hero">
            <div>
                <div class="gh-admin-buyerflow__kicker">Buyer-flow /gianhang</div>
                <h1 class="gh-admin-buyerflow__title">Giỏ hàng · chờ thanh toán · đơn mua</h1>
                <p class="gh-admin-buyerflow__sub">
                    Đây là lớp đối chiếu buyer-flow ngay trong <code>/gianhang/admin</code>. Giỏ hàng vẫn là trạng thái cá nhân của buyer,
                    còn từ lúc buyer tạo đơn hoặc vào bước chờ thanh toán thì admin có thể theo dõi tiếp ngay tại đây.
                </p>
            </div>
            <div class="gh-admin-buyerflow__actions">
                <a class="gh-admin-buyerflow__btn gh-admin-buyerflow__btn--primary" href="<%=OrdersUrl %>">Đơn gian hàng</a>
                <a class="gh-admin-buyerflow__btn gh-admin-buyerflow__btn--soft" href="<%=ContentUrl %>">Nội dung /gianhang</a>
                <a class="gh-admin-buyerflow__btn gh-admin-buyerflow__btn--soft" href="<%=CustomersUrl %>">Khách hàng /gianhang</a>
                <a class="gh-admin-buyerflow__btn gh-admin-buyerflow__btn--soft" href="<%=BookingsUrl %>">Lịch hẹn /gianhang</a>
                <a class="gh-admin-buyerflow__btn gh-admin-buyerflow__btn--soft" href="<%=UtilityUrl %>">Tiện ích /gianhang</a>
                <a class="gh-admin-buyerflow__btn gh-admin-buyerflow__btn--soft" href="<%=WaitUrl %>">Chờ thanh toán</a>
                <a class="gh-admin-buyerflow__btn gh-admin-buyerflow__btn--soft" href="<%=LegacyInvoiceUrl %>">Hóa đơn admin</a>
                <a class="gh-admin-buyerflow__btn gh-admin-buyerflow__btn--soft" href="<%=PublicUrl %>" target="_blank">Trang công khai</a>
                <a class="gh-admin-buyerflow__btn gh-admin-buyerflow__btn--soft" href="<%=SyncUrl %>">Làm mới bridge</a>
            </div>
        </section>

        <div class="gh-admin-buyerflow__summary">
            <div class="gh-admin-buyerflow__summary-card">
                <small>Tổng đơn buyer-flow</small>
                <strong><%=SummaryTotal.ToString("#,##0") %></strong>
                <span>Tất cả đơn public của workspace đã được tạo từ /gianhang.</span>
            </div>
            <div class="gh-admin-buyerflow__summary-card">
                <small>Chờ thanh toán</small>
                <strong><%=SummaryWaiting.ToString("#,##0") %></strong>
                <span>Buyer đang ở bước trao đổi hoặc chờ xác nhận thêm.</span>
            </div>
            <div class="gh-admin-buyerflow__summary-card">
                <small>Đã trao đổi</small>
                <strong><%=SummaryExchanged.ToString("#,##0") %></strong>
                <span>Đã xong bước chờ thanh toán, chờ giao hoặc hậu kiểm.</span>
            </div>
            <div class="gh-admin-buyerflow__summary-card">
                <small>Đã giao</small>
                <strong><%=SummaryDelivered.ToString("#,##0") %></strong>
                <span>Buyer-flow hoàn tất ở góc nhìn storefront.</span>
            </div>
            <div class="gh-admin-buyerflow__summary-card">
                <small>Đã hủy</small>
                <strong><%=SummaryCancelled.ToString("#,##0") %></strong>
                <span>Vòng đời đơn kết thúc bằng thao tác hủy.</span>
            </div>
            <div class="gh-admin-buyerflow__summary-card">
                <small>Đã mirror sang admin</small>
                <strong><%=SummaryMirrored.ToString("#,##0") %></strong>
                <span>Có thể mở tiếp trong lớp hóa đơn admin để xử lý sâu hơn.</span>
            </div>
        </div>

        <div class="gh-admin-buyerflow__map">
            <section class="gh-admin-buyerflow__map-card">
                <small>Giỏ hàng buyer</small>
                <strong>Trạng thái cá nhân</strong>
                <p>Admin không mirror từng dòng giỏ hàng. Từ lúc buyer bấm tạo đơn hoặc bước vào chờ thanh toán thì luồng mới hiện ở admin.</p>
            </section>
            <section class="gh-admin-buyerflow__map-card">
                <small>Chờ thanh toán</small>
                <strong>Mở phiên trao đổi</strong>
                <p>Đây là điểm admin theo dõi các đơn đang chờ buyer và seller hoàn tất bước trao đổi.</p>
            </section>
            <section class="gh-admin-buyerflow__map-card">
                <small>Đơn mua buyer</small>
                <strong>Kết quả buyer-flow</strong>
                <p>Trạng thái buyer nhìn thấy như đã đặt, chờ trao đổi, đã trao đổi, đã giao hoặc đã hủy đều được phản chiếu ở đây.</p>
            </section>
        </div>

        <section class="gh-admin-buyerflow__card">
            <div class="gh-admin-buyerflow__filters">
                <div class="gh-admin-buyerflow__field">
                    <label for="<%= txt_search.ClientID %>">Tìm buyer-flow</label>
                    <asp:TextBox ID="txt_search" runat="server" CssClass="gh-admin-buyerflow__input" MaxLength="100" placeholder="ID đơn, buyer, tên khách, tài khoản"></asp:TextBox>
                </div>
                <div class="gh-admin-buyerflow__field">
                    <label for="<%= ddl_status.ClientID %>">Lọc trạng thái</label>
                    <asp:DropDownList ID="ddl_status" runat="server" CssClass="gh-admin-buyerflow__select">
                        <asp:ListItem Text="Tất cả trạng thái" Value="all"></asp:ListItem>
                        <asp:ListItem Text="Đã đặt" Value="da-dat"></asp:ListItem>
                        <asp:ListItem Text="Chờ trao đổi" Value="cho-trao-doi"></asp:ListItem>
                        <asp:ListItem Text="Đã trao đổi" Value="da-trao-doi"></asp:ListItem>
                        <asp:ListItem Text="Đã giao" Value="da-giao"></asp:ListItem>
                        <asp:ListItem Text="Đã hủy" Value="da-huy"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:Button ID="btn_filter" runat="server" Text="Lọc đơn" CssClass="gh-admin-buyerflow__submit" OnClick="btn_filter_Click" />
                <asp:Button ID="btn_clear" runat="server" Text="Đặt lại" CssClass="gh-admin-buyerflow__reset" CausesValidation="false" OnClick="btn_clear_Click" />
            </div>
        </section>

        <section class="gh-admin-buyerflow__table-card">
            <div class="gh-admin-buyerflow__table-wrap">
                <table class="gh-admin-buyerflow__table">
                    <thead>
                        <tr>
                            <th style="width:120px;">ID đơn</th>
                            <th style="min-width:120px;">Ngày tạo</th>
                            <th style="min-width:220px;">Buyer</th>
                            <th style="min-width:160px;">Trạng thái buyer</th>
                            <th style="min-width:220px;">Ý nghĩa vận hành</th>
                            <th class="text-right" style="min-width:140px;">Tổng tiền</th>
                            <th style="min-width:220px;">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rp_orders" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <strong>#<%# Eval("DisplayId") %></strong><br />
                                        <span class="fg-gray">GH: <%# Eval("SourceInvoiceText") %></span>
                                    </td>
                                    <td><%# Eval("OrderedAtText") %></td>
                                    <td><strong><%# Eval("BuyerDisplay") %></strong></td>
                                    <td><span class='<%# Eval("StatusCss") %>'><%# Eval("StatusText") %></span></td>
                                    <td><span class="fg-gray"><%# Eval("StageHint") %></span></td>
                                    <td class="text-right"><strong><%# Eval("TotalAmountText") %> đ</strong></td>
                                    <td>
                                        <div class="gh-admin-buyerflow__actions-list">
                                            <a class="gh-admin-buyerflow__action-link" href="<%# Eval("OrderDetailUrl") %>">Mở đơn gian hàng</a>
                                            <asp:PlaceHolder ID="ph_legacy" runat="server" Visible='<%# Convert.ToBoolean(Eval("HasLegacyMirror")) %>'>
                                                <a class="gh-admin-buyerflow__action-link" href="<%# Eval("LegacyDetailUrl") %>">Hóa đơn admin</a>
                                            </asp:PlaceHolder>
                                            <a class="gh-admin-buyerflow__action-link" href="<%# Eval("PublicUrl") %>" target="_blank">Trang công khai</a>
                                        </div>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                            <tr>
                                <td colspan="7" class="gh-admin-buyerflow__empty">Chưa có bản ghi buyer-flow phù hợp để hiển thị.</td>
                            </tr>
                        </asp:PlaceHolder>
                    </tbody>
                </table>
            </div>
        </section>
    </div>
</asp:Content>
