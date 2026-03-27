using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangAdminSourceLifecycle_cl
{
    private const string StatusActive = "active";
    private const string StatusInactive = "inactive";

    public sealed class SourceLifecycleInfo
    {
        public string Status { get; set; }
        public string Label { get; set; }
        public string Css { get; set; }
        public string Note { get; set; }
        public bool IsInactive { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    private sealed class SourceLifecycleRaw
    {
        public long Id { get; set; }
        public string OwnerAccountKey { get; set; }
        public string SourceType { get; set; }
        public string SourceKey { get; set; }
        public string LifecycleStatus { get; set; }
        public string LifecycleLabel { get; set; }
        public string Note { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    private sealed class SourceLifecycleKeyRow
    {
        public string SourceKey { get; set; }
    }

    public static SourceLifecycleInfo GetInfo(
        dbDataContext db,
        string ownerAccountKey,
        string sourceType,
        string sourceKey,
        string activeLabel,
        string inactiveLabel,
        string activeNote,
        string inactiveNote)
    {
        if (db == null)
            return BuildDefault(activeLabel, activeNote);

        string ownerKey = Normalize(ownerAccountKey);
        string normalizedSourceType = Normalize(sourceType);
        string normalizedSourceKey = (sourceKey ?? "").Trim();
        if (ownerKey == "" || normalizedSourceType == "" || normalizedSourceKey == "")
            return BuildDefault(activeLabel, activeNote);

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        SourceLifecycleRaw row = GetRow(db, ownerKey, normalizedSourceType, normalizedSourceKey);
        string status = Normalize(row == null ? "" : row.LifecycleStatus);
        bool isInactive = status == StatusInactive;

        return new SourceLifecycleInfo
        {
            Status = isInactive ? StatusInactive : StatusActive,
            Label = isInactive ? inactiveLabel : activeLabel,
            Css = isInactive ? "bg-orange fg-white" : "bg-green fg-white",
            Note = isInactive ? inactiveNote : activeNote,
            IsInactive = isInactive,
            UpdatedAt = row == null ? null : row.UpdatedAt
        };
    }

    public static void SetActive(dbDataContext db, string ownerAccountKey, string sourceType, string sourceKey, string changedBy, string note)
    {
        Upsert(db, ownerAccountKey, sourceType, sourceKey, StatusActive, "Đang dùng ở nguồn", changedBy, note);
    }

    public static void SetInactive(dbDataContext db, string ownerAccountKey, string sourceType, string sourceKey, string changedBy, string note)
    {
        Upsert(db, ownerAccountKey, sourceType, sourceKey, StatusInactive, "Đã ngừng dùng ở nguồn", changedBy, note);
    }

    public static HashSet<string> GetInactiveKeySet(dbDataContext db, string ownerAccountKey, string sourceType)
    {
        HashSet<string> results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (db == null)
            return results;

        string ownerKey = Normalize(ownerAccountKey);
        string normalizedSourceType = Normalize(sourceType);
        if (ownerKey == "" || normalizedSourceType == "")
            return results;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        foreach (SourceLifecycleKeyRow row in db.ExecuteQuery<SourceLifecycleKeyRow>(
            "SELECT SourceKey FROM dbo.CoreGianHangSourceLifecycle_tb WHERE OwnerAccountKey = {0} AND SourceType = {1} AND LifecycleStatus = {2}",
            ownerKey,
            normalizedSourceType,
            StatusInactive))
        {
            string sourceKey = (row == null ? "" : row.SourceKey ?? "").Trim();
            if (sourceKey != "")
                results.Add(sourceKey);
        }

        return results;
    }

    private static SourceLifecycleInfo BuildDefault(string activeLabel, string activeNote)
    {
        return new SourceLifecycleInfo
        {
            Status = StatusActive,
            Label = activeLabel,
            Css = "bg-green fg-white",
            Note = activeNote,
            IsInactive = false
        };
    }

    private static void Upsert(dbDataContext db, string ownerAccountKey, string sourceType, string sourceKey, string status, string lifecycleLabel, string changedBy, string note)
    {
        if (db == null)
            return;

        string ownerKey = Normalize(ownerAccountKey);
        string normalizedSourceType = Normalize(sourceType);
        string normalizedSourceKey = (sourceKey ?? "").Trim();
        string normalizedStatus = Normalize(status) == StatusInactive ? StatusInactive : StatusActive;
        if (ownerKey == "" || normalizedSourceType == "" || normalizedSourceKey == "")
            return;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        DateTime now = AhaTime_cl.Now;

        int affected = db.ExecuteCommand(
            "UPDATE dbo.CoreGianHangSourceLifecycle_tb " +
            "SET LifecycleStatus = {3}, LifecycleLabel = {4}, Note = {5}, ChangedBy = {6}, UpdatedAt = {7} " +
            "WHERE OwnerAccountKey = {0} AND SourceType = {1} AND SourceKey = {2}",
            ownerKey,
            normalizedSourceType,
            normalizedSourceKey,
            normalizedStatus,
            string.IsNullOrWhiteSpace(lifecycleLabel) ? null : lifecycleLabel.Trim(),
            string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
            string.IsNullOrWhiteSpace(changedBy) ? null : changedBy.Trim(),
            now);

        if (affected > 0)
            return;

        db.ExecuteCommand(
            "INSERT INTO dbo.CoreGianHangSourceLifecycle_tb " +
            "(OwnerAccountKey, SourceType, SourceKey, LifecycleStatus, LifecycleLabel, Note, ChangedBy, CreatedAt, UpdatedAt) " +
            "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {7})",
            ownerKey,
            normalizedSourceType,
            normalizedSourceKey,
            normalizedStatus,
            string.IsNullOrWhiteSpace(lifecycleLabel) ? null : lifecycleLabel.Trim(),
            string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
            string.IsNullOrWhiteSpace(changedBy) ? null : changedBy.Trim(),
            now);
    }

    private static SourceLifecycleRaw GetRow(dbDataContext db, string ownerAccountKey, string sourceType, string sourceKey)
    {
        if (db == null)
            return null;

        string ownerKey = Normalize(ownerAccountKey);
        string normalizedSourceType = Normalize(sourceType);
        string normalizedSourceKey = (sourceKey ?? "").Trim();
        if (ownerKey == "" || normalizedSourceType == "" || normalizedSourceKey == "")
            return null;

        return db.ExecuteQuery<SourceLifecycleRaw>(
            "SELECT TOP 1 Id, OwnerAccountKey, SourceType, SourceKey, LifecycleStatus, LifecycleLabel, Note, ChangedBy, CreatedAt, UpdatedAt " +
            "FROM dbo.CoreGianHangSourceLifecycle_tb WHERE OwnerAccountKey = {0} AND SourceType = {1} AND SourceKey = {2} " +
            "ORDER BY UpdatedAt DESC, Id DESC",
            ownerKey,
            normalizedSourceType,
            normalizedSourceKey).FirstOrDefault();
    }

    private static string Normalize(string value)
    {
        return (value ?? "").Trim().ToLowerInvariant();
    }
}
