using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

public static class BaiVietSearchSchema_cl
{
    private static bool _schemaEnsured;
    private static bool? _hasSearchColumns;
    private static readonly object SyncRoot = new object();
    private static readonly object BackfillLock = new object();
    private static DateTime _lastBackfillUtc = DateTime.MinValue;
    private static readonly string_class SearchStringHelper = new string_class();

    public static void EnsureSchemaSafe(dbDataContext db)
    {
        if (db == null) return;

        try
        {
            EnsureSchema(db);
        }
        catch
        {
            // Không chặn luồng hiển thị nếu schema chưa cập nhật kịp.
            _schemaEnsured = true;
        }
    }

    public static bool HasSearchColumns(dbDataContext db)
    {
        if (db == null) return false;
        if (_hasSearchColumns.HasValue)
            return _hasSearchColumns.Value;

        try
        {
            int flag = db.ExecuteQuery<int>(@"
SELECT CASE
    WHEN COL_LENGTH('dbo.BaiViet_tb', 'name_khongdau') IS NULL THEN 0
    WHEN COL_LENGTH('dbo.BaiViet_tb', 'description_khongdau') IS NULL THEN 0
    ELSE 1
END AS ok").FirstOrDefault();
            _hasSearchColumns = (flag == 1);
            return _hasSearchColumns.Value;
        }
        catch
        {
            _hasSearchColumns = false;
            return false;
        }
    }

    private static void EnsureSchema(dbDataContext db)
    {
        if (_schemaEnsured) return;

        lock (SyncRoot)
        {
            if (_schemaEnsured) return;

            using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
IF COL_LENGTH('dbo.BaiViet_tb', 'name_khongdau') IS NULL
BEGIN
    ALTER TABLE dbo.BaiViet_tb ADD name_khongdau NVARCHAR(MAX) NULL;
END;
IF COL_LENGTH('dbo.BaiViet_tb', 'description_khongdau') IS NULL
BEGIN
    ALTER TABLE dbo.BaiViet_tb ADD description_khongdau NVARCHAR(MAX) NULL;
END;";

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            _schemaEnsured = true;
            _hasSearchColumns = true;
        }
    }

    public static string NormalizeText(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";

        string text = (input ?? "").Trim().ToLowerInvariant();
        text = SearchStringHelper.remove_vietnamchar(text);
        text = Regex.Replace(text, @"[^a-z0-9\s]+", " ");
        text = Regex.Replace(text, @"\s+", " ").Trim();
        return text;
    }

    public static int BackfillMissing(dbDataContext db, int batchSize = 500)
    {
        return BackfillMissing(db, batchSize, false);
    }

    public static int BackfillMissing(dbDataContext db, int batchSize, bool ignoreThrottle)
    {
        if (db == null) return 0;
        if (batchSize <= 0) batchSize = 500;

        if (!HasSearchColumns(db))
            return 0;

        EnsureSchemaSafe(db);

        if (!ignoreThrottle)
        {
            lock (BackfillLock)
            {
                if ((DateTime.UtcNow - _lastBackfillUtc).TotalSeconds < 30)
                    return 0;
                _lastBackfillUtc = DateTime.UtcNow;
            }
        }

        List<BaiViet_tb> missing = db.BaiViet_tbs
            .Where(p =>
                p.name_khongdau == null || p.name_khongdau == ""
                || p.description_khongdau == null || p.description_khongdau == "")
            .OrderBy(p => p.id)
            .Take(batchSize)
            .ToList();

        if (missing.Count == 0) return 0;

        foreach (BaiViet_tb item in missing)
        {
            if (string.IsNullOrWhiteSpace(item.name_khongdau))
                item.name_khongdau = NormalizeText(item.name);
            if (string.IsNullOrWhiteSpace(item.description_khongdau))
                item.description_khongdau = NormalizeText(item.description);
        }

        db.SubmitChanges();
        return missing.Count;
    }

    public static int BackfillAll(dbDataContext db, int batchSize = 2000, int maxBatches = 0)
    {
        if (db == null) return 0;
        if (batchSize <= 0) batchSize = 2000;

        int total = 0;
        int batches = 0;
        while (true)
        {
            int updated = BackfillMissing(db, batchSize, true);
            total += updated;
            batches++;
            if (updated <= 0) break;
            if (maxBatches > 0 && batches >= maxBatches) break;
        }
        return total;
    }

    public static int CountMissing(dbDataContext db)
    {
        if (db == null) return 0;
        if (!HasSearchColumns(db))
            return 0;
        EnsureSchemaSafe(db);
        return db.BaiViet_tbs.Count(p =>
            p.name_khongdau == null || p.name_khongdau == ""
            || p.description_khongdau == null || p.description_khongdau == "");
    }
}
