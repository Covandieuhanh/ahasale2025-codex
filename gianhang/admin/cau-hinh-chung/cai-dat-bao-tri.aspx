<%@ Page Title="Cài đặt bảo trì" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="cai-dat-bao-tri.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    
    <div id="main-content" class=" mb-10">
        <div class="row">
            <div class="cell-lg-6">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="mt-0">
                                <label class="fw-600">Trạng thái</label>
                                <asp:DropDownList ID="DropDownList1" CssClass="select-input select" runat="server">
                                    <asp:ListItem Text="Không bảo trì" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Bảo trì" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <asp:Panel ID="Panel2" runat="server">
                                <div class="mt-3">
                                    <label class="fw-600">Ngày bắt đầu</label>
                                    <asp:TextBox ID="txt_ngay_batdau" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                </div>
                                <div class="mt-3">
                                    <label class="fw-600">Thời gian bắt đầu</label>
                                    <div class="d-flex">
                                        <asp:DropDownList ID="ddl_giobatdau" runat="server" data-role="select" data-filter="flase" CssClass="mr-2"></asp:DropDownList>
                                        <asp:DropDownList ID="ddl_phutbatdau" runat="server" data-role="select" data-filter="flase" CssClass="ml-2"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="mt-3">
                                    <label class="fw-600">Ngày kết thúc</label>
                                    <asp:TextBox ID="txt_ngay_ketthuc" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                </div>
                                <div class="mt-3">
                                    <label class="fw-600">Thời gian kết thúc</label>
                                    <div class="d-flex">
                                        <asp:DropDownList ID="ddl_gioketthuc" runat="server" data-role="select" data-filter="flase" CssClass="mr-2"></asp:DropDownList>
                                        <asp:DropDownList ID="ddl_phutketthuc" runat="server" data-role="select" data-filter="flase" CssClass="ml-2"></asp:DropDownList>
                                    </div>
                                </div>
                            </asp:Panel>

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
    <script>
        (function () {
            function syncBaoTriState() {
                var select = document.getElementById("<%=DropDownList1.ClientID %>");
                var panel = document.getElementById("<%=Panel2.ClientID %>");
                if (!select || !panel) return;
                var enabled = select.value === "1";
                panel.style.opacity = enabled ? "1" : "0.55";
                var fields = panel.querySelectorAll("input, select, textarea, button");
                for (var i = 0; i < fields.length; i++) {
                    fields[i].disabled = !enabled;
                }
            }
            syncBaoTriState();
            var select = document.getElementById("<%=DropDownList1.ClientID %>");
            if (select) {
                select.addEventListener("change", syncBaoTriState);
            }
        })();
    </script>
</asp:Content>
