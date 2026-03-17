using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class webcon_uc_menutop_webcon_uc : System.Web.UI.UserControl
{
    menu_homeaka_class mn_cl = new menu_homeaka_class();
    dbDataContext db = new dbDataContext();
    public string kq = "", logo, avt, hoten, sodiem_eaha;
    DataTable giohang = new DataTable();
    public int sl_hangtronggio = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user_home_webcon"] == null)
            Session["user_home_webcon"] = "";
        show_menu_dacap();
        giohang = Session["giohang_webcon"] as DataTable;
        if (Session["giohang_webcon"] != null)
        {
            //sl_hangtronggio = giohang.Rows.Count;
            foreach (DataRow row in giohang.Rows) // Duyệt từng dòng (DataRow) trong DataTable để đếm tổng số lượng mỹ phẩm
            {
                sl_hangtronggio = sl_hangtronggio + int.Parse(row["soluong"].ToString());
            }
        }


        var q = db.config_thongtin_tables;//lấy logo menutop
        if (q.Count() != 0)
            logo = q.First().logo1;

        if (Session["user_home_webcon"].ToString() != "")
        {
            bspa_data_khachhang_table _ob = db.bspa_data_khachhang_tables.Where(p => p.sdt == Session["user_home_webcon"].ToString()).First();
            avt = _ob.anhdaidien;
            hoten = _ob.tenkhachhang;
            sodiem_eaha = _ob.sodiem_e_aha.Value.ToString("#,##0.00");
        }
    }
    #region show_menu_dacap
    public void show_menu_dacap()
    {
        foreach (var t in mn_cl.return_list().Where(p => p.id.ToString() == "550" || p.id.ToString() == "551").OrderBy(p => p.rank).ToList())
        {
            get_data(t.id.ToString());
        }
    }
    public void get_data(string _id_category)//đưa 1 id vào
    {
        if (mn_cl.exist_sub(_id_category)) //nếu có menucon
        {
            kq = kq + "<li class=''><a href='#' class='dropdown-toggle marker-light'>" + mn_cl.return_object(_id_category).name + "</a><ul class='d-menu place-right' data-role='dropdown'>";
            foreach (var t in mn_cl.return_list().Where(p => p.id_parent == _id_category && p.bin == false).OrderBy(p => p.rank))//thì duyệt hết con
            {
                get_data(t.id.ToString()); //thì gọi lại hàm, nếu có id con thì cứ gọi lại
            }
            kq = kq + "</ul></li>";
        }
        else
        {
            if (mn_cl.return_object(_id_category).url_other != "")
                kq = kq + "<li class=''><a href='" + mn_cl.return_object(_id_category).url_other + "'>" + mn_cl.return_object(_id_category).name + "</a></li>";
            else
            {
                string phanloai = mn_cl.return_object(_id_category).phanloai;
                string url = "/gianhang/webcon/danh-sach-bai-viet.aspx?tkchinhanh=" + Session["ten_tk_chinhanh"].ToString() + "&idmn=" + _id_category;
                if (phanloai == "dsdv")
                    url = "/gianhang/webcon/danh-sach-dich-vu.aspx?tkchinhanh=" + Session["ten_tk_chinhanh"].ToString() + "&idmn=" + _id_category;
                else if (phanloai == "dssp")
                    url = "/gianhang/webcon/danh-sach-san-pham.aspx?tkchinhanh=" + Session["ten_tk_chinhanh"].ToString() + "&idmn=" + _id_category;

                kq = kq + "<li class=''><a href='" + url + "'>" + mn_cl.return_object(_id_category).name + "</a></li>";
            }
        }
    }
    #endregion
    protected void but_dangxuat_Click(object sender, EventArgs e)
    {
        string redirect_url = "/";
        if (Session["ten_tk_chinhanh"] != null && Session["ten_tk_chinhanh"].ToString() != "")
            redirect_url = "/gianhang/webcon/Default.aspx?tkchinhanh=" + Session["ten_tk_chinhanh"].ToString();

        Session["user_home_webcon"] = "";
        app_cookie_policy_class.expire_cookie(HttpContext.Current, app_cookie_policy_class.shop_user_cookie);
        app_cookie_policy_class.expire_cookie(HttpContext.Current, app_cookie_policy_class.shop_pass_cookie);
        app_cookie_policy_class.expire_cookie(HttpContext.Current, app_cookie_policy_class.shop_return_url_cookie);
        Session["notifi_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng xuất thành công.", "2000", "light");
        Response.Redirect(redirect_url);
    }
}
