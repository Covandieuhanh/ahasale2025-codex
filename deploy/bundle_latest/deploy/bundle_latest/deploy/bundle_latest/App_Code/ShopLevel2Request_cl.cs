using System;
using System.Collections.Generic;
using System.Linq;

public static class ShopLevel2Request_cl
{
    public const byte StatusPending = 0;
    public const byte StatusApproved = 1;
    public const byte StatusRejected = 2;

    private static bool _schemaReady;
    private static readonly object _schemaLock = new object();

    public class Level2RequestInfo
    {
        public int ID { get; set; }
        public string taikhoan { get; set; }
        public byte TrangThai { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string AdminDuyet { get; set; }
        public string GhiChuAdmin { get; set; }
    }

    public static void EnsureSchemaSafe(dbDataContext db)
    {
        if (db == null)
            return;
        if (_schemaReady)
            return;

        lock (_schemaLock)
        {
            if (_schemaReady)
                return;

            try
            {
                db.ExecuteCommand(@"
IF OBJECT_ID('dbo.ShopLevel2Request_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ShopLevel2Request_tb
    (
        [ID] INT NOT NULL IDENTITY PRIMARY KEY,
        [taikhoan] NVARCHAR(200) NOT NULL,
        [TrangThai] TINYINT NOT NULL CONSTRAINT DF_ShopLevel2Request_TrangThai DEFAULT(0),
        [NgayTao] DATETIME NULL,
        [NgayDuyet] DATETIME NULL,
        [AdminDuyet] NVARCHAR(200) NULL,
        [GhiChuAdmin] NVARCHAR(MAX) NULL
    );
END
");
                _schemaReady = true;
            }
            catch
            {
                // thử lại ở lần sau
            }
        }
    }

    public static Level2RequestInfo GetLatestRequest(dbDataContext db, string shopAccount)
    {
        string shop = (shopAccount ?? "").Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(shop))
            return null;

        EnsureSchemaSafe(db);

        return db.ExecuteQuery<Level2RequestInfo>(
                "SELECT TOP 1 ID, taikhoan, TrangThai, NgayTao, NgayDuyet, AdminDuyet, GhiChuAdmin " +
                "FROM dbo.ShopLevel2Request_tb WHERE taikhoan = {0} ORDER BY NgayTao DESC, ID DESC",
                shop)
            .FirstOrDefault();
    }

    public static bool HasPendingRequest(dbDataContext db, string shopAccount)
    {
        string shop = (shopAccount ?? "").Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(shop))
            return false;

        EnsureSchemaSafe(db);
        int pending = db.ExecuteQuery<int>(
                "SELECT COUNT(1) FROM dbo.ShopLevel2Request_tb WHERE taikhoan = {0} AND TrangThai = {1}",
                shop,
                StatusPending)
            .FirstOrDefault();
        return pending > 0;
    }

    public static bool TryCreateRequest(dbDataContext db, string shopAccount, out string message)
    {
        message = "";
        string shop = (shopAccount ?? "").Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(shop))
        {
            message = "Không xác định được tài khoản shop.";
            return false;
        }

        EnsureSchemaSafe(db);

        if (ShopLevel_cl.IsAdvancedEnabled(db, shop))
        {
            message = "Shop này đã ở Level 2.";
            return false;
        }

        if (HasPendingRequest(db, shop))
        {
            message = "Đã có yêu cầu nâng cấp Level 2 đang chờ duyệt.";
            return false;
        }

        db.ExecuteCommand(
            "INSERT INTO dbo.ShopLevel2Request_tb (taikhoan, TrangThai, NgayTao) VALUES ({0}, {1}, {2})",
            shop,
            StatusPending,
            AhaTime_cl.Now);
        message = "Đã gửi yêu cầu nâng cấp Level 2. Vui lòng chờ admin duyệt.";
        return true;
    }

