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


public partial class badmin_Default : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    string_class str_cl = new string_class();
    data_khachhang_class dtkh_cl = new data_khachhang_class();
    datetime_class dt_cl = new datetime_class();
    public string user, user_parent, show_add = "none";
    public int tongdon, tong_sl_dv, tong_sl_sp;
    public Int64 sauck, tong_congno;
    #region phân trang
    public int stt = 1, current_page = 1, show = 50, total_page = 1;
    List<string> list_id_split;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login  
        string _quyen = "q8_1";
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
            var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                 select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                      );
            ddl_nhanvien_chamsoc.DataSource = list_nhanvien;
            ddl_nhanvien_chamsoc.DataTextField = "tennhanvien";
            ddl_nhanvien_chamsoc.DataValueField = "username";
            ddl_nhanvien_chamsoc.DataBind();
            ddl_nhanvien_chamsoc.Items.Insert(0, new ListItem("Chọn", ""));

            var list_nohmkhachhang = (from ob1 in db.nhomkhachhang_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                      select new { id = ob1.id, tennhom = ob1.tennhom, }
                      );
            DropDownList1.DataSource = list_nohmkhachhang;
            DropDownList1.DataTextField = "tennhom";
            DropDownList1.DataValueField = "id";
            DropDownList1.DataBind();
            DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));

            DropDownList3.DataSource = list_nohmkhachhang;
            DropDownList3.DataTextField = "tennhom";
            DropDownList3.DataValueField = "id";
            DropDownList3.DataBind();
            DropDownList3.Items.Insert(0, new ListItem("Tất cả", ""));


            var q = db.bspa_hoadon_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString());

            if (Session["current_page_dtkh"] == null)//lưu giữ trang hiện tại
                Session["current_page_dtkh"] = "1";

            if (Session["index_sapxep_dtkh"] == null)////lưu sắp xếp
            {
                DropDownList2.SelectedIndex = 1;//mặc định là giảm dần theo thời gian
                Session["index_sapxep_dtkh"] = DropDownList2.SelectedIndex.ToString();
            }
            else
                DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_dtkh"].ToString());

            if (Session["search_dtkh"] != null)//lưu tìm kiếm
                txt_search.Text = Session["search_dtkh"].ToString();
            else
                Session["search_dtkh"] = txt_search.Text;

            if (Session["show_dtkh"] == null)//lưu số dòng mặc định
            {
                txt_show.Text = "30";
                Session["show_dtkh"] = txt_show.Text;
            }
            else
                txt_show.Text = Session["show_dtkh"].ToString();

            if (Session["tungay_dtkh"] == null)
            {
                txt_tungay.Text = q.Count() != 0 ? q.Min(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                Session["tungay_dtkh"] = txt_tungay.Text;
            }
            else
                txt_tungay.Text = Session["tungay_dtkh"].ToString();

            if (Session["denngay_dtkh"] == null)
            {
                txt_denngay.Text = q.Count() != 0 ? q.Max(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                Session["denngay_dtkh"] = txt_denngay.Text;
            }
            else
                txt_denngay.Text = Session["denngay_dtkh"].ToString();

            if (Session["index_loc_nhomkh_dtkh"] != null)//lưu lọc theo nhóm kh
            {
                DropDownList3.SelectedIndex = int.Parse(Session["index_loc_nhomkh_dtkh"].ToString());
            }
            else
                Session["index_loc_nhomkh_dtkh"] = DropDownList3.SelectedIndex.ToString();
        }
        main();
        if (!string.IsNullOrWhiteSpace(Request.QueryString["q"]))//khi ng dùng nhấn vào nút tạo hóa đơn từ menutop --> tạo nhanh
        {
            string _q = Request.QueryString["q"].ToString().Trim();
            if (_q == "add")
                show_add = "block";
        }
    }
    public void main()
    {
        //lấy dữ liệu từ bên hoadon
        //var list_all = (from ob1 in db.bspa_data_khachhang_tables.Where(p => p.kyhieu == "hoadon").ToList()
        //                join ob2 in db.bspa_hoadon_tables.ToList() on ob1.sdt.ToString() equals ob2.sdt
        //                group ob2 by new { ob1.id, ob1.sdt, ob1.diachi, ob1.tenkhachhang,ob1.anhdaidien } into g
        //                select new
        //                {
        //                    id = g.Key.id,
        //                    sdt = g.Key.sdt,
        //                    tenkhachhang = g.Key.tenkhachhang,
        //                    diachi = g.Key.diachi,
        //                    sl_hoadon = g.Count(),
        //                    avt=g.Key.anhdaidien,
        //                    //tongtien = Int64.Parse(g.Sum(ob2 => ob2.tongtien).ToString()),
        //                    sauck = Int64.Parse(g.Sum(ob2 => ob2.tongsauchietkhau).ToString()),
        //                    //dathanhtoan = Int64.Parse(g.Sum(ob2 => ob2.sotien_dathanhtoan).ToString()),
        //                    sotien_conlai = Int64.Parse(g.Sum(ob2 => ob2.sotien_conlai).ToString()),
        //                    sl_dv = int.Parse(g.Sum(ob2 => ob2.sl_dichvu).ToString()),
        //                    sl_sp = int.Parse(g.Sum(ob2 => ob2.sl_sanpham).ToString()),
        //                });

        var list_all = (from ob1 in db.bspa_data_khachhang_tables.Where(p => p.nhomkhachhang != "" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                        join ob2 in db.nhomkhachhang_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on ob1.nhomkhachhang equals ob2.id.ToString()
                        select new
                        {
                            id = ob1.id,
                            sdt = ob1.sdt,
                            tenkhachhang = ob1.tenkhachhang,
                            diachi = ob1.diachi,
                            sl_hoadon = 0,
                            avt = ob1.anhdaidien,
                            ngaytao = ob1.ngaytao,
                            nguoitao = tk_cl.return_hoten(ob1.nguoitao),
                            //tongtien = Int64.Parse("0"),
                            sauck = Int64.Parse("0"),
                            //dathanhtoan = Int64.Parse("0"),
                            sotien_conlai = Int64.Parse("0"),
                            sl_dv = int.Parse("0"),
                            sl_sp = int.Parse("0"),
                            tennhom = ob2.tennhom,
                            idnhom = ob2.id.ToString(),
                        }).ToList();

        var list_no = db.bspa_data_khachhang_tables.Where(p => p.nhomkhachhang == "" && p.id_chinhanh == Session["chinhanh"].ToString()).Select(p => new
        {
            id = p.id,
            sdt = p.sdt,
            tenkhachhang = p.tenkhachhang,
            diachi = p.diachi,
            sl_hoadon = 0,
            avt = p.anhdaidien,
            ngaytao = p.ngaytao,
            nguoitao = tk_cl.return_hoten(p.nguoitao),
            //tongtien = Int64.Parse("0"),
            sauck = Int64.Parse("0"),
            //dathanhtoan = Int64.Parse("0"),
            sotien_conlai = Int64.Parse("0"),
            sl_dv = int.Parse("0"),
            sl_sp = int.Parse("0"),
            tennhom = "",
            idnhom = "",
        });
        list_all = list_all.Union(list_no).ToList();

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.tenkhachhang.ToLower().Contains(_key) || p.sdt.Contains(_key)).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        //lọc theo nhóm khách hàng
        if (DropDownList3.SelectedValue.ToString() != "")
        {
            var list_1 = list_all.Where(p => p.idnhom == DropDownList3.SelectedValue.ToString()).ToList();
            list_all = list_all.Intersect(list_1).ToList();
        }

        //tongdon = list_all.Sum(p => p.sl_hoadon);
        //tong_sl_dv = list_all.Sum(p => p.sl_dv);
        //tong_sl_sp = list_all.Sum(p => p.sl_sp);

        //tongtien = list_all.Sum(p => p.tongtien);
        //sauck = list_all.Sum(p => p.sauck);
        //tong_thanhtoan = list_all.Sum(p => p.dathanhtoan);
        //tong_congno = list_all.Sum(p => p.sotien_conlai);

        //sắp xếp
        switch (Session["index_sapxep_dtkh"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.ngaytao).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
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
        current_page = int.Parse(Session["current_page_dtkh"].ToString());
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
        Session["search_dtkh"] = txt_search.Text.Trim();
        Session["current_page_dtkh"] = "1";
        main();

    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_dtkh"] = int.Parse(Session["current_page_dtkh"].ToString()) - 1;
        if (int.Parse(Session["current_page_dtkh"].ToString()) < 1)
            Session["current_page_dtkh"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_dtkh"] = int.Parse(Session["current_page_dtkh"].ToString()) + 1;
        if (int.Parse(Session["current_page_dtkh"].ToString()) > total_page)
            Session["current_page_dtkh"] = total_page;
        main();
    }



    protected void but_xoa_Click(object sender, ImageClickEventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q8_4") == "")
        {
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    if (dtkh_cl.exist_id(_id, user_parent))
                        dtkh_cl.del(_id);
                }
            }
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "warning"), true);
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
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
        Session["index_sapxep_dtkh"] = null;
        Session["current_page_dtkh"] = null;
        Session["search_dtkh"] = null;
        Session["show_dtkh"] = null;
        Session["tungay_dtkh"] = null;
        Session["denngay_dtkh"] = null;

        Session["index_loc_nhomkh_dtkh"] = null;

    }
    protected void Button2_Click(object sender, EventArgs e)//reset
    {
        reset_ss();
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-khach-hang/Default.aspx");
    }

    protected void Button1_Click(object sender, EventArgs e)//but lọc
    {
        Session["index_sapxep_dtkh"] = DropDownList2.SelectedIndex;
        Session["current_page_dtkh"] = "1";
        Session["search_dtkh"] = txt_search.Text.Trim();
        Session["show_dtkh"] = txt_show.Text.Trim();
        Session["tungay_dtkh"] = txt_tungay.Text;
        Session["denngay_dtkh"] = txt_denngay.Text;

        Session["index_loc_nhomkh_dtkh"] = DropDownList3.SelectedIndex;

        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        string _tenkhachhang = str_cl.VietHoa_ChuCai_DauTien(txt_tenkhachhang.Text.Trim().ToLower());
        string _sdt = str_cl.remove_blank(txt_sdt.Text.Trim()).Replace(" ", "").Replace(".", "").Replace("-", "").Replace("+", "");
        string _diachi = txt_diachi.Text;
        if (_tenkhachhang == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tên khách hàng.", "false", "false", "OK", "alert", ""), true);
        else
        {
            if (_sdt == "")
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập số điện thoại khách hàng.", "false", "false", "OK", "alert", ""), true);
            else
            {
                var q_data = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt && p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString());
                if (q_data.Count() == 0)//chưa có sdt thì thêm vào
                {
                    bspa_data_khachhang_table _ob1 = new bspa_data_khachhang_table();
                    _ob1.tenkhachhang = _tenkhachhang;
                    _ob1.diachi = _diachi;
                    _ob1.magioithieu = txt_magioithieu.Text.Trim();
                    _ob1.ngaytao = DateTime.Now;
                    _ob1.nguoitao = user;
                    _ob1.nguoichamsoc = ddl_nhanvien_chamsoc.SelectedValue.ToString();
                    _ob1.sdt = _sdt;
                    _ob1.user_parent = user_parent;
                    _ob1.nhomkhachhang = DropDownList1.SelectedValue.ToString();
                    _ob1.matkhau= encode_class.encode_md5(encode_class.encode_sha1("12345678"));
                    _ob1.anhdaidien = "/uploads/images/macdinh.jpg";
                    if (txt_ngaysinh.Text != "" && dt_cl.check_date(txt_ngaysinh.Text) == true)
                        _ob1.ngaysinh = DateTime.Parse(txt_ngaysinh.Text);

                    _ob1.id_chinhanh = Session["chinhanh"].ToString();
                    _ob1.capbac = ""; _ob1.vnd_tu_e_aha = 0; _ob1.sodiem_e_aha = 0; _ob1.solan_lencap = 0;
                    db.bspa_data_khachhang_tables.InsertOnSubmit(_ob1);
                    db.SubmitChanges();

                    reset_control();
                    main();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm thành công.", "4000", "warning"), true);
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số điện thoại đã tồn tại.", "false", "false", "OK", "alert", ""), true);
            }
        }
    }

    protected void Button4_Click(object sender, EventArgs e)
    {
        #region lấy dữ liệu
        var list_all = db.bspa_data_khachhang_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).Select(p => new
        {
            id = p.id,
            sdt = p.sdt,
            tenkhachhang = p.tenkhachhang,
            diachi = p.diachi,
            sl_hoadon = 0,
            avt = p.anhdaidien,
            ngaytao = p.ngaytao,
            nguoitao = tk_cl.return_object(p.nguoitao).hoten,
            //tongtien = Int64.Parse("0"),
            //sauck = Int64.Parse("0"),
            //dathanhtoan = Int64.Parse("0"),
            //sotien_conlai = Int64.Parse("0"),
            //sl_dv = int.Parse("0"),
            //sl_sp = int.Parse("0"),
        }).ToList();

        //list_all = list_all.Union(list_data).ToList();

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.tenkhachhang.ToLower().Contains(_key)).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        //tongdon = list_all.Sum(p => p.sl_hoadon);
        //tong_sl_dv = list_all.Sum(p => p.sl_dv);
        //tong_sl_sp = list_all.Sum(p => p.sl_sp);

        //tongtien = list_all.Sum(p => p.tongtien);
        //sauck = list_all.Sum(p => p.sauck);
        //tong_thanhtoan = list_all.Sum(p => p.dathanhtoan);
        //tong_congno = list_all.Sum(p => p.sotien_conlai);

        //sắp xếp
        switch (Session["index_sapxep_dtkh"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.ngaytao).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
            default: list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
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
                            case "tenkhachhang": newRow.CreateCell(_socot1).SetCellValue(item.tenkhachhang); break;
                            case "diachi": newRow.CreateCell(_socot1).SetCellValue(item.diachi); break;
                            case "sdt": newRow.CreateCell(_socot1).SetCellValue(item.sdt); break;
                            default: break;
                        }
                        _socot1 = _socot1 + 1;
                    }
                }

                // tăng index
                rowIndex++;
            }

            // xong hết thì save file lại
            string _filetenbv = Guid.NewGuid().ToString();
            FileStream fs = new FileStream(Server.MapPath("~/uploads/Files/" + _filetenbv + ".xlsx"), FileMode.CreateNew);
            wb.Write(fs);
            Response.Redirect("/uploads/Files/" + _filetenbv + ".xlsx");
        }
    }
    public void reset_control()
    {
        txt_tenkhachhang.Text = ""; txt_sdt.Text = "";
        txt_ngaysinh.Text = ""; txt_diachi.Text = "";
        ddl_nhanvien_chamsoc.SelectedIndex = 0;
        txt_magioithieu.Text = "";
    }
}
