using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class f5_ss : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();

    private void RedirectToDashboard()
    {
        Response.Redirect(GianHangAdminBridge_cl.ResolvePreferredAdminRedirectUrl(HttpContext.Current, "", "/gianhang/admin"), false);
        HttpContext current = HttpContext.Current;
        if (current != null && current.ApplicationInstance != null)
            current.ApplicationInstance.CompleteRequest();
    }

    private void RedirectToLegacyLogin()
    {
        Response.Redirect(GianHangAdminBridge_cl.BuildLegacyAdminLoginUrl(HttpContext.Current, ""), false);
        HttpContext current = HttpContext.Current;
        if (current != null && current.ApplicationInstance != null)
            current.ApplicationInstance.CompleteRequest();
    }

    private bool TryBootstrapFromHomeWorkspace()
    {
        using (dbDataContext db = new dbDataContext())
        {
            string homeAccount;
            string deniedMessage;
            if (!GianHangAdminBridge_cl.EnsureLegacyAdminSessionFromCurrentHome(db, out homeAccount, out deniedMessage))
                return false;
        }

        RedirectToDashboard();
        return true;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] == null) Session["user"] = "";

        if ((Session["user"] + "").Trim() == "" && TryBootstrapFromHomeWorkspace())
            return;

        if (Session["user"].ToString() == "")
        {
            string _cookie_user = "";
            if (Request.Cookies[app_cookie_policy_class.admin_user_cookie] != null)
                _cookie_user = Request.Cookies[app_cookie_policy_class.admin_user_cookie].Value;
            if (_cookie_user != "")//nếu có lưu cc
            {
                string _user_mahoa = _cookie_user;//lấy giá trị user cookie đã đc mã hóa
                string _user = encode_class.decrypt(_user_mahoa);//giải mã ra sẽ được usernamer
                if (tk_cl.exist_user(_user))//nếu user này tồn tại
                {
                    taikhoan_table_2023 adminAcc = tk_cl.return_object(_user);
                    string userParent = "admin";
                    string chinhanhId = tk_cl.return_chinhanh(_user);
                    string nganhId = adminAcc == null ? "" : adminAcc.id_nganh;

                    using (dbDataContext db = new dbDataContext())
                    {
                        if (adminAcc != null)
                            userParent = ShopLevel_cl.ResolveOwnerShopAccountForAdmin(db, adminAcc.taikhoan, adminAcc.user_parent);

                        if (string.IsNullOrWhiteSpace(chinhanhId))
                            chinhanhId = AhaShineContext_cl.ResolveChiNhanhIdForShopAccount(db, userParent);

                        if (string.IsNullOrWhiteSpace(nganhId) && !string.IsNullOrWhiteSpace(chinhanhId))
                        {
                            var nganh = db.nganh_tables.FirstOrDefault(p => p.id_chinhanh == chinhanhId);
                            if (nganh != null)
                                nganhId = nganh.id.ToString();
                        }

                        if (!ShopLevel_cl.CanUseAdvancedAdmin(db, _user, userParent))
                        {
                            GianHangAdminBridge_cl.ClearLegacyAdminSession();
                            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Shop này đang ở Level 1 nên chưa dùng được bộ công cụ /gianhang/admin.", "2200", "warning");
                            RedirectToLegacyLogin();
                            return;
                        }
                    }

                    Session["user"] = _user;
                    Session["chinhanh"] = chinhanhId;
                    Session["nganh"] = nganhId;
                    Session["user_parent"] = userParent;
                    RedirectToDashboard();
                    return;
                }
            }

            GianHangAdminBridge_cl.ClearLegacyAdminSession();
            RedirectToLegacyLogin();
            return;
        }
        else
        {
            RedirectToDashboard();
            return;
        }
    }
}
