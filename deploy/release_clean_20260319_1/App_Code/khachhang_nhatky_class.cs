using System;
using System.Collections.Generic;
using System.Linq;

public static class khachhang_nhatky_class
{
    public static void ghi_su_kien(dbDataContext db, string _sdt, string _id_chinhanh, string _nguoitao, string _noidung, DateTime? _ngaytao)
    {
        if (db == null || string.IsNullOrWhiteSpace(_id_chinhanh))
            return;

        string _sdt_chuan = datlich_class.chuanhoa_sdt(_sdt);
        string _text = chuan_hoa_noidung(_noidung);
        if (_sdt_chuan == "" || _text == "")
            return;

        DateTime _thoigian = _ngaytao ?? DateTime.Now;
        string _user = (_nguoitao ?? "").Trim();
        if (_user == "")
            _user = lay_user_hien_tai();
        if (_user == "")
            _user = datlich_class.nguoitao_khach_online;

        var q_gannhat = db.ghichu_khachhang_tables
            .Where(p => p.sdt == _sdt_chuan && p.id_chinhanh == _id_chinhanh)
            .OrderByDescending(p => p.ngaytao);

        if (q_gannhat.Any())
        {
            ghichu_khachhang_table _last = q_gannhat.First();
            if (string.Equals((_last.ghichu ?? "").Trim(), _text, StringComparison.CurrentCultureIgnoreCase) &&
                _last.ngaytao.HasValue &&
                Math.Abs((_thoigian - _last.ngaytao.Value).TotalMinutes) <= 3)
                return;
        }

        ghichu_khachhang_table _ob = new ghichu_khachhang_table();
        _ob.ghichu = _text;
        _ob.sdt = _sdt_chuan;
        _ob.nguoitao = _user;
        _ob.ngaytao = _thoigian;
        _ob.id_chinhanh = _id_chinhanh;
        db.ghichu_khachhang_tables.InsertOnSubmit(_ob);
    }

    public static string tao_noidung_tao_lich(string _id_datlich, datlich_input _input, string _ten_dichvu)
    {
        if (_input == null || string.IsNullOrWhiteSpace(_id_datlich))
            return "";

        List<string> _parts = new List<string>();
        _parts.Add("Tạo lịch hẹn #" + _id_datlich);

        string _ten = (_ten_dichvu ?? "").Trim();
        if (_ten != "")
            _parts.Add(_ten);

        _parts.Add(_input.ngaydat.ToString("dd/MM/yyyy HH:mm"));
        _parts.Add("Trạng thái " + ((_input.trangthai ?? "").Trim() != "" ? _input.trangthai.Trim() : datlich_class.trangthai_chua_xacnhan));

        if (string.IsNullOrWhiteSpace(_input.nguongoc) == false)
            _parts.Add("Nguồn " + _input.nguongoc.Trim());

        return string.Join(" - ", _parts);
    }

