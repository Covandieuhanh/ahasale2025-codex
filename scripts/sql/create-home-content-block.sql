SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

IF OBJECT_ID(N'dbo.Home_Content_Block_tb', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Home_Content_Block_tb
    (
        block_key NVARCHAR(100) NOT NULL,
        title NVARCHAR(250) NULL,
        html_content NVARCHAR(MAX) NULL,
        is_enabled BIT NOT NULL,
        updated_by NVARCHAR(50) NULL,
        updated_at DATETIME NULL,
        created_at DATETIME NOT NULL,
        CONSTRAINT PK_Home_Content_Block_tb PRIMARY KEY CLUSTERED (block_key ASC)
    );
END;

IF COL_LENGTH(N'dbo.Home_Content_Block_tb', N'title') IS NULL
BEGIN
    ALTER TABLE dbo.Home_Content_Block_tb ADD title NVARCHAR(250) NULL;
END;

IF COL_LENGTH(N'dbo.Home_Content_Block_tb', N'html_content') IS NULL
BEGIN
    ALTER TABLE dbo.Home_Content_Block_tb ADD html_content NVARCHAR(MAX) NULL;
END;

IF COL_LENGTH(N'dbo.Home_Content_Block_tb', N'is_enabled') IS NULL
BEGIN
    ALTER TABLE dbo.Home_Content_Block_tb ADD is_enabled BIT NULL;
END;

IF COL_LENGTH(N'dbo.Home_Content_Block_tb', N'updated_by') IS NULL
BEGIN
    ALTER TABLE dbo.Home_Content_Block_tb ADD updated_by NVARCHAR(50) NULL;
END;

IF COL_LENGTH(N'dbo.Home_Content_Block_tb', N'updated_at') IS NULL
BEGIN
    ALTER TABLE dbo.Home_Content_Block_tb ADD updated_at DATETIME NULL;
END;

IF COL_LENGTH(N'dbo.Home_Content_Block_tb', N'created_at') IS NULL
BEGIN
    ALTER TABLE dbo.Home_Content_Block_tb ADD created_at DATETIME NULL;
END;

IF OBJECT_ID(N'DF_Home_Content_Block_tb_is_enabled', N'D') IS NULL
BEGIN
    ALTER TABLE dbo.Home_Content_Block_tb
    ADD CONSTRAINT DF_Home_Content_Block_tb_is_enabled DEFAULT ((1)) FOR is_enabled;
END;

IF OBJECT_ID(N'DF_Home_Content_Block_tb_created_at', N'D') IS NULL
BEGIN
    ALTER TABLE dbo.Home_Content_Block_tb
    ADD CONSTRAINT DF_Home_Content_Block_tb_created_at DEFAULT (GETDATE()) FOR created_at;
END;

UPDATE dbo.Home_Content_Block_tb
SET is_enabled = 1
WHERE is_enabled IS NULL;

UPDATE dbo.Home_Content_Block_tb
SET created_at = ISNULL(updated_at, GETDATE())
WHERE created_at IS NULL;

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Home_Content_Block_tb')
      AND name = N'is_enabled'
      AND is_nullable = 1
)
BEGIN
    ALTER TABLE dbo.Home_Content_Block_tb
    ALTER COLUMN is_enabled BIT NOT NULL;
END;

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Home_Content_Block_tb')
      AND name = N'created_at'
      AND is_nullable = 1
)
BEGIN
    ALTER TABLE dbo.Home_Content_Block_tb
    ALTER COLUMN created_at DATETIME NOT NULL;
END;

SELECT
    block_count = COUNT(1),
    enabled_count = ISNULL(SUM(CASE WHEN is_enabled = 1 THEN 1 ELSE 0 END), 0)
FROM dbo.Home_Content_Block_tb;
