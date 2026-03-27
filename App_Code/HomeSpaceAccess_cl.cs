using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

public static class HomeSpaceAccess_cl
{
    public sealed class SpaceGuideInfo
    {
        public string SpaceCode { get; set; }
        public string Title { get; set; }
        public string RoutePath { get; set; }
        public string Summary { get; set; }
        public string AdminToggleLabel { get; set; }
        public string AdminHint { get; set; }
        public string RequestHint { get; set; }
        public IList<string> UsageItems { get; set; }
    }

    public static SpaceGuideInfo Resolve(string rawSpaceCode)
    {
        string spaceCode = ModuleSpace_cl.Normalize(rawSpaceCode);
        switch (spaceCode)
        {
            case ModuleSpace_cl.Shop:
                return new SpaceGuideInfo
                {
                    SpaceCode = ModuleSpace_cl.Shop,
                    Title = "Không gian shop đối tác chiến lược",
                    RoutePath = "/shop/default.aspx",
                    Summary = "Đây là không gian shop đối tác chiến lược của AhaSale. Tài khoản Home gửi yêu cầu mở quyền kèm % chiết khấu cho sàn; sau khi admin duyệt thì toàn bộ tin đăng trong /shop sẽ tự động áp dụng mức % này.",
                    AdminToggleLabel = "Duyệt tại tab Duyệt gian hàng đối tác (Shop)",
                    AdminHint = "Admin xử lý tại: /admin > Duyệt gian hàng đối tác (Shop). Khi duyệt sẽ mở quyền /shop cho chính tài khoản Home và lưu chính sách % chiết khấu mặc định của shop.",
                    RequestHint = "Bạn cần nhập % chiết khấu cho sàn trước khi gửi yêu cầu. Sau khi được duyệt, các tin đăng mới trong /shop tự lấy đúng % này.",
                    UsageItems = new List<string>
                    {
                        "Truy cập không gian /shop bằng chính tài khoản Home đã được duyệt.",
                        "Toàn bộ tin đăng từ /shop dùng chính sách chia % 9 hành vi theo mức chiết khấu đã duyệt.",
                        "Mức % này là mặc định của shop đối tác; khi thay đổi cần gửi lại và được admin xác nhận."
                    }
                };
            case ModuleSpace_cl.GianHang:
                return new SpaceGuideInfo
                {
                    SpaceCode = ModuleSpace_cl.GianHang,
                    Title = "Không gian gian hàng",
                    RoutePath = "/gianhang",
                    Summary = "Đây là không gian gian hàng dành riêng cho tài khoản Home. Tài khoản Home cần gửi đăng ký mở gian hàng đối tác, sau khi admin duyệt tại tab Duyệt không gian gian hàng thì chính tài khoản Home đó sẽ được mở quyền truy cập /gianhang.",
                    AdminToggleLabel = "Duyệt tại tab Duyệt không gian gian hàng",
                    AdminHint = "Admin duyệt tại: /admin > Duyệt không gian gian hàng. Không gian /gianhang không cấp trực tiếp bằng cách tick quyền ở màn hình chi tiết tài khoản Home.",
                    RequestHint = "Bạn có thể gửi yêu cầu mở gian hàng đối tác ngay tại đây. Sau khi được duyệt, nút truy cập /gianhang sẽ xuất hiện trong Không gian hoạt động.",
                    UsageItems = new List<string>
                    {
                        "Truy cập trực tiếp không gian /gianhang bằng chính tài khoản Home sau khi được duyệt.",
                        "Sử dụng các chức năng gắn với không gian gian hàng công khai của tài khoản.",
                        "Luồng xét duyệt đi qua tab Duyệt không gian gian hàng thay vì cấp tay trong quản lý tài khoản Home."
                    }
                };
            case ModuleSpace_cl.GianHangAdmin:
                return new SpaceGuideInfo
                {
                    SpaceCode = ModuleSpace_cl.GianHangAdmin,
                    Title = "Không gian quản trị gian hàng",
                    RoutePath = "/gianhang/admin",
                    Summary = "Đây là khu quản trị nâng cao của gian hàng. Tài khoản Home có thể truy cập không gian do chính mình làm chủ, hoặc truy cập vào không gian của tài khoản Home khác khi được thêm vào với vai trò nhân viên.",
                    AdminToggleLabel = "Cho phép truy cập không gian quản trị Gian hàng (/gianhang/admin)",
                    AdminHint = "Admin mở quyền tại: /admin > Quản lý tài khoản Home > Chi tiết tài khoản > Phân quyền theo không gian > Cho phép truy cập không gian quản trị Gian hàng (/gianhang/admin). Trong từng không gian, chủ gian hàng cũng có thể liên kết thêm các tài khoản Home khác làm nhân viên tham gia.",
                    RequestHint = "Nếu bạn chưa có không gian riêng, hãy gửi yêu cầu mở quyền tại đây. Nếu bạn đã được thêm vào không gian của người khác, danh sách không gian đó sẽ hiển thị ở ngay màn hình này để bạn truy cập.",
                    UsageItems = new List<string>
                    {
                        "Vào khu quản trị nâng cao của gian hàng bằng chính tài khoản Home.",
                        "Có thể vừa làm chủ không gian của mình, vừa tham gia vận hành trong không gian của tài khoản Home khác.",
                        "Nếu tham gia nhiều không gian, tài khoản sẽ nhìn thấy danh sách các không gian đang được phép truy cập để chọn đúng nơi làm việc."
                    }
                };
            case ModuleSpace_cl.DauGia:
                return new SpaceGuideInfo
                {
                    SpaceCode = ModuleSpace_cl.DauGia,
                    Title = "Không gian quản trị đấu giá",
                    RoutePath = "/daugia/admin",
                    Summary = "Đây là khu quản lý và vận hành đấu giá riêng của tài khoản Home. Tài khoản chỉ có thể truy cập sau khi được admin mở quyền đúng cho không gian /daugia/admin.",
                    AdminToggleLabel = "Cho phép truy cập không gian Đấu giá (/daugia/admin)",
                    AdminHint = "Admin mở quyền tại: /admin > Quản lý tài khoản Home > Chi tiết tài khoản > Phân quyền theo không gian > Cho phép truy cập không gian Đấu giá (/daugia/admin).",
                    RequestHint = "Bạn có thể gửi yêu cầu mở quyền để admin xác nhận và bật không gian Đấu giá cho tài khoản Home.",
                    UsageItems = new List<string>
                    {
                        "Vào khu quản trị đấu giá tại /daugia/admin.",
                        "Tạo phiên mới và quản lý toàn bộ phiên thuộc chính tài khoản đó.",
                        "Tách biệt hoàn toàn với gian hàng, event và khu admin hệ thống."
                    }
                };
            case ModuleSpace_cl.Event:
                return new SpaceGuideInfo
                {
                    SpaceCode = ModuleSpace_cl.Event,
                    Title = "Không gian quản trị sự kiện",
                    RoutePath = "/event/admin",
                    Summary = "Đây là khu tạo và vận hành chiến dịch sự kiện. Tài khoản Home sẽ dùng trực tiếp không gian này sau khi được admin mở quyền.",
                    AdminToggleLabel = "Cho phép truy cập không gian Event (/event/admin)",
                    AdminHint = "Admin mở quyền tại: /admin > Quản lý tài khoản Home > Chi tiết tài khoản > Phân quyền theo không gian > Cho phép truy cập không gian Event (/event/admin).",
                    RequestHint = "Bạn có thể gửi yêu cầu mở quyền để admin xác nhận và bật không gian Event cho tài khoản Home.",
                    UsageItems = new List<string>
                    {
                        "Vào khu quản trị sự kiện tại /event/admin.",
                        "Tạo và vận hành chiến dịch, nội dung hoặc luồng làm việc thuộc Event Platform.",
                        "Chỉ có hiệu lực trong không gian event, không thay thế các quyền khác."
                    }
                };
            default:
                return null;
        }
    }

