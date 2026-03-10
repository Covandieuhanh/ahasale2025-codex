using System;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class shop_khoi_phuc_mat_khau : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string rawCode = (Request.QueryString["code"] ?? "").Trim();
            if (string.IsNullOrWhiteSpace(rawCode))
            {
                Label2.Text = "Trang bạn yêu cầu không hợp lệ.";
                return;
            }

            Guid parsed;
            if (!Guid.TryParse(rawCode, out parsed))
            {
                Label2.Text = "Trang bạn yêu cầu không hợp lệ.";
                return;
            }

            string code = rawCode.ToLowerInvariant();
            using (dbDataContext db = new dbDataContext())
            {
                var q = db.taikhoan_tbs.FirstOrDefault(p => p.makhoiphuc == code);
                if (q == null)
                {
                    Label2.Text = "Trang bạn yêu cầu không hợp lệ.";
                    return;
                }
                if (!PortalScope_cl.CanLoginShop(q.taikhoan, q.phanloai, q.permission) || q.block == true)
                {
                    Label2.Text = "Trang bạn yêu cầu không hợp lệ.";
                    return;
                }
                if (q.hsd_makhoiphuc == null || q.hsd_makhoiphuc.Value < AhaTime_cl.Now)
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
        string newPass = (txt_pass.Text ?? "").Trim();
        if (newPass == "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mật khẩu mới.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            string tk = Convert.ToString(ViewState["taikhoan"]);
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (q != null)
            {
                if (q.hsd_makhoiphuc == null || q.hsd_makhoiphuc.Value < AhaTime_cl.Now)
                {
                    Label2.Text = "Mã này đã hết hạn.";
                    PlaceHolder1.Visible = true;
                    UpdatePanel1.Visible = false;
                    return;
                }

                q.matkhau = newPass;
                string clearError;
                AccountResetSecurity_cl.ClearForceShopPassword(db, q.taikhoan, out clearError);
                q.makhoiphuc = null;
                q.hsd_makhoiphuc = AhaTime_cl.Now.AddMinutes(-1);
                db.SubmitChanges();

                #region clear old login
                Session["taikhoan_shop"] = "";
                Session["matkhau_shop"] = "";
                if (Request.Cookies["cookie_userinfo_shop_bcorn"] != null)
                    Response.Cookies["cookie_userinfo_shop_bcorn"].Expires = AhaTime_cl.Now.AddDays(-1);
                #endregion

                #region save new login and redirect
                string tkEnc = mahoa_cl.mahoa_Bcorn(q.taikhoan);
                string mkEnc = mahoa_cl.mahoa_Bcorn(newPass);

                HttpCookie ck = new HttpCookie("cookie_userinfo_shop_bcorn");
                ck["taikhoan"] = tkEnc;
                ck["matkhau"] = mkEnc;
                ck.Expires = AhaTime_cl.Now.AddDays(7);
                ck.HttpOnly = true;
                ck.Secure = AccountAuth_cl.ShouldUseSecureCookie(Request);
                Response.Cookies.Add(ck);

                Session["taikhoan_shop"] = tkEnc;
                Session["matkhau_shop"] = mkEnc;
                PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeShop);

                Session["thongbao_shop"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng nhập thành công.", "1000", "warning");
                Response.Redirect("/shop/default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                #endregion
            }
        }
    }
}
