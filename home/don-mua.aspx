<%@ Page Title="Đơn mua" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="don-mua.aspx.cs" Inherits="home_don_mua" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .tblr-overlay{
            position:fixed; inset:0; background:rgba(0,0,0,.65);
            z-index:99999; display:flex; align-items:center; justify-content:center;
        }
        .table thead th{ white-space:nowrap; }
        .table td{ vertical-align:middle; }
        .modal-body{ max-height: calc(100vh - 220px); overflow:auto; }
        .img-60{ width:60px; height:60px; object-fit:cover; border-radius:12px; }
        .w-240{ width:240px; }
        .money{ white-space:nowrap; }
        .addr{ min-width: 180px; }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">

    <!-- ===================== MODAL CHI TIẾT ===================== -->
    <asp:UpdatePanel ID="up_chitiet" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_chitiet" runat="server" Visible="false">

                <div class="modal modal-blur show" style="display:block;" tabindex="-1" role="dialog" aria-modal="true">
                    <div class="modal-dialog modal-xl modal-dialog-centered" role="document" style="max-width: 980px;">
                        <div class="modal-content">

                            <div class="modal-header">
                                <h5 class="modal-title">Chi tiết đơn hàng</h5>
                                <a href="#" class="btn-close" aria-label="Close" id="close_add" runat="server" onserverclick="but_close_form_chitiet_Click"></a>
                            </div>

                            <div class="modal-body">

                                <div class="card">
                                    <div class="table-responsive">
                                        <table class="table card-table table-vcenter">
                                            <thead>
                                                <tr>
                                                    <th style="width:1px;">ID</th>
                                                    <th style="width:90px;">Ảnh</th>
                                                    <th style="min-width:220px;">Tên sản phẩm</th>
                                                    <th class="text-end" style="min-width:120px;">Giá (VNĐ)</th>
                                                  
                                                    <th class="text-end" style="min-width:90px;">Ưu đãi (%)</th>

                                                    <th class="text-center" style="min-width:80px;">Số lượng</th>
                                                 
                                                    <th class="text-end" style="min-width:120px;">Trao đổi (A)</th>
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

                                                            <td class="text-center">
                                                                <a href="<%# Eval("image") %>" target="_blank" class="d-inline-block">
                                                                    <img src="<%# Eval("image") %>" class="img-60" />
                                                                </a>
                                                            </td>

                                                       <td>
    <a href="/<%# Eval("name_en") %>-<%# Eval("id") %>.html" class="text-decoration-none">
        <div class="fw-semibold"><%# Eval("name") %></div>
    </a>
</td>

                                                            <td class="text-end money">
                                                                <%#Eval("giaban","{0:#,##0}") %> ₫
                                                            </td>

                                                         
                                                            <td class="text-end">
    <%# (Eval("PhanTram_GiamGia_ThanhToan_BangEvoucher") == DBNull.Value || Eval("PhanTram_GiamGia_ThanhToan_BangEvoucher") == null)
        ? "0"
        : Eval("PhanTram_GiamGia_ThanhToan_BangEvoucher").ToString() %>%
