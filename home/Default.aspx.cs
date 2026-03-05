using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_Default : System.Web.UI.Page
{
    private int PageSize = 20;
    private int CurrentPage
    {
        get => ViewState["CurrentPage"] != null ? (int)ViewState["CurrentPage"] : 1;
        set => ViewState["CurrentPage"] = value;
    }
    private const decimal VND_PER_A = 1000m;
    private const bool PROFILE_COMMERCE_ENABLED = true;

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
        string spCuaMinh = DataBinder.Eval(dataItem, "SPCuaMinh")?.ToString() ?? "1";
        if (litBadge != null) litBadge.Text = RenderProductBadge(spCuaMinh);

        // 2) nếu chưa login: ẩn hết action
        string tkLogin = ViewState["taikhoan"]?.ToString() ?? "";
        if (string.IsNullOrEmpty(tkLogin))
        {
            btnBanSanPham.Visible = false;
            btnTraoDoi.Visible = false;
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
        string nguoiBanGoc = DataBinder.Eval(dataItem, "NguoiBan")?.ToString() ?? "";
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
        bool laGianHang = (ViewState["laGianHangDoiTac"]?.ToString() == "1");
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

        var list_all = (from ob1 in db.BaiViet_tbs.Where(p => p.bin == false && p.phanloai == "sanpham" && p.nguoitao == ViewState["user_query"].ToString())
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
                        }).AsQueryable();

        var list_1 = (from ob in db.BanSanPhamNay_tbs.Where(p => p.taikhoan_ban == ViewState["user_query"].ToString())
                      join ob1 in db.BaiViet_tbs on ob.idsp equals ob1.id.ToString() into BonBap
                      from ob1 in BonBap.DefaultIfEmpty()
                      where ob1 != null && ob1.bin == false
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
            var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == userQuery);
            bool laGianHang = q_tk != null && ShopSlug_cl.IsShopAccount(db, q_tk);
            if (!laGianHang) return new List<string>();

            var list_all = (from ob1 in db.BaiViet_tbs
                            where ob1.bin == false &&
                                  ob1.phanloai == "sanpham" &&
                                  ob1.nguoitao == userQuery &&
                                  (
                                      ob1.name.ToLower().Contains(prefixText.ToLower()) ||
                                      ob1.name_en.ToLower().Contains(prefixText.ToLower())
                                  )
                            select new { ob1.name }).AsQueryable();

            var list_1 = (from ob in db.BanSanPhamNay_tbs.Where(p => p.taikhoan_ban == userQuery)
                          join ob1 in db.BaiViet_tbs on ob.idsp equals ob1.id.ToString() into BonBap
                          from ob1 in BonBap.DefaultIfEmpty()
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
                BanSanPhamNay_tb _ob = new BanSanPhamNay_tb();
                _ob.idsp = _idsp;
                _ob.ban_ngungban = true;
                _ob.ngaythem = AhaTime_cl.Now;
                _ob.taikhoan_ban = ViewState["taikhoan"].ToString();
                _ob.taikhoan_goc = db.BaiViet_tbs.First(p => p.id.ToString() == _idsp).nguoitao;
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

                if (!fromSlugRoute && ShopSlug_cl.IsShopAccount(db, q_tk))
                {
                    string canonicalShopUrl = ShopSlug_cl.GetPublicUrl(db, q_tk);
                    if (!string.IsNullOrEmpty(canonicalShopUrl) && canonicalShopUrl != "/")
                    {
                        Response.Redirect(canonicalShopUrl, false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                }

                ViewState["user_query"] = q_tk.taikhoan;
                string linkHoSoCongKhai = ShopSlug_cl.GetPublicUrl(db, q_tk);
                ViewState["link_hoso_congkhai"] = linkHoSoCongKhai;
                ViewState["public_profile_url"] = Request.Url.GetLeftPart(UriPartial.Authority) + linkHoSoCongKhai;

                // ✅ XÁC ĐỊNH: user_query đã là gian hàng đối tác chưa (ưu tiên: phanloai hoặc đã duyệt đăng ký)
                bool laGianHangDoiTac = ShopSlug_cl.IsShopAccount(db, q_tk);

                ViewState["laGianHangDoiTac"] = laGianHangDoiTac ? "1" : "0";

                // Trang cá nhân bên home chỉ hiển thị hồ sơ cá nhân, không hiển thị khối cửa hàng/sản phẩm.
                phCuaHang.Visible = false;
                phSanPhamCuaHang.Visible = false;

                var link = db.MangXaHoi_tbs.Where(x => x.TaiKhoan == q_tk.taikhoan).ToList();
                var linkCaNhan = link.Where(x => x.Kieu == "Cá nhân").ToList();
                rptMangXaHoiCN.DataSource = linkCaNhan;
                rptMangXaHoiCH.DataSource = link.Where(x => x.Kieu == "Cửa hàng");
                rptMangXaHoiCN.DataBind();
                rptMangXaHoiCH.DataBind();
                phNoSocialLinks.Visible = linkCaNhan.Count == 0;

                string _tk1 = Session["taikhoan_home"] as string;
                if (!string.IsNullOrEmpty(_tk1))
                {
                    ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk1);
                    #region kiểm tra nếu có chờ Trao đổi
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
                    #endregion
                }
                else
                {
                    ViewState["taikhoan"] = "";
                }
                phOwnerEditButton.Visible = ViewState["taikhoan"].ToString() == q_tk.taikhoan;

                string metaTags = $@"
                    <title>Hồ sơ {q_tk.hoten}</title>
                ";
                literal_meta.Text = metaTags;

                ViewState["taikhoan_hienthi_query"] = q_tk.taikhoan;
                ViewState["avt_query"] = string.IsNullOrWhiteSpace(q_tk.anhdaidien) ? "/uploads/images/macdinh.jpg" : q_tk.anhdaidien.Trim();
                ViewState["hoten_query"] = string.IsNullOrWhiteSpace(q_tk.hoten) ? q_tk.taikhoan : q_tk.hoten.Trim();
                ViewState["gioithieu_query"] = string.IsNullOrWhiteSpace(q_tk.gioithieu) ? "Chưa cập nhật giới thiệu." : q_tk.gioithieu.Trim();
                ViewState["email_query"] = string.IsNullOrWhiteSpace(q_tk.email) ? "Chưa cập nhật" : q_tk.email.Trim();
                ViewState["diachi_query"] = string.IsNullOrWhiteSpace(q_tk.diachi) ? "Chưa cập nhật" : q_tk.diachi.Trim();

                bool hasSdt = !string.IsNullOrWhiteSpace(q_tk.dienthoai);
                ViewState["sdt_query"] = hasSdt ? q_tk.dienthoai.Trim() : "Chưa cập nhật";
                ViewState["sdt_href_query"] = hasSdt ? ("tel:" + q_tk.dienthoai.Trim()) : "#";
                ViewState["DongA_query"] = (q_tk.DongA ?? 0m).ToString("#,##0.##");

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

                LoadDanhGia(1);
                set_dulieu_macdinh();
            }
        }
        else
        {
            int totalPages = ViewState["TotalPages"] != null ? (int)ViewState["TotalPages"] : 1;
            DisplayPaging(totalPages);
        }
    }

    #region trao đổi (đặt hàng)
    protected void txt_soluong2_TextChanged(object sender, EventArgs e)
    {
        int sl = Number_cl.Check_Int(txt_soluong2.Text.Trim());
        if (sl <= 0) sl = 0;

        decimal giaVND = 0m;
        if (ViewState["giaban_vnd"] != null)
            decimal.TryParse(ViewState["giaban_vnd"].ToString(), out giaVND);

        decimal tongVND = giaVND * sl;

        Literal11.Text = QuyDoi_VND_To_A(tongVND).ToString("#,##0.##");
        up_dathang.Update();
    }

    private string BuildExchangePageUrl(string idsp, int soLuong, string userBanCheo)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["idsp"] = idsp;
        query["qty"] = (soLuong <= 0 ? 1 : soLuong).ToString();
        if (!string.IsNullOrWhiteSpace(userBanCheo))
            query["user_bancheo"] = userBanCheo;
        query["return_url"] = (Request.RawUrl ?? "/");
        if (PortalRequest_cl.IsShopPortalRequest())
            query["shop_portal"] = "1";

        return "/home/trao-doi.aspx?" + query.ToString();
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

        Response.Redirect(BuildExchangePageUrl(idsp, 1, ""), true);
    }

    protected void but_close_form_dathang_Click(object sender, EventArgs e)
    {
        Literal9.Text = "";
        Literal10.Text = "";
        Literal11.Text = "";
        LiteralUuDaiPercent.Text = "0";

        txt_soluong2.Text = "1";
        ViewState["idsp_giohang"] = null;
        ViewState["giaban"] = null;

        pn_dathang.Visible = false;
        up_dathang.Update();
    }

    protected void but_dathang_Click(object sender, EventArgs e)
    {
        if (!EnsureProfileCommerceEnabled()) return;

        using (dbDataContext db = new dbDataContext())
        {
            string tk = (ViewState["taikhoan"] ?? "").ToString();
            if (string.IsNullOrEmpty(tk))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Bạn cần đăng nhập.", "Thông báo", true, "warning");
                return;
            }

            int sl = Number_cl.Check_Int((txt_soluong2.Text ?? "").Trim());
            if (sl <= 0)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Số lượng không hợp lệ.", "Thông báo", true, "danger");
                return;
            }

            string idsp = (ViewState["idsp_giohang"] + "");
            if (string.IsNullOrEmpty(idsp))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Không xác định được sản phẩm.", "Thông báo", true, "danger");
                return;
            }

            var sp = db.BaiViet_tbs.FirstOrDefault(p => p.id.ToString() == idsp && p.bin == false);
            if (sp == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm đã ngừng bán", "Thông báo", true, "danger");
                return;
            }

            var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (q_tk == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản không tồn tại.", "Thông báo", true, "danger");
                return;
            }

            decimal soDuA = (q_tk.DongA.HasValue) ? q_tk.DongA.Value : 0m;
            decimal soDuUuDaiA = (q_tk.Vi1That_Evocher_30PhanTram.HasValue) ? q_tk.Vi1That_Evocher_30PhanTram.Value : 0m;


            decimal giaVND = sp.giaban ?? 0m;
            decimal tongVND = giaVND * sl;

            int phanTramUuDai = 0;
            if (sp.PhanTram_GiamGia_ThanhToan_BangEvoucher.HasValue)
                phanTramUuDai = sp.PhanTram_GiamGia_ThanhToan_BangEvoucher.Value;
            if (phanTramUuDai < 0) phanTramUuDai = 0;
            if (phanTramUuDai > 50) phanTramUuDai = 50;

            decimal canTraA_Tong = QuyDoi_VND_To_A(tongVND);

            decimal tienUuDaiVND = 0m;
            decimal A_UuDai = 0m;
            if (phanTramUuDai > 0)
            {
                tienUuDaiVND = tongVND * phanTramUuDai / 100m;
                A_UuDai = QuyDoi_VND_To_A(tienUuDaiVND);
            }

            bool apDungUuDai = (phanTramUuDai > 0 && A_UuDai > 0m && soDuUuDaiA >= A_UuDai);

            decimal A_ConLai = 0m;

            if (apDungUuDai)
            {
                A_ConLai = canTraA_Tong - A_UuDai;
                if (A_ConLai < 0m) A_ConLai = 0m;

                if (A_ConLai > soDuA)
                {
                    Helper_Tabler_cl.ShowModal(
                        this.Page,
                        $"Bạn đủ ví ưu đãi nhưng Đồng A không đủ cho phần còn lại.<br/>" +
                        $"Cần <b>{A_ConLai:#,##0.##} A</b>, bạn đang có <b>{soDuA:#,##0.##} A</b>.",
                        "Thông báo",
                        true,
                        "danger"
                    );
                    return;
                }
            }
            else
            {
                if (canTraA_Tong > soDuA)
                {
                    Helper_Tabler_cl.ShowModal(
                        this.Page,
                        $"Quyền tiêu dùng của bạn không đủ.<br/>" +
                        $"Cần <b>{canTraA_Tong:#,##0.##} A</b> (≈ {tongVND:#,##0}đ), bạn đang có <b>{soDuA:#,##0.##} A</b>.",
                        "Thông báo",
                        true,
                        "danger"
                    );
                    return;
                }
            }

            DateTime now = AhaTime_cl.Now;

            DonHang_tb dh = new DonHang_tb();
            dh.ngaydat = now;
            dh.nguoimua = tk;
            dh.nguoiban = sp.nguoitao;
            dh.order_status = DonHangStateMachine_cl.Order_DaDat;
            dh.exchange_status = DonHangStateMachine_cl.Exchange_ChuaTraoDoi;
            dh.tongtien = tongVND;
            dh.hoten_nguoinhan = (txt_hoten_nguoinhan.Text ?? "").Trim();
            dh.sdt_nguoinhan = (txt_sdt_nguoinhan.Text ?? "").Trim();
            dh.diahchi_nguoinhan = (txt_diachi_nguoinhan.Text ?? "").Trim();
            dh.online_offline = true;
            dh.chothanhtoan = false;
            DonHangStateMachine_cl.SyncLegacyStatus(dh);

            db.DonHang_tbs.InsertOnSubmit(dh);
            db.SubmitChanges();

            string id_dh = dh.id.ToString();

            DonHang_ChiTiet_tb ct = new DonHang_ChiTiet_tb();
            ct.id_donhang = id_dh;
            ct.idsp = idsp;
            ct.nguoiban_goc = dh.nguoiban;
            ct.nguoiban_danglai = "";
            ct.soluong = sl;
            ct.giaban = giaVND;
            ct.thanhtien = tongVND;
            ct.PhanTram_GiamGia_ThanhToan_BangEvoucher = phanTramUuDai;

            db.DonHang_ChiTiet_tbs.InsertOnSubmit(ct);

            if (apDungUuDai)
            {
                LichSu_DongA_tb lsUuDai = new LichSu_DongA_tb();
                lsUuDai.taikhoan = tk;
                lsUuDai.dongA = A_UuDai;
                lsUuDai.ngay = now;
                lsUuDai.CongTru = false;
                lsUuDai.id_donhang = id_dh;
                lsUuDai.LoaiHoSo_Vi = 2;
                lsUuDai.ghichu =
                    $"Ưu đãi {phanTramUuDai}% đơn {id_dh}: {A_UuDai:#,##0.##}A (≈ {tienUuDaiVND:#,##0}đ, 1A={VND_PER_A:#,##0}đ)";
                db.LichSu_DongA_tbs.InsertOnSubmit(lsUuDai);

                if (q_tk.Vi1That_Evocher_30PhanTram.HasValue)
                    q_tk.Vi1That_Evocher_30PhanTram = q_tk.Vi1That_Evocher_30PhanTram.Value - A_UuDai;


                decimal conLaiVND = tongVND - tienUuDaiVND;

                LichSu_DongA_tb lsA = new LichSu_DongA_tb();
                lsA.taikhoan = tk;
                lsA.dongA = A_ConLai;
                lsA.ngay = now;
                lsA.CongTru = false;
                lsA.id_donhang = id_dh;
                lsA.LoaiHoSo_Vi = 1;
                lsA.ghichu =
                    $"Trao đổi đơn {id_dh} (phần còn lại): {A_ConLai:#,##0.##}A (≈ {conLaiVND:#,##0}đ, 1A={VND_PER_A:#,##0}đ)";
                db.LichSu_DongA_tbs.InsertOnSubmit(lsA);

                if (q_tk.DongA.HasValue)
                    q_tk.DongA = q_tk.DongA.Value - A_ConLai;
            }
            else
            {
                LichSu_DongA_tb ls = new LichSu_DongA_tb();
                ls.taikhoan = tk;
                ls.dongA = canTraA_Tong;
                ls.ngay = now;
                ls.CongTru = false;
                ls.id_donhang = id_dh;
                ls.LoaiHoSo_Vi = 1;
                ls.ghichu =
                    $"Trao đổi đơn {id_dh}: {canTraA_Tong:#,##0.##}A (≈ {tongVND:#,##0}đ, 1A={VND_PER_A:#,##0}đ)";
                db.LichSu_DongA_tbs.InsertOnSubmit(ls);

                if (q_tk.DongA.HasValue)
                    q_tk.DongA = q_tk.DongA.Value - canTraA_Tong;
            }

            // ===================== THÔNG BÁO (KHÔNG TẠO TRÙNG) =====================
            string buyerName = (q_tk != null && !string.IsNullOrEmpty(q_tk.hoten)) ? q_tk.hoten : tk;

            bool daCoThongBao = db.ThongBao_tbs.Any(x =>
                x.bin == false
                && x.nguoinhan == dh.nguoiban
                && x.link == "/home/don-ban.aspx"
                && x.noidung.Contains("ID đơn hàng: " + id_dh)
            );

            if (!daCoThongBao)
            {
                db.ThongBao_tbs.InsertOnSubmit(new ThongBao_tb
                {
                    id = Guid.NewGuid(),
                    daxem = false,
                    nguoithongbao = tk,
                    nguoinhan = dh.nguoiban,
                    link = "/home/don-ban.aspx",
                    noidung = buyerName + " vừa đặt hàng đến bạn. ID đơn hàng: " + id_dh,
                    thoigian = now,
                    bin = false
                });
            }


            db.SubmitChanges();

            Literal9.Text = "";
            Literal10.Text = "";
            Literal11.Text = "";
            LiteralUuDaiPercent.Text = "0";

            txt_soluong2.Text = "1";
            ViewState["idsp_giohang"] = null;
            ViewState["giaban"] = null;
            ViewState["giaban_vnd"] = null;

            pn_dathang.Visible = false;
            up_dathang.Update();

            string msgOk;
            if (apDungUuDai)
            {
                decimal conLaiVND = tongVND - tienUuDaiVND;
                msgOk =
                    $"Đặt hàng thành công.<br/>" +
                    $"ID đơn hàng: <b>{id_dh}</b><br/><br/>" +
                    $"Tổng đơn: <b>{tongVND:#,##0}đ</b> (≈ <b>{canTraA_Tong:#,##0.##} A</b>)<br/>" +
                    $"Ưu đãi: <b>{phanTramUuDai}%</b> → trừ ví ưu đãi <b>{A_UuDai:#,##0.##} A</b> (≈ {tienUuDaiVND:#,##0}đ)<br/>" +
                    $"Còn lại: trừ Đồng A <b>{A_ConLai:#,##0.##} A</b> (≈ {conLaiVND:#,##0}đ)<br/><br/>" +
                    $"(Tổng trừ: {A_UuDai:#,##0.##}A + {A_ConLai:#,##0.##}A = {canTraA_Tong:#,##0.##}A)";
            }
            else
            {
                if (phanTramUuDai > 0)
                {
                    msgOk =
                        $"Đặt hàng thành công.<br/>" +
                        $"ID đơn hàng: <b>{id_dh}</b><br/><br/>" +
                        $"Tổng đơn: <b>{tongVND:#,##0}đ</b> (≈ <b>{canTraA_Tong:#,##0.##} A</b>)<br/>" +
                        $"Ưu đãi: <b>{phanTramUuDai}%</b> nhưng <b>ví ưu đãi không đủ</b> → hệ thống trừ toàn bộ bằng Đồng A.<br/>" +
                        $"Đã trừ: <b>{canTraA_Tong:#,##0.##} A</b>.";
                }
                else
                {
                    msgOk =
                        $"Đặt hàng thành công.<br/>" +
                        $"ID đơn hàng: <b>{id_dh}</b><br/>" +
                        $"Đã trừ <b>{canTraA_Tong:#,##0.##} A</b> (≈ {tongVND:#,##0}đ).";
                }
            }

            Helper_Tabler_cl.ShowModal(this.Page, msgOk, "Thông báo", true, "success");
        }
    }
    #endregion

    protected bool ShouldShowIcon(object icon)
    {
        return icon != null && !string.IsNullOrEmpty(icon.ToString());
    }

    protected string GetMarginStyle(object icon)
    {
        return !ShouldShowIcon(icon) ? "margin-left: 60px;" : "";
    }

    #region thêm vào giỏ
    protected void but_themvaogio_Click(object sender, EventArgs e)
    {
        if (!EnsureProfileCommerceEnabled()) return;

        using (dbDataContext db = new dbDataContext())
        {
            Button button = (Button)sender;
            string _idsp = button.CommandArgument;

            var q = db.BaiViet_tbs.FirstOrDefault(p => p.id.ToString() == _idsp);
            if (q != null)
            {
                ViewState["idsp_giohang"] = _idsp;
                Literal1.Text = q.name;
                txt_soluong1.Text = "1";
                pn_add_cart.Visible = !pn_add_cart.Visible;
                up_add_cart.Update();
            }
        }
    }

    protected void but_close_form_addcart_Click(object sender, EventArgs e)
    {
        Literal1.Text = "";
        txt_soluong1.Text = "1"; ViewState["idsp_giohang"] = null;
        pn_add_cart.Visible = !pn_add_cart.Visible;
    }

    protected void but_add_cart_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            var q = db.GioHang_tbs.FirstOrDefault(p => p.idsp == ViewState["idsp_giohang"].ToString() && p.taikhoan == ViewState["taikhoan"].ToString());
            if (q != null)
            {
                q.soluong = q.soluong + int.Parse(txt_soluong1.Text.Trim());
                q.ngaythem = AhaTime_cl.Now;
                if (q.nguoiban_goc != ViewState["user_query"].ToString())
                    q.nguoiban_danglai = ViewState["user_query"].ToString();
                db.SubmitChanges();

                Literal1.Text = "";
                txt_soluong1.Text = "1"; ViewState["idsp_giohang"] = null;
                pn_add_cart.Visible = !pn_add_cart.Visible;

                Helper_Tabler_cl.ShowModal(this.Page, "Xử lý thành công.", "Thông báo", true, "success");
            }
            else
            {
                GioHang_tb _ob = new GioHang_tb();
                _ob.ngaythem = AhaTime_cl.Now;
                _ob.taikhoan = ViewState["taikhoan"].ToString();
                _ob.idsp = ViewState["idsp_giohang"].ToString();
                _ob.soluong = int.Parse(txt_soluong1.Text.Trim());
                _ob.nguoiban_goc = db.BaiViet_tbs.First(p => p.id.ToString() == ViewState["idsp_giohang"].ToString()).nguoitao;
                _ob.nguoiban_danglai = "";
                if (_ob.nguoiban_goc != ViewState["user_query"].ToString())
                    _ob.nguoiban_danglai = ViewState["user_query"].ToString();
                db.GioHang_tbs.InsertOnSubmit(_ob);
                db.SubmitChanges();

                Literal1.Text = "";
                txt_soluong1.Text = "1"; ViewState["idsp_giohang"] = null;
                pn_add_cart.Visible = !pn_add_cart.Visible;

                Helper_Tabler_cl.ShowModal(this.Page, "Xử lý thành công.", "Thông báo", true, "success");
            }
        }
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

    private bool IsMyShop()
    {
        string tk = ViewState["taikhoan"]?.ToString() ?? "";
        string userQuery = ViewState["user_query"]?.ToString() ?? "";
        return !string.IsNullOrEmpty(tk) && tk == userQuery;
    }

    private string RenderProductBadge(object spCuaMinhObj)
    {
        return "<span class='badge bg-green-lt text-green'>SP của shop</span>";
    }
}
