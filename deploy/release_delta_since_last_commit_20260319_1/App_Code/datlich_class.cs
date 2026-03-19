using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

public class datlich_input
{
    public string tenkhachhang { get; set; }
    public string sdt { get; set; }
    public DateTime ngaydat { get; set; }
    public string dichvu { get; set; }
    public string nhanvien_thuchien { get; set; }
    public string ghichu { get; set; }
    public string trangthai { get; set; }
    public string nguongoc { get; set; }
    public int thoiluong_dichvu_phut { get; set; }
    public DateTime ngayketthucdukien { get; set; }
    public string id_nganh_dichvu { get; set; }
}

public class datlich_validate_result
{
    public bool hop_le { get; set; }
    public string thongbao { get; set; }
    public datlich_input dulieu { get; set; }
}

public class datlich_class
{
    public const string nguoitao_khach_online = "Khách online";
    public const string trangthai_chua_xacnhan = "Chưa xác nhận";
    public const string trangthai_da_xacnhan = "Đã xác nhận";
    public const string trangthai_khong_den = "Không đến";
    public const string trangthai_da_den = "Đã đến";
    public const string trangthai_da_huy = "Đã hủy";
    public const int thoiluong_macdinh_dichvu_phut = 60;
    public const int thoiluong_toi_thieu_dichvu_phut = 5;
    public const int thoiluong_toi_da_dichvu_phut = 480;

    private static readonly string[] ngay_formats = new string[] { "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy" };
    private static readonly string[] ngaygio_formats = new string[] { "dd/MM/yyyy HH:mm", "d/M/yyyy HH:mm", "dd/M/yyyy HH:mm", "d/MM/yyyy HH:mm" };
    private static readonly CultureInfo vi_culture = CultureInfo.GetCultureInfo("vi-VN");
    private static readonly string[] trangthai_giu_slot = new string[] { trangthai_chua_xacnhan, trangthai_da_xacnhan, trangthai_da_den };

    public static string return_lich_chauxacnhan()
    {
        string id_chinhanh = "";
        if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["chinhanh"] != null)
            id_chinhanh = System.Web.HttpContext.Current.Session["chinhanh"].ToString();
        if (id_chinhanh == "")
            return "0";

