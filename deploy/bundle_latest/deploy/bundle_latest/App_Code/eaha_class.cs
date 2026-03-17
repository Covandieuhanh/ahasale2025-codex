using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class eaha_class
{
   public static double quydoi_vnd_sang_eaha(double _vnd)
    {
        double _kq;
        if (_vnd > 0)
        {
            _kq = _vnd / 24000;
            return _kq;
        }
        return 0;
    }
}