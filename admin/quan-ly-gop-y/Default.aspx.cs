using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_Default : System.Web.UI.Page
{
    int PageSize = 10;

    int CurrentPage
    {
        get
        {
            if (ViewState["CurrentPage"] == null)
                ViewState["CurrentPage"] = 1;
            return (int)ViewState["CurrentPage"];
        }
        set
        {
            ViewState["CurrentPage"] = value;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            LoadGopY();
    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        if (CurrentPage > 1)
            CurrentPage--;

        LoadGopY();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            int total = db.DongGopYKien_tbs.Count();
            int maxPage = (int)Math.Ceiling((double)total / PageSize);

            if (CurrentPage < maxPage)
                CurrentPage++;
        }

        LoadGopY();
    }
    void LoadGopY()
    {
        using (dbDataContext db = new dbDataContext())
        {
            int skip = (CurrentPage - 1) * PageSize;

            var query =
                (from yk in db.DongGopYKien_tbs
                 orderby yk.id descending
                 select new
                 {
                     yk.id,
                     yk.taikhoan,
                     yk.HoTen,
                     yk.LoaiVanDe,
                     yk.ChiTietYKien,
                     yk.SoDienThoai,
                     yk.NgayTao,
                     Images = (
                         from map in db.MediaYKien_tbs
                         join m in db.media_tbs on map.idMedia equals m.id
                         where map.idYKien == yk.id
                         select m.link
                     ).ToList()
                 })
                .Skip(skip)
                .Take(PageSize)
                .ToList();

            Repeater1.DataSource = query;
            Repeater1.DataBind();
        }
    }
}
