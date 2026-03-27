<%@ Page Title="Thêm vào giỏ hàng" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="them-vao-gio.aspx.cs" Inherits="home_them_vao_gio" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .cart-add-page .product-media {
            width: 100%;
            max-height: 280px;
            object-fit: cover;
            border-radius: 12px;
            border: 1px solid rgba(98, 105, 118, .2);
            background: #fff;
        }
        .cart-add-page .metric {
            border: 1px solid rgba(98, 105, 118, .2);
            border-radius: 12px;
            padding: 12px;
            background: #fff;
        }
        .cart-add-page .metric .label {
            font-size: 12px;
            color: #6c7a91;
        }
        .cart-add-page .metric .value {
            font-size: 18px;
            font-weight: 600;
            margin-top: 6px;
            white-space: nowrap;
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="cart-add-page">
        <div class="page-header d-print-none">
            <div class="container-xl">
                <div class="row g-2 align-items-center">
                    <div class="col">
                        <div class="page-pretitle">Giỏ hàng</div>
                        <h2 class="page-title">Thêm sản phẩm vào giỏ</h2>
                    </div>
                    <div class="col-auto ms-auto">
                        <asp:HyperLink ID="hl_back_top" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                            <i class="ti ti-arrow-left"></i>&nbsp;Quay lại
                        </asp:HyperLink>
                    </div>
                </div>
            </div>
        </div>

        <div class="page-body">
            <div class="container-xl">
                <div class="card">
                    <div class="card-body">
                        <div class="row g-4">
                            <div class="col-lg-5">
                                <img id="img_product" runat="server" class="product-media" alt="Sản phẩm" src="/uploads/images/macdinh.jpg" />
                                <div class="mt-3">
                                    <div class="text-muted small">Sản phẩm</div>
                                    <div class="h3 mb-0"><asp:Label ID="lb_ten_sp" runat="server" Text="-"></asp:Label></div>
                                </div>
                            </div>
                            <div class="col-lg-7">
                                <div class="row g-2 mb-3">
                                    <div class="col-md-6">
                                        <div class="metric">
                                            <div class="label">Giá (VNĐ)</div>
                                            <div class="value text-danger"><asp:Label ID="lb_gia_vnd" runat="server" Text="0"></asp:Label> đ</div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="metric">
                                            <div class="label">Giá (A)</div>
                                            <div class="value text-primary"><asp:Label ID="lb_gia_a" runat="server" Text="0"></asp:Label> A</div>
                                        </div>
                                    </div>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Số lượng</label>
                                    <asp:TextBox ID="txt_soluong" runat="server" CssClass="form-control" MaxLength="3"
                                        AutoPostBack="true" OnTextChanged="txt_soluong_TextChanged"
                                        onfocus="AutoSelect(this)" oninput="format_sotien_new(this)"></asp:TextBox>
                                </div>

                                <div class="row g-2">
                                    <div class="col-md-6">
                                        <div class="metric">
                                            <div class="label">Tổng tiền (VNĐ)</div>
                                            <div class="value text-danger"><asp:Label ID="lb_tong_vnd" runat="server" Text="0"></asp:Label> đ</div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="metric">
                                            <div class="label">Tổng tương ứng (A)</div>
                                            <div class="value text-success"><asp:Label ID="lb_tong_a" runat="server" Text="0"></asp:Label> A</div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-footer d-flex align-items-center justify-content-between">
                        <asp:HyperLink ID="hl_back_bottom" runat="server" CssClass="btn btn-outline-secondary">Hủy</asp:HyperLink>
                        <asp:Button ID="but_xacnhan" runat="server" CssClass="btn btn-primary" Text="Thêm vào giỏ hàng" OnClick="but_xacnhan_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server"></asp:Content>
<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="Server"></asp:Content>
