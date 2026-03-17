using System;
using System.Collections.Generic;
using System.Linq;

public class lichhen_dieuphoi_nhanvien_item
{
    public string username { get; set; }
    public string hoten { get; set; }
    public string ten_bophan { get; set; }
    public int tong_lich { get; set; }
    public int so_lich_cho_xacnhan { get; set; }
    public int so_lich_da_den { get; set; }
    public int so_lich_qua_gio { get; set; }
    public int so_lich_sap_toi { get; set; }
    public string muc_tai { get; set; }
    public string muc_tai_css { get; set; }
    public string id_lich_gan_nhat { get; set; }
    public DateTime? gio_hen_gan_nhat { get; set; }
}

public class lichhen_dieuphoi_bophan_item
{
    public string id_bophan { get; set; }
    public string ten_bophan { get; set; }
    public int tong_lich { get; set; }
    public int so_lich_cho_xacnhan { get; set; }
    public int so_lich_da_den { get; set; }
    public int so_lich_qua_gio { get; set; }
}

public class lichhen_dieuphoi_canhbao_item
{
    public int uu_tien { get; set; }
    public string loai { get; set; }
    public string loai_css { get; set; }
    public string tieu_de { get; set; }
    public string mo_ta { get; set; }
    public DateTime? ngaydat { get; set; }
    public string id_lich { get; set; }
    public string url { get; set; }
}

public class lichhen_dieuphoi_tongquan
{
    public lichhen_dieuphoi_tongquan()
    {
        list_nhanvien = new List<lichhen_dieuphoi_nhanvien_item>();
        list_bophan = new List<lichhen_dieuphoi_bophan_item>();
        list_canhbao = new List<lichhen_dieuphoi_canhbao_item>();
    }

    public DateTime ngay { get; set; }
    public int tong_lich { get; set; }
    public int so_lich_cho_xacnhan { get; set; }
    public int so_lich_da_xacnhan { get; set; }
    public int so_lich_da_den { get; set; }
    public int so_lich_khong_den { get; set; }
    public int so_lich_da_huy { get; set; }
    public int so_lich_chua_phan_cong { get; set; }
    public int so_lich_qua_gio_chua_xuly { get; set; }
    public int so_lich_sap_toi_2gio { get; set; }
    public int so_lich_da_ghi_nhan_giaodich { get; set; }
    public int so_lich_chua_co_hoso { get; set; }
    public List<lichhen_dieuphoi_nhanvien_item> list_nhanvien { get; set; }
    public List<lichhen_dieuphoi_bophan_item> list_bophan { get; set; }
    public List<lichhen_dieuphoi_canhbao_item> list_canhbao { get; set; }
}

