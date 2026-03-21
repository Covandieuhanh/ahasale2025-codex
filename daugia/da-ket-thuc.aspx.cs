using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;

public partial class daugia_da_ket_thuc : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            DauGiaBootstrap_cl.EnsureSchemaSafe(db);
            if (!IsPostBack)
            {
                List<DauGiaService_cl.AuctionCardItem> ended = DauGiaService_cl.LoadEndedAuctions(db, 120);
                rptEnded.DataSource = ended;
                rptEnded.DataBind();
                phEmpty.Visible = ended.Count == 0;
            }
        }
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
            case "completed":
                return "Hoàn tất";
            case "expired":
                return "Hết hạn";
            case "cancelled":
                return "Đã hủy";
            case "settlement_failed":
                return "Lỗi tất toán";
            default:
                return status;
        }
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

    protected string FormatPoint(object amountObj)
    {
        double amount = 0;
        double.TryParse(amountObj == null ? "0" : amountObj.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out amount);
        return amount.ToString("#,##0.00");
    }
}
