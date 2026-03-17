#!/usr/bin/env python3
import os
import sys
from pathlib import Path
import re


DEFAULT_SOURCE = "/Users/duccuongtran/Documents/Aha Shine 2025/ahashine.vn/ahashine.vn/admin"
DEFAULT_TARGET = "/Users/duccuongtran/Documents/Aha Sale 2025/localhost/ahasale.vn.localhost/gianhang/admin"
INTENTIONAL_DIFFS = {
    "Default.aspx": "Standardized the admin home into a KiotViet-style operating cockpit with fast actions and cross-module work queues.",
    "Default.aspx.cs": "Added cockpit KPIs, receivable queue, and payable overview to turn admin home into a day-to-day business control center.",
    "mp-admin.master": "Kept aligned with AhaSale admin asset stack and route namespace.",
    "mp-admin.master.cs": "Added resilient AhaShine context bootstrap and retry handling for local/runtime stability.",
    "login.aspx.cs": "Added branch/bootstrap-aware admin login flow so GianHang admin can self-initialize on AhaSale.",
    "uc/menu_top.ascx": "Kept aligned with AhaSale admin topbar, dropdown behavior, and quick-access testing links.",
    "uc/menu_left.ascx": "Extended the admin navigation with storefront configuration entry points so /gianhang is fully driven from admin.",
    "uc/menu_left.ascx.cs": "Added storefront configuration route highlighting in the admin navigation state machine.",
    "quan-ly-khach-hang/chi-tiet.aspx": "Added direct CRM quick actions to create appointment, invoice, and service card from customer profile.",
    "quan-ly-khach-hang/chi-tiet.aspx.cs": "Added quick-action URL composition so customer profile can drive the new admin workflow.",
    "quan-ly-khach-hang/danh-sach-lich-hen.aspx.cs": "Added query-string prefill support for quick-create appointment flow.",
    "quan-ly-hoa-don/chi-tiet.aspx.cs": "Extended invoice detail to auto-sync real payments into thu-chi and support operational finance flow.",
    "quan-ly-hoa-don/chi-tiet.aspx": "Enabled invoice detail to open directly in payment mode from the invoice work queue.",
    "quan-ly-hoa-don/lich-su-thanh-toan.aspx.cs": "Extended invoice payment history to keep thu-chi in sync when deleting payments.",
    "quan-ly-hoa-don/Default.aspx": "Standardized the invoice list into a KiotViet-style work queue with quick filters and priority collection actions.",
    "quan-ly-hoa-don/Default.aspx.cs": "Added invoice work-queue KPIs, quick-filter support, and prioritized collection lists for day-to-day operations.",
    "quan-ly-hoc-vien/edit.aspx.cs": "Extended student payments to auto-sync into thu-chi for a unified business ledger.",
    "quan-ly-kho-hang/chi-tiet-nhap-hang.aspx.cs": "Extended warehouse import payments to auto-sync chi records into thu-chi.",
    "quan-ly-kho-hang/nhap-hang.aspx": "Adjusted import UI postback behavior for stable runtime execution on AhaSale local/Mono.",
    "quan-ly-the-dich-vu/chi-tiet.aspx.cs": "Extended service-card payments to auto-sync into thu-chi for unified finance management.",
    "quan-ly-thu-chi/Default.aspx": "Added source-aware finance UI so auto-generated receipts and expenses are distinguishable in thu-chi.",
    "quan-ly-thu-chi/Default.aspx.cs": "Added source-aware finance rendering for invoice, card, student, warehouse, and supply auto-sync records.",
    "quan-ly-thu-chi/edit.aspx.cs": "Protected auto-generated finance records from unsafe manual edits and redirects back to source documents.",
    "quan-ly-vat-tu/chi-tiet-nhap-hang.aspx.cs": "Extended supply import payments to auto-sync chi records into thu-chi.",
    "quan-ly-vat-tu/nhap-vat-tu.aspx": "Adjusted supply import UI postback behavior for stable runtime execution on AhaSale local/Mono.",
    "quan-ly-vat-tu/nhap-vat-tu.aspx.cs": "Added supply schema bootstrap and runtime-safe handling for vật tư import flow.",
    "quen-mat-khau/default.aspx.cs": "Added auth/bootstrap fallback so forgot-password flow degrades safely when local DB or context is cold.",
    "quen-mat-khau/nhap-ma-khoi-phuc.aspx.cs": "Added auth/bootstrap fallback so recovery-code flow degrades safely when local DB or context is cold.",
    "quen-mat-khau/dat-lai-mat-khau.aspx.cs": "Added auth/bootstrap fallback so password reset flow degrades safely when local DB or context is cold.",
}


