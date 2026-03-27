<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DanhChoBan_MoiNhat_UC.ascx.cs" Inherits="Uc_Home_DanhChoBan_MoiNhat_UC" %>
<%@ Register Src="~/Uc/Home/DanhMuc_icon_Home_UC.ascx" TagPrefix="uc1" TagName="DanhMuc_icon_Home_UC" %>

<style>
    /* ===== Grid 6 (desktop), 4 (lg), 3 (md), 2 (mobile) ===== */
    .grid-5 {
        display: flex;
        flex-wrap: wrap;
        gap: 12px;
    }

    .grid-5-item {
        width: 100%;
        box-sizing: border-box;
    }

    @media (max-width: 575.98px) {
        .grid-5-item { width: calc((100% - 12px) / 2); }
    }

    @media (min-width: 576px) and (max-width: 767.98px) {
        .grid-5-item { width: calc((100% - 12px) / 2); }
    }

    @media (min-width: 768px) and (max-width: 991.98px) {
        .grid-5-item { width: calc((100% - 12px*2) / 3); }
    }

    @media (min-width: 992px) and (max-width: 1199.98px) {
        .grid-5-item { width: calc((100% - 12px*3) / 4); }
    }

    @media (min-width: 1200px) {
        .grid-5-item { width: calc((100% - 12px*5) / 6); }
    }

    .sp-card {
        border: none;
        border-radius: 12px;
        background: #fff;
        height: 100%;
        display: flex;
        flex-direction: column;
        box-shadow: 0 2px 14px rgba(0,0,0,.06);
        transition: box-shadow .2s ease;
        overflow: visible;
        box-sizing: border-box;
    }

        .sp-card:hover {
            box-shadow: 0 8px 24px rgba(0,0,0,.10);
        }

    .sp-body {
        padding: 12px;
        display: flex;
        flex-direction: column;
        height: 100%;
        min-width: 0;
        box-sizing: border-box;
    }

    .thumb-wrap {
        border-radius: 10px;
        overflow: hidden;
        background: #f6f8fb;
    }

    .sp-thumb {
        width: 100%;
        aspect-ratio: 1 / 1;
        object-fit: cover;
        display: block;
        transition: transform .25s ease;
    }
    .sp-thumb-video {
        width: 100%;
        aspect-ratio: 1 / 1;
        object-fit: cover;
        display: block;
        background: #000;
        transition: transform .25s ease;
        pointer-events: none;
    }

    .sp-card:hover .sp-thumb {
        transform: scale(1.04);
    }

    .sp-title {
        margin-top: 10px;
        font-weight: 600;
        color: #1d273b;
        line-height: 1.25rem;
        min-height: 2.5rem;
        display: -webkit-box;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
        overflow: hidden;
        transition: color .2s ease;
    }

    .sp-card:hover .sp-title {
        color: #2fb344;
    }

    .sp-desc {
        margin-top: 6px;
        color: #626976;
        font-size: .875rem;
        line-height: 1.15rem;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    }

    .sp-price {
        margin-top: 6px;
        color: #d63939;
        font-weight: 700;
    }

    .sp-meta {
        margin-top: 8px;
        font-size: .875rem;
        color: #626976;
    }

        .sp-meta .rowline {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 8px;
        }

    .sp-loc {
        display: flex;
        align-items: center;
        gap: 6px;
        min-width: 0;
        color: #626976;
        font-weight: 400;
    }

        .sp-loc .loc-text {
            min-width: 0;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

    .thumb-wrap {
        position: relative;
    }
    
    .heart-icon {
        position: absolute;
        top: 8px;
        right: 8px;
        font-size: 22px;
        cursor: pointer;
        color: #adb5bd;          
        background: #fff;
        border-radius: 50%;
        padding: 6px;
    }

    .heart-icon.active {
        color: #dc3545;          
    }

    .heart-btn {
        text-decoration: none;
        padding: 0;
        border: 0;
        background: transparent;
    }

    .heart-btn:focus,
    .heart-btn:hover {
        text-decoration: none;
    }

    .icon-18 {
        width: 18px;
        height: 18px;
        flex: 0 0 18px;
    }

    .sp-time {
        display: flex;
        align-items: center;
        gap: 6px;
        color: #626976;
        font-weight: 400;
        white-space: nowrap;
    }

    /* ===== Dropdown 3 chấm ===== */
    .kebab-wrap {
        position: relative;
        z-index: 60;
    }

    .kebab-btn {
        border: none;
        background: transparent;
        padding: 4px 6px;
        border-radius: 8px;
        cursor: pointer;
    }

        .kebab-btn:hover {
            background: rgba(98,105,118,.10);
        }

    .kebab-menu {
        position: absolute;
        right: 0;
        top: calc(100% + 6px);
        min-width: 220px;
        background: #fff;
        border: 1px solid rgba(98,105,118,.18);
        border-radius: 12px;
        box-shadow: 0 8px 24px rgba(0,0,0,.12);
        padding: 6px;
        display: none;
        z-index: 9999;
    }

        .kebab-menu.show {
            display: block;
        }

    .kebab-item {
        display: flex;
        align-items: center;
        gap: 10px;
        padding: 10px 12px;
        border-radius: 10px;
        color: #1d273b;
        text-decoration: none;
        font-size: 14px;
        cursor: pointer;
        user-select: none;
    }

        .kebab-item:hover {
            background: rgba(98,105,118,.10);
        }

    .sp-actions {
        margin-top: 12px;
        display: grid;
        grid-template-columns: 1fr;
        gap: 8px;
        width: 100%;
        box-sizing: border-box;
    }

    .sp-actions .btn {
        width: 100%;
        min-height: 34px;
        padding: 6px 10px;
        font-size: 13px;
            display: flex;
            align-items: center;
            justify-content: center;
        white-space: nowrap;
        max-width: 100%;
        box-sizing: border-box;
    }

    @media (min-width: 1200px) {
        .sp-body { padding: 10px; }
        .sp-title { font-size: 14px; line-height: 1.2rem; min-height: 2.4rem; }
        .sp-desc { font-size: 12px; }
        .sp-price { font-size: 14px; }
        .sp-meta { font-size: 12px; }
        .heart-icon { font-size: 20px; padding: 5px; }
        .sp-actions .btn { min-height: 30px; font-size: 12px; }
    }

    .sp-actions .btn-outline-info {
        background: #0f8f5d;
        border-color: #0f8f5d;
        color: #fff;
    }

    .sp-actions .btn-outline-info:hover,
    .sp-actions .btn-outline-info:focus {
        background: #0d7b50;
        border-color: #0d7b50;
        color: #fff;
    }

    .sp-actions .btn-outline-success {
        background: #eef6ff;
        border-color: #bfd7f5;
        color: #0f4c81;
    }

    .sp-actions .btn-outline-success:hover,
    .sp-actions .btn-outline-success:focus {
        background: #e1efff;
        border-color: #a9c8ee;
        color: #0b3f66;
    }

    .sp-card a, .sp-card a:hover {
        color: inherit;
        text-decoration: none;
    }

    /* ===== Modal z-index cao hơn topbar ===== */
    .modal {
        z-index: 2000;
    }

    .modal-backdrop {
        z-index: 1990;
    }

    /* ===== Search Bar Base (shared) ===== */
    .search-wrap .g-icon {
        color: #94a3b8;
        font-size: 18px;
        width: 20px;
        text-align: center;
        flex: 0 0 20px;
    }

    .search-wrap .g-input {
        width: 100%;
        border: 0;
        outline: 0;
        background: transparent;
        font-size: 15px;
        color: #1f2937;
    }

    .search-wrap .g-input::placeholder {
        color: #9aa0a6;
    }

    .search-wrap .g-btn-inline-mobile {
        display: none;
    }

    .search-wrap .g-select {
        width: 100%;
        border: 0;
        background: transparent;
        font-size: 15px;
        color: #1f2937;
    }

    .search-wrap .g-seg .choices {
        width: 100%;
        flex: 1 1 auto;
        min-width: 0;
    }

    .search-wrap .g-seg .choices__inner {
        min-height: 40px;
        padding: 0;
        border: 0;
        background: transparent;
        box-shadow: none;
        display: flex;
        align-items: center;
    }

    .search-wrap .g-seg .choices__item--selectable {
        line-height: 40px;
    }

    .search-wrap .g-seg .choices__list--single {
        padding: 0;
    }

    .search-wrap .g-seg .choices__placeholder {
        color: #9aa0a6;
    }

    .search-wrap .g-seg .choices[data-type*="select-one"]::after {
        right: 0;
        border-color: #9aa0a6 transparent transparent;
    }

    .search-wrap .g-seg .choices__list--dropdown {
        border-radius: 12px;
        border: 1px solid #dfe1e5;
        box-shadow: 0 12px 24px rgba(32, 33, 36, .2);
        overflow: hidden;
        z-index: 2000;
    }

    /* ===== Desktop/tablet layout (same structure, responsive scale) ===== */
    @media (min-width: 576px) {
        .search-wrap .g-search {
            display: flex;
            flex-wrap: nowrap;
            align-items: center;
            gap: 8px;
            background: #ffffff;
            border: 1px solid #e5ebf0;
            border-radius: 18px;
            padding: 8px;
            min-height: 54px;
            box-shadow: 0 10px 24px rgba(15, 23, 42, .08);
            transition: box-shadow .18s ease, border-color .18s ease;
            overflow: visible;
        }

        .search-wrap .g-search:focus-within {
            border-color: #d6dee6;
            box-shadow: 0 12px 28px rgba(15, 23, 42, .12);
        }

        .search-wrap .g-seg {
            display: flex;
            align-items: center;
            gap: 8px;
            min-height: 42px;
            padding: 0 12px;
            flex: 1 1 180px;
            position: relative;
            background: #f3f6f9;
            border-radius: 14px;
        }

        .search-wrap .g-seg.g-query {
            flex: 2 1 260px;
        }

        .search-wrap .g-seg.g-action {
            flex: 0 0 auto;
            padding: 0;
            min-height: 42px;
            background: transparent;
        }

        .search-wrap .g-seg + .g-seg {
            border-top: 0;
            border-left: 0;
            margin-left: 0;
        }

        .search-wrap .g-btn {
            min-height: 40px;
            padding: 0 16px;
            border-radius: 12px;
            border: 0;
            background: #2f8f46;
            color: #ffffff;
            font-weight: 700;
            text-decoration: none !important;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
            box-shadow: 0 8px 18px rgba(47, 143, 70, .28);
            white-space: nowrap;
            width: auto;
        }

        .search-wrap .g-btn:hover {
            background: #27793b;
            color: #ffffff;
        }
    }

    @media (min-width: 992px) {
        .search-wrap .g-search {
            align-items: center;
            border-radius: 22px;
            min-height: 60px;
            gap: 10px;
        }

        .search-wrap .g-seg {
            flex: 1 1 220px;
            min-height: 50px;
            padding: 0 16px;
        }

        .search-wrap .g-seg.g-query {
            flex: 2 1 360px;
        }

        .search-wrap .g-seg + .g-seg {
            border-top: 0;
            border-left: 0;
            margin-left: 0;
        }

        .search-wrap .g-seg.g-action {
            flex: 0 0 auto;
            padding: 0;
            min-height: 50px;
            border-top: 0;
            background: transparent;
        }

        .search-wrap .g-btn {
            min-height: 44px;
            padding: 0 22px;
            border-radius: 14px;
        }
    }

    /* Nút X giống trang Thưởng */
    #modalDatHang .modal-header,
    #modalAddCart .modal-header {
        position: relative;
        display: flex;
        align-items: center;
        padding-right: 52px;
    }

    .modal-xbtn-abs {
        position: absolute;
        right: 12px;
        top: 10px;
        border: 0;
        background: transparent;
        width: 34px;
        height: 34px;
        border-radius: 10px;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 0;
        cursor: pointer;
    }

    .modal-xbtn-abs:hover {
        background: rgba(98,105,118,.10);
    }

    .modal-xbtn-abs span {
        font-size: 22px;
        line-height: 1;
        color: #98a2b3;
    }

    .ct-guest-auth-wrap {
        display: none;
    }

    :root {
        --aha-home-root-search-height: 88px;
    }

    .hero-ct-like {
        display: flex;
        align-items: center;
        justify-content: center;
        min-height: 220px;
    }

    .hero-ct-inner {
        width: 100%;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 0 16px 18px;
    }

    .hero-ct-title {
        transform: translateY(-8px);
    }

    body.aha-home-root-pinned-search .page-wrapper {
        padding-top: var(--aha-home-root-search-height) !important;
    }

    .search-wrap.is-root-pinned.is-sticky-fixed {
        z-index: 1040;
        padding: 4px 0 6px;
        background: linear-gradient(180deg, rgba(248, 251, 255, .98) 0%, rgba(248, 251, 255, .90) 100%);
        border-bottom: 1px solid #dce8f4;
    }


    html[data-bs-theme="dark"] .sp-card {
        background: #0f172a;
        box-shadow: 0 10px 26px rgba(0,0,0,.4);
    }

    html[data-bs-theme="dark"] .thumb-wrap {
        background: #111d34;
    }

    html[data-bs-theme="dark"] .sp-title {
        color: #e5e7eb;
    }

    html[data-bs-theme="dark"] .sp-card:hover .sp-title {
        color: #4ade80;
    }

    html[data-bs-theme="dark"] .sp-desc,
    html[data-bs-theme="dark"] .sp-meta,
    html[data-bs-theme="dark"] .sp-loc,
    html[data-bs-theme="dark"] .sp-time {
        color: #94a3b8;
    }

    html[data-bs-theme="dark"] .heart-icon {
        background: #0f172a;
        border: 1px solid #223246;
        color: #94a3b8;
    }

    html[data-bs-theme="dark"] .kebab-btn:hover {
        background: rgba(148,163,184,.12);
    }

    html[data-bs-theme="dark"] .kebab-menu {
        background: #0f172a;
        border-color: #223246;
        box-shadow: 0 10px 26px rgba(0,0,0,.45);
    }

    html[data-bs-theme="dark"] .kebab-item {
        color: #e5e7eb;
    }

    html[data-bs-theme="dark"] .kebab-item:hover {
        background: rgba(148,163,184,.12);
    }

    html[data-bs-theme="dark"] .sp-actions .btn-outline-success {
        background: #1f2a3a;
        border-color: #334155;
        color: #e5e7eb;
    }

    html[data-bs-theme="dark"] .sp-actions .btn-outline-success:hover,
    html[data-bs-theme="dark"] .sp-actions .btn-outline-success:focus {
        background: #273449;
        border-color: #3b4a61;
        color: #ffffff;
    }

    html[data-bs-theme="dark"] .search-wrap.is-root-pinned.is-sticky-fixed {
        background: linear-gradient(180deg, rgba(15, 23, 42, .98) 0%, rgba(15, 23, 42, .90) 100%);
        border-bottom: 1px solid #223246;
    }

    .ct-tabs {
        display: flex;
        gap: 16px;
        align-items: center;
        padding: 6px 0 0;
    }

    .ct-tab {
        border: 0;
        background: transparent;
        font-weight: 700;
        font-size: 16px;
        color: #9ca3af;
        padding: 6px 0;
        position: relative;
    }

    .ct-tab.is-active {
        color: #111827;
    }

    .ct-tab.is-active::after {
        content: "";
        position: absolute;
        left: 0;
        right: 0;
        bottom: -6px;
        height: 3px;
        border-radius: 999px;
        background: #ffb400;
    }

    .home-feed-wrap {
        margin-top: 14px;
    }

    html[data-bs-theme="dark"] .ct-guest-auth-btn.ct-login {
        background: #0f172a;
        border-color: #223246;
        color: #ffffff !important;
    }

    html[data-bs-theme="dark"] .ct-guest-auth-btn.ct-register {
        background: rgba(34, 197, 94, 0.16);
        border-color: rgba(34, 197, 94, 0.35);
        color: #86efac !important;
    }

    .search-wrap.is-root-pinned.is-sticky-fixed .ct-shell {
        padding: 0 12px;
    }

    .home-category-inline .dm-section {
        margin-top: 10px !important;
        margin-bottom: 12px !important;
        transform: none !important;
    }

    @media (max-width: 991.98px) {
        :root {
            --aha-home-root-search-height: 84px;
        }

        .hero-ct-like {
            min-height: 132px;
            height: 132px;
        }

        .hero-ct-inner {
            padding: 0 14px 12px;
        }

        .hero-ct-title {
            font-size: 26px;
            line-height: 1.15;
            transform: translateY(-4px);
        }

        .search-wrap.is-root-pinned.is-sticky-fixed .ct-shell {
            padding: 0 10px;
        }

        .search-wrap.is-root-pinned.is-sticky-fixed .ct-guest-auth-wrap {
            margin-bottom: 6px;
            padding-top: 1px;
        }

        .search-wrap .ct-guest-auth-wrap {
            display: flex;
            align-items: center;
            justify-content: flex-end;
            gap: 8px;
            max-width: 1000px;
            margin: 0 auto 8px;
            padding: 0 4px;
        }

        .search-wrap.is-sticky-fixed .ct-guest-auth-wrap {
            margin-bottom: 6px;
            padding-top: 2px;
        }

        .ct-guest-auth-btn {
            min-height: 36px;
            padding: 0 14px;
            border-radius: 999px;
            font-size: 13px;
            font-weight: 700;
            text-decoration: none !important;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            white-space: nowrap;
        }

        .ct-guest-auth-btn.ct-login {
            background: #0f172a;
            border: 1px solid #0f172a;
            color: #ffffff !important;
        }

        .ct-guest-auth-btn.ct-register {
            background: #e9f7ef;
            border: 1px solid rgba(25, 135, 84, .45);
            color: #146c43 !important;
        }

        .search-wrap.is-compact .ct-guest-auth-wrap {
            margin-bottom: 4px;
            padding-top: 0;
            gap: 6px;
        }

        .search-wrap.is-root-pinned.is-compact {
            padding: 4px 0 6px;
        }

        .search-wrap.is-compact .ct-guest-auth-btn {
            min-height: 32px;
            padding: 0 12px;
            font-size: 12px;
        }

        .search-wrap.is-compact .g-search {
            min-height: 46px;
            border-radius: 12px;
        }

        .search-wrap.is-compact .g-seg {
            min-height: 40px;
            padding: 0 10px;
        }

        .search-wrap.is-compact .g-input,
        .search-wrap.is-compact .g-select {
            font-size: 13px;
        }

        .search-wrap.is-compact .g-btn {
            min-height: 32px;
            padding: 0 10px;
            font-size: 12px;
            box-shadow: none;
        }

        .search-wrap.is-compact .g-btn-inline-mobile {
            min-height: 28px;
            padding: 0 10px;
            font-size: 11px;
        }
    }

    @media (max-width: 575.98px) {
        .hero-ct-like {
            min-height: 96px;
            height: 96px;
            display: flex !important;
            align-items: center;
            justify-content: center;
            border-radius: 0 0 18px 18px;
        }

        .hero-ct-inner {
            padding: 0 12px 10px;
        }

        .hero-ct-title {
            font-size: 19px;
            line-height: 1.15;
            transform: translateY(-2px);
        }

        .search-wrap {
            margin-top: 0;
            padding-top: 6px;
            padding-bottom: 10px;
            background: #f3f6f8;
            border-bottom: 1px solid #e6edf3;
        }

        .search-wrap .ct-shell {
            padding: 6px 12px 0;
        }

        .search-wrap .g-search {
            display: flex;
            flex-wrap: wrap;
            align-items: center;
            background: #ffffff;
            border: 1px solid #e5ebf0;
            box-shadow: 0 8px 18px rgba(15, 23, 42, .12);
            gap: 8px;
            padding: 8px;
            border-radius: 16px;
        }

        .search-wrap .g-seg {
            display: flex;
            align-items: center;
            gap: 8px;
            border: 0;
            border-radius: 12px;
            background: #f5f7f9;
            border: 1px solid #eef1f4;
            min-height: 38px;
            padding: 0 12px;
            box-shadow: none;
        }

        .search-wrap .g-seg + .g-seg {
            border: 0;
        }

        .search-wrap .g-seg.g-query {
            order: 1;
            flex: 1 1 100%;
            background: #ffffff;
            border-color: #e6ebf1;
            gap: 10px;
            padding-right: 6px;
        }

        .search-wrap .g-seg.g-cat {
            order: 2;
            flex: 1 1 calc(50% - 6px);
        }

        .search-wrap .g-seg.g-loc {
            order: 3;
            flex: 1 1 calc(50% - 6px);
        }

        .search-wrap .g-seg.g-action {
            display: none !important;
        }

        .search-wrap .g-btn {
            width: 100%;
            border-radius: 999px;
            min-height: 38px;
        }

        .search-wrap .g-input {
            flex: 1 1 auto;
            min-width: 0;
            font-size: 14px;
        }

        .search-wrap .g-btn-inline-mobile {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 30px;
            padding: 0 12px;
            border-radius: 999px;
            border: 1px solid #2f8f46;
            background: #2f8f46;
            color: #fff;
            font-size: 12px;
            font-weight: 700;
            text-decoration: none;
            white-space: nowrap;
            flex: 0 0 auto;
            box-shadow: 0 5px 12px rgba(47, 143, 70, .25);
        }

        .search-wrap .g-btn-inline-mobile:hover,
        .search-wrap .g-btn-inline-mobile:focus {
            background: #27793b;
            border-color: #27793b;
            color: #fff;
            text-decoration: none;
        }

        .search-wrap .g-seg.g-query .g-icon,
        .search-wrap.in-header .g-seg.g-query .g-icon {
            display: inline-flex !important;
            align-items: center;
            justify-content: center;
            flex: 0 0 20px;
            width: 20px;
            opacity: 1;
            color: #64748b;
        }

        .search-wrap .g-select {
            font-size: 13px;
            color: #4b5563;
        }

        .search-wrap .g-icon {
            color: #6b7280;
        }

        .search-wrap .g-seg .choices {
            flex: 1 1 auto;
            min-width: 0;
        }

        .search-wrap .g-seg .choices__inner {
            min-height: 34px;
            align-items: center;
        }

        .search-wrap .g-seg .choices__list--single {
            display: flex;
            align-items: center;
            min-width: 0;
        }

        .search-wrap.in-header {
            margin-top: 0 !important;
            padding-top: 0 !important;
            background: transparent !important;
            position: static !important;
            width: 100%;
        }

        .search-wrap.in-header .ct-shell {
            padding: 0 !important;
            width: 100%;
            transform: none;
            left: 0;
        }

        .search-wrap.in-header .g-search {
            background: #ffffff;
            border-radius: 18px;
            min-height: 40px;
            box-shadow: 0 4px 12px rgba(15, 23, 42, .12);
            border: 1px solid rgba(15, 23, 42, .08);
            padding: 6px;
            gap: 8px;
        }

        .search-wrap.in-header .g-seg.g-query {
            min-height: 38px;
            padding: 0 12px;
            background: #ffffff;
            border-radius: 999px;
        }

        .search-wrap.in-header .g-seg.g-cat,
        .search-wrap.in-header .g-seg.g-loc {
            border-radius: 999px;
        }

        .search-wrap.in-header .g-seg.g-action {
            display: none !important;
        }

        .home-feed-wrap {
            margin-top: 16px !important;
        }

        .card {
            border: 0;
            box-shadow: none;
            background: transparent;
        }

        .card-header {
            border: 0;
            background: transparent;
            padding: 0 12px 8px;
        }

        .card-title {
            display: none;
        }

        .card-body {
            padding: 0 12px 12px;
        }

        .grid-5 {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 12px;
        }

        .grid-5-item {
            width: auto;
        }

        .sp-card {
            border-radius: 16px;
            box-shadow: 0 6px 16px rgba(15, 23, 42, .10);
        }

        .sp-body {
            padding: 10px;
        }

        .sp-title {
            font-size: 13px;
            line-height: 1.3;
        }

        .sp-desc {
            display: none;
        }

        .sp-price {
            font-size: 15px;
            font-weight: 800;
            color: #e53935;
        }

        .sp-meta {
            font-size: 11px;
            color: #6b7280;
        }

        .sp-meta .rowline {
            flex-direction: column;
            align-items: flex-start;
            gap: 4px;
        }

        .sp-actions {
            display: grid;
            gap: 6px;
            margin-top: 10px;
        }

        .sp-actions .btn {
            min-height: 36px;
            padding: 8px 10px;
            font-size: 12px;
            white-space: normal;
            line-height: 1.2;
        }

        .kebab-wrap {
            display: none;
        }

        .hero-ct-like {
            box-shadow: inset 0 -14px 24px rgba(0,0,0,.08);
        }
    }
