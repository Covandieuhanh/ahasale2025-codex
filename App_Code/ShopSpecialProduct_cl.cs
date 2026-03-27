using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

public static class ShopSpecialProduct_cl
{
    public const string TableName = "CoreShopSpecialProduct_tb";
    public const string HandlerIssueCard = "issue_card";
    public const string StatusActive = "active";
    public const string StatusInactive = "inactive";

    private sealed class SpecialProductRow
    {
        public long Id { get; set; }
        public string AccountKey { get; set; }
        public int ShopPostId { get; set; }
        public int? SystemProductId { get; set; }
        public string HandlerCode { get; set; }
        public string HandlerConfig { get; set; }
        public string HandlerStatus { get; set; }
        public string Note { get; set; }
    }

    public sealed class SaleContext
    {
        public string SellerAccount { get; set; }
        public BaiViet_tb ProductPost { get; set; }
        public taikhoan_tb BuyerAccount { get; set; }
        public long SaleHistoryId { get; set; }
        public int Quantity { get; set; }
        public DateTime Now { get; set; }
        public string Actor { get; set; }
    }

    public sealed class ExecutionResult
    {
        public bool Applied { get; set; }
        public string HandlerCode { get; set; }
        public string Summary { get; set; }
        public int CardType { get; set; }
        public string TraceData { get; set; }
    }

    public sealed class ProductHandlerAssignment
    {
        public int ShopPostId { get; set; }
        public int? SystemProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string HandlerCode { get; set; }
        public string HandlerConfig { get; set; }
        public string HandlerStatus { get; set; }
        public string HandlerLabel { get; set; }
        public int CardType { get; set; }
        public bool ResetPin { get; set; }
        public bool IsHandlerActive { get; set; }
    }

    public static int CountActiveHandlers(dbDataContext db, string accountKey)
    {
        if (db == null || !HasTable(db))
            return 0;

        string seller = NormalizeAccount(accountKey);
        if (seller == "")
            return 0;

        try
        {
            return db.ExecuteQuery<int>(
                "SELECT COUNT(1) FROM dbo." + TableName + " WHERE AccountKey = {0} AND HandlerStatus = {1}",
                seller,
                StatusActive).FirstOrDefault();
        }
        catch
        {
            return 0;
        }
    }

    public static List<ProductHandlerAssignment> GetProductAssignments(dbDataContext db, string accountKey)
    {
        List<ProductHandlerAssignment> items = new List<ProductHandlerAssignment>();
        if (db == null)
            return items;

        string seller = NormalizeAccount(accountKey);
        if (seller == "")
            return items;

        List<BaiViet_tb> posts = db.BaiViet_tbs
            .Where(p => p.nguoitao == seller)
            .Where(p => p.phanloai == CompanyShop_cl.ProductTypePublic || p.phanloai == CompanyShop_cl.ProductTypeInternal)
            .OrderByDescending(p => p.ngaytao)
            .ThenByDescending(p => p.id)
            .ToList();

        if (posts.Count == 0)
            return items;

        Dictionary<int, SpecialProductRow> rowByPostId = new Dictionary<int, SpecialProductRow>();
        if (HasTable(db))
        {
            List<SpecialProductRow> rows = db.ExecuteQuery<SpecialProductRow>(
                    "SELECT Id, AccountKey, ShopPostId, SystemProductId, HandlerCode, HandlerConfig, HandlerStatus, Note FROM dbo." + TableName + " WHERE AccountKey = {0} ORDER BY Id DESC",
                    seller)
                .ToList();

            rowByPostId = rows
                .Where(p => p != null)
                .GroupBy(p => p.ShopPostId)
                .ToDictionary(p => p.Key, p => p.First());
        }

        for (int i = 0; i < posts.Count; i++)
        {
            BaiViet_tb post = posts[i];
            SpecialProductRow row = rowByPostId.ContainsKey(post.id) ? rowByPostId[post.id] : null;
            Dictionary<string, string> config = ParseHandlerConfig(row == null ? "" : row.HandlerConfig);
            string handlerCode = NormalizeHandlerCode(row == null ? "" : row.HandlerCode);
            string handlerStatus = NormalizeHandlerStatus(row == null ? "" : row.HandlerStatus);
            bool active = handlerCode != "" && handlerStatus == StatusActive;

            ProductHandlerAssignment item = new ProductHandlerAssignment();
            item.ShopPostId = post.id;
            item.SystemProductId = row == null ? null : row.SystemProductId;
            item.ProductName = (post.name ?? ("SP#" + post.id)).Trim();
            item.ProductType = CompanyShop_cl.BuildProductTypeLabel(post.phanloai);
            item.HandlerCode = active ? handlerCode : "";
            item.HandlerConfig = row == null ? "" : (row.HandlerConfig ?? "");
            item.HandlerStatus = handlerStatus;
            item.HandlerLabel = BuildHandlerLabel(item.HandlerCode);
            item.CardType = ParseInt(GetConfigValue(config, "card_type"), CardIssuance_cl.CardTypeTieuDung);
            item.ResetPin = ParseBool(GetConfigValue(config, "reset_pin"), true);
            item.IsHandlerActive = active;
            items.Add(item);
        }

        return items;
    }

