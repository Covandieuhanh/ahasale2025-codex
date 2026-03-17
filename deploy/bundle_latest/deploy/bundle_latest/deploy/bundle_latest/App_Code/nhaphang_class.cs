using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class nhaphang_class
{
    dbDataContext db = new dbDataContext();
    public static Int64 return_maxid()
    {
        dbDataContext db = new dbDataContext();
        var q = db.donnhaphang_tables;
        if (q.Count() == 0)
            return 1;
        return q.Max(p => p.id + 1);
    }
    public string return_id_bang_idguide(string _idguide)
    {
        var q = db.donnhaphang_tables.Where(p => p.id_guide.ToString() == _idguide.Trim() && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return q.First().id.ToString();
        return "";
    }
    public bool exist_id(string _id, string _user_parent)
    {
        var q = db.donnhaphang_tables.Where(p => p.id.ToString() == _id && p.user_parent == _user_parent && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return true;
        return false;
    }
    public donnhaphang_table return_object(string _id)
    {
        var q = db.donnhaphang_tables.Single(p => p.id.ToString() == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        return q;
    }
    public void del(string _id)
    {
        var q = db.donnhaphang_chitiet_tables.Where(p => p.id_hoadon == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        foreach (var t in q)//xóa chi tiết hóa đơn
        {
            donnhaphang_chitiet_table _ob1 = t;
            //k cần tăng sl lên lại, vì mất chi tiết đơn nhập hàng là mất sl lô hàng, nc khỏi lăn tăn
            db.donnhaphang_chitiet_tables.DeleteOnSubmit(_ob1);
            db.SubmitChanges();
        }

        //xóa lịch sử thanh toán
        HoaDonThuChiSync_cl.DeleteForNhapHang(db, _id, System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        db.SubmitChanges();
        var q1 = db.donnhaphang_lichsu_thanhtoan_tables.Where(p => p.id_hoadon == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        foreach (var t in q1)//xóa chi tiết hóa đơn
        {
            donnhaphang_lichsu_thanhtoan_table _ob1 = t;
            db.donnhaphang_lichsu_thanhtoan_tables.DeleteOnSubmit(_ob1);
            db.SubmitChanges();
        }

        //xóa đơn nhập hàng
        donnhaphang_table _ob = return_object(_id);
        db.donnhaphang_tables.DeleteOnSubmit(_ob);
        db.SubmitChanges();

    }
    public string return_maxid(string _user_parent)
    {
        return db.donnhaphang_tables.Where(p => p.user_parent == _user_parent && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).Max(p => p.id).ToString();
    }
    public static string return_soluong_hoadon(string _user_parent)
    {
        dbDataContext db = new dbDataContext();
        return db.donnhaphang_tables.Where(p => p.user_parent == _user_parent && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).Count().ToString("#,##0");
    }
   public string return_name_ncc(string _id)
    {
        var q = db.nhacungcap_nhaphang_tables.Where(p=>p.id.ToString()== _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return q.First().ten;
        return "";
    }
    public int return_index_ncc(string _id)
    {
        int i = 1;      //=1 khi có thêm iteam "Chọn" luôn nằm đầu tiên mà chỉ số index bắt đầu =0                                    
        foreach (var t in db.nhacungcap_nhaphang_tables.Where(p => p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()))
        {
            if (_id == t.id.ToString())
                return i;
            i = i + 1;
        }
        return 0;
    }
}
