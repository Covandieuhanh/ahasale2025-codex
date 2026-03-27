using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for hoadon_home_class
/// </summary>
public class hoadon_home_class
{
    dbDataContext db = new dbDataContext();
    post_class po_cl = new post_class();

    private static string ResolveCurrentChiNhanhId()
    {
        HttpContext context = HttpContext.Current;
        return GianHangContext_cl.ResolveSessionChiNhanhId(context == null ? null : context.Session);
    }

    public bool exist_id(string _id, string _user_parent)
    {
        string chiNhanhId = ResolveCurrentChiNhanhId();
        var q = db.bspa_hoadon_tables.Where(p => p.id.ToString() == _id && p.user_parent == _user_parent && p.id_chinhanh == chiNhanhId);
        if (q.Count() != 0)
            return true;
        return false;
    }
    public bspa_hoadon_table return_object(string _id)
    {
        string chiNhanhId = ResolveCurrentChiNhanhId();
        var q = db.bspa_hoadon_tables.Single(p => p.id.ToString() == _id && p.id_chinhanh == chiNhanhId);
        return q;
    }
    public void del(string _id)
    {
        string _id_chinhanh = ResolveCurrentChiNhanhId();
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
        string chiNhanhId = ResolveCurrentChiNhanhId();
        return db.bspa_hoadon_tables.Where(p => p.user_parent == _user_parent && p.id_chinhanh == chiNhanhId).Max(p => p.id).ToString();
    }
    public static string return_soluong_hoadon(string _user_parent)
    {
        dbDataContext db = new dbDataContext();
        string chiNhanhId = ResolveCurrentChiNhanhId();
        return db.bspa_hoadon_tables.Where(p => p.user_parent == _user_parent && p.id_chinhanh == chiNhanhId).Count().ToString("#,##0");
    }
    public string return_id_bang_idguide(string _idguide)
    {
        string chiNhanhId = ResolveCurrentChiNhanhId();
        var q = db.bspa_hoadon_tables.Where(p => p.id_guide.ToString() == _idguide.Trim() && p.id_chinhanh == chiNhanhId);
        if (q.Count() != 0)
            return q.First().id.ToString();
        return "";
    }
}
