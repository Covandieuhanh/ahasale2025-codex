using System;
using System.Data.SqlClient;
using System.IO;
using System.Net.Sockets;
using System.Threading;

public static class SqlTransientGuard_cl
{
    private static readonly string[] TransientMarkers = new[]
    {
        "operation on non-blocking socket would block",
        "unable to read data from the transport connection",
        "a transport-level error has occurred when receiving results from the server",
        "connection is broken and recovery is not possible",
        "physical connection is not usable",
        "snix_read",
        "snix_connect",
        "sni_error_35",
        "sni_error_40",
        "timeout expired",
        "the wait operation timed out",
        "a network-related or instance-specific error occurred",
        "transport connection",
        "cannot open database"
    };

    public static bool IsTransient(Exception ex)
    {
        while (ex != null)
        {
            if (ex is TimeoutException || ex is IOException || ex is SocketException || ex is SqlException)
            {
                SqlException sqlEx = ex as SqlException;
                if (sqlEx != null)
                {
                    foreach (SqlError error in sqlEx.Errors)
                    {
                        if (error != null && error.Number == 4060)
                            return true;
                    }
                }

                string message = (ex.Message ?? "").Trim();
                if (message.Length == 0)
                    return true;

                string normalized = message.ToLowerInvariant();
                for (int i = 0; i < TransientMarkers.Length; i++)
                {
                    if (normalized.Contains(TransientMarkers[i]))
                        return true;
                }
            }

            ex = ex.InnerException;
        }

        return false;
    }

    public static void Execute(Action action, int maxAttempts = 3, int delayMs = 250)
    {
        Execute<int>(() =>
        {
            action();
            return 0;
        }, maxAttempts, delayMs);
    }

    public static T Execute<T>(Func<T> action, int maxAttempts = 3, int delayMs = 250)
    {
        if (action == null)
            throw new ArgumentNullException("action");

        if (maxAttempts < 1)
            maxAttempts = 1;
        if (delayMs < 0)
            delayMs = 0;

        Exception lastError = null;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                if (!IsTransient(ex) || attempt >= maxAttempts)
                    throw;

                lastError = ex;
                try
                {
                    SqlConnection.ClearAllPools();
                }
                catch
                {
                }

                if (delayMs > 0)
                    Thread.Sleep(delayMs * attempt);
            }
        }

        throw lastError ?? new InvalidOperationException("Unknown SQL transient retry failure.");
    }
}