    public static void SaveProductHandler(
        dbDataContext db,
        string accountKey,
        int shopPostId,
        int? systemProductId,
        string handlerCode,
        int cardType,
        bool resetPin,
        string actor)
    {
        if (db == null)
            throw new ArgumentNullException("db");

        string handler = NormalizeHandlerCode(handlerCode);
        if (handler == "")
        {
            DisableHandler(db, accountKey, shopPostId, actor);
            return;
        }

        string config;
        switch (handler)
        {
            case HandlerIssueCard:
                config = BuildIssueCardConfig(cardType, resetPin);
                break;
            default:
                throw new InvalidOperationException("Handler sản phẩm đặc biệt chưa được hỗ trợ: " + handlerCode);
        }

        UpsertHandler(
            db,
            accountKey,
            shopPostId,
            systemProductId,
            handler,
            config,
            actor,
            "Manual configuration",
            true);
    }

    public static void DisableHandler(dbDataContext db, string accountKey, int shopPostId, string actor)
    {
        if (db == null)
            throw new ArgumentNullException("db");
        if (!HasTable(db))
            return;

        string seller = NormalizeAccount(accountKey);
        if (seller == "" || shopPostId <= 0)
            return;

        string actorKey = (actor ?? "").Trim();
        if (actorKey == "")
            actorKey = "system";

        db.ExecuteCommand(
            "UPDATE dbo." + TableName + " SET HandlerStatus = {2}, UpdatedBy = {3}, UpdatedAt = {4} WHERE AccountKey = {0} AND ShopPostId = {1}",
            seller,
            shopPostId,
            StatusInactive,
            actorKey,
            AhaTime_cl.Now);
    }

    public static string BuildIssueCardConfig(int cardType, bool resetPin)
    {
        return "card_type=" + cardType + ";reset_pin=" + (resetPin ? "true" : "false");
    }

    public static string BuildHandlerLabel(string handlerCode)
    {
        string handler = NormalizeHandlerCode(handlerCode);
        switch (handler)
        {
            case HandlerIssueCard:
                return "Phát hành thẻ";
            default:
                return "Không áp dụng";
        }
    }

    public static void EnsureLegacyDefaultHandlerForMirroredProduct(
        dbDataContext db,
        string accountKey,
        int systemProductId,
        int shopPostId,
        string productName,
        string actor)
    {
        if (db == null)
            return;
        if (shopPostId <= 0)
            return;
        if (!HasTable(db))
            return;

        string handlerCode = ResolveLegacyDefaultHandlerCode(productName);
        if (handlerCode == "")
            return;

        string seller = NormalizeAccount(accountKey);
        if (seller == "")
            return;

        if (HasAnyHandlerRecord(db, seller, shopPostId, systemProductId))
            return;

        string handlerConfig = BuildDefaultHandlerConfig(handlerCode);
        string note = "Legacy bootstrap: " + (productName ?? ("SP#" + systemProductId));
        UpsertHandler(db, seller, shopPostId, systemProductId, handlerCode, handlerConfig, actor, note, true);
    }

    public static ExecutionResult ExecuteForSale(dbDataContext db, SaleContext context)
    {
        ExecutionResult result = new ExecutionResult();
        if (db == null || context == null || context.ProductPost == null)
            return result;
        if (!HasTable(db))
            return result;

        string seller = NormalizeAccount(context.SellerAccount);
        if (seller == "")
            seller = NormalizeAccount(context.ProductPost.nguoitao);
        if (seller == "")
            return result;

        SpecialProductRow row = GetActiveHandler(db, seller, context.ProductPost.id);
        if (row == null)
            return result;

        string handlerCode = NormalizeHandlerCode(row.HandlerCode);
        result.HandlerCode = handlerCode;

        switch (handlerCode)
        {
            case HandlerIssueCard:
                ExecuteIssueCard(db, context, row, result);
                break;
            default:
                throw new InvalidOperationException("Handler sản phẩm đặc biệt chưa được hỗ trợ: " + (row.HandlerCode ?? ""));
        }

        if (result.Applied)
        {
            ShopSpecialExecution_cl.RecordSuccess(
                db,
                context.SaleHistoryId,
                seller,
                context.ProductPost.id,
                handlerCode,
                result.Summary,
                result.TraceData);
        }

        return result;
    }

