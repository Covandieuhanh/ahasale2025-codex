using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

public partial class gianhang_admin_gianhang_gio_hang : System.Web.UI.Page
{
    private sealed class CartRowView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string PriceText { get; set; }
        public string QuantityText { get; set; }
        public string LineTotalText { get; set; }
        public string RemoveUrl { get; set; }
    }

    private readonly dbDataContext db = new dbDataContext();
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");
    private string ownerAccountKey = string.Empty;

    public string WorkspaceDisplayName = "Không gian /gianhang";
    public string HubUrl = "/gianhang/admin/gianhang/default.aspx";
    public string PublicUrl = "/gianhang/public.aspx";
    public string PublicCartUrl = "/gianhang/giohang.aspx";
    public string OrdersUrl = "/gianhang/admin/gianhang/don-ban.aspx";
    public string WaitUrl = "/gianhang/admin/gianhang/cho-thanh-toan.aspx";
    public string BuyerFlowUrl = "/gianhang/admin/gianhang/don-mua.aspx";
    public string ElectronicInvoiceUrl = "/gianhang/admin/gianhang/hoa-don-dien-tu.aspx";
    public int CartLineCount;
    public int CartQuantity;
    public decimal CartTotal;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        ownerAccountKey = (access.OwnerAccountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (ownerAccountKey == string.Empty)
            return;

        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        OrdersUrl = GianHangRoutes_cl.BuildAdminWorkspaceOrdersUrl();
        WaitUrl = GianHangRoutes_cl.BuildAdminWorkspaceWaitUrl();
        BuyerFlowUrl = GianHangRoutes_cl.BuildAdminWorkspaceBuyerFlowUrl();
        ElectronicInvoiceUrl = GianHangRoutes_cl.BuildAdminWorkspaceElectronicInvoiceUrl();
        PublicUrl = GianHangRoutes_cl.BuildPublicUrl(ownerAccountKey);
        PublicCartUrl = GianHangRoutes_cl.BuildCartUrl(ownerAccountKey, Request.RawUrl ?? GianHangRoutes_cl.BuildAdminWorkspaceCartUrl());

        taikhoan_tb owner = RootAccount_cl.GetByAccountKey(db, ownerAccountKey);
        string display = owner == null ? string.Empty : ((owner.ten_shop ?? string.Empty).Trim());
        if (display == string.Empty)
            display = owner == null ? string.Empty : ((owner.hoten ?? string.Empty).Trim());
        if (display == string.Empty)
            display = ownerAccountKey;
        WorkspaceDisplayName = display;

        BindCart();
    }

    protected void btn_clear_preview_Click(object sender, EventArgs e)
    {
        if (ownerAccountKey == string.Empty)
            return;

        GianHangCart_cl.Clear(ownerAccountKey);
        Response.Redirect(GianHangRoutes_cl.BuildAdminWorkspaceCartUrl(), false);
        if (Context != null && Context.ApplicationInstance != null)
            Context.ApplicationInstance.CompleteRequest();
    }

    protected string FormatMoney(decimal value)
    {
        return value.ToString("#,##0.##", ViCulture) + " đ";
    }

    private void BindCart()
    {
        DataTable cart = GianHangCart_cl.GetCart(ownerAccountKey, false);
        List<CartRowView> rows = new List<CartRowView>();
        CartLineCount = 0;
        CartQuantity = 0;
        CartTotal = 0m;

        if (cart != null)
        {
            foreach (DataRow row in cart.Rows)
            {
                int id = SafeToInt(row["ID"]);
                int qty = SafeToInt(row["soluong"]);
                decimal price = SafeToDecimal(row["Price"]);
                decimal lineTotal = SafeToDecimal(row["thanhtien"]);
                CartLineCount++;
                CartQuantity += qty;
                CartTotal += lineTotal;

                rows.Add(new CartRowView
                {
                    Id = id,
                    Name = Convert.ToString(row["Name"], ViCulture) ?? string.Empty,
                    ImageUrl = GianHangStorefront_cl.ResolveImageUrl(Convert.ToString(row["img"], ViCulture)),
                    PriceText = FormatMoney(price),
                    QuantityText = qty.ToString("#,##0", ViCulture),
                    LineTotalText = FormatMoney(lineTotal),
                    RemoveUrl = GianHangRoutes_cl.BuildRemoveCartItemUrl(ownerAccountKey, id, Request.RawUrl ?? GianHangRoutes_cl.BuildAdminWorkspaceCartUrl())
                });
            }
        }

        btn_clear_preview.Visible = rows.Count > 0;
        ph_empty.Visible = rows.Count == 0;
        rp_cart.DataSource = rows;
        rp_cart.DataBind();
    }

    private static int SafeToInt(object raw)
    {
        if (raw == null || raw == DBNull.Value)
            return 0;
        int value;
        return int.TryParse(Convert.ToString(raw, ViCulture), out value) ? value : 0;
    }

    private static decimal SafeToDecimal(object raw)
    {
        if (raw == null || raw == DBNull.Value)
            return 0m;
        try
        {
            return Convert.ToDecimal(raw, ViCulture);
        }
        catch
        {
            return 0m;
        }
    }
}
