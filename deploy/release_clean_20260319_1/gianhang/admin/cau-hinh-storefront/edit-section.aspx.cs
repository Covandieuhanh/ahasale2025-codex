using System;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class gianhang_admin_cau_hinh_storefront_edit : System.Web.UI.Page
{
    private readonly dbDataContext db = new dbDataContext();
    public string notifi = string.Empty;
    private gianhang_storefront_section_table section;
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
                return;
            }

            chiNhanhId = AhaShineContext_cl.ResolveChiNhanhId();
            section = SqlTransientGuard_cl.Execute(() => ResolveSection(), 3, 250);
            if (section == null)
            {
                Response.Redirect("/gianhang/admin/cau-hinh-storefront/default.aspx");
                return;
            }

            if (!IsPostBack)
                LoadForm();

            notifi = (Session["notifi"] ?? string.Empty).ToString();
            Session["notifi"] = string.Empty;
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            ShowTransientWarning(AhaShineContext_cl.BuildTransientWarningMessage());
        }
    }

    private gianhang_storefront_section_table ResolveSection()
    {
        string id = (Request.QueryString["id"] ?? string.Empty).Trim();
        if (id == string.Empty)
            return null;
        return GianHangStorefrontConfig_cl.GetAllSections(db, chiNhanhId).FirstOrDefault(p => p.id.ToString() == id);
    }

    private void LoadForm()
    {
        txt_section_key.Text = section.section_key;
        string sourceType = GianHangStorefrontConfig_cl.ResolveText(section.source_type, section.section_key);
        if (ddl_source_type.Items.FindByValue(sourceType) != null)
            ddl_source_type.SelectedValue = sourceType;
        txt_subtitle.Text = section.subtitle;
        txt_title.Text = section.title;
        txt_description.Text = section.description;
        txt_badge.Text = section.badge_text;
        txt_item_label.Text = section.item_label;
        txt_item_limit.Text = section.item_limit.HasValue ? section.item_limit.Value.ToString() : string.Empty;
        txt_cta_text.Text = section.cta_text;
        txt_cta_url.Text = section.cta_url;
        txt_secondary_cta_text.Text = section.secondary_cta_text;
        txt_secondary_cta_url.Text = section.secondary_cta_url;
        txt_source_value.Text = section.source_value;
        txt_rank.Text = section.rank.HasValue ? section.rank.Value.ToString() : string.Empty;
        chk_visible.Checked = section.is_visible ?? false;
        txt_style_variant.Text = section.style_variant;
        txt_extra_json.Text = section.extra_json;
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
            section = SqlTransientGuard_cl.Execute(() => ResolveSection(), 3, 250);
            if (section == null)
            {
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Block storefront không còn tồn tại.", "2200", "warning");
                Response.Redirect("/gianhang/admin/cau-hinh-storefront/default.aspx");
                return;
            }

            int rank;
            int itemLimit;
            section.source_type = ddl_source_type.SelectedValue;
            section.subtitle = txt_subtitle.Text.Trim();
            section.title = txt_title.Text.Trim();
            section.description = txt_description.Text.Trim();
            section.badge_text = txt_badge.Text.Trim();
            section.item_label = txt_item_label.Text.Trim();
            section.cta_text = txt_cta_text.Text.Trim();
            section.cta_url = txt_cta_url.Text.Trim();
            section.secondary_cta_text = txt_secondary_cta_text.Text.Trim();
            section.secondary_cta_url = txt_secondary_cta_url.Text.Trim();
            section.source_value = txt_source_value.Text.Trim();
            section.rank = int.TryParse(txt_rank.Text.Trim(), out rank) ? rank : (int?)null;
            section.is_visible = chk_visible.Checked;
            section.item_limit = int.TryParse(txt_item_limit.Text.Trim(), out itemLimit) ? itemLimit : (int?)null;
            section.style_variant = txt_style_variant.Text.Trim();
            section.extra_json = txt_extra_json.Text.Trim();
            section.updated_at = DateTime.Now;
            SqlTransientGuard_cl.Execute(() => db.SubmitChanges(), 3, 250);

            Session["notifi"] = thongbao_class.metro_notifi_onload("Thong bao", "Cap nhat block storefront thanh cong.", "3000", "success");
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
