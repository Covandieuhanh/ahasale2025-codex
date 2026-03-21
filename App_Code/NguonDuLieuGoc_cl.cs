using System;

public static class NguonDuLieuGoc_cl
{
    public static dbDataContext Mo()
    {
        return new dbDataContext();
    }

    public static void ThucThi(Action<dbDataContext> hanhDong)
    {
        if (hanhDong == null)
            return;

        using (dbDataContext db = Mo())
        {
            hanhDong(db);
        }
    }

    public static T ThucThi<T>(Func<dbDataContext, T> ham)
    {
        if (ham == null)
            return default(T);

        using (dbDataContext db = Mo())
        {
            return ham(db);
        }
    }
}

