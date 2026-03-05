using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_lich_su_chuyen_diem_Default : System.Web.UI.Page
{
    private const string TabTieuDung = "tieu-dung";
    private const string TabUuDai = "uu-dai";
    private const string TabLaoDong = "lao-dong";
    private const string TabGanKet = "gan-ket";
    private const string TabShopOnly = "shop-only";
    private const string ViewTransfer = "transfer";
    private const int PageSize = 30;
    private const string ShopOnlyMarker = "|SHOPONLY|";
    private const int BridgeRecentLimit = 8;

    private class BridgeHistoryRowView
    {
        public long id { get; set; }
        public string time_text { get; set; }
        public string direction { get; set; }
        public decimal token_amount { get; set; }
        public decimal points_credited { get; set; }
        public string status { get; set; }
        public string tx_hash { get; set; }
        public string tx_hash_short { get; set; }
    }

    private class UuDaiYeuCauRowView
    {
        public Guid IdYeuCauRut { get; set; }
        public string IdShort { get; set; }
        public int ProfileCode { get; set; }
        public string ProfileName { get; set; }
        public string TaiKhoan { get; set; }
        public string TenHanhVi { get; set; }
        public decimal TongQuyen { get; set; }
        public string TrangThaiCode { get; set; }
        public string TrangThaiText { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public string NguoiDuyet { get; set; }
        public string GhiChu { get; set; }
    }

    // ======================================================
    // ✅ PAGE LOAD
    // ======================================================
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            string allPermissionCodes = string.Join("|", new[]
            {
                PermissionProfile_cl.HoSoTieuDung,
                PermissionProfile_cl.HoSoUuDai,
                PermissionProfile_cl.HoSoLaoDong,
                PermissionProfile_cl.HoSoGanKet,
                PermissionProfile_cl.HoSoShopOnly,
                "q1_6",
                "q1_7"
            });
            check_login_cl.check_login_admin(allPermissionCodes, allPermissionCodes);

            string tk = Session["taikhoan"] as string;
            tk = !string.IsNullOrEmpty(tk) ? mahoa_cl.giaima_Bcorn(tk) : "";
            ViewState["taikhoan"] = tk;
            ViewState["current_page_lscd"] = 1;
            ViewState["active_tab_lscd"] = NormalizeTab(Request.QueryString["tab"]);
        }

        EnsureTabUiAndPermission(!IsPostBack);
        if (!IsPostBack)
        {
            show_main();
            string requestedView = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
            if (requestedView == ViewTransfer)
            {
                ShowTransferForm();
            }
        }
    }

    private string GetCurrentUser()
    {
        return (ViewState["taikhoan"] ?? "").ToString();
    }

    private string GetActiveTab()
    {
        string tab = NormalizeTab((ViewState["active_tab_lscd"] ?? "").ToString());
        if (tab == "") tab = TabTieuDung;
        return tab;
    }

    private string NormalizeTab(string tab)
    {
        string t = (tab ?? "").Trim().ToLower();
        if (t == TabTieuDung || t == TabUuDai || t == TabLaoDong || t == TabGanKet || t == TabShopOnly)
            return t;
        return "";
    }

    private string GetTabUrl(string tab)
    {
        return ResolveUrl("~/admin/lich-su-chuyen-diem/default.aspx?tab=" + tab);
    }

    private string GetTabUrl(string tab, string view)
    {
        string url = GetTabUrl(tab);
        if (!string.IsNullOrWhiteSpace(view))
            url += "&view=" + HttpUtility.UrlEncode(view.Trim().ToLowerInvariant());
        return url;
    }

    private string GetTabCaption(string tab)
    {
        switch (tab)
        {
            case TabUuDai: return "Hồ sơ quyền ưu đãi";
            case TabLaoDong: return "Hồ sơ hành vi lao động";
            case TabGanKet: return "Hồ sơ chỉ số gắn kết";
            case TabShopOnly: return "Hồ sơ gian hàng đối tác";
            default: return "Hồ sơ quyền tiêu dùng";
        }
    }

    private bool IsHanhViRequestTab(string tab)
    {
        return tab == TabUuDai || tab == TabLaoDong || tab == TabGanKet;
    }

    private int GetProfileCodeByTab(string tab)
    {
        if (tab == TabLaoDong) return HanhViGhiNhanHoSo_cl.Profile_LaoDong;
        if (tab == TabGanKet) return HanhViGhiNhanHoSo_cl.Profile_GanKet;
        return HanhViGhiNhanHoSo_cl.Profile_UuDai;
    }

    private bool CanAccessTieuDungTab(dbDataContext db, string user)
    {
        if (PermissionProfile_cl.IsRootAdmin(user))
            return true;

        if (PermissionProfile_cl.HasPermission(db, user, PermissionProfile_cl.HoSoTieuDung))
            return true;

        return check_login_cl.CheckQuyen(db, user, "q1_6") || check_login_cl.CheckQuyen(db, user, "q1_7");
    }

    private bool CanAccessTab(dbDataContext db, string user, string tab)
    {
        if (PermissionProfile_cl.IsRootAdmin(user))
            return true;

        switch (tab)
        {
            case TabTieuDung:
                return CanAccessTieuDungTab(db, user);
            case TabUuDai:
                return PermissionProfile_cl.HasPermission(db, user, PermissionProfile_cl.HoSoUuDai);
            case TabLaoDong:
                return PermissionProfile_cl.HasPermission(db, user, PermissionProfile_cl.HoSoLaoDong);
            case TabGanKet:
                return PermissionProfile_cl.HasPermission(db, user, PermissionProfile_cl.HoSoGanKet);
            case TabShopOnly:
                return PermissionProfile_cl.HasPermission(db, user, PermissionProfile_cl.HoSoShopOnly);
            default:
                return false;
        }
    }

    private List<string> GetAllowedTabs(dbDataContext db, string user)
    {
        List<string> tabs = new List<string>();
        if (CanAccessTab(db, user, TabTieuDung)) tabs.Add(TabTieuDung);
        if (CanAccessTab(db, user, TabUuDai)) tabs.Add(TabUuDai);
        if (CanAccessTab(db, user, TabLaoDong)) tabs.Add(TabLaoDong);
        if (CanAccessTab(db, user, TabGanKet)) tabs.Add(TabGanKet);
        if (CanAccessTab(db, user, TabShopOnly)) tabs.Add(TabShopOnly);
        return tabs;
    }

    private void SetTabLinkState(HyperLink link, string tab, bool canAccess, bool isActive)
    {
        link.NavigateUrl = GetTabUrl(tab);
        link.Visible = canAccess;
        link.CssClass = isActive ? "profile-tab active" : "profile-tab";
    }

    private void EnsureTabUiAndPermission(bool redirectIfInvalid)
    {
        string currentUser = GetCurrentUser();
        using (dbDataContext db = new dbDataContext())
        {
            List<string> allowedTabs = GetAllowedTabs(db, currentUser);
            if (allowedTabs.Count == 0)
            {
                HttpContext.Current.Session["thongbao"] = thongbao_class.metro_dialog_onload(
                    "Thông báo",
                    "Bạn không đủ quyền truy cập màn hình này.",
                    "false",
                    "false",
                    "OK",
                    "alert",
                    "");
                Response.Redirect("/admin/default.aspx");
                return;
            }

            string activeTab = NormalizeTab((ViewState["active_tab_lscd"] ?? "").ToString());
            if (activeTab == "")
                activeTab = NormalizeTab(Request.QueryString["tab"]);
            if (activeTab == "")
                activeTab = TabTieuDung;

            if (!allowedTabs.Contains(activeTab))
            {
                activeTab = allowedTabs[0];
                if (redirectIfInvalid)
                {
                    Response.Redirect(GetTabUrl(activeTab));
                    return;
                }
            }

            ViewState["active_tab_lscd"] = activeTab;

            SetTabLinkState(hl_tab_tieudung, TabTieuDung, allowedTabs.Contains(TabTieuDung), activeTab == TabTieuDung);
            SetTabLinkState(hl_tab_uudai, TabUuDai, allowedTabs.Contains(TabUuDai), activeTab == TabUuDai);
            SetTabLinkState(hl_tab_laodong, TabLaoDong, allowedTabs.Contains(TabLaoDong), activeTab == TabLaoDong);
            SetTabLinkState(hl_tab_ganket, TabGanKet, allowedTabs.Contains(TabGanKet), activeTab == TabGanKet);
            SetTabLinkState(hl_tab_shoponly, TabShopOnly, allowedTabs.Contains(TabShopOnly), activeTab == TabShopOnly);

            bool canUseTransferButton = activeTab == TabTieuDung && IsAdminOrTreasury(db, currentUser);
            ph_transfer_action.Visible = canUseTransferButton;
            but_show_form_add.NavigateUrl = GetTabUrl(activeTab, ViewTransfer);
            lb_tab_caption.Text = GetTabCaption(activeTab);
        }
    }

    private string GetSearchKeyword()
    {
        string keyTop = (txt_timkiem.Text ?? "").Trim();
        string keyMain = (txt_timkiem1.Text ?? "").Trim();
        return keyTop != "" ? keyTop : keyMain;
    }

    private void SyncSearchInputs(string keyword)
    {
        string safe = (keyword ?? "").Trim();
        txt_timkiem.Text = safe;
        txt_timkiem1.Text = safe;
    }

    private void ApplyPagingState(int totalRecord, int currentPage)
    {
        int totalPage = number_of_page_class.return_total_page(totalRecord, PageSize);
        if (totalPage < 1) totalPage = 1;

        if (currentPage < 1) currentPage = 1;
        if (currentPage > totalPage) currentPage = totalPage;

        ViewState["current_page_lscd"] = currentPage;

        bool canPrev = currentPage > 1;
        bool canNext = currentPage < totalPage;

        but_quaylai.Enabled = canPrev;
        but_quaylai1.Enabled = canPrev;
        but_xemtiep.Enabled = canNext;
        but_xemtiep1.Enabled = canNext;

        int start = totalRecord == 0 ? 0 : (PageSize * currentPage - PageSize + 1);
        int end = totalRecord == 0 ? 0 : Math.Min(PageSize * currentPage, totalRecord);
        lb_show.Text = start + "-" + end + " trong số " + totalRecord.ToString("#,##0");
        lb_show_md.Text = lb_show.Text;
    }

    // ======================================================
    // ✅ CHECK QUYỀN: ADMIN hoặc TÀI KHOẢN TỔNG
    // ======================================================
    private bool IsAdminOrTreasury(dbDataContext db, string user)
    {
        if (string.IsNullOrEmpty(user)) return false;

        if (user.Equals(Helper_DongA_cl.GENESIS_WALLET, StringComparison.OrdinalIgnoreCase))
            return true;

        var acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == user);
        return acc != null && AccountType_cl.IsTreasury(acc.phanloai);
    }

    private bool HasOnlyOwnScopeForTieuDung(dbDataContext db, string user)
    {
        return check_login_cl.CheckQuyen(db, user, "q1_7")
            && !check_login_cl.CheckQuyen(db, user, "q1_6")
            && !check_login_cl.CheckQuyen(db, user, PermissionProfile_cl.HoSoTieuDung);
    }

    private string GetHoSoName(int? loaiHoSoVi, string ghichu, string activeTab)
    {
        if (activeTab == TabShopOnly)
        {
            if (loaiHoSoVi == 1) return "Tiêu dùng (Shop)";
            if (loaiHoSoVi == 2) return "Ưu đãi (Shop)";
            return "ShopOnly";
        }

        if (loaiHoSoVi == 1) return "Quyền tiêu dùng";
        if (loaiHoSoVi == 2) return "Quyền ưu đãi";
        if (loaiHoSoVi == 3) return "Hành vi lao động";
        if (loaiHoSoVi == 4) return "Chỉ số gắn kết";

        if (!string.IsNullOrEmpty(ghichu) && ghichu.Contains(ShopOnlyMarker))
            return "ShopOnly";
        return "Khác";
    }

    // ======================================================
    // ✅ HIỂN THỊ THEO TAB
    // ======================================================
    public void show_main()
    {
        try
        {
            EnsureTabUiAndPermission(false);

            string activeTab = GetActiveTab();
            pn_tab_tieudung.Visible = activeTab == TabTieuDung;
            pn_tab_hoso.Visible = activeTab != TabTieuDung;

            if (activeTab == TabTieuDung)
                show_tieudung();
            else
                show_hoso(activeTab);
        }
        catch (Exception ex)
        {
            SafeLog(ex);
        }
    }

    private void LoadBridgeSummary()
    {
        string treasuryAccount = (USDTBridgeConfig_cl.TreasuryAccount ?? "").Trim();
        lb_bridge_treasury_account.Text = treasuryAccount;
        lb_bridge_deposit_address.Text = (USDTBridgeConfig_cl.DepositAddress ?? "").Trim();
        lb_bridge_token_contract.Text = (USDTBridgeConfig_cl.TokenContract ?? "").Trim();
        lb_bridge_enabled.Text = USDTBridgeConfig_cl.Enabled ? "Bật" : "Tắt";
        lb_bridge_point_rate.Text = USDTBridgeConfig_cl.PointRatePerToken.ToString("#,##0.######");
        lb_bridge_min_confirmations.Text = USDTBridgeConfig_cl.MinConfirmations.ToString("#,##0");

        lb_bridge_treasury_name.Text = "";
        lb_bridge_points_now.Text = "0";
        lb_bridge_status_note.Text = "";
        RepeaterBridge.DataSource = null;
        RepeaterBridge.DataBind();

        using (dbDataContext db = new dbDataContext())
        {
            var treasury = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == treasuryAccount);
            if (treasury != null)
            {
                lb_bridge_treasury_name.Text = (treasury.hoten ?? "").Trim();
                lb_bridge_points_now.Text = (treasury.DongA ?? 0m).ToString("#,##0.##");
            }
            else
            {
                lb_bridge_treasury_name.Text = "(Không tìm thấy tài khoản tổng)";
            }

            try
            {
                List<BridgeHistoryRowView> viewRows = new List<BridgeHistoryRowView>();
                if (db.Connection.State != System.Data.ConnectionState.Open)
                    db.Connection.Open();

                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT TOP (@TopN)
                          id,
                          tx_hash,
                          status,
                          usdt_amount,
                          points_credited,
                          created_at,
                          credited_at
                      FROM dbo.USDT_Deposit_Bridge_tb
                      ORDER BY id DESC",
                    (SqlConnection)db.Connection))
                {
                    cmd.Parameters.AddWithValue("@TopN", BridgeRecentLimit);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime? createdAt = reader["created_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["created_at"]);
                            DateTime? creditedAt = reader["credited_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["credited_at"]);
                            string txHash = (reader["tx_hash"] ?? "").ToString();
                            decimal points = reader["points_credited"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["points_credited"]);

                            viewRows.Add(new BridgeHistoryRowView
                            {
                                id = reader["id"] == DBNull.Value ? 0L : Convert.ToInt64(reader["id"]),
                                time_text = (creditedAt ?? createdAt).HasValue
                                    ? (creditedAt ?? createdAt).Value.ToString("dd/MM/yyyy HH:mm:ss")
                                    : "",
                                direction = points < 0m ? "OUT" : "IN",
                                token_amount = reader["usdt_amount"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["usdt_amount"]),
                                points_credited = points,
                                status = (reader["status"] ?? "").ToString().Trim(),
                                tx_hash = txHash.Trim(),
                                tx_hash_short = ShortTxHash(txHash)
                            });
                        }
                    }
                }

                RepeaterBridge.DataSource = viewRows;
                RepeaterBridge.DataBind();

                if (viewRows.Count == 0)
                    lb_bridge_status_note.Text = "Chưa có giao dịch bridge nào.";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 208)
                    lb_bridge_status_note.Text = "Chưa có bảng bridge. Hãy chạy scripts/sql/create-usdt-point-bridge.sql trước.";
                else
                    lb_bridge_status_note.Text = "Không đọc được lịch sử bridge: " + ex.Message;
            }
            catch (Exception ex)
            {
                lb_bridge_status_note.Text = "Không đọc được lịch sử bridge: " + ex.Message;
            }
        }
    }

    private string ShortTxHash(string txHash)
    {
        string s = (txHash ?? "").Trim();
        if (s == "")
            return "-";
        if (s.Length <= 20)
            return s;
        return s.Substring(0, 10) + "..." + s.Substring(s.Length - 8);
    }

    private void show_tieudung()
    {
        string currentUser = GetCurrentUser();

        using (dbDataContext db = new dbDataContext())
        {
            var list_all = (from ob1 in db.LichSuChuyenDiem_tbs
                            select new
                            {
                                ob1.id,
                                ob1.ngay,
                                DongA = ob1.dongA,
                                ob1.taikhoan_chuyen,
                                ob1.taikhoan_nhan,
                                ob1.nap_rut,
                                ob1.trangtrai_rut
                            }).AsQueryable();

            if (HasOnlyOwnScopeForTieuDung(db, currentUser))
                list_all = list_all.Where(p => p.taikhoan_chuyen == currentUser || p.taikhoan_nhan == currentUser);

            string keyword = GetSearchKeyword();
            if (!string.IsNullOrEmpty(keyword))
            {
                long idSearch;
                bool isIdSearch = long.TryParse(keyword, out idSearch);

                if (isIdSearch)
                {
                    list_all = list_all.Where(p =>
                        p.id == idSearch ||
                        p.taikhoan_chuyen.Contains(keyword) ||
                        p.taikhoan_nhan.Contains(keyword));
                }
                else
                {
                    list_all = list_all.Where(p =>
                        p.taikhoan_chuyen.Contains(keyword) ||
                        p.taikhoan_nhan.Contains(keyword));
                }
            }
            SyncSearchInputs(keyword);

            list_all = list_all.OrderByDescending(p => p.ngay).ThenByDescending(p => p.id);

            int totalRecord = list_all.Count();
            int currentPage = Number_cl.Check_Int((ViewState["current_page_lscd"] ?? "1").ToString());
            ApplyPagingState(totalRecord, currentPage);
            currentPage = Number_cl.Check_Int((ViewState["current_page_lscd"] ?? "1").ToString());

            var list_split = list_all.Skip(currentPage * PageSize - PageSize).Take(PageSize);
            Repeater1.DataSource = list_split;
            Repeater1.DataBind();

            RepeaterHoSo.DataSource = null;
            RepeaterHoSo.DataBind();

            pn_yeucau_hanhvi_admin.Visible = false;
            RepeaterYeuCauHanhViAdmin.DataSource = null;
            RepeaterYeuCauHanhViAdmin.DataBind();
        }
    }

    private void show_hoso(string activeTab)
    {
        using (dbDataContext db = new dbDataContext())
        {
            var list_all = db.LichSu_DongA_tbs.AsQueryable();

            if (activeTab == TabUuDai)
            {
                list_all = list_all.Where(p =>
                    p.LoaiHoSo_Vi == 2
                    && (p.ghichu == null || !p.ghichu.Contains(ShopOnlyMarker)));
            }
            else if (activeTab == TabLaoDong)
            {
                list_all = list_all.Where(p => p.LoaiHoSo_Vi == 3);
            }
            else if (activeTab == TabGanKet)
            {
                list_all = list_all.Where(p => p.LoaiHoSo_Vi == 4);
            }
            else if (activeTab == TabShopOnly)
            {
                list_all = list_all.Where(p =>
                    (p.LoaiHoSo_Vi == 1 || p.LoaiHoSo_Vi == 2)
                    && p.ghichu != null
                    && p.ghichu.Contains(ShopOnlyMarker));
            }
            else
            {
                list_all = list_all.Where(p => p.LoaiHoSo_Vi == -999999);
            }

            string keyword = GetSearchKeyword();
            if (!string.IsNullOrEmpty(keyword))
            {
                long idSearch;
                bool isIdSearch = long.TryParse(keyword, out idSearch);

                if (isIdSearch)
                {
                    list_all = list_all.Where(p =>
                        p.id == idSearch
                        || p.taikhoan.Contains(keyword)
                        || (p.ghichu != null && p.ghichu.Contains(keyword)));
                }
                else
                {
                    list_all = list_all.Where(p =>
                        p.taikhoan.Contains(keyword)
                        || (p.ghichu != null && p.ghichu.Contains(keyword)));
                }
            }
            SyncSearchInputs(keyword);

            list_all = list_all.OrderByDescending(p => p.ngay).ThenByDescending(p => p.id);

            int totalRecord = list_all.Count();
            int currentPage = Number_cl.Check_Int((ViewState["current_page_lscd"] ?? "1").ToString());
            ApplyPagingState(totalRecord, currentPage);
            currentPage = Number_cl.Check_Int((ViewState["current_page_lscd"] ?? "1").ToString());

            var list_split = list_all.Skip(currentPage * PageSize - PageSize).Take(PageSize).ToList();
            var data = list_split.Select(p => new
            {
                p.id,
                p.ngay,
                taikhoan = p.taikhoan ?? "",
                HoSoName = GetHoSoName(p.LoaiHoSo_Vi, p.ghichu, activeTab),
                KyHieu9Text = HanhVi9Cap_cl.GetTenHanhViTheoLoai(p.KyHieu9HanhVi_1_9),
                dongA = p.dongA ?? 0m,
                CongTru = p.CongTru ?? false,
                ghichu = p.ghichu ?? ""
            });

            RepeaterHoSo.DataSource = data;
            RepeaterHoSo.DataBind();

            Repeater1.DataSource = null;
            Repeater1.DataBind();

            LoadYeuCauGhiNhanHanhViAdmin(db, activeTab, keyword);
        }
    }

    private void LoadYeuCauGhiNhanHanhViAdmin(dbDataContext db, string activeTab, string keyword)
    {
        bool showPanel = IsHanhViRequestTab(activeTab);
        pn_yeucau_hanhvi_admin.Visible = showPanel;

        if (!showPanel)
        {
            RepeaterYeuCauHanhViAdmin.DataSource = null;
            RepeaterYeuCauHanhViAdmin.DataBind();
            lb_yc_tong.Text = "0";
            lb_yc_cho_duyet.Text = "0";
            lb_yc_da_duyet.Text = "0";
            lb_yc_tu_choi.Text = "0";
            lb_yc_heading.Text = "Yêu cầu ghi nhận hành vi";
            return;
        }

        int profileCode = GetProfileCodeByTab(activeTab);
        int fromKy, toKy;
        HanhViGhiNhanHoSo_cl.TryGetHanhViRange(profileCode, out fromKy, out toKy);
        lb_yc_heading.Text = "Yêu cầu ghi nhận hành vi (" + HanhViGhiNhanHoSo_cl.GetTenHoSoByProfile(profileCode) + ")";

        string key = (keyword ?? "").Trim().ToLower();

        var rowsRaw = db.YeuCauRutQuyen_tbs
            .Where(p =>
                p.LoaiHanhVi == profileCode
                && p.KyHieu9HanhVi_1_9 >= fromKy
                && p.KyHieu9HanhVi_1_9 <= toKy)
            .OrderByDescending(p => p.NgayTao)
            .Select(p => new
            {
                IdYeuCauRut = p.IdYeuCauRut,
                TaiKhoan = p.TaiKhoan ?? "",
                p.KyHieu9HanhVi_1_9,
                TongQuyen = p.TongQuyen,
                p.TrangThai,
                NgayTao = p.NgayTao,
                NgayCapNhat = p.NgayCapNhat,
                NguoiDuyet = p.NguoiDuyet ?? "",
                GhiChu = p.GhiChu ?? ""
            })
            .ToList();

        var rows = rowsRaw.Select(p => new UuDaiYeuCauRowView
        {
            IdYeuCauRut = p.IdYeuCauRut,
            IdShort = p.IdYeuCauRut.ToString().Substring(0, 8).ToUpper(),
            ProfileCode = profileCode,
            ProfileName = HanhViGhiNhanHoSo_cl.GetTenHoSoByProfile(profileCode),
            TaiKhoan = p.TaiKhoan,
            TenHanhVi = HanhVi9Cap_cl.GetTenHanhViTheoLoai(p.KyHieu9HanhVi_1_9),
            TongQuyen = p.TongQuyen,
            TrangThaiCode = HanhViGhiNhanHoSo_cl.NormalizeTrangThai(p.TrangThai),
            TrangThaiText = HanhViGhiNhanHoSo_cl.GetTrangThaiText(p.TrangThai),
            NgayTao = p.NgayTao,
            NgayCapNhat = p.NgayCapNhat,
            NguoiDuyet = p.NguoiDuyet,
            GhiChu = p.GhiChu
        }).ToList();

        if (key != "")
        {
            rows = rows.Where(p =>
                (p.TaiKhoan ?? "").ToLower().Contains(key)
                || p.IdYeuCauRut.ToString("N").ToLower().Contains(key.Replace("-", ""))
                || (p.GhiChu ?? "").ToLower().Contains(key))
                .ToList();
        }

        lb_yc_tong.Text = rows.Count.ToString("#,##0");
        lb_yc_cho_duyet.Text = rows.Count(p => p.TrangThaiCode == HanhViGhiNhanHoSo_cl.TrangThaiChoDuyet).ToString("#,##0");
        lb_yc_da_duyet.Text = rows.Count(p => p.TrangThaiCode == HanhViGhiNhanHoSo_cl.TrangThaiDaDuyet).ToString("#,##0");
        lb_yc_tu_choi.Text = rows.Count(p => p.TrangThaiCode == HanhViGhiNhanHoSo_cl.TrangThaiTuChoi).ToString("#,##0");

        RepeaterYeuCauHanhViAdmin.DataSource = rows;
        RepeaterYeuCauHanhViAdmin.DataBind();
    }

    // ======================================================
    // ✅ PAGING
    // ======================================================
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        int currentPage = Number_cl.Check_Int((ViewState["current_page_lscd"] ?? "1").ToString());
        ViewState["current_page_lscd"] = currentPage - 1;
        show_main();
    }

    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        int currentPage = Number_cl.Check_Int((ViewState["current_page_lscd"] ?? "1").ToString());
        ViewState["current_page_lscd"] = currentPage + 1;
        show_main();
    }

    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        SyncSearchInputs(GetSearchKeyword());
        ViewState["current_page_lscd"] = 1;
        show_main();
    }

    protected void but_timkiem_Click(object sender, EventArgs e)
    {
        txt_timkiem_TextChanged(sender, e);
    }

    protected void but_xoa_timkiem_Click(object sender, EventArgs e)
    {
        SyncSearchInputs("");
        ViewState["current_page_lscd"] = 1;
        show_main();
        up_main.Update();
    }

    protected void but_refresh_bridge_Click(object sender, EventArgs e)
    {
        show_main();
        up_main.Update();
    }

    protected void but_duyet_yeucau_hanhvi_Click(object sender, EventArgs e)
    {
        string activeTab = GetActiveTab();
        if (!IsHanhViRequestTab(activeTab))
        {
            Alert("Tab hiện tại không có chức năng duyệt yêu cầu ghi nhận hành vi.");
            return;
        }

        Button btn = sender as Button;
        if (btn == null) return;

        Guid idYeuCau;
        if (!Guid.TryParse(btn.CommandArgument, out idYeuCau))
        {
            Alert("Mã yêu cầu không hợp lệ.");
            return;
        }

        int profileCode = GetProfileCodeByTab(activeTab);
        string currentUser = GetCurrentUser();
        using (dbDataContext db = new dbDataContext())
        {
            if (!CanAccessTab(db, currentUser, activeTab))
            {
                Alert("Bạn không có quyền duyệt " + HanhViGhiNhanHoSo_cl.GetTenHoSoByProfile(profileCode) + ".");
                return;
            }

            db.Connection.Open();
            var tran = db.Connection.BeginTransaction();
            db.Transaction = tran;

            try
            {
                string msg;
                bool ok = HanhViGhiNhanHoSo_cl.DuyetYeuCau(db, idYeuCau, profileCode, currentUser, out msg);
                if (!ok)
                {
                    tran.Rollback();
                    Alert(msg);
                    return;
                }

                db.SubmitChanges();
                tran.Commit();

                show_main();
                up_main.Update();
                Notifi("Đã duyệt yêu cầu ghi nhận hành vi cho " + HanhViGhiNhanHoSo_cl.GetTenHoSoByProfile(profileCode) + ".");
            }
            catch (Exception ex)
            {
                tran.Rollback();
                SafeLog(ex);
                Alert("Lỗi hệ thống: " + ex.Message);
            }
        }
    }

    protected void but_tuchoi_yeucau_hanhvi_Click(object sender, EventArgs e)
    {
        string activeTab = GetActiveTab();
        if (!IsHanhViRequestTab(activeTab))
        {
            Alert("Tab hiện tại không có chức năng từ chối yêu cầu ghi nhận hành vi.");
            return;
        }

        Button btn = sender as Button;
        if (btn == null) return;

        Guid idYeuCau;
        if (!Guid.TryParse(btn.CommandArgument, out idYeuCau))
        {
            Alert("Mã yêu cầu không hợp lệ.");
            return;
        }

        int profileCode = GetProfileCodeByTab(activeTab);
        string currentUser = GetCurrentUser();
        using (dbDataContext db = new dbDataContext())
        {
            if (!CanAccessTab(db, currentUser, activeTab))
            {
                Alert("Bạn không có quyền duyệt " + HanhViGhiNhanHoSo_cl.GetTenHoSoByProfile(profileCode) + ".");
                return;
            }

            db.Connection.Open();
            var tran = db.Connection.BeginTransaction();
            db.Transaction = tran;

            try
            {
                string msg;
                bool ok = HanhViGhiNhanHoSo_cl.TuChoiYeuCau(
                    db,
                    idYeuCau,
                    profileCode,
                    currentUser,
                    "Từ chối ghi nhận hành vi bởi admin (" + HanhViGhiNhanHoSo_cl.GetTenHoSoByProfile(profileCode) + ")",
                    out msg);
                if (!ok)
                {
                    tran.Rollback();
                    Alert(msg);
                    return;
                }

                db.SubmitChanges();
                tran.Commit();

                show_main();
                up_main.Update();
                Notifi("Đã từ chối yêu cầu ghi nhận hành vi cho " + HanhViGhiNhanHoSo_cl.GetTenHoSoByProfile(profileCode) + ".");
            }
            catch (Exception ex)
            {
                tran.Rollback();
                SafeLog(ex);
                Alert("Lỗi hệ thống: " + ex.Message);
            }
        }
    }

    // ======================================================
    // ✅ FORM CHUYỂN
    // ======================================================
    public void reset_control_add_edit()
    {
        DropDownList1.DataSource = null;
        DropDownList1.DataBind();
        txt_dongA_chuyen.Text = "";
    }

    // ======================================================
    // ✅ HIỆN FORM CHUYỂN (ADMIN hoặc TÀI KHOẢN TỔNG)
    // ======================================================
    private void ShowTransferForm()
    {
        if (GetActiveTab() != TabTieuDung)
        {
            Alert("Chức năng chỉ áp dụng ở tab Hồ sơ quyền tiêu dùng.");
            return;
        }

        reset_control_add_edit();
        string currentUser = GetCurrentUser();

        using (dbDataContext db = new dbDataContext())
        {
            var acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == currentUser);
            if (acc == null)
            {
                Alert("Không tìm thấy tài khoản.");
                return;
            }

            if (currentUser.Equals(Helper_DongA_cl.GENESIS_WALLET, StringComparison.OrdinalIgnoreCase))
            {
                var q = db.taikhoan_tbs
                    .Where(p => Helper_DongA_cl.TREASURY_WALLETS_SQL.Contains(p.taikhoan)
                        && p.phanloai != null
                        && (p.phanloai.StartsWith(AccountType_cl.Treasury) || p.phanloai.StartsWith(AccountType_cl.LegacyTreasury)))
                    .Select(p => new { taikhoan = p.taikhoan, hienthi = p.taikhoan + " - " + p.hoten })
                    .ToList();

                DropDownList1.DataSource = q;
                DropDownList1.DataTextField = "hienthi";
                DropDownList1.DataValueField = "taikhoan";
                DropDownList1.DataBind();
            }
            else if (AccountType_cl.IsTreasury(acc.phanloai))
            {
                string allowedGroup = Helper_DongA_cl.GetAllowedUserGroupByTreasuryWallet(currentUser);
                if (allowedGroup == "")
                {
                    Alert("Tài khoản tổng này chưa cấu hình nhóm user nhận.");
                    return;
                }

                var q = db.taikhoan_tbs
                    .Where(p => p.phanloai == allowedGroup)
                    .Select(p => new { taikhoan = p.taikhoan, hienthi = p.taikhoan + " - " + p.hoten })
                    .ToList();

                DropDownList1.DataSource = q;
                DropDownList1.DataTextField = "hienthi";
                DropDownList1.DataValueField = "taikhoan";
                DropDownList1.DataBind();
            }
            else
            {
                Alert("Bạn không có quyền sử dụng chức năng chuyển điểm.");
                return;
            }

            pn_add.Visible = true;
            up_add.Update();
            up_main.Visible = false;
        }
    }

    protected void but_show_form_add_Click(object sender, EventArgs e)
    {
        Response.Redirect(GetTabUrl(GetActiveTab(), ViewTransfer), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    protected void but_close_form_add_Click(object sender, EventArgs e)
    {
        Response.Redirect(GetTabUrl(GetActiveTab()), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    // ======================================================
    // ✅ XÁC NHẬN CHUYỂN (ADMIN hoặc TÀI KHOẢN TỔNG)
    // ======================================================
    protected void but_add_edit_Click(object sender, EventArgs e)
    {
        if (GetActiveTab() != TabTieuDung)
        {
            Alert("Chức năng chỉ áp dụng ở tab Hồ sơ quyền tiêu dùng.");
            return;
        }

        string fromWallet = GetCurrentUser();
        decimal amount = Number_cl.Check_Decimal((txt_dongA_chuyen.Text ?? "").Trim());
        string toWallet = DropDownList1.SelectedValue != null ? DropDownList1.SelectedValue.ToString() : "";

        if (amount <= 0)
        {
            Alert("Số Quyền tiêu dùng không hợp lệ.");
            return;
        }

        if (DropDownList1.Items.FindByValue(toWallet) == null)
        {
            Alert("Dữ liệu bị thay đổi không hợp lệ.");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            db.Connection.Open();
            var tran = db.Connection.BeginTransaction();
            db.Transaction = tran;

            try
            {
                if (Helper_DongA_cl.IsSpamTransfer(db, fromWallet, toWallet, amount))
                {
                    Alert("Bạn thao tác quá nhanh. Vui lòng thử lại sau vài giây.");
                    tran.Rollback();
                    return;
                }

                var acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == fromWallet);
                if (acc == null)
                {
                    tran.Rollback();
                    Alert("Không tìm thấy tài khoản gửi.");
                    return;
                }

                string msg;
                bool ok;

                if (fromWallet.Equals(Helper_DongA_cl.GENESIS_WALLET, StringComparison.OrdinalIgnoreCase))
                {
                    ok = Helper_DongA_cl.TransferGenesisToTreasury(db, fromWallet, toWallet, amount, out msg);
                }
                else if (AccountType_cl.IsTreasury(acc.phanloai))
                {
                    ok = Helper_DongA_cl.TransferTreasuryToUser(db, fromWallet, toWallet, amount, out msg);
                }
                else
                {
                    tran.Rollback();
                    Alert("Bạn không có quyền chuyển điểm.");
                    return;
                }

                if (!ok)
                {
                    tran.Rollback();
                    Alert(msg);
                    return;
                }

                db.SubmitChanges();
                tran.Commit();

                Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Chuyển điểm thành công.", "1500", "success");
                Response.Redirect(GetTabUrl(GetActiveTab()), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                SafeLog(ex);
                Alert("Lỗi hệ thống: " + ex.Message);
            }
        }
    }

    // ======================================================
    // ✅ XÁC NHẬN RÚT (ADMIN hoặc TÀI KHOẢN TỔNG)
    // ======================================================
    protected void but_xacnhan_rutdiem_Click(object sender, EventArgs e)
    {
        if (GetActiveTab() != TabTieuDung)
        {
            Alert("Thao tác chỉ hợp lệ ở tab Hồ sơ quyền tiêu dùng.");
            return;
        }

        string currentUser = GetCurrentUser();
        using (dbDataContext db = new dbDataContext())
        {
            if (!IsAdminOrTreasury(db, currentUser))
            {
                Alert("Chỉ ADMIN hoặc tài khoản tổng mới được phép xác nhận rút điểm.");
                return;
            }

            Button button = (Button)sender;
            string txId = button.CommandArgument;

            db.Connection.Open();
            var tran = db.Connection.BeginTransaction();
            db.Transaction = tran;

            try
            {
                string msg;
                bool ok = Helper_DongA_cl.ConfirmWithdraw(db, txId, out msg);
                if (!ok)
                {
                    tran.Rollback();
                    Alert(msg);
                    return;
                }

                db.SubmitChanges();
                tran.Commit();

                show_main();
                up_main.Update();
                Notifi("Xác nhận rút điểm thành công.");
            }
            catch (Exception ex)
            {
                tran.Rollback();
                SafeLog(ex);
                Alert("Lỗi hệ thống: " + ex.Message);
            }
        }
    }

    // ======================================================
    // ✅ HỦY RÚT (ADMIN hoặc TÀI KHOẢN TỔNG)
    // ======================================================
    protected void but_huy_rutdiem_Click(object sender, EventArgs e)
    {
        if (GetActiveTab() != TabTieuDung)
        {
            Alert("Thao tác chỉ hợp lệ ở tab Hồ sơ quyền tiêu dùng.");
            return;
        }

        string currentUser = GetCurrentUser();
        using (dbDataContext db = new dbDataContext())
        {
            if (!IsAdminOrTreasury(db, currentUser))
            {
                Alert("Chỉ ADMIN hoặc tài khoản tổng mới được phép hủy lệnh rút.");
                return;
            }

            Button button = (Button)sender;
            string txId = button.CommandArgument;

            db.Connection.Open();
            var tran = db.Connection.BeginTransaction();
            db.Transaction = tran;

            try
            {
                string msg;
                bool ok = Helper_DongA_cl.CancelWithdraw(db, txId, out msg);
                if (!ok)
                {
                    tran.Rollback();
                    Alert(msg);
                    return;
                }

                db.SubmitChanges();
                tran.Commit();

                show_main();
                up_main.Update();
                Notifi("Hủy lệnh rút thành công.");
            }
            catch (Exception ex)
            {
                tran.Rollback();
                SafeLog(ex);
                Alert("Lỗi hệ thống: " + ex.Message);
            }
        }
    }

    // ======================================================
    // ✅ UI HELPERS
    // ======================================================
    private void Alert(string msg)
    {
        msg = HttpUtility.JavaScriptStringEncode(msg ?? "");
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
            thongbao_class.metro_dialog("Thông báo", msg, "false", "false", "OK", "alert", ""), true);
    }

    private void Notifi(string msg)
    {
        msg = HttpUtility.JavaScriptStringEncode(msg ?? "");
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
            thongbao_class.metro_notifi("Thông báo", msg, "1500", "success"), true);
    }

    // ======================================================
    // ✅ LOG SAFE
    // ======================================================
    private void SafeLog(Exception ex)
    {
        try
        {
            string tk = Session["taikhoan"] as string;
            tk = !string.IsNullOrEmpty(tk) ? mahoa_cl.giaima_Bcorn(tk) : "";
            Log_cl.Add_Log(ex.Message, tk, ex.StackTrace);
        }
        catch
        {
        }
    }
}
