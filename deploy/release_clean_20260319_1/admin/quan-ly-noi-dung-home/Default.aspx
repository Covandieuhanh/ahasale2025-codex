<%@ Page Title="Quản lý nội dung Home" ValidateRequest="false" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_quan_ly_noi_dung_home_Default" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="Server">
    <style>
        .home-content-admin .box-card {
            border: 1px solid #e5e7eb;
            border-radius: 10px;
            background: #fff;
            padding: 12px;
        }

        .home-content-admin .box-title {
            font-size: 17px;
            font-weight: 700;
            margin-bottom: 6px;
        }

        .home-content-admin .muted {
            color: #64748b;
            font-size: 12px;
        }

        .home-content-admin .table td,
        .home-content-admin .table th {
            vertical-align: middle;
            font-size: 13px;
        }

        .home-content-admin textarea {
            min-height: 160px;
            width: 100%;
            border: 1px solid #cbd5e1;
            border-radius: 6px;
            padding: 10px;
            font-size: 13px;
            line-height: 1.5;
        }

        .home-content-admin .action-row {
            display: flex;
            gap: 8px;
            justify-content: flex-end;
            align-items: center;
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="home-content-admin p-3">
                <div class="mb-3">
                    <div class="box-title">Quản lý nội dung text trang Home (không cần code)</div>
                    <div class="muted">
                        Quyền truy cập: <b>q3_1</b>. Người phụ trách chỉ cần nhập văn bản và link, không cần HTML.
                    </div>
                </div>

                <div class="row">
                    <div class="cell-lg-7 mb-4">
                        <div class="box-card">
                            <div class="d-flex flex-justify-between flex-align-center mb-2">
                                <div class="box-title m-0">Bài viết Footer</div>
                                <asp:Button ID="but_footer_new" runat="server" Text="Thêm link mới" CssClass="button primary rounded" OnClick="but_footer_new_Click" />
                            </div>
                            <div class="muted mb-3">
                                Các mục này hiển thị ở footer và có thể dẫn đến bài viết chi tiết hoặc link ngoài.
                            </div>

                            <div class="table-container">
                                <table class="table striped border bordered">
                                    <thead>
                                        <tr>
                                            <th>Nhóm</th>
                                            <th>Tên hiển thị</th>
                                            <th>Link đích</th>
                                            <th>Bật</th>
                                            <th>Sửa</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rpt_footer_rows" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# Eval("GroupLabel") %></td>
                                                    <td><%# Eval("DisplayName") %></td>
                                                    <td>
                                                        <a href="<%# Eval("ResolvedUrl") %>" target="_blank"><%# Eval("ResolvedUrl") %></a>
                                                    </td>
                                                    <td><%# Convert.ToBoolean(Eval("IsEnabled")) ? "Có" : "Tắt" %></td>
                                                    <td>
                                                        <asp:LinkButton ID="but_select_footer" runat="server" CssClass="button small rounded"
                                                            CommandArgument='<%# Eval("ContentKey") %>'
                                                            OnClick="but_select_footer_Click">Chọn</asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div class="cell-lg-5 mb-4">
                        <div class="box-card">
                            <div class="box-title">Sửa bài viết Footer</div>
                            <asp:HiddenField ID="hf_footer_content_key" runat="server" />

                            <div class="mb-2">
                                <label class="fw-600">Mã nội dung (tự sinh)</label>
                                <asp:TextBox ID="txt_footer_content_key" runat="server" data-role="input" ReadOnly="true"></asp:TextBox>
                            </div>

                            <div class="mb-2">
                                <label class="fw-600">Nhóm hiển thị</label>
                                <asp:DropDownList ID="ddl_footer_group" runat="server" data-role="select">
                                    <asp:ListItem Text="Hỗ trợ khách hàng" Value="footer.support"></asp:ListItem>
                                    <asp:ListItem Text="Về AhaSale" Value="footer.about"></asp:ListItem>
                                </asp:DropDownList>
                            </div>

                            <div class="mb-2">
                                <label class="fw-600">Tên hiển thị ngoài footer</label>
                                <asp:TextBox ID="txt_footer_display_name" runat="server" data-role="input" MaxLength="250"></asp:TextBox>
                            </div>

                            <div class="mb-2">
                                <label class="fw-600">Đường dẫn bài viết (slug)</label>
                                <asp:TextBox ID="txt_footer_slug" runat="server" data-role="input" MaxLength="180" Placeholder="vd: quy-che-hoat-dong-san"></asp:TextBox>
                            </div>

                            <div class="mb-2">
                                <label class="fw-600">Link đích tùy chỉnh (không bắt buộc)</label>
                                <asp:TextBox ID="txt_footer_target_url" runat="server" data-role="input" MaxLength="500" Placeholder="Để trống để mở bài viết nội bộ"></asp:TextBox>
                            </div>

                            <div class="mb-2">
                                <label class="fw-600">Nội dung bài viết</label>
                                <asp:TextBox ID="txt_footer_body_content" runat="server" TextMode="MultiLine"></asp:TextBox>
                            </div>

                            <div class="mb-2">
                                <label class="fw-600">Thứ tự hiển thị</label>
                                <asp:TextBox ID="txt_footer_sort_order" runat="server" data-role="input" MaxLength="4" Text="0"></asp:TextBox>
                            </div>

                            <div class="mb-2">
                                <asp:CheckBox ID="chk_footer_enabled" runat="server" Text="Bật hiển thị ngoài footer" Checked="true" />
                            </div>

                            <div class="muted mb-2">
                                <asp:Literal ID="lit_footer_updated_info" runat="server"></asp:Literal>
                            </div>

                            <div class="action-row">
                                <asp:Button ID="but_footer_save" runat="server" Text="Lưu bài viết Footer" CssClass="button success rounded" OnClick="but_footer_save_Click" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="cell-lg-5 mb-4">
                        <div class="box-card">
                            <div class="box-title">Nội dung text trang chủ</div>
                            <div class="muted mb-3">Các khối chữ chính trên trang chủ Home.</div>
                            <asp:Repeater ID="rpt_text_blocks" runat="server">
                                <ItemTemplate>
                                    <div class="mb-2 p-2 border rounded">
                                        <div class="fw-600"><%# Eval("Title") %></div>
                                        <div class="muted">key: <%# Eval("Key") %></div>
                                        <div class="mt-1">
                                            <asp:LinkButton ID="but_select_text_block" runat="server" CssClass="button small rounded"
                                                CommandArgument='<%# Eval("Key") %>'
                                                OnClick="but_select_text_block_Click">Sửa nội dung</asp:LinkButton>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                    <div class="cell-lg-7 mb-4">
                        <div class="box-card">
                            <div class="box-title">Sửa text khối trang chủ</div>
                            <asp:HiddenField ID="hf_text_content_key" runat="server" />

                            <div class="mb-2">
                                <label class="fw-600">Mã khối</label>
                                <asp:TextBox ID="txt_text_content_key" runat="server" data-role="input" ReadOnly="true"></asp:TextBox>
                            </div>

                            <div class="mb-2">
                                <label class="fw-600">Tiêu đề hiển thị</label>
                                <asp:TextBox ID="txt_text_title" runat="server" data-role="input"></asp:TextBox>
                            </div>

                            <div class="mb-2">
                                <label class="fw-600">Nội dung text</label>
                                <asp:TextBox ID="txt_text_content" runat="server" TextMode="MultiLine"></asp:TextBox>
                            </div>

                            <div class="mb-2">
                                <asp:CheckBox ID="chk_text_enabled" runat="server" Text="Bật hiển thị khối này" Checked="true" />
                            </div>

                            <div class="muted mb-2">
                                <asp:Literal ID="lit_text_guide" runat="server"></asp:Literal>
                            </div>

                            <div class="muted mb-2">
                                <asp:Literal ID="lit_text_updated_info" runat="server"></asp:Literal>
                            </div>

                            <div class="action-row">
                                <asp:Button ID="but_text_save" runat="server" Text="Lưu nội dung text" CssClass="button success rounded" OnClick="but_text_save_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="ContentFoot" ContentPlaceHolderID="foot" runat="Server"></asp:Content>
