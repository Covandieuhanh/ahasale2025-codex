using System;
using System.Globalization;
using System.Web;
using System.Web.UI;

public partial class xoa_chitiet_giohang : Page
{
    private const string NoticePrefix = "__gh_public_cart_notice_";

    protected void Page_Load(object sender, EventArgs e)
    {
        string storeAccount = ResolveStoreAccount();
        if (string.IsNullOrWhiteSpace(storeAccount))
        {
            Response.Redirect("/gianhang/public.aspx", false);
            if (Context != null && Context.ApplicationInstance != null)
                Context.ApplicationInstance.CompleteRequest();
            return;
        }

        int itemId;
        if (int.TryParse((Request.QueryString["id"] ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out itemId) && itemId > 0)
        {
            GianHangCart_cl.RemoveItem(storeAccount, itemId);
            SetNotice(storeAccount, "Đã xóa mặt hàng khỏi giỏ.", "success");
        }

        string returnUrl = NormalizeReturnUrl(Request.QueryString["return_url"]);
        string redirectUrl = "/gianhang/giohang.aspx?user=" + HttpUtility.UrlEncode(storeAccount);
        if (!string.IsNullOrWhiteSpace(returnUrl))
            redirectUrl += "&return_url=" + HttpUtility.UrlEncode(returnUrl);

        Response.Redirect(redirectUrl, false);
        if (Context != null && Context.ApplicationInstance != null)
            Context.ApplicationInstance.CompleteRequest();
    }

    private string ResolveStoreAccount()
    {
        string storeAccount = (Request.QueryString["user"] ?? "").Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(storeAccount))
            return storeAccount;

        storeAccount = GianHangCart_cl.GetActiveStorefrontAccount();
        return (storeAccount ?? "").Trim().ToLowerInvariant();
    }

    private void SetNotice(string storeAccount, string message, string kind)
    {
        if (Context == null || Context.Session == null || string.IsNullOrWhiteSpace(storeAccount))
            return;

        Context.Session[NoticePrefix + storeAccount] = (kind ?? "warning") + "||" + (message ?? "");
    }

    private static string NormalizeReturnUrl(string rawReturnUrl)
    {
        string value = (rawReturnUrl ?? "").Trim();
        if (value == "")
            return "";
        if (!value.StartsWith("/", StringComparison.Ordinal))
            return "";
        if (value.StartsWith("//", StringComparison.Ordinal))
            return "";
        return value;
    }
}
