using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class gianhang_taikhoan_doi_mat_khau : System.Web.UI.Page
{
    public string notifi, user, url_back, current_user;
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    public string txt_oldpass = "", txt_pass1 = "", txt_pass2 = "";

    private bool HasAnyPermission(params string[] permissionKeys)
    {
        string actor = (current_user ?? "").Trim();
        if (string.IsNullOrEmpty(actor))
            return false;

        foreach (string permissionKey in permissionKeys)
        {
            if (!string.IsNullOrEmpty(permissionKey) && bcorn_class.check_quyen(actor, permissionKey) == "")
                return true;
        }

        return false;
    }

    private bool IsCurrentRootAdmin()
    {
        return string.Equals((current_user ?? "").Trim(), "admin", StringComparison.OrdinalIgnoreCase);
    }

    private void RedirectToAdminHome()
    {
        Response.Redirect(GianHangAdminBridge_cl.BuildAdminHomeUrl(HttpContext.Current));
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        #region Check quyen theo nganh
        string qsUser = (Request.QueryString["user"] ?? "").Trim();
        user = qsUser;
        current_user = (access.User ?? "").Trim();
        if (HasAnyPermission("q2_3", "n2_3") || string.Equals(user, current_user, StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(qsUser))
            {
                user = qsUser;
                if (tk_cl.exist_user(user))
                {
                    if (user == "admin" && !IsCurrentRootAdmin())
                    {
                        Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không thể đổi mật khẩu cho admin.", "false", "false", "OK", "alert", "");
                        RedirectToAdminHome();
                    }
                    else
                    {
                        url_back = GianHangAdminBridge_cl.ResolvePreferredAdminRedirectUrl(HttpContext.Current, "", "/gianhang/admin");
                    }
                }
                else
                {
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                    RedirectToAdminHome();
                }
            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                RedirectToAdminHome();
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            RedirectToAdminHome();
        }
        #endregion

        
    }


    protected void button1_Click1(object sender, EventArgs e)
    {
        if (!(HasAnyPermission("q2_3", "n2_3") || string.Equals(user, current_user, StringComparison.OrdinalIgnoreCase)))
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            RedirectToAdminHome();
            return;
        }

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
