using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

public partial class shop_quan_ly_lich_hen : System.Web.UI.Page
{
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");
    protected void Page_Load(object sender, EventArgs e)
    {
        check_login_cl.check_login_shop("none", "none", true);

        if (!IsPostBack)
        {
            HandleAction();
            BindPage();
        }
    }

    private void HandleAction()
    {
        string action = (Request.QueryString["action"] ?? "").Trim().ToLowerInvariant();
        long bookingId;
        if (!long.TryParse((Request.QueryString["id"] ?? "").Trim(), out bookingId))
            return;

        if (action != "confirm" && action != "done" && action != "cancel")
            return;

        string shopAccount = (PortalRequest_cl.GetCurrentAccount() ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(shopAccount))
            return;

        using (dbDataContext db = new dbDataContext())
        {
            string chiNhanhId = AhaShineContext_cl.ResolveChiNhanhIdForShopAccount(db, shopAccount);
            bspa_datlich_table booking = db.bspa_datlich_tables.FirstOrDefault(p => p.id == bookingId && p.id_chinhanh == chiNhanhId);
            if (booking == null)
                return;

            string nextStatus = "";
            string notice = "";
            switch (action)
            {
                case "confirm":
                    nextStatus = datlich_class.trangthai_da_xacnhan;
                    notice = "Đã xác nhận lịch hẹn #" + bookingId.ToString() + ".";
                    break;
                case "done":
                    nextStatus = datlich_class.trangthai_da_den;
                    notice = "Đã chuyển lịch hẹn #" + bookingId.ToString() + " sang trạng thái đã đến.";
                    break;
                case "cancel":
                    nextStatus = datlich_class.trangthai_da_huy;
                    notice = "Đã hủy lịch hẹn #" + bookingId.ToString() + ".";
                    break;
                default:
                    return;
            }

            booking.trangthai = nextStatus;
            db.SubmitChanges();
            Session["shop_booking_notice"] = notice;
            Response.Redirect("/shop/quan-ly-lich-hen.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }

    private void BindPage()
    {
        string shopAccount = (PortalRequest_cl.GetCurrentAccount() ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(shopAccount))
        {
            Response.Redirect("/shop/login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb shop = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == shopAccount);
            if (shop == null)
            {
                Response.Redirect("/shop/login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            bool hasAdvanced = ShopLevel_cl.IsAdvancedEnabled(db, shopAccount);
            ph_advanced_link.Visible = hasAdvanced;
            ph_upgrade_link.Visible = !hasAdvanced;
            lb_shop_name.Text = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(shop.ten_shop) ? shop.taikhoan : shop.ten_shop.Trim());

            string chiNhanhId = AhaShineContext_cl.ResolveChiNhanhIdForShopAccount(db, shopAccount);
            var bookings = db.bspa_datlich_tables
                .Where(p => p.id_chinhanh == chiNhanhId)
                .OrderByDescending(p => p.ngaydat)
                .ThenByDescending(p => p.ngaytao)
                .ToList();

            var serviceNameMap = BuildServiceNameMap(db, chiNhanhId, bookings);

            int total = bookings.Count;
            int pending = 0;
            int confirmed = 0;
            int done = 0;
            foreach (var booking in bookings)
            {
                string status = (booking.trangthai ?? "").Trim();
                if (status == datlich_class.trangthai_chua_xacnhan)
                    pending++;
                else if (status == datlich_class.trangthai_da_xacnhan)
                    confirmed++;
                else if (status == datlich_class.trangthai_da_den)
                    done++;
            }

            lb_total.Text = total.ToString("#,##0");
            lb_pending.Text = pending.ToString("#,##0");
            lb_confirmed.Text = confirmed.ToString("#,##0");
            lb_done.Text = done.ToString("#,##0");

            ph_empty.Visible = bookings.Count == 0;
            var bookingViews = new System.Collections.Generic.List<object>(bookings.Count);
            foreach (var p in bookings)
            {
                string trangthai = string.IsNullOrWhiteSpace(p.trangthai) ? datlich_class.trangthai_chua_xacnhan : p.trangthai;
                bookingViews.Add(new
                {
                    p.id,
                    p.tenkhachhang,
                    p.sdt,
                    tendichvu_taithoidiemnay = string.IsNullOrWhiteSpace(p.tendichvu_taithoidiemnay)
                        ? ResolveServiceName(serviceNameMap, p.dichvu)
                        : p.tendichvu_taithoidiemnay,
                    ngaydat_text = FormatDate(p.ngaydat),
                    trangthai = trangthai,
                    status_css = ResolveStatusCss(trangthai),
                    nguongoc = string.IsNullOrWhiteSpace(p.nguongoc) ? "Shop" : p.nguongoc,
                    show_confirm = CanShowConfirm(trangthai),
                    show_done = CanShowDone(trangthai),
                    show_cancel = CanShowCancel(trangthai),
                    url_confirm = BuildActionUrl("confirm", p.id),
                    url_done = BuildActionUrl("done", p.id),
                    url_cancel = BuildActionUrl("cancel", p.id)
                });
            }
            rp_bookings.DataSource = bookingViews;
            rp_bookings.DataBind();
        }

        string notice = (Session["shop_booking_notice"] ?? "").ToString();
        ph_notice.Visible = !string.IsNullOrWhiteSpace(notice);
        lb_notice.Text = notice;
        Session["shop_booking_notice"] = "";
    }

    private static Dictionary<string, string> BuildServiceNameMap(dbDataContext db, string chiNhanhId, System.Collections.Generic.List<bspa_datlich_table> bookings)
    {
        var map = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (db == null || string.IsNullOrWhiteSpace(chiNhanhId) || bookings == null || bookings.Count == 0)
            return map;

        var serviceIds = bookings
            .Select(p => (p.dichvu ?? "").Trim())
            .Where(p => !string.IsNullOrEmpty(p))
            .Distinct()
            .ToList();

        if (serviceIds.Count == 0)
            return map;

        var webRows = db.web_post_tables
            .Where(p => p.id_chinhanh == chiNhanhId && p.bin == false && serviceIds.Contains(p.id.ToString()))
            .Select(p => new { key = p.id.ToString(), name = p.name })
            .ToList();

        foreach (var row in webRows)
        {
            string key = (row.key ?? "").Trim();
            if (key == "")
                continue;
            if (!map.ContainsKey(key))
                map[key] = (row.name ?? "").Trim();
        }

        var missing = serviceIds.Where(id => !map.ContainsKey(id)).ToList();
        if (missing.Count == 0)
            return map;

        var idInts = new System.Collections.Generic.List<int>(missing.Count);
        foreach (string raw in missing)
        {
            int idInt;
            if (int.TryParse(raw, out idInt))
                idInts.Add(idInt);
        }

        if (idInts.Count == 0)
            return map;

        var shopRows = db.BaiViet_tbs
            .Where(p => idInts.Contains(p.id) && (p.bin == false || p.bin == null) && (p.phanloai ?? "").Trim() == AccountVisibility_cl.PostTypeService)
            .Select(p => new { key = p.id.ToString(), name = p.name })
            .ToList();

        foreach (var row in shopRows)
        {
            string key = (row.key ?? "").Trim();
            if (key == "")
                continue;
            if (!map.ContainsKey(key))
                map[key] = (row.name ?? "").Trim();
        }

        return map;
    }

    private static string ResolveServiceName(System.Collections.Generic.Dictionary<string, string> map, string serviceId)
    {
        if (map == null)
            return "";
        string key = (serviceId ?? "").Trim();
        if (string.IsNullOrEmpty(key))
            return "";
        string name;
        return map.TryGetValue(key, out name) ? name : "";
    }

    private string BuildActionUrl(string action, long id)
    {
        return "/shop/quan-ly-lich-hen.aspx?action=" + HttpUtility.UrlEncode(action) + "&id=" + id.ToString();
    }

    private static string FormatDate(DateTime? value)
    {
        if (!value.HasValue)
            return "--";
        return value.Value.ToString("dd/MM/yyyy HH:mm", ViCulture);
    }

    private static string ResolveStatusCss(string status)
    {
        string value = (status ?? "").Trim();
        if (value == datlich_class.trangthai_da_xacnhan)
            return "booking-status booking-status-confirmed";
        if (value == datlich_class.trangthai_da_den)
            return "booking-status booking-status-done";
        if (value == datlich_class.trangthai_da_huy || value == datlich_class.trangthai_khong_den)
            return "booking-status booking-status-cancelled";
        return "booking-status booking-status-pending";
    }

    private static bool CanShowConfirm(string status)
    {
        string value = (status ?? "").Trim();
        return value == "" || value == datlich_class.trangthai_chua_xacnhan;
    }

    private static bool CanShowDone(string status)
    {
        string value = (status ?? "").Trim();
        return value != datlich_class.trangthai_da_den && value != datlich_class.trangthai_da_huy && value != datlich_class.trangthai_khong_den;
    }

    private static bool CanShowCancel(string status)
    {
        string value = (status ?? "").Trim();
        return value != datlich_class.trangthai_da_huy && value != datlich_class.trangthai_da_den;
    }
}