    public static void UpsertHandler(
        dbDataContext db,
        string accountKey,
        int shopPostId,
        int? systemProductId,
        string handlerCode,
        string handlerConfig,
        string actor,
        string note,
        bool isActive)
    {
        if (db == null)
            throw new ArgumentNullException("db");
        if (shopPostId <= 0)
            throw new InvalidOperationException("ShopPostId không hợp lệ.");

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        if (!HasTable(db))
            return;

        string seller = NormalizeAccount(accountKey);
        if (seller == "")
            throw new InvalidOperationException("AccountKey không hợp lệ.");

        string normalizedHandler = NormalizeHandlerCode(handlerCode);
        if (normalizedHandler == "")
            throw new InvalidOperationException("HandlerCode không hợp lệ.");

        string actorKey = (actor ?? "").Trim();
        if (actorKey == "")
            actorKey = "system";

        string status = isActive ? StatusActive : StatusInactive;
        string config = (handlerConfig ?? "").Trim();
        string memo = (note ?? "").Trim();
        DateTime now = AhaTime_cl.Now;

        int updated;
        if (systemProductId.HasValue)
        {
            updated = db.ExecuteCommand(
                "UPDATE dbo." + TableName + " SET SystemProductId = {2}, HandlerCode = {3}, HandlerConfig = {4}, HandlerStatus = {5}, Note = {6}, UpdatedBy = {7}, UpdatedAt = {8} WHERE AccountKey = {0} AND ShopPostId = {1}",
                seller,
                shopPostId,
                systemProductId.Value,
                normalizedHandler,
                config,
                status,
                memo,
                actorKey,
                now);
        }
        else
        {
            updated = db.ExecuteCommand(
                "UPDATE dbo." + TableName + " SET SystemProductId = NULL, HandlerCode = {2}, HandlerConfig = {3}, HandlerStatus = {4}, Note = {5}, UpdatedBy = {6}, UpdatedAt = {7} WHERE AccountKey = {0} AND ShopPostId = {1}",
                seller,
                shopPostId,
                normalizedHandler,
                config,
                status,
                memo,
                actorKey,
                now);
        }

        if (updated > 0)
            return;

        if (systemProductId.HasValue)
        {
            db.ExecuteCommand(
                "INSERT INTO dbo." + TableName + " (AccountKey, ShopPostId, SystemProductId, HandlerCode, HandlerConfig, HandlerStatus, Note, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {7}, {8}, {8})",
                seller,
                shopPostId,
                systemProductId.Value,
                normalizedHandler,
                config,
                status,
                memo,
                actorKey,
                now);
        }
        else
        {
            db.ExecuteCommand(
                "INSERT INTO dbo." + TableName + " (AccountKey, ShopPostId, SystemProductId, HandlerCode, HandlerConfig, HandlerStatus, Note, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt) VALUES ({0}, {1}, NULL, {2}, {3}, {4}, {5}, {6}, {6}, {7}, {7})",
                seller,
                shopPostId,
                normalizedHandler,
                config,
                status,
                memo,
                actorKey,
                now);
        }
    }

    private static void ExecuteIssueCard(
        dbDataContext db,
        SaleContext context,
        SpecialProductRow row,
        ExecutionResult result)
    {
        if (context.BuyerAccount == null || string.IsNullOrWhiteSpace(context.BuyerAccount.taikhoan))
            throw new InvalidOperationException("Không xác định được tài khoản nhận sản phẩm đặc biệt.");

        Dictionary<string, string> config = ParseHandlerConfig(row == null ? "" : row.HandlerConfig);
        int cardType = ParseInt(GetConfigValue(config, "card_type"), CardIssuance_cl.CardTypeTieuDung);
        bool resetPin = ParseBool(GetConfigValue(config, "reset_pin"), true);
        string oldPin = (context.BuyerAccount.mapin_thanhtoan ?? "").Trim();

        CardIssuance_cl.IssueCard(
            db,
            context.BuyerAccount,
            cardType,
            context.Actor,
            context.Now);

        if (!resetPin && oldPin != "")
            context.BuyerAccount.mapin_thanhtoan = oldPin;

        result.Applied = true;
        result.CardType = cardType;
        result.Summary = CardIssuance_cl.GetCardName(cardType)
            + " đã được kích hoạt tự động cho tài khoản "
            + context.BuyerAccount.taikhoan
            + ".";
        result.TraceData = "buyer=" + context.BuyerAccount.taikhoan
            + ";card_type=" + cardType
            + ";card_name=" + CardIssuance_cl.GetCardName(cardType)
            + ";reset_pin=" + (resetPin ? "true" : "false")
            + ";quantity=" + context.Quantity;
    }

