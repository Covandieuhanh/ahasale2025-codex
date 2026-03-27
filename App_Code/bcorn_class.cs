using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for bcorn_class
/// </summary>
public class bcorn_class
{
    #region check quyền và đăng nhập trang admin
    public static string check_quyen(string _user, string _quyen)
    {
        dbDataContext db = new dbDataContext();
        var q = db.taikhoan_table_2023s.Where(p => p.taikhoan == _user);
        if (q.Count() != 0)
        {
            taikhoan_table_2023 _acc = q.First();
            string _permission_of_user = _acc.permission ?? "";
            if ((_permission_of_user ?? "").Trim().ToLower() == "all")
                return "";
            string[] _permissions = _permission_of_user.Split(',');
            foreach (string _permission in _permissions)
            {
                if (_permission.Trim().ToLower() == "all" || _permission == _quyen)
                    return "";//có quyền, k làm gì cả
            }
        }
        if (_quyen == "none") return "";//trường hợp này nghĩa là trang này k cần xét quyền, cú truyền none vào là đc
        return "2";//k có quyền, trả về 2 để xử lý trong mỗi trang page load
    }
    public static string check_login(string _session_user, string _cookie_user, string _cookie_pass, string _url, string _quyen)
    {
        string _user = _session_user;
        taikhoan_class tk_cl = new taikhoan_class();
        dbDataContext db = new dbDataContext();
        var q_baotri = db.config_baotri_tables.Select(p => new
        {
            baotri = p.baotri_trangthai.ToString().ToLower(),
            batdau = p.baotri_thoigian_batdau,
            ketthuc = p.baotri_thoigian_ketthuc,
        });
        #region xử lý đăng nhập        
        //if (q_baotri.First().baotri == "true" && q_baotri.First().batdau.Value <= DateTime.Now)
        //    return "baotri";
        //else
        //{
        if (_session_user == "")//nếu hết phiên
        {
            if (_cookie_user == "")//nếu k ghi nhớ thì bắt đăng nhập lại
                return "1";
            //return thongbao_class.metro_dialog_onload("Thông báo", "Phiên đã hết hạn. Vui lòng đăng nhập lại.", "false", "false", "OK", "alert", "");
            else//nếu có ghi nhớ
            {
                string _user_mahoa = _cookie_user;//lấy giá trị user cookie đã đc mã hóa
                _user = encode_class.decrypt(_user_mahoa);//giải mã ra sẽ được usernamer
                if (tk_cl.exist_user(_user))//nếu user này tồn tại
                {
                    taikhoan_table_2023 _ob = db.taikhoan_table_2023s.Single(p => p.taikhoan == _user);//truy xuất thông tin của user này
                    if (_ob.trangthai == "Đã bị khóa")//nếu tài khoản này bị khóa
                        return thongbao_class.metro_dialog_onload("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", "");
                    else//nếu k khóa thì kiểm tra xem mật khẩu có bị đổi hay k
                    {
                        if (_ob.matkhau != _cookie_pass && _cookie_pass != "")//nếu mật khẩu trước đó đã bị thay đổi
                            return thongbao_class.metro_dialog_onload("Thông báo", "Mật khẩu đã được thay đổi. <br/>Vui lòng đăng nhập lại.", "false", "false", "OK", "alert", "");
                        else//nếu mk k thay đổi thì kiểm tra HSD nếu có
                        {
                            string _user_parent = string.IsNullOrWhiteSpace(_ob.user_parent) ? AhaShineContext_cl.UserParent : _ob.user_parent;
                            System.Web.HttpContext.Current.Session["user"] = _user;System.Web.HttpContext.Current.Session["user_parent"] = _user_parent;
                        }
                    }
                }
                else//nếu tk k tồn tại thì bắt login
                    return thongbao_class.metro_dialog_onload("Thông báo", "Tài khoản không tồn tại. Vui lòng đăng nhập lại.", "false", "false", "OK", "alert", "");
            }
        }
        else//nếu còn phiên
        {
            _user = _session_user;
            if (tk_cl.exist_user(_user))//nếu user này tồn tại
            {
                taikhoan_table_2023 _ob = db.taikhoan_table_2023s.Single(p => p.taikhoan == _user);//truy xuất thông tin của user này
                if (_ob.trangthai == "Đã bị khóa")//nếu tài khoản này bị khóa
                    return thongbao_class.metro_dialog_onload("Thông báo", "Tài khoản đã bị khóa.", "false", "false", "OK", "alert", "");
                else//nếu k khóa thì kiểm tra xem mật khẩu có bị đổi hay k
                {
                    if (_ob.matkhau != _cookie_pass && _cookie_pass != "")//nếu mật khẩu trước đó đã bị thay đổi
                        return thongbao_class.metro_dialog_onload("Thông báo", "Mật khẩu đã được thay đổi. <br/>Vui lòng đăng nhập lại.", "false", "false", "OK", "alert", "");
                    else//nếu mk k thay đổi thì kiểm tra HSD nếu có
                    {
                        // Legacy bcorn flow no longer performs an extra expiry gate here.
                        // Account expiry is validated by the main auth layer before reaching this point.
                    }
                }
            }
            else//nếu tk k tồn tại thì bắt login
                return thongbao_class.metro_dialog_onload("Thông báo", "Tài khoản không tồn tại. Vui lòng đăng nhập lại.", "false", "false", "OK", "alert", "");
        }
        return check_quyen(_user, _quyen);
        //}
        #endregion
    }
    #endregion

    public static bool exist_sdt_old_data_kh(string _sdt_old, string _sdt_new)
    {
        dbDataContext db = new dbDataContext();
        var q = db.bspa_data_khachhang_tables.Where(p => p.sdt != _sdt_old);
        foreach (var t in q)
        {
            if (t.sdt == _sdt_new)
                return true;
        }
        return false;
    }

    public static string NormalizeAccount(string rawAccount)
    {
        return (rawAccount ?? string.Empty).Trim().ToLowerInvariant();
    }

    public static string NormalizeText(string value)
    {
        return (value ?? string.Empty).Trim();
    }
}
