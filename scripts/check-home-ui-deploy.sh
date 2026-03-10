#!/usr/bin/env bash
set -euo pipefail

HOST="https://ahasale.vn"
CURL_OPTS=(-fsSL)

while [[ $# -gt 0 ]]; do
  case "$1" in
    --host)
      HOST="${2:-}"
      shift 2
      ;;
    --insecure)
      CURL_OPTS+=(-k)
      shift
      ;;
    *)
      echo "Unknown argument: $1"
      echo "Usage: ./scripts/check-home-ui-deploy.sh [--host https://ahasale.vn] [--insecure]"
      exit 1
      ;;
  esac
done

if [[ -z "$HOST" ]]; then
  echo "Usage: ./scripts/check-home-ui-deploy.sh --host https://ahasale.vn"
  exit 1
fi

TMP_HTML="$(mktemp)"
trap 'rm -f "$TMP_HTML"' EXIT

echo "[1/3] Fetch homepage: $HOST/"
if ! curl "${CURL_OPTS[@]}" "$HOST/" -o "$TMP_HTML"; then
  echo "FAIL: cannot fetch homepage"
  exit 2
fi

declare -a REQUIRED_PATTERNS=(
  "/assetscss/Tabler-Hota.css"
  "/assetscss/header_tabler_UI.css"
  "/assetscss/aha-ui-refresh.css"
)

echo "[2/3] Validate CSS references in homepage HTML"
for p in "${REQUIRED_PATTERNS[@]}"; do
  if rg -q "$p" "$TMP_HTML"; then
    echo "OK  : found $p"
  else
    echo "FAIL: missing $p"
    exit 3
  fi
done

# Extract first concrete URLs (with version query) from homepage.
TABLER_CSS="$(rg -o '/assetscss/Tabler-Hota\.css[^" ]*' "$TMP_HTML" | head -n1 || true)"
HEADER_CSS="$(rg -o '/assetscss/header_tabler_UI\.css[^" ]*' "$TMP_HTML" | head -n1 || true)"
REFRESH_CSS="$(rg -o '/assetscss/aha-ui-refresh\.css[^" ]*' "$TMP_HTML" | head -n1 || true)"
REFRESH_JS="$(rg -o '/js/aha-ui-refresh\.js[^" ]*' "$TMP_HTML" | head -n1 || true)"

declare -a URLS=("$TABLER_CSS" "$HEADER_CSS" "$REFRESH_CSS" "$REFRESH_JS")

echo "[3/3] Probe asset HTTP status"
for path in "${URLS[@]}"; do
  if [[ -z "$path" ]]; then
    echo "FAIL: missing one concrete asset URL in HTML"
    exit 4
  fi

  code="$(curl -s "${CURL_OPTS[@]}" -o /dev/null -w '%{http_code}' "$HOST$path")"
  if [[ "$code" == "200" ]]; then
    echo "OK  : $code $HOST$path"
  else
    echo "FAIL: $code $HOST$path"
    exit 5
  fi
done

echo "SUCCESS: Homepage UI asset check passed."
