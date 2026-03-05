using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class taikhoan_cl
{
    public static bool exist_taikhoan(string _taikhoan)
    {
        string tk = (_taikhoan ?? "").Trim().ToLower();
        if (string.IsNullOrEmpty(tk)) return false;

        using (dbDataContext db = new dbDataContext())
        {
            return db.taikhoan_tbs.Any(p => (p.taikhoan ?? "").Trim().ToLower() == tk);
        }
    }
}