</style>

<asp:UpdatePanel ID="up_all" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <section class="hero-ct-like">
            <div class="hero-ct-inner">
                <h1 class="hero-ct-title">Giá Sale, gần bạn!</h1>
            </div>
        </section>
        <div id="homeSearchAnchor" class="home-search-anchor"></div>

        <asp:Panel ID="pnlSearch" runat="server" DefaultButton="btn_Search">
            <section class="search-wrap">
                <asp:PlaceHolder ID="phGuestMobileQuickAuth" runat="server" Visible="false">
                    <div class="ct-guest-auth-wrap">
                        <a href="/dang-nhap" class="ct-guest-auth-btn ct-login">Đăng nhập</a>
                        <a href="/home/dangky.aspx" class="ct-guest-auth-btn ct-register">Đăng ký</a>
                    </div>
                </asp:PlaceHolder>
                <div class="ct-shell">
                    <div class="g-search">
                        <div class="g-seg g-cat">
                            <span class="g-icon"><i class="ti ti-layout-grid"></i></span>
                            <asp:DropDownList ID="ddl_Category" runat="server" CssClass="g-select js-choices" data-choices-search="1" data-choices-placeholder="Tìm danh mục" />
                        </div>

                        <div class="g-seg g-query">
                            <span class="g-icon"><i class="ti ti-search"></i></span>
                            <asp:TextBox ID="txt_Search" runat="server"
                                CssClass="g-input"
                                placeholder="Tìm sản phẩm..." />
                            <button type="button" class="g-btn-inline-mobile" onclick="return AhaHomeSearchRedirect();">Tìm kiếm</button>
                        </div>

                        <div class="g-seg g-loc">
                            <span class="g-icon"><i class="ti ti-map-pin"></i></span>
                            <asp:DropDownList ID="ddl_Location" runat="server" CssClass="g-select js-choices" data-choices-search="1" data-choices-placeholder="Tìm tỉnh thành" />
                        </div>

                        <div class="g-seg g-action">
                            <asp:LinkButton ID="btn_Search" runat="server"
                                CssClass="g-btn"
                                OnClick="Loc_Click"
                                OnClientClick="return AhaHomeSearchRedirect();">
                                Tìm kiếm
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </section>
        </asp:Panel>

        <div class="home-category-inline">
            <uc1:DanhMuc_icon_Home_UC runat="server" ID="DanhMuc_icon_Home_UC" />
        </div>


        <script>
            // Init Choices (safe for WebForms partial postback)
            function initChoices() {
                document.querySelectorAll('.js-choices').forEach(function (el) {
                    if (el.tagName !== 'SELECT') return;

                    if (el.dataset.choicesDone) return;
                    var enableSearch = (el.dataset.choicesSearch === '1');
                    var searchPlaceholder = el.dataset.choicesPlaceholder || 'Tìm kiếm...';
                    new Choices(el, {
                        searchEnabled: enableSearch,
                        searchPlaceholderValue: enableSearch ? searchPlaceholder : null,
                        shouldSort: false,
                        itemSelectText: '',
                        position: 'bottom'
                    });
                    el.dataset.choicesDone = "1";
                });
            }

            function bindMobileQuickFilters() {
                var ddlCat = document.getElementById('<%= ddl_Category.ClientID %>');
                var ddlLoc = document.getElementById('<%= ddl_Location.ClientID %>');
                var txtSearch = document.getElementById('<%= txt_Search.ClientID %>');
                if (!ddlCat || !ddlLoc) return;

                function triggerSearch() {
                    if (typeof AhaHomeSearchRedirect === 'function') {
                        AhaHomeSearchRedirect();
                        return;
                    }
                    if (typeof __doPostBack === 'function') {
                        __doPostBack('<%= btn_Search.UniqueID %>', '');
                    }
                }

                if (ddlCat.dataset.autoSearchBound !== '1') {
                    ddlCat.dataset.autoSearchBound = '1';
                    ddlCat.addEventListener('change', triggerSearch);
                }
                if (ddlLoc.dataset.autoSearchBound !== '1') {
                    ddlLoc.dataset.autoSearchBound = '1';
                    ddlLoc.addEventListener('change', triggerSearch);
                }
                if (txtSearch && txtSearch.dataset.autoSearchBound !== '1') {
                    txtSearch.dataset.autoSearchBound = '1';
                    txtSearch.addEventListener('keydown', function (e) {
                        if (e.key === 'Enter') {
                            e.preventDefault();
                            triggerSearch();
                        }
                    });
                }
            }

            function buildSearchUrl() {
                var keywordEl = document.getElementById('<%= txt_Search.ClientID %>');
                var catEl = document.getElementById('<%= ddl_Category.ClientID %>');
                var locEl = document.getElementById('<%= ddl_Location.ClientID %>');

                var keyword = keywordEl ? keywordEl.value.trim() : '';
                var cat = catEl ? catEl.value : '';
                var loc = locEl ? locEl.value : '';
                var params = new URLSearchParams();
                if (keyword) params.set('q', keyword);
                if (cat) params.set('cat', cat);
                if (loc) params.set('loc', loc);
                params.set('type', 'all');
                params.set('page', '1');

                var qs = params.toString();
                return '/home/tim-kiem.aspx' + (qs ? ('?' + qs) : '');
            }

            function AhaHomeSearchRedirect() {
                try {
                    var url = buildSearchUrl();
                    if (url) {
                        window.location = url;
                        return false;
                    }
                } catch (e) { }
                return true;
            }

            function ensureHomeSearchSticky() {
                var wraps = document.querySelectorAll('.search-wrap');
                if (!wraps.length) return;

                wraps.forEach(function (wrap) {
                    if (!wrap || wrap.dataset.stickyBound === '1') return;
                    wrap.dataset.stickyBound = '1';

                    var ticking = false;
                    var body = document.body;
                    var html = document.documentElement;

                    var spacer = document.createElement('div');
                    spacer.className = 'search-wrap-spacer';
                    wrap.parentNode.insertBefore(spacer, wrap.nextSibling);

                    function setPinnedLayoutClass(enabled, wrapHeight) {
                        if (!body || !html) return;
                        body.classList.toggle('aha-home-root-pinned-search', !!enabled);
                        if (enabled && wrapHeight > 0) {
                            html.style.setProperty('--aha-home-root-search-height', (wrapHeight + 12) + 'px');
                        } else {
                            html.style.removeProperty('--aha-home-root-search-height');
                        }
                    }

                function updateStickyState() {
                    if (!document.body.contains(wrap)) return;
                    ticking = false;
                    var header = document.querySelector('.site-header');
                    var headerHeight = header ? Math.round(header.getBoundingClientRect().height) : 56;

                    var isMobile = window.matchMedia('(max-width: 991.98px)').matches;
                    var forcePinnedAtTop = false;

                    var host = document.getElementById('homeMobileSearchHost');
                    var anchor = document.getElementById('homeSearchAnchor');
                    // Keep search in normal flow (below hero banner) across devices.
                    var useHost = false;
                    if (useHost && anchor && isSmallMobile) {
                        var existing = host.querySelector('.search-wrap');
                        if (existing && existing !== wrap) {
                            existing.remove();
                        }
                        if (wrap.parentNode !== host) {
                            host.appendChild(wrap);
                        }
                        wrap.classList.add('in-header');
                    } else if (anchor && wrap.classList.contains('in-header')) {
                        if (host) {
                            var stray = host.querySelector('.search-wrap');
                            if (stray && stray !== wrap) {
                                stray.remove();
                            }
                        }
                        if (wrap.parentNode !== anchor.parentNode) {
                            anchor.parentNode.insertBefore(wrap, anchor.nextSibling);
                        }
                        wrap.classList.remove('in-header');
                    }

                    headerHeight = header ? Math.round(header.getBoundingClientRect().height) : 56;
                    document.documentElement.style.setProperty('--aha-header-offset', headerHeight + 'px');
                    var stickyTop = Math.max(48, headerHeight + 6);
                    document.documentElement.style.setProperty('--aha-home-sticky-top', stickyTop + 'px');

                    if (wrap.classList.contains('in-header')) {
                        spacer.style.height = '0px';
                        wrap.classList.remove('is-sticky-fixed', 'is-sticky', 'is-root-pinned');
                        wrap.classList.toggle('is-compact', isMobile);
                        setPinnedLayoutClass(false, 0);
                        return;
                    }

                        var isFixed = wrap.classList.contains('is-sticky-fixed');
                        var triggerTop = isFixed
                            ? spacer.getBoundingClientRect().top
                            : wrap.getBoundingClientRect().top;
                        var shouldStick = triggerTop <= stickyTop;
                        var keepSpacer = shouldStick;
                        spacer.style.height = keepSpacer ? (wrap.offsetHeight + 'px') : '0px';

                    wrap.classList.toggle('is-sticky-fixed', shouldStick);
                    wrap.classList.toggle('is-sticky', shouldStick);
                    wrap.classList.toggle('is-root-pinned', forcePinnedAtTop && shouldStick);
                    wrap.classList.toggle('is-compact', shouldStick && isMobile);
                    setPinnedLayoutClass(false, wrap.offsetHeight);
                    }

                    function queueUpdate() {
                        if (ticking) return;
                        ticking = true;
                        window.requestAnimationFrame(updateStickyState);
                    }

                    window.addEventListener('scroll', queueUpdate, { passive: true });
                    document.addEventListener('scroll', queueUpdate, true);
                    window.addEventListener('resize', queueUpdate);
                    document.addEventListener('visibilitychange', queueUpdate);
                    updateStickyState();

                    window.__ahaHomeSearchStickyRefresh = function () {
                        queueUpdate();
                    };
                });
            }

            document.addEventListener('DOMContentLoaded', function () {
                initChoices();
                bindMobileQuickFilters();
                ensureHomeSearchSticky();
            });

            if (window.Sys && Sys.WebForms) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    initChoices();
                    bindMobileQuickFilters();
                    if (window.__ahaHomeSearchStickyRefresh) {
                        window.__ahaHomeSearchStickyRefresh();
                    } else {
                        ensureHomeSearchSticky();
                    }
                });
            }
        </script>

        <!-- ============ MODAL ĐẶT HÀNG ============ -->
        <div class="modal fade" id="modalDatHang"
            data-bs-backdrop="true" data-bs-keyboard="true"
            data-backdrop="true" data-keyboard="true"
            tabindex="-1" aria-hidden="true">

            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">

                    <asp:Panel ID="pnlModalDatHang" runat="server" DefaultButton="but_dathang">
                        <asp:UpdatePanel ID="up_dathang" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="txt_soluong2" EventName="TextChanged" />
                                <asp:AsyncPostBackTrigger ControlID="but_dathang" EventName="Click" />
                            </Triggers>

                            <ContentTemplate>

                                <div class="modal-header">
                                    <h5 class="modal-title fw-bold">Đặt hàng</h5>

                                    <button type="button"
                                        class="modal-xbtn-abs"
                                        aria-label="Đóng"
                                        onclick="ModalHelper.hide('modalDatHang')"
                                        data-bs-dismiss="modal" data-dismiss="modal">
                                        <span>&times;</span>
                                    </button>
                                </div>

                                <div class="modal-body">
                                    <div class="mb-2">
                                        <div class="text-muted small">Sản phẩm</div>
                                        <div class="fw-bold">
                                            <asp:Literal ID="Literal9" runat="server"></asp:Literal>
                                        </div>
                                    </div>

                                    <div class="row g-2 mb-3">
                                        <div class="col-6">
                                            <div class="text-muted small">Giá</div>
                                            <div class="fw-bold">
                                                <asp:Literal ID="Literal10" runat="server"></asp:Literal>
                                                đ
                                            </div>
                                        </div>
                                        <div class="col-6">
                                            <div class="text-muted small">Điểm cần trao đổi</div>
                                            <div class="fw-bold text-success">
                                                <asp:Literal ID="Literal11" runat="server"></asp:Literal>
                                                đ
                                            </div>
                                        </div>
                                    </div>
                                    <div class="mb-2">
  <div class="text-muted small">Ưu đãi</div>
  <div class="fw-bold text-warning">
    <asp:Literal ID="LiteralUuDaiPercent" runat="server"></asp:Literal>% 
  </div>
