SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @ServiceName NVARCHAR(250) = N'$(SERVICE_NAME)';
DECLARE @ProductName NVARCHAR(250) = N'$(PRODUCT_NAME)';
DECLARE @RuntimeUserAccount NVARCHAR(250) = N'$(RUNTIME_USER_ACCOUNT)';
DECLARE @RuntimeVatTuName NVARCHAR(250) = N'$(RUNTIME_VATTU_NAME)';
DECLARE @RuntimeCustomerName NVARCHAR(250) = N'$(RUNTIME_CUSTOMER_NAME)';
DECLARE @RuntimeCustomerPhone NVARCHAR(250) = N'$(RUNTIME_CUSTOMER_PHONE)';
DECLARE @RuntimeAppointmentNote NVARCHAR(250) = N'$(RUNTIME_APPOINTMENT_NOTE)';
DECLARE @RuntimeInvoiceCustomerName NVARCHAR(250) = N'$(RUNTIME_INVOICE_CUSTOMER_NAME)';
DECLARE @RuntimeInvoicePhone NVARCHAR(250) = N'$(RUNTIME_INVOICE_PHONE)';
DECLARE @RuntimeImportNote NVARCHAR(250) = N'$(RUNTIME_IMPORT_NOTE)';
DECLARE @RuntimeIncludeInvoiceDetail NVARCHAR(10) = N'$(RUNTIME_INCLUDE_INVOICE_DETAIL)';
DECLARE @RuntimeIncludeWarehouse NVARCHAR(10) = N'$(RUNTIME_INCLUDE_WAREHOUSE)';

IF @ServiceName IS NULL OR LTRIM(RTRIM(@ServiceName)) = N''
BEGIN
    THROW 50000, 'SERVICE_NAME is required', 1;
END

IF @ProductName IS NULL OR LTRIM(RTRIM(@ProductName)) = N''
BEGIN
    THROW 50001, 'PRODUCT_NAME is required', 1;
END

IF @RuntimeUserAccount IS NULL OR LTRIM(RTRIM(@RuntimeUserAccount)) = N''
BEGIN
    THROW 50006, 'RUNTIME_USER_ACCOUNT is required', 1;
END

IF @RuntimeVatTuName IS NULL OR LTRIM(RTRIM(@RuntimeVatTuName)) = N''
BEGIN
    THROW 50007, 'RUNTIME_VATTU_NAME is required', 1;
END

IF @RuntimeCustomerName IS NULL OR LTRIM(RTRIM(@RuntimeCustomerName)) = N''
BEGIN
    THROW 50010, 'RUNTIME_CUSTOMER_NAME is required', 1;
END

IF @RuntimeCustomerPhone IS NULL OR LTRIM(RTRIM(@RuntimeCustomerPhone)) = N''
BEGIN
    THROW 50011, 'RUNTIME_CUSTOMER_PHONE is required', 1;
END

IF @RuntimeAppointmentNote IS NULL OR LTRIM(RTRIM(@RuntimeAppointmentNote)) = N''
BEGIN
    THROW 50017, 'RUNTIME_APPOINTMENT_NOTE is required', 1;
END

IF @RuntimeInvoiceCustomerName IS NULL OR LTRIM(RTRIM(@RuntimeInvoiceCustomerName)) = N''
BEGIN
    THROW 50012, 'RUNTIME_INVOICE_CUSTOMER_NAME is required', 1;
END

IF @RuntimeInvoicePhone IS NULL OR LTRIM(RTRIM(@RuntimeInvoicePhone)) = N''
BEGIN
    THROW 50013, 'RUNTIME_INVOICE_PHONE is required', 1;
END

IF @RuntimeImportNote IS NULL OR LTRIM(RTRIM(@RuntimeImportNote)) = N''
BEGIN
    THROW 50018, 'RUNTIME_IMPORT_NOTE is required', 1;
END

IF @RuntimeIncludeWarehouse IS NULL OR LTRIM(RTRIM(@RuntimeIncludeWarehouse)) = N''
BEGIN
    SET @RuntimeIncludeWarehouse = N'0';
END

IF @RuntimeIncludeInvoiceDetail IS NULL OR LTRIM(RTRIM(@RuntimeIncludeInvoiceDetail)) = N''
BEGIN
    SET @RuntimeIncludeInvoiceDetail = N'0';
END