public static class lichhen_dieuphoi_class
{
    public static lichhen_dieuphoi_tongquan tai_tongquan_homnay(dbDataContext db, string _id_chinhanh)
    {
        lichhen_dieuphoi_tongquan _result = new lichhen_dieuphoi_tongquan();
        if (db == null || string.IsNullOrWhiteSpace(_id_chinhanh))
            return _result;

        DateTime _now = DateTime.Now;
        DateTime _today = _now.Date;
        DateTime _tomorrow = _today.AddDays(1);
        DateTime _soon = _now.AddHours(2);
        _result.ngay = _today;

        List<bspa_datlich_table> _list_lich = db.bspa_datlich_tables
            .Where(p => p.id_chinhanh == _id_chinhanh && p.ngaydat.HasValue && p.ngaydat.Value >= _today && p.ngaydat.Value < _tomorrow)
            .OrderBy(p => p.ngaydat)
            .ToList();

        _result.tong_lich = _list_lich.Count;
        _result.so_lich_cho_xacnhan = _list_lich.Count(p => p.trangthai == datlich_class.trangthai_chua_xacnhan);
        _result.so_lich_da_xacnhan = _list_lich.Count(p => p.trangthai == datlich_class.trangthai_da_xacnhan);
        _result.so_lich_da_den = _list_lich.Count(p => p.trangthai == datlich_class.trangthai_da_den);
        _result.so_lich_khong_den = _list_lich.Count(p => p.trangthai == datlich_class.trangthai_khong_den);
        _result.so_lich_da_huy = _list_lich.Count(p => p.trangthai == datlich_class.trangthai_da_huy);

        List<bspa_datlich_table> _list_lich_hoatdong = _list_lich
            .Where(p => p.trangthai != datlich_class.trangthai_da_huy && p.trangthai != datlich_class.trangthai_khong_den)
            .ToList();

        _result.so_lich_chua_phan_cong = _list_lich_hoatdong.Count(p => string.IsNullOrWhiteSpace(p.nhanvien_thuchien));
        _result.so_lich_qua_gio_chua_xuly = _list_lich_hoatdong.Count(p =>
            p.ngaydat.HasValue &&
            p.ngaydat.Value <= _now &&
            p.trangthai != datlich_class.trangthai_da_den);
        _result.so_lich_sap_toi_2gio = _list_lich_hoatdong.Count(p =>
            p.ngaydat.HasValue &&
            p.ngaydat.Value > _now &&
            p.ngaydat.Value <= _soon);
        _result.so_lich_da_ghi_nhan_giaodich = _list_lich.Count(p =>
            datlich_lienket_class.lay_ds_id_hoadon_tu_ghichu(p.ghichu).Count != 0 ||
            datlich_lienket_class.lay_ds_id_thedv_tu_ghichu(p.ghichu).Count != 0);

        Dictionary<string, string> _map_hoso = new Dictionary<string, string>();
        List<string> _list_sdt = _list_lich
            .Select(p => datlich_class.chuanhoa_sdt(p.sdt))
            .Where(p => p != "")
            .Distinct()
            .ToList();
        if (_list_sdt.Count != 0)
        {
            foreach (bspa_data_khachhang_table _kh in db.bspa_data_khachhang_tables
                .Where(p => p.id_chinhanh == _id_chinhanh && _list_sdt.Contains(p.sdt))
                .OrderByDescending(p => p.ngaytao)
                .ToList())
            {
                if (_map_hoso.ContainsKey(_kh.sdt) == false)
                    _map_hoso.Add(_kh.sdt, _kh.id.ToString());
            }
        }
        _result.so_lich_chua_co_hoso = _list_lich.Count(p => _map_hoso.ContainsKey(datlich_class.chuanhoa_sdt(p.sdt)) == false);

        Dictionary<string, taikhoan_table_2023> _map_nhanvien = db.taikhoan_table_2023s
            .Where(p => p.id_chinhanh == _id_chinhanh)
            .ToList()
            .GroupBy(p => p.taikhoan ?? "")
            .Where(p => p.Key != "")
            .ToDictionary(p => p.Key, p => p.First());

        Dictionary<string, string> _map_bophan = db.phongban_tables
            .Where(p => p.id_chinhanh == _id_chinhanh)
            .ToList()
            .GroupBy(p => p.id.ToString())
            .ToDictionary(p => p.Key, p => (p.First().ten ?? "").Trim());

        _result.list_nhanvien = _list_lich_hoatdong
            .Where(p => string.IsNullOrWhiteSpace(p.nhanvien_thuchien) == false)
            .GroupBy(p => p.nhanvien_thuchien.Trim())
            .Select(g =>
            {
                taikhoan_table_2023 _nv = _map_nhanvien.ContainsKey(g.Key) ? _map_nhanvien[g.Key] : null;
                string _id_bophan = _nv != null ? (_nv.id_bophan ?? "").Trim() : "";
                List<bspa_datlich_table> _list_nv = g.OrderBy(p => p.ngaydat).ToList();
                bspa_datlich_table _lich_gan = _list_nv.FirstOrDefault(p => p.ngaydat.HasValue && p.ngaydat.Value >= _now) ?? _list_nv.LastOrDefault();
                int _qua_gio = _list_nv.Count(p => p.ngaydat.HasValue && p.ngaydat.Value <= _now && p.trangthai != datlich_class.trangthai_da_den);
                int _sap_toi = _list_nv.Count(p => p.ngaydat.HasValue && p.ngaydat.Value > _now && p.ngaydat.Value <= _soon);

                lichhen_dieuphoi_nhanvien_item _item = new lichhen_dieuphoi_nhanvien_item();
                _item.username = g.Key;
                _item.hoten = _nv != null && string.IsNullOrWhiteSpace(_nv.hoten) == false ? _nv.hoten : g.Key;
                _item.ten_bophan = _id_bophan != "" && _map_bophan.ContainsKey(_id_bophan) ? _map_bophan[_id_bophan] : "Chưa gán bộ phận";
                _item.tong_lich = _list_nv.Count;
                _item.so_lich_cho_xacnhan = _list_nv.Count(p => p.trangthai == datlich_class.trangthai_chua_xacnhan);
                _item.so_lich_da_den = _list_nv.Count(p => p.trangthai == datlich_class.trangthai_da_den);
                _item.so_lich_qua_gio = _qua_gio;
                _item.so_lich_sap_toi = _sap_toi;
                _item.id_lich_gan_nhat = _lich_gan != null ? _lich_gan.id.ToString() : "";
                _item.gio_hen_gan_nhat = _lich_gan != null ? _lich_gan.ngaydat : null;

                if (_qua_gio > 0)
                {
                    _item.muc_tai = "Cần xử lý";
                    _item.muc_tai_css = "bg-red";
                }
                else if (_item.tong_lich >= 6 || _sap_toi >= 4)
                {
                    _item.muc_tai = "Tải cao";
                    _item.muc_tai_css = "bg-orange";
                }
                else if (_item.tong_lich >= 3 || _sap_toi >= 1)
                {
                    _item.muc_tai = "Đang có lịch";
                    _item.muc_tai_css = "bg-green";
                }
                else
                {
                    _item.muc_tai = "Tải nhẹ";
                    _item.muc_tai_css = "bg-cyan";
                }

                return _item;
            })
            .OrderByDescending(p => p.so_lich_qua_gio)
            .ThenByDescending(p => p.tong_lich)
            .ThenBy(p => p.gio_hen_gan_nhat)
            .Take(8)
            .ToList();

        _result.list_bophan = _list_lich_hoatdong
            .GroupBy(p =>
            {
                if (string.IsNullOrWhiteSpace(p.nhanvien_thuchien))
                    return "unassigned";

                taikhoan_table_2023 _nv = _map_nhanvien.ContainsKey(p.nhanvien_thuchien.Trim()) ? _map_nhanvien[p.nhanvien_thuchien.Trim()] : null;
                string _id_bophan = _nv != null ? (_nv.id_bophan ?? "").Trim() : "";
                return _id_bophan != "" ? _id_bophan : "none";
            })
            .Select(g =>
            {
                lichhen_dieuphoi_bophan_item _item = new lichhen_dieuphoi_bophan_item();
                _item.id_bophan = g.Key;
                if (g.Key == "unassigned")
                    _item.ten_bophan = "Chưa phân công";
                else if (g.Key == "none")
                    _item.ten_bophan = "Chưa gán bộ phận";
                else
                    _item.ten_bophan = _map_bophan.ContainsKey(g.Key) ? _map_bophan[g.Key] : "Bộ phận #" + g.Key;

                _item.tong_lich = g.Count();
                _item.so_lich_cho_xacnhan = g.Count(p => p.trangthai == datlich_class.trangthai_chua_xacnhan);
                _item.so_lich_da_den = g.Count(p => p.trangthai == datlich_class.trangthai_da_den);
                _item.so_lich_qua_gio = g.Count(p => p.ngaydat.HasValue && p.ngaydat.Value <= _now && p.trangthai != datlich_class.trangthai_da_den);
                return _item;
            })
            .OrderByDescending(p => p.so_lich_qua_gio)
            .ThenByDescending(p => p.tong_lich)
            .Take(6)
            .ToList();

        List<lichhen_dieuphoi_canhbao_item> _alerts = new List<lichhen_dieuphoi_canhbao_item>();
        foreach (bspa_datlich_table _item in _list_lich_hoatdong)
        {
            lichhen_dieuphoi_canhbao_item _alert = tao_canhbao(_item, _now, _soon, _map_hoso);
            if (_alert != null)
                _alerts.Add(_alert);
        }

        _result.list_canhbao = _alerts
            .OrderBy(p => p.uu_tien)
            .ThenBy(p => p.ngaydat)
            .Take(8)
            .ToList();

        return _result;
    }

