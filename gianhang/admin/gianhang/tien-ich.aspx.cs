using System;

public partial class gianhang_admin_gianhang_tien_ich_default : System.Web.UI.Page
{
    private readonly dbDataContext db = new dbDataContext();

    public string WorkspaceDisplayName = "Không gian /gianhang";
    public string HubUrl = "/gianhang/admin/gianhang/default.aspx";
    public string ConfigUrl = "/gianhang/admin/gianhang/tien-ich-co-cau.aspx";
    public string DrawUrl = "/gianhang/admin/gianhang/tien-ich-quay-so.aspx";
    public string PublicConfigUrl = "/gianhang/tien-ich/co-cau.aspx";
    public string PublicDrawUrl = "/gianhang/tien-ich/quayso.aspx";
    public string CurrentCoCauLabel = "Đang tắt";
    public string CurrentCoCauNote = "Quay số sẽ chạy ngẫu nhiên hoàn toàn nếu không có cấu hình ưu tiên.";

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        string ownerAccountKey = (access.OwnerAccountKey ?? string.Empty).Trim().ToLowerInvariant();
        if (ownerAccountKey == string.Empty)
            return;

        HubUrl = GianHangRoutes_cl.BuildAdminWorkspaceHubUrl();
        ConfigUrl = GianHangRoutes_cl.BuildAdminWorkspaceUtilityConfigUrl();
        DrawUrl = GianHangRoutes_cl.BuildAdminWorkspaceUtilityDrawUrl();
        PublicConfigUrl = GianHangRoutes_cl.BuildUtilityConfigUrl(ownerAccountKey);
        PublicDrawUrl = GianHangRoutes_cl.BuildUtilityDrawUrl(ownerAccountKey);

        taikhoan_tb owner = RootAccount_cl.GetByAccountKey(db, ownerAccountKey);
        string display = owner == null ? string.Empty : ((owner.ten_shop ?? string.Empty).Trim());
        if (display == string.Empty)
            display = owner == null ? string.Empty : ((owner.hoten ?? string.Empty).Trim());
        if (display == string.Empty)
            display = ownerAccountKey;
        WorkspaceDisplayName = display;

        int? coCauIndex = GianHangWorkspaceUtility_cl.GetCoCauIndex(ownerAccountKey);
        if (coCauIndex.HasValue)
        {
            CurrentCoCauLabel = "Dòng " + coCauIndex.Value.ToString();
            CurrentCoCauNote = "Khi dừng quay số, workspace này đang ưu tiên chọn dòng thứ " + coCauIndex.Value.ToString() + ".";
        }
    }
}