IF NOT EXISTS (
    SELECT 1
    FROM dbo.web_post_table
    WHERE name = @ServiceName
      AND phanloai = N'ctdv'
      AND id_baiviet IS NOT NULL
      AND ISNULL(thoiluong_dichvu_phut, 0) = 90
)
BEGIN
    THROW 50002, 'Service runtime post was not stored correctly in web_post_table.', 1;
END

IF NOT EXISTS (
    SELECT 1
    FROM dbo.web_post_table
    WHERE name = @ProductName
      AND phanloai = N'ctsp'
      AND id_baiviet IS NOT NULL
      AND ISNULL(giaban_sanpham, 0) = 350000
      AND ISNULL(giavon_sanpham, 0) = 200000
)
BEGIN
    THROW 50003, 'Product runtime post was not stored correctly in web_post_table.', 1;
END

IF NOT EXISTS (
    SELECT 1
    FROM dbo.BaiViet_tb bv
    INNER JOIN dbo.web_post_table wp ON wp.id_baiviet = bv.id
    WHERE wp.name = @ServiceName
      AND bv.phanloai = N'dichvu'
      AND ISNULL(bv.giaban, 0) = 250000
)
BEGIN
    THROW 50004, 'Service runtime post was not synced to BaiViet_tb.', 1;
END

IF NOT EXISTS (
    SELECT 1
    FROM dbo.BaiViet_tb bv
    INNER JOIN dbo.web_post_table wp ON wp.id_baiviet = bv.id
    WHERE wp.name = @ProductName
      AND bv.phanloai = N'sanpham'
      AND ISNULL(bv.giaban, 0) = 350000
      AND ISNULL(bv.giavon, 0) = 200000
)
BEGIN
    THROW 50005, 'Product runtime post was not synced to BaiViet_tb.', 1;
END

IF NOT EXISTS (
    SELECT 1
    FROM dbo.taikhoan_table_2023
    WHERE taikhoan = @RuntimeUserAccount
      AND trangthai = N'Đang hoạt động'
      AND ISNULL(user_parent, N'') <> N''
      AND ISNULL(luongcoban, 0) = 12000000
      AND ISNULL(songaycong, 0) = 26
      AND ISNULL(luongngay, 0) = 461538
)
BEGIN
    THROW 50008, 'Runtime staff account was not stored correctly in taikhoan_table_2023.', 1;
END

IF NOT EXISTS (
    SELECT 1
    FROM dbo.danhsach_vattu_table
    WHERE tenvattu = @RuntimeVatTuName
      AND ISNULL(giaban, 0) = 180000
      AND ISNULL(gianhap, 0) = 90000
      AND ISNULL(donvitinh_sp, N'') = N'hop'
      AND ISNULL(tinhtrang, N'') = N'Mua'
)
BEGIN
    THROW 50009, 'Runtime supply item was not stored correctly in danhsach_vattu_table.', 1;
END

IF NOT EXISTS (
    SELECT 1
    FROM dbo.bspa_data_khachhang_table
    WHERE sdt = @RuntimeCustomerPhone
      AND tenkhachhang = @RuntimeCustomerName
      AND ISNULL(diachi, N'') = N'Runtime customer address'
      AND ISNULL(user_parent, N'') <> N''
)
BEGIN
    THROW 50014, 'Runtime customer was not stored correctly in bspa_data_khachhang_table.', 1;
END

IF NOT EXISTS (
    SELECT 1
    FROM dbo.bspa_datlich_table
    WHERE sdt = @RuntimeCustomerPhone
      AND tenkhachhang = @RuntimeCustomerName
      AND ISNULL(ghichu, N'') = @RuntimeAppointmentNote
      AND tendichvu_taithoidiemnay = @ServiceName
)
BEGIN
    THROW 50019, 'Runtime appointment was not stored correctly in bspa_datlich_table.', 1;
END

IF NOT EXISTS (
    SELECT 1
    FROM dbo.bspa_hoadon_table
    WHERE sdt = @RuntimeInvoicePhone
      AND tenkhachhang = @RuntimeInvoiceCustomerName
      AND ISNULL(diachi, N'') = N'Runtime invoice address'
      AND ISNULL(ghichu, N'') = N'Runtime invoice ' + @RuntimeInvoicePhone
      AND ISNULL(nguongoc, N'') = N'App'
      AND ISNULL(tongtien, 0) = 0
)
BEGIN
    THROW 50015, 'Runtime invoice was not stored correctly in bspa_hoadon_table.', 1;
END

