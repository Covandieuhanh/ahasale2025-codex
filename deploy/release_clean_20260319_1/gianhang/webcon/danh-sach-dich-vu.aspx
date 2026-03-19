<%@ Page Title="" Language="C#" MasterPageFile="~/gianhang/webcon/mp-webcon.master" AutoEventWireup="true" CodeFile="danh-sach-dich-vu.aspx.cs" Inherits="danh_sach_bai_viet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <%=meta %>
    <meta property="og:type" content="object" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="container-fluid pt-6 pt-10-lg ">
                <div class="container">
                    <div class="pl-3-lg pl-0 pr-3-lg pr-0">
                        <div style="border-left: 4px solid #008a00;" class="fw-600">
                            <div class="pl-3 fs-bc-34-28 pt-1 text-upper"><%=tenmn %></div>
                            <%if (mota != "")
                                { %>
                            <div class="pl-3 title-sub-home-bc mt-2-minus"><%=mota %></div>
                            <%} %>
                        </div>
                        <div class="pt-6">
                            <asp:TextBox ID="txt_search" runat="server" data-role="input" data-prepend="<span class='mif mif-search'></span>" placeholder="Tìm kiếm dịch vụ" OnTextChanged="txt_search_TextChanged" AutoPostBack="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <asp:Repeater ID="Repeater1" runat="server">
                            <ItemTemplate>
                                <div class="cell-lg-4 pl-3-lg pl-0 pr-3-lg pr-0 mt-10">
                                    <div class="h-100">
                                        <div>
                                            <a class="fg-black fg-green-hover" href="/gianhang/webcon/chi-tiet-dich-vu.aspx?tkchinhanh=<%=tkchinhanh %>&idbv=<%#Eval("id") %>">
                                                <img src="<%#Eval("image") %>" /></a>
                                        </div>
                                        <div class=" border bd-default border-top-none p-3">
                                            <div class=" fw-600 ellipsis-2">
                                                <a class="fg-black fg-green-hover" href="/gianhang/webcon/chi-tiet-dich-vu.aspx?tkchinhanh=<%=tkchinhanh %>&idbv=<%#Eval("id") %>"><%#Eval("name") %></a>
                                            </div>
                                            <div style="color: #808080" class="mt-1">
                                                <%#Eval("description") %>
                                            </div>
                                            <div class="fg-emerald mt-1 text-bold">
                                                <span class="mif mif-3x mif-dollar2 pr-1"></span><%#Eval("giaban","{0:#,##0}") %> VNĐ
                                            </div>
                                            <hr class="mt-3 mb-3" />
                                            <div class="text-right">
                                                <a class="button bg-emerald bg-darkEmerald-hover fg-white rounded" href="/gianhang/webcon/datlich.aspx?tkchinhanh=<%=tkchinhanh %>&id=<%#Eval("id").ToString() %>">Đặt lịch ngay</a>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <%--<div class="cell-lg-4 pl-3-lg pl-0 pr-3-lg pr-0 mt-9 ">
                            <div class="h-100 ">
                                <div class="fg-red fs-bc-34-28">Có thể dự án tiếp theo sẽ là của bạn!</div>
                                <div class="new-hero mt-4">
                                    <a onclick="show_hide_id_form_dangkytuvan()" class="button download-source-button large mr-1-sm mr-0 c-pointer"><span class="ml-4 mr-4">Tư Vấn Cho Tôi</span></a>
                                </div>
                            </div>
                        </div>--%>
                    </div>

                    <div class="text-center mt-8 pb-20">
                        <asp:Button ID="but_quaylai" runat="server" Text="Quay lại" CssClass="alert rounded fg-white" OnClick="but_quaylai_Click" />
                        <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                        <asp:Button ID="but_xemtiep" runat="server" Text="Xem thêm" CssClass="alert rounded fg-white" OnClick="but_xemtiep_Click" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>

