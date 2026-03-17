using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;

public class regex_class
{
    public static bool check_user_invalid(string _user)
    {
        return Regex.IsMatch(_user, @"^[a-zA-Z0-9]+$");
    }
    //Kiểm tra xem có phải là Email không
    public static bool check_email_invalid(string _email)
    {
        return Regex.IsMatch(_email.Trim(), @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
    }
    //Loại bỏ thẻ tag html
    public static string TrimHtmlTags(string input)
    {
        if (string.IsNullOrEmpty(input))
            return null;
        input = HttpUtility.HtmlDecode(input);
        return Regex.Replace(input, @"<.[^>]*>", string.Empty);
    }
    //Chuyển chuỗi có dấu sang không dấu
    public static string ToVietNameseChacracter(string s)
    {
        if (string.IsNullOrEmpty(s))
            return string.Empty;
        Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
        string temp = s.Normalize(NormalizationForm.FormD);
        return regex.Replace(temp, String.Empty);
    }
    // Kiểm tra có phải là số điện thoại di động ở Việt Nam không
    public bool IsValidVietNamPhoneNumber(string phoneNum)
    {
        if (string.IsNullOrEmpty(phoneNum))
            return false;
        string sMailPattern = @"^((09(\d){8})|(086(\d){7})|(088(\d){7})|(089(\d){7})|(01(\d){9}))$";
        return Regex.IsMatch(phoneNum.Trim(), sMailPattern);
    }

}