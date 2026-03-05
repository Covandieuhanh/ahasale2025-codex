using System;
using System.Configuration;
using System.Globalization;
using System.Linq;

public static class USDTBridgeConfig_cl
{
    public static bool Enabled
    {
        get { return ParseBool(Get("USDTBridge.Enabled", "false"), false); }
    }

    public static string ApiKey
    {
        get { return Get("USDTBridge.ApiKey", ""); }
    }

    public static string[] ApiKeys
    {
        get { return SplitCsv(Get("USDTBridge.ApiKeys", ApiKey)); }
    }

    public static bool RequireHmacSignature
    {
        get { return ParseBool(Get("USDTBridge.RequireHmacSignature", "true"), true); }
    }

    public static string SigningSecret
    {
        get { return Get("USDTBridge.SigningSecret", ""); }
    }

    public static int MaxClockSkewSeconds
    {
        get { return ParseInt(Get("USDTBridge.MaxClockSkewSeconds", "90"), 90); }
    }

    public static bool RequireHttps
    {
        get { return ParseBool(Get("USDTBridge.RequireHttps", "false"), false); }
    }

    public static int MaxRequestBodyBytes
    {
        get { return ParseInt(Get("USDTBridge.MaxRequestBodyBytes", "16384"), 16384); }
    }

    public static bool RequireJsonContentType
    {
        get { return ParseBool(Get("USDTBridge.RequireJsonContentType", "true"), true); }
    }

    public static bool TrustProxyHeaders
    {
        get { return ParseBool(Get("USDTBridge.TrustProxyHeaders", "false"), false); }
    }

    public static bool EnforceSourceIpAllowList
    {
        get { return ParseBool(Get("USDTBridge.EnforceSourceIpAllowList", "false"), false); }
    }

    public static string[] SourceIpAllowList
    {
        get { return SplitCsv(Get("USDTBridge.SourceIpAllowList", "")); }
    }

    public static int MaxRequestsPerMinutePerIp
    {
        get { return ParseInt(Get("USDTBridge.MaxRequestsPerMinutePerIp", "120"), 120); }
    }

    public static int NonceTtlHours
    {
        get { return ParseInt(Get("USDTBridge.NonceTtlHours", "48"), 48); }
    }

    public static string AllowedChain
    {
        get { return Get("USDTBridge.AllowedChain", "BSC"); }
    }

    public static string DepositAddress
    {
        get { return Get("USDTBridge.DepositAddress", ""); }
    }

    public static string TokenContract
    {
        get { return Get("USDTBridge.TokenContract", "0x55d398326f99059ff775485246999027b3197955"); }
    }

    public static int MinConfirmations
    {
        get { return ParseInt(Get("USDTBridge.MinConfirmations", "20"), 20); }
    }

    public static decimal PointRatePerUSDT
    {
        get { return ParseDecimal(Get("USDTBridge.PointRatePerUSDT", "1000"), 1000m); }
    }

    public static decimal PointRatePerToken
    {
        get { return ParseDecimal(Get("USDTBridge.PointRatePerToken", PointRatePerUSDT.ToString(CultureInfo.InvariantCulture)), PointRatePerUSDT); }
    }

    public static decimal MinUSDTPerTx
    {
        get { return ParseDecimal(Get("USDTBridge.MinUSDTPerTx", "0.01"), 0.01m); }
    }

    public static decimal MinTokenPerTx
    {
        get { return ParseDecimal(Get("USDTBridge.MinTokenPerTx", MinUSDTPerTx.ToString(CultureInfo.InvariantCulture)), MinUSDTPerTx); }
    }

    public static decimal MaxUSDTPerTx
    {
        get { return ParseDecimal(Get("USDTBridge.MaxUSDTPerTx", "100000"), 100000m); }
    }

    public static decimal MaxTokenPerTx
    {
        get { return ParseDecimal(Get("USDTBridge.MaxTokenPerTx", MaxUSDTPerTx.ToString(CultureInfo.InvariantCulture)), MaxUSDTPerTx); }
    }

    public static bool StrictTxHashFormat
    {
        get { return ParseBool(Get("USDTBridge.StrictTxHashFormat", "true"), true); }
    }

    public static bool StrictAddressFormat
    {
        get { return ParseBool(Get("USDTBridge.StrictAddressFormat", "true"), true); }
    }

    public static string TreasuryAccount
    {
        get { return Get("USDTBridge.TreasuryAccount", "vitonggianhangdoitac"); }
    }

    public static string BridgeSourceWallet
    {
        get { return Get("USDTBridge.BridgeSourceWallet", "usdt_bridge"); }
    }

    public static bool StrictWalletOnlyMinting
    {
        get { return ParseBool(Get("USDTBridge.StrictWalletOnlyMinting", "false"), false); }
    }

    public static bool IsValidApiKey(string provided)
    {
        if (string.IsNullOrWhiteSpace(provided))
            return false;

        string normalized = provided.Trim();
        return ApiKeys.Any(k => !string.IsNullOrWhiteSpace(k) && FixedTimeEquals(k, normalized));
    }

    public static bool IsValidSigningSecretConfigured()
    {
        return !string.IsNullOrWhiteSpace(SigningSecret);
    }

    private static string Get(string key, string defaultValue)
    {
        string value = ConfigurationManager.AppSettings[key];
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
    }

    private static bool ParseBool(string value, bool fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
            return fallback;

        string normalized = value.Trim().ToLowerInvariant();
        if (normalized == "1" || normalized == "true" || normalized == "yes" || normalized == "on")
            return true;
        if (normalized == "0" || normalized == "false" || normalized == "no" || normalized == "off")
            return false;
        return fallback;
    }

    private static int ParseInt(string value, int fallback)
    {
        int parsed;
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed) ? parsed : fallback;
    }

    private static decimal ParseDecimal(string value, decimal fallback)
    {
        decimal parsed;
        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out parsed) ? parsed : fallback;
    }

    private static string[] SplitCsv(string csv)
    {
        if (string.IsNullOrWhiteSpace(csv))
            return new string[0];

        return csv
            .Split(',')
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    // Compare with near constant-time loop to reduce timing leak.
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
}
