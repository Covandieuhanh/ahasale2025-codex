using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_banner_Default : System.Web.UI.Page
{
    // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", ""), true);

    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();
    private const string ViewAdd = "add";

    private string BuildListUrl()
    {
        return ResolveUrl("~/admin/quan-ly-banner/default.aspx");
    }

    private string BuildAddUrl()
    {
        return BuildListUrl() + "?view=" + ViewAdd;
    }

    private void RedirectTo(string url)
    {
        Response.Redirect(url, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void RedirectToListWithNotice(string title, string message, string type = "warning")
    {
        Session["thongbao"] = thongbao_class.metro_notifi_onload(title, message, "1000", type);
        RedirectTo(BuildListUrl());
    }

    private void ShowAddViewFromQuery()
    {
        check_login_cl.check_login_admin("none", "none");
        reset_control_add_edit();
        pn_add.Visible = true;
        up_add.Update();
        up_main.Visible = false;
    }

    private void ApplyOpenViewFromQuery()
    {
        string view = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
        if (view == ViewAdd)
            ShowAddViewFromQuery();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri;
            check_login_cl.check_login_admin("none", "none");

            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";

            ViewState["taikhoan"] = _tk;

            set_dulieu_macdinh();
            show_main();
            ApplyOpenViewFromQuery();

        }
    }

    public void set_dulieu_macdinh()
    {

        ViewState["current_page_qlbanner"] = "1";
    }
    #region main - phân trang - tìm kiếm

    public void show_main()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                #region lấy dữ liệu
                var list_all = (from ob1 in db.Banner_tbs
                                    //join ob2 in db.DanhMuc_tbs on ob1.id_DanhMuc equals ob2.id.ToString() into danhMucGroup
                                    //from ob2 in danhMucGroup.DefaultIfEmpty()
                                select new
                                {
                                    ob1.id,
                                    ob1.img,
                                    ob1.rank,
                                    ob1.show
                                }).AsQueryable();

                // Kiểm tra xem textbox có dữ liệu tìm kiếm không
                string _key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                    list_all = list_all.Where(p => p.id.ToString() == _key);
                else
                {
                    string _key1 = txt_timkiem1.Text.Trim();
                    if (!string.IsNullOrEmpty(_key1))
                        list_all = list_all.Where(p => p.id.ToString() == _key1);
                }

                //sắp xếp
                list_all = list_all.OrderBy(p => p.rank);
                int _Tong_Record = list_all.Count();
                #endregion

                #region phân trang OK, k sửa
                // Xử lý số record mỗi trang
                //int show = Number_cl.Check_Int(txt_show.Text.Trim()); if (show <= 0) show = 30;
                int show = 30;
                //xử lý trang hiện tại. Đảm bảo current_page không nhỏ hơn 1 và không lớn hơn total_page
                int current_page = int.Parse(ViewState["current_page_qlbanner"].ToString()); int total_page = number_of_page_class.return_total_page(_Tong_Record, show); if (current_page < 1) current_page = 1; else if (current_page > total_page) current_page = total_page;
                ViewState["total_page"] = total_page;
                //xử lý nút bấm tới lui
                if (current_page >= total_page)
                {
                    but_xemtiep.Enabled = false;//máy tính
                    but_xemtiep1.Enabled = false;//điện thoại
                }
                else
                {
                    but_xemtiep.Enabled = true;
                    but_xemtiep1.Enabled = true;
                }
                if (current_page == 1)
                {
                    but_quaylai.Enabled = false;
                    but_quaylai1.Enabled = false;
                }
                else
                {
                    but_quaylai.Enabled = true;
                    but_quaylai1.Enabled = true;
                }
                //PHÂN TRANG****PHÂN TRANG
                var list_split = list_all.Skip(current_page * show - show).Take(show);
                //xử lý thanh thông báo phân trang
                int stt = (show * current_page) - show + 1; int _s1 = stt + list_split.Count() - 1;
                if (_Tong_Record != 0) lb_show.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0"); else lb_show.Text = "0-0/0"; lb_show_md.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");
                #endregion
                Repeater1.DataSource = list_split;
                Repeater1.DataBind();
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["current_page_qlbanner"] = int.Parse(ViewState["current_page_qlbanner"].ToString()) - 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_qlbanner" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qlbanner"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qlbanner"].ToString();
                // Thiết lập lại thời gian hết hạn của cookie là 1 ngày từ thời điểm hiện tại
                cookie.Expires = AhaTime_cl.Now.AddDays(1);
                // Cập nhật cookie trong Response.Cookies
                Response.Cookies.Set(cookie);
            }
            #endregion
            show_main();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["current_page_qlbanner"] = int.Parse(ViewState["current_page_qlbanner"].ToString()) + 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_qlbanner" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qlbanner"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qlbanner"].ToString();
                // Thiết lập lại thời gian hết hạn của cookie là 1 ngày từ thời điểm hiện tại
                cookie.Expires = AhaTime_cl.Now.AddDays(1);
                // Cập nhật cookie trong Response.Cookies
                Response.Cookies.Set(cookie);
            }
            #endregion
            show_main();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["current_page_qlbanner"] = 1;
            show_main();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string; // Sử dụng 'as' để tránh lỗi nếu là null
            if (!string.IsNullOrEmpty(_tk)) // Kiểm tra xem '_tk' có hợp lệ hay không
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    #endregion



    #region ADD - EDIT - CHI TIẾT
    public void reset_control_add_edit()
    {
        txt_link_fileupload.Text = "";
    }

    protected void but_show_form_add_Click(object sender, EventArgs e)
    {

        check_login_cl.check_login_admin("none", "none");
        RedirectTo(BuildAddUrl());

    }
    protected void but_close_form_add_Click(object sender, EventArgs e)
    {

        check_login_cl.check_login_admin("none", "none");
        //reset control
        reset_control_add_edit();
        RedirectTo(BuildListUrl());

    }


    protected void but_add_edit_Click(object sender, EventArgs e)
    {

        #region Chuẩn bị dữ liệu
        //xử lý dữ liệu đầu vào
        string _img = txt_link_fileupload.Text;
        int _rank = Number_cl.Check_Int(txt_rank.Text.Trim());
        #endregion
        using (dbDataContext db = new dbDataContext())
        {
            #region Kiểm tra ngoại lệ.

            if (_img == "")
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn ảnh.", "false", "false", "OK", "alert", ""), true);
                return;
            }
            #endregion

            #region thêm mới
            Banner_tb _ob = new Banner_tb();
            _ob.img = _img;
            _ob.rank = _rank;
            _ob.show = CheckBox1.Checked;
            db.Banner_tbs.InsertOnSubmit(_ob);
            db.SubmitChanges();
            #endregion

            #region cập nhật dữ liệu và update hiển thị
            txt_link_fileupload.Text = "";
            RedirectToListWithNotice("Thông báo", "Xử lý thành công.");
            #endregion

        }
    }
    #endregion


    protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //lấy ra từng item
        var dataItem = (dynamic)e.Item.DataItem;


        // Tìm CheckBox1 và thiết lập Checked nếu là nổi bật
        var checkBox = (CheckBox)e.Item.FindControl("check_show_item");
        if (checkBox != null)
        {
            checkBox.Checked = dataItem.show;
            //hoặc
            // Lấy giá trị cần so sánh từ DataItem (sửa 'ten_field' thành tên trường dữ liệu phù hợp)
            //string valueToCompare = Convert.ToString(DataBinder.Eval(dataItem, "Tên Cột")) ?? string.Empty;
        }
    }

    protected void but_save_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("none", "none");
        using (dbDataContext db = new dbDataContext())
        {
            foreach (RepeaterItem item in Repeater1.Items)
            {
                // Tìm các điều khiển TextBox và Label từ RepeaterItem
                CheckBox check_show_item = (CheckBox)item.FindControl("check_show_item");
                TextBox txt_rank_1 = (TextBox)item.FindControl("txt_rank_1");
                Label lbID = (Label)item.FindControl("lbID");

                // Kiểm tra nếu cả TextBox và Label không null
                if (check_show_item != null && lbID != null && txt_rank_1 != null)
                {
                    // Lấy ID và rank từ các điều khiển
                    string _id = lbID.Text;
                    bool _show = check_show_item.Checked;
                    int _rank = Number_cl.Check_Int(txt_rank_1.Text);
                    var q = db.Banner_tbs.FirstOrDefault(p => p.id.ToString() == _id);
                    if (q != null)
                    {
                        q.show = _show;
                        q.rank = _rank;
                    }
                }
            }
            db.SubmitChanges();
            show_main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
        }
    }

    protected void but_remove_bin_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("none", "none");
        using (dbDataContext db = new dbDataContext())
        {
            foreach (RepeaterItem item in Repeater1.Items)
            {
                CheckBox chkItem = (CheckBox)item.FindControl("checkID");
                Label lbID = (Label)item.FindControl("lbID");

                if (chkItem != null && chkItem.Checked)
                {
                    string _id = lbID.Text;
                    var q = db.Banner_tbs.FirstOrDefault(p => p.id.ToString() == _id);
                    if (q != null)
                    {
                        db.Banner_tbs.DeleteOnSubmit(q);
                    }
                }
            }
            db.SubmitChanges();
            show_main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
        }
    }
}
