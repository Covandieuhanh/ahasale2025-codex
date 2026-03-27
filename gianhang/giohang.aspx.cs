using System;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;

public partial class gianhang_giohang : Page
{
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");
    private const string NoticePrefix = "__gh_public_cart_notice_";

    private RootAccount_cl.RootAccountInfo currentBuyerInfo;
    private taikhoan_tb currentStore;
    private string currentReturnUrl = "/gianhang/default.aspx";

    protected string StoreAccount = "";
    protected string StorefrontUrl = "/gianhang/public.aspx";
    protected string ContinueUrl = "/gianhang/default.aspx";
    protected string StoreName = "Gian hàng đối tác";
    protected string StoreAvatarUrl = "/uploads/images/macdinh.jpg";
    protected int CartLineCount;
    protected int CartQuantity;
    protected decimal CartTotal;
    protected decimal CartTotalQuyen;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!ResolveContext())
        {
            BindInvalidState("Không xác định được gian hàng công khai để xem giỏ hàng.");
            return;
        }

        if (!IsPostBack)
        {
            if (TryHandleIncomingAdd())
                return;

            PrefillCustomerInfo();
        }

        LoadNotice();
        BindStoreHeader();
        BindCart();
    }

    protected void btn_update_cart_Click(object sender, EventArgs e)
    {
        if (StoreAccount == "")
            return;

        GianHangCart_cl.UpdateQuantities(StoreAccount, Request);
        SetNotice("Đã cập nhật giỏ hàng thành công.", "success");
        RedirectCanonical(false);
    }

    protected void btn_clear_cart_Click(object sender, EventArgs e)
    {
        if (StoreAccount == "")
            return;

        GianHangCart_cl.Clear(StoreAccount);
        SetNotice("Đã xóa toàn bộ giỏ hàng.", "success");
        RedirectCanonical(false);
    }

    protected void btn_checkout_Click(object sender, EventArgs e)
    {
        if (StoreAccount == "")
            return;

        using (dbDataContext db = new dbDataContext())
        {
            DataTable cart = GianHangCart_cl.GetCart(StoreAccount, false);
            GianHangPublicOrder_cl.CheckoutResult result = GianHangPublicOrder_cl.CreateOrderFromCart(db, new GianHangPublicOrder_cl.CheckoutInput
            {
                StoreAccountKey = StoreAccount,
                CustomerName = (txt_customer_name.Text ?? "").Trim(),
                CustomerPhone = (txt_customer_phone.Text ?? "").Trim(),
                CustomerAddress = (txt_customer_address.Text ?? "").Trim(),
                Note = (txt_note.Text ?? "").Trim(),
                CartTable = cart,
                BuyerInfo = currentBuyerInfo
            });

            if (!result.Success)
            {
                ShowInlineNotice(result.ErrorMessage, "warning");
                BindCart();
                return;
            }

            GianHangCart_cl.Clear(StoreAccount);
            string successMessage = "Đặt hàng thành công. Mã đơn: #" + result.OrderId.ToString();

            if (currentBuyerInfo != null && currentBuyerInfo.IsAuthenticated && !string.IsNullOrWhiteSpace(currentBuyerInfo.AccountKey))
            {
                GianHangBuyerOrder_cl.SetNotice(Session, successMessage);
                Response.Redirect(GianHangRoutes_cl.BuildBuyerOrdersUrl(), false);
                if (Context != null && Context.ApplicationInstance != null)
                    Context.ApplicationInstance.CompleteRequest();
                return;
            }

            SetNotice(successMessage, "success");
            RedirectCanonical(false);
        }
    }

    protected string BuildRemoveUrl(object rawId)
    {
        int id;
        if (!int.TryParse(Convert.ToString(rawId, ViCulture), out id) || id <= 0)
            return "#";

        return GianHangRoutes_cl.BuildRemoveCartItemUrl(StoreAccount, id, currentReturnUrl);
    }

    protected string ResolveImageUrl(object raw)
    {
        return GianHangStorefront_cl.ResolveImageUrl(Convert.ToString(raw, ViCulture));
    }

    protected string FormatMoney(object raw)
    {
        decimal value = 0m;
        if (raw != null && raw != DBNull.Value)
        {
            try
            {
                value = Convert.ToDecimal(raw, ViCulture);
            }
            catch
            {
                value = 0m;
            }
        }

        return value.ToString("#,##0.##", ViCulture) + " đ";
    }

    protected string FormatQuyen(object raw)
    {
        decimal value = 0m;
        if (raw != null && raw != DBNull.Value)
        {
            try
            {
                value = Convert.ToDecimal(raw, ViCulture);
            }
            catch
            {
                value = 0m;
            }
        }

        decimal quyen = value / 1000m;
        return quyen.ToString("#,##0.##", ViCulture) + " A";
    }

    private bool ResolveContext()
    {
        currentBuyerInfo = RootAccount_cl.GetCurrentInfo();

        int incomingItemId;
        int? incomingItem = TryParseInt(Request.QueryString["id"], out incomingItemId) && incomingItemId > 0
            ? (int?)incomingItemId
            : null;

        string requestedUser = (Request.QueryString["user"] ?? "").Trim();
        if (requestedUser == "")
            requestedUser = GianHangCart_cl.GetActiveStorefrontAccount();

        using (dbDataContext db = new dbDataContext())
        {
            currentStore = GianHangPublicOrder_cl.ResolveStoreAccount(db, requestedUser, incomingItem);
        }

        if (currentStore == null)
            return false;

        StoreAccount = (currentStore.taikhoan ?? "").Trim().ToLowerInvariant();
        StorefrontUrl = GianHangPublic_cl.BuildStorefrontUrl(StoreAccount);
        currentReturnUrl = NormalizeReturnUrl(Request.QueryString["return_url"], StorefrontUrl);
        ContinueUrl = currentReturnUrl;
        StoreName = GianHangStorefront_cl.ResolveStorefrontName(currentStore);
        StoreAvatarUrl = GianHangStorefront_cl.ResolveAvatarUrl(currentStore);
        GianHangCart_cl.RememberActiveStorefrontAccount(StoreAccount);
        return true;
    }

    private bool TryHandleIncomingAdd()
    {
        int incomingItemId;
        if (!TryParseInt(Request.QueryString["id"], out incomingItemId) || incomingItemId <= 0)
            return false;

        int quantity = ParseQuantity();
        bool focusCheckout = ShouldFocusCheckout();

        using (dbDataContext db = new dbDataContext())
        {
            string error;
            bool ok = GianHangCart_cl.AddItem(db, StoreAccount, incomingItemId, quantity, out error);
            SetNotice(ok ? "Đã thêm vào giỏ hàng." : error, ok ? "success" : "warning");
        }

        RedirectCanonical(focusCheckout);
        return true;
    }

    private void PrefillCustomerInfo()
    {
        if (currentBuyerInfo == null || !currentBuyerInfo.IsAuthenticated || StoreAccount == "")
            return;

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb buyer = RootAccount_cl.GetByAccountKey(db, currentBuyerInfo.AccountKey);
            if (buyer == null)
                return;

            txt_customer_name.Text = (buyer.hoten ?? "").Trim();
            txt_customer_phone.Text = ResolveBuyerPhone(buyer);
            txt_customer_address.Text = (buyer.diachi ?? "").Trim();
        }
    }

    private void BindStoreHeader()
    {
        ph_invalid.Visible = false;
        ph_cart_page.Visible = true;

        img_store_avatar.ImageUrl = StoreAvatarUrl;
        lb_store_name.Text = HttpUtility.HtmlEncode(StoreName);
        lnk_storefront.NavigateUrl = StorefrontUrl;
        lnk_continue.NavigateUrl = ContinueUrl;
        lnk_back_to_store.NavigateUrl = StorefrontUrl;
    }

    private void BindCart()
    {
        DataTable cart = GianHangCart_cl.GetCart(StoreAccount, false);
        CartLineCount = cart == null ? 0 : cart.Rows.Count;
        CartQuantity = GianHangCart_cl.CalculateTotalQuantity(StoreAccount);
        CartTotal = GianHangCart_cl.CalculateTotal(StoreAccount);
        CartTotalQuyen = CartTotal / 1000m;

        lb_total_lines.Text = CartLineCount.ToString("#,##0", ViCulture);
        lb_total_quantity.Text = CartQuantity.ToString("#,##0", ViCulture);
        lb_total_amount.Text = CartTotal.ToString("#,##0.##", ViCulture) + " đ";
        lb_total_quyen.Text = CartTotalQuyen.ToString("#,##0.##", ViCulture) + " A";
        lb_checkout_amount.Text = lb_total_amount.Text;
        lb_checkout_quyen.Text = lb_total_quyen.Text;

        bool hasItems = cart != null && cart.Rows.Count > 0;
        ph_empty_cart.Visible = !hasItems;
        ph_cart_list.Visible = hasItems;
        ph_checkout.Visible = hasItems;
        btn_update_cart.Visible = hasItems;
        btn_clear_cart.Visible = hasItems;
        btn_checkout.Visible = hasItems;

        if (hasItems)
        {
            rpt_cart.DataSource = cart;
            rpt_cart.DataBind();
        }
        else
        {
            rpt_cart.DataSource = null;
            rpt_cart.DataBind();
        }
    }

    private void BindInvalidState(string message)
    {
        ph_invalid.Visible = true;
        ph_cart_page.Visible = false;
        lit_invalid_message.Text = HttpUtility.HtmlEncode((message ?? "").Trim());
        lnk_invalid_back.NavigateUrl = "/gianhang/public.aspx";
    }

    private void LoadNotice()
    {
        string raw = PullNotice();
        if (string.IsNullOrWhiteSpace(raw))
        {
            ph_notice.Visible = false;
            return;
        }

        string[] parts = raw.Split(new[] { "||" }, 2, StringSplitOptions.None);
        string kind = parts.Length > 0 ? parts[0] : "warning";
        string message = parts.Length > 1 ? parts[1] : raw;
        ShowInlineNotice(message, kind);
    }

    private void ShowInlineNotice(string message, string kind)
    {
        string normalizedKind = (kind ?? "warning").Trim().ToLowerInvariant();
        string css = normalizedKind == "success" ? "gh-cart-notice gh-cart-notice--success" : "gh-cart-notice gh-cart-notice--warning";

        ph_notice.Visible = !string.IsNullOrWhiteSpace(message);
        box_notice.Attributes["class"] = css;
        lit_notice.Text = HttpUtility.HtmlEncode((message ?? "").Trim());
    }

    private void RedirectCanonical(bool focusCheckout)
    {
        string url = BuildCanonicalUrl(focusCheckout);
        Response.Redirect(url, false);
        if (Context != null && Context.ApplicationInstance != null)
            Context.ApplicationInstance.CompleteRequest();
    }

    private string BuildCanonicalUrl(bool focusCheckout)
    {
        string url = "/gianhang/giohang.aspx?user=" + HttpUtility.UrlEncode(StoreAccount)
            + "&return_url=" + HttpUtility.UrlEncode(currentReturnUrl);
        if (focusCheckout)
            url += "#checkout-section";
        return url;
    }

    private void SetNotice(string message, string kind)
    {
        if (Context == null || Context.Session == null || StoreAccount == "")
            return;

        Context.Session[GetNoticeSessionKey()] = (kind ?? "warning") + "||" + (message ?? "");
    }

    private string PullNotice()
    {
        if (Context == null || Context.Session == null || StoreAccount == "")
            return "";

        string key = GetNoticeSessionKey();
        string raw = Convert.ToString(Context.Session[key], CultureInfo.InvariantCulture);
        Context.Session[key] = null;
        return raw ?? "";
    }

    private string GetNoticeSessionKey()
    {
        return NoticePrefix + StoreAccount;
    }

    private int ParseQuantity()
    {
        int quantity;
        if (!TryParseInt(Request.QueryString["qty"], out quantity))
            TryParseInt(Request.QueryString["sl"], out quantity);

        if (quantity <= 0)
            quantity = 1;
        if (quantity > 9999)
            quantity = 9999;
        return quantity;
    }

    private bool ShouldFocusCheckout()
    {
        string focus = (Request.QueryString["focus"] ?? "").Trim();
        string dh = (Request.QueryString["dh"] ?? "").Trim();
        return focus.Equals("checkout", StringComparison.OrdinalIgnoreCase)
            || dh.Equals("ok", StringComparison.OrdinalIgnoreCase);
    }

    private static string ResolveBuyerPhone(taikhoan_tb buyer)
    {
        if (buyer == null)
            return "";

        string phone = (buyer.dienthoai ?? "").Trim();
        if (phone != "")
            return phone;

        return (buyer.taikhoan ?? "").Trim();
    }

    private static string NormalizeReturnUrl(string rawReturnUrl, string fallbackUrl)
    {
        string fallback = string.IsNullOrWhiteSpace(fallbackUrl) ? "/gianhang/public.aspx" : fallbackUrl.Trim();
        string value = (rawReturnUrl ?? "").Trim();
        if (value == "")
            return fallback;
        if (!value.StartsWith("/", StringComparison.Ordinal))
            return fallback;
        if (value.StartsWith("//", StringComparison.Ordinal))
            return fallback;
        return value;
    }

    private static bool TryParseInt(string raw, out int value)
    {
        return int.TryParse((raw ?? "").Trim(), out value);
    }
}
