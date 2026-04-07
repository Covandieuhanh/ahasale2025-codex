using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class taikhoan_add : System.Web.UI.Page
{
    public string notifi, id, user, user_parent, sdt_kh;
    public string personHubUrl = "/gianhang/admin/quan-ly-con-nguoi/Default.aspx";
    public string personHubStatusLabel = "Chưa liên kết";
    public string personHubStatusCss = "bg-gray fg-white";
    public string personHubNote = "Mở hồ sơ người để liên kết hoặc tạo chờ liên kết theo số điện thoại này.";
    public string personHubRelatedRolesHtml = "";
    public string personHubAdminAccessLabel = "Không mở quyền /gianhang/admin ở vai trò thành viên / học viên";
    public string personHubAdminAccessCss = "bg-gray fg-white";
    public string personHubAdminAccessNote = "Vai trò thành viên / học viên chỉ là dữ liệu nghiệp vụ. Nếu cùng người này còn là nhân sự nội bộ thì quyền vào /gianhang/admin sẽ được quyết định ở hồ sơ nhân sự nội bộ đó.";
    public string personHubImpactTitle = "Tác động khi xóa vai trò thành viên / học viên";
    public string personHubImpactNote = "Xóa vai trò thành viên này sẽ không tự gỡ liên kết Home ở Hồ sơ người. Nếu cùng số điện thoại còn vai trò khác trong gian hàng thì hồ sơ trung tâm vẫn tiếp tục gom đúng người đó.";
    public string sourceLifecycleLabel = "Đang dùng thành viên";
    public string sourceLifecycleCss = "bg-green fg-white";
    public string sourceLifecycleNote = "Vai trò thành viên / học viên này đang được dùng bình thường trong module nguồn.";
    public bool sourceLifecycleIsInactive = false;
    dbDataContext db = new dbDataContext();
    string_class str_cl = new string_class();
    hocvien_class hv_cl = new hocvien_class();
    giangvien_class gv_cl = new giangvien_class();
    datetime_class dt_cl = new datetime_class();
    nganh_class nganh_cl = new nganh_class();
    data_khachhang_class dtkh_cl = new data_khachhang_class();
    public Int64 sotien_conlai = 0;

    private bool HasAnyPermission(params string[] permissionKeys)
    {
        if (permissionKeys == null)
            return false;

        foreach (string permissionKey in permissionKeys)
        {
            string key = (permissionKey ?? "").Trim();
            if (key != "" && bcorn_class.check_quyen(user, key) == "")
                return true;
        }

        return false;
    }

    public void main()
    {
        hocvien_table _ob = db.hocvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        BindPersonHub(_ob.hoten, _ob.dienthoai);
        BindMemberLifecycle(_ob);
        if (!IsPostBack)
        {
            txt_ngaythanhtoan.Text = DateTime.Now.ToShortDateString();

            if (HasAnyPermission("q14_2"))
            {
                var list_gv = (from ob1 in db.giangvien_tables.Where(p => p.trangthai == "Đang giảng dạy" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                               select new { id = ob1.id, ten = ob1.hoten, }
                                );
                DropDownList1.DataSource = list_gv;
                DropDownList1.DataTextField = "ten";
                DropDownList1.DataValueField = "id";
                DropDownList1.DataBind();
                DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));
                DropDownList1.SelectedIndex = gv_cl.return_index(_ob.id_giangvien);
            }
            else
            {
                if (_ob.nganhhoc != Session["nganh"].ToString())
                {
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để chỉnh sửa thành viên ngành khác.", "false", "false", "OK", "alert", "");
                    Response.Redirect("/gianhang/admin");
                }

                var list_gv = (from ob1 in db.giangvien_tables.Where(p => p.trangthai == "Đang giảng dạy" && p.chuyenmon == Session["nganh"].ToString() && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                   select new { id = ob1.id, ten = ob1.hoten, }
                                    );
                DropDownList1.DataSource = list_gv;
                DropDownList1.DataTextField = "ten";
                DropDownList1.DataValueField = "id";
                DropDownList1.DataBind();
                DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));
                DropDownList1.SelectedIndex = gv_cl.return_index_nganh(_ob.id_giangvien);
            }

            var list_nganh = (from ob1 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                              select new { id = ob1.id, ten = ob1.ten, }
                               );
            DropDownList5.DataSource = list_nganh;
            DropDownList5.DataTextField = "ten";
            DropDownList5.DataValueField = "id";
            DropDownList5.DataBind();
            DropDownList5.Items.Insert(0, new ListItem("Chọn", ""));
            DropDownList5.SelectedIndex = nganh_cl.return_index(_ob.nganhhoc);
            if (HasAnyPermission("q14_3"))
            {

            }
            else
            {
                DropDownList5.Enabled = false;
            }


            txt_hoten.Text = _ob.hoten;
            txt_ngaysinh.Text = _ob.ngaysinh != null ? _ob.ngaysinh.Value.ToString("dd/MM/yyyy") : "";

            txt_email.Text = _ob.email;
            txt_dienthoai.Text = _ob.dienthoai; sdt_kh = _ob.dienthoai;
            txt_zalo.Text = _ob.zalo;
            txt_facebook.Text = _ob.facebook;
            txt_sobuoi_lythuyet.Text = _ob.sobuoi_lythuyet.Value.ToString();
            txt_sobuoi_thuchanh.Text = _ob.sobuoi_thuchanh.Value.ToString();
            txt_sobuoi_trogiang.Text = _ob.sobuoi_trogiang.Value.ToString();
            if (_ob.goidaotao == "Cơ bản")
                DropDownList2.SelectedIndex = 0;
            else
            {
                if (_ob.goidaotao == "Nâng cao")
                    DropDownList2.SelectedIndex = 1;
                else
                    DropDownList2.SelectedIndex = 2;
            }
            if (_ob.capbang == "Chưa cấp bằng")
                DropDownList4.SelectedIndex = 0;
            else
                DropDownList4.SelectedIndex = 1;
            txt_ngaycapbang.Text = _ob.ngaycapbang != null ? _ob.ngaycapbang.Value.ToString("dd/MM/yyyy") : "";
            if (_ob.xeploai == "0")
                DropDownList3.SelectedIndex = 0;
            else
            {
                if (_ob.goidaotao == "A")
                    DropDownList2.SelectedIndex = 1;
                {
                    if (_ob.goidaotao == "B")
                        DropDownList2.SelectedIndex = 2;
                    else
                        DropDownList2.SelectedIndex = 3;
                }
            }
            //txt_nganhhoc.Text = _ob.nganhhoc;
            txt_hocphi.Text = _ob.hocphi.Value.ToString("#,##0");
        }
        if (_ob.anhdaidien != "")
        {
            Button2.Visible = true;
            Label2.Text = "<img src='" + _ob.anhdaidien + "' style='max-width: 100px' />";
        }
        else
        {
            Button2.Visible = false;
            Label2.Text = "";
        }
        if (_ob.anh_capbang != "")
        {
            Button3.Visible = true;
            Label3.Text = "<img src='" + _ob.anh_capbang + "' style='max-width: 100px' />";
        }
        else
        {
            Button3.Visible = false;
            Label3.Text = "";
        }

    }

    private void BindPersonHub(string displayName, string phone)
    {
        string normalizedPhone = AccountAuth_cl.NormalizePhone(phone);
        if (string.IsNullOrWhiteSpace(normalizedPhone))
        {
            personHubUrl = "/gianhang/admin/quan-ly-con-nguoi/Default.aspx";
            personHubStatusLabel = "Thiếu số điện thoại";
            personHubStatusCss = "bg-gray fg-white";
            personHubNote = "Cập nhật số điện thoại để thành viên này được gom vào hồ sơ người chung.";
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
        IList<GianHangAdminPersonHub_cl.PersonSourceRef> sources = GianHangAdminPersonHub_cl.GetOtherSourcesForPhone(db, user_parent, normalizedPhone, "member", id);
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
        GianHangAdminPersonHub_cl.PersonSourceRef sourceInfo = GianHangAdminPersonHub_cl.GetSourceInfo(db, user_parent, normalizedPhone, "member", id);
        if (sourceInfo == null)
            return;

        personHubAdminAccessLabel = string.IsNullOrWhiteSpace(sourceInfo.AdminAccessLabel)
            ? "Không mở quyền /gianhang/admin ở vai trò thành viên / học viên"
            : sourceInfo.AdminAccessLabel;
        personHubAdminAccessCss = string.IsNullOrWhiteSpace(sourceInfo.AdminAccessCss)
            ? "bg-gray fg-white"
            : sourceInfo.AdminAccessCss;
        personHubAdminAccessNote = "Vai trò thành viên / học viên chỉ là dữ liệu nghiệp vụ. Nếu cùng người này còn là nhân sự nội bộ thì quyền vào /gianhang/admin sẽ được quyết định ở hồ sơ nhân sự nội bộ đó.";
    }

    private string BuildImpactNote(string normalizedPhone)
    {
        bool hasOtherRoles = !string.IsNullOrWhiteSpace(BuildRelatedRolesHtml(normalizedPhone));
        return hasOtherRoles
            ? "Xóa vai trò thành viên / học viên này sẽ không tự gỡ liên kết Home ở Hồ sơ người. Các vai trò khác cùng số điện thoại trong gian hàng vẫn tiếp tục được gom chung và giữ trạng thái liên kết hiện có."
            : "Xóa vai trò thành viên / học viên này sẽ không tự gỡ liên kết Home ở Hồ sơ người. Nếu đây là vai trò nguồn cuối cùng thì hồ sơ trung tâm vẫn được giữ, chỉ chuyển sang trạng thái chưa còn vai trò nguồn.";
    }

    private void BindMemberLifecycle(hocvien_table member)
    {
        if (member == null)
            return;

        GianHangAdminSourceLifecycle_cl.SourceLifecycleInfo info = GianHangAdminSourceLifecycle_cl.GetInfo(
            db,
            user_parent,
            "member",
            member.id + "",
            "Đang dùng thành viên",
            "Đã ngừng dùng thành viên",
            "Vai trò thành viên / học viên này đang được dùng bình thường trong module nguồn.",
            "Vai trò thành viên / học viên này đang ở trạng thái ngừng dùng an toàn. Liên kết Home trung tâm và lịch sử nghiệp vụ vẫn được giữ.");

        sourceLifecycleLabel = info == null || string.IsNullOrWhiteSpace(info.Label) ? "Đang dùng thành viên" : info.Label;
        sourceLifecycleCss = info == null || string.IsNullOrWhiteSpace(info.Css) ? "bg-green fg-white" : info.Css;
        sourceLifecycleNote = info == null || string.IsNullOrWhiteSpace(info.Note)
            ? "Vai trò thành viên / học viên này đang được dùng bình thường trong module nguồn."
            : info.Note;
        sourceLifecycleIsInactive = info != null && info.IsInactive;
        personHubImpactTitle = sourceLifecycleIsInactive ? "Tác động khi xóa thành viên đã ngừng dùng" : "Tác động khi xóa vai trò thành viên / học viên";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;
        #region Check quyen theo nganh
        user = (access.User ?? "").Trim();
        user_parent = access.OwnerAccountKey;
        if (HasAnyPermission("q14_3", "n14_3"))
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
            {
                id = Request.QueryString["id"].ToString().Trim();
                if (hv_cl.exist_id(id))
                {
                    main();
                    reload_thanhtoan();
                }
                else
                {
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                    Response.Redirect("/gianhang/admin");
                }
            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin");
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        #endregion



    }

    //autocomplete ngành học
    //[System.Web.Script.Services.ScriptMethod()]
    //[System.Web.Services.WebMethod]
    //public static List<string> SearchCustomers(string prefixText, int count)
    //{
    //    dbDataContext db1 = new dbDataContext();
    //    return db1.giangvien_tables.Where(p => p.chuyenmon.Contains(prefixText)).Select(p => p.chuyenmon).ToList();
    //}

    protected void button1_Click1(object sender, EventArgs e)
    {
        if (!HasAnyPermission("q14_3", "n14_3"))
        {
            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "warning", "");
            return;
        }

        string _fullname = str_cl.VietHoa_ChuCai_DauTien(str_cl.remove_blank(txt_hoten.Text.Trim().ToLower()));
        string _ngaysinh = txt_ngaysinh.Text;
        string _ngaycapbang = txt_ngaycapbang.Text;
        string _email = txt_email.Text.ToLower().Trim();
        string _sdt = txt_dienthoai.Text.Trim();
        string _zalo = txt_zalo.Text.Trim();
        string _facebook = txt_facebook.Text.ToLower().Trim();
        //string _nganhhoc = txt_nganhhoc.Text.Trim();
        string _nganhhoc = DropDownList5.SelectedValue.ToString();
        string _idgiangvien = DropDownList1.SelectedValue.ToString();

        string _sobuoi_lt = txt_sobuoi_lythuyet.Text.Trim().Replace(".", "").Replace(",", ""); int _r1 = 0; int.TryParse(_sobuoi_lt, out _r1); if (_r1 < 0) _r1 = 0;
        string _sobuoi_th = txt_sobuoi_thuchanh.Text.Trim().Replace(".", "").Replace(",", ""); int _r2 = 0; int.TryParse(_sobuoi_th, out _r2); if (_r2 < 0) _r2 = 0;
        string _sobuoi_tg = txt_sobuoi_trogiang.Text.Trim().Replace(".", "").Replace(",", ""); int _r3 = 0; int.TryParse(_sobuoi_tg, out _r3); if (_r3 < 0) _r3 = 0;
        string _hocphi = txt_hocphi.Text.Trim().Replace(".", "").Replace(",", ""); int _r4 = 0; int.TryParse(_hocphi, out _r4); if (_r4 < 0) _r4 = 0;

        if (_fullname == "")
            notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng nhập họ tên.", "false", "false", "OK", "warning", "");
        else
        {
            if (_nganhhoc == "")
                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng chọn ngành học", "false", "false", "OK", "warning", "");
            else
            {
                if (_idgiangvien == "")
                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Vui lòng chọn Chuyên gia.", "false", "false", "OK", "warning", "");
                else
                {
                    if (_ngaysinh != "" && dt_cl.check_date(_ngaysinh) == false)//nếu có nhập mới kiểm tra
                        notifi = thongbao_class.metro_notifi_onload("Thông báo", "Ngày sinh không hợp lệ.", "4000", "warning");
                    else
                    {
                        if (!regex_class.check_email_invalid(_email) && _email != "")//nếu có nhập mail thì kiểm tra định dạng mail
                            notifi = thongbao_class.metro_notifi_onload("Thông báo", "Email không hợp lệ.", "4000", "warning");
                        else
                        {
                            hocvien_table _ob1 = db.hocvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
                            string _oldPhone = (_ob1.dienthoai ?? "").Trim();

                            bool _checkloi = false;
                            string _avt = _ob1.anhdaidien;
                            if (FileUpload2.HasFile)//nếu có ảnh thu nhỏ đc chọn
                            {
                                string _ext = Path.GetExtension(FileUpload2.FileName).ToLower();
                                if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                                {
                                    //byte - kb - mb  ContentLength trra về byte của file
                                    long _filesize = (FileUpload2.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                                    if (_filesize <= 1) //>1MB
                                    {
                                        _avt = "/uploads/images/avatar/" + Guid.NewGuid() + _ext;
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

                            string _anh_capbang = _ob1.anh_capbang;
                            if (FileUpload3.HasFile)//nếu có ảnh thu nhỏ đc chọn
                            {
                                string _ext = Path.GetExtension(FileUpload3.FileName).ToLower();
                                if (_ext == ".jpg" || _ext == ".jpeg" || _ext == ".png" || _ext == ".gif")
                                {
                                    //byte - kb - mb  ContentLength trra về byte của file
                                    long _filesize = (FileUpload3.PostedFile.ContentLength) / 1024 / 1024;//đổi ra MB
                                    if (_filesize <= 1) //>1MB
                                    {
                                        _anh_capbang = "/uploads/images/cap-bang/" + Guid.NewGuid() + _ext;
                                        FileUpload3.SaveAs(Server.MapPath("~" + _anh_capbang));//lưu ảnh mới
                                    }
                                    else
                                    {
                                        notifi = thongbao_class.metro_dialog_onload("Thông báo", "Kích thước ảnh cấp bằng quá lớn. Vui lòng chọn ảnh có dung lượng <1Mb.", "false", "false", "OK", "alert", "");
                                        _checkloi = true;
                                    }
                                }
                                else
                                {
                                    notifi = thongbao_class.metro_dialog_onload("Thông báo", "Ảnh cấp bằng không đúng định dạng.", "false", "false", "OK", "alert", "");
                                    _checkloi = true;
                                }
                            }

                            _ob1.hoten = _fullname;
                            _ob1.hoten_khongdau = str_cl.remove_vietnamchar(_fullname);
                            _ob1.anhdaidien = _avt;
                            _ob1.anh_capbang = _anh_capbang;
                            if (_ngaysinh != "")
                                _ob1.ngaysinh = DateTime.Parse(_ngaysinh);
                            else
                                _ob1.ngaysinh = null;

                            _ob1.email = _email;
                            _ob1.dienthoai = _sdt;
                            _ob1.zalo = _zalo;
                            _ob1.facebook = _facebook;

                            _ob1.sobuoi_lythuyet = _r1; _ob1.sobuoi_thuchanh = _r2; _ob1.sobuoi_trogiang = _r2;
                            _ob1.goidaotao = DropDownList2.SelectedValue.ToString();

                            _ob1.nganhhoc = _nganhhoc;
                            _ob1.id_giangvien = _idgiangvien;
                            _ob1.tengiangvien_hientai = gv_cl.return_object(_ob1.id_giangvien).hoten;
                            _ob1.xeploai = DropDownList3.SelectedValue.ToString();
                            _ob1.capbang = DropDownList4.SelectedValue.ToString();
                            if (_ngaycapbang != "")
                                _ob1.ngaycapbang = DateTime.Parse(_ngaycapbang);
                            else
                                _ob1.ngaycapbang = null;

                            _ob1.hocphi = _r4;

                            var q_tt = db.hocvien_lichsu_thanhtoan_tables.Where(p => p.id_hocvien == id && p.id_chinhanh == Session["chinhanh"].ToString());
                            if (q_tt.Count() != 0)
                            {
                                _ob1.sotien_dathanhtoan = q_tt.Sum(p => p.sotienthanhtoan);
                                _ob1.sotien_conlai = _ob1.hocphi - _ob1.sotien_dathanhtoan;
                            }
                            else
                            {
                                _ob1.sotien_conlai = _ob1.hocphi - _ob1.sotien_dathanhtoan;
                            }

                            if (_checkloi == false)
                            {
                                db.SubmitChanges();
                                GianHangAdminPersonHub_cl.SyncSourcePhoneState(db, user_parent, _oldPhone, _sdt, _fullname, user);
                                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                                Response.Redirect("/gianhang/admin/quan-ly-hoc-vien/Default.aspx");
                            }

                        }
                    }
                }
            }
        }
    }

    protected void Button2_Click(object sender, EventArgs e)//xóa ảnh đại diện
    {
        if (!HasAnyPermission("q14_3", "n14_3"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        hocvien_table _ob = db.hocvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        if (_ob.anhdaidien != "/uploads/images/macdinh.jpg")
        {
            file_folder_class.del_file(_ob.anhdaidien);//xóa ảnh cũ
            _ob.anhdaidien = "/uploads/images/macdinh.jpg";
        }
        db.SubmitChanges();
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa ảnh đại diện thành công.", "4000", "warning"), true);
    }
    protected void Button3_Click(object sender, EventArgs e)//xóa ảnh câp bằng
    {
        if (!HasAnyPermission("q14_3", "n14_3"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        hocvien_table _ob = db.hocvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        file_folder_class.del_file(_ob.anh_capbang);//xóa ảnh cũ
        _ob.anh_capbang = "";
        db.SubmitChanges();
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa ảnh cấp bằng thành công.", "4000", "warning"), true);
    }

    protected void but_ngung_hocvien_Click(object sender, EventArgs e)
    {
        if (!HasAnyPermission("q14_3", "n14_3"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Bạn không đủ quyền để đổi trạng thái thành viên.", "4000", "warning"), true);
            return;
        }

        GianHangAdminSourceLifecycle_cl.SetInactive(db, user_parent, "member", id, user, "Thành viên / học viên được chuyển sang trạng thái ngừng dùng an toàn từ trang chi tiết.");
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Đã chuyển thành viên sang trạng thái ngừng dùng an toàn.", "3200", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-hoc-vien/edit.aspx?id=" + id);
    }

    protected void but_molai_hocvien_Click(object sender, EventArgs e)
    {
        if (!HasAnyPermission("q14_3", "n14_3"))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Bạn không đủ quyền để đổi trạng thái thành viên.", "4000", "warning"), true);
            return;
        }

        GianHangAdminSourceLifecycle_cl.SetActive(db, user_parent, "member", id, user, "Thành viên / học viên được mở lại từ trang chi tiết.");
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Đã mở lại trạng thái dùng của thành viên.", "3200", "success");
        Response.Redirect("/gianhang/admin/quan-ly-hoc-vien/edit.aspx?id=" + id);
    }


    protected void but_xoathanhtoan_Click(object sender, ImageClickEventArgs e)
    {
        if (HasAnyPermission("q14_5", "n14_5"))
        {
            var q_thanhtoan = db.hocvien_lichsu_thanhtoan_tables.Where(p => p.id_hocvien == id && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.thoigian).ToList();
            foreach (var t in q_thanhtoan)
            {
                if (Request.Form["check_lichsu_thanhtoan_" + t.id.ToString()] == "on")
                {
                    var q = db.hocvien_lichsu_thanhtoan_tables.Where(p => p.id == t.id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (q.Count() != 0)
                    {
                        Int64 _sotien_thanhtoan = q.First().sotienthanhtoan.Value;
                        string _hinhthuc_thanhtoan = q.First().hinhthuc_thanhtoan;

                        var q_hoadon = db.hocvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                        hocvien_table _ob = q_hoadon.First();
                        _ob.sotien_dathanhtoan = _ob.sotien_dathanhtoan - _sotien_thanhtoan;
                        _ob.sotien_conlai = _ob.hocphi - _ob.sotien_dathanhtoan;
                        db.SubmitChanges();

                        //xóa lịch sử
                        hocvien_lichsu_thanhtoan_table _ob1 = q.First();
                        HoaDonThuChiSync_cl.DeleteForHocVienPayment(db, _ob1.id.ToString(), Session["chinhanh"].ToString());
                        db.hocvien_lichsu_thanhtoan_tables.DeleteOnSubmit(_ob1);
                        db.SubmitChanges();
                    }
                }
            }

            reload_thanhtoan();

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "warning"), true);
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }
    protected void but_thanhtoan_Click(object sender, EventArgs e)
    {
        if (HasAnyPermission("q14_5", "n14_5"))
        {
            if (sotien_conlai == 0)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "thành viên này đã thanh toán đủ.", "4000", "warning"), true);
            else
            {
                string _sotien = txt_sotien_thanhtoan_congno.Text.Trim().Replace(".", "");
                Int64 _st = 0;
                Int64.TryParse(_sotien, out _st);//nếu là số nguyên thì gán cho _st
                if (_st < 0)//nếu k phải là số hoặc nhập vô số âm thì trả về 0
                    _st = 0;
                if (_st > 0)
                {
                    //if (_st > sotien_conlai)
                    //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Số tiền thanh toán không được lớn hơn số tiền còn thiếu.", "4000", "warning"), true);
                    //else
                    //{
                    var q = db.hocvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString());
                    hocvien_table _ob = q.First();
                    _ob.sotien_dathanhtoan = _ob.sotien_dathanhtoan + _st;
                    _ob.sotien_conlai = _ob.hocphi - _ob.sotien_dathanhtoan;
                    db.SubmitChanges();

                    var q1 = db.hocvien_lichsu_thanhtoan_tables.Where(p => p.id_hocvien == id && p.id_chinhanh == Session["chinhanh"].ToString());
                    hocvien_lichsu_thanhtoan_table _ob1 = new hocvien_lichsu_thanhtoan_table();
                    _ob1.id_hocvien = id;
                    _ob1.sotienthanhtoan = _st;
                    _ob1.thoigian = DateTime.Parse(txt_ngaythanhtoan.Text + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
                    _ob1.hinhthuc_thanhtoan = ddl_hinhthuc_thanhtoan.SelectedValue.ToString();
                    _ob1.nguoithanhtoan = user;
                    db.hocvien_lichsu_thanhtoan_tables.InsertOnSubmit(_ob1);
                    db.SubmitChanges();
                    HoaDonThuChiSync_cl.UpsertFromHocVienPayment(db, _ob, _ob1, user_parent);
                    db.SubmitChanges();

                    reload_thanhtoan();

                    txt_sotien_thanhtoan_congno.Text = _ob.sotien_conlai.Value.ToString("#,##0");
                    dtkh_cl.tinhtong_chitieu_update_capbac(sdt_kh);//cập nhật điểm eha

                    //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thanh toán thành công.", "4000", "warning");
                    //Response.Redirect("/gianhang/admin/quan-ly-the-dich-vu/Default.aspx");
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thanh toán thành công.", "4000", "warning"), true);
                    //}
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Số tiền thanh toán không hợp lệ", "4000", "warning"), true);

            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }
    public void reload_thanhtoan()
    {
        //form thanh toán
        var q_thanhtoan = db.hocvien_lichsu_thanhtoan_tables.Where(p => p.id_hocvien == id && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.thoigian).ToList();
        Repeater2.DataSource = q_thanhtoan;
        Repeater2.DataBind();

        hocvien_table _ob = db.hocvien_tables.Where(p => p.id.ToString() == id && p.id_chinhanh == Session["chinhanh"].ToString()).First();
        sotien_conlai = _ob.sotien_conlai.Value;
        if (sotien_conlai == 0)
        {
            //var q_chitiet = db.bspa_hoadon_chitiet_tables.Where(p => p.id_hoadon == id);
            //if (q_chitiet.Count() != 0)
            Label1.Text = "<span class='fg-red'><b>Đã thanh toán.</b></span>";
            //else
            // Label1.Text = "";
        }
        else
            Label1.Text = "<span class='fg-red'><b>Chưa thanh toán: " + sotien_conlai.ToString("#,##0") + "</b></span>";

        if (!IsPostBack)
            txt_sotien_thanhtoan_congno.Text = sotien_conlai.ToString("#,##0");
    }
}
