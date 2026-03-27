using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_uc_menu_top_uc : System.Web.UI.UserControl
{
    private const string TopViewQueryKey = "topview";
    private const string TopViewChangePassword = "change-password";
    private const string PermissionManageAdminAccounts = "5";
    private const string PermissionLegacyGeneralAdmin = "1";
    private const string PermissionHomeContent = "q3_1";

    private bool GetFlag(string key)
    {
        object v = ViewState[key];
        if (v == null) return false;
        bool b;
        return bool.TryParse(v.ToString(), out b) && b;
    }

    private void SetFlag(string key, bool value)
    {
        ViewState[key] = value ? "true" : "false";
    }

    private static HashSet<string> ParsePermissionTokens(string permissionRaw)
    {
        var tokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(permissionRaw))
            return tokens;

        string[] arr = permissionRaw
            .Split(new[] { ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string token in arr)
        {
            string t = (token ?? "").Trim();
            if (t != "")
                tokens.Add(t);
        }
        return tokens;
    }

    private void BuildMenuPermissionFlags(taikhoan_tb account)
    {
        bool isRoot = account != null && PermissionProfile_cl.IsRootAdmin(account.taikhoan);
        SetFlag("menu_is_root", isRoot);
        if (isRoot)
        {
            SetFlag("menu_dashboard", true);
            SetFlag("menu_admin_account", true);
            SetFlag("menu_otp", true);
            SetFlag("menu_transfer_history", true);
            SetFlag("menu_home_account", true);
            SetFlag("menu_home_approve_hanhvi", true);
            SetFlag("menu_home_issue_card", true);
            SetFlag("menu_home_tier_desc", true);
            SetFlag("menu_home_sell_product", true);
            SetFlag("menu_shop_account", true);
            SetFlag("menu_shop_approve", true);
            SetFlag("menu_shop_email_template", true);
            SetFlag("menu_content_home", true);
            SetFlag("menu_content_home_text", true);
            SetFlag("menu_content_menu", true);
            SetFlag("menu_content_baiviet", true);
            SetFlag("menu_content_banner", true);
            SetFlag("menu_content_gopy", true);
            SetFlag("menu_content_thongbao", true);
            SetFlag("menu_content_tuvan", true);
            SetFlag("menu_group_admin", true);
            SetFlag("menu_group_home", true);
            SetFlag("menu_group_shop", true);
            SetFlag("menu_group_content", true);
            return;
        }

        HashSet<string> tokens = ParsePermissionTokens(account != null ? account.permission : "");
        bool legacyGeneral = tokens.Contains(PermissionLegacyGeneralAdmin);
        bool canManageAdminAccounts = tokens.Contains(PermissionManageAdminAccounts);

        bool canLegacyTransfer = PermissionProfile_cl.LegacyTieuDungPermissions.Any(code => tokens.Contains(code));
        bool canTieuDung = tokens.Contains(PermissionProfile_cl.HoSoTieuDung);
        bool canUuDai = tokens.Contains(PermissionProfile_cl.HoSoUuDai);
        bool canLaoDong = tokens.Contains(PermissionProfile_cl.HoSoLaoDong);
        bool canGanKet = tokens.Contains(PermissionProfile_cl.HoSoGanKet);
        bool canShopOnly = tokens.Contains(PermissionProfile_cl.HoSoShopOnly);
        bool canHomeContent = tokens.Contains(PermissionHomeContent);

        bool canApproveHanhVi = canUuDai || canLaoDong || canGanKet;
        // Quyền nội dung (q3_1) chỉ mở nhóm Quản lý nội dung, không mở Quản lý tài khoản home.
        bool canHomeAccount = canTieuDung || canApproveHanhVi;
        bool canTransferHistory = canLegacyTransfer || canTieuDung;

        bool showAdminAccount = canManageAdminAccounts;
        bool showOtp = legacyGeneral;
        bool showTransferHistory = legacyGeneral || canTransferHistory;
        bool showHomeAccount = legacyGeneral || canHomeAccount;
        bool showApproveHanhVi = legacyGeneral || canApproveHanhVi;
        bool showIssueCard = legacyGeneral || canTieuDung;
        bool showTierDescription = legacyGeneral || canApproveHanhVi;
        bool showSellProduct = legacyGeneral || canTieuDung;
        bool showShopAccount = legacyGeneral || canShopOnly;
        bool showShopApprove = legacyGeneral || canShopOnly;
        bool showShopEmailTemplate = legacyGeneral || canShopOnly;
        // "Trang chủ home" (Cài đặt trang chủ) chỉ dành cho admin gốc.
        bool showHomeContent = false;
        bool showOtherContent = legacyGeneral || canHomeContent;
        bool showHomeContentText = showOtherContent;

        SetFlag("menu_dashboard", false);
        SetFlag("menu_admin_account", showAdminAccount);
        SetFlag("menu_otp", showOtp);
        SetFlag("menu_transfer_history", showTransferHistory);
        SetFlag("menu_home_account", showHomeAccount);
        SetFlag("menu_home_approve_hanhvi", showApproveHanhVi);
        SetFlag("menu_home_issue_card", showIssueCard);
        SetFlag("menu_home_tier_desc", showTierDescription);
        SetFlag("menu_home_sell_product", showSellProduct);
        SetFlag("menu_shop_account", showShopAccount);
        SetFlag("menu_shop_approve", showShopApprove);
        SetFlag("menu_shop_email_template", showShopEmailTemplate);
        SetFlag("menu_content_home", showHomeContent);
        SetFlag("menu_content_home_text", showHomeContentText);
        SetFlag("menu_content_menu", showOtherContent);
        SetFlag("menu_content_baiviet", showOtherContent);
        SetFlag("menu_content_banner", showOtherContent);
        SetFlag("menu_content_gopy", showOtherContent);
        SetFlag("menu_content_thongbao", showOtherContent);
        SetFlag("menu_content_tuvan", showOtherContent);

        SetFlag("menu_group_admin", showAdminAccount || showOtp || showTransferHistory);
        SetFlag("menu_group_home", showHomeAccount || showApproveHanhVi || showIssueCard || showTierDescription || showSellProduct);
        SetFlag("menu_group_shop", showShopAccount || showShopApprove || showShopEmailTemplate);
        SetFlag("menu_group_content", showHomeContent || showOtherContent);
    }

    public bool ShowMenuGroupAdmin() { return GetFlag("menu_group_admin"); }
    public bool ShowMenuGroupHome() { return GetFlag("menu_group_home"); }
    public bool ShowMenuGroupShop() { return GetFlag("menu_group_shop"); }
    public bool ShowMenuGroupContent() { return GetFlag("menu_group_content"); }
    public bool ShowMenuDashboard() { return GetFlag("menu_dashboard"); }

    public bool ShowMenuAdminAccount() { return GetFlag("menu_admin_account"); }
    public bool ShowMenuOtp() { return GetFlag("menu_otp"); }
    public bool ShowMenuTransferHistory() { return GetFlag("menu_transfer_history"); }
    public bool ShowMenuHomeAccount() { return GetFlag("menu_home_account"); }
    public bool ShowMenuApproveHanhVi() { return GetFlag("menu_home_approve_hanhvi"); }
    public bool ShowMenuIssueCard() { return GetFlag("menu_home_issue_card"); }
    public bool ShowMenuTierDescription() { return GetFlag("menu_home_tier_desc"); }
    public bool ShowMenuSellProduct() { return GetFlag("menu_home_sell_product"); }
    public bool ShowMenuShopAccount() { return GetFlag("menu_shop_account"); }
    public bool ShowMenuShopApprove() { return GetFlag("menu_shop_approve"); }
    public bool ShowMenuShopEmailTemplate() { return GetFlag("menu_shop_email_template"); }
    public bool ShowMenuHomeContent() { return GetFlag("menu_content_home"); }
    public bool ShowMenuHomeTextContent() { return GetFlag("menu_content_home_text"); }
    public bool ShowMenuContentMenu() { return GetFlag("menu_content_menu"); }
    public bool ShowMenuContentBaiViet() { return GetFlag("menu_content_baiviet"); }
    public bool ShowMenuContentBanner() { return GetFlag("menu_content_banner"); }
    public bool ShowMenuContentGopY() { return GetFlag("menu_content_gopy"); }
    public bool ShowMenuContentThongBao() { return GetFlag("menu_content_thongbao"); }
    public bool ShowMenuContentTuVan() { return GetFlag("menu_content_tuvan"); }

    public string GetAdminHomeUrl()
    {
        return "/gianhang/admin/default.aspx";
    }

    public string MenuActive(params string[] urls)
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        foreach (string url in urls)
        {
            if (currentUrl == (url ?? "").ToLower().Trim())
                return "active";
        }
        return "";
    }

    public string MenuActiveTaiKhoanScope(string scope)
    {
        string currentUrl = (HttpContext.Current.Request.Url.AbsolutePath ?? "").ToLower().Trim();
        if (currentUrl != "/gianhang/admin/quan-ly-tai-khoan/Default.aspx")
            return "";

        string currentScope = (Request.QueryString["scope"] ?? "").Trim().ToLowerInvariant();
        string targetScope = (scope ?? "").Trim().ToLowerInvariant();
        if (currentScope == targetScope)
            return "active";
        return "";
    }

    private string GetCurrentAdminAccount()
    {
        return GianHangAdminContext_cl.ResolveDisplayAccountKey();
    }

    protected string BuildCurrentPageUrl(bool openChangePassword)
    {
        var query = HttpUtility.ParseQueryString(Request.QueryString.ToString());
        if (openChangePassword)
            query[TopViewQueryKey] = TopViewChangePassword;
        else
            query.Remove(TopViewQueryKey);

        string queryString = query.ToString();
        return Request.Url.AbsolutePath + (string.IsNullOrEmpty(queryString) ? "" : "?" + queryString);
    }

    private void RedirectTo(string url)
    {
        ScriptManager sm = ScriptManager.GetCurrent(Page);
        if (sm != null && sm.IsInAsyncPostBack)
        {
            string safeUrl = (url ?? "").Replace("'", "\\'");
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "window.location='" + safeUrl + "';", true);
            return;
        }

        Response.Redirect(url, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private string GetLogAccount()
    {
        return GetCurrentAdminAccount();
    }

    private void EnsureTopActionContext()
    {
        if (GianHangAdminContext_cl.IsHomeManagedSession())
            return;

        check_login_cl.check_login_admin("none", "none");
    }

    private void ApplyTopViewFromQuery()
    {
        string topView = (Request.QueryString[TopViewQueryKey] ?? "").Trim().ToLowerInvariant();
        if (topView != TopViewChangePassword)
            return;

        if (string.IsNullOrEmpty(ViewState["taikhoan"] as string))
            return;

        pn_doimatkhau.Visible = true;
        up_doimatkhau.Update();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                #region vô hiệu hóa timer trên một số trang có CKEditor
                ApplyMenuTimerSafe();
                #endregion


                if (Session["title"] != null)
                    ViewState["title"] = Session["title"].ToString();

                ViewState["sapxep_thongbao"] = "1";//mặc định sx thông báo theo mới nhất lên đầu
                but_sapxep_moinhat.CssClass = "info small rounded";

                using (dbDataContext db = new dbDataContext())
                {
                    show_soluong_thongbao(db);
                    lay_thongtin_nguoidung(db);
                    BindWorkspaceSwitcher(db);
                }

                ApplyTopViewFromQuery();

            }
            catch (Exception _ex)
            {
                Log_cl.Add_Log(_ex.Message, GetLogAccount(), _ex.StackTrace);
            }
        }
    }

    private void ApplyMenuTimerSafe()
    {
        try
        {
            string url = (Request.Url != null ? Request.Url.AbsolutePath : "").ToLowerInvariant();
            bool disableTimer = url == "/gianhang/admin/quan-ly-bai-viet/default.aspx"
                                || url == "/gianhang/admin/motacapbac.aspx"
                                || url == "/gianhang/admin/MoTaCapBac.aspx".ToLowerInvariant();

            Timer timer = FindControl("Timer1") as Timer;
            if (timer != null)
                timer.Enabled = !disableTimer;
        }
        catch
        {
            // ignore timer failures (Mono doesn't implement Timer.Visible)
        }
    }

    protected void Timer1_Tick(object sender, EventArgs e)
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                show_soluong_thongbao(db);
            }
        }
        catch
        {
            // ignore timer refresh failures
        }
    }

    public void lay_thongtin_nguoidung(dbDataContext db)
    {
        taikhoan_tb q = GianHangAdminContext_cl.ResolveDisplayAccount(db);
        if (q != null)
        {
            try
            {
                string displayAccount = (q.taikhoan ?? "").Trim();

                ViewState["hoten"] = q.hoten;
                ViewState["anhdaidien"] = q.anhdaidien;
                ViewState["email"] = q.email;
                ViewState["taikhoan"] = displayAccount;

                if (GianHangAdminContext_cl.IsHomeManagedSession())
                {
                    string ownerAccount = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();
                    string roleLabel = GianHangAdminContext_cl.ResolveCurrentRoleLabel();
                    bool isOwner = string.Equals(displayAccount, ownerAccount, StringComparison.OrdinalIgnoreCase);
                    string ownerDisplay = (ownerAccount ?? "").Trim();
                    if (!string.IsNullOrWhiteSpace(ownerAccount) && !string.Equals(displayAccount, ownerAccount, StringComparison.OrdinalIgnoreCase))
                    {
                        taikhoan_tb owner = RootAccount_cl.GetByAccountKey(db, ownerAccount);
                        if (owner != null && !string.IsNullOrWhiteSpace(owner.hoten))
                            ownerDisplay = owner.hoten.Trim();
                    }

                    string rolePrefix = roleLabel == "" ? "Đang tham gia" : roleLabel;
                    string label = isOwner
                        ? "Không gian của bạn"
                        : (rolePrefix + (string.IsNullOrWhiteSpace(ownerDisplay) ? "" : (" · " + ownerDisplay)));
                    string css = isOwner ? "success" : "info";
                    ViewState["phanloai_text"] = label;
                    ViewState["phanloai"] =
                        "<div class=\"button flat-button mr-1 rounded " + css + "\">" + HttpUtility.HtmlEncode(label) + "</div>";

                    if (!isOwner && !string.IsNullOrWhiteSpace(ownerDisplay))
                    {
                        ViewState["hoten"] = (q.hoten ?? "").Trim() + " · đang làm việc tại " + ownerDisplay;
                    }
                }
                else
                {
                    // Hiển thị nhãn theo hệ đăng nhập admin.
                    if (AccountType_cl.IsTreasury(q.phanloai))
                    {
                        ViewState["phanloai_text"] = "Tài khoản tổng";
                        ViewState["phanloai"] =
                            "<div class=\"button flat-button mr-1 rounded success\">Tài khoản tổng</div>";
                    }
                    else
                    {
                        ViewState["phanloai_text"] = "Nhân viên admin";
                        ViewState["phanloai"] =
                            "<div class=\"button flat-button mr-1 rounded info\">Nhân viên admin</div>";
                    }
                }


                ViewState["DongA"] = (q.DongA ?? 0).ToString("#,##0");
                BuildMenuPermissionFlags(q);
                // =============================
            }
            catch (Exception _ex)
            {
                Log_cl.Add_Log(_ex.Message, GetCurrentAdminAccount(), _ex.StackTrace);
            }
        }
    }

    private void BindWorkspaceSwitcher(dbDataContext db)
    {
        ph_workspace_switcher.Visible = false;
        lit_workspace_switcher.Text = "";
        // Không render khối thông tin cố định ở đầu dropdown nữa.
        // Chuyển đổi không gian đã có nút riêng trên top bar, còn dropdown tài khoản
        // chỉ giữ lại danh sách tab thao tác để gọn và dễ dùng hơn.
    }

    public bool HasAdminDropdownAvatar()
    {
        return !string.IsNullOrWhiteSpace(ViewState["anhdaidien"] as string);
    }

    public string GetAdminDropdownAvatarUrl()
    {
        string url = (ViewState["anhdaidien"] as string ?? "").Trim();
        return url == "" ? "/uploads/images/macdinh.jpg" : url;
    }

    public string GetAdminDropdownAccountKey()
    {
        return (ViewState["taikhoan"] as string ?? "").Trim();
    }

    public string GetAdminDropdownDisplayName()
    {
        string name = (ViewState["hoten"] as string ?? "").Trim();
        int ownerIndex = name.IndexOf(" · đang làm việc tại ", StringComparison.OrdinalIgnoreCase);
        if (ownerIndex > 0)
            name = name.Substring(0, ownerIndex).Trim();
        if (name == "")
            name = GetAdminDropdownAccountKey();
        return name;
    }

    public string GetAdminDropdownInitial()
    {
        string source = GetAdminDropdownDisplayName();
        if (string.IsNullOrWhiteSpace(source))
            source = GetAdminDropdownAccountKey();
        source = (source ?? "").Trim();
        if (source == "")
            return "A";
        return source.Substring(0, 1).ToUpperInvariant();
    }

    public string GetAdminDropdownStatusText()
    {
        string label = (ViewState["phanloai_text"] as string ?? "").Trim();
        if (label != "")
            return label;

        string html = (ViewState["phanloai"] as string ?? "").Trim();
        if (html == "")
            return "Không gian gian hàng";

        string plain = Regex.Replace(html, "<.*?>", " ");
        plain = HttpUtility.HtmlDecode(plain ?? "").Trim();
        plain = Regex.Replace(plain, "\\s+", " ").Trim();
        return plain == "" ? "Không gian gian hàng" : plain;
    }


    #region thông báo
    public void show_soluong_thongbao(dbDataContext db)
    {
        try
        {
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
            {
                lb_sl_thongbao.Text = "0";
                return;
            }

            // Đếm số lượng thông báo chưa đọc
            int soLuongThongBaoChuaDoc = db.ThongBao_tbs.Count(p => p.nguoinhan == taiKhoan && p.daxem == false && p.bin == false);

            // Cập nhật nhãn hiển thị số lượng thông báo
            if (soLuongThongBaoChuaDoc < 100)
                lb_sl_thongbao.Text = soLuongThongBaoChuaDoc.ToString();
            else
                lb_sl_thongbao.Text = "99+";
        }
        catch (Exception _ex)
        {
            Log_cl.Add_Log(_ex.Message, GetLogAccount(), _ex.StackTrace);
        }
    }

    public void show_noidung_thongbao(dbDataContext db)
    {
        try
        {
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
            {
                Repeater1.DataSource = new object[0];
                Repeater1.DataBind();
                ph_empty_thongbao.Visible = true;
                return;
            }

            var list_all = (from ob1 in db.ThongBao_tbs
                            join ob2 in db.taikhoan_tbs on ob1.nguoithongbao equals ob2.taikhoan into senderGroup
                            from ob2 in senderGroup.DefaultIfEmpty()
                            where ob1.nguoinhan == taiKhoan && ob1.bin == false
                            select new
                            {
                                ob1.id, // id thông báo
                                avt_nguoithongbao = (ob2 == null || ob2.anhdaidien == null || ob2.anhdaidien == "")
                                    ? "/uploads/images/macdinh.jpg"
                                    : ob2.anhdaidien,
                                daxem = ob1.daxem,
                                noidung = ob1.noidung ?? "",
                                thoigian = ob1.thoigian,
                                link = (ob1.link == null || ob1.link == "")
                                    ? "/gianhang/admin/default.aspx?"
                                    : (ob1.link.Contains("?") ? ob1.link + "&" : ob1.link + "?")
                            }).AsQueryable();

            if (Convert.ToString(ViewState["sapxep_thongbao"]) == "2")//lọc ra chưa đọc, mới nhất lên đầu
                list_all = list_all.Where(p => p.daxem == false).OrderByDescending(p => p.thoigian).Take(20);
            else//sx theo mới nhất lên đầu
                list_all = list_all.OrderByDescending(p => p.thoigian).Take(20);

            var result = list_all.ToList();
            // Gán dữ liệu cho Repeater
            Repeater1.DataSource = result;
            Repeater1.DataBind();
            ph_empty_thongbao.Visible = result.Count == 0;
        }
        catch (Exception _ex)
        {
            Log_cl.Add_Log(_ex.Message, GetLogAccount(), _ex.StackTrace);
            Repeater1.DataSource = new object[0];
            Repeater1.DataBind();
            ph_empty_thongbao.Visible = true;
        }
    }
    protected void but_sapxep_moinhat_Click(object sender, EventArgs e)
    {
        try
        {
            EnsureTopActionContext();
            ViewState["sapxep_thongbao"] = "1";
            but_sapxep_moinhat.CssClass = "info small rounded";
            but_sapxep_chuadoc.CssClass = "light small rounded";
            using (dbDataContext db = new dbDataContext())
            {
                show_noidung_thongbao(db);
            }
        }
        catch (Exception _ex)
        {
            Log_cl.Add_Log(_ex.Message, GetLogAccount(), _ex.StackTrace);
        }
    }

    protected void but_sapxep_chuadoc_Click(object sender, EventArgs e)
    {
        try
        {
            EnsureTopActionContext();
            ViewState["sapxep_thongbao"] = "2";
            but_sapxep_moinhat.CssClass = "light small rounded";
            but_sapxep_chuadoc.CssClass = "info small rounded";
            using (dbDataContext db = new dbDataContext())
            {
                show_noidung_thongbao(db);
            }
        }
        catch (Exception _ex)
        {
            Log_cl.Add_Log(_ex.Message, GetLogAccount(), _ex.StackTrace);
        }
    }

    protected void but_show_form_thongbao_Click(object sender, EventArgs e)
    {
        try
        {
            EnsureTopActionContext();
            using (dbDataContext db = new dbDataContext())
            {
                show_noidung_thongbao(db);
            }
            UpdatePanel2.Update();
            // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", mahoa_cl.giaima_Bcorn(Session["taikhoan"].ToString()), "1000", "warning"), true);
        }
        catch (Exception _ex)
        {
            Log_cl.Add_Log(_ex.Message, GetLogAccount(), _ex.StackTrace);
        }
    }
    protected void but_chuadoc_Click(object sender, EventArgs e)
    {
        try
        {
            EnsureTopActionContext();
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == taiKhoan && p.bin == false);
                if (q == null)
                    return;
                q.daxem = false;
                db.SubmitChanges();
                show_noidung_thongbao(db);
                show_soluong_thongbao(db);
                UpdatePanel1.Update();
            }
        }
        catch (Exception _ex)
        {
            Log_cl.Add_Log(_ex.Message, GetLogAccount(), _ex.StackTrace);
        }
    }

    protected void but_dadoc_Click(object sender, EventArgs e)
    {
        try
        {
            EnsureTopActionContext();
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == taiKhoan && p.bin == false);
                if (q == null)
                    return;
                q.daxem = true;
                db.SubmitChanges();
                show_noidung_thongbao(db);
                show_soluong_thongbao(db);
                UpdatePanel1.Update();
            }
        }
        catch (Exception _ex)
        {
            Log_cl.Add_Log(_ex.Message, GetLogAccount(), _ex.StackTrace);
        }
    }
    protected void but_xoathongbao_Click(object sender, EventArgs e)
    {
        try
        {
            EnsureTopActionContext();
            LinkButton button = (LinkButton)sender;
            string _id = button.CommandArgument;
            string taiKhoan = GetCurrentAdminAccount();
            if (string.IsNullOrEmpty(taiKhoan))
                return;
            using (dbDataContext db = new dbDataContext())
            {
                ThongBao_tb q = db.ThongBao_tbs.FirstOrDefault(p => p.id.ToString() == _id && p.nguoinhan == taiKhoan && p.bin == false);
                if (q == null)
                    return;
                q.bin = true;
                db.SubmitChanges();
                show_noidung_thongbao(db);
                show_soluong_thongbao(db);
                UpdatePanel1.Update();
            }
        }
        catch (Exception _ex)
        {
            Log_cl.Add_Log(_ex.Message, GetLogAccount(), _ex.StackTrace);
        }
    }
    #endregion

    protected void but_dangxuat_Click(object sender, EventArgs e)
    {
        if (GianHangAdminContext_cl.IsHomeManagedSession())
        {
            GianHangAdminBridge_cl.ClearLegacyAdminSession();
            Session["thongbao_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Đã thoát khỏi không gian gian hàng.", "2000", "warning");
            Response.Redirect("/home/default.aspx");
            return;
        }

        Session["taikhoan"] = "";
        Session["matkhau"] = "";
        if (Request.Cookies["cookie_userinfo_admin_bcorn"] != null)
            Response.Cookies["cookie_userinfo_admin_bcorn"].Expires = AhaTime_cl.Now.AddDays(-1);
        Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đăng xuất thành công.", "2000", "warning");
        Response.Redirect("/gianhang/admin/login.aspx");
    }
    #region ĐỔI MẬT KHẨU
    protected void but_doimatkhau_Click(object sender, EventArgs e)
    {
            using (dbDataContext db = new dbDataContext())
            {
                string _pass_old = TextBox1.Text.Trim();
            string _pass_1 = TextBox2.Text.Trim();
            string _pass_2 = TextBox3.Text.Trim();
            if (_pass_old == "" || _pass_1 == "" || _pass_2 == "")
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập đầy đủ thông tin.", "2000", "warning"), true);
                return;
            }
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
            if (q != null)
            {
                if (_pass_old != q.matkhau)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mật khẩu hiện tại không đúng.", "2000", "warning"), true);
                    return;
                }
                if (_pass_1 != _pass_2)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mật khẩu mới không trùng nhau.", "2000", "warning"), true);
                    return;
                }
                taikhoan_tb _ob = q;
                _ob.matkhau = _pass_1;
                db.SubmitChanges();
                if (GianHangAdminContext_cl.IsHomeManagedSession())
                {
                    GianHangAdminBridge_cl.ClearLegacyAdminSession();
                    Session["taikhoan_home"] = "";
                    Session["matkhau_home"] = "";
                    if (Request.Cookies["cookie_userinfo_home_bcorn"] != null)
                    {
                        HttpCookie cookie = new HttpCookie("cookie_userinfo_home_bcorn");
                        cookie.Expires = AhaTime_cl.Now.AddDays(-1);
                        cookie.Path = "/";
                        Response.Cookies.Set(cookie);
                    }

                    Session["thongbao_home"] = thongbao_class.metro_notifi_onload("Thông báo", "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.", "2000", "warning");
                    Response.Redirect("/dang-nhap");
                }
                else
                {
                    Session["taikhoan"] = "";
                    Session["matkhau"] = "";
                    if (Request.Cookies["cookie_userinfo_admin_bcorn"] != null)
                        Response.Cookies["cookie_userinfo_admin_bcorn"].Expires = AhaTime_cl.Now.AddDays(-1);
                    Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.", "2000", "warning");
                    Response.Redirect("/gianhang/admin/login.aspx");
                }
            }
            else
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng tải lại trang.", "2000", "warning"), true);
        }
    }
    protected void but_show_form_doimatkhau_Click(object sender, EventArgs e)
    {
        string taiKhoan = ViewState["taikhoan"] as string ?? "";
        if (taiKhoan != "")
        {
            string topView = (Request.QueryString[TopViewQueryKey] ?? "").Trim().ToLowerInvariant();
            if (topView == TopViewChangePassword)
                RedirectTo(BuildCurrentPageUrl(false));
            else
                RedirectTo(BuildCurrentPageUrl(true));
        }
    }
    protected void but_close_doimatkhau_Click(object sender, EventArgs e)
    {
        TextBox1.Text = ""; TextBox2.Text = ""; TextBox3.Text = "";
        RedirectTo(BuildCurrentPageUrl(false));
    }
    #endregion 

}
