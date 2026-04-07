using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.WebControls; // ✅ BẮT BUỘC – FIX lỗi ListItem


public partial class Uc_Home_DanhChoBan_MoiNhat_UC : System.Web.UI.UserControl
{
    private static readonly string_class SearchStringHelper = new string_class();
    private const int SearchLookupCacheSeconds = 600;
    private const int GuestLatestFeedCacheSeconds = 120;
    private const int DefaultPageSize = 24;
    private const int MaxPageSize = 60;
    private const int HomeFeedLinkedBdsStride = 5;
    private const int UnifiedSearchBdsStride = 4;
    // ===== QUY ƯỚC: 1 A = 1000 VNĐ =====
    private const decimal VND_PER_A = 1000m;

    private sealed class SearchLookupItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    private sealed class GuestLatestFeedCachePayload
    {
        public List<TinItem> Items { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<string, string> LocationMap { get; set; }
    }

    // ====== PUBLIC PROPS ======
    /// <summary>
    /// Nếu truyền Idmn => lọc theo danh mục + danh mục con (dùng cho trang ds-bai-viet)
    /// Nếu để trống => không lọc (dùng cho trang chủ)
    /// </summary>
    public string Idmn
    {
        get { return (ViewState["uc_idmn"] ?? "").ToString().Trim(); }
        set { ViewState["uc_idmn"] = (value ?? "").ToString().Trim(); }
    }

    public string tin_theo_doi
    {
        get { return (ViewState["uc_tin_theo_doi"] ?? "").ToString().Trim(); }
        set { ViewState["uc_tin_theo_doi"] = (value ?? "").ToString().Trim(); }
    }

    public string lich_su_xem_tin
    {
        get { return (ViewState["uc_lich_su_xem_tin"] ?? "").ToString().Trim(); }
        set { ViewState["uc_lich_su_xem_tin"] = (value ?? "").ToString().Trim(); }
    }

    /// <summary>Tiêu đề card</summary>
    public string TitleText
    {
        get { return (ViewState["uc_title"] ?? "Mới nhất").ToString(); }
        set { ViewState["uc_title"] = string.IsNullOrWhiteSpace(value) ? "Mới nhất" : value.Trim(); }
    }

    public int ResultCount
    {
        get { return ViewState["uc_result_count"] == null ? 0 : Convert.ToInt32(ViewState["uc_result_count"]); }
        private set { ViewState["uc_result_count"] = value; }
    }

    public string SearchSummary
    {
        get { return (ViewState["uc_search_summary"] ?? "").ToString(); }
        private set { ViewState["uc_search_summary"] = value ?? ""; }
    }

    /// <summary>Bật/tắt kebab</summary>
    public bool EnableKebab
    {
        get { return ViewState["uc_kebab"] == null ? true : (bool)ViewState["uc_kebab"]; }
        set { ViewState["uc_kebab"] = value; }
    }

    /// <summary>Bật/tắt location</summary>
    public bool EnableLocation
    {
        get { return ViewState["uc_loc"] == null ? true : (bool)ViewState["uc_loc"]; }
        set { ViewState["uc_loc"] = value; }
    }

    // ===== UNIQUE KEYS để tránh đạp nhau nếu sau này 1 trang có nhiều UC =====
    private string VS_PageKey
    {
        get { return "current_page_home_" + this.ClientID; }
    }

    private string SS_LoadedKey
    {
        get { return "home_loaded_list_" + this.ClientID; }
    }

    // ✅ Quy đổi VNĐ -> A (làm tròn lên 2 chữ số thập phân)
    private decimal QuyDoi_VND_To_A(decimal vnd)
    {
        if (vnd <= 0) return 0m;
        decimal a = vnd / VND_PER_A;
        return Math.Ceiling(a * 100m) / 100m;
    }

    // =========================================================
    // ✅ PHẦN CS ĐÃ TÁCH TỪ HEADER QUA (HERO/SEARCH)
    // =========================================================
    private void BindOnlyChildrenOfDanhMucRoot()
    {
        // Nếu UC này không có ddl_Category thì bỏ qua để tránh lỗi
        if (ddl_Category == null) return;

        ddl_Category.Items.Clear();
        ddl_Category.Items.Add(new ListItem("Danh mục", ""));

        List<SearchLookupItem> children = Helper_cl.RuntimeCacheGetOrAdd<List<SearchLookupItem>>(
            "home:search:category_children:v2",
            SearchLookupCacheSeconds,
            () =>
            {
                return SqlTransientGuard_cl.Execute(() =>
                {
                    using (dbDataContext db = new dbDataContext())
                    {
                        var root = db.DanhMuc_tbs.FirstOrDefault(p =>
                            p.id_level == 1 &&
                            p.bin == false &&
                            p.kyhieu_danhmuc == "web" &&
                            (
                                (p.name != null && p.name.Trim().ToLower() == "danh mục") ||
                                (p.name_en != null && (p.name_en.Trim().ToLower() == "danh-muc" || p.name_en.Trim().ToLower() == "danhmuc"))
                            )
                        );

                        IQueryable<DanhMuc_tb> query = root != null
                            ? db.DanhMuc_tbs.Where(p => p.id_parent == root.id.ToString() && p.bin == false && p.kyhieu_danhmuc == "web")
                            : db.DanhMuc_tbs.Where(p => p.id_level == 2 && p.bin == false && p.kyhieu_danhmuc == "web");

                        return query
                            .OrderBy(p => p.rank)
                            .Select(p => new SearchLookupItem
                            {
                                Value = p.id.ToString(),
                                Text = p.name
                            })
                            .ToList();
                    }
                });
            });

        foreach (SearchLookupItem dm in children ?? new List<SearchLookupItem>())
            ddl_Category.Items.Add(new ListItem(dm.Text, dm.Value));
    }

    private void LoadThanhPho_Search()
    {
        if (ddl_Location == null) return;

        List<SearchLookupItem> list = Helper_cl.RuntimeCacheGetOrAdd<List<SearchLookupItem>>(
            "home:search:locations:v2",
            SearchLookupCacheSeconds,
            () =>
            {
                return SqlTransientGuard_cl.Execute(() =>
                {
                    using (dbDataContext db = new dbDataContext())
                    {
                        return db.ThanhPhos
                            .ToList()
                            .Select(tp => new SearchLookupItem
                            {
                                Value = tp.id.ToString(),
                                Text = TinhThanhDisplay_cl.Format(tp.Ten)
                            })
                            .ToList();
                    }
                });
            });

        ddl_Location.DataSource = list;
        ddl_Location.DataTextField = "Text";
        ddl_Location.DataValueField = "Value";
        ddl_Location.DataBind();
        ddl_Location.Items.Insert(0, new ListItem("Tất cả", ""));
        ddl_Location.SelectedIndex = 0;
    }

    // =========================================================
    // ✅ END HERO/SEARCH
    // =========================================================

    protected void Page_Load(object sender, EventArgs e)
    {
        ApplyGuestMobileAuthVisibility();

        if (!IsPostBack)
        {
            try
            {
                // ✅ THÊM: bind dropdown hero/search (KHÔNG ảnh hưởng logic cũ)
                BindOnlyChildrenOfDanhMucRoot();
                LoadThanhPho_Search();
                ApplySearchParamsFromQuery();

                // ====== LOGIC CŨ GIỮ NGUYÊN ======
                lblTitle.Text = TitleText;

                string _tk = PortalActiveMode_cl.IsHomeActive()
                    ? (HttpContext.Current.Session["taikhoan_home"] as string)
                    : "";
                if (!string.IsNullOrEmpty(_tk))
                    ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
                else
                    ViewState["taikhoan"] = "";

                set_dulieu_macdinh();
                Session[SS_LoadedKey] = new List<TinItem>();

                SqlTransientGuard_cl.Execute(() =>
                {
                    using (dbDataContext db = new dbDataContext())
                        HienThiTinMoi(db, reset: true);
                });
            }
            catch (Exception ex)
            {
                ViewState["taikhoan"] = "";
                Session[SS_LoadedKey] = new List<TinItem>();
                RepeaterTin.DataSource = new List<TinItem>();
                RepeaterTin.DataBind();
                Log_cl.Add_Log(ex.Message, "home_latest_feed", ex.StackTrace);
            }
        }
        else
        {
            // nếu page cha thay TitleText trong postback
            lblTitle.Text = TitleText;
        }
    }

    private void ApplySearchParamsFromQuery()
    {
        if (txt_Search == null || ddl_Category == null || ddl_Location == null)
            return;

        string qRaw = (Request.QueryString["q"] ?? "").Trim();
        string cat = (Request.QueryString["cat"] ?? "").Trim();
        string loc = (Request.QueryString["loc"] ?? "").Trim();
        string province = (Request.QueryString["province"] ?? "").Trim();
        string district = (Request.QueryString["district"] ?? "").Trim();
        string ward = (Request.QueryString["ward"] ?? "").Trim();
        string type = (Request.QueryString["type"] ?? "").Trim().ToLower();

        if (!string.IsNullOrWhiteSpace(qRaw))
            txt_Search.Text = HttpUtility.UrlDecode(qRaw);

        if (!string.IsNullOrWhiteSpace(cat) && ddl_Category.Items.FindByValue(cat) != null)
            ddl_Category.SelectedValue = cat;

        string selectedLoc = !string.IsNullOrWhiteSpace(province) ? province : loc;
        if (!string.IsNullOrWhiteSpace(selectedLoc) && ddl_Location.Items.FindByValue(selectedLoc) != null)
            ddl_Location.SelectedValue = selectedLoc;

        if (hfProvinceCode != null) hfProvinceCode.Value = selectedLoc;
        if (hfDistrictCode != null) hfDistrictCode.Value = district;
        if (hfWardCode != null) hfWardCode.Value = ward;
        if (hfLocationDisplay != null)
        {
            string display = "";
            if (!string.IsNullOrWhiteSpace(selectedLoc) && ddl_Location.Items.FindByValue(selectedLoc) != null)
                display = ddl_Location.Items.FindByValue(selectedLoc).Text;
            hfLocationDisplay.Value = display;
        }

        if (string.IsNullOrWhiteSpace(type))
            type = "all";

        ViewState["search_type"] = type;

        if (txt_Search != null && string.IsNullOrWhiteSpace(txt_Search.Text))
        {
            string qSlug = (Request.QueryString["qslug"] ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(qSlug))
                txt_Search.Text = AhaSearchRoutes_cl.ToKeywordFromSlug(qSlug);
        }
    }

    private string ResolveSearchType()
    {
        string type = (ViewState["search_type"] ?? "").ToString().Trim().ToLower();
        if (string.IsNullOrWhiteSpace(type))
            type = (Request.QueryString["type"] ?? "").Trim().ToLower();
        return NormalizeSearchTypeLocal(type);
    }

    private static string NormalizeSearchTypeLocal(string rawType)
    {
        string value = (rawType ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(value))
            return "all";

        if (value == "service" || value == "dichvu" || value == "dịchvụ")
            return "service";

        if (value == "product" || value == "sanpham" || value == "sảnphẩm")
            return "product";

        return "all";
    }

    private void ApplyGuestMobileAuthVisibility()
    {
        if (phGuestMobileQuickAuth == null)
            return;

        string tkEnc = PortalRequest_cl.GetCurrentAccountEncrypted();
        bool isLogged = !string.IsNullOrEmpty(tkEnc);
        bool isShopPortal = PortalRequest_cl.IsShopPortalRequest();
        phGuestMobileQuickAuth.Visible = (!isLogged && !isShopPortal);
    }

    public void set_dulieu_macdinh()
    {
        ViewState[VS_PageKey] = ResolveRequestedPage().ToString();
    }

