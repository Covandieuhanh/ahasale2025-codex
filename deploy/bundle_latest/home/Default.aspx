<%@ Page Title="Trang cá nhân" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="home_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server">
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .square-container { width: 100%; position: relative; overflow: hidden; border-radius: 12px; background: #f6f8fb; }
        .square-container::before { content: ""; display: block; padding-top: 100%; }
        .square-container img { position: absolute; inset: 0; width: 100%; height: 100%; object-fit: cover; }
        .text-clamp-1, .text-clamp-2 {
            display: -webkit-box;
            -webkit-box-orient: vertical;
            overflow: hidden;
            text-overflow: ellipsis;
            line-height: 18px;
        }
        .text-clamp-1 { -webkit-line-clamp: 1; }
        .text-clamp-2 { -webkit-line-clamp: 2; }
        .rounded-circle { object-fit: cover; object-position: 50% 50%; border-radius: 50%; }
        .tblr-overlay {
            position: fixed; inset: 0;
            background: rgba(0,0,0,.65);
            z-index: 99999;
            display: flex; align-items: center; justify-content: center;
        }
        .modal-body { max-height: calc(100vh - 220px); overflow: auto; }


        .bio-public-shell {
            max-width: 780px;
            margin: 16px auto 34px;
            padding: 0 12px;
            --profile-accent: #22c55e;
            position: relative;
            font-family: inherit;
        }

        .bio-public-shell::before {
            content: "";
            position: absolute;
            inset: -120px -60px -80px;
            background:
                radial-gradient(420px 220px at 12% 8%, rgba(34,197,94,.14), transparent 70%),
                radial-gradient(520px 280px at 88% 4%, rgba(56,189,248,.12), transparent 70%),
                radial-gradient(680px 360px at 50% 80%, rgba(16,185,129,.12), transparent 70%);
            z-index: -1;
        }

        .bio-nav {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            margin: 12px 0 18px;
        }

        .bio-nav a {
            text-decoration: none;
            font-weight: 700;
            font-size: 12px;
            color: #0f172a;
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-radius: 999px;
            padding: 6px 12px;
            box-shadow: 0 6px 14px rgba(15, 23, 42, .06);
            transition: .2s ease;
        }

        .bio-nav a:hover {
            border-color: var(--profile-accent);
            color: var(--profile-accent);
            transform: translateY(-1px);
        }

        .profile-hero {
            position: relative;
            border-radius: 26px;
            background: linear-gradient(135deg, #0f6a3a, #2fb344 60%, #34d399);
            box-shadow: 0 24px 60px rgba(16, 42, 67, .25);
            overflow: hidden;
            margin-bottom: 18px;
            color: #ffffff;
        }

        .profile-hero::after {
            content: "";
            position: absolute;
            inset: 0;
            background: linear-gradient(180deg, rgba(0,0,0,.05), rgba(0,0,0,.3));
            pointer-events: none;
        }

        .profile-hero-inner {
            position: relative;
            z-index: 2;
            padding: 26px 22px 24px;
            text-align: center;
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 8px;
        }

        .profile-hero-avatar {
            width: 96px;
            height: 96px;
            border-radius: 50%;
            border: 4px solid rgba(255,255,255,.95);
            object-fit: cover;
            background: #f4f8fd;
            box-shadow: 0 14px 28px rgba(0,0,0,.25);
        }

        .profile-hero-name {
            font-size: 32px;
            font-weight: 800;
            line-height: 1.1;
            margin-top: 4px;
        }

        .profile-hero-user {
            font-size: 14px;
            opacity: .9;
        }

        .profile-hero-desc {
            max-width: 620px;
            font-size: 15px;
            line-height: 1.6;
            opacity: .92;
        }

        .profile-hero-actions {
            margin-top: 8px;
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
            gap: 10px;
        }

        .profile-hero-actions.secondary {
            margin-top: 6px;
        }

        .profile-hero .bio-action-btn {
            border-color: rgba(255,255,255,.55);
            background: rgba(255,255,255,.16);
            color: #ffffff;
        }

        .profile-hero .bio-action-btn:hover {
            border-color: rgba(255,255,255,.85);
            background: rgba(255,255,255,.28);
            color: #ffffff;
        }

        .profile-hero .bio-action-btn.bio-action-solid {
            background: #ffffff;
            border-color: #ffffff;
            color: #1f7a43;
        }

        .profile-hero .badge {
            margin-top: 2px;
        }

        .bio-action-btn {
            min-height: 38px;
            padding: 0 16px;
            border-radius: 999px;
            border: 1px solid rgba(255,255,255,.52);
            background: rgba(255,255,255,.16);
            color: #fff;
            font-size: 13px;
            font-weight: 700;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
            text-decoration: none;
            cursor: pointer;
            transition: .2s ease;
        }

        .bio-action-btn:hover {
            color: #fff;
            border-color: rgba(255,255,255,.78);
            background: rgba(255,255,255,.25);
            transform: translateY(-1px);
        }

        .bio-action-btn.bio-action-solid {
            border-color: var(--profile-accent);
            background: var(--profile-accent);
            color: #ffffff;
        }

        .bio-action-btn.bio-action-solid:hover {
            color: #ffffff;
            background: var(--profile-accent);
            filter: brightness(0.95);
        }

        .bio-card {
            border: 1px solid #d8e4f0;
            border-radius: 18px;
            background: #fff;
            box-shadow: 0 14px 34px rgba(16, 42, 67, .08);
            margin-bottom: 14px;
        }

        .bio-card-mini {
            border-radius: 16px;
            box-shadow: 0 12px 26px rgba(15, 23, 42, .08);
        }

        .bio-layout {
            display: block;
        }

        .bio-main {
            min-width: 0;
        }

        .bio-side {
            min-width: 0;
        }

        .bio-stat-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 10px;
        }

        .bio-stat {
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            padding: 10px;
            background: #f8fbff;
        }

        .bio-stat-label {
            font-size: 12px;
            color: #64748b;
            margin-bottom: 4px;
        }

        .bio-stat-value {
            font-size: 16px;
            font-weight: 800;
            color: #102a43;
        }

        .bio-quick-actions {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 8px;
        }

        .bio-quick-actions .btn {
            font-weight: 700;
        }

        .bio-card-head {
            padding: 14px 16px 8px;
            border-bottom: 1px solid #e6eef7;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            flex-wrap: wrap;
        }

        .bio-card-title {
            margin: 0;
            font-size: 20px;
            font-weight: 800;
            color: #102a43;
        }

        .bio-card-sub {
            color: #627d98;
            font-size: 13px;
        }

        .bio-card-body {
            padding: 14px 16px 16px;
        }

        .bio-contact-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(180px, 1fr));
            gap: 10px;
        }

        .bio-contact-item {
            border: 1px solid #dce7f2;
            border-radius: 12px;
            padding: 10px 12px;
            background: #f8fbff;
            display: flex;
            align-items: flex-start;
            gap: 10px;
            color: #102a43;
            min-height: 66px;
        }

        .bio-contact-item i {
            font-size: 18px;
            color: var(--profile-accent);
            margin-top: 1px;
        }

        .bio-contact-label {
            font-size: 12px;
            color: #627d98;
            line-height: 1.1;
            margin-bottom: 4px;
        }

        .bio-contact-value {
            font-size: 14px;
            font-weight: 700;
            line-height: 1.35;
            word-break: break-word;
        }

        .bio-link-list {
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .bio-link-item {
            width: 100%;
            border: 1px solid #dce7f2;
            border-radius: 14px;
            background: #f9fcff;
            color: #102a43;
            text-decoration: none;
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 10px 12px;
            transition: .16s ease;
        }

        .bio-link-item:hover {
            color: #0b3f65;
            border-color: #b8d1ea;
            background: #f1f8ff;
            transform: translateY(-1px);
        }

        .bio-link-icon {
            width: 42px;
            height: 42px;
            border-radius: 10px;
            object-fit: cover;
            border: 1px solid #dce7f2;
            background: #fff;
        }

        .bio-link-icon.empty {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            color: var(--profile-accent);
            font-size: 18px;
        }

        .bio-link-body {
            min-width: 0;
            flex: 1;
        }

        .bio-link-title {
            font-size: 15px;
            font-weight: 800;
            line-height: 1.2;
            margin-bottom: 0;
        }

        .bio-link-url {
            font-size: 13px;
            color: #627d98;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
            display: none;
        }

        .bio-link-arrow {
            color: #4f6f8f;
            font-size: 18px;
        }

        .bio-empty {
            border: 1px dashed #c4d5e7;
            border-radius: 14px;
            background: #f8fbff;
            padding: 18px;
            color: #627d98;
            text-align: center;
            font-size: 14px;
        }

        .bio-review-item {
            border: 1px solid #deebf7;
            border-radius: 14px;
            padding: 12px;
            background: #fff;
            margin-bottom: 12px;
        }

        .bio-review-head {
            display: flex;
            align-items: center;
            gap: 10px;
            margin-bottom: 8px;
        }

        .bio-review-avatar {
            width: 46px;
            height: 46px;
            border-radius: 50%;
            object-fit: cover;
            border: 1px solid #dbe7f3;
            background: #f1f5f9;
        }

        .bio-review-title {
            font-size: 15px;
            font-weight: 800;
            color: #102a43;
        }

        .bio-review-meta {
            font-size: 12px;
            color: #627d98;
        }

        .bio-review-stars {
            color: #f59f00;
            font-size: 16px;
            margin-bottom: 6px;
        }

        .bio-review-content {
            color: #334e68;
            font-size: 14px;
            line-height: 1.5;
        }

        .bio-lead-item {
            border: 1px solid #deebf7;
            border-radius: 14px;
            padding: 12px;
            background: #fff;
            margin-bottom: 12px;
        }

        .bio-lead-head {
            display: flex;
            justify-content: space-between;
            gap: 10px;
            margin-bottom: 6px;
            flex-wrap: wrap;
        }

        .bio-lead-name {
            font-weight: 800;
            color: #102a43;
        }

        .bio-lead-date {
            font-size: 12px;
            color: #627d98;
        }

        .bio-lead-meta {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            font-size: 13px;
            color: #334e68;
        }

        .bio-lead-meta i {
            margin-right: 6px;
            color: var(--profile-accent);
        }

        .timeline-composer {
            border: 1px solid #e2e8f0;
            border-radius: 16px;
            padding: 12px;
            background: #ffffff;
            box-shadow: 0 10px 24px rgba(15, 23, 42, .06);
            margin-bottom: 14px;
        }

        .timeline-composer textarea.form-control {
            min-height: 110px;
            resize: vertical;
        }

        .timeline-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            align-items: center;
            justify-content: space-between;
            margin-top: 10px;
        }

        .timeline-preview {
            margin-top: 10px;
            border-radius: 12px;
            max-height: 320px;
            width: 100%;
            object-fit: cover;
            border: 1px solid #e2e8f0;
            display: none;
        }

        .timeline-post {
            border: 1px solid #e2e8f0;
            border-radius: 16px;
            padding: 12px;
            background: #ffffff;
            box-shadow: 0 10px 24px rgba(15, 23, 42, .06);
            margin-bottom: 12px;
        }

        .timeline-post-head {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            flex-wrap: wrap;
        }

        .timeline-post-user {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .timeline-avatar {
            width: 44px;
            height: 44px;
            border-radius: 50%;
            object-fit: cover;
            border: 2px solid #ffffff;
            box-shadow: 0 6px 16px rgba(15, 23, 42, .12);
            background: #f1f5f9;
        }

        .timeline-post-name {
            font-weight: 800;
            color: #0f172a;
        }

        .timeline-post-time {
            font-size: 12px;
            color: #64748b;
        }

        .timeline-post-content {
            margin-top: 8px;
            color: #334e68;
            line-height: 1.6;
            white-space: pre-line;
        }

        .timeline-post-image {
            width: 100%;
            border-radius: 12px;
            margin-top: 10px;
            max-height: 420px;
            object-fit: cover;
            border: 1px solid #e2e8f0;
        }

        .timeline-comments {
            margin-top: 12px;
            border-top: 1px solid #e2e8f0;
            padding-top: 12px;
        }

        .timeline-comment-count {
            font-size: 13px;
            color: #64748b;
            margin-bottom: 8px;
        }

        .timeline-comment {
            display: flex;
            gap: 10px;
            margin-bottom: 10px;
        }

        .timeline-comment-avatar img {
            width: 34px;
            height: 34px;
            border-radius: 50%;
            object-fit: cover;
            border: 1px solid #e2e8f0;
            background: #f1f5f9;
        }

        .timeline-comment-body {
            flex: 1;
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            padding: 8px 10px;
        }

        .timeline-comment-head {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 8px;
            margin-bottom: 4px;
        }

        .timeline-comment-name {
            font-weight: 700;
            color: #0f172a;
            text-decoration: none;
        }

        .timeline-comment-name:hover {
            text-decoration: underline;
        }

        .timeline-comment-time {
            font-size: 11px;
            color: #94a3b8;
        }

        .timeline-comment-content {
            color: #334e68;
            line-height: 1.5;
            font-size: 14px;
        }

        .timeline-comment-delete {
            display: inline-block;
            margin-top: 6px;
            font-size: 12px;
            color: #ef4444;
            text-decoration: none;
        }

        .timeline-comment-delete:hover {
            text-decoration: underline;
        }

        .timeline-comment-form {
            display: flex;
            gap: 8px;
            align-items: center;
            margin-top: 8px;
        }

        .timeline-comment-form .form-control {
            flex: 1;
            min-height: 38px;
        }

        .timeline-comment-login {
            font-size: 13px;
            color: #64748b;
            margin-top: 6px;
        }

        html[data-bs-theme="dark"] .timeline-composer,
        html[data-bs-theme="dark"] .timeline-post {
            background: #0f172a;
            border-color: #223246;
            box-shadow: 0 12px 28px rgba(0, 0, 0, .4);
        }

        html[data-bs-theme="dark"] .timeline-post-name {
            color: #e2e8f0;
        }

        html[data-bs-theme="dark"] .timeline-post-time,
        html[data-bs-theme="dark"] .timeline-post-content {
            color: #94a3b8;
        }

        html[data-bs-theme="dark"] .timeline-preview,
        html[data-bs-theme="dark"] .timeline-post-image {
            border-color: #223246;
        }

        html[data-bs-theme="dark"] .timeline-comments {
            border-color: #223246;
        }

        html[data-bs-theme="dark"] .timeline-comment-body {
            background: #0b1220;
            border-color: #223246;
        }

        html[data-bs-theme="dark"] .timeline-comment-name {
            color: #e2e8f0;
        }

        html[data-bs-theme="dark"] .timeline-comment-content,
        html[data-bs-theme="dark"] .timeline-comment-time {
            color: #94a3b8;
        }

        .bio-public-shell.template-classic::before {
            background:
                radial-gradient(420px 200px at 12% -10%, rgba(34,197,94,.12), transparent 70%),
                radial-gradient(520px 260px at 88% 6%, rgba(14,165,233,.08), transparent 70%);
        }

        .bio-public-shell.template-classic .bio-hero {
            background: #ffffff;
            border: 1px solid #e2e8f0;
            box-shadow: 0 18px 40px rgba(15, 23, 42, .08);
        }

        .bio-public-shell.template-classic .bio-hero::after {
            background: none;
        }

        .bio-public-shell.template-classic .bio-hero-content {
            color: #0f172a;
        }

        .bio-public-shell.template-classic .bio-name {
            color: #0f172a;
            text-shadow: none;
        }

        .bio-public-shell.template-classic .bio-account,
        .bio-public-shell.template-classic .bio-role,
        .bio-public-shell.template-classic .bio-intro {
            color: #475569;
        }

        .bio-public-shell.template-classic .bio-chip {
            background: #ffffff;
            color: #0f172a;
            border: 1px solid #e2e8f0;
            backdrop-filter: none;
        }

        .bio-public-shell.template-classic .bio-action-btn {
            background: #ffffff;
            color: #0f172a;
            border: 1px solid #e2e8f0;
            box-shadow: none;
        }

        .bio-public-shell.template-classic .bio-action-btn:hover {
            border-color: var(--profile-accent);
            color: var(--profile-accent);
        }

        .bio-public-shell.template-classic .bio-action-btn.bio-action-solid {
            color: #ffffff;
            border-color: var(--profile-accent);
            background: var(--profile-accent);
        }

        .bio-public-shell.template-classic .bio-hero-stat {
            background: #f8fbff;
            color: #0f172a;
            border: 1px solid #e2e8f0;
        }

        .bio-public-shell.template-classic .bio-hero-stat-label {
            color: #64748b;
        }

        .bio-public-shell.template-classic .bio-nav a {
            box-shadow: none;
        }

        .bio-public-shell.template-pro {
            color: #0f172a;
        }

        .bio-public-shell.template-pro::before {
            background:
                radial-gradient(620px 260px at 12% -10%, rgba(34,197,94,.16), transparent 70%),
                radial-gradient(660px 300px at 88% -6%, rgba(14,165,233,.14), transparent 70%),
                radial-gradient(640px 360px at 50% 92%, rgba(56,189,248,.12), transparent 72%);
        }

        .bio-public-shell.template-pro .bio-hero {
            background:
                radial-gradient(820px 320px at 10% -30%, rgba(34,197,94,.3), transparent 60%),
                radial-gradient(900px 300px at 90% -35%, rgba(14,165,233,.28), transparent 60%),
                linear-gradient(135deg, #f8fafc 0%, #ffffff 55%, #ecfeff 100%);
            min-height: 230px;
            border-radius: 26px;
            border: 1px solid #e2e8f0;
            box-shadow: 0 24px 60px rgba(15, 23, 42, .12);
            background-size: 140% 140%;
            animation: proGlow 12s ease-in-out infinite alternate;
        }

        .bio-public-shell.template-pro .bio-hero::before {
            content: "";
            position: absolute;
            inset: 0;
            background-image:
                linear-gradient(90deg, rgba(148,163,184,.14) 1px, transparent 1px),
                linear-gradient(180deg, rgba(148,163,184,.14) 1px, transparent 1px);
            background-size: 42px 42px;
            opacity: .35;
            pointer-events: none;
        }

        .bio-public-shell.template-pro .bio-hero::after {
            background: linear-gradient(180deg, rgba(255,255,255,.2), rgba(255,255,255,.72));
        }

        .bio-public-shell.template-pro .bio-hero-content {
            align-items: flex-start;
            text-align: left;
            padding: 20px 26px 24px;
        }

        .bio-public-shell.template-pro .bio-hero-main {
            flex-direction: row;
            align-items: center;
            justify-content: flex-start;
            gap: 20px;
        }

        .bio-public-shell.template-pro .bio-hero-text {
            align-items: flex-start;
            text-align: left;
        }

        .bio-public-shell.template-pro .bio-name {
            font-size: 30px;
            color: #0f172a;
            text-shadow: none;
        }

        .bio-public-shell.template-pro .bio-avatar {
            width: 96px;
            height: 96px;
            border-width: 3px;
        }

        .bio-public-shell.template-pro .bio-actions {
            justify-content: flex-start;
            background: rgba(255,255,255,.85);
            border: 1px solid #e2e8f0;
            padding: 10px 12px;
            border-radius: 16px;
        }

        .bio-public-shell.template-pro .bio-chip {
            background: rgba(255,255,255,.9);
            border: 1px solid #e2e8f0;
            color: #0f172a;
        }

        .bio-public-shell.template-pro .bio-action-btn {
            background: #ffffff;
            border: 1px solid #e2e8f0;
            color: #0f172a;
        }

        .bio-public-shell.template-pro .bio-action-btn:hover {
            border-color: var(--profile-accent);
            color: var(--profile-accent);
        }

        .bio-public-shell.template-pro .bio-action-btn.bio-action-solid {
            border-color: var(--profile-accent);
            color: #ffffff;
        }

        .bio-public-shell.template-pro .bio-hero-stats {
            justify-content: flex-start;
        }

        .bio-public-shell.template-pro .bio-hero-stat {
            background: rgba(255,255,255,.9);
            border: 1px solid #e2e8f0;
            color: #0f172a;
        }

        .bio-public-shell.template-pro .bio-hero-stat-label {
            color: #64748b;
        }

        .bio-public-shell.template-pro .bio-nav {
            position: sticky;
            top: 8px;
            z-index: 6;
            background: rgba(255,255,255,.9);
            border: 1px solid #e2e8f0;
            border-radius: 16px;
            padding: 10px;
            box-shadow: 0 16px 30px rgba(15, 23, 42, .08);
        }

        .bio-public-shell.template-pro .bio-nav a {
            background: #ffffff;
            border: 1px solid #e2e8f0;
            color: #0f172a;
            box-shadow: none;
        }

        .bio-public-shell.template-pro .bio-nav a:hover {
            color: var(--profile-accent);
        }

        .bio-public-shell.template-pro .bio-layout {
            display: grid;
            grid-template-columns: minmax(0, 1.35fr) minmax(0, .85fr);
            gap: 18px;
            align-items: start;
        }

        .bio-public-shell.template-pro .bio-side {
            position: sticky;
            top: 90px;
        }

        @keyframes proGlow {
            0% { background-position: 0% 0%; }
            100% { background-position: 100% 100%; }
        }

        .bio-public-shell.template-creator {
            font-family: inherit;
        }

        .bio-public-shell.template-creator::before {
            background:
                radial-gradient(520px 260px at 12% -12%, rgba(14,165,233,.18), transparent 70%),
                radial-gradient(640px 280px at 88% -10%, rgba(34,197,94,.2), transparent 70%),
                radial-gradient(720px 360px at 50% 92%, rgba(245,158,11,.18), transparent 72%);
        }

        .bio-public-shell.template-creator .bio-hero {
            background:
                radial-gradient(900px 320px at 15% -30%, rgba(255,255,255,.25), transparent 60%),
                radial-gradient(960px 360px at 85% -40%, rgba(255,255,255,.2), transparent 60%),
                linear-gradient(120deg, #0ea5e9 0%, #22c55e 52%, #f59e0b 100%);
            min-height: 260px;
            border-radius: 30px;
            box-shadow: 0 24px 60px rgba(14, 165, 233, .25);
            background-size: 160% 160%;
            animation: creatorShift 14s ease-in-out infinite alternate;
        }

        .bio-public-shell.template-creator .bio-hero::before {
            content: "";
            position: absolute;
            inset: 0;
            background-image: linear-gradient(120deg, rgba(255,255,255,.26), rgba(255,255,255,0) 55%);
            mix-blend-mode: screen;
            opacity: .65;
            pointer-events: none;
        }

        .bio-public-shell.template-creator .bio-hero-content {
            min-height: 260px;
            padding: 22px 22px 26px;
        }

        .bio-public-shell.template-creator .bio-hero-main {
            gap: 14px;
        }

        .bio-public-shell.template-creator .bio-name {
            font-size: 38px;
        }

        .bio-public-shell.template-creator .bio-actions {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
            gap: 10px;
            background: rgba(255,255,255,.9);
            border: 1px solid rgba(226,232,240,.9);
            padding: 12px;
            border-radius: 18px;
            box-shadow: 0 12px 26px rgba(15, 23, 42, .12);
        }

        .bio-public-shell.template-creator .bio-action-btn {
            border-radius: 14px;
            border-color: rgba(226, 232, 240, .9);
            background: #f8fafc;
            color: #0f172a;
        }

        .bio-public-shell.template-creator .bio-action-btn.bio-action-solid {
            background: linear-gradient(120deg, var(--profile-accent), #16a34a);
            border-color: transparent;
            color: #ffffff;
        }

        .bio-public-shell.template-creator .bio-hero-stat {
            background: rgba(255,255,255,.85);
            border: 1px solid rgba(255,255,255,.7);
            color: #0f172a;
        }

        .bio-public-shell.template-creator .bio-hero-stat-label {
            color: #475569;
        }

        .bio-public-shell.template-creator .bio-hero-stats {
            gap: 12px;
        }

        .bio-public-shell.template-creator .bio-nav {
            background: #ffffff;
            border: 1px solid rgba(226, 232, 240, .95);
            border-radius: 999px;
            padding: 8px 10px;
            box-shadow: 0 12px 26px rgba(15, 23, 42, .08);
        }

        .bio-public-shell.template-creator .bio-nav a {
            background: transparent;
            border: 0;
            box-shadow: none;
            color: #0f172a;
        }

        .bio-public-shell.template-creator .bio-nav a:hover {
            background: rgba(34, 197, 94, .12);
        }

        .bio-public-shell.template-creator .bio-card {
            border-radius: 26px;
            border: 1px solid transparent;
            background:
                linear-gradient(#ffffff, #ffffff) padding-box,
                linear-gradient(120deg, rgba(34,197,94,.35), rgba(14,165,233,.35)) border-box;
            box-shadow: 0 18px 40px rgba(15, 23, 42, .12);
        }

        .bio-public-shell.template-creator .bio-card-head {
            border-bottom: 0;
            padding-bottom: 4px;
        }

        .bio-public-shell.template-creator .bio-layout {
            display: flex;
            gap: 20px;
            align-items: flex-start;
        }

        .bio-public-shell.template-creator .bio-main {
            flex: 1.2;
        }

        .bio-public-shell.template-creator .bio-side {
            flex: .8;
            order: -1;
            position: sticky;
            top: 82px;
        }

        @keyframes creatorShift {
            0% { background-position: 0% 0%; }
            100% { background-position: 100% 100%; }
        }

        @media (prefers-reduced-motion: reduce) {
            .bio-public-shell.template-pro .bio-hero,
            .bio-public-shell.template-creator .bio-hero {
                animation: none;
            }
        }

        /* template-pro + template-creator will follow site light/dark theme */

        .bio-lead-note {
            margin-top: 8px;
            font-size: 13px;
            color: #52606d;
        }

        .review-img {
            cursor: pointer;
            border-radius: 10px;
            object-fit: cover;
            margin-top: 8px;
        }

        .pagination a, .pagination .current-page {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 34px;
            height: 34px;
            padding: 0 10px;
            margin: 2px;
            border: 1px solid rgba(98,105,118,.3);
            border-radius: 8px;
            text-decoration: none;
            color: var(--tblr-body-color, #1f2937);
            background: #fff;
        }

        .pagination a:hover { background: rgba(98,105,118,.06); }

        .pagination .current-page {
            font-weight: 700;
            background: rgba(47,179,68,.12);
            border-color: rgba(47,179,68,.35);
            color: rgb(47,179,68);
            cursor: default;
        }

        html[data-bs-theme="dark"] .square-container {
            background: #111d34;
        }

        html[data-bs-theme="dark"] .bio-avatar {
            background: #0f172a;
        }

        html[data-bs-theme="dark"] .bio-card {
            border-color: #223246;
            background: #0f172a;
            box-shadow: 0 14px 34px rgba(0, 0, 0, .45);
        }

        html[data-bs-theme="dark"] .bio-card-head {
            border-bottom-color: #223246;
        }

        html[data-bs-theme="dark"] .bio-card-title {
            color: #e5e7eb;
        }

        html[data-bs-theme="dark"] .bio-card-sub,
        html[data-bs-theme="dark"] .bio-contact-label,
        html[data-bs-theme="dark"] .bio-link-url,
        html[data-bs-theme="dark"] .bio-review-meta,
        html[data-bs-theme="dark"] .bio-review-content {
            color: #94a3b8;
        }

        html[data-bs-theme="dark"] .bio-contact-item {
            background: #111d34;
            border-color: #223246;
            color: #e5e7eb;
        }

        html[data-bs-theme="dark"] .bio-contact-item i {
            color: #34d399;
        }

        html[data-bs-theme="dark"] .bio-link-item {
            background: #111d34;
            border-color: #223246;
            color: #e5e7eb;
        }

        html[data-bs-theme="dark"] .bio-link-item:hover {
            background: #16253a;
            border-color: #334155;
            color: #ffffff;
        }

        html[data-bs-theme="dark"] .bio-link-icon {
            background: #0f172a;
            border-color: #223246;
        }

        html[data-bs-theme="dark"] .bio-link-icon.empty {
            color: #94a3b8;
        }

        html[data-bs-theme="dark"] .bio-link-arrow {
            color: #94a3b8;
        }

        html[data-bs-theme="dark"] .bio-empty {
            background: #111d34;
            border-color: #223246;
            color: #94a3b8;
        }

        html[data-bs-theme="dark"] .bio-review-item {
            background: #0f172a;
            border-color: #223246;
        }

        html[data-bs-theme="dark"] .bio-review-title {
            color: #e5e7eb;
        }

        html[data-bs-theme="dark"] .bio-review-avatar {
            background: #0b1220;
            border-color: #223246;
        }

        html[data-bs-theme="dark"] .bio-stat {
            background: #111d34;
            border-color: #223246;
        }

        html[data-bs-theme="dark"] .bio-stat-label {
            color: #94a3b8;
        }

        html[data-bs-theme="dark"] .bio-stat-value {
            color: #e2e8f0;
        }

        html[data-bs-theme="dark"] .pagination a,
        html[data-bs-theme="dark"] .pagination .current-page {
            background: #0f172a;
            border-color: #334155;
            color: #e5e7eb;
        }

        html[data-bs-theme="dark"] .pagination a:hover {
            background: #162235;
        }

        html[data-bs-theme="dark"] .pagination .current-page {
            background: rgba(34, 197, 94, 0.18);
            border-color: rgba(34, 197, 94, 0.5);
            color: #86efac;
        }

        /* Dark mode overrides for templates */
        html[data-bs-theme="dark"] .bio-public-shell.template-classic .bio-hero {
            background: #0f172a;
            border-color: #223246;
            box-shadow: 0 18px 40px rgba(0, 0, 0, .45);
        }

        html[data-bs-theme="dark"] .bio-public-shell.template-classic .bio-hero-content,
        html[data-bs-theme="dark"] .bio-public-shell.template-classic .bio-name {
            color: #e2e8f0;
        }

        html[data-bs-theme="dark"] .bio-public-shell.template-classic .bio-account,
        html[data-bs-theme="dark"] .bio-public-shell.template-classic .bio-role,
        html[data-bs-theme="dark"] .bio-public-shell.template-classic .bio-intro,
        html[data-bs-theme="dark"] .bio-public-shell.template-classic .bio-hero-stat-label {
            color: #94a3b8;
        }

        html[data-bs-theme="dark"] .bio-public-shell.template-classic .bio-chip,
        html[data-bs-theme="dark"] .bio-public-shell.template-classic .bio-action-btn,
        html[data-bs-theme="dark"] .bio-public-shell.template-classic .bio-hero-stat,
        html[data-bs-theme="dark"] .bio-public-shell.template-classic .bio-nav a {
            background: #111d34;
            border-color: #223246;
            color: #e2e8f0;
        }

        html[data-bs-theme="dark"] .bio-public-shell.template-classic .bio-action-btn:hover {
            border-color: var(--profile-accent);
            color: #ffffff;
        }

        html[data-bs-theme="dark"] .bio-public-shell.template-pro {
            color: #e2e8f0;
        }

        html[data-bs-theme="dark"] .bio-public-shell.template-pro .bio-hero {
            background:
                radial-gradient(760px 320px at 10% -30%, rgba(34,197,94,.35), transparent 60%),
                radial-gradient(880px 300px at 90% -40%, rgba(14,165,233,.28), transparent 60%),
                linear-gradient(135deg, #0b1220 0%, #0f172a 55%, #052e1f 100%);
            border-color: #223246;
            box-shadow: 0 30px 70px rgba(0, 0, 0, .55);
        }

        html[data-bs-theme="dark"] .bio-public-shell.template-pro .bio-hero::before {
            background-image:
                linear-gradient(90deg, rgba(148,163,184,.12) 1px, transparent 1px),
                linear-gradient(180deg, rgba(148,163,184,.12) 1px, transparent 1px);
            opacity: .35;
        }

        html[data-bs-theme="dark"] .bio-public-shell.template-pro .bio-hero::after {
            background: linear-gradient(180deg, rgba(2, 6, 23, .2), rgba(2, 6, 23, .8));
        }

        html[data-bs-theme="dark"] .bio-public-shell.template-pro .bio-actions,
        html[data-bs-theme="dark"] .bio-public-shell.template-pro .bio-action-btn,
        html[data-bs-theme="dark"] .bio-public-shell.template-pro .bio-chip,
        html[data-bs-theme="dark"] .bio-public-shell.template-pro .bio-nav a {
            background: rgba(15, 23, 42, .85);
            border-color: rgba(148, 163, 184, .25);
            color: #e2e8f0;
        }

        html[data-bs-theme="dark"] .bio-public-shell.template-pro .bio-nav {
            background: rgba(2, 6, 23, .75);
            border-color: rgba(148, 163, 184, .2);
        }

        html[data-bs-theme="dark"] .bio-public-shell.template-pro .bio-card,
        html[data-bs-theme="dark"] .bio-public-shell.template-pro .bio-contact-item,
        html[data-bs-theme="dark"] .bio-public-shell.template-pro .bio-link-item,
        html[data-bs-theme="dark"] .bio-public-shell.template-pro .bio-stat,
        html[data-bs-theme="dark"] .bio-public-shell.template-pro .bio-review-item,
        html[data-bs-theme="dark"] .bio-public-shell.template-pro .bio-lead-item {
            background: #0f172a;
            border-color: #223246;
            color: #e2e8f0;
        }

        html[data-bs-theme="dark"] .bio-public-shell.template-creator .bio-hero {
            background:
                radial-gradient(900px 320px at 15% -30%, rgba(34,197,94,.25), transparent 60%),
                radial-gradient(960px 360px at 85% -40%, rgba(14,165,233,.2), transparent 60%),
                linear-gradient(120deg, #0b1220 0%, #052e1f 55%, #0f172a 100%);
            box-shadow: 0 24px 60px rgba(0, 0, 0, .55);
        }

        html[data-bs-theme="dark"] .bio-public-shell.template-creator .bio-actions,
        html[data-bs-theme="dark"] .bio-public-shell.template-creator .bio-action-btn,
        html[data-bs-theme="dark"] .bio-public-shell.template-creator .bio-nav {
            background: rgba(15, 23, 42, .7);
            border-color: #223246;
            color: #e2e8f0;
        }

        html[data-bs-theme="dark"] .bio-public-shell.template-creator .bio-nav a {
            color: #e2e8f0;
        }

        html[data-bs-theme="dark"] .bio-public-shell.template-creator .bio-card,
        html[data-bs-theme="dark"] .bio-public-shell.template-creator .bio-hero-stat {
            background: #0f172a;
            border-color: #223246;
            color: #e2e8f0;
        }

        html[data-bs-theme="dark"] .profile-hero {
            background: linear-gradient(135deg, #0b3d26, #1f7a4b 60%, #2fb344);
        }

        .bio-public-shell .profile-hero .bio-action-btn {
            border-color: rgba(255,255,255,.55) !important;
            background: rgba(255,255,255,.16) !important;
            color: #ffffff !important;
        }

        .bio-public-shell .profile-hero .bio-action-btn:hover {
            border-color: rgba(255,255,255,.85) !important;
            background: rgba(255,255,255,.28) !important;
            color: #ffffff !important;
        }

        .bio-public-shell .profile-hero .bio-action-btn.bio-action-solid {
            background: #ffffff !important;
            border-color: #ffffff !important;
            color: #1f7a43 !important;
        }

        @media (max-width: 767.98px) {
            .bio-public-shell {
                margin-top: 10px;
                padding: 0 10px;
            }

            .bio-hero {
                min-height: 210px;
                border-radius: 20px;
            }

            .bio-hero-content {
                min-height: 210px;
                padding: 16px 12px 16px;
            }

            .bio-avatar {
                width: 90px;
                height: 90px;
            }

            .bio-name {
                font-size: 28px;
            }

            .bio-contact-grid {
                grid-template-columns: 1fr;
            }

            .bio-link-item {
                padding: 10px;
            }

            .bio-link-title {
                font-size: 14px;
            }

            .bio-link-arrow {
                font-size: 16px;
            }

            .bio-public-shell.template-pro .bio-hero-content {
                align-items: center;
                text-align: center;
            }

            .bio-public-shell.template-pro .bio-actions {
                justify-content: center;
            }

            .bio-public-shell.template-pro .bio-hero-main {
                flex-direction: column;
                align-items: center;
            }

            .bio-public-shell.template-pro .bio-hero-text {
                align-items: center;
                text-align: center;
            }

            .bio-public-shell.template-pro .bio-layout,
            .bio-public-shell.template-creator .bio-layout {
                display: block;
            }

            .bio-public-shell.template-pro .bio-side,
            .bio-public-shell.template-creator .bio-side {
                position: static;
            }

            .bio-nav {
                justify-content: center;
            }

            .bio-public-shell.template-pro .bio-nav,
            .bio-public-shell.template-creator .bio-nav {
                position: static;
            }
        }

        .bio-public-shell.template-classic .bio-side {
            display: none;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <!-- ===================== MAIN ===================== -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="page-body">
                <div class="bio-public-shell template-<%= (ViewState["profile_template"] ?? "classic").ToString() %>" style="--profile-accent: <%= (ViewState["profile_accent"] ?? "#22c55e").ToString() %>;">
                    <div class="d-none">
                        <asp:Literal ID="Literal13" runat="server"></asp:Literal>
                    </div>

                    <section class="profile-hero">
                        <div class="profile-hero-inner">
                            <img src="<%= ViewState["avt_query"] %>" alt="avatar" class="profile-hero-avatar" />
                            <div class="profile-hero-name"><%= ViewState["hoten_query"] %></div>
                            <div class="profile-hero-user">@<%= ViewState["taikhoan_hienthi_query"] %></div>
                            <div><%= ViewState["phanloai_query"] %></div>
                            <div class="profile-hero-desc"><%= ViewState["gioithieu_query"] %></div>

                            <div class="profile-hero-actions">
                                <a href="<%= ViewState["sdt_href_query"] %>" class="bio-action-btn bio-action-solid">
                                    <i class="ti ti-phone"></i>Gọi ngay
                                </a>
                                <asp:PlaceHolder ID="phContactCta" runat="server" Visible="true">
                                    <button type="button" class="bio-action-btn" data-bs-toggle="modal" data-bs-target="#contactLeadModal">
                                        <i class="ti ti-message"></i>Liên hệ
                                    </button>
                                </asp:PlaceHolder>
                                <% if (string.IsNullOrEmpty((ViewState["taikhoan"] ?? "").ToString())) { %>
                                    <a href="<%= ViewState["link_dangky_ref"] %>" class="bio-action-btn">
                                        <i class="ti ti-user-plus"></i>Đăng ký
                                    </a>
                                <% } else { %>
                                    <a href="<%= ViewState["public_profile_url"] %>" target="_blank" class="bio-action-btn">
                                        <i class="ti ti-world"></i>Mở link hồ sơ
                                    </a>
                                <% } %>
                                <a href="<%= ViewState["link_luu_danhba"] %>" class="bio-action-btn">
                                    <i class="ti ti-id"></i>Lưu thông tin
                                </a>
                                <button type="button"
                                    class="bio-action-btn"
                                    data-profile-link="<%: (ViewState["public_profile_url"] ?? "").ToString() %>"
                                    onclick="copyProfileLink(this)">
                                    <i class="ti ti-copy"></i>Sao chép link
                                </button>
                            </div>

                            <asp:PlaceHolder ID="phHeroEditRow" runat="server" Visible="false">
                                <div class="profile-hero-actions secondary">
                                    <asp:PlaceHolder ID="phOwnerEditButton" runat="server" Visible="false">
                                        <a href="/home/edit-info.aspx" class="bio-action-btn">
                                            <i class="ti ti-edit"></i>Chỉnh sửa hồ sơ
                                        </a>
                                    </asp:PlaceHolder>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </section>

                    <nav class="bio-nav">
                        <a href="#section-info">Thông tin</a>
                        <% if ((ViewState["profile_show_social"] ?? "1").ToString() == "1") { %>
                            <a href="#section-links">Liên kết</a>
                        <% } %>
                        <% if ((ViewState["profile_show_reviews"] ?? "1").ToString() == "1") { %>
                            <a href="#section-reviews">Đánh giá</a>
                        <% } %>
                        <asp:PlaceHolder ID="phNavTimeline" runat="server" Visible="false">
                            <a href="#section-timeline">Bài viết</a>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phNavLeads" runat="server" Visible="false">
                            <a href="#section-leads">Khách hàng quan tâm</a>
                        </asp:PlaceHolder>
                        <% if ((ViewState["profile_show_shop"] ?? "0").ToString() == "1") { %>
                            <a href="#section-shop">Cửa hàng</a>
                        <% } %>
                        <% if ((ViewState["profile_show_products"] ?? "0").ToString() == "1") { %>
                            <a href="#section-products">Sản phẩm</a>
                        <% } %>
                    </nav>

                    <div class="bio-layout">
                        <div class="bio-main">
                            <section class="bio-card" id="section-info">
                                <div class="bio-card-head">
                                    <div>
                                        <h3 class="bio-card-title">Thông tin cá nhân</h3>
                                        <div class="bio-card-sub">Có <b><%= ViewState["SoLuongDanhGia"] %></b> lượt đánh giá trên hồ sơ này</div>
                                    </div>
                                </div>
                                <div class="bio-card-body">
                                    <div class="bio-contact-grid">
                                        <div class="bio-contact-item">
                                            <i class="ti ti-phone"></i>
                                            <div>
                                                <div class="bio-contact-label">Điện thoại</div>
                                                <div class="bio-contact-value"><%= ViewState["sdt_query"] %></div>
                                            </div>
                                        </div>
                                        <div class="bio-contact-item">
                                            <i class="ti ti-mail"></i>
                                            <div>
                                                <div class="bio-contact-label">Email</div>
                                                <div class="bio-contact-value"><%= ViewState["email_query"] %></div>
                                            </div>
                                        </div>
                                        <div class="bio-contact-item">
                                            <i class="ti ti-map-pin"></i>
                                            <div>
                                                <div class="bio-contact-label">Địa chỉ</div>
                                                <div class="bio-contact-value"><%= ViewState["diachi_query"] %></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </section>

                            <asp:PlaceHolder ID="phSocialLinksBlock" runat="server" Visible="true">
                                <section class="bio-card" id="section-links">
                                    <div class="bio-card-head">
                                        <div>
                                            <h3 class="bio-card-title">Liên kết cá nhân</h3>
                                            <div class="bio-card-sub">Các kênh liên hệ và mạng xã hội đã cấu hình từ trang chỉnh sửa hồ sơ</div>
                                        </div>
                                    </div>
                                    <div class="bio-card-body">
                                        <div class="bio-link-list">
                                            <asp:Repeater ID="rptMangXaHoiCN" runat="server">
                                                <ItemTemplate>
                                                    <a class="bio-link-item" target="_blank" href='<%# ResolveExternalLink(Eval("Link")) %>'>
                                                        <asp:Image ID="imgIcon" runat="server"
                                                            CssClass="bio-link-icon"
                                                            ImageUrl='<%# ResolveSocialIcon(Eval("Icon"), Eval("Link")) %>'
                                                            Visible='<%# ShouldShowIcon(Eval("Icon"), Eval("Link")) %>' />
                                                        <asp:Literal ID="litIconFallback" runat="server"
                                                            Text='<%# ShouldShowIcon(Eval("Icon"), Eval("Link")) ? "" : "<span class=\"bio-link-icon empty\"><i class=\"ti ti-world\"></i></span>" %>'></asp:Literal>
                                                        <span class="bio-link-body">
                                                            <span class="bio-link-title"><%# Eval("Ten") %></span>
                                                            <span class="bio-link-url">Nhấn để mở liên kết</span>
                                                        </span>
                                                        <i class="ti ti-arrow-up-right bio-link-arrow"></i>
                                                    </a>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                        <asp:PlaceHolder ID="phNoSocialLinks" runat="server" Visible="false">
                                            <div class="bio-empty">Chưa có liên kết cá nhân nào. Bạn có thể thêm tại mục chỉnh sửa hồ sơ.</div>
                                        </asp:PlaceHolder>
                                    </div>
                                </section>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="phTimelineSection" runat="server" Visible="true">
                                <section class="bio-card" id="section-timeline">
                                    <div class="bio-card-head">
                                        <div>
                                            <h3 class="bio-card-title">Bài viết</h3>
                                            <div class="bio-card-sub">Cập nhật và chia sẻ trên hồ sơ cá nhân</div>
                                        </div>
                                    </div>
                                    <div class="bio-card-body">
                                        <asp:PlaceHolder ID="phTimelineEditor" runat="server" Visible="false">
                                            <div class="timeline-composer">
                                                <div class="fw-bold mb-2">Tạo bài viết mới</div>
                                                <asp:TextBox ID="txtTimelineContent" runat="server" TextMode="MultiLine" Rows="4"
                                                    CssClass="form-control" placeholder="Bạn đang nghĩ gì?"></asp:TextBox>

                                                <div class="mt-2">
                                                    <input type="file" id="timelineImageInput" class="form-control" accept="image/*" onchange="uploadTimelineImage()" />
                                                    <div id="timelineImageMessage" class="text-danger small mt-1"></div>
                                                    <asp:HiddenField ID="hfTimelineImage" runat="server" />
                                                    <asp:Image ID="imgTimelinePreview" runat="server" CssClass="timeline-preview" />
                                                </div>

                                                <div class="timeline-actions">
                                                    <button type="button" class="btn btn-outline-secondary btn-sm" onclick="clearTimelineImage()">Xóa ảnh</button>
                                                    <div class="d-flex gap-2">
                                                        <asp:Button ID="btnTimelineCancel" runat="server" Text="Hủy"
                                                            CssClass="btn btn-outline-secondary btn-sm" Visible="false" OnClick="btnTimelineCancel_Click" />
                                                        <asp:Button ID="btnTimelineSave" runat="server" Text="Đăng bài"
                                                            CssClass="btn btn-primary btn-sm" OnClientClick="return prepareTimelineSubmit();" OnClick="btnTimelineSave_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>

                                        <asp:Repeater ID="rptTimeline" runat="server" OnItemCommand="rptTimeline_ItemCommand">
                                            <ItemTemplate>
                                                <div class="timeline-post">
                                                    <div class="timeline-post-head">
                                                        <div class="timeline-post-user">
                                                            <img class="timeline-avatar" src="<%# ViewState["avt_query"] %>" alt="avatar" />
                                                            <div>
                                                                <div class="timeline-post-name"><%# ViewState["hoten_query"] %></div>
                                                                <div class="timeline-post-time"><%# FormatTimelineTime(Eval("ngaytao")) %></div>
                                                            </div>
                                                        </div>
                                                        <asp:PlaceHolder ID="phTimelineActions" runat="server" Visible='<%# TimelineIsOwner %>'>
                                                            <div class="btn-group">
                                                                <asp:LinkButton ID="btnTimelineEdit" runat="server" Text="Sửa"
                                                                    CssClass="btn btn-outline-primary btn-sm"
                                                                    CommandName="edit" CommandArgument='<%# Eval("id") %>' />
                                                                <asp:LinkButton ID="btnTimelineDelete" runat="server" Text="Xóa"
                                                                    CssClass="btn btn-outline-danger btn-sm"
                                                                    CommandName="delete" CommandArgument='<%# Eval("id") %>'
                                                                    OnClientClick="return confirm('Xóa bài viết này?');" />
                                                            </div>
                                                        </asp:PlaceHolder>
                                                    </div>
                                                    <div class="timeline-post-content"><%# FormatTimelineContent(Eval("content_post")) %></div>
                                                    <asp:Image ID="imgTimelinePost" runat="server"
                                                        ImageUrl='<%# Eval("image") %>'
                                                        CssClass="timeline-post-image"
                                                        Visible='<%# !string.IsNullOrWhiteSpace(Eval("image") as string) %>' />
                                                    <div class="timeline-comments">
                                                        <asp:PlaceHolder ID="phTimelineCommentCount" runat="server" Visible='<%# (int)Eval("CommentCount") > 0 %>'>
                                                            <div class="timeline-comment-count"><%# Eval("CommentCount") %> bình luận</div>
                                                        </asp:PlaceHolder>

                                                        <asp:Repeater ID="rptTimelineComments" runat="server" DataSource='<%# Eval("Comments") %>'>
                                                            <ItemTemplate>
                                                                <div class="timeline-comment">
                                                                    <a class="timeline-comment-avatar" href="<%# Eval("ProfileUrl") %>">
                                                                        <img src="<%# Eval("Avatar") %>" alt="avatar" />
                                                                    </a>
                                                                    <div class="timeline-comment-body">
                                                                        <div class="timeline-comment-head">
                                                                            <a class="timeline-comment-name" href="<%# Eval("ProfileUrl") %>"><%# Eval("DisplayName") %></a>
                                                                            <span class="timeline-comment-time"><%# FormatTimelineTime(Eval("NgayTao")) %></span>
                                                                        </div>
                                                                        <div class="timeline-comment-content"><%# FormatTimelineContent(Eval("Content")) %></div>
                                                                        <asp:LinkButton ID="btnDeleteComment" runat="server" Text="Xóa"
                                                                            CssClass="timeline-comment-delete"
                                                                            Visible='<%# Convert.ToBoolean(Eval("CanDelete")) %>'
                                                                            CommandArgument='<%# Eval("Id") %>'
                                                                            OnCommand="DeleteTimelineComment_Command"
                                                                            OnClientClick="return confirm('Xóa bình luận này?');" />
                                                                    </div>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>

                                                        <asp:PlaceHolder ID="phTimelineCommentForm" runat="server" Visible='<%# Convert.ToBoolean(Eval("CanComment")) %>'>
                                                            <div class="timeline-comment-form">
                                                                <asp:TextBox ID="txtTimelineComment" runat="server" CssClass="form-control"
                                                                    MaxLength="500" placeholder="Viết bình luận..."></asp:TextBox>
                                                                <asp:Button ID="btnTimelineComment" runat="server" Text="Gửi"
                                                                    CssClass="btn btn-outline-primary btn-sm"
                                                                    CommandName="comment" CommandArgument='<%# Eval("id") %>' />
                                                            </div>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="phTimelineCommentLogin" runat="server" Visible='<%# !Convert.ToBoolean(Eval("CanComment")) %>'>
                                                            <div class="timeline-comment-login">Đăng nhập tài khoản home để bình luận.</div>
                                                        </asp:PlaceHolder>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                        <asp:PlaceHolder ID="phTimelineEmpty" runat="server" Visible="false">
                                            <div class="bio-empty">Chưa có bài viết nào.</div>
                                        </asp:PlaceHolder>
                                    </div>
                                </section>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="phOwnerLeads" runat="server" Visible="false">
                                <section class="bio-card" id="section-leads">
                                    <div class="bio-card-head">
                                        <div>
                                            <h3 class="bio-card-title">Khách hàng quan tâm</h3>
                                            <div class="bio-card-sub">Danh sách liên hệ gửi từ hồ sơ của bạn</div>
                                        </div>
                                    </div>
                                    <div class="bio-card-body">
                                        <asp:Repeater ID="rptContactLeads" runat="server">
                                            <ItemTemplate>
                                                <div class="bio-lead-item">
                                                    <div class="bio-lead-head">
                                                        <div class="bio-lead-name"><%# Eval("Ten") %></div>
                                                        <div class="bio-lead-date"><%# Eval("Ngay", "{0:dd/MM/yyyy HH:mm}") %></div>
                                                    </div>
                                                    <div class="bio-lead-meta">
                                                        <div><i class="ti ti-phone"></i><%# Eval("Sdt") %></div>
                                                        <div style='<%# string.IsNullOrWhiteSpace(Eval("Email") as string) ? "display:none" : "" %>'>
                                                            <i class="ti ti-mail"></i><%# Eval("Email") %>
                                                        </div>
                                                    </div>
                                                    <div class="bio-lead-note" style='<%# string.IsNullOrWhiteSpace(Eval("NoiDung") as string) ? "display:none" : "" %>'>
                                                        <%# Eval("NoiDung") %>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <asp:PlaceHolder ID="phNoContactLeads" runat="server" Visible="false">
                                            <div class="bio-empty">Chưa có liên hệ nào.</div>
                                        </asp:PlaceHolder>
                                    </div>
                                </section>
                            </asp:PlaceHolder>

                        <!-- ✅ BỌC TOÀN BỘ CỬA HÀNG: nếu chưa là gian hàng đối tác -> Visible=false -->
                        <asp:PlaceHolder ID="phCuaHang" runat="server" Visible="false">
                            <div class="col-lg-7">
                                <div class="card bio-card" id="section-shop">
                                    <div class="card-body">
                                        <div class="h3 mb-2"><i class="ti ti-building-store me-1"></i>Giới thiệu cửa hàng</div>

                                        <div class="mb-3">
                                            <asp:Literal ID="Literal6" runat="server"></asp:Literal>
                                        </div>

                                        <div class="card bg-muted-lt border-0">
                                            <div class="card-body">
                                                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                                            </div>
                                        </div>

                                        <div class="mt-3 text-muted">
                                            <div class="mb-2"><i class="ti ti-phone me-1"></i><asp:Literal ID="Literal2" runat="server"></asp:Literal></div>
                                            <div><i class="ti ti-map-pin me-1"></i><asp:Literal ID="Literal3" runat="server"></asp:Literal></div>
                                        </div>

                                        <div class="mt-4">
                                            <div class="fw-semibold mb-2">Mạng xã hội cửa hàng</div>
                                            <asp:Repeater ID="rptMangXaHoiCH" runat="server">
                                                <ItemTemplate>
                                                    <div class="d-flex align-items-center mb-3">
                                                        <asp:Image ID="imgIcon" runat="server"
                                                            ImageUrl='<%# ResolveSocialIcon(Eval("Icon"), Eval("Link")) %>'
                                                            Width="42" Height="42"
                                                            Style="object-fit: cover; border-radius: 10px; margin-right: 10px;"
                                                            Visible='<%# ShouldShowIcon(Eval("Icon"), Eval("Link")) %>' />
                                                        <div style='<%# GetMarginStyle(Eval("Icon"), Eval("Link")) %>' class="flex-fill">
                                                            <div class="fw-semibold"><%# Eval("Ten") %></div>
                                                            <div class="text-muted small text-truncate"><%# ResolveExternalLinkLabel(Eval("Link")) %></div>
                                                        </div>
                                                        <a class="btn btn-sm btn-outline-secondary" target="_blank" href='<%# ResolveExternalLink(Eval("Link")) %>'>
                                                            <i class="ti ti-external-link"></i>
                                                        </a>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                    <!-- ✅ BỌC TOÀN BỘ SẢN PHẨM CỬA HÀNG -->
                    <asp:PlaceHolder ID="phSanPhamCuaHang" runat="server" Visible="false">

                        <!-- Products -->
                        <div class="card mt-4" id="section-products">
                            <div class="card-header">
                                <div>
                                    <div class="card-title">Sản phẩm đang bán</div>
                                    <div class="text-muted small">Danh sách sản phẩm của cửa hàng</div>
                                </div>

                                <div class="ms-auto d-flex align-items-center gap-2">
                                    <div class="text-muted small d-none d-md-block">
                                        <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                                    </div>

                                    <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                                        <i class="ti ti-chevron-left"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                                        <i class="ti ti-chevron-right"></i>
                                    </asp:LinkButton>
                                </div>
                            </div>

                            <div class="card-body">
                                <div class="row g-2 align-items-center mb-3">
                                    <div class="col-md-6">
                                        <asp:TextBox MaxLength="50" ID="txt_search" runat="server" CssClass="form-control"
                                            AutoPostBack="true" placeholder="Nhập từ khóa..." OnTextChanged="txt_search_TextChanged"></asp:TextBox>
                                    </div>
                                    <div class="col-md-6" style="display:none">
                                        <asp:TextBox MaxLength="50" ID="txt_timkiem1" runat="server" CssClass="form-control"
                                            AutoPostBack="true" placeholder="Nhập từ khóa..." OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                                    </div>

                                    <!-- giữ control mobile cũ (không phá code phân trang) -->
                                    <div class="d-none">
                                        <asp:Label ID="lb_show_md" runat="server" Text=""></asp:Label>
                                        <asp:LinkButton ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" />
                                        <asp:LinkButton ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" />
                                    </div>
                                </div>

                                <div class="card bg-muted-lt border-0 mb-3">
                                    <div class="card-body">
                                        <div class="row">
                                            <div class="col-6">Sản phẩm: <b><asp:Literal ID="Literal7" runat="server"></asp:Literal></b></div>
                                            <div class="col-6">Đã bán: <b><asp:Literal ID="Literal12" runat="server"></asp:Literal></b></div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row g-3">

                                    <!-- Left column products -->
                                    <div class="col-lg-6">
                                        <div class="row g-3">
                                            <asp:Repeater ID="Repeater2" runat="server" OnItemDataBound="Repeater2_ItemDataBound">
                                                <ItemTemplate>
                                                    <div class="col-6">
                                                        <div class="card h-100">
                                                            <a class="text-decoration-none" href="/<%#Eval("name_en") %>-<%#Eval("id") %>.html">
                                                                <div class="square-container">
                                                                    <img src="<%# string.IsNullOrWhiteSpace((Eval("image") + "").Trim()) ? "/uploads/images/macdinh.jpg" : (Eval("image") + "").Trim() %>" alt="<%# Eval("name") %>" />
                                                                </div>
                                                            </a>

                                                            <div class="card-body p-2">
                                                                <div class="text-clamp-2 fw-semibold mb-1">
                                                                    <small><%# Eval("name") %></small>
                                                                </div>
                                                                <div class="text-clamp-2 text-muted">
                                                                    <small><%# Eval("description") %></small>
                                                                </div>

                                                                <div class="mt-2 fw-bold">
                                                                    <small><%# Eval("giaban", "{0:#,##0.##}") %> đ</small>
                                                                </div>

                                                                <!-- BADGE phân biệt SP -->
                                                                <div class="mt-2">
                                                                    <asp:Literal ID="lit_badge" runat="server"></asp:Literal>
                                                                </div>

                                                                <div class="mt-2 text-muted" style="font-size: 12px;">
                                                                    <div class="d-flex justify-content-between">
                                                                        <span><%# Eval("ngaytao", "{0:dd/MM/yyyy HH:mm}") %></span>
                                                                        <span>Lượt xem: <%# Eval("LuotTruyCap", "{0:#,##0}") %></span>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div class="card-footer p-2">
                                                                <div class="row g-2">
                                                                    <div class="col-6 d-none">
                                                                        <asp:Button ID="but_bansanphamnay" Width="100%" OnClick="but_bansanphamnay_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Bán chéo"
                                                                            CssClass="btn btn-sm btn-outline-warning w-100" Visible="false" />
                                                                    </div>
                                                                    <div class="col-6">
                                                                        <asp:Button ID="but_traodoi" Width="100%" OnClick="but_traodoi_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Trao đổi ngay"
                                                                            CssClass="btn btn-sm btn-primary w-100" />
                                                                    </div>
                                                                    <div class="col-12">
                                                                        <asp:Button ID="but_themvaogio" Width="100%" OnClick="but_themvaogio_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Thêm vào giỏ hàng"
                                                                            CssClass="btn btn-sm btn-outline-secondary w-100" />
                                                                    </div>

                                                                    <!-- NÚT HỦY BÁN CHÉO (chỉ hiện khi SP bán chéo & đang xem shop của chính mình) -->
                                                                    <div class="col-12">
                                                                        <asp:Button ID="but_huy_bancheo" Width="100%" OnClick="but_huy_bancheo_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Hủy bán chéo"
                                                                            CssClass="btn btn-sm btn-outline-danger w-100" Visible="false" />
                                                                    </div>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>

                                    <!-- Right column products -->
                                    <div class="col-lg-6">
                                        <div class="row g-3">
                                            <asp:Repeater ID="Repeater4" runat="server" OnItemDataBound="Repeater2_ItemDataBound">
                                                <ItemTemplate>
                                                    <div class="col-6">
                                                        <div class="card h-100">
                                                            <a class="text-decoration-none" href="/<%#Eval("name_en") %>-<%#Eval("id") %>.html">
                                                                <div class="square-container">
                                                                    <img src="<%# string.IsNullOrWhiteSpace((Eval("image") + "").Trim()) ? "/uploads/images/macdinh.jpg" : (Eval("image") + "").Trim() %>" alt="<%# Eval("name") %>" />
                                                                </div>
                                                            </a>

                                                            <div class="card-body p-2">
                                                                <div class="text-clamp-2 fw-semibold mb-1">
                                                                    <small><%# Eval("name") %></small>
                                                                </div>
                                                                <div class="text-clamp-2 text-muted">
                                                                    <small><%# Eval("description") %></small>
                                                                </div>

                                                                <div class="mt-2 fw-bold">
                                                                    <small><%# Eval("giaban", "{0:#,##0.##}") %> đ</small>
                                                                </div>

                                                                <!-- BADGE phân biệt SP -->
                                                                <div class="mt-2">
                                                                    <asp:Literal ID="lit_badge" runat="server"></asp:Literal>
                                                                </div>

                                                                <div class="mt-2 text-muted" style="font-size: 12px;">
                                                                    <div class="d-flex justify-content-between">
                                                                        <span><%# Eval("ngaytao", "{0:dd/MM/yyyy HH:mm}") %></span>
                                                                        <span>Lượt xem: <%# Eval("LuotTruyCap", "{0:#,##0}") %></span>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div class="card-footer p-2">
                                                                <div class="row g-2">
                                                                    <div class="col-6 d-none">
                                                                        <asp:Button ID="but_bansanphamnay" Width="100%" OnClick="but_bansanphamnay_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Bán chéo"
                                                                            CssClass="btn btn-sm btn-outline-warning w-100" Visible="false" />
                                                                    </div>
                                                                    <div class="col-6">
                                                                        <asp:Button ID="but_traodoi" Width="100%" OnClick="but_traodoi_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Trao đổi ngay"
                                                                            CssClass="btn btn-sm btn-primary w-100" />
                                                                    </div>
                                                                    <div class="col-12">
                                                                        <asp:Button ID="but_themvaogio" Width="100%" OnClick="but_themvaogio_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Thêm vào giỏ hàng"
                                                                            CssClass="btn btn-sm btn-outline-secondary w-100" />
                                                                    </div>

                                                                    <!-- NÚT HỦY BÁN CHÉO -->
                                                                    <div class="col-12">
                                                                        <asp:Button ID="but_huy_bancheo" Width="100%" OnClick="but_huy_bancheo_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Hủy bán chéo"
                                                                            CssClass="btn btn-sm btn-outline-danger w-100" Visible="false" />
                                                                    </div>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>

                                </div>

                            </div>
                        </div>

                    </asp:PlaceHolder>

                    <!-- Reviews -->
                            <asp:PlaceHolder ID="phReviewsBlock" runat="server" Visible="true">
                                <asp:UpdatePanel ID="Review" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>

                            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="true">
                                <div class="bio-card mt-2" id="section-reviews">
                                    <div class="bio-card-head">
                                        <div>
                                            <h3 class="bio-card-title">Đánh giá người dùng</h3>
                                            <div class="bio-card-sub">Tất cả phản hồi công khai từ người dùng đã trao đổi</div>
                                        </div>
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="ListReview" runat="server" Visible="true">
                                <div class="bio-card">
                                    <div class="bio-card-body">
                                        <asp:Repeater ID="rptDanhGia" runat="server">
                                            <ItemTemplate>
                                                <div class="bio-review-item">
                                                    <div class="bio-review-head">
                                                        <asp:Image ID="imgAvatar" runat="server" ImageUrl='<%# string.IsNullOrWhiteSpace((Eval("AnhDaiDien") + "").Trim()) ? "/uploads/images/macdinh.jpg" : (Eval("AnhDaiDien") + "").Trim() %>'
                                                            CssClass="bio-review-avatar" AlternateText="Ảnh đại diện" />

                                                        <div class="flex-fill">
                                                            <a href="<%#Eval("HoSoUrl")%>" class="bio-review-title text-decoration-none">
                                                                <%# Eval("TaiKhoanDanhGia") %>
                                                            </a>
                                                            <div class="bio-review-meta"><%# Eval("NgayDang", "{0:dd/MM/yyyy HH:mm}") %></div>
                                                        </div>
                                                    </div>

                                                    <div class="bio-review-stars"><%# new string('★', Convert.ToInt32(Eval("Diem"))) %></div>
                                                    <div class="bio-review-content"><%# Eval("NoiDung") %></div>

                                                    <asp:Image ID="imgReview" runat="server"
                                                        ImageUrl='<%# string.IsNullOrWhiteSpace((Eval("UrlAnh") + "").Trim()) ? "/uploads/images/macdinh.jpg" : (Eval("UrlAnh") + "").Trim() %>'
                                                        Width="120"
                                                        CssClass="review-img"
                                                        Style="max-height:120px;"
                                                        Visible='<%# !string.IsNullOrEmpty(Eval("UrlAnh") as string) %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                        <div class="mt-3">
                                            <asp:Panel ID="pnlPaging" runat="server" CssClass="pagination"></asp:Panel>
                                        </div>
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </asp:PlaceHolder>
                        </div>
                        <aside class="bio-side">
                            <section class="bio-card bio-card-mini">
                                <div class="bio-card-head">
                                    <div>
                                        <h3 class="bio-card-title">Tổng quan</h3>
                                        <div class="bio-card-sub">Thông tin nhanh hồ sơ</div>
                                    </div>
                                </div>
                                <div class="bio-card-body">
                                    <div class="bio-stat-grid">
                                        <div class="bio-stat">
                                            <div class="bio-stat-label">Đánh giá</div>
                                            <div class="bio-stat-value"><%= ViewState["SoLuongDanhGia"] %></div>
                                        </div>
                                        <div class="bio-stat" style="grid-column: span 2;">
                                            <div class="bio-stat-label">Tầng hồ sơ</div>
                                            <div class="bio-stat-value"><%= ViewState["phanloai_query"] %></div>
                                        </div>
                                    </div>
                                </div>
                            </section>

                            <section class="bio-card bio-card-mini">
                                <div class="bio-card-head">
                                    <div>
                                        <h3 class="bio-card-title">Chia sẻ nhanh</h3>
                                        <div class="bio-card-sub">Link hồ sơ & danh bạ</div>
                                    </div>
                                </div>
                                <div class="bio-card-body">
                                    <div class="bio-quick-actions">
                                        <a href="<%= ViewState["link_luu_danhba"] %>" class="btn btn-outline-success">
                                            <i class="ti ti-id"></i>&nbsp;Lưu danh bạ
                                        </a>
                                        <button type="button" class="btn btn-outline-primary"
                                            data-profile-link="<%: (ViewState["public_profile_url"] ?? "").ToString() %>"
                                            onclick="copyProfileLink(this)">
                                            <i class="ti ti-copy"></i>&nbsp;Copy link
                                        </button>
                                        <a href="<%= ViewState["public_profile_url"] %>" target="_blank" class="btn btn-outline-secondary">
                                            <i class="ti ti-world"></i>&nbsp;Mở hồ sơ
                                        </a>
                                        <asp:PlaceHolder ID="phOwnerEditButtonSide" runat="server" Visible="false">
                                            <a href="/home/edit-info.aspx" class="btn btn-outline-dark">
                                                <i class="ti ti-edit"></i>&nbsp;Chỉnh sửa
                                            </a>
                                        </asp:PlaceHolder>
                                    </div>
                                </div>
                            </section>
                        </aside>
                    </div>

                    <!-- Review image modal (Tabler/Bootstrap) -->
                    <div class="modal modal-blur fade" id="reviewImageModal" tabindex="-1" aria-hidden="true">
                        <div class="modal-dialog modal-lg modal-dialog-centered">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h5 class="modal-title">Xem ảnh</h5>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                </div>
                                <div class="modal-body text-center">
                                    <img id="reviewModalImage" src="" style="max-width:100%; max-height:75vh; border-radius: 12px;" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="modal modal-blur fade" id="contactLeadModal" tabindex="-1" aria-hidden="true">
                        <div class="modal-dialog modal-dialog-centered">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h5 class="modal-title">Liên hệ</h5>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                </div>
                                <div class="modal-body">
                                    <div class="mb-3">
                                        <label class="form-label text-danger">Họ tên</label>
                                        <input id="contact_name" type="text" class="form-control" />
                                    </div>
                                    <div class="mb-3">
                                        <label class="form-label text-danger">Số điện thoại</label>
                                        <input id="contact_phone" type="text" class="form-control" />
                                    </div>
                                    <div class="mb-3">
                                        <label class="form-label">Email (không bắt buộc)</label>
                                        <input id="contact_email" type="email" class="form-control" />
                                    </div>
                                    <div class="mb-3">
                                        <label class="form-label">Nội dung</label>
                                        <textarea id="contact_message" class="form-control" rows="3"></textarea>
                                    </div>
                                    <div id="contact_error" class="text-danger small"></div>
                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Đóng</button>
                                    <button type="button" class="btn btn-primary" id="contact_submit_btn">Gửi liên hệ</button>
                                </div>
                            </div>
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

<asp:Content ID="Content3" ContentPlaceHolderID="foot_sau" runat="Server">

    <!-- jQuery (phải trước jQuery UI) -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <!-- jQuery UI -->
    <link href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css" rel="stylesheet" />
    <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>

    <script type="text/javascript">
        var user_query = '<%= ViewState["user_query"] %>';

        function copyProfileLink(btn) {
            if (!btn) return;
            var link = btn.getAttribute('data-profile-link') || '';
            if (!link) return;

            function onCopied() {
                var oldHtml = btn.innerHTML;
                btn.innerHTML = '<i class="ti ti-check"></i>Đã sao chép';
                setTimeout(function () { btn.innerHTML = oldHtml; }, 1400);
            }

            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(link).then(onCopied).catch(function () {
                    window.prompt('Sao chép liên kết hồ sơ:', link);
                });
            } else {
                window.prompt('Sao chép liên kết hồ sơ:', link);
            }
        }

        function uploadTimelineImage() {
            var fileInput = document.getElementById("timelineImageInput");
            var messageDiv = document.getElementById("timelineImageMessage");
            if (!fileInput || fileInput.files.length === 0) return;

            var file = fileInput.files[0];
            if (file.type && file.type.indexOf("image/") !== 0) {
                messageDiv.innerHTML = "Vui lòng chọn file ảnh.";
                return;
            }
            var maxRawSize = 60 * 1024 * 1024;
            if (file.size > maxRawSize) {
                messageDiv.innerHTML = "Ảnh quá lớn. Vui lòng chọn ảnh nhỏ hơn 60 MB.";
                return;
            }

            messageDiv.innerHTML = "Đang nén ảnh...";

            compressImageForTimeline(file, {
                maxEdge: 1024,
                quality: 0.78,
                maxBytes: 90 * 1024
            }).then(function (blob) {
                if (!blob) {
                    messageDiv.innerHTML = "Không thể xử lý ảnh này. Vui lòng thử ảnh khác.";
                    return;
                }
                if (blob.size > (90 * 1024)) {
                    messageDiv.innerHTML = "Ảnh sau khi nén vẫn quá lớn. Vui lòng chọn ảnh khác hoặc cắt nhỏ.";
                    return;
                }
                messageDiv.innerHTML = "Đã nén ảnh: " + Math.round(blob.size / 1024) + " KB. Đang tải lên...";
                var formData = new FormData();
                formData.append("file", blob, "timeline.jpg");

                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/home/upload-timeline.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        messageDiv.innerHTML = "";
                        var url = xhr.responseText;
                        var hf = document.getElementById('<%= hfTimelineImage.ClientID %>');
                        if (hf) hf.value = url;

                        var img = document.getElementById('<%= imgTimelinePreview.ClientID %>');
                        if (img) {
                            img.src = url;
                            img.style.display = 'block';
                        }
                        if (fileInput) {
                            fileInput.value = "";
                            fileInput.disabled = true;
                        }
                    } else if (xhr.status === 413 || (xhr.responseText && xhr.responseText.indexOf("413 Request Entity Too Large") >= 0)) {
                        messageDiv.innerHTML = "Ảnh quá lớn sau khi nén. Vui lòng chọn ảnh khác hoặc cắt nhỏ.";
                    } else {
                        messageDiv.innerHTML = xhr.responseText || "Lỗi upload.";
                    }
                };
                xhr.onerror = function () {
                    messageDiv.innerHTML = "Không thể tải ảnh lên. Vui lòng thử lại.";
                };
                xhr.send(formData);
            }).catch(function () {
                messageDiv.innerHTML = "Không thể nén ảnh đủ nhỏ. Vui lòng chọn ảnh khác hoặc cắt nhỏ.";
            });
        }

        function clearTimelineImage() {
            var hf = document.getElementById('<%= hfTimelineImage.ClientID %>');
            if (hf) hf.value = "";
            var img = document.getElementById('<%= imgTimelinePreview.ClientID %>');
            if (img) {
                img.src = "";
                img.style.display = 'none';
            }
            var fileInput = document.getElementById("timelineImageInput");
            if (fileInput) {
                fileInput.value = "";
                fileInput.disabled = false;
            }
        }

        function prepareTimelineSubmit() {
            var fileInput = document.getElementById("timelineImageInput");
            var hf = document.getElementById('<%= hfTimelineImage.ClientID %>');
            var messageDiv = document.getElementById("timelineImageMessage");
            if (fileInput && fileInput.files && fileInput.files.length > 0) {
                if (!hf || !hf.value) {
                    if (messageDiv) {
                        messageDiv.innerHTML = "Ảnh chưa được tải lên. Vui lòng chờ hoàn tất hoặc chọn lại ảnh.";
                    }
                    return false;
                }
            }
            if (fileInput) {
                fileInput.disabled = true;
            }
            return true;
        }

        function compressImageForTimeline(file, options) {
            var maxEdge = options && options.maxEdge ? options.maxEdge : 1024;
            var quality = options && options.quality ? options.quality : 0.78;
            var maxBytes = options && options.maxBytes ? options.maxBytes : 90 * 1024;
            var minEdge = 240;
            var minQuality = 0.28;
            var maxAttempts = 40;

            return new Promise(function (resolve, reject) {
                function finalizeBlob(blob, edge, q, attempt) {
                    if (!blob) { reject(); return; }
                    if (blob.size <= maxBytes) {
                        resolve(blob);
                        return;
                    }
                    if (attempt >= maxAttempts) { reject(); return; }

                    // giảm dần chất lượng và kích thước
                    var nextQuality = q;
                    var nextEdge = edge;
                    if (q > minQuality + 0.01) {
                        nextQuality = Math.max(minQuality, q - 0.08);
                    } else if (edge > minEdge + 8) {
                        nextEdge = Math.max(minEdge, Math.round(edge * 0.85));
                        nextQuality = quality;
                    } else {
                        nextEdge = Math.max(240, Math.round(edge * 0.8));
                        nextQuality = Math.max(0.3, q - 0.05);
                        if (nextEdge === edge && nextQuality === q) { reject(); return; }
                    }
                    compressWithBitmap(currentBitmap, nextEdge, nextQuality, attempt + 1);
                }

                function compressWithBitmap(bitmap, edge, q, attempt) {
                    var canvas = drawImageToCanvas(bitmap, edge);
                    canvasToBlob(canvas, q, function (blob) {
                        finalizeBlob(blob, edge, q, attempt || 0);
                    });
                }

                var currentBitmap = null;

                if (window.createImageBitmap) {
                    createImageBitmap(file).then(function (bitmap) {
                        currentBitmap = bitmap;
                        compressWithBitmap(bitmap, maxEdge, quality, 0);
                    }).catch(function () { reject(); });
                    return;
                }

                var reader = new FileReader();
                var img = new Image();
                reader.onload = function (e) {
                    img.onload = function () {
                        currentBitmap = img;
                        var canvas = drawImageToCanvas(img, maxEdge);
                        canvasToBlob(canvas, quality, function (blob) {
                            if (!blob) { reject(); return; }
                            if (blob.size <= maxBytes) {
                                resolve(blob);
                            } else {
                                compressWithBitmap(img, maxEdge, quality, 0);
                            }
                        });
                    };
                    img.onerror = function () { reject(); };
                    img.src = e.target.result;
                };
                reader.onerror = function () { reject(); };
                reader.readAsDataURL(file);
            });
        }

        function canvasToBlob(canvas, quality, cb) {
            if (canvas.toBlob) {
                canvas.toBlob(function (blob) {
                    cb(blob);
                }, "image/jpeg", quality);
                return;
            }
            try {
                var dataUrl = canvas.toDataURL("image/jpeg", quality);
                var blob = dataURLToBlob(dataUrl);
                cb(blob);
            } catch (e) {
                cb(null);
            }
        }

        function dataURLToBlob(dataUrl) {
            var parts = dataUrl.split(",");
            if (parts.length < 2) return null;
            var byteString = atob(parts[1]);
            var mimeMatch = parts[0].match(/data:([^;]+)/);
            var mimeString = mimeMatch ? mimeMatch[1] : "image/jpeg";
            var ab = new ArrayBuffer(byteString.length);
            var ia = new Uint8Array(ab);
            for (var i = 0; i < byteString.length; i++) {
                ia[i] = byteString.charCodeAt(i);
            }
            return new Blob([ab], { type: mimeString });
        }

        function drawImageToCanvas(image, maxSize) {
            var width = image.width;
            var height = image.height;
            var scale = 1;
            var maxEdge = Math.max(width, height);
            if (maxEdge > maxSize) scale = maxSize / maxEdge;

            var targetW = Math.max(1, Math.round(width * scale));
            var targetH = Math.max(1, Math.round(height * scale));

            var canvas = document.createElement("canvas");
            canvas.width = targetW;
            canvas.height = targetH;
            var ctx = canvas.getContext("2d");
            ctx.drawImage(image, 0, 0, targetW, targetH);
            return canvas;
        }

        // Review image -> Tabler modal
        (function () {
            function initReviewModal() {
                var modalEl = document.getElementById('reviewImageModal');
                if (!modalEl || !window.bootstrap) return;

                var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
                document.querySelectorAll('.review-img').forEach(function (img) {
                    img.addEventListener('click', function () {
                        document.getElementById('reviewModalImage').src = img.src;
                        modal.show();
                    });
                });
            }

            // init on first load
            document.addEventListener('DOMContentLoaded', initReviewModal);

            // re-init after UpdatePanel postback
            if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    initReviewModal();
                });
            }
        })();

        // Contact lead submit (AJAX)
        (function () {
            function normalizePhone(input) {
                return (input || "").replace(/\s+/g, "").replace(/\./g, "").replace(/\+/g, "").replace(/-/g, "");
            }

            function showInlineError(msg) {
                var el = document.getElementById('contact_error');
                if (el) el.textContent = msg || '';
            }

            function showSuccessAndRedirect(targetUrl) {
                if (window.show_modal) {
                    show_modal('Gửi liên hệ thành công, tôi sẽ sớm liên hệ lại với bạn!', 'Thông báo', true, 'success');
                    var tries = 0;
                    var timer = setInterval(function () {
                        tries++;
                        var okBtn = document.querySelector('#dynamicModal .btn-ok');
                        var closeBtn = document.querySelector('#dynamicModal .btn-close');
                        if (okBtn) okBtn.addEventListener('click', function () { window.location.href = targetUrl; });
                        if (closeBtn) closeBtn.addEventListener('click', function () { window.location.href = targetUrl; });
                        if ((okBtn || closeBtn) || tries > 10) clearInterval(timer);
                    }, 100);
                } else {
                    alert('Gửi liên hệ thành công, tôi sẽ sớm liên hệ lại với bạn!');
                    window.location.href = targetUrl;
                }
            }

            function handleSubmit() {
                var btn = document.getElementById('contact_submit_btn');
                if (!btn) return;
                if (btn.getAttribute('data-loading') === '1') return;

                var name = (document.getElementById('contact_name') || {}).value || '';
                var phone = (document.getElementById('contact_phone') || {}).value || '';
                var email = (document.getElementById('contact_email') || {}).value || '';
                var message = (document.getElementById('contact_message') || {}).value || '';

                name = name.trim();
                phone = normalizePhone(phone);
                email = email.trim();
                message = message.trim();

                if (!name) {
                    showInlineError('Vui lòng nhập họ tên.');
                    return;
                }
                if (!phone) {
                    showInlineError('Vui lòng nhập số điện thoại.');
                    return;
                }
                if (email && email.indexOf('@') === -1) {
                    showInlineError('Email chưa đúng định dạng.');
                    return;
                }

                var contactUser = '<%: (ViewState["user_query"] ?? "").ToString() %>';
                if (!contactUser) {
                    showInlineError('Không xác định được hồ sơ nhận liên hệ.');
                    return;
                }

                var profileUrl = '<%: (ViewState["link_hoso_congkhai"] ?? "").ToString() %>';
                if (!profileUrl) profileUrl = window.location.pathname || '/';

                showInlineError('');
                btn.setAttribute('data-loading', '1');
                var oldText = btn.textContent;
                btn.textContent = 'Đang gửi...';
                btn.disabled = true;

                var body = new URLSearchParams();
                body.append('user', contactUser);
                body.append('name', name);
                body.append('phone', phone);
                body.append('email', email);
                body.append('message', message);

                fetch('/home/contact-lead.ashx', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
                    body: body.toString()
                })
                    .then(function (res) { return res.text(); })
                    .then(function (text) {
                        var data = null;
                        try { data = JSON.parse(text || '{}'); } catch (e) { }
                        if (!data || !data.ok) {
                            throw new Error((data && data.message) ? data.message : 'Gửi liên hệ thất bại.');
                        }

                        var modalEl = document.getElementById('contactLeadModal');
                        if (window.bootstrap && modalEl) {
                            bootstrap.Modal.getOrCreateInstance(modalEl).hide();
                        }

                        showSuccessAndRedirect(profileUrl);
                    })
                    .catch(function (err) {
                        showInlineError(err && err.message ? err.message : 'Gửi liên hệ thất bại.');
                    })
                    .finally(function () {
                        btn.removeAttribute('data-loading');
                        btn.textContent = oldText;
                        btn.disabled = false;
                    });
            }

            document.addEventListener('DOMContentLoaded', function () {
                var btn = document.getElementById('contact_submit_btn');
                if (!btn) return;
                btn.addEventListener('click', handleSubmit);
            });
        })();
    </script>

    <script type="text/javascript">
        $(function () {
            var txtSearch = $('#<%= txt_search.ClientID %>');
            txtSearch.autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        url: "home/Default.aspx/GetSuggestions",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify({
                            prefixText: request.term,
                            count: 10,
                            userQuery: user_query
                        }),
                        success: function (data) { response(data.d); }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    txtSearch.val(ui.item.value);
                    __doPostBack('<%= txt_search.UniqueID %>', '');
                    return false;
                }
            });

            var txtSearch1 = $('#<%= txt_timkiem1.ClientID %>');
            txtSearch1.autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        url: "home/Default.aspx/GetSuggestions",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify({
                            prefixText: request.term,
                            count: 10,
                            userQuery: user_query
                        }),
                        success: function (data) { response(data.d); }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    txtSearch1.val(ui.item.value);
                    __doPostBack('<%= txt_timkiem1.UniqueID %>', '');
                    return false;
                }
            });
        });
    </script>

</asp:Content>
