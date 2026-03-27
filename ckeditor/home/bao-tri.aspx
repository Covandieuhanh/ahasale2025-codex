<%@ Page Language="C#" AutoEventWireup="true" CodeFile="bao-tri.aspx.cs" Inherits="home_bao_tri" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Bảo trì hệ thống</title>
    <link href="/Metro-UI-CSS-master/css/metro-all.min.css" rel="stylesheet" />
    <link href="/assetscss/login.css?v=2026-03-06.2" rel="stylesheet" />
    <style>
        .maintenance-wrap {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 24px;
        }

        .maintenance-card {
            width: min(720px, 100%);
            border: 1px solid rgba(255, 255, 255, 0.22);
            border-radius: 24px;
            overflow: hidden;
            background: rgba(255, 255, 255, 0.08);
            box-shadow: 0 24px 48px rgba(0, 0, 0, 0.2);
            backdrop-filter: blur(2px);
        }

        .maintenance-head {
            background: rgba(0, 0, 0, 0.18);
            text-align: center;
            padding: 24px 20px 12px;
        }

        .maintenance-logo {
            width: 92px;
            height: 92px;
            border-radius: 50%;
            border: 2px solid rgba(255, 255, 255, 0.7);
            object-fit: cover;
        }

        .maintenance-body {
            padding: 24px 24px 28px;
            color: #f8fafc;
            text-align: center;
        }

        .maintenance-title {
            font-size: 30px;
            font-weight: 700;
            margin: 10px 0 6px;
        }

        .maintenance-sub {
            opacity: 0.95;
            margin-bottom: 14px;
            font-size: 16px;
        }

        .maintenance-note {
            opacity: 0.86;
            line-height: 1.55;
            margin: 0 auto 18px;
            max-width: 560px;
        }

        .maintenance-actions {
            display: flex;
            gap: 12px;
            justify-content: center;
            flex-wrap: wrap;
        }
    </style>
</head>
<body class="body-bao-tri">
    <form id="form1" runat="server">
        <div class="bg-bao-tri maintenance-wrap">
            <div class="maintenance-card">
                <div class="maintenance-head">
                    <img class="maintenance-logo" src="/uploads/images/avatar/avt-aha.jpg" alt="AhaSale" />
                </div>
                <div class="maintenance-body">
                    <div class="maintenance-title">AHASALE ĐANG BẢO TRÌ</div>
                    <div class="maintenance-sub">Hệ thống đang được nâng cấp để hoạt động ổn định hơn.</div>
                    <div class="maintenance-note">
                        Vui lòng quay lại sau ít phút. Trong thời gian này, các tính năng đăng nhập và giao dịch có thể tạm thời không khả dụng.
                    </div>
                    <div class="maintenance-actions">
                        <a href="/" class="button success rounded">Về trang chủ</a>
                        <a href="/dang-nhap" class="button dark rounded">Đăng nhập lại</a>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
