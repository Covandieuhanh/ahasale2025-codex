using System;
using System.Linq;
using System.Web;

public static class HoaDonThuChiSync_cl
{
    public const string SourceHoaDon = "hoadon";
    public const string SourceTheDichVu = "thedichvu";
    public const string SourceHocVien = "hocvien";
    public const string SourceNhapHang = "donnhaphang";
    public const string SourceNhapVatTu = "donnhapvattu";

    public const string AutoReceiptGroupName = "Thu tự động từ hóa đơn";
    public const string AutoServiceCardReceiptGroupName = "Thu tự động từ thẻ dịch vụ";
    public const string AutoStudentReceiptGroupName = "Thu tự động từ học viên";
    public const string AutoWarehouseExpenseGroupName = "Chi tự động nhập hàng";
    public const string AutoSupplyExpenseGroupName = "Chi tự động nhập vật tư";

    public static bool IsAutoInvoiceReceipt(bspa_thuchi_table item)
    {
        return item != null && item.tudong_tu_hoadon == true;
    }

    public static bool IsAutoSystemEntry(bspa_thuchi_table item)
    {
        return item != null && (item.tudong_tu_hethong == true || item.tudong_tu_hoadon == true);
    }

    public static string ResolveSourceLabel(bspa_thuchi_table item)
    {
        switch (ResolveSourceCode(item))
        {
            case SourceHoaDon:
                return "Hóa đơn";
            case SourceTheDichVu:
                return "Thẻ dịch vụ";
            case SourceHocVien:
                return "Học viên";
            case SourceNhapHang:
                return "Nhập hàng";
            case SourceNhapVatTu:
                return "Nhập vật tư";
            default:
                return "Tự động";
        }
    }

    public static string ResolveLinkedUrl(bspa_thuchi_table item)
    {
        if (item == null)
            return "";
        if (!string.IsNullOrWhiteSpace(item.url_admin_lienket))
            return item.url_admin_lienket;
        if (!string.IsNullOrWhiteSpace(item.id_hoadon_lienket) && IsAutoInvoiceReceipt(item))
            return BuildHoaDonUrl(item.id_hoadon_lienket);
        return "";
    }

    public static void UpsertFromInvoicePayment(dbDataContext db, bspa_hoadon_table invoice, bspa_lichsu_thanhtoan_table payment)
    {
        if (db == null || invoice == null || payment == null)
            return;

        UpsertAutoEntry(
            db,
            SourceHoaDon,
            AutoReceiptGroupName,
            "Thu",
            CoalesceNonEmpty(invoice.user_parent, payment.user_parent),
            CoalesceNonEmpty(invoice.id_nganh, payment.id_nganh),
            CoalesceNonEmpty(invoice.id_chinhanh, payment.id_chinhanh),
            invoice.id.ToString(),
            payment.id.ToString(),
            payment.thoigian,
            payment.sotienthanhtoan ?? 0,
            payment.nguoithanhtoan ?? "",
            BuildReceiptContent(invoice, payment),
            BuildHoaDonUrl(invoice.id.ToString()),
            true);
    }

    public static void UpsertFromTheDichVuPayment(dbDataContext db, thedichvu_table card, thedichvu_lichsu_thanhtoan_table payment, string userParent)
    {
        if (db == null || card == null || payment == null)
            return;

        UpsertAutoEntry(
            db,
            SourceTheDichVu,
            AutoServiceCardReceiptGroupName,
            "Thu",
            CoalesceNonEmpty(userParent, payment.user_parent),
            CoalesceNonEmpty(card.id_nganh, ResolveCurrentNganh()),
            CoalesceNonEmpty(card.id_chinhanh, payment.id_chinhanh, ResolveCurrentChiNhanh()),
            card.id.ToString(),
            payment.id.ToString(),
            payment.thoigian,
            payment.sotienthanhtoan ?? 0,
            payment.nguoithanhtoan ?? "",
            BuildServiceCardReceiptContent(card, payment),
            BuildTheDichVuUrl(card.id.ToString()),
            false);
    }

    public static void UpsertFromHocVienPayment(dbDataContext db, hocvien_table student, hocvien_lichsu_thanhtoan_table payment, string userParent)
    {
        if (db == null || student == null || payment == null)
            return;

        UpsertAutoEntry(
            db,
            SourceHocVien,
            AutoStudentReceiptGroupName,
            "Thu",
            CoalesceNonEmpty(userParent, AhaShineContext_cl.UserParent),
            CoalesceNonEmpty(student.nganhhoc, ResolveCurrentNganh()),
            CoalesceNonEmpty(student.id_chinhanh, ResolveCurrentChiNhanh()),
            student.id.ToString(),
            payment.id.ToString(),
            payment.thoigian,
            payment.sotienthanhtoan ?? 0,
            payment.nguoithanhtoan ?? "",
            BuildHocVienReceiptContent(student, payment),
            BuildHocVienUrl(student.id.ToString()),
            false);
    }

