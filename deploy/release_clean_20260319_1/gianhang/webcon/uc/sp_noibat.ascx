<%@ Control Language="C#" AutoEventWireup="true" CodeFile="sp_noibat.ascx.cs" Inherits="uc_home_sp_noibat" %>
<div class="container-fluid pt-20 pb-20" <%--style="background: rgb(0,61,0); background: radial-gradient(circle, rgba(0,61,0,1) 0%, rgba(0,214,0,1) 0%, rgba(0,138,0,1) 100%);"--%>>
    <div class="container">
        <div style="border-left: 4px solid #008a00;" class="fw-600">
            <div class="pl-3 fs-bc-34-28 pt-1 text-upper">SẢN PHẨM CỦA CHÚNG TÔI</div>
            <%--<div class="pl-3 title-sub-home-bc mt-2-minus pb-1"><%=des %></div>--%>
        </div>


        <div class="row">
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <div class="cell-lg-3 mt-3 p-2-lg">

                        <div>
                            <a href="/gianhang/webcon/chi-tiet-san-pham.aspx?tkchinhanh=<%=Session["ten_tk_chinhanh"].ToString() %>&idbv=<%#Eval("id") %>">
                                <img src="<%#Eval("image") %>" class="img-thumbnail" /></a>
                        </div>
                        <div class="fw-600 mt-2 ellipsis-1">
                            <a class="fg-black fg-emerald-hover" href="/gianhang/webcon/chi-tiet-san-pham.aspx?tkchinhanh=<%=Session["ten_tk_chinhanh"].ToString() %>&idbv=<%#Eval("id") %>"><%#Eval("name") %></a>
                        </div>
                        <div style="color: #808080" class="mt-1">
                            <%#Eval("description") %>
                        </div>
                        <div class="fg-emerald mt-1 text-bold">
                            <span class="mif mif-2x mif-dollar2 pr-1"></span><%#Eval("giaban_sanpham","{0:#,##0}") %> VNĐ
                        </div>
                        <div class="text-right mt-4">
                            <a class="button secondary rounded" href="/gianhang/webcon/giohang.aspx?tkchinhanh=<%=Session["ten_tk_chinhanh"].ToString() %>&id=<%#Eval("id").ToString() %>">Thêm vào giỏ</a>
                            <a class="button bg-emerald bg-darkEmerald-hover fg-white rounded" href="/gianhang/webcon/giohang.aspx?tkchinhanh=<%=Session["ten_tk_chinhanh"].ToString() %>&id=<%#Eval("id").ToString() %>&dh=ok">Đặt hàng</a>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>


    </div>

</div>
