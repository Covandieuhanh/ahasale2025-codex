using System;
using System.Web;
using System.Linq;

public partial class home_taikhoan : System.Web.UI.Page
{
    private void ClearPaymentSession()
    {
        WalletPaymentSession_cl.Clear(Session);
    }

    private string ResolveCurrentLoginAccount()
    {
        string tkEncrypted = Session["taikhoan_home"] as string;
        if (string.IsNullOrEmpty(tkEncrypted))
        {
            HttpCookie ckHome = Request.Cookies["cookie_userinfo_home_bcorn"];
            if (ckHome != null && !string.IsNullOrEmpty(ckHome["taikhoan"]))
                tkEncrypted = ckHome["taikhoan"];
        }

        if (string.IsNullOrEmpty(tkEncrypted))
        {
            tkEncrypted = Session["taikhoan_shop"] as string;
            if (string.IsNullOrEmpty(tkEncrypted))
            {
                HttpCookie ckShop = Request.Cookies["cookie_userinfo_shop_bcorn"];
                if (ckShop != null && !string.IsNullOrEmpty(ckShop["taikhoan"]))
                    tkEncrypted = ckShop["taikhoan"];
            }
        }

        if (string.IsNullOrEmpty(tkEncrypted))
            return "";

        try
        {
            return (mahoa_cl.giaima_Bcorn(tkEncrypted) ?? "").Trim().ToLowerInvariant();
        }
        catch
        {
            return "";
        }
    }

    private string ResolveFallbackUrl(dbDataContext db, string payer, int loaiThe)
    {
        if (loaiThe == 3)
        {
            string publicUrl = ShopSlug_cl.GetPublicUrlByTaiKhoan(db, payer);
            if (!string.IsNullOrEmpty(publicUrl) &&
                publicUrl.StartsWith("/shop/cong-khai/", StringComparison.OrdinalIgnoreCase))
            {
                return publicUrl;
            }
        }

        return "/" + payer + ".info";
    }

    private string ResolveSelfScanTarget(int loaiThe, string fallbackUrl)
    {
        if (loaiThe == 1) return "/home/lich-su-quyen-uu-dai.aspx";
        if (loaiThe == 2) return "/home/lich-su-giao-dich.aspx";
        if (loaiThe == 3) return "/shop/ho-so-tieu-dung";
        if (loaiThe == 4) return "/home/lich-su-quyen-gan-ket.aspx";
        return fallbackUrl;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        check_login_cl.check_login_home("none", "none", false);

        // ✅ reset phiên thanh toán cũ để không dính thẻ trước đó
        ClearPaymentSession();

        // 1) BẮT BUỘC có key
        string keyStr = (Request.QueryString["key"] ?? "").Trim();
        if (string.IsNullOrEmpty(keyStr))
        {
            Response.Redirect("/", true);
            return;
        }

        // 2) key phải parse được Guid
        if (!Guid.TryParse(keyStr, out Guid keyGuid))
        {
            Response.Redirect("/", true);
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            // 3) key phải khớp bản ghi trong The_PhatHanh_tb
            var the = db.The_PhatHanh_tbs.FirstOrDefault(p => p.idGuide == keyGuid);
            if (the == null)
            {
                Response.Redirect("/", true);
                return;
            }

            bool trangThai = false;
            int loaiThe = 0;

            try { trangThai = Convert.ToBoolean(the.TrangThai); } catch { trangThai = false; }
            try { loaiThe = Convert.ToInt32(the.LoaiThe); } catch { loaiThe = 0; }

            string payer = (the.taikhoan ?? "").Trim();  // A
            string tenThe = (the.TenThe ?? "").Trim();

            // payer phải có, nếu không => về trang chủ
            if (string.IsNullOrEmpty(payer))
            {
                Response.Redirect("/", true);
                return;
            }

            // A phải tồn tại trong taikhoan_tbs
            var qA = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == payer);
            if (qA == null)
            {
                Response.Redirect("/", true);
                return;
            }

            // 4) Xác định người đang login (home hoặc shop bridge)
            string tkB = ResolveCurrentLoginAccount();
            string fallbackUrl = ResolveFallbackUrl(db, payer, loaiThe);

            // 5) Self-scan: điều hướng theo loại thẻ
            if (!string.IsNullOrEmpty(tkB) &&
                string.Equals(payer, tkB, StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect(ResolveSelfScanTarget(loaiThe, fallbackUrl), true);
                return;
            }

            // 6) Không đăng nhập -> điều hướng fallback theo loại thẻ
            if (string.IsNullOrEmpty(tkB))
            {
                Response.Redirect(fallbackUrl, true);
                return;
            }

            // 7) Chỉ thẻ ưu đãi / tiêu dùng mới được vào phiên Trao đổi chờ
            if (loaiThe != 1 && loaiThe != 2)
            {
                Response.Redirect(fallbackUrl, true);
                return;
            }

            // 8) B phải có đơn "Chờ Trao đổi"
            var qWait = db.DonHang_tbs.FirstOrDefault(p =>
                p.nguoiban == tkB &&
                (
                    p.exchange_status == DonHangStateMachine_cl.Exchange_ChoTraoDoi
                    || (p.exchange_status == null && p.trangthai == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
                ));
            if (qWait == null)
            {
                Response.Redirect(fallbackUrl, true);
                return;
            }

            // 9) B đang chờ => set session và vào trang chờ
            WalletPaymentSession_cl.Set(
                Session,
                payer,
                loaiThe,
                tenThe,
                keyGuid,
                trangThai,
                qWait.id.ToString()
            );

            Response.Redirect("/home/cho-thanh-toan.aspx", true);
            return;
        }
    }
}
