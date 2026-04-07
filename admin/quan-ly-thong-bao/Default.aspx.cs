using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_thong_bao_Default : System.Web.UI.Page
{
    // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Tài khoản đã bị khóa.", "2600", "danger"), true);
    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();
    private const string ViewFilter = "filter";
    private const string ViewExport = "export";
    private const string ViewPrint = "print";
    private const string TrashQueryKey = "bin";
    private const string TrashQueryValue = "trash";


    private string BuildListUrl(bool showTrash = false)
    {
        string url = ResolveUrl("~/admin/quan-ly-thong-bao/default.aspx");
        if (!showTrash)
            return url;

        string separator = url.Contains("?") ? "&" : "?";
        return url + separator + TrashQueryKey + "=" + HttpUtility.UrlEncode(TrashQueryValue);
    }

    private bool IsTrashModeRequested()
    {
        return string.Equals((Request.QueryString[TrashQueryKey] ?? "").Trim(), TrashQueryValue, StringComparison.OrdinalIgnoreCase);
    }

    private string AppendTrashModeIfNeeded(string url)
    {
        if (string.IsNullOrWhiteSpace(url) || !IsTrashModeRequested())
            return url;

        string separator = url.Contains("?") ? "&" : "?";
        return url + separator + TrashQueryKey + "=" + HttpUtility.UrlEncode(TrashQueryValue);
    }

    private void ApplyTrashModeFromQuery()
    {
        bool showTrash = IsTrashModeRequested();
        ViewState["showbin"] = showTrash ? "1" : "0";
        UpdateTrashModeControls(showTrash);
    }

    private void UpdateTrashModeControls(bool? showTrashOverride = null)
    {
        bool showTrash = showTrashOverride.HasValue
            ? showTrashOverride.Value
            : string.Equals((ViewState["showbin"] ?? "0").ToString(), "1", StringComparison.OrdinalIgnoreCase);

        but_remove_bin.Visible = !showTrash;
        but_khoiphuc.Visible = showTrash;
        but_show_thungrac.Visible = !showTrash;
        but_show_main.Visible = showTrash;
        but_quayve_trangchu.Visible = showTrash;
    }

    private string BuildFilterUrl()
    {
        return ResolveUrl("~/admin/quan-ly-thong-bao/bo-loc.aspx");
    }

    private string BuildExportUrl()
    {
        return ResolveUrl("~/admin/quan-ly-thong-bao/xuat-du-lieu.aspx");
    }

    private string BuildPrintUrl()
    {
        return ResolveUrl("~/admin/quan-ly-thong-bao/ban-in.aspx");
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

    private void ApplyRouteLinks()
    {
        but_show_form_loc.NavigateUrl = AppendTrashModeIfNeeded(BuildFilterUrl());
        but_show_form_xuat.NavigateUrl = AppendTrashModeIfNeeded(BuildExportUrl());
        but_show_form_in.NavigateUrl = AppendTrashModeIfNeeded(BuildPrintUrl());
        but_show_thungrac.NavigateUrl = BuildListUrl(true);
        but_show_main.NavigateUrl = BuildListUrl();
        but_quayve_trangchu.NavigateUrl = BuildListUrl();
        close_loc.NavigateUrl = AppendTrashModeIfNeeded(BuildListUrl());
        close_xuat.NavigateUrl = AppendTrashModeIfNeeded(BuildListUrl());
        close_in.NavigateUrl = AppendTrashModeIfNeeded(BuildListUrl());
    }

    private void HideOtherPanels()
    {
        pn_loc.Visible = false;
        pn_xuat.Visible = false;
        pn_in.Visible = false;
    }

    private void ShowFilterViewFromQuery()
    {
        AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
        HideOtherPanels();
        pn_loc.Visible = true;
        up_loc.Update();
        up_main.Visible = false;
    }

    private void ShowExportViewFromQuery()
    {
        AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
        HideOtherPanels();

        check_list_page.Items.Clear();
        for (int i = 1; i <= int.Parse(ViewState["total_page"].ToString()); i++)
        {
            ListItem item = new ListItem(string.Format("Trang {0}", i), i.ToString());
            check_list_page.Items.Add(item);
            item.Selected = true;
        }

        pn_xuat.Visible = true;
        up_xuat.Update();
        up_main.Visible = false;
    }

    private void ShowPrintViewFromQuery()
    {
        AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
        HideOtherPanels();
        pn_in.Visible = true;
        up_in.Update();
        up_main.Visible = false;
    }

    private void ApplyOpenViewFromQuery()
    {
        string view = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
        if (view == ViewFilter)
        {
            if (!AdminFullPageRoute_cl.IsTransferredRequest(Context))
            {
                RedirectTo(BuildFilterUrl());
                return;
            }
            ShowFilterViewFromQuery();
            return;
        }
        if (view == ViewExport)
        {
            if (!AdminFullPageRoute_cl.IsTransferredRequest(Context))
            {
                RedirectTo(BuildExportUrl());
                return;
            }
            ShowExportViewFromQuery();
            return;
        }
        if (view == ViewPrint)
        {
            if (!AdminFullPageRoute_cl.IsTransferredRequest(Context))
            {
                RedirectTo(BuildPrintUrl());
                return;
            }
            ShowPrintViewFromQuery();
        }
    }

    private string GetCurrentAdminAccount()
    {
        string taiKhoanMaHoa = Session["taikhoan"] as string;
        if (string.IsNullOrEmpty(taiKhoanMaHoa))
            return "";

        return mahoa_cl.giaima_Bcorn(taiKhoanMaHoa);
    }

    public void set_dulieu_macdinh()
    {
        try
        {
            ResetButtonCss();
            txt_show.Text = "100";
            ViewState["current_page_qltb"] = "1";
            ViewState["showbin"] = "0";//hiển thị các mục k nằm trong thùng rác, 1: thùng rác

            #region set_get_cookie
            // Lấy cookie "cookie_qltb" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qltb"];
            if (cookie == null)
            {
                // Nếu cookie không tồn tại, tạo cookie mới
                cookie = new HttpCookie("cookie_qltb");
                cookie["show"] = txt_show.Text;//lưu số dòng hiển thị mỗi trang
                cookie["trang_hientai"] = "1";//lưu trang hiện tại
                cookie["id_loctheothoigian"] = ddl_thoigian.SelectedValue;
                cookie["tungay"] = txt_tungay.Text;
                cookie["denngay"] = txt_denngay.Text;
                // Đặt thời gian hết hạn của cookie là 1 ngày từ thời điểm hiện tại
                cookie.Expires = AhaTime_cl.Now.AddDays(1);
                cookie.HttpOnly = true;
                cookie.Secure = true;
                // Thêm cookie vào Response.Cookies
                Response.Cookies.Add(cookie);

            }
            else
            {
                // Nếu cookie đã tồn tại, lấy giá trị từ cookie
                txt_show.Text = cookie["show"];
                ViewState["current_page_qltb"] = cookie["trang_hientai"];
                ddl_thoigian.SelectedValue = cookie["id_loctheothoigian"];
                txt_tungay.Text = cookie["tungay"];
                txt_denngay.Text = cookie["denngay"];
                // Thiết lập lại thời gian hết hạn của cookie là 1 ngày từ thời điểm hiện tại
                cookie.Expires = AhaTime_cl.Now.AddDays(1);
                // Cập nhật cookie trong Response.Cookies
                Response.Cookies.Set(cookie);
            }
            #endregion
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
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri;
                AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");

                //Nó k kịp lưu vì nó tải trang này trước khi load menu-left
                //if (Session["title"] != null)
                //    ViewState["title"] = Session["title"].ToString();

                ViewState["sapxep_thongbao"] = "1";//mặc định sx thông báo theo mới nhất lên đầu
                but_sapxep_moinhat.CssClass = "info small rounded";

                ApplyRouteLinks();
                set_dulieu_macdinh();
                ApplyTrashModeFromQuery();
                show_main();
                ApplyOpenViewFromQuery();
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
    }
    #region main - phân trang - tìm kiếm

    protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        try
        {
            //lấy ra từng item
            //var dataItem = (dynamic)e.Item.DataItem;
            //truy xuất thuộc tính của item rồi làm gì thì làm, ví dụ: dataItem.noibat dataItem.id

            // Tìm checkID động và xử lý tiếp
            //var checkBox = (CheckBox)e.Item.FindControl("checkID");
            //if (checkBox != null)
            //{
            // if(dataItem.noibat==true)
            //    checkBox.Checked = true;
            //}
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
    public void show_main()
    {
        try
        {
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                #region lấy dữ liệu
                var list_all = (from ob1 in db.ThongBao_tbs
                                join ob2 in db.taikhoan_tbs on ob1.nguoithongbao equals ob2.taikhoan into senderGroup
                                from ob2 in senderGroup.DefaultIfEmpty()
                                where ob1.nguoinhan == taiKhoan
                                select new
                                {
                                    ob1.id, // id thông báo
                                    avt_nguoithongbao = (ob2 == null || ob2.anhdaidien == null || ob2.anhdaidien == "")
                                        ? "/uploads/images/macdinh.jpg"
                                        : ob2.anhdaidien,
                                    daxem = ob1.daxem,
                                    noidung = ob1.noidung ?? "",
                                    thoigian = ob1.thoigian,
                                    bin = ob1.bin,
                                    link = (ob1.link == null || ob1.link == "")
                                        ? "/admin/default.aspx?"
                                        : (ob1.link.Contains("?") ? ob1.link + "&" : ob1.link + "?")
                                }).AsQueryable();

                //hiển thị các mục đã xóa hoặc không
                if (ViewState["showbin"].ToString() == "1")
                    list_all = list_all.Where(p => p.bin == true);
                else
                    list_all = list_all.Where(p => p.bin == false);

                // Kiểm tra xem textbox có dữ liệu tìm kiếm không
                string _key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                    list_all = list_all.Where(p => p.noidung.Contains(_key) || p.id.ToString() == _key);
                else
                {
                    string _key1 = txt_timkiem1.Text.Trim();
                    if (!string.IsNullOrEmpty(_key1))
                        list_all = list_all.Where(p => p.noidung.Contains(_key1) || p.id.ToString() == _key1);
                }

                //xử lý theo thời gian
                string _id_locthoigian = ddl_thoigian.SelectedValue;
                if (_id_locthoigian == "1")//lọc theo ngày tạo
                {
                    if (txt_tungay.Text != "")
                        list_all = list_all.Where(p => p.thoigian.Value.Date >= DateTime.Parse(txt_tungay.Text).Date);
                    if (txt_denngay.Text != "")
                        list_all = list_all.Where(p => p.thoigian.Value.Date <= DateTime.Parse(txt_denngay.Text).Date);
                }

                //sắp xếp
                if (Convert.ToString(ViewState["sapxep_thongbao"]) == "2")//lọc ra chưa đọc, mới nhất lên đầu
                    list_all = list_all.Where(p => p.daxem == false).OrderByDescending(p => p.thoigian);
                else//sx theo mới nhất lên đầu
                    list_all = list_all.OrderByDescending(p => p.thoigian);
                int _Tong_Record = list_all.Count();
                #endregion

                #region phân trang OK, k sửa
                // Xử lý số record mỗi trang
                int show = Number_cl.Check_Int(txt_show.Text.Trim()); if (show <= 0) show = 100;
                //xử lý trang hiện tại. Đảm bảo current_page không nhỏ hơn 1 và không lớn hơn total_page
                int current_page = int.Parse(ViewState["current_page_qltb"].ToString()); int total_page = number_of_page_class.return_total_page(_Tong_Record, show); if (current_page < 1) current_page = 1; else if (current_page > total_page) current_page = total_page;
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
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            ViewState["current_page_qltb"] = int.Parse(ViewState["current_page_qltb"].ToString()) - 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_qltb" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qltb"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qltb"].ToString();
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
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            ViewState["current_page_qltb"] = int.Parse(ViewState["current_page_qltb"].ToString()) + 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_qltb" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qltb"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qltb"].ToString();
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
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            ViewState["current_page_qltb"] = 1;
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

    #region show thùng rác
    protected void but_show_thungrac_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            RedirectTo(BuildListUrl(true));
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
    //but_show_main và but_quayve_trangchu giống nhau
    protected void but_show_main_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            RedirectTo(BuildListUrl());
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
    protected void but_quayve_trangchu_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            RedirectTo(BuildListUrl());
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

    #region Xuất excel
    protected void but_show_form_xuat_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            string view = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
            if (view == ViewExport)
                RedirectTo(BuildListUrl());
            else
                RedirectTo(BuildExportUrl());
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

    protected void but_xuat_excel_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            bool _chonmuc = false, _chonPage = false;

            foreach (ListItem item in check_list_excel.Items)
            {
                if (item.Selected)
                {
                    _chonmuc = true;
                    break; // Thoát vòng lặp sớm nếu tìm thấy mục được chọn
                }
            }
            if (!_chonmuc)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "2600", "danger"), true);
              
                return; // Kết thúc sớm nếu không có mục nào được chọn
            }

            // Khởi tạo danh sách để lưu các mục được chọn (nếu cần)
            List<ListItem> selectedPage = new List<ListItem>();//để lưu các trang được chọn. dùng để xuất excel
            foreach (ListItem item in check_list_page.Items)
            {
                if (item.Selected)//nếu có trang đc chọn
                {
                    selectedPage.Add(item);//lưu lại trang đc chọn
                    _chonPage = true;
                    //break; // Thoát vòng lặp sớm nếu tìm thấy mục được chọn. K thoát vòng lặp vì để lưu hết trang đc chọn
                }
            }
            if (!_chonPage)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có trang nào được chọn.", "2600", "danger"), true);
                
                return; // Kết thúc sớm nếu không có mục nào được chọn
            }

            if (!Directory.Exists(Server.MapPath("~/uploads/files/")))
                Directory.CreateDirectory(Server.MapPath("~/uploads/files/"));
            using (dbDataContext db = new dbDataContext())
            {
                string taiKhoan = GetCurrentAdminAccount();
                if (string.IsNullOrEmpty(taiKhoan))
                    return;
                #region lấy dữ liệu
                var list_all = (from ob1 in db.ThongBao_tbs
                                join ob2 in db.taikhoan_tbs on ob1.nguoithongbao equals ob2.taikhoan into senderGroup
                                from ob2 in senderGroup.DefaultIfEmpty()
                                where ob1.nguoinhan == taiKhoan
                                select new
                                {
                                    ob1.id, // id thông báo
                                    avt_nguoithongbao = (ob2 == null || ob2.anhdaidien == null || ob2.anhdaidien == "")
                                        ? "/uploads/images/macdinh.jpg"
                                        : ob2.anhdaidien,
                                    daxem = ob1.daxem,
                                    noidung = ob1.noidung ?? "",
                                    thoigian = ob1.thoigian,
                                    bin = ob1.bin,
                                    link = (ob1.link == null || ob1.link == "")
                                        ? "/admin/default.aspx?"
                                        : (ob1.link.Contains("?") ? ob1.link + "&" : ob1.link + "?")
                                }).AsQueryable();

                //hiển thị các mục đã xóa hoặc không
                if (ViewState["showbin"].ToString() == "1")
                    list_all = list_all.Where(p => p.bin == true);
                else
                    list_all = list_all.Where(p => p.bin == false);

                // Kiểm tra xem textbox có dữ liệu tìm kiếm không
                string _key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                    list_all = list_all.Where(p => p.noidung.Contains(_key) || p.id.ToString() == _key);
                else
                {
                    string _key1 = txt_timkiem1.Text.Trim();
                    if (!string.IsNullOrEmpty(_key1))
                        list_all = list_all.Where(p => p.noidung.Contains(_key1) || p.id.ToString() == _key1);
                }

                //xử lý theo thời gian
                string _id_locthoigian = ddl_thoigian.SelectedValue;
                if (_id_locthoigian == "1")//lọc theo ngày tạo
                {
                    if (txt_tungay.Text != "")
                        list_all = list_all.Where(p => p.thoigian.Value.Date >= DateTime.Parse(txt_tungay.Text).Date);
                    if (txt_denngay.Text != "")
                        list_all = list_all.Where(p => p.thoigian.Value.Date <= DateTime.Parse(txt_denngay.Text).Date);
                }

                //sắp xếp
                if (Convert.ToString(ViewState["sapxep_thongbao"]) == "2")//lọc ra chưa đọc, mới nhất lên đầu
                    list_all = list_all.Where(p => p.daxem == false).OrderByDescending(p => p.thoigian);
                else//sx theo mới nhất lên đầu
                    list_all = list_all.OrderByDescending(p => p.thoigian);
                int _Tong_Record = list_all.Count();
                #endregion


                #region xuất vào excel
                // Sử dụng EPPlus để tạo một tệp Excel và ghi dữ liệu vào đó
                using (ExcelPackage package = new ExcelPackage())
                {
                    int _cot = 1;//đánh dấu là cột 1
                                 //đặt tên sheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                    //ghi tiêu đề
                    foreach (ListItem item in check_list_excel.Items)//duyệt qua hết check list mục cần xuất
                    {
                        if (item.Selected)//nếu mục này có chọn
                        {
                            worksheet.Cells[1, _cot].Value = item.Text;
                            _cot = _cot + 1;
                        }
                    }
                    //hết vòng lặp thì cột bằng 1 lại
                    _cot = 1;
                    // Bắt đầu ghi dữ liệu từ dòng thứ 2
                    int _row = 2;

                    #region xác định dữ liệu chuẩn bị xuất (list_xuat). Là xuất tất cả hay các trang riêng lẻ đc chọn
                    // Ghi dữ liệu từ LINQ Query vào ExcelWorksheet
                    IEnumerable<dynamic> list_xuat;
                    if (check_all_page.Checked == true)//nếu chọn tất cả
                        list_xuat = list_all;
                    else//nếu chọn trang riêng lẻ
                    {
                        // Khởi tạo danh sách để lưu trữ dữ liệu xuất ra từ các trang cụ thể
                        List<dynamic> list_split = new List<dynamic>();
                        // Duyệt qua `selectedItems` để lấy giá trị trang cụ thể
                        foreach (ListItem selectedItem in selectedPage)
                        {
                            // Chuyển đổi giá trị của `ListItem` thành số trang (int)
                            int pageNumber = int.Parse(selectedItem.Value);

                            // Tính toán vị trí bắt đầu và kết thúc của trang cụ thể trong `list_all`
                            int itemsPerPage = Number_cl.Check_Int(txt_show.Text.Trim()); // Số lượng mục trên mỗi trang
                            int startIndex = (pageNumber - 1) * itemsPerPage; // Chỉ số bắt đầu của trang cụ thể trong `list_all`
                            int endIndex = startIndex + itemsPerPage;

                            // Lọc dữ liệu từ `list_all` cho trang cụ thể
                            var pageData = list_all.Skip(startIndex).Take(itemsPerPage);

                            // Thêm dữ liệu đã lọc vào danh sách `list_xuat`
                            list_split.AddRange(pageData);
                        }
                        list_xuat = list_split;
                    }
                    #endregion

                    foreach (var t in list_xuat)
                    {
                        _cot = 1;
                        // Chỉ lặp qua các mục đã được chọn trong `check_list_excel.Items`
                        foreach (ListItem item in check_list_excel.Items.Cast<ListItem>().Where(item => item.Selected))
                        {
                            string _tencot = item.Value;//lấy tên cột
                            switch (_tencot)
                            {
                                case "id":
                                    worksheet.Cells[_row, _cot].Value = t.id; _cot = _cot + 1;
                                    break;
                                case "noidung":
                                    worksheet.Cells[_row, _cot].Value = t.noidung; _cot = _cot + 1;
                                    break;
                                case "ten_nguoithongbao":
                                    worksheet.Cells[_row, _cot].Value = t.ten_nguoithongbao; _cot = _cot + 1;
                                    break;
                                case "thoigian":
                                    // Giả định t.ngaytao là thuộc tính DateTime hoặc DateTime?
                                    DateTime? ngayTao = t.thoigian;

                                    if (ngayTao.HasValue)
                                    {
                                        // Chuyển đổi DateTime thành chỉ ngày (ngayTao.Value.Date)
                                        DateTime onlyDate = ngayTao.Value.Date;

                                        // Đặt giá trị ô là kiểu DateTime chỉ với ngày
                                        worksheet.Cells[_row, _cot].Value = onlyDate;

                                        // Định dạng số cho ô thành "dd/MM/yyyy"
                                        worksheet.Cells[_row, _cot].Style.Numberformat.Format = "dd/MM/yyyy";
                                    }
                                    else
                                    {
                                        // Nếu giá trị ngayTao là null, bạn có thể để trống ô đó hoặc xử lý theo cách khác
                                        worksheet.Cells[_row, _cot].Value = DBNull.Value; // Hoặc để trống, hoặc đặt giá trị mặc định
                                    }
                                    _cot = _cot + 1;
                                    break;
                                default: break;
                            }
                        }
                        _row++;
                    }
                    // Lưu tệp Excel vào đường dẫn đã chỉ định
                    string filePath = "/uploads/files/ThongBao-" + str_cl.taoma_theothoigian() + ".xlsx";
                    package.SaveAs(new System.IO.FileInfo(Server.MapPath("~" + filePath)));
                    Response.Redirect(filePath);
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành co", "1000", "warning"), true);

                    #region tắt Update Panel thì dùng được. Nó dùng RAM để xử lý, k lưu file trên ổ cứng
                    // Sử dụng MemoryStream để lưu tệp Excel
                    //using (MemoryStream stream = new MemoryStream())
                    //{
                    //    // Lưu tệp Excel vào MemoryStream
                    //    package.SaveAs(stream);

                    //    // Lưu nội dung MemoryStream vào một tệp tạm thời trên máy chủ
                    //    string filePath = Path.Combine(Server.MapPath("~/uploads/files/"), "DanhMuc-" + str_cl.taoma_theothoigian() + ".xlsx");
                    //    File.WriteAllBytes(filePath, stream.ToArray());

                    //    // Đăng ký script JavaScript để tự động tải xuống tệp Excel
                    //    string script = $"window.location.href = '/uploads/files/DanhMuc-{str_cl.taoma_theothoigian()}.xlsx';";
                    //    ScriptManager.RegisterStartupScript(this, GetType(), "DownloadExcel", script, true);
                    //}
                    #endregion
                }
                #endregion

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
    protected void check_all_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            // Kiểm tra trạng thái của checkbox "Chọn tất cả"
            bool isChecked = check_all_excel.Checked;

            // Đặt trạng thái của tất cả các mục trong CheckBoxList theo trạng thái của "Chọn tất cả"
            foreach (ListItem item in check_list_excel.Items)
            {
                item.Selected = isChecked;
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
    protected void check_list_excel_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            // Kiểm tra xem tất cả các mục trong CheckBoxList đã được chọn hay chưa
            bool allSelected = true;

            foreach (ListItem item in check_list_excel.Items)
            {
                if (!item.Selected)
                {
                    allSelected = false;
                    break;
                }
            }

            // Cập nhật trạng thái của check_all_page theo kết quả kiểm tra
            check_all_excel.Checked = allSelected;
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
    protected void check_all_page_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            // Kiểm tra trạng thái của checkbox "Chọn tất cả"
            bool isChecked = check_all_page.Checked;

            // Đặt trạng thái của tất cả các mục trong CheckBoxList theo trạng thái của "Chọn tất cả"
            foreach (ListItem item in check_list_page.Items)
            {
                item.Selected = isChecked;
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
    protected void check_list_page_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            // Kiểm tra xem tất cả các mục trong CheckBoxList đã được chọn hay chưa
            bool allSelected = true;

            foreach (ListItem item in check_list_page.Items)
            {
                if (!item.Selected)
                {
                    allSelected = false;
                    break;
                }
            }

            // Cập nhật trạng thái của check_all_page theo kết quả kiểm tra
            check_all_page.Checked = allSelected;
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

    #region Lọc dữ liệu
    #region chọn ngày nhanh
    private void ResetButtonCss()
    {
        but_homqua.CssClass = "small mt-1 light";
        but_homnay.CssClass = "small mt-1 light";
        but_tuantruoc.CssClass = "small mt-1 light";
        but_tuannay.CssClass = "small mt-1 light";
        but_thangtruoc.CssClass = "small mt-1 light";
        but_thangnay.CssClass = "small mt-1 light";
        but_quytruoc.CssClass = "small mt-1 light";
        but_quynay.CssClass = "small mt-1 light";
        but_namtruoc.CssClass = "small mt-1 light";
        but_namnay.CssClass = "small mt-1 light";
    }
    protected void but_homqua_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_homqua.CssClass = "small mt-1 info";
        txt_tungay.Text = AhaTime_cl.Now.Date.AddDays(-1).ToShortDateString();
        txt_denngay.Text = AhaTime_cl.Now.Date.AddDays(-1).ToShortDateString();
    }
    protected void but_homnay_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_homnay.CssClass = "small mt-1 info";
        txt_tungay.Text = AhaTime_cl.Now.Date.ToString();
        txt_denngay.Text = AhaTime_cl.Now.Date.ToString();
    }
    protected void but_tuantruoc_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_tuantruoc.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydautuan().AddDays(-7).ToShortDateString();//lấy ngày đầu tuần
        txt_denngay.Text = dt_cl.return_ngaydautuan().AddDays(-1).ToShortDateString();
    }
    protected void but_tuannay_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_tuannay.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydautuan().ToShortDateString();//lấy ngày đầu tuần
        txt_denngay.Text = dt_cl.return_ngaycuoituan().ToShortDateString();
    }
    protected void but_thangtruoc_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_thangtruoc.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydauthangtruoc(AhaTime_cl.Now.Month.ToString(), AhaTime_cl.Now.Year.ToString()).ToShortDateString();
        txt_denngay.Text = dt_cl.return_ngaycuoithangtruoc(AhaTime_cl.Now.Month.ToString(), AhaTime_cl.Now.Year.ToString()).ToShortDateString();
    }
    protected void but_thangnay_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_thangnay.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydauthang(AhaTime_cl.Now.Month.ToString(), AhaTime_cl.Now.Year.ToString()).ToShortDateString();
        txt_denngay.Text = dt_cl.return_ngaycuoithang(AhaTime_cl.Now.Month.ToString(), AhaTime_cl.Now.Year.ToString()).ToShortDateString();
    }
    protected void but_namtruoc_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_namtruoc.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydaunamtruoc(AhaTime_cl.Now.Year.ToString()).ToShortDateString();
        txt_denngay.Text = dt_cl.return_ngaycuoinamtruoc(AhaTime_cl.Now.Year.ToString()).ToShortDateString();
    }
    protected void but_namnay_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_namnay.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydaunam(AhaTime_cl.Now.Year.ToString()).ToShortDateString();
        txt_denngay.Text = dt_cl.return_ngaycuoinam(AhaTime_cl.Now.Year.ToString()).ToShortDateString();
    }
    protected void but_quytruoc_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_quytruoc.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydauquytruoc(AhaTime_cl.Now.Month.ToString(), AhaTime_cl.Now.Year.ToString()).ToShortDateString();
        txt_denngay.Text = dt_cl.return_ngaycuoiquytruoc(AhaTime_cl.Now.Month.ToString(), AhaTime_cl.Now.Year.ToString()).ToShortDateString();
    }
    protected void but_quynay_Click(object sender, EventArgs e)
    {
        ResetButtonCss(); but_quynay.CssClass = "small mt-1 info";
        txt_tungay.Text = dt_cl.return_ngaydauquynay(AhaTime_cl.Now.Month.ToString(), AhaTime_cl.Now.Year.ToString()).ToShortDateString();
        txt_denngay.Text = dt_cl.return_ngaycuoiquynay(AhaTime_cl.Now.Month.ToString(), AhaTime_cl.Now.Year.ToString()).ToShortDateString();
    }
    #endregion
    protected void but_show_form_loc_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            string view = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
            if (view == ViewFilter)
                RedirectTo(BuildListUrl());
            else
                RedirectTo(BuildFilterUrl());
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
    protected void but_loc_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            if (Request.Cookies["cookie_qltb"] != null)//nếu có ck r thì lưu giá trị mới
            {
                HttpCookie _ck = Request.Cookies["cookie_qltb"];
                _ck["show"] = txt_show.Text;
                _ck["trang_hientai"] = ViewState["current_page_qltb"].ToString();
                _ck["id_loctheothoigian"] = ddl_thoigian.SelectedValue;
                _ck["tungay"] = txt_tungay.Text;
                _ck["denngay"] = txt_denngay.Text;
                _ck.Expires = AhaTime_cl.Now.AddDays(1);
                Response.Cookies.Set(_ck); // Cập nhật lại cookie
            }
            show_main();
            up_main.Update();
            RedirectToListWithNotice("Thông báo", "Xử lý thành công.");
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
    protected void but_huy_loc_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            if (Request.Cookies["cookie_qltb"] != null)
                Response.Cookies["cookie_qltb"].Expires = AhaTime_cl.Now.AddYears(-1);
            RedirectTo(BuildListUrl());
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

    #region BIN - XÓA - KHÔI PHỤC - LƯU
    protected void but_remove_bin_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");

            var selectedIds = new List<string>(); // Danh sách để lưu trữ ID của các mục đã được chọn

            // Thu thập tất cả ID của các mục đã được chọn trong Repeater1
            foreach (RepeaterItem item in Repeater1.Items)
            {
                CheckBox chkItem = (CheckBox)item.FindControl("checkID");
                Label lblData = (Label)item.FindControl("lbID");

                if (chkItem != null && lblData != null && chkItem.Checked)
                {
                    string id = lblData.Text;
                    selectedIds.Add(id); // Thêm ID vào danh sách
                }
            }

            if (selectedIds.Count > 0)
            {
                // Sử dụng dbDataContext và thực hiện cập nhật hàng loạt
                using (dbDataContext db = new dbDataContext())
                {
                    // Lấy tất cả các mục có ID trong danh sách và cập nhật thuộc tính `bin` của chúng
                    var danhMucsToUpdate = db.ThongBao_tbs
                        .Where(d => selectedIds.Contains(d.id.ToString()))
                        .ToList();

                    foreach (var dm in danhMucsToUpdate)
                    {
                        dm.bin = true;
                    }

                    // Lưu tất cả các thay đổi trong một lần
                    db.SubmitChanges();
                }

                // Hiển thị thông báo thành công
                show_main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
            else
            {
                // Hiển thị thông báo không có mục nào được chọn
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "2600", "danger"), true);
                
            }
            #region CÁCH CỦ, TRUY VẤN NHIỀU LẦN
            //AdminRolePolicy_cl.RequireSuperAdmin();
            //int _count = 0;
            //using (dbDataContext db = new dbDataContext())
            //{
            //    foreach (RepeaterItem item in Repeater1.Items)
            //    {
            //        CheckBox chkItem = (CheckBox)item.FindControl("checkID");//lấy checkbox
            //        Label lblData = (Label)item.FindControl("lbID");//lấy ID
            //        if (chkItem != null && lblData != null)//đảm bảo có Control
            //        {
            //            if (chkItem.Checked)//nếu checkbox đc chọn
            //            {
            //                string _id = lblData.Text;
            //                // Xử lý các mục đã được chọn ngay sau đây

            //                // Thực hiện các thao tác với ID tại đây
            //                var q = db.DanhMuc_tbs.FirstOrDefault(p => p.id.ToString() == _id);
            //                if (q != null)
            //                {
            //                    DanhMuc_tb _ob = q;
            //                    _ob.bin = true;
            //                    db.SubmitChanges();
            //                    _count = _count + 1;
            //                }
            //            }
            //        }
            //    }
            //}

            //if (_count > 0)
            //{
            //    show_main();
            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            //}
            //else
            //{
            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "2600", "danger"), true);
            //}
            #endregion
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
    protected void but_xoa_vinh_vien_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");

            var selectedIds = new List<string>(); // Danh sách để lưu trữ ID của các mục đã được chọn

            // Thu thập tất cả ID của các mục đã được chọn trong Repeater1
            foreach (RepeaterItem item in Repeater1.Items)
            {
                CheckBox chkItem = (CheckBox)item.FindControl("checkID");
                Label lblData = (Label)item.FindControl("lbID");

                if (chkItem != null && lblData != null && chkItem.Checked)
                {
                    string id = lblData.Text;
                    selectedIds.Add(id); // Thêm ID vào danh sách
                }
            }

            if (selectedIds.Count > 0)
            {
                // Sử dụng dbDataContext và thực hiện cập nhật hàng loạt
                using (dbDataContext db = new dbDataContext())
                {
                    // Lấy tất cả các mục có ID trong danh sách và cập nhật thuộc tính `bin` của chúng
                    var danhMucsToUpdate = db.ThongBao_tbs
                        .Where(d => selectedIds.Contains(d.id.ToString()))
                        .ToList();

                    foreach (var dm in danhMucsToUpdate)
                    {
                        db.ThongBao_tbs.DeleteOnSubmit(dm);
                    }

                    // Lưu tất cả các thay đổi trong một lần
                    db.SubmitChanges();
                }

                // Hiển thị thông báo thành công
                show_main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
            else
            {
                // Hiển thị thông báo không có mục nào được chọn
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "2600", "danger"), true);
                
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
    protected void but_khoiphuc_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");

            var selectedIds = new List<string>(); // Danh sách để lưu trữ ID của các mục đã được chọn

            // Thu thập tất cả ID của các mục đã được chọn trong Repeater1
            foreach (RepeaterItem item in Repeater1.Items)
            {
                CheckBox chkItem = (CheckBox)item.FindControl("checkID");
                Label lblData = (Label)item.FindControl("lbID");

                if (chkItem != null && lblData != null && chkItem.Checked)
                {
                    string id = lblData.Text;
                    selectedIds.Add(id); // Thêm ID vào danh sách
                }
            }

            if (selectedIds.Count > 0)
            {
                // Sử dụng dbDataContext và thực hiện cập nhật hàng loạt
                using (dbDataContext db = new dbDataContext())
                {
                    // Lấy tất cả các mục có ID trong danh sách và cập nhật thuộc tính `bin` của chúng
                    var danhMucsToUpdate = db.ThongBao_tbs
                        .Where(d => selectedIds.Contains(d.id.ToString()))
                        .ToList();

                    foreach (var dm in danhMucsToUpdate)
                    {
                        dm.bin = false;
                    }

                    // Lưu tất cả các thay đổi trong một lần
                    db.SubmitChanges();
                }

                // Hiển thị thông báo thành công
                show_main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
            else
            {
                // Hiển thị thông báo không có mục nào được chọn
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "2600", "danger"), true);
                
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
    protected void but_danhdau_dadoc_nhieumuc_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");

            var selectedIds = new List<string>(); // Danh sách để lưu trữ ID của các mục đã được chọn

            // Thu thập tất cả ID của các mục đã được chọn trong Repeater1
            foreach (RepeaterItem item in Repeater1.Items)
            {
                CheckBox chkItem = (CheckBox)item.FindControl("checkID");
                Label lblData = (Label)item.FindControl("lbID");

                if (chkItem != null && lblData != null && chkItem.Checked)
                {
                    string id = lblData.Text.ToString();
                    selectedIds.Add(id); // Thêm ID vào danh sách
                }
            }

            if (selectedIds.Count > 0)
            {
                // Sử dụng dbDataContext và thực hiện cập nhật hàng loạt
                using (dbDataContext db = new dbDataContext())
                {
                    // Lấy tất cả các mục có ID trong danh sách và cập nhật thuộc tính `bin` của chúng
                    var danhMucsToUpdate = db.ThongBao_tbs
                        .Where(d => selectedIds.Contains(d.id.ToString()))
                        .ToList();

                    foreach (var dm in danhMucsToUpdate)
                    {
                        dm.daxem = true;
                    }

                    // Lưu tất cả các thay đổi trong một lần
                    db.SubmitChanges();
                }

                // Hiển thị thông báo thành công
                show_main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
            else
            {
                // Hiển thị thông báo không có mục nào được chọn
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "2600", "danger"), true);
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
    protected void but_danhdau_chuadoc_nhieumuc_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");

            var selectedIds = new List<string>(); // Danh sách để lưu trữ ID của các mục đã được chọn

            // Thu thập tất cả ID của các mục đã được chọn trong Repeater1
            foreach (RepeaterItem item in Repeater1.Items)
            {
                CheckBox chkItem = (CheckBox)item.FindControl("checkID");
                Label lblData = (Label)item.FindControl("lbID");

                if (chkItem != null && lblData != null && chkItem.Checked)
                {
                    string id = lblData.Text.ToString();
                    selectedIds.Add(id); // Thêm ID vào danh sách
                }
            }

            if (selectedIds.Count > 0)
            {
                // Sử dụng dbDataContext và thực hiện cập nhật hàng loạt
                using (dbDataContext db = new dbDataContext())
                {
                    // Lấy tất cả các mục có ID trong danh sách và cập nhật thuộc tính `bin` của chúng
                    var danhMucsToUpdate = db.ThongBao_tbs
                        .Where(d => selectedIds.Contains(d.id.ToString()))
                        .ToList();

                    foreach (var dm in danhMucsToUpdate)
                    {
                        dm.daxem = false;
                    }

                    // Lưu tất cả các thay đổi trong một lần
                    db.SubmitChanges();
                }

                // Hiển thị thông báo thành công
                show_main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
            else
            {
                // Hiển thị thông báo không có mục nào được chọn
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "2600", "danger"), true);
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
    #endregion

    #region IN
    protected void but_show_form_in_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            string view = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
            if (view == ViewPrint)
                RedirectTo(BuildListUrl());
            else
                RedirectTo(BuildPrintUrl());
        }

        //gọi lệnh in
        //ScriptManager.RegisterStartupScript(this, GetType(), "printPage", "window.print();", true);}
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
    protected void but_close_form_in_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");//reset control
                                                             //ddl_DanhMuc.DataSource = null;
                                                             //ddl_DanhMuc.DataBind();
                                                             //ẩn form
            RedirectTo(BuildListUrl());
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
    protected void but_in_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            RedirectTo("/admin/quan-ly-thong-bao/in.aspx");
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


    #region hàm riêng only
    protected void but_sapxep_moinhat_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            ViewState["sapxep_thongbao"] = "1";
            but_sapxep_moinhat.CssClass = "info small rounded";
            but_sapxep_chuadoc.CssClass = "light small rounded";
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

    protected void but_sapxep_chuadoc_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            ViewState["sapxep_thongbao"] = "2";
            but_sapxep_moinhat.CssClass = "light small rounded";
            but_sapxep_chuadoc.CssClass = "info small rounded";
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
    protected void but_chuadoc_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == taiKhoan && p.bin == false);
                if (q == null)
                    return;
                q.daxem = false;
                db.SubmitChanges();
                show_main();
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

    protected void but_dadoc_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == taiKhoan && p.bin == false);
                if (q == null)
                    return;
                q.daxem = true;
                db.SubmitChanges();
                show_main();
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
    protected void but_xoathongbao_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("notifications", "/admin/default.aspx?mspace=admin");
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == taiKhoan && p.bin == false);
                if (q == null)
                    return;
                q.bin = true;
                db.SubmitChanges();
                show_main();
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
    #endregion



}
