SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;

DECLARE @ShopTaiKhoan NVARCHAR(100) = '$(SHOP_TK)';
DECLARE @TenKhach NVARCHAR(200) = N'$(BOOKING_NAME)';

IF @ShopTaiKhoan IS NULL OR LTRIM(RTRIM(@ShopTaiKhoan)) = ''
BEGIN
    THROW 50000, 'SHOP_TK is required', 1;
END

IF @TenKhach IS NULL OR LTRIM(RTRIM(@TenKhach)) = ''
BEGIN
    THROW 50001, 'BOOKING_NAME is required', 1;
END

IF OBJECT_ID('dbo.GH_DatLich_tb', 'U') IS NULL
BEGIN
    THROW 50002, 'Missing GH_DatLich_tb', 1;
END

IF NOT EXISTS (
    SELECT 1
    FROM dbo.GH_DatLich_tb
    WHERE shop_taikhoan = @ShopTaiKhoan
      AND ten_khach = @TenKhach
)
BEGIN
    THROW 50003, 'Booking row not found', 1;
END

SELECT TOP 1
    ok = 1,
    id,
    shop_taikhoan,
    ten_khach,
    sdt,
    dich_vu,
    trang_thai,
    ngay_tao
FROM dbo.GH_DatLich_tb
WHERE shop_taikhoan = @ShopTaiKhoan
  AND ten_khach = @TenKhach
ORDER BY id DESC;
