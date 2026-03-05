IF OBJECT_ID('dbo.USDT_Deposit_Bridge_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.USDT_Deposit_Bridge_tb
    (
        id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        tx_hash NVARCHAR(120) NOT NULL,
        chain NVARCHAR(32) NULL,
        from_address NVARCHAR(128) NULL,
        to_address NVARCHAR(128) NULL,
        block_number BIGINT NULL,
        confirmations INT NULL,
        usdt_amount DECIMAL(38, 18) NOT NULL,
        point_rate DECIMAL(38, 18) NOT NULL,
        points_credited DECIMAL(18, 2) NOT NULL,
        status NVARCHAR(30) NOT NULL,
        source_payload NVARCHAR(MAX) NULL,
        note NVARCHAR(500) NULL,
        linked_transfer_id BIGINT NULL,
        credited_by NVARCHAR(120) NULL,
        created_at DATETIME NOT NULL DEFAULT(GETDATE()),
        credited_at DATETIME NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_USDT_Deposit_Bridge_tx_hash'
      AND object_id = OBJECT_ID('dbo.USDT_Deposit_Bridge_tb')
)
BEGIN
    CREATE UNIQUE INDEX UX_USDT_Deposit_Bridge_tx_hash
    ON dbo.USDT_Deposit_Bridge_tb(tx_hash);
END;
GO

IF OBJECT_ID('dbo.USDT_Bridge_Nonce_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.USDT_Bridge_Nonce_tb
    (
        nonce NVARCHAR(120) NOT NULL PRIMARY KEY,
        source_ip NVARCHAR(64) NULL,
        created_at DATETIME NOT NULL DEFAULT(GETDATE())
    );
END;
GO

IF OBJECT_ID('dbo.USDT_Bridge_Request_Log_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.USDT_Bridge_Request_Log_tb
    (
        id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        request_id NVARCHAR(64) NULL,
        source_ip NVARCHAR(64) NULL,
        nonce NVARCHAR(120) NULL,
        request_timestamp_utc DATETIME NULL,
        payload_sha256 NVARCHAR(64) NULL,
        tx_hash NVARCHAR(120) NULL,
        decision NVARCHAR(64) NULL,
        status_code INT NULL,
        detail NVARCHAR(500) NULL,
        api_key_fingerprint NVARCHAR(32) NULL,
        created_at DATETIME NOT NULL DEFAULT(GETDATE())
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_USDT_Bridge_Request_Log_source_ip_created_at'
      AND object_id = OBJECT_ID('dbo.USDT_Bridge_Request_Log_tb')
)
BEGIN
    CREATE INDEX IX_USDT_Bridge_Request_Log_source_ip_created_at
    ON dbo.USDT_Bridge_Request_Log_tb(source_ip, created_at);
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_USDT_Bridge_Request_Log_tx_hash'
      AND object_id = OBJECT_ID('dbo.USDT_Bridge_Request_Log_tb')
)
BEGIN
    CREATE INDEX IX_USDT_Bridge_Request_Log_tx_hash
    ON dbo.USDT_Bridge_Request_Log_tb(tx_hash);
END;
GO
