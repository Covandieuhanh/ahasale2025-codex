#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
source "$ROOT_DIR/scripts/lib/local-db-env.sh"
aha_export_local_db_env
SQL_FILE="$ROOT_DIR/scripts/sql/ensure-shop-slug-schema.sql"

docker exec -i "$DB_CONTAINER" /opt/mssql-tools18/bin/sqlcmd \
  -C -S localhost -U sa -P 'AhaSaleLocal#2026' -l "$DB_LOGIN_TIMEOUT" -t "$DB_QUERY_TIMEOUT" -d "$LOCAL_DB_NAME" \
  < "$SQL_FILE"
