using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_quan_ly_menu_Default : System.Web.UI.Page
{
    dbDataContext db = new dbDataContext();
    menu_class mn_cl = new menu_class();
    datetime_class dt_cl = new datetime_class();

    #region phân trang
    public int stt = 1, current_page = 1, total_page = 1, show = 30;
    List<string> list_id_split;
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {

        #region Check_Login
        string _quyen = "q3_1";
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

        var q = db.web_menu_tables;

        if (!IsPostBack)
        {
            if (Session["current_page_menu"] == null)//lưu giữ trang hiện tại
                Session["current_page_menu"] = "1";

            if (Session["index_sapxep_menu"] == null)////lưu sắp xếp
            {
                DropDownList2.SelectedIndex = 9;
                Session["index_sapxep_menu"] = DropDownList2.SelectedIndex.ToString();
            }
            else
                DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_menu"].ToString());

            if (Session["search_menu"] != null)//lưu tìm kiếm
                txt_search.Text = Session["search_menu"].ToString();
            else
                Session["search_menu"] = txt_search.Text;

            if (Session["index_trangthai_menu"] != null)//lưu lọc trạng thái
                DropDownList1.SelectedIndex = int.Parse(Session["index_trangthai_menu"].ToString());
            else
                Session["index_trangthai_menu"] = DropDownList1.SelectedValue.ToString();

            if (Session["show_menu"] == null)//lưu số dòng mặc định
            {
                txt_show.Text = "30";
                Session["show_menu"] = txt_show.Text;
            }
            else
                txt_show.Text = Session["show_menu"].ToString();

            if (Session["tungay_menu"] == null)
            {
                txt_tungay.Text = q.Count() != 0 ? q.Min(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                Session["tungay_menu"] = txt_tungay.Text;
            }
            else
                txt_tungay.Text = Session["tungay_menu"].ToString();

            if (Session["denngay_menu"] == null)
            {
                txt_denngay.Text = q.Count() != 0 ? q.Max(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                Session["denngay_menu"] = txt_denngay.Text;
            }
            else
                txt_denngay.Text = Session["denngay_menu"].ToString();
        }
        main();
    }

    public void main()
    {
        //Intersect: lấy ra các phần tử mà cả 2 bên đều có (phần chung)
        #region lấy dữ liệu
        var list_all = (from ob1 in db.web_menu_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                            //join ob2 in db.bspa_hoadon_chitiet_tables.ToList() on ob1.id.ToString() equals ob2.id_hoadon
                        select new
                        {
                            id = ob1.id,
                            name = ob1.name,
                            menucha = mn_cl.return_ten_menu_cha(ob1.id.ToString()),
                            capbac = ob1.id_level,
                            rank = ob1.rank,
                            sl_menu_con = mn_cl.return_soluong_menucon(ob1.id.ToString()),
                            ngaytao = ob1.ngaytao,
                            bin = ob1.bin,
                        });

        //xử lý theo thời gian
        var list_time = list_all.Where(p => p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_menu"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_menu"].ToString()).Date);
        list_all = list_all.Intersect(list_time).ToList();

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.name.ToLower().Contains(_key) || p.id.ToString() == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }
        //xử lý trạng thái

        //lọc dữ liệu
        //if (DropDownList1.SelectedValue.ToString() != "0")
        //{
        switch (DropDownList1.SelectedValue.ToString())
        {
            case ("0"): var list_0 = list_all.Where(p => p.bin == false).ToList(); list_all = list_all.Intersect(list_0).ToList(); break;
            case ("1"): var list_1 = list_all.Where(p => p.bin == true).ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
            default: /*var list_2 = list_all.ToList(); list_all = list_all.Intersect(list_2).ToList();*/ break;
        }
        //}

        //sắp xếp
        switch (Session["index_sapxep_menu"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.name).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.name).ToList(); break;
            case ("2"): list_all = list_all.OrderBy(p => p.menucha).ToList(); break;
            case ("3"): list_all = list_all.OrderByDescending(p => p.menucha).ToList(); break;
            case ("4"): list_all = list_all.OrderBy(p => p.sl_menu_con).ToList(); break;
            case ("5"): list_all = list_all.OrderByDescending(p => p.sl_menu_con).ToList(); break;
            case ("6"): list_all = list_all.OrderBy(p => p.rank).ToList(); break;
            case ("7"): list_all = list_all.OrderByDescending(p => p.rank).ToList(); break;
            case ("8"): list_all = list_all.OrderBy(p => p.ngaytao).ToList(); break;
            case ("9"): list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
            case ("10"): list_all = list_all.OrderBy(p => p.capbac).ToList(); break;
            case ("11"): list_all = list_all.OrderByDescending(p => p.capbac).ToList(); break;
            default: break;
        }
        #endregion

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 30;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_menu"].ToString());
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

    #region button click
    protected void but_luu_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q3_3") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {
            for (int i = 0; i < list_id_split.Count; i++)
            {
                string _id = list_id_split[i].Replace("check_", "");
                var q = db.web_menu_tables.Where(p => p.id.ToString() == _id);
                if (mn_cl.exist_id(_id))
                {
                    web_menu_table _ob = q.First();
                    string _rank = Request.Form["rank_" + _id];
                    int _r1 = 0;
                    int.TryParse(_rank, out _r1);//nếu là số nguyên thì gán cho _r1
                    if (_r1 > 0)//nếu số hợp lệ từ 1 trở lên thì lưu
                    {
                        //mn_cl.update_rank(_id, _r1);
                        var q1 = db.web_menu_tables.Where(p => p.id.ToString() == _id);
                        web_menu_table _ob1 = q1.First();
                        _ob.rank = _r1;
                        db.SubmitChanges();
                    }
                    db.SubmitChanges();
                }
            }
            //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Lưu thành công.", "4000", "warning");
            //Response.Redirect("/gianhang/admin/quan-ly-menu/Default.aspx");
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lưu thành công.", "4000", "warning"), true);
        }
    }
    protected void Button3_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q3_4") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {
            int _count = 0;
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    if (mn_cl.exist_id(_id))
                    {
                        mn_cl.remove_bin(_id, true);
                        _count = _count + 1;
                        //var q = db.web_menu_tables.Where(p=>p.id.ToString()== _id);
                        //web_menu_table _ob = q.First();
                        //_ob.bin = true;
                        //db.SubmitChanges();
                    }
                }
            }
            if (_count > 0)
            {
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xóa thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-menu/Default.aspx");
                //main();
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công.", "4000", "warning"), true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
            }
        }
    }
    protected void but_xoavinhvien_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q3_5") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {
            int _count = 0;
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    if (mn_cl.exist_id(_id))
                    {
                        mn_cl.del(_id);
                        _count = _count + 1;
                    }
                }
            }
            if (_count > 0)
            {
                main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa vĩnh viễn thành công.", "4000", "warning"), true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
            }
        }
    }
    protected void but_khoiphuc_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q3_6") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {
            int _count = 0;
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    if (mn_cl.exist_id(_id))
                    {
                        mn_cl.remove_bin(_id, false);
                        _count = _count + 1;
                        //var q = db.web_menu_tables.Where(p => p.id.ToString() == _id);
                        //web_menu_table _ob = q.First();
                        //_ob.bin = false;
                        //db.SubmitChanges();
                    }
                }
            }
            if (_count > 0)
            {
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Khôi phục thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-menu/Default.aspx");
                //main();
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Khôi phục thành công.", "4000", "warning"), true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
            }
        }
    }
    #endregion

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
        Session["current_page_menu"] = "1";

        main();
    }
    //protected void txt_show_TextChanged(object sender, EventArgs e)
    //{
    //    Session["current_page_menu"] = "1";
    //    main();
    //}
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_menu"] = int.Parse(Session["current_page_menu"].ToString()) - 1;
        if (int.Parse(Session["current_page_menu"].ToString()) < 1)
            Session["current_page_menu"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_menu"] = int.Parse(Session["current_page_menu"].ToString()) + 1;
        if (int.Parse(Session["current_page_menu"].ToString()) > total_page)
            Session["current_page_menu"] = total_page;
        main();
    }
    #endregion


    protected void Button1_Click(object sender, EventArgs e)
    {
        Session["index_trangthai_menu"] = DropDownList1.SelectedIndex;
        Session["index_sapxep_menu"] = DropDownList2.SelectedIndex;
        Session["current_page_menu"] = "1";
        Session["search_menu"] = txt_search.Text.Trim();
        Session["show_menu"] = txt_show.Text.Trim();
        Session["tungay_menu"] = txt_tungay.Text;
        Session["denngay_menu"] = txt_denngay.Text;
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        Session["index_trangthai_menu"] = null;
        Session["index_sapxep_menu"] = null;
        Session["current_page_menu"] = null;
        Session["search_menu"] = null;
        Session["show_menu"] = null;
        Session["tungay_menu"] = null;
        Session["denngay_menu"] = null;
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-menu/Default.aspx");
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

    protected void Button4_Click(object sender, EventArgs e)
    {
        #region lấy dữ liệu
        var list_all = (from ob1 in db.web_menu_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                            //join ob2 in db.bspa_hoadon_chitiet_tables.ToList() on ob1.id.ToString() equals ob2.id_hoadon
                        select new
                        {
                            id = ob1.id,
                            name = ob1.name,
                            menucha = mn_cl.return_ten_menu_cha(ob1.id.ToString()),
                            capbac = ob1.id_level,
                            rank = ob1.rank,
                            sl_menu_con = mn_cl.return_soluong_menucon(ob1.id.ToString()),
                            ngaytao = ob1.ngaytao,
                            bin = ob1.bin,
                        });

        //xử lý theo thời gian
        var list_time = list_all.Where(p => p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_menu"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_menu"].ToString()).Date);
        list_all = list_all.Intersect(list_time).ToList();

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.name.ToLower().Contains(_key) || p.id.ToString() == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }
        //xử lý trạng thái

        //lọc dữ liệu
        //if (DropDownList1.SelectedValue.ToString() != "0")
        //{
        switch (DropDownList1.SelectedValue.ToString())
        {
            case ("0"): var list_0 = list_all.Where(p => p.bin == false).ToList(); list_all = list_all.Intersect(list_0).ToList(); break;
            case ("1"): var list_1 = list_all.Where(p => p.bin == true).ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
            default: /*var list_2 = list_all.ToList(); list_all = list_all.Intersect(list_2).ToList();*/ break;
        }
        //}

        //sắp xếp
        switch (Session["index_sapxep_menu"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.name).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.name).ToList(); break;
            case ("2"): list_all = list_all.OrderBy(p => p.menucha).ToList(); break;
            case ("3"): list_all = list_all.OrderByDescending(p => p.menucha).ToList(); break;
            case ("4"): list_all = list_all.OrderBy(p => p.sl_menu_con).ToList(); break;
            case ("5"): list_all = list_all.OrderByDescending(p => p.sl_menu_con).ToList(); break;
            case ("6"): list_all = list_all.OrderBy(p => p.rank).ToList(); break;
            case ("7"): list_all = list_all.OrderByDescending(p => p.rank).ToList(); break;
            case ("8"): list_all = list_all.OrderBy(p => p.ngaytao).ToList(); break;
            case ("9"): list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
            case ("10"): list_all = list_all.OrderBy(p => p.capbac).ToList(); break;
            case ("11"): list_all = list_all.OrderByDescending(p => p.capbac).ToList(); break;
            default: break;
        }
        #endregion
        if (check_list_excel.Items.Count == 0)
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
        else
        {
            // khởi tạo wb rỗng
            XSSFWorkbook wb = new XSSFWorkbook();

            // Tạo ra 1 sheet
            ISheet sheet = wb.CreateSheet();

            // Bắt đầu ghi lên sheet

            // Tạo row
            var row0 = sheet.CreateRow(0);
            // Merge lại row đầu 3 cột
            row0.CreateCell(0); // tạo ra cell trc khi merge
            CellRangeAddress cellMerge = new CellRangeAddress(0, 0, 0, 2);
            sheet.AddMergedRegion(cellMerge);
            row0.GetCell(0).SetCellValue("Dữ liệu của bạn xuất ngày " + DateTime.Now);

            // Ghi tiêu đề cột ở row 1
            var row1 = sheet.CreateRow(1);

            //đếm xem có bao nhiêu cột đc chọn
            int _socot = 0;
            for (int i = 0; i < check_list_excel.Items.Count; i++)//duyệt hết các phần tử trong list check
            {
                if (check_list_excel.Items[i].Selected)//nếu cột này đc chọn
                {
                    //thì tạo cột tiêu đề
                    row1.CreateCell(_socot).SetCellValue(check_list_excel.Items[i].Text);
                    _socot = _socot + 1;
                }
            }

            // bắt đầu duyệt mảng và ghi tiếp tục
            int rowIndex = 2;
            foreach (var item in list_all)
            {
                // tao row mới
                var newRow = sheet.CreateRow(rowIndex);

                // set giá trị
                int _socot1 = 0;
                for (int i = 0; i < check_list_excel.Items.Count; i++)//duyệt hết các phần tử trong list check
                {
                    if (check_list_excel.Items[i].Selected)//nếu cột này đc chọn
                    {
                        string _tencot = check_list_excel.Items[i].Value;
                        switch (_tencot)
                        {
                            //case "ngaysinh":
                            //    if (item.ngaysinh != null)
                            //    {
                            //        newRow.CreateCell(_socot1).SetCellValue(item.ngaysinh.Value.ToString("dd/MM/yyyy"));
                            //        break;
                            //    }
                            //    else
                            //    {
                            //        newRow.CreateCell(_socot1).SetCellValue("");
                            //        break;
                            //    }
                            case "name": newRow.CreateCell(_socot1).SetCellValue(item.name); break;
                            default: break;
                        }
                        _socot1 = _socot1 + 1;
                    }
                }

                // tăng index
                rowIndex++;
            }

            // xong hết thì save file lại
            string _filename = Guid.NewGuid().ToString();
            FileStream fs = new FileStream(Server.MapPath("~/uploads/Files/" + _filename + ".xlsx"), FileMode.CreateNew);
            wb.Write(fs);
            Response.Redirect("/uploads/Files/" + _filename + ".xlsx");
        }
    }
}