<%@ Page Title="Thêm menu" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="add.aspx.cs" Inherits="badmin_temp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id="main-content" class=" mb-10">
        <div class="row border-bottom bd-light">
            <div class="cell-6">
                <ul class="h-menu">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Quay lại">
                        <a class="button" href="/gianhang/admin/quan-ly-menu/Default.aspx"><span class="mif mif-arrow-left"></span></a></li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                </ul>
            </div>
            <div class="cell-6">
            </div>
        </div>
        <div class="row">
            <div class="cell-lg-6">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="button1">
                    <div class="mt-3">
                        <label class="fg-red fw-600">Tên menu</label>
                        <asp:TextBox ID="txt_name" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class=" fw-600">Phân loại hiển thị</label>
                        <asp:DropDownList ID="DropDownList1" runat="server" data-role="select" data-filter="false">
                            <asp:ListItem Text="Tin tức" Value="dsbv"></asp:ListItem>
                            <asp:ListItem Text="Sản phẩm" Value="dssp"></asp:ListItem>
                            <asp:ListItem Text="Dịch vụ" Value="dsdv"></asp:ListItem>
                            
                        </asp:DropDownList>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Menu cha</label>
                        <span style="cursor: pointer" class="mif mif-question ml-1" data-role="popover" data-popover-text="<small>Nếu Menu bạn sắp thêm là Menu cấp 1 thì không chọn Menu cha.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                        <select name="listmenu" data-role="select">
                            <option value="0">Nhấn để chọn</option>
                            <%=result_listview %>
                        </select>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Mô tả ngắn</label>
                        <asp:TextBox ID="txt_description" data-role="textarea" runat="server" TextMode="MultiLine"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Thứ tự hiển thị</label>
                        <span style="cursor: pointer" class="mif mif-question ml-1" data-role="popover" data-popover-text="<small>Menu nào có thứ tự hiển thị <b>nhỏ hơn</b> sẽ ưu tiên <b>hiển thị trước</b>. Chỉ chấp nhận số nguyên dương.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                        <asp:TextBox ID="txt_rank" runat="server" data-role="input"></asp:TextBox>
                    </div>

                    <div class="mt-3">
                        <label class="fw-600">Đường dẫn khác</label>
                        <span style="cursor: pointer" class="mif mif-question ml-1" data-role="popover" data-popover-text="<small>Nếu bạn muốn Menu này liên kết với một trang khác thì hãy điền URL đó vào. Nếu không, hãy để trống ô này.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                        <asp:TextBox ID="txt_url" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Ảnh thu nhỏ</label>
                        <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Đây là ảnh sẽ hiển thị khi bạn chia sẻ link Website của bạn qua mạng xã hội.<br />Kích thước chuẩn: 1200x628 pixel.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                        <asp:FileUpload ID="FileUpload2" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                    </div>
  
                    <div class="text-right-lg text-center mt-10">
                        <asp:Button ID="button1" runat="server" Text="THÊM MENU" CssClass="button success" OnClientClick="Metro.activity.open({type:'cycle',overlayClickClose:false})" OnClick="button1_Click" />
                    </div>
                </asp:Panel>
            </div>


        </div>
    </div>



</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

