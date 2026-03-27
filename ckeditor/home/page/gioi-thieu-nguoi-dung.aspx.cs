using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using ZXing;

public partial class home_page_gioi_thieu_nguoi_dung : System.Web.UI.Page
{
    String_cl str_cl = new String_cl();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                check_login_cl.check_login_home("none", "none", false);

                // show thongbao_home nếu có
                if (Session["thongbao_home"] != null)
                {
                    Helper_Tabler_cl.ShowModal(
                        this.Page,
                        Session["thongbao_home"].ToString(),
                        "Thông báo",
                        true,
                        "warning");
                    Session["thongbao_home"] = null;
                }

                // checkbox mặc định unchecked
                cb_dongy.Checked = false;

                string userLogin = GetUserLoginDecoded();

                string u = (Request.QueryString["u"] ?? "").Trim().ToLower();

                if (string.IsNullOrEmpty(u))
                {
                    Session["thongbao_home"] = "Đường dẫn giới thiệu không hợp lệ.";
                    Response.Redirect("/");
                    return;
                }

                if (userLogin != "")
                {
                    Session["thongbao_home"] =
                        (u == userLogin.ToLower())
                        ? "Bạn không thể tự giới thiệu chính mình."
                        : "Bạn đang đăng nhập nên không thể đăng ký qua người giới thiệu.";
                    Response.Redirect("/");
                    return;
                }

                using (dbDataContext db = new dbDataContext())
                {
                    var refAcc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == u);
                    if (refAcc == null
                        || !PortalScope_cl.CanLoginHome(refAcc.taikhoan, refAcc.phanloai, refAcc.permission))
                    {
                        Session["thongbao_home"] = "Đường dẫn giới thiệu không hợp lệ.";
                        Response.Redirect("/");
                        return;
                    }

                    lb_ref_display.Text = refAcc.taikhoan + " - " + refAcc.hoten;
                    lb_phanloai_display.Text = "Khách hàng";

                    hdn_ref_taikhoan.Value = refAcc.taikhoan;
                    hdn_ref_phanloai.Value = "Khách hàng";