</td>

                                                            <td class="text-center">
                                                                <span class="badge bg-muted-lt text-muted"><%#Eval("soluong") %></span>
                                                            </td>

                                                          

                                                            <td class="text-end money">
                                                                <%# (Convert.ToDecimal(Eval("thanhtien")) / 1000m).ToString("0.00") %> A
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
                                <asp:Button ID="but_huydonhang" OnClick="but_huydonhang_Click" runat="server"
                                    Text="Hủy đơn hàng" CssClass="btn btn-outline-danger d-none" />
                                <asp:Button ID="but_danhanhang" OnClick="but_danhanhang_Click" runat="server"
                                    Text="Xác nhận đã nhận hàng" CssClass="btn btn-primary d-none" />
                                <a href="#" class="btn btn-outline-secondary" id="close_add_b" runat="server" onserverclick="but_close_form_chitiet_Click">Đóng</a>
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
                <div class="container-xl" >
                    <div class="row g-2 align-items-center">
                        <div class="col">
                            <div class="page-pretitle">Đơn hàng</div>
                            <h2 class="page-title">Đơn mua</h2>
                        </div>

                        <div class="col-auto ms-auto d-print-none">
                            <div class="text-muted small">
                                <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                                <asp:Label ID="lb_show_md" runat="server" Text="" CssClass="d-none"></asp:Label>
                            </div>
                        </div>
                    </div>

                    <div class="row g-2 align-items-center mt-3">
                        <div class="col-md-6">
                            <asp:TextBox MaxLength="50" ID="txt_timkiem1" runat="server"
                                placeholder="Tìm theo Shop / Người mua / ID..."
                                CssClass="form-control"
                                AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                        </div>

                        <div class="col-md-6" style="display:none">
                            <asp:TextBox MaxLength="50" ID="txt_timkiem" runat="server"
                                placeholder="Tìm nhanh..."
                                CssClass="form-control"
                                AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                        </div>

	                        <div class="col-12">
	                            <div class="btn-list">
	                                <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server" CssClass="btn btn-outline-secondary btn-sm">
	                                    <i class="ti ti-chevron-left"></i>&nbsp;Lùi
	                                </asp:LinkButton>

                                <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                                    Tới&nbsp;<i class="ti ti-chevron-right"></i>
                                </asp:LinkButton>

                                <!-- giữ lại bản mobile cũ để không đụng code -->
	                                <div class="d-none">
	                                    <asp:LinkButton ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" />
	                                    <asp:LinkButton ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" />
	                                </div>
	                            </div>
	                        </div>

                            <div class="col-12">
                                <div class="d-flex align-items-center gap-2 flex-wrap">
                                    <div class="text-muted small">
                                        Lọc trạng thái: <asp:Label ID="lb_status_filter" runat="server" Text="Tất cả trạng thái"></asp:Label>
                                    </div>
                                    <div class="btn-list ms-auto">
                                        <asp:LinkButton ID="but_loc_all" runat="server" CssClass="btn btn-outline-secondary btn-sm" CommandArgument="all" OnClick="but_loc_trangthai_Click">Tất cả</asp:LinkButton>
                                        <asp:LinkButton ID="but_loc_dadat" runat="server" CssClass="btn btn-outline-azure btn-sm" CommandArgument="da-dat" OnClick="but_loc_trangthai_Click">Đã đặt</asp:LinkButton>
                                        <asp:LinkButton ID="but_loc_cho" runat="server" CssClass="btn btn-outline-danger btn-sm" CommandArgument="cho-trao-doi" OnClick="but_loc_trangthai_Click">Chờ Trao đổi</asp:LinkButton>
                                        <asp:LinkButton ID="but_loc_datd" runat="server" CssClass="btn btn-outline-blue btn-sm" CommandArgument="da-trao-doi" OnClick="but_loc_trangthai_Click">Đã Trao đổi</asp:LinkButton>
                                        <asp:LinkButton ID="but_loc_giao" runat="server" CssClass="btn btn-outline-yellow btn-sm" CommandArgument="da-giao" OnClick="but_loc_trangthai_Click">Đã giao</asp:LinkButton>
                                        <asp:LinkButton ID="but_loc_nhan" runat="server" CssClass="btn btn-outline-success btn-sm" CommandArgument="da-nhan" OnClick="but_loc_trangthai_Click">Đã nhận</asp:LinkButton>
                                        <asp:LinkButton ID="but_loc_huy" runat="server" CssClass="btn btn-outline-danger btn-sm" CommandArgument="da-huy" OnClick="but_loc_trangthai_Click">Đã hủy</asp:LinkButton>
                                    </div>
                                </div>
                            </div>
	                    </div>

                </div>
            </div>

            <div class="page-body">
                <div class="container-xl" >

                    <div class="card">
                        <div class="table-responsive">
                            <table class="table card-table table-vcenter">
                                <thead>
                                    <tr>
                                        <th style="width:1px;">ID</th>
                                        <th style="width:1px;" class="text-center">
                                            <input type="checkbox"
                                                onkeypress="if (event.keyCode==13) return false;"
                                                onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </th>
                                        <th style="min-width:160px;">Ngày</th>
                                        <th style="min-width:90px;">Loại</th>
                                        <th style="min-width:160px;">Tên Shop</th>
                                    
                                        <th class="text-end" style="min-width:120px;">Trao đổi (A)</th>
                                        <th style="min-width:140px;">Trạng thái</th>
                                        <th style="min-width:190px;">Thao tác</th>
                                        <th style="min-width:170px;">Người nhận</th>
                                        <th class="addr">Địa chỉ</th>
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
                                                    <asp:CheckBox ID="checkID" runat="server" onkeypress="if (event.keyCode==13) return false;" />
                                                </td>

                                                <td>
                                                    <div class="fw-semibold"><%#Eval("ngaydat","{0:dd/MM/yyyy}") %></div>
                                                    <div class="mt-2">
                                                        <asp:LinkButton ID="LinkButton1" OnClick="LinkButton1_Click" CommandArgument='<%# Eval("id") %>'
                                                            runat="server" CssClass="btn btn-outline-secondary btn-sm">
                                                            Chi tiết
                                                        </asp:LinkButton>
                                                    </div>
                                                </td>

                                                <td>
                                                    <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("online_offline").ToString()=="True" %>'>
                                                        <span class="badge bg-yellow-lt text-yellow">Online</span>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#Eval("online_offline").ToString()=="False" %>'>
                                                        <span class="badge bg-blue-lt text-blue">Offline</span>
                                                    </asp:PlaceHolder>
                                                </td>

                                                <td>
                                                    <div class="fw-semibold"><%# Eval("TenShop") %></div>
                                                </td>

                                              

                                                <td class="text-end money">
                                                    <%# (Convert.ToDecimal(Eval("tongtien")) / 1000m).ToString("0.00") %> A
                                                </td>

                                                <td>
                                                    <asp:PlaceHolder ID="PlaceHolder19" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã đặt" %>'>
                                                        <span class="badge bg-azure-lt text-azure">Đã đặt</span>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã hủy" %>'>
                                                        <span class="badge bg-red-lt text-red">Đã hủy</span>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã giao" %>'>
                                                        <span class="badge bg-yellow-lt text-yellow">Đã giao</span>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã nhận" %>'>
                                                        <span class="badge bg-green-lt text-green">Đã nhận</span>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#Eval("trangthai").ToString()=="Chưa Trao đổi" %>'>
                                                        <span class="badge bg-orange-lt text-orange">Chưa Trao đổi</span>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#Eval("trangthai").ToString()=="Chờ Trao đổi" %>'>
                                                        <span class="badge bg-red-lt text-red">Chờ Trao đổi</span>
                                                    </asp:PlaceHolder>

                                                    <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible='<%#Eval("trangthai").ToString()=="Đã Trao đổi" %>'>
                                                        <span class="badge bg-blue-lt text-blue">Đã Trao đổi</span>
                                                    </asp:PlaceHolder>
                                                </td>

                                                <td>
                                                    <div class="btn-list">
                                                        <asp:PlaceHolder ID="ph_huy_row" runat="server" Visible='<%# Convert.ToBoolean(Eval("show_huydon")) %>'>
                                                            <asp:LinkButton ID="but_huydonhang_row" runat="server"
                                                                OnClick="but_huydonhang_row_Click"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                CssClass="btn btn-outline-danger btn-sm">
                                                                Hủy đơn
                                                            </asp:LinkButton>
                                                        </asp:PlaceHolder>

                                                        <asp:PlaceHolder ID="ph_nhan_row" runat="server" Visible='<%# Convert.ToBoolean(Eval("show_danhan")) %>'>
                                                            <asp:LinkButton ID="but_danhanhang_row" runat="server"
                                                                OnClick="but_danhanhang_row_Click"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                CssClass="btn btn-primary btn-sm">
                                                                Đã nhận hàng
                                                            </asp:LinkButton>
                                                        </asp:PlaceHolder>
                                                    </div>
                                                </td>

                                                <td>
                                                    <div class="fw-semibold"><%# Eval("hoten_nguoinhan") %></div>
                                                    <div class="text-muted"><%# Eval("sdt_nguoinhan") %></div>
                                                </td>

                                                <td class="text-muted">
                                                    <%# Eval("diahchi_nguoinhan") %>
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
                    <div class="mt-3 text-white">Đang tải...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="ContentFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="Server">
</asp:Content>