    private int ResolveRequestedPage()
    {
        int page = 1;
        int parsed;
        if (int.TryParse((Request.QueryString["page"] ?? "").Trim(), out parsed) && parsed > 0)
            page = parsed;
        return page;
    }

    private int ResolveRequestedPageSize()
    {
        int pageSize = DefaultPageSize;
        int parsed;
        if (int.TryParse((Request.QueryString["pageSize"] ?? "").Trim(), out parsed))
            pageSize = parsed;
        if (pageSize < 8) pageSize = 8;
        if (pageSize > MaxPageSize) pageSize = MaxPageSize;
        return pageSize;
    }

    private string BuildPageUrl(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 8) pageSize = DefaultPageSize;
        if (pageSize > MaxPageSize) pageSize = MaxPageSize;

        var qs = HttpUtility.ParseQueryString(Request.QueryString.ToString());
        qs["page"] = page.ToString();
        qs["pageSize"] = pageSize.ToString();

        string path = (Request.Url == null ? Request.RawUrl : Request.Url.AbsolutePath) ?? "/";
        return path + "?" + qs.ToString();
    }

    private void BindPager(int currentPage, int totalPages, int pageSize)
    {
        if (litPager == null)
            return;

        if (totalPages <= 1)
        {
            litPager.Text = string.Empty;
            return;
        }

        var sb = new System.Text.StringBuilder();
        sb.Append("<nav class=\"aha-feed-pager\" aria-label=\"Phân trang\">");

        if (currentPage > 1)
            sb.AppendFormat("<a class=\"pg-link\" href=\"{0}\" aria-label=\"Trang trước\">&laquo; Trước</a>", ResolveUrl(BuildPageUrl(currentPage - 1, pageSize)));

        int start = Math.Max(1, currentPage - 2);
        int end = Math.Min(totalPages, currentPage + 2);

        if (start > 1)
        {
            sb.AppendFormat("<a class=\"pg-link\" href=\"{0}\">1</a>", ResolveUrl(BuildPageUrl(1, pageSize)));
            if (start > 2) sb.Append("<span class=\"pg-dots\">...</span>");
        }

        for (int i = start; i <= end; i++)
        {
            if (i == currentPage)
                sb.AppendFormat("<span class=\"pg-current\" aria-current=\"page\">{0}</span>", i);
            else
                sb.AppendFormat("<a class=\"pg-link\" href=\"{0}\">{1}</a>", ResolveUrl(BuildPageUrl(i, pageSize)), i);
        }

        if (end < totalPages)
        {
            if (end < totalPages - 1) sb.Append("<span class=\"pg-dots\">...</span>");
            sb.AppendFormat("<a class=\"pg-link\" href=\"{0}\">{1}</a>", ResolveUrl(BuildPageUrl(totalPages, pageSize)), totalPages);
        }

        if (currentPage < totalPages)
            sb.AppendFormat("<a class=\"pg-link\" href=\"{0}\" aria-label=\"Trang sau\">Sau &raquo;</a>", ResolveUrl(BuildPageUrl(currentPage + 1, pageSize)));

        sb.Append("</nav>");
        litPager.Text = sb.ToString();
    }

    // ===== FILTER DANH MỤC + CON =====
    private List<string> GetDanhMucFilter(dbDataContext db, string idmn)
    {
        if (string.IsNullOrWhiteSpace(idmn)) return null;

        var list = db.DanhMuc_tbs
            .Where(dm => dm.id_parent == idmn)
            .Select(dm => dm.id.ToString())
            .ToList();

        if (!list.Contains(idmn)) list.Add(idmn);
        return list;
    }

    private static string BuildKeywordSlug(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return "";
        try
        {
            string slug = SearchStringHelper.replace_name_to_url(keyword);
            return (slug ?? "").Trim();
        }
        catch
        {
            return "";
        }
    }

