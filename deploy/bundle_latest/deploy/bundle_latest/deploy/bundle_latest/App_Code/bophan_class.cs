using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for bophan_class
/// </summary>
public class bophan_class
{
    dbDataContext db = new dbDataContext();
    public bool exist_id(string _id, string _user_parent)
    {
        var q = db.bspa_bophan_tables.Where(p => p.id.ToString() == _id && p.user_parent == _user_parent);
        if (q.Count() != 0)
            return true;
        return false;
    }
    public bspa_bophan_table return_object(string _id, string _user_parent)
    {
        var q = db.bspa_bophan_tables.Single(p => p.id.ToString() == _id && p.user_parent == _user_parent);
        return q;
    }
    public void del(string _id, string _user_parent)
    {
        //chuyển các nhân viên thuộc nhóm sắp xóa về KHÔNG THUỘC    bộ phận nào
        var q = db.taikhoan_table_2023s.Where(p => p.id_bophan == _id && p.user_parent == _user_parent && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString());
        if (q.Count() != 0)
        {
            foreach (var t in q)
            {
                taikhoan_table_2023 _ob1 = t;
                _ob1.id_bophan = "";
                db.SubmitChanges();
            }
        }

        //xóa bộ phận
        bspa_bophan_table _ob = return_object(_id, _user_parent);
        db.bspa_bophan_tables.DeleteOnSubmit(_ob);
        db.SubmitChanges();
    }
    public List<bspa_bophan_table> return_list(string _user_parent)
    {
        var q = db.bspa_bophan_tables.Where(p=>p.user_parent== _user_parent).ToList();
        return q;
    }
    public int return_index(string _id,string _user_parent)
    {
        int i = 1;      //=1 khi có thêm iteam "Chọn" luôn nằm đầu tiên mà chỉ số index bắt đầu =0                                    
        foreach (var t in return_list(_user_parent).OrderBy(p => p.tenbophan))
        {
            if (_id == t.id.ToString())
                return i;
            i = i + 1;
        }
        return 0;
    }
}