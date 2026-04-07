<%@ Page Title="Hóa đơn điện tử /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="hoa-don-dien-tu.aspx.cs" Inherits="gianhang_admin_gianhang_hoa_don_dien_tu" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-admin-einv{display:grid;gap:18px;}
        .gh-admin-einv__hero,.gh-admin-einv__card{border:1px solid #ecdcdc;border-radius:20px;background:#fff;box-shadow:0 18px 34px rgba(15,23,42,.06);}
        .gh-admin-einv__hero{padding:20px 22px;background:radial-gradient(720px 220px at 0% 0%, rgba(255,104,62,.16), transparent 60%), linear-gradient(180deg,#fff9f8 0%,#ffffff 100%);display:flex;justify-content:space-between;gap:16px;align-items:flex-start;flex-wrap:wrap;}
        .gh-admin-einv__kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#fff1ea;border:1px solid #ffd3c4;color:#c2410c;font-size:12px;font-weight:800;letter-spacing:.03em;text-transform:uppercase;}
        .gh-admin-einv__title{margin:10px 0 6px;color:#7f1d1d;font-size:30px;line-height:1.1;font-weight:900;}
        .gh-admin-einv__sub{margin:0;color:#8d5d5d;font-size:14px;line-height:1.65;max-width:760px;}
        .gh-admin-einv__actions{display:flex;gap:10px;flex-wrap:wrap;}
        .gh-admin-einv__btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;border:1px solid transparent;text-decoration:none !important;font-size:13px;font-weight:800;}
        .gh-admin-einv__btn--primary{color:#fff !important;background:linear-gradient(135deg,#d73a31,#ef6b41);box-shadow:0 14px 28px rgba(215,58,49,.18);}
        .gh-admin-einv__btn--soft{color:#7f1d1d !important;background:#fff;border-color:#f0d7d9;}
        .gh-admin-einv__filters{padding:16px;display:grid;grid-template-columns:minmax(0, 1fr) auto auto;gap:12px;align-items:end;}
        .gh-admin-einv__field{display:grid;gap:6px;}
        .gh-admin-einv__field label{color:#8d5d5d;font-size:12px;font-weight:700;text-transform:uppercase;letter-spacing:.03em;}
        .gh-admin-einv__input{min-height:42px;border-radius:14px;border:1px solid #d9dee7;background:#fff;padding:0 14px;color:#1f2937;font-size:14px;outline:none;}
        .gh-admin-einv__input:focus{border-color:#ef6b41;box-shadow:0 0 0 3px rgba(239,107,65,.12);}
        .gh-admin-einv__submit,.gh-admin-einv__reset{min-height:42px;border-radius:14px;padding:0 16px;font-size:13px;font-weight:800;}
        .gh-admin-einv__submit{border:0;background:linear-gradient(135deg,#d73a31,#ef6b41);color:#fff;}
        .gh-admin-einv__reset{border:1px solid #eadfe2;background:#fff;color:#7f1d1d;}
        .gh-admin-einv__summary{display:grid;grid-template-columns:repeat(auto-fit,minmax(170px,1fr));gap:12px;}
        .gh-admin-einv__summary-card{border:1px solid #ebeff5;border-radius:18px;background:#fff;padding:16px;}
        .gh-admin-einv__summary-card small{display:block;color:#8d5d5d;font-size:12px;text-transform:uppercase;font-weight:700;}
        .gh-admin-einv__summary-card strong{display:block;margin-top:8px;color:#1f2937;font-size:28px;line-height:1.05;font-weight:900;}
        .gh-admin-einv__summary-card span{display:block;margin-top:6px;color:#6b7280;font-size:13px;}
        .gh-admin-einv__split{display:grid;grid-template-columns:minmax(0,1.2fr) minmax(320px,.8fr);gap:16px;align-items:start;}
        .gh-admin-einv__card{padding:18px;}
        .gh-admin-einv__table-wrap{overflow:auto;}
        .gh-admin-einv__table{width:100%;border-collapse:collapse;}
        .gh-admin-einv__table th,.gh-admin-einv__table td{padding:12px 10px;border-bottom:1px solid #edf1f6;text-align:left;vertical-align:top;}
        .gh-admin-einv__table th{color:#8d5d5d;font-size:12px;text-transform:uppercase;letter-spacing:.03em;}
        .gh-admin-einv__badge{display:inline-flex;align-items:center;min-height:28px;padding:0 10px;border-radius:999px;font-size:12px;font-weight:800;white-space:nowrap;}
        .gh-admin-einv__badge--new{background:#fff7ed;color:#c2410c;}
        .gh-admin-einv__badge--paid{background:#ecfdf3;color:#15803d;}
        .gh-admin-einv__badge--cancelled{background:#f3f4f6;color:#6b7280;}
        .gh-admin-einv__links{display:flex;gap:8px;flex-wrap:wrap;}
        .gh-admin-einv__link{display:inline-flex;align-items:center;justify-content:center;min-height:34px;padding:0 12px;border-radius:12px;border:1px solid #eadfe2;background:#fff;color:#7f1d1d !important;font-size:12px;font-weight:800;text-decoration:none !important;}
        .gh-admin-einv__preview-grid{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:12px;}
        .gh-admin-einv__kv{padding:12px 14px;border:1px solid #edf2f7;border-radius:14px;background:#f8fafc;}
        .gh-admin-einv__kv small{display:block;color:#8d5d5d;font-size:11px;text-transform:uppercase;font-weight:700;}
        .gh-admin-einv__kv strong{display:block;margin-top:6px;color:#111827;font-size:15px;line-height:1.5;word-break:break-word;}
        .gh-admin-einv__lines{margin-top:14px;border-top:1px dashed #eddede;padding-top:14px;}
        .gh-admin-einv__lines table{width:100%;border-collapse:collapse;}
        .gh-admin-einv__lines th,.gh-admin-einv__lines td{padding:10px 8px;border-bottom:1px solid #edf2f7;text-align:left;vertical-align:top;}
        .gh-admin-einv__empty{padding:28px 12px;text-align:center;color:#6b7280;}
        @media (max-width:767px){.gh-admin-einv__filters{grid-template-columns:1fr;}.gh-admin-einv__split{grid-template-columns:1fr;}.gh-admin-einv__title{font-size:26px;}.gh-admin-einv__preview-grid{grid-template-columns:1fr;}}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-admin-einv">
        <section class="gh-admin-einv__hero">
            <div>
                <div class="gh-admin-einv__kicker">Chứng từ public</div>
                <h1 class="gh-admin-einv__title">Hóa đơn điện tử /gianhang</h1>
                <p class="gh-admin-einv__sub">
                    Đây là lớp tra cứu hóa đơn điện tử public của chính workspace <code>/gianhang</code>, đặt trong <code>/gianhang/admin</code> để mình có thể mở lại link gửi khách,
                    rà nhanh chứng từ đã mirror sang lớp hóa đơn admin và xem bản preview trước khi copy/chia sẻ.
                </p>
            </div>
            <div class="gh-admin-einv__actions">
                <a class="gh-admin-einv__btn gh-admin-einv__btn--primary" href="<%=OrdersUrl %>">Đơn gian hàng</a>
                <a class="gh-admin-einv__btn gh-admin-einv__btn--soft" href="<%=LegacyInvoiceUrl %>">Hóa đơn admin</a>
                <a class="gh-admin-einv__btn gh-admin-einv__btn--soft" href="<%=BuyerFlowUrl %>">Buyer-flow / Đơn mua</a>
                <a class="gh-admin-einv__btn gh-admin-einv__btn--soft" href="<%=HubUrl %>">Hub /gianhang</a>
            </div>
        </section>

        <section class="gh-admin-einv__card">
            <div class="gh-admin-einv__filters">
                <div class="gh-admin-einv__field">
                    <label for="<%= txt_search.ClientID %>">Tìm hóa đơn điện tử</label>
                    <asp:TextBox ID="txt_search" runat="server" CssClass="gh-admin-einv__input" MaxLength="100" placeholder="ID đơn, GUID hóa đơn, tên khách, số điện thoại"></asp:TextBox>
                </div>
                <asp:Button ID="btn_filter" runat="server" Text="Lọc hóa đơn" CssClass="gh-admin-einv__submit" OnClick="btn_filter_Click" />
                <asp:Button ID="btn_clear" runat="server" Text="Đặt lại" CssClass="gh-admin-einv__reset" CausesValidation="false" OnClick="btn_clear_Click" />
            </div>
        </section>

        <div class="gh-admin-einv__summary">
            <div class="gh-admin-einv__summary-card">
                <small>Tổng hóa đơn native</small>
                <strong><%=SummaryTotal.ToString("#,##0") %></strong>
                <span>Toàn bộ hóa đơn được tạo từ lõi đơn hàng /gianhang của workspace hiện tại.</span>
            </div>
            <div class="gh-admin-einv__summary-card">
                <small>Đã thanh toán</small>
                <strong><%=SummaryPaid.ToString("#,##0") %></strong>
                <span>Những hóa đơn đã chốt thanh toán ở lớp native hoặc mirror.</span>
            </div>
            <div class="gh-admin-einv__summary-card">
                <small>Chờ xử lý</small>
                <strong><%=SummaryPending.ToString("#,##0") %></strong>
                <span>Các hóa đơn còn đang chờ buyer hoàn tất thanh toán hoặc trao đổi.</span>
            </div>
            <div class="gh-admin-einv__summary-card">
                <small>Đã mirror admin</small>
                <strong><%=SummaryMirrored.ToString("#,##0") %></strong>
                <span>Đã có record tương ứng trong lớp hóa đơn admin để staff vận hành sâu hơn.</span>
            </div>
        </div>

        <div class="gh-admin-einv__split">
            <section class="gh-admin-einv__card">
                <div class="gh-admin-einv__table-wrap">
                    <table class="gh-admin-einv__table">
                        <thead>
                            <tr>
                                <th style="width:120px;">ID đơn</th>
                                <th style="min-width:140px;">Ngày tạo</th>
                                <th style="min-width:220px;">Khách hàng</th>
                                <th style="min-width:140px;">Trạng thái</th>
                                <th style="min-width:140px;">Tổng tiền</th>
                                <th style="min-width:220px;">Thao tác</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rp_invoices" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <strong>#<%# Eval("DisplayId") %></strong><br />
                                            <span class="fg-gray"><%# Eval("GuideText") %></span>
                                        </td>
                                        <td><%# Eval("CreatedAtText") %></td>
                                        <td>
                                            <strong><%# Eval("CustomerDisplay") %></strong><br />
                                            <span class="fg-gray"><%# Eval("CustomerPhone") %></span>
                                        </td>
                                        <td><span class='gh-admin-einv__badge <%# Eval("StatusCss") %>'><%# Eval("StatusText") %></span></td>
                                        <td><strong><%# Eval("TotalText") %></strong></td>
                                        <td>
                                            <div class="gh-admin-einv__links">
                                                <a class="gh-admin-einv__link" href="<%# Eval("PreviewUrl") %>">Preview</a>
                                                <a class="gh-admin-einv__link" href="<%# Eval("PublicUrl") %>" target="_blank">Mở public</a>
                                                <asp:PlaceHolder ID="ph_legacy" runat="server" Visible='<%# Convert.ToBoolean(Eval("HasLegacyMirror")) %>'>
                                                    <a class="gh-admin-einv__link" href="<%# Eval("LegacyUrl") %>">Hóa đơn admin</a>
                                                </asp:PlaceHolder>
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                                <tr>
                                    <td colspan="6" class="gh-admin-einv__empty">Chưa có hóa đơn native nào để tra cứu điện tử trong workspace này.</td>
                                </tr>
                            </asp:PlaceHolder>
                        </tbody>
                    </table>
                </div>
            </section>

            <section class="gh-admin-einv__card">
                <asp:PlaceHolder ID="ph_preview" runat="server" Visible="false">
                    <h3 style="margin:0;color:#7f1d1d;">Preview hóa đơn đang chọn</h3>
                    <p style="margin:8px 0 0;color:#6b7280;line-height:1.7;">Dùng block này để rà nhanh trước khi mở public hoặc gửi lại link cho khách.</p>
                    <div class="gh-admin-einv__preview-grid mt-4">
                        <div class="gh-admin-einv__kv"><small>ID đơn</small><strong><%=PreviewPublicId %></strong></div>
                        <div class="gh-admin-einv__kv"><small>GUID public</small><strong><%=PreviewGuideId %></strong></div>
                        <div class="gh-admin-einv__kv"><small>Khách hàng</small><strong><%=PreviewCustomer %></strong></div>
                        <div class="gh-admin-einv__kv"><small>Điện thoại</small><strong><%=PreviewPhone %></strong></div>
                        <div class="gh-admin-einv__kv"><small>Ngày tạo</small><strong><%=PreviewCreatedAt %></strong></div>
                        <div class="gh-admin-einv__kv"><small>Tổng tiền</small><strong><%=PreviewTotal %></strong></div>
                    </div>
                    <div class="gh-admin-einv__links mt-4">
                        <a class="gh-admin-einv__link" href="<%=PreviewPublicUrl %>" target="_blank">Mở hóa đơn public</a>
                        <asp:PlaceHolder ID="ph_preview_legacy" runat="server" Visible="false">
                            <a class="gh-admin-einv__link" href="<%=PreviewLegacyUrl %>">Mở hóa đơn admin</a>
                        </asp:PlaceHolder>
                    </div>
                    <div class="gh-admin-einv__lines">
                        <h4 style="margin:0 0 10px;color:#1f2937;">Dòng hàng</h4>
                        <table>
                            <thead>
                                <tr>
                                    <th>Tên SP/DV</th>
                                    <th style="width:90px;">SL</th>
                                    <th style="width:120px;">Giá</th>
                                    <th style="width:140px;">Sau CK</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rp_lines" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><strong><%# Eval("ten_dichvu_sanpham") %></strong></td>
                                            <td><%# Eval("soluong") %></td>
                                            <td><%# FormatLineMoney(Eval("gia")) %></td>
                                            <td><%# FormatLineMoney(Eval("sauck")) %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="ph_no_preview" runat="server" Visible="false">
                    <div class="gh-admin-einv__empty">Chọn một hóa đơn ở danh sách bên trái để xem preview điện tử.</div>
                </asp:PlaceHolder>
            </section>
        </div>
    </div>
</asp:Content>
