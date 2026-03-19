using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class webcon_uc_footer_webcon_uc : System.Web.UI.UserControl
{
    public string logo, tencongty, slogan, diachi, hotline, email, zalo, fb, ytb, ins, twitter, tiktok, wechat, linkedin, whatsapp, code_fb, code_map;
    dbDataContext db = new dbDataContext();
    menu_homeaka_class mn_cl = new menu_homeaka_class();
    protected void Page_Load(object sender, EventArgs e)
    {
        var q = db.chinhanh_tables.Where(p=>p.id .ToString()== AhaShineContext_cl.ResolveChiNhanhId());
        if (q.Count() != 0)
        {
            chinhanh_table _ob = q.First();
            logo = _ob.logo_footer;
            tencongty = _ob.ten;
            diachi = _ob.diachi;
            hotline = _ob.sdt;
            email = _ob.email;
            slogan = _ob.slogan;
        }

    }
}