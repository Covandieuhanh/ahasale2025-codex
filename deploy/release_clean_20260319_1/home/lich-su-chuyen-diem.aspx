<%@ Page Title="Lịch sử chuyển điểm" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="lich-su-chuyen-diem.aspx.cs" Inherits="home_lich_su_chuyen_diem" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .tblr-overlay{
            position:fixed; inset:0; background:rgba(0,0,0,.65);
            z-index:99999; display:flex; align-items:center; justify-content:center;
        }
        .table thead th{ white-space:nowrap; }
        .table td{ vertical-align:middle; }
        .modal-body{ max-height: calc(100vh - 220px); overflow:auto; }
        .money{ white-space:nowrap; }
        .a-coin{ width:18px; height:18px; object-fit:contain; vertical-align:-3px; }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">

    <!-- ===================== MODAL: CHUYỂN ĐIỂM ===================== -->
    <asp:UpdatePanel ID="up_add" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_add" runat="server" Visible="false" DefaultButton="but_add_edit">

                <div class="modal modal-blur show" style="display:block;" tabindex="-1" role="dialog" aria-modal="true">
                    <div class="modal-dialog modal-dialog-centered" role="document" style="max-width: 680px;">
                        <div class="modal-content">

                            <div class="modal-header">
                                <h5 class="modal-title">Chuyển điểm</h5>
                                <a href="#" class="btn-close" aria-label="Close" id="close_add" runat="server" onserverclick="but_close_form_add_Click"></a>
                            </div>

                            <div class="modal-body">
                                <div class="mb-3">
                                    <label class="form-label">Tài khoản nhận Quyền tiêu dùng</label>
                                    <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-select"></asp:DropDownList>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Số Quyền tiêu dùng muốn chuyển</label>
                                    <asp:TextBox ID="txt_dongA_chuyen" runat="server" CssClass="form-control" placeholder="Ví dụ: 10 hoặc 10.5"></asp:TextBox>
                                    <div class="form-hint">Nhập số dương. Có thể có số lẻ.</div>
                                </div>
                            </div>

                            <div class="modal-footer">
                                <asp:Button ID="but_add_edit" runat="server" Text="XÁC NHẬN CHUYỂN ĐIỂM"
                                    CssClass="btn btn-primary" OnClick="but_add_edit_Click" />
                            </div>

                        </div>
                    </div>
                </div>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="up_add">
        <ProgressTemplate>
            <div class="tblr-overlay">
                <div class="text-center">
                    <div class="spinner-border" role="status"></div>
                    <div class="mt-3 text-white">Đang xử lý...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>


    <!-- ===================== MAIN ===================== -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="page-header d-print-none">
                <div class="container-xl" style="max-width: 992px;">
                    <div class="row g-2 align-items-center">
                        <div class="col">
                            <div class="page-pretitle">Ví / Quyền tiêu dùng</div>
                            <h2 class="page-title">Lịch sử chuyển điểm</h2>
                        </div>
                        <div class="col-auto ms-auto d-print-none">
                            <asp:LinkButton ID="but_show_form_add" OnClick="but_show_form_add_Click" runat="server"
                                CssClass="btn btn-primary btn-sm">
                                <i class="ti ti-transfer"></i>&nbsp;Chuyển điểm
                            </asp:LinkButton>
                        </div>
                    </div>

                    <div class="row g-2 align-items-center mt-3">
                        <div class="col-12">
                            <div class="d-flex align-items-center gap-2 flex-wrap">
                                <div class="text-muted small">
                                    <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                                    <asp:Label ID="lb_show_md" runat="server" Text="" CssClass="d-none"></asp:Label>
                                </div>

                                <div class="ms-auto btn-list">
                                    <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server"
                                        CssClass="btn btn-outline-secondary btn-sm">
                                        <i class="ti ti-chevron-left"></i>&nbsp;Lùi
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server"
                                        CssClass="btn btn-outline-secondary btn-sm">
                                        Tới&nbsp;<i class="ti ti-chevron-right"></i>
                                    </asp:LinkButton>

                                    <!-- giữ lại bản mobile cũ để không đụng code -->
                                    <div class="d-none">
                                        <asp:LinkButton ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" />
                                        <asp:LinkButton ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- TextBox tìm kiếm cũ không dùng (logic đã comment), nhưng giữ control để không lỗi -->
                        <div class="d-none">
                            <asp:TextBox ID="txt_timkiem" runat="server" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                            <asp:TextBox ID="txt_timkiem1" runat="server" AutoPostBack="true" OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                        </div>

                    </div>

                </div>
            </div>

            <div class="page-body">
                <div class="container-xl" style="max-width: 992px;">

                    <div class="card">
                        <div class="table-responsive">
                            <table class="table card-table table-vcenter">
                                <thead>
                                    <tr>
                                        <th style="width:1px;">ID</th>
                                        <th style="min-width:140px;" class="text-center">Ngày</th>
                                        <th style="min-width:180px;" class="text-center">Người chuyển</th>
                                        <th style="min-width:180px;" class="text-center">Người nhận</th>
                                        <th style="min-width:120px;" class="text-center">Quyền tiêu dùng</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <span style="display:none">
                                                <asp:Label ID="lbID" runat="server" Text='<%#Eval("id") %>'></asp:Label>
                                            </span>

                                            <tr>
                                                <td class="text-center"><%# Eval("id") %></td>

                                                <td class="text-center">
                                                    <%#Eval("ngay","{0:dd/MM/yyyy}") %>
                                                </td>

                                                <td class="text-center">
                                                    <span class="fw-semibold"><%# Eval("taikhoan_chuyen") %></span>
                                                </td>

                                                <td class="text-center">
                                                    <span class="fw-semibold"><%# Eval("taikhoan_nhan") %></span>
                                                </td>

                                                <td class="text-center money">
                                                    <img class="a-coin" src="/uploads/images/dong-a.png" alt="A" />
                                                    <span class="badge bg-azure-lt text-azure ms-1">
                                                        <%#Eval("dongA","{0:#,##0.##}") %>
                                                    </span>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>

                            </table>
                        </div>
                    </div>

                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="tblr-overlay">
                <div class="text-center">
                    <div class="spinner-border" role="status"></div>
                    <div class="mt-3 text-white">Đang tải...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="ContentFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="Server">
</asp:Content>
