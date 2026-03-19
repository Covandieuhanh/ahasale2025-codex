using System;

public static class AccountType_cl
{
    public const string Treasury = "Tài khoản tổng";
    public const string LegacyTreasury = "Ví tổng";

    public static bool IsTreasury(string accountType)
    {
        string t = (accountType ?? "").Trim();
        if (string.Equals(t, Treasury, StringComparison.OrdinalIgnoreCase)
            || string.Equals(t, LegacyTreasury, StringComparison.OrdinalIgnoreCase))
            return true;

        return t.StartsWith(Treasury + " ", StringComparison.OrdinalIgnoreCase)
            || t.StartsWith(LegacyTreasury + " ", StringComparison.OrdinalIgnoreCase);
    }

    public static string Normalize(string accountType)
    {
        string t = (accountType ?? "").Trim();

        if (string.Equals(t, Treasury, StringComparison.OrdinalIgnoreCase)
            || string.Equals(t, LegacyTreasury, StringComparison.OrdinalIgnoreCase))
            return Treasury;

        if (t.StartsWith(Treasury + " ", StringComparison.OrdinalIgnoreCase))
            return Treasury + t.Substring(Treasury.Length);

        if (t.StartsWith(LegacyTreasury + " ", StringComparison.OrdinalIgnoreCase))
            return Treasury + t.Substring(LegacyTreasury.Length);

        return t;
    }
}
