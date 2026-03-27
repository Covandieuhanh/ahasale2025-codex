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

public partial class admin_Default : System.Web.UI.Page
{
    protected void FileBrowser1_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                FileBrowser1 = new CKFinder.FileBrowser();
                FileBrowser1.BasePath = "/ckfinder/";
                FileBrowser1.SetupCKEditor(txt_noidung); 
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
    // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", ""), true);
    DanhMuc_cl dm_cl = new DanhMuc_cl();
    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();
    private const string ViewAdd = "add";
    private const string ViewEdit = "edit";
    private const string ViewFilter = "filter";
    private const string ViewExport = "export";
    private const string ViewPrint = "print";

    private string BuildListUrl()
    {
        return ResolveUrl("~/admin/quan-ly-bai-viet/default.aspx");
    }

    private string BuildViewUrl(string view)
    {
        return BuildListUrl() + "?view=" + view;
    }

    private string BuildEditUrl(string id)
    {
        return BuildListUrl() + "?view=" + ViewEdit + "&id=" + HttpUtility.UrlEncode(id ?? "");
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

    private void HideOtherPanels()
    {
        pn_add.Visible = false;
        pn_loc.Visible = false;
        pn_xuat.Visible = false;
        pn_in.Visible = false;
    }

    private void ShowAddViewFromQuery()
    {
        check_login_cl.check_login_admin("none", "none");
        HideOtherPanels();
        reset_control_add_edit();
        ViewState["add_edit"] = "add";
        Label1.Text = "THÊM BÀI VIẾT MỚI";
        but_add_edit.Text = "THÊM MỚI";
        dm_cl.Show_DanhMuc(1, 0, ddl_DanhMuc, false, "web", "0");
        pn_add.Visible = true;
        up_add.Update();
        up_main.Visible = false;
    }

    private void ShowEditViewFromQuery(string id)
    {
        check_login_cl.check_login_admin("none", "none");
        HideOtherPanels();
        ViewState["add_edit"] = "edit";
        Label1.Text = "CHỈNH SỬA BÀI VIẾT";
        but_add_edit.Text = "CẬP NHẬT";

        using (dbDataContext db = new dbDataContext())
        {
            var q = db.BaiViet_tbs.FirstOrDefault(p => p.id.ToString() == id);
            if (q == null)
            {
                RedirectToListWithNotice("Thông báo", "Không tìm thấy bài viết cần chỉnh sửa.");
                return;
            }

            ViewState["id_edit"] = id;
            dm_cl.Show_DanhMuc(1, 0, ddl_DanhMuc, false, "web", "0");
            DropDownList_cl.Return_Index_By_ID(ddl_DanhMuc, q.id_DanhMuc);
            txt_name.Text = q.name;
            txt_description.Text = q.description;
            txt_link_fileupload.Text = q.image;
            txt_noidung.Text = q.content_post;
            DropDownList1.SelectedValue = q.phanloai;

            if (q.image != "")
            {
                Button2.Visible = true;
                Label2.Text = "<div><small>Ảnh cũ</small></div><img src='" + q.image + "' style='max-width: 100px' />";
            }
            else
            {
                Button2.Visible = false;
                Label2.Text = "";
            }

            string phanLoaiBaiViet = q.phanloai;
            if (phanLoaiBaiViet == "tintuc")
                PlaceHolder3.Visible = false;
            else
            {
                if (phanLoaiBaiViet == "sanpham" || phanLoaiBaiViet == "dichvu")
                {
                    PlaceHolder3.Visible = true;
                    txt_giaban.Text = q.giaban.Value.ToString("#,##0");
                    txt_giavon.Text = q.giavon.Value.ToString("#,##0");
                    txt_thuong_chotsale.Text = q.chotsale_thuong.Value.ToString("#,##0");
                    txt_thuong_banhang.Text = q.banhang_thuong.Value.ToString("#,##0");

                    if (q.chotsale_phantram_hoac_tien == true)
                    {
                        chotsale_phantram.Checked = true;
                        chotsale_tienmat.Checked = false;
                    }
                    else
                    {
                        chotsale_phantram.Checked = false;
                        chotsale_tienmat.Checked = true;
                    }

                    if (q.banhang_phantram_hoac_tien == true)
                    {
                        banhang_phantram.Checked = true;
                        banhang_tienmat.Checked = false;
                    }
                    else
                    {
                        banhang_phantram.Checked = false;
                        banhang_tienmat.Checked = true;
                    }
                }
            }

            Check_NoiBat.Checked = q.noibat == true;
        }

        pn_add.Visible = true;
        up_add.Update();
        up_main.Visible = false;
    }

    private void ShowFilterViewFromQuery()
    {
        check_login_cl.check_login_admin("none", "none");
        HideOtherPanels();
        pn_loc.Visible = true;
        up_loc.Update();
        up_main.Visible = false;
    }

    private void ShowExportViewFromQuery()
    {
        check_login_cl.check_login_admin("none", "none");
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
        check_login_cl.check_login_admin("none", "none");
        HideOtherPanels();
        pn_in.Visible = true;
        up_in.Update();
        up_main.Visible = false;
    }

    private void ApplyOpenViewFromQuery()
    {
        string view = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
        if (view == ViewAdd)
        {
            ShowAddViewFromQuery();
            return;
        }
        if (view == ViewEdit)
        {
            string id = (Request.QueryString["id"] ?? "").Trim();
            if (id == "")
            {
                RedirectToListWithNotice("Thông báo", "Thiếu ID bài viết để chỉnh sửa.");
                return;
            }
            ShowEditViewFromQuery(id);
            return;
        }
        if (view == ViewFilter)
        {
            ShowFilterViewFromQuery();
            return;
        }
        if (view == ViewExport)
        {
            ShowExportViewFromQuery();
            return;
        }
        if (view == ViewPrint)
        {
            ShowPrintViewFromQuery();
        }
    }

    public void set_dulieu_macdinh()
    {
        try
        {
            ResetButtonCss();//button chọn ngày nhanh
            txt_show.Text = "30";
            ViewState["current_page_qlbv"] = "1";
            ViewState["showbin"] = "0";//hiển thị các mục k nằm trong thùng rác, 1: thùng rác

            #region set_get_cookie
            // Lấy cookie "cookie_qlbv" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qlbv"];
            if (cookie == null)
            {
                ListBox1.SelectedIndex = 0;//mặc định chọn tất cả phân loại, nếu select=true ngoài html thì k lưu lịch sử đc, kệ mẹ nó cứ làm y vậy đi, đừng quan tâm tới đoạn này
                                           // Nếu cookie không tồn tại, tạo cookie mới
                cookie = new HttpCookie("cookie_qlbv");
                cookie["show"] = txt_show.Text;//lưu số dòng hiển thị mỗi trang
                cookie["trang_hientai"] = "1";//lưu trang hiện tại
                cookie["id_loctheothoigian"] = ddl_thoigian.SelectedValue;
                cookie["tungay"] = txt_tungay.Text;
                cookie["denngay"] = txt_denngay.Text;
                cookie["phanloai"] = "";//Tất cả phân loại
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
                ViewState["current_page_qlbv"] = cookie["trang_hientai"];
                ddl_thoigian.SelectedValue = cookie["id_loctheothoigian"];
                txt_tungay.Text = cookie["tungay"];
                txt_denngay.Text = cookie["denngay"];
                if (cookie["phanloai"] == "")//nếu phân loại là Tất cả (value = "")
                    ListBox1.SelectedIndex = 0;//Chọn mục Tất cả
                else
                {
                    // Chọn các mục tương ứng với giá trị đã lưu
                    string[] _chon_phanloai = cookie["phanloai"].Split(',');
                    foreach (ListItem item in ListBox1.Items)
                    {
                        if (_chon_phanloai.Contains(item.Value))
                            item.Selected = true;
                    }
                }
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
                check_login_cl.check_login_admin("none", "none");


                //Nó k kịp lưu vì nó tải trang này trước khi load menu-left
                //if (Session["title"] != null)
                //    ViewState["title"] = Session["title"].ToString();

                set_dulieu_macdinh();
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
            var dataItem = (dynamic)e.Item.DataItem;
            

            // Tìm CheckBox1 và thiết lập Checked nếu là nổi bật
            var checkBox = (CheckBox)e.Item.FindControl("CheckBox1");
            if (checkBox != null)
            {
                checkBox.Checked = dataItem.noibat;
                //hoặc
                // Lấy giá trị cần so sánh từ DataItem (sửa 'ten_field' thành tên trường dữ liệu phù hợp)
                //string valueToCompare = Convert.ToString(DataBinder.Eval(dataItem, "Tên Cột")) ?? string.Empty;
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
    public void show_main()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                #region lấy dữ liệu
                //var list_all = (from ob1 in db.BaiViet_tbs
                //                join ob2 in db.DanhMuc_tbs on ob1.id_DanhMuc equals ob2.id.ToString()
                //                select new
                //                {
                //                    ob1.id,
                //                    ob1.name,
                //                    ob1.name_en,
                //                    ob1.ngaytao,
                //                    ob1.bin,
                //                    ob1.phanloai,
                //                    ob1.noibat,
                //                    TenMenu = ob2.name,
                //                }).AsQueryable();
                var list_all = (from ob1 in db.BaiViet_tbs
                                join ob2 in db.DanhMuc_tbs on ob1.id_DanhMuc equals ob2.id.ToString() into danhMucGroup
                                from ob2 in danhMucGroup.DefaultIfEmpty()
                                select new
                                {
                                    ob1.id,
                                    ob1.name,
                                    ob1.name_en,
                                    ob1.ngaytao,
                                    ob1.bin,
                                    ob1.phanloai,
                                    ob1.noibat,
                                    TenMenu = ob2 != null ? ob2.name : null  // Trả về null nếu không có danh mục
                                }).AsQueryable();


                //hiển thị các mục đã xóa hoặc không
                if (ViewState["showbin"].ToString() == "1")
                    list_all = list_all.Where(p => p.bin == true);
                else
                    list_all = list_all.Where(p => p.bin == false);

                // Kiểm tra xem textbox có dữ liệu tìm kiếm không
                string _key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                    list_all = list_all.Where(p => p.name.Contains(_key) || p.name_en.Contains(_key) || p.id.ToString() == _key);
                else
                {
                    string _key1 = txt_timkiem1.Text.Trim();
                    if (!string.IsNullOrEmpty(_key1))
                        list_all = list_all.Where(p => p.name.Contains(_key1) || p.name_en.Contains(_key) || p.id.ToString() == _key1);
                }

                //xử lý theo thời gian
                string _id_locthoigian = ddl_thoigian.SelectedValue;
                if (_id_locthoigian == "1")//lọc theo ngày tạo
                {
                    if (txt_tungay.Text != "")
                        list_all = list_all.Where(p => p.ngaytao.Value.Date >= DateTime.Parse(txt_tungay.Text).Date);
                    if (txt_denngay.Text != "")
                        list_all = list_all.Where(p => p.ngaytao.Value.Date <= DateTime.Parse(txt_denngay.Text).Date);
                }

                //lọc theo phân loại bài viết
                List<string> list_phanloai_baiviet = new List<string>();
                foreach (ListItem item in ListBox1.Items)
                {
                    if (item.Selected)
                        list_phanloai_baiviet.Add(item.Value);
                }
                if (!list_phanloai_baiviet.Contains(""))//nếu tồn tại "": tất cả thì k lọc
                    list_all = list_all.Where(tk => list_phanloai_baiviet.Contains(tk.phanloai));

                //sắp xếp
                list_all = list_all.OrderByDescending(p => p.ngaytao);
                int _Tong_Record = list_all.Count();
                #endregion

                #region phân trang OK, k sửa
                // Xử lý số record mỗi trang
                int show = Number_cl.Check_Int(txt_show.Text.Trim()); if (show <= 0) show = 30;
                //xử lý trang hiện tại. Đảm bảo current_page không nhỏ hơn 1 và không lớn hơn total_page
                int current_page = int.Parse(ViewState["current_page_qlbv"].ToString()); int total_page = number_of_page_class.return_total_page(_Tong_Record, show); if (current_page < 1) current_page = 1; else if (current_page > total_page) current_page = total_page;
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
            ViewState["current_page_qlbv"] = int.Parse(ViewState["current_page_qlbv"].ToString()) - 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_qlbv" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qlbv"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qlbv"].ToString();
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
            ViewState["current_page_qlbv"] = int.Parse(ViewState["current_page_qlbv"].ToString()) + 1;
            #region LƯU TRANG HIỆN TẠI
            // Lấy cookie "cookie_qlbv" từ Request.Cookies
            HttpCookie cookie = Request.Cookies["cookie_qlbv"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qlbv"].ToString();
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
            ViewState["current_page_qlbv"] = 1;
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
            check_login_cl.check_login_admin("none", "none");
            ViewState["showbin"] = "1";//đánh dấu hiển thị thùng rác
            but_remove_bin.Visible = false;//ẨN nút Di chuyển vào thùng rác
            but_khoiphuc.Visible = true;//HIỆN nút xóa vĩnh viễn và khôi phục
            but_show_thungrac.Visible = false;//ẨN nút Xem thùng rác
            but_show_main.Visible = true;//HIỆN nút về trang chính (xem mục k nằm trong thùng rác)
            but_quayve_trangchu.Visible = true;//HIỆN nút về trang chính (xem mục k nằm trong thùng rác)

            ViewState["current_page_qlbv"] = 1;
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
    //but_show_main và but_quayve_trangchu giống nhau
    protected void but_show_main_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["showbin"] = "0";//đánh dấu hiển thị mục chưa xóa
            but_remove_bin.Visible = true;//ẨN nút Di chuyển vào thùng rác
            but_khoiphuc.Visible = false;//HIỆN nút xóa vĩnh viễn và khôi phục
            but_show_thungrac.Visible = true;//ẨN nút Xem thùng rác
            but_show_main.Visible = false;//HIỆN nút về trang chính (xem mục k nằm trong thùng rác)
            but_quayve_trangchu.Visible = false;//HIỆN nút về trang chính (xem mục k nằm trong thùng rác)
            ViewState["current_page_qlbv"] = 1;
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
    protected void but_quayve_trangchu_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["showbin"] = "0";//đánh dấu hiển thị mục chưa xóa
            but_remove_bin.Visible = true;//ẨN nút Di chuyển vào thùng rác
            but_khoiphuc.Visible = false;//HIỆN nút xóa vĩnh viễn và khôi phục
            but_show_thungrac.Visible = true;//ẨN nút Xem thùng rác
            but_show_main.Visible = false;//HIỆN nút về trang chính (xem mục k nằm trong thùng rác)
            but_quayve_trangchu.Visible = false;//HIỆN nút về trang chính (xem mục k nằm trong thùng rác)
            ViewState["current_page_qlbv"] = 1;
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
        try
        {
            Label1.Text = null; Check_NoiBat.Checked = false;
            PlaceHolder3.Visible = false;
            txt_name.Text = ""; txt_giaban.Text = "0"; txt_giavon.Text = "0"; txt_description.Text = "";txt_noidung.Text = "";
            Label2.Text = ""; Button2.Visible = false;
            chotsale_phantram.Checked = true;
            chotsale_tienmat.Checked = false;
            banhang_phantram.Checked = true;
            banhang_tienmat.Checked = false;
            txt_thuong_banhang.Text = "0"; txt_thuong_chotsale.Text = "0";
            DropDownList1.SelectedIndex = 0;
            ddl_DanhMuc.DataSource = null;
            ddl_DanhMuc.DataBind();
            ViewState["add_edit"] = null;
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
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            string _phanloai = DropDownList1.SelectedValue;
            if (_phanloai == "tintuc")
                PlaceHolder3.Visible = false;
            else
                PlaceHolder3.Visible = true;
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
    protected void but_show_form_add_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            RedirectTo(BuildViewUrl(ViewAdd));
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
    protected void but_close_form_add_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            //reset control
            reset_control_add_edit();
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

    //chỉnh sửa
    protected void Button2_Click(object sender, EventArgs e)//xóa ảnh cũ
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            using (dbDataContext db = new dbDataContext())
            {
                var q = db.BaiViet_tbs.FirstOrDefault(p => p.id.ToString() == ViewState["id_edit"].ToString());
                if (q != null)
                {
                    BaiViet_tb _ob = q;
                    File_Folder_cl.del_file(_ob.image);//xóa ảnh cũ nếu có
                    _ob.image = "";
                    Button2.Visible = false;
                    db.SubmitChanges();
                    Label2.Text = ""; txt_link_fileupload.Text = "";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa ảnh thành công.", "1000", "warning"), true);
                }
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
    protected void but_show_chinhsua_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            LinkButton button = (LinkButton)sender;
            string id = button.CommandArgument ?? "";
            RedirectTo(BuildEditUrl(id));
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
    protected void but_close_chinhsua_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            //reset control
            reset_control_add_edit();
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
    //code chung. add hoặc update

    protected void but_add_edit_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            #region Chuẩn bị dữ liệu
            //xác định loại bài viết
            string _phanloai_baiviet = DropDownList1.SelectedValue;
            //đảm bảo luôn có thư mục chứa ảnh
            if (!Directory.Exists(Server.MapPath("~/uploads/img-handler/"))) Directory.CreateDirectory(Server.MapPath("~/uploads/img-handler/"));
            //xử lý dữ liệu đầu vào
            string _name = str_cl.Remove_Blank(txt_name.Text.Trim());
            string _name_en = str_cl.replace_name_to_url(_name);
            string _idmenu = ddl_DanhMuc.SelectedValue.ToString();//giá trị đầu là ""
            string _description = txt_description.Text.Trim();
            string _noidung = txt_noidung.Text.Trim();
            string _image = txt_link_fileupload.Text;
            bool _bin = false;
            DateTime _ngaytao = AhaTime_cl.Now;
            string _nguoitao = mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString());
            bool _noibat = Check_NoiBat.Checked == true ? true : false;

            Int64 _giaban = 0, _giavon = 0;
            bool _chotsale_phantram_hoac_tien = true, _banhang_phantram_hoac_tien = true;
            Int64 _chotsale_thuong = 0, _banhang_thuong = 0;

            if (_phanloai_baiviet == "sanpham" || _phanloai_baiviet == "dichvu")
            {
                _giaban = Number_cl.Check_Int64(txt_giaban.Text.Trim());
                _giavon = Number_cl.Check_Int64(txt_giavon.Text.Trim());
                _chotsale_thuong = Number_cl.Check_Int64(txt_thuong_chotsale.Text.Trim());
                _banhang_thuong = Number_cl.Check_Int64(txt_thuong_banhang.Text.Trim());
                if (_giaban < 0) _giaban = 0;
                if (_giavon < 0) _giavon = 0;
                if (_chotsale_thuong < 0) _chotsale_thuong = 0;
                if (_banhang_thuong < 0) _banhang_thuong = 0;

                if (chotsale_phantram.Checked)//nếu thưởng chốt sale được tính bằng phần trăm, thì k đc lớn hơn 100%
                {
                    _chotsale_phantram_hoac_tien = true;
                    if (_chotsale_thuong > 100)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Phần trăm thưởng chốt sale không được lớn hơn 100.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }
                }
                else
                {
                    _chotsale_phantram_hoac_tien = false;
                }

                if (banhang_phantram.Checked)
                {
                    _banhang_phantram_hoac_tien = true;
                    if (_banhang_thuong > 100)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Phần trăm thưởng bán hàng không được lớn hơn 100.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }
                }
                else
                    _banhang_phantram_hoac_tien = false;
            }


            #endregion
            using (dbDataContext db = new dbDataContext())
            {
                #region Kiểm tra ngoại lệ.


                if (_idmenu == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn Menu.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (_name == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tên bài viết.", "false", "false", "OK", "alert", ""), true);
                    return;
                }


                #endregion

                if (ViewState["add_edit"].ToString() == "add")
                {
                    #region thêm mới
                    BaiViet_tb _ob = new BaiViet_tb();
                    _ob.name = _name;
                    _ob.name_en = _name_en;
                    _ob.id_DanhMuc = _idmenu;
                    _ob.content_post = _noidung;
                    _ob.description = _description;
                    _ob.image = _image;
                    _ob.bin = _bin;
                    _ob.ngaytao = _ngaytao;
                    _ob.nguoitao = _nguoitao;
                    _ob.noibat = _noibat;
                    _ob.giaban = _giaban;
                    _ob.giavon = _giavon;
                    _ob.soluong_tonkho = 0;
                    _ob.banhang_thuong = _banhang_thuong;
                    _ob.chotsale_thuong = _chotsale_thuong;
                    _ob.phanloai = _phanloai_baiviet;
                    _ob.banhang_phantram_hoac_tien = _banhang_phantram_hoac_tien;
                    _ob.chotsale_phantram_hoac_tien = _chotsale_phantram_hoac_tien;
                    db.BaiViet_tbs.InsertOnSubmit(_ob);
                    db.SubmitChanges();
                    #endregion
                    #region cập nhật dữ liệu và update hiển thị
                    //reset 1 vài control để việc tiếp tục nhập (nếu muốn nhập tiếp) nhanh hơn
                    txt_name.Text = ""; txt_giaban.Text = "0"; txt_giavon.Text = "0"; txt_description.Text = ""; txt_link_fileupload.Text = ""; txt_noidung.Text = "";
                    txt_thuong_chotsale.Text = "0"; txt_thuong_banhang.Text = "0";
                    RedirectToListWithNotice("Thông báo", "Xử lý thành công.");
                    #endregion
                }
                else//edit
                {
                    #region Chuẩn bị dữ liệu
                    var q_edit = db.BaiViet_tbs.FirstOrDefault(p => p.id.ToString() == ViewState["id_edit"].ToString());
                    if (q_edit != null)
                    {
                        _bin = q_edit.bin.Value;
                        #region Kiểm tra ngoại lệ. Sau đó cập nhật


                        BaiViet_tb _ob = q_edit;
                        _ob.name = _name;
                        _ob.name_en = _name_en;
                        _ob.id_DanhMuc = _idmenu;
                        _ob.content_post = _noidung;
                        _ob.description = _description;
                        _ob.image = _image;
                        _ob.bin = _bin;
                        _ob.ngaytao = _ngaytao;
                        _ob.nguoitao = _nguoitao;
                        _ob.noibat = _noibat;
                        _ob.giaban = _giaban;
                        _ob.giavon = _giavon;
                        _ob.soluong_tonkho = 0;
                        _ob.banhang_thuong = _banhang_thuong;
                        _ob.chotsale_thuong = _chotsale_thuong;
                        _ob.phanloai = _phanloai_baiviet;
                        _ob.banhang_phantram_hoac_tien = _banhang_phantram_hoac_tien;
                        _ob.chotsale_phantram_hoac_tien = _chotsale_phantram_hoac_tien;
                        db.SubmitChanges();


                        #region cập nhật dữ liệu và update hiển thị

                        //reset control
                        reset_control_add_edit();
                        RedirectToListWithNotice("Thông báo", "Xử lý thành công.");
                        #endregion

                        #endregion
                    }
                    #endregion
                }
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

    #region Xuất excel
    protected void but_show_form_xuat_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            string view = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
            if (view == ViewExport)
                RedirectTo(BuildListUrl());
            else
                RedirectTo(BuildViewUrl(ViewExport));
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
            check_login_cl.check_login_admin("none", "none");
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
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không có mục nào được chọn.", "false", "false", "OK", "alert", ""), true);
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
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không có trang nào được chọn.", "false", "false", "OK", "alert", ""), true);
                return; // Kết thúc sớm nếu không có mục nào được chọn
            }

            if (!Directory.Exists(Server.MapPath("~/uploads/files/")))
                Directory.CreateDirectory(Server.MapPath("~/uploads/files/"));
            using (dbDataContext db = new dbDataContext())
            {
                #region lấy dữ liệu
                var list_all = (from ob1 in db.BaiViet_tbs
                                join ob2 in db.DanhMuc_tbs on ob1.id_DanhMuc equals ob2.id.ToString()
                                select new
                                {
                                    ob1.id,
                                    ob1.name,
                                    ob1.name_en,
                                    ob1.ngaytao,
                                    ob1.bin,
                                    ob1.phanloai,
                                    ob1.noibat,
                                    TenMenu = ob2.name,
                                }).AsQueryable();

                //hiển thị các mục đã xóa hoặc không
                if (ViewState["showbin"].ToString() == "1")
                    list_all = list_all.Where(p => p.bin == true);
                else
                    list_all = list_all.Where(p => p.bin == false);

                // Kiểm tra xem textbox có dữ liệu tìm kiếm không
                string _key = txt_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(_key))
                    list_all = list_all.Where(p => p.name.Contains(_key) || p.name_en.Contains(_key) || p.id.ToString() == _key);
                else
                {
                    string _key1 = txt_timkiem1.Text.Trim();
                    if (!string.IsNullOrEmpty(_key1))
                        list_all = list_all.Where(p => p.name.Contains(_key1) || p.id.ToString() == _key1);
                }

                //xử lý theo thời gian
                string _id_locthoigian = ddl_thoigian.SelectedValue;
                if (_id_locthoigian == "1")//lọc theo ngày tạo
                {
                    if (txt_tungay.Text != "")
                        list_all = list_all.Where(p => p.ngaytao.Value.Date >= DateTime.Parse(txt_tungay.Text).Date);
                    if (txt_denngay.Text != "")
                        list_all = list_all.Where(p => p.ngaytao.Value.Date <= DateTime.Parse(txt_denngay.Text).Date);
                }

                //sắp xếp
                list_all = list_all.OrderByDescending(p => p.ngaytao);
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
                                case "name":
                                    worksheet.Cells[_row, _cot].Value = t.name; _cot = _cot + 1;
                                    break;
                                case "TenMenu":
                                    worksheet.Cells[_row, _cot].Value = t.TenMenu; _cot = _cot + 1;
                                    break;
                                case "noibat":
                                    worksheet.Cells[_row, _cot].Value = t.noibat; _cot = _cot + 1;
                                    break;
                                case "phanloai":
                                    worksheet.Cells[_row, _cot].Value = t.phanloai; _cot = _cot + 1;
                                    break;
                                case "ngaytao":
                                    // Giả định t.ngaytao là thuộc tính DateTime hoặc DateTime?
                                    DateTime? ngayTao = t.ngaytao;

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
                    string filePath = "/uploads/files/Bai-Viet-" + str_cl.taoma_theothoigian() + ".xlsx";
                    package.SaveAs(new System.IO.FileInfo(Server.MapPath("~" + filePath)));
                    //Response.Redirect(filePath);

                    // URL bạn muốn chuyển hướng đến
                    string url = filePath;
                    // Script để mở trang mới trong tab mới
                    string script = string.Format("window.open('{0}', '_blank');", url);
                    // Đăng ký script để thực thi sau khi UpdatePanel postback hoàn thành
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenNewTab", script, true);


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
            check_login_cl.check_login_admin("none", "none");
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
            check_login_cl.check_login_admin("none", "none");
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
            check_login_cl.check_login_admin("none", "none");
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
            check_login_cl.check_login_admin("none", "none");
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
            string view = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
            if (view == ViewFilter)
                RedirectTo(BuildListUrl());
            else
                RedirectTo(BuildViewUrl(ViewFilter));
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
            check_login_cl.check_login_admin("none", "none");
            if (Request.Cookies["cookie_qlbv"] != null)//nếu có ck r thì lưu giá trị mới
            {
                HttpCookie _ck = Request.Cookies["cookie_qlbv"];
                _ck["show"] = txt_show.Text;
                _ck["trang_hientai"] = ViewState["current_page_qlbv"].ToString();
                _ck["id_loctheothoigian"] = ddl_thoigian.SelectedValue;
                _ck["tungay"] = txt_tungay.Text;
                _ck["denngay"] = txt_denngay.Text;
                #region lưu giá trị Phân loại
                List<string> _chon_phanloai = new List<string>();
                foreach (ListItem item in ListBox1.Items)
                {
                    if (item.Selected)
                    {
                        _chon_phanloai.Add(item.Value);
                    }
                }
                // Kiểm tra nếu "Tất cả" được chọn
                if (_chon_phanloai.Contains(""))//Tất cả được chọn
                    _ck["phanloai"] = "";//Tất cả phân loại
                else
                    _ck["phanloai"] = string.Join(",", _chon_phanloai);//Ví dụ: Sản phẩm,Tin tức nếu chọn Sản phẩm và Tin tức
                #endregion
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
            check_login_cl.check_login_admin("none", "none");
            if (Request.Cookies["cookie_qlbv"] != null)
                Response.Cookies["cookie_qlbv"].Expires = AhaTime_cl.Now.AddYears(-1);
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
            check_login_cl.check_login_admin("none", "none");

            var selectedIds = new List<int>(); // Danh sách để lưu trữ ID của các mục đã được chọn

            // Thu thập tất cả ID của các mục đã được chọn trong Repeater1
            foreach (RepeaterItem item in Repeater1.Items)
            {
                CheckBox chkItem = (CheckBox)item.FindControl("checkID");
                Label lblData = (Label)item.FindControl("lbID");

                if (chkItem != null && lblData != null && chkItem.Checked)
                {
                    int id = int.Parse(lblData.Text);
                    selectedIds.Add(id); // Thêm ID vào danh sách
                }
            }

            if (selectedIds.Count > 0)
            {
                // Sử dụng dbDataContext và thực hiện cập nhật hàng loạt
                using (dbDataContext db = new dbDataContext())
                {
                    // Lấy tất cả các mục có ID trong danh sách và cập nhật thuộc tính `bin` của chúng
                    var ListsToUpdate = db.BaiViet_tbs
                        .Where(d => selectedIds.Contains(d.id))
                        .ToList();

                    foreach (var dm in ListsToUpdate)
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
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không có trang nào được chọn.", "false", "false", "OK", "alert", ""), true);
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
    protected void but_xoa_vinh_vien_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");

            var selectedIds = new List<int>(); // Danh sách để lưu trữ ID của các mục đã được chọn

            // Thu thập tất cả ID của các mục đã được chọn trong Repeater1
            foreach (RepeaterItem item in Repeater1.Items)
            {
                CheckBox chkItem = (CheckBox)item.FindControl("checkID");
                Label lblData = (Label)item.FindControl("lbID");

                if (chkItem != null && lblData != null && chkItem.Checked)
                {
                    int id = int.Parse(lblData.Text);
                    selectedIds.Add(id); // Thêm ID vào danh sách
                }
            }

            if (selectedIds.Count > 0)
            {
                // Sử dụng dbDataContext và thực hiện cập nhật hàng loạt
                using (dbDataContext db = new dbDataContext())
                {
                    // Lấy tất cả các mục có ID trong danh sách và cập nhật thuộc tính `bin` của chúng
                    var ListsToUpdate = db.BaiViet_tbs
                        .Where(d => selectedIds.Contains(d.id))
                        .ToList();

                    foreach (var dm in ListsToUpdate)
                    {
                        db.BaiViet_tbs.DeleteOnSubmit(dm);
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
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không có trang nào được chọn.", "false", "false", "OK", "alert", ""), true);
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
            check_login_cl.check_login_admin("none", "none");

            var selectedIds = new List<int>(); // Danh sách để lưu trữ ID của các mục đã được chọn

            // Thu thập tất cả ID của các mục đã được chọn trong Repeater1
            foreach (RepeaterItem item in Repeater1.Items)
            {
                CheckBox chkItem = (CheckBox)item.FindControl("checkID");
                Label lblData = (Label)item.FindControl("lbID");

                if (chkItem != null && lblData != null && chkItem.Checked)
                {
                    int id = int.Parse(lblData.Text);
                    selectedIds.Add(id); // Thêm ID vào danh sách
                }
            }

            if (selectedIds.Count > 0)
            {
                // Sử dụng dbDataContext và thực hiện cập nhật hàng loạt
                using (dbDataContext db = new dbDataContext())
                {
                    // Lấy tất cả các mục có ID trong danh sách và cập nhật thuộc tính `bin` của chúng
                    var ListsToUpdate = db.BaiViet_tbs
                        .Where(d => selectedIds.Contains(d.id))
                        .ToList();

                    foreach (var dm in ListsToUpdate)
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
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Không có trang nào được chọn.", "false", "false", "OK", "alert", ""), true);
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
    protected void but_save_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            // Tạo một danh sách để lưu trữ các cập nhật cần thực hiện
            var updates = new List<(int id, bool noibat)>();
            // Lấy thông tin từ Repeater1
            foreach (RepeaterItem item in Repeater1.Items)
            {
                // Tìm các điều khiển TextBox và Label từ RepeaterItem
                CheckBox CheckBox1 = (CheckBox)item.FindControl("CheckBox1");
                Label lblData = (Label)item.FindControl("lbID");

                // Kiểm tra nếu cả TextBox và Label không null
                if (CheckBox1 != null && lblData != null)
                {
                    // Lấy ID và rank từ các điều khiển
                    string _id = lblData.Text;
                    bool _check_noibat = CheckBox1.Checked;

                    // Thêm thông tin vào danh sách cập nhật
                    updates.Add((id: int.Parse(_id), noibat: _check_noibat));
                }
            }
            // Cập nhật cơ sở dữ liệu một cách hàng loạt
            using (dbDataContext db = new dbDataContext())
            {
                // Truy vấn và lấy tất cả các mục cần cập nhật trong một lần
                var itemsToUpdate = db.BaiViet_tbs
                    .Where(d => updates.Select(u => u.id).Contains(d.id))
                    .ToList();

                // Cập nhật giá trị rank cho tất cả các mục trong danh sách danhMucsToUpdate
                foreach (var dm in itemsToUpdate)
                {
                    var update = updates.First(u => u.id == dm.id);
                    dm.noibat = update.noibat;
                }

                // Lưu các thay đổi vào cơ sở dữ liệu một lần
                db.SubmitChanges();
            }
            show_main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
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
        //using (dbDataContext db = new dbDataContext())
        //{
        //    foreach (RepeaterItem item in Repeater1.Items)
        //    {
        //        TextBox txt_giaban_1 = (TextBox)item.FindControl("txt_giaban_1");//tìm txt_name
        //        Label lblData = (Label)item.FindControl("lbID");//tìm ID
        //        if (txt_giaban_1 != null && lblData != null)//đảm bảo có Control
        //        {
        //            string _id = lblData.Text;//lấy được ID
        //            string _giaban = txt_giaban_1.Text.Replace(".", "");
        //            if (!string.IsNullOrEmpty(_giaban))//có dữ liệu mới xử lý
        //            {
        //                // Thực hiện các thao tác với ID tại đây
        //                var q = db.DanhMuc_tbs.FirstOrDefault(p => p.id.ToString() == _id);
        //                if (q != null)
        //                {
        //                    int _r1 = Number_cl.Check_Int(_giaban);
        //                    if (_r1 > 0)
        //                    {
        //                        DanhMuc_tb _ob = q;
        //                        _ob.rank = _r1;
        //                    }
        //                }
        //            }
        //        }
        //        db.SubmitChanges();
        //    }
        //}
        //show_main();
        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
    }
    //protected void but_xoa_vinh_vien_only_Click(object sender, EventArgs e)
    //{
    //    //demo CommandArgument
    //    //check_login_cl.check_login_admin("none", "none");
    //    //LinkButton button = (LinkButton)sender;
    //    //string _id = button.CommandArgument;
    //}
    #endregion

    #region IN
    protected void but_show_form_in_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            string view = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
            if (view == ViewPrint)
                RedirectTo(BuildListUrl());
            else
                RedirectTo(BuildViewUrl(ViewPrint));
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
    protected void but_close_form_in_Click(object sender, EventArgs e)
    {
        try
        {
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
            // URL bạn muốn chuyển hướng đến
            string url = "/admin/quan-ly-bai-viet/in.aspx";

            // Script để mở trang mới trong tab mới
            string script = string.Format("window.open('{0}', '_blank');", url);

            // Đăng ký script để thực thi sau khi UpdatePanel postback hoàn thành
            ScriptManager.RegisterStartupScript(this, GetType(), "OpenNewTab", script, true);
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
