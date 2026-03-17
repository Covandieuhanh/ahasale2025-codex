using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class webcon_uc_dangnhap_webcon_uc : System.Web.UI.UserControl
{
    data_khachhang_class kh_cl = new data_khachhang_class();
    protected void Page_Load(object sender, EventArgs e)
    {
    
    }
    protected void but_login_Click(object sender, EventArgs e)
    {
        string _sdt = txt_sdt.Text.Trim().ToLower();
        string _pass = txt_pass.Text;
        if (_sdt == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập số điện thoại.", "false", "false", "OK", "alert", ""), true);
        else
        {
            if (_pass == "")
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mật khẩu.", "false", "false", "OK", "alert", ""), true);
            else
            {
                if (kh_cl.exist_sdt_cn(_sdt, AhaShineContext_cl.ResolveChiNhanhId()))
                {
                    bspa_data_khachhang_table _ob = kh_cl.return_object_sdt(_sdt, AhaShineContext_cl.ResolveChiNhanhId());
                    if (_ob.id_chinhanh == AhaShineContext_cl.ResolveChiNhanhId())//chi nhánh của tài khooản khách này
                    {
                        string _pass_mahoa = encode_class.encode_md5(encode_class.encode_sha1(_pass));
                        if (_ob.matkhau == _pass_mahoa)
                        {
                            //if (_ob.trangthai == "Đang hoạt động")
                            //{
                            //lưu cookier với tên tài khoản để đăng nhập trong 1 năm
                            app_cookie_policy_class.persist_cookie(
                                HttpContext.Current,
                                app_cookie_policy_class.shop_user_cookie,
                                encode_class.encrypt(_sdt),
                                365
                            );

                            //lưu cookier với pass để đăng nhập trong 1 năm
                            app_cookie_policy_class.persist_cookie(
                                HttpContext.Current,
                                app_cookie_policy_class.shop_pass_cookie,
                                _pass_mahoa,
                                365
                            );

                            Session["user_home_webcon"] = _sdt;
                            Session["notifi_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng nhập thành công.", "2000", "light");

                            if (Request.Cookies[app_cookie_policy_class.shop_return_url_cookie] != null &&
                                Request.Cookies[app_cookie_policy_class.shop_return_url_cookie].Value.Trim() != "")
                                Response.Redirect(Request.Cookies[app_cookie_policy_class.shop_return_url_cookie].Value);
                            else
                                Response.Redirect("/gianhang/webcon/Default.aspx?tkchinhanh=" + Session["ten_tk_chinhanh"].ToString());
                            //}
                            //else
                            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", ""), true);

                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Mật khẩu không đúng.", "false", "false", "OK", "alert", ""), true);
                    }
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Khách hàng không thuộc chi nhánh này.", "false", "false", "OK", "alert", ""), true);
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số điện thoại không tồn tại.", "false", "false", "OK", "alert", ""), true);
            }
        }
    }


}
