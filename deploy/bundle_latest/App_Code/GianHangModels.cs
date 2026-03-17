using System;
using System.Data.Linq.Mapping;

[Table(Name = "dbo.GH_SanPham_tb")]
public class GH_SanPham_tb
{
    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int id { get; set; }

    [Column]
    public string shop_taikhoan { get; set; }

    [Column]
    public string ten { get; set; }

    [Column]
    public string mo_ta { get; set; }

    [Column]
    public string noi_dung { get; set; }

    [Column]
    public string hinh_anh { get; set; }

    [Column]
    public decimal? gia_ban { get; set; }

    [Column]
    public long? gia_von { get; set; }

    [Column]
    public string loai { get; set; }

    [Column]
    public string id_danhmuc { get; set; }

    [Column]
    public bool? bin { get; set; }

    [Column]
    public DateTime? ngay_tao { get; set; }

    [Column]
    public DateTime? ngay_cap_nhat { get; set; }

    [Column]
    public int? id_baiviet { get; set; }

    [Column]
    public int? so_luong_ton { get; set; }

    [Column]
    public int? so_luong_da_ban { get; set; }

    [Column]
    public int? luot_truy_cap { get; set; }
}

[Table(Name = "dbo.GH_DatLich_tb")]
public class GH_DatLich_tb
{
    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public long id { get; set; }

    [Column]
    public string shop_taikhoan { get; set; }

    [Column]
    public string ten_khach { get; set; }

    [Column]
    public string sdt { get; set; }

    [Column]
    public string dich_vu { get; set; }

    [Column]
    public DateTime? thoi_gian_hen { get; set; }

    [Column]
    public string ghi_chu { get; set; }

    [Column]
    public string trang_thai { get; set; }

    [Column]
    public DateTime? ngay_tao { get; set; }
}

[Table(Name = "dbo.GH_HoaDon_tb")]
public class GH_HoaDon_tb
{
    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public long id { get; set; }

    [Column]
    public string shop_taikhoan { get; set; }

    [Column]
    public string ten_khach { get; set; }

    [Column]
    public string sdt { get; set; }

    [Column]
    public string dia_chi { get; set; }

    [Column]
    public string ghi_chu { get; set; }

    [Column]
    public decimal? tong_tien { get; set; }

    [Column]
    public string trang_thai { get; set; }

    [Column]
    public DateTime? ngay_tao { get; set; }
}

[Table(Name = "dbo.GH_HoaDon_ChiTiet_tb")]
public class GH_HoaDon_ChiTiet_tb
{
    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public long id { get; set; }

    [Column]
    public long id_hoadon { get; set; }

    [Column]
    public int? id_sanpham { get; set; }

    [Column]
    public string ten_sanpham { get; set; }

    [Column]
    public int? so_luong { get; set; }

    [Column]
    public decimal? gia { get; set; }

    [Column]
    public decimal? thanh_tien { get; set; }
}
