using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

public class USDTBridgeDepositRequest
{
    public string tx_hash { get; set; }
    public string direction { get; set; }
    public string chain { get; set; }
    public string token_contract { get; set; }
    public string from_address { get; set; }
    public string to_address { get; set; }
    public long? block_number { get; set; }
    public int confirmations { get; set; }
    public decimal token_amount { get; set; }
    public decimal usdt_amount { get; set; }
    public string observed_at_utc { get; set; }
    public string source_payload { get; set; }
}

public class USDTBridgeCreditResult
{
    public bool ok { get; set; }
    public string status { get; set; }
    public string message { get; set; }
    public string tx_hash { get; set; }
    public decimal usdt_amount { get; set; }
    public decimal points_credited { get; set; }
    public string treasury_account { get; set; }
    public long? linked_transfer_id { get; set; }
}

public static class USDTPointBridge_cl
{
    private static readonly Regex EvmTxHashRegex = new Regex("^0x[a-fA-F0-9]{64}$", RegexOptions.Compiled);
    private static readonly Regex EvmAddressRegex = new Regex("^0x[a-fA-F0-9]{40}$", RegexOptions.Compiled);

    private class DepositRow
    {
        public long id { get; set; }
        public string status { get; set; }
        public string chain { get; set; }
        public string from_address { get; set; }
        public string to_address { get; set; }
        public decimal usdt_amount { get; set; }
        public decimal points_credited { get; set; }
        public long? linked_transfer_id { get; set; }
    }

