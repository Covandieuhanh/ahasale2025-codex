#!/usr/bin/env bash
set -euo pipefail

BASE_URL="${BASE_URL:-https://ahasale.local}"
CURL_BIN="${CURL_BIN:-/usr/bin/curl}"

paths=(
  "/shop/nang-cap-level2.aspx"
  "/admin/duyet-nang-cap-level2.aspx"
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
CURL_EXTRA=()
TARGET_BASE="$BASE_URL"
if [[ "$HOST" == "ahasale.local" ]]; then
  TARGET_BASE="https://127.0.0.1"
  CURL_EXTRA=(-H "Host: ${HOST}")
fi

printf "Testing Level 2 request endpoints at %s\n" "$BASE_URL"

for p in "${paths[@]}"; do
  code=""
  for attempt in 1 2 3; do
    code=$("$CURL_BIN" -k -s -o /dev/null -w "%{http_code}" --connect-timeout 5 --max-time 30 --retry 2 --retry-connrefused ${RESOLVE_FLAGS[@]+"${RESOLVE_FLAGS[@]}"} ${CURL_EXTRA[@]+"${CURL_EXTRA[@]}"} "$TARGET_BASE$p" || true)
    if [[ "$code" == "000" && "$HOST" == "ahasale.local" ]]; then
      code=$("$CURL_BIN" -s -o /dev/null -w "%{http_code}" --connect-timeout 5 --max-time 30 --retry 2 --retry-connrefused -H "Host: ${HOST}" "http://127.0.0.1${p}" || true)
    fi
    if [[ "$code" == "200" || "$code" == "301" || "$code" == "302" ]]; then
      break
    fi
    sleep 2
  done
  if [[ "$code" != "200" && "$code" != "301" && "$code" != "302" ]]; then
    printf "FAIL %s -> %s\n" "$p" "$code"
    exit 1
  fi
  printf "OK   %s -> %s\n" "$p" "$code"
done

echo "Level 2 request smoke test passed."
