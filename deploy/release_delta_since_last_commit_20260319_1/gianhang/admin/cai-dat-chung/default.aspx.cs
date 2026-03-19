using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_Default : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    public string user = "", user_parent = "", notifi, logo_spa;
    public int stt_cp = 1;

    private bspa_caidatchung_table EnsureCaiDatChung()
    {
        var q = db.bspa_caidatchung_tables.Where(p => p.user_parent == user_parent);
        bspa_caidatchung_table ob = q.FirstOrDefault();
        if (ob != null)
            return ob;

        ob = new bspa_caidatchung_table
        {
            user_parent = user_parent,
            chitieu_doanhso_dichvu = 0,
            chitieu_doanhso_mypham = 0
        };
        db.bspa_caidatchung_tables.InsertOnSubmit(ob);
        db.SubmitChanges();
        return ob;
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login  
        string _quyen = "none";
        string _cookie_user = "", _cookie_pass = "";
        if (Request.Cookies["save_user_admin_aka_1"] != null) _cookie_user = Request.Cookies["save_user_admin_aka_1"].Value;
        if (Request.Cookies["save_pass_admin_aka_1"] != null) _cookie_pass = Request.Cookies["save_pass_admin_aka_1"].Value;
        if (Session["user"] == null) Session["user"] = ""; if (Session["notifi"] == null) Session["notifi"] = "";if (Session["user"].ToString() == "") Response.Redirect("/gianhang/admin/f5_ss_admin.aspx");
        string _url = Request.Url.GetLeftPart(UriPartial.Authority).ToLower();
        string _kq = bcorn_class.check_login(Session["user"].ToString(), _cookie_user, _cookie_pass, _url, _quyen);
        if (_kq != "")//nếu có thông báo --> có lỗi --> reset --> bắt login lại
        {
            if (_kq == "baotri") Response.Redirect("/baotri.aspx");
            else
            {
                if (_kq == "1") Response.Redirect("/gianhang/admin/login.aspx");//hết Session, hết Cookie
                else
                {
                    if (_kq == "2")//k đủ quyền
                    {
                        Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
                        Response.Redirect("/gianhang/admin");
                    }
                    else
                    {
                        Session["notifi"] = _kq; Session["user"] = "";
                        Response.Cookies["save_user_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["save_pass_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["save_url_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Redirect("/gianhang/admin/login.aspx");
                    }
                }
            }
        }
        #endregion
        user = Session["user"].ToString();
        user_parent = "admin";

        EnsureCaiDatChung();

        if (!IsPostBack)
        {
            reload();
 
          
        }
    }

    public void reload()
    {
        bspa_caidatchung_table _ob = EnsureCaiDatChung();
        if (_ob != null)
        {
            txt_chitieu_doanhso_dichvu.Text = (_ob.chitieu_doanhso_dichvu ?? 0).ToString("#,##0");
            txt_chitieu_doanhso_mypham.Text = (_ob.chitieu_doanhso_mypham ?? 0).ToString("#,##0");
        }

 
    }

    protected void but_update_chitieu_doanhso_Click(object sender, EventArgs e)
    {
        string _chitieu_dichvu = txt_chitieu_doanhso_dichvu.Text.Trim().Replace(".", "");
        int _r1 = 0;
        int.TryParse(_chitieu_dichvu, out _r1);
        if (_r1 < 0)
            _r1 = 0;

        string _chitieu_mypham = txt_chitieu_doanhso_mypham.Text.Trim().Replace(".", "");
        int _r2 = 0;
        int.TryParse(_chitieu_mypham, out _r2);
        if (_r2 < 0)
            _r2 = 0;

        bspa_caidatchung_table _ob = EnsureCaiDatChung();
        _ob.chitieu_doanhso_dichvu = _r1;
        _ob.chitieu_doanhso_mypham = _r2;
        db.SubmitChanges();

        reload();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Cập nhật thành công.", "4000", "warning"), true);
    }




}
