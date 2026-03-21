using System;
using System.Globalization;
using System.Web;
using System.Web.UI;

public partial class daugia_tao : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!EnsureHomeLogin())
            return;

        using (dbDataContext db = new dbDataContext())
        {
            DauGiaBootstrap_cl.EnsureSchemaSafe(db);
        }

        if (IsPostBack)
            return;

        BindDefaults();
        BindFromQuery();
    }

    protected void butCreate_Click(object sender, EventArgs e)
    {
        string seller = GetCurrentPortalUser();
        if (seller == "")
        {
            EnsureHomeLogin();
            return;
        }

        double giaNiemYet;
        if (!TryParseDouble(txtGiaNiemYet.Text, out giaNiemYet))
        {
            ShowHomeNotice("Giá niêm yết không hợp lệ.", "warning");
            return;
        }

        double phiLuot;
        if (!TryParseDouble(txtPhiLuot.Text, out phiLuot))
        {
            ShowHomeNotice("Phí mỗi lượt không hợp lệ.", "warning");
            return;
        }

        DateTime startAt;
        if (!TryParseDateTime(txtStartAt.Text, out startAt))
        {
            ShowHomeNotice("Thời gian bắt đầu không hợp lệ.", "warning");
            return;
        }

        DateTime endAt;
        if (!TryParseDateTime(txtEndAt.Text, out endAt))
        {
            ShowHomeNotice("Thời gian kết thúc không hợp lệ.", "warning");
            return;
        }

        DauGiaService_cl.CreateAuctionRequest request = new DauGiaService_cl.CreateAuctionRequest();
        request.SellerAccount = seller;
        request.SellerScope = NormalizeText(ddlSellerScope.SelectedValue, "shop");
        request.SourceType = NormalizeText(ddlSourceType.SelectedValue, "shop_post");
        request.SourceID = NormalizeText(txtSourceID.Text, "");
        request.Title = NormalizeText(txtTitle.Text, "");
        request.Description = NormalizeText(txtDescription.Text, "");
        request.Image = NormalizeText(txtImage.Text, "");
        request.SnapshotMeta = "";
        request.GiaNiemYet = giaNiemYet;
        request.PhiLuot = phiLuot;
        request.StartAt = startAt;
        request.EndAt = endAt;
        request.SettlementMode = NormalizeText(ddlSettlementMode.SelectedValue, DauGiaPolicy_cl.SettlementManualFulfillment);
        request.CreatedBy = seller;

        using (dbDataContext db = new dbDataContext())
        {
            long auctionId;
            string message;
            bool ok = DauGiaService_cl.TryCreateAuctionDraft(db, request, out auctionId, out message);
            if (!ok)
            {
                ShowHomeNotice(message, "warning");
                return;
            }

            Session["notifi_home"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                message,
                "1800",
                "success");
            Response.Redirect("/daugia/chi-tiet.aspx?id=" + auctionId, true);
        }
    }

    private void BindDefaults()
    {
        DateTime now = AhaTime_cl.Now;
        txtGiaNiemYet.Text = "100";
        txtPhiLuot.Text = "1";
        txtStartAt.Text = now.AddMinutes(10).ToString("yyyy-MM-ddTHH:mm");
        txtEndAt.Text = now.AddDays(1).ToString("yyyy-MM-ddTHH:mm");
        ddlSourceType.SelectedValue = "shop_post";
        ddlSettlementMode.SelectedValue = DauGiaPolicy_cl.SettlementManualFulfillment;
        ddlSellerScope.SelectedValue = PortalRequest_cl.IsShopPortalRequest() ? "shop" : "home";
    }

    private void BindFromQuery()
    {
        string sourceType = NormalizeText(Request.QueryString["source_type"], "");
        if (sourceType != "")
        {
            if (ddlSourceType.Items.FindByValue(sourceType) != null)
                ddlSourceType.SelectedValue = sourceType;
        }

        string sourceId = NormalizeText(Request.QueryString["source_id"], "");
        if (sourceId != "")
            txtSourceID.Text = sourceId;

        string sellerScope = NormalizeText(Request.QueryString["scope"], "");
        if (sellerScope != "")
        {
            if (ddlSellerScope.Items.FindByValue(sellerScope) != null)
                ddlSellerScope.SelectedValue = sellerScope;
        }

        string title = NormalizeText(Request.QueryString["title"], "");
        if (title != "")
            txtTitle.Text = title;
    }

    private bool TryParseDouble(string raw, out double value)
    {
        value = 0;
        string text = NormalizeText(raw, "");
        if (text == "")
            return false;

        if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            return true;
        if (double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out value))
            return true;
        return false;
    }

    private bool TryParseDateTime(string raw, out DateTime value)
    {
        value = DateTime.MinValue;
        string text = NormalizeText(raw, "");
        if (text == "")
            return false;

        if (DateTime.TryParseExact(text, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
            return true;
        if (DateTime.TryParse(text, out value))
            return true;
        return false;
    }

    private string GetCurrentPortalUser()
    {
        string user = ((Session["user_home"] ?? "") + "").Trim().ToLowerInvariant();
        if (user != "")
            return user;

        string home = DecodeSessionAccount("taikhoan_home");
        if (home != "")
            return home;

        return DecodeSessionAccount("taikhoan_shop");
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

    private bool EnsureHomeLogin()
    {
        string currentUser = GetCurrentPortalUser();
        if (currentUser != "")
            return true;

        string returnUrl = Request.RawUrl ?? "/daugia/tao";
        if (PortalRequest_cl.IsShopPortalRequest())
        {
            Session["ThongBao_Shop"] = string.Format("toast|{0}|{1}|{2}|{3}",
                "Thông báo",
                "Vui lòng đăng nhập để tạo phiên đấu giá.",
                "warning",
                "1200");
            Response.Redirect(check_login_cl.BuildShopLoginUrl(returnUrl), true);
            return false;
        }

        Session["thongbao_home"] = thongbao_class.metro_notifi_onload(
            "Thông báo",
            "Vui lòng đăng nhập để tạo phiên đấu giá.",
            "1200",
            "warning");
        Response.Redirect("/dang-nhap?return_url=" + HttpUtility.UrlEncode(returnUrl), true);
        return false;
    }

    private void ShowHomeNotice(string message, string cssClass)
    {
        Session["notifi_home"] = thongbao_class.metro_notifi_onload(
            "Thông báo",
            string.IsNullOrWhiteSpace(message) ? "Đã xử lý thao tác." : message,
            "1800",
            string.IsNullOrWhiteSpace(cssClass) ? "info" : cssClass);
        Response.Redirect(Request.RawUrl, true);
    }

    private string NormalizeText(string value, string fallback)
    {
        string normalized = (value ?? "").Trim();
        if (normalized == "")
            return fallback ?? "";
        return normalized;
    }
}
