<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TuKhoaPhoBien_UC.ascx.cs" Inherits="Uc_Home_TuKhoaPhoBien_UC" %>
<div class="container-xl my-3">
    <div class="card">
        <div class="card-body">
            <div class="fw-semibold text-secondary mb-3">
                <asp:Literal ID="lit_title_keywords" runat="server"></asp:Literal>
            </div>

            <div class="row g-3">
                <div class="col-6 col-md-3">
                    <ul class="list-unstyled dm-keywords">
                        <asp:Repeater ID="rpt_col_1" runat="server">
                            <ItemTemplate>
                                <li><a href="<%# Eval("Url") %>" class="dm-link"><%# Eval("Label") %></a></li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>

                <div class="col-6 col-md-3">
                    <ul class="list-unstyled dm-keywords">
                        <asp:Repeater ID="rpt_col_2" runat="server">
                            <ItemTemplate>
                                <li><a href="<%# Eval("Url") %>" class="dm-link"><%# Eval("Label") %></a></li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>

                <div class="col-6 col-md-3">
                    <ul class="list-unstyled dm-keywords">
                        <asp:Repeater ID="rpt_col_3" runat="server">
                            <ItemTemplate>
                                <li><a href="<%# Eval("Url") %>" class="dm-link"><%# Eval("Label") %></a></li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>

                <div class="col-6 col-md-3">
                    <ul class="list-unstyled dm-keywords">
                        <asp:Repeater ID="rpt_col_4" runat="server">
                            <ItemTemplate>
                                <li><a href="<%# Eval("Url") %>" class="dm-link"><%# Eval("Label") %></a></li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</div>
<style>
    .dm-keywords {
        margin: 0;
        padding: 0;
    }

    .dm-keywords li {
        position: relative;
        padding-left: 14px;
        margin-bottom: .45rem;
    }

    .dm-keywords li::before {
        content: "";
        width: 5px;
        height: 5px;
        border-radius: 999px;
        background: var(--tblr-secondary, #9ca3af);
        position: absolute;
        left: 0;
        top: .55em;
        opacity: .7;
    }

    .dm-link {
        color: var(--tblr-secondary, #6b7280);
        font-size: .875rem;
        text-decoration: none !important;
        transition: color .15s ease;
    }

    .dm-link:hover {
        color: var(--tblr-primary, #206bc4);
        text-decoration: underline;
        text-underline-offset: 2px;
    }
</style>
