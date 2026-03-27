using System;

public static class LegacyBool_cl
{
    public static bool IsTrue(bool? value)
    {
        return value == true;
    }

    public static bool IsFalseOrNull(bool? value)
    {
        return value != true;
    }
}
