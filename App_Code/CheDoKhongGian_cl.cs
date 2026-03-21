using System;
using System.Web;

public static class CheDoKhongGian_cl
{
    private const string KhoaSession = "che_do_khong_gian_goc";
    private const string TenCookie = "cookie_che_do_khong_gian_goc";

    public static string ChuanHoa(string raw)
    {
        return PhanQuyenTruyCap_cl.ChuanHoaKhongGian(raw);
    }

    public static string LayCheDoDangDung()
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null)
            return PhanQuyenTruyCap_cl.KhongGianHome;

        try
        {
            if (ctx.Session != null)
            {
                string fromSession = (ctx.Session[KhoaSession] as string ?? "").Trim();
                string normalizedSession = ChuanHoa(fromSession);
                if (normalizedSession != "")
                    return normalizedSession;
            }
        }
        catch
        {
        }

        if (ctx.Request != null)
        {
            HttpCookie ck = ctx.Request.Cookies[TenCookie];
            if (ck != null)
            {
                string normalizedCookie = ChuanHoa(ck["mode"]);
                if (normalizedCookie != "")
                    return normalizedCookie;
            }
        }

        string cu = PortalActiveMode_cl.GetCurrentMode();
        if (string.Equals(cu, PortalActiveMode_cl.ModeShop, StringComparison.OrdinalIgnoreCase))
            return PhanQuyenTruyCap_cl.KhongGianShop;

        return PhanQuyenTruyCap_cl.KhongGianHome;
    }

    public static void DatCheDo(string cheDo)
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null)
            return;

        string normalized = ChuanHoa(cheDo);
        if (normalized == "")
            normalized = PhanQuyenTruyCap_cl.KhongGianHome;

        try
        {
            if (ctx.Session != null)
                ctx.Session[KhoaSession] = normalized;
        }
        catch
        {
        }

        HttpCookie ck = new HttpCookie(TenCookie);
        ck["mode"] = normalized;
        ck.Expires = AhaTime_cl.Now.AddDays(30);
        ck.HttpOnly = true;
        ck.Secure = ctx.Request != null && ctx.Request.IsSecureConnection;
        ctx.Response.Cookies.Add(ck);

        if (normalized == PhanQuyenTruyCap_cl.KhongGianShop)
            PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeShop);
        else if (normalized == PhanQuyenTruyCap_cl.KhongGianHome)
            PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeHome);
    }
}
