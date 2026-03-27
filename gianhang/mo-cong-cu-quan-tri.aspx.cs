using System;
using System.Web.UI;

public partial class gianhang_mo_cong_cu_quan_tri : Page
{
    private RootAccount_cl.RootAccountInfo _info;

    protected void Page_Load(object sender, EventArgs e)
    {
        _info = GianHangContext_cl.EnsureCurrentAccess(this);
        if (_info == null)
            return;

        if (!IsPostBack)
            BindState();
    }

    protected void btn_request_access_Click(object sender, EventArgs e)
    {
        if (_info == null || string.IsNullOrWhiteSpace(_info.AccountKey))
        {
            Helper_Tabler_cl.ShowToast(this, "Không xác định được tài khoản hiện tại.", "warning");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            CoreSchemaMigration_cl.EnsureSchemaSafe(db);
            taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, _info.AccountKey);
            if (account == null || !SpaceAccess_cl.CanAccessHome(db, account))
            {
                Helper_Tabler_cl.ShowToast(this, "Tài khoản hiện tại không hợp lệ để gửi yêu cầu mở khu quản trị.", "warning");
                return;
            }

            string message;
            bool ok = CoreSpaceRequest_cl.TryCreateRequest(
                db,
                _info.AccountKey,
                ModuleSpace_cl.GianHangAdmin,
                "gianhang_account_portal",
                _info.AccountKey,
                null,
                out message);

            Helper_Tabler_cl.ShowToast(this, TranslateRequestMessage(message), ok ? "success" : "warning");
        }

        BindState();
    }

    private void BindState()
    {
        if (_info == null)
            return;

        lit_account_key.Text = Server.HtmlEncode(_info.AccountKey);

        using (dbDataContext db = new dbDataContext())
        {
            CoreSchemaMigration_cl.EnsureSchemaSafe(db);
            taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, _info.AccountKey);
            GianHangStorefront_cl.StorefrontSummary summary = GianHangStorefront_cl.BuildSummary(db, account, Request.Url);
            lit_store_name.Text = Server.HtmlEncode(summary.StorefrontName);

            bool canAccess = account != null && SpaceAccess_cl.CanAccessGianHangAdmin(db, account);
            SpaceAccess_cl.SpaceAccessRow access = SpaceAccess_cl.GetAccessRow(db, _info.AccountKey, ModuleSpace_cl.GianHangAdmin);
            CoreSpaceRequest_cl.SpaceRequestInfo request = CoreSpaceRequest_cl.GetLatestRequest(db, _info.AccountKey, ModuleSpace_cl.GianHangAdmin);

            string accessStatus = access == null ? "" : (access.AccessStatus ?? "");
            lit_access_status.Text = Server.HtmlEncode(HomeSpaceAccess_cl.GetAccessStatusText(accessStatus));
            lit_request_status.Text = Server.HtmlEncode(HomeSpaceAccess_cl.GetRequestStatusText(request == null ? "" : request.RequestStatus));
            statusBadge.Attributes["class"] = BuildStatusCss(canAccess, accessStatus);

            lnk_open_admin.Visible = canAccess;
            lnk_open_admin.NavigateUrl = "/gianhang/admin";

            bool canRequest = !canAccess
                && (request == null || !string.Equals(request.RequestStatus, CoreSpaceRequest_cl.StatusPending, StringComparison.OrdinalIgnoreCase));
            btn_request_access.Visible = canRequest;

            ph_request_time.Visible = request != null && request.RequestedAt.HasValue;
            lit_requested_at.Text = request != null && request.RequestedAt.HasValue
                ? request.RequestedAt.Value.ToString("dd/MM/yyyy HH:mm")
                : "";

            string reviewNote = request == null ? "" : (request.ReviewNote ?? "").Trim();
            ph_review_note.Visible = !string.IsNullOrWhiteSpace(reviewNote);
            lit_review_note.Text = Server.HtmlEncode(reviewNote);

            string lockReason = access == null ? "" : (access.LockedReason ?? "").Trim();
            ph_lock_reason.Visible = !string.IsNullOrWhiteSpace(lockReason);
            lit_lock_reason.Text = Server.HtmlEncode(lockReason);
        }
    }

    private static string BuildStatusCss(bool canAccess, string accessStatus)
    {
        if (canAccess)
            return "gh-admin-open-status gh-admin-open-status--ok";

        string normalized = (accessStatus ?? "").Trim().ToLowerInvariant();
        if (normalized == SpaceAccess_cl.StatusBlocked || normalized == SpaceAccess_cl.StatusRevoked)
            return "gh-admin-open-status gh-admin-open-status--danger";

        return "gh-admin-open-status gh-admin-open-status--warn";
    }

    private static string TranslateRequestMessage(string raw)
    {
        string message = (raw ?? "").Trim();
        if (message == "")
            return "Không thể gửi yêu cầu. Vui lòng thử lại.";

        string lowered = message.ToLowerInvariant();
        if (lowered.Contains("already has access"))
            return "Tài khoản này đã có quyền truy cập khu quản trị.";
        if (lowered.Contains("already a pending request"))
            return "Đã có yêu cầu mở khu quản trị đang chờ duyệt.";
        if (lowered.Contains("created successfully"))
            return "Đã gửi yêu cầu mở quyền khu quản trị. Vui lòng chờ admin duyệt.";

        return message;
    }
}
