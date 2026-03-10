SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

IF OBJECT_ID(N'dbo.Home_Text_Content_tb', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Home_Text_Content_tb
    (
        content_key NVARCHAR(100) NOT NULL,
        title NVARCHAR(250) NULL,
        text_content NVARCHAR(MAX) NULL,
        is_enabled BIT NOT NULL CONSTRAINT DF_Home_Text_Content_tb_is_enabled DEFAULT((1)),
        updated_by NVARCHAR(50) NULL,
        updated_at DATETIME NULL,
        created_at DATETIME NOT NULL CONSTRAINT DF_Home_Text_Content_tb_created_at DEFAULT(GETDATE()),
        CONSTRAINT PK_Home_Text_Content_tb PRIMARY KEY CLUSTERED (content_key ASC)
    );
END;

IF OBJECT_ID(N'dbo.Home_Footer_Article_tb', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Home_Footer_Article_tb
    (
        id INT IDENTITY(1,1) NOT NULL,
        content_key NVARCHAR(120) NOT NULL,
        group_key NVARCHAR(50) NOT NULL,
        display_name NVARCHAR(250) NOT NULL,
        slug NVARCHAR(180) NOT NULL,
        target_url NVARCHAR(500) NULL,
        body_content NVARCHAR(MAX) NULL,
        sort_order INT NOT NULL CONSTRAINT DF_Home_Footer_Article_tb_sort_order DEFAULT((0)),
        is_enabled BIT NOT NULL CONSTRAINT DF_Home_Footer_Article_tb_is_enabled DEFAULT((1)),
        updated_by NVARCHAR(50) NULL,
        updated_at DATETIME NULL,
        created_at DATETIME NOT NULL CONSTRAINT DF_Home_Footer_Article_tb_created_at DEFAULT(GETDATE()),
        CONSTRAINT PK_Home_Footer_Article_tb PRIMARY KEY CLUSTERED (id ASC)
    );
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Home_Footer_Article_tb_content_key'
      AND object_id = OBJECT_ID(N'dbo.Home_Footer_Article_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_Home_Footer_Article_tb_content_key
    ON dbo.Home_Footer_Article_tb(content_key);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Home_Footer_Article_tb_slug'
      AND object_id = OBJECT_ID(N'dbo.Home_Footer_Article_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_Home_Footer_Article_tb_slug
    ON dbo.Home_Footer_Article_tb(slug);
END;

SELECT
    text_block_count = (SELECT COUNT(1) FROM dbo.Home_Text_Content_tb),
    footer_article_count = (SELECT COUNT(1) FROM dbo.Home_Footer_Article_tb);
