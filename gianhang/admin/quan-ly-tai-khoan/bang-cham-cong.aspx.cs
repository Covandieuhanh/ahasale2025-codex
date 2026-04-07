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
    datetime_class dt_cl = new datetime_class();
    public string user = "", user_parent = "", notifi, thongbao_chamcong_homnay, t, n;
    public int tongsongay_trongthang;
    public string[] arr_nhanvien;
    nganh_class ng_cl = new nganh_class();
    #region phân trang
    public int stt = 1, current_page = 1, show = 50, total_page = 1;
    List<string> list_id_split;
    #endregion

    private bool HasAnyPermission(params string[] permissionKeys)
    {
        string currentUser = (user ?? "").Trim();
        if (string.IsNullOrEmpty(currentUser))
            return false;

        foreach (string permissionKey in permissionKeys)
        {
            if (!string.IsNullOrEmpty(permissionKey) && bcorn_class.check_quyen(currentUser, permissionKey) == "")
                return true;
        }

        return false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        #region Check quyen theo nganh
        user = (access.User ?? "").Trim();
        user_parent = access.OwnerAccountKey;
        if (HasAnyPermission("q2_7", "n2_7"))
        {
            tongsongay_trongthang = dt_cl.return_ngaycuoithang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).Day;
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
                if (HasAnyPermission("q2_7"))
                {
                }
                else
                {
                    DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList5.Enabled = false;
                }

                if (Session["current_page_bangchamcong"] == null)//lưu giữ trang hiện tại
                    Session["current_page_bangchamcong"] = "1";

                if (Session["search_bangchamcong"] != null)//lưu tìm kiếm
                    txt_search.Text = Session["search_bangchamcong"].ToString();
                else
                    Session["search_bangchamcong"] = txt_search.Text;

                if (Session["index_loc_nganh_bangchamcong"] != null)//lưu lọc theo theo toán
                    DropDownList5.SelectedIndex = int.Parse(Session["index_loc_nganh_bangchamcong"].ToString());
                else
                    Session["index_loc_nganh_bangchamcong"] = DropDownList5.SelectedIndex.ToString();

                if (Session["show_bangchamcong"] == null)//lưu số dòng mặc định
                {
                    txt_show.Text = "30";
                    Session["show_bangchamcong"] = txt_show.Text;
                }
                else
                    txt_show.Text = Session["show_bangchamcong"].ToString();

                for (int i = 12; i >= 1; i--)
                    ddl_thang.Items.Insert(0, new ListItem("Tháng " + i, i.ToString()));

                if (Session["thang_bangchamcong"] == null)
                {
                    Session["thang_bangchamcong"] = DateTime.Now.Month;
                    ddl_thang.SelectedIndex = DateTime.Now.Month - 1;
                }
                else
                {
                    ddl_thang.SelectedIndex = int.Parse(Session["thang_bangchamcong"].ToString()) - 1;
                }

                for (int i = 2020; i <= DateTime.Now.Year; i++)
                    ddl_nam.Items.Insert(0, new ListItem("Năm " + i, i.ToString()));

                if (Session["nam_bangchamcong"] == null)
                {
                    Session["nam_bangchamcong"] = DateTime.Now.Year;
                    ddl_nam.SelectedIndex = 0;
                }
                else
                {
                    ddl_nam.SelectedIndex = DateTime.Now.Year - int.Parse(Session["nam_bangchamcong"].ToString());
                }
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
        //list_all là chốt sale
        var list_all = (from ob1 in db.taikhoan_table_2023s.Where(p => p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString() && p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()

                        select new
                        {
                            username = ob1.taikhoan,
                            avt = ob1.anhdaidien,
                            block = ob1.trangthai,
                            fullname = ob1.hoten,
                            fullname_khongdau = ob1.hoten_khongdau,
                            id_nganh=ob1.id_nganh,
                        });
        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.fullname.ToLower().Contains(_key) || p.fullname_khongdau.ToLower().Contains(_key) || p.username.ToLower() == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        if (DropDownList5.SelectedValue.ToString() != "")//ngành
        {
            var list_1 = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList(); list_all = list_all.Intersect(list_1).ToList();
        }

        //xử lý lọc theo trạng thái khóa hoặc chưa khóa

        //if (DropDownList1.SelectedValue.ToString() != "0")
        //{
        //    var list_1 = list_all.Where(p => p.block == DropDownList1.SelectedValue.ToString()).ToList();
        //    list_all = list_all.Intersect(list_1).ToList();
        //}
        list_all = list_all.OrderBy(p => p.fullname).ToList();
        //sắp xếp
        //switch (Session["index_sapxep_bangchamcong"].ToString())
        //{
        //    //case ("1"): list_all = list_all.OrderBy(p => p.ngaytao_tk).ToList(); break;
        //    default: list_all = list_all.OrderBy(p => p.fullname).ToList(); break;
        //}

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 50;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_bangchamcong"].ToString());
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
            list_id_split.Add("check_" + t.username);
        }
        int _s1 = stt + list_split.Count - 1;
        if (list_all.Count() != 0)
            lb_show.Text = "Hiển thị " + stt + "-" + _s1 + " trong số " + list_all.Count().ToString("#,##0") + " mục";
        else
            lb_show.Text = "Hiển thị 0-0 trong số 0";

        Repeater2.DataSource = list_split;
        Repeater2.DataBind();

        arr_nhanvien = new string[list_split.Count()];
        int _i = 0;
        foreach (var t in list_split)
        {
            arr_nhanvien[_i] = t.username;
            _i = _i + 1;
        }

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
        Session["current_page_bangchamcong"] = "1";

        main();
    }
    protected void txt_show_TextChanged(object sender, EventArgs e)
    {
        Session["current_page_bangchamcong"] = "1";

        main();
    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_bangchamcong"] = int.Parse(Session["current_page_bangchamcong"].ToString()) - 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_bangchamcong"] = int.Parse(Session["current_page_bangchamcong"].ToString()) + 1;

        main();
    }





    public string return_chamcong(string _user, int _ngay)
    {
        //if (!string.IsNullOrWhiteSpace(Request.QueryString["t"]))
        //{
        //    t = Request.QueryString["t"].ToString().Trim();
        //    int _t = 0;
        //    int.TryParse(t, out _t);//nếu là số nguyên thì gán cho t
        //    if (_t < 1 || _t > 12)
        //    {
        //        thang = DateTime.Now.Month;
        //        ddl_thang.SelectedIndex = thang - 1;
        //        thang = int.Parse(ddl_thang.SelectedValue.ToString());
        //    }
        //    else
        //    {
        //        thang = _t;
        //        ddl_thang.SelectedIndex = thang - 1;
        //    }
        //}
        //else
        //{
        //    thang = DateTime.Now.Month;
        //    ddl_thang.SelectedIndex = thang - 1;
        //    thang = int.Parse(ddl_thang.SelectedValue.ToString());
        //}

        //if (!string.IsNullOrWhiteSpace(Request.QueryString["n"]))
        //{
        //    n = Request.QueryString["n"].ToString().Trim();
        //    int _n = 0;
        //    int.TryParse(n, out _n);//nếu là số nguyên thì gán cho t
        //    if (_n < 2020)
        //    {
        //        nam = DateTime.Now.Year;
        //        ddl_nam.SelectedIndex = DateTime.Now.Year - nam;
        //        nam = int.Parse(ddl_nam.SelectedValue.ToString());
        //    }
        //    else
        //    {
        //        nam = _n;
        //        ddl_nam.SelectedIndex = DateTime.Now.Year - nam;
        //    }
        //}
        //else
        //{
        //    nam = DateTime.Now.Year;
        //    ddl_nam.SelectedIndex = DateTime.Now.Year - nam;
        //}

        DateTime _dt = DateTime.Parse(_ngay + "/" + Session["thang_bangchamcong"].ToString() + "/" + Session["nam_bangchamcong"].ToString()).Date;
        var q = db.bspa_chamcong_tables.Where(p => p.username == _user && p.ngay.Value.Date == _dt && p.id_chinhanh == Session["chinhanh"].ToString());
        if (q.Count() != 0)
        {
            string _chamcong = q.First().chamcong;
            switch (_chamcong)
            {
                case ("0"): return "<span data-role='hint' data-hint-position='top' data-hint-text='Làm đủ ngày' class='mif mif-checkmark fg-green  c-pointer'></span>";
                case ("1"): return "<span data-role='hint' data-hint-position='top' data-hint-text='Làm nữa ngày' class='mif mif-checkmark fg-blue  c-pointer text-linethrough'></span>";
                case ("2"): return "<span data-role='hint' data-hint-position='top' data-hint-text='Nghỉ phép' class='mif mif-cross fg-taupe c-pointer'></span>";
                case ("3"): return "<span data-role='hint' data-hint-position='top' data-hint-text='Nghỉ có lương' class='mif mif-checkmark fg-orange c-pointer'></span>";
                case ("4"): return "<span data-role='hint' data-hint-position='top' data-hint-text='Nghỉ không lương' class='mif mif-cross fg-red c-pointer'></span>";
                default: return "";
            }
        }
        return "";


    }
    public string return_ngaycong(string _user)
    {
        double _tongngaycong = 0;
        var q = db.bspa_chamcong_tables.Where(p => p.username == _user && p.id_chinhanh == Session["chinhanh"].ToString() && p.ngay.Value.Month == int.Parse(Session["thang_bangchamcong"].ToString()) && p.ngay.Value.Year == int.Parse(Session["nam_bangchamcong"].ToString()));
        if (q.Count() != 0)
        {
            foreach (var t in q)
            {
                string _chamcong = t.chamcong;
                if (_chamcong == "0" || _chamcong == "3")
                    _tongngaycong = _tongngaycong + 1;
                if (_chamcong == "1")
                    _tongngaycong = _tongngaycong + 0.5;
            }
            return _tongngaycong.ToString();
        }
        return "0";
    }

    public string return_luongcong(string _user)
    {
        double _tongngaycong = 0;
        Int64 _luongngay = db.taikhoan_table_2023s.Where(p => p.taikhoan == _user && p.id_chinhanh == Session["chinhanh"].ToString()).First().luongngay.Value;
        var q = db.bspa_chamcong_tables.Where(p => p.username == _user && p.id_chinhanh == Session["chinhanh"].ToString() && p.ngay.Value.Month == int.Parse(Session["thang_bangchamcong"].ToString()) && p.ngay.Value.Year == int.Parse(Session["nam_bangchamcong"].ToString()));
        if (q.Count() != 0)
        {
            foreach (var t in q)
            {
                string _chamcong = t.chamcong;
                if (_chamcong == "0" || _chamcong == "3")
                    _tongngaycong = _tongngaycong + 1;
                if (_chamcong == "1")
                    _tongngaycong = _tongngaycong + 0.5;
            }
            return Math.Round((_luongngay * _tongngaycong), 0).ToString("#,##0");
        }
        return "0";
    }


    protected void but_xem_Click(object sender, EventArgs e)
    {
        Session["thang_bangchamcong"] = ddl_thang.SelectedValue.ToString();
        Session["nam_bangchamcong"] = ddl_nam.SelectedValue.ToString();
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "2000", "warning"), true);
        // Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/bang-cham-cong.aspx?t=" + ddl_thang.SelectedValue.ToString() + "&n=" + ddl_nam.SelectedValue.ToString());
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Session["current_page_bangchamcong"] = "1";
        Session["search_bangchamcong"] = txt_search.Text.Trim();
        Session["show_bangchamcong"] = txt_show.Text.Trim();

        Session["index_loc_nganh_bangchamcong"] = DropDownList5.SelectedIndex;
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        Session["current_page_bangchamcong"] = null;
        Session["search_bangchamcong"] = null;
        Session["show_bangchamcong"] = null;

        Session["index_loc_nganh_bangchamcong"] = null;

        Session["thang_bangchamcong"] = null;
        Session["nam_bangchamcong"] = null;

        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/bang-cham-cong.aspx");
    }
}
