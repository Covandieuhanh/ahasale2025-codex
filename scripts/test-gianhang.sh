#!/usr/bin/env bash
set -euo pipefail

BASE_URL="${BASE_URL:-https://ahasale.local}"

paths=(
  "/gianhang/Default.aspx"
  "/gianhang/datlich.aspx"
  "/gianhang/giohang.aspx"
  "/gianhang/hoa-don-dien-tu.aspx"
  "/gianhang/page/danh-sach-san-pham.aspx"
  "/gianhang/page/danh-sach-dich-vu.aspx"
  "/gianhang/admin/Default.aspx"
  "/gianhang/admin/login.aspx"
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

printf "Testing GianHang endpoints at %s\n" "$BASE_URL"

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

echo "GianHang smoke tests passed."