IF NOT EXISTS (
    SELECT 1
    FROM dbo.bspa_data_khachhang_table
    WHERE sdt = @RuntimeInvoicePhone
      AND tenkhachhang = @RuntimeInvoiceCustomerName
)
BEGIN
    THROW 50016, 'Runtime invoice did not backfill customer data in bspa_data_khachhang_table.', 1;
END

IF @RuntimeIncludeWarehouse = N'1'
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.donnhaphang_table d
        INNER JOIN dbo.donnhaphang_chitiet_table c ON c.id_hoadon = CONVERT(NVARCHAR(50), d.id)
        WHERE ISNULL(d.ghichu, N'') = @RuntimeImportNote
          AND c.ten_dvsp_taithoidiemnay = @ProductName
          AND ISNULL(c.gia_dvsp_taithoidiemnay, 0) = 200000
          AND ISNULL(c.soluong, 0) = 3
          AND ISNULL(c.dvt, N'') = N'chai'
    )
    BEGIN
        THROW 50020, 'Runtime stock import was not stored correctly in donnhaphang_table / donnhaphang_chitiet_table.', 1;
    END
END

IF @RuntimeIncludeInvoiceDetail = N'1'
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.bspa_hoadon_chitiet_table c
        INNER JOIN dbo.bspa_hoadon_table h ON h.id = TRY_CONVERT(BIGINT, c.id_hoadon)
        WHERE h.sdt = @RuntimeInvoicePhone
          AND c.kyhieu = N'dichvu'
          AND c.ten_dvsp_taithoidiemnay = @ServiceName
          AND ISNULL(c.gia_dvsp_taithoidiemnay, 0) = 250000
          AND ISNULL(c.soluong, 0) = 1
    )
    BEGIN
        THROW 50021, 'Runtime service line was not stored correctly in bspa_hoadon_chitiet_table.', 1;
    END

    IF @RuntimeIncludeWarehouse = N'1'
    BEGIN
        IF NOT EXISTS (
            SELECT 1
            FROM dbo.bspa_hoadon_chitiet_table c
            INNER JOIN dbo.bspa_hoadon_table h ON h.id = TRY_CONVERT(BIGINT, c.id_hoadon)
            WHERE h.sdt = @RuntimeInvoicePhone
              AND c.kyhieu = N'sanpham'
              AND c.ten_dvsp_taithoidiemnay = @ProductName
              AND ISNULL(c.gia_dvsp_taithoidiemnay, 0) = 350000
              AND ISNULL(c.soluong, 0) = 1
        )
        BEGIN
            THROW 50022, 'Runtime product line was not stored correctly in bspa_hoadon_chitiet_table.', 1;
        END

        IF NOT EXISTS (
            SELECT 1
            FROM dbo.bspa_hoadon_table
            WHERE sdt = @RuntimeInvoicePhone
              AND ISNULL(tongtien, 0) = 600000
              AND ISNULL(tongsauchietkhau, 0) = 600000
              AND ISNULL(sl_dichvu, 0) = 1
              AND ISNULL(sl_sanpham, 0) = 1
        )
        BEGIN
            THROW 50023, 'Runtime invoice totals were not updated correctly after adding service/product.', 1;
        END
    END
    ELSE
    BEGIN
        IF NOT EXISTS (
            SELECT 1
            FROM dbo.bspa_hoadon_table
            WHERE sdt = @RuntimeInvoicePhone
              AND ISNULL(tongtien, 0) = 250000
              AND ISNULL(tongsauchietkhau, 0) = 250000
              AND ISNULL(sl_dichvu, 0) = 1
              AND ISNULL(sl_sanpham, 0) = 0
        )
        BEGIN
            THROW 50024, 'Runtime invoice totals were not updated correctly after adding the service line.', 1;
        END
    END
END

SELECT
    N'OK' AS status,
    @ServiceName AS service_name,
    @ProductName AS product_name,
    @RuntimeUserAccount AS runtime_user_account,
    @RuntimeVatTuName AS runtime_vattu_name,
    @RuntimeCustomerPhone AS runtime_customer_phone,
    @RuntimeAppointmentNote AS runtime_appointment_note,
    @RuntimeInvoicePhone AS runtime_invoice_phone,
    @RuntimeImportNote AS runtime_import_note,
    @RuntimeIncludeInvoiceDetail AS runtime_include_invoice_detail,
    @RuntimeIncludeWarehouse AS runtime_include_warehouse;
