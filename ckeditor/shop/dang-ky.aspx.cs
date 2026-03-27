using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

public partial class shop_dang_ky : System.Web.UI.Page
{
    private readonly String_cl str_cl = new String_cl();

    private string BuildShopUsernameFromEmail(dbDataContext db, string email)
    {
        string localPart = "";
        if (!string.IsNullOrEmpty(email) && email.Contains("@"))
            localPart = email.Split('@')[0];

        localPart = Regex.Replace((localPart ?? "").ToLower(), "[^a-z0-9]", "");
        if (string.IsNullOrEmpty(localPart))
            localPart = "account";
        localPart = "shop" + localPart;
        if (localPart.Length > 30)
            localPart = localPart.Substring(0, 30);

        string candidate = localPart;
        int suffix = 0;
        while (db.taikhoan_tbs.Any(p => p.taikhoan == candidate))
        {
            suffix++;
            string suffixText = suffix.ToString();
            int maxBaseLength = Math.Max(1, 30 - suffixText.Length);
            string basePart = localPart.Length > maxBaseLength
                ? localPart.Substring(0, maxBaseLength)
                : localPart;
            candidate = basePart + suffixText;
        }

        return candidate;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            check_login_cl.check_login_shop("none", "none", false);
            string tk = Session["taikhoan_shop"] as string;
            if (!string.IsNullOrEmpty(tk))
            {
                Response.Redirect("/shop/default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }
    }

    protected void but_dangky_Click(object sender, EventArgs e)
    {
        lb_msg.Text = "";
        try
        {
            string pass = (txt_matkhau.Text ?? "").Trim();
            string hoten = (txt_hoten.Text ?? "").Trim();
            string sdt = AccountAuth_cl.NormalizePhone((txt_dienthoai.Text ?? "").Trim());
            string email = AccountAuth_cl.NormalizeLoginId(txt_email.Text);

            if (email == "" || pass == "" || hoten == "")
            {
                lb_msg.Text = "Vui lòng nhập đủ email, mật khẩu và tên shop.";
                return;
            }

            if (!AccountAuth_cl.IsValidEmail(email))
            {
                lb_msg.Text = "Email đăng ký không hợp lệ.";
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                bool existsEmailInShop = db.taikhoan_tbs
                    .Where(p => p.email != null && p.email.ToLower() == email)
                    .ToList()
                    .Any(p => PortalScope_cl.CanLoginShop(p.taikhoan, p.phanloai, p.permission));
                if (existsEmailInShop)
                {
                    lb_msg.Text = "Email này đã được dùng cho một tài khoản shop khác.";
                    return;
                }

                if (!string.IsNullOrEmpty(sdt))
                {
                    bool existsPhoneInShop = db.taikhoan_tbs
                        .Where(p => p.dienthoai == sdt)
                        .ToList()
                        .Any(p => PortalScope_cl.CanLoginShop(p.taikhoan, p.phanloai, p.permission));
                    if (existsPhoneInShop)
                    {
                        lb_msg.Text = "Số điện thoại đã được sử dụng cho một tài khoản shop khác.";
                        return;
                    }
                }

                string user = BuildShopUsernameFromEmail(db, email);

                taikhoan_tb acc = new taikhoan_tb();
                acc.taikhoan = user;
                acc.matkhau = pass;
                acc.hoten = str_cl.VietHoa_ChuCai_DauTien(str_cl.Remove_Blank(hoten.ToLower()));
                acc.ten = str_cl.tachten(acc.hoten);
                acc.hoten_khongdau = str_cl.remove_vietnamchar(acc.hoten);
                acc.dienthoai = sdt;
                acc.email = email;
                acc.ngaytao = AhaTime_cl.Now;
                acc.phanloai = "Gian hàng đối tác";
                acc.anhdaidien = "/uploads/images/macdinh.jpg";
                acc.qr_code = "";
                acc.permission = PortalScope_cl.NormalizePermissionWithScope("", PortalScope_cl.ScopeShop);
                acc.block = true; // chỉ mở khi admin duyệt
                acc.nguoitao = "shop-register";
                acc.makhoiphuc = "141191";
                acc.hsd_makhoiphuc = DateTime.Parse("01/01/1991");
                acc.DongA = 0m;

                acc.Affiliate_tai_khoan_cap_tren = "";
                acc.Affiliate_cap_tuyen = 0;
                acc.Affiliate_duong_dan_tuyen_tren = ",";

                acc.HeThongSanPham_Cap123 = 0;
                acc.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 = null;
                acc.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 = null;
                acc.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 = null;
                acc.ten_shop = acc.hoten;
                acc.sdt_shop = sdt;
                acc.email_shop = email;

                db.taikhoan_tbs.InsertOnSubmit(acc);

                DangKy_GianHangDoiTac_tb dk = new DangKy_GianHangDoiTac_tb();
                dk.taikhoan = user;
                dk.TrangThai = 0;
                dk.NgayTao = AhaTime_cl.Now;
                dk.GhiChuAdmin = "Đăng ký từ portal shop";
                dk.NgayDuyet = null;
                dk.AdminDuyet = null;
                db.DangKy_GianHangDoiTac_tbs.InsertOnSubmit(dk);

                db.SubmitChanges();
                ShopSlug_cl.EnsureSlugForShop(db, acc);
            }

            Session["thongbao_shop"] = "Đăng ký thành công, vui lòng chờ admin duyệt.";
            Response.Redirect("/shop/login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "", ex.StackTrace);
            lb_msg.Text = "Không thể đăng ký lúc này. Vui lòng thử lại.";
        }
    }
}
