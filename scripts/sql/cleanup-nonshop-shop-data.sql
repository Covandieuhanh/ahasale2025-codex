SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ANSI_PADDING ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;

DECLARE @updated_rows INT = 0;

BEGIN TRY
    BEGIN TRAN;

    ;WITH ScopeCTE AS
    (
        SELECT
            t.id,
            t.taikhoan,
            CASE
                WHEN LOWER(LTRIM(RTRIM(ISNULL(t.permission, N'')))) LIKE N'%portal_admin%' THEN N'admin'
                WHEN LOWER(LTRIM(RTRIM(ISNULL(t.permission, N'')))) LIKE N'%portal_shop%' THEN N'shop'
                WHEN LOWER(LTRIM(RTRIM(ISNULL(t.permission, N'')))) LIKE N'%portal_home%' THEN N'home'
                WHEN LOWER(LTRIM(RTRIM(ISNULL(t.taikhoan, N'')))) = N'admin' THEN N'admin'
                WHEN LOWER(ISNULL(t.permission, N'')) LIKE N'%q1[_]%' OR LOWER(ISNULL(t.permission, N'')) LIKE N'%q2[_]%' THEN N'admin'
                ELSE N'home'
            END AS scope,
            CASE
                WHEN
                    NULLIF(LTRIM(RTRIM(ISNULL(t.ten_shop, N''))), N'') IS NOT NULL
                    OR NULLIF(LTRIM(RTRIM(ISNULL(t.sdt_shop, N''))), N'') IS NOT NULL
                    OR NULLIF(LTRIM(RTRIM(ISNULL(t.email_shop, N''))), N'') IS NOT NULL
                    OR NULLIF(LTRIM(RTRIM(ISNULL(t.logo_shop, N''))), N'') IS NOT NULL
                    OR NULLIF(LTRIM(RTRIM(ISNULL(t.anhbia_shop, N''))), N'') IS NOT NULL
                    OR NULLIF(LTRIM(RTRIM(ISNULL(t.link_zalo_shop, N''))), N'') IS NOT NULL
                    OR NULLIF(LTRIM(RTRIM(ISNULL(t.motangan_shop, N''))), N'') IS NOT NULL
                    OR NULLIF(LTRIM(RTRIM(ISNULL(t.diachi_shop, N''))), N'') IS NOT NULL
                    OR NULLIF(LTRIM(RTRIM(ISNULL(t.linkfb_shop, N''))), N'') IS NOT NULL
                    OR NULLIF(LTRIM(RTRIM(ISNULL(t.youtube_shop, N''))), N'') IS NOT NULL
                    OR NULLIF(LTRIM(RTRIM(ISNULL(t.tiktok_shop, N''))), N'') IS NOT NULL
                    OR NULLIF(LTRIM(RTRIM(ISNULL(t.Ten_FB_Shop, N''))), N'') IS NOT NULL
                    OR NULLIF(LTRIM(RTRIM(ISNULL(t.Ten_Youtube_Shop, N''))), N'') IS NOT NULL
                    OR NULLIF(LTRIM(RTRIM(ISNULL(t.Ten_TikTok_Shop, N''))), N'') IS NOT NULL
                    OR NULLIF(LTRIM(RTRIM(ISNULL(t.Ten_Zalo_Shop, N''))), N'') IS NOT NULL
                    OR NULLIF(LTRIM(RTRIM(ISNULL(t.slug_shop, N''))), N'') IS NOT NULL
                    OR ISNULL(t.HoSo_TieuDung_ShopOnly, 0) <> 0
                    OR ISNULL(t.HoSo_UuDai_ShopOnly, 0) <> 0
                    OR ISNULL(t.ChiPhanTram_BanDichVu_ChoSan, 0) <> 0
                THEN 1
                ELSE 0
            END AS has_shop_data
        FROM dbo.taikhoan_tb t
    )
    UPDATE t
    SET
        logo_shop = NULL,
        anhbia_shop = NULL,
        ten_shop = NULL,
        sdt_shop = NULL,
        email_shop = NULL,
        link_zalo_shop = NULL,
        motangan_shop = NULL,
        diachi_shop = NULL,
        linkfb_shop = NULL,
        youtube_shop = NULL,
        tiktok_shop = NULL,
        Ten_FB_Shop = NULL,
        Ten_Youtube_Shop = NULL,
        Ten_TikTok_Shop = NULL,
        Ten_Zalo_Shop = NULL,
        slug_shop = NULL,
        HoSo_TieuDung_ShopOnly = 0,
        HoSo_UuDai_ShopOnly = 0,
        ChiPhanTram_BanDichVu_ChoSan = 0
    FROM dbo.taikhoan_tb t
    INNER JOIN ScopeCTE s ON s.id = t.id
    WHERE s.scope <> N'shop'
      AND s.has_shop_data = 1;

    SET @updated_rows = @@ROWCOUNT;

    COMMIT;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    THROW;
