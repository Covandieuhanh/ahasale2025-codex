using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class GianHangInvoicePrint_cl
{
    public sealed class InvoiceLine
    {
        public string ten_dichvu_sanpham { get; set; }
        public decimal gia { get; set; }
        public int soluong { get; set; }
        public int chietkhau { get; set; }
        public decimal sauck { get; set; }
    }

    public sealed class InvoicePrintState
    {
        public string PublicId { get; set; }
        public string GuideId { get; set; }
        public string SellerName { get; set; }
        public string SellerAddress { get; set; }
        public string SellerPhone { get; set; }
        public string SellerLogo { get; set; }
        public string SellerIssuer { get; set; }
        public string CreatedAtText { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public string TotalText { get; set; }
        public string DiscountText { get; set; }
        public string AfterDiscountText { get; set; }
        public string PaidText { get; set; }
        public string RemainingText { get; set; }
        public string AmountInWords { get; set; }
        public string NoteHtml { get; set; }
        public string MetaHtml { get; set; }
        public List<InvoiceLine> Lines { get; set; }
    }

    public static InvoicePrintState BuildState(dbDataContext db, string rawId)
    {
        if (db == null)
            return null;

        string value = (rawId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(value))
            return null;

        InvoicePrintState state = BuildNativeState(db, value);
        if (state != null)
            return state;

        return BuildLegacyState(db, value);
    }

    private static InvoicePrintState BuildNativeState(dbDataContext db, string rawId)
    {
        GH_HoaDon_tb invoice = GianHangInvoice_cl.FindByPublicKeyOrId(db, rawId);
        if (invoice == null)
            return null;

        taikhoan_tb gianHang = RootAccount_cl.GetByAccountKey(db, invoice.shop_taikhoan);
        decimal total = invoice.tong_tien ?? 0m;
        string displayId = GianHangInvoice_cl.ResolveOrderPublicId(invoice);

        return new InvoicePrintState
        {
            PublicId = displayId,
            GuideId = invoice.id_guide.HasValue ? invoice.id_guide.Value.ToString() : string.Empty,
            SellerName = gianHang == null ? "Gian hàng đối tác" : GianHangStorefront_cl.ResolveStorefrontName(gianHang),
            SellerAddress = gianHang == null ? string.Empty : ((gianHang.diachi ?? string.Empty).Trim()),
            SellerPhone = ResolveStorePhone(gianHang),
            SellerLogo = gianHang == null ? "/uploads/images/macdinh.jpg" : GianHangStorefront_cl.ResolveAvatarUrl(gianHang),
            SellerIssuer = gianHang == null
                ? "Gian hàng đối tác"
                : (!string.IsNullOrWhiteSpace(gianHang.hoten) ? gianHang.hoten.Trim() : GianHangStorefront_cl.ResolveStorefrontName(gianHang)),
            CreatedAtText = invoice.ngay_tao.HasValue
                ? invoice.ngay_tao.Value.ToString("dd/MM/yyyy HH:mm")
                : AhaTime_cl.Now.ToString("dd/MM/yyyy HH:mm"),
            CustomerName = (invoice.ten_khach ?? string.Empty).Trim(),
            CustomerPhone = (invoice.sdt ?? string.Empty).Trim(),
            CustomerAddress = (invoice.dia_chi ?? string.Empty).Trim(),
            TotalText = FormatMoney(total),
            DiscountText = "0",
            AfterDiscountText = FormatMoney(total),
            PaidText = invoice.trang_thai == GianHangInvoice_cl.TrangThaiDaThu ? FormatMoney(total) : "0",
            RemainingText = invoice.trang_thai == GianHangInvoice_cl.TrangThaiDaThu ? "0" : FormatMoney(total),
            AmountInWords = total <= 0m ? "0" : number_class.number_to_text_unlimit(Math.Round(total, 0).ToString("0")),
            NoteHtml = BuildNativeInvoiceNote(invoice),
            MetaHtml = BuildMeta((invoice.ten_khach ?? string.Empty).Trim(), gianHang == null ? "Gian hàng đối tác" : GianHangStorefront_cl.ResolveStorefrontName(gianHang)),
            Lines = GianHangInvoice_cl.GetDetails(db, invoice)
                .Select(p => new InvoiceLine
                {
                    ten_dichvu_sanpham = p.ten_sanpham,
                    gia = p.gia ?? 0m,
                    soluong = p.so_luong ?? 0,
                    chietkhau = p.phan_tram_uu_dai ?? 0,
                    sauck = p.thanh_tien ?? 0m
                })
                .ToList()
        };
    }

    private static InvoicePrintState BuildLegacyState(dbDataContext db, string rawId)
    {
        Guid guide;
        if (!Guid.TryParse(rawId, out guide))
            return null;

        bspa_hoadon_table invoice = db.bspa_hoadon_tables.FirstOrDefault(p => p.id_guide.HasValue && p.id_guide.Value == guide);
        if (invoice == null)
            return null;

        config_thongtin_table config = db.config_thongtin_tables.FirstOrDefault();
        decimal total = invoice.tongtien ?? 0m;
        decimal afterDiscount = invoice.tongsauchietkhau ?? 0m;

        return new InvoicePrintState
        {
            PublicId = invoice.id.ToString(),
            GuideId = guide.ToString(),
            SellerName = config == null ? "AhaSale" : (config.tencongty ?? "AhaSale"),
            SellerAddress = config == null ? string.Empty : (config.diachi ?? string.Empty),
            SellerPhone = config == null ? string.Empty : (config.hotline ?? string.Empty),
            SellerLogo = config == null ? string.Empty : (config.logo_in_hoadon ?? string.Empty),
            SellerIssuer = config == null ? "AhaSale" : (config.tencongty ?? "AhaSale"),
            CreatedAtText = invoice.ngaytao.HasValue ? invoice.ngaytao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
            CustomerName = invoice.tenkhachhang ?? string.Empty,
            CustomerPhone = invoice.sdt ?? string.Empty,
            CustomerAddress = invoice.diachi ?? string.Empty,
            TotalText = FormatMoney(total),
            DiscountText = ((invoice.chietkhau ?? 0)).ToString(),
            AfterDiscountText = FormatMoney(afterDiscount),
            PaidText = FormatMoney(invoice.sotien_dathanhtoan ?? 0m),
            RemainingText = FormatMoney(invoice.sotien_conlai ?? 0m),
            AmountInWords = afterDiscount <= 0m ? "0" : number_class.number_to_text_unlimit(Math.Round(afterDiscount, 0).ToString("0")),
            NoteHtml = invoice.km1_ghichu ?? string.Empty,
            MetaHtml = BuildMeta(invoice.tenkhachhang ?? string.Empty, config == null ? "AhaSale" : (config.tencongty ?? "AhaSale")),
            Lines = db.bspa_hoadon_chitiet_tables
                .Where(p => p.id_hoadon == invoice.id.ToString())
                .ToList()
                .Select(p => new InvoiceLine
                {
                    ten_dichvu_sanpham = p.ten_dvsp_taithoidiemnay,
                    gia = p.gia_dvsp_taithoidiemnay ?? 0m,
                    soluong = p.soluong ?? 0,
                    chietkhau = p.chietkhau ?? 0,
                    sauck = p.tongsauchietkhau ?? 0m
                })
                .ToList()
        };
    }

    private static string ResolveStorePhone(taikhoan_tb store)
    {
        if (store == null)
            return string.Empty;

        string phone = (store.dienthoai ?? string.Empty).Trim();
        if (!string.IsNullOrWhiteSpace(phone))
            return phone;

        return (store.taikhoan ?? string.Empty).Trim();
    }

    private static string BuildNativeInvoiceNote(GH_HoaDon_tb invoice)
    {
        if (invoice == null)
            return string.Empty;

        List<string> parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(invoice.trang_thai))
            parts.Add("Trạng thái: " + invoice.trang_thai.Trim());
        string publicId = GianHangInvoice_cl.ResolveOrderPublicId(invoice);
        if (!string.IsNullOrWhiteSpace(publicId))
            parts.Add("Đơn hàng: #" + publicId);
        if (!string.IsNullOrWhiteSpace(invoice.ghi_chu))
            parts.Add(invoice.ghi_chu.Trim());
        return string.Join("<br/>", parts.ToArray());
    }

    private static string BuildMeta(string customerName, string storeName)
    {
        string icon = "<link rel=\"shortcut icon\" type=\"image/x-icon\" href=\"/uploads/images/favicon.png\" />";
        string appletouch = "<link rel=\"apple-touch-icon\" href=\"/uploads/images/apple-touch-icon.png\" />";
        string titleOp = "<meta property=\"og:title\" content=\"Hóa đơn điện tử\" />";
        string image = "<meta property=\"og:image\" content=\"/uploads/images/hoa-don-dien-tu.jpg\" />";
        string descriptionText = "Khách hàng: " + HttpUtility.HtmlEncode((customerName ?? string.Empty).Trim()) + " - " + HttpUtility.HtmlEncode((storeName ?? string.Empty).Trim());
        string description = "<meta name=\"description\" content=\"" + descriptionText + "\" />";
        string descriptionOp = "<meta property=\"og:description\" content=\"" + descriptionText + "\" />";
        return titleOp + image + description + descriptionOp + icon + appletouch;
    }

    private static string FormatMoney(decimal value)
    {
        return value.ToString("#,##0.##");
    }
}
