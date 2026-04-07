<%@ Page Title="" Language="C#" MasterPageFile="~/gianhang/mp-home.master" AutoEventWireup="true" CodeFile="chi-tiet-bai-viet.aspx.cs" Inherits="danh_sach_bai_viet" %>


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
            width: min(1120px, calc(100% - 24px));
            margin: 0 auto;
            padding: 28px 0 40px;
        }

        .gh-article-card {
            border: 1px solid #fed7aa;
            border-radius: 28px;
            overflow: hidden;
            background: #fff;
            box-shadow: 0 20px 40px rgba(15,23,42,.08);
        }

        .gh-article-hero {
            padding: 22px;
            background:
                linear-gradient(125deg, rgba(249,115,22,.92), rgba(251,146,60,.84)),
                url('<%= Helper_cl.VersionedUrl("~/uploads/images/gianhang-banner-home-style.png") %>') center center / cover no-repeat;
            color: #fff;
        }

        .gh-article-kicker {
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

        .gh-article-title {
            margin-top: 12px;
            font-size: 36px;
            line-height: 1.08;
            font-weight: 900;
            color: #fff;
        }

        .gh-article-desc {
            margin-top: 10px;
            max-width: 760px;
            color: rgba(255,255,255,.92);
            font-size: 15px;
            line-height: 1.6;
        }

        .gh-article-body {
            padding: 22px;
        }

        .gh-article-cover {
            width: 100%;
            border-radius: 24px;
            overflow: hidden;
            background: #fff7ed;
            border: 1px solid #fde3c5;
        }

        .gh-article-cover img {
            width: 100%;
            display: block;
            object-fit: cover;
        }

        .gh-article-content {
            margin-top: 18px;
            color: #17233b;
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
            .gh-article-title {
                font-size: 28px;
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
            <div class="gh-article-card">
                <div class="gh-article-hero">
                    <div class="gh-article-kicker">Bài viết công khai</div>
                    <div class="gh-article-title"><%=title_web %></div>
                    <div class="gh-article-desc"><%=des %></div>
                </div>
                <div class="gh-article-body">
                    <div class="gh-article-cover">
                        <img src="<%=image %>" alt="<%=title_web %>" />
                    </div>
                    <div class="gh-article-content text-just chitiet-baiviet-bc">
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
                                    <a href="<%# BuildArticleUrl(Eval("id")) %>">
                                        <img src="<%#Eval("image") %>" alt="<%#Eval("name") %>" /></a>
                                </div>
                                <div>
                                    <div class="gh-related-name">
                                        <a href="<%# BuildArticleUrl(Eval("id")) %>"><%#Eval("name") %></a>
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
