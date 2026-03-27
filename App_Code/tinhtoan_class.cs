using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class tinhtoan_class
{
    #region Tính thu chi
    public static Int64 tinh_tong_thuchi(string _user_parent, DateTime _tungay, DateTime _denngay)
    {
        return tinh_tong_thu(_user_parent, _tungay, _denngay) - tinh_tong_chi(_user_parent, _tungay, _denngay);
    }
    public static Int64 tinh_tong_thu(string _user_parent, DateTime _tungay, DateTime _denngay)
    {
        dbDataContext db = new dbDataContext();
        Int64 thu = 0;
        var q_thu = (from ob1 in db.bspa_thuchi_tables.Where(p => p.thuchi == "Thu" && p.user_parent == _user_parent && p.tudong_tu_hoadon != true && p.ngay.Value.Date >= _tungay && p.ngay.Value.Date <= _denngay).ToList()
                     select new
                     {
                         sotien = ob1.sotien,
                     }).ToList();
        if (q_thu.Count() != 0)
            thu = q_thu.Sum(p => p.sotien).Value;
        return thu;
    }
    public static Int64 tinh_tong_chi(string _user_parent, DateTime _tungay, DateTime _denngay)
    {
        dbDataContext db = new dbDataContext();
        Int64 chi = 0;
        var q_chi = (from ob1 in db.bspa_thuchi_tables.Where(p => p.thuchi == "Chi" && p.user_parent == _user_parent && p.ngay.Value.Date >= _tungay && p.ngay.Value.Date <= _denngay).ToList()
                     select new
                     {
                         sotien = ob1.sotien,
                     }).ToList();
        if (q_chi.Count() != 0)
            chi = q_chi.Sum(p => p.sotien).Value;
        return chi;
    }
    public static Int64 tinh_tong_thuchi_theo_thangnam(string _user_parent, int _thang, int _nam)
    {
        return tinh_tong_thu_theo_thangnam(_user_parent, _thang, _nam) - tinh_tong_chi_theo_thangnam(_user_parent, _thang, _nam);
    }
    public static Int64 tinh_tong_thu_theo_thangnam(string _user_parent, int _thang, int _nam)
    {
        dbDataContext db = new dbDataContext();
        Int64 thu = 0;
        var q_thu = (from ob1 in db.bspa_thuchi_tables.Where(p => p.thuchi == "Thu" && p.user_parent == _user_parent && p.tudong_tu_hoadon != true && p.ngay.Value.Month == _thang && p.ngay.Value.Year == _nam).ToList()
                     select new
                     {
                         sotien = ob1.sotien,
                     }).ToList();
        if (q_thu.Count() != 0)
            thu = q_thu.Sum(p => p.sotien).Value;
        return thu;
    }
    public static Int64 tinh_tong_chi_theo_thangnam(string _user_parent, int _thang, int _nam)
    {
        dbDataContext db = new dbDataContext();
        Int64 chi = 0;
        var q_chi = (from ob1 in db.bspa_thuchi_tables.Where(p => p.thuchi == "Chi" && p.user_parent == _user_parent && p.ngay.Value.Month == _thang && p.ngay.Value.Year == _nam).ToList()
                     select new
                     {
                         sotien = ob1.sotien,
                     }).ToList();
        if (q_chi.Count() != 0)
            chi = q_chi.Sum(p => p.sotien).Value;
        return chi;
    }
    #endregion

    public static Int64 tinh_tong_doanhso(string _user_parent, DateTime _tungay, DateTime _denngay)
    {
        dbDataContext db = new dbDataContext();
        return tinh_tong_doanhso_dichvu(_user_parent, _tungay, _denngay) + tinh_tong_doanhso_sanpham(_user_parent, _tungay, _denngay);
    }
    public static Int64 tinh_tong_doanhso_hoadon(string _user_parent, DateTime _tungay, DateTime _denngay)
    {
        dbDataContext db = new dbDataContext();
        Int64 tong = 0;
        var q = (from ob1 in db.bspa_hoadon_tables.Where(p => p.user_parent == _user_parent && p.ngaytao.Value.Date >= _tungay && p.ngaytao.Value.Date <= _denngay).ToList()
                 select new
                 {
                     tongtien = ob1.tongtien,
                 });
        if (q.Count() != 0)
            return q.Sum(p => p.tongtien).Value;
        return tong;
    }
    public static Int64 tinh_tong_doanhso_hoadon_sauck(string _user_parent, DateTime _tungay, DateTime _denngay)
    {
        dbDataContext db = new dbDataContext();
        Int64 tong = 0;
        var q = (from ob1 in db.bspa_hoadon_tables.Where(p => p.user_parent == _user_parent && p.ngaytao.Value.Date >= _tungay && p.ngaytao.Value.Date <= _denngay).ToList()
                 select new
                 {
                     tongsauchietkhau = ob1.tongsauchietkhau,
                 });
        if (q.Count() != 0)
            return q.Sum(p => p.tongsauchietkhau).Value;
        return tong;
    }
    public static Int64 tinh_tong_doanhso_dichvu(string _user_parent, DateTime _tungay, DateTime _denngay)
    {
        dbDataContext db = new dbDataContext();
        Int64 tong = 0;
        var q = (from ob1 in db.bspa_hoadon_chitiet_tables.Where(p => p.kyhieu == "dichvu" && p.user_parent == _user_parent && p.ngaytao.Value.Date >= _tungay && p.ngaytao.Value.Date <= _denngay).ToList()
                 select new
                 {
                     thanhtien = ob1.thanhtien,
                 });
        if (q.Count() != 0)
            return q.Sum(p => p.thanhtien).Value;
        return tong;
    }
    public static Int64 tinh_tong_soluong_dichvu(string _user_parent, DateTime _tungay, DateTime _denngay)
    {
        dbDataContext db = new dbDataContext();
        Int64 tong = 0;
        var q = (from ob1 in db.bspa_hoadon_chitiet_tables.Where(p => p.kyhieu == "dichvu" && p.user_parent == _user_parent && p.ngaytao.Value.Date >= _tungay && p.ngaytao.Value.Date <= _denngay).ToList()
                 select new
                 {
                     soluong = ob1.soluong,
                 });
        if (q.Count() != 0)
            return q.Sum(p => p.soluong).Value;
        return tong;
    }
    public static Int64 tinh_tong_doanhso_dichvu_sauck(string _user_parent, DateTime _tungay, DateTime _denngay)
    {
        dbDataContext db = new dbDataContext();
        Int64 tong = 0;
        var q = (from ob1 in db.bspa_hoadon_chitiet_tables.Where(p => p.kyhieu == "dichvu" && p.user_parent == _user_parent && p.ngaytao.Value.Date >= _tungay && p.ngaytao.Value.Date <= _denngay).ToList()
                 select new
                 {
                     tongsauchietkhau = ob1.tongsauchietkhau,
                 });
        if (q.Count() != 0)
            return q.Sum(p => p.tongsauchietkhau).Value;
        return tong;
    }
    public static Int64 tinh_tong_doanhso_sanpham(string _user_parent, DateTime _tungay, DateTime _denngay)
    {
        dbDataContext db = new dbDataContext();
        Int64 tong = 0;
        var q = (from ob1 in db.bspa_hoadon_chitiet_tables.Where(p => p.kyhieu == "sanpham" && p.user_parent == _user_parent && p.ngaytao.Value.Date >= _tungay && p.ngaytao.Value.Date <= _denngay).ToList()
                 select new
                 {
                     thanhtien = ob1.thanhtien,
                 });
        if (q.Count() != 0)
            return q.Sum(p => p.thanhtien).Value;
        return tong;
    }
    public static Int64 tinh_tong_soluong_sanpham(string _user_parent, DateTime _tungay, DateTime _denngay)
    {
        dbDataContext db = new dbDataContext();
        Int64 tong = 0;
        var q = (from ob1 in db.bspa_hoadon_chitiet_tables.Where(p => p.kyhieu == "sanpham" && p.user_parent == _user_parent && p.ngaytao.Value.Date >= _tungay && p.ngaytao.Value.Date <= _denngay).ToList()
                 select new
                 {
                     soluong = ob1.soluong,
                 });
        if (q.Count() != 0)
            return q.Sum(p => p.soluong).Value;
        return tong;
    }
    public static Int64 tinh_tong_doanhso_sanpham_sauck(string _user_parent, DateTime _tungay, DateTime _denngay)
    {
        dbDataContext db = new dbDataContext();
        Int64 tong = 0;
        var q = (from ob1 in db.bspa_hoadon_chitiet_tables.Where(p => p.kyhieu == "sanpham" && p.user_parent == _user_parent && p.ngaytao.Value.Date >= _tungay && p.ngaytao.Value.Date <= _denngay).ToList()
                 select new
                 {
                     tongsauchietkhau = ob1.tongsauchietkhau,
                 });
        if (q.Count() != 0)
            return q.Sum(p => p.tongsauchietkhau).Value;
        return tong;
    }
    public static Int64 tinh_tong_doanhso_theo_thangnam(string _user_parent, int _thang, int _nam)
    {
        dbDataContext db = new dbDataContext();
        return tinh_tong_doanhso_dichvu_theo_thangnam(_user_parent, _thang, _nam) + tinh_tong_doanhso_sanpham_theo_thangnam(_user_parent, _thang, _nam);
    }
    public static Int64 tinh_tong_doanhso_dichvu_theo_thangnam(string _user_parent, int _thang, int _nam)
    {
        dbDataContext db = new dbDataContext();
        Int64 tong = 0;
        var q = (from ob1 in db.bspa_hoadon_chitiet_tables.Where(p => p.kyhieu == "dichvu" && p.user_parent == _user_parent && p.ngaytao.Value.Month == _thang && p.ngaytao.Value.Year == _nam).ToList()
                 select new
                 {
                     thanhtien = ob1.thanhtien,
                 });
        if (q.Count() != 0)
            return q.Sum(p => p.thanhtien).Value;
        return tong;
    }
    public static Int64 tinh_tong_doanhso_sanpham_theo_thangnam(string _user_parent, int _thang, int _nam)
    {
        dbDataContext db = new dbDataContext();
        Int64 tong = 0;
        var q = (from ob1 in db.bspa_hoadon_chitiet_tables.Where(p => p.kyhieu == "sanpham" && p.user_parent == _user_parent && p.ngaytao.Value.Month == _thang && p.ngaytao.Value.Year == _nam).ToList()
                 select new
                 {
                     thanhtien = ob1.thanhtien,
                 });
        if (q.Count() != 0)
            return q.Sum(p => p.thanhtien).Value;
        return tong;
    }

    public static Int64 tinh_tong_soluong_hoadon(string _user_parent, DateTime _tungay, DateTime _denngay)
    {
        dbDataContext db = new dbDataContext();
        var q = db.bspa_hoadon_tables.Where(p => p.user_parent == _user_parent && p.ngaytao.Value.Date >= _tungay && p.ngaytao.Value.Date <= _denngay);
        return q.Count();
    }

    public static Int64 tinh_tong_doanhthu_hoadon(string _user_parent, DateTime _tungay, DateTime _denngay)//tính tổng thu từ hóa đơn dựa vào lịch sử thanh toán
    {
        dbDataContext db = new dbDataContext();
        Int64 tong = 0;
        var q = (from ob1 in db.bspa_lichsu_thanhtoan_tables.Where(p => p.user_parent == _user_parent && p.thoigian.Value.Date >= _tungay && p.thoigian.Value.Date <= _denngay).ToList()
                 select new
                 {
                     sotienthanhtoan = ob1.sotienthanhtoan,
                 });
        if (q.Count() != 0)
            return q.Sum(p => p.sotienthanhtoan).Value;
        return tong;
    }
    public static Int64 tinh_tong_doanhthu_hoadon_theo_thangnam(string _user_parent, int _thang, int _nam)//tính tổng thu từ hóa đơn dựa vào lịch sử thanh toán
    {
        dbDataContext db = new dbDataContext();
        Int64 tong = 0;
        var q = (from ob1 in db.bspa_lichsu_thanhtoan_tables.Where(p => p.user_parent == _user_parent && p.thoigian.Value.Month == _thang && p.thoigian.Value.Year == _nam).ToList()
                 select new
                 {
                     sotienthanhtoan = ob1.sotienthanhtoan,
                 });
        if (q.Count() != 0)
            return q.Sum(p => p.sotienthanhtoan).Value;
        return tong;
    }
    public static Int64 tinh_tong_doanhthu(string _user_parent, DateTime _tungay, DateTime _denngay)//tính tổng tất cả các nguồn thu
    {
        //tổng thu từ thu chi + tổng thu từ hóa đơn
        dbDataContext db = new dbDataContext();
        return tinh_tong_thu(_user_parent, _tungay, _denngay) + tinh_tong_doanhthu_hoadon(_user_parent, _tungay, _denngay);
    }
    public static Int64 tinh_tong_doanhthu_theo_thangnam(string _user_parent, int _thang, int _nam)//tính tổng tất cả các nguồn thu
    {
        //tổng thu từ thu chi + tổng thu từ hóa đơn
        dbDataContext db = new dbDataContext();
        return tinh_tong_thu_theo_thangnam(_user_parent, _thang, _nam) + tinh_tong_doanhthu_hoadon_theo_thangnam(_user_parent, _thang, _nam);
    }

    public static Int64 tinh_tong_chiphi_codinh_theo_thangnam(string _user_parent, int _thang, int _nam)
    {
        dbDataContext db = new dbDataContext();
        Int64 tong = 0;
        var q = (from ob1 in db.bspa_chiphi_codinh_tables.Where(p => p.user_parent == _user_parent && p.apdung.Value.Month == _thang && p.apdung.Value.Year == _nam).ToList()
                 select new
                 {
                     sotien = ob1.sotien,
                 });
        if (q.Count() != 0)
            return q.Sum(p => p.sotien).Value;
        return tong;
    }
    public static Int64 tinh_tong_chiphi(string _user_parent, DateTime _tungay, DateTime _denngay)//tính tổng tất cả các chi phí
    {
        //tổng thu chi thu chi
        //+ tổng chi phí cố định
        dbDataContext db = new dbDataContext();
        return tinh_tong_chi(_user_parent, _tungay, _denngay) + tinh_tong_chiphi_codinh_theo_thangnam(_user_parent, _tungay.Month, _denngay.Year);
    }
    public static Int64 tinh_tong_chiphi_theo_thangnam(string _user_parent, int _thang, int _nam)
    {
        //tổng thu chi thu chi
        //+ tổng chi phí cố định
        dbDataContext db = new dbDataContext();
        return tinh_tong_chi_theo_thangnam(_user_parent, _thang, _nam) + tinh_tong_chiphi_codinh_theo_thangnam(_user_parent,_thang,_nam);
    }

    public static Int64 tinh_tong_loinhuan(string _user_parent, DateTime _tungay, DateTime _denngay)//tính tổng lợi nhuận
    {
        //tổng thu chi thu chi
        //+ tổng chi phí cố định
        dbDataContext db = new dbDataContext();
        return tinh_tong_doanhthu(_user_parent, _tungay, _denngay) - tinh_tong_chiphi_codinh_theo_thangnam(_user_parent, _tungay.Month, _denngay.Year);
    }
    public static Int64 tinh_tong_loinhuan_theo_thangnam(string _user_parent, int _thang, int _nam)//tính tổng lợi nhuận
    {
        //tổng thu chi thu chi
        //+ tổng chi phí cố định
        dbDataContext db = new dbDataContext();
        return tinh_tong_doanhthu_theo_thangnam(_user_parent, _thang, _nam) - tinh_tong_chiphi_theo_thangnam(_user_parent, _thang, _nam);
    }


}
