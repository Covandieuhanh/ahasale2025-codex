using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_quan_ly_website_quan_ly_menu_uc_treeview : System.Web.UI.UserControl
{
    menu_class mn_cl = new menu_class();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            show_tree_view();
    }
    #region show_treeview
    public void show_tree_view()
    {
        Label1.Text = "<ul style='font-size:15px' data-role='treeview'>";
        foreach (var t in mn_cl.return_list().Where(p => p.id_parent == "0" && p.bin==false && p.id_chinhanh == Session["chinhanh"].ToString()).ToList())
        {
            get_data_tree(t.id.ToString());
        }
        Label1.Text = Label1.Text + "</ul>";
    }
    public void get_data_tree(string _id_category)//đưa 1 id vào
    {
        if (mn_cl.exist_sub(_id_category)) //nếu có menucon
        {
            Label1.Text = Label1.Text + "<li  data-caption='" + mn_cl.return_object(_id_category).name + "'><ul>";  /*data-collapsed='true' nếu muốn đóng nhánh cây khi load*/
            foreach (var t in mn_cl.return_list().Where(p => p.id_parent == _id_category && p.bin == false))//thì duyệt hết con
            {
                get_data_tree(t.id.ToString()); //thì gọi lại hàm, nếu có id con thì cứ gọi lại
            }
            Label1.Text = Label1.Text + "</ul></li>";
        }
        else
        {
            Label1.Text = Label1.Text + "<li data-caption='" + mn_cl.return_object(_id_category).name + "'></li>";
        }
    }
    #endregion
}