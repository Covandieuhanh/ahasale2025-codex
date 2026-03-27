using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
                Label1.Text = "Đặt lại mã pin cho <b>" + q.taikhoan + "</b><div>Thời gian hết hạn: <b>" + q.hsd_makhoiphuc.Value.ToString("dd/MM/yyyy HH:mm") + "'</b></div>";
            }

        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        string _pin = txt_pin.Text.Trim();

        if (string.IsNullOrEmpty(_pin))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mã pin mới.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        if (!Regex.IsMatch(_pin, @"^\d{4}$"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                thongbao_class.metro_dialog("Thông báo", "Mã pin phải gồm đúng 4 chữ số.", "false", "false", "OK", "alert", ""), true);
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
                q.mapin_thanhtoan = PinSecurity_cl.HashPin(_pin);
                q.hsd_makhoiphuc = q.hsd_makhoiphuc.Value.AddYears(-1);
                db.SubmitChanges();

                string _url_back = Convert.ToString(Session["url_back_home"]);

                if (!string.IsNullOrEmpty(_url_back))
                {
                    Response.Redirect(_url_back, false);
                }
                else
                {
                    Response.Redirect("/home/default.aspx", false);
                }

                Context.ApplicationInstance.CompleteRequest();
            }
        }
    }
}
