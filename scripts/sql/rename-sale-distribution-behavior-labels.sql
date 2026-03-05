SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;

BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE dbo.LichSu_DongA_tb
    SET ghichu =
        REPLACE(
        REPLACE(
        REPLACE(
        REPLACE(
        REPLACE(
        REPLACE(
        REPLACE(
        REPLACE(
        REPLACE(ISNULL(ghichu, N''),
            N'15% = Kết nối', N'15% = Hành vi kết nối'),
            N'9% = Chia sẻ', N'9% = Hành vi chia sẻ'),
            N'6% = Marketing', N'6% = Hành vi marketing'),
            N'10% = Bán hàng', N'10% = Hành vi bán hàng'),
            N'15% = Phát triển', N'15% = Hành vi phát triển'),
            N'25% = Điều phối', N'25% = Hành vi điều phối'),
            N'10% = Phúc lợi', N'10% = Hành vi phúc lợi'),
            N'6% = Ghi nhận', N'6% = Hành vi ghi nhận'),
            N'4% = Chăm sóc', N'4% = Hành vi chăm sóc')
    WHERE id_rutdiem LIKE N'SALE_DIST:%'
      AND ghichu IS NOT NULL
      AND
      (
          ghichu LIKE N'%15% = Kết nối%'
          OR ghichu LIKE N'%9% = Chia sẻ%'
          OR ghichu LIKE N'%6% = Marketing%'
          OR ghichu LIKE N'%10% = Bán hàng%'
          OR ghichu LIKE N'%15% = Phát triển%'
          OR ghichu LIKE N'%25% = Điều phối%'
          OR ghichu LIKE N'%10% = Phúc lợi%'
          OR ghichu LIKE N'%6% = Ghi nhận%'
          OR ghichu LIKE N'%4% = Chăm sóc%'
      );

    DECLARE @Updated INT = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT @Updated AS updated_rows;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
