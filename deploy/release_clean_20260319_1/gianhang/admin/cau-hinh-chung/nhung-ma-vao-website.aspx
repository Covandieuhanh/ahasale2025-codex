<%@ Page Title="Nhúng mã vào website" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="nhung-ma-vao-website.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">


    <div id="main-content" class="mb-10">
        <div class="row">
            <div class="cell-lg-12">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="mt-0">
                                <label class="fw-600">Nhúng mã Plugin Fanpage Facebook</label>
                                <asp:TextBox ID="txt_code_page" data-role="textarea" runat="server" TextMode="MultiLine" CssClass="max-height-code"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Nhúng mã Google Maps</label>
                                <asp:TextBox ID="txt_code_maps" data-role="textarea" runat="server" TextMode="MultiLine" CssClass="max-height-code"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Nhúng mã vào trong thẻ &lt;head&gt;&lt;/head&gt; </label>
                                <asp:TextBox ID="txt_code_head" data-role="textarea" runat="server" TextMode="MultiLine" CssClass="max-height-code"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Nhúng mã vào ngay sau thẻ &lt;body&gt; </label>
                                <asp:TextBox ID="txt_code_body1" data-role="textarea" runat="server" TextMode="MultiLine" CssClass="max-height-code"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Nhúng mã vào ngay trước thẻ &lt;/body&gt; </label>
                                <asp:TextBox ID="txt_code_body2" data-role="textarea" runat="server" TextMode="MultiLine" CssClass="max-height-code"></asp:TextBox>
                            </div>
                            <div class="text-center mt-10">
                                <asp:Button ID="Button1" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="Button1_Click" />
                            </div>
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
                </asp:Panel>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

