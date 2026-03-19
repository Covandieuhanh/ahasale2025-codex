using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_page_chi_tiet_bai_viet : System.Web.UI.Page
{
    private sealed class OneFlagRow
    {
        public int Flag { get; set; }
    }

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
        get
        {
            return ViewState["CurrentPage"] != null ? (int)ViewState["CurrentPage"] : 1;
        }
        set
        {
            ViewState["CurrentPage"] = value;
        }
    }

    private string BuildQuickInfoHtml(string category, string area, DateTime? createdAt)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(category))
        {
            sb.Append("<span class='product-chip'>Danh mục: ");
            sb.Append(HttpUtility.HtmlEncode(category));
            sb.Append("</span>");
        }
        if (!string.IsNullOrWhiteSpace(area))
        {
            sb.Append("<span class='product-chip'>Khu vực: ");
            sb.Append(HttpUtility.HtmlEncode(area));
            sb.Append("</span>");
        }
        if (createdAt.HasValue)
        {
            sb.Append("<span class='product-chip'>Ngày đăng: ");
            sb.Append(createdAt.Value.ToString("dd/MM/yyyy"));
            sb.Append("</span>");
        }
        return sb.ToString();
    }

    private void BindRelatedProducts(dbDataContext db, BaiViet_tb current)
    {
        if (db == null || current == null)
            return;

        string currentCategory = (current.id_DanhMuc ?? "").Trim();
        string seller = (current.nguoitao ?? "").Trim();

        var baseQuery = AccountVisibility_cl.FilterVisibleTradePosts(db, db.BaiViet_tbs)
            .Where(p => p.id != current.id && (p.bin == false || p.bin == null));

        string currentType = (current.phanloai ?? "").Trim();
        if (!string.IsNullOrEmpty(currentType))
            baseQuery = baseQuery.Where(p => p.phanloai == currentType);

        if (!string.IsNullOrEmpty(currentCategory))
            baseQuery = baseQuery.Where(p => p.id_DanhMuc == currentCategory || p.nguoitao == seller);
        else
            baseQuery = baseQuery.Where(p => p.nguoitao == seller);

        var related = baseQuery
            .OrderByDescending(p => p.ngaytao)
            .Take(6)
            .Select(p => new
            {
                p.id,
                p.name,
                p.name_en,
                p.image,
                p.giaban
            })
            .ToList()
            .Select(p => new
            {
                Id = p.id,
                Name = p.name,
                Url = BuildProductDetailUrl(p.id, p.name_en, p.name),
                Image = ResolveMediaUrlOrFallback(p.image, "/uploads/images/macdinh.jpg"),
                Price = (p.giaban ?? 0m).ToString("#,##0.##")
            })
            .ToList();

        ph_related_products.Visible = related.Count > 0;
        rpt_related_products.DataSource = related;
        rpt_related_products.DataBind();
    }

    private string BuildProductDetailUrl(int id, string slugRaw, string nameRaw)
    {
        if (id <= 0) return "#";
        string slug = (slugRaw ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(slug))
        {
            slug = (nameRaw ?? "").Trim().ToLowerInvariant();
            slug = Regex.Replace(slug, @"[^a-z0-9\-]+", "-").Trim('-');
        }
        if (string.IsNullOrEmpty(slug))
            slug = "san-pham";
        return "/" + slug + "-" + id.ToString() + ".html";
    }

    private string RenderMainMediaHtml(string mediaUrl, string title)
    {
        const string fallback = "/uploads/images/macdinh.jpg";
        bool isVideo = MediaFile_cl.IsVideo(mediaUrl);
        string safeUrl = ResolveMediaUrlOrFallback(mediaUrl, fallback);
        string safeAlt = MediaFile_cl.GetSafeText(title);

        if (isVideo && !string.Equals(safeUrl, fallback, StringComparison.OrdinalIgnoreCase))
        {
            string mime = MediaFile_cl.GetVideoMime(mediaUrl);
            return "<video controls playsinline preload='metadata' style='max-width:100%;max-height:100%;object-fit:contain'>" +
                   "<source src='" + safeUrl + "' type='" + mime + "' />" +
                   "</video>";
        }

        return "<img src='" + safeUrl + "' alt='" + safeAlt + "' class='product-main-media zoomable' style='max-width:100%;max-height:100%;object-fit:contain' />";
    }

    private string RenderThumbMediaHtml(string mediaUrl)
    {
        const string fallback = "/uploads/images/macdinh.jpg";
        bool isVideo = MediaFile_cl.IsVideo(mediaUrl);
        string safeUrl = ResolveMediaUrlOrFallback(mediaUrl, fallback);

        string encodedForJs = HttpUtility.JavaScriptStringEncode(safeUrl);
        if (isVideo && !string.Equals(safeUrl, fallback, StringComparison.OrdinalIgnoreCase))
        {
            string mime = MediaFile_cl.GetVideoMime(mediaUrl);
            return "<video muted playsinline preload='metadata' onclick=\"changeMainMedia('" + encodedForJs + "','video')\">" +
                   "<source src='" + safeUrl + "' type='" + mime + "' />" +
                   "</video>";
        }

        return "<img src='" + safeUrl + "' loading='lazy' decoding='async' onclick=\"changeMainMedia('" + encodedForJs + "','image')\" />";
    }

    private string ResolveMediaUrlOrFallback(string mediaUrl, string fallback)
    {
        string safeUrl = MediaFile_cl.GetSafeUrl(mediaUrl);
        if (string.IsNullOrWhiteSpace(safeUrl))
            return fallback;

        if (safeUrl.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
            return fallback;

        Uri absolute;
        if (Uri.TryCreate(safeUrl, UriKind.Absolute, out absolute))
        {
            if (string.Equals(absolute.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                || string.Equals(absolute.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                return absolute.AbsoluteUri;
            return fallback;
        }

        string localPath = safeUrl;
        if (localPath.StartsWith("~/", StringComparison.Ordinal))
            localPath = localPath.Substring(1);
        if (!localPath.StartsWith("/", StringComparison.Ordinal))
            localPath = "/" + localPath;

        if (IsMissingUploadFile(localPath))
            return fallback;

        return localPath;
    }

    private bool IsMissingUploadFile(string relativeUrl)
    {
        return Helper_cl.IsMissingUploadFile(relativeUrl);
    }

    private string SanitizeContentHtml(string rawHtml)
    {
        if (string.IsNullOrEmpty(rawHtml))
            return "";

        string sanitized = Regex.Replace(
            rawHtml,
            "src\\s*=\\s*([\\\"'])(?<url>[^\\\"']+)\\1",
            delegate(Match match)
            {
                Group urlGroup = match.Groups["url"];
                if (!urlGroup.Success)
                    return match.Value;

                string resolved = ResolveMediaUrlOrFallback(urlGroup.Value, "/uploads/images/macdinh.jpg");
                return "src=\"" + HttpUtility.HtmlAttributeEncode(resolved) + "\"";
            },
            RegexOptions.IgnoreCase);

        sanitized = Regex.Replace(
            sanitized,
            "href\\s*=\\s*([\\\"'])(?<url>[^\\\"']+)\\1",
            delegate(Match match)
            {
                Group urlGroup = match.Groups["url"];
                if (!urlGroup.Success)
                    return match.Value;

                string resolved = ResolveMediaUrlOrFallback(urlGroup.Value, "");
                if (string.IsNullOrEmpty(resolved))
                    resolved = "#";
                return "href=\"" + HttpUtility.HtmlAttributeEncode(resolved) + "\"";
            },
            RegexOptions.IgnoreCase);

        return sanitized;
    }

    protected string ResolveSafeImage(object imageRaw)
    {
        return ResolveMediaUrlOrFallback((imageRaw ?? "").ToString(), "/uploads/images/macdinh.jpg");
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
                var q = AccountVisibility_cl.FindVisibleTradePostById(db, ViewState["idsp"].ToString());
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

                bool isService = AccountVisibility_cl.IsServicePost(q);
                ViewState["is_service"] = isService ? "1" : "0";
                ph_qty_block.Visible = !isService;
                ph_add_to_cart.Visible = !isService;
                ph_mobile_addcart.Visible = !isService;
                but_traodoi.Text = isService ? "Tạo đơn" : "Trao đổi";
                lit_mobile_traodoi.Text = isService ? "Tạo đơn" : "Trao đổi";
                ph_booking.Visible = isService;
                ph_mobile_booking.Visible = isService;

                if (isService)
                {
                    string shopAccount = (q.nguoitao ?? "").Trim().ToLowerInvariant();
                    string returnUrl = Request.Url.PathAndQuery;
                    string bookingUrl = "/home/dat-lich.aspx?user=" + HttpUtility.UrlEncode(shopAccount)
                        + "&id=" + HttpUtility.UrlEncode(q.id.ToString())
                        + "&return_url=" + HttpUtility.UrlEncode(returnUrl);
                    lnk_datlich.NavigateUrl = bookingUrl;
                    lnk_mobile_datlich.HRef = bookingUrl;
                }

                if (isService && string.Equals((Request.QueryString["service_notice"] ?? "").Trim(), "1", StringComparison.Ordinal))
                {
                    Helper_Tabler_cl.ShowModal(this.Page,
                        "Dịch vụ có thể đặt lịch trực tiếp tại Aha Sale. Vui lòng chọn Đặt lịch.",
                        "Thông báo",
                        true,
                        "warning");
                }

                string thanhPhoRaw = string.IsNullOrWhiteSpace(q.ThanhPho) ? "Không có" : q.ThanhPho;
                string thanhPho = TinhThanhDisplay_cl.Format(thanhPhoRaw);
                string mapLink = NormalizeMapLink(q.LinkMap);
                if (!string.IsNullOrWhiteSpace(mapLink))
                {
                    Label2.Text = string.Format("<a href='{0}' target='_blank' class='text-muted'>", mapLink) +
                                  string.Format("<i class='ti ti-map-pin'></i> {0}</a>", thanhPho);
                }
                else
                {
                    Label2.Text = string.Format("<span class='text-muted'><i class='ti ti-map-pin'></i> {0}</span>", thanhPho);
                }

                if (isService && !string.IsNullOrWhiteSpace(mapLink))
                {
                    ph_map_link.Visible = true;
                    lnk_map.NavigateUrl = mapLink;
                }
                else
                {
                    ph_map_link.Visible = false;
                }

                TimeSpan timeAgo = AhaTime_cl.Now - q.ngaytao.Value;
                if (timeAgo.TotalDays >= 1)
                    Label6.Text = string.Format("Tạo {0} ngày trước", (int)timeAgo.TotalDays);
                else if (timeAgo.TotalHours >= 1)
                    Label6.Text = string.Format("Tạo {0} giờ trước", (int)timeAgo.TotalHours);
                else if (timeAgo.TotalMinutes >= 1)
                    Label6.Text = string.Format("Tạo {0} phút trước", (int)timeAgo.TotalMinutes);
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

                string categoryName = "";
                if (!string.IsNullOrWhiteSpace(q.id_DanhMuc))
                {
                    int catId;
                    if (int.TryParse(q.id_DanhMuc, out catId))
                    {
                        var cat = db.DanhMuc_tbs.FirstOrDefault(p => p.id == catId);
                        if (cat != null)
                            categoryName = (cat.name ?? "").Trim();
                    }
                }

                string quickInfoHtml = BuildQuickInfoHtml(categoryName, thanhPho, q.ngaytao);
                lb_quick_info.Text = quickInfoHtml;
                lb_quick_info.Visible = !string.IsNullOrWhiteSpace(quickInfoHtml);

                Label4.Text = SanitizeContentHtml(q.content_post);
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
                string imageUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, imageRelativePath);

                string metaTags = string.Format(@"
                    <title>{0}</title>
                    <meta name='description' content='{1}' />
                    <meta property='og:title' content='{2}' />
                    <meta property='og:description' content='{3}' />
                    <meta property='og:image' content='{4}' />
                    <meta property='og:type' content='website' />
                    <meta property='og:url' content='{5}' />
                    <meta name='twitter:card' content='summary_large_image' />
                    <meta name='twitter:title' content='{6}' />
                    <meta name='twitter:description' content='{7}' />
                    <meta name='twitter:image' content='{8}' />
                ", title, description, title, description, imageUrl, Request.Url.AbsoluteUri, title, description, imageUrl);
                literal_meta.Text = metaTags;
                #endregion

                #region xử lý người bán
                string url = Request.RawUrl;
                string[] parts = url.Split('-');
                string _tk_ban = parts[parts.Length - 2];

                if (!string.IsNullOrEmpty(_tk_ban))
                {
                    var q_check = db.BanSanPhamNay_tbs.FirstOrDefault(p => p.taikhoan_ban == _tk_ban && p.idsp == ViewState["idsp"].ToString());
                    if (q_check != null && AccountVisibility_cl.IsSellerVisible(db, _tk_ban))
                    {
                        ViewState["nguoiban"] = _tk_ban;
                        ViewState["user_bancheo"] = _tk_ban;
                    }
                }

                var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["nguoiban"].ToString());
                if (q_tk != null)
                {
                    ViewState["avt_query"] = ResolveMediaUrlOrFallback(q_tk.anhdaidien, "/uploads/images/macdinh.jpg");
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
                string _tk1 = PortalActiveMode_cl.IsHomeActive() ? (Session["taikhoan_home"] as string) : "";

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
                phMobileCta.Visible = PlaceHolder1.Visible;
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

                BindRelatedProducts(db, q);
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

    private bool HasCompletedOrderForBuyerProduct(dbDataContext db, string taiKhoanMua, string idBaiViet)
    {
        if (db == null) return false;
        string tk = (taiKhoanMua ?? "").Trim();
        if (tk == "") return false;
        string idsp = (idBaiViet ?? "").Trim();
        if (idsp == "") return false;

        try
        {
            var orders = (from dh in db.DonHang_tbs
                          join ct in db.DonHang_ChiTiet_tbs on dh.id.ToString() equals ct.id_donhang
                          where dh.nguoimua == tk && ct.idsp == idsp
                          select new
                          {
                              dh.trangthai,
                              dh.order_status,
                              dh.exchange_status,
                              dh.online_offline
                          }).ToList();

            foreach (var o in orders)
            {
                var dh = new DonHang_tb
                {
                    trangthai = o.trangthai,
                    order_status = o.order_status,
                    exchange_status = o.exchange_status,
                    online_offline = o.online_offline
                };
                DonHangStateMachine_cl.EnsureStateFields(dh);
                string orderStatus = DonHangStateMachine_cl.GetOrderStatus(dh);
                string exchangeStatus = DonHangStateMachine_cl.GetExchangeStatus(dh);
                if (string.Equals(exchangeStatus, DonHangStateMachine_cl.Exchange_DaTraoDoi, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(orderStatus, DonHangStateMachine_cl.Order_DaNhan, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    private bool CanReviewPost(dbDataContext db, string taiKhoanMua, string idBaiViet, bool isService)
    {
        return HasCompletedOrderForBuyerProduct(db, taiKhoanMua, idBaiViet);
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
                bool isService = string.Equals(Convert.ToString(ViewState["is_service"]), "1", StringComparison.Ordinal);
                bool duocanhGia = CanReviewPost(db, taikhoan, idBaiViet, isService);
                string taiKhoanDanhGia = Convert.ToString(ViewState["taikhoan"]) ?? "";
                string nguoiBan = Convert.ToString(ViewState["nguoiban"]) ?? "";

                bool daDanhGia = false;
                if (duocanhGia)
                {
                    daDanhGia = danhSachDanhGiaVaAnh.Any(x =>
                        x.TaiKhoanDanhGia == taiKhoanDanhGia ||
                        x.ThuocTaiKhoan == nguoiBan
                    );
                }
                bool laChuTin = string.Equals(
                    Convert.ToString(ViewState["nguoiban"]) ?? "",
                    taiKhoanDanhGia,
                    StringComparison.OrdinalIgnoreCase);

                if (laChuTin)
                {
                    InputReview.Visible = false;
                    if (phReviewGate != null) phReviewGate.Visible = false;
                }
                else if (!duocanhGia)
                {
                    InputReview.Visible = false;
                    if (phReviewGate != null) phReviewGate.Visible = true;
                }
                else
                {
                    InputReview.Visible = !daDanhGia;
                    if (phReviewGate != null) phReviewGate.Visible = false;
                }
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
        using (dbDataContext db = new dbDataContext())
        {
            string taiKhoan = Convert.ToString(ViewState["taikhoan"]) ?? "";
            string idBaiViet = Convert.ToString(ViewState["idsp"]) ?? "";
            bool isService = string.Equals(Convert.ToString(ViewState["is_service"]), "1", StringComparison.Ordinal);

            if (string.IsNullOrWhiteSpace(taiKhoan) || !CanReviewPost(db, taiKhoan, idBaiViet, isService))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Bạn cần hoàn tất trao đổi sản phẩm hoặc dịch vụ này trước khi đánh giá.", "Thông báo", true, "warning");
                return;
            }
        }

        int result;
        int diem = int.TryParse(hdDiem.Value, out result) ? result : 0;
        var danhGia = new DanhGiaBaiViet
        {
            idBaiViet = Convert.ToString(ViewState["idsp"]),
            NoiDung = txtNoiDung.Text.Trim(),
            Diem = diem,
            TaiKhoanDanhGia = Convert.ToString(ViewState["taikhoan"]),
            ThuocTaiKhoan = Convert.ToString(ViewState["nguoiban"]),
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
                var sp = AccountVisibility_cl.FindVisibleProductById(db, ViewState["idsp"].ToString());
                if (sp == null)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm đã ngừng bán hoặc tài khoản đã bị khóa.", "Thông báo", true, "warning");
                    return;
                }
                _ob.taikhoan_goc = sp.nguoitao;
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

    protected string BuildExchangePageUrl(string idsp, int soLuong, string userBanCheo)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["idsp"] = idsp;
        query["qty"] = (soLuong <= 0 ? 1 : soLuong).ToString();
        if (!string.IsNullOrWhiteSpace(userBanCheo))
            query["user_bancheo"] = userBanCheo;
        query["return_url"] = (Request.RawUrl ?? "/");

        string basePath = PortalRequest_cl.IsShopPortalRequest()
            ? "/shop/trao-doi"
            : "/home/trao-doi.aspx";
        return basePath + "?" + query.ToString();
    }

    // ===================== MỞ FORM TRAO ĐỔI =====================
    protected void but_traodoi_Click(object sender, EventArgs e)
    {
        int sl = Number_cl.Check_Int((txt_soluong1.Text ?? "").Trim());
        if (sl <= 0) sl = 1;
        string idsp = (ViewState["idsp"] ?? "").ToString();
        if (string.IsNullOrEmpty(idsp))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Không xác định được tin đăng.", "Thông báo", true, "warning");
            return;
        }

        string userBanCheo = (ViewState["user_bancheo"] ?? "").ToString();
        Response.Redirect(BuildExchangePageUrl(idsp, sl, userBanCheo), true);
    }
    #endregion

    private string NormalizeMapLink(string raw)
    {
        string value = (raw ?? "").Trim();
        if (string.IsNullOrWhiteSpace(value))
            return "";

        if (value.StartsWith("//"))
            return "https:" + value;
        if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return value;

        return "https://" + value;
    }


    protected void but_themvaogio_Click(object sender, EventArgs e)
    {
        bool isService = string.Equals((ViewState["is_service"] ?? "").ToString(), "1", StringComparison.Ordinal);
        if (isService)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Dịch vụ chỉ hiển thị để tham khảo, không thể thêm vào giỏ.", "Thông báo", true, "warning");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            ShopStatus_cl.EnsureSchemaSafe(db);
            bool allStopped = !db.BaiViet_tbs.Any(p =>
                p.id.ToString() == ViewState["idsp"].ToString()
                && (p.bin == false || p.bin == null)
                && p.phanloai == "sanpham"
                && db.taikhoan_tbs.Any(acc =>
                    acc.taikhoan == p.nguoitao
                    && (((acc.phanloai == AccountVisibility_cl.ShopPartnerType
                          || ((acc.permission ?? "").ToLower()).Contains(PortalScope_cl.ScopeShop))
                         && acc.TrangThai_Shop == ShopStatus_cl.StatusApproved)
                        || (!(acc.phanloai == AccountVisibility_cl.ShopPartnerType
                              || ((acc.permission ?? "").ToLower()).Contains(PortalScope_cl.ScopeShop))
                            && acc.block != true))));
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
                var sp = AccountVisibility_cl.FindVisibleProductById(db, ViewState["idsp"].ToString());
                if (sp == null)
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Sản phẩm đã ngừng bán hoặc tài khoản đã bị khóa.", "Thông báo", true, "warning");
                    return;
                }
                _ob.nguoiban_goc = sp.nguoitao;
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
