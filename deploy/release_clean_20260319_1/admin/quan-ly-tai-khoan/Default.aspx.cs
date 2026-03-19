using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;

public partial class admin_Default : System.Web.UI.Page
{
    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();
    private const string ViewAdd = "add";
    private const string ViewEdit = "edit";
    private const string ViewFilter = "filter";
    private const string ViewPermission = "phanquyen";
    private const string AccountTypeAdminStaff = "Nhân viên admin";
    private const string AccountTypeHomeDefault = "Khách hàng";
    private const string AccountTypeShopDefault = "Gian hàng đối tác";
    private const string PermissionManageAdminAccounts = "5";

    private bool IsRootAdminCurrent()
    {
        string tk = (ViewState["taikhoan"] ?? "").ToString();
        return PermissionProfile_cl.IsRootAdmin(tk);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        bool isStandaloneView = IsStandaloneViewByQuery();
        bool isRepeaterRowPostback = IsRepeaterRowPostback();

        if (!IsPostBack)
        {
            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri;
            check_login_cl.check_login_admin("none", "none");

            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";

            ViewState["taikhoan"] = _tk;

            set_dulieu_macdinh();
            if (TryHandleToggleHomeLockRequest())
                return;

            show_main();

            ApplyOpenViewFromQuery();
        }
        else if (isRepeaterRowPostback)
        {
            // Rebind repeater controls early on postback so row-level LinkButton events fire correctly.
            show_main();
        }

        // Always render add button, then scope CSS decides display (admin scope shows it).
        // This avoids runtime visibility glitches where the control was not rendered in scope=admin.
        but_show_form_add.Visible = true;
        but_lock_legacy_home.Visible = IsRootAdminCurrent();

        if (isStandaloneView)
            up_main.Visible = false;

        but_show_form_add.NavigateUrl = BuildAddUrl();
        but_show_filter.NavigateUrl = BuildFilterUrl();
    }

    private bool IsRepeaterRowPostback()
    {
        string eventTarget = (Request["__EVENTTARGET"] ?? "").Trim();
        if (string.IsNullOrEmpty(eventTarget))
            return false;

        return eventTarget.StartsWith("ctl00$main$Repeater1$", StringComparison.OrdinalIgnoreCase);
    }

    public void set_dulieu_macdinh()
    {
        ViewState["current_page_qlnv"] = "1";
        ViewState["filter_phanloai"] = NormalizeFilterAccountType(Request.QueryString["ftype"]);

        string scopeFromPath = NormalizeFilterScope(Request.QueryString["scope"]);
        string scopeFromFilter = NormalizeFilterScope(Request.QueryString["fscope"]);
        ViewState["filter_scope"] = string.IsNullOrEmpty(scopeFromFilter) ? scopeFromPath : scopeFromFilter;
    }

    private string GetSearchKeyword()
    {
        string keyTop = txt_timkiem.Text.Trim();
        string keyMain = txt_timkiem1.Text.Trim();
        return !string.IsNullOrEmpty(keyTop) ? keyTop : keyMain;
    }

    private void SyncSearchInputs(string keyword)
    {
        string safe = (keyword ?? "").Trim();
        txt_timkiem.Text = safe;
        txt_timkiem1.Text = safe;
    }

    private string NormalizeFilterAccountType(string raw)
    {
        string type = (raw ?? "").Trim();
        return AccountType_cl.IsTreasury(type) ? AccountType_cl.Treasury : type;
    }

    private string NormalizeFilterScope(string raw)
    {
        string scope = (raw ?? "").Trim().ToLowerInvariant();
        if (scope == "admin" || scope == "home" || scope == "shop")
            return scope;
        return "";
    }

    private string NormalizeViewMode(string raw)
    {
        string mode = (raw ?? "").Trim().ToLowerInvariant();
        if (mode == ViewAdd || mode == ViewEdit || mode == ViewFilter || mode == ViewPermission)
            return mode;
        return "";
    }

    private string NormalizeSearchText(string raw)
    {
        string text = (raw ?? "").Trim();
        if (string.IsNullOrEmpty(text)) return "";

        try
        {
            text = str_cl.remove_vietnamchar(text);
        }
        catch
        {
            // keep original text if conversion fails
        }

        text = str_cl.Remove_Blank(text).ToLowerInvariant();
        return text;
    }

    private string NormalizeCompactToken(string raw)
    {
        return (raw ?? "")
            .Replace(" ", "")
            .Replace(".", "")
            .Replace(",", "")
            .Trim()
            .ToLowerInvariant();
    }

    private bool MatchesSearchValue(string source, string keywordRaw, string keywordNormalized, string keywordCompact)
    {
        if (string.IsNullOrEmpty(keywordRaw)) return true;

        string value = (source ?? "").Trim();
        if (string.IsNullOrEmpty(value)) return false;

        if (value.IndexOf(keywordRaw, StringComparison.OrdinalIgnoreCase) >= 0)
            return true;

        if (!string.IsNullOrEmpty(keywordCompact))
        {
            string valueCompact = NormalizeCompactToken(value);
            if (!string.IsNullOrEmpty(valueCompact) && valueCompact.Contains(keywordCompact))
                return true;
        }

        if (!string.IsNullOrEmpty(keywordNormalized))
        {
            string valueNormalized = NormalizeSearchText(value);
            if (!string.IsNullOrEmpty(valueNormalized) && valueNormalized.Contains(keywordNormalized))
                return true;
        }

        return false;
    }

    private bool IsAdminAccountManagementScope()
    {
        string scope = GetPageScopeForUrl();
        return string.Equals(scope, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase);
    }

    private bool CanManageAdminAccounts(dbDataContext db, string taiKhoanAdmin)
    {
        string tk = (taiKhoanAdmin ?? "").Trim();
        if (string.IsNullOrEmpty(tk)) return false;
        if (PermissionProfile_cl.IsRootAdmin(tk)) return true;
        return check_login_cl.CheckQuyen(db, tk, PermissionManageAdminAccounts);
    }

    private bool CanManageAdminAccounts()
    {
        string tk = GetCurrentAdminUser();
        if (string.IsNullOrEmpty(tk)) return false;
        if (PermissionProfile_cl.IsRootAdmin(tk)) return true;

        using (dbDataContext db = new dbDataContext())
            return CanManageAdminAccounts(db, tk);
    }

    private bool CanCreateAdminAccountInCurrentScope()
    {
        // Robust scope detection for create-admin action.
        // In some runtime flows, GetPageScopeForUrl() can be empty during redirect/open-view.
        // Accept if either scope or filter-scope indicates admin.
        string scope = NormalizeFilterScope(GetPageScopeForUrl());
        if (string.Equals(scope, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase))
            return true;

        string scopeFromQuery = NormalizeFilterScope(Request.QueryString["scope"]);
        if (string.Equals(scopeFromQuery, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase))
            return true;

        string scopeFromFilter = NormalizeFilterScope(Request.QueryString["fscope"]);
        if (string.Equals(scopeFromFilter, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase))
            return true;

        string rawUrl = (Request.RawUrl ?? "").ToLowerInvariant();
        return rawUrl.Contains("scope=admin") || rawUrl.Contains("fscope=admin");
    }

    protected bool IsStandaloneViewByQuery()
    {
        string view = NormalizeViewMode(Request.QueryString["view"]);
        string edit = (Request.QueryString["edit"] ?? "").Trim();
        return !string.IsNullOrEmpty(view) || !string.IsNullOrEmpty(edit);
    }

    private string GetPageScopeForUrl()
    {
        string scope = NormalizeFilterScope(Request.QueryString["scope"]);
        if (!string.IsNullOrEmpty(scope)) return scope;
        scope = NormalizeFilterScope(ViewState["filter_scope"] as string);
        return scope;
    }

    protected string GetAccountListScopeCssClass()
    {
        string scope = GetPageScopeForUrl();
        if (string.Equals(scope, "home", StringComparison.OrdinalIgnoreCase))
            return "scope-home";
        if (string.Equals(scope, "shop", StringComparison.OrdinalIgnoreCase))
            return "scope-shop";
        if (string.Equals(scope, "admin", StringComparison.OrdinalIgnoreCase))
            return "scope-admin";
        return "scope-all";
    }

