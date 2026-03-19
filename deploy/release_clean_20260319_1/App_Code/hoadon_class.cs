using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for hoadon_class
/// </summary>
public class hoadon_class
{
    dbDataContext db = new dbDataContext();
    post_class po_cl = new post_class();
    public bool exist_id(string _id, string _user_parent)
    {
        var q = db.bspa_hoadon_tables.Where(p => p.id.ToString() == _id && p.user_parent == _user_parent && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return true;
        return false;
    }
    public bspa_hoadon_table return_object(string _id)
    {
        var q = db.bspa_hoadon_tables.Single(p => p.id.ToString() == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        return q;
    }
    public void del(string _id)
    {
        string _id_chinhanh = System.Web.HttpContext.Current.Session["chinhanh"].ToString();
        var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id_hoadon == _id && p.id_chinhanh == _id_chinhanh);
        foreach (var t in q)//xóa chi tiết hóa đơn
        {
            bspa_hoadon_chitiet_table _ob1 = t;

            //tăng sl trong kho sp lên lại
            if (_ob1.kyhieu == "sanpham")
                po_cl.tang_soluong_sanpham(_ob1.id_dvsp, _ob1.soluong.Value);

            if (_ob1.id_thedichvu != null)
            {
                //công trừ số buổi của thẻ
                var q_thedv = db.thedichvu_tables.Where(p => p.id.ToString() == _ob1.id_thedichvu && p.id_chinhanh == _id_chinhanh);
                if (q_thedv.Count() != 0)
                {
                    thedichvu_table _ob_thedv = q_thedv.First();
                    _ob_thedv.sl_dalam = _ob_thedv.sl_dalam - 1;
                    _ob_thedv.sl_conlai = _ob_thedv.tongsoluong - _ob_thedv.sl_dalam;
                    db.SubmitChanges();
                }
            }

            db.bspa_hoadon_chitiet_tables.DeleteOnSubmit(_ob1);
            db.SubmitChanges();
        }

        //xóa lịch sử thanh toán
        HoaDonThuChiSync_cl.DeleteForInvoice(db, _id, _id_chinhanh);
        db.SubmitChanges();
        var q1 = db.bspa_lichsu_thanhtoan_tables.Where(p => p.id_hoadon == _id && p.id_chinhanh == _id_chinhanh);
        foreach (var t in q1)//xóa chi tiết hóa đơn
        {
            bspa_lichsu_thanhtoan_table _ob1 = t;
            db.bspa_lichsu_thanhtoan_tables.DeleteOnSubmit(_ob1);
            db.SubmitChanges();
        }

        //xóa hóa đơn
        bspa_hoadon_table _ob = return_object(_id);
        db.bspa_hoadon_tables.DeleteOnSubmit(_ob);
        db.SubmitChanges();

    }
    public string return_maxid(string _user_parent)
    {
        return db.bspa_hoadon_tables.Where(p => p.user_parent == _user_parent && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).Max(p => p.id).ToString();
    }
    public static string return_soluong_hoadon(string _user_parent)
    {
        dbDataContext db = new dbDataContext();
        return db.bspa_hoadon_tables.Where(p => p.user_parent == _user_parent && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).Count().ToString("#,##0");
    }
    public string return_id_bang_idguide(string _idguide)
    {
        var q = db.bspa_hoadon_tables.Where(p => p.id_guide.ToString() == _idguide.Trim() && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return q.First().id.ToString();
        return "";
    }

    public void tinhtoan_diemthuong_eaha(string _sdt_khachhang)
    {
        var q_khach = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt_khachhang && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q_khach.Count() != 0)
        {

            if (q_khach.First().solan_lencap.Value != 3)//đã tính thưởng mốc 3 thì k tính nữa
            {
                double _tong_thanhtoan = 0, _eaha_tang = 0;
                //lấy ra toàn bộ hóa đơn của khách hàng này, thuộc chi nhánh này
                var q = db.bspa_hoadon_tables.Where(p => p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString() && p.sdt == _sdt_khachhang);
                //duyệt từng hóa đơn để tính tổng thanh toán
                foreach (var t in q)
                {
                    var q_thanhtoan = db.bspa_lichsu_thanhtoan_tables.Where(p => p.id_hoadon == t.id.ToString());
                    _tong_thanhtoan = _tong_thanhtoan + q_thanhtoan.Sum(p => p.sotienthanhtoan.Value);
                }
                if (_tong_thanhtoan >= 48000000)
                {
                    bspa_data_khachhang_table _ob = q_khach.First();
                    double _8_phantram = 48000000 * 8 / 100;//8% cua 48tr
                    _eaha_tang = eaha_class.quydoi_vnd_sang_eaha(_8_phantram);
                    _ob.sodiem_e_aha = _ob.sodiem_e_aha + _eaha_tang;
                    _ob.solan_lencap = 3;//đc tặng mốc này là hết, k có tính toán nữa.
                    db.SubmitChanges();
                }
                else
                {
                    if (_tong_thanhtoan >= 4800000)
                    {
                        bspa_data_khachhang_table _ob = q_khach.First();
                        double _5_phantram = 4800000 * 5 / 100;//5% cua 4,8tr
                        _eaha_tang = eaha_class.quydoi_vnd_sang_eaha(_5_phantram);
                        _ob.sodiem_e_aha = _ob.sodiem_e_aha + _eaha_tang;
                        _ob.solan_lencap = 2;
                        db.SubmitChanges();
                    }
                    else
                    {
                        if (_tong_thanhtoan >= 480000)
                        {
                            bspa_data_khachhang_table _ob = q_khach.First();
                            double _3_phantram = 480000 * 3 / 100;//3% cua 480k
                            _eaha_tang = eaha_class.quydoi_vnd_sang_eaha(_3_phantram);
                            _ob.sodiem_e_aha = _ob.sodiem_e_aha + _eaha_tang;
                            _ob.solan_lencap = 1;
                            db.SubmitChanges();
                        }
                    }
                }
            }

        }
    }

}
