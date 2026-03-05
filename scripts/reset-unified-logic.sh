#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SQL_FILE="$ROOT_DIR/scripts/sql/reset-unified-home-admin-shop.sql"

docker exec -i ahasale_local_db /opt/mssql-tools18/bin/sqlcmd \
  -C -S localhost -U sa -P 'AhaSaleLocal#2026' -d ahasale_local < "$SQL_FILE"

echo "Reset unified logic completed."
