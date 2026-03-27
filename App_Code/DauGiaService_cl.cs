using System;
using System.Collections.Generic;
using System.Linq;

public static class DauGiaService_cl
{
    public sealed class AuctionSummaryInfo
    {
        public int PendingCount { get; set; }
        public int ScheduledCount { get; set; }
        public int LiveCount { get; set; }
        public int ReservedCount { get; set; }
        public int NeedSettleCount { get; set; }
        public int CompletedCount { get; set; }
        public int FailedCount { get; set; }
    }

    public class AuctionCardItem
    {
        public long ID { get; set; }
        public string Slug { get; set; }
        public string SnapshotTitle { get; set; }
        public string SnapshotImage { get; set; }
        public string SellerAccount { get; set; }
        public string TrangThai { get; set; }
        public string WinnerAccount { get; set; }
        public double GiaHienTai { get; set; }
        public double PhiLuot { get; set; }
        public int SoLuotBid { get; set; }
        public DateTime? PhienBatDau { get; set; }
        public DateTime? PhienKetThuc { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public sealed class AuctionDetailInfo
    {
        public long ID { get; set; }
        public string Slug { get; set; }
        public string SnapshotTitle { get; set; }
        public string SnapshotImage { get; set; }
        public string SellerAccount { get; set; }
        public string TrangThai { get; set; }
        public string WinnerAccount { get; set; }
        public double GiaHienTai { get; set; }
        public double PhiLuot { get; set; }
        public int SoLuotBid { get; set; }
        public DateTime? PhienBatDau { get; set; }
        public DateTime? PhienKetThuc { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string SourceType { get; set; }
        public string SourceID { get; set; }
        public string SnapshotDesc { get; set; }
        public string SnapshotMeta { get; set; }
        public string SettlementMode { get; set; }
        public double GiaNiemYet { get; set; }
        public double TienDatCoc { get; set; }
        public double TienDatCocUuDai { get; set; }
        public double TienDatCocTieuDung { get; set; }
        public double ViDauGia { get; set; }
        public double ThanhToanUuDai { get; set; }
        public double ThanhToanTieuDung { get; set; }
        public DateTime? WinnerReservedAt { get; set; }
        public DateTime? BuyerConfirmedAt { get; set; }
        public DateTime? SellerConfirmedAt { get; set; }
        public DateTime? AdminSettledAt { get; set; }
        public bool DaHoanCoc { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string RejectedReason { get; set; }
    }

    public sealed class BidItem
    {
        public long ID { get; set; }
        public string BidderAccount { get; set; }
        public double BidFee { get; set; }
        public double PriceBefore { get; set; }
        public double PriceAfter { get; set; }
        public string TrangThai { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public sealed class CreateAuctionRequest
    {
        public string SellerAccount { get; set; }
        public string SellerScope { get; set; }
        public string SourceType { get; set; }
        public string SourceID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string SnapshotMeta { get; set; }
        public double GiaNiemYet { get; set; }
        public double PhiLuot { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string SettlementMode { get; set; }
        public string CreatedBy { get; set; }
    }

    private sealed class TimeoutAuctionInfo
    {
        public long ID { get; set; }
        public string WinnerAccount { get; set; }
        public string SellerAccount { get; set; }
        public string TrangThai { get; set; }
        public double GiaHienTai { get; set; }
        public double TienDatCoc { get; set; }
        public double TienDatCocUuDai { get; set; }
        public double TienDatCocTieuDung { get; set; }
        public double ThanhToanUuDai { get; set; }
        public double ThanhToanTieuDung { get; set; }
        public DateTime? WinnerReservedAt { get; set; }
        public DateTime? BuyerConfirmedAt { get; set; }
        public bool DaHoanCoc { get; set; }
    }

    private sealed class SummaryRaw
    {
        public int PendingCount { get; set; }
        public int ScheduledCount { get; set; }
        public int LiveCount { get; set; }
        public int ReservedCount { get; set; }
        public int NeedSettleCount { get; set; }
        public int CompletedCount { get; set; }
        public int FailedCount { get; set; }
    }

    private sealed class RightsSplit
    {
        public double UuDai { get; set; }
        public double TieuDung { get; set; }
        public double Total
        {
            get
            {
                return Math.Round(Math.Max(0, UuDai) + Math.Max(0, TieuDung), 2);
            }
        }
    }

    private sealed class RightsBalanceInfo
    {
        public string Account { get; set; }
        public double UuDai { get; set; }
        public double TieuDung { get; set; }
    }

    public static void EnsureReady(dbDataContext db)
    {
        DauGiaBootstrap_cl.EnsureSchemaSafe(db);
    }

    public static AuctionSummaryInfo GetSummary(dbDataContext db)
    {
        EnsureReady(db);
        SummaryRaw raw = db.ExecuteQuery<SummaryRaw>(@"
SELECT
    ISNULL(SUM(CASE WHEN trang_thai = {0} THEN 1 ELSE 0 END), 0) AS PendingCount,
    ISNULL(SUM(CASE WHEN trang_thai = {1} THEN 1 ELSE 0 END), 0) AS ScheduledCount,
    ISNULL(SUM(CASE WHEN trang_thai = {2} THEN 1 ELSE 0 END), 0) AS LiveCount,
    ISNULL(SUM(CASE WHEN trang_thai = {3} THEN 1 ELSE 0 END), 0) AS ReservedCount,
    ISNULL(SUM(CASE WHEN trang_thai = {4} THEN 1 ELSE 0 END), 0) AS NeedSettleCount,
    ISNULL(SUM(CASE WHEN trang_thai = {5} THEN 1 ELSE 0 END), 0) AS CompletedCount,
    ISNULL(SUM(CASE WHEN trang_thai = {6} THEN 1 ELSE 0 END), 0) AS FailedCount
FROM dbo.DG_Auction_tb
WHERE ISNULL(is_deleted, 0) = 0
", DauGiaPolicy_cl.StatusPendingApproval,
   DauGiaPolicy_cl.StatusScheduled,
   DauGiaPolicy_cl.StatusLive,
   DauGiaPolicy_cl.StatusReserved,
   DauGiaPolicy_cl.StatusSellerConfirmed,
   DauGiaPolicy_cl.StatusCompleted,
   DauGiaPolicy_cl.StatusSettlementFailed).FirstOrDefault();

        if (raw == null)
            return new AuctionSummaryInfo();

        return new AuctionSummaryInfo
        {
            PendingCount = raw.PendingCount,
            ScheduledCount = raw.ScheduledCount,
            LiveCount = raw.LiveCount,
            ReservedCount = raw.ReservedCount,
            NeedSettleCount = raw.NeedSettleCount,
            CompletedCount = raw.CompletedCount,
            FailedCount = raw.FailedCount
        };
    }

    public static List<AuctionCardItem> LoadLiveAuctions(dbDataContext db, int top)
    {
        EnsureReady(db);
        int limit = top <= 0 ? 30 : top;
        return db.ExecuteQuery<AuctionCardItem>(@"
SELECT TOP ({0})
    id AS ID,
    slug AS Slug,
    snapshot_title AS SnapshotTitle,
    snapshot_image AS SnapshotImage,
    seller_account AS SellerAccount,
    trang_thai AS TrangThai,
    winner_account AS WinnerAccount,
    ISNULL(gia_hien_tai, 0) AS GiaHienTai,
    ISNULL(phi_luot, 0) AS PhiLuot,
    ISNULL(so_luot_bid, 0) AS SoLuotBid,
    phien_bat_dau AS PhienBatDau,
    phien_ket_thuc AS PhienKetThuc,
    updated_at AS UpdatedAt
FROM dbo.DG_Auction_tb
WHERE ISNULL(is_deleted, 0) = 0
  AND trang_thai = {1}
ORDER BY phien_ket_thuc ASC, id DESC
", limit, DauGiaPolicy_cl.StatusLive).ToList();
    }

    public static List<AuctionCardItem> LoadScheduledAuctions(dbDataContext db, int top)
    {
        EnsureReady(db);
        int limit = top <= 0 ? 30 : top;
        return db.ExecuteQuery<AuctionCardItem>(@"
SELECT TOP ({0})
    id AS ID,
    slug AS Slug,
    snapshot_title AS SnapshotTitle,
    snapshot_image AS SnapshotImage,
    seller_account AS SellerAccount,
    trang_thai AS TrangThai,
    winner_account AS WinnerAccount,
    ISNULL(gia_hien_tai, 0) AS GiaHienTai,
    ISNULL(phi_luot, 0) AS PhiLuot,
    ISNULL(so_luot_bid, 0) AS SoLuotBid,
    phien_bat_dau AS PhienBatDau,
    phien_ket_thuc AS PhienKetThuc,
    updated_at AS UpdatedAt
FROM dbo.DG_Auction_tb
WHERE ISNULL(is_deleted, 0) = 0
  AND trang_thai = {1}
ORDER BY phien_bat_dau ASC, id DESC
", limit, DauGiaPolicy_cl.StatusScheduled).ToList();
    }

    public static List<AuctionCardItem> LoadEndedAuctions(dbDataContext db, int top)
    {
        EnsureReady(db);
        int limit = top <= 0 ? 40 : top;
        return db.ExecuteQuery<AuctionCardItem>(@"
SELECT TOP ({0})
    id AS ID,
    slug AS Slug,
    snapshot_title AS SnapshotTitle,
    snapshot_image AS SnapshotImage,
    seller_account AS SellerAccount,
    trang_thai AS TrangThai,
    winner_account AS WinnerAccount,
    ISNULL(gia_hien_tai, 0) AS GiaHienTai,
    ISNULL(phi_luot, 0) AS PhiLuot,
    ISNULL(so_luot_bid, 0) AS SoLuotBid,
    phien_bat_dau AS PhienBatDau,
    phien_ket_thuc AS PhienKetThuc,
    updated_at AS UpdatedAt
FROM dbo.DG_Auction_tb
WHERE ISNULL(is_deleted, 0) = 0
  AND trang_thai IN ({1}, {2}, {3}, {4})
ORDER BY updated_at DESC, id DESC
", limit,
   DauGiaPolicy_cl.StatusCompleted,
   DauGiaPolicy_cl.StatusExpired,
   DauGiaPolicy_cl.StatusCancelled,
   DauGiaPolicy_cl.StatusSettlementFailed).ToList();
    }

    public static List<AuctionCardItem> LoadSellerAuctions(dbDataContext db, string sellerAccount, string statusFilter, string keyword, int top)
    {
        EnsureReady(db);
        string seller = NormalizeAccount(sellerAccount);
        if (seller == "")
            return new List<AuctionCardItem>();

        int limit = top <= 0 ? 200 : top;
        List<AuctionCardItem> list = db.ExecuteQuery<AuctionCardItem>(@"
SELECT TOP ({0})
    id AS ID,
    slug AS Slug,
    snapshot_title AS SnapshotTitle,
    snapshot_image AS SnapshotImage,
    seller_account AS SellerAccount,
    trang_thai AS TrangThai,
    winner_account AS WinnerAccount,
    ISNULL(gia_hien_tai, 0) AS GiaHienTai,
    ISNULL(phi_luot, 0) AS PhiLuot,
    ISNULL(so_luot_bid, 0) AS SoLuotBid,
    phien_bat_dau AS PhienBatDau,
    phien_ket_thuc AS PhienKetThuc,
    updated_at AS UpdatedAt
FROM dbo.DG_Auction_tb
WHERE ISNULL(is_deleted, 0) = 0
  AND seller_account = {1}
ORDER BY created_at DESC, id DESC
", limit, seller).ToList();

        string status = DauGiaPolicy_cl.NormalizeStatus(statusFilter);
        if (!string.IsNullOrWhiteSpace(statusFilter))
            list = list.Where(p => string.Equals((p.TrangThai ?? "").Trim(), status, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            string key = keyword.Trim();
            long idSearch;
            bool byId = long.TryParse(key, out idSearch);
            list = list.Where(p =>
                (byId && p.ID == idSearch)
                || ((p.SnapshotTitle ?? "").IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
                || ((p.WinnerAccount ?? "").IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
        }

        return list;
    }

    public static List<AuctionCardItem> LoadAdminAuctions(dbDataContext db, string statusFilter, string keyword, int top)
    {
        EnsureReady(db);
        int limit = top <= 0 ? 200 : top;
        List<AuctionCardItem> list = db.ExecuteQuery<AuctionCardItem>(@"
SELECT TOP ({0})
    id AS ID,
    slug AS Slug,
    snapshot_title AS SnapshotTitle,
    snapshot_image AS SnapshotImage,
    seller_account AS SellerAccount,
    trang_thai AS TrangThai,
    winner_account AS WinnerAccount,
    ISNULL(gia_hien_tai, 0) AS GiaHienTai,
    ISNULL(phi_luot, 0) AS PhiLuot,
    ISNULL(so_luot_bid, 0) AS SoLuotBid,
    phien_bat_dau AS PhienBatDau,
    phien_ket_thuc AS PhienKetThuc,
    updated_at AS UpdatedAt
FROM dbo.DG_Auction_tb
WHERE ISNULL(is_deleted, 0) = 0
ORDER BY created_at DESC, id DESC
", limit).ToList();

        string status = DauGiaPolicy_cl.NormalizeStatus(statusFilter);
        if (!string.IsNullOrWhiteSpace(statusFilter))
            list = list.Where(p => string.Equals((p.TrangThai ?? "").Trim(), status, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            string key = keyword.Trim();
            long idSearch;
            bool byId = long.TryParse(key, out idSearch);
            list = list.Where(p =>
                (byId && p.ID == idSearch)
                || ((p.SnapshotTitle ?? "").IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
                || ((p.SellerAccount ?? "").IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
                || ((p.WinnerAccount ?? "").IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
        }

        return list;
    }

    public static AuctionDetailInfo GetAuctionById(dbDataContext db, long id)
    {
        EnsureReady(db);
        return db.ExecuteQuery<AuctionDetailInfo>(@"
SELECT TOP 1
    id AS ID,
    slug AS Slug,
    source_type AS SourceType,
    source_id AS SourceID,
    snapshot_title AS SnapshotTitle,
    snapshot_desc AS SnapshotDesc,
    snapshot_image AS SnapshotImage,
    snapshot_meta AS SnapshotMeta,
    seller_account AS SellerAccount,
    trang_thai AS TrangThai,
    winner_account AS WinnerAccount,
    ISNULL(gia_niemyet, 0) AS GiaNiemYet,
    ISNULL(gia_hien_tai, 0) AS GiaHienTai,
    ISNULL(phi_luot, 0) AS PhiLuot,
    ISNULL(tien_dat_coc, 0) AS TienDatCoc,
    ISNULL(tien_dat_coc_uudai, 0) AS TienDatCocUuDai,
    ISNULL(tien_dat_coc_tieudung, 0) AS TienDatCocTieuDung,
    ISNULL(vi_dau_gia, 0) AS ViDauGia,
    ISNULL(so_luot_bid, 0) AS SoLuotBid,
    phien_bat_dau AS PhienBatDau,
    phien_ket_thuc AS PhienKetThuc,
    winner_reserved_at AS WinnerReservedAt,
    buyer_confirmed_at AS BuyerConfirmedAt,
    ISNULL(thanh_toan_uudai, 0) AS ThanhToanUuDai,
    ISNULL(thanh_toan_tieudung, 0) AS ThanhToanTieuDung,
    seller_confirmed_at AS SellerConfirmedAt,
    admin_settled_at AS AdminSettledAt,
    settlement_mode AS SettlementMode,
    CAST(ISNULL(da_hoan_coc, 0) AS BIT) AS DaHoanCoc,
    approved_by AS ApprovedBy,
    approved_at AS ApprovedAt,
    rejected_reason AS RejectedReason,
    updated_at AS UpdatedAt
FROM dbo.DG_Auction_tb
WHERE id = {0}
  AND ISNULL(is_deleted, 0) = 0
", id).FirstOrDefault();
    }

    public static List<BidItem> LoadBidHistory(dbDataContext db, long auctionId, int top)
    {
        EnsureReady(db);
        int limit = top <= 0 ? 50 : top;
        return db.ExecuteQuery<BidItem>(@"
SELECT TOP ({0})
    id AS ID,
    bidder_account AS BidderAccount,
    ISNULL(bid_fee, 0) AS BidFee,
    ISNULL(price_before, 0) AS PriceBefore,
    ISNULL(price_after, 0) AS PriceAfter,
    trang_thai AS TrangThai,
    created_at AS CreatedAt
FROM dbo.DG_Bid_tb
WHERE auction_id = {1}
ORDER BY created_at DESC, id DESC
", limit, auctionId).ToList();
    }

    public static bool TryCreateAuctionDraft(dbDataContext db, CreateAuctionRequest request, out long auctionId, out string message)
    {
        auctionId = 0;
        message = "";
        if (db == null)
        {
            message = DauGiaNotify_cl.BuildErrorMessage("Không kết nối được dữ liệu.");
            return false;
        }

        EnsureReady(db);
        if (request == null)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Thiếu dữ liệu tạo phiên đấu giá.");
            return false;
        }

        string seller = NormalizeAccount(request.SellerAccount);
        if (!DauGiaPolicy_cl.CanCreateAuction(seller))
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Bạn cần đăng nhập để tạo phiên đấu giá.");
            return false;
        }

        string title = (request.Title ?? "").Trim();
        if (title.Length < 4)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Tiêu đề phiên đấu giá không hợp lệ.");
            return false;
        }

        if (request.GiaNiemYet <= 0 || request.PhiLuot <= 0)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Giá niêm yết hoặc phí lượt không hợp lệ.");
            return false;
        }

        DateTime now = AhaTime_cl.Now;
        int startBufferMinutes = GetConfigInt(db, "start_buffer_minutes", 5);
        if (startBufferMinutes > 0 && request.StartAt < now.AddMinutes(startBufferMinutes))
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Thời gian bắt đầu phải cách hiện tại tối thiểu " + startBufferMinutes + " phút.");
            return false;
        }

        if (request.EndAt <= request.StartAt)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
            return false;
        }

        string sourceType = DauGiaPolicy_cl.NormalizeSourceType(request.SourceType);
        string sourceId = NormalizeText(request.SourceID, "");
        if (DauGiaPolicy_cl.RequiresSourceId(sourceType) && sourceId == "")
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Bạn cần chọn tài sản nguồn hợp lệ cho loại phiên này.");
            return false;
        }

        ReleaseAssetLocksForTerminalAuctions(db, "Giải phóng khóa tài sản tồn khi tạo phiên mới.");
        if (IsSourceAssetLockedByOtherAuction(db, sourceType, sourceId, 0))
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Tài sản nguồn đang được dùng ở phiên đấu giá khác.");
            return false;
        }

        double depositPercent = GetConfigDouble(db, "deposit_percent", 20d);
        double deposit = Math.Round(request.GiaNiemYet * depositPercent / 100d, 2);
        bool rightsDebited = false;
        RightsSplit depositSplit = new RightsSplit();
        try
        {
            if (!TryDebitRights(
                db,
                seller,
                deposit,
                "Đối ứng đấu giá mới.",
                BuildLedgerRef("deposit_hold", 0),
                out message,
                out depositSplit))
                return false;
            rightsDebited = depositSplit.Total > 0;

            string slug = BuildUniqueSlug(db, title);
            string settlementMode = DauGiaPolicy_cl.NormalizeSettlementMode(request.SettlementMode);
            string sellerScope = (request.SellerScope ?? "").Trim().ToLowerInvariant();
            if (sellerScope == "")
                sellerScope = "shop";

            auctionId = db.ExecuteQuery<long>(@"
INSERT INTO dbo.DG_Auction_tb
(
    slug, source_type, source_id, seller_account, seller_scope,
    snapshot_title, snapshot_desc, snapshot_image, snapshot_meta,
    gia_niemyet, gia_hien_tai, phi_luot, tien_dat_coc, tien_dat_coc_uudai, tien_dat_coc_tieudung, vi_dau_gia, so_luot_bid,
    trang_thai, phien_bat_dau, phien_ket_thuc, settlement_mode, da_hoan_coc, is_deleted,
    thanh_toan_uudai, thanh_toan_tieudung,
    created_at, updated_at, created_by, updated_by
)
VALUES
(
    {0}, {1}, {2}, {3}, {4},
    {5}, {6}, {7}, {8},
    {9}, {9}, {10}, {11}, {12}, {13}, 0, 0,
    {14}, {15}, {16}, {17}, 0, 0,
    0, 0,
    {18}, {18}, {19}, {19}
);
SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
", slug,
   sourceType,
   sourceId,
   seller,
   sellerScope,
   title,
   NormalizeText(request.Description, ""),
   NormalizeText(request.Image, ""),
   NormalizeText(request.SnapshotMeta, ""),
   request.GiaNiemYet,
   request.PhiLuot,
   deposit,
   depositSplit.UuDai,
   depositSplit.TieuDung,
   DauGiaPolicy_cl.StatusPendingApproval,
   request.StartAt,
   request.EndAt,
   settlementMode,
   now,
   NormalizeText(request.CreatedBy, seller)).FirstOrDefault();

            EnsureSourceAssetLock(db, auctionId, sourceType, sourceId, seller);
            InsertLedger(db, auctionId, seller, "admin", "deposit_hold", deposit, "Giữ cọc tạo phiên đấu giá.", "");
            message = DauGiaNotify_cl.BuildSuccessMessage("Đã tạo phiên đấu giá, đang chờ admin duyệt.");
            return true;
        }
        catch (Exception ex)
        {
            if (auctionId > 0)
                ReleaseSourceAssetLock(db, auctionId, "Giải phóng khóa nguồn do tạo phiên thất bại.");

            if (rightsDebited)
            {
                string refundMessage;
                TryCreditRights(db,
                    seller,
                    depositSplit.UuDai,
                    depositSplit.TieuDung,
                    "Hoàn cọc do lỗi tạo phiên đấu giá.",
                    BuildLedgerRef("deposit_refund_create_error", auctionId),
                    out refundMessage);
            }

            message = DauGiaNotify_cl.BuildErrorMessage("Tạo phiên đấu giá thất bại: " + ex.Message);
            return false;
        }
    }

    public static bool ApproveAuction(dbDataContext db, long auctionId, string adminUser, bool activateNow, out string message)
    {
        message = "";
        EnsureReady(db);
        AuctionDetailInfo row = GetAuctionById(db, auctionId);
        if (row == null)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Không tìm thấy phiên đấu giá.");
            return false;
        }

        if (!DauGiaPolicy_cl.IsValidTransition(row.TrangThai, DauGiaPolicy_cl.StatusScheduled)
            && !DauGiaPolicy_cl.IsValidTransition(row.TrangThai, DauGiaPolicy_cl.StatusLive))
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên đấu giá không ở trạng thái có thể duyệt.");
            return false;
        }

        DateTime now = AhaTime_cl.Now;
        string nextStatus = DauGiaPolicy_cl.StatusScheduled;
        if (activateNow)
            nextStatus = DauGiaPolicy_cl.StatusLive;
        else if (row.PhienBatDau.HasValue && row.PhienBatDau.Value <= now && (!row.PhienKetThuc.HasValue || row.PhienKetThuc.Value > now))
            nextStatus = DauGiaPolicy_cl.StatusLive;

        int updated = db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    approved_by = {1},
    approved_at = {2},
    rejected_reason = '',
    updated_at = {2},
    updated_by = {1}
WHERE id = {3}
  AND trang_thai IN ({4}, {5})
  AND ISNULL(is_deleted, 0) = 0
", nextStatus,
   NormalizeText(adminUser, "admin"),
   now,
   auctionId,
   DauGiaPolicy_cl.StatusPendingApproval,
   DauGiaPolicy_cl.StatusScheduled);

        if (updated <= 0)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên vừa đổi trạng thái, không thể duyệt ở thao tác hiện tại.");
            return false;
        }

        DauGiaNotify_cl.PushInAppNotice(
            db,
            NormalizeText(adminUser, "admin"),
            row.SellerAccount,
            BuildAuctionUrl(row.Slug, row.ID),
            "Phiên đấu giá #" + row.ID + " đã được duyệt.");

        message = DauGiaNotify_cl.BuildSuccessMessage("Đã duyệt phiên đấu giá #" + row.ID + ".");
        return true;
    }

    public static bool RejectAuction(dbDataContext db, long auctionId, string adminUser, string reason, out string message)
    {
        message = "";
        EnsureReady(db);
        AuctionDetailInfo row = GetAuctionById(db, auctionId);
        if (row == null)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Không tìm thấy phiên đấu giá.");
            return false;
        }

        string status = DauGiaPolicy_cl.NormalizeStatus(row.TrangThai);
        if (!(status == DauGiaPolicy_cl.StatusPendingApproval || status == DauGiaPolicy_cl.StatusScheduled))
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Chỉ có thể từ chối phiên đang chờ duyệt hoặc đã lịch.");
            return false;
        }

        string rejectReason = string.IsNullOrWhiteSpace(reason) ? "Admin từ chối duyệt phiên đấu giá." : reason.Trim();
        DateTime now = AhaTime_cl.Now;
        int updated = db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    rejected_reason = {1},
    updated_at = {2},
    updated_by = {3}
WHERE id = {4}
  AND trang_thai IN ({5}, {6})
  AND ISNULL(is_deleted, 0) = 0
", DauGiaPolicy_cl.StatusCancelled,
   rejectReason,
   now,
   NormalizeText(adminUser, "admin"),
   auctionId,
   DauGiaPolicy_cl.StatusPendingApproval,
   DauGiaPolicy_cl.StatusScheduled);

        if (updated <= 0)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên vừa đổi trạng thái, không thể từ chối ở thao tác hiện tại.");
            return false;
        }

        bool refundDepositOk = true;
        if (!row.DaHoanCoc && row.TienDatCoc > 0)
        {
            double refundUuDai = row.TienDatCocUuDai;
            double refundTieuDung = row.TienDatCocTieuDung;
            if (refundUuDai <= 0 && refundTieuDung <= 0)
                refundTieuDung = row.TienDatCoc;

            string refundMessage;
            refundDepositOk = TryCreditRights(
                db,
                row.SellerAccount,
                refundUuDai,
                refundTieuDung,
                "Hoàn cọc do phiên đấu giá bị từ chối.",
                BuildLedgerRef("deposit_refund_reject", row.ID),
                out refundMessage);
            if (refundDepositOk)
            {
                db.ExecuteCommand("UPDATE dbo.DG_Auction_tb SET da_hoan_coc = 1 WHERE id = {0}", auctionId);
                InsertLedger(db, auctionId, "admin", row.SellerAccount, "deposit_refund", row.TienDatCoc, "Hoàn cọc do từ chối duyệt.", "");
            }
        }

        ReleaseSourceAssetLock(db, row.ID, "Giải phóng khóa nguồn do phiên bị từ chối.");

        DauGiaNotify_cl.PushInAppNotice(
            db,
            NormalizeText(adminUser, "admin"),
            row.SellerAccount,
            BuildAuctionUrl(row.Slug, row.ID),
            "Phiên đấu giá #" + row.ID + " đã bị từ chối. Lý do: " + rejectReason);

        if (refundDepositOk)
            message = DauGiaNotify_cl.BuildSuccessMessage("Đã từ chối phiên đấu giá #" + row.ID + ".");
        else
            message = DauGiaNotify_cl.BuildWarningMessage("Đã từ chối phiên đấu giá #" + row.ID + " nhưng hoàn cọc thất bại, cần xử lý thủ công.");
        return true;
    }

    public static bool SellerCancelAuction(dbDataContext db, long auctionId, string sellerAccount, string reason, out string message)
    {
        message = "";
        EnsureReady(db);
        string seller = NormalizeAccount(sellerAccount);
        if (seller == "")
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Bạn cần đăng nhập để hủy phiên.");
            return false;
        }

        AuctionDetailInfo row = GetAuctionById(db, auctionId);
        if (row == null)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Không tìm thấy phiên đấu giá.");
            return false;
        }

        if (!string.Equals(NormalizeAccount(row.SellerAccount), seller, StringComparison.OrdinalIgnoreCase))
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Bạn không phải chủ phiên đấu giá này.");
            return false;
        }

        string status = DauGiaPolicy_cl.NormalizeStatus(row.TrangThai);
        if (!(status == DauGiaPolicy_cl.StatusPendingApproval || status == DauGiaPolicy_cl.StatusScheduled))
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Chỉ có thể hủy phiên ở trạng thái chờ duyệt hoặc đã lịch.");
            return false;
        }

        DateTime now = AhaTime_cl.Now;
        string cancelReason = string.IsNullOrWhiteSpace(reason)
            ? "Chủ phiên tự hủy đấu giá."
            : reason.Trim();

        int updated = db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    rejected_reason = {1},
    updated_at = {2},
    updated_by = {3}
WHERE id = {4}
  AND seller_account = {3}
  AND trang_thai IN ({5}, {6})
  AND ISNULL(is_deleted, 0) = 0
", DauGiaPolicy_cl.StatusCancelled,
   cancelReason,
   now,
   seller,
   row.ID,
   DauGiaPolicy_cl.StatusPendingApproval,
   DauGiaPolicy_cl.StatusScheduled);

        if (updated <= 0)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên vừa đổi trạng thái, không thể hủy ở thao tác hiện tại.");
            return false;
        }

