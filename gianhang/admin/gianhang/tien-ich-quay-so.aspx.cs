using System;

public partial class gianhang_admin_gianhang_tien_ich_quay_so : System.Web.UI.Page
{
    private readonly dbDataContext db = new dbDataContext();

    public string WorkspaceDisplayName = "Không gian /gianhang";
    public string HubUrl = "/gianhang/admin/gianhang/tien-ich.aspx";
    public string ConfigUrl = "/gianhang/admin/gianhang/tien-ich-co-cau.aspx";
    public string PublicDrawUrl = "/gianhang/tien-ich/quayso.aspx";
    public string CurrentCoCauLabel = "Đang tắt";
    public string CurrentCoCauNote = "Workspace này hiện không có cấu hình cơ cấu nên kết quả sẽ ngẫu nhiên hoàn toàn.";
    public int CoCauIndex = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (ownerAccountKey == string.Empty)
            return;

        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceUtilityUrl();
        ConfigUrl = GianHangRoutes_cl.BuildAdminWorkspaceUtilityConfigUrl();
        PublicDrawUrl = GianHangRoutes_cl.BuildUtilityDrawUrl(ownerAccountKey);

        taikhoan_tb owner = RootAccount_cl.GetByAccountKey(db, ownerAccountKey);
        string display = owner == null ? string.Empty : ((owner.ten_shop ?? string.Empty).Trim());
        if (display == string.Empty)
            display = owner == null ? string.Empty : ((owner.hoten ?? string.Empty).Trim());
        if (display == string.Empty)
            display = ownerAccountKey;
        WorkspaceDisplayName = display;

        int? current = GianHangWorkspaceUtility_cl.GetCoCauIndex(ownerAccountKey);
        if (current.HasValue)
        {
            CoCauIndex = current.Value;
            CurrentCoCauLabel = "Dòng " + current.Value.ToString();
            CurrentCoCauNote = "Workspace đang ưu tiên dòng thứ " + current.Value.ToString() + " khi dừng quay số.";
        }
    }
}
