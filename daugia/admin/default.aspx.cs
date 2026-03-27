using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class daugia_admin_default : Page
{
    protected DauGiaService_cl.AuctionSummaryInfo Summary = new DauGiaService_cl.AuctionSummaryInfo();
    private string CurrentHomeAccount = "";
    private string CurrentAdminAccount = "";
    private bool IsSystemAdminPortal = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        IsSystemAdminPortal = IsSystemAdminRequest();
        if (!IsSystemAdminPortal)
        {
            Response.Redirect("/daugia/admin/portal.aspx", true);
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            DauGiaBootstrap_cl.EnsureSchemaSafe(db);
            EnsurePortalGate(db);
            if (CurrentHomeAccount == "" && CurrentAdminAccount == "")
                return;

            if (IsSystemAdminPortal && !AdminManagementSpace_cl.EnsureSelectedSpaceRouteAccess(this))
                return;

            if (!IsPostBack)
            {
                BindStatusFilter();
                BindAdminState(db);
            }
        }
    }

    private bool IsSystemAdminRequest()
    {
        string raw = (Request.QueryString["system"] ?? "").Trim().ToLowerInvariant();
        return raw == "1" || raw == "true";
    }

    private void EnsurePortalGate(dbDataContext db)
    {
        if (IsSystemAdminPortal)
        {
            EnsureSystemAdminGate(db);
            return;
        }

        EnsureHomeGate(db);
    }

    private void EnsureHomeGate(dbDataContext db)
    {
        string homeUser = ReadHomeAccount();
        if (homeUser == "")
        {
            HomeSpaceAccess_cl.RedirectToHomeLogin(
                this,
                Request.RawUrl ?? "/daugia/admin/",
                "Vui lòng đăng nhập tài khoản Home để truy cập không gian /daugia/admin.");
            return;
        }

        taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == homeUser);
        if (account == null || !SpaceAccess_cl.CanAccessDauGia(db, account))
        {
            HomeSpaceAccess_cl.RedirectToAccessPage(this, ModuleSpace_cl.DauGia, Request.RawUrl ?? "/daugia/admin/");
            return;
        }

        CurrentHomeAccount = homeUser;
    }

    private void EnsureSystemAdminGate(dbDataContext db)
    {
        check_login_cl.check_login_admin("none", "none");
        string adminUser = ReadAdminAccount();
        if (adminUser == "")
            return;

        taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == adminUser);
        if (!DauGiaPolicy_cl.CanAccessAdmin(db, adminUser, account == null ? "" : (account.permission ?? "")))
        {
            Session["thongbao"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                "Bạn không có quyền truy cập quản trị đấu giá.",
                "1800",
                "warning");
            Response.Redirect("/admin/default.aspx", true);
            return;
        }

        CurrentAdminAccount = adminUser;
    }

    private string ReadHomeAccount()
    {
        string encrypted = "";
        if (Session != null)
            encrypted = Session["taikhoan_home"] as string;

        if (string.IsNullOrWhiteSpace(encrypted) && Request != null && Request.Cookies != null)
        {
            HttpCookie ck = Request.Cookies["cookie_userinfo_home_bcorn"];
            if (ck != null)
                encrypted = ck["taikhoan"] ?? "";
        }

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

    private string ReadAdminAccount()
    {
        string encrypted = "";
        if (Session != null)
            encrypted = Session["taikhoan"] as string;

        if (string.IsNullOrWhiteSpace(encrypted) && Request != null && Request.Cookies != null)
        {
            HttpCookie ck = Request.Cookies["cookie_userinfo_admin_bcorn"];
            if (ck != null)
                encrypted = ck["taikhoan"] ?? "";
        }

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
        ddlStatus.Items.Add(new ListItem("Đã hoàn tất", DauGiaPolicy_cl.StatusCompleted));
        ddlStatus.Items.Add(new ListItem("Lỗi tất toán", DauGiaPolicy_cl.StatusSettlementFailed));
        ddlStatus.Items.Add(new ListItem("Đã hủy", DauGiaPolicy_cl.StatusCancelled));
        ddlStatus.Items.Add(new ListItem("Hết hạn", DauGiaPolicy_cl.StatusExpired));
    }

    private void BindAdminState(dbDataContext db)
    {
        Summary = DauGiaService_cl.GetSummary(db);

        string status = (ddlStatus.SelectedValue ?? "").Trim();
        string keyword = (txtKeyword.Text ?? "").Trim();
        List<DauGiaService_cl.AuctionCardItem> list = DauGiaService_cl.LoadAdminAuctions(db, status, keyword, 300);

        rptAuctions.DataSource = list;
        rptAuctions.DataBind();
        rptAuctionCards.DataSource = list;
        rptAuctionCards.DataBind();
        phEmpty.Visible = list.Count == 0;
    }

    protected void butSearch_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            DauGiaBootstrap_cl.EnsureSchemaSafe(db);
            BindAdminState(db);
        }
    }

    protected void butActivateScheduled_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            DauGiaBootstrap_cl.EnsureSchemaSafe(db);
            string message;
            DauGiaService_cl.ActivateScheduled(db, out message);
            ShowAdminNotice(message, "success");
        }
    }

    protected void butRunAuto_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            DauGiaBootstrap_cl.EnsureSchemaSafe(db);
            string message;
            DauGiaService_cl.RunAutoClose(db, out message);
            ShowAdminNotice(message, "success");
        }
    }

    protected void rptAuctions_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        long id;
        if (!long.TryParse((e.CommandArgument ?? "").ToString(), out id) || id <= 0)
            return;

        string actor = IsSystemAdminPortal ? CurrentAdminAccount : CurrentHomeAccount;
        if (string.IsNullOrWhiteSpace(actor))
            actor = IsSystemAdminPortal ? ReadAdminAccount() : ReadHomeAccount();
        using (dbDataContext db = new dbDataContext())
        {
            DauGiaBootstrap_cl.EnsureSchemaSafe(db);
            EnsurePortalGate(db);
            if (CurrentHomeAccount == "" && CurrentAdminAccount == "")
                return;

            if (string.IsNullOrWhiteSpace(actor))
                actor = IsSystemAdminPortal ? CurrentAdminAccount : CurrentHomeAccount;

            string message;
            bool ok = false;
            string cssClass = "warning";
            switch ((e.CommandName ?? "").Trim().ToLowerInvariant())
            {
                case "approve":
                    ok = DauGiaService_cl.ApproveAuction(db, id, actor, false, out message);
                    break;
                case "reject":
                    ok = DauGiaService_cl.RejectAuction(db, id, actor, "Admin từ chối phiên đấu giá.", out message);
                    break;
                case "settle":
                    ok = DauGiaService_cl.AdminSettle(db, id, actor, out message);
                    break;
                default:
                    return;
            }

            if (ok)
                cssClass = "success";

            ShowAdminNotice(message, cssClass);
        }
    }

    private void ShowAdminNotice(string message, string cssClass)
    {
        string title = "Thông báo";
        string body = string.IsNullOrWhiteSpace(message) ? "Thao tác đã thực hiện." : message;
        string type = string.IsNullOrWhiteSpace(cssClass) ? "info" : cssClass;
        if (IsSystemAdminPortal)
        {
            Session["thongbao"] = thongbao_class.metro_notifi_onload(title, body, "1800", type);
        }
        else
        {
            Session["notifi_home"] = thongbao_class.metro_notifi_onload(title, body, "1800", type);
        }
        Response.Redirect(Request.RawUrl, true);
    }

    protected string FormatPoint(object amountObj)
    {
        double amount = 0;
        double.TryParse(amountObj == null ? "0" : amountObj.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out amount);
        return amount.ToString("#,##0.00") + " Quyền";
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

    protected string BuildAuctionUrl(object slugObj, object idObj)
    {
        string slug = (slugObj == null ? "" : slugObj.ToString()).Trim().ToLowerInvariant();
        if (slug == "")
            slug = "dau-gia";
        long id;
        long.TryParse(idObj == null ? "0" : idObj.ToString(), out id);
        return "/daugia/" + slug + "-" + id + ".html";
    }

    protected bool CanShowApprove(object statusObj)
    {
        string status = DauGiaPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        return status == DauGiaPolicy_cl.StatusPendingApproval || status == DauGiaPolicy_cl.StatusScheduled;
    }

    protected bool CanShowReject(object statusObj)
    {
        string status = DauGiaPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        return status == DauGiaPolicy_cl.StatusPendingApproval || status == DauGiaPolicy_cl.StatusScheduled;
    }

    protected bool CanShowSettle(object statusObj)
    {
        string status = DauGiaPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        return status == DauGiaPolicy_cl.StatusSellerConfirmed;
    }
}
