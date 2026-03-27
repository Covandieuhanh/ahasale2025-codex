using System;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class daugia_admin_portal : Page
{
    protected string SellerAccount = "";
    protected string AdminAccount = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            DauGiaBootstrap_cl.EnsureSchemaSafe(db);
            SellerAccount = ReadHomeAccount();
            if (SellerAccount == "")
            {
                HomeSpaceAccess_cl.RedirectToHomeLogin(
                    this,
                    Request.RawUrl ?? "/daugia/admin/portal.aspx",
                    "Vui lòng đăng nhập tài khoản Home để truy cập không gian /daugia/admin.");
                return;
            }

            taikhoan_tb home = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == SellerAccount);
            if (home == null || !SpaceAccess_cl.CanAccessDauGia(db, home))
            {
                HomeSpaceAccess_cl.RedirectToAccessPage(this, ModuleSpace_cl.DauGia, Request.RawUrl ?? "/daugia/admin/portal.aspx");
                return;
            }

            AdminAccount = "";
            phSeller.Visible = true;
            phSystemAdmin.Visible = false;
        }
    }

    private string ReadHomeAccount()
    {
        string encrypted = Session["taikhoan_home"] as string;
        if (string.IsNullOrWhiteSpace(encrypted) && Request != null && Request.Cookies != null)
        {
            HttpCookie ck = Request.Cookies["cookie_userinfo_home_bcorn"];
            if (ck != null)
                encrypted = ck["taikhoan"] ?? "";
        }

        if (string.IsNullOrWhiteSpace(encrypted))
            return "";

        try
        {
            return (mahoa_cl.giaima_Bcorn(encrypted) ?? "").Trim().ToLowerInvariant();
        }
        catch
        {
            return "";
        }
    }
}
