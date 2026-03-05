#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
WEB_CONFIG="$ROOT_DIR/Web.config"

DB_CONTAINER="${DB_CONTAINER:-ahasale_local_db}"
DB_NAME="${DB_NAME:-ahasale_local}"
DB_USER="${DB_USER:-sa}"
DB_PASS="${DB_PASS:-AhaSaleLocal#2026}"
LIMIT="${LIMIT:-5}"
INTERVAL_SEC="${INTERVAL_SEC:-5}"
LOOP_MODE="false"
TREASURY_ACCOUNT=""

usage() {
  cat <<'EOF'
Usage:
  ./scripts/check-token-bridge-status.sh [options]

Options:
  --account ACCOUNT    Treasury account to inspect (default: read from Web.config USDTBridge.TreasuryAccount)
  --limit N            Number of latest rows for history tables (default: 5)
  --loop               Keep refreshing continuously
  --interval SEC       Refresh interval when --loop is enabled (default: 5)
  -h, --help           Show help

Examples:
  ./scripts/check-token-bridge-status.sh
  ./scripts/check-token-bridge-status.sh --loop --interval 3
  ./scripts/check-token-bridge-status.sh --account vitong_usdt_test_20260303 --limit 10
EOF
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --account)
      TREASURY_ACCOUNT="${2:-}"; shift 2 ;;
    --limit)
      LIMIT="${2:-}"; shift 2 ;;
    --loop)
      LOOP_MODE="true"; shift ;;
    --interval)
      INTERVAL_SEC="${2:-}"; shift 2 ;;
    -h|--help)
      usage; exit 0 ;;
    *)
      echo "Unknown argument: $1" >&2
      usage
      exit 1 ;;
  esac
done

if ! [[ "$LIMIT" =~ ^[0-9]+$ ]] || [[ "$LIMIT" -le 0 ]]; then
  echo "Invalid --limit. Expect a positive integer." >&2
  exit 1
fi

if ! [[ "$INTERVAL_SEC" =~ ^[0-9]+$ ]] || [[ "$INTERVAL_SEC" -le 0 ]]; then
  echo "Invalid --interval. Expect a positive integer." >&2
  exit 1
fi

if [[ -z "$TREASURY_ACCOUNT" ]]; then
  if [[ -f "$WEB_CONFIG" ]]; then
    TREASURY_ACCOUNT="$(sed -n -E 's|.*<add key="USDTBridge.TreasuryAccount" value="([^"]+)".*|\1|p' "$WEB_CONFIG" | head -n 1)"
  fi
fi

if [[ -z "$TREASURY_ACCOUNT" ]]; then
  echo "Cannot detect treasury account. Please pass --account." >&2
  exit 1
fi

if ! command -v docker >/dev/null 2>&1; then
  echo "docker command not found." >&2
  exit 1
fi

sql_escape() {
  local value="$1"
  printf '%s' "$value" | sed "s/'/''/g"
}

run_sql() {
  local query="$1"
  docker exec "$DB_CONTAINER" /opt/mssql-tools18/bin/sqlcmd \
    -C -S localhost -U "$DB_USER" -P "$DB_PASS" -d "$DB_NAME" \
    -Q "$query" -W -s " | " -h -1
}

print_once() {
  local account_escaped
  account_escaped="$(sql_escape "$TREASURY_ACCOUNT")"

  printf '\n[%s] Treasury account: %s\n' "$(date '+%Y-%m-%d %H:%M:%S')" "$TREASURY_ACCOUNT"
  echo "---- Current balance (taikhoan_tb.DongA) ----"
  run_sql "SET NOCOUNT ON; SELECT taikhoan, ISNULL(DongA,0) AS DongA FROM dbo.taikhoan_tb WHERE taikhoan='${account_escaped}';"

  echo "---- Latest bridge rows (USDT_Deposit_Bridge_tb) ----"
  run_sql "SET NOCOUNT ON; SELECT TOP ${LIMIT} id, tx_hash, status, usdt_amount, points_credited, linked_transfer_id, CONVERT(varchar(19),created_at,120) AS created_at, CONVERT(varchar(19),credited_at,120) AS credited_at FROM dbo.USDT_Deposit_Bridge_tb ORDER BY id DESC;"

  echo "---- Latest transfer history of treasury (LichSuChuyenDiem_tb) ----"
  run_sql "SET NOCOUNT ON; SELECT TOP ${LIMIT} id, CONVERT(varchar(19),ngay,120) AS ngay, taikhoan_chuyen, taikhoan_nhan, dongA, trangtrai_rut FROM dbo.LichSuChuyenDiem_tb WHERE taikhoan_chuyen='${account_escaped}' OR taikhoan_nhan='${account_escaped}' ORDER BY id DESC;"
}

if [[ "$LOOP_MODE" == "true" ]]; then
  while true; do
    clear
    print_once
    sleep "$INTERVAL_SEC"
  done
else
  print_once
fi