        bool refundDepositOk = true;
        if (!row.DaHoanCoc && row.TienDatCoc > 0)
        {
            double refundUuDai = row.TienDatCocUuDai;
            double refundTieuDung = row.TienDatCocTieuDung;
            if (refundUuDai <= 0 && refundTieuDung <= 0)
                refundTieuDung = row.TienDatCoc;

            string refundMessage;
            refundDepositOk = TryCreditRights(
                db,
                row.SellerAccount,
                refundUuDai,
                refundTieuDung,
                "Hoàn cọc do chủ phiên hủy đấu giá.",
                BuildLedgerRef("deposit_refund_seller_cancel", row.ID),
                out refundMessage);
            if (refundDepositOk)
            {
                db.ExecuteCommand("UPDATE dbo.DG_Auction_tb SET da_hoan_coc = 1 WHERE id = {0}", row.ID);
                InsertLedger(db, row.ID, "admin", row.SellerAccount, "deposit_refund_seller_cancel", row.TienDatCoc, "Hoàn cọc do chủ phiên hủy.", "");
            }
        }

        ReleaseSourceAssetLock(db, row.ID, "Giải phóng khóa nguồn do chủ phiên hủy.");

        DauGiaNotify_cl.PushInAppNotice(
            db,
            seller,
            "admin",
            "/admin/quan-ly-dau-gia",
            "Chủ phiên đã hủy phiên đấu giá #" + row.ID + ".");

