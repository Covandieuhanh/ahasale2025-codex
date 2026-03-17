-- Ensure core home schema for search + shop visibility
-- Run once on production DB

IF COL_LENGTH('dbo.BaiViet_tb', 'name_khongdau') IS NULL
BEGIN
    ALTER TABLE dbo.BaiViet_tb ADD name_khongdau NVARCHAR(MAX) NULL;
END;

IF COL_LENGTH('dbo.BaiViet_tb', 'description_khongdau') IS NULL
BEGIN
    ALTER TABLE dbo.BaiViet_tb ADD description_khongdau NVARCHAR(MAX) NULL;
END;

IF COL_LENGTH('dbo.taikhoan_tb', 'TrangThai_Shop') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_tb
        ADD TrangThai_Shop TINYINT NOT NULL
        CONSTRAINT DF_taikhoan_tb_TrangThai_Shop DEFAULT(0) WITH VALUES;
END;

;WITH Latest AS (
    SELECT dk.taikhoan,
           dk.TrangThai,
           ROW_NUMBER() OVER (PARTITION BY dk.taikhoan ORDER BY dk.NgayTao DESC, dk.ID DESC) AS rn
    FROM dbo.DangKy_GianHangDoiTac_tb dk
)
UPDATE tk
SET TrangThai_Shop = l.TrangThai
FROM dbo.taikhoan_tb tk
INNER JOIN Latest l ON l.taikhoan = tk.taikhoan AND l.rn = 1;

-- Legacy shop accounts không có hồ sơ đăng ký -> mặc định đã duyệt
UPDATE tk
SET TrangThai_Shop = 1
FROM dbo.taikhoan_tb tk
WHERE NOT EXISTS (SELECT 1 FROM dbo.DangKy_GianHangDoiTac_tb dk WHERE dk.taikhoan = tk.taikhoan)
  AND (
        LOWER(LTRIM(RTRIM(ISNULL(tk.phanloai, '')))) = N'gian hàng đối tác'
        OR CHARINDEX('shop', LOWER(ISNULL(tk.permission, ''))) > 0
      );
