<%@ Page Title="BĐS - Liên kết tin" Language="C#" AutoEventWireup="true" CodeFile="bds-lien-ket-tin.aspx.cs" Inherits="admin_quan_ly_noi_dung_home_bds_lien_ket_tin" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>BĐS - Liên kết tin</title>
    <style>
        body {
            margin: 0;
            font-family: "Be Vietnam Pro", "Segoe UI", Arial, sans-serif;
            background: #f5f7fb;
            color: #1f2937;
        }
        .shell {
            max-width: 1100px;
            margin: 0 auto;
            padding: 24px 16px 40px;
        }
        .card {
            background: #fff;
            border: 1px solid #e5e7eb;
            border-radius: 16px;
            box-shadow: 0 8px 24px rgba(15, 23, 42, 0.06);
            padding: 20px;
            margin-bottom: 16px;
        }
        .muted {
            color: #6b7280;
            font-size: 14px;
        }
        .actions {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin: 16px 0;
        }
        .btn, select {
            height: 40px;
            border-radius: 10px;
            border: 1px solid #d1d5db;
            padding: 0 14px;
            font-size: 14px;
        }
        .btn {
            cursor: pointer;
            background: #fff;
        }
        .btn-primary {
            background: #0f766e;
            border-color: #0f766e;
            color: #fff;
        }
        .btn-outline {
            background: #fff;
            color: #111827;
        }
        table {
            width: 100%;
            border-collapse: collapse;
            font-size: 14px;
        }
        th, td {
            border-bottom: 1px solid #e5e7eb;
            padding: 10px 8px;
            text-align: left;
            vertical-align: top;
        }
        th {
            background: #f9fafb;
        }
        .badge {
            display: inline-block;
            padding: 4px 8px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 700;
        }
        .badge-success {
            background: #dcfce7;
            color: #166534;
        }
        .badge-danger {
            background: #fee2e2;
            color: #991b1b;
        }
        .empty {
            background: #f9fafb;
            border: 1px dashed #d1d5db;
            border-radius: 12px;
            padding: 16px;
            margin-top: 12px;
        }
        .topbar {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 12px;
            margin-bottom: 16px;
        }
        .back-link {
            color: #0f766e;
            text-decoration: none;
            font-weight: 700;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="shell">
            <div class="topbar">
                <div>
                    <div class="muted">Admin không gian Bất động sản</div>
                    <h1 style="margin: 6px 0 0;">BĐS - Liên kết tin</h1>
                </div>
                <a class="back-link" href="/admin/default.aspx?mspace=batdongsan">Quay lại không gian BĐS</a>
            </div>

            <div class="card">
                <div class="muted">Trang này đang được tách khỏi khung admin chung để xử lý lỗi timeout. Khi ổn định mình sẽ đưa lại vào shell admin chuẩn.</div>

                <div class="actions">
                    <asp:DropDownList ID="ddl_source" runat="server" Width="260px" />
                    <asp:Button ID="btn_sync_sources" runat="server" CssClass="btn btn-outline" Text="Đồng bộ nguồn hợp lệ" OnClick="btn_sync_sources_Click" />
                    <asp:Button ID="btn_refresh" runat="server" CssClass="btn btn-primary" Text="Chạy cập nhật ngay" OnClick="btn_refresh_Click" />
                    <asp:Button ID="btn_reload" runat="server" CssClass="btn btn-outline" Text="Tải lại danh sách" OnClick="btn_reload_Click" />
                </div>

                <asp:Label ID="lb_result" runat="server" CssClass="muted"></asp:Label>
            </div>

            <div class="card">
                <h3 style="margin-top: 0;">Log gần nhất</h3>
                <asp:Repeater ID="rpt_logs" runat="server">
                    <HeaderTemplate>
                        <table>
                            <thead>
                                <tr>
                                    <th>Thời gian</th>
                                    <th>Nguồn</th>
                                    <th>Mới</th>
                                    <th>Update</th>
                                    <th>Hết hạn</th>
                                    <th>Trạng thái</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("RanAtText") %></td>
                            <td><%# Eval("SourceLabel") %></td>
                            <td><%# Eval("Created") %></td>
                            <td><%# Eval("Updated") %></td>
                            <td><%# Eval("Expired") %></td>
                            <td><span class='badge <%# Eval("StatusBadge") %>'><%# Eval("StatusText") %></span></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>

                <asp:Panel ID="pn_empty" runat="server" Visible="false" CssClass="empty">
                    Chưa có log cập nhật nào hoặc trang đang ở safe mode.
                </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>
