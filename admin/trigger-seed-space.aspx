<%@ Page Language="C#" AutoEventWireup="true" CodeFile="trigger-seed-space.aspx.cs" Inherits="admin_trigger_seed_space" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <title>Trigger Seed Space Access</title>
    <style>
        body { font-family: "Be Vietnam Pro", "Segoe UI", Arial, sans-serif; padding: 24px; background: #f7f9fc; }
        .card { background: #ffffff; border: 1px solid #e1e7ef; border-radius: 10px; padding: 16px 20px; max-width: 720px; }
        .title { font-size: 18px; font-weight: 700; margin-bottom: 8px; }
        .muted { color: #667085; }
        .ok { color: #0f9d58; font-weight: 700; }
        .btn { padding: 8px 14px; border-radius: 6px; border: 1px solid #1f2937; background: #1f2937; color: #fff; cursor: pointer; }
        .btn.secondary { background: #ffffff; color: #1f2937; }
        .row { margin: 10px 0; }
        .mono { font-family: Consolas, monospace; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="card">
            <div class="title">Kích hoạt hiển thị dữ liệu Home/Shop</div>
            <div class="row muted">
                Trang này đồng bộ quyền hiển thị từ dữ liệu tài khoản sang bảng CoreSpaceAccess.
            </div>
            <div class="row">
                <asp:Label ID="lblStatus" runat="server" Text="" />
            </div>
            <div class="row">
                <asp:Button ID="btnRun" runat="server" CssClass="btn" Text="Chạy lại đồng bộ" OnClick="btnRun_Click" />
            </div>
            <div class="row muted mono">
                Endpoint: /admin/trigger-seed-space.aspx
            </div>
        </div>
    </form>
</body>
</html>
