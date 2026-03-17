using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class xoa_chitiet_giohang : System.Web.UI.Page
{
    DataTable giohang = new DataTable();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["giohang_webcon"] == null)
        {
            Response.Redirect("/");
        }
        else
        {
            giohang = Session["giohang_webcon"] as DataTable;
        }

        if (!String.IsNullOrWhiteSpace(Request.QueryString["id"]))
        {
            string _id = Request.QueryString["id"];
            //Response.Write(giohang.Rows.Count);
            foreach (DataRow row in giohang.Rows)
            {
                if (row["ID"].ToString() == _id)
                {
                    row.Delete(); break;
                }
            }
        }
        Session["notifi_home"] = "<script>function openNotify(){var notify = Metro.notify;notify.setup({timeout: 4000,});notify.create('Xóa thành công,', 'Thông báo', {cls: 'light'});}window.onload = function () {openNotify();};</script>";
        Response.Redirect("/gianhang/webcon/giohang.aspx?tkchinhanh=" + Session["ten_tk_chinhanh"].ToString());
    }
}
