using System;

public static class GianHangSchema_cl
{
    private static bool _schemaReady;
    private static readonly object _schemaLock = new object();

    public static void EnsureSchemaSafe(dbDataContext db)
    {
        if (db == null)
            return;
        if (_schemaReady)
            return;

        lock (_schemaLock)
        {
            if (_schemaReady)
                return;

            try
            {
                db.ExecuteCommand(@"
IF OBJECT_ID('dbo.GH_SanPham_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.GH_SanPham_tb
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        shop_taikhoan NVARCHAR(100) NOT NULL,
        ten NVARCHAR(MAX) NULL,
        mo_ta NVARCHAR(MAX) NULL,
        noi_dung NVARCHAR(MAX) NULL,
        hinh_anh NVARCHAR(MAX) NULL,
        gia_ban DECIMAL(18,2) NULL,
        gia_von BIGINT NULL,
        loai NVARCHAR(20) NULL,
        id_danhmuc NVARCHAR(50) NULL,
        bin BIT NULL,
        ngay_tao DATETIME NULL,
        ngay_cap_nhat DATETIME NULL,
        id_baiviet INT NULL,
        so_luong_ton INT NULL,
        so_luong_da_ban INT NULL,
        luot_truy_cap INT NULL,
        phan_tram_uu_dai INT NULL
    );
END

IF COL_LENGTH('dbo.GH_SanPham_tb', 'id_baiviet') IS NULL
BEGIN
    ALTER TABLE dbo.GH_SanPham_tb ADD id_baiviet INT NULL;
END

IF COL_LENGTH('dbo.GH_SanPham_tb', 'so_luong_ton') IS NULL
BEGIN
    ALTER TABLE dbo.GH_SanPham_tb ADD so_luong_ton INT NULL;
END

IF COL_LENGTH('dbo.GH_SanPham_tb', 'so_luong_da_ban') IS NULL
BEGIN
    ALTER TABLE dbo.GH_SanPham_tb ADD so_luong_da_ban INT NULL;
END

IF COL_LENGTH('dbo.GH_SanPham_tb', 'luot_truy_cap') IS NULL
BEGIN
    ALTER TABLE dbo.GH_SanPham_tb ADD luot_truy_cap INT NULL;
END

IF COL_LENGTH('dbo.GH_SanPham_tb', 'phan_tram_uu_dai') IS NULL
BEGIN
    ALTER TABLE dbo.GH_SanPham_tb ADD phan_tram_uu_dai INT NULL;
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_GH_SanPham_shop' AND object_id = OBJECT_ID('dbo.GH_SanPham_tb')
)
BEGIN
    CREATE INDEX IX_GH_SanPham_shop ON dbo.GH_SanPham_tb(shop_taikhoan, bin, loai);
END

IF OBJECT_ID('dbo.GH_BaiViet_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.GH_BaiViet_tb
    (
        id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        legacy_post_id INT NULL,
        shop_taikhoan NVARCHAR(100) NULL,
        id_chinhanh NVARCHAR(50) NULL,
        id_category NVARCHAR(50) NULL,
        name NVARCHAR(MAX) NULL,
        name_en NVARCHAR(MAX) NULL,
        description NVARCHAR(MAX) NULL,
        content_post NVARCHAR(MAX) NULL,
        image NVARCHAR(MAX) NULL,
        phanloai NVARCHAR(20) NULL,
        noibat BIT NULL,
        hienthi BIT NULL,
        bin BIT NULL,
        ngaytao DATETIME NULL,
        ngay_cap_nhat DATETIME NULL
    );
END

IF COL_LENGTH('dbo.GH_BaiViet_tb', 'legacy_post_id') IS NULL
BEGIN
    ALTER TABLE dbo.GH_BaiViet_tb ADD legacy_post_id INT NULL;
END

IF COL_LENGTH('dbo.GH_BaiViet_tb', 'shop_taikhoan') IS NULL
BEGIN
    ALTER TABLE dbo.GH_BaiViet_tb ADD shop_taikhoan NVARCHAR(100) NULL;
END

IF COL_LENGTH('dbo.GH_BaiViet_tb', 'id_chinhanh') IS NULL
BEGIN
    ALTER TABLE dbo.GH_BaiViet_tb ADD id_chinhanh NVARCHAR(50) NULL;
END

IF COL_LENGTH('dbo.GH_BaiViet_tb', 'id_category') IS NULL
BEGIN
    ALTER TABLE dbo.GH_BaiViet_tb ADD id_category NVARCHAR(50) NULL;
END

IF COL_LENGTH('dbo.GH_BaiViet_tb', 'name') IS NULL
BEGIN
    ALTER TABLE dbo.GH_BaiViet_tb ADD name NVARCHAR(MAX) NULL;
END

IF COL_LENGTH('dbo.GH_BaiViet_tb', 'name_en') IS NULL
BEGIN
    ALTER TABLE dbo.GH_BaiViet_tb ADD name_en NVARCHAR(MAX) NULL;
END

IF COL_LENGTH('dbo.GH_BaiViet_tb', 'description') IS NULL
BEGIN
    ALTER TABLE dbo.GH_BaiViet_tb ADD description NVARCHAR(MAX) NULL;
END

IF COL_LENGTH('dbo.GH_BaiViet_tb', 'content_post') IS NULL
BEGIN
    ALTER TABLE dbo.GH_BaiViet_tb ADD content_post NVARCHAR(MAX) NULL;
END

IF COL_LENGTH('dbo.GH_BaiViet_tb', 'image') IS NULL
BEGIN
    ALTER TABLE dbo.GH_BaiViet_tb ADD image NVARCHAR(MAX) NULL;
END

IF COL_LENGTH('dbo.GH_BaiViet_tb', 'phanloai') IS NULL
BEGIN
    ALTER TABLE dbo.GH_BaiViet_tb ADD phanloai NVARCHAR(20) NULL;
END

IF COL_LENGTH('dbo.GH_BaiViet_tb', 'noibat') IS NULL
BEGIN
    ALTER TABLE dbo.GH_BaiViet_tb ADD noibat BIT NULL;
END

IF COL_LENGTH('dbo.GH_BaiViet_tb', 'hienthi') IS NULL
BEGIN
    ALTER TABLE dbo.GH_BaiViet_tb ADD hienthi BIT NULL;
END

IF COL_LENGTH('dbo.GH_BaiViet_tb', 'bin') IS NULL
BEGIN
    ALTER TABLE dbo.GH_BaiViet_tb ADD bin BIT NULL;
END

IF COL_LENGTH('dbo.GH_BaiViet_tb', 'ngaytao') IS NULL
BEGIN
    ALTER TABLE dbo.GH_BaiViet_tb ADD ngaytao DATETIME NULL;
END

IF COL_LENGTH('dbo.GH_BaiViet_tb', 'ngay_cap_nhat') IS NULL
BEGIN
    ALTER TABLE dbo.GH_BaiViet_tb ADD ngay_cap_nhat DATETIME NULL;
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_GH_BaiViet_chinhanh' AND object_id = OBJECT_ID('dbo.GH_BaiViet_tb')
)
BEGIN
    CREATE INDEX IX_GH_BaiViet_chinhanh ON dbo.GH_BaiViet_tb(id_chinhanh, bin, hienthi, id_category, ngaytao);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_GH_BaiViet_legacy' AND object_id = OBJECT_ID('dbo.GH_BaiViet_tb')
)
BEGIN
    CREATE INDEX IX_GH_BaiViet_legacy ON dbo.GH_BaiViet_tb(legacy_post_id);
END

IF OBJECT_ID('dbo.GH_DatLich_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.GH_DatLich_tb
    (
        id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        shop_taikhoan NVARCHAR(100) NOT NULL,
        ten_khach NVARCHAR(200) NULL,
        sdt NVARCHAR(30) NULL,
        dich_vu NVARCHAR(200) NULL,
        id_dichvu INT NULL,
        home_post_id INT NULL,
        id_danhmuc NVARCHAR(50) NULL,
        thoi_gian_hen DATETIME NULL,
        ghi_chu NVARCHAR(MAX) NULL,
        trang_thai NVARCHAR(30) NULL,
        ngay_tao DATETIME NULL
    );
END

IF COL_LENGTH('dbo.GH_DatLich_tb', 'id_dichvu') IS NULL
BEGIN
    ALTER TABLE dbo.GH_DatLich_tb ADD id_dichvu INT NULL;
END

IF COL_LENGTH('dbo.GH_DatLich_tb', 'home_post_id') IS NULL
BEGIN
    ALTER TABLE dbo.GH_DatLich_tb ADD home_post_id INT NULL;
END

IF COL_LENGTH('dbo.GH_DatLich_tb', 'id_danhmuc') IS NULL
BEGIN
    ALTER TABLE dbo.GH_DatLich_tb ADD id_danhmuc NVARCHAR(50) NULL;
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_GH_DatLich_shop' AND object_id = OBJECT_ID('dbo.GH_DatLich_tb')
)
BEGIN
    CREATE INDEX IX_GH_DatLich_shop ON dbo.GH_DatLich_tb(shop_taikhoan, trang_thai, thoi_gian_hen);
END

IF OBJECT_ID('dbo.GH_HoaDon_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.GH_HoaDon_tb
    (
        id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        id_guide UNIQUEIDENTIFIER NULL,
        shop_taikhoan NVARCHAR(100) NOT NULL,
        id_donhang NVARCHAR(50) NULL,
        buyer_account NVARCHAR(100) NULL,
        ten_khach NVARCHAR(200) NULL,
        sdt NVARCHAR(30) NULL,
        dia_chi NVARCHAR(MAX) NULL,
        ghi_chu NVARCHAR(MAX) NULL,
        tong_tien DECIMAL(18,2) NULL,
        trang_thai NVARCHAR(30) NULL,
        order_status NVARCHAR(50) NULL,
        exchange_status NVARCHAR(50) NULL,
        is_offline BIT NULL,
        ngay_tao DATETIME NULL
    );
END

IF COL_LENGTH('dbo.GH_HoaDon_tb', 'id_guide') IS NULL
BEGIN
    ALTER TABLE dbo.GH_HoaDon_tb ADD id_guide UNIQUEIDENTIFIER NULL;
END

IF COL_LENGTH('dbo.GH_HoaDon_tb', 'id_donhang') IS NULL
BEGIN
    ALTER TABLE dbo.GH_HoaDon_tb ADD id_donhang NVARCHAR(50) NULL;
END

IF COL_LENGTH('dbo.GH_HoaDon_tb', 'buyer_account') IS NULL
BEGIN
    ALTER TABLE dbo.GH_HoaDon_tb ADD buyer_account NVARCHAR(100) NULL;
END

IF COL_LENGTH('dbo.GH_HoaDon_tb', 'order_status') IS NULL
BEGIN
    ALTER TABLE dbo.GH_HoaDon_tb ADD order_status NVARCHAR(50) NULL;
END

IF COL_LENGTH('dbo.GH_HoaDon_tb', 'exchange_status') IS NULL
BEGIN
    ALTER TABLE dbo.GH_HoaDon_tb ADD exchange_status NVARCHAR(50) NULL;
END

IF COL_LENGTH('dbo.GH_HoaDon_tb', 'is_offline') IS NULL
BEGIN
    ALTER TABLE dbo.GH_HoaDon_tb ADD is_offline BIT NULL;
END

IF OBJECT_ID('dbo.GH_HoaDon_ChiTiet_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.GH_HoaDon_ChiTiet_tb
    (
        id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        id_hoadon BIGINT NOT NULL,
        id_sanpham INT NULL,
        ten_sanpham NVARCHAR(MAX) NULL,
        hinh_anh NVARCHAR(MAX) NULL,
        loai NVARCHAR(30) NULL,
        so_luong INT NULL,
        gia DECIMAL(18,2) NULL,
        thanh_tien DECIMAL(18,2) NULL,
        phan_tram_uu_dai INT NULL
    );
END

IF COL_LENGTH('dbo.GH_HoaDon_ChiTiet_tb', 'hinh_anh') IS NULL
BEGIN
    ALTER TABLE dbo.GH_HoaDon_ChiTiet_tb ADD hinh_anh NVARCHAR(MAX) NULL;
END

IF COL_LENGTH('dbo.GH_HoaDon_ChiTiet_tb', 'loai') IS NULL
BEGIN
    ALTER TABLE dbo.GH_HoaDon_ChiTiet_tb ADD loai NVARCHAR(30) NULL;
END

IF COL_LENGTH('dbo.GH_HoaDon_ChiTiet_tb', 'phan_tram_uu_dai') IS NULL
BEGIN
    ALTER TABLE dbo.GH_HoaDon_ChiTiet_tb ADD phan_tram_uu_dai INT NULL;
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_GH_HoaDon_shop' AND object_id = OBJECT_ID('dbo.GH_HoaDon_tb')
)
BEGIN
    CREATE INDEX IX_GH_HoaDon_shop ON dbo.GH_HoaDon_tb(shop_taikhoan, ngay_tao, trang_thai);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_GH_HoaDon_guide' AND object_id = OBJECT_ID('dbo.GH_HoaDon_tb')
)
BEGIN
    CREATE INDEX IX_GH_HoaDon_guide ON dbo.GH_HoaDon_tb(id_guide);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_GH_HoaDonCT_hoadon' AND object_id = OBJECT_ID('dbo.GH_HoaDon_ChiTiet_tb')
)
BEGIN
    CREATE INDEX IX_GH_HoaDonCT_hoadon ON dbo.GH_HoaDon_ChiTiet_tb(id_hoadon);
END

IF COL_LENGTH('dbo.DonHang_ChiTiet_tb', 'gh_product_id') IS NULL
BEGIN
    ALTER TABLE dbo.DonHang_ChiTiet_tb ADD gh_product_id INT NULL;
END

IF COL_LENGTH('dbo.DonHang_ChiTiet_tb', 'gh_name') IS NULL
BEGIN
    ALTER TABLE dbo.DonHang_ChiTiet_tb ADD gh_name NVARCHAR(MAX) NULL;
END

IF COL_LENGTH('dbo.DonHang_ChiTiet_tb', 'gh_image') IS NULL
BEGIN
    ALTER TABLE dbo.DonHang_ChiTiet_tb ADD gh_image NVARCHAR(MAX) NULL;
END

IF COL_LENGTH('dbo.DonHang_ChiTiet_tb', 'gh_type') IS NULL
BEGIN
    ALTER TABLE dbo.DonHang_ChiTiet_tb ADD gh_type NVARCHAR(30) NULL;
END
");

                _schemaReady = true;
            }
            catch
            {
                // Nếu lỗi schema thì để lần sau thử lại.
            }
        }
    }
}
