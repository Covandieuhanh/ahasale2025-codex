using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

public partial class admin_duyet_gian_hang_doi_tac : System.Web.UI.Page
{
    private sealed class HomeGianHangAccessView
    {
        public long RequestId { get; set; }
        public string AccountKey { get; set; }
        public string FullName { get; set; }
        public string ShopName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string PickupAddress { get; set; }
        public string RequestStatusText { get; set; }
        public string RequestedAtText { get; set; }
        public string ReviewedAtText { get; set; }
        public string ReviewNote { get; set; }
        public bool CanApprove { get; set; }
        public bool CanReject { get; set; }
        public bool CanCancel { get; set; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        DisablePageCache();
        EnsureCanAccessPage();
        LoadHomeRequests();
    }

    protected void btn_home_reload_Click(object sender, EventArgs e)
    {
        EnsureCanAccessPage();
        LoadHomeRequests();
    }

    protected void btn_home_approve_Click(object sender, EventArgs e)
    {
        EnsureCanAccessPage();

        long requestId = ParseRequestId(sender);
        if (requestId <= 0)
            return;

        string message;
        bool ok;
        using (dbDataContext db = new dbDataContext())
        {
            ok = CoreSpaceRequest_cl.ApproveRequest(db, requestId, GetAdminName(), (txt_home_admin_note.Text ?? "").Trim(), out message);
        }

        Helper_Tabler_cl.ShowToast(this, message, ok ? "success" : "warning");
        LoadHomeRequests();
    }

    protected void btn_home_reject_Click(object sender, EventArgs e)
    {
        EnsureCanAccessPage();

        long requestId = ParseRequestId(sender);
        if (requestId <= 0)
            return;

        string message;
        bool ok;
        using (dbDataContext db = new dbDataContext())
        {
            ok = CoreSpaceRequest_cl.RejectRequest(db, requestId, GetAdminName(), (txt_home_admin_note.Text ?? "").Trim(), out message);
        }

        Helper_Tabler_cl.ShowToast(this, message, ok ? "success" : "warning");
        LoadHomeRequests();
    }

    protected void btn_home_cancel_Click(object sender, EventArgs e)
    {
        EnsureCanAccessPage();

        long requestId = ParseRequestId(sender);
        if (requestId <= 0)
            return;

        string message;
        bool ok;
        using (dbDataContext db = new dbDataContext())
        {
            ok = CoreSpaceRequest_cl.CancelApprovedRequest(db, requestId, GetAdminName(), (txt_home_admin_note.Text ?? "").Trim(), out message);
        }

        Helper_Tabler_cl.ShowToast(this, message, ok ? "warning" : "warning");
        LoadHomeRequests();
    }

    string GetAdminName()
    {
        string admin = AdminRolePolicy_cl.GetCurrentAdminUser();
        if (!string.IsNullOrWhiteSpace(admin))
            return admin.Trim();

        admin = Session["admin"] as string;
        if (!string.IsNullOrWhiteSpace(admin))
            return admin.Trim();

        admin = Session["taikhoan_admin"] as string;
        if (!string.IsNullOrWhiteSpace(admin))
            return admin.Trim();

        return "";
    }

