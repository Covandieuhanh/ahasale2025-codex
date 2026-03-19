<%@ Control Language="C#" AutoEventWireup="true" CodeFile="slider.ascx.cs" Inherits="myweb_uc_metro_style_slider" %>
<%--data-height="@(max-width: 100vw),21/9 | (max-width: 768px),16/9 | (max-width: 576px),4/3"--%>
<%--data-cls-bullet-on="bg-red" màu của nút ở dưới--%>
<%--data-controls="false"hiển thị nút tới lui--%>
<%--data-bullets="false"icon điều khiển ở dưới--%>
<%--data-effect-func="easeOutBounce"hiệu ứng--%>
<%--data-cls-slides="rounded"--%>
<%--data-effect="fade" data-effect="slide" data-effect="slide-v"--%>

<div class="home-carousel"
    data-role="carousel"
    data-bullet-style="circle"
    data-effect="fade" data-height="21/9"
    data-control-next="<span class='mif-chevron-right'></span>"
    data-control-prev="<span class='mif-chevron-left'></span>"
    data-cls-controls="fg-lightEmerald"
    data-cls-bullet-on="bg-lightEmerald"
    data-bullets="true"
    data-auto-start="true"
    data-period="4800"
    data-duration="700"
    data-effect-func="easeInQuart">
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#Eval("link").ToString() == "" %>'>
                <div class="slide" data-cover="<%#Eval("img").ToString()%>"></div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("link").ToString() != "" %>'>
                <div class="slide" data-cover="<%#Eval("img").ToString()%>">
                    <div style="position: absolute; width: 100%;height:100%; z-index: 1" class="text-center">
                        <a href="<%#Eval("link").ToString()%>" class="button bg-transparent fg-white" style="width: 80%;height:100%;"></a>
                    </div>
                </div>
            </asp:PlaceHolder>
        </ItemTemplate>
    </asp:Repeater>
</div>
