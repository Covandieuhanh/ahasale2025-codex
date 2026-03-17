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
    nhapvattu_class nvt_cl = new nhapvattu_class();
    datetime_class dt_cl = new datetime_class();
    string_class str_cl = new string_class();
    public string user, user_parent, show_add = "none";
    public Int64 doanhso_hoadon_sauck = 0, doanhso_dichvu = 0, doanhso_sanpham, doanhso_hoadon = 0, tong_congno = 0, tongtien_dathanhtoan = 0,
        dichvu_sauck = 0, sanpham_sauck = 0, sanpham_soluong = 0, dichvu_soluong = 0, hoadon_sl = 0,
        /*tong_tienmat = 0, tong_chuyenkhoan = 0,tong_quetthe = 0,*/ tongrieng_dichvu = 0, tongrieng_sanpham = 0;
    #region phân trang
    public int stt = 1, current_page = 1, show = 30, total_page = 1;
    List<string> list_id_split;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
  
        #region Check_Login  
        string _quyen = "q13_4";
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
        user_parent = "admin";
        if (!IsPostBack)
        {
            var q = db.donnhap_vattu_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString());

            if (Session["current_page_donnhapvattu"] == null)//lưu giữ trang hiện tại
                Session["current_page_donnhapvattu"] = "1";

            if (Session["index_sapxep_donnhapvattu"] == null)////lưu sắp xếp
            {
                DropDownList2.SelectedIndex = 1;//mặc định là giảm dần theo thời gian
                Session["index_sapxep_donnhapvattu"] = DropDownList2.SelectedIndex.ToString();
            }
            else
                DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_donnhapvattu"].ToString());

            if (Session["search_donnhapvattu"] != null)//lưu tìm kiếm
                txt_search.Text = Session["search_donnhapvattu"].ToString();
            else
                Session["search_donnhapvattu"] = txt_search.Text;

            if (Session["show_donnhapvattu"] == null)//lưu số dòng mặc định
            {
                txt_show.Text = "30";
                Session["show_donnhapvattu"] = txt_show.Text;
            }
            else
                txt_show.Text = Session["show_donnhapvattu"].ToString();

            if (Session["tungay_donnhapvattu"] == null)
            {
                txt_tungay.Text = q.Count() != 0 ? q.Min(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                Session["tungay_donnhapvattu"] = txt_tungay.Text;
            }
            else
                txt_tungay.Text = Session["tungay_donnhapvattu"].ToString();

            if (Session["denngay_donnhapvattu"] == null)
            {
                txt_denngay.Text = q.Count() != 0 ? q.Max(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                Session["denngay_donnhapvattu"] = txt_denngay.Text;
            }
            else
                txt_denngay.Text = Session["denngay_donnhapvattu"].ToString();

            if (Session["index_loc_thanhtoan_donnhapvattu"] != null)//lưu lọc theo theo toán
                ddl_locdulieu.SelectedIndex = int.Parse(Session["index_loc_thanhtoan_donnhapvattu"].ToString());
            else
                Session["index_loc_thanhtoan_donnhapvattu"] = ddl_locdulieu.SelectedValue.ToString();


        }
        main();

        if (!string.IsNullOrWhiteSpace(Request.QueryString["q"]))//khi ng dùng nhấn vào nút tạo hóa đơn từ menutop --> tạo nhanh
        {
            string _q = Request.QueryString["q"].ToString().Trim();
            if (_q == "add")
                show_add = "block";
        }
    }

    public string return_ten_nhomvattu(string _id)
    {
        var q = db.nhomvattu_tables.Where(p=>p.id.ToString()== _id && p.id_chinhanh == Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return q.First().tennhom;
        return "";
    }
    public string return_ten_nhacungcap(string _id)
    {
        var q = db.nhacungcap_nhaphang_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return q.First().ten;
        return "";
    }

    public void main()
    {
        //lấy dữ liệu
        var list_all = (from ob1 in db.donnhap_vattu_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()&& p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_donnhapvattu"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_donnhapvattu"].ToString()).Date).ToList()
                        join ob2 in db.donnhap_vattu_chitiet_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on ob1.id.ToString() equals ob2.id_hoadon
                        //join ob3 in db.bspa_lichsu_thanhtoan_tables.ToList() on ob1.id.ToString() equals ob3.id_hoadon  --> k đc, k có thanh toán là k hiện đơn
                        group ob2 by new
                        {
                            ob1.id,
                            ob1.id_guide,
                            ob1.ngaytao,
                            ob1.nguoitao,
                            ob1.chietkhau,
                            ob1.tongtien_ck_hoadon,
                            ob1.tongtien,
                            ob1.tongsauchietkhau,
                            ob1.sotien_dathanhtoan,
                            ob1.sotien_conlai,
                          
                            ob1.dichvu_hay_sanpham,//"dichvu" "sanpham" "dichvusanpham"
                            ob1.sl_dichvu,
                            ob1.sl_sanpham,
                            ob1.ds_dichvu,
                            ob1.ds_sanpham,
                            ob1.sauck_dichvu,
                            ob1.sauck_sanpham,
                            ob1.nguongoc,
                            ob1.nhacungcap,
                            
                        } into g
                        select new
                        {
                            id = g.Key.id,
                            id_guide = g.Key.id_guide,
                            ngaytao = g.Key.ngaytao,
                            nguoitao = g.Key.nguoitao,

                            ck_hoadon = g.Key.chietkhau,
                            tongtien_ck = g.Key.tongtien_ck_hoadon,
                            tongtien = g.Key.tongtien,
                            tongsauchietkhau = g.Key.tongsauchietkhau,
                            sotien_dathanhtoan = g.Key.sotien_dathanhtoan,
                            sotien_conlai = g.Key.sotien_conlai,

                            phanloai_hoadon = g.Key.dichvu_hay_sanpham,//"dichvu" "sanpham" "dichvusanpham"
                            sl_dv = g.Key.sl_dichvu,
                            sl_sp = g.Key.sl_sanpham,
                            ds_dv = g.Key.ds_dichvu,
                            ds_sp = g.Key.ds_sanpham,
                            sauck_dv = g.Key.sauck_dichvu,
                            sauck_sp = g.Key.sauck_sanpham,
                            tongtien_dichvu = g.Where(p => p.kyhieu == "dichvu").Sum(ob2 => ob2.tongsauchietkhau).Value,
                            tongtien_sanpham = g.Where(p => p.kyhieu == "sanpham").Sum(ob2 => ob2.tongsauchietkhau).Value,
                            nguongoc = g.Key.nguongoc,
                            nhacungcap = g.Key.nhacungcap,
                        });


        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.nguoitao.ToLower().Contains(_key)).ToList();
            list_all = list_all.Intersect(list_search).ToList();
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


        //TÍNH HÓA ĐƠN
        hoadon_sl = list_all.Count();
        doanhso_hoadon = list_all.Sum(p => p.tongtien).Value;
        doanhso_hoadon_sauck = list_all.Sum(p => p.tongsauchietkhau).Value;
        dichvu_soluong = list_all.Sum(p => p.sl_dv).Value;
        sanpham_soluong = list_all.Sum(p => p.sl_sp).Value;
        tongtien_dathanhtoan = list_all.Sum(p => p.sotien_dathanhtoan).Value;
        //tong_tienmat = list_all.Sum(p => p.tienmat).Value;
        //tong_chuyenkhoan = list_all.Sum(p => p.chuyenkhoan).Value;
        //tong_quetthe = list_all.Sum(p => p.quetthe).Value;
        tong_congno = list_all.Sum(p => p.sotien_conlai).Value;
        doanhso_dichvu = list_all.Sum(p => p.ds_dv).Value;
        doanhso_sanpham = list_all.Sum(p => p.ds_sp).Value;
        dichvu_sauck = list_all.Sum(p => p.sauck_dv).Value;
        sanpham_sauck = list_all.Sum(p => p.sauck_sp).Value;
        tongrieng_dichvu = list_all.Sum(p => p.tongtien_dichvu);
        tongrieng_sanpham = list_all.Sum(p => p.tongtien_sanpham);


        //sắp xếp
        switch (Session["index_sapxep_donnhapvattu"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.ngaytao).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
            default: list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
        }

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 30;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_donnhapvattu"].ToString());
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
        Session["search_donnhapvattu"] = txt_search.Text.Trim();
        Session["current_page_donnhapvattu"] = "1";
        main();

    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_donnhapvattu"] = int.Parse(Session["current_page_donnhapvattu"].ToString()) - 1;
        if (int.Parse(Session["current_page_donnhapvattu"].ToString()) < 1)
            Session["current_page_donnhapvattu"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_donnhapvattu"] = int.Parse(Session["current_page_donnhapvattu"].ToString()) + 1;
        if (int.Parse(Session["current_page_donnhapvattu"].ToString()) > total_page)
            Session["current_page_donnhapvattu"] = total_page;
        main();
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


    public void reset_ss()
    {
        Session["index_sapxep_donnhapvattu"] = null;
        Session["current_page_donnhapvattu"] = null;
        Session["search_donnhapvattu"] = null;
        Session["show_donnhapvattu"] = null;
        Session["tungay_donnhapvattu"] = null;
        Session["denngay_donnhapvattu"] = null;
        Session["index_loc_thanhtoan_donnhapvattu"] = null;

    }
    protected void Button2_Click(object sender, EventArgs e)//reset
    {
        reset_ss();
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-vat-tu/vat-tu-da-nhap.aspx");
    }

    protected void Button1_Click(object sender, EventArgs e)//but lọc
    {
        Session["index_sapxep_donnhapvattu"] = DropDownList2.SelectedIndex;
        Session["current_page_donnhapvattu"] = "1";
        Session["search_donnhapvattu"] = txt_search.Text.Trim();
        Session["show_donnhapvattu"] = txt_show.Text.Trim();

        Session["tungay_donnhapvattu"] = txt_tungay.Text;
        Session["denngay_donnhapvattu"] = txt_denngay.Text;

        Session["index_loc_thanhtoan_donnhapvattu"] = ddl_locdulieu.SelectedIndex;


        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button4_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q13_6") == "")
        {
            int _count = 0;
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    var q = db.donnhaphang_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (nvt_cl.exist_id(_id, user_parent))
                    {
                        nvt_cl.del(_id);
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
