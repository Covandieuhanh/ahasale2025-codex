#!/usr/bin/env bash
set -euo pipefail

BASE_URL=${BASE_URL:-https://ahasale.local}
CURL_OPTS=(-k -sS -L --max-time 25)

REPORT=${REPORT:-/tmp/ahasale_home_smoke_report.txt}
: > "$REPORT"

fail_count=0
pass_count=0

function log() {
  printf "%s\n" "$*" | tee -a "$REPORT" >/dev/null
}

function check_page() {
  local path="$1"
  local expect1="${2:-}"
  local expect2="${3:-}"
  local url="${BASE_URL}${path}"
  local tmp
  tmp=$(mktemp)

  local code
  code=$(curl "${CURL_OPTS[@]}" -o "$tmp" -w "%{http_code}" "$url" || true)
  if [[ "$code" != "200" ]]; then
    log "FAIL: $path (HTTP $code)"
    fail_count=$((fail_count+1))
    rm -f "$tmp"
    return
  fi

  if rg -n "Application Exception|Server Error in|Compilation Error" "$tmp" >/dev/null 2>&1; then
    log "FAIL: $path (error text detected)"
    fail_count=$((fail_count+1))
    rm -f "$tmp"
    return
  fi

  if [[ -n "$expect1" ]]; then
    if ! rg -n "$expect1" "$tmp" >/dev/null 2>&1; then
      if [[ -n "$expect2" ]] && rg -n "$expect2" "$tmp" >/dev/null 2>&1; then
        :
      else
        log "WARN: $path (missing expected marker: $expect1)"
      fi
    fi
  fi

  log "OK  : $path"
  pass_count=$((pass_count+1))
  rm -f "$tmp"
}

log "BASE_URL=$BASE_URL"

check_page "/" "AhaSale" "Tìm sản phẩm"
check_page "/home/Default.aspx" "Tìm sản phẩm" "AhaSale"
check_page "/home/login.aspx" "Đăng nhập" "Login"
check_page "/home/dangky.aspx" "Đăng ký" "Đăng kí"
check_page "/home/tim-kiem.aspx?q=kem" "Tìm kiếm" "Kết quả"
check_page "/home/dat-lich.aspx" "Đặt lịch" "booking"
check_page "/home/lich-hen.aspx" "Lịch hẹn" "đăng nhập"
check_page "/home/gio-hang.aspx" "Giỏ hàng" "Đăng nhập"
check_page "/home/don-mua.aspx" "Đơn mua" "Đăng nhập"
check_page "/home/tao-yeu-cau.aspx" "Yêu cầu" "Đăng nhập"

log "\nSUMMARY: pass=$pass_count fail=$fail_count"
if [[ $fail_count -gt 0 ]]; then
  exit 1
fi
