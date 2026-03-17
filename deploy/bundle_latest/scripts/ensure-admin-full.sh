#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
source "$ROOT_DIR/scripts/lib/local-db-env.sh"
aha_export_local_db_env

SQL="
IF EXISTS (SELECT 1 FROM [${LOCAL_DB_NAME}].dbo.taikhoan_tb WHERE taikhoan='admin')
BEGIN
    UPDATE [${LOCAL_DB_NAME}].dbo.taikhoan_tb
    SET
        matkhau = '123',
        phanloai = N'Nhân viên admin',
        hansudung = NULL,
        block = 0,
        permission = '1,2,3,4,5,q1_1,q1_2,q1_3,q1_4,q1_5,q1_6,q1_7,q2_1,q2_2,q2_3,q2_4,q2_5,portal_admin'
    WHERE taikhoan = 'admin';
END

SELECT TOP 1
    taikhoan,
    matkhau,
    phanloai,
    block,
    permission
FROM [${LOCAL_DB_NAME}].dbo.taikhoan_tb
WHERE taikhoan = 'admin';
"

docker exec ahasale_local_db /opt/mssql-tools18/bin/sqlcmd \
  -C -S localhost -U sa -P 'AhaSaleLocal#2026' -l "$DB_LOGIN_TIMEOUT" -t "$DB_QUERY_TIMEOUT" \
  -d "$LOCAL_DB_NAME" -Q "$SQL" -W -s '|'
