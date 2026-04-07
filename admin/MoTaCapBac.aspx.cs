using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_MoTaCapBac : System.Web.UI.Page
{
    String_cl str_cl = new String_cl();

    // 0 = đang sửa Mô tả
    // 1 = đang sửa Trách nhiệm
    private const int TAB_MOTA = 0;
    private const int TAB_TRACHNHIEM = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        AdminAccessGuard_cl.RequireFeatureAccess("tier_reference", "/admin/default.aspx?mspace=home");

        if (!IsPostBack)
        {
            Session["url_back"] = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;

            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk)) _tk = mahoa_cl.giaima_Bcorn(_tk);
            else _tk = "";
            ViewState["taikhoan"] = _tk;

            ViewState["selected_id"] = null;
            ViewState["tab"] = TAB_MOTA;

            BindList();
            SetEditorState(false);
        }
    }

    private void BindList()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                // === BẮT BUỘC DB phải có cột TrachNhiem (string) trong VanBanMoTa_9Level_tbs ===
                var data = db.VanBanMoTa_9Level_tbs
                    .OrderBy(x => x.id)
                    .Select(x => new
                    {
                        x.id,
                        x.Capbac,
                        x.MoTa,
                        x.TrachNhiem,
                        MoTaPreview = MakePreview(x.MoTa, 80)
                    })
                    .ToList();

                lb_empty.Visible = (data.Count == 0);
                rp_list.DataSource = data;
                rp_list.DataBind();
            }
        }
        catch (Exception ex)
        {
            string _tk = (ViewState["taikhoan"] ?? "").ToString();
            Log_cl.Add_Log(ex.Message, _tk, ex.StackTrace);
        }
    }

    private string MakePreview(string html, int maxLen)
    {
        if (string.IsNullOrEmpty(html)) return "(trống)";

        string s = html;
        s = System.Text.RegularExpressions.Regex.Replace(s, "<.*?>", " ");
        s = str_cl.Remove_Blank(s).Trim();

        if (s.Length <= maxLen) return s;
        return s.Substring(0, maxLen).Trim() + "...";
    }

    private void SetEditorState(bool enable)
    {
        pn_editor.Visible = enable;
        pn_hint.Visible = !enable;
        but_save.Enabled = enable;
        lb_msg.Text = "";
        ApplyTabUI();
    }

    private void ApplyTabUI()
    {
        int tab = TAB_MOTA;
        if (ViewState["tab"] != null) int.TryParse(ViewState["tab"].ToString(), out tab);

        // panel ckeditor
        pn_ck_mota.Visible = (tab == TAB_MOTA);
        pn_ck_trachnhiem.Visible = (tab == TAB_TRACHNHIEM);

        // highlight tab button (dùng class "active" nếu css của bạn có; không thì vẫn ok)
        tab_mota.CssClass = "button mini " + (tab == TAB_MOTA ? "warning" : "light");
        tab_trachnhiem.CssClass = "button mini " + (tab == TAB_TRACHNHIEM ? "warning" : "light");
    }

    protected void tab_mota_Click(object sender, EventArgs e)
    {
        ViewState["tab"] = TAB_MOTA;
        ApplyTabUI();
        up_main.Update();
    }

    protected void tab_trachnhiem_Click(object sender, EventArgs e)
    {
        ViewState["tab"] = TAB_TRACHNHIEM;
        ApplyTabUI();
        up_main.Update();
    }

    protected void lk_select_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("tier_reference", "/admin/default.aspx?mspace=home");

            LinkButton lk = (LinkButton)sender;
            int id = int.Parse(lk.CommandArgument);

            LoadItem(id);
            up_main.Update();
        }
        catch (Exception ex)
        {
            string _tk = (ViewState["taikhoan"] ?? "").ToString();
            Log_cl.Add_Log(ex.Message, _tk, ex.StackTrace);
        }
    }

    private void LoadItem(int id)
    {
        using (dbDataContext db = new dbDataContext())
        {
            var item = db.VanBanMoTa_9Level_tbs.FirstOrDefault(x => x.id == id);
            if (item == null)
            {
                SetEditorState(false);
                lb_msg.Text = thongbao_class.metro_notifi("Thông báo", "Không tìm thấy dữ liệu.", "1500", "warning");
                return;
            }

            ViewState["selected_id"] = id;
            lb_capbac.Text = item.Capbac ?? "";

            // CKEditor content
            ck_mota.Text = item.MoTa ?? "";

            // CKEditor content: Trách nhiệm (field name TrachNhiem)
            TrachNhiem.Text = item.TrachNhiem ?? "";

            SetEditorState(true);
        }
    }

    protected void but_save_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("tier_reference", "/admin/default.aspx?mspace=home");

            if (ViewState["selected_id"] == null)
            {
                lb_msg.Text = thongbao_class.metro_notifi("Thông báo", "Bạn chưa chọn cấp bậc để sửa.", "1500", "warning");
                return;
            }

            int id = int.Parse(ViewState["selected_id"].ToString());

            int tab = TAB_MOTA;
            if (ViewState["tab"] != null) int.TryParse(ViewState["tab"].ToString(), out tab);

            using (dbDataContext db = new dbDataContext())
            {
                var item = db.VanBanMoTa_9Level_tbs.FirstOrDefault(x => x.id == id);
                if (item == null)
                {
                    lb_msg.Text = thongbao_class.metro_notifi("Thông báo", "Không tìm thấy dữ liệu.", "1500", "warning");
                    return;
                }

                if (tab == TAB_MOTA)
                {
                    string mota = ck_mota.Text ?? "";
                    item.MoTa = mota;
                }
                else
                {
                    string trachnhiem = TrachNhiem.Text ?? "";
                    item.TrachNhiem = trachnhiem;
                }

                db.SubmitChanges();
            }

            BindList();

            lb_msg.Text = thongbao_class.metro_notifi("Thông báo", "Đã lưu thành công.", "1200", "warning");
            up_main.Update();
        }
        catch (Exception ex)
        {
            string _tk = (ViewState["taikhoan"] ?? "").ToString();
            Log_cl.Add_Log(ex.Message, _tk, ex.StackTrace);

            lb_msg.Text = thongbao_class.metro_notifi("Thông báo", "Có lỗi khi lưu.", "1500", "warning");
            up_main.Update();
        }
    }

    protected void but_reload_Click(object sender, EventArgs e)
    {
        try
        {
            AdminAccessGuard_cl.RequireFeatureAccess("tier_reference", "/admin/default.aspx?mspace=home");

            BindList();

            if (ViewState["selected_id"] != null)
            {
                int id = int.Parse(ViewState["selected_id"].ToString());
                LoadItem(id);
            }
            else
            {
                SetEditorState(false);
            }

            up_main.Update();
        }
        catch (Exception ex)
        {
            string _tk = (ViewState["taikhoan"] ?? "").ToString();
            Log_cl.Add_Log(ex.Message, _tk, ex.StackTrace);
        }
    }

    protected void rp_list_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

        var lk = (LinkButton)e.Item.FindControl("lk_select");
        if (lk == null) return;

        if (ViewState["selected_id"] != null)
        {
            int selectedId;
            if (int.TryParse(ViewState["selected_id"].ToString(), out selectedId))
            {
                int rowId;
                if (int.TryParse(lk.CommandArgument, out rowId) && rowId == selectedId)
                {
                    lk.CssClass = lk.CssClass + " active";
                }
            }
        }
    }
}
