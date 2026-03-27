using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_category_Default : System.Web.UI.Page
{
    dbDataContext db = new dbDataContext();

    #region phân trang
    public int stt = 1, current_page = 1, total_page = 1, show = 30;
    List<string> list_id_split;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login
        string _quyen = "q5_1";
        string _cookie_user = "", _cookie_pass = "";
        if (Request.Cookies["save_user_admin_aka_1"] != null) _cookie_user = Request.Cookies["save_user_admin_aka_1"].Value;
        if (Request.Cookies["save_pass_admin_aka_1"] != null) _cookie_pass = Request.Cookies["save_pass_admin_aka_1"].Value;
        if (Session["user"] == null) Session["user"] = ""; if (Session["notifi"] == null) Session["notifi"] = "";if (Session["user"].ToString() == "") Response.Redirect("/gianhang/admin/f5_ss_admin.aspx");
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
                        Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "warning", "");
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
        if (!IsPostBack)
        {
            if (Session["current_page_slide"] == null)//lưu giữ trang hiện tại
                Session["current_page_slide"] = "1";

            if (Session["search_slide"] != null)//lưu tìm kiếm
                txt_search.Text = Session["search_slide"].ToString();
            else
                Session["search_slide"] = txt_search.Text;

            if (Session["show_slide"] == null)//lưu số dòng mặc định
            {
                txt_show.Text = "30";
                Session["show_slide"] = txt_show.Text;
            }
            else
                txt_show.Text = Session["show_slide"].ToString();
        }
        main();
    }
    protected void but_luu_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < list_id_split.Count; i++)
        {
            string _id = list_id_split[i].Replace("check_", "");
            var q = db.web_module_slider_tables.Where(p => p.id.ToString() == _id);
            if (q.Count() != 0)
            {
                string _rank = Request.Form["rank_" + _id];
                int _r1 = 0;
                int.TryParse(_rank, out _r1);//nếu là số nguyên thì gán cho _r1
                if (_r1 > 0)//nếu số hợp lệ từ 1 trở lên thì lưu
                {
                    web_module_slider_table _ob = q.First();
                    _ob.rank = _r1;
                    db.SubmitChanges();
                }
            }
        }
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lưu thành công.", "4000", "warning"), true);
    }
    protected void but_del_Click(object sender, EventArgs e)
    {
        int _count = 0;
        for (int i = 0; i < list_id_split.Count; i++)
        {
            if (Request.Form[list_id_split[i]] == "on")
            {
                string _id = list_id_split[i].Replace("check_", "");
                var q = db.web_module_slider_tables.Where(p => p.id.ToString() == _id);
                if (q.Count() != 0)
                {
                    web_module_slider_table _ob = q.First();
                    file_folder_class.del_file(_ob.img);
                    db.web_module_slider_tables.DeleteOnSubmit(_ob);
                    db.SubmitChanges();
                    _count = _count + 1;
                }
            }
        }
        if (_count > 0)
        {
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa ảnh thành công.", "4000", "warning"), true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Session["current_page_slide"] = "1";
        Session["search_slide"] = txt_search.Text.Trim();
        Session["show_slide"] = txt_show.Text.Trim();
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        Session["current_page_slide"] = null;
        Session["search_slide"] = null;
        Session["show_slide"] = null;

        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-module/slide-anh/default.aspx");
    }

    public void main()
    {
        //lấy dữ liệu
        var list_all = db.web_module_slider_tables.ToList().Select(p => new
        {
            id = p.id,
            rank = p.rank,
            img = p.img,
            but_title = p.but_title,
            link = p.link
        });

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.id.ToString() == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }
        //xử lý trạng thái


        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 30;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_slide"].ToString());
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



    #region autopostback
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
        Session["current_page_slide"] = "1";

        main();
    }
    protected void txt_show_TextChanged(object sender, EventArgs e)
    {
        Session["current_page_slide"] = "1";
        main();
    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_slide"] = int.Parse(Session["current_page_slide"].ToString()) - 1;
        if (int.Parse(Session["current_page_slide"].ToString()) < 1)
            Session["current_page_slide"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_slide"] = int.Parse(Session["current_page_slide"].ToString()) + 1;
        if (int.Parse(Session["current_page_slide"].ToString()) > total_page)
            Session["current_page_slide"] = total_page;
        main();
    }
    #endregion

}