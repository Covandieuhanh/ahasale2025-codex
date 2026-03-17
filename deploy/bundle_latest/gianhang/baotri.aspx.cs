using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_Default2 : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    public string notifi, meta, tongsogiay = "0";
    protected void Timer1_Tick(object sender, EventArgs e)
    {
        main();
    }
    public void main()
    { //tính thời gian bảo trì
        var q = db.config_baotri_tables;

        if (q.First().baotri_trangthai == true)
        {
            DateTime _dt_baotri = q.First().baotri_thoigian_ketthuc.Value;
            TimeSpan _ts = _dt_baotri - DateTime.Now;
            int _songay = _ts.Days * 84600;
            int _sogio = _ts.Hours * 3600;
            int _sophut = _ts.Minutes * 60;
            int _sogiay = _ts.Seconds;
            int _tongsogiay = _songay + _sogio + _sophut + _sogiay;
            tongsogiay = _tongsogiay.ToString();
            if (_tongsogiay <= 0)
            {
                config_baotri_table _ob = q.First();
                _ob.baotri_trangthai = false;
                db.SubmitChanges();
                Response.Redirect(AhaShineHomeRoutes_cl.HomeUrl);
            }
        }
        else
            Response.Redirect(AhaShineHomeRoutes_cl.HomeUrl);

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            #region meta 
            if (db.config_baotri_tables.First().baotri_trangthai == false)
                Response.Redirect(AhaShineHomeRoutes_cl.HomeUrl);
            else
            {
                var q = db.config_thongtin_tables;
                string _icon = "<link rel=\"shortcut icon\" type=\"image/x-icon\" href=\"" + q.First().icon + "\" />";
                string _appletouch = "<link rel=\"apple-touch-icon\" href=\"" + q.First().apple_touch_icon + "\" />";
                meta = _icon;
            }

            #endregion
            main();
        }
    }
}