    public static string BuildAccessPageUrl(string rawSpaceCode, string rawReturnUrl)
    {
        SpaceGuideInfo info = Resolve(rawSpaceCode);
        if (info == null)
            return "/home/default.aspx";

        string returnUrl = NormalizeReturnUrl(rawReturnUrl, info.RoutePath);
        if (string.Equals(info.SpaceCode, ModuleSpace_cl.GianHang, StringComparison.OrdinalIgnoreCase))
        {
            return "/home/dang-ky-gian-hang-doi-tac.aspx?return_url=" + HttpUtility.UrlEncode(returnUrl);
        }

        return "/home/mo-khong-gian.aspx?space=" + HttpUtility.UrlEncode(info.SpaceCode)
            + "&return_url=" + HttpUtility.UrlEncode(returnUrl);
    }

    public static string NormalizeReturnUrl(string rawReturnUrl, string fallbackUrl)
    {
        string fallback = string.IsNullOrWhiteSpace(fallbackUrl) ? "/home/default.aspx" : fallbackUrl.Trim();
        string returnUrl = (rawReturnUrl ?? "").Trim();
        if (returnUrl == "")
            return fallback;

        if (!returnUrl.StartsWith("/", StringComparison.Ordinal))
            return fallback;

        if (returnUrl.StartsWith("//", StringComparison.Ordinal))
            return fallback;

        if (returnUrl.IndexOf("/home/mo-khong-gian.aspx", StringComparison.OrdinalIgnoreCase) >= 0)
            return fallback;

        return returnUrl;
    }

