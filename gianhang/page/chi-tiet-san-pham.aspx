<%@ Page Title="" Language="C#" MasterPageFile="~/gianhang/mp-home.master" AutoEventWireup="true" CodeFile="chi-tiet-san-pham.aspx.cs" Inherits="danh_sach_bai_viet" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <%=meta %>
    <meta property="og:type" content="article" />
    <style>
        .gh-public-shell {
            background:
                radial-gradient(920px 320px at 18% -4%, rgba(249, 115, 22, .14), transparent 62%),
                radial-gradient(920px 320px at 88% -6%, rgba(251, 146, 60, .18), transparent 62%),
                #fff7ed;
        }

        .gh-detail-shell {
            width: min(1180px, calc(100% - 24px));
            margin: 0 auto;
            padding: 28px 0 40px;
        }

        .gh-detail-card {
            border: 1px solid #fed7aa;
            border-radius: 28px;
            overflow: hidden;
            background: #fff;
            box-shadow: 0 20px 40px rgba(15,23,42,.08);
        }

        .gh-detail-hero {
            padding: 22px;
            background:
                linear-gradient(125deg, rgba(249,115,22,.92), rgba(251,146,60,.84)),
                url('<%= Helper_cl.VersionedUrl("~/uploads/images/gianhang-banner-home-style.png") %>') center center / cover no-repeat;
            color: #fff;
        }

        .gh-detail-kicker {
            display: inline-flex;
            align-items: center;
            min-height: 34px;
            padding: 0 12px;
            border-radius: 999px;
            background: rgba(255,255,255,.16);
            border: 1px solid rgba(255,255,255,.28);
            font-size: 12px;
            font-weight: 800;
            letter-spacing: .03em;
            text-transform: uppercase;
        }

        .gh-detail-title {
            margin-top: 12px;
            font-size: 36px;
            line-height: 1.08;
            font-weight: 900;
            color: #fff;
        }

        .gh-detail-desc {
            margin-top: 10px;
            max-width: 760px;
            color: rgba(255,255,255,.92);
            font-size: 15px;
            line-height: 1.6;
        }

        .gh-detail-layout {
            display: grid;
            grid-template-columns: minmax(320px, 460px) minmax(0, 1fr);
            gap: 22px;
            padding: 22px;
        }

        .gh-detail-media {
            width: 100%;
            border-radius: 24px;
            overflow: hidden;
            background: #fff7ed;
            border: 1px solid #fde3c5;
        }

        .gh-detail-media img {
            width: 100%;
            display: block;
            object-fit: cover;
        }

        .gh-detail-panel {
            display: flex;
            flex-direction: column;
            min-width: 0;
        }

        .gh-detail-trust,
        .gh-detail-meta {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
        }

        .gh-pill,
        .gh-meta-chip {
            display: inline-flex;
            align-items: center;
            min-height: 34px;
            padding: 0 12px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 800;
        }

        .gh-pill {
            background: #fff7ed;
            color: #9a3412;
            border: 1px solid #fed7aa;
        }

        .gh-meta-chip {
            background: #fffaf5;
            color: #7c2d12;
            border: 1px solid #fde3c5;
        }

        .gh-detail-price {
            margin-top: 16px;
            font-size: 30px;
            line-height: 1.1;
            font-weight: 900;
            color: #dc2626;
        }

        .gh-detail-actions {
            margin-top: 18px;
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
        }

        .gh-detail-actions .secondary,
        .gh-detail-actions .secondary:visited {
            min-height: 42px;
            padding: 0 18px;
            border-radius: 999px;
            background: #fff7ed !important;
            color: #9a3412 !important;
            border: 1px solid #fdba74 !important;
            font-weight: 800;
        }

        .gh-detail-actions .bg-emerald,
        .gh-detail-actions .bg-emerald:visited {
            min-height: 42px;
            padding: 0 18px;
            border-radius: 999px;
            background: #f97316 !important;
            color: #fff !important;
            border: 1px solid #f97316 !important;
            font-weight: 800;
        }

        .gh-detail-actions .secondary:hover,
        .gh-detail-actions .secondary:focus {
            background: #ffedd5 !important;
            color: #7c2d12 !important;
        }

        .gh-detail-actions .bg-emerald:hover,
        .gh-detail-actions .bg-emerald:focus {
            background: #ea580c !important;
            color: #fff !important;
        }

        .gh-detail-content {
            margin-top: 18px;
            padding: 22px;
            border-top: 1px solid #f7e8d8;
        }

        .gh-detail-content h2 {
            margin: 0 0 12px;
            font-size: 24px;
            line-height: 1.2;
            color: #17233b;
            font-weight: 900;
        }

        .gh-related {
            margin-top: 18px;
            padding: 22px;
            border-top: 1px solid #f7e8d8;
        }

        .gh-related-title {
            margin: 0 0 14px;
            font-size: 24px;
            line-height: 1.2;
            color: #17233b;
            font-weight: 900;
        }

        .gh-related-item {
            display: grid;
            grid-template-columns: 180px 1fr;
            gap: 14px;
            padding-top: 16px;
            margin-top: 16px;
            border-top: 1px solid #f3e8d8;
        }

        .gh-related-item:first-child {
            margin-top: 0;
            padding-top: 0;
            border-top: 0;
        }

        .gh-related-item img {
            width: 100%;
            border-radius: 16px;
            display: block;
            object-fit: cover;
        }

        .gh-related-name a {
            color: #17233b;
            font-size: 18px;
            line-height: 1.35;
            font-weight: 800;
            text-decoration: none;
        }

        .gh-related-desc {
            margin-top: 8px;
            color: #64748b;
            line-height: 1.6;
        }

        @media (max-width: 767.98px) {
            .gh-detail-title {
                font-size: 28px;
            }

            .gh-detail-layout {
                grid-template-columns: 1fr;
            }

            .gh-detail-actions {
                flex-direction: column;
            }

            .gh-detail-actions .secondary,
            .gh-detail-actions .bg-emerald,
            .gh-detail-actions .input-small {
                width: 100%;
            }

            .gh-related-item {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="gh-public-shell">
        <div class="gh-detail-shell">
            <div class="gh-detail-card">
                <div class="gh-detail-hero">
                    <div class="gh-detail-kicker">Sản phẩm công khai</div>
                    <div class="gh-detail-title"><%=title_web %></div>
                    <div class="gh-detail-desc"><%=des %></div>
                </div>

                <div class="gh-detail-layout">
                    <div class="gh-detail-media">
                        <img src="<%=image %>" alt="<%=title_web %>" />
                    </div>
                    <div class="gh-detail-panel">
                        <div class="gh-detail-trust">
                            <span class="gh-pill">Hiển thị công khai trong gian hàng</span>
                            <span class="gh-pill">Trao đổi trực tiếp bằng tài khoản AhaSale</span>
                        </div>
                        <div class="gh-detail-price">Giá sản phẩm: <%=gia %> VNĐ</div>
                        <div class="gh-detail-meta mt-4">
                            <span class="gh-meta-chip">Đặt hàng trực tiếp từ gian hàng</span>
                            <span class="gh-meta-chip">Xem thêm các sản phẩm liên quan phía dưới</span>
                        </div>
                        <div class="gh-detail-actions">
                            <asp:TextBox ID="txt_soluong_dv" runat="server" data-role="spinner" data-min-value="1" value="1"></asp:TextBox>
                            <asp:Button ID="but_themvaogio" runat="server" Text="Thêm vào giỏ" CssClass="secondary" OnClick="but_themvaogio_Click" />
                            <asp:Button ID="but_dathang" runat="server" Text="Đặt hàng" CssClass="bg-emerald bg-darkEmerald-hover fg-white" OnClick="but_dathang_Click" />
                        </div>
                    </div>
                </div>

                <div class="gh-detail-content">
                    <h2>Nội dung chi tiết</h2>
                    <div class="text-just chitiet-baiviet-bc">
                        <%=noidung %>
                    </div>
                </div>

                <div class="gh-related">
                    <asp:Panel ID="Panel1" runat="server">
                        <h2 class="gh-related-title">
                            <a class="fg-black fg-emerald-hover" href="<%=url_menu %>">Cùng chủ đề</a>
                        </h2>
                    </asp:Panel>
                    <asp:Repeater ID="Repeater2" runat="server">
                        <ItemTemplate>
                            <div class="gh-related-item">
                                <div>
                                    <a href="<%# BuildRelatedDetailUrl(Eval("id")) %>">
                                        <img src="<%#Eval("image") %>" alt="<%#Eval("name") %>" /></a>
                                </div>
                                <div>
                                    <div class="gh-related-name">
                                        <a href="<%# BuildRelatedDetailUrl(Eval("id")) %>"><%#Eval("name") %></a>
                                    </div>
                                    <div class="gh-related-desc">
                                        <%#Eval("description") %>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>
