using System;
using System.Linq;

public partial class home_tao_yeu_cau : System.Web.UI.Page
{
    int CapHienTai, GiaTriHienTai;
    int CapYeuCau, GiaTriYeuCau;

    protected void Page_Load(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        if (!IsPostBack)
        {
            LoadAll();
        }
        else
        {
            RestoreCurrentStateFromViewState();
        }
    }

    private int GetIntFromViewState(string key, int fallback)
    {
        object raw = ViewState[key];
        int value;
        return int.TryParse(raw + "", out value) ? value : fallback;
    }

    private void SaveCurrentStateToViewState()
    {
        ViewState["CapHienTai"] = CapHienTai;
        ViewState["GiaTriHienTai"] = GiaTriHienTai;
    }

    private void RestoreCurrentStateFromViewState()
    {
        CapHienTai = GetIntFromViewState("CapHienTai", 0);
        GiaTriHienTai = GetIntFromViewState("GiaTriHienTai", 0);
    }

    private void PopulateCurrentStateFromAccount(taikhoan_tb acc)
    {
        CapHienTai = acc.HeThongSanPham_Cap123 ?? 1;
        GiaTriHienTai =
            CapHienTai == 1 ? (acc.HeThongSanPham_QuyenLoi_MoVi_Cap1_15_9_6 ?? 15) :
            CapHienTai == 2 ? (acc.HeThongSanPham_QuyenLoi_MoVi_Cap2_25_15_10 ?? 10) :
                              (acc.HeThongSanPham_QuyenLoi_MoVi_Cap3_10_6_4 ?? 10);
        SaveCurrentStateToViewState();
    }

    private void EnsureCurrentStateLoaded()
    {
        if (CapHienTai > 0 && GiaTriHienTai > 0) return;

        RestoreCurrentStateFromViewState();
        if (CapHienTai > 0 && GiaTriHienTai > 0) return;

        string tk = mahoa_cl.giaima_Bcorn(Session["taikhoan_home"] + "");
        using (dbDataContext db = new dbDataContext())
        {
            var acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (acc != null)
            {
                PopulateCurrentStateFromAccount(acc);
            }
        }
    }

    void LoadAll()
    {
        string tk = mahoa_cl.giaima_Bcorn(Session["taikhoan_home"] + "");
        using (dbDataContext db = new dbDataContext())
        {
            var acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (acc == null) return;

            // ===== HIỆN TRẠNG =====
            PopulateCurrentStateFromAccount(acc);

            lb_hientai.Text = HanhVi9Cap_cl.GetTenCapDoTheoCapGiaTri(CapHienTai, GiaTriHienTai);
            lb_mota_hientai.Text = GetMoTaFromDB(CapHienTai, GiaTriHienTai);

            // ===== TRÁCH NHIỆM (CHỈ HIỆN Ở HIỆN TRẠNG) =====
            string trachNhiem = GetTrachNhiemFromDB(CapHienTai, GiaTriHienTai);
            lb_trachnhiem_hientai.Text = trachNhiem;
            pn_trachnhiem_hientai.Visible = !string.IsNullOrWhiteSpace(trachNhiem);

            // Reset phần hiển thị yêu cầu khi load lại
            lb_yeucau.Text = "";
            lb_mota_yeucau.Text = "";
            ViewState["CapYeuCau"] = null;
            ViewState["GiaTriYeuCau"] = null;

            // ===== BIND DROPDOWN =====
            BindDropdownTheoCap(CapHienTai, GiaTriHienTai);
            LoadLichSuYeuCau(db, tk);
        }
    }

    private string GetTrangThaiText(int trangThai)
    {
        if (trangThai == 1) return "Đã duyệt";
        if (trangThai == 2) return "Từ chối";
        return "Chờ duyệt";
    }

    private string GetTrangThaiCss(int trangThai)
    {
        if (trangThai == 1) return "approved";
        if (trangThai == 2) return "rejected";
        return "pending";
    }

