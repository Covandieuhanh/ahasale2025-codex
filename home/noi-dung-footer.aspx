<%@ Page Title="Nội dung AhaSale" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="noi-dung-footer.aspx.cs" Inherits="home_noi_dung_footer" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head_truoc" runat="Server">
    <style>
        .footer-article-wrap {
            max-width: 980px;
        }

        .footer-article-content p {
            margin-bottom: .85rem;
            line-height: 1.7;
            color: #4b5563;
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl py-4 footer-article-wrap">
        <div class="card shadow-sm">
            <div class="card-body p-4 p-lg-5">
                <h1 class="h3 mb-2">
                    <asp:Literal ID="lit_title" runat="server"></asp:Literal>
                </h1>

                <div class="text-secondary mb-4">
                    Cập nhật:
                    <asp:Literal ID="lit_updated_at" runat="server"></asp:Literal>
                </div>

                <div class="footer-article-content">
                    <asp:Literal ID="lit_body_content" runat="server"></asp:Literal>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentFoot" ContentPlaceHolderID="foot_sau" runat="Server"></asp:Content>
