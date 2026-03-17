using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for nhapvattu_chitiet_class
/// </summary>
public class nhapvattu_chitiet_class
{
    dbDataContext db = new dbDataContext();
    public bool exist_id(string _id, string _user_parent)
    {
        var q = db.donnhap_vattu_chitiet_tables.Where(p => p.id.ToString() == _id && p.user_parent == _user_parent && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return true;
        return false;
    }
    public donnhap_vattu_chitiet_table return_object(string _id)
    {

        var q = db.donnhap_vattu_chitiet_tables.Single(p => p.id.ToString() == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        return q;
    }
    public void del(string _id)
    {
        donnhap_vattu_chitiet_table _ob = return_object(_id);
        db.donnhap_vattu_chitiet_tables.DeleteOnSubmit(_ob);
        db.SubmitChanges();
    }
}