    private static lichhen_dieuphoi_canhbao_item tao_canhbao(
        bspa_datlich_table _lich,
        DateTime _now,
        DateTime _soon,
        Dictionary<string, string> _map_hoso)
    {
        if (_lich == null || _lich.ngaydat.HasValue == false)
            return null;

        string _sdt = datlich_class.chuanhoa_sdt(_lich.sdt);
        bool _co_hoso = _map_hoso.ContainsKey(_sdt);
        string _tieude = "Lịch #" + _lich.id + " - " + ((_lich.tenkhachhang ?? "").Trim() != "" ? _lich.tenkhachhang.Trim() : "Khách hẹn");
        string _mota = _lich.ngaydat.Value.ToString("HH:mm") + " | " + ((_lich.tendichvu_taithoidiemnay ?? "").Trim() != "" ? _lich.tendichvu_taithoidiemnay.Trim() : "Chưa chọn dịch vụ");

        if (_lich.ngaydat.Value <= _now && _lich.trangthai == datlich_class.trangthai_chua_xacnhan)
        {
            return new lichhen_dieuphoi_canhbao_item
            {
                uu_tien = 1,
                loai = "Quá giờ",
                loai_css = "bg-red",
                tieu_de = _tieude,
                mo_ta = _mota + " | Chưa xác nhận",
                ngaydat = _lich.ngaydat,
                id_lich = _lich.id.ToString(),
                url = "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=" + _lich.id
            };
        }

        if (_lich.ngaydat.Value <= _now && _lich.trangthai == datlich_class.trangthai_da_xacnhan)
        {
            return new lichhen_dieuphoi_canhbao_item
            {
                uu_tien = 2,
                loai = "Check-in",
                loai_css = "bg-orange",
                tieu_de = _tieude,
                mo_ta = _mota + " | Đã xác nhận nhưng chưa chốt đến",
                ngaydat = _lich.ngaydat,
                id_lich = _lich.id.ToString(),
                url = "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=" + _lich.id
            };
        }

        if (_lich.ngaydat.Value > _now && _lich.ngaydat.Value <= _soon && string.IsNullOrWhiteSpace(_lich.nhanvien_thuchien))
        {
            return new lichhen_dieuphoi_canhbao_item
            {
                uu_tien = 3,
                loai = "Phân công",
                loai_css = "bg-magenta",
                tieu_de = _tieude,
                mo_ta = _mota + " | Sắp tới nhưng chưa gán nhân viên",
                ngaydat = _lich.ngaydat,
                id_lich = _lich.id.ToString(),
                url = "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=" + _lich.id
            };
        }

        if (_lich.ngaydat.Value > _now && _lich.ngaydat.Value <= _soon && _lich.trangthai == datlich_class.trangthai_chua_xacnhan)
        {
            return new lichhen_dieuphoi_canhbao_item
            {
                uu_tien = 4,
                loai = "Xác nhận",
                loai_css = "bg-cyan",
                tieu_de = _tieude,
                mo_ta = _mota + " | Cần xác nhận trước giờ hẹn",
                ngaydat = _lich.ngaydat,
                id_lich = _lich.id.ToString(),
                url = "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=" + _lich.id
            };
        }

        if (_co_hoso == false)
        {
            return new lichhen_dieuphoi_canhbao_item
            {
                uu_tien = 5,
                loai = "CRM",
                loai_css = "bg-violet",
                tieu_de = _tieude,
                mo_ta = _mota + " | Khách chưa có hồ sơ CRM nội bộ",
                ngaydat = _lich.ngaydat,
                id_lich = _lich.id.ToString(),
                url = "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=" + _lich.id
            };
        }

        return null;
    }
}