    public static string tao_noidung_capnhat_lich(dbDataContext db, bspa_datlich_table _cu, datlich_input _moi, string _id_datlich, string _id_chinhanh)
    {
        if (db == null || _cu == null || _moi == null || string.IsNullOrWhiteSpace(_id_datlich))
            return "";

        List<string> _changes = new List<string>();

        if ((_cu.tenkhachhang ?? "") != (_moi.tenkhachhang ?? ""))
            _changes.Add("khách hàng " + ((_cu.tenkhachhang ?? "").Trim() != "" ? _cu.tenkhachhang.Trim() : "Chưa có tên") + " -> " + ((_moi.tenkhachhang ?? "").Trim() != "" ? _moi.tenkhachhang.Trim() : "Chưa có tên"));

        string _sdt_cu = datlich_class.chuanhoa_sdt(_cu.sdt);
        string _sdt_moi = datlich_class.chuanhoa_sdt(_moi.sdt);
        if (_sdt_cu != _sdt_moi)
            _changes.Add("SĐT " + (_sdt_cu != "" ? _sdt_cu : "Chưa có") + " -> " + (_sdt_moi != "" ? _sdt_moi : "Chưa có"));

        if ((_cu.trangthai ?? "") != (_moi.trangthai ?? ""))
            _changes.Add("trạng thái " + (_cu.trangthai ?? "Trống") + " -> " + (_moi.trangthai ?? "Trống"));

        DateTime _ngaydat_cu = _cu.ngaydat ?? DateTime.MinValue;
        if (_ngaydat_cu != _moi.ngaydat)
            _changes.Add("giờ hẹn " + _ngaydat_cu.ToString("dd/MM/yyyy HH:mm") + " -> " + _moi.ngaydat.ToString("dd/MM/yyyy HH:mm"));

        string _ten_dv_cu = return_ten_dichvu(db, _cu.dichvu, _cu.tendichvu_taithoidiemnay, _id_chinhanh);
        string _ten_dv_moi = return_ten_dichvu(db, _moi.dichvu, "", _id_chinhanh);
        if ((_cu.dichvu ?? "") != (_moi.dichvu ?? ""))
            _changes.Add("dịch vụ " + (_ten_dv_cu != "" ? _ten_dv_cu : "Chưa chọn") + " -> " + (_ten_dv_moi != "" ? _ten_dv_moi : "Chưa chọn"));

        string _nv_cu = datlich_class.return_ten_nguoitao_hienthi(_cu.nhanvien_thuchien);
        string _nv_moi = datlich_class.return_ten_nguoitao_hienthi(_moi.nhanvien_thuchien);
        if ((_cu.nhanvien_thuchien ?? "") != (_moi.nhanvien_thuchien ?? ""))
            _changes.Add("nhân viên " + (_nv_cu != "" ? _nv_cu : "Chưa gán") + " -> " + (_nv_moi != "" ? _nv_moi : "Chưa gán"));

        if ((_cu.nguongoc ?? "") != (_moi.nguongoc ?? ""))
            _changes.Add("nguồn " + ((_cu.nguongoc ?? "").Trim() != "" ? _cu.nguongoc.Trim() : "Trống") + " -> " + ((_moi.nguongoc ?? "").Trim() != "" ? _moi.nguongoc.Trim() : "Trống"));

        if (_changes.Count == 0)
            return "";

        return "Cập nhật lịch hẹn #" + _id_datlich + ": " + string.Join("; ", _changes);
    }

    public static string tao_noidung_lienket_datlich(bspa_datlich_table _lich, string _ghi_chu_bo_sung, string _id_chinhanh)
    {
        if (_lich == null || string.IsNullOrWhiteSpace(_ghi_chu_bo_sung))
            return "";

        string _ten_dichvu = return_ten_dichvu(null, _lich.dichvu, _lich.tendichvu_taithoidiemnay, _id_chinhanh);
        string _prefix = "Lịch hẹn #" + _lich.id;
        if (_ten_dichvu != "")
            _prefix += " - " + _ten_dichvu;

        return _prefix + ": " + chuan_hoa_noidung(_ghi_chu_bo_sung);
    }

    private static string return_ten_dichvu(dbDataContext db, string _id_dichvu, string _ten_taithoidiem, string _id_chinhanh)
    {
        string _ten = (_ten_taithoidiem ?? "").Trim();
        if (_ten != "")
            return _ten;

        if (db == null)
            db = new dbDataContext();

        return datlich_class.return_ten_dichvu(db, _id_dichvu, _id_chinhanh);
    }

    private static string lay_user_hien_tai()
    {
        if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Session == null)
            return "";

        if (System.Web.HttpContext.Current.Session["user"] != null && System.Web.HttpContext.Current.Session["user"].ToString() != "")
            return System.Web.HttpContext.Current.Session["user"].ToString();

        if (System.Web.HttpContext.Current.Session["user_home"] != null && System.Web.HttpContext.Current.Session["user_home"].ToString() != "")
            return System.Web.HttpContext.Current.Session["user_home"].ToString();

        return "";
    }

    private static string chuan_hoa_noidung(string _value)
    {
        string _text = (_value ?? "").Replace("\r", " ").Replace("\n", " ").Trim();
        while (_text.Contains("  "))
            _text = _text.Replace("  ", " ");

        if (_text == "")
            return "";

        if (_text.EndsWith(".") == false)
            _text += ".";

        return _text;
    }
}
