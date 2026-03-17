SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;

DECLARE @ShopTaiKhoan NVARCHAR(100) = '$(SHOP_TK)';
DECLARE @ProductName NVARCHAR(200) = N'$(PRODUCT_NAME)';
DECLARE @InvoiceName NVARCHAR(200) = N'$(INVOICE_NAME)';
DECLARE @InitStock INT = $(INIT_STOCK);
DECLARE @Qty INT = $(INVOICE_QTY);

IF @ShopTaiKhoan IS NULL OR LTRIM(RTRIM(@ShopTaiKhoan)) = ''
BEGIN
    THROW 50000, 'SHOP_TK is required', 1;
END
IF @ProductName IS NULL OR LTRIM(RTRIM(@ProductName)) = ''
BEGIN
    THROW 50001, 'PRODUCT_NAME is required', 1;
END
IF @InvoiceName IS NULL OR LTRIM(RTRIM(@InvoiceName)) = ''
BEGIN
    THROW 50002, 'INVOICE_NAME is required', 1;
END
IF @Qty IS NULL OR @Qty <= 0
BEGIN
    THROW 50003, 'INVOICE_QTY is required', 1;
END

IF OBJECT_ID('dbo.GH_SanPham_tb', 'U') IS NULL
BEGIN
    THROW 50010, 'Missing GH_SanPham_tb', 1;
END
IF OBJECT_ID('dbo.GH_HoaDon_tb', 'U') IS NULL
BEGIN
    THROW 50011, 'Missing GH_HoaDon_tb', 1;
END
IF OBJECT_ID('dbo.GH_HoaDon_ChiTiet_tb', 'U') IS NULL
BEGIN
    THROW 50012, 'Missing GH_HoaDon_ChiTiet_tb', 1;
END

DECLARE @GhId INT = NULL;
SELECT TOP 1 @GhId = id
FROM dbo.GH_SanPham_tb
WHERE shop_taikhoan = @ShopTaiKhoan
  AND ten = @ProductName
ORDER BY id DESC;

IF @GhId IS NULL
BEGIN
    THROW 50020, 'GH product not found', 1;
END

DECLARE @BvId INT = NULL;
SELECT @BvId = id_baiviet
FROM dbo.GH_SanPham_tb
WHERE id = @GhId;

IF @BvId IS NULL
BEGIN
    THROW 50021, 'BaiViet not synced for GH product', 1;
END

IF NOT EXISTS (
    SELECT 1 FROM dbo.BaiViet_tb
    WHERE id = @BvId
      AND nguoitao = @ShopTaiKhoan
)
BEGIN
    THROW 50022, 'BaiViet row missing for GH product', 1;
END

DECLARE @HdId BIGINT = NULL;
SELECT TOP 1 @HdId = id
FROM dbo.GH_HoaDon_tb
WHERE shop_taikhoan = @ShopTaiKhoan
  AND ten_khach = @InvoiceName
ORDER BY id DESC;

IF @HdId IS NULL
BEGIN
    THROW 50030, 'Invoice not found', 1;
END

IF NOT EXISTS (
    SELECT 1
    FROM dbo.GH_HoaDon_ChiTiet_tb
    WHERE id_hoadon = @HdId
      AND id_sanpham = @GhId
      AND so_luong = @Qty
)
BEGIN
    THROW 50031, 'Invoice detail not found', 1;
END

DECLARE @Stock INT = NULL;
SELECT @Stock = so_luong_ton FROM dbo.GH_SanPham_tb WHERE id = @GhId;

IF @InitStock > 0 AND @Stock <> (@InitStock - @Qty)
BEGIN
    THROW 50032, 'Stock not decremented as expected', 1;
END

SELECT
    ok = 1,
    gh_id = @GhId,
    bv_id = @BvId,
    hd_id = @HdId,
    stock = @Stock;
