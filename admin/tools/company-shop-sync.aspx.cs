using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_tools_company_shop_sync : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminAccessGuard_cl.RequireFeatureAccess("admin_company_shop_sync", "/admin/default.aspx?mspace=admin");

        if (!IsPostBack)
            BindStatus(false);
    }

    protected void btn_run_Click(object sender, EventArgs e)
    {
        BindStatus(true);
    }

    private void BindStatus(bool forceSync)
    {
        pn_error.Visible = false;
        lit_error.Text = string.Empty;
        pn_config_status.Visible = false;
        lbl_config_status.Text = string.Empty;

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                CompanyShopBootstrap_cl.CompanyShopSyncReport report = forceSync
                    ? CompanyShopBootstrap_cl.RunManualSync(db)
                    : CompanyShopBootstrap_cl.RunManualSync(db);

                lit_account.Text = string.IsNullOrWhiteSpace(report.AccountKey) ? "(chưa có)" : report.AccountKey;
                lit_system_count.Text = report.SystemProductCount.ToString("#,##0");
                lit_internal_count.Text = report.InternalProductCount.ToString("#,##0");
                lit_map_count.Text = report.MapCount.ToString("#,##0");
                lit_special_count.Text = report.SpecialProductHandlerCount.ToString("#,##0");
                lit_policy.Text = report.HasActivePolicy
                    ? report.PolicyPercent.ToString("#,##0") + "%"
                    : "Chưa có";
                lit_access.Text = string.Format(
                    "Home: {0}<br/>Shop: {1}",
                    report.HasHomeAccess ? "active" : "missing",
                    report.HasShopAccess ? "active" : "missing");

                BindSpecialProducts(db, report.AccountKey);

                lbl_status.Text = report.AccountCreated
                    ? "Đã khởi tạo mới shop công ty và đồng bộ dữ liệu."
                    : "Đã rà soát và đồng bộ lại shop công ty.";
            }
        }
        catch (Exception ex)
        {
            lbl_status.Text = "Đồng bộ chưa thành công.";
            pn_error.Visible = true;
            lit_error.Text = Server.HtmlEncode(ex.ToString());
        }
    }

    protected void rpt_special_products_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            return;

        ShopSpecialProduct_cl.ProductHandlerAssignment item = e.Item.DataItem as ShopSpecialProduct_cl.ProductHandlerAssignment;
        if (item == null)
            return;

        DropDownList ddlHandler = e.Item.FindControl("ddl_handler") as DropDownList;
        DropDownList ddlCardType = e.Item.FindControl("ddl_card_type") as DropDownList;
        CheckBox chkResetPin = e.Item.FindControl("chk_reset_pin") as CheckBox;
        Literal litBadge = e.Item.FindControl("lit_handler_badge") as Literal;
        Literal litNote = e.Item.FindControl("lit_handler_note") as Literal;

        if (ddlHandler != null)
        {
            ListItem selected = ddlHandler.Items.FindByValue(item.HandlerCode ?? "");
            if (selected != null)
                ddlHandler.SelectedValue = selected.Value;
        }

        if (ddlCardType != null)
        {
            BindHomeCardTypes(ddlCardType);
            ListItem selectedCard = ddlCardType.Items.FindByValue(item.CardType.ToString());
            if (selectedCard != null)
                ddlCardType.SelectedValue = selectedCard.Value;
        }

        if (chkResetPin != null)
            chkResetPin.Checked = item.ResetPin;

        if (litBadge != null)
        {
            if (item.IsHandlerActive)
            {
                litBadge.Text = "<span class='company-special-badge is-active'>" + Server.HtmlEncode(item.HandlerLabel) + "</span>";
            }
            else
            {
                litBadge.Text = "<span class='company-special-badge is-off'>Không áp dụng</span>";
            }
        }

        if (litNote != null)
        {
            if (item.IsHandlerActive && string.Equals(item.HandlerCode, ShopSpecialProduct_cl.HandlerIssueCard, StringComparison.OrdinalIgnoreCase))
            {
                litNote.Text = "Khi bán, hệ thống sẽ phát hành <strong>" + Server.HtmlEncode(CardIssuance_cl.GetCardName(item.CardType)) + "</strong>"
                    + (item.ResetPin ? " và reset PIN về <code>6868</code>." : " và giữ nguyên PIN hiện tại nếu đã có.");
            }
            else
            {
                litNote.Text = "Sản phẩm này đang dùng luồng bán tiêu chuẩn, không có nghiệp vụ hậu xử lý riêng.";
            }
        }
    }

    protected void rpt_special_products_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (!string.Equals(e.CommandName, "save_handler", StringComparison.OrdinalIgnoreCase))
            return;

        HiddenField hfPostId = e.Item.FindControl("hf_post_id") as HiddenField;
        HiddenField hfSystemId = e.Item.FindControl("hf_system_id") as HiddenField;
        DropDownList ddlHandler = e.Item.FindControl("ddl_handler") as DropDownList;
        DropDownList ddlCardType = e.Item.FindControl("ddl_card_type") as DropDownList;
        CheckBox chkResetPin = e.Item.FindControl("chk_reset_pin") as CheckBox;

        int shopPostId;
        if (hfPostId == null || !int.TryParse((hfPostId.Value ?? "").Trim(), out shopPostId) || shopPostId <= 0)
        {
            ShowConfigStatus("Không xác định được sản phẩm cần cấu hình.", false);
            return;
        }

        int systemIdValue;
        int? systemProductId = null;
        if (hfSystemId != null && int.TryParse((hfSystemId.Value ?? "").Trim(), out systemIdValue) && systemIdValue > 0)
            systemProductId = systemIdValue;

        string handlerCode = ddlHandler == null ? "" : (ddlHandler.SelectedValue ?? "").Trim();
        int cardType = CardIssuance_cl.CardTypeTieuDung;
        if (ddlCardType != null)
            int.TryParse((ddlCardType.SelectedValue ?? "").Trim(), out cardType);
        if (cardType <= 0)
            cardType = CardIssuance_cl.CardTypeTieuDung;

        bool resetPin = chkResetPin != null && chkResetPin.Checked;

        try
        {
            string statusMessage = string.IsNullOrWhiteSpace(handlerCode)
                ? "Đã tắt handler đặc biệt cho sản phẩm #" + shopPostId + "."
                : "Đã lưu handler `" + handlerCode + "` cho sản phẩm #" + shopPostId + ".";

            using (dbDataContext db = new dbDataContext())
            {
                CompanyShopBootstrap_cl.CompanyShopSyncReport report = CompanyShopBootstrap_cl.RunManualSync(db);
                ShopSpecialProduct_cl.SaveProductHandler(
                    db,
                    report.AccountKey,
                    shopPostId,
                    systemProductId,
                    handlerCode,
                    cardType,
                    resetPin,
                    ResolveActor());

                BindStatus(false);
            }

            ShowConfigStatus(statusMessage, true);
        }
        catch (Exception ex)
        {
            ShowConfigStatus("Lưu cấu hình chưa thành công: " + ex.Message, false);
        }
    }

    private void BindSpecialProducts(dbDataContext db, string accountKey)
    {
        List<ShopSpecialProduct_cl.ProductHandlerAssignment> items = ShopSpecialProduct_cl.GetProductAssignments(db, accountKey);
        pn_special_empty.Visible = items.Count == 0;
        rpt_special_products.Visible = items.Count > 0;
        rpt_special_products.DataSource = items;
        rpt_special_products.DataBind();
    }

    private void BindHomeCardTypes(DropDownList ddl)
    {
        if (ddl == null)
            return;

        ddl.Items.Clear();
        ddl.Items.Add(new ListItem(CardIssuance_cl.GetCardName(CardIssuance_cl.CardTypeUuDai), CardIssuance_cl.CardTypeUuDai.ToString()));
        ddl.Items.Add(new ListItem(CardIssuance_cl.GetCardName(CardIssuance_cl.CardTypeTieuDung), CardIssuance_cl.CardTypeTieuDung.ToString()));
        ddl.Items.Add(new ListItem(CardIssuance_cl.GetCardName(CardIssuance_cl.CardTypeLaoDong), CardIssuance_cl.CardTypeLaoDong.ToString()));
        ddl.Items.Add(new ListItem(CardIssuance_cl.GetCardName(CardIssuance_cl.CardTypeDongHanhHeSinhThai), CardIssuance_cl.CardTypeDongHanhHeSinhThai.ToString()));
    }

    private void ShowConfigStatus(string message, bool success)
    {
        pn_config_status.Visible = true;
        lbl_config_status.Text = Server.HtmlEncode(message ?? "");
        lbl_config_status.ForeColor = success
            ? System.Drawing.ColorTranslator.FromHtml("#166534")
            : System.Drawing.ColorTranslator.FromHtml("#9f1239");
    }

    private string ResolveActor()
    {
        try
        {
            string raw = Session["taikhoan"] as string;
            if (!string.IsNullOrWhiteSpace(raw))
                return mahoa_cl.giaima_Bcorn(raw);
        }
        catch { }
        return "admin";
    }
}
