using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

public partial class admin_duyet_shop_doi_tac : System.Web.UI.Page
{
    private sealed class ShopAccessRequestView
    {
        public long RequestId { get; set; }
        public string AccountKey { get; set; }
        public string FullName { get; set; }
        public string ShopName { get; set; }
        public int RequestedCommissionPercent { get; set; }
        public string PolicyCommissionText { get; set; }
        public string RequestStatusText { get; set; }
        public string RequestedAtText { get; set; }
        public string ReviewedAtText { get; set; }
        public string ReviewNote { get; set; }
        public bool CanApprove { get; set; }
        public bool CanReject { get; set; }
        public bool CanCancel { get; set; }
    }

    private sealed class PolicySnapshot
    {
        public string AccountKey { get; set; }
        public int? CommissionPercent { get; set; }
        public string PolicyStatus { get; set; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        DisablePageCache();
        EnsureCanAccessPage();
        LoadShopRequests();
    }

    protected void btn_shop_reload_Click(object sender, EventArgs e)
    {
        EnsureCanAccessPage();
        LoadShopRequests();
    }

    protected void btn_shop_approve_Click(object sender, EventArgs e)
    {
        EnsureCanAccessPage();

        long requestId = ParseRequestId(sender);
        if (requestId <= 0)
            return;

        string note = (txt_shop_admin_note.Text ?? "").Trim();
        string adminName = GetAdminName();

        string message;
        bool ok;
        using (dbDataContext db = new dbDataContext())
        {
            CoreSchemaMigration_cl.EnsureSchemaSafe(db);
            CoreSpaceRequest_cl.SpaceRequestInfo request = CoreSpaceRequest_cl.GetRequestById(db, requestId);
            if (request == null)
            {
                Helper_Tabler_cl.ShowToast(this, "Không tìm thấy yêu cầu cần duyệt.", "warning");
                LoadShopRequests();
                return;
            }

            int requestedPercent = ShopPolicy_cl.ExtractRequestedCommissionPercent(request.RequestMetaJson, 0);
            ok = CoreSpaceRequest_cl.ApproveRequest(db, requestId, adminName, note, out message);
            if (ok)
            {
                ApplyShopStatusMarker(db, request.AccountKey, ShopStatus_cl.StatusApproved);
                message = "Đã duyệt yêu cầu mở /shop và áp dụng % chiết khấu mặc định: " + requestedPercent + "%.";
            }
        }

        Helper_Tabler_cl.ShowToast(this, message, ok ? "success" : "warning");
        LoadShopRequests();
    }

    protected void btn_shop_reject_Click(object sender, EventArgs e)
    {
        EnsureCanAccessPage();

        long requestId = ParseRequestId(sender);
        if (requestId <= 0)
            return;

        string note = (txt_shop_admin_note.Text ?? "").Trim();
        string adminName = GetAdminName();

        string message;
        bool ok;
        using (dbDataContext db = new dbDataContext())
        {
            ok = CoreSpaceRequest_cl.RejectRequest(db, requestId, adminName, note, out message);
            if (ok)
            {
                CoreSpaceRequest_cl.SpaceRequestInfo request = CoreSpaceRequest_cl.GetRequestById(db, requestId);
                if (request != null)
                    ApplyShopStatusMarker(db, request.AccountKey, ShopStatus_cl.StatusRejected);
            }
        }

        Helper_Tabler_cl.ShowToast(this, message, ok ? "warning" : "warning");
        LoadShopRequests();
    }

    protected void btn_shop_cancel_Click(object sender, EventArgs e)
    {
        EnsureCanAccessPage();

        long requestId = ParseRequestId(sender);
        if (requestId <= 0)
            return;

        string note = (txt_shop_admin_note.Text ?? "").Trim();
        string adminName = GetAdminName();

        string message;
        bool ok;
        using (dbDataContext db = new dbDataContext())
        {
            CoreSpaceRequest_cl.SpaceRequestInfo request = CoreSpaceRequest_cl.GetRequestById(db, requestId);
            ok = CoreSpaceRequest_cl.CancelApprovedRequest(db, requestId, adminName, note, out message);
            if (ok && request != null)
            {
                ApplyShopStatusMarker(db, request.AccountKey, ShopStatus_cl.StatusRevoked);
                message = "Đã hủy duyệt /shop và khóa lại chính sách % chiết khấu mặc định của shop.";
            }
        }

        Helper_Tabler_cl.ShowToast(this, message, ok ? "warning" : "warning");
        LoadShopRequests();
    }

