using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for thedichvu_class
/// </summary>
public class thedichvu_class
{
    dbDataContext db = new dbDataContext();
    post_class po_cl = new post_class();
    public bool exist_id(string _id, string _user_parent)
    {
        var q = db.thedichvu_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return true;
        return false;
    }
    public thedichvu_table return_object(string _id)
    {
        var q = db.thedichvu_tables.Single(p => p.id.ToString() == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        return q;
    }
    public void del(string _id)
    {
        //var q = db.bspa_hoadon_chitiet_tables.Where(p => p.id_hoadon == _id);
        //foreach (var t in q)//xóa chi tiết hóa đơn
        //{
        //    bspa_hoadon_chitiet_table _ob1 = t;

        //    //tăng sl trong kho sp lên lại
        //    if (_ob1.kyhieu == "sanpham")
        //        po_cl.tang_soluong_sanpham(_ob1.id_dvsp, _ob1.soluong.Value);

        //    db.bspa_hoadon_chitiet_tables.DeleteOnSubmit(_ob1);
        //    db.SubmitChanges();
        //}


        thedichvu_table _ob = return_object(_id);
        if (_ob.tongsoluong == _ob.sl_conlai)//nếu thẻ chưa sử dụng thì mới cho xóa
        {
            HoaDonThuChiSync_cl.DeleteForTheDichVu(db, _id, System.Web.HttpContext.Current.Session["chinhanh"].ToString());
            db.SubmitChanges();
            db.thedichvu_tables.DeleteOnSubmit(_ob);
            db.SubmitChanges();

            //xóa lịch sử thanh toán
            var q1 = db.thedichvu_lichsu_thanhtoan_tables.Where(p => p.id_hoadon == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
            foreach (var t in q1)
            {
                thedichvu_lichsu_thanhtoan_table _ob1 = t;
                db.thedichvu_lichsu_thanhtoan_tables.DeleteOnSubmit(_ob1);
                db.SubmitChanges();
            }
        }



    }
    public string return_maxid(string _user_parent)
    {
        return db.thedichvu_tables.Max(p => p.id).ToString();
    }
    public static string return_soluong(string _user_parent)
    {
        dbDataContext db = new dbDataContext();
        return db.thedichvu_tables.Where(p => p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).Count().ToString("#,##0");
    }

}
