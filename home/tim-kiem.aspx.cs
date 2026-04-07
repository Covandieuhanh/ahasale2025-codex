using System;
using System.Web;

public partial class home_tim_kiem : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ApplySearchMeta();
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        ApplySearchMeta();
    }

    private void ApplySearchMeta()
    {
        string keyword = ResolveKeyword();
        string categoryId = (Request.QueryString["cat"] ?? "").Trim();
        string province = (Request.QueryString["province"] ?? Request.QueryString["loc"] ?? "").Trim();
        string district = (Request.QueryString["district"] ?? "").Trim();
        string ward = (Request.QueryString["ward"] ?? "").Trim();
        string locationLevel = ResolveLocationLevel(province, district, ward);

        string summary = BuildSummary(keyword, categoryId, province, district, ward, locationLevel);
        string pageTitle = "Tìm kiếm";
        if (!string.IsNullOrWhiteSpace(keyword))
            pageTitle = "Tìm kiếm: " + keyword.Trim();
        int resultCount = UcSearch != null ? UcSearch.ResultCount : 0;

        Page.Title = pageTitle;
        UcSearch.TitleText = "Kết quả tìm kiếm";

        litSearchSummary.Text = HttpUtility.HtmlEncode(summary);
        phSearchSummary.Visible = !string.IsNullOrWhiteSpace(summary);
        string safeKeywordJs = HttpUtility.JavaScriptStringEncode(keyword ?? "");
        string safeCategoryJs = HttpUtility.JavaScriptStringEncode(categoryId ?? "");
        string safeProvinceJs = HttpUtility.JavaScriptStringEncode(province ?? "");
        string safeDistrictJs = HttpUtility.JavaScriptStringEncode(district ?? "");
        string safeWardJs = HttpUtility.JavaScriptStringEncode(ward ?? "");
        string safeLocationLevelJs = HttpUtility.JavaScriptStringEncode(locationLevel ?? "none");
        litSearchMeta.Text =
            "<meta name=\"robots\" content=\"index,follow\" />" +
            "<script>" +
            "(function(){try{" +
            "var dl=window.dataLayer;" +
            "if(!dl||typeof dl.push!=='function')return;" +
            "dl.push({event:'track_event',event_category:'search_box',event_action:'search_results_view',event_label:'" + safeKeywordJs + "',event_model:{keyword:'" + safeKeywordJs + "',cat:'" + safeCategoryJs + "',province:'" + safeProvinceJs + "',district:'" + safeDistrictJs + "',ward:'" + safeWardJs + "',location_level:'" + safeLocationLevelJs + "',result_count:" + resultCount.ToString() + "}});" +
            (resultCount == 0 && !string.IsNullOrWhiteSpace(keyword)
                ? "dl.push({event:'track_event',event_category:'search_box',event_action:'search_no_result',event_label:'" + safeKeywordJs + "',event_model:{keyword:'" + safeKeywordJs + "',cat:'" + safeCategoryJs + "',province:'" + safeProvinceJs + "',district:'" + safeDistrictJs + "',ward:'" + safeWardJs + "',location_level:'" + safeLocationLevelJs + "'}});"
                : "") +
            "}catch(e){}})();" +
            "</script>";
    }

    private string ResolveKeyword()
    {
        string keyword = (Request.QueryString["q"] ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(keyword))
            return HttpUtility.UrlDecode(keyword);

        string qSlug = (Request.QueryString["qslug"] ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(qSlug))
            return AhaSearchRoutes_cl.ToKeywordFromSlug(qSlug);

        return string.Empty;
    }

    private string BuildSummary(string keyword, string categoryId, string province, string district, string ward, string locationLevel)
    {
        string summary = "";

        if (!string.IsNullOrWhiteSpace(keyword))
            summary = "Tu khoa: " + keyword.Trim();

        if (!string.IsNullOrWhiteSpace(categoryId))
            summary = AppendPart(summary, "Danh muc ID: " + categoryId);

        if (!string.IsNullOrWhiteSpace(province))
            summary = AppendPart(summary, "Tinh ID: " + province);
        if (!string.IsNullOrWhiteSpace(district))
            summary = AppendPart(summary, "Huyen ID: " + district);
        if (!string.IsNullOrWhiteSpace(ward))
            summary = AppendPart(summary, "Xa ID: " + ward);
        if (!string.IsNullOrWhiteSpace(locationLevel) && !string.Equals(locationLevel, "none", StringComparison.OrdinalIgnoreCase))
            summary = AppendPart(summary, "Cap dia diem: " + locationLevel);

        if (UcSearch != null && !string.IsNullOrWhiteSpace(UcSearch.SearchSummary))
            summary = UcSearch.SearchSummary;

        if (UcSearch != null)
            summary = AppendPart(summary, "So ket qua: " + UcSearch.ResultCount.ToString());

        return summary;
    }

    private static string ResolveLocationLevel(string province, string district, string ward)
    {
        if (!string.IsNullOrWhiteSpace(ward))
            return "ward";
        if (!string.IsNullOrWhiteSpace(district))
            return "district";
        if (!string.IsNullOrWhiteSpace(province))
            return "province";
        return "none";
    }

    private static string AppendPart(string summary, string part)
    {
        if (string.IsNullOrWhiteSpace(part))
            return summary ?? "";
        if (string.IsNullOrWhiteSpace(summary))
            return part.Trim();
        if (summary.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0)
            return summary;
        return summary.Trim() + " | " + part.Trim();
    }
}
