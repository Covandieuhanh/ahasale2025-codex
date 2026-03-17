using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_cai_dat_trang_chu_Default : System.Web.UI.Page
{// ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", ""), true);
    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa ảnh thành công.", "1000", "warning"), true);
    DateTime_cl dt_cl = new DateTime_cl();

    private string GetCurrentAdminUser()
    {
        string tkEnc = Session["taikhoan"] as string;
        if (string.IsNullOrEmpty(tkEnc))
            return "";

        try { return mahoa_cl.giaima_Bcorn(tkEnc); }
        catch { return tkEnc; }
    }

    private bool EnsureRootAdminAccess()
    {
        string tk = GetCurrentAdminUser();
        if (PermissionProfile_cl.IsRootAdmin(tk))
            return true;

        Session["thongbao"] = thongbao_class.metro_notifi_onload(
            "Thông báo",
            "Chỉ tài khoản admin gốc mới được truy cập Trang chủ Home.",
            "1500",
            "warning");
        Response.Redirect("/admin/default.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
        return false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("none", "none");
        if (!EnsureRootAdminAccess())
            return;

        if (!IsPostBack)
        {
            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            try
            {
                //Nó k kịp lưu vì nó tải trang này trước khi load menu-left
                //if (Session["title"] != null)
                //    ViewState["title"] = Session["title"].ToString();           
                using (dbDataContext db = new dbDataContext())
                {
                    load_data_all(db);
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
    }
    public void load_data_all(dbDataContext db)
    {
        load_data_baotri(db);
        load_data_lienket_chiase(db);
        load_data_nhungma(db);
        load_data_lienketngoai(db);
        load_data_thongtinkhac(db);
    }
    #region LIÊN KẾT CHIA SẺ
    public void load_anh_lienket_chiase(string _img, string _url)
    {
        try
        {
            if (!string.IsNullOrEmpty(_img) && File.Exists(_url)) //nếu có lưu thông tin file và file này tồn tại trên máy chủ
            {
                TextBox1.Text = _img;//textbox ẩn để chứa link ảnh /uploads/....
                Button11.Visible = true;
                Label1.Text = "<img class='img-cover-vuong' style='max-width:100px;max-height:100px' src='" + _img + "' />";
            }
            else
            {
                TextBox1.Text = "";//textbox ẩn để chứa link ảnh /uploads/....
                Button11.Visible = false;
                Label1.Text = "";
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
    public void load_data_lienket_chiase(dbDataContext db)
    {
        try
        {
            var q_lienket_chiase = db.CaiDatChung_tbs.Where(p => p.phanloai_trang == "home".ToString()).Select(p => new
            {
                p.lienket_chiase_description,
                p.lienket_chiase_image,
                p.lienket_chiase_title
            }).FirstOrDefault();
            if (q_lienket_chiase != null)
            {
                txt_title.Text = q_lienket_chiase.lienket_chiase_title;
                txt_description.Text = q_lienket_chiase.lienket_chiase_description;

                string _img = q_lienket_chiase.lienket_chiase_image;
                string _url = Server.MapPath("~" + _img);
                load_anh_lienket_chiase(_img, _url);
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
    //but hủy ảnh cũ, phải nhấn cập nhật mới lưu
    protected void Button11_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            TextBox1.Text = "";
            Label1.Text = "";
            Button11.Visible = false;
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
    protected void Button1_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            using (dbDataContext db = new dbDataContext())
            {
                var q = db.CaiDatChung_tbs.Where(p => p.phanloai_trang == "home".ToString()).FirstOrDefault();
                if (q != null)
                {
                    string _title = txt_title.Text.Trim();
                    string _des = txt_description.Text.Trim();
                    string _img = TextBox1.Text.Trim();
                    string _img_old = q.lienket_chiase_image;
                    CaiDatChung_tb _ob = q;
                    _ob.lienket_chiase_title = _title;
                    _ob.lienket_chiase_description = _des;
                    _ob.lienket_chiase_image = _img;
                    db.SubmitChanges();
                    if (_img != _img_old)//nếu có hình mới đc chọn hoặc có yêu cầu xóa ảnh cũ
                    {
                        //thì xóa hình cũ
                        string _url_old = Server.MapPath("~" + _img_old);
                        if (File.Exists(_url_old)) //nếu có file
                            File.Delete(_url_old);
                    }
                    //giữ nguyên hiển thị ảnh khi nhấn nút CẬP NHẬT
                    string _url = Server.MapPath("~" + _img);
                    load_anh_lienket_chiase(_img, _url);
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Cập nhật thành công.", "1000", "warning"), true);
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

    #region CÀI ĐẶT BẢO TRÌ
    public void load_data_baotri(dbDataContext db)
    {
        try
        {
            var q_baotri = db.CaiDatChung_tbs.Where(p => p.phanloai_trang == "home".ToString()).Select(p => new
            {
                baotri_trangthai = p.baotri_trangthai,
                batdau = p.baotri_thoigian_batdau,
                ketthuc = p.baotri_thoigian_ketthuc,

            }).FirstOrDefault();
            if (q_baotri != null)
            {
                if (q_baotri.baotri_trangthai == true)
                {
                    PlaceHolder1.Visible = true;
                    DropDownList1.SelectedIndex = 1;
                }
                else
                {
                    PlaceHolder1.Visible = false;
                    DropDownList1.SelectedIndex = 0;
                }

                txt_ngay_batdau.Text = AhaTime_cl.Now.ToString("dd/MM/yyyy");
                txt_ngay_ketthuc.Text = AhaTime_cl.Now.AddDays(1).ToString("dd/MM/yyyy");
                for (int i = 23; i >= 0; i--)
                {
                    string hourText = i.ToString("00") + " giờ";
                    string hourValue = i.ToString("00");
                    ddl_giobatdau.Items.Add(new ListItem(hourText, hourValue));
                    ddl_gioketthuc.Items.Add(new ListItem(hourText, hourValue));
                }
                for (int i = 59; i >= 0; i--)
                {
                    string minuteText = i.ToString("00") + " phút";
                    string minuteValue = i.ToString("00");

                    ddl_phutbatdau.Items.Add(new ListItem(minuteText, minuteValue));
                    ddl_phutketthuc.Items.Add(new ListItem(minuteText, minuteValue));
                }
                if (q_baotri.baotri_trangthai == true)
                {
                    var batdau = q_baotri.batdau.Value;
                    var ketthuc = q_baotri.ketthuc.Value;
                    txt_ngay_batdau.Text = batdau.ToShortDateString();
                    txt_ngay_ketthuc.Text = ketthuc.ToShortDateString();
                    ddl_giobatdau.SelectedValue = batdau.Hour.ToString("00");
                    ddl_phutbatdau.SelectedValue = batdau.Minute.ToString("00");
                    ddl_gioketthuc.SelectedValue = ketthuc.Hour.ToString("00");
                    ddl_phutketthuc.SelectedValue = ketthuc.Minute.ToString("00");
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
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            using (dbDataContext db = new dbDataContext())
            {
                string _check_baotri = DropDownList1.SelectedValue.ToString();
                if (_check_baotri == "0")//k bảo trì
                {
                    PlaceHolder1.Visible = false;
                    txt_ngay_batdau.Text = ""; txt_ngay_ketthuc.Text = "";
                    ddl_giobatdau.Items.Clear(); ddl_gioketthuc.Items.Clear();
                    ddl_phutbatdau.Items.Clear(); ddl_phutketthuc.Items.Clear();
                }
                else//bảo trì
                {
                    load_data_baotri(db);
                    PlaceHolder1.Visible = true;
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
    protected void Button2_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            string _check_baotri = DropDownList1.SelectedValue.ToString();
            if (_check_baotri == "0")//k bảo trì
            {
                using (dbDataContext db = new dbDataContext())
                {
                    var q = db.CaiDatChung_tbs.Where(p => p.phanloai_trang == "home".ToString()).FirstOrDefault();
                    if (q != null)
                    {
                        CaiDatChung_tb _ob = q;
                        _ob.baotri_trangthai = _check_baotri == "0" ? false : true;
                        _ob.baotri_thoigian_batdau = null;
                        _ob.baotri_thoigian_ketthuc = null;
                        db.SubmitChanges();
                    }
                }
            }
            else//có bảo trì
            {
                string _ngay_batdau = txt_ngay_batdau.Text;
                string _gio_batdau = ddl_giobatdau.SelectedValue.ToString();
                string _phut_batdau = ddl_phutbatdau.SelectedValue.ToString();
                string _ngaygio_batdau = _ngay_batdau + " " + _gio_batdau + ":" + _phut_batdau;

                string _ngay_ketthuc = txt_ngay_ketthuc.Text;
                string _gio_ketthuc = ddl_gioketthuc.SelectedValue.ToString();
                string _phut_ketthuc = ddl_phutketthuc.SelectedValue.ToString();
                string _ngaygio_ketthuc = _ngay_ketthuc + " " + _gio_ketthuc + ":" + _phut_ketthuc;

                if (!dt_cl.check_date(_ngaygio_batdau))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Ngày giờ bắt đầu không hợp lệ.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                if (!dt_cl.check_date(_ngaygio_ketthuc))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Ngày giờ kết thúc không hợp lệ.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                DateTime _batdau = DateTime.Parse(_ngaygio_batdau);
                DateTime _ketthuc = DateTime.Parse(_ngaygio_ketthuc);
                if (_batdau >= _ketthuc)
                {


                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                using (dbDataContext db = new dbDataContext())
                {
                    var q = db.CaiDatChung_tbs.Where(p => p.phanloai_trang == "home".ToString()).FirstOrDefault();
                    if (q != null)
                    {
                        CaiDatChung_tb _ob = q;
                        _ob.baotri_trangthai = _check_baotri == "0" ? false : true;
                        _ob.baotri_thoigian_batdau = _batdau;
                        _ob.baotri_thoigian_ketthuc = _ketthuc;
                        db.SubmitChanges();
                    }
                }
            }
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Cập nhật thành công.", "1000", "warning"), true);
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

    #region NHÚNG MÃ
    public void load_data_nhungma(dbDataContext db)
    {
        try
        {
            var q = db.CaiDatChung_tbs.Where(p => p.phanloai_trang == "home".ToString()).Select(p => new
            {
                p.nhungma_head1,
                p.nhungma_head2,
                p.nhungma_body1,
                p.nhungma_body2,
                p.nhungma_fanpage,
                p.nhungma_googlemaps,

            }).FirstOrDefault();
            if (q != null)
            {
                txt_code_head1.Text = q.nhungma_head1;
                txt_code_head2.Text = q.nhungma_head2;
                txt_code_body1.Text = q.nhungma_body1;
                txt_code_body2.Text = q.nhungma_body2;
                txt_code_page.Text = q.nhungma_fanpage;
                txt_code_maps.Text = q.nhungma_googlemaps;
            }
            else
            {
                txt_code_head1.Text = "";
                txt_code_head2.Text = "";
                txt_code_body1.Text = "";
                txt_code_body2.Text = "";
                txt_code_page.Text = "";
                txt_code_maps.Text = "";
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
    protected void Button3_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            using (dbDataContext db = new dbDataContext())
            {
                var q = db.CaiDatChung_tbs.Where(p => p.phanloai_trang == "home".ToString()).FirstOrDefault();
                if (q != null)
                {
                    string _Code_head1 = txt_code_head1.Text.Trim();//ngay sau<head>
                    string _Code_head2 = txt_code_head2.Text.Trim();//ngay trước</head>
                    string _Code_body1 = txt_code_body1.Text.Trim();//ngay sau<body>
                    string _Code_body2 = txt_code_body2.Text.Trim();//ngay trước</body>
                    string _Plugin_Fanpage_Facebook = txt_code_page.Text.Trim();
                    string _Google_Maps = txt_code_maps.Text.Trim();

                    CaiDatChung_tb _ob = q;
                    _ob.nhungma_head1 = _Code_head1;
                    _ob.nhungma_head2 = _Code_head2;
                    _ob.nhungma_body1 = _Code_body1;
                    _ob.nhungma_body2 = _Code_body2;
                    _ob.nhungma_fanpage = _Plugin_Fanpage_Facebook;
                    _ob.nhungma_googlemaps = _Google_Maps;
                    db.SubmitChanges();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Cập nhật thành công.", "1000", "warning"), true);
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

    #region LIÊN KẾT NGOÀI - MXH...
    public void load_data_lienketngoai(dbDataContext db)
    {
        try
        {
            var q = db.CaiDatChung_tbs.Where(p => p.phanloai_trang == "home".ToString()).Select(p => new
            {
                p.link_facebook,
                p.link_zalo,
                p.link_youtube,
                p.link_instagram,
                p.link_twitter,
                p.link_tiktok,
                p.link_wechat,
                p.link_linkedin,
                p.link_whatsapp,

            }).FirstOrDefault();
            if (q != null)
            {
                TextBox2.Text = q.link_facebook;
                TextBox3.Text = q.link_zalo;
                TextBox4.Text = q.link_youtube;
                TextBox5.Text = q.link_instagram;
                TextBox6.Text = q.link_twitter;
                TextBox7.Text = q.link_tiktok;
                TextBox8.Text = q.link_wechat;
                TextBox9.Text = q.link_linkedin;
                TextBox10.Text = q.link_whatsapp;
            }
            else
            {
                TextBox2.Text = ""; TextBox3.Text = ""; TextBox4.Text = ""; TextBox5.Text = "";
                TextBox6.Text = ""; TextBox7.Text = ""; TextBox8.Text = ""; TextBox9.Text = ""; TextBox10.Text = "";
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
    protected void Button4_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            using (dbDataContext db = new dbDataContext())
            {
                var q = db.CaiDatChung_tbs.Where(p => p.phanloai_trang == "home".ToString()).FirstOrDefault();
                if (q != null)
                {
                    CaiDatChung_tb _ob = q;
                    _ob.link_facebook = TextBox2.Text.Trim() == "" ? "#" : TextBox2.Text.Trim();
                    _ob.link_zalo = TextBox3.Text.Trim() == "" ? "#" : TextBox3.Text.Trim();
                    _ob.link_youtube = TextBox4.Text.Trim() == "" ? "#" : TextBox4.Text.Trim();
                    _ob.link_instagram = TextBox5.Text.Trim() == "" ? "#" : TextBox5.Text.Trim();
                    _ob.link_twitter = TextBox6.Text.Trim() == "" ? "#" : TextBox6.Text.Trim();
                    _ob.link_tiktok = TextBox7.Text.Trim() == "" ? "#" : TextBox7.Text.Trim();
                    _ob.link_wechat = TextBox8.Text.Trim() == "" ? "#" : TextBox8.Text.Trim();
                    _ob.link_linkedin = TextBox9.Text.Trim() == "" ? "#" : TextBox9.Text.Trim();
                    _ob.link_whatsapp = TextBox10.Text.Trim() == "" ? "#" : TextBox10.Text.Trim();
                    db.SubmitChanges();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Cập nhật thành công.", "1000", "warning"), true);
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

    #region THÔNG TIN KHÁC
    private static bool IsExternalHttpUrl(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return false;
        string v = raw.Trim();
        return v.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || v.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsLocalUploadPath(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return false;
        return raw.Trim().StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase);
    }

    private string MapIfLocalUpload(string raw)
    {
        if (!IsLocalUploadPath(raw))
            return "";

        try
        {
            return Server.MapPath("~" + raw.Trim());
        }
        catch
        {
            return "";
        }
    }

    private void TryDeleteOldUploadedFile(string oldPath)
    {
        string fullPath = MapIfLocalUpload(oldPath);
        if (string.IsNullOrEmpty(fullPath))
            return;

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private static string NormalizeIconShortcutValue(string rawValue)
    {
        string value = (rawValue ?? "").Trim();
        if (value == "")
            return "/uploads/images/icon-mobile.jpg";

        if (IsExternalHttpUrl(value))
            return value;

        if (!value.StartsWith("/"))
            value = "/" + value;

        return value;
    }

    private CaiDatChung_tb GetOrCreateSetting(dbDataContext db, string scope)
    {
        var q = db.CaiDatChung_tbs.FirstOrDefault(p => p.phanloai_trang == scope);
        if (q != null)
            return q;

        q = new CaiDatChung_tb
        {
            phanloai_trang = scope
        };
        db.CaiDatChung_tbs.InsertOnSubmit(q);
        return q;
    }

    private void ApplyIconPreview(string image, TextBox targetBox, Label targetLabel, Button targetButton)
    {
        string normalized = NormalizeIconShortcutValue(image);
        if (!string.IsNullOrEmpty(normalized))
        {
            targetBox.Text = normalized;
            targetButton.Visible = true;
            targetLabel.Text = "<img class='img-cover-vuong' style='max-width:100px;max-height:100px' src='" + normalized + "' />";
        }
        else
        {
            targetBox.Text = "";
            targetButton.Visible = false;
            targetLabel.Text = "";
        }
    }

    public void load_anh_favicon(string _img, string _url)
    {
        try
        {
            string image = (_img ?? "").Trim();
            if (!string.IsNullOrEmpty(image))
            {
                txt_link_upload_2.Text = image;//textbox ẩn để chứa link ảnh /uploads/....
                Button22.Visible = true;
                Label2.Text = "<img class='img-cover-vuong' style='max-width:100px;max-height:100px' src='" + image + "' />";
            }
            else
            {
                txt_link_upload_2.Text = "";//textbox ẩn để chứa link ảnh /uploads/....
                Button22.Visible = false;
                Label2.Text = "";
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
    public void load_anh_icon_mobile(string _img, string _url)
    {
        try
        {
            ApplyIconPreview(_img, txt_link_upload_3, Label3, Button33);
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
    public void load_data_thongtinkhac(dbDataContext db)
    {
        try
        {
            var qHome = db.CaiDatChung_tbs.Where(p => p.phanloai_trang == "home".ToString()).Select(p => new
            {
                p.thongtin_icon,
                p.thongtin_apple_touch_icon,
            }).FirstOrDefault();
            if (qHome != null)
            {
                string _img_favicon = qHome.thongtin_icon;
                string _url_favicon = MapIfLocalUpload(_img_favicon);
                load_anh_favicon(_img_favicon, _url_favicon);

                string _img_icon_mobile = qHome.thongtin_apple_touch_icon;
                load_anh_icon_mobile(_img_icon_mobile, "");
            }
            else
            {
                load_anh_favicon("", "");
                load_anh_icon_mobile("", "");
            }

            var qShop = db.CaiDatChung_tbs.Where(p => p.phanloai_trang == "shop").Select(p => new
            {
                p.thongtin_apple_touch_icon
            }).FirstOrDefault();
            ApplyIconPreview(qShop != null ? qShop.thongtin_apple_touch_icon : "", txt_link_upload_shop, Label4, Button44);

            var qAdmin = db.CaiDatChung_tbs.Where(p => p.phanloai_trang == "admin").Select(p => new
            {
                p.thongtin_apple_touch_icon
            }).FirstOrDefault();
            ApplyIconPreview(qAdmin != null ? qAdmin.thongtin_apple_touch_icon : "", txt_link_upload_admin, Label5, Button55);
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

    protected void Button22_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            txt_link_upload_2.Text = "";
            Label2.Text = "";
            Button22.Visible = false;
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
    protected void Button33_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            txt_link_upload_3.Text = "";
            Label3.Text = "";
            Button33.Visible = false;
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
    protected void Button44_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            txt_link_upload_shop.Text = "";
            Label4.Text = "";
            Button44.Visible = false;
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void Button55_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            txt_link_upload_admin.Text = "";
            Label5.Text = "";
            Button55.Visible = false;
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
            {
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    protected void Button5_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            using (dbDataContext db = new dbDataContext())
            {
                var qHome = GetOrCreateSetting(db, "home");
                var qShop = GetOrCreateSetting(db, "shop");
                var qAdmin = GetOrCreateSetting(db, "admin");

                if (qHome != null)
                {
                    string _img_favicon = txt_link_upload_2.Text.Trim();
                    if (string.IsNullOrEmpty(_img_favicon))
                        _img_favicon = "/uploads/images/favicon.png";

                    string _img_favicon_old = qHome.thongtin_icon;
                    qHome.thongtin_icon = _img_favicon;
                    if (_img_favicon != _img_favicon_old)//nếu có hình mới đc chọn hoặc có yêu cầu xóa ảnh cũ
                    {
                        //thì xóa hình cũ nếu là file upload local
                        TryDeleteOldUploadedFile(_img_favicon_old);
                    }
                    //giữ nguyên hiển thị ảnh khi nhấn nút CẬP NHẬT
                    string _url_favicon = MapIfLocalUpload(_img_favicon);
                    load_anh_favicon(_img_favicon, _url_favicon);

                    string _img_iconmobile_home = NormalizeIconShortcutValue(txt_link_upload_3.Text);
                    string _img_iconmobile_home_old = qHome.thongtin_apple_touch_icon;
                    qHome.thongtin_apple_touch_icon = _img_iconmobile_home;

                    string _img_iconmobile_shop = NormalizeIconShortcutValue(txt_link_upload_shop.Text);
                    string _img_iconmobile_shop_old = qShop.thongtin_apple_touch_icon;
                    qShop.thongtin_apple_touch_icon = _img_iconmobile_shop;

                    string _img_iconmobile_admin = NormalizeIconShortcutValue(txt_link_upload_admin.Text);
                    string _img_iconmobile_admin_old = qAdmin.thongtin_apple_touch_icon;
                    qAdmin.thongtin_apple_touch_icon = _img_iconmobile_admin;

                    db.SubmitChanges();

                    if (_img_iconmobile_home != _img_iconmobile_home_old)
                        TryDeleteOldUploadedFile(_img_iconmobile_home_old);
                    if (_img_iconmobile_shop != _img_iconmobile_shop_old)
                        TryDeleteOldUploadedFile(_img_iconmobile_shop_old);
                    if (_img_iconmobile_admin != _img_iconmobile_admin_old)
                        TryDeleteOldUploadedFile(_img_iconmobile_admin_old);

                    load_anh_icon_mobile(_img_iconmobile_home, "");
                    ApplyIconPreview(_img_iconmobile_shop, txt_link_upload_shop, Label4, Button44);
                    ApplyIconPreview(_img_iconmobile_admin, txt_link_upload_admin, Label5, Button55);
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Cập nhật thành công.", "1000", "warning"), true);
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