        dbDataContext db = new dbDataContext();
        var list_all = db.bspa_datlich_tables.Where(p => p.trangthai == trangthai_chua_xacnhan && p.id_chinhanh == id_chinhanh);
        return list_all.Count().ToString("#,##0");
    }

    public static string return_ngay_text(DateTime _dt)
    {
        return _dt.ToString("dd/MM/yyyy");
    }

    public static void bind_gio_phut(DropDownList ddl_gio, DropDownList ddl_phut, DateTime? _selected)
    {
        if (ddl_gio == null || ddl_phut == null)
            return;

        ddl_gio.Items.Clear();
        ddl_phut.Items.Clear();

        for (int i = 0; i <= 23; i++)
        {
            string _value = i.ToString("00");
            ddl_gio.Items.Add(new ListItem(_value + " giờ", _value));
        }

        for (int i = 0; i <= 59; i++)
        {
            string _value = i.ToString("00");
            ddl_phut.Items.Add(new ListItem(_value + " phút", _value));
        }

        DateTime _time = _selected.HasValue ? _selected.Value : DateTime.Now;
        try_select_dropdown_value(ddl_gio, _time.Hour.ToString("00"));
        try_select_dropdown_value(ddl_phut, _time.Minute.ToString("00"));
    }

    public static bool try_select_dropdown_value(DropDownList ddl, string _value)
    {
        if (ddl == null || string.IsNullOrWhiteSpace(_value))
            return false;

        ListItem _item = ddl.Items.FindByValue(_value.Trim());
        if (_item == null)
            return false;

        ddl.ClearSelection();
        _item.Selected = true;
        return true;
    }

    public static string chuanhoa_tenkhachhang(string _value)
    {
        string_class str_cl = new string_class();
        return str_cl.VietHoa_ChuCai_DauTien(str_cl.remove_blank((_value ?? "").Trim().ToLower()));
    }

    public static string chuanhoa_sdt(string _value)
    {
        return (_value ?? "").Trim().Replace(" ", "").Replace(".", "").Replace("-", "").Replace("+", "");
    }

    public static string chuanhoa_nguongoc(string _value, string _macdinh)
    {
        string _result = (_value ?? "").Trim();
        if (_result == "")
            _result = _macdinh;
        return _result;
    }

    public static string chuanhoa_trangthai(string _value)
    {
        switch ((_value ?? "").Trim())
        {
            case trangthai_da_xacnhan: return trangthai_da_xacnhan;
            case trangthai_khong_den: return trangthai_khong_den;
            case trangthai_da_den: return trangthai_da_den;
            case trangthai_da_huy: return trangthai_da_huy;
            case trangthai_chua_xacnhan:
            default:
                return trangthai_chua_xacnhan;
        }
    }

    public static int chuanhoa_thoiluong_dichvu_phut(int? _value)
    {
        int _result = _value ?? 0;
        if (_result < thoiluong_toi_thieu_dichvu_phut || _result > thoiluong_toi_da_dichvu_phut)
            return thoiluong_macdinh_dichvu_phut;
        return _result;
    }

    public static bool la_trangthai_giu_slot(string _value)
    {
        return trangthai_giu_slot.Contains(chuanhoa_trangthai(_value));
    }

    public static string return_khoang_thoi_gian_text(DateTime _batdau, DateTime _ketthuc)
    {
        return _batdau.ToString("HH:mm") + " - " + _ketthuc.ToString("HH:mm") + " ngày " + _batdau.ToString("dd/MM/yyyy");
    }

    public static string return_ten_nguoitao_hienthi(string _nguoitao)
    {
        string _result = (_nguoitao ?? "").Trim();
        if (_result == "")
            return "";

        taikhoan_class tk_cl = new taikhoan_class();
        string _hoten = tk_cl.return_hoten(_result);
        if (_hoten != "")
            return _hoten;

        return _result;
    }

    public static bool try_parse_ngay(string _value, out DateTime _ngay)
    {
        return DateTime.TryParseExact((_value ?? "").Trim(), ngay_formats, vi_culture, DateTimeStyles.None, out _ngay);
    }

    public static bool try_parse_ngaygio(string _ngay, string _gio, string _phut, out DateTime _ngaygio)
    {
        string _gio_text = (_gio ?? "").Trim();
        string _phut_text = (_phut ?? "").Trim();

        if (_gio_text == "")
            _gio_text = "00";
        if (_phut_text == "")
            _phut_text = "00";

        if (_gio_text.Length == 1)
            _gio_text = "0" + _gio_text;
        if (_phut_text.Length == 1)
            _phut_text = "0" + _phut_text;

        string _value = ((_ngay ?? "").Trim() + " " + _gio_text + ":" + _phut_text).Trim();
        return DateTime.TryParseExact(_value, ngaygio_formats, vi_culture, DateTimeStyles.None, out _ngaygio);
    }

    public static datlich_validate_result chuanhoa_du_lieu(string _tenkhachhang, string _sdt, string _ngay, string _gio, string _phut, string _dichvu, string _nhanvien, string _ghichu, string _trangthai, string _nguongoc, string _nguongoc_macdinh, bool _allow_past_date)
    {
        datlich_validate_result _result = new datlich_validate_result();
        datlich_input _input = new datlich_input();

        _input.tenkhachhang = chuanhoa_tenkhachhang(_tenkhachhang);
        _input.sdt = chuanhoa_sdt(_sdt);
        _input.dichvu = (_dichvu ?? "").Trim();
        _input.nhanvien_thuchien = (_nhanvien ?? "").Trim();
        _input.ghichu = (_ghichu ?? "").Trim();
        _input.trangthai = chuanhoa_trangthai(_trangthai);
        _input.nguongoc = chuanhoa_nguongoc(_nguongoc, _nguongoc_macdinh);

        if (_input.tenkhachhang == "")
        {
            _result.hop_le = false;
            _result.thongbao = "Vui lòng nhập tên khách hàng.";
            return _result;
        }

        if (_input.sdt == "")
        {
            _result.hop_le = false;
            _result.thongbao = "Vui lòng nhập số điện thoại.";
            return _result;
        }

        DateTime _ngaygio;
        if (try_parse_ngaygio(_ngay, _gio, _phut, out _ngaygio) == false)
        {
            _result.hop_le = false;
            _result.thongbao = "Ngày giờ đặt không hợp lệ.";
            return _result;
        }

        if (_allow_past_date == false && _ngaygio < DateTime.Now)
        {
            _result.hop_le = false;
            _result.thongbao = "Vui lòng chọn thời gian hẹn trong tương lai.";
            return _result;
        }

        _input.ngaydat = _ngaygio;

        _result.hop_le = true;
        _result.dulieu = _input;
        _result.thongbao = "";
        return _result;
    }

    public static string kiemtra_quy_tac_van_hanh(dbDataContext db, datlich_input _input, string _id_chinhanh, long? _id_boqua, bool _batbuoc_dichvu)
    {
        if (_input == null)
            return "Dữ liệu lịch hẹn không hợp lệ.";

        if (_batbuoc_dichvu && string.IsNullOrWhiteSpace(_input.dichvu) && _input.trangthai != trangthai_da_huy)
            return "Vui lòng chọn dịch vụ.";

        if ((_input.trangthai == trangthai_da_xacnhan || _input.trangthai == trangthai_da_den) && string.IsNullOrWhiteSpace(_input.nhanvien_thuchien))
            return "Vui lòng chọn nhân viên thực hiện trước khi xác nhận lịch.";

        if ((_input.trangthai == trangthai_da_den || _input.trangthai == trangthai_khong_den) && _input.ngaydat > DateTime.Now)
            return "Chỉ có thể chuyển sang trạng thái đã đến hoặc không đến khi lịch đã tới giờ hẹn.";

        if (db == null || string.IsNullOrWhiteSpace(_id_chinhanh))
            return "";

        dongbo_thongtin_slot_vao_input(db, _input, _id_chinhanh, _id_boqua);

        string _loi_nhanvien = kiemtra_nhanvien_phuhop_dichvu(db, _input, _id_chinhanh);
        if (_loi_nhanvien != "")
            return _loi_nhanvien;

        if (_input.ngaydat >= DateTime.Now && la_trangthai_giu_slot(_input.trangthai))
        {
            List<bspa_datlich_table> _list_trung_slot = lay_ds_lich_trung_slot(db, _id_chinhanh, _input.ngaydat, _input.ngayketthucdukien, _id_boqua);
            bspa_datlich_table _xungdot_khach = _list_trung_slot.FirstOrDefault(p => chuanhoa_sdt(p.sdt) == _input.sdt);
            if (_xungdot_khach != null)
                return "Khách hàng đã có lịch khác trùng trong khung " + return_khoang_thoi_gian_text(_xungdot_khach.ngaydat.Value, return_ngayketthuc_cua_lich(_xungdot_khach)) + ".";

            if (!string.IsNullOrWhiteSpace(_input.nhanvien_thuchien))
            {
                string _nhanvien = (_input.nhanvien_thuchien ?? "").Trim();
                bspa_datlich_table _xungdot_nhanvien = _list_trung_slot.FirstOrDefault(p => string.Equals((p.nhanvien_thuchien ?? "").Trim(), _nhanvien, StringComparison.OrdinalIgnoreCase));
                if (_xungdot_nhanvien != null)
                    return "Nhân viên này đang bận trong khung " + return_khoang_thoi_gian_text(_xungdot_nhanvien.ngaydat.Value, return_ngayketthuc_cua_lich(_xungdot_nhanvien)) + ".";
            }

            string _loi_congsuat = kiemtra_cong_suat_chinhanh(db, _input, _id_chinhanh, _list_trung_slot);
            if (_loi_congsuat != "")
                return _loi_congsuat;
        }

        return "";
    }

    private static web_post_table return_dichvu_hoatdong(dbDataContext db, string _id_dichvu, string _id_chinhanh)
    {
        if (db == null || string.IsNullOrWhiteSpace(_id_dichvu) || string.IsNullOrWhiteSpace(_id_chinhanh))
            return null;

        return db.web_post_tables.FirstOrDefault(p => p.id.ToString() == _id_dichvu && p.id_chinhanh == _id_chinhanh && p.bin == false);
    }

    private static BaiViet_tb return_dichvu_shop_hoatdong(dbDataContext db, string _id_dichvu)
    {
        if (db == null || string.IsNullOrWhiteSpace(_id_dichvu))
            return null;

        int _id;
        if (int.TryParse(_id_dichvu.Trim(), out _id) == false)
            return null;

        return db.BaiViet_tbs.FirstOrDefault(p =>
            p.id == _id
            && (p.bin == false || p.bin == null)
            && (p.phanloai ?? "").Trim() == AccountVisibility_cl.PostTypeService);
    }

    private static bspa_datlich_table return_lich_hienco(dbDataContext db, string _id_chinhanh, long? _id)
    {
        if (db == null || string.IsNullOrWhiteSpace(_id_chinhanh) || _id.HasValue == false)
            return null;

        return db.bspa_datlich_tables.FirstOrDefault(p => p.id == _id.Value && p.id_chinhanh == _id_chinhanh);
    }

    public static string return_ten_dichvu(dbDataContext db, string _id_dichvu, string _id_chinhanh)
    {
        web_post_table _dv = return_dichvu_hoatdong(db, _id_dichvu, _id_chinhanh);
        if (_dv != null)
            return (_dv.name ?? "").Trim();

        BaiViet_tb _shopService = return_dichvu_shop_hoatdong(db, _id_dichvu);
        return _shopService == null ? "" : (_shopService.name ?? "").Trim();
    }

    public static string return_id_nganh_dichvu(dbDataContext db, string _id_dichvu, string _id_chinhanh)
    {
        web_post_table _dv = return_dichvu_hoatdong(db, _id_dichvu, _id_chinhanh);
        if (_dv != null)
            return (_dv.id_nganh ?? "").Trim();

        return "";
    }

    public static int return_thoiluong_dichvu_phut(dbDataContext db, string _id_dichvu, string _id_chinhanh)
    {
        web_post_table _dv = return_dichvu_hoatdong(db, _id_dichvu, _id_chinhanh);
        if (_dv != null)
            return chuanhoa_thoiluong_dichvu_phut(_dv.thoiluong_dichvu_phut);

        return chuanhoa_thoiluong_dichvu_phut(null);
    }

    public static DateTime return_ngayketthuc_cua_lich(bspa_datlich_table _lich)
    {
        if (_lich == null)
            return DateTime.Now;

        DateTime _batdau = _lich.ngaydat ?? DateTime.Now;
        if (_lich.ngayketthucdukien.HasValue)
            return _lich.ngayketthucdukien.Value;

        return _batdau.AddMinutes(chuanhoa_thoiluong_dichvu_phut(_lich.thoiluong_dichvu_phut));
    }

    private static bool co_giao_nhau_khung_gio(DateTime _batdau_a, DateTime _ketthuc_a, DateTime _batdau_b, DateTime _ketthuc_b)
    {
        return _batdau_a < _ketthuc_b && _ketthuc_a > _batdau_b;
    }

    private static List<bspa_datlich_table> lay_ds_lich_trung_slot(dbDataContext db, string _id_chinhanh, DateTime _batdau_moi, DateTime _ketthuc_moi, long? _id_boqua)
    {
        if (db == null || string.IsNullOrWhiteSpace(_id_chinhanh))
            return new List<bspa_datlich_table>();

        DateTime _moc_tu = _batdau_moi.AddMinutes(-thoiluong_toi_da_dichvu_phut);
        var q = db.bspa_datlich_tables.Where(p =>
            p.id_chinhanh == _id_chinhanh &&
            p.ngaydat.HasValue &&
            p.ngaydat.Value < _ketthuc_moi &&
            p.ngaydat.Value > _moc_tu &&
            trangthai_giu_slot.Contains(p.trangthai)
        );

        if (_id_boqua.HasValue)
            q = q.Where(p => p.id != _id_boqua.Value);

        return q.ToList()
            .Where(p => co_giao_nhau_khung_gio(_batdau_moi, _ketthuc_moi, p.ngaydat.Value, return_ngayketthuc_cua_lich(p)))
            .ToList();
    }

    private static int return_thoiluong_cho_input(dbDataContext db, datlich_input _input, string _id_chinhanh, long? _id_boqua)
    {
        if (_input == null)
            return thoiluong_macdinh_dichvu_phut;

        if (_input.thoiluong_dichvu_phut > 0)
            return chuanhoa_thoiluong_dichvu_phut(_input.thoiluong_dichvu_phut);

        bspa_datlich_table _lich_hienco = return_lich_hienco(db, _id_chinhanh, _id_boqua);
        if (_lich_hienco != null)
        {
            string _dichvu_cu = (_lich_hienco.dichvu ?? "").Trim();
            string _dichvu_moi = (_input.dichvu ?? "").Trim();
            if ((_dichvu_moi == "" || _dichvu_moi == _dichvu_cu) && (_lich_hienco.thoiluong_dichvu_phut ?? 0) > 0)
                return chuanhoa_thoiluong_dichvu_phut(_lich_hienco.thoiluong_dichvu_phut);
        }

        return return_thoiluong_dichvu_phut(db, _input.dichvu, _id_chinhanh);
    }

    private static void dongbo_thongtin_slot_vao_input(dbDataContext db, datlich_input _input, string _id_chinhanh, long? _id_boqua)
    {
        if (_input == null)
            return;

        _input.thoiluong_dichvu_phut = return_thoiluong_cho_input(db, _input, _id_chinhanh, _id_boqua);
        _input.ngayketthucdukien = _input.ngaydat.AddMinutes(_input.thoiluong_dichvu_phut);

        if (string.IsNullOrWhiteSpace(_input.id_nganh_dichvu))
        {
            _input.id_nganh_dichvu = return_id_nganh_dichvu(db, _input.dichvu, _id_chinhanh);
            if (_input.id_nganh_dichvu == "")
            {
                bspa_datlich_table _lich_hienco = return_lich_hienco(db, _id_chinhanh, _id_boqua);
                if (_lich_hienco != null)
                    _input.id_nganh_dichvu = return_id_nganh_dichvu(db, _lich_hienco.dichvu, _id_chinhanh);
            }
        }
    }

    private static taikhoan_table_2023 return_nhanvien_hoatdong(dbDataContext db, string _taikhoan, string _id_chinhanh)
    {
        if (db == null || string.IsNullOrWhiteSpace(_taikhoan) || string.IsNullOrWhiteSpace(_id_chinhanh))
            return null;

        return db.taikhoan_table_2023s.FirstOrDefault(p => p.taikhoan == _taikhoan && p.id_chinhanh == _id_chinhanh && p.trangthai == "Đang hoạt động");
    }

    private static List<string> lay_ds_nhanvien_du_dieu_kien(dbDataContext db, string _id_chinhanh, string _id_nganh)
    {
        if (db == null || string.IsNullOrWhiteSpace(_id_chinhanh))
            return new List<string>();

        var q = db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_chinhanh == _id_chinhanh);
        if (string.IsNullOrWhiteSpace(_id_nganh) == false)
            q = q.Where(p => p.id_nganh == _id_nganh || p.id_nganh == null || p.id_nganh == "");

        return q.Select(p => p.taikhoan).Where(p => p != null && p != "").Distinct().ToList();
    }

    private static bool is_level2_chinhanh(dbDataContext db, string _id_chinhanh)
    {
        if (db == null || string.IsNullOrWhiteSpace(_id_chinhanh))
            return false;

        try
        {
            chinhanh_table cn = db.chinhanh_tables.FirstOrDefault(p => p.id.ToString() == _id_chinhanh);
            string owner = (cn == null ? "" : (cn.taikhoan_quantri ?? "")).Trim().ToLower();
            if (owner == "")
                return false;
            return ShopLevel_cl.IsAdvancedEnabled(db, owner);
        }
        catch
        {
            return false;
        }
    }

    private static string kiemtra_nhanvien_phuhop_dichvu(dbDataContext db, datlich_input _input, string _id_chinhanh)
    {
        if (db == null || _input == null || string.IsNullOrWhiteSpace(_input.nhanvien_thuchien))
            return "";

        taikhoan_table_2023 _nhanvien = return_nhanvien_hoatdong(db, _input.nhanvien_thuchien, _id_chinhanh);
        if (_nhanvien == null)
            return "Nhân viên thực hiện không còn hoạt động tại chi nhánh.";

        string _id_nganh = (_input.id_nganh_dichvu ?? "").Trim();
        string _id_nganh_nhanvien = (_nhanvien.id_nganh ?? "").Trim();
        if (_id_nganh != "" && _id_nganh_nhanvien != "" && _id_nganh_nhanvien != _id_nganh)
            return "Nhân viên này chưa được cấu hình phù hợp với dịch vụ đã chọn.";

        return "";
    }

    private static string kiemtra_cong_suat_chinhanh(dbDataContext db, datlich_input _input, string _id_chinhanh, List<bspa_datlich_table> _list_trung_slot)
    {
        if (db == null || _input == null || la_trangthai_giu_slot(_input.trangthai) == false)
            return "";

        // Level 1: bỏ qua kiểm tra nhân viên để vẫn nhận lịch cơ bản.
        if (is_level2_chinhanh(db, _id_chinhanh) == false)
            return "";

        List<string> _list_nhanvien_du_dieu_kien = lay_ds_nhanvien_du_dieu_kien(db, _id_chinhanh, _input.id_nganh_dichvu);
        if (_list_nhanvien_du_dieu_kien.Count == 0)
            return "Chi nhánh hiện chưa có nhân viên khả dụng cho dịch vụ này.";

        if (_list_trung_slot == null || _list_trung_slot.Count == 0)
            return "";

        HashSet<string> _set_nhanvien_du_dieu_kien = new HashSet<string>(_list_nhanvien_du_dieu_kien, StringComparer.OrdinalIgnoreCase);
        List<string> _list_id_dichvu = _list_trung_slot.Select(p => (p.dichvu ?? "").Trim()).Where(p => p != "").Distinct().ToList();
        Dictionary<string, string> _map_nganh_dichvu = new Dictionary<string, string>();
        if (_list_id_dichvu.Count != 0)
        {
            var q_dichvu = db.web_post_tables
                .Where(p => p.id_chinhanh == _id_chinhanh && _list_id_dichvu.Contains(p.id.ToString()))
                .Select(p => new { id = p.id.ToString(), id_nganh = p.id_nganh })
                .ToList();

            foreach (var _dv in q_dichvu)
            {
                if (_map_nganh_dichvu.ContainsKey(_dv.id) == false)
                    _map_nganh_dichvu.Add(_dv.id, (_dv.id_nganh ?? "").Trim());
            }
        }

        int _so_lich_dang_giu = 0;
        foreach (bspa_datlich_table _lich in _list_trung_slot)
        {
            string _nhanvien = (_lich.nhanvien_thuchien ?? "").Trim();
            if (_nhanvien != "")
            {
                if (_set_nhanvien_du_dieu_kien.Contains(_nhanvien))
                    _so_lich_dang_giu++;
                continue;
            }

            if (string.IsNullOrWhiteSpace(_input.id_nganh_dichvu))
            {
                _so_lich_dang_giu++;
                continue;
            }

            string _id_dichvu = (_lich.dichvu ?? "").Trim();
            string _id_nganh_lich = _map_nganh_dichvu.ContainsKey(_id_dichvu) ? _map_nganh_dichvu[_id_dichvu] : "";
            if (_id_nganh_lich == "" || _id_nganh_lich == _input.id_nganh_dichvu)
                _so_lich_dang_giu++;
        }

        if (_so_lich_dang_giu >= _list_nhanvien_du_dieu_kien.Count)
            return "Khung giờ này không còn nhân viên trống trong thời lượng dịch vụ đã chọn.";

        return "";
    }

    public static void gan_du_lieu_vao_lich(dbDataContext db, bspa_datlich_table _ob, datlich_input _input, string _nguoitao, string _id_chinhanh, bool _giu_thongtin_tao)
    {
        if (db == null || _ob == null || _input == null)
            return;

        if (_giu_thongtin_tao == false || string.IsNullOrWhiteSpace(_ob.nguoitao))
            _ob.nguoitao = _nguoitao;

        _ob.tenkhachhang = _input.tenkhachhang;
        _ob.sdt = _input.sdt;
        _ob.ngaydat = _input.ngaydat;
        dongbo_thongtin_slot_vao_input(db, _input, _id_chinhanh, _giu_thongtin_tao ? (long?)_ob.id : null);
        _ob.thoiluong_dichvu_phut = _input.thoiluong_dichvu_phut;
        _ob.ngayketthucdukien = _input.ngayketthucdukien;

        if (_giu_thongtin_tao == false || _ob.ngaytao.HasValue == false)
            _ob.ngaytao = DateTime.Now;

        _ob.dichvu = _input.dichvu;
        _ob.tendichvu_taithoidiemnay = return_ten_dichvu(db, _input.dichvu, _id_chinhanh);
        _ob.nhanvien_thuchien = _input.nhanvien_thuchien;
        _ob.ghichu = _input.ghichu;
        _ob.trangthai = _input.trangthai;
        _ob.nguongoc = _input.nguongoc;
        _ob.id_chinhanh = _id_chinhanh;

        capnhat_ten_khachhang_hienco(db, _input.sdt, _id_chinhanh, _input.tenkhachhang);
    }

    public static void capnhat_ten_khachhang_hienco(dbDataContext db, string _sdt, string _id_chinhanh, string _tenkhachhang)
    {
        if (db == null || string.IsNullOrWhiteSpace(_sdt) || string.IsNullOrWhiteSpace(_id_chinhanh) || string.IsNullOrWhiteSpace(_tenkhachhang))
            return;

        var q = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt && p.id_chinhanh == _id_chinhanh);
        if (q.Count() != 0)
        {
            bspa_data_khachhang_table _kh = q.OrderByDescending(p => p.ngaytao).First();
            _kh.tenkhachhang = _tenkhachhang;
        }
    }
}
