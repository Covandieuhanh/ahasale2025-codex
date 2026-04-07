<%@ Page Title="" Language="C#" MasterPageFile="~/gianhang/mp-home.master" AutoEventWireup="true" CodeFile="danh-sach-bai-viet.aspx.cs" Inherits="danh_sach_bai_viet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <%=meta %>
    <meta property="og:type" content="object" />
    <style>
        .gh-list-shell {
            background:
                radial-gradient(920px 320px at 18% -4%, rgba(249, 115, 22, .14), transparent 62%),
                radial-gradient(920px 320px at 88% -6%, rgba(251, 146, 60, .18), transparent 62%),
                #fff7ed;
        }

        .gh-list-wrap {
            width: min(1180px, calc(100% - 24px));
            margin: 0 auto;
            padding: 28px 0 40px;
        }

        .gh-list-hero {
            border: 1px solid #fed7aa;
            border-radius: 28px;
            overflow: hidden;
            background:
                linear-gradient(125deg, rgba(249,115,22,.92), rgba(251,146,60,.84)),
                url('<%= Helper_cl.VersionedUrl("~/uploads/images/gianhang-banner-home-style.png") %>') center center / cover no-repeat;
            color: #fff;
            padding: 22px;
            box-shadow: 0 20px 40px rgba(15,23,42,.08);
        }

        .gh-list-kicker {
            display: inline-flex;
            align-items: center;
            min-height: 34px;
            padding: 0 12px;
            border-radius: 999px;
            background: rgba(255,255,255,.16);
            border: 1px solid rgba(255,255,255,.28);
            font-size: 12px;
            font-weight: 800;
            text-transform: uppercase;
            letter-spacing: .03em;
        }

        .gh-list-title {
            margin-top: 12px;
            font-size: 36px;
            line-height: 1.08;
            font-weight: 900;
            color: #fff;
        }

        .gh-list-desc {
            margin-top: 10px;
            max-width: 760px;
            color: rgba(255,255,255,.92);
            font-size: 15px;
            line-height: 1.6;
        }

        .gh-list-toolbar {
            margin-top: 16px;
            border: 1px solid #fed7aa;
            border-radius: 22px;
            background: linear-gradient(180deg, #fffdfb, #fff8f2);
            padding: 16px;
            box-shadow: 0 16px 32px rgba(15,23,42,.08);
        }

        .gh-list-search input { width: 100%; }

        .gh-list-grid {
            margin-top: 16px;
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 16px;
        }

        .gh-list-card {
            border: 1px solid #fde3c5;
            border-radius: 22px;
            overflow: hidden;
            background: #fff;
            box-shadow: 0 14px 28px rgba(15,23,42,.08);
            display: flex;
            flex-direction: column;
        }

        .gh-list-card img {
            width: 100%;
            aspect-ratio: 4 / 5;
            object-fit: cover;
            display: block;
            background: #fff7ed;
        }

        .gh-list-card-body {
            padding: 14px;
            display: flex;
            flex-direction: column;
            min-height: 154px;
        }

        .gh-list-card-name a {
            color: #17233b;
            font-size: 18px;
            line-height: 1.35;
            font-weight: 800;
            text-decoration: none;
        }

        .gh-list-card-desc {
            margin-top: 8px;
            color: #64748b;
            font-size: 13px;
            line-height: 1.55;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
        }

        .gh-list-card-actions {
            margin-top: auto;
            padding-top: 12px;
            display: grid;
            grid-template-columns: 1fr;
        }

        .gh-btn-soft,
        .gh-btn-soft:visited {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 40px;
            padding: 0 14px;
            border-radius: 999px;
            background: #fff7ed;
            color: #9a3412 !important;
            border: 1px solid #fdba74;
            font-weight: 800;
            text-decoration: none !important;
        }

        .gh-btn-soft:hover,
        .gh-btn-soft:focus {
            background: #ffedd5;
            color: #7c2d12 !important;
        }

        .gh-list-pagination {
            text-align: center;
            margin-top: 24px;
            padding-bottom: 8px;
        }

        @media (max-width: 991.98px) {
            .gh-list-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); }
        }

        @media (max-width: 767.98px) {
            .gh-list-title { font-size: 28px; }
            .gh-list-grid { grid-template-columns: 1fr; }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="gh-list-shell">
                <div class="gh-list-wrap">
                    <div class="gh-list-hero">
                        <div class="gh-list-kicker">Bài viết công khai</div>
                        <div class="gh-list-title"><%=tenmn %></div>
                        <%if (mota != "")
                            { %>
                        <div class="gh-list-desc"><%=mota %></div>
                        <%} %>
                    </div>
                    <div class="gh-list-toolbar">
                        <div class="gh-list-search">
                            <asp:TextBox ID="txt_search" runat="server" data-role="input" data-prepend="<span class='mif mif-search'></span>" placeholder="Tìm kiếm bài viết" OnTextChanged="txt_search_TextChanged" AutoPostBack="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="gh-list-grid">
                        <asp:Repeater ID="Repeater1" runat="server">
                            <ItemTemplate>
                                <div class="gh-list-card">
                                    <div>
                                        <a href="<%# BuildDetailUrl(Eval("id")) %>">
                                            <img src="<%#Eval("image") %>" alt="<%#Eval("name") %>" /></a>
                                    </div>
                                    <div class="gh-list-card-body">
                                        <div class="gh-list-card-name">
                                            <a href="<%# BuildDetailUrl(Eval("id")) %>"><%#Eval("name") %></a>
                                        </div>
                                        <div class="gh-list-card-desc">
                                            <%#Eval("description") %>
                                        </div>
                                        <div class="gh-list-card-actions">
                                            <a href="<%# BuildDetailUrl(Eval("id")) %>" class="gh-btn-soft">Xem thêm</a>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <div class="gh-list-pagination">
                        <asp:Button ID="but_quaylai" runat="server" Text="Quay lại" CssClass="alert rounded fg-white" OnClick="but_quaylai_Click" />
                        <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                        <asp:Button ID="but_xemtiep" runat="server" Text="Xem thêm" CssClass="alert rounded fg-white" OnClick="but_xemtiep_Click" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>
