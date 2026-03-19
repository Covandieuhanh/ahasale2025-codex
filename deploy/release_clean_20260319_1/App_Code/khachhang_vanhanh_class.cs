using System;
using System.Collections.Generic;
using System.Linq;

public class khachhang_vanhanh_timeline_item
{
    public string loai { get; set; }
    public string loai_hienthi { get; set; }
    public string loai_css { get; set; }
    public string tieude { get; set; }
    public string mota { get; set; }
    public string trangthai { get; set; }
    public string trangthai_css { get; set; }
    public string icon_css { get; set; }
    public DateTime? thoigian { get; set; }
    public string url_chitiet { get; set; }
    public string label_url { get; set; }
}

public class khachhang_vanhanh_tongquan
{
    public khachhang_vanhanh_tongquan()
    {
        list_timeline = new List<khachhang_vanhanh_timeline_item>();
    }

    public string sdt { get; set; }
    public int tong_lich { get; set; }
    public int so_lich_chua_xacnhan { get; set; }
    public int so_lich_sap_toi { get; set; }
    public string id_lich_gannhat { get; set; }
    public string ten_dichvu_lich_gannhat { get; set; }
    public string trangthai_lich_gannhat { get; set; }
    public DateTime? ngay_lich_gannhat { get; set; }
    public int tong_hoadon { get; set; }
    public int so_hoadon_congno { get; set; }
    public long tong_congno { get; set; }
    public string id_hoadon_gannhat { get; set; }
    public DateTime? ngay_hoadon_gannhat { get; set; }
    public int so_thedv_hoatdong { get; set; }
    public int tong_buoi_conlai { get; set; }
    public string hanh_dong_goi_y { get; set; }
    public string hanh_dong_css { get; set; }
    public string url_hanh_dong_goi_y { get; set; }
    public List<khachhang_vanhanh_timeline_item> list_timeline { get; set; }
}

