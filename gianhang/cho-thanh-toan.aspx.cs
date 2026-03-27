using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class gianhang_cho_thanh_toan : System.Web.UI.Page
{
    private void ApplyExchangeResult(GianHangCheckoutCommand_cl.ExchangeResult result)
    {
        GianHangCheckoutPortal_cl.ApplyExchangeResult(this.Page, Session, result);
    }

    private string SellerDonBanUrl()
    {
        return GianHangCheckoutPortal_cl.SellerDonBanUrl();
    }

    private string ChoThanhToanUrl()
    {
        return GianHangCheckoutPortal_cl.ChoThanhToanUrl();
    }

    private string LoginUrl()
    {
        return GianHangCheckoutPortal_cl.LoginUrl();
    }

    private void EnsureStableFormAction()
    {
        if (form1 == null) return;
        form1.Action = ChoThanhToanUrl();
    }

    private string GetCurrentSellerAccount()
    {
        RootAccount_cl.RootAccountInfo info = GianHangBootstrap_cl.GetCurrentInfo();
        if (info == null || !info.IsAuthenticated)
            return string.Empty;

        return (info.AccountKey ?? string.Empty).Trim();
    }

    private void ConfigureWaitActions(string orderId)
    {
        lnk_refresh_wait.NavigateUrl = ChoThanhToanUrl();
    }

    private void SetCardNotice(string htmlMessage)
    {
        bool hasMessage = !string.IsNullOrEmpty(htmlMessage);
        lb_thongbao_the.Visible = hasMessage;
        lb_thongbao_the.Text = hasMessage ? htmlMessage : string.Empty;
        box_alert_the.Visible = hasMessage;
    }

    private void ApplyCheckoutPageState(GianHangCheckoutView_cl.CheckoutPageState state)
    {
        if (state == null)
            return;

        ViewState["id_donhang"] = state.OrderId ?? string.Empty;
        Label4.Text = state.OrderId ?? string.Empty;
        ConfigureWaitActions(state.OrderId ?? string.Empty);

        lb_order_items.Text = state.OrderItemCountText ?? "0";
        Repeater2.DataSource = state.OrderDetails ?? new List<GianHangOrderDetail_cl.OrderDetailRow>();
        Repeater2.DataBind();

        PlaceHolder2.Visible = state.ShowWaitingPlaceholder;
        PlaceHolder1.Visible = state.ShowPaymentPlaceholder;

        Label1.Text = state.WaitAmountText ?? string.Empty;
        Label2.Text = state.ConfirmAmountText ?? string.Empty;
        Label3.Text = state.BuyerDisplayName ?? string.Empty;
        lb_tengianhang.Text = string.IsNullOrWhiteSpace(state.SellerDisplayName) ? "Gian hàng" : state.SellerDisplayName;

        txt_mapin.Enabled = state.PinEnabled;
        Button3.Enabled = state.ConfirmEnabled;
        Timer1.Enabled = state.PollEnabled;
        SetCardNotice(state.CardNoticeHtml ?? string.Empty);
    }

    // Quy đổi VNĐ -> Quyền (làm tròn lên 2 chữ số)
    private static decimal QuyDoi_VND_To_Quyen(decimal vnd)
    {
        return GianHangCheckoutCore_cl.ConvertVndToRights(vnd);
    }

    // Helper cho aspx binding
    protected string FormatQuyen(object vndObj)
    {
        if (vndObj == null) return "0";
        decimal vnd = 0m;
        try { vnd = Convert.ToDecimal(vndObj); } catch { vnd = 0m; }
        decimal q = QuyDoi_VND_To_Quyen(vnd);
        return q.ToString("#,##0.##");
    }

    protected override void OnInit(EventArgs e)
    {
        // Ràng buộc ViewState theo session để giảm rủi ro CSRF với postback trao đổi.
        if (Context != null && Session != null)
        {
            ViewStateUserKey = Session.SessionID;
        }
        base.OnInit(e);
    }

    private void ClearPaymentSession()
    {
        GianHangCheckoutPortal_cl.ClearPaymentSession(Session);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
        Response.Cache.SetNoStore();
        Response.Cache.SetRevalidation(System.Web.HttpCacheRevalidation.AllCaches);
        EnsureStableFormAction();

        ph_gianhang_home.Visible = true;
        if (GianHangContext_cl.EnsureCurrentAccess(this) == null)
            return;
        using (dbDataContext db = new dbDataContext())
        {
            GianHangCheckoutPage_cl.LoadResult loadResult =
                GianHangCheckoutPage_cl.BuildLoadResult(
                    db,
                    Session,
                    GetCurrentSellerAccount(),
                    Request.QueryString["cancel_wait"],
                    Request.QueryString["id"],
                    LoginUrl(),
                    ChoThanhToanUrl(),
                    SellerDonBanUrl(),
                    GianHangCheckoutPortal_cl.LinkContextTtlMinutes);
            if (loadResult != null && loadResult.StopRequest)
            {
                if (loadResult.ClearPaymentSession)
                    ClearPaymentSession();
                if (loadResult.UseOnloadDialog)
                    GianHangCheckoutPortal_cl.SetDialogOnload(Session, loadResult.DialogTitle, loadResult.DialogMessage, loadResult.DialogType);
                if (loadResult.ShouldRedirect)
                    Response.Redirect(loadResult.RedirectUrl, false);
                if (Context != null && Context.ApplicationInstance != null)
                    Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                ApplyCheckoutPageState(loadResult == null ? null : loadResult.PageState);
            }
        }
    }

    public void ReloadCheckoutState(dbDataContext db)
    {
        RootAccount_cl.RootAccountInfo info = GianHangContext_cl.EnsureCurrentAccess(this);
        if (info == null)
        {
            Response.Redirect(SellerDonBanUrl());
            return;
        }

        GianHangCheckoutPage_cl.LoadResult loadResult =
            GianHangCheckoutPage_cl.BuildLoadResult(
                db,
                Session,
                info.AccountKey,
                string.Empty,
                string.Empty,
                LoginUrl(),
                ChoThanhToanUrl(),
                SellerDonBanUrl(),
                GianHangCheckoutPortal_cl.LinkContextTtlMinutes);
        if (loadResult == null || loadResult.StopRequest || loadResult.PageState == null)
        {
            Response.Redirect(SellerDonBanUrl());
            return;
        }

        ApplyCheckoutPageState(loadResult.PageState);
    }

    protected void Timer1_Tick(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            ReloadCheckoutState(db);
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string orderId = ViewState["id_donhang"] != null ? ViewState["id_donhang"].ToString() : string.Empty;
            if (!string.IsNullOrWhiteSpace(orderId))
            {
                GianHangCheckoutInteraction_cl.CancelWaitActionResult result =
                    GianHangCheckoutInteraction_cl.CancelCurrentWait(
                        db,
                        GetCurrentSellerAccount(),
                        orderId,
                        SellerDonBanUrl());
                if (result != null && result.Success)
                    ClearPaymentSession();
            }
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "redirectScript",
                "window.location.href='" + SellerDonBanUrl() + "';", true);
        }
    }

    protected void btn_huy_cho_Click(object sender, EventArgs e)
    {
        if (GianHangContext_cl.EnsureCurrentAccess(this) == null)
            return;

        string sellerAccount = GetCurrentSellerAccount();
        if (string.IsNullOrEmpty(sellerAccount))
        {
            Response.Redirect(LoginUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        string requestOrderId = ViewState["id_donhang"] == null
            ? string.Empty
            : ViewState["id_donhang"].ToString();

        using (dbDataContext db = new dbDataContext())
        {
            GianHangCheckoutInteraction_cl.CancelWaitActionResult result =
                GianHangCheckoutInteraction_cl.CancelCurrentWait(
                    db,
                    sellerAccount,
                    requestOrderId,
                    SellerDonBanUrl());
            if (result == null || !result.Success)
            {
                GianHangCheckoutPortal_cl.ShowDialog(
                    this.Page,
                    result == null ? "Thông báo" : result.DialogTitle,
                    result == null ? "Có lỗi xảy ra. Vui lòng thử lại." : result.DialogMessage,
                    result == null ? "alert" : result.DialogType);
                return;
            }
        }

        ClearPaymentSession();
        Response.Redirect(SellerDonBanUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        ClearPaymentSession();

        Response.Redirect(ChoThanhToanUrl());
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        if (GianHangContext_cl.EnsureCurrentAccess(this) == null)
            return;

        string sellerAccount = GetCurrentSellerAccount();
        string orderId = ViewState["id_donhang"] == null ? string.Empty : ViewState["id_donhang"].ToString();
        using (dbDataContext db = new dbDataContext())
        {
            GianHangCheckoutCommand_cl.ExchangeResult result =
                GianHangCheckoutInteraction_cl.ExecuteExchange(
                    db,
                    Session,
                    sellerAccount,
                    orderId,
                    txt_mapin.Text,
                    SellerDonBanUrl(),
                    ChoThanhToanUrl(),
                    GianHangRoutes_cl.BuildBuyerOrdersUrl(),
                    LoginUrl(),
                    GianHangCheckoutPortal_cl.GetClientIp(),
                    GianHangCheckoutPortal_cl.LinkContextTtlMinutes);
            ApplyExchangeResult(result);
        }
    }
}
