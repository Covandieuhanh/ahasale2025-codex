using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_quan_ly_website_quan_ly_menu_uc_xemtruoc_dacap : System.Web.UI.UserControl
{
    menu_class mn_cl = new menu_class();
    public string kq = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            show_menu_dacap();
    }
    #region show_menu_dacap
    public void show_menu_dacap()
    {
        foreach (var t in mn_cl.return_list().Where(p => p.id_parent == "0" && p.bin == false && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.rank).ToList())
        {
            get_data(t.id.ToString());
        }        
    }
    public void get_data(string _id_category)//đưa 1 id vào
    {
        if (mn_cl.exist_sub(_id_category)) //nếu có menucon
        {
            kq = kq + "<li><a href='#' class='dropdown-toggle marker-light'>" + mn_cl.return_object(_id_category).name + "</a><ul class='d-menu place-right bg-indigo fg-white' data-role='dropdown'>"; 
            foreach (var t in mn_cl.return_list().Where(p => p.id_parent == _id_category && p.bin == false).OrderBy(p => p.rank))//thì duyệt hết con
            {
                get_data(t.id.ToString()); //thì gọi lại hàm, nếu có id con thì cứ gọi lại
            }
            kq = kq + "</ul></li>";
        }
        else
        {
            kq = kq + "<li><a href='#'>" + mn_cl.return_object(_id_category).name + "</a></li>";
        }
    }
    #endregion
}