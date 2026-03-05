<%@ Page Title="Trao đổi sản phẩm" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="trao-doi.aspx.cs" Inherits="home_trao_doi" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server"></asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .exchange-page .card-title-sub {
            font-size: 12px;
            color: #6c7a91;
        }
        .exchange-page .product-media {
            width: 100%;
            max-height: 280px;
            object-fit: cover;
            border-radius: 12px;
            border: 1px solid rgba(98, 105, 118, .2);
            background: #fff;
        }
        .exchange-page .metric {
            border: 1px solid rgba(98, 105, 118, .2);
            border-radius: 12px;
            padding: 12px;
            background: #fff;
        }
        .exchange-page .metric .label {
            font-size: 12px;
            color: #6c7a91;
        }
        .exchange-page .metric .value {
            font-size: 18px;
            font-weight: 600;
            margin-top: 6px;
        }
        .exchange-page .money {
            white-space: nowrap;
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="exchange-page">
        <div class="page-header d-print-none">
            <div class="container-xl">
                <div class="row g-2 align-items-center">
                    <div class="col">
                        <div class="page-pretitle">Đơn hàng</div>
                        <h2 class="page-title">Xác nhận trao đổi</h2>
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
                                    <div class="card-title-sub">Sản phẩm</div>
                                    <div class="h3 mb-2">
                                        <asp:Label ID="lb_ten_sp" runat="server" Text="-"></asp:Label>
                                    </div>
                                </div>

                                <div class="row g-2 mt-1">
                                    <div class="col-6">
                                        <div class="metric">
                                            <div class="label">Giá (VNĐ)</div>
                                            <div class="value money text-danger"><asp:Label ID="lb_gia_vnd" runat="server" Text="0"></asp:Label> đ</div>
                                        </div>
                                    </div>
                                    <div class="col-6">
                                        <div class="metric">
                                            <div class="label">Giá (A)</div>
                                            <div class="value money text-primary"><asp:Label ID="lb_gia_a" runat="server" Text="0"></asp:Label> A</div>
                                        </div>
                                    </div>
                                    <div class="col-12">
                                        <div class="metric">
                                            <div class="label">Ưu đãi eVoucher</div>
                                            <div class="value text-warning"><asp:Label ID="lb_uu_dai" runat="server" Text="0"></asp:Label>%</div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-7">
                                <div class="mb-3">
                                    <label class="form-label">Số lượng</label>
                                    <asp:TextBox ID="txt_soluong" runat="server" CssClass="form-control" MaxLength="3"
                                        AutoPostBack="true" OnTextChanged="txt_soluong_TextChanged"
                                        onfocus="AutoSelect(this)" oninput="format_sotien_new(this)"></asp:TextBox>
                                </div>

                                <div class="row g-2 mb-3">
                                    <div class="col-md-6">
                                        <div class="metric">
                                            <div class="label">Tổng (VNĐ)</div>
                                            <div class="value money text-danger"><asp:Label ID="lb_tong_vnd" runat="server" Text="0"></asp:Label> đ</div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="metric">
                                            <div class="label">Tổng phải trao đổi (A)</div>
                                            <div class="value money text-success"><asp:Label ID="lb_tong_a" runat="server" Text="0"></asp:Label> A</div>
                                        </div>
                                    </div>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label text-danger">Người nhận</label>
                                    <asp:TextBox ID="txt_hoten_nguoinhan" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label text-danger">Điện thoại</label>
                                    <asp:TextBox ID="txt_sdt_nguoinhan" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>

                                <div class="mb-0">
                                    <label class="form-label text-danger">Địa chỉ</label>
                                    <asp:TextBox ID="txt_diachi_nguoinhan" runat="server" TextMode="MultiLine" Rows="4" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card-footer d-flex align-items-center justify-content-between">
                        <asp:HyperLink ID="hl_back_bottom" runat="server" CssClass="btn btn-outline-secondary">Hủy</asp:HyperLink>
                        <asp:Button ID="but_xacnhan" runat="server" CssClass="btn btn-primary" Text="Xác nhận trao đổi" OnClick="but_xacnhan_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server"></asp:Content>
<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="Server"></asp:Content>
