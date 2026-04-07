using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_quan_ly_menu_Default : System.Web.UI.Page
{
    dbDataContext db = new dbDataContext();
    giangvien_class gv_cl = new giangvien_class();
    datetime_class dt_cl = new datetime_class(); nganh_class nganh_cl = new nganh_class();
    nganh_class ng_cl = new nganh_class();
    public string user, user_parent;
    private bool HasAnyPermission(params string[] permissionKeys)
    {
        if (string.IsNullOrWhiteSpace(user) || permissionKeys == null)
            return false;

        for (int i = 0; i < permissionKeys.Length; i++)
        {
            string permissionKey = (permissionKeys[i] ?? "").Trim();
            if (permissionKey != "" && bcorn_class.check_quyen(user, permissionKey) == "")
                return true;
        }

        return false;
    }
    #region phân trang
    public int stt = 1, current_page = 1, total_page = 1, show = 30;
    List<string> list_id_split;
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        #region Check quyen theo nganh
        user = (access.User ?? "").Trim();
        user_parent = access.OwnerAccountKey;
        if (HasAnyPermission("q15_1", "n15_1"))
        {
            if (!IsPostBack)
            {
                var list_nganh = (from ob1 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                  select new { id = ob1.id, ten = ob1.ten, }
                                   );
                DropDownList5.DataSource = list_nganh;
                DropDownList5.DataTextField = "ten";
                DropDownList5.DataValueField = "id";
                DropDownList5.DataBind();
                DropDownList5.Items.Insert(0, new ListItem("Tất cả", ""));
                if (HasAnyPermission("q15_1"))
                {
                }
                else
                {
                    DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList5.Enabled = false;
                }


                var q = db.giangvien_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString());
                if (Session["current_page_giangvien"] == null)//lưu giữ trang hiện tại
                    Session["current_page_giangvien"] = "1";

                if (Session["index_sapxep_giangvien"] == null)////lưu sắp xếp
                {
                    DropDownList2.SelectedIndex = 0;
                    Session["index_sapxep_giangvien"] = DropDownList2.SelectedIndex.ToString();
                }
                else
                    DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_giangvien"].ToString());

                if (Session["search_giangvien"] != null)//lưu tìm kiếm
                    txt_search.Text = Session["search_giangvien"].ToString();
                else
                    Session["search_giangvien"] = txt_search.Text;

                if (Session["index_trangthai_giangvien"] != null)//lưu lọc trạng thái
                    DropDownList1.SelectedIndex = int.Parse(Session["index_trangthai_giangvien"].ToString());
                else
                    Session["index_trangthai_giangvien"] = DropDownList1.SelectedValue.ToString();

                if (Session["show_giangvien"] == null)//lưu số dòng mặc định
                {
                    txt_show.Text = "30";
                    Session["show_giangvien"] = txt_show.Text;
                }
                else
                    txt_show.Text = Session["show_giangvien"].ToString();

                if (Session["tungay_giangvien"] == null)
                {
                    txt_tungay.Text = q.Count() != 0 ? q.Min(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                    Session["tungay_giangvien"] = txt_tungay.Text;
                }
                else
                    txt_tungay.Text = Session["tungay_giangvien"].ToString();

                if (Session["denngay_giangvien"] == null)
                {
                    txt_denngay.Text = q.Count() != 0 ? q.Max(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                    Session["denngay_giangvien"] = txt_denngay.Text;
                }
                else
                    txt_denngay.Text = Session["denngay_giangvien"].ToString();

                if (Session["index_loc_nganh_giangvien"] != null)//lưu lọc theo theo toán
                    DropDownList5.SelectedIndex = int.Parse(Session["index_loc_nganh_giangvien"].ToString());
                else
                    Session["index_loc_nganh_giangvien"] = DropDownList5.SelectedIndex.ToString();
            }
            main();
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        #endregion



    }

    public void main()
    {
        //Intersect: lấy ra các phần tử mà cả 2 bên đều có (phần chung)
        #region lấy dữ liệu
        var list_all = (from ob1 in db.giangvien_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                        join ob2 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on ob1.chuyenmon.ToString() equals ob2.id.ToString()
                        select new
                        {
                            id = ob1.id,
                            hoten = ob1.hoten,
                            hoten_khongdau = ob1.hoten_khongdau,
                            avt = ob1.anhdaidien,
                            trangthai = ob1.trangthai,
                            ngaytao = ob1.ngaytao,
                            sdt = ob1.dienthoai,
                            email = ob1.email,
                            nguoitao = ob1.nguoitao,
                            ngaysinh = ob1.ngaysinh,
                            sobuoi_lt = ob1.sobuoi_lythuyet,
                            sobuoi_th = ob1.sobuoi_thuchanh,
                            sobuoi_tg = ob1.sobuoi_trogiang,
                            chuyenmon = ob2.ten,
                            goidaotao = ob1.goidaotao,
                            danhgia = ob1.danhgiagiangvien,
                            id_nganh = ob1.chuyenmon,
                        });

        //xử lý theo thời gian
        var list_time = list_all.Where(p => p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_giangvien"].ToString()).Date && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_giangvien"].ToString()).Date);
        list_all = list_all.Intersect(list_time).ToList();

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.hoten.ToLower().Contains(_key) || p.hoten_khongdau.ToLower().Contains(_key) || p.sdt.ToString() == _key || p.email.ToString() == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }
        //xử lý trạng thái

        //lọc dữ liệu
        //if (DropDownList1.SelectedValue.ToString() != "0")
        //{
        switch (DropDownList1.SelectedValue.ToString())
        {
            case ("0"): var list_0 = list_all.Where(p => p.trangthai == "Đang giảng dạy").ToList(); list_all = list_all.Intersect(list_0).ToList(); break;
            case ("1"): var list_1 = list_all.Where(p => p.trangthai == "Ngưng giảng dạy").ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
            default: var list_2 = list_all.ToList(); list_all = list_all.Intersect(list_2).ToList(); break;//tất cả
        }
        //}

        if (DropDownList5.SelectedValue.ToString() != "")//ngành
        {
            var list_1 = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList(); list_all = list_all.Intersect(list_1).ToList();
        }

        //sắp xếp
        switch (Session["index_sapxep_giangvien"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.ngaytao).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
            default: break;
        }
        #endregion

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 30;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_giangvien"].ToString());
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

    protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            return;

        Literal litPersonHub = e.Item.FindControl("litPersonHub") as Literal;
        if (litPersonHub == null)
            return;

        string displayName = (DataBinder.Eval(e.Item.DataItem, "hoten") + "").Trim();
        string phone = (DataBinder.Eval(e.Item.DataItem, "sdt") + "").Trim();
        string sourceKey = (DataBinder.Eval(e.Item.DataItem, "id") + "").Trim();
        litPersonHub.Text = BuildPersonHubListHtml(displayName, phone, "chuyên gia", "lecturer", sourceKey);
    }

    private string BuildPersonHubListHtml(string displayName, string phone, string roleLabel, string sourceType, string sourceKey)
    {
        string normalizedPhone = AccountAuth_cl.NormalizePhone(phone);
        if (normalizedPhone == "")
        {
            return "<small class='fg-gray'>Thiếu số điện thoại để gắn Home. Cập nhật hồ sơ " + HttpUtility.HtmlEncode(roleLabel) + " này trước.</small>";
        }

        GianHangAdminPersonHub_cl.PersonLinkInfo linkInfo = GianHangAdminPersonHub_cl.GetLinkInfo(db, user_parent, normalizedPhone, displayName);
        string css = linkInfo == null || string.IsNullOrWhiteSpace(linkInfo.LinkCss) ? "bg-gray fg-white" : linkInfo.LinkCss;
        string label = linkInfo == null || string.IsNullOrWhiteSpace(linkInfo.StatusLabel) ? "Chưa liên kết" : linkInfo.StatusLabel;
        string url = GianHangAdminPersonHub_cl.BuildDetailUrl(normalizedPhone);
        string note = "Mở hồ sơ người để liên kết Home một lần cho toàn bộ vai trò cùng số điện thoại.";
        GianHangAdminPersonHub_cl.PersonSourceRef sourceInfo = GianHangAdminPersonHub_cl.GetSourceInfo(db, user_parent, normalizedPhone, sourceType, sourceKey);
        string accessCss = sourceInfo == null || string.IsNullOrWhiteSpace(sourceInfo.AdminAccessCss) ? "bg-gray fg-white" : sourceInfo.AdminAccessCss;
        string accessLabel = sourceInfo == null || string.IsNullOrWhiteSpace(sourceInfo.AdminAccessLabel) ? "Không mở quyền /gianhang/admin ở nguồn này" : sourceInfo.AdminAccessLabel;

        if (linkInfo != null && linkInfo.LinkedHomeAccount != null)
        {
            string linkedName = string.IsNullOrWhiteSpace(linkInfo.LinkedHomeAccount.hoten)
                ? (linkInfo.LinkedHomeAccount.taikhoan ?? "")
                : linkInfo.LinkedHomeAccount.hoten;
            note = "Đã liên kết với Home " + linkedName + " • " + (linkInfo.LinkedHomeAccount.taikhoan ?? "");
        }
        else if (linkInfo != null && !string.IsNullOrWhiteSpace(linkInfo.PendingPhone))
        {
            note = "Đang chờ số " + linkInfo.PendingPhone + " đăng ký hoặc đăng nhập AhaSale.";
        }

        return "<span class='data-wrapper'><code class='" + HttpUtility.HtmlAttributeEncode(css) + "'>" + HttpUtility.HtmlEncode(label) + "</code></span>"
            + "<div class='mt-1'><span class='data-wrapper'><code class='" + HttpUtility.HtmlAttributeEncode(accessCss) + "'>" + HttpUtility.HtmlEncode(accessLabel) + "</code></span></div>"
            + "<div class='mt-1'><small class='fg-gray'>" + HttpUtility.HtmlEncode(note) + "</small></div>"
            + "<div class='mt-1'><a class='fg-blue fg-darkBlue-hover' href='" + HttpUtility.HtmlAttributeEncode(url) + "'>Mở hồ sơ người</a></div>";
    }


    #region autopostback
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
        Session["current_page_giangvien"] = "1";

        main();
    }
    //protected void txt_show_TextChanged(object sender, EventArgs e)
    //{
    //    Session["current_page_giangvien"] = "1";
    //    main();
    //}
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_giangvien"] = int.Parse(Session["current_page_giangvien"].ToString()) - 1;
        if (int.Parse(Session["current_page_giangvien"].ToString()) < 1)
            Session["current_page_giangvien"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_giangvien"] = int.Parse(Session["current_page_giangvien"].ToString()) + 1;
        if (int.Parse(Session["current_page_giangvien"].ToString()) > total_page)
            Session["current_page_giangvien"] = total_page;
        main();
    }
    #endregion


    protected void Button1_Click(object sender, EventArgs e)
    {
        Session["index_trangthai_giangvien"] = DropDownList1.SelectedIndex;
        Session["index_sapxep_giangvien"] = DropDownList2.SelectedIndex;
        Session["current_page_giangvien"] = "1";
        Session["search_giangvien"] = txt_search.Text.Trim();
        Session["show_giangvien"] = txt_show.Text.Trim();
        Session["tungay_giangvien"] = txt_tungay.Text;
        Session["denngay_giangvien"] = txt_denngay.Text;

        Session["index_loc_nganh_giangvien"] = DropDownList5.SelectedIndex;
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        Session["index_trangthai_giangvien"] = null;
        Session["index_sapxep_giangvien"] = null;
        Session["current_page_giangvien"] = null;
        Session["search_giangvien"] = null;
        Session["show_giangvien"] = null;
        Session["tungay_giangvien"] = null;
        Session["denngay_giangvien"] = null;

        Session["index_loc_nganh_giangvien"] = null;
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-giang-vien/Default.aspx");
    }

    #region chọn ngày nhanh
    protected void but_homqua_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = DateTime.Now.Date.AddDays(-1).ToString();
        txt_denngay.Text = DateTime.Now.Date.AddDays(-1).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_homnay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = DateTime.Now.Date.ToString();
        txt_denngay.Text = DateTime.Now.Date.ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_tuantruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydautuan().AddDays(-7).ToString();//lấy ngày đầu tuần
        txt_denngay.Text = dt_cl.return_ngaydautuan().AddDays(-1).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_tuannay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydautuan().ToString();//lấy ngày đầu tuần
        txt_denngay.Text = dt_cl.return_ngaycuoituan().ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_thangtruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauthangtruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoithangtruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_thangnay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauthang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoithang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_namtruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydaunamtruoc(DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoinamtruoc(DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_namnay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydaunam(DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoinam(DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_quytruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauquytruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoiquytruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_quynay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauquynay(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoiquynay(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }


    #endregion



    protected void but_xoa_Click(object sender, ImageClickEventArgs e)
    {
        if (HasAnyPermission("q15_4", "n15_4"))
        {
            int _count = 0;
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    var q = db.giangvien_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (q.Count() != 0)
                    {
                        giangvien_table _ob = q.First();
                        string _oldPhone = _ob == null ? "" : (_ob.dienthoai ?? "");
                        string _oldName = _ob == null ? "" : (_ob.hoten ?? "");
                        db.giangvien_tables.DeleteOnSubmit(_ob);
                        db.SubmitChanges();
                        GianHangAdminPersonHub_cl.PreserveLinkAfterSourceRemoval(
                            db,
                            user_parent,
                            _oldPhone,
                            _oldName,
                            user,
                            "lecturer",
                            "Chuyên gia đào tạo",
                            _id,
                            "Chuyên gia đào tạo",
                            "Vai trò chuyên gia đào tạo đã bị xóa khỏi module nguồn.");
                        _count = _count + 1;
                    }
                }
            }
            if (_count > 0)
            {
                main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công.", "4000", "warning"), true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }

    protected void but_ngung_Click(object sender, EventArgs e)
    {
        UpdateSelectedLecturerStatus("Ngưng giảng dạy", "Đã chuyển các chuyên gia được chọn sang trạng thái ngừng dùng an toàn.");
    }

    protected void but_molai_Click(object sender, EventArgs e)
    {
        UpdateSelectedLecturerStatus("Đang giảng dạy", "Đã mở lại các chuyên gia được chọn.");
    }

    private void UpdateSelectedLecturerStatus(string targetStatus, string successMessage)
    {
        if (!HasAnyPermission("q15_3", "n15_3"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Bạn không đủ quyền để đổi trạng thái chuyên gia.", "4000", "warning"), true);
            return;
        }

        int affected = 0;
        for (int i = 0; i < list_id_split.Count; i++)
        {
            if (Request.Form[list_id_split[i]] != "on")
                continue;

            string selectedId = list_id_split[i].Replace("check_", "");
            giangvien_table lecturer = db.giangvien_tables.FirstOrDefault(p => p.id.ToString() == selectedId && p.id_chinhanh == Session["chinhanh"].ToString());
            if (lecturer == null)
                continue;

            if (string.Equals((lecturer.trangthai ?? "").Trim(), targetStatus, StringComparison.OrdinalIgnoreCase))
                continue;

            lecturer.trangthai = targetStatus;
            db.SubmitChanges();
            affected++;
        }

        if (affected > 0)
        {
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", successMessage, "4000", targetStatus == "Đang giảng dạy" ? "success" : "warning"), true);
            return;
        }

        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có chuyên gia nào phù hợp để đổi trạng thái.", "4000", "warning"), true);
    }
}
