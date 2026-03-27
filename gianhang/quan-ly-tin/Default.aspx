<%@ Page Title="Quản lý tin" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="gianhang_quan_ly_tin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server">
    <style>
        .gh-manage-shell { max-width: 1260px; margin: 0 auto; }
        .gh-manage-summary {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 12px;
            margin-bottom: 16px;
        }
        .gh-manage-summary-card {
            background: #fff;
            border: 1px solid #e5edf5;
            border-radius: 18px;
            box-shadow: 0 16px 40px rgba(15, 23, 42, .06);
            padding: 16px;
        }
        .gh-manage-summary-label { font-size: 12px; color: #6b7280; margin-bottom: 4px; }
        .gh-manage-summary-value { font-size: 28px; line-height: 1.05; font-weight: 800; color: #0f172a; }
        .gh-chip {
            display: inline-flex;
            align-items: center;
            min-height: 28px;
            padding: 0 10px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 700;
        }
        .gh-chip--product { background: #eff6ff; color: #1d4ed8; }
        .gh-chip--service { background: #ecfdf5; color: #047857; }
        .gh-chip--active { background: #ecfdf5; color: #047857; }
        .gh-chip--hidden { background: #fff7ed; color: #c2410c; }
        .gh-manage-thumb {
            width: 64px;
            height: 64px;
            object-fit: cover;
            border-radius: 14px;
            border: 1px solid #e5edf5;
            background: #f8fafc;
        }
        @media (max-width: 767px) {
            .gh-manage-table td, .gh-manage-table th { white-space: nowrap; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl gh-manage-shell py-4">
        <div class="page-header d-print-none mb-3">
            <div class="row align-items-center">
                <div class="col">
                    <div class="page-pretitle">Gian hàng</div>
                    <h2 class="page-title">Quản lý tin</h2>
                </div>
                <div class="col-auto ms-auto d-print-none">
                    <div class="btn-list">
                        <asp:HyperLink ID="lnk_back_storefront" runat="server" NavigateUrl="/gianhang/default.aspx" CssClass="btn btn-outline-secondary">Trang công khai</asp:HyperLink>
                        <asp:HyperLink ID="lnk_add_new" runat="server" NavigateUrl="/gianhang/quan-ly-tin/Them.aspx" CssClass="btn btn-primary">Đăng tin mới</asp:HyperLink>
                    </div>
                </div>
            </div>
        </div>

        <div class="gh-manage-summary">
            <div class="gh-manage-summary-card">
                <div class="gh-manage-summary-label">Tổng tin</div>
                <div class="gh-manage-summary-value"><asp:Label ID="lb_total" runat="server" Text="0"></asp:Label></div>
            </div>
            <div class="gh-manage-summary-card">
                <div class="gh-manage-summary-label">Đang hiển thị</div>
                <div class="gh-manage-summary-value"><asp:Label ID="lb_active" runat="server" Text="0"></asp:Label></div>
            </div>
            <div class="gh-manage-summary-card">
                <div class="gh-manage-summary-label">Đang ẩn</div>
                <div class="gh-manage-summary-value"><asp:Label ID="lb_hidden" runat="server" Text="0"></asp:Label></div>
            </div>
            <div class="gh-manage-summary-card">
                <div class="gh-manage-summary-label">Sản phẩm</div>
                <div class="gh-manage-summary-value"><asp:Label ID="lb_products" runat="server" Text="0"></asp:Label></div>
            </div>
            <div class="gh-manage-summary-card">
                <div class="gh-manage-summary-label">Dịch vụ</div>
                <div class="gh-manage-summary-value"><asp:Label ID="lb_services" runat="server" Text="0"></asp:Label></div>
            </div>
        </div>

        <div class="card shadow-sm mb-3">
            <div class="card-body">
                <div class="row g-3 align-items-end">
                    <div class="col-lg-5">
                        <label class="form-label">Tìm kiếm</label>
                        <asp:TextBox ID="txt_timkiem" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged" placeholder="Nhập tên tin hoặc ID"></asp:TextBox>
                    </div>
                    <div class="col-lg-3">
                        <label class="form-label">Loại tin</label>
                        <asp:DropDownList ID="ddl_loai" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddl_loai_SelectedIndexChanged">
                            <asp:ListItem Value="all">Tất cả</asp:ListItem>
                            <asp:ListItem Value="sanpham">Sản phẩm</asp:ListItem>
                            <asp:ListItem Value="dichvu">Dịch vụ</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-lg-3">
                        <label class="form-label">Trạng thái</label>
                        <asp:DropDownList ID="ddl_trangthai" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddl_trangthai_SelectedIndexChanged">
                            <asp:ListItem Value="all">Tất cả</asp:ListItem>
                            <asp:ListItem Value="active">Đang hiển thị</asp:ListItem>
                            <asp:ListItem Value="hidden">Đang ẩn</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-lg-1 d-grid">
                        <asp:Button ID="but_refresh" runat="server" CssClass="btn btn-outline-secondary" Text="Tải lại" OnClick="but_refresh_Click" />
                    </div>
                </div>
                <div class="text-muted small mt-2"><asp:Label ID="lb_result_summary" runat="server" Text=""></asp:Label></div>
            </div>
        </div>

        <div class="card shadow-sm">
            <div class="table-responsive">
                <table class="table table-vcenter card-table gh-manage-table">
                    <thead>
                        <tr>
                            <th style="width:80px;">ID</th>
                            <th style="width:90px;">Ảnh</th>
                            <th>Tên tin</th>
                            <th style="width:140px;">Loại</th>
                            <th style="width:150px;" class="text-end">Giá</th>
                            <th style="width:160px;">Trạng thái</th>
                            <th style="width:180px;">Danh mục</th>
                            <th style="width:160px;">Cập nhật</th>
                            <th style="width:220px;" class="text-end">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rpt_posts" runat="server" OnItemCommand="rpt_posts_ItemCommand">
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("Id") %></td>
                                    <td>
                                        <img src="<%# Eval("ImageUrl") %>" class="gh-manage-thumb" alt="" />
                                    </td>
                                    <td>
                                        <div class="fw-bold"><%# Eval("Name") %></div>
                                        <div class="text-muted small mt-1"><%# Eval("Description") %></div>
                                    </td>
                                    <td>
                                        <span class='<%# (Eval("PostType") ?? "").ToString() == "dichvu" ? "gh-chip gh-chip--service" : "gh-chip gh-chip--product" %>'>
                                            <%# (Eval("PostType") ?? "").ToString() == "dichvu" ? "Dịch vụ" : "Sản phẩm" %>
                                        </span>
                                    </td>
                                    <td class="text-end"><%# Eval("PriceText") %></td>
                                    <td>
                                        <span class='<%# (bool)Eval("IsHidden") ? "gh-chip gh-chip--hidden" : "gh-chip gh-chip--active" %>'>
                                            <%# (bool)Eval("IsHidden") ? "Đang ẩn" : "Đang hiển thị" %>
                                        </span>
                                    </td>
                                    <td><%# Eval("CategoryName") %></td>
                                    <td><%# Eval("UpdatedAtText") %></td>
                                    <td class="text-end">
                                        <div class="btn-list justify-content-end flex-nowrap">
                                            <asp:HyperLink ID="lnk_edit" runat="server" NavigateUrl='<%# "/gianhang/quan-ly-tin/Them.aspx?id=" + Eval("Id") %>' CssClass="btn btn-outline-primary btn-sm">Sửa</asp:HyperLink>
                                            <asp:LinkButton ID="but_toggle" runat="server" CommandName="toggle" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-outline-warning btn-sm">
                                                <%# (bool)Eval("IsHidden") ? "Hiện lại" : "Ẩn tin" %>
                                            </asp:LinkButton>
                                        </div>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
            <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                <div class="card-body text-center text-muted py-5">
                    Chưa có tin nào. Hãy đăng tin đầu tiên để bắt đầu vận hành gian hàng.
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
