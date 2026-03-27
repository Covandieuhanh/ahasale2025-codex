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

public partial class admin_quan_ly_hoa_don_lich_su_ban_hang : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    hoadon_class hd_cl = new hoadon_class();
    datetime_class dt_cl = new datetime_class(); nganh_class ng_cl = new nganh_class();
    public string user, user_parent;
    public Int64 tongtien, tong_sauck, tongtien_ck, tongtien_chot, tongtien_lam, tongsoluong;
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
        user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();
        if (bcorn_class.check_quyen(user, "q7_8") == "" || bcorn_class.check_quyen(user, "n7_8") == "")
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
                if (bcorn_class.check_quyen(user, "q7_8") == "")
                {
                }
                else
                {
                    DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList5.Enabled = false;
                }
                if (Session["index_loc_nganh_lsbh"] != null)//lưu lọc theo theo toán
                    DropDownList5.SelectedIndex = int.Parse(Session["index_loc_nganh_lsbh"].ToString());
                else
                    Session["index_loc_nganh_lsbh"] = DropDownList5.SelectedIndex.ToString();
                //var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString());

                if (Session["current_page_lsbh"] == null)//lưu giữ trang hiện tại
                    Session["current_page_lsbh"] = "1";
                if (Session["index_sapxep_lsbh"] == null)////lưu sắp xếp
                {
                    DropDownList2.SelectedIndex = 1;//mặc định là giảm dần theo thời gian
                    Session["index_sapxep_lsbh"] = DropDownList2.SelectedIndex.ToString();
                }
                else
                    DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_lsbh"].ToString());
                if (Session["search_lsbh"] != null)//lưu tìm kiếm
                    txt_search.Text = Session["search_lsbh"].ToString();
                else
                    Session["search_lsbh"] = txt_search.Text;
                if (Session["show_lsbh"] == null)//lưu số dòng mặc định
                {
                    txt_show.Text = "30";
                    Session["show_lsbh"] = txt_show.Text;
                }
                else
                    txt_show.Text = Session["show_lsbh"].ToString();

                if (Session["tungay_lsbh"] == null)
                {
                    txt_tungay.Text = "01/01/2023";
                    Session["tungay_lsbh"] = txt_tungay.Text;
                }
                else
                    txt_tungay.Text = Session["tungay_lsbh"].ToString();
                if (Session["denngay_lsbh"] == null)
                {
                    txt_denngay.Text = DateTime.Now.ToShortDateString();
                    Session["denngay_lsbh"] = txt_denngay.Text;
                }
                else
                    txt_denngay.Text = Session["denngay_lsbh"].ToString();



                var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() select new { username = ob1.taikhoan, hoten = ob1.hoten, });

                ddl_nhanvien_lamdichvu.DataSource = list_nhanvien.OrderBy(p => p.hoten);
                ddl_nhanvien_lamdichvu.DataTextField = "hoten";
                ddl_nhanvien_lamdichvu.DataValueField = "username";
                ddl_nhanvien_lamdichvu.DataBind();
                ddl_nhanvien_lamdichvu.Items.Insert(0, new ListItem("Tất cả", "0"));

                ddl_nhanvien_chotsale.DataSource = list_nhanvien.OrderBy(p => p.hoten);
                ddl_nhanvien_chotsale.DataTextField = "hoten";
                ddl_nhanvien_chotsale.DataValueField = "username";
                ddl_nhanvien_chotsale.DataBind();
                ddl_nhanvien_chotsale.Items.Insert(0, new ListItem("Tất cả", "0"));

                if (Session["index_loc_lsbv_nv_lamdichvu"] != null)//lưu lọc theo nv làm dịch vụ
                    ddl_nhanvien_lamdichvu.SelectedIndex = int.Parse(Session["index_loc_lsbv_nv_lamdichvu"].ToString());
                else
                    Session["index_loc_lsbv_nv_lamdichvu"] = ddl_nhanvien_lamdichvu.SelectedValue.ToString();

                if (Session["index_loc_lsbv_nv_chotsale"] != null)//lưu lọc theo nv chốt sale
                    ddl_nhanvien_chotsale.SelectedIndex = int.Parse(Session["index_loc_lsbv_nv_chotsale"].ToString());
                else
                    Session["index_loc_lsbv_nv_chotsale"] = ddl_nhanvien_chotsale.SelectedValue.ToString();

                if (Session["index_loc_lsbv_mathang"] != null)//lưu lọc theo mặt hàng
                    ddl_loc_mathang.SelectedIndex = int.Parse(Session["index_loc_lsbv_mathang"].ToString());
                else
                    Session["index_loc_lsbv_mathang"] = ddl_loc_mathang.SelectedValue.ToString();

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
        //lấy dữ liệu
        var list_all = (from ob1 in db.bspa_hoadon_chitiet_tables.Where(p =>p.id_chinhanh == Session["chinhanh"].ToString()&& p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_lsbh"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_lsbh"].ToString()).Date).ToList()
                        join ob2 in db.bspa_hoadon_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on ob1.id_hoadon equals ob2.id.ToString()
                        select new
                        {
                            id = ob1.id,
                            ngaytao = ob1.ngaytao,
                            id_hoadon = ob1.id_hoadon,
                            tenkhachhang = ob2.tenkhachhang,
                            sdt = ob2.sdt,
                            kyhieu = ob1.kyhieu,
                            tendvsp = ob1.ten_dvsp_taithoidiemnay,
                            gia = ob1.gia_dvsp_taithoidiemnay,
                            soluong = ob1.soluong,
                            thanhtien = ob1.thanhtien,
                            chietkhau = ob1.chietkhau,
                            sauck = ob1.tongsauchietkhau,
                            tongtien_ck = ob1.tongtien_ck_dvsp,
                            tennguoilam_xuat_excel = tk_cl.exist_user_of_userparent(ob1.nguoilam_dichvu, user_parent) == false ? ob1.nguoilam_dichvu : tk_cl.return_object(ob1.nguoilam_dichvu).hoten,
                            tennguoichot_xuat_excel = tk_cl.exist_user_of_userparent(ob1.nguoichot_dvsp, user_parent) == false ? ob1.nguoichot_dvsp : tk_cl.return_object(ob1.nguoichot_dvsp).hoten,

                            phantramchot = ob1.phantram_chotsale_dvsp,
                            tongtien_chot = ob1.tongtien_chotsale_dvsp == 0 ? 0 : ob1.tongtien_chotsale_dvsp,
                            phantramlam = ob1.phantram_lamdichvu,
                            tongtien_lam = ob1.tongtien_lamdichvu.Value == 0 ? 0 : ob1.tongtien_lamdichvu.Value,

                            userlam = ob1.nguoilam_dichvu,
                            userchot = ob1.nguoichot_dvsp,
                            tennguoilam = tk_cl.exist_user_of_userparent(ob1.nguoilam_dichvu, user_parent) == false ? ob1.nguoilam_dichvu : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nguoilam_dichvu + "'>" + tk_cl.return_object(ob1.nguoilam_dichvu).hoten + "</a></div>",
                            tennguoichot = tk_cl.exist_user_of_userparent(ob1.nguoichot_dvsp, user_parent) == false ? ob1.nguoichot_dvsp : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nguoichot_dvsp + "'>" + tk_cl.return_object(ob1.nguoichot_dvsp).hoten + "</a></div>",
                            //tennguoilam = ac_cl.exist_user_of_userparent(ob1.nguoilam_dichvu, user_parent) == false ? ob1.nguoilam_dichvu : "<div><span data-role='hint' data-hint-position='top' data-hint-text='" + ac_cl.return_object(ob1.nguoilam_dichvu).fullname + "'>" + ob1.nguoilam_dichvu + "</span></div>",
                            //tennguoichot = ac_cl.exist_user_of_userparent(ob1.nguoichot_dvsp, user_parent) == false ? ob1.nguoichot_dvsp : "<div><span data-role='hint' data-hint-position='top' data-hint-text='" + ac_cl.return_object(ob1.nguoichot_dvsp).fullname + "'>" + ob1.nguoichot_dvsp + "</span></div>",
                            id_thedichvu = ob1.id_thedichvu,
                            id_nganh = ob1.id_nganh,
                        });

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.tenkhachhang.ToLower().Contains(_key) || p.tendvsp.ToLower().Contains(_key) || p.sdt == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        //xử lý lọc dữ liệu
        if (ddl_nhanvien_lamdichvu.SelectedValue.ToString() != "0")
        {
            var list_1 = list_all.Where(p => p.userlam == ddl_nhanvien_lamdichvu.SelectedValue.ToString()).ToList();
            list_all = list_all.Intersect(list_1).ToList();
        }
        if (ddl_nhanvien_chotsale.SelectedValue.ToString() != "0")
        {
            var list_1 = list_all.Where(p => p.userchot == ddl_nhanvien_chotsale.SelectedValue.ToString()).ToList();
            list_all = list_all.Intersect(list_1).ToList();
        }
        if (ddl_loc_mathang.SelectedValue.ToString() != "0")
        {
            var list_1 = list_all.Where(p => p.kyhieu == ddl_loc_mathang.SelectedValue.ToString()).ToList();
            list_all = list_all.Intersect(list_1).ToList();
        }

        if (DropDownList5.SelectedValue.ToString() != "")//ngành
        {
            var list_1 = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList(); list_all = list_all.Intersect(list_1).ToList();
        }

        tongtien_chot = list_all.Sum(p => p.tongtien_chot).Value;
        tongtien_lam = list_all.Sum(p => p.tongtien_lam);
        tongtien_ck = list_all.Sum(p => p.tongtien_ck).Value;
        tongtien = list_all.Sum(p => p.thanhtien).Value;
        tong_sauck = list_all.Sum(p => p.sauck).Value;
        tongsoluong = list_all.Sum(p => p.soluong).Value;

        //sắp xếp
        switch (Session["index_sapxep_lsbh"].ToString())
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
        current_page = int.Parse(Session["current_page_lsbh"].ToString());
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
        Session["current_page_lsbh"] = "1";

        main();
    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_lsbh"] = int.Parse(Session["current_page_lsbh"].ToString()) - 1;
        if (int.Parse(Session["current_page_lsbh"].ToString()) < 1)
            Session["current_page_lsbh"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_lsbh"] = int.Parse(Session["current_page_lsbh"].ToString()) + 1;
        if (int.Parse(Session["current_page_lsbh"].ToString()) > total_page)
            Session["current_page_lsbh"] = total_page;
        main();
    }

    protected void Button1_Click(object sender, EventArgs e)//but lọc
    {
        Session["index_sapxep_lsbh"] = DropDownList2.SelectedIndex;
        Session["current_page_lsbh"] = "1";
        Session["search_lsbh"] = txt_search.Text.Trim();
        Session["show_lsbh"] = txt_show.Text.Trim();
        Session["tungay_lsbh"] = txt_tungay.Text;
        Session["denngay_lsbh"] = txt_denngay.Text;

        Session["index_loc_lsbv_nv_lamdichvu"] = ddl_nhanvien_lamdichvu.SelectedIndex;
        Session["index_loc_lsbv_nv_chotsale"] = ddl_nhanvien_chotsale.SelectedIndex;
        Session["index_loc_lsbv_mathang"] = ddl_loc_mathang.SelectedIndex;
        Session["index_loc_nganh_lsbh"] = DropDownList5.SelectedIndex.ToString();
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
        Session["index_sapxep_lsbh"] = null;
        Session["current_page_lsbh"] = null;
        Session["search_lsbh"] = null;
        Session["show_lsbh"] = null;
        Session["tungay_lsbh"] = null;
        Session["denngay_lsbh"] = null;

        Session["index_loc_lsbv_nv_lamdichvu"] = null;
        Session["index_loc_lsbv_nv_chotsale"] = null;
        Session["index_loc_lsbv_mathang"] = null;
        Session["index_loc_nganh_lsbh"] = null;
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


    //Chưa có chỉnh sửa chuẩn
    //protected void but_xoa_lichsu_Click(object sender, ImageClickEventArgs e)
    //{
    //    if (bcorn_class.check_quyen(user, "q7_9") == "")
    //    {
    //        int _count = 0;
    //        for (int i = 0; i < list_id_split.Count; i++)
    //        {
    //            if (Request.Form[list_id_split[i]] == "on")
    //            {
    //                string _id = list_id_split[i].Replace("check_", "");
    //                var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id.ToString() == _id);
    //                if (q.Count() != 0)
    //                {
    //                    Int64 _sotien_thanhtoan = q.First().sotienthanhtoan.Value;
    //                    string _id_hoadon = q.First().id_hoadon;
    //                    var q_hoadon = db.bspa_hoadon_tables.Where(p => p.id.ToString() == _id_hoadon);
    //                    bspa_hoadon_table _ob = q_hoadon.First();
    //                    _ob.sotien_dathanhtoan = _ob.sotien_dathanhtoan - _sotien_thanhtoan;
    //                    _ob.sotien_conlai = _ob.tongsauchietkhau - _ob.sotien_dathanhtoan;
    //                    db.SubmitChanges();

    //                    bspa_hoadon_chitiet_table _ob1 = q.First();
    //                    db.bspa_hoadon_chitiet_tables.DeleteOnSubmit(_ob1);
    //                    db.SubmitChanges();
    //                    _count = _count + 1;
    //                }
    //            }
    //            if (_count > 0)
    //            {
    //                main();
    //                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "warning"), true);
    //            }
    //        }
    //    }
    //    else
    //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    //}

    protected void Button5_Click(object sender, EventArgs e)
    {
        #region lấy dữ liệu
        var list_all = (from ob1 in db.bspa_hoadon_chitiet_tables.Where(p => p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString() && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_lsbh"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_lsbh"].ToString()).Date).ToList()
                        join ob2 in db.bspa_hoadon_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on ob1.id_hoadon equals ob2.id.ToString()
                        select new
                        {
                            id = ob1.id,
                            ngaytao = ob1.ngaytao,
                            id_hoadon = ob1.id_hoadon,
                            tenkhachhang = ob2.tenkhachhang,
                            sdt = ob2.sdt,
                            kyhieu = ob1.kyhieu,
                            tendvsp = ob1.ten_dvsp_taithoidiemnay,
                            gia = ob1.gia_dvsp_taithoidiemnay,
                            soluong = ob1.soluong,
                            thanhtien = ob1.thanhtien,
                            chietkhau = ob1.chietkhau,
                            sauck = ob1.tongsauchietkhau,
                            tongtien_ck = ob1.tongtien_ck_dvsp,
                            tennguoilam_xuat_excel = tk_cl.exist_user_of_userparent(ob1.nguoilam_dichvu, user_parent) == false ? ob1.nguoilam_dichvu : tk_cl.return_object(ob1.nguoilam_dichvu).hoten,
                            tennguoichot_xuat_excel = tk_cl.exist_user_of_userparent(ob1.nguoichot_dvsp, user_parent) == false ? ob1.nguoichot_dvsp : tk_cl.return_object(ob1.nguoichot_dvsp).hoten,

                            phantramchot = ob1.phantram_chotsale_dvsp,
                            tongtien_chot = ob1.tongtien_chotsale_dvsp == 0 ? 0 : ob1.tongtien_chotsale_dvsp,
                            phantramlam = ob1.phantram_lamdichvu,
                            tongtien_lam = ob1.tongtien_lamdichvu.Value == 0 ? 0 : ob1.tongtien_lamdichvu.Value,

                            userlam = ob1.nguoilam_dichvu,
                            userchot = ob1.nguoichot_dvsp,
                            tennguoilam = tk_cl.exist_user_of_userparent(ob1.nguoilam_dichvu, user_parent) == false ? ob1.nguoilam_dichvu : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nguoilam_dichvu + "'>" + tk_cl.return_object(ob1.nguoilam_dichvu).hoten + "</a></div>",
                            tennguoichot = tk_cl.exist_user_of_userparent(ob1.nguoichot_dvsp, user_parent) == false ? ob1.nguoichot_dvsp : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nguoichot_dvsp + "'>" + tk_cl.return_object(ob1.nguoichot_dvsp).hoten + "</a></div>",
                            //tennguoilam = ac_cl.exist_user_of_userparent(ob1.nguoilam_dichvu, user_parent) == false ? ob1.nguoilam_dichvu : "<div><span data-role='hint' data-hint-position='top' data-hint-text='" + ac_cl.return_object(ob1.nguoilam_dichvu).fullname + "'>" + ob1.nguoilam_dichvu + "</span></div>",
                            //tennguoichot = ac_cl.exist_user_of_userparent(ob1.nguoichot_dvsp, user_parent) == false ? ob1.nguoichot_dvsp : "<div><span data-role='hint' data-hint-position='top' data-hint-text='" + ac_cl.return_object(ob1.nguoichot_dvsp).fullname + "'>" + ob1.nguoichot_dvsp + "</span></div>",
                            id_thedichvu = ob1.id_thedichvu,
                            id_nganh = ob1.id_nganh,
                        });

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.tenkhachhang.ToLower().Contains(_key) || p.tendvsp.ToLower().Contains(_key) || p.sdt == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        //xử lý lọc dữ liệu
        if (ddl_nhanvien_lamdichvu.SelectedValue.ToString() != "0")
        {
            var list_1 = list_all.Where(p => p.userlam == ddl_nhanvien_lamdichvu.SelectedValue.ToString()).ToList();
            list_all = list_all.Intersect(list_1).ToList();
        }
        if (ddl_nhanvien_chotsale.SelectedValue.ToString() != "0")
        {
            var list_1 = list_all.Where(p => p.userchot == ddl_nhanvien_chotsale.SelectedValue.ToString()).ToList();
            list_all = list_all.Intersect(list_1).ToList();
        }
        if (ddl_loc_mathang.SelectedValue.ToString() != "0")
        {
            var list_1 = list_all.Where(p => p.kyhieu == ddl_loc_mathang.SelectedValue.ToString()).ToList();
            list_all = list_all.Intersect(list_1).ToList();
        }

        if (DropDownList5.SelectedValue.ToString() != "")//ngành
        {
            var list_1 = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList(); list_all = list_all.Intersect(list_1).ToList();
        }

        tongtien_chot = list_all.Sum(p => p.tongtien_chot).Value;
        tongtien_lam = list_all.Sum(p => p.tongtien_lam);
        tongtien_ck = list_all.Sum(p => p.tongtien_ck).Value;
        tongtien = list_all.Sum(p => p.thanhtien).Value;
        tong_sauck = list_all.Sum(p => p.sauck).Value;
        tongsoluong = list_all.Sum(p => p.soluong).Value;

        //sắp xếp
        switch (Session["index_sapxep_lsbh"].ToString())
        {
            //case ("1"): list_all = list_all.OrderBy(p => p.ngaytao_tk).ToList(); break;
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

            //đếm xem có bao nhiêu cột đc chọn
            // Ghi tiêu đề cột ở row 1
            var row1 = sheet.CreateRow(1);
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
                            case "ngaytao": newRow.CreateCell(_socot1).SetCellValue(item.ngaytao.Value); break;
                            case "id_hoadon": newRow.CreateCell(_socot1).SetCellValue(item.id_hoadon); break;
                            case "tenkhachhang": newRow.CreateCell(_socot1).SetCellValue(item.tenkhachhang); break;
                            case "sdt": newRow.CreateCell(_socot1).SetCellValue(item.sdt); break;
                            case "tendvsp": newRow.CreateCell(_socot1).SetCellValue(item.tendvsp); break;

                            case "kyhieu": newRow.CreateCell(_socot1).SetCellValue(item.kyhieu); break;
                            case "id_thedichvu": newRow.CreateCell(_socot1).SetCellValue(item.id_thedichvu); break;

                            case "gia": newRow.CreateCell(_socot1).SetCellValue(item.gia.Value); break;
                            case "soluong": newRow.CreateCell(_socot1).SetCellValue(item.soluong.Value); break;
                            case "thanhtien": newRow.CreateCell(_socot1).SetCellValue(item.thanhtien.Value); break;
                            case "chietkhau": newRow.CreateCell(_socot1).SetCellValue(item.chietkhau.Value); break;
                            case "sauck": newRow.CreateCell(_socot1).SetCellValue(item.sauck.Value); break;
                            case "tennguoichot_xuat_excel": newRow.CreateCell(_socot1).SetCellValue(item.tennguoichot_xuat_excel); break;
                            case "phantramchot": newRow.CreateCell(_socot1).SetCellValue(item.phantramchot.Value); break;
                            case "tongtien_chot": newRow.CreateCell(_socot1).SetCellValue(item.tongtien_chot.Value); break;
                            case "tennguoilam_xuat_excel": newRow.CreateCell(_socot1).SetCellValue(item.tennguoilam_xuat_excel); break;
                            case "phantramlam": newRow.CreateCell(_socot1).SetCellValue(item.phantramlam.Value); break;
                            case "tongtien_lam": newRow.CreateCell(_socot1).SetCellValue(item.tongtien_lam); break;
                            default: break;
                        }
                        _socot1 = _socot1 + 1;
                    }
                }

                // tăng index
                rowIndex++;
            }

            //tính tổng
            // Ghi tiêu đề cột ở row 1
            var row3 = sheet.CreateRow(rowIndex);//tạo dòng tiếp theo
            int _socot3 = 0;
            for (int i = 0; i < check_list_excel.Items.Count; i++)//duyệt hết các phần tử trong list check
            {
                if (check_list_excel.Items[i].Selected)//nếu cột này đc chọn
                {
                    //thì ghi dữ liệu
                    string _tencot = check_list_excel.Items[i].Value;
                    switch (_tencot)
                    {
                        //case "tongchot": row3.CreateCell(_socot3).SetCellValue(tongchot); break;
                        //case "tonglam": row3.CreateCell(_socot3).SetCellValue(tonglam); break;
                        //case "tongbanthe": row3.CreateCell(_socot3).SetCellValue(tongbanthe); break;
                        //case "tongcong": row3.CreateCell(_socot3).SetCellValue(tongcong); break;
                        default: break;
                    }
                    _socot3 = _socot3 + 1;
                }
            }

            // xong hết thì save file lại
            string _filename = Guid.NewGuid().ToString();
            FileStream fs = new FileStream(Server.MapPath("~/uploads/Files/" + _filename + ".xlsx"), FileMode.CreateNew);
            wb.Write(fs);
            Response.Redirect("/uploads/Files/" + _filename + ".xlsx");

        }
    }
}
