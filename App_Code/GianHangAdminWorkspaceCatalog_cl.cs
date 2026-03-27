using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public static class GianHangAdminWorkspaceCatalog_cl
{
    public sealed class CatalogRow
    {
        public int NativeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string PriceText { get; set; }
        public string VisibilityText { get; set; }
        public string VisibilityCss { get; set; }
        public string UpdatedAtText { get; set; }
        public string NativeEditUrl { get; set; }
        public string PublicUrl { get; set; }
        public string AdminEditUrl { get; set; }
        public string WorkspaceDetailUrl { get; set; }
        public bool HasLegacyMirror { get; set; }
        public string BridgeText { get; set; }
        public string CategoryText { get; set; }
        public string StockText { get; set; }
        public string DiscountText { get; set; }
    }

    public sealed class CatalogDetail
    {
        public int NativeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContentHtml { get; set; }
        public string ImageUrl { get; set; }
        public string PriceText { get; set; }
        public string VisibilityText { get; set; }
        public string VisibilityCss { get; set; }
        public string UpdatedAtText { get; set; }
        public string NativeEditUrl { get; set; }
        public string PublicUrl { get; set; }
        public string AdminEditUrl { get; set; }
        public bool HasLegacyMirror { get; set; }
        public long LegacyMirrorId { get; set; }
        public string BridgeText { get; set; }
        public string CategoryText { get; set; }
        public string StockText { get; set; }
        public string DiscountText { get; set; }
        public int DiscountPercent { get; set; }
        public bool IsService { get; set; }
        public string OwnerAccountKey { get; set; }
    }

    public sealed class CatalogPayload
    {
        public int TotalCount { get; set; }
        public int VisibleCount { get; set; }
        public int HiddenCount { get; set; }
        public int MirroredCount { get; set; }
        public int DiscountedCount { get; set; }
        public List<CatalogRow> Rows { get; set; }
    }

    private sealed class MirrorLookupRow
    {
        public string SourceId { get; set; }
        public string LegacyId { get; set; }
    }

    public static CatalogPayload LoadByType(dbDataContext db, string ownerAccountKey, string loai, string keyword, string visibility, int take)
    {
        string ownerKey = (ownerAccountKey ?? string.Empty).Trim().ToLowerInvariant();
        string normalizedType = GianHangProduct_cl.NormalizeLoai(loai);
        string normalizedVisibility = (visibility ?? string.Empty).Trim().ToLowerInvariant();
        string search = (keyword ?? string.Empty).Trim();

        CatalogPayload payload = new CatalogPayload
        {
            Rows = new List<CatalogRow>()
        };

        if (db == null || ownerKey == string.Empty)
            return payload;

        GianHangWorkspaceLink_cl.EnsureWorkspaceLinked(db, ownerKey, false);

        IQueryable<GH_SanPham_tb> baseQuery = GianHangProduct_cl.QueryByStorefront(db, ownerKey)
            .Where(p => (p.loai ?? string.Empty).Trim().ToLower() == normalizedType);

        List<GH_SanPham_tb> summaryItems = baseQuery.ToList();
        payload.TotalCount = summaryItems.Count;
        payload.VisibleCount = summaryItems.Count(p => p.bin == null || p.bin == false);
        payload.HiddenCount = payload.TotalCount - payload.VisibleCount;
        payload.DiscountedCount = summaryItems.Count(p => GianHangProduct_cl.ResolveDiscountPercent(p) > 0);

        Dictionary<string, long> mirrorMap = LoadMirrorMap(db, ownerKey);
        payload.MirroredCount = summaryItems.Count(p => mirrorMap.ContainsKey(p.id.ToString()));

        IQueryable<GH_SanPham_tb> query = baseQuery;
        if (normalizedVisibility == "visible")
            query = query.Where(p => p.bin == null || p.bin == false);
        else if (normalizedVisibility == "hidden")
            query = query.Where(p => p.bin == true);

        int numericKeyword;
        bool hasNumericKeyword = int.TryParse(search, out numericKeyword);
        if (search != string.Empty)
        {
            query = query.Where(p =>
                (p.ten ?? string.Empty).Contains(search)
                || (p.mo_ta ?? string.Empty).Contains(search)
                || (hasNumericKeyword && (p.id == numericKeyword || p.id_baiviet == numericKeyword)));
        }

        List<GH_SanPham_tb> items = query
            .OrderByDescending(p => p.ngay_cap_nhat ?? p.ngay_tao)
            .ThenByDescending(p => p.id)
            .Take(take <= 0 ? 160 : take)
            .ToList();

        Dictionary<string, string> categoryMap = db.DanhMuc_tbs.ToList()
            .GroupBy(p => p.id.ToString())
            .ToDictionary(g => g.Key, g => ((g.First().name ?? string.Empty).Trim()), StringComparer.OrdinalIgnoreCase);

        payload.Rows = items.Select(item =>
        {
            long legacyId;
            bool hasMirror = mirrorMap.TryGetValue(item.id.ToString(), out legacyId) && legacyId > 0;
            string publicUrl = normalizedType == GianHangProduct_cl.LoaiDichVu
                ? GianHangRoutes_cl.BuildXemDichVuUrl(item.id)
                : GianHangRoutes_cl.BuildXemSanPhamUrl(item.id);
            publicUrl = GianHangRoutes_cl.AppendUserToUrl(publicUrl, ownerKey);

            string categoryName;
            if (!categoryMap.TryGetValue((item.id_danhmuc ?? string.Empty).Trim(), out categoryName) || string.IsNullOrWhiteSpace(categoryName))
                categoryName = "Chưa gắn danh mục";

            int discountPercent = GianHangProduct_cl.ResolveDiscountPercent(item);
            return new CatalogRow
            {
                NativeId = item.id,
                Name = string.IsNullOrWhiteSpace(item.ten) ? (normalizedType == GianHangProduct_cl.LoaiDichVu ? "Dịch vụ chưa đặt tên" : "Sản phẩm chưa đặt tên") : item.ten.Trim(),
                Description = string.IsNullOrWhiteSpace(item.mo_ta) ? "Chưa có mô tả ngắn." : item.mo_ta.Trim(),
                ImageUrl = GianHangStorefront_cl.ResolveImageUrl(item.hinh_anh),
                PriceText = (item.gia_ban ?? 0m) <= 0m ? "Liên hệ" : (item.gia_ban ?? 0m).ToString("#,##0", CultureInfo.GetCultureInfo("vi-VN")) + " đ",
                VisibilityText = item.bin == true ? "Đang ẩn" : "Đang hiển thị",
                VisibilityCss = item.bin == true ? "gh-catalog-badge gh-catalog-badge--hidden" : "gh-catalog-badge gh-catalog-badge--visible",
                UpdatedAtText = (item.ngay_cap_nhat ?? item.ngay_tao).HasValue ? (item.ngay_cap_nhat ?? item.ngay_tao).Value.ToString("dd/MM/yyyy HH:mm") : "--",
                NativeEditUrl = "/gianhang/quan-ly-tin/Them.aspx?id=" + item.id.ToString(),
                PublicUrl = publicUrl,
                AdminEditUrl = hasMirror ? ("/gianhang/admin/quan-ly-bai-viet/edit.aspx?id=" + legacyId.ToString()) : (normalizedType == GianHangProduct_cl.LoaiDichVu ? "/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=dv" : "/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=sp"),
                WorkspaceDetailUrl = normalizedType == GianHangProduct_cl.LoaiDichVu
                    ? GianHangRoutes_cl.BuildAdminWorkspaceServiceDetailUrl(item.id)
                    : GianHangRoutes_cl.BuildAdminWorkspaceProductDetailUrl(item.id),
                HasLegacyMirror = hasMirror,
                BridgeText = hasMirror ? ("Đã bridge #" + legacyId.ToString()) : "Chưa bridge sang admin",
                CategoryText = categoryName,
                StockText = normalizedType == GianHangProduct_cl.LoaiDichVu ? "Không theo tồn kho" : ("Tồn: " + ((item.so_luong_ton ?? 0).ToString("#,##0"))),
                DiscountText = discountPercent > 0 ? ("Ưu đãi " + discountPercent.ToString() + "%") : "Chưa đặt ưu đãi"
            };
        }).ToList();

        return payload;
    }

    public static CatalogDetail LoadDetail(dbDataContext db, string ownerAccountKey, string loai, int nativeId)
    {
        string ownerKey = (ownerAccountKey ?? string.Empty).Trim().ToLowerInvariant();
        string normalizedType = GianHangProduct_cl.NormalizeLoai(loai);
        if (db == null || ownerKey == string.Empty || nativeId <= 0)
            return null;

        GianHangWorkspaceLink_cl.EnsureWorkspaceLinked(db, ownerKey, false);

        GH_SanPham_tb item = GianHangProduct_cl.QueryByStorefront(db, ownerKey)
            .FirstOrDefault(p => p.id == nativeId && (p.loai ?? string.Empty).Trim().ToLower() == normalizedType);
        if (item == null)
            return null;

        Dictionary<string, long> mirrorMap = LoadMirrorMap(db, ownerKey);
        long legacyId;
        bool hasMirror = mirrorMap.TryGetValue(item.id.ToString(), out legacyId) && legacyId > 0;

        string categoryName = "Chưa gắn danh mục";
        string categoryId = (item.id_danhmuc ?? string.Empty).Trim();
        if (categoryId != string.Empty)
        {
            DanhMuc_tb category = db.DanhMuc_tbs.FirstOrDefault(p => p.id.ToString() == categoryId);
            if (category != null && !string.IsNullOrWhiteSpace(category.name))
                categoryName = category.name.Trim();
        }

        int discountPercent = GianHangProduct_cl.ResolveDiscountPercent(item);
        string publicUrl = normalizedType == GianHangProduct_cl.LoaiDichVu
            ? GianHangRoutes_cl.BuildXemDichVuUrl(item.id)
            : GianHangRoutes_cl.BuildXemSanPhamUrl(item.id);
        publicUrl = GianHangRoutes_cl.AppendUserToUrl(publicUrl, ownerKey);

        return new CatalogDetail
        {
            NativeId = item.id,
            Name = string.IsNullOrWhiteSpace(item.ten) ? (normalizedType == GianHangProduct_cl.LoaiDichVu ? "Dịch vụ chưa đặt tên" : "Sản phẩm chưa đặt tên") : item.ten.Trim(),
            Description = string.IsNullOrWhiteSpace(item.mo_ta) ? "Chưa có mô tả ngắn." : item.mo_ta.Trim(),
            ContentHtml = item.noi_dung ?? string.Empty,
            ImageUrl = GianHangStorefront_cl.ResolveImageUrl(item.hinh_anh),
            PriceText = (item.gia_ban ?? 0m) <= 0m ? "Liên hệ" : (item.gia_ban ?? 0m).ToString("#,##0", CultureInfo.GetCultureInfo("vi-VN")) + " đ",
            VisibilityText = item.bin == true ? "Đang ẩn" : "Đang hiển thị",
            VisibilityCss = item.bin == true ? "gh-catalog-badge gh-catalog-badge--hidden" : "gh-catalog-badge gh-catalog-badge--visible",
            UpdatedAtText = (item.ngay_cap_nhat ?? item.ngay_tao).HasValue ? (item.ngay_cap_nhat ?? item.ngay_tao).Value.ToString("dd/MM/yyyy HH:mm") : "--",
            NativeEditUrl = "/gianhang/quan-ly-tin/Them.aspx?id=" + item.id.ToString(),
            PublicUrl = publicUrl,
            AdminEditUrl = hasMirror ? ("/gianhang/admin/quan-ly-bai-viet/edit.aspx?id=" + legacyId.ToString()) : (normalizedType == GianHangProduct_cl.LoaiDichVu ? "/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=dv" : "/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=sp"),
            HasLegacyMirror = hasMirror,
            LegacyMirrorId = legacyId,
            BridgeText = hasMirror ? ("Đã bridge #" + legacyId.ToString()) : "Chưa bridge sang admin",
            CategoryText = categoryName,
            StockText = normalizedType == GianHangProduct_cl.LoaiDichVu ? "Không theo tồn kho" : ("Tồn: " + ((item.so_luong_ton ?? 0).ToString("#,##0"))),
            DiscountText = discountPercent > 0 ? ("Ưu đãi " + discountPercent.ToString() + "%") : "Chưa đặt ưu đãi",
            DiscountPercent = discountPercent,
            IsService = normalizedType == GianHangProduct_cl.LoaiDichVu,
            OwnerAccountKey = ownerKey
        };
    }

    private static Dictionary<string, long> LoadMirrorMap(dbDataContext db, string ownerAccountKey)
    {
        Dictionary<string, long> map = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
        List<MirrorLookupRow> rows = db.ExecuteQuery<MirrorLookupRow>(
            "SELECT SourceId, LegacyId FROM dbo.CoreGianHangWorkspaceMirror_tb WHERE OwnerAccountKey = {0} AND SourceType = {1} AND LegacyType = {2}",
            ownerAccountKey,
            "gh_product",
            "web_post").ToList();

        for (int i = 0; i < rows.Count; i++)
        {
            MirrorLookupRow row = rows[i];
            string sourceId = (row.SourceId ?? string.Empty).Trim();
            long legacyId;
            if (sourceId == string.Empty || !long.TryParse((row.LegacyId ?? string.Empty).Trim(), out legacyId) || legacyId <= 0)
                continue;
            if (!map.ContainsKey(sourceId))
                map[sourceId] = legacyId;
        }

        return map;
    }
}
