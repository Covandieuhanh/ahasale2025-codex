SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @DeletedOldAutoRows INT = 0;
    DECLARE @InsertedRows INT = 0;

    ;WITH old_auto AS
    (
        SELECT ls.id
        FROM dbo.LichSu_DongA_tb ls
        WHERE ls.LoaiHoSo_Vi IN (2, 3, 4)
          AND ISNULL(ls.CongTru, 0) = 1
          AND (ls.ghichu LIKE N'Ghi nhận quyền ưu đãi từ ID:%'
               OR ls.ghichu LIKE N'Ghi nhận hành vi lao động từ ID:%'
               OR ls.ghichu LIKE N'Ghi nhận chỉ số gắn kết từ ID:%')
          AND (ISNULL(ls.id_rutdiem, N'') = N'' OR ls.id_rutdiem = N'0')
    )
    DELETE ls
    FROM dbo.LichSu_DongA_tb ls
    INNER JOIN old_auto x ON x.id = ls.id;

    SET @DeletedOldAutoRows = @@ROWCOUNT;

    ;WITH src AS
    (
        SELECT
            d.id_LichSuBanHang,
            d.TaiKhoan_Nhan,
            d.LoaiHanhVi,
            d.PhanTramNhanDuoc,
            d.DongANhanDuoc,
            d.ThoiGian,
            CASE
                WHEN d.LoaiHanhVi BETWEEN 1 AND 3 THEN 2
                WHEN d.LoaiHanhVi BETWEEN 4 AND 6 THEN 3
                WHEN d.LoaiHanhVi BETWEEN 7 AND 9 THEN 4
                ELSE NULL
            END AS LoaiHoSo_Vi,
            CASE d.LoaiHanhVi
                WHEN 1 THEN N'15% = Hành vi kết nối'
                WHEN 2 THEN N'9% = Hành vi chia sẻ'
                WHEN 3 THEN N'6% = Hành vi marketing'
                WHEN 4 THEN N'10% = Hành vi bán hàng'
                WHEN 5 THEN N'15% = Hành vi phát triển'
                WHEN 6 THEN N'25% = Hành vi điều phối'
                WHEN 7 THEN N'4% = Hành vi chăm sóc'
                WHEN 8 THEN N'6% = Hành vi ghi nhận'
                WHEN 9 THEN N'10% = Hành vi phúc lợi'
                ELSE N'Hành vi'
            END AS TenHanhVi
        FROM dbo.ViLoiNhuan_LichSuBanHang_ChiTiet_tb d
        WHERE ISNULL(d.TaiKhoan_Nhan, N'') <> N''
          AND ISNULL(d.DongANhanDuoc, 0) > 0
          AND d.LoaiHanhVi BETWEEN 1 AND 9
    )
    INSERT INTO dbo.LichSu_DongA_tb
    (
        taikhoan,
        dongA,
        ngay,
        CongTru,
        id_donhang,
        ghichu,
        id_rutdiem,
        LoaiHoSo_Vi,
        KyHieu9HanhVi_1_9
    )
    SELECT
        s.TaiKhoan_Nhan,
        s.DongANhanDuoc,
        s.ThoiGian,
        1,
        N'',
        N'Ghi nhận từ bán thẻ: ' + s.TenHanhVi
            + N' (ID: ' + CONVERT(NVARCHAR(30), s.id_LichSuBanHang) + N')',
        N'SALE_DIST:' + CONVERT(NVARCHAR(30), s.id_LichSuBanHang)
            + N':' + s.TaiKhoan_Nhan
            + N':' + CONVERT(NVARCHAR(10), s.LoaiHanhVi),
        s.LoaiHoSo_Vi,
        s.LoaiHanhVi
    FROM src s
    WHERE s.LoaiHoSo_Vi IS NOT NULL
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.LichSu_DongA_tb ls
          WHERE ls.id_rutdiem =
                N'SALE_DIST:' + CONVERT(NVARCHAR(30), s.id_LichSuBanHang)
                + N':' + s.TaiKhoan_Nhan
                + N':' + CONVERT(NVARCHAR(10), s.LoaiHanhVi)
      );

    SET @InsertedRows = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT
        @DeletedOldAutoRows AS deleted_old_auto_rows,
        @InsertedRows AS inserted_rows,
        (SELECT COUNT(*) FROM dbo.LichSu_DongA_tb WHERE id_rutdiem LIKE N'SALE_DIST:%') AS total_sale_dist_rows;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
