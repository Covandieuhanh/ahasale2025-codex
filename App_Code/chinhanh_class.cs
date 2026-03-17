using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for chinhanh_class
/// </summary>
public class chinhanh_class
{
    dbDataContext db = new dbDataContext();
    public bool exist_id(string _id)
    {
        var q = db.chinhanh_tables.Where(p => p.id.ToString() == _id);
        if (q.Count() != 0)
            return true;
        return false;
    }
    public string return_name(string _id)
    {
        var q = db.chinhanh_tables.Where(p => p.id.ToString() == _id);
        if (q.Count() != 0)
            return q.First().ten;
        return "";
    }
    public chinhanh_table return_object(string _id)
    {
        var q = db.chinhanh_tables.Single(p => p.id.ToString() == _id);
        return q;
    }
    public List<chinhanh_table> return_list()
    {
        var q = db.chinhanh_tables.ToList();
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
    public Int64 return_max_id()
    {
        return db.chinhanh_tables.Max(p => p.id);
    }
}