    public static void RedirectToAccessPage(Page page, string rawSpaceCode, string rawReturnUrl)
    {
        if (page == null || page.Response == null)
            return;

        page.Response.Redirect(BuildAccessPageUrl(rawSpaceCode, rawReturnUrl), true);
    }

    public static void RedirectToHomeLogin(Page page, string rawReturnUrl, string message)
    {
        if (page == null || page.Response == null)
            return;

        string returnUrl = ResolvePreferredReturnUrl(HttpContext.Current, rawReturnUrl, "/home/default.aspx");
        RememberReturnUrl(HttpContext.Current, returnUrl, "/home/default.aspx");

        if (page.Session != null)
            page.Session["thongbao_home"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                string.IsNullOrWhiteSpace(message) ? "Vui lòng đăng nhập tài khoản Home để tiếp tục." : message,
                "1500",
                "warning");

        page.Response.Redirect("/dang-nhap?return_url=" + HttpUtility.UrlEncode(returnUrl), true);
    }

    public static void RememberReturnUrl(HttpContext ctx, string rawReturnUrl, string fallbackUrl)
    {
        if (ctx == null)
            return;

        string returnUrl = NormalizeReturnUrl(rawReturnUrl, fallbackUrl);
        if (ctx.Session != null)
            ctx.Session["url_back_home"] = returnUrl;

        app_cookie_policy_class.persist_cookie(
            ctx,
            app_cookie_policy_class.home_return_url_cookie,
            returnUrl,
            1);
    }

    public static string ResolvePreferredReturnUrl(HttpContext ctx, string rawReturnUrl, string fallbackUrl)
    {
        string fallback = NormalizeReturnUrl(rawReturnUrl, fallbackUrl);
        if (ctx == null)
            return fallback;

        string[] candidates = new[]
        {
            rawReturnUrl,
            ctx.Request == null ? "" : (ctx.Request.QueryString["return_url"] ?? ctx.Request.QueryString["returnUrl"] ?? ""),
            ctx.Session == null ? "" : ((ctx.Session["url_back_home"] ?? "").ToString()),
            app_cookie_policy_class.read_cookie(ctx, app_cookie_policy_class.home_return_url_cookie),
            app_cookie_policy_class.read_cookie(ctx, app_cookie_policy_class.admin_return_url_cookie),
            ctx.Request != null && ctx.Request.UrlReferrer != null ? ctx.Request.UrlReferrer.PathAndQuery : ""
        };

        foreach (string candidate in candidates)
        {
            string normalized = NormalizeReturnUrl(candidate, "");
            if (string.IsNullOrWhiteSpace(normalized))
                continue;
            return normalized;
        }

        return fallback;
    }

    public static string GetAccessStatusText(string rawStatus)
    {
        string status = (rawStatus ?? "").Trim().ToLowerInvariant();
        switch (status)
        {
            case SpaceAccess_cl.StatusActive:
                return "Đã được mở quyền";
            case SpaceAccess_cl.StatusPending:
                return "Đang chờ xử lý";
            case SpaceAccess_cl.StatusBlocked:
                return "Đang bị khóa";
            case SpaceAccess_cl.StatusRevoked:
                return "Đã bị thu hồi";
            default:
                return "Chưa được mở quyền";
        }
    }

    public static string GetRequestStatusText(string rawStatus)
    {
        string status = (rawStatus ?? "").Trim().ToLowerInvariant();
        switch (status)
        {
            case CoreSpaceRequest_cl.StatusPending:
                return "Đã gửi yêu cầu, đang chờ duyệt";
            case CoreSpaceRequest_cl.StatusApproved:
                return "Yêu cầu gần nhất đã được duyệt";
            case CoreSpaceRequest_cl.StatusRejected:
                return "Yêu cầu gần nhất đã bị từ chối";
            case CoreSpaceRequest_cl.StatusCancelled:
                return "Yêu cầu gần nhất đã bị hủy";
            default:
                return "Chưa có yêu cầu";
        }
    }
}
