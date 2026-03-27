using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

public static class GianHangArticleSync_cl
{
    public static void EnsureMirrorByChiNhanh(dbDataContext db, string chiNhanhId, string storeAccountKey)
    {
        string normalizedChiNhanhId = (chiNhanhId ?? string.Empty).Trim();
        if (db == null || normalizedChiNhanhId == string.Empty)
            return;

        GianHangSchema_cl.EnsureSchemaSafe(db);

        string normalizedStoreAccount = (storeAccountKey ?? string.Empty).Trim().ToLowerInvariant();
        List<web_post_table> legacyArticles = db.web_post_tables
            .Where(p => p.id_chinhanh == normalizedChiNhanhId && p.phanloai == GianHangArticle_cl.LoaiBaiVietChiTiet)
            .ToList();
        if (legacyArticles.Count == 0)
            return;

        Table<GH_BaiViet_tb> nativeTable = db.GetTable<GH_BaiViet_tb>();
        Dictionary<int, GH_BaiViet_tb> existing = nativeTable
            .Where(p => p.id_chinhanh == normalizedChiNhanhId && p.legacy_post_id != null)
            .ToList()
            .GroupBy(p => p.legacy_post_id.Value)
            .ToDictionary(g => g.Key, g => g.First());

        bool changed = false;
        DateTime now = AhaTime_cl.Now;
        for (int i = 0; i < legacyArticles.Count; i++)
        {
            web_post_table legacy = legacyArticles[i];
            if (legacy.id > int.MaxValue)
                continue;

            int legacyId = (int)legacy.id;
            GH_BaiViet_tb native;
            if (!existing.TryGetValue(legacyId, out native))
            {
                native = new GH_BaiViet_tb
                {
                    legacy_post_id = legacyId,
                    id_chinhanh = normalizedChiNhanhId,
                    shop_taikhoan = normalizedStoreAccount,
                    ngaytao = legacy.ngaytao,
                    ngay_cap_nhat = now
                };
                nativeTable.InsertOnSubmit(native);
                existing[legacyId] = native;
                changed = true;
            }

            changed |= ApplyLegacy(native, legacy, normalizedStoreAccount, now);
        }

        if (changed)
            db.SubmitChanges();
    }

    private static bool ApplyLegacy(GH_BaiViet_tb native, web_post_table legacy, string normalizedStoreAccount, DateTime now)
    {
        bool changed = false;
        if (normalizedStoreAccount != string.Empty
            && !string.Equals(native.shop_taikhoan ?? string.Empty, normalizedStoreAccount, StringComparison.Ordinal))
        {
            native.shop_taikhoan = normalizedStoreAccount;
            changed = true;
        }

        changed |= SetIfChanged(native.id_chinhanh, (legacy.id_chinhanh ?? string.Empty).Trim(), value => native.id_chinhanh = value);
        changed |= SetIfChanged(native.id_category, legacy.id_category, value => native.id_category = value);
        changed |= SetIfChanged(native.name, legacy.name, value => native.name = value);
        changed |= SetIfChanged(native.name_en, legacy.name_en, value => native.name_en = value);
        changed |= SetIfChanged(native.description, legacy.description, value => native.description = value);
        changed |= SetIfChanged(native.content_post, legacy.content_post, value => native.content_post = value);
        changed |= SetIfChanged(native.image, legacy.image, value => native.image = value);
        changed |= SetIfChanged(native.phanloai, legacy.phanloai, value => native.phanloai = value);
        changed |= SetIfChanged(native.noibat, legacy.noibat, value => native.noibat = value);
        changed |= SetIfChanged(native.hienthi, legacy.hienthi, value => native.hienthi = value);
        changed |= SetIfChanged(native.bin, legacy.bin, value => native.bin = value);
        changed |= SetIfChanged(native.ngaytao, legacy.ngaytao, value => native.ngaytao = value);
        if (changed || native.ngay_cap_nhat == null)
        {
            native.ngay_cap_nhat = now;
            return true;
        }

        return false;
    }

    private static bool SetIfChanged<T>(T currentValue, T nextValue, Action<T> assign)
    {
        if (EqualityComparer<T>.Default.Equals(currentValue, nextValue))
            return false;

        assign(nextValue);
        return true;
    }
}
