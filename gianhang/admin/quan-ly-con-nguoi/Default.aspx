<%@ Page Title="Hồ sơ người" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="gianhang_person_hub_default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .personhub-note {
            border: 1px solid #d7e8d8;
            border-radius: 16px;
            background: linear-gradient(180deg, #f7fff7 0%, #ffffff 100%);
            padding: 16px 18px;
        }

        .personhub-card {
            border: 1px solid #dce7f4;
            border-radius: 18px;
            background: #fff;
            box-shadow: 0 14px 30px rgba(15, 34, 61, 0.06);
            overflow: hidden;
        }

        .personhub-card__head {
            padding: 18px 20px;
            border-bottom: 1px solid #edf3fb;
            background: linear-gradient(180deg, #f8fbff 0%, #ffffff 100%);
        }

        .personhub-card__title {
            font-size: 20px;
            font-weight: 700;
            color: #17324a;
        }

        .personhub-card__sub {
            margin-top: 6px;
            color: #5d7284;
            line-height: 1.6;
        }

        .personhub-summary-grid {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 12px;
            margin-top: 20px;
        }

        .personhub-summary-link {
            display: block;
            text-decoration: none;
            border: 1px solid #dce7f4;
            border-radius: 16px;
            padding: 14px 16px;
            background: linear-gradient(180deg, #fbfdff 0%, #ffffff 100%);
            color: #17324a;
            box-shadow: 0 10px 24px rgba(15, 34, 61, 0.05);
        }

        .personhub-summary-link:hover {
            text-decoration: none;
            border-color: #9bc5ff;
            box-shadow: 0 14px 28px rgba(15, 34, 61, 0.08);
        }

        .personhub-summary-link__label {
            font-size: 11px;
            text-transform: uppercase;
            letter-spacing: .08em;
            color: #6e8092;
            font-weight: 700;
        }

        .personhub-summary-link__value {
            margin-top: 8px;
            font-size: 28px;
            font-weight: 700;
            color: #17324a;
        }

        .personhub-filter-bar {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            align-items: center;
        }

        .personhub-filter-bar .input-large {
            max-width: 280px;
        }

        .personhub-role-chip {
            display: inline-block;
            margin: 4px 6px 0 0;
            padding: 4px 10px;
            border-radius: 999px;
            background: #eef6ff;
            color: #22507a;
            font-size: 12px;
            font-weight: 600;
        }

        .personhub-empty {
            padding: 28px 20px;
            text-align: center;
            color: #71869a;
        }

        @media (max-width: 767px) {
            .personhub-summary-grid {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }

        @media (max-width: 640px) {
            .personhub-summary-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id="main-content" class="mb-10">
        <div class="row border-bottom bd-light">
            <div class="cell-6">
                <ul class="h-menu">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Trang chủ gian hàng admin">
                        <a class="button" href="/gianhang/admin/default.aspx"><span class="mif mif-home"></span></a>
                    </li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                </ul>
            </div>
        </div>

        <div class="personhub-note mt-4">
            <div class="text-bold">Hồ sơ người là nơi duy nhất để gắn tài khoản Home trong `/gianhang/admin`.</div>
            <div class="mt-1 fg-gray">
                Khi liên kết tại đây, hệ thống sẽ tự nhận toàn bộ vai trò cùng số điện thoại trong không gian hiện tại như nhân sự nội bộ, khách hàng, chuyên gia đào tạo và thành viên.
            </div>
        </div>

        <div class="personhub-card mt-5">
            <div class="personhub-card__head">
                <div class="personhub-card__title">Danh sách hồ sơ người</div>
                <div class="personhub-card__sub">
                    Chỉ những hồ sơ đã có số điện thoại mới được gom và liên kết tự động. Nếu một module gốc còn thiếu số điện thoại, hãy cập nhật số đó trước để hồ sơ xuất hiện ở đây.
                </div>

                <div class="personhub-summary-grid">
                    <a class="personhub-summary-link" href="<%=BuildFilterUrl("all") %>">
                        <div class="personhub-summary-link__label">Tổng hồ sơ người</div>
                        <div class="personhub-summary-link__value"><%=summary_total %></div>
                    </a>
                    <a class="personhub-summary-link" href="<%=BuildFilterUrl("active") %>">
                        <div class="personhub-summary-link__label">Đã liên kết Home</div>
                        <div class="personhub-summary-link__value"><%=summary_active %></div>
                    </a>
                    <a class="personhub-summary-link" href="<%=BuildFilterUrl("pending") %>">
                        <div class="personhub-summary-link__label">Chờ liên kết</div>
                        <div class="personhub-summary-link__value"><%=summary_pending %></div>
                    </a>
                    <a class="personhub-summary-link" href="<%=BuildFilterUrl("none") %>">
                        <div class="personhub-summary-link__label">Chưa liên kết</div>
                        <div class="personhub-summary-link__value"><%=summary_none %></div>
                    </a>
                </div>

                <div class="personhub-filter-bar mt-4">
                    <asp:TextBox ID="txt_keyword" runat="server" CssClass="input-large" placeholder="Tìm theo tên, số điện thoại, vai trò, tài khoản Home"></asp:TextBox>
                    <asp:Button ID="but_search" runat="server" Text="Lọc" CssClass="button primary" OnClick="but_search_Click" />
                    <a class="button <%=FilterButtonCss("all") %>" href="<%=BuildFilterUrl("all") %>">Tất cả</a>
                    <a class="button <%=FilterButtonCss("active") %>" href="<%=BuildFilterUrl("active") %>">Đã liên kết</a>
                    <a class="button <%=FilterButtonCss("pending") %>" href="<%=BuildFilterUrl("pending") %>">Chờ liên kết</a>
                    <a class="button <%=FilterButtonCss("none") %>" href="<%=BuildFilterUrl("none") %>">Chưa liên kết</a>
                </div>

                <div class="personhub-filter-bar mt-3">
                    <a class="button <%=SourceButtonCss("all") %>" href="<%=BuildFilterUrl(Request.QueryString["status"], "all", Request.QueryString["lifecycle"]) %>">Mọi nguồn</a>
                    <a class="button <%=SourceButtonCss("staff") %>" href="<%=BuildFilterUrl(Request.QueryString["status"], "staff", Request.QueryString["lifecycle"]) %>">Nhân sự nội bộ</a>
                    <a class="button <%=SourceButtonCss("customer") %>" href="<%=BuildFilterUrl(Request.QueryString["status"], "customer", Request.QueryString["lifecycle"]) %>">Khách hàng</a>
                    <a class="button <%=SourceButtonCss("lecturer") %>" href="<%=BuildFilterUrl(Request.QueryString["status"], "lecturer", Request.QueryString["lifecycle"]) %>">Chuyên gia đào tạo</a>
                    <a class="button <%=SourceButtonCss("member") %>" href="<%=BuildFilterUrl(Request.QueryString["status"], "member", Request.QueryString["lifecycle"]) %>">Thành viên / Học viên</a>
                </div>

                <div class="personhub-filter-bar mt-3">
                    <a class="button <%=LifecycleButtonCss("all") %>" href="<%=BuildFilterUrl(Request.QueryString["status"], Request.QueryString["source"], "all") %>">Mọi hồ sơ</a>
                    <a class="button <%=LifecycleButtonCss("removed") %>" href="<%=BuildFilterUrl(Request.QueryString["status"], Request.QueryString["source"], "removed") %>">Có vai trò đã gỡ</a>
                    <a class="button <%=LifecycleButtonCss("orphaned") %>" href="<%=BuildFilterUrl(Request.QueryString["status"], Request.QueryString["source"], "orphaned") %>">Không còn vai trò nguồn</a>
                </div>
            </div>

            <div style="overflow:auto">
                <table class="table row-hover table-border cell-border compact normal-lg">
                    <thead>
                        <tr style="background-color: #f5f5f5">
                            <td class="text-bold" style="min-width: 220px;">Người</td>
                            <td class="text-bold" style="min-width: 220px;">Vai trò trong gian hàng</td>
                            <td class="text-bold" style="min-width: 220px;">Liên kết Home</td>
                            <td class="text-bold text-center" style="width: 1px;">Nguồn</td>
                            <td class="text-bold text-center" style="width: 1px;">Thao tác</td>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="Repeater1" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <div class="text-bold"><%#HttpUtility.HtmlEncode(Eval("PrimaryName").ToString()) %></div>
                                        <div class="fg-cobalt"><%#HttpUtility.HtmlEncode(Eval("DisplayPhone").ToString()) %></div>
                                    </td>
                                    <td>
                                        <%#RenderRoleChips(Eval("RolesSummary").ToString()) %>
                                    </td>
                                    <td>
                                        <span class="data-wrapper"><code class="<%# HttpUtility.HtmlEncode(((GianHangAdminPersonHub_cl.PersonSummary)Container.DataItem).LinkInfo.LinkCss) %>"><%# HttpUtility.HtmlEncode(((GianHangAdminPersonHub_cl.PersonSummary)Container.DataItem).LinkInfo.StatusLabel) %></code></span>
                                        <div class="mt-1"><%#RenderLinkMeta((GianHangAdminPersonHub_cl.PersonSummary)Container.DataItem) %></div>
                                    </td>
                                    <td class="text-center"><%#RenderSourceCountHtml((GianHangAdminPersonHub_cl.PersonSummary)Container.DataItem) %></td>
                                    <td class="text-center">
                                        <a class="button success small" href="<%#HttpUtility.HtmlAttributeEncode(Eval("DetailUrl").ToString()) %>">Mở hồ sơ</a>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
                <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                    <div class="personhub-empty">Chưa có hồ sơ người phù hợp với bộ lọc hiện tại.</div>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>
</asp:Content>
