using System;
using System.Data;
using System.Data.SqlClient;

public class SmsOtpConfig
{
    public string Endpoint;
    public string ApiKey;
    public string Sender;
    public string Template;
    public string Method;
    public string Params;
    public bool DevMode;
    public DateTime? UpdatedAt;
    public string UpdatedBy;
}

public class EmailOtpConfig
{
    public string Host;
    public int Port;
    public string Username;
    public string Password;
    public string FromName;
    public string FromEmail;
    public string SubjectTemplate;
    public string BodyTemplate;
    public bool UseSsl;
    public bool DevMode;
    public DateTime? UpdatedAt;
    public string UpdatedBy;
}

public static class OtpConfig_cl
{
    private static readonly object SyncRoot = new object();
    private static bool _schemaEnsured;

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
IF OBJECT_ID('dbo.Otp_Config_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Otp_Config_tb
    (
        id INT NOT NULL PRIMARY KEY,
        sms_endpoint NVARCHAR(500) NULL,
        sms_apikey NVARCHAR(200) NULL,
        sms_sender NVARCHAR(100) NULL,
        sms_template NVARCHAR(500) NULL,
        sms_method NVARCHAR(10) NULL,
        sms_params NVARCHAR(MAX) NULL,
        sms_dev_mode BIT NOT NULL CONSTRAINT DF_Otp_Config_tb_sms_dev_mode DEFAULT(0),
        email_host NVARCHAR(200) NULL,
        email_port INT NULL,
        email_username NVARCHAR(200) NULL,
        email_password NVARCHAR(200) NULL,
        email_from_name NVARCHAR(200) NULL,
        email_from_address NVARCHAR(200) NULL,
        email_subject NVARCHAR(300) NULL,
        email_body NVARCHAR(MAX) NULL,
        email_ssl BIT NOT NULL CONSTRAINT DF_Otp_Config_tb_email_ssl DEFAULT(1),
        email_dev_mode BIT NOT NULL CONSTRAINT DF_Otp_Config_tb_email_dev_mode DEFAULT(0),
        updated_by NVARCHAR(50) NULL,
        updated_at DATETIME NULL
    );
    INSERT INTO dbo.Otp_Config_tb (id, sms_dev_mode) VALUES (1, 0);
END;

IF COL_LENGTH('dbo.Otp_Config_tb', 'sms_method') IS NULL
    ALTER TABLE dbo.Otp_Config_tb ADD sms_method NVARCHAR(10) NULL;
IF COL_LENGTH('dbo.Otp_Config_tb', 'sms_params') IS NULL
    ALTER TABLE dbo.Otp_Config_tb ADD sms_params NVARCHAR(MAX) NULL;
IF COL_LENGTH('dbo.Otp_Config_tb', 'email_host') IS NULL
    ALTER TABLE dbo.Otp_Config_tb ADD email_host NVARCHAR(200) NULL;
IF COL_LENGTH('dbo.Otp_Config_tb', 'email_port') IS NULL
    ALTER TABLE dbo.Otp_Config_tb ADD email_port INT NULL;
IF COL_LENGTH('dbo.Otp_Config_tb', 'email_username') IS NULL
    ALTER TABLE dbo.Otp_Config_tb ADD email_username NVARCHAR(200) NULL;
IF COL_LENGTH('dbo.Otp_Config_tb', 'email_password') IS NULL
    ALTER TABLE dbo.Otp_Config_tb ADD email_password NVARCHAR(200) NULL;
IF COL_LENGTH('dbo.Otp_Config_tb', 'email_from_name') IS NULL
    ALTER TABLE dbo.Otp_Config_tb ADD email_from_name NVARCHAR(200) NULL;
IF COL_LENGTH('dbo.Otp_Config_tb', 'email_from_address') IS NULL
    ALTER TABLE dbo.Otp_Config_tb ADD email_from_address NVARCHAR(200) NULL;
IF COL_LENGTH('dbo.Otp_Config_tb', 'email_subject') IS NULL
    ALTER TABLE dbo.Otp_Config_tb ADD email_subject NVARCHAR(300) NULL;
IF COL_LENGTH('dbo.Otp_Config_tb', 'email_body') IS NULL
    ALTER TABLE dbo.Otp_Config_tb ADD email_body NVARCHAR(MAX) NULL;
IF COL_LENGTH('dbo.Otp_Config_tb', 'email_ssl') IS NULL
    ALTER TABLE dbo.Otp_Config_tb ADD email_ssl BIT NOT NULL CONSTRAINT DF_Otp_Config_tb_email_ssl DEFAULT(1);
IF COL_LENGTH('dbo.Otp_Config_tb', 'email_dev_mode') IS NULL
    ALTER TABLE dbo.Otp_Config_tb ADD email_dev_mode BIT NOT NULL CONSTRAINT DF_Otp_Config_tb_email_dev_mode DEFAULT(0);";
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            _schemaEnsured = true;
        }
    }

    public static SmsOtpConfig GetSmsConfig()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                return GetSmsConfig(db);
            }
        }
        catch
        {
            return null;
        }
    }

    public static SmsOtpConfig GetSmsConfig(dbDataContext db)
    {
        if (db == null) return null;
        EnsureSchema(db);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT TOP 1 sms_endpoint, sms_apikey, sms_sender, sms_template, sms_method, sms_params, sms_dev_mode, updated_by, updated_at
FROM dbo.Otp_Config_tb WHERE id = 1";
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                if (!reader.Read()) return null;

                return new SmsOtpConfig
                {
                    Endpoint = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    ApiKey = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Sender = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Template = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Method = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Params = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    DevMode = !reader.IsDBNull(6) && reader.GetBoolean(6),
                    UpdatedBy = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    UpdatedAt = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8)
                };
            }
        }
    }

    public static EmailOtpConfig GetEmailConfig(dbDataContext db)
    {
        if (db == null) return null;
        EnsureSchema(db);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT TOP 1 email_host, email_port, email_username, email_password,
       email_from_name, email_from_address, email_subject, email_body, email_ssl, email_dev_mode,
       updated_by, updated_at
FROM dbo.Otp_Config_tb WHERE id = 1";
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                if (!reader.Read()) return null;

                return new EmailOtpConfig
                {
                    Host = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    Port = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                    Username = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Password = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    FromName = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    FromEmail = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    SubjectTemplate = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    BodyTemplate = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    UseSsl = !reader.IsDBNull(8) && reader.GetBoolean(8),
                    DevMode = !reader.IsDBNull(9) && reader.GetBoolean(9),
                    UpdatedBy = reader.IsDBNull(10) ? "" : reader.GetString(10),
                    UpdatedAt = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11)
                };
            }
        }
    }

    public static void SaveEmailConfig(dbDataContext db, EmailOtpConfig config, string updatedBy)
    {
        if (db == null || config == null) return;
        EnsureSchema(db);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
IF EXISTS (SELECT 1 FROM dbo.Otp_Config_tb WHERE id = 1)
BEGIN
    UPDATE dbo.Otp_Config_tb
    SET email_host = @host,
        email_port = @port,
        email_username = @username,
        email_password = @password,
        email_from_name = @from_name,
        email_from_address = @from_address,
        email_subject = @subject,
        email_body = @body,
        email_ssl = @ssl,
        email_dev_mode = @devmode,
        updated_by = @updated_by,
        updated_at = GETDATE()
    WHERE id = 1;
END
ELSE
BEGIN
    INSERT INTO dbo.Otp_Config_tb
        (id, email_host, email_port, email_username, email_password, email_from_name, email_from_address,
         email_subject, email_body, email_ssl, email_dev_mode, updated_by, updated_at)
    VALUES
        (1, @host, @port, @username, @password, @from_name, @from_address, @subject, @body, @ssl, @devmode, @updated_by, GETDATE());
END;";
            cmd.Parameters.AddWithValue("@host", (object)(config.Host ?? "") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@port", config.Port);
            cmd.Parameters.AddWithValue("@username", (object)(config.Username ?? "") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@password", (object)(config.Password ?? "") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@from_name", (object)(config.FromName ?? "") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@from_address", (object)(config.FromEmail ?? "") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@subject", (object)(config.SubjectTemplate ?? "") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@body", (object)(config.BodyTemplate ?? "") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ssl", config.UseSsl ? 1 : 0);
            cmd.Parameters.AddWithValue("@devmode", config.DevMode ? 1 : 0);
            cmd.Parameters.AddWithValue("@updated_by", (object)(updatedBy ?? "") ?? DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public static void SaveSmsConfig(dbDataContext db, SmsOtpConfig config, string updatedBy)
    {
        if (db == null || config == null) return;
        EnsureSchema(db);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
IF EXISTS (SELECT 1 FROM dbo.Otp_Config_tb WHERE id = 1)
BEGIN
    UPDATE dbo.Otp_Config_tb
    SET sms_endpoint = @endpoint,
        sms_apikey = @apikey,
        sms_sender = @sender,
        sms_template = @template,
        sms_method = @method,
        sms_params = @params,
        sms_dev_mode = @devmode,
        updated_by = @updated_by,
        updated_at = GETDATE()
    WHERE id = 1;
END
ELSE
BEGIN
    INSERT INTO dbo.Otp_Config_tb (id, sms_endpoint, sms_apikey, sms_sender, sms_template, sms_method, sms_params, sms_dev_mode, updated_by, updated_at)
    VALUES (1, @endpoint, @apikey, @sender, @template, @method, @params, @devmode, @updated_by, GETDATE());
END;";
            cmd.Parameters.AddWithValue("@endpoint", (object)(config.Endpoint ?? "") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@apikey", (object)(config.ApiKey ?? "") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@sender", (object)(config.Sender ?? "") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@template", (object)(config.Template ?? "") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@method", (object)(config.Method ?? "") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@params", (object)(config.Params ?? "") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@devmode", config.DevMode ? 1 : 0);
            cmd.Parameters.AddWithValue("@updated_by", (object)(updatedBy ?? "") ?? DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