    public static void UpsertFromNhapHangPayment(dbDataContext db, donnhaphang_table order, donnhaphang_lichsu_thanhtoan_table payment)
    {
        if (db == null || order == null || payment == null)
            return;

        UpsertAutoEntry(
            db,
            SourceNhapHang,
            AutoWarehouseExpenseGroupName,
            "Chi",
            CoalesceNonEmpty(order.user_parent, payment.user_parent),
            ResolveCurrentNganh(),
            CoalesceNonEmpty(order.id_chinhanh, payment.id_chinhanh, ResolveCurrentChiNhanh()),
            order.id.ToString(),
            payment.id.ToString(),
            payment.thoigian,
            payment.sotienthanhtoan ?? 0,
            payment.nguoithanhtoan ?? "",
            BuildNhapHangExpenseContent(order, payment),
            BuildNhapHangUrl(order.id.ToString()),
            false);
    }

    public static void UpsertFromNhapVatTuPayment(dbDataContext db, donnhap_vattu_table order, donnhap_vattu_lichsu_thanhtoan_table payment)
    {
        if (db == null || order == null || payment == null)
            return;

        UpsertAutoEntry(
            db,
            SourceNhapVatTu,
            AutoSupplyExpenseGroupName,
            "Chi",
            CoalesceNonEmpty(order.user_parent, payment.user_parent),
            ResolveCurrentNganh(),
            CoalesceNonEmpty(order.id_chinhanh, payment.id_chinhanh, ResolveCurrentChiNhanh()),
            order.id.ToString(),
            payment.id.ToString(),
            payment.thoigian,
            payment.sotienthanhtoan ?? 0,
            payment.nguoithanhtoan ?? "",
            BuildNhapVatTuExpenseContent(order, payment),
            BuildNhapVatTuUrl(order.id.ToString()),
            false);
    }

    public static void DeleteForInvoicePayment(dbDataContext db, string paymentId, string idChiNhanh)
    {
        DeleteForSourcePayment(db, SourceHoaDon, paymentId, idChiNhanh);
    }

    public static void DeleteForInvoice(dbDataContext db, string invoiceId, string idChiNhanh)
    {
        DeleteForSourceDocument(db, SourceHoaDon, invoiceId, idChiNhanh);
    }

    public static void DeleteForTheDichVuPayment(dbDataContext db, string paymentId, string idChiNhanh)
    {
        DeleteForSourcePayment(db, SourceTheDichVu, paymentId, idChiNhanh);
    }

    public static void DeleteForTheDichVu(dbDataContext db, string cardId, string idChiNhanh)
    {
        DeleteForSourceDocument(db, SourceTheDichVu, cardId, idChiNhanh);
    }

    public static void DeleteForHocVienPayment(dbDataContext db, string paymentId, string idChiNhanh)
    {
        DeleteForSourcePayment(db, SourceHocVien, paymentId, idChiNhanh);
    }

    public static void DeleteForHocVien(dbDataContext db, string studentId, string idChiNhanh)
    {
        DeleteForSourceDocument(db, SourceHocVien, studentId, idChiNhanh);
    }

    public static void DeleteForNhapHangPayment(dbDataContext db, string paymentId, string idChiNhanh)
    {
        DeleteForSourcePayment(db, SourceNhapHang, paymentId, idChiNhanh);
    }

    public static void DeleteForNhapHang(dbDataContext db, string orderId, string idChiNhanh)
    {
        DeleteForSourceDocument(db, SourceNhapHang, orderId, idChiNhanh);
    }

    public static void DeleteForNhapVatTuPayment(dbDataContext db, string paymentId, string idChiNhanh)
    {
        DeleteForSourcePayment(db, SourceNhapVatTu, paymentId, idChiNhanh);
    }

    public static void DeleteForNhapVatTu(dbDataContext db, string orderId, string idChiNhanh)
    {
        DeleteForSourceDocument(db, SourceNhapVatTu, orderId, idChiNhanh);
    }

