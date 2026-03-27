using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_he_thong_san_pham_ban_the : System.Web.UI.Page
{
    // ======================= CONST =======================
    const long VNDTOA = 1000; // 1A = 1000 VNĐ
    private const string ViewSell = "sell";
    private const string ViewDetail = "detail";
    private const string PortalModeAdmin = "admin";
    private const string PortalModeShop = "shop";
    private const string ViewStatePortalMode = "portal_mode_bansp";
    private const string ViewStateSellerAccount = "seller_account_bansp";
    private const string ViewStateShopSpace = "shop_space_bansp";
    private const string ShopSpacePublic = "public";
    private const string ShopSpaceInternal = "internal";
    private bool? _hasLoaiHanhViColumn;
    private const string SessionSellTokenCurrent = "sell_token_current";
    private const string SessionSellTokenUsed = "sell_token_used";
    private const string SessionSellTokenInflight = "sell_token_inflight";

    private sealed class SaleDetailRow
    {
        public string TaiKhoan_Nhan { get; set; }
        public decimal DongANhanDuoc { get; set; }
        public int? LoaiHanhVi { get; set; }
        public DateTime ThoiGian { get; set; }
        public int? PhanTramNhanDuoc { get; set; }
    }

    private sealed class SaleSpecialTraceRow
    {
        public long SaleHistoryId { get; set; }
        public string HandlerLabel { get; set; }
        public string ExecutionSummary { get; set; }
        public string ExecutionData { get; set; }
        public string ExecutedAtText { get; set; }
        public string ExecutionStatus { get; set; }
    }

    private bool HasLoaiHanhViColumn(dbDataContext db)
    {
        if (_hasLoaiHanhViColumn.HasValue)
            return _hasLoaiHanhViColumn.Value;
        if (db == null)
            return false;
        try
        {
            int? len = db.ExecuteQuery<int?>(
                "SELECT COL_LENGTH('dbo.ViLoiNhuan_LichSuBanHang_ChiTiet_tb','LoaiHanhVi')")
                .FirstOrDefault();
            _hasLoaiHanhViColumn = len.HasValue;
        }
        catch
        {
            _hasLoaiHanhViColumn = false;
        }
        return _hasLoaiHanhViColumn.Value;
    }

    private void EnsureSellToken()
    {
        if (hf_sell_token == null) return;
        string token = Guid.NewGuid().ToString("N");
        hf_sell_token.Value = token;
        ViewState["sell_token"] = token;
        Session[SessionSellTokenCurrent] = token;
    }

    private bool TryBeginSellToken(out string token)
    {
        token = "";
        if (hf_sell_token == null) return true;
        token = (hf_sell_token.Value ?? "").Trim();
        if (string.IsNullOrEmpty(token)) return false;

        string current = (Session[SessionSellTokenCurrent] as string) ?? (ViewState["sell_token"] as string);
        if (string.IsNullOrEmpty(current) || !string.Equals(token, current, StringComparison.Ordinal))
            return false;

        string used = Session[SessionSellTokenUsed] as string;
        if (!string.IsNullOrEmpty(used) && string.Equals(token, used, StringComparison.Ordinal))
            return false;

        string inflight = Session[SessionSellTokenInflight] as string;
        if (!string.IsNullOrEmpty(inflight) && string.Equals(token, inflight, StringComparison.Ordinal))
            return false;

        Session[SessionSellTokenInflight] = token;
        return true;
    }

    private void CompleteSellToken(string token, bool success)
    {
        if (string.IsNullOrEmpty(token)) return;
        string inflight = Session[SessionSellTokenInflight] as string;
        if (!string.IsNullOrEmpty(inflight) && string.Equals(inflight, token, StringComparison.Ordinal))
            Session[SessionSellTokenInflight] = null;
        if (success)
            Session[SessionSellTokenUsed] = token;
    }

    private void ApplyShopPortalMasterLayout()
    {
        if (!IsShopPortalMode() || Master == null)
            return;

        Control leftMenu = Master.FindControl("menuleftuc");
        if (leftMenu != null)
            leftMenu.Visible = false;

        Control topMenu = Master.FindControl("menutopuc");
        if (topMenu != null)
            topMenu.Visible = false;
    }

    protected override void LoadViewState(object savedState)
    {
        try
        {
            base.LoadViewState(savedState);
        }
        catch (InvalidCastException ex)
        {
            // ViewState có thể bị lệch sau khi thay đổi giao diện.
            // Bỏ qua ViewState để tránh lỗi 500, trang sẽ tự load lại dữ liệu.
            try
            {
                base.LoadViewState(null);
            }
            catch { }

            try
            {
                ViewState.Clear();
            }
            catch { }

            string actor = GetSellerAccount();
            if (string.IsNullOrEmpty(actor))
                actor = IsShopPortalMode() ? "shop" : "admin";
            Log_cl.Add_Log("LoadViewState InvalidCast: " + ex.Message, actor, ex.StackTrace);
        }
    }

    private bool IsShopPortalMode()
    {
        return string.Equals((ViewState[ViewStatePortalMode] ?? "").ToString(), PortalModeShop, StringComparison.OrdinalIgnoreCase);
    }

    private string GetSellerAccount()
    {
        return ((ViewState[ViewStateSellerAccount] ?? "").ToString() ?? "").Trim().ToLowerInvariant();
    }

    private string ResolveRequestedShopSpace()
    {
        string raw = (Request.QueryString["space"] ?? "").Trim().ToLowerInvariant();
        if (raw == ShopSpaceInternal)
            return ShopSpaceInternal;
        return ShopSpacePublic;
    }

    private bool IsInternalShopSpace()
    {
        if (!IsShopPortalMode())
            return false;
        string space = ((ViewState[ViewStateShopSpace] ?? ShopSpacePublic).ToString() ?? "").Trim().ToLowerInvariant();
        return space == ShopSpaceInternal;
    }

    private string BuildShopPortalSalesBaseUrl()
    {
        if (IsInternalShopSpace())
            return "/shop/noi-bo/ban-san-pham";
        return "/shop/ban-san-pham";
    }

    private string ResolveCurrentShopAccount()
    {
        string tk = "";
        string encodedSession = Session["taikhoan_shop"] as string;
        if (!string.IsNullOrEmpty(encodedSession))
        {
            tk = mahoa_cl.giaima_Bcorn(encodedSession);
        }
        else
        {
            HttpCookie ck = Request.Cookies["cookie_userinfo_shop_bcorn"];
            if (ck != null && !string.IsNullOrEmpty(ck["taikhoan"]))
                tk = mahoa_cl.giaima_Bcorn(ck["taikhoan"]);
        }

        return (tk ?? "").Trim().ToLowerInvariant();
    }

    private string ResolveCurrentAdminAccount()
    {
        string tk = "";
        string encodedSession = Session["taikhoan"] as string;
        if (!string.IsNullOrEmpty(encodedSession))
        {
            tk = mahoa_cl.giaima_Bcorn(encodedSession);
        }
        else
        {
            HttpCookie ck = Request.Cookies["cookie_userinfo_admin_bcorn"];
            if (ck != null && !string.IsNullOrEmpty(ck["taikhoan"]))
                tk = mahoa_cl.giaima_Bcorn(ck["taikhoan"]);
        }
        return (tk ?? "").Trim().ToLowerInvariant();
    }

    private bool EnsurePortalContext()
    {
        bool shopPortal = PortalRequest_cl.IsShopPortalRequest();

        if (shopPortal)
        {
            check_login_cl.check_login_shop("none", "none", true);

            string tkShop = ResolveCurrentShopAccount();
            if (string.IsNullOrEmpty(tkShop))
            {
                Response.Redirect("/shop/login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return false;
            }

            using (dbDataContext db = new dbDataContext())
            {
                if (!CompanyShop_cl.IsCompanyShopAccount(db, tkShop))
                {
                    Session["ThongBao_Shop"] = "modal|Thông báo|Tính năng này chỉ dành cho tài khoản gian hàng đối tác công ty.|warning|0";
                    Response.Redirect("/shop/default.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return false;
                }

                CompanyShopBootstrap_cl.EnsureSystemCatalogMirrored(db);
            }

            ViewState[ViewStatePortalMode] = PortalModeShop;
            ViewState[ViewStateSellerAccount] = tkShop;
            ViewState[ViewStateShopSpace] = ResolveRequestedShopSpace();
            return true;
        }

        if (CompanyShop_cl.HideLegacyAdminSystemProduct())
        {
            Session["thongbao"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                "Luồng bán sản phẩm/thẻ đã chuyển sang không gian /shop của tài khoản shop công ty.",
                "2200",
                "warning");
            Response.Redirect("/admin/default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return false;
        }

        AdminRolePolicy_cl.RequireSuperAdmin();
        ViewState[ViewStatePortalMode] = PortalModeAdmin;
        ViewState[ViewStateShopSpace] = ShopSpacePublic;

        string tkAdmin = ResolveCurrentAdminAccount();
        if (string.IsNullOrEmpty(tkAdmin))
            tkAdmin = "admin";
        ViewState[ViewStateSellerAccount] = tkAdmin;

        return true;
    }

    private string BuildListUrl()
    {
        if (IsShopPortalMode())
            return ResolveUrl(BuildShopPortalSalesBaseUrl());
        return ResolveUrl("~/admin/he-thong-san-pham/ban-san-pham.aspx");
    }

    private string BuildSellUrl()
    {
        if (IsShopPortalMode())
            return ResolveUrl(BuildShopPortalSalesBaseUrl() + "?view=" + ViewSell);
        return ResolveUrl("~/admin/he-thong-san-pham/ban-the.aspx");
    }

    private string BuildSellPageUrl()
    {
        if (IsShopPortalMode())
        {
            string url = BuildSellUrl();
            string joiner = url.Contains("?") ? "&" : "?";
            return url + joiner + "mode=page";
        }

        return BuildSellUrl();
    }

    private void RedirectToPage(string url)
    {
        Response.Redirect(url, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private bool IsInlineSellMode()
    {
        string mode = (Request.QueryString["mode"] ?? "").Trim().ToLowerInvariant();
        return mode == "page";
    }

    public string BuildChiTietUrl(object idObj)
    {
        string id = (idObj ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(id)) return "#";
        if (IsShopPortalMode())
            return ResolveUrl(BuildShopPortalSalesBaseUrl() + "?view=" + ViewDetail + "&id=" + id);
        return ResolveUrl("~/admin/he-thong-san-pham/chi-tiet-giao-dich.aspx?id=" + id);
    }

    public string BuildSpecialExecutionBadge(object labelObj)
    {
        string label = (labelObj ?? "").ToString().Trim();
        if (label == "")
            return "<span class='fg-gray'>-</span>";

        return "<span class='sell-special-badge'>" + HttpUtility.HtmlEncode(label) + "</span>";
    }

    private void ShowSellForm()
    {
        load_taikhoan_nhadautu();
        load_sanpham();

        txt_soluong.Text = "1";
        lb_giatri.Text = "0";
        lb_dongA.Text = "0.00";
        lb_phantram_san.Text = "0 %";

        string idSp = (Request.QueryString["idsp"] ?? "").Trim();
        if (!string.IsNullOrEmpty(idSp))
        {
            ListItem found = ddl_sanpham.Items.FindByValue(idSp);
            if (found != null)
                ddl_sanpham.SelectedValue = idSp;
        }

        tinhTienVaDongA();

        pn_banthe.Visible = true;
        if (hl_back_sell != null)
            hl_back_sell.NavigateUrl = BuildListUrl();
        if (pn_sell_error != null)
            pn_sell_error.Visible = false;
        EnsureSellToken();
        up_banthe.Update();
        up_main.Visible = false;
    }

    private void ShowSellError(string message)
    {
        if (pn_sell_error == null || lt_sell_error == null)
            return;
        pn_sell_error.Visible = true;
        lt_sell_error.Text = Server.HtmlEncode(message ?? "Có lỗi xảy ra.");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!EnsurePortalContext())
            return;

        ApplyShopPortalMasterLayout();
        but_show_form_banthe.NavigateUrl = BuildSellPageUrl();

        if (!IsPostBack)
        {
            ViewState["title"] = IsShopPortalMode() ? "Bán sản phẩm công ty" : "Bán sản phẩm";

            // paging lịch sử bán
            ViewState["current_page_bansp"] = 1;
            ViewState["total_page_bansp"] = 1;

            show_main();

            string view = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
            if (view == ViewSell)
            {
                if (!IsShopPortalMode() && !AdminFullPageRoute_cl.IsTransferredRequest(Context))
                {
                    RedirectToPage(BuildSellPageUrl());
                    return;
                }
                ShowSellForm();
            }
            else if (view == ViewDetail)
            {
                long idDetail;
                if (long.TryParse((Request.QueryString["id"] ?? "").Trim(), out idDetail) && idDetail > 0)
                {
                    if (!IsShopPortalMode() && !AdminFullPageRoute_cl.IsTransferredRequest(Context))
                    {
                        RedirectToPage(BuildChiTietUrl(idDetail));
                        return;
                    }
                    load_chitiet(idDetail);
                }
            }
        }
    }

    private string ResolveProductName(dbDataContext db, int? productId, bool preferShopProduct)
    {
        int id = productId ?? 0;
        if (id <= 0)
            return "SP#0";

        string tenShop = db.BaiViet_tbs.Where(p => p.id == id).Select(p => p.name).FirstOrDefault();
        string tenSystem = db.SanPham_Aha_tbs.Where(p => p.id == id).Select(p => p.TenSanPham).FirstOrDefault();

        if (preferShopProduct)
        {
            if (!string.IsNullOrEmpty(tenShop)) return tenShop;
            if (!string.IsNullOrEmpty(tenSystem)) return tenSystem;
        }
        else
        {
            if (!string.IsNullOrEmpty(tenSystem)) return tenSystem;
            if (!string.IsNullOrEmpty(tenShop)) return tenShop;
        }

        return "SP#" + id;
    }

    private BaiViet_tb ResolveShopPortalProduct(dbDataContext db, int productId, string sellerAccount)
    {
        if (db == null || productId <= 0)
            return null;

        string seller = (sellerAccount ?? "").Trim().ToLowerInvariant();
        if (seller == "")
            return null;

        return db.BaiViet_tbs.FirstOrDefault(x =>
            x.id == productId
            && x.nguoitao == seller
            && (x.phanloai == CompanyShop_cl.ProductTypePublic || x.phanloai == CompanyShop_cl.ProductTypeInternal));
    }

    // ======================= MAIN: LIST LỊCH SỬ BÁN =======================
    public void show_main()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                int show = 30;
                int current_page = 1;

                if (ViewState["current_page_bansp"] != null)
                    int.TryParse(ViewState["current_page_bansp"].ToString(), out current_page);

                if (current_page < 1) current_page = 1;

                bool shopMode = IsShopPortalMode();
                string sellerAccount = GetSellerAccount();

                var q = db.ViLoiNhuan_LichSuBanHang_tbs.AsQueryable();
                if (shopMode && !string.IsNullOrEmpty(sellerAccount))
                    q = q.Where(hd => hd.TaiKhoan_Ban == sellerAccount);

                var listQuery = q
                    .OrderByDescending(hd => hd.ThoiGian)
                    .Select(hd => new
                    {
                        hd.id,
                        hd.TaiKhoan_Mua,
                        hd.TaiKhoan_Ban,
                        hd.id_SanPhamDichVu,
                        hd.SoLuong,
                        hd.Gia_VND_SanPhamDichVu,
                        hd.Gia_DongA_SanPhamDichVu,
                        hd.PhanTram_ChiTra_ChoSan,
                        hd.ViLoiNhuan_DongA_CuaSan_NhanDuoc_ViTong,
                        hd.Vi1_30PhanTram_NhanDuoc_ViEVoucher,
                        hd.Vi2_50PhanTram_NhanDuoc_ViLaoDong,
                        hd.Vi3_20PhanTram_NhanDuoc_ViGanKet,
                        hd.ThoiGian
                    });

                int total_record = listQuery.Count();
                int total_page = (total_record <= 0) ? 1 : (int)Math.Ceiling((double)total_record / show);

                if (current_page > total_page) current_page = total_page;

                ViewState["current_page_bansp"] = current_page;
                ViewState["total_page_bansp"] = total_page;

                // enable/disable nút phân trang
                if (but_xemtiep != null) but_xemtiep.Enabled = current_page < total_page;
                if (but_quaylai != null) but_quaylai.Enabled = current_page > 1;

                var list = listQuery.Skip((current_page - 1) * show).Take(show).ToList();
                List<long> saleIds = list.Select(x => x.id).ToList();
                Dictionary<long, ShopSpecialExecution_cl.ExecutionTraceInfo> specialMap =
                    ShopSpecialExecution_cl.GetLatestMap(db, saleIds);

                // Map hiển thị: tính tổng VNĐ/A theo số lượng
                var list_show = list.Select(x =>
                {
                    int sl = (int)(x.SoLuong ?? 0);
                    long gia1VND = (long)(x.Gia_VND_SanPhamDichVu ?? 0);
                    decimal gia1A = (decimal)(x.Gia_DongA_SanPhamDichVu ?? 0m);

                    long tongVND = gia1VND * (long)sl;
                    decimal tongA = Math.Round(gia1A * sl, 2, MidpointRounding.AwayFromZero);
                    string tenSanPham = ResolveProductName(db, x.id_SanPhamDichVu, shopMode);
                    ShopSpecialExecution_cl.ExecutionTraceInfo specialTrace = null;
                    if (specialMap.ContainsKey(x.id))
                        specialTrace = specialMap[x.id];

                    return new
                    {
                        x.id,
                        x.TaiKhoan_Mua,
                        x.TaiKhoan_Ban,
                        TenSanPham = tenSanPham,

                        SoLuong = sl,
                        TongVND = tongVND,
                        TongDongA = tongA,

                        x.PhanTram_ChiTra_ChoSan,
                        x.ViLoiNhuan_DongA_CuaSan_NhanDuoc_ViTong,
                        x.Vi3_20PhanTram_NhanDuoc_ViGanKet,
                        x.Vi1_30PhanTram_NhanDuoc_ViEVoucher,
                        x.Vi2_50PhanTram_NhanDuoc_ViLaoDong,

                        ThoiGian_Text = (x.ThoiGian ?? DateTime.MinValue).ToString("dd/MM/yyyy HH:mm"),
                        SpecialExecutionLabel = specialTrace == null ? "" : (specialTrace.HandlerLabel ?? "")
                    };
                }).ToList();

                // label hiển thị
                int stt = (show * current_page) - show + 1;
                int s2 = stt + list.Count - 1;
                if (total_record > 0) lb_show.Text = stt + "-" + s2 + " trong số " + total_record.ToString("#,##0");
                else lb_show.Text = "0-0/0";

                Repeater1.DataSource = list_show;
                Repeater1.DataBind();

                if (up_main != null) up_main.Update();
            }
        }
        catch (Exception ex)
        {
            string actor = GetSellerAccount();
            if (string.IsNullOrEmpty(actor))
                actor = IsShopPortalMode() ? "shop" : "admin";
            Log_cl.Add_Log(ex.Message, actor, ex.StackTrace);
            lb_show.Text = "";
        }
    }

    // ======================= PAGING BUTTONS =======================
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        int p = 1;
        int.TryParse((ViewState["current_page_bansp"] ?? "1").ToString(), out p);
        p--;
        if (p < 1) p = 1;
        ViewState["current_page_bansp"] = p;
        show_main();
    }

    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        int p = 1, total = 1;
        int.TryParse((ViewState["current_page_bansp"] ?? "1").ToString(), out p);
        int.TryParse((ViewState["total_page_bansp"] ?? "1").ToString(), out total);

        p++;
        if (p > total) p = total;
        ViewState["current_page_bansp"] = p;
        show_main();
    }

    // ======================= OPEN DETAIL FULL-PAGE =======================
    protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "chitiet")
        {
            long id = 0;
            long.TryParse(e.CommandArgument.ToString(), out id);
            if (id > 0)
            {
                load_chitiet(id);
            }
        }
    }

    void load_chitiet(long idLichSu)
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                var hd = db.ViLoiNhuan_LichSuBanHang_tbs.FirstOrDefault(x => x.id == idLichSu);
                if (hd == null)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                        thongbao_class.metro_notifi("Thông báo", "Không tìm thấy giao dịch.", "2600", "warning"), true);
                    return;
                }

                bool shopMode = IsShopPortalMode();
                string sellerAccount = GetSellerAccount();
                if (shopMode && !string.IsNullOrEmpty(sellerAccount))
                {
                    string tkBan = (hd.TaiKhoan_Ban ?? "").Trim().ToLowerInvariant();
                    if (!string.Equals(tkBan, sellerAccount, StringComparison.OrdinalIgnoreCase))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                            thongbao_class.metro_notifi("Thông báo", "Bạn không có quyền xem giao dịch này.", "2600", "warning"), true);
                        return;
                    }
                }

                int sl = (int)(hd.SoLuong ?? 0);
                long gia1VND = (long)(hd.Gia_VND_SanPhamDichVu ?? 0);
                decimal gia1A = (decimal)(hd.Gia_DongA_SanPhamDichVu ?? 0m);

                long tongVND = gia1VND * (long)sl;
                decimal tongA = Math.Round(gia1A * sl, 2, MidpointRounding.AwayFromZero);

                // Header labels
                lb_ct_id.Text = hd.id.ToString();
                lb_ct_thoigian.Text = (hd.ThoiGian ?? DateTime.MinValue).ToString("dd/MM/yyyy HH:mm");
                lb_ct_tk_mua.Text = (hd.TaiKhoan_Mua ?? "");
                lb_ct_tk_ban.Text = (hd.TaiKhoan_Ban ?? "");

                lb_ct_sanpham.Text = ResolveProductName(db, hd.id_SanPhamDichVu, shopMode);
                lb_ct_soluong.Text = sl.ToString("#,##0");
                lb_ct_tongvnd.Text = tongVND.ToString("#,##0") + " VNĐ";
                lb_ct_tongdonga.Text = tongA.ToString("#,##0.##") + " A";
                lb_ct_pt_san.Text = ((decimal)(hd.PhanTram_ChiTra_ChoSan ?? 0m)).ToString("0.##") + " %";

                lb_ct_vitong.Text = ((decimal)(hd.ViLoiNhuan_DongA_CuaSan_NhanDuoc_ViTong ?? 0m)).ToString("#,##0.##");
                lb_ct_vi1.Text = ((decimal)(hd.Vi1_30PhanTram_NhanDuoc_ViEVoucher  ?? 0m)).ToString("#,##0.##");
                lb_ct_vi2.Text = ((decimal)(hd.Vi2_50PhanTram_NhanDuoc_ViLaoDong ?? 0m)).ToString("#,##0.##");
                lb_ct_vi3.Text = ((decimal)(hd.Vi3_20PhanTram_NhanDuoc_ViGanKet  ?? 0m)).ToString("#,##0.##");

                // Detail rows (skip when host lacks LoaiHanhVi column)
                bool hasLoaiHanhVi = HasLoaiHanhViColumn(db);
                List<SaleDetailRow> listCt;
                if (hasLoaiHanhVi)
                {
                    listCt = db.ViLoiNhuan_LichSuBanHang_ChiTiet_tbs
                        .Where(x => x.id_LichSuBanHang == idLichSu)
                        .OrderBy(x => x.LoaiHanhVi)
                        .ThenBy(x => x.TaiKhoan_Nhan)
                        .Select(x => new SaleDetailRow
                        {
                            TaiKhoan_Nhan = x.TaiKhoan_Nhan,
                            DongANhanDuoc = x.DongANhanDuoc,
                            LoaiHanhVi = x.LoaiHanhVi,
                            ThoiGian = x.ThoiGian,
                            PhanTramNhanDuoc = x.PhanTramNhanDuoc
                        }).ToList();
                }
                else
                {
                    listCt = new List<SaleDetailRow>();
                }


                var showCt = listCt.Select(x => new
                {
                    x.TaiKhoan_Nhan,
                    x.DongANhanDuoc,
                    x.LoaiHanhVi,
                    HanhVi_Text = GetLoaiHanhViText(x.LoaiHanhVi),


                    PhanTramNhanDuoc_Text = (x.PhanTramNhanDuoc == null ? "" : (x.PhanTramNhanDuoc.ToString() + " %")), // ✅ thêm

                    ThoiGian_Text = x.ThoiGian.ToString("dd/MM/yyyy HH:mm"),
                }).ToList();


                RepeaterChiTiet.DataSource = showCt;
                RepeaterChiTiet.DataBind();

                if (hasLoaiHanhVi)
                {
                    decimal sum = showCt.Sum(x => (decimal)x.DongANhanDuoc);
                    lb_ct_note.Text = "Tổng cộng đã chia: " + sum.ToString("#,##0.##") + " A";
                }
                else
                {
                    lb_ct_note.Text = "Chi tiết phân bổ tạm ẩn do thiếu cột dữ liệu LoaiHanhVi.";
                }

                pn_chitiet.Visible = true;
                close_chitiet.NavigateUrl = BuildListUrl();
                BindSpecialTrace(db, idLichSu);
                if (up_chitiet != null) up_chitiet.Update();
                up_main.Visible = false;
            }
        }
        catch (Exception ex)
        {
            string actor = GetSellerAccount();
            if (string.IsNullOrEmpty(actor))
                actor = IsShopPortalMode() ? "shop" : "admin";
            Log_cl.Add_Log(ex.Message, actor, ex.StackTrace);
        }
    }

    private void BindSpecialTrace(dbDataContext db, long idLichSu)
    {
        List<ShopSpecialExecution_cl.ExecutionTraceInfo> traces = ShopSpecialExecution_cl.GetBySaleHistoryId(db, idLichSu);
        List<SaleSpecialTraceRow> rows = traces
            .Select(x => new SaleSpecialTraceRow
            {
                SaleHistoryId = x.SaleHistoryId,
                HandlerLabel = x.HandlerLabel,
                ExecutionSummary = x.ExecutionSummary,
                ExecutionData = x.ExecutionData,
                ExecutedAtText = x.ExecutedAtText,
                ExecutionStatus = (x.ExecutionStatus ?? "").Trim()
            })
            .ToList();

        if (pn_ct_special != null)
            pn_ct_special.Visible = rows.Count > 0;

        if (rpt_ct_special != null)
        {
            rpt_ct_special.DataSource = rows;
            rpt_ct_special.DataBind();
        }
    }

    string GetLoaiHanhViText(int? loaiHanhVi)
    {
        if (!loaiHanhVi.HasValue) return "Số dư về sàn";
        return HanhVi9Cap_cl.GetTenHanhViTheoLoai(loaiHanhVi);
    }



    protected void but_close_chitiet_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildListUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
    }
    string GetTreasuryWalletByBuyer(dbDataContext db, taikhoan_tb buyer)
    {
        if (db == null || buyer == null) return "";

        string scope = PortalScope_cl.ResolveScope(buyer.taikhoan, buyer.phanloai, buyer.permission);
        if (string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
            return "vitonggianhangdoitac";

        int tierHome = TierHome_cl.TinhTierHome(db, buyer.taikhoan);
        if (tierHome >= TierHome_cl.Tier3)
            return "vitongdonghanhhesinhthai";
        if (tierHome >= TierHome_cl.Tier2)
            return "vitongcongtacphattrien";

        return "vitongkhachhang";
    }


    // ======================= LOAD DROPDOWN (Đồng hành hệ sinh thái) =======================
    public void load_taikhoan_nhadautu()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                var list = db.taikhoan_tbs
       .Where(p => p.taikhoan != "admin"
                && (p.phanloai == null
                    || (p.phanloai != AccountType_cl.Treasury
                        && p.phanloai != AccountType_cl.LegacyTreasury
                        && !p.phanloai.StartsWith(AccountType_cl.Treasury + " ")
                        && !p.phanloai.StartsWith(AccountType_cl.LegacyTreasury + " ")))
                && p.permission != null
                && p.permission.Contains(PortalScope_cl.ScopeHome))
       .OrderBy(p => p.taikhoan)
       .Select(p => new
       {
           p.taikhoan,
           Text = p.taikhoan + " - " + (p.hoten ?? ""),  // hiển thị: taikhoan - hoten
           p.phanloai
       })
       .ToList();

                ddl_taikhoan_nhadautu.DataSource = list;
                ddl_taikhoan_nhadautu.DataTextField = "Text";
                ddl_taikhoan_nhadautu.DataValueField = "taikhoan";
                ddl_taikhoan_nhadautu.DataBind();

                ddl_taikhoan_nhadautu.Items.Insert(0, new ListItem("-- Chọn tài khoản --", ""));


            }
        }
        catch { }
    }

    // ======================= LOAD DROPDOWN (Sản phẩm) =======================
    public void load_sanpham()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                bool shopMode = IsShopPortalMode();
                if (shopMode)
                {
                    string seller = GetSellerAccount();
                    bool internalSpace = IsInternalShopSpace();
                    var listShopQuery = db.BaiViet_tbs
                        .Where(p => p.nguoitao == seller);

                    if (internalSpace)
                        listShopQuery = listShopQuery.Where(p => p.phanloai == CompanyShop_cl.ProductTypeInternal);
                    else
                        listShopQuery = listShopQuery.Where(p => p.phanloai == CompanyShop_cl.ProductTypePublic);

                    var listShop = listShopQuery
                        .OrderByDescending(p => p.ngaytao)
                        .Select(p => new
                        {
                            p.id,
                            p.name,
                            p.phanloai
                        }).ToList()
                        .Select(p => new
                        {
                            id = p.id,
                            TenSanPham = "[" + CompanyShop_cl.BuildProductTypeLabel(p.phanloai) + "] " + (p.name ?? ("SP#" + p.id))
                        }).ToList();

                    ddl_sanpham.DataSource = listShop;
                    ddl_sanpham.DataTextField = "TenSanPham";
                    ddl_sanpham.DataValueField = "id";
                    ddl_sanpham.DataBind();
                }
                else
                {
                    var list = db.SanPham_Aha_tbs
                        .OrderBy(p => p.TenSanPham)
                        .Select(p => new
                        {
                            p.id,
                            p.TenSanPham,
                            p.GiaBan,
                            p.PhanTramChoSan
                        }).ToList();

                    ddl_sanpham.DataSource = list;
                    ddl_sanpham.DataTextField = "TenSanPham";
                    ddl_sanpham.DataValueField = "id";
                    ddl_sanpham.DataBind();
                }

                ddl_sanpham.Items.Insert(0, new ListItem("-- Chọn sản phẩm --", ""));
            }
        }
        catch { }
    }

    // ======================= HELPERS =======================
    int clampSoLuong(int sl)
    {
        if (sl < 1) sl = 1;
        if (sl > 100000) sl = 100000;
        return sl;
    }

    int getIdSanPham()
    {
        int id = 0;
        int.TryParse(ddl_sanpham.SelectedValue, out id);
        return id;
    }

    long getGiaBanSanPham()
    {
        int id = getIdSanPham();
        if (id <= 0) return 0;

        using (dbDataContext db = new dbDataContext())
        {
            if (IsShopPortalMode())
            {
                string seller = GetSellerAccount();
                var spShop = db.BaiViet_tbs.FirstOrDefault(x =>
                    x.id == id
                    && x.nguoitao == seller
                    && (x.phanloai == CompanyShop_cl.ProductTypePublic || x.phanloai == CompanyShop_cl.ProductTypeInternal));

                if (spShop == null) return 0;
                return (long)(spShop.giaban ?? 0m);
            }
            else
            {
                var sp = db.SanPham_Aha_tbs.FirstOrDefault(x => x.id == id);
                if (sp == null) return 0;

                return (long)(sp.GiaBan ?? 0);
            }
        }
    }

    decimal getPhanTramChoSan()
    {
        int id = getIdSanPham();
        if (id <= 0) return 0;

        using (dbDataContext db = new dbDataContext())
        {
            if (IsShopPortalMode())
            {
                string seller = GetSellerAccount();
                var spShop = db.BaiViet_tbs.FirstOrDefault(x =>
                    x.id == id
                    && x.nguoitao == seller
                    && (x.phanloai == CompanyShop_cl.ProductTypePublic || x.phanloai == CompanyShop_cl.ProductTypeInternal));

                if (spShop == null) return 0;
                return CompanyShop_cl.GetPlatformSharePercent(spShop);
            }
            else
            {
                var sp = db.SanPham_Aha_tbs.FirstOrDefault(x => x.id == id);
                if (sp == null) return 0;

                return (decimal)(sp.PhanTramChoSan ?? 0);
            }
        }
    }

    string formatMoneyVND(long money)
    {
        return money.ToString("#,##0");
    }

    string formatDongAInternational(decimal dongA)
    {
        return dongA.ToString("#,##0.##");
    }

    void tinhTienVaDongA()
    {
        int sl = 1;
        if (!int.TryParse(txt_soluong.Text.Trim(), out sl)) sl = 1;
        sl = clampSoLuong(sl);
        txt_soluong.Text = sl.ToString();

        long giaBan = getGiaBanSanPham();
        decimal ptChoSan = getPhanTramChoSan();

        lb_phantram_san.Text = ptChoSan.ToString("0.##") + " %";

        if (giaBan <= 0)
        {
            lb_giatri.Text = "0";
            lb_dongA.Text = "0.00";
            return;
        }

        long tien = sl * giaBan;
        lb_giatri.Text = formatMoneyVND(tien);

        decimal dongA = Math.Round((decimal)tien / (decimal)VNDTOA, 2, MidpointRounding.AwayFromZero);
        lb_dongA.Text = formatDongAInternational(dongA);
    }

    // ======================= EVENTS (FULL-PAGE BÁN) =======================
    protected void but_show_form_banthe_Click(object sender, EventArgs e)
    {
        try
        {
            if (!EnsurePortalContext())
                return;
            Response.Redirect(BuildSellPageUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch { }
    }

    protected void but_close_form_banthe_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildListUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void txt_soluong_TextChanged(object sender, EventArgs e)
    {
        tinhTienVaDongA();
        up_banthe.Update();
    }

    protected void ddl_sanpham_SelectedIndexChanged(object sender, EventArgs e)
    {
        tinhTienVaDongA();
        up_banthe.Update();
    }
 
  

    // ======================= CORE: BÁN SẢN PHẨM (LOGIC MỚI) =======================
    protected void but_tao_banthe_Click(object sender, EventArgs e)
    {
        if (!EnsurePortalContext())
            return;

        if (pn_sell_error != null) pn_sell_error.Visible = false;
        string sellToken;
        if (!TryBeginSellToken(out sellToken))
        {
            ShowSellError("Giao dịch đang xử lý hoặc không hợp lệ. Vui lòng tải lại trang và thử lại.");
            return;
        }
        bool sellSuccess = false;

        try
        {
            // Validate
            if (string.IsNullOrEmpty(ddl_sanpham.SelectedValue))
            {
                ShowSellError("Vui lòng chọn sản phẩm trước khi bán.");
                return;
            }

            if (string.IsNullOrEmpty(ddl_taikhoan_nhadautu.SelectedValue))
            {
                ShowSellError("Vui lòng chọn tài khoản.");
                return;
            }

            int sl = 1;
            if (!int.TryParse(txt_soluong.Text.Trim(), out sl)) sl = 1;
            sl = clampSoLuong(sl);
            txt_soluong.Text = sl.ToString();

            string taiKhoanMua = ddl_taikhoan_nhadautu.SelectedValue.Trim();
            int idSanPham = getIdSanPham();
            string taiKhoanBan = GetSellerAccount();
            if (string.IsNullOrEmpty(taiKhoanBan))
                taiKhoanBan = IsShopPortalMode() ? ResolveCurrentShopAccount() : ResolveCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoanBan))
                taiKhoanBan = IsShopPortalMode() ? "" : "admin";

            if (string.IsNullOrEmpty(taiKhoanBan))
            {
                ShowSellError("Không xác định được tài khoản bán.");
                return;
            }

            DateTime now = AhaTime_cl.Now;

            using (dbDataContext db = new dbDataContext())
            {
                db.CommandTimeout = 30;
                db.Connection.Open();

                using (var tran = db.Connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    db.Transaction = tran;

                    bool shopMode = IsShopPortalMode();
                    long giaBan1 = 0;
                    decimal ptChoSan = 0;
                    BaiViet_tb shopPortalProduct = null;
                    ShopSpecialProduct_cl.ExecutionResult specialExecution = null;

                    if (shopMode)
                    {
                        BaiViet_tb spShop = ResolveShopPortalProduct(db, idSanPham, taiKhoanBan);
                        if (spShop == null)
                            throw new Exception("Sản phẩm không tồn tại hoặc không thuộc gian hàng đối tác công ty.");

                        shopPortalProduct = spShop;
                        giaBan1 = (long)(spShop.giaban ?? 0m);
                        ptChoSan = CompanyShop_cl.GetPlatformSharePercent(spShop);
                    }
                    else
                    {
                        var sp = db.SanPham_Aha_tbs.FirstOrDefault(x => x.id == idSanPham);
                        if (sp == null)
                            throw new Exception("Sản phẩm không tồn tại.");

                        giaBan1 = (long)(sp.GiaBan ?? 0);
                        ptChoSan = (decimal)(sp.PhanTramChoSan ?? 0);
                    }

                    if (giaBan1 <= 0)
                        throw new Exception("Giá bán sản phẩm không hợp lệ.");

                    long tongVND = giaBan1 * (long)sl;

                    decimal tongDongA = Math.Round((decimal)tongVND / (decimal)VNDTOA, 2, MidpointRounding.AwayFromZero);
                    decimal gia1DongA = Math.Round((decimal)giaBan1 / (decimal)VNDTOA, 2, MidpointRounding.AwayFromZero);

                    // Lấy người mua
                    var obMua = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == taiKhoanMua);
                    if (obMua == null)
                        throw new Exception("Tài khoản mua không tồn tại.");
                    bool taoMoiHomeRoot;
                    taikhoan_tb homeRootAcc = HomeRoot_cl.GetOrCreate(db, out taoMoiHomeRoot);
                    if (homeRootAcc == null || string.IsNullOrWhiteSpace(homeRootAcc.taikhoan))
                        throw new Exception("Không xác định được tài khoản home_root.");
                    string homeRootAccount = homeRootAcc.taikhoan.Trim();

                    // =======================
                    // 1) CHUYỂN Quyền tiêu dùng: tài khoản tổng phù hợp -> người mua
                    // =======================
                    string viTong = GetTreasuryWalletByBuyer(db, obMua);

                    if (string.IsNullOrEmpty(viTong))
                    {
                        tran.Rollback();
                        ShowSellError("Không xác định được tài khoản tổng cho tài khoản mua.");
                        return;
                    }

                    string msg = "";
                    bool ok = Helper_DongA_cl.TransferTreasuryToUser(db, viTong, taiKhoanMua, tongDongA, out msg);
                    if (!ok)
                    {
                        tran.Rollback();
                        ShowSellError(msg);
                        return;
                    }

                    // =======================
                    // 2) TÍNH VÍ SÀN
                    // =======================
                    decimal viTongSan = Math.Round(tongDongA * ptChoSan / 100m, 2, MidpointRounding.AwayFromZero);
                    decimal vi1_30 = Math.Round(viTongSan * 0.30m, 2, MidpointRounding.AwayFromZero);
                    decimal vi2_50 = Math.Round(viTongSan * 0.50m, 2, MidpointRounding.AwayFromZero);
                    decimal vi3_20 = Math.Round(viTongSan * 0.20m, 2, MidpointRounding.AwayFromZero);

                    // =======================
                    // 3) INSERT HEADER (LỊCH SỬ BÁN)
                    // =======================
                    var hd = new ViLoiNhuan_LichSuBanHang_tb();
                    hd.TaiKhoan_Ban = taiKhoanBan;
                    hd.TaiKhoan_Mua = taiKhoanMua;
                    hd.id_SanPhamDichVu = idSanPham;
                    hd.SanPham_Or_Dichvu = true;

                    hd.SoLuong = sl;
                    hd.Gia_VND_SanPhamDichVu = giaBan1;
                    hd.Gia_DongA_SanPhamDichVu = gia1DongA;

                    hd.PhanTram_ChiTra_ChoSan = ptChoSan;
                    hd.ViLoiNhuan_DongA_CuaSan_NhanDuoc_ViTong = viTongSan;

                    hd.Vi1_30PhanTram_NhanDuoc_ViEVoucher = vi1_30;
                    hd.Vi2_50PhanTram_NhanDuoc_ViLaoDong = vi2_50;
                    hd.Vi3_20PhanTram_NhanDuoc_ViGanKet = vi3_20;

                    hd.ThoiGian = now;

                    db.ViLoiNhuan_LichSuBanHang_tbs.InsertOnSubmit(hd);
                    db.SubmitChanges();

                    long idLichSu = hd.id;

                    // =======================
                    // 4) CHIA VÍ1: 15% / 9% / 6%  (NEW: TRUY NGƯỢC AFFILIATE LIKE VÍ 2)
                    // - base tính trên viTongSan
                    // - dư trừ vào vi1_30
                    // =======================
                    decimal baseTinh15_9_6 = viTongSan;

                    decimal tienBuyer = Math.Round(baseTinh15_9_6 * 0.15m, 2, MidpointRounding.AwayFromZero);
                    decimal tien9 = Math.Round(baseTinh15_9_6 * 0.09m, 2, MidpointRounding.AwayFromZero);
                    decimal tien6 = Math.Round(baseTinh15_9_6 * 0.06m, 2, MidpointRounding.AwayFromZero);

                    // Buyer nhận 15% (luôn nhận)
                    AddVi1ChiTietAndCongSoDu(db, idLichSu, taiKhoanMua, tienBuyer, 15, now);

                    // 9%: truy ngược affiliate để tìm người đủ điều kiện nhận 9
                    decimal tien9Thuc = 0m;
                    string tkNhan9 = FindNguoiNhanVi1_ByPercent(db, obMua, 9);

                    if (!string.IsNullOrEmpty(tkNhan9)
                        && tkNhan9 != taiKhoanMua
                        && tien9 > 0)
                    {
                        AddVi1ChiTietAndCongSoDu(db, idLichSu, tkNhan9, tien9, 9, now);
                        tien9Thuc = tien9;
                    }
                    else if (tien9 > 0)
                    {
                        AddVi1ChiTietAndCongSoDu(db, idLichSu, homeRootAccount, tien9, 9, now);
                        tien9Thuc = tien9;
                    }

                    // 6%: truy ngược affiliate để tìm người đủ điều kiện nhận 6
                    decimal tien6Thuc = 0m;
                    string tkNhan6 = FindNguoiNhanVi1_ByPercent(db, obMua, 6);

                    if (!string.IsNullOrEmpty(tkNhan6)
                        && tkNhan6 != taiKhoanMua
                        && tkNhan6 != tkNhan9        // tránh trùng người nhận 9 (an toàn)
                        && tien6 > 0)
                    {
                        AddVi1ChiTietAndCongSoDu(db, idLichSu, tkNhan6, tien6, 6, now);
                        tien6Thuc = tien6;
                    }
                    else if (tien6 > 0)
                    {
                        AddVi1ChiTietAndCongSoDu(db, idLichSu, homeRootAccount, tien6, 6, now);
                        tien6Thuc = tien6;
                    }

                    // =======================
                    // 5) DƯ VÍ1 -> ADMIN
                    // DuVi1 = vi1_30 - (15 + 9_thực + 6_thực)
                    // =======================
                    decimal tongDaChia_Vi1 = tienBuyer + tien9Thuc + tien6Thuc;

                    decimal duVi1 = Math.Round(vi1_30 - tongDaChia_Vi1, 2, MidpointRounding.AwayFromZero);
                    if (duVi1 < 0) duVi1 = 0m;


                    // =======================
                    // 6) CHIA VÍ2 (50%) - NEW: 10% / 15% / 25%
                    // RULE:
                    // - base = viTongSan
                    // - dư trừ vào quỹ 50% (vi2_50)
                    // - truy ngược affiliate, gặp người đủ điều kiện theo mức thì dừng
                    // - không tìm thấy => phần đó về admin
                    // =======================
                    decimal baseTinh10_15_25 = viTongSan;

                    decimal tien10 = Math.Round(baseTinh10_15_25 * 0.10m, 2, MidpointRounding.AwayFromZero);
                    decimal tien15 = Math.Round(baseTinh10_15_25 * 0.15m, 2, MidpointRounding.AwayFromZero);
                    decimal tien25 = Math.Round(baseTinh10_15_25 * 0.25m, 2, MidpointRounding.AwayFromZero);

                    decimal tongDaChia_Vi2 = 0m;

                    // 10% -> LoaiHanhVi=4 -> Vi4_10PhanTram
                    string tkNhan10 = FindNguoiNhanVi2_ByPercent(db, obMua, 10);
                    if (!string.IsNullOrEmpty(tkNhan10) && tien10 > 0)
                    {
                        AddVi2ChiTietAndCongSoDu(db, idLichSu, tkNhan10, tien10, loaiHanhVi: 4, phanTram: 10, now: now);
                        tongDaChia_Vi2 += tien10;
                    }
                    else if (tien10 > 0)
                    {
                        AddVi2ChiTietAndCongSoDu(db, idLichSu, homeRootAccount, tien10, loaiHanhVi: 4, phanTram: 10, now: now);
                        tongDaChia_Vi2 += tien10;
                    }

                    // 15% -> LoaiHanhVi=5 -> Vi5_15PhanTram
                    string tkNhan15 = FindNguoiNhanVi2_ByPercent(db, obMua, 15);
                    if (!string.IsNullOrEmpty(tkNhan15) && tien15 > 0)
                    {
                        AddVi2ChiTietAndCongSoDu(db, idLichSu, tkNhan15, tien15, loaiHanhVi: 5, phanTram: 15, now: now);
                        tongDaChia_Vi2 += tien15;
                    }
                    else if (tien15 > 0)
                    {
                        AddVi2ChiTietAndCongSoDu(db, idLichSu, homeRootAccount, tien15, loaiHanhVi: 5, phanTram: 15, now: now);
                        tongDaChia_Vi2 += tien15;
                    }

                    // 25% -> LoaiHanhVi=6 -> Vi6_25PhanTram
                    string tkNhan25 = FindNguoiNhanVi2_ByPercent(db, obMua, 25);
                    if (!string.IsNullOrEmpty(tkNhan25) && tien25 > 0)
                    {
                        AddVi2ChiTietAndCongSoDu(db, idLichSu, tkNhan25, tien25, loaiHanhVi: 6, phanTram: 25, now: now);
                        tongDaChia_Vi2 += tien25;
                    }
                    else if (tien25 > 0)
                    {
                        AddVi2ChiTietAndCongSoDu(db, idLichSu, homeRootAccount, tien25, loaiHanhVi: 6, phanTram: 25, now: now);
                        tongDaChia_Vi2 += tien25;
                    }

                    // DƯ VÍ2 = vi2_50 - (tiền thực trả 10/15/25)
                    decimal duVi2 = Math.Round(vi2_50 - tongDaChia_Vi2, 2, MidpointRounding.AwayFromZero);
                    if (duVi2 < 0) duVi2 = 0m;

                    // =======================
                    // 7) VÍ3 (20%) - NEW: TRUY NGƯỢC AFFILIATE (4% / 6% / 10)
                    // =======================
                    decimal baseTinh4_6_10_v3 = viTongSan;

                    decimal tien4_v3 = Math.Round(baseTinh4_6_10_v3 * 0.04m, 2, MidpointRounding.AwayFromZero);
                    decimal tien6_v3 = Math.Round(baseTinh4_6_10_v3 * 0.06m, 2, MidpointRounding.AwayFromZero);
                    decimal tien10_v3 = Math.Round(baseTinh4_6_10_v3 * 0.10m, 2, MidpointRounding.AwayFromZero);

                    decimal daChiaVi3 = 0m;

                    // 4%
                    string tkNhan4_v3 = FindNguoiNhanVi3_ByPercent(db, obMua, 4);
                    if (!string.IsNullOrEmpty(tkNhan4_v3) && tien4_v3 > 0)
                    {
                        AddVi3ChiTietAndCongSoDu(db, idLichSu, tkNhan4_v3, tien4_v3, 4, now);
                        daChiaVi3 += tien4_v3;
                    }
                    else if (tien4_v3 > 0)
                    {
                        AddVi3ChiTietAndCongSoDu(db, idLichSu, homeRootAccount, tien4_v3, 4, now);
                        daChiaVi3 += tien4_v3;
                    }

                    // 6%
                    string tkNhan6_v3 = FindNguoiNhanVi3_ByPercent(db, obMua, 6);
                    if (!string.IsNullOrEmpty(tkNhan6_v3)
                        && tkNhan6_v3 != tkNhan4_v3
                        && tien6_v3 > 0)
                    {
                        AddVi3ChiTietAndCongSoDu(db, idLichSu, tkNhan6_v3, tien6_v3, 6, now);
                        daChiaVi3 += tien6_v3;
                    }
                    else if (tien6_v3 > 0)
                    {
                        AddVi3ChiTietAndCongSoDu(db, idLichSu, homeRootAccount, tien6_v3, 6, now);
                        daChiaVi3 += tien6_v3;
                    }

                    // 10%
                    string tkNhan10_v3 = FindNguoiNhanVi3_ByPercent(db, obMua, 10);
                    if (!string.IsNullOrEmpty(tkNhan10_v3)
                        && tkNhan10_v3 != tkNhan4_v3
                        && tkNhan10_v3 != tkNhan6_v3
                        && tien10_v3 > 0)
                    {
                        AddVi3ChiTietAndCongSoDu(db, idLichSu, tkNhan10_v3, tien10_v3, 10, now);
                        daChiaVi3 += tien10_v3;
                    }
                    else if (tien10_v3 > 0)
                    {
                        AddVi3ChiTietAndCongSoDu(db, idLichSu, homeRootAccount, tien10_v3, 10, now);
                        daChiaVi3 += tien10_v3;
                    }

                    // DƯ VÍ3 -> ADMIN
                    decimal duVi3 = Math.Round(vi3_20 - daChiaVi3, 2, MidpointRounding.AwayFromZero);
                    if (duVi3 < 0) duVi3 = 0m;


                    // =======================
                    // 8) DƯ LÀM TRÒN VỀ HOME_ROOT
                    // Phần dư sau chia (do làm tròn) dồn về home_root để không thất thoát.
                    // =======================
                    if (duVi1 > 0m)
                    {
                        homeRootAcc.DuVi1_Evocher_30PhanTram =
                            (homeRootAcc.DuVi1_Evocher_30PhanTram ?? 0m) + duVi1;
                        Helper_DongA_cl.AddLedger(
                            db,
                            homeRootAccount,
                            duVi1,
                            true,
                            "Dư làm tròn hồ sơ quyền ưu đãi từ ID: " + idLichSu,
                            "SALE_ROUNDING:" + idLichSu + ":2",
                            2,
                            1);
                    }

                    if (duVi2 > 0m)
                    {
                        homeRootAcc.DuVi2_LaoDong_50PhanTram =
                            (homeRootAcc.DuVi2_LaoDong_50PhanTram ?? 0m) + duVi2;
                        Helper_DongA_cl.AddLedger(
                            db,
                            homeRootAccount,
                            duVi2,
                            true,
                            "Dư làm tròn hồ sơ hành vi lao động từ ID: " + idLichSu,
                            "SALE_ROUNDING:" + idLichSu + ":3",
                            3,
                            4);
                    }

                    if (duVi3 > 0m)
                    {
                        homeRootAcc.DuVi3_GanKet_20PhanTram =
                            (homeRootAcc.DuVi3_GanKet_20PhanTram ?? 0m) + duVi3;
                        Helper_DongA_cl.AddLedger(
                            db,
                            homeRootAccount,
                            duVi3,
                            true,
                            "Dư làm tròn hồ sơ chỉ số gắn kết từ ID: " + idLichSu,
                            "SALE_ROUNDING:" + idLichSu + ":4",
                            4,
                            7);
                    }

                    // =======================
                    // 9) LƯU DƯ VÀO HEADER
                    // =======================
                    hd.DuVi1_30PhanTram_NhanDuoc_ViEVoucher = duVi1;
                    hd.DuVi2_50PhanTram_NhanDuoc_ViLaoDong = duVi2; // ✅ NEW: lưu đúng dư (không còn là "toàn bộ 50%")
                    hd.DuVi3_20PhanTram_NhanDuoc_ViGanKet = duVi3;

                    if (shopMode && shopPortalProduct != null)
                    {
                        specialExecution = ShopSpecialProduct_cl.ExecuteForSale(
                            db,
                            new ShopSpecialProduct_cl.SaleContext
                            {
                                SellerAccount = taiKhoanBan,
                                ProductPost = shopPortalProduct,
                                BuyerAccount = obMua,
                                SaleHistoryId = idLichSu,
                                Quantity = sl,
                                Now = now,
                                Actor = taiKhoanBan
                            });
                    }

                    db.SubmitChanges();
                    tran.Commit();
                    sellSuccess = true;

                    string buyerDisplay = taiKhoanMua;
                    if (obMua != null)
                    {
                        string hoten = (obMua.hoten ?? "").Trim();
                        if (!string.IsNullOrEmpty(hoten))
                            buyerDisplay = hoten + " (" + taiKhoanMua + ")";
                    }

                    string successMessage = "Đã bán thành công cho tài khoản " + buyerDisplay + ".";
                    if (specialExecution != null
                        && specialExecution.Applied
                        && !string.IsNullOrWhiteSpace(specialExecution.Summary))
                    {
                        successMessage += " " + specialExecution.Summary;
                    }

                    Session["thongbao"] = thongbao_class.metro_notifi_onload(
                        "Thông báo",
                        successMessage,
                        "2600",
                        "success");
                    Response.Redirect(BuildListUrl(), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
            }
        }
        catch (SqlException sqlEx)
        {
            if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
            {
                ShowSellError("Giao dịch có thể đã được ghi trước đó (trùng dữ liệu). Vui lòng kiểm tra lịch sử trước khi thao tác lại.");
                return;
            }

            string actor = GetSellerAccount();
            if (string.IsNullOrEmpty(actor))
                actor = IsShopPortalMode() ? "shop" : "admin";
            Log_cl.Add_Log(sqlEx.Message, actor, sqlEx.StackTrace);
            ShowSellError("Lỗi SQL: " + sqlEx.Message);
        }
        catch (Exception ex)
        {
            string actor = GetSellerAccount();
            if (string.IsNullOrEmpty(actor))
                actor = IsShopPortalMode() ? "shop" : "admin";
            Log_cl.Add_Log(ex.Message, actor, ex.StackTrace);
            ShowSellError("Có lỗi xảy ra: " + ex.Message);
        }
        finally
        {
            CompleteSellToken(sellToken, sellSuccess);
            if (!sellSuccess)
                EnsureSellToken();
        }
    }

    #region ======================= VÍ 2 (50%) - HELPERS NEW =======================

    /// <summary>
    /// Tìm người nhận theo % (10/15/25) bằng cách truy ngược affiliate.
    /// Bắt đầu từ cấp trên trực tiếp của người mua.
    /// Gặp tài khoản đủ điều kiện (cap 2/3 + mở đúng mức) thì dừng.
    /// Không tìm thấy => return "" (phần đó về admin).
    /// </summary>
    private string FindNguoiNhanVi2_ByPercent(dbDataContext db, taikhoan_tb obMua, int percent)
    {
        if (obMua == null) return "";

        string cur = (obMua.Affiliate_tai_khoan_cap_tren ?? "").Trim();
        int safe = 0;

        while (!string.IsNullOrEmpty(cur) && safe < 200)
        {
            safe++;

            var u = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == cur);
            if (u == null) return "";

            if (DuDieuKienNhanVi2(u, percent))
                return cur;

            cur = (u.Affiliate_tai_khoan_cap_tren ?? "").Trim();
        }

        return "";
    }

    /// <summary>
    /// Điều kiện nhận ví 2 theo mức 10/15/25:
    /// - HeThongSanPham_Cap123 = 2 hoặc 3
    /// - HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 = đúng percent (10/15/25)
    /// </summary>
    private bool DuDieuKienNhanVi2(taikhoan_tb u, int percent)
{
    if (u == null) return false;

    int cap = (int)(u.HeThongSanPham_Cap123 ?? 0);
    if (cap != 2 && cap != 3) return false;

    int quyen = (int)(u.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 ?? 0);
    return quyen == percent;
}



 

    #endregion




    // ======================= VÍ 1 (30%) - ADD + TỔNG (NEW) =======================
    void AddVi1ChiTietAndCongSoDu(
        dbDataContext db,
        long idLichSu,
        string taiKhoanNhan,
        decimal soTien,
        int phanTramNhanDuoc,
        DateTime now)
    {
        if (db == null) throw new Exception("DB null.");
        if (soTien <= 0) return;
        if (string.IsNullOrEmpty(taiKhoanNhan)) return;

        var acc = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == taiKhoanNhan);
        if (acc == null) return;

        // Chỉ áp dụng cho 15% / 9% / 6% (Vi1/Vi2/Vi3)
        int loaiHanhVi;
        switch (phanTramNhanDuoc)
        {
            case 15: loaiHanhVi = 1; break; // Vi1_15PhanTram
            case 9: loaiHanhVi = 2; break; // Vi2_9PhanTram
            case 6: loaiHanhVi = 3; break; // Vi3_6PhanTram
            default:
                throw new Exception("AddVi1ChiTietAndCongSoDu chỉ hỗ trợ % nhận: 15 / 9 / 6. Đang nhận: "
                                    + phanTramNhanDuoc + "%");
        }

        bool canWriteDetail = HasLoaiHanhViColumn(db);
        if (canWriteDetail)
        {
            // CHỐNG TRÙNG KEY (idLichSu + taiKhoanNhan + loaiHanhVi)
            bool existed = db.ViLoiNhuan_LichSuBanHang_ChiTiet_tbs.Any(x =>
                x.id_LichSuBanHang == idLichSu
                && x.TaiKhoan_Nhan == taiKhoanNhan
                && x.LoaiHanhVi == loaiHanhVi);

            if (existed) return;

            db.ViLoiNhuan_LichSuBanHang_ChiTiet_tbs.InsertOnSubmit(new ViLoiNhuan_LichSuBanHang_ChiTiet_tb()
            {
                id_LichSuBanHang = idLichSu,
                TaiKhoan_Nhan = taiKhoanNhan,
                DongANhanDuoc = soTien,
                LoaiHanhVi = loaiHanhVi,
                PhanTramNhanDuoc = phanTramNhanDuoc,
                ThoiGian = now,
                GhiChu = ""
            });
        }

        // Cộng số dư vào đúng hành vi (15/9/6)
        switch (loaiHanhVi)
        {
            case 1:
                acc.Vi1_15PhanTram = (decimal)(acc.Vi1_15PhanTram ?? 0m) + soTien;
                break;
            case 2:
                acc.Vi2_9PhanTram = (decimal)(acc.Vi2_9PhanTram ?? 0m) + soTien;
                break;
            case 3:
                acc.Vi3_6PhanTram = (decimal)(acc.Vi3_6PhanTram ?? 0m) + soTien;
                break;
        }

        // ✅ NEW: Cộng dồn tổng nhóm Ví1 (Evoucher 30%)
        acc.DuVi1_Evocher_30PhanTram = (decimal)(acc.DuVi1_Evocher_30PhanTram ?? 0m) + soTien;
        string ledgerRef = "SALE_DIST:" + idLichSu + ":" + acc.taikhoan + ":" + loaiHanhVi;
        Helper_DongA_cl.AddLedger(
            db,
            acc.taikhoan,
            soTien,
            true, // CongTru = true (cộng)
            "Ghi nhận quyền ưu đãi từ ID: " + idLichSu,
            ledgerRef,
            2,
            loaiHanhVi);

        // chưa SubmitChanges() ở đây, để batch submit cuối transaction
    }


    // ======================= VÍ 2 (50%) - ADD + TỔNG (NEW) =======================
    /// <summary>
    /// Cộng số dư vào đúng trường ví 2:
    /// - LoaiHanhVi=4 => Vi4_10PhanTram
    /// - LoaiHanhVi=5 => Vi5_15PhanTram
    /// - LoaiHanhVi=6 => Vi6_25PhanTram
    /// Đồng thời insert chi tiết lịch sử.
    /// ✅ NEW: cộng dồn tổng nhóm Ví2 (DuVi2_LaoDong_50PhanTram)
    /// </summary>
    private void AddVi2ChiTietAndCongSoDu(
        dbDataContext db,
        long idLichSu,
        string tkNhan,
        decimal soTien,
        int loaiHanhVi,
        int phanTram,
        DateTime now)
    {
        if (db == null) throw new Exception("DB null.");
        if (string.IsNullOrEmpty(tkNhan)) return;
        if (soTien <= 0) return;

        var u = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == tkNhan);
        if (u == null) return;

        bool canWriteDetail = HasLoaiHanhViColumn(db);
        if (canWriteDetail)
        {
            // Chống trùng key (id+tk+loaiHanhVi): nếu đã có thì bỏ qua toàn bộ để tránh cộng/trừ lặp.
            bool existed = db.ViLoiNhuan_LichSuBanHang_ChiTiet_tbs.Any(x =>
                x.id_LichSuBanHang == idLichSu
                && x.TaiKhoan_Nhan == tkNhan
                && x.LoaiHanhVi == loaiHanhVi);
            if (existed) return;
        }

        // Cộng vào đúng hành vi (10/15/25)
        if (loaiHanhVi == 4)
            u.Vi4_10PhanTram = (decimal)(u.Vi4_10PhanTram ?? 0m) + soTien;
        else if (loaiHanhVi == 5)
            u.Vi5_15PhanTram = (decimal)(u.Vi5_15PhanTram ?? 0m) + soTien;
        else if (loaiHanhVi == 6)
            u.Vi6_25PhanTram = (decimal)(u.Vi6_25PhanTram ?? 0m) + soTien;
        else
            throw new Exception("Loại hành vi nhóm 2 không hợp lệ (chỉ 4/5/6).");

        // ✅ NEW: Cộng dồn tổng nhóm Ví2 (Lao động 50%)
        u.DuVi2_LaoDong_50PhanTram = (decimal)(u.DuVi2_LaoDong_50PhanTram ?? 0m) + soTien;
        string ledgerRef = "SALE_DIST:" + idLichSu + ":" + u.taikhoan + ":" + loaiHanhVi;
        Helper_DongA_cl.AddLedger(
            db,
            u.taikhoan,
            soTien,
            true,
            "Ghi nhận hành vi lao động từ ID: " + idLichSu,
            ledgerRef,
            3,
            loaiHanhVi);
        if (canWriteDetail)
        {
            db.ViLoiNhuan_LichSuBanHang_ChiTiet_tbs.InsertOnSubmit(new ViLoiNhuan_LichSuBanHang_ChiTiet_tb()
            {
                id_LichSuBanHang = idLichSu,
                TaiKhoan_Nhan = tkNhan,
                DongANhanDuoc = soTien,
                LoaiHanhVi = loaiHanhVi,                // 4/5/6
                PhanTramNhanDuoc = phanTram,    // 10/15/25
                ThoiGian = now,
                GhiChu = ""
            });
        }

        // chưa SubmitChanges() ở đây, để batch submit cuối transaction
    }


    // ======================= VÍ 3 (20%) - ADD + TỔNG (NEW) =======================
    /// <summary>
    /// Cộng số dư vào đúng field Ví 3 + insert chi tiết lịch sử.
    /// - 10% -> LoaiHanhVi = 7 (Hành vi phúc lợi)  -> Vi9_10PhanTram
    /// - 6%  -> LoaiHanhVi = 8 (Hành vi ghi nhận)  -> Vi8_6PhanTram
    /// - 4%  -> LoaiHanhVi = 9 (Hành vi chăm sóc)  -> Vi7_4PhanTram
    /// ✅ NEW: cộng dồn tổng nhóm Ví3 (DuVi3_GanKet_20PhanTram)
    /// </summary>
    private void AddVi3ChiTietAndCongSoDu(
        dbDataContext db,
        long idLichSu,
        string tkNhan,
        decimal soTien,
        int percent,
        DateTime now)
    {
        if (db == null) throw new Exception("DB null.");
        if (string.IsNullOrEmpty(tkNhan)) return;
        if (soTien <= 0) return;

        var u = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == tkNhan);
        if (u == null) return;

        int loaiHanhVi;
        switch (percent)
        {
            case 10:
                loaiHanhVi = 7;
                u.Vi9_10PhanTram = (decimal)(u.Vi9_10PhanTram ?? 0m) + soTien;
                break;

            case 6:
                loaiHanhVi = 8;
                u.Vi8_6PhanTram = (decimal)(u.Vi8_6PhanTram ?? 0m) + soTien;
                break;

            case 4:
                loaiHanhVi = 9;
                u.Vi7_4PhanTram = (decimal)(u.Vi7_4PhanTram ?? 0m) + soTien;
                break;

            default:
                throw new Exception("Hành vi nhóm 3 chỉ hỗ trợ %: 4 / 6 / 10.");
        }

        bool canWriteDetail = HasLoaiHanhViColumn(db);
        if (canWriteDetail)
        {
            // Chống trùng key (id+tk+loaiHanhVi): nếu đã có thì bỏ qua toàn bộ để tránh cộng/trừ lặp.
            bool existed = db.ViLoiNhuan_LichSuBanHang_ChiTiet_tbs.Any(x =>
                x.id_LichSuBanHang == idLichSu
                && x.TaiKhoan_Nhan == tkNhan
                && x.LoaiHanhVi == loaiHanhVi);
            if (existed) return;
        }

        // ✅ NEW: Cộng dồn tổng nhóm Ví3 (Gắn kết 20%)
        u.DuVi3_GanKet_20PhanTram = (decimal)(u.DuVi3_GanKet_20PhanTram ?? 0m) + soTien;
        string ledgerRef = "SALE_DIST:" + idLichSu + ":" + u.taikhoan + ":" + loaiHanhVi;
        Helper_DongA_cl.AddLedger(
            db,
            u.taikhoan,
            soTien,
            true,
            "Ghi nhận chỉ số gắn kết từ ID: " + idLichSu,
            ledgerRef,
            4,
            loaiHanhVi);

        if (canWriteDetail)
        {
            db.ViLoiNhuan_LichSuBanHang_ChiTiet_tbs.InsertOnSubmit(
                new ViLoiNhuan_LichSuBanHang_ChiTiet_tb
                {
                    id_LichSuBanHang = idLichSu,
                    TaiKhoan_Nhan = tkNhan,
                    DongANhanDuoc = soTien,
                    LoaiHanhVi = loaiHanhVi,
                    PhanTramNhanDuoc = percent,
                    ThoiGian = now,
                    GhiChu = ""
                });
        }

        // chưa SubmitChanges() ở đây, để batch submit cuối transaction
    }



    bool DuDieuKienNhanCap1_9(taikhoan_tb acc)
    {
        if (acc == null) return false;

        int cap = (int)(acc.HeThongSanPham_Cap123 ?? 0);
        int quyen = (int)(acc.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 ?? 0);

        return (cap >= 1 && cap <= 3) && (quyen == 9);
    }

    bool DuDieuKienNhanCap2_6(taikhoan_tb acc)
    {
        if (acc == null) return false;

        int cap = (int)(acc.HeThongSanPham_Cap123 ?? 0);
        int quyen = (int)(acc.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 ?? 0);

        return (cap >= 1 && cap <= 3) && (quyen == 6);
    }
    #region ======================= VÍ 1 (30%) - HELPERS NEW (TRUY NGƯỢC LIKE VÍ 2) =======================

    /// <summary>
    /// Tìm người nhận ví 1 theo % (9 hoặc 6) bằng cách truy ngược affiliate.
    /// Bắt đầu từ cấp trên trực tiếp của người mua.
    /// Gặp tài khoản đủ điều kiện theo % thì dừng.
    /// Không tìm thấy => return "" (phần đó về admin qua dư ví1).
    /// </summary>
    private string FindNguoiNhanVi1_ByPercent(dbDataContext db, taikhoan_tb obMua, int percent)
    {
        if (db == null) return "";
        if (obMua == null) return "";

        // Bắt đầu từ cấp trên trực tiếp của người mua
        string cur = (obMua.Affiliate_tai_khoan_cap_tren ?? "").Trim();
        int safe = 0;

        while (!string.IsNullOrEmpty(cur) && safe < 200)
        {
            safe++;

            var u = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == cur);
            if (u == null) return ""; // chain đứt => coi như không tìm thấy

            if (DuDieuKienNhanVi1(u, percent))
                return cur;

            cur = (u.Affiliate_tai_khoan_cap_tren ?? "").Trim();
        }

        return "";
    }

    /// <summary>
    /// Điều kiện nhận ví 1:
    /// - percent = 9 => dùng DuDieuKienNhanCap1_9
    /// - percent = 6 => dùng DuDieuKienNhanCap2_6
    /// </summary>
    private bool DuDieuKienNhanVi1(taikhoan_tb u, int percent)
    {
        if (u == null) return false;

        if (percent == 9) return DuDieuKienNhanCap1_9(u);
        if (percent == 6) return DuDieuKienNhanCap2_6(u);

        return false; // ví1 chỉ hỗ trợ 9/6 theo rule hiện tại
    }

    #endregion
    #region ======================= VÍ 3 (20%) - HELPERS NEW =======================

    /// <summary>
    /// Tìm người nhận Ví 3 theo % (4 / 6 / 10) bằng cách truy ngược affiliate.
    /// Bắt đầu từ cấp trên trực tiếp của người mua.
    /// Gặp tài khoản đủ điều kiện thì dừng.
    /// Không tìm thấy => return "" (phần đó về admin).
    /// </summary>
    private string FindNguoiNhanVi3_ByPercent(dbDataContext db, taikhoan_tb obMua, int percent)
    {
        if (db == null || obMua == null) return "";

        string cur = (obMua.Affiliate_tai_khoan_cap_tren ?? "").Trim();
        int safe = 0;

        while (!string.IsNullOrEmpty(cur) && safe < 200)
        {
            safe++;

            var u = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == cur);
            if (u == null) return "";

            if (DuDieuKienNhanVi3(u, percent))
                return cur;

            cur = (u.Affiliate_tai_khoan_cap_tren ?? "").Trim();
        }

        return "";
    }

    /// <summary>
    /// Điều kiện nhận Ví 3:
    /// - Cap = 3
    /// - Quyền mở ví = percent (4 / 6 / 10)
    /// </summary>
    private bool DuDieuKienNhanVi3(taikhoan_tb u, int percent)
    {
        if (u == null) return false;

        int cap = (int)(u.HeThongSanPham_Cap123 ?? 0);
        if (cap != 3) return false;

        int quyen = (int)(u.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 ?? 0);
        return quyen == percent;
    }

  

    #endregion

}