        if (refundDepositOk)
            message = DauGiaNotify_cl.BuildSuccessMessage("Đã hủy phiên đấu giá #" + row.ID + ".");
        else
            message = DauGiaNotify_cl.BuildWarningMessage("Đã hủy phiên đấu giá #" + row.ID + " nhưng hoàn cọc thất bại, cần xử lý thủ công.");
        return true;
    }

    public static int ActivateScheduled(dbDataContext db, out string message)
    {
        EnsureReady(db);
        DateTime now = AhaTime_cl.Now;
        int activated = db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    updated_at = {1},
    updated_by = 'system'
WHERE trang_thai = {2}
  AND phien_bat_dau <= {1}
  AND (phien_ket_thuc IS NULL OR phien_ket_thuc > {1})
  AND ISNULL(is_deleted, 0) = 0
", DauGiaPolicy_cl.StatusLive, now, DauGiaPolicy_cl.StatusScheduled);

        message = activated > 0
            ? "Đã kích hoạt " + activated + " phiên đấu giá đến giờ mở."
            : "Không có phiên lịch nào đến giờ mở.";
        return activated;
    }

    public static int RunAutoClose(dbDataContext db, out string message)
    {
        EnsureReady(db);
        DateTime now = AhaTime_cl.Now;

        int activated = ActivateScheduled(db, out message);
        int expired = db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    updated_at = {1},
    updated_by = 'system'
WHERE trang_thai = {2}
  AND phien_ket_thuc IS NOT NULL
  AND phien_ket_thuc < {1}
  AND ISNULL(is_deleted, 0) = 0
", DauGiaPolicy_cl.StatusExpired, now, DauGiaPolicy_cl.StatusLive);

        int reserveTimeoutMinutes = GetConfigInt(db, "reserve_timeout_minutes", 10080);
        List<TimeoutAuctionInfo> timeoutRows = db.ExecuteQuery<TimeoutAuctionInfo>(@"
SELECT
    id AS ID,
    winner_account AS WinnerAccount,
    seller_account AS SellerAccount,
    trang_thai AS TrangThai,
    ISNULL(gia_hien_tai, 0) AS GiaHienTai,
    ISNULL(tien_dat_coc, 0) AS TienDatCoc,
    ISNULL(tien_dat_coc_uudai, 0) AS TienDatCocUuDai,
    ISNULL(tien_dat_coc_tieudung, 0) AS TienDatCocTieuDung,
    ISNULL(thanh_toan_uudai, 0) AS ThanhToanUuDai,
    ISNULL(thanh_toan_tieudung, 0) AS ThanhToanTieuDung,
    winner_reserved_at AS WinnerReservedAt,
    buyer_confirmed_at AS BuyerConfirmedAt,
    CAST(ISNULL(da_hoan_coc, 0) AS BIT) AS DaHoanCoc
FROM dbo.DG_Auction_tb
WHERE trang_thai IN ({0}, {1}, {2})
  AND winner_reserved_at IS NOT NULL
  AND ISNULL(is_deleted, 0) = 0
", DauGiaPolicy_cl.StatusReserved, DauGiaPolicy_cl.StatusBuyerConfirmed, DauGiaPolicy_cl.StatusSellerConfirmed).ToList();

        int settlementFailedCount = 0;
        int refundFailedCount = 0;
        foreach (TimeoutAuctionInfo row in timeoutRows)
        {
            DateTime reserveAt = row.WinnerReservedAt.HasValue ? row.WinnerReservedAt.Value : now;
            DateTime deadline = reserveAt.AddMinutes(reserveTimeoutMinutes);
            if (deadline >= now)
                continue;

            if (row.BuyerConfirmedAt.HasValue && row.GiaHienTai > 0 && !string.IsNullOrWhiteSpace(row.WinnerAccount))
            {
                double refundBuyerUuDai = row.ThanhToanUuDai;
                double refundBuyerTieuDung = row.ThanhToanTieuDung;
                if (refundBuyerUuDai <= 0 && refundBuyerTieuDung <= 0)
                    refundBuyerTieuDung = row.GiaHienTai;

                string buyerRefundMessage;
                bool buyerRefundOk = TryCreditRights(
                    db,
                    row.WinnerAccount,
                    refundBuyerUuDai,
                    refundBuyerTieuDung,
                    "Hoàn tiền do phiên đấu giá quá hạn xác nhận.",
                    BuildLedgerRef("buyer_refund_timeout", row.ID),
                    out buyerRefundMessage);
                if (buyerRefundOk)
                    InsertLedger(db, row.ID, "admin", row.WinnerAccount, "buyer_refund_timeout", row.GiaHienTai, "Hoàn tiền cho người mua khi quá hạn.", "");
                else
                    refundFailedCount++;
            }

            bool depositRefunded = row.DaHoanCoc;
            if (!row.DaHoanCoc && row.TienDatCoc > 0 && !string.IsNullOrWhiteSpace(row.SellerAccount))
            {
                double refundDepositUuDai = row.TienDatCocUuDai;
                double refundDepositTieuDung = row.TienDatCocTieuDung;
                if (refundDepositUuDai <= 0 && refundDepositTieuDung <= 0)
                    refundDepositTieuDung = row.TienDatCoc;

                string depositRefundMessage;
                depositRefunded = TryCreditRights(
                    db,
                    row.SellerAccount,
                    refundDepositUuDai,
                    refundDepositTieuDung,
                    "Hoàn cọc do phiên đấu giá quá hạn xử lý.",
                    BuildLedgerRef("deposit_refund_timeout", row.ID),
                    out depositRefundMessage);
                if (depositRefunded)
                    InsertLedger(db, row.ID, "admin", row.SellerAccount, "deposit_refund_timeout", row.TienDatCoc, "Hoàn cọc cho chủ phiên khi quá hạn.", "");
                else
                    refundFailedCount++;
            }

            db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    da_hoan_coc = {1},
    updated_at = {2},
    updated_by = 'system'
WHERE id = {3}
", DauGiaPolicy_cl.StatusSettlementFailed, depositRefunded ? 1 : 0, now, row.ID);

            settlementFailedCount++;
        }

        ReleaseAssetLocksForTerminalAuctions(db, "Giải phóng khóa tài sản theo tác vụ auto-close.");

        message = "Tự động xử lý: mở " + activated + ", hết hạn live " + expired + ", quá hạn xác nhận " + settlementFailedCount + ", hoàn tiền lỗi " + refundFailedCount + ".";
        return activated + expired + settlementFailedCount;
    }

    public static bool PlaceBid(dbDataContext db, long auctionId, string bidderAccount, out double newPrice, out string message)
    {
        newPrice = 0;
        message = "";
        EnsureReady(db);

        string bidder = NormalizeAccount(bidderAccount);
        if (bidder == "")
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Bạn cần đăng nhập để đấu giá.");
            return false;
        }

        AuctionDetailInfo row = GetAuctionById(db, auctionId);
        if (row == null)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên đấu giá không tồn tại.");
            return false;
        }

