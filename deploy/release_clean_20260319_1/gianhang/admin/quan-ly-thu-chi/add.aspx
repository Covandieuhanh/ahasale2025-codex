<%@ Page Title="Tạo thu chi" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="add.aspx.cs" Inherits="badmin_Default" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <%if (bcorn_class.check_quyen(user, "q9_5") == ""||bcorn_class.check_quyen(user, "n9_5") == "")
        { %>
    <div id='form_2' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_2()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Thêm loại thu chi</h5>
                <hr />
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel2" runat="server" DefaultButton="Button3">
                            <div class="row">
                                <div class="cell-lg-12">
                                    <div class="mt-3">
                                        <label class="fw-600">Ngành</label>
                                        <asp:DropDownList ID="DropDownList1" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Tên loại thu chi</label>
                                        <asp:TextBox ID="txt_tennhomdv" runat="server" data-role="input"></asp:TextBox><%--autocomplete="off" --%>
                                    </div>
                                </div>

                            </div>


                            <div class="mt-6 mb-10">
                                <div style="float: left">
                                    <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                </div>
                                <div style="float: right">
                                    <asp:Button ID="Button3" runat="server" Text="THÊM MỚI" CssClass="button success" OnClick="Button3_Click" />
                                </div>
                                <div style="clear: both"></div>
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

    <div id="main-content" class="mb-10">
        <div class="row border-bottom bd-light">
            <div class="cell-6">
                <ul class="h-menu">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Quay lại">
                        <a class="button" href="/gianhang/admin/quan-ly-thu-chi/Default.aspx"><span class="mif mif-arrow-left"></span></a></li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                </ul>
            </div>
            <div class="cell-6">
            </div>
        </div>
        <div class="row">
            <div class="cell-lg-6">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="but_form_themthuchi">
                    <div>
                        <div class="mt-3">
                            <label class="fw-600">Ngành</label>
                            <asp:DropDownList ID="DropDownList3" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                        </div>
                        <div class="mt-3">
                            <label class="fw-600">Ngày</label>
                            <asp:TextBox ID="txt_ngaylap" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                        </div>
                        <div class="mt-3">
                            <label class="fw-600">Thu/Chi</label>
                            <asp:DropDownList ID="ddl_loaiphieu" runat="server" data-role="select" data-filter="false">
                                <asp:ListItem Text="Phiếu thu" Value="Thu"></asp:ListItem>
                                <asp:ListItem Text="Phiếu chi" Value="Chi"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="mt-3">
                            <label class="fw-600">Loại thu chi</label>
                            <div class="place-right">
                                <a class=" fg-red fg-orange-hover" href="#" onclick="show_hide_id_form_2()"><small>Thêm nhanh</small></a> / 
                            <a class=" fg-red fg-orange-hover" href="/gianhang/admin/quan-ly-thu-chi/nhom-thu-chi.aspx"><small>Quản lý loại</small></a>
                            </div>
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:DropDownList ID="ddl_nhomtc" runat="server" data-role="select" data-filter="false"></asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>

                        <div class="mt-3">
                            <label class="fw-600">Số tiền</label>
                            <asp:TextBox ID="txt_sotien" MaxLength="13" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                        </div>
                        <div class="mt-3">
                            <label class="fw-600">Người nhận tiền</label>
                            <asp:DropDownList ID="ddl_nhanvien_nhantien" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                        </div>
                        <div class="mt-3">
                            <label class="fw-600">Nội dung</label>
                            <asp:TextBox ID="txt_noidung" runat="server" data-role="textarea" TextMode="MultiLine"></asp:TextBox><%--autocomplete="off" --%>
                        </div>
                        <div class="mt-3">
                            <label class="fw-600">Ảnh đính kèm</label>
                            <asp:FileUpload ID="FileUpload1" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="true" />
                        </div>
                        <div class="mt-6 mb-10">
                            <div style="float: left">
                                <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                            </div>
                            <div style="float: right">
                                <asp:Button ID="but_form_themthuchi" runat="server" Text="TẠO MỚI" CssClass="button success" OnClick="but_form_themthuchi_Click" />
                            </div>
                            <div style="clear: both"></div>
                        </div>
                    </div>

                </asp:Panel>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