END CATCH;

;WITH ScopeCTE AS
(
    SELECT
        t.id,
        t.taikhoan,
        CASE
            WHEN LOWER(LTRIM(RTRIM(ISNULL(t.permission, N'')))) LIKE N'%portal_admin%' THEN N'admin'
            WHEN LOWER(LTRIM(RTRIM(ISNULL(t.permission, N'')))) LIKE N'%portal_shop%' THEN N'shop'
            WHEN LOWER(LTRIM(RTRIM(ISNULL(t.permission, N'')))) LIKE N'%portal_home%' THEN N'home'
            WHEN LOWER(LTRIM(RTRIM(ISNULL(t.taikhoan, N'')))) = N'admin' THEN N'admin'
            WHEN LOWER(ISNULL(t.permission, N'')) LIKE N'%q1[_]%' OR LOWER(ISNULL(t.permission, N'')) LIKE N'%q2[_]%' THEN N'admin'
            ELSE N'home'
        END AS scope
    FROM dbo.taikhoan_tb t
)
SELECT
    @updated_rows AS updated_rows,
    SUM(
        CASE WHEN s.scope <> N'shop' AND
            (
                NULLIF(LTRIM(RTRIM(ISNULL(t.ten_shop, N''))), N'') IS NOT NULL
                OR NULLIF(LTRIM(RTRIM(ISNULL(t.sdt_shop, N''))), N'') IS NOT NULL
                OR NULLIF(LTRIM(RTRIM(ISNULL(t.email_shop, N''))), N'') IS NOT NULL
                OR NULLIF(LTRIM(RTRIM(ISNULL(t.logo_shop, N''))), N'') IS NOT NULL
                OR NULLIF(LTRIM(RTRIM(ISNULL(t.anhbia_shop, N''))), N'') IS NOT NULL
                OR NULLIF(LTRIM(RTRIM(ISNULL(t.link_zalo_shop, N''))), N'') IS NOT NULL
                OR NULLIF(LTRIM(RTRIM(ISNULL(t.motangan_shop, N''))), N'') IS NOT NULL
                OR NULLIF(LTRIM(RTRIM(ISNULL(t.diachi_shop, N''))), N'') IS NOT NULL
                OR NULLIF(LTRIM(RTRIM(ISNULL(t.linkfb_shop, N''))), N'') IS NOT NULL
                OR NULLIF(LTRIM(RTRIM(ISNULL(t.youtube_shop, N''))), N'') IS NOT NULL
                OR NULLIF(LTRIM(RTRIM(ISNULL(t.tiktok_shop, N''))), N'') IS NOT NULL
                OR NULLIF(LTRIM(RTRIM(ISNULL(t.Ten_FB_Shop, N''))), N'') IS NOT NULL
                OR NULLIF(LTRIM(RTRIM(ISNULL(t.Ten_Youtube_Shop, N''))), N'') IS NOT NULL
                OR NULLIF(LTRIM(RTRIM(ISNULL(t.Ten_TikTok_Shop, N''))), N'') IS NOT NULL
                OR NULLIF(LTRIM(RTRIM(ISNULL(t.Ten_Zalo_Shop, N''))), N'') IS NOT NULL
                OR NULLIF(LTRIM(RTRIM(ISNULL(t.slug_shop, N''))), N'') IS NOT NULL
                OR ISNULL(t.HoSo_TieuDung_ShopOnly, 0) <> 0
                OR ISNULL(t.HoSo_UuDai_ShopOnly, 0) <> 0
                OR ISNULL(t.ChiPhanTram_BanDichVu_ChoSan, 0) <> 0
            )
        THEN 1 ELSE 0 END
    ) AS remaining_nonshop_with_shop_data
FROM dbo.taikhoan_tb t
INNER JOIN ScopeCTE s ON s.id = t.id;
