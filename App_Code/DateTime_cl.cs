using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DateTime_cl
/// </summary>
public class DateTime_cl
{
    public static int TinhSoThang_DuNgay(DateTime ngayBatDau, DateTime ngayKetThuc)
    {
        int soThangChenhLech = (ngayKetThuc.Year - ngayBatDau.Year) * 12 + (ngayKetThuc.Month - ngayBatDau.Month);
        if (ngayKetThuc.Day < ngayBatDau.Day)
        {
            soThangChenhLech--;
        }
        return soThangChenhLech;
    }

    public static DateTime timngay_dautien_cuatuan(int _nam, int _tuanthu)
    {
        DateTime jan1 = new DateTime(_nam, 1, 1);
        int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

        // Xác định ngày đầu tiên của tuần yêu cầu
        var cal = CultureInfo.CurrentCulture.Calendar;
        DateTime firstThursday = jan1.AddDays(daysOffset);
        int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

        // Nếu tuần đầu tiên của năm không phải là tuần 1, điều chỉnh số tuần
        if (firstWeek != 1)
        {
            _tuanthu--;
        }

        DateTime result = firstThursday.AddDays(_tuanthu * 7 - 3);
        return result.AddDays(-((int)result.DayOfWeek - (int)DayOfWeek.Monday));
    }

    public static int trave_tuan_trongnam(DateTime date)
    {
        Calendar cal = CultureInfo.CurrentCulture.Calendar;
        return cal.GetWeekOfYear(date, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule,
                                  CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
    }

    public static int trave_quy_trongnam(DateTime _dt)
    {
        int month = _dt.Month;
        return (month - 1) / 3 + 1;
    }

    public DateTime trave_ngaydautuan(DateTime currentDate)
    {
        return currentDate.AddDays(-(int)currentDate.DayOfWeek - 1).Date;
    }

    public bool check_date(string _date)
    {
        DateTime parsed;
        return DateTime.TryParse(_date, out parsed);
    }

    public DateTime return_ngaydautuan()
    {
        return trave_ngaydautuan(AhaTime_cl.Now);
    }

    public DateTime return_ngaycuoituan()
    {
        return return_ngaydautuan().AddDays(6).Date;
    }

    public DateTime return_ngaydauthang(string _thang, string _nam)
    {
        int month = int.Parse(_thang);
        int year = int.Parse(_nam);
        return new DateTime(year, month, 1);
    }

    public DateTime return_ngaycuoithang(string _thang, string _nam)
    {
        DateTime dt = return_ngaydauthang(_thang, _nam);
        return dt.AddMonths(1).AddDays(-1).Date;
    }

    public DateTime return_ngaydauthangtruoc(string _thang, string _nam)
    {
        DateTime dt = return_ngaydauthang(_thang, _nam);
        return dt.AddMonths(-1).Date;
    }

    public DateTime return_ngaycuoithangtruoc(string _thang, string _nam)
    {
        DateTime dt = return_ngaydauthangtruoc(_thang, _nam);
        return dt.AddMonths(1).AddDays(-1).Date;
    }

    public int return_songay_cuathang(string _thang, string _nam)
    {
        return return_ngaycuoithang(_thang, _nam).Day;
    }

    public DateTime return_ngaydaunam(string _nam)
    {
        int year = int.Parse(_nam);
        return new DateTime(year, 1, 1);
    }

    public DateTime return_ngaycuoinam(string _nam)
    {
        return return_ngaydaunam(_nam).AddYears(1).AddDays(-1).Date;
    }

    public DateTime return_ngaydaunamtruoc(string _nam)
    {
        int year = int.Parse(_nam);
        return new DateTime(year - 1, 1, 1);
    }

    public DateTime return_ngaycuoinamtruoc(string _nam)
    {
        return return_ngaydaunamtruoc(_nam).AddYears(1).AddDays(-1).Date;
    }

    public DateTime return_ngaydauquynay(string _thang, string _nam)
    {
        int month = int.Parse(_thang);
        int year = int.Parse(_nam);
        int quarter = (month - 1) / 3;

        switch (quarter)
        {
            case 0:
                return new DateTime(year, 1, 1);
            case 1:
                return new DateTime(year, 4, 1);
            case 2:
                return new DateTime(year, 7, 1);
            default:
                return new DateTime(year, 10, 1);
        }
    }

    public DateTime return_ngaycuoiquynay(string _thang, string _nam)
    {
        int month = int.Parse(_thang);
        int year = int.Parse(_nam);
        int quarter = (month - 1) / 3;

        switch (quarter)
        {
            case 0:
                return new DateTime(year, 3, 31);
            case 1:
                return new DateTime(year, 6, 30);
            case 2:
                return new DateTime(year, 9, 30);
            default:
                return new DateTime(year, 12, 31);
        }
    }

    public DateTime return_ngaydauquytruoc(string _thang, string _nam)
    {
        DateTime dt = return_ngaydauquynay(_thang, _nam);
        return dt.AddMonths(-3);
    }

    public DateTime return_ngaycuoiquytruoc(string _thang, string _nam)
    {
        DateTime dt = return_ngaycuoiquynay(_thang, _nam);
        return dt.AddMonths(-3);
    }

    public string return_thuvietnam(DateTime _dt)
    {
        switch (_dt.DayOfWeek)
        {
            case DayOfWeek.Monday: return "Thứ hai";
            case DayOfWeek.Tuesday: return "Thứ ba";
            case DayOfWeek.Wednesday: return "Thứ tư";
            case DayOfWeek.Thursday: return "Thứ năm";
            case DayOfWeek.Friday: return "Thứ sáu";
            case DayOfWeek.Saturday: return "Thứ bảy";
            case DayOfWeek.Sunday: return "Chủ nhật";
            default: return string.Empty;
        }
    }

    public string return_thuvietnam_viettat(DateTime _dt)
    {
        switch (_dt.DayOfWeek)
        {
            case DayOfWeek.Monday: return "T.2";
            case DayOfWeek.Tuesday: return "T.3";
            case DayOfWeek.Wednesday: return "T.4";
            case DayOfWeek.Thursday: return "T.5";
            case DayOfWeek.Friday: return "T.6";
            case DayOfWeek.Saturday: return "T.7";
            case DayOfWeek.Sunday: return "CN";
            default: return string.Empty;
        }
    }

    public string return_index(DateTime _dt)
    {
        return ((int)_dt.DayOfWeek + 1).ToString();
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
