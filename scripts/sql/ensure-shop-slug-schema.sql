SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

IF COL_LENGTH('dbo.taikhoan_tb', 'slug_shop') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_tb
    ADD slug_shop NVARCHAR(120) NULL;
END;

IF COL_LENGTH('dbo.taikhoan_tb', 'slug_shop') IS NOT NULL
BEGIN
    -- Chuẩn hóa dữ liệu cũ (nếu có) về lower + trim.
    EXEC(N'
        UPDATE dbo.taikhoan_tb
        SET slug_shop = LOWER(LTRIM(RTRIM(slug_shop)))
        WHERE slug_shop IS NOT NULL
          AND slug_shop <> LOWER(LTRIM(RTRIM(slug_shop)));
    ');

    -- Backfill slug cho shop hiện có (nếu chưa có): dùng taikhoan nội bộ để đảm bảo unique.
    EXEC(N'
        UPDATE dbo.taikhoan_tb
        SET slug_shop = LOWER(LTRIM(RTRIM(taikhoan)))
        WHERE (slug_shop IS NULL OR LTRIM(RTRIM(slug_shop)) = N'''')
          AND taikhoan IS NOT NULL
          AND LTRIM(RTRIM(taikhoan)) <> N''''
          AND permission LIKE N''%portal_shop%'';
    ');

    IF NOT EXISTS (
        SELECT 1
        FROM sys.indexes
        WHERE name = N'UX_taikhoan_tb_slug_shop'
          AND object_id = OBJECT_ID(N'dbo.taikhoan_tb')
    )
    BEGIN
        EXEC(N'
            CREATE UNIQUE NONCLUSTERED INDEX UX_taikhoan_tb_slug_shop
                ON dbo.taikhoan_tb(slug_shop)
                WHERE slug_shop IS NOT NULL AND slug_shop <> N'''';
        ');
    END;

    EXEC(N'
        SELECT
            has_slug_column = 1,
            slug_not_null_count = SUM(CASE WHEN slug_shop IS NOT NULL AND LTRIM(RTRIM(slug_shop)) <> N'''' THEN 1 ELSE 0 END)
        FROM dbo.taikhoan_tb;
    ');
END
ELSE
BEGIN
    SELECT
        has_slug_column = 0,
        slug_not_null_count = 0;
END;
