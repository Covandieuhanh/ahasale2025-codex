using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_Default2 : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    public string notifi, meta;
    protected void Page_Load(object sender, EventArgs e)
    {
        //Session["user"] = ""; Session["title"] = "";
        //if (Request.Cookies["save_user_admin_aka_1"] != null)
        //    Response.Cookies["save_user_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
        //if (Request.Cookies["save_pass_admin_aka_1"] != null)
        //    Response.Cookies["save_pass_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
        //if (Request.Cookies["save_url_admin_aka_1"] != null)
        //    Response.Cookies["save_url_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
       // Response.Write(encode_class.encode_md5(encode_class.encode_sha1("12345678")));

        if (Session["user"] == null) Session["user"] = "";
        if (Session["notifi"] == null) Session["notifi"] = "";
        if (Session["user_parent"] == null) Session["user_parent"] = "admin";

        if (Session["user"].ToString() != "")
            Response.Redirect("/gianhang/admin");

        if (!IsPostBack)
        {
            notifi = Session["notifi"].ToString();
            Session["notifi"] = "";

            var list_chinhanh = (from ob1 in db.chinhanh_tables.ToList()
                                 select new { id = ob1.id, ten = ob1.ten, }
                               );
            ddl_chinhanh.DataSource = list_chinhanh;
            ddl_chinhanh.DataTextField = "ten";
            ddl_chinhanh.DataValueField = "id";
            ddl_chinhanh.DataBind();
            ddl_chinhanh.Items.Insert(0, new ListItem("Chọn chi nhánh", ""));

            // Không bắt người dùng chọn chi nhánh khi đăng nhập.
            branch_wrap.Visible = false;
            branch_note.Visible = true;
        }
        #region meta
        var q = db.config_thongtin_tables;
        if (q.Count() != 0)
        {
            string _icon = "<link rel=\"shortcut icon\" type=\"image/x-icon\" href=\"" + q.First().icon + "\" />";
            string _appletouch = "<link rel=\"apple-touch-icon\" href=\"" + q.First().apple_touch_icon + "\" />";
            meta = _icon;
        }
        #endregion
    }


    protected void but_login_Click(object sender, EventArgs e)
    {
        string _user = txt_user.Text.Trim().ToLower();
        string _pass = txt_pass.Text;

        if (_user == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tài khoản.", "false", "false", "OK", "alert", ""), true);
        else
        {
            if (_pass == "")
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mật khẩu.", "false", "false", "OK", "alert", ""), true);
            else
            {
                string loginId = AccountAuth_cl.NormalizeLoginId(_user);
                string resolvedUser = loginId;
                AccountLoginInfo shopLoginInfo = AccountAuth_cl.FindAccountByLoginId(db, loginId, PortalScope_cl.ScopeShop);
                if (shopLoginInfo != null && shopLoginInfo.IsAmbiguous)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Email đang trùng nhiều tài khoản gian hàng đối tác. Vui lòng liên hệ admin.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (shopLoginInfo != null && !string.IsNullOrWhiteSpace(shopLoginInfo.TaiKhoan))
                    resolvedUser = shopLoginInfo.TaiKhoan;

                taikhoan_table_2023 adminAcc = tk_cl.return_object(resolvedUser);
                taikhoan_tb shopAcc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == resolvedUser);
                bool shopAdvanced = shopAcc != null && ShopLevel_cl.IsAdvancedEnabled(db, resolvedUser);
                if (shopAcc != null && !shopAdvanced)
                {
                    var latestReq = ShopLevel2Request_cl.GetLatestRequest(db, resolvedUser);
                    if (latestReq != null && latestReq.TrangThai == ShopLevel2Request_cl.StatusApproved)
                    {
                        taikhoan_tb approvedShop;
                        taikhoan_table_2023 ownerAdmin;
                        bool createdOwner;
                        ShopLevel_cl.EnableAdvancedForShop(db, resolvedUser, out approvedShop, out ownerAdmin, out createdOwner);
                        shopAdvanced = true;
                        if (adminAcc == null && ownerAdmin != null)
                            adminAcc = ownerAdmin;
                    }
                }

                if (adminAcc == null && shopAdvanced)
                {
                    adminAcc = AhaShineContext_cl.EnsureAdvancedAdminBootstrapForShop(db, resolvedUser);
                }

                if (adminAcc == null && shopAcc == null)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản không tồn tại.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                if (adminAcc == null && shopAcc != null && !shopAdvanced)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Shop này đang ở Level 1 nên chưa dùng được bộ công cụ /gianhang/admin.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                if (adminAcc == null && shopAcc != null && shopAdvanced)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không thể khởi tạo tài khoản quản trị cho shop này. Vui lòng thử lại hoặc liên hệ admin.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                string _pass_mahoa = encode_class.encode_md5(encode_class.encode_sha1(_pass));
                bool passOk = adminAcc != null && AccountAuth_cl.IsPasswordValid(_pass, adminAcc.matkhau);
                if (!passOk && shopAcc != null)
                    passOk = AccountAuth_cl.IsPasswordValid(_pass, shopAcc.matkhau);

                if (!passOk)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Mật khẩu không đúng.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                if (adminAcc != null && adminAcc.trangthai != "Đang hoạt động")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                string adminUserForScope = adminAcc != null ? adminAcc.taikhoan : (shopAcc != null ? shopAcc.taikhoan : "");
                string adminUserParent = adminAcc != null ? adminAcc.user_parent : "";
                string userParent = ShopLevel_cl.ResolveOwnerShopAccountForAdmin(db, adminUserForScope, adminUserParent);
                if (!ShopLevel_cl.CanUseAdvancedAdmin(db, adminUserForScope, userParent))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Shop này đang ở Level 1 nên chưa dùng được bộ công cụ /gianhang/admin.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                string id_chinhanh = ddl_chinhanh.SelectedValue ?? "";
                if (string.IsNullOrWhiteSpace(id_chinhanh))
                {
                    id_chinhanh = adminAcc != null ? (adminAcc.id_chinhanh ?? "") : "";
                    if (string.IsNullOrWhiteSpace(id_chinhanh))
                        id_chinhanh = AhaShineContext_cl.ResolveChiNhanhIdForShopAccount(db, userParent);
                }

                if (string.IsNullOrWhiteSpace(id_chinhanh))
                    id_chinhanh = "1";

                //lưu cookier với tên tài khoản để đăng nhập trong 1 năm
                app_cookie_policy_class.persist_cookie(
                    HttpContext.Current,
                    app_cookie_policy_class.admin_user_cookie,
                    encode_class.encrypt(resolvedUser),
                    365
                );

                //lưu cookier với pass để đăng nhập trong 1 năm
                string cookiePassValue = _pass_mahoa;
                if (adminAcc != null && !string.IsNullOrWhiteSpace(adminAcc.matkhau))
                    cookiePassValue = adminAcc.matkhau;

                app_cookie_policy_class.persist_cookie(
                    HttpContext.Current,
                    app_cookie_policy_class.admin_pass_cookie,
                    cookiePassValue,
                    365
                );

                Session["user"] = adminAcc != null ? adminAcc.taikhoan : (shopAcc != null ? shopAcc.taikhoan : resolvedUser);
                Session["chinhanh"] = id_chinhanh;
                Session["user_parent"] = string.IsNullOrWhiteSpace(userParent) ? "admin" : userParent;
                Session["nganh"] = adminAcc == null || string.IsNullOrWhiteSpace(adminAcc.id_nganh) ? ResolveNganhId(id_chinhanh) : adminAcc.id_nganh;
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng nhập thành công.", "2000", "warning");
                Response.Redirect("/gianhang/admin");
            }
        }
    }

    private string ResolveNganhId(string chinhanhId)
    {
        string id = (chinhanhId ?? "").Trim();
        if (id == "")
            return "";

        var nganh = db.nganh_tables.FirstOrDefault(p => p.id_chinhanh == id);
        return nganh == null ? "" : nganh.id.ToString();
    }
}
