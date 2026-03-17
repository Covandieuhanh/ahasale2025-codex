using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

public static class AccountResetSecurity_cl
{
    private const string ColForceHomePassword = "ForceChangePasswordHome";
    private const string ColForceHomePin = "ForceChangePinHome";
    private const string ColForceShopPassword = "ForceChangePasswordShop";

    private static readonly object SchemaLock = new object();
    private static bool _schemaReady;

    public static bool EnsureSchemaSafe(dbDataContext db, out string error)
    {
        error = "";
        if (db == null)
        {
            error = "Không có kết nối cơ sở dữ liệu.";
            return false;
        }

        if (_schemaReady)
            return true;

        lock (SchemaLock)
        {
            if (_schemaReady)
                return true;

            try
            {
                ExecuteNonQuery(db, @"
IF COL_LENGTH('dbo.taikhoan_tb', 'ForceChangePasswordHome') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_tb
    ADD ForceChangePasswordHome BIT NOT NULL
        CONSTRAINT DF_taikhoan_tb_ForceChangePasswordHome DEFAULT(0) WITH VALUES;
END

IF COL_LENGTH('dbo.taikhoan_tb', 'ForceChangePinHome') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_tb
    ADD ForceChangePinHome BIT NOT NULL
        CONSTRAINT DF_taikhoan_tb_ForceChangePinHome DEFAULT(0) WITH VALUES;
END

IF COL_LENGTH('dbo.taikhoan_tb', 'ForceChangePasswordShop') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_tb
    ADD ForceChangePasswordShop BIT NOT NULL
        CONSTRAINT DF_taikhoan_tb_ForceChangePasswordShop DEFAULT(0) WITH VALUES;
END

IF COL_LENGTH('dbo.taikhoan_tb', 'ResetSecurityBy') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_tb
    ADD ResetSecurityBy NVARCHAR(50) NULL;
END

IF COL_LENGTH('dbo.taikhoan_tb', 'ResetSecurityAt') IS NULL
BEGIN
    ALTER TABLE dbo.taikhoan_tb
    ADD ResetSecurityAt DATETIME NULL;
END
");

                _schemaReady = true;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                LogError("EnsureSchemaSafe", ex, "");
                return false;
            }
        }
    }

    public static bool ShouldForceHomePassword(dbDataContext db, string taiKhoan)
    {
        return ReadFlag(db, taiKhoan, ColForceHomePassword);
    }

    public static bool ShouldForceHomePin(dbDataContext db, string taiKhoan)
    {
        return ReadFlag(db, taiKhoan, ColForceHomePin);
    }

    public static bool ShouldForceShopPassword(dbDataContext db, string taiKhoan)
    {
        return ReadFlag(db, taiKhoan, ColForceShopPassword);
    }

    public static bool ResetHomePassword(dbDataContext db, string taiKhoan, string matKhauTam, string nguoiReset, out string error)
    {
        error = "";
        string tk = (taiKhoan ?? "").Trim();
        string pass = (matKhauTam ?? "").Trim();
        string actor = (nguoiReset ?? "").Trim();

        if (tk == "" || pass == "")
        {
            error = "Thiếu thông tin reset mật khẩu.";
            return false;
        }

        if (!EnsureSchemaSafe(db, out error))
            return false;

        try
        {
            int rows = ExecuteNonQuery(
                db,
                @"UPDATE dbo.taikhoan_tb
                  SET matkhau = @MatKhauTam,
                      ForceChangePasswordHome = 1,
                      ResetSecurityBy = @NguoiReset,
                      ResetSecurityAt = GETDATE()
                  WHERE taikhoan = @TaiKhoan;",
                NewParams(
                    "@MatKhauTam", pass,
                    "@NguoiReset", actor,
                    "@TaiKhoan", tk));

            if (rows <= 0)
            {
                error = "Không tìm thấy tài khoản cần reset.";
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            LogError("ResetHomePassword", ex, actor);
            return false;
        }
    }

    public static bool ResetHomePin(dbDataContext db, string taiKhoan, string pinHashTam, string nguoiReset, out string error)
    {
        error = "";
        string tk = (taiKhoan ?? "").Trim();
        string pinHash = (pinHashTam ?? "").Trim();
        string actor = (nguoiReset ?? "").Trim();

        if (tk == "" || pinHash == "")
        {
            error = "Thiếu thông tin reset PIN.";
            return false;
        }

        if (!EnsureSchemaSafe(db, out error))
            return false;

        try
        {
            int rows = ExecuteNonQuery(
                db,
                @"UPDATE dbo.taikhoan_tb
                  SET mapin_thanhtoan = @PinHashTam,
                      ForceChangePinHome = 1,
                      ResetSecurityBy = @NguoiReset,
                      ResetSecurityAt = GETDATE()
                  WHERE taikhoan = @TaiKhoan;",
                NewParams(
                    "@PinHashTam", pinHash,
                    "@NguoiReset", actor,
                    "@TaiKhoan", tk));

            if (rows <= 0)
            {
                error = "Không tìm thấy tài khoản cần reset.";
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            LogError("ResetHomePin", ex, actor);
            return false;
        }
    }

    public static bool ResetShopPassword(dbDataContext db, string taiKhoan, string matKhauTam, string nguoiReset, out string error)
    {
        error = "";
        string tk = (taiKhoan ?? "").Trim();
        string pass = (matKhauTam ?? "").Trim();
        string actor = (nguoiReset ?? "").Trim();

        if (tk == "" || pass == "")
        {
            error = "Thiếu thông tin reset mật khẩu gian hàng đối tác.";
            return false;
        }

        if (!EnsureSchemaSafe(db, out error))
            return false;

        try
        {
            int rows = ExecuteNonQuery(
                db,
                @"UPDATE dbo.taikhoan_tb
                  SET matkhau = @MatKhauTam,
                      ForceChangePasswordShop = 1,
                      ResetSecurityBy = @NguoiReset,
                      ResetSecurityAt = GETDATE()
                  WHERE taikhoan = @TaiKhoan;",
                NewParams(
                    "@MatKhauTam", pass,
                    "@NguoiReset", actor,
                    "@TaiKhoan", tk));

            if (rows <= 0)
            {
                error = "Không tìm thấy tài khoản cần reset.";
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            LogError("ResetShopPassword", ex, actor);
            return false;
        }
    }

    public static bool ClearForceHomePassword(dbDataContext db, string taiKhoan, out string error)
    {
        return ClearFlag(db, taiKhoan, ColForceHomePassword, out error);
    }

    public static bool ClearForceHomePin(dbDataContext db, string taiKhoan, out string error)
    {
        return ClearFlag(db, taiKhoan, ColForceHomePin, out error);
    }

    public static bool ClearForceShopPassword(dbDataContext db, string taiKhoan, out string error)
    {
        return ClearFlag(db, taiKhoan, ColForceShopPassword, out error);
    }

    private static bool ReadFlag(dbDataContext db, string taiKhoan, string columnName)
    {
        string tk = (taiKhoan ?? "").Trim();
        if (db == null || tk == "")
            return false;

        string error;
        if (!EnsureSchemaSafe(db, out error))
            return false;

        try
        {
            object raw = ExecuteScalar(
                db,
                "SELECT TOP 1 ISNULL(" + columnName + ", 0) FROM dbo.taikhoan_tb WHERE taikhoan = @TaiKhoan;",
                NewParams("@TaiKhoan", tk));

            return ToBool(raw);
        }
        catch (Exception ex)
        {
            LogError("ReadFlag:" + columnName, ex, tk);
            return false;
        }
    }

    private static bool ClearFlag(dbDataContext db, string taiKhoan, string columnName, out string error)
    {
        error = "";
        string tk = (taiKhoan ?? "").Trim();
        if (db == null || tk == "")
        {
            error = "Thiếu thông tin tài khoản.";
            return false;
        }

        if (!EnsureSchemaSafe(db, out error))
            return false;

        try
        {
            ExecuteNonQuery(
                db,
                "UPDATE dbo.taikhoan_tb SET " + columnName + " = 0 WHERE taikhoan = @TaiKhoan;",
                NewParams("@TaiKhoan", tk));
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            LogError("ClearFlag:" + columnName, ex, tk);
            return false;
        }
    }

    private static Dictionary<string, object> NewParams(params object[] pairs)
    {
        Dictionary<string, object> map = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        if (pairs == null)
            return map;

        for (int i = 0; i + 1 < pairs.Length; i += 2)
        {
            string key = (pairs[i] == null) ? "" : pairs[i].ToString();
            if (string.IsNullOrEmpty(key))
                continue;
            map[key] = pairs[i + 1];
        }

        return map;
    }

    private static int ExecuteNonQuery(dbDataContext db, string sql, IDictionary<string, object> parameters)
    {
        bool closeAfter = false;
        DbConnection conn = db.Connection;
        if (conn.State != ConnectionState.Open)
        {
            conn.Open();
            closeAfter = true;
        }

        try
        {
            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                AddParameters(cmd, parameters);
                return cmd.ExecuteNonQuery();
            }
        }
        finally
        {
            if (closeAfter && conn.State == ConnectionState.Open)
                conn.Close();
        }
    }

    private static int ExecuteNonQuery(dbDataContext db, string sql)
    {
        return ExecuteNonQuery(db, sql, null);
    }

    private static object ExecuteScalar(dbDataContext db, string sql, IDictionary<string, object> parameters)
    {
        bool closeAfter = false;
        DbConnection conn = db.Connection;
        if (conn.State != ConnectionState.Open)
        {
            conn.Open();
            closeAfter = true;
        }

        try
        {
            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                AddParameters(cmd, parameters);
                return cmd.ExecuteScalar();
            }
        }
        finally
        {
            if (closeAfter && conn.State == ConnectionState.Open)
                conn.Close();
        }
    }

    private static void AddParameters(DbCommand cmd, IDictionary<string, object> parameters)
    {
        if (cmd == null || parameters == null)
            return;

        foreach (KeyValuePair<string, object> p in parameters)
        {
            DbParameter prm = cmd.CreateParameter();
            prm.ParameterName = p.Key;
            prm.Value = p.Value ?? DBNull.Value;
            cmd.Parameters.Add(prm);
        }
    }

    private static bool ToBool(object raw)
    {
        if (raw == null || raw == DBNull.Value)
            return false;

        try
        {
            if (raw is bool)
                return (bool)raw;
            return Convert.ToInt32(raw) != 0;
        }
        catch
        {
            return false;
        }
    }

    private static void LogError(string action, Exception ex, string actor)
    {
        try
        {
            Log_cl.Add_Log(
                "[" + (action ?? "AccountResetSecurity") + "] " + ex.Message,
                actor ?? "",
                ex.StackTrace);
        }
        catch
        {
            // ignore logging exception
        }
    }
}
