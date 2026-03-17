using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for post_webcon_class
/// </summary>
public class post_webcon_class
{
    dbDataContext db = new dbDataContext();

    private string CurrentChiNhanhId
    {
        get { return AhaShineContext_cl.ResolveChiNhanhId(); }
    }

    private static string ResolveCurrentChiNhanhId()
    {
        return AhaShineContext_cl.ResolveChiNhanhId();
    }

    private string CurrentNganhId
    {
        get { return (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["nganh"] != null) ? HttpContext.Current.Session["nganh"].ToString() : ""; }
    }
    #region kiểm tra tồn tại
    public bool exist_id(string _id)
    {
        var q = db.web_post_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == CurrentChiNhanhId);
        if (q.Count() != 0)
            return true;
        return false;
    }
    #endregion
    #region trả về đối tượng duy nhất dựa vào id
    public web_post_table return_object(string _id)
    {
        var q = db.web_post_tables.Single(p => p.id.ToString() == _id && p.id_chinhanh == CurrentChiNhanhId);
        return q;
    }
    #endregion
    #region trả về toàn bộ danh sách
    public List<web_post_table> return_list()
    {
        var q = db.web_post_tables.Where(p => p.id_chinhanh == CurrentChiNhanhId).ToList();
        return q;
    }
    #endregion
    #region trả về toàn bộ danh sách của 1 id
    public List<web_post_table> return_list(string _idmn)
    {
        var q = db.web_post_tables.Where(p => p.id_category.ToString() == _idmn && p.id_chinhanh == CurrentChiNhanhId).ToList();
        return q;
    }
    #endregion   
    public void del(string _id)
    {
        var q = db.web_post_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == CurrentChiNhanhId);
        if (q.Count() != 0)
        {
            web_post_table _po_tb = q.First();
            file_folder_class.del_file(_po_tb.image);
            db.web_post_tables.DeleteOnSubmit(_po_tb);
            db.SubmitChanges();
        }
    }

    public void del_list_idmenu(string _idmn)//xóa toàn bộ bài viết của 1 menu
    {
        foreach (var t in return_list(_idmn))
        {
            del(t.id.ToString());
        }
    }

    public void change_status_bin(string _id, bool _stt)//thay đổi trạng thái true false của thùng rác, true là đã xóa, false là chưa xóa
    {
        web_post_table _tb_oj = return_object(_id);
        _tb_oj.bin = _stt;
        db.SubmitChanges();
    }
    public void change_status_bin_list_idmenu(string _idmn, bool _stt)//đưa toàn bộ bài viết của 1 menu vào thùng rác
    {
        foreach (var t in return_list(_idmn))
        {
            change_status_bin(t.id.ToString(), _stt);
        }
    }
    public List<web_post_table> return_list_of_idmn(string _idmn)//trả về danh sách bài viết của 1 menu
    {
        var q = db.web_post_tables.Where(p => p.id_category == _idmn && p.id_chinhanh == CurrentChiNhanhId).ToList();
        return q;
    }
    public bool check_id_in_bin(string _id) //kiểm tra id này có nằm trong thùng rác hay không
    {
        var q = db.web_post_tables.Where(p => p.id.ToString() == _id && p.bin == true && p.id_chinhanh == CurrentChiNhanhId);
        if (q.Count() != 0)
            return true;
        return false;
    }
    public string return_id(string _name)
    {
        var q = db.web_post_tables.Where(p => p.name == _name.Trim() && p.id_chinhanh == CurrentChiNhanhId);
        if (q.Count() != 0)
            return q.First().id.ToString();
        return "";
    }
    public int return_index_dv(string _id)
    {
        int i = 1;      //=1 khi có thêm iteam "Chọn" luôn nằm đầu tiên mà chỉ số index bắt đầu =0                                    
        foreach (var t in db.web_post_tables.Where(p => p.phanloai == "ctdv" && p.id_chinhanh == CurrentChiNhanhId && p.bin == false))
        {
            if (_id == t.id.ToString())
                return i;
            i = i + 1;
        }
        return 0;
    }
    public int return_index_dv_nganh(string _id)
    {
        int i = 1;      //=1 khi có thêm iteam "Chọn" luôn nằm đầu tiên mà chỉ số index bắt đầu =0                                    
        foreach (var t in db.web_post_tables.Where(p => p.phanloai == "ctdv" && p.bin == false && p.id_nganh == CurrentNganhId && p.id_chinhanh == CurrentChiNhanhId))
        {
            if (_id == t.id.ToString())
                return i;
            i = i + 1;
        }
        return 0;
    }

    public static int dem_soluong_dv()
    {
        dbDataContext db = new dbDataContext();
        string chiNhanhId = ResolveCurrentChiNhanhId();
        return db.web_post_tables.Where(p => p.phanloai == "ctdv" && p.bin == false && p.id_chinhanh == chiNhanhId).Count();
    }
    public static int dem_soluong_sp()
    {
        dbDataContext db = new dbDataContext();
        string chiNhanhId = ResolveCurrentChiNhanhId();
        return db.web_post_tables.Where(p => p.phanloai == "ctsp" && p.bin == false && p.id_chinhanh == chiNhanhId).Count();
    }

    #region công trừ sp tồn kho, hàm cũ
    //public bool check_sl_ton_sanpham(string _id, int _slxuat)
    //{
    //    var q = db.web_post_tables.Where(p => p.phanloai == "ctsp" && p.bin == false && p.id.ToString() == _id);
    //    if (q.Count() != 0)
    //    {
    //        int _sl_ton = q.First().soluong_ton_sanpham.Value;
    //        if (_sl_ton < _slxuat)
    //            return false;//hàng k đủ xuất
    //    }
    //    return true;//còn hàng, cho phép xuất
    //}
    //public void tanggiam_soluong_sanpham(string _tanggiam, string _id,int _sl) //hàm cũ, áp dụng khi trừ sl trực tiếp trong cột sl của bv sản phẩm
    //{
    //    var q = db.web_post_tables.Where(p => p.phanloai == "ctsp" && p.bin == false && p.id.ToString() == _id);
    //    if (q.Count() != 0)
    //    {
    //        web_post_table _ob = q.First();
    //        if (_tanggiam == "giam")
    //            _ob.soluong_ton_sanpham = _ob.soluong_ton_sanpham - _sl;
    //        if (_tanggiam == "tang")
    //            _ob.soluong_ton_sanpham = _ob.soluong_ton_sanpham + _sl;
    //        db.SubmitChanges();
    //    }
    //}
    #endregion


    public bool check_sl_ton_sanpham(string _id, int _slxuat)
    {
        var q = db.donnhaphang_chitiet_tables.Where(p => p.id_dvsp.ToString() == _id && p.sl_conlai > 0 && p.id_chinhanh == CurrentChiNhanhId);
        if (q.Count() != 0)
        {
            int _sl_ton = q.Sum(p => p.sl_conlai).Value;
            if (_sl_ton < _slxuat)
                return false;//hàng k đủ xuất
            else
                return true;//còn hàng, cho xuất
        }
        else//k có hàng
            return false;
    }
    public void giam_soluong_sanpham(string _id, int _sl)
    {
        //lấy ra danh sách các lô hàng còn sản phẩm. Lô nào nhập trước thì xếp trước
        var q = db.donnhaphang_chitiet_tables.Where(p => p.id_dvsp.ToString() == _id && p.sl_conlai > 0 && p.id_chinhanh == CurrentChiNhanhId).OrderBy(p => p.ngaytao);
        if (q.Count() != 0)
        {//QUY TÁC: nếu có đủ sl xuất thì lô nào còn trước xuất trước, đến đây đã đc kiểm tra sl tồn hợp lệ
            int _daxuat = 0;//để biết đã xuất đc bao nhiêu
            int _sl_xuat_conlai = _sl;//để biết cần phải xuất bn nữa

            foreach (var t in q)
            {
                donnhaphang_chitiet_table _ob = t;
                if (_ob.sl_conlai >= _sl_xuat_conlai)//nếu sl tồn của lô này đủ để xuất
                {
                    _ob.sl_daban = _ob.sl_daban + _sl_xuat_conlai;//update sl đã bán = sl đã bán trước đó + sl mới
                    _ob.sl_conlai = _ob.soluong - _ob.sl_daban;//update sl còn lại
                    db.SubmitChanges();
                    break;//đủ để xuất thì ngắt vòng lặp.
                }
                else//nếu lô này k đủ, thì còn nhiu xuất nhiu
                {
                    _daxuat = _daxuat + _ob.sl_conlai.Value;
                    _sl_xuat_conlai = _sl - _daxuat;
                    _ob.sl_daban = _ob.sl_daban + _ob.sl_conlai;//update sl đã bán = sl đã bán trước đó + sl mới
                    _ob.sl_conlai = 0;//update sl còn lại
                    db.SubmitChanges();
                }
            }


        }
    }
    public void tang_soluong_sanpham(string _id, int _sl)
    {
        //lấy ra danh sách các lô hàng hết sản phẩm. Lô nào vừa hết xếp trước
        var q = db.donnhaphang_chitiet_tables.Where(p => p.id_dvsp.ToString() == _id && p.id_chinhanh == CurrentChiNhanhId).OrderByDescending(p => p.ngaytao);
        var q1 = q.Where(p => p.soluong.Value != p.sl_daban.Value);
        if (q.Count() != 0)
        {
            int _danhap = 0;//để biết đã xuất đc bao nhiêu
            int _sl_nhap_conlai = _sl;//để biết cần phải xuất bn nữa

            foreach (var t in q)
            {
                donnhaphang_chitiet_table _ob = t;
                if (_ob.sl_daban >= _sl_nhap_conlai)//nếu sl của lô này đủ chứa sp cần nhập vào
                {
                    _ob.sl_daban = _ob.sl_daban - _sl_nhap_conlai;//update sl đã bán
                    _ob.sl_conlai = _ob.soluong - _ob.sl_daban;//update sl còn lại
                    db.SubmitChanges();
                    break;//đủ để xuất thì ngắt vòng lặp.
                }
                else//nếu sl lô này k đủ chứa, thì còn bn chổ thì nhét vào hết
                {
                    _danhap = _danhap + _ob.sl_daban.Value;
                    _sl_nhap_conlai = _sl - _danhap;
                    _ob.sl_daban = 0;
                    _ob.sl_conlai = _ob.soluong - _ob.sl_daban;//update sl còn lại
                    db.SubmitChanges();
                }
            }


        }
    }
}
