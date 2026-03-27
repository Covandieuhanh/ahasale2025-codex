using System;
using System.Collections.Generic;
using System.Linq;

public static class GianHangLegacyPost_cl
{
    public static Dictionary<string, BaiViet_tb> LoadMapByReferenceIds(dbDataContext db, IEnumerable<string> rawIds)
    {
        Dictionary<string, BaiViet_tb> map = new Dictionary<string, BaiViet_tb>(StringComparer.OrdinalIgnoreCase);
        if (db == null || rawIds == null)
            return map;

        List<int> ids = rawIds
            .Select(ParseInt)
            .Where(p => p > 0)
            .Distinct()
            .ToList();
        if (ids.Count == 0)
            return map;

        foreach (BaiViet_tb post in db.BaiViet_tbs.Where(p => ids.Contains(p.id)).ToList())
            map[post.id.ToString()] = post;

        return map;
    }

    public static Dictionary<string, int> ResolveNativeIdByReferenceIds(
        dbDataContext db,
        IEnumerable<string> rawIds,
        string sellerAccount = "")
    {
        Dictionary<string, int> map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (db == null || rawIds == null)
            return map;

        List<int> referenceIds = rawIds
            .Select(ParseInt)
            .Where(p => p > 0)
            .Distinct()
            .ToList();
        if (referenceIds.Count == 0)
            return map;

        string sellerKey = (sellerAccount ?? string.Empty).Trim().ToLowerInvariant();
        IQueryable<GH_SanPham_tb> query = db.GetTable<GH_SanPham_tb>()
            .Where(p => p.id_baiviet.HasValue && referenceIds.Contains(p.id_baiviet.Value));
        if (sellerKey != string.Empty)
            query = query.Where(p => p.shop_taikhoan == sellerKey);

        foreach (GH_SanPham_tb product in query.ToList())
        {
            if (!product.id_baiviet.HasValue)
                continue;

            string referenceKey = product.id_baiviet.Value.ToString();
            if (!map.ContainsKey(referenceKey))
                map[referenceKey] = product.id;
        }

        return map;
    }

    private static int ParseInt(string raw)
    {
        int parsed;
        return int.TryParse((raw ?? string.Empty).Trim(), out parsed) ? parsed : 0;
    }
}
