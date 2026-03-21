using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_khoi_phuc_mat_khau : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (string.IsNullOrWhiteSpace(Request.QueryString["code"]))
            {
                Label2.Text = "Trang bạn yêu cầu không hợp lệ.";
                return;
            }
            string _code = Request.QueryString["code"].ToString().ToLower();

            using (dbDataContext db = new dbDataContext())
            {
                var q = db.taikhoan_tbs.FirstOrDefault(p => p.makhoiphuc == _code);
                if (q == null)
                {
                    Label2.Text = "Trang bạn yêu cầu không hợp lệ.";
                    return;
                }
                if (q.hsd_makhoiphuc.Value < AhaTime_cl.Now)
                {
                    Label2.Text = "Mã này đã hết hạn.";
                    return;
                }
                ViewState["taikhoan"] = q.taikhoan;

                PlaceHolder1.Visible = false;
                UpdatePanel1.Visible = true;
                Label1.Text = "Đặt lại mật khẩu cho <b>" + q.taikhoan + "</b><div>Thời gian hết hạn: <b>" + q.hsd_makhoiphuc.Value.ToString("dd/MM/yyyy HH:mm") + "'</b></div>";
            }

        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        string _pass = txt_pass.Text.Trim();
        if (_pass == "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập mật khẩu mới.", "2600", "danger"), true);
            return;
        }
        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
            if (q != null)
            {
                if (q.hsd_makhoiphuc.Value < AhaTime_cl.Now)
                {
                    Label2.Text = "Mã này đã hết hạn.";
                    PlaceHolder1.Visible = true;
                    UpdatePanel1.Visible = false;
                    return;
                }
                q.matkhau = _pass;
                q.hsd_makhoiphuc = q.hsd_makhoiphuc.Value.AddYears(-1);
                db.SubmitChanges();

                #region xóa cũ
                Session["taikhoan"] = "";
                Session["matkhau"] = "";
                if (Request.Cookies["cookie_userinfo_admin_bcorn"] != null)
                    Response.Cookies["cookie_userinfo_admin_bcorn"].Expires = AhaTime_cl.Now.AddDays(-1);
                #endregion

                #region lưu mới và tự động đăng nhập
                string _taikhoan_mahoa = mahoa_cl.mahoa_Bcorn(ViewState["taikhoan"].ToString());
                string _matkhau_mahoa = mahoa_cl.mahoa_Bcorn(_pass);
                HttpCookie _ck = new HttpCookie("cookie_userinfo_admin_bcorn");
                _ck["taikhoan"] = _taikhoan_mahoa;
                _ck["matkhau"] = _matkhau_mahoa;
                _ck.Expires = AhaTime_cl.Now.AddDays(7);
                // Đặt thuộc tính HttpOnly để ngăn chặn truy cập từ mã JavaScript
                _ck.HttpOnly = true;
                // Đặt thuộc tính Secure để chỉ cho phép truyền cookie qua kết nối an toàn
                _ck.Secure = true;
                //chỉ định tên miền mà cookie được áp dụng. Bằng cách này, cookie chỉ được gửi đến máy chủ từ tên miền đã chỉ định, các miền con sẽ đc áp dụng theo
                //_ck.Domain = "https://ahasale.vn";//bị ảnh hưởng khi ở localhost
                Response.Cookies.Add(_ck);

                //lưu session
                Session["taikhoan"] = _taikhoan_mahoa;
                Session["matkhau"] = _matkhau_mahoa;
                Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng nhập thành công.", "1000", "warning");

                string _url_back = Convert.ToString(Session["url_back"]);

                if (!string.IsNullOrEmpty(_url_back))
                {
                    Response.Redirect(_url_back, false);
                }
                else
                {
                    Response.Redirect("/admin/default.aspx", false);
                }

                Context.ApplicationInstance.CompleteRequest();
                #endregion
            }
        }
    }
}