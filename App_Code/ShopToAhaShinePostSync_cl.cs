using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class ShopToAhaShinePostSync_cl
{
    private static bool ShouldSync(string shopAccount, TimeSpan minInterval)
    {
        if (string.IsNullOrWhiteSpace(shopAccount))
            return true;

        HttpContext ctx = HttpContext.Current;
        if (ctx == null || ctx.Session == null)
            return true;

        string key = "shop_sync_last_" + shopAccount.Trim().ToLowerInvariant();
        object raw = ctx.Session[key];
        DateTime last;
        if (raw is DateTime)
            last = (DateTime)raw;
        else if (raw is string && DateTime.TryParse(raw.ToString(), out last))
        {
        }
        else
        {
            ctx.Session[key] = AhaTime_cl.Now;
            return true;
        }

        if ((AhaTime_cl.Now - last) < minInterval)
            return false;

        ctx.Session[key] = AhaTime_cl.Now;
        return true;
    }

    private static long? ToNullableLong(decimal? value)
    {
        if (!value.HasValue)
            return null;

        return Convert.ToInt64(decimal.Truncate(value.Value));
    }

    public static bool IsSupportedTradePost(BaiViet_tb post)
    {
        return AccountVisibility_cl.IsProductPost(post) || AccountVisibility_cl.IsServicePost(post);
    }

    public static web_post_table SyncTradePost(dbDataContext db, BaiViet_tb post)
    {
        if (db == null || post == null || post.id <= 0 || string.IsNullOrWhiteSpace(post.nguoitao))
            return null;

        string shopAccount = (post.nguoitao ?? "").Trim().ToLowerInvariant();
        string chiNhanhId = AhaShineContext_cl.ResolveChiNhanhIdForShopAccount(db, shopAccount);
        if (string.IsNullOrWhiteSpace(chiNhanhId))
            return null;

        taikhoan_table_2023 adminOwner = db.taikhoan_table_2023s.FirstOrDefault(p =>
            p.user_parent == shopAccount
            && p.id_chinhanh == chiNhanhId
            && p.trangthai == "Đang hoạt động");

        web_post_table mirror = db.web_post_tables.FirstOrDefault(p =>
            p.id_baiviet == post.id
            && p.id_chinhanh == chiNhanhId);

        if (!IsSupportedTradePost(post))
        {
            if (mirror != null)
            {
                mirror.bin = true;
                mirror.hienthi = false;
                mirror.nguoitao = shopAccount;
                db.SubmitChanges();
            }

            return mirror;
        }

        if (mirror == null)
        {
            mirror = new web_post_table();
            db.web_post_tables.InsertOnSubmit(mirror);
        }

        bool isService = AccountVisibility_cl.IsServicePost(post);
        int giavonSanPham = 0;
        if ((post.giavon ?? 0) > 0)
            giavonSanPham = (int)Math.Min(post.giavon.Value, int.MaxValue);

        mirror.id_baiviet = post.id;
        mirror.id_category = (post.id_DanhMuc ?? "").Trim();
        mirror.name = post.name;
        mirror.name_en = post.name_en;
        mirror.content_post = post.content_post;
        mirror.description = post.description;
        mirror.image = post.image;
        mirror.ngaytao = post.ngaytao ?? AhaTime_cl.Now;
        mirror.nguoitao = shopAccount;
        mirror.bin = post.bin == true;
        mirror.noibat = post.noibat == true;
        mirror.phanloai = isService ? "ctdv" : "ctsp";
        mirror.id_nganh = adminOwner == null ? "" : (adminOwner.id_nganh ?? "").Trim();
        mirror.id_chinhanh = chiNhanhId;
        mirror.hienthi = post.bin != true;

        if (isService)
        {
            mirror.giaban_dichvu = ToNullableLong(post.giaban);
            mirror.giaban_sanpham = null;
            mirror.giavon_sanpham = null;
            mirror.soluong_ton_sanpham = null;
            if ((mirror.thoiluong_dichvu_phut ?? 0) <= 0)
                mirror.thoiluong_dichvu_phut = datlich_class.thoiluong_macdinh_dichvu_phut;
        }
        else
        {
            mirror.giaban_sanpham = ToNullableLong(post.giaban);
            mirror.giavon_sanpham = giavonSanPham;
            mirror.soluong_ton_sanpham = post.soluong_tonkho ?? 0;
            mirror.giaban_dichvu = null;
        }

        db.SubmitChanges();
        return mirror;
    }

    public static int SyncTradePostsForShop(dbDataContext db, string shopAccount)
    {
        string tk = (shopAccount ?? "").Trim().ToLowerInvariant();
        if (db == null || string.IsNullOrWhiteSpace(tk))
            return 0;

        List<BaiViet_tb> posts = db.BaiViet_tbs
            .Where(p => p.nguoitao == tk)
            .ToList();

        int synced = 0;
        foreach (BaiViet_tb post in posts)
        {
            if (SyncTradePost(db, post) != null || IsSupportedTradePost(post))
                synced++;
        }

        return synced;
    }

    public static int SyncTradePostsForShopThrottled(dbDataContext db, string shopAccount, int minMinutes = 3)
    {
        if (!ShouldSync(shopAccount, TimeSpan.FromMinutes(Math.Max(1, minMinutes))))
            return 0;
        return SyncTradePostsForShop(db, shopAccount);
    }
}
