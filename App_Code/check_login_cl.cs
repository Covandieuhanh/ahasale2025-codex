using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.UI;


public class check_login_cl
{
    private static readonly string[] ShopReturnUrlBlockedPrefixes = new string[]
    {
        "/shop/login",
        "/shop/dang-ky",
        "/shop/xac-nhan-otp",
        "/shop/khoi-phuc-mat-khau",
        "/shop/dat-lai-mat-khau"
    };

    public static string NormalizeShopReturnUrl(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "";

        string cleaned = raw.Trim();
        if (!cleaned.StartsWith("/", StringComparison.Ordinal) || cleaned.StartsWith("//", StringComparison.Ordinal))
            return "";

        foreach (string prefix in ShopReturnUrlBlockedPrefixes)
        {
            if (cleaned.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return "";
        }

        return cleaned;
    }

    public static string BuildShopLoginUrl(string returnUrlRaw)
    {
        string safe = NormalizeShopReturnUrl(returnUrlRaw);
        if (string.IsNullOrEmpty(safe))
            return "/shop/login.aspx";

        return "/shop/login.aspx?return_url=" + HttpUtility.UrlEncode(safe);
    }

    private static bool IsLocalhostRequest()
    {
        string host = "";
        if (HttpContext.Current != null
            && HttpContext.Current.Request != null
            && HttpContext.Current.Request.Url != null)
        {
            host = HttpContext.Current.Request.Url.Host;
        }
        host = (host ?? "").ToLower();
        return host == "localhost" || host == "127.0.0.1" || host == "::1";
    }

    private static bool IsLocalSuperAdmin(string taikhoan)
    {
        return IsLocalhostRequest() && string.Equals(taikhoan, "admin", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsRootAdmin(string taikhoan)
    {
        return string.Equals((taikhoan ?? "").Trim(), "admin", StringComparison.OrdinalIgnoreCase);
    }

    public static IEnumerable<string> NormalizePermissionTokensForDisplay(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return Enumerable.Empty<string>();

        return raw
            .Split(new[] { ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => (x ?? "").Trim())
            .Where(x => x != "")
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private static bool HasAnyPermission(string userPermissionRaw, params string[] requiredPermissionRaw)
    {
        var userSet = new HashSet<string>(NormalizePermissionTokensForDisplay(userPermissionRaw), StringComparer.OrdinalIgnoreCase);
        if (userSet.Count == 0)
            return false;

        foreach (string raw in requiredPermissionRaw)
        {
            foreach (string token in NormalizePermissionTokensForDisplay(raw))
            {
                if (userSet.Contains(token))
                    return true;
            }
        }

        return false;
    }

    // Bridge cho phép tài khoản scope shop dùng các module shop cũ đang nằm ở /home/*
    // (không mở toàn bộ home để tránh lẫn quyền).
    private static readonly HashSet<string> ShopHomeBridgeExactPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "/home/trao-doi.aspx",
        "/home/them-vao-gio.aspx",
        "/home/don-chi-tiet.aspx",
        "/home/quan-ly-tin/default.aspx",
        "/home/quan-ly-tin/them.aspx",
        "/home/don-ban.aspx",
        "/home/cho-thanh-toan.aspx",
        "/home/ho-so-tieu-dung-shop.aspx",
        "/home/ho-so-uu-dai-shop.aspx",
        "/home/taikhoan.aspx",
        "/home/edit-info.aspx",
        "/home/doimatkhau.aspx",
        "/home/dong-gop-y-kien.aspx",
        "/home/khach-hang.aspx",
        "/home/lich-su-giao-dich.aspx"
    };

    private static readonly string[] ShopHomeBridgePrefixes = new[] { "/home/page/", "/daugia/" };

    private static bool IsShopHomeBridgePath()
    {
        string path = "";
        if (HttpContext.Current != null
            && HttpContext.Current.Request != null
            && HttpContext.Current.Request.Url != null)
        {
            path = (HttpContext.Current.Request.Url.AbsolutePath ?? "").Trim().ToLowerInvariant();
        }

        if (path == "")
            return false;

        if (path.EndsWith("-shop.aspx", StringComparison.OrdinalIgnoreCase))
            return true;

        for (int i = 0; i < ShopHomeBridgePrefixes.Length; i++)
        {
            if (path.StartsWith(ShopHomeBridgePrefixes[i], StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return ShopHomeBridgeExactPaths.Contains(path);
    }

    private static bool HasShopPortalMarker()
    {
        string marker = "";
        if (HttpContext.Current != null && HttpContext.Current.Request != null)
            marker = (HttpContext.Current.Request.QueryString["shop_portal"] ?? "").Trim();

        if (string.Equals(marker, "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(marker, "true", StringComparison.OrdinalIgnoreCase))
            return true;

        // Fallback cho postback WebForms: một số action bị rơi query shop_portal,
        // nhưng phiên shop vẫn còn và không có phiên home.
        if (HttpContext.Current != null && HttpContext.Current.Session != null)
        {
            string tkHome = HttpContext.Current.Session["taikhoan_home"] as string;
            string tkShop = HttpContext.Current.Session["taikhoan_shop"] as string;
            bool hasHomeSession = !string.IsNullOrEmpty(tkHome);
            bool hasShopSession = !string.IsNullOrEmpty(tkShop);

            if (!hasHomeSession && hasShopSession && IsShopHomeBridgePath())
                return true;
        }

        return false;
    }

    private static string GetCurrentPathLower()
    {
        if (HttpContext.Current == null
            || HttpContext.Current.Request == null
            || HttpContext.Current.Request.Url == null)
            return "";

        return (HttpContext.Current.Request.Url.AbsolutePath ?? "").Trim().ToLowerInvariant();
    }

    private static bool IsHomePasswordChangePath()
    {
        string path = GetCurrentPathLower();
        return path == "/home/doimatkhau.aspx";
    }

    private static bool IsHomePinChangePath()
    {
        string path = GetCurrentPathLower();
        return path == "/home/doipin.aspx";
    }

    private static bool IsShopPasswordChangePath()
    {
        string path = GetCurrentPathLower();
        if (path == "/shop/doi-mat-khau")
            return true;

        if (path == "/home/doimatkhau.aspx" && HasShopPortalMarker())
            return true;

        return false;
    }

    private static bool TryReadShopCredentialForHomeBridge(
        out string tk,
        out string mk,
        out string tkMaHoa,
        out string mkMaHoa)
    {
        tk = "";
        mk = "";
        tkMaHoa = "";
        mkMaHoa = "";

        HttpCookie ckShop = HttpContext.Current.Request.Cookies["cookie_userinfo_shop_bcorn"];
        if (ckShop != null && !string.IsNullOrEmpty(ckShop["taikhoan"]) && !string.IsNullOrEmpty(ckShop["matkhau"]))
        {
            tkMaHoa = ckShop["taikhoan"];
            mkMaHoa = ckShop["matkhau"];
            tk = mahoa_cl.giaima_Bcorn(tkMaHoa);
            mk = mahoa_cl.giaima_Bcorn(mkMaHoa);
            return !string.IsNullOrEmpty(tk);
        }

        if (HttpContext.Current.Session["taikhoan_shop"] != null && HttpContext.Current.Session["matkhau_shop"] != null)
        {
            tkMaHoa = HttpContext.Current.Session["taikhoan_shop"].ToString();
            mkMaHoa = HttpContext.Current.Session["matkhau_shop"].ToString();
            tk = mahoa_cl.giaima_Bcorn(tkMaHoa);
            mk = mahoa_cl.giaima_Bcorn(mkMaHoa);
            return !string.IsNullOrEmpty(tk);
        }

        return false;
    }

    //HttpCookie _ck = HttpContext.Current.Request.Cookies["cookie_userinfo_admin_bcorn"];
    //HttpContext.Current.Response.Write(_ck["taikhoan"] + "<br>");
    //HttpContext.Current.Response.Cookies["cookie_userinfo_admin_bcorn"].Expires = AhaTime_cl.Now.AddDays(-1);
    //HttpContext.Current.Response.Write(_tk + "1<br>");
    //HttpContext.Current.Response.Redirect("/123");

    //thông báo từ class ra trang chủ
    //Page page = HttpContext.Current.Handler as Page;
    //ScriptManager.RegisterStartupScript(page, page.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", ""), true);

    #region admin
    public static void del_all_cookie_session_admin()
    {
        try
        {
            //xóa Cookie cookie_userinfo_admin_bcorn
            HttpCookie myCookie = new HttpCookie("cookie_userinfo_admin_bcorn");
            myCookie.Expires = AhaTime_cl.Now.AddYears(-1);
            HttpContext.Current.Response.Cookies.Add(myCookie);
            //xóa tất cả Session
            // HttpContext.Current.Session.Clear();
            HttpContext.Current.Session["taikhoan"] = "";
            HttpContext.Current.Session["matkhau"] = "";
        }
        catch (Exception _ex)
        {
            string _tk = HttpContext.Current.Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    public static void check_login_admin(string _quyen1, string _quyen2)
    {
        try
        {
            SqlTransientGuard_cl.Execute(() =>
            {
                using (dbDataContext db = new dbDataContext())
                {
                    #region XỬ LÝ TÀI KHOẢN. LẤY TÀI KHOẢN VÀ MẬT KHẨU ĐÃ ĐƯỢC MÃ HÓA --> Giải mã
                    string _tk = "", _mk = "", _tk_mahoa = "", _mk_mahoa = "";
                    // Lấy giá trị từ cookie
                    HttpCookie _ck = HttpContext.Current.Request.Cookies["cookie_userinfo_admin_bcorn"];
                    if (_ck != null && !string.IsNullOrEmpty(_ck["taikhoan"]) && !string.IsNullOrEmpty(_ck["matkhau"]))
                    {
                        // Nếu có cookie, thì lấy giá trị từ cookie và giải mã chúng
                        _tk_mahoa = _ck["taikhoan"];
                        _mk_mahoa = _ck["matkhau"];
                        _tk = mahoa_cl.giaima_Bcorn(_tk_mahoa);
                        _mk = mahoa_cl.giaima_Bcorn(_mk_mahoa);
                    }
                    else
                    {
                        // Nếu không có cookie, thì kiểm tra session. Nếu có session, thì lấy giá trị từ session
                        if (HttpContext.Current.Session["taikhoan"] != null && HttpContext.Current.Session["matkhau"] != null)
                        {
                            _tk_mahoa = HttpContext.Current.Session["taikhoan"].ToString();
                            _mk_mahoa = HttpContext.Current.Session["matkhau"].ToString();
                            _tk = mahoa_cl.giaima_Bcorn(_tk_mahoa);
                            _mk = mahoa_cl.giaima_Bcorn(_mk_mahoa);
                        }
                    }
                    #endregion

                    #region KIỂM TRA TÍNH HỢP LỆ & QUYỀN CỦA TÀI KHOẢN
                    if (!taikhoan_cl.exist_taikhoan(_tk)) // nếu tài khoản không tồn tại
                    {
                        del_all_cookie_session_admin(); // xóa toàn bộ Cookie và Session
                                                        // lưu nội dung thông báo
                        HttpContext.Current.Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Vui lòng đăng nhập để tiếp tục.", "1000", "warning");
                        HttpContext.Current.Response.Redirect("/admin/login.aspx"); // chuyển trang và nhận thông báo
                    }
                    else // nếu tài khoản tồn tại
                    {
                        // lấy thông tin tài khoản và xử lý tiếp
                        taikhoan_tb _ob = db.taikhoan_tbs.FirstOrDefault(tk => tk.taikhoan == _tk);
                        if (_ob == null)
                        {
                            del_all_cookie_session_admin();
                            HttpContext.Current.Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Vui lòng đăng nhập để tiếp tục.", "1000", "warning");
                            HttpContext.Current.Response.Redirect("/admin/login.aspx");
                            return;
                        }

                        if (_mk != (_ob.matkhau ?? "")) // so sánh với mật khẩu được giải mã từ Cookie, nếu khác nhau
                        {
                            del_all_cookie_session_admin(); // xóa toàn bộ Cookie và Session
                            HttpContext.Current.Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Mật khẩu đã được thay đổi. Vui lòng đăng nhập lại.", "1800", "warning");
                            HttpContext.Current.Response.Redirect("/admin/login.aspx"); // chuyển trang và nhận thông báo

                        }
                        else // tiếp tục xử lý
                        {
                            bool localSuperAdmin = IsLocalSuperAdmin(_tk);
                            bool rootAdmin = IsRootAdmin(_tk);
                            if (_ob.block == true) // nếu tài khoản này bị khóa
                            {
                                del_all_cookie_session_admin(); // xóa toàn bộ Cookie và Session
                                HttpContext.Current.Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Tài khoản đã bị khóa.", "1800", "warning");
                                HttpContext.Current.Response.Redirect("/admin/login.aspx"); // chuyển trang và nhận thông báo

                            }
                            else
                            {
                                if (_ob.hansudung != null && AhaTime_cl.Now > _ob.hansudung.Value) // nếu có hạn sử dụng và hết hạn
                                {
                                    del_all_cookie_session_admin(); // xóa toàn bộ Cookie và Session
                                    HttpContext.Current.Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Tài khoản của bạn đã hết hạn sử dụng.", "1800", "warning");
                                    HttpContext.Current.Response.Redirect("/admin/login.aspx"); // chuyển trang và nhận thông báo

                                }
                                else // kiểm tra phạm vi đăng nhập
                                {
                                    bool canLoginAdmin = PortalScope_cl.CanLoginAdmin(_tk, _ob.phanloai, _ob.permission);
                                    if (!canLoginAdmin)
                                    {
                                        del_all_cookie_session_admin(); // xóa toàn bộ Cookie và Session
                                        string scope = PortalScope_cl.ResolveScope(_tk, _ob.phanloai, _ob.permission);
                                        string targetPortal = scope == PortalScope_cl.ScopeShop ? "trang gian hàng đối tác" : "AhaSale";
                                        HttpContext.Current.Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Tài khoản này chỉ được phép đăng nhập ở " + targetPortal + ".", "1800", "warning");
                                        HttpContext.Current.Response.Redirect("/admin/login.aspx"); // chuyển trang và nhận thông báo

                                    }
                                    else // kiểm tra quyền
                                    {
                                        if (PortalScope_cl.EnsureScope(_ob, PortalScope_cl.ScopeAdmin))
                                            db.SubmitChanges();

                                        string _quyen = _ob.permission ?? "";
                                        if (localSuperAdmin
                                            || rootAdmin
                                            || _quyen1 == "none"
                                            || _quyen2 == "none"
                                            || HasAnyPermission(_quyen, _quyen1, _quyen2)) // có quyền
                                        {
                                            // Gia hạn cookie. Tôi sợ lâu ngày họ quên mật khẩu, với lại sợ chậm nên chưa dùng đoạn sau
                                            //_ck["taikhoan"] = _tk_mahoa;
                                            //_ck["matkhau"] = _mk_mahoa;
                                            //_ck.Expires = AhaTime_cl.Now.AddDays(7);
                                            //HttpContext.Current.Response.Cookies.Set(_ck);
                                            // gia hạn session. Đảm bảo khi nào cũng có Session
                                            HttpContext.Current.Session["taikhoan"] = _tk_mahoa;
                                            HttpContext.Current.Session["matkhau"] = _mk_mahoa;
                                        }
                                        else // nếu k có quyền
                                        {
                                            HttpContext.Current.Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Bạn không đủ quyền thực hiện thao tác vừa rồi.", "1800", "warning");
                                            HttpContext.Current.Response.Redirect("/admin/default.aspx"); // chuyển trang và nhận thông báo

                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            });
        }
        catch (Exception ex)
        {
            if (!SqlTransientGuard_cl.IsTransient(ex))
                throw;

            del_all_cookie_session_admin();
            HttpContext.Current.Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Kết nối dữ liệu đang tạm thời gián đoạn. Vui lòng đăng nhập lại hoặc tải lại trang sau vài giây.", "2200", "warning");
            HttpContext.Current.Response.Redirect("/admin/login.aspx");
        }
    }
    #endregion

    #region home
    public static void del_all_cookie_session_home()
    {
        try
        {
            //xóa Cookie cookie_userinfo_admin_bcorn
            HttpCookie myCookie = new HttpCookie("cookie_userinfo_home_bcorn");
            myCookie.Expires = AhaTime_cl.Now.AddYears(-1);
            HttpContext.Current.Response.Cookies.Add(myCookie);
            //xóa tất cả Session
            // HttpContext.Current.Session.Clear();
            HttpContext.Current.Session["taikhoan_home"] = "";
            HttpContext.Current.Session["matkhau_home"] = "";
        }
        catch (Exception _ex)
        {
            string _tk = HttpContext.Current.Session["taikhoan_home"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }


    public static void check_login_home(string _quyen1, string _quyen2, bool _chuyentrang)
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {

            #region KIỂM TRA BẢO TRÌ HAY KHÔNG
            var q_baotri = db.CaiDatChung_tbs.Select(p => new
            {
                baotri = p.baotri_trangthai.ToString().ToLower(),
                batdau = p.baotri_thoigian_batdau,
                ketthuc = p.baotri_thoigian_ketthuc,
            }).FirstOrDefault();
            bool baotriBat = q_baotri != null && q_baotri.baotri == "true";
            bool daDenGioBaoTri = q_baotri == null || !q_baotri.batdau.HasValue || q_baotri.batdau.Value <= AhaTime_cl.Now;
            if (baotriBat && !IsLocalhostRequest() && daDenGioBaoTri)
                HttpContext.Current.Response.Redirect("/bao-tri"); // chuyển trang và nhận thông báo
            #endregion
            #region XỬ LÝ TÀI KHOẢN. LẤY TÀI KHOẢN VÀ MẬT KHẨU ĐÃ ĐƯỢC MÃ HÓA --> Giải mã
            string _tk = "", _mk = "", _tk_mahoa = "", _mk_mahoa = "";
            bool isShopBridge = false;
            bool isShopBridgePath = IsShopHomeBridgePath();
            bool isShopPortalRequest = HasShopPortalMarker();
            bool shopAccessRequested = isShopBridgePath && isShopPortalRequest;

            // ✅ Tách rõ chế độ Home/Shop: portal nào thì chỉ nhận chế độ đó.
            if (shopAccessRequested && !PortalActiveMode_cl.IsShopActive())
            {
                if (_chuyentrang)
                {
                    SetShopToast("Vui lòng chuyển sang tài khoản gian hàng đối tác để tiếp tục.", 1200, "warning");
                    HttpContext.Current.Response.Redirect("/shop/login.aspx");
                }
                return;
            }

            if (!shopAccessRequested && !PortalActiveMode_cl.IsHomeActive())
            {
                if (_chuyentrang)
                {
                    HttpContext.Current.Session["thongbao_home"] = thongbao_class.metro_notifi_onload(
                        "Thông báo",
                        "Vui lòng chuyển sang tài khoản cá nhân để tiếp tục.",
                        "1200",
                        "warning");
                    HttpContext.Current.Response.Redirect("/dang-nhap");
                }
                return;
            }

            bool allowShopBridge = shopAccessRequested && PortalActiveMode_cl.IsShopActive();

            // shop_portal phải ưu tiên phiên shop để tránh dính phiên home cũ.
            if (allowShopBridge)
            {
                string tkShop, mkShop, tkShopMaHoa, mkShopMaHoa;
                if (TryReadShopCredentialForHomeBridge(out tkShop, out mkShop, out tkShopMaHoa, out mkShopMaHoa))
                {
                    _tk = tkShop;
                    _mk = mkShop;
                    _tk_mahoa = tkShopMaHoa;
                    _mk_mahoa = mkShopMaHoa;
                    isShopBridge = true;
                }
            }

            // Chỉ đọc phiên home khi KHONG phải shop_portal.
            if (string.IsNullOrEmpty(_tk) && !shopAccessRequested && PortalActiveMode_cl.IsHomeActive())
            {
                HttpCookie _ck = HttpContext.Current.Request.Cookies["cookie_userinfo_home_bcorn"];
                if (_ck != null && !string.IsNullOrEmpty(_ck["taikhoan"]) && !string.IsNullOrEmpty(_ck["matkhau"]))
                {
                    _tk_mahoa = _ck["taikhoan"];
                    _mk_mahoa = _ck["matkhau"];
                    _tk = mahoa_cl.giaima_Bcorn(_tk_mahoa);
                    _mk = mahoa_cl.giaima_Bcorn(_mk_mahoa);
                }
                else
                {
                    if (HttpContext.Current.Session["taikhoan_home"] != null && HttpContext.Current.Session["matkhau_home"] != null)
                    {
                        _tk_mahoa = HttpContext.Current.Session["taikhoan_home"].ToString();
                        _mk_mahoa = HttpContext.Current.Session["matkhau_home"].ToString();
                        _tk = mahoa_cl.giaima_Bcorn(_tk_mahoa);
                        _mk = mahoa_cl.giaima_Bcorn(_mk_mahoa);
                    }
                }
            }
            #endregion
            bool shopFlow = shopAccessRequested || isShopBridge;
            #region KIỂM TRA TÍNH HỢP LỆ & QUYỀN CỦA TÀI KHOẢN
            if (!taikhoan_cl.exist_taikhoan(_tk)) // nếu tài khoản không tồn tại
            {
                if (!shopFlow)
                    del_all_cookie_session_home(); // xóa toàn bộ Cookie và Session
                                               // lưu nội dung thông báo
                if (_chuyentrang == true)
                {
                    if (shopFlow)
                    {
                        SetShopToast("Vui lòng đăng nhập gian hàng đối tác để tiếp tục.", 1000, "warning");
                        HttpContext.Current.Response.Redirect("/shop/login.aspx");
                    }
                    else
                    {
                        HttpContext.Current.Session["thongbao_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Vui lòng đăng nhập để tiếp tục.", "1000", "warning");
                        HttpContext.Current.Response.Redirect("/dang-nhap"); // chuyển trang và nhận thông báo
                    }
                }
            }
            else // nếu tài khoản tồn tại
            {
                // lấy thông tin tài khoản và xử lý tiếp
                taikhoan_tb _ob = db.taikhoan_tbs.FirstOrDefault(tk => tk.taikhoan == _tk);
                if (_ob == null)
                {
                    if (!shopFlow)
                        del_all_cookie_session_home(); // xóa toàn bộ Cookie và Session
                    if (_chuyentrang == true)
                    {
                        if (shopFlow)
                        {
                            SetShopToast("Vui lòng đăng nhập gian hàng đối tác để tiếp tục.", 1000, "warning");
                            HttpContext.Current.Response.Redirect("/shop/login.aspx");
                        }
                        else
                        {
                            HttpContext.Current.Session["thongbao_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Vui lòng đăng nhập để tiếp tục.", "1000", "warning");
                            HttpContext.Current.Response.Redirect("/dang-nhap");
                        }
                    }
                    return;
                }

                if (_mk != (_ob.matkhau ?? "")) // so sánh với mật khẩu được giải mã từ Cookie, nếu khác nhau
                {
                    if (!shopFlow)
                        del_all_cookie_session_home(); // xóa toàn bộ Cookie và Session
                    if (_chuyentrang == true)
                    {
                        if (shopFlow)
                        {
                            SetShopModal("Mật khẩu đã được thay đổi. <br/>Vui lòng đăng nhập lại.", "warning");
                            HttpContext.Current.Response.Redirect("/shop/login.aspx");
                        }
                        else
                        {
                            HttpContext.Current.Session["thongbao_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Mật khẩu đã được thay đổi. <br/>Vui lòng đăng nhập lại.", "false", "false", "OK", "alert", "");
                            HttpContext.Current.Response.Redirect("/dang-nhap"); // chuyển trang và nhận thông báo
                        }
                    }
                }
                else // tiếp tục xử lý
                {
                    if (_ob.block == true) // nếu tài khoản này bị khóa
                    {
                        if (!shopFlow)
                            del_all_cookie_session_home(); // xóa toàn bộ Cookie và Session
                        if (_chuyentrang == true)
                        {
                            if (shopFlow)
                            {
                                SetShopModal("Tài khoản gian hàng đối tác đã bị khóa.", "danger");
                                HttpContext.Current.Response.Redirect("/shop/login.aspx");
                            }
                            else
                            {
                                HttpContext.Current.Session["thongbao_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", "");
                                HttpContext.Current.Response.Redirect("/dang-nhap"); // chuyển trang và nhận thông báo
                            }
                        }
                    }
                    else
                    {
                        if (_ob.hansudung != null && AhaTime_cl.Now > _ob.hansudung.Value) // nếu có hạn sử dụng và hết hạn
                        {
                            if (!shopFlow)
                                del_all_cookie_session_home(); // xóa toàn bộ Cookie và Session
                            if (_chuyentrang == true)
                            {
                                if (shopFlow)
                                {
                                    SetShopModal("Tài khoản gian hàng đối tác của bạn đã hết hạn sử dụng.", "warning");
                                    HttpContext.Current.Response.Redirect("/shop/login.aspx");
                                }
                                else
                                {
                                    HttpContext.Current.Session["thongbao_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Tài khoản của bạn đã hết hạn sử dụng.", "false", "false", "OK", "alert", "");
                                    HttpContext.Current.Response.Redirect("/dang-nhap"); // chuyển trang và nhận thông báo
                                }
                            }
                        }
                        else // kiểm tra phạm vi đăng nhập
                        {
                            bool canLoginHome = PortalScope_cl.CanLoginHome(_tk, _ob.phanloai, _ob.permission);
                            bool canLoginShopBridge = allowShopBridge && PortalScope_cl.CanLoginShop(_tk, _ob.phanloai, _ob.permission);
                            if (!canLoginHome && !canLoginShopBridge)
                            {
                                if (!shopFlow)
                                    del_all_cookie_session_home(); // xóa toàn bộ Cookie và Session
                                string scope = PortalScope_cl.ResolveScope(_tk, _ob.phanloai, _ob.permission);
                                if (_chuyentrang == true)
                                {
                                    if (shopFlow)
                                    {
                                        SetShopModal("Tài khoản này không được phép dùng module này.", "warning");
                                        HttpContext.Current.Response.Redirect("/shop/default.aspx");
                                    }
                                    else
                                    {
                                        if (scope == PortalScope_cl.ScopeShop)
                                        {
                                        SetShopModal("Tài khoản này chỉ được phép đăng nhập ở trang gian hàng đối tác.", "warning");
                                            HttpContext.Current.Response.Redirect("/shop/login.aspx");
                                        }
                                        else
                                        {
                                            HttpContext.Current.Session["thongbao_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Tài khoản này chỉ được phép đăng nhập ở trang admin.", "false", "false", "OK", "alert", "");
                                            HttpContext.Current.Response.Redirect("/dang-nhap"); // chuyển trang và nhận thông báo
                                        }
                                    }
                                }
                            }
                            else // kiểm tra quyền
                            {
                                if (canLoginHome && PortalScope_cl.EnsureScope(_ob, PortalScope_cl.ScopeHome))
                                    db.SubmitChanges();
                                if (canLoginShopBridge && PortalScope_cl.EnsureScope(_ob, PortalScope_cl.ScopeShop))
                                    db.SubmitChanges();

                                if (_chuyentrang)
                                {
                                    if (canLoginShopBridge)
                                    {
                                        bool forceShopPassword = AccountResetSecurity_cl.ShouldForceShopPassword(db, _ob.taikhoan);
                                        if (forceShopPassword && !IsShopPasswordChangePath())
                                        {
                                            SetShopToast("Mật khẩu gian hàng đối tác hiện tại là mật khẩu tạm thời. Vui lòng đổi lại ngay.", 1800, "warning");
                                            HttpContext.Current.Response.Redirect("/shop/doi-mat-khau?force=1");
                                            return;
                                        }
                                    }
                                }

                                string _quyen = _ob.permission ?? "";
                                if (_quyen1 == "none" || _quyen2 == "none" || HasAnyPermission(_quyen, _quyen1, _quyen2)) // có quyền
                                {
                                    // Gia hạn cookie. Tôi sợ lâu ngày họ quên mật khẩu, với lại sợ chậm nên chưa dùng đoạn sau
                                    //_ck["taikhoan"] = _tk_mahoa;
                                    //_ck["matkhau"] = _mk_mahoa;
                                    //_ck.Expires = AhaTime_cl.Now.AddDays(7);
                                    //HttpContext.Current.Response.Cookies.Set(_ck);
                                    if (canLoginHome)
                                    {
                                        HttpContext.Current.Session["taikhoan_home"] = _tk_mahoa;
                                        HttpContext.Current.Session["matkhau_home"] = _mk_mahoa;
                                    }
                                    if (canLoginShopBridge)
                                    {
                                        HttpContext.Current.Session["taikhoan_shop"] = _tk_mahoa;
                                        HttpContext.Current.Session["matkhau_shop"] = _mk_mahoa;
                                    }
                                }
                                else // nếu k có quyền
                                {
                                    if (_chuyentrang == true)
                                    {
                                        HttpContext.Current.Session["thongbao_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
                                        HttpContext.Current.Response.Redirect("/"); // chuyển trang và nhận thông báo
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            }
        }
        catch (Exception ex)
        {
            string actor = "";
            try
            {
                actor = PortalRequest_cl.GetCurrentAccount();
            }
            catch
            {
                actor = "";
            }
            if (string.IsNullOrEmpty(actor))
                actor = "check_login_home";
            Log_cl.Add_Log(ex.Message, actor, ex.StackTrace);
        }
    }

    private static void SetShopToast(string message, int delayMs = 1200, string type = "warning", string title = "Thông báo")
    {
        if (HttpContext.Current == null)
            return;
        HttpContext.Current.Session["ThongBao_Shop"] = string.Format("toast|{0}|{1}|{2}|{3}", title, message, type, delayMs);
    }

    private static void SetShopModal(string message, string type = "warning", string title = "Thông báo")
    {
        if (HttpContext.Current == null)
            return;
        HttpContext.Current.Session["ThongBao_Shop"] = string.Format("modal|{0}|{1}|{2}|0", title, message, type);
    }


    #region shop
    private static void ExpireAuthCookie(string cookieName)
    {
        if (HttpContext.Current == null || HttpContext.Current.Response == null)
            return;

        // Gửi cả 2 biến thể secure/non-secure để chắc chắn xóa được cookie cũ sau khi chuyển HTTP <-> HTTPS.
        HttpCookie insecure = new HttpCookie(cookieName);
        insecure["taikhoan"] = "";
        insecure["matkhau"] = "";
        insecure.Expires = AhaTime_cl.Now.AddYears(-1);
        insecure.Path = "/";
        insecure.HttpOnly = true;
        insecure.Secure = false;
        HttpContext.Current.Response.Cookies.Add(insecure);

        HttpCookie secure = new HttpCookie(cookieName);
        secure["taikhoan"] = "";
        secure["matkhau"] = "";
        secure.Expires = AhaTime_cl.Now.AddYears(-1);
        secure.Path = "/";
        secure.HttpOnly = true;
        secure.Secure = true;
        HttpContext.Current.Response.Cookies.Add(secure);
    }

    public static void del_all_cookie_session_shop()
    {
        try
        {
            ExpireAuthCookie("cookie_userinfo_shop_bcorn");

            HttpContext.Current.Session["taikhoan_shop"] = "";
            HttpContext.Current.Session["matkhau_shop"] = "";
        }
        catch (Exception _ex)
        {
            string _tk = HttpContext.Current.Session["taikhoan_shop"] as string;
            if (!string.IsNullOrEmpty(_tk))
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    public static void check_login_shop(string _quyen1, string _quyen2, bool _chuyentrang)
    {
        using (dbDataContext db = new dbDataContext())
        {
            if (!PortalActiveMode_cl.IsShopActive())
            {
                if (_chuyentrang)
                {
                    SetShopToast("Vui lòng chuyển sang tài khoản gian hàng đối tác để tiếp tục.", 1200, "warning");
                    HttpContext.Current.Response.Redirect(BuildShopLoginUrl(HttpContext.Current.Request.RawUrl));
                }
                return;
            }

            string _tk = "", _mk = "", _tk_mahoa = "", _mk_mahoa = "";

            HttpCookie _ck = HttpContext.Current.Request.Cookies["cookie_userinfo_shop_bcorn"];
            if (_ck != null && !string.IsNullOrEmpty(_ck["taikhoan"]) && !string.IsNullOrEmpty(_ck["matkhau"]))
            {
                _tk_mahoa = _ck["taikhoan"];
                _mk_mahoa = _ck["matkhau"];
                _tk = mahoa_cl.giaima_Bcorn(_tk_mahoa);
                _mk = mahoa_cl.giaima_Bcorn(_mk_mahoa);
            }
            else if (HttpContext.Current.Session["taikhoan_shop"] != null && HttpContext.Current.Session["matkhau_shop"] != null)
            {
                _tk_mahoa = HttpContext.Current.Session["taikhoan_shop"].ToString();
                _mk_mahoa = HttpContext.Current.Session["matkhau_shop"].ToString();
                _tk = mahoa_cl.giaima_Bcorn(_tk_mahoa);
                _mk = mahoa_cl.giaima_Bcorn(_mk_mahoa);
            }

            if (!taikhoan_cl.exist_taikhoan(_tk))
            {
                del_all_cookie_session_shop();
                if (_chuyentrang)
                {
                    SetShopToast("Vui lòng đăng nhập để tiếp tục.", 1000, "warning");
                    HttpContext.Current.Response.Redirect(BuildShopLoginUrl(HttpContext.Current.Request.RawUrl));
                }
                return;
            }

            taikhoan_tb _ob = db.taikhoan_tbs.FirstOrDefault(tk => tk.taikhoan == _tk);
            if (_ob == null)
            {
                del_all_cookie_session_shop();
                if (_chuyentrang)
                {
                    SetShopToast("Vui lòng đăng nhập để tiếp tục.", 1000, "warning");
                    HttpContext.Current.Response.Redirect(BuildShopLoginUrl(HttpContext.Current.Request.RawUrl));
                }
                return;
            }

            if (_mk != (_ob.matkhau ?? ""))
            {
                del_all_cookie_session_shop();
                if (_chuyentrang)
                {
                    SetShopModal("Mật khẩu đã được thay đổi. <br/>Vui lòng đăng nhập lại.", "warning");
                    HttpContext.Current.Response.Redirect(BuildShopLoginUrl(HttpContext.Current.Request.RawUrl));
                }
                return;
            }

            if (_ob.block == true)
            {
                del_all_cookie_session_shop();
                if (_chuyentrang)
                {
                    SetShopModal("Tài khoản đã bị khóa.", "danger");
                    HttpContext.Current.Response.Redirect(BuildShopLoginUrl(HttpContext.Current.Request.RawUrl));
                }
                return;
            }

            if (_ob.hansudung != null && AhaTime_cl.Now > _ob.hansudung.Value)
            {
                del_all_cookie_session_shop();
                if (_chuyentrang)
                {
                    SetShopModal("Tài khoản của bạn đã hết hạn sử dụng.", "warning");
                    HttpContext.Current.Response.Redirect(BuildShopLoginUrl(HttpContext.Current.Request.RawUrl));
                }
                return;
            }

            bool canLoginShop = PortalScope_cl.CanLoginShop(_tk, _ob.phanloai, _ob.permission);
            if (!canLoginShop)
            {
                del_all_cookie_session_shop();
                string scope = PortalScope_cl.ResolveScope(_tk, _ob.phanloai, _ob.permission);
                string targetPortal = scope == PortalScope_cl.ScopeAdmin ? "trang admin" : "AhaSale";
                if (_chuyentrang)
                {
                    SetShopModal("Tài khoản này chỉ được phép đăng nhập ở " + targetPortal + ".", "warning");
                    HttpContext.Current.Response.Redirect(BuildShopLoginUrl(HttpContext.Current.Request.RawUrl));
                }
                return;
            }

            if (PortalScope_cl.EnsureScope(_ob, PortalScope_cl.ScopeShop))
                db.SubmitChanges();

            if (_chuyentrang)
            {
                bool forceShopPassword = AccountResetSecurity_cl.ShouldForceShopPassword(db, _ob.taikhoan);
                if (forceShopPassword && !IsShopPasswordChangePath())
                {
                    SetShopToast("Mật khẩu gian hàng đối tác hiện tại là mật khẩu tạm thời. Vui lòng đổi lại ngay.", 1800, "warning");
                    HttpContext.Current.Response.Redirect("/shop/doi-mat-khau?force=1");
                    return;
                }
            }

            string _quyen = _ob.permission ?? "";
            if (_quyen1 == "none" || _quyen2 == "none" || HasAnyPermission(_quyen, _quyen1, _quyen2))
            {
                HttpContext.Current.Session["taikhoan_shop"] = _tk_mahoa;
                HttpContext.Current.Session["matkhau_shop"] = _mk_mahoa;
            }
            else
            {
                if (_chuyentrang)
                {
                    SetShopModal("Bạn không đủ quyền thực hiện thao tác vừa rồi.", "warning");
                    HttpContext.Current.Response.Redirect("/shop/default.aspx");
                }
            }
        }
    }
    #endregion

    public static bool CheckQuyen(dbDataContext db, string _tk, string _quyen)
    {
        // Chỉ lấy trường permission từ database
        var permissionOfUser = db.taikhoan_tbs
            .Where(p => p.taikhoan == _tk)
            .Select(p => p.permission)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(permissionOfUser))
            return false;

        return HasAnyPermission(permissionOfUser, _quyen);
    }
    #endregion
}