                    var liType = DropDownList1.Items.FindByValue("Khách hàng");
                    if (liType != null) DropDownList1.SelectedValue = "Khách hàng";
                    DropDownList1.Enabled = true;
                    DropDownList1.Attributes["disabled"] = "disabled";
                }
            }
            catch
            {
                Session["thongbao_home"] = "Có lỗi xảy ra.";
                Response.Redirect("/");
            }
        }
    }

    protected void but_dangky_Click(object sender, EventArgs e)
    {
        try
        {
            string userLogin = GetUserLoginDecoded();

            if (userLogin != "")
            {
                Session["thongbao_home"] = "Bạn đang đăng nhập nên không thể đăng ký.";
                Response.Redirect("/");
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

            string _ref = (hdn_ref_taikhoan.Value ?? "").Trim().ToLower();
            if (string.IsNullOrEmpty(_ref))
            {
                Session["thongbao_home"] = "Đường dẫn giới thiệu không hợp lệ.";
                Response.Redirect("/");
                return;
            }

            string _phone = AccountAuth_cl.NormalizePhone(txt_taikhoan.Text);
            string _user = _phone;
            string _pass = (txt_matkhau.Text ?? "").Trim();
            string _fullname = (txt_hoten.Text ?? "").Trim();
            string _email = (txt_email.Text ?? "").Trim();
            string _ngaysinh = (txt_ngaysinh.Text ?? "").Trim();
            string _anh = (txt_link_fileupload.Text ?? "").Trim();

            if (_user == "")
            {
                Helper_Tabler_cl.ShowToast(this.Page, "Vui lòng nhập số điện thoại.", "warning");
                return;
            }
            if (!AccountAuth_cl.IsValidPhone(_phone))
            {
                Helper_Tabler_cl.ShowToast(this.Page, "Số điện thoại không hợp lệ.", "warning");
                return;
            }
            if (_pass == "")
            {
                Helper_Tabler_cl.ShowToast(this.Page, "Vui lòng nhập mật khẩu.", "warning");
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                var refAcc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _ref);
                if (refAcc == null
                    || !PortalScope_cl.CanLoginHome(refAcc.taikhoan, refAcc.phanloai, refAcc.permission))
                {
                    Session["thongbao_home"] = "Đường dẫn giới thiệu không hợp lệ.";
                    Response.Redirect("/");
                    return;
                }

                string emailLower = string.IsNullOrEmpty(_email) ? "" : _email.ToLower();
                var duplicateCandidates = db.taikhoan_tbs
                    .Where(p =>
                        p.taikhoan == _user
                        || (!string.IsNullOrEmpty(emailLower) && p.email != null && p.email.ToLower() == emailLower)
                        || p.dienthoai == _phone)
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
                    Helper_Tabler_cl.ShowModal(
                        this.Page,
                        phoneTaken || usernameTaken
                            ? "Số điện thoại đã được dùng cho tài khoản home khác."
                            : "Email đã được dùng cho tài khoản home khác.",
                        "Thông báo",
                        true,
                        "warning");
                    return;
                }

                // QR
                string qrPath = "/uploads/images/qr-user/" + _user + ".png";
                string dir = Server.MapPath("~/uploads/images/qr-user/");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                var bw = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new ZXing.Common.EncodingOptions { Width = 200, Height = 200, Margin = 3 }
                };
                bw.Write(string.Format("https://ahasale.vn/{0}.info", _user))
                  .Save(Server.MapPath("~" + qrPath), ImageFormat.Png);

                taikhoan_tb ob = new taikhoan_tb();
                ob.taikhoan = _user;
                ob.matkhau = _pass;
                ob.hoten = _fullname;
                ob.ten = str_cl.tachten(_fullname);
                ob.hoten_khongdau = str_cl.remove_vietnamchar(_fullname);
                ob.email = _email;
                ob.dienthoai = _phone;
                ob.phanloai = "Khách hàng";
                ob.ngaytao = AhaTime_cl.Now;
                ob.qr_code = ResolveUrl(qrPath);
                ob.anhdaidien = string.IsNullOrEmpty(_anh)
                    ? "/uploads/images/macdinh.jpg"
                    : _anh;
                ob.block = false;
                ob.DongA = 0;
                ob.permission = PortalScope_cl.NormalizePermissionWithScope("", PortalScope_cl.ScopeHome);
                ob.Affiliate_tai_khoan_cap_tren = refAcc.taikhoan;
                ob.Affiliate_cap_tuyen = (refAcc.Affiliate_cap_tuyen ?? 0) + 1;
                ob.Affiliate_duong_dan_tuyen_tren = (string.IsNullOrEmpty(refAcc.Affiliate_duong_dan_tuyen_tren) ? "," : refAcc.Affiliate_duong_dan_tuyen_tren) + refAcc.taikhoan + ",";
                // Tài khoản home mới luôn bắt đầu ở tier 0.
                ob.HeThongSanPham_Cap123 = 0;
                ob.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 = null;
                ob.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 = null;
                ob.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 = null;

                db.taikhoan_tbs.InsertOnSubmit(ob);
                db.SubmitChanges();

                // ================= AUTO LOGIN SAU KHI ĐĂNG KÝ =================
                string _taikhoan_mahoa = mahoa_cl.mahoa_Bcorn(_user);
                string _matkhau_mahoa = mahoa_cl.mahoa_Bcorn(_pass);

                // COOKIE (GIỐNG LOGIN)
                HttpCookie _ck = new HttpCookie("cookie_userinfo_home_bcorn");
                _ck["taikhoan"] = _taikhoan_mahoa;
                _ck["matkhau"] = _matkhau_mahoa;
                _ck.Expires = AhaTime_cl.Now.AddDays(7);
                _ck.HttpOnly = true;
                _ck.Secure = AccountAuth_cl.ShouldUseSecureCookie(Request);
                Response.Cookies.Add(_ck);

                Session["taikhoan_home"] = _taikhoan_mahoa;
                Session["matkhau_home"] = _matkhau_mahoa;

                // THÔNG BÁO
                Session["thongbao_home"] =
                    "Đăng ký tài khoản thành công. Hệ thống đã tự động đăng nhập cho bạn.";

                // URL BACK (NẾU CÓ)
                string _url_back = Convert.ToString(Session["url_back_home"]);
                if (!string.IsNullOrEmpty(_url_back))
                {
                    Response.Redirect(_url_back, false);
                }
                else
                {
                    Response.Redirect("/home/default.aspx", false);
                }

                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }
        catch
        {
            Session["thongbao_home"] = "Có lỗi xảy ra.";
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
        }
        catch { }

        return "";
    }
}
