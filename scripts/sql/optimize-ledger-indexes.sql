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

    IF COL_LENGTH(N'dbo.LichSu_DongA_tb', N'taikhoan_idx') IS NULL
    BEGIN
        ALTER TABLE dbo.LichSu_DongA_tb
        ADD taikhoan_idx AS CONVERT(NVARCHAR(450), ISNULL(taikhoan, N'')) PERSISTED;
    END;

    IF COL_LENGTH(N'dbo.LichSu_DongA_tb', N'id_rutdiem_idx') IS NULL
    BEGIN
        ALTER TABLE dbo.LichSu_DongA_tb
        ADD id_rutdiem_idx AS CONVERT(NVARCHAR(450), ISNULL(id_rutdiem, N'')) PERSISTED;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.LichSu_DongA_tb')
          AND name = N'IX_LichSu_DongA_tb_TaiKhoan_HoSo_Ngay_Id'
    )
    BEGIN
        EXEC(N'
            CREATE NONCLUSTERED INDEX IX_LichSu_DongA_tb_TaiKhoan_HoSo_Ngay_Id
            ON dbo.LichSu_DongA_tb
            (
                taikhoan_idx ASC,
                LoaiHoSo_Vi ASC,
                ngay DESC,
                id DESC
            )
            INCLUDE
            (
                dongA,
                CongTru,
                KyHieu9HanhVi_1_9
            );');
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.LichSu_DongA_tb')
          AND name = N'IX_LichSu_DongA_tb_IdRutDiem'
    )
    BEGIN
        EXEC(N'
            CREATE NONCLUSTERED INDEX IX_LichSu_DongA_tb_IdRutDiem
            ON dbo.LichSu_DongA_tb (id_rutdiem_idx ASC)
            INCLUDE (LoaiHoSo_Vi, KyHieu9HanhVi_1_9, dongA, ngay, taikhoan_idx);');
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;

SELECT i.name, i.is_unique, i.has_filter, i.filter_definition
FROM sys.indexes i
WHERE i.object_id = OBJECT_ID(N'dbo.LichSu_DongA_tb')
  AND i.name IN (N'IX_LichSu_DongA_tb_TaiKhoan_HoSo_Ngay_Id', N'IX_LichSu_DongA_tb_IdRutDiem');
