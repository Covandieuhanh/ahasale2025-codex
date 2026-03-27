using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_Default : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();

    public string user, user_parent;
    #region phân trang
    public int stt = 1, current_page = 1, show = 50, total_page = 1;
    List<string> list_id_split;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login
        string _quyen = "q16_1";
        string _cookie_user = "", _cookie_pass = "";
        if (Request.Cookies["save_user_admin_aka_1"] != null) _cookie_user = Request.Cookies["save_user_admin_aka_1"].Value;
        if (Request.Cookies["save_pass_admin_aka_1"] != null) _cookie_pass = Request.Cookies["save_pass_admin_aka_1"].Value;
        if (Session["user"] == null) Session["user"] = ""; if (Session["notifi"] == null) Session["notifi"] = ""; if (Session["user"].ToString() == "") Response.Redirect("/gianhang/admin/f5_ss_admin.aspx");
        string _url = Request.Url.GetLeftPart(UriPartial.Authority).ToLower();
        string _kq = bcorn_class.check_login(Session["user"].ToString(), _cookie_user, _cookie_pass, _url, _quyen);
        if (_kq != "")//nếu có thông báo --> có lỗi --> reset --> bắt login lại
        {
            if (_kq == "baotri") Response.Redirect("/baotri.aspx");
            else
            {
                if (_kq == "1") Response.Redirect("/gianhang/admin/login.aspx");//hết Session, hết Cookie
                else
                {
                    if (_kq == "2")//k đủ quyền
                    {
                        Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
                        Response.Redirect("/gianhang/admin");
                    }
                    else
                    {
                        Session["notifi"] = _kq; Session["user"] = "";
                        Response.Cookies["save_user_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["save_pass_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["save_url_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Redirect("/gianhang/admin/login.aspx");
                    }
                }
            }
        }
        #endregion 
        user = Session["user"].ToString();
        user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();
        if (!IsPostBack)
        {
            Session["index_sapxep_quanlynganh"] = "0";
            Session["current_page_quanlynganh"] = "1";
        }
        main();
    }
    public void main()
    {
        //lấy dữ liệu
        var list_all = (from ob1 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                        select new
                        {
                            id = ob1.id,
                            ten = ob1.ten,
                        }).ToList();

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.ten.ToLower().Contains(_key)).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        //sắp xếp
        switch (Session["index_sapxep_quanlynganh"].ToString())
        {
            //case ("1"): list_all = list_all.OrderBy(p => p.ngaytao_tk).ToList(); break;
            default: list_all = list_all.OrderBy(p => p.ten).ToList(); break;
        }

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 50;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_quanlynganh"].ToString());
        if (current_page > total_page)
            current_page = total_page;
        if (current_page >= total_page)
            but_xemtiep.Enabled = false;
        else
            but_xemtiep.Enabled = true;
        if (current_page == 1)
            but_quaylai.Enabled = false;
        else
            but_quaylai.Enabled = true;

        //main
        stt = (show * current_page) - show + 1;
        var list_split = list_all.Skip(current_page * show - show).Take(show).ToList();
        list_id_split = new List<string>();
        foreach (var t in list_split)
        {
            list_id_split.Add("check_" + t.id);
        }
        int _s1 = stt + list_split.Count - 1;
        if (list_all.Count() != 0)
            lb_show.Text = "Hiển thị " + stt + "-" + _s1 + " trong số " + list_all.Count().ToString("#,##0") + " mục";
        else
            lb_show.Text = "Hiển thị 0-0 trong số 0";
        Repeater1.DataSource = list_split;
        Repeater1.DataBind();
    }
    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        ApplySearchState();
    }
    protected void but_search_Click(object sender, EventArgs e)
    {
        ApplySearchState();
    }
    private void ApplySearchState()
    {
        Session["search_quanlynganh"] = txt_search.Text.Trim();
        Session["current_page_quanlynganh"] = "1";
        main();
    }

    protected void txt_show_TextChanged(object sender, EventArgs e)
    {
        Session["current_page_quanlynganh"] = "1";

        main();
    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_quanlynganh"] = int.Parse(Session["current_page_quanlynganh"].ToString()) - 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_quanlynganh"] = int.Parse(Session["current_page_quanlynganh"].ToString()) + 1;

        main();
    }

    protected void but_form_themnhomthuchi_Click(object sender, EventArgs e)
    {
        string _tennhom = txt_tennhom.Text.Trim();

        if (_tennhom == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập tên ngành.", "4000", "warning"), true);
        else
        {
            //System.Web.HttpContext.Current.Session
            //InsertOnSubmit
            nganh_table _ob = new nganh_table();
            _ob.ten = _tennhom;
            _ob.id_chinhanh = Session["chinhanh"].ToString();
            db.nganh_tables.InsertOnSubmit(_ob);
            db.SubmitChanges();
            txt_tennhom.Text = "";
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm ngành thành công.", "4000", "warning"), true);
            //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thêm nhóm khách hàng thành công.", "2000", "warning");
            //Response.Redirect("/gianhang/admin/quan-ly-thu-chi/nhom-thu-chi.aspx");
        }
    }

    protected void but_save_Click(object sender, ImageClickEventArgs e)
    {
        for (int i = 0; i < list_id_split.Count; i++)
        {
            string _id = list_id_split[i].Replace("check_", "");
            string _tennhom = Request.Form["name_" + _id].Trim();

            var q = db.nganh_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
            if (q.Count() != 0)
            {
                nganh_table _ob = q.First();
                _ob.ten = _tennhom;
                db.SubmitChanges();
            }
        }
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lưu thành công.", "4000", "warning"), true);
        //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Lưu thành công.", "2000", "warning");
        //Response.Redirect("/gianhang/admin/quan-ly-thu-chi/nhom-thu-chi.aspx");
    }

    protected void but_xoa_Click(object sender, ImageClickEventArgs e)
    {
        for (int i = 0; i < list_id_split.Count; i++)
        {
            if (Request.Form[list_id_split[i]] == "on")
            {
                string _id = list_id_split[i].Replace("check_", "");
                var q = db.nganh_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                if (q.Count() != 0)
                {
                    nganh_table _ob = q.First();
                    db.nganh_tables.DeleteOnSubmit(_ob);
                    db.SubmitChanges();
                }
            }
        }
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "warning"), true);
    }

}
