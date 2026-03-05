#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

EXIT_CODE=0

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

echo "== HanhVi Sync Guard =="
echo "Workspace: $ROOT_DIR"
echo

echo "1) Check legacy naming in active source..."
LEGACY_REGEX='KyHieu9ViCon_1_9|(^|[^A-Za-z0-9_])LoaiVi([^A-Za-z0-9_]|$)|HoSoCon9Cap_cl|GetTenHoSoConTheoLoai|GetLoaiHoSoConByCapGiaTri|GetLoaiHoSoTongTheoHoSoCon|\bhosocon\b|\bvicon\b|\bViCon\b|\bHoSoCon\b|\bTenHoSoCon\b|\bLoaiHoSoCon\b'
if rg -n -S "$LEGACY_REGEX" App_Code admin home scripts/sql --glob '!**/Bin/**' > /tmp/ahasale_hanhvi_guard_legacy.log; then
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
  if rg -q -S "$token" App_Code admin home scripts/sql --glob '!**/Bin/**'; then
    ok "Token present: $token"
  else
    fail "Missing required token: $token"
  fi
done
echo

echo "3) Check database schema (if local DB container is running)..."
if ! command -v docker >/dev/null 2>&1; then
  warn "Docker not found. Skipped DB schema guard."
elif ! docker ps --format '{{.Names}}' | grep -qx 'ahasale_local_db'; then
  warn "Container ahasale_local_db is not running. Skipped DB schema guard."
else
  if docker exec -i ahasale_local_db /opt/mssql-tools18/bin/sqlcmd \
      -C -S localhost -U sa -P 'AhaSaleLocal#2026' -d ahasale_local \
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
  if docker exec ahasale_local_db /opt/mssql-tools18/bin/sqlcmd \
      -C -S localhost -U sa -P 'AhaSaleLocal#2026' -d ahasale_local \
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
