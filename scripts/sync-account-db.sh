#!/usr/bin/env bash
set -euo pipefail

SQL="
SET NOCOUNT ON;

-- 1) Chuan hoa thong tin dang nhap co ban
UPDATE ahasale_local.dbo.taikhoan_tb
SET taikhoan = LOWER(LTRIM(RTRIM(taikhoan)))
WHERE taikhoan IS NOT NULL
  AND taikhoan <> LOWER(LTRIM(RTRIM(taikhoan)));

UPDATE ahasale_local.dbo.taikhoan_tb
SET matkhau = LTRIM(RTRIM(matkhau))
WHERE matkhau IS NOT NULL
  AND matkhau <> LTRIM(RTRIM(matkhau));

UPDATE ahasale_local.dbo.taikhoan_tb
SET email = NULLIF(LOWER(LTRIM(RTRIM(email))), '')
WHERE email IS NOT NULL;

UPDATE ahasale_local.dbo.taikhoan_tb
SET dienthoai = NULLIF(
    REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(dienthoai)),
        ' ', ''), '+', ''), '-', ''), '.', ''), '(', ''), ')', ''), ',', ''),
    ''
)
WHERE dienthoai IS NOT NULL;

UPDATE ahasale_local.dbo.taikhoan_tb
SET dienthoai = '0' + SUBSTRING(dienthoai, 3, 250)
WHERE dienthoai LIKE '84%';

UPDATE ahasale_local.dbo.taikhoan_tb
SET block = 0
WHERE block IS NULL;

UPDATE ahasale_local.dbo.taikhoan_tb
SET permission = ''
WHERE permission IS NULL;

-- 2) Bao dam tai khoan admin local full quyen de test nang cap
IF EXISTS (SELECT 1 FROM ahasale_local.dbo.taikhoan_tb WHERE taikhoan = 'admin')
BEGIN
    UPDATE ahasale_local.dbo.taikhoan_tb
    SET
        phanloai = N'Cộng tác phát triển',
        matkhau = '123',
        block = 0,
        hansudung = NULL,
        permission = '1,2,3,4,5,q1_1,q1_2,q1_3,q1_4,q1_5,q1_6,q1_7'
    WHERE taikhoan = 'admin';
END

-- 3) Bao cao trang thai du lieu tai khoan
SELECT
    COUNT(*) AS total_accounts,
    SUM(CASE WHEN taikhoan IS NULL OR LTRIM(RTRIM(taikhoan)) = '' THEN 1 ELSE 0 END) AS blank_taikhoan,
    SUM(CASE WHEN matkhau IS NULL OR LTRIM(RTRIM(matkhau)) = '' THEN 1 ELSE 0 END) AS blank_matkhau,
    SUM(CASE WHEN block = 1 THEN 1 ELSE 0 END) AS blocked_accounts,
    SUM(CASE WHEN email IS NULL OR LTRIM(RTRIM(email)) = '' THEN 1 ELSE 0 END) AS blank_email,
    SUM(CASE WHEN dienthoai IS NULL OR LTRIM(RTRIM(dienthoai)) = '' THEN 1 ELSE 0 END) AS blank_phone
FROM ahasale_local.dbo.taikhoan_tb;

SELECT 'dup_taikhoan' AS issue, taikhoan AS value_key, COUNT(*) AS total
FROM ahasale_local.dbo.taikhoan_tb
WHERE taikhoan IS NOT NULL AND LTRIM(RTRIM(taikhoan)) <> ''
GROUP BY taikhoan
HAVING COUNT(*) > 1
UNION ALL
SELECT 'dup_email' AS issue, email AS value_key, COUNT(*) AS total
FROM ahasale_local.dbo.taikhoan_tb
WHERE email IS NOT NULL AND LTRIM(RTRIM(email)) <> ''
GROUP BY email
HAVING COUNT(*) > 1
UNION ALL
SELECT 'dup_dienthoai' AS issue, dienthoai AS value_key, COUNT(*) AS total
FROM ahasale_local.dbo.taikhoan_tb
WHERE dienthoai IS NOT NULL AND LTRIM(RTRIM(dienthoai)) <> ''
GROUP BY dienthoai
HAVING COUNT(*) > 1;

SELECT TOP 1 taikhoan, phanloai, block, permission
FROM ahasale_local.dbo.taikhoan_tb
WHERE taikhoan = 'admin';
"

docker exec ahasale_local_db /opt/mssql-tools18/bin/sqlcmd \
  -C -S localhost -U sa -P 'AhaSaleLocal#2026' \
  -Q "$SQL" -W -s '|'

