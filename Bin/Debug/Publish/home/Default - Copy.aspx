<%@ page title="Trang cá nhân" language="C#" masterpagefile="~/home/MasterPageHome.master" autoeventwireup="true" inherits="home_Default, App_Web_md1cs0my" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>
    <style>
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
                        <div class=" bg-ahasale1 p-4">
                            <div class="fg-ahasale text-center mb-3 fw-600">Hồ sơ</div>
                            <div class="text-center">
                                <img src="<%= ViewState["avt_query"] %>" width="100" height="100" class="img-cover-vuongtron border bg-white border-size-2" />
                            </div>
                            <div class="text-bold text-center fg-ahasale"><small><%= ViewState["hoten_query"] %></small></div>
                            <div class="text-center"><%=ViewState["phanloai_query"] %></div>
                            <div class="p-3 bg-ahasale fg-ahasale mt-3 text-center">
                                <small>
                                    <asp:Literal ID="Literal4" runat="server"></asp:Literal></small>
                            </div>
                            <div class="text-center mt-3"><a class="fg-ahasale fg-yellow-hover" href="tel:<%= ViewState["sdt_query"] %>"><small><span class="mif-phone pr-1"></span><%= ViewState["sdt_query"] %></small></a></div>
                            <div class="text-center">
                                <span class="mif-location"></span>
                                <small>
                                    <asp:Literal ID="Literal5" runat="server"></asp:Literal></small>
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
                            <asp:Literal ID="lb_link_canhan" runat="server"></asp:Literal>

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
                            <asp:Literal ID="lb_link_shop" runat="server"></asp:Literal>

                        </div>
                    </div>
                </div>

                <div class=" bg-ahasale1 p-4 mt-4">
                    <div class="row">
                        <div class="cell-lg-6">
                            <div class="text-bold  fg-ahasale">Sản phẩm đang bán</div>
                        </div>
                        <div class="cell-lg-6 mt-2 mt-0-lg">
                            <asp:TextBox MaxLength="50" ID="txt_search" runat="server" data-role="input" AutoPostBack="true" placeholder="Nhập từ khóa..." OnTextChanged="txt_search_TextChanged" data-prepend="<span class='mif-search'>"></asp:TextBox><%--autocomplete="off" --%>
                        </div>
                    </div>
                    <div class="p-4 bg-ahasale fg-ahasale mt-3">
                        <div class="row">
                            <div class="cell-6">
                                Sản phẩm:
            <asp:Literal ID="Literal7" runat="server"></asp:Literal>
                            </div>
                            <div class="cell-6">
                                Đã bán:
            <asp:Literal ID="Literal12" runat="server"></asp:Literal>
                            </div>
                        </div>

                    </div>

                    <div class="row">
                        <div class="cell-lg-6">
                            <div class="row">...</div>
                        </div>
                        <div class="cell-lg-6">
                            <div class="row">..</div>
                        </div>
                    </div>


                    <div class="row">
                        <asp:Repeater ID="Repeater2" runat="server" OnItemDataBound="Repeater2_ItemDataBound">
                            <ItemTemplate>
                                <div class="cell-lg-6 mt-5 p-3-lg p-0 <%--border--%>">
                                    <a href="/<%#Eval("name_en") %>-<%#Eval("NguoiBan") %>-<%#Eval("id") %>.html" class="list-sp-bc fg-ahasale fg-yellow-hover">
                                        <div class="row">
                                            <div class="cell-3">
                                                <div class="img-container">
                                                    <img src="<%# Eval("image") %>" alt="<%# Eval("name") %>" />
                                                </div>
                                            </div>
                                            <div class="cell-9 pl-2">
                                                <div><small class="fw-600"><%# Eval("name") %></small></div>
                                                <div><small class="fg-gray"><%# Eval("description") %></small></div>
                                                <div class="fg-yellow text-bold">
                                                    <img src="/uploads/images/dong-a.png" style="width: 14px!important" /><small class="pl-1"><%# Eval("giaban", "{0:#,##0}") %></small>
                                                </div>
                                                <div><small class="fg-gray"><%# Eval("ngaytao", "{0:dd/MM/yyyy HH:mm}") %>'</small></div>
                                            </div>
                                        </div>
                                    </a>
                                    <div>
                                        <asp:Button ID="but_bansanphamnay" OnClick="but_bansanphamnay_Click" CommandArgument='<%# Eval("id") %>' runat="server" Text="Bán chéo" CssClass="yellow mini rounded" />
                                        <asp:Button ID="but_traodoi" OnClick="but_traodoi_Click" CommandArgument='<%# Eval("id") %>' runat="server" Text="Trao đổi" CssClass="bg-yellow fg-black bg-amber-hover mini rounded" />
                                        <asp:Button ID="but_themvaogio" OnClick="but_themvaogio_Click" CommandArgument='<%# Eval("id") %>' runat="server" Text="Thêm vào giỏ" CssClass="light mini rounded" />
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

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
</asp:Content>

