using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
public class thuchi_class
{
    dbDataContext db = new dbDataContext();
    public bool exist_id(string _id, string _user_parent)
    {
        var q = db.bspa_thuchi_tables.Where(p => p.id.ToString() == _id && p.user_parent == _user_parent && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return true;
        return false;
    }
    public bspa_thuchi_table return_object(string _id)
    {

        var q = db.bspa_thuchi_tables.Single(p => p.id.ToString() == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        return q;
    }
    public void del(string _id)
    {
        bspa_thuchi_table _ob = return_object(_id);
        if (HoaDonThuChiSync_cl.IsAutoSystemEntry(_ob))
            return;
        db.bspa_thuchi_tables.DeleteOnSubmit(_ob);
        db.SubmitChanges();
    }
    public string return_maxid(string _user_parent)
    {
        return db.bspa_thuchi_tables.Where(p => p.user_parent == _user_parent && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).Max(p => p.id).ToString();
    }
   
}
