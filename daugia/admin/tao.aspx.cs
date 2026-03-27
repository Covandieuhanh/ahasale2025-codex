using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;

public partial class daugia_admin_tao : Page
{
    private string _currentAccount = "";
    protected string SellerUuDaiBalance = "0.00";
    protected string SellerTieuDungBalance = "0.00";
    protected string SellerTotalRights = "0.00";
    protected string DepositPercentText = "20";
    protected string DepositPreview = "0.00";
    protected string DepositPercentJs = "20";

    protected void Page_Load(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            DauGiaBootstrap_cl.EnsureSchemaSafe(db);
            _currentAccount = DauGiaPortalAccess_cl.EnsureSellerManagementAccess(this, db, "/daugia/admin/tao");
            if (_currentAccount == "")
                return;

            if (!IsPostBack)
            {
                BindDefaults();
                BindFromQuery();
            }

            BindRightsSnapshot(db, _currentAccount);
        }
    }

    protected void butCreate_Click(object sender, EventArgs e)
    {
        string seller = (_currentAccount ?? "").Trim().ToLowerInvariant();
        if (seller == "")
        {
            using (dbDataContext db = new dbDataContext())
            {
                DauGiaBootstrap_cl.EnsureSchemaSafe(db);
                seller = DauGiaPortalAccess_cl.EnsureSellerManagementAccess(this, db, "/daugia/admin/tao");
                if (seller == "")
                    return;
            }
        }

        double giaNiemYet;
        if (!TryParseDouble(txtGiaNiemYet.Text, out giaNiemYet))
        {
            ShowPortalNotice("Giá niêm yết không hợp lệ.", "warning");
            return;
        }

        double phiLuot;
        if (!TryParseDouble(txtPhiLuot.Text, out phiLuot))
        {
            ShowPortalNotice("Phí mỗi lượt không hợp lệ.", "warning");
            return;
        }

        DateTime startAt;
        if (!TryParseDateTime(txtStartAt.Text, out startAt))
        {
            ShowPortalNotice("Thời gian bắt đầu không hợp lệ.", "warning");
            return;
        }

        DateTime endAt;
        if (!TryParseDateTime(txtEndAt.Text, out endAt))
        {
            ShowPortalNotice("Thời gian kết thúc không hợp lệ.", "warning");
            return;
        }

        DauGiaService_cl.CreateAuctionRequest request = new DauGiaService_cl.CreateAuctionRequest();
        request.SellerAccount = seller;
        request.SellerScope = NormalizeText(ddlSellerScope.SelectedValue, "shop");
        request.SourceType = NormalizeText(ddlSourceType.SelectedValue, DauGiaPolicy_cl.SourceManualAsset);
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
                BindRightsSnapshot(db, seller);
                ShowPortalNotice(message, "warning", false);
                return;
            }

            ShowPortalNotice(message, "success", false);
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
        ddlSourceType.SelectedValue = DauGiaPolicy_cl.SourceManualAsset;
        ddlSettlementMode.SelectedValue = DauGiaPolicy_cl.SettlementManualFulfillment;
        ddlSellerScope.SelectedValue = PortalRequest_cl.IsShopPortalRequest() ? "shop" : "home";
    }

    private void BindRightsSnapshot(dbDataContext db, string sellerAccount)
    {
        taikhoan_tb account = db == null
            ? null
            : db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == sellerAccount);

        double uuDai = account == null ? 0 : Math.Round((double)(account.Vi1That_Evocher_30PhanTram ?? 0m), 2);
        double tieuDung = account == null ? 0 : Math.Round((double)(account.DongA ?? 0m), 2);
        double tong = Math.Round(uuDai + tieuDung, 2);
        double depositPercent = LoadDepositPercent(db);
        double giaNiemYet = 0;
        TryParseDouble(txtGiaNiemYet.Text, out giaNiemYet);
        double depositPreview = Math.Round(Math.Max(0, giaNiemYet) * depositPercent / 100d, 2);

        SellerUuDaiBalance = uuDai.ToString("#,##0.00");
        SellerTieuDungBalance = tieuDung.ToString("#,##0.00");
        SellerTotalRights = tong.ToString("#,##0.00");
        DepositPercentText = depositPercent.ToString("#,##0.##");
        DepositPreview = depositPreview.ToString("#,##0.00");
        DepositPercentJs = depositPercent.ToString(CultureInfo.InvariantCulture);
    }

    private double LoadDepositPercent(dbDataContext db)
    {
        if (db == null)
            return 20d;

        string raw = db.ExecuteQuery<string>(
            "SELECT TOP 1 config_value FROM dbo.DG_Config_tb WHERE config_key = {0}",
            "deposit_percent").FirstOrDefault();

        double parsed;
        if (double.TryParse((raw ?? "").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out parsed))
            return parsed;
        if (double.TryParse((raw ?? "").Trim(), NumberStyles.Any, CultureInfo.CurrentCulture, out parsed))
            return parsed;
        return 20d;
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

    private void ShowPortalNotice(string message, string cssClass, bool redirectCurrent = true)
    {
        string normalizedMessage = string.IsNullOrWhiteSpace(message) ? "Đã xử lý thao tác." : message;
        string normalizedCss = string.IsNullOrWhiteSpace(cssClass) ? "info" : cssClass;

        if (!redirectCurrent)
        {
            Helper_Tabler_cl.ShowToast(this, normalizedMessage, normalizedCss, true, 2200, "Thông báo");
            if (PortalRequest_cl.IsShopPortalRequest())
            {
                string toastType = normalizedCss == "success" ? "success" : (normalizedCss == "warning" ? "warning" : "info");
                Session["ThongBao_Shop"] = string.Format("toast|{0}|{1}|{2}|{3}",
                    "Thông báo",
                    normalizedMessage.Replace("|", "/"),
                    toastType,
                    "1800");
            }
            return;
        }

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

        if (redirectCurrent)
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
