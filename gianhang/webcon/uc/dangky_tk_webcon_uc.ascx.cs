using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class webcon_uc_dangky_tk_webcon_uc : System.Web.UI.UserControl
{
    dbDataContext db = new dbDataContext();
    string_class str_cl = new string_class();
    random_class rd_cl = new random_class();
    data_khachhang_class kh_cl = new data_khachhang_class();
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void but_login_Click(object sender, EventArgs e)
    {
        string _pass = txt_matkhau.Text.Trim();
        string _fullname = str_cl.VietHoa_ChuCai_DauTien(str_cl.remove_blank(txt_hoten.Text.Trim().ToLower()));
        string _sdt = txt_dienthoai.Text.Trim().Replace(" ", "").Replace(".", "").Replace("-", "").Replace("+", "");

        if (_sdt == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập số điện thoại.", "false", "false", "OK", "alert", ""), true);
        else
        {
            if (kh_cl.exist_sdt_cnaka(_sdt))
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số điện thoại này đã được đăng ký.", "false", "false", "OK", "alert", ""), true);
            else
            {
                if (_pass == "")
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mật khẩu.", "false", "false", "OK", "alert", ""), true);
                else
                {
                    if (_fullname == "")
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập họ tên.", "false", "false", "OK", "alert", ""), true);
                    else
                    {

                        bspa_data_khachhang_table _ob1 = new bspa_data_khachhang_table();
                        _ob1.tenkhachhang = _fullname;
                        _ob1.diachi = "";
                        _ob1.magioithieu = "";
                        _ob1.ngaytao = DateTime.Now;
                        _ob1.nguoitao = "admin";
                        _ob1.nguoichamsoc = "";
                        _ob1.sdt = _sdt;
                        _ob1.user_parent = AhaShineContext_cl.UserParent;
                        _ob1.nhomkhachhang = "";
                        _ob1.matkhau = encode_class.encode_md5(encode_class.encode_sha1(_pass));
                        _ob1.anhdaidien = "/uploads/images/macdinh.jpg";

                        _ob1.ngaysinh = null;
                        _ob1.capbac = ""; _ob1.vnd_tu_e_aha = 0; _ob1.sodiem_e_aha = 0;

                        _ob1.id_chinhanh = AhaShineContext_cl.ResolveChiNhanhId();
                        _ob1.solan_lencap = 0;
                        db.bspa_data_khachhang_tables.InsertOnSubmit(_ob1);
                        db.SubmitChanges();

                        Session["user_home_webcon"] = _sdt;
                        Session["notifi_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng ký tài khoản thành công.", "2000", "light");
                        if (Request.Cookies[app_cookie_policy_class.shop_return_url_cookie] != null &&
                            Request.Cookies[app_cookie_policy_class.shop_return_url_cookie].Value.Trim() != "")
                            Response.Redirect(Request.Cookies[app_cookie_policy_class.shop_return_url_cookie].Value);
                        else
                            Response.Redirect("/gianhang/webcon/Default.aspx?tkchinhanh=" + Session["ten_tk_chinhanh"].ToString());
                    }
                }
            }
        }
    }
}
