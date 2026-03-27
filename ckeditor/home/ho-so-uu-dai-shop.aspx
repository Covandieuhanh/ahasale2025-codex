<%@ Page Title="Hồ sơ ưu đãi Gian hàng đối tác" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master"
    AutoEventWireup="true" CodeFile="ho-so-uu-dai-shop.aspx.cs" Inherits="home_lich_su_quyen_uu_dai" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        /* Overlay loading (UpdateProgress) */
        .tblr-overlay {
            position: fixed;
            inset: 0;
            background: rgba(0,0,0,.65);
            z-index: 99999;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        /* Make modal scrollable if content long */
        .modal-body {
            max-height: calc(100vh - 220px);
            overflow: auto;
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="main" runat="Server">

    <!-- ===================== MODAL: CHI TIẾT ĐƠN ===================== -->
    <asp:UpdatePanel ID="up_chitiet" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_chitiet" runat="server" Visible="false">

                <div class="modal modal-blur show" style="display:block;" tabindex="-1" role="dialog" aria-modal="true">
                    <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Chi tiết đơn hàng</h5>
                                <a href="#" class="btn-close" aria-label="Close" id="close_add" runat="server" onserverclick="but_close_form_chitiet_Click"></a>
                            </div>

                            <div class="modal-body">
                                <div class="row g-3">
                                    <div class="col-md-6">
                                        <div class="card">
                                            <div class="card-body">
                                                <div class="fw-bold mb-2">Thông tin Gian hàng đối tác</div>
                                                <div class="text-muted">Tên shop</div>
                                                <div class="fw-semibold"><asp:Label ID="Label100" runat="server" Text=""></asp:Label></div>
                                                <div class="text-muted mt-2">Điện thoại</div>
                                                <div class="fw-semibold"><asp:Label ID="Label101" runat="server" Text=""></asp:Label></div>
                                                <div class="text-muted mt-2">Địa chỉ</div>
                                                <div class="fw-semibold"><asp:Label ID="Label102" runat="server" Text=""></asp:Label></div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="card">
                                            <div class="card-body">
                                                <div class="fw-bold mb-2">Thông tin khách hàng</div>
                                                <div class="text-muted">Tên khách</div>
                                                <div class="fw-semibold"><asp:Label ID="Label103" runat="server" Text=""></asp:Label></div>
                                                <div class="text-muted mt-2">Điện thoại</div>
                                                <div class="fw-semibold"><asp:Label ID="Label104" runat="server" Text=""></asp:Label></div>
                                                <div class="text-muted mt-2">Địa chỉ</div>
                                                <div class="fw-semibold"><asp:Label ID="Label105" runat="server" Text=""></asp:Label></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="card mt-3">
                                    <div class="table-responsive">
                                        <table class="table table-vcenter card-table">
                                            <thead>
                                                <tr>
                                                    <th style="width:1px;">ID</th>
                                                    <th style="width:1px;">Ảnh</th>
                                                    <th>Tên sản phẩm</th>
                                                    <th class="text-end">Giá (Quyền tiêu dùng)</th>
                                                    <th class="text-center">Số lượng</th>
                                                    <th class="text-end">Trao đổi</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:Repeater ID="Repeater2" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td class="text-center"><%# Eval("id") %></td>
                                                            <td class="text-center">
                                                                <img src="<%# Eval("image") %>" width="60" height="60" style="object-fit:cover;border-radius:8px;" />
                                                            </td>
                                                          <td>
    <a href="/<%# Eval("name_en") %>-<%# Eval("id") %>.html" class="text-decoration-none">
        <%# Eval("name") %>
    </a>
</td>

                                                            <td class="text-end">
                                                                <%# Eval("giaban","{0:#,##0.##}") %>
                                                                <img src="/uploads/images/dong-a.png" style="width:18px" class="ms-1" />
                                                            </td>
                                                            <td class="text-center"><%# Eval("soluong") %></td>
                                                            <td class="text-end">
                                                                <%# Eval("thanhtien","{0:#,##0.##}") %>
                                                                <img src="/uploads/images/dong-a.png" style="width:18px" class="ms-1" />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>

                            </div>

                            <div class="modal-footer">
                                <a href="#" class="btn btn-outline-secondary" id="A5" runat="server" onserverclick="but_close_form_chitiet_Click">Đóng</a>
                            </div>
                        </div>
                    </div>
                </div>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="up_chitiet">
        <ProgressTemplate>
            <div class="tblr-overlay">
                <div class="text-center">
                    <div class="spinner-border" role="status"></div>
                    <div class="mt-3 text-white">Đang xử lý...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>


    <!-- ===================== MAIN ===================== -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="page-header d-print-none">
                <div class="container-xl">
                    <div class="row g-2 align-items-center">
                        <div class="col">
                            <h2 class="page-title">Hồ sơ ưu đãi Gian hàng đối tác</h2>
                        </div>

                        <%-- ✅ ĐÃ BỎ NÚT NẠP / RÚT --%>
                        <div class="col-auto ms-auto d-print-none"></div>
                    </div>

                    <div class="row g-2 align-items-center mt-2">
                        <div class="col-auto text-muted">
                            <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="col-auto ms-auto">
                            <div class="btn-list">
                                <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                                    <i class="ti ti-chevron-left"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                                    <i class="ti ti-chevron-right"></i>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>

                </div>
            </div>

            <div class="page-body">
                <div class="container-xl">

                    <!-- Search -->
                    <div class="row g-2 mb-3">
                        <div class="col-md-6">
                            <asp:TextBox ID="txt_timkiem1" runat="server" MaxLength="50" CssClass="form-control"
                                placeholder="Nhập từ khóa" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                        </div>
                        <div class="col-md-6" style="display:none">
                            <asp:TextBox ID="txt_timkiem" runat="server" MaxLength="50" CssClass="form-control"
                                placeholder="Nhập từ khóa" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                        </div>

                        <div class="d-none">
                            <asp:Label ID="lb_show_md" runat="server" Text=""></asp:Label>
                            <asp:LinkButton ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" />
                            <asp:LinkButton ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" />
                        </div>
                    </div>

                    <div class="card">
                        <div class="table-responsive">
                            <table class="table table-vcenter card-table">
                                <thead>
                                    <tr>
                                        <th style="width:1px;">ID</th>
                                        <th style="width:140px;">Ngày</th>
                                        <th class="text-end" style="min-width:140px;">Quyền ưu đãi</th>
                                        <th>Nội dung</th>
                                        <th style="width:120px;">Đơn</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="text-center"><%# Eval("id") %></td>
                                                <td><%# Eval("ngay","{0:dd/MM/yyyy}") %></td>

                                                <td class="text-end">
                                                    <asp:PlaceHolder ID="PlaceHolder19" runat="server" Visible='<%# Eval("CongTru").ToString()=="True" %>'>
                                                        <span class="badge bg-green-lt">+ <%# Eval("dongA","{0:#,##0.##}") %></span>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# Eval("CongTru").ToString()=="False" %>'>
                                                        <span class="badge bg-yellow-lt text-yellow">- <%# Eval("dongA","{0:#,##0.##}") %></span>
                                                    </asp:PlaceHolder>
                                                    <img src="/uploads/images/dong-a.png" style="width:18px" class="ms-1" />
                                                </td>

                                                <td><%# Eval("ghichu") %></td>

                                                <td>
                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("id_donhang").ToString()!="" %>'>
                                                        <asp:LinkButton ID="LinkButton1" OnClick="LinkButton1_Click" CommandArgument='<%# Eval("id_donhang") %>'
                                                            runat="server" CssClass="btn btn-sm btn-outline-primary">
                                                            Chi tiết
                                                        </asp:LinkButton>
                                                    </asp:PlaceHolder>
                                                </td>
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
            <div class="tblr-overlay">
                <div class="text-center">
                    <div class="spinner-border" role="status"></div>
                    <div class="mt-3 text-white">Đang tải dữ liệu...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="foot_sau" runat="Server">
</asp:Content>
