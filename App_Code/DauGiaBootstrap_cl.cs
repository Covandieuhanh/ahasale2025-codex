using System;

public static class DauGiaBootstrap_cl
{
    private static bool _schemaReady;
    private static readonly object _schemaLock = new object();

    public static void EnsureReady()
    {
        using (dbDataContext db = new dbDataContext())
        {
            EnsureSchemaSafe(db);
        }
    }

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
IF OBJECT_ID('dbo.DG_Auction_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DG_Auction_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [slug] NVARCHAR(350) NULL,
        [source_type] NVARCHAR(60) NULL,
        [source_id] NVARCHAR(100) NULL,
        [seller_account] NVARCHAR(120) NOT NULL,
        [seller_scope] NVARCHAR(20) NULL,
        [snapshot_title] NVARCHAR(MAX) NULL,
        [snapshot_desc] NVARCHAR(MAX) NULL,
        [snapshot_image] NVARCHAR(MAX) NULL,
        [snapshot_meta] NVARCHAR(MAX) NULL,
        [gia_niemyet] FLOAT NULL,
        [gia_hien_tai] FLOAT NULL,
        [phi_luot] FLOAT NULL,
        [tien_dat_coc] FLOAT NULL,
        [vi_dau_gia] FLOAT NULL,
        [so_luot_bid] INT NULL,
        [trang_thai] NVARCHAR(40) NULL,
        [phien_bat_dau] DATETIME NULL,
        [phien_ket_thuc] DATETIME NULL,
        [winner_account] NVARCHAR(120) NULL,
        [winner_reserved_at] DATETIME NULL,
        [buyer_confirmed_at] DATETIME NULL,
        [seller_confirmed_at] DATETIME NULL,
        [admin_settled_at] DATETIME NULL,
        [settlement_mode] NVARCHAR(40) NULL,
        [approved_by] NVARCHAR(120) NULL,
        [approved_at] DATETIME NULL,
        [rejected_reason] NVARCHAR(MAX) NULL,
        [da_hoan_coc] BIT NULL CONSTRAINT DF_DG_Auction_da_hoan_coc DEFAULT(0),
        [is_deleted] BIT NULL CONSTRAINT DF_DG_Auction_is_deleted DEFAULT(0),
        [created_at] DATETIME NULL,
        [updated_at] DATETIME NULL,
        [created_by] NVARCHAR(120) NULL,
        [updated_by] NVARCHAR(120) NULL
    );
END

IF OBJECT_ID('dbo.DG_Bid_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DG_Bid_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [auction_id] BIGINT NOT NULL,
        [bidder_account] NVARCHAR(120) NOT NULL,
        [bid_fee] FLOAT NULL,
        [price_before] FLOAT NULL,
        [price_after] FLOAT NULL,
        [trang_thai] NVARCHAR(30) NULL,
        [created_at] DATETIME NULL,
        [created_by] NVARCHAR(120) NULL
    );
END

IF OBJECT_ID('dbo.DG_AssetLock_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DG_AssetLock_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [auction_id] BIGINT NOT NULL,
        [source_type] NVARCHAR(60) NULL,
        [source_id] NVARCHAR(100) NULL,
        [owner_account] NVARCHAR(120) NULL,
        [so_luong_khoa] FLOAT NULL,
        [trang_thai] NVARCHAR(30) NULL,
        [created_at] DATETIME NULL,
        [released_at] DATETIME NULL,
        [released_reason] NVARCHAR(MAX) NULL
    );
END

IF OBJECT_ID('dbo.DG_SoCai_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DG_SoCai_tb
    (
        [id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [auction_id] BIGINT NULL,
        [actor_account] NVARCHAR(120) NULL,
        [contra_account] NVARCHAR(120) NULL,
        [loai_giao_dich] NVARCHAR(60) NULL,
        [so_tien] FLOAT NULL,
        [ghi_chu] NVARCHAR(MAX) NULL,
        [ref_id] NVARCHAR(120) NULL,
        [created_at] DATETIME NULL
    );
END

IF OBJECT_ID('dbo.DG_Config_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DG_Config_tb
    (
        [id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [config_key] NVARCHAR(120) NOT NULL,
        [config_value] NVARCHAR(MAX) NULL,
        [note] NVARCHAR(MAX) NULL,
        [updated_at] DATETIME NULL
    );
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'UX_DG_Config_key' AND object_id = OBJECT_ID('dbo.DG_Config_tb')
)
BEGIN
    CREATE UNIQUE INDEX UX_DG_Config_key ON dbo.DG_Config_tb(config_key);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_DG_Auction_status_time' AND object_id = OBJECT_ID('dbo.DG_Auction_tb')
)
BEGIN
    CREATE INDEX IX_DG_Auction_status_time ON dbo.DG_Auction_tb(trang_thai, phien_bat_dau, phien_ket_thuc);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_DG_Auction_seller_status' AND object_id = OBJECT_ID('dbo.DG_Auction_tb')
)
BEGIN
    CREATE INDEX IX_DG_Auction_seller_status ON dbo.DG_Auction_tb(seller_account, trang_thai, created_at);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_DG_Bid_auction_time' AND object_id = OBJECT_ID('dbo.DG_Bid_tb')
)
BEGIN
    CREATE INDEX IX_DG_Bid_auction_time ON dbo.DG_Bid_tb(auction_id, created_at DESC);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_DG_AssetLock_source' AND object_id = OBJECT_ID('dbo.DG_AssetLock_tb')
)
BEGIN
    CREATE INDEX IX_DG_AssetLock_source ON dbo.DG_AssetLock_tb(source_type, source_id, owner_account);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_DG_SoCai_auction' AND object_id = OBJECT_ID('dbo.DG_SoCai_tb')
)
BEGIN
    CREATE INDEX IX_DG_SoCai_auction ON dbo.DG_SoCai_tb(auction_id, created_at DESC);
END
");

                EnsureDefaultConfig(db);
                _schemaReady = true;
            }
            catch
            {
                // Nếu lỗi schema thì lần sau sẽ thử lại.
            }
        }
    }

    private static void EnsureDefaultConfig(dbDataContext db)
    {
        if (db == null)
            return;

        DateTime now = AhaTime_cl.Now;
        EnsureConfig(db, "deposit_percent", "20", "Ty le coc mac dinh (%).", now);
        EnsureConfig(db, "reserve_timeout_minutes", "10080", "Thoi gian cho phep xac nhan sau khi giu cho (phut).", now);
        EnsureConfig(db, "start_buffer_minutes", "5", "Thoi gian bat dau toi thieu sau khi tao phien (phut).", now);
        EnsureConfig(db, "settlement_mode_default", DauGiaPolicy_cl.SettlementManualFulfillment, "Che do tat toan mac dinh.", now);
    }

    private static void EnsureConfig(dbDataContext db, string key, string value, string note, DateTime now)
    {
        db.ExecuteCommand(@"
IF NOT EXISTS (SELECT 1 FROM dbo.DG_Config_tb WHERE config_key = {0})
BEGIN
    INSERT INTO dbo.DG_Config_tb(config_key, config_value, note, updated_at)
    VALUES ({0}, {1}, {2}, {3});
END
", key, value, note, now);
    }
}
