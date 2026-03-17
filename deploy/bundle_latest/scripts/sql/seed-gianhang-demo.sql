SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;

DECLARE @Now DATETIME = GETDATE();
DECLARE @ShopTaiKhoan NVARCHAR(100) = '$(SHOP_TK)';
DECLARE @ShopMatKhau NVARCHAR(100) = '$(SHOP_PW)';
DECLARE @ShopEmail NVARCHAR(200) = '$(SHOP_EMAIL)';
DECLARE @ShopTen NVARCHAR(200) = N'Gian hàng demo';
DECLARE @ShopSlug NVARCHAR(120) = N'gian-hang-demo';

IF @ShopTaiKhoan IS NULL OR LTRIM(RTRIM(@ShopTaiKhoan)) = ''
BEGIN
    THROW 50000, 'SHOP_TK is required', 1;
END

IF @ShopMatKhau IS NULL OR LTRIM(RTRIM(@ShopMatKhau)) = ''
BEGIN
    SET @ShopMatKhau = '123123';
END

IF @ShopEmail IS NULL OR LTRIM(RTRIM(@ShopEmail)) = ''
BEGIN
    SET @ShopEmail = 'demo@ahasale.local';
END

IF COL_LENGTH('dbo.taikhoan_tb', 'TrangThai_Shop') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_tb
        ADD TrangThai_Shop TINYINT NOT NULL
        CONSTRAINT DF_taikhoan_tb_TrangThai_Shop DEFAULT(0) WITH VALUES;
END

IF NOT EXISTS (SELECT 1 FROM dbo.taikhoan_tb WHERE taikhoan = @ShopTaiKhoan)
BEGIN
    INSERT INTO dbo.taikhoan_tb
    (
        taikhoan, matkhau, hoten, ten, hoten_khongdau, phanloai,
        permission, block, nguoitao, ngaytao, hansudung, DongA,
        anhdaidien, TrangThai_Shop,
        email,
        ten_shop, sdt_shop, email_shop, motangan_shop, diachi_shop
    )
    VALUES
    (
        @ShopTaiKhoan, @ShopMatKhau, @ShopTen, @ShopTen, @ShopSlug, N'Gian hàng đối tác',
        N'portal_shop', 0, N'system_seed', @Now, DATEADD(YEAR, 10, @Now), 0,
        N'/uploads/images/macdinh.jpg', 1,
        @ShopEmail,
        @ShopTen, N'0900000000', N'demo@ahasale.local', N'Demo gian hàng đồng bộ từ ahashine', N'AhaSale local'
    );
END
ELSE
BEGIN
    UPDATE dbo.taikhoan_tb
    SET
        matkhau = @ShopMatKhau,
        phanloai = N'Gian hàng đối tác',
        permission = N'portal_shop',
        block = 0,
        TrangThai_Shop = 1,
        email = ISNULL(NULLIF(email, ''), @ShopEmail),
        ten_shop = ISNULL(NULLIF(ten_shop, ''), @ShopTen),
        sdt_shop = ISNULL(NULLIF(sdt_shop, ''), N'0900000000'),
        email_shop = ISNULL(NULLIF(email_shop, ''), N'demo@ahasale.local'),
        motangan_shop = ISNULL(NULLIF(motangan_shop, ''), N'Demo gian hàng đồng bộ từ ahashine'),
        diachi_shop = ISNULL(NULLIF(diachi_shop, ''), N'AhaSale local')
    WHERE taikhoan = @ShopTaiKhoan;
END

IF COL_LENGTH('dbo.taikhoan_tb', 'slug_shop') IS NOT NULL
BEGIN
    UPDATE dbo.taikhoan_tb
    SET slug_shop = @ShopSlug
    WHERE taikhoan = @ShopTaiKhoan;
END

DECLARE @DanhMucId NVARCHAR(50) = NULL;
SELECT TOP 1 @DanhMucId = CAST(id AS NVARCHAR(50))
FROM dbo.DanhMuc_tb
WHERE LOWER(LTRIM(RTRIM(ISNULL(kyhieu_danhmuc, '')))) = N'gianhang'
  AND (bin = 0 OR bin IS NULL);

IF @DanhMucId IS NULL
BEGIN
    INSERT INTO dbo.DanhMuc_tb
    (
        name, name_en, kyhieu_danhmuc,
        bin, rank, id_level, nguoitao, ngaytao
    )
    VALUES
    (
        N'Gian hàng', N'gian-hang', N'gianhang',
        0, 1, 1, N'system_seed', @Now
    );

    SET @DanhMucId = CAST(SCOPE_IDENTITY() AS NVARCHAR(50));