        if (!string.Equals(DauGiaPolicy_cl.NormalizeStatus(row.TrangThai), DauGiaPolicy_cl.StatusLive, StringComparison.OrdinalIgnoreCase))
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên đấu giá hiện không ở trạng thái đang diễn ra.");
            return false;
        }

        DateTime now = AhaTime_cl.Now;
        if (row.PhienKetThuc.HasValue && row.PhienKetThuc.Value < now)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên đấu giá đã hết thời gian.");
            return false;
        }

        if (string.Equals(NormalizeAccount(row.SellerAccount), bidder, StringComparison.OrdinalIgnoreCase))
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Chủ phiên không thể tự đấu giá.");
            return false;
        }

        double fee = row.PhiLuot;
        if (fee <= 0)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên đấu giá chưa có phí lượt hợp lệ.");
            return false;
        }

        RightsSplit feeSplit;
        if (!TryDebitRights(
            db,
            bidder,
            fee,
            "Thanh toán phí lượt đấu giá #" + row.ID + ".",
            BuildLedgerRef("bid_fee_debit", row.ID),
            out message,
            out feeSplit))
            return false;

        double before = row.GiaHienTai;
        double after = Math.Round(Math.Max(0, before - fee), 2);

        int updated = db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET gia_hien_tai = {0},
    vi_dau_gia = ISNULL(vi_dau_gia, 0) + {1},
    so_luot_bid = ISNULL(so_luot_bid, 0) + 1,
    updated_at = {2},
    updated_by = {3}
