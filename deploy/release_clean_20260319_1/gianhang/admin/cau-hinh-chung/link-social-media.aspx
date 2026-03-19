<%@ Page Title="Link social media" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="link-social-media.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
            <div class="cell-lg-7">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="mt-0">
                                <label class="fw-600">Facebook</label>
                                <asp:TextBox ID="TextBox1" runat="server" data-role="input"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Zalo</label>
                                <asp:TextBox ID="TextBox2" runat="server" data-role="input"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Youtube</label>
                                <asp:TextBox ID="TextBox3" runat="server" data-role="input"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Instagram</label>
                                <asp:TextBox ID="TextBox4" runat="server" data-role="input"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Twitter</label>
                                <asp:TextBox ID="TextBox5" runat="server" data-role="input"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">TikTok</label>
                                <asp:TextBox ID="TextBox6" runat="server" data-role="input"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">WeChat</label>
                                <asp:TextBox ID="TextBox7" runat="server" data-role="input"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">linkedIn</label>
                                <asp:TextBox ID="TextBox8" runat="server" data-role="input"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">WhatsApp</label>
                                <asp:TextBox ID="TextBox9" runat="server" data-role="input"></asp:TextBox>
                            </div>
                            <div class="text-right-lg text-center mt-10">
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

