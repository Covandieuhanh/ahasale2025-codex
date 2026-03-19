using System;
using System.Collections.Generic;
using System.Linq;

public class datlich_vanhanh_thedv_item
{
    public string id { get; set; }
    public string tenthe { get; set; }
    public string tendichvu { get; set; }
    public int tongsoluong { get; set; }
    public int sl_dalam { get; set; }
    public int sl_conlai { get; set; }
    public DateTime? hsd { get; set; }
    public long congno { get; set; }
}

public class datlich_vanhanh_hoadon_item
{
    public string id { get; set; }
    public DateTime? ngaytao { get; set; }
    public long tongtien { get; set; }
    public long sotien_conlai { get; set; }
}

public class datlich_vanhanh_tongquan
{
    public datlich_vanhanh_tongquan()
    {
        list_thedv = new List<datlich_vanhanh_thedv_item>();
        list_hoadon_congno = new List<datlich_vanhanh_hoadon_item>();
    }

    public bool co_hoso_khachhang { get; set; }
    public string id_khachhang { get; set; }
    public string tenkhachhang { get; set; }
    public string sdt { get; set; }
    public string nhomkhachhang { get; set; }
    public string ten_nhomkhachhang { get; set; }
    public string nguoichamsoc { get; set; }
    public string ten_nguoichamsoc { get; set; }
    public string diachi { get; set; }
    public string id_nganh_goiy { get; set; }
    public int so_thedv_hoatdong { get; set; }
    public int tong_buoi_conlai { get; set; }
    public int so_thedv_phuhop_dichvu { get; set; }
    public int tong_buoi_conlai_phuhop { get; set; }
    public int so_hoadon_congno { get; set; }
    public long tong_congno { get; set; }
    public List<datlich_vanhanh_thedv_item> list_thedv { get; set; }
    public List<datlich_vanhanh_hoadon_item> list_hoadon_congno { get; set; }
}

public class datlich_vanhanh_class
{
    public static datlich_vanhanh_tongquan tai_tongquan(dbDataContext db, string _sdt, string _id_chinhanh, string _id_dichvu)
    {
        datlich_vanhanh_tongquan _result = new datlich_vanhanh_tongquan();
        if (db == null || string.IsNullOrWhiteSpace(_id_chinhanh))
            return _result;

        string _sdt_chuan = datlich_class.chuanhoa_sdt(_sdt);
        _result.sdt = _sdt_chuan;
        if (_sdt_chuan == "")
            return _result;

        var q_khach = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt_chuan && p.id_chinhanh == _id_chinhanh).OrderByDescending(p => p.ngaytao);
        if (q_khach.Count() != 0)
        {
            bspa_data_khachhang_table _kh = q_khach.First();
            _result.co_hoso_khachhang = true;
            _result.id_khachhang = _kh.id.ToString();
            _result.tenkhachhang = _kh.tenkhachhang;
            _result.nhomkhachhang = _kh.nhomkhachhang;
            _result.ten_nhomkhachhang = return_ten_nhomkhachhang(db, _kh.nhomkhachhang, _id_chinhanh);
            _result.nguoichamsoc = _kh.nguoichamsoc;
            _result.ten_nguoichamsoc = datlich_class.return_ten_nguoitao_hienthi(_kh.nguoichamsoc);
            _result.diachi = _kh.diachi;
        }

        if (_result.tenkhachhang == "")
        {
            var q_lich = db.bspa_datlich_tables.Where(p => p.sdt == _sdt_chuan && p.id_chinhanh == _id_chinhanh).OrderByDescending(p => p.ngaytao);
            if (q_lich.Count() != 0)
                _result.tenkhachhang = q_lich.First().tenkhachhang;
        }

        _result.id_nganh_goiy = return_id_nganh_dichvu(db, _id_dichvu, _id_chinhanh);

        DateTime _today = DateTime.Now.Date;
        var q_thedv = db.thedichvu_tables.Where(p =>
            p.sdt == _sdt_chuan &&
            p.id_chinhanh == _id_chinhanh &&
            (p.sl_conlai ?? 0) > 0 &&
            (p.hsd.HasValue == false || p.hsd.Value.Date >= _today)
        );

        _result.so_thedv_hoatdong = q_thedv.Count();
        _result.tong_buoi_conlai = q_thedv.Any() ? q_thedv.Sum(p => p.sl_conlai ?? 0) : 0;
        _result.list_thedv = q_thedv
            .OrderBy(p => p.hsd)
            .ThenByDescending(p => p.ngaytao)
            .Take(5)
            .Select(p => new datlich_vanhanh_thedv_item
            {
                id = p.id.ToString(),
                tenthe = p.tenthe,
                tendichvu = p.ten_taithoidiemnay,
                tongsoluong = p.tongsoluong ?? 0,
                sl_dalam = p.sl_dalam ?? 0,
                sl_conlai = p.sl_conlai ?? 0,
                hsd = p.hsd,
                congno = p.sotien_conlai ?? 0,
            })
            .ToList();

        if (!string.IsNullOrWhiteSpace(_id_dichvu))
        {
            var q_thedv_phuhop = q_thedv.Where(p => p.iddv == _id_dichvu);
            _result.so_thedv_phuhop_dichvu = q_thedv_phuhop.Count();
            _result.tong_buoi_conlai_phuhop = q_thedv_phuhop.Any() ? q_thedv_phuhop.Sum(p => p.sl_conlai ?? 0) : 0;
        }

        var q_hoadon_congno = db.bspa_hoadon_tables.Where(p =>
            p.sdt == _sdt_chuan &&
            p.id_chinhanh == _id_chinhanh &&
            (p.sotien_conlai ?? 0) > 0
        );

        _result.so_hoadon_congno = q_hoadon_congno.Count();
        _result.tong_congno = q_hoadon_congno.Any() ? q_hoadon_congno.Sum(p => p.sotien_conlai ?? 0) : 0;
        _result.list_hoadon_congno = q_hoadon_congno
            .OrderByDescending(p => p.ngaytao)
            .Take(5)
            .Select(p => new datlich_vanhanh_hoadon_item
            {
                id = p.id.ToString(),
                ngaytao = p.ngaytao,
                tongtien = p.tongsauchietkhau ?? p.tongtien ?? 0,
                sotien_conlai = p.sotien_conlai ?? 0,
            })
            .ToList();

        return _result;
    }

    public static string return_ten_nhomkhachhang(dbDataContext db, string _id_nhom, string _id_chinhanh)
    {
        if (db == null || string.IsNullOrWhiteSpace(_id_nhom) || string.IsNullOrWhiteSpace(_id_chinhanh))
            return "";

        var q = db.nhomkhachhang_tables.Where(p => p.id.ToString() == _id_nhom && p.id_chinhanh == _id_chinhanh);
        if (q.Count() != 0)
            return q.First().tennhom;
        return "";
    }

    public static string return_id_nganh_dichvu(dbDataContext db, string _id_dichvu, string _id_chinhanh)
    {
        if (db == null || string.IsNullOrWhiteSpace(_id_dichvu) || string.IsNullOrWhiteSpace(_id_chinhanh))
            return "";

        var q = db.web_post_tables.Where(p => p.id.ToString() == _id_dichvu && p.id_chinhanh == _id_chinhanh && p.bin == false);
        if (q.Count() != 0)
            return q.First().id_nganh;
        return "";
    }
}