public static class khachhang_vanhanh_class
{
    public static khachhang_vanhanh_tongquan tai_tongquan(dbDataContext db, string _sdt, string _id_chinhanh)
    {
        khachhang_vanhanh_tongquan _result = new khachhang_vanhanh_tongquan();
        if (db == null || string.IsNullOrWhiteSpace(_id_chinhanh))
            return _result;

        string _sdt_chuan = datlich_class.chuanhoa_sdt(_sdt);
        _result.sdt = _sdt_chuan;
        if (_sdt_chuan == "")
            return _result;

        DateTime _now = DateTime.Now;
        DateTime _today = _now.Date;

        List<bspa_datlich_table> _list_lich = db.bspa_datlich_tables
            .Where(p => p.id_chinhanh == _id_chinhanh && p.sdt == _sdt_chuan)
            .ToList();

        List<bspa_hoadon_table> _list_hoadon = db.bspa_hoadon_tables
            .Where(p => p.id_chinhanh == _id_chinhanh && p.sdt == _sdt_chuan)
            .ToList();

        List<thedichvu_table> _list_thedv = db.thedichvu_tables
            .Where(p => p.id_chinhanh == _id_chinhanh && p.sdt == _sdt_chuan)
            .ToList();

        List<ghichu_khachhang_table> _list_ghichu = db.ghichu_khachhang_tables
            .Where(p => p.id_chinhanh == _id_chinhanh && p.sdt == _sdt_chuan)
            .ToList();

        List<string> _list_id_hoadon = _list_hoadon.Select(p => p.id.ToString()).ToList();
        List<bspa_hoadon_chitiet_table> _list_tieu_buoi = new List<bspa_hoadon_chitiet_table>();
        if (_list_id_hoadon.Count != 0)
        {
            _list_tieu_buoi = db.bspa_hoadon_chitiet_tables
                .Where(p =>
                    p.id_chinhanh == _id_chinhanh &&
                    p.id_thedichvu != null &&
                    p.id_thedichvu != "" &&
                    _list_id_hoadon.Contains(p.id_hoadon))
                .ToList();
        }

        _result.tong_lich = _list_lich.Count;
        _result.tong_hoadon = _list_hoadon.Count;
        _result.so_hoadon_congno = _list_hoadon.Count(p => (p.sotien_conlai ?? 0) > 0);
        _result.tong_congno = _list_hoadon.Where(p => (p.sotien_conlai ?? 0) > 0).Sum(p => p.sotien_conlai ?? 0);

        List<thedichvu_table> _list_thedv_hoatdong = _list_thedv
            .Where(p => (p.sl_conlai ?? 0) > 0 && (p.hsd.HasValue == false || p.hsd.Value.Date >= _today))
            .ToList();
        _result.so_thedv_hoatdong = _list_thedv_hoatdong.Count;
        _result.tong_buoi_conlai = _list_thedv_hoatdong.Sum(p => p.sl_conlai ?? 0);

        List<bspa_datlich_table> _list_lich_saptoi = _list_lich
            .Where(p =>
                p.ngaydat.HasValue &&
                p.ngaydat.Value >= _now &&
                p.trangthai != datlich_class.trangthai_da_huy &&
                p.trangthai != datlich_class.trangthai_khong_den)
            .OrderBy(p => p.ngaydat)
            .ToList();

        _result.so_lich_sap_toi = _list_lich_saptoi.Count;
        _result.so_lich_chua_xacnhan = _list_lich_saptoi.Count(p => p.trangthai == datlich_class.trangthai_chua_xacnhan);

        bspa_datlich_table _lich_goi_y = _list_lich_saptoi.FirstOrDefault();
        if (_lich_goi_y == null)
            _lich_goi_y = _list_lich.OrderByDescending(p => p.ngaydat ?? p.ngaytao).FirstOrDefault();

        if (_lich_goi_y != null)
        {
            _result.id_lich_gannhat = _lich_goi_y.id.ToString();
            _result.ten_dichvu_lich_gannhat = return_ten_dichvu_lich(db, _lich_goi_y, _id_chinhanh);
            _result.trangthai_lich_gannhat = _lich_goi_y.trangthai;
            _result.ngay_lich_gannhat = _lich_goi_y.ngaydat;
        }

        bspa_hoadon_table _hoadon_gannhat = _list_hoadon.OrderByDescending(p => p.ngaytao).FirstOrDefault();
        if (_hoadon_gannhat != null)
        {
            _result.id_hoadon_gannhat = _hoadon_gannhat.id.ToString();
            _result.ngay_hoadon_gannhat = _hoadon_gannhat.ngaytao;
        }

        bspa_datlich_table _lich_can_xacnhan = _list_lich_saptoi
            .Where(p => p.trangthai == datlich_class.trangthai_chua_xacnhan)
            .OrderBy(p => p.ngaydat)
            .FirstOrDefault();
        bspa_datlich_table _lich_da_xacnhan = _list_lich_saptoi
            .Where(p => p.trangthai == datlich_class.trangthai_da_xacnhan)
            .OrderBy(p => p.ngaydat)
            .FirstOrDefault();
        thedichvu_table _thedv_goi_y = _list_thedv_hoatdong
            .OrderBy(p => p.hsd)
            .ThenByDescending(p => p.ngaytao)
            .FirstOrDefault();

        if (_lich_can_xacnhan != null)
        {
            _result.hanh_dong_goi_y = "Xác nhận lịch hẹn #" + _lich_can_xacnhan.id;
            _result.hanh_dong_css = "crm-kpi--cyan";
            _result.url_hanh_dong_goi_y = "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=" + _lich_can_xacnhan.id;
        }
        else if (_lich_da_xacnhan != null)
        {
            _result.hanh_dong_goi_y = "Theo dõi check-in lịch #" + _lich_da_xacnhan.id;
            _result.hanh_dong_css = "crm-kpi--green";
            _result.url_hanh_dong_goi_y = "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=" + _lich_da_xacnhan.id;
        }
        else if (_result.tong_congno > 0 && _hoadon_gannhat != null)
        {
            _result.hanh_dong_goi_y = "Thu nốt công nợ HĐ #" + _hoadon_gannhat.id;
            _result.hanh_dong_css = "crm-kpi--orange";
            _result.url_hanh_dong_goi_y = "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + _hoadon_gannhat.id;
        }
        else if (_thedv_goi_y != null)
        {
            _result.hanh_dong_goi_y = "Khách còn " + (_thedv_goi_y.sl_conlai ?? 0).ToString("#,##0") + " buổi ở thẻ #" + _thedv_goi_y.id;
            _result.hanh_dong_css = "crm-kpi--teal";
            _result.url_hanh_dong_goi_y = "/gianhang/admin/quan-ly-the-dich-vu/chi-tiet.aspx?id=" + _thedv_goi_y.id;
        }
        else if (_hoadon_gannhat != null)
        {
            _result.hanh_dong_goi_y = "Xem hóa đơn gần nhất #" + _hoadon_gannhat.id;
            _result.hanh_dong_css = "crm-kpi--slate";
            _result.url_hanh_dong_goi_y = "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + _hoadon_gannhat.id;
        }
        else if (_lich_goi_y != null)
        {
            _result.hanh_dong_goi_y = "Mở lịch hẹn gần nhất #" + _lich_goi_y.id;
            _result.hanh_dong_css = "crm-kpi--slate";
            _result.url_hanh_dong_goi_y = "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=" + _lich_goi_y.id;
        }
        else
        {
            _result.hanh_dong_goi_y = "Tạo lịch hẹn mới cho khách";
            _result.hanh_dong_css = "crm-kpi--slate";
            _result.url_hanh_dong_goi_y = "/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx?q=add";
        }

        List<khachhang_vanhanh_timeline_item> _timeline = new List<khachhang_vanhanh_timeline_item>();
        foreach (bspa_datlich_table _item in _list_lich.OrderByDescending(p => p.ngaydat ?? p.ngaytao).Take(6))
            _timeline.Add(tao_item_lichhen(db, _item, _id_chinhanh));

        foreach (bspa_hoadon_table _item in _list_hoadon.OrderByDescending(p => p.ngaytao).Take(6))
            _timeline.Add(tao_item_hoadon(_item));

        foreach (thedichvu_table _item in _list_thedv.OrderByDescending(p => p.ngaytao).Take(6))
            _timeline.Add(tao_item_thedv(_item));

        foreach (bspa_hoadon_chitiet_table _item in _list_tieu_buoi.OrderByDescending(p => p.ngaytao).Take(6))
            _timeline.Add(tao_item_tieu_buoi(_item));

        foreach (ghichu_khachhang_table _item in _list_ghichu.OrderByDescending(p => p.ngaytao).Take(4))
            _timeline.Add(tao_item_ghichu(_item));

        _result.list_timeline = _timeline
            .Where(p => p != null && p.thoigian.HasValue)
            .OrderByDescending(p => p.thoigian)
            .Take(14)
            .ToList();

        return _result;
    }

