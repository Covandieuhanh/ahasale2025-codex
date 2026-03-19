<%@ Page Title="Chỉnh sửa ảnh slider" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="edit.aspx.cs" Inherits="admin_slider_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id="main-content" class=" mb-10">
        <div class="row border-bottom bd-light">
            <div class="cell-6">
                <ul class="h-menu">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Quay lại">
                        <a class="button" href="/gianhang/admin/quan-ly-module/slide-anh/default.aspx"><span class="mif mif-arrow-left"></span></a></li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                </ul>
            </div>
            <div class="cell-6">
            </div>
        </div>
        <div class="row">
            <div class="cell-lg-6">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="but_add">
                    <div class="mt-3">
                        <label class="fg-red fw-600">Hình ảnh</label>
                        <asp:FileUpload ID="FileUpload2" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                        <div>
                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                        </div>

                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Thứ tự hiển thị</label>
                        <span style="cursor: pointer" class="mif mif-question ml-1" data-role="popover" data-popover-text="<small>Ảnh nào có thứ tự hiển thị <b>nhỏ hơn</b> sẽ ưu tiên <b>hiển thị trước</b>. Chỉ chấp nhận số nguyên dương.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                        <asp:TextBox ID="txt_rank" runat="server" data-role="input"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Link liên kết</label>
                        <span style="cursor: pointer" class="mif mif-question ml-1" data-role="popover" data-popover-text="<small>Nếu bạn muốn ảnh này liên kết đến một trang khác thì hãy điền URL trang đó vào. Nếu không thì bỏ trống.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                        <asp:TextBox ID="txt_link1" runat="server" data-role="input" placeholder="Ví dụ: http://bcorn.net"></asp:TextBox>
                    </div>
                    <div class="mt-3">
                        <label class="fw-600">Tiêu đề liên kết</label>
                        <span style="cursor: pointer" class="mif mif-question ml-1" data-role="popover" data-popover-text="<small>Đây là nội dung sẽ hiển thị trên nút liên kết nếu bạn nhập Link liên kết cho ảnh.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                        <asp:TextBox ID="txt_title_link1" runat="server" data-role="input" placeholder="Ví dụ: Trang chủ"></asp:TextBox>
                    </div>
                    <div class="text-right-lg text-center mt-10">
                        <asp:Button OnClientClick="Metro.activity.open({type:'cycle',overlayClickClose:false})" ID="but_add" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="but_add_Click" />
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

