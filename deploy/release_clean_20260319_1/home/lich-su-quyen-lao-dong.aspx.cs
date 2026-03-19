using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_lich_su_quyen_lao_dong : System.Web.UI.Page
{
    private const int ProfileCode = HanhViGhiNhanHoSo_cl.Profile_LaoDong;
    private const int LoaiHoSoVi = 3;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", true);

            string _tk = Session["taikhoan_home"] as string;
            if (!string.IsNullOrEmpty(_tk))
                ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);

            using (dbDataContext db = new dbDataContext())
            {
                int tierHome = TierHome_cl.TinhTierHome(db, ViewState["taikhoan"] + "");
                if (!TierHome_cl.CanViewHoSo(tierHome, LoaiHoSoVi))
                {
                    Session["thongbao_home"] = "Bạn cần đạt tầng cộng tác phát triển để xem hồ sơ hành vi lao động.";
                    Response.Redirect("/home/lich-su-quyen-uu-dai.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
            }

            set_dulieu_macdinh();
            show_main();
        }
    }

    public void set_dulieu_macdinh()
    {
        ViewState["current_page_lsgd_home"] = "1";
        ViewState["yeucau_kyhieu_uudai"] = "";
    }

    private string GetCurrentUser()
    {
        return (ViewState["taikhoan"] ?? "").ToString();
    }

    private void LoadTongHopHanhViVaYeuCau(dbDataContext db, string tk)
    {
        int fromKy, toKy;
        HanhViGhiNhanHoSo_cl.TryGetHanhViRange(ProfileCode, out fromKy, out toKy);

        HanhViHoSoSummary_cl summary = HanhViGhiNhanHoSo_cl.TinhTongHop(db, tk, ProfileCode);

        lb_so_du_trong_hoso.Text = summary.SoDuTrongHoSo.ToString("#,##0.##");
        lb_so_du_hanhvi_hople.Text = summary.TongSoDuHanhViHopLe.ToString("#,##0.##");
        lb_so_du_diem_nhan.Text = summary.TongSoDuDiemNhan.ToString("#,##0.##");
        lb_so_du_cho_duyet.Text = summary.TongSoDuDangChoDuyet.ToString("#,##0.##");
        lb_so_du_da_ghi_nhan.Text = summary.TongSoDaGhiNhan.ToString("#,##0.##");

        lb_moc_hople.Text =
            (summary.DieuKienHopLeText ?? HanhViGhiNhanHoSo_cl.GetDieuKienHopLeText())
            + " Mốc hệ thống hiện tại: " + AhaTime_cl.Now.ToString("dd/MM/yyyy HH:mm:ss")
            + " Điểm đã duyệt của hồ sơ này được ghi nhận vào dữ liệu chờ chi trả lương, không cộng trực tiếp vào Số dư điểm trong hồ sơ.";

        var hanhViRows = summary.Rows.Select(p => new
        {
            p.KyHieu9HanhVi_1_9,
            TenHanhVi = HanhVi9Cap_cl.EnsureDisplayWithoutPercent(p.KyHieu9HanhVi_1_9, p.TenHanhVi),
            p.SoDuDiemNhan,
            p.SoDuHanhViHopLe,
            p.SoDuDangChoDuyet,
            p.SoDaGhiNhan,
            p.CoTheGui
        }).ToList();

        RepeaterHanhViUuDai.DataSource = hanhViRows;
        RepeaterHanhViUuDai.DataBind();

        var dsYeuCauRaw = db.YeuCauRutQuyen_tbs
            .Where(p => p.TaiKhoan == tk
                && p.LoaiHanhVi == ProfileCode
                && p.KyHieu9HanhVi_1_9 >= fromKy
                && p.KyHieu9HanhVi_1_9 <= toKy)
            .OrderByDescending(p => p.NgayTao)
            .Select(p => new
            {
                IdYeuCauRut = p.IdYeuCauRut,
                p.NgayTao,
                p.NgayCapNhat,
                p.KyHieu9HanhVi_1_9,
                p.TongQuyen,
                p.TrangThai,
                p.NguoiDuyet,
                p.GhiChu
            })
            .ToList();

        Dictionary<Guid, decimal> daChiTraTheoYeuCau = new Dictionary<Guid, decimal>();
        if (dsYeuCauRaw.Count > 0)
        {
            var refToIdMap = dsYeuCauRaw.ToDictionary(
                p => HanhViGhiNhanHoSo_cl.GetPayoutRefId(p.IdYeuCauRut),
                p => p.IdYeuCauRut,
                StringComparer.OrdinalIgnoreCase);

            var payoutRows = db.LichSu_DongA_tbs
                .Where(p =>
                    p.taikhoan == tk
                    && p.KyHieu9HanhVi_1_9.HasValue
                    && p.KyHieu9HanhVi_1_9.Value >= fromKy
                    && p.KyHieu9HanhVi_1_9.Value <= toKy
                    && p.LoaiHoSo_Vi == LoaiHoSoVi
                    && p.CongTru.HasValue
                    && p.CongTru.Value == false
                    && p.id_rutdiem != null
                    && p.id_rutdiem.StartsWith(HanhViGhiNhanHoSo_cl.PayoutRefPrefix))
                .Select(p => new
                {
                    RefId = p.id_rutdiem,
                    SoDiem = p.dongA
                })
                .ToList();

            foreach (var payout in payoutRows)
            {
                Guid idYeuCau;
                if (!refToIdMap.TryGetValue((payout.RefId ?? "").Trim(), out idYeuCau))
                    continue;

                decimal current = 0m;
                if (daChiTraTheoYeuCau.ContainsKey(idYeuCau))
                    current = daChiTraTheoYeuCau[idYeuCau];
                daChiTraTheoYeuCau[idYeuCau] = current + (payout.SoDiem ?? 0m);
            }
        }

        var dsYeuCau = dsYeuCauRaw.Select(p => new
        {
            DaChiTra = daChiTraTheoYeuCau.ContainsKey(p.IdYeuCauRut) ? daChiTraTheoYeuCau[p.IdYeuCauRut] : 0m,
            p.IdYeuCauRut,
            p.NgayTao,
            p.NgayCapNhat,
            p.KyHieu9HanhVi_1_9,
            TenHanhVi = HanhVi9Cap_cl.GetTenHanhViKhongPhanTram(p.KyHieu9HanhVi_1_9),
            p.TongQuyen,
            TrangThaiCode = HanhViGhiNhanHoSo_cl.NormalizeTrangThai(p.TrangThai),
            TrangThaiText = HanhViGhiNhanHoSo_cl.GetTrangThaiText(p.TrangThai),
            p.NguoiDuyet,
            p.GhiChu
        }).ToList()
        .Select(p =>
        {
            bool daChiTraXong =
                p.TrangThaiCode == HanhViGhiNhanHoSo_cl.TrangThaiDaDuyet
                && (p.TongQuyen - p.DaChiTra) <= 0m;
            return new
            {
                p.IdYeuCauRut,
                p.NgayTao,
                p.NgayCapNhat,
                p.KyHieu9HanhVi_1_9,
                p.TenHanhVi,
                p.TongQuyen,
                TrangThaiCode = daChiTraXong ? "3" : p.TrangThaiCode,
                TrangThaiText = daChiTraXong ? "Đã chi trả" : p.TrangThaiText,
                p.NguoiDuyet,
                p.GhiChu
            };
        })
        .ToList();

        RepeaterYeuCauGhiNhan.DataSource = dsYeuCau;
        RepeaterYeuCauGhiNhan.DataBind();
    }

    #region main - phân trang - tìm kiếm
    public void show_main()
    {
        using (dbDataContext db = new dbDataContext())
        {
            string tk = GetCurrentUser();
            var list_all = HoSoLichSuTongHop_cl.LayLichSuTongHop(db, tk, LoaiHoSoVi)
                .Select(p =>
                {
                    p.TenHanhVi = HanhVi9Cap_cl.EnsureDisplayWithoutPercent(p.KyHieu9HanhVi_1_9, p.TenHanhVi);
                    return p;
                })
                .ToList();

            int _Tong_Record = list_all.Count;
            int show = 30; if (show <= 0) show = 30;
            int current_page = int.Parse(ViewState["current_page_lsgd_home"].ToString());
            int total_page = number_of_page_class.return_total_page(_Tong_Record, show);

            if (current_page < 1) current_page = 1;
            else if (current_page > total_page) current_page = total_page;

            ViewState["total_page"] = total_page;

            bool canNext = current_page < total_page;
            bool canPrev = current_page > 1;

            but_xemtiep.Enabled = canNext;
            but_xemtiep1.Enabled = canNext;
            but_quaylai.Enabled = canPrev;
            but_quaylai1.Enabled = canPrev;

            var list_split = list_all
                .Skip(current_page * show - show)
                .Take(show)
                .Select(p => new
                {
                    p.id,
                    p.ngay,
                    p.dongA,
                    p.CongTru,
                    p.ghichu,
                    p.id_donhang,
                    p.TenHanhVi
                })
                .ToList();

            int stt = (show * current_page) - show + 1;
            int _s1 = stt + list_split.Count() - 1;

            if (_Tong_Record != 0) lb_show.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");
            else lb_show.Text = "0-0/0";

            lb_show_md.Text = lb_show.Text;

            Repeater1.DataSource = list_split;
            Repeater1.DataBind();

            LoadTongHopHanhViVaYeuCau(db, tk);
        }
    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_lsgd_home"] = int.Parse(ViewState["current_page_lsgd_home"].ToString()) - 1;
        show_main();
    }

    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_lsgd_home"] = int.Parse(ViewState["current_page_lsgd_home"].ToString()) + 1;
        show_main();
    }

    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_lsgd_home"] = 1;
        show_main();
    }
    #endregion

    #region yêu cầu ghi nhận hành vi
    protected void but_show_form_yeucau_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);

        LinkButton btn = sender as LinkButton;
        if (btn == null) return;

        int kyHieu = Number_cl.Check_Int(btn.CommandArgument);
        if (!HanhViGhiNhanHoSo_cl.IsHanhViThuocHoSo(ProfileCode, kyHieu))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Hành vi không hợp lệ.", "Thông báo", true, "warning");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            string tk = GetCurrentUser();
            HanhViHoSoSummary_cl summary = HanhViGhiNhanHoSo_cl.TinhTongHop(db, tk, ProfileCode);
            HanhViHoSoRowBalance_cl row = summary.Rows.FirstOrDefault(p => p.KyHieu9HanhVi_1_9 == kyHieu);

            if (row == null || row.SoDuHanhViHopLe <= 0m)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Hành vi này chưa có số dư hợp lệ để gửi yêu cầu.", "Thông báo", true, "warning");
                return;
            }

            ViewState["yeucau_kyhieu_uudai"] = kyHieu.ToString();
            lb_yeucau_hanhvi.Text = HanhVi9Cap_cl.EnsureDisplayWithoutPercent(row.KyHieu9HanhVi_1_9, row.TenHanhVi);
            lb_yeucau_hople.Text = row.SoDuHanhViHopLe.ToString("#,##0.##");
            txt_so_diem_yeucau.Text = "";

            pn_yeucau.Visible = true;
            up_yeucau.Update();
        }
    }

    protected void but_close_form_yeucau_Click(object sender, EventArgs e)
    {
        txt_so_diem_yeucau.Text = "";
        pn_yeucau.Visible = false;
        up_yeucau.Update();
    }

    protected void but_gui_yeu_cau_ghi_nhan_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);

        int kyHieu = Number_cl.Check_Int((ViewState["yeucau_kyhieu_uudai"] ?? "").ToString());
        if (!HanhViGhiNhanHoSo_cl.IsHanhViThuocHoSo(ProfileCode, kyHieu))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Thiếu thông tin hành vi cần ghi nhận.", "Thông báo", true, "warning");
            return;
        }

        decimal soDiem = Number_cl.Check_Decimal((txt_so_diem_yeucau.Text ?? "").Trim());

        using (dbDataContext db = new dbDataContext())
        {
            try
            {
                string message;
                Guid idMoi;
                bool ok = HanhViGhiNhanHoSo_cl.TaoYeuCau(
                    db,
                    GetCurrentUser(),
                    ProfileCode,
                    kyHieu,
                    soDiem,
                    out message,
                    out idMoi);

                if (!ok)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, message, "Không thể gửi yêu cầu", true, "warning");
                    return;
                }

                db.SubmitChanges();

                Helper_Tabler_cl.ShowToast(
                    this.Page,
                    "Đã gửi yêu cầu ghi nhận hành vi. Mã yêu cầu: " + idMoi.ToString("N").Substring(0, 8).ToUpper(),
                    "success",
                    true,
                    2600,
                    "Thành công");

                txt_so_diem_yeucau.Text = "";
                pn_yeucau.Visible = false;
                up_yeucau.Update();

                show_main();
                up_main.Update();
            }
            catch (Exception ex)
            {
                try
                {
                    string tk = GetCurrentUser();
                    Log_cl.Add_Log(ex.Message, tk, ex.StackTrace);
                }
                catch { }

                Helper_Tabler_cl.ShowModal(this.Page, "Lỗi hệ thống: " + ex.Message, "Thông báo", true, "danger");
            }
        }
    }
    #endregion

    #region chi tiết đơn hàng
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            LinkButton button = (LinkButton)sender;
            string _iddh = button.CommandArgument;

            var q = db.DonHang_tbs.FirstOrDefault(p => p.id.ToString() == _iddh);
            if (q != null)
            {
                ViewState["iddh"] = _iddh;

                var q_dh = db.DonHang_tbs.FirstOrDefault(p => p.id.ToString() == _iddh);
                string _nguoimua = q_dh.nguoimua;
                string _nguoiban = q_dh.nguoiban;

                var q_ban = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _nguoiban);
                if (q_ban != null)
                {
                    Label100.Text = q_ban.hoten;
                    Label101.Text = q_ban.dienthoai;
                    Label102.Text = q_ban.diachi;
                }
                else
                {
                    Label100.Text = "";
                    Label101.Text = "";
                    Label102.Text = "";
                }

                var q_mua = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _nguoimua);
                if (q_mua != null)
                {
                    Label103.Text = q_mua.hoten;
                    Label104.Text = q_mua.dienthoai;
                    Label105.Text = q_mua.diachi;
                }
                else
                {
                    Label103.Text = "";
                    Label104.Text = "";
                    Label105.Text = "";
                }

                var q_ct = from ob1 in db.DonHang_ChiTiet_tbs.Where(p => p.id_donhang == _iddh)
                           join ob2 in db.BaiViet_tbs on ob1.idsp equals ob2.id.ToString() into SanPhamGroup
                           from ob2 in SanPhamGroup.DefaultIfEmpty()
                           select new
                           {
                               ob1.id,
                               ob1.id_donhang,
                               name = ob2 != null ? ob2.name : "",
                               image = ob2 != null ? ob2.image : "",
                               ob1.giaban,
                               ob1.soluong,
                               ob1.thanhtien
                           };

                Repeater2.DataSource = q_ct;
                Repeater2.DataBind();

                pn_chitiet.Visible = true;
                up_chitiet.Update();
            }
        }
    }

    protected void but_close_form_chitiet_Click(object sender, EventArgs e)
    {
        Repeater2.DataSource = null;
        Repeater2.DataBind();

        pn_chitiet.Visible = false;
        up_chitiet.Update();
    }
    #endregion
}