    private void LoadLichSuYeuCau(dbDataContext db, string tk)
    {
        var listRaw = db.YeuCau_HeThongSanPham_tbs
            .Where(x => x.taikhoan == tk)
            .OrderByDescending(x => x.id)
            .Take(100)
            .Select(x => new
            {
                x.id,
                x.NgayTao,
                x.NgayDuyet,
                x.NguoiDuyet,
                x.GhiChuAdmin,
                x.CapHienTai,
                x.GiaTriHienTai,
                x.CapYeuCau,
                x.GiaTriYeuCau,
                x.TrangThai
            })
            .ToList();

        var list = listRaw.Select(x => new
        {
            x.id,
            x.NgayTao,
            x.NgayDuyet,
            x.NguoiDuyet,
            x.GhiChuAdmin,
            HienTaiText = HanhVi9Cap_cl.GetTenCapDoTheoCapGiaTri(x.CapHienTai, x.GiaTriHienTai),
            YeuCauText = HanhVi9Cap_cl.GetTenCapDoTheoCapGiaTri(x.CapYeuCau, x.GiaTriYeuCau),
            TrangThaiText = GetTrangThaiText(x.TrangThai),
            TrangThaiCss = GetTrangThaiCss(x.TrangThai)
        }).ToList();

        rp_lichsu_yeucau.DataSource = list;
        rp_lichsu_yeucau.DataBind();
        pn_lichsu_empty.Visible = list.Count == 0;
    }

    // ✅ CHỈNH: truyền thêm cap/giaTri hiện tại để loại bỏ khỏi dropdown
    void BindDropdownTheoCap(int capHienTai, int giaTriHienTai)
    {
        ddl_level.Items.Clear();
        ddl_level.Items.Add(new System.Web.UI.WebControls.ListItem("-- Chọn cấp độ --", ""));

        // ===== THỨ TỰ LEVEL CHUẨN – KHÓA BẰNG SỐ =====

        // Cấp 1
        AddLevelOption(1, 1, 15, capHienTai, giaTriHienTai); // KẾT NỐI
        AddLevelOption(2, 1, 9, capHienTai, giaTriHienTai);  // CHIA SẺ
        AddLevelOption(3, 1, 6, capHienTai, giaTriHienTai);  // MARKETING

        // Cấp 2
        if (capHienTai >= 2)
        {
            AddLevelOption(4, 2, 10, capHienTai, giaTriHienTai); // BÁN HÀNG
            AddLevelOption(5, 2, 15, capHienTai, giaTriHienTai); // PHÁT TRIỂN
            AddLevelOption(6, 2, 25, capHienTai, giaTriHienTai); // ĐIỀU PHỐI
        }

        // Cấp 3
        if (capHienTai >= 3)
        {
            AddLevelOption(7, 3, 10, capHienTai, giaTriHienTai); // PHÚC LỢI
            AddLevelOption(8, 3, 6, capHienTai, giaTriHienTai);  // GHI NHẬN
            AddLevelOption(9, 3, 4, capHienTai, giaTriHienTai);  // CHĂM SÓC
        }
    }

    void AddLevelOption(int order, int c, int v, int capHienTai, int giaTriHienTai)
    {
        // ✅ KHÔNG CHO CHỌN LEVEL ĐANG Ở HIỆN TẠI
        if (c == capHienTai && v == giaTriHienTai) return;

        ddl_level.Items.Add(
            new System.Web.UI.WebControls.ListItem(
                order + ". " + HanhVi9Cap_cl.GetTenCapDoTheoCapGiaTri(c, v),
                c + "_" + v
            )
        );
    }

    protected void ddl_level_SelectedIndexChanged(object sender, EventArgs e)
    {
        EnsureCurrentStateLoaded();

        if (string.IsNullOrEmpty(ddl_level.SelectedValue))
        {
            lb_yeucau.Text = "";
            lb_mota_yeucau.Text = "";
            ViewState["CapYeuCau"] = null;
            ViewState["GiaTriYeuCau"] = null;
            return;
        }

        var arr = ddl_level.SelectedValue.Split('_');
        CapYeuCau = int.Parse(arr[0]);
        GiaTriYeuCau = int.Parse(arr[1]);

        // ✅ an toàn thêm: nếu lỡ có trường hợp trùng hiện tại (do data/logic khác), chặn luôn
        if (CapYeuCau == CapHienTai && GiaTriYeuCau == GiaTriHienTai)
        {
            ddl_level.SelectedIndex = 0;
            lb_yeucau.Text = "";
            lb_mota_yeucau.Text = "";
            ViewState["CapYeuCau"] = null;
            ViewState["GiaTriYeuCau"] = null;

            Helper_Tabler_cl.ShowModal(this,
                "Bạn đang ở cấp độ này rồi, vui lòng chọn cấp độ khác.",
                "Không thể chọn cấp hiện tại", true, "warning");
            return;
        }

        ViewState["CapYeuCau"] = CapYeuCau;
        ViewState["GiaTriYeuCau"] = GiaTriYeuCau;

        lb_yeucau.Text = HanhVi9Cap_cl.GetTenCapDoTheoCapGiaTri(CapYeuCau, GiaTriYeuCau);
        lb_mota_yeucau.Text = GetMoTaFromDB(CapYeuCau, GiaTriYeuCau);

        // KHÔNG HIỂN THỊ TRÁCH NHIỆM Ở PHẦN YÊU CẦU
    }

