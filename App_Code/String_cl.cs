using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for String_cl
/// </summary>
public class String_cl
{
    //loại bỏ khoảng trắng dư thừa giữa các chữ
    public string Remove_Blank(string input)//vd: " ngô   quang  bôn " ==> "ngô quang bôn"
    {
        return (System.Text.RegularExpressions.Regex.Replace(input, " +", " ").Trim());
    }

    //lọc dấu tiếng việt và chuyển sang tiếng anh
    public string[] VietNamChar = new string[]
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
    public string remove_vietnamchar(string str)
    {
        for (int i = 1; i < VietNamChar.Length; i++)
        {
            for (int j = 0; j < VietNamChar[i].Length; j++)
                str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
        }
        return str;
    }
    public string replace_name_to_url(string _str)
    {
        _str = remove_vietnamchar(_str);
        _str = Remove_Blank(_str);
        _str = Regex.Replace(_str, @"[^a-zA-Z0-9\s]", "");
        _str = Remove_Blank(_str);
        _str = _str.Replace(" ", "-");
        return _str.ToLower();
    }
    public bool check_taikhoan_hople(string account)
    {
        // Kiểm tra null hoặc rỗng
        if (string.IsNullOrEmpty(account))
        {
            return false;
        }

        // Biểu thức chính quy: 5-30 ký tự, không dấu, không khoảng trắng
        string pattern = @"^[a-zA-Z0-9]{5,30}$";
        Regex regex = new Regex(pattern);

        // Kiểm tra tài khoản với biểu thức chính quy
        return regex.IsMatch(account);
    }
    public bool check_user_invalid_regex(string _user)
    {
        return Regex.IsMatch(_user, @"^[a-zA-Z0-9]+$");
    }
    public bool check_name_invalid(string _name)
    {
        string[] _not_invalid = new string[] { "badmin", "login", "sanpham" };
        for (int i = 0; i < _not_invalid.Length; i++)
        {
            if (_not_invalid[i] == _name.ToLower())
                return false;
        }
        return true;
    }
    public string tachho(string _hoten)
    {
        return _hoten.Substring(0, _hoten.IndexOf(' '));
    }
    public string tachchulot(string _hoten)
    {

        string _ho = _hoten.Substring(0, _hoten.IndexOf(' '));
        return _hoten.Substring(_hoten.IndexOf(' ') + 1, _hoten.LastIndexOf(' ') - _ho.Length - 1);
    }
    public string tachten(string _hoten)
    {
        return _hoten.Substring(_hoten.LastIndexOf(' ') + 1);
    }
    public string taoma_theothoigian()
    {
        DateTime now = AhaTime_cl.Now;
        string year = now.Year.ToString();
        string month = now.Month.ToString().PadLeft(2, '0');
        string day = now.Day.ToString().PadLeft(2, '0');
        string hour = now.Hour.ToString().PadLeft(2, '0');
        string minute = now.Minute.ToString().PadLeft(2, '0');
        string second = now.Second.ToString().PadLeft(2, '0');
        string millisecond = now.Millisecond.ToString().PadLeft(3, '0');

        string timestamp = day + "-" + month + "-" + year + "-" + hour + "-" + minute + "-" + second + "-" + millisecond;
        return timestamp;
    }
    public string VietHoa_ChuCai_DauTien(string s)
    {
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
    #region KIỂM TRA SĐT VIỆT NAM
    public string XuLy_SDT_NhapVao(string _sdt)
    {
        // Loại bỏ các ký tự không hợp lệ
        string cleanedNumber = _sdt.Replace(" ", "")
                                   .Replace("+", "")
                                   .Replace("-", "")
                                   .Replace(".", "")
                                   .Replace("(", "")
                                   .Replace(")", "")
                                   .Replace(",", "")
                                   .Replace(";", "");

        // Nếu số bắt đầu bằng "84" thay vì "0", chuyển thành "0"
        if (cleanedNumber.StartsWith("84"))
        {
            cleanedNumber = "0" + cleanedNumber.Substring(2);
        }
        return cleanedNumber;
    }
    private static readonly HashSet<string> validPrefixes = new HashSet<string>
    {
        "032", "033", "034", "035", "036", "037", "038", "039", // Đầu số Viettel
        "056", "058", "059", // Đầu số Vietnamobile
        "070", "076", "077", "078", "079", // Đầu số Mobifone
        "081", "082", "083", "084", "085", "088", "089", // Đầu số Vinaphone
        "090","091", "093", "094", "096", "097", "098", "099" // Đầu số Gmobile và Viettel
    };

    public bool KiemTra_SDT(string phoneNumber)
    {
        // Kiểm tra độ dài của số điện thoại
        if (phoneNumber.Length != 10)
        {
            return false;
        }

        // Biểu thức chính quy để kiểm tra số điện thoại chỉ chứa 10 chữ số
        string pattern = @"^\d{10}$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(phoneNumber);
    }

    #endregion

    public bool KiemTra_Email(string email)
    {
        // Kiểm tra null hoặc chuỗi trống
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        // Biểu thức chính quy kiểm tra email
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        // Kiểm tra email với Regex
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }
}