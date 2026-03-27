<%@ Page Title="Giỏ hàng của bạn" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true" CodeFile="gio-hang.aspx.cs" Inherits="home_gio_hang" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server">
    <style>
        .cart-wrap { max-width: 1100px; }

        /* Overlay đặt hàng (giữ cơ chế pn_dathang.Visible) */
        .order-overlay {
            position: fixed;
            inset: 0;
            z-index: 2000;
            background: rgba(0,0,0,.55);
            overflow: auto;
            padding: 18px 12px;
        }
        .order-dialog { max-width: 900px; margin: 0 auto; }

        /* Table */
        .table thead th { white-space: nowrap; }
        .table td, .table th { vertical-align: middle; }
        .td-img img { border-radius: 10px; border: 1px solid rgba(98,105,118,.18); object-fit: cover; }

        /* Paging / misc (nếu sau này cần) */
        .muted { color: #626976; }

        /* fix for small qty input */
        .qty-input { width: 70px; }

        /* progress overlay */
        .overlay-loading {
            position: fixed; inset: 0;
            background: rgba(0,0,0,.6);
            z-index: 99999;
            display: flex; align-items: center; justify-content: center;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <!-- ====== FORM TRAO ĐỔI (GIỮ LOGIC: toggle pn_dathang.Visible) ====== -->
    <asp:UpdatePanel ID="up_dathang" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_dathang" runat="server" Visible="false" DefaultButton="but_dathang">
                <div class="order-overlay">
                    <div class="order-dialog">
                        <div class="card shadow-sm">
                            <div class="card-header d-flex align-items-center justify-content-between">
                                <div class="card-title fw-bold">Xác nhận trao đổi</div>
                                <a href="#" id="A1" runat="server" onserverclick="but_close_form_dathang_Click" title="Đóng"
                                    class="btn btn-ghost-danger btn-icon">
                                    <i class="ti ti-x"></i>
                                </a>
                            </div>

                            <div class="card-body">
                                <div class="table-responsive">
                                    <table class="table table-vcenter card-table">
                                        <thead>
                                            <tr>
                                                <th style="width:1px;">ID</th>
                                                <th style="width:1px;">Ảnh</th>
                                                <th class="text-start" style="min-width: 180px;">Tên sản phẩm</th>
                                                <th class="text-end" style="min-width: 110px;">Giá (VNĐ)</th>
                                                <th class="text-center" style="width:1px;">Số lượng</th>
                                                <th class="text-end" style="min-width: 140px;">Thành tiền (VNĐ)</th>
                                                <th class="text-start" style="min-width: 160px;">Shop</th>
                                            </tr>
                                        </thead>

                                        <tbody>
                                            <asp:Repeater ID="Repeater2" runat="server">
                                                <ItemTemplate>
                                                    <span style="display:none">
                                                        <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                                    </span>
                                                    <tr>
                                                        <td class="text-center"><%# Eval("id") %></td>

                                                        <td class="text-center td-img">
                                                            <img src="<%# Eval("image") %>" width="60" height="60" alt="" />
                                                        </td>

                                                       <td class="text-start">
    <a href="/<%# Eval("name_en") %>-<%# Eval("id") %>.html" class="text-decoration-none">
        <%# Eval("name") %>
    </a>
</td>


                                                        <td class="text-end">
    <span class="me-1"><%# Eval("giaban","{0:#,##0}") %> đ</span>

    <asp:PlaceHolder runat="server"
        Visible='<%# Convert.ToInt32(Eval("PhanTramUuDai")) > 0 %>'>
        <span class="badge bg-azure-lt">-<%# Eval("PhanTramUuDai") %>%</span>
    </asp:PlaceHolder>
</td>


                                                        <td class="text-center"><%# Eval("soluong") %></td>

                                                        <td class="text-end">
                                                            <%# Eval("ThanhTien","{0:#,##0}") %> đ
                                                        </td>

                                                        <td class="text-start"><%# Eval("TenShop") %></td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </div>

                                <hr class="my-4" />

                                <div class="row g-3">
                                    <div class="col-md-4">
                                        <label class="form-label">Người nhận</label>
                                        <asp:TextBox ID="txt_hoten_nguoinhan" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-md-4">
                                        <label class="form-label">Điện thoại</label>
                                        <asp:TextBox ID="txt_sdt_nguoinhan" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-md-4">
                                        <label class="form-label">Địa chỉ</label>
                                        <asp:TextBox ID="txt_diachi_nguoinhan" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="card-footer d-flex align-items-center justify-content-between">
                                <a href="#" id="A2" runat="server" onserverclick="but_close_form_dathang_Click"
                                   class="btn btn-link text-decoration-none">Hủy</a>
                                <asp:Button ID="but_dathang" OnClick="but_dathang_Click" runat="server"
                                    Text="Xác nhận trao đổi" CssClass="btn btn-success px-4" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_dathang">
        <ProgressTemplate>
            <div class="overlay-loading">
                <div class="spinner-border" role="status" aria-label="loading"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <!-- ====== MAIN CART ====== -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="container-xl cart-wrap py-4">

                <div class="d-flex align-items-center justify-content-between mb-3">
                    <div class="h2 mb-0">Giỏ hàng của bạn</div>

                    <div class="d-flex gap-2 flex-wrap">
                        <asp:Button ID="Button1" runat="server" Text="Xóa" CssClass="btn btn-outline-danger" OnClick="Button1_Click" />
                        <asp:Button ID="Button2" runat="server" Text="Trao đổi ngay" CssClass="btn btn-warning" OnClick="Button2_Click" />
                        <asp:Button ID="Button3" runat="server" Text="Lưu chỉnh sửa" CssClass="btn btn-outline-info" OnClick="Button3_Click" />
                    </div>
                </div>

                <div class="card shadow-sm">
                    <div class="card-body p-0">
                        <div class="table-responsive">
                            <table class="table table-vcenter card-table">
                                <thead>
                                    <tr>
                                        <th style="width:1px;">ID</th>
                                        <th style="width:1px;" class="text-center">
                                            <input type="checkbox"
                                                aria-label="Chọn/Bỏ chọn"
                                                onkeypress="if (event.keyCode==13) return false;"
                                                onclick="document.querySelectorAll('.checkbox-table input[type=checkbox]').forEach(cb=>cb.checked=this.checked);" />
                                        </th>
                                        <th style="width:1px;">Ảnh</th>
                                        <th class="text-start" style="min-width: 180px;">Tên sản phẩm</th>
                                        <th class="text-end" style="min-width: 110px;">Giá (VNĐ)</th>
                                        <th style="width:1px;" class="text-center">Số lượng</th>
                                        <th class="text-end" style="min-width: 140px;">Thành tiền (VNĐ)</th>
                                        <th class="text-start" style="min-width: 160px;">Shop</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <span style="display:none">
                                                <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                            </span>
                                            <tr>
                                                <td class="text-center"><%# Eval("id") %></td>

                                                <td class="checkbox-table text-center">
                                                    <asp:CheckBox ID="checkID" runat="server"
                                                        onkeypress="if (event.keyCode==13) return false;" />
                                                </td>

                                                <td class="text-center td-img">
                                                    <img src="<%# Eval("image") %>" width="60" height="60" alt="" />
                                                </td>

                                               <td class="text-start">
    <a href="/<%# Eval("name_en") %>-<%# Eval("id") %>.html" class="text-decoration-none">
        <%# Eval("name") %>
    </a>
</td>


                                               <td class="text-end">
    <span class="me-1"><%# Eval("giaban","{0:#,##0}") %> đ</span>

    <asp:PlaceHolder runat="server"
        Visible='<%# Convert.ToInt32(Eval("PhanTramUuDai")) > 0 %>'>
        <span class="badge bg-azure-lt">-<%# Eval("PhanTramUuDai") %>%</span>
    </asp:PlaceHolder>
</td>


                                                <td class="text-center">
                                                    <asp:TextBox onfocus="AutoSelect(this)"
                                                        CssClass="form-control form-control-sm text-center qty-input"
                                                        oninput="format_sotien_new(this)"
                                                        ID="txt_sl_1" MaxLength="3" runat="server"
                                                        Text='<%#Eval("soluong") %>'
                                                        onkeypress="if (event.keyCode==13) return false;"></asp:TextBox>
                                                </td>

                                                <td class="text-end">
                                                    <%# Eval("ThanhTien","{0:#,##0}") %> đ
                                                </td>

                                                <td class="text-start"><%# Eval("TenShop") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>

                            </table>
                        </div>
                    </div>
                </div>

            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="overlay-loading">
                <div class="spinner-border" role="status" aria-label="loading"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot_sau" runat="Server">
</asp:Content>
