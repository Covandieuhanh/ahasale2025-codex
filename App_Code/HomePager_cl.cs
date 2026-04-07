using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

public static class HomePager_cl
{
    public static int ResolvePage(HttpRequest request, string key = "page")
    {
        if (request == null)
            return 1;

        int page;
        if (int.TryParse((request.QueryString[key] ?? "").Trim(), out page) && page > 0)
            return page;

        return 1;
    }

    public static string BuildPageUrl(HttpRequest request, int page, string key = "page")
    {
        if (request == null)
            return "#";

        if (page < 1)
            page = 1;

        NameValueCollection query = HttpUtility.ParseQueryString(request.QueryString.ToString());
        query[key] = page.ToString();

        string path = request.Url != null ? request.Url.AbsolutePath : (request.Path ?? "/");
        string queryText = query.ToString();
        return string.IsNullOrEmpty(queryText) ? path : (path + "?" + queryText);
    }

    public static string RenderPager(HttpRequest request, int currentPage, int totalPages, string key = "page")
    {
        if (totalPages <= 1)
            return string.Empty;

        if (currentPage < 1)
            currentPage = 1;
        if (currentPage > totalPages)
            currentPage = totalPages;

        var html = new StringBuilder();
        html.Append("<nav aria-label='Phan trang' class='mt-2'>");
        html.Append("<ul class='pagination pagination-sm mb-0'>");

        AppendNavItem(html, request, currentPage > 1 ? currentPage - 1 : 1, currentPage > 1, "&laquo;", key);

        int window = 2;
        int start = Math.Max(1, currentPage - window);
        int end = Math.Min(totalPages, currentPage + window);

        if (start > 1)
        {
            AppendPageItem(html, request, 1, false, key);
            if (start > 2)
                AppendDots(html);
        }

        for (int i = start; i <= end; i++)
            AppendPageItem(html, request, i, i == currentPage, key);

        if (end < totalPages)
        {
            if (end < totalPages - 1)
                AppendDots(html);
            AppendPageItem(html, request, totalPages, false, key);
        }

        AppendNavItem(html, request, currentPage < totalPages ? currentPage + 1 : totalPages, currentPage < totalPages, "&raquo;", key);

        html.Append("</ul>");
        html.Append("</nav>");
        return html.ToString();
    }

    private static void AppendPageItem(StringBuilder html, HttpRequest request, int page, bool active, string key)
    {
        if (active)
        {
            html.Append("<li class='page-item active' aria-current='page'><span class='page-link'>");
            html.Append(page.ToString());
            html.Append("</span></li>");
            return;
        }

        html.Append("<li class='page-item'><a class='page-link' href='");
        html.Append(HttpUtility.HtmlAttributeEncode(BuildPageUrl(request, page, key)));
        html.Append("'>");
        html.Append(page.ToString());
        html.Append("</a></li>");
    }

    private static void AppendNavItem(StringBuilder html, HttpRequest request, int page, bool enabled, string labelHtml, string key)
    {
        if (!enabled)
        {
            html.Append("<li class='page-item disabled'><span class='page-link'>");
            html.Append(labelHtml);
            html.Append("</span></li>");
            return;
        }

        html.Append("<li class='page-item'><a class='page-link' href='");
        html.Append(HttpUtility.HtmlAttributeEncode(BuildPageUrl(request, page, key)));
        html.Append("'>");
        html.Append(labelHtml);
        html.Append("</a></li>");
    }

    private static void AppendDots(StringBuilder html)
    {
        html.Append("<li class='page-item disabled'><span class='page-link'>...</span></li>");
    }
}
