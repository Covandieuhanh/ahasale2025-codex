using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class GianHangAdminWorkspace_cl
{
    private const string StatusActive = "active";
    private const string StatusPending = "pending";
    private const string StatusRevoked = "revoked";
    private const string MemberTypeOwner = "owner";
    private const string MemberTypeStaff = "staff";
    private const string MemberTypeExpert = "expert";
    private const string SessionSelectedOwnerKey = "gianhang_admin_space_owner";
    private const string SessionSelectedLegacyUserKey = "gianhang_admin_space_legacy_user";
    private const string SessionWorkspaceModeKey = "gianhang_admin_space_mode";

    public sealed class WorkspaceInfo
    {
        public string OwnerAccountKey { get; set; }
        public string OwnerDisplayName { get; set; }
        public string OwnerPhone { get; set; }
        public string LegacyUser { get; set; }
        public string LegacyDisplayName { get; set; }
        public string RoleLabel { get; set; }
        public string MemberType { get; set; }
        public bool IsOwner { get; set; }
        public string ChiNhanhId { get; set; }
        public string NganhId { get; set; }
    }

    public sealed class MemberBindingInfo
    {
        public string Status { get; set; }
        public string StatusLabel { get; set; }
        public string RoleLabel { get; set; }
        public string MemberType { get; set; }
        public string PendingPhone { get; set; }
        public taikhoan_tb LinkedHomeAccount { get; set; }
    }

    private sealed class WorkspaceMemberRaw
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
        public string CreatedBy { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public static void ClearWorkspaceSelection()
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null)
            return;

        ctx.Session[SessionSelectedOwnerKey] = "";
        ctx.Session[SessionSelectedLegacyUserKey] = "";
        ctx.Session[SessionWorkspaceModeKey] = "";
    }

    public static IList<WorkspaceInfo> GetAvailableWorkspaces(dbDataContext db, string homeAccountKey)
    {
        List<WorkspaceInfo> items = new List<WorkspaceInfo>();
        if (db == null)
            return items;

        string accountKey = Normalize(homeAccountKey);
        if (accountKey == "")
            return items;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        GianHangAdminPersonHub_cl.PromotePendingLinks(db, accountKey);
        PromotePendingMemberships(db, accountKey);

        WorkspaceInfo ownWorkspace = BuildOwnedWorkspace(db, accountKey);
        if (ownWorkspace != null)
            items.Add(ownWorkspace);

        foreach (WorkspaceInfo joined in BuildJoinedWorkspaces(db, accountKey))
        {
            if (joined == null)
                continue;

            bool exists = items.Any(p =>
                string.Equals(p.OwnerAccountKey, joined.OwnerAccountKey, StringComparison.OrdinalIgnoreCase)
                && string.Equals(p.LegacyUser, joined.LegacyUser, StringComparison.OrdinalIgnoreCase));
            if (!exists)
                items.Add(joined);
        }

        return items;
    }

    public static bool HasAnyWorkspace(dbDataContext db, string homeAccountKey)
    {
        return GetAvailableWorkspaces(db, homeAccountKey).Count > 0;
    }

    public static WorkspaceInfo ResolveSelectedWorkspace(dbDataContext db, string homeAccountKey)
    {
        IList<WorkspaceInfo> items = GetAvailableWorkspaces(db, homeAccountKey);
        if (items.Count == 0)
        {
            ClearWorkspaceSelection();
            return null;
        }

        string explicitOwner = Normalize(ReadQueryString("space_owner"));
        if (explicitOwner != "")
        {
            WorkspaceInfo explicitWorkspace = items.FirstOrDefault(p =>
                string.Equals(p.OwnerAccountKey, explicitOwner, StringComparison.OrdinalIgnoreCase));
            if (explicitWorkspace != null)
            {
                SaveWorkspaceSelection(explicitWorkspace);
                return explicitWorkspace;
            }

            ClearWorkspaceSelection();
            return null;
        }

        if (ShouldPromptWorkspaceSelection(items))
        {
            ClearWorkspaceSelection();
            return null;
        }

        WorkspaceInfo selectedFromSession = ResolveFromSession(items);
        if (selectedFromSession != null)
            return selectedFromSession;

        if (items.Count == 1)
        {
            SaveWorkspaceSelection(items[0]);
            return items[0];
        }

        return null;
    }

    public static bool TryBootstrapSelectedWorkspace(
        dbDataContext db,
        string homeAccountKey,
        out WorkspaceInfo workspace,
        out string deniedMessage)
    {
        workspace = null;
        deniedMessage = "";

        string accountKey = Normalize(homeAccountKey);
        if (db == null || accountKey == "")
            return false;

        workspace = ResolveSelectedWorkspace(db, accountKey);
        if (workspace == null)
        {
            deniedMessage = "Bạn chưa chọn không gian gian hàng cần truy cập.";
            ClearLegacyAdminSessionOnly();
            return false;
        }

        try
        {
            GianHangWorkspaceLink_cl.EnsureWorkspaceLinked(
                db,
                workspace.OwnerAccountKey,
                workspace.ChiNhanhId,
                workspace.NganhId);
        }
        catch
        {
        }

        if (!ApplyLegacySession(workspace))
        {
            deniedMessage = "Không thể khởi tạo phiên quản trị cho không gian đã chọn.";
            ClearLegacyAdminSessionOnly();
            return false;
        }

        return true;
    }

    public static bool TryAssignMember(
        dbDataContext db,
        string ownerAccountKey,
        string legacyUser,
        string homeAccountOrPhone,
        string roleLabel,
        string memberType,
        string createdBy,
        out string message)
    {
        message = "";
        if (db == null)
        {
            message = "Không có kết nối dữ liệu.";
            return false;
        }

        string ownerKey = Normalize(ownerAccountKey);
        string legacyAccount = Normalize(legacyUser);
        string rawHome = (homeAccountOrPhone ?? "").Trim();
        if (ownerKey == "" || legacyAccount == "" || rawHome == "")
        {
            message = "Cần nhập tài khoản Home hoặc số điện thoại để liên kết.";
            return false;
        }

        taikhoan_tb owner = RootAccount_cl.GetByAccountKey(db, ownerKey);
        if (owner == null || !SpaceAccess_cl.CanAccessGianHangAdmin(db, owner))
        {
            message = "Chủ không gian hiện chưa được mở quyền /gianhang/admin.";
            return false;
        }

        taikhoan_table_2023 legacy = db.taikhoan_table_2023s.FirstOrDefault(p => p.taikhoan == legacyAccount && p.user_parent == ownerKey);
        if (legacy == null)
        {
            message = "Không tìm thấy hồ sơ nhân sự cần liên kết trong không gian hiện tại.";
            return false;
        }

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        DateTime now = AhaTime_cl.Now;
        string type = NormalizeMemberType(memberType, false);
        string role = NormalizeRoleLabel(roleLabel, false, type);
        string createdByValue = (createdBy ?? "").Trim();
        taikhoan_tb member = ResolveHomeAccount(db, rawHome);

        if (member != null)
        {
            if (!PortalScope_cl.CanLoginHome(member.taikhoan, member.phanloai, member.permission))
            {
                message = "Tài khoản nhập vào hiện không dùng được như một tài khoản Home.";
                return false;
            }

            string memberKey = Normalize(member.taikhoan);
            if (memberKey == ownerKey)
            {
                message = "Không cần liên kết tài khoản chủ vào chính không gian của mình.";
                return false;
            }

            int affected = db.ExecuteCommand(
                "UPDATE dbo.CoreGianHangAdminMember_tb " +
                "SET MemberAccountKey = {2}, LegacyUser = {3}, MembershipStatus = {4}, RoleLabel = {5}, MemberType = {6}, PendingPhone = NULL, InviteLookupKey = NULL, CreatedBy = {7}, UpdatedAt = {8}, AcceptedAt = ISNULL(AcceptedAt, {8}) " +
                "WHERE OwnerAccountKey = {0} AND MemberAccountKey = {1}",
                ownerKey,
                memberKey,
                memberKey,
                legacyAccount,
                StatusActive,
                role,
                type,
                createdByValue,
                now);

            if (affected == 0)
            {
                affected = db.ExecuteCommand(
                    "UPDATE dbo.CoreGianHangAdminMember_tb " +
                    "SET MemberAccountKey = {2}, PendingPhone = NULL, InviteLookupKey = NULL, MembershipStatus = {3}, RoleLabel = {4}, MemberType = {5}, CreatedBy = {6}, UpdatedAt = {7}, AcceptedAt = ISNULL(AcceptedAt, {7}) " +
                    "WHERE OwnerAccountKey = {0} AND LegacyUser = {1}",
                    ownerKey,
                    legacyAccount,
                    memberKey,
                    StatusActive,
                    role,
                    type,
                    createdByValue,
                    now);
            }

            if (affected == 0)
            {
                db.ExecuteCommand(
                    "INSERT INTO dbo.CoreGianHangAdminMember_tb " +
                    "(OwnerAccountKey, MemberAccountKey, LegacyUser, MembershipStatus, RoleLabel, MemberType, PendingPhone, InviteLookupKey, CreatedBy, AcceptedAt, CreatedAt, UpdatedAt) " +
                    "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, NULL, NULL, {6}, {7}, {7}, {7})",
                    ownerKey,
                    memberKey,
                    legacyAccount,
                    StatusActive,
                    role,
                    type,
                    createdByValue,
                    now);
            }

            message = "Đã liên kết tài khoản Home vào không gian quản trị gian hàng.";
            return true;
        }

        string pendingPhone = NormalizePhone(rawHome);
        if (pendingPhone == "")
        {
            message = "Nếu người này chưa có tài khoản AhaSale, hãy nhập số điện thoại để tạo lời mời chờ liên kết.";
            return false;
        }

        int pendingAffected = db.ExecuteCommand(
            "UPDATE dbo.CoreGianHangAdminMember_tb " +
            "SET MemberAccountKey = NULL, PendingPhone = {2}, InviteLookupKey = {3}, MembershipStatus = {4}, RoleLabel = {5}, MemberType = {6}, CreatedBy = {7}, UpdatedAt = {8}, AcceptedAt = NULL " +
            "WHERE OwnerAccountKey = {0} AND LegacyUser = {1}",
            ownerKey,
            legacyAccount,
            pendingPhone,
            Normalize(rawHome),
            StatusPending,
            role,
            type,
            createdByValue,
            now);

        if (pendingAffected == 0)
        {
            db.ExecuteCommand(
                "INSERT INTO dbo.CoreGianHangAdminMember_tb " +
                "(OwnerAccountKey, MemberAccountKey, LegacyUser, MembershipStatus, RoleLabel, MemberType, PendingPhone, InviteLookupKey, CreatedBy, AcceptedAt, CreatedAt, UpdatedAt) " +
                "VALUES ({0}, NULL, {1}, {2}, {3}, {4}, {5}, {6}, {7}, NULL, {8}, {8})",
                ownerKey,
                legacyAccount,
                StatusPending,
                role,
                type,
                pendingPhone,
                Normalize(rawHome),
                createdByValue,
                now);
        }

        message = "Đã tạo lời mời chờ liên kết. Khi số điện thoại này đăng ký hoặc đăng nhập AhaSale, hệ thống sẽ tự thêm vào không gian này theo vai trò đã chọn.";
        return true;
    }

    public static bool TryRemoveMember(
        dbDataContext db,
        string ownerAccountKey,
        string legacyUser,
        out string message)
    {
        message = "";
        if (db == null)
        {
            message = "Không có kết nối dữ liệu.";
            return false;
        }

        string ownerKey = Normalize(ownerAccountKey);
        string legacyAccount = Normalize(legacyUser);
        if (ownerKey == "" || legacyAccount == "")
        {
            message = "Thiếu dữ liệu để gỡ liên kết.";
            return false;
        }

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);
        int affected = db.ExecuteCommand(
            "UPDATE dbo.CoreGianHangAdminMember_tb SET MembershipStatus = {2}, UpdatedAt = {3} WHERE OwnerAccountKey = {0} AND LegacyUser = {1}",
            ownerKey,
            legacyAccount,
            StatusRevoked,
            AhaTime_cl.Now);

        if (affected == 0)
        {
            message = "Không tìm thấy liên kết Home cần gỡ.";
            return false;
        }

        message = "Đã gỡ liên kết tài khoản Home khỏi không gian này.";
        return true;
    }

    public static void SyncLegacySourceAccess(
        dbDataContext db,
        string ownerAccountKey,
        string legacyUser,
        bool canUseAdmin)
    {
        if (db == null)
            return;

        string ownerKey = Normalize(ownerAccountKey);
        string legacyAccount = Normalize(legacyUser);
        if (ownerKey == "" || legacyAccount == "")
            return;

        CoreSchemaMigration_cl.EnsureSchemaSafe(db);

        if (canUseAdmin)
        {
            taikhoan_table_2023 legacy = db.taikhoan_table_2023s.FirstOrDefault(p => p.taikhoan == legacyAccount && p.user_parent == ownerKey);
            if (legacy == null)
                return;

            if (!string.Equals((legacy.trangthai ?? "").Trim(), "Đang hoạt động", StringComparison.OrdinalIgnoreCase))
                return;

            WorkspaceMemberRaw existing = db.ExecuteQuery<WorkspaceMemberRaw>(
                "SELECT TOP 1 Id, OwnerAccountKey, MemberAccountKey, LegacyUser, MembershipStatus, RoleLabel, MemberType, PendingPhone, InviteLookupKey, CreatedBy, AcceptedAt, CreatedAt, UpdatedAt " +
                "FROM dbo.CoreGianHangAdminMember_tb WHERE OwnerAccountKey = {0} AND LegacyUser = {1} ORDER BY UpdatedAt DESC, Id DESC",
                ownerKey,
                legacyAccount).FirstOrDefault();

            string roleLabel = NormalizeRoleLabel(existing == null ? "" : existing.RoleLabel, false, existing == null ? MemberTypeStaff : existing.MemberType);
            string memberType = NormalizeMemberType(existing == null ? MemberTypeStaff : existing.MemberType, false);
            DateTime now = AhaTime_cl.Now;

            GianHangAdminPersonHub_cl.PersonLinkInfo linkInfo = GianHangAdminPersonHub_cl.GetLinkInfo(db, ownerKey, legacy.dienthoai, legacy.hoten);
            if (linkInfo == null)
                return;

            string linkStatus = Normalize(linkInfo.Status);
            if (linkStatus == StatusActive && linkInfo.LinkedHomeAccount != null)
            {
                string memberKey = Normalize(linkInfo.LinkedHomeAccount.taikhoan);
                if (memberKey == "" || memberKey == ownerKey)
                    return;

                int activated = db.ExecuteCommand(
                    "UPDATE dbo.CoreGianHangAdminMember_tb " +
                    "SET MemberAccountKey = {2}, MembershipStatus = {3}, RoleLabel = {4}, MemberType = {5}, PendingPhone = NULL, InviteLookupKey = NULL, UpdatedAt = {6}, AcceptedAt = ISNULL(AcceptedAt, {6}) " +
                    "WHERE OwnerAccountKey = {0} AND LegacyUser = {1}",
                    ownerKey,
                    legacyAccount,
                    memberKey,
                    StatusActive,
                    roleLabel,
                    memberType,
                    now);

                if (activated == 0)
                {
                    db.ExecuteCommand(
                        "INSERT INTO dbo.CoreGianHangAdminMember_tb " +
                        "(OwnerAccountKey, MemberAccountKey, LegacyUser, MembershipStatus, RoleLabel, MemberType, PendingPhone, InviteLookupKey, CreatedBy, AcceptedAt, CreatedAt, UpdatedAt) " +
                        "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, NULL, NULL, {6}, {7}, {7}, {7})",
                        ownerKey,
                        memberKey,
                        legacyAccount,
                        StatusActive,
                        roleLabel,
                        memberType,
                        "legacy-sync",
                        now);
                }

                return;
            }

            if (linkStatus == StatusPending && !string.IsNullOrWhiteSpace(linkInfo.PendingPhone))
            {
                string pendingPhone = NormalizePhone(linkInfo.PendingPhone);
                int rePending = db.ExecuteCommand(
                    "UPDATE dbo.CoreGianHangAdminMember_tb " +
                    "SET MemberAccountKey = NULL, PendingPhone = {2}, InviteLookupKey = {3}, MembershipStatus = {4}, RoleLabel = {5}, MemberType = {6}, UpdatedAt = {7}, AcceptedAt = NULL " +
                    "WHERE OwnerAccountKey = {0} AND LegacyUser = {1}",
                    ownerKey,
                    legacyAccount,
                    pendingPhone,
                    pendingPhone,
                    StatusPending,
                    roleLabel,
                    memberType,
                    now);

                if (rePending == 0)
                {
                    db.ExecuteCommand(
                        "INSERT INTO dbo.CoreGianHangAdminMember_tb " +
                        "(OwnerAccountKey, MemberAccountKey, LegacyUser, MembershipStatus, RoleLabel, MemberType, PendingPhone, InviteLookupKey, CreatedBy, AcceptedAt, CreatedAt, UpdatedAt) " +
                        "VALUES ({0}, NULL, {1}, {2}, {3}, {4}, {5}, {6}, {7}, NULL, {8}, {8})",
                        ownerKey,
                        legacyAccount,
                        StatusPending,
                        roleLabel,
                        memberType,
                        pendingPhone,
                        pendingPhone,
                        "legacy-sync",
                        now);
                }
            }
            return;
        }

        db.ExecuteCommand(
            "UPDATE dbo.CoreGianHangAdminMember_tb " +
            "SET MembershipStatus = {2}, UpdatedAt = {3} " +
            "WHERE OwnerAccountKey = {0} AND LegacyUser = {1} AND MembershipStatus IN ({4}, {5})",
            ownerKey,
            legacyAccount,
            StatusRevoked,
            AhaTime_cl.Now,
            StatusActive,
            StatusPending);
    }

    public static taikhoan_tb GetLinkedHomeAccount(dbDataContext db, string ownerAccountKey, string legacyUser)
    {
        MemberBindingInfo info = GetMemberBindingInfo(db, ownerAccountKey, legacyUser);
        return info != null ? info.LinkedHomeAccount : null;
    }

    public static MemberBindingInfo GetMemberBindingInfo(dbDataContext db, string ownerAccountKey, string legacyUser)
    {
        if (db == null)
            return null;

        string ownerKey = Normalize(ownerAccountKey);
        string legacyKey = Normalize(legacyUser);
        if (ownerKey == "" || legacyKey == "")
            return null;

        WorkspaceMemberRaw raw = db.ExecuteQuery<WorkspaceMemberRaw>(
                "SELECT TOP 1 Id, OwnerAccountKey, MemberAccountKey, LegacyUser, MembershipStatus, RoleLabel, MemberType, PendingPhone, InviteLookupKey, CreatedBy, AcceptedAt, CreatedAt, UpdatedAt " +
                "FROM dbo.CoreGianHangAdminMember_tb " +
                "WHERE OwnerAccountKey = {0} AND LegacyUser = {1} AND MembershipStatus IN ({2}, {3}) " +
                "ORDER BY CASE WHEN MembershipStatus = {2} THEN 0 ELSE 1 END, UpdatedAt DESC, Id DESC",
                ownerKey,
                legacyKey,
                StatusActive,
                StatusPending)
            .FirstOrDefault();

        if (raw == null)
            return null;

        taikhoan_tb linkedHome = null;
        if (!string.IsNullOrWhiteSpace(raw.MemberAccountKey))
            linkedHome = RootAccount_cl.GetByAccountKey(db, raw.MemberAccountKey);

        return new MemberBindingInfo
        {
            Status = (raw.MembershipStatus ?? "").Trim().ToLowerInvariant(),
            StatusLabel = ResolveStatusLabel(raw.MembershipStatus),
            RoleLabel = NormalizeRoleLabel(raw.RoleLabel, false, raw.MemberType),
            MemberType = NormalizeMemberType(raw.MemberType, false),
            PendingPhone = (raw.PendingPhone ?? "").Trim(),
            LinkedHomeAccount = linkedHome
        };
    }

    private static WorkspaceInfo BuildOwnedWorkspace(dbDataContext db, string homeAccountKey)
    {
        taikhoan_tb owner = RootAccount_cl.GetByAccountKey(db, homeAccountKey);
        if (owner == null || !SpaceAccess_cl.CanAccessGianHangAdmin(db, owner))
            return null;

        taikhoan_table_2023 adminAcc = AhaShineContext_cl.EnsureAdvancedAdminBootstrapForShop(db, homeAccountKey);
        if (adminAcc == null)
            return null;

        return BuildWorkspaceInfo(
            db,
            homeAccountKey,
            owner,
            adminAcc,
            true,
            "Chủ không gian",
            MemberTypeOwner);
    }

    private static IEnumerable<WorkspaceInfo> BuildJoinedWorkspaces(dbDataContext db, string homeAccountKey)
    {
        List<WorkspaceInfo> items = new List<WorkspaceInfo>();
        IEnumerable<WorkspaceMemberRaw> rows = db.ExecuteQuery<WorkspaceMemberRaw>(
            "SELECT Id, OwnerAccountKey, MemberAccountKey, LegacyUser, MembershipStatus, RoleLabel, MemberType, PendingPhone, InviteLookupKey, CreatedBy, AcceptedAt, CreatedAt, UpdatedAt " +
            "FROM dbo.CoreGianHangAdminMember_tb WHERE MemberAccountKey = {0} AND MembershipStatus = {1} ORDER BY UpdatedAt DESC, Id DESC",
            homeAccountKey,
            StatusActive);

        foreach (WorkspaceMemberRaw row in rows)
        {
            string ownerKey = Normalize(row.OwnerAccountKey);
            if (ownerKey == "" || string.Equals(ownerKey, homeAccountKey, StringComparison.OrdinalIgnoreCase))
                continue;

            taikhoan_tb owner = RootAccount_cl.GetByAccountKey(db, ownerKey);
            if (owner == null || !SpaceAccess_cl.CanAccessGianHangAdmin(db, owner))
                continue;

            string legacyUser = Normalize(row.LegacyUser);
            if (legacyUser == "")
                continue;

            taikhoan_table_2023 legacy = db.taikhoan_table_2023s.FirstOrDefault(p => p.taikhoan == legacyUser && p.user_parent == ownerKey);
            if (legacy == null)
                continue;

            string legacyStatus = (legacy.trangthai ?? "").Trim();
            if (legacyStatus != "" && !string.Equals(legacyStatus, "Đang hoạt động", StringComparison.OrdinalIgnoreCase))
                continue;

            WorkspaceInfo info = BuildWorkspaceInfo(db, ownerKey, owner, legacy, false, row.RoleLabel, row.MemberType);
            if (info != null)
                items.Add(info);
        }

        return items;
    }

    private static WorkspaceInfo BuildWorkspaceInfo(
        dbDataContext db,
        string ownerAccountKey,
        taikhoan_tb owner,
        taikhoan_table_2023 legacy,
        bool isOwner,
        string roleLabel,
        string memberType)
    {
        if (owner == null || legacy == null)
            return null;

        string ownerKey = Normalize(ownerAccountKey);
        string chiNhanhId = (legacy.id_chinhanh ?? "").Trim();
        if (chiNhanhId == "")
            chiNhanhId = (AhaShineContext_cl.ResolveChiNhanhIdForShopAccount(db, ownerKey) ?? "").Trim();

        string nganhId = (legacy.id_nganh ?? "").Trim();
        if (nganhId == "" && chiNhanhId != "")
        {
            nganh_table nganh = db.nganh_tables.FirstOrDefault(p => p.id_chinhanh == chiNhanhId);
            if (nganh != null)
                nganhId = (nganh.id + "").Trim();
        }

        string ownerDisplay = (owner.hoten ?? "").Trim();
        if (ownerDisplay == "")
            ownerDisplay = owner.taikhoan;

        string legacyDisplay = (legacy.hoten ?? "").Trim();
        if (legacyDisplay == "")
            legacyDisplay = legacy.taikhoan;

        return new WorkspaceInfo
        {
            OwnerAccountKey = ownerKey,
            OwnerDisplayName = ownerDisplay,
            OwnerPhone = (owner.dienthoai ?? "").Trim(),
            LegacyUser = Normalize(legacy.taikhoan),
            LegacyDisplayName = legacyDisplay,
            RoleLabel = NormalizeRoleLabel(roleLabel, isOwner, memberType),
            MemberType = NormalizeMemberType(memberType, isOwner),
            IsOwner = isOwner,
            ChiNhanhId = chiNhanhId,
            NganhId = nganhId
        };
    }

    private static taikhoan_tb ResolveHomeAccount(dbDataContext db, string homeAccountOrPhone)
    {
        if (db == null)
            return null;

        string raw = (homeAccountOrPhone ?? "").Trim();
        string key = Normalize(raw);
        taikhoan_tb account = null;
        if (key != "")
            account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == key);

        if (account == null && raw != "")
        {
            string normalizedPhone = NormalizePhone(raw);
            if (normalizedPhone != "")
                account = db.taikhoan_tbs.ToList().FirstOrDefault(p => NormalizePhone(p.dienthoai) == normalizedPhone || (p.dienthoai ?? "").Trim() == raw);
        }

        return account;
    }

    private static void PromotePendingMemberships(dbDataContext db, string homeAccountKey)
    {
        if (db == null)
            return;

        taikhoan_tb home = RootAccount_cl.GetByAccountKey(db, homeAccountKey);
        if (home == null)
            return;

        string phone = NormalizePhone(home.dienthoai);
        if (phone == "")
            return;

        List<WorkspaceMemberRaw> pendingRows = db.ExecuteQuery<WorkspaceMemberRaw>(
                "SELECT Id, OwnerAccountKey, MemberAccountKey, LegacyUser, MembershipStatus, RoleLabel, MemberType, PendingPhone, InviteLookupKey, CreatedBy, AcceptedAt, CreatedAt, UpdatedAt " +
                "FROM dbo.CoreGianHangAdminMember_tb WHERE PendingPhone = {0} AND MembershipStatus = {1} ORDER BY UpdatedAt DESC, Id DESC",
                phone,
                StatusPending)
            .ToList();

        if (pendingRows.Count == 0)
            return;

        DateTime now = AhaTime_cl.Now;
        foreach (WorkspaceMemberRaw row in pendingRows)
        {
            string ownerKey = Normalize(row.OwnerAccountKey);
            string legacyUser = Normalize(row.LegacyUser);
            if (ownerKey == "" || legacyUser == "" || string.Equals(ownerKey, homeAccountKey, StringComparison.OrdinalIgnoreCase))
                continue;

            taikhoan_tb owner = RootAccount_cl.GetByAccountKey(db, ownerKey);
            if (owner == null || !SpaceAccess_cl.CanAccessGianHangAdmin(db, owner))
                continue;

            taikhoan_table_2023 legacy = db.taikhoan_table_2023s.FirstOrDefault(p => p.taikhoan == legacyUser && p.user_parent == ownerKey);
            if (legacy == null)
                continue;

            string legacyStatus = (legacy.trangthai ?? "").Trim();
            if (legacyStatus != "" && !string.Equals(legacyStatus, "Đang hoạt động", StringComparison.OrdinalIgnoreCase))
                continue;

            WorkspaceMemberRaw activeConflict = db.ExecuteQuery<WorkspaceMemberRaw>(
                    "SELECT TOP 1 Id, OwnerAccountKey, MemberAccountKey, LegacyUser, MembershipStatus, RoleLabel, MemberType, PendingPhone, InviteLookupKey, CreatedBy, AcceptedAt, CreatedAt, UpdatedAt " +
                    "FROM dbo.CoreGianHangAdminMember_tb WHERE OwnerAccountKey = {0} AND MemberAccountKey = {1} AND MembershipStatus = {2} ORDER BY UpdatedAt DESC, Id DESC",
                    ownerKey,
                    homeAccountKey,
                    StatusActive)
                .FirstOrDefault();

            if (activeConflict != null && activeConflict.Id != row.Id)
                continue;

            db.ExecuteCommand(
                "UPDATE dbo.CoreGianHangAdminMember_tb " +
                "SET MemberAccountKey = {2}, MembershipStatus = {3}, PendingPhone = NULL, InviteLookupKey = NULL, UpdatedAt = {4}, AcceptedAt = ISNULL(AcceptedAt, {4}) " +
                "WHERE Id = {0} AND MembershipStatus = {1}",
                row.Id,
                StatusPending,
                homeAccountKey,
                StatusActive,
                now);
        }
    }

    private static WorkspaceInfo ResolveFromSession(IList<WorkspaceInfo> items)
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null)
            return null;

        string ownerKey = Normalize((ctx.Session[SessionSelectedOwnerKey] ?? "").ToString());
        string legacyUser = Normalize((ctx.Session[SessionSelectedLegacyUserKey] ?? "").ToString());
        if (ownerKey == "")
            return null;

        WorkspaceInfo item = items.FirstOrDefault(p =>
            string.Equals(p.OwnerAccountKey, ownerKey, StringComparison.OrdinalIgnoreCase)
            && (legacyUser == "" || string.Equals(p.LegacyUser, legacyUser, StringComparison.OrdinalIgnoreCase)));
        if (item != null)
            return item;

        ClearWorkspaceSelection();
        return null;
    }

    private static void SaveWorkspaceSelection(WorkspaceInfo workspace)
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null || workspace == null)
            return;

        ctx.Session[SessionSelectedOwnerKey] = workspace.OwnerAccountKey ?? "";
        ctx.Session[SessionSelectedLegacyUserKey] = workspace.LegacyUser ?? "";
        ctx.Session[SessionWorkspaceModeKey] = workspace.IsOwner ? "owner" : "member";
    }

    private static bool ApplyLegacySession(WorkspaceInfo workspace)
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null || workspace == null)
            return false;

        ClearLegacyAdminSessionOnly();
        ClearLegacyCookies(ctx);

        ctx.Session["user"] = workspace.LegacyUser ?? "";
        ctx.Session["chinhanh"] = workspace.ChiNhanhId ?? "";
        ctx.Session["nganh"] = workspace.NganhId ?? "";
        ctx.Session["user_parent"] = workspace.OwnerAccountKey ?? "";
        ctx.Session["gianhang_admin_owner"] = workspace.OwnerAccountKey ?? "";
        ctx.Session["gianhang_admin_home"] = GianHangAdminBridge_cl.ReadHomeAccountFromSessionOrCookie();
        ctx.Session["gianhang_admin_mode"] = workspace.IsOwner ? "owner" : "member";
        ctx.Session["gianhang_admin_role"] = workspace.RoleLabel ?? "";
        SaveWorkspaceSelection(workspace);

        return true;
    }

    private static void ClearLegacyAdminSessionOnly()
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null)
            return;

        ctx.Session["user"] = "";
        ctx.Session["chinhanh"] = "";
        ctx.Session["nganh"] = "";
        ctx.Session["user_parent"] = "";
        ctx.Session["gianhang_admin_owner"] = "";
        ctx.Session["gianhang_admin_home"] = "";
        ctx.Session["gianhang_admin_mode"] = "";
        ctx.Session["gianhang_admin_role"] = "";
    }

    private static void ClearLegacyCookies(HttpContext ctx)
    {
        if (ctx == null || ctx.Response == null)
            return;

        ExpireCookie(ctx, "save_user_admin_aka_1");
        ExpireCookie(ctx, "save_pass_admin_aka_1");
        ExpireCookie(ctx, "save_url_admin_aka_1");
    }

    private static void ExpireCookie(HttpContext ctx, string cookieName)
    {
        if (ctx == null || ctx.Response == null || string.IsNullOrWhiteSpace(cookieName))
            return;

        HttpCookie cookie = new HttpCookie(cookieName);
        cookie.Expires = DateTime.Now.AddDays(-1);
        cookie.Value = "";
        cookie.Path = "/";
        ctx.Response.Cookies.Set(cookie);
    }

    private static string NormalizeRoleLabel(string roleLabel, bool isOwner, string memberType)
    {
        string value = (roleLabel ?? "").Trim();
        if (value != "")
            return value;
        if (isOwner)
            return "Chủ không gian";
        return string.Equals(NormalizeMemberType(memberType, false), MemberTypeExpert, StringComparison.OrdinalIgnoreCase)
            ? "Chuyên gia"
            : "Nhân viên";
    }

    private static string NormalizeMemberType(string memberType, bool isOwner)
    {
        if (isOwner)
            return MemberTypeOwner;

        string value = Normalize(memberType);
        if (value == "" || value == "nhanvien" || value == "nhânviên" || value == "nhan-vien")
            return MemberTypeStaff;
        if (value == MemberTypeExpert || value == "chuyengia" || value == "chuyêngia" || value == "chuyen-gia")
            return MemberTypeExpert;
        return MemberTypeStaff;
    }

    private static string ResolveStatusLabel(string membershipStatus)
    {
        string value = Normalize(membershipStatus);
        if (value == StatusPending)
            return "Chờ liên kết";
        if (value == StatusRevoked)
            return "Đã gỡ liên kết";
        return "Đã liên kết";
    }

    private static string NormalizePhone(string raw)
    {
        string phone = AccountAuth_cl.NormalizePhone(raw);
        return (phone ?? "").Trim();
    }

    private static string ReadQueryString(string key)
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Request == null || ctx.Request.QueryString == null)
            return "";

        return (ctx.Request.QueryString[key] ?? "").Trim();
    }

    private static bool ShouldPromptWorkspaceSelection(IList<WorkspaceInfo> items)
    {
        if (items == null || items.Count == 0)
            return false;

        if (!IsBaseAdminEntryRequest())
            return false;

        bool hasJoinedSpaces = items.Any(p => p != null && !p.IsOwner);
        if (hasJoinedSpaces)
            return true;

        return items.Count > 1;
    }

    private static bool IsBaseAdminEntryRequest()
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Request == null)
            return false;

        string rawPath = "";
        if (ctx.Request.Url != null)
            rawPath = ctx.Request.Url.AbsolutePath ?? "";
        if (rawPath == "")
            rawPath = ctx.Request.Path ?? "";

        string normalized = NormalizePath(rawPath);
        return string.Equals(normalized, "/gianhang/admin", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePath(string raw)
    {
        string value = (raw ?? "").Trim();
        if (value == "")
            return "";

        value = value.ToLowerInvariant();
        while (value.Length > 1 && value.EndsWith("/"))
            value = value.Substring(0, value.Length - 1);
        return value;
    }

    private static string Normalize(string raw)
    {
        return (raw ?? "").Trim().ToLowerInvariant();
    }
}
