#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DOMAIN="ahasale.vn"
SERVER_IP="112.78.2.206"
WATCHER_IP="CHANGE_ME_WATCHER_PUBLIC_IP"
OUT_DIR=""
SOURCE_WEB_CONFIG="$ROOT_DIR/Web.config"

usage() {
  cat <<'EOF'
Usage:
  ./scripts/prepare-hosting-package.sh \
    [--domain ahasale.vn] \
    [--server-ip 112.78.2.206] \
    [--watcher-ip 1.2.3.4] \
    [--out-dir /path/to/output]

Output:
  releases/ahasale-hosting-YYYYmmdd_HHMMSS/
    - site/                         (upload folder via FTP)
    - usdt_bridge.hosting.env       (env for watcher machine)
    - DEPLOY_HOSTING_AHASALE_VN.md  (deploy checklist)
    - docs/host-compat/             (rulebook + deploy checklist)
    - ahasale-hosting-*.zip|tar.gz  (archive)
EOF
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --domain)
      DOMAIN="${2:-}"; shift 2 ;;
    --server-ip)
      SERVER_IP="${2:-}"; shift 2 ;;
    --watcher-ip)
      WATCHER_IP="${2:-}"; shift 2 ;;
    --out-dir)
      OUT_DIR="${2:-}"; shift 2 ;;
    -h|--help)
      usage; exit 0 ;;
    *)
      echo "Unknown argument: $1" >&2
      usage
      exit 1 ;;
  esac
done

if [[ -z "$OUT_DIR" ]]; then
  TS="$(date +%Y%m%d_%H%M%S)"
  OUT_DIR="$ROOT_DIR/releases/ahasale-hosting-$TS"
fi

SITE_DIR="$OUT_DIR/site"
TEMPLATE_WATCHER_ENV="$ROOT_DIR/deploy/usdt_bridge.hosting.ahasale.vn.env.example"
TEMPLATE_GUIDE="$ROOT_DIR/deploy/DEPLOY_HOSTING_AHASALE_VN.md"
HOST_COMPAT_DOCS_SRC="$ROOT_DIR/docs/host-compat"
HOST_COMPAT_DOCS_OUT="$OUT_DIR/docs/host-compat"

if [[ ! -f "$SOURCE_WEB_CONFIG" ]]; then
  echo "Missing source Web.config: $SOURCE_WEB_CONFIG" >&2
  exit 1
fi

get_app_setting_value() {
  local key="$1"
  sed -n -E "s|.*<add[[:space:]]+key=\"$key\"[[:space:]]+value=\"([^\"]*)\"[[:space:]]*/>.*|\\1|p" "$SOURCE_WEB_CONFIG" | head -n 1
}

escape_sed_replacement() {
  printf '%s' "$1" | sed -e 's/[&|\\]/\\&/g'
}

DEPOSIT_ADDRESS="$(get_app_setting_value "USDTBridge.DepositAddress")"
TOKEN_CONTRACT="$(get_app_setting_value "USDTBridge.TokenContract")"
TREASURY_ACCOUNT="$(get_app_setting_value "USDTBridge.TreasuryAccount")"
POINT_RATE="$(get_app_setting_value "USDTBridge.PointRatePerToken")"

[[ -n "$DEPOSIT_ADDRESS" ]] || DEPOSIT_ADDRESS="CHANGE_ME_BSC_DEPOSIT_ADDRESS"
[[ -n "$TOKEN_CONTRACT" ]] || TOKEN_CONTRACT="CHANGE_ME_BSC_TOKEN_CONTRACT"
[[ -n "$TREASURY_ACCOUNT" ]] || TREASURY_ACCOUNT="CHANGE_ME_TREASURY_ACCOUNT"
[[ -n "$POINT_RATE" ]] || POINT_RATE="1"

DOMAIN_ESCAPED="$(escape_sed_replacement "$DOMAIN")"
SERVER_IP_ESCAPED="$(escape_sed_replacement "$SERVER_IP")"
WATCHER_IP_ESCAPED="$(escape_sed_replacement "$WATCHER_IP")"
DEPOSIT_ADDRESS_ESCAPED="$(escape_sed_replacement "$DEPOSIT_ADDRESS")"
TOKEN_CONTRACT_ESCAPED="$(escape_sed_replacement "$TOKEN_CONTRACT")"
TREASURY_ACCOUNT_ESCAPED="$(escape_sed_replacement "$TREASURY_ACCOUNT")"
POINT_RATE_ESCAPED="$(escape_sed_replacement "$POINT_RATE")"

mkdir -p "$SITE_DIR"

rsync -a \
  --exclude '.git/' \
  --exclude '.DS_Store' \
  --exclude 'docker/' \
  --exclude 'docker-compose.yml' \
  --exclude 'README.localhost.md' \
  --exclude 'releases/' \
  --exclude 'deploy/' \
  --exclude 'uploads/' \
  --exclude 'App_Data/' \
  --exclude 'Bin/roslyn/' \
  --exclude 'Bin/*.exe' \
  --exclude 'scripts/start-localhost.sh' \
  --exclude 'scripts/start-localhost.cmd' \
  --exclude 'scripts/start-localhost.ps1' \
  --exclude 'scripts/stop-localhost.sh' \
  --exclude 'scripts/usdt_bridge.env' \
  --exclude 'scripts/usdt_bridge.env.save' \
  --exclude 'scripts/usdt_bridge_state.json' \
  --exclude 'account-login-data-*.csv' \
  --exclude 'Bin/Debug/Publish/' \
  "$ROOT_DIR/" "$SITE_DIR/"

