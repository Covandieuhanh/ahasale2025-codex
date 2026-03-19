using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;


public class datetime_class
{
    public bool check_date(string _date)
    {
        try
        {
            DateTime dt = DateTime.Parse(_date);
            return true;
        }
        catch
        {
            return false;
        }
    }
    public DateTime return_ngaydautuan()
    {
        DateTime dt = DateTime.Now;
        string thuhientai = dt.DayOfWeek.ToString();
        switch (thuhientai)
        {
            case "Monday": return dt; break;
            case "Tuesday": return dt.AddDays(-1).Date; break;
            case "Wednesday": return dt.AddDays(-2).Date; break;
            case "Thursday": return dt.AddDays(-3).Date; break;
            case "Friday": return dt.AddDays(-4).Date; break;
            case "Saturday": return dt.AddDays(-5).Date; break;
            case "Sunday": return dt.AddDays(-6).Date; break;
            default: return dt.Date; break;
        }
    }
    public DateTime return_ngaycuoituan()
    {
        return return_ngaydautuan().AddDays(6).Date;
    }
        
    public DateTime return_ngaydauthang(string _thang, string _nam)
    {
        DateTime dt = DateTime.Parse("01/" + _thang + "/" + _nam);
        return dt.Date;
    }
    public DateTime return_ngaycuoithang(string _thang, string _nam)
    {
        DateTime dt = return_ngaydauthang(_thang, _nam).AddMonths(1).AddDays(-1);
        return dt.Date;
    }
    public DateTime return_ngaydauthangtruoc(string _thang, string _nam)
    {
        DateTime dt = DateTime.Parse("01/" + _thang + "/" + _nam);
        dt = dt.AddMonths(-1);
        return dt.Date;
    }
    public DateTime return_ngaycuoithangtruoc(string _thang, string _nam)
    {
        DateTime dt = return_ngaydauthangtruoc(_thang, _nam).AddMonths(1).AddDays(-1);
        return dt.Date;
    }
    public int return_songay_cuathang(string _thang, string _nam)
    {
        DateTime _ngaycuoithang = return_ngaydauthang(_thang, _nam).AddMonths(1).AddDays(-1);
        return _ngaycuoithang.Day;
    }

    public DateTime return_ngaydaunam(string _nam)
    {
        DateTime dt = DateTime.Parse("01/01/" + _nam);
        return dt.Date;
    }
    public DateTime return_ngaycuoinam(string _nam)
    {
        return return_ngaydaunam(_nam).AddYears(1).AddDays(-1).Date;
    }
    public DateTime return_ngaydaunamtruoc(string _nam)
    {
        DateTime dt = DateTime.Parse("01/01/" + _nam);
        dt = dt.AddYears(-1);
        return dt.Date;
    }
    public DateTime return_ngaycuoinamtruoc(string _nam)
    {
        return return_ngaydaunamtruoc(_nam).AddYears(1).AddDays(-1).Date;
    }
    public DateTime return_ngaydauquynay(string _thang, string _nam)
    {
        DateTime _dt;
        if (_thang == "1" || _thang == "2" || _thang == "3")
        {
            _dt= DateTime.Parse("01/01/" + _nam);
            return _dt;
        }
        else
        {
            if (_thang == "4" || _thang == "5" || _thang == "6")
            {
                _dt = DateTime.Parse("01/04/" + _nam);
                return _dt;
            }
            else
            {
                if (_thang == "7" || _thang == "8" || _thang == "9")
                {
                    _dt = DateTime.Parse("01/07/" + _nam);
                    return _dt;
                }
                else
                {
                    _dt = DateTime.Parse("01/10/" + _nam);
                    return _dt;
                }
            }
        }
    }
    public DateTime return_ngaycuoiquynay(string _thang, string _nam)
    {
        DateTime _dt;
        if (_thang == "1" || _thang == "2" || _thang == "3")
        {
            _dt = DateTime.Parse("31/03/" + _nam);
            return _dt;
        }
        else
        {
            if (_thang == "4" || _thang == "5" || _thang == "6")
            {
                _dt = DateTime.Parse("30/06/" + _nam);
                return _dt;
            }
            else
            {
                if (_thang == "7" || _thang == "8" || _thang == "9")
                {
                    _dt = DateTime.Parse("30/09/" + _nam);
                    return _dt;
                }
                else
                {
                    _dt = DateTime.Parse("31/12/" + _nam);
                    return _dt;
                }
            }
        }
    }
    public DateTime return_ngaydauquytruoc(string _thang, string _nam)
    {
        DateTime _dt;
        if (_thang == "1" || _thang == "2" || _thang == "3")
        {
            _dt = DateTime.Parse("01/01/" + _nam);
            return _dt.AddMonths(-3);
        }
        else
        {
            if (_thang == "4" || _thang == "5" || _thang == "6")
            {
                _dt = DateTime.Parse("01/04/" + _nam);
                return _dt.AddMonths(-3);
            }
            else
            {
                if (_thang == "7" || _thang == "8" || _thang == "9")
                {
                    _dt = DateTime.Parse("01/07/" + _nam);
                    return _dt.AddMonths(-3);
                }
                else
                {
                    _dt = DateTime.Parse("01/10/" + _nam);
                    return _dt.AddMonths(-3);
                }
            }
        }
    }
    public DateTime return_ngaycuoiquytruoc(string _thang, string _nam)
    {
        DateTime _dt;
        if (_thang == "1" || _thang == "2" || _thang == "3")
        {
            _dt = DateTime.Parse("31/03/" + _nam);
            return _dt.AddMonths(-3);
        }
        else
        {
            if (_thang == "4" || _thang == "5" || _thang == "6")
            {
                _dt = DateTime.Parse("30/06/" + _nam);
                return _dt.AddMonths(-3);
            }
            else
            {
                if (_thang == "7" || _thang == "8" || _thang == "9")
                {
                    _dt = DateTime.Parse("30/09/" + _nam);
                    return _dt.AddMonths(-3);
                }
                else
                {
                    _dt = DateTime.Parse("31/12/" + _nam);
                    return _dt.AddMonths(-3);
                }
            }
        }
    }

    public string return_thuvietnam(DateTime _dt)
    {
        string thuhientai = _dt.DayOfWeek.ToString();
        switch (thuhientai)
        {
            case "Monday": return "Thứ hai"; break;
            case "Tuesday": return "Thứ ba"; break;
            case "Wednesday": return "Thứ tư"; break;
            case "Thursday": return "Thứ năm"; break;
            case "Friday": return "Thứ sáu"; break;
            case "Saturday": return "Thứ bảy"; break;
            case "Sunday": return "Chủ nhật"; break;
            default: return ""; break;
        }
    }

    public int return_tuancuanam(DateTime _dt)
    {
        CultureInfo myCI = CultureInfo.CurrentCulture;
        Calendar myCal = myCI.Calendar;
        CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
        DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
        return myCal.GetWeekOfYear(_dt, myCWR, myFirstDOW);
    }
}