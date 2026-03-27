using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_uc_menu_top_home_uc : System.Web.UI.UserControl
{
    public string show_menu;

    private string GetCurrentHomeAccount()
    {
        string taiKhoanMaHoa = Session["taikhoan_home"] as string;
        if (string.IsNullOrEmpty(taiKhoanMaHoa))
            return "";

        return mahoa_cl.giaima_Bcorn(taiKhoanMaHoa);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            check_login_cl.check_login_home("none", "none", false);

            #region vô hiệu hóa timer trên một số trang có CKEditor
            string _url = Request.Url.AbsolutePath.ToLower();
            switch (_url)
            {
                case "/home/quan-ly-tin/default.aspx":
                    //case "/admin/quan-ly-tin-tuc/default.aspx":
                    //case "/admin/quan-ly-tai-lieu/default.aspx":
                    Timer1.Enabled = false;
                    break;
                default:
                    Timer1.Enabled = true; // Bật lại Timer nếu không phải là các trang trên
                    break;
            }
            #endregion

            DanhMuc_cl dm_cl = new DanhMuc_cl();
            show_menu = dm_cl.Show_MenuTop_Home(1, 2, false, "web", "0");//show menutop

            ViewState["sapxep_thongbao"] = "1";//mặc định sx thông báo theo mới nhất lên đầu
            but_sapxep_moinhat.CssClass = "info small rounded";

            string _tk = Session["taikhoan_home"] as string;

            if (!string.IsNullOrEmpty(_tk))//nếu có khách đăng nhập
            {
                PlaceHolder5.Visible = true;
                UpdatePanel1.Visible = true;
                UpdatePanel2.Visible = true;
                PlaceHolder7.Visible = false;
                PlaceHolder6.Visible = false;
                using (dbDataContext db = new dbDataContext())
                {
                    show_soluong_thongbao(db);
                    lay_thongtin_nguoidung(db);
                }
            }
            else//k có người đăng nhập
            {
                PlaceHolder5.Visible = false;
                UpdatePanel1.Visible = false;
                UpdatePanel2.Visible = false;
                PlaceHolder7.Visible = true;
                PlaceHolder6.Visible = true;
                Timer1.Enabled = false;
            }

        }
    }

    public void lay_thongtin_nguoidung(dbDataContext db)
    {
        string _tk = Session["taikhoan_home"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
        if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
        {
            _tk = mahoa_cl.giaima_Bcorn(_tk);

            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
            ViewState["hoten"] = q.hoten;
            ViewState["anhdaidien"] = q.anhdaidien;
            ViewState["qr_code"] = q.qr_code;
            ViewState["email"] = q.email;
            ViewState["taikhoan"] = _tk;
            ViewState["DongA"] = q.DongA.Value.ToString("#,##0");
            if (q.phanloai == "Gian hàng đối tác")
                ViewState["phanloai"] = "<div class=\"button flat-button mr-1 rounded yellow\">Gian hàng đối tác</div>";
            else if (q.phanloai == "Đồng hành hệ sinh thái")
                ViewState["phanloai"] = "<div class=\"button flat-button mr-1 rounded alert\">Đồng hành hệ sinh thái</div>";
            else if (q.phanloai == "Khách hàng")
                ViewState["phanloai"] = "<div class=\"button flat-button mr-1 rounded success\">Khách hàng</div>";
        }
    }

    #region thông báo
    protected void Timer1_Tick(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string _tk = Session["taikhoan_home"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);

                show_soluong_thongbao(db);

            }
        }
    }
    public void show_soluong_thongbao(dbDataContext db)
    {

        string _tk = GetCurrentHomeAccount();
        if (!string.IsNullOrEmpty(_tk))
        {
            // Đếm số lượng thông báo chưa đọc
            int soLuongThongBaoChuaDoc = db.ThongBao_tbs.Count(p => p.nguoinhan == _tk && p.daxem == false && p.bin == false);

            // Cập nhật nhãn hiển thị số lượng thông báo
            if (soLuongThongBaoChuaDoc < 100)
                lb_sl_thongbao.Text = soLuongThongBaoChuaDoc.ToString();
            else
                lb_sl_thongbao.Text = "99+";

            int soLuongGioHang = db.GioHang_tbs.Count(p => p.taikhoan== _tk);
            if (soLuongGioHang < 100)
                lb_sl_giohang.Text = soLuongGioHang.ToString();
            else
                lb_sl_giohang.Text = "99+";
        }
        else
        {
            lb_sl_thongbao.Text = "0";
            lb_sl_giohang.Text = "0";
        }

    }
    public void show_noidung_thongbao(dbDataContext db)
    {

        string _tk = GetCurrentHomeAccount();
        if (!string.IsNullOrEmpty(_tk))
        {
            var list_all = (from ob1 in db.ThongBao_tbs
                            join ob2 in db.taikhoan_tbs on ob1.nguoithongbao equals ob2.taikhoan into senderGroup
                            from ob2 in senderGroup.DefaultIfEmpty()
                            where ob1.nguoinhan == _tk && ob1.bin == false
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
                                    ? "/home/default.aspx?"
                                    : (ob1.link.Contains("?") ? ob1.link + "&" : ob1.link + "?")
                            }).AsQueryable();

            if (Convert.ToString(ViewState["sapxep_thongbao"]) == "2")//lọc ra chưa đọc, mới nhất lên đầu
                list_all = list_all.Where(p => p.daxem == false).OrderByDescending(p => p.thoigian);
            else//sx theo mới nhất lên đầu
                list_all = list_all.OrderByDescending(p => p.thoigian);
            var result = list_all.ToList();
            // Gán dữ liệu cho Repeater
            Repeater1.DataSource = result;
            Repeater1.DataBind();
            ph_empty_thongbao.Visible = result.Count == 0;
        }
        else
        {
            Repeater1.DataSource = new object[0];
            Repeater1.DataBind();
            ph_empty_thongbao.Visible = true;
        }

    }
    protected void but_sapxep_moinhat_Click(object sender, EventArgs e)
    {

        check_login_cl.check_login_home("none", "none", false);
        ViewState["sapxep_thongbao"] = "1";
        but_sapxep_moinhat.CssClass = "info small rounded";
        but_sapxep_chuadoc.CssClass = "light small rounded";
        using (dbDataContext db = new dbDataContext())
        {
            show_noidung_thongbao(db);
        }

    }

    protected void but_sapxep_chuadoc_Click(object sender, EventArgs e)
    {

        check_login_cl.check_login_home("none", "none", false);
        ViewState["sapxep_thongbao"] = "2";
        but_sapxep_moinhat.CssClass = "light small rounded";
        but_sapxep_chuadoc.CssClass = "info small rounded";
        using (dbDataContext db = new dbDataContext())
        {
            show_noidung_thongbao(db);
        }

    }

    protected void but_show_form_thongbao_Click(object sender, EventArgs e)
    {

        check_login_cl.check_login_home("none", "none", false);
        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk))
            return;
        using (dbDataContext db = new dbDataContext())
        {
            var q = db.ThongBao_tbs.Where(p => p.nguoinhan == _tk && p.daxem == false && p.bin == false);
            foreach(var t in q)//nhấn vô nút thông báo là đánh dấu đã xem hết
            {
                t.daxem = true;
            }
            db.SubmitChanges();
            show_noidung_thongbao(db);
            show_soluong_thongbao(db);
        }
        UpdatePanel2.Update();
        // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", mahoa_cl.giaima_Bcorn(Session["taikhoan_home"].ToString()), "1000", "warning"), true);

    }
    protected void but_chuadoc_Click(object sender, EventArgs e)
    {

        check_login_cl.check_login_home("none", "none", false);
        LinkButton button = (LinkButton)sender;
        string _id = button.CommandArgument;
        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk))
            return;
        using (dbDataContext db = new dbDataContext())
        {
            ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == _tk && p.bin == false);
            if (q == null)
                return;
            q.daxem = false;
            db.SubmitChanges();
            show_noidung_thongbao(db);
            show_soluong_thongbao(db);
            UpdatePanel1.Update();
        }

    }

    protected void but_dadoc_Click(object sender, EventArgs e)
    {

        check_login_cl.check_login_home("none", "none", false);
        LinkButton button = (LinkButton)sender;
        string _id = button.CommandArgument;
        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk))
            return;
        using (dbDataContext db = new dbDataContext())
        {
            ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == _tk && p.bin == false);
            if (q == null)
                return;
            q.daxem = true;
            db.SubmitChanges();
            show_noidung_thongbao(db);
            show_soluong_thongbao(db);
            UpdatePanel1.Update();
        }

    }
    protected void but_xoathongbao_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", false);
        LinkButton button = (LinkButton)sender;
        string _id = button.CommandArgument;
        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk))
            return;
        using (dbDataContext db = new dbDataContext())
        {
            ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == _tk && p.bin == false);
            if (q == null)
                return;
            q.bin = true;
            db.SubmitChanges();
            show_noidung_thongbao(db);
            show_soluong_thongbao(db);
            UpdatePanel1.Update();
        }

    }
    #endregion

    protected void but_dangxuat_Click(object sender, EventArgs e)
    {
        Session["taikhoan_home"] = "";
        Session["matkhau_home"] = "";
        if (Request.Cookies["cookie_userinfo_home_bcorn"] != null)
            Response.Cookies["cookie_userinfo_home_bcorn"].Expires = AhaTime_cl.Now.AddDays(-1);
        Session["thongbao_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng xuất thành công.", "1000", "warning");
        Response.Redirect("/");
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
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập đầy đủ thông tin.", "1000", "warning"), true);
                return;
            }
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
            if (q != null)
            {
                if (_pass_old != q.matkhau)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mật khẩu hiện tại không đúng.", "1000", "warning"), true);
                    return;
                }
                if (_pass_1 != _pass_2)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mật khẩu mới không trùng nhau.", "1000", "warning"), true);
                    return;
                }
                taikhoan_tb _ob = q;
                _ob.matkhau = _pass_1;
                db.SubmitChanges();
                Session["taikhoan_home"] = "";
                Session["matkhau_home"] = "";
                if (Request.Cookies["cookie_userinfo_home_bcorn"] != null)
                    Response.Cookies["cookie_userinfo_home_bcorn"].Expires = AhaTime_cl.Now.AddDays(-1);
                Session["thongbao_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.", "1000", "warning");
                Response.Redirect("/dang-nhap");
            }
            else
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng tải lại trang.", "1000", "warning"), true);
        }
    }
    protected void but_show_form_doimatkhau_Click(object sender, EventArgs e)
    {
        if (ViewState["taikhoan"].ToString() != "")
        {
            pn_doimatkhau.Visible = !pn_doimatkhau.Visible;
            up_doimatkhau.Update();
        }
    }
    protected void but_close_doimatkhau_Click(object sender, EventArgs e)
    {
        TextBox1.Text = ""; TextBox2.Text = ""; TextBox3.Text = "";
        pn_doimatkhau.Visible = !pn_doimatkhau.Visible;
    }
    #endregion

    protected void but_restpin_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan.Trim().ToLower() == ViewState["taikhoan"].ToString().ToLower());
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
                    q.email//,email_khác
                };

                string _tenmien = HttpContext.Current.Request.Url.Host.ToUpper();
                string _tieude = "Khôi phục mã pin";

                string _ma = Guid.NewGuid().ToString().ToLower();
                string _link_khoiphuc = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, "/home/khoi-phuc-ma-pin.aspx?code=" + _ma);
                DateTime _hsd = AhaTime_cl.Now.AddMinutes(5);

                string _noidung = "<div style='color:red'>Ai đó đã yêu cầu đặt lại mã pin của bạn tại " + _tenmien + "</div>";
                _noidung = _noidung + "<div style='color:red'>Nếu không phải là bạn, vui lòng bỏ qua email này và không phải làm gì cả. Tài khoản của bạn vẫn được an toàn.<hr/></div>";
                _noidung = _noidung + "<div>Tài khoản của bạn: <b>" + q.taikhoan + "</b></div>";
                _noidung = _noidung + "<div><a href='" + _link_khoiphuc + "'><b>Nhấp vào đây</b></a> để đặt lại mã pin của bạn.</div>";
                _noidung = _noidung + "<div>Thời gian hết hạn: " + _hsd.ToString("dd/MM/yyyy HH:mm") + "'</div>";
                string _ten_nguoigui = _tenmien;
                string _link_dinhkem = "";

                if (q.hsd_makhoiphuc == null || q.hsd_makhoiphuc.Value < AhaTime_cl.Now)
                {  
                    q.makhoiphuc = _ma;
                    q.hansudung = null;
                    q.hsd_makhoiphuc = _hsd;
                    db.SubmitChanges();
                    foreach (var _email_nhan in emailAddresses)
                    {
                        guiEmail_cl.SendEmail(_email_nhan, _tieude, _noidung, _ten_nguoigui, _link_dinhkem);
                    }
                }
                pn_doipin.Visible = !pn_doipin.Visible;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Chúng tôi đã gửi yêu cầu khôi phục vào email của bạn. Vui lòng kiểm tra kỹ Hộp thử đến hoặc Hộp thư rác và làm theo hướng dẫn.", "false", "false", "OK", "alert", ""), true);
                #endregion
            }
        }
    }
    #region ĐỔI PIN
    protected void but_doipin_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string _pass_old = TextBox4.Text.Trim();
            string _pass_1 = TextBox5.Text.Trim();
            string _pass_2 = TextBox6.Text.Trim();
            if (_pass_old == "" || _pass_1 == "" || _pass_2 == "")
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập đầy đủ thông tin.", "1000", "warning"), true);
                return;
            }
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
            if (q != null)
            {
                if (!PinSecurity_cl.VerifyAndUpgrade(q, _pass_old))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mã pin hiện tại không đúng.", "1000", "warning"), true);
                    return;
                }
                if (_pass_1 != _pass_2)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mã pin mới không trùng nhau.", "1000", "warning"), true);
                    return;
                }
                if (!PinSecurity_cl.IsValidPinFormat(_pass_1))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mã pin mới phải gồm đúng 4 chữ số.", "1000", "warning"), true);
                    return;
                }
                taikhoan_tb _ob = q;
                _ob.mapin_thanhtoan = PinSecurity_cl.HashPin(_pass_1);
                db.SubmitChanges();
                TextBox4.Text = ""; TextBox5.Text = ""; TextBox6.Text = "";
                pn_doipin.Visible = !pn_doipin.Visible;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Đổi pin thành công.", "1000", "warning"), true);
            }
            else
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng tải lại trang.", "1000", "warning"), true);
        }
    }
    protected void but_show_form_doipin_Click(object sender, EventArgs e)
    {
        if (ViewState["taikhoan"].ToString() != "")
        {
            pn_doipin.Visible = !pn_doipin.Visible;
            up_doipin.Update();
        }
    }
    protected void but_close_doipin_Click(object sender, EventArgs e)
    {
        TextBox4.Text = ""; TextBox5.Text = ""; TextBox6.Text = "";
        pn_doipin.Visible = !pn_doipin.Visible;
    }
    #endregion
}
