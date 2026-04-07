using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class gianhang_person_hub_detail : System.Web.UI.Page
{
    protected string DisplayName = "";
    protected string DisplayPhone = "";
    protected string LinkCss = "bg-gray fg-white";
    protected string LinkStatusLabel = "Chưa liên kết";
    protected string RoleSummaryHtml = "<span class='fg-gray'>Chưa có vai trò nào.</span>";
    protected string SourceCountText = "0";
    protected string FirstSeenText = "Chưa rõ";
    protected string LatestSeenText = "Chưa rõ";
    protected string AdminAccessSummaryText = "Không có vai trò nào mở quyền vào /gianhang/admin";
    protected string InternalAccessHintHtml = "";
    protected string RemovedSourceCountText = "0";
    private string OwnerAccountKey = "";
    private string NormalizedPhone = "";

    private string ResolveActor()
    {
        return GianHangAdminContext_cl.ResolveDisplayAccountKey();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!GianHangSystemAdminGuard_cl.EnsurePageAccess(this))
            return;

        if (!AdvancedAdminAccessGate_cl.EnsurePageAccess(this))
            return;

        OwnerAccountKey = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();
        NormalizedPhone = AccountAuth_cl.NormalizePhone(Request.QueryString["phone"]);

        if (NormalizedPhone == "")
        {
            ph_not_found.Visible = true;
            ph_detail.Visible = false;
            return;
        }

        LoadDetail();
    }

    private void LoadDetail()
    {
        using (dbDataContext db = new dbDataContext())
        {
            GianHangAdminPersonHub_cl.PersonDetail detail = GianHangAdminPersonHub_cl.GetPersonDetail(db, OwnerAccountKey, NormalizedPhone);
            if (detail == null)
            {
                ph_not_found.Visible = true;
                ph_detail.Visible = false;
                return;
            }

            DisplayName = detail.PrimaryName;
            DisplayPhone = detail.DisplayPhone;
            LinkCss = detail.LinkInfo == null ? "bg-gray fg-white" : detail.LinkInfo.LinkCss;
            LinkStatusLabel = detail.LinkInfo == null ? "Chưa liên kết" : detail.LinkInfo.StatusLabel;
            RoleSummaryHtml = BuildRoleSummaryHtml(detail);
            SourceCountText = (detail.Sources == null ? 0 : detail.Sources.Count).ToString("#,##0");
            RemovedSourceCountText = (detail.RemovedSources == null ? 0 : detail.RemovedSources.Count).ToString("#,##0");
            FirstSeenText = ResolveSeenText(detail, true);
            LatestSeenText = ResolveSeenText(detail, false);
            AdminAccessSummaryText = string.IsNullOrWhiteSpace(detail.AdminAccessSummary)
                ? "Không có vai trò nào mở quyền vào /gianhang/admin"
                : detail.AdminAccessSummary;
            InternalAccessHintHtml = BuildInternalAccessHintHtml(detail);

            lit_link_status.Text = BuildLinkStatusHtml(detail.LinkInfo);
            RepeaterSources.DataSource = detail.Sources;
            RepeaterSources.DataBind();
            ph_empty_sources.Visible = detail.Sources == null || detail.Sources.Count == 0;
            RepeaterRemovedSources.DataSource = detail.RemovedSources;
            RepeaterRemovedSources.DataBind();
            ph_removed_sources.Visible = detail.RemovedSources != null && detail.RemovedSources.Count > 0;
            ph_internal_hint.Visible = detail.HasInternalStaffRole || detail.HasAdminEligibleRole;
            ph_not_found.Visible = false;
            ph_detail.Visible = true;
        }
    }

    protected void but_link_existing_Click(object sender, EventArgs e)
    {
        string message;
        using (dbDataContext db = new dbDataContext())
        {
            bool ok = GianHangAdminPersonHub_cl.TryLinkExistingHome(
                db,
                OwnerAccountKey,
                NormalizedPhone,
                txt_home_account_link.Text,
                DisplayName,
                ResolveActor(),
                out message);

            if (ok)
            {
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", message, "2600", "success");
                Response.Redirect(Request.RawUrl, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }

        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", message, "3500", "warning"), true);
    }

    protected void but_create_pending_Click(object sender, EventArgs e)
    {
        string message;
        using (dbDataContext db = new dbDataContext())
        {
            bool ok = GianHangAdminPersonHub_cl.TryCreatePendingLink(
                db,
                OwnerAccountKey,
                NormalizedPhone,
                DisplayName,
                ResolveActor(),
                out message);

            if (ok)
            {
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", message, "2600", "warning");
                Response.Redirect(Request.RawUrl, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }

        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", message, "3500", "warning"), true);
    }

    protected void but_unlink_Click(object sender, EventArgs e)
    {
        string message;
        using (dbDataContext db = new dbDataContext())
        {
            bool ok = GianHangAdminPersonHub_cl.TryUnlink(
                db,
                OwnerAccountKey,
                NormalizedPhone,
                ResolveActor(),
                out message);

            if (ok)
            {
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", message, "2600", "warning");
                Response.Redirect(Request.RawUrl, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }

        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", message, "3500", "warning"), true);
    }

    protected string RenderCreatedAt(object value)
    {
        DateTime? createdAt = null;
        if (value is DateTime)
            createdAt = (DateTime)value;
        else if (value is DateTime?)
            createdAt = (DateTime?)value;

        if (createdAt.HasValue == false && value != null)
        {
            DateTime parsed;
            if (DateTime.TryParse(value.ToString(), out parsed))
                createdAt = parsed;
        }

        if (!createdAt.HasValue)
            return string.Empty;

        return " • Ghi nhận: <strong>" + HttpUtility.HtmlEncode(createdAt.Value.ToString("dd/MM/yyyy HH:mm")) + "</strong>";
    }

    protected string RenderAdminAccessCss(object value)
    {
        string css = (value ?? "").ToString().Trim();
        return css == "" ? "bg-gray fg-white" : css;
    }

    protected string RenderAdminAccessLabel(object value)
    {
        string label = (value ?? "").ToString().Trim();
        return label == "" ? "Không mở quyền /gianhang/admin ở nguồn này" : label;
    }

    protected string RenderSourceLifecycleCss(object value)
    {
        string css = (value ?? "").ToString().Trim();
        return css == "" ? "bg-green fg-white" : css;
    }

    protected string RenderSourceLifecycleLabel(object value)
    {
        string label = (value ?? "").ToString().Trim();
        return label == "" ? "Đang dùng ở nguồn" : label;
    }

    protected string RenderRemovedSourceMeta(GianHangAdminPersonHub_cl.PersonRemovedSourceRef item)
    {
        if (item == null)
            return "Vai trò đã bị gỡ khỏi module nguồn.";

        string actor = string.IsNullOrWhiteSpace(item.CreatedBy) ? "" : " • Thao tác bởi: " + item.CreatedBy;
        string time = item.CreatedAt.HasValue ? " • Ghi nhận: " + item.CreatedAt.Value.ToString("dd/MM/yyyy HH:mm") : "";
        string note = string.IsNullOrWhiteSpace(item.EventNote) ? "Vai trò này đã bị gỡ khỏi module nguồn, nhưng Hồ sơ người trung tâm vẫn được giữ." : item.EventNote.Trim();
        return HttpUtility.HtmlEncode(note + actor + time);
    }

    private string BuildLinkStatusHtml(GianHangAdminPersonHub_cl.PersonLinkInfo info)
    {
        if (info == null)
            return "<div class='fg-gray'>Chưa có liên kết Home nào cho hồ sơ người này.</div>";

        if (info.LinkedHomeAccount != null)
        {
            string display = string.IsNullOrWhiteSpace(info.LinkedHomeAccount.hoten)
                ? (info.LinkedHomeAccount.taikhoan ?? "")
                : info.LinkedHomeAccount.hoten;

            return "<div class='fg-gray'>Đang liên kết với tài khoản Home <strong>" + HttpUtility.HtmlEncode(display) + "</strong> • <strong>" + HttpUtility.HtmlEncode(info.LinkedHomeAccount.taikhoan) + "</strong></div>";
        }

        if (!string.IsNullOrWhiteSpace(info.PendingPhone))
            return "<div class='fg-gray'>Đang chờ số điện thoại <strong>" + HttpUtility.HtmlEncode(info.PendingPhone) + "</strong> đăng ký hoặc đăng nhập AhaSale để hệ thống tự gắn.</div>";

        return "<div class='fg-gray'>Chưa gắn tài khoản Home nào. Bạn có thể liên kết ngay tài khoản đã tồn tại hoặc tạo chờ liên kết theo số điện thoại này.</div>";
    }

    private static string BuildRoleSummaryHtml(GianHangAdminPersonHub_cl.PersonDetail detail)
    {
        if (detail == null || detail.Sources == null || detail.Sources.Count == 0)
            return "<span class='fg-gray'>Chưa có vai trò nào.</span>";

        return string.Join("", detail.Sources
            .Select(p => p.RoleLabel)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(p => "<span class='personhub-role-chip'>" + HttpUtility.HtmlEncode(p) + "</span>"));
    }

    private static string ResolveSeenText(GianHangAdminPersonHub_cl.PersonDetail detail, bool first)
    {
        if (detail == null)
            return "Chưa rõ";

        List<DateTime> dates = new List<DateTime>();
        if (detail.Sources != null)
        {
            dates.AddRange(detail.Sources
                .Where(p => p != null && p.CreatedAt.HasValue)
                .Select(p => p.CreatedAt.Value));
        }

        if (dates.Count == 0)
        {
            if (first && detail.LinkCreatedAt.HasValue)
                dates.Add(detail.LinkCreatedAt.Value);
            if (!first && detail.LinkUpdatedAt.HasValue)
                dates.Add(detail.LinkUpdatedAt.Value);
        }

        if (dates.Count == 0)
            return "Chưa rõ";

        DateTime chosen = first ? dates.First() : dates.Last();
        return chosen.ToString("dd/MM/yyyy HH:mm");
    }

    private static string BuildInternalAccessHintHtml(GianHangAdminPersonHub_cl.PersonDetail detail)
    {
        if (detail == null)
            return "Vai trò nội bộ trong /gianhang/admin sẽ được quản lý tập trung từ Hồ sơ người.";

        if (detail.HasGrantedAdminAccess)
            return "Số điện thoại này đang có ít nhất một vai trò nội bộ đã được mở quyền vào /gianhang/admin. Sau khi liên kết Home tại đây, người dùng sẽ dùng chính tài khoản Home đó để vào không gian quản trị tương ứng.";

        if (detail.HasPendingAdminAccess)
            return "Số điện thoại này đang có vai trò nội bộ chờ kích hoạt quyền vào /gianhang/admin. Khi tài khoản Home tương ứng được gắn hoặc kích hoạt xong, hệ thống sẽ cho phép vào không gian quản trị theo membership đang chờ.";

        if (detail.HasAdminEligibleRole || detail.HasInternalStaffRole)
            return "Số điện thoại này đang xuất hiện ở vai trò nội bộ, nhưng hiện chưa được mở membership vào /gianhang/admin. Muốn người này dùng được không gian quản trị, hãy cấp đúng membership nội bộ rồi quay lại liên kết Home tại đây.";

        return "Vai trò nội bộ trong /gianhang/admin sẽ được quản lý tập trung từ Hồ sơ người.";
    }
}