    public static List<Level2RequestInfo> LoadRequests(dbDataContext db, string keyword, int? status)
    {
        EnsureSchemaSafe(db);
        var list = db.ExecuteQuery<Level2RequestInfo>(
                "SELECT ID, taikhoan, TrangThai, NgayTao, NgayDuyet, AdminDuyet, GhiChuAdmin " +
                "FROM dbo.ShopLevel2Request_tb")
            .ToList();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            string key = keyword.Trim();
            int idSearch;
            bool byId = int.TryParse(key, out idSearch);
            if (byId)
                list = list.Where(p => p.ID == idSearch || (p.taikhoan ?? "").IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            else
                list = list.Where(p => (p.taikhoan ?? "").IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        if (status.HasValue)
            list = list.Where(p => p.TrangThai == status.Value).ToList();

        return list.OrderByDescending(p => p.NgayTao).ThenByDescending(p => p.ID).ToList();
    }

    public static bool ApproveRequest(dbDataContext db, int id, string adminUser, out string message)
    {
        message = "";
        if (db == null)
        {
            message = "Không kết nối được dữ liệu.";
            return false;
        }

        EnsureSchemaSafe(db);
        Level2RequestInfo req = db.ExecuteQuery<Level2RequestInfo>(
                "SELECT TOP 1 ID, taikhoan, TrangThai, NgayTao, NgayDuyet, AdminDuyet, GhiChuAdmin " +
                "FROM dbo.ShopLevel2Request_tb WHERE ID = {0}",
                id)
            .FirstOrDefault();
        if (req == null || req.TrangThai != StatusPending)
        {
            message = "Yêu cầu không còn ở trạng thái chờ duyệt.";
            return false;
        }

        string shop = (req.taikhoan ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(shop))
        {
            message = "Yêu cầu không hợp lệ.";
            return false;
        }

        if (!ShopLevel_cl.IsAdvancedEnabled(db, shop))
        {
            taikhoan_tb shopAccount;
            taikhoan_table_2023 ownerAdmin;
            bool createdOwnerAdmin;
            if (!ShopLevel_cl.EnableAdvancedForShop(db, shop, out shopAccount, out ownerAdmin, out createdOwnerAdmin))
            {
                message = "Không bật được Level 2 cho shop này.";
                return false;
            }
        }

        db.ExecuteCommand(
            "UPDATE dbo.ShopLevel2Request_tb SET TrangThai = {0}, NgayDuyet = {1}, AdminDuyet = {2}, GhiChuAdmin = {3} WHERE ID = {4}",
            StatusApproved,
            AhaTime_cl.Now,
            adminUser ?? "",
            "Đã duyệt nâng cấp Level 2.",
            id);
        message = "Đã duyệt nâng cấp Level 2.";
        return true;
    }

    public static bool RejectRequest(dbDataContext db, int id, string adminUser, string note, out string message)
    {
        message = "";
        if (db == null)
        {
            message = "Không kết nối được dữ liệu.";
            return false;
        }

        EnsureSchemaSafe(db);
        Level2RequestInfo req = db.ExecuteQuery<Level2RequestInfo>(
                "SELECT TOP 1 ID, taikhoan, TrangThai FROM dbo.ShopLevel2Request_tb WHERE ID = {0}",
                id)
            .FirstOrDefault();
        if (req == null || req.TrangThai != StatusPending)
        {
            message = "Yêu cầu không còn ở trạng thái chờ duyệt.";
            return false;
        }

        string reason = string.IsNullOrWhiteSpace(note) ? "Từ chối yêu cầu nâng cấp Level 2." : note.Trim();
        db.ExecuteCommand(
            "UPDATE dbo.ShopLevel2Request_tb SET TrangThai = {0}, NgayDuyet = {1}, AdminDuyet = {2}, GhiChuAdmin = {3} WHERE ID = {4}",
            StatusRejected,
            AhaTime_cl.Now,
            adminUser ?? "",
            reason,
            id);
        message = "Đã từ chối yêu cầu nâng cấp Level 2.";
        return true;
    }
}
