<%@ Page Title="Giỏ hàng của bạn" Language="C#" MasterPageFile="~/gianhang/webcon/mp-webcon.master" AutoEventWireup="true" CodeFile="giohang.aspx.cs" Inherits="chitiettintuc" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <meta property="og:type" content="website" />
    <title><%=title_web %></title>
    <asp:PlaceHolder runat="server"><%=meta %></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="container-fluid pt-10 pb-20 bg-aka-1" <%--style="background: rgb(0,61,0); background: radial-gradient(circle, rgba(0,61,0,1) 0%, rgba(0,214,0,1) 0%, rgba(0,138,0,1) 100%);"--%>>
        <div class="container">
            <div style="border-left: 4px solid #008a00;" class="fw-600">
                <div class="pl-3 fs-bc-34-28 pt-1 text-upper">Giỏ hàng của bạn</div>
                <div class="pl-3 title-sub-home-bc mt-2-minus pb-1">Xem lại thông tin Dịch vụ & Sản phẩm trước khi đặt</div>
            </div>
            <div>
                <div class="mt-5 mb-5 text-right">
                    <asp:Button OnClientClick="Metro.activity.open({type:'cycle',overlayClickClose:false})" ID="but_save" runat="server" Text="Cập nhật giỏ hàng" Visible="false" CssClass="success mr-2" OnClick="but_capnhat_Click" />
                    <asp:Button OnClientClick="Metro.activity.open({type:'cycle',overlayClickClose:false})" ID="but_huygiohang" runat="server" Text="Hủy giỏ hàng" Visible="false" CssClass="alert" OnClick="but_huygiohang_Click" />
                </div>
                <div class="mt-2" style="overflow: auto">
                    <table class="table row-hover table-border cell-border compact normal-lg bg-white <%--striped--%> <%--compact normal-lg--%>">
                        <thead>
                            <tr style="background-color: #60a917">
                                <th style="width: 1px"></th>
                                <td class="text-bold text-center fg-white" style="width: 50px; min-width: 50px">Ảnh</td>
                                <td class="text-bold fg-white" style="min-width: 150px">Mặt hàng</td>
                                <td class="text-bold fg-white" style="width: 96px; min-width: 96px">Phân loại</td>
                                <td class="text-bold text-center fg-white" style="width: 1px;">Giá</td>
                                <td class="text-bold fg-white" style="width: 140px; min-width: 140px;">Số lượng</td>
                                <td class="text-bold text-right fg-white" style="width: 102px; min-width: 102px">Thành tiền</td>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="Repeater1" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <a href='/gianhang/webcon/xoa_chitiet_giohang.aspx?tkchinhanh=<%=Session["ten_tk_chinhanh"].ToString() %>&id=<%#Eval("ID").ToString() %>' data-role="hint" data-hint-text="Xóa" data-hint-position="top">
                                                <span class="mif mif-2x mif-cancel fg-red"></span>
                                            </a>
                                        </td>
                                        <td>
                                            <img src="<%#Eval("img") %>" class="img-cover-vuong w-h-50" style="max-width: none!important">
                                        </td>
                                        <td><%#Eval("Name").ToString() %></td>
                                        <td>
                                            <asp:PlaceHolder ID="PlaceHolder14" runat="server" Visible='<%#Eval("kyhieu").ToString()=="dichvu" %>'>
                                                <span class="data-wrapper"><code class="bg-cyan fg-white">Dịch vụ</code></span>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("kyhieu").ToString()=="sanpham" %>'>
                                                <span class="data-wrapper"><code class="bg-emerald fg-white">Sản phẩm</code></span>
                                            </asp:PlaceHolder>
                                        </td>
                                        <td class="text-right">
                                            <%#tien(Eval("Price").ToString()) %>
                                        </td>
                                        <td class="text-center">
                                            <input data-role="spinner" data-min-value="1" data-max-value="9999" maxlength="4" data-clear-button="false" name="sl_<%#Eval("ID").ToString() %>" type="text" value="<%#Eval("soluong").ToString() %>" onkeypress="if (event.keyCode==13) return false;">
                                        </td>
                                        <td class="text-right"><%#tien(Eval("thanhtien").ToString()) %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                        <%if (sl_hangtronggio > 0)
                            { %>
                        <tfoot>
                            <tr class=" text-bold">
                                <td colspan="6" class="text-right">Tổng cộng</td>
                                <td class="text-right"><%=tongtien.ToString("#,##0") %></td>

                            </tr>
                            <tr class=" text-bold">
                                <td colspan="6" class="text-right">Chiết khấu</td>
                                <td class="text-right"><%=chietkhau %>%</td>

                            </tr>
                            <tr class=" text-bold">
                                <td colspan="6" class="text-right text-size-20">Sau chiết khấu</td>
                                <td class="text-right text-size-20"><%=sauchietkhau.ToString("#,##0") %></td>

                            </tr>
                            <tr class="text-bold">
                                <td colspan="7" class="text-right fg-red">Số tiền bằng chữ: <%=number_class.number_to_text_unlimit(sauchietkhau.ToString()) %> đồng.
                                </td>
                            </tr>
                        </tfoot>
                        <%}
                            else
                            { %>
                        <h5>Giỏ hàng của bạn không có gì.</h5>
                        <%} %>
                    </table>
                </div>
            </div>



        </div>
    </div>

    <div class="container-fluid pt-20 pb-20" <%--style="background: rgb(0,61,0); background: radial-gradient(circle, rgba(0,61,0,1) 0%, rgba(0,214,0,1) 0%, rgba(0,138,0,1) 100%);"--%>>
        <div class="container">
            <div style="border-left: 4px solid #008a00;" class="fw-600">
                <div class="pl-3 fs-bc-34-28 pt-1 text-upper">ĐẶT HÀNG</div>
                <div class="pl-3 title-sub-home-bc mt-2-minus pb-1">Vui lòng điền đầy đủ thông tin bên dưới</div>
            </div>
            <asp:Panel ID="Panel1" runat="server" DefaultButton="but_dathang">
                <div class="example bg-light p-4 mt-6">
                    <div class="row">
                        
                        <div class="cell-lg-6 mt-3 pr-2-lg">
                            <span class="place-left fw-600">Tên khách hàng</span>
                            <span class="ani-float place-left pl-2">
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="!" ForeColor="#CE352C" ControlToValidate="txt_hoten" ValidationGroup="val_dathang"></asp:RequiredFieldValidator>
                            </span>
                            <div class="input">
                                <asp:TextBox ID="txt_hoten" runat="server" ValidationGroup="val_dathang" MaxLength="50"></asp:TextBox>
                            </div>
                        </div>
                        <div class="cell-lg-6 mt-3 pl-2-lg">
                            <span class="place-left fw-600">Điện thoại</span>
                            <span class="ani-float place-left pl-2">
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="!" ForeColor="#CE352C" ControlToValidate="txt_sdt" ValidationGroup="val_dathang"></asp:RequiredFieldValidator>
                            </span>
                            <div class="input">
                                <asp:TextBox ID="txt_sdt" runat="server" ValidationGroup="val_dathang"></asp:TextBox>
                            </div>
                        </div>

                        <div class="cell-lg-12 mt-3">
                            <div>
                                <span class="place-left fw-600">Địa chỉ giao hàng</span>
                                <span class="ani-float place-left pl-2">
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="!" ForeColor="#CE352C" ControlToValidate="txt_diachi" ValidationGroup="val_dathang"></asp:RequiredFieldValidator>
                                </span>
                                <div class="clr-bc"></div>
                            </div>
                            <div>
                                <asp:TextBox ID="txt_diachi" data-role="textarea" runat="server" TextMode="MultiLine"></asp:TextBox>
                            </div>
                        </div>
                        <div class="cell-lg-12 mt-3">
                            <label class=" fw-600">Ghi chú</label>
                            <asp:TextBox ID="txt_ghichu" data-role="textarea" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </div>
                        <%--<div class="cell-lg-12 mb-2">
                            <asp:CheckBox ID="CheckBox1" runat="server" Text="Tôi đồng ý với các điều khoản" />
                        </div>--%>
                        <div class="cell-lg-12 text-center mt-8 mb-6">
                            <asp:Button ID="but_dathang" runat="server" CssClass="button success large primary" Text="XÁC NHẬN ĐẶT HÀNG" ValidationGroup="val_dathang" OnClick="but_dathang_Click" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>
