<%@ page title="Trang cá nhân" language="C#" masterpagefile="~/home/MasterPageHome.master" autoeventwireup="true" inherits="home_Default, App_Web_i5twubwd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>
    <style>
        .square-container {
            width: 100%;
            position: relative;
            overflow: hidden;
        }

        .square-container::before {
            content: "";
            display: block;
            padding-top: 100%; /* Chiều cao = 100% chiều rộng */
        }

        .square-container img {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            object-fit: cover;
        }

        .text-clamp-1 {
            display: -webkit-box;
            -webkit-line-clamp: 1; /* Giới hạn tối đa 2 dòng */
            -webkit-box-orient: vertical;
            overflow: hidden;
            text-overflow: ellipsis;
            line-height: 18px;
            min-height: 40px
        }

        .text-clamp-2 {
            display: -webkit-box;
            -webkit-line-clamp: 2; /* Giới hạn tối đa 2 dòng */
            -webkit-box-orient: vertical;
            overflow: hidden;
            text-overflow: ellipsis;
            line-height: 18px;
            min-height: 40px
        }

        .custom-button {
            display: flex;
            align-items: center;
            width: 100%;
            max-width: 100%;
            padding: 10px;
            background-color: #e0e0e0;
            color: black;
            text-decoration: none;
            font-size: 14px;
            font-weight: bold;
            border-radius: 8px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            transition: background 0.3s ease;
            position: relative;
        }

        .custom-button:hover {
            background-color: #d6d6d6;
        }

        .custom-button img {
            width: 28px;
            height: 28px;
            border-radius: 50%;
            position: absolute;
            left: 12px;
            top: 50%;
            transform: translateY(-50%);
        }

        .custom-button span {
            flex: 1;
            text-align: center;
        }
        
        .modal {
            position: fixed;
            z-index: 9999;
            padding-top: 60px;
            left: 0;
            top: 0;
            width: 100%; 
            height: 100%; 
            overflow: auto;
            background-color: rgba(0,0,0,0.8);
        }

        .modal-content {
            margin: auto;
            display: block;
            max-width: 80%;
            max-height: 80%;
            border-radius: 4px;
        }

        .close {
            position: absolute;
            top: 20px;
            right: 35px;
            color: #fff;
            font-size: 40px;
            font-weight: bold;
            cursor: pointer;
        }

        .review-img {
            cursor: pointer;
        }

        .rounded-circle {
            object-fit: cover;
            object-position: 50% 50%;
            border-radius: 50%;
        }

        .pagination a, .pagination .current-page {
            padding: 5px 10px;
            margin: 2px;
            border: 1px solid #ccc;
            cursor: pointer;
            text-decoration: none;
            color: white;
        }

        .pagination .current-page {
            font-weight: bold;
            background-color: white;
            color: black;
            cursor: default;
        }

        .anh-ca-nhan{
            height: 200px;
            width: 100%;
            object-fit: cover;
        }

        .info-user{
            left: 0;
            top: 76%;
            position: absolute;
            padding: 0 20px;
        }

        .btn-info {
            border-radius: 5px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="up_dathang" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_dathang" runat="server" Visible="false" DefaultButton="but_dathang">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 600px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' id="A1" runat="server" onserverclick="but_close_form_dathang_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                Xác nhận trao đổi
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 606px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <%--pl-4 pl-8-md pr-8-md pr-4--%>
                            <div class="row">
                                <div class="cell-lg-12">
                                    <div class="mt-3">
                                        <label class="fw-600">
                                            <asp:Literal ID="Literal9" runat="server"></asp:Literal></label>
                                    </div>
                                    <div class="mt-1">
                                        <img src="/uploads/images/dong-a.png" style="width: 14px!important" /><small class="pl-1">
                                            <asp:Literal ID="Literal10" runat="server"></asp:Literal>
                                        </small>
                                    </div>
                                    <div class="mt-1">
                                        <label class="fw-600 fg-red"><small>Số lượng</small></label>
                                        <div>
                                            <asp:TextBox ID="txt_soluong2" AutoPostBack="true" OnTextChanged="txt_soluong2_TextChanged" runat="server" data-role="input" data-buttons-position="right" onfocus="AutoSelect(this)" MaxLength="3" oninput="format_sotien_new(this)"></asp:TextBox><%--data-min-value="1" data-max-value="999"--%>
                                        </div>
                                    </div>
                                    <div class="mt-1">
                                        <small>Tổng phải Trao đổi: </small>
                                        <img src="/uploads/images/dong-a.png" style="width: 14px!important" /><small class="pl-1">
                                            <asp:Literal ID="Literal11" runat="server"></asp:Literal>
                                        </small>
                                    </div>
                                    <div class="mt-1">
                                        <label class="fw-600 fg-red"><small>Người nhận</small></label>
                                        <div>
                                            <asp:TextBox ID="txt_hoten_nguoinhan" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-1">
                                        <label class="fw-600 fg-red"><small>Điện thoại</small></label>
                                        <div>
                                            <asp:TextBox ID="txt_sdt_nguoinhan" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-1">
                                        <label class="fw-600 fg-red"><small>Địa chỉ</small></label>
                                        <div>
                                            <asp:TextBox ID="txt_diachi_nguoinhan" runat="server" data-role="textarea" TextMode="MultiLine"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_dathang" OnClick="but_dathang_Click" runat="server" Text="Xác nhận trao đổi" CssClass="button dark" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_dathang">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="up_add_cart" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_add_cart" runat="server" Visible="false" DefaultButton="but_add_cart">
                <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                    <div style='top: 0; left: 0px; margin: 0 auto; max-width: 600px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' id="close_add" runat="server" onserverclick="but_close_form_addcart_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">
                                Thêm vào giỏ hàng
                            </div>
                            <hr />
                        </div>
                    </div>
                </div>
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='top: 0; left: 0; margin: 0 auto; max-width: 606px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                            <%--pl-4 pl-8-md pr-8-md pr-4--%>
                            <div class="row">
                                <div class="cell-lg-12">
                                    <div class="mt-3">
                                        <label class="fw-600">
                                            <asp:Literal ID="Literal8" runat="server"></asp:Literal></label>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600 fg-red"><small>Số lượng</small></label>
                                        <div>
                                            <asp:TextBox ID="txt_soluong1" runat="server" data-min-value="1" data-max-value="999" data-role="spinner" data-buttons-position="right" onfocus="AutoSelect(this)" MaxLength="3" oninput="format_sotien_new(this)"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="mt-6 mb-20 text-right">
                                <asp:Button ID="but_add_cart" OnClick="but_add_cart_Click" runat="server" Text="Thêm vào giỏ hàng" CssClass="button dark" />
                            </div>
                            <div class="mb-20"></div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="up_add_cart">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>




    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="container pt-10-lg pt-3 pb-10-lg pb-3" style="max-width: 992px !important">
                <div class="row">
                    <div class="cell-lg-5 pr-4-lg fg-ahasale">
                        <div style="min-height: 500px;" class="bg-ahasale1 p-4">
                            <div>
                                <div>
                                    <asp:Literal ID="Literal13" runat="server"></asp:Literal>
                                </div>
                                <div class="info-user w-100">
                                    <img src="<%= ViewState["avt_query"] %>" width="100" height="100" class="img-cover-vuongtron border bg-white border-size-2" />
                                    <div style="font-size: 24px" class="text-bold fg-ahasale"><small><%= ViewState["hoten_query"] %></small> <%=ViewState["phanloai_query"] %></div>
                                    <div>
                                        Có <%= ViewState["SoLuongDanhGia"] %> lượt đánh giá
                                    </div>
                                    <div class="d-flex align-items-center">
                                        Người theo dõi: 0  <div style="height: 20px; border-left: 1px solid white; margin: 0 12px;"></div>  Đang theo dõi: 0
                                    </div>
                                    <div>
                                        <a href="/" class="btn-info button warning w-100 mt-4">
                                            Chia sẻ trang của bạn
                                        </a>
                                    </div>
                                    <div>
                                        <a href="/home/edit-info.aspx" class="btn-info button button-outline dark w-100 mt-4">
                                            Chỉnh sửa trang cá nhân
                                        </a>
                                    </div>
                                    <%--<div class="p-3 bg-ahasale fg-ahasale mt-3 text-center">
                                        <small><asp:Literal ID="Literal4" runat="server"></asp:Literal></small>
                                    </div>
                                    <div class="text-center mt-3"><a class="fg-ahasale fg-yellow-hover" href="tel:<%= ViewState["sdt_query"] %>"><small><span class="mif-phone pr-1"></span><%= ViewState["sdt_query"] %></small></a></div>
                                    <div class="text-center">
                                        <span class="mif-location"></span>
                                        <small>
                                            <asp:Literal ID="Literal5" runat="server"></asp:Literal>
                                        </small>
                                    </div>--%>
                                </div>
                            </div>
                            <div style="margin-top: 270px !important; padding: 0 20px;" >
                                <asp:Repeater ID="rptMangXaHoiCN" runat="server">
                                <ItemTemplate>
                                    <div class="cell-lg-12 d-flex mb-4">
                                        <a href='<%# Eval("Link") %>' target="_blank" style="text-decoration: none; color: inherit;">
                                            <div class="d-flex align-items-center">
                                                <asp:Image ID="imgIcon" runat="server"
                                                    ImageUrl='<%# Eval("Icon") %>'
                                                    Width="50" Height="50"
                                                    Style="object-fit: cover; border-radius: 5px; margin-right: 10px;"
                                                    Visible='<%# ShouldShowIcon(Eval("Icon")) %>' />

                                                <div style='<%# GetMarginStyle(Eval("Icon")) %>'>
                                                    <div class="fw-600 fg-ahasale">
                                                        <small><%# Eval("Ten") %></small>
                                                    </div>
                                                    <div style="font-style: italic; color: gray; font-size: 0.9em;">
                                                        <%# Eval("Link") %>
                                                    </div>
                                                </div>
                                            </div>
                                        </a>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            </div>
                            <%--<div class="bg-ahasale1 d-flex flex-justify-between flex-equal-items p-2">
                                <a class="button flat-button fg-white" target="_blank" href="zalo://profile?q=<%= ViewState["zalo_query"] %>" style="background-color: #0068FF">Zalo: <%= ViewState["zalo_query"] %></a>
                                <a class="button flat-button fg-white" target="_blank" href="<%= ViewState["fb_query"] %>" style="background-color: #08509D">Facebook</a>
                            </div>--%>
                            <%--<div class="text-left word-wrap-bc">
                                <small><a target="_blank" href="<%= ViewState["tiktok_query"] %>">TikTok của tôi</a></small>
                            </div>
                            <div class="text-left word-wrap-bc">
                                <small><a target="_blank" href="<%= ViewState["ytb_query"] %>">Youtube của tôi</a></small>
                            </div>--%>

                            <%-- <a href='#' class='custom-button mt-3'>
                                <img src='/uploads/images/icon/icon-fb.png' alt='icon'>
                                <span>Bôn Bắp</span>
                            </a>
                            <a href='#' class='custom-button mt-3'>
                                <img src='/uploads/images/icon/icon-zalo.png' alt='icon'>
                                <span>Bôn Bắp</span>
                            </a>
                            <a href='#' class='custom-button mt-3'>
                                <img src='/uploads/images/icon/icon-youtube.png' alt='icon'>
                                <span>Bôn Bắp</span>
                            </a>
                            <a href='#' class='custom-button mt-3'>
                                <img src='/uploads/images/icon/icon-tiktok.png' alt='icon'>
                                <span>Bôn Bắp</span>
                            </a>--%>
                        </div>
                    </div>
                    <div class="cell-lg-7 pl-4-lg mt-3 mt-0-lg fg-ahasale">
                        <div class=" bg-ahasale1 p-4">
                            <div class="text-bold fg-ahasale"><span class="mif-shop pr-1"></span>Giới thiệu cửa hàng</div>
                            <div>
                                <asp:Literal ID="Literal6" runat="server"></asp:Literal>
                            </div>
                            <div class="p-4 bg-ahasale fg-ahasale mt-3">
                                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                            </div>
                            <div class="mt-2">
                                <span class="mif-phone pr-1"></span>
                                <asp:Literal ID="Literal2" runat="server"></asp:Literal>
                            </div>
                            <div>
                                <span class="mif-location pr-1"></span>
                                <asp:Literal ID="Literal3" runat="server"></asp:Literal>
                            </div>
                            <asp:Repeater ID="rptMangXaHoiCH" runat="server">
                                    <ItemTemplate>
                                        <div class="cell-lg-12 d-flex mt-2 mb-3">
                                            <a href='<%# Eval("Link") %>' target="_blank" style="text-decoration: none; color: inherit;">
                                                <div class="d-flex align-items-center">
                                                    <asp:Image ID="imgIcon" runat="server"
                                                        ImageUrl='<%# Eval("Icon") %>'
                                                        Width="50" Height="50"
                                                        Style="object-fit: cover; border-radius: 5px; margin-right: 10px;"
                                                        Visible='<%# ShouldShowIcon(Eval("Icon")) %>' />

                                                    <div style='<%# GetMarginStyle(Eval("Icon")) %>'>
                                                        <div class="fw-600 fg-ahasale">
                                                            <small><%# Eval("Ten") %></small>
                                                        </div>
                                                        <div style="font-style: italic; color: gray; font-size: 0.9em;">
                                                            <%# Eval("Link") %>
                                                        </div>
                                                    </div>
                                                </div>
                                            </a>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            <%-- <div class="bg-ahasale1 d-flex flex-justify-between flex-equal-items p-2">
                                <a class="button flat-button fg-white" target="_blank" href="zalo://profile?q=<%= ViewState["zalo_shop_query"] %>" style="background-color: #0068FF">Zalo: <%= ViewState["zalo_shop_query"] %></a>
                                <a class="button flat-button fg-white" target="_blank" href="<%= ViewState["fb_shop_query"] %>" style="background-color: #08509D">Facebook</a>
                            </div>
                            <div class="text-left word-wrap-bc">
                                <small><a target="_blank" href="<%= ViewState["tiktok_shop_query"] %>">TikTok của Shop</a></small>
                            </div>
                            <div class="text-left word-wrap-bc">
                                <small><a target="_blank" href="<%= ViewState["ytb_shop_query"] %>">Youtube của Shop</a></small>
                            </div>--%>

                        </div>
                    </div>
                </div>

                <div class=" bg-ahasale1 p-4 mt-4">
                    <div class="text-bold mb-3 fg-ahasale">
                        <div class="text-bold  fg-ahasale">Sản phẩm đang bán</div>
                    </div>
                     <div class="pos-relative">
                        <div id="menutop-tool-bc">
                            <ul class="h-menu bg-white">
                                <li class="d-block-lg d-none">
                                    <a data-role="hint" data-hint-position="top" data-hint-text="Hiển thị">
                                        <small><asp:Label ID="lb_show" runat="server" Text=""></asp:Label></small></a>
                                </li>
                                <li class="d-block-lg d-none" data-role="hint" data-hint-position="top" data-hint-text="Lùi">
                                    <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server"><span class="mif-chevron-left"></span></asp:LinkButton>
                                </li>
                                <li class="d-block-lg d-none" data-role="hint" data-hint-position="top" data-hint-text="Tới">
                                    <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server"><span class="mif-chevron-right"></span></asp:LinkButton>
                                </li>
                            </ul>
                        </div>
                        <div id="timkiem-fixtop-bc" style="position: absolute; right: 10px; top: 6px; width: 300px; z-index: 4" class="d-none d-block-sm">
                            <asp:TextBox MaxLength="50" CssClass="input-small" ID="txt_search" runat="server" data-role="input" AutoPostBack="true" placeholder="Nhập từ khóa..." OnTextChanged="txt_search_TextChanged" data-prepend="<span class='mif-search'>"></asp:TextBox><%--autocomplete="off" --%>
                        </div>
                    </div>
                    <div class="<%--border-top bd-lightGray--%> <%--pt-3 pl-3-lg pl-0 pr-3-lg pr-0 pb-3--%>p-3">
                       <div class="d-none-sm d-block">
                           <asp:TextBox MaxLength="50" data-prepend="<span class='mif mif-search'></span>" ID="txt_timkiem1" runat="server" placeholder="Nhập từ khóa" data-role="input" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                       </div>
                       <div style="background: white;" class="d-none-lg d-block mb-3 mt-0-lg mt-3">
                           <div class="place-left">
                               <%--<b><%=ViewState["title"] %></b> Nó k kịp lưu vì nó tải trang này trước khi load menu-left--%>
                           </div>
                           <div class="place-right text-right">
                               <small class="pr-1"><asp:Label ID="lb_show_md" runat="server" Text=""></asp:Label></small>
                               <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Lùi" ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" CssClass="button small light"><span class="mif-chevron-left"></span></asp:LinkButton>
                               <asp:LinkButton data-role="hint" data-hint-position="top" data-hint-text="Tới" ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" CssClass="button small light"><span class="mif-chevron-right"></span></asp:LinkButton>
                           </div>
                           <div class="clr-bc"></div>
                        </div>         
                    </div>         
                    <div class="p-4 bg-ahasale fg-ahasale mt-3">
                        <div class="row">
                            <div class="cell-6">
                                Sản phẩm: <asp:Literal ID="Literal7" runat="server"></asp:Literal>
                            </div>
                            <div class="cell-6">
                                Đã bán: <asp:Literal ID="Literal12" runat="server"></asp:Literal>
                            </div>
                        </div>

                    </div>

                    <div class="row">
                        <div class="cell-lg-6">
                            <div class="row">
                                <asp:Repeater ID="Repeater2" runat="server" OnItemDataBound="Repeater2_ItemDataBound">
                                    <ItemTemplate>
                                        <div class="cell-6 p-4-lg p-3 mt-5">
                                            <a href="/<%#Eval("name_en") %>-<%#Eval("id") %>.html" class="list-sp-bc fg-ahasale fg-yellow-hover">
                                                <div class="square-container">
                                                    <img src="<%# Eval("image") %>" alt="<%# Eval("name") %>" />
                                                </div>
                                                <div class="text-clamp-2 mt-1"><small><%# Eval("name") %></small></div>
                                                <div class="text-clamp-2"><small class="fg-gray"><%# Eval("description") %></small></div>
                                                <div class="fg-yellow text-bold">
                                                    <small class="pl-1"><%# Eval("giaban", "{0:#,##0}") %> đ</small>
                                                </div>
                                                <div style="font-size: 12px">
                                                    <div style="float: left"><small class="fg-gray"><%# Eval("ngaytao", "{0:dd/MM/yyyy HH:mm}") %>'</small></div>
                                                    <%--<div style="float: right"><small class="fg-gray">Đã bán: <%# Eval("soluong_daban", "{0:#,##0}") %></small></div>--%>
                                                    <div style="float: right"><small class="fg-gray">Lượt xem: <%# Eval("LuotTruyCap", "{0:#,##0}") %></small></div>
                                                    <div style="clear: both"></div>
                                                </div>
                                            </a>
                                            <div class="row">
                                                <div class="cell-6 pr-1">
                                                    <asp:Button ID="but_bansanphamnay" Width="100%" OnClick="but_bansanphamnay_Click" CommandArgument='<%# Eval("id") %>' runat="server" Text="Bán chéo" CssClass="yellow mini rounded" />
                                                </div>
                                                <div class="cell-6 pl-1">
                                                    <asp:Button ID="but_traodoi" OnClick="but_traodoi_Click" Width="100%" CommandArgument='<%# Eval("id") %>' runat="server" Text="Trao đổi ngay" CssClass="bg-yellow fg-black bg-amber-hover mini rounded" />
                                                </div>
                                            </div>
                                            <div>
                                                <asp:Button ID="but_themvaogio" OnClick="but_themvaogio_Click" Width="100%" CommandArgument='<%# Eval("id") %>' runat="server" Text="Thêm vào giỏ hàng" CssClass="light mini rounded" />
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                        <div class="cell-lg-6">
                            <div class="row">
                                <asp:Repeater ID="Repeater4" runat="server" OnItemDataBound="Repeater2_ItemDataBound">
                                    <ItemTemplate>
                                        <div class="cell-6 p-4-lg p-3 mt-5">
                                            <a href="/<%#Eval("name_en") %>-<%#Eval("id") %>.html" class="list-sp-bc fg-ahasale fg-yellow-hover">
                                                <div class="square-container">
                                                    <img src="<%# Eval("image") %>" alt="<%# Eval("name") %>" />
                                                </div>
                                                <div class="text-clamp-2 mt-1"><small><%# Eval("name") %></small></div>
                                                <div class="text-clamp-2"><small class="fg-gray"><%# Eval("description") %></small></div>
                                                <div class="fg-yellow text-bold">
                                                    <small class="pl-1"><%# Eval("giaban", "{0:#,##0}") %> đ</small>
                                                </div>
                                                <div style="font-size: 12px">
                                                    <div style="float: left"><small class="fg-gray"><%# Eval("ngaytao", "{0:dd/MM/yyyy HH:mm}") %>'</small></div>
                                                    <div style="float: right"><small class="fg-gray">Lượt xem: <%# Eval("LuotTruyCap", "{0:#,##0}") %></small></div>
                                                    <div style="clear: both"></div>
                                                </div>
                                            </a>
                                            <div class="row">
                                                <div class="cell-6 pr-1">
                                                    <asp:Button ID="but_bansanphamnay" Width="100%" OnClick="but_bansanphamnay_Click" CommandArgument='<%# Eval("id") %>' runat="server" Text="Bán chéo" CssClass="yellow mini rounded" />
                                                </div>
                                                <div class="cell-6 pl-1">
                                                    <asp:Button ID="but_traodoi" OnClick="but_traodoi_Click" Width="100%" CommandArgument='<%# Eval("id") %>' runat="server" Text="Trao đổi ngay" CssClass="bg-yellow fg-black bg-amber-hover mini rounded" />
                                                </div>
                                            </div>
                                            <div>
                                                <asp:Button ID="but_themvaogio" OnClick="but_themvaogio_Click" Width="100%" CommandArgument='<%# Eval("id") %>' runat="server" Text="Thêm vào giỏ hàng" CssClass="light mini rounded" />
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>

                </div>

            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="Review" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="true">
                <div class="review-form container bg-ahasale1 pt-5-lg pt-3 pb-5-lg pb-3 fg-ahasale" style="max-width: 992px !important">
                    <h3 class="text-center mt-4 mb-4 text-primary">Danh sách đánh giá</h3>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="ListReview" runat="server" Visible="true">
                <asp:Repeater ID="rptDanhGia" runat="server">
                    <ItemTemplate>
                        <div class="review-item bg-ahasale1 container pb-5-lg pb-3 fg-ahasale" style="max-width: 992px !important; display:flex; align-items:center; gap:15px;">
                            <asp:Image ID="imgAvatar" runat="server" ImageUrl='<%# Eval("AnhDaiDien") %>' Width="50" Height="50" CssClass="rounded-circle" AlternateText="Ảnh đại diện" />
            
                            <div>
                                <a style="color: white;" href="/<%#Eval("TaiKhoanDanhGia")%>.info"><b>Người dùng: <%# Eval("TaiKhoanDanhGia") %></b></a>
                                <br />
                                <span style="color: #ffc107; font-size: 18px;">
                                    <%# new string('★', Convert.ToInt32(Eval("Diem"))) %>
                                </span>
                                <br />
                                <span style="font-style: italic; font-size: 12px; color: gray;">
                                    <%# Eval("NgayDang", "{0:dd/MM/yyyy HH:mm}") %>
                                </span>
                                <br />
                                <span><%# Eval("NoiDung") %></span>
                                <br />
                                <asp:Image ID="imgReview" runat="server" ImageUrl='<%# Eval("UrlAnh") %>' Width="100" CssClass="review-img" Visible='<%# !string.IsNullOrEmpty(Eval("UrlAnh") as string) %>' />
                            </div>

                            <div id="imgModal" class="modal" style="display:none;">
                              <span class="close">&times;</span>
                              <img class="modal-content" id="modalImage" />
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:PlaceHolder>
            <div class="review-item bg-ahasale1 container pb-5-lg pb-3 fg-ahasale" style="max-width: 992px !important; display:flex; align-items:center; gap:15px;">
                <asp:Panel ID="pnlPaging" runat="server" CssClass="pagination"></asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <!-- jQuery (phải trước jQuery UI) -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <!-- jQuery UI -->
    <link href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css" rel="stylesheet" />
    <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>
    <script type="text/javascript">
        var user_query = '<%= ViewState["user_query"] %>';
    </script>
    <script type="text/javascript">
        $(function () {
            var txtSearch = $('#<%= txt_search.ClientID %>');
            txtSearch.autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        url: "home/Default.aspx/GetSuggestions",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify({
                            prefixText: request.term,
                            count: 10,
                            userQuery: user_query
                        }),
                        success: function (data) {
                            response(data.d);
                        }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    txtSearch.val(ui.item.value);
                    __doPostBack('<%= txt_search.UniqueID %>', '');
                    return false;
                }
            });

            var txtSearch1 = $('#<%= txt_timkiem1.ClientID %>');
            txtSearch1.autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        url: "home/Default.aspx/GetSuggestions",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify({
                            prefixText: request.term,
                            count: 10,
                            userQuery: user_query
                        }),
                        success: function (data) {
                            response(data.d);
                        }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    txtSearch1.val(ui.item.value);
                    __doPostBack('<%= txt_timkiem1.UniqueID %>', '');
                    return false;
                }
            });
        });
    </script>

</asp:Content>