    private static khachhang_vanhanh_timeline_item tao_item_lichhen(dbDataContext db, bspa_datlich_table _item, string _id_chinhanh)
    {
        if (_item == null)
            return null;

        khachhang_vanhanh_timeline_item _result = new khachhang_vanhanh_timeline_item();
        _result.loai = "lichhen";
        _result.loai_hienthi = "Lịch hẹn";
        _result.loai_css = "crm-chip--cyan";
        _result.icon_css = "mif-calendar";
        _result.tieude = "Lịch hẹn #" + _item.id + return_hau_to_dichvu(return_ten_dichvu_lich(db, _item, _id_chinhanh));
        _result.mota = "Trạng thái: " + (_item.trangthai ?? "Chưa cập nhật") + return_hau_to_nhanvien(_item.nhanvien_thuchien);
        _result.trangthai = _item.trangthai;
        _result.trangthai_css = return_css_trangthai_lich(_item.trangthai);
        _result.thoigian = _item.ngaydat ?? _item.ngaytao;
        _result.url_chitiet = "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=" + _item.id;
        _result.label_url = "Mở lịch";
        return _result;
    }

    private static khachhang_vanhanh_timeline_item tao_item_hoadon(bspa_hoadon_table _item)
    {
        if (_item == null)
            return null;

        long _tong = _item.tongsauchietkhau ?? _item.tongtien ?? 0;
        long _congno = _item.sotien_conlai ?? 0;

        khachhang_vanhanh_timeline_item _result = new khachhang_vanhanh_timeline_item();
        _result.loai = "hoadon";
        _result.loai_hienthi = "Hóa đơn";
        _result.loai_css = "crm-chip--green";
        _result.icon_css = "mif-file-text";
        _result.tieude = "Hóa đơn #" + _item.id;
        _result.mota = "Sau CK " + _tong.ToString("#,##0") + (_congno > 0 ? " | Còn nợ " + _congno.ToString("#,##0") : " | Đã thanh toán");
        _result.trangthai = _congno > 0 ? "Còn nợ" : "Đã thanh toán";
        _result.trangthai_css = _congno > 0 ? "crm-chip--orange" : "crm-chip--teal";
        _result.thoigian = _item.ngaytao;
        _result.url_chitiet = "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + _item.id;
        _result.label_url = "Xem hóa đơn";
        return _result;
    }

    private static khachhang_vanhanh_timeline_item tao_item_thedv(thedichvu_table _item)
    {
        if (_item == null)
            return null;

        int _conlai = _item.sl_conlai ?? 0;
        string _trangthai = _conlai > 0 ? "Còn " + _conlai.ToString("#,##0") + " buổi" : "Hết buổi";
        if (_item.hsd.HasValue && _item.hsd.Value.Date < DateTime.Now.Date)
            _trangthai = "Đã hết hạn";

        khachhang_vanhanh_timeline_item _result = new khachhang_vanhanh_timeline_item();
        _result.loai = "thedv";
        _result.loai_hienthi = "Thẻ dịch vụ";
        _result.loai_css = "crm-chip--teal";
        _result.icon_css = "mif-credit-card";
        _result.tieude = "Thẻ DV #" + _item.id + return_hau_to_dichvu(_item.tenthe);
        _result.mota = (_item.ten_taithoidiemnay ?? "") + " | Còn " + _conlai.ToString("#,##0") + "/" + (_item.tongsoluong ?? 0).ToString("#,##0") + " buổi";
        _result.trangthai = _trangthai;
        _result.trangthai_css = return_css_trangthai_thedv(_trangthai);
        _result.thoigian = _item.ngaytao;
        _result.url_chitiet = "/gianhang/admin/quan-ly-the-dich-vu/chi-tiet.aspx?id=" + _item.id;
        _result.label_url = "Xem thẻ";
        return _result;
    }

