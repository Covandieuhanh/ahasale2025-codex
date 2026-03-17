using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class tai_khoan_doi_mat_khau : System.Web.UI.Page
{
    public string id, sdt, notifi, user, user_parent, hoten, txt_pass1, txt_pass2;
    dbDataContext db = new dbDataContext();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user_home"] == null) Session["user_home"] = ""; if (Session["notifi_home"] == null) Session["notifi_home"] = "";
        user = Session["user_home"].ToString();
        user_parent = AhaShineContext_cl.UserParent;
        if (user == "")
            Response.Redirect(AhaShineHomeRoutes_cl.HomeUrl);

        var q_1 = db.bspa_data_khachhang_tables.Where(p => p.sdt.ToString() == Session["user_home"].ToString()).First();
        id = q_1.id.ToString();

        var q = db.bspa_data_khachhang_tables.Where(p => p.id.ToString() == id);
        if (q.Count() != 0)
        {
            bspa_data_khachhang_table _ob = db.bspa_data_khachhang_tables.Where(p => p.id.ToString() == id).First();
            hoten = _ob.tenkhachhang;
            sdt = _ob.sdt;
            
            if (!IsPostBack)
            {

            }
        }
        else
        {
            Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
            Response.Redirect(AhaShineHomeRoutes_cl.HomeUrl);
        }
    }

    protected void button1_Click(object sender, EventArgs e)
    {
        bspa_data_khachhang_table _ob = db.bspa_data_khachhang_tables.Where(p => p.id.ToString() == id).First();
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
                _ob.matkhau = encode_class.encode_md5(encode_class.encode_sha1(_pass1));
                db.SubmitChanges();
                Session["notifi_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Đổi mật khẩu thành công.", "4000", "warning");
                Response.Redirect(AhaShineHomeRoutes_cl.AccountUrl);
            }
        }

    }
}