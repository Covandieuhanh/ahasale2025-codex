#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"
source "$ROOT_DIR/scripts/lib/local-db-env.sh"
aha_export_local_db_env

EXIT_CODE=0
SEARCH_PATHS=(App_Code admin home scripts/sql)
USE_RG=0

if command -v rg >/dev/null 2>&1 && [ "${AHA_FORCE_GREP:-0}" != "1" ]; then
  USE_RG=1
fi

ok() {
  echo "[OK] $1"
}

warn() {
  echo "[WARN] $1"
}

fail() {
  echo "[FAIL] $1"
  EXIT_CODE=1
}

scan_regex() {
  local pattern="$1"
  if [ "$USE_RG" -eq 1 ]; then
    rg -n -S -- "$pattern" "${SEARCH_PATHS[@]}" --glob '!**/Bin/**'
    return $?
  fi

  local found=1
  while IFS= read -r -d '' file; do
    if grep -nE -- "$pattern" "$file"; then
      found=0
    fi
  done < <(find "${SEARCH_PATHS[@]}" -type d -name Bin -prune -o -type f -print0)
  return "$found"
}

token_exists() {
  local token="$1"
  if [ "$USE_RG" -eq 1 ]; then
    rg -q -S -- "$token" "${SEARCH_PATHS[@]}" --glob '!**/Bin/**'
    return $?
  fi

  while IFS= read -r -d '' file; do
    if grep -qF -- "$token" "$file"; then
      return 0
    fi
  done < <(find "${SEARCH_PATHS[@]}" -type d -name Bin -prune -o -type f -print0)
  return 1
}

echo "== HanhVi Sync Guard =="
echo "Workspace: $ROOT_DIR"
echo
if [ "$USE_RG" -eq 1 ]; then
  ok "Using rg for source scan."
else
  warn "rg not found (or forced off). Using grep fallback for source scan."
fi
echo

echo "1) Check legacy naming in active source..."
LEGACY_REGEX='KyHieu9ViCon_1_9|(^|[^A-Za-z0-9_])LoaiVi([^A-Za-z0-9_]|$)|HoSoCon9Cap_cl|GetTenHoSoConTheoLoai|GetLoaiHoSoConByCapGiaTri|GetLoaiHoSoTongTheoHoSoCon|(^|[^A-Za-z0-9_])(hosocon|vicon|ViCon|HoSoCon|TenHoSoCon|LoaiHoSoCon)([^A-Za-z0-9_]|$)'
if scan_regex "$LEGACY_REGEX" > /tmp/ahasale_hanhvi_guard_legacy.log; then
  fail "Found legacy naming tokens. First 80 matches:"
  sed -n '1,80p' /tmp/ahasale_hanhvi_guard_legacy.log
else
  ok "No legacy naming tokens found in App_Code/admin/home/scripts/sql."
fi
echo

echo "2) Check required hanhvi files/tokens..."
REQUIRED_FILES=(
  "App_Code/HanhVi9Cap_cl.cs"
  "scripts/sql/ensure-hanhvi-schema.sql"
)
for f in "${REQUIRED_FILES[@]}"; do
  if [ -f "$f" ]; then
    ok "File exists: $f"
  else
    fail "Missing required file: $f"
  fi
done

REQUIRED_TOKENS=(
  "KyHieu9HanhVi_1_9"
  "LoaiHanhVi"
  "GetTenHanhViTheoLoai"
  "GetLoaiHanhViByCapGiaTri"
)
for token in "${REQUIRED_TOKENS[@]}"; do
  if token_exists "$token"; then
    ok "Token present: $token"
  else
    fail "Missing required token: $token"
  fi
done
echo

echo "3) Check database schema (if local DB container is running)..."
if ! command -v docker >/dev/null 2>&1; then
  warn "Docker not found. Skipped DB schema guard."
elif ! docker ps --format '{{.Names}}' | grep -qx "$DB_CONTAINER"; then
  warn "Container $DB_CONTAINER is not running. Skipped DB schema guard."
else
  if docker exec -i "$DB_CONTAINER" /opt/mssql-tools18/bin/sqlcmd \
      -C -S localhost -U sa -P 'AhaSaleLocal#2026' -l "$DB_LOGIN_TIMEOUT" -t "$DB_QUERY_TIMEOUT" -d "$LOCAL_DB_NAME" \
      < "$ROOT_DIR/scripts/sql/ensure-hanhvi-schema.sql" \
      > /tmp/ahasale_hanhvi_schema_guard.log 2>&1; then
    ok "ensure-hanhvi-schema.sql executed successfully."
  else
    fail "ensure-hanhvi-schema.sql failed. See /tmp/ahasale_hanhvi_schema_guard.log"
  fi

  SQL_VERIFY="
SET NOCOUNT ON;
DECLARE @legacy_columns INT = (
  SELECT COUNT(*) FROM sys.columns
  WHERE name IN (N'KyHieu9ViCon_1_9', N'LoaiVi')
);
DECLARE @required_columns INT = (
  SELECT COUNT(*) FROM sys.columns
  WHERE (object_id = OBJECT_ID(N'dbo.LichSu_DongA_tb') AND name = N'KyHieu9HanhVi_1_9')
     OR (object_id = OBJECT_ID(N'dbo.ViLoiNhuan_LichSuBanHang_ChiTiet_tb') AND name = N'LoaiHanhVi')
     OR (object_id = OBJECT_ID(N'dbo.YeuCauRutQuyen_tb') AND name = N'LoaiHanhVi')
     OR (object_id = OBJECT_ID(N'dbo.YeuCauRutQuyen_tb') AND name = N'KyHieu9HanhVi_1_9')
);
SELECT @legacy_columns AS legacy_columns, @required_columns AS required_columns;
IF @legacy_columns > 0
    THROW 51040, N'Legacy columns still exist', 1;
IF @required_columns < 4
    THROW 51041, N'Missing required hanhvi columns', 1;
"
  if docker exec "$DB_CONTAINER" /opt/mssql-tools18/bin/sqlcmd \
      -C -S localhost -U sa -P 'AhaSaleLocal#2026' -l "$DB_LOGIN_TIMEOUT" -t "$DB_QUERY_TIMEOUT" -d "$LOCAL_DB_NAME" \
      -Q "$SQL_VERIFY" -W -s '|' \
      >> /tmp/ahasale_hanhvi_schema_guard.log 2>&1; then
    ok "DB schema uses hanhvi columns only."
  else
    fail "DB schema guard failed. See /tmp/ahasale_hanhvi_schema_guard.log"
  fi
fi
echo

if [ "$EXIT_CODE" -ne 0 ]; then
  echo "HanhVi sync guard: FAILED"
else
  echo "HanhVi sync guard: PASSED"
fi

exit "$EXIT_CODE"
