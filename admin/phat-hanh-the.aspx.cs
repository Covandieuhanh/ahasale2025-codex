using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_phat_hanh_the : System.Web.UI.Page
{
    private const string ViewAdd = "add";
    private const int CardTypeUuDai = CardIssuance_cl.CardTypeUuDai;
    private const int CardTypeTieuDung = CardIssuance_cl.CardTypeTieuDung;
    private const int CardTypeShopPartner = CardIssuance_cl.CardTypeShopPartner;
    private const int CardTypeDongHanhHeSinhThai = CardIssuance_cl.CardTypeDongHanhHeSinhThai;
    private const int CardTypeLaoDong = CardIssuance_cl.CardTypeLaoDong;

    private string BuildListUrl()
    {
        return ResolveUrl("~/admin/phat-hanh-the.aspx");
    }

    private string BuildAddUrl()
    {
        return ResolveUrl("~/admin/phat-hanh-the/them-moi.aspx");
    }

    private bool RedirectWhenLegacyCardFlowDisabled()
    {
        if (!CompanyShop_cl.HideLegacyAdminSystemProduct())
            return false;

        Session["thongbao"] = thongbao_class.metro_notifi_onload(
            "Thông báo",
            "Luồng phát hành/bán thẻ đã chuyển sang không gian /shop của tài khoản shop công ty.",
            "2200",
            "warning");
        Response.Redirect("/admin/default.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
        return true;
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
        AdminRolePolicy_cl.RequireSuperAdmin();
        if (RedirectWhenLegacyCardFlowDisabled())
            return;

        try
        {
            but_show_add.NavigateUrl = BuildAddUrl();
            lnk_back_list.NavigateUrl = BuildListUrl();
            string view = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
            bool isAddView = view == ViewAdd;

            if (isAddView && !AdminFullPageRoute_cl.IsTransferredRequest(Context))
            {
                Response.Redirect(BuildAddUrl(), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (isAddView)
            {
                ShowAddPage(!IsPostBack);
            }

            if (!IsPostBack)
            {
                AdminRolePolicy_cl.RequireSuperAdmin();
                ViewState["title"] = "Phát hành thẻ";
                if (!isAddView)
                    show_main();
            }
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "admin", ex.StackTrace);
            Session["thongbao"] = thongbao_class.metro_notifi_onload("Thông báo", "Trang phát hành thẻ đang gặp lỗi dữ liệu. Vui lòng đăng nhập lại hoặc thử lại sau.", "2600", "warning");
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
        return CardIssuance_cl.GetCardName(loai);
    }

    private string ResolveAccountScope(taikhoan_tb account)
    {
        return CardIssuance_cl.ResolveAccountScope(account);
    }

    private bool IsCardTypeAllowedForScope(string scope, int loaiThe)
    {
        return CardIssuance_cl.IsCardTypeAllowedForScope(scope, loaiThe);
    }

    private string BuildInvalidCardTypeMessage(string scope)
    {
        return CardIssuance_cl.BuildInvalidCardTypeMessage(scope);
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

    // ================== UI: OPEN/CLOSE FULL-PAGE ==================
    protected void but_show_add_Click(object sender, EventArgs e)
    {
        try
        {
            AdminRolePolicy_cl.RequireSuperAdmin();
            Response.Redirect(BuildAddUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "admin", ex.StackTrace);
            RunClientScriptSafe(thongbao_class.metro_notifi("Lỗi", ex.Message, "2600", "warning"));
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
            AdminRolePolicy_cl.RequireSuperAdmin();

            string tk = (ddl_taikhoan.SelectedValue ?? "").Trim();
            int loai;
            int.TryParse(ddl_loaithe.SelectedValue, out loai);

            if (string.IsNullOrEmpty(tk) || loai <= 0)
            {
                RunClientScriptSafe(thongbao_class.metro_notifi("Thông báo", "Vui lòng chọn tài khoản và loại thẻ.", "2600", "warning"));
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
                        RunClientScriptSafe(thongbao_class.metro_notifi("Thông báo", "Tài khoản không tồn tại.", "2600", "warning"));
                        return;
                    }

                    string scope = ResolveAccountScope(acc);
                    if (!IsCardTypeAllowedForScope(scope, loai))
                    {
                        try { tran.Rollback(); } catch { }
                        RunClientScriptSafe(thongbao_class.metro_notifi("Thông báo", BuildInvalidCardTypeMessage(scope), "2600", "warning"));
                        return;
                    }

                    CardIssuance_cl.IssueCard(db, acc, loai, actor, now);
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
                    RunClientScriptSafe(thongbao_class.metro_notifi("Thông báo", "Có lỗi: " + ex2.Message, "2600", "warning"));
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
            RunClientScriptSafe(thongbao_class.metro_notifi("Lỗi", ex.Message, "2600", "warning"));
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
                RunClientScriptSafe(thongbao_class.metro_notifi("Thông báo", "Có lỗi: " + ex.Message, "2600", "warning"));
            }
            finally
            {
                db.Transaction = null;
                db.Connection.Close();
            }
        }
    }
}
