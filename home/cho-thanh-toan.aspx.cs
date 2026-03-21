using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class home_cho_thanh_toan : System.Web.UI.Page
{
    private const decimal VND_PER_QUYEN = 1000m;

    private string FormatQuyenWithVnd(decimal quyen)
    {
        decimal vnd = quyen * VND_PER_QUYEN;
        return string.Format("{0:#,##0.##} Quyền (~{1:#,##0}đ)", quyen, vnd);
    }

    private static string FormatQuyenValue(decimal quyen)
    {
        return quyen.ToString("#,##0.##");
    }

    private static string BuildExchangeSuccessMessage(decimal quyenUuDai, decimal quyenTieuDung)
    {
        if (quyenUuDai > 0m && quyenTieuDung > 0m)
        {
            return string.Format(
                "Đã trao đổi thành công {0} Quyền ưu đãi và {1} Quyền tiêu dùng.",
                FormatQuyenValue(quyenUuDai),
                FormatQuyenValue(quyenTieuDung));
        }

        if (quyenUuDai > 0m)
        {
            return string.Format(
                "Đã trao đổi thành công {0} Quyền ưu đãi.",
                FormatQuyenValue(quyenUuDai));
        }

        return string.Format(
            "Đã trao đổi thành công {0} Quyền tiêu dùng.",
            FormatQuyenValue(quyenTieuDung));
    }

    private static string BuildExchangeSuccessNotice(decimal quyenUuDai, decimal quyenTieuDung, string orderId)
    {
        return BuildExchangeSuccessMessage(quyenUuDai, quyenTieuDung) + " ID đơn hàng: " + orderId;
    }

    private string ResolvePortalInlineDialogClass(string defaultCssClass)
    {
        return IsShopPortal() ? "warning" : defaultCssClass;
    }

    private void SetPortalDialogOnload(string title, string message, string defaultCssClass)
    {
        if (IsShopPortal())
        {
            Session["ThongBao_Shop"] = string.Format(
                "modal|{0}|{1}|primary|0",
                (title ?? "Thông báo").Replace("|", " "),
                (message ?? string.Empty).Replace("|", " "));
            return;
        }

        Session["thongbao_home"] = thongbao_class.metro_dialog_onload(
            title ?? "Thông báo",
            message ?? string.Empty,
            "false", "false", "OK", defaultCssClass, ""
        );
    }

    private void ShowPortalDialog(string title, string message, string defaultCssClass)
    {
        ScriptManager.RegisterStartupScript(
            this.Page,
            this.GetType(),
            Guid.NewGuid().ToString(),
            thongbao_class.metro_dialog(
                title ?? "Thông báo",
                message ?? string.Empty,
                "false", "false", "OK",
                ResolvePortalInlineDialogClass(defaultCssClass),
                ""),
            true);
    }

    private void SetPortalExchangeSuccessNotice(string message)
    {
        if (IsShopPortal())
        {
            Session["ThongBao_Shop"] = string.Format(
                "modal|Thông báo|{0}|primary|0",
                HttpUtility.HtmlEncode(message ?? string.Empty));
            return;
        }

        Session["thongbao_home"] = thongbao_class.metro_dialog_onload(
            "Thông báo",
            message ?? string.Empty,
            "false", "false", "OK", "alert", ""
        );
    }
    private const int LINK_CONTEXT_TTL_MINUTES = 30;

    private bool IsShopPortal()
    {
        return PortalRequest_cl.IsShopPortalRequest();
    }

    private string SellerDonBanUrl()
    {
        return IsShopPortal() ? "/shop/don-ban" : "/home/don-ban.aspx";
    }

    private string ChoThanhToanUrl()
    {
        return IsShopPortal() ? "/shop/cho-thanh-toan" : "/home/cho-thanh-toan.aspx";
    }

    private string LoginUrl()
    {
        return IsShopPortal() ? "/shop/login.aspx" : "/dang-nhap";
    }

    private void EnsurePortalLogin()
    {
        if (IsShopPortal())
            check_login_cl.check_login_shop("none", "none", true);
        else
            check_login_cl.check_login_home("none", "none", true);
    }

    private void EnsureStableFormAction()
    {
        if (form1 == null) return;
        form1.Action = ChoThanhToanUrl();
    }

    private string GetCurrentSellerAccount()
    {
        string encryptedSeller = PortalRequest_cl.GetCurrentAccountEncrypted();
        if (string.IsNullOrEmpty(encryptedSeller))
        {
            return string.Empty;
        }

        try
        {
            return (mahoa_cl.giaima_Bcorn(encryptedSeller) ?? "").Trim();
        }
        catch
        {
            return string.Empty;
        }
    }

    private DonHang_tb ResolveOrderForCancel(dbDataContext db, string sellerAccount, int? requestedOrderId, bool requireExactOrderId)
    {
        if (db == null || string.IsNullOrEmpty(sellerAccount))
        {
            return null;
        }

        DonHang_tb order = null;
        if (requestedOrderId.HasValue)
        {
            int id = requestedOrderId.Value;
            order = db.DonHang_tbs.FirstOrDefault(p => p.id == id && p.nguoiban == sellerAccount);
            if (order == null && requireExactOrderId)
            {
                return null;
            }
        }

        if (order == null)
        {
            order = QueryDonChoTraoDoiBySeller(db, sellerAccount)
                .OrderByDescending(p => p.id)
                .FirstOrDefault();
        }

        return order;
    }

    private void ConfigureWaitActions(string orderId)
    {
        lnk_refresh_wait.NavigateUrl = ChoThanhToanUrl();
    }

    private void SetCardNotice(string htmlMessage)
    {
        bool hasMessage = !string.IsNullOrEmpty(htmlMessage);
        lb_thongbao_the.Visible = hasMessage;
        lb_thongbao_the.Text = hasMessage ? htmlMessage : string.Empty;
        box_alert_the.Visible = hasMessage;
    }

    private bool CancelOrderIfAllowed(dbDataContext db, DonHang_tb order)
    {
        if (order == null) return false;
        DonHangStateMachine_cl.EnsureStateFields(order);
        if (!DonHangStateMachine_cl.CanCancelChoTraoDoi(order)) return false;

        DonHangStateMachine_cl.SetExchangeStatus(order, DonHangStateMachine_cl.Exchange_ChuaTraoDoi);
        order.chothanhtoan = false;
        db.SubmitChanges();
        ClearPaymentSession();
        return true;
    }

    private bool TryHandleCancelWaitRequest(dbDataContext db)
    {
        if (!string.Equals((Request.QueryString["cancel_wait"] ?? "").Trim(), "1", StringComparison.Ordinal))
        {
            return false;
        }

        string sellerAccount = GetCurrentSellerAccount();
        if (string.IsNullOrEmpty(sellerAccount))
        {
            Response.Redirect(LoginUrl());
            return true;
        }

        string requestedOrderId = (Request.QueryString["id"] ?? "").Trim();
        int orderIdInt;
        if (!int.TryParse(requestedOrderId, out orderIdInt))
        {
            SetPortalDialogOnload("Thông báo", "Thiếu mã đơn hợp lệ để hủy chờ Trao đổi.", "alert");
            Response.Redirect(ChoThanhToanUrl());
            return true;
        }

        DonHang_tb donCho = ResolveOrderForCancel(db, sellerAccount, orderIdInt, true);
        if (donCho == null)
        {
            SetPortalDialogOnload("Thông báo", "Không tìm thấy đơn hàng cần hủy hoặc bạn không có quyền thao tác đơn này.", "alert");
            Response.Redirect(ChoThanhToanUrl());
            return true;
        }

        if (!CancelOrderIfAllowed(db, donCho))
        {
            SetPortalDialogOnload("Thông báo", "Đơn hàng không còn ở trạng thái chờ Trao đổi.", "alert");
            Response.Redirect(ChoThanhToanUrl());
            return true;
        }

        Response.Redirect(SellerDonBanUrl());
        return true;
    }

    private IQueryable<DonHang_tb> QueryDonChoTraoDoiBySeller(dbDataContext db, string sellerAccount)
    {
        return db.DonHang_tbs.Where(p =>
            p.nguoiban == sellerAccount &&
            (
                p.exchange_status == DonHangStateMachine_cl.Exchange_ChoTraoDoi
                || (p.exchange_status == null && p.trangthai == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
            )
        );
    }

    // Quy đổi VNĐ -> Quyền (làm tròn lên 2 chữ số)
    private static decimal QuyDoi_VND_To_Quyen(decimal vnd)
    {
        if (vnd <= 0m) return 0m;
        decimal q = vnd / VND_PER_QUYEN;
        return Math.Ceiling(q * 100m) / 100m;
    }

    // Helper cho aspx binding
    protected string FormatQuyen(object vndObj)
    {
        if (vndObj == null) return "0";
        decimal vnd = 0m;
        try { vnd = Convert.ToDecimal(vndObj); } catch { vnd = 0m; }
        decimal q = QuyDoi_VND_To_Quyen(vnd);
        return q.ToString("#,##0.##");
    }

    protected override void OnInit(EventArgs e)
    {
        // Ràng buộc ViewState theo session để giảm rủi ro CSRF với postback trao đổi.
        if (Context != null && Session != null)
        {
            ViewStateUserKey = Session.SessionID;
        }
        base.OnInit(e);
    }

    private static string GetClientIp()
    {
        if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Request == null)
            return string.Empty;

        string ip = (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? string.Empty).Trim();
        if (!string.IsNullOrEmpty(ip))
        {
            string[] ips = ip.Split(',');
            if (ips.Length > 0) return ips[0].Trim();
        }

        return (System.Web.HttpContext.Current.Request.UserHostAddress ?? string.Empty).Trim();
    }

    private void ClearPaymentSession()
    {
        WalletPaymentSession_cl.Clear(Session);
    }

    private bool TryLoadPaymentContext(
        dbDataContext db,
        string sellerAccount,
        out DonHang_tb donCho,
        out taikhoan_tb payerAccount,
        out The_PhatHanh_tb card,
        out int loaiThe,
        out string contextError,
        bool requireFreshLink)
    {
        donCho = null;
        payerAccount = null;
        card = null;
        loaiThe = 0;
        contextError = string.Empty;

        donCho = QueryDonChoTraoDoiBySeller(db, sellerAccount)
            .OrderByDescending(p => p.id)
            .FirstOrDefault();
        if (donCho == null)
        {
            contextError = "missing_order";
            return false;
        }

        DonHangStateMachine_cl.EnsureStateFields(donCho);

        Guid keyGuid;
        if (!WalletPaymentSession_cl.TryGetCardKey(Session, out keyGuid))
        {
            contextError = "missing_key";
            return false;
        }

        if (requireFreshLink && !WalletPaymentSession_cl.IsFresh(Session, LINK_CONTEXT_TTL_MINUTES))
        {
            contextError = "expired_key";
            return false;
        }

        card = db.The_PhatHanh_tbs.FirstOrDefault(p => p.idGuide == keyGuid);
        if (card == null)
        {
            contextError = "invalid_key";
            return false;
        }

        string payerCode = (card.taikhoan ?? "").Trim();
        payerAccount = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == payerCode);
        if (payerAccount == null)
        {
            contextError = "missing_payer";
            return false;
        }

        loaiThe = card.LoaiThe;

        string orderIdInSession = WalletPaymentSession_cl.GetOrderId(Session);
        if (!string.IsNullOrEmpty(orderIdInSession) && !string.Equals(orderIdInSession, donCho.id.ToString(), StringComparison.Ordinal))
        {
            contextError = "order_mismatch";
            return false;
        }

        // Đồng bộ lại session từ DB (không tin session cũ)
        WalletPaymentSession_cl.Set(
            Session,
            payerAccount.taikhoan,
            loaiThe,
            card.TenThe ?? string.Empty,
            keyGuid,
            card.TrangThai,
            donCho.id.ToString()
        );

        return true;
    }

    private string BuildContextErrorMessage(string contextError)
    {
        if (contextError == "expired_key")
            return "Phiên trao đổi đã hết hạn. Vui lòng quét link trao đổi lại.";
        if (contextError == "invalid_key")
            return "Link trao đổi không còn hợp lệ. Vui lòng quét lại thẻ.";
        if (contextError == "missing_payer")
            return "Tài khoản trao đổi không tồn tại. Vui lòng kiểm tra lại thẻ.";
        if (contextError == "order_mismatch")
            return "Đơn chờ trao đổi đã thay đổi. Vui lòng quét lại link trao đổi.";
        return "Vui lòng quét link trao đổi để tiếp tục.";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
        Response.Cache.SetNoStore();
        Response.Cache.SetRevalidation(System.Web.HttpCacheRevalidation.AllCaches);
        EnsureStableFormAction();

        ph_shop_home.Visible = IsShopPortal();
        EnsurePortalLogin();
        using (dbDataContext db = new dbDataContext())
        {
            if (TryHandleCancelWaitRequest(db))
            {
                return;
            }

            if (!IsPostBack)
            {
                check(db);
            }
        }
    }

    public void check(dbDataContext db)
    {
        EnsurePortalLogin();

        string _tk = PortalRequest_cl.GetCurrentAccountEncrypted();
        if (string.IsNullOrEmpty(_tk))
        {
            Response.Redirect(SellerDonBanUrl());
            return;
        }

        _tk = mahoa_cl.giaima_Bcorn(_tk);

        var q1 = QueryDonChoTraoDoiBySeller(db, _tk)
            .OrderByDescending(p => p.id)
            .FirstOrDefault();
        if (q1 == null)
        {
            Response.Redirect(SellerDonBanUrl());
            return;
        }

        DonHangStateMachine_cl.EnsureStateFields(q1);

        ViewState["id_donhang"] = q1.id.ToString();
        Label4.Text = q1.id.ToString();
        ConfigureWaitActions(q1.id.ToString());

        // Bind chi tiết
        var q_ct = (
            from ob1 in db.DonHang_ChiTiet_tbs.Where(p => p.id_donhang == q1.id.ToString())
            join ob2 in db.BaiViet_tbs on ob1.idsp equals ob2.id.ToString() into SanPhamGroup
            from ob2 in SanPhamGroup.DefaultIfEmpty()
            select new
            {
                ob1.id,
                ob1.id_donhang,
                name = ob2 != null ? ob2.name : "",
                image = ob2 != null ? ob2.image : "",
                ob1.giaban,
                ob1.soluong,
                ob1.thanhtien,
                PhanTramUuDai = (ob1.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0)
            }).ToList();

        lb_order_items.Text = q_ct.Count.ToString("#,##0");
        Repeater2.DataSource = q_ct;
        Repeater2.DataBind();

        decimal tongTienVND = q1.tongtien ?? 0m;
        decimal tongQuyen = QuyDoi_VND_To_Quyen(tongTienVND);

        DonHang_tb donCho = null;
        taikhoan_tb q_tk_mua = null;
        The_PhatHanh_tb card = null;
        int loaiThe = 0;
        string contextError = string.Empty;

        if (!TryLoadPaymentContext(db, _tk, out donCho, out q_tk_mua, out card, out loaiThe, out contextError, true))
        {
            if (contextError != "missing_key")
            {
                ClearPaymentSession();
            }

            PlaceHolder2.Visible = true;
            PlaceHolder1.Visible = false;

            Label1.Text = FormatQuyenWithVnd(tongQuyen);

            txt_mapin.Enabled = true;
            Button3.Enabled = true;
            SetCardNotice((!string.IsNullOrEmpty(contextError) && contextError != "missing_key")
                ? BuildContextErrorMessage(contextError)
                : string.Empty);

            Timer1.Enabled = true;
            return;
        }

        PlaceHolder2.Visible = false;
        PlaceHolder1.Visible = true;

        lb_tenshop.Text = db.taikhoan_tbs.First(p => p.taikhoan == _tk).ten_shop;
        string tenThe = card.TenThe ?? "";
        bool trangThaiThe = card.TrangThai;

        if (!string.IsNullOrEmpty(tenThe))
            Label3.Text = q_tk_mua.hoten + " (Đang dùng thẻ: " + tenThe + ")";
        else
            Label3.Text = q_tk_mua.hoten;

        // ===== ƯU TIÊN 1: THẺ BỊ KHÓA =====
        if (!trangThaiThe)
        {
            Label2.Text = FormatQuyenWithVnd(tongQuyen);

            txt_mapin.Enabled = false;
            Button3.Enabled = false;

            SetCardNotice("Thẻ này đã bị khóa. Vui lòng liên hệ quản trị hoặc sử dụng thẻ khác.");

            Timer1.Enabled = false;
            return;
        }

        // ===== ƯU TIÊN 2: THẺ CHƯA HỖ TRỢ =====
        if (loaiThe != 1 && loaiThe != 2)
        {
            Label2.Text = FormatQuyenWithVnd(tongQuyen);

            txt_mapin.Enabled = false;
            Button3.Enabled = false;

            SetCardNotice("Hiện tại chưa Trao đổi được bằng loại thẻ này.");

            Timer1.Enabled = false;
            return;
        }

        // ===== TÍNH ƯU ĐÃI THEO ĐƠN =====
        string id_dh = ViewState["id_donhang"].ToString();
        var q_ct2 = db.DonHang_ChiTiet_tbs.Where(p => p.id_donhang == id_dh).ToList();

        decimal tienUuDaiVND = 0m;
        foreach (var ct in q_ct2)
        {
            int pt = 0;

            try
            {
                pt = (ct.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0);
            }
            catch
            {
                var sp = db.BaiViet_tbs.FirstOrDefault(x => x.id.ToString() == ct.idsp && x.bin == false);
                if (sp != null) pt = (sp.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0);
            }

            if (pt < 0) pt = 0;
            if (pt > 50) pt = 50;

            decimal dongThanhTienVND = ct.thanhtien ?? 0m;
            if (dongThanhTienVND <= 0m) continue;

            if (pt > 0)
                tienUuDaiVND += dongThanhTienVND * pt / 100m;
        }

        decimal quyenUuDai = (tienUuDaiVND > 0m) ? QuyDoi_VND_To_Quyen(tienUuDaiVND) : 0m;
        bool donCoUuDai = (tienUuDaiVND > 0m && quyenUuDai > 0m);

        // ===== UI theo từng loại thẻ =====

        // ------------------ LOẠI 1: THẺ ƯU ĐÃI ------------------
        if (loaiThe == 1)
        {
            if (!donCoUuDai)
            {
            Label2.Text = FormatQuyenWithVnd(tongQuyen);

                txt_mapin.Enabled = false;
                Button3.Enabled = false;

                SetCardNotice("Thẻ ưu đãi chỉ áp dụng cho đơn có ưu đãi. Đơn này không có ưu đãi nên không thể Trao đổi bằng thẻ ưu đãi.");

                Timer1.Enabled = false;
                return;
            }

            decimal quyenConLai = tongQuyen - quyenUuDai;
            if (quyenConLai < 0m) quyenConLai = 0m;

            Label2.Text = FormatQuyenWithVnd(quyenUuDai);

            txt_mapin.Enabled = true;
            Button3.Enabled = true;

            SetCardNotice(
                "Trao đổi bằng <b>Thẻ ưu đãi</b>: <b>" + quyenUuDai.ToString("#,##0.##") + " Quyền</b>."
                + "<br/>Phần còn lại: <b>" + quyenConLai.ToString("#,##0.##") + " Quyền</b> (trao đổi tiền mặt theo quy đổi)."
            );

            Timer1.Enabled = false;
            return;
        }

        // ------------------ LOẠI 2 ------------------
        Label2.Text = FormatQuyenWithVnd(tongQuyen);

        txt_mapin.Enabled = true;
        Button3.Enabled = true;

        decimal soDuUuDaiQuyen = q_tk_mua.Vi1That_Evocher_30PhanTram ?? 0m; // ví ưu đãi (mới)
        decimal soDuQuyen = q_tk_mua.DongA ?? 0m;                           // ví tiêu dùng

        string html = "";
        html += "Tổng Trao đổi: <b>" + tongQuyen.ToString("#,##0.##") + " Quyền</b>.";

        if (donCoUuDai)
        {
            decimal quyenConLai = tongQuyen - quyenUuDai;
            if (quyenConLai < 0m) quyenConLai = 0m;

            html += "<br/>Ưu đãi: <b>" + quyenUuDai.ToString("#,##0.##") + " Quyền</b>."
                  + "<br/>Còn lại: <b>" + quyenConLai.ToString("#,##0.##") + " Quyền</b>.";

            // Quy tắc: nếu ví ưu đãi không đủ -> trừ 100% Quyền tiêu dùng
            if (soDuUuDaiQuyen >= quyenUuDai)
            {
                html += "<br/><b>Sẽ trừ:</b> "
                      + "<br/><b>-" + quyenUuDai.ToString("#,##0.##") + " Quyền ưu đãi</b>"
                      + "<br/><b>-" + quyenConLai.ToString("#,##0.##") + " Quyền tiêu dùng</b>";
            }
            else
            {
                html += "<br/><b>Hồ sơ ưu đãi không đủ</b> → sẽ trừ 100% vào <b>Hồ sơ tiêu dùng</b>: <b>-" + tongQuyen.ToString("#,##0.##") + " Quyền tiêu dùng</b>.";
            }
        }
        else
        {
            html += "<br/><b>Sẽ trừ:</b> <b>-" + tongQuyen.ToString("#,##0.##") + " Quyền tiêu dùng</b>.";
        }

        SetCardNotice(html);

        Timer1.Enabled = false;
    }

    protected void Timer1_Tick(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            check(db);
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string orderId = ViewState["id_donhang"] != null ? ViewState["id_donhang"].ToString() : string.Empty;
            var q = string.IsNullOrEmpty(orderId)
                ? null
                : db.DonHang_tbs.FirstOrDefault(p => p.id.ToString() == orderId);
            CancelOrderIfAllowed(db, q);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "redirectScript",
                "window.location.href='" + SellerDonBanUrl() + "';", true);
        }
    }

    protected void btn_huy_cho_Click(object sender, EventArgs e)
    {
        EnsurePortalLogin();

        string sellerAccount = GetCurrentSellerAccount();
        if (string.IsNullOrEmpty(sellerAccount))
        {
            Response.Redirect(LoginUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        int parsedOrderId;
        int? requestOrderId = null;
        if (ViewState["id_donhang"] != null && int.TryParse(ViewState["id_donhang"].ToString(), out parsedOrderId))
        {
            requestOrderId = parsedOrderId;
        }

        using (dbDataContext db = new dbDataContext())
        {
            DonHang_tb donCho = ResolveOrderForCancel(db, sellerAccount, requestOrderId, false);
            if (donCho == null)
            {
                ShowPortalDialog("Thông báo", "Không tìm thấy đơn hàng chờ Trao đổi để hủy.", "alert");
                return;
            }

            if (!CancelOrderIfAllowed(db, donCho))
            {
                ShowPortalDialog("Thông báo", "Đơn hàng không còn ở trạng thái chờ Trao đổi.", "alert");
                return;
            }
        }

        Response.Redirect(SellerDonBanUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        ClearPaymentSession();

        Response.Redirect(ChoThanhToanUrl());
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        EnsurePortalLogin();

        string _tk = PortalRequest_cl.GetCurrentAccountEncrypted();
        if (!string.IsNullOrEmpty(_tk))
        {
            _tk = mahoa_cl.giaima_Bcorn(_tk);
        }
        else
        {
            SetPortalDialogOnload("Thông báo", "Phiên của bạn đã hết hạn. Vui lòng đăng nhập lại.", "alert");

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "redirectScript",
                "window.location.href='" + LoginUrl() + "';", true);
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            DonHang_tb donCho = null;
            taikhoan_tb q_tk_mua = null;
            The_PhatHanh_tb card = null;
            int loaiThe = 0;
            string contextError = string.Empty;

            if (!TryLoadPaymentContext(db, _tk, out donCho, out q_tk_mua, out card, out loaiThe, out contextError, true))
            {
                ClearPaymentSession();
                SetPortalDialogOnload("Thông báo", BuildContextErrorMessage(contextError), "alert");
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "redirectScript",
                    "window.location.href='" + ChoThanhToanUrl() + "';", true);
                return;
            }

            if (!card.TrangThai)
            {
                ShowPortalDialog("Thông báo", "Thẻ đã bị khóa. Vui lòng liên hệ quản trị hoặc quét thẻ khác.", "alert");
                return;
            }

            if (loaiThe != 1 && loaiThe != 2)
            {
                ShowPortalDialog("Thông báo", "Thẻ này không hợp lệ để Trao đổi.", "alert");
                return;
            }

            if (ViewState["id_donhang"] == null || !string.Equals(ViewState["id_donhang"].ToString(), donCho.id.ToString(), StringComparison.Ordinal))
            {
                ShowPortalDialog("Thông báo", "Đơn chờ trao đổi đã thay đổi. Vui lòng quét lại link trao đổi.", "alert");
                return;
            }

            string payer = q_tk_mua.taikhoan;

            string _pinDaCai = (q_tk_mua.mapin_thanhtoan ?? "").Trim();
            if (string.IsNullOrEmpty(_pinDaCai))
            {
                ShowPortalDialog("Thông báo", "Vui lòng cài đặt mã pin Trao đổi trước khi Trao đổi", "alert");
                return;
            }

            string _mapin = txt_mapin.Text.Trim();
            if (!PinSecurity_cl.IsValidPinFormat(_mapin))
            {
                ShowPortalDialog("Mã pin không đúng", "Mã PIN phải gồm đúng 4 chữ số.", "alert");
                return;
            }

            string clientIp = GetClientIp();
            DateTime lockUntil;
            if (PinAttemptGuard_cl.IsLocked(payer, clientIp, out lockUntil))
            {
                ShowPortalDialog("Thông báo", "Chức năng đang bị tạm khóa, vui lòng thử lại vào lúc " + lockUntil.ToString("dd/MM/yyyy HH:mm"), "alert");
                return;
            }

            if (!PinSecurity_cl.VerifyAndUpgrade(q_tk_mua, _mapin))
            {
                DateTime? lockUntilOnFail;
                bool shouldBlockAccount;
                int failCount = PinAttemptGuard_cl.RegisterFailure(payer, clientIp, out lockUntilOnFail, out shouldBlockAccount);

                if (shouldBlockAccount)
                {
                    // khóa chính tài khoản trao đổi (payer), không khóa nhầm tài khoản người bán
                    q_tk_mua.block = true;
                    db.SubmitChanges();
                    PinAttemptGuard_cl.Reset(payer, clientIp);

                    SetPortalDialogOnload("Mã pin không đúng", "Bạn đã nhập sai " + failCount + " lần. Tài khoản trao đổi đã bị khóa. Vui lòng liên hệ với chúng tôi để xác thực.", "alert");

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "redirectScript",
                        "window.location.href='" + LoginUrl() + "';", true);
                    return;
                }

                if (lockUntilOnFail != null)
                {
                    ShowPortalDialog("Mã pin không đúng", "Bạn đã nhập sai " + failCount + " lần. Chức năng sẽ bị khóa đến " + lockUntilOnFail.Value.ToString("dd/MM/yyyy HH:mm"), "alert");
                }
                else
                {
                    ShowPortalDialog("Mã pin không đúng", "Bạn đã nhập sai " + failCount + " lần.", "alert");
                }
                return;
            }

            PinAttemptGuard_cl.Reset(payer, clientIp);

            // ================== XỬ LÝ Trao đổi ==================
            DateTime _now = AhaTime_cl.Now;
            db.Connection.Open();
            var tran = db.Connection.BeginTransaction(IsolationLevel.Serializable);
            db.Transaction = tran;
            bool committed = false;
            try
            {
                Guid keyGuid;
                if (!WalletPaymentSession_cl.TryGetCardKey(Session, out keyGuid))
                {
                    ShowPortalDialog("Thông báo", "Phiên trao đổi không hợp lệ. Vui lòng quét lại link trao đổi.", "alert");
                    return;
                }

                var cardTx = db.The_PhatHanh_tbs.FirstOrDefault(p => p.idGuide == keyGuid);
                if (cardTx == null || !cardTx.TrangThai)
                {
                    ShowPortalDialog("Thông báo", "Thẻ không còn hợp lệ để Trao đổi.", "alert");
                    return;
                }

                if (!string.Equals((cardTx.taikhoan ?? "").Trim(), payer, StringComparison.OrdinalIgnoreCase))
                {
                    ShowPortalDialog("Thông báo", "Thông tin tài khoản trao đổi đã thay đổi. Vui lòng quét lại link trao đổi.", "alert");
                    return;
                }

                loaiThe = cardTx.LoaiThe;

                q_tk_mua = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == payer);
                if (q_tk_mua == null || q_tk_mua.block == true)
                {
                    ShowPortalDialog("Thông báo", "Tài khoản trao đổi không hợp lệ hoặc đã bị khóa.", "alert");
                    return;
                }

                // Re-check PIN trong cùng transaction để tránh TOCTOU.
                if (!PinSecurity_cl.VerifyAndUpgrade(q_tk_mua, _mapin))
                {
                    ShowPortalDialog("Mã pin không đúng", "Mã PIN không đúng hoặc đã thay đổi. Vui lòng thử lại.", "alert");
                    return;
                }

                var q = db.DonHang_tbs.FirstOrDefault(p => p.id.ToString() == ViewState["id_donhang"].ToString());
                if (q == null)
                {
                    SetPortalDialogOnload("Thông báo", "Có lỗi xãy ra.", "alert");
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "redirectScript",
                        "window.location.href='" + SellerDonBanUrl() + "';", true);
                    return;
                }

                string id_dh = ViewState["id_donhang"].ToString();

                DonHangStateMachine_cl.EnsureStateFields(q);
                if (!DonHangStateMachine_cl.CanExecuteExchange(q))
                {
                    ShowPortalDialog("Thông báo", "Đơn hàng không còn ở trạng thái chờ Trao đổi.", "alert");
                    return;
                }

            decimal tongTienVND = q.tongtien ?? 0m;
            decimal tongQuyen = QuyDoi_VND_To_Quyen(tongTienVND);

            var q_ct2 = db.DonHang_ChiTiet_tbs.Where(p => p.id_donhang == id_dh).ToList();

            // ===== tính tổng ưu đãi theo dòng =====
            decimal tienUuDaiVND = 0m;
            foreach (var ct in q_ct2)
            {
                int pt = 0;

                try
                {
                    pt = (ct.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0);
                }
                catch
                {
                    var sp = db.BaiViet_tbs.FirstOrDefault(x => x.id.ToString() == ct.idsp && x.bin == false);
                    if (sp != null) pt = (sp.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0);
                }

                if (pt < 0) pt = 0;
                if (pt > 50) pt = 50;

                decimal dongThanhTienVND = ct.thanhtien ?? 0m;
                if (dongThanhTienVND <= 0m) continue;

                if (pt > 0)
                    tienUuDaiVND += dongThanhTienVND * pt / 100m;
            }

            decimal quyenUuDai = (tienUuDaiVND > 0m) ? QuyDoi_VND_To_Quyen(tienUuDaiVND) : 0m;
            bool donCoUuDai = (tienUuDaiVND > 0m && quyenUuDai > 0m);

            // =========================================================================================
            // ============================ NHÁNH 1: THẺ ƯU ĐÃI (loaiThe = 1) ===========================
            // =========================================================================================
            if (loaiThe == 1)
            {
                if (!donCoUuDai)
                {
                    ShowPortalDialog("Thông báo", "Thẻ ưu đãi chỉ áp dụng cho đơn có ưu đãi. Đơn này không có ưu đãi nên không thể Trao đổi bằng thẻ ưu đãi.", "alert");
                    return;
                }

                // ✅ TRỪ: dùng field mới của người mua
                decimal soDuUuDaiQuyen = q_tk_mua.Vi1That_Evocher_30PhanTram ?? 0m;

                if (soDuUuDaiQuyen < quyenUuDai)
                {
                    ShowPortalDialog("Thông báo", "Ví ưu đãi không đủ để Trao đổi.\nCần: " + quyenUuDai.ToString("#,##0.##") + " Quyền.", "alert");
                    return;
                }

                // 1) Trừ ví ưu đãi người mua + lịch sử
                q_tk_mua.Vi1That_Evocher_30PhanTram = soDuUuDaiQuyen - quyenUuDai;

                db.LichSu_DongA_tbs.InsertOnSubmit(new LichSu_DongA_tb
                {
                    taikhoan = payer,
                    dongA = quyenUuDai,
                    ngay = _now,
                    CongTru = false,
                    id_donhang = id_dh,
                    LoaiHoSo_Vi = 2,
                    ghichu = "Ưu đãi đơn hàng số " + id_dh + " (Thẻ ưu đãi)"
                });

                // 2) Cập nhật đơn
                DonHangStateMachine_cl.SetExchangeStatus(q, DonHangStateMachine_cl.Exchange_DaTraoDoi);
                q.nguoimua = payer;
                q.hoten_nguoinhan = q_tk_mua.hoten;
                q.sdt_nguoinhan = q_tk_mua.dienthoai;
                q.diahchi_nguoinhan = q_tk_mua.diachi;
                q.chothanhtoan = false;

                // 3) tăng số lượng đã bán
                foreach (var item in q_ct2)
                {
                    var q_bv = db.BaiViet_tbs.FirstOrDefault(p => p.id.ToString() == item.idsp);
                    if (q_bv != null)
                        q_bv.soluong_daban = (q_bv.soluong_daban ?? 0) + item.soluong;
                }

                // 4) ✅ CỘNG: ghi nhận ShopOnly cho người bán
                if (!string.IsNullOrWhiteSpace(q.nguoiban))
                {
                    ShopOnlyLedger_cl.AddSellerCreditFromOrder(
                        db,
                        q.nguoiban,
                        id_dh,
                        quyenUuDai,
                        2,
                        string.Format("Bán đơn hàng số {0} (Thẻ ưu đãi - Hồ sơ ưu đãi ShopOnly)", id_dh));
                }

                // 5) Thông báo 2 chiều
                string successNotice = BuildExchangeSuccessNotice(quyenUuDai, 0m, id_dh);
                db.ThongBao_tbs.InsertOnSubmit(new ThongBao_tb
                {
                    id = Guid.NewGuid(),
                    daxem = false,
                    nguoithongbao = payer,
                    nguoinhan = q.nguoiban,
                    link = SellerDonBanUrl(),
                    noidung = db.taikhoan_tbs.First(p => p.taikhoan == payer).hoten + " " + successNotice,
                    thoigian = _now,
                    bin = false
                });

                db.ThongBao_tbs.InsertOnSubmit(new ThongBao_tb
                {
                    id = Guid.NewGuid(),
                    daxem = false,
                    nguoithongbao = q.nguoiban,
                    nguoinhan = payer,
                    link = "/home/don-mua.aspx",
                    noidung = successNotice,
                    thoigian = _now,
                    bin = false
                });

                db.SubmitChanges();
                tran.Commit();
                committed = true;
                ClearPaymentSession();

                SetPortalExchangeSuccessNotice(BuildExchangeSuccessMessage(quyenUuDai, 0m));

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "redirectScript",
                    "window.location.href='" + SellerDonBanUrl() + "';", true);

                return;
            }

            // =========================================================================================
            // ============================ NHÁNH 2: LOẠI THẺ = 2 ======================================
            // =========================================================================================
            if (loaiThe == 2)
            {
                decimal soDuQuyen = q_tk_mua.DongA ?? 0m;

                // ✅ TRỪ: dùng field mới của người mua
                decimal soDuUuDaiQuyen = q_tk_mua.Vi1That_Evocher_30PhanTram ?? 0m;

                bool apDungUuDai = false;
                decimal quyenUuDai_2 = 0m;
                decimal quyenConLai = 0m;

                if (donCoUuDai)
                {
                    quyenUuDai_2 = quyenUuDai;


//                    string debug =
//    "tongQuyen=" + tongQuyen.ToString("0.##") +
//    "\nquyenUuDai=" + quyenUuDai_2.ToString("0.##") +
//    "\nsoDuUuDai=" + soDuUuDaiQuyen.ToString("0.##") +
//    "\nsoDuTieuDung=" + soDuQuyen.ToString("0.##") +
//    "\ntienUuDaiVND=" + tienUuDaiVND.ToString("0") +
//    "\ndonCoUuDai=" + donCoUuDai;

//ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
//    thongbao_class.metro_dialog("DEBUG", debug.Replace("\n", "<br/>"), "false", "false", "OK", "alert", ""),
//    true);
//return;


                    apDungUuDai = (soDuUuDaiQuyen >= quyenUuDai_2);

                    if (apDungUuDai)
                    {
                        quyenConLai = tongQuyen - quyenUuDai_2;
                        if (quyenConLai < 0m) quyenConLai = 0m;

                        if (soDuQuyen < quyenConLai)
                        {
                            ShowPortalDialog("Thông báo", "Ví Quyền tiêu dùng không đủ cho phần còn lại.", "alert");
                            return;
                        }
                    }
                    else
                    {
                        if (soDuQuyen < tongQuyen)
                        {
                            ShowPortalDialog("Thông báo", "Quyền tiêu dùng của bạn không đủ để Trao đổi.", "alert");
                            return;
                        }
                    }
                }
                else
                {
                    if (soDuQuyen < tongQuyen)
                    {
                        ShowPortalDialog("Thông báo", "Quyền tiêu dùng của bạn không đủ để Trao đổi.", "alert");
                        return;
                    }
                }

                // ===== TRỪ VÍ NGƯỜI MUA + LỊCH SỬ =====
                if (apDungUuDai)
                {
                    // ✅ TRỪ: field mới
                    q_tk_mua.Vi1That_Evocher_30PhanTram = soDuUuDaiQuyen - quyenUuDai_2;

                    db.LichSu_DongA_tbs.InsertOnSubmit(new LichSu_DongA_tb
                    {
                        taikhoan = payer,
                        dongA = quyenUuDai_2,
                        ngay = _now,
                        CongTru = false,
                        id_donhang = id_dh,
                        LoaiHoSo_Vi = 2,
                        ghichu = "Ưu đãi đơn hàng số " + id_dh
                    });

                    q_tk_mua.DongA = soDuQuyen - quyenConLai;

                    db.LichSu_DongA_tbs.InsertOnSubmit(new LichSu_DongA_tb
                    {
                        taikhoan = payer,
                        dongA = quyenConLai,
                        ngay = _now,
                        CongTru = false,
                        id_donhang = id_dh,
                        LoaiHoSo_Vi = 1,
                        ghichu = "Trao đổi đơn hàng số " + id_dh + " (phần còn lại)"
                    });
                }
                else
                {
                    q_tk_mua.DongA = soDuQuyen - tongQuyen;

                    db.LichSu_DongA_tbs.InsertOnSubmit(new LichSu_DongA_tb
                    {
                        taikhoan = payer,
                        dongA = tongQuyen,
                        ngay = _now,
                        CongTru = false,
                        id_donhang = id_dh,
                        LoaiHoSo_Vi = 1,
                        ghichu = "Trao đổi đơn hàng số " + id_dh
                    });
                }

                // ===== UPDATE ĐƠN =====
                DonHangStateMachine_cl.SetExchangeStatus(q, DonHangStateMachine_cl.Exchange_DaTraoDoi);
                q.nguoimua = payer;
                q.hoten_nguoinhan = q_tk_mua.hoten;
                q.sdt_nguoinhan = q_tk_mua.dienthoai;
                q.diahchi_nguoinhan = q_tk_mua.diachi;
                q.chothanhtoan = false;

                foreach (var item in q_ct2)
                {
                    var q_bv = db.BaiViet_tbs.FirstOrDefault(p => p.id.ToString() == item.idsp);
                    if (q_bv != null)
                        q_bv.soluong_daban = (q_bv.soluong_daban ?? 0) + item.soluong;
                }

                // ===== CỘNG CHO NGƯỜI BÁN + LỊCH SỬ (ShopOnly) =====
                if (!string.IsNullOrWhiteSpace(q.nguoiban))
                {
                    if (apDungUuDai)
                    {
                        ShopOnlyLedger_cl.AddSellerCreditFromOrder(
                            db,
                            q.nguoiban,
                            id_dh,
                            quyenUuDai_2,
                            2,
                            string.Format("Bán đơn hàng số {0} (ưu đãi - Hồ sơ ưu đãi ShopOnly)", id_dh));

                        ShopOnlyLedger_cl.AddSellerCreditFromOrder(
                            db,
                            q.nguoiban,
                            id_dh,
                            quyenConLai,
                            1,
                            string.Format("Bán đơn hàng số {0} (phần còn lại - Hồ sơ tiêu dùng ShopOnly)", id_dh));
                    }
                    else
                    {
                        ShopOnlyLedger_cl.AddSellerCreditFromOrder(
                            db,
                            q.nguoiban,
                            id_dh,
                            tongQuyen,
                            1,
                            string.Format("Bán đơn hàng số {0} (Hồ sơ tiêu dùng ShopOnly)", id_dh));
                    }
                }

                // ===== THÔNG BÁO 2 CHIỀU =====
                decimal successQuyenUuDai = apDungUuDai ? quyenUuDai_2 : 0m;
                decimal successQuyenTieuDung = apDungUuDai ? quyenConLai : tongQuyen;
                string successNotice = BuildExchangeSuccessNotice(successQuyenUuDai, successQuyenTieuDung, id_dh);
                db.ThongBao_tbs.InsertOnSubmit(new ThongBao_tb
                {
                    id = Guid.NewGuid(),
                    daxem = false,
                    nguoithongbao = payer,
                    nguoinhan = q.nguoiban,
                    link = SellerDonBanUrl(),
                    noidung = db.taikhoan_tbs.First(p => p.taikhoan == payer).hoten + " " + successNotice,
                    thoigian = _now,
                    bin = false
                });

                db.ThongBao_tbs.InsertOnSubmit(new ThongBao_tb
                {
                    id = Guid.NewGuid(),
                    daxem = false,
                    nguoithongbao = q.nguoiban,
                    nguoinhan = payer,
                    link = "/home/don-mua.aspx",
                    noidung = successNotice,
                    thoigian = _now,
                    bin = false
                });

                db.SubmitChanges();
                tran.Commit();
                committed = true;
                ClearPaymentSession();

                SetPortalExchangeSuccessNotice(BuildExchangeSuccessMessage(successQuyenUuDai, successQuyenTieuDung));

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "redirectScript",
                    "window.location.href='" + SellerDonBanUrl() + "';", true);

                return;
            }

            ShowPortalDialog("Thông báo", "Loại thẻ không hợp lệ.", "alert");
            }
            catch (Exception ex)
            {
                Log_cl.Add_Log(ex.Message, _tk, ex.StackTrace);
                ShowPortalDialog("Thông báo", "Có lỗi xảy ra trong quá trình xử lý trao đổi. Vui lòng thử lại.", "alert");
            }
            finally
            {
                if (!committed)
                {
                    try { tran.Rollback(); } catch { }
                }
                db.Transaction = null;
                try { db.Connection.Close(); } catch { }
            }
        }
    }
}