    string GetMoTaFromDB(int cap, int giaTri)
    {
        string key = cap + "_" + giaTri;
        using (dbDataContext db = new dbDataContext())
        {
            return db.VanBanMoTa_9Level_tbs
                .Where(x => x.Capbac == key)
                .Select(x => x.MoTa)
                .FirstOrDefault() ?? "";
        }
    }

    // LẤY TRÁCH NHIỆM (cột TrachNhiem trong VanBanMoTa_9Level_tbs)
    string GetTrachNhiemFromDB(int cap, int giaTri)
    {
        string key = cap + "_" + giaTri;
        using (dbDataContext db = new dbDataContext())
        {
            return db.VanBanMoTa_9Level_tbs
                .Where(x => x.Capbac == key)
                .Select(x => x.TrachNhiem)
                .FirstOrDefault() ?? "";
        }
    }

    protected void but_gui_yeucau_Click(object sender, EventArgs e)
    {
        EnsureCurrentStateLoaded();

        if (!chk_dongy.Checked)
        {
            Helper_Tabler_cl.ShowModal(this,
                "Bạn phải đồng ý với điều khoản và chính sách.",
                "Chưa xác nhận", true, "warning");
            return;
        }

        if (ViewState["CapYeuCau"] == null || ViewState["GiaTriYeuCau"] == null)
        {
            Helper_Tabler_cl.ShowModal(this,
                "Vui lòng chọn cấp độ muốn đề xuất.",
                "Thiếu thông tin", true, "warning");
            return;
        }

        CapYeuCau = (int)ViewState["CapYeuCau"];
        GiaTriYeuCau = (int)ViewState["GiaTriYeuCau"];

        // ✅ chặn thêm ở bước gửi (phòng trường hợp viewstate bị set sai)
        if (CapYeuCau == CapHienTai && GiaTriYeuCau == GiaTriHienTai)
        {
            Helper_Tabler_cl.ShowModal(this,
                "Bạn đang ở cấp độ này rồi, không thể gửi yêu cầu cho cấp hiện tại.",
                "Không hợp lệ", true, "warning");
            return;
        }

        string tk = mahoa_cl.giaima_Bcorn(Session["taikhoan_home"] + "");

        using (dbDataContext db = new dbDataContext())
        {
            var acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (acc == null)
            {
                Helper_Tabler_cl.ShowModal(this,
                    "Không tìm thấy thông tin tài khoản hiện tại.",
                    "Thiếu dữ liệu", true, "warning");
                return;
            }
            PopulateCurrentStateFromAccount(acc);

            if (CapYeuCau == CapHienTai && GiaTriYeuCau == GiaTriHienTai)
            {
                Helper_Tabler_cl.ShowModal(this,
                    "Bạn đang ở cấp độ này rồi, vui lòng chọn cấp độ khác.",
                    "Không hợp lệ", true, "warning");
                return;
            }

            var yc = new YeuCau_HeThongSanPham_tb
            {
                taikhoan = tk,
                CapHienTai = CapHienTai,
                GiaTriHienTai = GiaTriHienTai,
                CapYeuCau = CapYeuCau,
                GiaTriYeuCau = GiaTriYeuCau,
                TrangThai = 0,
                NgayTao = AhaTime_cl.Now
            };

            db.YeuCau_HeThongSanPham_tbs.InsertOnSubmit(yc);
            db.SubmitChanges();
        }

        Helper_Tabler_cl.ShowToast(this,
            "Đã gửi yêu cầu, vui lòng chờ admin duyệt.",
            "success", true, 2500, "Thành công");

        LoadAll();
    }
}