    public void HienThiTinMoi(dbDataContext db, bool reset = false)
    {
        if (TryBindCachedGuestHomepageFeed(reset))
            return;

        BaiVietSearchSchema_cl.EnsureSchemaSafe(db);

        string idmn = this.Idmn;
        string Is_tin_theo_doi = this.tin_theo_doi;
        string Is_lich_su_xem_tin = this.lich_su_xem_tin;
        var danhMucIds = GetDanhMucFilter(db, idmn);
        var visibleBase = AccountVisibility_cl.FilterVisibleTradePosts(db, db.BaiViet_tbs);
        bool hasSearchColumns = BaiVietSearchSchema_cl.HasSearchColumns(db);

        string searchType = ResolveSearchType();
        if (string.Equals(searchType, "service", StringComparison.OrdinalIgnoreCase))
            visibleBase = visibleBase.Where(p => p.phanloai == AccountVisibility_cl.PostTypeService);
        else if (string.Equals(searchType, "product", StringComparison.OrdinalIgnoreCase))
            visibleBase = visibleBase.Where(p => p.phanloai == AccountVisibility_cl.PostTypeProduct);

        string keyword = txt_Search.Text.Trim();
        string keywordSlug = BuildKeywordSlug(keyword);
        string keywordNormalized = hasSearchColumns ? BaiVietSearchSchema_cl.NormalizeText(keyword) : "";
        string thanhPho = ddl_Location.SelectedValue;
        string selectedDanhMuc = ddl_Category.SelectedValue;
        Dictionary<long, string> thanhPhoMap = null;

        IQueryable<TinItem> list_all;
        List<string> danhMucIds_Search = null;

        if (!string.IsNullOrEmpty(selectedDanhMuc))
        {
            danhMucIds_Search = new List<string> { selectedDanhMuc };
        }

        string hiddenIds = Session["HiddenPostIds"] as string;

        List<int> hideList = new List<int>();
        if (!string.IsNullOrWhiteSpace(hiddenIds))
        {
            hideList = hiddenIds
                .Split(',')
                .Select(x => int.Parse(x))
                .ToList();
        }
        if (!string.IsNullOrEmpty(Is_lich_su_xem_tin))
        {
            string taiKhoanHienTai = Convert.ToString(ViewState["taikhoan"]) ?? "";

            // Chỉ lấy các tin đã xem bởi user hiện tại
            var query = visibleBase.Where(p =>
                db.TinDaXem_tbs.Any(t => t.idBaiViet == p.id && t.TaiKhoan == taiKhoanHienTai)
                && !hideList.Contains(p.id)
            );

            // Lọc theo danh mục nếu có
            if (danhMucIds_Search != null && danhMucIds_Search.Count > 0)
            {
                query = query.Where(p =>
                    danhMucIds_Search.Contains(p.id_DanhMuc) ||
                    danhMucIds_Search.Contains(p.id_DanhMucCap2)
                );
            }

            if (hasSearchColumns)
            {
                list_all =
                    from ob1 in query
                    join ob2 in db.DanhMuc_tbs
                        on ob1.id_DanhMuc equals ob2.id.ToString() into g1
                    from ob2 in g1.DefaultIfEmpty()

                    join ob3 in db.DanhMuc_tbs
                        on ob1.id_DanhMucCap2 equals ob3.id.ToString() into g2
                    from ob3 in g2.DefaultIfEmpty()

                    join ob4 in db.TheoDoiTin_dbs.Where(t => t.taikhoan == taiKhoanHienTai)
                        on ob1.id equals ob4.idBaiviet into gj
                    from ob4 in gj.DefaultIfEmpty()

                    join ob5 in db.TinDaXem_tbs
                        on ob1.id equals ob5.idBaiViet
                    where ob5.TaiKhoan == taiKhoanHienTai // ✅ chỉ lấy bản ghi của user hiện tại

                    select new TinItem
                    {
                        id = ob1.id,
                        image = ob1.image,
                        name = ob1.name,
                        name_en = ob1.name_en,
                        name_khongdau = ob1.name_khongdau,
                        ngayxem = ob5.NgayXem,
                        IsTheoDoi = (ob4 != null),
                        ThanhPho = (ob1.ThanhPho ?? "Không có"),
                        ngaytao = ob1.ngaytao,
                        giaban = ob1.giaban,
                        nguoitao = ob1.nguoitao,
                        description = ob1.description,
                        description_khongdau = ob1.description_khongdau,
                        soluong_daban = ob1.soluong_daban,
                        LuotTruyCap = (ob1.LuotTruyCap ?? 0),
                        TenMenu = ob2 != null ? ob2.name : "",
                        TenMenu2 = ob3 != null
                            ? "<span class='pl-3 mif-chevron-right'></span>" + ob3.name
                            : "",
                        phanloai = ob1.phanloai
                    };
            }
            else
            {
                list_all =
                    from ob1 in query
                    join ob2 in db.DanhMuc_tbs
                        on ob1.id_DanhMuc equals ob2.id.ToString() into g1
                    from ob2 in g1.DefaultIfEmpty()

                    join ob3 in db.DanhMuc_tbs
                        on ob1.id_DanhMucCap2 equals ob3.id.ToString() into g2
                    from ob3 in g2.DefaultIfEmpty()

                    join ob4 in db.TheoDoiTin_dbs.Where(t => t.taikhoan == taiKhoanHienTai)
                        on ob1.id equals ob4.idBaiviet into gj
                    from ob4 in gj.DefaultIfEmpty()

                    join ob5 in db.TinDaXem_tbs
                        on ob1.id equals ob5.idBaiViet
                    where ob5.TaiKhoan == taiKhoanHienTai

                    select new TinItem
                    {
                        id = ob1.id,
                        image = ob1.image,
                        name = ob1.name,
                        name_en = ob1.name_en,
                        name_khongdau = "",
                        ngayxem = ob5.NgayXem,
                        IsTheoDoi = (ob4 != null),
                        ThanhPho = (ob1.ThanhPho ?? "Không có"),
                        ngaytao = ob1.ngaytao,
                        giaban = ob1.giaban,
                        nguoitao = ob1.nguoitao,
                        description = ob1.description,
                        description_khongdau = "",
                        soluong_daban = ob1.soluong_daban,
                        LuotTruyCap = (ob1.LuotTruyCap ?? 0),
                        TenMenu = ob2 != null ? ob2.name : "",
                        TenMenu2 = ob3 != null
                            ? "<span class='pl-3 mif-chevron-right'></span>" + ob3.name
                            : "",
                        phanloai = ob1.phanloai
                    };
            }

            list_all.OrderByDescending(p => p.ngayxem).AsQueryable();
        }
        else if (!string.IsNullOrEmpty(Is_tin_theo_doi))
        {
            string taiKhoanHienTai = Convert.ToString(ViewState["taikhoan"]) ?? "";

            // Chỉ lấy các tin được theo dõi
            var query = visibleBase.Where(p =>
                db.TheoDoiTin_dbs.Any(t =>
                    t.idBaiviet == p.id &&
                    t.taikhoan == taiKhoanHienTai
                )
                && !hideList.Contains(p.id)
            );
            // Lọc thêm danh mục từ dropdown nếu có
            if (danhMucIds_Search != null)
            {
                query = query.Where(p =>
                    danhMucIds_Search.Contains(p.id_DanhMuc)
                    || danhMucIds_Search.Contains(p.id_DanhMucCap2));
            }

            if (hasSearchColumns)
            {
                list_all = (from ob1 in query
                            join ob2 in db.DanhMuc_tbs on ob1.id_DanhMuc equals ob2.id.ToString() into g1
                            from ob2 in g1.DefaultIfEmpty()
                            join ob3 in db.DanhMuc_tbs on ob1.id_DanhMucCap2 equals ob3.id.ToString() into g2
                            from ob3 in g2.DefaultIfEmpty()
                            join ob4 in db.TheoDoiTin_dbs.Where(t => t.taikhoan == taiKhoanHienTai)
                                  on ob1.id equals ob4.idBaiviet
                                  into gj
                            from ob4 in gj.DefaultIfEmpty()
                            select new TinItem
                            {
                                id = ob1.id,
                                image = ob1.image,
                                name = ob1.name,
                                name_en = ob1.name_en,
                                name_khongdau = ob1.name_khongdau,
                                IsTheoDoi = (ob4 != null),
                                ThanhPho = (ob1.ThanhPho ?? "Không có"),
                                ngaytao = ob1.ngaytao,
                                giaban = ob1.giaban,
                                nguoitao = ob1.nguoitao,
                                description = ob1.description,
                                description_khongdau = ob1.description_khongdau,
                                soluong_daban = ob1.soluong_daban,
                                LuotTruyCap = (ob1.LuotTruyCap ?? 0),
                                TenMenu = ob2 != null ? ob2.name : "",
                                TenMenu2 = ob3 != null
                                    ? "<span class='pl-3 mif-chevron-right'></span>" + ob3.name
                                    : "",
                                phanloai = ob1.phanloai
                            }).AsQueryable();
            }
            else
            {
                list_all = (from ob1 in query
                            join ob2 in db.DanhMuc_tbs on ob1.id_DanhMuc equals ob2.id.ToString() into g1
                            from ob2 in g1.DefaultIfEmpty()
                            join ob3 in db.DanhMuc_tbs on ob1.id_DanhMucCap2 equals ob3.id.ToString() into g2
                            from ob3 in g2.DefaultIfEmpty()
                            join ob4 in db.TheoDoiTin_dbs.Where(t => t.taikhoan == taiKhoanHienTai)
                                  on ob1.id equals ob4.idBaiviet
                                  into gj
                            from ob4 in gj.DefaultIfEmpty()
                            select new TinItem
                            {
                                id = ob1.id,
                                image = ob1.image,
                                name = ob1.name,
                                name_en = ob1.name_en,
                                name_khongdau = "",
                                IsTheoDoi = (ob4 != null),
                                ThanhPho = (ob1.ThanhPho ?? "Không có"),
                                ngaytao = ob1.ngaytao,
                                giaban = ob1.giaban,
                                nguoitao = ob1.nguoitao,
                                description = ob1.description,
                                description_khongdau = "",
                                soluong_daban = ob1.soluong_daban,
                                LuotTruyCap = (ob1.LuotTruyCap ?? 0),
                                TenMenu = ob2 != null ? ob2.name : "",
                                TenMenu2 = ob3 != null
                                    ? "<span class='pl-3 mif-chevron-right'></span>" + ob3.name
                                    : "",
                                phanloai = ob1.phanloai
                            }).AsQueryable();
            }
        }
        // ✅ Nếu không có idmn => không lọc (trang chủ)
        else if (danhMucIds == null)
        {
            string taiKhoanHienTai = Convert.ToString(ViewState["taikhoan"]) ?? "";

            var query = visibleBase.Where(p =>
                !hideList.Contains(p.id));

            // ✅ lọc danh mục từ dropdown
            if (danhMucIds_Search != null)
            {
                query = query.Where(p =>
                    danhMucIds_Search.Contains(p.id_DanhMuc)
                    || danhMucIds_Search.Contains(p.id_DanhMucCap2));
            }

            if (hasSearchColumns)
            {
                list_all = (from ob1 in query
                            join ob2 in db.TheoDoiTin_dbs.Where(t => t.taikhoan == taiKhoanHienTai)
                                on ob1.id equals ob2.idBaiviet
                                into gj
                            from ob2 in gj.DefaultIfEmpty()
                            select new TinItem
                            {
                                id = ob1.id,
                                image = ob1.image,
                                name = ob1.name,
                                name_en = ob1.name_en,
                                name_khongdau = ob1.name_khongdau,
                                IsTheoDoi = (ob2 != null),
                                ThanhPho = (ob1.ThanhPho ?? "Không có"),
                                ngaytao = ob1.ngaytao,
                                giaban = ob1.giaban,
                                nguoitao = ob1.nguoitao,
                                description = ob1.description,
                                description_khongdau = ob1.description_khongdau,
                                soluong_daban = ob1.soluong_daban,
                                LuotTruyCap = (ob1.LuotTruyCap ?? 0),
                                TenMenu = "",
                                TenMenu2 = "",
                                phanloai = ob1.phanloai
                            }).AsQueryable();
            }
            else
            {
                list_all = (from ob1 in query
                            join ob2 in db.TheoDoiTin_dbs.Where(t => t.taikhoan == taiKhoanHienTai)
                                on ob1.id equals ob2.idBaiviet
                                into gj
                            from ob2 in gj.DefaultIfEmpty()
                            select new TinItem
                            {
                                id = ob1.id,
                                image = ob1.image,
                                name = ob1.name,
                                name_en = ob1.name_en,
                                name_khongdau = "",
                                IsTheoDoi = (ob2 != null),
                                ThanhPho = (ob1.ThanhPho ?? "Không có"),
                                ngaytao = ob1.ngaytao,
                                giaban = ob1.giaban,
                                nguoitao = ob1.nguoitao,
                                description = ob1.description,
                                description_khongdau = "",
                                soluong_daban = ob1.soluong_daban,
                                LuotTruyCap = (ob1.LuotTruyCap ?? 0),
                                TenMenu = "",
                                TenMenu2 = "",
                                phanloai = ob1.phanloai
                            }).AsQueryable();
            }
        }
        else
        {
            string taiKhoanHienTai = Convert.ToString(ViewState["taikhoan"]) ?? "";

            var query = visibleBase.Where(p =>
                danhMucIds.Contains(p.id_DanhMuc) &&
                !hideList.Contains(p.id));

            // ✅ lọc thêm danh mục từ dropdown
            if (danhMucIds_Search != null)
            {
                query = query.Where(p =>
                    danhMucIds_Search.Contains(p.id_DanhMuc)
                    || danhMucIds_Search.Contains(p.id_DanhMucCap2));
            }

            if (hasSearchColumns)
            {
                list_all =
                    (from ob1 in query
                     join ob2 in db.DanhMuc_tbs on ob1.id_DanhMuc equals ob2.id.ToString() into g1
                     from ob2 in g1.DefaultIfEmpty()
                     join ob3 in db.DanhMuc_tbs on ob1.id_DanhMucCap2 equals ob3.id.ToString() into g2
                     from ob3 in g2.DefaultIfEmpty()
                     join ob4 in db.TheoDoiTin_dbs.Where(t => t.taikhoan == taiKhoanHienTai)
                               on ob1.id equals ob4.idBaiviet
                               into gj
                     from ob4 in gj.DefaultIfEmpty()
                     select new TinItem
                     {
                         id = ob1.id,
                         image = ob1.image,
                         name = ob1.name,
                         name_en = ob1.name_en,
                         name_khongdau = ob1.name_khongdau,
                         IsTheoDoi = (ob4 != null),
                         ThanhPho = (ob1.ThanhPho ?? "Không có"),
                         ngaytao = ob1.ngaytao,
                         giaban = ob1.giaban,
                         nguoitao = ob1.nguoitao,
                         description = ob1.description,
                         description_khongdau = ob1.description_khongdau,
                         soluong_daban = ob1.soluong_daban,
                         LuotTruyCap = (ob1.LuotTruyCap ?? 0),
                         TenMenu = ob2 != null ? ob2.name : "",
                         TenMenu2 = ob3 != null
                             ? "<span class='pl-3 mif-chevron-right'></span>" + ob3.name
                             : "",
                         phanloai = ob1.phanloai
                     }).AsQueryable();
            }
            else
            {
                list_all =
                    (from ob1 in query
                     join ob2 in db.DanhMuc_tbs on ob1.id_DanhMuc equals ob2.id.ToString() into g1
                     from ob2 in g1.DefaultIfEmpty()
                     join ob3 in db.DanhMuc_tbs on ob1.id_DanhMucCap2 equals ob3.id.ToString() into g2
                     from ob3 in g2.DefaultIfEmpty()
                     join ob4 in db.TheoDoiTin_dbs.Where(t => t.taikhoan == taiKhoanHienTai)
                               on ob1.id equals ob4.idBaiviet
                               into gj
                     from ob4 in gj.DefaultIfEmpty()
                     select new TinItem
                     {
                         id = ob1.id,
                         image = ob1.image,
                         name = ob1.name,
                         name_en = ob1.name_en,
                         name_khongdau = "",
                         IsTheoDoi = (ob4 != null),
                         ThanhPho = (ob1.ThanhPho ?? "Không có"),
                         ngaytao = ob1.ngaytao,
                         giaban = ob1.giaban,
                         nguoitao = ob1.nguoitao,
                         description = ob1.description,
                         description_khongdau = "",
                         soluong_daban = ob1.soluong_daban,
                         LuotTruyCap = (ob1.LuotTruyCap ?? 0),
                         TenMenu = ob2 != null ? ob2.name : "",
                         TenMenu2 = ob3 != null
                             ? "<span class='pl-3 mif-chevron-right'></span>" + ob3.name
                             : "",
                         phanloai = ob1.phanloai
                     }).AsQueryable();
            }
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            if (hasSearchColumns)
            {
                BaiVietSearchSchema_cl.BackfillMissing(db, 2000);

                if (!string.IsNullOrEmpty(keywordSlug) || !string.IsNullOrEmpty(keywordNormalized))
                {
                    list_all = list_all.Where(p =>
                        (p.name != null && p.name.Contains(keyword)) ||
                        (p.description != null && p.description.Contains(keyword)) ||
                        (p.name_en != null && p.name_en.Contains(keywordSlug)) ||
                        (p.name_khongdau != null && p.name_khongdau.Contains(keywordNormalized)) ||
                        (p.description_khongdau != null && p.description_khongdau.Contains(keywordNormalized)));
                }
                else
                {
                    list_all = list_all.Where(p =>
                        (p.name != null && p.name.Contains(keyword)) ||
                        (p.description != null && p.description.Contains(keyword)));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(keywordSlug))
                {
                    list_all = list_all.Where(p =>
                        (p.name != null && p.name.Contains(keyword)) ||
                        (p.description != null && p.description.Contains(keyword)) ||
                        (p.name_en != null && p.name_en.Contains(keywordSlug)));
                }
                else
                {
                    list_all = list_all.Where(p =>
                        (p.name != null && p.name.Contains(keyword)) ||
                        (p.description != null && p.description.Contains(keyword)));
                }
            }
        }

        if (!string.IsNullOrEmpty(thanhPho))
        {
            if (thanhPhoMap == null)
                thanhPhoMap = db.ThanhPhos.ToDictionary(p => p.id, p => p.Ten);
            string thanhPhoTen = TinhThanhDisplay_cl.ResolveNameFromId(thanhPho, thanhPhoMap);
            list_all = list_all.Where(p => p.ThanhPho == thanhPho || p.ThanhPho == thanhPhoTen);
        }


        list_all = list_all.OrderByDescending(p => p.ngaytao);

        int show = ResolveRequestedPageSize();
        if (show <= 0) show = DefaultPageSize;

        int current_page = ResolveRequestedPage();

        List<TinItem> bdsSearchItems = new List<TinItem>();
        bool shouldMergeUnifiedBdsSearch = ShouldMergeUnifiedBdsSearch(db, keyword, selectedDanhMuc, thanhPho);
        if (shouldMergeUnifiedBdsSearch)
            bdsSearchItems = BuildUnifiedBdsSearchTinItems(db, keyword, selectedDanhMuc, thanhPho, current_page * show);

        bool shouldMixLinkedBds = !shouldMergeUnifiedBdsSearch && ShouldMixLinkedBdsIntoHomeFeed();
        List<TinItem> linkedBdsItems = shouldMixLinkedBds ? BuildLinkedBdsTinItems(db, current_page * show) : new List<TinItem>();
        int linkedBdsCount = shouldMixLinkedBds ? LinkedFeedStore_cl.GetActiveCount(db) : 0;

        int total_record = list_all.Count() + linkedBdsCount + (shouldMergeUnifiedBdsSearch ? bdsSearchItems.Count : 0);
        ResultCount = total_record;
        int total_page = number_of_page_class.return_total_page(total_record, show);
        if (total_page <= 0) total_page = 1;

        if (reset)
        {
            current_page = ResolveRequestedPage();
            if (current_page < 1) current_page = 1;
        }

        if (current_page > total_page)
            current_page = total_page;
        if (current_page < 1)
            current_page = 1;
        ViewState[VS_PageKey] = current_page.ToString();

        List<TinItem> list_split;
        if (shouldMergeUnifiedBdsSearch)
        {
            int neededCombined = current_page * show;
            List<TinItem> primaryItems = list_all.Take(neededCombined).ToList();
            List<TinItem> combinedItems = InterleaveTinItems(primaryItems, bdsSearchItems.Take(neededCombined).ToList(), UnifiedSearchBdsStride, neededCombined);
            list_split = combinedItems.Skip((current_page - 1) * show).Take(show).ToList();
        }
        else if (shouldMixLinkedBds)
        {
            int neededCombined = current_page * show;
            List<TinItem> primaryItems = list_all.Take(neededCombined).ToList();
            List<TinItem> combinedItems = InterleaveTinItems(primaryItems, linkedBdsItems, HomeFeedLinkedBdsStride, neededCombined);
            list_split = combinedItems.Skip((current_page - 1) * show).Take(show).ToList();
        }
        else
        {
            list_split = list_all.Skip((current_page - 1) * show).Take(show).ToList();
        }

        list_split = DeduplicateTinItemsForRender(list_split);

        if (thanhPhoMap == null)
            thanhPhoMap = db.ThanhPhos.ToDictionary(p => p.id, p => p.Ten);
        foreach (var it in list_split)
        {
            string name = TinhThanhDisplay_cl.ResolveNameFromId(it.ThanhPho, thanhPhoMap);
            it.ThanhPhoDisplay = TinhThanhDisplay_cl.Format(name);
        }

        NormalizeTinItemsForBind(list_split);

        SearchSummary = BuildSearchSummary(keyword, selectedDanhMuc, thanhPho, db, thanhPhoMap);

        RepeaterTin.DataSource = list_split;
        RepeaterTin.DataBind();
        BindPager(current_page, total_page, show);
        if (but_xemthem != null) but_xemthem.Visible = false;

        up_all.Update();
    }

