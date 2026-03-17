<%@ Page Title="Reindex tìm kiếm bài viết" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master"
    AutoEventWireup="true" CodeFile="reindex-baiviet.aspx.cs" Inherits="admin_tools_reindex_baiviet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .reindex-card { max-width: 820px; }
        .reindex-status { font-weight: 600; }
        .reindex-note { color: #6b7280; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="p-3">
                <div class="panel reindex-card">
                    <div class="panel-header">
                        <h4 class="mb-0">Reindex tìm kiếm bài viết</h4>
                    </div>
                    <div class="panel-content">
                        <p class="reindex-note">
                            Công cụ này sẽ chuẩn hoá dữ liệu không dấu cho <strong>BaiViet_tb</strong> để tìm kiếm có dấu/không dấu chính xác.
                        </p>
                        <div class="mb-2">
                            <asp:Label ID="lbl_status" runat="server" CssClass="reindex-status"></asp:Label>
                        </div>
                        <div class="d-flex align-items-center">
                            <asp:Button ID="btn_run" runat="server" CssClass="button primary" Text="Chạy reindex"
                                OnClick="btn_run_Click" />
                            <asp:Button ID="btn_run_all" runat="server" CssClass="button success ml-2"
                                Text="Reindex toàn bộ (1 lần)" OnClick="btn_run_all_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
