using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;

public partial class daugia_chi_tiet : Page
{
    protected long AuctionID;
    protected string ImageUrl = "/uploads/images/icon-img.png";
    protected string AuctionTitle = "";
    protected string AuctionDesc = "";
    protected string StatusLabel = "Không xác định";
    protected string CurrentPrice = "0.00";
    protected string BidFee = "0.00";
    protected string Seller = "-";
    protected string Winner = "-";
    protected string StartAt = "-";
    protected string EndAt = "-";

    private DauGiaService_cl.AuctionDetailInfo _auction;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!TryReadAuctionID())
        {
            ShowNotFound();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            DauGiaBootstrap_cl.EnsureSchemaSafe(db);
            if (!IsPostBack)
            {
                string automationMessage;
                DauGiaService_cl.RunAutoClose(db, out automationMessage);
            }

            BindDetail(db);
        }
    }

    protected void butBid_Click(object sender, EventArgs e)
    {
        if (!EnsureHomeLogin())
            return;

        string user = GetCurrentPortalUser();
        using (dbDataContext db = new dbDataContext())
        {
            double newPrice;
            string message;
            bool ok = DauGiaService_cl.PlaceBid(db, AuctionID, user, out newPrice, out message);
            ShowHomeNotice(message, ok ? "success" : "warning");
        }
    }

    protected void butReserve_Click(object sender, EventArgs e)
    {
        if (!EnsureHomeLogin())
            return;

        string user = GetCurrentPortalUser();
        using (dbDataContext db = new dbDataContext())
        {
            string message;
            bool ok = DauGiaService_cl.ReserveWinner(db, AuctionID, user, out message);
            ShowHomeNotice(message, ok ? "success" : "warning");
        }
    }

    protected void butBuyerConfirm_Click(object sender, EventArgs e)
    {
        if (!EnsureHomeLogin())
            return;

        string user = GetCurrentPortalUser();
        using (dbDataContext db = new dbDataContext())
        {
            string message;
            bool ok = DauGiaService_cl.BuyerConfirmPayment(db, AuctionID, user, out message);
            ShowHomeNotice(message, ok ? "success" : "warning");
        }
    }

    protected void butSellerConfirm_Click(object sender, EventArgs e)
    {
        if (!EnsureHomeLogin())
            return;

        string user = GetCurrentPortalUser();
        using (dbDataContext db = new dbDataContext())
        {
            string message;
            bool ok = DauGiaService_cl.SellerConfirmFulfillment(db, AuctionID, user, out message);
            ShowHomeNotice(message, ok ? "success" : "warning");
        }
    }

    private bool TryReadAuctionID()
    {
        long id;
        bool ok = long.TryParse((Request.QueryString["id"] ?? "").Trim(), out id);
        if (!ok || id <= 0)
            return false;

        AuctionID = id;
        return true;
    }

    private void BindDetail(dbDataContext db)
    {
        _auction = DauGiaService_cl.GetAuctionById(db, AuctionID);
        if (_auction == null)
        {
            ShowNotFound();
            return;
        }

        phNotFound.Visible = false;
        phDetail.Visible = true;

        AuctionTitle = (_auction.SnapshotTitle ?? "").Trim();
        AuctionDesc = (_auction.SnapshotDesc ?? "").Trim();
        StatusLabel = BuildStatusLabel(_auction.TrangThai);
        CurrentPrice = FormatPoint(_auction.GiaHienTai);
        BidFee = FormatPoint(_auction.PhiLuot);
        Seller = (_auction.SellerAccount ?? "").Trim() == "" ? "-" : _auction.SellerAccount;
        Winner = (_auction.WinnerAccount ?? "").Trim() == "" ? "-" : _auction.WinnerAccount;
        StartAt = FormatDate(_auction.PhienBatDau);
        EndAt = FormatDate(_auction.PhienKetThuc);

        string image = (_auction.SnapshotImage ?? "").Trim();
        if (image != "")
            ImageUrl = image;

        List<DauGiaService_cl.BidItem> bids = DauGiaService_cl.LoadBidHistory(db, AuctionID, 100);
        rptBids.DataSource = bids;
        rptBids.DataBind();
        phBidsEmpty.Visible = bids.Count == 0;

        ApplyActionVisibility();
    }

    private void ApplyActionVisibility()
    {
        string currentUser = GetCurrentPortalUser();
        string status = DauGiaPolicy_cl.NormalizeStatus(_auction == null ? "" : _auction.TrangThai);
        string seller = _auction == null ? "" : (_auction.SellerAccount ?? "").Trim().ToLowerInvariant();
        string winner = _auction == null ? "" : (_auction.WinnerAccount ?? "").Trim().ToLowerInvariant();

        butBid.Visible = status == DauGiaPolicy_cl.StatusLive
            && currentUser != ""
            && currentUser != seller;

        butReserve.Visible = status == DauGiaPolicy_cl.StatusLive
            && currentUser != ""
            && currentUser != seller;

        butBuyerConfirm.Visible = status == DauGiaPolicy_cl.StatusReserved
            && currentUser != ""
            && currentUser == winner;

        butSellerConfirm.Visible = status == DauGiaPolicy_cl.StatusBuyerConfirmed
            && currentUser != ""
            && currentUser == seller;
    }

    private void ShowNotFound()
    {
        phNotFound.Visible = true;
        phDetail.Visible = false;
    }

    private string GetCurrentPortalUser()
    {
        string portalUser = (PortalRequest_cl.GetCurrentAccount() ?? "").Trim().ToLowerInvariant();
        if (portalUser != "")
            return portalUser;

        string user = ((Session["user_home"] ?? "") + "").Trim().ToLowerInvariant();
        if (user != "")
            return user;

        string home = DecodeSessionAccount("taikhoan_home");
        if (home != "")
            return home;

        string homeCookie = DecodeCookieAccount("cookie_userinfo_home_bcorn");
        if (homeCookie != "")
            return homeCookie;

        string shop = DecodeSessionAccount("taikhoan_shop");
        if (shop != "")
            return shop;

        return DecodeCookieAccount("cookie_userinfo_shop_bcorn");
    }

    private string DecodeSessionAccount(string sessionKey)
    {
        string encrypted = Session[sessionKey] as string;
        if (string.IsNullOrWhiteSpace(encrypted))
            return "";

        try
        {
            return (mahoa_cl.giaima_Bcorn(encrypted) ?? "").Trim().ToLowerInvariant();
        }
        catch
        {
            return "";
        }
    }

    private string DecodeCookieAccount(string cookieName)
    {
        if (Request == null || Request.Cookies == null || string.IsNullOrWhiteSpace(cookieName))
            return "";

        HttpCookie cookie = Request.Cookies[cookieName];
        if (cookie == null)
            return "";

        string encrypted = (cookie["taikhoan"] ?? "").Trim();
        if (encrypted == "")
            return "";

        try
        {
            return (mahoa_cl.giaima_Bcorn(encrypted) ?? "").Trim().ToLowerInvariant();
        }
        catch
        {
            return "";
        }
    }

    private bool EnsureHomeLogin()
    {
        string currentUser = GetCurrentPortalUser();
        if (currentUser != "")
            return true;

        string returnUrl = Request.RawUrl ?? "/daugia";
        if (PortalRequest_cl.IsShopPortalRequest())
        {
            Session["ThongBao_Shop"] = string.Format("toast|{0}|{1}|{2}|{3}",
                "Thông báo",
                "Vui lòng đăng nhập để thao tác đấu giá.",
                "warning",
                "1200");
            Response.Redirect(check_login_cl.BuildShopLoginUrl(returnUrl), true);
            return false;
        }

        Session["thongbao_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Vui lòng đăng nhập để thao tác đấu giá.", "1200", "warning");
        Response.Redirect("/dang-nhap?return_url=" + HttpUtility.UrlEncode(returnUrl), true);
        return false;
    }

    private void ShowHomeNotice(string message, string cssClass)
    {
        string noticeText = string.IsNullOrWhiteSpace(message) ? "Đã xử lý thao tác." : message;
        string noticeType = string.IsNullOrWhiteSpace(cssClass) ? "info" : cssClass;

        if (PortalRequest_cl.IsShopPortalRequest())
        {
            Session["ThongBao_Shop"] = string.Format("toast|{0}|{1}|{2}|{3}",
                "Thông báo",
                noticeText,
                noticeType,
                "1800");
        }
        else
        {
            Session["thongbao_home"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                noticeText,
                "1800",
                noticeType);
        }
        Response.Redirect(Request.RawUrl, true);
    }

    protected string FormatDate(object dateObj)
    {
        if (dateObj == null)
            return "-";
        DateTime date;
        if (!DateTime.TryParse(dateObj.ToString(), out date))
            return "-";
        return date.ToString("dd/MM/yyyy HH:mm");
    }

    private string FormatDate(DateTime? date)
    {
        if (!date.HasValue)
            return "-";
        return date.Value.ToString("dd/MM/yyyy HH:mm");
    }

    protected string FormatPoint(object amountObj)
    {
        double amount = 0;
        double.TryParse(amountObj == null ? "0" : amountObj.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out amount);
        return amount.ToString("#,##0.00");
    }

    private string FormatPoint(double amount)
    {
        return amount.ToString("#,##0.00");
    }

    private string BuildStatusLabel(string statusRaw)
    {
        string status = DauGiaPolicy_cl.NormalizeStatus(statusRaw);
        switch (status)
        {
            case "pending_approval":
                return "Chờ admin duyệt";
            case "scheduled":
                return "Đã lịch";
            case "live":
                return "Đang diễn ra";
            case "reserved":
                return "Đã có người giữ mua";
            case "buyer_confirmed":
                return "Khách đã xác nhận";
            case "seller_confirmed":
                return "Shop đã xác nhận";
            case "completed":
                return "Đã hoàn tất";
            case "expired":
                return "Hết hạn";
            case "cancelled":
                return "Đã hủy";
            case "settlement_failed":
                return "Lỗi tất toán";
            default:
                return "Nháp";
        }
    }
}
