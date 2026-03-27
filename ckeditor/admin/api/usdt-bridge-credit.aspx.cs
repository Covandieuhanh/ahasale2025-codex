using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;

public partial class admin_api_usdt_bridge_credit : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "application/json; charset=utf-8";
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();

        if (!USDTBridgeConfig_cl.Enabled)
        {
            WriteJson(503, new
            {
                ok = false,
                status = "error",
                message = "Bridge unavailable."
            });
            return;
        }

        int maxBytes = USDTBridgeConfig_cl.MaxRequestBodyBytes;
        if (maxBytes > 0 && Request.TotalBytes > maxBytes)
        {
            WriteJson(413, new
            {
                ok = false,
                status = "error",
                message = "Payload too large."
            });
            return;
        }

        string rawBody = ReadRequestBody();
        USDTBridgeSecurityCheckResult security = USDTBridgeSecurity_cl.ValidateInbound(Request, rawBody);
        if (!security.ok)
        {
            USDTBridgeSecurityStore_cl.WriteAudit(
                security.request_id,
                security.source_ip,
                security.nonce,
                security.timestamp_utc,
                security.payload_sha256,
                "",
                security.reason,
                security.http_status,
                "Pre-check rejected",
                security.api_key_fingerprint
            );

            WriteJson(security.http_status, new
            {
                ok = false,
                status = "error",
                message = security.public_message,
                request_id = security.request_id
            });
            return;
        }

        string throttleReason;
        if (!USDTBridgeSecurityStore_cl.TryEnforceReplayAndRateLimit(security.source_ip, security.nonce, out throttleReason))
        {
            int blockedStatusCode = throttleReason == "rate_limited" ? 429 :
                                    throttleReason == "nonce_replay" ? 409 :
                                    503;
            USDTBridgeSecurityStore_cl.WriteAudit(
                security.request_id,
                security.source_ip,
                security.nonce,
                security.timestamp_utc,
                security.payload_sha256,
                "",
                throttleReason,
                blockedStatusCode,
                "Rate/replay gate rejected",
                security.api_key_fingerprint
            );

            WriteJson(blockedStatusCode, new
            {
                ok = false,
                status = throttleReason,
                message = "Request rejected.",
                request_id = security.request_id
            });
            return;
        }

        Dictionary<string, object> payload;
        try
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = maxBytes > 0 ? Math.Max(maxBytes, 1024) : 1048576;
            payload = serializer.Deserialize<Dictionary<string, object>>(rawBody);
        }
        catch
        {
            USDTBridgeSecurityStore_cl.WriteAudit(
                security.request_id,
                security.source_ip,
                security.nonce,
                security.timestamp_utc,
                security.payload_sha256,
                "",
                "invalid_json",
                400,
                "JSON parse failed",
                security.api_key_fingerprint
            );

            WriteJson(400, new
            {
                ok = false,
                status = "invalid_json",
                message = "Invalid payload.",
                request_id = security.request_id
            });
            return;
        }

        USDTBridgeDepositRequest request = BuildDepositRequest(payload, rawBody);
        USDTBridgeCreditResult result = USDTPointBridge_cl.CreditTreasuryFromDeposit(request);

        int statusCode = MapStatusCode(result);
        string txHash = (request.tx_hash ?? "").Trim().ToLowerInvariant();

        USDTBridgeSecurityStore_cl.WriteAudit(
            security.request_id,
            security.source_ip,
            security.nonce,
            security.timestamp_utc,
            security.payload_sha256,
            txHash,
            result.status,
            statusCode,
            result.message,
            security.api_key_fingerprint
        );

        WriteJson(statusCode, result);
    }

    private static int MapStatusCode(USDTBridgeCreditResult result)
    {
        if (result == null)
            return 500;

        if (result.ok)
            return 200;

        switch ((result.status ?? "").Trim().ToLowerInvariant())
        {
            case "already_processed":
                return 200;
            case "exists_not_credited":
            case "duplicate_tx":
            case "security_replay_mismatch":
            case "insufficient_treasury_points":
                return 409;
            case "internal_error":
                return 500;
            default:
                return 400;
        }
    }

    private static USDTBridgeDepositRequest BuildDepositRequest(Dictionary<string, object> payload, string rawBody)
    {
        return new USDTBridgeDepositRequest
        {
            tx_hash = PickString(payload, "tx_hash", "txHash", "transaction_hash", "txid", "txID"),
            direction = PickString(payload, "direction", "flow", "transfer_direction"),
            chain = PickString(payload, "chain", "network"),
            token_contract = PickString(payload, "token_contract", "tokenContract", "contract", "contractAddress"),
            from_address = PickString(payload, "from_address", "fromAddress", "owner_address"),
            to_address = PickString(payload, "to_address", "toAddress", "recipient_address"),
            block_number = PickLong(payload, "block_number", "blockNumber", "block"),
            confirmations = PickInt(payload, "confirmations"),
            token_amount = PickDecimal(payload, "token_amount", "tokenAmount"),
            usdt_amount = PickDecimal(payload, "usdt_amount", "usdtAmount", "amount"),
            observed_at_utc = PickString(payload, "observed_at_utc", "observedAtUtc", "timestamp_utc"),
            source_payload = rawBody
        };
    }

    private static string PickString(Dictionary<string, object> payload, params string[] keys)
    {
        foreach (string key in keys)
        {
            object val;
            if (payload.TryGetValue(key, out val))
                return CoerceString(val);
        }
        return "";
    }

    private static int PickInt(Dictionary<string, object> payload, params string[] keys)
    {
        foreach (string key in keys)
        {
            object val;
            if (!payload.TryGetValue(key, out val))
                continue;

            if (val is int) return (int)val;
            if (val is long) return Convert.ToInt32((long)val);
            if (val is bool) return ((bool)val) ? 1 : 0;

            int parsed;
            if (int.TryParse(CoerceString(val), NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed))
                return parsed;
        }
        return 0;
    }

    private static long? PickLong(Dictionary<string, object> payload, params string[] keys)
    {
        foreach (string key in keys)
        {
            object val;
            if (!payload.TryGetValue(key, out val))
                continue;

            if (val is long) return (long)val;
            if (val is int) return Convert.ToInt64((int)val);

            long parsed;
            if (long.TryParse(CoerceString(val), NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed))
                return parsed;
        }
        return null;
    }

    private static decimal PickDecimal(Dictionary<string, object> payload, params string[] keys)
    {
        foreach (string key in keys)
        {
            object val;
            if (!payload.TryGetValue(key, out val))
                continue;

            if (val is decimal) return (decimal)val;
            if (val is double) return Convert.ToDecimal((double)val);
            if (val is int) return Convert.ToDecimal((int)val);
            if (val is long) return Convert.ToDecimal((long)val);

            decimal parsed;
            if (decimal.TryParse(CoerceString(val), NumberStyles.Any, CultureInfo.InvariantCulture, out parsed))
                return parsed;
        }
        return 0m;
    }

    private static string CoerceString(object value)
    {
        if (value == null) return "";
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
    }

    private string ReadRequestBody()
    {
        Request.InputStream.Position = 0;
        using (StreamReader reader = new StreamReader(Request.InputStream))
            return reader.ReadToEnd();
    }

    private void WriteJson(int statusCode, object payload)
    {
        Response.StatusCode = statusCode;
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        Response.Write(serializer.Serialize(payload));
        Response.Flush();
        HttpContext.Current.ApplicationInstance.CompleteRequest();
    }
}
