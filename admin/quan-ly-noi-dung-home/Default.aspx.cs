using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_noi_dung_home_Default : System.Web.UI.Page
{
    private sealed class FooterRowVm
    {
        public string ContentKey { get; set; }
        public string GroupLabel { get; set; }
        public string DisplayName { get; set; }
        public string ResolvedUrl { get; set; }
        public bool IsEnabled { get; set; }
    }

    private sealed class TextRowVm
    {
        public string Key { get; set; }
        public string Title { get; set; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        AdminAccessGuard_cl.RequireFeatureAccess("home_content", "/admin/default.aspx?mspace=content");

        Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
        AdminRolePolicy_cl.RequireContentManager();

        if (!IsPostBack)
        {
            string footerKey = (Request.QueryString["footer"] ?? "").Trim().ToLowerInvariant();
            string textKey = (Request.QueryString["text"] ?? "").Trim().ToLowerInvariant();
            BindAll(footerKey, textKey);
        }
    }

    protected void but_select_footer_Click(object sender, EventArgs e)
    {
        AdminRolePolicy_cl.RequireContentManager();
        LinkButton button = sender as LinkButton;
        string key = button == null ? "" : (button.CommandArgument ?? "").Trim().ToLowerInvariant();
        BindAll(key, (hf_text_content_key.Value ?? "").Trim().ToLowerInvariant());
    }

    protected void but_footer_new_Click(object sender, EventArgs e)
    {
        AdminRolePolicy_cl.RequireContentManager();
        BindAll("", (hf_text_content_key.Value ?? "").Trim().ToLowerInvariant());
    }

    protected void but_footer_save_Click(object sender, EventArgs e)
    {
        AdminRolePolicy_cl.RequireContentManager();

        string currentKey = (hf_footer_content_key.Value ?? "").Trim().ToLowerInvariant();
        string groupKey = (ddl_footer_group.SelectedValue ?? "").Trim();
        string displayName = (txt_footer_display_name.Text ?? "").Trim();
        string slugInput = (txt_footer_slug.Text ?? "").Trim();
        string targetUrl = (txt_footer_target_url.Text ?? "").Trim();
        string bodyContent = (txt_footer_body_content.Text ?? "").Trim();
        bool isEnabled = chk_footer_enabled.Checked;

        int sortOrder;
        if (!int.TryParse((txt_footer_sort_order.Text ?? "").Trim(), out sortOrder))
            sortOrder = 0;

        if (string.IsNullOrWhiteSpace(displayName))
        {
            ShowDialog("Tên hiển thị không được để trống.");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            string contentKey = currentKey;
            if (string.IsNullOrWhiteSpace(contentKey))
            {
                string baseKey = HomeFooterArticle_cl.BuildContentKey(groupKey, string.IsNullOrWhiteSpace(slugInput) ? displayName : slugInput);
                contentKey = BuildUniqueContentKey(db, baseKey);
            }

            HomeFooterArticle_cl.Upsert(
                db,
                contentKey,
                groupKey,
                displayName,
                slugInput,
                targetUrl,
                bodyContent,
                sortOrder,
                isEnabled,
                GetCurrentAdminUser());

            ShowNotifi("Đã lưu bài viết Footer.");
            BindAll(contentKey, (hf_text_content_key.Value ?? "").Trim().ToLowerInvariant());
        }
    }

    protected void but_select_text_block_Click(object sender, EventArgs e)
    {
        AdminRolePolicy_cl.RequireContentManager();
        LinkButton button = sender as LinkButton;
        string key = button == null ? "" : (button.CommandArgument ?? "").Trim().ToLowerInvariant();
        BindAll((hf_footer_content_key.Value ?? "").Trim().ToLowerInvariant(), key);
    }

    protected void but_text_save_Click(object sender, EventArgs e)
    {
        AdminRolePolicy_cl.RequireContentManager();

        string key = (hf_text_content_key.Value ?? "").Trim().ToLowerInvariant();
        if (!HomeTextContent_cl.IsValidKey(key))
        {
            ShowDialog("Không xác định được khối text cần lưu.");
            return;
        }

        string title = (txt_text_title.Text ?? "").Trim();
        string textContent = txt_text_content.Text ?? "";
        bool isEnabled = chk_text_enabled.Checked;

        using (dbDataContext db = new dbDataContext())
        {
            HomeTextContent_cl.Upsert(db, key, title, textContent, isEnabled, GetCurrentAdminUser());
            ShowNotifi("Đã lưu nội dung text trang chủ.");
            BindAll((hf_footer_content_key.Value ?? "").Trim().ToLowerInvariant(), key);
        }
    }

    private void BindAll(string preferredFooterKey, string preferredTextKey)
    {
        using (dbDataContext db = new dbDataContext())
        {
            BindFooterArea(db, preferredFooterKey);
            BindTextArea(db, preferredTextKey);
        }
    }

    private void BindFooterArea(dbDataContext db, string preferredFooterKey)
    {
        List<HomeFooterArticle_cl.FooterArticleItem> rows = HomeFooterArticle_cl.GetAll(db);
        string selectedKey = ResolveFooterSelection(rows, preferredFooterKey);

        rpt_footer_rows.DataSource = rows.Select(x => new FooterRowVm
        {
            ContentKey = x.ContentKey,
            GroupLabel = string.Equals(x.GroupKey, HomeFooterArticle_cl.GroupAbout, StringComparison.OrdinalIgnoreCase) ? "Về AhaSale" : "Hỗ trợ khách hàng",
            DisplayName = string.IsNullOrWhiteSpace(x.DisplayName) ? x.ContentKey : x.DisplayName,
            ResolvedUrl = HomeFooterArticle_cl.ResolveLinkUrl(x),
            IsEnabled = x.IsEnabled
        }).ToList();
        rpt_footer_rows.DataBind();

        BindFooterEditor(db, selectedKey);
    }

    private void BindFooterEditor(dbDataContext db, string selectedKey)
    {
        HomeFooterArticle_cl.FooterArticleItem item = string.IsNullOrWhiteSpace(selectedKey)
            ? null
            : HomeFooterArticle_cl.GetByContentKey(db, selectedKey);

        if (item == null)
        {
            hf_footer_content_key.Value = "";
            txt_footer_content_key.Text = "(tự sinh khi lưu)";
            ddl_footer_group.SelectedValue = HomeFooterArticle_cl.GroupSupport;
            txt_footer_display_name.Text = "";
            txt_footer_slug.Text = "";
            txt_footer_target_url.Text = "";
            txt_footer_body_content.Text = "";
            txt_footer_sort_order.Text = "0";
            chk_footer_enabled.Checked = true;
            lit_footer_updated_info.Text = "Bạn đang tạo link footer mới.";
            return;
        }

        hf_footer_content_key.Value = item.ContentKey;
        txt_footer_content_key.Text = item.ContentKey;
        ddl_footer_group.SelectedValue = HomeFooterArticle_cl.NormalizeGroupKey(item.GroupKey);
        txt_footer_display_name.Text = item.DisplayName;
        txt_footer_slug.Text = item.Slug;
        txt_footer_target_url.Text = item.TargetUrl;
        txt_footer_body_content.Text = item.BodyContent;
        txt_footer_sort_order.Text = item.SortOrder.ToString(CultureInfo.InvariantCulture);
        chk_footer_enabled.Checked = item.IsEnabled;

        if (item.UpdatedAt.HasValue)
        {
            lit_footer_updated_info.Text = "Cập nhật: <b>" + item.UpdatedAt.Value.ToString("dd/MM/yyyy HH:mm") + "</b> bởi <b>" + Server.HtmlEncode(item.UpdatedBy ?? "") + "</b>";
        }
        else
        {
            lit_footer_updated_info.Text = "Chưa có thông tin cập nhật.";
        }
    }

    private void BindTextArea(dbDataContext db, string preferredTextKey)
    {
        List<HomeTextContent_cl.TextContentItem> list = HomeTextContent_cl.GetAll(db);
        string selectedKey = ResolveTextSelection(list, preferredTextKey);

        rpt_text_blocks.DataSource = list.Select(x => new TextRowVm
        {
            Key = x.Key,
            Title = string.IsNullOrWhiteSpace(x.Title) ? x.Key : x.Title
        }).ToList();
        rpt_text_blocks.DataBind();

        BindTextEditor(db, selectedKey);
    }

    private void BindTextEditor(dbDataContext db, string selectedKey)
    {
        HomeTextContent_cl.TextContentItem item = HomeTextContent_cl.GetByKey(db, selectedKey);
        HomeTextContent_cl.TextContentItem effective = HomeTextContent_cl.GetEffectiveByKey(db, selectedKey);

        hf_text_content_key.Value = selectedKey;
        txt_text_content_key.Text = selectedKey;
        txt_text_title.Text = effective.Title;
        txt_text_content.Text = effective.TextContent;
        chk_text_enabled.Checked = item == null || item.IsEnabled;
        lit_text_guide.Text = HomeTextContent_cl.GetGuideText(selectedKey);

        if (item != null && item.UpdatedAt.HasValue)
        {
            lit_text_updated_info.Text = "Cập nhật: <b>" + item.UpdatedAt.Value.ToString("dd/MM/yyyy HH:mm") + "</b> bởi <b>" + Server.HtmlEncode(item.UpdatedBy ?? "") + "</b>";
        }
        else
        {
            lit_text_updated_info.Text = "Đang dùng nội dung mặc định hệ thống.";
        }
    }

    private string ResolveFooterSelection(List<HomeFooterArticle_cl.FooterArticleItem> rows, string preferredKey)
    {
        string key = (preferredKey ?? "").Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(key) && rows.Any(x => string.Equals(x.ContentKey, key, StringComparison.OrdinalIgnoreCase)))
            return key;
        return rows.Select(x => x.ContentKey).FirstOrDefault() ?? "";
    }

    private string ResolveTextSelection(List<HomeTextContent_cl.TextContentItem> rows, string preferredKey)
    {
        string key = (preferredKey ?? "").Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(key) && rows.Any(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase)))
            return key;
        return HomeTextContent_cl.KeyPopularKeywords;
    }

    private string BuildUniqueContentKey(dbDataContext db, string baseKey)
    {
        string normalizedBase = HomeFooterArticle_cl.NormalizeContentKey(baseKey);
        if (!HomeFooterArticle_cl.IsValidContentKey(normalizedBase))
            normalizedBase = "footer.support.item";

        string candidate = normalizedBase;
        int suffix = 1;
        while (HomeFooterArticle_cl.GetByContentKey(db, candidate) != null)
        {
            suffix++;
            candidate = normalizedBase + "_" + suffix.ToString(CultureInfo.InvariantCulture);
            if (candidate.Length > 120)
                candidate = candidate.Substring(0, 120).Trim('_');
        }
        return candidate;
    }

    private string GetCurrentAdminUser()
    {
        string encoded = Session["taikhoan"] as string;
        if (string.IsNullOrWhiteSpace(encoded)) return "";
        return mahoa_cl.giaima_Bcorn(encoded);
    }

    private void ShowDialog(string content)
    {
        ScriptManager.RegisterStartupScript(
            this.Page,
            this.GetType(),
            Guid.NewGuid().ToString(),
            thongbao_class.metro_notifi("Thông báo", content, "2600", "danger"),
            true);
    }

    private void ShowNotifi(string content)
    {
        ScriptManager.RegisterStartupScript(
            this.Page,
            this.GetType(),
            Guid.NewGuid().ToString(),
            thongbao_class.metro_notifi("Thông báo", content, "1000", "warning"),
            true);
    }
}
