SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @ShopTaiKhoan NVARCHAR(100) = '$(SHOP_TK)';
DECLARE @OrderName NVARCHAR(200) = '$(ORDER_NAME)';
DECLARE @OrderPhone NVARCHAR(50) = '$(ORDER_PHONE)';
DECLARE @OrderQty INT = $(ORDER_QTY);

IF @ShopTaiKhoan IS NULL OR LTRIM(RTRIM(@ShopTaiKhoan)) = ''
BEGIN
    THROW 50000, 'SHOP_TK is required', 1;
END

DECLARE @InvoiceId BIGINT;
SELECT TOP 1 @InvoiceId = id
FROM dbo.GH_HoaDon_tb
WHERE shop_taikhoan = @ShopTaiKhoan
  AND ten_khach = @OrderName
  AND sdt = @OrderPhone
ORDER BY id DESC;

IF @InvoiceId IS NULL
BEGIN
    THROW 50000, 'Invoice not found for cart order', 1;
END

DECLARE @TotalQty INT = (SELECT SUM(ISNULL(so_luong, 0)) FROM dbo.GH_HoaDon_ChiTiet_tb WHERE id_hoadon = @InvoiceId);
IF @TotalQty IS NULL OR @TotalQty <> @OrderQty
BEGIN
    THROW 50000, 'Invoice quantity mismatch', 1;
END

DECLARE @DetailTotal DECIMAL(18,2) = (SELECT SUM(ISNULL(thanh_tien, 0)) FROM dbo.GH_HoaDon_ChiTiet_tb WHERE id_hoadon = @InvoiceId);
DECLARE @InvoiceTotal DECIMAL(18,2) = (SELECT tong_tien FROM dbo.GH_HoaDon_tb WHERE id = @InvoiceId);

IF @DetailTotal IS NULL OR @InvoiceTotal IS NULL OR ABS(@DetailTotal - @InvoiceTotal) > 0.01
BEGIN
    THROW 50000, 'Invoice total mismatch', 1;
END

PRINT 'Cart invoice validation passed';
