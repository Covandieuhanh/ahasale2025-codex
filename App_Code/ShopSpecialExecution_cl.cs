using System;
using System.Collections.Generic;
using System.Linq;

public static class ShopSpecialExecution_cl
{
    public const string TableName = "CoreShopSpecialExecution_tb";
    public const string StatusSuccess = "success";
    public const string StatusSkipped = "skipped";
    public const string StatusError = "error";

    private sealed class ExecutionRow
    {
        public long Id { get; set; }
        public long SaleHistoryId { get; set; }
        public string AccountKey { get; set; }
        public int ShopPostId { get; set; }
        public string HandlerCode { get; set; }
        public string ExecutionStatus { get; set; }
        public string ExecutionSummary { get; set; }
        public string ExecutionData { get; set; }
        public DateTime ExecutedAt { get; set; }
    }

    public sealed class ExecutionTraceInfo
    {
        public long SaleHistoryId { get; set; }
        public string HandlerCode { get; set; }
        public string HandlerLabel { get; set; }
        public string ExecutionStatus { get; set; }
        public string ExecutionSummary { get; set; }
        public string ExecutionData { get; set; }
        public DateTime ExecutedAt { get; set; }
        public string ExecutedAtText { get; set; }
    }

    public static void RecordSuccess(
        dbDataContext db,
        long saleHistoryId,
        string accountKey,
        int shopPostId,
        string handlerCode,
        string summary,
        string executionData)
    {
        if (db == null)
            throw new ArgumentNullException("db");

        Record(
            db,
            saleHistoryId,
            accountKey,
            shopPostId,
            handlerCode,
            StatusSuccess,
            summary,
            executionData,
            AhaTime_cl.Now);
    }

    public static List<ExecutionTraceInfo> GetBySaleHistoryId(dbDataContext db, long saleHistoryId)
    {
        if (db == null || saleHistoryId <= 0 || !HasTable(db))
            return new List<ExecutionTraceInfo>();

        string sql = "SELECT Id, SaleHistoryId, AccountKey, ShopPostId, HandlerCode, ExecutionStatus, ExecutionSummary, ExecutionData, ExecutedAt FROM dbo."
            + TableName
            + " WHERE SaleHistoryId = "
            + saleHistoryId
            + " ORDER BY ExecutedAt DESC, Id DESC";

        return MapRows(db.ExecuteQuery<ExecutionRow>(sql).ToList());
    }

    public static Dictionary<long, ExecutionTraceInfo> GetLatestMap(dbDataContext db, IEnumerable<long> saleHistoryIds)
    {
        Dictionary<long, ExecutionTraceInfo> map = new Dictionary<long, ExecutionTraceInfo>();
        if (db == null || !HasTable(db))
            return map;

        List<long> ids = NormalizeIds(saleHistoryIds);
        if (ids.Count == 0)
            return map;

        string sql = "SELECT Id, SaleHistoryId, AccountKey, ShopPostId, HandlerCode, ExecutionStatus, ExecutionSummary, ExecutionData, ExecutedAt FROM dbo."
            + TableName
            + " WHERE SaleHistoryId IN ("
            + string.Join(",", ids.Select(p => p.ToString()).ToArray())
            + ") ORDER BY ExecutedAt DESC, Id DESC";

        List<ExecutionTraceInfo> traces = MapRows(db.ExecuteQuery<ExecutionRow>(sql).ToList());
        for (int i = 0; i < traces.Count; i++)
        {
            ExecutionTraceInfo trace = traces[i];
            if (!map.ContainsKey(trace.SaleHistoryId))
                map[trace.SaleHistoryId] = trace;
        }

        return map;
    }

    private static void Record(
        dbDataContext db,
        long saleHistoryId,
        string accountKey,
        int shopPostId,
        string handlerCode,
        string executionStatus,
        string summary,
        string executionData,
        DateTime executedAt)
    {
        if (db == null)
            throw new ArgumentNullException("db");
        if (saleHistoryId <= 0)
            throw new InvalidOperationException("SaleHistoryId không hợp lệ.");

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        if (!HasTable(db))
            return;

        string seller = Normalize(accountKey);
        string handler = Normalize(handlerCode);
        string status = Normalize(executionStatus);
        if (handler == "")
            throw new InvalidOperationException("HandlerCode không hợp lệ.");
        if (status == "")
            status = StatusSuccess;

        string textSummary = (summary ?? "").Trim();
        string textData = (executionData ?? "").Trim();

        int updated = db.ExecuteCommand(
            "UPDATE dbo." + TableName + " SET AccountKey = {1}, ShopPostId = {2}, ExecutionStatus = {4}, ExecutionSummary = {5}, ExecutionData = {6}, ExecutedAt = {7} WHERE SaleHistoryId = {0} AND HandlerCode = {3}",
            saleHistoryId,
            seller,
            shopPostId,
            handler,
            status,
            textSummary,
            textData,
            executedAt);

        if (updated > 0)
            return;

        db.ExecuteCommand(
            "INSERT INTO dbo." + TableName + " (SaleHistoryId, AccountKey, ShopPostId, HandlerCode, ExecutionStatus, ExecutionSummary, ExecutionData, ExecutedAt) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})",
            saleHistoryId,
            seller,
            shopPostId,
            handler,
            status,
            textSummary,
            textData,
            executedAt);
    }

    private static List<ExecutionTraceInfo> MapRows(List<ExecutionRow> rows)
    {
        List<ExecutionTraceInfo> items = new List<ExecutionTraceInfo>();
        if (rows == null || rows.Count == 0)
            return items;

        for (int i = 0; i < rows.Count; i++)
        {
            ExecutionRow row = rows[i];
            if (row == null)
                continue;

            ExecutionTraceInfo item = new ExecutionTraceInfo();
            item.SaleHistoryId = row.SaleHistoryId;
            item.HandlerCode = Normalize(row.HandlerCode);
            item.HandlerLabel = ShopSpecialProduct_cl.BuildHandlerLabel(row.HandlerCode);
            item.ExecutionStatus = Normalize(row.ExecutionStatus);
            item.ExecutionSummary = (row.ExecutionSummary ?? "").Trim();
            item.ExecutionData = (row.ExecutionData ?? "").Trim();
            item.ExecutedAt = row.ExecutedAt;
            item.ExecutedAtText = row.ExecutedAt == DateTime.MinValue
                ? ""
                : row.ExecutedAt.ToString("dd/MM/yyyy HH:mm:ss");
            items.Add(item);
        }

        return items;
    }

    private static List<long> NormalizeIds(IEnumerable<long> saleHistoryIds)
    {
        if (saleHistoryIds == null)
            return new List<long>();

        return saleHistoryIds
            .Where(p => p > 0)
            .Distinct()
            .ToList();
    }

    private static bool HasTable(dbDataContext db)
    {
        if (db == null)
            return false;

        try
        {
            int count = db.ExecuteQuery<int>(
                "SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}",
                TableName).FirstOrDefault();
            return count > 0;
        }
        catch
        {
            return false;
        }
    }

    private static string Normalize(string raw)
    {
        return (raw ?? "").Trim().ToLowerInvariant();
    }
}
