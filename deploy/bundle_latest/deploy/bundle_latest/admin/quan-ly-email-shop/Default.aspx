<%@ Page Title="Nội dung email thông báo shop" ValidateRequest="false" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_quan_ly_email_shop_Default" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="Server">
    <style>
        .shop-email-admin .box-card {
            border: 1px solid #e5e7eb;
            border-radius: 10px;
            background: #fff;
            padding: 12px;
        }

        .shop-email-admin .box-title {
            font-size: 17px;
            font-weight: 700;
            margin-bottom: 6px;
        }

        .shop-email-admin .muted {
            color: #64748b;
            font-size: 12px;
        }

        .shop-email-admin .table td,
        .shop-email-admin .table th {
            vertical-align: middle;
            font-size: 13px;
        }

        .shop-email-admin textarea {
            min-height: 180px;
            width: 100%;
            border: 1px solid #cbd5e1;
            border-radius: 6px;
            padding: 10px;
            font-size: 13px;
            line-height: 1.5;
        }

        .shop-email-admin .action-row {
            display: flex;
            gap: 8px;
            justify-content: flex-end;
            align-items: center;
        }

        .shop-email-admin .badge {
            display: inline-block;
            font-size: 11px;
            padding: 2px 8px;
            border-radius: 999px;
            background: #e2e8f0;
            color: #0f172a;
        }

        .shop-email-admin .badge.active {
            background: #d1fae5;
            color: #065f46;
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="shop-email-admin p-3">
                <div class="mb-3">
                    <div class="box-title">Nội dung email thông báo cho gian hàng đối tác</div>
                    <div class="muted">
                        Chỉ nhập văn bản thuần (không dùng HTML). Nội dung ở đây sẽ dùng để gửi email từ hệ thống AhaSale đến các shop.
                    </div>
                </div>

                <div class="row">
                    <div class="cell-lg-5 mb-4">
                        <div class="box-card">
                            <div class="box-title">Danh sách template</div>
                            <div class="muted mb-3">Chọn 1 template để chỉnh sửa nội dung.</div>
                            <div class="table-container">
                                <table class="table striped border bordered">
                                    <thead>
                                        <tr>
                                            <th>Mã</th>
                                            <th>Tên hiển thị</th>
                                            <th>Trạng thái</th>
                                            <th>Sửa</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rpt_templates" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# Eval("Code") %></td>
                                                    <td><%# Eval("Name") %></td>
                                                    <td>
                                                        <span class='badge <%# Convert.ToBoolean(Eval("IsActive")) ? "active" : "" %>'>
                                                            <%# Convert.ToBoolean(Eval("IsActive")) ? "Đang bật" : "Tắt" %>
                                                        </span>
                                                    </td>
                                                    <td>
                                                        <asp:LinkButton ID="but_select_template" runat="server" CssClass="button small rounded"
                                                            CommandArgument='<%# Eval("Code") %>'
                                                            OnClick="but_select_template_Click">Chọn</asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div class="cell-lg-7 mb-4">
                        <div class="box-card">
                            <div class="box-title">Chỉnh sửa template</div>
                            <asp:HiddenField ID="hf_template_code" runat="server" />

                            <div class="mb-2">
                                <label class="fw-600">Mã template</label>
                                <asp:TextBox ID="txt_template_code" runat="server" data-role="input" ReadOnly="true"></asp:TextBox>
                            </div>

                            <div class="mb-2">
                                <label class="fw-600">Tên hiển thị</label>
                                <asp:TextBox ID="txt_template_name" runat="server" data-role="input" ReadOnly="true"></asp:TextBox>
                            </div>

                            <div class="mb-2">
                                <label class="fw-600">Tiêu đề email</label>
                                <asp:TextBox ID="txt_template_subject" runat="server" data-role="input" MaxLength="250"></asp:TextBox>
                            </div>

                            <div class="mb-2">
                                <label class="fw-600">Nội dung email (text)</label>
                                <asp:TextBox ID="txt_template_body" runat="server" TextMode="MultiLine"></asp:TextBox>
                            </div>

                            <div class="mb-2">
                                <asp:CheckBox ID="chk_template_active" runat="server" Text="Bật gửi email cho template này" Checked="true" />
                            </div>

                            <div class="mb-3 p-2 border rounded">
                                <div class="fw-600">Biến hỗ trợ</div>
                                <div class="muted">
                                    {SHOP_NAME}, {SHOP_EMAIL}, {ORDER_CODE}, {CUSTOMER_NAME}, {TOTAL}, {ORDER_STATUS}, {ORDER_URL}, {CREATED_AT}, {MESSAGE}
                                </div>
                            </div>

                            <div class="muted mb-2">
                                <asp:Literal ID="lit_template_updated_info" runat="server"></asp:Literal>
                            </div>

                            <div class="action-row">
                                <asp:Button ID="but_template_reset" runat="server" Text="Khôi phục mặc định" CssClass="button rounded"
                                    CausesValidation="false" OnClick="but_template_reset_Click" />
                                <asp:Button ID="but_template_save" runat="server" Text="Lưu nội dung" CssClass="button success rounded"
                                    OnClick="but_template_save_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
