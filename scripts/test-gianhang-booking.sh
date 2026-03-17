#!/usr/bin/env bash
set -euo pipefail

BASE_URL="${BASE_URL:-https://ahasale.local}"
PATHS=(
  "/home/dat-lich.aspx?user=admin"
  "/shop/quan-ly-lich-hen.aspx"
  "/shop/nang-cap-level2.aspx"
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

printf "Checking booking page at %s\n" "$BASE_URL"
for p in "${PATHS[@]}"; do
  code=$(curl -k -s -o /dev/null -w "%{http_code}" --max-time 30 ${RESOLVE_FLAGS[@]+"${RESOLVE_FLAGS[@]}"} "$BASE_URL$p" || true)
  if [[ "$code" != "200" && "$code" != "302" ]]; then
    printf "FAIL %s -> %s\n" "$p" "$code"
    exit 1
  fi
  printf "OK   %s -> %s\n" "$p" "$code"
done

echo "Booking page smoke test passed."