    private string BuildListUrl(string overrideFilterType = null, string overrideFilterScope = null)
    {
        string scope = GetPageScopeForUrl();
        string filterType = NormalizeFilterAccountType(overrideFilterType ?? (ViewState["filter_phanloai"] as string));
        string filterScope = NormalizeFilterScope(overrideFilterScope ?? (ViewState["filter_scope"] as string));

        if (!string.IsNullOrEmpty(scope) && string.IsNullOrEmpty(filterScope))
            filterScope = scope;

        var query = new List<string>();
        if (!string.IsNullOrEmpty(scope))
            query.Add("scope=" + HttpUtility.UrlEncode(scope));
        if (!string.IsNullOrEmpty(filterType))
            query.Add("ftype=" + HttpUtility.UrlEncode(filterType));
        if (!string.IsNullOrEmpty(filterScope))
            query.Add("fscope=" + HttpUtility.UrlEncode(filterScope));

        return "/admin/quan-ly-tai-khoan/Default.aspx" + (query.Count > 0 ? "?" + string.Join("&", query) : "");
    }

    private string BuildViewUrl(string viewMode, string taiKhoan = "")
    {
        string mode = NormalizeViewMode(viewMode);
        if (string.IsNullOrEmpty(mode))
            return BuildListUrl();

        var query = new List<string>();
        string scope = GetPageScopeForUrl();
        string filterType = NormalizeFilterAccountType(ViewState["filter_phanloai"] as string);
        string filterScope = NormalizeFilterScope(ViewState["filter_scope"] as string);

        if (!string.IsNullOrEmpty(scope))
            query.Add("scope=" + HttpUtility.UrlEncode(scope));
        if (!string.IsNullOrEmpty(filterType))
            query.Add("ftype=" + HttpUtility.UrlEncode(filterType));
        if (!string.IsNullOrEmpty(filterScope))
            query.Add("fscope=" + HttpUtility.UrlEncode(filterScope));

        query.Add("view=" + HttpUtility.UrlEncode(mode));
        if (!string.IsNullOrWhiteSpace(taiKhoan))
        {
            string tk = taiKhoan.Trim();
            if (mode == ViewEdit)
                query.Add("edit=" + HttpUtility.UrlEncode(tk));
            else
                query.Add("tk=" + HttpUtility.UrlEncode(tk));
        }

        return "/admin/quan-ly-tai-khoan/Default.aspx?" + string.Join("&", query);
    }

    public string BuildAddUrl()
    {
        return BuildViewUrl(ViewAdd);
    }

    public string BuildFilterUrl()
    {
        return BuildViewUrl(ViewFilter);
    }

    public string BuildPermissionUrl(object taiKhoanObj)
    {
        string tk = (taiKhoanObj ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(tk)) return "#";
        return BuildViewUrl(ViewPermission, tk);
    }

    public string BuildEditUrl(object taiKhoanObj)
    {
        string tk = (taiKhoanObj ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(tk)) return "#";
        return BuildViewUrl(ViewEdit, tk);
    }

    public string BuildToggleHomeLockUrl(object taiKhoanObj)
    {
        string tk = (taiKhoanObj ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(tk))
            return "#";

        string url = BuildListUrl();
        string sep = url.Contains("?") ? "&" : "?";
        return url
            + sep + "toggle_home_lock=1"
            + "&tk=" + HttpUtility.UrlEncode(tk)
            + "&r=" + DateTime.UtcNow.Ticks.ToString();
    }

