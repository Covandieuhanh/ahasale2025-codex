#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
BASE_URL="${BASE_URL:-https://ahasale.local}"

printf "Testing AhaShine schema readiness...\n"

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

response="$(curl -k -sS --max-time 60 ${RESOLVE_FLAGS[@]+"${RESOLVE_FLAGS[@]}"} "$BASE_URL/debug/test-gianhang-sync.aspx")"

if [[ "$response" != *"ok=1"* ]]; then
  printf "Schema test failed.\n%s\n" "$response" >&2
  exit 1
fi

printf "Schema test passed.\n"
