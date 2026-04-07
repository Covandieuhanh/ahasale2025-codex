<%@ WebHandler Language="C#" Class="AppUiRuntimeProfileLinksHandler" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

public class AppUiRuntimeProfileLinksHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();

        JavaScriptSerializer serializer = new JavaScriptSerializer();

        if (!string.Equals(context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = 405;
            context.Response.Write(serializer.Serialize(new
            {
                ok = false,
                message = "Phương thức không hợp lệ."
            }));
            return;
        }

        try
        {
            RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetCurrentInfo();
            if (info == null || !info.IsAuthenticated || string.IsNullOrWhiteSpace(info.AccountKey))
            {
                context.Response.StatusCode = 401;
                context.Response.Write(serializer.Serialize(new
                {
                    ok = false,
                    message = "Bạn cần đăng nhập để thao tác Bio Link."
                }));
                return;
            }

            string action = (context.Request["action"] ?? "").Trim().ToLowerInvariant();
            object payload = CoreDb_cl.Use(db => HandleAction(db, info, action, context.Request));
            context.Response.Write(serializer.Serialize(payload));
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "app_ui_runtime_profile_links", ex.StackTrace);
            context.Response.StatusCode = 500;
            context.Response.Write(serializer.Serialize(new
            {
                ok = false,
                message = "Lỗi xử lý Bio Link. Vui lòng thử lại."
            }));
        }
    }

    public bool IsReusable { get { return false; } }

    private static object HandleAction(dbDataContext db, RootAccount_cl.RootAccountInfo info, string action, HttpRequest request)
    {
        taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, info.AccountKey);
        if (account == null || string.IsNullOrWhiteSpace(account.taikhoan))
        {
            return new
            {
                ok = false,
                message = "Không tìm thấy tài khoản."
            };
        }

        if (action == "add")
            return HandleAdd(db, account, request);
        if (action == "delete")
            return HandleDelete(db, account, request);

        return new
        {
            ok = false,
            message = "Hành động không hợp lệ."
        };
    }

    private static object HandleAdd(dbDataContext db, taikhoan_tb account, HttpRequest request)
    {
        string rawTitle = (request["title"] ?? "").Trim();
        string rawLink = (request["url"] ?? "").Trim();
        string normalizedLink = SocialLinkIcon_cl.NormalizeExternalLink(rawLink);
        if (string.IsNullOrWhiteSpace(normalizedLink))
        {
            return new
            {
                ok = false,
                message = "URL liên kết chưa hợp lệ."
            };
        }

        int currentCount = db.MangXaHoi_tbs.Count(x => x.TaiKhoan == account.taikhoan && x.Kieu == "Cá nhân");
        if (currentCount >= 30)
        {
            return new
            {
                ok = false,
                message = "Bạn đã đạt giới hạn 30 Bio Link."
            };
        }

        string title = string.IsNullOrWhiteSpace(rawTitle) ? ResolveExternalLinkLabel(normalizedLink) : rawTitle;
        if (title.Length > 80)
            title = title.Substring(0, 80).Trim();

        string icon = SocialLinkIcon_cl.ResolveIconForSave(normalizedLink, "", "");
        MangXaHoi_tb row = new MangXaHoi_tb
        {
            TaiKhoan = account.taikhoan,
            Kieu = "Cá nhân",
            Ten = title,
            Link = normalizedLink,
            Icon = icon
        };
        db.MangXaHoi_tbs.InsertOnSubmit(row);
        db.SubmitChanges();

        return BuildSuccessPayload(db, account.taikhoan, "Đã thêm Bio Link.");
    }

    private static object HandleDelete(dbDataContext db, taikhoan_tb account, HttpRequest request)
    {
        long id;
        if (!long.TryParse((request["id"] ?? "").Trim(), out id) || id <= 0)
        {
            return new
            {
                ok = false,
                message = "ID link không hợp lệ."
            };
        }

        MangXaHoi_tb row = db.MangXaHoi_tbs.FirstOrDefault(x => x.id == id && x.TaiKhoan == account.taikhoan && x.Kieu == "Cá nhân");
        if (row == null)
        {
            return new
            {
                ok = false,
                message = "Không tìm thấy link để xóa."
            };
        }

        db.MangXaHoi_tbs.DeleteOnSubmit(row);
        db.SubmitChanges();

        HomeProfileSetting_cl.ProfileSettings settings = HomeProfileSetting_cl.GetSettings(db, account.taikhoan, ShopSlug_cl.IsShopAccount(db, account));
        List<MangXaHoi_tb> linksNow = db.MangXaHoi_tbs
            .Where(x => x.TaiKhoan == account.taikhoan && x.Kieu == "Cá nhân")
            .ToList();
        settings.SocialOrderPersonal = SocialLinkOrder_cl.Normalize(settings.SocialOrderPersonal, linksNow);
        HomeProfileSetting_cl.Upsert(db, account.taikhoan, settings, account.taikhoan, ShopSlug_cl.IsShopAccount(db, account));

        return BuildSuccessPayload(db, account.taikhoan, "Đã xóa Bio Link.");
    }

    private static object BuildSuccessPayload(dbDataContext db, string taiKhoan, string message)
    {
        HomeProfileSetting_cl.ProfileSettings settings = HomeProfileSetting_cl.GetSettings(db, taiKhoan, false);
        List<MangXaHoi_tb> links = db.MangXaHoi_tbs
            .Where(x => x.TaiKhoan == taiKhoan && x.Kieu == "Cá nhân")
            .ToList();
        List<MangXaHoi_tb> sorted = SocialLinkOrder_cl.SortLinks(links, settings.SocialOrderPersonal);
        List<object> mapped = sorted
            .Select(MapSocialLink)
            .Where(x => x != null)
            .Cast<object>()
            .ToList();

        return new
        {
            ok = true,
            message = message,
            links = mapped,
            social_count = mapped.Count
        };
    }

    private static object MapSocialLink(MangXaHoi_tb row)
    {
        if (row == null)
            return null;

        string href = ResolveExternalLink(row.Link);
        if (href == "#")
            return null;

        string icon = ResolveSocialIcon(row.Icon, row.Link);
        string title = string.IsNullOrWhiteSpace(row.Ten) ? ResolveExternalLinkLabel(row.Link) : row.Ten.Trim();
        return new
        {
            id = row.id,
            title = title,
            host = ResolveExternalLinkLabel(row.Link),
            href = href,
            icon = icon
        };
    }

    private static string ResolveExternalLink(string linkRaw)
    {
        string link = (linkRaw ?? "").Trim();
        if (string.IsNullOrEmpty(link))
            return "#";
        if (link.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
            return "#";

        Uri absolute;
        if (!Uri.TryCreate(link, UriKind.Absolute, out absolute))
        {
            string normalized = "https://" + link.TrimStart('/');
            if (!Uri.TryCreate(normalized, UriKind.Absolute, out absolute))
                return "#";
        }

        if (!string.Equals(absolute.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(absolute.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            return "#";

        return absolute.AbsoluteUri;
    }

    private static string ResolveExternalLinkLabel(string linkRaw)
    {
        string link = (linkRaw ?? "").Trim();
        if (string.IsNullOrEmpty(link))
            return "Liên kết";

        Uri absolute;
        if (Uri.TryCreate(link, UriKind.Absolute, out absolute))
            return absolute.Host;

        string compact = link.TrimStart('/');
        int slash = compact.IndexOf('/');
        if (slash > 0)
            return compact.Substring(0, slash);
        return compact;
    }

    private static string ResolveSocialIcon(string iconRaw, string linkRaw)
    {
        string explicitIcon = (iconRaw ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(explicitIcon))
            return explicitIcon;

        return SocialLinkIcon_cl.ResolveIconForDisplay("", linkRaw ?? "");
    }
}
