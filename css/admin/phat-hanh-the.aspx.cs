using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_phat_hanh_the : System.Web.UI.Page
{
    private const string ViewAdd = "add";

    private string BuildListUrl()
    {
        return ResolveUrl("~/admin/phat-hanh-the.aspx");
    }

    private string BuildAddUrl()
    {
        return ResolveUrl("~/admin/phat-hanh-the.aspx?view=" + ViewAdd);
    }

    private void ShowAddPage()
    {
        load_dropdown_taikhoan();
        pn_add.Visible = true;
        up_add.Update();
        up_main.Visible = false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        but_show_add.NavigateUrl = BuildAddUrl();

        if (!IsPostBack)
        {
            check_login_cl.check_login_admin(PermissionProfile_cl.HoSoTieuDung, PermissionProfile_cl.HoSoTieuDung);
            ViewState["title"] = "Phát hành thẻ";
            show_main();

            string view = (Request.QueryString["view"] ?? "").Trim().ToLowerInvariant();
            if (view == ViewAdd)
                ShowAddPage();
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
            case 1: return "Thẻ ưu đãi";
            case 2: return "Thẻ tiêu dùng";
            case 3: return "Thẻ gian hàng đối tác";
            case 4: return "Thẻ đồng hành hệ sinh thái";
            default: return "Thẻ #" + loai;
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
            var list = db.taikhoan_tbs
                .Where(p => p.taikhoan != "admin"
                    && (p.phanloai == null
                        || (!p.phanloai.StartsWith(AccountType_cl.Treasury)
                            && !p.phanloai.StartsWith(AccountType_cl.LegacyTreasury))))
                .OrderBy(p => p.taikhoan)
                .Select(p => new
                {
                    p.taikhoan,
                    Text = p.taikhoan + " - " + (p.hoten ?? "")
                })
                .ToList();

            ddl_taikhoan.DataSource = list;
            ddl_taikhoan.DataTextField = "Text";
            ddl_taikhoan.DataValueField = "taikhoan";
            ddl_taikhoan.DataBind();

            ddl_taikhoan.Items.Insert(0, new ListItem("-- Chọn tài khoản --", ""));
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
                        RunClientScriptSafe(thongbao_class.metro_dialog("Thông báo", "Tài khoản không tồn tại.", "false", "false", "OK", "alert", ""));
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
