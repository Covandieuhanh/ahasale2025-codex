using System;
using System.Data;
using System.Data.SqlClient;

public class HomeOtpRequestInfo
{
    public int Id;
    public string Phone;
    public string TaiKhoan;
    public DateTime ExpiresAt;
    public int Status;
}

public static class HomeOtp_cl
{
    public const string TypePassword = "password";
    public const string TypePin = "pin";

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
IF OBJECT_ID('dbo.Home_Otp_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Home_Otp_tb
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        taikhoan NVARCHAR(50) NOT NULL,
        phone NVARCHAR(20) NOT NULL,
        otp_code NVARCHAR(10) NOT NULL,
        otp_type NVARCHAR(20) NOT NULL,
        status INT NOT NULL CONSTRAINT DF_Home_Otp_tb_status DEFAULT(0),
        attempt_count INT NOT NULL CONSTRAINT DF_Home_Otp_tb_attempt DEFAULT(0),
        sent_at DATETIME NOT NULL CONSTRAINT DF_Home_Otp_tb_sent_at DEFAULT(GETDATE()),
        expires_at DATETIME NOT NULL,
        verified_at DATETIME NULL,
        consumed_at DATETIME NULL,
        last_attempt_at DATETIME NULL,
        client_ip NVARCHAR(45) NULL,
        user_agent NVARCHAR(200) NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Home_Otp_tb_phone_type' AND object_id = OBJECT_ID('dbo.Home_Otp_tb'))
BEGIN
    CREATE INDEX IX_Home_Otp_tb_phone_type ON dbo.Home_Otp_tb(phone, otp_type, sent_at DESC);
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

    public static bool TryGetOtpInfo(dbDataContext db, int requestId, string otpType, out HomeOtpRequestInfo info, out string error)
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
FROM dbo.Home_Otp_tb
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

                    info = new HomeOtpRequestInfo
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
            Log_cl.Add_Log("[HOME OTP] " + ex.Message, "", ex.StackTrace);
            return false;
        }
    }

    public static bool TrySendOtp(dbDataContext db, string phone, string taiKhoan, string otpType,
                                  out int requestId, out string error, out bool usedFallback, out string devOtp)
    {
        requestId = 0;
        error = "";
        usedFallback = false;
        devOtp = "";

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
            error = "Thiếu thông tin gửi OTP.";
            return false;
        }

        try
        {
            EnsureSchema(db);
            try
            {
                ShopOtp_cl.EnsureSchemaSafe(db);
            }
            catch
            {
                // Ignore mirror schema errors.
            }

            DateTime now = AhaTime_cl.Now;
            DateTime expires = now.AddMinutes(ExpireMinutes);

            using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmdCheck = conn.CreateCommand())
                {
                    cmdCheck.CommandText = @"
SELECT TOP 1 sent_at, status
FROM dbo.Home_Otp_tb
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

                string otp = OtpGenerator_cl.GenerateNumericCode(OtpLength);

                using (SqlCommand cmdInsert = conn.CreateCommand())
                {
                    cmdInsert.CommandText = @"
INSERT INTO dbo.Home_Otp_tb
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

                bool sent = SmsOtp_cl.SendOtp(db, cleanPhone, otp, out error, out usedFallback);
                if (!sent)
                {
                    using (SqlCommand cmdFail = conn.CreateCommand())
                    {
                        cmdFail.CommandText = "UPDATE dbo.Home_Otp_tb SET status = @status WHERE id = @id";
                        cmdFail.Parameters.AddWithValue("@status", StatusSendFailed);
                        cmdFail.Parameters.AddWithValue("@id", requestId);
                        cmdFail.ExecuteNonQuery();
                    }
                    if (string.IsNullOrEmpty(devOtp))
                        devOtp = otp;
                    usedFallback = true;
                    MirrorToShopLog(conn, cleanTk, cleanPhone, otp, cleanType, StatusSendFailed, now, expires);
                    return false;
                }

                if (usedFallback)
                {
                    devOtp = otp;
                }

                MirrorToShopLog(conn, cleanTk, cleanPhone, otp, cleanType, StatusSent, now, expires);
                return true;
            }
        }
        catch (Exception ex)
        {
            error = "Có lỗi xảy ra. Vui lòng thử lại.";
            Log_cl.Add_Log("[HOME OTP] " + ex.Message, cleanPhone, ex.StackTrace);
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
            try
            {
                ShopOtp_cl.EnsureSchemaSafe(db);
            }
            catch
            {
                // Ignore mirror schema errors.
            }

            DateTime now = AhaTime_cl.Now;
            DateTime expires = now.AddMinutes(ExpireMinutes);

            using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmdCheck = conn.CreateCommand())
                {
                    cmdCheck.CommandText = @"
SELECT TOP 1 sent_at, status
FROM dbo.Home_Otp_tb
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
INSERT INTO dbo.Home_Otp_tb
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

                if (requestId > 0)
                {
                    MirrorToShopLog(conn, cleanTk, cleanPhone, otp, cleanType, StatusSent, now, expires);
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
            Log_cl.Add_Log("[HOME OTP] " + ex.Message, cleanPhone, ex.StackTrace);
            return false;
        }
    }

    public static bool VerifyOtp(dbDataContext db, int requestId, string otpInput, string otpType, out string error)
    {
        error = "";
        if (db == null)
        {
            error = "Hệ thống chưa sẵn sàng.";
            return false;
        }

        string cleanOtp = (otpInput ?? "").Trim();
        string cleanType = (otpType ?? "").Trim().ToLowerInvariant();
        if (requestId <= 0 || string.IsNullOrEmpty(cleanOtp))
        {
            error = "Vui lòng nhập mã OTP.";
            return false;
        }

        try
        {
            EnsureSchema(db);

            using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
            {
                conn.Open();
                bool isMismatch = false;
                int attempts = 0;
                int status = 0;
                string phone = "";
                DateTime now = AhaTime_cl.Now;

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
SELECT otp_code, expires_at, status, attempt_count, phone
FROM dbo.Home_Otp_tb
WHERE id = @id AND otp_type = @type";
                    cmd.Parameters.AddWithValue("@id", requestId);
                    cmd.Parameters.AddWithValue("@type", cleanType);

                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!reader.Read())
                        {
                            error = "Mã OTP không hợp lệ.";
                            return false;
                        }

                        string otp = reader.GetString(0);
                        DateTime expiresAt = reader.GetDateTime(1);
                        status = reader.GetInt32(2);
                        attempts = reader.GetInt32(3);
                        phone = reader.IsDBNull(4) ? "" : reader.GetString(4);

                        if (expiresAt < now)
                        {
                            MarkStatus(conn, requestId, StatusExpired);
                            error = "Mã OTP đã hết hạn. Vui lòng gửi lại.";
                            return false;
                        }

                        if (status == StatusConsumed || status == StatusExpired)
                        {
                            error = "Mã OTP đã hết hạn hoặc đã được sử dụng.";
                            return false;
                        }

                        if (status == StatusLocked)
                        {
                            error = "Mã OTP đã bị khóa do nhập sai nhiều lần.";
                            return false;
                        }

                        if (attempts >= MaxAttempts)
                        {
                            MarkStatus(conn, requestId, StatusLocked);
                            error = "Mã OTP đã bị khóa do nhập sai nhiều lần.";
                            return false;
                        }

                        if (!string.Equals(otp ?? "", cleanOtp, StringComparison.Ordinal))
                        {
                            isMismatch = true;
                        }
                    }
                }

                if (isMismatch)
                {
                    if (TryVerifyFallbackOtp(conn, requestId, phone, cleanType, cleanOtp, now))
                    {
                        return true;
                    }

                    attempts += 1;
                    int newStatus = (attempts >= MaxAttempts) ? StatusLocked : status;
                    UpdateAttempt(conn, requestId, attempts, newStatus);
                    error = "Mã OTP không đúng.";
                    return false;
                }

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
UPDATE dbo.Home_Otp_tb
SET status = @status, verified_at = @verified, last_attempt_at = @attempt
WHERE id = @id";
                    cmd.Parameters.AddWithValue("@status", StatusVerified);
                    cmd.Parameters.AddWithValue("@verified", AhaTime_cl.Now);
                    cmd.Parameters.AddWithValue("@attempt", AhaTime_cl.Now);
                    cmd.Parameters.AddWithValue("@id", requestId);
                    cmd.ExecuteNonQuery();
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            error = "Có lỗi xảy ra. Vui lòng thử lại.";
            Log_cl.Add_Log("[HOME OTP] " + ex.Message, "", ex.StackTrace);
            return false;
        }
    }

    public static bool TryConsumeVerifiedOtp(dbDataContext db, int requestId, string otpType, out string taiKhoan, out string error)
    {
        taiKhoan = "";
        error = "";
        if (db == null)
        {
            error = "Hệ thống chưa sẵn sàng.";
            return false;
        }

        string cleanType = (otpType ?? "").Trim().ToLowerInvariant();
        if (requestId <= 0 || string.IsNullOrEmpty(cleanType))
        {
            error = "Yêu cầu không hợp lệ.";
            return false;
        }

        try
        {
            EnsureSchema(db);

            using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
SELECT taikhoan, expires_at, status
FROM dbo.Home_Otp_tb
WHERE id = @id AND otp_type = @type";
                    cmd.Parameters.AddWithValue("@id", requestId);
                    cmd.Parameters.AddWithValue("@type", cleanType);

                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!reader.Read())
                        {
                            error = "Yêu cầu không tồn tại.";
                            return false;
                        }

                        taiKhoan = reader.GetString(0);
                        DateTime expiresAt = reader.GetDateTime(1);
                        int status = reader.GetInt32(2);

                        if (expiresAt < AhaTime_cl.Now)
                        {
                            MarkStatus(conn, requestId, StatusExpired);
                            error = "Mã OTP đã hết hạn. Vui lòng gửi lại.";
                            return false;
                        }

                        if (status != StatusVerified)
                        {
                            error = "Mã OTP chưa được xác thực.";
                            return false;
                        }
                    }
                }

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
UPDATE dbo.Home_Otp_tb
SET status = @status, consumed_at = @consumed
WHERE id = @id";
                    cmd.Parameters.AddWithValue("@status", StatusConsumed);
                    cmd.Parameters.AddWithValue("@consumed", AhaTime_cl.Now);
                    cmd.Parameters.AddWithValue("@id", requestId);
                    cmd.ExecuteNonQuery();
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            error = "Có lỗi xảy ra. Vui lòng thử lại.";
            Log_cl.Add_Log("[HOME OTP] " + ex.Message, "", ex.StackTrace);
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

    private static void MarkStatus(SqlConnection conn, int requestId, int status)
    {
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = "UPDATE dbo.Home_Otp_tb SET status = @status WHERE id = @id";
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@id", requestId);
            cmd.ExecuteNonQuery();
        }
    }

    private static void UpdateAttempt(SqlConnection conn, int requestId, int attempts, int status)
    {
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
UPDATE dbo.Home_Otp_tb
SET attempt_count = @attempt, last_attempt_at = @attempt_at, status = @status
WHERE id = @id";
            cmd.Parameters.AddWithValue("@attempt", attempts);
            cmd.Parameters.AddWithValue("@attempt_at", AhaTime_cl.Now);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@id", requestId);
            cmd.ExecuteNonQuery();
        }
    }

    private static void MirrorToShopLog(SqlConnection conn, string taiKhoan, string phone, string otp, string otpType,
                                        int status, DateTime sentAt, DateTime expiresAt)
    {
        if (conn == null)
            return;

        string cleanTk = (taiKhoan ?? "").Trim();
        string cleanPhone = (phone ?? "").Trim();
        string cleanOtp = (otp ?? "").Trim();
        string cleanType = (otpType ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(cleanTk) || string.IsNullOrEmpty(cleanPhone) || string.IsNullOrEmpty(cleanOtp) || string.IsNullOrEmpty(cleanType))
            return;

        try
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO dbo.Shop_Otp_tb
    (taikhoan, phone, otp_code, otp_type, status, attempt_count, sent_at, expires_at, client_ip, user_agent)
VALUES
    (@tk, @phone, @otp, @type, @status, 0, @sent_at, @expires_at, @ip, @ua);";
                cmd.Parameters.AddWithValue("@tk", cleanTk);
                cmd.Parameters.AddWithValue("@phone", cleanPhone);
                cmd.Parameters.AddWithValue("@otp", cleanOtp);
                cmd.Parameters.AddWithValue("@type", "home_" + cleanType);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@sent_at", sentAt);
                cmd.Parameters.AddWithValue("@expires_at", expiresAt);
                cmd.Parameters.AddWithValue("@ip", "");
                cmd.Parameters.AddWithValue("@ua", "");
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log("[HOME OTP MIRROR] " + ex.Message, cleanPhone, ex.StackTrace);
        }
    }

    private static bool TryVerifyFallbackOtp(SqlConnection conn, int requestId, string phone, string otpType, string otpInput, DateTime now)
    {
        if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(otpType) || string.IsNullOrEmpty(otpInput))
            return false;

        using (SqlConnection fallbackConn = new SqlConnection(conn.ConnectionString))
        {
            fallbackConn.Open();

            using (SqlCommand cmd = fallbackConn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT TOP 1 id, expires_at, status
FROM dbo.Home_Otp_tb
WHERE phone = @phone AND otp_type = @type AND otp_code = @otp
ORDER BY id DESC";
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@type", otpType);
                cmd.Parameters.AddWithValue("@otp", otpInput);

                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!reader.Read())
                        return false;

                    int matchedId = reader.GetInt32(0);
                    DateTime expiresAt = reader.GetDateTime(1);
                    int status = reader.GetInt32(2);

                    if (expiresAt < now)
                        return false;
                    if (status == StatusConsumed || status == StatusExpired || status == StatusLocked)
                        return false;

                    reader.Close();

                    using (SqlCommand cmdUpdate = fallbackConn.CreateCommand())
                    {
                        cmdUpdate.CommandText = @"
UPDATE dbo.Home_Otp_tb
SET status = @status, verified_at = @verified, consumed_at = @consumed, last_attempt_at = @attempt
WHERE id = @id";
                        cmdUpdate.Parameters.AddWithValue("@status", StatusConsumed);
                        cmdUpdate.Parameters.AddWithValue("@verified", now);
                        cmdUpdate.Parameters.AddWithValue("@consumed", now);
                        cmdUpdate.Parameters.AddWithValue("@attempt", now);
                        cmdUpdate.Parameters.AddWithValue("@id", matchedId);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    using (SqlCommand cmdUpdateReq = fallbackConn.CreateCommand())
                    {
                        cmdUpdateReq.CommandText = @"
UPDATE dbo.Home_Otp_tb
SET status = @status, verified_at = @verified, last_attempt_at = @attempt
WHERE id = @id";
                        cmdUpdateReq.Parameters.AddWithValue("@status", StatusVerified);
                        cmdUpdateReq.Parameters.AddWithValue("@verified", now);
                        cmdUpdateReq.Parameters.AddWithValue("@attempt", now);
                        cmdUpdateReq.Parameters.AddWithValue("@id", requestId);
                        cmdUpdateReq.ExecuteNonQuery();
                    }

                    return true;
                }
            }
        }
    }
}
