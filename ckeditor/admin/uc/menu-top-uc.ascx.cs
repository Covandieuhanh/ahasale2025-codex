using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_uc_menu_top_uc : System.Web.UI.UserControl
{
    private const string TopViewQueryKey = "topview";
    private const string TopViewChangePassword = "change-password";

    public string MenuActive(params string[] urls)
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        foreach (string url in urls)
        {
            if (currentUrl == (url ?? "").ToLower().Trim())
                return "active";
        }
        return "";
    }

    public string MenuActiveTaiKhoanScope(string scope)
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        if (currentUrl != "/admin/quan-ly-tai-khoan/Default.aspx")
            return "";

        string currentScope = (Request.QueryString["scope"] ?? "").Trim().ToLowerInvariant();
        string targetScope = (scope ?? "").Trim().ToLowerInvariant();
        if (currentScope == targetScope)
            return "active";
        return "";
    }

    private string GetCurrentAdminAccount()
    {
        string taiKhoanMaHoa = Session["taikhoan"] as string;
        if (string.IsNullOrEmpty(taiKhoanMaHoa))
            return "";

        return mahoa_cl.giaima_Bcorn(taiKhoanMaHoa);
    }

    private string BuildCurrentPageUrl(bool openChangePassword)
    {
        var query = HttpUtility.ParseQueryString(Request.QueryString.ToString());
        if (openChangePassword)
            query[TopViewQueryKey] = TopViewChangePassword;
        else
            query.Remove(TopViewQueryKey);

        string queryString = query.ToString();
        return Request.Url.AbsolutePath + (string.IsNullOrEmpty(queryString) ? "" : "?" + queryString);
    }

    private void RedirectTo(string url)
    {
        ScriptManager sm = ScriptManager.GetCurrent(Page);
        if (sm != null && sm.IsInAsyncPostBack)
        {
            string safeUrl = (url ?? "").Replace("'", "\\'");
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "window.location='" + safeUrl + "';", true);
            return;
        }

        Response.Redirect(url, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void ApplyTopViewFromQuery()
    {
        string topView = (Request.QueryString[TopViewQueryKey] ?? "").Trim().ToLowerInvariant();
        if (topView != TopViewChangePassword)
            return;

        if (string.IsNullOrEmpty(ViewState["taikhoan"] as string))
            return;

        pn_doimatkhau.Visible = true;
        up_doimatkhau.Update();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                #region vô hiệu hóa timer trên một số trang có CKEditor
                string _url = Request.Url.AbsolutePath.ToLower();
                switch (_url)
                {
                    case "/admin/quan-ly-bai-viet/default.aspx":
                    case "/admin/motacapbac.aspx":
                        //case "/admin/quan-ly-tin-tuc/default.aspx":
                        //case "/admin/quan-ly-tai-lieu/default.aspx":
                        Timer1.Enabled = false;
                        break;
                    default:
                        Timer1.Enabled = true; // Bật lại Timer nếu không phải là các trang trên
                        break;
                }
                #endregion


                if (Session["title"] != null)
                    ViewState["title"] = Session["title"].ToString();

                ViewState["sapxep_thongbao"] = "1";//mặc định sx thông báo theo mới nhất lên đầu
                but_sapxep_moinhat.CssClass = "info small rounded";

                using (dbDataContext db = new dbDataContext())
                {
                    show_soluong_thongbao(db);
                    lay_thongtin_nguoidung(db);
                }

                ApplyTopViewFromQuery();

            }
            catch (Exception _ex)
            {
                string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
                if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                    _tk = mahoa_cl.giaima_Bcorn(_tk);
                else
                    _tk = "";
                Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            }
        }
    }

    public void lay_thongtin_nguoidung(dbDataContext db)
    {
        string _tk = Session["taikhoan"] as string;
        if (!string.IsNullOrEmpty(_tk))
        {
            _tk = mahoa_cl.giaima_Bcorn(_tk);
            try
            {
                var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
                if (q == null) return;

                ViewState["hoten"] = q.hoten;
                ViewState["anhdaidien"] = q.anhdaidien;
                ViewState["email"] = q.email;
                ViewState["taikhoan"] = _tk;

                // Hiển thị nhãn theo hệ đăng nhập admin.
                if (AccountType_cl.IsTreasury(q.phanloai))
                {
                    ViewState["phanloai"] =
                        "<div class=\"button flat-button mr-1 rounded success\">Tài khoản tổng</div>";
                }
                else
                {
                    ViewState["phanloai"] =
                        "<div class=\"button flat-button mr-1 rounded info\">Nhân viên admin</div>";
                }


                ViewState["DongA"] = (q.DongA ?? 0).ToString("#,##0");
                // =============================
            }
            catch (Exception _ex)
            {
                Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            }
        }
    }


    #region thông báo
    protected void Timer1_Tick(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            show_soluong_thongbao(db);
        }
    }

    public void show_soluong_thongbao(dbDataContext db)
    {
        try
        {
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
            {
                lb_sl_thongbao.Text = "0";
                return;
            }

            // Đếm số lượng thông báo chưa đọc
            int soLuongThongBaoChuaDoc = db.ThongBao_tbs.Count(p => p.nguoinhan == taiKhoan && p.daxem == false && p.bin == false);

            // Cập nhật nhãn hiển thị số lượng thông báo
            if (soLuongThongBaoChuaDoc < 100)
                lb_sl_thongbao.Text = soLuongThongBaoChuaDoc.ToString();
            else
                lb_sl_thongbao.Text = "99+";
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    public void show_noidung_thongbao(dbDataContext db)
    {
        try
        {
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
            {
                Repeater1.DataSource = new object[0];
                Repeater1.DataBind();
                ph_empty_thongbao.Visible = true;
                return;
            }

            var list_all = (from ob1 in db.ThongBao_tbs
                            join ob2 in db.taikhoan_tbs on ob1.nguoithongbao equals ob2.taikhoan into senderGroup
                            from ob2 in senderGroup.DefaultIfEmpty()
                            where ob1.nguoinhan == taiKhoan && ob1.bin == false
                            select new
                            {
                                ob1.id, // id thông báo
                                avt_nguoithongbao = (ob2 == null || ob2.anhdaidien == null || ob2.anhdaidien == "")
                                    ? "/uploads/images/macdinh.jpg"
                                    : ob2.anhdaidien,
                                daxem = ob1.daxem,
                                noidung = ob1.noidung ?? "",
                                thoigian = ob1.thoigian,
                                link = (ob1.link == null || ob1.link == "")
                                    ? "/admin/default.aspx?"
                                    : (ob1.link.Contains("?") ? ob1.link + "&" : ob1.link + "?")
                            }).AsQueryable();

            if (Convert.ToString(ViewState["sapxep_thongbao"]) == "2")//lọc ra chưa đọc, mới nhất lên đầu
                list_all = list_all.Where(p => p.daxem == false).OrderByDescending(p => p.thoigian).Take(20);
            else//sx theo mới nhất lên đầu
                list_all = list_all.OrderByDescending(p => p.thoigian).Take(20);

            var result = list_all.ToList();
            // Gán dữ liệu cho Repeater
            Repeater1.DataSource = result;
            Repeater1.DataBind();
            ph_empty_thongbao.Visible = result.Count == 0;
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
            Repeater1.DataSource = new object[0];
            Repeater1.DataBind();
            ph_empty_thongbao.Visible = true;
        }
    }
    protected void but_sapxep_moinhat_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["sapxep_thongbao"] = "1";
            but_sapxep_moinhat.CssClass = "info small rounded";
            but_sapxep_chuadoc.CssClass = "light small rounded";
            using (dbDataContext db = new dbDataContext())
            {
                show_noidung_thongbao(db);
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_sapxep_chuadoc_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["sapxep_thongbao"] = "2";
            but_sapxep_moinhat.CssClass = "light small rounded";
            but_sapxep_chuadoc.CssClass = "info small rounded";
            using (dbDataContext db = new dbDataContext())
            {
                show_noidung_thongbao(db);
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_show_form_thongbao_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            using (dbDataContext db = new dbDataContext())
            {
                show_noidung_thongbao(db);
            }
            UpdatePanel2.Update();
            // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString()), "1000", "warning"), true);
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void but_chuadoc_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == taiKhoan && p.bin == false);
                if (q == null)
                    return;
                q.daxem = false;
                db.SubmitChanges();
                show_noidung_thongbao(db);
                show_soluong_thongbao(db);
                UpdatePanel1.Update();
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_dadoc_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == taiKhoan && p.bin == false);
                if (q == null)
                    return;
                q.daxem = true;
                db.SubmitChanges();
                show_noidung_thongbao(db);
                show_soluong_thongbao(db);
                UpdatePanel1.Update();
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void but_xoathongbao_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == taiKhoan && p.bin == false);
                if (q == null)
                    return;
                q.bin = true;
                db.SubmitChanges();
                show_noidung_thongbao(db);
                show_soluong_thongbao(db);
                UpdatePanel1.Update();
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    #endregion

    protected void but_dangxuat_Click(object sender, EventArgs e)
    {
        Session["taikhoan"] = "";
        Session["matkhau"] = "";
        if (Request.Cookies["cookie_userinfo_admin_bcorn"] != null)
            Response.Cookies["cookie_userinfo_admin_bcorn"].Expires = AhaTime_cl.Now.AddDays(-1);
        Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng xuất thành công.", "2000", "warning");
        Response.Redirect("/admin/login.aspx");
    }
    #region ĐỔI MẬT KHẨU
    protected void but_doimatkhau_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string _pass_old = TextBox1.Text.Trim();
            string _pass_1 = TextBox2.Text.Trim();
            string _pass_2 = TextBox3.Text.Trim();
            if (_pass_old == "" || _pass_1 == "" || _pass_2 == "")
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập đầy đủ thông tin.", "2000", "warning"), true);
                return;
            }
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
            if (q != null)
            {
                if (_pass_old != q.matkhau)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mật khẩu hiện tại không đúng.", "2000", "warning"), true);
                    return;
                }
                if (_pass_1 != _pass_2)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mật khẩu mới không trùng nhau.", "2000", "warning"), true);
                    return;
                }
                taikhoan_tb _ob = q;
                _ob.matkhau = _pass_1;
                db.SubmitChanges();
                Session["taikhoan"] = "";
                Session["matkhau"] = "";
                if (Request.Cookies["cookie_userinfo_admin_bcorn"] != null)
                    Response.Cookies["cookie_userinfo_admin_bcorn"].Expires = AhaTime_cl.Now.AddDays(-1);
                Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.", "2000", "warning");
                Response.Redirect("/admin/login.aspx");
            }
            else
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng tải lại trang.", "2000", "warning"), true);
        }
    }
    protected void but_show_form_doimatkhau_Click(object sender, EventArgs e)
    {
        string taiKhoan = ViewState["taikhoan"] as string ?? "";
        if (taiKhoan != "")
        {
            string topView = (Request.QueryString[TopViewQueryKey] ?? "").Trim().ToLowerInvariant();
            if (topView == TopViewChangePassword)
                RedirectTo(BuildCurrentPageUrl(false));
            else
                RedirectTo(BuildCurrentPageUrl(true));
        }
    }
    protected void but_close_doimatkhau_Click(object sender, EventArgs e)
    {
        TextBox1.Text = ""; TextBox2.Text = ""; TextBox3.Text = "";
        RedirectTo(BuildCurrentPageUrl(false));
    }
    #endregion 

}
