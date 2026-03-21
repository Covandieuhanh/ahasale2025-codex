using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;

public partial class daugia_default : Page
{
    protected DauGiaService_cl.AuctionSummaryInfo Summary = new DauGiaService_cl.AuctionSummaryInfo();

    protected void Page_Load(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            DauGiaBootstrap_cl.EnsureSchemaSafe(db);
            if (!IsPostBack)
            {
                string automationMessage;
                DauGiaService_cl.RunAutoClose(db, out automationMessage);
                BindDefaultState(db);
            }
        }
    }

    private void BindDefaultState(dbDataContext db)
    {
        Summary = DauGiaService_cl.GetSummary(db);
        List<DauGiaService_cl.AuctionCardItem> liveList = DauGiaService_cl.LoadLiveAuctions(db, 18);
        rptLive.DataSource = liveList;
        rptLive.DataBind();
        phLiveEmpty.Visible = liveList.Count == 0;
    }

    protected string ResolveImage(object imageObj)
    {
        string image = (imageObj == null ? "" : imageObj.ToString()).Trim();
        if (image == "")
            return "/uploads/images/icon-img.png";
        return image;
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
            case "live":
                return "Đang diễn ra";
            case "scheduled":
                return "Đã lịch";
            case "pending_approval":
                return "Chờ duyệt";
            case "reserved":
                return "Đã có người giữ";
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

    protected string BuildStatusBadgeCss(object statusObj)
    {
        string status = DauGiaPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        if (status == DauGiaPolicy_cl.StatusLive)
            return "daugia-badge daugia-badge--live";
        if (status == DauGiaPolicy_cl.StatusScheduled)
            return "daugia-badge daugia-badge--scheduled";
        if (status == DauGiaPolicy_cl.StatusPendingApproval)
            return "daugia-badge daugia-badge--pending";
        return "daugia-badge daugia-badge--done";
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
}
