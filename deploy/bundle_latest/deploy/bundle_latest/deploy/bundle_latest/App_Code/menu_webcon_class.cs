using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for menu_webcon_class
/// </summary>
public class menu_webcon_class
{
    dbDataContext db = new dbDataContext();
    post_class po_cl = new post_class();

    private string CurrentChiNhanhId
    {
        get { return AhaShineContext_cl.ResolveChiNhanhId(); }
    }

    #region kiểm tra tồn tại
    public bool exist_id(string _id)
    {
        var q = db.web_menu_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == CurrentChiNhanhId);
        if (q.Count() != 0)
            return true;
        return false;
    }
    public bool exist_name(string _name)
    {
        var q = db.web_menu_tables.Where(p => p.name.ToLower() == _name.ToLower() && p.id_chinhanh == CurrentChiNhanhId);
        if (q.Count() != 0)
            return true;
        return false;
    }
    #endregion

    #region trả về đối tượng duy nhất dựa vào id
    public web_menu_table return_object(string _id)
    {
        var q = db.web_menu_tables.Single(p => p.id.ToString() == _id && p.id_chinhanh == CurrentChiNhanhId);
        return q;
    }
    #endregion    
    public bool check_sub(string _id1, string _id2) //kiểm tra xem _id2 có phải là con cháu của _id1 hay k
    {
        if (exist_sub(_id1)) //nếu có menucon
        {
            foreach (var t in return_list(_id1))//thì duyệt hết menu con
            {
                if (t.id.ToString() == _id2)
                    return true;
                check_sub(t.id.ToString(), _id2); //thì gọi lại hàm, nếu có id con thì cứ gọi lại
            }
        }
        return false;
    }

    #region trả về toàn bộ danh sách
    public List<web_menu_table> return_list()
    {
        var q = db.web_menu_tables.Where(p => p.id_chinhanh == CurrentChiNhanhId).ToList();
        return q;
    }
    #endregion

    #region trả về toàn bộ danh sách của 1 id
    public List<web_menu_table> return_list(string _id_parent)
    {
        var q = db.web_menu_tables.Where(p => p.id_parent.ToString() == _id_parent && p.id_chinhanh == CurrentChiNhanhId).ToList();
        return q;
    }
    #endregion

    #region kiểm tra xem id này có menu con hay k
    public bool exist_sub(string _id)
    {
        var q = db.web_menu_tables.Where(p => p.id_parent.ToString() == _id && p.bin == false && p.id_chinhanh == CurrentChiNhanhId);
        if (q.Count() != 0)
            return true;
        return false;
    }
    #endregion

    #region xóa 
    public void del(string _id)
    {
        if (check_menu_khi_xoa(_id) == true)
        {
            po_cl.del_list_idmenu(_id);//xóa toàn bộ bài viết của menu này        
            if (exist_sub(_id))//nếu id này có menu con
            {
                foreach (var t in return_list(_id))//thì duyệt hết menu con
                {
                    del(t.id.ToString());//gọi lại hàm để xóa id này                  
                }
            }
            del_only(_id);//xóa id này
        }
    }
    public void del_only(string _id)
    {
        if (check_menu_khi_xoa(_id) == true)
        {
            web_menu_table _tb_oj = return_object(_id);
            file_folder_class.del_file(_tb_oj.image);
            db.web_menu_tables.DeleteOnSubmit(_tb_oj);
            db.SubmitChanges();
        }
    }
    public void remove_bin(string _id, bool _stt)
    {
        if (check_menu_khi_xoa(_id) == true)
        {
            po_cl.change_status_bin_list_idmenu(_id, _stt);//đưa toàn bộ bài viết của menu này vào thùng rác      
            if (exist_sub(_id))//nếu id này có menu con
            {
                foreach (var t in return_list(_id))//thì duyệt hết menu con
                {
                    remove_bin(t.id.ToString(), _stt);//gọi lại hàm               
                }
            }
            remove_bin_only(_id, _stt);//đưa id này vô thùng rác
        }
    }
    public void remove_bin_only(string _id, bool _stt)//thay đổi trạng thái true false của thùng rác, true là đã xóa, false là chưa xóa
    {
        if (check_menu_khi_xoa(_id) == true)
        {
            web_menu_table _tb_oj = return_object(_id);
            _tb_oj.bin = _stt;
            db.SubmitChanges();
        }

    }
    #endregion

    public bool check_menu_khi_xoa(string _id)
    {
        if (_id == "550" || _id == "551")//nếu là danh mục dv và sp thì k xóa
            return false;
        return true;
    }

    #region hàm cập nhật rank 
    public void update_rank(string _id, int _rank)
    {
        var q = db.web_menu_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == CurrentChiNhanhId);
        web_menu_table _mn_tb = q.First();
        _mn_tb.rank = _rank;
        db.SubmitChanges();
    }
    #endregion    

    #region hàm đếm toàn bộ số lượng menu con cháu của 1 menu bất kỳ
    public int dem;
    public int return_soluong_menucon(string _id)
    {
        dem = 0;
        foreach (var t in return_list().Where(p => p.id_parent == _id && p.bin == false).ToList())
        {
            get_data_tree(t.id.ToString());
        }
        return dem;
    }
    public void get_data_tree(string _id)//đưa 1 id vào
    {
        dem = dem + 1;
        if (exist_sub(_id)) //nếu có menucon
        {
            foreach (var t in return_list().Where(p => p.id_parent == _id && p.bin == false))//thì duyệt hết con
            {
                get_data_tree(t.id.ToString()); //thì gọi lại hàm, nếu có id con thì cứ gọi lại
            }
        }
    }
    #endregion

    #region trả về tên mmenu cha của 1 id
    public string return_ten_menu_cha(string _id)
    {
        string _id_parent = return_object(_id).id_parent;
        if (_id_parent != "0")
            return return_object(_id_parent).name;
        return "";
    }
    #endregion
    #region trả về idmn cấp 1 của 1 id bất kỳ
    public string idmn_cap1;
    public string get_idmn_cap1(string _idmn)//hàm lấy id cấp 1
    {
        int _id_level;
        idmn_cap1 = _idmn;
        var q1 = db.web_menu_tables.Where(p => p.id.ToString() == _idmn && p.id_chinhanh == CurrentChiNhanhId);
        if (q1.Count() != 0)
        {
            _id_level = q1.First().id_level.Value;
            if (_id_level == 1)
                return q1.First().id.ToString();
            else
            {
                for (int i = _id_level; i > 1; i--)
                {
                    var q2 = db.web_menu_tables.Where(p => p.id.ToString() == idmn_cap1 && p.id_chinhanh == CurrentChiNhanhId);
                    idmn_cap1 = q2.First().id_parent;
                }
                return idmn_cap1;//lấy đc id cấp 1
            }

        }
        return "";
    }
    #endregion

    public bool exist_sub_category(string _id)
    {
        var q = db.web_menu_tables.Where(p => p.id_parent.ToString() == _id && p.id_chinhanh == CurrentChiNhanhId);
        if (q.Count() != 0)
            return true;
        return false;
    }
    public List<web_menu_table> return_list_sub(string _id_parent)//trả về danh sách menu con của 1 id
    {
        var q = db.web_menu_tables.Where(p => p.id_parent.ToString() == _id_parent && p.id_chinhanh == CurrentChiNhanhId).ToList();
        return q;
    }
}