END

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
        luot_truy_cap INT NULL
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

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_GH_SanPham_shop' AND object_id = OBJECT_ID('dbo.GH_SanPham_tb')
)
BEGIN
    CREATE INDEX IX_GH_SanPham_shop ON dbo.GH_SanPham_tb(shop_taikhoan, bin, loai);
END

IF NOT EXISTS (SELECT 1 FROM dbo.GH_SanPham_tb WHERE shop_taikhoan = @ShopTaiKhoan AND ten = N'Sản phẩm demo')
BEGIN
    INSERT INTO dbo.GH_SanPham_tb
    (
        shop_taikhoan, ten, mo_ta, noi_dung, hinh_anh,
        gia_ban, gia_von, loai, id_danhmuc, bin,
        ngay_tao, ngay_cap_nhat, so_luong_ton, so_luong_da_ban, luot_truy_cap
    )
    VALUES
    (
        @ShopTaiKhoan, N'Sản phẩm demo', N'Mô tả ngắn sản phẩm demo', N'Nội dung chi tiết sản phẩm demo',
        N'/uploads/images/macdinh.jpg',
        120000, 80000, N'sanpham', @DanhMucId, 0,
        @Now, @Now, 25, 0, 0
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.GH_SanPham_tb WHERE shop_taikhoan = @ShopTaiKhoan AND ten = N'Dịch vụ demo')
BEGIN
    INSERT INTO dbo.GH_SanPham_tb
    (
        shop_taikhoan, ten, mo_ta, noi_dung, hinh_anh,
        gia_ban, gia_von, loai, id_danhmuc, bin,
        ngay_tao, ngay_cap_nhat, so_luong_ton, so_luong_da_ban, luot_truy_cap
    )
    VALUES
    (
        @ShopTaiKhoan, N'Dịch vụ demo', N'Mô tả ngắn dịch vụ demo', N'Nội dung chi tiết dịch vụ demo',
        N'/uploads/images/macdinh.jpg',
        450000, 0, N'dichvu', @DanhMucId, 0,
        @Now, @Now, 0, 0, 0
    );
END

DECLARE @Map TABLE (gh_id INT, bv_id INT);

MERGE dbo.BaiViet_tb AS target
USING
(
    SELECT
        src.id AS gh_id,
        src.ten,
        LOWER(REPLACE(REPLACE(LTRIM(RTRIM(ISNULL(src.ten, N''))), N' ', N'-'), N'--', N'-')) AS name_en,
        src.id_danhmuc,
        src.noi_dung,
        src.mo_ta,
        src.hinh_anh,
        ISNULL(src.bin, 0) AS bin,
        ISNULL(src.ngay_tao, @Now) AS ngay_tao,
        src.shop_taikhoan,
        src.gia_ban,
        src.gia_von,
        ISNULL(src.so_luong_ton, 0) AS so_luong_ton,
        ISNULL(src.so_luong_da_ban, 0) AS so_luong_da_ban,
        ISNULL(src.luot_truy_cap, 0) AS luot_truy_cap,
        CASE WHEN LOWER(LTRIM(RTRIM(ISNULL(src.loai, '')))) = N'dichvu' THEN N'dichvu' ELSE N'sanpham' END AS phanloai
    FROM dbo.GH_SanPham_tb src
    WHERE src.shop_taikhoan = @ShopTaiKhoan
      AND src.id_baiviet IS NULL
) AS src
ON 1 = 0
WHEN NOT MATCHED THEN
    INSERT
    (
        name, name_en, id_DanhMuc, content_post, description, image,
        bin, ngaytao, nguoitao, noibat,
        giaban, giavon, soluong_tonkho, soluong_daban, LuotTruyCap,
        phanloai
    )
    VALUES
    (
        src.ten, src.name_en, src.id_danhmuc, src.noi_dung, src.mo_ta, src.hinh_anh,
        src.bin, src.ngay_tao, src.shop_taikhoan, 0,
        src.gia_ban, src.gia_von, src.so_luong_ton, src.so_luong_da_ban, src.luot_truy_cap,
        src.phanloai
    )
OUTPUT src.gh_id, inserted.id INTO @Map(gh_id, bv_id);

UPDATE gh
SET gh.id_baiviet = m.bv_id
FROM dbo.GH_SanPham_tb gh
INNER JOIN @Map m ON m.gh_id = gh.id;

SELECT
    shop = @ShopTaiKhoan,
    gh_count = (SELECT COUNT(1) FROM dbo.GH_SanPham_tb WHERE shop_taikhoan = @ShopTaiKhoan),
    bv_count = (SELECT COUNT(1) FROM dbo.BaiViet_tb WHERE nguoitao = @ShopTaiKhoan AND phanloai IN (N'sanpham', N'dichvu'));
