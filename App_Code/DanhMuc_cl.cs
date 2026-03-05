using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Web;
using System.Web.UI.WebControls; // Import namespace này để sử dụng DropDownList và ListItem

public class DanhMuc_cl
{
    //#region CÁCH CŨ, DANH MỤC CON CÓ THỂ THỤT VÀO
    //public string result_listview, idmenu_cha, selected;
    //public void show_menu_dropdownbox()
    //{
    //    foreach (var t in mn_cl.return_list().Where(p => p.id_level == 1 && p.bin == false).OrderBy(p => p.rank))//duyệt hết menu cấp 1 k nằm trong thùng rác
    //    {
    //        get_data_menu(t.id.ToString());
    //    }
    //}
    //public void get_data_menu(string _id_category)//đưa 1 id vào
    //{
    //    if (idmenu_cha == _id_category)
    //        selected = "selected";
    //    else
    //        selected = "";
    //    //gán vào select dropbox        
    //    result_listview = result_listview + "<option " + selected + " class='pl-" + 4 * (mn_cl.return_object(_id_category).id_level - 1) + "' value='" + _id_category + "'>" + mn_cl.return_object(_id_category).name + "</option>";
    //    if (mn_cl.exist_sub(_id_category)) //nếu có menucon
    //    {
    //        foreach (var t in mn_cl.return_list(_id_category).Where(p => p.bin == false).OrderBy(p => p.rank))//thì duyệt hết menu con, chỉ những đứa k nằm trong thùng rác
    //        {
    //            get_data_menu(t.id.ToString()); //thì gọi lại hàm, nếu có id con thì cứ gọi lại
    //        }
    //    }
    //}
    //#endregion
    #region Hiển thị Danh Mục vào DropDownList
    public void Show_DanhMuc(int _CapBatDau, int _CapKetThuc, DropDownList dropdown, bool _bin, string _kyhieu, string _id_loaitru)
    {
        //_CapBatDau: bắt đầu lấy từ cấp mấy, 
        //_CapKetThuc: đến cấp mấy thì kết thúc, nếu cấp kết thúc =0 thì lấy hết
        //dropdown: truyền ID DropDownList muốn buộc dữ liệu vào ở ngoài trang aspx
        //_bin = true: lấy luôn DanhMuc trong thùng rác. False: ngược lại
        //ký hiệu: web hoặc khác để DanhMuc này áp dụng cho nhiều việc chứ k riêng Menu Web
        //id_loaitru: trừ id đó ra, áp dụng lúc chỉnh sửa, id_loaitru=0: k loại trừ
        using (dbDataContext db = new dbDataContext())
        {
            dropdown.Items.Clear();
            // Thêm mục đầu tiên vào DropDownList
            dropdown.Items.Add(new ListItem("Nhấn để chọn", ""));
            var q = from dm in db.DanhMuc_tbs
                    where dm.id_level == _CapBatDau && dm.bin == _bin && dm.kyhieu_danhmuc == _kyhieu
                    select new
                    {
                        id = dm.id,
                        name = dm.name,
                        rank = dm.rank,
                    };
            if (_id_loaitru != "0")//nếu có id loại trừ thì k lấy id đó
                q = q.Where(p => p.id.ToString() != _id_loaitru);
            foreach (var t in q.OrderBy(p => p.rank))
            {
                DeQuy_DanhMuc(db, t.id.ToString(), _CapKetThuc, dropdown, _bin, _CapBatDau, _id_loaitru);//bắt đầu gọi hàm đệ quy với _CapBatDau. VD _CapBatDau =1
            }
        }
    }
    public void DeQuy_DanhMuc(dbDataContext db, string _ID_DanhMuc, int _CapKetThuc, DropDownList dropdown, bool _bin, int _CapHienTai, string _id_loaitru)
    {
        if (_CapHienTai > _CapKetThuc && _CapKetThuc != 0)//nếu cấp hiện tại > cấp kết thúc thì ngưng
            return;//_CapKetThuc khác 0 thì mới kiểm tra điều kiện ngưng, _CapKetThuc= 0 nghĩa là lấy đến cùng


        var q = db.DanhMuc_tbs
              .Where(p => p.id.ToString() == _ID_DanhMuc)
              .Select(p => new
              {
                  p.id,
                  p.name,
                  p.rank
              })
              .FirstOrDefault();
        dropdown.Items.Add(new ListItem(q.name, _ID_DanhMuc));
        var q1 = db.DanhMuc_tbs.Where(p => p.id_parent == _ID_DanhMuc && p.bin == _bin);
        if (_id_loaitru != "0")//nếu có id loại trừ thì k lấy id đó
            q1 = q1.Where(p => p.id.ToString() != _id_loaitru);
        if (q1.Any())//nếu có con thì đệ quy
        {
            foreach (var t in q1.OrderBy(p => p.rank))//thì duyệt hết menu con nếu có
                DeQuy_DanhMuc(db, t.id.ToString(), _CapKetThuc, dropdown, _bin, _CapHienTai + 1, _id_loaitru); //thì gọi lại hàm, nếu có id con thì cứ gọi lại
        }

    }
    #endregion

