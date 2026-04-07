<%@ Page Language="C#" AutoEventWireup="true" CodeFile="cho-thanh-toan.aspx.cs" Inherits="gianhang_cho_thanh_toan" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AHASALE.VN</title>
    <meta charset="UTF-8" />
    <meta http-equiv="content-language" content="vi" />
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=yes" />

    <script>
        (function () {
            document.documentElement.setAttribute('data-bs-theme', 'light');
            try { localStorage.removeItem('theme-preference'); } catch (e) { }
            document.addEventListener('DOMContentLoaded', function () {
                if (!document.body) return;
                document.body.setAttribute('data-bs-theme', 'light');
                document.body.classList.remove('theme-dark', 'dark', 'dark-mode');
            });
        })();
    </script>

    <link href="/Metro-UI-CSS-master/css/metro-all.min.css" rel="stylesheet" />
    <link href="/css/gianhang-home.css" rel="stylesheet" />
    <link href="/assetscss/login.css?v=2026-03-02.1" rel="stylesheet" />
    <link href="/assetscss/gianhang-ui-refresh.css?v=2026-03-02.1" rel="stylesheet" />
    <link href="/assetscss/bcorn-with-metro.css" rel="stylesheet" />
    <link href="/assetscss/fix-metro.css" rel="stylesheet" />

    <style>
        :root {
            --wait-bg-1: #eef3fa;
            --wait-bg-2: #eef3fa;
            --wait-card: #ffffff;
            --wait-card-soft: #ffffff;
            --wait-border: rgba(98, 105, 118, 0.2);
            --wait-text: #182433;
            --wait-muted: #637085;
            --wait-primary: #ff5b2e;
            --wait-success: #06c167;
            --wait-danger: #dc2626;
            --wait-warn: #f59e0b;
        }

        html[data-bs-theme="dark"] {
            --wait-bg-1: #0b1220;
            --wait-bg-2: #0f1b2d;
            --wait-card: #0f172a;
            --wait-card-soft: #111d34;
            --wait-border: rgba(148, 163, 184, 0.18);
            --wait-text: #e5e7eb;
            --wait-muted: #94a3b8;
        }

        <% if (PortalRequest_cl.IsGianHangPortalRequest()) { %>
        :root {
            --wait-bg-1: #fff4ee;
            --wait-bg-2: #ffe2d3;
            --wait-card: #ffffff;
            --wait-card-soft: #fff8f4;
            --wait-border: rgba(238, 77, 45, 0.18);
            --wait-primary: #ee4d2d;
            --wait-success: #ee4d2d;
            --wait-warn: #ff8a3d;
        }

        html[data-bs-theme="dark"] {
            --wait-bg-1: #2a150f;
            --wait-bg-2: #1b120e;
            --wait-card: #181110;
            --wait-card-soft: #241714;
            --wait-border: rgba(255, 145, 105, 0.22);
            --wait-text: #fff1eb;
            --wait-muted: #f2c0ad;
        }
        <% } %>

        html,
        body {
            min-height: 100%;
            margin: 0;
        }

        .body-login-bcorn1 {
            background: linear-gradient(180deg, var(--wait-bg-1), var(--wait-bg-2));
            color: var(--wait-text);
            overflow-x: hidden;
            -webkit-font-smoothing: antialiased;
            -webkit-overflow-scrolling: touch;
        }

        <% if (PortalRequest_cl.IsGianHangPortalRequest()) { %>
        .body-login-bcorn1 {
            background: linear-gradient(180deg, #fff4ee, #ffe2d3) !important;
        }
        <% } %>

        .wait-shell,
        .wait-shell * {
            color: inherit;
            box-sizing: border-box;
        }

        .wait-shell {
            max-width: 1020px;
            width: 100%;
            padding: 0 14px;
            margin: 14px auto 32px;
        }

        .wait-stack {
            display: flex;
            flex-direction: column;
            gap: 14px;
        }

        .wait-hero-card {
            border: 1px solid var(--wait-border);
            border-radius: 18px;
            background: linear-gradient(140deg, #ffffff, #f5f7fb);
            box-shadow: 0 12px 28px rgba(15, 23, 42, 0.08);
            padding: 16px;
        }

        .wait-hero-top {
            display: flex;
            align-items: center;
            gap: 11px;
        }

        .wait-logo {
            width: 50px;
            height: 50px;
            border-radius: 12px;
            object-fit: cover;
            border: 1px solid rgba(98, 105, 118, 0.2);
            background: #0f172a;
            padding: 5px;
        }

        .wait-stage {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 8px;
            padding: 6px 11px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 700;
            color: #6b3700;
            background: #fff3db;
            border: 1px solid #ffd38a;
        }

        <% if (PortalRequest_cl.IsGianHangPortalRequest()) { %>
        .wait-hero-card {
            background: linear-gradient(140deg, #ffffff, #fff4ee);
            border-color: rgba(238, 77, 45, 0.16);
        }

        .wait-card {
            background: linear-gradient(160deg, #ffffff, #fff9f5);
            border-color: rgba(238, 77, 45, 0.16);
        }

        .wait-stage,
        .wait-tag,
        .wait-kicker,
        .wait-amount {
            border-color: #fdba74;
            background: #fff7ed;
            color: #c2410c;
        }

        .wait-alert {
            border-color: #fdba74;
            background: #fff7ed;
            color: #c2410c;
        }

        .wait-btn.primary,
        .wait-btn.success {
            background: linear-gradient(140deg, #ee4d2d, #ff7a45);
            border-color: rgba(238, 77, 45, 0.45);
        }

        .wait-order-list {
            border-color: rgba(238, 77, 45, 0.16);
            background: #fffaf7;
        }

        .wait-order-table thead th {
            background: #fff7ed;
        }

        .wait-order-price,
        .wait-order-total {
            color: #ea580c;
        }

        .wait-progress::before {
            background: linear-gradient(90deg, #ee4d2d, #ff8a3d);
        }

        html[data-bs-theme="dark"] .body-login-bcorn1 {
            background: linear-gradient(180deg, #2a150f, #1b120e) !important;
        }

        html[data-bs-theme="dark"] .wait-hero-card {
            background: linear-gradient(140deg, #201412, #2a1814);
            border-color: rgba(255, 145, 105, 0.22);
        }

        html[data-bs-theme="dark"] .wait-card {
            background: linear-gradient(160deg, #181110, #241714);
            border-color: rgba(255, 145, 105, 0.22);
        }

        html[data-bs-theme="dark"] .wait-stage,
        html[data-bs-theme="dark"] .wait-tag,
        html[data-bs-theme="dark"] .wait-kicker,
        html[data-bs-theme="dark"] .wait-amount {
            background: rgba(238, 77, 45, 0.16);
            border-color: rgba(255, 165, 132, 0.38);
            color: #ffd2c1;
        }

        html[data-bs-theme="dark"] .wait-alert {
            background: rgba(238, 77, 45, 0.14);
            border-color: rgba(255, 165, 132, 0.34);
            color: #ffd2c1;
        }

        html[data-bs-theme="dark"] .wait-order-list {
            background: #221613;
            border-color: rgba(255, 145, 105, 0.2);
        }

        html[data-bs-theme="dark"] .wait-order-table thead th {
            background: #2a1814;
            border-bottom-color: rgba(255, 145, 105, 0.16);
        }

        html[data-bs-theme="dark"] .wait-order-price,
        html[data-bs-theme="dark"] .wait-order-total {
            color: #ffb395;
        }

        html[data-bs-theme="dark"] .wait-btn.primary,
        html[data-bs-theme="dark"] .wait-btn.success {
            background: linear-gradient(140deg, #ee4d2d, #ff7a45);
            border-color: rgba(255, 145, 105, 0.45);
        }
        <% } %>

        .wait-title {
            margin: 0;
            font-size: 30px;
            line-height: 1.18;
            font-weight: 800;
            letter-spacing: .12px;
            color: var(--wait-text);
        }

        .wait-tag {
            margin-top: 8px;
            display: inline-flex;
            align-items: center;
            padding: 5px 11px;
            border-radius: 999px;
            border: 1px solid #b7f0ce;
            background: #e8fbf0;
            color: #005e30;
            font-size: 13px;
            font-weight: 700;
        }

        .wait-sub {
            margin-top: 10px;
            color: var(--wait-muted);
            font-size: 14px;
            line-height: 1.52;
        }

        .wait-hero-actions {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
            margin-top: 12px;
        }

        .wait-hero-card,
        .wait-stack,
        .wait-card {
            width: 100%;
            max-width: 100%;
        }

        .wait-card {
            border: 1px solid var(--wait-border);
            border-radius: 16px;
            background: linear-gradient(160deg, var(--wait-card), var(--wait-card-soft));
            box-shadow: 0 8px 24px rgba(15, 23, 42, 0.06);
            padding: 14px;
            position: relative;
        }

        .wait-kicker {
            display: inline-flex;
            align-items: center;
            border: 1px solid #b7f0ce;
            background: #e8fbf0;
            color: #005e30;
            border-radius: 999px;
            padding: 5px 10px;
            font-size: 12px;
            font-weight: 700;
            margin-bottom: 8px;
        }

        .wait-card-title {
            margin: 0;
            font-size: 28px;
            line-height: 1.2;
            font-weight: 800;
            color: var(--wait-text);
        }

        .wait-note {
            margin-top: 8px;
            color: var(--wait-muted);
            font-size: 14px;
            line-height: 1.45;
        }

        .wait-note.warn {
            color: #b91c1c;
        }

        .wait-amount {
            margin-top: 12px;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 8px 12px;
            border-radius: 12px;
            border: 1px solid #b7f0ce;
            background: #f0fdf4;
            color: #166534;
            font-size: 36px;
            font-weight: 800;
            line-height: 1.1;
        }

        .wait-pin {
            margin-top: 10px;
            display: flex;
            align-items: center;
            width: 100%;
            min-height: 50px;
            border-radius: 14px;
            border: 1px solid rgba(98, 105, 118, 0.3);
            background: #ffffff;
            overflow: hidden;
        }

        .wait-pin-icon {
            flex: 0 0 48px;
            text-align: center;
            color: #0f172a;
            border-right: 1px solid rgba(15, 23, 42, 0.14);
            line-height: 50px;
            font-size: 18px;
        }

        .wait-pin-input {
            width: 100%;
            min-width: 0;
            border: 0 !important;
            box-shadow: none !important;
            background: transparent !important;
            color: #111827 !important;
            height: 50px !important;
            font-size: 16px;
            font-weight: 700;
            letter-spacing: .2px;
            padding: 0 12px !important;
        }

        .wait-pin-input::placeholder {
            color: #64748b;
            font-weight: 500;
        }

        .wait-pin .aha-password-toggle-label {
            font-size: 14px;
        }

        .wait-inline-error {
            margin-top: 6px;
            font-size: 12px;
            color: #dc2626;
            line-height: 1.4;
            display: none;
        }

        .wait-inline-error.show {
            display: block;
        }

        .wait-alert {
            margin-top: 10px;
            border-radius: 12px;
            padding: 10px 12px;
            border: 1px solid #fecaca;
            background: #fff5f5;
            color: #b91c1c;
            font-size: 13px;
            line-height: 1.45;
        }

        .wait-actions {
            margin-top: 12px;
            display: grid;
            gap: 8px;
        }

        .wait-card-active .wait-actions {
            position: sticky;
            bottom: max(8px, env(safe-area-inset-bottom));
            z-index: 6;
            background: linear-gradient(180deg, rgba(246, 248, 252, 0), rgba(246, 248, 252, 0.94) 26%, rgba(246, 248, 252, 1));
            padding-top: 10px;
        }

        html[data-bs-theme="dark"] .wait-card-active .wait-actions {
            background: linear-gradient(180deg, rgba(15, 23, 42, 0), rgba(15, 23, 42, 0.88) 26%, rgba(15, 23, 42, 1));
        }

        .wait-btn,
        .wait-btn:visited {
            appearance: none;
            border: 1px solid transparent;
            min-height: 44px;
            border-radius: 12px;
            padding: 0 14px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            text-decoration: none;
            font-size: 15px;
            font-weight: 700;
            cursor: pointer;
            transition: all .18s ease;
            color: #fff !important;
            width: 100%;
        }

        .wait-btn:hover {
            transform: translateY(-1px);
        }

        .wait-btn.primary {
            background: linear-gradient(140deg, #ff5b2e, #ff7a47);
            border-color: rgba(255, 106, 57, 0.5);
        }

        .wait-btn.success {
            background: linear-gradient(140deg, #06c167, #05a75a);
            border-color: rgba(6, 193, 103, 0.45);
        }

        .wait-btn.danger {
            background: linear-gradient(145deg, #dc2626, #ef4444);
            border-color: rgba(239, 68, 68, 0.45);
        }

        .wait-btn.muted {
            background: #edf2f7;
            border-color: rgba(98, 105, 118, 0.28);
            color: #334155 !important;
        }

        .wait-btn.loading {
            opacity: .92;
            pointer-events: none;
            cursor: wait;
        }

        .wait-btn.loading::after {
            content: "";
            width: 14px;
            height: 14px;
            border-radius: 50%;
            border: 2px solid rgba(255, 255, 255, 0.45);
            border-top-color: #fff;
            display: inline-block;
            margin-left: 8px;
            vertical-align: -2px;
            animation: waitSpin .75s linear infinite;
        }

        .wait-trust {
            margin-top: 10px;
            font-size: 12px;
            color: #5b6b83;
            line-height: 1.45;
        }

        .wait-order-head {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 10px;
            flex-wrap: wrap;
        }

        .wait-order-summary {
            min-width: 0;
        }

        .wait-order-summary .wait-note {
            margin-top: 6px;
        }

        .wait-order-badge {
            display: inline-flex;
            align-items: center;
            padding: 5px 10px;
            border-radius: 999px;
            border: 1px solid rgba(255, 91, 46, 0.18);
            background: rgba(255, 91, 46, 0.08);
            color: var(--wait-primary);
            font-size: 12px;
            font-weight: 700;
            white-space: nowrap;
        }

        .wait-order-list {
            margin-top: 14px;
            border: 1px solid var(--wait-border);
            border-radius: 16px;
            background: #fff;
            overflow: hidden;
            box-shadow: 0 8px 24px rgba(15, 23, 42, 0.05);
        }

        .wait-order-table-wrap {
            overflow-x: auto;
            overflow-y: hidden;
            -webkit-overflow-scrolling: touch;
        }

        .wait-order-table {
            width: 100%;
            min-width: 680px;
            border-collapse: separate;
            border-spacing: 0;
        }

        .wait-order-table thead th {
            padding: 12px 10px;
            border-bottom: 1px solid var(--wait-border);
            background: #f8fafc;
            color: #48566e;
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: .16px;
            white-space: nowrap;
        }

        .wait-order-line td {
            padding: 12px 10px;
            border-bottom: 1px solid rgba(98, 105, 118, 0.14);
            vertical-align: middle;
            color: #1e293b;
            font-size: 14px;
            background: #fff;
        }

        .wait-order-line:last-child td {
            border-bottom: 0;
        }

        .wait-order-thumb {
            width: 64px;
            height: 64px;
            border-radius: 14px;
            object-fit: cover;
            border: 1px solid rgba(98, 105, 118, 0.2);
            background: #f8fafc;
            display: block;
            margin: 0 auto;
        }

        .wait-order-product-name {
            font-weight: 700;
            line-height: 1.35;
            color: var(--wait-text);
        }

        .wait-order-product-meta {
            margin-top: 6px;
            display: flex;
            align-items: center;
            gap: 8px;
            flex-wrap: wrap;
        }

        .wait-order-price {
            display: inline-flex;
            align-items: center;
            padding: 4px 10px;
            border-radius: 999px;
            background: rgba(255, 91, 46, 0.08);
            color: var(--wait-primary);
            font-size: 12px;
            font-weight: 700;
        }

        .wait-order-pill {
            display: inline-flex;
            align-items: center;
            padding: 3px 8px;
            border-radius: 999px;
            border: 1px solid rgba(245, 180, 1, .38);
            background: rgba(245, 180, 1, .12);
            color: #b45309;
            font-size: 11px;
            font-weight: 700;
        }

        .wait-order-total {
            font-weight: 800;
            color: #166534;
            white-space: nowrap;
        }

        .wait-progress {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            height: 3px;
            background: rgba(15, 23, 42, 0.08);
            z-index: 9999;
            pointer-events: none;
            overflow: hidden;
        }

        .wait-progress::before {
            content: "";
            display: block;
            width: 42%;
            height: 100%;
            background: linear-gradient(90deg, #ff5b2e, #06c167);
            animation: wait-progress-move 1.05s linear infinite;
            border-radius: 999px;
        }

        html[data-bs-theme="dark"] .wait-hero-card {
            background: linear-gradient(140deg, #0f172a, #111d34);
            border-color: #223246;
            box-shadow: 0 12px 28px rgba(0, 0, 0, 0.45);
        }

        html[data-bs-theme="dark"] .wait-stage {
            background: rgba(245, 158, 11, 0.18);
            border-color: rgba(245, 158, 11, 0.45);
            color: #fcd34d;
        }

        html[data-bs-theme="dark"] .wait-tag,
        html[data-bs-theme="dark"] .wait-kicker,
        html[data-bs-theme="dark"] .wait-amount {
            background: rgba(16, 185, 129, 0.12);
            border-color: rgba(16, 185, 129, 0.35);
            color: #6ee7b7;
        }

        html[data-bs-theme="dark"] .wait-pin {
            background: #0f172a;
            border-color: #223246;
        }

        html[data-bs-theme="dark"] .wait-pin-icon {
            color: #e5e7eb;
            border-right-color: rgba(226, 232, 240, 0.16);
        }

        html[data-bs-theme="dark"] .wait-pin-input {
            color: #e5e7eb !important;
        }

        html[data-bs-theme="dark"] .wait-pin-input::placeholder {
            color: #94a3b8;
        }

        html[data-bs-theme="dark"] .wait-inline-error,
        html[data-bs-theme="dark"] .wait-note.warn {
            color: #fecaca;
        }

        html[data-bs-theme="dark"] .wait-alert {
            background: rgba(248, 113, 113, 0.12);
            border-color: rgba(248, 113, 113, 0.35);
            color: #fecaca;
        }

        html[data-bs-theme="dark"] .wait-btn.muted {
            background: #1f2a3a;
            border-color: #334155;
            color: #e5e7eb !important;
        }

        html[data-bs-theme="dark"] .wait-trust {
            color: #94a3b8;
        }

        html[data-bs-theme="dark"] .wait-order-badge {
            background: rgba(255, 122, 71, 0.12);
            border-color: rgba(255, 145, 105, 0.25);
            color: #ffb395;
        }

        html[data-bs-theme="dark"] .wait-order-list {
            background: #0f172a;
            border-color: #223246;
        }

        html[data-bs-theme="dark"] .wait-order-table thead th {
            background: #111d34;
            color: #cbd5e1;
            border-bottom-color: #223246;
        }

        html[data-bs-theme="dark"] .wait-order-line td {
            color: #e5e7eb;
            background: #0f172a;
            border-bottom-color: rgba(148, 163, 184, 0.2);
        }

        html[data-bs-theme="dark"] .wait-order-thumb {
            background: #0b1220;
            border-color: rgba(148, 163, 184, 0.3);
        }

        html[data-bs-theme="dark"] .wait-order-product-name {
            color: #f8fafc;
        }

        html[data-bs-theme="dark"] .wait-order-price {
            background: rgba(255, 122, 71, 0.12);
            color: #ffb395;
        }

        html[data-bs-theme="dark"] .wait-order-pill {
            background: rgba(245, 180, 1, .14);
            border-color: rgba(245, 180, 1, .28);
            color: #fde68a;
        }

        html[data-bs-theme="dark"] .wait-order-total {
            color: #6ee7b7;
        }

        html[data-bs-theme="dark"] .wait-progress {
            background: rgba(15, 23, 42, 0.55);
        }

        @keyframes wait-progress-move {
            0% { transform: translateX(-110%); }
            100% { transform: translateX(320%); }
        }

        @keyframes waitSpin {
            to { transform: rotate(360deg); }
        }

        @media (max-width: 739px) {
            .wait-order-table-wrap {
                overflow: visible;
            }

            .wait-order-table {
                min-width: 0;
            }

            .wait-order-table thead {
                display: none;
            }

            .wait-order-line {
                display: grid;
                grid-template-columns: 84px minmax(0, 1fr);
                gap: 10px;
                padding: 12px;
                border-bottom: 1px solid rgba(98, 105, 118, 0.14);
            }

            .wait-order-line td {
                padding: 0;
                border: 0;
                background: transparent;
            }

            .wait-order-line .cell-id {
                grid-column: 2;
                font-size: 12px;
                color: var(--wait-muted);
                text-align: left !important;
            }

            .wait-order-line .cell-image {
                grid-column: 1;
                grid-row: 1 / span 4;
                align-self: start;
            }

            .wait-order-line .cell-product {
                grid-column: 2;
            }

            .wait-order-line .cell-qty,
            .wait-order-line .cell-uudai,
            .wait-order-line .cell-total {
                grid-column: 2;
                display: flex;
                align-items: center;
                justify-content: space-between;
                gap: 12px;
                font-size: 13px;
            }

            .wait-order-line .cell-qty::before,
            .wait-order-line .cell-uudai::before,
            .wait-order-line .cell-total::before {
                content: attr(data-label);
                color: var(--wait-muted);
                font-weight: 600;
            }

            .wait-order-thumb {
                width: 74px;
                height: 74px;
            }

            .wait-order-product-meta {
                margin-top: 8px;
            }

            .wait-order-total {
                text-align: right;
            }

            html[data-bs-theme="dark"] .wait-order-line {
                border-bottom-color: rgba(148, 163, 184, 0.2);
            }
        }

        @media (min-width: 740px) {
            .wait-shell {
                padding: 0 18px;
                margin-top: 18px;
            }

            .wait-title {
                font-size: 32px;
            }

            .wait-card-title {
                font-size: 30px;
            }

            .wait-actions {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }

            .wait-card-active .wait-actions {
                position: static;
                background: transparent;
                padding-top: 0;
            }
        }

        @media (min-width: 1020px) {
            .wait-title {
                font-size: 34px;
            }
        }
    </style>
</head>

<body class="body-login-bcorn1">
<form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True"></asp:ScriptManager>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <asp:Timer ID="Timer1" runat="server" Interval="12000" OnTick="Timer1_Tick"></asp:Timer>

            <div class="wait-shell">
                <div class="wait-stack">
                    <section class="wait-card wait-hero-card">
                        <div class="wait-hero-top">
                            <img src="/uploads/images/logo-aha-trang.png" alt="AhaSale" class="wait-logo" />
                            <div>
                                <div class="wait-stage">Quy trình 2 bước: Kích hoạt và Trao đổi</div>
                                <h1 class="wait-title">Chờ Trao đổi Đơn #<asp:Label ID="Label4" runat="server" Text=""></asp:Label></h1>
                                <span class="wait-tag">1 Quyền = 1000 VNĐ (quy đổi nội bộ)</span>
                            </div>
                        </div>
                        <div class="wait-sub">
                            Quyền tiêu dùng là đơn vị quy ước nội bộ được hiển thị và sử dụng trên nền tảng ahasale.vn. Quyền tiêu dùng không phải tiền tệ hợp pháp và chỉ có hiệu lực trong phạm vi nền tảng.
                        </div>
                        <asp:PlaceHolder ID="ph_gianhang_home" runat="server" Visible="false">
                            <div class="wait-hero-actions">
                                <a href="/gianhang/default.aspx" class="wait-btn primary" style="width:auto; min-width:140px;">Trang chủ</a>
                                <a href="/gianhang/don-ban.aspx" class="wait-btn muted" style="width:auto; min-width:120px;">Đơn bán</a>
                            </div>
                        </asp:PlaceHolder>
                    </section>

                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="false">
                        <section class="wait-card">
                            <div class="wait-kicker">Bước 1: Chờ quét thẻ</div>
                            <h2 class="wait-card-title">Sẵn sàng Trao đổi</h2>
                            <div class="wait-note">Vui lòng chạm thẻ khách hàng để bắt đầu phiên Trao đổi.</div>
                            <div class="wait-amount"><asp:Label ID="Label1" runat="server" Text=""></asp:Label></div>
                            <div class="wait-actions">
                                <asp:Button ID="btn_huy_cho" runat="server" OnClick="btn_huy_cho_Click" CausesValidation="false" Text="Hủy chờ Trao đổi" CssClass="wait-btn danger" />
                                <asp:HyperLink ID="lnk_refresh_wait" runat="server" CssClass="wait-btn muted">Làm mới</asp:HyperLink>
                            </div>
                            <div class="wait-trust">Đơn sẽ giữ trạng thái chờ Trao đổi cho tới khi có thẻ hợp lệ.</div>
                        </section>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="false">
                        <section class="wait-card wait-card-active">
                            <div class="wait-kicker">Bước 2: Xác thực PIN</div>
                            <h2 class="wait-card-title">Xin chào, <asp:Label ID="Label3" runat="server" Text=""></asp:Label></h2>
                            <div class="wait-note">Bạn sắp trao đổi <b><asp:Label ID="Label2" runat="server" Text=""></asp:Label></b> với <b><asp:Label ID="lb_tengianhang" runat="server" Text=""></asp:Label></b>.</div>
                            <div class="wait-note warn">Nếu không phải bạn, vui lòng nhấn <b>Hủy</b>.</div>

                            <div class="wait-pin">
                                <span class="mif-key wait-pin-icon"></span>
                                <asp:TextBox
                                    MaxLength="4"
                                    TextMode="Password"
                                    ID="txt_mapin"
                                    runat="server"
                                    Attributes="inputmode:numeric;pattern:[0-9]*"
                                    CssClass="wait-pin-input"
                                    placeholder="Nhập mã PIN 4 số để hoàn tất Trao đổi">
                                </asp:TextBox>
                                <button type="button" class="aha-password-toggle js-toggle-password" aria-label="Hiện mã PIN">
                                    <span class="aha-password-toggle-label">Hiện</span>
                                </button>
                            </div>
                            <div class="wait-inline-error" id="pin_inline_error"></div>

                            <asp:PlaceHolder ID="ph_alert_the" runat="server">
                                <div class="wait-alert" runat="server" id="box_alert_the" visible="false">
                                    <asp:Literal ID="lb_thongbao_the" runat="server" Visible="false"></asp:Literal>
                                </div>
                            </asp:PlaceHolder>

                            <div class="wait-actions">
                                <asp:Button ID="Button2" OnClick="Button2_Click" runat="server" Text="Hủy" CssClass="wait-btn muted" />
                                <asp:Button ID="Button3" OnClick="Button3_Click" runat="server" Text="Trao đổi" CssClass="wait-btn success" OnClientClick="return ahaWaitBeforeSubmit(this);" />
                            </div>
                            <div class="wait-trust">PIN thẻ gồm 4 số. Nếu bạn nhập sai quá số lần cho phép, hệ thống sẽ khóa thao tác để bảo mật.</div>
                        </section>
                    </asp:PlaceHolder>

                    <section class="wait-card">
                        <div class="wait-order-head">
                            <div class="wait-order-summary">
                                <h2 class="wait-card-title">Chi tiết đơn hàng</h2>
                                <div class="wait-note">Danh sách sản phẩm trong đơn đang chờ Trao đổi, hiển thị theo kiểu giỏ hàng để dễ xem hơn trên cả máy tính và điện thoại.</div>
                            </div>
                            <div class="wait-order-badge">
                                <asp:Label ID="lb_order_items" runat="server" Text="0"></asp:Label>&nbsp;sản phẩm
                            </div>
                        </div>

                        <div class="wait-order-list">
                            <div class="wait-order-table-wrap">
                                <table class="wait-order-table">
                                    <thead>
                                        <tr>
                                            <th style="width:64px;">ID</th>
                                            <th style="width:88px;">Ảnh</th>
                                            <th class="text-left" style="min-width:220px;">Sản phẩm</th>
                                            <th style="width:80px;">SL</th>
                                            <th style="width:104px;">Ưu đãi</th>
                                            <th class="text-end" style="min-width:132px;">Tổng</th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        <asp:Repeater ID="Repeater2" runat="server">
                                            <ItemTemplate>
                                                <tr class="wait-order-line">
                                                    <td class="text-center cell-id"><%# Eval("id") %></td>

                                                    <td class="text-center cell-image">
                                                        <div data-role="lightbox" class="c-pointer">
                                                            <img src="<%# Eval("image") %>" class="wait-order-thumb" alt="" />
                                                        </div>
                                                    </td>

                                                    <td class="text-left cell-product">
                                                        <div class="wait-order-product-name"><%# Eval("name") %></div>
                                                        <div class="wait-order-product-meta">
                                                            <span class="wait-order-price"><%# FormatQuyen(Eval("giaban")) %> Quyền</span>
                                                            <%# (Convert.ToInt32(Eval("PhanTramUuDai")) > 0
                                                                ? "<span class='wait-order-pill'>-" + Eval("PhanTramUuDai") + "% ưu đãi</span>"
                                                                : "") %>
                                                        </div>
                                                    </td>

                                                    <td class="text-center cell-qty" data-label="Số lượng"><%# Eval("soluong") %></td>

                                                    <td class="text-center cell-uudai" data-label="Ưu đãi">
                                                        <%# (Convert.ToInt32(Eval("PhanTramUuDai")) > 0 ? (Eval("PhanTramUuDai") + "%") : "-") %>
                                                    </td>

                                                    <td class="text-end cell-total" data-label="Tổng">
                                                        <div class="wait-order-total"><%# FormatQuyen(Eval("thanhtien")) %> Quyền</div>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </section>
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btn_huy_cho" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="900">
        <ProgressTemplate>
            <div class="wait-progress"></div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</form>

<script src="/Metro-UI-CSS-master/js/metro.min.js"></script>
    <script src="/js/gianhang-ui-refresh.js?v=2026-03-07.2"></script>
<script>
    function ahaWaitValidatePin() {
        var pinInput = document.getElementById("<%= txt_mapin.ClientID %>");
        var inlineErr = document.getElementById("pin_inline_error");
        if (!pinInput || !inlineErr) return true;

        var digits = String(pinInput.value || "").replace(/\D+/g, "");
        if (!/^\d{4}$/.test(digits)) {
            inlineErr.textContent = "Vui lòng nhập mã PIN gồm đúng 4 số.";
            inlineErr.classList.add("show");
            pinInput.focus();
            return false;
        }

        inlineErr.textContent = "";
        inlineErr.classList.remove("show");
        return true;
    }

    function ahaWaitBeforeSubmit(btn) {
        if (!ahaWaitValidatePin()) return false;
        if (!btn) return true;
        if (btn.dataset.loading === "1") return false;

        btn.dataset.loading = "1";
        btn.dataset.originalText = btn.value;
        btn.value = "Đang trao đổi...";
        btn.classList.add("loading");
        return true;
    }

    function ahaWaitResetSubmitState() {
        var btn = document.getElementById("<%= Button3.ClientID %>");
        if (!btn) return;
        btn.dataset.loading = "0";
        btn.disabled = false;
        btn.classList.remove("loading");
        if (btn.dataset.originalText) {
            btn.value = btn.dataset.originalText;
        }
    }

    function ahaWaitBindPinInput() {
        var pinInput = document.getElementById("<%= txt_mapin.ClientID %>");
        var inlineErr = document.getElementById("pin_inline_error");
        if (!pinInput || !inlineErr) return;
        if (pinInput.dataset.boundValidation === "1") return;

        pinInput.dataset.boundValidation = "1";
        pinInput.addEventListener("input", function () {
            pinInput.value = String(pinInput.value || "").replace(/\D+/g, "").slice(0, 4);
            if (/^\d{4}$/.test(pinInput.value)) {
                inlineErr.textContent = "";
                inlineErr.classList.remove("show");
            }
        });
    }

    document.addEventListener("DOMContentLoaded", function () {
        ahaWaitBindPinInput();
    });

    if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            ahaWaitBindPinInput();
            ahaWaitResetSubmitState();
        });
    }
</script>
</body>
</html>
