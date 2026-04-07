<%@ WebHandler Language="C#" Class="AppUiRuntimeProfileHandler" %>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

public class AppUiRuntimeProfileHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();

        try
        {
            RootAccount_cl.RootAccountInfo info = RootAccount_cl.GetCurrentInfo();
            if (info == null || !info.IsAuthenticated || string.IsNullOrWhiteSpace(info.AccountKey))
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new
                {
                    ok = false,
                    reason = "unauthorized",
                    profile = (object)null,
                    social_links = new object[0]
                }));
                return;
            }

            object payload = CoreDb_cl.Use(db => BuildPayload(context, db, info));
            context.Response.Write(new JavaScriptSerializer().Serialize(payload));
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "app_ui_runtime_profile", ex.StackTrace);
            context.Response.StatusCode = 500;
            context.Response.Write("{\"ok\":false,\"reason\":\"error\",\"profile\":null,\"social_links\":[]}");
        }
    }

    public bool IsReusable { get { return false; } }

    private static object BuildPayload(HttpContext context, dbDataContext db, RootAccount_cl.RootAccountInfo info)
    {
        taikhoan_tb account = ResolveAccount(db, info);
        if (account == null || string.IsNullOrWhiteSpace(account.taikhoan))
        {
            return new
            {
                ok = false,
                reason = "not_found",
                profile = (object)null,
                social_links = new object[0]
            };
        }

        bool isShop = ShopSlug_cl.IsShopAccount(db, account);
        HomeProfileSetting_cl.ProfileSettings settings = HomeProfileSetting_cl.GetSettings(db, account.taikhoan, isShop);

        string accountKey = (account.taikhoan ?? "").Trim().ToLowerInvariant();
        string displayName = ResolveDisplayName(account, info);
        string avatarUrl = ResolveImageOrFallback(GianHangStorefront_cl.ResolveAvatarUrl(account), "/app-ui/assets/placeholder-media.svg");
        string intro = string.IsNullOrWhiteSpace(account.gioithieu) ? "Chưa cập nhật giới thiệu." : account.gioithieu.Trim();

        string profilePath = ShopSlug_cl.GetPublicUrl(db, account);
        string profileUrl = ResolveAbsoluteUrl(context, profilePath);
        string saveContactUrl = "/home/luu-danh-ba.aspx?user=" + HttpUtility.UrlEncode(accountKey);
        string phone = (account.dienthoai ?? "").Trim();

        List<MangXaHoi_tb> links = db.MangXaHoi_tbs
            .Where(x => x.TaiKhoan == account.taikhoan && x.Kieu == "Cá nhân")
            .ToList();
        List<MangXaHoi_tb> ordered = SocialLinkOrder_cl.SortLinks(links, settings.SocialOrderPersonal);
        List<object> socialLinks = ordered
            .Select(MapSocialLink)
            .Where(x => x != null)
            .Cast<object>()
            .ToList();

        int reviewCount = CountSafe(() => db.DanhGiaBaiViets.Count(x => x.ThuocTaiKhoan == account.taikhoan));
        int postCount = CountSafe(() => db.BaiViet_tbs.Count(x => x.nguoitao == account.taikhoan && (x.bin == null || x.bin == false)));
        decimal consumerRights = account.DongA ?? 0m;
        decimal offerRights = account.Vi1That_Evocher_30PhanTram ?? 0m;
        decimal laborRights = account.Vi2That_LaoDong_50PhanTram ?? 0m;
        decimal engagementRights = account.Vi3That_GanKet_20PhanTram ?? 0m;

        return new
        {
            ok = true,
            source = "runtime",
            profile = new
            {
                account_key = accountKey,
                display_name = displayName,
                username = "Home " + accountKey,
                role_label = ResolveRoleLabel(account, isShop),
                intro = intro,
                avatar_url = avatarUrl,
                avatar_fallback = BuildAvatarFallback(displayName, accountKey),
                phone = string.IsNullOrWhiteSpace(phone) ? "" : phone,
                phone_href = string.IsNullOrWhiteSpace(phone) ? "" : ("tel:" + phone),
                email = string.IsNullOrWhiteSpace(account.email) ? "" : account.email.Trim(),
                address = string.IsNullOrWhiteSpace(account.diachi) ? "" : account.diachi.Trim(),
                public_profile_url = profileUrl,
                save_contact_url = saveContactUrl,
                edit_url = "/app-ui/home/settings.aspx?ui_mode=app",
                template_key = string.IsNullOrWhiteSpace(settings.TemplateKey) ? "classic" : settings.TemplateKey,
                accent_color = string.IsNullOrWhiteSpace(settings.AccentColor) ? "#22c55e" : settings.AccentColor
            },
            settings = new
            {
                show_contact = settings.ShowContact,
                show_social = settings.ShowSocial
            },
            stats = new
            {
                review_count = reviewCount,
                post_count = postCount,
                social_count = socialLinks.Count
            },
            balances = new
            {
                consumer_rights = consumerRights,
                offer_rights = offerRights,
                labor_rights = laborRights,
                engagement_rights = engagementRights,
                total_points = consumerRights + offerRights + laborRights + engagementRights
            },
            social_links = socialLinks
        };
    }

    private static taikhoan_tb ResolveAccount(dbDataContext db, RootAccount_cl.RootAccountInfo info)
    {
        if (db == null || info == null)
            return null;

        taikhoan_tb byKey = RootAccount_cl.GetByAccountKey(db, info.AccountKey);
        if (byKey != null)
            return byKey;

        string accountKey = (info.AccountKey ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(accountKey))
            return null;

        return db.taikhoan_tbs.FirstOrDefault(x => (x.taikhoan ?? "").Trim().ToLower() == accountKey);
    }

    private static string ResolveDisplayName(taikhoan_tb account, RootAccount_cl.RootAccountInfo info)
    {
        if (account != null && !string.IsNullOrWhiteSpace(account.hoten))
            return account.hoten.Trim();
        if (info != null && !string.IsNullOrWhiteSpace(info.FullName))
            return info.FullName.Trim();
        if (account != null && !string.IsNullOrWhiteSpace(account.taikhoan))
            return "Home " + account.taikhoan.Trim();
        return "Tài khoản Home";
    }

    private static string ResolveRoleLabel(taikhoan_tb account, bool isShop)
    {
        if (account == null)
            return "Khách hàng";

        if (isShop)
            return "Gian hàng đối tác";

        string scope = PortalScope_cl.ResolveScope(account.taikhoan, account.phanloai, account.permission);
        if (string.Equals(scope, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase))
            return "Nhân viên admin";

        return "Khách hàng";
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
        string host = ResolveExternalLinkLabel(row.Link);
        return new
        {
            id = row.id,
            title = title,
            host = host,
            href = href,
            icon = icon
        };
    }

    private static int CountSafe(Func<int> resolver)
    {
        try
        {
            return resolver == null ? 0 : resolver();
        }
        catch
        {
            return 0;
        }
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
        string explicitIcon = ResolveImageOrFallback(iconRaw, "");
        if (!string.IsNullOrEmpty(explicitIcon))
            return explicitIcon;

        string resolved = SocialLinkIcon_cl.ResolveIconForDisplay("", linkRaw ?? "");
        return ResolveImageOrFallback(resolved, "");
    }

    private static string ResolveImageOrFallback(string imageRaw, string fallback)
    {
        string image = (imageRaw ?? "").Trim();
        if (string.IsNullOrEmpty(image))
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

        if (IsMissingUploadFile(image))
            return fallback;

        return image;
    }

    private static bool IsMissingUploadFile(string relativeUrl)
    {
        if (string.IsNullOrEmpty(relativeUrl))
            return false;

        string cleanPath = relativeUrl.Trim();
        if (cleanPath.Length == 0)
            return false;

        int q = cleanPath.IndexOf('?');
        if (q >= 0)
            cleanPath = cleanPath.Substring(0, q);
        int h = cleanPath.IndexOf('#');
        if (h >= 0)
            cleanPath = cleanPath.Substring(0, h);

        if (!cleanPath.StartsWith("/", StringComparison.Ordinal))
            cleanPath = "/" + cleanPath.TrimStart('/');

        if (!cleanPath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            return false;

        try
        {
            HttpContext current = HttpContext.Current;
            if (current == null || current.Server == null)
                return false;
            string path = current.Server.MapPath(cleanPath);
            if (string.IsNullOrEmpty(path))
                return false;
            return !File.Exists(path);
        }
        catch
        {
            return false;
        }
    }

    private static string ResolveAbsoluteUrl(HttpContext context, string url)
    {
        string value = (url ?? "").Trim();
        if (string.IsNullOrEmpty(value))
            return "";

        Uri absolute;
        if (Uri.TryCreate(value, UriKind.Absolute, out absolute))
            return absolute.AbsoluteUri;

        if (!value.StartsWith("/", StringComparison.Ordinal))
            value = "/" + value;

        string authority = "";
        if (context != null && context.Request != null && context.Request.Url != null)
            authority = context.Request.Url.GetLeftPart(UriPartial.Authority);

        if (string.IsNullOrEmpty(authority))
            return value;
        return authority + value;
    }

    private static string BuildAvatarFallback(string displayName, string accountKey)
    {
        string value = (displayName ?? "").Trim();
        if (string.IsNullOrWhiteSpace(value))
            value = (accountKey ?? "").Trim();
        if (string.IsNullOrWhiteSpace(value))
            return "AH";

        string[] parts = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return (parts[0].Substring(0, 1) + parts[1].Substring(0, 1)).ToUpperInvariant();
        if (parts[0].Length >= 2)
            return parts[0].Substring(0, 2).ToUpperInvariant();
        return parts[0].ToUpperInvariant();
    }
}
