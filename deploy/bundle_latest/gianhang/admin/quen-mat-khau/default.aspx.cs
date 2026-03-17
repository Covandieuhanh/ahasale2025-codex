using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_Default2 : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    random_class rd_cl = new random_class();
    public string notifi, meta;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"].ToString() != "")
        {
            if (Request.Cookies["save_url_admin_aka_1"] != null)
                Response.Redirect(Request.Cookies["save_url_admin_aka_1"].Value);
            else
                Response.Redirect("/gianhang/admin");
        }

        if (!IsPostBack)
        {
            notifi = Session["notifi"].ToString();
            Session["notifi"] = "";
        }
        #region meta
        var q = db.config_thongtin_tables;
        if (q.Count() != 0)
        {
            string _icon = "<link rel=\"shortcut icon\" type=\"image/x-icon\" href=\"" + q.First().icon + "\" />";
            string _appletouch = "<link rel=\"apple-touch-icon\" href=\"" + q.First().apple_touch_icon + "\" />";
            meta = _icon;
        }
        #endregion
    }

    public void send_code(string _user, string _emailnhan)
    {
        string _code = rd_cl.random_number(6);
        tk_cl.change_makhoiphuc(_user, _code);

        string _tennguoigui = "", _tieude_mail = "", _noidung_mail = "";
        _tennguoigui = HttpContext.Current.Request.Url.Host;
        _tieude_mail = "Khôi phục mật khẩu";
        _noidung_mail = _noidung_mail + "<b>Ai đó đã yêu cầu khôi phục mật khẩu cho tài khoản của bạn. Nếu không phải là bạn hãy bỏ qua email này.</b>";
        _noidung_mail = _noidung_mail + "<h3>THÔNG TIN TÀI KHOẢN</h3>";
        _noidung_mail = _noidung_mail + "<div>TÊN TÀI KHOẢN: <b>" + _user + "</b></div>";
        _noidung_mail = _noidung_mail + "<div>EMAIL: <b>" + _emailnhan + "</b></div>";
        _noidung_mail = _noidung_mail + "<div>MÃ KHÔI PHỤC: <b>" + _code + "</b></div>";
        _noidung_mail = _noidung_mail + "<div>Mã khôi phục chỉ được sử dụng <b>1 lần</b> và có thời hạn trong <b>5 phút</b>. Hãy điền mã này vào ô NHẬP MÃ KHÔI PHỤC trên website để tạo mật khẩu mới.</div>";
        _noidung_mail = _noidung_mail + "<div style='color:red'>Lưu ý: Đây là thư tự động được gửi từ hệ thống - Không phản hồi mail này, xin cám ơn.</div>";
        sendmail_class.sendmail("smtp.gmail.com", 587, _tieude_mail, _noidung_mail, _emailnhan, _tennguoigui, "");
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        //code = rd_cl.random_number(6);
        string _tk_email = txt_taikhoan_email.Text.Trim().ToLower();
        if (_tk_email == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tài khoản hoặc email.", "false", "false", "OK", "alert", ""), true);
        else
        {
            if (tk_cl.exist_user(_tk_email))//nếu nhập đúng tên tk
            {
                string _email = tk_cl.return_object(_tk_email).email;//thì lấy email của tk này
                if (_email == "")//nếu tk này chưa có email
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản này chưa cập nhật email.", "false", "false", "OK", "alert", ""), true);
                else//nếu có mail thì tiến hành gửi mã
                {
                    taikhoan_table_2023 _ob = tk_cl.return_object(_tk_email);
                    if (_ob.hsd_makhoiphuc.Value.AddMinutes(5) < DateTime.Now)  //đã quá 5 phút thì mới cho gửi lại
                        send_code(_tk_email, _email);   
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Một mã khôi phục đã được gửi về email của bạn.<br/>Vui lòng kiểm tra hộp thư đến (hoặc hộp thư rác).", "false", "false", "OK", "alert", "");
                    Response.Redirect("/gianhang/admin/quen-mat-khau/nhap-ma-khoi-phuc.aspx?user=" + _tk_email);
                }
            }
            else//nếu nhập sai tên tk thì kiểm tra xem đây có phải là email không?
            {
                if (!tk_cl.exist_email(_tk_email))//nếu nhập sai tên email
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản hoặc email không tồn tại.", "false", "false", "OK", "alert", ""), true);
                else//nếu có email thì gửi code về mail
                {
                    string _user = tk_cl.return_user(_tk_email);
                    taikhoan_table_2023 _ob = tk_cl.return_object(_user);
                    if (_ob.hsd_makhoiphuc.Value.AddMinutes(5) < DateTime.Now)  //đã quá 5 phút thì mới cho gửi lại
                        send_code(_user, _tk_email);
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Một mã khôi phục đã được gửi về email của bạn.<br/>Vui lòng kiểm tra hộp thư đến (hoặc hộp thư rác).", "false", "false", "OK", "alert", "");
                    Response.Redirect("/gianhang/admin/quen-mat-khau/nhap-ma-khoi-phuc.aspx?user=" + _user);
                }
            }
        }
    }
}