    private static void UpsertAutoEntry(
        dbDataContext db,
        string sourceCode,
        string groupName,
        string thuChi,
        string userParent,
        string idNganh,
        string idChiNhanh,
        string documentId,
        string paymentId,
        DateTime? ngay,
        long soTien,
        string username,
        string noidung,
        string urlLienKet,
        bool isInvoiceReceipt)
    {
        if (string.IsNullOrWhiteSpace(idChiNhanh) || string.IsNullOrWhiteSpace(paymentId))
            return;

        string groupId = EnsureAutoGroupId(db, userParent, idNganh, idChiNhanh, groupName);
        var q = db.bspa_thuchi_tables.Where(
            p => p.id_chinhanh == idChiNhanh
                && p.id_lichsu_thanhtoan_lienket == paymentId
                && (
                    p.nguon_tudong == sourceCode
                    || (sourceCode == SourceHoaDon && p.tudong_tu_hoadon == true)
                ));
        bspa_thuchi_table receipt = q.FirstOrDefault();
        if (receipt == null)
        {
            receipt = new bspa_thuchi_table();
            db.bspa_thuchi_tables.InsertOnSubmit(receipt);
        }

        receipt.user_parent = userParent;
        receipt.id_nhomthuchi = groupId;
        receipt.nguoilapphieu = ResolveDisplayName(db, username, idChiNhanh);
        receipt.thuchi = thuChi;
        receipt.ngay = ngay ?? DateTime.Now;
        receipt.noidung = noidung;
        receipt.sotien = soTien;
        receipt.nguoinhantien = username ?? "";
        receipt.duyet_phieuchi = "Đã duyệt";
        receipt.chophep_duyetvahuy_phieuchi = false;
        receipt.nguoihuy_duyet_phieuchi = "";
        receipt.thoigian_huyduyet_phieuchi = null;
        receipt.tudong_tu_hethong = true;
        receipt.nguon_tudong = sourceCode;
        receipt.url_admin_lienket = urlLienKet;
        receipt.tudong_tu_hoadon = isInvoiceReceipt;
        receipt.id_hoadon_lienket = documentId;
        receipt.id_lichsu_thanhtoan_lienket = paymentId;
        receipt.id_chinhanh = idChiNhanh;
        receipt.id_nganh = idNganh;
    }

    private static string EnsureAutoGroupId(dbDataContext db, string userParent, string idNganh, string idChiNhanh, string groupName)
    {
        var existing = db.bspa_nhomthuchi_tables.FirstOrDefault(
            p => p.user_parent == userParent
                && p.id_chinhanh == idChiNhanh
                && (p.id_nganh ?? "") == (idNganh ?? "")
                && p.tennhom == groupName);
        if (existing != null)
            return existing.id.ToString();

        bspa_nhomthuchi_table group = new bspa_nhomthuchi_table();
        group.user_parent = userParent;
        group.tennhom = groupName;
        group.id_nganh = idNganh;
        group.id_chinhanh = idChiNhanh;
        db.bspa_nhomthuchi_tables.InsertOnSubmit(group);
        db.SubmitChanges();
        return group.id.ToString();
    }

    private static void DeleteForSourcePayment(dbDataContext db, string sourceCode, string paymentId, string idChiNhanh)
    {
        if (db == null || string.IsNullOrWhiteSpace(paymentId) || string.IsNullOrWhiteSpace(idChiNhanh))
            return;

        var rows = db.bspa_thuchi_tables.Where(
            p => p.id_chinhanh == idChiNhanh
                && p.id_lichsu_thanhtoan_lienket == paymentId
                && (
                    p.nguon_tudong == sourceCode
                    || (sourceCode == SourceHoaDon && p.tudong_tu_hoadon == true)
                )).ToList();
        foreach (var row in rows)
        {
            db.bspa_thuchi_tables.DeleteOnSubmit(row);
        }
    }

    private static void DeleteForSourceDocument(dbDataContext db, string sourceCode, string documentId, string idChiNhanh)
    {
        if (db == null || string.IsNullOrWhiteSpace(documentId) || string.IsNullOrWhiteSpace(idChiNhanh))
            return;

        var rows = db.bspa_thuchi_tables.Where(
            p => p.id_chinhanh == idChiNhanh
                && p.id_hoadon_lienket == documentId
                && (
                    p.nguon_tudong == sourceCode
                    || (sourceCode == SourceHoaDon && p.tudong_tu_hoadon == true)
                )).ToList();
        foreach (var row in rows)
        {
            db.bspa_thuchi_tables.DeleteOnSubmit(row);
        }
    }

    private static string ResolveSourceCode(bspa_thuchi_table item)
    {
        if (item == null)
            return "";
        if (!string.IsNullOrWhiteSpace(item.nguon_tudong))
            return item.nguon_tudong;
        if (item.tudong_tu_hoadon == true)
            return SourceHoaDon;
        return "";
    }

