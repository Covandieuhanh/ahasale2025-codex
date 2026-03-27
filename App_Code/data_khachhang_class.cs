using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for data_khachhang_class
/// </summary>
public class data_khachhang_class
{
    dbDataContext db = new dbDataContext();
    random_class rd_cl = new random_class();

    private string CurrentChiNhanhId
    {
        get { return AhaShineContext_cl.ResolveChiNhanhId(); }
    }

    private static string ResolveCurrentChiNhanhId()
    {
        return AhaShineContext_cl.ResolveChiNhanhId();
    }

    public bool exist_id(string _id, string _user_parent)
    {
        var q = db.bspa_data_khachhang_tables.Where(p => p.id.ToString() == _id && p.user_parent == _user_parent && p.id_chinhanh == CurrentChiNhanhId);
        if (q.Count() != 0)
            return true;
        return false;
    }
    public bool exist_sdt(string _sdt)
    {
        var q = db.bspa_data_khachhang_tables.Where(p => p.sdt.ToString() == _sdt && p.id_chinhanh == CurrentChiNhanhId);
        if (q.Count() != 0)
            return true;
        return false;
    }
    public bool exist_sdt_cnaka(string _sdt)
    {
        var q = db.bspa_data_khachhang_tables.Where(p => p.sdt.ToString() == _sdt && p.id_chinhanh == CurrentChiNhanhId);
        if (q.Count() != 0)
            return true;
        return false;
    }
    public bool exist_sdt_cn(string _sdt, string _idcn)
    {
        var q = db.bspa_data_khachhang_tables.Where(p => p.sdt.ToString() == _sdt && p.id_chinhanh == _idcn);
        if (q.Count() != 0)
            return true;
        return false;
    }
    public bspa_data_khachhang_table return_object_sdt_cnaka(string _sdt)
    {
        var q = db.bspa_data_khachhang_tables.Single(p => p.sdt.ToString() == _sdt && p.id_chinhanh == CurrentChiNhanhId);
        return q;
    }
    public bspa_data_khachhang_table return_object(string _id)
    {
        var q = db.bspa_data_khachhang_tables.Single(p => p.id.ToString() == _id && p.id_chinhanh == CurrentChiNhanhId);
        return q;
    }
    public bspa_data_khachhang_table return_object_sdt(string _sdt)
    {
        var q = db.bspa_data_khachhang_tables.Single(p => p.sdt.ToString() == _sdt && p.id_chinhanh == CurrentChiNhanhId);
        return q;
    }
    public bspa_data_khachhang_table return_object_sdt(string _sdt,string _idcn)
    {
        var q = db.bspa_data_khachhang_tables.Single(p => p.sdt.ToString() == _sdt && p.id_chinhanh == _idcn);
        return q;
    }
    public void del(string _id)
    {
        //chuyển các dịch vụ thuộc nhóm sắp xóa về KHÔNG THUỘC NHÓM NÀO
        var q = db.bspa_data_khachhang_tables.Where(p => p.nhomkhachhang == _id && p.id_chinhanh == CurrentChiNhanhId);
        if (q.Count() != 0)
        {
            foreach (var t in q)
            {
                bspa_data_khachhang_table _ob1 = t;
                _ob1.nhomkhachhang = "";
                db.SubmitChanges();
            }
        }

        //xóa nhóm 
        bspa_data_khachhang_table _ob = return_object(_id);
        string _oldPhone = _ob == null ? "" : (_ob.sdt ?? "");
        string _oldName = _ob == null ? "" : (_ob.tenkhachhang ?? "");
        db.bspa_data_khachhang_tables.DeleteOnSubmit(_ob);
        db.SubmitChanges();
        GianHangAdminPersonHub_cl.PreserveLinkAfterSourceRemoval(
            db,
            AhaShineContext_cl.ResolveUserParent(),
            _oldPhone,
            _oldName,
            (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null ? ((System.Web.HttpContext.Current.Session["user"] ?? "") + "") : ""),
            "customer",
            "Khách hàng",
            _id,
            "Khách hàng",
            "Hồ sơ khách hàng đã bị xóa khỏi module nguồn.");
    }
    public static string return_soluong_khachhang(string _user_parent)
    {
        dbDataContext db = new dbDataContext();
        //var list_all = (from ob1 in db.bspa_data_khachhang_tables.Where(p => p.user_parent == _user_parent).ToList()
        //                join ob2 in db.bspa_hoadon_tables.ToList() on ob1.sdt.ToString() equals ob2.sdt
        //                group ob2 by new { ob1.id, ob2.tenkhachhang } into g
        //                select new
        //                {
        //                    id = g.Key.id,
        //                });
        string chiNhanhId = ResolveCurrentChiNhanhId();
        var list_all = db.bspa_data_khachhang_tables.Where(p => p.id_chinhanh == chiNhanhId);

        return list_all.Count().ToString("#,##0");
    }
    public int return_index(string _id)
    {
        int i = 1;      //=1 khi có thêm iteam "Chọn" luôn nằm đầu tiên mà chỉ số index bắt đầu =0                                    
        foreach (var t in db.nhomkhachhang_tables.Where(p => p.id_chinhanh == CurrentChiNhanhId))
        {
            if (_id == t.id.ToString())
                return i;
            i = i + 1;
        }
        return 0;
    }
    public string return_tennhom_khachhang(string _id)
    {
        var q = db.nhomkhachhang_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == CurrentChiNhanhId);
        if (q.Count() != 0)
            return q.First().tennhom;
        return "";
    }
    public string layid_tu_sdt(string _sdt)
    {
        var q = db.bspa_data_khachhang_tables.Where(p => p.sdt.ToString() == _sdt && p.id_chinhanh == CurrentChiNhanhId);
        if (q.Count() != 0)
            return q.First().id.ToString();
        return "";
    }
    public void update_vnd_tu_eaha(string _sdt)
    {
        var q = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt);
        if (q.Count() != 0)
        {
            bspa_data_khachhang_table _ob = q.First();
            _ob.vnd_tu_e_aha = _ob.sodiem_e_aha * 24000;
            db.SubmitChanges();
        }
    }
    public void update_capbac(string _sdt, string _capbac)
    {
        var q = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt);
        if (q.Count() != 0)
        {
            bspa_data_khachhang_table _ob = q.First();
            _ob.capbac = _capbac;
            db.SubmitChanges();
        }
    }
    public void update_solan_lencap(string _sdt, int _solan_lencap)
    {
        var q = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt);
        if (q.Count() != 0)
        {
            bspa_data_khachhang_table _ob = q.First();
            _ob.solan_lencap = _solan_lencap;
            db.SubmitChanges();
        }
    }


    public void tangdiem_eaha(string _taikhoan_nhan, string _taikhoan_gui, double _sodiem, string _noidung)
    {
        lichsudiem_table _ob1 = new lichsudiem_table();
        _ob1.ngay = DateTime.Now;
        _ob1.taikhoan_nhan = _taikhoan_nhan; _ob1.taikhoan_gui = _taikhoan_gui;
        _ob1.sodiem = _sodiem;
        _ob1.loaidiem = "E-AHA";
        _ob1.noidung = _noidung;
        _ob1.tanggiam = "+";//ký hiệu của tăng
        db.lichsudiem_tables.InsertOnSubmit(_ob1);
        db.SubmitChanges();

        bspa_data_khachhang_table _ob = db.bspa_data_khachhang_tables.Where(p => p.sdt == _taikhoan_nhan).First();
        _ob.sodiem_e_aha = _ob.sodiem_e_aha + _sodiem;
        db.SubmitChanges();

        //update vnđ từ eaha
        update_vnd_tu_eaha(_taikhoan_nhan);
    }
    public void giamdiem_eaha(string _taikhoan_nhan, string _taikhoan_gui, double _sodiem, string _noidung)
    {
        lichsudiem_table _ob1 = new lichsudiem_table();
        _ob1.ngay = DateTime.Now;
        _ob1.taikhoan_nhan = _taikhoan_nhan; _ob1.taikhoan_gui = _taikhoan_gui;
        _ob1.sodiem = _sodiem;
        _ob1.loaidiem = "E-AHA";
        _ob1.noidung = _noidung;
        _ob1.tanggiam = "-";//ký hiệu của giảm
        db.lichsudiem_tables.InsertOnSubmit(_ob1);
        db.SubmitChanges();

        bspa_data_khachhang_table _ob = db.bspa_data_khachhang_tables.Where(p => p.sdt == _taikhoan_nhan).First();
        _ob.sodiem_e_aha = _ob.sodiem_e_aha - _sodiem;
        db.SubmitChanges();

        //update vnđ từ eaha
        update_vnd_tu_eaha(_taikhoan_nhan);
    }

    public void tinhtong_chitieu_update_capbac(string _sdt)
    {
        var q_kh = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt);
        if (q_kh.Count() != 0)
        {
            bspa_data_khachhang_table _ob = q_kh.First();

            double _kq = 0;
            var q = db.bspa_hoadon_tables.Where(p => p.sdt == _sdt && p.sotien_conlai == 0);
            if (q.Count() != 0)
            {
                _kq = _kq + q.Sum(p => p.tongsauchietkhau.Value);
            }
            var q_thedv = db.thedichvu_tables.Where(p => p.sdt == _sdt && p.sotien_conlai == 0);
            if (q_thedv.Count() != 0)
            {
                _kq = _kq + q_thedv.Sum(p => p.tongsauchietkhau.Value);
            }
            var q_hocvien = db.hocvien_tables.Where(p => p.dienthoai == _sdt && p.sotien_conlai == 0);
            if (q_hocvien.Count() != 0)
            {
                _kq = _kq + q_hocvien.Sum(p => p.hocphi.Value);
            }

            //đã có tổng chi tiêu của khách hàng tại đây

            if (_kq >= 48000000)
            {
                if (_ob.solan_lencap.Value < 3)//tránh trường hợp tính lại
                {
                    double _phantram = 48000000 * 8 / 100;//8%
                    double _quydoi_thanh_eaha = eaha_class.quydoi_vnd_sang_eaha(_phantram);
                    tangdiem_eaha(_sdt, "admin", _quydoi_thanh_eaha, "Lên cấp VIP");//cộng điểm eaha
                    update_capbac(_sdt, "VIP");//sửa cấp bậc thành VIP
                    update_solan_lencap(_sdt, 3);//đánh dấu đã lên cấp 3
                }
            }
            else
            {
                if (_kq >= 4800000)
                {
                    if (_ob.solan_lencap.Value < 2)//tránh trường hợp tính lại
                    {
                        double _phantram = 4800000 * 5 / 100;//5%
                        double _quydoi_thanh_eaha = eaha_class.quydoi_vnd_sang_eaha(_phantram);
                        tangdiem_eaha(_sdt, "admin", _quydoi_thanh_eaha, "Lên cấp PRO");//cộng điểm eaha
                        update_capbac(_sdt, "PRO");//sửa cấp bậc thành PRO
                        update_solan_lencap(_sdt, 2);//đánh dấu đã lên cấp 2
                    }
                }
                else
                {
                    if (_kq >= 480000)
                    {
                        if (_ob.solan_lencap.Value < 1)//tránh trường hợp tính lại
                        {
                            double _phantram = 480000 * 3 / 100;//3%
                            double _quydoi_thanh_eaha = eaha_class.quydoi_vnd_sang_eaha(_phantram);
                            tangdiem_eaha(_sdt, "admin", _quydoi_thanh_eaha, "Lên cấp BASIC");//cộng điểm eaha
                            update_capbac(_sdt, "BASIC");//sửa cấp bậc thành BASIC
                            update_solan_lencap(_sdt, 1);//đánh dấu đã lên cấp 1
                        }
                    }
                }
            }
        }
    }
}
