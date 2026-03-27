using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class GianHangAdminPersonHub_cl
{
    private const string StatusActive = "active";
    private const string StatusPending = "pending";
    private const string StatusRevoked = "revoked";

    public sealed class PersonSourceRef
    {
        public string SourceType { get; set; }
        public string SourceLabel { get; set; }
        public string SourceKey { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string NormalizedPhone { get; set; }
        public string Email { get; set; }
        public string RoleLabel { get; set; }
        public string DetailUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsAdminAccessEligible { get; set; }
        public string AdminAccessStatus { get; set; }
        public string AdminAccessLabel { get; set; }
        public string AdminAccessCss { get; set; }
        public string SourceLifecycleStatus { get; set; }
        public string SourceLifecycleLabel { get; set; }
        public string SourceLifecycleCss { get; set; }
        public string SourceLifecycleNote { get; set; }
        public bool IsSourceInactive { get; set; }
    }

    public sealed class PersonLinkInfo
    {
        public string Status { get; set; }
        public string StatusLabel { get; set; }
        public string LinkCss { get; set; }
        public string PendingPhone { get; set; }
        public string PrimaryName { get; set; }
        public taikhoan_tb LinkedHomeAccount { get; set; }
    }

    public sealed class PersonSummary
    {
        public string PrimaryName { get; set; }
        public string DisplayPhone { get; set; }
        public string NormalizedPhone { get; set; }
        public string RolesSummary { get; set; }
        public int SourceCount { get; set; }
        public int RemovedSourceCount { get; set; }
        public PersonLinkInfo LinkInfo { get; set; }
        public string DetailUrl { get; set; }
    }

    public sealed class PersonRemovedSourceRef
    {
        public string SourceType { get; set; }
        public string SourceLabel { get; set; }
        public string SourceKey { get; set; }
        public string RoleLabel { get; set; }
        public string DisplayName { get; set; }
        public string EventType { get; set; }
        public string EventNote { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public sealed class PersonDetail
    {
        public string OwnerAccountKey { get; set; }
        public string PrimaryName { get; set; }
        public string DisplayPhone { get; set; }
        public string NormalizedPhone { get; set; }
        public string RoleSummary { get; set; }
        public bool HasSourceRoles { get; set; }
        public bool HasInternalStaffRole { get; set; }
        public bool HasAdminEligibleRole { get; set; }
        public bool HasGrantedAdminAccess { get; set; }
        public bool HasPendingAdminAccess { get; set; }
        public string AdminAccessSummary { get; set; }
        public DateTime? LinkCreatedAt { get; set; }
        public DateTime? LinkUpdatedAt { get; set; }
        public PersonLinkInfo LinkInfo { get; set; }
        public IList<PersonSourceRef> Sources { get; set; }
        public IList<PersonRemovedSourceRef> RemovedSources { get; set; }
    }

    private sealed class PersonLinkRaw
    {
        public long Id { get; set; }
        public string OwnerAccountKey { get; set; }
        public string NormalizedPhone { get; set; }
        public string PrimaryName { get; set; }
        public string HomeAccountKey { get; set; }
        public string LinkStatus { get; set; }
        public string PendingPhone { get; set; }
        public string InviteLookupKey { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    private sealed class LegacyMemberRaw
    {
        public long Id { get; set; }
        public string OwnerAccountKey { get; set; }
        public string MemberAccountKey { get; set; }
        public string LegacyUser { get; set; }
        public string MembershipStatus { get; set; }
        public string RoleLabel { get; set; }
        public string MemberType { get; set; }
        public string PendingPhone { get; set; }
        public string InviteLookupKey { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    private sealed class PersonSourceEventRaw
    {
        public long Id { get; set; }
        public string OwnerAccountKey { get; set; }
        public string NormalizedPhone { get; set; }
        public string SourceType { get; set; }
        public string SourceLabel { get; set; }
        public string SourceKey { get; set; }
        public string RoleLabel { get; set; }
        public string DisplayName { get; set; }
        public string EventType { get; set; }
        public string EventNote { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public static string BuildDetailUrl(string phone)
    {
        string normalizedPhone = NormalizePhone(phone);
        if (normalizedPhone == "")
            return "/gianhang/admin/quan-ly-con-nguoi/Default.aspx";

        return "/gianhang/admin/quan-ly-con-nguoi/chi-tiet.aspx?phone=" + HttpUtility.UrlEncode(normalizedPhone);
    }

    public static IList<PersonSummary> GetPeople(dbDataContext db, string ownerAccountKey, string keywordRaw, string statusRaw)
    {
        return GetPeople(db, ownerAccountKey, keywordRaw, statusRaw, "");
    }

    public static IList<PersonSummary> GetPeople(dbDataContext db, string ownerAccountKey, string keywordRaw, string statusRaw, string sourceRaw)
    {
        List<PersonSummary> items = new List<PersonSummary>();
        if (db == null)
            return items;

        string ownerKey = Normalize(ownerAccountKey);
        if (ownerKey == "")
            return items;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        EnsureLegacyBackfill(db, ownerKey);

        string keyword = (keywordRaw ?? "").Trim();
        string normalizedKeywordPhone = NormalizePhone(keyword);
        string normalizedStatus = NormalizeStatusFilter(statusRaw);
        string normalizedSource = NormalizeSourceFilter(sourceRaw);

        Dictionary<string, List<PersonSourceRef>> sourceMap = CollectPhoneSources(db, ownerKey)
            .GroupBy(p => p.NormalizedPhone)
            .ToDictionary(g => g.Key, g => g.OrderBy(p => GetSourcePriority(p.SourceType)).ThenBy(p => p.Name ?? "").ToList(), StringComparer.OrdinalIgnoreCase);

        Dictionary<string, PersonLinkRaw> linkRows = GetPersonLinkRows(db, ownerKey)
            .GroupBy(p => NormalizePhone(p.NormalizedPhone))
            .Where(g => g.Key != "")
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt ?? DateTime.MinValue).ThenByDescending(p => p.Id).First(),
                StringComparer.OrdinalIgnoreCase);

        Dictionary<string, int> removedSourceCounts = GetRemovedSourceEvents(db, ownerKey)
            .GroupBy(p => NormalizePhone(p.NormalizedPhone))
            .Where(g => g.Key != "")
            .ToDictionary(
                g => g.Key,
                g => g.Count(),
                StringComparer.OrdinalIgnoreCase);

        foreach (string normalizedPhone in sourceMap.Keys.Union(linkRows.Keys, StringComparer.OrdinalIgnoreCase))
        {
            if (normalizedPhone == "")
                continue;

            List<PersonSourceRef> groupItems;
            if (!sourceMap.TryGetValue(normalizedPhone, out groupItems))
                groupItems = new List<PersonSourceRef>();

            PersonLinkRaw raw = null;
            linkRows.TryGetValue(normalizedPhone, out raw);
            string primaryName = ResolvePrimaryName(groupItems);
            if (primaryName == "" && raw != null)
                primaryName = (raw.PrimaryName ?? "").Trim();
            if (primaryName == "")
                primaryName = normalizedPhone;

            PersonLinkInfo linkInfo = BuildLinkInfo(db, raw, primaryName);
            string rolesSummary = string.Join(" • ", groupItems.Select(p => p.RoleLabel).Where(p => !string.IsNullOrWhiteSpace(p)).Distinct());
            if (rolesSummary == "")
                rolesSummary = raw == null ? "Hồ sơ người" : "Chưa còn vai trò nguồn";

            string displayPhone = groupItems.Count == 0
                ? (raw != null && !string.IsNullOrWhiteSpace(raw.PendingPhone) ? raw.PendingPhone.Trim() : normalizedPhone)
                : ResolveDisplayPhone(groupItems, normalizedPhone);

            if (!MatchFilter(groupItems, linkInfo, primaryName, displayPhone, keyword, normalizedKeywordPhone, normalizedStatus, normalizedSource))
                continue;

            items.Add(new PersonSummary
            {
                PrimaryName = primaryName,
                DisplayPhone = displayPhone,
                NormalizedPhone = normalizedPhone,
                RolesSummary = rolesSummary,
                SourceCount = groupItems.Count,
                RemovedSourceCount = removedSourceCounts.ContainsKey(normalizedPhone) ? removedSourceCounts[normalizedPhone] : 0,
                LinkInfo = linkInfo,
                DetailUrl = BuildDetailUrl(normalizedPhone)
            });
        }

        return items
            .OrderByDescending(p => GetStatusPriority(p.LinkInfo == null ? "" : p.LinkInfo.Status))
            .ThenBy(p => p.PrimaryName ?? "")
            .ThenBy(p => p.DisplayPhone ?? "")
            .ToList();
    }

    public static PersonDetail GetPersonDetail(dbDataContext db, string ownerAccountKey, string phoneRaw)
    {
        if (db == null)
            return null;

        string ownerKey = Normalize(ownerAccountKey);
        string normalizedPhone = NormalizePhone(phoneRaw);
        if (ownerKey == "" || normalizedPhone == "")
            return null;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        EnsureLegacyBackfill(db, ownerKey);

        List<PersonSourceRef> sources = CollectPhoneSources(db, ownerKey)
            .Where(p => string.Equals(p.NormalizedPhone, normalizedPhone, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => GetSourcePriority(p.SourceType))
            .ThenBy(p => p.Name ?? "")
            .ToList();

        PersonLinkRaw raw = GetPersonLinkRow(db, ownerKey, normalizedPhone);
        IList<PersonRemovedSourceRef> removedSources = GetRemovedSources(db, ownerKey, normalizedPhone);
        PersonLinkInfo linkInfo = BuildLinkInfo(db, raw, ResolvePrimaryName(sources));
        if (sources.Count == 0 && raw == null && (linkInfo == null || string.IsNullOrWhiteSpace(linkInfo.Status)))
            return null;

        string primaryName = ResolvePrimaryName(sources);
        if (primaryName == "" && raw != null)
            primaryName = (raw.PrimaryName ?? "").Trim();
        if (primaryName == "" && linkInfo != null)
            primaryName = (linkInfo.PrimaryName ?? "").Trim();

        string roleSummary = string.Join(" • ", sources.Select(p => p.RoleLabel).Where(p => !string.IsNullOrWhiteSpace(p)).Distinct());
        if (roleSummary == "")
            roleSummary = raw == null
                ? "Chưa có vai trò nguồn nào trong dữ liệu hiện tại."
                : "Hiện chưa còn vai trò nguồn nào trong dữ liệu hiện tại. Liên kết Home trung tâm vẫn được giữ để bạn chủ động quản lý.";

        List<PersonSourceRef> eligibleSources = sources
            .Where(p => p != null && p.IsAdminAccessEligible)
            .ToList();

        bool hasGrantedAdminAccess = eligibleSources.Any(p => string.Equals(Normalize(p.AdminAccessStatus), StatusActive, StringComparison.OrdinalIgnoreCase));
        bool hasPendingAdminAccess = !hasGrantedAdminAccess && eligibleSources.Any(p => string.Equals(Normalize(p.AdminAccessStatus), StatusPending, StringComparison.OrdinalIgnoreCase));

        return new PersonDetail
        {
            OwnerAccountKey = ownerKey,
            PrimaryName = primaryName == "" ? normalizedPhone : primaryName,
            DisplayPhone = sources.Count == 0
                ? (raw != null && !string.IsNullOrWhiteSpace(raw.PendingPhone) ? raw.PendingPhone.Trim() : normalizedPhone)
                : ResolveDisplayPhone(sources, normalizedPhone),
            NormalizedPhone = normalizedPhone,
            RoleSummary = roleSummary,
            HasSourceRoles = sources.Count > 0,
            HasInternalStaffRole = sources.Any(p => string.Equals(p.SourceType, "staff", StringComparison.OrdinalIgnoreCase)),
            HasAdminEligibleRole = eligibleSources.Count > 0,
            HasGrantedAdminAccess = hasGrantedAdminAccess,
            HasPendingAdminAccess = hasPendingAdminAccess,
            AdminAccessSummary = ResolveAdminAccessSummary(eligibleSources, hasGrantedAdminAccess, hasPendingAdminAccess),
            LinkCreatedAt = raw == null ? null : raw.CreatedAt,
            LinkUpdatedAt = raw == null ? null : raw.UpdatedAt,
            LinkInfo = linkInfo,
            Sources = sources,
            RemovedSources = removedSources
        };
    }

    public static IList<PersonSourceRef> GetOtherSourcesForPhone(dbDataContext db, string ownerAccountKey, string phoneRaw, string currentSourceType, string currentSourceKey)
    {
        List<PersonSourceRef> items = new List<PersonSourceRef>();
        if (db == null)
            return items;

        string ownerKey = Normalize(ownerAccountKey);
        string normalizedPhone = NormalizePhone(phoneRaw);
        if (ownerKey == "" || normalizedPhone == "")
            return items;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        EnsureLegacyBackfill(db, ownerKey);

        string normalizedSourceType = Normalize(currentSourceType);
        string normalizedSourceKey = Normalize(currentSourceKey);

        return CollectPhoneSources(db, ownerKey)
            .Where(p => string.Equals(p.NormalizedPhone, normalizedPhone, StringComparison.OrdinalIgnoreCase))
            .Where(p =>
                !(string.Equals(Normalize(p.SourceType), normalizedSourceType, StringComparison.OrdinalIgnoreCase) &&
                  string.Equals(Normalize(p.SourceKey), normalizedSourceKey, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(p => GetSourcePriority(p.SourceType))
            .ThenBy(p => p.Name ?? "")
            .ToList();
    }

    public static PersonSourceRef GetSourceInfo(dbDataContext db, string ownerAccountKey, string phoneRaw, string sourceType, string sourceKey)
    {
        if (db == null)
            return null;

        string ownerKey = Normalize(ownerAccountKey);
        string normalizedPhone = NormalizePhone(phoneRaw);
        string normalizedSourceType = Normalize(sourceType);
        string normalizedSourceKey = Normalize(sourceKey);
        if (ownerKey == "" || normalizedPhone == "" || normalizedSourceType == "")
            return null;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        EnsureLegacyBackfill(db, ownerKey);

        return CollectPhoneSources(db, ownerKey)
            .Where(p => string.Equals(p.NormalizedPhone, normalizedPhone, StringComparison.OrdinalIgnoreCase))
            .Where(p => string.Equals(Normalize(p.SourceType), normalizedSourceType, StringComparison.OrdinalIgnoreCase))
            .Where(p => normalizedSourceKey == "" || string.Equals(Normalize(p.SourceKey), normalizedSourceKey, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => GetSourcePriority(p.SourceType))
            .ThenBy(p => p.Name ?? "")
            .FirstOrDefault();
    }

    public static PersonLinkInfo GetLinkInfo(dbDataContext db, string ownerAccountKey, string phoneRaw, string defaultName)
    {
        if (db == null)
            return new PersonLinkInfo { Status = "", StatusLabel = "Chưa liên kết", LinkCss = "bg-gray fg-white", PrimaryName = defaultName ?? "" };

        string ownerKey = Normalize(ownerAccountKey);
        string normalizedPhone = NormalizePhone(phoneRaw);
        if (ownerKey == "" || normalizedPhone == "")
        {
            return new PersonLinkInfo
            {
                Status = "",
                StatusLabel = "Thiếu số điện thoại",
                LinkCss = "bg-gray fg-white",
                PrimaryName = defaultName ?? ""
            };
        }

        return BuildLinkInfo(db, GetPersonLinkRow(db, ownerKey, normalizedPhone), defaultName);
    }

    public static bool TryLinkExistingHome(dbDataContext db, string ownerAccountKey, string phoneRaw, string homeAccountOrPhone, string displayName, string createdBy, out string message)
    {
        message = "";
        if (db == null)
        {
            message = "Không có kết nối dữ liệu.";
            return false;
        }

        string ownerKey = Normalize(ownerAccountKey);
        string normalizedPhone = NormalizePhone(phoneRaw);
        string lookupRaw = (homeAccountOrPhone ?? "").Trim();
        if (ownerKey == "" || normalizedPhone == "")
        {
            message = "Hồ sơ người này chưa có số điện thoại để liên kết.";
            return false;
        }

        if (lookupRaw == "")
        {
            message = "Hãy nhập tài khoản Home hoặc số điện thoại của tài khoản AhaSale đã tồn tại.";
            return false;
        }

        taikhoan_tb member = ResolveHomeAccount(db, lookupRaw);
        if (member == null)
        {
            message = "Không tìm thấy tài khoản AhaSale phù hợp để liên kết.";
            return false;
        }

        if (!PortalScope_cl.CanLoginHome(member.taikhoan, member.phanloai, member.permission))
        {
            message = "Tài khoản đã chọn hiện không dùng được như một tài khoản Home.";
            return false;
        }

        UpsertPersonLinkRow(db, ownerKey, normalizedPhone, (displayName ?? "").Trim(), Normalize(member.taikhoan), StatusActive, null, null, createdBy, true);
        SyncLegacyMembershipsForPhone(db, ownerKey, normalizedPhone, Normalize(member.taikhoan), null, StatusActive, createdBy);
        message = "Đã liên kết tài khoản Home cho hồ sơ người này. Nếu số điện thoại trùng ở các module khác thì toàn bộ vai trò tương ứng cũng sẽ nhận trạng thái đã liên kết.";
        return true;
    }

    public static bool TryCreatePendingLink(dbDataContext db, string ownerAccountKey, string phoneRaw, string displayName, string createdBy, out string message)
    {
        message = "";
        if (db == null)
        {
            message = "Không có kết nối dữ liệu.";
            return false;
        }

        string ownerKey = Normalize(ownerAccountKey);
        string normalizedPhone = NormalizePhone(phoneRaw);
        if (ownerKey == "" || normalizedPhone == "")
        {
            message = "Hồ sơ người này chưa có số điện thoại để tạo chờ liên kết.";
            return false;
        }

        taikhoan_tb existingHome = ResolveHomeAccount(db, normalizedPhone);
        if (existingHome != null && PortalScope_cl.CanLoginHome(existingHome.taikhoan, existingHome.phanloai, existingHome.permission))
        {
            message = "Số điện thoại này đã có tài khoản AhaSale. Hãy dùng nút liên kết tài khoản đã có để gắn ngay.";
            return false;
        }

        UpsertPersonLinkRow(db, ownerKey, normalizedPhone, (displayName ?? "").Trim(), null, StatusPending, normalizedPhone, normalizedPhone, createdBy, true);
        SyncLegacyMembershipsForPhone(db, ownerKey, normalizedPhone, null, normalizedPhone, StatusPending, createdBy);
        message = "Đã tạo trạng thái chờ liên kết theo số điện thoại này. Khi tài khoản Home tương ứng đăng ký hoặc đăng nhập sau này, hệ thống sẽ tự gắn vào toàn bộ vai trò trùng số điện thoại.";
        return true;
    }

    public static bool TryUnlink(dbDataContext db, string ownerAccountKey, string phoneRaw, string createdBy, out string message)
    {
        message = "";
        if (db == null)
        {
            message = "Không có kết nối dữ liệu.";
            return false;
        }

        string ownerKey = Normalize(ownerAccountKey);
        string normalizedPhone = NormalizePhone(phoneRaw);
        if (ownerKey == "" || normalizedPhone == "")
        {
            message = "Không xác định được hồ sơ người cần gỡ liên kết.";
            return false;
        }

        PersonLinkRaw row = GetPersonLinkRow(db, ownerKey, normalizedPhone);
        if (row == null)
        {
            message = "Hồ sơ người này hiện chưa có liên kết Home để gỡ.";
            return false;
        }

        UpsertPersonLinkRow(db, ownerKey, normalizedPhone, row.PrimaryName, null, StatusRevoked, null, null, createdBy, true);
        SyncLegacyMembershipsForPhone(db, ownerKey, normalizedPhone, null, null, StatusRevoked, createdBy);
        message = "Đã gỡ liên kết Home của hồ sơ người này.";
        return true;
    }

    public static void SyncSourcePhoneState(dbDataContext db, string ownerAccountKey, string oldPhoneRaw, string newPhoneRaw, string displayName, string createdBy)
    {
        if (db == null)
            return;

        string ownerKey = Normalize(ownerAccountKey);
        string oldPhone = NormalizePhone(oldPhoneRaw);
        string newPhone = NormalizePhone(newPhoneRaw);
        string name = (displayName ?? "").Trim();
        string actor = (createdBy ?? "").Trim();

        if (ownerKey == "")
            return;

        if (newPhone != "")
        {
            PersonLinkRaw newRow = GetPersonLinkRow(db, ownerKey, newPhone);
            if (newRow != null)
            {
                UpsertPersonLinkRow(
                    db,
                    ownerKey,
                    newPhone,
                    name == "" ? newRow.PrimaryName : name,
                    newRow.HomeAccountKey,
                    newRow.LinkStatus,
                    newRow.PendingPhone,
                    newRow.InviteLookupKey,
                    actor,
                    true);
                return;
            }
        }

        if (oldPhone == "" || newPhone == "" || string.Equals(oldPhone, newPhone, StringComparison.OrdinalIgnoreCase))
            return;

        PersonLinkRaw oldRow = GetPersonLinkRow(db, ownerKey, oldPhone);
        if (oldRow == null)
            return;

        UpsertPersonLinkRow(
            db,
            ownerKey,
            newPhone,
            name == "" ? oldRow.PrimaryName : name,
            oldRow.HomeAccountKey,
            oldRow.LinkStatus,
            oldRow.PendingPhone,
            oldRow.InviteLookupKey,
            actor,
            true);

        SyncLegacyMembershipsForPhone(
            db,
            ownerKey,
            newPhone,
            oldRow.HomeAccountKey,
            oldRow.PendingPhone,
            Normalize(oldRow.LinkStatus),
            actor);
    }

    public static void PreserveLinkAfterSourceRemoval(dbDataContext db, string ownerAccountKey, string phoneRaw, string displayName, string createdBy, string sourceType = "", string sourceLabel = "", string sourceKey = "", string roleLabel = "", string eventNote = "")
    {
        if (db == null)
            return;

        string ownerKey = Normalize(ownerAccountKey);
        string normalizedPhone = NormalizePhone(phoneRaw);
        if (ownerKey == "" || normalizedPhone == "")
            return;

        PersonLinkRaw existing = GetPersonLinkRow(db, ownerKey, normalizedPhone);
        if (existing == null)
            return;

        string name = (displayName ?? "").Trim();
        if (name == "")
            name = (existing.PrimaryName ?? "").Trim();

        UpsertPersonLinkRow(
            db,
            ownerKey,
            normalizedPhone,
            name,
            existing.HomeAccountKey,
            existing.LinkStatus,
            existing.PendingPhone,
            existing.InviteLookupKey,
            createdBy,
            true);

        AppendRemovedSourceEvent(
            db,
            ownerKey,
            normalizedPhone,
            sourceType,
            sourceLabel,
            sourceKey,
            roleLabel,
            name,
            createdBy,
            eventNote);
    }

    public static void PromotePendingLinks(dbDataContext db, string homeAccountKey)
    {
        if (db == null)
            return;

        taikhoan_tb home = RootAccount_cl.GetByAccountKey(db, homeAccountKey);
        if (home == null)
            return;

        string homeKey = Normalize(home.taikhoan);
        string normalizedPhone = NormalizePhone(home.dienthoai);
        if (homeKey == "" || normalizedPhone == "")
            return;

        List<PersonLinkRaw> pendingRows = db.ExecuteQuery<PersonLinkRaw>(
            "SELECT Id, OwnerAccountKey, NormalizedPhone, PrimaryName, HomeAccountKey, LinkStatus, PendingPhone, InviteLookupKey, CreatedBy, AcceptedAt, CreatedAt, UpdatedAt " +
            "FROM dbo.CoreGianHangPersonLink_tb WHERE NormalizedPhone = {0} AND LinkStatus = {1}",
            normalizedPhone,
            StatusPending).ToList();

        if (pendingRows.Count == 0)
            return;

        foreach (PersonLinkRaw row in pendingRows)
        {
            string ownerKey = Normalize(row.OwnerAccountKey);
            if (ownerKey == "")
                continue;

            UpsertPersonLinkRow(db, ownerKey, normalizedPhone, row.PrimaryName, homeKey, StatusActive, null, null, "system", true);
            SyncLegacyMembershipsForPhone(db, ownerKey, normalizedPhone, homeKey, null, StatusActive, "system");
        }
    }

    public static bool CanManagePeopleHub(string user)
    {
        return AdvancedAdminPermission_cl.CanAccessPeopleHub(user);
    }

    private static void EnsureLegacyBackfill(dbDataContext db, string ownerAccountKey)
    {
        if (db == null)
            return;

        string ownerKey = Normalize(ownerAccountKey);
        if (ownerKey == "")
            return;

        List<LegacyMemberRaw> rows = db.ExecuteQuery<LegacyMemberRaw>(
            "SELECT Id, OwnerAccountKey, MemberAccountKey, LegacyUser, MembershipStatus, RoleLabel, MemberType, PendingPhone, InviteLookupKey, UpdatedAt " +
            "FROM dbo.CoreGianHangAdminMember_tb WHERE OwnerAccountKey = {0} AND MembershipStatus IN ({1}, {2})",
            ownerKey,
            StatusActive,
            StatusPending).ToList();

        if (rows.Count == 0)
            return;

        foreach (LegacyMemberRaw row in rows)
        {
            string legacyUser = Normalize(row.LegacyUser);
            if (legacyUser == "")
                continue;

            taikhoan_table_2023 legacy = db.taikhoan_table_2023s.FirstOrDefault(p => p.taikhoan == legacyUser && p.user_parent == ownerKey);
            if (legacy == null)
                continue;

            string normalizedPhone = NormalizePhone(legacy.dienthoai);
            if (normalizedPhone == "")
                continue;

            UpsertPersonLinkRow(
                db,
                ownerKey,
                normalizedPhone,
                (legacy.hoten ?? "").Trim(),
                Normalize(row.MemberAccountKey),
                Normalize(row.MembershipStatus),
                (row.PendingPhone ?? "").Trim(),
                (row.InviteLookupKey ?? "").Trim(),
                "legacy-sync",
                false);
        }
    }

    private static IEnumerable<PersonSourceRef> CollectPhoneSources(dbDataContext db, string ownerAccountKey)
    {
        List<PersonSourceRef> items = new List<PersonSourceRef>();
        string ownerKey = Normalize(ownerAccountKey);
        if (db == null || ownerKey == "")
            return items;

        List<string> branchIds = db.taikhoan_table_2023s
            .Where(p => p.user_parent == ownerKey && p.id_chinhanh != null && p.id_chinhanh != "")
            .Select(p => p.id_chinhanh)
            .Distinct()
            .ToList();

        string currentBranch = ((HttpContext.Current != null && HttpContext.Current.Session != null ? HttpContext.Current.Session["chinhanh"] : "") + "").Trim();
        if (currentBranch != "" && !branchIds.Contains(currentBranch))
            branchIds.Add(currentBranch);

        foreach (taikhoan_table_2023 item in db.taikhoan_table_2023s.Where(p => p.user_parent == ownerKey).ToList())
        {
            PersonSourceRef source = AddSource(items, "staff", "Nhân sự nội bộ", item.taikhoan, item.hoten, item.dienthoai, item.email, "Nhân sự nội bộ",
                item.ngaytao,
                "/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + HttpUtility.UrlEncode(item.taikhoan));
            ApplyAdminAccessMetadata(db, ownerKey, source, item.taikhoan, (item.trangthai ?? "").Trim());
            ApplySourceLifecycleMetadata(db, ownerKey, source, (item.trangthai ?? "").Trim());
        }

        foreach (bspa_data_khachhang_table item in db.bspa_data_khachhang_tables.Where(p => p.user_parent == ownerKey).ToList())
        {
            PersonSourceRef source = AddSource(items, "customer", "Khách hàng", item.id + "", item.tenkhachhang, item.sdt, item.email, "Khách hàng",
                item.ngaytao,
                "/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=" + HttpUtility.UrlEncode(item.id + ""));
            ApplyAdminAccessMetadata(db, ownerKey, source, "", "");
            ApplySourceLifecycleMetadata(db, ownerKey, source, "");
        }

        if (branchIds.Count != 0)
        {
            foreach (giangvien_table item in db.giangvien_tables.Where(p => branchIds.Contains(p.id_chinhanh)).ToList())
            {
                PersonSourceRef source = AddSource(items, "lecturer", "Chuyên gia đào tạo", item.id + "", item.hoten, item.dienthoai, item.email, "Chuyên gia đào tạo",
                    item.ngaytao,
                    "/gianhang/admin/quan-ly-giang-vien/edit.aspx?id=" + HttpUtility.UrlEncode(item.id + ""));
                ApplyAdminAccessMetadata(db, ownerKey, source, "", "");
                ApplySourceLifecycleMetadata(db, ownerKey, source, (item.trangthai ?? "").Trim());
            }

            foreach (hocvien_table item in db.hocvien_tables.Where(p => branchIds.Contains(p.id_chinhanh)).ToList())
            {
                PersonSourceRef source = AddSource(items, "member", "Thành viên / Học viên", item.id + "", item.hoten, item.dienthoai, item.email, "Thành viên / Học viên",
                    item.ngaytao,
                    "/gianhang/admin/quan-ly-hoc-vien/edit.aspx?id=" + HttpUtility.UrlEncode(item.id + ""));
                ApplyAdminAccessMetadata(db, ownerKey, source, "", "");
                ApplySourceLifecycleMetadata(db, ownerKey, source, "");
            }
        }

        return items;
    }

    private static PersonSourceRef AddSource(ICollection<PersonSourceRef> items, string sourceType, string sourceLabel, string sourceKey, string name, string phone, string email, string roleLabel, DateTime? createdAt, string detailUrl)
    {
        if (items == null)
            return null;

        string normalizedPhone = NormalizePhone(phone);
        if (normalizedPhone == "")
            return null;

        PersonSourceRef source = new PersonSourceRef
        {
            SourceType = sourceType,
            SourceLabel = sourceLabel,
            SourceKey = (sourceKey ?? "").Trim(),
            Name = ((name ?? "").Trim() == "" ? normalizedPhone : (name ?? "").Trim()),
            Phone = (phone ?? "").Trim(),
            NormalizedPhone = normalizedPhone,
            Email = (email ?? "").Trim(),
            RoleLabel = roleLabel,
            CreatedAt = createdAt,
            DetailUrl = detailUrl
        };

        items.Add(source);
        return source;
    }

    private static void ApplyAdminAccessMetadata(dbDataContext db, string ownerAccountKey, PersonSourceRef source, string legacyUser, string legacyStatus)
    {
        if (source == null)
            return;

        string sourceType = Normalize(source.SourceType);
        if (sourceType != "staff")
        {
            source.IsAdminAccessEligible = false;
            source.AdminAccessStatus = "";
            source.AdminAccessLabel = "Không mở quyền /gianhang/admin ở nguồn này";
            source.AdminAccessCss = "bg-gray fg-white";
            return;
        }

        source.IsAdminAccessEligible = true;

        string normalizedLegacyStatus = Normalize(legacyStatus);
        if (normalizedLegacyStatus != "" && normalizedLegacyStatus != Normalize("Đang hoạt động"))
        {
            source.AdminAccessStatus = StatusRevoked;
            source.AdminAccessLabel = "Hồ sơ nội bộ đang bị khóa hoặc ngừng dùng nên không vào /gianhang/admin";
            source.AdminAccessCss = "bg-red fg-white";
            return;
        }

        GianHangAdminWorkspace_cl.MemberBindingInfo binding = GianHangAdminWorkspace_cl.GetMemberBindingInfo(db, ownerAccountKey, legacyUser);
        if (binding == null)
        {
            source.AdminAccessStatus = "";
            source.AdminAccessLabel = "Có vai trò nội bộ nhưng chưa mở quyền /gianhang/admin";
            source.AdminAccessCss = "bg-gray fg-white";
            return;
        }

        string status = Normalize(binding.Status);
        source.AdminAccessStatus = status;
        source.AdminAccessCss = ResolveStatusCss(status);

        if (status == StatusActive)
        {
            string roleLabel = string.IsNullOrWhiteSpace(binding.RoleLabel) ? "nhân sự nội bộ" : binding.RoleLabel;
            source.AdminAccessLabel = "Đã mở quyền /gianhang/admin với vai trò " + roleLabel;
            return;
        }

        if (status == StatusPending)
        {
            source.AdminAccessLabel = "Đang chờ kích hoạt quyền /gianhang/admin";
            return;
        }

        source.AdminAccessLabel = "Có vai trò nội bộ nhưng chưa mở quyền /gianhang/admin";
        source.AdminAccessCss = "bg-gray fg-white";
    }

    private static void ApplySourceLifecycleMetadata(dbDataContext db, string ownerAccountKey, PersonSourceRef source, string nativeStatus)
    {
        if (source == null)
            return;

        string sourceType = Normalize(source.SourceType);
        if (sourceType == "staff")
        {
            bool inactive = nativeStatus != "" && Normalize(nativeStatus) != Normalize("Đang hoạt động");
            source.SourceLifecycleStatus = inactive ? StatusRevoked : StatusActive;
            source.SourceLifecycleLabel = inactive ? "Đã khóa ở nguồn" : "Đang dùng ở nguồn";
            source.SourceLifecycleCss = inactive ? "bg-orange fg-white" : "bg-green fg-white";
            source.SourceLifecycleNote = inactive
                ? "Hồ sơ nhân sự nội bộ này đang bị khóa hoặc ngừng dùng ở module nguồn."
                : "Hồ sơ nhân sự nội bộ này đang được dùng tại module nguồn.";
            source.IsSourceInactive = inactive;
            return;
        }

        if (sourceType == "lecturer")
        {
            bool inactive = nativeStatus != "" && Normalize(nativeStatus) != Normalize("Đang giảng dạy");
            source.SourceLifecycleStatus = inactive ? StatusRevoked : StatusActive;
            source.SourceLifecycleLabel = inactive ? "Đã ngừng dùng ở nguồn" : "Đang dùng ở nguồn";
            source.SourceLifecycleCss = inactive ? "bg-orange fg-white" : "bg-green fg-white";
            source.SourceLifecycleNote = inactive
                ? "Vai trò chuyên gia đào tạo này đang ở trạng thái ngừng dùng tại module nguồn."
                : "Vai trò chuyên gia đào tạo này đang được dùng tại module nguồn.";
            source.IsSourceInactive = inactive;
            return;
        }

        if (sourceType == "customer")
        {
            GianHangAdminSourceLifecycle_cl.SourceLifecycleInfo info = GianHangAdminSourceLifecycle_cl.GetInfo(
                db,
                ownerAccountKey,
                sourceType,
                source.SourceKey,
                "Đang dùng ở nguồn",
                "Đã ngừng dùng ở nguồn",
                "Hồ sơ khách hàng này đang được dùng tại module nguồn.",
                "Hồ sơ khách hàng này đang ở trạng thái ngừng dùng an toàn tại module nguồn.");
            ApplySourceLifecycleInfo(source, info);
            return;
        }

        if (sourceType == "member")
        {
            GianHangAdminSourceLifecycle_cl.SourceLifecycleInfo info = GianHangAdminSourceLifecycle_cl.GetInfo(
                db,
                ownerAccountKey,
                sourceType,
                source.SourceKey,
                "Đang dùng ở nguồn",
                "Đã ngừng dùng ở nguồn",
                "Vai trò thành viên / học viên này đang được dùng tại module nguồn.",
                "Vai trò thành viên / học viên này đang ở trạng thái ngừng dùng an toàn tại module nguồn.");
            ApplySourceLifecycleInfo(source, info);
            return;
        }

        source.SourceLifecycleStatus = StatusActive;
        source.SourceLifecycleLabel = "Đang dùng ở nguồn";
        source.SourceLifecycleCss = "bg-green fg-white";
        source.SourceLifecycleNote = "Vai trò này đang được dùng tại module nguồn.";
        source.IsSourceInactive = false;
    }

    private static void ApplySourceLifecycleInfo(PersonSourceRef source, GianHangAdminSourceLifecycle_cl.SourceLifecycleInfo info)
    {
        if (source == null)
            return;

        source.SourceLifecycleStatus = info == null ? StatusActive : (info.Status ?? StatusActive);
        source.SourceLifecycleLabel = info == null || string.IsNullOrWhiteSpace(info.Label) ? "Đang dùng ở nguồn" : info.Label;
        source.SourceLifecycleCss = info == null || string.IsNullOrWhiteSpace(info.Css) ? "bg-green fg-white" : info.Css;
        source.SourceLifecycleNote = info == null ? "Vai trò này đang được dùng tại module nguồn." : (info.Note ?? "");
        source.IsSourceInactive = info != null && info.IsInactive;
    }

    private static bool MatchFilter(IEnumerable<PersonSourceRef> groupItems, PersonLinkInfo linkInfo, string primaryName, string displayPhone, string keyword, string normalizedKeywordPhone, string statusFilter, string sourceFilter)
    {
        string status = Normalize(linkInfo == null ? "" : linkInfo.Status);
        if (statusFilter == StatusActive && status != StatusActive)
            return false;
        if (statusFilter == StatusPending && status != StatusPending)
            return false;
        if (statusFilter == "none" && status != "")
            return false;

        if (sourceFilter != "all" && !groupItems.Any(p => string.Equals(Normalize(p.SourceType), sourceFilter, StringComparison.OrdinalIgnoreCase)))
            return false;

        if (string.IsNullOrWhiteSpace(keyword))
            return true;

        if ((primaryName ?? "").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
            return true;

        if ((displayPhone ?? "").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
            return true;

        if (normalizedKeywordPhone != "" && string.Equals(NormalizePhone(displayPhone), normalizedKeywordPhone, StringComparison.OrdinalIgnoreCase))
            return true;

        foreach (PersonSourceRef item in groupItems)
        {
            if ((item.Name ?? "").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if ((item.Email ?? "").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if ((item.RoleLabel ?? "").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if ((item.Phone ?? "").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if (normalizedKeywordPhone != "" && string.Equals(item.NormalizedPhone, normalizedKeywordPhone, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        if (linkInfo != null && linkInfo.LinkedHomeAccount != null)
        {
            taikhoan_tb linked = linkInfo.LinkedHomeAccount;
            if (((linked.taikhoan ?? "") + "").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if (((linked.hoten ?? "") + "").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if (((linked.dienthoai ?? "") + "").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
        }

        if (linkInfo != null)
        {
            if ((linkInfo.PrimaryName ?? "").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if ((linkInfo.PendingPhone ?? "").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
        }

        return false;
    }

    private static void SyncLegacyMembershipsForPhone(dbDataContext db, string ownerAccountKey, string normalizedPhone, string homeAccountKey, string pendingPhone, string status, string createdBy)
    {
        if (db == null)
            return;

        string ownerKey = Normalize(ownerAccountKey);
        string phone = NormalizePhone(normalizedPhone);
        string memberAccountKey = Normalize(homeAccountKey);
        string pending = NormalizePhone(pendingPhone);
        string createdByValue = (createdBy ?? "").Trim();
        DateTime now = AhaTime_cl.Now;

        List<taikhoan_table_2023> staffRows = db.taikhoan_table_2023s
            .Where(p => p.user_parent == ownerKey)
            .ToList()
            .Where(p => string.Equals(NormalizePhone(p.dienthoai), phone, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (taikhoan_table_2023 legacy in staffRows)
        {
            string legacyUser = Normalize(legacy.taikhoan);
            if (legacyUser == "")
                continue;

            if (status == StatusRevoked)
            {
                db.ExecuteCommand(
                    "UPDATE dbo.CoreGianHangAdminMember_tb SET MembershipStatus = {2}, MemberAccountKey = NULL, PendingPhone = NULL, InviteLookupKey = NULL, UpdatedAt = {3} WHERE OwnerAccountKey = {0} AND LegacyUser = {1}",
                    ownerKey,
                    legacyUser,
                    StatusRevoked,
                    now);
                continue;
            }

            if (memberAccountKey != "" && string.Equals(memberAccountKey, ownerKey, StringComparison.OrdinalIgnoreCase))
                continue;

            string roleLabel = "Nhân sự nội bộ";
            string memberType = "staff";

            int affected = db.ExecuteCommand(
                "UPDATE dbo.CoreGianHangAdminMember_tb " +
                "SET MemberAccountKey = {2}, MembershipStatus = {3}, RoleLabel = {4}, MemberType = {5}, PendingPhone = {6}, InviteLookupKey = {7}, CreatedBy = {8}, UpdatedAt = {9}, AcceptedAt = CASE WHEN {3} = {10} THEN ISNULL(AcceptedAt, {9}) ELSE AcceptedAt END " +
                "WHERE OwnerAccountKey = {0} AND LegacyUser = {1}",
                ownerKey,
                legacyUser,
                memberAccountKey == "" ? null : memberAccountKey,
                status,
                roleLabel,
                memberType,
                pending == "" ? null : pending,
                pending == "" ? null : pending,
                createdByValue,
                now,
                StatusActive);

            if (affected == 0)
            {
                db.ExecuteCommand(
                    "INSERT INTO dbo.CoreGianHangAdminMember_tb " +
                    "(OwnerAccountKey, MemberAccountKey, LegacyUser, MembershipStatus, RoleLabel, MemberType, PendingPhone, InviteLookupKey, CreatedBy, AcceptedAt, CreatedAt, UpdatedAt) " +
                    "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {9}, {9})",
                    ownerKey,
                    memberAccountKey == "" ? null : memberAccountKey,
                    legacyUser,
                    status,
                    roleLabel,
                    memberType,
                    pending == "" ? null : pending,
                    pending == "" ? null : pending,
                    createdByValue,
                    now);
            }
        }
    }

    private static void UpsertPersonLinkRow(dbDataContext db, string ownerAccountKey, string normalizedPhone, string primaryName, string homeAccountKey, string status, string pendingPhone, string inviteLookupKey, string createdBy, bool overwrite)
    {
        string ownerKey = Normalize(ownerAccountKey);
        string phone = NormalizePhone(normalizedPhone);
        if (db == null || ownerKey == "" || phone == "")
            return;

        string normalizedStatus = Normalize(status);
        if (normalizedStatus != StatusPending && normalizedStatus != StatusRevoked)
            normalizedStatus = StatusActive;

        string pending = NormalizePhone(pendingPhone);
        string homeKey = Normalize(homeAccountKey);
        string name = (primaryName ?? "").Trim();
        string createdByValue = (createdBy ?? "").Trim();
        DateTime now = AhaTime_cl.Now;

        PersonLinkRaw existing = GetPersonLinkRow(db, ownerKey, phone);
        if (existing == null)
        {
            db.ExecuteCommand(
                "INSERT INTO dbo.CoreGianHangPersonLink_tb " +
                "(OwnerAccountKey, NormalizedPhone, PrimaryName, HomeAccountKey, LinkStatus, PendingPhone, InviteLookupKey, CreatedBy, AcceptedAt, CreatedAt, UpdatedAt) " +
                "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {8}, {8})",
                ownerKey,
                phone,
                name,
                homeKey == "" ? null : homeKey,
                normalizedStatus,
                pending == "" ? null : pending,
                string.IsNullOrWhiteSpace(inviteLookupKey) ? null : inviteLookupKey.Trim(),
                createdByValue,
                now);
            return;
        }

        bool shouldApply = overwrite || GetStatusPriority(normalizedStatus) >= GetStatusPriority(existing.LinkStatus);
        if (!shouldApply)
            return;

        db.ExecuteCommand(
            "UPDATE dbo.CoreGianHangPersonLink_tb " +
            "SET PrimaryName = CASE WHEN {2} IS NULL OR {2} = '' THEN PrimaryName ELSE {2} END, HomeAccountKey = {3}, LinkStatus = {4}, PendingPhone = {5}, InviteLookupKey = {6}, CreatedBy = {7}, UpdatedAt = {8}, AcceptedAt = CASE WHEN {4} = {9} THEN ISNULL(AcceptedAt, {8}) ELSE AcceptedAt END " +
            "WHERE OwnerAccountKey = {0} AND NormalizedPhone = {1}",
            ownerKey,
            phone,
            name,
            homeKey == "" ? null : homeKey,
            normalizedStatus,
            pending == "" ? null : pending,
            string.IsNullOrWhiteSpace(inviteLookupKey) ? null : inviteLookupKey.Trim(),
            createdByValue,
            now,
            StatusActive);
    }

    private static PersonLinkRaw GetPersonLinkRow(dbDataContext db, string ownerAccountKey, string normalizedPhone)
    {
        if (db == null)
            return null;

        string ownerKey = Normalize(ownerAccountKey);
        string phone = NormalizePhone(normalizedPhone);
        if (ownerKey == "" || phone == "")
            return null;

        return db.ExecuteQuery<PersonLinkRaw>(
            "SELECT TOP 1 Id, OwnerAccountKey, NormalizedPhone, PrimaryName, HomeAccountKey, LinkStatus, PendingPhone, InviteLookupKey, CreatedBy, AcceptedAt, CreatedAt, UpdatedAt " +
            "FROM dbo.CoreGianHangPersonLink_tb WHERE OwnerAccountKey = {0} AND NormalizedPhone = {1} ORDER BY UpdatedAt DESC, Id DESC",
            ownerKey,
            phone).FirstOrDefault();
    }

    private static IList<PersonLinkRaw> GetPersonLinkRows(dbDataContext db, string ownerAccountKey)
    {
        if (db == null)
            return new List<PersonLinkRaw>();

        string ownerKey = Normalize(ownerAccountKey);
        if (ownerKey == "")
            return new List<PersonLinkRaw>();

        return db.ExecuteQuery<PersonLinkRaw>(
            "SELECT Id, OwnerAccountKey, NormalizedPhone, PrimaryName, HomeAccountKey, LinkStatus, PendingPhone, InviteLookupKey, CreatedBy, AcceptedAt, CreatedAt, UpdatedAt " +
            "FROM dbo.CoreGianHangPersonLink_tb WHERE OwnerAccountKey = {0} ORDER BY UpdatedAt DESC, Id DESC",
            ownerKey).ToList();
    }

    private static IList<PersonSourceEventRaw> GetRemovedSourceEvents(dbDataContext db, string ownerAccountKey)
    {
        if (db == null)
            return new List<PersonSourceEventRaw>();

        string ownerKey = Normalize(ownerAccountKey);
        if (ownerKey == "")
            return new List<PersonSourceEventRaw>();

        return db.ExecuteQuery<PersonSourceEventRaw>(
            "SELECT Id, OwnerAccountKey, NormalizedPhone, SourceType, SourceLabel, SourceKey, RoleLabel, DisplayName, EventType, EventNote, CreatedBy, CreatedAt " +
            "FROM dbo.CoreGianHangPersonSourceEvent_tb WHERE OwnerAccountKey = {0} ORDER BY CreatedAt DESC, Id DESC",
            ownerKey).ToList();
    }

    private static IList<PersonRemovedSourceRef> GetRemovedSources(dbDataContext db, string ownerAccountKey, string normalizedPhone)
    {
        string ownerKey = Normalize(ownerAccountKey);
        string phone = NormalizePhone(normalizedPhone);
        if (db == null || ownerKey == "" || phone == "")
            return new List<PersonRemovedSourceRef>();

        return db.ExecuteQuery<PersonSourceEventRaw>(
            "SELECT Id, OwnerAccountKey, NormalizedPhone, SourceType, SourceLabel, SourceKey, RoleLabel, DisplayName, EventType, EventNote, CreatedBy, CreatedAt " +
            "FROM dbo.CoreGianHangPersonSourceEvent_tb WHERE OwnerAccountKey = {0} AND NormalizedPhone = {1} ORDER BY CreatedAt DESC, Id DESC",
            ownerKey,
            phone)
            .Select(p => new PersonRemovedSourceRef
            {
                SourceType = (p.SourceType ?? "").Trim(),
                SourceLabel = (p.SourceLabel ?? "").Trim(),
                SourceKey = (p.SourceKey ?? "").Trim(),
                RoleLabel = (p.RoleLabel ?? "").Trim(),
                DisplayName = (p.DisplayName ?? "").Trim(),
                EventType = (p.EventType ?? "").Trim(),
                EventNote = (p.EventNote ?? "").Trim(),
                CreatedBy = (p.CreatedBy ?? "").Trim(),
                CreatedAt = p.CreatedAt
            })
            .ToList();
    }

    private static void AppendRemovedSourceEvent(dbDataContext db, string ownerAccountKey, string normalizedPhone, string sourceType, string sourceLabel, string sourceKey, string roleLabel, string displayName, string createdBy, string eventNote)
    {
        string ownerKey = Normalize(ownerAccountKey);
        string phone = NormalizePhone(normalizedPhone);
        if (db == null || ownerKey == "" || phone == "")
            return;

        string sourceTypeValue = Normalize(sourceType);
        string sourceKeyValue = (sourceKey ?? "").Trim();
        string eventTypeValue = "source_removed";
        DateTime now = AhaTime_cl.Now;

        PersonSourceEventRaw latest = db.ExecuteQuery<PersonSourceEventRaw>(
            "SELECT TOP 1 Id, OwnerAccountKey, NormalizedPhone, SourceType, SourceLabel, SourceKey, RoleLabel, DisplayName, EventType, EventNote, CreatedBy, CreatedAt " +
            "FROM dbo.CoreGianHangPersonSourceEvent_tb WHERE OwnerAccountKey = {0} AND NormalizedPhone = {1} AND ISNULL(SourceType, '') = {2} AND ISNULL(SourceKey, '') = {3} " +
            "ORDER BY CreatedAt DESC, Id DESC",
            ownerKey,
            phone,
            sourceTypeValue,
            sourceKeyValue).FirstOrDefault();

        if (latest != null && latest.CreatedAt.HasValue && (now - latest.CreatedAt.Value).TotalSeconds < 5)
            return;

        db.ExecuteCommand(
            "INSERT INTO dbo.CoreGianHangPersonSourceEvent_tb " +
            "(OwnerAccountKey, NormalizedPhone, SourceType, SourceLabel, SourceKey, RoleLabel, DisplayName, EventType, EventNote, CreatedBy, CreatedAt) " +
            "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10})",
            ownerKey,
            phone,
            sourceTypeValue == "" ? null : sourceTypeValue,
            string.IsNullOrWhiteSpace(sourceLabel) ? null : sourceLabel.Trim(),
            sourceKeyValue == "" ? null : sourceKeyValue,
            string.IsNullOrWhiteSpace(roleLabel) ? null : roleLabel.Trim(),
            string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim(),
            eventTypeValue,
            string.IsNullOrWhiteSpace(eventNote) ? null : eventNote.Trim(),
            string.IsNullOrWhiteSpace(createdBy) ? null : createdBy.Trim(),
            now);
    }

    private static PersonLinkInfo BuildLinkInfo(dbDataContext db, PersonLinkRaw raw, string defaultName)
    {
        if (raw == null)
        {
            return new PersonLinkInfo
            {
                Status = "",
                StatusLabel = "Chưa liên kết",
                LinkCss = "bg-gray fg-white",
                PendingPhone = "",
                PrimaryName = defaultName ?? ""
            };
        }

        taikhoan_tb linked = null;
        if (!string.IsNullOrWhiteSpace(raw.HomeAccountKey))
            linked = RootAccount_cl.GetByAccountKey(db, raw.HomeAccountKey);

        string pendingPhone = (raw.PendingPhone ?? "").Trim();
        if (pendingPhone == "")
            pendingPhone = NormalizePhone(raw.NormalizedPhone);

        return new PersonLinkInfo
        {
            Status = Normalize(raw.LinkStatus),
            StatusLabel = ResolveStatusLabel(raw.LinkStatus),
            LinkCss = ResolveStatusCss(raw.LinkStatus),
            PendingPhone = pendingPhone,
            PrimaryName = string.IsNullOrWhiteSpace(raw.PrimaryName) ? (defaultName ?? "") : raw.PrimaryName.Trim(),
            LinkedHomeAccount = linked
        };
    }

    private static taikhoan_tb ResolveHomeAccount(dbDataContext db, string homeAccountOrPhone)
    {
        if (db == null)
            return null;

        string raw = (homeAccountOrPhone ?? "").Trim();
        string accountKey = Normalize(raw);
        if (accountKey != "")
        {
            taikhoan_tb byAccount = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == accountKey);
            if (byAccount != null)
                return byAccount;
        }

        string normalizedPhone = NormalizePhone(raw);
        if (normalizedPhone == "")
            return null;

        return db.taikhoan_tbs
            .ToList()
            .FirstOrDefault(p => string.Equals(NormalizePhone(p.dienthoai), normalizedPhone, StringComparison.OrdinalIgnoreCase));
    }

    private static string ResolvePrimaryName(IList<PersonSourceRef> sources)
    {
        if (sources == null || sources.Count == 0)
            return "";

        PersonSourceRef preferred = sources
            .OrderBy(p => GetSourcePriority(p.SourceType))
            .ThenBy(p => p.Name ?? "")
            .FirstOrDefault();

        return preferred == null ? "" : (preferred.Name ?? "").Trim();
    }

    private static string ResolveDisplayPhone(IList<PersonSourceRef> sources, string normalizedPhone)
    {
        if (sources != null)
        {
            string phone = sources.Select(p => (p.Phone ?? "").Trim()).FirstOrDefault(p => p != "");
            if (!string.IsNullOrWhiteSpace(phone))
                return phone;
        }

        return normalizedPhone ?? "";
    }

    private static string NormalizeStatusFilter(string raw)
    {
        string value = Normalize(raw);
        if (value == StatusActive)
            return StatusActive;
        if (value == StatusPending)
            return StatusPending;
        if (value == "none")
            return "none";
        return "all";
    }

    private static string NormalizeSourceFilter(string raw)
    {
        string value = Normalize(raw);
        if (value == "staff" || value == "customer" || value == "lecturer" || value == "member")
            return value;
        return "all";
    }

    private static int GetSourcePriority(string sourceType)
    {
        string value = Normalize(sourceType);
        if (value == "staff")
            return 0;
        if (value == "lecturer")
            return 1;
        if (value == "member")
            return 2;
        if (value == "customer")
            return 3;
        return 10;
    }

    private static int GetStatusPriority(string status)
    {
        string value = Normalize(status);
        if (value == StatusActive)
            return 3;
        if (value == StatusPending)
            return 2;
        if (value == StatusRevoked)
            return 1;
        return 0;
    }

    private static string ResolveStatusLabel(string status)
    {
        string value = Normalize(status);
        if (value == StatusActive)
            return "Đã liên kết";
        if (value == StatusPending)
            return "Chờ liên kết";
        if (value == StatusRevoked)
            return "Đã gỡ liên kết";
        return "Chưa liên kết";
    }

    private static string ResolveStatusCss(string status)
    {
        string value = Normalize(status);
        if (value == StatusActive)
            return "bg-green fg-white";
        if (value == StatusPending)
            return "bg-orange fg-white";
        if (value == StatusRevoked)
            return "bg-red fg-white";
        return "bg-gray fg-white";
    }

    private static string ResolveAdminAccessSummary(IList<PersonSourceRef> eligibleSources, bool hasGrantedAdminAccess, bool hasPendingAdminAccess)
    {
        if (eligibleSources == null || eligibleSources.Count == 0)
            return "Không có vai trò nào mở quyền vào /gianhang/admin";

        if (hasGrantedAdminAccess)
            return "Đã có ít nhất 1 vai trò nội bộ được mở quyền vào /gianhang/admin";

        if (hasPendingAdminAccess)
            return "Đang có vai trò nội bộ chờ kích hoạt quyền vào /gianhang/admin";

        return "Có vai trò nội bộ nhưng chưa mở quyền vào /gianhang/admin";
    }

    private static string NormalizePhone(string raw)
    {
        return (AccountAuth_cl.NormalizePhone(raw) ?? "").Trim();
    }

    private static string Normalize(string raw)
    {
        return (raw ?? "").Trim().ToLowerInvariant();
    }
}
