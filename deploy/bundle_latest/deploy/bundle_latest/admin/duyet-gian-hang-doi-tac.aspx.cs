using System;
using System.Linq;

public partial class admin_duyet_gian_hang_doi_tac : System.Web.UI.Page
{
    // Quy ước trạng thái:
    // 0 = Chờ duyệt
    // 1 = Đã duyệt
    // 2 = Từ chối
    // 3 = Hủy duyệt (đã từng duyệt rồi, giờ bị admin hủy)
    // Lưu ý: "hủy duyệt từ đầu" (tức record đang CHỜ) => dùng TỪ CHỐI (2),
    // và user muốn đăng ký lại thì phải tạo YÊU CẦU MỚI (record mới), không reset về 0.

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            check_login_cl.check_login_admin("none", "none");
            LoadDanhSach();
        }
    }

    void LoadDanhSach()
    {
        using (dbDataContext db = new dbDataContext())
        {
            ShopStatus_cl.EnsureSchemaSafe(db);
            var list = db.DangKy_GianHangDoiTac_tbs
                .OrderByDescending(x => x.NgayTao)
                .Select(x => new
                {
                    ID = x.ID, // nếu DBML map là "id" thì đổi lại: ID = x.id
                    TaiKhoan = x.taikhoan,
                    x.NgayTao,
                    x.TrangThai,
                    x.GhiChuAdmin,
                    TrangThaiText =
                        x.TrangThai == 0 ? "Chờ duyệt" :
                        x.TrangThai == 1 ? "Đã duyệt" :
                        x.TrangThai == 2 ? "Từ chối" :
                        x.TrangThai == 3 ? "Hủy duyệt" : "Không xác định"
                })
                .ToList();

            rp_dangky.DataSource = list;
            rp_dangky.DataBind();
        }
    }

    string GetAdminName()
    {
        string admin = Session["admin"] as string;

        if (string.IsNullOrEmpty(admin))
            admin = Session["taikhoan_admin"] as string;

        return admin ?? "";
    }

    // DUYỆT (từ CHỜ -> ĐÃ DUYỆT)
    protected void btn_duyet_Click(object sender, EventArgs e)
    {
        int id = int.Parse(((System.Web.UI.WebControls.LinkButton)sender).CommandArgument);

        using (dbDataContext db = new dbDataContext())
        {
            ShopStatus_cl.EnsureSchemaSafe(db);
            var dk = db.DangKy_GianHangDoiTac_tbs
                .FirstOrDefault(x => x.ID == id && x.TrangThai == 0); // nếu là x.id thì đổi lại
            if (dk == null) return;

            // Chỉ cho phép 1 bản ghi ĐÃ DUYỆT cho 1 tài khoản tại cùng thời điểm
            bool daCoDuyet = db.DangKy_GianHangDoiTac_tbs
                .Any(x => x.taikhoan == dk.taikhoan && x.TrangThai == 1);

            if (daCoDuyet)
            {
                // Không duyệt thêm; chuyển record hiện tại sang TỪ CHỐI
                dk.TrangThai = 2;
                dk.GhiChuAdmin = string.IsNullOrEmpty(dk.GhiChuAdmin)
                    ? "Từ chối vì tài khoản đã có đăng ký được duyệt trước đó."
                    : dk.GhiChuAdmin;

                dk.NgayDuyet = AhaTime_cl.Now;
                dk.AdminDuyet = GetAdminName();

                db.SubmitChanges();

                Helper_Tabler_cl.ShowToast(this, "Tài khoản đã có đăng ký được duyệt trước đó. Đã chuyển yêu cầu này sang Từ chối.", "warning");
                LoadDanhSach();
                return;
            }

            dk.TrangThai = 1; // duyệt
            dk.NgayDuyet = AhaTime_cl.Now;
            dk.AdminDuyet = GetAdminName();

            var acc = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == dk.taikhoan);
            if (acc != null)
            {
                acc.TrangThai_Shop = ShopStatus_cl.StatusApproved;
                if (acc.block == true && PortalScope_cl.CanLoginShop(acc.taikhoan, acc.phanloai, acc.permission))
                    acc.block = false;
                acc.phanloai = "Gian hàng đối tác";
                acc.permission = PortalScope_cl.NormalizePermissionWithScope(acc.permission, PortalScope_cl.ScopeShop);
                ShopSlug_cl.EnsureSlugForShop(db, acc);
            }

            db.SubmitChanges();
        }

        Helper_Tabler_cl.ShowToast(this, "Đã duyệt đăng ký gian hàng đối tác.", "success");
        LoadDanhSach();
    }

    // TỪ CHỐI (từ CHỜ -> TỪ CHỐI). Sau đó muốn đăng ký lại thì phải tạo YÊU CẦU MỚI.
    protected void btn_tuchoi_Click(object sender, EventArgs e)
    {
        int id = int.Parse(((System.Web.UI.WebControls.LinkButton)sender).CommandArgument);

        using (dbDataContext db = new dbDataContext())
        {
            ShopStatus_cl.EnsureSchemaSafe(db);
            var dk = db.DangKy_GianHangDoiTac_tbs
                .FirstOrDefault(x => x.ID == id && x.TrangThai == 0); // nếu là x.id thì đổi lại
            if (dk == null) return;

            dk.TrangThai = 2; // từ chối
            dk.NgayDuyet = AhaTime_cl.Now;
            dk.AdminDuyet = GetAdminName();

            var acc = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == dk.taikhoan);
            if (acc != null && PortalScope_cl.ResolveScope(acc.taikhoan, acc.phanloai, acc.permission) == PortalScope_cl.ScopeShop)
            {
                acc.TrangThai_Shop = ShopStatus_cl.StatusRejected;
                acc.phanloai = "Gian hàng đối tác";
                acc.permission = PortalScope_cl.NormalizePermissionWithScope(acc.permission, PortalScope_cl.ScopeShop);
            }

            db.SubmitChanges();
        }

        Helper_Tabler_cl.ShowToast(this, "Đã từ chối đăng ký. Muốn đăng ký lại phải tạo yêu cầu mới.", "warning");
        LoadDanhSach();
    }

    // HỦY DUYỆT (từ ĐÃ DUYỆT -> HỦY DUYỆT). Không reset về CHỜ.
    protected void btn_huyduyet_Click(object sender, EventArgs e)
    {
        int id = int.Parse(((System.Web.UI.WebControls.LinkButton)sender).CommandArgument);

        using (dbDataContext db = new dbDataContext())
        {
            ShopStatus_cl.EnsureSchemaSafe(db);
            // Chỉ hủy khi record đang ở trạng thái ĐÃ DUYỆT
            var dk = db.DangKy_GianHangDoiTac_tbs
                .FirstOrDefault(x => x.ID == id && x.TrangThai == 1); // nếu là x.id thì đổi lại
            if (dk == null) return;

            dk.TrangThai = 3; // hủy duyệt
            dk.NgayDuyet = AhaTime_cl.Now;   // lưu thời điểm thay đổi trạng thái (hoặc bạn có thể dùng cột khác nếu có)
            dk.AdminDuyet = GetAdminName();

            if (string.IsNullOrEmpty(dk.GhiChuAdmin))
                dk.GhiChuAdmin = "Admin hủy duyệt.";

            var acc = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == dk.taikhoan);
            if (acc != null)
            {
                acc.TrangThai_Shop = ShopStatus_cl.StatusRevoked;
                acc.phanloai = "Gian hàng đối tác";
                acc.permission = PortalScope_cl.NormalizePermissionWithScope(acc.permission, PortalScope_cl.ScopeShop);
            }

            // Ẩn toàn bộ tin của gian hàng trên toàn hệ thống
            var posts = db.BaiViet_tbs
                .Where(p => p.nguoitao == dk.taikhoan && p.bin == false)
                .ToList();
            foreach (var p in posts)
            {
                p.bin = true;
            }

            db.SubmitChanges();
        }

        Helper_Tabler_cl.ShowToast(this, "Đã hủy duyệt. Nếu muốn tham gia lại phải tạo yêu cầu đăng ký mới.", "warning");
        LoadDanhSach();
    }
}
