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
    nganh_class nganh_cl = new nganh_class();

    private void EnsureLegacyAdminSession()
    {
        if (Session == null)
            return;

        string legacyUser = (Session["user"] ?? "").ToString().Trim();
        if (legacyUser != "")
            return;

        string homeAccount;
        string deniedMessage;
        GianHangAdminBridge_cl.EnsureLegacyAdminSessionFromCurrentHome(db, out homeAccount, out deniedMessage);
    }

    #region phân trang
    public int stt = 1, current_page = 1, total_page = 1, show = 30;
    List<string> list_id_split;
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        EnsureLegacyAdminSession();

        #region Check_Login
        string _quyen = "q4_1";
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
            
            var list_nganh = (from ob1 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                              select new { id = ob1.id, ten = ob1.ten, }
                               );
            DropDownList5.DataSource = list_nganh;
            DropDownList5.DataTextField = "ten";
            DropDownList5.DataValueField = "id";
            DropDownList5.DataBind();
            DropDownList5.Items.Insert(0, new ListItem("Tất cả", ""));

            if (Session["index_loc_nganh_baiviet"] != null)//lưu lọc theo theo toán
                DropDownList5.SelectedIndex = int.Parse(Session["index_loc_nganh_baiviet"].ToString());
            else
                Session["index_loc_nganh_baiviet"] = DropDownList5.SelectedIndex.ToString();

            if (Session["current_page_baiviet"] == null)//lưu giữ trang hiện tại
                Session["current_page_baiviet"] = "1";

            if (Session["index_sapxep_baiviet"] == null)////lưu sắp xếp
            {
                DropDownList2.SelectedIndex = 5;
                Session["index_sapxep_baiviet"] = DropDownList2.SelectedIndex.ToString();
            }
            else
                DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_baiviet"].ToString());

            if (Session["search_baiviet"] != null)//lưu tìm kiếm
                txt_search.Text = Session["search_baiviet"].ToString();
            else
                Session["search_baiviet"] = txt_search.Text;

            if (Session["index_trangthai_baiviet"] != null)//lưu lọc trạng thái
                DropDownList1.SelectedIndex = int.Parse(Session["index_trangthai_baiviet"].ToString());
            else
                Session["index_trangthai_baiviet"] = DropDownList1.SelectedValue.ToString();

            if (Session["show_baiviet"] == null)//lưu số dòng mặc định
            {
                txt_show.Text = "30";
                Session["show_baiviet"] = txt_show.Text;
            }
            else
                txt_show.Text = Session["show_baiviet"].ToString();

            if (Session["tungay_baiviet"] == null)
            {
                txt_tungay.Text = q.Count() != 0 ? q.Min(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                Session["tungay_baiviet"] = txt_tungay.Text;
            }
            else
                txt_tungay.Text = Session["tungay_baiviet"].ToString();

            if (Session["denngay_baiviet"] == null)
            {
                txt_denngay.Text = q.Count() != 0 ? q.Max(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                Session["denngay_baiviet"] = txt_denngay.Text;
            }
            else
                txt_denngay.Text = Session["denngay_baiviet"].ToString();

            if (Session["index_loc_phanloai_baiviet"] != null)//lưu lọc theo phân loại
            {
                DropDownList3.SelectedIndex = int.Parse(Session["index_loc_phanloai_baiviet"].ToString());
                //Response.Write(Session["index_loc_phanloai_baiviet"].ToString());
            }
            else
                Session["index_loc_phanloai_baiviet"] = DropDownList3.SelectedIndex.ToString();

            if (!string.IsNullOrWhiteSpace(Request.QueryString["pl"]))//lọc nhanh sp hoặc dv
            {
                string _pl = Request.QueryString["pl"].ToString().Trim();
                if (_pl == "dv")
                {
                    DropDownList3.SelectedIndex = 3;
                    Session["index_loc_phanloai_baiviet"] = 3;
                }
                else
                {
                    if (_pl == "sp")
                    {
                        DropDownList3.SelectedIndex = 2;
                        Session["index_loc_phanloai_baiviet"] = 2;
                    }
                }
            }
            else
            {
                DropDownList3.SelectedIndex = 1;
                Session["index_loc_phanloai_baiviet"] = 1;
            }
        }
        main();
    }

    public void main()
    {
        //Intersect: lấy ra các phần tử mà cả 2 bên đều có (phần chung)
        #region lấy dữ liệu
        var list_all = (from bv in db.web_post_tables.Where(p => p.id_category != "0" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                        join mn in db.web_menu_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on bv.id_category equals mn.id.ToString()
                        select new
                        {
                            id = bv.id,
                            name = bv.name,
                            name_en = bv.name_en,
                            tenmn = mn.name,
                            noibat = bv.noibat,
                            ngaytao = bv.ngaytao,
                            bin = bv.bin,
                            phanloai = bv.phanloai,
                            giadv = bv.giaban_dichvu,
                            giasp = bv.giaban_sanpham,
                            image = bv.image,
                            phantram_chotsale_sanpham=bv.phantram_chotsale_sanpham,
                            //soluong_ton_sanpham = bv.soluong_ton_sanpham,
                            phantram_lamdichvu=bv.phantram_lamdichvu,
                            phantram_chotsale_dichvu=bv.phantram_chotsale_dichvu,
                            giavon_sp=bv.giavon_sanpham,
                            dvt_sp=bv.donvitinh_sp,
                            hienthi = bv.hienthi,
                            id_nganh = bv.id_nganh,
                            tennganh = nganh_cl.return_name(bv.id_nganh),
                        }).ToList();

        var list_no = db.web_post_tables.Where(p => p.id_category == "0"&& p.id_chinhanh == Session["chinhanh"].ToString()).Select(p => new
        {
            id = p.id,
            name = p.name,
            name_en = p.name_en,
            tenmn = "",
            noibat = p.noibat,
            ngaytao = p.ngaytao,
            bin = p.bin,
            phanloai = p.phanloai,
            giadv = p.giaban_dichvu,
            giasp = p.giaban_sanpham,
            image = p.image,
            phantram_chotsale_sanpham = p.phantram_chotsale_sanpham,
            //soluong_ton_sanpham = p.soluong_ton_sanpham,
            phantram_lamdichvu = p.phantram_lamdichvu,
            phantram_chotsale_dichvu = p.phantram_chotsale_dichvu,
            giavon_sp = p.giavon_sanpham,
            dvt_sp = p.donvitinh_sp,
            hienthi = p.hienthi,
            id_nganh = p.id_nganh,
            tennganh = nganh_cl.return_name(p.id_nganh),
        });
        list_all = list_all.Union(list_no).ToList();

        //xử lý theo thời gian
        var list_time = list_all.Where(p => p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_baiviet"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_baiviet"].ToString()).Date);
        list_all = list_all.Intersect(list_time).ToList();

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.name.ToLower().Contains(_key) || p.name_en.ToLower().Contains(_key) || p.id.ToString() == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }
        //xử lý trạng thái

        //LỌC DỮ LIỆU
        //if (DropDownList1.SelectedValue.ToString() != "0")
        //{
        switch (DropDownList1.SelectedValue.ToString())
        {
            case ("0"): var list_0 = list_all.Where(p => p.bin == false).ToList(); list_all = list_all.Intersect(list_0).ToList(); break;
            case ("1"): var list_1 = list_all.Where(p => p.bin == true).ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
            default: /*var list_2 = list_all.ToList(); list_all = list_all.Intersect(list_2).ToList();*/ break;
        }
        //}

        if (DropDownList3.SelectedValue.ToString() != "0")
        {
            switch (DropDownList3.SelectedValue.ToString())
            {
                case ("ctbv"): var list_1 = list_all.Where(p => p.phanloai =="ctbv").ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
                case ("ctsp"): var list_2 = list_all.Where(p => p.phanloai == "ctsp").ToList(); list_all = list_all.Intersect(list_2).ToList(); break;
                case ("ctdv"): var list_3 = list_all.Where(p => p.phanloai == "ctdv").ToList(); list_all = list_all.Intersect(list_3).ToList(); break;
                default: break;
            }
        }

        if (DropDownList5.SelectedValue.ToString() != "")//ngành
        {
            var list_1 = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList();
            list_all = list_all.Intersect(list_1).ToList();
        }


        switch (Session["index_sapxep_baiviet"].ToString())
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
        current_page = int.Parse(Session["current_page_baiviet"].ToString());
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
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q4_3") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {
            for (int i = 0; i < list_id_split.Count; i++)
            {
                string _id = list_id_split[i].Replace("check_", "");
                string _noibat = Request.Form["noibat_" + _id];
                string _hienthi = Request.Form["hienthi_" + _id];
                var q = db.web_post_tables.Where(p => p.id.ToString() == _id);
                if (po_cl.exist_id(_id))
                {
                    web_post_table _ob = q.First();
                    if (_noibat == "on")
                        _ob.noibat = true;
                    else
                        _ob.noibat = false;
                    if (_hienthi == "on")
                        _ob.hienthi = true;
                    else
                        _ob.hienthi = false;
                    db.SubmitChanges();
                }
            }
            //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Lưu thành công.", "4000", "warning");
            //Response.Redirect("/gianhang/admin/quan-ly-menu/Default.aspx");
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lưu thành công.", "4000", "warning"), true);
        }
    }
    protected void but_del_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q4_4") == "2")
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
                    if (po_cl.exist_id(_id))
                    {
                        //po_cl.change_status_bin(_id, true);

                        var q = db.web_post_tables.Where(p => p.id.ToString() == _id);
                        web_post_table _ob = q.First();
                        _ob.bin = true;
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
    protected void but_xoavinhvien_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q4_5") == "2")
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
                    if (po_cl.exist_id(_id))
                    {
                        po_cl.del(_id);
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
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q4_6") == "2")
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
                    if (po_cl.exist_id(_id))
                    {
                        var q = db.web_post_tables.Where(p => p.id.ToString() == _id);
                        web_post_table _ob = q.First();
                        _ob.bin = false;
                        db.SubmitChanges();
                        _count = _count + 1;
                    }
                }
            }
            if (_count > 0)
            {
                //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Khôi phục thành công.", "4000", "warning");
                //Response.Redirect("/gianhang/admin/quan-ly-bai-viet/Default.aspx");
                main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Khôi phục thành công.", "4000", "warning"), true);
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
        Session["current_page_baiviet"] = "1";

        main();
    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_baiviet"] = int.Parse(Session["current_page_baiviet"].ToString()) - 1;
        if (int.Parse(Session["current_page_baiviet"].ToString()) < 1)
            Session["current_page_baiviet"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_baiviet"] = int.Parse(Session["current_page_baiviet"].ToString()) + 1;
        if (int.Parse(Session["current_page_baiviet"].ToString()) > total_page)
            Session["current_page_baiviet"] = total_page;
        main();
    }
    #endregion


    protected void Button1_Click(object sender, EventArgs e)
    {
        Session["index_trangthai_baiviet"] = DropDownList1.SelectedIndex;
        Session["index_sapxep_baiviet"] = DropDownList2.SelectedIndex;
        Session["current_page_baiviet"] = "1";
        Session["search_baiviet"] = txt_search.Text.Trim();
        Session["show_baiviet"] = txt_show.Text.Trim();
        Session["tungay_baiviet"] = txt_tungay.Text;
        Session["denngay_baiviet"] = txt_denngay.Text;
        Session["index_loc_phanloai_baiviet"] = DropDownList3.SelectedIndex;

        Session["index_loc_nganh_baiviet"] = DropDownList5.SelectedIndex;
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        Session["index_trangthai_baiviet"] = null;
        Session["index_sapxep_baiviet"] = null;
        Session["current_page_baiviet"] = null;
        Session["search_baiviet"] = null;
        Session["show_baiviet"] = null;
        Session["tungay_baiviet"] = null;
        Session["denngay_baiviet"] = null;
        Session["index_loc_phanloai_baiviet"] = null;

        Session["index_loc_nganh_baiviet"] = null;
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-bai-viet/Default.aspx");
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
        var list_all = (from bv in db.web_post_tables.Where(p => p.id_category != "0" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                        join mn in db.web_menu_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on bv.id_category equals mn.id.ToString()
                        select new
                        {
                            id = bv.id,
                            name = bv.name,
                            name_en = bv.name_en,
                            tenmn = mn.name,
                            noibat = bv.noibat,
                            ngaytao = bv.ngaytao,
                            bin = bv.bin,
                            phanloai = bv.phanloai,
                            giadv = bv.giaban_dichvu,
                            giasp = bv.giaban_sanpham,
                            image = bv.image,
                            phantram_chotsale_sanpham = bv.phantram_chotsale_sanpham,
                            //soluong_ton_sanpham = bv.soluong_ton_sanpham,
                            phantram_lamdichvu = bv.phantram_lamdichvu,
                            phantram_chotsale_dichvu = bv.phantram_chotsale_dichvu,
                            giavon_sp = bv.giavon_sanpham,
                            dvt_sp = bv.donvitinh_sp,
                            hienthi = bv.hienthi,
                            id_nganh = bv.id_nganh,
                            tennganh = nganh_cl.return_name(bv.id_nganh),
                        }).ToList();

        var list_no = db.web_post_tables.Where(p => p.id_category == "0" && p.id_chinhanh == Session["chinhanh"].ToString()).Select(p => new
        {
            id = p.id,
            name = p.name,
            name_en = p.name_en,
            tenmn = "",
            noibat = p.noibat,
            ngaytao = p.ngaytao,
            bin = p.bin,
            phanloai = p.phanloai,
            giadv = p.giaban_dichvu,
            giasp = p.giaban_sanpham,
            image = p.image,
            phantram_chotsale_sanpham = p.phantram_chotsale_sanpham,
            //soluong_ton_sanpham = p.soluong_ton_sanpham,
            phantram_lamdichvu = p.phantram_lamdichvu,
            phantram_chotsale_dichvu = p.phantram_chotsale_dichvu,
            giavon_sp = p.giavon_sanpham,
            dvt_sp = p.donvitinh_sp,
            hienthi = p.hienthi,
            id_nganh = p.id_nganh,
            tennganh = nganh_cl.return_name(p.id_nganh),
        });
        list_all = list_all.Union(list_no).ToList();

        //xử lý theo thời gian
        var list_time = list_all.Where(p => p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_baiviet"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_baiviet"].ToString()).Date);
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
        switch (Session["index_sapxep_baiviet"].ToString())
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
            string _filetenbv = Guid.NewGuid().ToString();
            FileStream fs = new FileStream(Server.MapPath("~/uploads/Files/" + _filetenbv + ".xlsx"), FileMode.CreateNew);
            wb.Write(fs);
            Response.Redirect("/uploads/Files/" + _filetenbv + ".xlsx");
        }
    }
}
