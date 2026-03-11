using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.WebControls; // ✅ BẮT BUỘC – FIX lỗi ListItem


public partial class Uc_Home_DanhChoBan_MoiNhat_UC : System.Web.UI.UserControl
{
    // ===== QUY ƯỚC: 1 A = 1000 VNĐ =====
    private const decimal VND_PER_A = 1000m;

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

        using (dbDataContext db = new dbDataContext())
        {
            // 1) tìm root "Danh mục" (level 1) theo kyhieu + bin
            var root = db.DanhMuc_tbs.FirstOrDefault(p =>
                p.id_level == 1 &&
                p.bin == false &&
                p.kyhieu_danhmuc == "web" &&
                (
                    (p.name != null && p.name.Trim().ToLower() == "danh mục") ||
                    (p.name_en != null && (p.name_en.Trim().ToLower() == "danh-muc" || p.name_en.Trim().ToLower() == "danhmuc"))
                )
            );

            if (root == null) return;

            // 2) chỉ lấy con trực tiếp của root Danh mục
            var children = db.DanhMuc_tbs
                .Where(p =>
                    p.id_parent == root.id.ToString() &&
                    p.bin == false &&
                    p.kyhieu_danhmuc == "web")
                .OrderBy(p => p.rank)
                .ToList();

            foreach (var dm in children)
            {
                ddl_Category.Items.Add(new ListItem(dm.name, dm.id.ToString()));
            }
        }
    }

    private void LoadThanhPho_Search()
    {
        if (ddl_Location == null) return;

        using (dbDataContext db = new dbDataContext())
        {
            var list = db.ThanhPhos
                .ToList()
                .Select(tp => new
                {
                    tp.id,
                    Ten = TinhThanhDisplay_cl.Format(tp.Ten)
                })
                .ToList();
            ddl_Location.DataSource = list;
            ddl_Location.DataTextField = "Ten";
            ddl_Location.DataValueField = "id";
            ddl_Location.DataBind();
            ddl_Location.Items.Insert(0, new ListItem("Tất cả", ""));
            ddl_Location.SelectedIndex = 0;
        }
    }

    // =========================================================
    // ✅ END HERO/SEARCH
    // =========================================================

    protected void Page_Load(object sender, EventArgs e)
    {
        ApplyGuestMobileAuthVisibility();

        if (!IsPostBack)
        {
            // ✅ THÊM: bind dropdown hero/search (KHÔNG ảnh hưởng logic cũ) 
            BindOnlyChildrenOfDanhMucRoot();
            LoadThanhPho_Search();

            // ====== LOGIC CŨ GIỮ NGUYÊN ======
            lblTitle.Text = TitleText;

            try
            {
                string _tk = HttpContext.Current.Session["taikhoan_home"] as string;
                if (!string.IsNullOrEmpty(_tk))
                    ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
                else
                    ViewState["taikhoan"] = "";
            }
            catch { ViewState["taikhoan"] = ""; }

            set_dulieu_macdinh();
            Session[SS_LoadedKey] = new List<TinItem>();

            using (dbDataContext db = new dbDataContext())
                HienThiTinMoi(db, reset: true);
        }
        else
        {
            // nếu page cha thay TitleText trong postback
            lblTitle.Text = TitleText;
        }
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
        ViewState[VS_PageKey] = "1";
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

    public void HienThiTinMoi(dbDataContext db, bool reset = false)
    {
        string idmn = this.Idmn;
        string Is_tin_theo_doi = this.tin_theo_doi;
        string Is_lich_su_xem_tin = this.lich_su_xem_tin;
        var danhMucIds = GetDanhMucFilter(db, idmn);

        string keyword = txt_Search.Text.Trim();
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
            var query = db.BaiViet_tbs.Where(p =>
                p.bin == false &&
                p.phanloai == "sanpham" &&
                db.taikhoan_tbs.Any(acc => acc.taikhoan == p.nguoitao && acc.block != true) &&
                db.TinDaXem_tbs.Any(t => t.idBaiViet == p.id && t.TaiKhoan == taiKhoanHienTai) &&
                !hideList.Contains(p.id)
            );

            // Lọc theo danh mục nếu có
            if (danhMucIds_Search != null && danhMucIds_Search.Count > 0)
            {
                query = query.Where(p =>
                    danhMucIds_Search.Contains(p.id_DanhMuc) ||
                    danhMucIds_Search.Contains(p.id_DanhMucCap2)
                );
            }

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
                    ngayxem = ob5.NgayXem,
                    IsTheoDoi = (ob4 != null),
                    ThanhPho = (ob1.ThanhPho ?? "Không có"),
                    ngaytao = ob1.ngaytao,
                    giaban = ob1.giaban,
                    nguoitao = ob1.nguoitao,
                    description = ob1.description,
                    soluong_daban = ob1.soluong_daban,
                    LuotTruyCap = (ob1.LuotTruyCap ?? 0),
                    TenMenu = ob2 != null ? ob2.name : "",
                    TenMenu2 = ob3 != null
                        ? "<span class='pl-3 mif-chevron-right'></span>" + ob3.name
                        : "",
                };

            list_all.OrderByDescending(p => p.ngayxem).AsQueryable();
        }
        else if (!string.IsNullOrEmpty(Is_tin_theo_doi))
        {
            string taiKhoanHienTai = Convert.ToString(ViewState["taikhoan"]) ?? "";

            // Chỉ lấy các tin được theo dõi
            var query = db.BaiViet_tbs.Where(p =>
                p.bin == false &&
                p.phanloai == "sanpham" &&
                db.taikhoan_tbs.Any(acc => acc.taikhoan == p.nguoitao && acc.block != true) &&
                db.TheoDoiTin_dbs.Any(t =>
                    t.idBaiviet == p.id &&
                    t.taikhoan == taiKhoanHienTai
                ) &&
                !hideList.Contains(p.id)
            );
            // Lọc thêm danh mục từ dropdown nếu có
            if (danhMucIds_Search != null)
            {
                query = query.Where(p =>
                    danhMucIds_Search.Contains(p.id_DanhMuc)
                    || danhMucIds_Search.Contains(p.id_DanhMucCap2));
            }

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
                            IsTheoDoi = (ob4 != null),
                            ThanhPho = (ob1.ThanhPho ?? "Không có"),
                            ngaytao = ob1.ngaytao,
                            giaban = ob1.giaban,
                            nguoitao = ob1.nguoitao,
                            description = ob1.description,
                            soluong_daban = ob1.soluong_daban,
                            LuotTruyCap = (ob1.LuotTruyCap ?? 0),
                            TenMenu = ob2 != null ? ob2.name : "",
                            TenMenu2 = ob3 != null
                                ? "<span class='pl-3 mif-chevron-right'></span>" + ob3.name
                                : "",
                        }).AsQueryable();
        }
        // ✅ Nếu không có idmn => không lọc (trang chủ)
        else if (danhMucIds == null)
        {
            string taiKhoanHienTai = Convert.ToString(ViewState["taikhoan"]) ?? "";

            var query = db.BaiViet_tbs.Where(p =>
                p.bin == false &&
                p.phanloai == "sanpham" &&
                db.taikhoan_tbs.Any(acc => acc.taikhoan == p.nguoitao && acc.block != true) &&
                !hideList.Contains(p.id));

            // ✅ lọc danh mục từ dropdown
            if (danhMucIds_Search != null)
            {
                query = query.Where(p =>
                    danhMucIds_Search.Contains(p.id_DanhMuc)
                    || danhMucIds_Search.Contains(p.id_DanhMucCap2));
            }

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
                            IsTheoDoi = (ob2 != null),
                            ThanhPho = (ob1.ThanhPho ?? "Không có"),
                            ngaytao = ob1.ngaytao,
                            giaban = ob1.giaban,
                            nguoitao = ob1.nguoitao,
                            description = ob1.description,
                            soluong_daban = ob1.soluong_daban,
                            LuotTruyCap = (ob1.LuotTruyCap ?? 0),
                            TenMenu = "",
                            TenMenu2 = ""
                        }).AsQueryable();
        }
        else
        {
            string taiKhoanHienTai = Convert.ToString(ViewState["taikhoan"]) ?? "";

            var query = db.BaiViet_tbs.Where(p =>
                p.bin == false &&
                p.phanloai == "sanpham" &&
                db.taikhoan_tbs.Any(acc => acc.taikhoan == p.nguoitao && acc.block != true) &&
                danhMucIds.Contains(p.id_DanhMuc) &&
                !hideList.Contains(p.id));

            // ✅ lọc thêm danh mục từ dropdown
            if (danhMucIds_Search != null)
            {
                query = query.Where(p =>
                    danhMucIds_Search.Contains(p.id_DanhMuc)
                    || danhMucIds_Search.Contains(p.id_DanhMucCap2));
            }

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
                     IsTheoDoi = (ob4 != null),
                     ThanhPho = (ob1.ThanhPho ?? "Không có"),
                     ngaytao = ob1.ngaytao,
                     giaban = ob1.giaban,
                     nguoitao = ob1.nguoitao,
                     description = ob1.description,
                     soluong_daban = ob1.soluong_daban,
                     LuotTruyCap = (ob1.LuotTruyCap ?? 0),
                     TenMenu = ob2 != null ? ob2.name : "",
                     TenMenu2 = ob3 != null
                         ? "<span class='pl-3 mif-chevron-right'></span>" + ob3.name
                         : "",
                 }).AsQueryable();
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            list_all = list_all.Where(p =>
                p.name.Contains(keyword) ||
                p.description.Contains(keyword));
        }

        if (!string.IsNullOrEmpty(thanhPho))
        {
            if (thanhPhoMap == null)
                thanhPhoMap = db.ThanhPhos.ToDictionary(p => p.id, p => p.Ten);
            string thanhPhoTen = TinhThanhDisplay_cl.ResolveNameFromId(thanhPho, thanhPhoMap);
            list_all = list_all.Where(p => p.ThanhPho == thanhPho || p.ThanhPho == thanhPhoTen);
        }


        list_all = list_all.OrderByDescending(p => p.ngaytao);

        int show = 30; if (show <= 0) show = 30;

        int current_page = 1;
        if (ViewState[VS_PageKey] != null)
            current_page = int.Parse(ViewState[VS_PageKey].ToString());

        int total_record = list_all.Count();
        int total_page = number_of_page_class.return_total_page(total_record, show);
        if (total_page <= 0) total_page = 1;

        if (reset)
        {
            current_page = 1;
            ViewState[VS_PageKey] = "1";
            Session[SS_LoadedKey] = new List<TinItem>();
        }

        var list_split = list_all.Skip((current_page - 1) * show).Take(show).ToList();

        var loaded = Session[SS_LoadedKey] as List<TinItem>;
        if (loaded == null) loaded = new List<TinItem>();

        foreach (var it in list_split)
            if (!loaded.Any(x => x.id == it.id)) loaded.Add(it);

        Session[SS_LoadedKey] = loaded;

        if (thanhPhoMap == null)
            thanhPhoMap = db.ThanhPhos.ToDictionary(p => p.id, p => p.Ten);
        foreach (var it in loaded)
        {
            string name = TinhThanhDisplay_cl.ResolveNameFromId(it.ThanhPho, thanhPhoMap);
            it.ThanhPhoDisplay = TinhThanhDisplay_cl.Format(name);
        }

        RepeaterTin.DataSource = loaded;
        RepeaterTin.DataBind();

        but_xemthem.Visible = current_page < total_page;

        up_all.Update();
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
        ViewState["page"] = 1;
        using (dbDataContext db = new dbDataContext())
            HienThiTinMoi(db, reset: true);
    }

    protected void but_xemthem_Click(object sender, EventArgs e)
    {
        int current_page = 1;
        if (ViewState[VS_PageKey] != null)
            current_page = int.Parse(ViewState[VS_PageKey].ToString());

        current_page++;
        ViewState[VS_PageKey] = current_page.ToString();

        using (dbDataContext db = new dbDataContext())
            HienThiTinMoi(db);
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

            Button btnBanSanPham = (Button)e.Item.FindControl("but_bansanphamnay");
            Button btnTraoDoi = (Button)e.Item.FindControl("but_traodoi");
            Button btnThemVaoGio = (Button)e.Item.FindControl("but_themvaogio");
            PlaceHolder phActions = (PlaceHolder)e.Item.FindControl("ph_actions");

            bool showButtons = true;

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

            // Tính năng bán chéo đã tắt phía home.
            btnBanSanPham.Visible = false;
            btnTraoDoi.Visible = showButtons;
            btnThemVaoGio.Visible = showButtons;
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
                    && db.taikhoan_tbs.Any(acc => acc.taikhoan == p.nguoitao && acc.block != true));
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

            Response.Redirect(BuildExchangePageUrl(idsp, 1, ""), false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        });
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
                    && db.taikhoan_tbs.Any(acc => acc.taikhoan == p.nguoitao && acc.block != true));
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
                    && db.taikhoan_tbs.Any(acc => acc.taikhoan == p.nguoitao && acc.block != true));
                if (q == null)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm đã ngừng bán", "Thông báo", true, "danger");
                    return;
                }

                ViewState["idsp_giohang"] = _idsp;
                Literal1.Text = q.name;
                txt_soluong1.Text = "1";

                up_add_cart.Update();

                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.Page.GetType(),
                    "openModalAddCart_" + Guid.NewGuid().ToString("N"),
                    "ModalHelper.show('modalAddCart');",
                    true
                );
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

                var q = db.GioHang_tbs.FirstOrDefault(p => p.idsp == idsp && p.taikhoan == tk);
                if (q != null)
                {
                    q.soluong = q.soluong + slAdd;
                    q.ngaythem = DateTime.Now;
                    db.SubmitChanges();
                }
                else
                {
                    var sp = AccountVisibility_cl.FindVisibleProductById(db, idsp);
                    if (sp == null)
                    {
                        Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm đã ngừng bán hoặc tài khoản đã bị khóa.", "Thông báo", true, "warning");
                        return;
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
        public string ThanhPho { get; set; }
        public string ThanhPhoDisplay { get; set; }
        public DateTime? ngaytao { get; set; }
        public DateTime? ngayxem { get; set; }
        public decimal? giaban { get; set; }
        public string nguoitao { get; set; }
        public string description { get; set; }
        public int? soluong_daban { get; set; }
        public int LuotTruyCap { get; set; }
        public string TenMenu { get; set; }
        public string TenMenu2 { get; set; }
        public bool IsTheoDoi { get; set; }
    }

    [WebMethod]
    [ScriptMethod]
    public static List<string> GetSuggestions(string prefixText, int count)
    {
        using (var db = new dbDataContext())
        {
            prefixText = prefixText ?? "";
            return db.BaiViet_tbs
                .Where(p => p.bin == false && p.phanloai == "sanpham"
                         && db.taikhoan_tbs.Any(acc => acc.taikhoan == p.nguoitao && acc.block != true)
                         && (((p.name ?? "")).StartsWith(prefixText) || ((p.name_en ?? "")).StartsWith(prefixText)))
                .Select(p => p.name)
                .Distinct()
                .Take(count)
                .ToList();
        }
    }
}
