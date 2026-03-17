using System;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class gianhang_admin_cau_hinh_storefront_default : System.Web.UI.Page
{
    private readonly dbDataContext db = new dbDataContext();
    public string notifi = string.Empty;
    public string shopDashboardUrl = "/shop/default.aspx";
    public string shopPublicUrl = "/shop/public.aspx";
    private gianhang_storefront_config_table config;
    private string chiNhanhId = "1";

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            RequireAdmin("q1_3");
            if (!AdvancedAdminOwnerGuard_cl.EnsureOwnerOnly(this, Session["user"].ToString(), AhaShineContext_cl.UserParent, "cấu hình /shop", "/gianhang/admin"))
                return;

            string warningMessage;
            if (!AhaShineContext_cl.TryEnsureContext(out warningMessage))
            {
                ShowTransientWarning(warningMessage);
                BindEmptySections();
                return;
            }

            chiNhanhId = AhaShineContext_cl.ResolveChiNhanhId();
            SqlTransientGuard_cl.Execute(() =>
            {
                config = GianHangStorefrontConfig_cl.GetConfig(db, chiNhanhId);
                shopPublicUrl = ShopSlug_cl.GetPublicUrlByTaiKhoan(db, AhaShineContext_cl.UserParent);

                if (!IsPostBack)
                    LoadForm();

                BindSections();
            }, 3, 250);

            notifi = (Session["notifi"] ?? string.Empty).ToString();
            Session["notifi"] = string.Empty;
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            ShowTransientWarning(AhaShineContext_cl.BuildTransientWarningMessage());
            BindEmptySections();
        }
    }

    private void LoadForm()
    {
        string mode = GianHangStorefrontConfig_cl.ResolveText(config.storefront_mode, GianHangStorefrontConfig_cl.ModeAuto);
        if (ddl_mode.Items.FindByValue(mode) != null)
            ddl_mode.SelectedValue = mode;
        txt_brand_note.Text = config.brand_note;
        txt_nav_home.Text = config.nav_home_text;
        txt_nav_booking.Text = config.nav_booking_text;
        chk_quickstrip_visible.Checked = config.quickstrip_visible ?? true;
        txt_quick_service.Text = config.quick_service_text;
        txt_quick_product.Text = config.quick_product_text;
        txt_quick_article.Text = config.quick_article_text;
        txt_quick_consult.Text = config.quick_consult_text;
        txt_quick_booking.Text = config.quick_booking_text;
        txt_hero_eyebrow.Text = config.hero_eyebrow;
        txt_hero_title.Text = config.hero_title;
        txt_hero_description.Text = config.hero_description;
        txt_hero_primary_text.Text = config.hero_primary_text;
        txt_hero_primary_url.Text = config.hero_primary_url;
        txt_hero_secondary_text.Text = config.hero_secondary_text;
        txt_hero_secondary_url.Text = config.hero_secondary_url;
        txt_hero_tertiary_text.Text = config.hero_tertiary_text;
        txt_hero_tertiary_url.Text = config.hero_tertiary_url;
        txt_stage_primary_title.Text = config.hero_highlight_title;
        txt_stage_primary_desc.Text = config.hero_highlight_description;
        txt_stage_secondary_title.Text = config.hero_highlight_secondary_title;
        txt_stage_secondary_desc.Text = config.hero_highlight_secondary_description;
        txt_metric_service.Text = config.hero_metric_service_text;
        txt_metric_product.Text = config.hero_metric_product_text;
        txt_metric_article.Text = config.hero_metric_article_text;
        txt_footer_description.Text = config.footer_description;
        txt_footer_chip1.Text = config.footer_chip_1;
        txt_footer_chip2.Text = config.footer_chip_2;
        txt_footer_chip3.Text = config.footer_chip_3;
        txt_footer_chip4.Text = config.footer_chip_4;
        txt_footer_nav_title.Text = config.footer_nav_title;
        txt_footer_category_title.Text = config.footer_category_title;
        txt_footer_contact_title.Text = config.footer_contact_title;
        txt_footer_primary_text.Text = config.footer_bottom_primary_text;
        txt_footer_primary_url.Text = config.footer_bottom_primary_url;
        txt_footer_secondary_text.Text = config.footer_bottom_secondary_text;
        txt_footer_secondary_url.Text = config.footer_bottom_secondary_url;
    }

    private void BindSections()
    {
        rptSections.DataSource = GianHangStorefrontConfig_cl.GetAllSections(db, chiNhanhId);
        rptSections.DataBind();
    }

    private void BindEmptySections()
    {
        rptSections.DataSource = new object[0];
        rptSections.DataBind();
    }

    private void ShowTransientWarning(string warningMessage)
    {
        string message = string.IsNullOrWhiteSpace(warningMessage)
            ? AhaShineContext_cl.BuildTransientWarningMessage()
            : warningMessage;
        if (Session["notifi"] == null || string.IsNullOrWhiteSpace(Session["notifi"].ToString()))
            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", message, "2200", "warning");
        notifi = (Session["notifi"] ?? string.Empty).ToString();
        Session["notifi"] = string.Empty;
    }

    protected void ButtonSave_Click(object sender, EventArgs e)
    {
        try
        {
            RequireAdmin("q1_3");
            if (!AdvancedAdminOwnerGuard_cl.EnsureOwnerOnly(this, Session["user"].ToString(), AhaShineContext_cl.UserParent, "cấu hình /shop", "/gianhang/admin"))
                return;
            chiNhanhId = AhaShineContext_cl.ResolveChiNhanhId();
            config = SqlTransientGuard_cl.Execute(() => GianHangStorefrontConfig_cl.GetConfig(db, chiNhanhId), 3, 250);

            config.storefront_mode = ddl_mode.SelectedValue;
            config.brand_note = txt_brand_note.Text.Trim();
            config.nav_home_text = txt_nav_home.Text.Trim();
            config.nav_booking_text = txt_nav_booking.Text.Trim();
            config.quickstrip_visible = chk_quickstrip_visible.Checked;
            config.quick_service_text = txt_quick_service.Text.Trim();
            config.quick_product_text = txt_quick_product.Text.Trim();
            config.quick_article_text = txt_quick_article.Text.Trim();
            config.quick_consult_text = txt_quick_consult.Text.Trim();
            config.quick_booking_text = txt_quick_booking.Text.Trim();
            config.hero_eyebrow = txt_hero_eyebrow.Text.Trim();
            config.hero_title = txt_hero_title.Text.Trim();
            config.hero_description = txt_hero_description.Text.Trim();
            config.hero_primary_text = txt_hero_primary_text.Text.Trim();
            config.hero_primary_url = txt_hero_primary_url.Text.Trim();
            config.hero_secondary_text = txt_hero_secondary_text.Text.Trim();
            config.hero_secondary_url = txt_hero_secondary_url.Text.Trim();
            config.hero_tertiary_text = txt_hero_tertiary_text.Text.Trim();
            config.hero_tertiary_url = txt_hero_tertiary_url.Text.Trim();
            config.hero_highlight_title = txt_stage_primary_title.Text.Trim();
            config.hero_highlight_description = txt_stage_primary_desc.Text.Trim();
            config.hero_highlight_secondary_title = txt_stage_secondary_title.Text.Trim();
            config.hero_highlight_secondary_description = txt_stage_secondary_desc.Text.Trim();
            config.hero_metric_service_text = txt_metric_service.Text.Trim();
            config.hero_metric_product_text = txt_metric_product.Text.Trim();
            config.hero_metric_article_text = txt_metric_article.Text.Trim();
            config.footer_description = txt_footer_description.Text.Trim();
            config.footer_chip_1 = txt_footer_chip1.Text.Trim();
            config.footer_chip_2 = txt_footer_chip2.Text.Trim();
            config.footer_chip_3 = txt_footer_chip3.Text.Trim();
            config.footer_chip_4 = txt_footer_chip4.Text.Trim();
            config.footer_nav_title = txt_footer_nav_title.Text.Trim();
            config.footer_category_title = txt_footer_category_title.Text.Trim();
            config.footer_contact_title = txt_footer_contact_title.Text.Trim();
            config.footer_bottom_primary_text = txt_footer_primary_text.Text.Trim();
            config.footer_bottom_primary_url = txt_footer_primary_url.Text.Trim();
            config.footer_bottom_secondary_text = txt_footer_secondary_text.Text.Trim();
            config.footer_bottom_secondary_url = txt_footer_secondary_url.Text.Trim();
            config.updated_at = DateTime.Now;
            SqlTransientGuard_cl.Execute(() => db.SubmitChanges(), 3, 250);

            Session["notifi"] = thongbao_class.metro_notifi_onload("Thong bao", "Cap nhat storefront thanh cong.", "3000", "success");
            Response.Redirect("/gianhang/admin/cau-hinh-storefront/default.aspx");
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", AhaShineContext_cl.BuildTransientWarningMessage(), "2200", "warning");
            Response.Redirect("/gianhang/admin/cau-hinh-storefront/default.aspx");
        }
    }

    private void RequireAdmin(string quyen)
    {
        try
        {
            string cookieUser = Request.Cookies["save_user_admin_aka_1"] != null ? Request.Cookies["save_user_admin_aka_1"].Value : "";
            string cookiePass = Request.Cookies["save_pass_admin_aka_1"] != null ? Request.Cookies["save_pass_admin_aka_1"].Value : "";
            if (Session["user"] == null) Session["user"] = "";
            if (Session["notifi"] == null) Session["notifi"] = "";
            if (Session["user"].ToString() == "") Response.Redirect("/gianhang/admin/f5_ss_admin.aspx");
            string url = Request.Url.GetLeftPart(UriPartial.Authority).ToLower();
            string result = SqlTransientGuard_cl.Execute(() => bcorn_class.check_login(Session["user"].ToString(), cookieUser, cookiePass, url, quyen), 3, 250);
            if (result == "")
                return;

            if (result == "baotri")
            {
                Response.Redirect("/baotri.aspx");
                return;
            }
            if (result == "1")
            {
                Response.Redirect("/gianhang/admin/login.aspx");
                return;
            }
            if (result == "2")
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thong bao", "Ban khong du quyen de truy cap thao tac vua roi.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin");
                return;
            }

            Session["notifi"] = result;
            Session["user"] = "";
            Response.Cookies["save_user_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["save_pass_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["save_url_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
            Response.Redirect("/gianhang/admin/login.aspx");
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", AhaShineContext_cl.BuildTransientWarningMessage(), "2200", "warning");
            Response.Redirect("/gianhang/admin/login.aspx");
        }
    }
}
