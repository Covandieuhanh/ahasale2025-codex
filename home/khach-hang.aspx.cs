using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;
using System.Globalization;

public partial class home_khach_hang : System.Web.UI.Page
{
    DanhMuc_cl dm_cl = new DanhMuc_cl();
    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();

    private bool AllowCreateFromKhachHangPage()
    {
        return false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", true);

            string _tk = PortalRequest_cl.GetCurrentAccountEncrypted();
            if (!string.IsNullOrEmpty(_tk))
            {
                ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
            }

            set_dulieu_macdinh();
            show_main();
            but_show_form_add.Visible = AllowCreateFromKhachHangPage();
        }
    }

    public void set_dulieu_macdinh()
    {
        ViewState["current_page_khach_home"] = "1";
    }

    #region main - phân trang - tìm kiếm
    public void show_main()
    {
        using (dbDataContext db = new dbDataContext())
        {
            var list_all = (from ob1 in db.taikhoan_tbs.Where(p => p.nguoitao == ViewState["taikhoan"].ToString())
                            select new
                            {
                                ob1.id,
                                ob1.DongA,
                                ob1.taikhoan,
                                ob1.matkhau,
                                ob1.anhdaidien,
                                ob1.hoten,
                                ob1.hoten_khongdau,
                                ob1.ngaysinh,
                                ob1.email,
                                ob1.dienthoai,
                                ob1.ngaytao,
                                ob1.phanloai,
                            }).AsQueryable();

            string _key = txt_timkiem.Text.Trim();
            if (!string.IsNullOrEmpty(_key))
                list_all = list_all.Where(p => p.hoten.Contains(_key) || p.hoten_khongdau.Contains(_key) || p.taikhoan == _key || p.email == _key || p.dienthoai == _key);
            else
            {
                string _key1 = txt_timkiem1.Text.Trim();
                if (!string.IsNullOrEmpty(_key1))
                    list_all = list_all.Where(p => p.hoten.Contains(_key1) || p.hoten_khongdau.Contains(_key1) || p.taikhoan == _key1 || p.email == _key1 || p.dienthoai == _key1);
            }

            list_all = list_all.OrderByDescending(p => p.ngaytao);
            int _Tong_Record = list_all.Count();

            int show = 30;
            int current_page = int.Parse(ViewState["current_page_khach_home"].ToString());
            int total_page = number_of_page_class.return_total_page(_Tong_Record, show);
            if (current_page < 1) current_page = 1; else if (current_page > total_page) current_page = total_page;

            ViewState["total_page"] = total_page;

            if (current_page >= total_page)
            {
                but_xemtiep.Enabled = false;
                but_xemtiep1.Enabled = false;
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

            var list_split = list_all.Skip(current_page * show - show).Take(show);

            int stt = (show * current_page) - show + 1;
            int _s1 = stt + list_split.Count() - 1;
            if (_Tong_Record != 0) lb_show.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");
            else lb_show.Text = "0-0/0";
            lb_show_md.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");

            Repeater1.DataSource = list_split;
            Repeater1.DataBind();
        }
    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_khach_home"] = int.Parse(ViewState["current_page_khach_home"].ToString()) - 1;
        show_main();
    }

    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_khach_home"] = int.Parse(ViewState["current_page_khach_home"].ToString()) + 1;
        show_main();
    }

    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_khach_home"] = 1;
        show_main();
    }
    #endregion

    #region ADD
    public void reset_control_add_edit()
    {
        txt_taikhoan.Text = "";
        txt_matkhau.Text = "";
        txt_link_fileupload.Text = "";
        txt_hoten.Text = "";
        txt_ngaysinh.Text = "";
        txt_dienthoai.Text = "";
        txt_email.Text = "";

        ViewState["add_edit"] = null;
        ViewState["id_edit"] = null;

        Button2.Visible = false;
        Label2.Text = "";
        txt_taikhoan.ReadOnly = false;

        lb_nguoi_gioi_thieu.Text = "";
    }

    protected void but_show_form_add_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        if (!AllowCreateFromKhachHangPage())
        {
            Helper_Tabler_cl.ShowModal(this.Page,
                "Tính năng tạo tài khoản tại màn hình này đã tắt. Vui lòng dùng trang Đăng ký hoặc link giới thiệu để tạo tài khoản mới.",
                "Thông báo", true, "warning");
            return;
        }

        reset_control_add_edit();

        PlaceHolder1.Visible = true;
        ViewState["add_edit"] = "add";
        Label1.Text = "THÊM TÀI KHOẢN";
        but_add_edit.Text = "THÊM MỚI";

        using (dbDataContext db = new dbDataContext())
        {
            string tk_ref = ViewState["taikhoan"].ToString();
            var refAcc = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == tk_ref);
            if (refAcc != null)
                lb_nguoi_gioi_thieu.Text = refAcc.taikhoan + " - " + refAcc.hoten;
            else
                lb_nguoi_gioi_thieu.Text = tk_ref;
        }

        pn_add.Visible = true;
        up_add.Update();
    }

    protected void but_close_form_add_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        reset_control_add_edit();
        pn_add.Visible = false;
        up_add.Update();
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["id_edit"].ToString());
            if (q != null)
            {
                taikhoan_tb _ob = q;
                File_Folder_cl.del_file(_ob.anhdaidien);
                _ob.anhdaidien = "/uploads/images/macdinh.jpg";
                Button2.Visible = false;
                db.SubmitChanges();
                Label2.Text = "";
                txt_link_fileupload.Text = "";

                Helper_Tabler_cl.ShowModal(this.Page, "Xóa ảnh thành công.", "Thông báo", true, "success");
            }
        }
    }

    protected void but_add_edit_Click(object sender, EventArgs e)
    {
        try
        {
            if (!AllowCreateFromKhachHangPage())
            {
                Helper_Tabler_cl.ShowModal(this.Page,
                    "Tính năng tạo tài khoản tại màn hình này đã tắt. Vui lòng dùng trang Đăng ký hoặc link giới thiệu để tạo tài khoản mới.",
                    "Thông báo", true, "warning");
                return;
            }

            if (!Directory.Exists(Server.MapPath("~/uploads/img-handler/")))
                Directory.CreateDirectory(Server.MapPath("~/uploads/img-handler/"));

            string _user = txt_taikhoan.Text.Trim().ToLower();
            string _pass = txt_matkhau.Text.Trim();
            string _anhdaidien_new = txt_link_fileupload.Text;

            string _fullname = txt_hoten.Text.Trim();
            if (!string.IsNullOrEmpty(_fullname))
                _fullname = str_cl.VietHoa_ChuCai_DauTien(str_cl.Remove_Blank(_fullname.ToLower()));

            string _ngaysinh = txt_ngaysinh.Text.Trim();
            string _sdt = txt_dienthoai.Text.Trim().Replace(" ", "");
            string _email = txt_email.Text.Trim();

            string _loaitaikhoan = "Khách hàng";

            using (dbDataContext db = new dbDataContext())
            {
                if (_user == "")
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập tài khoản.", "Thông báo", true, "warning");
                    return;
                }
                if (str_cl.check_taikhoan_hople(_user) == false)
                {
                    Helper_Tabler_cl.ShowModal(this.Page,
                        "Tài khoản phải có độ dài từ 5-30 ký tự không dấu hoặc chữ số và không chứa dấu cách. Vui lòng kiểm tra lại.",
                        "Thông báo", true, "warning");
                    return;
                }

                if (ViewState["add_edit"] == null || ViewState["add_edit"].ToString() != "add")
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Thiếu trạng thái thêm mới.", "Thông báo", true, "warning");
                    return;
                }

                if (_pass == "")
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập mật khẩu.", "Thông báo", true, "warning");
                    return;
                }

                var q_tk = db.taikhoan_tbs
                    .Where(p => p.taikhoan == _user
                        || (!string.IsNullOrEmpty(_email) && p.email.ToLower() == _email.ToLower()))
                    .Select(p => new { p.taikhoan, p.email })
                    .FirstOrDefault();

                if (q_tk != null)
                {
                    string message = (q_tk.taikhoan == _user)
                        ? "Tài khoản đã tồn tại. Vui lòng chọn tên khác."
                        : "Email này đã được dùng cho một tài khoản khác.";

                    Helper_Tabler_cl.ShowModal(this.Page, message, "Thông báo", true, "warning");
                    return;
                }

                string tk_ref = ViewState["taikhoan"].ToString();
                taikhoan_tb refAcc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk_ref);

                string _link_qr = string.Format("https://ahasale.vn/{0}.info", _user);
                string directoryPath = Server.MapPath("~/uploads/images/qr-user/");
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                string _link_anh_qr = "";
                var qrCodeWriter = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Width = 200,
                        Height = 200,
                        Margin = 3,
                    }
                };
                Bitmap qrCodeBitmap = qrCodeWriter.Write(_link_qr);

                string fileName = _user + ".png";
                string filePath = Server.MapPath("~/" + "/uploads/images/qr-user/" + fileName);
                _link_anh_qr = "/uploads/images/qr-user/" + fileName;
                qrCodeBitmap.Save(filePath, ImageFormat.Png);

                taikhoan_tb _ob = new taikhoan_tb();
                _ob.taikhoan = _user;
                _ob.matkhau = _pass;
                _ob.hoten = _fullname;

                if (!string.IsNullOrEmpty(_ngaysinh))
                {
                    DateTime d;
                    if (DateTime.TryParseExact(_ngaysinh, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"),
                        DateTimeStyles.None, out d))
                        _ob.ngaysinh = d;
                    else
                        _ob.ngaysinh = DateTime.Parse(_ngaysinh);
                }

                _ob.ngaytao = AhaTime_cl.Now;
                _ob.phanloai = _loaitaikhoan;
                _ob.ten = str_cl.tachten(_fullname);
                _ob.hoten_khongdau = str_cl.remove_vietnamchar(_fullname);
                _ob.dienthoai = _sdt;
                _ob.qr_code = ResolveUrl(_link_anh_qr);

                _ob.anhdaidien = (!string.IsNullOrEmpty(_anhdaidien_new))
                    ? _anhdaidien_new
                    : "/uploads/images/macdinh.jpg";

                _ob.permission = PortalScope_cl.NormalizePermissionWithScope("", PortalScope_cl.ScopeHome);
                _ob.makhoiphuc = "141191";
                _ob.hsd_makhoiphuc = DateTime.Parse("01/01/1991");
                _ob.block = false;

                _ob.nguoitao = ViewState["taikhoan"].ToString();
                _ob.email = _email;
                _ob.DongA = 0;

                if (refAcc == null)
                {
                    bool taoMoiHomeRoot;
                    refAcc = HomeRoot_cl.GetOrCreate(db, out taoMoiHomeRoot);
                    if (refAcc == null
                        || string.IsNullOrWhiteSpace(refAcc.taikhoan)
                        || !PortalScope_cl.CanLoginHome(refAcc.taikhoan, refAcc.phanloai, refAcc.permission))
                    {
                        Helper_Tabler_cl.ShowModal(this.Page,
                            "Không thể xác định tài khoản ref mặc định home_root. Vui lòng thử lại.",
                            "Thông báo", true, "warning");
                        return;
                    }
                }

                string refPath = string.IsNullOrEmpty(refAcc.Affiliate_duong_dan_tuyen_tren) ? "," : refAcc.Affiliate_duong_dan_tuyen_tren;
                int refLevel = (refAcc.Affiliate_cap_tuyen == null) ? 0 : refAcc.Affiliate_cap_tuyen.Value;

                _ob.Affiliate_tai_khoan_cap_tren = refAcc.taikhoan;
                _ob.Affiliate_cap_tuyen = refLevel + 1;
                _ob.Affiliate_duong_dan_tuyen_tren = refPath + refAcc.taikhoan + ",";
                _ob.HeThongSanPham_Cap123 = 1;
                _ob.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 = 15;
                db.taikhoan_tbs.InsertOnSubmit(_ob);
                db.SubmitChanges();

                reset_control_add_edit();
                pn_add.Visible = false;
                up_add.Update();

                show_main();
                up_main.Update();

                Helper_Tabler_cl.ShowModal(this.Page, "Xử lý thành công.", "Thông báo", true, "success");
            }
        }
        catch (Exception _ex)
        {
            string _tk = PortalRequest_cl.GetCurrentAccountEncrypted();
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    #endregion
}
