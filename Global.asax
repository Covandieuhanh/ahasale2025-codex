<%@ Application Language="C#" %>

<script RunAt="server">

    void Application_BeginRequest(object sender, EventArgs e)
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Response == null) return;

        // Enforce UTF-8 for all dynamic ASP.NET responses on hosting.
        ctx.Response.ContentEncoding = System.Text.Encoding.UTF8;
        ctx.Response.Charset = "utf-8";

        if (RewriteLegacyLoginRoutes(ctx))
            return;

        if (RewriteGianHangRoutes(ctx))
            return;

        CompanyShopBootstrap_cl.EnsureRuntimeWarmup();
        RewriteDauGiaRoutes(ctx);
    }

    void Application_AcquireRequestState(object sender, EventArgs e)
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null) return;

        ManagedHomeSpaceGuard_cl.TryHandleCurrentRequest(ctx);
    }

    private bool RewriteGianHangRoutes(HttpContext ctx)
    {
        if (ctx == null || ctx.Request == null || ctx.Request.Url == null)
            return false;

        string rawPath = ctx.Request.Url.AbsolutePath ?? "";
        if (rawPath == "")
            return false;

        string normalized = rawPath.Trim().ToLowerInvariant();
        while (normalized.Length > 1 && normalized.EndsWith("/"))
            normalized = normalized.Substring(0, normalized.Length - 1);

        string requestQueryRaw = ctx.Request.Url.Query ?? "";
        string requestQuery = requestQueryRaw.StartsWith("?") ? requestQueryRaw.Substring(1) : requestQueryRaw;

        System.Collections.Generic.Dictionary<string, string> routeMap = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "/gianhang", "/gianhang/default.aspx" },
            { "/gianhang/default", "/gianhang/default.aspx" },
            { "/gianhang/public", "/gianhang/public.aspx" },
            { "/gianhang/don-ban", "/gianhang/don-ban.aspx" },
            { "/gianhang/cho-thanh-toan", "/gianhang/cho-thanh-toan.aspx" },
            { "/gianhang/bao-cao", "/gianhang/bao-cao.aspx" },
            { "/gianhang/dat-lich", "/gianhang/dat-lich.aspx" },
            { "/gianhang/chi-tiet-san-pham", "/gianhang/chi-tiet-san-pham.aspx" },
            { "/gianhang/chi-tiet-dich-vu", "/gianhang/chi-tiet-dich-vu.aspx" },
            { "/gianhang/khach-hang", "/gianhang/khach-hang.aspx" },
            { "/gianhang/khach-hang-chi-tiet", "/gianhang/khach-hang-chi-tiet.aspx" },
            { "/gianhang/quan-ly-lich-hen", "/gianhang/quan-ly-lich-hen.aspx" },
            { "/gianhang/quan-ly-tin", "/gianhang/quan-ly-tin/default.aspx" },
            { "/gianhang/tai-khoan", "/gianhang/tai-khoan/default.aspx" },
            { "/gianhang/mo-cong-cu-quan-tri", "/gianhang/mo-cong-cu-quan-tri.aspx" },
            { "/gianhang/xem-san-pham", "/gianhang/xem-san-pham.aspx" },
            { "/gianhang/xem-dich-vu", "/gianhang/xem-dich-vu.aspx" },
            { "/gianhang/tao-don", "/gianhang/don-ban.aspx?taodon=1" },
            { "/gianhang/admin", "/gianhang/admin/default.aspx" },
            { "/gianhang/page/danh-sach-san-pham", "/gianhang/page/danh-sach-san-pham.aspx" },
            { "/gianhang/page/danh-sach-dich-vu", "/gianhang/page/danh-sach-dich-vu.aspx" },
            { "/gianhang/page/danh-sach-bai-viet", "/gianhang/page/danh-sach-bai-viet.aspx" },
            { "/gianhang/page/chi-tiet-san-pham", "/gianhang/page/chi-tiet-san-pham.aspx" },
            { "/gianhang/page/chi-tiet-dich-vu", "/gianhang/page/chi-tiet-dich-vu.aspx" },
            { "/gianhang/page/chi-tiet-bai-viet", "/gianhang/page/chi-tiet-bai-viet.aspx" }
        };

        string mappedTarget;
        if (routeMap.TryGetValue(normalized, out mappedTarget))
        {
            string targetPath = mappedTarget;
            string targetQuery = "";
            int split = mappedTarget.IndexOf('?');
            if (split >= 0)
            {
                targetPath = mappedTarget.Substring(0, split);
                targetQuery = mappedTarget.Substring(split + 1);
            }

            if (requestQuery != "")
                targetQuery = targetQuery == "" ? requestQuery : (targetQuery + "&" + requestQuery);

            ctx.RewritePath(targetPath, "", targetQuery, false);
            return true;
        }

        if (normalized.StartsWith("/gianhang/") &&
            normalized.IndexOf('.') < 0 &&
            !normalized.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
        {
            string candidate = normalized + ".aspx";
            string physical = "";
            try { physical = ctx.Server.MapPath(candidate); } catch { physical = ""; }

            if (!string.IsNullOrWhiteSpace(physical) && System.IO.File.Exists(physical))
            {
                ctx.RewritePath(candidate, "", requestQuery, false);
                return true;
            }
        }

        return false;
    }

    private void RewriteDauGiaRoutes(HttpContext ctx)
    {
        if (ctx == null || ctx.Request == null || ctx.Request.Url == null)
            return;

        string rawPath = ctx.Request.Url.AbsolutePath ?? "";
        if (rawPath == "")
            return;

        string normalized = rawPath.Trim().ToLowerInvariant();
        while (normalized.Length > 1 && normalized.EndsWith("/"))
            normalized = normalized.Substring(0, normalized.Length - 1);

        string requestQueryRaw = ctx.Request.Url.Query ?? "";
        string requestQuery = requestQueryRaw.StartsWith("?") ? requestQueryRaw.Substring(1) : requestQueryRaw;

        if (normalized == "/daugia")
        {
            ctx.RewritePath("/daugia/default.aspx", "", requestQuery, false);
            return;
        }

        if (normalized == "/home/daugia")
        {
            string target = "/daugia/admin";
            if (requestQuery != "")
                target += "?" + requestQuery;
            ctx.Response.Redirect(target, false);
            ctx.ApplicationInstance.CompleteRequest();
            return;
        }

        if (normalized == "/shop/dau-gia")
        {
            string target = "/daugia/admin?shop_portal=1";
            if (requestQuery != "")
                target += "&" + requestQuery;
            ctx.Response.Redirect(target, false);
            ctx.ApplicationInstance.CompleteRequest();
            return;
        }

        if (normalized == "/home/daugia/quan-ly")
        {
            string target = "/daugia/admin/quan-ly";
            if (requestQuery != "")
                target += "?" + requestQuery;
            ctx.Response.Redirect(target, false);
            ctx.ApplicationInstance.CompleteRequest();
            return;
        }

        if (normalized == "/shop/dau-gia/quan-ly")
        {
            string target = "/daugia/admin/quan-ly?shop_portal=1";
            if (requestQuery != "")
                target += "&" + requestQuery;
            ctx.Response.Redirect(target, false);
            ctx.ApplicationInstance.CompleteRequest();
            return;
        }

        if (normalized == "/home/daugia/tao")
        {
            string target = "/daugia/admin/tao";
            if (requestQuery != "")
                target += "?" + requestQuery;
            ctx.Response.Redirect(target, false);
            ctx.ApplicationInstance.CompleteRequest();
            return;
        }

        if (normalized == "/shop/dau-gia/tao")
        {
            string target = "/daugia/admin/tao?shop_portal=1";
            if (requestQuery != "")
                target += "&" + requestQuery;
            ctx.Response.Redirect(target, false);
            ctx.ApplicationInstance.CompleteRequest();
            return;
        }

        if (normalized == "/daugia/da-ket-thuc")
        {
            ctx.RewritePath("/daugia/da-ket-thuc.aspx", "", requestQuery, false);
            return;
        }

        if (normalized == "/daugia/quan-ly")
        {
            string target = "/daugia/admin/quan-ly";
            if (requestQuery != "")
                target += "?" + requestQuery;
            ctx.Response.Redirect(target, false);
            ctx.ApplicationInstance.CompleteRequest();
            return;
        }

        if (normalized == "/daugia/tao")
        {
            string target = "/daugia/admin/tao";
            if (requestQuery != "")
                target += "?" + requestQuery;
            ctx.Response.Redirect(target, false);
            ctx.ApplicationInstance.CompleteRequest();
            return;
        }

        if (normalized == "/daugia/admin")
        {
            ctx.RewritePath("/daugia/admin/portal.aspx", "", requestQuery, false);
            return;
        }

        if (normalized == "/daugia/admin/quan-ly")
        {
            ctx.RewritePath("/daugia/admin/quan-ly.aspx", "", requestQuery, false);
            return;
        }

        if (normalized == "/daugia/admin/tao")
        {
            ctx.RewritePath("/daugia/admin/tao.aspx", "", requestQuery, false);
            return;
        }

        if (normalized == "/admin/quan-ly-dau-gia")
        {
            string adminRewriteQuery = "system=1";
            if (requestQuery != "")
                adminRewriteQuery += "&" + requestQuery;
            ctx.RewritePath("/daugia/admin/default.aspx", "", adminRewriteQuery, false);
            return;
        }

        if (normalized == "/event/admin")
        {
            ctx.RewritePath("/event/admin/default.aspx", "", requestQuery, false);
            return;
        }

        if (normalized == "/admin/quan-ly-event")
        {
            string adminEventQuery = "system=1";
            if (requestQuery != "")
                adminEventQuery += "&" + requestQuery;
            ctx.RewritePath("/event/admin/default.aspx", "", adminEventQuery, false);
            return;
        }

        System.Text.RegularExpressions.Match detailMatch =
            System.Text.RegularExpressions.Regex.Match(
                normalized,
                "^/daugia/([^/]+)-([0-9]+)\\.html$",
                System.Text.RegularExpressions.RegexOptions.CultureInvariant);

        if (!detailMatch.Success)
            return;

        string id = detailMatch.Groups[2].Value;
        string rewriteQuery = "id=" + id;
        if (requestQuery != "")
            rewriteQuery += "&" + requestQuery;

        ctx.RewritePath("/daugia/chi-tiet.aspx", "", rewriteQuery, false);
    }

    private bool RewriteLegacyLoginRoutes(HttpContext ctx)
    {
        if (ctx == null || ctx.Request == null || ctx.Request.Url == null)
            return false;

        string rawPath = ctx.Request.Url.AbsolutePath ?? "";
        string normalized = rawPath.Trim().ToLowerInvariant();
        while (normalized.Length > 1 && normalized.EndsWith("/"))
            normalized = normalized.Substring(0, normalized.Length - 1);

        if (normalized == "/dang-ky")
        {
            string requestQueryRaw = ctx.Request.Url.Query ?? "";
            string requestQuery = requestQueryRaw.StartsWith("?") ? requestQueryRaw.Substring(1) : requestQueryRaw;
            ctx.RewritePath("/home/dangky.aspx", "", requestQuery, false);
            return true;
        }

        string fallbackReturn = "";
        if (normalized == "/shop")
            fallbackReturn = "/shop/default.aspx";
        else if (normalized == "/shop/login.aspx" || normalized == "/shop/dang-nhap")
            fallbackReturn = "/shop/default.aspx";
        else if (normalized == "/gianhang/admin/login.aspx")
            fallbackReturn = "/gianhang/admin/default.aspx";

        if (fallbackReturn == "")
            return false;

        string requestedReturn = ctx.Request.QueryString["return_url"] ?? ctx.Request.QueryString["returnUrl"] ?? "";
        string targetReturn = HomeSpaceAccess_cl.ResolvePreferredReturnUrl(ctx, requestedReturn, fallbackReturn);
        string safeReturn = check_login_cl.NormalizeUnifiedReturnUrl(targetReturn);
        string target = string.IsNullOrEmpty(safeReturn)
            ? "/dang-nhap"
            : "/dang-nhap?return_url=" + HttpUtility.UrlEncode(safeReturn);

        ctx.Response.Redirect(target, false);
        ctx.ApplicationInstance.CompleteRequest();
        return true;
    }

    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                ShopStatus_cl.EnsureSchemaSafe(db);
                CompanyShopBootstrap_cl.EnsureSystemCatalogMirrored(db);
            }
        }
        catch { }
    }

    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }

    void Application_Error(object sender, EventArgs e)
    {
        // Tự phục hồi khi browser gửi lại POST cũ (stale __VIEWSTATE / __EVENTVALIDATION)
        Exception ex = Server.GetLastError();
        HttpContext ctx = HttpContext.Current;
        if (ex == null || ctx == null) return;

        // Bỏ qua ThreadAbortException do Response.Redirect/End gây ra
        if (ex is System.Threading.ThreadAbortException)
        {
            Server.ClearError();
            return;
        }

        bool isPost = string.Equals(ctx.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase);
        string full = ex.ToString();
        if (ex.InnerException != null)
            full += " " + ex.InnerException.ToString();

        bool isStateValidationError =
            full.IndexOf("Unable to validate data", StringComparison.OrdinalIgnoreCase) >= 0
            || full.IndexOf("state information is invalid", StringComparison.OrdinalIgnoreCase) >= 0
            || full.IndexOf("RestoreEventValidationState", StringComparison.OrdinalIgnoreCase) >= 0
            || full.IndexOf("ObjectStateFormatter.Deserialize", StringComparison.OrdinalIgnoreCase) >= 0;

        if (isPost && isStateValidationError)
        {
            try
            {
                if (ctx.Session != null)
                {
                    ctx.Session["thongbao"] = thongbao_class.metro_dialog_onload(
                        "Thông báo",
                        "Phiên thao tác trước đã hết hạn. Hệ thống đã tải lại trang, vui lòng thao tác lại.",
                        "false",
                        "false",
                        "OK",
                        "alert",
                        "");
                }
            }
            catch { }

            Server.ClearError();
            string target = ctx.Request.RawUrl;
            if (string.IsNullOrEmpty(target)) target = "/";
            ctx.Response.Redirect(target, false);
            ctx.ApplicationInstance.CompleteRequest();
        }
    }

    void Session_Start(object sender, EventArgs e)
    {
        Session["taikhoan"] = "";
        Session["matkhau"] = "";
        Session["thongbao"] = "";
        Session["url_back"] = "";

        Session["taikhoan_home"] = "";
        Session["matkhau_home"] = "";
        Session["thongbao_home"] = "";
        Session["url_back_home"] = "";

        Session["taikhoan_shop"] = "";
        Session["matkhau_shop"] = "";
        Session["ThongBao_Shop"] = "";
        Session["url_back_shop"] = "";
    }

    void Session_End(object sender, EventArgs e)
    {
        Session["taikhoan"] = "";
        Session["matkhau"] = "";
        Session["thongbao"] = "";
        Session["url_back"] = "";

        Session["taikhoan_home"] = "";
        Session["matkhau_home"] = "";
        Session["thongbao_home"] = "";
        Session["url_back_home"] = "";

        Session["taikhoan_shop"] = "";
        Session["matkhau_shop"] = "";
        Session["ThongBao_Shop"] = "";
        Session["url_back_shop"] = "";
    }

</script>
