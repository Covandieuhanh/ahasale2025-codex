using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class datlich_lienket_hoadon_item
{
    public string id { get; set; }
    public DateTime? ngaytao { get; set; }
    public string tenkhachhang { get; set; }
    public long tongtien { get; set; }
    public long sotien_conlai { get; set; }
}

public class datlich_lienket_thedv_item
{
    public string id { get; set; }
    public DateTime? ngaytao { get; set; }
    public string tenthe { get; set; }
    public string tendichvu { get; set; }
    public int sl_conlai { get; set; }
    public DateTime? hsd { get; set; }
}

public static class datlich_lienket_class
{
    private static readonly Regex regex_datlich = new Regex(@"lịch hẹn\s*#\s*(\d+)", RegexOptions.IgnoreCase);
    private static readonly Regex regex_hoadon = new Regex(@"HĐ\s*#\s*(\d+)", RegexOptions.IgnoreCase);
    private static readonly Regex regex_thedv = new Regex(@"Thẻ\s*DV\s*#\s*(\d+)", RegexOptions.IgnoreCase);

    public static string lay_id_datlich_tu_ghichu(string _ghichu)
    {
        if (string.IsNullOrWhiteSpace(_ghichu))
            return "";

        Match _match = regex_datlich.Match(_ghichu);
        if (_match.Success == false)
            return "";

        return (_match.Groups[1].Value ?? "").Trim();
    }

    public static string them_dong_ghi_chu(string _ghichu, string _dong)
    {
        string _ghichu_hientai = (_ghichu ?? "").Trim();
        string _dong_moi = (_dong ?? "").Trim();
        if (_dong_moi == "")
            return _ghichu_hientai;

        List<string> _lines = _ghichu_hientai
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .Where(p => p != "")
            .ToList();

        if (_lines.Any(p => string.Equals(p, _dong_moi, StringComparison.CurrentCultureIgnoreCase)))
            return _ghichu_hientai;

        _lines.Add(_dong_moi);
        return string.Join(Environment.NewLine, _lines.Distinct(StringComparer.CurrentCultureIgnoreCase));
    }

    public static string dam_bao_ghi_chu_datlich(string _ghichu, string _id_datlich, string _hanhdong, string _ten_dichvu)
    {
        if (string.IsNullOrWhiteSpace(_id_datlich))
            return (_ghichu ?? "").Trim();

        string _dong = (_hanhdong ?? "").Trim();
        if (_dong == "")
            _dong = "Tạo từ";

        _dong = _dong + " lịch hẹn #" + _id_datlich;
        string _ten_dichvu_hienthi = (_ten_dichvu ?? "").Trim();
        if (_ten_dichvu_hienthi != "")
            _dong += " - " + _ten_dichvu_hienthi;

        return them_dong_ghi_chu(_ghichu, _dong);
    }

    public static bool check_ghichu_thuoc_datlich(string _ghichu, string _id_datlich)
    {
        if (string.IsNullOrWhiteSpace(_id_datlich))
            return false;

        string _needle = "lịch hẹn #" + _id_datlich.Trim();
        return ((_ghichu ?? "").IndexOf(_needle, StringComparison.CurrentCultureIgnoreCase) >= 0);
    }

    public static List<datlich_lienket_hoadon_item> lay_ds_hoadon_lienket(dbDataContext db, string _id_datlich, string _id_chinhanh)
    {
        if (db == null || string.IsNullOrWhiteSpace(_id_datlich) || string.IsNullOrWhiteSpace(_id_chinhanh))
            return new List<datlich_lienket_hoadon_item>();

        string _needle = "lịch hẹn #" + _id_datlich.Trim();
        List<string> _list_id = db.bspa_hoadon_tables
            .Where(p => p.id_chinhanh == _id_chinhanh && p.ghichu != null && p.ghichu.Contains(_needle))
            .Select(p => p.id.ToString())
            .ToList();

        var q_lich = db.bspa_datlich_tables.Where(p => p.id.ToString() == _id_datlich && p.id_chinhanh == _id_chinhanh);
        if (q_lich.Count() != 0)
            _list_id.AddRange(lay_tatca_id(q_lich.First().ghichu, regex_hoadon));

        _list_id = _list_id.Where(p => string.IsNullOrWhiteSpace(p) == false).Distinct().ToList();
        if (_list_id.Count == 0)
            return new List<datlich_lienket_hoadon_item>();

        return db.bspa_hoadon_tables
            .Where(p => p.id_chinhanh == _id_chinhanh && _list_id.Contains(p.id.ToString()))
            .OrderByDescending(p => p.ngaytao)
            .Take(5)
            .Select(p => new datlich_lienket_hoadon_item
            {
                id = p.id.ToString(),
                ngaytao = p.ngaytao,
                tenkhachhang = p.tenkhachhang,
                tongtien = p.tongsauchietkhau ?? p.tongtien ?? 0,
                sotien_conlai = p.sotien_conlai ?? 0,
            })
            .ToList();
    }

    public static List<string> lay_ds_id_hoadon_tu_ghichu(string _ghichu)
    {
        return lay_tatca_id(_ghichu, regex_hoadon)
            .Where(p => string.IsNullOrWhiteSpace(p) == false)
            .Distinct()
            .ToList();
    }

