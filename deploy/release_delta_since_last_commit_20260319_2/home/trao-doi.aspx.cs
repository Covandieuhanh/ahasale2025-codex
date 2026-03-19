using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

public partial class home_trao_doi : System.Web.UI.Page
{
    private const decimal VND_PER_A = 1000m;
    private const int ServiceDefaultDurationMinutes = 60;
    private const int ServiceMinDurationMinutes = 15;
    private const int ServiceMaxDurationMinutes = 480;

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

    private bool IsServiceMode()
    {
        return string.Equals((ViewState["is_service"] ?? "").ToString(), "1", StringComparison.Ordinal);
    }

    private DateTime ParseDateTimeLocalInput(string raw)
    {
        DateTime dt;
        string value = (raw ?? "").Trim();
        if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
            return dt;
        if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out dt))
            return dt;
        return DateTime.MinValue;
    }

    private string BuildPostDetailUrl(BaiViet_tb post)
    {
        if (post == null)
            return "/";

        string slug = (post.name_en ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(slug))
        {
            slug = (post.name ?? "").Trim().ToLowerInvariant();
            slug = Regex.Replace(slug, @"[^a-z0-9\-]+", "-").Trim('-');
        }
        if (string.IsNullOrEmpty(slug))
            slug = "san-pham";

        return "/" + slug + "-" + post.id.ToString() + ".html";
    }

    private void RedirectServiceNotice(BaiViet_tb post)
    {
        string target = BuildPostDetailUrl(post);
        if (string.IsNullOrWhiteSpace(target))
            target = GetBackUrl();

        string joiner = target.Contains("?") ? "&" : "?";
        Response.Redirect(target + joiner + "service_notice=1", true);
    }

    private string BuildServiceOrderAddress()
    {
        DateTime slotStart = DateTime.MinValue;
        int durationMinutes = ServiceDefaultDurationMinutes;
        string branch = "";
        string note = "";

        if (ViewState["service_slot"] != null && ViewState["service_slot"] is DateTime)
            slotStart = (DateTime)ViewState["service_slot"];
        if (slotStart == DateTime.MinValue)
            slotStart = ParseDateTimeLocalInput(txt_service_datetime.Text);

        if (ViewState["service_duration"] != null)
            durationMinutes = ClampInt(Number_cl.Check_Int(ViewState["service_duration"].ToString()), ServiceMinDurationMinutes, ServiceMaxDurationMinutes);
        else
            durationMinutes = ClampInt(Number_cl.Check_Int(txt_service_duration.Text), ServiceMinDurationMinutes, ServiceMaxDurationMinutes);

        if (ViewState["service_branch"] != null)
            branch = (ViewState["service_branch"] ?? "").ToString();
        if (string.IsNullOrWhiteSpace(branch))
            branch = ((ddl_service_branch.SelectedValue ?? "").Trim() != "" ? ddl_service_branch.SelectedValue : (ddl_service_branch.SelectedItem == null ? "" : (ddl_service_branch.SelectedItem.Text ?? ""))).Trim();
        if (string.IsNullOrWhiteSpace(branch))
            branch = "Liên hệ gian hàng đối tác xác nhận";

        if (ViewState["service_note"] != null)
            note = (ViewState["service_note"] ?? "").ToString().Trim();
        else
            note = (txt_service_note.Text ?? "").Trim();

        string text = string.Format("Lịch hẹn: {0:dd/MM/yyyy HH:mm} | Thời lượng: {1} phút | Cơ sở: {2}",
            slotStart == DateTime.MinValue ? AhaTime_cl.Now : slotStart,
            durationMinutes,
            branch);
        if (!string.IsNullOrEmpty(note))
            text += " | Ghi chú: " + note;
        return text;
    }

    private bool TryParseServiceBookingFromAddress(string raw, out DateTime slotStart, out int durationMinutes, out string branch)
    {
        slotStart = DateTime.MinValue;
        durationMinutes = 0;
        branch = "";

        string text = (raw ?? "").Trim();
        if (string.IsNullOrEmpty(text))
            return false;

        Match m = Regex.Match(
            text,
            @"Lịch hẹn:\s*(?<dt>\d{2}/\d{2}/\d{4}\s+\d{2}:\d{2})\s*\|\s*Thời lượng:\s*(?<dur>\d+)\s*phút\s*\|\s*Cơ sở:\s*(?<branch>[^|]+)",
            RegexOptions.IgnoreCase);
        if (!m.Success)
            return false;

        DateTime dt;
        if (!DateTime.TryParseExact(m.Groups["dt"].Value.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
            return false;

        int dur;
        if (!int.TryParse(m.Groups["dur"].Value.Trim(), out dur))
            return false;

        slotStart = dt;
        durationMinutes = ClampInt(dur, ServiceMinDurationMinutes, ServiceMaxDurationMinutes);
        branch = (m.Groups["branch"].Value ?? "").Trim();
        return true;
    }

    private bool HasServiceSlotConflict(dbDataContext db, BaiViet_tb servicePost, DateTime requestedStart, int requestedDurationMinutes, out string conflictText)
    {
        conflictText = "";
        if (db == null || servicePost == null)
            return false;

        DateTime requestedEnd = requestedStart.AddMinutes(requestedDurationMinutes);
        string serviceId = servicePost.id.ToString();
        string seller = (servicePost.nguoitao ?? "").Trim();

        var rows = (from dh in db.DonHang_tbs
                    join ct in db.DonHang_ChiTiet_tbs on dh.id.ToString() equals ct.id_donhang
                    where ct.idsp == serviceId && dh.nguoiban == seller
                    select new
                    {
                        dh.order_status,
                        dh.exchange_status,
                        dh.trangthai,
                        dh.online_offline,
                        dh.diahchi_nguoinhan
                    }).ToList();

        foreach (var row in rows)
        {
            DonHang_tb state = new DonHang_tb();
            state.order_status = row.order_status;
            state.exchange_status = row.exchange_status;
            state.trangthai = row.trangthai;
            state.online_offline = row.online_offline;
            DonHangStateMachine_cl.EnsureStateFields(state);

            if (DonHangStateMachine_cl.GetOrderStatus(state) == DonHangStateMachine_cl.Order_DaHuy)
                continue;

            DateTime bookedStart;
            int bookedDuration;
            string bookedBranch;
            if (!TryParseServiceBookingFromAddress(row.diahchi_nguoinhan, out bookedStart, out bookedDuration, out bookedBranch))
                continue;

            DateTime bookedEnd = bookedStart.AddMinutes(bookedDuration);
            bool overlap = requestedStart < bookedEnd && bookedStart < requestedEnd;
            if (overlap)
            {
                conflictText = string.Format("Khung giờ này đã có lịch vào {0:dd/MM/yyyy HH:mm} ({1} phút). Vui lòng chọn lịch khác.",
                    bookedStart, bookedDuration);
                return true;
            }
        }

        return false;
    }

    private bool ValidateServiceBooking(out DateTime slotStart, out int durationMinutes, out string branch, out string note)
    {
        slotStart = DateTime.MinValue;
        durationMinutes = ServiceDefaultDurationMinutes;
        branch = "";
        note = (txt_service_note.Text ?? "").Trim();

        slotStart = ParseDateTimeLocalInput(txt_service_datetime.Text);
        if (slotStart == DateTime.MinValue)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn thời gian sử dụng dịch vụ.", "Thông báo", true, "warning");
            return false;
        }

        if (slotStart < AhaTime_cl.Now.AddMinutes(15))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Lịch hẹn cần lớn hơn thời điểm hiện tại tối thiểu 15 phút.", "Thông báo", true, "warning");
            return false;
        }

        durationMinutes = ClampInt(Number_cl.Check_Int((txt_service_duration.Text ?? "").Trim()), ServiceMinDurationMinutes, ServiceMaxDurationMinutes);
        txt_service_duration.Text = durationMinutes.ToString();

        branch = ((ddl_service_branch.SelectedValue ?? "").Trim() != ""
            ? ddl_service_branch.SelectedValue
            : (ddl_service_branch.SelectedItem == null ? "" : (ddl_service_branch.SelectedItem.Text ?? ""))).Trim();
        if (string.IsNullOrEmpty(branch))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn cơ sở dịch vụ.", "Thông báo", true, "warning");
            return false;
        }

        ViewState["service_slot"] = slotStart;
        ViewState["service_duration"] = durationMinutes;
        ViewState["service_branch"] = branch;
        ViewState["service_note"] = note;
        return true;
    }

    private List<string> ExtractServiceBranches(BaiViet_tb post)
    {
        var list = new List<string>();
        if (post == null)
            return list;

        string linkMap = (post.LinkMap ?? "").Trim();
        if (!string.IsNullOrEmpty(linkMap))
        {
            string normalized = linkMap.Replace("\r", "\n");
            string[] tokens = normalized.Split(new[] { '\n', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string tokenRaw in tokens)
            {
                string token = (tokenRaw ?? "").Trim();
                if (string.IsNullOrEmpty(token))
                    continue;

                // URL map thường không thân thiện để hiển thị trực tiếp trong dropdown.
                if (token.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                    || token.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!list.Any(x => string.Equals(x, token, StringComparison.OrdinalIgnoreCase)))
                    list.Add(token);
            }
        }

        string area = TinhThanhDisplay_cl.Format((post.ThanhPho ?? "").Trim());
        if (!string.IsNullOrWhiteSpace(area))
        {
            string fromArea = "Cơ sở tại " + area;
            if (!list.Any(x => string.Equals(x, fromArea, StringComparison.OrdinalIgnoreCase)))
                list.Add(fromArea);
        }

        if (list.Count == 0 && !string.IsNullOrWhiteSpace(linkMap))
            list.Add("Cơ sở theo bản đồ gian hàng đối tác");

        if (list.Count == 0)
            list.Add("Liên hệ gian hàng đối tác xác nhận địa điểm");

        return list;
    }

    private void BindServiceBranchOptions(BaiViet_tb post)
    {
        ddl_service_branch.Items.Clear();
        ddl_service_branch.Items.Add(new System.Web.UI.WebControls.ListItem("-- Chọn cơ sở dịch vụ --", ""));

        List<string> branches = ExtractServiceBranches(post);
        foreach (string branch in branches)
            ddl_service_branch.Items.Add(new System.Web.UI.WebControls.ListItem(branch, branch));

        if (ddl_service_branch.Items.Count == 2)
            ddl_service_branch.SelectedIndex = 1;
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
        if (IsServiceMode())
            sl = 1;
        else
            sl = ClampInt(sl, 1, 999);
        txt_soluong.Text = sl.ToString();

        decimal giaVND = 0m;
        if (ViewState["gia_vnd"] != null)
            decimal.TryParse(ViewState["gia_vnd"].ToString(), out giaVND);

        decimal tongVND = giaVND * sl;
        lb_tong_vnd.Text = tongVND.ToString("#,##0");
        lb_tong_vnd_footer.Text = tongVND.ToString("#,##0");
        lb_tong_a.Text = QuyDoi_VND_To_A(tongVND).ToString("#,##0.##");
    }

    private string BuildReceiverAddress()
    {
        if (IsServiceMode())
            return BuildServiceOrderAddress();

        string tinh = (hf_tinh.Value ?? "").Trim();
        string quan = (hf_quan.Value ?? "").Trim();
        string phuong = (hf_phuong.Value ?? "").Trim();
        string chiTiet = (txt_diachi_chitiet.Text ?? "").Trim();

        if (string.IsNullOrEmpty(tinh) && string.IsNullOrEmpty(quan) && string.IsNullOrEmpty(phuong))
        {
            txt_diachi_nguoinhan.Text = chiTiet;
            return chiTiet;
        }

        string full = AddressFormat_cl.BuildFullAddress(chiTiet, phuong, quan, tinh);
        txt_diachi_nguoinhan.Text = full;
        return full;
    }

    private bool ValidateReceiverAddress()
    {
        if (IsServiceMode())
            return true;

        string tinh = (hf_tinh.Value ?? "").Trim();
        string quan = (hf_quan.Value ?? "").Trim();
        string phuong = (hf_phuong.Value ?? "").Trim();
        string chiTiet = (txt_diachi_chitiet.Text ?? "").Trim();

        bool hasRegion = !(string.IsNullOrEmpty(tinh) && string.IsNullOrEmpty(quan) && string.IsNullOrEmpty(phuong));

        if (!hasRegion)
        {
            if (chiTiet.Length < 4)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập địa chỉ chi tiết.", "Thông báo", true, "warning");
                return false;
            }
            return true;
        }

        if (string.IsNullOrEmpty(tinh) || string.IsNullOrEmpty(quan) || string.IsNullOrEmpty(phuong))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn đầy đủ Tỉnh/Thành, Quận/Huyện và Phường/Xã.", "Thông báo", true, "warning");
            return false;
        }

        if (chiTiet.Length < 4)
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng nhập địa chỉ chi tiết.", "Thông báo", true, "warning");
            return false;
        }

        return true;
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

    private void BindSavedAddresses(dbDataContext db, string taiKhoan)
    {
        if (db == null)
        {
            pnl_saved_address.Visible = false;
            return;
        }

        var list = AddressHistory_cl.GetRecentAddresses(db, taiKhoan, 5, true);
        if (list != null && list.Count > 0)
        {
            rpt_saved_address.DataSource = list;
            rpt_saved_address.DataBind();
            pnl_saved_address.Visible = true;
        }
        else
        {
            pnl_saved_address.Visible = false;
        }
    }

    protected void SavedAddress_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        if (e == null)
            return;

        string command = (e.CommandName ?? "").Trim().ToLowerInvariant();
        if (command != "delete" && command != "set-default")
            return;

        string tk = (ViewState["taikhoan"] ?? "").ToString();
        if (string.IsNullOrEmpty(tk))
            return;

        long id;
        if (!long.TryParse((e.CommandArgument ?? "").ToString(), out id))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            if (command == "delete")
            {
                AddressHistory_cl.DeleteAddress(db, tk, id);
                Helper_Tabler_cl.ShowToast(this.Page, "Đã xoá địa chỉ.", null, true, 2000, "Thông báo");
            }
            else if (command == "set-default")
            {
                AddressHistory_cl.SetDefaultAddress(db, tk, id);
                Helper_Tabler_cl.ShowToast(this.Page, "Đã đặt làm mặc định.", null, true, 2000, "Thông báo");
            }

            BindSavedAddresses(db, tk);
        }
    }

    private bool LoadProductAndReceiver()
    {
        using (dbDataContext db = new dbDataContext())
        {
            string idsp = (ViewState["idsp"] ?? "").ToString();
            var sp = AccountVisibility_cl.FindVisibleTradePostById(db, idsp);
            if (sp == null)
            {
                but_xacnhan.Enabled = false;
                Helper_Tabler_cl.ShowModal(this.Page, "Tin đăng đã ngừng hoạt động hoặc không tồn tại.", "Thông báo", true, "warning");
                return false;
            }

            if (AccountVisibility_cl.IsServicePost(sp))
            {
                RedirectServiceNotice(sp);
                return false;
            }

            ViewState["is_service"] = "0";
            hf_is_service_mode.Value = "0";
            ph_qty_section.Visible = true;
            ph_product_address.Visible = true;
            ph_service_booking.Visible = false;
            lb_item_card_header.Text = "Sản phẩm";
            lb_item_tag.Text = "Đang mua ngay";
            lb_receiver_card_title.Text = "Thông tin nhận hàng";
            lb_page_title.Text = "Trao đổi đơn mua";
            lb_page_sub.Text = "Trình bày ưu tiên rõ thông tin như Shopee, hành động xác nhận nổi bật như TikTok.";
            lb_page_step.Text = "Bước cuối: Xác nhận trao đổi";
            but_xacnhan.Text = "Xác nhận trao đổi";

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
                string diaChi = q_tk.diachi ?? "";
                txt_diachi_nguoinhan.Text = diaChi;
                txt_diachi_chitiet.Text = diaChi;
                hf_address_raw.Value = diaChi;
            }

            BindSavedAddresses(db, tk);

            UpdateTongTienLabel();
            return true;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (txt_service_datetime != null)
            txt_service_datetime.Attributes["type"] = "datetime-local";
        if (txt_service_duration != null)
        {
            txt_service_duration.Attributes["type"] = "number";
            txt_service_duration.Attributes["min"] = ServiceMinDurationMinutes.ToString();
            txt_service_duration.Attributes["max"] = ServiceMaxDurationMinutes.ToString();
        }

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
            Helper_Tabler_cl.ShowModal(this.Page, "Không xác định được tin đăng.", "Thông báo", true, "warning");
            return;
        }

        int sl = Number_cl.Check_Int((txt_soluong.Text ?? "").Trim());
        sl = IsServiceMode() ? 1 : ClampInt(sl, 1, 999);
        txt_soluong.Text = sl.ToString();

        using (dbDataContext db = new dbDataContext())
        {
            var sp = AccountVisibility_cl.FindVisibleTradePostById(db, idsp);
            if (sp == null)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Tin đăng đã ngừng hoạt động hoặc không tồn tại.", "Thông báo", true, "warning");
                return;
            }

            if (AccountVisibility_cl.IsServicePost(sp))
            {
                RedirectServiceNotice(sp);
                return;
            }

            ViewState["is_service"] = "0";
            hf_is_service_mode.Value = "0";

            if (!ValidateReceiverAddress())
                return;

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
            dh.diahchi_nguoinhan = BuildReceiverAddress();
            dh.online_offline = true;
            dh.chothanhtoan = false;
            DonHangStateMachine_cl.SyncLegacyStatus(dh);

            AddressHistory_cl.UpsertAddress(db, tk, dh.hoten_nguoinhan, dh.sdt_nguoinhan, dh.diahchi_nguoinhan);

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

            string orderVerb = "Trao đổi";

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
                    orderVerb + " ưu đãi " + phanTramUuDai + "% đơn " + id_dh + ": " + A_UuDai.ToString("#,##0.##") + "A (≈ " + tienUuDaiVND.ToString("#,##0") + "đ, 1A=" + VND_PER_A.ToString("#,##0") + "đ)";
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
                    orderVerb + " đơn " + id_dh + " (phần còn lại): " + A_ConLai.ToString("#,##0.##") + "A (≈ " + conLaiVND.ToString("#,##0") + "đ, 1A=" + VND_PER_A.ToString("#,##0") + "đ)";
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
                    orderVerb + " đơn " + id_dh + ": " + canTraA_Tong.ToString("#,##0.##") + "A (≈ " + tongVND.ToString("#,##0") + "đ, 1A=" + VND_PER_A.ToString("#,##0") + "đ)";
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

            string _emailErr;
            ShopEmailNotify_cl.TryNotifyOrder(db, dh, ShopEmailTemplate_cl.CodeOrderCreated, "", out _emailErr);

            string successHtml = "Đặt hàng thành công. ID đơn hàng: <b>" + id_dh + "</b>";
            Helper_Tabler_cl.ShowModal(this.Page, successHtml, "Thông báo", true, "success");
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "redirect_donmua_after_create",
                "setTimeout(function(){ window.location.href='/home/don-mua.aspx'; }, 1000);", true);
        }
    }
}
