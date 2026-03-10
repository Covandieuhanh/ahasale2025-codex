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
        updated_by NVARCHAR(50) NULL,
        updated_at DATETIME NULL
    );
    INSERT INTO dbo.Otp_Config_tb (id, sms_dev_mode) VALUES (1, 0);
END;

IF COL_LENGTH('dbo.Otp_Config_tb', 'sms_method') IS NULL
    ALTER TABLE dbo.Otp_Config_tb ADD sms_method NVARCHAR(10) NULL;
IF COL_LENGTH('dbo.Otp_Config_tb', 'sms_params') IS NULL
    ALTER TABLE dbo.Otp_Config_tb ADD sms_params NVARCHAR(MAX) NULL;";
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
