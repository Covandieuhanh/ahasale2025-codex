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

    @media (max-width: 620px) {
        .grid-5-item { width: calc((100% - 12px) / 2); }
    }

    @media (min-width: 620.02px) and (max-width: 767.98px) {
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

    .sp-source-badge {
        display: inline-flex;
        align-items: center;
        max-width: 100%;
        margin-top: 8px;
        padding: 4px 8px;
        border-radius: 999px;
        background: #eef8f1;
        color: #176a4a;
        font-size: 12px;
        font-weight: 700;
        line-height: 1.2;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
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

    /* ===== Suggest panel (Chợ Tốt style) ===== */
    .search-wrap .g-query { position: relative; }

    .search-suggest-panel {
        position: absolute;
        left: 0;
        right: 0;
        top: calc(100% + 4px);
        background: #fff;
        border: 1px solid #dfe1e5;
        border-radius: 12px;
        box-shadow: 0 12px 32px rgba(32,33,36,.16);
        overflow: hidden;
        display: none;
        z-index: 3500;
    }

    .search-suggest-panel.is-open { display: block; }

    .search-suggest-scroll {
        max-height: 420px;
        overflow-y: auto;
        padding: 6px 0;
    }

    .search-suggest-section {
        padding: 6px 0;
    }

    .search-suggest-section + .search-suggest-section {
        border-top: 1px solid #eef2f7;
    }

    .search-suggest-heading {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 12px;
        padding: 8px 16px 6px;
    }

    .search-suggest-label {
        font-size: 12px;
        font-weight: 800;
        letter-spacing: .04em;
        text-transform: uppercase;
        color: #6b7280;
    }

    .search-suggest-action {
        border: 0;
        background: transparent;
        color: #43b02a;
        font-size: 13px;
        font-weight: 700;
        cursor: pointer;
        padding: 0;
    }

    .search-suggest-item {
        display: flex;
        align-items: center;
        gap: 10px;
        padding: 10px 16px;
        cursor: pointer;
    }

    .search-suggest-remove {
        border: 0;
        background: transparent;
        color: #9ca3af;
        padding: 4px;
        line-height: 1;
        cursor: pointer;
        border-radius: 999px;
        flex: 0 0 auto;
    }

    .search-suggest-remove:hover {
        color: #111827;
        background: #eef2f7;
    }

    .g-location-clear {
        width: 26px;
        height: 26px;
        border: 0;
        border-radius: 999px;
        background: #eef2f7;
        color: #4b5563;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        cursor: pointer;
        flex: 0 0 auto;
    }

    .g-location-clear:hover {
        background: #dbe4ec;
        color: #111827;
    }

    .g-location-clear[hidden] {
        display: none !important;
    }

    .search-suggest-item:hover,
    .search-suggest-item.is-active {
        background: #f5f7fa;
    }

    .search-suggest-text {
        flex: 1 1 auto;
        min-width: 0;
    }

    .search-suggest-title {
        font-weight: 700;
        color: #111827;
        line-height: 1.25rem;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    }

    .search-suggest-meta {
        margin-top: 2px;
        color: #6b7280;
        font-size: 13px;
        display: flex;
        gap: 8px;
        flex-wrap: wrap;
    }

    .search-suggest-empty {
        padding: 14px 16px;
        color: #9aa0a6;
        font-size: 14px;
    }

    .search-suggest-badge {
        background: #eef2f7;
        color: #374151;
        border-radius: 10px;
        padding: 2px 8px;
        font-size: 12px;
        font-weight: 700;
    }

    .search-wrap .g-input::placeholder {
        color: #9aa0a6;
    }

    .search-wrap .g-btn-inline-mobile {
        display: none;
    }

    .search-wrap .g-btn-inline-mobile i {
        font-size: 17px;
        line-height: 1;
    }

    .search-wrap .g-select {
        width: 100%;
        border: 0;
        background: transparent;
        font-size: 15px;
        color: #1f2937;
    }

    .search-wrap .ct-shell {
        width: 100%;
        margin-left: auto;
        margin-right: auto;
    }

    .search-wrap .g-location-trigger {
        border: 0 !important;
        outline: 0;
        box-shadow: none !important;
        background: transparent;
        width: 100%;
        min-height: 38px;
        display: inline-flex;
        align-items: center;
        gap: 8px;
        padding: 0;
        cursor: pointer;
        text-align: left;
    }

    .search-wrap .g-location-label {
        flex: 1 1 auto;
        min-width: 0;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        color: #1f2937;
        font-weight: 600;
    }

    .search-wrap .g-location-caret {
        color: #6b7280;
        display: inline-flex;
        align-items: center;
    }

    .search-wrap .g-seg.is-missing {
        border-color: #ef4444 !important;
        box-shadow: 0 0 0 1px rgba(239, 68, 68, .25);
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

    /* Danh mục dropdown kiểu Chợ Tốt: radio ở bên phải */
    .search-wrap .g-seg.g-cat .choices {
        position: relative;
    }

    .search-wrap .g-seg.g-cat .choices__list--dropdown {
        border-radius: 16px;
        border: 1px solid #e3e8ef;
        box-shadow: 0 16px 30px rgba(15, 23, 42, .18);
        background: #ffffff;
        width: max-content;
        min-width: max(320px, 100%);
        max-width: calc(100vw - 24px);
        overflow-y: hidden !important;
        overflow-x: hidden !important;
    }

    .search-wrap .g-seg.g-cat .choices__list {
        max-height: min(62vh, 440px);
        overflow-y: auto !important;
        overflow-x: hidden !important;
    }

    .search-wrap .g-seg.g-cat .choices__item--choice {
        position: relative;
        display: flex;
        align-items: center;
        min-height: 52px;
        padding: 10px 48px 10px 16px;
        font-size: 16px;
        line-height: 1.15;
        color: #111827;
        white-space: nowrap;
        border-bottom: 1px solid #f0f3f7;
    }

    .search-wrap .g-seg.g-cat .choices__item--choice:last-child {
        border-bottom: 0;
    }

    .search-wrap .g-seg.g-cat .choices__item--choice::after {
        content: "";
        position: absolute;
        right: 14px;
        top: 50%;
        transform: translateY(-50%);
        width: 28px;
        height: 28px;
        border-radius: 999px;
        border: 2px solid #c7cbd1;
        background: #ffffff;
        box-sizing: border-box;
    }

    .search-wrap .g-seg.g-cat .choices__item--choice.is-selected {
        color: #0f2f19;
        font-weight: 700;
        background: #f6fbf8;
    }

    .search-wrap .g-seg.g-cat .choices__item--choice.is-selected::after {
        border-color: #2f8f46;
        background: #2f8f46;
        content: "\2713";
        color: #ffffff;
        font-size: 16px;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        font-weight: 900;
    }

    .search-wrap .g-seg.g-cat .choices__item--choice.is-highlighted {
        background: #f4f7fa;
    }

    /* ===== Desktop/tablet layout (same structure, responsive scale) ===== */
    @media (min-width: 620.02px) {
        .search-wrap {
            display: flex;
            justify-content: center;
        }

        .search-wrap .ct-shell {
            width: min(calc(100% - 24px), 1040px) !important;
            max-width: 1040px !important;
            margin-left: auto !important;
            margin-right: auto !important;
            left: auto !important;
            right: auto !important;
            transform: none !important;
            position: relative;
        }

        .search-wrap .g-search {
            display: grid;
            grid-template-columns: minmax(108px, 1fr) minmax(180px, 1.7fr) minmax(220px, 2fr) auto;
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
            position: relative;
            background: #f3f6f9;
            border-radius: 14px;
            min-width: 0;
            width: 100%;
        }

        .search-wrap .g-seg.g-query {
            width: 100%;
        }

        .search-wrap .g-seg.g-action {
            width: auto;
            padding: 0;
            min-height: 42px;
            background: transparent;
        }

        .search-wrap .g-input,
        .search-wrap .g-location-label,
        .search-wrap .g-seg .choices__item--selectable,
        .search-wrap .g-seg .choices__placeholder {
            min-width: 0;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
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

        /* Desktop: keep popup panels detached from search shell */
        .search-wrap .g-seg.g-cat .choices__list--dropdown {
            top: calc(100% + 8px) !important;
            margin-top: 2px !important;
        }

        .search-wrap .g-query .search-suggest-panel {
            top: calc(100% + 10px) !important;
        }
    }

    @media (min-width: 992px) {
        .search-wrap .g-search {
            align-items: center;
            border-radius: 22px;
            min-height: 60px;
            gap: 10px;
            grid-template-columns: minmax(132px, 1fr) minmax(260px, 1.75fr) minmax(320px, 2.1fr) auto;
        }

        .search-wrap .g-seg {
            min-height: 50px;
            padding: 0 16px;
        }

        .search-wrap .g-seg.g-query {
            width: 100%;
        }

        .search-wrap .g-seg + .g-seg {
            border-top: 0;
            border-left: 0;
            margin-left: 0;
        }

        .search-wrap .g-seg.g-action {
            width: auto;
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
        --aha-home-mobile-chip-height: 0px;
    }

    .hero-ct-like {
        display: flex;
        align-items: center;
        justify-content: center;
        min-height: 292px;
        margin: 0;
        border-radius: 0 0 28px 28px;
        background-image: url('<%= Helper_cl.VersionedUrl("~/uploads/images/home-banner-20260328.png") %>') !important;
        background-position: center 52% !important;
        background-size: cover !important;
        background-repeat: no-repeat !important;
        box-shadow: 0 16px 32px rgba(23, 94, 67, .14) !important;
        overflow: hidden;
        position: relative;
    }

    .hero-ct-like::before,
    .hero-ct-like::after {
        content: none !important;
        display: none !important;
        background: none !important;
    }

    .home-fixed-shortcuts {
        display: none;
    }

    @media (min-width: 620.02px) {
        html.aha-bds-chip-below-topbar .home-fixed-shortcuts {
            position: fixed;
            top: calc(var(--aha-header-offset, 54px) + 8px);
            left: 12px;
            right: 12px;
            z-index: 1045;
            display: flex !important;
            align-items: center;
            gap: 8px;
            pointer-events: none;
        }

        html.aha-bds-chip-below-topbar .home-fixed-shortcuts .hero-mobile-bds-shortcut {
            display: inline-flex;
            align-items: center;
            min-height: 34px;
            padding: 0 14px;
            border-radius: 999px;
            text-decoration: none !important;
            font-size: 13px;
            font-weight: 800;
            color: #ffffff !important;
            background: rgba(15, 23, 42, .26);
            border: 1px solid rgba(255, 255, 255, .28);
            box-shadow: 0 10px 20px rgba(15, 23, 42, .16);
            backdrop-filter: blur(10px);
            pointer-events: auto;
        }
    }

    .hero-ct-inner {
        display: none !important;
    }

    .hero-ct-title {
        display: none !important;
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

    @media (min-width: 620.02px) {
        .search-wrap.is-root-pinned.is-sticky-fixed .ct-shell {
            width: min(calc(100% - 24px), 1040px) !important;
            max-width: 1040px !important;
            margin-left: auto !important;
            margin-right: auto !important;
            padding-left: 0 !important;
            padding-right: 0 !important;
        }
    }

    .home-category-inline .dm-section {
        margin-top: 10px !important;
        margin-bottom: 12px !important;
        transform: none !important;
    }

    @media (max-width: 620px) {
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

    @media (max-width: 620px) {
        .hero-ct-like {
            min-height: 144px;
            height: 144px;
            display: flex !important;
            align-items: center;
            justify-content: center;
            border-radius: 0 0 24px 24px;
            background-position: 50% 50%;
            background-size: cover;
        }

        .home-fixed-shortcuts {
            position: fixed;
            top: calc(var(--aha-header-offset, 54px) + 8px);
            left: 12px;
            right: 12px;
            z-index: 1045;
            display: flex;
            align-items: center;
            gap: 8px;
            pointer-events: none;
        }

        .hero-mobile-bds-shortcut {
            display: inline-flex;
            align-items: center;
            min-height: 32px;
            padding: 0 12px;
            border-radius: 999px;
            text-decoration: none !important;
            font-size: 12px;
            font-weight: 800;
            color: #ffffff !important;
            background: rgba(15, 23, 42, .26);
            border: 1px solid rgba(255, 255, 255, .26);
            box-shadow: 0 10px 20px rgba(15, 23, 42, .16);
            backdrop-filter: blur(10px);
            pointer-events: auto;
        }

        .hero-ct-inner {
            padding: 0 12px 10px;
        }

        .hero-ct-title {
            display: none;
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
            flex-wrap: nowrap;
            align-items: center;
            background: #ffffff;
            border: 1px solid #e5ebf0;
            box-shadow: 0 8px 18px rgba(15, 23, 42, .12);
            gap: 6px;
            padding: 6px;
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
            flex: 1 1 auto;
            background: #ffffff;
            border-color: #e6ebf1;
            gap: 8px;
            padding-right: 4px;
        }

        .search-wrap .g-seg.g-cat {
            order: 2;
            flex: 0 0 auto;
            min-width: 108px;
            max-width: 132px;
            padding: 0 10px;
        }

        .search-wrap .g-seg.g-loc {
            display: none !important;
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
            width: 38px;
            min-width: 38px;
            height: 38px;
            min-height: 38px;
            padding: 0;
            border-radius: 999px;
            border: 1px solid #2f8f46;
            background: #2f8f46;
            color: #fff;
            font-size: 15px;
            font-weight: 700;
            text-decoration: none;
            white-space: nowrap;
            flex: 0 0 auto;
            box-shadow: 0 5px 12px rgba(47, 143, 70, .25);
        }

        .search-wrap .g-btn-inline-mobile .g-btn-inline-label {
            display: none;
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

        .search-wrap .g-seg.g-cat .choices__list--single .choices__item--selectable,
        .search-wrap .g-seg.g-cat .choices__list--single .choices__placeholder {
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
            max-width: 84px;
            display: inline-block;
        }

        .search-wrap .g-seg.g-cat .choices__list--dropdown {
            width: min(92vw, 360px) !important;
            min-width: min(92vw, 360px) !important;
            max-width: calc(100vw - 16px) !important;
            left: auto !important;
            right: 0 !important;
            top: calc(100% + 8px) !important;
        }

        .search-wrap .g-seg.g-cat .choices__list--dropdown .choices__item--choice {
            max-width: none !important;
            overflow: visible !important;
            text-overflow: clip !important;
            white-space: nowrap !important;
        }

        .search-wrap .g-query .search-suggest-panel {
            top: calc(100% + 12px) !important;
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

        .hero-ct-like { box-shadow: 0 16px 32px rgba(23, 94, 67, .14); }
    }

    /* Modal chọn khu vực kiểu Chợ Tốt */
    .ct-loc-modal { position: fixed; inset: 0; z-index: 2500; display: none; }
    .ct-loc-modal.is-open { display: block; }
    .ct-loc-backdrop { position: absolute; inset: 0; background: rgba(0,0,0,.32); }
    .ct-loc-dialog { position: absolute; left: 12px; right: 12px; bottom: 12px; top: 12px; background: #fff; border-radius: 22px; overflow: hidden; display: flex; flex-direction: column; box-shadow: 0 22px 48px rgba(15,23,42,.28); }
    @media (min-width: 768px) { .ct-loc-dialog { left: 50%; right: auto; width: 540px; top: 80px; transform: translateX(-50%); } }
    .ct-loc-header { display: flex; align-items: center; gap: 10px; padding: 14px 16px; border-bottom: 1px solid #eef2f7; }
    .ct-loc-title { flex: 1 1 auto; text-align: center; font-size: 20px; font-weight: 800; }
    .ct-loc-body { flex: 1 1 auto; display: flex; flex-direction: column; overflow: hidden; }
    .ct-loc-tabs { display: grid; grid-template-columns: repeat(3,1fr); border-bottom: 1px solid #eef2f7; }
    .ct-loc-tab { padding: 10px 12px; text-align: center; font-weight: 700; color: #4b5563; cursor: pointer; }
    .ct-loc-tab.is-active { color: #111827; border-bottom: 3px solid #43b02a; }
    .ct-loc-search { padding: 10px 14px; }
    .ct-loc-search input { width: 100%; border: 1px solid #e5ebf0; border-radius: 14px; padding: 10px 12px; font-size: 15px; }
    .ct-loc-list { flex: 1 1 auto; overflow-y: auto; padding: 4px 0 10px; }
    .ct-loc-item { display: flex; align-items: center; justify-content: space-between; padding: 10px 16px; cursor: pointer; }
    .ct-loc-item:hover { background: #f5f7fa; }
    .ct-loc-radio { width: 20px; height: 20px; border: 2px solid #d0d7de; border-radius: 50%; box-sizing: border-box; }
    .ct-loc-item.is-selected .ct-loc-radio { border-color: #43b02a; box-shadow: inset 0 0 0 6px #43b02a; }
    .ct-loc-footer { padding: 12px 16px; border-top: 1px solid #eef2f7; display: flex; justify-content: flex-end; }
    .ct-loc-apply { min-width: 130px; min-height: 44px; border: 0; border-radius: 14px; background: #43b02a; color: #fff; font-weight: 800; box-shadow: 0 10px 20px rgba(67,176,42,.24); }

    .aha-feed-pager {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 8px;
        flex-wrap: wrap;
        margin-top: 16px;
    }

    .aha-feed-pager .pg-link,
    .aha-feed-pager .pg-current {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        min-width: 36px;
        height: 36px;
        padding: 0 12px;
        border-radius: 10px;
        border: 1px solid #d8e1ea;
        text-decoration: none !important;
        font-weight: 700;
        color: #243b53;
        background: #ffffff;
    }

    .aha-feed-pager .pg-link:hover {
        background: #f6f9fc;
        border-color: #c9d6e2;
        color: #102a43;
    }

    .aha-feed-pager .pg-current {
        background: #2f8f46;
        border-color: #2f8f46;
        color: #ffffff;
    }

    .aha-feed-pager .pg-dots {
        color: #7b8794;
        font-weight: 700;
        padding: 0 2px;
    }
</style>

<asp:UpdatePanel ID="up_all" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <div class="home-fixed-shortcuts" aria-label="Lối tắt nhanh">
            <a href="/bat-dong-san" class="hero-mobile-bds-shortcut" title="Bất động sản">Bất động sản</a>
        </div>
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
                            <asp:DropDownList ID="ddl_Category" runat="server" CssClass="g-select js-choices" data-choices-search="0" data-choices-placeholder="Chọn danh mục" />
                        </div>

                        <div class="g-seg g-query">
                            <span class="g-icon"><i class="ti ti-search"></i></span>
                            <asp:TextBox ID="txt_Search" runat="server"
                                CssClass="g-input"
                                placeholder="Tìm sản phẩm..."
                                autocomplete="off"
                                role="combobox"
                                aria-autocomplete="list"
                                aria-haspopup="listbox"
                                aria-expanded="false"
                                aria-controls="homeSuggestList"
                                aria-activedescendant="" />
                            <button type="button" class="g-btn-inline-mobile" onclick="return AhaHomeSearchRedirect();" aria-label="Tìm kiếm">
                                <i class="ti ti-search" aria-hidden="true"></i>
                                <span class="g-btn-inline-label">Tìm kiếm</span>
                            </button>
                            <div id="homeSuggestPanel" class="search-suggest-panel" aria-live="polite" role="listbox">
                                <div class="search-suggest-scroll" id="homeSuggestList"></div>
                            </div>
                        </div>

                        <div class="g-seg g-loc">
                            <button type="button" class="g-location-trigger" onclick="openLocationPicker(); return false;" aria-haspopup="dialog" aria-controls="homeLocationModal">
                                <span class="g-icon"><i class="ti ti-map-pin"></i></span>
                                <span id="homeLocationLabel" class="g-location-label">Địa điểm</span>
                                <span class="g-location-caret"><i class="ti ti-chevron-down"></i></span>
                            </button>
                            <button type="button" id="homeLocationClearBtn" class="g-location-clear" onclick="clearHomeLocation(); return false;" aria-label="Xóa địa điểm" hidden>
                                <i class="ti ti-x"></i>
                            </button>
                            <asp:DropDownList ID="ddl_Location" runat="server" CssClass="g-location-hidden" Style="display:none" />
                            <asp:HiddenField ID="hfProvinceCode" runat="server" />
                            <asp:HiddenField ID="hfDistrictCode" runat="server" />
                            <asp:HiddenField ID="hfWardCode" runat="server" />
                            <asp:HiddenField ID="hfLocationDisplay" runat="server" />
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

        <div id="homeLocationModal" class="ct-loc-modal" aria-hidden="true">
            <div class="ct-loc-backdrop" onclick="closeLocationPicker()"></div>
            <div class="ct-loc-dialog" role="dialog" aria-modal="true" aria-labelledby="ctLocTitle">
                <div class="ct-loc-header">
                    <button type="button" class="btn btn-link p-0" onclick="closeLocationPicker()" aria-label="Đóng"><i class="ti ti-arrow-left"></i></button>
                    <div id="ctLocTitle" class="ct-loc-title">Khu vực</div>
                    <div style="width:32px;"></div>
                </div>
                <div class="ct-loc-body">
                    <div class="ct-loc-tabs">
                        <div id="ctLocTabProvince" class="ct-loc-tab is-active" onclick="switchLocLevel('province')">Tỉnh thành</div>
                        <div id="ctLocTabDistrict" class="ct-loc-tab" onclick="switchLocLevel('district')">Quận huyện</div>
                        <div id="ctLocTabWard" class="ct-loc-tab" onclick="switchLocLevel('ward')">Phường xã</div>
                    </div>
                    <div class="ct-loc-search">
                        <input id="ctLocSearchInput" type="text" placeholder="Tìm nhanh..." oninput="filterLocList()" />
                    </div>
                    <div id="ctLocList" class="ct-loc-list"></div>
                </div>
                <div class="ct-loc-footer">
                    <button type="button" class="ct-loc-apply" onclick="applyLocationPicker()">Áp dụng</button>
                </div>
            </div>
        </div>


        <script>
            var AHA_CHOICES_CSS_ID = 'aha-choices-css';
            var AHA_CHOICES_JS_ID = 'aha-choices-js';
            var AHA_CHOICES_CSS_HREF = 'https://cdn.jsdelivr.net/npm/choices.js/public/assets/styles/choices.min.css';
            var AHA_CHOICES_JS_HREF = 'https://cdn.jsdelivr.net/npm/choices.js/public/assets/scripts/choices.min.js';
            var ahaChoicesLoading = false;
            var ahaChoicesQueue = [];

            function flushChoicesReady(ok) {
                var queue = ahaChoicesQueue.slice(0);
                ahaChoicesQueue = [];
                ahaChoicesLoading = false;
                queue.forEach(function (fn) {
                    try { fn(ok); } catch (e) { }
                });
            }

            function ensureChoicesAssets(callback) {
                if (typeof callback !== 'function') callback = function () { };
                if (typeof window.Choices !== 'undefined') {
                    callback(true);
                    return;
                }

                ahaChoicesQueue.push(callback);
                if (ahaChoicesLoading) return;
                ahaChoicesLoading = true;

                if (!document.getElementById(AHA_CHOICES_CSS_ID)) {
                    var css = document.createElement('link');
                    css.id = AHA_CHOICES_CSS_ID;
                    css.rel = 'stylesheet';
                    css.href = AHA_CHOICES_CSS_HREF;
                    document.head.appendChild(css);
                }

                var existing = document.getElementById(AHA_CHOICES_JS_ID);
                if (existing) {
                    existing.addEventListener('load', function () { flushChoicesReady(true); });
                    existing.addEventListener('error', function () { flushChoicesReady(false); });
                    return;
                }

                var script = document.createElement('script');
                script.id = AHA_CHOICES_JS_ID;
                script.src = AHA_CHOICES_JS_HREF;
                script.defer = true;
                script.onload = function () { flushChoicesReady(true); };
                script.onerror = function () { flushChoicesReady(false); };
                document.head.appendChild(script);
            }

            // Init Choices (safe for WebForms partial postback)
            function initChoices() {
                var selects = document.querySelectorAll('.js-choices');
                if (!selects.length) return;

                if (typeof window.Choices === 'undefined') {
                    ensureChoicesAssets(function (ok) {
                        if (ok) initChoices();
                    });
                    return;
                }

                selects.forEach(function (el) {
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

            var AhaSearchSuggestState = {
                timer: null,
                abortController: null,
                activeIndex: -1,
                items: [],
                open: false,
                source: ''
            };

            var AHA_RECENT_SEARCH_KEY = 'aha-search-history-v1';
            var AHA_RECENT_SEARCH_LIMIT = 8;
            var AHA_LOCATION_STATE_KEY = 'aha-search-location-v1';

            function trackSearchEvent(action, payload) {
                try {
                    var eventModel = payload || {};
                    if (window.dataLayer && typeof window.dataLayer.push === 'function') {
                        window.dataLayer.push({
                            event: 'track_event',
                            event_category: 'search_box',
                            event_action: action,
                            event_label: eventModel.label || '',
                            event_model: eventModel
                        });
                    }
                } catch (e) { }
            }

            function getSearchTrackingContext(overrides) {
                var keywordEl = document.getElementById('<%= txt_Search.ClientID %>');
                var catEl = document.getElementById('<%= ddl_Category.ClientID %>');
                var locEl = document.getElementById('<%= ddl_Location.ClientID %>');
                var hfProvince = document.getElementById('<%= hfProvinceCode.ClientID %>');
                var hfDistrict = document.getElementById('<%= hfDistrictCode.ClientID %>');
                var hfWard = document.getElementById('<%= hfWardCode.ClientID %>');
                var ctx = {
                    label: keywordEl ? (keywordEl.value || '').trim() : '',
                    cat: catEl ? (catEl.value || '').trim() : '',
                    loc: locEl ? (locEl.value || '').trim() : '',
                    province: hfProvince ? (hfProvince.value || '').trim() : '',
                    district: hfDistrict ? (hfDistrict.value || '').trim() : '',
                    ward: hfWard ? (hfWard.value || '').trim() : '',
                    type: 'all'
                };

                if (!ctx.province && ctx.loc) ctx.province = ctx.loc;
                ctx.location_level = ctx.ward ? 'ward' : (ctx.district ? 'district' : (ctx.province ? 'province' : 'none'));
                ctx.query_len = ctx.label.length;

                if (overrides) {
                    Object.keys(overrides).forEach(function (key) {
                        ctx[key] = overrides[key];
                    });
                    if (!ctx.province && ctx.loc) ctx.province = ctx.loc;
                    ctx.location_level = ctx.ward ? 'ward' : (ctx.district ? 'district' : (ctx.province ? 'province' : 'none'));
                    ctx.query_len = (ctx.label || '').length;
                }

                return ctx;
            }

            function getSearchElements() {
                return {
                    keywordEl: document.getElementById('<%= txt_Search.ClientID %>'),
                    catEl: document.getElementById('<%= ddl_Category.ClientID %>'),
                    locEl: document.getElementById('<%= ddl_Location.ClientID %>'),
                    panelEl: document.getElementById('homeSuggestPanel'),
                    listEl: document.getElementById('homeSuggestList')
                };
            }

            function clearRecentSearches() {
                writeRecentSearches([]);
            }

            function removeRecentSearch(item) {
                if (!item || !item.keyword) return;
                var keyword = (item.keyword || '').trim().toLowerCase();
                var cat = (item.categoryId || item.cat || '').trim();
                var loc = (item.locationId || item.loc || '').trim();
                var type = (item.type || 'all').trim();
                var key = [keyword, cat, loc, type].join('|');
                var list = readRecentSearches().filter(function (x) {
                    var xKey = [((x.keyword || '').trim().toLowerCase()), (x.cat || '').trim(), (x.loc || '').trim(), ((x.type || 'all').trim())].join('|');
                    return xKey !== key;
                });
                writeRecentSearches(list);
            }

            function readLocationState() {
                try {
                    var raw = localStorage.getItem(AHA_LOCATION_STATE_KEY);
                    var item = raw ? JSON.parse(raw) : null;
                    if (!item || typeof item !== 'object') return null;
                    return item;
                } catch (e) {
                    return null;
                }
            }

            function writeLocationState(item) {
                try {
                    localStorage.setItem(AHA_LOCATION_STATE_KEY, JSON.stringify(item || {}));
                } catch (e) { }
            }

            function updateLocationClearState(label) {
                var clearBtn = document.getElementById('homeLocationClearBtn');
                if (!clearBtn) return;
                clearBtn.hidden = !label || !label.trim() || label === 'Địa điểm';
            }

            function escapeHtml(s) {
                return (s || '')
                    .replace(/&/g, '&amp;')
                    .replace(/</g, '&lt;')
                    .replace(/>/g, '&gt;')
                    .replace(/"/g, '&quot;')
                    .replace(/'/g, '&#39;');
            }

            function toSearchSlug(keyword) {
                var s = (keyword || '').toString().trim().toLowerCase();
                if (!s) return '';
                s = s.normalize ? s.normalize('NFD').replace(/[\u0300-\u036f]/g, '') : s;
                s = s.replace(/đ/g, 'd').replace(/[^a-z0-9\s]/g, ' ');
                s = s.replace(/\s+/g, '-').replace(/-+/g, '-').replace(/^-|-$/g, '');
                return s;
            }

            function readRecentSearches() {
                try {
                    var raw = localStorage.getItem(AHA_RECENT_SEARCH_KEY);
                    var list = raw ? JSON.parse(raw) : [];
                    if (!Array.isArray(list)) return [];
                    return list.filter(function (x) { return x && typeof x.keyword === 'string'; });
                } catch (e) {
                    return [];
                }
            }

            function writeRecentSearches(items) {
                try {
                    localStorage.setItem(AHA_RECENT_SEARCH_KEY, JSON.stringify(items || []));
                } catch (e) { }
            }

            function saveRecentSearch(item) {
                if (!item || !item.keyword || !item.keyword.trim()) return;
                var keyword = item.keyword.trim();
                var cat = (item.cat || '').trim();
                var loc = (item.loc || '').trim();
                var type = (item.type || 'all').trim();
                var key = [keyword.toLowerCase(), cat, loc, type].join('|');
                var list = readRecentSearches();
                list = list.filter(function (x) {
                    var k = [(x.keyword || '').toLowerCase(), x.cat || '', x.loc || '', x.type || 'all'].join('|');
                    return k !== key;
                });
                list.unshift({
                    keyword: keyword,
                    cat: cat,
                    loc: loc,
                    type: type,
                    savedAt: new Date().getTime()
                });
                if (list.length > AHA_RECENT_SEARCH_LIMIT) {
                    list = list.slice(0, AHA_RECENT_SEARCH_LIMIT);
                }
                writeRecentSearches(list);
            }

            function openSuggestPanel() {
                var els = getSearchElements();
                if (!els.panelEl || !els.keywordEl) return;
                els.panelEl.classList.add('is-open');
                els.keywordEl.setAttribute('aria-expanded', 'true');
                AhaSearchSuggestState.open = true;
            }

            function closeSuggestPanel() {
                var els = getSearchElements();
                if (!els.panelEl || !els.keywordEl) return;
                els.panelEl.classList.remove('is-open');
                els.keywordEl.setAttribute('aria-expanded', 'false');
                els.keywordEl.setAttribute('aria-activedescendant', '');
                AhaSearchSuggestState.open = false;
                AhaSearchSuggestState.activeIndex = -1;
                AhaSearchSuggestState.source = '';
            }

            function renderSuggestEmpty(message) {
                var els = getSearchElements();
                if (!els.listEl) return;
                els.listEl.innerHTML = '';
                var empty = document.createElement('div');
                empty.className = 'search-suggest-empty';
                empty.textContent = message || 'Không có gợi ý phù hợp';
                els.listEl.appendChild(empty);
                AhaSearchSuggestState.items = [];
                AhaSearchSuggestState.activeIndex = -1;
                AhaSearchSuggestState.source = '';
                openSuggestPanel();
            }

            function updateSuggestActiveState() {
                var els = getSearchElements();
                if (!els.listEl) return;
                var nodes = els.listEl.querySelectorAll('.search-suggest-item');
                nodes.forEach(function (node, idx) {
                    node.classList.toggle('is-active', idx === AhaSearchSuggestState.activeIndex);
                    node.setAttribute('aria-selected', idx === AhaSearchSuggestState.activeIndex ? 'true' : 'false');
                });
                if (!els.keywordEl) return;
                if (AhaSearchSuggestState.activeIndex >= 0 && nodes[AhaSearchSuggestState.activeIndex]) {
                    var activeNode = nodes[AhaSearchSuggestState.activeIndex];
                    els.keywordEl.setAttribute('aria-activedescendant', activeNode.id || '');
                    if (typeof activeNode.scrollIntoView === 'function') {
                        activeNode.scrollIntoView({ block: 'nearest' });
                    }
                } else {
                    els.keywordEl.setAttribute('aria-activedescendant', '');
                }
            }

            function createSuggestSection(title, items, source, options) {
                options = options || {};
                var section = document.createElement('div');
                section.className = 'search-suggest-section';

                var heading = document.createElement('div');
                heading.className = 'search-suggest-heading';
                heading.innerHTML = '<span class="search-suggest-label">' + escapeHtml(title || 'Gợi ý tìm kiếm') + '</span>';
                if (options.showClearRecent) {
                    heading.innerHTML += '<button type="button" class="search-suggest-action" data-role="clear-recent">Xóa</button>';
                }
                section.appendChild(heading);

                var frag = document.createDocumentFragment();
                (items || []).forEach(function (it, idx) {
                    var div = document.createElement('div');
                    div.className = 'search-suggest-item';
                    div.id = 'homeSuggestOption' + idx;
                    div.setAttribute('data-index', String(idx));
                    div.setAttribute('data-source', source || 'suggest');
                    div.setAttribute('role', 'option');
                    div.setAttribute('aria-selected', 'false');
                    var iconClass = source === 'recent' ? 'ti-history' : ((it.source || '') === 'popular' ? 'ti-trending-up' : 'ti-search');
                    div.innerHTML =
                        '<span class="g-icon"><i class="ti ' + iconClass + '"></i></span>' +
                        '<div class="search-suggest-text">' +
                            '<div class="search-suggest-title">' + escapeHtml(it.title || it.keyword || '') + '</div>' +
                            '<div class="search-suggest-meta">' +
                                (it.categoryName ? ('<span class="search-suggest-badge">' + escapeHtml(it.categoryName) + '</span>') : '') +
                                (it.locationName ? ('<span>' + escapeHtml(it.locationName) + '</span>') : '') +
                            '</div>' +
                        '</div>' +
                        (source === 'recent'
                            ? '<button type="button" class="search-suggest-remove" data-role="remove-recent" aria-label="Xóa tìm kiếm gần đây"><i class="ti ti-x"></i></button>'
                            : '');
                    frag.appendChild(div);
                });

                section.appendChild(frag);
                return section;
            }

            function renderSuggestSections(sections) {
                var els = getSearchElements();
                if (!els.listEl) return;
                els.listEl.innerHTML = '';
                AhaSearchSuggestState.items = [];
                AhaSearchSuggestState.activeIndex = -1;

                var validSections = (sections || []).filter(function (section) {
                    return section && Array.isArray(section.items) && section.items.length;
                });

                if (!validSections.length) {
                    renderSuggestEmpty('Không có gợi ý phù hợp');
                    return;
                }

                var globalIndex = 0;
                validSections.forEach(function (section) {
                    var mappedItems = section.items.map(function (item) {
                        var clone = Object.assign({}, item);
                        clone._globalIndex = globalIndex++;
                        clone._sectionSource = section.source || item.source || 'suggest';
                        return clone;
                    });

                    mappedItems.forEach(function (item) {
                        AhaSearchSuggestState.items.push(item);
                    });

                    var sectionNode = createSuggestSection(section.title, mappedItems.map(function (item) {
                        return Object.assign({}, item, { _globalIndex: item._globalIndex });
                    }).map(function (item) { return item; }), section.source, {
                        showClearRecent: !!section.showClearRecent
                    });

                    var nodes = sectionNode.querySelectorAll('.search-suggest-item');
                    nodes.forEach(function (node, idx) {
                        node.id = 'homeSuggestOption' + mappedItems[idx]._globalIndex;
                        node.setAttribute('data-index', String(mappedItems[idx]._globalIndex));
                        node.setAttribute('data-source', mappedItems[idx]._sectionSource || section.source || 'suggest');
                    });

                    els.listEl.appendChild(sectionNode);
                });
                AhaSearchSuggestState.source = validSections.map(function (x) { return x.source || 'suggest'; }).join(',');
                openSuggestPanel();
            }

            function renderSuggestItems(items, source) {
                renderSuggestSections([{
                    title: source === 'recent' ? 'Tìm kiếm gần đây' : 'Gợi ý tìm kiếm',
                    items: items || [],
                    source: source || 'suggest',
                    showClearRecent: source === 'recent'
                }]);
            }

            function renderRecentSuggestions() {
                var list = readRecentSearches().map(function (x) {
                    return {
                        title: x.keyword,
                        keyword: x.keyword,
                        categoryId: x.cat || '',
                        locationId: x.loc || '',
                        type: x.type || 'all',
                        source: 'recent'
                    };
                });
                renderSuggestSections([{
                    title: 'Tìm kiếm gần đây',
                    items: list,
                    source: 'recent',
                    showClearRecent: true
                }]);
            }

            function getMatchingRecentSuggestions(keyword) {
                var needle = (keyword || '').trim().toLowerCase();
                if (!needle) return [];
                return readRecentSearches()
                    .filter(function (x) {
                        return (x.keyword || '').toLowerCase().indexOf(needle) >= 0;
                    })
                    .map(function (x) {
                        return {
                            title: x.keyword,
                            keyword: x.keyword,
                            categoryId: x.cat || '',
                            locationId: x.loc || '',
                            type: x.type || 'all',
                            source: 'recent'
                        };
                    })
                    .slice(0, 3);
            }

            function fetchSearchSuggestions(keyword, cat, loc) {
                if (AhaSearchSuggestState.abortController) {
                    AhaSearchSuggestState.abortController.abort();
                }
                var ctrl = new AbortController();
                AhaSearchSuggestState.abortController = ctrl;

                var qs = new URLSearchParams();
                qs.set('q', keyword || '');
                if (cat) qs.set('cat', cat);
                if (loc) qs.set('loc', loc);
                qs.set('limit', '8');

                return fetch('/home/search-suggest.ashx?' + qs.toString(), {
                    method: 'GET',
                    signal: ctrl.signal,
                    headers: { 'Accept': 'application/json' }
                }).then(function (r) { return r.json(); });
            }

            function initSearchSuggest() {
                var els = getSearchElements();
                if (!els.keywordEl || !els.listEl || !els.panelEl) return;
                if (els.keywordEl.dataset.suggestBound === '1') return;
                els.keywordEl.dataset.suggestBound = '1';

                els.keywordEl.addEventListener('focus', function () {
                    var q = (els.keywordEl.value || '').trim();
                    if (q.length < 2) {
                        trackSearchEvent('search_panel_open', getSearchTrackingContext({ label: q }));
                        renderRecentSuggestions();
                    }
                });

                els.keywordEl.addEventListener('input', function () {
                    var keyword = (els.keywordEl.value || '').trim();
                    var cat = els.catEl ? (els.catEl.value || '').trim() : '';
                    var loc = els.locEl ? (els.locEl.value || '').trim() : '';

                    if (AhaSearchSuggestState.timer) {
                        clearTimeout(AhaSearchSuggestState.timer);
                    }

                    if (!keyword) {
                        renderRecentSuggestions();
                        return;
                    }
                    if (keyword.length < 2) {
                        renderSuggestEmpty('Nhập ít nhất 2 ký tự để xem gợi ý');
                        return;
                    }

                    AhaSearchSuggestState.timer = setTimeout(function () {
                        fetchSearchSuggestions(keyword, cat, loc)
                            .then(function (res) {
                                if (!res || res.ok !== true || !Array.isArray(res.items)) {
                                    trackSearchEvent('search_suggest_empty', getSearchTrackingContext({ label: keyword }));
                                    renderSuggestEmpty('Không có gợi ý phù hợp');
                                    return;
                                }
                                var items = res.items.map(function (it) {
                                    return {
                                        id: it.id || 0,
                                        title: it.title || '',
                                        keyword: it.title || '',
                                        categoryId: it.categoryId || '',
                                        categoryName: it.categoryName || '',
                                        locationId: it.locationId || '',
                                        locationName: it.locationName || '',
                                        type: 'all',
                                        url: it.url || '',
                                        source: it.source || 'suggest'
                                    };
                                });
                                var recentItems = getMatchingRecentSuggestions(keyword);
                                var realSuggestItems = items.filter(function (it) { return (it.source || 'suggest') !== 'popular'; }).slice(0, 5);
                                var popularItems = items.filter(function (it) { return (it.source || '') === 'popular'; }).slice(0, 4);

                                var sections = [];
                                if (recentItems.length) {
                                    sections.push({
                                        title: 'Tìm kiếm gần đây',
                                        items: recentItems,
                                        source: 'recent',
                                        showClearRecent: true
                                    });
                                }
                                if (realSuggestItems.length) {
                                    sections.push({
                                        title: 'Gợi ý phù hợp',
                                        items: realSuggestItems,
                                        source: 'suggest'
                                    });
                                }
                                if (popularItems.length) {
                                    sections.push({
                                        title: 'Từ khóa phổ biến',
                                        items: popularItems,
                                        source: 'popular'
                                    });
                                }

                                if (!sections.length) {
                                    trackSearchEvent('search_suggest_empty', getSearchTrackingContext({ label: keyword }));
                                    renderSuggestEmpty('Không có gợi ý phù hợp');
                                    return;
                                }
                                renderSuggestSections(sections);
                            })
                            .catch(function (err) {
                                if (err && err.name === 'AbortError') return;
                                renderSuggestEmpty('Không thể tải gợi ý lúc này');
                            });
                    }, 180);
                });

                els.listEl.addEventListener('mousedown', function (e) {
                    e.preventDefault();
                });

                els.listEl.addEventListener('click', function (e) {
                    var clearBtn = e.target.closest('[data-role="clear-recent"]');
                    if (clearBtn) {
                        clearRecentSearches();
                        renderRecentSuggestions();
                        trackSearchEvent('recent_search_clear', getSearchTrackingContext({ label: '' }));
                        return;
                    }
                    var removeBtn = e.target.closest('[data-role="remove-recent"]');
                    if (removeBtn) {
                        var removeRow = e.target.closest('.search-suggest-item');
                        if (!removeRow) return;
                        var removeIdx = parseInt(removeRow.getAttribute('data-index') || '-1', 10);
                        if (removeIdx < 0 || removeIdx >= AhaSearchSuggestState.items.length) return;
                        var removeItem = AhaSearchSuggestState.items[removeIdx];
                        removeRecentSearch(removeItem);
                        renderRecentSuggestions();
                        trackSearchEvent('recent_search_remove', getSearchTrackingContext({ label: removeItem.keyword || removeItem.title || '' }));
                        return;
                    }
                    var row = e.target.closest('.search-suggest-item');
                    if (!row) return;
                    var idx = parseInt(row.getAttribute('data-index') || '-1', 10);
                    if (idx < 0 || idx >= AhaSearchSuggestState.items.length) return;
                    var item = AhaSearchSuggestState.items[idx];
                    chooseSuggestItem(item, row.getAttribute('data-source') || 'suggest');
                });

                if (!window.__ahaSuggestDocClickBound) {
                    window.__ahaSuggestDocClickBound = true;
                    document.addEventListener('click', function (e) {
                        var curr = getSearchElements();
                        if (!curr.keywordEl) return;
                        var qWrap = curr.keywordEl.closest('.g-query');
                        if (!qWrap) return;
                        if (qWrap.contains(e.target)) return;
                        closeSuggestPanel();
                    });
                }

                window.AhaHomeSearchHandleKeydown = function (e) {
                    if (!AhaSearchSuggestState.open || !AhaSearchSuggestState.items.length) return false;
                    if (e.key === 'ArrowDown') {
                        e.preventDefault();
                        AhaSearchSuggestState.activeIndex = Math.min(AhaSearchSuggestState.items.length - 1, AhaSearchSuggestState.activeIndex + 1);
                        updateSuggestActiveState();
                        return true;
                    }
                    if (e.key === 'ArrowUp') {
                        e.preventDefault();
                        AhaSearchSuggestState.activeIndex = Math.max(0, AhaSearchSuggestState.activeIndex - 1);
                        updateSuggestActiveState();
                        return true;
                    }
                    if (e.key === 'Escape') {
                        e.preventDefault();
                        closeSuggestPanel();
                        return true;
                    }
                    if (e.key === 'Enter' && AhaSearchSuggestState.activeIndex >= 0) {
                        e.preventDefault();
                        var item = AhaSearchSuggestState.items[AhaSearchSuggestState.activeIndex];
                        chooseSuggestItem(item, 'keyboard');
                        return true;
                    }
                    return false;
                };
            }

            function chooseSuggestItem(item, source) {
                if (!item) return;
                var els = getSearchElements();
                var keyword = (item.keyword || item.title || '').trim();
                var cat = (item.categoryId || '').trim();
                var loc = (item.locationId || '').trim();
                var type = (item.type || 'all').trim();

                if (els.keywordEl) els.keywordEl.value = keyword;
                if (els.catEl && cat && els.catEl.querySelector('option[value="' + cat + '"]')) els.catEl.value = cat;
                if (els.locEl && loc && els.locEl.querySelector('option[value="' + loc + '"]')) els.locEl.value = loc;

                saveRecentSearch({ keyword: keyword, cat: cat, loc: loc, type: type });
                trackSearchEvent(source === 'recent' ? 'recent_search_click' : 'search_suggestion_click', getSearchTrackingContext({
                    label: keyword,
                    cat: cat || '',
                    loc: loc || '',
                    type: type
                }));
                closeSuggestPanel();
            }

            // ===== Chợ Tốt style location picker =====
            var LocStore = { provinces: null, districts: null, wards: null, promise: null };
            var LocState = { level: 'province', province: '', district: '', ward: '' };

            function loadLocData() {
                if (LocStore.promise) return LocStore.promise;
                LocStore.promise = Promise.all([
                    fetch('/assets/data/tinh_tp.json').then(r => r.json()),
                    fetch('/assets/data/quan_huyen.json').then(r => r.json()),
                    fetch('/assets/data/xa_phuong.json').then(r => r.json())
                ]).then(parts => {
                    LocStore.provinces = parts[0] || {};
                    LocStore.districts = parts[1] || {};
                    LocStore.wards = parts[2] || {};
                    return LocStore;
                }).catch(() => { LocStore.promise = null; return LocStore; });
                return LocStore.promise;
            }

            function updateLocTabs() {
                ['province','district','ward'].forEach(function(lvl){
                    var el = document.getElementById('ctLocTab' + lvl.charAt(0).toUpperCase() + lvl.slice(1));
                    if (el) el.classList.toggle('is-active', LocState.level === lvl);
                });
                var title = document.getElementById('ctLocTitle');
                if (title) title.textContent = LocState.level === 'province' ? 'Tỉnh thành' : (LocState.level === 'district' ? 'Quận huyện' : 'Phường xã');
            }

            function currentList() {
                var q = (document.getElementById('ctLocSearchInput')?.value || '').trim().toLowerCase();
                if (LocState.level === 'province') {
                    return Object.keys(LocStore.provinces || {}).map(code => {
                        var it = LocStore.provinces[code]; return { code, name: it.name_with_type || it.name || '', sel: LocState.province === code };
                    }).filter(x => !q || x.name.toLowerCase().indexOf(q) !== -1);
                }
                if (LocState.level === 'district') {
                    return Object.keys(LocStore.districts || {}).map(code => {
                        var it = LocStore.districts[code]; if (!it || it.parent_code !== LocState.province) return null;
                        return { code, name: it.name_with_type || it.name || '', sel: LocState.district === code };
                    }).filter(Boolean).filter(x => !q || x.name.toLowerCase().indexOf(q) !== -1);
                }
                return Object.keys(LocStore.wards || {}).map(code => {
                    var it = LocStore.wards[code]; if (!it || it.parent_code !== LocState.district) return null;
                    return { code, name: it.name_with_type || it.name || '', sel: LocState.ward === code };
                }).filter(Boolean).filter(x => !q || x.name.toLowerCase().indexOf(q) !== -1);
            }

            function buildHomeLocationDisplay(province, district, ward) {
                var parts = [];
                var provinceItem = province ? (LocStore.provinces || {})[province] : null;
                var districtItem = district ? (LocStore.districts || {})[district] : null;
                var wardItem = ward ? (LocStore.wards || {})[ward] : null;

                if (wardItem) parts.push(wardItem.name_with_type || wardItem.name || '');
                if (districtItem) parts.push(districtItem.name_with_type || districtItem.name || '');
                if (provinceItem) parts.push(provinceItem.name_with_type || provinceItem.name || '');

                parts = parts.filter(function (x) { return (x || '').trim() !== ''; });
                return parts.join(', ');
            }

            function renderLocList() {
                var host = document.getElementById('ctLocList');
                if (!host) return;
                host.innerHTML = '';
                currentList().forEach(function (item) {
                    var div = document.createElement('div');
                    div.className = 'ct-loc-item' + (item.sel ? ' is-selected' : '');
                    div.innerHTML = '<div>' + item.name + '</div><div class="ct-loc-radio"></div>';
                    div.onclick = function () {
                        if (LocState.level === 'province') { LocState.province = item.code; LocState.district = ''; LocState.ward = ''; switchLocLevel('district', true); }
                        else if (LocState.level === 'district') { LocState.district = item.code; LocState.ward = ''; switchLocLevel('ward', true); }
                        else { LocState.ward = item.code; }
                        renderLocList();
                    };
                    host.appendChild(div);
                });
            }

            function switchLocLevel(level, skipRender) {
                LocState.level = level;
                updateLocTabs();
                if (!skipRender) renderLocList();
            }

            function openLocationPicker() {
                var modal = document.getElementById('homeLocationModal');
                if (!modal) return false;
                modal.classList.add('is-open');
                modal.setAttribute('aria-hidden','false');
                document.body.classList.add('home-location-modal-open');
                loadLocData().then(function () {
                    var hfP = document.getElementById('<%= hfProvinceCode.ClientID %>');
                    var hfD = document.getElementById('<%= hfDistrictCode.ClientID %>');
                    var hfW = document.getElementById('<%= hfWardCode.ClientID %>');
                    LocState.province = hfP ? hfP.value : '';
                    LocState.district = hfD ? hfD.value : '';
                    LocState.ward = hfW ? hfW.value : '';
                    LocState.level = LocState.province ? (LocState.district ? (LocState.ward ? 'ward' : 'ward') : 'district') : 'province';
                    document.getElementById('ctLocSearchInput').value = '';
                    switchLocLevel(LocState.level, true);
                    renderLocList();
                });
                return false;
            }

            function closeLocationPicker() {
                var modal = document.getElementById('homeLocationModal');
                if (!modal) return;
                modal.classList.remove('is-open');
                modal.setAttribute('aria-hidden','true');
                document.body.classList.remove('home-location-modal-open');
            }

            function filterLocList() { renderLocList(); }

            function applyLocationPicker() {
                var hfP = document.getElementById('<%= hfProvinceCode.ClientID %>');
                var hfD = document.getElementById('<%= hfDistrictCode.ClientID %>');
                var hfW = document.getElementById('<%= hfWardCode.ClientID %>');
                var hfDisp = document.getElementById('<%= hfLocationDisplay.ClientID %>');
                var ddlLoc = document.getElementById('<%= ddl_Location.ClientID %>');
                if (hfP) hfP.value = LocState.province;
                if (hfD) hfD.value = LocState.district;
                if (hfW) hfW.value = LocState.ward;
                if (ddlLoc) ddlLoc.value = LocState.province;
                var display = buildHomeLocationDisplay(LocState.province, LocState.district, LocState.ward);
                if (hfDisp) hfDisp.value = display;
                var lbl = document.getElementById('homeLocationLabel');
                if (lbl) lbl.textContent = display || 'Địa điểm';
                updateLocationClearState(display || '');
                writeLocationState({
                    province: LocState.province || '',
                    district: LocState.district || '',
                    ward: LocState.ward || '',
                    display: display || '',
                    savedAt: new Date().getTime()
                });
                validateHomeSearchRequired({ notify: false });
                trackSearchEvent('search_location_apply', getSearchTrackingContext({
                    label: '',
                    province: LocState.province || '',
                    district: LocState.district || '',
                    ward: LocState.ward || '',
                    loc: LocState.province || ''
                }));
                closeLocationPicker();
            }

            function clearHomeLocation() {
                var hfP = document.getElementById('<%= hfProvinceCode.ClientID %>');
                var hfD = document.getElementById('<%= hfDistrictCode.ClientID %>');
                var hfW = document.getElementById('<%= hfWardCode.ClientID %>');
                var hfDisp = document.getElementById('<%= hfLocationDisplay.ClientID %>');
                var ddlLoc = document.getElementById('<%= ddl_Location.ClientID %>');
                var lbl = document.getElementById('homeLocationLabel');

                LocState.province = '';
                LocState.district = '';
                LocState.ward = '';

                if (hfP) hfP.value = '';
                if (hfD) hfD.value = '';
                if (hfW) hfW.value = '';
                if (hfDisp) hfDisp.value = '';
                if (ddlLoc) ddlLoc.value = '';
                if (lbl) lbl.textContent = 'Địa điểm';
                updateLocationClearState('');
                writeLocationState({});
                validateHomeSearchRequired({ notify: false });
                trackSearchEvent('search_location_clear', getSearchTrackingContext({ label: '', province: '', district: '', ward: '', loc: '' }));
                closeLocationPicker();
                return false;
            }

            function restoreStoredLocation() {
                var hfP = document.getElementById('<%= hfProvinceCode.ClientID %>');
                var hfD = document.getElementById('<%= hfDistrictCode.ClientID %>');
                var hfW = document.getElementById('<%= hfWardCode.ClientID %>');
                var hfDisp = document.getElementById('<%= hfLocationDisplay.ClientID %>');
                var ddlLoc = document.getElementById('<%= ddl_Location.ClientID %>');
                var lbl = document.getElementById('homeLocationLabel');
                var hasQueryLocation = false;

                if (hfP && hfP.value) hasQueryLocation = true;
                if (hfD && hfD.value) hasQueryLocation = true;
                if (hfW && hfW.value) hasQueryLocation = true;
                if (hasQueryLocation) return;

                var stored = readLocationState();
                if (!stored) return;

                if (hfP) hfP.value = stored.province || '';
                if (hfD) hfD.value = stored.district || '';
                if (hfW) hfW.value = stored.ward || '';
                if (hfDisp) hfDisp.value = stored.display || '';
                if (ddlLoc && stored.province && ddlLoc.querySelector('option[value="' + stored.province + '"]')) {
                    ddlLoc.value = stored.province;
                }
                if (lbl) lbl.textContent = stored.display || 'Địa điểm';
                updateLocationClearState(stored.display || '');
            }

            function bindMobileQuickFilters() {
                var txtSearch = document.getElementById('<%= txt_Search.ClientID %>');
                if (!txtSearch || txtSearch.dataset.autoSearchBound === '1') return;
                txtSearch.dataset.autoSearchBound = '1';
                txtSearch.addEventListener('keydown', function (e) {
                    if (typeof window.AhaHomeSearchHandleKeydown === 'function') {
                        if (window.AhaHomeSearchHandleKeydown(e)) return;
                    }
                    if (e.key === 'Enter') {
                        e.preventDefault();
                    }
                });
            }

            function toggleSearchMissingStyles(flags) {
                flags = flags || {};
                var catSeg = document.querySelector('.search-wrap .g-seg.g-cat');
                var querySeg = document.querySelector('.search-wrap .g-seg.g-query');
                var locSeg = document.querySelector('.search-wrap .g-seg.g-loc');
                if (catSeg) catSeg.classList.toggle('is-missing', !!flags.catMissing);
                if (querySeg) querySeg.classList.toggle('is-missing', !!flags.keywordMissing);
                if (locSeg) locSeg.classList.toggle('is-missing', !!flags.locationMissing);
            }

            function validateHomeSearchRequired(opts) {
                opts = opts || {};
                var keywordEl = document.getElementById('<%= txt_Search.ClientID %>');
                var catEl = document.getElementById('<%= ddl_Category.ClientID %>');
                var locEl = document.getElementById('<%= ddl_Location.ClientID %>');
                var hfProvince = document.getElementById('<%= hfProvinceCode.ClientID %>');
                var keyword = keywordEl ? (keywordEl.value || '').trim() : '';
                var cat = catEl ? (catEl.value || '').trim() : '';
                var loc = locEl ? (locEl.value || '').trim() : '';
                var province = hfProvince ? (hfProvince.value || '').trim() : '';
                if (!province && loc) province = loc;
                var hasAny = !!(keyword || cat || province);
                var result = {
                    ok: hasAny,
                    keywordMissing: !hasAny && !keyword,
                    catMissing: !hasAny && !cat,
                    locationMissing: !hasAny && !province
                };

                toggleSearchMissingStyles(result);

                if (!result.ok && opts.notify !== false) {
                    alert('Chỉ cần chọn ít nhất 1 trong 3 trường Danh mục, Từ khóa hoặc Địa điểm để tìm kiếm.');
                    if (keywordEl) {
                        try { keywordEl.focus(); } catch (e) { }
                    } else if (catEl) {
                        try { catEl.focus(); } catch (e) { }
                    } else if (result.locationMissing) {
                        try { openLocationPicker(); } catch (e) { }
                    }
                }

                return result;
            }

            function bindRequiredFieldState() {
                var keywordEl = document.getElementById('<%= txt_Search.ClientID %>');
                var catEl = document.getElementById('<%= ddl_Category.ClientID %>');
                var locEl = document.getElementById('<%= ddl_Location.ClientID %>');
                var clearBtn = document.getElementById('homeLocationClearBtn');
                var trigger = document.querySelector('.search-wrap .g-location-trigger');

                function refresh() {
                    validateHomeSearchRequired({ notify: false });
                }

                if (keywordEl && keywordEl.dataset.requiredStateBound !== '1') {
                    keywordEl.dataset.requiredStateBound = '1';
                    keywordEl.addEventListener('input', refresh);
                }
                if (catEl && catEl.dataset.requiredStateBound !== '1') {
                    catEl.dataset.requiredStateBound = '1';
                    catEl.addEventListener('change', refresh);
                }
                if (locEl && locEl.dataset.requiredStateBound !== '1') {
                    locEl.dataset.requiredStateBound = '1';
                    locEl.addEventListener('change', refresh);
                }
                if (clearBtn && clearBtn.dataset.requiredStateBound !== '1') {
                    clearBtn.dataset.requiredStateBound = '1';
                    clearBtn.addEventListener('click', function () {
                        setTimeout(refresh, 0);
                    });
                }
                if (trigger && trigger.dataset.requiredStateBound !== '1') {
                    trigger.dataset.requiredStateBound = '1';
                    trigger.addEventListener('click', function () {
                        setTimeout(refresh, 0);
                    });
                }

                refresh();
            }

            function buildSearchUrl(opts) {
                opts = opts || {};
                var keywordEl = document.getElementById('<%= txt_Search.ClientID %>');
                var catEl = document.getElementById('<%= ddl_Category.ClientID %>');
                var locEl = document.getElementById('<%= ddl_Location.ClientID %>');
                var hfProvince = document.getElementById('<%= hfProvinceCode.ClientID %>');
                var hfDistrict = document.getElementById('<%= hfDistrictCode.ClientID %>');
                var hfWard = document.getElementById('<%= hfWardCode.ClientID %>');

                var keyword = (opts.keyword !== undefined) ? String(opts.keyword || '').trim() : (keywordEl ? keywordEl.value.trim() : '');
                var cat = (opts.cat !== undefined) ? String(opts.cat || '').trim() : (catEl ? catEl.value : '');
                var loc = (opts.loc !== undefined) ? String(opts.loc || '').trim() : (locEl ? locEl.value : '');
                var province = (opts.province !== undefined) ? String(opts.province || '').trim() : (hfProvince ? (hfProvince.value || '').trim() : '');
                var district = (opts.district !== undefined) ? String(opts.district || '').trim() : (hfDistrict ? (hfDistrict.value || '').trim() : '');
                var ward = (opts.ward !== undefined) ? String(opts.ward || '').trim() : (hfWard ? (hfWard.value || '').trim() : '');
                var type = (opts.type !== undefined) ? String(opts.type || '').trim() : 'all';
                var params = new URLSearchParams();
                if (keyword) params.set('q', keyword);
                if (cat) params.set('cat', cat);
                if (loc) params.set('loc', loc);
                if (!province && loc) province = loc;
                if (province) params.set('province', province);
                if (district) params.set('district', district);
                if (ward) params.set('ward', ward);
                params.set('type', type || 'all');
                params.set('page', '1');

                var slug = toSearchSlug(keyword);
                var path = '/tim-kiem' + (slug ? ('/' + slug) : '');
                var qs = params.toString();
                return path + (qs ? ('?' + qs) : '');
            }

            function AhaHomeSearchRedirect() {
                try {
                    var keywordEl = document.getElementById('<%= txt_Search.ClientID %>');
                    var catEl = document.getElementById('<%= ddl_Category.ClientID %>');
                    var locEl = document.getElementById('<%= ddl_Location.ClientID %>');
                    var keyword = keywordEl ? (keywordEl.value || '').trim() : '';
                    var cat = catEl ? (catEl.value || '').trim() : '';
                    var loc = locEl ? (locEl.value || '').trim() : '';
                    var required = validateHomeSearchRequired({ notify: true });
                    if (!required.ok) return false;
                    var type = 'all';
                    if (keyword) {
                        saveRecentSearch({ keyword: keyword, cat: cat, loc: loc, type: type });
                    }
                    trackSearchEvent('search_submit', getSearchTrackingContext({
                        label: keyword || '',
                        cat: cat || '',
                        loc: loc || '',
                        type: type
                    }));

                    var url = buildSearchUrl({ keyword: keyword, cat: cat, loc: loc, type: type });
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
                    var stickyReleaseGap = 8;
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
                    if (document.querySelector('.offcanvas.show, .modal.show')) return;
                    ticking = false;
                    var header = document.querySelector('.site-header');
                    var headerHeight = header ? Math.round(header.getBoundingClientRect().height) : 56;

                    var shortcutBar = document.querySelector('.home-fixed-shortcuts');
                    var layoutMode = (document.documentElement.getAttribute('data-aha-layout-mode') || '').toLowerCase();
                    var isMobile = layoutMode ? (layoutMode === 'mobile') : window.matchMedia('(max-width: 849.98px)').matches;
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
                    var hasPinnedShortcut = document.documentElement.classList.contains('aha-bds-chip-below-topbar');
                    var shortcutVisible = shortcutBar && window.getComputedStyle(shortcutBar).display !== 'none';
                    var shortcutHeight = ((isMobile || hasPinnedShortcut) && shortcutVisible) ? Math.round(shortcutBar.getBoundingClientRect().height) : 0;
                    document.documentElement.style.setProperty('--aha-home-mobile-chip-height', shortcutHeight + 'px');
                    var stickyTop = Math.max(48, headerHeight + shortcutHeight + (isMobile ? 14 : 6));
                    document.documentElement.style.setProperty('--aha-home-sticky-top', stickyTop + 'px');

                    if (wrap.classList.contains('in-header')) {
                        spacer.style.height = '0px';
                        wrap.classList.remove('is-sticky-fixed', 'is-sticky', 'is-root-pinned');
                        wrap.classList.toggle('is-compact', isMobile);
                        setPinnedLayoutClass(false, 0);
                        return;
                    }

                        var isFixed = wrap.classList.contains('is-sticky-fixed');
                        var anchorDocTop = spacer.getBoundingClientRect().top + window.scrollY;
                        var triggerY = anchorDocTop - stickyTop;
                        var currentY = window.scrollY || window.pageYOffset || 0;
                        var shouldStick = isFixed
                            ? currentY >= (triggerY - stickyReleaseGap)
                            : currentY >= triggerY;
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
                    document.addEventListener('shown.bs.offcanvas', queueUpdate);
                    document.addEventListener('hidden.bs.offcanvas', queueUpdate);
                    updateStickyState();

                    window.__ahaHomeSearchStickyRefresh = function () {
                        queueUpdate();
                    };
                });
            }

            document.addEventListener('DOMContentLoaded', function () {
                initChoices();
                restoreStoredLocation();
                updateLocationClearState((document.getElementById('homeLocationLabel') || {}).textContent || '');
                initSearchSuggest();
                bindMobileQuickFilters();
                bindRequiredFieldState();
                ensureHomeSearchSticky();
            });

            if (window.Sys && Sys.WebForms) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    initChoices();
                    restoreStoredLocation();
                    updateLocationClearState((document.getElementById('homeLocationLabel') || {}).textContent || '');
                    initSearchSuggest();
                    bindMobileQuickFilters();
                    bindRequiredFieldState();
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
                                                <a href="<%# Eval("DetailUrl") %>" class="text-decoration-none">
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
                                            <a href="<%# Eval("DetailUrl") %>" class="text-decoration-none">
                                                <div class="sp-title"><%# Eval("name") %></div>
                                            </a>
                                            <div class="sp-desc"><%# Eval("description") %></div>
                                            <asp:Literal ID="litSourceBadge" runat="server"></asp:Literal>
                                            <div class="sp-price"><%# Eval("DisplayPriceText") %></div>

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

                                                    <asp:PlaceHolder ID="ph_kebab" runat="server">
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
                                                    </asp:PlaceHolder>
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
                        <asp:Literal ID="litPager" runat="server"></asp:Literal>
                        <asp:Button ID="but_xemthem" runat="server"
                            CssClass="btn btn-outline-primary d-none"
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
