SET NOCOUNT ON;

IF COL_LENGTH('dbo.taikhoan_tb', 'ForceChangePasswordHome') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_tb
    ADD ForceChangePasswordHome BIT NOT NULL
        CONSTRAINT DF_taikhoan_tb_ForceChangePasswordHome DEFAULT(0) WITH VALUES;
END

IF COL_LENGTH('dbo.taikhoan_tb', 'ForceChangePinHome') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_tb
    ADD ForceChangePinHome BIT NOT NULL
        CONSTRAINT DF_taikhoan_tb_ForceChangePinHome DEFAULT(0) WITH VALUES;
END

IF COL_LENGTH('dbo.taikhoan_tb', 'ForceChangePasswordShop') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_tb
    ADD ForceChangePasswordShop BIT NOT NULL
        CONSTRAINT DF_taikhoan_tb_ForceChangePasswordShop DEFAULT(0) WITH VALUES;
END

IF COL_LENGTH('dbo.taikhoan_tb', 'ResetSecurityBy') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_tb
    ADD ResetSecurityBy NVARCHAR(50) NULL;
END

IF COL_LENGTH('dbo.taikhoan_tb', 'ResetSecurityAt') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_tb
    ADD ResetSecurityAt DATETIME NULL;
END

SELECT
    has_force_home_password = CASE WHEN COL_LENGTH('dbo.taikhoan_tb', 'ForceChangePasswordHome') IS NULL THEN 0 ELSE 1 END,
    has_force_home_pin = CASE WHEN COL_LENGTH('dbo.taikhoan_tb', 'ForceChangePinHome') IS NULL THEN 0 ELSE 1 END,
    has_force_shop_password = CASE WHEN COL_LENGTH('dbo.taikhoan_tb', 'ForceChangePasswordShop') IS NULL THEN 0 ELSE 1 END,
    has_reset_security_by = CASE WHEN COL_LENGTH('dbo.taikhoan_tb', 'ResetSecurityBy') IS NULL THEN 0 ELSE 1 END,
    has_reset_security_at = CASE WHEN COL_LENGTH('dbo.taikhoan_tb', 'ResetSecurityAt') IS NULL THEN 0 ELSE 1 END;
