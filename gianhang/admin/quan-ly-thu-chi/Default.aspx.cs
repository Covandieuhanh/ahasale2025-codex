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
    thuchi_class tc_cl = new thuchi_class();
    datetime_class dt_cl = new datetime_class(); nganh_class ng_cl = new nganh_class();
    public Int64 _tongthu_giuaky = 0, _tongchi_giuaky = 0, _tongthuchi = 0, _tongchuaduyet = 0;

    public string user, user_parent, tondauky, toncuoiky;
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
        if (bcorn_class.check_quyen(user, "q9_1") == "" || bcorn_class.check_quyen(user, "n9_1") == "")
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

                if (bcorn_class.check_quyen(user, "q9_3") == "")//neu la quyen cap chi nhanh
                {
                    ddl_loc1.DataSource = db.bspa_nhomthuchi_tables.Where(p => p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.tennhom).ToList();
                    ddl_loc1.DataTextField = "tennhom";
                    ddl_loc1.DataValueField = "id";
                    ddl_loc1.DataBind();
                    ddl_loc1.Items.Insert(0, new ListItem("Tất cả", "0"));
                }
                else//neu la quyen cap nganh
                {
                    ddl_loc1.DataSource = db.bspa_nhomthuchi_tables.Where(p => p.user_parent == user_parent && p.id_nganh == Session["nganh"].ToString() && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.tennhom).ToList();
                    ddl_loc1.DataTextField = "tennhom";
                    ddl_loc1.DataValueField = "id";
                    ddl_loc1.DataBind();
                    ddl_loc1.Items.Insert(0, new ListItem("Tất cả", "0"));

                    DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList5.Enabled = false;
                }
                if (Session["index_loc_nganh_thuchi"] != null)//lưu lọc theo theo toán
                    DropDownList5.SelectedIndex = int.Parse(Session["index_loc_nganh_thuchi"].ToString());
                else
                    Session["index_loc_nganh_thuchi"] = DropDownList5.SelectedIndex.ToString();


                var q = db.bspa_thuchi_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString());

                if (Session["current_page_thuchi"] == null)//lưu giữ trang hiện tại
                    Session["current_page_thuchi"] = "1";

                if (Session["index_sapxep_thuchi"] == null)////lưu sắp xếp
                {
                    DropDownList2.SelectedIndex = 1;//mặc định là giảm dần theo thời gian
                    Session["index_sapxep_thuchi"] = DropDownList2.SelectedIndex.ToString();
                }
                else
                    DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_thuchi"].ToString());

                if (Session["search_thuchi"] != null)//lưu tìm kiếm
                    txt_search.Text = Session["search_thuchi"].ToString();
                else
                    Session["search_thuchi"] = txt_search.Text;

                if (Session["show_thuchi"] == null)//lưu số dòng mặc định
                {
                    txt_show.Text = "30";
                    Session["show_thuchi"] = txt_show.Text;
                }
                else
                    txt_show.Text = Session["show_thuchi"].ToString();

                if (Session["tungay_thuchi"] == null)
                {
                    txt_tungay.Text = q.Count() != 0 ? q.Min(p => p.ngay.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                    Session["tungay_thuchi"] = txt_tungay.Text;
                }
                else
                    txt_tungay.Text = Session["tungay_thuchi"].ToString();

                if (Session["denngay_thuchi"] == null)
                {
                    txt_denngay.Text = q.Count() != 0 ? q.Max(p => p.ngay.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                    Session["denngay_thuchi"] = txt_denngay.Text;
                }
                else
                    txt_denngay.Text = Session["denngay_thuchi"].ToString();

                if (Session["index_locnhom_thuchi"] != null)//lưu lọc theo nhóm
                    ddl_loc1.SelectedIndex = int.Parse(Session["index_locnhom_thuchi"].ToString());
                else
                    Session["index_locnhom_thuchi"] = ddl_loc1.SelectedIndex.ToString();
                if (Session["index_loc_duyetchi"] != null)//lưu lọc theo phiếu đã duyệt hay chưa
                    DropDownList1.SelectedIndex = int.Parse(Session["index_loc_duyetchi"].ToString());
                else
                    Session["index_loc_duyetchi"] = DropDownList1.SelectedIndex.ToString();

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

    public string get_hinhanh(string _id_thuchi)
    {
        string _kq;
        var q = db.bspa_hinhanhthuchi_tables.Where(p => p.user_parent == user_parent && p.id_thuchi == _id_thuchi && p.id_chinhanh == Session["chinhanh"].ToString());
        if (q.Count() != 0)
        {
            if (q.Count() == 1)
            {
                _kq = "<img src='" + q.First().hinhanh + "' data-original='" + q.First().hinhanh + "' class='img-cover-vuong w-h-50' style='max-width: none!important'";
                return _kq;
            }
            else
            {
                string _kq2 = "<img src='" + q.First().hinhanh + "' data-original='" + q.First().hinhanh + "' class='img-cover-vuong w-h-50' style='max-width: none!important'";
                foreach (var t in q)
                {
                    _kq2 = _kq2 + "<img class='d-none' src='" + t.hinhanh + "'>";
                }
                return _kq2;
            }
        }
        return "";
    }
    public void main()
    {
        //lấy dữ liệu
        var list_all = (from ob1 in db.bspa_thuchi_tables.Where(p => p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString() && p.id_nhomthuchi != "" && p.ngay.Value.Date >= DateTime.Parse(Session["tungay_thuchi"].ToString()).Date && p.ngay.Value.Date <= DateTime.Parse(Session["denngay_thuchi"].ToString()).Date).ToList()
                        join ob2 in db.bspa_nhomthuchi_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on ob1.id_nhomthuchi equals ob2.id.ToString()
                        select new
                        {
                            id = ob1.id,
                            nhom = ob2.tennhom,

                            nguoilapphieu = ob1.nguoilapphieu,
                            nguoinhan = tk_cl.exist_user_of_userparent(ob1.nguoinhantien, user_parent) == false ? ob1.nguoinhantien : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nguoinhantien + "'>" + tk_cl.return_object(ob1.nguoinhantien).hoten + "</a></div>",
                            thuchi = ob1.thuchi,
                            ngay = ob1.ngay,
                            noidung = ob1.noidung,
                            sotien = ob1.sotien,
                            hinhanh = get_hinhanh(ob1.id.ToString()),
                            duyet_chi = ob1.duyet_phieuchi,
                            nguoi_duyet_huy = ob1.nguoihuy_duyet_phieuchi,
                            thoigian_duyet_huy = ob1.thoigian_huyduyet_phieuchi,
                            id_nganh = ob1.id_nganh,
                            is_auto = HoaDonThuChiSync_cl.IsAutoSystemEntry(ob1),
                            auto_source = HoaDonThuChiSync_cl.ResolveSourceLabel(ob1),
                            auto_link = HoaDonThuChiSync_cl.ResolveLinkedUrl(ob1),
                            check_attr = HoaDonThuChiSync_cl.IsAutoSystemEntry(ob1) ? "disabled='disabled'" : "",
                            row_class = HoaDonThuChiSync_cl.IsAutoSystemEntry(ob1) ? "thuchi-auto-row" : ""
                        });
        var list_no = (from ob1 in db.bspa_thuchi_tables.Where(p => p.id_nhomthuchi == "" && p.id_chinhanh == Session["chinhanh"].ToString() && p.user_parent == user_parent && p.ngay.Value.Date >= DateTime.Parse(Session["tungay_thuchi"].ToString()).Date && p.ngay.Value.Date <= DateTime.Parse(Session["denngay_thuchi"].ToString()).Date).ToList()
                       select new
                       {
                           id = ob1.id,
                           nhom = "",

                           nguoilapphieu = ob1.nguoilapphieu,
                           nguoinhan = tk_cl.exist_user_of_userparent(ob1.nguoinhantien, user_parent) == false ? ob1.nguoinhantien : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nguoinhantien + "'>" + tk_cl.return_object(ob1.nguoinhantien).hoten + "</a></div>",
                           thuchi = ob1.thuchi,
                           ngay = ob1.ngay,
                           noidung = ob1.noidung,
                           sotien = ob1.sotien,
                           hinhanh = get_hinhanh(ob1.id.ToString()),
                           duyet_chi = ob1.duyet_phieuchi,
                           nguoi_duyet_huy = ob1.nguoihuy_duyet_phieuchi,
                           thoigian_duyet_huy = ob1.thoigian_huyduyet_phieuchi,
                           id_nganh = ob1.id_nganh,
                           is_auto = HoaDonThuChiSync_cl.IsAutoSystemEntry(ob1),
                           auto_source = HoaDonThuChiSync_cl.ResolveSourceLabel(ob1),
                           auto_link = HoaDonThuChiSync_cl.ResolveLinkedUrl(ob1),
                           check_attr = HoaDonThuChiSync_cl.IsAutoSystemEntry(ob1) ? "disabled='disabled'" : "",
                           row_class = HoaDonThuChiSync_cl.IsAutoSystemEntry(ob1) ? "thuchi-auto-row" : ""
                       }).ToList();
        list_all = list_all.Union(list_no).ToList();

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.noidung.ToLower().Contains(_key)).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        //xử lý lọc dữ liệu
        if (ddl_loc1.SelectedValue.ToString() != "0")
        {
            var list_1 = list_all.Where(p => p.nhom == ddl_loc1.SelectedItem.ToString()).ToList();
            list_all = list_all.Intersect(list_1).ToList();
        }
        if (DropDownList1.SelectedValue.ToString() != "0")
        {
            var list_1 = list_all.Where(p => p.duyet_chi.ToString() == DropDownList1.SelectedValue.ToString()).ToList();
            list_all = list_all.Intersect(list_1).ToList();
        }

        if (DropDownList5.SelectedValue.ToString() != "")//ngành
        {
            var list_1 = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList(); list_all = list_all.Intersect(list_1).ToList();
        }

        Int64 _tongthu_dauky = 0, _tongchi_dauky = 0, _tongthu_cuoiky = 0, _tongchi_cuoiky = 0, _tondauky = 0;
        //tính tồn đầu tồn cuối
        var q_tondau_thu = db.bspa_thuchi_tables.Where(p => p.thuchi == "Thu" && p.id_chinhanh == Session["chinhanh"].ToString() && p.duyet_phieuchi == "Đã duyệt" && p.user_parent == user_parent && p.ngay.Value.Date < DateTime.Parse(Session["tungay_thuchi"].ToString()).Date);
        if (q_tondau_thu.Count() != 0)
            _tongthu_dauky = q_tondau_thu.Sum(p => p.sotien).Value;

        var q_tondau_chi = db.bspa_thuchi_tables.Where(p => p.thuchi == "Chi" && p.id_chinhanh == Session["chinhanh"].ToString() && p.duyet_phieuchi == "Đã duyệt" && p.user_parent == user_parent && p.ngay.Value.Date < DateTime.Parse(Session["tungay_thuchi"].ToString()).Date);
        if (q_tondau_chi.Count() != 0)
            _tongchi_dauky = q_tondau_chi.Sum(p => p.sotien).Value;

        _tongthu_giuaky = list_all.Where(p => p.thuchi == "Thu" && p.duyet_chi == "Đã duyệt").Sum(p => p.sotien).Value;
        _tongchi_giuaky = list_all.Where(p => p.thuchi == "Chi" && p.duyet_chi == "Đã duyệt").Sum(p => p.sotien).Value;
        _tongthuchi = _tongthu_giuaky - _tongchi_giuaky;
        _tongchuaduyet = list_all.Where(p => p.thuchi == "Thu" && p.duyet_chi != "Đã duyệt").Sum(p => p.sotien).Value - list_all.Where(p => p.thuchi == "Chi" && p.duyet_chi != "Đã duyệt").Sum(p => p.sotien).Value;
        _tongthu_cuoiky = _tongthu_dauky + _tongthu_giuaky;
        _tongchi_cuoiky = _tongchi_dauky + _tongchi_giuaky;
        _tondauky = _tongthu_dauky - _tongchi_dauky;
        tondauky = _tondauky.ToString("#,##0");
        toncuoiky = (_tongthu_giuaky - _tongchi_giuaky + _tondauky).ToString("#,##0");

        //sắp xếp
        switch (Session["index_sapxep_thuchi"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.ngay).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.ngay).ToList(); break;
            default: list_all = list_all.OrderByDescending(p => p.ngay).ToList(); break;
        }

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 50;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_thuchi"].ToString());
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

    protected void but_xoa_Click(object sender, ImageClickEventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q9_4") == "")
        {
            bool _skip_auto = false;
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    var q = db.bspa_thuchi_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (tc_cl.exist_id(_id, user_parent))
                    {
                        if (HoaDonThuChiSync_cl.IsAutoSystemEntry(q.First()))
                        {
                            _skip_auto = true;
                            continue;
                        }
                        if (q.First().duyet_phieuchi == "Đã duyệt" && q.First().thuchi == "Chi")
                        {
                            main();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không thể xóa các phiếu chi đã duyệt.", "4000", "warning"), true);
                        }
                        else
                        {
                            //xóa hình ảnh thu chi
                            var q_anh = db.bspa_hinhanhthuchi_tables.Where(p => p.id_thuchi == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                            foreach (var t in q_anh)
                            {
                                bspa_hinhanhthuchi_table _ob1 = t;
                                string _linkanh = _ob1.hinhanh;
                                if (File.Exists(Server.MapPath("~" + _linkanh)))
                                    File.Delete(Server.MapPath("~" + _linkanh));
                                db.bspa_hinhanhthuchi_tables.DeleteOnSubmit(_ob1);
                                db.SubmitChanges();
                            }

                            tc_cl.del(_id);
                        }
                    }
                }
            }
            main();
            if (_skip_auto)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Một số phiếu tự động đã được bỏ qua.", "4000", "warning"), true);
            else
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "warning"), true);
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }

    protected void but_duyetchi_Click1(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q9_6") == ""|| bcorn_class.check_quyen(user, "n9_6") == "")
        {
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    var q = db.bspa_thuchi_tables.Where(p => p.id.ToString() == _id && p.thuchi == "Chi" && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (tc_cl.exist_id(_id, user_parent))
                    {
                        bspa_thuchi_table _ob = q.First();
                        if (HoaDonThuChiSync_cl.IsAutoSystemEntry(_ob))
                            continue;
                        if (_ob.chophep_duyetvahuy_phieuchi == true)//nếu đang còn cho phép duyệt hoặc hủy, thì chuyển sang false --> khóa lại, k cho duyệt hoặc hủy nữa
                        {
                            _ob.duyet_phieuchi = "Đã duyệt";
                            _ob.nguoihuy_duyet_phieuchi = user;
                            _ob.thoigian_huyduyet_phieuchi = DateTime.Now;
                            _ob.chophep_duyetvahuy_phieuchi = false;//khóa hủy hoặc duyệt
                            db.SubmitChanges();
                        }
                    }
                }
            }
            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Đã duyệt các phiếu chi hợp lệ.", "4000", "warning");
            Response.Redirect("/gianhang/admin/quan-ly-thu-chi/Default.aspx");
            //main();
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Duyệt phiếu chi thành công.", "4000", "warning"), true);
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }
    protected void but_huyduyetchi_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q9_6") == "" || bcorn_class.check_quyen(user, "n9_6") == "")
        {
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    var q = db.bspa_thuchi_tables.Where(p => p.id.ToString() == _id && p.thuchi == "Chi" && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (tc_cl.exist_id(_id, user_parent))
                    {
                        bspa_thuchi_table _ob = q.First();
                        if (HoaDonThuChiSync_cl.IsAutoSystemEntry(_ob))
                            continue;
                        if (_ob.chophep_duyetvahuy_phieuchi == true)//nếu đang còn cho phép duyệt hoặc hủy, thì chuyển sang false --> khóa lại, k cho duyệt hoặc hủy nữa
                        {
                            _ob.duyet_phieuchi = "Hủy duyệt";
                            _ob.nguoihuy_duyet_phieuchi = user;
                            _ob.thoigian_huyduyet_phieuchi = DateTime.Now;
                            _ob.chophep_duyetvahuy_phieuchi = false;//khóa hủy hoặc duyệt
                            db.SubmitChanges();
                        }
                    }
                }
            }
            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Đã hủy duyệt các phiếu chi hợp lệ.", "4000", "warning");
            Response.Redirect("/gianhang/admin/quan-ly-thu-chi/Default.aspx");
            //main();
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Duyệt phiếu chi thành công.", "4000", "warning"), true);
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
        Session["index_sapxep_thuchi"] = null;
        Session["current_page_thuchi"] = null;
        Session["search_thuchi"] = null;
        Session["show_thuchi"] = null;
        Session["tungay_thuchi"] = null;
        Session["denngay_thuchi"] = null;
        Session["index_locnhom_thuchi"] = null;
        Session["index_loc_duyetchi"] = null;
        Session["index_loc_nganh_thuchi"] = null;
    }
    protected void Button2_Click(object sender, EventArgs e)//reset
    {
        reset_ss();
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-thu-chi/Default.aspx");
    }

    protected void Button1_Click(object sender, EventArgs e)//but lọc
    {
        Session["index_sapxep_thuchi"] = DropDownList2.SelectedIndex;
        Session["current_page_thuchi"] = "1";
        Session["search_thuchi"] = txt_search.Text.Trim();
        Session["show_thuchi"] = txt_show.Text.Trim();

        Session["tungay_thuchi"] = txt_tungay.Text;
        Session["denngay_thuchi"] = txt_denngay.Text;

        Session["index_locnhom_thuchi"] = ddl_loc1.SelectedIndex;
        Session["index_loc_duyetchi"] = DropDownList1.SelectedIndex;
        Session["index_loc_nganh_thuchi"] = DropDownList5.SelectedIndex.ToString();

        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
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
        Session["current_page_thuchi"] = "1";

        main();
    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_thuchi"] = int.Parse(Session["current_page_thuchi"].ToString()) - 1;
        if (int.Parse(Session["current_page_thuchi"].ToString()) < 1)
            Session["current_page_thuchi"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_thuchi"] = int.Parse(Session["current_page_thuchi"].ToString()) + 1;
        if (int.Parse(Session["current_page_thuchi"].ToString()) > total_page)
            Session["current_page_thuchi"] = total_page;
        main();
    }


}
