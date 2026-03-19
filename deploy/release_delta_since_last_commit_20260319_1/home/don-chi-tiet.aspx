<%@ Page Title="Chi tiết đơn hàng" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="don-chi-tiet.aspx.cs" Inherits="home_don_chi_tiet" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .order-meta .label {
            color: #6c7a91;
            font-size: 12px;
        }
        .order-meta .value {
            margin-top: 2px;
            font-weight: 600;
            word-break: break-word;
        }
        .order-money {
            white-space: nowrap;
        }
        .order-image {
            width: 60px;
            height: 60px;
            object-fit: cover;
            border-radius: 12px;
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="page-header d-print-none">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Đơn hàng</div>
                    <h2 class="page-title"><asp:Label ID="lb_page_title" runat="server" Text="Chi tiết đơn hàng"></asp:Label></h2>
                </div>
                <div class="col-auto ms-auto d-print-none">
                    <asp:HyperLink ID="hl_back_top" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                        <i class="ti ti-arrow-left"></i>&nbsp;Quay lại
                    </asp:HyperLink>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="card mb-3">
                <div class="card-body">
                    <div class="row g-3 order-meta">
                        <div class="col-6 col-md-3">
                            <div class="label">ID đơn</div>
                            <div class="value">#<asp:Label ID="lb_id" runat="server" Text="-"></asp:Label></div>
                        </div>
                        <div class="col-6 col-md-3">
                            <div class="label">Trạng thái</div>
                            <div class="value"><asp:Label ID="lb_status" runat="server" Text="-"></asp:Label></div>
                        </div>
                        <div class="col-6 col-md-3">
                            <div class="label">Loại đơn</div>
                            <div class="value"><asp:Label ID="lb_loai" runat="server" Text="-"></asp:Label></div>
                        </div>
                        <div class="col-6 col-md-3">
                            <div class="label">Ngày đặt</div>
                            <div class="value"><asp:Label ID="lb_ngaydat" runat="server" Text="-"></asp:Label></div>
                        </div>
                        <div class="col-12 col-md-4">
                            <div class="label">Tên gian hàng đối tác</div>
                            <div class="value"><asp:Label ID="lb_ten_shop" runat="server" Text="-"></asp:Label></div>
                        </div>
                        <div class="col-12 col-md-4">
                            <div class="label">Người mua</div>
                            <div class="value"><asp:Label ID="lb_nguoi_mua" runat="server" Text="-"></asp:Label></div>
                        </div>
                        <div class="col-12 col-md-4">
                            <div class="label">Người bán</div>
                            <div class="value"><asp:Label ID="lb_nguoi_ban" runat="server" Text="-"></asp:Label></div>
                        </div>
                        <div class="col-12 col-md-4">
                            <div class="label">Người nhận</div>
                            <div class="value"><asp:Label ID="lb_nguoinhan" runat="server" Text="-"></asp:Label></div>
                        </div>
                        <div class="col-12 col-md-4">
                            <div class="label">Số điện thoại nhận</div>
                            <div class="value"><asp:Label ID="lb_sdt" runat="server" Text="-"></asp:Label></div>
                        </div>
                        <div class="col-12 col-md-4">
                            <div class="label">Địa chỉ nhận</div>
                            <div class="value"><asp:Label ID="lb_diachi" runat="server" Text="-"></asp:Label></div>
                        </div>
                        <div class="col-6 col-md-3">
                            <div class="label">Tổng tiền (VNĐ)</div>
                            <div class="value order-money text-danger"><asp:Label ID="lb_tong_vnd" runat="server" Text="0"></asp:Label> đ</div>
                        </div>
                        <div class="col-6 col-md-3">
                            <div class="label">Tổng trao đổi (A)</div>
                            <div class="value order-money text-primary"><asp:Label ID="lb_tong_a" runat="server" Text="0"></asp:Label> A</div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="table-responsive">
                    <table class="table card-table table-vcenter">
                        <thead>
                            <tr>
                                <th style="width:1px;">ID</th>
                                <th style="width:90px;">Ảnh</th>
                                <th style="min-width:220px;">Tên sản phẩm</th>
                                <th class="text-end" style="min-width:120px;">Giá (VNĐ)</th>
                                <th class="text-center" style="min-width:90px;">Số lượng</th>
                                <th class="text-end" style="min-width:90px;">Ưu đãi (%)</th>
                                <th class="text-end" style="min-width:140px;">Thành tiền (VNĐ)</th>
                                <th class="text-end" style="min-width:120px;">Trao đổi (A)</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rp_chitiet" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="text-center"><%# Eval("id") %></td>
                                        <td class="text-center">
                                            <a href="<%# Eval("image") %>" target="_blank" class="d-inline-block">
                                                <img src="<%# Eval("image") %>" class="order-image" />
                                            </a>
                                        </td>
                                        <td>
                                            <a href="<%# BuildProductUrl(Eval("name_en"), Eval("idsp")) %>" class="text-decoration-none">
                                                <div class="fw-semibold"><%# Eval("name") %></div>
                                            </a>
                                            <asp:PlaceHolder ID="ph_review_row" runat="server" Visible='<%# Convert.ToBoolean(Eval("show_review")) %>'>
                                                <div class="mt-2">
                                                    <a class="btn btn-outline-primary btn-sm" href="<%# Eval("review_url") %>">Viết đánh giá</a>
                                                </div>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="ph_reviewed_row" runat="server" Visible='<%# Convert.ToBoolean(Eval("show_reviewed")) %>'>
                                                <div class="mt-2 text-muted small">Đã đánh giá</div>
                                            </asp:PlaceHolder>
                                        </td>
                                        <td class="text-end order-money"><%# Eval("giaban", "{0:#,##0}") %> ₫</td>
                                        <td class="text-center"><span class="badge bg-muted-lt text-muted"><%# Eval("soluong") %></span></td>
                                        <td class="text-end"><%# Eval("PhanTramUuDai") %>%</td>
                                        <td class="text-end order-money"><%# Eval("thanhtien", "{0:#,##0}") %> ₫</td>
                                        <td class="text-end order-money"><%# FormatA(Eval("thanhtien")) %> A</td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>

                            <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                                <tr>
                                    <td colspan="8" class="text-center text-muted py-4">Không có chi tiết đơn hàng.</td>
                                </tr>
                            </asp:PlaceHolder>
                        </tbody>
                    </table>
                </div>
            </div>

            <div class="mt-3">
                <asp:HyperLink ID="hl_back_bottom" runat="server" CssClass="btn btn-outline-secondary">
                    <i class="ti ti-arrow-left"></i>&nbsp;Quay lại
                </asp:HyperLink>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server"></asp:Content>
<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="Server"></asp:Content>
