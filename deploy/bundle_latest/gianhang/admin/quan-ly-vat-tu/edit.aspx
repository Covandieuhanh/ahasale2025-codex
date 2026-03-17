<%@ Page Title="Sửa vật tư" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="edit.aspx.cs" Inherits="badmin_temp" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Register Namespace="CKFinder" Assembly="CKFinder" TagPrefix="CKFinder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <%if (bcorn_class.check_quyen(user, "q16_2") == "")
        { %>
    <div id='form_3' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_3()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Thêm phòng ban</h5>
                <hr />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel3" runat="server" DefaultButton="Button2">
                            <div class="mt-3">
                                <asp:TextBox ID="txt_tenphongban" runat="server" data-role="input" placeholder="Nhập tên phòng ban"></asp:TextBox><%--autocomplete="off" --%>
                            </div>
                            <div class="mt-6 mb-10 text-right">
                                <asp:Button ID="Button2" runat="server" Text="THÊM MỚI" CssClass="button success" OnClick="Button2_Click" />
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

            </div>

        </div>
    </div>
    <script>
        function show_hide_id_form_3() {
            var x = document.getElementById("form_3");
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
                        <a class="button" href="/gianhang/admin/quan-ly-vat-tu/Default.aspx"><span class="mif mif-arrow-left"></span></a></li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                </ul>
            </div>
            <div class="cell-6">
            </div>
        </div>


        <asp:Panel ID="Panel1" runat="server" DefaultButton="button1">
            <div class="row">
                <div class="cell-lg-6 pl-2-lg pr-2-lg mt-3">
                    <label class="fg-red fw-600">Tên vật tư</label>
                    <asp:TextBox ID="txt_name" runat="server" data-role="input"></asp:TextBox>
                </div>
                <div class="cell-lg-6 pl-2-lg pr-2-lg mt-3">
                    <label class="fw-600">Nhóm vật tư</label>
                    <asp:DropDownList ID="ddl_nhom" runat="server" data-role="select" data-filter="false"></asp:DropDownList>
                </div>
                <div class="cell-lg-6 pl-2-lg pr-2-lg mt-3">
                           
                                <label class="fw-600">Nhà cung cấp</label>
                                <asp:DropDownList ID="DropDownList3" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                        
                        </div>
                <div class="cell-lg-6 pl-2-lg pr-2-lg mt-3">
                    <label class="fw-600">Hình ảnh</label>
                    
                    <asp:FileUpload ID="FileUpload2" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                </div>
                <div class="cell-lg-6 pl-2-lg pr-2-lg mt-3">
                    <label class=" fw-600">Giá bán</label>
                    <asp:TextBox ID="txt_giasanpham" MaxLength="13" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox>
                </div>
                <div class="cell-lg-6 pl-2-lg pr-2-lg mt-3">
                    <label class=" fw-600">Giá nhập</label>
                    <asp:TextBox ID="txt_giavonsanpham" MaxLength="13" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox>
                </div>
                <div class="cell-lg-6 pl-2-lg pr-2-lg mt-3">
                    <label class=" fw-600">Đơn vị tính</label>
                    <asp:TextBox ID="txt_dvt_sp" runat="server" data-role="input"></asp:TextBox>
                </div>
                <div class="cell-lg-6 pl-2-lg pr-2-lg mt-3">
                    <label class="fw-600">Ghi chú</label>
                    <asp:TextBox ID="txt_ghichu" data-role="input" runat="server"></asp:TextBox>
                </div>
                <div class="cell-lg-6 pl-2-lg pr-2-lg mt-3">
                    <label class="fw-600">Tình trạng</label>
                    <asp:DropDownList ID="DropDownList1" runat="server">
                        <asp:ListItem Value="Thuê" Text="Thuê"></asp:ListItem>
                        <asp:ListItem Value="Mua" Text="Mua"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="cell-lg-6 pl-2-lg pr-2-lg mt-3">
                    <label class="fw-600">Vị trí phòng ban</label>
                    <div class="place-right">
                        <a class=" fg-red fg-orange-hover" href="#" onclick="show_hide_id_form_3()"><small>Thêm nhanh</small></a>/ 
                            <a class=" fg-red fg-orange-hover" href="/gianhang/admin/quan-ly-he-thong/phong-ban.aspx"><small>Quản lý</small></a>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />
                        </Triggers>
                        <ContentTemplate>
                            <asp:DropDownList ID="DropDownList2" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
            <div class="text-center mt-10">
                    <asp:Button ID="button1" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClientClick="Metro.activity.open({type:'cycle',overlayClickClose:false})" OnClick="button1_Click" />
                </div>
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

