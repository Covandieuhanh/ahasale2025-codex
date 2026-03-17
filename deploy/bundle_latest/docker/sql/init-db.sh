#!/usr/bin/env bash
set -euo pipefail

SA_PASSWORD="${MSSQL_SA_PASSWORD:-}"
DB_NAME="${LOCAL_DB_NAME:-ahasale_local}"
SQL_LOGIN_TIMEOUT="${DB_LOGIN_TIMEOUT:-5}"
if [ -z "$SA_PASSWORD" ]; then
  echo "[init-db] MSSQL_SA_PASSWORD is required"
  exit 1
fi

BACKUP_PATH="/var/opt/mssql/backup/ahasale.bak"
if [ ! -f "$BACKUP_PATH" ]; then
  echo "[init-db] Backup file not found at $BACKUP_PATH"
  exit 1
fi

SQLCMD=(/opt/mssql-tools18/bin/sqlcmd -C -S db -U sa -P "$SA_PASSWORD" -l "$SQL_LOGIN_TIMEOUT")

echo "[init-db] Waiting for SQL Server..."
for _ in $(seq 1 90); do
  if "${SQLCMD[@]}" -Q "SELECT 1" >/dev/null 2>&1; then
    break
  fi
  sleep 2
done

if ! "${SQLCMD[@]}" -Q "SELECT 1" >/dev/null 2>&1; then
  echo "[init-db] SQL Server is not ready"
  exit 1
fi

DB_EXISTS="$("${SQLCMD[@]}" -h -1 -W -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE name='${DB_NAME}';" | tr -d '\r' | tr -d '[:space:]')"
if [ "$DB_EXISTS" = "1" ]; then
  echo "[init-db] Database ${DB_NAME} already exists. Skip restore."
  exit 0
fi

echo "[init-db] Detecting logical file names from backup..."
FILELIST="$("${SQLCMD[@]}" -h -1 -W -s "|" -Q "SET NOCOUNT ON; RESTORE FILELISTONLY FROM DISK='${BACKUP_PATH}';")"
DATA_LOGICAL="$(printf '%s\n' "$FILELIST" | awk -F'|' '$3 ~ /D/ {gsub(/^[ \t]+|[ \t]+$/, "", $1); print $1; exit}')"
LOG_LOGICAL="$(printf '%s\n' "$FILELIST" | awk -F'|' '$3 ~ /L/ {gsub(/^[ \t]+|[ \t]+$/, "", $1); print $1; exit}')"

if [ -z "${DATA_LOGICAL}" ]; then
  DATA_LOGICAL="app56734_test_gpt"
fi
if [ -z "${LOG_LOGICAL}" ]; then
  LOG_LOGICAL="app56734_test_gpt_log"
fi

echo "[init-db] Restoring ${DB_NAME} from backup..."
"${SQLCMD[@]}" -Q "RESTORE DATABASE [${DB_NAME}] FROM DISK='${BACKUP_PATH}' WITH MOVE '${DATA_LOGICAL}' TO '/var/opt/mssql/data/${DB_NAME}.mdf', MOVE '${LOG_LOGICAL}' TO '/var/opt/mssql/data/${DB_NAME}_log.ldf', REPLACE, RECOVERY;"

echo "[init-db] Restore completed."
