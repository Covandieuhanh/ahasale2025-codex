<%@ Page Title="Thêm bài viết" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="add.aspx.cs" Inherits="badmin_temp" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Register Namespace="CKFinder" Assembly="CKFinder" TagPrefix="CKFinder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <%if (bcorn_class.check_quyen(user, "q16_1") == "")
        { %>
    <div id='form_2' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_2()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Thêm ngành</h5>
                <hr />
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel2" runat="server" DefaultButton="but_form_themnhomthuchi">
                            <div class="mt-3">
                                <asp:TextBox ID="txt_tennhom" runat="server" data-role="input" placeholder="Nhập tên ngành"></asp:TextBox><%--autocomplete="off" --%>
                            </div>
                            <div class="mt-6 mb-10 text-right">
                                <asp:Button ID="but_form_themnhomthuchi" runat="server" Text="THÊM MỚI" CssClass="button success" OnClick="but_form_themnhomthuchi_Click" />
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanel2">
                    <ProgressTemplate>
                        <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                            <div style="padding-top: 50vh;">
                                <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                            </div>
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>

            </div>

        </div>
    </div>
    <script>
        function show_hide_id_form_2() {
            var x = document.getElementById("form_2");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>
    <%} %>


    <div id="main-content" class=" mb-10">
        <div class="row border-bottom bd-light">
            <div class="cell-6">
                <ul class="h-menu">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Quay lại">
                        <a class="button" href="<%=url_back %>"><span class="mif mif-arrow-left"></span></a></li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                </ul>
            </div>
            <div class="cell-6">
            </div>
        </div>
        <div class="row">
            <div class="cell-lg-12">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="button1">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="mt-3">
                                <label class=" fw-600">Phân loại bài viết</label>
                                <asp:DropDownList ID="DropDownList1" CssClass="select-input select" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                                    <asp:ListItem Text="Tin tức" Value="ctbv"></asp:ListItem>
                                    <asp:ListItem Text="Sản phẩm" Value="ctsp"></asp:ListItem>
                                    <asp:ListItem Text="Dịch vụ" Value="ctdv"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <asp:Panel ID="Panel_dichvu" runat="server">
                                <div class="row bg-light pb-4">
                                    <div class="cell-lg-4 pl-2-lg pr-2-lg mt-3">
                                        <label class=" fw-600 fg-blue">Giá dịch vụ</label>
                                        <asp:TextBox ID="txt_giadichvu" MaxLength="13" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox>
                                    </div>
                                    <div class="cell-lg-4 pl-2-lg pr-2-lg mt-3">
                                        <label class=" fw-600 fg-blue">Phần trăm chốt sale dịch vụ</label>
                                        <asp:TextBox ID="txt_chotsale_dichvu" MaxLength="3" runat="server" data-role="input" onchange="format_sotien(this);" placeholder="Từ 0-100"></asp:TextBox>
                                    </div>
                                    <div class="cell-lg-4 pl-2-lg pr-2-lg mt-3">
                                        <label class=" fw-600 fg-blue">Phần trăm làm dịch vụ</label>
                                        <asp:TextBox ID="txt_lam_dichvu" MaxLength="3" runat="server" data-role="input" onchange="format_sotien(this);" placeholder="Từ 0-100"></asp:TextBox>
                                    </div>
                                    <div class="cell-lg-4 pl-2-lg pr-2-lg mt-3">
                                        <label class=" fw-600 fg-blue">Thời lượng dịch vụ (phút)</label>
                                        <asp:TextBox ID="txt_thoiluong_dichvu" MaxLength="3" runat="server" data-role="input" onchange="format_sotien(this);" placeholder="Mặc định 60 phút"></asp:TextBox>
                                    </div>
                                    <div class="cell-lg-4 pl-2-lg pr-2-lg mt-3">
                                        <div class="mt-3">
                                            <label class="fw-600">Ngành</label>
                                            <div class="place-right">
                                                <a class=" fg-red fg-orange-hover" href="#" onclick="show_hide_id_form_2()"><small>Thêm nhanh</small></a>/ 
                                                 <a class=" fg-red fg-orange-hover" href="/gianhang/admin/quan-ly-he-thong/nganh.aspx"><small>Quản lý</small></a>
                                            </div>
                                            <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="but_form_themnhomthuchi" EventName="Click" />
                                                </Triggers>
                                                <ContentTemplate>
                                                    <asp:DropDownList ID="DropDownList5" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel ID="Panel_sanpham" runat="server">
                                <div class="row bg-light pb-4">
                                    <div class="cell-lg-6 pl-2-lg pr-2-lg mt-3">
                                        <label class=" fw-600 fg-green">Giá bán sản phẩm</label>
                                        <asp:TextBox ID="txt_giasanpham" MaxLength="13" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox>
                                    </div>
                                    <div class="cell-lg-6 pl-2-lg pr-2-lg mt-3">
                                        <label class=" fw-600 fg-green">Giá vốn sản phẩm</label>
                                        <asp:TextBox ID="txt_giavonsanpham" MaxLength="13" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox>
                                    </div>
                                    <div class="cell-lg-6 pl-2-lg pr-2-lg mt-3">
                                        <label class=" fw-600 fg-green">Phần trăm chốt sale sản phẩm</label>
                                        <asp:TextBox ID="txt_chotsale_sanpham" MaxLength="3" runat="server" data-role="input" onchange="format_sotien(this);" placeholder="Từ 0-100"></asp:TextBox>
                                    </div>
                                    <div class="cell-lg-6 pl-2-lg pr-2-lg mt-3">
                                        <label class=" fw-600 fg-green">Đơn vị tính</label>
                                        <asp:TextBox ID="txt_dvt_sp" runat="server" data-role="input"></asp:TextBox>
                                    </div>
                                    <div class="cell-lg-4 pl-2-lg pr-2-lg mt-3">
                                        <div class="mt-3">
                                            <label class="fw-600">Ngành</label>
                                            <div class="place-right">
                                                <a class=" fg-red fg-orange-hover" href="#" onclick="show_hide_id_form_2()"><small>Thêm nhanh</small></a>/ 
                                                 <a class=" fg-red fg-orange-hover" href="/gianhang/admin/quan-ly-he-thong/nganh.aspx"><small>Quản lý</small></a>
                                            </div>
                                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="but_form_themnhomthuchi" EventName="Click" />
                                                </Triggers>
                                                <ContentTemplate>
                                                    <asp:DropDownList ID="DropDownList6" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                    <%--<div class="cell-lg-6 pl-2-lg pr-2-lg mt-3">
                                        <label class=" fw-600 fg-green">Số lượng hiện tại</label>
                                        <asp:TextBox ID="txt_soluong_ton_sanpham" MaxLength="12" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox>
                                    </div>--%>
                                </div>
                            </asp:Panel>
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

                    <div class="mt-3">
                        <label class="fg-red fw-600">Tên bài viết</label>
                        <asp:TextBox ID="txt_name" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Menu</label>
                        <select name="listmenu" data-role="select">
                            <option value="0">Nhấn để chọn</option>
                            <%=result_listview %>
                        </select>
                    </div>

                    <div class="mt-3">
                        <label class="fw-600">Ảnh thu nhỏ</label>
                        <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Kích thước chuẩn: 1200x628 pixel.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                        <asp:FileUpload ID="FileUpload2" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Mô tả ngắn</label>
                        <asp:TextBox ID="txt_description" data-role="textarea" runat="server" TextMode="MultiLine"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <input type="checkbox" data-role="checkbox" name="check_noibat" data-caption="&nbsp;Bài viết nổi bật" data-style="2" data-cls-caption="fg-orange text-bold" data-cls-check="bd-orange myCheck">
                        <input type="checkbox" data-role="checkbox" name="check_hienthi" data-caption="&nbsp;Hiển thị trang chủ" data-style="2" data-cls-caption="fg-green text-bold" data-cls-check="bd-green myCheck" checked>
                    </div>
                    <div class="mt-3">
                        <div style="position: absolute; left: 0; bottom: 0">
                            <CKFinder:FileBrowser ID="FileBrowser1" Width="0" Height="0" runat="server" OnLoad="FileBrowser1_Load"></CKFinder:FileBrowser>
                        </div>
                        <label class="fw-600">Nội dung bài viết</label>
                        <CKEditor:CKEditorControl ID="txt_content" runat="server" Height="300"></CKEditor:CKEditorControl>
                    </div>




                    <div class="text-center mt-10">
                        <asp:Button ID="button1" runat="server" Text="THÊM BÀI VIẾT" CssClass="button success" OnClientClick="Metro.activity.open({type:'cycle',overlayClickClose:false})" OnClick="button1_Click" />
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

