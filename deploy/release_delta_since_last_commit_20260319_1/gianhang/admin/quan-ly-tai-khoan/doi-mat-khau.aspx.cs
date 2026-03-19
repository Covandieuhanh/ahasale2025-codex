using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class gianhang_taikhoan_doi_mat_khau : System.Web.UI.Page
{
    public string notifi, user, url_back;
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    public string txt_oldpass = "", txt_pass1 = "", txt_pass2 = "";
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
        string qsUser = (Request.QueryString["user"] ?? "").Trim();
        user = qsUser;
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q2_3") == "" || bcorn_class.check_quyen(Session["user"].ToString(), "n2_3") == "" || user == Session["user"].ToString())
        {
            if (!string.IsNullOrWhiteSpace(qsUser))
            {
                user = qsUser;
                if (tk_cl.exist_user(user))
                {
                    if (user == "admin" && Session["user"].ToString() != "admin")
                    {
                        Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không thể đổi mật khẩu cho admin.", "false", "false", "OK", "alert", "");
                        Response.Redirect("/gianhang/admin");
                    }
                    else
                    {
                        if (Request.Cookies["save_url_admin_aka_1"] != null)
                            url_back = Request.Cookies["save_url_admin_aka_1"].Value;
                        else
                            url_back = "/gianhang/admin";
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


    protected void button1_Click1(object sender, EventArgs e)
    {
        taikhoan_table_2023 q = tk_cl.return_object(user);
        //string _oldpass = Request.Form["txt_oldpass"];
        string _pass1 = Request.Form["txt_pass1"];
        string _pass2 = Request.Form["txt_pass2"];
        if (_pass1 == "" || _pass2 == "")
        {
            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng nhập đầy đủ thông tin.", "false", "false", "OK", "warning", "");
            txt_pass1 = _pass1; ; txt_pass2 = _pass2;
        }
        else
        {
            //if (q.matkhau != encode_class.encode_md5(encode_class.encode_sha1(_oldpass)))
            //{
            //    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Mật khẩu hiện tại không chính xác.", "false", "false", "OK", "warning", "");
            //    txt_oldpass = _oldpass; txt_pass1 = _pass1; ; txt_pass2 = _pass2;
            //}
            //else
            //{
            if (_pass1 != _pass2)
            {
                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Mật khẩu mới không trùng nhau.", "false", "false", "OK", "warning", "");
                txt_pass1 = _pass1; ; txt_pass2 = _pass2;
            }
            else
            {
                string chinhanhId = (Session["chinhanh"] ?? "").ToString();
                taikhoan_table_2023 _ob = db.taikhoan_table_2023s
                    .FirstOrDefault(p => p.taikhoan == user && (string.IsNullOrWhiteSpace(chinhanhId) || p.id_chinhanh == chinhanhId));
                if (_ob == null)
                {
                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Không tìm thấy tài khoản cần đổi mật khẩu.", "false", "false", "OK", "alert", "");
                    return;
                }
                _ob.matkhau = encode_class.encode_md5(encode_class.encode_sha1(_pass1));
                db.SubmitChanges();
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Đổi mật khẩu thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + user);
            }
        }

    }
}
