using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_Default : System.Web.UI.Page
{
    private int PageSize = 20;
    private int CurrentPage
    {
        get
        {
            return ViewState["CurrentPage"] != null ? (int)ViewState["CurrentPage"] : 1;
        }
        set
        {
            ViewState["CurrentPage"] = value;
        }
    }
    private const decimal VND_PER_A = 1000m;
    private const bool PROFILE_COMMERCE_ENABLED = true;
    private const string CONTACT_SOURCE_PREFIX = "home_profile:";
    private const string TIMELINE_POST_TYPE = "home_post";
    private const int TIMELINE_COMMENT_MAX_LEN = 500;
    private const int TIMELINE_COMMENT_MAX_PER_POST = 10;
    private static bool _timelineCommentSchemaEnsured = false;
    private static readonly object TimelineCommentSchemaLock = new object();

    protected bool TimelineIsOwner
    {
        get
        {
            return ResolveIsOwner();
        }
    }

    private string ResolveProfileUser()
    {
        string user = Convert.ToString(ViewState["user_query"]) ?? "";
        if (!string.IsNullOrWhiteSpace(user)) return user;

        user = (Request.QueryString["user"] ?? "").Trim().ToLower();
        if (string.IsNullOrWhiteSpace(user))
        {
            string path = (Request.Path ?? "").Trim().ToLower();
            if (path.EndsWith(".info", StringComparison.OrdinalIgnoreCase))
                user = Path.GetFileNameWithoutExtension(path).Trim().ToLower();
        }

        if (!string.IsNullOrWhiteSpace(user))
            ViewState["user_query"] = user;

        return user;
    }

    private bool ResolveIsOwner()
    {
        object cachedObj = ViewState["profile_is_owner"];
        if (cachedObj != null && cachedObj is bool)
            return (bool)cachedObj;

        string tkEnc = PortalRequest_cl.GetCurrentAccountEncrypted();
        if (string.IsNullOrWhiteSpace(tkEnc)) return false;

        string tk = mahoa_cl.giaima_Bcorn(tkEnc);
        string user = ResolveProfileUser();
        bool isOwner = !string.IsNullOrWhiteSpace(user)
            && string.Equals(tk, user, StringComparison.OrdinalIgnoreCase);

        ViewState["profile_is_owner"] = isOwner;
        return isOwner;
    }

    private static string ExtractField(string note, string prefix)
    {
        if (string.IsNullOrWhiteSpace(note)) return "";
        string[] lines = note.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string raw in lines)
        {
            string line = raw.Trim();
            if (line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return line.Substring(prefix.Length).Trim();
            }
        }
        return "";
    }

    private static string ExtractEmailFromNote(string note)
    {
        return ExtractField(note, "Email:");
    }

    private static string ExtractMessageFromNote(string note)
    {
        if (string.IsNullOrWhiteSpace(note)) return "";
        string[] lines = note.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        List<string> filtered = new List<string>();
        foreach (string raw in lines)
        {
            string line = raw.Trim();
            if (!line.StartsWith("Email:", StringComparison.OrdinalIgnoreCase))
                filtered.Add(line);
        }
        string merged = string.Join(" ", filtered).Trim();
        string message = ExtractField(merged, "Noi dung:");
        if (!string.IsNullOrEmpty(message)) return message;
        message = ExtractField(merged, "Nội dung:");
        if (!string.IsNullOrEmpty(message)) return message;
        return merged;
    }

    private string GetContactSourceKey(string userQuery)
    {
        string key = (userQuery ?? "").Trim().ToLowerInvariant();
        return CONTACT_SOURCE_PREFIX + key;
    }


    private void LoadContactLeads(string userQuery)
    {
        string login = Convert.ToString(ViewState["taikhoan"]) ?? "";
        bool isOwner = !string.IsNullOrWhiteSpace(userQuery)
            && string.Equals(login, userQuery, StringComparison.OrdinalIgnoreCase);

        phOwnerLeads.Visible = isOwner;
        if (!isOwner) return;

        using (dbDataContext db = new dbDataContext())
        {
            AhaShineSchema_cl.EnsureSchemaSafe(db);
            string key = GetContactSourceKey(userQuery);
            var list = db.data_yeucau_tuvan_tables
                .Where(p => p.kyhieu_nguon == key && (p.bin == null || p.bin == false))
                .OrderByDescending(p => p.ngay)
                .Take(50)
                .ToList();

            var view = list.Select(p => new
            {
                Ten = p.ten ?? "",
                Sdt = p.sdt ?? "",
                Email = ExtractEmailFromNote(p.noidung),
                NoiDung = ExtractMessageFromNote(p.noidung),
                Ngay = p.ngay
            }).ToList();

            rptContactLeads.DataSource = view;
            rptContactLeads.DataBind();
            phNoContactLeads.Visible = view.Count == 0;
        }
    }

    private bool IsProfileCommerceEnabled()
    {
        return PROFILE_COMMERCE_ENABLED;
    }

    private bool EnsureProfileCommerceEnabled()
    {
        if (IsProfileCommerceEnabled()) return true;
        Helper_Tabler_cl.ShowModal(this.Page, "Trang cá nhân chỉ hiển thị thông tin hồ sơ. Tính năng cửa hàng/sản phẩm đã được tách khỏi màn hình này.", "Thông báo", true, "warning");
        return false;
    }

    private decimal QuyDoi_VND_To_A(decimal vnd)
    {
        if (vnd <= 0) return 0m;
        decimal a = vnd / VND_PER_A;
        return Math.Ceiling(a * 100m) / 100m; // làm tròn lên 2 chữ số
    }

    private string BuildPostDetailUrl(BaiViet_tb post)
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

    protected void Repeater2_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

        var dataItem = e.Item.DataItem;

        Button btnBanSanPham = (Button)e.Item.FindControl("but_bansanphamnay");
        Button btnTraoDoi = (Button)e.Item.FindControl("but_traodoi");
        Button btnThemVaoGio = (Button)e.Item.FindControl("but_themvaogio");
        Button btnHuyBanCheo = (Button)e.Item.FindControl("but_huy_bancheo");
        Literal litBadge = (Literal)e.Item.FindControl("lit_badge");

        if (!IsProfileCommerceEnabled())
        {
            if (btnBanSanPham != null) btnBanSanPham.Visible = false;
            if (btnTraoDoi != null) btnTraoDoi.Visible = false;
            if (btnThemVaoGio != null) btnThemVaoGio.Visible = false;
            if (btnHuyBanCheo != null) btnHuyBanCheo.Visible = false;
            if (litBadge != null) litBadge.Text = "";
            return;
        }

        // 1) badge
        string spCuaMinh = Convert.ToString(DataBinder.Eval(dataItem, "SPCuaMinh")) ?? "1";
        string postType = Convert.ToString(DataBinder.Eval(dataItem, "phanloai")) ?? "";
        bool isService = string.Equals(postType.Trim(), AccountVisibility_cl.PostTypeService, StringComparison.OrdinalIgnoreCase);
        if (litBadge != null) litBadge.Text = RenderProductBadge(spCuaMinh, isService);

        string serviceId = Convert.ToString(DataBinder.Eval(dataItem, "id")) ?? "";
        string serviceBookingUrl = BuildHomeServiceBookingUrl(serviceId);

        // 2) nếu chưa login: ẩn hết action (nhưng vẫn cho đặt lịch dịch vụ)
        string tkLogin = Convert.ToString(ViewState["taikhoan"]) ?? "";
        if (string.IsNullOrEmpty(tkLogin))
        {
            btnBanSanPham.Visible = false;
            if (btnTraoDoi != null)
            {
                if (isService)
                {
                    btnTraoDoi.Visible = true;
                    btnTraoDoi.Text = "Đặt lịch";
                    btnTraoDoi.OnClientClick = "window.location.href='" + serviceBookingUrl + "'; return false;";
                }
                else
                {
                    btnTraoDoi.Visible = false;
                }
            }
            btnThemVaoGio.Visible = false;
            if (btnHuyBanCheo != null) btnHuyBanCheo.Visible = false;
            return;
        }

        // 3) nếu đang xem shop của CHÍNH MÌNH -> chỉ show "Hủy bán chéo" nếu là SP bán chéo
        if (IsMyShop())
        {
            btnBanSanPham.Visible = false;
            btnTraoDoi.Visible = false;
            btnThemVaoGio.Visible = false;

            if (btnHuyBanCheo != null)
                btnHuyBanCheo.Visible = (spCuaMinh == "0");  // chỉ SP bán chéo mới có nút hủy

            return;
        }

        // 4) đang xem shop người khác
        // không cho thao tác với sản phẩm do chính mình là người bán gốc (tránh tự bán chéo / tự mua chính mình)
        string nguoiBanGoc = Convert.ToString(DataBinder.Eval(dataItem, "NguoiBan")) ?? "";
        if (!string.IsNullOrEmpty(nguoiBanGoc) && nguoiBanGoc == tkLogin)
        {
            btnBanSanPham.Visible = false;
            btnTraoDoi.Visible = false;
            btnThemVaoGio.Visible = false;
        }
        else
        {
            btnBanSanPham.Visible = true;
            btnTraoDoi.Visible = true;
            btnThemVaoGio.Visible = true;
        }

        if (btnHuyBanCheo != null) btnHuyBanCheo.Visible = false; // người xem không thấy nút hủy

        if (isService)
        {
            if (btnBanSanPham != null) btnBanSanPham.Visible = false;
            if (btnTraoDoi != null)
            {
                btnTraoDoi.Text = "Đặt lịch";
                btnTraoDoi.OnClientClick = "window.location.href='" + serviceBookingUrl + "'; return false;";
            }
            if (btnThemVaoGio != null) btnThemVaoGio.Visible = false;
        }
    }

    protected void but_huy_bancheo_Click(object sender, EventArgs e)
    {
        if (!EnsureProfileCommerceEnabled()) return;

        // Chỉ chủ shop (đang login và đang xem shop của chính mình) mới được hủy bán chéo
        if (!IsMyShop())
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Bạn không có quyền thao tác.", "Thông báo", true, "warning");
            return;
        }

        Button button = (Button)sender;
        string idsp = button.CommandArgument; // id bài viết (idsp)

        using (dbDataContext db = new dbDataContext())
        {
            string tk = ViewState["taikhoan"].ToString();

            var row = db.BanSanPhamNay_tbs
                .FirstOrDefault(p => p.idsp == idsp && p.taikhoan_ban == tk);

            if (row == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Không tìm thấy bản ghi bán chéo để hủy.", "Thông báo", true, "warning");
                ShowCuaHang(db);
                return;
            }

            db.BanSanPhamNay_tbs.DeleteOnSubmit(row);
            db.SubmitChanges();

            Helper_Tabler_cl.ShowModal(this.Page, "Đã hủy bán chéo sản phẩm thành công.", "Thông báo", true, "success");

            // Refresh UI
            ShowCuaHang(db);
        }
    }

    public void set_dulieu_macdinh()
    {
        ViewState["current_page_danhmuc_home"] = "1";
    }

    public void ShowCuaHang(dbDataContext db)
    {
        ShopStatus_cl.EnsureSchemaSafe(db);
        AccountVisibility_cl.EnsureTradeTypeNormalized(db);
        if (!IsProfileCommerceEnabled())
        {
            if (Repeater2 != null) { Repeater2.DataSource = null; Repeater2.DataBind(); }
            if (Repeater4 != null) { Repeater4.DataSource = null; Repeater4.DataBind(); }
            if (Literal7 != null) Literal7.Text = "0";
            if (lb_show != null) lb_show.Text = "0-0/0";
            if (lb_show_md != null) lb_show_md.Text = "0-0/0";
            return;
        }

        // ✅ GUARD: nếu user_query chưa là gian hàng đối tác => không load/bind cửa hàng
        bool laGianHang = (Convert.ToString(ViewState["laGianHangDoiTac"]) == "1");
        if (!laGianHang)
        {
            // giữ không phá code, chỉ set rỗng / 0
            if (Repeater2 != null) { Repeater2.DataSource = null; Repeater2.DataBind(); }
            if (Repeater4 != null) { Repeater4.DataSource = null; Repeater4.DataBind(); }
            if (Literal7 != null) Literal7.Text = "0";
            if (lb_show != null) lb_show.Text = "0-0/0";
            if (lb_show_md != null) lb_show_md.Text = "0-0/0";
            return;
        }

        var list_all = (from ob1 in db.BaiViet_tbs.Where(p => (p.bin == false || p.bin == null)
                                                             && (p.phanloai == AccountVisibility_cl.PostTypeProduct || p.phanloai == AccountVisibility_cl.PostTypeService)
                                                             && p.nguoitao == ViewState["user_query"].ToString())
                        join ob2 in db.DanhMuc_tbs on ob1.id_DanhMuc equals ob2.id.ToString() into danhMucGroup
                        from ob2 in danhMucGroup.DefaultIfEmpty()
                        join ob3 in db.DanhMuc_tbs on ob1.id_DanhMucCap2 equals ob3.id.ToString() into danhMucGroup2
                        from ob3 in danhMucGroup2.DefaultIfEmpty()
                        select new
                        {
                            ob1.id,
                            ob1.image,
                            ob1.name,
                            ob1.name_en,
                            ob1.ngaytao,
                            ob1.giaban,
                            NguoiBan = ob1.nguoitao,
                            ob1.description,
                            ob1.soluong_daban,
                            LuotTruyCap = (ob1.LuotTruyCap ?? 0),
                            TenMenu = ob2 != null ? ob2.name : "",
                            TenMenu2 = ob3 != null ? "<span class='pl-3 mif-chevron-right'></span>" + ob3.name : "",
                            SPCuaMinh = "1",
                            phanloai = ob1.phanloai,
                        }).AsQueryable();

        var list_1 = (from ob in db.BanSanPhamNay_tbs.Where(p => p.taikhoan_ban == ViewState["user_query"].ToString())
                      join ob1 in db.BaiViet_tbs on ob.idsp equals ob1.id.ToString() into BonBap
                      from ob1 in BonBap.DefaultIfEmpty()
                      where ob1 != null
                            && (ob1.bin == false || ob1.bin == null)
                            && db.taikhoan_tbs.Any(acc =>
                                acc.taikhoan == ob1.nguoitao
                                && (((acc.phanloai == AccountVisibility_cl.ShopPartnerType
                                      || ((acc.permission ?? "").ToLower()).Contains(PortalScope_cl.ScopeShop))
                                     && acc.TrangThai_Shop == ShopStatus_cl.StatusApproved)
                                    || (!(acc.phanloai == AccountVisibility_cl.ShopPartnerType
                                          || ((acc.permission ?? "").ToLower()).Contains(PortalScope_cl.ScopeShop))
                                        && acc.block != true)))
                      join ob2 in db.DanhMuc_tbs on ob1.id_DanhMuc equals ob2.id.ToString() into danhMucGroup
                      from ob2 in danhMucGroup.DefaultIfEmpty()
                      join ob3 in db.DanhMuc_tbs on ob1.id_DanhMucCap2 equals ob3.id.ToString() into danhMucGroup2
                      from ob3 in danhMucGroup2.DefaultIfEmpty()
                      select new
                      {
                          ob1.id,
                          ob1.image,
                          ob1.name,
                          ob1.name_en,
                          ob1.ngaytao,
                          giaban = ob1.giaban,
                          NguoiBan = ob.taikhoan_goc,
                          ob1.description,
                          ob1.soluong_daban,
                          LuotTruyCap = (ob1.LuotTruyCap ?? 0),
                          TenMenu = ob2 != null ? ob2.name : "",
                          TenMenu2 = ob3 != null ? "<span class='pl-3 mif-chevron-right'></span>" + ob3.name : "",
                          SPCuaMinh = "0",
                          phanloai = ob1.phanloai,
                      }).AsQueryable();

        string _key = txt_search.Text.Trim();
        if (!string.IsNullOrEmpty(_key))
            list_all = list_all.Where(p => p.name.Contains(_key) || p.name_en.Contains(_key) || p.id.ToString() == _key);

        int _Tong_Record = list_all.Count();

        #region phân trang OK, k sửa
        int show = 40; if (show <= 0) show = 40;

        int current_page = int.Parse(ViewState["current_page_danhmuc_home"].ToString());
        int total_page = number_of_page_class.return_total_page(_Tong_Record, show);
        if (current_page < 1) current_page = 1;
        else if (current_page > total_page) current_page = total_page;

        ViewState["total_page"] = total_page;

        if (current_page >= total_page)
        {
            but_xemtiep.Enabled = false;
            but_xemtiep1.Enabled = false;
        }
        else
        {
            but_xemtiep.Enabled = true;
            but_xemtiep1.Enabled = true;
        }
        if (current_page == 1)
        {
            but_quaylai.Enabled = false;
            but_quaylai1.Enabled = false;
        }
        else
        {
            but_quaylai.Enabled = true;
            but_quaylai1.Enabled = true;
        }

        var list_split = list_all.Skip(current_page * show - show).Take(show);

        int stt = (show * current_page) - show + 1;
        int _s1 = stt + list_split.Count() - 1;

        if (_Tong_Record != 0) lb_show.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");
        else lb_show.Text = "0-0/0";

        lb_show_md.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");
        #endregion

        var combinedList = list_split.Concat(list_1).OrderByDescending(p => p.ngaytao);

        #region chia đôi ra gán cho 2 repetar
        int totalCount = combinedList.Count();
        int remainder = totalCount % 4;
        int halfCount;

        if (remainder == 0) halfCount = totalCount / 2;
        else if (remainder == 1) halfCount = (totalCount / 2) + 1;
        else if (remainder == 2) halfCount = (totalCount / 2) + 1;
        else halfCount = (totalCount / 2) + 1;

        var list1 = combinedList.Take(halfCount).ToList();
        var list2 = combinedList.Skip(halfCount).ToList();

        Repeater2.DataSource = list1;
        Repeater2.DataBind();

        Repeater4.DataSource = list2;
        Repeater4.DataBind();
        #endregion

        Literal7.Text = combinedList.Count().ToString();
    }

    [System.Web.Services.WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    public static List<string> GetSuggestions(string prefixText, int count, string userQuery)
    {
        using (var db = new dbDataContext())
        {
            // ✅ GUARD: nếu userQuery chưa là gian hàng đối tác => trả về rỗng
            ShopStatus_cl.EnsureSchemaSafe(db);
            AccountVisibility_cl.EnsureTradeTypeNormalized(db);
            var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == userQuery);
            bool laGianHang = q_tk != null && ShopSlug_cl.IsShopAccount(db, q_tk);
            if (!laGianHang) return new List<string>();

            var list_all = (from ob1 in db.BaiViet_tbs
                            where (ob1.bin == false || ob1.bin == null) &&
                                  (ob1.phanloai == AccountVisibility_cl.PostTypeProduct || ob1.phanloai == AccountVisibility_cl.PostTypeService) &&
                                  ob1.nguoitao == userQuery &&
                                  (
                                      ob1.name.ToLower().Contains(prefixText.ToLower()) ||
                                      ob1.name_en.ToLower().Contains(prefixText.ToLower())
                                  )
                            select new { ob1.name }).AsQueryable();

            var list_1 = (from ob in db.BanSanPhamNay_tbs.Where(p => p.taikhoan_ban == userQuery)
                          join ob1 in db.BaiViet_tbs on ob.idsp equals ob1.id.ToString() into BonBap
                          from ob1 in BonBap.DefaultIfEmpty()
                          where ob1 != null
                                && ob1.bin == false
                                && db.taikhoan_tbs.Any(acc =>
                                    acc.taikhoan == ob1.nguoitao
                                    && (((acc.phanloai == AccountVisibility_cl.ShopPartnerType
                                          || ((acc.permission ?? "").ToLower()).Contains(PortalScope_cl.ScopeShop))
                                         && acc.TrangThai_Shop == ShopStatus_cl.StatusApproved)
                                        || (!(acc.phanloai == AccountVisibility_cl.ShopPartnerType
                                              || ((acc.permission ?? "").ToLower()).Contains(PortalScope_cl.ScopeShop))
                                            && acc.block != true)))
                          select new { ob1.name }).AsQueryable();

            var combinedList = list_all.Concat(list_1);
            return combinedList.Select(x => x.name).Take(count).ToList();
        }
    }

    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            ShowCuaHang(db);
        }
    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        ViewState["current_page_danhmuc_home"] = int.Parse(ViewState["current_page_danhmuc_home"].ToString()) - 1;
        using (dbDataContext db = new dbDataContext())
            ShowCuaHang(db);
    }

    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        ViewState["current_page_danhmuc_home"] = int.Parse(ViewState["current_page_danhmuc_home"].ToString()) + 1;
        using (dbDataContext db = new dbDataContext())
            ShowCuaHang(db);
    }

    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        ViewState["current_page_danhmuc_home"] = 1;
        using (dbDataContext db = new dbDataContext())
            ShowCuaHang(db);
    }

    protected void but_bansanphamnay_Click(object sender, EventArgs e)
    {
        if (!EnsureProfileCommerceEnabled()) return;

        using (dbDataContext db = new dbDataContext())
        {
            Button button = (Button)sender;
            string _idsp = button.CommandArgument;

            var q = db.BanSanPhamNay_tbs.FirstOrDefault(p => p.idsp == _idsp && p.taikhoan_ban == ViewState["taikhoan"].ToString());
            if (q == null)
            {
                var sp = AccountVisibility_cl.FindVisibleProductById(db, _idsp);
                if (sp == null)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm đã ngừng bán hoặc tài khoản đã bị khóa.", "Thông báo", true, "warning");
                    return;
                }

                BanSanPhamNay_tb _ob = new BanSanPhamNay_tb();
                _ob.idsp = _idsp;
                _ob.ban_ngungban = true;
                _ob.ngaythem = AhaTime_cl.Now;
                _ob.taikhoan_ban = ViewState["taikhoan"].ToString();
                _ob.taikhoan_goc = sp.nguoitao;
                db.BanSanPhamNay_tbs.InsertOnSubmit(_ob);
                db.SubmitChanges();

                Helper_Tabler_cl.ShowModal(this.Page, "Xử lý thành công.", "Thông báo", true, "success");
            }
            else
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm này đã được thêm vào cửa hàng của bạn.", "Thông báo", true, "warning");
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", false);

            string queryUser = (Request.QueryString["user"] ?? "").Trim().ToLower();
            string queryShopSlug = (Request.QueryString["shop_slug"] ?? "").Trim().ToLower();

            if (string.IsNullOrWhiteSpace(queryUser) && string.IsNullOrWhiteSpace(queryShopSlug))
            {
                Response.Redirect("/", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            using (dbDataContext db = new dbDataContext())
            {
                taikhoan_tb q_tk = null;
                bool fromSlugRoute = !string.IsNullOrEmpty(queryShopSlug);

                if (fromSlugRoute)
                {
                    q_tk = ShopSlug_cl.FindApprovedShopBySlug(db, queryShopSlug);
                }
                else
                {
                    q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == queryUser);
                }

                if (q_tk == null)
                {
                    Response.Redirect("/", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                if (q_tk.block == true)
                {
                    Response.Redirect("/", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                if (AccountType_cl.IsTreasury(q_tk.phanloai))
                {
                    Response.Redirect("/", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                string scopeQuery = PortalScope_cl.ResolveScope(q_tk.taikhoan, q_tk.phanloai, q_tk.permission);
                if (string.Equals(scopeQuery, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase)
                    && !ShopSlug_cl.IsShopAccount(db, q_tk))
                {
                    Response.Redirect("/", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                if (ShopSlug_cl.IsShopAccount(db, q_tk))
                {
                    string targetPublic = ShopSlug_cl.GetPublicUrl(db, q_tk);
                    string legacyInfoPath = "/" + (q_tk.taikhoan ?? "").Trim().ToLowerInvariant() + ".info";
                    string currentRawUrl = (Request.RawUrl ?? "").Trim().ToLowerInvariant();

                    // Chặn vòng lặp khi route .info cũ tự trỏ về chính nó.
                    if (string.IsNullOrEmpty(targetPublic)
                        || string.Equals(targetPublic, legacyInfoPath, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(currentRawUrl, targetPublic, StringComparison.OrdinalIgnoreCase))
                    {
                        targetPublic = "/shop/public.aspx?user=" + HttpUtility.UrlEncode((q_tk.taikhoan ?? "").Trim().ToLowerInvariant());
                    }

                    Response.Redirect(targetPublic, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                ViewState["user_query"] = q_tk.taikhoan;
                string linkHoSoCongKhai = ShopSlug_cl.GetPublicUrl(db, q_tk);
                ViewState["link_hoso_congkhai"] = linkHoSoCongKhai;
                ViewState["public_profile_url"] = Request.Url.GetLeftPart(UriPartial.Authority) + linkHoSoCongKhai;
                ViewState["link_dangky_ref"] = "/home/dangky.aspx?ref=" + HttpUtility.UrlEncode((q_tk.taikhoan ?? "").Trim().ToLowerInvariant());
                ViewState["link_luu_danhba"] = "/home/luu-danh-ba.aspx?user=" + HttpUtility.UrlEncode((q_tk.taikhoan ?? "").Trim().ToLowerInvariant());

                // ✅ XÁC ĐỊNH: user_query đã là gian hàng đối tác chưa (ưu tiên: phanloai hoặc đã duyệt đăng ký)
                bool laGianHangDoiTac = ShopSlug_cl.IsShopAccount(db, q_tk);

                ViewState["laGianHangDoiTac"] = laGianHangDoiTac ? "1" : "0";

                var profileSettings = HomeProfileSetting_cl.GetSettings(db, q_tk.taikhoan, laGianHangDoiTac);
                ViewState["profile_template"] = string.IsNullOrEmpty(profileSettings.TemplateKey) ? "classic" : profileSettings.TemplateKey;
                ViewState["profile_accent"] = string.IsNullOrEmpty(profileSettings.AccentColor) ? "#22c55e" : profileSettings.AccentColor;
                ViewState["profile_show_social"] = profileSettings.ShowSocial ? "1" : "0";
                ViewState["profile_show_reviews"] = profileSettings.ShowReviews ? "1" : "0";
                ViewState["profile_show_contact"] = profileSettings.ShowContact ? "1" : "0";
                ViewState["profile_show_shop"] = profileSettings.ShowShop ? "1" : "0";
                ViewState["profile_show_products"] = profileSettings.ShowProducts ? "1" : "0";

                var link = db.MangXaHoi_tbs.Where(x => x.TaiKhoan == q_tk.taikhoan).ToList();
                var linkCaNhan = SocialLinkOrder_cl.SortLinks(link.Where(x => x.Kieu == "Cá nhân"), profileSettings.SocialOrderPersonal);
                var linkCuaHang = SocialLinkOrder_cl.SortLinks(link.Where(x => x.Kieu == "Cửa hàng"), profileSettings.SocialOrderShop);
                rptMangXaHoiCN.DataSource = linkCaNhan;
                rptMangXaHoiCH.DataSource = linkCuaHang;
                rptMangXaHoiCN.DataBind();
                rptMangXaHoiCH.DataBind();
                phNoSocialLinks.Visible = profileSettings.ShowSocial && linkCaNhan.Count == 0;

                string _tk1 = PortalRequest_cl.GetCurrentAccountEncrypted();
                if (!string.IsNullOrEmpty(_tk1))
                {
                    ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk1);
                    #region kiểm tra nếu có chờ Trao đổi
                    try
                    {
                        var q1 = db.DonHang_tbs.FirstOrDefault(p =>
                            p.nguoiban == ViewState["taikhoan"].ToString()
                            && (
                                p.exchange_status == DonHangStateMachine_cl.Exchange_ChoTraoDoi
                                || (p.exchange_status == null && p.trangthai == DonHangStateMachine_cl.Exchange_ChoTraoDoi)
                            ));
                        if (q1 != null)
                        {
                            Response.Redirect("/home/cho-thanh-toan.aspx");
                        }
                    }
                    catch (SqlException ex)
                    {
                        if (!IsMissingDonHangStatusColumnError(ex))
                            throw;
                    }
                    #endregion
                }
                else
                {
                    ViewState["taikhoan"] = "";
                }
                bool isOwner = string.Equals(
                    Convert.ToString(ViewState["taikhoan"]) ?? "",
                    q_tk.taikhoan,
                    StringComparison.OrdinalIgnoreCase
                );
                ViewState["profile_is_owner"] = isOwner;
                phOwnerEditButton.Visible = isOwner;
                phOwnerEditButtonSide.Visible = isOwner;
                phHeroEditRow.Visible = isOwner;
                phContactCta.Visible = !isOwner && profileSettings.ShowContact;
                phNavLeads.Visible = isOwner;
                phTimelineEditor.Visible = isOwner;

                phSocialLinksBlock.Visible = profileSettings.ShowSocial;
                phReviewsBlock.Visible = profileSettings.ShowReviews;

                bool showShop = laGianHangDoiTac && profileSettings.ShowShop;
                bool showProducts = laGianHangDoiTac && profileSettings.ShowProducts;
                phCuaHang.Visible = showShop;
                phSanPhamCuaHang.Visible = showProducts;

                string metaTags = string.Format(@"
                    <title>Hồ sơ {0}</title>
                ", q_tk.hoten);
                literal_meta.Text = metaTags;

                ViewState["taikhoan_hienthi_query"] = q_tk.taikhoan;
                ViewState["avt_query"] = ResolveProductImage(q_tk.anhdaidien);
                ViewState["hoten_query"] = string.IsNullOrWhiteSpace(q_tk.hoten) ? q_tk.taikhoan : q_tk.hoten.Trim();
                ViewState["gioithieu_query"] = string.IsNullOrWhiteSpace(q_tk.gioithieu) ? "Chưa cập nhật giới thiệu." : q_tk.gioithieu.Trim();
                ViewState["email_query"] = string.IsNullOrWhiteSpace(q_tk.email) ? "Chưa cập nhật" : q_tk.email.Trim();
                ViewState["diachi_query"] = string.IsNullOrWhiteSpace(q_tk.diachi) ? "Chưa cập nhật" : q_tk.diachi.Trim();

                bool hasSdt = !string.IsNullOrWhiteSpace(q_tk.dienthoai);
                ViewState["sdt_query"] = hasSdt ? q_tk.dienthoai.Trim() : "Chưa cập nhật";
                ViewState["sdt_href_query"] = hasSdt ? ("tel:" + q_tk.dienthoai.Trim()) : "#";
                // Không hiển thị Điểm A trên hồ sơ công khai

                if (string.Equals(scopeQuery, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
                {
                    ViewState["phanloai_query"] = "<span class='badge bg-yellow-lt text-yellow'>Gian hàng đối tác</span>";
                }
                else if (string.Equals(scopeQuery, PortalScope_cl.ScopeAdmin, StringComparison.OrdinalIgnoreCase))
                {
                    ViewState["phanloai_query"] = "<span class='badge bg-blue-lt text-blue'>Nhân viên admin</span>";
                }
                else
                {
                    int tierHomeQuery = TierHome_cl.TinhTierHome(db, q_tk.taikhoan);
                    string tenTangHome = TierHome_cl.GetTenTangHome(tierHomeQuery);
                    if (tenTangHome == "Đồng hành hệ sinh thái")
                        ViewState["phanloai_query"] = "<span class='badge bg-red-lt text-red'>Đồng hành hệ sinh thái</span>";
                    else if (tenTangHome == "Cộng tác phát triển")
                        ViewState["phanloai_query"] = "<span class='badge bg-yellow-lt text-yellow'>Cộng tác phát triển</span>";
                    else
                        ViewState["phanloai_query"] = "<span class='badge bg-green-lt text-green'>Khách hàng</span>";
                }

                #region tính tổng số lượng sản phẩm đã bán
                var q_daban = db.DonHang_ChiTiet_tbs.Where(p => p.nguoiban_goc == ViewState["user_query"].ToString() || p.nguoiban_danglai == ViewState["user_query"].ToString());
                if (q_daban.Any())
                    Literal12.Text = q_daban.Sum(p => p.soluong.Value).ToString("#,##0");
                else
                    Literal12.Text = "0";
                #endregion

                if (profileSettings.ShowReviews)
                    LoadDanhGia(1);
                set_dulieu_macdinh();
                if (showShop || showProducts)
                    ShowCuaHang(db);
                LoadContactLeads(q_tk.taikhoan);
                LoadTimeline(db, q_tk.taikhoan, isOwner);
            }
        }
        else
        {
            bool showReviews = (ViewState["profile_show_reviews"] ?? "1").ToString() == "1";
            if (showReviews)
            {
                int totalPages = ViewState["TotalPages"] != null ? (int)ViewState["TotalPages"] : 1;
                DisplayPaging(totalPages);
            }
            LoadContactLeads(Convert.ToString(ViewState["user_query"]) ?? "");
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        string action = Convert.ToString(ViewState["link_hoso_congkhai"]) ?? "";
        if (string.IsNullOrWhiteSpace(action))
        {
            string user = ResolveProfileUser();
            if (!string.IsNullOrWhiteSpace(user))
                action = "/" + user.Trim().ToLowerInvariant() + ".info";
        }

        if (!string.IsNullOrWhiteSpace(action) && this.Form != null)
        {
            this.Form.Action = action;
        }
    }


    #region trao đổi (đặt hàng)
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

    private string BuildAddCartPageUrl(string idsp, int soLuong, string userBanCheo)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["idsp"] = idsp;
        query["qty"] = (soLuong <= 0 ? 1 : soLuong).ToString();
        if (!string.IsNullOrWhiteSpace(userBanCheo))
            query["user_bancheo"] = userBanCheo;
        query["return_url"] = (Request.RawUrl ?? "/");

        string basePath = PortalRequest_cl.IsShopPortalRequest()
            ? "/shop/them-vao-gio"
            : "/home/them-vao-gio.aspx";
        return basePath + "?" + query.ToString();
    }

    private string BuildHomeServiceBookingUrl(string serviceId)
    {
        if (string.IsNullOrWhiteSpace(serviceId))
            return "/home/dat-lich.aspx";

        var query = HttpUtility.ParseQueryString(string.Empty);
        query["id"] = serviceId;

        string userQuery = (ViewState["user_query"] ?? "").ToString().Trim();
        if (!string.IsNullOrWhiteSpace(userQuery))
            query["user"] = userQuery;

        query["return_url"] = (Request.RawUrl ?? "/");
        return "/home/dat-lich.aspx?" + query.ToString();
    }

    protected void but_traodoi_Click(object sender, EventArgs e)
    {
        if (!EnsureProfileCommerceEnabled()) return;

        string tk = (ViewState["taikhoan"] ?? "").ToString();
        if (string.IsNullOrEmpty(tk))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Bạn cần đăng nhập.", "Thông báo", true, "warning");
            return;
        }

        Button button = (Button)sender;
        string idsp = (button.CommandArgument ?? "").Trim();
        if (string.IsNullOrEmpty(idsp))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Không xác định được sản phẩm.", "Thông báo", true, "warning");
            return;
        }

        string userBanCheo = (ViewState["user_query"] ?? "").ToString().Trim();
        using (dbDataContext db = new dbDataContext())
        {
            var sp = AccountVisibility_cl.FindVisibleTradePostById(db, idsp);
            if (sp == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm đã ngừng bán hoặc tài khoản đã bị khóa.", "Thông báo", true, "warning");
                return;
            }

            if (AccountVisibility_cl.IsServicePost(sp))
            {
                Response.Redirect(BuildHomeServiceBookingUrl(idsp), true);
                return;
            }
        }

        Response.Redirect(BuildExchangePageUrl(idsp, 1, userBanCheo), true);
    }

    #endregion

    protected bool ShouldShowIcon(object icon)
    {
        return ShouldShowIcon(icon, null);
    }

    protected bool ShouldShowIcon(object icon, object link)
    {
        return !string.IsNullOrEmpty(ResolveSocialIcon(icon, link));
    }

    protected string GetMarginStyle(object icon)
    {
        return GetMarginStyle(icon, null);
    }

    protected string GetMarginStyle(object icon, object link)
    {
        return !ShouldShowIcon(icon, link) ? "margin-left: 60px;" : "";
    }

    protected string ResolveExternalLink(object linkRaw)
    {
        string link = (linkRaw ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(link))
            return "#";

        if (link.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
            return "#";

        Uri absolute;
        if (!Uri.TryCreate(link, UriKind.Absolute, out absolute))
        {
            string normalized = "https://" + link.TrimStart('/');
            if (!Uri.TryCreate(normalized, UriKind.Absolute, out absolute))
                return "#";
        }

        if (!string.Equals(absolute.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(absolute.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            return "#";

        return absolute.AbsoluteUri;
    }

    protected string ResolveExternalLinkLabel(object linkRaw)
    {
        string link = (linkRaw ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(link))
            return "Liên kết chưa hợp lệ";

        Uri absolute;
        if (Uri.TryCreate(link, UriKind.Absolute, out absolute))
            return absolute.Host;

        string compact = link.TrimStart('/');
        int slash = compact.IndexOf('/');
        if (slash > 0)
            return compact.Substring(0, slash);

        return compact;
    }

    protected string ResolveProductImage(object imageRaw)
    {
        return ResolveImageOrFallback(imageRaw, "/uploads/images/macdinh.jpg");
    }

    protected string ResolveSocialIcon(object imageRaw)
    {
        return ResolveSocialIcon(imageRaw, null);
    }

    protected string ResolveSocialIcon(object imageRaw, object linkRaw)
    {
        string explicitIcon = ResolveImageOrFallback(imageRaw, "");
        if (!string.IsNullOrEmpty(explicitIcon))
            return explicitIcon;

        return SocialLinkIcon_cl.ResolveIconForDisplay(
            "",
            Convert.ToString(linkRaw) ?? ""
        );
    }

    private string ResolveImageOrFallback(object imageRaw, string fallback)
    {
        string image = (imageRaw ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(image))
            return fallback;

        if (image.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
            return fallback;

        Uri absolute;
        if (Uri.TryCreate(image, UriKind.Absolute, out absolute))
        {
            if (string.Equals(absolute.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                || string.Equals(absolute.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                return absolute.AbsoluteUri;
            return fallback;
        }

        if (image.StartsWith("~/", StringComparison.Ordinal))
            image = image.Substring(1);
        if (!image.StartsWith("/", StringComparison.Ordinal))
            image = "/" + image;

        if (IsMissingUploadFile(image))
            return fallback;

        return image;
    }

    private bool IsMissingUploadFile(string relativeUrl)
    {
        if (string.IsNullOrEmpty(relativeUrl))
            return false;

        string cleanPath = relativeUrl.Trim();
        if (cleanPath.Length == 0)
            return false;

        int q = cleanPath.IndexOf('?');
        if (q >= 0)
            cleanPath = cleanPath.Substring(0, q);

        int h = cleanPath.IndexOf('#');
        if (h >= 0)
            cleanPath = cleanPath.Substring(0, h);

        if (!cleanPath.StartsWith("/", StringComparison.Ordinal))
            cleanPath = "/" + cleanPath.TrimStart('/');

        if (!cleanPath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            return false;

        try
        {
            string physical = Server.MapPath("~" + cleanPath);
            if (string.IsNullOrEmpty(physical))
                return false;

            return !System.IO.File.Exists(physical);
        }
        catch
        {
            return false;
        }
    }

    #region thêm vào giỏ
    protected void but_themvaogio_Click(object sender, EventArgs e)
    {
        if (!EnsureProfileCommerceEnabled()) return;

        Button button = (Button)sender;
        string idsp = (button.CommandArgument ?? "").Trim();
        if (string.IsNullOrEmpty(idsp))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Không xác định được sản phẩm.", "Thông báo", true, "warning");
            return;
        }

        string userBanCheo = (ViewState["user_query"] ?? "").ToString().Trim();
        Response.Redirect(BuildAddCartPageUrl(idsp, 1, userBanCheo), true);
    }
    #endregion

    protected void Page_Changed(object sender, CommandEventArgs e)
    {
        int pageIndex = int.Parse(e.CommandArgument.ToString());
        LoadDanhGia(pageIndex);
    }

    private void LoadDanhGia(int page)
    {
        using (dbDataContext db = new dbDataContext())
        {
            var taiKhoan = ViewState["user_query"].ToString();

            var danhSachDanhGiaVaAnh = (from dg in db.DanhGiaBaiViets
                                        join tk in db.taikhoan_tbs on dg.TaiKhoanDanhGia equals tk.taikhoan
                                        where dg.ThuocTaiKhoan == taiKhoan
                                        orderby dg.NgayDang descending
                                        select new
                                        {
                                            dg.NoiDung,
                                            dg.Diem,
                                            dg.TaiKhoanDanhGia,
                                            dg.NgayDang,
                                            dg.UrlAnh,
                                            dg.ThuocTaiKhoan,
                                            AnhDaiDien = tk.anhdaidien
                                        }).ToList();

            int totalItems = danhSachDanhGiaVaAnh.Count;
            ViewState["SoLuongDanhGia"] = totalItems;
            int totalPages = (int)Math.Ceiling((double)totalItems / PageSize);
            if (totalPages == 0) totalPages = 1;

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            CurrentPage = page;
            ViewState["TotalPages"] = totalPages;

            var dataPage = danhSachDanhGiaVaAnh
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList()
                .Select(x => new
                {
                    x.NoiDung,
                    x.Diem,
                    x.TaiKhoanDanhGia,
                    x.NgayDang,
                    x.UrlAnh,
                    x.ThuocTaiKhoan,
                    x.AnhDaiDien,
                    HoSoUrl = ShopSlug_cl.GetPublicUrlByTaiKhoan(db, x.TaiKhoanDanhGia)
                })
                .ToList();

            ListReview.Visible = dataPage.Any();
            PlaceHolder2.Visible = dataPage.Any();
            rptDanhGia.DataSource = dataPage;
            rptDanhGia.DataBind();

            DisplayPaging(totalPages);
        }
    }

    private decimal Ceil2(decimal v)
    {
        if (v <= 0) return 0m;
        return Math.Ceiling(v * 100m) / 100m;
    }

    private void DisplayPaging(int totalPages)
    {
        pnlPaging.Controls.Clear();

        if (totalPages <= 1) return;

        for (int i = 1; i <= totalPages; i++)
        {
            LinkButton lb = new LinkButton();
            lb.Text = i.ToString();
            lb.CommandArgument = i.ToString();
            lb.Command += Page_Changed;

            if (i == CurrentPage)
            {
                lb.Enabled = false;
                lb.CssClass = "current-page";
            }

            pnlPaging.Controls.Add(lb);
            pnlPaging.Controls.Add(new LiteralControl(" "));
        }
    }

    private void LoadTimeline(dbDataContext db, string taiKhoan, bool isOwner)
    {
        if (string.IsNullOrWhiteSpace(taiKhoan))
        {
            phTimelineSection.Visible = false;
            phNavTimeline.Visible = false;
            return;
        }

        var posts = db.BaiViet_tbs
            .Where(p => p.nguoitao == taiKhoan
                && p.phanloai == TIMELINE_POST_TYPE
                && (p.bin == false || p.bin == null))
            .OrderByDescending(p => p.ngaytao ?? new DateTime(1753, 1, 1))
            .ToList();

        string currentUser = Convert.ToString(ViewState["taikhoan"]) ?? "";
        bool canComment = !string.IsNullOrWhiteSpace(currentUser);

        List<int> postIds = posts.Select(p => p.id).ToList();
        Dictionary<int, List<TimelineCommentItem>> commentsByPost = LoadTimelineComments(db, postIds, currentUser, taiKhoan);

        var view = posts.Select(p =>
        {
            List<TimelineCommentItem> comments = commentsByPost.ContainsKey(p.id)
                ? commentsByPost[p.id]
                : new List<TimelineCommentItem>();

            return new TimelinePostView
            {
                id = p.id,
                content_post = p.content_post,
                image = p.image,
                ngaytao = p.ngaytao,
                Comments = comments,
                CommentCount = comments.Count,
                CanComment = canComment
            };
        }).ToList();

        rptTimeline.DataSource = view;
        rptTimeline.DataBind();
        phTimelineEmpty.Visible = !posts.Any();
        phTimelineSection.Visible = posts.Any() || isOwner;
        phNavTimeline.Visible = posts.Any() || isOwner;
    }

    private void ResetTimelineForm()
    {
        txtTimelineContent.Text = "";
        hfTimelineImage.Value = "";
        imgTimelinePreview.ImageUrl = "";
        imgTimelinePreview.Style["display"] = "none";
        ViewState["timeline_edit_id"] = null;
        btnTimelineSave.Text = "Đăng bài";
        btnTimelineCancel.Visible = false;
    }

    protected void btnTimelineSave_Click(object sender, EventArgs e)
    {
        if (!ResolveIsOwner()) return;

        string content = (txtTimelineContent.Text ?? "").Trim();
        string imageUrl = (hfTimelineImage.Value ?? "").Trim();

        if (string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(imageUrl))
        {
            Helper_Tabler_cl.ShowToast(this.Page, "Vui lòng nhập nội dung hoặc chọn ảnh.", "warning", true, 2500, "Thông báo");
            return;
        }

        string taiKhoan = ResolveProfileUser();
        if (string.IsNullOrWhiteSpace(taiKhoan)) return;

        using (dbDataContext db = new dbDataContext())
        {
            int editId = 0;
            if (ViewState["timeline_edit_id"] != null)
                int.TryParse(ViewState["timeline_edit_id"].ToString(), out editId);

            if (editId > 0)
            {
                var post = db.BaiViet_tbs.FirstOrDefault(p => p.id == editId && p.nguoitao == taiKhoan && p.phanloai == TIMELINE_POST_TYPE);
                if (post != null)
                {
                    post.content_post = content;
                    post.image = imageUrl;
                }
            }
            else
            {
                BaiViet_tb post = new BaiViet_tb
                {
                    name = "Bài viết",
                    name_en = "home-post-" + DateTime.Now.Ticks,
                    content_post = content,
                    image = imageUrl,
                    bin = false,
                    ngaytao = DateTime.Now,
                    nguoitao = taiKhoan,
                    phanloai = TIMELINE_POST_TYPE
                };
                db.BaiViet_tbs.InsertOnSubmit(post);
            }

            db.SubmitChanges();
        }

        ResetTimelineForm();
        using (dbDataContext db = new dbDataContext())
        {
            LoadTimeline(db, taiKhoan, true);
        }
    }

    protected void btnTimelineCancel_Click(object sender, EventArgs e)
    {
        ResetTimelineForm();
    }

    protected void rptTimeline_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string command = (e.CommandName ?? "").Trim().ToLowerInvariant();

        if (command == "comment")
        {
            string currentUser = PortalRequest_cl.GetCurrentAccount();
            if (string.IsNullOrWhiteSpace(currentUser))
            {
                Helper_Tabler_cl.ShowToast(this.Page, "Bạn cần đăng nhập tài khoản home để bình luận.", "warning", true, 2500, "Thông báo");
                return;
            }

            int postId = 0;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out postId)) return;

            TextBox txtComment = e.Item.FindControl("txtTimelineComment") as TextBox;
            string content = (txtComment == null ? "" : (txtComment.Text ?? "")).Trim();
            if (string.IsNullOrWhiteSpace(content))
            {
                Helper_Tabler_cl.ShowToast(this.Page, "Vui lòng nhập nội dung bình luận.", "warning", true, 2500, "Thông báo");
                return;
            }

            if (content.Length > TIMELINE_COMMENT_MAX_LEN)
                content = content.Substring(0, TIMELINE_COMMENT_MAX_LEN);

            string taiKhoan = ResolveProfileUser();
            if (string.IsNullOrWhiteSpace(taiKhoan)) return;

            using (dbDataContext db = new dbDataContext())
            {
                var post = db.BaiViet_tbs.FirstOrDefault(p => p.id == postId && p.phanloai == TIMELINE_POST_TYPE && (p.bin == false || p.bin == null));
                if (post == null)
                {
                    Helper_Tabler_cl.ShowToast(this.Page, "Không tìm thấy bài viết để bình luận.", "warning", true, 2500, "Thông báo");
                    return;
                }

                string error;
                if (!TryInsertTimelineComment(db, postId, currentUser, content, out error))
                {
                    Helper_Tabler_cl.ShowToast(this.Page, string.IsNullOrWhiteSpace(error) ? "Không thể gửi bình luận." : error, "error", true, 2500, "Thông báo");
                    return;
                }

                if (txtComment != null) txtComment.Text = "";
                LoadTimeline(db, taiKhoan, ResolveIsOwner());
            }
            return;
        }

        if (!ResolveIsOwner()) return;

        int id = 0;
        if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

        string taiKhoanEdit = ResolveProfileUser();
        if (string.IsNullOrWhiteSpace(taiKhoanEdit)) return;

        using (dbDataContext db = new dbDataContext())
        {
            var post = db.BaiViet_tbs.FirstOrDefault(p => p.id == id && p.nguoitao == taiKhoanEdit && p.phanloai == TIMELINE_POST_TYPE);
            if (post == null) return;

            if (string.Equals(command, "edit", StringComparison.OrdinalIgnoreCase))
            {
                ViewState["timeline_edit_id"] = id;
                txtTimelineContent.Text = post.content_post ?? "";
                hfTimelineImage.Value = post.image ?? "";
                if (!string.IsNullOrWhiteSpace(post.image))
                {
                    imgTimelinePreview.ImageUrl = post.image;
                    imgTimelinePreview.Style["display"] = "block";
                }
                else
                {
                    imgTimelinePreview.ImageUrl = "";
                    imgTimelinePreview.Style["display"] = "none";
                }
                btnTimelineSave.Text = "Cập nhật";
                btnTimelineCancel.Visible = true;
            }
            else if (string.Equals(command, "delete", StringComparison.OrdinalIgnoreCase))
            {
                post.bin = true;
                db.SubmitChanges();
                ResetTimelineForm();
            }

            LoadTimeline(db, taiKhoanEdit, true);
        }
    }

    protected string FormatTimelineTime(object raw)
    {
        if (raw == null) return "";
        DateTime dt;
        if (!DateTime.TryParse(raw.ToString(), out dt)) return "";
        return dt.ToString("dd/MM/yyyy HH:mm");
    }

    protected string FormatTimelineContent(object raw)
    {
        string text = (raw ?? "").ToString().Trim();
        if (string.IsNullOrWhiteSpace(text)) return "";
        return HttpUtility.HtmlEncode(text).Replace("\r\n", "\n").Replace("\n", "<br/>");
    }

    protected void DeleteTimelineComment_Command(object sender, CommandEventArgs e)
    {
        string currentUser = PortalRequest_cl.GetCurrentAccount();
        if (string.IsNullOrWhiteSpace(currentUser))
        {
            Helper_Tabler_cl.ShowToast(this.Page, "Bạn cần đăng nhập tài khoản home để thao tác.", "warning", true, 2500, "Thông báo");
            return;
        }

        long commentId = 0;
        if (!long.TryParse(Convert.ToString(e.CommandArgument), out commentId))
            return;

        string profileOwner = ResolveProfileUser();
        if (string.IsNullOrWhiteSpace(profileOwner)) return;

        using (dbDataContext db = new dbDataContext())
        {
            string error;
            if (!TryDeleteTimelineComment(db, commentId, currentUser, profileOwner, out error))
            {
                Helper_Tabler_cl.ShowToast(this.Page, string.IsNullOrWhiteSpace(error) ? "Không thể xóa bình luận." : error, "warning", true, 2500, "Thông báo");
                return;
            }

            LoadTimeline(db, profileOwner, ResolveIsOwner());
        }
    }

    private Dictionary<int, List<TimelineCommentItem>> LoadTimelineComments(dbDataContext db, List<int> postIds, string currentUser, string profileOwner)
    {
        Dictionary<int, List<TimelineCommentItem>> dict = new Dictionary<int, List<TimelineCommentItem>>();
        if (postIds == null || postIds.Count == 0) return dict;

        EnsureTimelineCommentSchema(db == null ? "" : db.Connection.ConnectionString);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            List<string> placeholders = new List<string>();
            for (int i = 0; i < postIds.Count; i++)
            {
                string key = "@p" + i;
                placeholders.Add(key);
                cmd.Parameters.AddWithValue(key, postIds[i]);
            }

            cmd.CommandText = @"
SELECT c.id, c.post_id, c.taikhoan, c.noidung, c.ngaytao,
       tk.hoten, tk.anhdaidien
FROM Home_Timeline_Comment_tb c
LEFT JOIN taikhoan_tb tk ON tk.taikhoan = c.taikhoan
WHERE c.post_id IN (" + string.Join(",", placeholders) + @")
  AND (c.bin = 0 OR c.bin IS NULL)
ORDER BY c.post_id ASC, c.ngaytao ASC";

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int postId = reader["post_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["post_id"]);
                    if (postId == 0) continue;

                    if (!dict.ContainsKey(postId))
                        dict[postId] = new List<TimelineCommentItem>();

                    string tk = reader["taikhoan"] == DBNull.Value ? "" : reader["taikhoan"].ToString();
                    string hoten = reader["hoten"] == DBNull.Value ? "" : reader["hoten"].ToString();
                    string displayName = string.IsNullOrWhiteSpace(hoten) ? tk : hoten;
                    string avatar = ResolveProductImage(reader["anhdaidien"]);
                    DateTime? ngaytao = null;
                    if (reader["ngaytao"] != DBNull.Value)
                        ngaytao = Convert.ToDateTime(reader["ngaytao"]);

                    bool canDelete = !string.IsNullOrWhiteSpace(currentUser)
                        && (string.Equals(currentUser, tk, StringComparison.OrdinalIgnoreCase)
                            || string.Equals(currentUser, profileOwner, StringComparison.OrdinalIgnoreCase));

                    dict[postId].Add(new TimelineCommentItem
                    {
                        Id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]),
                        PostId = postId,
                        TaiKhoan = tk,
                        DisplayName = displayName,
                        Avatar = avatar,
                        Content = reader["noidung"] == DBNull.Value ? "" : reader["noidung"].ToString(),
                        NgayTao = ngaytao,
                        CanDelete = canDelete,
                        ProfileUrl = ShopSlug_cl.GetPublicUrlByTaiKhoan(db, tk)
                    });
                }
            }
        }

        foreach (int postId in postIds)
        {
            if (!dict.ContainsKey(postId))
                continue;

            List<TimelineCommentItem> list = dict[postId];
            if (list.Count > TIMELINE_COMMENT_MAX_PER_POST)
            {
                dict[postId] = list.Skip(Math.Max(0, list.Count - TIMELINE_COMMENT_MAX_PER_POST)).ToList();
            }
        }

        return dict;
    }

    private bool TryInsertTimelineComment(dbDataContext db, int postId, string taiKhoan, string content, out string error)
    {
        error = "";
        if (string.IsNullOrWhiteSpace(taiKhoan))
        {
            error = "Bạn cần đăng nhập tài khoản home.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(content))
        {
            error = "Vui lòng nhập nội dung bình luận.";
            return false;
        }

        EnsureTimelineCommentSchema(db.Connection.ConnectionString);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
INSERT INTO Home_Timeline_Comment_tb (post_id, taikhoan, noidung, ngaytao, bin)
VALUES (@post_id, @taikhoan, @noidung, @ngaytao, 0)";
            cmd.Parameters.AddWithValue("@post_id", postId);
            cmd.Parameters.AddWithValue("@taikhoan", taiKhoan);
            cmd.Parameters.AddWithValue("@noidung", content);
            cmd.Parameters.AddWithValue("@ngaytao", DateTime.Now);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        return true;
    }

    private bool TryDeleteTimelineComment(dbDataContext db, long commentId, string currentUser, string profileOwner, out string error)
    {
        error = "";
        if (string.IsNullOrWhiteSpace(currentUser))
        {
            error = "Bạn cần đăng nhập tài khoản home.";
            return false;
        }

        EnsureTimelineCommentSchema(db.Connection.ConnectionString);

        using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"SELECT taikhoan FROM Home_Timeline_Comment_tb WHERE id = @id AND (bin = 0 OR bin IS NULL)";
            cmd.Parameters.AddWithValue("@id", commentId);
            conn.Open();
            object rawOwner = cmd.ExecuteScalar();
            string owner = rawOwner == null ? "" : rawOwner.ToString();

            if (string.IsNullOrWhiteSpace(owner))
            {
                error = "Bình luận không tồn tại.";
                return false;
            }

            bool canDelete = string.Equals(currentUser, owner, StringComparison.OrdinalIgnoreCase)
                || string.Equals(currentUser, profileOwner, StringComparison.OrdinalIgnoreCase);

            if (!canDelete)
            {
                error = "Bạn không có quyền xóa bình luận này.";
                return false;
            }

            cmd.CommandText = "UPDATE Home_Timeline_Comment_tb SET bin = 1 WHERE id = @id";
            cmd.ExecuteNonQuery();
        }

        return true;
    }

    private void EnsureTimelineCommentSchema(string connectionString)
    {
        if (_timelineCommentSchemaEnsured || string.IsNullOrWhiteSpace(connectionString))
            return;

        lock (TimelineCommentSchemaLock)
        {
            if (_timelineCommentSchemaEnsured) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Home_Timeline_Comment_tb')
BEGIN
    CREATE TABLE dbo.Home_Timeline_Comment_tb (
        id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        post_id INT NOT NULL,
        taikhoan NVARCHAR(50) NOT NULL,
        noidung NVARCHAR(1000) NOT NULL,
        ngaytao DATETIME NULL,
        bin BIT NULL
    )
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Home_Timeline_Comment_post' AND object_id = OBJECT_ID('dbo.Home_Timeline_Comment_tb'))
BEGIN
    CREATE INDEX IX_Home_Timeline_Comment_post ON dbo.Home_Timeline_Comment_tb (post_id, ngaytao DESC)
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Home_Timeline_Comment_user' AND object_id = OBJECT_ID('dbo.Home_Timeline_Comment_tb'))
BEGIN
    CREATE INDEX IX_Home_Timeline_Comment_user ON dbo.Home_Timeline_Comment_tb (taikhoan)
END";
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            _timelineCommentSchemaEnsured = true;
        }
    }

    private class TimelinePostView
    {
        public int id { get; set; }
        public string content_post { get; set; }
        public string image { get; set; }
        public DateTime? ngaytao { get; set; }
        public List<TimelineCommentItem> Comments { get; set; }
        public int CommentCount { get; set; }
        public bool CanComment { get; set; }
    }

    private class TimelineCommentItem
    {
        public long Id { get; set; }
        public int PostId { get; set; }
        public string TaiKhoan { get; set; }
        public string DisplayName { get; set; }
        public string Avatar { get; set; }
        public string Content { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool CanDelete { get; set; }
        public string ProfileUrl { get; set; }
    }

    private bool IsMyShop()
    {
        string tk = Convert.ToString(ViewState["taikhoan"]) ?? "";
        string userQuery = Convert.ToString(ViewState["user_query"]) ?? "";
        return !string.IsNullOrEmpty(tk) && tk == userQuery;
    }

    private string RenderProductBadge(object spCuaMinhObj, bool isService)
    {
        var badges = new List<string>();
        if (isService)
            badges.Add("<span class='badge bg-indigo-lt text-indigo'>Dịch vụ</span>");
        badges.Add("<span class='badge bg-green-lt text-green'>SP của gian hàng đối tác</span>");
        return string.Join(" ", badges);
    }

    private static bool IsMissingDonHangStatusColumnError(SqlException ex)
    {
        if (ex == null)
            return false;

        string message = ex.Message ?? "";
        return message.IndexOf("Invalid column name 'exchange_status'", StringComparison.OrdinalIgnoreCase) >= 0
            || message.IndexOf("Invalid column name 'order_status'", StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