    #region Hiển thị Cây Danh Mục
    public string Show_CayDanhMuc(int _CapBatDau, int _CapKetThuc, bool _bin, string _kyhieu, string _id_loaitru)
    {
        StringBuilder _kq = new StringBuilder();
        using (dbDataContext db = new dbDataContext())
        {
            var q = from dm in db.DanhMuc_tbs
                    where dm.id_level == _CapBatDau && dm.bin == _bin && dm.kyhieu_danhmuc == _kyhieu
                    select new
                    {
                        id = dm.id,
                        name = dm.name,
                        rank = dm.rank,
                    };
            if (_id_loaitru != "0")//nếu có id loại trừ thì k lấy id đó
                q = q.Where(p => p.id.ToString() != _id_loaitru);
            foreach (var t in q.OrderBy(p => p.rank))
            {
                DeQuy_CayDanhMuc(db, t.id.ToString(), _CapKetThuc, _kq, _bin, _CapBatDau, _id_loaitru);//bắt đầu gọi hàm đệ quy với _CapBatDau. VD _CapBatDau =1
            }
        }
        return _kq.ToString();
    }
    public void DeQuy_CayDanhMuc(dbDataContext db, string _ID_DanhMuc, int _CapKetThuc, StringBuilder _kq, bool _bin, int _CapHienTai, string _id_loaitru)
    {
        if (_CapHienTai > _CapKetThuc && _CapKetThuc != 0)//nếu cấp hiện tại > cấp kết thúc thì ngưng
            return;//_CapKetThuc khác 0 thì mới kiểm tra điều kiện ngưng, _CapKetThuc= 0 nghĩa là lấy đến cùng


        var q = db.DanhMuc_tbs
              .Where(p => p.id.ToString() == _ID_DanhMuc)
              .Select(p => new
              {
                  p.id,
                  p.name,
                  p.rank
              })
              .FirstOrDefault();

        var q1 = db.DanhMuc_tbs.Where(p => p.id_parent == _ID_DanhMuc && p.bin == _bin);
        if (_id_loaitru != "0")//nếu có id loại trừ thì k lấy id đó
            q1 = q1.Where(p => p.id.ToString() != _id_loaitru);
        if (q1.Any())//nếu có con thì đệ quy
        {
            _kq.AppendFormat("<li data-caption='{0}'><ul>", q.name);
            foreach (var t in q1.OrderBy(p => p.rank))//thì duyệt hết menu con nếu có
                DeQuy_CayDanhMuc(db, t.id.ToString(), _CapKetThuc, _kq, _bin, _CapHienTai + 1, _id_loaitru); //thì gọi lại hàm, nếu có id con thì cứ gọi lại
            _kq.Append("</ul></li>");
        }
        else
            _kq.AppendFormat("<li data-caption='{0}'></li>", q.name);

    }
    #endregion

