using System;

public partial class bat_dong_san_chi_tiet_lien_ket : BatDongSanPageBase
{
    protected LinkedFeedStore_cl.LinkedPost Post;

    protected void Page_Load(object sender, EventArgs e)
    {
        int id;
        if (!int.TryParse(Request.QueryString["id"], out id))
            id = 0;

        using (dbDataContext db = new dbDataContext())
        {
            Post = LinkedFeedStore_cl.GetById(db, id);
        }

        phDetail.Visible = Post != null;
        phEmpty.Visible = Post == null;
    }

    protected string ResolveSourceUrl()
    {
        if (Post == null || string.IsNullOrWhiteSpace(Post.SourceUrl))
            return "#";
        return Post.SourceUrl;
    }
}
