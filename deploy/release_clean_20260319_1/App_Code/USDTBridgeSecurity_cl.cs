using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

public class USDTBridgeSecurityCheckResult
{
    public bool ok { get; set; }
    public int http_status { get; set; }
    public string public_message { get; set; }
    public string reason { get; set; }
    public string request_id { get; set; }
    public string source_ip { get; set; }
    public string nonce { get; set; }
    public DateTime? timestamp_utc { get; set; }
    public string payload_sha256 { get; set; }
    public string api_key_fingerprint { get; set; }
}

public static class USDTBridgeSecurity_cl
{
    private static readonly Regex NonceRegex = new Regex("^[A-Za-z0-9_\\-:.=]{16,120}$", RegexOptions.Compiled);

    public static USDTBridgeSecurityCheckResult ValidateInbound(HttpRequest request, string rawBody)
    {
        USDTBridgeSecurityCheckResult result = new USDTBridgeSecurityCheckResult
        {
            ok = false,
            http_status = 400,
            public_message = "Request rejected.",
            reason = "invalid_request",
            request_id = Guid.NewGuid().ToString("N"),
            source_ip = ResolveSourceIp(request),
            nonce = "",
            timestamp_utc = null,
            payload_sha256 = ComputeSha256Hex(rawBody ?? ""),
            api_key_fingerprint = ""
        };

        if (!request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            return Fail(result, 405, "method_not_allowed");

        if (USDTBridgeConfig_cl.RequireHttps && !request.IsSecureConnection)
            return Fail(result, 403, "https_required");

        if (USDTBridgeConfig_cl.RequireJsonContentType)
        {
            string contentType = (request.ContentType ?? "").ToLowerInvariant();
            if (!contentType.Contains("application/json"))
                return Fail(result, 415, "invalid_content_type");
        }

        if (string.IsNullOrWhiteSpace(rawBody))
            return Fail(result, 400, "empty_payload");

        int maxBytes = USDTBridgeConfig_cl.MaxRequestBodyBytes;
        int bodyBytes = Encoding.UTF8.GetByteCount(rawBody ?? "");
        if (maxBytes > 0 && bodyBytes > maxBytes)
            return Fail(result, 413, "payload_too_large");

        if (USDTBridgeConfig_cl.EnforceSourceIpAllowList)
        {
            if (!IsIpAllowed(result.source_ip, USDTBridgeConfig_cl.SourceIpAllowList))
                return Fail(result, 403, "ip_not_allowed");
        }

        string apiKey = (request.Headers["X-Bridge-Key"] ?? "").Trim();
        if (!USDTBridgeConfig_cl.IsValidApiKey(apiKey))
            return Fail(result, 401, "invalid_api_key");

        result.api_key_fingerprint = ComputeSha256Hex(apiKey).Substring(0, 12);

        if (USDTBridgeConfig_cl.RequireHmacSignature)
        {
            if (!USDTBridgeConfig_cl.IsValidSigningSecretConfigured())
                return Fail(result, 503, "signing_secret_missing");

            string tsRaw = (request.Headers["X-Bridge-Timestamp"] ?? "").Trim();
            DateTime tsUtc;
            if (!TryParseTimestamp(tsRaw, out tsUtc))
                return Fail(result, 401, "invalid_timestamp");

            int maxSkew = Math.Max(USDTBridgeConfig_cl.MaxClockSkewSeconds, 1);
            double skew = Math.Abs((DateTime.UtcNow - tsUtc).TotalSeconds);
            if (skew > maxSkew)
                return Fail(result, 401, "timestamp_skew");

            string nonce = (request.Headers["X-Bridge-Nonce"] ?? "").Trim();
            if (!NonceRegex.IsMatch(nonce))
                return Fail(result, 401, "invalid_nonce");

            string signature = (request.Headers["X-Bridge-Signature"] ?? "").Trim().ToLowerInvariant();
            if (signature.Length != 64 || !IsHex(signature))
                return Fail(result, 401, "invalid_signature_format");

            string signingInput = BuildSigningInput(tsRaw, nonce, rawBody ?? "");
            string expected = ComputeHmacSha256Hex(USDTBridgeConfig_cl.SigningSecret, signingInput);
            if (!FixedTimeEquals(expected, signature))
                return Fail(result, 401, "signature_mismatch");

            result.nonce = nonce;
            result.timestamp_utc = tsUtc;
        }

        result.ok = true;
        result.http_status = 200;
        result.public_message = "OK";
        result.reason = "ok";
        return result;
    }

    public static string BuildSigningInput(string timestampRaw, string nonce, string rawBody)
    {
        return (timestampRaw ?? "") + "\n" + (nonce ?? "") + "\n" + (rawBody ?? "");
    }

    public static string ComputeHmacSha256Hex(string secret, string data)
    {
        byte[] key = Encoding.UTF8.GetBytes(secret ?? "");
        byte[] payload = Encoding.UTF8.GetBytes(data ?? "");
        using (HMACSHA256 hmac = new HMACSHA256(key))
        {
            byte[] hash = hmac.ComputeHash(payload);
            return ToLowerHex(hash);
        }
    }

    public static string ComputeSha256Hex(string data)
    {
        byte[] payload = Encoding.UTF8.GetBytes(data ?? "");
        using (SHA256 sha = SHA256.Create())
        {
            byte[] hash = sha.ComputeHash(payload);
            return ToLowerHex(hash);
        }
    }

    private static USDTBridgeSecurityCheckResult Fail(USDTBridgeSecurityCheckResult state, int statusCode, string reason)
    {
        state.ok = false;
        state.http_status = statusCode;
        state.reason = reason;
        state.public_message = "Request rejected.";
        return state;
    }

    private static bool TryParseTimestamp(string raw, out DateTime utc)
    {
        utc = DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        long epoch;
        if (long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out epoch))
        {
            try
            {
                utc = DateTimeOffset.FromUnixTimeSeconds(epoch).UtcDateTime;
                return true;
            }
            catch
            {
                return false;
            }
        }

        DateTime parsed;
        if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out parsed))
        {
            utc = parsed.ToUniversalTime();
            return true;
        }

        return false;
    }

    private static string ResolveSourceIp(HttpRequest request)
    {
        if (USDTBridgeConfig_cl.TrustProxyHeaders)
        {
            string xff = request.Headers["X-Forwarded-For"];
            if (!string.IsNullOrWhiteSpace(xff))
            {
                string first = xff.Split(',').Select(x => x.Trim()).FirstOrDefault(x => x.Length > 0);
                if (!string.IsNullOrWhiteSpace(first))
                    return first;
            }

            string xri = request.Headers["X-Real-IP"];
            if (!string.IsNullOrWhiteSpace(xri))
                return xri.Trim();
        }

        return (request.UserHostAddress ?? "").Trim();
    }

    private static bool IsIpAllowed(string sourceIp, string[] allowList)
    {
        if (allowList == null || allowList.Length == 0)
            return false;

        if (allowList.Any(x => x == "*"))
            return true;

        IPAddress source;
        if (!IPAddress.TryParse(sourceIp, out source))
            return false;

        foreach (string rule in allowList)
        {
            if (string.IsNullOrWhiteSpace(rule))
                continue;

            string token = rule.Trim();
            if (!token.Contains("/"))
            {
                IPAddress ip;
                if (IPAddress.TryParse(token, out ip) && ip.Equals(source))
                    return true;
                continue;
            }

            if (IsInCidr(source, token))
                return true;
        }

        return false;
    }

    private static bool IsInCidr(IPAddress source, string cidr)
    {
        string[] parts = cidr.Split('/');
        if (parts.Length != 2)
            return false;

        IPAddress network;
        if (!IPAddress.TryParse(parts[0], out network))
            return false;

        int prefixLength;
        if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out prefixLength))
            return false;

        byte[] sourceBytes = source.GetAddressBytes();
        byte[] networkBytes = network.GetAddressBytes();
        if (sourceBytes.Length != networkBytes.Length)
            return false;

        int maxPrefix = sourceBytes.Length * 8;
        if (prefixLength < 0 || prefixLength > maxPrefix)
            return false;

        int fullBytes = prefixLength / 8;
        int remainingBits = prefixLength % 8;

        for (int i = 0; i < fullBytes; i++)
        {
            if (sourceBytes[i] != networkBytes[i])
                return false;
        }

        if (remainingBits == 0)
            return true;

        int mask = 0xFF << (8 - remainingBits);
        return (sourceBytes[fullBytes] & mask) == (networkBytes[fullBytes] & mask);
    }

    private static string ToLowerHex(byte[] bytes)
    {
        StringBuilder sb = new StringBuilder(bytes.Length * 2);
        for (int i = 0; i < bytes.Length; i++)
            sb.Append(bytes[i].ToString("x2", CultureInfo.InvariantCulture));
        return sb.ToString();
    }

    private static bool FixedTimeEquals(string left, string right)
    {
        if (left == null || right == null)
            return false;

        int diff = left.Length ^ right.Length;
        int minLength = Math.Min(left.Length, right.Length);
        for (int i = 0; i < minLength; i++)
            diff |= left[i] ^ right[i];
        return diff == 0;
    }

    private static bool IsHex(string value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            bool ok = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f');
            if (!ok)
                return false;
        }
        return true;
    }
}
