using System;
using System.Linq;

public static class AhaShineHomeSync_cl
{
    public static void SyncPost(dbDataContext db, web_post_table post)
    {
        if (db == null || post == null)
            return;

        string loai = (post.phanloai ?? "").Trim().ToLowerInvariant();
        if (loai != "ctsp" && loai != "ctdv")
            return;

        string postType = loai == "ctdv" ? AccountVisibility_cl.PostTypeService : AccountVisibility_cl.PostTypeProduct;
        BaiViet_tb bv = null;

        if (post.id_baiviet.HasValue && post.id_baiviet.Value > 0)
        {
            int id = post.id_baiviet.Value;
            bv = db.BaiViet_tbs.FirstOrDefault(p => p.id == id);
        }

        if (bv == null)
        {
            bv = new BaiViet_tb();
            db.BaiViet_tbs.InsertOnSubmit(bv);
        }

        String_cl str = new String_cl();
        string ten = (post.name ?? "").Trim();
        string nguoitao = AhaShineContext_cl.UserParent;

        bv.name = ten;
        bv.name_en = string.IsNullOrEmpty(ten) ? "" : str.replace_name_to_url(ten);
        bv.id_DanhMuc = post.id_category;
        bv.content_post = post.content_post;
        bv.description = post.description;
        bv.image = post.image;
        bv.bin = post.bin == true;
        bv.ngaytao = post.ngaytao ?? DateTime.Now;
        bv.nguoitao = string.IsNullOrEmpty(nguoitao) ? post.nguoitao ?? "" : nguoitao;
        bv.noibat = post.noibat == true;
        bv.phanloai = postType;

        if (loai == "ctsp")
        {
            bv.giaban = post.giaban_sanpham;
            bv.giavon = post.giavon_sanpham;
            bv.soluong_tonkho = post.soluong_ton_sanpham ?? 0;
            bv.soluong_daban = 0;
        }
        else
        {
            bv.giaban = post.giaban_dichvu;
            bv.giavon = 0;
            bv.soluong_tonkho = 0;
            bv.soluong_daban = 0;
        }

        db.SubmitChanges();

        if (!post.id_baiviet.HasValue && bv.id > 0)
        {
            post.id_baiviet = bv.id;
            if (post.id > 0)
                db.ExecuteCommand("UPDATE dbo.web_post_table SET id_baiviet = {0} WHERE id = {1}", bv.id, post.id);
            else
                db.SubmitChanges();
        }
    }

    public static void RemovePost(dbDataContext db, web_post_table post)
    {
        if (db == null || post == null)
            return;

        if (post.id_baiviet.HasValue)
        {
            int id = post.id_baiviet.Value;
            BaiViet_tb bv = db.BaiViet_tbs.FirstOrDefault(p => p.id == id);
            if (bv != null)
            {
                bv.bin = true;
                db.SubmitChanges();
            }
        }
    }
}
