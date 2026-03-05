using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_page_chi_tiet_bai_viet : System.Web.UI.Page
{
    private int PageSize = 20;

    // ✅ TỶ GIÁ: 1A = 1000 VNĐ
    private const decimal VND_PER_A = 1000m;

    // ✅ Quy đổi VNĐ -> A (làm tròn lên 2 chữ số)
    private decimal QuyDoi_VND_To_A(decimal vnd)
    {
        if (vnd <= 0) return 0m;
        decimal a = vnd / VND_PER_A;
        return Math.Ceiling(a * 100m) / 100m;
    }

    private int CurrentPage
    {
        get => ViewState["CurrentPage"] != null ? (int)ViewState["CurrentPage"] : 1;
        set => ViewState["CurrentPage"] = value;
    }

    private string RenderMainMediaHtml(string mediaUrl, string title)
    {
        string safeUrl = MediaFile_cl.GetSafeUrl(mediaUrl);
        if (string.IsNullOrEmpty(safeUrl))
            safeUrl = "/uploads/images/macdinh.jpg";
        string safeAlt = MediaFile_cl.GetSafeText(title);

        if (MediaFile_cl.IsVideo(mediaUrl))
        {
            string mime = MediaFile_cl.GetVideoMime(mediaUrl);
            return "<video controls playsinline preload='metadata' style='max-width:100%;max-height:100%;object-fit:contain'>" +
                   "<source src='" + safeUrl + "' type='" + mime + "' />" +
                   "</video>";
        }

        return "<img src='" + safeUrl + "' alt='" + safeAlt + "' style='max-width:100%;max-height:100%;object-fit:contain' />";
    }

    private string RenderThumbMediaHtml(string mediaUrl)
    {
        string safeUrl = MediaFile_cl.GetSafeUrl(mediaUrl);
        if (string.IsNullOrEmpty(safeUrl))
            safeUrl = "/uploads/images/macdinh.jpg";

        string encodedForJs = HttpUtility.JavaScriptStringEncode(safeUrl);
        if (MediaFile_cl.IsVideo(mediaUrl))
        {
            string mime = MediaFile_cl.GetVideoMime(mediaUrl);
            return "<video muted playsinline preload='metadata' onclick=\"changeMainMedia('" + encodedForJs + "','video')\">" +
                   "<source src='" + safeUrl + "' type='" + mime + "' />" +
                   "</video>";
        }

        return "<img src='" + safeUrl + "' onclick=\"changeMainMedia('" + encodedForJs + "','image')\" />";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", false);

            if (string.IsNullOrWhiteSpace(Request.QueryString["idbv"]))
            {
                Response.Redirect("/");
            }

            ViewState["idsp"] = Request.QueryString["idbv"].ToString().Trim();

            using (dbDataContext db = new dbDataContext())
            {
                var q = db.BaiViet_tbs.FirstOrDefault(p => p.id.ToString() == ViewState["idsp"].ToString() && p.bin == false);
                if (q == null)
                {
                    Response.Redirect("/");
                }

                string sessionKey = "viewed_" + ViewState["idsp"].ToString();
                if (Session[sessionKey] == null)
                {
                    q.LuotTruyCap = (q.LuotTruyCap ?? 0) + 1;
                    db.SubmitChanges();
                    Session[sessionKey] = true;
                }

                Label3.Text = q.name;
                ViewState["name"] = q.name;

                Label1.Text = q.description;

                string thanhPho = string.IsNullOrWhiteSpace(q.ThanhPho) ? "Không có" : q.ThanhPho;
                Label2.Text = $"<a href='{q.LinkMap}' target='_blank' class='text-muted'>" +
                              $"<i class='ti ti-map-pin'></i> {thanhPho}</a>";

                TimeSpan timeAgo = AhaTime_cl.Now - q.ngaytao.Value;
                if (timeAgo.TotalDays >= 1)
                    Label6.Text = $"Tạo {(int)timeAgo.TotalDays} ngày trước";
                else if (timeAgo.TotalHours >= 1)
                    Label6.Text = $"Tạo {(int)timeAgo.TotalHours} giờ trước";
                else if (timeAgo.TotalMinutes >= 1)
                    Label6.Text = $"Tạo {(int)timeAgo.TotalMinutes} phút trước";
                else
                    Label6.Text = "Vừa xong";

                litMainMedia.Text = RenderMainMediaHtml(q.image, q.name);

                var listAnh = new List<string>();
                if (!string.IsNullOrEmpty(q.image))
                    listAnh.Add(q.image);

                var anhPhuDB = db.AnhSanPham_tbs
                    .Where(p => p.idsp.ToString() == q.id.ToString())
                    .Select(p => p.url)
                    .ToList();

                listAnh.AddRange(anhPhuDB);

                string html = "";
                foreach (var link in listAnh)
                {
                    html += RenderThumbMediaHtml(link);
                }
                listAnhPhu.InnerHtml = html;

                // ✅ GIÁ BÁN VNĐ
                Label5.Text = q.giaban.Value.ToString("#,##0.##");

                Label4.Text = q.content_post;
                ViewState["giaban"] = q.giaban;
                ViewState["idmn"] = q.id_DanhMuc;
                ViewState["nguoiban"] = q.nguoitao;
                ViewState["user_bancheo"] = "";

                #region meta
                string title = q.name;
                string description = q.description;
                string imageRelativePath = listAnh.FirstOrDefault(p => MediaFile_cl.IsImage(p));
                if (string.IsNullOrEmpty(imageRelativePath))
                    imageRelativePath = "/uploads/images/macdinh.jpg";
                string imageUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}{imageRelativePath}";

                string metaTags = $@"
                    <title>{title}</title>
                    <meta name='description' content='{description}' />
                    <meta property='og:title' content='{title}' />
                    <meta property='og:description' content='{description}' />
                    <meta property='og:image' content='{imageUrl}' />
                    <meta property='og:type' content='website' />
                    <meta property='og:url' content='{Request.Url.AbsoluteUri}' />
                    <meta name='twitter:card' content='summary_large_image' />
                    <meta name='twitter:title' content='{title}' />
                    <meta name='twitter:description' content='{description}' />
                    <meta name='twitter:image' content='{imageUrl}' />
                ";
                literal_meta.Text = metaTags;
                #endregion

                #region xử lý người bán
                string url = Request.RawUrl;
                string[] parts = url.Split('-');
                string _tk_ban = parts[parts.Length - 2];

                if (!string.IsNullOrEmpty(_tk_ban))
                {
                    var q_check = db.BanSanPhamNay_tbs.FirstOrDefault(p => p.taikhoan_ban == _tk_ban && p.idsp == ViewState["idsp"].ToString());
                    if (q_check != null)
                    {
                        ViewState["nguoiban"] = _tk_ban;
                        ViewState["user_bancheo"] = _tk_ban;
                    }
                }

                var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["nguoiban"].ToString());
                if (q_tk != null)
                {
                    ViewState["avt_query"] = q_tk.anhdaidien;
                    ViewState["hoten_query"] = q_tk.hoten;
                    ViewState["sdt_query"] = q_tk.dienthoai;
                    ViewState["link_nguoiban"] = ShopSlug_cl.GetPublicUrl(db, q_tk);

                    // ✅ Quyền tiêu dùng format có lẻ
                    ViewState["DongA_query"] = q_tk.DongA.Value.ToString("#,##0.##");
                }
                else
                {
                    ViewState["link_nguoiban"] = "/" + ViewState["nguoiban"] + ".info";
                }
                #endregion

                #region k cho mua hàng sp mình bán
                string _tk1 = Session["taikhoan_home"] as string;

                if (!string.IsNullOrEmpty(_tk1))
                {
                    ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk1);
                }
                else
                {
                    ViewState["taikhoan"] = null;
                }

                LoadDanhGia(1);

                if (ViewState["taikhoan"] == null)
                {
                    PlaceHolder1.Visible = false;
                    InputReview.Visible = false;
                }
                else
                {
                    if (ViewState["nguoiban"].ToString() == ViewState["taikhoan"].ToString())
                    {
                        PlaceHolder1.Visible = false;
                        InputReview.Visible = false;
                    }
                    else
                    {
                        PlaceHolder1.Visible = true;
                    }
                }
                #endregion

                if (ViewState["taikhoan"] != null)
                {
                    int idBaiViet = int.Parse(ViewState["idsp"].ToString());
                    string taiKhoan = ViewState["taikhoan"].ToString();

                    var existing = db.TinDaXem_tbs
                                     .FirstOrDefault(t => t.idBaiViet == idBaiViet && t.TaiKhoan == taiKhoan);

                    if (existing != null)
                    {
                        existing.NgayXem = AhaTime_cl.Now;
                    }
                    else
                    {
                        TinDaXem_tb TinDaXem = new TinDaXem_tb
                        {
                            idBaiViet = idBaiViet,
                            TaiKhoan = taiKhoan,
                            NgayXem = AhaTime_cl.Now
                        };
                        db.TinDaXem_tbs.InsertOnSubmit(TinDaXem);
                    }

                    db.SubmitChanges();
                }
            }
        }
        else
        {
            int totalPages = ViewState["TotalPages"] != null ? (int)ViewState["TotalPages"] : 1;
            DisplayPaging(totalPages);
        }
    }

    protected void Page_Changed(object sender, CommandEventArgs e)
    {
        int pageIndex = int.Parse(e.CommandArgument.ToString());
        LoadDanhGia(pageIndex);
    }

    private void LoadDanhGia(int page)
    {
        using (dbDataContext db = new dbDataContext())
        {
            hdDiem.Value = "1";
            var idBaiViet = ViewState["idsp"].ToString();

            var danhSachDanhGiaVaAnh = (from dg in db.DanhGiaBaiViets
                                        join tk in db.taikhoan_tbs on dg.TaiKhoanDanhGia equals tk.taikhoan
                                        where dg.idBaiViet == idBaiViet
                                        orderby dg.NgayDang descending
                                        select new
                                        {
                                            dg.NoiDung,
                                            dg.Diem,
                                            dg.TaiKhoanDanhGia,
                                            dg.NgayDang,
                                            dg.UrlAnh,
                                            dg.ThuocTaiKhoan,
                                            HoTen = tk.hoten,
                                            AnhDaiDien = tk.anhdaidien
                                        }).ToList();

            int totalItems = danhSachDanhGiaVaAnh.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / PageSize);
            if (totalPages == 0) totalPages = 1;

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            CurrentPage = page;
            ViewState["TotalPages"] = totalPages;

            var dataPage = danhSachDanhGiaVaAnh
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList()
                .Select(x => new
                {
                    x.NoiDung,
                    x.Diem,
                    x.TaiKhoanDanhGia,
                    x.NgayDang,
                    x.UrlAnh,
                    x.ThuocTaiKhoan,
                    x.HoTen,
                    x.AnhDaiDien,
                    HoSoUrl = ShopSlug_cl.GetPublicUrlByTaiKhoan(db, x.TaiKhoanDanhGia)
                })
                .ToList();

            ListReview.Visible = dataPage.Any();
            rptDanhGia.DataSource = dataPage;
            rptDanhGia.DataBind();

            if (ViewState["taikhoan"] != null)
            {
                string taikhoan = ViewState["taikhoan"].ToString();

                var duocanhGia = (from dh in db.DonHang_tbs
                                  join ct in db.DonHang_ChiTiet_tbs on dh.id.ToString() equals ct.id_donhang
                                  where dh.nguoimua == taikhoan
                                  select dh).FirstOrDefault();

                bool daDanhGia = false;
                if (duocanhGia != null)
                {
                    string taiKhoanDanhGia = ViewState["taikhoan"]?.ToString() ?? "";
                    string nguoiBan = ViewState["nguoiban"]?.ToString() ?? "";

                    daDanhGia = danhSachDanhGiaVaAnh.Any(x =>
                        x.TaiKhoanDanhGia == taiKhoanDanhGia ||
                        x.ThuocTaiKhoan == nguoiBan
                    );
                }
                InputReview.Visible = !daDanhGia;
            }

            DisplayPaging(totalPages);
        }
    }

    private void DisplayPaging(int totalPages)
    {
        pnlPaging.Controls.Clear();

        if (totalPages <= 1)
            return;

        for (int i = 1; i <= totalPages; i++)
        {
            LinkButton lb = new LinkButton();
            lb.Text = i.ToString();
            lb.CommandArgument = i.ToString();
            lb.Command += Page_Changed;

            if (i == CurrentPage)
            {
                lb.Enabled = false;
                lb.CssClass = "current-page";
            }

            pnlPaging.Controls.Add(lb);
            pnlPaging.Controls.Add(new LiteralControl(" "));
        }
    }

    protected void btnGuiDanhGia_Click(object sender, EventArgs e)
    {
        int diem = int.TryParse(hdDiem.Value, out int result) ? result : 0;
        var danhGia = new DanhGiaBaiViet
        {
            idBaiViet = ViewState["idsp"]?.ToString(),
            NoiDung = txtNoiDung.Text.Trim(),
            Diem = diem,
            TaiKhoanDanhGia = ViewState["taikhoan"]?.ToString(),
            ThuocTaiKhoan = ViewState["nguoiban"]?.ToString(),
            NgayDang = AhaTime_cl.Now,
            UrlAnh = TxtIcon.Text
        };

        using (dbDataContext db = new dbDataContext())
        {
            db.DanhGiaBaiViets.InsertOnSubmit(danhGia);
            db.SubmitChanges();
            LoadDanhGia(1);
        }
    }

    protected void but_bansanphamnay_Click(object sender, EventArgs e)
    {
        Helper_Tabler_cl.ShowModal(this.Page, "Tính năng bán chéo đã được ẩn trên AhaSale.", "Thông báo", true, "warning");
        return;

        using (dbDataContext db = new dbDataContext())
        {
            var q = db.BanSanPhamNay_tbs.FirstOrDefault(p => p.idsp == ViewState["idsp"].ToString() && p.taikhoan_ban == ViewState["taikhoan"].ToString());
            if (q == null)
            {
                BanSanPhamNay_tb _ob = new BanSanPhamNay_tb();
                _ob.idsp = ViewState["idsp"].ToString();
                _ob.ban_ngungban = true;
                _ob.ngaythem = AhaTime_cl.Now;
                _ob.taikhoan_ban = ViewState["taikhoan"].ToString();
                _ob.taikhoan_goc = db.BaiViet_tbs.First(p => p.id.ToString() == ViewState["idsp"].ToString()).nguoitao;
                db.BanSanPhamNay_tbs.InsertOnSubmit(_ob);
                db.SubmitChanges();

                // ✅ đổi thông báo sang Tabler
                Helper_Tabler_cl.ShowToast(this.Page, "Xử lý thành công", null, true, 2000, "Thông báo");
            }
            else
            {
                // ✅ đổi dialog sang Tabler
                Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm này đã được thêm vào cửa hàng của bạn.", "Thông báo", true, "warning");
            }
        }
    }

    #region trao đổi (đặt hàng)

    // ===================== SỐ LƯỢNG =====================
    protected void txt_soluong2_TextChanged(object sender, EventArgs e)
    {
        int sl = Number_cl.Check_Int(txt_soluong2.Text.Trim());
        if (sl <= 0) sl = 0;

        decimal gia = 0m;
        decimal.TryParse(ViewState["giaban"].ToString(), out gia);

        Literal11.Text = (sl * gia).ToString("#,##0");
    }

    protected string BuildExchangePageUrl(string idsp, int soLuong, string userBanCheo)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["idsp"] = idsp;
        query["qty"] = (soLuong <= 0 ? 1 : soLuong).ToString();
        if (!string.IsNullOrWhiteSpace(userBanCheo))
            query["user_bancheo"] = userBanCheo;
        query["return_url"] = (Request.RawUrl ?? "/");
        if (PortalRequest_cl.IsShopPortalRequest())
            query["shop_portal"] = "1";

        return "/home/trao-doi.aspx?" + query.ToString();
    }

    // ===================== MỞ FORM TRAO ĐỔI =====================
    protected void but_traodoi_Click(object sender, EventArgs e)
    {
        int sl = Number_cl.Check_Int((txt_soluong1.Text ?? "").Trim());
        if (sl <= 0) sl = 1;

        string idsp = (ViewState["idsp"] ?? "").ToString();
        if (string.IsNullOrEmpty(idsp))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Không xác định được sản phẩm.", "Thông báo", true, "warning");
            return;
        }

        string userBanCheo = (ViewState["user_bancheo"] ?? "").ToString();
        Response.Redirect(BuildExchangePageUrl(idsp, sl, userBanCheo), true);
    }

    // ===================== ĐÓNG FORM =====================
    protected void but_close_form_dathang_Click(object sender, EventArgs e)
    {
        pn_dathang.Visible = false;
        up_dathang.Update();
    }

    // ===================== ĐẶT HÀNG (FULL LOGIC 2 VÍ) =====================
    protected void but_dathang_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string tk = ViewState["taikhoan"]?.ToString();
            if (string.IsNullOrEmpty(tk))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Bạn cần đăng nhập.", "Thông báo", true, "warning");
                return;
            }

            int sl = Number_cl.Check_Int(txt_soluong2.Text.Trim());
            if (sl <= 0)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Số lượng không hợp lệ.", "Thông báo", true, "danger");
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


            var sp = db.BaiViet_tbs.FirstOrDefault(p =>
                p.id.ToString() == ViewState["idsp"].ToString() && p.bin == false);

            if (sp == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm đã ngừng bán.", "Thông báo", true, "danger");
                return;
            }

            decimal giaVND = sp.giaban ?? 0m;
            decimal tongVND = giaVND * sl;

            int phanTramUuDai = sp.PhanTram_GiamGia_ThanhToan_BangEvoucher ?? 0;
            if (phanTramUuDai < 0) phanTramUuDai = 0;
            if (phanTramUuDai > 50) phanTramUuDai = 50;

            decimal canTraA_Tong = QuyDoi_VND_To_A(tongVND);

            decimal tienUuDaiVND = tongVND * phanTramUuDai / 100m;
            decimal A_UuDai = QuyDoi_VND_To_A(tienUuDaiVND);

            bool apDungUuDai = phanTramUuDai > 0 && A_UuDai > 0 && soDuUuDaiA >= A_UuDai;

            decimal A_ConLai = apDungUuDai ? canTraA_Tong - A_UuDai : canTraA_Tong;
            if (A_ConLai < 0) A_ConLai = 0m;

            if (apDungUuDai)
            {
                if (A_ConLai > soDuA)
                {
                    Helper_Tabler_cl.ShowModal(
                        this.Page,
                        $"Bạn đủ ví ưu đãi nhưng Đồng A không đủ.<br/>Cần <b>{A_ConLai:#,##0.##} A</b>, bạn đang có <b>{soDuA:#,##0.##} A</b>.",
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
                        $"Quyền tiêu dùng không đủ.<br/>Cần <b>{canTraA_Tong:#,##0.##} A</b> (≈ {tongVND:#,##0}đ), bạn đang có <b>{soDuA:#,##0.##} A</b>.",
                        "Thông báo",
                        true,
                        "danger"
                    );
                    return;
                }
            }

            DateTime now = AhaTime_cl.Now;

            // ===================== ĐƠN HÀNG =====================
            DonHang_tb dh = new DonHang_tb();
            dh.ngaydat = now;
            dh.nguoimua = tk;
            dh.nguoiban = sp.nguoitao;
            dh.order_status = DonHangStateMachine_cl.Order_DaDat;
            dh.exchange_status = DonHangStateMachine_cl.Exchange_ChuaTraoDoi;
            dh.tongtien = tongVND;
            dh.hoten_nguoinhan = txt_hoten_nguoinhan.Text.Trim();
            dh.sdt_nguoinhan = txt_sdt_nguoinhan.Text.Trim();
            dh.diahchi_nguoinhan = txt_diachi_nguoinhan.Text.Trim();
            dh.online_offline = true;
            dh.chothanhtoan = false;
            DonHangStateMachine_cl.SyncLegacyStatus(dh);

            db.DonHang_tbs.InsertOnSubmit(dh);
            db.SubmitChanges();

            string id_dh = dh.id.ToString();

            // ===================== CHI TIẾT =====================
            DonHang_ChiTiet_tb ct = new DonHang_ChiTiet_tb();
            ct.id_donhang = id_dh;
            ct.idsp = sp.id.ToString();
            ct.nguoiban_goc = dh.nguoiban;
            ct.nguoiban_danglai = ViewState["user_bancheo"].ToString();
            ct.soluong = sl;
            ct.giaban = giaVND;
            ct.thanhtien = tongVND;
            ct.PhanTram_GiamGia_ThanhToan_BangEvoucher = phanTramUuDai;

            db.DonHang_ChiTiet_tbs.InsertOnSubmit(ct);

            // ===================== LỊCH SỬ & TRỪ VÍ =====================
            if (apDungUuDai)
            {
                db.LichSu_DongA_tbs.InsertOnSubmit(new LichSu_DongA_tb
                {
                    taikhoan = tk,
                    dongA = A_UuDai,
                    ngay = now,
                    CongTru = false,
                    id_donhang = id_dh,
                    LoaiHoSo_Vi = 2,
                    ghichu = $"Ưu đãi {phanTramUuDai}% đơn {id_dh}: {A_UuDai:#,##0.##}A"
                });

                q_tk.Vi1That_Evocher_30PhanTram -= A_UuDai;


                db.LichSu_DongA_tbs.InsertOnSubmit(new LichSu_DongA_tb
                {
                    taikhoan = tk,
                    dongA = A_ConLai,
                    ngay = now,
                    CongTru = false,
                    id_donhang = id_dh,
                    LoaiHoSo_Vi = 1,
                    ghichu = $"Trao đổi đơn {id_dh} (còn lại): {A_ConLai:#,##0.##}A"
                });

                q_tk.DongA -= A_ConLai;
            }
            else
            {
                db.LichSu_DongA_tbs.InsertOnSubmit(new LichSu_DongA_tb
                {
                    taikhoan = tk,
                    dongA = canTraA_Tong,
                    ngay = now,
                    CongTru = false,
                    id_donhang = id_dh,
                    LoaiHoSo_Vi = 1,
                    ghichu = $"Trao đổi đơn {id_dh}: {canTraA_Tong:#,##0.##}A"
                });

                q_tk.DongA -= canTraA_Tong;
            }

            // ===================== THÔNG BÁO CHO NGƯỜI BÁN (KHÔNG TRÙNG) =====================
            string buyerName = (q_tk != null && !string.IsNullOrEmpty(q_tk.hoten)) ? q_tk.hoten : tk;

            // người nhận: người bán thực tế theo đơn (ưu tiên dh.nguoiban)
            string nguoiNhan = dh.nguoiban;

            // link người bán xem đơn
            string linkSeller = "/home/don-ban.aspx";

            // nội dung theo yêu cầu: "Họ tên vừa trao đổi sản phẩm của bạn. ID: + id"
            string noiDungTb = buyerName + " vừa trao đổi sản phẩm của bạn. ID: " + id_dh;

            // check đã có thông báo cho đúng đơn này chưa
            bool daCoThongBao = db.ThongBao_tbs.Any(x =>
                x.bin == false
                && x.nguoinhan == nguoiNhan
                && x.link == linkSeller
                && x.noidung.Contains("ID: " + id_dh) // hoặc Contains("ID đơn hàng: " + id_dh) nếu bạn muốn format cũ
            );

            if (!daCoThongBao)
            {
                db.ThongBao_tbs.InsertOnSubmit(new ThongBao_tb
                {
                    id = Guid.NewGuid(),
                    daxem = false,
                    nguoithongbao = tk,   // người tạo = người mua
                    nguoinhan = nguoiNhan,
                    link = linkSeller,
                    noidung = noiDungTb,
                    thoigian = now,
                    bin = false
                });
            }


            db.SubmitChanges();

            pn_dathang.Visible = false;

            Helper_Tabler_cl.ShowModal(
                this.Page,
                $"Đặt hàng thành công.<br/>ID đơn hàng: <b>{id_dh}</b>",
                "Thông báo",
                true,
                "success"
            );
        }
    }

    #endregion


    protected void but_themvaogio_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            bool allStopped = !db.BaiViet_tbs.Any(p => p.id.ToString() == ViewState["idsp"].ToString() && p.bin == false);
            if (allStopped)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Có sản phẩm đã ngừng bán", "Thông báo", true, "danger");
                return;
            }

            var q = db.GioHang_tbs.FirstOrDefault(p => p.idsp == ViewState["idsp"].ToString() && p.taikhoan == ViewState["taikhoan"].ToString());
            if (q != null)
            {
                q.soluong = q.soluong + int.Parse(txt_soluong1.Text.Trim());
                q.ngaythem = AhaTime_cl.Now;

                if (q.nguoiban_goc != ViewState["user_bancheo"].ToString())
                    q.nguoiban_danglai = ViewState["user_bancheo"].ToString();

                db.SubmitChanges();

                txt_soluong1.Text = "1";

                // ✅ đổi thông báo sang Tabler
                Helper_Tabler_cl.ShowToast(this.Page, "Xử lý thành công", null, true, 2000, "Thông báo");
            }
            else
            {
                GioHang_tb _ob = new GioHang_tb();
                _ob.ngaythem = AhaTime_cl.Now;
                _ob.taikhoan = ViewState["taikhoan"].ToString();
                _ob.idsp = ViewState["idsp"].ToString();
                _ob.soluong = int.Parse(txt_soluong1.Text.Trim());
                _ob.nguoiban_goc = db.BaiViet_tbs.First(p => p.id.ToString() == ViewState["idsp"].ToString()).nguoitao;
                _ob.nguoiban_danglai = "";

                if (_ob.nguoiban_goc != ViewState["user_bancheo"].ToString())
                    _ob.nguoiban_danglai = ViewState["user_bancheo"].ToString();

                db.GioHang_tbs.InsertOnSubmit(_ob);
                db.SubmitChanges();

                txt_soluong1.Text = "1";

                // ✅ đổi thông báo sang Tabler
                Helper_Tabler_cl.ShowToast(this.Page, "Xử lý thành công", null, true, 2000, "Thông báo");
            }
        }
    }
}
