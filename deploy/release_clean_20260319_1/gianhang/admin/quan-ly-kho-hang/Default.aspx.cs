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
    post_class po_cl = new post_class();
    datetime_class dt_cl = new datetime_class();
    public int tong_sl_nhap, tong_sl_xuat, tong_sl_ton;
    public Int64 tong_giatri_ton;

    #region phân trang
    public int stt = 1, current_page = 1, total_page = 1, show = 30;
    List<string> list_id_split;
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {

        #region Check_Login
        string _quyen = "q11_1";
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

        var q = db.web_post_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString());

        if (!IsPostBack)
        {

            if (Session["current_page_quanlykho"] == null)//lưu giữ trang hiện tại
                Session["current_page_quanlykho"] = "1";

            if (Session["index_sapxep_quanlykho"] == null)////lưu sắp xếp
            {
                DropDownList2.SelectedIndex = 1;//số lượng tồn giảm dần
                Session["index_sapxep_quanlykho"] = DropDownList2.SelectedIndex.ToString();
            }
            else
                DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_quanlykho"].ToString());

            if (Session["search_quanlykho"] != null)//lưu tìm kiếm
                txt_search.Text = Session["search_quanlykho"].ToString();
            else
                Session["search_quanlykho"] = txt_search.Text;
            if (Session["show_quanlykho"] == null)//lưu số dòng mặc định
            {
                txt_show.Text = "30";
                Session["show_quanlykho"] = txt_show.Text;
            }
            else
                txt_show.Text = Session["show_quanlykho"].ToString();

            if (Session["tungay_quanlykho"] == null)
            {
                txt_tungay.Text = q.Count() != 0 ? q.Min(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                Session["tungay_quanlykho"] = txt_tungay.Text;
            }
            else
                txt_tungay.Text = Session["tungay_quanlykho"].ToString();

            if (Session["denngay_quanlykho"] == null)
            {
                txt_denngay.Text =  DateTime.Now.ToShortDateString();
                Session["denngay_quanlykho"] = txt_denngay.Text;
            }
            else
                txt_denngay.Text = Session["denngay_quanlykho"].ToString();

            if (Session["index_trangthai_quanlykho"] != null)//lưu lọc hạn sử dụng
                DropDownList1.SelectedIndex = int.Parse(Session["index_trangthai_quanlykho"].ToString());
            else
                Session["index_trangthai_quanlykho"] = DropDownList1.SelectedIndex.ToString();

            if (Session["index_locton_quanlykho"] != null)//lưu lọc sl tồn
                DropDownList3.SelectedIndex = int.Parse(Session["index_locton_quanlykho"].ToString());
            else
            {
                DropDownList3.SelectedIndex = 1;
                Session["index_locton_quanlykho"] = DropDownList3.SelectedIndex.ToString();
            }

        }
        main();
    }

    public string check_hsd(string _hsd)
    {
        if (DateTime.Now.Date > DateTime.Parse(_hsd).Date)
            return "<span class='data-wrapper'><code class='bg-red fg-white'>Đã hết hạn</code></span>";
        else
            return "";
    }

    public void main()
    {
        //Intersect: lấy ra các phần tử mà cả 2 bên đều có (phần chung)
        #region lấy dữ liệu
        var list_all = (//from bv in db.web_post_tables.Where(p => p.id_category != "0" && p.phanloai == "ctsp").ToList()
                        //join mn in db.web_menu_tables.ToList() on bv.id_category equals mn.id.ToString()
                        from ob1 in db.donnhaphang_chitiet_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                        join ob2 in db.web_post_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on ob1.id_dvsp equals ob2.id.ToString()
                        select new
                        {
                            id = ob1.id,
                            ngaynhap = ob1.ngaytao,
                            name = ob1.ten_dvsp_taithoidiemnay,
                            name_en = ob2.name_en,
                            gianhap = ob1.gia_dvsp_taithoidiemnay,
                            image = ob2.image,
                            soluong = ob1.soluong,
                            soluong_ton_sanpham = ob1.sl_conlai,
                            soluong_daban = ob1.sl_daban,
                            hsd = ob1.hsd,
                            nsx = ob1.nsx,
                            solo = ob1.solo,
                            giatri_ton = ob1.gia_dvsp_taithoidiemnay * ob1.sl_conlai,
                            dvt=ob1.dvt,
                        }).ToList();
        

        //xử lý theo thời gian
        var list_time = list_all.Where(p => p.ngaynhap.Value.Date >= DateTime.Parse(Session["tungay_quanlykho"].ToString()).Date && p.ngaynhap.Value.Date <= DateTime.Parse(Session["denngay_quanlykho"].ToString()).Date);
        
        list_all = list_all.Intersect(list_time).ToList();
        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.name.ToLower().Contains(_key) || p.name_en.ToLower().Contains(_key) || p.solo == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }
        //xử lý trạng thái

        //LỌC DỮ LIỆU
        //if (DropDownList1.SelectedValue.ToString() != "0")
        //{
        switch (DropDownList1.SelectedValue.ToString())//hạn sử dụng
        {
            case ("0"): break;
            case ("1"): var list_1 = list_all.Where(p => p.hsd.Value.Date < DateTime.Now.Date).ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
            default: /*var list_2 = list_all.ToList(); list_all = list_all.Intersect(list_2).ToList();*/ break;
        }

        switch (DropDownList3.SelectedValue.ToString())//tồn kho
        {
            case ("0"): break;
            case ("1"): var list_1 = list_all.Where(p => p.soluong_ton_sanpham > 0).ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
            default: /*var list_2 = list_all.ToList(); list_all = list_all.Intersect(list_2).ToList();*/ break;
        }
        //}

        tong_sl_nhap = list_all.Sum(p => p.soluong).Value;
        tong_sl_xuat = list_all.Sum(p => p.soluong_daban).Value;
        tong_sl_ton = list_all.Sum(p => p.soluong_ton_sanpham).Value;
        tong_giatri_ton = list_all.Sum(p => p.giatri_ton).Value;

        switch (Session["index_sapxep_quanlykho"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.soluong_ton_sanpham).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.soluong_ton_sanpham).ToList(); break;
            case ("2"): list_all = list_all.OrderBy(p => p.hsd).ToList(); break;
            case ("3"): list_all = list_all.OrderByDescending(p => p.hsd).ToList(); break;
            case ("4"): list_all = list_all.OrderBy(p => p.ngaynhap).ToList(); break;
            case ("5"): list_all = list_all.OrderByDescending(p => p.ngaynhap).ToList(); break;
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
        current_page = int.Parse(Session["current_page_quanlykho"].ToString());
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
        Session["search_quanlykho"] = txt_search.Text.Trim();
        Session["current_page_quanlykho"] = "1";
        main();
    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_quanlykho"] = int.Parse(Session["current_page_quanlykho"].ToString()) - 1;
        if (int.Parse(Session["current_page_quanlykho"].ToString()) < 1)
            Session["current_page_quanlykho"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_quanlykho"] = int.Parse(Session["current_page_quanlykho"].ToString()) + 1;
        if (int.Parse(Session["current_page_quanlykho"].ToString()) > total_page)
            Session["current_page_quanlykho"] = total_page;
        main();
    }
    #endregion


    protected void Button1_Click(object sender, EventArgs e)
    {
        Session["index_trangthai_quanlykho"] = DropDownList1.SelectedIndex;
        Session["index_sapxep_quanlykho"] = DropDownList2.SelectedIndex;
        Session["current_page_quanlykho"] = "1";
        Session["search_quanlykho"] = txt_search.Text.Trim();
        Session["show_quanlykho"] = txt_show.Text.Trim();
        Session["tungay_quanlykho"] = txt_tungay.Text;
        Session["denngay_quanlykho"] = txt_denngay.Text;

        Session["index_locton_quanlykho"] = DropDownList3.SelectedIndex;

        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        Session["index_trangthai_quanlykho"] = null;
        Session["index_sapxep_quanlykho"] = null;
        Session["current_page_quanlykho"] = null;
        Session["search_quanlykho"] = null;
        Session["show_quanlykho"] = null;
        Session["tungay_quanlykho"] = null;
        Session["denngay_quanlykho"] = null;

        Session["index_locton_quanlykho"] = null;

        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-kho-hang/Default.aspx");
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