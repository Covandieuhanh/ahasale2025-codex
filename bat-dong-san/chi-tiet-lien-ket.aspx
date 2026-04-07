<%@ Page Title="Tin liên kết BĐS" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerBatDongSan.master" AutoEventWireup="true" CodeFile="chi-tiet-lien-ket.aspx.cs" Inherits="bat_dong_san_chi_tiet_lien_ket" %>
<%@ Register Src="~/bat-dong-san/uc-bds-nav.ascx" TagPrefix="uc" TagName="BdsNav" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">AhaSale / Bất động sản / Tin liên kết</div>
                    <h2 class="page-title"><%= Post == null ? "Tin liên kết" : Server.HtmlEncode(Post.Title) %></h2>
                    <div class="text-muted small mt-1">
                        <span class="me-3"><i class="ti ti-clock"></i> <%= Post == null ? "-" : Post.PublishedAt.ToString("dd/MM HH:mm") %></span>
                        <span class="badge bg-secondary-lt text-secondary">Liên kết</span>
                        <span class="badge bg-azure-lt text-azure"><%= Post == null ? "" : Server.HtmlEncode(Post.Source) %></span>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="mb-3">
                <uc:BdsNav ID="BdsNavMain" runat="server" Current="overview" ShowTopbar="false" IncludeAssets="false" ShowBanner="false" ShowNav="false" />
            </div>
            <asp:PlaceHolder ID="phDetail" runat="server">
                <div class="card">
                    <div class="card-body d-flex flex-column gap-3">
                        <div class="text-muted small"><%= Post == null ? "" : Server.HtmlEncode(Post.District + ", " + Post.Province) %></div>
                        <p class="mb-0"><%= Post == null ? "" : Server.HtmlEncode(Post.Summary) %></p>
                        <div class="btn-list">
                            <a class="btn btn-success" href="<%= ResolveSourceUrl() %>" target="_blank" rel="noopener">Đọc tại nguồn</a>
                            <a class="btn btn-outline-success" href="/bat-dong-san">Về trang BĐS</a>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
                <div class="card">
                    <div class="card-body text-muted">Không tìm thấy tin liên kết.</div>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentFoot" ContentPlaceHolderID="foot_sau" runat="Server">
</asp:Content>
