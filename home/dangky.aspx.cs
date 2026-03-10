using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;

public partial class home_dangky : System.Web.UI.Page
{
    String_cl str_cl = new String_cl();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // ✅ nếu có thông báo từ trang khác redirect về (toast/modal) thì show ra
            Helper_Tabler_cl.ShowThongBaoSession(this.Page);

            try
            {
                check_login_cl.check_login_home("none", "none", false);

                // ✅ chặn nếu đang đăng nhập (ưu tiên session taikhoan_home)
                string userLogin = GetUserLoginDecoded();
                if (!string.IsNullOrEmpty(userLogin))
                {
                    string targetUrl = Request.RawUrl ?? "/home/dangky.aspx";
                    string logoutUrl = "/home/logout.aspx?return_url=" + HttpUtility.UrlEncode(targetUrl);
                    string redirectUrl = "/?need_logout=1&return_url=" + HttpUtility.UrlEncode(targetUrl);

                    Session["home_logout_return"] = targetUrl;
                    Session["home_modal2_msg"] = "Bạn phải đăng xuất tài khoản để được phép thực hiện Đăng ký tài khoản";
                    Session["home_modal2_title"] = "Thông báo";
                    Session["home_modal2_type"] = "warning";
                    Session["home_modal2_primary_text"] = "Đăng xuất";
                    Session["home_modal2_primary_href"] = logoutUrl;
                    Session["home_modal2_secondary_text"] = "Để sau";
                    Session["home_modal2_secondary_href"] = "/";

                    Response.Redirect(redirectUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // ✅ meta đơn giản (giữ nguyên)
                string title = "Đăng ký tài khoản mới";
                string description = "Đăng ký tài khoản mới.";
                string imageUrl = string.Format("{0}://{1}/uploads/images/logo.png", Request.Url.Scheme, Request.Url.Authority);
                literal_meta.Text = string.Format(@"
                    <title>{0}</title>
                    <meta name='description' content='{1}' />
                    <meta property='og:title' content='{2}' />
                    <meta property='og:description' content='{3}' />
                    <meta property='og:image' content='{4}' />
                    <meta property='og:type' content='website' />
                    <meta property='og:url' content='{5}' />
                    <meta name='twitter:card' content='summary_large_image' />
                    <meta name='twitter:title' content='{6}' />
                    <meta name='twitter:description' content='{7}' />
                    <meta name='twitter:image' content='{8}' />
                ", title, description, title, description, imageUrl, Request.Url.AbsoluteUri, title, description, imageUrl);

                // ✅ loại tk mặc định
                lb_phanloai_display.Text = "Khách hàng";

                // init ref display
                lb_ref_hoten.Text = "";
                hf_ref_valid.Value = "";
                string refQuery = (Request.QueryString["ref"] ?? "").Trim().ToLowerInvariant();
                if (!string.IsNullOrEmpty(refQuery))
                {
                    txt_ref_taikhoan.Text = refQuery;
                    txt_ref_taikhoan_TextChanged(txt_ref_taikhoan, EventArgs.Empty);
                }

                // checkbox mặc định unchecked (giữ an toàn)
                cb_dongy.Checked = false;
            }
            catch (Exception)
            {
                Session["thongbao_home"] =
                    "modal|Thông báo|Có lỗi xảy ra. Vui lòng thử lại sau.|warning|3500";
                Response.Redirect("/");
            }
        }
        else
        {
            // Postback: nếu có session thông báo (trường hợp hiếm), vẫn show
            Helper_Tabler_cl.ShowThongBaoSession(this.Page);
        }
    }

    // ✅ Autopostback: nhập ref -> kiểm tra & hiển thị họ tên
    protected void txt_ref_taikhoan_TextChanged(object sender, EventArgs e)
    {
        try
        {
            string _ref = (txt_ref_taikhoan.Text ?? "").Trim().ToLower();

            lb_ref_hoten.Text = "";
            hf_ref_valid.Value = "";

            if (string.IsNullOrEmpty(_ref))
            {
                // không bắt buộc -> để trống thì không hiển thị gì
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                var refAcc = db.taikhoan_tbs
                    .Where(p => p.taikhoan == _ref)
                    .Select(p => new { p.taikhoan, p.hoten, p.phanloai, p.permission })
                    .FirstOrDefault();

                if (refAcc == null
                    || !PortalScope_cl.CanLoginHome(refAcc.taikhoan, refAcc.phanloai, refAcc.permission))
                {
                    lb_ref_hoten.Text = "Tài khoản giới thiệu không hợp lệ.";
                    lb_ref_hoten.CssClass = "small text-danger";
                    hf_ref_valid.Value = "0";
                    return;
                }

                string hoten = (refAcc.hoten ?? "").Trim();
                if (string.IsNullOrEmpty(hoten)) hoten = refAcc.taikhoan;

                lb_ref_hoten.Text = "Họ tên: " + hoten;
                lb_ref_hoten.CssClass = "small text-success";
                hf_ref_valid.Value = "1";
            }
        }
        catch
        {
            lb_ref_hoten.Text = "Không kiểm tra được người giới thiệu. Vui lòng thử lại.";
            lb_ref_hoten.CssClass = "small text-danger";
            hf_ref_valid.Value = "0";
        }
    }

    protected void but_dangky_Click(object sender, EventArgs e)
    {
        try
        {
            // ✅ chặn POST nếu user đang đăng nhập
            string userLogin = GetUserLoginDecoded();
            if (!string.IsNullOrEmpty(userLogin))
            {
                string targetUrl = Request.RawUrl ?? "/home/dangky.aspx";
                string logoutUrl = "/home/logout.aspx?return_url=" + HttpUtility.UrlEncode(targetUrl);
                string redirectUrl = "/?need_logout=1&return_url=" + HttpUtility.UrlEncode(targetUrl);

                Session["home_logout_return"] = targetUrl;
                Session["home_modal2_msg"] = "Bạn phải đăng xuất tài khoản để được phép thực hiện Đăng ký tài khoản";
                Session["home_modal2_title"] = "Thông báo";
                Session["home_modal2_type"] = "warning";
                Session["home_modal2_primary_text"] = "Đăng xuất";
                Session["home_modal2_primary_href"] = logoutUrl;
                Session["home_modal2_secondary_text"] = "Để sau";
                Session["home_modal2_secondary_href"] = "/";

                Response.Redirect(redirectUrl, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            // ✅ BẮT BUỘC tick đồng ý điều khoản
            if (cb_dongy == null || cb_dongy.Checked == false)
            {
                Helper_Tabler_cl.ShowModal(this.Page,
                    "Vui lòng tick chọn: Bạn đã đọc và đồng ý với Điều khoản sử dụng và Chính sách bảo mật của AhaSale.",
                    "Thông báo", true, "warning");
                return;
            }

            // ✅ người giới thiệu: KHÔNG bắt buộc
            string _ref = (txt_ref_taikhoan.Text ?? "").Trim().ToLower();

            // Home đăng ký/đăng nhập theo số điện thoại.
            string _phone = AccountAuth_cl.NormalizePhone(txt_taikhoan.Text);
            string _user = _phone; // giữ username nội bộ trùng số điện thoại để đồng nhất.
            string _pass = (txt_matkhau.Text ?? "").Trim();
            string _anhdaidien_new = (txt_link_fileupload.Text ?? "").Trim();
            string _fullname = (txt_hoten.Text ?? "").Trim();
            if (!string.IsNullOrEmpty(_fullname))
                _fullname = str_cl.VietHoa_ChuCai_DauTien(str_cl.Remove_Blank(_fullname.ToLower()));
            string _ngaysinh = (txt_ngaysinh.Text ?? "").Trim();
            string _email = (txt_email.Text ?? "").Trim();

            if (_user == "")
            {
                Helper_Tabler_cl.ShowToast(this.Page, "Vui lòng nhập số điện thoại.", "warning", true, 3000, "Thông báo");
                return;
            }
            if (!AccountAuth_cl.IsValidPhone(_phone))
            {
                Helper_Tabler_cl.ShowModal(this.Page,
                    "Số điện thoại không hợp lệ. Vui lòng kiểm tra lại.",
                    "Thông báo", true, "warning");
                return;
            }
            if (_pass == "")
            {
                Helper_Tabler_cl.ShowToast(this.Page, "Vui lòng nhập mật khẩu.", "warning", true, 3000, "Thông báo");
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                // ✅ nếu có nhập người giới thiệu -> bắt buộc tồn tại
                taikhoan_tb refAccFull = null;
                if (!string.IsNullOrEmpty(_ref))
                {
                    refAccFull = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _ref);
                    if (refAccFull == null
                        || !PortalScope_cl.CanLoginHome(refAccFull.taikhoan, refAccFull.phanloai, refAccFull.permission))
                    {
                        Helper_Tabler_cl.ShowModal(this.Page,
                            "Tài khoản người giới thiệu không hợp lệ. Vui lòng kiểm tra lại hoặc để trống.",
                            "Thông báo", true, "warning");
                        return;
                    }
                }

                // Nếu người dùng không nhập ref thì mặc định gán về home_root.
                if (refAccFull == null)
                {
                    bool taoMoiHomeRoot;
                    refAccFull = HomeRoot_cl.GetOrCreate(db, out taoMoiHomeRoot);
                    if (refAccFull == null
                        || string.IsNullOrWhiteSpace(refAccFull.taikhoan)
                        || !PortalScope_cl.CanLoginHome(refAccFull.taikhoan, refAccFull.phanloai, refAccFull.permission))
                    {
                        Helper_Tabler_cl.ShowModal(this.Page,
                            "Không thể xác định tài khoản ref mặc định home_root. Vui lòng thử lại.",
                            "Thông báo", true, "warning");
                        return;
                    }
                }

                // ✅ loại tk mặc định cố định
                string _loaitaikhoan = "Khách hàng";

                // ✅ check trùng tk/email
                string emailLower = string.IsNullOrEmpty(_email) ? "" : _email.ToLower();

                var duplicateCandidates = db.taikhoan_tbs
                    .Where(p =>
                        p.taikhoan == _user
                        || (!string.IsNullOrEmpty(emailLower) && p.email != null && p.email.ToLower() == emailLower)
                        || p.dienthoai == _phone
                    )
                    .Select(p => new { p.taikhoan, p.email, p.dienthoai, p.phanloai, p.permission })
                    .ToList();

                bool usernameTaken = duplicateCandidates.Any(p => p.taikhoan == _user);
                var scopedCandidates = duplicateCandidates
                    .Where(p => PortalScope_cl.CanLoginHome(p.taikhoan, p.phanloai, p.permission))
                    .ToList();
                bool emailTaken = !string.IsNullOrEmpty(emailLower)
                    && scopedCandidates.Any(p => (p.email ?? "").ToLower() == emailLower);
                bool phoneTaken = scopedCandidates.Any(p => (p.dienthoai ?? "").Trim() == _phone);

                if (usernameTaken || emailTaken || phoneTaken)
                {
                    string message = phoneTaken || usernameTaken
                        ? "Số điện thoại này đã được dùng cho một tài khoản home khác."
                        : "Email này đã được dùng cho một tài khoản home khác.";

                    Helper_Tabler_cl.ShowModal(this.Page, message, "Thông báo", true, "warning");
                    return;
                }

                // ✅ tạo QR giống trang mẫu
                string _link_qr = string.Format("https://ahasale.vn/{0}.info", _user);
                List<string> dataList = new List<string> { _link_qr };
                string directoryPath = Server.MapPath("~/uploads/images/qr-user/");
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                string _link_anh_qr = "";

                foreach (string data in dataList)
                {
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

                    using (Bitmap qrCodeBitmap = qrCodeWriter.Write(data))
                    {
                        string fileName = _user + ".png";
                        string filePath = Server.MapPath("~/" + "/uploads/images/qr-user/" + fileName);
                        _link_anh_qr = "/uploads/images/qr-user/" + fileName;
                        qrCodeBitmap.Save(filePath, ImageFormat.Png);
                    }
                }

                // ✅ insert giống trang mẫu
                taikhoan_tb _ob = new taikhoan_tb();
                _ob.taikhoan = _user;
                _ob.matkhau = _pass;
                _ob.hoten = _fullname;

                if (!string.IsNullOrEmpty(_ngaysinh))
                {
                    DateTime d;
                    if (DateTime.TryParseExact(_ngaysinh, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"), DateTimeStyles.None, out d))
                        _ob.ngaysinh = d;
                    else
                        _ob.ngaysinh = DateTime.Parse(_ngaysinh);
                }

                _ob.ngaytao = AhaTime_cl.Now;
                _ob.phanloai = _loaitaikhoan;
                _ob.ten = str_cl.tachten(_fullname);
                _ob.hoten_khongdau = str_cl.remove_vietnamchar(_fullname);
                _ob.dienthoai = _phone;
                _ob.qr_code = ResolveUrl(_link_anh_qr);
                _ob.anhdaidien = (!string.IsNullOrEmpty(_anhdaidien_new)) ? _anhdaidien_new : "/uploads/images/macdinh.jpg";
                _ob.permission = PortalScope_cl.NormalizePermissionWithScope("", PortalScope_cl.ScopeHome);
                _ob.makhoiphuc = "141191";
                _ob.hsd_makhoiphuc = DateTime.Parse("01/01/1991");
                _ob.block = false;
                _ob.nguoitao = ""; // trang home
                _ob.email = _email;
                _ob.DongA = 0;

                string refPath = string.IsNullOrEmpty(refAccFull.Affiliate_duong_dan_tuyen_tren) ? "," : refAccFull.Affiliate_duong_dan_tuyen_tren;
                int refLevel = (refAccFull.Affiliate_cap_tuyen == null) ? 0 : refAccFull.Affiliate_cap_tuyen.Value;

                _ob.Affiliate_tai_khoan_cap_tren = refAccFull.taikhoan;
                _ob.Affiliate_cap_tuyen = refLevel + 1;
                _ob.Affiliate_duong_dan_tuyen_tren = refPath + refAccFull.taikhoan + ",";

                // Tài khoản home mới luôn bắt đầu ở tier 0.
                _ob.HeThongSanPham_Cap123 = 0;
                _ob.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 = null;
                _ob.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 = null;
                _ob.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 = null;

                db.taikhoan_tbs.InsertOnSubmit(_ob);
                db.SubmitChanges();

                // ✅ đăng ký xong -> tự động đăng nhập (GIỮ LOGIC LOGIN)
                string _taikhoan_mahoa = mahoa_cl.mahoa_Bcorn(_user);
                string _matkhau_mahoa = mahoa_cl.mahoa_Bcorn(_pass);

                HttpCookie _ck = new HttpCookie("cookie_userinfo_home_bcorn");
                _ck["taikhoan"] = _taikhoan_mahoa;
                _ck["matkhau"] = _matkhau_mahoa;
                _ck.Expires = AhaTime_cl.Now.AddDays(7);
                _ck.HttpOnly = true;
                _ck.Secure = AccountAuth_cl.ShouldUseSecureCookie(Request);
                Response.Cookies.Add(_ck);

                Session["taikhoan_home"] = _taikhoan_mahoa;
                Session["matkhau_home"] = _matkhau_mahoa;

                Session["thongbao_home"] =
                    "modal|Thông báo|Đăng ký tài khoản thành công. Hệ thống đã tự động đăng nhập cho bạn.|success|3500";

                string _url_back = Convert.ToString(Session["url_back_home"]);
                if (!string.IsNullOrEmpty(_url_back))
                    Response.Redirect(_url_back, false);
                else
                    Response.Redirect("/home/default.aspx", false);

                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }
        catch (Exception)
        {
            Session["thongbao_home"] =
                "modal|Thông báo|Có lỗi xảy ra. Vui lòng thử lại sau.|warning|3500";
            Response.Redirect("/");
        }
    }

    private string GetUserLoginDecoded()
    {
        try
        {
            string encoded = Session["taikhoan_home"] as string;
            if (!string.IsNullOrEmpty(encoded))
                return mahoa_cl.giaima_Bcorn(encoded);

            HttpCookie ck = Request.Cookies["cookie_userinfo_home_bcorn"];
            if (ck != null && !string.IsNullOrEmpty(ck["taikhoan"]))
                return mahoa_cl.giaima_Bcorn(ck["taikhoan"]);

            return "";
        }
        catch
        {
            return "";
        }
    }
}
