using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;

public partial class home_mo_khong_gian : Page
{
    private HomeSpaceAccess_cl.SpaceGuideInfo _spaceInfo = null;
    private string _spaceCode = "";
    private string _returnUrl = "/home/default.aspx";
    private string _accountKey = "";
    private string _workspaceHintMessage = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);

        _spaceCode = ModuleSpace_cl.Normalize(Request.QueryString["space"]);
        _spaceInfo = HomeSpaceAccess_cl.Resolve(_spaceCode);
        if (_spaceInfo == null)
        {
            Response.Redirect("/home/default.aspx", true);
            return;
        }

        _returnUrl = HomeSpaceAccess_cl.NormalizeReturnUrl(Request.QueryString["return_url"], _spaceInfo.RoutePath);
        if (string.Equals(_spaceCode, ModuleSpace_cl.GianHang, StringComparison.OrdinalIgnoreCase))
        {
            Response.Redirect("/home/dang-ky-gian-hang-doi-tac.aspx?return_url=" + Server.UrlEncode(_returnUrl), true);
            return;
        }

        _accountKey = (PortalRequest_cl.GetCurrentAccount() ?? "").Trim().ToLowerInvariant();
        if (_accountKey == "")
        {
            Response.Redirect("/dang-nhap?return_url=" + Server.UrlEncode(Request.RawUrl ?? "/home/default.aspx"), true);
            return;
        }

        if (!IsPostBack)
            BindState();
    }

    protected void btn_request_open_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            CoreSchemaMigration_cl.EnsureSchemaSafe(db);
            taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, _accountKey);
            if (account == null || !SpaceAccess_cl.CanAccessHome(db, account))
            {
                Helper_Tabler_cl.ShowToast(this, "Tài khoản Home hiện tại không hợp lệ để gửi yêu cầu mở không gian.", "warning");
                return;
            }

            string requestMeta = null;
            if (string.Equals(_spaceCode, ModuleSpace_cl.Shop, StringComparison.OrdinalIgnoreCase))
            {
                int commissionPercent;
                if (!ShopPolicy_cl.TryParseCommissionPercent(txt_shop_commission.Text, out commissionPercent))
                {
                    Helper_Tabler_cl.ShowToast(this, "Vui lòng nhập % chiết khấu cho sàn hợp lệ (0 - 100).", "warning");
                    return;
                }

                requestMeta = ShopPolicy_cl.BuildRequestMeta(commissionPercent);
            }

            string message;
            bool ok = CoreSpaceRequest_cl.TryCreateRequest(
                db,
                _accountKey,
                _spaceCode,
                "home_space_portal",
                _accountKey,
                requestMeta,
                out message);

            if (ok)
            {
                string successMessage = string.Equals(_spaceCode, ModuleSpace_cl.Shop, StringComparison.OrdinalIgnoreCase)
                    ? "Đã gửi yêu cầu mở không gian shop đối tác chiến lược. Admin sẽ duyệt và áp dụng % chiết khấu mặc định cho shop."
                    : "Đã gửi yêu cầu mở quyền. Admin có thể mở trực tiếp tại phần quản lý tài khoản Home.";
                Helper_Tabler_cl.ShowToast(this, successMessage, "success");
            }
            else
            {
                Helper_Tabler_cl.ShowToast(this, TranslateRequestMessage(message), "warning");
            }
        }

        BindState();
    }

    private void BindState()
    {
        _workspaceHintMessage = "";
        lit_title.Text = Server.HtmlEncode(_spaceInfo.Title);
        lit_summary.Text = Server.HtmlEncode(_spaceInfo.Summary);
        lit_admin_toggle_label.Text = Server.HtmlEncode(_spaceInfo.AdminToggleLabel ?? "");
        lit_admin_hint.Text = Server.HtmlEncode(_spaceInfo.AdminHint);
        lit_request_hint.Text = Server.HtmlEncode(_spaceInfo.RequestHint);
        lit_account.Text = Server.HtmlEncode(_accountKey);
        lit_route.Text = Server.HtmlEncode(_spaceInfo.RoutePath);
        lit_usage_items.Text = BuildUsageItemsHtml(_spaceInfo);

        using (dbDataContext db = new dbDataContext())
        {
            CoreSchemaMigration_cl.EnsureSchemaSafe(db);
            taikhoan_tb account = RootAccount_cl.GetByAccountKey(db, _accountKey);
            bool canAccess = SpaceAccess_cl.CanAccessSpace(db, account, _spaceCode);
            SpaceAccess_cl.SpaceAccessRow access = SpaceAccess_cl.GetAccessRow(db, _accountKey, _spaceCode);
            CoreSpaceRequest_cl.SpaceRequestInfo request = CoreSpaceRequest_cl.GetLatestRequest(db, _accountKey, _spaceCode);
            BindWorkspaceOptions(db, canAccess);

            bool isShopSpace = string.Equals(_spaceCode, ModuleSpace_cl.Shop, StringComparison.OrdinalIgnoreCase);
            ph_shop_commission.Visible = isShopSpace;
            if (isShopSpace)
            {
                int commissionPercent = 0;
                if (!ShopPolicy_cl.TryGetActivePolicyPercent(db, _accountKey, out commissionPercent))
                    commissionPercent = ShopPolicy_cl.ExtractRequestedCommissionPercent(request == null ? "" : request.RequestMetaJson, 0);
                txt_shop_commission.Text = commissionPercent.ToString();
            }

            string accessStatus = access == null ? "" : (access.AccessStatus ?? "");
            lit_access_status.Text = Server.HtmlEncode(HomeSpaceAccess_cl.GetAccessStatusText(accessStatus));
            lit_request_status.Text = Server.HtmlEncode(HomeSpaceAccess_cl.GetRequestStatusText(request == null ? "" : request.RequestStatus));

            statusBadge.Attributes["class"] = BuildStatusCss(canAccess, accessStatus);

            lnk_open_space_top.Visible = canAccess;
            lnk_open_space_top.NavigateUrl = BuildOpenSpaceUrl();
            lnk_open_space_top.Text = string.Equals(_spaceCode, ModuleSpace_cl.GianHangAdmin, StringComparison.OrdinalIgnoreCase)
                ? "Vào không gian của bạn"
                : "Mở không gian";

            bool canRequest = !canAccess
                && (request == null || !string.Equals(request.RequestStatus, CoreSpaceRequest_cl.StatusPending, StringComparison.OrdinalIgnoreCase));
            btn_request_open_top.Visible = canRequest;

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

            ph_workspace_hint.Visible = !string.IsNullOrWhiteSpace(_workspaceHintMessage);
            lit_workspace_hint.Text = Server.HtmlEncode(_workspaceHintMessage);
        }
    }

    private string BuildStatusCss(bool canAccess, string accessStatus)
    {
        if (canAccess)
            return "space-access-status space-access-status--ok";

        string normalized = (accessStatus ?? "").Trim().ToLowerInvariant();
        if (normalized == SpaceAccess_cl.StatusBlocked || normalized == SpaceAccess_cl.StatusRevoked)
            return "space-access-status space-access-status--danger";

        return "space-access-status space-access-status--warn";
    }

    private string TranslateRequestMessage(string raw)
    {
        string message = (raw ?? "").Trim();
        if (message == "")
            return "Không thể gửi yêu cầu. Vui lòng thử lại.";

        string lowered = message.ToLowerInvariant();
        if (lowered.Contains("already has access"))
            return "Tài khoản này đã được mở quyền cho không gian đang chọn.";
        if (lowered.Contains("already a pending request"))
            return "Không gian này đang có yêu cầu chờ duyệt. Bạn chưa cần gửi lại.";
        if (lowered.Contains("requires a valid commission percent"))
            return "Yêu cầu mở /shop cần nhập % chiết khấu hợp lệ trong khoảng 0 - 100.";
        return message;
    }

    private string BuildUsageItemsHtml(HomeSpaceAccess_cl.SpaceGuideInfo info)
    {
        if (info == null || info.UsageItems == null || info.UsageItems.Count == 0)
            return "<li>Quyền này sẽ mở đúng không gian tương ứng cho tài khoản Home.</li>";

        return string.Join("", info.UsageItems
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => "<li>" + Server.HtmlEncode(p.Trim()) + "</li>"));
    }

    private void BindWorkspaceOptions(dbDataContext db, bool canAccessOwnSpace)
    {
        bool isGianHangAdmin = string.Equals(_spaceCode, ModuleSpace_cl.GianHangAdmin, StringComparison.OrdinalIgnoreCase);
        if (!isGianHangAdmin)
        {
            ph_workspace_list.Visible = false;
            lit_workspace_cards.Text = "";
            _workspaceHintMessage = "";
            return;
        }

        IList<GianHangAdminWorkspace_cl.WorkspaceInfo> workspaces = GianHangAdminWorkspace_cl.GetAvailableWorkspaces(db, _accountKey);
        ph_workspace_list.Visible = workspaces.Count > 0;
        if (workspaces.Count == 0)
        {
            lit_workspace_cards.Text = "";
            _workspaceHintMessage = "";
            return;
        }

        lit_workspace_cards.Text = string.Join("", workspaces.Select(BuildWorkspaceCardHtml));
        bool hasJoinedWorkspace = workspaces.Any(p => p != null && !p.IsOwner);
        if (hasJoinedWorkspace && !canAccessOwnSpace)
            _workspaceHintMessage = "Bạn đang tham gia không gian của tài khoản Home khác. Bạn vẫn có thể gửi yêu cầu mở không gian của riêng mình. Khi được duyệt, không gian do chính bạn làm chủ sẽ xuất hiện thêm trong danh sách bên trên.";
        else if (hasJoinedWorkspace)
            _workspaceHintMessage = "Tài khoản này hiện có cả không gian của chính bạn và các không gian đang tham gia. Hãy chọn đúng nơi làm việc trước khi truy cập.";
        else if (canAccessOwnSpace)
            _workspaceHintMessage = "Hiện tại bạn mới chỉ có không gian của chính mình. Nếu sau này được thêm vào không gian của tài khoản Home khác, các không gian đó cũng sẽ được liệt kê tại đây.";
        else
            _workspaceHintMessage = "";
    }

    private string BuildWorkspaceCardHtml(GianHangAdminWorkspace_cl.WorkspaceInfo workspace)
    {
        if (workspace == null)
            return "";

        string badgeCss = workspace.IsOwner ? "space-workspace-card__badge" : "space-workspace-card__badge space-workspace-card__badge--member";
        string badgeText = workspace.IsOwner ? "Không gian của bạn" : "Không gian đang tham gia";
        string roleText = (workspace.RoleLabel ?? "").Trim();
        string ownerText = (workspace.OwnerDisplayName ?? workspace.OwnerAccountKey ?? "").Trim();
        string ownerPhone = (workspace.OwnerPhone ?? "").Trim();
        string legacyText = (workspace.LegacyDisplayName ?? workspace.LegacyUser ?? "").Trim();
        string entryUrl = "/gianhang/admin?space_owner=" + Server.UrlEncode(workspace.OwnerAccountKey ?? "");

        string ownerLine = ownerText;
        if (ownerPhone != "")
            ownerLine += " (" + ownerPhone + ")";

        return "<div class='space-workspace-card'>"
            + "<div class='space-workspace-card__badges'>"
            + "<span class='" + badgeCss + "'>" + Server.HtmlEncode(badgeText) + "</span>"
            + "<span class='space-workspace-card__badge space-workspace-card__badge--member'>" + Server.HtmlEncode(roleText) + "</span>"
            + "</div>"
            + "<h4 class='space-workspace-card__title'>" + Server.HtmlEncode(ownerLine) + "</h4>"
            + "<div class='space-workspace-card__meta'>"
            + "<div><strong>Không gian:</strong> /gianhang/admin</div>"
            + "<div><strong>Tài khoản vận hành:</strong> " + Server.HtmlEncode(legacyText) + "</div>"
            + "<div><strong>Chủ không gian:</strong> " + Server.HtmlEncode(workspace.OwnerAccountKey ?? "") + "</div>"
            + "</div>"
            + "<div class='space-workspace-card__actions'>"
            + "<a class='btn btn-success' href='" + entryUrl + "'>" + (workspace.IsOwner ? "Vào không gian của bạn" : "Vào không gian này") + "</a>"
            + "</div>"
            + "</div>";
    }

    private string BuildOpenSpaceUrl()
    {
        if (!string.Equals(_spaceCode, ModuleSpace_cl.GianHangAdmin, StringComparison.OrdinalIgnoreCase))
            return _returnUrl;

        return "/gianhang/admin?space_owner=" + Server.UrlEncode(_accountKey);
    }
}
