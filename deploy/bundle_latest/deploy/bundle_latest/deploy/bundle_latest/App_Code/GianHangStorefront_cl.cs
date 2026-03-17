using System;
using System.Linq;
using System.Web;

public static class GianHangStorefront_cl
{
    public const string MenuTypeService = "dsdv";
    public const string MenuTypeProduct = "dssp";
    public const string MenuTypeArticle = "dsbv";

    private const string DefaultServiceMenuId = "551";
    private const string DefaultProductMenuId = "550";
    private const string DefaultArticleMenuId = "577";

    public static string ServicesUrl
    {
        get { return AhaShineHomeRoutes_cl.BasePath + "/page/danh-sach-dich-vu.aspx"; }
    }

    public static string ProductsUrl
    {
        get { return AhaShineHomeRoutes_cl.BasePath + "/page/danh-sach-san-pham.aspx"; }
    }

    public static string ArticlesUrl
    {
        get { return AhaShineHomeRoutes_cl.BasePath + "/page/danh-sach-bai-viet.aspx"; }
    }

    public static string ResolveDefaultMenuId(string phanloai)
    {
        string chiNhanhId = AhaShineContext_cl.ResolveChiNhanhId();
        string normalizedType = (phanloai ?? string.Empty).Trim().ToLowerInvariant();
        string fallbackId = DefaultArticleMenuId;

        if (normalizedType == MenuTypeService)
            fallbackId = DefaultServiceMenuId;
        else if (normalizedType == MenuTypeProduct)
            fallbackId = DefaultProductMenuId;

        using (dbDataContext db = new dbDataContext())
        {
            var menus = db.web_menu_tables
                .Where(p => p.bin == false && p.id_chinhanh == chiNhanhId)
                .OrderBy(p => p.rank)
                .ThenBy(p => p.id)
                .ToList();

            var reserved = menus.FirstOrDefault(p => p.id.ToString() == fallbackId);
            if (reserved != null)
                return reserved.id.ToString();

            var topLevel = menus.FirstOrDefault(p => string.Equals((p.phanloai ?? string.Empty).Trim(), normalizedType, StringComparison.OrdinalIgnoreCase) && p.id_parent == "0");
            if (topLevel != null)
                return topLevel.id.ToString();

            var anyLevel = menus.FirstOrDefault(p => string.Equals((p.phanloai ?? string.Empty).Trim(), normalizedType, StringComparison.OrdinalIgnoreCase));
            if (anyLevel != null)
                return anyLevel.id.ToString();
        }

        return string.Empty;
    }

    public static string ResolveMenuUrl(object idObject, object phanloaiObject, object urlOtherObject)
    {
        string urlOther = (urlOtherObject ?? string.Empty).ToString().Trim();
        if (urlOther != string.Empty)
            return NormalizeStandaloneUrl(urlOther);

        string id = (idObject ?? string.Empty).ToString().Trim();
        string phanloai = (phanloaiObject ?? string.Empty).ToString().Trim().ToLowerInvariant();
        if (id == string.Empty)
            return AhaShineHomeRoutes_cl.HomeUrl;

        return ResolveCategoryUrl(id, phanloai);
    }

    public static string ResolveCategoryUrl(string id, string phanloai)
    {
        string normalizedId = (id ?? string.Empty).Trim();
        string normalizedType = (phanloai ?? string.Empty).Trim().ToLowerInvariant();
        if (normalizedId == string.Empty)
            return AhaShineHomeRoutes_cl.HomeUrl;

        if (normalizedType == MenuTypeService)
            return ServicesUrl + "?idmn=" + HttpUtility.UrlEncode(normalizedId);
        if (normalizedType == MenuTypeProduct)
            return ProductsUrl + "?idmn=" + HttpUtility.UrlEncode(normalizedId);

        return ArticlesUrl + "?idmn=" + HttpUtility.UrlEncode(normalizedId);
    }

    public static string NormalizeStandaloneUrl(string rawUrl)
    {
        string value = (rawUrl ?? string.Empty).Trim();
        if (value == string.Empty)
            return AhaShineHomeRoutes_cl.HomeUrl;

        string lower = value.ToLowerInvariant();
        if (lower.StartsWith("http://") || lower.StartsWith("https://") || lower.StartsWith("//")
            || lower.StartsWith("mailto:") || lower.StartsWith("tel:") || lower.StartsWith("javascript:") || lower.StartsWith("#"))
            return value;

        if (lower.StartsWith("/gianhang"))
            return value;

        if (lower.StartsWith("/page/") || lower.StartsWith("/tai-khoan/") || lower.StartsWith("/webcon/")
            || lower.StartsWith("/chi-tiet-") || lower.StartsWith("/datlich") || lower.StartsWith("/giohang")
            || lower.StartsWith("/hoa-don-dien-tu"))
            return AhaShineHomeRoutes_cl.BasePath + value;

        return AhaShineHomeRoutes_cl.NormalizeReturnUrl(value);
    }
}
