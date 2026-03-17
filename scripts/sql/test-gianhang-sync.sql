SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;

IF OBJECT_ID('dbo.web_post_table', 'U') IS NULL
    THROW 60001, 'Missing web_post_table (AhaShine posts)', 1;

IF OBJECT_ID('dbo.bspa_datlich_table', 'U') IS NULL
    THROW 60002, 'Missing bspa_datlich_table (AhaShine booking)', 1;

IF OBJECT_ID('dbo.bspa_hoadon_table', 'U') IS NULL
    THROW 60003, 'Missing bspa_hoadon_table (AhaShine invoices)', 1;

IF OBJECT_ID('dbo.taikhoan_table_2023', 'U') IS NULL
    THROW 60004, 'Missing taikhoan_table_2023 (AhaShine staff)', 1;

IF COL_LENGTH('dbo.web_post_table', 'id_baiviet') IS NULL
    THROW 60005, 'Missing id_baiviet sync column', 1;

SELECT ok = 1;
