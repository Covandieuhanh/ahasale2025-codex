using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class giangvien_class
{
    dbDataContext db = new dbDataContext();
    public bool exist_id(string _id)
    {
        var q = db.giangvien_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return true;
        return false;
    }
    public giangvien_table return_object(string _id)
    {
        var q = db.giangvien_tables.Single(p => p.id.ToString() == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        return q;
    }
    public List<giangvien_table> return_list()
    {
        var q = db.giangvien_tables.Where(p => p.trangthai == "Đang giảng dạy" && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).ToList();
        return q;
    }
    public List<giangvien_table> return_list_nganh()
    {
        var q = db.giangvien_tables.Where(p => p.trangthai == "Đang giảng dạy" && p.chuyenmon == System.Web.HttpContext.Current.Session["nganh"].ToString() && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).ToList();
        return q;
    }
    public int return_index(string _id)
    {
        int i = 1;      //=1 khi có thêm iteam "Chọn" luôn nằm đầu tiên mà chỉ số index bắt đầu =0                                    
        foreach (var t in return_list())
        {
            if (_id == t.id.ToString())
                return i;
            i = i + 1;
        }
        return 0;
    }
    public int return_index_nganh(string _id)
    {
        int i = 1;      //=1 khi có thêm iteam "Chọn" luôn nằm đầu tiên mà chỉ số index bắt đầu =0                                    
        foreach (var t in return_list_nganh())
        {
            if (_id == t.id.ToString())
                return i;
            i = i + 1;
        }
        return 0;
    }
}