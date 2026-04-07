using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class gianhang_don_ban : System.Web.UI.Page
{
    private const string CART_SESSION_KEY = "offline_pos_cart_gianhang";
    private const string FILTER_TYPE_KEY = "pos_filter_type_gianhang";
    private const string RESULT_LIMIT_KEY = "pos_result_limit_gianhang";
    private const string ORDER_STATUS_FILTER_KEY = "order_status_filter_gianhang";

    protected void Page_Load(object sender, EventArgs e)
    {
        EnsureAccess();
        ApplyPageMode();

        if (!IsPostBack && IsCommitCreateModeRequest())
        {
            HandleCommitCreateModeRequest();
            return;
        }

        if (!IsPostBack)
        {
            InitializePage();
            return;
        }

        if (IsCreateMode())
            HandlePassivePostbacks();
        else
            HandlePassiveOrderListPostbacks();
    }

    private bool IsCreateMode()
    {
        string raw = (Request.QueryString["taodon"] ?? "").Trim();
        return raw == "1" || raw.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    private bool IsCommitCreateModeRequest()
    {
        if (!IsCreateMode())
            return false;

        string raw = (Request.QueryString["commit"] ?? string.Empty).Trim();
        return raw == "1" || raw.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    private void ApplyPageMode()
    {
        bool isCreateMode = IsCreateMode();
        pn_taodon.Visible = isCreateMode;
        pn_donban.Visible = !isCreateMode;
        Page.Title = isCreateMode ? "Tạo đơn gian hàng" : "Đơn bán gian hàng";

        if (isCreateMode)
        {
            string backUrl = ResolveBackUrl();
            lnk_back_top.HRef = backUrl;
            lnk_back_bottom.HRef = backUrl;
            lnk_taodon.HRef = ResolveCommitCreateUrl();
        }
    }

    private void EnsureAccess()
    {
        RootAccount_cl.RootAccountInfo info = GianHangContext_cl.EnsureCurrentAccess(this);
        if (info == null)
            return;
        ViewState["taikhoan"] = info.AccountKey;
    }

    private void InitializePage()
    {
        if (IsCreateMode())
            InitializeCreateMode();
        else
            InitializeOrderListMode();
    }

    private void InitializeCreateMode()
    {
        SetTradeTypeFilter("all");
        ResetResultLimit();
        SyncTradeTypeDropdown();

        txt_sp_search.Text = "";
        BindCreateModeUI("");

        string initialId = (Request.QueryString["idsp"] ?? "").Trim();
        if (!string.IsNullOrEmpty(initialId))
        {
            AddProductToCartById(initialId, false);
            ApplyInitialQuantity(initialId);
            BindCreateModeUI("");
        }
    }

    private void InitializeOrderListMode()
    {
        SetOrderStatusFilter("all");
        SyncOrderStatusDropdown();
        txt_order_search.Text = "";
        BindOrderList();
    }

    private string CurrentAccountKey()
    {
        return ((ViewState["taikhoan"] ?? "").ToString() ?? "").Trim().ToLowerInvariant();
    }

    private string ResolveListUrl()
    {
        return GianHangRoutes_cl.BuildDonBanUrl();
    }

    private string ResolveCreateModeEntryUrl(string productId)
    {
        return GianHangPosWorkflow_cl.BuildCreateModeEntryUrl(productId, ResolveListUrl());
    }

    private List<GianHangPosCartSession_cl.CartItem> GetCart()
    {
        return GianHangPosCartSession_cl.GetCart(Session, CART_SESSION_KEY);
    }

    private static int ClampInt(int value, int min, int max)
    {
        return GianHangPosCartSession_cl.ClampInt(value, min, max);
    }

    private int ResolveInitialQuantity()
    {
        int qty = Number_cl.Check_Int((Request.QueryString["qty"] ?? "1").Trim());
        return ClampInt(qty, 1, 999);
    }

    private void ApplyInitialQuantity(string productId)
    {
        string idsp = (productId ?? "").Trim();
        if (string.IsNullOrEmpty(idsp))
            return;

        int qty = ResolveInitialQuantity();
        if (qty <= 1)
            return;

        List<GianHangPosCartSession_cl.CartItem> cart = GetCart();
        GianHangPosCartSession_cl.ApplyInitialQuantity(cart, idsp, qty);
        GianHangPosCartSession_cl.SaveCart(Session, CART_SESSION_KEY, cart);
    }

    private string ResolveBackUrl()
    {
        string raw = (Request.QueryString["return_url"] ?? "").Trim();
        if (string.IsNullOrEmpty(raw))
            return GianHangRoutes_cl.BuildDashboardUrl();

        string value = HttpUtility.UrlDecode(raw) ?? "";
        value = value.Trim();
        if (string.IsNullOrEmpty(value))
            return GianHangRoutes_cl.BuildDashboardUrl();
        if (!value.StartsWith("/", StringComparison.Ordinal))
            return GianHangRoutes_cl.BuildDashboardUrl();
        if (value.StartsWith("//", StringComparison.Ordinal))
            return GianHangRoutes_cl.BuildDashboardUrl();
        return value;
    }

    private string ResolveChoThanhToanUrl()
    {
        return GianHangCheckoutPortal_cl.ChoThanhToanUrl();
    }

    private string ResolveCommitCreateUrl()
    {
        Uri requestUrl = Request == null ? null : Request.Url;
        string path = requestUrl == null ? GianHangRoutes_cl.BuildDonBanUrl() : requestUrl.AbsolutePath;
        List<string> parts = new List<string>();

        string taodon = (Request.QueryString["taodon"] ?? string.Empty).Trim();
        if (!string.IsNullOrEmpty(taodon))
            parts.Add("taodon=" + HttpUtility.UrlEncode(taodon));

        string idsp = (Request.QueryString["idsp"] ?? string.Empty).Trim();
        if (!string.IsNullOrEmpty(idsp))
            parts.Add("idsp=" + HttpUtility.UrlEncode(idsp));

        string qty = (Request.QueryString["qty"] ?? string.Empty).Trim();
        if (!string.IsNullOrEmpty(qty))
            parts.Add("qty=" + HttpUtility.UrlEncode(qty));

        string returnUrl = (Request.QueryString["return_url"] ?? string.Empty).Trim();
        if (!string.IsNullOrEmpty(returnUrl))
            parts.Add("return_url=" + HttpUtility.UrlEncode(returnUrl));

        parts.Add("commit=1");
        return path + "?" + string.Join("&", parts.ToArray());
    }

    private void SetCreateFlowDialog(string message, string alertType)
    {
        Session["thongbao_home"] = thongbao_class.metro_dialog_onload(
            "Thông báo",
            message ?? string.Empty,
            "false", "false", "OK", string.IsNullOrWhiteSpace(alertType) ? "alert" : alertType, "");
    }

    private string ResolveCreateModeUrlWithoutCommit()
    {
        Uri requestUrl = Request == null ? null : Request.Url;
        string path = requestUrl == null ? GianHangRoutes_cl.BuildDonBanUrl() : requestUrl.AbsolutePath;
        List<string> parts = new List<string>();

        string taodon = (Request.QueryString["taodon"] ?? string.Empty).Trim();
        if (!string.IsNullOrEmpty(taodon))
            parts.Add("taodon=" + HttpUtility.UrlEncode(taodon));

        string idsp = (Request.QueryString["idsp"] ?? string.Empty).Trim();
        if (!string.IsNullOrEmpty(idsp))
            parts.Add("idsp=" + HttpUtility.UrlEncode(idsp));

        string qty = (Request.QueryString["qty"] ?? string.Empty).Trim();
        if (!string.IsNullOrEmpty(qty))
            parts.Add("qty=" + HttpUtility.UrlEncode(qty));

        string returnUrl = (Request.QueryString["return_url"] ?? string.Empty).Trim();
        if (!string.IsNullOrEmpty(returnUrl))
            parts.Add("return_url=" + HttpUtility.UrlEncode(returnUrl));

        return parts.Count == 0 ? path : (path + "?" + string.Join("&", parts.ToArray()));
    }

    private void HandleCommitCreateModeRequest()
    {
        if (GetCart().Count == 0)
        {
            SetCreateFlowDialog("Giỏ hàng trống. Vui lòng thêm tin để tạo đơn.", "warning");
            Response.Redirect(ResolveCreateModeUrlWithoutCommit(), true);
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            string sellerAccount = CurrentAccountKey();
            GianHangPosInteraction_cl.CreateOrderActionResult actionResult =
                GianHangPosInteraction_cl.CreateOfflineOrder(
                    db,
                    Session,
                    CART_SESSION_KEY,
                    sellerAccount,
                    ResolveChoThanhToanUrl());

            GianHangOrderCommand_cl.CommandResult result = actionResult == null ? null : actionResult.Command;
            if (result == null || !result.Success)
            {
                SetCreateFlowDialog(result == null ? "Có lỗi xảy ra khi tạo đơn." : result.Message, result == null ? "alert" : result.AlertType);
                Response.Redirect(ResolveCreateModeUrlWithoutCommit(), true);
                return;
            }

            SetCreateFlowDialog(result.Message ?? string.Empty, result.AlertType ?? "alert");
            Response.Redirect(string.IsNullOrWhiteSpace(result.RedirectUrl) ? ResolveChoThanhToanUrl() : result.RedirectUrl, true);
        }
    }

    private void RedirectAfterPostback(string rawUrl)
    {
        string url = (rawUrl ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(url))
            url = ResolveChoThanhToanUrl();

        ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
        string js = "window.location.href='" + HttpUtility.JavaScriptStringEncode(url) + "';";
        if (scriptManager != null && scriptManager.IsInAsyncPostBack)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString("N"), js, true);
            return;
        }

        Response.Redirect(url, false);
        if (Context != null && Context.ApplicationInstance != null)
            Context.ApplicationInstance.CompleteRequest();
    }

    protected string ResolveImageUrl(object raw)
    {
        return GianHangStorefront_cl.ResolveImageUrl((raw ?? "").ToString());
    }

    private string GetTradeTypeFilter()
    {
        string value = (ViewState[FILTER_TYPE_KEY] ?? "all").ToString().Trim().ToLowerInvariant();
        if (value == "product" || value == "service")
            return value;
        return "all";
    }

    private void SetTradeTypeFilter(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        if (value != "product" && value != "service")
            value = "all";
        ViewState[FILTER_TYPE_KEY] = value;
    }

    private string GetTradeTypeFilterLabel()
    {
        return GianHangPosView_cl.ResolveTradeTypeFilterLabel(GetTradeTypeFilter());
    }

    private int GetResultLimit()
    {
        int limit;
        if (!int.TryParse((ViewState[RESULT_LIMIT_KEY] ?? "").ToString(), out limit))
            limit = GianHangPosPage_cl.ResultLimitStep;
        return GianHangPosPage_cl.NormalizeResultLimit(limit);
    }

    private void ResetResultLimit()
    {
        ViewState[RESULT_LIMIT_KEY] = GianHangPosPage_cl.ResultLimitStep.ToString();
    }

    private void IncreaseResultLimit()
    {
        ViewState[RESULT_LIMIT_KEY] = GianHangPosPage_cl.IncreaseResultLimit(GetResultLimit()).ToString();
    }

    private void SyncTradeTypeDropdown()
    {
        if (ddl_trade_type == null)
            return;

        string selected = GetTradeTypeFilter();
        ListItem item = ddl_trade_type.Items.FindByValue(selected);
        if (item == null)
            return;

        ddl_trade_type.ClearSelection();
        item.Selected = true;
    }

    private string GetOrderStatusFilter()
    {
        string value = (ViewState[ORDER_STATUS_FILTER_KEY] ?? "all").ToString().Trim().ToLowerInvariant();
        return GianHangOrderCore_cl.NormalizeOrderStatusFilter(value);
    }

    private void SetOrderStatusFilter(string raw)
    {
        ViewState[ORDER_STATUS_FILTER_KEY] = GianHangOrderCore_cl.NormalizeOrderStatusFilter(raw);
    }

    private string ResolveOrderStatusFilterLabel(string filterKey)
    {
        return GianHangPosView_cl.ResolveOrderStatusFilterLabel(filterKey);
    }

    private void SyncOrderStatusDropdown()
    {
        if (ddl_order_status == null)
            return;

        string selected = GetOrderStatusFilter();
        ListItem item = ddl_order_status.Items.FindByValue(selected);
        if (item == null)
            return;

        ddl_order_status.ClearSelection();
        item.Selected = true;
    }

    private void HandlePassivePostbacks()
    {
        string eventTarget = (Request["__EVENTTARGET"] ?? "").Trim();
        GianHangPosPage_cl.PassivePostbackState state = GianHangPosInteraction_cl.HandleCreatePostback(
            GetTradeTypeFilter(),
            ddl_trade_type == null ? string.Empty : ddl_trade_type.SelectedValue,
            but_search_more != null && string.Equals(eventTarget, but_search_more.UniqueID, StringComparison.Ordinal),
            GetResultLimit());
        if (!state.ShouldRebind)
            return;

        SetTradeTypeFilter(state.TradeTypeFilter);
        ViewState[RESULT_LIMIT_KEY] = state.ResultLimit.ToString();
        SyncTradeTypeDropdown();
        BindCreateModeUI(txt_sp_search.Text.Trim());
        up_taodon.Update();
    }

    private void HandlePassiveOrderListPostbacks()
    {
        if (ddl_order_status == null)
            return;

        GianHangPosWorkflow_cl.OrderListPostbackState state =
            GianHangPosInteraction_cl.HandleOrderListPostback(
                GetOrderStatusFilter(),
                ddl_order_status.SelectedValue);
        if (state == null || !state.ShouldRebind)
            return;

        SetOrderStatusFilter(state.StatusFilter);
        SyncOrderStatusDropdown();
        BindOrderList();
        up_main.Update();
    }

    private void BindCreateModeUI(string keyword)
    {
        SqlTransientGuard_cl.Execute(() =>
        {
            using (dbDataContext db = new dbDataContext())
            {
                BindCreateModeUI(db, keyword);
            }
        });
    }

    private void BindCreateModeUI(dbDataContext db, string keyword)
    {
        GianHangPosPage_cl.CreateModeBindingState state = GianHangPosInteraction_cl.BuildCreateModeState(
            db,
            CurrentAccountKey(),
            GetTradeTypeFilter(),
            keyword,
            GetResultLimit(),
            Session,
            CART_SESSION_KEY);

        RepeaterSearch.DataSource = state.SearchPanel == null ? new List<GianHangOrderCore_cl.SellablePostCard>() : state.SearchPanel.Rows;
        RepeaterSearch.DataBind();
        ph_no_search.Visible = state.SearchPanel == null || state.SearchPanel.IsEmpty;
        but_search_more.Visible = state.SearchPanel != null && state.SearchPanel.HasMore;
        lb_sp_search_note.Text = state.SearchPanel == null ? string.Empty : (state.SearchPanel.NoteText ?? string.Empty);
        lb_search_result_summary.Text = state.SearchPanel == null ? string.Empty : (state.SearchPanel.SummaryText ?? string.Empty);

        BindCartUI(state.Cart);
    }

    private void BindCartUI()
    {
        BindCartUI(GianHangPosPage_cl.BuildCartState(GetCart()));
    }

    private void BindCartUI(GianHangPosPage_cl.CartBindingState state)
    {
        GianHangPosPage_cl.CartBindingState cartState = state ?? GianHangPosPage_cl.BuildCartState(GetCart());
        ph_no_cart.Visible = cartState.IsEmpty;
        RepeaterCart.DataSource = cartState.Items ?? new List<GianHangPosCartSession_cl.CartItem>();
        RepeaterCart.DataBind();

        lb_cart_total_vnd.Text = cartState.TotalVndText ?? "0";
        lb_cart_total_a.Text = cartState.TotalRightsText ?? "0.00";
        lb_cart_count_mobile.Text = cartState.TotalCountText ?? "0";
        lb_cart_count_mobile_tab.Text = cartState.TotalCountText ?? "0";
        lb_cart_total_mobile.Text = cartState.TotalVndText ?? "0";
        lb_cart_err.Text = "";
    }

    private bool AddProductToCartById(string idsp, bool showWarning)
    {
        return SqlTransientGuard_cl.Execute(() =>
        {
            using (dbDataContext db = new dbDataContext())
            {
                return AddProductToCartById(db, idsp, showWarning);
            }
        });
    }

    private bool AddProductToCartById(dbDataContext db, string idsp, bool showWarning)
    {
        string productId = (idsp ?? "").Trim();
        if (string.IsNullOrEmpty(productId))
            return false;
        string warningMessage;
        if (!GianHangPosInteraction_cl.TryAddProductToCart(
                db,
                Session,
                CART_SESSION_KEY,
                CurrentAccountKey(),
                productId,
                out warningMessage))
        {
            if (showWarning)
                Helper_Tabler_cl.ShowModal(this.Page, string.IsNullOrWhiteSpace(warningMessage) ? "Tin không tồn tại hoặc không thuộc quyền quản lý của tài khoản này." : warningMessage, "Thông báo", true, "warning");
            return false;
        }
        return true;
    }

    private void BindOrderList()
    {
        SqlTransientGuard_cl.Execute(() =>
        {
            using (dbDataContext db = new dbDataContext())
            {
                BindOrderList(db);
            }
        });
    }

    private void BindOrderList(dbDataContext db)
    {
        GianHangPosPage_cl.OrderListBindingState state = GianHangPosInteraction_cl.BuildOrderListState(
            db,
            CurrentAccountKey(),
            GetOrderStatusFilter(),
            txt_order_search == null ? string.Empty : txt_order_search.Text.Trim());

        GianHangOrderCore_cl.SellerOrderSummary summary = state == null ? new GianHangOrderCore_cl.SellerOrderSummary() : (state.Summary ?? new GianHangOrderCore_cl.SellerOrderSummary());
        lb_count_total.Text = summary.Total.ToString("#,##0");
        lb_count_pending.Text = summary.Pending.ToString("#,##0");
        lb_count_wait.Text = summary.WaitingExchange.ToString("#,##0");
        lb_count_exchanged.Text = summary.Exchanged.ToString("#,##0");
        lb_count_delivered.Text = summary.Delivered.ToString("#,##0");
        lb_count_cancelled.Text = summary.Cancelled.ToString("#,##0");
        lb_order_status_filter.Text = state == null || state.Panel == null ? ResolveOrderStatusFilterLabel(GetOrderStatusFilter()) : (state.Panel.FilterLabel ?? ResolveOrderStatusFilterLabel(GetOrderStatusFilter()));
        lb_order_result_summary.Text = state == null || state.Panel == null ? string.Empty : (state.Panel.SummaryText ?? string.Empty);
        SyncOrderStatusDropdown();

        RepeaterOrders.DataSource = state == null || state.Panel == null ? new List<GianHangOrderCore_cl.SellerOrderRow>() : (state.Panel.Rows ?? new List<GianHangOrderCore_cl.SellerOrderRow>());
        RepeaterOrders.DataBind();
        ph_empty_orders.Visible = state == null || state.Panel == null || state.Panel.IsEmpty;
    }

    protected string ResolveOrderTypeBadgeCss(object rawOffline)
    {
        bool isOffline = false;
        bool.TryParse((rawOffline ?? "False").ToString(), out isOffline);
        return isOffline ? "badge bg-blue-lt text-blue" : "badge bg-yellow-lt text-yellow";
    }

    protected string ResolveOrderTypeLabel(object rawOffline)
    {
        bool isOffline = false;
        bool.TryParse((rawOffline ?? "False").ToString(), out isOffline);
        return isOffline ? "Offline" : "Online";
    }

    protected string ResolveOrderStatusBadgeCss(object rawGroup)
    {
        string statusGroup = (rawGroup ?? "").ToString().Trim().ToLowerInvariant();
        switch (statusGroup)
        {
            case "cho-trao-doi":
                return "badge bg-red-lt text-red";
            case "da-trao-doi":
                return "badge bg-blue-lt text-blue";
            case "da-giao":
                return "badge bg-green-lt text-green";
            case "da-huy":
                return "badge bg-muted-lt text-muted";
            default:
                return "badge bg-azure-lt text-azure";
        }
    }

    protected void txt_sp_search_TextChanged(object sender, EventArgs e)
    {
        ResetResultLimit();
        BindCreateModeUI(txt_sp_search.Text.Trim());
        up_taodon.Update();
    }

    protected void ddl_trade_type_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetTradeTypeFilter(ddl_trade_type.SelectedValue);
        ResetResultLimit();
        SyncTradeTypeDropdown();
        BindCreateModeUI(txt_sp_search.Text.Trim());
        up_taodon.Update();
    }

    protected void but_search_more_Click(object sender, EventArgs e)
    {
        IncreaseResultLimit();
        BindCreateModeUI(txt_sp_search.Text.Trim());
        up_taodon.Update();
    }

    protected void RepeaterSearch_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName != "add")
            return;

        string idsp = (e.CommandArgument ?? "").ToString();
        if (string.IsNullOrEmpty(idsp))
            return;

        AddProductToCartById(idsp, true);
        BindCreateModeUI(txt_sp_search.Text.Trim());
        up_taodon.Update();
    }

    protected void RepeaterCart_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string idsp = (e.CommandArgument ?? "").ToString();
        if (string.IsNullOrEmpty(idsp))
            return;

        List<GianHangPosCartSession_cl.CartItem> cart = GetCart();
        if (!cart.Any(x => x.ProductId == idsp))
        {
            BindCartUI();
            up_taodon.Update();
            return;
        }

        TextBox txtQty = e.Item.FindControl("txt_qty") as TextBox;
        TextBox txtDiscount = e.Item.FindControl("txt_discount") as TextBox;
        BindCartUI(
            GianHangPosInteraction_cl.HandleCartCommand(
                Session,
                CART_SESSION_KEY,
                idsp,
                e.CommandName,
                txtQty == null ? string.Empty : txtQty.Text,
                txtDiscount == null ? string.Empty : txtDiscount.Text));
        up_taodon.Update();
    }

    protected void but_clear_cart_Click(object sender, EventArgs e)
    {
        BindCartUI(GianHangPosInteraction_cl.ClearCart(Session, CART_SESSION_KEY));
        up_taodon.Update();
    }

    protected void but_taodon_Click(object sender, EventArgs e)
    {
        HandleCommitCreateModeRequest();
    }

    protected void but_show_form_taodon_Click(object sender, EventArgs e)
    {
        RedirectAfterPostback(ResolveCreateModeEntryUrl(""));
    }

    protected void txt_order_search_TextChanged(object sender, EventArgs e)
    {
        BindOrderList();
        up_main.Update();
    }

    protected void ddl_order_status_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetOrderStatusFilter(ddl_order_status.SelectedValue);
        BindOrderList();
        up_main.Update();
    }

    protected void RepeaterOrders_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string orderId = (e.CommandArgument ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(orderId))
            return;
        using (dbDataContext db = new dbDataContext())
        {
            string sellerAccount = CurrentAccountKey();
            GianHangOrderCommand_cl.CommandResult result =
                GianHangPosWorkflow_cl.ExecuteOrderListCommand(db, sellerAccount, e.CommandName, orderId);

            if (result.ShouldRedirect && !string.IsNullOrWhiteSpace(result.RedirectUrl))
            {
                RedirectAfterPostback(result.RedirectUrl);
                return;
            }

            BindOrderList();
            up_main.Update();
            Helper_Tabler_cl.ShowModal(this.Page, result.Message, "Thông báo", true, result.AlertType);
        }
    }
}
