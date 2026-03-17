#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
source "$ROOT_DIR/scripts/lib/local-db-env.sh"
aha_export_local_db_env
SQL_FILE="$ROOT_DIR/scripts/sql/seed-gianhang-demo.sql"

SHOP_TK="${SHOP_TK:-gh_demo}"
SHOP_PW="${SHOP_PW:-123123}"
SHOP_EMAIL="${SHOP_EMAIL:-demo@ahasale.local}"

if [ ! -f "$SQL_FILE" ]; then
  echo "Missing SQL file: $SQL_FILE" >&2
  exit 1
fi

printf "Seeding GianHang demo data for shop '%s'...\n" "$SHOP_TK"

docker exec -i "$DB_CONTAINER" /opt/mssql-tools18/bin/sqlcmd \
  -b -C -S localhost -U sa -P 'AhaSaleLocal#2026' -l "$DB_LOGIN_TIMEOUT" -t "$DB_QUERY_TIMEOUT" -d "$LOCAL_DB_NAME" \
  -v SHOP_TK="$SHOP_TK" -v SHOP_PW="$SHOP_PW" -v SHOP_EMAIL="$SHOP_EMAIL" \
  < "$SQL_FILE"

printf "Seed completed.\n"