    private bool TryBindCachedGuestHomepageFeed(bool reset)
    {
        if (!IsGuestHomepageInitialFeedRequest(reset))
            return false;

        GuestLatestFeedCachePayload payload = Helper_cl.RuntimeCacheGetOrAdd<GuestLatestFeedCachePayload>(
            "home:latest-feed:guest:v2",
            GuestLatestFeedCacheSeconds,
            BuildGuestLatestFeedCachePayload);

        if (payload == null || payload.Items == null)
            return false;

        List<TinItem> loaded = payload.Items
            .Select(CloneTinItem)
            .ToList();

        Dictionary<string, string> locationMap = payload.LocationMap ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (TinItem item in loaded)
        {
            string name;
            if (locationMap.TryGetValue(item.ThanhPho ?? "", out name))
                item.ThanhPhoDisplay = TinhThanhDisplay_cl.Format(name);
        }

        int show = ResolveRequestedPageSize();
        int currentPage = ResolveRequestedPage();
        if (currentPage < 1) currentPage = 1;
        int totalPages = number_of_page_class.return_total_page(payload.TotalCount, show);
        if (totalPages <= 0) totalPages = 1;
        if (currentPage > totalPages) currentPage = totalPages;

        var pageItems = loaded.Skip((currentPage - 1) * show).Take(show).ToList();
        pageItems = DeduplicateTinItemsForRender(pageItems);
        NormalizeTinItemsForBind(pageItems);

        ViewState[VS_PageKey] = currentPage.ToString();
        ResultCount = payload.TotalCount;
        SearchSummary = "";
        RepeaterTin.DataSource = pageItems;
        RepeaterTin.DataBind();
        BindPager(currentPage, totalPages, show);
        if (but_xemthem != null) but_xemthem.Visible = false;
        up_all.Update();
        return true;
    }

    private bool IsGuestHomepageInitialFeedRequest(bool reset)
    {
        if (!reset)
            return false;
        if (ResolveRequestedPage() != 1)
            return false;
        if (ResolveRequestedPageSize() != DefaultPageSize)
            return false;
        if (!string.IsNullOrWhiteSpace(Idmn) || !string.IsNullOrWhiteSpace(tin_theo_doi) || !string.IsNullOrWhiteSpace(lich_su_xem_tin))
            return false;
        if (!string.IsNullOrWhiteSpace(Convert.ToString(ViewState["taikhoan"])))
            return false;
        if (!string.IsNullOrWhiteSpace(txt_Search != null ? txt_Search.Text : ""))
            return false;
        if (!string.IsNullOrWhiteSpace(ddl_Category != null ? ddl_Category.SelectedValue : ""))
            return false;
        if (!string.IsNullOrWhiteSpace(ddl_Location != null ? ddl_Location.SelectedValue : ""))
            return false;
        if (hfProvinceCode != null && !string.IsNullOrWhiteSpace(hfProvinceCode.Value))
            return false;
        if (hfDistrictCode != null && !string.IsNullOrWhiteSpace(hfDistrictCode.Value))
            return false;
        if (hfWardCode != null && !string.IsNullOrWhiteSpace(hfWardCode.Value))
            return false;
        if (!string.IsNullOrWhiteSpace(Session["HiddenPostIds"] as string))
            return false;
        return true;
    }

    private GuestLatestFeedCachePayload BuildGuestLatestFeedCachePayload()
    {
        return SqlTransientGuard_cl.Execute(() =>
        {
            using (dbDataContext db = new dbDataContext())
            {
                var visible = AccountVisibility_cl.FilterVisibleTradePosts(db, db.BaiViet_tbs);
                int totalCount = visible.Count();
                int linkedCount = 0;

                List<TinItem> items = visible
                    .OrderByDescending(p => p.ngaytao)
                    .Select(p => new TinItem
                    {
                        id = p.id,
                        image = p.image,
                        name = p.name,
                        name_en = p.name_en,
                        name_khongdau = p.name_khongdau,
                        IsTheoDoi = false,
                        ThanhPho = (p.ThanhPho ?? "Không có"),
                        ngaytao = p.ngaytao,
                        giaban = p.giaban,
                        nguoitao = p.nguoitao,
                        description = p.description,
                        description_khongdau = p.description_khongdau,
                        soluong_daban = p.soluong_daban,
                        LuotTruyCap = (p.LuotTruyCap ?? 0),
                        TenMenu = "",
                        TenMenu2 = "",
                        phanloai = p.phanloai
                    })
                    .ToList();

                if (ShouldMixLinkedBdsIntoHomeFeed())
                {
                    List<TinItem> linkedItems = BuildLinkedBdsTinItems(db, DefaultPageSize);
                    linkedCount = LinkedFeedStore_cl.GetActiveCount(db);
                    items = InterleaveTinItems(items.Take(DefaultPageSize).ToList(), linkedItems, HomeFeedLinkedBdsStride, DefaultPageSize);
                }
                else
                {
                    items = items.Take(DefaultPageSize).ToList();
                }
                items = DeduplicateTinItemsForRender(items);

                Dictionary<string, string> locationMap = db.ThanhPhos
                    .ToDictionary(p => p.id.ToString(), p => p.Ten, StringComparer.OrdinalIgnoreCase);

                return new GuestLatestFeedCachePayload
                {
                    Items = items,
                    TotalCount = totalCount + linkedCount,
                    LocationMap = locationMap
                };
            }
        });
    }

    private bool ShouldMixLinkedBdsIntoHomeFeed()
    {
        if (!string.IsNullOrWhiteSpace(Idmn) || !string.IsNullOrWhiteSpace(tin_theo_doi) || !string.IsNullOrWhiteSpace(lich_su_xem_tin))
            return false;
        if (!string.Equals(ResolveSearchType(), "all", StringComparison.OrdinalIgnoreCase))
            return false;
        if (!string.IsNullOrWhiteSpace(txt_Search != null ? txt_Search.Text : ""))
            return false;
        if (!string.IsNullOrWhiteSpace(ddl_Category != null ? ddl_Category.SelectedValue : ""))
            return false;
        if (!string.IsNullOrWhiteSpace(ddl_Location != null ? ddl_Location.SelectedValue : ""))
            return false;
        if (hfProvinceCode != null && !string.IsNullOrWhiteSpace(hfProvinceCode.Value))
            return false;
        if (hfDistrictCode != null && !string.IsNullOrWhiteSpace(hfDistrictCode.Value))
            return false;
        if (hfWardCode != null && !string.IsNullOrWhiteSpace(hfWardCode.Value))
            return false;
        if (!string.IsNullOrWhiteSpace(Session["HiddenPostIds"] as string))
            return false;
        return true;
    }

    private bool ShouldMergeUnifiedBdsSearch(dbDataContext db, string keyword, string categoryId, string provinceId)
    {
        string path = (Request == null ? "" : (Request.Path ?? "")).Trim().ToLowerInvariant();
        if (path == "/tim-kiem" || path.EndsWith("/home/tim-kiem.aspx", StringComparison.OrdinalIgnoreCase))
            return true;

        if (!string.IsNullOrWhiteSpace(keyword))
            return true;

        if (!string.IsNullOrWhiteSpace(provinceId))
            return true;

        return IsBatDongSanCategory(db, categoryId);
    }

    private bool IsBatDongSanCategory(dbDataContext db, string categoryId)
    {
        if (db == null || string.IsNullOrWhiteSpace(categoryId))
            return false;

        string catId = categoryId.Trim();
        DanhMuc_tb category = db.DanhMuc_tbs.FirstOrDefault(p => p.id.ToString() == catId);
        if (category == null)
            return false;

        string name = ((category.name ?? "") + " " + (category.name_en ?? "")).Trim().ToLowerInvariant();
        return name.Contains("bat dong san")
            || name.Contains("bất động sản")
            || name.Contains("bat-dong-san")
            || name.Contains("nha dat")
            || name.Contains("nhà đất")
            || name.Contains("nha-dat");
    }

    private List<TinItem> BuildLinkedBdsTinItems(dbDataContext db, int take)
    {
        if (db == null)
            return new List<TinItem>();

        int safeTake = take <= 0 ? DefaultPageSize : (take > 500 ? 500 : take);
        LinkedFeedStore_cl.EnsureSchema(db);
        return LinkedFeedStore_cl.GetActiveForSearch(db, safeTake)
            .Select(MapLinkedBdsToTinItem)
            .Where(x => x != null)
            .ToList();
    }