    private static khachhang_vanhanh_timeline_item tao_item_tieu_buoi(bspa_hoadon_chitiet_table _item)
    {
        if (_item == null || string.IsNullOrWhiteSpace(_item.id_thedichvu))
            return null;

        khachhang_vanhanh_timeline_item _result = new khachhang_vanhanh_timeline_item();
        _result.loai = "tieuboi";
        _result.loai_hienthi = "Dùng thẻ";
        _result.loai_css = "crm-chip--violet";
        _result.icon_css = "mif-history";
        _result.tieude = "Dùng thẻ DV #" + _item.id_thedichvu + return_hau_to_dichvu(_item.ten_dvsp_taithoidiemnay);
        _result.mota = "HĐ #" + (_item.id_hoadon ?? "") + " | SL " + ((_item.soluong ?? 0).ToString("#,##0")) + return_hau_to_nhanvien(_item.nguoilam_dichvu);
        _result.trangthai = "Đã sử dụng";
        _result.trangthai_css = "crm-chip--violet";
        _result.thoigian = _item.ngaytao;
        _result.url_chitiet = "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + (_item.id_hoadon ?? "");
        _result.label_url = "Xem hóa đơn";
        return _result;
    }

    private static khachhang_vanhanh_timeline_item tao_item_ghichu(ghichu_khachhang_table _item)
    {
        if (_item == null)
            return null;

        khachhang_vanhanh_timeline_item _result = new khachhang_vanhanh_timeline_item();
        _result.loai = "ghichu";
        _result.loai_hienthi = "Ghi chú";
        _result.loai_css = "crm-chip--slate";
        _result.icon_css = "mif-pencil";
        _result.tieude = "Ghi chú nội bộ";
        _result.mota = rut_gon_noidung(_item.ghichu, 140);
        _result.trangthai = datlich_class.return_ten_nguoitao_hienthi(_item.nguoitao);
        _result.trangthai_css = "crm-chip--slate";
        _result.thoigian = _item.ngaytao;
        _result.url_chitiet = "";
        _result.label_url = "";
        return _result;
    }

    private static string return_ten_dichvu_lich(dbDataContext db, bspa_datlich_table _item, string _id_chinhanh)
    {
        if (_item == null)
            return "";

        string _ten = (_item.tendichvu_taithoidiemnay ?? "").Trim();
        if (_ten != "")
            return _ten;

        return datlich_class.return_ten_dichvu(db, _item.dichvu, _id_chinhanh);
    }

    private static string return_hau_to_dichvu(string _ten)
    {
        string _value = (_ten ?? "").Trim();
        if (_value == "")
            return "";
        return " - " + _value;
    }

    private static string return_hau_to_nhanvien(string _user)
    {
        string _ten = datlich_class.return_ten_nguoitao_hienthi(_user);
        if (_ten == "")
            return "";
        return " | NV: " + _ten;
    }

    private static string rut_gon_noidung(string _value, int _max)
    {
        string _text = (_value ?? "").Replace("\r", " ").Replace("\n", " ").Trim();
        while (_text.Contains("  "))
            _text = _text.Replace("  ", " ");

        if (_text.Length <= _max)
            return _text;

        return _text.Substring(0, _max).TrimEnd() + "...";
    }

    private static string return_css_trangthai_lich(string _trangthai)
    {
        switch ((_trangthai ?? "").Trim())
        {
            case datlich_class.trangthai_chua_xacnhan: return "crm-chip--cyan";
            case datlich_class.trangthai_da_xacnhan: return "crm-chip--green";
            case datlich_class.trangthai_khong_den: return "crm-chip--orange";
            case datlich_class.trangthai_da_den: return "crm-chip--magenta";
            case datlich_class.trangthai_da_huy: return "crm-chip--red";
            default: return "crm-chip--slate";
        }
    }

    private static string return_css_trangthai_thedv(string _trangthai)
    {
        if (string.IsNullOrWhiteSpace(_trangthai))
            return "crm-chip--slate";

        if (_trangthai.IndexOf("Còn", StringComparison.CurrentCultureIgnoreCase) >= 0)
            return "crm-chip--teal";
        if (_trangthai.IndexOf("Hết", StringComparison.CurrentCultureIgnoreCase) >= 0)
            return "crm-chip--orange";
        return "crm-chip--slate";
    }
}
