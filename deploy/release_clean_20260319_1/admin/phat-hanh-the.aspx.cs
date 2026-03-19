using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_phat_hanh_the : System.Web.UI.Page
{
    private const string ViewAdd = "add";
    private const int CardTypeUuDai = 1;
    private const int CardTypeTieuDung = 2;
    private const int CardTypeShopPartner = 3;
    private const int CardTypeDongHanhHeSinhThai = 4;
    private const int CardTypeLaoDong = 5;

    private string BuildListUrl()
    {
        return ResolveUrl("~/admin/phat-hanh-the.aspx");
    }

    private string BuildAddUrl()
    {
        return ResolveUrl("~/admin/phat-hanh-the.aspx?view=" + ViewAdd);
    }

    private void ShowAddPage(bool bindData)
    {
        if (bindData)
        {
            load_dropdown_taikhoan();
        }
        pn_add.Visible = true;
        up_main.Visible = false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            but_show_add.NavigateUrl = BuildAddUrl();
            lnk_back_list.NavigateUrl = BuildListUrl();
            string view = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
            bool isAddView = view == ViewAdd;

            if (isAddView)
            {
                ShowAddPage(!IsPostBack);
            }

            if (!IsPostBack)
            {
                check_login_cl.check_login_admin(PermissionProfile_cl.HoSoTieuDung, PermissionProfile_cl.HoSoTieuDung);
                ViewState["title"] = "Phát hành thẻ";
                if (!isAddView)
                    show_main();
            }
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "admin", ex.StackTrace);
            Session["thongbao"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang phát hành thẻ đang gặp lỗi dữ liệu. Vui lòng đăng nhập lại hoặc thử lại sau.", "false", "false", "OK", "alert", "");
            Response.Redirect("/admin/login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }

    // ================== HELPERS ==================
    string GetActor()
    {
        string actor = "admin";
        try
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) actor = mahoa_cl.giaima_Bcorn(_tk);
        }
        catch { }
        return actor;
    }

    // QUAN TRỌNG: nếu thongbao_class.metro_* đã có <script>...</script> thì addScriptTags = false
    void RunClientScriptSafe(string scriptOrHtmlScriptTag)
    {
        if (string.IsNullOrEmpty(scriptOrHtmlScriptTag)) return;

        bool hasScriptTag = scriptOrHtmlScriptTag.IndexOf("<script", StringComparison.OrdinalIgnoreCase) >= 0;
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(),
            scriptOrHtmlScriptTag, !hasScriptTag);
    }

    string GetTenTheByLoai(int loai)
    {
        switch (loai)
        {
            case CardTypeUuDai: return "Thẻ ưu đãi";
            case CardTypeTieuDung: return "Thẻ tiêu dùng";
            case CardTypeShopPartner: return "Thẻ gian hàng đối tác";
            case CardTypeDongHanhHeSinhThai: return "Thẻ đồng hành hệ sinh thái";
            case CardTypeLaoDong: return "Thẻ lao động";
            default: return "Thẻ #" + loai;
        }
    }

    private string ResolveAccountScope(taikhoan_tb account)
    {
        if (account == null) return "";
        return PortalScope_cl.ResolveScope(account.taikhoan, account.phanloai, account.permission);
    }

    private bool IsCardTypeAllowedForScope(string scope, int loaiThe)
    {
        if (string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
            return loaiThe == CardTypeShopPartner;

        if (string.Equals(scope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase))
            return loaiThe == CardTypeUuDai
                || loaiThe == CardTypeTieuDung
                || loaiThe == CardTypeLaoDong
                || loaiThe == CardTypeDongHanhHeSinhThai;

        return false;
    }

    private string BuildInvalidCardTypeMessage(string scope)
    {
        if (string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
            return "Tài khoản gian hàng đối tác chỉ được phát hành thẻ gian hàng đối tác.";

        if (string.Equals(scope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase))
            return "Tài khoản home chỉ được phát hành thẻ ưu đãi, thẻ tiêu dùng, thẻ lao động hoặc thẻ đồng hành hệ sinh thái.";

        return "Tài khoản này không thuộc hệ home/gian hàng đối tác nên không thể phát hành thẻ.";
    }

    private void BindCardTypesByScope(string scope)
    {
        ddl_loaithe.Items.Clear();
        ddl_loaithe.Items.Add(new ListItem("-- Chọn loại thẻ --", ""));

        if (string.Equals(scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
        {
            ddl_loaithe.Items.Add(new ListItem(GetTenTheByLoai(CardTypeShopPartner), CardTypeShopPartner.ToString()));
            ddl_loaithe.SelectedValue = CardTypeShopPartner.ToString();
            return;
        }

        if (string.Equals(scope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase))
        {
            ddl_loaithe.Items.Add(new ListItem(GetTenTheByLoai(CardTypeUuDai), CardTypeUuDai.ToString()));
            ddl_loaithe.Items.Add(new ListItem(GetTenTheByLoai(CardTypeTieuDung), CardTypeTieuDung.ToString()));
            ddl_loaithe.Items.Add(new ListItem(GetTenTheByLoai(CardTypeLaoDong), CardTypeLaoDong.ToString()));
            ddl_loaithe.Items.Add(new ListItem(GetTenTheByLoai(CardTypeDongHanhHeSinhThai), CardTypeDongHanhHeSinhThai.ToString()));
        }
    }

    // ================== UI: OPEN/CLOSE POPUP ==================
    protected void but_show_add_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin(PermissionProfile_cl.HoSoTieuDung, PermissionProfile_cl.HoSoTieuDung);
            Response.Redirect(BuildAddUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "admin", ex.StackTrace);
            RunClientScriptSafe(thongbao_class.metro_dialog("Lỗi", ex.Message, "false", "false", "OK", "alert", ""));
        }
    }

    protected void but_close_add_Click(object sender, EventArgs e)
    {
        Response.Redirect(BuildListUrl(), false);
        Context.ApplicationInstance.CompleteRequest();
    }

    // ================== DROPDOWN TÀI KHOẢN (LINQ) ==================
    void load_dropdown_taikhoan()
    {
        using (dbDataContext db = new dbDataContext())
        {
            var raw = db.taikhoan_tbs
                .Where(p => p.taikhoan != "admin"
                    && (p.phanloai == null
                        || (!p.phanloai.StartsWith(AccountType_cl.Treasury)
                            && !p.phanloai.StartsWith(AccountType_cl.LegacyTreasury))))
                .Select(p => new
                {
                    p.taikhoan,
                    p.hoten,
                    p.phanloai,
                    p.permission
                })
                .ToList();

            var list = raw
                .Select(p =>
                {
                    string scope = PortalScope_cl.ResolveScope(p.taikhoan, p.phanloai, p.permission);
                    return new
                    {
                        p.taikhoan,
                        Scope = scope,
                        Text = p.taikhoan + " - " + (p.hoten ?? "")
                    };
                })
                .Where(p => string.Equals(p.Scope, PortalScope_cl.ScopeHome, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(p.Scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.taikhoan)
                .ToList();

            ddl_taikhoan.Items.Clear();
            ddl_taikhoan.Items.Add(new ListItem("-- Chọn tài khoản --", ""));

            foreach (var item in list)
            {
                bool isShop = string.Equals(item.Scope, PortalScope_cl.ScopeShop, StringComparison.OrdinalIgnoreCase);
                ListItem li = new ListItem(item.Text + (isShop ? " [SHOP]" : " [HOME]"), item.taikhoan);
                li.Attributes["data-scope"] = item.Scope;
                ddl_taikhoan.Items.Add(li);
            }
        }
    }

    // ================== MAIN LIST (LINQ) ==================
    void show_main()
    {
        using (dbDataContext db = new dbDataContext())
        {
            var list = db.The_PhatHanh_tbs
                .OrderByDescending(p => p.NgayPhatHanh)
                .ToList();

            lb_show.Text = list.Count + " thẻ";
            Repeater1.DataSource = list;
            Repeater1.DataBind();
            up_main.Update();
        }
    }



    // ================== CREATE (LINQ + SERIALIZABLE) ==================
    protected void but_tao_the_Click(object sender, EventArgs e)
    {
        try
        {
            check_login_cl.check_login_admin(PermissionProfile_cl.HoSoTieuDung, PermissionProfile_cl.HoSoTieuDung);

            string tk = (ddl_taikhoan.SelectedValue ?? "").Trim();
            int loai;
            int.TryParse(ddl_loaithe.SelectedValue, out loai);

            if (string.IsNullOrEmpty(tk) || loai <= 0)
            {
                RunClientScriptSafe(thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn tài khoản và loại thẻ.", "false", "false", "OK", "alert", ""));
                return;
            }

            string actor = GetActor();
            DateTime now = AhaTime_cl.Now;
            string tenThe = GetTenTheByLoai(loai);

            using (dbDataContext db = new dbDataContext())
            {
                db.Connection.Open();
                var tran = db.Connection.BeginTransaction(System.Data.IsolationLevel.Serializable);
                db.Transaction = tran;

                try
                {
                    taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
                    if (acc == null)
                    {
                        try { tran.Rollback(); } catch { }
                        RunClientScriptSafe(thongbao_class.metro_dialog("Thông báo", "Tài khoản không tồn tại.", "false", "false", "OK", "alert", ""));
                        return;
                    }

                    string scope = ResolveAccountScope(acc);
                    if (!IsCardTypeAllowedForScope(scope, loai))
                    {
                        try { tran.Rollback(); } catch { }
                        RunClientScriptSafe(thongbao_class.metro_dialog("Thông báo", BuildInvalidCardTypeMessage(scope), "false", "false", "OK", "alert", ""));
                        return;
                    }

                    // 1) khóa thẻ cũ cùng (tk + loai) đang bật
                    var olds = db.The_PhatHanh_tbs
                        .Where(p => p.taikhoan == tk && p.LoaiThe == loai && p.TrangThai == true)
                        .ToList();

                    foreach (var x in olds)
                    {
                        x.TrangThai = false;
                        x.NgayCapNhatTrangThai = now;
                        x.NguoiCapNhatTrangThai = actor;
                    }

                    // 2) thêm thẻ mới (mặc định bật)
                    The_PhatHanh_tb ob = new The_PhatHanh_tb();
                    ob.idGuide = Guid.NewGuid();
                    ob.taikhoan = tk;
                    ob.LoaiThe = loai;
                    ob.TenThe = tenThe;

                    ob.NgayPhatHanh = now;
                    ob.TrangThai = true;

                    ob.NgayTao = now;
                    ob.NguoiTao = actor;

                    ob.NgayCapNhatTrangThai = now;
                    ob.NguoiCapNhatTrangThai = actor;

                    // PIN mặc định khi phát hành thẻ: 6868 (4 chữ số).
                    acc.mapin_thanhtoan = PinSecurity_cl.HashPin("6868");

                    db.The_PhatHanh_tbs.InsertOnSubmit(ob);
                    db.SubmitChanges();

                    tran.Commit();

                    Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Phát hành thẻ thành công.", "1000", "warning");
                    Response.Redirect(BuildListUrl(), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                catch (Exception ex2)
                {
                    try { tran.Rollback(); } catch { }
                    Log_cl.Add_Log(ex2.Message, actor, ex2.StackTrace);
                    RunClientScriptSafe(thongbao_class.metro_dialog("Thông báo", "Có lỗi: " + ex2.Message, "false", "false", "OK", "alert", ""));
                }
                finally
                {
                    db.Transaction = null;
                    db.Connection.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "admin", ex.StackTrace);
            RunClientScriptSafe(thongbao_class.metro_dialog("Lỗi", ex.Message, "false", "false", "OK", "alert", ""));
        }
    }

    // ================== TOGGLE ==================
    protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "toggle")
        {
            Guid id;
            if (Guid.TryParse((e.CommandArgument ?? "").ToString(), out id))
                toggle_the(id);
        }
    }

    void toggle_the(Guid idGuide)
    {
        string actor = GetActor();
        DateTime now = AhaTime_cl.Now;

        using (dbDataContext db = new dbDataContext())
        {
            db.Connection.Open();
            var tran = db.Connection.BeginTransaction(System.Data.IsolationLevel.Serializable);
            db.Transaction = tran;

            try
            {
                var cur = db.The_PhatHanh_tbs.FirstOrDefault(p => p.idGuide == idGuide);
                if (cur == null) throw new Exception("Không tìm thấy thẻ.");

                string tk = cur.taikhoan;
                int loai = cur.LoaiThe;

                if (cur.TrangThai == true)
                {
                    // bật -> tắt
                    cur.TrangThai = false;
                    cur.NgayCapNhatTrangThai = now;
                    cur.NguoiCapNhatTrangThai = actor;
                }
                else
                {
                    // tắt -> bật: khóa các thẻ khác cùng (tk+loai) rồi bật cái này
                    var others = db.The_PhatHanh_tbs
                        .Where(p => p.taikhoan == tk && p.LoaiThe == loai && p.idGuide != idGuide && p.TrangThai == true)
                        .ToList();

                    foreach (var x in others)
                    {
                        x.TrangThai = false;
                        x.NgayCapNhatTrangThai = now;
                        x.NguoiCapNhatTrangThai = actor;
                    }

                    cur.TrangThai = true;
                    cur.NgayCapNhatTrangThai = now;
                    cur.NguoiCapNhatTrangThai = actor;
                }

                db.SubmitChanges();
                tran.Commit();

                show_main();
            }
            catch (Exception ex)
            {
                try { tran.Rollback(); } catch { }
                Log_cl.Add_Log(ex.Message, actor, ex.StackTrace);
                RunClientScriptSafe(thongbao_class.metro_dialog("Thông báo", "Có lỗi: " + ex.Message, "false", "false", "OK", "alert", ""));
            }
            finally
            {
                db.Transaction = null;
                db.Connection.Close();
            }
        }
    }
}
