SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;

BEGIN TRY
    BEGIN TRAN;

    DECLARE @Now DATETIME = GETDATE();

    -- 1) Ensure home_root exists (single sink account for fallback distribution)
    IF NOT EXISTS (SELECT 1 FROM dbo.taikhoan_tb WHERE taikhoan = N'home_root')
    BEGIN
        INSERT INTO dbo.taikhoan_tb
        (
            taikhoan, matkhau, hoten, ten, hoten_khongdau, phanloai,
            permission, block, nguoitao, ngaytao, hansudung, DongA,
            Affiliate_tai_khoan_cap_tren, Affiliate_duong_dan_tuyen_tren, Affiliate_cap_tuyen,
            HeThongSanPham_Cap123, HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6,
            HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10, HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4,
            anhdaidien
        )
        VALUES
        (
            N'home_root', N'123123', N'Home Root', N'HOME ROOT', N'home-root', N'Khách hàng',
            N'portal_home', 0, N'system', @Now, DATEADD(YEAR, 50, @Now), 0,
            N'', N',', 0,
            0, NULL, NULL, NULL,
            N'/uploads/images/macdinh.jpg'
        );
    END
    ELSE
    BEGIN
        UPDATE dbo.taikhoan_tb
        SET
            permission = CASE
                WHEN permission IS NULL OR LTRIM(RTRIM(permission)) = N'' THEN N'portal_home'
                WHEN permission LIKE N'%portal_home%' THEN permission
                ELSE permission + N',portal_home'
            END,
            phanloai = N'Khách hàng',
            block = 0,
            HeThongSanPham_Cap123 = 0,
            HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 = NULL,
            HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 = NULL,
            HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 = NULL
        WHERE taikhoan = N'home_root';
    END

    -- 2) Normalize admin root account permissions/scope
    UPDATE dbo.taikhoan_tb
    SET
        phanloai = N'Nhân viên admin',
        permission = N'1,2,3,4,5,q1_1,q1_2,q1_3,q1_4,q1_5,q1_6,q1_7,q2_1,q2_2,q2_3,q2_4,q2_5,portal_admin',
        block = 0
    WHERE taikhoan = N'admin';

    -- 3) Reset transactional data to avoid mixed legacy/new logic
    DELETE FROM dbo.ViLoiNhuan_LichSuBanHang_ChiTiet_tb;
    DELETE FROM hot79334_aha_sale.ViLoiNhuan_LichSuBanHang_tb;
    DELETE FROM dbo.YeuCauRutQuyen_tb;
    DELETE FROM hot79334_aha_sale.YeuCau_HeThongSanPham_tb;

    DELETE FROM dbo.LichSu_DongA_tb
    WHERE LoaiHoSo_Vi IN (2, 3, 4)
       OR ISNULL(id_rutdiem, N'') LIKE N'SALE_DIST:%'
       OR ISNULL(id_rutdiem, N'') LIKE N'SALE_ROUNDING:%'
       OR ISNULL(id_rutdiem, N'') LIKE N'YCRQ:%';

    -- 4) Reset profile/behavior balances for non-admin scope accounts
    UPDATE dbo.taikhoan_tb
    SET
        DuVi1_Evocher_30PhanTram = 0,
        DuVi2_LaoDong_50PhanTram = 0,
        DuVi3_GanKet_20PhanTram = 0,
        Vi1_15PhanTram = 0,
        Vi2_9PhanTram = 0,
        Vi3_6PhanTram = 0,
        Vi4_10PhanTram = 0,
        Vi5_15PhanTram = 0,
        Vi6_25PhanTram = 0,
        Vi7_4PhanTram = 0,
        Vi8_6PhanTram = 0,
        Vi9_10PhanTram = 0,
        Vi1That_Evocher_30PhanTram = 0,
        Vi2That_LaoDong_50PhanTram = 0,
        Vi3That_GanKet_20PhanTram = 0,
        HoSo_TieuDung_ShopOnly = 0,
        HoSo_UuDai_ShopOnly = 0,
        HeThongSanPham_Cap123 = 0,
        HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 = NULL,
        HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 = NULL,
        HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 = NULL
    WHERE taikhoan <> N'admin'
      AND (permission IS NULL OR permission NOT LIKE N'%portal_admin%');

    -- 5) Scope normalization (single scope token per account)
    -- 5.1 Admin scope rows
    UPDATE dbo.taikhoan_tb
    SET permission = ISNULL(permission, N'')
    WHERE permission IS NULL;

    UPDATE dbo.taikhoan_tb
    SET permission = REPLACE(REPLACE(permission, N'portal_home', N''), N'portal_shop', N'')
    WHERE taikhoan = N'admin'
       OR permission LIKE N'%portal_admin%'
       OR permission LIKE N'%q1_%'
       OR permission LIKE N'%q2_%';

    UPDATE dbo.taikhoan_tb
    SET permission = CASE
        WHEN LTRIM(RTRIM(permission)) = N'' THEN N'portal_admin'
        WHEN permission LIKE N'%portal_admin%' THEN permission
        ELSE permission + N',portal_admin'
    END
    WHERE taikhoan = N'admin'
       OR permission LIKE N'%q1_%'
       OR permission LIKE N'%q2_%'
       OR permission LIKE N'%portal_admin%';

    -- 5.2 Shop scope rows (approved registration or already shop-scoped)
    ;WITH ShopRows AS
    (
        SELECT DISTINCT taikhoan
        FROM dbo.DangKy_GianHangDoiTac_tb
        WHERE TrangThai = 1
        UNION
        SELECT taikhoan
        FROM dbo.taikhoan_tb
        WHERE permission LIKE N'%portal_shop%'
    )
    UPDATE tk
    SET tk.permission = REPLACE(REPLACE(ISNULL(tk.permission, N''), N'portal_admin', N''), N'portal_home', N'')
    FROM dbo.taikhoan_tb tk
    INNER JOIN ShopRows s ON s.taikhoan = tk.taikhoan;

    ;WITH ShopRows AS
    (
        SELECT DISTINCT taikhoan
        FROM dbo.DangKy_GianHangDoiTac_tb
        WHERE TrangThai = 1
        UNION
        SELECT taikhoan
        FROM dbo.taikhoan_tb
        WHERE permission LIKE N'%portal_shop%'
    )
    UPDATE tk
    SET
        tk.permission = CASE
            WHEN LTRIM(RTRIM(ISNULL(tk.permission, N''))) = N'' THEN N'portal_shop'
            WHEN tk.permission LIKE N'%portal_shop%' THEN tk.permission
            ELSE tk.permission + N',portal_shop'
        END,
        tk.phanloai = N'Gian hàng đối tác'
    FROM dbo.taikhoan_tb tk
    INNER JOIN ShopRows s ON s.taikhoan = tk.taikhoan;

    -- 5.3 Home scope rows (default)
    UPDATE dbo.taikhoan_tb
    SET permission = REPLACE(REPLACE(ISNULL(permission, N''), N'portal_admin', N''), N'portal_shop', N'')
    WHERE taikhoan <> N'admin'
      AND taikhoan NOT IN
      (
          SELECT DISTINCT taikhoan FROM dbo.DangKy_GianHangDoiTac_tb WHERE TrangThai = 1
      )
      AND (permission NOT LIKE N'%q1_%' AND permission NOT LIKE N'%q2_%');

    UPDATE dbo.taikhoan_tb
    SET permission = CASE
        WHEN LTRIM(RTRIM(ISNULL(permission, N''))) = N'' THEN N'portal_home'
        WHEN permission LIKE N'%portal_home%' THEN permission
        ELSE permission + N',portal_home'
    END
    WHERE taikhoan <> N'admin'
      AND taikhoan NOT IN
      (
          SELECT DISTINCT taikhoan FROM dbo.DangKy_GianHangDoiTac_tb WHERE TrangThai = 1
      )
      AND permission NOT LIKE N'%portal_admin%'
      AND permission NOT LIKE N'%portal_shop%';

    UPDATE dbo.taikhoan_tb
    SET phanloai = N'Khách hàng'
    WHERE taikhoan <> N'admin'
      AND permission LIKE N'%portal_home%'
      AND permission NOT LIKE N'%portal_admin%';

    -- 6) Ensure demo home account aha0001 is always available on home scope.
    IF NOT EXISTS (SELECT 1 FROM dbo.taikhoan_tb WHERE taikhoan = N'aha0001')
    BEGIN
        INSERT INTO dbo.taikhoan_tb
        (
            taikhoan, matkhau, hoten, ten, hoten_khongdau, phanloai,
            permission, block, nguoitao, ngaytao, hansudung, DongA,
            Affiliate_tai_khoan_cap_tren, Affiliate_duong_dan_tuyen_tren, Affiliate_cap_tuyen,
            HeThongSanPham_Cap123, HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6,
            HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10, HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4,
            anhdaidien
        )
        VALUES
        (
            N'aha0001', N'123123', N'Aha 0001', N'Aha 0001', N'aha-0001', N'Khách hàng',
            N'portal_home', 0, N'system', @Now, DATEADD(YEAR, 50, @Now), 0,
            N'home_root', N',home_root,', 1,
            0, NULL, NULL, NULL,
            N'/uploads/images/macdinh.jpg'
        );
    END
    ELSE
    BEGIN
        UPDATE dbo.taikhoan_tb
        SET
            matkhau = N'123123',
            phanloai = N'Khách hàng',
            permission = N'portal_home',
            block = 0,
            HeThongSanPham_Cap123 = 0,
            HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 = NULL,
            HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 = NULL,
            HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 = NULL
        WHERE taikhoan = N'aha0001';
    END

    COMMIT TRAN;

    SELECT
        scope_admin = SUM(CASE WHEN permission LIKE N'%portal_admin%' THEN 1 ELSE 0 END),
        scope_home  = SUM(CASE WHEN permission LIKE N'%portal_home%' THEN 1 ELSE 0 END),
        scope_shop  = SUM(CASE WHEN permission LIKE N'%portal_shop%' THEN 1 ELSE 0 END)
    FROM dbo.taikhoan_tb;

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRAN;
    THROW;
END CATCH;
