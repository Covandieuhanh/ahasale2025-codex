using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for hinhanh_thuchi_class
/// </summary>
public class hinhanh_thuchi_class
{
    dbDataContext db = new dbDataContext();
    public bool exist_id(string _id, string _user_parent)
    {
        var q = db.bspa_hinhanhthuchi_tables.Where(p => p.id.ToString() == _id && p.user_parent == _user_parent && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return true;
        return false;
    }
    public bspa_hinhanhthuchi_table return_object(string _id)
    {
        var q = db.bspa_hinhanhthuchi_tables.Single(p => p.id.ToString() == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        return q;
    }
    public void del(string _id)
    {
        bspa_hinhanhthuchi_table _ob = return_object(_id);
        db.bspa_hinhanhthuchi_tables.DeleteOnSubmit(_ob);
        db.SubmitChanges();
    }
}