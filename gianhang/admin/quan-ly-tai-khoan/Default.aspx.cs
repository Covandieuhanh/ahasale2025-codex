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
    private const string SessionHomeLinkFilterKey = "home_link_filter_taikhoan";

    private sealed class AccountListItem
    {
        public string taikhoan { get; set; }
        public string hoten { get; set; }
        public string hoten_khongdau { get; set; }
        public string avt { get; set; }
        public string trangthai { get; set; }
        public DateTime? ngaytao { get; set; }
        public string sdt { get; set; }
        public string email { get; set; }
        public string id_nganh { get; set; }
        public string tennganh { get; set; }
        public string lienket_home_status { get; set; }
        public string lienket_home_label { get; set; }
        public string lienket_home_note { get; set; }
        public string lienket_home_css { get; set; }
        public string lienket_home_action { get; set; }
        public string lienket_home_url { get; set; }
        public string gianhang_admin_access_label { get; set; }
        public string gianhang_admin_access_css { get; set; }
    }

    dbDataContext db = new dbDataContext();
    taikhoan_class tk_cl = new taikhoan_class();
    datetime_class dt_cl = new datetime_class();
    nganh_class ng_cl = new nganh_class();
    public string user, user_parent;
    public string CurrentHomeLinkFilter = "all";
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
        if (HasAnyPermission("q2_4", "n2_4"))
        {
            //main
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

                if (HasAnyPermission("q2_4"))
                {
                }
                else
                {
                    DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList5.Enabled = false;
                }
                if (Session["index_loc_nganh_taikhoan"] != null)//lưu lọc theo theo toán
                    DropDownList5.SelectedIndex = int.Parse(Session["index_loc_nganh_taikhoan"].ToString());
                else
                    Session["index_loc_nganh_taikhoan"] = DropDownList5.SelectedIndex.ToString();

                var q = db.taikhoan_table_2023s.Where(p => p.id_chinhanh == Session["chinhanh"].ToString());
                if (Session["current_page_taikhoan"] == null)//lưu giữ trang hiện tại
                    Session["current_page_taikhoan"] = "1";

                if (Session["index_sapxep_taikhoan"] == null)////lưu sắp xếp
                {
                    DropDownList2.SelectedIndex = 0;
                    Session["index_sapxep_taikhoan"] = DropDownList2.SelectedIndex.ToString();
                }
                else
                    DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_taikhoan"].ToString());

                if (Session["search_taikhoan"] != null)//lưu tìm kiếm
                    txt_search.Text = Session["search_taikhoan"].ToString();
                else
                    Session["search_taikhoan"] = txt_search.Text;

                if (Session["index_trangthai_taikhoan"] != null)//lưu lọc trạng thái
                    DropDownList1.SelectedIndex = int.Parse(Session["index_trangthai_taikhoan"].ToString());
                else
                    Session["index_trangthai_taikhoan"] = DropDownList1.SelectedValue.ToString();

                if (Session["show_taikhoan"] == null)//lưu số dòng mặc định
                {
                    txt_show.Text = "30";
                    Session["show_taikhoan"] = txt_show.Text;
                }
                else
                    txt_show.Text = Session["show_taikhoan"].ToString();

                if (Session["tungay_taikhoan"] == null)
                {
                    txt_tungay.Text = q.Count() != 0 ? q.Min(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                    Session["tungay_taikhoan"] = txt_tungay.Text;
                }
                else
                    txt_tungay.Text = Session["tungay_taikhoan"].ToString();

                if (Session["denngay_taikhoan"] == null)
                {
                    txt_denngay.Text = q.Count() != 0 ? q.Max(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                    Session["denngay_taikhoan"] = txt_denngay.Text;
                }
                else
                    txt_denngay.Text = Session["denngay_taikhoan"].ToString();

                string requestedHomeLinkFilter = (Request.QueryString["home_link"] ?? "").Trim();
                if (requestedHomeLinkFilter != "")
                {
                    Session[SessionHomeLinkFilterKey] = NormalizeHomeLinkFilter(requestedHomeLinkFilter);
                    Session["current_page_taikhoan"] = "1";
                }
                else if (Session[SessionHomeLinkFilterKey] == null)
                {
                    Session[SessionHomeLinkFilterKey] = "all";
                }

                string selectedHomeLinkFilter = NormalizeHomeLinkFilter(((Session[SessionHomeLinkFilterKey] ?? "all") + "").Trim());
                ListItem selectedItem = DropDownList6.Items.FindByValue(selectedHomeLinkFilter);
                if (selectedItem != null)
                    DropDownList6.SelectedValue = selectedHomeLinkFilter;
            }
            CurrentHomeLinkFilter = NormalizeHomeLinkFilter(((Session[SessionHomeLinkFilterKey] ?? "all") + "").Trim());
            if (DropDownList6.Items.Count > 0)
            {
                ListItem selectedItem = DropDownList6.Items.FindByValue(CurrentHomeLinkFilter);
                if (selectedItem != null)
                    DropDownList6.SelectedValue = CurrentHomeLinkFilter;
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
        #region lấy dữ liệu
        List<AccountListItem> list_all = GetFilteredAccountList(CurrentHomeLinkFilter != "all");
        #endregion

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 30;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_taikhoan"].ToString());
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
        if (CurrentHomeLinkFilter == "all")
            DecorateHomeBindingInfo(list_split);
        list_id_split = new List<string>();
        foreach (var t in list_split)
        {
            list_id_split.Add("check_" + t.taikhoan);
        }
        int _s1 = stt + list_split.Count - 1;
        if (list_all.Count() != 0)
            lb_show.Text = "Hiển thị " + stt + "-" + _s1 + " trong số " + list_all.Count().ToString("#,##0") + " mục";
        else
            lb_show.Text = "Hiển thị 0-0 trong số 0";
        Repeater1.DataSource = list_split;
        Repeater1.DataBind();
    }

    private List<AccountListItem> GetFilteredAccountList(bool preloadHomeBinding)
    {
        List<AccountListItem> list_all = (from ob1 in db.taikhoan_table_2023s.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                          select new AccountListItem
                                          {
                                              taikhoan = (ob1.taikhoan ?? "").Trim(),
                                              hoten = (ob1.hoten ?? "").Trim(),
                                              hoten_khongdau = (ob1.hoten_khongdau ?? "").Trim(),
                                              avt = ob1.anhdaidien,
                                              trangthai = (ob1.trangthai ?? "").Trim(),
                                              ngaytao = ob1.ngaytao,
                                              sdt = (ob1.dienthoai ?? "").Trim(),
                                              email = (ob1.email ?? "").Trim(),
                                              id_nganh = (ob1.id_nganh ?? "").Trim(),
                                              tennganh = ng_cl.return_name(ob1.id_nganh),
                                              lienket_home_status = "",
                                              lienket_home_label = "Chưa liên kết",
                                              lienket_home_note = "Nhấn vào chi tiết để liên kết với tài khoản Home.",
                                              lienket_home_css = "bg-gray fg-white",
                                              lienket_home_action = "Mở hồ sơ người",
                                              lienket_home_url = "/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + HttpUtility.UrlEncode((ob1.taikhoan ?? "").Trim()),
                                              gianhang_admin_access_label = "Có vai trò nội bộ nhưng chưa mở quyền /gianhang/admin",
                                              gianhang_admin_access_css = "bg-gray fg-white"
                                          }).ToList();

        DateTime tuNgay = DateTime.Parse(Session["tungay_taikhoan"].ToString()).Date;
        DateTime denNgay = DateTime.Parse(Session["denngay_taikhoan"].ToString()).Date;
        list_all = list_all.Where(p => p.ngaytao.HasValue && p.ngaytao.Value.Date >= tuNgay && p.ngaytao.Value.Date <= denNgay).ToList();

        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            list_all = list_all.Where(p =>
                p.hoten.ToLower().Contains(_key)
                || p.hoten_khongdau.ToLower().Contains(_key)
                || p.taikhoan == _key
                || p.sdt == _key
                || p.email == _key).ToList();
        }

        switch (DropDownList1.SelectedValue.ToString())
        {
            case "0":
                list_all = list_all.Where(p => p.trangthai == "Đang hoạt động").ToList();
                break;
            case "1":
                list_all = list_all.Where(p => p.trangthai == "Đã bị khóa").ToList();
                break;
            default:
                break;
        }

        if (DropDownList5.SelectedValue.ToString() != "")
            list_all = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList();

        if (preloadHomeBinding || CurrentHomeLinkFilter != "all")
        {
            DecorateHomeBindingInfo(list_all);
            list_all = ApplyHomeLinkFilter(list_all, CurrentHomeLinkFilter);
        }

        switch (Session["index_sapxep_taikhoan"].ToString())
        {
            case "0":
                list_all = list_all.OrderBy(p => p.ngaytao).ToList();
                break;
            case "1":
                list_all = list_all.OrderByDescending(p => p.ngaytao).ToList();
                break;
            default:
                break;
        }

        return list_all;
    }

    private List<AccountListItem> ApplyHomeLinkFilter(List<AccountListItem> source, string filterValue)
    {
        if (source == null)
            return new List<AccountListItem>();

        switch (NormalizeHomeLinkFilter(filterValue))
        {
            case "active":
                return source.Where(p => p.lienket_home_status == "active").ToList();
            case "pending":
                return source.Where(p => p.lienket_home_status == "pending").ToList();
            case "none":
                return source.Where(p => p.lienket_home_status == "").ToList();
            default:
                return source;
        }
    }

    private static string NormalizeHomeLinkFilter(string raw)
    {
        string value = (raw ?? "").Trim().ToLowerInvariant();
        switch (value)
        {
            case "active":
            case "pending":
            case "none":
            case "all":
                return value;
            default:
                return "all";
        }
    }

    public string HomeFilterButtonCss(string filterValue)
    {
        string value = NormalizeHomeLinkFilter(filterValue);
        bool active = string.Equals(CurrentHomeLinkFilter, value, StringComparison.OrdinalIgnoreCase);
        if (!active)
            return "button small light";

        switch (value)
        {
            case "active":
                return "button small bg-cyan fg-white";
            case "pending":
                return "button small bg-orange fg-white";
            case "none":
                return "button small bg-gray fg-white";
            default:
                return "button small bg-dark fg-white";
        }
    }

    public string HomeFilterSummaryLabel()
    {
        switch (NormalizeHomeLinkFilter(CurrentHomeLinkFilter))
        {
            case "active":
                return "Đang xem: chỉ các hồ sơ đã liên kết với tài khoản Home.";
            case "pending":
                return "Đang xem: chỉ các hồ sơ chờ tài khoản Home tự liên kết.";
            case "none":
                return "Đang xem: chỉ các hồ sơ chưa gắn với tài khoản Home.";
            default:
                return "Đang xem: toàn bộ hồ sơ nhân sự và chuyên gia.";
        }
    }

    private void DecorateHomeBindingInfo(List<AccountListItem> listSplit)
    {
        if (listSplit == null || listSplit.Count == 0)
            return;

        string ownerKey = (user_parent ?? "").Trim();
        if (ownerKey == "")
            ownerKey = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();

        foreach (AccountListItem item in listSplit)
        {
            string normalizedPhone = AccountAuth_cl.NormalizePhone(item.sdt);
            if (string.IsNullOrWhiteSpace(normalizedPhone))
            {
                item.lienket_home_status = "";
                item.lienket_home_label = "Thiếu số điện thoại";
                item.lienket_home_css = "bg-gray fg-white";
                item.lienket_home_note = "Cập nhật số điện thoại để gom người này vào hồ sơ người chung.";
                item.lienket_home_action = "Cập nhật nhân sự";
                item.lienket_home_url = "/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + HttpUtility.UrlEncode(item.taikhoan ?? "");
                item.gianhang_admin_access_label = "Có vai trò nội bộ nhưng chưa mở quyền /gianhang/admin";
                item.gianhang_admin_access_css = "bg-gray fg-white";
                continue;
            }

            GianHangAdminPersonHub_cl.PersonLinkInfo linkInfo = GianHangAdminPersonHub_cl.GetLinkInfo(db, ownerKey, normalizedPhone, item.hoten);
            item.lienket_home_status = linkInfo == null ? "" : ((linkInfo.Status ?? "").Trim().ToLowerInvariant());
            item.lienket_home_label = linkInfo == null ? "Chưa liên kết" : linkInfo.StatusLabel;
            item.lienket_home_css = linkInfo == null ? "bg-gray fg-white" : linkInfo.LinkCss;
            item.lienket_home_action = "Mở hồ sơ người";
            item.lienket_home_url = GianHangAdminPersonHub_cl.BuildDetailUrl(normalizedPhone);
            GianHangAdminPersonHub_cl.PersonSourceRef sourceInfo = GianHangAdminPersonHub_cl.GetSourceInfo(db, ownerKey, normalizedPhone, "staff", item.taikhoan);
            item.gianhang_admin_access_label = sourceInfo == null || string.IsNullOrWhiteSpace(sourceInfo.AdminAccessLabel)
                ? "Có vai trò nội bộ nhưng chưa mở quyền /gianhang/admin"
                : sourceInfo.AdminAccessLabel;
            item.gianhang_admin_access_css = sourceInfo == null || string.IsNullOrWhiteSpace(sourceInfo.AdminAccessCss)
                ? "bg-gray fg-white"
                : sourceInfo.AdminAccessCss;

            if (linkInfo != null && linkInfo.LinkedHomeAccount != null)
            {
                string accountKey = (linkInfo.LinkedHomeAccount.taikhoan ?? "").Trim();
                string phone = (linkInfo.LinkedHomeAccount.dienthoai ?? "").Trim();
                item.lienket_home_note = "Home: " + accountKey;
                if (phone != "")
                    item.lienket_home_note += " - " + phone;
                continue;
            }

            if (linkInfo != null && string.Equals(linkInfo.Status, "pending", StringComparison.OrdinalIgnoreCase))
            {
                item.lienket_home_note = (linkInfo.PendingPhone ?? "").Trim();
                if (item.lienket_home_note == "")
                    item.lienket_home_note = "Đang chờ tài khoản Home đăng ký hoặc đăng nhập.";
                else
                    item.lienket_home_note += " - Chờ tài khoản Home xác lập tự động";
                continue;
            }

            item.lienket_home_note = "Mở hồ sơ người để liên kết hoặc tạo chờ liên kết theo số điện thoại này.";
        }
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
        Session["current_page_taikhoan"] = "1";

        main();
    }
    //protected void txt_show_TextChanged(object sender, EventArgs e)
    //{
    //    Session["current_page_taikhoan"] = "1";
    //    main();
    //}
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_taikhoan"] = int.Parse(Session["current_page_taikhoan"].ToString()) - 1;
        if (int.Parse(Session["current_page_taikhoan"].ToString()) < 1)
            Session["current_page_taikhoan"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_taikhoan"] = int.Parse(Session["current_page_taikhoan"].ToString()) + 1;
        if (int.Parse(Session["current_page_taikhoan"].ToString()) > total_page)
            Session["current_page_taikhoan"] = total_page;
        main();
    }
    #endregion


    protected void Button1_Click(object sender, EventArgs e)
    {
        Session["index_trangthai_taikhoan"] = DropDownList1.SelectedIndex;
        Session["index_sapxep_taikhoan"] = DropDownList2.SelectedIndex;
        Session["current_page_taikhoan"] = "1";
        Session["search_taikhoan"] = txt_search.Text.Trim();
        Session["show_taikhoan"] = txt_show.Text.Trim();
        Session["tungay_taikhoan"] = txt_tungay.Text;
        Session["denngay_taikhoan"] = txt_denngay.Text;

        Session["index_loc_nganh_taikhoan"] = DropDownList5.SelectedIndex;
        Session[SessionHomeLinkFilterKey] = NormalizeHomeLinkFilter(DropDownList6.SelectedValue);
        CurrentHomeLinkFilter = NormalizeHomeLinkFilter(DropDownList6.SelectedValue);
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        Session["index_trangthai_taikhoan"] = null;
        Session["index_sapxep_taikhoan"] = null;
        Session["current_page_taikhoan"] = null;
        Session["search_taikhoan"] = null;
        Session["show_taikhoan"] = null;
        Session["tungay_taikhoan"] = null;
        Session["denngay_taikhoan"] = null;

        Session["index_loc_nganh_taikhoan"] = null;
        Session[SessionHomeLinkFilterKey] = null;
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/Default.aspx");
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

    protected void Button4_Click(object sender, EventArgs e)
    {
        List<AccountListItem> list_all = GetFilteredAccountList(CurrentHomeLinkFilter != "all");
        if (check_list_excel.Items.Count == 0)
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
        else
        {
            // khởi tạo wb rỗng
            XSSFWorkbook wb = new XSSFWorkbook();

            // Tạo ra 1 sheet
            ISheet sheet = wb.CreateSheet();

            // Bắt đầu ghi lên sheet

            // Tạo row
            var row0 = sheet.CreateRow(0);
            // Merge lại row đầu 3 cột
            row0.CreateCell(0); // tạo ra cell trc khi merge
            CellRangeAddress cellMerge = new CellRangeAddress(0, 0, 0, 2);
            sheet.AddMergedRegion(cellMerge);
            row0.GetCell(0).SetCellValue("Dữ liệu của bạn xuất ngày " + DateTime.Now);

            // Ghi tiêu đề cột ở row 1
            var row1 = sheet.CreateRow(1);

            //đếm xem có bao nhiêu cột đc chọn
            int _socot = 0;
            for (int i = 0; i < check_list_excel.Items.Count; i++)//duyệt hết các phần tử trong list check
            {
                if (check_list_excel.Items[i].Selected)//nếu cột này đc chọn
                {
                    //thì tạo cột tiêu đề
                    row1.CreateCell(_socot).SetCellValue(check_list_excel.Items[i].Text);
                    _socot = _socot + 1;
                }
            }

            // bắt đầu duyệt mảng và ghi tiếp tục
            int rowIndex = 2;
            foreach (var item in list_all)
            {
                // tao row mới
                var newRow = sheet.CreateRow(rowIndex);

                // set giá trị
                int _socot1 = 0;
                for (int i = 0; i < check_list_excel.Items.Count; i++)//duyệt hết các phần tử trong list check
                {
                    if (check_list_excel.Items[i].Selected)//nếu cột này đc chọn
                    {
                        string _tencot = check_list_excel.Items[i].Value;
                        switch (_tencot)
                        {
                            //case "ngaysinh":
                            //    if (item.ngaysinh != null)
                            //    {
                            //        newRow.CreateCell(_socot1).SetCellValue(item.ngaysinh.Value.ToString("dd/MM/yyyy"));
                            //        break;
                            //    }
                            //    else
                            //    {
                            //        newRow.CreateCell(_socot1).SetCellValue("");
                            //        break;
                            //    }
                            case "taikhoan": newRow.CreateCell(_socot1).SetCellValue(item.taikhoan); break;
                            default: break;
                        }
                        _socot1 = _socot1 + 1;
                    }
                }

                // tăng index
                rowIndex++;
            }

            // xong hết thì save file lại
            string _filename = Guid.NewGuid().ToString();
            FileStream fs = new FileStream(Server.MapPath("~/uploads/Files/" + _filename + ".xlsx"), FileMode.CreateNew);
            wb.Write(fs);
            Response.Redirect("/uploads/Files/" + _filename + ".xlsx");

        }
    }

    protected void but_khoa_Click(object sender, ImageClickEventArgs e)
    {
        if (HasAnyPermission("q2_3", "n2_3"))
        {
            int _count = 0;
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _user = list_id_split[i].Replace("check_", "");
                    if (tk_cl.exist_user(_user))
                    {
                        var q = db.taikhoan_table_2023s.Where(p => p.taikhoan.ToString() == _user && p.id_chinhanh == Session["chinhanh"].ToString());
                        taikhoan_table_2023 _ob = q.First();
                        _ob.trangthai = "Đã bị khóa";
                        db.SubmitChanges();
                        GianHangAdminWorkspace_cl.SyncLegacySourceAccess(db, user_parent, _ob.taikhoan, false);
                        _count = _count + 1;
                    }
                }
            }
            if (_count > 0)
            {
                //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Khóa thành công.", "4000", "warning");
                //Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/Default.aspx");
                main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Khóa tài khoản thành công", "4000", "warning"), true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
            }
            
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
    }



    protected void but_mokhoa_Click(object sender, ImageClickEventArgs e)
    {
        if (HasAnyPermission("q2_3", "n2_3"))
        {
            int _count = 0;
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _user = list_id_split[i].Replace("check_", "");
                    if (tk_cl.exist_user(_user))
                    {
                        var q = db.taikhoan_table_2023s.Where(p => p.taikhoan.ToString() == _user && p.id_chinhanh == Session["chinhanh"].ToString());
                        taikhoan_table_2023 _ob = q.First();
                        _ob.trangthai = "Đang hoạt động";
                        db.SubmitChanges();
                        GianHangAdminWorkspace_cl.SyncLegacySourceAccess(db, user_parent, _ob.taikhoan, true);
                        _count = _count + 1;
                    }
                }
            }
            if (_count > 0)
            {
                //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Khóa thành công.", "4000", "warning");
                //Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/Default.aspx");
                main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mở khóa tài khoản thành công", "4000", "warning"), true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
            }
            
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
    }
}
