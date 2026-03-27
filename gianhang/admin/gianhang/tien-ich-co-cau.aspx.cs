using System;

public partial class gianhang_admin_gianhang_tien_ich_co_cau : System.Web.UI.Page
{
    private readonly dbDataContext db = new dbDataContext();

    private string ownerAccountKey = string.Empty;

    public string WorkspaceDisplayName = "Không gian /gianhang";
    public string HubUrl = "/gianhang/admin/gianhang/tien-ich.aspx";
    public string DrawUrl = "/gianhang/admin/gianhang/tien-ich-quay-so.aspx";
    public string PublicConfigUrl = "/gianhang/tien-ich/co-cau.aspx";
    public string CurrentCoCauLabel = "Đang tắt";
    public string CurrentCoCauNote = "Quay số đang chạy theo chế độ ngẫu nhiên hoàn toàn.";
    public string StatusMessage = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        ownerAccountKey = (access.OwnerAccountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (ownerAccountKey == string.Empty)
            return;

        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceUtilityUrl();
        DrawUrl = GianHangRoutes_cl.BuildAdminWorkspaceUtilityDrawUrl();
        PublicConfigUrl = GianHangRoutes_cl.BuildUtilityConfigUrl(ownerAccountKey);

        taikhoan_tb owner = RootAccount_cl.GetByAccountKey(db, ownerAccountKey);
        string display = owner == null ? string.Empty : ((owner.ten_shop ?? string.Empty).Trim());
        if (display == string.Empty)
            display = owner == null ? string.Empty : ((owner.hoten ?? string.Empty).Trim());
        if (display == string.Empty)
            display = ownerAccountKey;
        WorkspaceDisplayName = display;

        if (!IsPostBack)
        {
            int? current = GianHangWorkspaceUtility_cl.GetCoCauIndex(ownerAccountKey);
            txtCoCau.Text = current.HasValue ? current.Value.ToString() : string.Empty;
        }

        BindCurrentState();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        int index;
        if (!int.TryParse((txtCoCau.Text ?? string.Empty).Trim(), out index) || index <= 0)
        {
            StatusMessage = "Vui lòng nhập một số nguyên dương hợp lệ để cấu hình cơ cấu.";
            phStatus.Visible = true;
            return;
        }

        GianHangWorkspaceUtility_cl.SetCoCauIndex(ownerAccountKey, index);
        txtCoCau.Text = index.ToString();
        StatusMessage = "Đã lưu cơ cấu cho workspace này ở dòng thứ " + index.ToString() + ".";
        phStatus.Visible = true;
        BindCurrentState();
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        GianHangWorkspaceUtility_cl.ClearCoCauIndex(ownerAccountKey);
        txtCoCau.Text = string.Empty;
        StatusMessage = "Đã tắt cơ cấu. Công cụ quay số sẽ trở về chế độ ngẫu nhiên hoàn toàn.";
        phStatus.Visible = true;
        BindCurrentState();
    }

    private void BindCurrentState()
    {
        int? current = GianHangWorkspaceUtility_cl.GetCoCauIndex(ownerAccountKey);
        if (current.HasValue)
        {
            CurrentCoCauLabel = "Dòng " + current.Value.ToString();
            CurrentCoCauNote = "Khi dừng quay số, workspace sẽ ưu tiên chọn dòng thứ " + current.Value.ToString() + ".";
        }
        else
        {
            CurrentCoCauLabel = "Đang tắt";
            CurrentCoCauNote = "Quay số đang chạy theo chế độ ngẫu nhiên hoàn toàn.";
        }
    }
}
