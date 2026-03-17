#!/usr/bin/env bash
set -euo pipefail

BASE_URL="${BASE_URL:-https://ahasale.local}"

paths=(
  "/gianhang/admin/login.aspx"
  "/gianhang/admin/Default.aspx"
  "/gianhang/admin/cai-dat-chung/default.aspx"
  "/gianhang/admin/cau-hinh-chung/cap-nhat-thong-tin.aspx"
  "/gianhang/admin/cau-hinh-storefront/default.aspx"
  "/gianhang/admin/cau-hinh-storefront/edit-section.aspx?id=1"
  "/gianhang/admin/cau-hinh-chung/cai-dat-bao-tri.aspx"
  "/gianhang/admin/quan-ly-bai-viet/Default.aspx"
  "/gianhang/admin/quan-ly-bai-viet/add.aspx"
  "/gianhang/admin/quan-ly-bai-viet/in/a4.aspx"
  "/gianhang/admin/quan-ly-hoa-don/Default.aspx"
  "/gianhang/admin/quan-ly-hoa-don/in-a5.aspx?id=1"
  "/gianhang/admin/quan-ly-hoa-don/in-bill.aspx?id=1"
  "/gianhang/admin/quan-ly-khach-hang/Default.aspx"
  "/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx"
  "/gianhang/admin/quan-ly-kho-hang/Default.aspx"
  "/gianhang/admin/quan-ly-kho-hang/nhap-hang.aspx"
  "/gianhang/admin/quan-ly-kho-hang/xoa_chitiet_giohang.aspx?id=1"
  "/gianhang/admin/quan-ly-menu/Default.aspx"
  "/gianhang/admin/quan-ly-menu/in/a4.aspx"
  "/gianhang/admin/quan-ly-menu/in/bill.aspx"
  "/gianhang/admin/quan-ly-tai-khoan/Default.aspx"
  "/gianhang/admin/quan-ly-tai-khoan/add.aspx"
  "/gianhang/admin/quan-ly-tai-khoan/phan-quyen.aspx?user=admin"
  "/gianhang/admin/quan-ly-the-dich-vu/Default.aspx"
  "/gianhang/admin/quan-ly-thu-chi/Default.aspx"
  "/gianhang/admin/quan-ly-thu-chi/inthuchi.aspx?id=1"
  "/gianhang/admin/quan-ly-thu-chi/xoa-anh.aspx?id=1"
  "/gianhang/admin/quan-ly-vat-tu/Default.aspx"
  "/gianhang/admin/quan-ly-vat-tu/add.aspx"
  "/gianhang/admin/quan-ly-vat-tu/xoa_chitiet_giohang.aspx?id=1"
  "/gianhang/admin/quan-ly-hoc-vien/Default.aspx"
  "/gianhang/admin/quan-ly-giang-vien/Default.aspx"
  "/gianhang/admin/quan-ly-he-thong/nganh.aspx"
  "/gianhang/admin/quan-ly-thong-bao/Default.aspx"
  "/gianhang/admin/yeu-cau-tu-van/default.aspx"
)

HOST="$(printf '%s' "$BASE_URL" | sed -E 's#^https?://([^/:]+).*#\1#')"
PORT="$(printf '%s' "$BASE_URL" | sed -nE 's#^https?://[^/:]+:([0-9]+).*#\1#p')"
if [[ -z "$PORT" ]]; then
  if [[ "$BASE_URL" == https://* ]]; then
    PORT="443"
  else
    PORT="80"
  fi
fi

RESOLVE_FLAGS=()
if [[ "$HOST" == "ahasale.local" ]]; then
  RESOLVE_FLAGS=(--resolve "${HOST}:${PORT}:127.0.0.1")
fi

printf "Testing AhaShine admin endpoints at %s\n" "$BASE_URL"

for p in "${paths[@]}"; do
  code=""
  for attempt in 1 2 3; do
    code=$(curl -k -s -o /dev/null -w "%{http_code}" --max-time 30 ${RESOLVE_FLAGS[@]+"${RESOLVE_FLAGS[@]}"} "$BASE_URL$p" || true)
    if [[ "$code" == "200" || "$code" == "302" ]]; then
      break
    fi
    sleep 2
  done
  if [[ "$code" != "200" && "$code" != "302" ]]; then
    printf "FAIL %s -> %s\n" "$p" "$code"
    exit 1
  fi
  printf "OK   %s -> %s\n" "$p" "$code"
done

echo "Admin smoke test passed."
