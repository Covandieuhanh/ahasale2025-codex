using System;
using System.Data;
using System.Data.SqlClient;

public class ShopOtpRequestInfo
{
    public int Id;
    public string Phone;
    public string TaiKhoan;
    public DateTime ExpiresAt;
    public int Status;
}

public static class ShopOtp_cl
{
    public const string TypePassword = "password";

    public const int OtpLength = 6;
    public const int ExpireMinutes = 5;
    public const int ResendCooldownSeconds = 60;
    public const int MaxAttempts = 5;

    public const int StatusSent = 0;
    public const int StatusVerified = 1;
    public const int StatusConsumed = 2;
    public const int StatusExpired = 3;
    public const int StatusLocked = 4;
    public const int StatusSendFailed = 5;

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
IF OBJECT_ID('dbo.Shop_Otp_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Shop_Otp_tb
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        taikhoan NVARCHAR(50) NOT NULL,
        phone NVARCHAR(20) NOT NULL,
        otp_code NVARCHAR(10) NOT NULL,
        otp_type NVARCHAR(20) NOT NULL,
        status INT NOT NULL CONSTRAINT DF_Shop_Otp_tb_status DEFAULT(0),
        attempt_count INT NOT NULL CONSTRAINT DF_Shop_Otp_tb_attempt DEFAULT(0),
        sent_at DATETIME NOT NULL CONSTRAINT DF_Shop_Otp_tb_sent_at DEFAULT(GETDATE()),
        expires_at DATETIME NOT NULL,
        verified_at DATETIME NULL,
        consumed_at DATETIME NULL,
        last_attempt_at DATETIME NULL,
        client_ip NVARCHAR(45) NULL,
        user_agent NVARCHAR(200) NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Shop_Otp_tb_phone_type' AND object_id = OBJECT_ID('dbo.Shop_Otp_tb'))
BEGIN
    CREATE INDEX IX_Shop_Otp_tb_phone_type ON dbo.Shop_Otp_tb(phone, otp_type, sent_at DESC);
END;";
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            _schemaEnsured = true;
        }
    }

    public static void EnsureSchemaSafe(dbDataContext db)
    {
        EnsureSchema(db);
    }

    public static bool TryGetOtpInfo(dbDataContext db, int requestId, string otpType, out ShopOtpRequestInfo info, out string error)
    {
        info = null;
        error = "";
        if (db == null)
        {
            error = "Hệ thống chưa sẵn sàng.";
            return false;
        }
        if (requestId <= 0)
        {
            error = "Yêu cầu không hợp lệ.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(otpType))
        {
            error = "Loại OTP không hợp lệ.";
            return false;
        }

        try
        {
            EnsureSchema(db);

            using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT TOP 1 id, phone, taikhoan, expires_at, status
FROM dbo.Shop_Otp_tb
WHERE id = @id AND otp_type = @type";
                cmd.Parameters.AddWithValue("@id", requestId);
                cmd.Parameters.AddWithValue("@type", otpType);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!reader.Read())
                    {
                        error = "Yêu cầu không tồn tại.";
                        return false;
                    }

                    info = new ShopOtpRequestInfo
                    {
                        Id = reader.GetInt32(0),
                        Phone = reader.GetString(1),
                        TaiKhoan = reader.GetString(2),
                        ExpiresAt = reader.GetDateTime(3),
                        Status = reader.GetInt32(4)
                    };
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            error = "Có lỗi xảy ra. Vui lòng thử lại.";
            Log_cl.Add_Log("[SHOP OTP] " + ex.Message, "", ex.StackTrace);
            return false;
        }
    }

    public static bool CreateManualOtp(dbDataContext db, string phone, string taiKhoan, string otpType,
                                       out int requestId, out string otp, out string error)
    {
        requestId = 0;
        otp = "";
        error = "";
        if (db == null)
        {
            error = "Hệ thống chưa sẵn sàng.";
            return false;
        }

        string cleanPhone = (phone ?? "").Trim();
        string cleanTk = (taiKhoan ?? "").Trim();
        string cleanType = (otpType ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(cleanPhone) || string.IsNullOrEmpty(cleanTk) || string.IsNullOrEmpty(cleanType))
        {
            error = "Thiếu thông tin tạo OTP.";
            return false;
        }

        try
        {
            EnsureSchema(db);

            DateTime now = AhaTime_cl.Now;
            DateTime expires = now.AddMinutes(ExpireMinutes);

            using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmdCheck = conn.CreateCommand())
                {
                    cmdCheck.CommandText = @"
SELECT TOP 1 sent_at, status
FROM dbo.Shop_Otp_tb
WHERE phone = @phone AND otp_type = @type
ORDER BY id DESC";
                    cmdCheck.Parameters.AddWithValue("@phone", cleanPhone);
                    cmdCheck.Parameters.AddWithValue("@type", cleanType);
                    using (SqlDataReader reader = cmdCheck.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            DateTime lastSent = reader.GetDateTime(0);
                            int status = reader.GetInt32(1);
                            if (status == StatusSent || status == StatusVerified)
                            {
                                double delta = (now - lastSent).TotalSeconds;
                                if (delta < ResendCooldownSeconds)
                                {
                                    int wait = (int)Math.Ceiling(ResendCooldownSeconds - delta);
                                    error = "Vui lòng đợi " + wait + " giây rồi thử lại.";
                                    return false;
                                }
                            }
                        }
                    }
                }

                otp = OtpGenerator_cl.GenerateNumericCode(OtpLength);

                using (SqlCommand cmdInsert = conn.CreateCommand())
                {
                    cmdInsert.CommandText = @"
INSERT INTO dbo.Shop_Otp_tb
    (taikhoan, phone, otp_code, otp_type, status, attempt_count, sent_at, expires_at, client_ip, user_agent)
OUTPUT INSERTED.id
VALUES
    (@tk, @phone, @otp, @type, @status, 0, @sent_at, @expires_at, @ip, @ua);";
                    cmdInsert.Parameters.AddWithValue("@tk", cleanTk);
                    cmdInsert.Parameters.AddWithValue("@phone", cleanPhone);
                    cmdInsert.Parameters.AddWithValue("@otp", otp);
                    cmdInsert.Parameters.AddWithValue("@type", cleanType);
                    cmdInsert.Parameters.AddWithValue("@status", StatusSent);
                    cmdInsert.Parameters.AddWithValue("@sent_at", now);
                    cmdInsert.Parameters.AddWithValue("@expires_at", expires);
                    cmdInsert.Parameters.AddWithValue("@ip", "");
                    cmdInsert.Parameters.AddWithValue("@ua", "");

                    object result = cmdInsert.ExecuteScalar();
                    requestId = (result != null) ? Convert.ToInt32(result) : 0;
                }
            }

            if (requestId <= 0)
            {
                error = "Không tạo được OTP.";
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            error = "Có lỗi xảy ra. Vui lòng thử lại.";
            Log_cl.Add_Log("[SHOP OTP] " + ex.Message, cleanPhone, ex.StackTrace);
            return false;
        }
    }

    public static string MaskPhone(string phone)
    {
        string p = (phone ?? "").Trim();
        if (p.Length <= 4)
            return p;
        string tail = p.Substring(p.Length - 4);
        return new string('*', Math.Max(0, p.Length - 4)) + tail;
    }
}
