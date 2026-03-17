using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class taikhoan_add : System.Web.UI.Page
{
    public string notifi, user_parent, user, url_back, trangthai, ngaysinh, ngaytao, nguoitao, email, sdt, zalo, facebook, hsd, songaycong,luongcb;
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    string_class str_cl = new string_class();
    datetime_class dt_cl = new datetime_class();
    public string hoten;
    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login
        string _quyen = "none";
        string _cookie_user = "", _cookie_pass = "";
        if (Request.Cookies["save_user_admin_aka_1"] != null) _cookie_user = Request.Cookies["save_user_admin_aka_1"].Value;
        if (Request.Cookies["save_pass_admin_aka_1"] != null) _cookie_pass = Request.Cookies["save_pass_admin_aka_1"].Value;
        if (Session["user"] == null) Session["user"] = ""; if (Session["notifi"] == null) Session["notifi"] = "";if (Session["user"].ToString() == "") Response.Redirect("/gianhang/admin/f5_ss_admin.aspx");
        string _url = Request.Url.GetLeftPart(UriPartial.Authority).ToLower();
        string _kq = bcorn_class.check_login(Session["user"].ToString(), _cookie_user, _cookie_pass, _url, _quyen);
        if (_kq != "")//nếu có thông báo --> có lỗi --> reset --> bắt login lại
        {
            if (_kq == "baotri") Response.Redirect("/baotri.aspx");
            else
            {
                if (_kq == "1") Response.Redirect("/gianhang/admin/login.aspx");//hết Session, hết Cookie
                else
                {
                    if (_kq == "2")//k đủ quyền
                    {
                        Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
                        Response.Redirect("/gianhang/admin");
                    }
                    else
                    {
                        Session["notifi"] = _kq; Session["user"] = "";
                        Response.Cookies["save_user_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["save_pass_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["save_url_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Redirect("/gianhang/admin/login.aspx");
                    }
                }
            }
        }
        #endregion

        #region Check quyen theo nganh
        user = Request.QueryString["user"].ToString();
        user_parent = "admin";
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q2_2") == "" || bcorn_class.check_quyen(Session["user"].ToString(), "n2_2") == "" || user == Session["user"].ToString())
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["user"]))
            {
                user = Request.QueryString["user"].ToString().Trim();
                if (tk_cl.exist_user(user))
                {
                        main();
                        if (Request.Cookies["save_url_admin_aka_1"] != null)
                            url_back = Request.Cookies["save_url_admin_aka_1"].Value;
                        else
                            url_back = "/gianhang/admin";
                }
                else
                {
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                    Response.Redirect("/gianhang/admin");
                }
            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin");
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        #endregion
       
    }
    //protected void Page_InitComplete(object sender, EventArgs e)
    //{

    //}
    public void main()
    {
        taikhoan_table_2023 _ob = db.taikhoan_table_2023s.Where(p => p.taikhoan == user && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        if (!IsPostBack)
        {
            hoten = _ob.hoten;
            trangthai = _ob.trangthai;
            ngaysinh = _ob.ngaysinh != null ? _ob.ngaysinh.Value.ToString("dd/MM/yyyy") : "";
            ngaytao = _ob.ngaytao.Value.ToString("dd/MM/yyyy");
            nguoitao = _ob.nguoitao;
            songaycong = _ob.songaycong.ToString();
            luongcb = _ob.luongcoban.Value.ToString("#,##0");

            email = _ob.email; sdt = _ob.dienthoai; zalo = _ob.zalo; facebook = _ob.facebook;
            hsd = _ob.hansudung != null ? _ob.hansudung.Value.ToString("dd/MM/yyyy") : "Không có hạn";

            if (_ob.trangthai == "Đang hoạt động")
            {
                but_khoa.Visible = true;
                but_mokhoa.Visible = false;
            }
            else
            {
                but_khoa.Visible = false;
                but_mokhoa.Visible = true;
            }
        }
        if (_ob.anhdaidien != "")
            Label2.Text = "<img src='" + _ob.anhdaidien + "' class='img-cover-vuongtron' width='100' height='100' />";
        else
            Label2.Text = "<img src='/uploads/images/macdinh.jpg' class='img-cover-vuongtron' width='100' height='100' />";
    }



    protected void but_khoa_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q2_3") == ""|| bcorn_class.check_quyen(Session["user"].ToString(), "n2_3") == "")// ="": có quyền; =2: k có quyền
        {
            if (user == "admin" && Session["user"].ToString() != "admin")
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không thể khóa tài khoản admin.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin");
            }
            else
            {
                var q = db.taikhoan_table_2023s.Where(p => p.taikhoan == user && p.id_chinhanh == Session["chinhanh"].ToString());
                if (q.Count() != 0)
                {
                    taikhoan_table_2023 _ob = q.First();
                    _ob.trangthai = "Đã bị khóa";
                    db.SubmitChanges();
                }
                //main();
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Khóa thành công.", "4000", "warning"), true);
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Khóa thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + user);
            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Bạn không có đủ quyền để thực hiện hành động này.", "4000", "warning"), true);
    }

    protected void but_mokhoa_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q2_3") == "" || bcorn_class.check_quyen(Session["user"].ToString(), "n2_3") == "")// ="": có quyền; =2: k có quyền
        {
            if (user == "admin" && Session["user"].ToString() != "admin")
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không thể mở khóa tài khoản admin.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin");
            }
            else
            {
                var q = db.taikhoan_table_2023s.Where(p => p.taikhoan == user&& p.id_chinhanh == Session["chinhanh"].ToString());
                if (q.Count() != 0)
                {
                    taikhoan_table_2023 _ob = q.First();
                    _ob.trangthai = "Đang hoạt động";
                    db.SubmitChanges();
                }
                //main();
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mở khóa thành công.", "4000", "warning"), true);
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Mở khóa thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + user);
            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Bạn không có đủ quyền để thực hiện hành động này.", "4000", "warning"), true);
    }
}