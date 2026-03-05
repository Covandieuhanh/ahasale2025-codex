<%@ page title="" language="C#" masterpagefile="~/home/MasterPageHome.master" autoeventwireup="true" inherits="_Default, App_Web_4g0uj05j" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <meta property="og:type" content="website" />
    <%--title & meta--%>
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

        #ui-id-1 {
            max-width: 300px;     
            width: auto !important; 
            overflow: hidden;
            box-sizing: border-box;
        }

        #ui-id-1 .ui-menu-item-wrapper {
            white-space: nowrap;       
            overflow: hidden;         
            text-overflow: ellipsis;  
            line-height: 34px;     
            height: 34px;          
            padding: 0 10px;      
            box-sizing: border-box;
        }

        .input-search{
            height: 35px;
            min-width: 250px;
            right: 10px;
        }

        .dropdown-menu ul li{
            list-style: none;
        }
        
        .menu-container {
            display: flex;
            flex-wrap: wrap;
            justify-content: space-between;
            gap: 10px;
            align-items: flex-start;
            padding: 10px 0;
        }

        .tool-group {
            display: flex;
            flex-wrap: wrap;
            align-items: center;
            gap: 10px;
            flex: 1;
        }

        .search-wrapper {
            display: flex;
            width: 100%;
            background-color: white;
            border-radius: 5px;
            overflow: hidden;
            border: 1px solid #ccc;
        }

        .dropdown-select select, 
        .btn-thanhpho {
            border: none;
            padding: 10px 12px;
            background: transparent;
            min-width: 150px;
            cursor: pointer;
            appearance: none;
        }

        .input-search {
            flex: 1;
            border: none;
            padding: 10px 12px;
            font-size: 16px;
            outline: none;
        }

        .btn-search {
            background-color: #ffa500;
            border: none;
            padding: 0 16px;
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
            cursor: pointer;
        }

        .btn-search i {
            pointer-events: none;
        }


        #phPageNumbers {
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
            margin-bottom: 10px;
        }

        .paging-button {
            padding: 3px 10px;
            border: 1px solid #0078D7;
            background-color: white;
            color: #0078D7;
            cursor: pointer;
            font-size: 14px;
            font-family: 'Segoe UI', sans-serif;
        }

        .paging-button.active {
            background-color: #0078D7;
            color: white;
        }

        @media (max-width: 768px) {
            .menu-container {
                flex-direction: column;
                align-items: stretch;
            }

            .tool-group {
                flex-direction: column;
                align-items: stretch;
            }

            .btn-thanhpho,
            .input-search {
                width: 100%;
            }

            .paging-button {
                flex: 1 1 auto;
                text-align: center;
            }

            .dropdown-select select, .btn-thanhpho {
                font-size: 12px !important;
            }
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
                                            <asp:Literal ID="Literal1" runat="server"></asp:Literal></label>
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

    <div class="container text-center p-0" style="max-width: 992px !important">
        <div class=" bg-ahasale" data-role="carousel" data-effect="slide"
            <%--data-height="21/9"--%>
            data-height="@ (max-width: 2000px),240px | (max-width: 768px),190px | (max-width: 576px),140px"
            data-control-next="<span class='mif-chevron-right'></span>"
            data-control-prev="<span class='mif-chevron-left'></span>"
            data-cls-controls="fg-yellow"
            data-cls-bullet-on="bg-yellow"
            data-auto-start="true"
            data-period="3000"
            data-duration="1000"
            data-controls="false"
            data-bullets="false"
            data-effect-func="easeInQuart">

            <asp:Repeater ID="Repeater3" runat="server">
                <ItemTemplate>
                    <div class="slide" data-cover='<%#Eval("img") %>'></div>
                </ItemTemplate>
            </asp:Repeater>


            <%--<div class="slide" data-cover="/uploads/images/slide/ct2.jpg"></div>--%>
        </div>
    </div>

    <div class="container p-0 bg-ahasale1" style="max-width: 992px !important">
        <div class="p-4">
            <div class="text-bold fg-ahasale">Khám phá danh mục</div>
            <div class="danhmuc-container mt-4 pb-4">
                <asp:Repeater ID="Repeater1" runat="server">
                    <ItemTemplate>

                        <a href='/<%# Eval("name_en") %>-<%# Eval("id") %>' class="danhmuc-item">
                            <!-- Hình ảnh danh mục -->
                            <img src='<%# Eval("image") %>' width="70" height="70" />
                            <!-- Tên danh mục -->
                            <div><%# Eval("name") %></div>
                        </a>

                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>

    <div class="container p-0 bg-ahasale1 mt-3" style="max-width: 992px !important">
        <div class="p-4">
            <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="row">
                        <div class="cell-lg-6">
                            <div class="text-bold mb-3 fg-ahasale">Tin đăng mới</div>
                        </div>
                    </div>
                    <div class="search-wrapper mb-2">
                        <div class="dropdown-select">
                            <asp:DropDownList ID="ddlThanhPho" runat="server" CssClass="btn-thanhpho" AutoPostBack="true" OnSelectedIndexChanged="ddlThanhPho_SelectedIndexChanged" />
                        </div>

                        <asp:TextBox 
                            ID="txt_search" 
                            runat="server" 
                            placeholder="Nhập từ khóa..." 
                            MaxLength="50"
                            CssClass="input-search"
                            AutoPostBack="true"
                            OnTextChanged="txt_search_TextChanged" 
                        />
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
                                                    <%--<img src="/uploads/images/dong-a.png" style="width: 14px!important" /><small class="pl-1"><%# Eval("giaban", "{0:#,##0}") %></small>--%>
                                                    <small class="pl-1"><%# Eval("giaban", "{0:#,##0}") %> đ</small>
                                                </div>
                                                <div style="font-size: 12px">
                                                    <div style="float: left"><small class="fg-gray"><%# Eval("ngaytao", "{0:dd/MM/yyyy}") %>'</small></div>
                                                    <div style="float: right;display: flex; flex-direction: column;">
                                                        <small class="fg-gray">Lượt truy cập: <%# Eval("LuotTruyCap", "{0:#,##0}") %></small>
                                                        <small class="fg-gray"><span class='mif-location fg-white'></span> <%# Eval("ThanhPho") %></small>
                                                    </div>
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
                                                    <div style="float: left"><small class="fg-gray"><%# Eval("ngaytao", "{0:dd/MM/yyyy}") %>'</small></div>
                                                    <div style="float: right;display: flex; flex-direction: column;">
                                                        <small class="fg-gray">Lượt truy cập: <%# Eval("LuotTruyCap", "{0:#,##0}") %></small>
                                                        <small class="fg-gray"><span class='mif-location fg-white'></span> <%# Eval("ThanhPho") %></small>
                                                    </div>
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
            <div>
                <asp:PlaceHolder ID="phPageNumbers" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <!-- jQuery (phải trước jQuery UI) -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <!-- jQuery UI -->
    <link href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css" rel="stylesheet" />
    <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>

    <script type="text/javascript">
        

        $(function () {
            var txtSearch = $('#<%= txt_search.ClientID %>');
            txtSearch.on('input', function () {
                var value = $(this).val();
                if (value.length >= 2) {
                    $(this).autocomplete("search", value);
                }
            });
            txtSearch.autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        url: "Default.aspx/GetSuggestions",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify({ prefixText: request.term, count: 10 }),
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
        });
    </script>
</asp:Content>

