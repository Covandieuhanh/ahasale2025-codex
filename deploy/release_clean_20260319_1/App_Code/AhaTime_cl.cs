using System;
using System.Collections.Generic;
using System.Configuration;

public static class AhaTime_cl
{
    private static readonly object _syncRoot = new object();
    private static TimeZoneInfo _cachedTimeZone;

    private static TimeZoneInfo ResolveTimeZone()
    {
        var candidates = new List<string>();
        string configured = ConfigurationManager.AppSettings["Aha.TimeZoneId"];
        if (!string.IsNullOrWhiteSpace(configured))
        {
            candidates.Add(configured.Trim());
        }

        candidates.Add("Asia/Ho_Chi_Minh");
        candidates.Add("SE Asia Standard Time");
        candidates.Add("Asia/Bangkok");

        foreach (string id in candidates)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch
            {
                // Try next candidate.
            }
        }

        return TimeZoneInfo.Local;
    }

    public static TimeZoneInfo TimeZone
    {
        get
        {
            if (_cachedTimeZone != null) return _cachedTimeZone;
            lock (_syncRoot)
            {
                if (_cachedTimeZone == null)
                {
                    _cachedTimeZone = ResolveTimeZone();
                }
                return _cachedTimeZone;
            }
        }
    }

    public static DateTime UtcNow
    {
        get { return DateTime.UtcNow; }
    }

    public static DateTime Now
    {
        get { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone); }
    }

    public static DateTime Today
    {
        get { return Now.Date; }
    }

    public static DateTime ToLocal(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(value, TimeZone);
        }

        if (value.Kind == DateTimeKind.Local)
        {
            return TimeZoneInfo.ConvertTime(value, TimeZone);
        }

        return value;
    }

    public static DateTime? ToLocal(DateTime? value)
    {
        if (!value.HasValue) return null;
        return ToLocal(value.Value);
    }
}
