<%@ Application Language="C#" %>

<script RunAt="server">

    void Application_BeginRequest(object sender, EventArgs e)
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Response == null) return;

        // Enforce UTF-8 for all dynamic ASP.NET responses on hosting.
        ctx.Response.ContentEncoding = System.Text.Encoding.UTF8;
        ctx.Response.Charset = "utf-8";
    }

    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup

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
        Session["thongbao_shop"] = "";
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
        Session["thongbao_shop"] = "";
        Session["url_back_shop"] = "";
    }

</script>
