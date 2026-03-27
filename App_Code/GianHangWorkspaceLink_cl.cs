using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class GianHangWorkspaceLink_cl
{
    private const string SessionThrottlePrefix = "gianhang_workspace_link_sync_";
    private const string SourceTypeProduct = "gh_product";
    private const string SourceTypeBooking = "gh_booking";
    private const string SourceTypeInvoice = "gh_invoice";
    private const string SourceTypeInvoiceDetail = "gh_invoice_detail";
    private const string LegacyTypePost = "web_post";
    private const string LegacyTypeBooking = "legacy_booking";
    private const string LegacyTypeInvoice = "legacy_invoice";
    private const string LegacyTypeInvoiceDetail = "legacy_invoice_detail";
    private const string SourceMarkerPrefix = "gianhang_workspace:";
    private static readonly object _schemaLock = new object();
    private static bool _schemaReady;

    private sealed class MirrorMapRow
    {
        public long Id { get; set; }
        public string OwnerAccountKey { get; set; }
        public string SourceType { get; set; }
        public string SourceId { get; set; }
        public string LegacyType { get; set; }
        public string LegacyId { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    private sealed class SyncContext
    {
        public dbDataContext Db { get; set; }
        public string OwnerAccountKey { get; set; }
        public string ChiNhanhId { get; set; }
        public string NganhId { get; set; }
        public taikhoan_table_2023 OwnerAdmin { get; set; }
        public string OwnerAdminDisplayName { get; set; }
        public Dictionary<string, MirrorMapRow> ProductMap { get; set; }
        public Dictionary<string, MirrorMapRow> BookingMap { get; set; }
        public Dictionary<string, MirrorMapRow> InvoiceMap { get; set; }
        public Dictionary<string, MirrorMapRow> InvoiceDetailMap { get; set; }
        public Dictionary<string, web_post_table> LegacyProductLookup { get; set; }
        public Dictionary<string, bspa_data_khachhang_table> CustomerByPhone { get; set; }
        public HashSet<string> MenuIds { get; set; }
    }

    public static void EnsureWorkspaceLinked(dbDataContext db, string ownerAccountKey, string chiNhanhIdHint, string nganhIdHint, bool force = false)
    {
        string ownerKey = Normalize(ownerAccountKey);
        if (db == null || ownerKey == "")
            return;

        AhaShineSchema_cl.EnsureSchemaSafe(db);
        GianHangSchema_cl.EnsureSchemaSafe(db);

        taikhoan_tb owner = RootAccount_cl.GetByAccountKey(db, ownerKey);
        if (owner == null || !SpaceAccess_cl.CanAccessGianHang(db, owner))
            return;

        taikhoan_table_2023 ownerAdmin = AhaShineContext_cl.EnsureAdvancedAdminBootstrapForShop(db, ownerKey);
        if (ownerAdmin == null)
            return;

        string chiNhanhId = (chiNhanhIdHint ?? "").Trim();
        if (chiNhanhId == "")
            chiNhanhId = (ownerAdmin.id_chinhanh ?? "").Trim();
        if (chiNhanhId == "")
            chiNhanhId = AhaShineContext_cl.ResolveChiNhanhIdForShopAccount(db, ownerKey);
        if (chiNhanhId == "")
            return;

        string nganhId = (nganhIdHint ?? "").Trim();
        if (nganhId == "")
            nganhId = (ownerAdmin.id_nganh ?? "").Trim();
        if (nganhId == "")
        {
            nganh_table nganh = db.nganh_tables.FirstOrDefault(p => p.id_chinhanh == chiNhanhId);
            if (nganh != null)
                nganhId = nganh.id.ToString();
        }

        if (!force && !ShouldSyncNow(ownerKey))
            return;

        EnsureSchemaSafe(db);

        SyncContext context = new SyncContext
        {
            Db = db,
            OwnerAccountKey = ownerKey,
            ChiNhanhId = chiNhanhId,
            NganhId = nganhId,
            OwnerAdmin = ownerAdmin,
            OwnerAdminDisplayName = ResolveOwnerAdminDisplay(ownerAdmin),
            ProductMap = LoadMappings(db, ownerKey, SourceTypeProduct, LegacyTypePost),
            BookingMap = LoadMappings(db, ownerKey, SourceTypeBooking, LegacyTypeBooking),
            InvoiceMap = LoadMappings(db, ownerKey, SourceTypeInvoice, LegacyTypeInvoice),
            InvoiceDetailMap = LoadMappings(db, ownerKey, SourceTypeInvoiceDetail, LegacyTypeInvoiceDetail),
            LegacyProductLookup = new Dictionary<string, web_post_table>(StringComparer.OrdinalIgnoreCase),
            CustomerByPhone = LoadCustomerMap(db, chiNhanhId),
            MenuIds = new HashSet<string>(db.web_menu_tables.Where(p => p.id_chinhanh == chiNhanhId).Select(p => p.id.ToString()).ToList(), StringComparer.OrdinalIgnoreCase)
        };

        SyncProducts(context);
        SyncBookings(context);
        SyncInvoices(context);
    }

    public static void EnsureWorkspaceLinked(dbDataContext db, string ownerAccountKey, bool force = false)
    {
        EnsureWorkspaceLinked(db, ownerAccountKey, "", "", force);
    }

    public static long ResolveLegacyProductId(dbDataContext db, string ownerAccountKey, long sourceProductId)
    {
        return ResolveLegacyId(db, ownerAccountKey, SourceTypeProduct, sourceProductId.ToString(), LegacyTypePost);
    }

    public static long ResolveLegacyBookingId(dbDataContext db, string ownerAccountKey, long sourceBookingId)
    {
        return ResolveLegacyId(db, ownerAccountKey, SourceTypeBooking, sourceBookingId.ToString(), LegacyTypeBooking);
    }

    public static long ResolveLegacyInvoiceId(dbDataContext db, string ownerAccountKey, long sourceInvoiceId)
    {
        return ResolveLegacyId(db, ownerAccountKey, SourceTypeInvoice, sourceInvoiceId.ToString(), LegacyTypeInvoice);
    }

    public static long ResolveSourceBookingId(dbDataContext db, string ownerAccountKey, long legacyBookingId)
    {
        return ResolveSourceId(db, ownerAccountKey, SourceTypeBooking, LegacyTypeBooking, legacyBookingId.ToString());
    }

    public static long ResolveSourceInvoiceId(dbDataContext db, string ownerAccountKey, long legacyInvoiceId)
    {
        return ResolveSourceId(db, ownerAccountKey, SourceTypeInvoice, LegacyTypeInvoice, legacyInvoiceId.ToString());
    }

    public static bool IsWorkspaceMirrorSource(string rawSource)
    {
        string source = (rawSource ?? "").Trim();
        return source.StartsWith(SourceMarkerPrefix, StringComparison.OrdinalIgnoreCase);
    }

    private static void SyncProducts(SyncContext context)
    {
        List<GH_SanPham_tb> products = GianHangProduct_cl.QueryByStorefront(context.Db, context.OwnerAccountKey)
            .OrderByDescending(p => p.id)
            .ToList();

        HashSet<string> currentSourceIds = new HashSet<string>(products.Select(p => p.id.ToString()), StringComparer.OrdinalIgnoreCase);
        String_cl stringHelper = new String_cl();

        for (int i = 0; i < products.Count; i++)
        {
            GH_SanPham_tb product = products[i];
            string sourceId = product.id.ToString();
            web_post_table legacy = ResolveLegacyPost(context, product);
            bool isNew = legacy == null;
            if (isNew)
            {
                legacy = new web_post_table();
                context.Db.web_post_tables.InsertOnSubmit(legacy);
            }

            ApplyProduct(context, legacy, product, stringHelper);
            context.Db.SubmitChanges();
            SaveMapping(context.Db, context.OwnerAccountKey, SourceTypeProduct, sourceId, LegacyTypePost, legacy.id.ToString());
            context.ProductMap[sourceId] = new MirrorMapRow
            {
                OwnerAccountKey = context.OwnerAccountKey,
                SourceType = SourceTypeProduct,
                SourceId = sourceId,
                LegacyType = LegacyTypePost,
                LegacyId = legacy.id.ToString(),
                UpdatedAt = AhaTime_cl.Now
            };
            context.LegacyProductLookup[sourceId] = legacy;
        }

        foreach (KeyValuePair<string, MirrorMapRow> pair in context.ProductMap.ToList())
        {
            if (currentSourceIds.Contains(pair.Key))
                continue;

            long legacyId;
            if (!long.TryParse((pair.Value == null ? "" : pair.Value.LegacyId) ?? "", out legacyId) || legacyId <= 0)
                continue;

            web_post_table legacy = context.Db.web_post_tables.FirstOrDefault(p => p.id == legacyId && p.id_chinhanh == context.ChiNhanhId);
            if (legacy == null)
                continue;

            legacy.bin = true;
            legacy.hienthi = false;
            context.Db.SubmitChanges();
        }
    }

    private static void SyncBookings(SyncContext context)
    {
        List<GH_DatLich_tb> bookings = GianHangBooking_cl.QueryByStorefront(context.Db, context.OwnerAccountKey)
            .OrderByDescending(p => p.id)
            .ToList();

        for (int i = 0; i < bookings.Count; i++)
        {
            GH_DatLich_tb booking = bookings[i];
            EnsureLegacyCustomer(context, booking.ten_khach, booking.sdt, "", "", booking.ngay_tao);

            string sourceId = booking.id.ToString();
            string sourceMarker = BuildSourceMarker(SourceTypeBooking, sourceId);
            bspa_datlich_table legacy = ResolveLegacyBooking(context, sourceId, sourceMarker);
            bool isNew = legacy == null;
            if (isNew)
            {
                legacy = new bspa_datlich_table();
                context.Db.bspa_datlich_tables.InsertOnSubmit(legacy);
            }

            ApplyBooking(context, legacy, booking, sourceMarker);
            context.Db.SubmitChanges();
            SaveMapping(context.Db, context.OwnerAccountKey, SourceTypeBooking, sourceId, LegacyTypeBooking, legacy.id.ToString());
            context.BookingMap[sourceId] = new MirrorMapRow
            {
                OwnerAccountKey = context.OwnerAccountKey,
                SourceType = SourceTypeBooking,
                SourceId = sourceId,
                LegacyType = LegacyTypeBooking,
                LegacyId = legacy.id.ToString(),
                UpdatedAt = AhaTime_cl.Now
            };
        }
    }

    private static void SyncInvoices(SyncContext context)
    {
        List<GH_HoaDon_tb> invoices = GianHangInvoice_cl.QueryByStorefront(context.Db, context.OwnerAccountKey)
            .OrderByDescending(p => p.id)
            .ToList();

        for (int i = 0; i < invoices.Count; i++)
        {
            GH_HoaDon_tb invoice = invoices[i];
            List<GH_HoaDon_ChiTiet_tb> details = GianHangInvoice_cl.GetDetails(context.Db, invoice);
            EnsureLegacyCustomer(context, invoice.ten_khach, invoice.sdt, invoice.dia_chi, "", invoice.ngay_tao);

            string sourceId = invoice.id.ToString();
            string sourceMarker = BuildSourceMarker(SourceTypeInvoice, sourceId);
            bspa_hoadon_table legacy = ResolveLegacyInvoice(context, invoice, sourceId, sourceMarker);
            bool isNew = legacy == null;
            if (isNew)
            {
                legacy = new bspa_hoadon_table();
                context.Db.bspa_hoadon_tables.InsertOnSubmit(legacy);
            }

            ApplyInvoice(context, legacy, invoice, details, sourceMarker);
            context.Db.SubmitChanges();
            SaveMapping(context.Db, context.OwnerAccountKey, SourceTypeInvoice, sourceId, LegacyTypeInvoice, legacy.id.ToString());
            context.InvoiceMap[sourceId] = new MirrorMapRow
            {
                OwnerAccountKey = context.OwnerAccountKey,
                SourceType = SourceTypeInvoice,
                SourceId = sourceId,
                LegacyType = LegacyTypeInvoice,
                LegacyId = legacy.id.ToString(),
                UpdatedAt = AhaTime_cl.Now
            };

            SyncInvoiceDetails(context, legacy, invoice, details);
        }
    }

    private static void SyncInvoiceDetails(SyncContext context, bspa_hoadon_table legacyInvoice, GH_HoaDon_tb invoice, List<GH_HoaDon_ChiTiet_tb> details)
    {
        if (legacyInvoice == null || invoice == null)
            return;

        List<GH_HoaDon_ChiTiet_tb> safeDetails = details ?? new List<GH_HoaDon_ChiTiet_tb>();
        HashSet<string> activeSourceIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < safeDetails.Count; i++)
        {
            GH_HoaDon_ChiTiet_tb detail = safeDetails[i];
            string sourceId = detail.id.ToString();
            activeSourceIds.Add(sourceId);

            bspa_hoadon_chitiet_table legacy = ResolveLegacyInvoiceDetail(context, sourceId);
            bool isNew = legacy == null;
            if (isNew)
            {
                legacy = new bspa_hoadon_chitiet_table();
                context.Db.bspa_hoadon_chitiet_tables.InsertOnSubmit(legacy);
            }

            ApplyInvoiceDetail(context, legacy, legacyInvoice, detail, i + 1);
            context.Db.SubmitChanges();
            SaveMapping(context.Db, context.OwnerAccountKey, SourceTypeInvoiceDetail, sourceId, LegacyTypeInvoiceDetail, legacy.id.ToString());
            context.InvoiceDetailMap[sourceId] = new MirrorMapRow
            {
                OwnerAccountKey = context.OwnerAccountKey,
                SourceType = SourceTypeInvoiceDetail,
                SourceId = sourceId,
                LegacyType = LegacyTypeInvoiceDetail,
                LegacyId = legacy.id.ToString(),
                UpdatedAt = AhaTime_cl.Now
            };
        }

        foreach (KeyValuePair<string, MirrorMapRow> pair in context.InvoiceDetailMap.ToList())
        {
            if (activeSourceIds.Contains(pair.Key))
                continue;

            long legacyId;
            if (!long.TryParse((pair.Value == null ? "" : pair.Value.LegacyId) ?? "", out legacyId) || legacyId <= 0)
                continue;

            bspa_hoadon_chitiet_table legacy = context.Db.bspa_hoadon_chitiet_tables.FirstOrDefault(p => p.id == legacyId);
            if (legacy == null)
                continue;

            if (!string.Equals((legacy.id_hoadon ?? "").Trim(), legacyInvoice.id.ToString(), StringComparison.OrdinalIgnoreCase))
                continue;

            context.Db.bspa_hoadon_chitiet_tables.DeleteOnSubmit(legacy);
            context.Db.SubmitChanges();
        }
    }

    private static web_post_table ResolveLegacyPost(SyncContext context, GH_SanPham_tb product)
    {
        if (product == null)
            return null;

        string sourceId = product.id.ToString();
        MirrorMapRow map;
        if (context.ProductMap.TryGetValue(sourceId, out map))
        {
            long legacyId;
            if (long.TryParse((map.LegacyId ?? "").Trim(), out legacyId) && legacyId > 0)
            {
                web_post_table legacy = context.Db.web_post_tables.FirstOrDefault(p => p.id == legacyId && p.id_chinhanh == context.ChiNhanhId);
                if (legacy != null)
                    return legacy;
            }
        }

        if (product.id_baiviet.HasValue && product.id_baiviet.Value > 0)
        {
            long legacyPostId = product.id_baiviet.Value;
            web_post_table legacyByReference = context.Db.web_post_tables.FirstOrDefault(
                p => p.id == legacyPostId && p.id_chinhanh == context.ChiNhanhId);
            if (legacyByReference != null)
                return legacyByReference;
        }

        return null;
    }

    private static bspa_datlich_table ResolveLegacyBooking(SyncContext context, string sourceId, string sourceMarker)
    {
        MirrorMapRow map;
        if (context.BookingMap.TryGetValue(sourceId, out map))
        {
            long legacyId;
            if (long.TryParse((map.LegacyId ?? "").Trim(), out legacyId) && legacyId > 0)
            {
                bspa_datlich_table legacy = context.Db.bspa_datlich_tables.FirstOrDefault(p => p.id == legacyId);
                if (legacy != null)
                    return legacy;
            }
        }

        return context.Db.bspa_datlich_tables.FirstOrDefault(p => p.id_chinhanh == context.ChiNhanhId && p.nguongoc == sourceMarker);
    }

    private static bspa_hoadon_table ResolveLegacyInvoice(SyncContext context, GH_HoaDon_tb invoice, string sourceId, string sourceMarker)
    {
        if (invoice != null && invoice.id_guide.HasValue)
        {
            Guid guide = invoice.id_guide.Value;
            bspa_hoadon_table byGuide = context.Db.bspa_hoadon_tables.FirstOrDefault(p => p.id_guide == guide);
            if (byGuide != null)
                return byGuide;
        }

        MirrorMapRow map;
        if (context.InvoiceMap.TryGetValue(sourceId, out map))
        {
            long legacyId;
            if (long.TryParse((map.LegacyId ?? "").Trim(), out legacyId) && legacyId > 0)
            {
                bspa_hoadon_table legacy = context.Db.bspa_hoadon_tables.FirstOrDefault(p => p.id == legacyId);
                if (legacy != null)
                    return legacy;
            }
        }

        return context.Db.bspa_hoadon_tables.FirstOrDefault(p => p.id_chinhanh == context.ChiNhanhId && p.nguongoc == sourceMarker);
    }

    private static bspa_hoadon_chitiet_table ResolveLegacyInvoiceDetail(SyncContext context, string sourceId)
    {
        MirrorMapRow map;
        if (!context.InvoiceDetailMap.TryGetValue(sourceId, out map))
            return null;

        long legacyId;
        if (!long.TryParse((map.LegacyId ?? "").Trim(), out legacyId) || legacyId <= 0)
            return null;

        return context.Db.bspa_hoadon_chitiet_tables.FirstOrDefault(p => p.id == legacyId);
    }

    private static void ApplyProduct(SyncContext context, web_post_table legacy, GH_SanPham_tb product, String_cl stringHelper)
    {
        string loai = GianHangProduct_cl.NormalizeLoai(product.loai);
        bool isService = string.Equals(loai, GianHangProduct_cl.LoaiDichVu, StringComparison.OrdinalIgnoreCase);
        string categoryId = ResolveLegacyCategoryId(context, (product.id_danhmuc ?? "").Trim());
        long price = DecimalToLong(product.gia_ban);
        long cost = LongToLong(product.gia_von);

        legacy.id_category = categoryId;
        legacy.name = (product.ten ?? "").Trim();
        legacy.name_en = string.IsNullOrWhiteSpace(legacy.name) ? "" : stringHelper.replace_name_to_url(legacy.name);
        legacy.content_post = product.noi_dung ?? "";
        legacy.description = product.mo_ta ?? "";
        legacy.image = product.hinh_anh ?? "/uploads/images/icon-img.png";
        legacy.ngaytao = product.ngay_cap_nhat ?? product.ngay_tao ?? AhaTime_cl.Now;
        legacy.nguoitao = context.OwnerAccountKey;
        legacy.bin = product.bin == true;
        legacy.noibat = legacy.noibat ?? false;
        legacy.hienthi = product.bin != true;
        legacy.phanloai = isService ? "ctdv" : "ctsp";
        legacy.id_nganh = context.NganhId ?? "";
        legacy.id_chinhanh = context.ChiNhanhId;
        legacy.id_baiviet = product.id_baiviet;

        if (isService)
        {
            legacy.giaban_dichvu = price;
            legacy.giaban_sanpham = 0;
            legacy.giavon_sanpham = 0;
            legacy.soluong_ton_sanpham = 0;
            legacy.phantram_lamdichvu = 0;
            legacy.phantram_chotsale_dichvu = 0;
            legacy.thoiluong_dichvu_phut = datlich_class.thoiluong_macdinh_dichvu_phut;
        }
        else
        {
            legacy.giaban_sanpham = price;
            legacy.giavon_sanpham = (int)Math.Min(int.MaxValue, Math.Max(0, cost));
            legacy.soluong_ton_sanpham = product.so_luong_ton ?? 0;
            legacy.phantram_chotsale_sanpham = 0;
            legacy.donvitinh_sp = string.IsNullOrWhiteSpace(legacy.donvitinh_sp) ? "" : legacy.donvitinh_sp;
            legacy.giaban_dichvu = 0;
            legacy.phantram_lamdichvu = 0;
            legacy.phantram_chotsale_dichvu = 0;
            legacy.thoiluong_dichvu_phut = null;
        }
    }

    private static void ApplyBooking(SyncContext context, bspa_datlich_table legacy, GH_DatLich_tb booking, string sourceMarker)
    {
        web_post_table legacyService = ResolveLegacyProductByGhId(context, booking.id_dichvu);
        DateTime scheduleAt = booking.thoi_gian_hen ?? booking.ngay_tao ?? AhaTime_cl.Now;
        int duration = datlich_class.thoiluong_macdinh_dichvu_phut;

        legacy.tenkhachhang = (booking.ten_khach ?? "").Trim();
        legacy.sdt = NormalizePhone(booking.sdt);
        legacy.ngaydat = scheduleAt;
        legacy.nguoitao = context.OwnerAccountKey;
        legacy.dichvu = legacyService == null ? ((booking.id_dichvu ?? 0) > 0 ? booking.id_dichvu.Value.ToString() : "") : legacyService.id.ToString();
        legacy.tendichvu_taithoidiemnay = ResolveServiceDisplayName(legacyService, booking);
        legacy.nhanvien_thuchien = legacy.nhanvien_thuchien ?? "";
        legacy.ghichu = booking.ghi_chu ?? "";
        legacy.trangthai = MapBookingStatus(booking.trang_thai);
        legacy.ngaytao = booking.ngay_tao ?? scheduleAt;
        legacy.nguongoc = sourceMarker;
        legacy.id_chinhanh = context.ChiNhanhId;
        legacy.thoiluong_dichvu_phut = duration;
        legacy.ngayketthucdukien = scheduleAt.AddMinutes(duration);
    }

    private static void ApplyInvoice(SyncContext context, bspa_hoadon_table legacy, GH_HoaDon_tb invoice, List<GH_HoaDon_ChiTiet_tb> details, string sourceMarker)
    {
        List<GH_HoaDon_ChiTiet_tb> safeDetails = details ?? new List<GH_HoaDon_ChiTiet_tb>();
        long tongTien = 0;
        long tongSauCk = 0;
        long dsDv = 0;
        long dsSp = 0;
        long sauCkDv = 0;
        long sauCkSp = 0;
        int slDv = 0;
        int slSp = 0;

        for (int i = 0; i < safeDetails.Count; i++)
        {
            GH_HoaDon_ChiTiet_tb detail = safeDetails[i];
            long lineTotal = DecimalToLong(detail.thanh_tien);
            long lineAfter = ResolveAfterDiscount(lineTotal, detail.phan_tram_uu_dai);
            bool isService = string.Equals((detail.loai ?? "").Trim(), GianHangProduct_cl.LoaiDichVu, StringComparison.OrdinalIgnoreCase);

            tongTien += lineTotal;
            tongSauCk += lineAfter;
            if (isService)
            {
                dsDv += lineTotal;
                sauCkDv += lineAfter;
                slDv += Math.Max(1, detail.so_luong ?? 0);
            }
            else
            {
                dsSp += lineTotal;
                sauCkSp += lineAfter;
                slSp += Math.Max(1, detail.so_luong ?? 0);
            }
        }

        if (tongTien <= 0)
            tongTien = DecimalToLong(invoice.tong_tien);
        if (tongSauCk <= 0)
            tongSauCk = tongTien;

        long daThanhToan = invoice.is_offline == true ? tongSauCk : 0;
        long conLai = Math.Max(0, tongSauCk - daThanhToan);

        legacy.user_parent = context.OwnerAccountKey;
        legacy.ngaytao = invoice.ngay_tao ?? AhaTime_cl.Now;
        legacy.nguoitao = context.OwnerAccountKey;
        legacy.tongtien = tongTien;
        legacy.chietkhau = 0;
        legacy.tongtien_ck_hoadon = Math.Max(0, tongTien - tongSauCk);
        legacy.tongsauchietkhau = tongSauCk;
        legacy.tenkhachhang = (invoice.ten_khach ?? "").Trim();
        legacy.sdt = NormalizePhone(invoice.sdt);
        legacy.diachi = invoice.dia_chi ?? "";
        legacy.ghichu = invoice.ghi_chu ?? "";
        legacy.sotien_dathanhtoan = daThanhToan;
        legacy.thanhtoan_tienmat = invoice.is_offline == true ? daThanhToan : 0;
        legacy.thanhtoan_chuyenkhoan = 0;
        legacy.thanhtoan_quetthe = 0;
        legacy.sotien_conlai = conLai;
        legacy.album = legacy.album ?? "";
        legacy.dichvu_hay_sanpham = ResolveInvoiceType(slDv, slSp);
        legacy.sl_dichvu = slDv;
        legacy.sl_sanpham = slSp;
        legacy.ds_dichvu = dsDv;
        legacy.ds_sanpham = dsSp;
        legacy.sauck_dichvu = sauCkDv;
        legacy.sauck_sanpham = sauCkSp;
        legacy.km1_ghichu = BuildSourceLabel(invoice, sourceMarker);
        legacy.id_guide = invoice.id_guide;
        legacy.nguongoc = sourceMarker;
        legacy.id_chinhanh = context.ChiNhanhId;
        legacy.id_nganh = context.NganhId ?? "";
    }

    private static void ApplyInvoiceDetail(SyncContext context, bspa_hoadon_chitiet_table legacy, bspa_hoadon_table legacyInvoice, GH_HoaDon_ChiTiet_tb detail, int lineNumber)
    {
        web_post_table legacyProduct = ResolveLegacyProductByGhId(context, detail.id_sanpham);
        long lineTotal = DecimalToLong(detail.thanh_tien);
        int discountPercent = ClampDiscount(detail.phan_tram_uu_dai);
        long discountAmount = ResolveDiscountAmount(lineTotal, discountPercent);
        long afterDiscount = lineTotal - discountAmount;

        legacy.user_parent = context.OwnerAccountKey;
        legacy.id_hoadon = legacyInvoice.id.ToString();
        legacy.id_dvsp = legacyProduct == null
            ? ((detail.id_sanpham ?? 0) > 0 ? detail.id_sanpham.Value.ToString() : "")
            : legacyProduct.id.ToString();
        legacy.ten_dvsp_taithoidiemnay = (detail.ten_sanpham ?? "").Trim();
        legacy.gia_dvsp_taithoidiemnay = DecimalToLong(detail.gia);
        legacy.soluong = detail.so_luong ?? 1;
        legacy.thanhtien = lineTotal;
        legacy.chietkhau = discountPercent;
        legacy.tongtien_ck_dvsp = discountAmount;
        legacy.tongsauchietkhau = afterDiscount;
        legacy.nguoichot_dvsp = context.OwnerAdmin == null ? "" : (context.OwnerAdmin.taikhoan ?? "");
        legacy.phantram_chotsale_dvsp = 0;
        legacy.tongtien_chotsale_dvsp = 0;
        legacy.nguoilam_dichvu = "";
        legacy.phantram_lamdichvu = 0;
        legacy.tongtien_lamdichvu = 0;
        legacy.kyhieu = lineNumber.ToString();
        legacy.tennguoichot_hientai = context.OwnerAdminDisplayName;
        legacy.tennguoilam_hientai = "";
        legacy.hinhanh_hientai = detail.hinh_anh ?? "";
        legacy.danhgia_nhanvien_lamdichvu = legacy.danhgia_nhanvien_lamdichvu ?? "";
        legacy.danhgia_5sao_dv = legacy.danhgia_5sao_dv ?? "";
        legacy.ngaytao = legacyInvoice.ngaytao;
        legacy.nguoitao = context.OwnerAccountKey;
        legacy.solo = BuildSourceLabel(null, BuildSourceMarker(SourceTypeInvoiceDetail, detail.id.ToString()));
        legacy.id_thedichvu = "";
        legacy.gia_hienthi_khidung_thedv = null;
        legacy.id_chinhanh = context.ChiNhanhId;
        legacy.id_nganh = context.NganhId ?? "";
    }

    private static bspa_data_khachhang_table EnsureLegacyCustomer(SyncContext context, string name, string phone, string address, string email, DateTime? createdAt)
    {
        string normalizedPhone = NormalizePhone(phone);
        if (normalizedPhone == "")
            return null;

        bspa_data_khachhang_table customer;
        if (!context.CustomerByPhone.TryGetValue(normalizedPhone, out customer) || customer == null)
        {
            customer = new bspa_data_khachhang_table
            {
                sdt = normalizedPhone,
                matkhau = "",
                tenkhachhang = (name ?? "").Trim(),
                ngaytao = createdAt ?? AhaTime_cl.Now,
                nguoitao = context.OwnerAccountKey,
                user_parent = context.OwnerAccountKey,
                anhdaidien = "",
                diachi = address ?? "",
                magioithieu = "",
                nguoichamsoc = "",
                nhomkhachhang = "",
                id_chinhanh = context.ChiNhanhId,
                sodiem_e_aha = 0,
                vnd_tu_e_aha = 0,
                capbac = "",
                solan_lencap = 0,
                email = email ?? ""
            };
            context.Db.bspa_data_khachhang_tables.InsertOnSubmit(customer);
            context.Db.SubmitChanges();
            context.CustomerByPhone[normalizedPhone] = customer;
            return customer;
        }

        bool changed = false;
        if (string.IsNullOrWhiteSpace(customer.tenkhachhang) && !string.IsNullOrWhiteSpace(name))
        {
            customer.tenkhachhang = name.Trim();
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(customer.diachi) && !string.IsNullOrWhiteSpace(address))
        {
            customer.diachi = address.Trim();
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(customer.email) && !string.IsNullOrWhiteSpace(email))
        {
            customer.email = email.Trim();
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(customer.user_parent))
        {
            customer.user_parent = context.OwnerAccountKey;
            changed = true;
        }
        if (string.IsNullOrWhiteSpace(customer.id_chinhanh))
        {
            customer.id_chinhanh = context.ChiNhanhId;
            changed = true;
        }
        if (changed)
            context.Db.SubmitChanges();

        return customer;
    }

    private static Dictionary<string, bspa_data_khachhang_table> LoadCustomerMap(dbDataContext db, string chiNhanhId)
    {
        Dictionary<string, bspa_data_khachhang_table> map = new Dictionary<string, bspa_data_khachhang_table>(StringComparer.OrdinalIgnoreCase);
        List<bspa_data_khachhang_table> customers = db.bspa_data_khachhang_tables.Where(p => p.id_chinhanh == chiNhanhId).ToList();
        for (int i = 0; i < customers.Count; i++)
        {
            bspa_data_khachhang_table customer = customers[i];
            string phone = NormalizePhone(customer.sdt);
            if (phone == "" || map.ContainsKey(phone))
                continue;
            map[phone] = customer;
        }
        return map;
    }

    private static web_post_table ResolveLegacyProductByGhId(SyncContext context, int? sourceProductId)
    {
        if (!sourceProductId.HasValue || sourceProductId.Value <= 0)
            return null;

        string sourceId = sourceProductId.Value.ToString();
        web_post_table cached;
        if (context.LegacyProductLookup.TryGetValue(sourceId, out cached) && cached != null)
            return cached;

        GH_SanPham_tb sourceProduct = context.Db.GetTable<GH_SanPham_tb>().FirstOrDefault(
            p => p.id == sourceProductId.Value && p.shop_taikhoan == context.OwnerAccountKey);
        web_post_table legacy = ResolveLegacyPost(context, sourceProduct);
        if (legacy != null)
            context.LegacyProductLookup[sourceId] = legacy;
        return legacy;
    }

    private static string ResolveLegacyCategoryId(SyncContext context, string rawCategoryId)
    {
        string categoryId = (rawCategoryId ?? "").Trim();
        if (categoryId != "" && context.MenuIds.Contains(categoryId))
            return categoryId;
        return "0";
    }

    private static Dictionary<string, MirrorMapRow> LoadMappings(dbDataContext db, string ownerAccountKey, string sourceType, string legacyType)
    {
        List<MirrorMapRow> rows = db.ExecuteQuery<MirrorMapRow>(
            "SELECT Id, OwnerAccountKey, SourceType, SourceId, LegacyType, LegacyId, UpdatedAt FROM dbo.CoreGianHangWorkspaceMirror_tb WHERE OwnerAccountKey = {0} AND SourceType = {1} AND LegacyType = {2}",
            ownerAccountKey,
            sourceType,
            legacyType).ToList();

        Dictionary<string, MirrorMapRow> map = new Dictionary<string, MirrorMapRow>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < rows.Count; i++)
        {
            MirrorMapRow row = rows[i];
            string key = (row.SourceId ?? "").Trim();
            if (key == "" || map.ContainsKey(key))
                continue;
            map[key] = row;
        }
        return map;
    }

    private static long ResolveLegacyId(dbDataContext db, string ownerAccountKey, string sourceType, string sourceId, string legacyType)
    {
        string ownerKey = Normalize(ownerAccountKey);
        string sourceKey = (sourceId ?? "").Trim();
        if (db == null || ownerKey == "" || sourceKey == "" || sourceType == "" || legacyType == "")
            return 0L;

        EnsureSchemaSafe(db);

        MirrorMapRow row = db.ExecuteQuery<MirrorMapRow>(
            "SELECT TOP 1 Id, OwnerAccountKey, SourceType, SourceId, LegacyType, LegacyId, UpdatedAt " +
            "FROM dbo.CoreGianHangWorkspaceMirror_tb " +
            "WHERE OwnerAccountKey = {0} AND SourceType = {1} AND SourceId = {2} AND LegacyType = {3}",
            ownerKey,
            sourceType,
            sourceKey,
            legacyType).FirstOrDefault();

        long legacyId;
        return row != null && long.TryParse((row.LegacyId ?? "").Trim(), out legacyId) ? legacyId : 0L;
    }

    private static long ResolveSourceId(dbDataContext db, string ownerAccountKey, string sourceType, string legacyType, string legacyId)
    {
        string ownerKey = Normalize(ownerAccountKey);
        string legacyKey = (legacyId ?? "").Trim();
        if (db == null || ownerKey == "" || legacyKey == "" || sourceType == "" || legacyType == "")
            return 0L;

        EnsureSchemaSafe(db);

        MirrorMapRow row = db.ExecuteQuery<MirrorMapRow>(
            "SELECT TOP 1 Id, OwnerAccountKey, SourceType, SourceId, LegacyType, LegacyId, UpdatedAt " +
            "FROM dbo.CoreGianHangWorkspaceMirror_tb " +
            "WHERE OwnerAccountKey = {0} AND SourceType = {1} AND LegacyType = {2} AND LegacyId = {3}",
            ownerKey,
            sourceType,
            legacyType,
            legacyKey).FirstOrDefault();

        long sourceValue;
        return row != null && long.TryParse((row.SourceId ?? "").Trim(), out sourceValue) ? sourceValue : 0L;
    }

    private static void SaveMapping(dbDataContext db, string ownerAccountKey, string sourceType, string sourceId, string legacyType, string legacyId)
    {
        DateTime now = AhaTime_cl.Now;
        int affected = db.ExecuteCommand(
            "UPDATE dbo.CoreGianHangWorkspaceMirror_tb SET LegacyId = {4}, UpdatedAt = {5} WHERE OwnerAccountKey = {0} AND SourceType = {1} AND SourceId = {2} AND LegacyType = {3}",
            ownerAccountKey,
            sourceType,
            sourceId,
            legacyType,
            legacyId,
            now);

        if (affected != 0)
            return;

        db.ExecuteCommand(
            "INSERT INTO dbo.CoreGianHangWorkspaceMirror_tb (OwnerAccountKey, SourceType, SourceId, LegacyType, LegacyId, CreatedAt, UpdatedAt) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {5})",
            ownerAccountKey,
            sourceType,
            sourceId,
            legacyType,
            legacyId,
            now);
    }

    private static bool ShouldSyncNow(string ownerAccountKey)
    {
        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null)
            return true;

        string key = SessionThrottlePrefix + Normalize(ownerAccountKey);
        object raw = ctx.Session[key];
        DateTime last;
        if (raw is DateTime)
            last = (DateTime)raw;
        else if (!DateTime.TryParse((raw ?? "").ToString(), out last))
        {
            ctx.Session[key] = AhaTime_cl.Now;
            return true;
        }

        if ((AhaTime_cl.Now - last).TotalSeconds < 15)
            return false;

        ctx.Session[key] = AhaTime_cl.Now;
        return true;
    }

    private static string ResolveOwnerAdminDisplay(taikhoan_table_2023 ownerAdmin)
    {
        if (ownerAdmin == null)
            return "";

        string display = (ownerAdmin.hoten ?? "").Trim();
        return display == "" ? (ownerAdmin.taikhoan ?? "") : display;
    }

    private static string ResolveServiceDisplayName(web_post_table legacyService, GH_DatLich_tb booking)
    {
        if (legacyService != null && !string.IsNullOrWhiteSpace(legacyService.name))
            return legacyService.name.Trim();

        string serviceName = (booking == null ? "" : (booking.dich_vu ?? "")).Trim();
        if (serviceName != "")
            return serviceName;

        if (booking != null && booking.id_dichvu.HasValue)
            return "Dịch vụ #" + booking.id_dichvu.Value.ToString();

        return "";
    }

    private static string MapBookingStatus(string raw)
    {
        string status = (raw ?? "").Trim();
        if (string.Equals(status, GianHangBooking_cl.TrangThaiDaXacNhan, StringComparison.OrdinalIgnoreCase))
            return datlich_class.trangthai_da_xacnhan;
        if (string.Equals(status, GianHangBooking_cl.TrangThaiHoanThanh, StringComparison.OrdinalIgnoreCase))
            return datlich_class.trangthai_da_den;
        if (string.Equals(status, GianHangBooking_cl.TrangThaiHuy, StringComparison.OrdinalIgnoreCase))
            return datlich_class.trangthai_da_huy;
        return datlich_class.trangthai_chua_xacnhan;
    }

    private static string ResolveInvoiceType(int serviceCount, int productCount)
    {
        if (serviceCount > 0 && productCount > 0)
            return "Dịch vụ + Sản phẩm";
        if (serviceCount > 0)
            return "Dịch vụ";
        if (productCount > 0)
            return "Sản phẩm";
        return "Đơn hàng";
    }

    private static string BuildSourceMarker(string sourceType, string sourceId)
    {
        return SourceMarkerPrefix + (sourceType ?? "").Trim() + ":" + (sourceId ?? "").Trim();
    }

    private static string BuildSourceLabel(GH_HoaDon_tb invoice, string marker)
    {
        string status = invoice == null ? "" : GianHangInvoice_cl.ResolveOrderStatusText(invoice);
        if (string.IsNullOrWhiteSpace(status))
            status = "Đồng bộ từ /gianhang";
        return status + " | " + marker;
    }

    private static int ClampDiscount(int? percent)
    {
        int value = percent ?? 0;
        if (value < 0) value = 0;
        if (value > 50) value = 50;
        return value;
    }

    private static long ResolveDiscountAmount(long lineTotal, int? percent)
    {
        int safePercent = ClampDiscount(percent);
        if (safePercent <= 0 || lineTotal <= 0)
            return 0;
        return (long)Math.Round((decimal)lineTotal * safePercent / 100m, MidpointRounding.AwayFromZero);
    }

    private static long ResolveAfterDiscount(long lineTotal, int? percent)
    {
        long after = lineTotal - ResolveDiscountAmount(lineTotal, percent);
        return after < 0 ? 0 : after;
    }

    private static long DecimalToLong(decimal? value)
    {
        return value.HasValue ? (long)Math.Round(value.Value, MidpointRounding.AwayFromZero) : 0;
    }

    private static long LongToLong(long? value)
    {
        return value ?? 0;
    }

    private static string Normalize(string raw)
    {
        return (raw ?? "").Trim().ToLowerInvariant();
    }

    private static string NormalizePhone(string raw)
    {
        return AccountAuth_cl.NormalizePhone(raw) ?? "";
    }

    private static void EnsureSchemaSafe(dbDataContext db)
    {
        if (db == null || _schemaReady)
            return;

        lock (_schemaLock)
        {
            if (_schemaReady)
                return;

            try
            {
                db.ExecuteCommand(@"
IF OBJECT_ID('dbo.CoreGianHangWorkspaceMirror_tb', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoreGianHangWorkspaceMirror_tb
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        OwnerAccountKey NVARCHAR(128) NOT NULL,
        SourceType NVARCHAR(64) NOT NULL,
        SourceId NVARCHAR(128) NOT NULL,
        LegacyType NVARCHAR(64) NOT NULL,
        LegacyId NVARCHAR(128) NOT NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangWorkspaceMirror_CreatedAt DEFAULT(GETDATE()),
        UpdatedAt DATETIME NOT NULL CONSTRAINT DF_CoreGianHangWorkspaceMirror_UpdatedAt DEFAULT(GETDATE())
    );
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'UX_CoreGianHangWorkspaceMirror_Source'
      AND object_id = OBJECT_ID('dbo.CoreGianHangWorkspaceMirror_tb')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_CoreGianHangWorkspaceMirror_Source
        ON dbo.CoreGianHangWorkspaceMirror_tb(OwnerAccountKey, SourceType, SourceId, LegacyType);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_CoreGianHangWorkspaceMirror_Legacy'
      AND object_id = OBJECT_ID('dbo.CoreGianHangWorkspaceMirror_tb')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_CoreGianHangWorkspaceMirror_Legacy
        ON dbo.CoreGianHangWorkspaceMirror_tb(OwnerAccountKey, LegacyType, LegacyId);
END
");
                _schemaReady = true;
            }
            catch
            {
            }
        }
    }
}
