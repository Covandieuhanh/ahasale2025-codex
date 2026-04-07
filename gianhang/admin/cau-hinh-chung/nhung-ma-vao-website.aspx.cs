using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_Default : System.Web.UI.Page
{
    dbDataContext db = new dbDataContext();
    datetime_class dt_cl = new datetime_class();
    public string notifi;
    private bool EnsureActionAccess(string requiredPermission)
    {
        return GianHangAdminPageGuard_cl.EnsureAccess(this, db, requiredPermission) != null;
    }
    private config_nhungma_table EnsureNhungMa()
    {
        config_nhungma_table ob = db.config_nhungma_tables.FirstOrDefault();
        if (ob != null)
            return ob;

        ob = new config_nhungma_table
        {
            nhungma_head = "",
            nhungma_body1 = "",
            nhungma_body2 = "",
            nhungma_fanpage = "",
            nhungma_googlemaps = ""
        };
        db.config_nhungma_tables.InsertOnSubmit(ob);
        db.SubmitChanges();
        return ob;
    }
    public void main()
    {

        config_nhungma_table _ob = EnsureNhungMa();
        if (!IsPostBack)
        {
            txt_code_head.Text = _ob.nhungma_head;
            txt_code_body1.Text = _ob.nhungma_body1;
            txt_code_body2.Text = _ob.nhungma_body2;
            txt_code_page.Text = _ob.nhungma_fanpage;
            txt_code_maps.Text = _ob.nhungma_googlemaps;
        }

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!EnsureActionAccess("q1_2"))
            return;

        main();
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        if (!EnsureActionAccess("q1_2"))
            return;
        else
        {
            string _code_head = txt_code_head.Text.Trim();
            string _code_body1 = txt_code_body1.Text.Trim();
            string _code_body2 = txt_code_body2.Text.Trim();
            string _code_page = txt_code_page.Text.Trim();
            string _code_map = txt_code_maps.Text.Trim();
            cauhinhchung_class.update_nhung_ma(_code_head, _code_body1, _code_body2, _code_page, _code_map);
            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
            Response.Redirect("/gianhang/admin/cau-hinh-chung/nhung-ma-vao-website.aspx");
        }
    }
}