    private bool TryHandleToggleHomeLockRequest()
    {
        string toggle = (Request.QueryString["toggle_home_lock"] ?? "").Trim();
        if (toggle != "1")
            return false;

        if (!IsRootAdminCurrent())
        {
            Session["thongbao"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                "Chỉ admin gốc được phép khóa/mở khóa tài khoản Home.",
                "1800",
                "warning");
            Response.Redirect(BuildListUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return true;
        }

        string taiKhoan = (Request.QueryString["tk"] ?? "").Trim();
        if (string.IsNullOrEmpty(taiKhoan))
        {
            Session["thongbao"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                "Không xác định được tài khoản cần khóa/mở.",
                "1500",
                "warning");
            Response.Redirect(BuildListUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return true;
        }

        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == taiKhoan);
                if (acc == null)
                {
                    acc = db.taikhoan_tbs
                        .AsEnumerable()
                        .FirstOrDefault(p => string.Equals((p.taikhoan ?? "").Trim(), taiKhoan, StringComparison.OrdinalIgnoreCase));
                }

                if (acc == null)
                {
                    Session["thongbao"] = thongbao_class.metro_notifi_onload(
                        "Thông báo",
                        "Không tìm thấy tài khoản.",
                        "1500",
                        "warning");
                    Response.Redirect(BuildListUrl(), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return true;
                }

                if (!AccountVisibility_cl.CanToggleHomeLock(acc))
                {
                    Session["thongbao"] = thongbao_class.metro_notifi_onload(
                        "Thông báo",
                        "Chỉ áp dụng khóa/mở khóa với tài khoản Home.",
                        "1500",
                        "warning");
                    Response.Redirect(BuildListUrl(), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return true;
                }

                bool lockNow = acc.block != true;
                acc.block = lockNow;
                db.SubmitChanges();

                int visiblePostCount = db.BaiViet_tbs.Count(p =>
                    p.nguoitao == acc.taikhoan
                    && p.phanloai == "sanpham"
                    && p.bin == false);

                string msg = lockNow
                    ? ("Đã khóa Home tài khoản " + acc.taikhoan + ". Tin đang hiển thị bị ảnh hưởng: " + visiblePostCount.ToString("#,##0") + ".")
                    : ("Đã mở khóa Home tài khoản " + acc.taikhoan + ".");

                Session["thongbao"] = thongbao_class.metro_notifi_onload(
                    "Thông báo",
                    msg,
                    "1800",
                    lockNow ? "warning" : "success");
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);

            Session["thongbao"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                "Có lỗi khi cập nhật trạng thái khóa Home.",
                "1800",
                "alert");
        }

        Response.Redirect(BuildListUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
        return true;
    }

    private void ApplyOpenViewFromQuery()
    {
        string view = NormalizeViewMode(Request.QueryString["view"]);
        string openEdit = (Request.QueryString["edit"] ?? "").Trim();
        string targetTk = (Request.QueryString["tk"] ?? "").Trim();

        if (string.IsNullOrEmpty(view) && !string.IsNullOrEmpty(openEdit))
            view = ViewEdit;

        if (string.IsNullOrEmpty(view))
            return;

        up_main.Visible = false;

        if (view == ViewAdd)
        {
            ShowAddForm();
            return;
        }

        if (view == ViewFilter)
        {
            ShowFilterForm();
            return;
        }

        if (view == ViewEdit)
        {
            string tkEdit = string.IsNullOrEmpty(openEdit) ? targetTk : openEdit;
            if (string.IsNullOrEmpty(tkEdit))
            {
                up_main.Visible = true;
                return;
            }
            OpenEditFormByAccount(tkEdit);
            return;
        }

        if (view == ViewPermission)
        {
            if (string.IsNullOrEmpty(targetTk))
            {
                up_main.Visible = true;
                return;
            }
            OpenPermissionFormByAccount(targetTk);
            return;
        }

    }

    private string GetCurrentAdminUser()
    {
        return (ViewState["taikhoan"] ?? "").ToString().Trim();
    }

    private bool IsHomeScopeAccount(taikhoan_tb acc)
    {
        if (acc == null) return false;
        string scope = PortalScope_cl.ResolveScope(acc.taikhoan, acc.phanloai, acc.permission);
        return string.Equals(scope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase);
    }

    private taikhoan_tb FindAccountByUsername(dbDataContext db, string taiKhoan)
    {
        if (db == null) return null;
        string tk = (taiKhoan ?? "").Trim();
        if (tk == "") return null;

        taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
        if (acc != null) return acc;

        return db.taikhoan_tbs
            .AsEnumerable()
            .FirstOrDefault(p => string.Equals((p.taikhoan ?? "").Trim(), tk, StringComparison.OrdinalIgnoreCase));
    }

    private int GetCurrentTierFromAccountForEdit(dbDataContext db, taikhoan_tb acc)
    {
        if (acc == null) return TierHome_cl.Tier1;

        int tierFromPhanLoai = TierHome_cl.GetTierFromPhanLoai(acc.phanloai);
        if (tierFromPhanLoai > TierHome_cl.Tier0) return tierFromPhanLoai;

        int tierFromAccount = TierHome_cl.GetTierFromHanhVi(TierHome_cl.GetCurrentHanhViFromAccount(acc));
        if (tierFromAccount > TierHome_cl.Tier0) return tierFromAccount;

        return TierHome_cl.Tier1;
    }

    private bool CanAdjustTierTarget(dbDataContext db, string tkAdmin, int targetTier)
    {
        if (db == null || string.IsNullOrWhiteSpace(tkAdmin)) return false;
        if (PermissionProfile_cl.IsRootAdmin(tkAdmin)) return true;

        if (targetTier >= TierHome_cl.Tier3)
            return PermissionProfile_cl.HasPermission(db, tkAdmin, PermissionProfile_cl.HoSoGanKet);

        if (targetTier >= TierHome_cl.Tier2)
            return PermissionProfile_cl.HasPermission(db, tkAdmin, PermissionProfile_cl.HoSoLaoDong);

        return PermissionProfile_cl.HasPermission(db, tkAdmin, PermissionProfile_cl.HoSoUuDai);
    }

    private bool CanAdjustAnyHomeTier(dbDataContext db, string tkAdmin)
    {
        if (db == null || string.IsNullOrWhiteSpace(tkAdmin)) return false;
        if (PermissionProfile_cl.IsRootAdmin(tkAdmin)) return true;

        return PermissionProfile_cl.HasAnyPermission(
            db,
            tkAdmin,
            PermissionProfile_cl.HoSoUuDai,
            PermissionProfile_cl.HoSoLaoDong,
            PermissionProfile_cl.HoSoGanKet);
    }

    private void ResetHomeHanhVi(taikhoan_tb acc)
    {
        if (acc == null) return;

        acc.HeThongSanPham_Cap123 = 0;
        acc.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 = null;
        acc.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 = null;
        acc.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 = null;
    }

    private void CancelPendingHanhViRequests(dbDataContext db, string taiKhoan, string nguoiDuyet, string ghiChu)
    {
        if (db == null || string.IsNullOrWhiteSpace(taiKhoan)) return;

        var pending = db.YeuCau_HeThongSanPham_tbs
            .Where(x => x.taikhoan == taiKhoan && x.TrangThai == 0)
            .ToList();

        DateTime now = AhaTime_cl.Now;
        string note = (ghiChu ?? "").Trim();
        if (string.IsNullOrEmpty(note))
            note = "Yêu cầu bị huỷ do admin thay đổi tầng hồ sơ.";

        foreach (var yc in pending)
        {
            yc.TrangThai = 2;
            yc.NgayDuyet = now;
            yc.NguoiDuyet = nguoiDuyet;
            yc.GhiChuAdmin = note;
        }
    }

    #region main - phân trang - tìm kiếm
    public void show_main()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                var list_all = (from ob1 in db.taikhoan_tbs
                                select new
                                {
                                    ob1.id,
                                    ob1.DongA,

                                    // ✅ NEW: 3 ví mới
                                    //ob1.Vi3_20PhanTram_ViGanKet,
                                    //ob1.Vi1_30PhanTram_ViEVoucher,
                                    //ob1.Vi2_50PhanTram_ViLaoDong,

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
                                    ob1.permission,
                                    ob1.block,
                                    ob1.ChiPhanTram_BanDichVu_ChoSan,
                                    ob1.HeThongSanPham_Cap123,

                                    ob1.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6,
                                    ob1.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10,
                                    ob1.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4,
                                }).AsQueryable();

                string _filter_phanloai = NormalizeFilterAccountType(ViewState["filter_phanloai"] as string);
                if (!string.IsNullOrEmpty(_filter_phanloai))
                {
                    if (AccountType_cl.IsTreasury(_filter_phanloai))
                    {
                        list_all = list_all.Where(p =>
                            p.phanloai != null
                            && (p.phanloai.StartsWith(AccountType_cl.Treasury)
                                || p.phanloai.StartsWith(AccountType_cl.LegacyTreasury)));
                    }
                    else
                    {
                        list_all = list_all.Where(p => p.phanloai == _filter_phanloai);
                    }
                }

                string _key = GetSearchKeyword();
                string _keyNormalized = NormalizeSearchText(_key);
                string _keyCompact = NormalizeCompactToken(_key);
                SyncSearchInputs(_key);

                var list_filtered = list_all.ToList();
                string _filter_scope = NormalizeFilterScope(ViewState["filter_scope"] as string);
                if (!string.IsNullOrEmpty(_filter_scope))
                {
                    list_filtered = list_filtered.Where(p =>
                    {
                        string resolvedScope = PortalScope_cl.ResolveScope(p.taikhoan, p.phanloai, p.permission);
                        if (_filter_scope == "admin")
                            return string.Equals(resolvedScope, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase);
                        if (_filter_scope == "shop")
                            return string.Equals(resolvedScope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase);
                        return string.Equals(resolvedScope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase);
                    }).ToList();
                }

                if (!string.IsNullOrEmpty(_key))
                {
                    list_filtered = list_filtered.Where(p =>
                    {
                        int cap = p.HeThongSanPham_Cap123 ?? 0;
                        int giaTriHanhVi = 0;
                        if (cap == 1) giaTriHanhVi = p.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 ?? 0;
                        else if (cap == 2) giaTriHanhVi = p.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 ?? 0;
                        else if (cap == 3) giaTriHanhVi = p.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 ?? 0;

                        int? loaiHanhVi = HanhVi9Cap_cl.GetLoaiHanhViByCapGiaTri(cap, giaTriHanhVi);
                        string hanhViHienThi = loaiHanhVi.HasValue
                            ? HanhVi9Cap_cl.GetTenHanhViTheoLoai(loaiHanhVi)
                            : "-";

                        string scope = PortalScope_cl.ResolveScope(p.taikhoan, p.phanloai, p.permission);
                        string ngaySinh = p.ngaysinh.HasValue ? p.ngaysinh.Value.ToString("dd/MM/yyyy") : "";
                        string ngayTao = p.ngaytao.HasValue ? p.ngaytao.Value.ToString("dd/MM/yyyy HH:mm") : "";
                        string dongA = (p.DongA ?? 0).ToString(CultureInfo.InvariantCulture);
                        string dongAFormatted = (p.DongA ?? 0).ToString("#,##0.##");
                        string chiPhanTram = (p.ChiPhanTram_BanDichVu_ChoSan ?? 0).ToString(CultureInfo.InvariantCulture);
                        string tierName = TierHome_cl.GetTenTangHome(cap);
                        string trangThaiKhoa = (p.block == true) ? "khoa" : "mo";

                        return MatchesSearchValue(p.id.ToString(), _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(p.taikhoan, _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(p.hoten, _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(p.hoten_khongdau, _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(p.dienthoai, _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(p.email, _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(p.phanloai, _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(scope, _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(tierName, _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(cap.ToString(), _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(giaTriHanhVi.ToString(), _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(hanhViHienThi, _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(ngaySinh, _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(ngayTao, _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(dongA, _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(dongAFormatted, _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(chiPhanTram, _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(trangThaiKhoa, _key, _keyNormalized, _keyCompact)
                               || MatchesSearchValue(p.permission, _key, _keyNormalized, _keyCompact);
                    }).ToList();
                }

                list_filtered = list_filtered.OrderByDescending(p => p.ngaytao).ToList();
                int _Tong_Record = list_filtered.Count();

                int show = 30;
                int current_page = int.Parse(ViewState["current_page_qlnv"].ToString());
                int total_page = number_of_page_class.return_total_page(_Tong_Record, show);
                if (current_page < 1) current_page = 1; else if (current_page > total_page) current_page = total_page;
                ViewState["total_page"] = total_page;

                but_xemtiep.Enabled = current_page < total_page;
                but_xemtiep1.Enabled = current_page < total_page;
                but_quaylai.Enabled = current_page > 1;
                but_quaylai1.Enabled = current_page > 1;

                var list_split = list_filtered.Skip(current_page * show - show).Take(show).ToList();
                bool canManageAdminAccountsCurrent = CanManageAdminAccounts(db, GetCurrentAdminUser());

                var list_show = list_split.Select(x =>
                {
                    int capHienTai = x.HeThongSanPham_Cap123 ?? 0;
                    int giaTriHanhVi = 0;
                    if (capHienTai == 1) giaTriHanhVi = x.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 ?? 0;
                    else if (capHienTai == 2) giaTriHanhVi = x.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 ?? 0;
                    else if (capHienTai == 3) giaTriHanhVi = x.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 ?? 0;

                    int? loaiHanhVi = HanhVi9Cap_cl.GetLoaiHanhViByCapGiaTri(capHienTai, giaTriHanhVi);
                    string hanhViHienThi = loaiHanhVi.HasValue
                        ? HanhVi9Cap_cl.GetTenHanhViTheoLoai(loaiHanhVi)
                        : "-";

                    string scope = PortalScope_cl.ResolveScope(x.taikhoan, x.phanloai, x.permission);
                    bool isAdminScope = string.Equals(scope, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase);
                    bool isHomeScope = string.Equals(scope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase);
                    bool canShowPhanQuyen = canManageAdminAccountsCurrent && isAdminScope && !PermissionProfile_cl.IsRootAdmin(x.taikhoan);
                    bool isBlocked = x.block == true;
                    bool canToggleHomeLock = IsRootAdminCurrent()
                        && isHomeScope
                        && !PermissionProfile_cl.IsRootAdmin(x.taikhoan);

                    return new
                    {
                        x.id,
                        x.DongA,

                        x.taikhoan,
                        x.matkhau,
                        x.anhdaidien,
                        x.hoten,
                        x.hoten_khongdau,
                        x.ngaysinh,
                        x.email,
                        x.dienthoai,
                        x.ngaytao,
                        x.phanloai,
                        x.block,
                        x.ChiPhanTram_BanDichVu_ChoSan,

                        HanhVi_HienThi = hanhViHienThi,
                        IsAdminScope = isAdminScope,
                        IsHomeScope = isHomeScope,
                        IsBlocked = isBlocked,
                        CanToggleHomeLock = canToggleHomeLock,
                        CanShowPhanQuyen = canShowPhanQuyen,
                    };
                }).ToList();

                int stt = (show * current_page) - show + 1;
                int _s1 = stt + list_split.Count() - 1;
                if (_Tong_Record != 0) lb_show.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");
                else lb_show.Text = "0-0/0";
                lb_show_md.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");

                Repeater1.DataSource = list_show;
                Repeater1.DataBind();
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["current_page_qlnv"] = int.Parse(ViewState["current_page_qlnv"].ToString()) - 1;

            HttpCookie cookie = Request.Cookies["cookie_qlnv"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qlnv"].ToString();
                cookie.Expires = AhaTime_cl.Now.AddDays(1);
                Response.Cookies.Set(cookie);
            }

            show_main();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            ViewState["current_page_qlnv"] = int.Parse(ViewState["current_page_qlnv"].ToString()) + 1;

            HttpCookie cookie = Request.Cookies["cookie_qlnv"];
            if (cookie != null)
            {
                cookie["trang_hientai"] = ViewState["current_page_qlnv"].ToString();
                cookie.Expires = AhaTime_cl.Now.AddDays(1);
                Response.Cookies.Set(cookie);
            }

            show_main();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            SyncSearchInputs(GetSearchKeyword());
            ViewState["current_page_qlnv"] = 1;
            show_main();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_timkiem_Click(object sender, EventArgs e)
    {
        txt_timkiem_TextChanged(sender, e);
    }

    protected void but_xoa_timkiem_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            SyncSearchInputs("");
            ViewState["current_page_qlnv"] = 1;
            show_main();
            up_main.Update();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_toggle_home_lock_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");

            if (!IsRootAdminCurrent())
            {
                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.GetType(),
                    Guid.NewGuid().ToString(),
                    thongbao_class.metro_notifi("Thông báo", "Chỉ admin gốc được phép khóa/mở khóa tài khoản Home.", "1800", "warning"),
                    true);
                show_main();
                up_main.Update();
                return;
            }

            LinkButton button = sender as LinkButton;
            string taiKhoan = button == null ? "" : (button.CommandArgument ?? "").Trim();
            if (string.IsNullOrEmpty(taiKhoan))
            {
                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.GetType(),
                    Guid.NewGuid().ToString(),
                    thongbao_class.metro_notifi("Thông báo", "Không xác định được tài khoản cần khóa/mở.", "1500", "warning"),
                    true);
                show_main();
                up_main.Update();
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == taiKhoan);
                if (acc == null)
                {
                    acc = db.taikhoan_tbs
                        .AsEnumerable()
                        .FirstOrDefault(p => string.Equals((p.taikhoan ?? "").Trim(), taiKhoan, StringComparison.OrdinalIgnoreCase));
                }
                if (acc == null)
                {
                    ScriptManager.RegisterStartupScript(
                        this.Page,
                        this.GetType(),
                        Guid.NewGuid().ToString(),
                        thongbao_class.metro_notifi("Thông báo", "Không tìm thấy tài khoản.", "1500", "warning"),
                        true);
                    show_main();
                    up_main.Update();
                    return;
                }

                if (!AccountVisibility_cl.CanToggleHomeLock(acc))
                {
                    ScriptManager.RegisterStartupScript(
                        this.Page,
                        this.GetType(),
                        Guid.NewGuid().ToString(),
                        thongbao_class.metro_notifi("Thông báo", "Chỉ áp dụng khóa/mở khóa với tài khoản Home.", "1500", "warning"),
                        true);
                    show_main();
                    up_main.Update();
                    return;
                }

                bool lockNow = acc.block != true;
                acc.block = lockNow;
                db.SubmitChanges();

                int visiblePostCount = db.BaiViet_tbs.Count(p =>
                    p.nguoitao == acc.taikhoan
                    && p.phanloai == "sanpham"
                    && p.bin == false);

                string msg = lockNow
                    ? ("Đã khóa Home tài khoản " + acc.taikhoan + ". Tin đang hiển thị bị ảnh hưởng: " + visiblePostCount.ToString("#,##0") + ".")
                    : ("Đã mở khóa Home tài khoản " + acc.taikhoan + ".");

                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.GetType(),
                    Guid.NewGuid().ToString(),
                    thongbao_class.metro_notifi("Thông báo", msg, "1800", lockNow ? "warning" : "success"),
                    true);
            }

            show_main();
            up_main.Update();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_lock_legacy_home_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");

            if (!IsRootAdminCurrent())
            {
                Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Chỉ admin gốc được phép khóa hàng loạt tài khoản Home.", "1800", "warning");
                Response.Redirect(BuildListUrl(), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                int hiddenPostCount;
                int lockedCount = AccountVisibility_cl.LockLegacyHomeAccountsWithoutPhone(db, out hiddenPostCount);

                string msg;
                if (lockedCount <= 0)
                    msg = "Không có tài khoản Home nào cần khóa hàng loạt.";
                else
                    msg = "Đã khóa " + lockedCount.ToString("#,##0") + " tài khoản Home không đủ điều kiện đăng nhập bằng số điện thoại. Tin bị ảnh hưởng: " + hiddenPostCount.ToString("#,##0") + ".";

                Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", msg, "2400", lockedCount <= 0 ? "warning" : "success");
                Response.Redirect(BuildListUrl(), false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    #endregion

    #region FILTER (NEW)
    protected void but_show_filter_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            Response.Redirect(BuildFilterUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_close_form_filter_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildListUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void but_apdung_loc_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");

            ViewState["filter_phanloai"] = NormalizeFilterAccountType(ddl_loc_phanloai.SelectedValue);
            ViewState["filter_scope"] = NormalizeFilterScope(ddl_loc_scope.SelectedValue);
            ViewState["current_page_qlnv"] = 1;

            Response.Redirect(BuildListUrl(ViewState["filter_phanloai"] as string, ViewState["filter_scope"] as string), false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_xoa_loc_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");

            ViewState["filter_phanloai"] = "";
            ViewState["filter_scope"] = "";
            ddl_loc_phanloai.SelectedValue = "";
            ddl_loc_scope.SelectedValue = "";

            ViewState["current_page_qlnv"] = 1;

            Response.Redirect(BuildListUrl("", ""), false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    #endregion
    #region ref (Home-only)
    public void bind_dropdown_nguoi_gioi_thieu(string selectedValue = "")
    {
        ddl_nguoi_gioi_thieu.Items.Clear();
        ddl_nguoi_gioi_thieu.Items.Add(new ListItem("Không áp dụng ở Admin/Shop", ""));
        ddl_nguoi_gioi_thieu.SelectedValue = "";
    }
    #endregion

    #region ADD - EDIT
    public void reset_control_add_edit()
    {
        txt_taikhoan.Text = "";
        txt_matkhau.Text = "";
        txt_link_fileupload.Text = "";
        txt_hoten.Text = "";
        txt_ngaysinh.Text = "";
        txt_dienthoai.Text = "";
        txt_email.Text = "";

        ddl_cap_sp.SelectedValue = "1";
        ddl_cap_sp.Enabled = false;
        pn_tang_home.Visible = false;

        txt_reset_home_password.Text = "";
        txt_reset_home_pin.Text = "";
        txt_reset_shop_password.Text = "";
        lb_reset_scope_note.Text = "";
        pn_reset_security_actions.Visible = false;
        pn_reset_home_credentials.Visible = false;
        pn_reset_shop_credentials.Visible = false;





        bind_dropdown_nguoi_gioi_thieu("");

        ViewState["add_edit"] = null;
        ViewState["id_edit"] = null;

        Button2.Visible = false;
        Label2.Text = "";
        txt_taikhoan.ReadOnly = false;

        BindAccountTypeDropdownForAdminCreate();
        pn_loai_taikhoan.Visible = false;
        DropDownList1.Enabled = false;
        ddl_nguoi_gioi_thieu.Enabled = false;
    }

    private void BindAccountTypeDropdownForAdminCreate()
    {
        DropDownList1.Items.Clear();
        DropDownList1.Items.Add(new ListItem(AccountTypeAdminStaff, AccountTypeAdminStaff));
        DropDownList1.Items.Add(new ListItem(AccountType_cl.Treasury, AccountType_cl.Treasury));
        DropDownList1.SelectedValue = AccountTypeAdminStaff;
    }

    private void BindAccountTypeDropdownForScopedReadonly(taikhoan_tb acc, string resolvedScope)
    {
        string scope = (resolvedScope ?? "").Trim().ToLowerInvariant();
        string displayType = "";

        if (scope == PortalScope_cl.ScopeHome)
        {
            displayType = (acc != null ? (acc.phanloai ?? "").Trim() : "");
            if (string.IsNullOrWhiteSpace(displayType) || string.Equals(displayType, AccountTypeAdminStaff, StringComparison.OrdinalIgnoreCase) || AccountType_cl.IsTreasury(displayType))
                displayType = AccountTypeHomeDefault;
        }
        else if (scope == PortalScope_cl.ScopeShop)
        {
            displayType = AccountTypeShopDefault;
        }
        else
        {
            string normalized = AccountType_cl.Normalize(acc != null ? acc.phanloai : "");
            if (string.IsNullOrWhiteSpace(normalized))
                normalized = AccountTypeAdminStaff;

            bool validAdminType = string.Equals(normalized, AccountTypeAdminStaff, StringComparison.OrdinalIgnoreCase)
                || AccountType_cl.IsTreasury(normalized);
            displayType = validAdminType ? normalized : AccountTypeAdminStaff;
        }

        DropDownList1.Items.Clear();
        DropDownList1.Items.Add(new ListItem(displayType, displayType));
        DropDownList1.SelectedIndex = 0;
    }

    private void ShowFilterForm()
    {
        string _filter = NormalizeFilterAccountType(ViewState["filter_phanloai"] as string);
        if (_filter == null) _filter = "";
        var liFilter = ddl_loc_phanloai.Items.FindByValue(_filter);
        ddl_loc_phanloai.SelectedValue = liFilter != null ? _filter : "";

        string scopeFilter = NormalizeFilterScope(ViewState["filter_scope"] as string);
        var liScope = ddl_loc_scope.Items.FindByValue(scopeFilter);
        ddl_loc_scope.SelectedValue = liScope != null ? scopeFilter : "";

        pn_filter.Visible = true;
        up_filter.Update();
    }

    private void ShowAddForm()
    {
        if (!CanCreateAdminAccountInCurrentScope())
        {
            Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Chỉ thao tác trong tab quản lý tài khoản admin mới được tạo tài khoản admin.", "1000", "warning");
            Response.Redirect(BuildListUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        reset_control_add_edit();
        ddl_cap_sp.SelectedValue = "1";
        ddl_cap_sp.Enabled = false;
        pn_tang_home.Visible = false;

        PlaceHolder1.Visible = true;
        ViewState["add_edit"] = "add";
        Label1.Text = "THÊM TÀI KHOẢN ADMIN";
        but_add_edit.Text = "THÊM MỚI";

        bind_dropdown_nguoi_gioi_thieu("");
        BindAccountTypeDropdownForAdminCreate();
        pn_loai_taikhoan.Visible = true;
        DropDownList1.Enabled = true;
        ddl_nguoi_gioi_thieu.Enabled = false;

        pn_add.Visible = true;
        up_add.Update();
    }

    protected void but_show_form_add_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            if (!CanCreateAdminAccountInCurrentScope())
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "Chỉ thao tác trong tab quản lý tài khoản admin mới được tạo tài khoản admin.", "false", "false", "OK", "alert", ""), true);
                return;
            }
            Response.Redirect(BuildAddUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_close_form_add_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            Response.Redirect(BuildListUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_show_form_edit_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            LinkButton button = (LinkButton)sender;
            string _tk_edit = (button.CommandArgument ?? "").Trim();
            if (string.IsNullOrEmpty(_tk_edit))
                _tk_edit = (hf_selected_taikhoan.Value ?? "").Trim();
            if (string.IsNullOrEmpty(_tk_edit))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "Không xác định được tài khoản cần xem chi tiết. Vui lòng thử lại.", "false", "false", "OK", "alert", ""), true);
                return;
            }
            OpenEditFormByAccount(_tk_edit);
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    private void OpenEditFormByAccount(string taiKhoanCanSua)
    {
        string _tk_edit = (taiKhoanCanSua ?? "").Trim();
        if (string.IsNullOrEmpty(_tk_edit)) return;

        reset_control_add_edit();
        ViewState["add_edit"] = "edit";

        PlaceHolder1.Visible = false;
        Label1.Text = "CHỈNH SỬA TÀI KHOẢN";
        but_add_edit.Text = "CẬP NHẬT";

        using (dbDataContext db = new dbDataContext())
        {
            var q = FindAccountByUsername(db, _tk_edit);
            if (q == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "Không tìm thấy tài khoản cần xem chi tiết.", "false", "false", "OK", "alert", ""), true);
                return;
            }

            ViewState["id_edit"] = q.taikhoan;

            string tkAdmin = GetCurrentAdminUser();
            string resolvedScope = PortalScope_cl.ResolveScope(q.taikhoan, q.phanloai, q.permission);
            bool isHomeAccount = string.Equals(resolvedScope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase);
            bool isShopAccount = string.Equals(resolvedScope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase);
            int currentTier = GetCurrentTierFromAccountForEdit(db, q);
            if (currentTier < TierHome_cl.Tier1) currentTier = TierHome_cl.Tier1;
            if (currentTier > TierHome_cl.Tier3) currentTier = TierHome_cl.Tier3;

            string tierValue = currentTier.ToString();
            var liCap = ddl_cap_sp.Items.FindByValue(tierValue);
            ddl_cap_sp.SelectedValue = (liCap != null) ? tierValue : "1";
            ddl_cap_sp.Enabled = isHomeAccount && CanAdjustAnyHomeTier(db, tkAdmin);
            pn_tang_home.Visible = isHomeAccount;

            txt_taikhoan.Text = q.taikhoan;
            txt_taikhoan.ReadOnly = true;
            txt_hoten.Text = q.hoten;
            txt_dienthoai.Text = q.dienthoai;
            txt_email.Text = q.email;
            txt_ngaysinh.Text = q.ngaysinh != null ? q.ngaysinh.Value.ToString("dd/MM/yyyy") : "";

            BindAccountTypeDropdownForScopedReadonly(q, resolvedScope);
            // Loại tài khoản không còn là trường chỉnh sửa ở màn hình chi tiết.
            // Chỉ giữ khi tạo mới tài khoản admin.
            pn_loai_taikhoan.Visible = false;

            string anh = q.anhdaidien;
            if (string.IsNullOrEmpty(anh)) anh = "/uploads/images/macdinh.jpg";
            Label2.Text = "<div class='mt-2'><small>Ảnh hiện tại</small></div><img width='100' src='" + anh + "' />";
            Button2.Visible = (anh != "/uploads/images/macdinh.jpg");

            bind_dropdown_nguoi_gioi_thieu("");
            DropDownList1.Enabled = false;
            ddl_nguoi_gioi_thieu.Enabled = false;

            pn_reset_security_actions.Visible = isHomeAccount || isShopAccount;
            pn_reset_home_credentials.Visible = isHomeAccount;
            pn_reset_shop_credentials.Visible = isShopAccount;
            lb_reset_scope_note.Text = isHomeAccount
                ? "Phạm vi hiện tại: Home. Có thể reset mật khẩu/PIN tạm thời."
                : (isShopAccount ? "Phạm vi hiện tại: Shop. Có thể reset mật khẩu tạm thời." : "");

            string schemaError;
            if (pn_reset_security_actions.Visible && !AccountResetSecurity_cl.EnsureSchemaSafe(db, out schemaError))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_notifi("Thông báo", "Không thể khởi tạo schema reset bảo mật: " + schemaError, "1800", "warning"), true);
            }
        }

        pn_add.Visible = true;
        up_add.Update();
    }

    private bool TryGetEditingAccountWithScope(
        dbDataContext db,
        out taikhoan_tb account,
        out string scope,
        out string taiKhoan,
        bool requiredHome,
        bool requiredShop,
        out string error)
    {
        account = null;
        scope = "";
        taiKhoan = "";
        error = "";

        if (ViewState["id_edit"] == null)
        {
            error = "Không xác định được tài khoản cần thao tác. Vui lòng mở lại trang chi tiết.";
            return false;
        }

        taiKhoan = (ViewState["id_edit"] ?? "").ToString().Trim();
        if (taiKhoan == "")
        {
            error = "Không xác định được tài khoản cần thao tác.";
            return false;
        }

        account = FindAccountByUsername(db, taiKhoan);
        if (account == null)
        {
            error = "Không tìm thấy tài khoản cần thao tác.";
            return false;
        }

        scope = PortalScope_cl.ResolveScope(account.taikhoan, account.phanloai, account.permission);
        bool isHome = string.Equals(scope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase);
        bool isShop = string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase);

        if (requiredHome && !isHome)
        {
            error = "Chức năng này chỉ áp dụng cho tài khoản Home.";
            return false;
        }

        if (requiredShop && !isShop)
        {
            error = "Chức năng này chỉ áp dụng cho tài khoản Shop.";
            return false;
        }

        return true;
    }

    private void NotifyResetResultAndReload(string taiKhoan, string message, string style)
    {
        Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", message, "1800", style);
        Response.Redirect(BuildViewUrl(ViewEdit, taiKhoan), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void but_reset_home_password_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            string passTam = (txt_reset_home_password.Text ?? "").Trim();
            if (string.IsNullOrEmpty(passTam))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mật khẩu tạm thời cho Home.", "false", "false", "OK", "alert", ""), true);
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                taikhoan_tb account;
                string scope;
                string tk;
                string error;
                if (!TryGetEditingAccountWithScope(db, out account, out scope, out tk, true, false, out error))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                        thongbao_class.metro_dialog("Thông báo", error, "false", "false", "OK", "alert", ""), true);
                    return;
                }

                string actor = GetCurrentAdminUser();
                string resetError;
                if (!AccountResetSecurity_cl.ResetHomePassword(db, tk, passTam, actor, out resetError))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                        thongbao_class.metro_dialog("Thông báo", "Reset mật khẩu Home thất bại: " + resetError, "false", "false", "OK", "alert", ""), true);
                    return;
                }

                Helper_DongA_cl.AddNotify(
                    db,
                    actor,
                    tk,
                    "Admin đã cấp mật khẩu tạm thời mới. Vui lòng đăng nhập bằng mật khẩu tạm thời và đổi lại ngay.",
                    "/home/DoiMatKhau.aspx?force=1");
                db.SubmitChanges();

                txt_reset_home_password.Text = "";
                NotifyResetResultAndReload(tk, "Đã reset mật khẩu Home tạm thời cho tài khoản " + tk + ".", "success");
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_reset_home_pin_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            string pinTam = (txt_reset_home_pin.Text ?? "").Trim();
            if (!PinSecurity_cl.IsValidPinFormat(pinTam))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "PIN tạm thời phải gồm đúng 4 chữ số.", "false", "false", "OK", "alert", ""), true);
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                taikhoan_tb account;
                string scope;
                string tk;
                string error;
                if (!TryGetEditingAccountWithScope(db, out account, out scope, out tk, true, false, out error))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                        thongbao_class.metro_dialog("Thông báo", error, "false", "false", "OK", "alert", ""), true);
                    return;
                }

                string actor = GetCurrentAdminUser();
                string resetError;
                string pinHash = PinSecurity_cl.HashPin(pinTam);
                if (!AccountResetSecurity_cl.ResetHomePin(db, tk, pinHash, actor, out resetError))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                        thongbao_class.metro_dialog("Thông báo", "Reset PIN Home thất bại: " + resetError, "false", "false", "OK", "alert", ""), true);
                    return;
                }

                Helper_DongA_cl.AddNotify(
                    db,
                    actor,
                    tk,
                    "Admin đã cấp PIN tạm thời mới. Vui lòng vào mục Đổi PIN để cập nhật lại ngay.",
                    "/home/DoiPin.aspx?force=1");
                db.SubmitChanges();

                txt_reset_home_pin.Text = "";
                NotifyResetResultAndReload(tk, "Đã reset PIN Home tạm thời cho tài khoản " + tk + ".", "success");
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    protected void but_reset_shop_password_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            string passTam = (txt_reset_shop_password.Text ?? "").Trim();
            if (string.IsNullOrEmpty(passTam))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mật khẩu tạm thời cho Shop.", "false", "false", "OK", "alert", ""), true);
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                taikhoan_tb account;
                string scope;
                string tk;
                string error;
                if (!TryGetEditingAccountWithScope(db, out account, out scope, out tk, false, true, out error))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                        thongbao_class.metro_dialog("Thông báo", error, "false", "false", "OK", "alert", ""), true);
                    return;
                }

                string actor = GetCurrentAdminUser();
                string resetError;
                if (!AccountResetSecurity_cl.ResetShopPassword(db, tk, passTam, actor, out resetError))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                        thongbao_class.metro_dialog("Thông báo", "Reset mật khẩu Shop thất bại: " + resetError, "false", "false", "OK", "alert", ""), true);
                    return;
                }

                Helper_DongA_cl.AddNotify(
                    db,
                    actor,
                    tk,
                    "Admin đã cấp mật khẩu gian hàng đối tác tạm thời mới. Vui lòng đăng nhập và đổi lại ngay.",
                    "/shop/doi-mat-khau?force=1");
                db.SubmitChanges();

                txt_reset_shop_password.Text = "";
                NotifyResetResultAndReload(tk, "Đã reset mật khẩu Shop tạm thời cho tài khoản " + tk + ".", "success");
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
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
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_notifi("Thông báo", "Xóa ảnh thành công.", "1000", "warning"), true);
            }
        }
    }

 

    protected void but_add_edit_Click(object sender, EventArgs e)
    {
        try
        {
            if (!Directory.Exists(Server.MapPath("~/uploads/img-handler/")))
                Directory.CreateDirectory(Server.MapPath("~/uploads/img-handler/"));

            string _user = txt_taikhoan.Text.Trim().ToLower();
            string _pass = txt_matkhau.Text.Trim();
            string _anhdaidien_new = txt_link_fileupload.Text;
            string _fullname = txt_hoten.Text.Trim();
            if (!string.IsNullOrEmpty(_fullname))
                _fullname = str_cl.VietHoa_ChuCai_DauTien(str_cl.Remove_Blank(_fullname.ToLower()));
            string _ngaysinh = txt_ngaysinh.Text;
            string _sdt = txt_dienthoai.Text.Trim().Replace(" ", "");
            string _email = txt_email.Text.Trim();
            string _loaitaikhoan = AccountType_cl.Normalize(DropDownList1.SelectedValue);



            using (dbDataContext db = new dbDataContext())
            {
                bool isAddMode = ViewState["add_edit"] != null && ViewState["add_edit"].ToString() == "add";
                if (isAddMode && !CanCreateAdminAccountInCurrentScope())
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                        thongbao_class.metro_dialog("Thông báo", "Chỉ thao tác trong tab quản lý tài khoản admin mới được tạo tài khoản admin.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                if (_user == "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                        thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tài khoản.", "false", "false", "OK", "alert", ""), true);
                    return;
                }
                if (str_cl.check_taikhoan_hople(_user) == false)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                        thongbao_class.metro_dialog("Thông báo", "Tài khoản phải có độ dài từ 5-30 ký tự không dấu hoặc chữ số và không chứa dấu cách. Vui lòng kiểm tra lại.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                // ===== ADD =====
                if (isAddMode)
                {
                    bool validAdminType = string.Equals(_loaitaikhoan, AccountTypeAdminStaff, StringComparison.OrdinalIgnoreCase)
                        || AccountType_cl.IsTreasury(_loaitaikhoan);
                    if (!validAdminType)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                            thongbao_class.metro_dialog("Thông báo", "Trang admin chỉ cho phép tạo tài khoản nhân viên admin hoặc tài khoản tổng.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }

                    if (_pass == "")
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                            thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mật khẩu.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }

                    var q_tk = db.taikhoan_tbs
                        .Where(p => p.taikhoan == _user || (!string.IsNullOrEmpty(_email) && p.email.ToLower() == _email.ToLower()))
                        .Select(p => new { p.taikhoan, p.email })
                        .FirstOrDefault();

                    if (q_tk != null)
                    {
                        string message = (q_tk.taikhoan == _user)
                            ? "Tài khoản đã tồn tại. Vui lòng chọn tên khác."
                            : "Email này đã được dùng cho một tài khoản khác.";

                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                            thongbao_class.metro_dialog("Thông báo", message, "false", "false", "OK", "alert", ""), true);
                        return;
                    }

                    string _link_qr = string.Format("https://ahasale.vn/{0}.info", _user);
                    List<string> dataList = new List<string>();
                    dataList.Add(_link_qr);
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
                        Bitmap qrCodeBitmap = qrCodeWriter.Write(data);

                        string fileName = _user + ".png";
                        string filePath = Server.MapPath("~/" + "/uploads/images/qr-user/" + fileName);
                        _link_anh_qr = "/uploads/images/qr-user/" + fileName;
                        qrCodeBitmap.Save(filePath, ImageFormat.Png);
                    }

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
                    _ob.dienthoai = _sdt;
                    _ob.qr_code = ResolveUrl(_link_anh_qr);
                    _ob.anhdaidien = (!string.IsNullOrEmpty(_anhdaidien_new)) ? _anhdaidien_new : "/uploads/images/macdinh.jpg";
                    _ob.permission = PortalScope_cl.NormalizePermissionWithScope("", PortalScope_cl.ScopeAdmin);
                    _ob.makhoiphuc = "141191";
                    _ob.hsd_makhoiphuc = DateTime.Parse("01/01/1991");
                    _ob.block = false;
                    _ob.nguoitao = ViewState["taikhoan"].ToString();
                    _ob.email = _email;
                    _ob.DongA = 0;

                    // Ref chỉ dùng ở Home. Tài khoản do Admin tạo không tham gia tuyến ref.
                    _ob.Affiliate_tai_khoan_cap_tren = "";
                    _ob.Affiliate_cap_tuyen = 0;
                    _ob.Affiliate_duong_dan_tuyen_tren = ",";
                    // Tài khoản admin tạo mới luôn khởi tạo ở tier 0 (chưa có hành vi).
                    _ob.HeThongSanPham_Cap123 = 0;
                    _ob.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 = null;
                    _ob.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 = null;
                    _ob.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 = null;


                    db.taikhoan_tbs.InsertOnSubmit(_ob);
                    db.SubmitChanges();

                    Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "1000", "warning");
                    if (CanManageAdminAccounts(db, GetCurrentAdminUser()))
                        Response.Redirect(BuildViewUrl(ViewPermission, _user), false);
                    else
                        Response.Redirect(BuildListUrl(), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                // ===== EDIT =====
                else
                {
                    if (ViewState["id_edit"] == null)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                            thongbao_class.metro_dialog("Thông báo", "Thiếu thông tin tài khoản cần sửa.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }

                    string _tk_edit = ViewState["id_edit"].ToString();
                    var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk_edit);
                    if (q == null)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                            thongbao_class.metro_dialog("Thông báo", "Không tìm thấy tài khoản cần sửa.", "false", "false", "OK", "alert", ""), true);
                        return;
                    }

                    string resolvedScope = PortalScope_cl.ResolveScope(q.taikhoan, q.phanloai, q.permission);
                    if (string.Equals(resolvedScope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase))
                    {
                        _loaitaikhoan = AccountTypeHomeDefault;
                    }
                    else if (string.Equals(resolvedScope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
                    {
                        _loaitaikhoan = AccountTypeShopDefault;
                    }
                    else
                    {
                        _loaitaikhoan = AccountType_cl.IsTreasury(q.phanloai) ? AccountType_cl.Treasury : AccountTypeAdminStaff;
                    }

                    if (!string.IsNullOrEmpty(_email))
                    {
                        bool emailExist = db.taikhoan_tbs.Any(p => p.taikhoan != _tk_edit && p.email.ToLower() == _email.ToLower());
                        if (emailExist)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                                thongbao_class.metro_dialog("Thông báo", "Email này đã được dùng cho một tài khoản khác.", "false", "false", "OK", "alert", ""), true);
                            return;
                        }
                    }

                    q.hoten = _fullname;
                    q.ten = str_cl.tachten(_fullname);
                    q.hoten_khongdau = str_cl.remove_vietnamchar(_fullname);
                    q.dienthoai = _sdt;
                    q.email = _email;

                    // Không cho sửa trực tiếp affiliate; phanloai home sẽ được cập nhật theo tầng nếu có đổi tầng.

                    if (!string.IsNullOrEmpty(_ngaysinh))
                    {
                        DateTime d;
                        if (DateTime.TryParseExact(_ngaysinh, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"), DateTimeStyles.None, out d))
                            q.ngaysinh = d;
                        else
                            q.ngaysinh = DateTime.Parse(_ngaysinh);
                    }
                    else
                    {
                        q.ngaysinh = null;
                    }

                    if (!string.IsNullOrEmpty(_anhdaidien_new))
                    {
                        string old = q.anhdaidien;
                        q.anhdaidien = _anhdaidien_new;

                        if (!string.IsNullOrEmpty(old) && old != "/uploads/images/macdinh.jpg" && old != _anhdaidien_new)
                        {
                            File_Folder_cl.del_file(old);
                        }
                    }

                    string tkAdmin = GetCurrentAdminUser();
                    if (string.Equals(resolvedScope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase))
                    {
                        int currentTier = GetCurrentTierFromAccountForEdit(db, q);
                        if (currentTier < TierHome_cl.Tier1) currentTier = TierHome_cl.Tier1;
                        if (currentTier > TierHome_cl.Tier3) currentTier = TierHome_cl.Tier3;

                        int targetTier = currentTier;
                        int parsedTier;
                        if (int.TryParse((ddl_cap_sp.SelectedValue ?? "").Trim(), out parsedTier) && parsedTier >= TierHome_cl.Tier1 && parsedTier <= TierHome_cl.Tier3)
                            targetTier = parsedTier;

                        if (targetTier != currentTier)
                        {
                            if (!CanAdjustTierTarget(db, tkAdmin, targetTier))
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                                    thongbao_class.metro_dialog("Thông báo", "Bạn chưa có quyền đổi sang tầng này.", "false", "false", "OK", "alert", ""), true);
                                return;
                            }

                            q.phanloai = TierHome_cl.GetTenTangHome(targetTier);
                            ResetHomeHanhVi(q);
                            CancelPendingHanhViRequests(
                                db,
                                q.taikhoan,
                                tkAdmin,
                                "Yêu cầu bị huỷ do admin thay đổi tầng hồ sơ. Vui lòng tạo yêu cầu xác nhận hành vi mới.");

                            Helper_DongA_cl.AddNotify(
                                db,
                                tkAdmin,
                                q.taikhoan,
                                "Tầng hồ sơ của bạn đã được cập nhật: " + TierHome_cl.GetTenTangHome(targetTier) + ". Quyền hành vi đã được đặt lại, vui lòng gửi yêu cầu xác nhận hành vi mới.",
                                "/home/tao-yeu-cau.aspx");
                        }
                    }

                    db.SubmitChanges();

                    Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "1000", "warning");
                    Response.Redirect(BuildListUrl(), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
            }
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    #endregion

    #region phân quyền
    public void reset_control_phanquyen()
    {
        check_all_quyen_quanlynhanvien.Checked = false;
        check_list_quyen_quanlynhanvien.SelectedIndex = -1;
        check_all_quyen_1.Checked = false;
        check_list_quyen_1.SelectedIndex = -1;
        check_all_quyen_hoso.Checked = false;
        check_list_quyen_hoso.SelectedIndex = -1;
        check_all_quyen_noidung_home.Checked = false;
        check_list_quyen_noidung_home.SelectedIndex = -1;
        ViewState["tk_phanquyen"] = null;
    }

    protected void but_close_form_phanquyen_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildListUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void OpenPermissionFormByAccount(string taiKhoan)
    {
        check_login_cl.check_login_admin("5", "5");
        reset_control_phanquyen();
        string _tk = (taiKhoan ?? "").Trim();
        if (string.IsNullOrEmpty(_tk))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                thongbao_class.metro_dialog("Thông báo", "Không xác định được tài khoản cần phân quyền. Vui lòng thử lại.", "false", "false", "OK", "alert", ""), true);
            return;
        }
        if (PermissionProfile_cl.IsRootAdmin(_tk))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                thongbao_class.metro_dialog("Thông báo", "Tài khoản admin gốc luôn giữ toàn quyền và không chỉnh ở màn hình này.", "false", "false", "OK", "alert", ""), true);
            return;
        }
        ViewState["tk_phanquyen"] = _tk;

        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
            if (q == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "Không tìm thấy tài khoản.", "false", "false", "OK", "alert", ""), true);
                return;
            }

            string targetScope = PortalScope_cl.ResolveScope(q.taikhoan, q.phanloai, q.permission);
            if (!string.Equals(targetScope, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "Chỉ tài khoản thuộc hệ admin mới được phân quyền tại màn hình này.", "false", "false", "OK", "alert", ""), true);
                return;
            }

            string _quyen = q.permission;

            if (!string.IsNullOrEmpty(_quyen))
            {
                var quyenArray = _quyen.Split(',');

                foreach (ListItem item in check_list_quyen_quanlynhanvien.Items)
                    item.Selected = quyenArray.Contains(item.Value);
                check_all_quyen_quanlynhanvien.Checked = check_list_quyen_quanlynhanvien.Items.Cast<ListItem>().All(i => i.Selected);

                foreach (ListItem item in check_list_quyen_1.Items)
                    item.Selected = quyenArray.Contains(item.Value);
                check_all_quyen_1.Checked = check_list_quyen_1.Items.Cast<ListItem>().All(i => i.Selected);

                foreach (ListItem item in check_list_quyen_hoso.Items)
                    item.Selected = quyenArray.Contains(item.Value);
                check_all_quyen_hoso.Checked = check_list_quyen_hoso.Items.Cast<ListItem>().All(i => i.Selected);

                foreach (ListItem item in check_list_quyen_noidung_home.Items)
                    item.Selected = quyenArray.Contains(item.Value);
                check_all_quyen_noidung_home.Checked = check_list_quyen_noidung_home.Items.Cast<ListItem>().All(i => i.Selected);
            }
        }

        pn_phanquyen.Visible = true;
        up_phanquyen.Update();
    }

    protected void but_show_form_phanquyen_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("5", "5");

        LinkButton button = (LinkButton)sender;
        string _tk = (button.CommandArgument ?? "").Trim();
        if (string.IsNullOrEmpty(_tk))
            _tk = (hf_selected_taikhoan.Value ?? "").Trim();

        if (string.IsNullOrEmpty(_tk))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                thongbao_class.metro_dialog("Thông báo", "Không xác định được tài khoản cần phân quyền. Vui lòng thử lại.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        Response.Redirect(BuildViewUrl(ViewPermission, _tk), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void but_phanquyen_Click(object sender, EventArgs e)
    {
        string _tk = ViewState["tk_phanquyen"].ToString();
        if (PermissionProfile_cl.IsRootAdmin(_tk))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                thongbao_class.metro_dialog("Thông báo", "Tài khoản admin gốc luôn giữ toàn quyền và không chỉnh ở màn hình này.", "false", "false", "OK", "alert", ""), true);
            return;
        }
        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
            if (q != null)
            {
                string existingScope = PortalScope_cl.ResolveScope(q.taikhoan, q.phanloai, q.permission);
                if (!string.Equals(existingScope, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                        thongbao_class.metro_dialog("Thông báo", "Tài khoản này không thuộc hệ admin nên không áp dụng phân quyền admin.", "false", "false", "OK", "alert", ""), true);
                    return;
                }

                var allPermissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (string code in check_list_quyen_quanlynhanvien.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => (i.Value ?? "").Trim()))
                    if (code != "") allPermissions.Add(code);

                foreach (string code in check_list_quyen_1.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => (i.Value ?? "").Trim()))
                    if (code != "") allPermissions.Add(code);

                foreach (string code in check_list_quyen_hoso.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => (i.Value ?? "").Trim()))
                    if (code != "") allPermissions.Add(code);

                foreach (string code in check_list_quyen_noidung_home.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => (i.Value ?? "").Trim()))
                    if (code != "") allPermissions.Add(code);

                string selectedPermissions = string.Join(",", allPermissions.OrderBy(x => x));
                string targetScope = PortalScope_cl.ResolveScope(q.taikhoan, q.phanloai, q.permission);
                if (PortalScope_cl.ContainsAnyAdminPermission(selectedPermissions) || AccountType_cl.IsTreasury(q.phanloai) || PermissionProfile_cl.IsRootAdmin(q.taikhoan))
                {
                    targetScope = PortalScope_cl.ScopeAdmin;
                }
                q.permission = PortalScope_cl.NormalizePermissionWithScope(selectedPermissions, targetScope);

                db.SubmitChanges();

                Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "1000", "warning");
                Response.Redirect(BuildListUrl(), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }
    }

    protected void check_all_quyen_quanlynhanvien_CheckedChanged(object sender, EventArgs e)
    {
        bool isChecked = check_all_quyen_quanlynhanvien.Checked;
        foreach (ListItem item in check_list_quyen_quanlynhanvien.Items) item.Selected = isChecked;
    }

    protected void check_list_quyen_quanlynhanvien_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool allSelected = check_list_quyen_quanlynhanvien.Items.Cast<ListItem>().All(i => i.Selected);
        check_all_quyen_quanlynhanvien.Checked = allSelected;
    }

    protected void check_all_quyen_1_CheckedChanged(object sender, EventArgs e)
    {
        bool isChecked = check_all_quyen_1.Checked;
        foreach (ListItem item in check_list_quyen_1.Items) item.Selected = isChecked;
    }

    protected void check_list_quyen_1_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool allSelected = check_list_quyen_1.Items.Cast<ListItem>().All(i => i.Selected);
        check_all_quyen_1.Checked = allSelected;
    }

    protected void check_all_quyen_hoso_CheckedChanged(object sender, EventArgs e)
    {
        bool isChecked = check_all_quyen_hoso.Checked;
        foreach (ListItem item in check_list_quyen_hoso.Items) item.Selected = isChecked;
    }

    protected void check_list_quyen_hoso_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool allSelected = check_list_quyen_hoso.Items.Cast<ListItem>().All(i => i.Selected);
        check_all_quyen_hoso.Checked = allSelected;
    }

    protected void check_all_quyen_noidung_home_CheckedChanged(object sender, EventArgs e)
    {
        bool isChecked = check_all_quyen_noidung_home.Checked;
        foreach (ListItem item in check_list_quyen_noidung_home.Items) item.Selected = isChecked;
    }

    protected void check_list_quyen_noidung_home_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool allSelected = check_list_quyen_noidung_home.Items.Cast<ListItem>().All(i => i.Selected);
        check_all_quyen_noidung_home.Checked = allSelected;
    }
    #endregion

}