    private static SpecialProductRow GetActiveHandler(dbDataContext db, string accountKey, int shopPostId)
    {
        if (db == null || !HasTable(db))
            return null;

        string seller = NormalizeAccount(accountKey);
        if (seller == "" || shopPostId <= 0)
            return null;

        try
        {
            return db.ExecuteQuery<SpecialProductRow>(
                "SELECT TOP 1 Id, AccountKey, ShopPostId, SystemProductId, HandlerCode, HandlerConfig, HandlerStatus, Note FROM dbo." + TableName + " WHERE AccountKey = {0} AND ShopPostId = {1} AND HandlerStatus = {2} ORDER BY Id DESC",
                seller,
                shopPostId,
                StatusActive).FirstOrDefault();
        }
        catch
        {
            return null;
        }
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

    private static bool HasAnyHandlerRecord(dbDataContext db, string accountKey, int shopPostId, int systemProductId)
    {
        if (db == null || !HasTable(db))
            return false;
        if (shopPostId <= 0)
            return false;

        string seller = NormalizeAccount(accountKey);
        if (seller == "")
            return false;

        try
        {
            int count = db.ExecuteQuery<int>(
                "SELECT COUNT(1) FROM dbo." + TableName + " WHERE AccountKey = {0} AND (ShopPostId = {1} OR (SystemProductId IS NOT NULL AND SystemProductId = {2}))",
                seller,
                shopPostId,
                systemProductId).FirstOrDefault();
            return count > 0;
        }
        catch
        {
            return false;
        }
    }

    private static string ResolveLegacyDefaultHandlerCode(string productName)
    {
        string normalized = NormalizeText(productName)
            .Replace('-', ' ')
            .Replace('_', ' ')
            .Trim();

        if (normalized == "the"
            || normalized.StartsWith("the ", StringComparison.Ordinal)
            || normalized.EndsWith(" the", StringComparison.Ordinal)
            || normalized.IndexOf(" the ", StringComparison.Ordinal) >= 0)
        {
            return HandlerIssueCard;
        }

        return "";
    }

    private static string BuildDefaultHandlerConfig(string handlerCode)
    {
        string normalized = NormalizeHandlerCode(handlerCode);
        if (normalized == HandlerIssueCard)
            return BuildIssueCardConfig(CardIssuance_cl.CardTypeTieuDung, true);
        return "";
    }

    private static Dictionary<string, string> ParseHandlerConfig(string raw)
    {
        Dictionary<string, string> values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string text = (raw ?? "").Trim();
        if (text == "")
            return values;

        string[] parts = text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < parts.Length; i++)
        {
            string item = (parts[i] ?? "").Trim();
            if (item == "")
                continue;

            int splitIndex = item.IndexOf('=');
            if (splitIndex <= 0)
                continue;

            string key = item.Substring(0, splitIndex).Trim();
            string value = item.Substring(splitIndex + 1).Trim();
            if (key == "")
                continue;

            values[key] = value;
        }

        return values;
    }

    private static string GetConfigValue(Dictionary<string, string> config, string key)
    {
        if (config == null || string.IsNullOrWhiteSpace(key))
            return "";

        string value;
        if (config.TryGetValue(key, out value))
            return (value ?? "").Trim();
        return "";
    }

    private static bool ParseBool(string raw, bool defaultValue)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        if (value == "")
            return defaultValue;

        if (value == "1" || value == "true" || value == "yes" || value == "on")
            return true;
        if (value == "0" || value == "false" || value == "no" || value == "off")
            return false;

        return defaultValue;
    }

    private static int ParseInt(string raw, int defaultValue)
    {
        int value;
        if (int.TryParse((raw ?? "").Trim(), out value))
            return value;
        return defaultValue;
    }

    private static string NormalizeAccount(string raw)
    {
        return (raw ?? "").Trim().ToLowerInvariant();
    }

    private static string NormalizeHandlerCode(string raw)
    {
        return (raw ?? "").Trim().ToLowerInvariant();
    }

    private static string NormalizeHandlerStatus(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        if (value == "")
            return StatusInactive;
        return value;
    }

    private static string NormalizeText(string raw)
    {
        string text = (raw ?? "").Trim();
        if (text == "")
            return "";

        string normalized = text.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < normalized.Length; i++)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(normalized[i]);
            if (category != UnicodeCategory.NonSpacingMark)
                sb.Append(normalized[i]);
        }

        return sb.ToString()
            .Normalize(NormalizationForm.FormC)
            .Replace('\u0111', 'd')
            .ToLowerInvariant();
    }
}
