using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class daugia_admin_quan_ly : Page
{
    protected int PendingCount;
    protected int LiveCount;
    protected int ReservedCount;
    protected int CompletedCount;

    private string _currentAccount = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            DauGiaBootstrap_cl.EnsureSchemaSafe(db);
            _currentAccount = DauGiaPortalAccess_cl.EnsureSellerManagementAccess(this, db, "/daugia/admin/quan-ly");
            if (_currentAccount == "")
                return;

            lnkCreateAuction.NavigateUrl = "/daugia/admin/tao";
            if (!IsPostBack)
            {
                BindStatusFilter();
                BindState(db);
            }
        }
    }

    protected void butSearch_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            DauGiaBootstrap_cl.EnsureSchemaSafe(db);
            BindState(db);
        }
    }

    protected void rptAuctions_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        long auctionId;
        if (!long.TryParse((e.CommandArgument ?? "").ToString(), out auctionId) || auctionId <= 0)
            return;

        string command = (e.CommandName ?? "").Trim().ToLowerInvariant();
        using (dbDataContext db = new dbDataContext())
        {
            DauGiaBootstrap_cl.EnsureSchemaSafe(db);

            string message;
            bool ok;
            if (command == "cancel")
            {
                ok = DauGiaService_cl.SellerCancelAuction(
                    db,
                    auctionId,
                    _currentAccount,
                    "Chủ phiên tự hủy trong khu quản lý đấu giá.",
                    out message);
            }
            else if (command == "seller_confirm")
            {
                ok = DauGiaService_cl.SellerConfirmFulfillment(db, auctionId, _currentAccount, out message);
            }
            else
            {
                return;
            }

            ShowOwnerNotice(message, ok ? "success" : "warning");
        }
    }

    private void BindStatusFilter()
    {
        ddlStatus.Items.Clear();
        ddlStatus.Items.Add(new ListItem("Tất cả", ""));
        ddlStatus.Items.Add(new ListItem("Chờ duyệt", DauGiaPolicy_cl.StatusPendingApproval));
        ddlStatus.Items.Add(new ListItem("Đã lịch", DauGiaPolicy_cl.StatusScheduled));
        ddlStatus.Items.Add(new ListItem("Đang diễn ra", DauGiaPolicy_cl.StatusLive));
        ddlStatus.Items.Add(new ListItem("Đã giữ mua", DauGiaPolicy_cl.StatusReserved));
        ddlStatus.Items.Add(new ListItem("Khách đã xác nhận", DauGiaPolicy_cl.StatusBuyerConfirmed));
        ddlStatus.Items.Add(new ListItem("Shop đã xác nhận", DauGiaPolicy_cl.StatusSellerConfirmed));
        ddlStatus.Items.Add(new ListItem("Hoàn tất", DauGiaPolicy_cl.StatusCompleted));
        ddlStatus.Items.Add(new ListItem("Hết hạn", DauGiaPolicy_cl.StatusExpired));
        ddlStatus.Items.Add(new ListItem("Đã hủy", DauGiaPolicy_cl.StatusCancelled));
        ddlStatus.Items.Add(new ListItem("Lỗi tất toán", DauGiaPolicy_cl.StatusSettlementFailed));
    }

    private void BindState(dbDataContext db)
    {
        List<DauGiaService_cl.AuctionCardItem> all = DauGiaService_cl.LoadSellerAuctions(db, _currentAccount, "", "", 500);
        PendingCount = all.Count(p => DauGiaPolicy_cl.NormalizeStatus(p.TrangThai) == DauGiaPolicy_cl.StatusPendingApproval);
        LiveCount = all.Count(p => DauGiaPolicy_cl.NormalizeStatus(p.TrangThai) == DauGiaPolicy_cl.StatusLive);
        ReservedCount = all.Count(p =>
            DauGiaPolicy_cl.NormalizeStatus(p.TrangThai) == DauGiaPolicy_cl.StatusReserved
            || DauGiaPolicy_cl.NormalizeStatus(p.TrangThai) == DauGiaPolicy_cl.StatusBuyerConfirmed
            || DauGiaPolicy_cl.NormalizeStatus(p.TrangThai) == DauGiaPolicy_cl.StatusSellerConfirmed);
        CompletedCount = all.Count(p => DauGiaPolicy_cl.NormalizeStatus(p.TrangThai) == DauGiaPolicy_cl.StatusCompleted);

        string status = (ddlStatus.SelectedValue ?? "").Trim();
        string keyword = (txtKeyword.Text ?? "").Trim();
        List<DauGiaService_cl.AuctionCardItem> list = DauGiaService_cl.LoadSellerAuctions(db, _currentAccount, status, keyword, 500);

        rptAuctions.DataSource = list;
        rptAuctions.DataBind();
        rptAuctionCards.DataSource = list;
        rptAuctionCards.DataBind();
        phEmpty.Visible = list.Count == 0;
    }

    protected string BuildAuctionUrl(object slugObj, object idObj)
    {
        string slug = (slugObj == null ? "" : slugObj.ToString()).Trim().ToLowerInvariant();
        if (slug == "")
            slug = "dau-gia";
        long id;
        long.TryParse(idObj == null ? "0" : idObj.ToString(), out id);
        return "/daugia/" + slug + "-" + id + ".html";
    }

    protected string BuildStatusLabel(object statusObj)
    {
        string status = DauGiaPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        switch (status)
        {
            case "pending_approval":
                return "Chờ duyệt";
            case "scheduled":
                return "Đã lịch";
            case "live":
                return "Đang diễn ra";
            case "reserved":
                return "Đã giữ mua";
            case "buyer_confirmed":
                return "Khách đã xác nhận";
            case "seller_confirmed":
                return "Shop đã xác nhận";
            case "completed":
                return "Hoàn tất";
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

    protected bool CanShowSellerCancel(object statusObj)
    {
        string status = DauGiaPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        return status == DauGiaPolicy_cl.StatusPendingApproval
            || status == DauGiaPolicy_cl.StatusScheduled;
    }

    protected bool CanShowSellerConfirm(object statusObj)
    {
        string status = DauGiaPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        return status == DauGiaPolicy_cl.StatusBuyerConfirmed;
    }

    protected string FormatPoint(object amountObj)
    {
        double amount = 0;
        double.TryParse(amountObj == null ? "0" : amountObj.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out amount);
        return amount.ToString("#,##0.00");
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

    private void ShowOwnerNotice(string message, string cssClass)
    {
        string normalizedMessage = string.IsNullOrWhiteSpace(message) ? "Đã xử lý thao tác." : message;
        string normalizedCss = string.IsNullOrWhiteSpace(cssClass) ? "info" : cssClass;
        Session["notifi_home"] = thongbao_class.metro_notifi_onload(
            "Thông báo",
            normalizedMessage,
            "1800",
            normalizedCss);

        if (PortalRequest_cl.IsShopPortalRequest())
        {
            string toastType = normalizedCss == "success" ? "success" : (normalizedCss == "warning" ? "warning" : "info");
            Session["ThongBao_Shop"] = string.Format("toast|{0}|{1}|{2}|{3}",
                "Thông báo",
                normalizedMessage.Replace("|", "/"),
                toastType,
                "1800");
        }

        Response.Redirect(Request.RawUrl, true);
    }
}
