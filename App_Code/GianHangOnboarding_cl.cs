using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangOnboarding_cl
{
    public sealed class ShopInfoDraft
    {
        public string AccountKey { get; set; }
        public string FullName { get; set; }
        public string ShopName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string PickupAddress { get; set; }
    }

    public sealed class ShopInfoState
    {
        public ShopInfoDraft Draft { get; set; }
        public string AccessStatus { get; set; }
        public string RequestStatus { get; set; }
        public string AccessStatusText { get; set; }
        public string RequestStatusText { get; set; }
        public string ReviewNote { get; set; }
        public DateTime? RequestedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public bool CanSubmit { get; set; }
        public bool IsActive { get; set; }
        public bool IsPending { get; set; }
        public bool IsRejected { get; set; }
        public bool IsBlocked { get; set; }
        public string MissingProfileMessage { get; set; }
    }

    public sealed class RequestHistoryItem
    {
        public long Id { get; set; }
        public string RequestStatus { get; set; }
        public string RequestStatusText { get; set; }
        public string ReviewNote { get; set; }
        public DateTime? RequestedAt { get; set; }
        public string RequestedAtText { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string ReviewedAtText { get; set; }
    }

    public sealed class AdminShopInfo
    {
        public string AccountKey { get; set; }
        public string ShopName { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string PickupAddress { get; set; }
    }

    private sealed class ShopInfoRow
    {
        public long Id { get; set; }
        public string AccountKey { get; set; }
        public string ShopName { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string PickupAddress { get; set; }
        public long? LastRequestId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public static ShopInfoState LoadState(dbDataContext db, string accountKey)
    {
        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        string key = NormalizeAccountKey(accountKey);
        taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, key);
        if (account == null)
            return new ShopInfoState { Draft = new ShopInfoDraft { AccountKey = key }, CanSubmit = false };

        ShopInfoDraft draft = BuildDraft(db, account);
        CoreSpaceRequest_cl.SpaceRequestInfo request = CoreSpaceRequest_cl.GetLatestRequest(db, key, ModuleSpace_cl.GianHang);
        string accessStatus = SpaceAccess_cl.GetSpaceStatus(db, key, ModuleSpace_cl.GianHang);
        string requestStatus = request == null ? "" : NormalizeText(request.RequestStatus).ToLowerInvariant();
        string missingProfileMessage = BuildMissingProfileMessage(draft);
        bool isActive = string.Equals(accessStatus, SpaceAccess_cl.StatusActive, StringComparison.OrdinalIgnoreCase);
        bool isPending = string.Equals(requestStatus, CoreSpaceRequest_cl.StatusPending, StringComparison.OrdinalIgnoreCase);
        bool isRejected = string.Equals(requestStatus, CoreSpaceRequest_cl.StatusRejected, StringComparison.OrdinalIgnoreCase);
        bool isBlocked = string.Equals(accessStatus, SpaceAccess_cl.StatusBlocked, StringComparison.OrdinalIgnoreCase)
            || string.Equals(accessStatus, SpaceAccess_cl.StatusRevoked, StringComparison.OrdinalIgnoreCase);

        return new ShopInfoState
        {
            Draft = draft,
            AccessStatus = accessStatus,
            RequestStatus = requestStatus,
            AccessStatusText = HomeSpaceAccess_cl.GetAccessStatusText(accessStatus),
            RequestStatusText = HomeSpaceAccess_cl.GetRequestStatusText(requestStatus),
            ReviewNote = request == null ? "" : NormalizeText(request.ReviewNote),
            RequestedAt = request == null ? (DateTime?)null : request.RequestedAt,
            ReviewedAt = request == null ? (DateTime?)null : request.ReviewedAt,
            // Home co the gui lai yeu cau mo /gianhang sau khi bi khoa/thu hoi.
            // Trang thai blocked/revoked chi de canh bao va ghi chu admin, khong khoa nut gui lai.
            CanSubmit = !isActive && !isPending && string.IsNullOrWhiteSpace(missingProfileMessage),
            IsActive = isActive,
            IsPending = isPending,
            IsRejected = isRejected,
            IsBlocked = isBlocked,
            MissingProfileMessage = missingProfileMessage
        };
    }

    public static bool SubmitOnboarding(
        dbDataContext db,
        string accountKey,
        string shopNameRaw,
        string requestedBy,
        out string message)
    {
        message = "";
        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        string key = NormalizeAccountKey(accountKey);
        taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, key);
        if (account == null)
        {
            message = "Không tìm thấy tài khoản Home hiện tại.";
            return false;
        }

        if (!SpaceAccess_cl.CanAccessHome(db, account))
        {
            message = "Tài khoản hiện tại chưa đủ điều kiện làm tài khoản gốc Home.";
            return false;
        }

        ShopInfoState state = LoadState(db, key);
        if (state.IsActive)
        {
            message = "Tài khoản này đã có quyền truy cập /gianhang.";
            return false;
        }

        if (state.IsPending)
        {
            message = "Yêu cầu mở gian hàng đang chờ duyệt, bạn chưa cần gửi lại.";
            return false;
        }

        string shopName = NormalizeText(shopNameRaw);
        if (shopName == "")
        {
            message = "Vui lòng nhập tên gian hàng đối tác.";
            return false;
        }

        if (shopName.Length > 30)
        {
            message = "Tên gian hàng đối tác chỉ được tối đa 30 ký tự.";
            return false;
        }

        string contactEmail = ResolveContactEmail(account, state.Draft);
        string contactPhone = ResolveContactPhone(account, state.Draft);
        string pickupAddress = ResolvePickupAddress(account, state.Draft);
        if (BuildMissingProfileMessage(new ShopInfoDraft
        {
            ContactEmail = contactEmail,
            ContactPhone = contactPhone,
            PickupAddress = pickupAddress
        }) != "")
        {
            message = "Hồ sơ Home hiện chưa đủ email, số điện thoại hoặc địa chỉ để mở gian hàng.";
            return false;
        }

        UpsertShopInfo(
            db,
            key,
            shopName,
            NormalizeText(account.hoten),
            contactPhone,
            contactEmail,
            pickupAddress);

        SyncAccountShopFields(account, shopName, contactPhone, contactEmail, pickupAddress);
        db.SubmitChanges();

        bool ok = CoreSpaceRequest_cl.TryCreateRequest(
            db,
            key,
            ModuleSpace_cl.GianHang,
            "gianhang_onboarding",
            NormalizeText(requestedBy),
            null,
            out message);

        if (!ok)
            return false;

        CoreSpaceRequest_cl.SpaceRequestInfo latest = CoreSpaceRequest_cl.GetLatestRequest(db, key, ModuleSpace_cl.GianHang);
        if (latest != null)
        {
            db.ExecuteCommand(
                "UPDATE dbo.CoreGianHangOnboarding_tb SET LastRequestId = {1}, UpdatedAt = {2} WHERE AccountKey = {0}",
                key,
                latest.Id,
                AhaTime_cl.Now);
        }

        message = "Đã gửi đăng ký gian hàng đối tác. Admin sẽ duyệt và mở quyền /gianhang cho chính tài khoản Home này.";
        return true;
    }

    public static List<RequestHistoryItem> LoadHistory(dbDataContext db, string accountKey)
    {
        List<CoreSpaceRequest_cl.SpaceRequestInfo> rows = CoreSpaceRequest_cl.LoadRequests(db, NormalizeAccountKey(accountKey), ModuleSpace_cl.GianHang, "");
        return rows.Select(p => new RequestHistoryItem
        {
            Id = p.Id,
            RequestStatus = NormalizeText(p.RequestStatus),
            RequestStatusText = HomeSpaceAccess_cl.GetRequestStatusText(p.RequestStatus),
            ReviewNote = NormalizeText(p.ReviewNote),
            RequestedAt = p.RequestedAt,
            RequestedAtText = p.RequestedAt.HasValue ? p.RequestedAt.Value.ToString("dd/MM/yyyy HH:mm") : "--",
            ReviewedAt = p.ReviewedAt,
            ReviewedAtText = p.ReviewedAt.HasValue ? p.ReviewedAt.Value.ToString("dd/MM/yyyy HH:mm") : "--"
        }).ToList();
    }

    public static Dictionary<string, AdminShopInfo> LoadAdminSummaryMap(dbDataContext db, IEnumerable<string> accountKeys)
    {
        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        HashSet<string> keys = new HashSet<string>(
            (accountKeys ?? Enumerable.Empty<string>())
                .Select(NormalizeAccountKey)
                .Where(p => p != ""),
            StringComparer.OrdinalIgnoreCase);

        List<ShopInfoRow> rows = db.ExecuteQuery<ShopInfoRow>(
            "SELECT Id, AccountKey, ShopName, ContactName, ContactPhone, ContactEmail, PickupAddress, LastRequestId, CreatedAt, UpdatedAt FROM dbo.CoreGianHangOnboarding_tb")
            .ToList();

        Dictionary<string, AdminShopInfo> map = new Dictionary<string, AdminShopInfo>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < rows.Count; i++)
        {
            ShopInfoRow row = rows[i];
            string key = NormalizeAccountKey(row.AccountKey);
            if (key == "")
                continue;
            if (keys.Count > 0 && !keys.Contains(key))
                continue;

            map[key] = new AdminShopInfo
            {
                AccountKey = key,
                ShopName = NormalizeText(row.ShopName),
                ContactName = NormalizeText(row.ContactName),
                ContactPhone = NormalizeText(row.ContactPhone),
                ContactEmail = NormalizeText(row.ContactEmail),
                PickupAddress = NormalizeText(row.PickupAddress)
            };
        }

        return map;
    }

    private static ShopInfoDraft BuildDraft(dbDataContext db, taikhoan_tb account)
    {
        ShopInfoRow row = LoadRow(db, NormalizeAccountKey(account.taikhoan));
        return new ShopInfoDraft
        {
            AccountKey = NormalizeAccountKey(account.taikhoan),
            FullName = NormalizeText(account.hoten),
            ShopName = ResolveShopName(account, row),
            ContactEmail = ResolveContactEmail(account, row),
            ContactPhone = ResolveContactPhone(account, row),
            PickupAddress = ResolvePickupAddress(account, row)
        };
    }

    private static ShopInfoRow LoadRow(dbDataContext db, string accountKey)
    {
        string key = NormalizeAccountKey(accountKey);
        if (key == "")
            return null;

        return db.ExecuteQuery<ShopInfoRow>(
            "SELECT TOP 1 Id, AccountKey, ShopName, ContactName, ContactPhone, ContactEmail, PickupAddress, LastRequestId, CreatedAt, UpdatedAt " +
            "FROM dbo.CoreGianHangOnboarding_tb WHERE AccountKey = {0}",
            key).FirstOrDefault();
    }

    private static void UpsertShopInfo(
        dbDataContext db,
        string accountKey,
        string shopName,
        string contactName,
        string contactPhone,
        string contactEmail,
        string pickupAddress)
    {
        string key = NormalizeAccountKey(accountKey);
        if (LoadRow(db, key) == null)
        {
            db.ExecuteCommand(
                "INSERT INTO dbo.CoreGianHangOnboarding_tb (AccountKey, ShopName, ContactName, ContactPhone, ContactEmail, PickupAddress, CreatedAt, UpdatedAt) " +
                "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {6})",
                key,
                NormalizeText(shopName),
                NormalizeText(contactName),
                NormalizeText(contactPhone),
                NormalizeText(contactEmail),
                NormalizeText(pickupAddress),
                AhaTime_cl.Now);
            return;
        }

        db.ExecuteCommand(
            "UPDATE dbo.CoreGianHangOnboarding_tb SET ShopName = {1}, ContactName = {2}, ContactPhone = {3}, ContactEmail = {4}, PickupAddress = {5}, UpdatedAt = {6} " +
            "WHERE AccountKey = {0}",
            key,
            NormalizeText(shopName),
            NormalizeText(contactName),
            NormalizeText(contactPhone),
            NormalizeText(contactEmail),
            NormalizeText(pickupAddress),
            AhaTime_cl.Now);
    }

    private static void SyncAccountShopFields(taikhoan_tb account, string shopName, string contactPhone, string contactEmail, string pickupAddress)
    {
        if (account == null)
            return;

        account.ten_shop = NormalizeText(shopName);
        account.sdt_shop = NormalizeText(contactPhone);
        account.email_shop = NormalizeText(contactEmail);
        account.diachi_shop = NormalizeText(pickupAddress);
    }

    private static string ResolveShopName(taikhoan_tb account, ShopInfoRow row)
    {
        string value = row == null ? "" : NormalizeText(row.ShopName);
        if (value != "")
            return value;

        value = NormalizeText(account.ten_shop);
        if (value != "")
            return value;

        return NormalizeText(account.hoten);
    }

    private static string ResolveContactEmail(taikhoan_tb account, ShopInfoDraft draft)
    {
        if (draft != null && NormalizeText(draft.ContactEmail) != "")
            return NormalizeText(draft.ContactEmail);
        return ResolveContactEmail(account, (ShopInfoRow)null);
    }

    private static string ResolveContactEmail(taikhoan_tb account, ShopInfoRow row)
    {
        string value = row == null ? "" : NormalizeText(row.ContactEmail);
        if (value != "")
            return value;

        value = NormalizeText(account.email_shop);
        if (value != "")
            return value;

        return NormalizeText(account.email);
    }

    private static string ResolveContactPhone(taikhoan_tb account, ShopInfoDraft draft)
    {
        if (draft != null && NormalizeText(draft.ContactPhone) != "")
            return NormalizeText(draft.ContactPhone);
        return ResolveContactPhone(account, (ShopInfoRow)null);
    }

    private static string ResolveContactPhone(taikhoan_tb account, ShopInfoRow row)
    {
        string value = row == null ? "" : NormalizeText(row.ContactPhone);
        if (value != "")
            return value;

        value = NormalizeText(account.sdt_shop);
        if (value != "")
            return value;

        return NormalizeText(account.dienthoai);
    }

    private static string ResolvePickupAddress(taikhoan_tb account, ShopInfoDraft draft)
    {
        if (draft != null && NormalizeText(draft.PickupAddress) != "")
            return NormalizeText(draft.PickupAddress);
        return ResolvePickupAddress(account, (ShopInfoRow)null);
    }

    private static string ResolvePickupAddress(taikhoan_tb account, ShopInfoRow row)
    {
        string value = row == null ? "" : NormalizeText(row.PickupAddress);
        if (value != "")
            return value;

        value = NormalizeText(account.diachi_shop);
        if (value != "")
            return value;

        return NormalizeText(account.diachi);
    }

    private static string BuildMissingProfileMessage(ShopInfoDraft draft)
    {
        List<string> missing = new List<string>();
        string email = NormalizeText(draft == null ? "" : draft.ContactEmail);
        string phone = NormalizeText(draft == null ? "" : draft.ContactPhone);
        string address = NormalizeText(draft == null ? "" : draft.PickupAddress);

        if (!AccountAuth_cl.IsValidEmail(email))
            missing.Add("email");
        if (!AccountAuth_cl.IsValidPhone(phone))
            missing.Add("số điện thoại");
        if (address == "")
            missing.Add("địa chỉ lấy hàng");

        if (missing.Count == 0)
            return "";

        return "Hồ sơ Home hiện đang thiếu " + string.Join(", ", missing.ToArray()) + ". Vui lòng cập nhật trước khi mở gian hàng.";
    }

    private static string NormalizeAccountKey(string accountKey)
    {
        return (accountKey ?? "").Trim().ToLowerInvariant();
    }

    private static string NormalizeText(string value)
    {
        return (value ?? "").Trim();
    }
}
