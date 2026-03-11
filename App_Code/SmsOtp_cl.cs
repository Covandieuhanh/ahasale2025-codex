using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;

public static class SmsOtp_cl
{
    private const string DefaultTemplate = "Ma OTP AhaSale cua ban la: {OTP}. Het han sau 5 phut.";

    public static bool SendOtp(string phone, string otp, out string error, out bool usedFallback)
    {
        return SendOtpInternal(null, phone, otp, out error, out usedFallback);
    }

    public static bool SendOtp(dbDataContext db, string phone, string otp, out string error, out bool usedFallback)
    {
        return SendOtpInternal(db, phone, otp, out error, out usedFallback);
    }

    private static bool SendOtpInternal(dbDataContext db, string phone, string otp, out string error, out bool usedFallback)
    {
        error = "";
        usedFallback = false;

        string endpoint = "";
        string apiKey = "";
        string sender = "";
        string template = "";
        string method = "";
        string rawParams = "";
        bool devMode = false;

        try
        {
            SmsOtpConfig cfg = (db != null) ? OtpConfig_cl.GetSmsConfig(db) : OtpConfig_cl.GetSmsConfig();
            if (cfg != null)
            {
                endpoint = cfg.Endpoint ?? "";
                apiKey = cfg.ApiKey ?? "";
                sender = cfg.Sender ?? "";
                template = cfg.Template ?? "";
                method = cfg.Method ?? "";
                rawParams = cfg.Params ?? "";
                devMode = cfg.DevMode;
            }
            else
            {
                endpoint = ConfigurationManager.AppSettings["SmsOtp.Endpoint"] ?? "";
                apiKey = ConfigurationManager.AppSettings["SmsOtp.ApiKey"] ?? "";
                sender = ConfigurationManager.AppSettings["SmsOtp.Sender"] ?? "";
                template = ConfigurationManager.AppSettings["SmsOtp.Template"] ?? "";
                method = ConfigurationManager.AppSettings["SmsOtp.Method"] ?? "";
                rawParams = ConfigurationManager.AppSettings["SmsOtp.Params"] ?? "";
                devMode = IsTruthy(ConfigurationManager.AppSettings["SmsOtp.DevMode"]);
            }
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log("[SMS OTP CFG] " + ex.Message, phone ?? "", ex.StackTrace);
            endpoint = ConfigurationManager.AppSettings["SmsOtp.Endpoint"] ?? "";
            apiKey = ConfigurationManager.AppSettings["SmsOtp.ApiKey"] ?? "";
            sender = ConfigurationManager.AppSettings["SmsOtp.Sender"] ?? "";
            template = ConfigurationManager.AppSettings["SmsOtp.Template"] ?? "";
            method = ConfigurationManager.AppSettings["SmsOtp.Method"] ?? "";
            rawParams = ConfigurationManager.AppSettings["SmsOtp.Params"] ?? "";
            devMode = IsTruthy(ConfigurationManager.AppSettings["SmsOtp.DevMode"]);
        }

        if (string.IsNullOrWhiteSpace(template))
            template = DefaultTemplate;

        string message = (template ?? "").Replace("{OTP}", otp ?? "");

        if (devMode)
        {
            usedFallback = true;
            Log_cl.Add_Log("[SMS OTP DEV] " + (phone ?? "") + " - " + message, phone ?? "", "");
            return true;
        }

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            error = "Chưa cấu hình SmsOtp.Endpoint.";
            return false;
        }