</div>


                                    <div class="row g-2">
                                        <div class="col-12">
                                            <label class="form-label">Số lượng</label>
                                            <asp:TextBox ID="txt_soluong2" runat="server"
                                                CssClass="form-control"
                                                Text="1"
                                                AutoPostBack="true"
                                                OnTextChanged="txt_soluong2_TextChanged"
                                                MaxLength="6" />
                                        </div>

                                        <div class="col-12">
                                            <label class="form-label">Họ tên người nhận</label>
                                            <asp:TextBox ID="txt_hoten_nguoinhan" runat="server" CssClass="form-control" MaxLength="100" />
                                        </div>
                                        <div class="col-12">
                                            <label class="form-label">SĐT người nhận</label>
                                            <asp:TextBox ID="txt_sdt_nguoinhan" runat="server" CssClass="form-control" MaxLength="20" />
                                        </div>
                                        <div class="col-12">
                                            <label class="form-label">Địa chỉ người nhận</label>
                                            <asp:TextBox ID="txt_diachi_nguoinhan" runat="server" CssClass="form-control" MaxLength="200" />
                                        </div>
                                    </div>
                                </div>

                                <div class="modal-footer d-flex align-items-center justify-content-between">
                                    <button type="button"
                                        class="btn btn-link text-decoration-none"
                                        onclick="ModalHelper.hide('modalDatHang')"
                                        data-bs-dismiss="modal" data-dismiss="modal">
                                        Hủy
                                    </button>

                                    <asp:Button ID="but_dathang" runat="server"
    CssClass="btn btn-success px-4"
    Text="Xác nhận đặt hàng"
    OnClick="but_dathang_Click"
    OnClientClick="return preventDoubleClick(this);" />

                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>

                </div>
            </div>
        </div>

        <!-- ============ MODAL THÊM GIỎ ============ -->
        <div class="modal fade" id="modalAddCart"
            data-bs-backdrop="true" data-bs-keyboard="true"
            data-backdrop="true" data-keyboard="true"
            tabindex="-1" aria-hidden="true">

            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">

                    <asp:Panel ID="pnlModalAddCart" runat="server" DefaultButton="but_add_cart">
                        <asp:UpdatePanel ID="up_add_cart" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="but_add_cart" EventName="Click" />
                            </Triggers>

                            <ContentTemplate>

                                <div class="modal-header">
                                    <h5 class="modal-title fw-bold">Thêm vào giỏ hàng</h5>

                                    <button type="button"
                                        class="modal-xbtn-abs"
                                        aria-label="Đóng"
                                        onclick="ModalHelper.hide('modalAddCart')"
                                        data-bs-dismiss="modal" data-dismiss="modal">
                                        <span>&times;</span>
                                    </button>
                                </div>

                                <div class="modal-body">
                                    <div class="mb-2">
                                        <div class="text-muted small">Sản phẩm</div>
                                        <div class="fw-bold">
                                            <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                                        </div>
                                    </div>

                                    <div class="mb-2">
                                        <label class="form-label">Số lượng</label>
                                        <asp:TextBox ID="txt_soluong1" runat="server" CssClass="form-control" Text="1" MaxLength="6" />
                                    </div>
                                </div>

                                <div class="modal-footer d-flex align-items-center justify-content-between">
                                    <button type="button"
                                        class="btn btn-link text-decoration-none"
                                        onclick="ModalHelper.hide('modalAddCart')"
                                        data-bs-dismiss="modal" data-dismiss="modal">
                                        Hủy
                                    </button>

                                    <asp:Button ID="but_add_cart" runat="server"
                                        CssClass="btn btn-success px-4"
                                        Text="Thêm vào giỏ"
                                        OnClick="but_add_cart_Click" />
                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>

                </div>
            </div>
        </div>

        <!-- ============ LIST ============ -->
        <div class="container-xl home-feed-wrap">
            <div class="card">
                <div class="card-header">
                    <div class="card-title">
                        <asp:Label ID="lblTitle" runat="server" Text="Mới nhất"></asp:Label>
                    </div>
                    <div class="ct-tabs">
                        <button type="button" class="ct-tab is-active">Dành cho bạn</button>
                        <button type="button" class="ct-tab">Gần bạn</button>
                        <button type="button" class="ct-tab">Mới nhất</button>
                        <button type="button" class="ct-tab">Video</button>
                    </div>

                </div>
                <div class="card-body">
                    <div class="grid-5">
                        <asp:Repeater ID="RepeaterTin" runat="server" OnItemDataBound="RepeaterTin_ItemDataBound">
                            <ItemTemplate>
                                <div class="grid-5-item">
                                    <div class="sp-card">
                                        <div class="sp-body">
                                            <div class="thumb-wrap position-relative">
                                                <a href="/<%# Eval("name_en") %>-<%# Eval("id") %>.html" class="text-decoration-none">
                                                    <img class="sp-thumb" src="<%# string.IsNullOrWhiteSpace((Eval("image") + "").Trim()) ? "/uploads/images/macdinh.jpg" : (Eval("image") + "").Trim() %>" alt="<%# Eval("name") %>" />
                                                </a>
                                             <asp:UpdatePanel ID="upHeart" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:LinkButton ID="btnHeart"
                                                        runat="server"
                                                        CssClass="heart-btn"
                                                        CommandArgument='<%# Eval("id") %>'
                                                        OnClick="btnHeart_Click">
                                                        <i class='<%# (bool)Eval("IsTheoDoi")
                                                            ? "ti ti-heart-filled heart-icon active"
                                                            : "ti ti-heart heart-icon" %>'></i>
                                                    </asp:LinkButton>

                                                </ContentTemplate>

                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="btnHeart" EventName="Click" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                            </div>
                                            <a href="/<%# Eval("name_en") %>-<%# Eval("id") %>.html" class="text-decoration-none">
                                                <div class="sp-title"><%# Eval("name") %></div>
                                            </a>
                                            <div class="sp-desc"><%# Eval("description") %></div>
                                            <div class="sp-price"><%# Eval("giaban", "{0:#,##0}") %> đ</div>

                                            <div class="sp-meta">
                                                <div class="sp-time text-muted">
                                                    <svg class="icon-18" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"
                                                        fill="none" stroke="currentColor" stroke-width="2"
                                                        stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                                                        <circle cx="12" cy="12" r="9"></circle>
                                                        <path d="M12 7v5l3 3"></path>
                                                    </svg>
                                                    <asp:Literal ID="lit_timeago" runat="server"></asp:Literal>
                                                </div>

                                                <div class="rowline mt-1">
                                                    <div class="sp-loc text-muted">
                                                        <svg class="icon-18" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"
                                                            fill="none" stroke="currentColor" stroke-width="2"
                                                            stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                                                            <path d="M12 21s-6-5.686-6-10a6 6 0 1 1 12 0c0 4.314-6 10-6 10z"></path>
                                                            <circle cx="12" cy="11" r="2"></circle>
                                                        </svg>
                                                        <span class="loc-text text-muted"><%# Eval("ThanhPhoDisplay") %></span>
                                                    </div>

                                                    <div class="kebab-wrap">
                                                        <button type="button" class="kebab-btn js-kebab-toggle" aria-label="menu">
                                                            <svg class="icon-18" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"
                                                                fill="currentColor" aria-hidden="true">
                                                                <circle cx="12" cy="5" r="2"></circle>
                                                                <circle cx="12" cy="12" r="2"></circle>
                                                                <circle cx="12" cy="19" r="2"></circle>
                                                            </svg>
                                                        </button>
                                                        <div class="kebab-menu">
                                                            <asp:LinkButton ID="btn1" runat="server"
                                                                CssClass="kebab-item"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                CommandName="khongdungnhucau"
                                                                OnClick="AddItemtoSession">
                                                                    <i class="ti ti-x"></i> Không đúng nhu cầu
                                                            </asp:LinkButton>

                                                            <asp:LinkButton ID="btn2" runat="server"
                                                                CssClass="kebab-item"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                CommandName="daxem"
                                                                OnClick="AddItemtoSession">
                                                                    <i class="ti ti-eye-off"></i> Tin đã xem rồi
                                                            </asp:LinkButton>

                                                            <asp:LinkButton ID="btn3" runat="server"
                                                                CssClass="kebab-item"
                                                                CommandArgument='<%# Eval("id") %>'
                                                                CommandName="xakhuvuc"
                                                                OnClick="AddItemtoSession">
                                                                    <i class="ti ti-map-pin-off"></i> Xa khu vực đang sống
                                                            </asp:LinkButton>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <asp:PlaceHolder ID="ph_actions" runat="server">
                                                <div class="sp-actions">
                                                    <asp:Button ID="but_bansanphamnay" runat="server"
                                                        Text="Bán chéo"
                                                        CssClass="btn btn-sm btn-outline-warning"
                                                        OnClick="but_bansanphamnay_Click"
                                                        CommandArgument='<%# Eval("id") %>'
                                                        Visible="false" />

                                                    <asp:Button ID="but_traodoi" runat="server"
                                                        Text="Trao đổi"
                                                        CssClass="btn btn-sm btn-outline-info"
                                                        OnClick="but_traodoi_Click"
                                                        CommandArgument='<%# Eval("id") %>' />

                                                    <asp:Button ID="but_themvaogio" runat="server"
                                                        Text="Thêm vào giỏ hàng"
                                                        CssClass="btn btn-sm btn-outline-success"
                                                        OnClick="but_themvaogio_Click"
                                                        CommandArgument='<%# Eval("id") %>' />
                                                </div>
                                            </asp:PlaceHolder>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div class="mt-3 text-center">
                        <asp:Button ID="but_xemthem" runat="server"
                            CssClass="btn btn-outline-primary"
                            Text="Xem thêm"
                            OnClick="but_xemthem_Click" />
                    </div>

                </div>
            </div>
        </div>  
        <script>
            window.ModalHelper = (function () {

                function _el(id) { return document.getElementById(id); }

                function forceCleanup() {
                    document.querySelectorAll('.modal-backdrop').forEach(function (b) { b.remove(); });
                    document.body.classList.remove('modal-open');
                    document.body.style.paddingRight = '';
                    document.body.style.overflow = '';
                }

                function show(id) {
                    var el = _el(id); if (!el) return;

                    if (window.bootstrap && bootstrap.Modal) {
                        bootstrap.Modal.getOrCreateInstance(el, { backdrop: true, keyboard: true }).show();
                        return;
                    }

                    if (window.jQuery && typeof jQuery(el).modal === "function") {
                        jQuery(el).modal({ backdrop: true, keyboard: true, show: true });
                        return;
                    }

                    el.style.display = 'block';
                    el.classList.add('show');
                    el.removeAttribute('aria-hidden');
                    document.body.classList.add('modal-open');
                    document.body.style.overflow = 'hidden';

                    if (!document.querySelector('.modal-backdrop')) {
                        var bd = document.createElement('div');
                        bd.className = 'modal-backdrop fade show';
                        document.body.appendChild(bd);
                    }
                }

                function hide(id) {
                    var el = _el(id); if (!el) return;

                    if (window.bootstrap && bootstrap.Modal) {
                        try {
                            var ins = bootstrap.Modal.getOrCreateInstance(el);
                            ins.hide();
                        } catch (e) { }

                        forceCleanup();
                        setTimeout(forceCleanup, 50);
                        setTimeout(forceCleanup, 300);
                        return;
                    }

                    if (window.jQuery && typeof jQuery(el).modal === "function") {
                        try { jQuery(el).modal('hide'); } catch (e) { }

                        forceCleanup();
                        setTimeout(forceCleanup, 50);
                        setTimeout(forceCleanup, 300);
                        return;
                    }

                    el.classList.remove('show');
                    el.style.display = 'none';
                    el.setAttribute('aria-hidden', 'true');

                    forceCleanup();
                    setTimeout(forceCleanup, 50);
                    setTimeout(forceCleanup, 300);
                }

                function hookMsAjax() {
                    if (!window.Sys || !Sys.WebForms) return;
                    var prm = Sys.WebForms.PageRequestManager.getInstance();
                    prm.add_endRequest(function () {
                        if (!document.querySelector('.modal.show')) {
                            forceCleanup();
                            setTimeout(forceCleanup, 150);
                        }
                    });
                }

                hookMsAjax();
                return { show: show, hide: hide, cleanup: forceCleanup };
            })();
        </script>
        <script>
            (function () {
                function closeAllMenus(exceptMenu) {
                    document.querySelectorAll('.kebab-menu.show').forEach(function (m) {
                        if (exceptMenu && m === exceptMenu) return;
                        m.classList.remove('show');
                    });
                }

                document.addEventListener('click', function (e) {
                    if (!e.target.closest('.kebab-wrap')) closeAllMenus();
                });

                document.addEventListener('click', function (e) {
                    var btn = e.target.closest('.js-kebab-toggle');
                    if (!btn) return;

                    e.preventDefault();
                    e.stopPropagation();

                    var wrap = btn.closest('.kebab-wrap');
                    if (!wrap) return;

                    var menu = wrap.querySelector('.kebab-menu');
                    if (!menu) return;

                    var isOpen = menu.classList.contains('show');
                    closeAllMenus();
                    if (!isOpen) menu.classList.add('show');
                });
            })();
        </script>
    </ContentTemplate>
</asp:UpdatePanel>
