using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class GianHangArticle_cl
{
    public const string LoaiBaiVietChiTiet = "ctbv";
    public const string LoaiBaiVietDanhSach = "dsbv";

    public static IQueryable<GH_BaiViet_tb> QueryPublicByChiNhanh(dbDataContext db, string chiNhanhId)
    {
        return QueryPublicByChiNhanh(db, chiNhanhId, null);
    }

    public static IQueryable<GH_BaiViet_tb> QueryPublicByChiNhanh(dbDataContext db, string chiNhanhId, string storeAccountKey)
    {
        string normalizedChiNhanhId = (chiNhanhId ?? string.Empty).Trim();
        if (db == null || normalizedChiNhanhId == string.Empty)
            return Enumerable.Empty<GH_BaiViet_tb>().AsQueryable();

        GianHangArticleSync_cl.EnsureMirrorByChiNhanh(db, normalizedChiNhanhId, storeAccountKey);

        return db.GetTable<GH_BaiViet_tb>().Where(p =>
            p.id_chinhanh == normalizedChiNhanhId
            && (p.bin == null || p.bin == false)
            && (p.hienthi == null || p.hienthi == true)
            && p.phanloai == LoaiBaiVietChiTiet);
    }

    public static GH_BaiViet_tb FindPublicById(dbDataContext db, string chiNhanhId, string rawArticleId)
    {
        return FindPublicById(db, chiNhanhId, rawArticleId, null);
    }

    public static GH_BaiViet_tb FindPublicById(dbDataContext db, string chiNhanhId, string rawArticleId, string storeAccountKey)
    {
        string articleId = (rawArticleId ?? string.Empty).Trim();
        string normalizedChiNhanhId = (chiNhanhId ?? string.Empty).Trim();
        if (db == null || articleId == string.Empty || normalizedChiNhanhId == string.Empty)
            return null;

        GianHangArticleSync_cl.EnsureMirrorByChiNhanh(db, normalizedChiNhanhId, storeAccountKey);

        long nativeId;
        int legacyId;
        bool hasNativeId = long.TryParse(articleId, out nativeId);
        bool hasLegacyId = int.TryParse(articleId, out legacyId);

        return QueryPublicByChiNhanh(db, normalizedChiNhanhId, storeAccountKey)
            .FirstOrDefault(p =>
                (hasNativeId && p.id == nativeId)
                || (hasLegacyId && p.legacy_post_id == legacyId));
    }

    public static string ResolveDefaultMenuId(dbDataContext db, string chiNhanhId)
    {
        return ResolveDefaultMenuId(db, chiNhanhId, null);
    }

    public static string ResolveDefaultMenuId(dbDataContext db, string chiNhanhId, string storeAccountKey)
    {
        string configuredMenuId = GianHangStorefront_cl.ResolveDefaultMenuId(db, chiNhanhId, LoaiBaiVietDanhSach);
        if (!string.IsNullOrWhiteSpace(configuredMenuId))
            return configuredMenuId.Trim();

        string normalizedChiNhanhId = (chiNhanhId ?? string.Empty).Trim();
        if (db == null || normalizedChiNhanhId == string.Empty)
            return string.Empty;

        return QueryPublicByChiNhanh(db, normalizedChiNhanhId, storeAccountKey)
            .Select(p => p.id_category)
            .FirstOrDefault(p => p != null && p != "");
    }

    public static web_menu_table ResolveValidMenu(dbDataContext db, string chiNhanhId, string requestedMenuId)
    {
        web_menu_table menu = ResolveMenuById(db, chiNhanhId, requestedMenuId, LoaiBaiVietDanhSach);
        if (menu != null)
            return menu;

        string fallbackMenuId = ResolveDefaultMenuId(db, chiNhanhId);
        return ResolveMenuById(db, chiNhanhId, fallbackMenuId, LoaiBaiVietDanhSach);
    }

    public static string BuildListUrl(string menuId, string accountKey)
    {
        string url = GianHangRoutes_cl.BuildArticlesUrl(accountKey);
        string normalizedMenuId = (menuId ?? string.Empty).Trim();
        if (normalizedMenuId != string.Empty)
            url += (url.IndexOf('?') >= 0 ? "&" : "?") + "idmn=" + HttpUtility.UrlEncode(normalizedMenuId);
        return url;
    }

    public static string BuildDetailUrl(object rawId, string accountKey)
    {
        string articleId = Convert.ToString(rawId) ?? string.Empty;
        string normalizedId = articleId.Trim();
        if (normalizedId == string.Empty)
            return GianHangRoutes_cl.BuildStorefrontUrl(accountKey);

        string url = "/gianhang/page/chi-tiet-bai-viet.aspx?idbv=" + HttpUtility.UrlEncode(normalizedId);
        return GianHangPublic_cl.AppendUserToUrl(url, accountKey);
    }

    private static web_menu_table ResolveMenuById(dbDataContext db, string chiNhanhId, string rawMenuId, string expectedType)
    {
        return GianHangMenu_cl.ResolveByType(db, chiNhanhId, rawMenuId, expectedType);
    }

    public static long ResolveRouteId(GH_BaiViet_tb article)
    {
        if (article == null)
            return 0;
        if (article.legacy_post_id.HasValue && article.legacy_post_id.Value > 0)
            return article.legacy_post_id.Value;
        return article.id;
    }

}
