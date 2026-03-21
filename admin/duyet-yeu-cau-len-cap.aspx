<%@ Page Title="Duyệt yêu cầu xác nhận hành vi" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master"
    AutoEventWireup="true" CodeFile="duyet-yeu-cau-len-cap.aspx.cs"
    Inherits="admin_duyet_yeu_cau_len_cap" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .aha-admin-action-group,
        .aha-admin-action-group * {
            pointer-events: auto !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <script>
        (function () {
            function unlockFilterControls() {
                var txt = document.getElementById("<%= txt_timkiem.ClientID %>");
                var ddl = document.getElementById("<%= ddl_trangthai.ClientID %>");

                if (txt) {
                    txt.disabled = false;
                    txt.readOnly = false;
                }
                if (ddl) {
                    ddl.disabled = false;
                }

            }

            document.addEventListener("DOMContentLoaded", function () {
                window.setTimeout(unlockFilterControls, 30);
            });

            if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    window.setTimeout(unlockFilterControls, 30);
                });
            }
        })();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="aha-admin-screen p-3">
                <div class="aha-admin-head">
                    <div>
                        <h4>DUYỆT YÊU CẦU XÁC NHẬN HÀNH VI</h4>
                        <div class="aha-admin-meta">Tập trung xử lý theo trạng thái, tìm nhanh theo tài khoản hoặc ID.</div>
                    </div>
                    <div class="aha-admin-summary">
                        <div class="aha-admin-kpi">Tổng: <span class="value"><asp:Label ID="lb_tong" runat="server" Text="0"></asp:Label></span></div>
                        <div class="aha-admin-kpi">Chờ duyệt: <span class="value"><asp:Label ID="lb_cho_duyet" runat="server" Text="0"></asp:Label></span></div>
                        <div class="aha-admin-kpi">Đã duyệt: <span class="value"><asp:Label ID="lb_da_duyet" runat="server" Text="0"></asp:Label></span></div>
                        <div class="aha-admin-kpi">Từ chối: <span class="value"><asp:Label ID="lb_tu_choi" runat="server" Text="0"></asp:Label></span></div>
                    </div>
                </div>

                <div class="aha-admin-control-card">
                    <div class="aha-admin-control-row">
                        <div class="aha-admin-search-wrap">
                            <span class="mif mif-search"></span>
                            <asp:TextBox
                                ID="txt_timkiem"
                                runat="server"
                                MaxLength="80"
                                CssClass="aha-admin-native-input"
                                placeholder="Nhập ID hoặc tài khoản"
                                AutoPostBack="false"
                                data-enter-click="btn_timkiem"></asp:TextBox>
                        </div>

                        <asp:DropDownList
                            ID="ddl_trangthai"
                            runat="server"
                            CssClass="aha-admin-native-select"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddl_trangthai_SelectedIndexChanged">
                            <asp:ListItem Value="" Text="Tất cả trạng thái"></asp:ListItem>
                            <asp:ListItem Value="0" Text="Chờ duyệt"></asp:ListItem>
                            <asp:ListItem Value="1" Text="Đã duyệt"></asp:ListItem>
                            <asp:ListItem Value="2" Text="Từ chối"></asp:ListItem>
                        </asp:DropDownList>

                        <div class="aha-admin-control-actions">
                            <asp:LinkButton ID="btn_timkiem" ClientIDMode="Static" runat="server" CssClass="button dark" OnClick="btn_timkiem_Click">
                                <span class="mif-search"></span> Tìm
                            </asp:LinkButton>
                            <asp:LinkButton ID="btn_reset" runat="server" CssClass="button light" OnClick="btn_reset_Click">
                                <span class="mif-loop2"></span> Làm mới
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>

                <div class="bcorn-fix-title-table-container aha-admin-grid">
                    <table class="bcorn-fix-title-table">
                        <thead>
                            <tr>
                                <th style="min-width: 60px;">ID</th>
                                <th style="min-width: 140px;">Tài khoản</th>
                                <th style="min-width: 170px;">Hiện tại</th>
                                <th style="min-width: 170px;">Yêu cầu</th>
                                <th style="min-width: 160px;">Ngày tạo</th>
                                <th style="min-width: 120px;">Trạng thái</th>
                                <th style="min-width: 180px;" class="text-center">Thao tác</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rp_yeucau" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="text-center"><%# Eval("ID") %></td>
                                        <td class="fw-600"><%# Eval("TaiKhoan") %></td>
                                        <td>
                                            Cấp <%# Eval("CapHienTai") %> - <%# Eval("GiaTriHienTai") %>
                                        </td>
                                        <td class="fg-green fw-600">
                                            Cấp <%# Eval("CapYeuCau") %> - <%# Eval("GiaTriYeuCau") %>
                                        </td>
                                        <td>
                                            <%# Eval("NgayTao","{0:dd/MM/yyyy HH:mm}") %>
                                        </td>
                                        <td>
                                            <asp:PlaceHolder runat="server" Visible='<%# Eval("TrangThai").ToString()=="0" %>'>
                                                <span class="aha-admin-status pending">Chờ duyệt</span>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder runat="server" Visible='<%# Eval("TrangThai").ToString()=="1" %>'>
                                                <span class="aha-admin-status approved">Đã duyệt</span>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder runat="server" Visible='<%# Eval("TrangThai").ToString()=="2" %>'>
                                                <span class="aha-admin-status rejected">Từ chối</span>
                                            </asp:PlaceHolder>
                                        </td>
                                        <td class="text-center">
                                            <div class="aha-admin-action-group">
                                                <asp:Button
                                                    ID="btn_duyet"
                                                    runat="server"
                                                    CssClass="button success small"
                                                    Text="Duyệt"
                                                    UseSubmitBehavior="false"
                                                    OnClientClick="this.disabled=true;this.value='Đang duyệt...';"
                                                    CommandArgument='<%# Eval("ID") %>'
                                                    OnClick="btn_duyet_Click"
                                                    Visible='<%# Eval("CanAction").ToString()=="True" %>' />

                                                <asp:Button
                                                    ID="btn_tuchoi"
                                                    runat="server"
                                                    CssClass="button alert small"
                                                    Text="Từ chối"
                                                    UseSubmitBehavior="false"
                                                    OnClientClick="this.disabled=true;this.value='Đang xử lý...';"
                                                    CommandArgument='<%# Eval("ID") %>'
                                                    OnClick="btn_tuchoi_Click"
                                                    Visible='<%# Eval("CanAction").ToString()=="True" %>' />
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>

                <asp:Panel ID="pn_empty" runat="server" Visible="false" CssClass="aha-admin-control-card text-center fg-gray">
                    Không có yêu cầu nào phù hợp bộ lọc hiện tại.
                </asp:Panel>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
