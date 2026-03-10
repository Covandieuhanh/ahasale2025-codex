SET NOCOUNT ON;

DECLARE @TaiKhoan NVARCHAR(100) = N'shop_cong_ty';
DECLARE @Email NVARCHAR(255) = N'admin@ahasale.vn';
DECLARE @MatKhau NVARCHAR(255) = N'1313';
DECLARE @NguoiTao NVARCHAR(100) = N'admin';

IF EXISTS (SELECT 1 FROM dbo.taikhoan_tb WHERE LOWER(LTRIM(RTRIM(ISNULL(taikhoan, '')))) = LOWER(@TaiKhoan))
BEGIN
    UPDATE dbo.taikhoan_tb
    SET
        email = @Email,
        matkhau = @MatKhau,
        permission = N'portal_shop',
        phanloai = N'Shop công ty',
        hoten = ISNULL(NULLIF(hoten, N''), N'Shop công ty'),
        ten = ISNULL(NULLIF(ten, N''), N'Shop công ty'),
        hoten_khongdau = ISNULL(NULLIF(hoten_khongdau, N''), N'Shop cong ty'),
        block = 0,
        nguoitao = ISNULL(NULLIF(nguoitao, N''), @NguoiTao),
        ngaytao = ISNULL(ngaytao, GETDATE())
    WHERE LOWER(LTRIM(RTRIM(ISNULL(taikhoan, '')))) = LOWER(@TaiKhoan);
END
ELSE
BEGIN
    INSERT INTO dbo.taikhoan_tb
    (
        phanloai,
        taikhoan,
        matkhau,
        hoten,
        ten,
        hoten_khongdau,
        email,
        permission,
        block,
        nguoitao,
        ngaytao,
        DongA,
        ChiPhanTram_BanDichVu_ChoSan
    )
    VALUES
    (
        N'Shop công ty',
        @TaiKhoan,
        @MatKhau,
        N'Shop công ty',
        N'Shop công ty',
        N'Shop cong ty',
        @Email,
        N'portal_shop',
        0,
        @NguoiTao,
        GETDATE(),
        0,
        0
    );
END

IF EXISTS (SELECT 1 FROM dbo.DangKy_GianHangDoiTac_tb WHERE taikhoan = @TaiKhoan)
BEGIN
    UPDATE dbo.DangKy_GianHangDoiTac_tb
    SET
        TrangThai = 1,
        GhiChuAdmin = N'Duyệt tự động cho shop công ty',
        NgayDuyet = GETDATE(),
        AdminDuyet = @NguoiTao
    WHERE taikhoan = @TaiKhoan;
END
ELSE
BEGIN
    INSERT INTO dbo.DangKy_GianHangDoiTac_tb
    (
        taikhoan,
        TrangThai,
        NgayTao,
        GhiChuAdmin,
        NgayDuyet,
        AdminDuyet
    )
    VALUES
    (
        @TaiKhoan,
        1,
        GETDATE(),
        N'Duyệt tự động cho shop công ty',
        GETDATE(),
        @NguoiTao
    );
END

SELECT
    taikhoan,
    email,
    matkhau,
    permission,
    phanloai,
    block
FROM dbo.taikhoan_tb
WHERE taikhoan = @TaiKhoan;

SELECT
    taikhoan,
    TrangThai,
    NgayTao,
    NgayDuyet,
    AdminDuyet
FROM dbo.DangKy_GianHangDoiTac_tb
WHERE taikhoan = @TaiKhoan;
