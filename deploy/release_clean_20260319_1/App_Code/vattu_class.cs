using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class vattu_class
{
    dbDataContext db = new dbDataContext();

    public bool exist_id(string _id)
    {
        var q = db.danhsach_vattu_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return true;
        return false;
    }
    public danhsach_vattu_table return_object(string _id)
    {
        var q = db.danhsach_vattu_tables.Single(p => p.id.ToString() == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        return q;
    }
    public string return_id(string _name)
    {
        var q = db.danhsach_vattu_tables.Where(p => p.tenvattu == _name.Trim() && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return q.First().id.ToString();
        return "";
    }
}