    private TinItem MapLinkedBdsToTinItem(LinkedFeedStore_cl.LinkedPost row)
    {
        if (row == null || string.IsNullOrWhiteSpace(row.Title))
            return null;

        string priceText = string.IsNullOrWhiteSpace(row.PriceText) ? "Liên hệ" : row.PriceText.Trim();
        string areaText = string.IsNullOrWhiteSpace(row.AreaText) ? "" : (" · " + row.AreaText.Trim());
        string province = (row.Province ?? "").Trim();
        string district = (row.District ?? "").Trim();
        string location = string.IsNullOrWhiteSpace(district) ? province : (district + ", " + province);
        string sourceLabel = BatDongSanService_cl.ResolveLinkedSourceLabel(row.Source);

        return new TinItem
        {
            id = -row.Id,
            image = BatDongSanService_cl.BuildLinkedImageProxyUrl(row.Id, 0),
            name = row.Title.Trim(),
            name_en = BatDongSanService_cl.Slugify(row.Title),
            name_khongdau = "",
            ThanhPho = province,
            ThanhPhoDisplay = location,
            ngaytao = row.PublishedAt,
            giaban = null,
            nguoitao = "bds_linked_feed",
            description = string.IsNullOrWhiteSpace(row.Summary) ? ((sourceLabel + " · " + priceText + areaText).Trim()) : row.Summary.Trim(),
            description_khongdau = "",
            soluong_daban = null,
            LuotTruyCap = 0,
            TenMenu = "BĐS liên kết",
            TenMenu2 = string.IsNullOrWhiteSpace(sourceLabel) ? "" : ("<span class='pl-3 mif-chevron-right'></span>" + sourceLabel),
            IsTheoDoi = false,
            phanloai = "bds_linked",
            DetailUrl = "/bat-dong-san/chi-tiet.aspx?linkedId=" + row.Id.ToString(),
            DisplayPriceText = priceText,
            IsLinkedBds = true,
            SourceLabel = sourceLabel
        };
    }

    private List<TinItem> BuildUnifiedBdsSearchTinItems(dbDataContext db, string keyword, string categoryId, string provinceId, int take)
    {
        if (db == null)
            return new List<TinItem>();

        string provinceSlug = ResolveProvinceSlug(db, provinceId);
        bool categoryIsBds = IsBatDongSanCategory(db, categoryId);
        bool hasInput = !string.IsNullOrWhiteSpace(keyword) || !string.IsNullOrWhiteSpace(provinceSlug) || categoryIsBds;
        if (!hasInput)
            return new List<TinItem>();

        var query = new BatDongSanService_cl.ListingQuery
        {
            Keyword = (keyword ?? "").Trim(),
            Province = provinceSlug,
            Sort = "relevance"
        };

        return BatDongSanSearch_cl.QueryUnifiedListings(query)
            .Take(take <= 0 ? DefaultPageSize : take)
            .Select(MapBdsListingToTinItem)
            .Where(x => x != null)
            .ToList();
    }

    private string ResolveProvinceSlug(dbDataContext db, string provinceId)
    {
        string raw = (provinceId ?? "").Trim();
        if (string.IsNullOrWhiteSpace(raw))
            return "";

        ThanhPho province = db.ThanhPhos.FirstOrDefault(p => p.id.ToString() == raw);
        if (province == null)
            return "";

        return BatDongSanService_cl.Slugify(province.Ten);
    }

