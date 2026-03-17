using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Summary description for khachhang_class
/// </summary>
public class khachhang_class
{
    dbDataContext db = new dbDataContext();
    random_class rd_cl = new random_class();
    public khachhang_table_2023 return_object(string _user)
    {
        var q = db.khachhang_table_2023s.Single(p => p.taikhoan.ToString() == _user);
        return q;
    }

    public bool exist_user(string _user)
    {
        var q = db.khachhang_table_2023s.Where(p => p.taikhoan.ToString() == _user);
        if (q.Count() != 0)
            return true;
        return false;
    }
    public bool exist_email(string _email)
    {
        var q = db.khachhang_table_2023s.Where(p => p.email == _email);
        if (q.Count() != 0)
            return true;
        return false;
    }
    public static bool check_user_invalid_regex(string _user)
    {
        return Regex.IsMatch(_user, @"^[a-zA-Z0-9]+$");
    }
    public bool check_user_invalid(string _user)
    {
        //a-zA-Z0-9
        Char[] mang = _user.ToCharArray();
        for (int i = 0; i < mang.Length; i++)
        {
            byte _kdoce_ASCII = (byte)(mang[i]);
            if (!(_kdoce_ASCII >= 48 && _kdoce_ASCII <= 57) && !(_kdoce_ASCII >= 65 && _kdoce_ASCII <= 90) && !(_kdoce_ASCII >= 97 && _kdoce_ASCII <= 122))
                return false;
        }
        return true;
    }
    public bool check_name_invalid(string _name)
    {
        string[] _not_invalid = new string[] { "admin", "home", "keno", "bspa", "bcorn", "badmin", "bcard" };
        for (int i = 0; i < _not_invalid.Length; i++)
        {
            if (_not_invalid[i] == _name.ToLower())
                return false;
        }
        return true;
    }
    public bool exist_email_old(string _email_old, string _email_new)
    {
        var q = db.khachhang_table_2023s.Where(p => p.email != _email_old);
        foreach (var t in q)
        {
            if (t.email == _email_new)
                return true;
        }
        return false;
    }
    public string return_user(string _email)
    {
        var q = db.khachhang_table_2023s.Where(p => p.email == _email);
        if (q.Count() != 0)
            return q.First().taikhoan;
        return "";
    }
    public void change_makhoiphuc(string _user, string _code)
    {
        var q = db.khachhang_table_2023s.Where(p => p.taikhoan == _user).First();
        khachhang_table_2023 _ob = q;
        _ob.makhoiphuc = _code;
        _ob.hsd_makhoiphuc = DateTime.Now;
        db.SubmitChanges();
    }
    public void change_pass(string _user, string _newpass)
    {
        var q = db.khachhang_table_2023s.Where(p => p.taikhoan == _user).First();
        khachhang_table_2023 _ob = q;
        _ob.matkhau = encode_class.encode_md5(encode_class.encode_sha1(_newpass));
        _ob.makhoiphuc = rd_cl.random_number(6);
        db.SubmitChanges();
    }
    public void update_quyen(string _user, string _quyen)
    {
        var q = db.khachhang_table_2023s.Where(p => p.taikhoan == _user).First();
        khachhang_table_2023 _ob = q;
        _ob.permission = _quyen;
        db.SubmitChanges();
    }
}