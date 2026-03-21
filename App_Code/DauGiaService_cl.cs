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
        public double ViDauGia { get; set; }
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
    ISNULL(vi_dau_gia, 0) AS ViDauGia,
    ISNULL(so_luot_bid, 0) AS SoLuotBid,
    phien_bat_dau AS PhienBatDau,
    phien_ket_thuc AS PhienKetThuc,
    winner_reserved_at AS WinnerReservedAt,
    buyer_confirmed_at AS BuyerConfirmedAt,
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
        if (request.StartAt < now.AddMinutes(startBufferMinutes))
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Thời gian bắt đầu phải cách hiện tại tối thiểu " + startBufferMinutes + " phút.");
            return false;
        }

        if (request.EndAt <= request.StartAt)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
            return false;
        }

        double depositPercent = GetConfigDouble(db, "deposit_percent", 20d);
        double deposit = Math.Round(request.GiaNiemYet * depositPercent / 100d, 2);
        bool walletDebited = false;
        try
        {
            if (!TryDebitWallet(db, seller, deposit, "Đối ứng đấu giá mới.", out message))
                return false;
            walletDebited = deposit > 0;

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
    gia_niemyet, gia_hien_tai, phi_luot, tien_dat_coc, vi_dau_gia, so_luot_bid,
    trang_thai, phien_bat_dau, phien_ket_thuc, settlement_mode, da_hoan_coc, is_deleted,
    created_at, updated_at, created_by, updated_by
)
VALUES
(
    {0}, {1}, {2}, {3}, {4},
    {5}, {6}, {7}, {8},
    {9}, {9}, {10}, {11}, 0, 0,
    {12}, {13}, {14}, {15}, 0, 0,
    {16}, {16}, {17}, {17}
);
SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
", slug,
   NormalizeText(request.SourceType, "shop_post"),
   NormalizeText(request.SourceID, ""),
   seller,
   sellerScope,
   title,
   NormalizeText(request.Description, ""),
   NormalizeText(request.Image, ""),
   NormalizeText(request.SnapshotMeta, ""),
   request.GiaNiemYet,
   request.PhiLuot,
   deposit,
   DauGiaPolicy_cl.StatusPendingApproval,
   request.StartAt,
   request.EndAt,
   settlementMode,
   now,
   NormalizeText(request.CreatedBy, seller)).FirstOrDefault();

            InsertLedger(db, auctionId, seller, "admin", "deposit_hold", deposit, "Giữ cọc tạo phiên đấu giá.", "");
            message = DauGiaNotify_cl.BuildSuccessMessage("Đã tạo phiên đấu giá, đang chờ admin duyệt.");
            return true;
        }
        catch (Exception ex)
        {
            if (walletDebited)
            {
                TryCreditWallet(seller, deposit, "Hoàn cọc do lỗi tạo phiên đấu giá.");
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

        db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    approved_by = {1},
    approved_at = {2},
    rejected_reason = '',
    updated_at = {2},
    updated_by = {1}
WHERE id = {3}
", nextStatus, NormalizeText(adminUser, "admin"), now, auctionId);

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
        db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    rejected_reason = {1},
    updated_at = {2},
    updated_by = {3}
WHERE id = {4}
", DauGiaPolicy_cl.StatusCancelled, rejectReason, now, NormalizeText(adminUser, "admin"), auctionId);

        if (!row.DaHoanCoc && row.TienDatCoc > 0)
        {
            TryCreditWallet(row.SellerAccount, row.TienDatCoc, "Hoàn cọc do phiên đấu giá bị từ chối.");
            db.ExecuteCommand("UPDATE dbo.DG_Auction_tb SET da_hoan_coc = 1 WHERE id = {0}", auctionId);
            InsertLedger(db, auctionId, "admin", row.SellerAccount, "deposit_refund", row.TienDatCoc, "Hoàn cọc do từ chối duyệt.", "");
        }

        DauGiaNotify_cl.PushInAppNotice(
            db,
            NormalizeText(adminUser, "admin"),
            row.SellerAccount,
            BuildAuctionUrl(row.Slug, row.ID),
            "Phiên đấu giá #" + row.ID + " đã bị từ chối. Lý do: " + rejectReason);

        message = DauGiaNotify_cl.BuildSuccessMessage("Đã từ chối phiên đấu giá #" + row.ID + ".");
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
    winner_reserved_at AS WinnerReservedAt,
    buyer_confirmed_at AS BuyerConfirmedAt,
    CAST(ISNULL(da_hoan_coc, 0) AS BIT) AS DaHoanCoc
FROM dbo.DG_Auction_tb
WHERE trang_thai IN ({0}, {1}, {2})
  AND winner_reserved_at IS NOT NULL
  AND ISNULL(is_deleted, 0) = 0
", DauGiaPolicy_cl.StatusReserved, DauGiaPolicy_cl.StatusBuyerConfirmed, DauGiaPolicy_cl.StatusSellerConfirmed).ToList();

        int settlementFailedCount = 0;
        foreach (TimeoutAuctionInfo row in timeoutRows)
        {
            DateTime reserveAt = row.WinnerReservedAt.HasValue ? row.WinnerReservedAt.Value : now;
            DateTime deadline = reserveAt.AddMinutes(reserveTimeoutMinutes);
            if (deadline >= now)
                continue;

            if (row.BuyerConfirmedAt.HasValue && row.GiaHienTai > 0 && !string.IsNullOrWhiteSpace(row.WinnerAccount))
            {
                TryCreditWallet(row.WinnerAccount, row.GiaHienTai, "Hoàn tiền do phiên đấu giá quá hạn xác nhận.");
                InsertLedger(db, row.ID, "admin", row.WinnerAccount, "buyer_refund_timeout", row.GiaHienTai, "Hoàn tiền cho người mua khi quá hạn.", "");
            }

            if (!row.DaHoanCoc && row.TienDatCoc > 0 && !string.IsNullOrWhiteSpace(row.SellerAccount))
            {
                TryCreditWallet(row.SellerAccount, row.TienDatCoc, "Hoàn cọc do phiên đấu giá quá hạn xử lý.");
                InsertLedger(db, row.ID, "admin", row.SellerAccount, "deposit_refund_timeout", row.TienDatCoc, "Hoàn cọc cho chủ phiên khi quá hạn.", "");
            }

            db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    da_hoan_coc = 1,
    updated_at = {1},
    updated_by = 'system'
WHERE id = {2}
", DauGiaPolicy_cl.StatusSettlementFailed, now, row.ID);

            settlementFailedCount++;
        }

        message = "Tự động xử lý: mở " + activated + ", hết hạn live " + expired + ", quá hạn xác nhận " + settlementFailedCount + ".";
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

        if (!TryDebitWallet(db, bidder, fee, "Thanh toán phí lượt đấu giá #" + row.ID + ".", out message))
            return false;

        double before = row.GiaHienTai;
        double after = Math.Round(Math.Max(0, before - fee), 2);

        db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET gia_hien_tai = {0},
    vi_dau_gia = ISNULL(vi_dau_gia, 0) + {1},
    so_luot_bid = ISNULL(so_luot_bid, 0) + 1,
    updated_at = {2},
    updated_by = {3}
WHERE id = {4}
", after, fee, now, bidder, row.ID);

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
        message = DauGiaNotify_cl.BuildSuccessMessage("Đấu giá thành công. Giá hiện tại còn " + after.ToString("#,##0.00") + " E-AHA.");
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

        db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET winner_account = {0},
    winner_reserved_at = {1},
    trang_thai = {2},
    updated_at = {1},
    updated_by = {0}
WHERE id = {3}
  AND (winner_account IS NULL OR winner_account = '')
", buyer, now, DauGiaPolicy_cl.StatusReserved, row.ID);

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
        if (!TryDebitWallet(db, buyer, payAmount, "Thanh toán giá chốt đấu giá #" + row.ID + ".", out message))
            return false;

        DateTime now = AhaTime_cl.Now;
        db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    buyer_confirmed_at = {1},
    updated_at = {1},
    updated_by = {2}
WHERE id = {3}
", DauGiaPolicy_cl.StatusBuyerConfirmed, now, buyer, row.ID);

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
        db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    seller_confirmed_at = {1},
    updated_at = {1},
    updated_by = {2}
WHERE id = {3}
", DauGiaPolicy_cl.StatusSellerConfirmed, now, seller, row.ID);

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

        // Giữ nguyên công thức bản cũ: payout cố định cho shop = giá niêm yết - cọc.
        double sellerPayout = Math.Round(Math.Max(0, row.GiaNiemYet - row.TienDatCoc), 2);
        if (!TryCreditWallet(row.SellerAccount, sellerPayout, "Tất toán đấu giá #" + row.ID + "."))
        {
            message = DauGiaNotify_cl.BuildErrorMessage("Không thể cộng ví cho chủ phiên.");
            return false;
        }

        DateTime now = AhaTime_cl.Now;
        db.ExecuteCommand(@"
UPDATE dbo.DG_Auction_tb
SET trang_thai = {0},
    admin_settled_at = {1},
    updated_at = {1},
    updated_by = {2}
WHERE id = {3}
", DauGiaPolicy_cl.StatusCompleted, now, NormalizeText(adminUser, "admin"), row.ID);

        InsertLedger(db, row.ID, "admin", row.SellerAccount, "seller_payout_credit", sellerPayout, "Tất toán hoàn thành cho chủ phiên.", "");

        if (!string.IsNullOrWhiteSpace(row.WinnerAccount))
        {
            DauGiaNotify_cl.PushInAppNotice(
                db,
                NormalizeText(adminUser, "admin"),
                row.WinnerAccount,
                BuildAuctionUrl(row.Slug, row.ID),
                "Admin đã xác nhận hoàn tất phiên đấu giá #" + row.ID + ".");
        }

        DauGiaNotify_cl.PushInAppNotice(
            db,
            NormalizeText(adminUser, "admin"),
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

    private static bool HasWalletAccount(dbDataContext db, string account)
    {
        if (db == null || string.IsNullOrWhiteSpace(account))
            return false;

        int count = db.ExecuteQuery<int>(
            "SELECT COUNT(1) FROM dbo.bspa_data_khachhang_table WHERE sdt = {0}",
            NormalizeAccount(account)).FirstOrDefault();
        return count > 0;
    }

    private static double GetWalletBalance(dbDataContext db, string account)
    {
        if (!HasWalletAccount(db, account))
            return 0;

        double? balance = db.ExecuteQuery<double?>(
            "SELECT TOP 1 sodiem_e_aha FROM dbo.bspa_data_khachhang_table WHERE sdt = {0}",
            NormalizeAccount(account)).FirstOrDefault();

        return balance.HasValue ? balance.Value : 0;
    }

    private static bool TryDebitWallet(dbDataContext db, string account, double amount, string note, out string message)
    {
        message = "";
        double value = Math.Round(Math.Max(0, amount), 2);
        if (value <= 0)
            return true;

        string holder = NormalizeAccount(account);
        if (!HasWalletAccount(db, holder))
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Tài khoản ví không tồn tại trong hệ thống.");
            return false;
        }

        double balance = GetWalletBalance(db, holder);
        if (balance < value)
        {
            message = DauGiaNotify_cl.BuildWarningMessage("Ví E-AHA không đủ, cần tối thiểu " + value.ToString("#,##0.00") + " E-AHA.");
            return false;
        }

        data_khachhang_class wallet = new data_khachhang_class();
        wallet.giamdiem_eaha(holder, "admin", value, note ?? "Trừ ví đấu giá.");
        return true;
    }

    private static bool TryCreditWallet(string account, double amount, string note)
    {
        string holder = NormalizeAccount(account);
        double value = Math.Round(Math.Max(0, amount), 2);
        if (holder == "" || value <= 0)
            return true;

        try
        {
            data_khachhang_class wallet = new data_khachhang_class();
            wallet.tangdiem_eaha(holder, "admin", value, note ?? "Cộng ví đấu giá.");
            return true;
        }
        catch
        {
            return false;
        }
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
