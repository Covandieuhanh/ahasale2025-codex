using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangArticleView_cl
{
    public sealed class ArticleCardItem
    {
        public long id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string description { get; set; }
        public DateTime? ngaytao { get; set; }
    }

    public sealed class ArticleListPageState
    {
        public web_menu_table Menu { get; set; }
        public List<ArticleCardItem> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public bool CanGoBack { get; set; }
        public bool CanLoadMore { get; set; }
    }

    public sealed class ArticleDetailPageState
    {
        public GH_BaiViet_tb Article { get; set; }
        public web_menu_table Menu { get; set; }
        public string MenuUrl { get; set; }
        public List<ArticleCardItem> RelatedArticles { get; set; }
    }

    public static ArticleListPageState BuildListPageState(
        dbDataContext db,
        string chiNhanhId,
        string storeAccountKey,
        string requestedMenuId,
        string searchText,
        int currentPage,
        int pageSize)
    {
        if (db == null)
            return null;

        web_menu_table menu = GianHangArticle_cl.ResolveValidMenu(db, chiNhanhId, requestedMenuId);
        if (menu == null)
            return null;

        List<ArticleCardItem> allItems = GianHangArticle_cl.QueryPublicByChiNhanh(db, chiNhanhId, storeAccountKey)
            .Where(p => p.id_category == menu.id.ToString())
            .Select(p => new ArticleCardItem
            {
                id = p.legacy_post_id.HasValue && p.legacy_post_id.Value > 0 ? p.legacy_post_id.Value : p.id,
                name = p.name ?? string.Empty,
                image = p.image ?? string.Empty,
                description = p.description ?? string.Empty,
                ngaytao = p.ngaytao
            })
            .OrderByDescending(p => p.ngaytao)
            .ToList();

        string normalizedSearch = (searchText ?? string.Empty).Trim().ToLowerInvariant();
        if (normalizedSearch != string.Empty)
        {
            allItems = allItems.Where(p =>
                    (p.name ?? string.Empty).ToLowerInvariant().Contains(normalizedSearch)
                    || p.id.ToString() == normalizedSearch
                    || (p.description ?? string.Empty).ToLowerInvariant().Contains(normalizedSearch))
                .ToList();
        }

        int safePageSize = pageSize > 0 ? pageSize : 20;
        int totalPage = number_of_page_class.return_total_page(allItems.Count, safePageSize);
        int safeCurrentPage = currentPage;
        if (safeCurrentPage < 1)
            safeCurrentPage = 1;
        if (totalPage > 0 && safeCurrentPage > totalPage)
            safeCurrentPage = totalPage;
        if (totalPage <= 0)
            safeCurrentPage = 1;

        List<ArticleCardItem> pageItems = allItems.Skip((safeCurrentPage - 1) * safePageSize).Take(safePageSize).ToList();

        return new ArticleListPageState
        {
            Menu = menu,
            Items = pageItems,
            CurrentPage = safeCurrentPage,
            TotalPage = totalPage,
            CanGoBack = safeCurrentPage > 1,
            CanLoadMore = safeCurrentPage < totalPage
        };
    }

    public static ArticleDetailPageState BuildDetailPageState(
        dbDataContext db,
        string chiNhanhId,
        string articleId,
        string storeAccountKey)
    {
        if (db == null)
            return null;

        GH_BaiViet_tb article = GianHangArticle_cl.FindPublicById(db, chiNhanhId, articleId, storeAccountKey);
        if (article == null)
            return null;

        web_menu_table menu = GianHangMenu_cl.FindById(db, chiNhanhId, article.id_category);
        if (menu == null)
            return null;

        string menuType = (menu.phanloai ?? string.Empty).Trim();
        string menuUrl;
        if (string.Equals(menuType, GianHangStorefront_cl.MenuTypeService, StringComparison.OrdinalIgnoreCase))
            menuUrl = GianHangRoutes_cl.BuildServiceCategoryUrl(storeAccountKey, article.id_category);
        else if (string.Equals(menuType, GianHangStorefront_cl.MenuTypeProduct, StringComparison.OrdinalIgnoreCase))
            menuUrl = GianHangRoutes_cl.BuildProductCategoryUrl(storeAccountKey, article.id_category);
        else
            menuUrl = GianHangArticle_cl.BuildListUrl(article.id_category, storeAccountKey);

        List<ArticleCardItem> relatedArticles = GianHangArticle_cl.QueryPublicByChiNhanh(db, chiNhanhId, storeAccountKey)
            .Where(p => p.id_category == article.id_category && p.id != article.id)
            .OrderByDescending(p => p.ngaytao)
            .Take(9)
            .Select(p => new ArticleCardItem
            {
                id = p.legacy_post_id.HasValue && p.legacy_post_id.Value > 0 ? p.legacy_post_id.Value : p.id,
                name = p.name ?? string.Empty,
                image = p.image ?? string.Empty,
                description = p.description ?? string.Empty,
                ngaytao = p.ngaytao
            })
            .ToList();

        return new ArticleDetailPageState
        {
            Article = article,
            Menu = menu,
            MenuUrl = menuUrl,
            RelatedArticles = relatedArticles
        };
    }
}