WHERE id = {4}
  AND trang_thai = {5}
  AND (phien_ket_thuc IS NULL OR phien_ket_thuc > {2})
  AND ISNULL(is_deleted, 0) = 0
", after, fee, now, bidder, row.ID, DauGiaPolicy_cl.StatusLive);

        if (updated <= 0)
        {
            string refundMessage;
            TryCreditRights(
                db,
                bidder,
                feeSplit.UuDai,
                feeSplit.TieuDung,
                "Hoàn phí bid do trạng thái phiên vừa thay đổi.",
                BuildLedgerRef("bid_fee_refund_state_changed", row.ID),
                out refundMessage);
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên đấu giá vừa đổi trạng thái, hệ thống đã hoàn lại phí bid.");
            return false;
        }

        db.ExecuteCommand(@"
INSERT INTO dbo.DG_Bid_tb
(
    auction_id, bidder_account, bid_fee, price_before, price_after, trang_thai, created_at, created_by
)
VALUES
(
    {0}, {1}, {2}, {3}, {4}, {5}, {6}, {1}
)
", row.ID, bidder, fee, before, after, "bid", now);

        InsertLedger(db, row.ID, bidder, "admin", "bid_fee_debit", fee, "Trừ phí lượt đấu giá.", "");
        InsertLedger(db, row.ID, "admin", row.SellerAccount, "bid_pool_credit", fee, "Cộng vào ví đấu giá của phiên.", "");

        DauGiaNotify_cl.PushInAppNotice(
            db,
            bidder,
            row.SellerAccount,
            BuildAuctionUrl(row.Slug, row.ID),
            "Phiên đấu giá #" + row.ID + " vừa có lượt đấu giá mới.");

        newPrice = after;
        message = DauGiaNotify_cl.BuildSuccessMessage("Đấu giá thành công. Giá hiện tại còn " + after.ToString("#,##0.00") + " Quyền.");
        return true;
    }

    public static bool ReserveWinner(dbDataContext db, long auctionId, string buyerAccount, out string message)
    {
        message = "";
        EnsureReady(db);
        string buyer = NormalizeAccount(buyerAccount);
        if (buyer == "")
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Bạn cần đăng nhập để chốt mua.");
            return false;
        }

        AuctionDetailInfo row = GetAuctionById(db, auctionId);
        if (row == null)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên đấu giá không tồn tại.");
            return false;
        }

        if (DauGiaPolicy_cl.NormalizeStatus(row.TrangThai) != DauGiaPolicy_cl.StatusLive)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên đấu giá không ở trạng thái có thể chốt mua.");
            return false;
        }

        if (!string.IsNullOrWhiteSpace(row.WinnerAccount))
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên này đã có người chốt mua.");
            return false;
        }

        DateTime now = AhaTime_cl.Now;
        if (row.PhienKetThuc.HasValue && row.PhienKetThuc.Value < now)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên đấu giá đã kết thúc.");
            return false;
        }

        int ownBidCount = db.ExecuteQuery<int>(@"
SELECT COUNT(1)
FROM dbo.DG_Bid_tb
WHERE auction_id = {0}
  AND bidder_account = {1}
", row.ID, buyer).FirstOrDefault();
        if (ownBidCount <= 0)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Bạn cần có ít nhất 1 lượt đấu giá trước khi giữ mua.");
            return false;
        }

        int updated = db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET winner_account = {0},
    winner_reserved_at = {1},
    trang_thai = {2},
    updated_at = {1},
    updated_by = {0}