    private void LoadHomeRequests()
    {
        using (dbDataContext db = new dbDataContext())
        {
            CoreSchemaMigration_cl.EnsureSchemaSafe(db);

            List<CoreSpaceRequest_cl.SpaceRequestInfo> requests = CoreSpaceRequest_cl.LoadRequests(db, "", ModuleSpace_cl.GianHang, "")
                .Where(p => string.Equals(
                    ModuleSpace_cl.Normalize(p.SpaceCode),
                    ModuleSpace_cl.GianHang,
                    StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(p => p.Id)
                .ToList();

            List<string> accountKeys = requests
                .Select(p => (p.AccountKey ?? "").Trim().ToLowerInvariant())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(p => p)
                .ToList();

            Dictionary<string, taikhoan_tb> accountMap = db.taikhoan_tbs
                .Where(p => accountKeys.Contains(p.taikhoan))
                .ToList()
                .ToDictionary(p => (p.taikhoan ?? "").Trim().ToLowerInvariant(), p => p, StringComparer.OrdinalIgnoreCase);
            Dictionary<string, GianHangOnboarding_cl.AdminShopInfo> shopInfoMap = GianHangOnboarding_cl.LoadAdminSummaryMap(db, accountKeys);

            List<HomeGianHangAccessView> rows = new List<HomeGianHangAccessView>();
            for (int i = 0; i < requests.Count; i++)
            {
                CoreSpaceRequest_cl.SpaceRequestInfo request = requests[i];
                string accountKey = (request.AccountKey ?? "").Trim().ToLowerInvariant();
                taikhoan_tb account = accountMap.ContainsKey(accountKey) ? accountMap[accountKey] : null;
                GianHangOnboarding_cl.AdminShopInfo shopInfo = shopInfoMap.ContainsKey(accountKey) ? shopInfoMap[accountKey] : null;

                string requestStatus = (request.RequestStatus ?? "").Trim().ToLowerInvariant();

                rows.Add(new HomeGianHangAccessView
                {
                    RequestId = request.Id,
                    AccountKey = accountKey,
                    FullName = account == null
                        ? ""
                        : (!string.IsNullOrWhiteSpace(account.hoten) ? account.hoten.Trim() : account.taikhoan),
                    ShopName = shopInfo == null ? "" : (shopInfo.ShopName ?? ""),
                    ContactPhone = shopInfo == null ? "" : (shopInfo.ContactPhone ?? ""),
                    ContactEmail = shopInfo == null ? "" : (shopInfo.ContactEmail ?? ""),
                    PickupAddress = shopInfo == null ? "" : (shopInfo.PickupAddress ?? ""),
                    RequestStatusText = GetHomeRequestStatusText(requestStatus),
                    RequestedAtText = request.RequestedAt.HasValue ? request.RequestedAt.Value.ToString("dd/MM/yyyy HH:mm") : "--",
                    ReviewedAtText = request.ReviewedAt.HasValue ? request.ReviewedAt.Value.ToString("dd/MM/yyyy HH:mm") : "--",
                    ReviewNote = (request.ReviewNote ?? "").Trim(),
                    CanApprove = requestStatus == CoreSpaceRequest_cl.StatusPending,
                    CanReject = requestStatus == CoreSpaceRequest_cl.StatusPending,
                    CanCancel = requestStatus == CoreSpaceRequest_cl.StatusApproved
                });
            }

            List<HomeGianHangAccessView> finalRows = rows
                .OrderBy(p => p.CanApprove ? 0 : (p.CanCancel ? 1 : 2))
                .ThenByDescending(p => p.RequestId)
                .ToList();

            bool hasRows = finalRows.Count > 0;
            ph_home_empty.Visible = !hasRows;
            rp_home_requests.Visible = hasRows;

            if (hasRows)
            {
                rp_home_requests.DataSource = finalRows;
                rp_home_requests.DataBind();
            }
            else
            {
                rp_home_requests.DataSource = null;
                rp_home_requests.DataBind();
            }
        }
    }

    private void EnsureCanAccessPage()
    {
        AdminAccessGuard_cl.RequireFeatureAccess("home_gianhang_space", "/admin/default.aspx?mspace=gianhang");
    }

    private static string GetHomeRequestStatusText(string status)
    {
        switch ((status ?? "").Trim().ToLowerInvariant())
        {
            case CoreSpaceRequest_cl.StatusPending: return "Chờ duyệt";
            case CoreSpaceRequest_cl.StatusApproved: return "Đã duyệt";
            case CoreSpaceRequest_cl.StatusRejected: return "Từ chối";
            case CoreSpaceRequest_cl.StatusCancelled: return "Đã hủy";
            default: return "Chưa có yêu cầu";
        }
    }

    private static long ParseRequestId(object sender)
    {
        LinkButton button = sender as LinkButton;
        long requestId;
        return button != null && long.TryParse(button.CommandArgument, out requestId) ? requestId : 0L;
    }

    private void DisablePageCache()
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();
        Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
        Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
    }
}
