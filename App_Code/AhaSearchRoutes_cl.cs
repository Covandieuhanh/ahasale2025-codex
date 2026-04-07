using System;
using System.Collections.Specialized;
using System.Web;

public static class AhaSearchRoutes_cl
{
    private static readonly string_class SlugHelper = new string_class();

    public static string BuildSearchUrl(
        string keyword,
        string categoryId,
        string locationId,
        string province,
        string district,
        string ward,
        string type,
        string page)
    {
        string safeKeyword = (keyword ?? string.Empty).Trim();
        string safeCategoryId = (categoryId ?? string.Empty).Trim();
        string safeLocationId = (locationId ?? string.Empty).Trim();
        string safeProvince = (province ?? string.Empty).Trim();
        string safeDistrict = (district ?? string.Empty).Trim();
        string safeWard = (ward ?? string.Empty).Trim();
        string safeType = string.IsNullOrWhiteSpace(type) ? "all" : type.Trim().ToLowerInvariant();
        string safePage = string.IsNullOrWhiteSpace(page) ? "1" : page.Trim();

        if (string.IsNullOrWhiteSpace(safeProvince))
            safeProvince = safeLocationId;

        string path = "/tim-kiem";
        string slug = ToSearchSlug(safeKeyword);
        if (!string.IsNullOrWhiteSpace(slug))
            path += "/" + slug;

        NameValueCollection qs = HttpUtility.ParseQueryString(string.Empty);
        if (!string.IsNullOrWhiteSpace(safeKeyword)) qs["q"] = safeKeyword;
        if (!string.IsNullOrWhiteSpace(safeCategoryId)) qs["cat"] = safeCategoryId;
        if (!string.IsNullOrWhiteSpace(safeLocationId)) qs["loc"] = safeLocationId;
        if (!string.IsNullOrWhiteSpace(safeProvince)) qs["province"] = safeProvince;
        if (!string.IsNullOrWhiteSpace(safeDistrict)) qs["district"] = safeDistrict;
        if (!string.IsNullOrWhiteSpace(safeWard)) qs["ward"] = safeWard;
        qs["type"] = safeType;
        qs["page"] = safePage;

        string query = qs.ToString();
        return path + (string.IsNullOrWhiteSpace(query) ? string.Empty : "?" + query);
    }

    public static string ToSearchSlug(string keyword)
    {
        string safeKeyword = (keyword ?? string.Empty).Trim();
        if (safeKeyword == string.Empty)
            return string.Empty;

        string slug = string.Empty;
        try
        {
            slug = (SlugHelper.replace_name_to_url(safeKeyword) ?? string.Empty).Trim().ToLowerInvariant();
        }
        catch
        {
            slug = string.Empty;
        }

        if (slug == string.Empty)
            slug = "tim-kiem";

        return slug;
    }

    public static string ToKeywordFromSlug(string slug)
    {
        string safeSlug = (slug ?? string.Empty).Trim().Trim('/');
        if (safeSlug == string.Empty)
            return string.Empty;

        return safeSlug.Replace("-", " ").Trim();
    }
}
