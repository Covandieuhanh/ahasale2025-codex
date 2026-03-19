using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_hoa_don_lich_su_ban_hang : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    hoadon_class hd_cl = new hoadon_class();
    datetime_class dt_cl = new datetime_class();
    public string user, user_parent, user_chitiet, hoten;
    public Int64 tongtien, tong_sauck, tongtien_ck, tongtien_chot, tongtien_lam, tongds, tongtien_banthe;
    public int sl_chot, sl_lam, sl_banthe;
    #region phân trang
    public int stt = 1, current_page = 1, show = 50, total_page = 1;
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
        if (bcorn_class.check_quyen(user, "q2_10") == "" || bcorn_class.check_quyen(user, "n2_10") == "")
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["user"]))
            {
                user_chitiet = Request.QueryString["user"].ToString().Trim();
                if (tk_cl.exist_user(user))
                {
                    if (bcorn_class.check_quyen(user, "q2_10") == "")//neu la quyen cap chi nhanh
                    {

                    }
                    else//neu la quyen cap nganh
                    {
                        if (tk_cl.return_object(user_chitiet).id_nganh != Session["nganh"].ToString())
                        {
                            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để xem ngành khác.", "false", "false", "OK", "alert", "");
                            Response.Redirect("/gianhang/admin");
                        }
                    }

                    hoten = tk_cl.return_object(user_chitiet).hoten;
                    if (!IsPostBack)
                    {
                        var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString());

                        if (Session["current_page_ctds"] == null)//lưu giữ trang hiện tại
                            Session["current_page_ctds"] = "1";
                        if (Session["index_sapxep_ctds"] == null)////lưu sắp xếp
                        {
                            DropDownList2.SelectedIndex = 1;//mặc định là giảm dần theo thời gian
                            Session["index_sapxep_ctds"] = DropDownList2.SelectedIndex.ToString();
                        }
                        else
                            DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_ctds"].ToString());
                        if (Session["search_ctds"] != null)//lưu tìm kiếm
                            txt_search.Text = Session["search_ctds"].ToString();
                        else
                            Session["search_ctds"] = txt_search.Text;
                        if (Session["show_ctds"] == null)//lưu số dòng mặc định
                        {
                            txt_show.Text = "30";
                            Session["show_ctds"] = txt_show.Text;
                        }
                        else
                            txt_show.Text = Session["show_ctds"].ToString();

                        if (Session["tungay_dsnv"] == null)
                        {
                            txt_tungay.Text = "01/01/2023";
                            Session["tungay_dsnv"] = txt_tungay.Text;
                        }
                        else
                            txt_tungay.Text = Session["tungay_dsnv"].ToString();
                        if (Session["denngay_dsnv"] == null)
                        {
                            txt_denngay.Text = DateTime.Now.ToShortDateString();
                            Session["denngay_dsnv"] = txt_denngay.Text;
                        }
                        else
                            txt_denngay.Text = Session["denngay_dsnv"].ToString();

                        if (Session["index_loc_lsbv_mathang"] != null)//lưu lọc theo mặt hàng
                            ddl_loc_mathang.SelectedIndex = int.Parse(Session["index_loc_lsbv_mathang"].ToString());
                        else
                            Session["index_loc_lsbv_mathang"] = ddl_loc_mathang.SelectedValue.ToString();

                    }
                    main();
                }
                else
                {
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                    Response.Redirect("/gianhang/admin");
                }
            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin");
            }
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
        //lấy dữ liệu
        var list_all = (from ob1 in db.bspa_hoadon_chitiet_tables.Where(p => (p.nguoichot_dvsp == user_chitiet&& p.id_chinhanh == Session["chinhanh"].ToString()) || (p.nguoilam_dichvu == user_chitiet&& p.id_chinhanh == Session["chinhanh"].ToString())).ToList()
                        join ob2 in db.bspa_hoadon_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on ob1.id_hoadon equals ob2.id.ToString()
                        select new
                        {
                            id = ob1.id,
                            ngaytao = ob1.ngaytao,
                            tenkhachhang = ob2.tenkhachhang,
                            sdt = ob2.sdt,
                            tongsauchietkhau = ob1.tongsauchietkhau,

                            tendvsp = ob1.ten_dvsp_taithoidiemnay,
                            gia = ob1.gia_dvsp_taithoidiemnay,
                            soluong = ob1.soluong.Value,
                            thanhtien = ob1.thanhtien,
                            chietkhau = ob1.chietkhau,
                            tongtien_ck = ob1.tongtien_ck_dvsp,
                            sauck = ob1.tongsauchietkhau,

                            id_hoadon = ob1.id_hoadon,
                            kyhieu = ob1.kyhieu,
                            phantramchot = ob1.phantram_chotsale_dvsp,
                            tongtien_chot = ob1.tongtien_chotsale_dvsp == 0 ? 0 : ob1.tongtien_chotsale_dvsp,
                            phantramlam = ob1.phantram_lamdichvu.Value,
                            tongtien_lam = ob1.tongtien_lamdichvu.Value == 0 ? 0 : ob1.tongtien_lamdichvu.Value,

                            userlam = ob1.nguoilam_dichvu,
                            userchot = ob1.nguoichot_dvsp,
                            id_thedichvu = ob1.id_thedichvu,
                            kyhieu_list = "hoadon",
                        });

        var list_banthe = (from ob1 in db.thedichvu_tables.Where(p => p.nguoichotsale == user_chitiet&& p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                           select new
                           {
                               id = ob1.id,
                               ngaytao = ob1.ngaytao,
                               tenkhachhang = ob1.tenkh,
                               sdt = ob1.sdt,
                               tongsauchietkhau = ob1.tongsauchietkhau,

                               tendvsp = ob1.tenthe,
                               gia = ob1.tongtien,
                               soluong = int.Parse("1"),
                               thanhtien = ob1.tongtien,
                               chietkhau = ob1.chietkhau,
                               tongtien_ck = ob1.tongtien_ck_hoadon,
                               sauck = ob1.tongsauchietkhau,

                               id_hoadon = ob1.id.ToString(),//là id thẻ
                               kyhieu = "dichvu",
                               phantramchot = ob1.phantram_chotsale,
                               tongtien_chot = ob1.tongtien_chotsale_dvsp == 0 ? 0 : ob1.tongtien_chotsale_dvsp,
                               phantramlam = 0,//k có làm, chỉ có chốt
                               tongtien_lam = Int64.Parse("0"),//k có làm, chỉ có chốt

                               userlam = "",//k có làm, chỉ có chốt
                               userchot = ob1.nguoichotsale,
                               id_thedichvu = ob1.id.ToString(),
                               kyhieu_list = "thedv",
                           });

        list_all = list_all.Union(list_banthe);
        list_all = list_all.Where(p => p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_dsnv"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_dsnv"].ToString()).Date);

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.tenkhachhang.ToLower().Contains(_key) || p.tendvsp.ToLower().Contains(_key) || p.sdt == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        //xử lý lọc dữ liệu


        if (ddl_loc_mathang.SelectedValue.ToString() != "0")
        {
            var list_1 = list_all.Where(p => p.kyhieu == ddl_loc_mathang.SelectedValue.ToString()).ToList();
            list_all = list_all.Intersect(list_1).ToList();
        }

        tongtien_chot = list_all.Where(p => p.userchot == user_chitiet && p.kyhieu_list == "hoadon").Sum(p => p.tongtien_chot).Value;
        tongtien_lam = list_all.Where(p => p.userlam == user_chitiet && p.kyhieu_list == "hoadon").Sum(p => p.tongtien_lam);
        sl_chot = list_all.Where(p => p.userchot == user_chitiet && p.kyhieu_list == "hoadon").Count();
        sl_lam = list_all.Where(p => p.userlam == user_chitiet && p.kyhieu_list == "hoadon").Count();

        tongtien_banthe = list_all.Where(p => p.userchot == user_chitiet && p.kyhieu_list == "thedv").Sum(p => p.tongtien_chot).Value;
        sl_banthe = list_all.Where(p => p.userchot == user_chitiet && p.kyhieu_list == "thedv").Count();

        tongds = tongtien_chot + tongtien_lam + tongtien_banthe;

        tongtien_ck = list_all.Sum(p => p.tongtien_ck).Value;
        tongtien = list_all.Sum(p => p.thanhtien).Value;
        tong_sauck = list_all.Sum(p => p.sauck).Value;
        //tongsoluong = list_all.Sum(p => p.soluong);

        //sắp xếp
        switch (Session["index_sapxep_ctds"].ToString())
        {
            //case ("1"): list_all = list_all.OrderBy(p => p.ngaytao_tk).ToList(); break;
            default: list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
        }

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 50;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_ctds"].ToString());
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
        Session["search_ctds"] = txt_search.Text.Trim();
        Session["current_page_ctds"] = "1";
        main();

    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_ctds"] = int.Parse(Session["current_page_ctds"].ToString()) - 1;
        if (int.Parse(Session["current_page_ctds"].ToString()) < 1)
            Session["current_page_ctds"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_ctds"] = int.Parse(Session["current_page_ctds"].ToString()) + 1;
        if (int.Parse(Session["current_page_ctds"].ToString()) > total_page)
            Session["current_page_ctds"] = total_page;
        main();
    }

    protected void Button1_Click(object sender, EventArgs e)//but lọc
    {
        Session["index_sapxep_ctds"] = DropDownList2.SelectedIndex;
        Session["current_page_ctds"] = "1";
        Session["search_ctds"] = txt_search.Text.Trim();
        Session["show_ctds"] = txt_show.Text.Trim();
        Session["tungay_dsnv"] = txt_tungay.Text;
        Session["denngay_dsnv"] = txt_denngay.Text;


        Session["index_loc_lsbv_mathang"] = ddl_loc_mathang.SelectedIndex;
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button2_Click(object sender, EventArgs e)//reset
    {
        reset_ss();
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-hoa-don/lich-su-ban-hang.aspx");
    }
    public void reset_ss()
    {
        Session["index_sapxep_ctds"] = null;
        Session["current_page_ctds"] = null;
        Session["search_ctds"] = null;
        Session["show_ctds"] = null;
        Session["tungay_dsnv"] = null;
        Session["denngay_dsnv"] = null;

        Session["index_loc_lsbv_mathang"] = null;
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
