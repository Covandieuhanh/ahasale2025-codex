using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

public class string_class
{
    public  string remove_blank(string _str) //vd: " ngô   quang  bôn " ==> "ngô quang bôn"
    {
        return (System.Text.RegularExpressions.Regex.Replace(_str, " +", " ").Trim());
    }
    #region lọc dấu TV
    public  string[] VietNamChar = new string[]
    {
        "aAeEoOuUiIdDyY",
        "áàạảãâấầậẩẫăắằặẳẵ",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữ",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ"
    };
    public  string remove_vietnamchar(string str)
    {
        for (int i = 1; i < VietNamChar.Length; i++)
        {
            for (int j = 0; j < VietNamChar[i].Length; j++)
                str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
        }
        return str;
    }
    #endregion
    public  string replace_name_to_url(string _str)
    {
        string str = remove_vietnamchar(_str);
        str = remove_blank(str);
        str = str.Replace(",", "").Replace("(", "").Replace("/", "").Replace("\\", "").Replace(")", "").Replace("\"", "").Replace(":", "").Replace(".", "").Replace("&", "").Replace("-", "").Replace("#", "").Replace("'", "").Replace("?", "").Replace("+", "").Replace("%", "");
        str = remove_blank(str);
        str = str.Replace(" ", "-");
        return str.ToLower();
    }
    public  string taoma()
    {
        return (DateTime.Now.Day > 10 ? DateTime.Now.Day.ToString() : "0" + DateTime.Now.Day.ToString()) + (DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month.ToString()) + DateTime.Now.Year.ToString() + (DateTime.Now.Hour > 9 ? DateTime.Now.Hour.ToString() : "0" + DateTime.Now.Hour.ToString()) + (DateTime.Now.Minute > 9 ? DateTime.Now.Minute.ToString() : "0" + DateTime.Now.Minute.ToString()) + (DateTime.Now.Second > 9 ? DateTime.Now.Second.ToString() : "0" + DateTime.Now.Second.ToString()) + DateTime.Now.Millisecond.ToString();
    }
    public  string VietHoa_ChuCai_DauTien(string s)
    {
        s = remove_blank(s);
        if (String.IsNullOrEmpty(s))
            return s;
        string result = "";
        //lấy danh sách các từ  
        string[] words = s.Split(' ');
        foreach (string word in words)
        {
            // từ nào là các khoảng trắng thừa thì bỏ  
            if (word.Trim() != "")
            {
                if (word.Length > 1)
                    result += word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower() + " ";
                else
                    result += word.ToUpper() + " ";
            }
        }
        return result.Trim();
    }
    public string xuly_sdt(string _sdt)
    {
        return _sdt.Replace(" ", "").Replace(".", "").Replace("-", "");
    }
    public  bool check_user_invalid(string _user)
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
    public bool check_user_invalid_regex(string _user)
    {
        return Regex.IsMatch(_user, @"^[a-zA-Z0-9]+$");
    }
    public bool check_name_invalid(string _name)
    {
        string[] _not_invalid = new string[] { "badmin","login","sanpham"};
        for (int i = 0; i < _not_invalid.Length; i++)
        {
            if (_not_invalid[i] == _name.ToLower())
                return false;
        }
        return true;
    }
    public  string tachho(string _hoten)
    {
        _hoten = remove_blank(_hoten);
        return _hoten.Substring(0, _hoten.IndexOf(' '));
    }
    public  string tachchulot(string _hoten)
    {
        _hoten = remove_blank(_hoten);
        string _ho = _hoten.Substring(0, _hoten.IndexOf(' '));
        return _hoten.Substring(_hoten.IndexOf(' ') + 1, _hoten.LastIndexOf(' ') - _ho.Length - 1);
    }
    public  string tachten(string _hoten)
    {
        _hoten = remove_blank(_hoten);
        return _hoten.Substring(_hoten.LastIndexOf(' ') + 1);
    }
    
}