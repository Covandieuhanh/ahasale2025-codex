<%@ Page Title="Sổ quỹ tiền mặt" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="so-quy-tien-mat.aspx.cs" Inherits="badmin_Default" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <div class="row border-bottom bd-lightGray m-3">
        <div class="cell-md-6 text-center text-left-md">
            <h3 class="dashboard-section-title">Sổ quỹ tiền mặt</h3>
        </div>
        <div class="cell-md-6 d-flex flex-justify-center flex-justify-end-md">
            <div class="you-are-here mt-1-md mt-4-minus">
                <a href="/gianhang/admin/Default.aspx">Trang chủ</a>
            </div>
            <div class="you-are-here mt-1-md mt-4-minus ml-2 mr-2">
                <a href="/gianhang/admin/quan-ly-thu-chi/Default.aspx">> Quản lý thu chi</a>
            </div>
        </div>
    </div>

    <div id='form_locdulieu' style="position: fixed; width: 100%; height: 100%; z-index: 100!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');" class="pop-up-form">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 600px; opacity: 1; margin-left: 0;' class="pl-3 pr-3 ">
            <div style='position: absolute; right: 20px; top: 12px; z-index: 999!important'>
                <a class='fg-white d-inline c-pointer' onclick='show_hide_id_form_locdulieu()' title='Đóng'>
                    <span class='mif mif-cross mif-lg fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="presenta-box bg-white border-radius-10  border bd-transparent pl-4 pl-10-md pr-10-md pr-4 pt-10 mt-10-md mt-5">
                <h3>Lọc dữ liệu</h3>
                <asp:Panel ID="Panel3" runat="server" DefaultButton="but_locdulieu">
                    <div class="mt-7">
                        <label>Lọc theo nhóm</label>
                        <asp:DropDownList ID="ddl_loc1" runat="server" data-role="select" data-filter="false"></asp:DropDownList>
                    </div>
                    <div class="mt-3">
                        <label>Lọc theo loại phiếu</label>
                        <asp:DropDownList ID="ddl_loc2" runat="server" data-role="select" data-filter="false">
                            <asp:ListItem Text="Tất cả" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Phiếu thu" Value="Thu"></asp:ListItem>
                            <asp:ListItem Text="Phiếu chi" Value="Chi"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="mt-6 mb-10 text-right">
                        <div style="float: left">
                            <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                        </div>
                        <div style="float: right" onclick='show_hide_id_form_locdulieu()'>
                            <asp:Button ID="but_locdulieu" runat="server" Text="LỌC" CssClass="button warning" OnClick="but_locdulieu_Click" />
                        </div>
                        <div style="clear: both"></div>
                    </div>
                </asp:Panel>
            </div>
        </div>
        <script>
            function show_hide_id_form_locdulieu() {
                var x = document.getElementById("form_locdulieu");
                if (x.style.display === "none") { x.style.display = "block"; }
                else { x.style.display = "none"; }
            };
        </script>
    </div>

    <div id='form_locthoigian' style="position: fixed; width: 100%; height: 100%; z-index: 100!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');" class="pop-up-form">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 600px; opacity: 1; margin-left: 0;' class="pl-3 pr-3">
            <div style='position: absolute; right: 20px; top: 12px; z-index: 999!important'>
                <a class='fg-white d-inline c-pointer' onclick='show_hide_id_form_locthoigian()' title='Đóng'>
                    <span class='mif mif-cross mif-lg fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="presenta-box bg-white border-radius-10  border bd-transparent pl-4 pl-10-md pr-10-md pr-4 pt-10 mt-20">
                <h3>Lọc thời gian</h3>
                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="mt-1-minus">
                            <span onclick="show_hide_id_form_locthoigian()">
                                <asp:Button ID="but_loc_homqua" runat="server" Text="Hôm qua" CssClass="mt-1" OnClick="but_loc_homqua_Click" /></span>
                            <span onclick="show_hide_id_form_locthoigian()">
                                <asp:Button ID="but_loc_homnay" runat="server" Text="Hôm nay" CssClass="mt-1" OnClick="but_loc_homnay_Click" /></span>
                            <span onclick="show_hide_id_form_locthoigian()">
                                <asp:Button ID="but_loc_tuantruoc" runat="server" Text="Tuần trước" CssClass="mt-1" OnClick="but_loc_tuantruoc_Click" /></span>
                            <span onclick="show_hide_id_form_locthoigian()">
                                <asp:Button ID="but_loc_tuannay" runat="server" Text="Tuần này" CssClass="mt-1" OnClick="but_loc_tuannay_Click" /></span>
                            <span onclick="show_hide_id_form_locthoigian()">
                                <asp:Button ID="but_loc_thangtruoc" runat="server" Text="Tháng trước" CssClass="mt-1" OnClick="but_loc_thangtruoc_Click" /></span>
                            <span onclick="show_hide_id_form_locthoigian()">
                                <asp:Button ID="but_loc_thangnay" runat="server" Text="Tháng này" CssClass="mt-1" OnClick="but_loc_thangnay_Click" /></span>
                            <span onclick="show_hide_id_form_locthoigian()">
                                <asp:Button ID="but_loc_namtruoc" runat="server" Text="Năm trước" CssClass="mt-1" OnClick="but_loc_namtruoc_Click" /></span>
                            <span onclick="show_hide_id_form_locthoigian()">
                                <asp:Button ID="but_loc_namnay" runat="server" Text="Năm này" CssClass="mt-1" OnClick="but_loc_namnay_Click" /></span>

                        </div>
                        <asp:Panel ID="Panel2" runat="server" DefaultButton="but_locthoigian">
                            <div class="mt-6">
                                <label>Từ ngày</label>
                                <asp:TextBox ID="txt_tungay" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label>Đến ngày</label>
                                <asp:TextBox ID="txt_denngay" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                            </div>

                            <div class="mt-6 mb-10">
                                <div style="float: left">
                                    <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                </div>
                                <div style="float: right" onclick="show_hide_id_form_locthoigian()">
                                    <asp:Button ID="but_locthoigian" runat="server" Text="LỌC" CssClass="button warning" OnClick="but_locthoigian_Click" />
                                </div>
                                <div style="clear: both"></div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

        </div>
    </div>
    <script>
        function show_hide_id_form_locthoigian() {
            var x = document.getElementById("form_locthoigian");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="but_locdulieu" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="but_locthoigian" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="but_loc_homqua" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="but_loc_homnay" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="but_loc_tuantruoc" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="but_loc_tuannay" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="but_loc_thangtruoc" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="but_loc_thangnay" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="but_loc_namtruoc" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="but_loc_namnay" EventName="Click" />
        </Triggers>
        <ContentTemplate>
            <%--BUTTON TOP--%>
            <%--<div class="ml-3 mr-3 ">
                <div class="text-left-md text-center">
                    <div class="dropdown-button">
                        <button class="button dropdown-toggle secondary">Danh sách</button>
                        <ul class="d-menu" data-role="dropdown">
                            <li><a href="#">Danh sách menu</a></li>
                            <li class="divider"></li>
                            <li><a href="#">Mục đã xóa</a></li>
                        </ul>
                    </div>
                    <asp:Button ID="but_home" runat="server" Text="Trang chủ" CssClass="secondary mt-1 d-none d-inline-md" />
                    <asp:Button ID="Button4" runat="server" Text="Home" CssClass="secondary mt-1 d-none-md d-inline" />
                </div>
            </div>--%>
            <%--END BUTTON TOP--%>

            <%--NỘI DUNG CHÍNH--%>

            <div class="ml-3-md mr-3-md ml-0 mr-0 mt-3 bg-white ">
                <div class="pl-3 pr-3 pb-10">
                    <%--TIÊU ĐỀ - MENU - THANH CÔNG CỤ--%>
                    <div>
                        <div style="float: left" class="mt-3">
                            <%--<h4>Danh sách thu chi</h4>--%>
                            <ul class="h-menu ">
                                <li data-role="hint" data-hint-position="top" data-hint-text="Quay lại"><a class="button" href="/gianhang/admin/quan-ly-thu-chi/Default.aspx"><span class="mif mif-arrow-left"></span></a></li>                                                                

                                <%--<li class="bd-gray border bd-default" style="height:38px"></li>--%>
                                <%--<li class="bd-gray border bd-default mt-1" style="height:28px"></li>--%>

                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Lọc dữ liệu"><a class="button" onclick='show_hide_id_form_locdulieu()'><span class="mif mif-filter"></span></a></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Thời gian"><a class="button" onclick='show_hide_id_form_locthoigian()'><span class="mif mif-calendar"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li>
                                    <a href="#" <%-- class="dropdown-toggle"--%>><span class="mif mif-more-vert"></span></a>
                                    <ul class="d-menu place-right" data-role="dropdown">
                                        <li><a href="/gianhang/admin/quan-ly-thu-chi/add.aspx">Lập phiếu thu chi</a></li>
                                        <li><a href="/gianhang/admin/quan-ly-thu-chi/Default.aspx">Quản lý thu chi</a></li>
                                        <li><a href="/gianhang/admin/quan-ly-thu-chi/nhom-thu-chi.aspx">Quản lý nhóm thu chi</a></li>                                        
                                        <%--<li class="divider"></li>--%>
                                    </ul>
                                </li>
                            </ul>
                        </div>
                        <div style="float: right" class="mt-3">
                            <ul class="h-menu ">
                               
                            </ul>
                        </div>
                        <div class="clr-float"></div>
                    </div>
                    <%--END TIÊU ĐỀ - MENU - THANH CÔNG CỤ--%>



                    <%--TABLE CHÍNH--%>
                    <div style="overflow: auto; min-height: 700px" class="mt-2">
                        <table class="table row-hover table-border cell-border striped <%--compact--%>">
                                        <thead>
                                            <tr>
                                                <tr class="border-top bd-default">
                                                    <td colspan="7" class="text-right fg-red">Tồn đầu kỳ</td>
                                                    <td class="text-right fg-red"><%=tondauky.ToString("#,##0") %></td>
                                                </tr>
                                                <th style="width: 1px" class="text-center">STT</th>
                                                <th style="width: 1px">Ngày</th>
                                                <th style="min-width: 200px">Người lập phiếu</th>
                                                <td class="text-bold" style="min-width: 150px;">Nhóm</td>
                                                <th style="min-width: 200px">Nội dung</th>
                                                <th style="width: 100px">Thu</th>
                                                <th style="width: 100px">Chi</th>
                                                <th style="width: 100px">Tồn</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Repeater ID="Repeater2" runat="server">
                                                <ItemTemplate>
                                                    <tr>

                                                        <td class="text-center">
                                                            <%=stt %>                                                         
                                                        </td>
                                                        <td>
                                                            <%#Eval("ngay","{0:dd/MM/yyyy}").ToString() %>
                                                        </td>
                                                        <td><%#Eval("nguoilapphieu") %></td>
                                                        <td> <%#Eval("tennhom").ToString() %></td>
                                                        <td><%#Eval("noidung") %></td>
                                                        <td class="text-right">
                                                            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#Eval("thuchi").ToString()=="Thu" %>'>
                                                                <%#Eval("sotien","{0:#,##0}") %>
                                                            </asp:PlaceHolder>
                                                        </td>
                                                        <td class="text-right">
                                                            <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#Eval("thuchi").ToString()=="Chi" %>'>
                                                                <%#Eval("sotien","{0:#,##0}") %>
                                                            </asp:PlaceHolder>
                                                        </td>
                                                        <td class="text-right">
                                                            <%#tinhtonhientai(Eval("id").ToString())%>
                                                        </td>
                                                    </tr>
                                                    <%stt = stt + 1; %>
                                                </ItemTemplate>
                                            </asp:Repeater>

                                        </tbody>
                                        <thead>
                                            <tr class="border-top bd-default">
                                                <td colspan="7" class="text-right fg-red">Tồn cuối kỳ</td>
                                                <td class="text-right fg-red"><%=tonhientai.ToString("#,##0") %></td>
                                            </tr>
                                            <tr>
                                                <td colspan="8" class="text-right">Số tiền bằng chữ: <%=tienbangchu %> đồng.</td>
                                            </tr>
                                        </thead>
                                    </table>
                    </div>
                    <%--END TABLE CHÍNH--%>

                </div>
            </div>
            <%--END DUNG CHÍNH--%>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 50vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%--<%=notifi %>--%>
</asp:Content>