WHERE id = {3}
  AND trang_thai = {4}
  AND (winner_account IS NULL OR winner_account = '')
  AND ISNULL(is_deleted, 0) = 0
", buyer, now, DauGiaPolicy_cl.StatusReserved, row.ID, DauGiaPolicy_cl.StatusLive);

        if (updated <= 0)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên vừa được người khác chốt hoặc trạng thái đã thay đổi.");
            return false;
        }

        db.ExecuteCommand(@"
UPDATE dbo.DG_Bid_tb
SET trang_thai = 'reserved'
WHERE id = (
    SELECT TOP 1 id
    FROM dbo.DG_Bid_tb
    WHERE auction_id = {0}
      AND bidder_account = {1}
    ORDER BY created_at DESC, id DESC
)
", row.ID, buyer);

        DauGiaNotify_cl.PushInAppNotice(
            db,
            buyer,
            row.SellerAccount,
            BuildAuctionUrl(row.Slug, row.ID),
            "Phiên đấu giá #" + row.ID + " đã có người chốt mua.");

        message = DauGiaNotify_cl.BuildSuccessMessage("Đã giữ quyền mua. Vui lòng xác nhận thanh toán trước thời hạn.");
        return true;
    }

    public static bool BuyerConfirmPayment(dbDataContext db, long auctionId, string buyerAccount, out string message)
    {
        message = "";
        EnsureReady(db);
        string buyer = NormalizeAccount(buyerAccount);
        if (buyer == "")
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Bạn cần đăng nhập để xác nhận thanh toán.");
            return false;
        }

        AuctionDetailInfo row = GetAuctionById(db, auctionId);
        if (row == null)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên đấu giá không tồn tại.");
            return false;
        }

        if (DauGiaPolicy_cl.NormalizeStatus(row.TrangThai) != DauGiaPolicy_cl.StatusReserved)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên đấu giá không ở trạng thái chờ khách xác nhận.");
            return false;
        }

        if (!string.Equals(NormalizeAccount(row.WinnerAccount), buyer, StringComparison.OrdinalIgnoreCase))
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Bạn không phải người đang giữ quyền mua phiên này.");
            return false;
        }

        double payAmount = row.GiaHienTai;
        RightsSplit paymentSplit;
        if (!TryDebitRights(
            db,
            buyer,
            payAmount,
            "Thanh toán giá chốt đấu giá #" + row.ID + ".",
            BuildLedgerRef("buyer_payment_debit", row.ID),
            out message,
            out paymentSplit))
            return false;

        DateTime now = AhaTime_cl.Now;
        int updated = db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    buyer_confirmed_at = {1},
    thanh_toan_uudai = ISNULL(thanh_toan_uudai, 0) + {2},
    thanh_toan_tieudung = ISNULL(thanh_toan_tieudung, 0) + {3},
    updated_at = {1},
    updated_by = {4}
WHERE id = {5}
  AND trang_thai = {6}
  AND winner_account = {4}
  AND ISNULL(is_deleted, 0) = 0
", DauGiaPolicy_cl.StatusBuyerConfirmed, now, paymentSplit.UuDai, paymentSplit.TieuDung, buyer, row.ID, DauGiaPolicy_cl.StatusReserved);

        if (updated <= 0)
        {
            string refundMessage;
            TryCreditRights(
                db,
                buyer,
                paymentSplit.UuDai,
                paymentSplit.TieuDung,
                "Hoàn tiền do trạng thái phiên vừa thay đổi.",
                BuildLedgerRef("buyer_payment_refund_state_changed", row.ID),
                out refundMessage);
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên vừa đổi trạng thái, hệ thống đã hoàn lại số tiền thanh toán.");
            return false;
        }

        InsertLedger(db, row.ID, buyer, "admin", "buyer_payment_debit", payAmount, "Thanh toán giá mua đấu giá.", "");

        DauGiaNotify_cl.PushInAppNotice(
            db,
            buyer,
            row.SellerAccount,
            BuildAuctionUrl(row.Slug, row.ID),
            "Người mua đã xác nhận thanh toán cho phiên #" + row.ID + ".");

        message = DauGiaNotify_cl.BuildSuccessMessage("Đã xác nhận thanh toán thành công.");
        return true;
    }

    public static bool SellerConfirmFulfillment(dbDataContext db, long auctionId, string sellerAccount, out string message)
    {
        message = "";
        EnsureReady(db);
        string seller = NormalizeAccount(sellerAccount);
        if (seller == "")
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Không xác định được tài khoản xác nhận.");
            return false;
        }

        AuctionDetailInfo row = GetAuctionById(db, auctionId);
        if (row == null)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên đấu giá không tồn tại.");
            return false;
        }

        if (!string.Equals(NormalizeAccount(row.SellerAccount), seller, StringComparison.OrdinalIgnoreCase))
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Bạn không phải chủ phiên đấu giá này.");
            return false;
        }

        if (DauGiaPolicy_cl.NormalizeStatus(row.TrangThai) != DauGiaPolicy_cl.StatusBuyerConfirmed)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên chưa ở trạng thái chờ shop xác nhận.");
            return false;
        }

        DateTime now = AhaTime_cl.Now;
        int updated = db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    seller_confirmed_at = {1},
    updated_at = {1},
    updated_by = {2}
WHERE id = {3}
  AND seller_account = {2}
  AND trang_thai = {4}
  AND ISNULL(is_deleted, 0) = 0
", DauGiaPolicy_cl.StatusSellerConfirmed, now, seller, row.ID, DauGiaPolicy_cl.StatusBuyerConfirmed);

        if (updated <= 0)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên vừa đổi trạng thái, không thể xác nhận lại.");
            return false;
        }

        DauGiaNotify_cl.PushInAppNotice(
            db,
            seller,
            "admin",
            "/admin/quan-ly-dau-gia",
            "Phiên đấu giá #" + row.ID + " đã đủ điều kiện tất toán.");

        message = DauGiaNotify_cl.BuildSuccessMessage("Đã xác nhận hoàn tất phía shop.");
        return true;
    }

    public static bool AdminSettle(dbDataContext db, long auctionId, string adminUser, out string message)
    {
        message = "";
        EnsureReady(db);
        AuctionDetailInfo row = GetAuctionById(db, auctionId);
        if (row == null)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Không tìm thấy phiên đấu giá.");
            return false;
        }

        if (DauGiaPolicy_cl.NormalizeStatus(row.TrangThai) != DauGiaPolicy_cl.StatusSellerConfirmed)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên chưa đủ điều kiện admin tất toán.");
            return false;
        }

        DateTime now = AhaTime_cl.Now;
        string admin = NormalizeText(adminUser, "admin");
        int claimed = db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET admin_settled_at = {0},
    updated_at = {0},
    updated_by = {1}
WHERE id = {2}
  AND trang_thai = {3}
  AND admin_settled_at IS NULL
  AND ISNULL(is_deleted, 0) = 0
