using System;

public static class CoreDb_cl
{
    public static dbDataContext Open()
    {
        return new dbDataContext();
    }

    public static void Use(Action<dbDataContext> action)
    {
        if (action == null)
            return;

        using (dbDataContext db = Open())
        {
            action(db);
        }
    }

    public static T Use<T>(Func<dbDataContext, T> func)
    {
        if (func == null)
            return default(T);

        using (dbDataContext db = Open())
        {
            return func(db);
        }
    }
}

