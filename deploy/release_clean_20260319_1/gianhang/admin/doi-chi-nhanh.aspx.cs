using System;
using System.Web;
using System.Web.UI;

public partial class badmin_doi_chi_nhanh : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] == null) Session["user"] = "";
        string user = (Session["user"] ?? "").ToString().Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(user))
        {
            Response.Redirect("/gianhang/admin/login.aspx");
            return;
        }

        string branchId = (Request.QueryString["id"] ?? "").Trim();
        if (string.IsNullOrWhiteSpace(branchId))
        {
            Response.Redirect("/gianhang/admin");
            return;
        }

        string returnUrl = (Request.QueryString["return"] ?? "").Trim();
        if (string.IsNullOrWhiteSpace(returnUrl) || returnUrl.Contains("://"))
            returnUrl = "/gianhang/admin";

        dbDataContext db = new dbDataContext();
        string ownerShop = ShopLevel_cl.ResolveOwnerShopAccountForAdmin(db, user, AhaShineContext_cl.UserParent);
        bool isSuper = string.Equals(user, "admin", StringComparison.OrdinalIgnoreCase);
        bool isOwner = string.Equals(user, ownerShop, StringComparison.OrdinalIgnoreCase);

        if (!isSuper && !isOwner)
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Chỉ tài khoản chủ shop mới được đổi chi nhánh.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
            return;
        }

        if (!isSuper && !BranchAccess_cl.IsBranchOwnedBy(db, ownerShop, branchId))
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Chi nhánh không hợp lệ hoặc không thuộc quyền quản trị.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
            return;
        }

        Session["chinhanh"] = branchId;
        Session["id_chinhanh_webcon"] = branchId;

        Response.Redirect(returnUrl);
    }
}
