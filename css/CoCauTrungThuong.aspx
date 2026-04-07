<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CoCauTrungThuong.aspx.cs" Inherits="CoCauTrungThuong" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <title>Cơ cấu trúng thưởng</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        body{font-family:"Be Vietnam Pro","Segoe UI",Arial;background:#0f172a;color:#e5e7eb;display:flex;justify-content:center;align-items:center;min-height:100vh;margin:0}
        .card{background:#111827;border:1px solid rgba(255,255,255,.1);border-radius:14px;padding:20px;max-width:420px;width:100%}
        h2{margin-top:0}
        .row{display:flex;gap:10px}
        input[type=number]{flex:1;padding:10px 12px;border-radius:10px;border:1px solid rgba(255,255,255,.15);background:#0b1220;color:#e5e7eb}
        .btn{padding:10px 14px;border:0;border-radius:10px;font-weight:700;cursor:pointer;background:linear-gradient(135deg,#22d3ee,#f472b6);color:#061016}
        .ok{color:#34d399;margin-top:8px}
        .err{color:#fb7185;margin-top:8px}
        .muted{color:#94a3b8;margin-top:8px}
    </style>
</head>
<body>
<form id="form1" runat="server">
  <div class="card">
    <h2>⚙️ Cơ cấu trúng thưởng</h2>
    <div class="row">
      <asp:TextBox ID="txtIndex" runat="server" TextMode="Number" Min="1" placeholder="Nhập số thứ tự (VD: 2)"></asp:TextBox>
      <asp:Button ID="btnSet" runat="server" CssClass="btn" Text="Cơ cấu" OnClick="btnSet_Click" />
    </div>
    <div class="muted">Số thứ tự tính từ 1 (1 = người đầu tiên trong danh sách ở trang quay). Áp dụng cho <strong>1 lượt quay kế tiếp</strong> trên toàn site.</div>
    <asp:Literal ID="ltStatus" runat="server" />
  </div>
</form>
</body>
</html>
