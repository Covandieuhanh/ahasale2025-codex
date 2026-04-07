using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class gianhang_taikhoan_detail : System.Web.UI.Page
{
    public string notifi, user_parent, user, url_back, trangthai, ngaysinh, ngaytao, nguoitao, email, sdt, zalo, facebook, hsd, songaycong,luongcb;
    public string home_linked_info = "Chưa có tài khoản Home nào được liên kết.";
    public string personHubUrl = "/gianhang/admin/quan-ly-con-nguoi/Default.aspx";
    public string personHubRelatedRolesHtml = "";
    public string personHubAdminAccessLabel = "Có vai trò nội bộ nhưng chưa mở quyền /gianhang/admin";
    public string personHubAdminAccessCss = "bg-gray fg-white";
    public string personHubAdminAccessNote = "Nhân sự nội bộ chỉ dùng được /gianhang/admin khi đã được mở membership đúng vai trò.";
    public string personHubImpactTitle = "Tác động khi khóa hoặc gỡ vai trò nguồn";
    public string personHubImpactNote = "Nếu khóa hoặc gỡ vai trò nhân sự này, quyền vào /gianhang/admin sẽ bị thu hồi ở nguồn nội bộ. Liên kết Home trong Hồ sơ người vẫn được giữ để bạn có thể mở lại sau này.";
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    string_class str_cl = new string_class();
    datetime_class dt_cl = new datetime_class();
    public string hoten;
    private bool HasAnyPermission(string currentUser, params string[] permissionKeys)
    {
        if (string.IsNullOrWhiteSpace(currentUser) || permissionKeys == null)
            return false;

        for (int i = 0; i < permissionKeys.Length; i++)
        {
            string permissionKey = (permissionKeys[i] ?? "").Trim();
            if (permissionKey != "" && bcorn_class.check_quyen(currentUser, permissionKey) == "")
                return true;
        }

        return false;
    }

    private bool IsCurrentRootAdmin(string currentUser)
    {
        return string.Equals((currentUser ?? "").Trim(), "admin", StringComparison.OrdinalIgnoreCase);
    }

    private void RedirectToAdminHome()
    {
        Response.Redirect(GianHangAdminBridge_cl.BuildAdminHomeUrl(HttpContext.Current));
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        #region Check quyen theo nganh
        string qsUser = (Request.QueryString["user"] ?? "").Trim();
        user = qsUser;
        string currentUser = (access.User ?? "").Trim();
        user_parent = access.OwnerAccountKey;
        if (HasAnyPermission(currentUser, "q2_2", "n2_2") || string.Equals(user, currentUser, StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(qsUser))
            {
                user = qsUser;
                if (tk_cl.exist_user(user))
                {
                    if (string.Equals(user, "admin", StringComparison.OrdinalIgnoreCase) && !IsCurrentRootAdmin(currentUser))
                    {
                        Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không thể xem tài khoản admin.", "false", "false", "OK", "alert", "");
                        RedirectToAdminHome();
                    }
                    else
                    {
                        main();
                        url_back = GianHangAdminBridge_cl.ResolvePreferredAdminRedirectUrl(HttpContext.Current, "", "/gianhang/admin");
                    }
                }
                else
                {
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                    RedirectToAdminHome();
                }
            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                RedirectToAdminHome();
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            RedirectToAdminHome();
        }
        #endregion
       
    }
    //protected void Page_InitComplete(object sender, EventArgs e)
    //{

    //}
    public void main()
    {
        string chinhanhId = (Session["chinhanh"] ?? "").ToString();
        taikhoan_table_2023 _ob = db.taikhoan_table_2023s
            .FirstOrDefault(p => p.taikhoan == user && (string.IsNullOrWhiteSpace(chinhanhId) || p.id_chinhanh == chinhanhId));
        if (_ob == null)
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Không tìm thấy tài khoản cần xem.", "false", "false", "OK", "alert", "");
            RedirectToAdminHome();
            return;
        }
        if (!IsPostBack)
        {
            hoten = _ob.hoten;
            trangthai = _ob.trangthai;
            ngaysinh = _ob.ngaysinh != null ? _ob.ngaysinh.Value.ToString("dd/MM/yyyy") : "";
            ngaytao = _ob.ngaytao.Value.ToString("dd/MM/yyyy");
            nguoitao = _ob.nguoitao;
            songaycong = (_ob.songaycong ?? 0).ToString();
            luongcb = (_ob.luongcoban ?? 0).ToString("#,##0");

            email = _ob.email; sdt = _ob.dienthoai; zalo = _ob.zalo; facebook = _ob.facebook;
            hsd = _ob.hansudung != null ? _ob.hansudung.Value.ToString("dd/MM/yyyy") : "Không có hạn";

            if (_ob.trangthai == "Đang hoạt động")
            {
                but_khoa.Visible = true;
                but_mokhoa.Visible = false;
            }
            else
            {
                but_khoa.Visible = false;
                but_mokhoa.Visible = true;
            }
        }
        BindLinkedHome(_ob);
        if (!string.IsNullOrEmpty(_ob.anhdaidien))
            Label2.Text = "<img src='" + _ob.anhdaidien + "' class='img-cover-vuongtron' width='100' height='100' />";
        else
            Label2.Text = "<img src='/uploads/images/macdinh.jpg' class='img-cover-vuongtron' width='100' height='100' />";

    }

    private void BindLinkedHome(taikhoan_table_2023 legacyUser)
    {
        string ownerAccount = (user_parent ?? "").Trim().ToLowerInvariant();
        if (ownerAccount == "")
            ownerAccount = ((Session["user_parent"] ?? "") + "").Trim().ToLowerInvariant();
        string normalizedPhone = AccountAuth_cl.NormalizePhone(legacyUser.dienthoai);
        personHubUrl = GianHangAdminPersonHub_cl.BuildDetailUrl(normalizedPhone);
        BindSourceAdminAccess(ownerAccount, normalizedPhone, "staff", legacyUser.taikhoan);
        personHubRelatedRolesHtml = BuildRelatedRolesHtml(ownerAccount, normalizedPhone, "staff", legacyUser.taikhoan);
        personHubImpactNote = BuildImpactNote(legacyUser, normalizedPhone);
        GianHangAdminPersonHub_cl.PersonLinkInfo binding = GianHangAdminPersonHub_cl.GetLinkInfo(db, ownerAccount, normalizedPhone, legacyUser.hoten);
        if (binding == null)
        {
            lit_home_linked.Text = "Chưa có tài khoản Home nào được liên kết.";
            return;
        }

        string html = "<span class='tag " + (binding.Status == "pending" ? "warning" : (binding.Status == "active" ? "success" : "gray")) + "'>" + Server.HtmlEncode(binding.StatusLabel) + "</span>";
        if (normalizedPhone == "")
        {
            html += "<div class='mt-1 fg-red'>Hồ sơ này chưa có số điện thoại nên chưa thể gắn Home tập trung. Hãy cập nhật số điện thoại trước.</div>";
            personHubUrl = "/gianhang/admin/quan-ly-con-nguoi/Default.aspx";
            lit_home_linked.Text = html;
            return;
        }

        taikhoan_tb linkedHome = binding.LinkedHomeAccount;
        if (linkedHome != null)
        {
            string displayName = string.IsNullOrWhiteSpace(linkedHome.hoten) ? linkedHome.taikhoan : linkedHome.hoten;
            html += "<div class='mt-1 fg-gray'>Home: <strong>" + Server.HtmlEncode(displayName) + "</strong> • <strong>" + Server.HtmlEncode(linkedHome.taikhoan) + "</strong></div>";
        }
        else if (!string.IsNullOrWhiteSpace(binding.PendingPhone))
        {
            html += "<div class='mt-1 fg-gray'>Đang chờ số điện thoại <strong>" + Server.HtmlEncode(binding.PendingPhone) + "</strong> đăng ký hoặc đăng nhập AhaSale.</div>";
        }
        else
        {
            html += "<div class='mt-1 fg-gray'>Mở Hồ sơ người để gắn Home một lần cho toàn bộ vai trò cùng số điện thoại.</div>";
        }

        lit_home_linked.Text = html;
    }

    private string BuildRelatedRolesHtml(string ownerAccount, string normalizedPhone, string currentSourceType, string currentSourceKey)
    {
        IList<GianHangAdminPersonHub_cl.PersonSourceRef> sources = GianHangAdminPersonHub_cl.GetOtherSourcesForPhone(db, ownerAccount, normalizedPhone, currentSourceType, currentSourceKey);
        if (sources == null || sources.Count == 0)
            return "";

        return string.Join("", sources.Select(p =>
            "<div class='mt-2'>" +
            "<a class='fg-cobalt' href='" + HttpUtility.HtmlAttributeEncode(p.DetailUrl ?? "#") + "'>" + HttpUtility.HtmlEncode((p.SourceLabel ?? "").Trim() == "" ? "Hồ sơ liên quan" : p.SourceLabel) + "</a>" +
            "<span class='fg-gray'> • " + HttpUtility.HtmlEncode((p.Name ?? "").Trim() == "" ? (p.Phone ?? "") : p.Name) + "</span>" +
            "<div class='fg-gray'><small>Vai trò: <strong>" + HttpUtility.HtmlEncode(p.RoleLabel ?? "") + "</strong></small></div>" +
            "<div class='fg-gray'><small>Quyền /gianhang/admin: <span class='data-wrapper'><code class='" + HttpUtility.HtmlAttributeEncode(string.IsNullOrWhiteSpace(p.AdminAccessCss) ? "bg-gray fg-white" : p.AdminAccessCss) + "'>" + HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(p.AdminAccessLabel) ? "Không mở quyền /gianhang/admin ở nguồn này" : p.AdminAccessLabel) + "</code></span></small></div>" +
            "</div>"));
    }

    private void BindSourceAdminAccess(string ownerAccount, string normalizedPhone, string sourceType, string sourceKey)
    {
        GianHangAdminPersonHub_cl.PersonSourceRef sourceInfo = GianHangAdminPersonHub_cl.GetSourceInfo(db, ownerAccount, normalizedPhone, sourceType, sourceKey);
        if (sourceInfo == null)
            return;

        personHubAdminAccessLabel = string.IsNullOrWhiteSpace(sourceInfo.AdminAccessLabel)
            ? "Có vai trò nội bộ nhưng chưa mở quyền /gianhang/admin"
            : sourceInfo.AdminAccessLabel;
        personHubAdminAccessCss = string.IsNullOrWhiteSpace(sourceInfo.AdminAccessCss)
            ? "bg-gray fg-white"
            : sourceInfo.AdminAccessCss;

        if ((sourceInfo.AdminAccessStatus ?? "") == "active")
        {
            personHubAdminAccessNote = "Nhân sự này đã có membership nội bộ. Khi liên kết Home ở Hồ sơ người, người đó sẽ dùng chính tài khoản Home để vào /gianhang/admin theo quyền đã cấp.";
            return;
        }

        if ((sourceInfo.AdminAccessStatus ?? "") == "pending")
        {
            personHubAdminAccessNote = "Nhân sự này đang chờ kích hoạt membership vào /gianhang/admin. Sau khi hoàn tất liên kết Home, hệ thống sẽ cho phép truy cập theo membership đang chờ.";
            return;
        }

        personHubAdminAccessNote = "Nhân sự nội bộ chỉ dùng được /gianhang/admin khi đã được mở membership đúng vai trò. Nếu muốn người này đăng nhập quản trị, hãy cấp quyền nội bộ trước rồi quay lại Hồ sơ người để liên kết Home.";
    }

    private string BuildImpactNote(taikhoan_table_2023 legacyUser, string normalizedPhone)
    {
        bool hasOtherRoles = !string.IsNullOrWhiteSpace(BuildRelatedRolesHtml(user_parent, normalizedPhone, "staff", legacyUser.taikhoan));
        bool isActive = string.Equals((legacyUser.trangthai ?? "").Trim(), "Đang hoạt động", StringComparison.OrdinalIgnoreCase);
        if (!isActive)
        {
            return hasOtherRoles
                ? "Nhân sự này đang bị khóa ở nguồn nội bộ nên quyền vào /gianhang/admin đã bị thu hồi. Liên kết Home tại Hồ sơ người vẫn được giữ; các vai trò khác cùng số điện thoại vẫn tiếp tục được gom chung."
                : "Nhân sự này đang bị khóa ở nguồn nội bộ nên quyền vào /gianhang/admin đã bị thu hồi. Liên kết Home tại Hồ sơ người vẫn được giữ để bạn có thể mở lại nhân sự hoặc gắn lại vai trò nguồn sau này.";
        }

        return hasOtherRoles
            ? "Nếu khóa hoặc gỡ vai trò nhân sự này, quyền vào /gianhang/admin sẽ bị thu hồi ở nguồn nội bộ. Liên kết Home trong Hồ sơ người vẫn được giữ, và các vai trò khác cùng số điện thoại trong gian hàng vẫn tiếp tục được gom chung."
            : "Nếu khóa hoặc gỡ vai trò nhân sự này, quyền vào /gianhang/admin sẽ bị thu hồi ở nguồn nội bộ. Liên kết Home trong Hồ sơ người vẫn được giữ; nếu đây là vai trò nguồn cuối cùng thì hồ sơ trung tâm vẫn còn nhưng sẽ chuyển sang trạng thái chưa còn vai trò nguồn.";
    }


    protected void but_khoa_Click(object sender, EventArgs e)
    {
        string currentUser = GianHangAdminContext_cl.ResolveDisplayAccountKey();
        if (HasAnyPermission(currentUser, "q2_3", "n2_3"))// ="": có quyền; =2: k có quyền
        {
            if (string.Equals(user, "admin", StringComparison.OrdinalIgnoreCase) && !IsCurrentRootAdmin(currentUser))
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không thể khóa tài khoản admin.", "false", "false", "OK", "alert", "");
                RedirectToAdminHome();
            }
            else
            {
                string chinhanhId = (Session["chinhanh"] ?? "").ToString();
                var q = db.taikhoan_table_2023s.Where(p => p.taikhoan == user && (string.IsNullOrWhiteSpace(chinhanhId) || p.id_chinhanh == chinhanhId));
                if (q.Count() != 0)
                {
                    taikhoan_table_2023 _ob = q.First();
                    _ob.trangthai = "Đã bị khóa";
                    db.SubmitChanges();
                    GianHangAdminWorkspace_cl.SyncLegacySourceAccess(db, user_parent, user, false);
                }
                //main();
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Khóa thành công.", "4000", "warning"), true);
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Khóa thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + user);
            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Bạn không có đủ quyền để thực hiện hành động này.", "4000", "warning"), true);
    }

    protected void but_mokhoa_Click(object sender, EventArgs e)
    {
        string currentUser = GianHangAdminContext_cl.ResolveDisplayAccountKey();
        if (HasAnyPermission(currentUser, "q2_3", "n2_3"))// ="": có quyền; =2: k có quyền
        {
            if (string.Equals(user, "admin", StringComparison.OrdinalIgnoreCase) && !IsCurrentRootAdmin(currentUser))
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không thể mở khóa tài khoản admin.", "false", "false", "OK", "alert", "");
                RedirectToAdminHome();
            }
            else
            {
                string chinhanhId = (Session["chinhanh"] ?? "").ToString();
                var q = db.taikhoan_table_2023s.Where(p => p.taikhoan == user && (string.IsNullOrWhiteSpace(chinhanhId) || p.id_chinhanh == chinhanhId));
                if (q.Count() != 0)
                {
                    taikhoan_table_2023 _ob = q.First();
                    _ob.trangthai = "Đang hoạt động";
                    db.SubmitChanges();
                    GianHangAdminWorkspace_cl.SyncLegacySourceAccess(db, user_parent, user, true);
                }
                //main();
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Mở khóa thành công.", "4000", "warning"), true);
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Mở khóa thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + user);
            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Bạn không có đủ quyền để thực hiện hành động này.", "4000", "warning"), true);
    }

}
