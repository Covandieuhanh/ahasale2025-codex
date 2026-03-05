using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Number_cl
/// </summary>
public class Number_cl
{
    public static int Check_Int(string _input)
    {
        int _kq = 0;
        int.TryParse(_input.Replace(".",""), out _kq);
        return _kq;
    }
    public static Int64 Check_Int64(string _input)
    {
        Int64 _kq = 0;
        Int64.TryParse(_input.Replace(".", ""), out _kq);
        return _kq;
    }
    public static decimal Check_Decimal(string input)
    {
        decimal result;
        if (decimal.TryParse(input.Replace(",", "").Trim(), out result))
            return result;
        return 0;
    }

}