<%@ Application Language="C#" %>

<script RunAt="server">

    void Application_BeginRequest(object sender, EventArgs e)
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Response == null) return;

        // Enforce UTF-8 for all dynamic ASP.NET responses on hosting.
        ctx.Response.ContentEncoding = System.Text.Encoding.UTF8;
        ctx.Response.Charset = "utf-8";

        RewriteDauGiaRoutes(ctx);
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

        if (normalized == "/daugia/da-ket-thuc")
        {
            ctx.RewritePath("/daugia/da-ket-thuc.aspx", "", requestQuery, false);
            return;
        }

        if (normalized == "/daugia/quan-ly")
        {
            ctx.RewritePath("/daugia/quan-ly.aspx", "", requestQuery, false);
            return;
        }

        if (normalized == "/daugia/tao")
        {
            ctx.RewritePath("/daugia/tao.aspx", "", requestQuery, false);
            return;
        }

        if (normalized == "/admin/quan-ly-dau-gia")
        {
            ctx.RewritePath("/daugia/admin/default.aspx", "", requestQuery, false);
            return;
        }

        if (normalized == "/home/dau-gia" || normalized == "/home/daugia")
        {
            ctx.RewritePath("/daugia/quan-ly.aspx", "", requestQuery, false);
            return;
        }

        if (normalized == "/home/dau-gia/tao" || normalized == "/home/daugia/tao")
        {
            ctx.RewritePath("/daugia/tao.aspx", "", requestQuery, false);
            return;
        }

        if (normalized == "/shop/dau-gia" || normalized == "/shop/daugia")
        {
            ctx.RewritePath("/daugia/quan-ly.aspx", "", MergeQuery("shop_portal=1", requestQuery), false);
            return;
        }

        if (normalized == "/shop/dau-gia/tao" || normalized == "/shop/daugia/tao")
        {
            ctx.RewritePath("/daugia/tao.aspx", "", MergeQuery("shop_portal=1", requestQuery), false);
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

    private string MergeQuery(string forcedQuery, string requestQuery)
    {
        string forced = (forcedQuery ?? "").Trim();
        string request = (requestQuery ?? "").Trim();
        if (forced == "") return request;
        if (request == "") return forced;
        return forced + "&" + request;
    }

    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        try
        {
            using (dbDataContext db = new dbDataContext())
                ShopStatus_cl.EnsureSchemaSafe(db);
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