    private void LoadShopRequests()
    {
        using (dbDataContext db = new dbDataContext())
        {
            CoreSchemaMigration_cl.EnsureSchemaSafe(db);

            List<CoreSpaceRequest_cl.SpaceRequestInfo> requests = CoreSpaceRequest_cl.LoadRequests(db, "", ModuleSpace_cl.Shop, "")
                .Where(p => string.Equals(ModuleSpace_cl.Normalize(p.SpaceCode), ModuleSpace_cl.Shop, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(p => p.Id)
                .ToList();

            List<string> accountKeys = requests
                .Select(p => NormalizeKey(p.AccountKey))
                .Where(p => p != "")
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            Dictionary<string, taikhoan_tb> accountMap = db.taikhoan_tbs
                .Where(p => accountKeys.Contains(p.taikhoan))
                .ToList()
                .ToDictionary(p => NormalizeKey(p.taikhoan), p => p, StringComparer.OrdinalIgnoreCase);

            Dictionary<string, PolicySnapshot> policyMap = db.ExecuteQuery<PolicySnapshot>(
                    "SELECT AccountKey, CommissionPercent, PolicyStatus FROM dbo.CoreShopPolicy_tb")
                .ToList()
                .Where(p => string.Equals((p.PolicyStatus ?? "").Trim(), ShopPolicy_cl.PolicyStatusActive, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(p => NormalizeKey(p.AccountKey), p => p, StringComparer.OrdinalIgnoreCase);

            List<ShopAccessRequestView> rows = new List<ShopAccessRequestView>();
            for (int i = 0; i < requests.Count; i++)
            {
                CoreSpaceRequest_cl.SpaceRequestInfo request = requests[i];
                string accountKey = NormalizeKey(request.AccountKey);
                taikhoan_tb account = accountMap.ContainsKey(accountKey) ? accountMap[accountKey] : null;
                PolicySnapshot policy = policyMap.ContainsKey(accountKey) ? policyMap[accountKey] : null;
                string requestStatus = (request.RequestStatus ?? "").Trim().ToLowerInvariant();

                int requestedPercent = ShopPolicy_cl.ExtractRequestedCommissionPercent(request.RequestMetaJson, 0);
                string policyText = policy != null && policy.CommissionPercent.HasValue
                    ? ShopPolicy_cl.ClampCommissionPercent(policy.CommissionPercent.Value).ToString() + "%"
                    : "--";

                rows.Add(new ShopAccessRequestView
                {
                    RequestId = request.Id,
                    AccountKey = accountKey,
                    FullName = account == null ? "" : ((account.hoten ?? "").Trim() == "" ? account.taikhoan : account.hoten.Trim()),
                    ShopName = ResolveShopName(account, accountKey),
                    RequestedCommissionPercent = requestedPercent,
                    PolicyCommissionText = policyText,
                    RequestStatusText = GetRequestStatusText(requestStatus),
                    RequestedAtText = request.RequestedAt.HasValue ? request.RequestedAt.Value.ToString("dd/MM/yyyy HH:mm") : "--",
                    ReviewedAtText = request.ReviewedAt.HasValue ? request.ReviewedAt.Value.ToString("dd/MM/yyyy HH:mm") : "--",
                    ReviewNote = (request.ReviewNote ?? "").Trim(),
                    CanApprove = requestStatus == CoreSpaceRequest_cl.StatusPending,
                    CanReject = requestStatus == CoreSpaceRequest_cl.StatusPending,
                    CanCancel = requestStatus == CoreSpaceRequest_cl.StatusApproved
                });
            }

            List<ShopAccessRequestView> finalRows = rows
                .OrderBy(p => p.CanApprove ? 0 : (p.CanCancel ? 1 : 2))
                .ThenByDescending(p => p.RequestId)
                .ToList();

            bool hasRows = finalRows.Count > 0;
            ph_shop_empty.Visible = !hasRows;
            rp_shop_requests.Visible = hasRows;

            if (hasRows)
            {
                rp_shop_requests.DataSource = finalRows;
                rp_shop_requests.DataBind();
            }
            else
            {
                rp_shop_requests.DataSource = null;
                rp_shop_requests.DataBind();
            }
        }
    }

    private static string ResolveShopName(taikhoan_tb account, string fallbackAccount)
    {
        if (account == null)
            return fallbackAccount;

        string shopName = (account.ten_shop ?? "").Trim();
        if (shopName != "")
            return shopName;

        string fullName = (account.hoten ?? "").Trim();
        if (fullName != "")
            return fullName;

        return (account.taikhoan ?? fallbackAccount ?? "").Trim();
    }

    private static string NormalizeKey(string raw)
    {
        return (raw ?? "").Trim().ToLowerInvariant();
    }

    private void ApplyShopStatusMarker(dbDataContext db, string accountKey, int status)
    {
        if (db == null)
            return;

        ShopStatus_cl.EnsureSchemaSafe(db);

        string key = NormalizeKey(accountKey);
        if (key == "")
            return;

        taikhoan_tb account = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == key);
        if (account == null)
            return;

        account.TrangThai_Shop = (byte)status;
        if (string.IsNullOrWhiteSpace(account.ten_shop))
            account.ten_shop = string.IsNullOrWhiteSpace(account.hoten) ? account.taikhoan : account.hoten;
        if (string.IsNullOrWhiteSpace(account.sdt_shop))
            account.sdt_shop = account.dienthoai;
        if (string.IsNullOrWhiteSpace(account.email_shop))
            account.email_shop = account.email;

        ShopSlug_cl.EnsureSlugForShop(db, account);
        db.SubmitChanges();
    }

    private void EnsureCanAccessPage()
    {
        AdminAccessGuard_cl.RequireFeatureAccess("shop_partner", "/admin/default.aspx?mspace=gianhang");
    }

    private string GetAdminName()
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

    private static string GetRequestStatusText(string status)
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
