using System;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class home_trao_doi : System.Web.UI.Page
{
    private const decimal VND_PER_A = 1000m;

    private decimal QuyDoi_VND_To_A(decimal vnd)
    {
        if (vnd <= 0) return 0m;
        decimal a = vnd / VND_PER_A;
        return Math.Ceiling(a * 100m) / 100m;
    }

    private int ClampInt(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    private string NormalizeReturnUrl(string raw)
    {
        string value = (raw ?? "").Trim();
        if (string.IsNullOrEmpty(value))
            return "";

        if (value.StartsWith("//", StringComparison.Ordinal))
            return "";

        Uri absolute;
        if (Uri.TryCreate(value, UriKind.Absolute, out absolute))
        {
            if (Request.Url != null && string.Equals(absolute.Host, Request.Url.Host, StringComparison.OrdinalIgnoreCase))
                value = absolute.PathAndQuery;
            else
                return "";
        }

        if (!value.StartsWith("/", StringComparison.Ordinal))
            return "";

        return value;
    }

    private string GetBackUrl()
    {
        string fromQuery = NormalizeReturnUrl((ViewState["return_url"] ?? "").ToString());
        if (!string.IsNullOrEmpty(fromQuery))
            return fromQuery;

        return PortalRequest_cl.IsShopPortalRequest() ? "/shop/default.aspx" : "/";
    }

    private void BindBackLinks()
    {
        string backUrl = GetBackUrl();
        hl_back_top.NavigateUrl = backUrl;
        hl_back_bottom.NavigateUrl = backUrl;
    }

    private void UpdateTongTienLabel()
    {
        int sl = Number_cl.Check_Int((txt_soluong.Text ?? "").Trim());
        sl = ClampInt(sl, 1, 999);
        txt_soluong.Text = sl.ToString();

        decimal giaVND = 0m;
        if (ViewState["gia_vnd"] != null)
            decimal.TryParse(ViewState["gia_vnd"].ToString(), out giaVND);

        decimal tongVND = giaVND * sl;
        lb_tong_vnd.Text = tongVND.ToString("#,##0");
        lb_tong_a.Text = QuyDoi_VND_To_A(tongVND).ToString("#,##0.##");
    }

    private string ResolveNguoiBanDangLai(dbDataContext db, BaiViet_tb sp)
    {
        string requested = (ViewState["user_bancheo"] ?? "").ToString().Trim();
        if (string.IsNullOrEmpty(requested))
            return "";

        if (string.Equals(requested, sp.nguoitao ?? "", StringComparison.OrdinalIgnoreCase))
            return "";

        if (!AccountVisibility_cl.IsSellerVisible(db, requested))
            return "";

        bool isValid = db.BanSanPhamNay_tbs.Any(x => x.taikhoan_ban == requested && x.idsp == sp.id.ToString());
        return isValid ? requested : "";
    }

    private bool LoadProductAndReceiver()
    {
        using (dbDataContext db = new dbDataContext())
        {
            string idsp = (ViewState["idsp"] ?? "").ToString();
            var sp = AccountVisibility_cl.FindVisibleProductById(db, idsp);
            if (sp == null)
            {
                but_xacnhan.Enabled = false;
                Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm đã ngừng bán hoặc không tồn tại.", "Thông báo", true, "warning");
                return false;
            }

            decimal giaVND = sp.giaban ?? 0m;
            int phanTram = ClampInt(sp.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0, 0, 50);

            ViewState["gia_vnd"] = giaVND;

            lb_ten_sp.Text = sp.name ?? "";
            lb_gia_vnd.Text = giaVND.ToString("#,##0.##");
            lb_gia_a.Text = QuyDoi_VND_To_A(giaVND).ToString("#,##0.##");
            lb_uu_dai.Text = phanTram.ToString();

            string image = string.IsNullOrWhiteSpace(sp.image) ? "/uploads/images/macdinh.jpg" : sp.image.Trim();
            img_product.Src = image;

            string tk = (ViewState["taikhoan"] ?? "").ToString();
            var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (q_tk != null)
            {
                txt_hoten_nguoinhan.Text = q_tk.hoten ?? "";
                txt_sdt_nguoinhan.Text = q_tk.dienthoai ?? "";
                txt_diachi_nguoinhan.Text = q_tk.diachi ?? "";
            }

            UpdateTongTienLabel();
            return true;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", true);

            string tkMaHoa = PortalRequest_cl.GetCurrentAccountEncrypted();
            if (string.IsNullOrEmpty(tkMaHoa))
            {
                Response.Redirect(PortalRequest_cl.IsShopPortalRequest() ? "/shop/login.aspx" : "/dang-nhap", true);
                return;
            }

            ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(tkMaHoa);
            ViewState["idsp"] = (Request.QueryString["idsp"] ?? "").Trim();
            ViewState["user_bancheo"] = (Request.QueryString["user_bancheo"] ?? "").Trim();
            ViewState["return_url"] = NormalizeReturnUrl(Request.QueryString["return_url"] ?? "");

            string idsp = (ViewState["idsp"] ?? "").ToString();
            if (string.IsNullOrEmpty(idsp))
            {
                Response.Redirect(GetBackUrl(), true);
                return;
            }

            int soLuong = Number_cl.Check_Int((Request.QueryString["qty"] ?? "1").Trim());
            soLuong = ClampInt(soLuong, 1, 999);
            txt_soluong.Text = soLuong.ToString();

            BindBackLinks();
            LoadProductAndReceiver();
        }
    }

    protected void txt_soluong_TextChanged(object sender, EventArgs e)
    {
        UpdateTongTienLabel();
    }

    protected void but_xacnhan_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);

        string tk = (ViewState["taikhoan"] ?? "").ToString();
        if (string.IsNullOrEmpty(tk))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Bạn cần đăng nhập.", "Thông báo", true, "warning");
            return;
        }

        string idsp = (ViewState["idsp"] ?? "").ToString();
        if (string.IsNullOrEmpty(idsp))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Không xác định được sản phẩm.", "Thông báo", true, "warning");
            return;
        }

        int sl = Number_cl.Check_Int((txt_soluong.Text ?? "").Trim());
        sl = ClampInt(sl, 1, 999);
        txt_soluong.Text = sl.ToString();

        using (dbDataContext db = new dbDataContext())
        {
            var sp = AccountVisibility_cl.FindVisibleProductById(db, idsp);
            if (sp == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm đã ngừng bán hoặc không tồn tại.", "Thông báo", true, "warning");
                return;
            }

            if (string.Equals(sp.nguoitao ?? "", tk, StringComparison.OrdinalIgnoreCase))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Bạn không thể trao đổi sản phẩm do chính mình đăng.", "Thông báo", true, "warning");
                return;
            }

            var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (q_tk == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản không tồn tại.", "Thông báo", true, "danger");
                return;
            }

            decimal soDuA = q_tk.DongA ?? 0m;
            decimal soDuUuDaiA = q_tk.Vi1That_Evocher_30PhanTram ?? 0m;

            decimal giaVND = sp.giaban ?? 0m;
            decimal tongVND = giaVND * sl;

            int phanTramUuDai = ClampInt(sp.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0, 0, 50);

            decimal canTraA_Tong = QuyDoi_VND_To_A(tongVND);

            decimal tienUuDaiVND = 0m;
            decimal A_UuDai = 0m;
            if (phanTramUuDai > 0)
            {
                tienUuDaiVND = tongVND * phanTramUuDai / 100m;
                A_UuDai = QuyDoi_VND_To_A(tienUuDaiVND);
            }

            bool apDungUuDai = (phanTramUuDai > 0 && A_UuDai > 0m && soDuUuDaiA >= A_UuDai);

            decimal A_ConLai = 0m;
            if (apDungUuDai)
            {
                A_ConLai = canTraA_Tong - A_UuDai;
                if (A_ConLai < 0m) A_ConLai = 0m;

                if (A_ConLai > soDuA)
                {
                    Helper_Tabler_cl.ShowModal(
                        this.Page,
                        "Bạn đủ ví ưu đãi nhưng Đồng A không đủ cho phần còn lại.<br/>" +
                        "Cần <b>" + A_ConLai.ToString("#,##0.##") + " A</b>, bạn đang có <b>" + soDuA.ToString("#,##0.##") + " A</b>.",
                        "Thông báo",
                        true,
                        "danger"
                    );
                    return;
                }
            }
            else
            {
                if (canTraA_Tong > soDuA)
                {
                    Helper_Tabler_cl.ShowModal(
                        this.Page,
                        "Quyền tiêu dùng không đủ.<br/>" +
                        "Cần <b>" + canTraA_Tong.ToString("#,##0.##") + " A</b> (≈ " + tongVND.ToString("#,##0") + "đ), bạn đang có <b>" + soDuA.ToString("#,##0.##") + " A</b>.",
                        "Thông báo",
                        true,
                        "danger"
                    );
                    return;
                }
            }

            DateTime now = AhaTime_cl.Now;

            DonHang_tb dh = new DonHang_tb();
            dh.ngaydat = now;
            dh.nguoimua = tk;
            dh.nguoiban = sp.nguoitao;
            dh.order_status = DonHangStateMachine_cl.Order_DaDat;
            dh.exchange_status = DonHangStateMachine_cl.Exchange_ChuaTraoDoi;
            dh.tongtien = tongVND;
            dh.hoten_nguoinhan = (txt_hoten_nguoinhan.Text ?? "").Trim();
            dh.sdt_nguoinhan = (txt_sdt_nguoinhan.Text ?? "").Trim();
            dh.diahchi_nguoinhan = (txt_diachi_nguoinhan.Text ?? "").Trim();
            dh.online_offline = true;
            dh.chothanhtoan = false;
            DonHangStateMachine_cl.SyncLegacyStatus(dh);

            db.DonHang_tbs.InsertOnSubmit(dh);
            db.SubmitChanges();

            string id_dh = dh.id.ToString();

            DonHang_ChiTiet_tb ct = new DonHang_ChiTiet_tb();
            ct.id_donhang = id_dh;
            ct.idsp = idsp;
            ct.nguoiban_goc = dh.nguoiban;
            ct.nguoiban_danglai = ResolveNguoiBanDangLai(db, sp);
            ct.soluong = sl;
            ct.giaban = giaVND;
            ct.thanhtien = tongVND;
            ct.PhanTram_GiamGia_ThanhToan_BangEvoucher = phanTramUuDai;
            db.DonHang_ChiTiet_tbs.InsertOnSubmit(ct);

            if (apDungUuDai)
            {
                LichSu_DongA_tb lsUuDai = new LichSu_DongA_tb();
                lsUuDai.taikhoan = tk;
                lsUuDai.dongA = A_UuDai;
                lsUuDai.ngay = now;
                lsUuDai.CongTru = false;
                lsUuDai.id_donhang = id_dh;
                lsUuDai.LoaiHoSo_Vi = 2;
                lsUuDai.ghichu =
                    "Ưu đãi " + phanTramUuDai + "% đơn " + id_dh + ": " + A_UuDai.ToString("#,##0.##") + "A (≈ " + tienUuDaiVND.ToString("#,##0") + "đ, 1A=" + VND_PER_A.ToString("#,##0") + "đ)";
                db.LichSu_DongA_tbs.InsertOnSubmit(lsUuDai);

                q_tk.Vi1That_Evocher_30PhanTram = (q_tk.Vi1That_Evocher_30PhanTram ?? 0m) - A_UuDai;

                decimal conLaiVND = tongVND - tienUuDaiVND;
                LichSu_DongA_tb lsA = new LichSu_DongA_tb();
                lsA.taikhoan = tk;
                lsA.dongA = A_ConLai;
                lsA.ngay = now;
                lsA.CongTru = false;
                lsA.id_donhang = id_dh;
                lsA.LoaiHoSo_Vi = 1;
                lsA.ghichu =
                    "Trao đổi đơn " + id_dh + " (phần còn lại): " + A_ConLai.ToString("#,##0.##") + "A (≈ " + conLaiVND.ToString("#,##0") + "đ, 1A=" + VND_PER_A.ToString("#,##0") + "đ)";
                db.LichSu_DongA_tbs.InsertOnSubmit(lsA);

                q_tk.DongA = (q_tk.DongA ?? 0m) - A_ConLai;
            }
            else
            {
                LichSu_DongA_tb ls = new LichSu_DongA_tb();
                ls.taikhoan = tk;
                ls.dongA = canTraA_Tong;
                ls.ngay = now;
                ls.CongTru = false;
                ls.id_donhang = id_dh;
                ls.LoaiHoSo_Vi = 1;
                ls.ghichu =
                    "Trao đổi đơn " + id_dh + ": " + canTraA_Tong.ToString("#,##0.##") + "A (≈ " + tongVND.ToString("#,##0") + "đ, 1A=" + VND_PER_A.ToString("#,##0") + "đ)";
                db.LichSu_DongA_tbs.InsertOnSubmit(ls);

                q_tk.DongA = (q_tk.DongA ?? 0m) - canTraA_Tong;
            }

            string buyerName = !string.IsNullOrEmpty(q_tk.hoten) ? q_tk.hoten : tk;
            bool daCoThongBao = db.ThongBao_tbs.Any(x =>
                x.bin == false
                && x.nguoinhan == dh.nguoiban
                && x.link == "/home/don-ban.aspx"
                && x.noidung.Contains("ID đơn hàng: " + id_dh)
            );

            if (!daCoThongBao)
            {
                ThongBao_tb tb = new ThongBao_tb();
                tb.id = Guid.NewGuid();
                tb.daxem = false;
                tb.nguoithongbao = tk;
                tb.nguoinhan = dh.nguoiban;
                tb.link = "/home/don-ban.aspx";
                tb.noidung = buyerName + " vừa đặt hàng đến bạn. ID đơn hàng: " + id_dh;
                tb.thoigian = now;
                tb.bin = false;
                db.ThongBao_tbs.InsertOnSubmit(tb);
            }

            db.SubmitChanges();

            Helper_Tabler_cl.ShowModal(this.Page, "Đặt hàng thành công. ID đơn hàng: <b>" + id_dh + "</b>", "Thông báo", true, "success");
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "redirect_donmua_after_create",
                "setTimeout(function(){ window.location.href='/home/don-mua.aspx'; }, 1000);", true);
        }
    }
}