    public static USDTBridgeCreditResult CreditTreasuryFromDeposit(USDTBridgeDepositRequest request)
    {
        USDTBridgeCreditResult invalid = ValidateRequest(request);
        if (invalid != null)
            return invalid;

        string txHash = NormalizeTxHash(request.tx_hash);
        string direction = NormalizeDirection(request.direction);
        decimal points = Math.Round(request.usdt_amount * USDTBridgeConfig_cl.PointRatePerToken, 2, MidpointRounding.AwayFromZero);
        string treasuryAccount = USDTBridgeConfig_cl.TreasuryAccount;

        using (dbDataContext db = new dbDataContext())
        {
            db.Connection.Open();
            var tran = db.Connection.BeginTransaction(IsolationLevel.Serializable);
            db.Transaction = tran;

            try
            {
                EnsureBridgeTable(db);

                var existed = GetDepositByTxHash(db, txHash);
                if (existed != null)
                {
                    if (!IsReplayPayloadConsistent(request, existed))
                    {
                        MarkSuspiciousReplay(db, existed.id);
                        tran.Commit();
                        return Error("Duplicate tx_hash with mismatched payload.", txHash, request.usdt_amount, 0, treasuryAccount, "security_replay_mismatch");
                    }

                    tran.Commit();
                    return BuildAlreadyProcessedResult(txHash, treasuryAccount, existed);
                }

                var treasury = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == treasuryAccount);
                if (treasury == null)
                {
                    tran.Rollback();
                    return Error("Treasury account not found.", txHash, request.usdt_amount, 0, treasuryAccount, "treasury_not_found");
                }

                LichSuChuyenDiem_tb tx = new LichSuChuyenDiem_tb();
                tx.ngay = AhaTime_cl.Now;
                tx.nap_rut = true;
                tx.GhiChu = BuildTransferNote(request, points, txHash);
                decimal signedPoints;
                string rowStatus;
                string rowNote;
                string ledgerNote;
                string notifyContent;
                long depositId;

                if (direction == "OUT")
                {
                    decimal current = treasury.DongA ?? 0m;
                    if (current < points)
                    {
                        tran.Rollback();
                        return Error(
                            "Insufficient treasury points for outbound token transfer.",
                            txHash,
                            request.usdt_amount,
                            0,
                            treasuryAccount,
                            "insufficient_treasury_points"
                        );
                    }

                    treasury.DongA = current - points;
                    tx.taikhoan_chuyen = treasury.taikhoan;
                    tx.taikhoan_nhan = USDTBridgeConfig_cl.BridgeSourceWallet;
                    tx.trangtrai_rut = "Rút Token";
                    signedPoints = -points;
                    rowStatus = "Debited";
                    rowNote = "Đã trừ điểm theo giao dịch token rút ra";
                    ledgerNote = "Trừ " + points.ToString("#,##0.##") + " Quyền tiêu dùng do token rút ra. TX: " + txHash;
                    notifyContent = "Tài khoản tổng vừa bị trừ " + points.ToString("#,##0.##") + " Quyền tiêu dùng do token rút ra.";
                    depositId = InsertPendingDeposit(db, request, points, txHash);
                }
                else
                {
                    treasury.DongA = (treasury.DongA ?? 0m) + points;
                    tx.taikhoan_chuyen = USDTBridgeConfig_cl.BridgeSourceWallet;
                    tx.taikhoan_nhan = treasury.taikhoan;
                    tx.trangtrai_rut = "Nạp Token";
                    signedPoints = points;
                    rowStatus = "Credited";
                    rowNote = "Đã cộng điểm thành công";
                    ledgerNote = "Cộng " + points.ToString("#,##0.##") + " Quyền tiêu dùng từ nạp token. TX: " + txHash;
                    notifyContent = "Tài khoản tổng vừa được cộng " + points.ToString("#,##0.##") + " Quyền tiêu dùng từ giao dịch token nạp vào.";
                    depositId = InsertPendingDeposit(db, request, points, txHash);
                }

                tx.dongA = points;
                db.LichSuChuyenDiem_tbs.InsertOnSubmit(tx);

                Helper_DongA_cl.AddLedger(
                    db,
                    treasury.taikhoan,
                    points,
                    direction != "OUT",
                    ledgerNote,
                    txHash,
                    1
                );

                Helper_DongA_cl.AddNotify(
                    db,
                    Helper_DongA_cl.GENESIS_WALLET,
                    treasury.taikhoan,
                    notifyContent,
                    "/admin/lich-su-chuyen-diem/default.aspx"
                );

                db.SubmitChanges();

                db.ExecuteCommand(
                    @"UPDATE dbo.USDT_Deposit_Bridge_tb
                      SET status = {0},
                          points_credited = {1},
                          linked_transfer_id = {2},
                          credited_at = GETDATE(),
                          credited_by = {3},
                          note = {4}
                      WHERE id = {5}",
                    rowStatus,
                    signedPoints,
                    tx.id,
                    treasury.taikhoan,
                    rowNote,
                    depositId
                );

                tran.Commit();

                return new USDTBridgeCreditResult
                {
                    ok = true,
                    status = direction == "OUT" ? "debited" : "credited",
                    message = direction == "OUT" ? "Bridge debited by outbound transfer." : "Bridge credited.",
                    tx_hash = txHash,
                    usdt_amount = request.usdt_amount,
                    points_credited = signedPoints,
                    treasury_account = treasury.taikhoan,
                    linked_transfer_id = tx.id
                };
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    tran.Rollback();
                    using (dbDataContext db2 = new dbDataContext())
                    {
                        EnsureBridgeTable(db2);
                        var existed = GetDepositByTxHash(db2, txHash);
                        if (existed != null)
                        {
                            if (!IsReplayPayloadConsistent(request, existed))
                                return Error("Duplicate tx_hash with mismatched payload.", txHash, request.usdt_amount, 0, treasuryAccount, "security_replay_mismatch");
                            return BuildAlreadyProcessedResult(txHash, treasuryAccount, existed);
                        }
                    }

                    return Error("Duplicate transaction detected.", txHash, request.usdt_amount, 0, treasuryAccount, "duplicate_tx");
                }
                tran.Rollback();
                return Error("Bridge internal sql error.", txHash, request.usdt_amount, 0, treasuryAccount, "internal_sql_error");
            }
            catch
            {
                tran.Rollback();
                return Error("Bridge internal error.", txHash, request.usdt_amount, 0, treasuryAccount, "internal_error");
            }
        }
    }

    private static USDTBridgeCreditResult ValidateRequest(USDTBridgeDepositRequest request)
    {
        if (!USDTBridgeConfig_cl.Enabled)
            return Error("Bridge disabled.", "", 0, 0, USDTBridgeConfig_cl.TreasuryAccount, "bridge_disabled");

        if (request == null)
            return Error("Empty payload.", "", 0, 0, USDTBridgeConfig_cl.TreasuryAccount, "empty_payload");

        string txHash = NormalizeTxHash(request.tx_hash);
        if (string.IsNullOrWhiteSpace(txHash))
            return Error("Missing tx_hash.", "", request.usdt_amount, 0, USDTBridgeConfig_cl.TreasuryAccount, "missing_tx_hash");

        request.usdt_amount = ResolveTokenAmount(request);
        if (request.usdt_amount <= 0)
            return Error("Invalid token amount.", txHash, request.usdt_amount, 0, USDTBridgeConfig_cl.TreasuryAccount, "invalid_amount");

        if (GetDecimalScale(request.usdt_amount) > 6)
            return Error("Too many decimal places.", txHash, request.usdt_amount, 0, USDTBridgeConfig_cl.TreasuryAccount, "invalid_amount_scale");

        if (request.usdt_amount < USDTBridgeConfig_cl.MinTokenPerTx)
            return Error("Amount below minimum threshold.", txHash, request.usdt_amount, 0, USDTBridgeConfig_cl.TreasuryAccount, "amount_too_small");

        if (USDTBridgeConfig_cl.MaxTokenPerTx > 0 && request.usdt_amount > USDTBridgeConfig_cl.MaxTokenPerTx)
            return Error("Amount exceeds maximum threshold.", txHash, request.usdt_amount, 0, USDTBridgeConfig_cl.TreasuryAccount, "amount_too_large");

        if (request.block_number.HasValue && request.block_number.Value <= 0)
            return Error("Invalid block_number.", txHash, request.usdt_amount, 0, USDTBridgeConfig_cl.TreasuryAccount, "invalid_block_number");

        decimal points = Math.Round(request.usdt_amount * USDTBridgeConfig_cl.PointRatePerToken, 2, MidpointRounding.AwayFromZero);
        if (points <= 0)
            return Error("Invalid converted points.", txHash, request.usdt_amount, 0, USDTBridgeConfig_cl.TreasuryAccount, "invalid_converted_points");

        if (request.confirmations < USDTBridgeConfig_cl.MinConfirmations)
        {
            return Error(
                "Insufficient confirmations.",
                txHash,
                request.usdt_amount,
                0,
                USDTBridgeConfig_cl.TreasuryAccount,
                "insufficient_confirmations"
            );
        }

        string allowedChain = NormalizeChain(USDTBridgeConfig_cl.AllowedChain);
        if (!string.IsNullOrWhiteSpace(allowedChain))
        {
            string requestChain = NormalizeChain(request.chain);
            if (!requestChain.Equals(allowedChain, StringComparison.OrdinalIgnoreCase))
            {
                return Error(
                    "Invalid chain.",
                    txHash,
                    request.usdt_amount,
                    0,
                    USDTBridgeConfig_cl.TreasuryAccount,
                    "invalid_chain"
                );
            }
        }

        if (USDTBridgeConfig_cl.StrictTxHashFormat && IsEvmChain(allowedChain))
        {
            if (!EvmTxHashRegex.IsMatch(txHash))
                return Error("Invalid tx_hash format.", txHash, request.usdt_amount, 0, USDTBridgeConfig_cl.TreasuryAccount, "invalid_tx_hash_format");
        }

        string tokenContract = (request.token_contract ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(USDTBridgeConfig_cl.TokenContract))
        {
            if (!tokenContract.Equals(USDTBridgeConfig_cl.TokenContract.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return Error(
                    "Invalid token contract.",
                    txHash,
                    request.usdt_amount,
                    0,
                    USDTBridgeConfig_cl.TreasuryAccount,
                    "invalid_token_contract"
                );
            }
        }

        string from = (request.from_address ?? "").Trim();
        string to = (request.to_address ?? "").Trim();
        if (string.IsNullOrWhiteSpace(from))
            return Error("Missing source address.", txHash, request.usdt_amount, 0, USDTBridgeConfig_cl.TreasuryAccount, "missing_from_address");
        if (string.IsNullOrWhiteSpace(to))
            return Error("Missing destination address.", txHash, request.usdt_amount, 0, USDTBridgeConfig_cl.TreasuryAccount, "missing_to_address");

        if (USDTBridgeConfig_cl.StrictAddressFormat && IsEvmChain(allowedChain))
        {
            if (!EvmAddressRegex.IsMatch(to))
                return Error("Invalid destination address format.", txHash, request.usdt_amount, 0, USDTBridgeConfig_cl.TreasuryAccount, "invalid_to_address_format");
            if (!string.IsNullOrWhiteSpace(from) && !EvmAddressRegex.IsMatch(from))
                return Error("Invalid source address format.", txHash, request.usdt_amount, 0, USDTBridgeConfig_cl.TreasuryAccount, "invalid_from_address_format");
            if (!string.IsNullOrWhiteSpace(tokenContract) && !EvmAddressRegex.IsMatch(tokenContract))
                return Error("Invalid token contract format.", txHash, request.usdt_amount, 0, USDTBridgeConfig_cl.TreasuryAccount, "invalid_token_contract_format");
        }

        string depositAddress = (USDTBridgeConfig_cl.DepositAddress ?? "").Trim();
        string inferredDirection = InferDirection(from, to, depositAddress);
        if (string.IsNullOrWhiteSpace(inferredDirection))
        {
            return Error(
                "Transfer does not involve configured deposit wallet.",
                txHash,
                request.usdt_amount,
                0,
                USDTBridgeConfig_cl.TreasuryAccount,
                "invalid_wallet_relation"
            );
        }

        string requestDirection = NormalizeDirection(request.direction);
        if (string.IsNullOrWhiteSpace(requestDirection))
            request.direction = inferredDirection;
        else if (!requestDirection.Equals(inferredDirection, StringComparison.OrdinalIgnoreCase))
            return Error("Direction mismatched with transfer addresses.", txHash, request.usdt_amount, 0, USDTBridgeConfig_cl.TreasuryAccount, "direction_mismatch");

        if ((from ?? "").Equals(to ?? "", StringComparison.OrdinalIgnoreCase))
        {
            return Error(
                "Self transfer is not supported for bridge mint/burn.",
                txHash,
                request.usdt_amount,
                0,
                USDTBridgeConfig_cl.TreasuryAccount,
                "self_transfer_not_supported"
            );
        }

        return null;
    }

    private static USDTBridgeCreditResult BuildAlreadyProcessedResult(string txHash, string treasuryAccount, DepositRow existed)
    {
        string status = (existed.status ?? "").Trim();
        bool processed = status.Equals("Credited", StringComparison.OrdinalIgnoreCase) ||
                         status.Equals("Debited", StringComparison.OrdinalIgnoreCase);
        return new USDTBridgeCreditResult
        {
            ok = processed,
            status = processed ? "already_processed" : "exists_not_credited",
            message = processed ? "Already processed." : "Transaction exists but is not credited.",
            tx_hash = txHash,
            usdt_amount = existed.usdt_amount,
            points_credited = existed.points_credited,
            treasury_account = treasuryAccount,
            linked_transfer_id = existed.linked_transfer_id
        };
    }

    private static USDTBridgeCreditResult Error(string message, string txHash, decimal usdtAmount, decimal points, string treasury, string status)
    {
        return new USDTBridgeCreditResult
        {
            ok = false,
            status = string.IsNullOrWhiteSpace(status) ? "error" : status,
            message = message,
            tx_hash = txHash,
            usdt_amount = usdtAmount,
            points_credited = points,
            treasury_account = treasury,
            linked_transfer_id = null
        };
    }

    private static string NormalizeTxHash(string txHash)
    {
        if (string.IsNullOrWhiteSpace(txHash))
            return "";
        return txHash.Trim().ToLowerInvariant();
    }

    private static decimal ResolveTokenAmount(USDTBridgeDepositRequest request)
    {
        if (request == null)
            return 0m;

        if (request.token_amount > 0)
            return request.token_amount;

        return request.usdt_amount;
    }

    private static string NormalizeDirection(string direction)
    {
        string normalized = (direction ?? "").Trim().ToUpperInvariant();
        if (normalized == "IN" || normalized == "INBOUND" || normalized == "DEPOSIT" || normalized == "MINT")
            return "IN";
        if (normalized == "OUT" || normalized == "OUTBOUND" || normalized == "WITHDRAW" || normalized == "BURN")
            return "OUT";
        return "";
    }

    private static string InferDirection(string from, string to, string depositAddress)
    {
        string src = (from ?? "").Trim();
        string dst = (to ?? "").Trim();
        string deposit = (depositAddress ?? "").Trim();

        if (string.IsNullOrWhiteSpace(deposit))
            return "";

        if (dst.Equals(deposit, StringComparison.OrdinalIgnoreCase))
            return "IN";

        if (src.Equals(deposit, StringComparison.OrdinalIgnoreCase))
            return "OUT";

        return "";
    }

    private static string NormalizeChain(string chain)
    {
        string normalized = (chain ?? "").Trim().ToUpperInvariant();
        if (normalized == "BEP20" || normalized == "BNB" || normalized == "BINANCE SMART CHAIN" || normalized == "BNB SMART CHAIN")
            return "BSC";
        return normalized;
    }

    private static bool IsEvmChain(string chain)
    {
        string normalized = NormalizeChain(chain);
        return normalized == "BSC" ||
               normalized == "ETH" ||
               normalized == "POLYGON" ||
               normalized == "AVAX" ||
               normalized == "ARBITRUM" ||
               normalized == "OPTIMISM";
    }

    private static string BuildTransferNote(USDTBridgeDepositRequest request, decimal points, string txHash)
    {
        string direction = NormalizeDirection(request.direction);
        string chain = request.chain ?? "";
        string tokenContract = request.token_contract ?? "";
        string from = request.from_address ?? "";
        string to = request.to_address ?? "";
        string block = request.block_number.HasValue ? request.block_number.Value.ToString() : "";
        string cfm = request.confirmations.ToString();
        string amount = request.usdt_amount.ToString("#,##0.######");

        return "TOKEN BRIDGE | tx=" + txHash +
               " | direction=" + direction +
               " | chain=" + chain +
               " | token_contract=" + tokenContract +
               " | from=" + from +
               " | to=" + to +
               " | block=" + block +
               " | confirmations=" + cfm +
               " | token=" + amount +
               " | points=" + points.ToString("#,##0.##");
    }

    private static DepositRow GetDepositByTxHash(dbDataContext db, string txHash)
    {
        return db.ExecuteQuery<DepositRow>(
            @"SELECT TOP 1 id, status, chain, from_address, to_address, usdt_amount, points_credited, linked_transfer_id
              FROM dbo.USDT_Deposit_Bridge_tb
              WHERE tx_hash = {0}",
            txHash
        ).FirstOrDefault();
    }

    private static long InsertPendingDeposit(dbDataContext db, USDTBridgeDepositRequest request, decimal points, string txHash)
    {
        string safePayload = request.source_payload ?? "";
        if (safePayload.Length > 3800)
            safePayload = safePayload.Substring(0, 3800);

        return db.ExecuteQuery<long>(
            @"DECLARE @inserted TABLE(id BIGINT);
              INSERT INTO dbo.USDT_Deposit_Bridge_tb
              (
                  tx_hash,
                  chain,
                  from_address,
                  to_address,
                  block_number,
                  confirmations,
                  usdt_amount,
                  point_rate,
                  points_credited,
                  status,
                  source_payload,
                  note,
                  created_at
              )
              OUTPUT inserted.id INTO @inserted(id)
              VALUES
              (
                  {0},
                  {1},
                  {2},
                  {3},
                  {4},
                  {5},
                  {6},
                  {7},
                  {8},
                  {9},
                  {10},
                  {11},
                  GETDATE()
              );
              SELECT TOP 1 id FROM @inserted;",
            txHash,
            (request.chain ?? "").Trim(),
            (request.from_address ?? "").Trim(),
            (request.to_address ?? "").Trim(),
            request.block_number,
            request.confirmations,
            request.usdt_amount,
            USDTBridgeConfig_cl.PointRatePerToken,
            points,
            "Pending",
            safePayload,
                    "Đã nhận request, đang xử lý cộng/trừ điểm"
        ).First();
    }

    private static bool IsReplayPayloadConsistent(USDTBridgeDepositRequest request, DepositRow existed)
    {
        if (existed == null)
            return false;

        decimal diff = Math.Abs(existed.usdt_amount - request.usdt_amount);
        if (diff > 0.000001m)
            return false;

        string reqChain = NormalizeChain(request.chain);
        string dbChain = NormalizeChain(existed.chain);
        if (!reqChain.Equals(dbChain, StringComparison.OrdinalIgnoreCase))
            return false;

        string reqTo = (request.to_address ?? "").Trim();
        string dbTo = (existed.to_address ?? "").Trim();
        if (!reqTo.Equals(dbTo, StringComparison.OrdinalIgnoreCase))
            return false;

        string reqFrom = (request.from_address ?? "").Trim();
        string dbFrom = (existed.from_address ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(reqFrom) && !string.IsNullOrWhiteSpace(dbFrom))
        {
            if (!reqFrom.Equals(dbFrom, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        string requestDirection = InferDirection(reqFrom, reqTo, USDTBridgeConfig_cl.DepositAddress);
        string existedDirection = InferDirection(dbFrom, dbTo, USDTBridgeConfig_cl.DepositAddress);
        if (!requestDirection.Equals(existedDirection, StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }

    private static void MarkSuspiciousReplay(dbDataContext db, long depositId)
    {
        db.ExecuteCommand(
            @"UPDATE dbo.USDT_Deposit_Bridge_tb
              SET note = {0}
              WHERE id = {1}",
            "SECURITY ALERT: replay mismatch blocked",
            depositId
        );
    }

    private static int GetDecimalScale(decimal value)
    {
        int[] bits = decimal.GetBits(value);
        return (bits[3] >> 16) & 0xFF;
    }

    public static void EnsureBridgeTable(dbDataContext db)
    {
        db.ExecuteCommand(@"
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
");
    }
}
