using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class xoa_chitiet_giohang : System.Web.UI.Page
{
    DataTable giohang_nhaphang_admin = new DataTable();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!GianHangSystemAdminGuard_cl.EnsurePageAccess(this))
            return;

        if (Session["giohang_nhaphang_admin"] == null)
        {
            Response.Redirect("/gianhang/admin/quan-ly-kho-hang/nhap-hang.aspx");
        }
        else
        {
            giohang_nhaphang_admin = Session["giohang_nhaphang_admin"] as DataTable;
        }    
            
        if (!String.IsNullOrWhiteSpace(Request.QueryString["id"]))
        {
            string _id = Request.QueryString["id"];
            //Response.Write(giohang_nhaphang_admin.Rows.Count);
            foreach (DataRow row in giohang_nhaphang_admin.Rows)
            {
                if (row["ID"].ToString() == _id)
                {
                    row.Delete(); break;
                }
            }
        }
        Session["notifi"] = "<script>function openNotify(){var notify = Metro.notify;notify.setup({timeout: 4000,});notify.create('Xóa thành công,', 'Thông báo', {cls: 'warning'});}window.onload = function () {openNotify();};</script>";
        Response.Redirect("/gianhang/admin/quan-ly-kho-hang/nhap-hang.aspx");
    }
}
