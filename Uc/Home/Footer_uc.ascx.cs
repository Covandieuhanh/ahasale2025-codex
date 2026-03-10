using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class Uc_Home_Footer_uc : System.Web.UI.UserControl
{
    private sealed class FooterLinkVm
    {
        public string DisplayName { get; set; }
        public string Url { get; set; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            BindFooterLinks();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    private void BindFooterLinks()
    {
        using (dbDataContext db = new dbDataContext())
        {
            List<FooterLinkVm> support = HomeFooterArticle_cl
                .GetEnabledByGroup(db, HomeFooterArticle_cl.GroupSupport)
                .Select(ToVm)
                .ToList();

            List<FooterLinkVm> about = HomeFooterArticle_cl
                .GetEnabledByGroup(db, HomeFooterArticle_cl.GroupAbout)
                .Select(ToVm)
                .ToList();

            rpt_support_links.DataSource = support;
            rpt_support_links.DataBind();

            rpt_about_links.DataSource = about;
            rpt_about_links.DataBind();

            BindFooterTextSettings(db);
        }
    }

    private void BindFooterTextSettings(dbDataContext db)
    {
        string contactEmail = GetTextBlock(db, HomeTextContent_cl.KeyFooterContactEmail, "ahasale.vn@gmail.com");
        string contactHotline = GetTextBlock(db, HomeTextContent_cl.KeyFooterContactHotline, "0868.877.686");
        string contactAddress = GetTextBlock(db, HomeTextContent_cl.KeyFooterContactAddress, "Số 46/3, đường Võ Thị Sáu, khu phố Gò Me, Phường Trấn Biên, Tỉnh Đồng Nai, Việt Nam");
        string legalLine = GetTextBlock(db, HomeTextContent_cl.KeyFooterLegalLine, "CÔNG TY CỔ PHẦN ĐÀO TẠO AHA SALE - Người đại diện theo pháp luật: Trần Đức Cường; GPKDKD: 3603907499 do Sở tài chính tỉnh Đồng Nai cấp ngày 29/03/2023;");
        string policyText = GetTextBlock(db, HomeTextContent_cl.KeyFooterPolicyUsageText, "Chính sách sử dụng");
        string policyUrl = GetTextBlock(db, HomeTextContent_cl.KeyFooterPolicyUsageUrl, "/home/noi-dung-footer.aspx?slug=quy-che-hoat-dong-san");
        string linkedinUrl = GetTextBlock(db, HomeTextContent_cl.KeyFooterSocialLinkedin, "#");
        string youtubeUrl = GetTextBlock(db, HomeTextContent_cl.KeyFooterSocialYoutube, "#");
        string facebookUrl = GetTextBlock(db, HomeTextContent_cl.KeyFooterSocialFacebook, "#");

        lnk_contact_email.Text = Server.HtmlEncode(contactEmail);
        lnk_contact_email.NavigateUrl = "mailto:" + contactEmail;

        lit_contact_hotline.Text = Server.HtmlEncode(contactHotline);
        lit_contact_address.Text = Server.HtmlEncode(contactAddress);

        lit_footer_legal_line.Text = Server.HtmlEncode(legalLine);

        lnk_policy_usage.Text = Server.HtmlEncode(policyText);
        lnk_policy_usage.NavigateUrl = NormalizePublicUrl(policyUrl, "/home/noi-dung-footer.aspx?slug=quy-che-hoat-dong-san");

        lnk_social_linkedin.NavigateUrl = NormalizePublicUrl(linkedinUrl, "#");
        lnk_social_youtube.NavigateUrl = NormalizePublicUrl(youtubeUrl, "#");
        lnk_social_facebook.NavigateUrl = NormalizePublicUrl(facebookUrl, "#");
    }

    private string GetTextBlock(dbDataContext db, string key, string defaultValue)
    {
        HomeTextContent_cl.TextContentItem item = HomeTextContent_cl.GetEffectiveByKey(db, key);
        if (item == null || !item.IsEnabled)
            return defaultValue;

        string value = (item.TextContent ?? "").Trim();
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;
        return value;
    }

    private string NormalizePublicUrl(string rawUrl, string fallback)
    {
        string url = (rawUrl ?? "").Trim();
        if (string.IsNullOrWhiteSpace(url))
            return fallback;
        if (url.StartsWith("/", StringComparison.Ordinal))
            return url;
        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            return url;
        if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return url;
        return fallback;
    }

    private FooterLinkVm ToVm(HomeFooterArticle_cl.FooterArticleItem item)
    {
        return new FooterLinkVm
        {
            DisplayName = (item == null || string.IsNullOrWhiteSpace(item.DisplayName)) ? "Nội dung" : item.DisplayName,
            Url = HomeFooterArticle_cl.ResolveLinkUrl(item)
        };
    }
}
