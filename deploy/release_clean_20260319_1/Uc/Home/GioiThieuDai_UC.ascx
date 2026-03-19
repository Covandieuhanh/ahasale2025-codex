<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GioiThieuDai_UC.ascx.cs" Inherits="Uc_Home_GioiThieuDai_UC" %>
<div class="container-xl my-3">
    <div class="card">
        <div class="card-body">
            <h2 class="h4 mb-3">
                <asp:Literal ID="lit_about_title" runat="server"></asp:Literal>
            </h2>

            <div class="text-secondary dm-seo dm-seo-scroll">
                <asp:Literal ID="lit_about_content" runat="server"></asp:Literal>
            </div>
        </div>
    </div>
</div>

<style>
    .dm-seo p {
        margin-bottom: .6rem;
        line-height: 1.55;
    }

    .dm-seo-scroll {
        max-height: 260px;
        overflow-y: auto;
        overflow-x: hidden;
        padding-right: 8px;
    }

    .dm-seo-scroll::-webkit-scrollbar {
        width: 6px;
    }

    .dm-seo-scroll::-webkit-scrollbar-thumb {
        background: rgba(0,0,0,.15);
        border-radius: 999px;
    }

    .dm-seo-scroll::-webkit-scrollbar-track {
        background: transparent;
    }

    html[data-bs-theme="dark"] .dm-seo-scroll::-webkit-scrollbar-thumb {
        background: rgba(148,163,184,.35);
    }
</style>
