using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class f5_ss : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] == null) Session["user"] = "";

        if (Session["user"].ToString() == "")
        {
            string _cookie_user = "";
            if (Request.Cookies["save_user_admin_aka_1"] != null) _cookie_user = Request.Cookies["save_user_admin_aka_1"].Value;
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
                            Session["user"] = "";
                            Session["chinhanh"] = "";
                            Session["nganh"] = "";
                            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Shop này đang ở Level 1 nên chưa dùng được bộ công cụ /gianhang/admin.", "2200", "warning");
                            Response.Redirect("/gianhang/admin/login.aspx");
                            return;
                        }
                    }

                    Session["user"] = _user;
                    Session["chinhanh"] = chinhanhId;
                    Session["nganh"] = nganhId;
                    Session["user_parent"] = userParent;
                    Response.Redirect("/gianhang/admin");
                }
            }
            else
            {
                Session["user"] = "";
                Session["chinhanh"] = "";
                Session["nganh"] = "";
                if (Request.Cookies["save_user_admin_aka_1"] != null)
                    Response.Cookies["save_user_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                if (Request.Cookies["save_pass_admin_aka_1"] != null)
                    Response.Cookies["save_pass_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                if (Request.Cookies["save_url_admin_aka_1"] != null)
                    Response.Cookies["save_url_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                Response.Redirect("/gianhang/admin/login.aspx");
            }
        }
        else
        {
            Response.Redirect("/gianhang/admin");
        }
    }
}
