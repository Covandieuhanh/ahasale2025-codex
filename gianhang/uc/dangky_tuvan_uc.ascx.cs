using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class uc_dangky_tuvan_uc : System.Web.UI.UserControl
{
    string_class str_cl = new string_class();
    dbDataContext db = new dbDataContext();
    taikhoan_class tk_cl = new taikhoan_class();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string currentUser = (Session["user_home"] ?? "").ToString();
            if (currentUser != "")
            {
                try
                {
                    var q = SqlTransientGuard_cl.Execute(
                        () => db.khachhang_table_2023s.Where(p => p.taikhoan == currentUser).ToList(),
                        3,
                        200);
                    if (q.Count != 0)
                    {
                        khachhang_table_2023 _ob = q[0];
                        txt_ten_dangky.Text = currentUser;
                        txt_ten_dangky.ReadOnly = true;

                        txt_sdt_dangky.Text = _ob.sdt;
                    }
                }
                catch (Exception ex)
                {
                    if (!SqlTransientGuard_cl.IsTransient(ex))
                        throw;
                }
            }
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        string _tenkh = str_cl.VietHoa_ChuCai_DauTien(str_cl.remove_blank(txt_ten_dangky.Text.Trim().ToLower()));
        string _sdtkh = txt_sdt_dangky.Text.Replace(" ", "").Replace(".", "").Replace("+", "").Replace("-", "");
        string _noidung1 = txt_noidung1_dangky.Text.Trim();
        if (_tenkh == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tên của bạn.", "false", "false", "OK", "alert", ""), true);
        else
        {
            if (_sdtkh == "")
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập số điện thoại của bạn.", "false", "false", "OK", "alert", ""), true);
            else
            {
                if (_noidung1 == "")
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập nội dung cần tư vấn.", "false", "false", "OK", "alert", ""), true);
                else
                {
                    try
                    {
                        SqlTransientGuard_cl.Execute(() =>
                        {
                            data_yeucau_tuvan_table _ob = new data_yeucau_tuvan_table();
                            _ob.ten = _tenkh;
                            _ob.sdt = _sdtkh;
                            _ob.noidung1 = _noidung1;
                            _ob.ngay = DateTime.Now;
                            db.data_yeucau_tuvan_tables.InsertOnSubmit(_ob);
                            db.SubmitChanges();

                            // thông báo trên app
                            thongbao_table _ob1 = new thongbao_table();
                            _ob1.id = Guid.NewGuid();
                            _ob1.daxem = false;//chưa xem
                            _ob1.nguoithongbao = "admin";
                            _ob1.nguoinhan = "admin";
                            _ob1.link = "/gianhang/admin/yeu-cau-tu-van/default.aspx";
                            _ob1.noidung = _tenkh + " vừa gửi yêu cầu tư vấn tại Website";
                            _ob1.thoigian = DateTime.Now;
                            db.thongbao_tables.InsertOnSubmit(_ob1);
                            db.SubmitChanges();
                        }, 3, 200);

                        //gửi mail
                        string _tennguoigui = "", _tieude_mail = "", _noidung_mail = "", _emailnhan = "";
                        _emailnhan = tk_cl.return_object("admin").email;
                        if (_emailnhan != "")
                        {
                            _tennguoigui = HttpContext.Current.Request.Url.Host;
                            _tieude_mail = _tenkh + " vừa gửi yêu cầu tư vấn tại " + HttpContext.Current.Request.Url.Host;
                            _noidung_mail = _noidung_mail + "<h3>YÊU CẦU TƯ VẤN</h3>";
                            _noidung_mail = _noidung_mail + "<div>HỌ TÊN: <b>" + _tenkh + "</b></div>";
                            _noidung_mail = _noidung_mail + "<div>SỐ ĐIỆN THOẠI: <b>" + _sdtkh + "</b></div>";
                            _noidung_mail = _noidung_mail + "<div><b>NỘI DUNG: </b>" + _noidung1 + "</div>";
                            _noidung_mail = _noidung_mail + "<div style='color:red'>Lưu ý: Đây là thư tự động được gửi từ hệ thống - Không phản hồi mail này, xin cám ơn.</div>";
                            sendmail_class.sendmail("smtp.gmail.com", 587, _tieude_mail, _noidung_mail, _emailnhan, _tennguoigui, "");
                        }

                        Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Gửi yêu cầu thành công.", "false", "false", "OK", "alert", "");
                        if (Request.Cookies[app_cookie_policy_class.home_return_url_cookie] != null)
                            Response.Redirect(AhaShineHomeRoutes_cl.NormalizeReturnUrl(Request.Cookies[app_cookie_policy_class.home_return_url_cookie].Value));
                        else
                            Response.Redirect(AhaShineHomeRoutes_cl.HomeUrl);
                    }
                    catch (Exception ex)
                    {
                        if (SqlTransientGuard_cl.IsTransient(ex))
                        {
                            ScriptManager.RegisterStartupScript(
                                this.Page,
                                this.GetType(),
                                Guid.NewGuid().ToString(),
                                thongbao_class.metro_dialog("Thông báo", "Hệ thống đang bận, vui lòng thử lại sau ít phút.", "false", "false", "OK", "alert", ""),
                                true);
                            return;
                        }

                        throw;
                    }
                }
            }
        }
    }
}
