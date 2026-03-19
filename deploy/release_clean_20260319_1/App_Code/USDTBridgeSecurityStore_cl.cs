using System;
using System.Data.SqlClient;
using System.Linq;

public static class USDTBridgeSecurityStore_cl
{
    public static bool TryEnforceReplayAndRateLimit(string sourceIp, string nonce, out string reason)
    {
        reason = "";

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                EnsureSecurityTables(db);
                CleanupExpiredNonces(db);

                int limit = USDTBridgeConfig_cl.MaxRequestsPerMinutePerIp;
                if (limit > 0)
                {
                    int count = db.ExecuteQuery<int>(
                        @"SELECT COUNT(1)
                          FROM dbo.USDT_Bridge_Request_Log_tb
                          WHERE source_ip = {0}
                            AND created_at >= DATEADD(MINUTE, -1, GETDATE())",
                        sourceIp ?? ""
                    ).FirstOrDefault();

                    if (count >= limit)
                    {
                        reason = "rate_limited";
                        return false;
                    }
                }

                if (!string.IsNullOrWhiteSpace(nonce))
                {
                    try
                    {
                        db.ExecuteCommand(
                            @"INSERT INTO dbo.USDT_Bridge_Nonce_tb (nonce, source_ip, created_at)
                              VALUES ({0}, {1}, GETDATE())",
                            nonce,
                            sourceIp ?? ""
                        );
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2601 || ex.Number == 2627)
                        {
                            reason = "nonce_replay";
                            return false;
                        }
                        throw;
                    }
                }
            }
        }
        catch
        {
            reason = "security_store_unavailable";
            return false;
        }

        return true;
    }

    public static void WriteAudit(
        string requestId,
        string sourceIp,
        string nonce,
        DateTime? timestampUtc,
        string payloadSha256,
        string txHash,
        string decision,
        int statusCode,
        string detail,
        string apiKeyFingerprint)
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                EnsureSecurityTables(db);
                db.ExecuteCommand(
                    @"INSERT INTO dbo.USDT_Bridge_Request_Log_tb
                      (
                          request_id, source_ip, nonce, request_timestamp_utc,
                          payload_sha256, tx_hash, decision, status_code,
                          detail, api_key_fingerprint, created_at
                      )
                      VALUES
                      (
                          {0}, {1}, {2}, {3},
                          {4}, {5}, {6}, {7},
                          {8}, {9}, GETDATE()
                      )",
                    Safe(requestId, 64),
                    Safe(sourceIp, 64),
                    Safe(nonce, 120),
                    timestampUtc,
                    Safe(payloadSha256, 64),
                    Safe(txHash, 120),
                    Safe(decision, 64),
                    statusCode,
                    Safe(detail, 500),
                    Safe(apiKeyFingerprint, 32)
                );
            }
        }
        catch
        {
            // Security log failures must not break request processing flow.
        }
    }

    private static void CleanupExpiredNonces(dbDataContext db)
    {
        int ttl = Math.Max(USDTBridgeConfig_cl.NonceTtlHours, 1);
        db.ExecuteCommand(
            @"DELETE TOP (1000)
              FROM dbo.USDT_Bridge_Nonce_tb
              WHERE created_at < DATEADD(HOUR, -{0}, GETDATE())",
            ttl
        );
    }

    public static void EnsureSecurityTables(dbDataContext db)
    {
        db.ExecuteCommand(@"
IF OBJECT_ID('dbo.USDT_Bridge_Nonce_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.USDT_Bridge_Nonce_tb
    (
        nonce NVARCHAR(120) NOT NULL PRIMARY KEY,
        source_ip NVARCHAR(64) NULL,
        created_at DATETIME NOT NULL DEFAULT(GETDATE())
    );
END;

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
");
    }

    private static string Safe(string value, int max)
    {
        if (string.IsNullOrEmpty(value))
            return "";
        return value.Length <= max ? value : value.Substring(0, max);
    }
}
