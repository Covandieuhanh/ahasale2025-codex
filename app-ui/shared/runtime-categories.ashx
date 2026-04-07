<%@ WebHandler Language="C#" Class="AppUiRuntimeCategoriesHandler" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

public class AppUiRuntimeCategoriesHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();

        string space = ((context.Request["space"] ?? "home").Trim() ?? "home").ToLowerInvariant();

        try
        {
            if (space != "home")
            {
                context.Response.Write("{\"ok\":true,\"items\":[]}");
                return;
            }

            List<object> items = LoadHomeCategories();
            context.Response.Write(new JavaScriptSerializer().Serialize(new
            {
                ok = true,
                source = "runtime-home-categories",
                items = items
            }));
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "app_ui_runtime_categories", ex.StackTrace);
            context.Response.StatusCode = 500;
            context.Response.Write("{\"ok\":false,\"items\":[]}");
        }
    }

    public bool IsReusable { get { return false; } }

    private static List<object> LoadHomeCategories()
    {
        return SqlTransientGuard_cl.Execute(() =>
        {
            using (dbDataContext db = new dbDataContext())
            {
                DanhMuc_tb root = db.DanhMuc_tbs.FirstOrDefault(p =>
                    p.id_level == 1
                    && p.bin == false
                    && p.kyhieu_danhmuc == "web"
                    && (
                        (p.name != null && p.name.Trim().ToLower() == "danh mục")
                        || (p.name_en != null && (p.name_en.Trim().ToLower() == "danh-muc" || p.name_en.Trim().ToLower() == "danhmuc"))
                    ));

                IQueryable<DanhMuc_tb> query = root != null
                    ? db.DanhMuc_tbs.Where(p => p.id_parent == root.id.ToString() && p.bin == false && p.kyhieu_danhmuc == "web")
                    : db.DanhMuc_tbs.Where(p => p.id_level == 2 && p.bin == false && p.kyhieu_danhmuc == "web");

                return query
                    .OrderBy(p => p.rank)
                    .ToList()
                    .Select(p => new
                    {
                        id = p.id.ToString(),
                        label = (p.name ?? "").Trim(),
                        slug = ResolveSlug(p),
                        image = ResolveCategoryImage(p.image),
                        href = ResolveCategoryHref(p),
                        is_app_target = IsAppTarget(p)
                    })
                    .Cast<object>()
                    .ToList();
            }
        });
    }

    private static string ResolveSlug(DanhMuc_tb node)
    {
        string source = (node == null ? "" : ((node.name_en ?? "").Trim()));
        if (string.IsNullOrWhiteSpace(source))
            source = node == null ? "" : ((node.name ?? "").Trim());
        return BatDongSanService_cl.Slugify(source);
    }

    private static bool IsAppTarget(DanhMuc_tb node)
    {
        string href = ResolveCategoryHref(node);
        return !string.IsNullOrWhiteSpace(href)
            && href.StartsWith("/app-ui/", StringComparison.OrdinalIgnoreCase);
    }

    private static string ResolveCategoryHref(DanhMuc_tb node)
    {
        string slug = ResolveSlug(node);
        if (slug == "bat-dong-san" || slug == "nha-dat")
            return "/app-ui/batdongsan/default.aspx?ui_mode=app";
        if (slug.Contains("phuong-tien") || slug.Contains("xe"))
            return "/app-ui/choxe/default.aspx?ui_mode=app";
        if (slug.Contains("cong-nghe") || slug.Contains("thiet-bi-so") || slug.Contains("dien-thoai") || slug.Contains("may-tinh"))
            return "/app-ui/dienthoai-maytinh/default.aspx?ui_mode=app";

        string id = node == null ? "" : node.id.ToString();
        if (string.IsNullOrWhiteSpace(id))
            return "/app-ui/home/categories.aspx?ui_mode=app";

        string label = node == null ? "" : ((node.name ?? "").Trim());
        if (string.IsNullOrWhiteSpace(slug))
            slug = BatDongSanService_cl.Slugify(label);
        if (string.IsNullOrWhiteSpace(slug))
            return "/app-ui/home/categories.aspx?ui_mode=app";

        return "/app-ui/home/category.aspx?ui_mode=app"
            + "&slug=" + HttpUtility.UrlEncode(slug)
            + "&id=" + HttpUtility.UrlEncode(id)
            + "&label=" + HttpUtility.UrlEncode(label);
    }

    private static string ResolveCategoryImage(string imageRaw)
    {
        const string fallback = "/app-ui/assets/placeholder-media.svg";
        string image = (imageRaw ?? "").Trim();
        if (string.IsNullOrWhiteSpace(image))
            return fallback;

        if (image.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
            return fallback;

        Uri absolute;
        if (Uri.TryCreate(image, UriKind.Absolute, out absolute))
        {
            if (string.Equals(absolute.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                || string.Equals(absolute.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                return absolute.AbsoluteUri;
            return fallback;
        }

        if (image.StartsWith("~/", StringComparison.Ordinal))
            image = image.Substring(1);
        if (!image.StartsWith("/", StringComparison.Ordinal))
            image = "/" + image;

        if (Helper_cl.IsMissingUploadFile(image))
            return fallback;

        return image;
    }
}
