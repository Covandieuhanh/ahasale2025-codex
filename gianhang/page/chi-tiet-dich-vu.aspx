<%@ Page Title="" Language="C#" MasterPageFile="~/gianhang/mp-home.master" AutoEventWireup="true" CodeFile="chi-tiet-dich-vu.aspx.cs" Inherits="danh_sach_bai_viet" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <%=meta %>
    <meta property="og:type" content="article" />
    <style>
        .bd-style-bc1 {
            border-right: 1px solid #eeeeee
        }

        .bd-style-bc2 {
            border-top: 0px solid #eeeeee
        }

        @media (max-width: 991px) { /*lg*/
            .bd-style-bc1 {
                border-right: 0px solid #eeeeee
            }

            .bd-style-bc2 {
                border-top: 1px solid #eeeeee;
                padding-top: 40px
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="container-fluid pt-10-lg pt-6 pb-10">
        <div class="container">
            <div class="row">
                <div class="cell-lg-8 pr-4-lg bd-style-bc1">
                    <div class="text-bold mt-1 fs-bc-18-16"><%=title_web %></div>

                    <div class="fg-darkGray"><%=des %></div>
                    <div class="text-center">
                        <img src="<%=image %>" class="mt-2" />
                    </div>
                    <div class="fg-emerald mt-4 text-bold">
                        <span class="mif mif-3x mif-dollar2 pr-1"></span>GIÁ DỊCH VỤ: <%=gia %> VNĐ                 
                    </div>
                    <div class="d-flex mt-4">
                        <asp:Button ID="but_dathang" runat="server" Text="Đặt lịch ngay" CssClass="bg-emerald bg-darkEmerald-hover fg-white" OnClick="but_dathang_Click" />
                    </div>
                    <div class="text-just mt-4 chitiet-baiviet-bc">
                        <%=noidung %>
                    </div>

                    <div style="border-top: 1px solid #eeeeee" class="mt-9">
                        <asp:Panel ID="Panel1" runat="server">
                            <div style="border-left: 4px solid #008a00;" class="fw-600 mt-9">
                                <div class="pl-2 fs-bc-34-28">
                                    <a class="fg-black fg-emerald-hover" href="<%=url_menu %>">
                                        <div class=" text-bold text-upper" style="font-size: 28px">CÙNG CHỦ đề</div>
                                    </a>
                                </div>
                            </div>
                        </asp:Panel>
                        <asp:Repeater ID="Repeater2" runat="server">
                            <ItemTemplate>
                                <div class="row mt-9">
                                    <div class="cell-lg-4">
                                        <a href="<%# BuildRelatedDetailUrl(Eval("id")) %>">
                                            <img src="<%#Eval("image") %>" class="radius-bc-4" /></a>
                                    </div>
                                    <div class="cell-lg-8 pl-3-lg">
                                        <div class="fw-600 ellipsis-2 mt-0-lg mt-2">
                                            <a class="fg-black fg-emerald-hover" href="<%# BuildRelatedDetailUrl(Eval("id")) %>"><%#Eval("name") %></a>
                                        </div>
                                        <div style="color: #808080" class="mt-1">
                                            <%#Eval("description") %>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
                <div class="cell-lg-4  pl-4-lg mt-0-lg mt-14 bd-style-bc2 ">
                </div>
            </div>


        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>