    private static string BuildReceiptContent(bspa_hoadon_table invoice, bspa_lichsu_thanhtoan_table payment)
    {
        return "Thu tự động từ HĐ #" + invoice.id + BuildPartySegment("KH", invoice.tenkhachhang) + BuildAmountSegment(payment.sotienthanhtoan) + BuildMethodSegment(payment.hinhthuc_thanhtoan);
    }

    private static string BuildServiceCardReceiptContent(thedichvu_table card, thedichvu_lichsu_thanhtoan_table payment)
    {
        return "Thu tự động từ thẻ DV #" + card.id + BuildPartySegment("KH", card.tenkh) + BuildAmountSegment(payment.sotienthanhtoan) + BuildMethodSegment(payment.hinhthuc_thanhtoan);
    }

    private static string BuildHocVienReceiptContent(hocvien_table student, hocvien_lichsu_thanhtoan_table payment)
    {
        return "Thu tự động từ học viên #" + student.id + BuildPartySegment("HV", student.hoten) + BuildAmountSegment(payment.sotienthanhtoan) + BuildMethodSegment(payment.hinhthuc_thanhtoan);
    }

    private static string BuildNhapHangExpenseContent(donnhaphang_table order, donnhaphang_lichsu_thanhtoan_table payment)
    {
        string supplier = !string.IsNullOrWhiteSpace(order.tenkhachhang) ? order.tenkhachhang : order.nhacungcap;
        return "Chi tự động nhập hàng #" + order.id + BuildPartySegment("NCC", supplier) + BuildAmountSegment(payment.sotienthanhtoan) + BuildMethodSegment(payment.hinhthuc_thanhtoan);
    }

    private static string BuildNhapVatTuExpenseContent(donnhap_vattu_table order, donnhap_vattu_lichsu_thanhtoan_table payment)
    {
        string supplier = !string.IsNullOrWhiteSpace(order.tenkhachhang) ? order.tenkhachhang : order.nhacungcap;
        return "Chi tự động nhập vật tư #" + order.id + BuildPartySegment("NCC", supplier) + BuildAmountSegment(payment.sotienthanhtoan) + BuildMethodSegment(payment.hinhthuc_thanhtoan);
    }

    private static string BuildPartySegment(string label, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "";
        return " - " + label + " " + name;
    }

    private static string BuildAmountSegment(long? amount)
    {
        return " - " + (amount ?? 0).ToString("#,##0") + " đ";
    }

    private static string BuildMethodSegment(string method)
    {
        if (string.IsNullOrWhiteSpace(method))
            return "";
        return " - " + method;
    }

    private static string ResolveDisplayName(dbDataContext db, string username, string idChiNhanh)
    {
        if (string.IsNullOrWhiteSpace(username))
            return "Hệ thống";

        var user = db.taikhoan_table_2023s.FirstOrDefault(
            p => p.taikhoan == username && p.id_chinhanh == idChiNhanh);
        if (user != null && !string.IsNullOrWhiteSpace(user.hoten))
            return user.hoten;

        return username;
    }

    private static string CoalesceNonEmpty(params string[] values)
    {
        if (values == null)
            return "";
        foreach (string value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();
        }
        return "";
    }

    private static string ResolveCurrentNganh()
    {
        HttpContext context = HttpContext.Current;
        if (context == null || context.Session == null || context.Session["nganh"] == null)
            return "";
        return context.Session["nganh"].ToString();
    }

    private static string ResolveCurrentChiNhanh()
    {
        HttpContext context = HttpContext.Current;
        if (context == null || context.Session == null || context.Session["chinhanh"] == null)
            return "";
        return context.Session["chinhanh"].ToString();
    }

    private static string BuildHoaDonUrl(string id)
    {
        return "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + id;
    }

    private static string BuildTheDichVuUrl(string id)
    {
        return "/gianhang/admin/quan-ly-the-dich-vu/chi-tiet.aspx?id=" + id;
    }

    private static string BuildHocVienUrl(string id)
    {
        return "/gianhang/admin/quan-ly-hoc-vien/edit.aspx?id=" + id;
    }

    private static string BuildNhapHangUrl(string id)
    {
        return "/gianhang/admin/quan-ly-kho-hang/chi-tiet-nhap-hang.aspx?id=" + id;
    }

    private static string BuildNhapVatTuUrl(string id)
    {
        return "/gianhang/admin/quan-ly-vat-tu/chi-tiet-nhap-hang.aspx?id=" + id;
    }
}