    #region Hiển thị MenuTop
    public string Show_MenuTop_Home(int _CapBatDau, int _CapKetThuc, bool _bin, string _kyhieu, string _id_loaitru)
    {
        StringBuilder _kq = new StringBuilder();
        using (dbDataContext db = new dbDataContext())
        {
            var q = from dm in db.DanhMuc_tbs
                    where dm.id_level == _CapBatDau && dm.bin == _bin && dm.kyhieu_danhmuc == _kyhieu
                    select new
                    {
                        id = dm.id,
                        name = dm.name,
                        rank = dm.rank,
                    };
            if (_id_loaitru != "0")//nếu có id loại trừ thì k lấy id đó
                q = q.Where(p => p.id.ToString() != _id_loaitru);
            foreach (var t in q.OrderBy(p => p.rank))
            {
                DeQuy_MenuTop_Home(db, t.id.ToString(), _CapKetThuc, _kq, _bin, _CapBatDau, _id_loaitru);//bắt đầu gọi hàm đệ quy với _CapBatDau. VD _CapBatDau =1
            }
        }
        return _kq.ToString();
    }
    public void DeQuy_MenuTop_Home(dbDataContext db, string _ID_DanhMuc, int _CapKetThuc, StringBuilder _kq, bool _bin, int _CapHienTai, string _id_loaitru)
    {
        if (_CapHienTai > _CapKetThuc && _CapKetThuc != 0)//nếu cấp hiện tại > cấp kết thúc thì ngưng
            return;//_CapKetThuc khác 0 thì mới kiểm tra điều kiện ngưng, _CapKetThuc= 0 nghĩa là lấy đến cùng


        var q = db.DanhMuc_tbs
              .Where(p => p.id.ToString() == _ID_DanhMuc)
              .Select(p => new
              {
                  p.id,
                  p.name,
                  p.name_en,
                  p.rank,
                  p.url_other
              })
              .FirstOrDefault();

        var q1 = db.DanhMuc_tbs.Where(p => p.id_parent == _ID_DanhMuc && p.bin == _bin && p.id_level<= _CapKetThuc);
        if (_id_loaitru != "0")//nếu có id loại trừ thì k lấy id đó
            q1 = q1.Where(p => p.id.ToString() != _id_loaitru);
        if (q1.Any())//nếu có con thì đệ quy
        {
            _kq.AppendFormat("<li><a style=\"color:#eaecef\" href='#' class='dropdown-toggle marker-light fg-yellow-hover'>{0}</a><ul style='background-color:#1e2329' class='d-menu place-right' data-role='dropdown'>", q.name);
            foreach (var t in q1.OrderBy(p => p.rank))//thì duyệt hết menu con nếu có
                DeQuy_MenuTop_Home(db, t.id.ToString(), _CapKetThuc, _kq, _bin, _CapHienTai + 1, _id_loaitru); //thì gọi lại hàm, nếu có id con thì cứ gọi lại
            _kq.Append("</ul></li>");
        }
        else
        {
            // Kiểm tra giá trị của q.other_url
            string url = string.IsNullOrEmpty(q.url_other) ? $"/{q.name_en}-{q.id}" : q.url_other;
            _kq.AppendFormat("<li><a style=\"color:#eaecef\" class='fg-yellow-hover' href='{0}'>{1}</a></li>", url, q.name);
        }    

    }
    #endregion

    public void change_id_level_of_sub_category(dbDataContext db, string _id, int _id_level)//đưa 1 id vào
    {
        var q = db.DanhMuc_tbs.Where(p => p.id_parent.ToString() == _id); // Lấy tất cả danh mục con
        foreach (var t in q) //duyệt qua hết menu con
        {
            t.id_level = _id_level + 1;
            if (db.DanhMuc_tbs.Any(p => p.id_parent.ToString() == t.id.ToString()))//nếu Menu đang chỉnh sửa có menu con
                change_id_level_of_sub_category(db, t.id.ToString(), _id_level + 1); //thì gọi lại hàm, nếu có id con thì cứ gọi lại   
        }
        db.SubmitChanges();
    }

    //public bool exist_email_old(string _email_old, string _email_new)
    //{
    //    var q = db.taikhoan_table_2023s.Where(p => p.email != _email_old);
    //    foreach (var t in q)
    //    {
    //        if (t.email == _email_new)
    //            return true;
    //    }
    //    return false;
    //}
}
