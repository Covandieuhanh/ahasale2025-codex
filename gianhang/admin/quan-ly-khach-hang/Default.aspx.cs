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


public partial class badmin_Default : System.Web.UI.Page
{
    private sealed class CustomerListRow
    {
        public Int64 id { get; set; }
        public string sdt { get; set; }
        public string tenkhachhang { get; set; }
        public string diachi { get; set; }
        public int sl_hoadon { get; set; }
        public string avt { get; set; }
        public DateTime? ngaytao { get; set; }
        public string nguoitao { get; set; }
        public Int64 sauck { get; set; }
        public Int64 sotien_conlai { get; set; }
        public int sl_dv { get; set; }
        public int sl_sp { get; set; }
        public string tennhom { get; set; }
        public string idnhom { get; set; }
    }

    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    string_class str_cl = new string_class();
    data_khachhang_class dtkh_cl = new data_khachhang_class();
    datetime_class dt_cl = new datetime_class();
    public string user, user_parent, show_add = "none";
    public int tongdon, tong_sl_dv, tong_sl_sp;
    public Int64 sauck, tong_congno;
    #region phân trang
    public int stt = 1, current_page = 1, show = 50, total_page = 1;
    List<string> list_id_split;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login  
        string _quyen = "q8_1";
        string _cookie_user = "", _cookie_pass = "";
        if (Request.Cookies["save_user_admin_aka_1"] != null) _cookie_user = Request.Cookies["save_user_admin_aka_1"].Value;
        if (Request.Cookies["save_pass_admin_aka_1"] != null) _cookie_pass = Request.Cookies["save_pass_admin_aka_1"].Value;
        if (Session["user"] == null) Session["user"] = ""; if (Session["notifi"] == null) Session["notifi"] = ""; if (Session["user"].ToString() == "") Response.Redirect("/gianhang/admin/f5_ss_admin.aspx");
        string _url = Request.Url.GetLeftPart(UriPartial.Authority).ToLower();
        string _kq = bcorn_class.check_login(Session["user"].ToString(), _cookie_user, _cookie_pass, _url, _quyen);
        if (_kq != "")//nếu có thông báo --> có lỗi --> reset --> bắt login lại
        {
            if (_kq == "baotri") Response.Redirect("/baotri.aspx");
            else
            {
                if (_kq == "1") Response.Redirect("/gianhang/admin/login.aspx");//hết Session, hết Cookie
                else
                {
                    if (_kq == "2")//k đủ quyền
                    {
                        Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
                        Response.Redirect("/gianhang/admin");
                    }
                    else
                    {
                        Session["notifi"] = _kq; Session["user"] = "";
                        Response.Cookies["save_user_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["save_pass_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["save_url_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Redirect("/gianhang/admin/login.aspx");
                    }
                }
            }
        }
        #endregion
        user = Session["user"].ToString();
        user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();
        if (!IsPostBack)
        {
            var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                 select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                      );
            ddl_nhanvien_chamsoc.DataSource = list_nhanvien;
            ddl_nhanvien_chamsoc.DataTextField = "tennhanvien";
            ddl_nhanvien_chamsoc.DataValueField = "username";
            ddl_nhanvien_chamsoc.DataBind();
            ddl_nhanvien_chamsoc.Items.Insert(0, new ListItem("Chọn", ""));

            var list_nohmkhachhang = (from ob1 in db.nhomkhachhang_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                      select new { id = ob1.id, tennhom = ob1.tennhom, }
                      );
            DropDownList1.DataSource = list_nohmkhachhang;
            DropDownList1.DataTextField = "tennhom";
            DropDownList1.DataValueField = "id";
            DropDownList1.DataBind();
            DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));

            DropDownList3.DataSource = list_nohmkhachhang;
            DropDownList3.DataTextField = "tennhom";
            DropDownList3.DataValueField = "id";
            DropDownList3.DataBind();
            DropDownList3.Items.Insert(0, new ListItem("Tất cả", ""));


