using System;
using System.Web;
using System.Linq;

public partial class home_taikhoan : System.Web.UI.Page
{
    private void ClearPaymentSession()
    {
        WalletPaymentSession_cl.Clear(Session);
    }

    private static string DecryptAccount(string encrypted)
    {
        if (string.IsNullOrEmpty(encrypted))
            return "";

        try
        {
            return (mahoa_cl.giaima_Bcorn(encrypted) ?? "").Trim().ToLowerInvariant();
        }
        catch
        {
            return "";
        }
    }

    private string ReadHomeEncryptedAccount()
    {
        if (!PortalActiveMode_cl.IsHomeActive() || PortalRequest_cl.IsShopPortalRequest())
            return "";

        string tkEncrypted = Session["taikhoan_home"] as string;
        if (string.IsNullOrEmpty(tkEncrypted))
        {
            HttpCookie ckHome = Request.Cookies["cookie_userinfo_home_bcorn"];
            if (ckHome != null && !string.IsNullOrEmpty(ckHome["taikhoan"]))
                tkEncrypted = ckHome["taikhoan"];
        }

        return tkEncrypted ?? "";
    }

    private string ReadShopEncryptedAccount()
    {
        string tkEncrypted = Session["taikhoan_shop"] as string;
        if (string.IsNullOrEmpty(tkEncrypted))
        {
            HttpCookie ckShop = Request.Cookies["cookie_userinfo_shop_bcorn"];
            if (ckShop != null && !string.IsNullOrEmpty(ckShop["taikhoan"]))
                tkEncrypted = ckShop["taikhoan"];
        }

        return tkEncrypted ?? "";
    }

    private string ResolveCurrentLoginAccount(bool preferShopAccount)
    {
        if (preferShopAccount)
        {
            string shopAccount = DecryptAccount(ReadShopEncryptedAccount());
            if (!string.IsNullOrEmpty(shopAccount))
                return shopAccount;

            return DecryptAccount(ReadHomeEncryptedAccount());
        }

        string homeAccount = DecryptAccount(ReadHomeEncryptedAccount());
        if (!string.IsNullOrEmpty(homeAccount))
            return homeAccount;

        return DecryptAccount(ReadShopEncryptedAccount());
    }

    private bool IsShopModeWithLogin()
    {
        return PortalRequest_cl.IsShopPortalRequest()
            && PortalActiveMode_cl.IsShopActive()
            && PortalActiveMode_cl.HasShopCredential();
    }

    private bool IsShopFlowReferrer()
    {
        if (Request == null || Request.UrlReferrer == null)
            return false;

        string refPath = (Request.UrlReferrer.AbsolutePath ?? "").Trim();
        if (string.IsNullOrEmpty(refPath))
            return false;

        return refPath.StartsWith("/shop/", StringComparison.OrdinalIgnoreCase)
            || refPath.StartsWith("/admin/he-thong-san-pham/ban-san-pham.aspx", StringComparison.OrdinalIgnoreCase);
    }

    private bool IsGianHangFlowReferrer()
    {
        if (Request == null || Request.UrlReferrer == null)
            return false;

        string refPath = (Request.UrlReferrer.AbsolutePath ?? "").Trim();
        if (string.IsNullOrEmpty(refPath))
            return false;

        return refPath.StartsWith("/gianhang/", StringComparison.OrdinalIgnoreCase);
    }

    private bool TryGetPendingGianHangContext(out string sellerAccount, out string orderId)
    {
        sellerAccount = string.Empty;
        orderId = string.Empty;

        string spaceCode;
        if (!WalletPaymentSession_cl.TryGetPending(Session, GianHangCheckoutPortal_cl.LinkContextTtlMinutes, out spaceCode, out sellerAccount, out orderId))
            return false;

        return string.Equals(spaceCode, "gianhang", StringComparison.OrdinalIgnoreCase);
    }

    private static string ResolveGianHangWaitingOrderId(dbDataContext db, string sellerAccount)
    {
        if (db == null)
            return string.Empty;

        string account = (sellerAccount ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(account))
            return string.Empty;

        GianHangOrderRuntime_cl.OrderRuntime runtime = GianHangOrderRuntime_cl.ResolveLatestWaitingExchange(db, account, false);
        if (runtime == null)
            return string.Empty;

        return (runtime.OrderId ?? string.Empty).Trim();
    }

