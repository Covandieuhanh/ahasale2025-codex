using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

public static class AddressHistory_cl
{
    private static readonly object SyncRoot = new object();
    private static bool _schemaEnsured;

    public sealed class AddressItem
    {
        public long Id { get; set; }
        public bool IsDefault { get; set; }
        public string HoTen { get; set; }
        public string Sdt { get; set; }
        public string DiaChi { get; set; }
        public string DisplayTitle { get; set; }
        public string DisplayAddress { get; set; }
    }

    public static List<AddressItem> GetRecentAddresses(dbDataContext db, string taiKhoan, int limit, bool includeProfile)
    {
        return GetSavedAddresses(db, taiKhoan, limit, includeProfile);
    }

    public static List<AddressItem> GetSavedAddresses(dbDataContext db, string taiKhoan, int limit, bool includeProfile)
    {
        List<AddressItem> result = new List<AddressItem>();
        if (db == null || string.IsNullOrWhiteSpace(taiKhoan) || limit <= 0)
            return result;

        EnsureSchema(db);
        SeedIfEmpty(db, taiKhoan, includeProfile);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT TOP (@limit) id, hoten, sdt, diachi, is_default
FROM dbo.Home_Address_Book_tb
WHERE taikhoan = @tk AND deleted_at IS NULL
ORDER BY ISNULL(is_default, 0) DESC, ISNULL(updated_at, created_at) DESC, id DESC";
            cmd.Parameters.AddWithValue("@limit", limit);
            cmd.Parameters.AddWithValue("@tk", taiKhoan);
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    long id = reader["id"] != DBNull.Value ? Convert.ToInt64(reader["id"]) : 0;
                    string hoTen = reader["hoten"] as string ?? "";
                    string sdt = reader["sdt"] as string ?? "";
                    string diaChi = reader["diachi"] as string ?? "";
                    bool isDefault = reader["is_default"] != DBNull.Value && Convert.ToBoolean(reader["is_default"]);

                    string title = string.IsNullOrEmpty(hoTen) ? "Người nhận" : hoTen;
                    string phoneMasked = MaskPhone(sdt);
                    if (!string.IsNullOrEmpty(phoneMasked))
                        title = title + " (" + phoneMasked + ")";

                    result.Add(new AddressItem
                    {
                        Id = id,
                        IsDefault = isDefault,
                        HoTen = hoTen,
                        Sdt = sdt,
                        DiaChi = diaChi,
                        DisplayTitle = title,
                        DisplayAddress = diaChi
                    });
                }
            }
        }

        return result;
    }

    public static void UpsertAddress(dbDataContext db, string taiKhoan, string hoTenRaw, string sdtRaw, string diaChiRaw, bool setDefault = false)
    {
        if (db == null || string.IsNullOrWhiteSpace(taiKhoan))
            return;

        string hoTen = (hoTenRaw ?? "").Trim();
        string sdt = (sdtRaw ?? "").Trim();
        string diaChi = NormalizeAddress(diaChiRaw);

        if (string.IsNullOrEmpty(hoTen) && string.IsNullOrEmpty(sdt) && string.IsNullOrEmpty(diaChi))
            return;

        EnsureSchema(db);
        bool makeDefault = setDefault || !HasDefault(db, taiKhoan);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
DECLARE @existingId BIGINT = NULL;
SELECT TOP 1 @existingId = id
FROM dbo.Home_Address_Book_tb
WHERE taikhoan = @tk
  AND ISNULL(hoten,'') = @hoten
  AND ISNULL(sdt,'') = @sdt
  AND ISNULL(diachi,'') = @diachi;

IF @existingId IS NULL
BEGIN
    INSERT INTO dbo.Home_Address_Book_tb (taikhoan, hoten, sdt, diachi, is_default, created_at, updated_at, deleted_at)
    VALUES (@tk, @hoten, @sdt, @diachi, @is_default, @now, @now, NULL);
    SET @existingId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Home_Address_Book_tb
    SET updated_at = @now,
        deleted_at = NULL,
        is_default = CASE WHEN @is_default = 1 THEN 1 ELSE ISNULL(is_default, 0) END
    WHERE id = @existingId;
END";

            cmd.Parameters.AddWithValue("@tk", taiKhoan);
            cmd.Parameters.AddWithValue("@hoten", hoTen);
            cmd.Parameters.AddWithValue("@sdt", sdt);
            cmd.Parameters.AddWithValue("@diachi", diaChi);
            cmd.Parameters.AddWithValue("@is_default", makeDefault);
            cmd.Parameters.AddWithValue("@now", AhaTime_cl.Now);

            conn.Open();
            cmd.ExecuteNonQuery();

            if (makeDefault)
            {
                using (SqlCommand cmd2 = conn.CreateCommand())
                {
                    cmd2.CommandText = @"
UPDATE dbo.Home_Address_Book_tb
SET is_default = 0
WHERE taikhoan = @tk AND deleted_at IS NULL
  AND NOT (ISNULL(hoten,'') = @hoten AND ISNULL(sdt,'') = @sdt AND ISNULL(diachi,'') = @diachi)";
                    cmd2.Parameters.AddWithValue("@tk", taiKhoan);
                    cmd2.Parameters.AddWithValue("@hoten", hoTen);
                    cmd2.Parameters.AddWithValue("@sdt", sdt);
                    cmd2.Parameters.AddWithValue("@diachi", diaChi);
                    cmd2.ExecuteNonQuery();
                }
            }
        }
    }

    public static void UpdateAddress(dbDataContext db, string taiKhoan, long id, string hoTenRaw, string sdtRaw, string diaChiRaw, bool setDefault)
    {
        if (db == null || string.IsNullOrWhiteSpace(taiKhoan) || id <= 0)
            return;

        string hoTen = (hoTenRaw ?? "").Trim();
        string sdt = (sdtRaw ?? "").Trim();
        string diaChi = NormalizeAddress(diaChiRaw);

        if (string.IsNullOrEmpty(hoTen) && string.IsNullOrEmpty(sdt) && string.IsNullOrEmpty(diaChi))
            return;

        EnsureSchema(db);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
UPDATE dbo.Home_Address_Book_tb
SET hoten = @hoten,
    sdt = @sdt,
    diachi = @diachi,
    deleted_at = NULL,
    updated_at = @now,
    is_default = CASE WHEN @set_default = 1 THEN 1 ELSE ISNULL(is_default, 0) END
WHERE id = @id AND taikhoan = @tk";
            cmd.Parameters.AddWithValue("@hoten", hoTen);
            cmd.Parameters.AddWithValue("@sdt", sdt);
            cmd.Parameters.AddWithValue("@diachi", diaChi);
            cmd.Parameters.AddWithValue("@now", AhaTime_cl.Now);
            cmd.Parameters.AddWithValue("@set_default", setDefault);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@tk", taiKhoan);

            conn.Open();
            cmd.ExecuteNonQuery();

            if (setDefault)
            {
                using (SqlCommand cmd2 = conn.CreateCommand())
                {
                    cmd2.CommandText = @"
UPDATE dbo.Home_Address_Book_tb
SET is_default = 0
WHERE taikhoan = @tk AND deleted_at IS NULL AND id <> @id";
                    cmd2.Parameters.AddWithValue("@tk", taiKhoan);
                    cmd2.Parameters.AddWithValue("@id", id);
                    cmd2.ExecuteNonQuery();
                }
            }
            else if (!HasDefault(db, taiKhoan))
            {
                using (SqlCommand cmd3 = conn.CreateCommand())
                {
                    cmd3.CommandText = @"
UPDATE dbo.Home_Address_Book_tb
SET is_default = 1
WHERE id = @id AND taikhoan = @tk";
                    cmd3.Parameters.AddWithValue("@id", id);
                    cmd3.Parameters.AddWithValue("@tk", taiKhoan);
                    cmd3.ExecuteNonQuery();
                }
            }
        }
    }

    public static AddressItem FindById(dbDataContext db, string taiKhoan, long id)
    {
        if (db == null || string.IsNullOrWhiteSpace(taiKhoan) || id <= 0)
            return null;

        EnsureSchema(db);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
SELECT TOP 1 id, hoten, sdt, diachi, is_default
FROM dbo.Home_Address_Book_tb
WHERE taikhoan = @tk AND id = @id AND deleted_at IS NULL";
            cmd.Parameters.AddWithValue("@tk", taiKhoan);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                    return null;

                long outId = reader["id"] != DBNull.Value ? Convert.ToInt64(reader["id"]) : 0;
                string hoTen = reader["hoten"] as string ?? "";
                string sdt = reader["sdt"] as string ?? "";
                string diaChi = reader["diachi"] as string ?? "";
                bool isDefault = reader["is_default"] != DBNull.Value && Convert.ToBoolean(reader["is_default"]);

                string title = string.IsNullOrEmpty(hoTen) ? "Người nhận" : hoTen;
                string phoneMasked = MaskPhone(sdt);
                if (!string.IsNullOrEmpty(phoneMasked))
                    title = title + " (" + phoneMasked + ")";

                return new AddressItem
                {
                    Id = outId,
                    IsDefault = isDefault,
                    HoTen = hoTen,
                    Sdt = sdt,
                    DiaChi = diaChi,
                    DisplayTitle = title,
                    DisplayAddress = diaChi
                };
            }
        }
    }

    public static void SetDefaultAddress(dbDataContext db, string taiKhoan, long id)
    {
        if (db == null || string.IsNullOrWhiteSpace(taiKhoan) || id <= 0)
            return;

        EnsureSchema(db);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
UPDATE dbo.Home_Address_Book_tb
SET is_default = CASE WHEN id = @id THEN 1 ELSE 0 END,
    updated_at = @now
WHERE taikhoan = @tk AND deleted_at IS NULL";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@tk", taiKhoan);
            cmd.Parameters.AddWithValue("@now", AhaTime_cl.Now);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public static void DeleteAddress(dbDataContext db, string taiKhoan, long id)
    {
        if (db == null || string.IsNullOrWhiteSpace(taiKhoan) || id <= 0)
            return;

        EnsureSchema(db);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
UPDATE dbo.Home_Address_Book_tb
SET deleted_at = @now, updated_at = @now, is_default = 0
WHERE id = @id AND taikhoan = @tk";
            cmd.Parameters.AddWithValue("@now", AhaTime_cl.Now);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@tk", taiKhoan);
            conn.Open();
            cmd.ExecuteNonQuery();

            using (SqlCommand cmd2 = conn.CreateCommand())
            {
                cmd2.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM dbo.Home_Address_Book_tb WHERE taikhoan = @tk AND deleted_at IS NULL AND is_default = 1)
BEGIN
    DECLARE @newId BIGINT = (
        SELECT TOP 1 id
        FROM dbo.Home_Address_Book_tb
        WHERE taikhoan = @tk AND deleted_at IS NULL
        ORDER BY ISNULL(updated_at, created_at) DESC, id DESC
    );
    IF @newId IS NOT NULL
        UPDATE dbo.Home_Address_Book_tb SET is_default = 1 WHERE id = @newId;
END";
                cmd2.Parameters.AddWithValue("@tk", taiKhoan);
                cmd2.ExecuteNonQuery();
            }
        }
    }

    private static string MaskPhone(string phoneRaw)
    {
        string raw = (phoneRaw ?? "").Trim();
        if (string.IsNullOrEmpty(raw))
            return "";

        string digits = new string(raw.Where(char.IsDigit).ToArray());
        if (digits.Length <= 4)
            return raw;

        int tailLen = 2;
        int headLen = digits.Length >= 7 ? 4 : 2;
        if (digits.Length <= headLen + tailLen)
            return raw;

        string head = digits.Substring(0, headLen);
        string tail = digits.Substring(digits.Length - tailLen);
        string masked = head + new string('*', digits.Length - headLen - tailLen) + tail;
        return masked;
    }

    private static void EnsureSchema(dbDataContext db)
    {
        if (_schemaEnsured || db == null)
            return;

        lock (SyncRoot)
        {
            if (_schemaEnsured) return;

            using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Home_Address_Book_tb')
BEGIN
    CREATE TABLE dbo.Home_Address_Book_tb (
        id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        taikhoan NVARCHAR(50) NOT NULL,
        hoten NVARCHAR(200) NULL,
        sdt NVARCHAR(50) NULL,
        diachi NVARCHAR(MAX) NULL,
        is_default BIT NULL,
        created_at DATETIME NULL,
        updated_at DATETIME NULL,
        deleted_at DATETIME NULL
    );
    CREATE INDEX IX_Home_Address_Book_TaiKhoan ON dbo.Home_Address_Book_tb(taikhoan);
END";
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            _schemaEnsured = true;
        }
    }

    private static bool HasDefault(dbDataContext db, string taiKhoan)
    {
        if (db == null || string.IsNullOrWhiteSpace(taiKhoan))
            return false;

        EnsureSchema(db);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT TOP 1 id FROM dbo.Home_Address_Book_tb WHERE taikhoan = @tk AND deleted_at IS NULL AND is_default = 1";
            cmd.Parameters.AddWithValue("@tk", taiKhoan);
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                return reader.Read();
            }
        }
    }

    private static void SeedIfEmpty(dbDataContext db, string taiKhoan, bool includeProfile)
    {
        if (db == null || string.IsNullOrWhiteSpace(taiKhoan))
            return;

        bool hasAny = false;
        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT TOP 1 id FROM dbo.Home_Address_Book_tb WHERE taikhoan = @tk";
            cmd.Parameters.AddWithValue("@tk", taiKhoan);
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                hasAny = reader.Read();
            }
        }

        if (hasAny) return;

        HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        List<Tuple<string, string, string>> seedRows = new List<Tuple<string, string, string>>();

        var orders = db.DonHang_tbs
            .Where(x => x.nguoimua == taiKhoan && x.diahchi_nguoinhan != null && x.diahchi_nguoinhan != "")
            .OrderByDescending(x => x.ngaydat)
            .Select(x => new { x.hoten_nguoinhan, x.sdt_nguoinhan, x.diahchi_nguoinhan })
            .Take(20)
            .ToList();

        foreach (var row in orders)
        {
            if (TrySeed(seen, seedRows, row.hoten_nguoinhan, row.sdt_nguoinhan, row.diahchi_nguoinhan, 10))
            {
                if (seedRows.Count >= 5) break;
            }
        }

        if (includeProfile)
        {
            var acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == taiKhoan);
            if (acc != null)
            {
                TrySeed(seen, seedRows, acc.hoten, acc.dienthoai, acc.diachi, 10);
            }
        }

        if (seedRows.Count == 0)
            return;

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        {
            conn.Open();
            for (int i = 0; i < seedRows.Count; i++)
            {
                var row = seedRows[i];
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
INSERT INTO dbo.Home_Address_Book_tb (taikhoan, hoten, sdt, diachi, is_default, created_at, updated_at, deleted_at)
VALUES (@tk, @hoten, @sdt, @diachi, @is_default, @now, @now, NULL)";
                    cmd.Parameters.AddWithValue("@tk", taiKhoan);
                    cmd.Parameters.AddWithValue("@hoten", row.Item1 ?? "");
                    cmd.Parameters.AddWithValue("@sdt", row.Item2 ?? "");
                    cmd.Parameters.AddWithValue("@diachi", row.Item3 ?? "");
                    cmd.Parameters.AddWithValue("@is_default", i == 0);
                    cmd.Parameters.AddWithValue("@now", AhaTime_cl.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    private static bool TrySeed(HashSet<string> seen, List<Tuple<string, string, string>> list, string hoTenRaw, string sdtRaw, string diaChiRaw, int limit)
    {
        if (list.Count >= limit) return false;
        string hoTen = (hoTenRaw ?? "").Trim();
        string sdt = (sdtRaw ?? "").Trim();
        string diaChi = NormalizeAddress(diaChiRaw);

        if (string.IsNullOrEmpty(hoTen) && string.IsNullOrEmpty(sdt) && string.IsNullOrEmpty(diaChi))
            return false;

        string key = string.Concat(hoTen, "|", sdt, "|", diaChi).ToLowerInvariant();
        if (!seen.Add(key))
            return false;

        list.Add(Tuple.Create(hoTen, sdt, diaChi));
        return true;
    }

    private static string NormalizeAddress(string raw)
    {
        string value = (raw ?? "").Trim();
        if (string.IsNullOrEmpty(value))
            return "";

        value = value.Replace("\r\n", "\n").Replace("\r", "\n");
        return value.Trim();
    }
}