            var q = db.bspa_hoadon_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString());

            if (Session["current_page_dtkh"] == null)//lưu giữ trang hiện tại
                Session["current_page_dtkh"] = "1";

            if (Session["index_sapxep_dtkh"] == null)////lưu sắp xếp
            {
                DropDownList2.SelectedIndex = 1;//mặc định là giảm dần theo thời gian
                Session["index_sapxep_dtkh"] = DropDownList2.SelectedIndex.ToString();
            }
            else
                DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_dtkh"].ToString());

            if (Session["search_dtkh"] != null)//lưu tìm kiếm
                txt_search.Text = Session["search_dtkh"].ToString();
            else
                Session["search_dtkh"] = txt_search.Text;

            if (Session["show_dtkh"] == null)//lưu số dòng mặc định
            {
                txt_show.Text = "30";
                Session["show_dtkh"] = txt_show.Text;
            }
            else
                txt_show.Text = Session["show_dtkh"].ToString();

            if (Session["tungay_dtkh"] == null)
            {
                txt_tungay.Text = q.Count() != 0 ? q.Min(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                Session["tungay_dtkh"] = txt_tungay.Text;
            }
            else
                txt_tungay.Text = Session["tungay_dtkh"].ToString();

            if (Session["denngay_dtkh"] == null)
            {
                txt_denngay.Text = q.Count() != 0 ? q.Max(p => p.ngaytao.Value).ToString("dd/MM/yyyy") : DateTime.Now.ToShortDateString();
                Session["denngay_dtkh"] = txt_denngay.Text;
            }
            else
                txt_denngay.Text = Session["denngay_dtkh"].ToString();

            if (Session["index_loc_nhomkh_dtkh"] != null)//lưu lọc theo nhóm kh
            {
                DropDownList3.SelectedIndex = int.Parse(Session["index_loc_nhomkh_dtkh"].ToString());
            }
            else
                Session["index_loc_nhomkh_dtkh"] = DropDownList3.SelectedIndex.ToString();

            if (Session["index_loc_nguon_dtkh"] != null)
                ddl_trangthai_nguon.SelectedIndex = int.Parse(Session["index_loc_nguon_dtkh"].ToString());
            else
                Session["index_loc_nguon_dtkh"] = ddl_trangthai_nguon.SelectedIndex.ToString();
        }
        main();
        if (!string.IsNullOrWhiteSpace(Request.QueryString["q"]))//khi ng dùng nhấn vào nút tạo hóa đơn từ menutop --> tạo nhanh
        {
            string _q = Request.QueryString["q"].ToString().Trim();
            if (_q == "add")
                show_add = "block";
        }
    }
    public void main()
    {
        string branchId = Session["chinhanh"].ToString();
        var query = db.bspa_data_khachhang_tables.Where(p => p.id_chinhanh == branchId);

        string keyword = txt_search.Text.Trim();
        if (keyword != "")
            query = query.Where(p => (p.tenkhachhang != null && p.tenkhachhang.Contains(keyword)) || (p.sdt != null && p.sdt.Contains(keyword)));

        if (DropDownList3.SelectedValue.ToString() != "")
            query = query.Where(p => p.nhomkhachhang == DropDownList3.SelectedValue.ToString());

        List<Int64> inactiveCustomerIds = GianHangAdminSourceLifecycle_cl
            .GetInactiveKeySet(db, user_parent, "customer")
            .Select(p =>
            {
                Int64 id;
                return Int64.TryParse((p ?? "").Trim(), out id) ? (Int64?)id : null;
            })
            .Where(p => p.HasValue)
            .Select(p => p.Value)
            .Distinct()
            .ToList();

        switch (ddl_trangthai_nguon.SelectedValue.ToString())
        {
            case "1":
                if (inactiveCustomerIds.Count != 0)
                    query = query.Where(p => !inactiveCustomerIds.Contains(p.id));
                break;
            case "2":
                if (inactiveCustomerIds.Count != 0)
                    query = query.Where(p => inactiveCustomerIds.Contains(p.id));
                else
                    query = query.Where(p => false);
                break;
        }

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 50;
        txt_show.Text = show.ToString();

        int totalRows = query.Count();
        total_page = number_of_page_class.return_total_page(totalRows, show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_dtkh"].ToString());
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
        bool sortAsc = Session["index_sapxep_dtkh"].ToString() == "0";
        query = sortAsc ? query.OrderBy(p => p.ngaytao) : query.OrderByDescending(p => p.ngaytao);
        var pageRows = query.Skip(current_page * show - show)
            .Take(show)
            .Select(p => new
            {
                p.id,
                p.sdt,
                p.tenkhachhang,
                p.diachi,
                p.anhdaidien,
                p.ngaytao,
                p.nguoitao,
                p.nhomkhachhang
            })
            .ToList();

        var groupIds = pageRows
            .Select(p => (p.nhomkhachhang ?? "").Trim())
            .Where(p => p != "")
            .Distinct()
            .ToList();
        var groupIdNumbers = groupIds
            .Select(p =>
            {
                Int64 value;
                return Int64.TryParse(p, out value) ? (Int64?)value : null;
            })
            .Where(p => p.HasValue)
            .Select(p => p.Value)
            .Distinct()
            .ToList();
        var groupMap = db.nhomkhachhang_tables
            .Where(p => p.id_chinhanh == branchId && groupIdNumbers.Contains(p.id))
            .Select(p => new { id = p.id.ToString(), p.tennhom })
            .ToList()
            .ToDictionary(p => p.id, p => p.tennhom ?? "");

        var creatorKeys = pageRows
            .Select(p => (p.nguoitao ?? "").Trim())
            .Where(p => p != "")
            .Distinct()
            .ToList();
        var creatorMap = db.taikhoan_table_2023s
            .Where(p => creatorKeys.Contains(p.taikhoan))
            .Select(p => new { p.taikhoan, p.hoten })
            .ToList()
            .ToDictionary(p => p.taikhoan, p => p.hoten ?? "");

        var list_split = pageRows.Select(p =>
        {
            string groupId = (p.nhomkhachhang ?? "").Trim();
            string creatorKey = (p.nguoitao ?? "").Trim();
            string creatorName;
            if (!creatorMap.TryGetValue(creatorKey, out creatorName) || creatorName == "")
                creatorName = creatorKey;

            return new CustomerListRow
            {
                id = p.id,
                sdt = p.sdt,
                tenkhachhang = p.tenkhachhang,
                diachi = p.diachi,
                sl_hoadon = 0,
                avt = p.anhdaidien,
                ngaytao = p.ngaytao,
                nguoitao = creatorName,
                sauck = 0,
                sotien_conlai = 0,
                sl_dv = 0,
                sl_sp = 0,
                tennhom = groupId != "" && groupMap.ContainsKey(groupId) ? groupMap[groupId] : "",
                idnhom = groupId
            };
        }).ToList();
        list_id_split = new List<string>();
        foreach (var t in list_split)
        {
            list_id_split.Add("check_" + t.id);
        }
        int _s1 = stt + list_split.Count - 1;
        if (totalRows != 0)
            lb_show.Text = "Hiển thị " + stt + "-" + _s1 + " trong số " + totalRows.ToString("#,##0") + " mục";
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
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_dtkh"] = int.Parse(Session["current_page_dtkh"].ToString()) - 1;
        if (int.Parse(Session["current_page_dtkh"].ToString()) < 1)
            Session["current_page_dtkh"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_dtkh"] = int.Parse(Session["current_page_dtkh"].ToString()) + 1;
        if (int.Parse(Session["current_page_dtkh"].ToString()) > total_page)
            Session["current_page_dtkh"] = total_page;
        main();
    }

    protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            return;

        Literal litPersonHub = e.Item.FindControl("litPersonHub") as Literal;
        Literal litLifecycle = e.Item.FindControl("litLifecycle") as Literal;
        if (litPersonHub == null || litLifecycle == null)
            return;

        string displayName = (DataBinder.Eval(e.Item.DataItem, "tenkhachhang") + "").Trim();
        string phone = (DataBinder.Eval(e.Item.DataItem, "sdt") + "").Trim();
        string sourceKey = (DataBinder.Eval(e.Item.DataItem, "id") + "").Trim();
        litPersonHub.Text = BuildPersonHubListHtml(displayName, phone, "khách hàng", "customer", sourceKey);
        litLifecycle.Text = BuildSourceLifecycleHtml(sourceKey);
    }

    private void ApplySearchState()
    {
        Session["search_dtkh"] = txt_search.Text.Trim();
        Session["current_page_dtkh"] = "1";
        main();
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

    private string BuildSourceLifecycleHtml(string sourceKey)
    {
        GianHangAdminSourceLifecycle_cl.SourceLifecycleInfo info = GianHangAdminSourceLifecycle_cl.GetInfo(
            db,
            user_parent,
            "customer",
            sourceKey,
            "Đang dùng khách hàng",
            "Đã ngừng dùng khách hàng",
            "Hồ sơ khách hàng này đang được dùng bình thường trong module nguồn.",
            "Hồ sơ khách hàng này đang ở trạng thái ngừng dùng an toàn. Lịch sử và liên kết Home trung tâm vẫn được giữ.");

        string css = info == null || string.IsNullOrWhiteSpace(info.Css) ? "bg-green fg-white" : info.Css;
        string label = info == null || string.IsNullOrWhiteSpace(info.Label) ? "Đang dùng khách hàng" : info.Label;
        string note = info == null || string.IsNullOrWhiteSpace(info.Note)
            ? "Hồ sơ khách hàng này đang được dùng bình thường trong module nguồn."
            : info.Note;

        return "<span class='data-wrapper'><code class='" + HttpUtility.HtmlAttributeEncode(css) + "'>" + HttpUtility.HtmlEncode(label) + "</code></span>"
            + "<div class='mt-1'><small class='fg-gray'>" + HttpUtility.HtmlEncode(note) + "</small></div>";
    }



    protected void but_xoa_Click(object sender, ImageClickEventArgs e)
    {
        if (bcorn_class.check_quyen(user, "q8_4") == "")
        {
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    if (dtkh_cl.exist_id(_id, user_parent))
                        dtkh_cl.del(_id);
                }
            }
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "warning"), true);
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }

    protected void but_ngung_Click(object sender, EventArgs e)
    {
        UpdateSelectedCustomerLifecycle(false, "Đã chuyển các khách hàng được chọn sang trạng thái ngừng dùng an toàn.");
    }

    protected void but_molai_Click(object sender, EventArgs e)
    {
        UpdateSelectedCustomerLifecycle(true, "Đã mở lại các khách hàng được chọn.");
    }

    private void UpdateSelectedCustomerLifecycle(bool makeActive, string successMessage)
    {
        if (bcorn_class.check_quyen(user, "q8_3") != "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Bạn không đủ quyền để đổi trạng thái khách hàng.", "4000", "warning"), true);
            return;
        }

        int affected = 0;
        for (int i = 0; i < list_id_split.Count; i++)
        {
            if (Request.Form[list_id_split[i]] != "on")
                continue;

            string selectedId = list_id_split[i].Replace("check_", "");
            if (!dtkh_cl.exist_id(selectedId, user_parent))
                continue;

            if (makeActive)
                GianHangAdminSourceLifecycle_cl.SetActive(db, user_parent, "customer", selectedId, user, "Khách hàng được mở lại từ danh sách khách hàng.");
            else
                GianHangAdminSourceLifecycle_cl.SetInactive(db, user_parent, "customer", selectedId, user, "Khách hàng được chuyển sang trạng thái ngừng dùng an toàn từ danh sách khách hàng.");

            affected++;
        }

        if (affected > 0)
        {
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", successMessage, "4000", makeActive ? "success" : "warning"), true);
            return;
        }

        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có khách hàng nào phù hợp để đổi trạng thái.", "4000", "warning"), true);
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

    public void reset_ss()
    {
        Session["index_sapxep_dtkh"] = null;
        Session["current_page_dtkh"] = null;
        Session["search_dtkh"] = null;
        Session["show_dtkh"] = null;
        Session["tungay_dtkh"] = null;
        Session["denngay_dtkh"] = null;

        Session["index_loc_nhomkh_dtkh"] = null;
        Session["index_loc_nguon_dtkh"] = null;

    }
    protected void Button2_Click(object sender, EventArgs e)//reset
    {
        reset_ss();
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-khach-hang/Default.aspx");
    }

    protected void Button1_Click(object sender, EventArgs e)//but lọc
    {
        Session["index_sapxep_dtkh"] = DropDownList2.SelectedIndex;
        Session["current_page_dtkh"] = "1";
        Session["search_dtkh"] = txt_search.Text.Trim();
        Session["show_dtkh"] = txt_show.Text.Trim();
        Session["tungay_dtkh"] = txt_tungay.Text;
        Session["denngay_dtkh"] = txt_denngay.Text;

        Session["index_loc_nhomkh_dtkh"] = DropDownList3.SelectedIndex;
        Session["index_loc_nguon_dtkh"] = ddl_trangthai_nguon.SelectedIndex;

        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        string _tenkhachhang = str_cl.VietHoa_ChuCai_DauTien(txt_tenkhachhang.Text.Trim().ToLower());
        string _sdt = str_cl.remove_blank(txt_sdt.Text.Trim()).Replace(" ", "").Replace(".", "").Replace("-", "").Replace("+", "");
        string _diachi = txt_diachi.Text;
        if (_tenkhachhang == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tên khách hàng.", "false", "false", "OK", "alert", ""), true);
        else
        {
            if (_sdt == "")
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập số điện thoại khách hàng.", "false", "false", "OK", "alert", ""), true);
            else
            {
                var q_data = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt && p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString());
                if (q_data.Count() == 0)//chưa có sdt thì thêm vào
                {
                    bspa_data_khachhang_table _ob1 = new bspa_data_khachhang_table();
                    _ob1.tenkhachhang = _tenkhachhang;
                    _ob1.diachi = _diachi;
                    _ob1.magioithieu = txt_magioithieu.Text.Trim();
                    _ob1.ngaytao = DateTime.Now;
                    _ob1.nguoitao = user;
                    _ob1.nguoichamsoc = ddl_nhanvien_chamsoc.SelectedValue.ToString();
                    _ob1.sdt = _sdt;
                    _ob1.user_parent = user_parent;
                    _ob1.nhomkhachhang = DropDownList1.SelectedValue.ToString();
                    _ob1.matkhau= encode_class.encode_md5(encode_class.encode_sha1("12345678"));
                    _ob1.anhdaidien = "/uploads/images/macdinh.jpg";
                    if (txt_ngaysinh.Text != "" && dt_cl.check_date(txt_ngaysinh.Text) == true)
                        _ob1.ngaysinh = DateTime.Parse(txt_ngaysinh.Text);

                    _ob1.id_chinhanh = Session["chinhanh"].ToString();
                    _ob1.capbac = ""; _ob1.vnd_tu_e_aha = 0; _ob1.sodiem_e_aha = 0; _ob1.solan_lencap = 0;
                    db.bspa_data_khachhang_tables.InsertOnSubmit(_ob1);
                    db.SubmitChanges();
                    GianHangAdminPersonHub_cl.SyncSourcePhoneState(db, user_parent, "", _sdt, _tenkhachhang, user);

                    reset_control();
                    main();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm thành công.", "4000", "warning"), true);
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Số điện thoại đã tồn tại.", "false", "false", "OK", "alert", ""), true);
            }
        }
    }

    protected void Button4_Click(object sender, EventArgs e)
    {
        #region lấy dữ liệu
        var list_all = db.bspa_data_khachhang_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).Select(p => new
        {
            id = p.id,
            sdt = p.sdt,
            tenkhachhang = p.tenkhachhang,
            diachi = p.diachi,
            sl_hoadon = 0,
            avt = p.anhdaidien,
            ngaytao = p.ngaytao,
            nguoitao = tk_cl.return_object(p.nguoitao).hoten,
            //tongtien = Int64.Parse("0"),
            //sauck = Int64.Parse("0"),
            //dathanhtoan = Int64.Parse("0"),
            //sotien_conlai = Int64.Parse("0"),
            //sl_dv = int.Parse("0"),
            //sl_sp = int.Parse("0"),
        }).ToList();

        //list_all = list_all.Union(list_data).ToList();

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.tenkhachhang.ToLower().Contains(_key)).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        //tongdon = list_all.Sum(p => p.sl_hoadon);
        //tong_sl_dv = list_all.Sum(p => p.sl_dv);
        //tong_sl_sp = list_all.Sum(p => p.sl_sp);

        //tongtien = list_all.Sum(p => p.tongtien);
        //sauck = list_all.Sum(p => p.sauck);
        //tong_thanhtoan = list_all.Sum(p => p.dathanhtoan);
        //tong_congno = list_all.Sum(p => p.sotien_conlai);

        //sắp xếp
        switch (Session["index_sapxep_dtkh"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.ngaytao).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
            default: list_all = list_all.OrderByDescending(p => p.ngaytao).ToList(); break;
        }
        #endregion
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
                            case "tenkhachhang": newRow.CreateCell(_socot1).SetCellValue(item.tenkhachhang); break;
                            case "diachi": newRow.CreateCell(_socot1).SetCellValue(item.diachi); break;
                            case "sdt": newRow.CreateCell(_socot1).SetCellValue(item.sdt); break;
                            default: break;
                        }
                        _socot1 = _socot1 + 1;
                    }
                }

                // tăng index
                rowIndex++;
            }

            // xong hết thì save file lại
            string _filetenbv = Guid.NewGuid().ToString();
            FileStream fs = new FileStream(Server.MapPath("~/uploads/Files/" + _filetenbv + ".xlsx"), FileMode.CreateNew);
            wb.Write(fs);
            Response.Redirect("/uploads/Files/" + _filetenbv + ".xlsx");
        }
    }
    public void reset_control()
    {
        txt_tenkhachhang.Text = ""; txt_sdt.Text = "";
        txt_ngaysinh.Text = ""; txt_diachi.Text = "";
        ddl_nhanvien_chamsoc.SelectedIndex = 0;
        txt_magioithieu.Text = "";
    }
}