cp "$SOURCE_WEB_CONFIG" "$SITE_DIR/Web.config"

render_template() {
  local input="$1"
  local output="$2"
  sed \
    -e "s|__SITE_DOMAIN__|$DOMAIN_ESCAPED|g" \
    -e "s|__SERVER_IP__|$SERVER_IP_ESCAPED|g" \
    -e "s|__WATCHER_IP__|$WATCHER_IP_ESCAPED|g" \
    -e "s|__DEPOSIT_ADDRESS__|$DEPOSIT_ADDRESS_ESCAPED|g" \
    -e "s|__TOKEN_CONTRACT__|$TOKEN_CONTRACT_ESCAPED|g" \
    -e "s|__TREASURY_ACCOUNT__|$TREASURY_ACCOUNT_ESCAPED|g" \
    -e "s|__POINT_RATE__|$POINT_RATE_ESCAPED|g" \
    "$input" > "$output"
}

if [[ -f "$TEMPLATE_WATCHER_ENV" ]]; then
  render_template "$TEMPLATE_WATCHER_ENV" "$OUT_DIR/usdt_bridge.hosting.env"
fi

if [[ -f "$TEMPLATE_GUIDE" ]]; then
  render_template "$TEMPLATE_GUIDE" "$OUT_DIR/DEPLOY_HOSTING_AHASALE_VN.md"
fi

if [[ -d "$HOST_COMPAT_DOCS_SRC" ]]; then
  mkdir -p "$HOST_COMPAT_DOCS_OUT"
  if [[ -f "$HOST_COMPAT_DOCS_SRC/RULEBOOK.md" ]]; then
    cp "$HOST_COMPAT_DOCS_SRC/RULEBOOK.md" "$HOST_COMPAT_DOCS_OUT/RULEBOOK.md"
  fi
  if [[ -f "$HOST_COMPAT_DOCS_SRC/DEPLOY_CHECKLIST_AHASALE.md" ]]; then
    cp "$HOST_COMPAT_DOCS_SRC/DEPLOY_CHECKLIST_AHASALE.md" "$HOST_COMPAT_DOCS_OUT/DEPLOY_CHECKLIST_AHASALE.md"
  fi
fi

ARCHIVE_BASENAME="ahasale-hosting-$(date +%Y%m%d_%H%M%S)"
if command -v zip >/dev/null 2>&1; then
  (
    cd "$OUT_DIR"
    files=("site")
    [[ -f "usdt_bridge.hosting.env" ]] && files+=("usdt_bridge.hosting.env")
    [[ -f "DEPLOY_HOSTING_AHASALE_VN.md" ]] && files+=("DEPLOY_HOSTING_AHASALE_VN.md")
    [[ -f "docs/host-compat/RULEBOOK.md" ]] && files+=("docs/host-compat/RULEBOOK.md")
    [[ -f "docs/host-compat/DEPLOY_CHECKLIST_AHASALE.md" ]] && files+=("docs/host-compat/DEPLOY_CHECKLIST_AHASALE.md")
    zip -rq "${ARCHIVE_BASENAME}.zip" "${files[@]}"
  )
  ARCHIVE_PATH="$OUT_DIR/${ARCHIVE_BASENAME}.zip"
else
  files=("site")
  [[ -f "$OUT_DIR/usdt_bridge.hosting.env" ]] && files+=("usdt_bridge.hosting.env")
  [[ -f "$OUT_DIR/DEPLOY_HOSTING_AHASALE_VN.md" ]] && files+=("DEPLOY_HOSTING_AHASALE_VN.md")
  [[ -f "$OUT_DIR/docs/host-compat/RULEBOOK.md" ]] && files+=("docs/host-compat/RULEBOOK.md")
  [[ -f "$OUT_DIR/docs/host-compat/DEPLOY_CHECKLIST_AHASALE.md" ]] && files+=("docs/host-compat/DEPLOY_CHECKLIST_AHASALE.md")
  tar -czf "$OUT_DIR/${ARCHIVE_BASENAME}.tar.gz" -C "$OUT_DIR" "${files[@]}"
  ARCHIVE_PATH="$OUT_DIR/${ARCHIVE_BASENAME}.tar.gz"
fi

echo "Hosting package is ready:"
echo " - Site folder: $SITE_DIR"
if [[ -f "$OUT_DIR/usdt_bridge.hosting.env" ]]; then
  echo " - Watcher env: $OUT_DIR/usdt_bridge.hosting.env"
fi
if [[ -f "$OUT_DIR/DEPLOY_HOSTING_AHASALE_VN.md" ]]; then
  echo " - Checklist : $OUT_DIR/DEPLOY_HOSTING_AHASALE_VN.md"
fi
if [[ -f "$OUT_DIR/docs/host-compat/RULEBOOK.md" ]]; then
  echo " - Rulebook  : $OUT_DIR/docs/host-compat/RULEBOOK.md"
fi
if [[ -f "$OUT_DIR/docs/host-compat/DEPLOY_CHECKLIST_AHASALE.md" ]]; then
  echo " - Checklist+: $OUT_DIR/docs/host-compat/DEPLOY_CHECKLIST_AHASALE.md"
fi
echo " - Archive   : $ARCHIVE_PATH"
