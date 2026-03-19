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
    giangvien_class gv_cl = new giangvien_class();
    hocvien_class hv_cl = new hocvien_class();
    datetime_class dt_cl = new datetime_class();
    nganh_class ng_cl = new nganh_class();
    public string user, user_parent;
    public Int64 tong_hocphi, tong_thanhtoan, tong_congno;
    #region phân trang
    public int stt = 1, current_page = 1, total_page = 1, show = 30;
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
        if (bcorn_class.check_quyen(user, "q14_1") == "" || bcorn_class.check_quyen(user, "n14_1") == "")
        {
            if (!IsPostBack)
            {
                var list_nganh = (from ob1 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                  select new { id = ob1.id, ten = ob1.ten, }
                                   );
                DropDownList5.DataSource = list_nganh;
                DropDownList5.DataTextField = "ten";
                DropDownList5.DataValueField = "id";
                DropDownList5.DataBind();
                DropDownList5.Items.Insert(0, new ListItem("Tất cả", ""));
                if (bcorn_class.check_quyen(user, "q14_1") == "")
                {
                }
                else
                {
                    DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList5.Enabled = false;
                }

                if (Session["current_page_hocvien"] == null)//lưu giữ trang hiện tại
                    Session["current_page_hocvien"] = "1";

                if (Session["index_sapxep_hocvien"] == null)////lưu sắp xếp
                {
                    DropDownList2.SelectedIndex = 0;
                    Session["index_sapxep_hocvien"] = DropDownList2.SelectedIndex.ToString();
                }
                else
                    DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_hocvien"].ToString());

                if (Session["search_hocvien"] != null)//lưu tìm kiếm
                    txt_search.Text = Session["search_hocvien"].ToString();
                else
                    Session["search_hocvien"] = txt_search.Text;

                if (Session["index_trangthai_hocvien"] != null)//lưu lọc trạng thái
                    DropDownList1.SelectedIndex = int.Parse(Session["index_trangthai_hocvien"].ToString());
                else
                    Session["index_trangthai_hocvien"] = DropDownList1.SelectedValue.ToString();

                if (Session["show_hocvien"] == null)//lưu số dòng mặc định
                {
                    txt_show.Text = "30";
                    Session["show_hocvien"] = txt_show.Text;
                }
                else
                    txt_show.Text = Session["show_hocvien"].ToString();

                if (Session["tungay_hocvien"] == null)
                {
                    txt_tungay.Text = "01/01/2023";
                    Session["tungay_hocvien"] = txt_tungay.Text;
                }
                else
                    txt_tungay.Text = Session["tungay_hocvien"].ToString();

                if (Session["denngay_hocvien"] == null)
                {
                    txt_denngay.Text = DateTime.Now.ToShortDateString();
                    Session["denngay_hocvien"] = txt_denngay.Text;
                }
                else
                    txt_denngay.Text = Session["denngay_hocvien"].ToString();


                if (Session["index_loc_thanhtoan_hocvien"] != null)//lưu lọc theo theo toán
                    ddl_locdulieu.SelectedIndex = int.Parse(Session["index_loc_thanhtoan_hocvien"].ToString());
                else
                    Session["index_loc_thanhtoan_hocvien"] = ddl_locdulieu.SelectedIndex.ToString();
                if (Session["index_loc_nganh_hocvien"] != null)//lưu lọc theo theo toán
                    DropDownList5.SelectedIndex = int.Parse(Session["index_loc_nganh_hocvien"].ToString());
                else
                    Session["index_loc_nganh_hocvien"] = DropDownList5.SelectedIndex.ToString();
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
    public string xuly_xeploai(string _xl)
    {
        switch (_xl)
        {
            case ("0"):return "Chưa học xong"; break;
            case ("A"): return "Xếp loại A (Giỏi)"; break;
            case ("B"): return "Xếp loại B (Đạt)"; break;
            case ("C"): return "Xếp loại C (Loại)"; break;
            default: return ""; break;
        }
    }
    public void main()
    {
        //Intersect: lấy ra các phần tử mà cả 2 bên đều có (phần chung)
        #region lấy dữ liệu
        var list_all = (from ob1 in db.hocvien_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                            join ob2 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on ob1.nganhhoc.ToString() equals ob2.id.ToString()
                        select new
                        {
                            id = ob1.id,
                            hoten = ob1.hoten,
                            hoten_khongdau = ob1.hoten_khongdau,
                            avt = ob1.anhdaidien,
                            ngaytao = ob1.ngaytao,
                            sdt = ob1.dienthoai,
                            email = ob1.email,
                            nguoitao = ob1.nguoitao,
                            ngaysinh = ob1.ngaysinh,
                            sobuoi_lt = ob1.sobuoi_lythuyet,
                            sobuoi_th = ob1.sobuoi_thuchanh,
                            sobuoi_tg = ob1.sobuoi_trogiang,
                            nganhhoc = ob2.ten,
                            goidaotao = ob1.goidaotao,
                            capbang = ob1.capbang,
                            ten_gv=ob1.tengiangvien_hientai,
                            xeploai=ob1.xeploai,
                            ngaycapbang=ob1.ngaycapbang,
                            sotien_dathanhtoan = ob1.sotien_dathanhtoan,
                            sotien_conlai = ob1.sotien_conlai,
                            hocphi=ob1.hocphi,
                            id_nganh=ob1.nganhhoc,
                        });

        //xử lý theo thời gian
        var list_time = list_all.Where(p => p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_hocvien"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_hocvien"].ToString()).Date);
        list_all = list_all.Intersect(list_time).ToList();

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.hoten.ToLower().Contains(_key) || p.hoten_khongdau.ToLower().Contains(_key) || p.sdt.ToString() == _key || p.email.ToString() == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }
        //xử lý trạng thái

        //lọc dữ liệu
        if (DropDownList1.SelectedValue.ToString() != "0")
        {
            switch (DropDownList1.SelectedValue.ToString())
            {
                case ("1"): var list_0 = list_all.Where(p => p.capbang == "Chưa cấp bằng").ToList(); list_all = list_all.Intersect(list_0).ToList(); break;
                case ("2"): var list_1 = list_all.Where(p => p.capbang == "Đã cấp bằng").ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
                default: var list_2 = list_all.ToList(); list_all = list_all.Intersect(list_2).ToList(); break;//tất cả
            }
        }

        //xử lý lọc dữ liệu
        if (ddl_locdulieu.SelectedValue.ToString() != "0")
        {
            switch (ddl_locdulieu.SelectedValue.ToString())
            {
                case ("1"): var list_1 = list_all.Where(p => p.sotien_conlai != 0).ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
                case ("2"): var list_2 = list_all.Where(p => p.sotien_conlai == 0).ToList(); list_all = list_all.Intersect(list_2).ToList(); break;
                default: break;
            }
        }
        if (DropDownList5.SelectedValue.ToString() != "")//ngành
        {
            var list_1 = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList(); list_all = list_all.Intersect(list_1).ToList();
        }

        tong_hocphi = list_all.Sum(p => p.hocphi).Value;
        tong_thanhtoan = list_all.Sum(p => p.sotien_dathanhtoan).Value;
        tong_congno = list_all.Sum(p => p.sotien_conlai).Value;

        //sắp xếp
        switch (Session["index_sapxep_hocvien"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.ngaytao).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
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
        current_page = int.Parse(Session["current_page_hocvien"].ToString());
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
        Session["search_hocvien"] = txt_search.Text.Trim();
        Session["current_page_hocvien"] = "1";
        main();
    }
    //protected void txt_show_TextChanged(object sender, EventArgs e)
    //{
    //    Session["current_page_hocvien"] = "1";
    //    main();
    //}
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_hocvien"] = int.Parse(Session["current_page_hocvien"].ToString()) - 1;
        if (int.Parse(Session["current_page_hocvien"].ToString()) < 1)
            Session["current_page_hocvien"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_hocvien"] = int.Parse(Session["current_page_hocvien"].ToString()) + 1;
        if (int.Parse(Session["current_page_hocvien"].ToString()) > total_page)
            Session["current_page_hocvien"] = total_page;
        main();
    }
    #endregion


    protected void Button1_Click(object sender, EventArgs e)
    {
        Session["index_trangthai_hocvien"] = DropDownList1.SelectedIndex;
        Session["index_sapxep_hocvien"] = DropDownList2.SelectedIndex;
        Session["current_page_hocvien"] = "1";
        Session["search_hocvien"] = txt_search.Text.Trim();
        Session["show_hocvien"] = txt_show.Text.Trim();
        Session["tungay_hocvien"] = txt_tungay.Text;
        Session["denngay_hocvien"] = txt_denngay.Text;

        Session["index_loc_thanhtoan_hocvien"] = ddl_locdulieu.SelectedIndex;
        Session["index_loc_nganh_hocvien"] = DropDownList5.SelectedIndex;
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        Session["index_trangthai_hocvien"] = null;
        Session["index_sapxep_hocvien"] = null;
        Session["current_page_hocvien"] = null;
        Session["search_hocvien"] = null;
        Session["show_hocvien"] = null;
        Session["tungay_hocvien"] = null;
        Session["denngay_hocvien"] = null;

        Session["index_loc_thanhtoan_hocvien"] = null;
        Session["index_loc_nganh_hocvien"] = null;
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-hoc-vien/Default.aspx");
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



    protected void but_xoa_Click(object sender, ImageClickEventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q14_4") == "" || bcorn_class.check_quyen(user, "n14_4") == "")
        {
            int _count = 0;
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    var q = db.hocvien_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (hv_cl.exist_id(_id))
                    {
                        hv_cl.del(_id);
                        _count = _count + 1;
                    }
                }
            }
            if (_count > 0)
            {
                main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công.", "4000", "warning"), true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }
}