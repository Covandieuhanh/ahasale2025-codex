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
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        user = (access.User ?? "").Trim();
        user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();

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
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

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
