using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_Default : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();

    public string user, user_parent;
    #region phân trang
    public int stt = 1, current_page = 1, show = 50, total_page = 1;
    List<string> list_id_split;
    #endregion

    private bool HasPermission(string permissionKey)
    {
        string currentUser = (user ?? "").Trim();
        return !string.IsNullOrWhiteSpace(currentUser) && bcorn_class.check_quyen(currentUser, permissionKey) != "2";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q8_5");
        if (access == null)
            return;

        user = (access.User ?? "").Trim();
        user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();
        if (!IsPostBack)
        {
            Session["index_sapxep_nhomkhachhang"] = "0";
            Session["current_page_nhomkhachhang"] = "1";
        }
        main();
    }
    public void main()
    {
        //lấy dữ liệu
        var list_all = (from ob1 in db.nhomkhachhang_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                        select new
                        {
                            id = ob1.id,
                            tennhom = ob1.tennhom,
                        }).ToList();

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.tennhom.ToLower().Contains(_key)).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        //sắp xếp
        switch (Session["index_sapxep_nhomkhachhang"].ToString())
        {
            //case ("1"): list_all = list_all.OrderBy(p => p.ngaytao_tk).ToList(); break;
            default: list_all = list_all.OrderBy(p => p.tennhom).ToList(); break;
        }

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 50;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_nhomkhachhang"].ToString());
        if (current_page > total_page)
            current_page = total_page;
        if (current_page >= total_page)
            but_xemtiep.Enabled = false;
        else
            but_xemtiep.Enabled = true;
        if (current_page == 1)
            but_quaylai.Enabled = false;
        else
            but_quaylai.Enabled = true;

        //main
        stt = (show * current_page) - show + 1;
        var list_split = list_all.Skip(current_page * show - show).Take(show).ToList();
        list_id_split = new List<string>();
        foreach (var t in list_split)
        {
            list_id_split.Add("check_" + t.id);
        }
        int _s1 = stt + list_split.Count - 1;
        if (list_all.Count() != 0)
            lb_show.Text = "Hiển thị " + stt + "-" + _s1 + " trong số " + list_all.Count().ToString("#,##0") + " mục";
        else
            lb_show.Text = "Hiển thị 0-0 trong số 0";
        Repeater1.DataSource = list_split;
        Repeater1.DataBind();
    }
    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        ApplySearchState();
    }
    protected void but_search_Click(object sender, EventArgs e)
    {
        ApplySearchState();
    }
    private void ApplySearchState()
    {
        Session["search_nhomkhachhang"] = txt_search.Text.Trim();
        Session["current_page_nhomkhachhang"] = "1";
        main();
    }

    protected void txt_show_TextChanged(object sender, EventArgs e)
    {
        Session["current_page_nhomkhachhang"] = "1";

        main();
    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_nhomkhachhang"] = int.Parse(Session["current_page_nhomkhachhang"].ToString()) - 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_nhomkhachhang"] = int.Parse(Session["current_page_nhomkhachhang"].ToString()) + 1;

        main();
    }

    protected void but_form_themnhomthuchi_Click(object sender, EventArgs e)
    {
        if (!HasPermission("q8_5"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        string _tennhom = txt_tennhom.Text.Trim();

        if (_tennhom == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập tên nhóm khách hàng.", "4000", "warning"), true);
        else
        {

            nhomkhachhang_table _ob = new nhomkhachhang_table();
            _ob.tennhom = _tennhom;
            _ob.id_chinhanh = Session["chinhanh"].ToString();
            db.nhomkhachhang_tables.InsertOnSubmit(_ob);
            db.SubmitChanges();
            txt_tennhom.Text = "";
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm nhóm khách hàng thành công.", "4000", "warning"), true);
            //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thêm nhóm khách hàng thành công.", "2000", "warning");
            //Response.Redirect("/gianhang/admin/quan-ly-thu-chi/nhom-thu-chi.aspx");
        }
    }

    protected void but_lammoi_Click(object sender, EventArgs e)
    {
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Làm mới thành công.", "2000", "warning"), true);
    }

    protected void but_save_Click(object sender, ImageClickEventArgs e)
    {
        if (!HasPermission("q8_5"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        for (int i = 0; i < list_id_split.Count; i++)
        {
            string _id = list_id_split[i].Replace("check_", "");
            string _tennhom = Request.Form["name_" + _id].Trim();

            var q = db.nhomkhachhang_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
            if (q.Count() != 0)
            {
                nhomkhachhang_table _ob = q.First();
                _ob.tennhom = _tennhom;
                db.SubmitChanges();
            }
        }
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lưu thành công.", "4000", "warning"), true);
        //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Lưu thành công.", "2000", "warning");
        //Response.Redirect("/gianhang/admin/quan-ly-thu-chi/nhom-thu-chi.aspx");
    }

    protected void but_xoa_Click(object sender, ImageClickEventArgs e)
    {
        if (!HasPermission("q8_5"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        for (int i = 0; i < list_id_split.Count; i++)
        {
            if (Request.Form[list_id_split[i]] == "on")
            {
                string _id = list_id_split[i].Replace("check_", "");
                var q = db.nhomkhachhang_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                if (q.Count() != 0)
                {
                    nhomkhachhang_table _ob = q.First();
                    db.nhomkhachhang_tables.DeleteOnSubmit(_ob);
                    db.SubmitChanges();
                }
            }
        }
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "warning"), true);
    }

}
