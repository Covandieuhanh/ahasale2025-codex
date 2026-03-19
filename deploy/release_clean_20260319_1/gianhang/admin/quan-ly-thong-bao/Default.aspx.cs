using System;
using System.Linq;
using System.Web.UI;

public partial class admin_quan_ly_thong_bao_Default : Page
{
    dbDataContext db = new dbDataContext();
    taikhoan_class tk_cl = new taikhoan_class();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] == null || string.IsNullOrWhiteSpace(Session["user"].ToString()))
        {
            Response.Redirect("/gianhang/admin/login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            mark_all_as_read(Session["user"].ToString());
            load_data(Session["user"].ToString());
        }
    }

    private void load_data(string user)
    {
        var list = (from t in db.thongbao_tables.Where(p => p.nguoinhan == user).OrderByDescending(p => p.thoigian).ToList()
                    select new
                    {
                        daxem = t.daxem.HasValue ? t.daxem.Value : false,
                        noidung = string.IsNullOrWhiteSpace(t.noidung) ? "(trống)" : t.noidung,
                        thoigian = t.thoigian.HasValue ? t.thoigian.Value.ToString("dd/MM/yyyy HH:mm") : "",
                        nguoithongbao = get_sender_name(t.nguoithongbao),
                        link_full = build_link(t.link, t.id.ToString())
                    }).ToList();

        Repeater1.DataSource = list;
        Repeater1.DataBind();

        PanelEmpty.Visible = list.Count == 0;
    }

    private string get_sender_name(string user)
    {
        if (string.IsNullOrWhiteSpace(user))
            return "(hệ thống)";

        string hoten = tk_cl.return_hoten(user);
        if (!string.IsNullOrWhiteSpace(hoten))
            return hoten;
        return user;
    }

    private string build_link(string link, string id)
    {
        if (string.IsNullOrWhiteSpace(link))
            return "/gianhang/admin/Default.aspx";

        if (link.IndexOf("?") == -1)
            return link + "?idtb=" + id;

        return link + "&idtb=" + id;
    }

    private void mark_all_as_read(string user)
    {
        var list = db.thongbao_tables.Where(p => p.nguoinhan == user && p.daxem == false).ToList();
        if (list.Count == 0)
            return;

        foreach (var item in list)
            item.daxem = true;

        db.SubmitChanges();
    }
}
