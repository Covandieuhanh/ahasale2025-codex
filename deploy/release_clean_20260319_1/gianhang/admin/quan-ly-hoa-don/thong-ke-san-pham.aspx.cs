using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_Default : System.Web.UI.Page
{
    dbDataContext db = new dbDataContext();
    datetime_class dt_cl = new datetime_class(); nganh_class ng_cl = new nganh_class();
    public string user, user_parent;
    public Int64 tongtien, tongsauck;
    public int tongsl;
    public Int64 tongthu = 0, tongchi = 0, loinhuan = 0;
    public string loinhuan_class = "fg-green";
    #region phân trang
    public int stt = 1, current_page = 1, show = 30, total_page = 1;
    List<string> list_id_split;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login  
        string _quyen = "none";
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
        #region Check quyen theo nganh
        user = Session["user"].ToString();
        user_parent = "admin";
        if (bcorn_class.check_quyen(user, "q7_10") == "" || bcorn_class.check_quyen(user, "n7_10") == "")
        {
            if (!IsPostBack)
            {
                var list_nganh = (from ob1 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                  select new { id = ob1.id, ten = ob1.ten, }
                                        );
                DropDownList5.DataSource = list_nganh;//lọc
                DropDownList5.DataTextField = "ten";
                DropDownList5.DataValueField = "id";
                DropDownList5.DataBind();
                DropDownList5.Items.Insert(0, new ListItem("Tất cả", ""));
                if (bcorn_class.check_quyen(user, "q7_10") == "")
                {
                }
                else
                {
                    DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList5.Enabled = false;
                }
                if (Session["index_loc_nganh_tkdv"] != null)//lưu lọc theo theo toán
                    DropDownList5.SelectedIndex = int.Parse(Session["index_loc_nganh_tkdv"].ToString());
                else
                    Session["index_loc_nganh_tkdv"] = DropDownList5.SelectedIndex.ToString();

                var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString());

                if (Session["current_page_tksp"] == null)//lưu giữ trang hiện tại
                    Session["current_page_tksp"] = "1";

                if (Session["index_sapxep_tksp"] == null)////lưu sắp xếp
                {
                    DropDownList2.SelectedIndex = 1;//mặc định là giảm dần theo thời gian
                    Session["index_sapxep_tksp"] = DropDownList2.SelectedIndex.ToString();
                }
                else
                    DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_tksp"].ToString());

                if (Session["search_tksp"] != null)//lưu tìm kiếm
                    txt_search.Text = Session["search_tksp"].ToString();
                else
                    Session["search_tksp"] = txt_search.Text;

                if (Session["show_tksp"] == null)//lưu số dòng mặc định
                {
                    txt_show.Text = "30";
                    Session["show_tksp"] = txt_show.Text;
                }
                else
                    txt_show.Text = Session["show_tksp"].ToString();

                if (Session["tungay_tksp"] == null)
                {
                    txt_tungay.Text = q.Count() != 0 ? q.Min(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                    Session["tungay_tksp"] = txt_tungay.Text;
                }
                else
                    txt_tungay.Text = Session["tungay_tksp"].ToString();

                if (Session["denngay_tksp"] == null)
                {
                    txt_denngay.Text = q.Count() != 0 ? q.Max(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                    Session["denngay_tksp"] = txt_denngay.Text;
                }
                else
                    txt_denngay.Text = Session["denngay_tksp"].ToString();


            }

            main();
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        #endregion

        
    }

    public void main()
    {
        var list_all = (from ob1 in db.web_post_tables.Where(p => p.phanloai == "ctsp" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                        join ob2 in db.bspa_hoadon_chitiet_tables.Where(p => p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString() && p.kyhieu == "dichvu" && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_tksp"].ToString()) && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_tksp"].ToString())).ToList() on ob1.id.ToString() equals ob2.id_dvsp
                        group ob2 by new { ob1.id, ob1.name, ob1.id_nganh } into g
                        select new
                        {
                            id = g.Key.id,
                            id_nganh = g.Key.id_nganh,
                            tensp = g.Key.name,
                            tongsl = g.Sum(ob2 => ob2.soluong).Value,
                            tongtien = g.Sum(ob2 => ob2.thanhtien).Value,
                            sauck = g.Sum(ob2 => ob2.tongsauchietkhau).Value,
                        });

        


        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.tensp.ToLower().Contains(_key)).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        if (DropDownList5.SelectedValue.ToString() != "")//ngành
        {
            var list_1 = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList(); list_all = list_all.Intersect(list_1).ToList();
        }

        tongsl = list_all.Sum(p => p.tongsl);
        tongtien = list_all.Sum(p => p.tongtien);
        tongsauck = list_all.Sum(p => p.sauck);

        DateTime _from = DateTime.Parse(Session["tungay_tksp"].ToString());
        DateTime _to = DateTime.Parse(Session["denngay_tksp"].ToString());
        var q_thuchi = db.bspa_thuchi_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString() && p.duyet_phieuchi == "Đã duyệt" && p.ngay != null && p.ngay.Value.Date >= _from.Date && p.ngay.Value.Date <= _to.Date);
        if (DropDownList5.SelectedValue.ToString() != "")
            q_thuchi = q_thuchi.Where(p => (p.id_nganh ?? "") == DropDownList5.SelectedValue.ToString());
        tongthu = q_thuchi.Where(p => p.thuchi == "Thu").Sum(p => (long?)p.sotien) ?? 0;
        tongchi = q_thuchi.Where(p => p.thuchi == "Chi").Sum(p => (long?)p.sotien) ?? 0;
        loinhuan = tongthu - tongchi;
        loinhuan_class = loinhuan >= 0 ? "fg-green" : "fg-red";

        //sắp xếp
        switch (Session["index_sapxep_tksp"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.tongsl).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.tongsl).ToList(); break;
            case ("2"): list_all = list_all.OrderBy(p => p.tongtien).ToList(); break;
            case ("3"): list_all = list_all.OrderByDescending(p => p.tongtien).ToList(); break;
            case ("4"): list_all = list_all.OrderBy(p => p.sauck).ToList(); break;
            case ("5"): list_all = list_all.OrderByDescending(p => p.sauck).ToList(); break;
            default: list_all = list_all.OrderByDescending(p => p.tongsl).ToList(); break;
        }

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 30;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_tksp"].ToString());
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
        Session["search_tksp"] = txt_search.Text.Trim();
        Session["current_page_tksp"] = "1";
        main();

    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_tksp"] = int.Parse(Session["current_page_tksp"].ToString()) - 1;
        if (int.Parse(Session["current_page_tksp"].ToString()) < 1)
            Session["current_page_tksp"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_tksp"] = int.Parse(Session["current_page_tksp"].ToString()) + 1;
        if (int.Parse(Session["current_page_tksp"].ToString()) > total_page)
            Session["current_page_tksp"] = total_page;
        main();
    }
    public void reset_ss()
    {
        Session["index_sapxep_tksp"] = null;
        Session["current_page_tksp"] = null;
        Session["search_tksp"] = null;
        Session["show_tksp"] = null;
        Session["tungay_tksp"] = null;
        Session["denngay_tksp"] = null;
        Session["index_loc_nganh_tksp"] = null;
    }
    protected void Button2_Click(object sender, EventArgs e)//reset
    {
        reset_ss();
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-hoa-don/thong-ke-dich-vu.aspx");
    }

    protected void Button1_Click(object sender, EventArgs e)//but lọc
    {
        Session["index_sapxep_tksp"] = DropDownList2.SelectedIndex;
        Session["current_page_tksp"] = "1";
        Session["search_tksp"] = txt_search.Text.Trim();
        Session["show_tksp"] = txt_show.Text.Trim();

        Session["tungay_tksp"] = txt_tungay.Text;
        Session["denngay_tksp"] = txt_denngay.Text;
        Session["index_loc_nganh_tksp"] = DropDownList5.SelectedIndex.ToString();

        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }


    #region chọn ngày nhanh
    protected void but_homqua_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = DateTime.Now.Date.AddDays(-1).ToString();
        txt_denngay.Text = DateTime.Now.Date.AddDays(-1).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_homnay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = DateTime.Now.Date.ToString();
        txt_denngay.Text = DateTime.Now.Date.ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_tuantruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydautuan().AddDays(-7).ToString();//lấy ngày đầu tuần
        txt_denngay.Text = dt_cl.return_ngaydautuan().AddDays(-1).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_tuannay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydautuan().ToString();//lấy ngày đầu tuần
        txt_denngay.Text = dt_cl.return_ngaycuoituan().ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_thangtruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauthangtruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoithangtruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_thangnay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauthang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoithang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_namtruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydaunamtruoc(DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoinamtruoc(DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_namnay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydaunam(DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoinam(DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_quytruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauquytruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoiquytruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_quynay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauquynay(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoiquynay(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }


    #endregion
}