", now, admin, row.ID, DauGiaPolicy_cl.StatusSellerConfirmed);

        if (claimed <= 0)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Phiên vừa được xử lý bởi tác vụ khác.");
            return false;
        }

        // Giữ nguyên công thức bản cũ: payout cố định cho shop = giá niêm yết - cọc.
        double sellerPayout = Math.Round(Math.Max(0, row.GiaNiemYet - row.TienDatCoc), 2);
        string payoutMessage;
        if (!TryCreditRights(
            db,
            row.SellerAccount,
            0,
            sellerPayout,
            "Tất toán đấu giá #" + row.ID + ".",
            BuildLedgerRef("seller_payout_credit", row.ID),
            out payoutMessage))
        {
            db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    updated_at = {1},
    updated_by = {2}
WHERE id = {3}
", DauGiaPolicy_cl.StatusSettlementFailed, now, admin, row.ID);

            ReleaseSourceAssetLock(db, row.ID, "Giải phóng khóa nguồn do lỗi tất toán.");

            message = DauGiaNotify_cl.BuildErrorMessage("Không thể cộng ví cho chủ phiên. Phiên đã chuyển sang lỗi tất toán.");
            return false;
        }

        db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    updated_at = {1},
    updated_by = {2}
WHERE id = {3}
", DauGiaPolicy_cl.StatusCompleted, now, admin, row.ID);

        ReleaseSourceAssetLock(db, row.ID, "Giải phóng khóa nguồn do tất toán hoàn tất.");

        InsertLedger(db, row.ID, "admin", row.SellerAccount, "seller_payout_credit", sellerPayout, "Tất toán hoàn thành cho chủ phiên.", "");

        if (!string.IsNullOrWhiteSpace(row.WinnerAccount))
        {
            DauGiaNotify_cl.PushInAppNotice(
                db,
                admin,
                row.WinnerAccount,
                BuildAuctionUrl(row.Slug, row.ID),
                "Admin đã xác nhận hoàn tất phiên đấu giá #" + row.ID + ".");
        }

        DauGiaNotify_cl.PushInAppNotice(
            db,
            admin,
            row.SellerAccount,
            BuildAuctionUrl(row.Slug, row.ID),
            "Admin đã tất toán thành công phiên đấu giá #" + row.ID + ".");

        message = DauGiaNotify_cl.BuildSuccessMessage("Đã tất toán phiên đấu giá #" + row.ID + ".");
        return true;
    }

    private static string NormalizeAccount(string account)
    {
        return (account ?? "").Trim().ToLowerInvariant();
    }

    private static string NormalizeText(string value, string fallback)
    {
        string normalized = (value ?? "").Trim();
        if (normalized == "")
            return fallback ?? "";
        return normalized;
    }

    private static string BuildAuctionUrl(string slug, long id)
    {
        string normalizedSlug = (slug ?? "").Trim().ToLowerInvariant();
        if (normalizedSlug == "")
            normalizedSlug = "dau-gia";
        return "/daugia/" + normalizedSlug + "-" + id + ".html";
    }

    private static string BuildUniqueSlug(dbDataContext db, string title)
    {
        string_class str = new string_class();
        string baseSlug = str.replace_name_to_url(title ?? "");
        if (string.IsNullOrWhiteSpace(baseSlug))
            baseSlug = "dau-gia";

        string slug = baseSlug;
        int index = 1;
        while (true)
        {
            int exists = db.ExecuteQuery<int>(
                "SELECT COUNT(1) FROM dbo.DG_Auction_tb WHERE slug = {0} AND ISNULL(is_deleted, 0) = 0",
                slug).FirstOrDefault();
            if (exists == 0)
                return slug;

            index++;
            slug = baseSlug + "-" + index;
        }
    }

    private static int GetConfigInt(dbDataContext db, string key, int defaultValue)
    {
        int parsed;
        string raw = GetConfig(db, key, defaultValue.ToString());
        if (int.TryParse(raw, out parsed))
            return parsed;
        return defaultValue;
    }

    private static double GetConfigDouble(dbDataContext db, string key, double defaultValue)
    {
        double parsed;
        string raw = GetConfig(db, key, defaultValue.ToString());
        if (double.TryParse(raw, out parsed))
            return parsed;
        return defaultValue;
    }

    private static string GetConfig(dbDataContext db, string key, string defaultValue)
    {
        string value = db.ExecuteQuery<string>(
            "SELECT TOP 1 config_value FROM dbo.DG_Config_tb WHERE config_key = {0}",
            key).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;
        return value.Trim();
    }

    private static bool IsSourceAssetLockedByOtherAuction(dbDataContext db, string sourceType, string sourceId, long ignoreAuctionId)
    {
        if (db == null)
            return false;

        string type = DauGiaPolicy_cl.NormalizeSourceType(sourceType);
        string id = NormalizeText(sourceId, "");
        if (id == "")
            return false;

        int lockCount = db.ExecuteQuery<int>(@"
SELECT COUNT(1)
FROM dbo.DG_AssetLock_tb l
INNER JOIN dbo.DG_Auction_tb a ON a.id = l.auction_id
WHERE l.source_type = {0}
  AND l.source_id = {1}
  AND ISNULL(l.trang_thai, '') = 'locked'
  AND l.auction_id <> {2}
  AND ISNULL(a.is_deleted, 0) = 0
  AND a.trang_thai IN ({3}, {4}, {5}, {6}, {7}, {8})
", type,
   id,
   ignoreAuctionId,
   DauGiaPolicy_cl.StatusPendingApproval,
   DauGiaPolicy_cl.StatusScheduled,
   DauGiaPolicy_cl.StatusLive,
   DauGiaPolicy_cl.StatusReserved,
   DauGiaPolicy_cl.StatusBuyerConfirmed,
   DauGiaPolicy_cl.StatusSellerConfirmed).FirstOrDefault();
        return lockCount > 0;
    }

    private static void EnsureSourceAssetLock(dbDataContext db, long auctionId, string sourceType, string sourceId, string ownerAccount)
    {
        if (db == null || auctionId <= 0)
            return;

        string type = DauGiaPolicy_cl.NormalizeSourceType(sourceType);
        string id = NormalizeText(sourceId, "");
        if (id == "")
            return;

        int exists = db.ExecuteQuery<int>(@"
SELECT COUNT(1)
FROM dbo.DG_AssetLock_tb
WHERE auction_id = {0}
  AND source_type = {1}
  AND source_id = {2}
", auctionId, type, id).FirstOrDefault();
        if (exists > 0)
            return;

        db.ExecuteCommand(@"
INSERT INTO dbo.DG_AssetLock_tb
(
    auction_id, source_type, source_id, owner_account, so_luong_khoa, trang_thai, created_at
)
VALUES
(
    {0}, {1}, {2}, {3}, 1, 'locked', {4}
)
", auctionId, type, id, NormalizeText(ownerAccount, ""), AhaTime_cl.Now);
    }

    private static void ReleaseSourceAssetLock(dbDataContext db, long auctionId, string reason)
    {
        if (db == null || auctionId <= 0)
            return;

        db.ExecuteCommand(@"
UPDATE dbo.DG_AssetLock_tb
SET trang_thai = 'released',
    released_at = {0},
    released_reason = CASE
        WHEN ISNULL(released_reason, '') = '' THEN {1}
        ELSE released_reason
    END
WHERE auction_id = {2}
  AND ISNULL(trang_thai, '') = 'locked'
", AhaTime_cl.Now, NormalizeText(reason, "Giải phóng khóa tài sản nguồn."), auctionId);
    }

    private static int ReleaseAssetLocksForTerminalAuctions(dbDataContext db, string reason)
    {
        if (db == null)
            return 0;

        return db.ExecuteCommand(@"
UPDATE l
SET l.trang_thai = 'released',
    l.released_at = {0},
    l.released_reason = CASE
        WHEN ISNULL(l.released_reason, '') = '' THEN {1}
        ELSE l.released_reason
    END
FROM dbo.DG_AssetLock_tb l
INNER JOIN dbo.DG_Auction_tb a ON a.id = l.auction_id
WHERE ISNULL(l.trang_thai, '') = 'locked'
  AND ISNULL(a.is_deleted, 0) = 0
  AND a.trang_thai IN ({2}, {3}, {4}, {5})
", AhaTime_cl.Now,
   NormalizeText(reason, "Giải phóng khóa tài sản theo trạng thái kết thúc."),
   DauGiaPolicy_cl.StatusCompleted,
   DauGiaPolicy_cl.StatusExpired,
   DauGiaPolicy_cl.StatusCancelled,
   DauGiaPolicy_cl.StatusSettlementFailed);
    }

    private static string BuildLedgerRef(string action, long auctionId)
    {
        string safeAction = NormalizeText(action, "unknown");
        string safeAuction = auctionId <= 0 ? "0" : auctionId.ToString();
        return "DG|" + safeAction + "|" + safeAuction + "|" + Guid.NewGuid().ToString("N");
    }

    private static RightsBalanceInfo GetRightsAccount(dbDataContext db, string account)
    {
        if (db == null)
            return null;

        string holder = NormalizeAccount(account);
        if (holder == "")
            return null;

        return db.ExecuteQuery<RightsBalanceInfo>(@"
SELECT TOP 1
    taikhoan AS Account,
    ISNULL(CAST(Vi1That_Evocher_30PhanTram AS FLOAT), 0) AS UuDai,
    ISNULL(CAST(DongA AS FLOAT), 0) AS TieuDung
FROM dbo.taikhoan_tb
WHERE taikhoan = {0}
", holder).FirstOrDefault();
    }

    private static string BuildRightsInsufficientMessage(double need, double uuDai, double tieuDung)
    {
        return "Quyền ưu đãi + quyền tiêu dùng không đủ, cần tối thiểu "
            + need.ToString("#,##0.00")
            + " Quyền. Hiện có: ưu đãi "
            + uuDai.ToString("#,##0.00")
            + ", tiêu dùng "
            + tieuDung.ToString("#,##0.00")
            + ".";
    }

    private static bool PersistRightsMutation(
        dbDataContext db,
        string account,
        double nextUuDaiBalance,
        double nextTieuDungBalance,
        double uuDaiJournalAmount,
        double tieuDungJournalAmount,
        bool congTru,
        string note,
        string refId,
        out string message)
    {
        message = "";
        if (db == null)
        {
            message = "Thiếu kết nối dữ liệu để cập nhật quyền.";
            return false;
        }

        string holder = NormalizeAccount(account);
        if (holder == "")
        {
            message = "Không xác định được tài khoản để cập nhật quyền.";
            return false;
        }

        double nextUuDai = Math.Round(Math.Max(0, nextUuDaiBalance), 2);
        double nextTieuDung = Math.Round(Math.Max(0, nextTieuDungBalance), 2);
        double uuDaiAmount = Math.Round(Math.Max(0, uuDaiJournalAmount), 2);
        double tieuDungAmount = Math.Round(Math.Max(0, tieuDungJournalAmount), 2);
        DateTime now = AhaTime_cl.Now;

        int updated = db.ExecuteQuery<int>(@"
DECLARE @updated INT;

UPDATE dbo.taikhoan_tb
SET Vi1That_Evocher_30PhanTram = {0},
    DongA = {1}
WHERE taikhoan = {2};

SET @updated = @@ROWCOUNT;

IF @updated > 0 AND {3} > 0
BEGIN
    INSERT INTO dbo.LichSu_DongA_tb
    (
        taikhoan, dongA, ngay, CongTru, id_donhang,
        ghichu, id_rutdiem, LoaiHoSo_Vi, KyHieu9ViCon_1_9
    )
    VALUES
    (
        {2}, {3}, {4}, {5}, '',
        {6}, {7}, 2, NULL
    );
END

IF @updated > 0 AND {8} > 0
BEGIN
    INSERT INTO dbo.LichSu_DongA_tb
    (
        taikhoan, dongA, ngay, CongTru, id_donhang,
        ghichu, id_rutdiem, LoaiHoSo_Vi, KyHieu9ViCon_1_9
    )
    VALUES
    (
        {2}, {8}, {4}, {5}, '',
        {9}, {7}, 1, NULL
    );
END

SELECT @updated;
", nextUuDai,
   nextTieuDung,
   holder,
   uuDaiAmount,
   now,
   congTru ? 1 : 0,
   NormalizeText(note, "Đấu giá") + " (Quyền ưu đãi)",
   NormalizeText(refId, ""),
   tieuDungAmount,
   NormalizeText(note, "Đấu giá") + " (Quyền tiêu dùng)").FirstOrDefault();

        if (updated <= 0)
        {
            message = "Không tìm thấy tài khoản để cập nhật số dư.";
            return false;
        }

        return true;
    }

    private static bool TryDebitRights(
        dbDataContext db,
        string account,
        double amount,
        string note,
        string refId,
        out string message,
        out RightsSplit split)
    {
        message = "";
        split = new RightsSplit();

        double value = Math.Round(Math.Max(0, amount), 2);
        if (value <= 0)
            return true;

        RightsBalanceInfo holder = GetRightsAccount(db, account);
        if (holder == null)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Tài khoản không tồn tại để trừ quyền tham gia đấu giá.");
            return false;
        }

        double uuDaiBalance = Math.Round(holder.UuDai, 2);
        double tieuDungBalance = Math.Round(holder.TieuDung, 2);
        double total = Math.Round(uuDaiBalance + tieuDungBalance, 2);
        if (total < value)
        {
            message = DauGiaNotify_cl.BuildWarningMessage(
                BuildRightsInsufficientMessage(value, uuDaiBalance, tieuDungBalance));
            return false;
        }

        double uuDaiDebit = Math.Round(Math.Min(uuDaiBalance, value), 2);
        double tieuDungDebit = Math.Round(value - uuDaiDebit, 2);
        double nextUuDaiBalance = Math.Round(Math.Max(0, uuDaiBalance - uuDaiDebit), 2);
        double nextTieuDungBalance = Math.Round(Math.Max(0, tieuDungBalance - tieuDungDebit), 2);

        split.UuDai = uuDaiDebit;
        split.TieuDung = tieuDungDebit;

        string persistMessage;
        if (!PersistRightsMutation(
            db,
            holder.Account,
            nextUuDaiBalance,
            nextTieuDungBalance,
            uuDaiDebit,
            tieuDungDebit,
            false,
            NormalizeText(note, "Trừ quyền đấu giá"),
            refId,
            out persistMessage))
        {
            message = DauGiaNotify_cl.BuildErrorMessage("Không thể cập nhật số dư quyền tham gia đấu giá: " + persistMessage);
            return false;
        }

        return true;
    }

    private static bool TryCreditRights(
        dbDataContext db,
        string account,
        double uuDaiAmount,
        double tieuDungAmount,
        string note,
        string refId,
        out string message)
    {
        message = "";
        double uuDai = Math.Round(Math.Max(0, uuDaiAmount), 2);
        double tieuDung = Math.Round(Math.Max(0, tieuDungAmount), 2);
        if (uuDai <= 0 && tieuDung <= 0)
            return true;

        RightsBalanceInfo holder = GetRightsAccount(db, account);
        if (holder == null)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Tài khoản không tồn tại để hoàn/cộng quyền.");
            return false;
        }

        double nextUuDaiBalance = Math.Round(holder.UuDai + uuDai, 2);
        double nextTieuDungBalance = Math.Round(holder.TieuDung + tieuDung, 2);

        string persistMessage;
        if (!PersistRightsMutation(
            db,
            holder.Account,
            nextUuDaiBalance,
            nextTieuDungBalance,
            uuDai,
            tieuDung,
            true,
            NormalizeText(note, "Cộng quyền đấu giá"),
            refId,
            out persistMessage))
        {
            message = DauGiaNotify_cl.BuildErrorMessage("Không thể cập nhật số dư quyền hoàn/cộng: " + persistMessage);
            return false;
        }

        return true;
    }

    private static void InsertLedger(
        dbDataContext db,
        long auctionId,
        string actorAccount,
        string contraAccount,
        string type,
        double amount,
        string note,
        string refId)
    {
        if (db == null)
            return;

        db.ExecuteCommand(@"
INSERT INTO dbo.DG_SoCai_tb
(
    auction_id, actor_account, contra_account, loai_giao_dich,
    so_tien, ghi_chu, ref_id, created_at
)
VALUES
(
    {0}, {1}, {2}, {3},
    {4}, {5}, {6}, {7}
)
", auctionId,
   NormalizeText(actorAccount, ""),
   NormalizeText(contraAccount, ""),
   NormalizeText(type, "unknown"),
   Math.Round(Math.Max(0, amount), 2),
   NormalizeText(note, ""),
   NormalizeText(refId, ""),
   AhaTime_cl.Now);
    }
}
