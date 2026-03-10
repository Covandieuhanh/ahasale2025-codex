using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Uc_Home_Header_uc : System.Web.UI.UserControl
{
    public string show_danhmuc_nav = "";
    public string show_danhmuc_mobile = "";

    private string GetCurrentHomeAccount()
    {
        return PortalRequest_cl.GetCurrentAccount();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // Luôn dựng lại trạng thái menu/account cho mọi request để tránh
            // lệch Visible của PlaceHolder sau postback/async postback trên mobile.
            BuildDanhMucTabler(1, 3, false, "web", "0");

            // Luôn nạp lại số dư hồ sơ ở mỗi request để không bị giữ ViewState cũ
            // (đặc biệt sau khi thay đổi logic từ DuVi* -> Vi*That).
            RefreshLoggedUserHeaderState();
        }
        catch (Exception ex)
        {
            SafeFallbackForGuestHeader();
            Log_cl.Add_Log(ex.Message, "header_uc", ex.StackTrace);
        }
    }

    private void SafeFallbackForGuestHeader()
    {
        try
        {
            show_danhmuc_nav = "";
            show_danhmuc_mobile = "";
            if (phDangNhap != null) phDangNhap.Visible = true;
            if (PlaceHolder1 != null) PlaceHolder1.Visible = true;
            if (phTopDesktopAccount != null) phTopDesktopAccount.Visible = false;
            if (PlaceHolderLogged != null) PlaceHolderLogged.Visible = false;
            if (phAccountLogoutFooter != null) phAccountLogoutFooter.Visible = false;
            if (UpdatePanelGuestCard != null) UpdatePanelGuestCard.Visible = true;
            if (phMenuHomeYeuCau != null) phMenuHomeYeuCau.Visible = false;
            if (phMenuHomeCopyLink != null) phMenuHomeCopyLink.Visible = false;
            if (phMenuHomeDoiPin != null) phMenuHomeDoiPin.Visible = false;
            if (phMenuHomeKhachHang != null) phMenuHomeKhachHang.Visible = false;
            if (phMenuHomeDonMua != null) phMenuHomeDonMua.Visible = false;
            if (phMenuHomeLichSuTraoDoi != null) phMenuHomeLichSuTraoDoi.Visible = false;
            if (phMenuShopTinhNang != null) phMenuShopTinhNang.Visible = false;
            if (phUtilityHome != null) phUtilityHome.Visible = false;
            if (phMenuHomeExtra != null) phMenuHomeExtra.Visible = false;
            if (phTopDesktopHomeUtilities != null) phTopDesktopHomeUtilities.Visible = false;
            if (phTopNotificationDesktop != null) phTopNotificationDesktop.Visible = false;
            if (phTopMobileAccount != null) phTopMobileAccount.Visible = true;
            if (phTopMobileHomeUtilities != null) phTopMobileHomeUtilities.Visible = true;
            if (phTopMobileFavorite != null) phTopMobileFavorite.Visible = true;
            if (phTopNotificationMobile != null) phTopNotificationMobile.Visible = true;
            if (badgeThongBaoDesktop != null) badgeThongBaoDesktop.Visible = false;
            if (badgeThongBaoMobile != null) badgeThongBaoMobile.Visible = false;
        }
        catch
        {
        }
    }

    private void RefreshLoggedUserHeaderState()
    {
        try
        {
            string tkEnc = PortalRequest_cl.GetCurrentAccountEncrypted();
            if (string.IsNullOrEmpty(tkEnc))
                return;

            using (dbDataContext db = new dbDataContext())
            {
                lay_thongtin_nguoidung(db);
                show_soluong_thongbao(db);
            }
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "header_uc_refresh", ex.StackTrace);
        }
    }

    private void BuildDanhMucTabler(int capBatDau, int capKetThuc, bool bin, string kyhieu, string idLoaiTru)
    {
        var li = new StringBuilder();
        var mobile = new StringBuilder();

        using (dbDataContext db = new dbDataContext())
        {
            string _tk_enc = PortalRequest_cl.GetCurrentAccountEncrypted();
            bool isShopPortalRequest = PortalRequest_cl.IsShopPortalRequest();
            bool hasHomeCredential = PortalActiveMode_cl.HasHomeCredential();
            bool hasShopCredential = PortalActiveMode_cl.HasShopCredential();
            bool homeModeActive = PortalActiveMode_cl.IsHomeActive();
            bool shopModeActive = PortalActiveMode_cl.IsShopActive();

            if (!string.IsNullOrEmpty(_tk_enc)) // có đăng nhập
            {
                phDangNhap.Visible = false;
                PlaceHolder1.Visible = false;
            if (phTopDesktopAccount != null) phTopDesktopAccount.Visible = true;
            PlaceHolderLogged.Visible = true;
            if (phAccountLogoutFooter != null) phAccountLogoutFooter.Visible = true;
                UpdatePanelGuestCard.Visible = false;

                lay_thongtin_nguoidung(db);
                show_soluong_thongbao(db);

                int tierHome = Number_cl.Check_Int((ViewState["tier_home"] ?? "0").ToString());

                // Home và Shop tách cổng đăng nhập: chỉ scope shop mới thấy menu shop-only.
                bool laGianHangDoiTac = string.Equals(
                    (ViewState["portal_scope"] ?? "").ToString(),
                    PortalScope_cl.ScopeShop,
                    StringComparison.OrdinalIgnoreCase);

                if (laGianHangDoiTac)
                {
                    if (phHoSoHomeMacDinh != null) phHoSoHomeMacDinh.Visible = false;
                    if (phHoSoLaoDong != null) phHoSoLaoDong.Visible = false;
                    if (phHoSoGanKet != null) phHoSoGanKet.Visible = false;
                    if (phTopDesktopHomeUtilities != null) phTopDesktopHomeUtilities.Visible = false;
                    if (phTopMobileHomeUtilities != null) phTopMobileHomeUtilities.Visible = false;
                    if (phTopMobileFavorite != null) phTopMobileFavorite.Visible = false;
                    if (phTopNotificationDesktop != null) phTopNotificationDesktop.Visible = false;
                    if (phTopNotificationMobile != null) phTopNotificationMobile.Visible = false;
                    if (phTopMobileAccount != null) phTopMobileAccount.Visible = true;
                }
                else
                {
                    if (phHoSoHomeMacDinh != null) phHoSoHomeMacDinh.Visible = true;
                    if (phHoSoLaoDong != null) phHoSoLaoDong.Visible = TierHome_cl.CanViewHoSo(tierHome, 3);
                    if (phHoSoGanKet != null) phHoSoGanKet.Visible = TierHome_cl.CanViewHoSo(tierHome, 4);
                    if (phTopDesktopHomeUtilities != null) phTopDesktopHomeUtilities.Visible = true;
                    if (phTopMobileHomeUtilities != null) phTopMobileHomeUtilities.Visible = true;
                    if (phTopMobileFavorite != null) phTopMobileFavorite.Visible = true;
                    if (phTopNotificationDesktop != null) phTopNotificationDesktop.Visible = true;
                    if (phTopNotificationMobile != null) phTopNotificationMobile.Visible = true;
                    if (phTopMobileAccount != null) phTopMobileAccount.Visible = true;
                }

                if (phDonBan != null) phDonBan.Visible = laGianHangDoiTac;

                // ✅ NEW: chỉ hiện 2 hồ sơ shop khi là gian hàng đối tác
                if (phHoSoShopOnly != null) phHoSoShopOnly.Visible = laGianHangDoiTac;
                if (phMenuShopTinhNang != null) phMenuShopTinhNang.Visible = laGianHangDoiTac;
                if (phMenuHomeYeuCau != null) phMenuHomeYeuCau.Visible = !laGianHangDoiTac;
                if (phMenuHomeCopyLink != null) phMenuHomeCopyLink.Visible = !laGianHangDoiTac;
                if (phMenuHomeDoiPin != null) phMenuHomeDoiPin.Visible = !laGianHangDoiTac;
                if (phMenuHomeKhachHang != null) phMenuHomeKhachHang.Visible = !laGianHangDoiTac;
                if (phMenuHomeDonMua != null) phMenuHomeDonMua.Visible = !laGianHangDoiTac;
                if (phMenuHomeLichSuTraoDoi != null) phMenuHomeLichSuTraoDoi.Visible = !laGianHangDoiTac;
                if (phUtilityHome != null) phUtilityHome.Visible = !laGianHangDoiTac;
                if (phMenuHomeExtra != null) phMenuHomeExtra.Visible = !laGianHangDoiTac;
                if (phSwitchToShop != null) phSwitchToShop.Visible = !laGianHangDoiTac && hasShopCredential;
                if (phSwitchToHome != null) phSwitchToHome.Visible = laGianHangDoiTac && hasHomeCredential;
                if (phGuestSwitchHome != null) phGuestSwitchHome.Visible = false;
            }
            else // chưa đăng nhập
            {
                phDangNhap.Visible = true;
                PlaceHolder1.Visible = !isShopPortalRequest;
                if (phTopDesktopAccount != null) phTopDesktopAccount.Visible = false;
                PlaceHolderLogged.Visible = false;
                if (phAccountLogoutFooter != null) phAccountLogoutFooter.Visible = false;
                UpdatePanelGuestCard.Visible = true;

                if (phDonBan != null) phDonBan.Visible = false;

                // ✅ NEW: chưa đăng nhập thì không hiện 2 hồ sơ shop
                if (phHoSoHomeMacDinh != null) phHoSoHomeMacDinh.Visible = false;
                if (phHoSoShopOnly != null) phHoSoShopOnly.Visible = false;
                if (phHoSoLaoDong != null) phHoSoLaoDong.Visible = false;
                if (phHoSoGanKet != null) phHoSoGanKet.Visible = false;
                if (phMenuShopTinhNang != null) phMenuShopTinhNang.Visible = false;
                if (phMenuHomeYeuCau != null) phMenuHomeYeuCau.Visible = false;
                if (phMenuHomeCopyLink != null) phMenuHomeCopyLink.Visible = false;
                if (phMenuHomeDoiPin != null) phMenuHomeDoiPin.Visible = false;
                if (phMenuHomeKhachHang != null) phMenuHomeKhachHang.Visible = false;
                if (phMenuHomeDonMua != null) phMenuHomeDonMua.Visible = false;
                if (phMenuHomeLichSuTraoDoi != null) phMenuHomeLichSuTraoDoi.Visible = false;
                if (phUtilityHome != null) phUtilityHome.Visible = false;
                if (phMenuHomeExtra != null) phMenuHomeExtra.Visible = false;
                if (phTopDesktopHomeUtilities != null) phTopDesktopHomeUtilities.Visible = false;
                if (phTopMobileHomeUtilities != null) phTopMobileHomeUtilities.Visible = !isShopPortalRequest;
                if (phTopMobileFavorite != null) phTopMobileFavorite.Visible = !isShopPortalRequest;
                if (phTopNotificationDesktop != null) phTopNotificationDesktop.Visible = false;
                if (phTopNotificationMobile != null) phTopNotificationMobile.Visible = !isShopPortalRequest;
                if (phTopMobileAccount != null) phTopMobileAccount.Visible = !isShopPortalRequest;
                badgeThongBaoDesktop.Visible = false;
                badgeThongBaoMobile.Visible = false;
                if (phSwitchToShop != null) phSwitchToShop.Visible = false;
                if (phSwitchToHome != null) phSwitchToHome.Visible = false;
                if (phGuestSwitchHome != null) phGuestSwitchHome.Visible = (!homeModeActive && shopModeActive && hasHomeCredential);
            }

            // Shop portal chỉ hiển thị nút quay về trang chủ shop ở top-nav/mobile-nav.
            string portalScope = (ViewState["portal_scope"] ?? "").ToString();
            if (string.IsNullOrEmpty(portalScope))
                portalScope = isShopPortalRequest ? PortalScope_cl.ScopeShop : PortalScope_cl.ScopeHome;

            if (string.Equals(portalScope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
            {
                show_danhmuc_nav = "";
                show_danhmuc_mobile = @"<a href=""/dang-nhap?switch=home"" class=""list-group-item list-group-item-action fw-semibold"">Chuyển sang home</a>";
                return;
            }

            // 1) tìm root "Danh mục" (level 1)
            var root = db.DanhMuc_tbs.FirstOrDefault(p =>
                p.id_level == 1 &&
                p.bin == bin &&
                p.kyhieu_danhmuc == kyhieu &&
                (
                    (p.name != null && p.name.Trim().ToLower() == "danh mục") ||
                    (p.name_en != null && (p.name_en.Trim().ToLower() == "danh-muc" || p.name_en.Trim().ToLower() == "danhmuc"))
                )
            );

            if (root == null) return;

            // 2) chỉ lấy con trực tiếp của root Danh mục làm “cấp 1” hiển thị
            var cap1 = db.DanhMuc_tbs.Where(p =>
                p.id_parent == root.id.ToString() &&
                p.bin == bin &&
                p.kyhieu_danhmuc == kyhieu
            );

            if (idLoaiTru != "0")
                cap1 = cap1.Where(p => p.id.ToString() != idLoaiTru);

            if (capKetThuc != 0)
                cap1 = cap1.Where(p => p.id_level <= capKetThuc);

            // =========================
            // DESKTOP DROPDOWN: Danh mục
            // =========================
            li.Append(@"
                <li class=""nav-item dropdown"">
                  <a class=""nav-link dropdown-toggle fw-semibold"" href=""#"" data-bs-toggle=""dropdown"" data-bs-auto-close=""outside"">
                    Danh mục
                  </a>
                  <div class=""dropdown-menu dropdown-menu-arrow dm-scroll"" style=""min-width:280px; border-radius:14px;"">
                    <div class=""dm-scroll-host"">");

            foreach (var dm1 in cap1.OrderBy(p => p.rank))
            {
                bool isRootDanhMuc =
                    (dm1.name ?? "").Trim().ToLower() == "danh mục"
                    || (dm1.name_en ?? "").Trim().ToLower() == "danh-muc"
                    || (dm1.name_en ?? "").Trim().ToLower() == "danhmuc";

                // Lấy con cấp 2 của dm1
                var cap2 = db.DanhMuc_tbs.Where(p =>
                    p.id_parent == dm1.id.ToString()
                    && p.bin == bin
                    && p.kyhieu_danhmuc == kyhieu);

                if (idLoaiTru != "0")
                    cap2 = cap2.Where(p => p.id.ToString() != idLoaiTru);

                if (capKetThuc != 0)
                    cap2 = cap2.Where(p => p.id_level <= capKetThuc);

                // ✅ Nếu dm1 là root "Danh mục" -> không hiển thị dm1, đẩy con dm2 lên
                if (isRootDanhMuc)
                {
                    foreach (var dm2 in cap2.OrderBy(p => p.rank))
                    {
                        string url2 = string.IsNullOrEmpty(dm2.url_other)
                            ? ("/" + dm2.name_en + "-" + dm2.id)
                            : dm2.url_other;

                        // Lấy con cấp 3 của dm2
                        var cap3 = db.DanhMuc_tbs.Where(p =>
                            p.id_parent == dm2.id.ToString()
                            && p.bin == bin
                            && p.kyhieu_danhmuc == kyhieu);

                        if (idLoaiTru != "0")
                            cap3 = cap3.Where(p => p.id.ToString() != idLoaiTru);

                        if (cap3.Any())
                        {
                            li.AppendFormat(@"
                            <div class=""dropend dm-cap1"">
                              <a class=""dropdown-item d-flex align-items-center"" href=""{0}"">
                                <span class=""dropdown-item-icon"">{2}</span>
                                <span class=""flex-grow-1"">{1}</span>
                                <span class=""ms-auto text-muted""><i class=""ti ti-chevron-right""></i></span>
                              </a>

                              <div class=""dropdown-menu dm-submenu"" style=""min-width:280px; border-radius:14px;"">",
                                url2,
                                HttpUtility.HtmlEncode(dm2.name),
                                GetIcon(dm2.icon_html)
                            );

                            foreach (var dm3 in cap3.OrderBy(p => p.rank))
                            {
                                string url3 = string.IsNullOrEmpty(dm3.url_other)
                                    ? ("/" + dm3.name_en + "-" + dm3.id)
                                    : dm3.url_other;

                                li.AppendFormat(@"
                                    <a class=""dropdown-item"" href=""{0}"">
                                      <span class=""dropdown-item-icon"">{2}</span>
                                      {1}
                                    </a>",
                                    url3,
                                    HttpUtility.HtmlEncode(dm3.name),
                                    GetIcon(dm3.icon_html)
                                );
                            }

                            li.Append(@"
  </div>
</div>");
                        }
                        else
                        {
                            li.AppendFormat(@"
<a class=""dropdown-item d-flex align-items-center"" href=""{0}"">
  <span class=""dropdown-item-icon"">{2}</span>
  <span class=""flex-grow-1"">{1}</span>
</a>",
                                url2,
                                HttpUtility.HtmlEncode(dm2.name),
                                GetIcon(dm2.icon_html)
                            );
                        }
                    }

                    continue;
                }

                // ===== Trường hợp dm1 bình thường =====
                string url1 = string.IsNullOrEmpty(dm1.url_other)
                    ? ("/" + dm1.name_en + "-" + dm1.id)
                    : dm1.url_other;

                if (cap2.Any())
                {
                    li.AppendFormat(@"
<div class=""dropend dm-cap1"">
  <a class=""dropdown-item d-flex align-items-center"" href=""{0}"">
    <span class=""dropdown-item-icon"">{2}</span>
    <span class=""flex-grow-1"">{1}</span>
    <span class=""ms-auto text-muted""><i class=""ti ti-chevron-right""></i></span>
  </a>

  <div class=""dropdown-menu"" style=""min-width:280px; border-radius:14px;"">",
                        url1,
                        HttpUtility.HtmlEncode(dm1.name),
                        GetIcon(dm1.icon_html)
                    );

                    foreach (var dm2 in cap2.OrderBy(p => p.rank))
                    {
                        string url2 = string.IsNullOrEmpty(dm2.url_other)
                            ? ("/" + dm2.name_en + "-" + dm2.id)
                            : dm2.url_other;

                        li.AppendFormat(@"
    <a class=""dropdown-item"" href=""{0}"">
      <span class=""dropdown-item-icon"">{2}</span>
      {1}
    </a>",
                            url2,
                            HttpUtility.HtmlEncode(dm2.name),
                            GetIcon(dm2.icon_html)
                        );
                    }

                    li.Append(@"
  </div>
</div>");
                }
                else
                {
                    li.AppendFormat(@"
<a class=""dropdown-item d-flex align-items-center"" href=""{0}"">
  <span class=""dropdown-item-icon"">{2}</span>
  <span class=""flex-grow-1"">{1}</span>
</a>",
                        url1,
                        HttpUtility.HtmlEncode(dm1.name),
                        GetIcon(dm1.icon_html)
                    );
                }
            }

            // ✅ đóng dm-scroll-host + dropdown-menu + li
            li.Append(@"
    </div>
  </div>
</li>");

            // =========================
            // MOBILE OFFCANVAS
            // =========================
            mobile.Append(@"
<div class=""list-group-item fw-semibold"">Danh mục</div>");

            foreach (var dm1 in cap1.OrderBy(p => p.rank))
            {
                bool isRootDanhMuc =
                    (dm1.name ?? "").Trim().ToLower() == "danh mục"
                    || (dm1.name_en ?? "").Trim().ToLower() == "danh-muc"
                    || (dm1.name_en ?? "").Trim().ToLower() == "danhmuc";

                var cap2 = db.DanhMuc_tbs.Where(p =>
                    p.id_parent == dm1.id.ToString()
                    && p.bin == bin
                    && p.kyhieu_danhmuc == kyhieu);

                if (idLoaiTru != "0")
                    cap2 = cap2.Where(p => p.id.ToString() != idLoaiTru);

                if (capKetThuc != 0)
                    cap2 = cap2.Where(p => p.id_level <= capKetThuc);

                if (isRootDanhMuc)
                {
                    foreach (var dm2 in cap2.OrderBy(p => p.rank))
                    {
                        string url2 = string.IsNullOrEmpty(dm2.url_other)
                            ? ("/" + dm2.name_en + "-" + dm2.id)
                            : dm2.url_other;

                        mobile.AppendFormat(@"
<a href=""{0}"" class=""list-group-item list-group-item-action ps-4"">
  <span class=""me-2"">{2}</span>{1}
</a>",
                            url2,
                            HttpUtility.HtmlEncode(dm2.name),
                            GetIcon(dm2.icon_html)
                        );
                    }
                    continue;
                }

                string url1 = string.IsNullOrEmpty(dm1.url_other)
                    ? ("/" + dm1.name_en + "-" + dm1.id)
                    : dm1.url_other;

                mobile.AppendFormat(@"
<a href=""{0}"" class=""list-group-item list-group-item-action ps-4"">
  <span class=""me-2"">{2}</span>{1}
</a>",
                    url1,
                    HttpUtility.HtmlEncode(dm1.name),
                    GetIcon(dm1.icon_html)
                );

                if (cap2.Any())
                {
                    foreach (var dm2 in cap2.OrderBy(p => p.rank))
                    {
                        string url2 = string.IsNullOrEmpty(dm2.url_other)
                            ? ("/" + dm2.name_en + "-" + dm2.id)
                            : dm2.url_other;

                        mobile.AppendFormat(@"
<a href=""{0}"" class=""list-group-item list-group-item-action ps-5 text-muted"">
  <span class=""me-2"">{2}</span>{1}
</a>",
                            url2,
                            HttpUtility.HtmlEncode(dm2.name),
                            GetIcon(dm2.icon_html)
                        );
                    }
                }
            }
        }

        show_danhmuc_nav = li.ToString();
        show_danhmuc_mobile = mobile.ToString();
    }

    public void lay_thongtin_nguoidung(dbDataContext db)
    {
        string _tk = PortalRequest_cl.GetCurrentAccountEncrypted();
        if (!string.IsNullOrEmpty(_tk))
        {
            _tk = mahoa_cl.giaima_Bcorn(_tk);

            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk);
            if (q == null) return;

            ViewState["hoten"] = q.hoten;
            ViewState["anhdaidien"] = q.anhdaidien;
            ViewState["qr_code"] = q.qr_code;
            ViewState["email"] = q.email;
            ViewState["taikhoan"] = _tk;
            string scope = PortalScope_cl.ResolveScope(q.taikhoan, q.phanloai, q.permission);
            ViewState["portal_scope"] = scope;
            ViewState["public_profile_link"] = ShopSlug_cl.GetPublicUrl(db, q);
            int tierHome = TierHome_cl.TinhTierHome(db, _tk);
            ViewState["tier_home"] = tierHome;

            // ✅ lưu raw để check nghiệp vụ
            ViewState["phanloai_raw"] = q.phanloai;

            ViewState["DongA"] = (q.DongA ?? 0m).ToString("#,##0");

            // Dropdown hồ sơ phải hiển thị SỐ DƯ THẬT đã ghi nhận (Vi*That),
            // không hiển thị phần điểm nhận/chờ xử lý (DuVi*).
            string soDuUuDaiThat = (q.Vi1That_Evocher_30PhanTram ?? 0m).ToString("#,##0.00");
            string soDuLaoDongThat = (q.Vi2That_LaoDong_50PhanTram ?? 0m).ToString("#,##0.00");
            string soDuGanKetThat = (q.Vi3That_GanKet_20PhanTram ?? 0m).ToString("#,##0.00");

            // Key mới (ưu tiên render)
            ViewState["HoSo_UuDai_Real"] = soDuUuDaiThat;
            ViewState["HoSo_LaoDong_Real"] = soDuLaoDongThat;
            ViewState["HoSo_GanKet_Real"] = soDuGanKetThat;

            // Giữ key cũ để tránh ảnh hưởng đoạn code đang dùng tên cũ.
            ViewState["DuVi1_Evocher_30PhanTram"] = soDuUuDaiThat;
            ViewState["DuVi2_LaoDong_50PhanTram"] = soDuLaoDongThat;
            ViewState["DuVi3_GanKet_20PhanTram"] = soDuGanKetThat;

            // ✅ NEW: 2 trường hồ sơ shop only (null -> 0)
            ViewState["HoSo_TieuDung_ShopOnly"] = (q.HoSo_TieuDung_ShopOnly ?? 0m).ToString("#,##0.00");
            ViewState["HoSo_UuDai_ShopOnly"] = (q.HoSo_UuDai_ShopOnly ?? 0m).ToString("#,##0.00");

            if (scope == PortalScope_cl.ScopeShop)
            {
                ViewState["phanloai"] = "<span class=\"badge rounded-pill px-3 py-2 text-dark bg-warning\">Gian hàng đối tác</span>";
            }
            else
            {
                string tenTang = TierHome_cl.GetTenTangHome(tierHome);
                if (tenTang == "Đồng hành hệ sinh thái")
                    ViewState["phanloai"] = "<span class=\"badge rounded-pill px-3 py-2 text-dark\" style=\"background-color:#ce352c;\">Đồng hành hệ sinh thái</span>";
                else if (tenTang == "Cộng tác phát triển")
                    ViewState["phanloai"] = "<span class=\"badge rounded-pill px-3 py-2 text-dark\" style=\"background-color:#f6c945;\">Cộng tác phát triển</span>";
                else
                    ViewState["phanloai"] = "<span class=\"badge rounded-pill bg-success px-3 text-dark py-2\">Khách hàng</span>";
            }
        }
    }

    private string GetIcon(string iconHtml)
    {
        if (string.IsNullOrWhiteSpace(iconHtml))
            return "<span class='ti ti-category'></span>";

        return iconHtml;
    }

    #region ✅ COPY LINK GIỚI THIỆU (NEW)
    protected void but_copy_link_gioithieu_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", false);

        string tk = Convert.ToString(ViewState["taikhoan"]) ?? "";
        if (string.IsNullOrEmpty(tk))
        {
            Helper_Tabler_cl.ShowToast(Page, "Bạn chưa đăng nhập", "warning");
            return;
        }

        string url = "https://ahasale.vn/home/page/gioi-thieu-nguoi-dung.aspx?u=" + tk;
        string safeUrl = HttpUtility.JavaScriptStringEncode(url);

        // copy clipboard (navigator.clipboard + fallback)
        string jsCopy = string.Format(@"
(function(){{
    var text = '{0}';
    function fallbackCopy(t) {{
        var ta = document.createElement('textarea');
        ta.value = t;
        ta.setAttribute('readonly', '');
        ta.style.position = 'fixed';
        ta.style.top = '-1000px';
        document.body.appendChild(ta);
        ta.select();
        try {{ document.execCommand('copy'); }} catch(e){{}}
        document.body.removeChild(ta);
    }}
    if (navigator.clipboard && navigator.clipboard.writeText) {{
        navigator.clipboard.writeText(text).catch(function(){{ fallbackCopy(text); }});
    }} else {{
        fallbackCopy(text);
    }}
}})();", safeUrl);

        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "copy_ref_link_" + Guid.NewGuid().ToString("N"), jsCopy, true);

        // toast "Đã copy" (đúng cơ chế updatepanel)
        Helper_Tabler_cl.ShowToast(Page, "Đã copy", "success", true, 3000, "Thông báo");
    }
    #endregion

    #region thông báo
    protected void Timer1_Tick(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string _tk = GetCurrentHomeAccount();
            if (!string.IsNullOrEmpty(_tk))
            {
                show_soluong_thongbao(db);
            }
        }
    }

    public void show_soluong_thongbao(dbDataContext db)
    {
        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk))
        {
            lb_sl_thongbao_desktop.Text = "0";
            lb_sl_thongbao_mobile.Text = "0";
            badgeThongBaoDesktop.Visible = false;
            badgeThongBaoMobile.Visible = false;
            lb_sl_giohang_desktop.Text = "0";
            lb_sl_giohang_mobile.Text = "0";
            badgeGioHangDesktop.Visible = false;
            badgeGioHangMobile.Visible = false;
            return;
        }

        int soLuongThongBaoChuaDoc = db.ThongBao_tbs.Count(p => p.nguoinhan == _tk && p.daxem == false && p.bin == false);
        string badgeText = soLuongThongBaoChuaDoc < 100 ? soLuongThongBaoChuaDoc.ToString() : "99+";
        bool coThongBao = soLuongThongBaoChuaDoc > 0;

        lb_sl_thongbao_desktop.Text = badgeText;
        lb_sl_thongbao_mobile.Text = badgeText;
        badgeThongBaoDesktop.Visible = coThongBao;
        badgeThongBaoMobile.Visible = coThongBao;

        int soLuongGioHang = db.GioHang_tbs.Count(p => p.taikhoan == _tk);
        string gioHangBadgeText = soLuongGioHang < 100 ? soLuongGioHang.ToString() : "99+";
        bool coGioHang = soLuongGioHang > 0;

        lb_sl_giohang_desktop.Text = gioHangBadgeText;
        lb_sl_giohang_mobile.Text = gioHangBadgeText;
        badgeGioHangDesktop.Visible = coGioHang;
        badgeGioHangMobile.Visible = coGioHang;
    }

    public void show_noidung_thongbao(dbDataContext db)
    {
        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk))
        {
            Repeater1.DataSource = new object[0];
            Repeater1.DataBind();
            ph_empty_thongbao.Visible = true;
            return;
        }

        var query = from ob1 in db.ThongBao_tbs
                    join ob2 in db.taikhoan_tbs
                        on ob1.nguoithongbao equals ob2.taikhoan into senderGroup
                    from ob2 in senderGroup.DefaultIfEmpty()
                    where ob1.nguoinhan == _tk
                          && ob1.bin == false
                    select new
                    {
                        ob1.id,
                        avt_nguoithongbao = (ob2 == null || ob2.anhdaidien == null || ob2.anhdaidien == "")
                            ? "/uploads/images/macdinh.jpg"
                            : ob2.anhdaidien,
                        daxem = ob1.daxem,
                        noidung = ob1.noidung ?? "",
                        thoigian = ob1.thoigian,
                        link = (ob1.link == null || ob1.link == "")
                            ? "/home/default.aspx?"
                            : (ob1.link.Contains("?") ? ob1.link + "&" : ob1.link + "?")
                    };

        if (Convert.ToString(ViewState["sapxep_thongbao"]) == "2")
        {
            query = query
                .Where(p => p.daxem == false)
                .OrderByDescending(p => p.thoigian);
        }
        else
        {
            query = query
                .OrderByDescending(p => p.thoigian);
        }

        var result = query.Take(20).ToList();

        Repeater1.DataSource = result;
        Repeater1.DataBind();
        ph_empty_thongbao.Visible = result.Count == 0;

        ScriptManager.RegisterStartupScript(
            this,
            GetType(),
            "openNotif",
            "showNotif();",
            true
        );
    }

    protected void but_sapxep_moinhat_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", false);
        ViewState["sapxep_thongbao"] = "1";
        but_sapxep_moinhat_desk.CssClass = "btn btn-sm btn-outline-secondary active";
        but_sapxep_chuadoc_desk.CssClass = "btn btn-sm btn-outline-secondary";

        using (dbDataContext db = new dbDataContext())
        {
            show_noidung_thongbao(db);
        }
    }

    protected void but_sapxep_chuadoc_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", false);
        ViewState["sapxep_thongbao"] = "2";
        but_sapxep_moinhat_desk.CssClass = "btn btn-sm btn-outline-secondary";
        but_sapxep_chuadoc_desk.CssClass = "btn btn-sm btn-outline-secondary active";

        using (dbDataContext db = new dbDataContext())
        {
            show_noidung_thongbao(db);
        }
    }

    protected void but_show_form_thongbao_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", false);

        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk)) return;

        using (dbDataContext db = new dbDataContext())
        {
            // đánh dấu đã xem
            var q = db.ThongBao_tbs.Where(p => p.nguoinhan == _tk && p.daxem == false && p.bin == false);
            foreach (var t in q) t.daxem = true;
            db.SubmitChanges();

            show_noidung_thongbao(db);
            show_soluong_thongbao(db);
        }

        UpdatePanel2.Update();

        ScriptManager.RegisterStartupScript(
            Page,
            Page.GetType(),
            "openNoti",
            "showNotif();",
            true
        );
    }

    protected void but_chuadoc_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", false);
        LinkButton button = (LinkButton)sender;
        string _id = button.CommandArgument;
        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == _tk && p.bin == false);
            if (q == null) return;

            q.daxem = false;
            db.SubmitChanges();

            show_noidung_thongbao(db);
            show_soluong_thongbao(db);
        }
    }

    protected void but_dadoc_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", false);
        LinkButton button = (LinkButton)sender;
        string _id = button.CommandArgument;
        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == _tk && p.bin == false);
            if (q == null) return;

            q.daxem = true;
            db.SubmitChanges();

            show_noidung_thongbao(db);
            show_soluong_thongbao(db);
        }
    }

    protected void but_xoathongbao_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", false);
        LinkButton button = (LinkButton)sender;
        string _id = button.CommandArgument;
        string _tk = GetCurrentHomeAccount();
        if (string.IsNullOrEmpty(_tk))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == _tk && p.bin == false);
            if (q == null) return;

            q.bin = true;
            db.SubmitChanges();

            show_noidung_thongbao(db);
            show_soluong_thongbao(db);
        }
    }
    #endregion

    protected void dangxuat_Click(object sender, EventArgs e)
    {
        bool isShopPortal = PortalRequest_cl.IsShopPortalRequest();
        if (isShopPortal)
        {
            Session["taikhoan_shop"] = "";
            Session["matkhau_shop"] = "";
            if (Request.Cookies["cookie_userinfo_shop_bcorn"] != null)
                Response.Cookies["cookie_userinfo_shop_bcorn"].Expires = DateTime.Now.AddDays(-1);
            Session["thongbao_shop"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng xuất thành công.", "1000", "warning");
            Response.Redirect("/shop/login.aspx");
            return;
        }

        Session["taikhoan_home"] = "";
        Session["matkhau_home"] = "";

        if (Request.Cookies["cookie_userinfo_home_bcorn"] != null)
            Response.Cookies["cookie_userinfo_home_bcorn"].Expires = DateTime.Now.AddDays(-1);

        Session["thongbao_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng xuất thành công.", "1000", "warning");
        Response.Redirect("/");
    }
}
