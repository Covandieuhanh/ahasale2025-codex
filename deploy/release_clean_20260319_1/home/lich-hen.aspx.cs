using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public partial class home_lich_hen : System.Web.UI.Page
{
    protected string FilterQuery = "";
    protected string FilterStatus = "all";
    protected string FilterTime = "all";

    protected void Page_Load(object sender, EventArgs e)
    {
        ReadFilters();
        if (!IsPostBack)
        {
            HandleAction();
            BindData();
        }
    }

    private void ReadFilters()
    {
        FilterQuery = (Request.QueryString["q"] ?? "").Trim();
        FilterStatus = NormalizeStatusFilter(Request.QueryString["st"]);
        FilterTime = NormalizeTimeFilter(Request.QueryString["time"]);
    }

    private void HandleAction()
    {
        string action = (Request.QueryString["action"] ?? "").Trim().ToLowerInvariant();
        if (action != "cancel")
            return;

        long bookingId;
        if (!long.TryParse((Request.QueryString["id"] ?? "").Trim(), out bookingId))
            return;

        string phone = ResolveHomePhone();
        if (string.IsNullOrWhiteSpace(phone))
        {
            Session["home_booking_notice"] = "Vui lòng đăng nhập để thao tác lịch hẹn.";
            Response.Redirect("/home/lich-hen.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            bspa_datlich_table booking = db.bspa_datlich_tables.FirstOrDefault(p => p.id == bookingId && p.sdt == phone);
            if (booking == null)
            {
                Session["home_booking_notice"] = "Không tìm thấy lịch hẹn cần hủy.";
                Response.Redirect("/home/lich-hen.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            string status = datlich_class.chuanhoa_trangthai(booking.trangthai);
            if (status == datlich_class.trangthai_da_huy || status == datlich_class.trangthai_da_den || status == datlich_class.trangthai_khong_den)
            {
                Session["home_booking_notice"] = "Lịch hẹn này không thể hủy.";
                Response.Redirect("/home/lich-hen.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (booking.ngaydat.HasValue && booking.ngaydat.Value < DateTime.Now)
            {
                Session["home_booking_notice"] = "Lịch hẹn đã qua thời gian hẹn nên không thể hủy.";
                Response.Redirect("/home/lich-hen.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            booking.trangthai = datlich_class.trangthai_da_huy;
            db.SubmitChanges();

            TryNotifyShopCancel(db, booking, phone);

            Session["home_booking_notice"] = "Bạn đã hủy lịch hẹn #" + bookingId.ToString() + ".";
            Response.Redirect("/home/lich-hen.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }

    private void BindData()
    {
        string phone = ResolveHomePhone();
        lb_phone.Text = phone;

        if (string.IsNullOrWhiteSpace(phone))
        {
            ph_require_login.Visible = true;
            ph_empty.Visible = false;
            rpt_bookings.Visible = false;
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            var bookings = db.bspa_datlich_tables
                .Where(p => p.sdt != null && p.sdt == phone)
                .ToList();

            bookings = bookings
                .OrderByDescending(p => p.ngaydat.HasValue ? p.ngaydat.Value : (p.ngaytao.HasValue ? p.ngaytao.Value : DateTime.Now))
                .Take(200)
                .ToList();

            if (bookings.Count == 0)
            {
                ph_require_login.Visible = false;
                ph_empty.Visible = true;
                rpt_bookings.Visible = false;
                return;
            }

            var branchIds = bookings
                .Select(p => (p.id_chinhanh ?? "").Trim())
                .Where(p => p != "")
                .Distinct()
                .ToList();

            var branches = branchIds.Count == 0
                ? new List<chinhanh_table>()
                : db.chinhanh_tables.Where(p => branchIds.Contains(p.id.ToString())).ToList();

            var branchMap = branches.ToDictionary(p => p.id.ToString(), p => p);

            var shopAccounts = branches
                .Select(p => (p.taikhoan_quantri ?? "").Trim().ToLowerInvariant())
                .Where(p => p != "")
                .Distinct()
                .ToList();

            var shops = shopAccounts.Count == 0
                ? new List<taikhoan_tb>()
                : db.taikhoan_tbs.Where(p => shopAccounts.Contains(p.taikhoan)).ToList();

            var shopMap = shops.ToDictionary(p => p.taikhoan, p => p);

            var rebookMap = BuildRebookMap(db, bookings);
            List<BookingRow> rows = new List<BookingRow>();
            foreach (var booking in bookings)
            {
                rows.Add(BuildRow(db, booking, branchMap, shopMap, rebookMap));
            }

            rows = ApplyFilters(rows);

            rpt_bookings.DataSource = rows;
            rpt_bookings.DataBind();

            string notice = (Session["home_booking_notice"] ?? "").ToString();
            ph_notice.Visible = !string.IsNullOrWhiteSpace(notice);
            lb_notice.Text = notice;
            Session["home_booking_notice"] = "";

            lb_count.Text = rows.Count.ToString();
            ph_require_login.Visible = false;
            ph_empty.Visible = rows.Count == 0;
            rpt_bookings.Visible = rows.Count != 0;
        }
    }

    private BookingRow BuildRow(dbDataContext db, bspa_datlich_table booking, Dictionary<string, chinhanh_table> branchMap, Dictionary<string, taikhoan_tb> shopMap, Dictionary<long, long> rebookMap)
    {
        string branchId = (booking.id_chinhanh ?? "").Trim();
        chinhanh_table branch = null;
        if (!string.IsNullOrWhiteSpace(branchId) && branchMap.ContainsKey(branchId))
            branch = branchMap[branchId];

        string shopAccount = "";
        if (branch != null)
            shopAccount = (branch.taikhoan_quantri ?? "").Trim().ToLowerInvariant();

        taikhoan_tb shop = null;
        if (!string.IsNullOrWhiteSpace(shopAccount) && shopMap.ContainsKey(shopAccount))
            shop = shopMap[shopAccount];

        string shopName = "Gian hàng đối tác";
        if (shop != null)
        {
            if (!string.IsNullOrWhiteSpace(shop.ten_shop))
                shopName = shop.ten_shop;
            else if (!string.IsNullOrWhiteSpace(shop.hoten))
                shopName = shop.hoten;
            else if (!string.IsNullOrWhiteSpace(shop.ten))
                shopName = shop.ten;
            else if (!string.IsNullOrWhiteSpace(shop.taikhoan))
                shopName = shop.taikhoan;
        }
        else if (!string.IsNullOrWhiteSpace(shopAccount))
            shopName = shopAccount;

        string branchName = branch != null && !string.IsNullOrWhiteSpace(branch.ten)
            ? branch.ten
            : (string.IsNullOrWhiteSpace(branchId) ? "Chi nhánh" : "Chi nhánh #" + branchId);

        string shopPhone = "";
        if (shop != null && !string.IsNullOrWhiteSpace(shop.dienthoai))
            shopPhone = shop.dienthoai.Trim();
        else if (branch != null && !string.IsNullOrWhiteSpace(branch.sdt))
            shopPhone = branch.sdt.Trim();

        string shopPhoneLabel = string.IsNullOrWhiteSpace(shopPhone) ? "" : "SĐT: " + shopPhone;

        string serviceName = (booking.tendichvu_taithoidiemnay ?? "").Trim();
        if (string.IsNullOrWhiteSpace(serviceName))
            serviceName = string.IsNullOrWhiteSpace(booking.dichvu) ? "Dịch vụ" : "Dịch vụ #" + booking.dichvu;

        string dateText = "";
        if (booking.ngaydat.HasValue)
        {
            if (booking.ngayketthucdukien.HasValue)
                dateText = datlich_class.return_khoang_thoi_gian_text(booking.ngaydat.Value, booking.ngayketthucdukien.Value);
            else
                dateText = booking.ngaydat.Value.ToString("HH:mm dd/MM/yyyy");
        }
        else if (booking.ngaytao.HasValue)
        {
            dateText = booking.ngaytao.Value.ToString("HH:mm dd/MM/yyyy");
        }

        string status = datlich_class.chuanhoa_trangthai(booking.trangthai);
        string statusCss = MapStatusCss(status);

        string addressLabel = branch != null && !string.IsNullOrWhiteSpace(branch.diachi)
            ? "Địa chỉ: " + branch.diachi
            : "";

        string noteLabel = string.IsNullOrWhiteSpace(booking.ghichu)
            ? ""
            : "Ghi chú: " + booking.ghichu;

        string shopUrl = "/";
        if (!string.IsNullOrWhiteSpace(shopAccount))
            shopUrl = ShopSlug_cl.GetPublicUrlByTaiKhoan(db, shopAccount);

        string rebookUrl = "";
        if (!string.IsNullOrWhiteSpace(shopAccount) && rebookMap != null)
        {
            long rawId;
            if (long.TryParse((booking.dichvu ?? "").Trim(), out rawId) && rebookMap.ContainsKey(rawId))
            {
                string query = "user=" + HttpUtility.UrlEncode(shopAccount)
                    + "&id=" + HttpUtility.UrlEncode(rebookMap[rawId].ToString())
                    + "&return_url=" + HttpUtility.UrlEncode("/home/lich-hen.aspx");
                rebookUrl = "/home/dat-lich.aspx?" + query;
            }
        }

        string cancelUrl = "";
        if (CanCancelBooking(booking))
            cancelUrl = "/home/lich-hen.aspx?action=cancel&id=" + booking.id.ToString();

        string editUrl = "";
        if (CanEditBooking(booking))
        {
            string query = "edit_id=" + HttpUtility.UrlEncode(booking.id.ToString())
                + "&return_url=" + HttpUtility.UrlEncode("/home/lich-hen.aspx");
            editUrl = "/home/dat-lich.aspx?" + query;
        }

        return new BookingRow
        {
            Id = booking.id,
            ServiceName = serviceName,
            DateText = dateText,
            StatusText = status,
            StatusCss = statusCss,
            ShopName = shopName,
            BranchName = branchName,
            ShopPhoneLabel = shopPhoneLabel,
            BranchAddressLabel = addressLabel,
            NoteLabel = noteLabel,
            ShopUrl = shopUrl,
            RebookUrl = rebookUrl,
            CancelUrl = cancelUrl,
            EditUrl = editUrl,
            RawStatus = status,
            RawDate = booking.ngaydat ?? booking.ngaytao
        };
    }

    private string ResolveHomePhone()
    {
        string phone = "";
        if (Session["user_home"] != null)
            phone = (Session["user_home"] ?? "").ToString().Trim();

        if (string.IsNullOrWhiteSpace(phone) && Request.Cookies["save_sdt_home_aka"] != null)
        {
            try
            {
                phone = encode_class.decrypt(Request.Cookies["save_sdt_home_aka"].Value);
            }
            catch
            {
                phone = "";
            }
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            string tk = "";
            if (string.IsNullOrWhiteSpace(tk))
                tk = (PortalRequest_cl.GetCurrentAccount() ?? "").Trim().ToLowerInvariant();
            if (Session["taikhoan_home"] != null)
            {
                try
                {
                    tk = mahoa_cl.giaima_Bcorn(Session["taikhoan_home"].ToString());
                }
                catch
                {
                    tk = "";
                }
            }

            if (string.IsNullOrWhiteSpace(tk))
            {
                HttpCookie ck = Request.Cookies["cookie_userinfo_home_bcorn"];
                if (ck != null && !string.IsNullOrEmpty(ck["taikhoan"]))
                {
                    try
                    {
                        tk = mahoa_cl.giaima_Bcorn(ck["taikhoan"]);
                    }
                    catch
                    {
                        tk = "";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(tk))
            {
                try
                {
                    using (dbDataContext db = new dbDataContext())
                    {
                        taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
                        if (acc != null)
                            phone = (acc.dienthoai ?? "").Trim();
                    }
                }
                catch
                {
                }

                if (string.IsNullOrWhiteSpace(phone))
                    phone = tk;
            }
        }

        return datlich_class.chuanhoa_sdt(phone);
    }

    private string MapStatusCss(string status)
    {
        switch ((status ?? "").Trim())
        {
            case datlich_class.trangthai_da_xacnhan:
                return "booking-status-confirmed";
            case datlich_class.trangthai_da_den:
                return "booking-status-done";
            case datlich_class.trangthai_da_huy:
                return "booking-status-cancelled";
            case datlich_class.trangthai_khong_den:
                return "booking-status-missed";
            default:
                return "booking-status-pending";
        }
    }

    private bool CanCancelBooking(bspa_datlich_table booking)
    {
        if (booking == null)
            return false;

        return CanEditBooking(booking);
    }

    private bool CanEditBooking(bspa_datlich_table booking)
    {
        if (booking == null)
            return false;

        string status = datlich_class.chuanhoa_trangthai(booking.trangthai);
        if (status == datlich_class.trangthai_da_huy || status == datlich_class.trangthai_da_den || status == datlich_class.trangthai_khong_den)
            return false;

        if (booking.ngaydat.HasValue && booking.ngaydat.Value < DateTime.Now)
            return false;

        return true;
    }

    private void TryNotifyShopCancel(dbDataContext db, bspa_datlich_table booking, string phone)
    {
        if (db == null || booking == null)
            return;

        string branchId = (booking.id_chinhanh ?? "").Trim();
        if (string.IsNullOrWhiteSpace(branchId))
            return;

        chinhanh_table branch = db.chinhanh_tables.FirstOrDefault(p => p.id.ToString() == branchId);
        if (branch == null || string.IsNullOrWhiteSpace(branch.taikhoan_quantri))
            return;

        try
        {
            thongbao_table notice = new thongbao_table();
            notice.id = Guid.NewGuid();
            notice.daxem = false;
            notice.nguoithongbao = "Home Public";
            notice.nguoinhan = (branch.taikhoan_quantri ?? "").Trim().ToLowerInvariant();
            notice.link = "/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx";
            notice.noidung = "Khách " + (string.IsNullOrWhiteSpace(phone) ? "Home" : phone) + " đã hủy lịch #" + booking.id.ToString();
            notice.thoigian = DateTime.Now;
            db.thongbao_tables.InsertOnSubmit(notice);
            db.SubmitChanges();
        }
        catch
        {
        }
    }

    private Dictionary<long, long> BuildRebookMap(dbDataContext db, List<bspa_datlich_table> bookings)
    {
        Dictionary<long, long> map = new Dictionary<long, long>();
        if (db == null || bookings == null || bookings.Count == 0)
            return map;

        List<long> rawIds = bookings
            .Select(p =>
            {
                long id;
                return long.TryParse((p.dichvu ?? "").Trim(), out id) ? id : 0;
            })
            .Where(p => p > 0)
            .Distinct()
            .ToList();

        if (rawIds.Count == 0)
            return map;

        string serviceType = AccountVisibility_cl.PostTypeService;

        var directServices = db.BaiViet_tbs
            .Where(p => rawIds.Contains(p.id)
                        && (p.bin == false || p.bin == null)
                        && (p.phanloai ?? "").Trim() == serviceType)
            .Select(p => (long)p.id)
            .ToList();

        HashSet<long> directSet = new HashSet<long>(directServices);

        var mirrors = db.web_post_tables
            .Where(p => rawIds.Contains(p.id) && p.id_baiviet.HasValue)
            .Select(p => new { id = p.id, id_baiviet = p.id_baiviet.Value })
            .ToList();

        var mirrorIds = mirrors.Select(p => p.id_baiviet).Distinct().ToList();
        HashSet<long> mirrorSet = new HashSet<long>();
        if (mirrorIds.Count > 0)
        {
            mirrorSet = new HashSet<long>(db.BaiViet_tbs
                .Where(p => mirrorIds.Contains(p.id)
                            && (p.bin == false || p.bin == null)
                            && (p.phanloai ?? "").Trim() == serviceType)
                .Select(p => (long)p.id)
                .ToList());
        }

        foreach (long rawId in rawIds)
        {
            if (directSet.Contains(rawId))
            {
                map[rawId] = rawId;
                continue;
            }

            var mirror = mirrors.FirstOrDefault(p => p.id == rawId);
            if (mirror != null && mirrorSet.Contains(mirror.id_baiviet))
                map[rawId] = mirror.id_baiviet;
        }

        return map;
    }

    private List<BookingRow> ApplyFilters(List<BookingRow> rows)
    {
        if (rows == null) return new List<BookingRow>();

        string keyword = (FilterQuery ?? "").Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            rows = rows.Where(p =>
                ((p.ServiceName ?? "").ToLowerInvariant().Contains(keyword)) ||
                ((p.ShopName ?? "").ToLowerInvariant().Contains(keyword)) ||
                ((p.BranchName ?? "").ToLowerInvariant().Contains(keyword))
            ).ToList();
        }

        if (!string.IsNullOrWhiteSpace(FilterStatus) && FilterStatus != "all")
        {
            rows = rows.Where(p => MapStatusFilter(p.RawStatus) == FilterStatus).ToList();
        }

        if (!string.IsNullOrWhiteSpace(FilterTime) && FilterTime != "all")
        {
            DateTime now = DateTime.Now;
            if (FilterTime == "upcoming")
                rows = rows.Where(p => p.RawDate.HasValue && p.RawDate.Value >= now).ToList();
            else if (FilterTime == "past")
                rows = rows.Where(p => !p.RawDate.HasValue || p.RawDate.Value < now).ToList();
        }

        return rows;
    }

    private string NormalizeStatusFilter(string raw)
    {
        switch ((raw ?? "").Trim().ToLowerInvariant())
        {
            case "pending":
            case "confirmed":
            case "done":
            case "cancelled":
            case "missed":
                return raw.Trim().ToLowerInvariant();
            default:
                return "all";
        }
    }

    private string NormalizeTimeFilter(string raw)
    {
        switch ((raw ?? "").Trim().ToLowerInvariant())
        {
            case "upcoming":
            case "past":
                return raw.Trim().ToLowerInvariant();
            default:
                return "all";
        }
    }

    private string MapStatusFilter(string status)
    {
        switch ((status ?? "").Trim())
        {
            case datlich_class.trangthai_da_xacnhan:
                return "confirmed";
            case datlich_class.trangthai_da_den:
                return "done";
            case datlich_class.trangthai_da_huy:
                return "cancelled";
            case datlich_class.trangthai_khong_den:
                return "missed";
            default:
                return "pending";
        }
    }

    protected string SelectedStatus(string value)
    {
        return string.Equals(FilterStatus, value, StringComparison.OrdinalIgnoreCase) ? "selected" : "";
    }

    protected string SelectedTime(string value)
    {
        return string.Equals(FilterTime, value, StringComparison.OrdinalIgnoreCase) ? "selected" : "";
    }

    public class BookingRow
    {
        public long Id { get; set; }
        public string ServiceName { get; set; }
        public string DateText { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public string ShopName { get; set; }
        public string BranchName { get; set; }
        public string ShopPhoneLabel { get; set; }
        public string BranchAddressLabel { get; set; }
        public string NoteLabel { get; set; }
        public string ShopUrl { get; set; }
        public string RebookUrl { get; set; }
        public string CancelUrl { get; set; }
        public string EditUrl { get; set; }
        public string RawStatus { get; set; }
        public DateTime? RawDate { get; set; }
    }
}
