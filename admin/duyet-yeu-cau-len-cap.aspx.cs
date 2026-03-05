using System;
using System.Linq;
using System.Web.UI.WebControls;

public partial class admin_duyet_yeu_cau_len_cap : System.Web.UI.Page
{
    private string GetCurrentAdminUser()
    {
        string tkEnc = Session["taikhoan"] as string;
        if (string.IsNullOrEmpty(tkEnc)) return "";

        try { return mahoa_cl.giaima_Bcorn(tkEnc); }
        catch { return tkEnc; }
    }

    private bool TryGetRequestIdFromSender(object sender, out int id)
    {
        id = 0;
        IButtonControl btn = sender as IButtonControl;
        if (btn == null) return false;
        return int.TryParse(btn.CommandArgument, out id);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string allApprovalPermission = string.Join("|", new[]
            {
                PermissionProfile_cl.HoSoUuDai,
                PermissionProfile_cl.HoSoLaoDong,
                PermissionProfile_cl.HoSoGanKet
            });
            check_login_cl.check_login_admin(allApprovalPermission, allApprovalPermission);
            txt_timkiem.Text = "";
            ddl_trangthai.SelectedValue = "";
            LoadDanhSach();
        }
    }

    private bool IsRootAdmin(string tk)
    {
        return PermissionProfile_cl.IsRootAdmin(tk);
    }

    private int? GetHanhViByRequest(int capYeuCau, int giaTriYeuCau)
    {
        return HanhVi9Cap_cl.GetLoaiHanhViByCapGiaTri(capYeuCau, giaTriYeuCau);
    }

    private string GetPermissionByHanhVi(int? hanhVi)
    {
        if (!hanhVi.HasValue) return "";
        int h = hanhVi.Value;
        if (h >= 1 && h <= 3) return PermissionProfile_cl.HoSoUuDai;
        if (h >= 4 && h <= 6) return PermissionProfile_cl.HoSoLaoDong;
        if (h >= 7 && h <= 9) return PermissionProfile_cl.HoSoGanKet;
        return "";
    }

    private bool CanReviewByRequest(dbDataContext db, string tkAdmin, int capYeuCau, int giaTriYeuCau)
    {
        if (IsRootAdmin(tkAdmin)) return true;
        string permission = GetPermissionByHanhVi(GetHanhViByRequest(capYeuCau, giaTriYeuCau));
        if (permission == "") return false;
        return PermissionProfile_cl.HasPermission(db, tkAdmin, permission);
    }

    private IQueryable<YeuCau_HeThongSanPham_tb> BuildScopedQuery(dbDataContext db, string tkAdmin)
    {
        var query = db.YeuCau_HeThongSanPham_tbs.AsQueryable();
        if (IsRootAdmin(tkAdmin))
            return query;

        bool canUuDai = PermissionProfile_cl.HasPermission(db, tkAdmin, PermissionProfile_cl.HoSoUuDai);
        bool canLaoDong = PermissionProfile_cl.HasPermission(db, tkAdmin, PermissionProfile_cl.HoSoLaoDong);
        bool canGanKet = PermissionProfile_cl.HasPermission(db, tkAdmin, PermissionProfile_cl.HoSoGanKet);

        if (!canUuDai && !canLaoDong && !canGanKet)
            return query.Where(x => x.id < 0);

        query = query.Where(x =>
            (canUuDai && x.CapYeuCau == 1 && (x.GiaTriYeuCau == 15 || x.GiaTriYeuCau == 9 || x.GiaTriYeuCau == 6))
            || (canLaoDong && x.CapYeuCau == 2 && (x.GiaTriYeuCau == 10 || x.GiaTriYeuCau == 15 || x.GiaTriYeuCau == 25))
            || (canGanKet && x.CapYeuCau == 3 && (x.GiaTriYeuCau == 10 || x.GiaTriYeuCau == 6 || x.GiaTriYeuCau == 4)));

        return query;
    }

    private void LoadThongKe(IQueryable<YeuCau_HeThongSanPham_tb> query)
    {
        lb_tong.Text = query.Count().ToString("#,##0");
        lb_cho_duyet.Text = query.Count(x => x.TrangThai == 0).ToString("#,##0");
        lb_da_duyet.Text = query.Count(x => x.TrangThai == 1).ToString("#,##0");
        lb_tu_choi.Text = query.Count(x => x.TrangThai == 2).ToString("#,##0");
    }

    void LoadDanhSach()
    {
        using (dbDataContext db = new dbDataContext())
        {
            string tkAdmin = GetCurrentAdminUser();
            var query = BuildScopedQuery(db, tkAdmin);
            LoadThongKe(query);

            string keyword = txt_timkiem.Text.Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                int idSearch;
                bool isIdSearch = int.TryParse(keyword, out idSearch);

                if (isIdSearch)
                {
                    query = query.Where(x => x.id == idSearch || x.taikhoan.Contains(keyword));
                }
                else
                {
                    query = query.Where(x => x.taikhoan.Contains(keyword));
                }
            }

            int trangThai;
            if (int.TryParse(ddl_trangthai.SelectedValue, out trangThai))
            {
                query = query.Where(x => x.TrangThai == trangThai);
            }

            var listRaw = query
                .OrderByDescending(x => x.NgayTao)
                .Select(x => new
                {
                    ID = x.id,
                    TaiKhoan = x.taikhoan,
                    x.CapHienTai,
                    x.GiaTriHienTai,
                    x.CapYeuCau,
                    x.GiaTriYeuCau,
                    x.NgayTao,
                    TrangThai = x.TrangThai,
                    x.NgayDuyet,
                    x.NguoiDuyet,
                    x.GhiChuAdmin
                }).ToList();

            var list = listRaw.Select(x => new
            {
                x.ID,
                x.TaiKhoan,
                x.CapHienTai,
                x.GiaTriHienTai,
                x.CapYeuCau,
                x.GiaTriYeuCau,
                x.NgayTao,
                x.TrangThai,
                x.NgayDuyet,
                x.NguoiDuyet,
                x.GhiChuAdmin,
                CanAction = x.TrangThai == 0 && CanReviewByRequest(db, tkAdmin, x.CapYeuCau, x.GiaTriYeuCau)
            }).ToList();

            rp_yeucau.DataSource = list;
            rp_yeucau.DataBind();
            pn_empty.Visible = list.Count == 0;
        }
    }

    protected void btn_timkiem_Click(object sender, EventArgs e)
    {
        LoadDanhSach();
    }

    protected void ddl_trangthai_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadDanhSach();
    }

    protected void btn_reset_Click(object sender, EventArgs e)
    {
        txt_timkiem.Text = "";
        ddl_trangthai.SelectedValue = "";
        LoadDanhSach();
    }

    protected void btn_duyet_Click(object sender, EventArgs e)
    {
        int id;
        if (!TryGetRequestIdFromSender(sender, out id))
        {
            Helper_Tabler_cl.ShowToast(this, "Không xác định được yêu cầu cần duyệt.", "warning");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            var yc = db.YeuCau_HeThongSanPham_tbs
                .FirstOrDefault(x => x.id == id && x.TrangThai == 0);
            if (yc == null)
            {
                Helper_Tabler_cl.ShowToast(this, "Yêu cầu không còn ở trạng thái chờ duyệt.", "warning");
                LoadDanhSach();
                return;
            }
            string tkAdmin = GetCurrentAdminUser();
            if (!CanReviewByRequest(db, tkAdmin, yc.CapYeuCau, yc.GiaTriYeuCau))
            {
                Helper_Tabler_cl.ShowToast(this, "Bạn không có quyền duyệt hành vi của yêu cầu này.", "warning");
                LoadDanhSach();
                return;
            }

            var acc = db.taikhoan_tbs.FirstOrDefault(x => x.taikhoan == yc.taikhoan);
            if (acc == null)
            {
                Helper_Tabler_cl.ShowToast(this, "Không tìm thấy tài khoản của yêu cầu.", "warning");
                LoadDanhSach();
                return;
            }

            // cập nhật tài khoản
            acc.HeThongSanPham_Cap123 = yc.CapYeuCau;

            if (yc.CapYeuCau == 1)
                acc.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 = yc.GiaTriYeuCau;
            else if (yc.CapYeuCau == 2)
                acc.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 = yc.GiaTriYeuCau;
            else
                acc.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 = yc.GiaTriYeuCau;

            yc.TrangThai = 1; // đã duyệt
            yc.NgayDuyet = AhaTime_cl.Now;
            yc.NguoiDuyet = tkAdmin;
            yc.GhiChuAdmin = "Đã duyệt yêu cầu nâng cấp.";

            Helper_DongA_cl.AddNotify(
                db,
                tkAdmin,
                yc.taikhoan,
                "Yêu cầu nâng cấp hành vi của bạn đã được duyệt.",
                "/home/tao-yeu-cau.aspx");
            db.SubmitChanges();
        }

        Helper_Tabler_cl.ShowToast(this, "Đã duyệt yêu cầu nâng cấp.", "success");
        LoadDanhSach();
    }

    protected void btn_tuchoi_Click(object sender, EventArgs e)
    {
        int id;
        if (!TryGetRequestIdFromSender(sender, out id))
        {
            Helper_Tabler_cl.ShowToast(this, "Không xác định được yêu cầu cần từ chối.", "warning");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            var yc = db.YeuCau_HeThongSanPham_tbs
                .FirstOrDefault(x => x.id == id && x.TrangThai == 0);
            if (yc == null)
            {
                Helper_Tabler_cl.ShowToast(this, "Yêu cầu không còn ở trạng thái chờ duyệt.", "warning");
                LoadDanhSach();
                return;
            }
            string tkAdmin = GetCurrentAdminUser();
            if (!CanReviewByRequest(db, tkAdmin, yc.CapYeuCau, yc.GiaTriYeuCau))
            {
                Helper_Tabler_cl.ShowToast(this, "Bạn không có quyền từ chối hành vi của yêu cầu này.", "warning");
                LoadDanhSach();
                return;
            }

            yc.TrangThai = 2; // từ chối
            yc.NgayDuyet = AhaTime_cl.Now;
            yc.NguoiDuyet = tkAdmin;
            yc.GhiChuAdmin = "Từ chối yêu cầu nâng cấp.";

            Helper_DongA_cl.AddNotify(
                db,
                tkAdmin,
                yc.taikhoan,
                "Yêu cầu nâng cấp hành vi của bạn đã bị từ chối.",
                "/home/tao-yeu-cau.aspx");
            db.SubmitChanges();
        }

        Helper_Tabler_cl.ShowToast(this, "Đã từ chối yêu cầu.", "warning");
        LoadDanhSach();
    }
}
