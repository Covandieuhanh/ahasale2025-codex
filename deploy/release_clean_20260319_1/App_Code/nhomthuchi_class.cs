using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for nhomthuchi_class
/// </summary>
public class nhomthuchi_class
{
    dbDataContext db = new dbDataContext();
    public bool exist_id(string _id, string _user_parent)
    {
        var q = db.bspa_nhomthuchi_tables.Where(p => p.id.ToString() == _id && p.user_parent == _user_parent && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
            return true;
        return false;
    }
    public bspa_nhomthuchi_table return_object(string _id)
    {
        var q = db.bspa_nhomthuchi_tables.Single(p => p.id.ToString() == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        return q;
    }
    public void del(string _id)
    {
        //chuyển các dịch vụ thuộc nhóm sắp xóa về KHÔNG THUỘC NHÓM NÀO
        var q = db.bspa_thuchi_tables.Where(p => p.id_nhomthuchi == _id && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
        {
            foreach (var t in q)
            {
                bspa_thuchi_table _ob1 = t;
                _ob1.id_nhomthuchi = "";
                db.SubmitChanges();
            }
        }

        //xóa nhóm 
        bspa_nhomthuchi_table _ob = return_object(_id);
        db.bspa_nhomthuchi_tables.DeleteOnSubmit(_ob);
        db.SubmitChanges();
    }

    public List<bspa_nhomthuchi_table> return_list(string _user_parent)
    {
        var q = db.bspa_nhomthuchi_tables.Where(p=>p.user_parent== _user_parent && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).ToList();
        return q;
    }
    public int return_index(string _id,string _user_parent)
    {
        int i = 1;      //=1 khi có thêm iteam "Chọn" luôn nằm đầu tiên mà chỉ số index bắt đầu =0                                    
        foreach (var t in return_list(_user_parent).OrderBy(p => p.tennhom))
        {
            if (_id == t.id.ToString())
                return i;
            i = i + 1;
        }
        return 0;
    }
    public int return_index_nganh(string _id, string _user_parent)
    {
        int i = 1;      //=1 khi có thêm iteam "Chọn" luôn nằm đầu tiên mà chỉ số index bắt đầu =0                                    
        foreach (var t in db.bspa_nhomthuchi_tables.Where(p => p.id_nganh == System.Web.HttpContext.Current.Session["nganh"].ToString() && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).OrderBy(p => p.tennhom))
        {
            if (_id == t.id.ToString())
                return i;
            i = i + 1;
        }
        return 0;
    }
}