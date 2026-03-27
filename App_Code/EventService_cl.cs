using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public static class EventService_cl
{
    public sealed class ProgramSummaryInfo
    {
        public int DraftCount { get; set; }
        public int PendingApprovalCount { get; set; }
        public int ActiveCount { get; set; }
        public int PausedCount { get; set; }
        public int EndedCount { get; set; }
        public int ArchivedCount { get; set; }
        public int VoucherTierCount { get; set; }
        public int SalaryBonusTierCount { get; set; }
    }

    private sealed class ProgramSummaryRaw
    {
        public int DraftCount { get; set; }
        public int PendingApprovalCount { get; set; }
        public int ActiveCount { get; set; }
        public int PausedCount { get; set; }
        public int EndedCount { get; set; }
        public int ArchivedCount { get; set; }
        public int VoucherTierCount { get; set; }
        public int SalaryBonusTierCount { get; set; }
    }

    public sealed class ProgramCardItem
    {
        public long ID { get; set; }
        public string CampaignCode { get; set; }
        public string CampaignName { get; set; }
        public string CampaignType { get; set; }
        public string Description { get; set; }
        public string ShopScope { get; set; }
        public string Status { get; set; }
        public double TierStepPercent { get; set; }
        public double TierMaxPercent { get; set; }
        public int TierCapOccurrence { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string PublishedBy { get; set; }
        public int TargetShopCount { get; set; }
        public string DefinitionJson { get; set; }
        public int VersionNo { get; set; }
        public int PublishedVersionNo { get; set; }
        public bool IsPublished { get; set; }
    }

    private sealed class ProgramStateRow
    {
        public long ID { get; set; }
        public string CampaignCode { get; set; }
        public string CampaignName { get; set; }
        public string CampaignType { get; set; }
        public string ShopScope { get; set; }
        public string Status { get; set; }
        public string DefinitionJson { get; set; }
        public int VersionNo { get; set; }
        public int PublishedVersionNo { get; set; }
    }

    private sealed class ProgramSimulationSource
    {
        public long ID { get; set; }
        public string CampaignCode { get; set; }
        public string CampaignName { get; set; }
        public string CampaignType { get; set; }
        public string Status { get; set; }
        public double TierStepPercent { get; set; }
        public double TierMaxPercent { get; set; }
        public int TierCapOccurrence { get; set; }
    }

    public sealed class ProgramPublicRow
    {
        public long ProgramID { get; set; }
        public string CampaignCode { get; set; }
        public string CampaignName { get; set; }
        public string CampaignType { get; set; }
        public string ShopScope { get; set; }
        public int TargetShopCount { get; set; }
        public string Status { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public int PublishedVersionNo { get; set; }
        public string DefinitionJson { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string PublishedBy { get; set; }
    }

    public sealed class CreateProgramRequest
    {
        public string CampaignName { get; set; }
        public string CampaignType { get; set; }
        public string Description { get; set; }
        public string ShopScope { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public double TierStepPercent { get; set; }
        public double TierMaxPercent { get; set; }
        public int TierCapOccurrence { get; set; }
        public string ActorAccount { get; set; }
        public IList<string> ShopTargets { get; set; }
    }

    private sealed class ProgramTargetRow
    {
        public string ShopAccount { get; set; }
    }

    public sealed class RewardSimulationResult
    {
        public long ProgramID { get; set; }
        public string CampaignCode { get; set; }
        public string CampaignName { get; set; }
        public string CampaignType { get; set; }
        public string Status { get; set; }
        public int InputOccurrence { get; set; }
        public int EffectiveOccurrence { get; set; }
        public double Revenue { get; set; }
        public double RatePercent { get; set; }
        public double RewardValue { get; set; }
    }

    public static void EnsureReady(dbDataContext db)
    {
        EventBootstrap_cl.EnsureSchemaSafe(db);
    }

    public static ProgramSummaryInfo GetSummary(dbDataContext db)
    {
        EnsureReady(db);
        ProgramSummaryRaw raw = db.ExecuteQuery<ProgramSummaryRaw>(@"
SELECT
    ISNULL(SUM(CASE WHEN status = {0} THEN 1 ELSE 0 END), 0) AS DraftCount,
    ISNULL(SUM(CASE WHEN status = {1} THEN 1 ELSE 0 END), 0) AS PendingApprovalCount,
    ISNULL(SUM(CASE WHEN status = {2} THEN 1 ELSE 0 END), 0) AS ActiveCount,
    ISNULL(SUM(CASE WHEN status = {3} THEN 1 ELSE 0 END), 0) AS PausedCount,
    ISNULL(SUM(CASE WHEN status = {4} THEN 1 ELSE 0 END), 0) AS EndedCount,
    ISNULL(SUM(CASE WHEN status = {5} THEN 1 ELSE 0 END), 0) AS ArchivedCount,
    ISNULL(SUM(CASE WHEN campaign_type = {6} THEN 1 ELSE 0 END), 0) AS VoucherTierCount,
    ISNULL(SUM(CASE WHEN campaign_type = {7} THEN 1 ELSE 0 END), 0) AS SalaryBonusTierCount
FROM dbo.EV_Program_tb
WHERE ISNULL(is_deleted, 0) = 0
", EventPolicy_cl.StatusDraft,
   EventPolicy_cl.StatusPendingApproval,
   EventPolicy_cl.StatusActive,
   EventPolicy_cl.StatusPaused,
   EventPolicy_cl.StatusEnded,
   EventPolicy_cl.StatusArchived,
   EventPolicy_cl.CampaignTypeVoucherTier,
   EventPolicy_cl.CampaignTypeSalaryBonusTier).FirstOrDefault();

        if (raw == null)
            return new ProgramSummaryInfo();

        return new ProgramSummaryInfo
        {
            DraftCount = raw.DraftCount,
            PendingApprovalCount = raw.PendingApprovalCount,
            ActiveCount = raw.ActiveCount,
            PausedCount = raw.PausedCount,
            EndedCount = raw.EndedCount,
            ArchivedCount = raw.ArchivedCount,
            VoucherTierCount = raw.VoucherTierCount,
            SalaryBonusTierCount = raw.SalaryBonusTierCount
        };
    }

    public static List<ProgramCardItem> LoadAdminPrograms(dbDataContext db, string statusFilter, string typeFilter, string keyword, int top)
    {
        EnsureReady(db);
        int limit = top <= 0 ? 300 : top;
        List<ProgramCardItem> list = db.ExecuteQuery<ProgramCardItem>(@"
SELECT TOP ({0})
    p.id AS ID,
    p.campaign_code AS CampaignCode,
    p.campaign_name AS CampaignName,
    p.campaign_type AS CampaignType,
    p.[description] AS [Description],
    p.shop_scope AS ShopScope,
    p.[status] AS [Status],
    ISNULL(p.tier_step_percent, 5) AS TierStepPercent,
    ISNULL(p.tier_max_percent, 50) AS TierMaxPercent,
    ISNULL(p.tier_cap_occurrence, 10) AS TierCapOccurrence,
    p.start_at AS StartAt,
    p.end_at AS EndAt,
    p.created_at AS CreatedAt,
    p.updated_at AS UpdatedAt,
    p.published_at AS PublishedAt,
    p.created_by AS CreatedBy,
    p.updated_by AS UpdatedBy,
    p.published_by AS PublishedBy,
    ISNULL(t.TargetShopCount, 0) AS TargetShopCount,
    p.definition_json AS DefinitionJson,
    ISNULL(p.version_no, 1) AS VersionNo,
    ISNULL(p.published_version_no, 0) AS PublishedVersionNo,
    CASE WHEN ISNULL(p.published_version_no, 0) > 0 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsPublished
FROM dbo.EV_Program_tb p
OUTER APPLY
(
    SELECT COUNT(1) AS TargetShopCount
    FROM dbo.EV_ProgramTarget_tb x
    WHERE x.program_id = p.id
) t
WHERE ISNULL(p.is_deleted, 0) = 0
ORDER BY p.created_at DESC, p.id DESC
", limit).ToList();

        string normalizedStatusFilter = (statusFilter ?? "").Trim().ToLowerInvariant();
        if (normalizedStatusFilter != "")
        {
            string status = EventPolicy_cl.NormalizeStatus(normalizedStatusFilter);
            list = list.Where(p => string.Equals((p.Status ?? "").Trim(), status, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        string normalizedTypeFilter = (typeFilter ?? "").Trim().ToLowerInvariant();
        if (normalizedTypeFilter != "")
        {
            string campaignType = EventPolicy_cl.NormalizeCampaignType(normalizedTypeFilter);
            list = list.Where(p => string.Equals((p.CampaignType ?? "").Trim(), campaignType, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        string key = (keyword ?? "").Trim();
        if (key != "")
        {
            long idSearch;
            bool byId = long.TryParse(key, out idSearch);
            list = list.Where(p =>
                (byId && p.ID == idSearch)
                || ((p.CampaignCode ?? "").IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
                || ((p.CampaignName ?? "").IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
                || ((p.CreatedBy ?? "").IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
            ).ToList();
        }

        return list;
    }

    public static bool TryCreateTierProgram(dbDataContext db, CreateProgramRequest request, out long programId, out string message)
    {
        programId = 0;
        message = "";
        EnsureReady(db);

        if (db == null)
        {
            message = "Không kết nối được dữ liệu.";
            return false;
        }

        if (request == null)
        {
            message = "Thiếu dữ liệu tạo chiến dịch.";
            return false;
        }

        string actor = NormalizeAccount(request.ActorAccount);
        if (actor == "")
            actor = "admin";

        string campaignName = NormalizeText(request.CampaignName, "");
        if (campaignName == "")
        {
            message = "Vui lòng nhập tên chiến dịch.";
            return false;
        }

        string campaignType = EventPolicy_cl.NormalizeCampaignType(request.CampaignType);
        double stepPercent = EventPolicy_cl.NormalizePercent(request.TierStepPercent, 5);
        double maxPercent = EventPolicy_cl.NormalizePercent(request.TierMaxPercent, 50);
        if (maxPercent < stepPercent)
            maxPercent = stepPercent;
        int capOccurrence = EventPolicy_cl.NormalizeCapOccurrence(request.TierCapOccurrence);

        DateTime? startAt = request.StartAt;
        DateTime? endAt = request.EndAt;
        if (startAt.HasValue && endAt.HasValue && endAt.Value <= startAt.Value)
        {
            message = "Thời gian kết thúc phải lớn hơn thời gian bắt đầu.";
            return false;
        }

        string status = EventPolicy_cl.StatusDraft;
        string scope = NormalizeScope(request.ShopScope);
        List<string> shopTargets = NormalizeShopTargets(request.ShopTargets);
        if (scope == "selected" && shopTargets.Count == 0)
        {
            message = "Phạm vi shop chỉ định cần ít nhất 1 tài khoản shop.";
            return false;
        }
        string description = NormalizeText(request.Description, "");
        string rewardUnit = campaignType == EventPolicy_cl.CampaignTypeSalaryBonusTier ? "salary_bonus" : "voucher";
        string definitionJson = BuildTierDefinitionJson(campaignType, stepPercent, maxPercent, capOccurrence);
        DateTime now = AhaTime_cl.Now;
        string campaignCode = BuildProgramCode(db, campaignType, campaignName, now);

        decimal inserted = db.ExecuteQuery<decimal>(@"
INSERT INTO dbo.EV_Program_tb
(
    campaign_code, campaign_name, campaign_type, [description], shop_scope, [status],
    start_at, end_at, tier_step_percent, tier_max_percent, tier_cap_occurrence, reward_unit, definition_json, version_no,
    created_at, updated_at, created_by, updated_by
)
VALUES
(
    {0}, {1}, {2}, {3}, {4}, {5},
    {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13},
    {14}, {14}, {15}, {15}
);
SELECT CAST(SCOPE_IDENTITY() AS DECIMAL(19,0));
", campaignCode, campaignName, campaignType, description, scope, status,
   startAt.HasValue ? (object)startAt.Value : DBNull.Value,
   endAt.HasValue ? (object)endAt.Value : DBNull.Value,
   stepPercent, maxPercent, capOccurrence, rewardUnit, definitionJson, 1,
   now, actor).FirstOrDefault();

        programId = Convert.ToInt64(inserted);

        if (scope == "selected")
            ReplaceProgramTargets(db, programId, shopTargets, actor, now);

        string note = "Tạo chiến dịch mới: " + campaignName;
        if (scope == "selected")
            note += " | Số shop chỉ định: " + shopTargets.Count.ToString("#,##0");
        InsertAudit(db, programId, "created", note, actor, now);
        InsertVersion(db, programId, 1, definitionJson, actor, now, "Initial draft");

        message = "Đã tạo chiến dịch #" + programId.ToString() + " (" + campaignCode + ") ở trạng thái nháp.";
        return true;
    }

    public static bool TryChangeProgramStatus(dbDataContext db, long programId, string nextStatusInput, string actorAccount, out string message)
    {
        message = "";
        EnsureReady(db);
        if (db == null)
        {
            message = "Không kết nối được dữ liệu.";
            return false;
        }

        if (programId <= 0)
        {
            message = "Chiến dịch không hợp lệ.";
            return false;
        }

        ProgramStateRow row = db.ExecuteQuery<ProgramStateRow>(@"
SELECT TOP 1
    id AS ID,
    campaign_code AS CampaignCode,
    campaign_name AS CampaignName,
    campaign_type AS CampaignType,
    shop_scope AS ShopScope,
    [status] AS [Status],
    definition_json AS DefinitionJson,
    ISNULL(version_no, 1) AS VersionNo,
    ISNULL(published_version_no, 0) AS PublishedVersionNo
FROM dbo.EV_Program_tb
WHERE id = {0}
  AND ISNULL(is_deleted, 0) = 0
", programId).FirstOrDefault();

        if (row == null)
        {
            message = "Không tìm thấy chiến dịch.";
            return false;
        }

        string fromStatus = EventPolicy_cl.NormalizeStatus(row.Status);
        string toStatus = EventPolicy_cl.NormalizeStatus(nextStatusInput);

        if (!EventPolicy_cl.IsValidTransition(fromStatus, toStatus))
        {
            message = "Không thể chuyển trạng thái từ " + BuildStatusLabel(fromStatus) + " sang " + BuildStatusLabel(toStatus) + ".";
            return false;
        }

        if (fromStatus == toStatus)
        {
            message = "Chiến dịch đã ở trạng thái " + BuildStatusLabel(toStatus) + ".";
            return false;
        }

        if (toStatus == EventPolicy_cl.StatusActive)
        {
            string scope = NormalizeScope(row.ShopScope);
            if (scope == "selected")
            {
                int targetCount = db.ExecuteQuery<int>(@"
SELECT COUNT(1)
FROM dbo.EV_ProgramTarget_tb
WHERE program_id = {0}
", programId).FirstOrDefault();

                if (targetCount <= 0)
                {
                    message = "Chiến dịch phạm vi shop chỉ định chưa có shop mục tiêu.";
                    return false;
                }
            }
        }

        string actor = NormalizeAccount(actorAccount);
        if (actor == "")
            actor = "admin";
        DateTime now = AhaTime_cl.Now;

        db.ExecuteCommand(@"
UPDATE dbo.EV_Program_tb
SET
    [status] = {1},
    updated_at = {2},
    updated_by = {3},
    published_by = CASE WHEN {1} = {4} THEN ISNULL(published_by, {3}) ELSE published_by END,
    published_at = CASE WHEN {1} = {4} THEN ISNULL(published_at, {2}) ELSE published_at END
WHERE id = {0}
", programId, toStatus, now, actor, EventPolicy_cl.StatusActive);

        db.ExecuteCommand(@"
UPDATE dbo.EV_ProgramPublic_tb
SET
    [status] = {1},
    is_active = CASE WHEN {1} IN ({2}, {3}) THEN 0 ELSE is_active END,
    updated_at = {4}
WHERE program_id = {0}
  AND ISNULL(is_deleted, 0) = 0
", programId, toStatus, EventPolicy_cl.StatusEnded, EventPolicy_cl.StatusArchived, now);

        string note = "Đổi trạng thái từ " + BuildStatusLabel(fromStatus) + " sang " + BuildStatusLabel(toStatus) + ".";
        InsertAudit(db, programId, "status_changed", note, actor, now);
        message = "Đã cập nhật trạng thái chiến dịch #" + programId.ToString() + " -> " + BuildStatusLabel(toStatus) + ".";
        return true;
    }

    public static bool TrySimulateReward(dbDataContext db, long programId, int occurrence, double revenue, out RewardSimulationResult result, out string message)
    {
        result = null;
        message = "";
        EnsureReady(db);
        if (db == null)
        {
            message = "Không kết nối được dữ liệu.";
            return false;
        }

        if (programId <= 0)
        {
            message = "Vui lòng chọn chiến dịch để mô phỏng.";
            return false;
        }

        if (occurrence <= 0)
        {
            message = "Số lần phát sinh phải lớn hơn 0.";
            return false;
        }

        if (double.IsNaN(revenue) || double.IsInfinity(revenue) || revenue < 0)
        {
            message = "Doanh thu không hợp lệ.";
            return false;
        }

        ProgramSimulationSource source = db.ExecuteQuery<ProgramSimulationSource>(@"
SELECT TOP 1
    id AS ID,
    campaign_code AS CampaignCode,
    campaign_name AS CampaignName,
    campaign_type AS CampaignType,
    [status] AS [Status],
    ISNULL(tier_step_percent, 5) AS TierStepPercent,
    ISNULL(tier_max_percent, 50) AS TierMaxPercent,
    ISNULL(tier_cap_occurrence, 10) AS TierCapOccurrence
FROM dbo.EV_Program_tb
WHERE id = {0}
  AND ISNULL(is_deleted, 0) = 0
", programId).FirstOrDefault();

        if (source == null)
        {
            message = "Không tìm thấy chiến dịch cần mô phỏng.";
            return false;
        }

        int effectiveOccurrence = EventPolicy_cl.GetEffectiveOccurrence(occurrence, source.TierCapOccurrence);
        double rateRatio = EventPolicy_cl.GetRateRatio(occurrence, source.TierStepPercent, source.TierMaxPercent, source.TierCapOccurrence);
        double ratePercent = Math.Round(rateRatio * 100d, 4);
        double rewardValue = Math.Round(revenue * rateRatio, 4);

        result = new RewardSimulationResult
        {
            ProgramID = source.ID,
            CampaignCode = source.CampaignCode,
            CampaignName = source.CampaignName,
            CampaignType = source.CampaignType,
            Status = source.Status,
            InputOccurrence = occurrence,
            EffectiveOccurrence = effectiveOccurrence,
            Revenue = revenue,
            RatePercent = ratePercent,
            RewardValue = rewardValue
        };

        message = "Mô phỏng thành công theo chiến dịch " + (source.CampaignCode ?? "") + ".";
        return true;
    }

    public static List<string> ParseShopTargetsRaw(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return new List<string>();

        string[] tokens = raw.Split(new[] { ',', ';', '|', '\r', '\n', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return NormalizeShopTargets(tokens);
    }

    public static ProgramCardItem GetProgramById(dbDataContext db, long programId)
    {
        EnsureReady(db);
        if (db == null || programId <= 0)
            return null;

        return db.ExecuteQuery<ProgramCardItem>(@"
SELECT TOP 1
    p.id AS ID,
    p.campaign_code AS CampaignCode,
    p.campaign_name AS CampaignName,
    p.campaign_type AS CampaignType,
    p.[description] AS [Description],
    p.shop_scope AS ShopScope,
    p.[status] AS [Status],
    ISNULL(p.tier_step_percent, 5) AS TierStepPercent,
    ISNULL(p.tier_max_percent, 50) AS TierMaxPercent,
    ISNULL(p.tier_cap_occurrence, 10) AS TierCapOccurrence,
    p.start_at AS StartAt,
    p.end_at AS EndAt,
    p.created_at AS CreatedAt,
    p.updated_at AS UpdatedAt,
    p.published_at AS PublishedAt,
    p.created_by AS CreatedBy,
    p.updated_by AS UpdatedBy,
    p.published_by AS PublishedBy,
    ISNULL(t.TargetShopCount, 0) AS TargetShopCount,
    p.definition_json AS DefinitionJson,
    ISNULL(p.version_no, 1) AS VersionNo,
    ISNULL(p.published_version_no, 0) AS PublishedVersionNo,
    CASE WHEN ISNULL(p.published_version_no, 0) > 0 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsPublished
FROM dbo.EV_Program_tb p
OUTER APPLY
(
    SELECT COUNT(1) AS TargetShopCount
    FROM dbo.EV_ProgramTarget_tb x
    WHERE x.program_id = p.id
) t
WHERE p.id = {0}
  AND ISNULL(p.is_deleted, 0) = 0
", programId).FirstOrDefault();
    }

    public static List<string> LoadProgramTargetAccounts(dbDataContext db, long programId, int top)
    {
        EnsureReady(db);
        if (db == null || programId <= 0)
            return new List<string>();

        int limit = top <= 0 ? 200 : top;
        List<ProgramTargetRow> rows = db.ExecuteQuery<ProgramTargetRow>(@"
SELECT TOP ({0})
    shop_account AS ShopAccount
FROM dbo.EV_ProgramTarget_tb
WHERE program_id = {1}
ORDER BY id ASC
", limit, programId).ToList();

        return rows
            .Select(p => NormalizeAccount(p.ShopAccount))
            .Where(p => p != "")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static bool TryUpdateProgramTargets(dbDataContext db, long programId, string shopScopeInput, IList<string> shopTargetsInput, string actorAccount, out string message)
    {
        message = "";
        EnsureReady(db);
        if (db == null)
        {
            message = "Không kết nối được dữ liệu.";
            return false;
        }

        if (programId <= 0)
        {
            message = "Chiến dịch không hợp lệ.";
            return false;
        }

        ProgramStateRow row = db.ExecuteQuery<ProgramStateRow>(@"
SELECT TOP 1
    id AS ID,
    campaign_code AS CampaignCode,
    campaign_name AS CampaignName,
    campaign_type AS CampaignType,
    shop_scope AS ShopScope,
    [status] AS [Status]
FROM dbo.EV_Program_tb
WHERE id = {0}
  AND ISNULL(is_deleted, 0) = 0
", programId).FirstOrDefault();

        if (row == null)
        {
            message = "Không tìm thấy chiến dịch cần cập nhật phạm vi shop.";
            return false;
        }

        string status = EventPolicy_cl.NormalizeStatus(row.Status);
        if (status != EventPolicy_cl.StatusDraft
            && status != EventPolicy_cl.StatusPendingApproval
            && status != EventPolicy_cl.StatusPaused)
        {
            message = "Chỉ chỉnh phạm vi shop khi chiến dịch ở trạng thái Nháp, Chờ duyệt hoặc Tạm dừng.";
            return false;
        }

        string scope = NormalizeScope(shopScopeInput);
        List<string> targets = NormalizeShopTargets(shopTargetsInput);
        if (scope == "selected" && targets.Count == 0)
        {
            message = "Phạm vi shop chỉ định cần ít nhất 1 tài khoản shop.";
            return false;
        }

        string actor = NormalizeAccount(actorAccount);
        if (actor == "")
            actor = "admin";
        DateTime now = AhaTime_cl.Now;

        db.ExecuteCommand(@"
UPDATE dbo.EV_Program_tb
SET
    shop_scope = {1},
    updated_at = {2},
    updated_by = {3}
WHERE id = {0}
", programId, scope, now, actor);

        if (scope == "selected")
            ReplaceProgramTargets(db, programId, targets, actor, now);
        else
            ReplaceProgramTargets(db, programId, new List<string>(), actor, now);

        string note = "Cập nhật phạm vi shop -> " + (scope == "selected" ? "Danh sách chỉ định" : "Tất cả shop")
            + " | Số shop: " + targets.Count.ToString("#,##0");
        InsertAudit(db, programId, "targets_updated", note, actor, now);

        message = "Đã cập nhật phạm vi shop cho chiến dịch #" + programId.ToString() + ".";
        return true;
    }

    public static bool TrySaveProgramDefinition(dbDataContext db, long programId, string definitionJsonInput, string actorAccount, string changeNote, out int newVersionNo, out string message)
    {
        newVersionNo = 0;
        message = "";
        EnsureReady(db);
        if (db == null)
        {
            message = "Không kết nối được dữ liệu.";
            return false;
        }

        if (programId <= 0)
        {
            message = "Chiến dịch không hợp lệ.";
            return false;
        }

        ProgramStateRow row = db.ExecuteQuery<ProgramStateRow>(@"
SELECT TOP 1
    id AS ID,
    campaign_code AS CampaignCode,
    campaign_name AS CampaignName,
    campaign_type AS CampaignType,
    shop_scope AS ShopScope,
    [status] AS [Status],
    definition_json AS DefinitionJson,
    ISNULL(version_no, 1) AS VersionNo,
    ISNULL(published_version_no, 0) AS PublishedVersionNo
FROM dbo.EV_Program_tb
WHERE id = {0}
  AND ISNULL(is_deleted, 0) = 0
", programId).FirstOrDefault();

        if (row == null)
        {
            message = "Không tìm thấy chiến dịch để cập nhật định nghĩa.";
            return false;
        }

        string status = EventPolicy_cl.NormalizeStatus(row.Status);
        if (status != EventPolicy_cl.StatusDraft
            && status != EventPolicy_cl.StatusPendingApproval
            && status != EventPolicy_cl.StatusPaused)
        {
            message = "Chỉ cập nhật định nghĩa khi chiến dịch ở trạng thái Nháp, Chờ duyệt hoặc Tạm dừng.";
            return false;
        }

        string definitionJson = NormalizeText(definitionJsonInput, "");
        if (definitionJson == "")
            definitionJson = BuildTierDefinitionJson(row.CampaignType, 5, 50, 10);

        string actor = NormalizeAccount(actorAccount);
        if (actor == "")
            actor = "admin";
        DateTime now = AhaTime_cl.Now;
        int versionNo = row.VersionNo <= 0 ? 1 : row.VersionNo;
        newVersionNo = versionNo + 1;

        db.ExecuteCommand(@"
UPDATE dbo.EV_Program_tb
SET
    definition_json = {1},
    version_no = {2},
    updated_at = {3},
    updated_by = {4}
WHERE id = {0}
", programId, definitionJson, newVersionNo, now, actor);

        InsertVersion(db, programId, newVersionNo, definitionJson, actor, now, NormalizeText(changeNote, "Update definition"));
        InsertAudit(db, programId, "definition_saved", "Lưu định nghĩa phiên bản " + newVersionNo.ToString("#,##0") + ".", actor, now);

        message = "Đã lưu định nghĩa campaign ở phiên bản " + newVersionNo.ToString("#,##0") + ".";
        return true;
    }

    public static bool TryPublishProgram(dbDataContext db, long programId, string actorAccount, out string message)
    {
        message = "";
        EnsureReady(db);
        if (db == null)
        {
            message = "Không kết nối được dữ liệu.";
            return false;
        }

        if (programId <= 0)
        {
            message = "Chiến dịch không hợp lệ.";
            return false;
        }

        ProgramStateRow row = db.ExecuteQuery<ProgramStateRow>(@"
SELECT TOP 1
    id AS ID,
    campaign_code AS CampaignCode,
    campaign_name AS CampaignName,
    campaign_type AS CampaignType,
    shop_scope AS ShopScope,
    [status] AS [Status],
    definition_json AS DefinitionJson,
    ISNULL(version_no, 1) AS VersionNo,
    ISNULL(published_version_no, 0) AS PublishedVersionNo
FROM dbo.EV_Program_tb
WHERE id = {0}
  AND ISNULL(is_deleted, 0) = 0
", programId).FirstOrDefault();

        if (row == null)
        {
            message = "Không tìm thấy chiến dịch để publish.";
            return false;
        }

        if (EventPolicy_cl.NormalizeStatus(row.Status) != EventPolicy_cl.StatusActive)
        {
            message = "Chỉ publish khi chiến dịch đang ở trạng thái Đang chạy.";
            return false;
        }

        string scope = NormalizeScope(row.ShopScope);
        int targetCount = db.ExecuteQuery<int>(@"
SELECT COUNT(1)
FROM dbo.EV_ProgramTarget_tb
WHERE program_id = {0}
", programId).FirstOrDefault();
        if (scope == "selected" && targetCount <= 0)
        {
            message = "Chiến dịch phạm vi shop chỉ định chưa có shop mục tiêu.";
            return false;
        }

        string definitionJson = NormalizeText(row.DefinitionJson, "");
        if (definitionJson == "")
            definitionJson = BuildTierDefinitionJson(row.CampaignType, 5, 50, 10);

        int versionNo = row.VersionNo <= 0 ? 1 : row.VersionNo;
        string actor = NormalizeAccount(actorAccount);
        if (actor == "")
            actor = "admin";
        DateTime now = AhaTime_cl.Now;

        db.ExecuteCommand(@"
UPDATE dbo.EV_ProgramPublic_tb
SET
    campaign_code = {1},
    campaign_name = {2},
    campaign_type = {3},
    shop_scope = {4},
    target_shop_count = {5},
    [status] = {6},
    start_at = p.start_at,
    end_at = p.end_at,
    published_version_no = {7},
    definition_json = {8},
    is_active = 1,
    is_deleted = 0,
    published_at = {9},
    published_by = {10},
    updated_at = {9}
FROM dbo.EV_ProgramPublic_tb pub
INNER JOIN dbo.EV_Program_tb p ON p.id = pub.program_id
WHERE pub.program_id = {0}
  AND ISNULL(pub.is_deleted, 0) = 0
", programId, row.CampaignCode, row.CampaignName, row.CampaignType, scope, targetCount, EventPolicy_cl.StatusActive, versionNo, definitionJson, now, actor);

        int affected = db.ExecuteQuery<int>(@"
SELECT COUNT(1)
FROM dbo.EV_ProgramPublic_tb
WHERE program_id = {0}
  AND ISNULL(is_deleted, 0) = 0
", programId).FirstOrDefault();

        if (affected <= 0)
        {
            db.ExecuteCommand(@"
INSERT INTO dbo.EV_ProgramPublic_tb
(
    program_id, campaign_code, campaign_name, campaign_type, shop_scope, target_shop_count,
    [status], start_at, end_at, published_version_no, definition_json,
    is_active, is_deleted, published_at, published_by, updated_at
)
SELECT
    p.id, p.campaign_code, p.campaign_name, p.campaign_type, p.shop_scope, {1},
    p.[status], p.start_at, p.end_at, {2}, {3},
    1, 0, {4}, {5}, {4}
FROM dbo.EV_Program_tb p
WHERE p.id = {0}
", programId, targetCount, versionNo, definitionJson, now, actor);
        }

        db.ExecuteCommand(@"
UPDATE dbo.EV_Program_tb
SET
    published_version_no = {1},
    published_snapshot_json = {2},
    published_at = {3},
    published_by = {4},
    updated_at = {3},
    updated_by = {4}
WHERE id = {0}
", programId, versionNo, definitionJson, now, actor);

        InsertAudit(db, programId, "published", "Publish chiến dịch phiên bản " + versionNo.ToString("#,##0") + ".", actor, now);
        message = "Đã publish chiến dịch #" + programId.ToString() + " (version " + versionNo.ToString("#,##0") + ").";
        return true;
    }

    public static bool TryUnpublishProgram(dbDataContext db, long programId, string actorAccount, out string message)
    {
        message = "";
        EnsureReady(db);
        if (db == null)
        {
            message = "Không kết nối được dữ liệu.";
            return false;
        }

        if (programId <= 0)
        {
            message = "Chiến dịch không hợp lệ.";
            return false;
        }

        ProgramStateRow row = db.ExecuteQuery<ProgramStateRow>(@"
SELECT TOP 1
    id AS ID,
    campaign_code AS CampaignCode,
    campaign_name AS CampaignName,
    campaign_type AS CampaignType,
    shop_scope AS ShopScope,
    [status] AS [Status],
    definition_json AS DefinitionJson,
    ISNULL(version_no, 1) AS VersionNo,
    ISNULL(published_version_no, 0) AS PublishedVersionNo
FROM dbo.EV_Program_tb
WHERE id = {0}
  AND ISNULL(is_deleted, 0) = 0
", programId).FirstOrDefault();

        if (row == null)
        {
            message = "Không tìm thấy chiến dịch để unpublish.";
            return false;
        }

        string actor = NormalizeAccount(actorAccount);
        if (actor == "")
            actor = "admin";
        DateTime now = AhaTime_cl.Now;

        db.ExecuteCommand(@"
UPDATE dbo.EV_ProgramPublic_tb
SET
    is_active = 0,
    updated_at = {1}
WHERE program_id = {0}
  AND ISNULL(is_deleted, 0) = 0
", programId, now);

        db.ExecuteCommand(@"
UPDATE dbo.EV_Program_tb
SET
    published_version_no = NULL,
    published_snapshot_json = NULL,
    updated_at = {1},
    updated_by = {2}
WHERE id = {0}
", programId, now, actor);

        InsertAudit(db, programId, "unpublished", "Gỡ publish chiến dịch.", actor, now);
        message = "Đã gỡ publish chiến dịch #" + programId.ToString() + ".";
        return true;
    }

    public static List<ProgramPublicRow> LoadPublishedPrograms(dbDataContext db, string campaignTypeFilter, string statusFilter, int top)
    {
        EnsureReady(db);
        if (db == null)
            return new List<ProgramPublicRow>();

        int limit = top <= 0 ? 200 : top;
        List<ProgramPublicRow> rows = db.ExecuteQuery<ProgramPublicRow>(@"
SELECT TOP ({0})
    program_id AS ProgramID,
    campaign_code AS CampaignCode,
    campaign_name AS CampaignName,
    campaign_type AS CampaignType,
    shop_scope AS ShopScope,
    ISNULL(target_shop_count, 0) AS TargetShopCount,
    [status] AS [Status],
    start_at AS StartAt,
    end_at AS EndAt,
    ISNULL(published_version_no, 0) AS PublishedVersionNo,
    definition_json AS DefinitionJson,
    published_at AS PublishedAt,
    published_by AS PublishedBy
FROM dbo.EV_ProgramPublic_tb
WHERE ISNULL(is_deleted, 0) = 0
  AND ISNULL(is_active, 1) = 1
ORDER BY published_at DESC, id DESC
", limit).ToList();

        string type = NormalizeText(campaignTypeFilter, "").ToLowerInvariant();
        if (type != "")
        {
            string normalizedType = EventPolicy_cl.NormalizeCampaignType(type);
            rows = rows.Where(p => string.Equals((p.CampaignType ?? "").Trim(), normalizedType, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        string status = NormalizeText(statusFilter, "").ToLowerInvariant();
        if (status != "")
        {
            string normalizedStatus = EventPolicy_cl.NormalizeStatus(status);
            rows = rows.Where(p => string.Equals((p.Status ?? "").Trim(), normalizedStatus, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return rows;
    }

    private static void InsertAudit(dbDataContext db, long programId, string actionCode, string actionNote, string actor, DateTime at)
    {
        if (db == null)
            return;

        db.ExecuteCommand(@"
INSERT INTO dbo.EV_ProgramAudit_tb
(
    program_id, action_code, action_note, actor_account, created_at
)
VALUES
(
    {0}, {1}, {2}, {3}, {4}
)
", programId, NormalizeText(actionCode, ""), NormalizeText(actionNote, ""), NormalizeAccount(actor), at);
    }

    private static void InsertVersion(dbDataContext db, long programId, int versionNo, string definitionJson, string actor, DateTime at, string changeNote)
    {
        if (db == null || programId <= 0 || versionNo <= 0)
            return;

        db.ExecuteCommand(@"
INSERT INTO dbo.EV_ProgramVersion_tb
(
    program_id, version_no, definition_json, change_note, created_at, created_by
)
VALUES
(
    {0}, {1}, {2}, {3}, {4}, {5}
)
", programId, versionNo, NormalizeText(definitionJson, ""), NormalizeText(changeNote, ""), at, NormalizeAccount(actor));
    }

    private static string BuildTierDefinitionJson(string campaignType, double stepPercent, double maxPercent, int capOccurrence)
    {
        string reward = EventPolicy_cl.NormalizeCampaignType(campaignType) == EventPolicy_cl.CampaignTypeSalaryBonusTier
            ? "salary_bonus"
            : "voucher";

        string step = EventPolicy_cl.NormalizePercent(stepPercent, 5).ToString("0.####", CultureInfo.InvariantCulture);
        string max = EventPolicy_cl.NormalizePercent(maxPercent, 50).ToString("0.####", CultureInfo.InvariantCulture);
        int cap = EventPolicy_cl.NormalizeCapOccurrence(capOccurrence);

        return "{\"engine\":\"tier\",\"reward\":\"" + reward
            + "\",\"params\":{\"step_percent\":" + step
            + ",\"max_percent\":" + max
            + ",\"cap_occurrence\":" + cap.ToString(CultureInfo.InvariantCulture) + "}}";
    }

    private static string BuildProgramCode(dbDataContext db, string campaignType, string campaignName, DateTime now)
    {
        string typePrefix = campaignType == EventPolicy_cl.CampaignTypeSalaryBonusTier ? "SALARY" : "VOUCHER";
        string_class str = new string_class();
        string slug = str.replace_name_to_url(campaignName ?? "");
        if (string.IsNullOrWhiteSpace(slug))
            slug = "event";
        slug = slug.Replace("-", "_");
        if (slug.Length > 18)
            slug = slug.Substring(0, 18);
        string stamp = now.ToString("yyyyMMddHHmmss");
        string baseCode = "EVT_" + typePrefix + "_" + slug.ToUpperInvariant() + "_" + stamp;
        string candidate = baseCode;
        int suffix = 1;

        while (ProgramCodeExists(db, candidate))
        {
            suffix += 1;
            candidate = baseCode + "_" + suffix.ToString();
        }

        return candidate;
    }

    private static bool ProgramCodeExists(dbDataContext db, string campaignCode)
    {
        if (db == null || string.IsNullOrWhiteSpace(campaignCode))
            return false;

        int count = db.ExecuteQuery<int>(@"
SELECT COUNT(1)
FROM dbo.EV_Program_tb
WHERE campaign_code = {0}
  AND ISNULL(is_deleted, 0) = 0
", campaignCode).FirstOrDefault();

        return count > 0;
    }

    private static List<string> NormalizeShopTargets(IEnumerable<string> values)
    {
        if (values == null)
            return new List<string>();

        return values
            .Select(NormalizeAccount)
            .Where(p => p != "")
            .Where(p => p.Length <= 120)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static void ReplaceProgramTargets(dbDataContext db, long programId, IEnumerable<string> shopTargets, string actor, DateTime now)
    {
        if (db == null || programId <= 0)
            return;

        List<string> targets = NormalizeShopTargets(shopTargets);
        db.ExecuteCommand("DELETE FROM dbo.EV_ProgramTarget_tb WHERE program_id = {0}", programId);

        if (targets.Count == 0)
            return;

        foreach (string shop in targets)
        {
            db.ExecuteCommand(@"
INSERT INTO dbo.EV_ProgramTarget_tb
(
    program_id, shop_account, created_at, created_by
)
VALUES
(
    {0}, {1}, {2}, {3}
)
", programId, shop, now, NormalizeAccount(actor));
        }
    }

    private static string NormalizeText(string value, string fallback)
    {
        string normalized = (value ?? "").Trim();
        if (normalized == "")
            return fallback ?? "";
        return normalized;
    }

    private static string NormalizeAccount(string value)
    {
        return (value ?? "").Trim().ToLowerInvariant();
    }

    private static string NormalizeScope(string value)
    {
        string normalized = (value ?? "").Trim().ToLowerInvariant();
        if (normalized == "selected")
            return "selected";
        return "all";
    }

    private static string BuildStatusLabel(string statusRaw)
    {
        string status = EventPolicy_cl.NormalizeStatus(statusRaw);
        switch (status)
        {
            case EventPolicy_cl.StatusDraft:
                return "Nháp";
            case EventPolicy_cl.StatusPendingApproval:
                return "Chờ duyệt";
            case EventPolicy_cl.StatusActive:
                return "Đang chạy";
            case EventPolicy_cl.StatusPaused:
                return "Tạm dừng";
            case EventPolicy_cl.StatusEnded:
                return "Kết thúc";
            case EventPolicy_cl.StatusArchived:
                return "Lưu trữ";
            default:
                return "Không xác định";
        }
    }
}