NORMALIZE_REPLACEMENTS = [
    ("/gianhang/admin", "/admin"),
    ("/gianhang/webcon", "/webcon"),
    ("AhaShineContext_cl.ResolveChiNhanhId()", 'Session["chinhanh"].ToString()'),
    ("AhaShineContext_cl.ResolveCurrentChiNhanhId()", 'Session["chinhanh"].ToString()'),
    ("System.Web.HttpContext.Current.AhaShineContext_cl.ResolveChiNhanhId()", 'Session["chinhanh"].ToString()'),
    ("AhaShineContext_cl.UserParent", '"admin"'),
]


def normalize_target(text):
    normalized = text
    for current, expected in NORMALIZE_REPLACEMENTS:
        normalized = normalized.replace(current, expected)
    return normalized


LINE_REMOVALS = [
    r'^\s*AhaShineContext_cl\.EnsureSchemaAndDefaults\(\);\s*$',
    r'^\s*AhaShineContext_cl\.EnsureContext\(\);\s*$',
    r'^\s*AhaShineHomeSync_cl\.SyncPost\(db,\s*_ob\);\s*$',
    r'^\s*_ob\.nguoitao\s*=\s*user_parent;\s*$',
    r'^\s*var _ob = tk_cl\.return_object\(_user\);\s*$',
    r'^\s*if \(Session\["user"\] == null\) Session\["user"\] = "";\s*$',
    r'^\s*if \(Session\["notifi"\] == null\) Session\["notifi"\] = "";\s*$',
    r'^\s*Session\["user_parent"\]\s*=\s*string\.IsNullOrWhiteSpace\(_ob\.user_parent\)\s*\?\s*"admin"\s*:\s*_ob\.user_parent;\s*$',
    r'^\s*Session\["user_parent"\]\s*=\s*"admin";\s*$',
]

LINE_REPLACEMENTS = [
    (r'System\.Web\.HttpContext\.Current\.Session\["chinhanh"\]\.ToString\(\)', 'Session["chinhanh"].ToString()'),
    (r'Session\["user_parent"\]\s*=\s*string\.IsNullOrWhiteSpace\(_ob\.user_parent\)\s*\?\s*"admin"\s*:\s*_ob\.user_parent;', 'Session["user_parent"] = "admin";'),
    (r'Session\["nganh"\]\s*=\s*_ob\.id_nganh;', 'Session["nganh"] = tk_cl.return_object(_user).id_nganh;'),
]


def reduce_logic_text(text):
    lines = text.splitlines()
    kept = []
    for line in lines:
        skip = False
        for pattern in LINE_REMOVALS:
            if re.match(pattern, line):
                skip = True
                break
        if skip:
            continue
        for pattern, replacement in LINE_REPLACEMENTS:
            line = re.sub(pattern, replacement, line)
        kept.append(line)
    reduced = "\n".join(kept)
    return re.sub(r"\s+", "", reduced)


def main():
    source_root = Path(os.environ.get("AHASHINE_SOURCE_ADMIN", DEFAULT_SOURCE))
    target_root = Path(os.environ.get("AHASHINE_TARGET_ADMIN", DEFAULT_TARGET))

    if not source_root.exists():
        print(f"Missing source admin path: {source_root}", file=sys.stderr)
        return 2
    if not target_root.exists():
        print(f"Missing target admin path: {target_root}", file=sys.stderr)
        return 2

    files = sorted(
        p.relative_to(source_root)
        for p in source_root.rglob("*")
        if p.is_file()
    )

    exact = 0
    normalized = 0
    substantive = []
    missing = []
    intentional = []

    for rel in files:
        source_file = source_root / rel
        target_file = target_root / rel
        if not target_file.exists():
            missing.append(str(rel))
            continue

        source_text = source_file.read_text(encoding="utf-8", errors="ignore")
        target_text = target_file.read_text(encoding="utf-8", errors="ignore")

        if source_text == target_text:
            exact += 1
            continue

        normalized_target = normalize_target(target_text)
        if normalized_target == source_text:
            normalized += 1
            continue

        if source_file.suffix == ".cs" and reduce_logic_text(normalized_target) == reduce_logic_text(source_text):
            normalized += 1
            continue

        if str(rel) in INTENTIONAL_DIFFS:
            intentional.append(str(rel))
            continue

        substantive.append(str(rel))

    print(f"Source admin : {source_root}")
    print(f"Target admin : {target_root}")
    print(f"Total files  : {len(files)}")
    print(f"Exact match  : {exact}")
    print(f"Normalized   : {normalized}")
    print(f"Intentional  : {len(intentional)}")
    print(f"Missing      : {len(missing)}")
    print(f"Substantive  : {len(substantive)}")

    if missing:
        print("\nMissing files:")
        for rel in missing[:40]:
            print(f"  - {rel}")

    if intentional:
        print("\nIntentional diffs:")
        for rel in intentional[:20]:
            print(f"  - {rel}: {INTENTIONAL_DIFFS[rel]}")

    if substantive:
        print("\nSubstantive diffs:")
        for rel in substantive[:40]:
            print(f"  - {rel}")
        return 1

    print("\nAdmin parity check passed.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