    private string ResolveChoThanhToanTarget(bool isShopModeWithLogin, bool forceGianHangFlow)
    {
        if (forceGianHangFlow)
            return "/gianhang/cho-thanh-toan.aspx";
        return isShopModeWithLogin ? "/shop/cho-thanh-toan" : "/home/cho-thanh-toan.aspx";
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

    private string ResolveSelfScanTarget(int loaiThe, string fallbackUrl, bool isShopModeWithLogin)
    {
        if (loaiThe == 1)
            return isShopModeWithLogin ? "/shop/ho-so-uu-dai" : "/home/lich-su-quyen-uu-dai.aspx";
        if (loaiThe == 2)
            return isShopModeWithLogin ? "/shop/ho-so-tieu-dung" : "/home/lich-su-giao-dich.aspx";
        if (loaiThe == 3) return "/shop/ho-so-tieu-dung";
        if (loaiThe == 4) return "/home/lich-su-quyen-gan-ket.aspx";
        if (loaiThe == 5) return "/home/lich-su-quyen-lao-dong.aspx";
        return fallbackUrl;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        check_login_cl.check_login_home("none", "none", false);

        // ✅ reset phiên trao đổi cũ để không dính thẻ trước đó
        ClearPaymentSession();

        // 1) BẮT BUỘC có key
        string keyStr = (Request.QueryString["key"] ?? "").Trim();
        if (string.IsNullOrEmpty(keyStr))
        {
            Response.Redirect("/", true);
            return;
        }

        // 2) key phải parse được Guid
        Guid keyGuid;
        if (!Guid.TryParse(keyStr, out keyGuid))
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
            string shopEncrypted = ReadShopEncryptedAccount();
            bool hasShopCredential = !string.IsNullOrEmpty(shopEncrypted);
            bool isShopModeWithLogin = IsShopModeWithLogin();
            if (PortalRequest_cl.IsShopPortalRequest())
            {
                if (!isShopModeWithLogin && PortalActiveMode_cl.HasShopCredential() && IsShopFlowReferrer())
                {
                    PortalActiveMode_cl.SetMode(PortalActiveMode_cl.ModeShop);
                    isShopModeWithLogin = IsShopModeWithLogin();
                }
            }
            if (!isShopModeWithLogin && hasShopCredential)
                isShopModeWithLogin = true;

            string pendingSellerAccount;
            string pendingOrderId;
            bool hasPendingGianHangContext = TryGetPendingGianHangContext(out pendingSellerAccount, out pendingOrderId);
            string gianHangSessionAccount = GianHangContext_cl.ResolveSessionTaiKhoan(Session);

            string tkB = ResolveCurrentLoginAccount(hasShopCredential);
            if (string.IsNullOrEmpty(tkB) && hasPendingGianHangContext)
            {
                string gianHangAccount = (gianHangSessionAccount ?? string.Empty).Trim().ToLowerInvariant();
                string pendingAccount = (pendingSellerAccount ?? string.Empty).Trim().ToLowerInvariant();
                tkB = !string.IsNullOrEmpty(gianHangAccount) ? gianHangAccount : pendingAccount;
            }

            string fallbackUrl = ResolveFallbackUrl(db, payer, loaiThe);

            // Thẻ gian hàng đối tác:
            // - Chưa đăng nhập shop => về trang công khai shop.
            // - Đã đăng nhập shop (đang ở mode shop) => vào lịch sử hồ sơ tiêu dùng shop.
            if (loaiThe == 3)
            {
                if (isShopModeWithLogin)
                {
                    Response.Redirect("/shop/ho-so-tieu-dung", true);
                    return;
                }

                Response.Redirect(fallbackUrl, true);
                return;
            }

            // 5) Self-scan: điều hướng theo loại thẻ
            if (!string.IsNullOrEmpty(tkB) &&
                string.Equals(payer, tkB, StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect(ResolveSelfScanTarget(loaiThe, fallbackUrl, isShopModeWithLogin), true);
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
            string tkBLower = (tkB ?? "").Trim().ToLowerInvariant();
            bool forceGianHangFlow = hasPendingGianHangContext
                || IsGianHangFlowReferrer()
                || string.Equals((Request.QueryString["space"] ?? "").Trim(), "gianhang", StringComparison.OrdinalIgnoreCase);

            string waitingOrderId = string.Empty;
            if (forceGianHangFlow)
            {
                waitingOrderId = (pendingOrderId ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(waitingOrderId))
                    waitingOrderId = ResolveGianHangWaitingOrderId(db, tkBLower);
            }

            if (string.IsNullOrEmpty(waitingOrderId))
            {
                var qWait = db.DonHang_tbs
                    .Where(p => (p.nguoiban ?? "").ToLower() == tkBLower)
                    .OrderByDescending(p => p.id)
                    .ToList()
                    .FirstOrDefault(p =>
                    {
                        string exchangeStatus = DonHangStateMachine_cl.NormalizeExchangeStatus(p.exchange_status);
                        if (exchangeStatus == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
                            return true;

                        return string.IsNullOrWhiteSpace(p.exchange_status)
                            && DonHangStateMachine_cl.NormalizeExchangeStatus(p.trangthai) == DonHangStateMachine_cl.Exchange_ChoTraoDoi;
                    });
                waitingOrderId = qWait == null ? string.Empty : qWait.id.ToString();
            }

            if (string.IsNullOrEmpty(waitingOrderId))
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
                waitingOrderId
            );

            Response.Redirect(ResolveChoThanhToanTarget(isShopModeWithLogin, forceGianHangFlow), true);
            return;
        }
    }
}
