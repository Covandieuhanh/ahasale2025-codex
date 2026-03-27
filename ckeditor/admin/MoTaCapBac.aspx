<%@ Page Title="Mô tả cấp bậc" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master"
    AutoEventWireup="true" CodeFile="MoTaCapBac.aspx.cs" Inherits="admin_MoTaCapBac"
    ValidateRequest="false" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .mota-wrap {
            padding: 12px;
            width: 100%;
            max-width: 100%;
            overflow-x: hidden;
        }
        .mota-left {
            width: 360px;
            min-width: 300px;
            flex: 0 0 360px;
        }
        .mota-item {
            cursor: pointer;
            width: 100% !important;
            max-width: 100%;
            height: auto !important;
            min-height: 64px;
            padding: 10px 12px !important;
            white-space: normal !important;
            line-height: 1.35;
            overflow: hidden;
        }
        .mota-item.button,
        .mota-item.button.mini {
            height: auto !important;
            white-space: normal !important;
        }
        .mota-item.active { background: #fff3cd; }
        .mota-item .cap {
            display: block !important;
            width: 100%;
            font-weight: 700;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }
        .mota-item .desc {
            display: -webkit-box !important;
            width: 100%;
            font-size: 12px;
            opacity: .85;
            margin-top: 4px;
            white-space: normal !important;
            overflow: hidden;
            text-overflow: ellipsis;
            overflow-wrap: anywhere;
            word-break: break-word;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
        }
        .mota-card {
            background: #fff;
            border: 1px solid #e5e5e5;
            border-radius: 8px;
            min-width: 0;
            max-width: 100%;
        }
        .mota-header { padding: 10px 12px; border-bottom: 1px solid #eee; font-weight: 700; }
        .mota-body {
            padding: 12px;
            min-width: 0;
            overflow-wrap: anywhere;
        }
        .mota-actions { padding: 12px; border-top: 1px solid #eee; text-align: right; }
        .mota-muted { font-size: 12px; opacity: .75; }
        .mota-row {
            display: flex;
            gap: 12px;
            align-items: flex-start;
            min-width: 0;
            max-width: 100%;
        }
        .mota-editor {
            flex: 1;
            min-width: 0;
        }
        @media(max-width: 992px){
            .mota-row{ flex-direction: column; }
            .mota-left{
                width: 100%;
                min-width: 0;
                flex: 1 1 auto;
            }
            .mota-editor{
                width: 100%;
            }
        }
        .mota-tabs{ display:flex; gap:8px; margin: 8px 0 12px; flex-wrap:wrap; }
        .mota-tabs .button{ padding: 6px 10px; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="mota-wrap">
                <div class="mota-row">

                    <!-- LEFT: danh sách -->
                    <div class="mota-left mota-card">
                        <div class="mota-header">
                            Danh sách cấp bậc
                            <div class="mota-muted mt-1">Nhấp vào 1 dòng để sửa</div>
                        </div>
                        <div class="mota-body">
                            <asp:Repeater ID="rp_list" runat="server" OnItemDataBound="rp_list_ItemDataBound">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lk_select" runat="server"
                                        CssClass="button mini light w-100 text-left mota-item"
                                        OnClick="lk_select_Click"
                                        CommandArgument='<%# Eval("id") %>'>
                                        <div class="cap"><%# Eval("Capbac") %></div>
                                        <div class="desc"><%# Eval("MoTaPreview") %></div>
                                    </asp:LinkButton>

                                    <div class="mb-2"></div>
                                </ItemTemplate>
                            </asp:Repeater>

                            <asp:Label ID="lb_empty" runat="server" Text="Không có dữ liệu." Visible="false"></asp:Label>
                        </div>
                    </div>

                    <!-- RIGHT: editor -->
                    <div class="mota-card mota-editor">
                        <div class="mota-header">
                            Chỉnh sửa nội dung
                        </div>

                        <div class="mota-body">
                            <asp:Panel ID="pn_editor" runat="server" Visible="false">
                                <div class="mb-2">
                                    <span class="mota-muted">Cấp bậc đang sửa:</span>
                                    <span style="font-weight:700;">
                                        <asp:Label ID="lb_capbac" runat="server"></asp:Label>
                                    </span>
                                </div>

                                <!-- Tabs chọn nội dung đang sửa -->
                                <div class="mota-tabs">
                                    <asp:LinkButton ID="tab_mota" runat="server" CssClass="button mini light"
                                        OnClick="tab_mota_Click">Mô tả</asp:LinkButton>
                                    <asp:LinkButton ID="tab_trachnhiem" runat="server" CssClass="button mini light"
                                        OnClick="tab_trachnhiem_Click">Trách nhiệm</asp:LinkButton>
                                </div>

                                <!-- CKEditor: Mô tả -->
                                <asp:Panel ID="pn_ck_mota" runat="server">
                                    <div class="mb-2">
                                        <CKEditor:CKEditorControl ID="ck_mota" runat="server" Height="320" />
                                    </div>
                                </asp:Panel>

                                <!-- CKEditor: Trách nhiệm (TÊN TRƯỜNG: TrachNhiem) -->
                                <asp:Panel ID="pn_ck_trachnhiem" runat="server" Visible="false">
                                    <div class="mb-2">
                                        <CKEditor:CKEditorControl ID="TrachNhiem" runat="server" Height="320" />
                                    </div>
                                </asp:Panel>

                                <asp:Label ID="lb_msg" runat="server" EnableViewState="false"></asp:Label>
                            </asp:Panel>

                            <asp:Panel ID="pn_hint" runat="server" Visible="true">
                                <div class="mota-muted">
                                    Hãy chọn một cấp bậc ở danh sách bên trái để bắt đầu chỉnh sửa.
                                </div>
                            </asp:Panel>
                        </div>

                        <div class="mota-actions">
                            <asp:Button ID="but_save" runat="server" Text="Lưu" CssClass="button success"
                                OnClick="but_save_Click" Enabled="false" />
                            <asp:Button ID="but_reload" runat="server" Text="Tải lại" CssClass="button light"
                                OnClick="but_reload_Click" />
                        </div>
                    </div>

                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="up_prog" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.8; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true">
                        <span class="electron"></span><span class="electron"></span><span class="electron"></span>
                    </div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
</asp:Content>
