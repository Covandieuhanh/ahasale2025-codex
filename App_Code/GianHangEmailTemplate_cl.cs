using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

public static class GianHangEmailTemplate_cl
{
    private static readonly object SyncRoot = new object();
    private static bool _schemaEnsured;

    public const string CodeOrderCreated = "order_created";
    public const string CodeOrderPaid = "order_paid";
    public const string CodeOrderShipping = "order_shipping";
    public const string CodeOrderDelivered = "order_delivered";
    public const string CodeOrderCancelled = "order_cancelled";
    public const string CodeCustomerMessage = "customer_message";

    public sealed class TemplateItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsBuiltIn { get; set; }
    }

    public sealed class BuiltInTemplate
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }
        public bool IsActive { get; private set; }

        public BuiltInTemplate(string code, string name, string subject, string body, bool isActive = true)
        {
            Code = code;
            Name = name;
            Subject = subject;
            Body = body;
            IsActive = isActive;
        }
    }

    public static List<BuiltInTemplate> GetBuiltInTemplates()
    {
        return new List<BuiltInTemplate>
        {
            new BuiltInTemplate(
                CodeOrderCreated,
                "Gian hàng nhận đơn hàng mới",
                "[AhaSale] Gian hàng có đơn mới {ORDER_CODE}",
                string.Join("\n", new[]
                {
                    "Xin chào {SHOP_NAME},",
                    "",
                    "Gian hàng của bạn vừa nhận được đơn hàng mới {ORDER_CODE} từ {CUSTOMER_NAME}.",
                    "Tổng giá trị: {TOTAL}",
                    "Trạng thái hiện tại: {ORDER_STATUS}",
                    "Xem chi tiết đơn hàng: {ORDER_URL}",
                    "",
                    "AhaSale"
                })),
            new BuiltInTemplate(
                CodeOrderPaid,
                "Đơn hàng gian hàng đã thanh toán",
                "[AhaSale] Đơn hàng {ORDER_CODE} đã thanh toán",
                string.Join("\n", new[]
                {
                    "Xin chào {SHOP_NAME},",
                    "",
                    "Đơn hàng {ORDER_CODE} trong gian hàng đã được thanh toán.",
                    "Tổng giá trị: {TOTAL}",
                    "Khách hàng: {CUSTOMER_NAME}",
                    "Xem chi tiết: {ORDER_URL}",
                    "",
                    "AhaSale"
                })),
            new BuiltInTemplate(
                CodeOrderShipping,
                "Đơn hàng gian hàng đang giao",
                "[AhaSale] Đơn hàng {ORDER_CODE} đang giao",
                string.Join("\n", new[]
                {
                    "Xin chào {SHOP_NAME},",
                    "",
                    "Đơn hàng {ORDER_CODE} của gian hàng đang trong quá trình giao.",
                    "Khách hàng: {CUSTOMER_NAME}",
                    "Xem chi tiết: {ORDER_URL}",
                    "",
                    "AhaSale"
                })),
            new BuiltInTemplate(
                CodeOrderDelivered,
                "Đơn hàng gian hàng giao thành công",
                "[AhaSale] Đơn hàng {ORDER_CODE} đã giao",
                string.Join("\n", new[]
                {
                    "Xin chào {SHOP_NAME},",
                    "",
                    "Đơn hàng {ORDER_CODE} của gian hàng đã giao thành công.",
                    "Khách hàng: {CUSTOMER_NAME}",
                    "Xem chi tiết: {ORDER_URL}",
                    "",
                    "AhaSale"
                })),
            new BuiltInTemplate(
                CodeOrderCancelled,
                "Đơn hàng gian hàng bị hủy",
                "[AhaSale] Đơn hàng {ORDER_CODE} đã bị hủy",
                string.Join("\n", new[]
                {
                    "Xin chào {SHOP_NAME},",
                    "",
                    "Đơn hàng {ORDER_CODE} của gian hàng đã bị hủy.",
                    "Khách hàng: {CUSTOMER_NAME}",
                    "Xem chi tiết: {ORDER_URL}",
                    "",
                    "AhaSale"
                })),
            new BuiltInTemplate(
                CodeCustomerMessage,
                "Khách hàng gửi tin nhắn cho gian hàng",
                "[AhaSale] Tin nhắn mới từ {CUSTOMER_NAME}",
                string.Join("\n", new[]
                {
                    "Xin chào {SHOP_NAME},",
                    "",
                    "Bạn vừa nhận được tin nhắn mới cho gian hàng từ {CUSTOMER_NAME}.",
                    "Nội dung: {MESSAGE}",
                    "Xem chi tiết: {ORDER_URL}",
                    "",
                    "AhaSale"
                }))
        };
    }

    public static bool IsValidCode(string code)
    {
        string normalized = NormalizeCode(code);
        if (string.IsNullOrEmpty(normalized)) return false;
        return Regex.IsMatch(normalized, @"^[a-z0-9_]{3,80}$");
    }

    public static string NormalizeCode(string code)
    {
        return (code ?? "").Trim().ToLowerInvariant();
    }

    public static List<TemplateItem> GetAll(dbDataContext db)
    {
        EnsureSchema(db);

        var builtInMap = GetBuiltInTemplates().ToDictionary(x => x.Code, x => x, StringComparer.OrdinalIgnoreCase);
        var rows = new List<TemplateItem>();

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT template_code, display_name, subject, body_text, is_active, updated_by, updated_at
FROM dbo.GianHang_Email_Template_tb
ORDER BY template_code";

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string code = (reader["template_code"] as string ?? "").Trim();
                    rows.Add(new TemplateItem
                    {
                        Code = code,
                        Name = (reader["display_name"] as string ?? "").Trim(),
                        Subject = (reader["subject"] as string ?? "").Trim(),
                        Body = reader["body_text"] as string ?? "",
                        IsActive = reader["is_active"] != DBNull.Value && Convert.ToBoolean(reader["is_active"]),
                        UpdatedBy = (reader["updated_by"] as string ?? "").Trim(),
                        UpdatedAt = reader["updated_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["updated_at"]),
                        IsBuiltIn = builtInMap.ContainsKey(code)
                    });
                }
            }
        }

        foreach (BuiltInTemplate builtIn in GetBuiltInTemplates())
        {
            if (rows.Any(x => string.Equals(x.Code, builtIn.Code, StringComparison.OrdinalIgnoreCase)))
                continue;

            rows.Add(new TemplateItem
            {
                Code = builtIn.Code,
                Name = builtIn.Name,
                Subject = builtIn.Subject,
                Body = builtIn.Body,
                IsActive = builtIn.IsActive,
                UpdatedBy = "",
                UpdatedAt = null,
                IsBuiltIn = true
            });
        }

        return rows
            .OrderByDescending(x => x.IsBuiltIn)
            .ThenBy(x => x.Code)
            .ToList();
    }

    public static TemplateItem GetByCode(dbDataContext db, string code)
    {
        EnsureSchema(db);

        string key = NormalizeCode(code);
        if (!IsValidCode(key)) return null;

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT TOP 1 template_code, display_name, subject, body_text, is_active, updated_by, updated_at
FROM dbo.GianHang_Email_Template_tb
WHERE template_code = @code";
            cmd.Parameters.AddWithValue("@code", key);

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new TemplateItem
                    {
                        Code = (reader["template_code"] as string ?? "").Trim(),
                        Name = (reader["display_name"] as string ?? "").Trim(),
                        Subject = (reader["subject"] as string ?? "").Trim(),
                        Body = reader["body_text"] as string ?? "",
                        IsActive = reader["is_active"] != DBNull.Value && Convert.ToBoolean(reader["is_active"]),
                        UpdatedBy = (reader["updated_by"] as string ?? "").Trim(),
                        UpdatedAt = reader["updated_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["updated_at"]),
                        IsBuiltIn = true
                    };
                }
            }
        }

        BuiltInTemplate builtInItem = GetBuiltInTemplates()
            .FirstOrDefault(x => string.Equals(x.Code, key, StringComparison.OrdinalIgnoreCase));
        if (builtInItem == null) return null;

        return new TemplateItem
        {
            Code = builtInItem.Code,
            Name = builtInItem.Name,
            Subject = builtInItem.Subject,
            Body = builtInItem.Body,
            IsActive = builtInItem.IsActive,
            UpdatedBy = "",
            UpdatedAt = null,
            IsBuiltIn = true
        };
    }

    public static TemplateItem GetEffectiveByCode(dbDataContext db, string code)
    {
        return GetByCode(db, code);
    }

    public static void Upsert(dbDataContext db, string code, string name, string subject, string body, bool isActive, string updatedBy)
    {
        EnsureSchema(db);

        string key = NormalizeCode(code);
        if (!IsValidCode(key))
            throw new InvalidOperationException("Mã template không hợp lệ.");

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
IF EXISTS (SELECT 1 FROM dbo.GianHang_Email_Template_tb WHERE template_code = @code)
BEGIN
    UPDATE dbo.GianHang_Email_Template_tb
        SET display_name = @name,
            subject = @subject,
            body_text = @body,
            is_active = @is_active,
            updated_by = @updated_by,
            updated_at = GETDATE()
    WHERE template_code = @code;
END
ELSE
BEGIN
    INSERT INTO dbo.GianHang_Email_Template_tb
        (template_code, display_name, subject, body_text, is_active, updated_by, updated_at, created_at)
    VALUES
        (@code, @name, @subject, @body, @is_active, @updated_by, GETDATE(), GETDATE());
END;";

            cmd.Parameters.AddWithValue("@code", key);
            cmd.Parameters.AddWithValue("@name", name ?? "");
            cmd.Parameters.AddWithValue("@subject", subject ?? "");
            cmd.Parameters.AddWithValue("@body", body ?? "");
            cmd.Parameters.AddWithValue("@is_active", isActive);
            cmd.Parameters.AddWithValue("@updated_by", updatedBy ?? "");

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public static bool ResetToBuiltIn(dbDataContext db, string code, string updatedBy)
    {
        EnsureSchema(db);

        string key = NormalizeCode(code);
        if (!IsValidCode(key)) return false;

        BuiltInTemplate builtIn = GetBuiltInTemplates()
            .FirstOrDefault(x => string.Equals(x.Code, key, StringComparison.OrdinalIgnoreCase));
        if (builtIn == null) return false;

        Upsert(db, builtIn.Code, builtIn.Name, builtIn.Subject, builtIn.Body, builtIn.IsActive, updatedBy);
        return true;
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
IF OBJECT_ID('dbo.GianHang_Email_Template_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.GianHang_Email_Template_tb
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        template_code NVARCHAR(80) NOT NULL,
        display_name NVARCHAR(200) NOT NULL,
        subject NVARCHAR(250) NOT NULL,
        body_text NVARCHAR(MAX) NULL,
        is_active BIT NOT NULL CONSTRAINT DF_GianHang_Email_Template_tb_is_active DEFAULT(1),
        updated_by NVARCHAR(50) NULL,
        updated_at DATETIME NULL,
        created_at DATETIME NOT NULL CONSTRAINT DF_GianHang_Email_Template_tb_created_at DEFAULT(GETDATE())
    );
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_GianHang_Email_Template_tb_code'
      AND object_id = OBJECT_ID(N'dbo.GianHang_Email_Template_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_GianHang_Email_Template_tb_code
        ON dbo.GianHang_Email_Template_tb(template_code);
END;";
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            SeedBuiltInIfMissing(db);
            _schemaEnsured = true;
        }
    }

    private static void SeedBuiltInIfMissing(dbDataContext db)
    {
        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            conn.Open();
            foreach (BuiltInTemplate builtIn in GetBuiltInTemplates())
            {
                cmd.Parameters.Clear();
                cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM dbo.GianHang_Email_Template_tb WHERE template_code = @code)
BEGIN
    INSERT INTO dbo.GianHang_Email_Template_tb
        (template_code, display_name, subject, body_text, is_active, updated_by, updated_at, created_at)
    VALUES
        (@code, @name, @subject, @body, @is_active, 'system_seed', GETDATE(), GETDATE());
END;";
                cmd.Parameters.AddWithValue("@code", builtIn.Code);
                cmd.Parameters.AddWithValue("@name", builtIn.Name);
                cmd.Parameters.AddWithValue("@subject", builtIn.Subject);
                cmd.Parameters.AddWithValue("@body", builtIn.Body);
                cmd.Parameters.AddWithValue("@is_active", builtIn.IsActive);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