    private TinItem MapBdsListingToTinItem(BatDongSanService_cl.ListingItem item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.Title))
            return null;

        string location = "";
        if (!string.IsNullOrWhiteSpace(item.District) && !string.IsNullOrWhiteSpace(item.Province))
            location = item.District.Trim() + ", " + item.Province.Trim();
        else if (!string.IsNullOrWhiteSpace(item.Province))
            location = item.Province.Trim();
        else if (!string.IsNullOrWhiteSpace(item.AddressText))
            location = item.AddressText.Trim();

        string sourceLabel = item.IsLinked
            ? (item.LinkedSource ?? "Nguồn liên kết")
            : "Tin bất động sản";

        return new TinItem
        {
            id = item.IsLinked ? -item.Id : item.Id,
            image = string.IsNullOrWhiteSpace(item.ThumbnailUrl) ? BatDongSanService_cl.DefaultBdsFallbackImage : item.ThumbnailUrl,
            name = item.Title.Trim(),
            name_en = item.Slug,
            name_khongdau = "",
            ThanhPho = item.Province ?? "",
            ThanhPhoDisplay = location,
            ngaytao = item.IsLinked ? (DateTime?)null : AhaTime_cl.Now,
            giaban = null,
            nguoitao = item.IsLinked ? "bds_linked_feed" : "bds_native_listing",
            description = string.IsNullOrWhiteSpace(item.Description) ? BatDongSanService_cl.BuildListingMeta(item) : item.Description.Trim(),
            description_khongdau = "",
            soluong_daban = null,
            LuotTruyCap = 0,
            TenMenu = "Bất động sản",
            TenMenu2 = string.IsNullOrWhiteSpace(sourceLabel) ? "" : ("<span class='pl-3 mif-chevron-right'></span>" + sourceLabel),
            IsTheoDoi = false,
            phanloai = "bds_search",
            DetailUrl = BatDongSanService_cl.BuildListingUrl(item),
            DisplayPriceText = string.IsNullOrWhiteSpace(item.PriceText) ? "Liên hệ" : item.PriceText.Trim(),
            IsLinkedBds = true,
            SourceLabel = sourceLabel
        };
    }

    private static List<TinItem> InterleaveTinItems(List<TinItem> primary, List<TinItem> secondary, int stride, int maxTake)
    {
        var result = new List<TinItem>();
        primary = primary ?? new List<TinItem>();
        secondary = secondary ?? new List<TinItem>();
        if (stride <= 0) stride = 5;
        if (maxTake <= 0) maxTake = int.MaxValue;

        int i = 0;
        int j = 0;
        int sinceSecondary = 0;

        while (result.Count < maxTake && (i < primary.Count || j < secondary.Count))
        {
            if (i < primary.Count)
            {
                result.Add(primary[i++]);
                sinceSecondary++;
                if (result.Count >= maxTake)
                    break;
            }

            if (sinceSecondary >= stride && j < secondary.Count)
            {
                result.Add(secondary[j++]);
                sinceSecondary = 0;
            }
            else if (i >= primary.Count && j < secondary.Count)
            {
                result.Add(secondary[j++]);
                sinceSecondary = 0;
            }
        }

        return result;
    }

    private static TinItem CloneTinItem(TinItem item)
    {
        if (item == null)
            return null;

        return new TinItem
        {
            id = item.id,
            image = item.image,
            name = item.name,
            name_en = item.name_en,
            name_khongdau = item.name_khongdau,
            ThanhPho = item.ThanhPho,
            ThanhPhoDisplay = item.ThanhPhoDisplay,
            ngaytao = item.ngaytao,
            ngayxem = item.ngayxem,
            giaban = item.giaban,
            nguoitao = item.nguoitao,
            description = item.description,
            description_khongdau = item.description_khongdau,
            soluong_daban = item.soluong_daban,
            LuotTruyCap = item.LuotTruyCap,
            TenMenu = item.TenMenu,
            TenMenu2 = item.TenMenu2,
            IsTheoDoi = item.IsTheoDoi,
            phanloai = item.phanloai,
            DetailUrl = item.DetailUrl,
            DisplayPriceText = item.DisplayPriceText,
            IsLinkedBds = item.IsLinkedBds,
            SourceLabel = item.SourceLabel
        };
    }

    private static List<TinItem> DeduplicateTinItemsForRender(IEnumerable<TinItem> source)
    {
        List<TinItem> result = new List<TinItem>();
        if (source == null)
            return result;

        HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (TinItem item in source)
        {
            if (item == null)
                continue;

            string key = BuildTinItemRenderKey(item);
            if (!seen.Add(key))
                continue;

            result.Add(item);
        }

        return result;
    }

    private static string BuildTinItemRenderKey(TinItem item)
    {
        if (item == null)
            return "";

        if (item.IsLinkedBds)
        {
            string linkedUrl = (item.DetailUrl ?? "").Trim().ToLowerInvariant();
            string linkedName = ((item.name_en ?? item.name) ?? "").Trim().ToLowerInvariant();
            return "linked|" + linkedUrl + "|" + linkedName;
        }

        string owner = (item.nguoitao ?? "").Trim().ToLowerInvariant();
        string postType = (item.phanloai ?? "").Trim().ToLowerInvariant();
        string slug = (item.name_en ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(slug))
            slug = (item.name ?? "").Trim().ToLowerInvariant();
        string price = item.giaban.HasValue ? item.giaban.Value.ToString("0.##") : "";
        string image = (item.image ?? "").Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(owner) && string.IsNullOrWhiteSpace(slug))
            return "id|" + item.id.ToString();

        return string.Join("|", "post", owner, postType, slug, price, image);
    }

    private void NormalizeTinItemsForBind(List<TinItem> items)
    {
        if (items == null)
            return;

        foreach (TinItem item in items)
        {
            if (item == null)
                continue;

            if (string.IsNullOrWhiteSpace(item.DetailUrl))
                item.DetailUrl = BuildDefaultTinDetailUrl(item);

            if (string.IsNullOrWhiteSpace(item.DisplayPriceText))
                item.DisplayPriceText = item.giaban.HasValue ? item.giaban.Value.ToString("#,##0") + " đ" : "Liên hệ";

            if (string.IsNullOrWhiteSpace(item.ThanhPhoDisplay))
                item.ThanhPhoDisplay = TinhThanhDisplay_cl.Format(item.ThanhPho);
        }
    }

    private string BuildDefaultTinDetailUrl(TinItem item)
    {
        if (item == null)
            return "/";

        string slug = (item.name_en ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(slug))
        {
            slug = (item.name ?? "").Trim().ToLowerInvariant();
            slug = Regex.Replace(slug, @"[^a-z0-9\-]+", "-").Trim('-');
        }
        if (string.IsNullOrEmpty(slug))
            slug = "san-pham";

        return "/" + slug + "-" + item.id.ToString() + ".html";
    }

    public void AddItemtoSession(object sender, EventArgs e)
    {
        var btn = (LinkButton)sender;
        string id = btn.CommandArgument;
        string key = "HiddenPostIds";
        if (Session[key] == null || string.IsNullOrWhiteSpace(Session[key].ToString()))
        {
            Session[key] = id;
            return;
        }
        var list = Session[key].ToString()
                               .Split(',')
                               .Select(x => x.Trim())
                               .ToList();
        if (!list.Contains(id))
        {
            list.Add(id);
            Session[key] = string.Join(",", list);
        }

        using (dbDataContext db = new dbDataContext())
            HienThiTinMoi(db, reset: true);

        Helper_Tabler_cl.ShowToast(this.Page, "Xử lý thành công", null, true, 3000, "Thông báo");
    }

    public void btnHeart_Click(object sender, EventArgs e)
    {
        var btn = (LinkButton)sender;
        string productId = btn.CommandArgument;
        string tk = (ViewState["taikhoan"] ?? "").ToString();

        if (string.IsNullOrEmpty(tk))
        {
            HttpContext.Current.Response.Redirect("/dang-nhap");
            return;
        }

        int idBaiViet = int.Parse(productId);

        using (dbDataContext db = new dbDataContext())
        {
            var existed = db.TheoDoiTin_dbs.FirstOrDefault(x => x.idBaiviet == idBaiViet && x.taikhoan == tk);

            if (existed != null)
            {
                db.TheoDoiTin_dbs.DeleteOnSubmit(existed);
                Helper_Tabler_cl.ShowToast(this.Page, "Đã hủy theo dõi tin này", null, true, 3000, "Thông báo");
            }
            else
            {
                TheoDoiTin_db theoDoiTin = new TheoDoiTin_db
                {
                    idBaiviet = idBaiViet,
                    taikhoan = tk,
                    NgayLuu = DateTime.Now
                };
                db.TheoDoiTin_dbs.InsertOnSubmit(theoDoiTin);
                Helper_Tabler_cl.ShowToast(this.Page, "Đã theo dõi tin này", null, true, 3000, "Thông báo");
            }
            db.SubmitChanges();
            HienThiTinMoi(db, reset: true);
        }
    }


    public void Loc_Click(object sender, EventArgs e)
    {
        string keyword = (txt_Search != null ? txt_Search.Text : "").Trim();
        string danhMuc = (ddl_Category != null ? ddl_Category.SelectedValue : "");
        string thanhPho = (ddl_Location != null ? ddl_Location.SelectedValue : "");
        string province = (hfProvinceCode != null ? hfProvinceCode.Value : "");
        string district = (hfDistrictCode != null ? hfDistrictCode.Value : "");
        string ward = (hfWardCode != null ? hfWardCode.Value : "");
        string type = ResolveSearchType();

        var qs = HttpUtility.ParseQueryString(string.Empty);
        if (!string.IsNullOrWhiteSpace(keyword)) qs["q"] = keyword;
        if (!string.IsNullOrWhiteSpace(danhMuc)) qs["cat"] = danhMuc;
        if (!string.IsNullOrWhiteSpace(thanhPho)) qs["loc"] = thanhPho;
        if (string.IsNullOrWhiteSpace(province)) province = thanhPho;
        if (!string.IsNullOrWhiteSpace(province)) qs["province"] = province;
        if (!string.IsNullOrWhiteSpace(district)) qs["district"] = district;
        if (!string.IsNullOrWhiteSpace(ward)) qs["ward"] = ward;
        if (!string.IsNullOrWhiteSpace(type)) qs["type"] = type;
        qs["page"] = "1";

        string url = AhaSearchRoutes_cl.BuildSearchUrl(
            keyword,
            danhMuc,
            thanhPho,
            province,
            district,
            ward,
            type,
            "1");

        var sm = ScriptManager.GetCurrent(Page);
        if (sm != null && sm.IsInAsyncPostBack)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "redirect_search",
                "window.location='" + ResolveUrl(url) + "';", true);
        }
        else
        {
            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }

    private string BuildSearchSummary(string keyword, string categoryId, string thanhPho, dbDataContext db, Dictionary<long, string> thanhPhoMap)
    {
        string summary = "";

        if (!string.IsNullOrWhiteSpace(keyword))
            summary = "Tu khoa: " + keyword.Trim();

        if (!string.IsNullOrWhiteSpace(categoryId))
        {
            string categoryName = db.DanhMuc_tbs
                .Where(dm => dm.id.ToString() == categoryId)
                .Select(dm => dm.name)
                .FirstOrDefault() ?? "";
            if (!string.IsNullOrWhiteSpace(categoryName))
                summary = AppendSummaryPart(summary, "Danh muc: " + categoryName.Trim());
        }

        if (!string.IsNullOrWhiteSpace(thanhPho))
        {
            string locationName = TinhThanhDisplay_cl.ResolveNameFromId(thanhPho, thanhPhoMap);
            locationName = TinhThanhDisplay_cl.Format(locationName);
            if (!string.IsNullOrWhiteSpace(locationName))
                summary = AppendSummaryPart(summary, "Dia diem: " + locationName);
        }

        return summary;
    }

    private static string AppendSummaryPart(string summary, string part)
    {
        if (string.IsNullOrWhiteSpace(part))
            return summary ?? "";
        if (string.IsNullOrWhiteSpace(summary))
            return part.Trim();
        return summary.Trim() + " | " + part.Trim();
    }

    protected void but_xemthem_Click(object sender, EventArgs e)
    {
        int currentPage = ResolveRequestedPage();
        int pageSize = ResolveRequestedPageSize();
        string url = BuildPageUrl(currentPage + 1, pageSize);
        Response.Redirect(url, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private string ToTimeAgoOrDate(object ngayTaoObj)
    {
        if (ngayTaoObj == null) return "";

        DateTime ngayTao;
        try { ngayTao = Convert.ToDateTime(ngayTaoObj); }
        catch { return ""; }

        TimeSpan diff = DateTime.Now - ngayTao;
        if (diff.TotalSeconds < 0) diff = TimeSpan.Zero;

        if (diff.TotalMinutes < 1) return "vừa xong";
        if (diff.TotalMinutes < 60) return ((int)diff.TotalMinutes) + " phút trước";
        if (diff.TotalHours < 24) return ((int)diff.TotalHours) + " giờ trước";
        if (diff.TotalDays < 7) return ((int)diff.TotalDays) + " ngày trước";

        return ngayTao.ToString("dd/MM/yyyy");
    }

    private string RenderCardMediaHtml(string mediaUrl, string altText)
    {
        const string fallback = "/uploads/images/macdinh.jpg";
        bool isVideo = MediaFile_cl.IsVideo(mediaUrl);
        string safeUrl = ResolveMediaUrlOrFallback(mediaUrl, fallback);
        string safeAlt = MediaFile_cl.GetSafeText(altText);

        if (isVideo && !string.Equals(safeUrl, fallback, StringComparison.OrdinalIgnoreCase))
        {
            string mime = MediaFile_cl.GetVideoMime(mediaUrl);
            return "<video class='sp-thumb-video' muted playsinline preload='metadata'>" +
                   "<source src='" + safeUrl + "' type='" + mime + "' />" +
                   "</video>";
        }

        return "<img class='sp-thumb' src='" + safeUrl + "' alt='" + safeAlt + "' />";
    }

    private string ResolveMediaUrlOrFallback(string mediaUrl, string fallback)
    {
        string safeUrl = MediaFile_cl.GetSafeUrl(mediaUrl);
        if (string.IsNullOrWhiteSpace(safeUrl))
            return fallback;

        if (safeUrl.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
            return fallback;

        Uri absolute;
        if (Uri.TryCreate(safeUrl, UriKind.Absolute, out absolute))
        {
            if (string.Equals(absolute.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                || string.Equals(absolute.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                return absolute.AbsoluteUri;
            return fallback;
        }

        string localPath = safeUrl;
        if (localPath.StartsWith("~/", StringComparison.Ordinal))
            localPath = localPath.Substring(1);
        if (!localPath.StartsWith("/", StringComparison.Ordinal))
            localPath = "/" + localPath;

        if (IsMissingUploadFile(localPath))
            return fallback;

        return localPath;
    }

    private bool IsMissingUploadFile(string relativeUrl)
    {
        return Helper_cl.IsMissingUploadFile(relativeUrl);
    }

    protected void RepeaterTin_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            var dataItem = e.Item.DataItem;

            Literal litTimeAgo = (Literal)e.Item.FindControl("lit_timeago");
            litTimeAgo.Text = ToTimeAgoOrDate(DataBinder.Eval(dataItem, "ngaytao"));

            Literal litCardMedia = (Literal)e.Item.FindControl("litCardMedia");
            string mediaUrl = DataBinder.Eval(dataItem, "image") == null ? "" : DataBinder.Eval(dataItem, "image").ToString();
            string mediaAlt = DataBinder.Eval(dataItem, "name") == null ? "" : DataBinder.Eval(dataItem, "name").ToString();
            if (litCardMedia != null)
                litCardMedia.Text = RenderCardMediaHtml(mediaUrl, mediaAlt);

            PlaceHolder phLocRow = (PlaceHolder)e.Item.FindControl("ph_loc_row");
            if (phLocRow != null) phLocRow.Visible = EnableLocation;

            PlaceHolder phKebab = (PlaceHolder)e.Item.FindControl("ph_kebab");
            if (phKebab != null) phKebab.Visible = EnableKebab;
            LinkButton btnHeart = (LinkButton)e.Item.FindControl("btnHeart");

            Button btnBanSanPham = (Button)e.Item.FindControl("but_bansanphamnay");
            Button btnTraoDoi = (Button)e.Item.FindControl("but_traodoi");
            Button btnThemVaoGio = (Button)e.Item.FindControl("but_themvaogio");
            PlaceHolder phActions = (PlaceHolder)e.Item.FindControl("ph_actions");
            Literal litSourceBadge = (Literal)e.Item.FindControl("litSourceBadge");

            bool showButtons = true;
            bool isService = false;
            bool isLinkedBds = Convert.ToBoolean(DataBinder.Eval(dataItem, "IsLinkedBds") ?? false);

            string postType = Convert.ToString(DataBinder.Eval(dataItem, "phanloai")) ?? "";
            if (string.Equals(postType.Trim(), AccountVisibility_cl.PostTypeService, StringComparison.OrdinalIgnoreCase))
                isService = true;

            if (litSourceBadge != null)
            {
                string sourceLabel = Convert.ToString(DataBinder.Eval(dataItem, "SourceLabel")) ?? "";
                litSourceBadge.Text = isLinkedBds
                    ? ("<span class=\"sp-source-badge\">BĐS liên kết" + (string.IsNullOrWhiteSpace(sourceLabel) ? "" : (": " + HttpUtility.HtmlEncode(sourceLabel))) + "</span>")
                    : "";
            }

            if (ViewState["taikhoan"] == null || string.IsNullOrEmpty(ViewState["taikhoan"].ToString()))
            {
                showButtons = false;
            }
            else
            {
                string nguoitao = Convert.ToString(DataBinder.Eval(dataItem, "nguoitao"));
                if (nguoitao == ViewState["taikhoan"].ToString())
                    showButtons = false;
            }

            if (isLinkedBds)
            {
                showButtons = false;
                if (phKebab != null) phKebab.Visible = false;
                if (btnHeart != null) btnHeart.Visible = false;
            }

            // Tính năng bán chéo đã tắt phía home.
            btnBanSanPham.Visible = false;
            btnTraoDoi.Visible = showButtons;
            btnTraoDoi.Text = isService ? "Đặt lịch" : "Trao đổi";
            btnThemVaoGio.Visible = showButtons && !isService;
            phActions.Visible = btnTraoDoi.Visible || btnThemVaoGio.Visible;
        }
    }

    // ===================== BÁN CHÉO =====================
    protected void but_bansanphamnay_Click(object sender, EventArgs e)
    {
        Helper_Tabler_cl.ShowModal(this.Page, "Tính năng bán chéo đã được ẩn trên AhaSale.", "Thông báo", true, "warning");
        return;

        TrapClick("Bán chéo", () =>
        {
            using (dbDataContext db = new dbDataContext())
            {
                var button = (Button)sender;
                string _idsp = button.CommandArgument;

                string tk = (ViewState["taikhoan"] ?? "").ToString();
                if (string.IsNullOrEmpty(tk))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Bạn cần đăng nhập.", "Thông báo", true, "warning");
                    return;
                }

                var sp = db.BaiViet_tbs.FirstOrDefault(p =>
                    p.id.ToString() == _idsp
                    && p.bin == false
                    && db.taikhoan_tbs.Any(acc =>
                        acc.taikhoan == p.nguoitao
                        && (((acc.phanloai == AccountVisibility_cl.ShopPartnerType
                              || ((acc.permission ?? "").ToLower()).Contains(PortalScope_cl.ScopeShop))
                             && acc.TrangThai_Shop == ShopStatus_cl.StatusApproved)
                            || (!(acc.phanloai == AccountVisibility_cl.ShopPartnerType
                                  || ((acc.permission ?? "").ToLower()).Contains(PortalScope_cl.ScopeShop))
                                && acc.block != true))));
                if (sp == null)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm không tồn tại / đã ngừng bán.", "Thông báo", true, "danger");
                    return;
                }

                var q = db.BanSanPhamNay_tbs.FirstOrDefault(p => p.idsp == _idsp && p.taikhoan_ban == tk);
                if (q == null)
                {
                    var ob = new BanSanPhamNay_tb();
                    ob.idsp = _idsp;
                    ob.ban_ngungban = true;
                    ob.ngaythem = DateTime.Now;
                    ob.taikhoan_ban = tk;
                    ob.taikhoan_goc = sp.nguoitao;

                    db.BanSanPhamNay_tbs.InsertOnSubmit(ob);
                    db.SubmitChanges();

                    Helper_Tabler_cl.ShowToast(this.Page, "Xử lý thành công.", "warning", true, 3000, "Thông báo");
                }
                else
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm này đã được thêm vào cửa hàng của bạn.", "Thông báo", true, "danger");
                }
            }

            up_all.Update();
        });
    }

    // ===================== SỐ LƯỢNG (KHÔNG ĐÓNG MODAL) =====================
    protected void txt_soluong2_TextChanged(object sender, EventArgs e)
    {
        int sl = Number_cl.Check_Int(txt_soluong2.Text.Trim());
        if (sl <= 0) sl = 0;

        decimal giaVND = 0m;
        if (ViewState["giaban"] != null)
            decimal.TryParse(ViewState["giaban"].ToString(), out giaVND);

        decimal tongVND = giaVND * sl;

        Literal11.Text = tongVND.ToString("#,##0");

        up_dathang.Update();
        // tuyệt đối không update up_all ở đây
    }

    private string BuildExchangePageUrl(string idsp, int soLuong, string userBanCheo)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["idsp"] = idsp;
        query["qty"] = (soLuong <= 0 ? 1 : soLuong).ToString();
        if (!string.IsNullOrWhiteSpace(userBanCheo))
            query["user_bancheo"] = userBanCheo;
        query["return_url"] = (Request.RawUrl ?? "/");

        string basePath = PortalRequest_cl.IsShopPortalRequest()
            ? "/shop/trao-doi"
            : "/home/trao-doi.aspx";
        return basePath + "?" + query.ToString();
    }

    private string BuildDetailUrl(BaiViet_tb post)
    {
        if (post == null)
            return "/";

        string slug = (post.name_en ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(slug))
        {
            slug = (post.name ?? "").Trim().ToLowerInvariant();
            slug = Regex.Replace(slug, @"[^a-z0-9\-]+", "-").Trim('-');
        }
        if (string.IsNullOrEmpty(slug))
            slug = "san-pham";

        return "/" + slug + "-" + post.id.ToString() + ".html";
    }

    // ===================== TRAO ĐỔI (CHUYỂN TRANG) =====================
    protected void but_traodoi_Click(object sender, EventArgs e)
    {
        TrapClick("Trao đổi", () =>
        {
            Button button = (Button)sender;
            string idsp = (button.CommandArgument ?? "").Trim();
            if (string.IsNullOrEmpty(idsp))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Không xác định được sản phẩm.", "Thông báo", true, "warning");
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                var sp = AccountVisibility_cl.FindVisibleTradePostById(db, idsp);
                if (sp == null)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm không tồn tại / đã ngừng bán.", "Thông báo", true, "danger");
                    return;
                }

                if (AccountVisibility_cl.IsServicePost(sp))
                {
                    string bookingUrl = BuildBookingUrl(sp);
                    Response.Redirect(bookingUrl, false);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    return;
                }
            }

            Response.Redirect(BuildExchangePageUrl(idsp, 1, ""), false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        });
    }

    private string BuildBookingUrl(BaiViet_tb post)
    {
        if (post == null)
            return "/";

        var query = HttpUtility.ParseQueryString(string.Empty);
        string shopAccount = (post.nguoitao ?? "").Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(shopAccount))
            query["user"] = shopAccount;
        query["id"] = post.id.ToString();
        query["return_url"] = (Request.RawUrl ?? "/");
        return "/home/dat-lich.aspx?" + query.ToString();
    }

    // ===================== ĐẶT HÀNG =====================
    protected void but_dathang_Click(object sender, EventArgs e)
    {
        TrapClick("Đặt hàng", () =>
        {
            using (dbDataContext db = new dbDataContext())
            {
                string tk = (ViewState["taikhoan"] ?? "").ToString();
                if (string.IsNullOrEmpty(tk))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Bạn cần đăng nhập.", "Thông báo", true, "warning");
                    return;
                }

                int sl = Number_cl.Check_Int(txt_soluong2.Text.Trim());
                if (sl <= 0)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Số lượng không hợp lệ.", "Thông báo", true, "danger");
                    return;
                }

                var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
                decimal soDuA = (q_tk != null && q_tk.DongA.HasValue) ? q_tk.DongA.Value : 0m;

                // ✅ ví ưu đãi 30% (LoaiHoSo_Vi=2)
                decimal soDuUuDaiA = 0m;
                if (q_tk != null)
                {
                    // đổi đúng tên field bạn đang có trên bảng tài khoản
                    // (ở đây dùng DuVi1_Evocher_30PhanTram theo yêu cầu của bạn)
                    soDuUuDaiA = (q_tk.Vi1That_Evocher_30PhanTram.HasValue) ? q_tk.Vi1That_Evocher_30PhanTram.Value : 0m;

                }

                string idsp = (ViewState["idsp_giohang"] + "");
                var sp = db.BaiViet_tbs.FirstOrDefault(p =>
                    p.id.ToString() == idsp
                    && p.bin == false
                    && db.taikhoan_tbs.Any(acc =>
                        acc.taikhoan == p.nguoitao
                        && (((acc.phanloai == AccountVisibility_cl.ShopPartnerType
                              || ((acc.permission ?? "").ToLower()).Contains(PortalScope_cl.ScopeShop))
                             && acc.TrangThai_Shop == ShopStatus_cl.StatusApproved)
                            || (!(acc.phanloai == AccountVisibility_cl.ShopPartnerType
                                  || ((acc.permission ?? "").ToLower()).Contains(PortalScope_cl.ScopeShop))
                                && acc.block != true))));
                if (sp == null)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm đã ngừng bán", "Thông báo", true, "danger");
                    return;
                }

                decimal giaVND = sp.giaban ?? 0m;
                decimal tongVND = giaVND * sl;

                // ✅ % ưu đãi từ BaiViet (null -> 0)
                int phanTramUuDai = 0;
                if (sp.PhanTram_GiamGia_ThanhToan_BangEvoucher.HasValue)
                    phanTramUuDai = sp.PhanTram_GiamGia_ThanhToan_BangEvoucher.Value;
                if (phanTramUuDai < 0) phanTramUuDai = 0;
                if (phanTramUuDai > 50) phanTramUuDai = 50;

                // ✅ Tổng A của đơn (luôn tính theo tổng đơn)
                decimal canTraA_Tong = QuyDoi_VND_To_A(tongVND);

                // ✅ Tính phần ưu đãi (VND/A)
                decimal tienUuDaiVND = 0m;
                decimal A_UuDai = 0m;
                if (phanTramUuDai > 0)
                {
                    tienUuDaiVND = tongVND * phanTramUuDai / 100m;
                    A_UuDai = QuyDoi_VND_To_A(tienUuDaiVND);
                }

                bool apDungUuDai = (phanTramUuDai > 0 && A_UuDai > 0m && soDuUuDaiA >= A_UuDai);

                // ✅ Nếu áp dụng ưu đãi: A còn lại = TổngA - A ưu đãi (đảm bảo cộng 2 lịch sử = tổng đơn)
                decimal A_ConLai = 0m;
                if (apDungUuDai)
                {
                    A_ConLai = canTraA_Tong - A_UuDai;
                    if (A_ConLai < 0m) A_ConLai = 0m;

                    // Check đủ Đồng A cho phần còn lại
                    if (A_ConLai > soDuA)
                    {
                        Helper_Tabler_cl.ShowModal(
                            this.Page,
                            "Bạn đủ ví ưu đãi nhưng Quyền tiêu dùng không đủ cho phần còn lại.<br/>" +
                            string.Format("Cần thêm <b>{0:#,##0.##} Quyền tiêu dùng</b>, bạn đang có <b>{1:#,##0.##} Quyền tiêu dùng</b>.", A_ConLai, soDuA),
                            "Thông báo",
                            true,
                            "danger"
                        );
                        return;
                    }
                }
                else
                {
                    // Fallback y như cũ: trừ toàn bộ Đồng A
                    if (canTraA_Tong > soDuA)
                    {
                        Helper_Tabler_cl.ShowModal(
                            this.Page,
                            string.Format("Quyền tiêu dùng của bạn không đủ.<br/>Cần <b>{0:#,##0.##} Quyền tiêu dùng</b>, bạn đang có <b>{1:#,##0.##} Quyền tiêu dùng</b>.", canTraA_Tong, soDuA),
                            "Thông báo",
                            true,
                            "danger"
                        );
                        return;
                    }
                }

                DateTime now = DateTime.Now;

                // ===================== LƯU ĐƠN =====================
                DonHang_tb dh = new DonHang_tb();
                dh.ngaydat = now;
                dh.nguoimua = tk;
                dh.nguoiban = sp.nguoitao;
                dh.order_status = DonHangStateMachine_cl.Order_DaDat;
                dh.exchange_status = DonHangStateMachine_cl.Exchange_ChuaTraoDoi;
                dh.tongtien = tongVND;
                dh.hoten_nguoinhan = txt_hoten_nguoinhan.Text.Trim();
                dh.sdt_nguoinhan = txt_sdt_nguoinhan.Text.Trim();
                dh.diahchi_nguoinhan = txt_diachi_nguoinhan.Text.Trim();
                dh.online_offline = true;
                dh.chothanhtoan = false;
                DonHangStateMachine_cl.SyncLegacyStatus(dh);

                db.DonHang_tbs.InsertOnSubmit(dh);
                db.SubmitChanges();

                string id_dh = dh.id.ToString();

                // ===================== CHI TIẾT =====================
                DonHang_ChiTiet_tb ct = new DonHang_ChiTiet_tb();
                ct.id_donhang = id_dh;
                ct.idsp = idsp;
                ct.nguoiban_goc = dh.nguoiban;
                ct.nguoiban_danglai = "";
                ct.soluong = sl;
                ct.giaban = giaVND;
                ct.thanhtien = tongVND;

                // ✅ NEW: lưu % ưu đãi vào chi tiết (từ BaiViet)
                ct.PhanTram_GiamGia_ThanhToan_BangEvoucher = phanTramUuDai;

                db.DonHang_ChiTiet_tbs.InsertOnSubmit(ct);

                // ===================== LỊCH SỬ & TRỪ VÍ =====================
                if (apDungUuDai)
                {
                    // 1) Trừ ví ưu đãi (LoaiHoSo_Vi = 2)
                    LichSu_DongA_tb lsUuDai = new LichSu_DongA_tb();
                    lsUuDai.taikhoan = tk;
                    lsUuDai.dongA = A_UuDai;
                    lsUuDai.ngay = now;
                    lsUuDai.CongTru = false;
                    lsUuDai.id_donhang = id_dh;
                    lsUuDai.LoaiHoSo_Vi = 2; // ✅ ví ưu đãi
                    lsUuDai.ghichu =
                        string.Format("Ưu đãi {0}% đơn {1}: trừ {2:#,##0.##} Quyền tiêu dùng", phanTramUuDai, id_dh, A_UuDai);
                    db.LichSu_DongA_tbs.InsertOnSubmit(lsUuDai);

                    if (q_tk != null && q_tk.Vi1That_Evocher_30PhanTram.HasValue)
                        q_tk.Vi1That_Evocher_30PhanTram = q_tk.Vi1That_Evocher_30PhanTram.Value - A_UuDai;


                    // 2) Trừ ví tiêu dùng (LoaiHoSo_Vi = 1) phần còn lại
                    LichSu_DongA_tb lsA = new LichSu_DongA_tb();
                    lsA.taikhoan = tk;
                    lsA.dongA = A_ConLai;                  // ✅ đảm bảo A_UuDai + A_ConLai = canTraA_Tong
                    lsA.ngay = now;
                    lsA.CongTru = false;
                    lsA.id_donhang = id_dh;
                    lsA.LoaiHoSo_Vi = 1; // ví tiêu dùng (Đồng A)
                    lsA.ghichu =
                        string.Format("Trao đổi đơn {0} (phần còn lại): trừ {1:#,##0.##} Quyền tiêu dùng", id_dh, A_ConLai);
                    db.LichSu_DongA_tbs.InsertOnSubmit(lsA);

                    if (q_tk != null && q_tk.DongA.HasValue)
                        q_tk.DongA = q_tk.DongA.Value - A_ConLai;
                }
                else
                {
                    // Cũ: trừ toàn bộ Đồng A (LoaiHoSo_Vi = 1)
                    LichSu_DongA_tb ls = new LichSu_DongA_tb();
                    ls.taikhoan = tk;
                    ls.dongA = canTraA_Tong;
                    ls.ngay = now;
                    ls.CongTru = false;
                    ls.id_donhang = id_dh;
                    ls.LoaiHoSo_Vi = 1;//ví tiêu dùng
                    ls.ghichu = string.Format("Trao đổi đơn {0}: trừ {1:#,##0.##} Quyền tiêu dùng", id_dh, canTraA_Tong);
                    db.LichSu_DongA_tbs.InsertOnSubmit(ls);

                    if (q_tk != null && q_tk.DongA.HasValue)
                        q_tk.DongA = q_tk.DongA.Value - canTraA_Tong;
                }


                // ===================== THÔNG BÁO CHO NGƯỜI BÁN =====================
                string buyerName = (q_tk != null && !string.IsNullOrEmpty(q_tk.hoten)) ? q_tk.hoten : tk;

                db.ThongBao_tbs.InsertOnSubmit(new ThongBao_tb
                {
                    id = Guid.NewGuid(),
                    daxem = false,
                    nguoithongbao = tk,           // người tạo thông báo = người mua
                    nguoinhan = sp.nguoitao,      // người nhận = người bán
                    link = "/home/don-ban.aspx",  // trang người bán xem đơn
                    noidung = buyerName + " vừa trao đổi sản phẩm của bạn. ID: " + id_dh,
                    thoigian = now,
                    bin = false
                });


                db.SubmitChanges();

                string _emailErr;
                ShopEmailNotify_cl.TryNotifyOrder(db, dh, ShopEmailTemplate_cl.CodeOrderCreated, "", out _emailErr);


                // ===================== THÔNG BÁO RÕ RÀNG =====================
                string msgOk;
                if (apDungUuDai)
                {
                    msgOk =
                        "Đặt hàng thành công.<br/>" +
                        string.Format("ID đơn hàng: <b>{0}</b><br/><br/>", id_dh) +
                        string.Format("Ưu đãi: <b>{0}%</b> → trừ ví ưu đãi <b>{1:#,##0.##} Quyền tiêu dùng</b><br/>", phanTramUuDai, A_UuDai) +
                        string.Format("Còn lại: trừ Quyền tiêu dùng <b>{0:#,##0.##} Quyền tiêu dùng</b><br/><br/>", A_ConLai) +
                        string.Format("Tổng trừ: <b>{0:#,##0.##} Quyền tiêu dùng</b>", canTraA_Tong);
                }
                else
                {
                    if (phanTramUuDai > 0)
                    {
                        msgOk =
                            "Đặt hàng thành công.<br/>" +
                            string.Format("ID đơn hàng: <b>{0}</b><br/><br/>", id_dh) +
                            string.Format("Ưu đãi: <b>{0}%</b> nhưng <b>ví ưu đãi không đủ</b> → hệ thống trừ toàn bộ bằng Quyền tiêu dùng.<br/>", phanTramUuDai) +
                            string.Format("Đã trừ: <b>{0:#,##0.##} Quyền tiêu dùng</b>.", canTraA_Tong);
                    }
                    else
                    {
                        msgOk =
                            "Đặt hàng thành công.<br/>" +
                            string.Format("ID đơn hàng: <b>{0}</b><br/>", id_dh) +
                            string.Format("Đã trừ <b>{0:#,##0.##} Quyền tiêu dùng</b>.", canTraA_Tong);
                    }
                }

                Helper_Tabler_cl.ShowModal(this.Page, msgOk, "Thông báo", true, "success");

                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.Page.GetType(),
                    "closeModalDatHang_" + Guid.NewGuid().ToString("N"),
                    "ModalHelper.hide('modalDatHang');",
                    true
                );

                up_dathang.Update();
                up_all.Update();
            }
        });
    }

    // ===================== THÊM VÀO GIỎ =====================
    protected void but_themvaogio_Click(object sender, EventArgs e)
    {
        TrapClick("Thêm vào giỏ", () =>
        {
            string tk = (ViewState["taikhoan"] ?? "").ToString();
            if (string.IsNullOrEmpty(tk))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Bạn cần đăng nhập.", "Thông báo", true, "warning");
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                Button button = (Button)sender;
                string _idsp = button.CommandArgument;

                var q = db.BaiViet_tbs.FirstOrDefault(p =>
                    p.id.ToString() == _idsp
                    && p.bin == false
                    && db.taikhoan_tbs.Any(acc =>
                        acc.taikhoan == p.nguoitao
                        && (((acc.phanloai == AccountVisibility_cl.ShopPartnerType
                              || ((acc.permission ?? "").ToLower()).Contains(PortalScope_cl.ScopeShop))
                             && acc.TrangThai_Shop == ShopStatus_cl.StatusApproved)
                            || (!(acc.phanloai == AccountVisibility_cl.ShopPartnerType
                                  || ((acc.permission ?? "").ToLower()).Contains(PortalScope_cl.ScopeShop))
                                && acc.block != true))));
                if (q == null)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm đã ngừng bán", "Thông báo", true, "danger");
                    return;
                }

                string errorMessage;
                if (!TryAddProductToCart(db, tk, _idsp, 1, out errorMessage))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, errorMessage, "Thông báo", true, "warning");
                    return;
                }

                Helper_Tabler_cl.ShowToast(this.Page, "Đã thêm vào giỏ hàng thành công. Bạn có thể chỉnh số lượng trong giỏ hàng.", "success", true, 2200, "Thông báo");
            }

            up_all.Update();
        });
    }

    protected void but_add_cart_Click(object sender, EventArgs e)
    {
        TrapClick("Thêm vào giỏ", () =>
        {
            using (dbDataContext db = new dbDataContext())
            {
                string idsp = (ViewState["idsp_giohang"] + "");
                string tk = (ViewState["taikhoan"] + "");

                if (string.IsNullOrEmpty(tk))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Bạn cần đăng nhập.", "Thông báo", true, "warning");
                    return;
                }

                int slAdd = 0;
                int.TryParse((txt_soluong1.Text ?? "0").Trim(), out slAdd);
                if (slAdd <= 0) slAdd = 1;

                string errorMessage;
                if (!TryAddProductToCart(db, tk, idsp, slAdd, out errorMessage))
                {
                    Helper_Tabler_cl.ShowModal(this.Page, errorMessage, "Thông báo", true, "warning");
                    return;
                }

                Helper_Tabler_cl.ShowToast(this.Page, "Xử lý thành công.", "success", true, 2000, "Thông báo");

                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.Page.GetType(),
                    "closeModalAddCart_" + Guid.NewGuid().ToString("N"),
                    "ModalHelper.hide('modalAddCart');",
                    true
                );

                up_add_cart.Update();
                up_all.Update();
            }
        });
    }

    private bool TryAddProductToCart(dbDataContext db, string tk, string idsp, int slAdd, out string errorMessage)
    {
        errorMessage = "";

        if (db == null)
        {
            errorMessage = "Không thể kết nối dữ liệu giỏ hàng.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(tk))
        {
            errorMessage = "Bạn cần đăng nhập.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(idsp))
        {
            errorMessage = "Không xác định được sản phẩm.";
            return false;
        }

        if (slAdd <= 0)
            slAdd = 1;

        var q = db.GioHang_tbs.FirstOrDefault(p => p.idsp == idsp && p.taikhoan == tk);
        if (q != null)
        {
            q.soluong = q.soluong + slAdd;
            q.ngaythem = DateTime.Now;
            db.SubmitChanges();
            return true;
        }

        var sp = AccountVisibility_cl.FindVisibleProductById(db, idsp);
        if (sp == null)
        {
            errorMessage = "Sản phẩm đã ngừng bán hoặc tài khoản đã bị khóa.";
            return false;
        }

        GioHang_tb ob = new GioHang_tb();
        ob.ngaythem = DateTime.Now;
        ob.taikhoan = tk;
        ob.idsp = idsp;
        ob.soluong = slAdd;
        ob.nguoiban_goc = sp.nguoitao;
        ob.nguoiban_danglai = "";
        db.GioHang_tbs.InsertOnSubmit(ob);
        db.SubmitChanges();
        return true;
    }

    private void TrapClick(string actionName, Action run)
    {
        try { run(); }
        catch (Exception ex)
        {
            string msg = "<b>" + HttpUtility.HtmlEncode(actionName) + "</b><br/>"
                       + "<div style='font-size:13px;white-space:pre-wrap'>"
                       + HttpUtility.HtmlEncode(ex.Message)
                       + "<br/><br/>"
                       + HttpUtility.HtmlEncode(ex.StackTrace)
                       + "</div>";

            Helper_Tabler_cl.ShowModal(this.Page, msg, "Lỗi phát sinh", true, "danger");

            ScriptManager.RegisterStartupScript(
                up_all, up_all.GetType(),
                "dbg_err_" + Guid.NewGuid().ToString("N"),
                "console.error(" + HttpUtility.JavaScriptStringEncode(ex.ToString(), true) + ");",
                true
            );

            up_all.Update();
        }
    }

    public class TinItem
    {
        public int id { get; set; }
        public string image { get; set; }
        public string name { get; set; }
        public string name_en { get; set; }
        public string name_khongdau { get; set; }
        public string ThanhPho { get; set; }
        public string ThanhPhoDisplay { get; set; }
        public DateTime? ngaytao { get; set; }
        public DateTime? ngayxem { get; set; }
        public decimal? giaban { get; set; }
        public string nguoitao { get; set; }
        public string description { get; set; }
        public string description_khongdau { get; set; }
        public int? soluong_daban { get; set; }
        public int LuotTruyCap { get; set; }
        public string TenMenu { get; set; }
        public string TenMenu2 { get; set; }
        public bool IsTheoDoi { get; set; }
        public string phanloai { get; set; }
        public string DetailUrl { get; set; }
        public string DisplayPriceText { get; set; }
        public bool IsLinkedBds { get; set; }
        public string SourceLabel { get; set; }
    }

    [WebMethod]
    [ScriptMethod]
    public static List<string> GetSuggestions(string prefixText, int count)
    {
        using (var db = new dbDataContext())
        {
            prefixText = prefixText ?? "";
            var visible = AccountVisibility_cl.FilterVisibleTradePosts(db, db.BaiViet_tbs);
            return visible
                .Where(p => (((p.name ?? "")).StartsWith(prefixText) || ((p.name_en ?? "")).StartsWith(prefixText)))
                .Select(p => p.name)
                .Distinct()
                .Take(count)
                .ToList();
        }
    }
}
