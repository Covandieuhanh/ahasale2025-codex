using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class caidatchung_class
{
    
    public static string return_tenspa(string _user_parent)
    {
        dbDataContext db = new dbDataContext();
        var q = db.config_thongtin_tables;
        if (q.Count() != 0)
        {
            config_thongtin_table _ob = q.First();
            return _ob.tencongty;
        }
        return "";
    }
}