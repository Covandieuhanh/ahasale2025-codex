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
    BEGIN TRAN;

    /* Required columns for hanhvi naming */
    IF COL_LENGTH(N'dbo.LichSu_DongA_tb', N'KyHieu9HanhVi_1_9') IS NULL
        THROW 51001, N'Missing required column dbo.LichSu_DongA_tb.KyHieu9HanhVi_1_9', 1;

    IF COL_LENGTH(N'dbo.ViLoiNhuan_LichSuBanHang_ChiTiet_tb', N'LoaiHanhVi') IS NULL
        THROW 51002, N'Missing required column dbo.ViLoiNhuan_LichSuBanHang_ChiTiet_tb.LoaiHanhVi', 1;

    IF COL_LENGTH(N'dbo.YeuCauRutQuyen_tb', N'LoaiHanhVi') IS NULL
        THROW 51003, N'Missing required column dbo.YeuCauRutQuyen_tb.LoaiHanhVi', 1;

    IF COL_LENGTH(N'dbo.YeuCauRutQuyen_tb', N'KyHieu9HanhVi_1_9') IS NULL
        THROW 51004, N'Missing required column dbo.YeuCauRutQuyen_tb.KyHieu9HanhVi_1_9', 1;

    /* Lich su ledger indexes */
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.LichSu_DongA_tb') AND name = N'IX_LichSu_DongA_tb_TaiKhoan_HoSo_Ngay_Id')
        EXEC(N'CREATE NONCLUSTERED INDEX IX_LichSu_DongA_tb_TaiKhoan_HoSo_Ngay_Id
               ON dbo.LichSu_DongA_tb (taikhoan_idx ASC, LoaiHoSo_Vi ASC, ngay DESC, id DESC)
               INCLUDE (dongA, CongTru, KyHieu9HanhVi_1_9);');

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.LichSu_DongA_tb') AND name = N'IX_LichSu_DongA_tb_IdRutDiem')
        EXEC(N'CREATE NONCLUSTERED INDEX IX_LichSu_DongA_tb_IdRutDiem
               ON dbo.LichSu_DongA_tb (id_rutdiem_idx ASC)
               INCLUDE (LoaiHoSo_Vi, KyHieu9HanhVi_1_9, dongA, ngay, taikhoan_idx);');

    /* Distribution detail table indexes/default */
    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.default_constraints dc
        JOIN sys.columns c
          ON c.object_id = dc.parent_object_id
         AND c.column_id = dc.parent_column_id
        WHERE dc.parent_object_id = OBJECT_ID(N'dbo.ViLoiNhuan_LichSuBanHang_ChiTiet_tb')
          AND c.name = N'LoaiHanhVi'
    )
        ALTER TABLE dbo.ViLoiNhuan_LichSuBanHang_ChiTiet_tb
        ADD CONSTRAINT DF_VLNH_LSBH_CT_LoaiHanhVi DEFAULT ((0)) FOR LoaiHanhVi;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ViLoiNhuan_LichSuBanHang_ChiTiet_tb') AND name = N'IX_VLNH_LSBH_CT_id_LichSuBanHang')
        EXEC(N'CREATE NONCLUSTERED INDEX IX_VLNH_LSBH_CT_id_LichSuBanHang
               ON dbo.ViLoiNhuan_LichSuBanHang_ChiTiet_tb (id_LichSuBanHang ASC)
               INCLUDE (TaiKhoan_Nhan, DongANhanDuoc, LoaiHanhVi, ThoiGian);');

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ViLoiNhuan_LichSuBanHang_ChiTiet_tb') AND name = N'IX_VLNH_LSBH_CT_TaiKhoan_Nhan_ThoiGian')
        EXEC(N'CREATE NONCLUSTERED INDEX IX_VLNH_LSBH_CT_TaiKhoan_Nhan_ThoiGian
               ON dbo.ViLoiNhuan_LichSuBanHang_ChiTiet_tb (TaiKhoan_Nhan ASC, ThoiGian DESC)
               INCLUDE (id_LichSuBanHang, DongANhanDuoc, LoaiHanhVi);');

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ViLoiNhuan_LichSuBanHang_ChiTiet_tb') AND name = N'UX_VLNH_LSBH_CT_NoDup')
        EXEC(N'CREATE UNIQUE NONCLUSTERED INDEX UX_VLNH_LSBH_CT_NoDup
               ON dbo.ViLoiNhuan_LichSuBanHang_ChiTiet_tb (id_LichSuBanHang ASC, TaiKhoan_Nhan ASC, LoaiHanhVi ASC);');

    /* Request table constraints/index */
    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.YeuCauRutQuyen_tb') AND name = N'CK_YCRQ_LoaiHanhVi')
        EXEC(N'ALTER TABLE dbo.YeuCauRutQuyen_tb WITH CHECK
               ADD CONSTRAINT CK_YCRQ_LoaiHanhVi CHECK (LoaiHanhVi IN (1,2,3));');

    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.YeuCauRutQuyen_tb') AND name = N'CK_YCRQ_KyHieu9HanhVi')
        EXEC(N'ALTER TABLE dbo.YeuCauRutQuyen_tb WITH CHECK
               ADD CONSTRAINT CK_YCRQ_KyHieu9HanhVi CHECK (KyHieu9HanhVi_1_9 >= 1 AND KyHieu9HanhVi_1_9 <= 9);');

    IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.YeuCauRutQuyen_tb') AND name = N'CK_YCRQ_TrangThai')
        EXEC(N'ALTER TABLE dbo.YeuCauRutQuyen_tb WITH CHECK
               ADD CONSTRAINT CK_YCRQ_TrangThai CHECK (TrangThai IN (0,1,2));');

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.YeuCauRutQuyen_tb') AND name = N'IX_YCRQ_LoaiHanhVi_KyHieu')
        EXEC(N'CREATE NONCLUSTERED INDEX IX_YCRQ_LoaiHanhVi_KyHieu
               ON dbo.YeuCauRutQuyen_tb (LoaiHanhVi ASC, KyHieu9HanhVi_1_9 ASC, NgayTao DESC);');

    COMMIT TRAN;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRAN;
    THROW;
END CATCH;

SELECT t.name AS table_name, c.name AS column_name
FROM sys.tables t
JOIN sys.columns c ON c.object_id = t.object_id
WHERE t.name IN (N'LichSu_DongA_tb', N'ViLoiNhuan_LichSuBanHang_ChiTiet_tb', N'YeuCauRutQuyen_tb')
  AND c.name IN (N'KyHieu9HanhVi_1_9', N'LoaiHanhVi')
ORDER BY t.name, c.name;
