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
    vattu_class vattu_cl = new vattu_class();
    datetime_class dt_cl = new datetime_class();
    nhaphang_class nh_cl = new nhaphang_class();
    phongban_class pb_cl = new phongban_class();
    #region phân trang
    public int stt = 1, current_page = 1, total_page = 1, show = 30;
    List<string> list_id_split;
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {

        #region Check_Login
        string _quyen = "q13_9";
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


        if (!IsPostBack)
        {
            ddl_loc1.DataSource = db.nhomvattu_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList();
            ddl_loc1.DataTextField = "tennhom";
            ddl_loc1.DataValueField = "id";
            ddl_loc1.DataBind();
            ddl_loc1.Items.Insert(0, new ListItem("Tất cả", "0"));

            ddl_loc2.DataSource = db.nhacungcap_nhaphang_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList();
            ddl_loc2.DataTextField = "ten";
            ddl_loc2.DataValueField = "id";
            ddl_loc2.DataBind();
            ddl_loc2.Items.Insert(0, new ListItem("Tất cả", "0"));

            ddl_loc3.DataSource = db.phongban_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList();
            ddl_loc3.DataTextField = "ten";
            ddl_loc3.DataValueField = "id";
            ddl_loc3.DataBind();
            ddl_loc3.Items.Insert(0, new ListItem("Tất cả", "0"));

            if (Session["current_page_qlvt"] == null)//lưu giữ trang hiện tại
                Session["current_page_qlvt"] = "1";

            if (Session["index_sapxep_qlvt"] == null)////lưu sắp xếp
            {
                DropDownList2.SelectedIndex = 5;
                Session["index_sapxep_qlvt"] = DropDownList2.SelectedIndex.ToString();
            }
            else
                DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_qlvt"].ToString());

            if (Session["search_qlvt"] != null)//lưu tìm kiếm
                txt_search.Text = Session["search_qlvt"].ToString();
            else
                Session["search_qlvt"] = txt_search.Text;

            if (Session["show_qlvt"] == null)//lưu số dòng mặc định
            {
                txt_show.Text = "30";
                Session["show_qlvt"] = txt_show.Text;
            }
            else
                txt_show.Text = Session["show_qlvt"].ToString();

            if (Session["tungay_qlvt"] == null)
            {
                txt_tungay.Text = "01/01/2023";
                Session["tungay_qlvt"] = txt_tungay.Text;
            }
            else
                txt_tungay.Text = Session["tungay_qlvt"].ToString();

            if (Session["denngay_qlvt"] == null)
            {
                txt_denngay.Text = DateTime.Now.ToShortDateString();
                Session["denngay_qlvt"] = txt_denngay.Text;
            }
            else
                txt_denngay.Text = Session["denngay_qlvt"].ToString();

            if (Session["index_loc_phanloai_qlvt"] != null)//lưu lọc theo nhóm, phân loại
            {
                ddl_loc1.SelectedIndex = int.Parse(Session["index_loc_phanloai_qlvt"].ToString());
            }
            else
                Session["index_loc_phanloai_qlvt"] = ddl_loc1.SelectedIndex.ToString();
            if (Session["index_loc_ncc_qlvt"] != null)//ncc
            {
                ddl_loc2.SelectedIndex = int.Parse(Session["index_loc_ncc_qlvt"].ToString());
            }
            else
                Session["index_loc_ncc_qlvt"] = ddl_loc2.SelectedIndex.ToString();
            if (Session["index_loc_phongban_qlvt"] != null)//phòng ban
            {
                ddl_loc3.SelectedIndex = int.Parse(Session["index_loc_phongban_qlvt"].ToString());
            }
            else
                Session["index_loc_phongban_qlvt"] = ddl_loc3.SelectedIndex.ToString();
        }
        main();
    }

    public void main()
    {
        //Intersect: lấy ra các phần tử mà cả 2 bên đều có (phần chung)
        #region lấy dữ liệu
        var list_all = (from ob1 in db.danhsach_vattu_tables.Where(p => p.id_nhom != "0" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                        join ob2 in db.nhomvattu_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on ob1.id_nhom equals ob2.id.ToString()
                        select new
                        {
                            id = ob1.id,
                            name = ob1.tenvattu,
                            tenmn = ob2.tennhom,
                            ngaytao = ob1.ngaytao,
                            nguoitao=ob1.nguoitao,
                            id_nhom=ob1.id_nhom,
                            ghichu=ob1.ghichu,
                            dvt_sp = ob1.donvitinh_sp,
                            image = ob1.image,
                            giasp = ob1.giaban,
                            giavon_sp=ob1.gianhap,
                            tinhtrang=ob1.tinhtrang,
                            vitriphongban=ob1.vitriphongban,
                            tenphongban= pb_cl.return_name(ob1.vitriphongban),
                            ncc = ob1.ncc,
                            tenncc=nh_cl.return_name_ncc(ob1.ncc),
                        }).ToList();

        var list_no = db.danhsach_vattu_tables.Where(p => p.id_nhom == "0" && p.id_chinhanh == Session["chinhanh"].ToString()).Select(ob1 => new
        {
            id = ob1.id,
            name = ob1.tenvattu,
            tenmn = "",
            ngaytao = ob1.ngaytao,
            nguoitao = ob1.nguoitao,
            id_nhom = "",
            ghichu = ob1.ghichu,
            dvt_sp = ob1.donvitinh_sp,
            image = ob1.image,
            giasp = ob1.giaban,
            giavon_sp = ob1.gianhap,
            tinhtrang = ob1.tinhtrang,
            vitriphongban = ob1.vitriphongban,
            tenphongban = pb_cl.return_name(ob1.vitriphongban),
            ncc = ob1.ncc,
            tenncc = nh_cl.return_name_ncc(ob1.ncc),
        });
        list_all = list_all.Union(list_no).ToList();

        //xử lý theo thời gian
        var list_time = list_all.Where(p => p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_qlvt"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_qlvt"].ToString()).Date);
        list_all = list_all.Intersect(list_time).ToList();

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.name.ToLower().Contains(_key) || p.id.ToString() == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }
        //xử lý trạng thái

        //LỌC DỮ LIỆU
        //if (DropDownList1.SelectedValue.ToString() != "0")
        //{
        //switch (DropDownList1.SelectedValue.ToString())
        //{
        //    case ("0"): var list_0 = list_all.Where(p => p.bin == false).ToList(); list_all = list_all.Intersect(list_0).ToList(); break;
        //    case ("1"): var list_1 = list_all.Where(p => p.bin == true).ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
        //    default: /*var list_2 = list_all.ToList(); list_all = list_all.Intersect(list_2).ToList();*/ break;
        //}
        //}

        if (ddl_loc1.SelectedValue.ToString() != "0")
        {
            var list_1 = list_all.Where(p => p.id_nhom == ddl_loc1.SelectedValue.ToString()).ToList();
            list_all = list_all.Intersect(list_1).ToList();
        }
        if (ddl_loc2.SelectedValue.ToString() != "0")
        {
            var list_1 = list_all.Where(p => p.ncc == ddl_loc2.SelectedValue.ToString()).ToList();
            list_all = list_all.Intersect(list_1).ToList();
        }
        if (ddl_loc3.SelectedValue.ToString() != "0")
        {
            var list_1 = list_all.Where(p => p.vitriphongban == ddl_loc3.SelectedValue.ToString()).ToList();
            list_all = list_all.Intersect(list_1).ToList();
        }



        switch (Session["index_sapxep_qlvt"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.name).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.name).ToList(); break;
            case ("2"): list_all = list_all.OrderBy(p => p.tenmn).ToList(); break;
            case ("3"): list_all = list_all.OrderByDescending(p => p.tenmn).ToList(); break;
            case ("4"): list_all = list_all.OrderBy(p => p.ngaytao).ToList(); break;
            case ("5"): list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
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
        current_page = int.Parse(Session["current_page_qlvt"].ToString());
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
 
    protected void but_del_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q13_9") == "2")
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
                    if (vattu_cl.exist_id(_id))
                    {

                        var q = db.danhsach_vattu_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                        danhsach_vattu_table _ob = q.First();
                        db.danhsach_vattu_tables.DeleteOnSubmit(_ob);
                        db.SubmitChanges();
                        _count = _count + 1;
                    }
                }
            }
            if (_count > 0)
            {
                //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xóa thành công.", "4000", "warning");
                //Response.Redirect("/gianhang/admin/quan-ly-bai-viet/Default.aspx");
                main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công.", "4000", "warning"), true);
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
        Session["current_page_qlvt"] = "1";

        main();
    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_qlvt"] = int.Parse(Session["current_page_qlvt"].ToString()) - 1;
        if (int.Parse(Session["current_page_qlvt"].ToString()) < 1)
            Session["current_page_qlvt"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_qlvt"] = int.Parse(Session["current_page_qlvt"].ToString()) + 1;
        if (int.Parse(Session["current_page_qlvt"].ToString()) > total_page)
            Session["current_page_qlvt"] = total_page;
        main();
    }
    #endregion


    protected void Button1_Click(object sender, EventArgs e)
    {
        Session["index_sapxep_qlvt"] = DropDownList2.SelectedIndex;
        Session["current_page_qlvt"] = "1";
        Session["search_qlvt"] = txt_search.Text.Trim();
        Session["show_qlvt"] = txt_show.Text.Trim();
        Session["tungay_qlvt"] = txt_tungay.Text;
        Session["denngay_qlvt"] = txt_denngay.Text;
        Session["index_loc_phanloai_qlvt"] = ddl_loc1.SelectedIndex;
        Session["index_loc_ncc_qlvt"] = ddl_loc2.SelectedIndex;
        Session["index_loc_phongban_qlvt"] = ddl_loc3.SelectedIndex;
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        Session["index_sapxep_qlvt"] = null;
        Session["current_page_qlvt"] = null;
        Session["search_qlvt"] = null;
        Session["show_qlvt"] = null;
        Session["tungay_qlvt"] = null;
        Session["denngay_qlvt"] = null;
        Session["index_loc_phanloai_qlvt"] = null;
        Session["index_loc_ncc_qlvt"] = null;
        Session["index_loc_phongban_qlvt"] = null;
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-vat-tu/Default.aspx");
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