    public static List<datlich_lienket_thedv_item> lay_ds_thedv_lienket(dbDataContext db, string _id_datlich, string _id_chinhanh)
    {
        if (db == null || string.IsNullOrWhiteSpace(_id_datlich) || string.IsNullOrWhiteSpace(_id_chinhanh))
            return new List<datlich_lienket_thedv_item>();

        string _needle = "lịch hẹn #" + _id_datlich.Trim();
        List<string> _list_id = db.thedichvu_tables
            .Where(p => p.id_chinhanh == _id_chinhanh && p.ghichu != null && p.ghichu.Contains(_needle))
            .Select(p => p.id.ToString())
            .ToList();

        var q_lich = db.bspa_datlich_tables.Where(p => p.id.ToString() == _id_datlich && p.id_chinhanh == _id_chinhanh);
        if (q_lich.Count() != 0)
            _list_id.AddRange(lay_tatca_id(q_lich.First().ghichu, regex_thedv));

        _list_id = _list_id.Where(p => string.IsNullOrWhiteSpace(p) == false).Distinct().ToList();
        if (_list_id.Count == 0)
            return new List<datlich_lienket_thedv_item>();

        return db.thedichvu_tables
            .Where(p => p.id_chinhanh == _id_chinhanh && _list_id.Contains(p.id.ToString()))
            .OrderByDescending(p => p.ngaytao)
            .Take(5)
            .Select(p => new datlich_lienket_thedv_item
            {
                id = p.id.ToString(),
                ngaytao = p.ngaytao,
                tenthe = p.tenthe,
                tendichvu = p.ten_taithoidiemnay,
                sl_conlai = p.sl_conlai ?? 0,
                hsd = p.hsd,
            })
            .ToList();
    }

    public static List<string> lay_ds_id_thedv_tu_ghichu(string _ghichu)
    {
        return lay_tatca_id(_ghichu, regex_thedv)
            .Where(p => string.IsNullOrWhiteSpace(p) == false)
            .Distinct()
            .ToList();
    }

    public static void dong_bo_vao_lich_hen(
        dbDataContext db,
        string _id_datlich,
        string _id_chinhanh,
        string _id_hoadon,
        string _id_thedv,
        string _nhanvien_thuchien,
        DateTime? _thoigian_thuchien,
        string _ghi_chu_bo_sung,
        bool _co_the_danh_dau_da_den)
    {
        if (db == null || string.IsNullOrWhiteSpace(_id_datlich) || string.IsNullOrWhiteSpace(_id_chinhanh))
            return;

        var q_lich = db.bspa_datlich_tables.Where(p => p.id.ToString() == _id_datlich && p.id_chinhanh == _id_chinhanh);
        if (q_lich.Count() == 0)
            return;

        bspa_datlich_table _lich = q_lich.First();
        if (!string.IsNullOrWhiteSpace(_nhanvien_thuchien))
            _lich.nhanvien_thuchien = _nhanvien_thuchien;

        if (_lich.trangthai == datlich_class.trangthai_chua_xacnhan)
            _lich.trangthai = datlich_class.trangthai_da_xacnhan;

        if (_co_the_danh_dau_da_den && _thoigian_thuchien.HasValue && _lich.ngaydat.HasValue)
        {
            if (_thoigian_thuchien.Value <= DateTime.Now.AddMinutes(5) && _lich.ngaydat.Value.Date <= _thoigian_thuchien.Value.Date)
                _lich.trangthai = datlich_class.trangthai_da_den;
        }

        string _dong_lienket = "";
        if (!string.IsNullOrWhiteSpace(_id_hoadon))
            _dong_lienket = "Đã liên kết HĐ #" + _id_hoadon.Trim();
        if (!string.IsNullOrWhiteSpace(_id_thedv))
            _dong_lienket += (_dong_lienket != "" ? " / " : "") + "Thẻ DV #" + _id_thedv.Trim();
        if (_dong_lienket != "")
            _lich.ghichu = them_dong_ghi_chu(_lich.ghichu, _dong_lienket);

        if (!string.IsNullOrWhiteSpace(_ghi_chu_bo_sung))
            _lich.ghichu = them_dong_ghi_chu(_lich.ghichu, _ghi_chu_bo_sung);

        string _noi_dung_nhatky = "";
        if (_dong_lienket != "" && !string.IsNullOrWhiteSpace(_ghi_chu_bo_sung))
            _noi_dung_nhatky = _dong_lienket + "; " + _ghi_chu_bo_sung.Trim();
        else if (_dong_lienket != "")
            _noi_dung_nhatky = _dong_lienket;
        else if (!string.IsNullOrWhiteSpace(_ghi_chu_bo_sung))
            _noi_dung_nhatky = _ghi_chu_bo_sung.Trim();

        if (_noi_dung_nhatky != "" && !string.IsNullOrWhiteSpace(_lich.sdt))
        {
            khachhang_nhatky_class.ghi_su_kien(
                db,
                _lich.sdt,
                _id_chinhanh,
                !string.IsNullOrWhiteSpace(_nhanvien_thuchien) ? _nhanvien_thuchien : _lich.nguoitao,
                khachhang_nhatky_class.tao_noidung_lienket_datlich(_lich, _noi_dung_nhatky, _id_chinhanh),
                _thoigian_thuchien ?? DateTime.Now
            );
        }
    }

    private static List<string> lay_tatca_id(string _ghichu, Regex _regex)
    {
        List<string> _result = new List<string>();
        if (string.IsNullOrWhiteSpace(_ghichu) || _regex == null)
            return _result;

        MatchCollection _matches = _regex.Matches(_ghichu);
        foreach (Match _match in _matches)
        {
            if (_match.Success)
            {
                string _id = (_match.Groups[1].Value ?? "").Trim();
                if (_id != "")
                    _result.Add(_id);
            }
        }
        return _result;
    }
}
