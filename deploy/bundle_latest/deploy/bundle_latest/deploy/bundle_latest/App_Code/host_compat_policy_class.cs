using System;
using System.Configuration;
using System.Web;

public static class host_compat_policy_class
{
    public static bool is_allowed_request_host(HttpRequest request)
    {
        if (request == null || request.Url == null)
            return true;

        string host = (request.Url.Host ?? "").Trim().ToLower();
        if (host == "")
            return false;

        string allowedHostsRaw = ConfigurationManager.AppSettings["AllowedHosts"];
        if (string.IsNullOrWhiteSpace(allowedHostsRaw))
            return true;

        string[] tokens = allowedHostsRaw.Split(new char[] { ',', ';', ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string token in tokens)
        {
            string candidate = token.Trim().ToLower();
            if (candidate == "")
                continue;

            if (candidate == "*")
                return true;

            if (candidate == host)
                return true;

            if (candidate.StartsWith("*."))
            {
                string suffix = candidate.Substring(1); // ".example.com"
                if (host.EndsWith(suffix))
                    return true;
            }
        }

        return false;
    }
}