        try
        {
            string resolvedEndpoint = endpoint;
            Dictionary<string, string> paramMap = ParseParams(rawParams);
            Dictionary<string, string> tokens = BuildTokenMap(phone, otp, message, sender, apiKey);

            resolvedEndpoint = ReplaceTokens(resolvedEndpoint, tokens);
            string query = BuildQueryString(paramMap, tokens, endpoint);
            if (!string.IsNullOrEmpty(query))
            {
                resolvedEndpoint += (resolvedEndpoint.Contains("?") ? "&" : "?") + query;
            }

            string httpMethod = (method ?? "").Trim().ToUpperInvariant();
            if (httpMethod == "GET")
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resolvedEndpoint);
                req.Method = "GET";
                req.Timeout = 15000;
                if (!string.IsNullOrWhiteSpace(apiKey))
                    req.Headers["X-Api-Key"] = apiKey;

                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    int code = (int)resp.StatusCode;
                    if (code < 200 || code >= 300)
                    {
                        error = "SMS OTP thất bại (HTTP " + code + ").";
                        return false;
                    }
                }

                return true;
            }

            HttpWebRequest postReq = (HttpWebRequest)WebRequest.Create(resolvedEndpoint);
            postReq.Method = "POST";
            postReq.ContentType = "application/json";
            postReq.Timeout = 15000;
            if (!string.IsNullOrWhiteSpace(apiKey))
                postReq.Headers["X-Api-Key"] = apiKey;

            string payload = BuildJsonPayload(phone, message, sender);
            byte[] data = Encoding.UTF8.GetBytes(payload);
            postReq.ContentLength = data.Length;

            using (var stream = postReq.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            using (HttpWebResponse resp = (HttpWebResponse)postReq.GetResponse())
            {
                int code = (int)resp.StatusCode;
                if (code < 200 || code >= 300)
                {
                    error = "SMS OTP thất bại (HTTP " + code + ").";
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            Log_cl.Add_Log("[SMS OTP] " + ex.Message, phone ?? "", ex.StackTrace);
            return false;
        }
    }

    private static Dictionary<string, string> BuildTokenMap(string phone, string otp, string message, string sender, string apiKey)
    {
        var tokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        tokens["OTP"] = otp ?? "";
        tokens["otp"] = otp ?? "";
        tokens["phoneNumber"] = phone ?? "";
        tokens["message"] = message ?? "";
        tokens["brandName"] = sender ?? "";
        tokens["sender"] = sender ?? "";
        tokens["apiKey"] = apiKey ?? "";
        tokens["timestamp"] = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        tokens["now"] = DateTime.Now.ToString("yyyyMMddHHmmss");
        return tokens;
    }

    private static Dictionary<string, string> ParseParams(string rawParams)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(rawParams)) return result;

        string[] lines = rawParams.Replace("\r", "").Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string line = (lines[i] ?? "").Trim();
            if (string.IsNullOrEmpty(line)) continue;
            int idx = line.IndexOf('=');
            if (idx <= 0) continue;
            string key = line.Substring(0, idx).Trim();
            string value = line.Substring(idx + 1).Trim();
            if (string.IsNullOrEmpty(key)) continue;
            result[key] = value;
        }
        return result;
    }

    private static string ReplaceTokens(string input, Dictionary<string, string> tokens)
    {
        if (string.IsNullOrEmpty(input) || tokens == null) return input ?? "";
        string output = input;
        foreach (var kv in tokens)
        {
            output = output.Replace("{" + kv.Key + "}", kv.Value ?? "");
        }
        return output;
    }

    private static string BuildQueryString(Dictionary<string, string> paramMap, Dictionary<string, string> tokens, string endpointTemplate)
    {
        if (paramMap == null || paramMap.Count == 0) return "";
        var sb = new StringBuilder();
        foreach (var kv in paramMap)
        {
            string key = kv.Key ?? "";
            if (string.IsNullOrWhiteSpace(key)) continue;

            string placeholder = "{" + key + "}";
            if (!string.IsNullOrEmpty(endpointTemplate) && endpointTemplate.IndexOf(placeholder, StringComparison.OrdinalIgnoreCase) >= 0)
                continue;

            string val = ReplaceTokens(kv.Value ?? "", tokens);
            if (tokens != null && !tokens.ContainsKey(key))
                tokens[key] = val ?? "";

            if (sb.Length > 0) sb.Append("&");
            sb.Append(Uri.EscapeDataString(key));
            sb.Append("=");
            sb.Append(Uri.EscapeDataString(val ?? ""));
        }
        return sb.ToString();
    }

    private static string BuildJsonPayload(string phone, string message, string sender)
    {
        string json = "{\"to\":\"" + EscapeJson(phone) + "\",\"message\":\"" + EscapeJson(message) + "\"";
        if (!string.IsNullOrWhiteSpace(sender))
            json += ",\"sender\":\"" + EscapeJson(sender) + "\"";
        json += "}";
        return json;
    }

    private static string EscapeJson(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        return value.Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\r", "\\r")
                    .Replace("\n", "\\n");
    }

    private static bool IsTruthy(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        string v = value.Trim().ToLowerInvariant();
        return v == "1" || v == "true" || v == "yes" || v == "on";
    }
}
