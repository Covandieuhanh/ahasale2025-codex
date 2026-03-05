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
    private const string ViewAffiliateTree = "afftree";

    private bool IsRootAdminCurrent()
    {
        string tk = (ViewState["taikhoan"] ?? "").ToString();
        return PermissionProfile_cl.IsRootAdmin(tk);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri;
            check_login_cl.check_login_admin("none", "none");

            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";

            ViewState["taikhoan"] = _tk;
            but_show_form_add.Visible = IsRootAdminCurrent();

            set_dulieu_macdinh();
            show_main();

            ApplyOpenViewFromQuery();
        }

        but_show_form_add.NavigateUrl = BuildAddUrl();
        but_show_filter.NavigateUrl = BuildFilterUrl();
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
        if (mode == ViewAdd || mode == ViewEdit || mode == ViewFilter || mode == ViewPermission || mode == ViewAffiliateTree)
            return mode;
        return "";
    }

    private string GetPageScopeForUrl()
    {
        string scope = NormalizeFilterScope(Request.QueryString["scope"]);
        if (!string.IsNullOrEmpty(scope)) return scope;
        scope = NormalizeFilterScope(ViewState["filter_scope"] as string);
        return scope;
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

        return "/admin/quan-ly-tai-khoan/default.aspx" + (query.Count > 0 ? "?" + string.Join("&", query) : "");
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

        return "/admin/quan-ly-tai-khoan/default.aspx?" + string.Join("&", query);
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

    public string BuildAffiliateTreeUrl(object taiKhoanObj)
    {
        string tk = (taiKhoanObj ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(tk)) return "#";
        return BuildViewUrl(ViewAffiliateTree, tk);
    }

    public string BuildEditUrl(object taiKhoanObj)
    {
        string tk = (taiKhoanObj ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(tk)) return "#";
        return BuildViewUrl(ViewEdit, tk);
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

        if (view == ViewAffiliateTree)
        {
            if (string.IsNullOrEmpty(targetTk))
            {
                up_main.Visible = true;
                return;
            }
            ShowAffiliateTreePopup(targetTk);
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

    private int GetGiaTriByCap(taikhoan_tb acc, int cap)
    {
        if (acc == null) return 0;
        if (cap == 1) return acc.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 ?? 0;
        if (cap == 2) return acc.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 ?? 0;
        if (cap == 3) return acc.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 ?? 0;
        return 0;
    }

    private int NormalizeGiaTriForCap(int cap, int? rawGiaTri)
    {
        int giaTri = rawGiaTri ?? 0;
        if (cap == 1)
        {
            if (giaTri == 15 || giaTri == 9 || giaTri == 6) return giaTri;
            return 15;
        }
        if (cap == 2)
        {
            if (giaTri == 10 || giaTri == 15 || giaTri == 25) return giaTri;
            return 10;
        }
        if (cap == 3)
        {
            if (giaTri == 10 || giaTri == 6 || giaTri == 4) return giaTri;
            return 10;
        }
        return 0;
    }

    private int GetCurrentTierFromAccountForEdit(dbDataContext db, taikhoan_tb acc)
    {
        if (acc == null) return TierHome_cl.Tier0;

        int tierFromAccount = TierHome_cl.GetTierFromHanhVi(TierHome_cl.GetCurrentHanhViFromAccount(acc));
        if (tierFromAccount > TierHome_cl.Tier0) return tierFromAccount;

        int tierFromHistory = TierHome_cl.TinhTierHome(db, acc.taikhoan);
        if (tierFromHistory > TierHome_cl.Tier0) return tierFromHistory;

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

    private void ApplyTierToHomeAccount(taikhoan_tb acc, int targetTier, out int newCap, out int newGiaTri)
    {
        newCap = 1;
        newGiaTri = 15;
        if (acc == null) return;

        if (targetTier >= TierHome_cl.Tier3)
        {
            newCap = 3;
            newGiaTri = NormalizeGiaTriForCap(3, acc.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4);
            acc.HeThongSanPham_Cap123 = 3;
            acc.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 = newGiaTri;
            return;
        }

        if (targetTier >= TierHome_cl.Tier2)
        {
            newCap = 2;
            newGiaTri = NormalizeGiaTriForCap(2, acc.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10);
            acc.HeThongSanPham_Cap123 = 2;
            acc.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 = newGiaTri;
            return;
        }

        newCap = 1;
        newGiaTri = NormalizeGiaTriForCap(1, acc.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6);
        acc.HeThongSanPham_Cap123 = 1;
        acc.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 = newGiaTri;
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
                                    ob1.ChiPhanTram_BanDichVu_ChoSan,
                                    ob1.HeThongSanPham_Cap123,

                                    ob1.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6,
                                    ob1.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10,
                                    ob1.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4,



                                    // ✅ NEW: affiliate
                                    ob1.Affiliate_tai_khoan_cap_tren
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
                if (!string.IsNullOrEmpty(_key))
                    list_all = list_all.Where(p => p.hoten.Contains(_key) || p.hoten_khongdau.Contains(_key) || p.taikhoan == _key || p.email == _key || p.dienthoai == _key);

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

                // ✅ NEW: map người giới thiệu ra text
                var listRefAcc = list_split
                    .Where(x => !string.IsNullOrEmpty(x.Affiliate_tai_khoan_cap_tren))
                    .Select(x => x.Affiliate_tai_khoan_cap_tren)
                    .Distinct()
                    .ToList();

                var dictRef = db.taikhoan_tbs
                    .Where(x => listRefAcc.Contains(x.taikhoan))
                    .Select(x => new { x.taikhoan, x.hoten })
                    .ToList()
                    .ToDictionary(x => x.taikhoan, x => x.hoten);

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
                    bool canShowPhanQuyen = isAdminScope && !PermissionProfile_cl.IsRootAdmin(x.taikhoan);

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
                        x.ChiPhanTram_BanDichVu_ChoSan,

                        HanhVi_HienThi = hanhViHienThi,
                        IsAdminScope = isAdminScope,
                        CanShowPhanQuyen = canShowPhanQuyen,

                        // ✅ NEW: hiển thị người giới thiệu
                        NguoiGioiThieu_HienThi = (string.IsNullOrEmpty(x.Affiliate_tai_khoan_cap_tren))
                            ? "Không có"
                            : x.Affiliate_tai_khoan_cap_tren + " - " + (dictRef.ContainsKey(x.Affiliate_tai_khoan_cap_tren) ? dictRef[x.Affiliate_tai_khoan_cap_tren] : "")
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
    #region ✅ NEW: bind dropdown Người giới thiệu
    public void bind_dropdown_nguoi_gioi_thieu(string selectedValue = "")
    {
        using (dbDataContext db = new dbDataContext())
        {
            var list = db.taikhoan_tbs
                .OrderBy(p => p.taikhoan)
                .Select(p => new
                {
                    p.taikhoan,
                    p.hoten
                }).ToList();

            ddl_nguoi_gioi_thieu.Items.Clear();

            ddl_nguoi_gioi_thieu.Items.Add(new ListItem("Không có", ""));

            foreach (var item in list)
            {
                string text = item.taikhoan + " - " + item.hoten;
                ddl_nguoi_gioi_thieu.Items.Add(new ListItem(text, item.taikhoan));
            }

            if (!string.IsNullOrEmpty(selectedValue))
            {
                var li = ddl_nguoi_gioi_thieu.Items.FindByValue(selectedValue);
                if (li != null) ddl_nguoi_gioi_thieu.SelectedValue = selectedValue;
                else ddl_nguoi_gioi_thieu.SelectedValue = "";
            }
            else
            {
                ddl_nguoi_gioi_thieu.SelectedValue = "";
            }
        }
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





        ddl_nguoi_gioi_thieu.Items.Clear();

        ViewState["add_edit"] = null;
        ViewState["id_edit"] = null;

        Button2.Visible = false;
        Label2.Text = "";
        txt_taikhoan.ReadOnly = false;

        DropDownList1.Enabled = true;
        ddl_nguoi_gioi_thieu.Enabled = true;
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
        if (!IsRootAdminCurrent())
        {
            Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Chỉ tài khoản admin gốc mới được tạo tài khoản ở trang admin.", "1000", "warning");
            Response.Redirect(BuildListUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        reset_control_add_edit();
        ddl_cap_sp.SelectedValue = "1";
        ddl_cap_sp.Enabled = false;

        PlaceHolder1.Visible = true;
        ViewState["add_edit"] = "add";
        Label1.Text = "THÊM TÀI KHOẢN ADMIN";
        but_add_edit.Text = "THÊM MỚI";

        bind_dropdown_nguoi_gioi_thieu("");

        DropDownList1.Enabled = true;
        ddl_nguoi_gioi_thieu.Enabled = true;

        pn_add.Visible = true;
        up_add.Update();
    }

    protected void but_show_form_add_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");
            if (!IsRootAdminCurrent())
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "Chỉ tài khoản admin gốc mới được tạo tài khoản ở trang admin.", "false", "false", "OK", "alert", ""), true);
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
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk_edit);
            if (q == null)
            {
                q = db.taikhoan_tbs
                    .AsEnumerable()
                    .FirstOrDefault(p => string.Equals((p.taikhoan ?? "").Trim(), _tk_edit, StringComparison.OrdinalIgnoreCase));
            }
            if (q == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "Không tìm thấy tài khoản cần xem chi tiết.", "false", "false", "OK", "alert", ""), true);
                return;
            }

            ViewState["id_edit"] = q.taikhoan;

            string tkAdmin = GetCurrentAdminUser();
            bool isHomeAccount = IsHomeScopeAccount(q);
            int currentTier = GetCurrentTierFromAccountForEdit(db, q);
            if (currentTier < TierHome_cl.Tier1) currentTier = TierHome_cl.Tier1;
            if (currentTier > TierHome_cl.Tier3) currentTier = TierHome_cl.Tier3;

            string tierValue = currentTier.ToString();
            var liCap = ddl_cap_sp.Items.FindByValue(tierValue);
            ddl_cap_sp.SelectedValue = (liCap != null) ? tierValue : "1";
            ddl_cap_sp.Enabled = isHomeAccount && CanAdjustAnyHomeTier(db, tkAdmin);

            txt_taikhoan.Text = q.taikhoan;
            txt_taikhoan.ReadOnly = true;
            txt_hoten.Text = q.hoten;
            txt_dienthoai.Text = q.dienthoai;
            txt_email.Text = q.email;
            txt_ngaysinh.Text = q.ngaysinh != null ? q.ngaysinh.Value.ToString("dd/MM/yyyy") : "";

            if (!string.IsNullOrEmpty(q.phanloai))
            {
                string selectedType = AccountType_cl.Normalize(q.phanloai);
                var liType = DropDownList1.Items.FindByValue(selectedType);
                if (liType == null)
                {
                    DropDownList1.Items.Add(new ListItem(selectedType, selectedType));
                    liType = DropDownList1.Items.FindByValue(selectedType);
                }
                if (liType != null) DropDownList1.SelectedValue = selectedType;
            }

            string anh = q.anhdaidien;
            if (string.IsNullOrEmpty(anh)) anh = "/uploads/images/macdinh.jpg";
            Label2.Text = "<div class='mt-2'><small>Ảnh hiện tại</small></div><img width='100' src='" + anh + "' />";
            Button2.Visible = (anh != "/uploads/images/macdinh.jpg");

            bind_dropdown_nguoi_gioi_thieu(q.Affiliate_tai_khoan_cap_tren);
            DropDownList1.Enabled = false;
            ddl_nguoi_gioi_thieu.Enabled = false;
        }

        pn_add.Visible = true;
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



            // ✅ NEW
            string _nguoi_gioi_thieu = ddl_nguoi_gioi_thieu.SelectedValue;

            using (dbDataContext db = new dbDataContext())
            {
                bool isAddMode = ViewState["add_edit"] != null && ViewState["add_edit"].ToString() == "add";
                if (isAddMode && !IsRootAdminCurrent())
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                        thongbao_class.metro_dialog("Thông báo", "Chỉ tài khoản admin gốc mới được tạo tài khoản ở trang admin.", "false", "false", "OK", "alert", ""), true);
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
                    bool validAdminType = string.Equals(_loaitaikhoan, "Nhân viên admin", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(_loaitaikhoan, "Cộng tác phát triển", StringComparison.OrdinalIgnoreCase)
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

                    // ✅ validate người giới thiệu nếu có
                    taikhoan_tb refAcc = null;
                    if (!string.IsNullOrEmpty(_nguoi_gioi_thieu))
                    {
                        refAcc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _nguoi_gioi_thieu);
                        if (refAcc == null)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                                thongbao_class.metro_dialog("Thông báo", "Tài khoản giới thiệu không tồn tại.", "false", "false", "OK", "alert", ""), true);
                            return;
                        }
                    }

                    string _link_qr = $"https://ahasale.vn/{_user}.info";
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

                    // ✅ SET affiliate theo yêu cầu
                    if (refAcc == null)
                    {
                        _ob.Affiliate_tai_khoan_cap_tren = "";
                        _ob.Affiliate_duong_dan_tuyen_tren = ",";
                        _ob.Affiliate_cap_tuyen = 0;
                    }
                    else
                    {
                        string refPath = string.IsNullOrEmpty(refAcc.Affiliate_duong_dan_tuyen_tren) ? "," : refAcc.Affiliate_duong_dan_tuyen_tren;
                        int refLevel = (refAcc.Affiliate_cap_tuyen == null) ? 0 : refAcc.Affiliate_cap_tuyen.Value;

                        _ob.Affiliate_tai_khoan_cap_tren = refAcc.taikhoan;
                        _ob.Affiliate_cap_tuyen = refLevel + 1;
                        _ob.Affiliate_duong_dan_tuyen_tren = refPath + refAcc.taikhoan + ",";
                    }
                    // Tài khoản admin tạo mới luôn khởi tạo ở tier 0 (chưa có hành vi).
                    _ob.HeThongSanPham_Cap123 = 0;
                    _ob.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 = null;
                    _ob.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 = null;
                    _ob.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 = null;


                    db.taikhoan_tbs.InsertOnSubmit(_ob);
                    db.SubmitChanges();

                    Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "1000", "warning");
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
                    if (IsHomeScopeAccount(q))
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

                            int oldCap = q.HeThongSanPham_Cap123 ?? 0;
                            int oldGiaTri = GetGiaTriByCap(q, oldCap);

                            int newCap;
                            int newGiaTri;
                            ApplyTierToHomeAccount(q, targetTier, out newCap, out newGiaTri);
                            q.phanloai = TierHome_cl.GetTenTangHome(targetTier);

                            YeuCau_HeThongSanPham_tb yc = new YeuCau_HeThongSanPham_tb();
                            yc.taikhoan = q.taikhoan;
                            yc.CapHienTai = oldCap;
                            yc.GiaTriHienTai = oldGiaTri;
                            yc.CapYeuCau = newCap;
                            yc.GiaTriYeuCau = newGiaTri;
                            yc.TrangThai = 1;
                            yc.NgayTao = AhaTime_cl.Now;
                            yc.NgayDuyet = AhaTime_cl.Now;
                            yc.NguoiDuyet = tkAdmin;
                            yc.GhiChuAdmin = "Điều chỉnh tầng home thủ công bởi admin.";
                            db.YeuCau_HeThongSanPham_tbs.InsertOnSubmit(yc);

                            Helper_DongA_cl.AddNotify(
                                db,
                                tkAdmin,
                                q.taikhoan,
                                "Tầng hồ sơ của bạn đã được cập nhật: " + TierHome_cl.GetTenTangHome(targetTier) + ".",
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
    #endregion

    protected void but_close_form_aff_tree_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildListUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void but_xem_cay_aff_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin("none", "none");

            LinkButton button = (LinkButton)sender;
            string tk = button.CommandArgument;

            ShowAffiliateTreePopup(tk);
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }
    public void ShowAffiliateTreePopup(string currentUser)
    {
        using (dbDataContext db = new dbDataContext())
        {
            var cur = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == currentUser);
            if (cur == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "Không tìm thấy tài khoản.", "false", "false", "OK", "alert", ""), true);
                return;
            }

            lb_aff_current.Text = currentUser;

            // 1) Lấy tuyến trên (ancestors) từ Affiliate_duong_dan_tuyen_tren
            List<string> ancestors = new List<string>();
            if (!string.IsNullOrEmpty(cur.Affiliate_duong_dan_tuyen_tren))
            {
                // ví dụ: ",a,b,c,"
                var arr = cur.Affiliate_duong_dan_tuyen_tren
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                // arr sẽ là tuyến trên từ thấp lên cao theo cách bạn lưu
                // mình giữ thứ tự như vậy để build chain
                ancestors = arr;
            }

            // Query info tất cả tài khoản liên quan (tuyến trên + current + tuyến dưới)
            // 2) lấy full tuyến dưới (descendants) của currentUser
            var allUsers = db.taikhoan_tbs
                .Select(x => new
                {
                    x.taikhoan,
                    x.hoten,
                    x.Affiliate_tai_khoan_cap_tren
                })
                .ToList();

            // Dictionary để map taikhoan -> hoten
            Dictionary<string, string> dictName = allUsers
                .ToDictionary(x => x.taikhoan, x => (x.hoten ?? ""));

            // Build map parent -> children cho tất cả
            Dictionary<string, List<string>> mapChildren = new Dictionary<string, List<string>>();
            foreach (var u in allUsers)
            {
                string parent = u.Affiliate_tai_khoan_cap_tren;
                if (string.IsNullOrEmpty(parent)) continue;

                if (!mapChildren.ContainsKey(parent))
                    mapChildren[parent] = new List<string>();

                mapChildren[parent].Add(u.taikhoan);
            }

            // Sắp xếp children theo taikhoan cho đẹp
            foreach (var k in mapChildren.Keys.ToList())
            {
                mapChildren[k] = mapChildren[k].OrderBy(x => x).ToList();
            }

            // 3) Xác định ROOT hiển thị:
            // Nếu có ancestors thì root = ancestor đầu tiên trong chuỗi
            // Nếu không có ancestors => root chính là currentUser
            string root = (ancestors.Count > 0) ? ancestors[0] : currentUser;

            // 4) Build HTML tree:
            // Mình dựng chain tuyến trên trước,
            // rồi tại node currentUser thì append toàn bộ descendants.
            string html = "<ul>";

            // Nếu có ancestors: build chain
            if (ancestors.Count > 0)
            {
                html += BuildAncestorChainHtml(ancestors, currentUser, dictName, mapChildren);
            }
            else
            {
                // Không có tuyến trên => root=currentUser luôn
                html += BuildNodeWithDescendants(currentUser, currentUser, dictName, mapChildren);
            }

            html += "</ul>";

            lit_aff_tree.Text = html;
            pn_aff_tree.Visible = true;
            up_aff_tree.Update();
        }
    }
    private string BuildAncestorChainHtml(
    List<string> ancestors,
    string currentUser,
    Dictionary<string, string> dictName,
    Dictionary<string, List<string>> mapChildren)
    {
        // ancestors ví dụ: [a,b,c] nghĩa là: a -> b -> c -> currentUser
        // Nhưng currentUser đã nằm ngoài duongdan, nên chain sẽ nối đến currentUser

        string html = "";

        for (int i = 0; i < ancestors.Count; i++)
        {
            string tk = ancestors[i];
            string name = dictName.ContainsKey(tk) ? dictName[tk] : "";

            html += "<li>";
            html += $"<a href='#'>{tk} - {HttpUtility.HtmlEncode(name)}</a>";

            // Nếu đây là ancestor cuối cùng => con tiếp theo là currentUser
            if (i == ancestors.Count - 1)
            {
                html += "<ul>";
                html += BuildNodeWithDescendants(currentUser, currentUser, dictName, mapChildren);
                html += "</ul>";
            }
            else
            {
                // còn ancestor tiếp theo
                html += "<ul>";
            }
        }

        // đóng các ul/li còn mở
        for (int i = 0; i < ancestors.Count; i++)
        {
            html += "</li></ul>";
        }

        // đoạn trên sẽ dư 1 </ul> ở cuối, mình trim lại cho chuẩn:
        // thực tế BuildAncestorChainHtml return sẽ được wrap trong <ul> ngoài.
        // cách đơn giản: remove 1 </ul> cuối nếu có
        if (html.EndsWith("</ul>"))
        {
            html = html.Substring(0, html.Length - 5);
        }

        return html;
    }
    private string BuildNodeWithDescendants(
        string tk,
        string currentUser,
        Dictionary<string, string> dictName,
        Dictionary<string, List<string>> mapChildren)
    {
        string name = dictName.ContainsKey(tk) ? dictName[tk] : "";

        // highlight current user
        string liClass = (tk == currentUser) ? " class='aff-current-node'" : "";

        string html = $"<li{liClass}>";

        html += $"<a href='#'>{tk} - {HttpUtility.HtmlEncode(name)}</a>";

        // nếu có con => đệ quy
        if (mapChildren.ContainsKey(tk) && mapChildren[tk].Count > 0)
        {
            html += "<ul>";
            foreach (string child in mapChildren[tk])
            {
                html += BuildNodeWithDescendants(child, currentUser, dictName, mapChildren);
            }
            html += "</ul>";
        }

        html += "</li>";
        return html;
    }

}
