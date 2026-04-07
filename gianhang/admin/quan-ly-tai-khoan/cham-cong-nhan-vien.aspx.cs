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
    public string user = "", user_parent = "", notifi,thongbao_chamcong_homnay;
    public int tongsongay_trongthang, thang, nam;
    #region phân trang
    public int stt = 1, current_page = 1, show = 50, total_page = 1;
    List<string> list_id_split;
    #endregion
    nganh_class ng_cl = new nganh_class();

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
        if (HasAnyPermission("q2_8", "n2_8"))
        {
            tongsongay_trongthang = dt_cl.return_ngaycuoithang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).Day;
            thang = dt_cl.return_ngaycuoithang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).Month;
            nam = dt_cl.return_ngaycuoithang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).Year;

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
                if (HasAnyPermission("q2_8"))
                {
                }
                else
                {
                    DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList5.Enabled = false;
                }
                if (Session["current_page_chamcong"] == null)//lưu giữ trang hiện tại
                    Session["current_page_chamcong"] = "1";

                Session["index_sapxep_chamcong"] = "0";

                if (Session["search_chamcong"] != null)//lưu tìm kiếm
                    txt_search.Text = Session["search_chamcong"].ToString();
                else
                    Session["search_chamcong"] = txt_search.Text;

                if (Session["index_loc_nganh_chamcong"] != null)//lưu lọc theo theo toán
                    DropDownList5.SelectedIndex = int.Parse(Session["index_loc_nganh_chamcong"].ToString());
                else
                    Session["index_loc_nganh_chamcong"] = DropDownList5.SelectedIndex.ToString();

                string ngayQuery = (Request.QueryString["ngay"] ?? "").Trim();
                if (ngayQuery != "")
                {
                    Session["ngay_chamcong"] = ngayQuery;
                    txt_ngaychamcong.Text = ngayQuery;
                }
                else if (Session["ngay_chamcong"] != null)
                    txt_ngaychamcong.Text = Session["ngay_chamcong"].ToString();
                else
                {
                    txt_ngaychamcong.Text = DateTime.Now.Date.ToString();
                    Session["ngay_chamcong"] = txt_ngaychamcong.Text;
                }
            }
            var q = db.bspa_chamcong_tables.Where(p => p.user_parent == user_parent && DateTime.Now.Date == p.ngay.Value.Date && p.id_chinhanh == Session["chinhanh"].ToString());
            if (q.Count() == 0)
                thongbao_chamcong_homnay = "Hôm nay chưa chấm công.";
            else
                thongbao_chamcong_homnay = "Hôm nay đã chấm công.";
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
        var list_all = (from ob1 in db.taikhoan_table_2023s.Where(p => p.user_parent == user_parent && p.trangthai == "Đang hoạt động"&& p.id_chinhanh == Session["chinhanh"].ToString()).ToList()

                        select new
                        {
                            username = ob1.taikhoan,
                            avt = ob1.anhdaidien,
                            block = ob1.trangthai,
                            fullname = ob1.hoten,
                            fullname_khongdau = ob1.hoten_khongdau,
                            id_nganh = ob1.id_nganh,
                        });
        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.fullname.ToLower().Contains(_key) || p.fullname_khongdau.ToLower().Contains(_key) || p.username.ToLower() == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        //xử lý lọc theo trạng thái khóa hoặc chưa khóa

        //if (DropDownList1.SelectedValue.ToString() != "0")
        //{
        //    var list_1 = list_all.Where(p => p.block == DropDownList1.SelectedValue.ToString()).ToList();
        //    list_all = list_all.Intersect(list_1).ToList();
        //}

        if (DropDownList5.SelectedValue.ToString() != "")//ngành
        {
            var list_1 = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList(); list_all = list_all.Intersect(list_1).ToList();
        }

        //sắp xếp
        switch (Session["index_sapxep_chamcong"].ToString())
        {
            //case ("1"): list_all = list_all.OrderBy(p => p.ngaytao_tk).ToList(); break;
            default: list_all = list_all.OrderBy(p => p.fullname).ToList(); break;
        }

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 50;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_chamcong"].ToString());
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
        Session["current_page_chamcong"] = "1";

        main();
    }
    protected void txt_show_TextChanged(object sender, EventArgs e)
    {
        Session["current_page_chamcong"] = "1";

        main();
    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_chamcong"] = int.Parse(Session["current_page_chamcong"].ToString()) - 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_chamcong"] = int.Parse(Session["current_page_chamcong"].ToString()) + 1;

        main();
    }

    protected void but_lammoi_Click(object sender, EventArgs e)
    {
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Làm mới thành công.", "2000", "warning"), true);
    }

    public string return_chamcong(string _username)
    {
        string _kq = "";

        var q = db.bspa_chamcong_tables.Where(p => p.username == _username&& p.id_chinhanh == Session["chinhanh"].ToString() && p.ngay.Value.Date == DateTime.Parse(Session["ngay_chamcong"].ToString()));
        if (q.Count() != 0)
        {
            string _chamcong = q.First().chamcong;
            if (_chamcong == "0")
                _kq = _kq + "<option selected value='0'>Làm đủ ngày</option>";
            else
                _kq = _kq + "<option value='0'>Làm đủ ngày</option>";

            if (_chamcong == "1")
                _kq = _kq + "<option selected value='1'>Làm nữa ngày</option>";
            else
                _kq = _kq + "<option value='1'>Làm nữa ngày</option>";

            if (_chamcong == "2")
                _kq = _kq + "<option selected value='2'>Nghỉ phép</option>";
            else
                _kq = _kq + "<option value='2'>Nghỉ phép</option>";

            if (_chamcong == "3")
                _kq = _kq + "<option selected value='3'>Nghỉ có lương</option>";
            else
                _kq = _kq + "<option value='3'>Nghỉ có lương</option>";

            if (_chamcong == "4")
                _kq = _kq + "<option selected value='4'>Nghỉ không lương</option>";
            else
                _kq = _kq + "<option value='4'>Nghỉ không lương</option>";

         
            return _kq;
        }
        else
        {
            if (DateTime.Now.Date == DateTime.Parse(Session["ngay_chamcong"].ToString()))//nếu ngày hôm nay chưa chấm công thì mặc định mà làm đđ ngày
            {
                _kq = _kq + "<option value='0'>Làm đủ ngày</option>";
                _kq = _kq + "<option value='1'>Làm nữa ngày</option>";
                _kq = _kq + "<option value='2'>Nghỉ phép</option>";
                _kq = _kq + "<option value='3'>Nghỉ có lương</option>";
                _kq = _kq + "<option value='4'>Nghỉ không lương</option>";
              
                return _kq;
            }
            else//nếu trước ngày hôm nay chưa chấm công mặc định là nghỉ k lương
            {
                _kq = _kq + "<option value='0'>Làm đủ ngày</option>";
                _kq = _kq + "<option value='1'>Làm nữa ngày</option>";
                _kq = _kq + "<option value='2'>Nghỉ phép</option>";
                _kq = _kq + "<option value='3'>Nghỉ có lương</option>";
                _kq = _kq + "<option selected value='4'>Nghỉ không lương</option>";
             
                return _kq;
            }
        }

    }

    protected void but_save_Click(object sender, ImageClickEventArgs e)
    {
        string _ngaychamcong = txt_ngaychamcong.Text;
        if (dt_cl.check_date(_ngaychamcong) == false)
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Ngày chấm công không hợp lệ.", "2000", "warning"), true);
        else
        {
            DateTime _dt = DateTime.Parse(_ngaychamcong).Date;
            if (_dt > DateTime.Now.Date)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Ngày chấm công không hợp lệ.", "2000", "warning"), true);
            else
            {
                for (int i = 0; i < list_id_split.Count; i++)
                {
                    string _user = list_id_split[i].Replace("check_", "");
                    string _chamcong = Request.Form["chamcong_" + _user].Trim();
                    if (tk_cl.exist_user_of_userparent(_user, user_parent))
                    {
                        var q = db.bspa_chamcong_tables.Where(p => p.username == _user && p.ngay.Value.Date == _dt&& p.id_chinhanh == Session["chinhanh"].ToString());
                        if (q.Count() == 0)//nếu chưa chấm công ngày này thì thêm mới chấm công
                        {
                            bspa_chamcong_table _ob = new bspa_chamcong_table();
                            _ob.username = _user;
                            _ob.user_parent = user_parent;
                            _ob.ngay = _dt;
                            _ob.chamcong = _chamcong;
                            _ob.id_chinhanh = Session["chinhanh"].ToString();
                            db.bspa_chamcong_tables.InsertOnSubmit(_ob);
                            db.SubmitChanges();
                        }
                        else//nếu có chấm rồi thì cập nhật mới
                        {
                            bspa_chamcong_table _ob = q.First();
                            _ob.chamcong = _chamcong;
                            db.SubmitChanges();
                        }
                    }
                }
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Chấm công thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/bang-cham-cong.aspx");
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chấm công thành công.", "2000", "warning"), true);
            }
        }
    }

    protected void txt_ngaychamcong_TextChanged(object sender, EventArgs e)
    {
        Session["ngay_chamcong"] = txt_ngaychamcong.Text;
        Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/cham-cong-nhan-vien.aspx");
    }

    protected void Button4_Click(object sender, EventArgs e)
    {
        if (HasAnyPermission("q2_8", "n2_8"))
        {
            string _ngaychamcong = txt_ngaychamcong.Text;
            if (dt_cl.check_date(_ngaychamcong) == false)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Ngày chấm công không hợp lệ.", "2000", "warning"), true);
            else
            {
                DateTime _dt = DateTime.Parse(_ngaychamcong).Date;
                if (_dt > DateTime.Now.Date)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Ngày chấm công không hợp lệ.", "2000", "warning"), true);
                else
                {
                    int _count = 0;
                    for (int i = 0; i < list_id_split.Count; i++)
                    {
                        if (Request.Form[list_id_split[i]] == "on")
                        {
                            string _id = list_id_split[i].Replace("check_", "");
                            var q_tk = db.taikhoan_table_2023s.Where(p => p.taikhoan.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                            if (q_tk.Count() != 0)
                            {
                                //lưu chấm công
                                var q = db.bspa_chamcong_tables.Where(p => p.username == _id && p.ngay.Value.Date == _dt&& p.id_chinhanh == Session["chinhanh"].ToString());
                                if (q.Count() == 0)//nếu chưa chấm công ngày này thì thêm mới chấm công
                                {
                                    bspa_chamcong_table _ob = new bspa_chamcong_table();
                                    _ob.username = _id;
                                    _ob.user_parent = user_parent;
                                    _ob.ngay = _dt;
                                    _ob.id_chinhanh = Session["chinhanh"].ToString();
                                    _ob.chamcong = DropDownList1.SelectedValue.ToString();
                                    db.bspa_chamcong_tables.InsertOnSubmit(_ob);
                                    db.SubmitChanges();
                                }
                                else//nếu có chấm rồi thì cập nhật mới
                                {
                                    bspa_chamcong_table _ob = q.First();
                                    _ob.chamcong = DropDownList1.SelectedValue.ToString();
                                    db.SubmitChanges();
                                }

                                _count = _count + 1;
                            }
                        }
                    }
                    if (_count > 0)
                    {
                        main();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lưu thành công.", "4000", "warning"), true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
                    }
                }
            } 
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Session["current_page_chamcong"] = "1";
        Session["search_chamcong"] = txt_search.Text.Trim();
        Session["show_chamcong"] = txt_show.Text.Trim();

        Session["index_loc_nganh_chamcong"] = DropDownList5.SelectedIndex;
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        Session["current_page_chamcong"] = null;
        Session["search_chamcong"] = null;
        Session["show_chamcong"] = null;

        Session["index_loc_nganh_chamcong"] = null;

        Session["thang_chamcong"] = null;
        Session["nam_chamcong"] = null;

        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/cham-cong-nhan-vien.aspx");
    }
}
