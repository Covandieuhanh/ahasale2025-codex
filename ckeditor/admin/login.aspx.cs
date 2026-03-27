using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_Default2 : System.Web.UI.Page
{
    private const string ViewRecover = "recover";

    private string BuildLoginUrl()
    {
        return ResolveUrl("~/admin/login.aspx");
    }

    private string BuildRecoverUrl()
    {
        return BuildLoginUrl() + "?view=" + ViewRecover;
    }

    private void RedirectTo(string url)
    {
        Response.Redirect(url, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void ApplyRecoverViewFromQuery()
    {
        string view = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
        if (view == ViewRecover)
        {
            pn_khoiphuc.Visible = true;
            up_khoiphuc.Update();
            txt_email_khoiphuc.Focus();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                #region THÔNG TIN TRANG
                using (dbDataContext db = new dbDataContext())
                {

                    var q = (from tk in db.CaiDatChung_tbs
                             where tk.phanloai_trang == "login"
                             select new { tk.thongtin_icon, tk.thongtin_apple_touch_icon, tk.lienket_chiase_title, tk.lienket_chiase_description, tk.lienket_chiase_image }).FirstOrDefault();

                    if (q != null)
                    {
                        string baseUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);

                        string iconUrl = string.Format("{0}{1}", baseUrl, q.thongtin_icon);
                        string appleTouchIconUrl = string.Format("{0}{1}", baseUrl, q.thongtin_apple_touch_icon);

                        string iconsHtml = string.Format(@"
                <!-- Favicon -->
                <link rel='icon' href='{0}' sizes='16x16' type='image/x-icon'>
                <link rel='icon' href='{1}' sizes='32x32' type='image/x-icon'>
                <link rel='icon' href='{2}' sizes='48x48' type='image/x-icon'>

                <!-- Apple Touch Icon -->
                <link rel='apple-touch-icon' href='{3}' sizes='180x180'>
                <link rel='apple-touch-icon' href='{4}' sizes='167x167'>
                <link rel='apple-touch-icon' href='{5}' sizes='152x152'>
                <link rel='apple-touch-icon' href='{6}' sizes='120x120'>

                <!-- Android Icons -->
                <link rel='icon' href='{7}' sizes='192x192'>
                <link rel='icon' href='{8}' sizes='144x144'>
                ", iconUrl, iconUrl, iconUrl, appleTouchIconUrl, appleTouchIconUrl, appleTouchIconUrl, appleTouchIconUrl, iconUrl, iconUrl);

                        string title = q.lienket_chiase_title;
                        string description = q.lienket_chiase_description;
                        string imageRelativePath = q.lienket_chiase_image;

                        // Tạo URL tuyệt đối cho hình ảnh
                        string imageUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, imageRelativePath);

                        string metaTags = string.Format(@"
                    <!-- Title -->
                    <title>{0}</title>

                    <!-- Meta Description -->
                    <meta name='description' content='{1}' />

                    <!-- Open Graph Meta Tags -->
                    <meta property='og:title' content='{2}' />
                    <meta property='og:description' content='{3}' />
                    <meta property='og:image' content='{4}' />
                    <meta property='og:type' content='website' />
                    <meta property='og:url' content='{5}' />

                    <!-- Twitter Card Meta Tags -->
                    <meta name='twitter:card' content='summary_large_image' />
                    <meta name='twitter:title' content='{6}' />
                    <meta name='twitter:description' content='{7}' />
                    <meta name='twitter:image' content='{8}' />
                ", title, description, title, description, imageUrl, Request.Url.AbsoluteUri, title, description, imageUrl);
                        //literal_fav_icon.Text = iconsHtml + metaTags;
                    }

                }
                #endregion
                #region KIỂM TRA ĐÃ ĐĂNG NHẬP HAY CHƯA
                // Lấy giá trị từ cookie
                string _tk = "";
                HttpCookie _ck = Request.Cookies["cookie_userinfo_admin_bcorn"];
                if (_ck != null && !string.IsNullOrEmpty(_ck["taikhoan"]))// Nếu có cookie, thì lấy giá trị từ cookie và giải mã chúng
                    _tk = mahoa_cl.giaima_Bcorn(_ck["taikhoan"]);
                else
                {
                    // Nếu không có cookie, thì kiểm tra session. Nếu có session, thì lấy giá trị từ session
                    if (Session["taikhoan"] != null)
                        _tk = mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString());
                }
                if (taikhoan_cl.exist_taikhoan(_tk)) // nếu tài khoản tồn tại
                {
                    taikhoan_tb acc;
                    using (dbDataContext dbLogin = new dbDataContext())
                    {
                        acc = dbLogin.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
                    }
                    bool canLoginAdmin = acc != null && PortalScope_cl.CanLoginAdmin(acc.taikhoan, acc.phanloai, acc.permission);
                    if (canLoginAdmin)
                    {
                        string _url_back = Session["url_back"] as string; // Sử dụng 'as' để tránh lỗi nếu 'url_back' là null
                        if (!string.IsNullOrEmpty(_url_back)) // Kiểm tra xem '_url_back' có hợp lệ hay không
                        {
                            Response.Redirect(_url_back, false);
                        }
                        else
                        {
                            Response.Redirect("/admin/default.aspx", false);
                        }
                        Context.ApplicationInstance.CompleteRequest(); // Hoàn tất yêu cầu mà không ném 'ThreadAbortException'
                    }
                    else
                    {
                        check_login_cl.del_all_cookie_session_admin();
                        string scope = acc == null ? "" : PortalScope_cl.ResolveScope(acc.taikhoan, acc.phanloai, acc.permission);
                        string targetPortal = scope == PortalScope_cl.ScopeShop ? "trang shop" : "AhaSale";
                        Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Tài khoản này chỉ được phép đăng nhập ở " + targetPortal + ".", "1800", "warning");
                    }
                }

                //đưa vào trong trang chủ của admin rồi tính kiểm tra tiếp tính hợp lệ của tài khoản
                #endregion
                #region lưu nội dung thông báo nếu có
                if (Session["thongbao"] != null)
                {
                    ViewState["thongbao"] = Session["thongbao"].ToString();
                    Session["thongbao"] = null;
                }
                #endregion
                ApplyRecoverViewFromQuery();
            }
            catch (Exception _ex)
            {
                string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
                if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                {
                    _tk = mahoa_cl.giaima_Bcorn(_tk);
                }
                else
                    _tk = "";
                Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            }
        }
    }


    protected void but_login_Click(object sender, EventArgs e)
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                string _loginId = AccountAuth_cl.NormalizeLoginId(txt_user.Text);
                string _pass = txt_pass.Text ?? "";
                if (_loginId == "")
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập tài khoản/email/số điện thoại.", "5000", "warning"), true);
                else
                {
                    if (_pass == "")
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mật khẩu.", "false", "false", "OK", "alert", ""), true);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập mật khẩu.", "5000", "warning"), true);
                    else
                    {
                        AccountLoginInfo account = AccountAuth_cl.FindAccountByLoginId(db, _loginId, PortalScope_cl.ScopeAdmin);
                        if (account != null && account.IsAmbiguous)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Email hoặc số điện thoại đang trùng nhiều tài khoản. Vui lòng dùng tên tài khoản.", "3000", "warning"), true);
                            return;
                        }
                        if (account != null && !string.IsNullOrEmpty(account.TaiKhoan))
                        {
                            if (!AccountAuth_cl.IsPasswordValid(_pass, account.MatKhau))
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mật khẩu không đúng.", "2000", "warning"), true);
                                return;
                            }

                            if (account.Block)
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Tài khoản đã bị khóa.", "2500", "warning"), true);
                                return;
                            }

                            if (account.HanSuDung != null && AhaTime_cl.Now > account.HanSuDung.Value)
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Tài khoản đã hết hạn sử dụng.", "2500", "warning"), true);
                                return;
                            }

                            taikhoan_tb fullAccount = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == account.TaiKhoan);
                            if (fullAccount == null)
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Tài khoản không tồn tại.", "2000", "warning"), true);
                                return;
                            }

                            if (!PortalScope_cl.CanLoginAdmin(fullAccount.taikhoan, fullAccount.phanloai, fullAccount.permission))
                            {
                                string scope = PortalScope_cl.ResolveScope(fullAccount.taikhoan, fullAccount.phanloai, fullAccount.permission);
                                string targetPortal = scope == PortalScope_cl.ScopeShop ? "trang shop" : "AhaSale";
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Tài khoản này chỉ được phép đăng nhập ở " + targetPortal + ".", "2600", "warning"), true);
                                return;
                            }

                            if (PortalScope_cl.EnsureScope(fullAccount, PortalScope_cl.ScopeAdmin))
                            {
                                db.SubmitChanges();
                            }

                            string _taikhoan_mahoa = mahoa_cl.mahoa_Bcorn(account.TaiKhoan);
                            string _matkhau_mahoa = mahoa_cl.mahoa_Bcorn(account.MatKhau);
                            //lưu cookier với thông tin tài khoản để lưu giữ đăng nhập trong 7 ngày;
                            HttpCookie _ck = new HttpCookie("cookie_userinfo_admin_bcorn");
                            _ck["taikhoan"] = _taikhoan_mahoa;
                            _ck["matkhau"] = _matkhau_mahoa;
                            _ck.Expires = AhaTime_cl.Now.AddDays(7);
                            // Đặt thuộc tính HttpOnly để ngăn chặn truy cập từ mã JavaScript
                            _ck.HttpOnly = true;
                            // Đặt thuộc tính Secure để chỉ cho phép truyền cookie qua kết nối an toàn
                            _ck.Secure = AccountAuth_cl.ShouldUseSecureCookie(Request);
                            //chỉ định tên miền mà cookie được áp dụng. Bằng cách này, cookie chỉ được gửi đến máy chủ từ tên miền đã chỉ định, các miền con sẽ đc áp dụng theo
                            //_ck.Domain = "https://bcorn.net";//bị ảnh hưởng khi ở localhost
                            Response.Cookies.Add(_ck);

                            //lưu session
                            Session["taikhoan"] = _taikhoan_mahoa;
                            Session["matkhau"] = _matkhau_mahoa;
                            Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng nhập thành công.", "1000", "warning");

                            string _url_back = Convert.ToString(Session["url_back"]);

                            if (!string.IsNullOrEmpty(_url_back))
                            {
                                Response.Redirect(_url_back, false);
                            }
                            else
                            {
                                Response.Redirect("/admin/default.aspx", false);
                            }

                            Context.ApplicationInstance.CompleteRequest();

                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Tài khoản không tồn tại.", "2000", "warning"), true);

                    }
                }
            }
        }
        catch (Exception _ex)
        {
            Log_cl.Add_Log(_ex.Message, "", _ex.StackTrace);
        }
    }

    #region khôi phục mật khẩu
    protected void but_show_form_quenmk_Click(object sender, EventArgs e)
    {
        string view = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
        if (view == ViewRecover)
            RedirectTo(BuildLoginUrl());
        else
            RedirectTo(BuildRecoverUrl());
    }
    protected void but_close_form_quenmk_Click(object sender, EventArgs e)
    {
        txt_email_khoiphuc.Text = "";
        RedirectTo(BuildLoginUrl());
    }
    protected void but_nhanma_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            String_cl str_cl = new String_cl();
            string _email = txt_email_khoiphuc.Text.Trim().ToLower();
            if (str_cl.KiemTra_Email(_email) == false)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Email không hợp lệ.", "false", "false", "OK", "alert", ""), true);
                return;
            }
            var scopedAccounts = db.taikhoan_tbs
                .Where(p => (p.email ?? "").Trim().ToLower() == _email)
                .ToList()
                .Where(p => PortalScope_cl.CanLoginAdmin(p.taikhoan, p.phanloai, p.permission))
                .ToList();

            if (scopedAccounts.Count > 1)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Email này đang trùng nhiều tài khoản trong hệ admin. Vui lòng liên hệ quản trị để xử lý.", "false", "false", "OK", "alert", ""), true);
                return;
            }

            taikhoan_tb q = scopedAccounts.FirstOrDefault();
            if (q != null)
            {
                #region THÔNG BÁO QUA EMAIL
                // Lấy danh sách email từ bảng taikhoan_tb
                //var emailAddresses = db.taikhoan_tbs
                //    .Where(tk => tk.taikhoan == "admin" /*|| tk.username == "bonbap"*/)
                //    .Select(tk => tk.email)
                //    .ToList();
                //gán mail trực tiếp

                List<string> emailAddresses = new List<string>
                {
                    _email//,email_khác
                };

                string _tenmien = HttpContext.Current.Request.Url.Host.ToUpper();
                string _tieude = "Khôi phục mật khẩu";

                string _ma = Guid.NewGuid().ToString().ToLower();
                string _link_khoiphuc = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, "/admin/khoi-phuc-mat-khau.aspx?code=" + _ma);
                DateTime _hsd = AhaTime_cl.Now.AddMinutes(5);

                string _noidung = "<div style='color:red'>Ai đó đã yêu cầu đặt lại mật khẩu của bạn tại " + _tenmien + "</div>";
                _noidung = _noidung + "<div style='color:red'>Nếu không phải là bạn, vui lòng bỏ qua email này và không phải làm gì cả. Tài khoản của bạn vẫn được an toàn.<hr/></div>";
                _noidung = _noidung + "<div>Tài khoản của bạn: <b>" + q.taikhoan + "</b></div>";
                _noidung = _noidung + "<div><a href='" + _link_khoiphuc + "'><b>Nhấp vào đây</b></a> để đặt lại mật khẩu của bạn.</div>";
                _noidung = _noidung + "<div>Thời gian hết hạn: " + _hsd.ToString("dd/MM/yyyy HH:mm") + "'</div>";
                string _ten_nguoigui = _tenmien;
                string _link_dinhkem = "";


                if (q.hsd_makhoiphuc == null || q.hsd_makhoiphuc.Value < AhaTime_cl.Now)
                {  //đã quá 5 phút thì mới cho gửi lại
                    
                    q.makhoiphuc = _ma;
                    q.hansudung = null;
                    q.hsd_makhoiphuc = _hsd;
                    db.SubmitChanges();
                    foreach (var _email_nhan in emailAddresses)
                    {
                        guiEmail_cl.SendEmail(_email_nhan, _tieude, _noidung, _ten_nguoigui, _link_dinhkem);
                    }
                }

                txt_email_khoiphuc.Text = "";
                Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đã gửi yêu cầu khôi phục vào email của bạn. Vui lòng kiểm tra hộp thư đến hoặc thư rác.", "2500", "warning");
                RedirectTo(BuildLoginUrl());
                #endregion
            }
        }
    }
    #endregion
}
