using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class daugia_quan_ly : Page
{
    protected int PendingCount;
    protected int LiveCount;
    protected int ReservedCount;
    protected int CompletedCount;

    private string _currentAccount = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);

        _currentAccount = (PortalRequest_cl.GetCurrentAccount() ?? "").Trim().ToLowerInvariant();
        if (_currentAccount == "")
        {
            string returnUrl = Request.RawUrl ?? "/home/dau-gia";
            if (PortalRequest_cl.IsShopPortalRequest())
            {
                Session["ThongBao_Shop"] = string.Format("toast|{0}|{1}|{2}|{3}",
                    "Thông báo",
                    "Vui lòng đăng nhập để quản lý đấu giá.",
                    "warning",
                    "1200");
                Response.Redirect(check_login_cl.BuildShopLoginUrl(returnUrl), true);
                return;
            }

            Session["thongbao_home"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                "Vui lòng đăng nhập để quản lý đấu giá.",
                "1200",
                "warning");
            Response.Redirect("/dang-nhap?return_url=" + HttpUtility.UrlEncode(returnUrl), true);
            return;
        }

        lnkCreateAuction.NavigateUrl = BuildCreateUrl();
        lnkManagePost.NavigateUrl = PortalRequest_cl.IsShopPortalRequest() ? "/shop/quan-ly-tin" : "/home/quan-ly-tin/default.aspx";

        using (dbDataContext db = new dbDataContext())
        {
            DauGiaBootstrap_cl.EnsureSchemaSafe(db);
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
        phEmpty.Visible = list.Count == 0;
    }

    private string BuildCreateUrl()
    {
        if (PortalRequest_cl.IsShopPortalRequest())
            return "/shop/dau-gia/tao";
        return "/home/dau-gia/tao";
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
