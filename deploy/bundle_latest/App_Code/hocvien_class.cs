using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for hocvien_class
/// </summary>
public class hocvien_class
{
    dbDataContext db = new dbDataContext();
    public bool exist_id(string _id)
    {
        var q = db.hocvien_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return true;
        return false;
    }
    public hocvien_table return_object(string _id)
    {
        var q = db.hocvien_tables.Single(p => p.id.ToString() == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        return q;
    }
    public List<hocvien_table> return_list()
    {
        var q = db.hocvien_tables.Where(p => p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).ToList();
        return q;
    }
    public void del(string _id)
    {
        HoaDonThuChiSync_cl.DeleteForHocVien(db, _id, System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        db.SubmitChanges();
        //xóa lịch sử thanh toán
        var q1 = db.hocvien_lichsu_thanhtoan_tables.Where(p => p.id_hocvien == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        foreach (var t in q1)//xóa chi tiết hóa đơn
        {
            hocvien_lichsu_thanhtoan_table _ob1 = t;
            db.hocvien_lichsu_thanhtoan_tables.DeleteOnSubmit(_ob1);
            db.SubmitChanges();
        }

        //xóa
        hocvien_table _ob = return_object(_id);
        db.hocvien_tables.DeleteOnSubmit(_ob);
        db.SubmitChanges();

    }
}
