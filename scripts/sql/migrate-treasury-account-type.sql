SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @LegacyType NVARCHAR(50) = N'Ví tổng';
    DECLARE @NewType NVARCHAR(50) = N'Tài khoản tổng';

    DECLARE @BeforeLegacy INT =
        (SELECT COUNT(*) FROM dbo.taikhoan_tb WHERE LTRIM(RTRIM(ISNULL(phanloai, N''))) = @LegacyType);
    DECLARE @BeforeNew INT =
        (SELECT COUNT(*) FROM dbo.taikhoan_tb WHERE LTRIM(RTRIM(ISNULL(phanloai, N''))) = @NewType);

    UPDATE dbo.taikhoan_tb
    SET phanloai =
        CASE
            WHEN LTRIM(RTRIM(ISNULL(phanloai, N''))) = @LegacyType THEN @NewType
            WHEN LTRIM(RTRIM(ISNULL(phanloai, N''))) LIKE @LegacyType + N' %'
                THEN @NewType + SUBSTRING(LTRIM(RTRIM(ISNULL(phanloai, N''))), LEN(@LegacyType) + 1, 4000)
            ELSE phanloai
        END
    WHERE LTRIM(RTRIM(ISNULL(phanloai, N''))) = @LegacyType
       OR LTRIM(RTRIM(ISNULL(phanloai, N''))) LIKE @LegacyType + N' %';

    DECLARE @Updated INT = @@ROWCOUNT;

    DECLARE @AfterLegacy INT =
        (SELECT COUNT(*) FROM dbo.taikhoan_tb WHERE LTRIM(RTRIM(ISNULL(phanloai, N''))) = @LegacyType);
    DECLARE @AfterNew INT =
        (SELECT COUNT(*) FROM dbo.taikhoan_tb WHERE LTRIM(RTRIM(ISNULL(phanloai, N''))) = @NewType);

    COMMIT TRANSACTION;

    SELECT
        @BeforeLegacy AS before_legacy,
        @BeforeNew AS before_new,
        @Updated AS updated_rows,
        @AfterLegacy AS after_legacy,
        @AfterNew AS after_new;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
