using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class taikhoan_add : System.Web.UI.Page
{
    public string id, sdt, notifi, user, user_parent;
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
    public string personHubUrl = "/gianhang/admin/quan-ly-con-nguoi/Default.aspx";
    public string personHubStatusLabel = "Chưa liên kết";
    public string personHubStatusCss = "bg-gray fg-white";
    public string personHubNote = "Mở hồ sơ người để liên kết hoặc tạo chờ liên kết theo số điện thoại này.";
    public string personHubRelatedRolesHtml = "";
    public string personHubAdminAccessLabel = "Không mở quyền /gianhang/admin ở vai trò khách hàng";
    public string personHubAdminAccessCss = "bg-gray fg-white";
    public string personHubAdminAccessNote = "Vai trò khách hàng chỉ là dữ liệu nghiệp vụ. Nếu cùng người này còn là nhân sự nội bộ thì quyền vào /gianhang/admin sẽ được quyết định ở hồ sơ nhân sự nội bộ đó.";
    public string personHubImpactTitle = "Tác động khi xóa vai trò khách hàng";
    public string personHubImpactNote = "Xóa hồ sơ khách hàng này sẽ không tự gỡ liên kết Home ở Hồ sơ người. Nếu cùng số điện thoại còn xuất hiện ở vai trò khác trong gian hàng, người này vẫn được gom chung ở hồ sơ trung tâm.";
    public string sourceLifecycleLabel = "Đang dùng khách hàng";
    public string sourceLifecycleCss = "bg-green fg-white";
    public string sourceLifecycleNote = "Hồ sơ khách hàng này đang được dùng bình thường trong module nguồn.";
    public bool sourceLifecycleIsInactive = false;

    // BỔ SUNG EMAIL (giả định cột trong bspa_data_khachhang_table là: email)
    public string email;
    // /BỔ SUNG EMAIL

    taikhoan_class tk_cl = new taikhoan_class();
    hoadon_class hd_cl = new hoadon_class();
    dbDataContext db = new dbDataContext();
    string_class str_cl = new string_class();
    datetime_class dt_cl = new datetime_class();
    data_khachhang_class dt_kh_cl = new data_khachhang_class(); thedichvu_class tdv_cl = new thedichvu_class();
    post_class po_cl = new post_class();
    public string hoten;
    public int tong_hoadon = 0, tong_dv, tong_sp;
    public Int64 tong_chitieu = 0, tong_congno = 0;
    public List<string> list_id_split_ghichu, list_id_split_donthuoc, list_id_split_hinhanh, list_id_split_thedv;
    public string act_hinhanh, act_thedv;
    public bool co_ngu_canh_datlich = false;
    public string id_datlich_lienket = "", ten_dichvu_datlich = "", ngay_datlich_lienket = "", ten_nhanvien_datlich = "", url_quay_lai_datlich = "", thongbao_datlich_thedv = "";
    public List<string> list_id_thedv_goiy = new List<string>();
    public string id_thedv_tu_dong_goiy = "";
    public khachhang_vanhanh_tongquan tongquan_crm = new khachhang_vanhanh_tongquan();

    //thẻ dịch vụ
    public Int64 sl_thedv = 0, doanhso_tdv = 0, doanhso_tdv_sauck = 00, tongtien_dathanhtoan_tdv = 0, tong_congno_tdv = 0;
    public int stt_tdv = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        user = (access.User ?? "").Trim();
        user_parent = access.OwnerAccountKey;
        if (!HasAnyPermission("q8_1"))
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
            return;
        }
        if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
        {
            id = Request.QueryString["id"].ToString().Trim();
            var q = db.bspa_data_khachhang_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
            if (q.Count() != 0)
            {
                bspa_data_khachhang_table currentCustomer = q.First();
                BindPersonHub(currentCustomer.tenkhachhang, currentCustomer.sdt);
                BindCustomerLifecycle(currentCustomer);

                main();
                nap_ngu_canh_datlich();
                nap_tongquan_crm();
                show_hoadon();
                show_lichhen();
                show_ghichu();
                show_hinhanh();
                show_donthuoc();
                show_thedichvu();

                if (!IsPostBack)
                {
                    txt_ngayban_thedv.Text = DateTime.Now.ToString();
                    var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.user_parent == user_parent && p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                         select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                         );
                    ddl_nhanvien_lamdichvu_thedv.DataSource = list_nhanvien;
                    ddl_nhanvien_lamdichvu_thedv.DataTextField = "tennhanvien";
                    ddl_nhanvien_lamdichvu_thedv.DataValueField = "username";
                    ddl_nhanvien_lamdichvu_thedv.DataBind();
                    ddl_nhanvien_lamdichvu_thedv.Items.Insert(0, new ListItem("Chọn", ""));
                    ap_dung_ngu_canh_datlich_vao_form_thedv();
                }

                if (!string.IsNullOrWhiteSpace(Request.QueryString["act"]))
                {
                    string _act = Request.QueryString["act"].ToString().Trim();
                    switch (_act)
                    {
                        case ("hinhanh"): act_hinhanh = "active"; break;
                        case ("thedv"): act_thedv = "active"; break;
                    }
                }
                else if (co_ngu_canh_datlich)
                    act_thedv = "active";
            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin/Default.aspx");
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin/Default.aspx");
        }
    }
    public string check_hsd(string _hsd)
    {
        if (DateTime.Now.Date > DateTime.Parse(_hsd).Date)
            return "<span class='fg-red'>" + _hsd + "</span>";
        else
            return _hsd;
    }

    private void nap_tongquan_crm()
    {
        tongquan_crm = khachhang_vanhanh_class.tai_tongquan(db, sdt, Session["chinhanh"].ToString());
    }

    private void BindPersonHub(string displayName, string phone)
    {
        string normalizedPhone = AccountAuth_cl.NormalizePhone(phone);
        if (string.IsNullOrWhiteSpace(normalizedPhone))
        {
            personHubUrl = "/gianhang/admin/quan-ly-con-nguoi/Default.aspx";
            personHubStatusLabel = "Thiếu số điện thoại";
            personHubStatusCss = "bg-gray fg-white";
            personHubNote = "Cập nhật số điện thoại để khách hàng này được gom vào hồ sơ người chung.";
            return;
        }

        personHubUrl = GianHangAdminPersonHub_cl.BuildDetailUrl(normalizedPhone);
        BindSourceAdminAccess(normalizedPhone);
        personHubRelatedRolesHtml = BuildRelatedRolesHtml(normalizedPhone);
        personHubImpactNote = BuildImpactNote(normalizedPhone);
        GianHangAdminPersonHub_cl.PersonLinkInfo linkInfo = GianHangAdminPersonHub_cl.GetLinkInfo(db, user_parent, normalizedPhone, displayName);
        if (linkInfo == null)
            return;

        personHubStatusLabel = linkInfo.StatusLabel;
        personHubStatusCss = linkInfo.LinkCss;
        if (linkInfo.LinkedHomeAccount != null)
        {
            string linkedName = string.IsNullOrWhiteSpace(linkInfo.LinkedHomeAccount.hoten)
                ? (linkInfo.LinkedHomeAccount.taikhoan ?? "")
                : linkInfo.LinkedHomeAccount.hoten;
            personHubNote = "Đã liên kết với tài khoản Home " + linkedName + " • " + (linkInfo.LinkedHomeAccount.taikhoan ?? "");
            return;
        }

        if (!string.IsNullOrWhiteSpace(linkInfo.PendingPhone))
        {
            personHubNote = "Đang chờ số " + linkInfo.PendingPhone + " đăng ký hoặc đăng nhập AhaSale để tự gắn.";
            return;
        }

        personHubNote = "Mở hồ sơ người để liên kết hoặc tạo chờ liên kết theo số điện thoại này.";
    }

    private string BuildRelatedRolesHtml(string normalizedPhone)
    {
        IList<GianHangAdminPersonHub_cl.PersonSourceRef> sources = GianHangAdminPersonHub_cl.GetOtherSourcesForPhone(db, user_parent, normalizedPhone, "customer", id);
        if (sources == null || sources.Count == 0)
            return "";

        return string.Join("", sources.Select(p =>
            "<div class='mt-2'>" +
            "<a class='fg-cobalt' href='" + HttpUtility.HtmlAttributeEncode(p.DetailUrl ?? "#") + "'>" + HttpUtility.HtmlEncode((p.SourceLabel ?? "").Trim() == "" ? "Hồ sơ liên quan" : p.SourceLabel) + "</a>" +
            "<span class='fg-gray'> • " + HttpUtility.HtmlEncode((p.Name ?? "").Trim() == "" ? (p.Phone ?? "") : p.Name) + "</span>" +
            "<div class='fg-gray'><small>Vai trò: <strong>" + HttpUtility.HtmlEncode(p.RoleLabel ?? "") + "</strong></small></div>" +
            "<div class='fg-gray'><small>Trạng thái nguồn: <span class='data-wrapper'><code class='" + HttpUtility.HtmlAttributeEncode(string.IsNullOrWhiteSpace(p.SourceLifecycleCss) ? "bg-green fg-white" : p.SourceLifecycleCss) + "'>" + HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(p.SourceLifecycleLabel) ? "Đang dùng ở nguồn" : p.SourceLifecycleLabel) + "</code></span></small></div>" +
            "<div class='fg-gray'><small>Quyền /gianhang/admin: <span class='data-wrapper'><code class='" + HttpUtility.HtmlAttributeEncode(string.IsNullOrWhiteSpace(p.AdminAccessCss) ? "bg-gray fg-white" : p.AdminAccessCss) + "'>" + HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(p.AdminAccessLabel) ? "Không mở quyền /gianhang/admin ở nguồn này" : p.AdminAccessLabel) + "</code></span></small></div>" +
            "</div>"));
    }

    private void BindSourceAdminAccess(string normalizedPhone)
    {
        GianHangAdminPersonHub_cl.PersonSourceRef sourceInfo = GianHangAdminPersonHub_cl.GetSourceInfo(db, user_parent, normalizedPhone, "customer", id);
        if (sourceInfo == null)
            return;

        personHubAdminAccessLabel = string.IsNullOrWhiteSpace(sourceInfo.AdminAccessLabel)
            ? "Không mở quyền /gianhang/admin ở vai trò khách hàng"
            : sourceInfo.AdminAccessLabel;
        personHubAdminAccessCss = string.IsNullOrWhiteSpace(sourceInfo.AdminAccessCss)
            ? "bg-gray fg-white"
            : sourceInfo.AdminAccessCss;
        personHubAdminAccessNote = "Vai trò khách hàng chỉ là dữ liệu nghiệp vụ. Nếu cùng người này còn là nhân sự nội bộ thì quyền vào /gianhang/admin sẽ được quyết định ở hồ sơ nhân sự nội bộ đó.";
    }

    private string BuildImpactNote(string normalizedPhone)
    {
        bool hasOtherRoles = !string.IsNullOrWhiteSpace(BuildRelatedRolesHtml(normalizedPhone));
        return hasOtherRoles
            ? "Xóa hồ sơ khách hàng này sẽ không tự gỡ liên kết Home ở Hồ sơ người. Các vai trò khác cùng số điện thoại trong gian hàng vẫn tiếp tục được gom chung và giữ trạng thái liên kết hiện có."
            : "Xóa hồ sơ khách hàng này sẽ không tự gỡ liên kết Home ở Hồ sơ người. Nếu đây là vai trò nguồn cuối cùng thì hồ sơ trung tâm vẫn được giữ, chỉ chuyển sang trạng thái chưa còn vai trò nguồn.";
    }

    private void BindCustomerLifecycle(bspa_data_khachhang_table customer)
    {
        if (customer == null)
            return;

        GianHangAdminSourceLifecycle_cl.SourceLifecycleInfo info = GianHangAdminSourceLifecycle_cl.GetInfo(
            db,
            user_parent,
            "customer",
            customer.id + "",
            "Đang dùng khách hàng",
            "Đã ngừng dùng khách hàng",
            "Hồ sơ khách hàng này đang được dùng bình thường trong module nguồn.",
            "Hồ sơ khách hàng này đang ở trạng thái ngừng dùng an toàn. Lịch sử nghiệp vụ và liên kết Home trung tâm vẫn được giữ.");

        sourceLifecycleLabel = info == null || string.IsNullOrWhiteSpace(info.Label) ? "Đang dùng khách hàng" : info.Label;
        sourceLifecycleCss = info == null || string.IsNullOrWhiteSpace(info.Css) ? "bg-green fg-white" : info.Css;
        sourceLifecycleNote = info == null || string.IsNullOrWhiteSpace(info.Note)
            ? "Hồ sơ khách hàng này đang được dùng bình thường trong module nguồn."
            : info.Note;
        sourceLifecycleIsInactive = info != null && info.IsInactive;
        personHubImpactTitle = sourceLifecycleIsInactive ? "Tác động khi xóa hồ sơ khách hàng đã ngừng dùng" : "Tác động khi xóa vai trò khách hàng";
    }

    private void nap_ngu_canh_datlich()
    {
        id_datlich_lienket = (Request.QueryString["id_datlich"] ?? "").Trim();
        if (id_datlich_lienket == "")
            return;

        var q_lich = db.bspa_datlich_tables.Where(p => p.id.ToString() == id_datlich_lienket && p.id_chinhanh == Session["chinhanh"].ToString());
        if (q_lich.Count() == 0)
            return;

        bspa_datlich_table _lich = q_lich.First();
        if ((_lich.sdt ?? "") != sdt)
            return;

        co_ngu_canh_datlich = true;
        ten_dichvu_datlich = _lich.tendichvu_taithoidiemnay;
        if (ten_dichvu_datlich == "")
            ten_dichvu_datlich = datlich_class.return_ten_dichvu(db, _lich.dichvu, Session["chinhanh"].ToString());

        ngay_datlich_lienket = _lich.ngaydat.HasValue ? _lich.ngaydat.Value.ToString("dd/MM/yyyy HH:mm") : "";
        ten_nhanvien_datlich = datlich_class.return_ten_nguoitao_hienthi(_lich.nhanvien_thuchien);
        url_quay_lai_datlich = "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=" + HttpUtility.UrlEncode(id_datlich_lienket);

        if (!string.IsNullOrWhiteSpace(_lich.dichvu))
        {
            DateTime _today = DateTime.Now.Date;
            list_id_thedv_goiy = db.thedichvu_tables
                .Where(p =>
                    p.sdt == sdt &&
                    p.id_chinhanh == Session["chinhanh"].ToString() &&
                    p.iddv == _lich.dichvu &&
                    (p.sl_conlai ?? 0) > 0 &&
                    (p.hsd.HasValue == false || p.hsd.Value.Date >= _today)
                )
                .OrderBy(p => p.hsd)
                .ThenByDescending(p => p.ngaytao)
                .Select(p => p.id.ToString())
                .ToList();
        }

        if (list_id_thedv_goiy.Count == 1)
            id_thedv_tu_dong_goiy = list_id_thedv_goiy[0];

        if (list_id_thedv_goiy.Count == 0)
            thongbao_datlich_thedv = "Khách chưa có thẻ còn hiệu lực phù hợp với dịch vụ của lịch hẹn này.";
        else if (list_id_thedv_goiy.Count == 1)
            thongbao_datlich_thedv = "Đã tự gợi ý 1 thẻ phù hợp để staff tiêu buổi ngay.";
        else
            thongbao_datlich_thedv = "Có " + list_id_thedv_goiy.Count.ToString("#,##0") + " thẻ phù hợp. Staff chọn 1 thẻ rồi tiêu buổi.";
    }

    private void ap_dung_ngu_canh_datlich_vao_form_thedv()
    {
        if (co_ngu_canh_datlich == false)
            return;

        var q_lich = db.bspa_datlich_tables.Where(p => p.id.ToString() == id_datlich_lienket && p.id_chinhanh == Session["chinhanh"].ToString());
        if (q_lich.Count() == 0)
            return;

        bspa_datlich_table _lich = q_lich.First();
        if (_lich.ngaydat.HasValue)
            txt_ngayban_thedv.Text = _lich.ngaydat.Value.ToString("dd/MM/yyyy");

        if (!string.IsNullOrWhiteSpace(_lich.nhanvien_thuchien))
            datlich_class.try_select_dropdown_value(ddl_nhanvien_lamdichvu_thedv, _lich.nhanvien_thuchien);
    }

    public string return_thedv_checked_attr(string _id)
    {
        if (id_thedv_tu_dong_goiy == _id)
            return "checked='checked'";
        return "";
    }

    public string return_thedv_row_class(string _id)
    {
        if (list_id_thedv_goiy.Contains(_id))
            return "bg-lightGreen";
        return "";
    }

    public bool check_thedv_goiy(string _id)
    {
        return list_id_thedv_goiy.Contains(_id);
    }

    public int return_so_hoadon_lienket_tu_ghichu(string _ghichu)
    {
        return datlich_lienket_class.lay_ds_id_hoadon_tu_ghichu(_ghichu).Count;
    }

    public int return_so_thedv_lienket_tu_ghichu(string _ghichu)
    {
        return datlich_lienket_class.lay_ds_id_thedv_tu_ghichu(_ghichu).Count;
    }

    public string return_url_hoadon_lienket_tu_ghichu(string _ghichu)
    {
        List<string> _list_id = datlich_lienket_class.lay_ds_id_hoadon_tu_ghichu(_ghichu);
        if (_list_id.Count == 0)
            return "";

        string _id_hoadon = _list_id[0];
        return "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + HttpUtility.UrlEncode(_id_hoadon);
    }

    public string return_url_tieu_buoi_tu_lich(string _id_datlich)
    {
        if (string.IsNullOrWhiteSpace(_id_datlich))
            return "";

        return "/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=" + HttpUtility.UrlEncode(id) + "&act=thedv&id_datlich=" + HttpUtility.UrlEncode(_id_datlich);
    }

    private void dongbo_lai_lich_hen_sau_khi_tieu_buoi(string _id_hoadon, string _id_thedv, string _user_lamdichvu, DateTime _thoigian_thuchien)
    {
        if (co_ngu_canh_datlich == false || id_datlich_lienket == "")
            return;

        datlich_lienket_class.dong_bo_vao_lich_hen(
            db,
            id_datlich_lienket,
            Session["chinhanh"].ToString(),
            _id_hoadon,
            _id_thedv,
            _user_lamdichvu,
            _thoigian_thuchien,
            "Đã dùng thẻ DV #" + _id_thedv + " / HĐ #" + _id_hoadon,
            true);
    }
    public void show_thedichvu()
    {
        var q = (from ob1 in db.thedichvu_tables.Where(p => p.sdt == sdt && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                     //join ob2 in db.web_menu_tables.ToList() on bv.id_category equals mn.id.ToString()
                 select new
                 {
                     id = ob1.id,
                     tenkhachhang = ob1.tenkh,
                     sdt = ob1.sdt,
                     ngaytao = ob1.ngaytao,
                     tenthe = ob1.tenthe,
                     tendv = ob1.ten_taithoidiemnay,
                     hsd = ob1.hsd,
                     sobuoi = ob1.tongsoluong,
                     sl_dalam = ob1.sl_dalam,
                     sl_conlai = ob1.sl_conlai,
                     tongtien = ob1.tongtien,
                     ck_hoadon = ob1.chietkhau,
                     tongtien_ck = ob1.tongtien_ck_hoadon,
                     tongsauchietkhau = ob1.tongsauchietkhau,
                     sotien_dathanhtoan = ob1.sotien_dathanhtoan,
                     sotien_conlai = ob1.sotien_conlai,
                     phantramchot = ob1.phantram_chotsale,
                     tongtien_chot = ob1.tongtien_chotsale_dvsp,
                     tennguoichot = tk_cl.exist_user_of_userparent(ob1.nguoichotsale, user_parent) == false ? ob1.nguoichotsale : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nguoichotsale + "'>" + tk_cl.return_object(ob1.nguoichotsale).hoten + "</a></div>",
                 }).ToList();
        q = q.OrderByDescending(p => p.ngaytao.Value).ToList();
        var list_split_thedv = q.ToList();
        list_id_split_thedv = new List<string>();
        foreach (var t in list_split_thedv)
        {
            list_id_split_thedv.Add("check_thedv_" + t.id);
        }
        if (q.Count() != 0)
        {
            sl_thedv = q.Count();
            doanhso_tdv = q.Sum(p => p.tongtien).Value;
            doanhso_tdv_sauck = q.Sum(p => p.tongsauchietkhau).Value;
            tongtien_dathanhtoan_tdv = q.Sum(p => p.sotien_dathanhtoan).Value;
            tong_congno_tdv = q.Sum(p => p.sotien_conlai).Value;


            Repeater7.DataSource = q;
            Repeater7.DataBind();
        }
    }

    public void show_lichhen()
    {
        var q = from ob1 in db.bspa_datlich_tables.Where(p => p.sdt == sdt && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                select new
                {
                    id = ob1.id,
                    ngaydat = ob1.ngaydat,
                    tenkhachhang = ob1.tenkhachhang,
                    sdt = ob1.sdt,
                    email = email,
                    nguoitao = ob1.nguoitao,
                    dichvu = ob1.dichvu,
                    tendv = ob1.tendichvu_taithoidiemnay,
                    nhanvien_thuchien = tk_cl.exist_user_of_userparent(ob1.nhanvien_thuchien, user_parent) == false ? ob1.nhanvien_thuchien : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nhanvien_thuchien + "'>" + tk_cl.return_object(ob1.nhanvien_thuchien).hoten + "</a></div>",
                    ghichu = ob1.ghichu,
                    trangthai = ob1.trangthai,
                    ngaytao = ob1.ngaytao,
                };
        Repeater3.DataSource = q.OrderByDescending(p => p.ngaydat);
        Repeater3.DataBind();
    }

    public void show_ghichu()
    {
        var list_ghichu_hoadon = (from ob1 in db.bspa_hoadon_tables.Where(p => p.sdt == sdt && p.ghichu != "" && p.id_chinhanh == Session["chinhanh"].ToString())
                                      //join ob2 in db.bspa_hoadon_chitiet_tables.ToList() on ob1.id.ToString() equals ob2.id_hoadon
                                  select new
                                  {
                                      id = ob1.id,
                                      ghichu = ob1.ghichu,
                                      ngaytao = ob1.ngaytao,
                                      nguoitao = ob1.nguoitao,
                                      kyhieu = "hoadon",
                                      id_hoadon = ob1.id.ToString(),
                                  });
        var list_ghichu_db = (from ob1 in db.ghichu_khachhang_tables.Where(p => p.sdt == sdt && p.id_chinhanh == Session["chinhanh"].ToString())
                                  //join ob2 in db.bspa_hoadon_chitiet_tables.ToList() on ob1.id.ToString() equals ob2.id_hoadon
                              select new
                              {
                                  id = ob1.id,
                                  ghichu = ob1.ghichu,
                                  ngaytao = ob1.ngaytao,
                                  nguoitao = ob1.nguoitao,
                                  kyhieu = "table",
                                  id_hoadon = "",
                              });
        var q = list_ghichu_hoadon.Union(list_ghichu_db);
        q = q.OrderByDescending(p => p.ngaytao);

        list_id_split_ghichu = new List<string>();
        foreach (var t in q)
        {
            list_id_split_ghichu.Add("ghichu_" + t.id);
        }

        Repeater4.DataSource = q;
        Repeater4.DataBind();

    }

    public void show_donthuoc()
    {
        var q = (from ob1 in db.donthuoc_khachhang_tables.Where(p => p.sdt == sdt && p.id_chinhanh == Session["chinhanh"].ToString())
                     //join ob2 in db.bspa_hoadon_chitiet_tables.ToList() on ob1.id.ToString() equals ob2.id_hoadon
                 select new
                 {
                     id = ob1.id,
                     ghichu = ob1.ghichu,
                     ngaytao = ob1.ngaytao,
                     nguoitao = ob1.nguoitao,
                     noitaikham = ob1.noitaikham,
                     loidan = ob1.loidanbacsi,
                     ngaytaikham = ob1.ngaytaikham,
                 });
        q = q.OrderByDescending(p => p.ngaytao);

        list_id_split_donthuoc = new List<string>();
        foreach (var t in q)
        {
            list_id_split_donthuoc.Add("donthuoc_" + t.id);
        }

        Repeater5.DataSource = q;
        Repeater5.DataBind();

    }
    public void show_hinhanh()
    {
        var q = (from ob1 in db.hinhanh_truocsau_khachhang_tables.Where(p => p.sdt == sdt && p.id_chinhanh == Session["chinhanh"].ToString())
                     //join ob2 in db.bspa_hoadon_chitiet_tables.ToList() on ob1.id.ToString() equals ob2.id_hoadon
                 select new
                 {
                     id = ob1.id,
                     ghichu = ob1.ghichu,
                     ngaytao = ob1.ngaytao,
                     nguoitao = ob1.nguoitao,
                     anhtruoc = ob1.hinhanh_truoc == "" ? "" : "<img src='" + ob1.hinhanh_truoc + "' class='img-cover-vuong w-h-100' />",
                     anhsau = ob1.hinhanh_sau == "" ? "" : "<img src='" + ob1.hinhanh_sau + "' class='img-cover-vuong w-h-100' />",
                 });
        q = q.OrderByDescending(p => p.ngaytao);

        list_id_split_hinhanh = new List<string>();
        foreach (var t in q)
        {
            list_id_split_hinhanh.Add("hinhanh_" + t.id);
        }

        Repeater6.DataSource = q;
        Repeater6.DataBind();

    }
    public void show_hoadon()
    {
        var q = db.bspa_hoadon_tables.Where(p => p.sdt == sdt && p.id_chinhanh == Session["chinhanh"].ToString());
        if (q.Count() != 0)
        {
            tong_hoadon = q.Count();
            tong_dv = q.Sum(p => p.sl_dichvu).Value;
            tong_sp = q.Sum(p => p.sl_sanpham).Value;
            tong_chitieu = q.Sum(p => p.tongsauchietkhau).Value;
            tong_congno = q.Sum(p => p.sotien_conlai).Value;

            q = q.OrderByDescending(p => p.ngaytao);

            Repeater1.DataSource = q;
            Repeater1.DataBind();
        }
    }

    public List<bspa_hoadon_chitiet_table> show_chitiet_hoadon(string _idhd)
    {
        var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id_hoadon == _idhd && p.id_chinhanh == Session["chinhanh"].ToString()).OrderByDescending(p => p.ngaytao).ToList();
        if (q.Count() != 0)
            return q;
        return null;
    }

    public List<bspa_hoadon_chitiet_table> show_chitiet_thedv(string _id_tdv)
    {
        var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id_thedichvu == _id_tdv && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.ngaytao).ToList();
        if (q.Count() != 0)
            return q;
        return null;
    }

    public string return_ten_nhanvien(string _user)
    {
        return datlich_class.return_ten_nguoitao_hienthi(_user);
    }
    public void main()
    {
        bspa_data_khachhang_table _ob = db.bspa_data_khachhang_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        hoten = _ob.tenkhachhang;
        sdt = _ob.sdt;

        // BỔ SUNG EMAIL
        email = _ob.email == null ? "" : _ob.email;
        // /BỔ SUNG EMAIL

        if (!IsPostBack)
        {

            txt_hoten.Text = hoten;
            txt_dienthoai.Text = sdt;

            // BỔ SUNG EMAIL
            txt_email.Text = email;
            // /BỔ SUNG EMAIL

            txt_ngaysinh.Text = _ob.ngaysinh != null ? _ob.ngaysinh.Value.ToString("dd/MM/yyyy") : "";
            txt_diachi.Text = _ob.diachi;
            txt_magioithieu.Text = _ob.magioithieu;

            var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                 select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                            );
            ddl_nhanvien_chamsoc.DataSource = list_nhanvien;
            ddl_nhanvien_chamsoc.DataTextField = "tennhanvien";
            ddl_nhanvien_chamsoc.DataValueField = "username";
            ddl_nhanvien_chamsoc.DataBind();
            ddl_nhanvien_chamsoc.Items.Insert(0, new ListItem("Chọn nhân viên", ""));
            if (_ob.nguoichamsoc != "")
                ddl_nhanvien_chamsoc.SelectedIndex = tk_cl.return_index(_ob.nguoichamsoc);

            var list_nohmkhachhang = (from ob1 in db.nhomkhachhang_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                      select new { id = ob1.id, tennhom = ob1.tennhom, }
                      );
            DropDownList1.DataSource = list_nohmkhachhang;
            DropDownList1.DataTextField = "tennhom";
            DropDownList1.DataValueField = "id";
            DropDownList1.DataBind();
            DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));
            if (_ob.nhomkhachhang != "")
                DropDownList1.SelectedIndex = dt_kh_cl.return_index(_ob.nhomkhachhang);


            //ngaytao = _ob.ngaytao.Value.ToString("dd/MM/yyyy");
            //nguoitao = _ob.nguoitao;


        }
        if (_ob.anhdaidien != "")
        {
            Button2.Visible = true;
            Label1.Text = "<img src='" + _ob.anhdaidien + "' class='img-cover-vuongtron' width='100' height='100' />";
        }
        else
        {
            Label1.Text = "<img src='/uploads/images/macdinh.jpg' class='img-cover-vuongtron' width='100' height='100' />";
            Button2.Visible = false;
        }

    }




    protected void button1_Click(object sender, EventArgs e)
    {
        if (!HasAnyPermission("q8_3"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để cập nhật khách hàng.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        bspa_data_khachhang_table _ob = db.bspa_data_khachhang_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        string _sdt_old = _ob.sdt;
        string _tenkhachhang = str_cl.VietHoa_ChuCai_DauTien(txt_hoten.Text.Trim().ToLower());
        string _sdt = str_cl.remove_blank(txt_dienthoai.Text.Trim()).Replace(" ", "").Replace(".", "").Replace("-", "").Replace("+", "");

        // BỔ SUNG EMAIL
        string _email = txt_email.Text.Trim();
        // /BỔ SUNG EMAIL

        string _diachi = txt_diachi.Text;
        if (_tenkhachhang == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tên khách hàng.", "false", "false", "OK", "alert", ""), true);
        else
        {
            if (_sdt == "")
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập số điện thoại khách hàng.", "false", "false", "OK", "alert", ""), true);
            else
            {
                if (bcorn_class.exist_sdt_old_data_kh(_sdt_old, _sdt) && _sdt != _sdt_old)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Email này đã được đăng ký cho một tài khoản khác.", "false", "false", "OK", "alert", ""), true);
                else
                {
                    var q_data = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt && p.id_chinhanh == Session["chinhanh"].ToString());

                    bspa_data_khachhang_table _ob1 = q_data.First();

                    bool _checkloi = false;
                    string _avt = "";
                    _avt = _ob1.anhdaidien;

                    if (FileUpload2.HasFile)//nếu có ảnh thu nhỏ đc chọn
                    {
                        string _ext = Path.GetExtension(FileUpload2.FileName).ToLower();
                        if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                        {
                            //byte - kb - mb  ContentLength trra về byte của file
                            long _filesize = (FileUpload2.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                            if (_filesize <= 1) //>1MB
                            {
                                if (_ob1.anhdaidien != "/uploads/images/macdinh.jpg")
                                    file_folder_class.del_file(_ob1.anhdaidien);//xóa ảnh cũ
                                _avt = "/uploads/images/khach-hang/" + Guid.NewGuid() + _ext;
                                FileUpload2.SaveAs(Server.MapPath("~" + _avt));//lưu ảnh mới
                            }
                            else
                            {
                                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước ảnh đại diện quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                                _checkloi = true;
                            }
                        }
                        else
                        {
                            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Ảnh đại diện không đúng định dạng.", "false", "false", "OK", "alert", "");
                            _checkloi = true;
                        }
                    }
                    _ob1.anhdaidien = _avt;
                    _ob1.tenkhachhang = _tenkhachhang;
                    _ob1.diachi = _diachi;
                    _ob1.magioithieu = txt_magioithieu.Text.Trim();

                    // BỔ SUNG EMAIL
                    _ob1.email = _email;
                    // /BỔ SUNG EMAIL

                    //_ob1.ngaytao = DateTime.Now;
                    //_ob1.nguoitao = user;
                    //_ob1.user_parent = user_parent;
                    _ob1.nguoichamsoc = ddl_nhanvien_chamsoc.SelectedValue.ToString();
                    _ob1.sdt = _sdt;
                    _ob1.nhomkhachhang = DropDownList1.SelectedValue.ToString();


                    if (txt_ngaysinh.Text != "" && dt_cl.check_date(txt_ngaysinh.Text) == true)
                        _ob1.ngaysinh = DateTime.Parse(txt_ngaysinh.Text);

                    if (_checkloi == false)
                    {
                        db.SubmitChanges();
                        GianHangAdminPersonHub_cl.SyncSourcePhoneState(db, user_parent, _sdt_old, _sdt, _tenkhachhang, user);
                        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                        Response.Redirect("/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=" + id);
                    }
                }
            }
        }
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        if (!HasAnyPermission("q8_3"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
        }
        else
        {
            bspa_data_khachhang_table _ob = db.bspa_data_khachhang_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
            if (_ob.anhdaidien != "/uploads/images/macdinh.jpg")
            {
                file_folder_class.del_file(_ob.anhdaidien);//xóa ảnh cũ
                _ob.anhdaidien = "/uploads/images/macdinh.jpg";
            }
            db.SubmitChanges();
            main();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa ảnh đại diện thành công.", "4000", "warning"), true);
        }
    }

    protected void but_ngung_khachhang_Click(object sender, EventArgs e)
    {
        if (!HasAnyPermission("q8_3"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Bạn không đủ quyền để đổi trạng thái khách hàng.", "4000", "warning"), true);
            return;
        }

        GianHangAdminSourceLifecycle_cl.SetInactive(db, user_parent, "customer", id, user, "Khách hàng được chuyển sang trạng thái ngừng dùng an toàn từ trang chi tiết.");
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Đã chuyển khách hàng sang trạng thái ngừng dùng an toàn.", "3200", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=" + id);
    }

    protected void but_molai_khachhang_Click(object sender, EventArgs e)
    {
        if (!HasAnyPermission("q8_3"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Bạn không đủ quyền để đổi trạng thái khách hàng.", "4000", "warning"), true);
            return;
        }

        GianHangAdminSourceLifecycle_cl.SetActive(db, user_parent, "customer", id, user, "Khách hàng được mở lại từ trang chi tiết.");
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Đã mở lại trạng thái dùng của khách hàng.", "3200", "success");
        Response.Redirect("/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=" + id);
    }

    protected void but_xoa_ghichu_Click(object sender, ImageClickEventArgs e)
    {
        if (HasAnyPermission("q8_3"))
        {
            int _count = 0;
            for (int i = 0; i < list_id_split_ghichu.Count; i++)
            {
                if (Request.Form[list_id_split_ghichu[i]] == "on")
                {
                    string _id = list_id_split_ghichu[i].Replace("ghichu_", "");
                    var q = db.ghichu_khachhang_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (q.Count() != 0)
                    {
                        ghichu_khachhang_table _ob = q.First();
                        db.ghichu_khachhang_tables.DeleteOnSubmit(_ob);
                        db.SubmitChanges();
                        _count = _count + 1;
                    }
                }
            }
            if (_count > 0)
            {
                main();
                show_ghichu();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "warning"), true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
            }

        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        if (!HasAnyPermission("q8_3"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để tạo ghi chú.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        string _ghichu = txt_noidung_ghichu.Text.Trim();
        if (_ghichu == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập nội dung ghi chú.", "false", "false", "OK", "alert", ""), true);
        else
        {
            ghichu_khachhang_table _ob = new ghichu_khachhang_table();
            _ob.ngaytao = DateTime.Now;
            _ob.nguoitao = user;
            _ob.ghichu = _ghichu;
            _ob.sdt = sdt;

            _ob.id_chinhanh = Session["chinhanh"].ToString();
            db.ghichu_khachhang_tables.InsertOnSubmit(_ob);
            db.SubmitChanges();
            txt_noidung_ghichu.Text = "";
            main();
            show_ghichu();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Tạo ghi chú thành công.", "4000", "warning"), true);
        }
    }

    protected void Button4_Click(object sender, EventArgs e)
    {
        if (!HasAnyPermission("q8_3"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để tạo đơn thuốc.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        string _ghichu = txt_ghichu_donthuoc.Text.Trim();
        if (_ghichu == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập nội dung đơn thuốc.", "false", "false", "OK", "alert", ""), true);
        else
        {
            donthuoc_khachhang_table _ob = new donthuoc_khachhang_table();
            _ob.ngaytao = DateTime.Now;
            _ob.nguoitao = user;
            _ob.ghichu = _ghichu;
            _ob.sdt = sdt;
            _ob.noitaikham = txt_noitamkham.Text.Trim();
            _ob.loidanbacsi = txt_loidanbacsi.Text.Trim();
            if (txt_ngaytaikham.Text != "" && dt_cl.check_date(txt_ngaytaikham.Text) == true)
                _ob.ngaytaikham = DateTime.Parse(txt_ngaytaikham.Text);

            _ob.id_chinhanh = Session["chinhanh"].ToString();
            db.donthuoc_khachhang_tables.InsertOnSubmit(_ob);
            db.SubmitChanges();
            txt_ghichu_donthuoc.Text = ""; txt_ngaytaikham.Text = ""; txt_noitamkham.Text = ""; txt_loidanbacsi.Text = "";
            main();
            show_donthuoc();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Tạo đơn thuốc thành công.", "4000", "warning"), true);
        }
    }

    protected void but_xoa_donthuoc_Click(object sender, ImageClickEventArgs e)
    {
        if (HasAnyPermission("q8_3"))
        {
            int _count = 0;
            for (int i = 0; i < list_id_split_donthuoc.Count; i++)
            {
                if (Request.Form[list_id_split_donthuoc[i]] == "on")
                {
                    string _id = list_id_split_donthuoc[i].Replace("donthuoc_", "");
                    var q = db.donthuoc_khachhang_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (q.Count() != 0)
                    {
                        donthuoc_khachhang_table _ob = q.First();
                        db.donthuoc_khachhang_tables.DeleteOnSubmit(_ob);
                        db.SubmitChanges();
                        _count = _count + 1;
                    }
                }
            }
            if (_count > 0)
            {
                main();
                show_donthuoc();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "warning"), true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
            }

        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }




    protected void them_hinhanh_truocsau_Click(object sender, EventArgs e)
    {
        if (!HasAnyPermission("q8_3"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để thêm ảnh trước sau.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        string _ghichu = txt_ghichu_hinhanh_truocsau.Text.Trim();
        hinhanh_truocsau_khachhang_table _ob = new hinhanh_truocsau_khachhang_table();
        _ob.ngaytao = DateTime.Now;
        _ob.nguoitao = user;
        _ob.ghichu = _ghichu;
        _ob.sdt = sdt;
        _ob.hinhanh_truoc = "";
        _ob.hinhanh_sau = "";
        if (FileUpload1.HasFile)//ảnh trước
        {
            string _ext = Path.GetExtension(FileUpload1.FileName).ToLower();
            if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
            {
                string _anhtruoc = "/uploads/images/truoc-sau/" + Guid.NewGuid() + _ext;
                FileUpload1.SaveAs(Server.MapPath("~" + _anhtruoc));//lưu ảnh mới
                _ob.hinhanh_truoc = _anhtruoc;
            }
        }
        if (FileUpload3.HasFile)//ảnh sau
        {
            string _ext = Path.GetExtension(FileUpload3.FileName).ToLower();
            if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
            {
                string _anhsau = "/uploads/images/truoc-sau/" + Guid.NewGuid() + _ext;
                FileUpload3.SaveAs(Server.MapPath("~" + _anhsau));//lưu ảnh mới
                _ob.hinhanh_sau = _anhsau;
            }
        }

        _ob.id_chinhanh = Session["chinhanh"].ToString();
        db.hinhanh_truocsau_khachhang_tables.InsertOnSubmit(_ob);
        db.SubmitChanges();
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thêm ảnh trước sau thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=" + id + "&act=hinhanh");
    }

    protected void but_xoa_hinhanh_truocsau_Click(object sender, ImageClickEventArgs e)
    {
        if (HasAnyPermission("q8_3"))
        {
            int _count = 0;
            for (int i = 0; i < list_id_split_hinhanh.Count; i++)
            {
                if (Request.Form[list_id_split_hinhanh[i]] == "on")
                {
                    string _id = list_id_split_hinhanh[i].Replace("hinhanh_", "");
                    var q = db.hinhanh_truocsau_khachhang_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (q.Count() != 0)
                    {
                        hinhanh_truocsau_khachhang_table _ob = q.First();
                        file_folder_class.del_file(_ob.hinhanh_truoc);//xóa ảnh cũ
                        file_folder_class.del_file(_ob.hinhanh_sau);//xóa ảnh cũ
                        db.hinhanh_truocsau_khachhang_tables.DeleteOnSubmit(_ob);
                        db.SubmitChanges();
                        _count = _count + 1;
                    }
                }
            }
            if (_count > 0)
            {
                main();
                show_hinhanh();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "warning"), true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
            }

        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }

    protected void Button5_Click(object sender, EventArgs e)
    {
        string _id_thedv = "";
        int _count = 0;
        if (HasAnyPermission("q7_2"))
        {
            //đảm bảo rằng chỉ chọn 1 lần 1 thẻ dv
            for (int i = 0; i < list_id_split_thedv.Count; i++)
            {
                if (Request.Form[list_id_split_thedv[i]] == "on")
                {
                    string _id = list_id_split_thedv[i].Replace("check_thedv_", "");
                    var q = db.thedichvu_tables.Where(p => p.id.ToString() == _id && p.sdt == sdt && p.hsd.Value.Date >= DateTime.Now.Date && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (tdv_cl.exist_id(_id, user_parent))
                    {
                        _id_thedv = _id;
                        _count = _count + 1;
                    }
                }
            }

            if (_count > 1 || _count == 0)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn 1 thẻ dịch vụ.", "false", "false", "OK", "alert", ""), true);
            else
            {
                var q_thedv = db.thedichvu_tables.Where(p => p.id.ToString() == _id_thedv && p.id_chinhanh == Session["chinhanh"].ToString());
                if (q_thedv.First().sl_conlai <= 0)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Thẻ này đã sử dụng hết số buổi.", "false", "false", "OK", "alert", ""), true);
                else
                {
                    string _ten_dichvu = tdv_cl.return_object(_id_thedv).ten_taithoidiemnay;
                    string _id_dichvu = tdv_cl.return_object(_id_thedv).iddv;
                    Int64 _gia_1buoi = tdv_cl.return_object(_id_thedv).tongsauchietkhau.Value / tdv_cl.return_object(_id_thedv).tongsoluong.Value;

                    string _user_lamdichvu = ddl_nhanvien_lamdichvu_thedv.SelectedValue.ToString();

                    int _r1 = 0;//giá

                    int _r3 = 1;//sl

                    DateTime _ngaytao = DateTime.Parse(txt_ngayban_thedv.Text + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());

                    string _phantram_lam = txt_chietkhau_lamdichvu_thedv.Text.Trim().Replace(".", "");
                    int _r5 = 0;
                    int.TryParse(_phantram_lam, out _r5);

                    //var q_chitiet_hoadon = db.bspa_hoadon_chitiet_tables.Where(p => p.user_parent == user_parent && p.id_dvsp == _id_dichvu && p.id_hoadon == id);

                    if (po_cl.exist_id(_id_dichvu))
                    {
                        if (_r1 < 0)//giá k đc = 0            
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Giá dịch vụ không hợp lệ.", "4000", "warning"), true);
                        else
                        {
                            if (_r3 < 1)//số lượng     
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Số lượng không hợp lệ.", "4000", "warning"), true);
                            else
                            {

                                if ((_r5 < 0 && ck_dv_phantram_lamdv_thedv.Checked == true) || (_r5 > 100 && ck_dv_phantram_lamdv_thedv.Checked == true))//nếu chọn % thì k đc <0 & >100
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chiết khấu cho nhân viên thực hiện không hợp lệ.", "4000", "warning"), true);
                                else
                                {

                                    #region THEM HOA DON
                                    bspa_hoadon_table _ob_hd = new bspa_hoadon_table();
                                    _ob_hd.id_guide = Guid.NewGuid();
                                    _ob_hd.ngaytao = _ngaytao;
                                    _ob_hd.tongtien = 0;
                                    _ob_hd.chietkhau = 0;
                                    _ob_hd.tongsauchietkhau = 0;
                                    _ob_hd.tenkhachhang = hoten;
                                    _ob_hd.ghichu = "";
                                    _ob_hd.sdt = sdt;
                                    _ob_hd.diachi = "";
                                    _ob_hd.sotien_dathanhtoan = 0;
                                    _ob_hd.thanhtoan_tienmat = 0;
                                    _ob_hd.thanhtoan_chuyenkhoan = 0;
                                    _ob_hd.thanhtoan_quetthe = 0;
                                    _ob_hd.sotien_conlai = 0;
                                    _ob_hd.sl_dichvu = 1;
                                    _ob_hd.sl_sanpham = 0;
                                    _ob_hd.album = "";
                                    _ob_hd.user_parent = user_parent;
                                    _ob_hd.dichvu_hay_sanpham = "dichvu";
                                    _ob_hd.ds_dichvu = 0; _ob_hd.ds_sanpham = 0; _ob_hd.sauck_dichvu = 0; _ob_hd.sauck_sanpham = 0;
                                    _ob_hd.tongtien_ck_hoadon = 0;
                                    _ob_hd.km1_ghichu = "";
                                    _ob_hd.nguoitao = user;
                                    _ob_hd.nguongoc = "App";
                                    if (co_ngu_canh_datlich)
                                        _ob_hd.ghichu = datlich_lienket_class.dam_bao_ghi_chu_datlich(_ob_hd.ghichu, id_datlich_lienket, "Sử dụng thẻ từ", ten_dichvu_datlich);

                                    _ob_hd.id_chinhanh = Session["chinhanh"].ToString();
                                    db.bspa_hoadon_tables.InsertOnSubmit(_ob_hd);
                                    db.SubmitChanges();

                                    string _idhd = _ob_hd.id.ToString();
                                    #endregion

                                    bspa_hoadon_chitiet_table _ob = new bspa_hoadon_chitiet_table();
                                    _ob.id_hoadon = _idhd;
                                    _ob.id_thedichvu = _id_thedv;//đánh dấu dv này là sử dụng thẻ dv
                                    _ob.id_dvsp = _id_dichvu;
                                    _ob.ten_dvsp_taithoidiemnay = _ten_dichvu;
                                    _ob.gia_dvsp_taithoidiemnay = _r1;
                                    _ob.soluong = _r3;
                                    _ob.thanhtien = _r1 * _r3;
                                    _ob.hinhanh_hientai = "";
                                    _ob.chietkhau = 0;
                                    _ob.tongsauchietkhau = 0;
                                    _ob.tongtien_ck_dvsp = 0;
                                    _ob.ngaytao = _ngaytao;

                                    _ob.nguoichot_dvsp = "";
                                    _ob.tennguoichot_hientai = "";
                                    _ob.phantram_chotsale_dvsp = 0;
                                    _ob.tongtien_chotsale_dvsp = 0;

                                    _ob.nguoilam_dichvu = _user_lamdichvu;

                                    if (_user_lamdichvu == "")
                                    {
                                        _ob.phantram_lamdichvu = 0;
                                        _ob.tongtien_lamdichvu = 0;
                                        _ob.tennguoilam_hientai = "";
                                    }
                                    else
                                    {
                                        _ob.tennguoilam_hientai = tk_cl.return_object(_user_lamdichvu).hoten;
                                        if (ck_dv_phantram_lamdv_thedv.Checked == true)//nếu ck thực hiện là %
                                        {
                                            _ob.phantram_lamdichvu = _r5;
                                            _ob.tongtien_lamdichvu = _gia_1buoi * _r5 / 100;
                                        }
                                        else
                                        {
                                            _ob.phantram_lamdichvu = 0;
                                            _ob.tongtien_lamdichvu = _r5;
                                        }
                                    }


                                    _ob.kyhieu = "dichvu";
                                    _ob.user_parent = user_parent;
                                    _ob.nguoitao = user;
                                    _ob.danhgia_nhanvien_lamdichvu = txt_danhgia_dichvu_lamdv.Text.Trim();
                                    _ob.danhgia_5sao_dv = Request.Form["danhgia_5sao_nhanvien_dv_lamdv"];

                                    _ob.id_chinhanh = Session["chinhanh"].ToString();
                                    db.bspa_hoadon_chitiet_tables.InsertOnSubmit(_ob);
                                    db.SubmitChanges();


                                    //công trừ số buổi của thẻ

                                    thedichvu_table _ob_thedv = q_thedv.First();
                                    _ob_thedv.sl_dalam = _ob_thedv.sl_dalam + 1;
                                    _ob_thedv.sl_conlai = _ob_thedv.tongsoluong - _ob_thedv.sl_dalam;

                                    dongbo_lai_lich_hen_sau_khi_tieu_buoi(_idhd, _id_thedv, _user_lamdichvu, _ngaytao);
                                    db.SubmitChanges();

                                    Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Sử dụng thẻ dịch vụ thành công.", "4000", "warning");
                                    Response.Redirect("/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + _idhd + (co_ngu_canh_datlich ? "&id_datlich=" + HttpUtility.UrlEncode(id_datlich_lienket) : ""));
                                    //main();
                                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Sử dụng thẻ dịch vụ thành công.", "4000", "warning"), true);


                                }

                            }


                        }

                    }
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Dịch vụ không tồn tại.", "4000", "warning"), true);
                }
            }

        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }
}
