<%@ Page Title="Cài đặt chung" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="badmin_Default" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

   
    <%--NỘI DUNG CHÍNH--%>

    <div id="main-content" class="mb-10">
        <div class="row">
            <div class="cell-lg-6 mt-5">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <%--<asp:AsyncPostBackTrigger ControlID="but_loc" EventName="Click" />--%>
                    </Triggers>
                    <ContentTemplate>
                        <div data-role="panel"
                            data-title-caption="Chỉ tiêu doanh số"
                            data-title-icon="<span class='mif-chart-bars2'></span>"
                            data-collapsible="true">
                            <div class="bg-white p-2">
                                <asp:Panel ID="Panel2" runat="server" DefaultButton="but_update_chitieu_doanhso">
                                    <div class="mt-0">
                                        <label>Dịch vụ</label>
                                        <div>
                                            <asp:TextBox ID="txt_chitieu_doanhso_dichvu" MaxLength="15" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label>Sản phẩm</label>
                                        <div>
                                            <asp:TextBox ID="txt_chitieu_doanhso_mypham" MaxLength="15" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox><%--autocomplete="off" --%>
                                        </div>
                                    </div>
                                    <div class="text-right mt-3">
                                        <asp:Button ID="but_update_chitieu_doanhso" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="but_update_chitieu_doanhso_Click" />
                                    </div>
                                </asp:Panel>
                            </div>
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
            </div>
            

        </div>
    </div>

    <%--END DUNG CHÍNH--%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>

