using System;
using System.Collections.Generic;

public static class PinAttemptGuard_cl
{
    private sealed class AttemptState
    {
        public int FailCount;
        public DateTime LastFailUtc;
        public DateTime? LockedUntilUtc;
    }

    private static readonly object SyncRoot = new object();
    private static readonly Dictionary<string, AttemptState> States = new Dictionary<string, AttemptState>(StringComparer.OrdinalIgnoreCase);
    private static readonly TimeSpan FailWindow = TimeSpan.FromMinutes(30);

    public static bool IsLocked(string account, string clientIp, out DateTime lockedUntilLocal)
    {
        lockedUntilLocal = DateTime.MinValue;
        string key = BuildKey(account, clientIp);

        lock (SyncRoot)
        {
            CleanupExpired_NoLock();

            AttemptState state;
            if (!States.TryGetValue(key, out state) || state == null || state.LockedUntilUtc == null)
                return false;

            if (state.LockedUntilUtc.Value <= DateTime.UtcNow)
            {
                state.LockedUntilUtc = null;
                return false;
            }

            lockedUntilLocal = state.LockedUntilUtc.Value.ToLocalTime();
            return true;
        }
    }

    public static int RegisterFailure(string account, string clientIp, out DateTime? lockedUntilLocal, out bool shouldBlockAccount)
    {
        lockedUntilLocal = null;
        shouldBlockAccount = false;

        string key = BuildKey(account, clientIp);

        lock (SyncRoot)
        {
            CleanupExpired_NoLock();

            AttemptState state;
            if (!States.TryGetValue(key, out state) || state == null)
            {
                state = new AttemptState();
                States[key] = state;
            }

            if (state.LastFailUtc != DateTime.MinValue && DateTime.UtcNow - state.LastFailUtc > FailWindow)
            {
                state.FailCount = 0;
                state.LockedUntilUtc = null;
            }

            state.FailCount++;
            state.LastFailUtc = DateTime.UtcNow;

            if (state.FailCount >= 10)
            {
                shouldBlockAccount = true;
                state.LockedUntilUtc = null;
            }
            else if (state.FailCount >= 5)
            {
                state.LockedUntilUtc = DateTime.UtcNow.AddMinutes(15);
            }
            else if (state.FailCount >= 3)
            {
                state.LockedUntilUtc = DateTime.UtcNow.AddMinutes(5);
            }

            if (state.LockedUntilUtc != null)
                lockedUntilLocal = state.LockedUntilUtc.Value.ToLocalTime();

            return state.FailCount;
        }
    }

    public static void Reset(string account, string clientIp)
    {
        string key = BuildKey(account, clientIp);
        lock (SyncRoot)
        {
            States.Remove(key);
        }
    }

    private static string BuildKey(string account, string clientIp)
    {
        account = (account ?? string.Empty).Trim().ToLowerInvariant();
        clientIp = (clientIp ?? string.Empty).Trim();
        return account + "|" + clientIp;
    }

    private static void CleanupExpired_NoLock()
    {
        var now = DateTime.UtcNow;
        var toRemove = new List<string>();

        foreach (var kv in States)
        {
            AttemptState state = kv.Value;
            if (state == null)
            {
                toRemove.Add(kv.Key);
                continue;
            }

            bool isOld = state.LastFailUtc != DateTime.MinValue && (now - state.LastFailUtc) > FailWindow;
            bool lockExpired = state.LockedUntilUtc != null && state.LockedUntilUtc.Value <= now;

            if (isOld || (lockExpired && state.FailCount < 3))
            {
                toRemove.Add(kv.Key);
            }
            else if (lockExpired)
            {
                state.LockedUntilUtc = null;
            }
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            States.Remove(toRemove[i]);
        }
    }
}
