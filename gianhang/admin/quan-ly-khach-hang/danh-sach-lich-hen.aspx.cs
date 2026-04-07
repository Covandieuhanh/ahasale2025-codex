using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_khach_hang_danh_sach_lich_hen : System.Web.UI.Page
{
    public class lichhen_vanhanh_item
    {
        public string id_khachhang { get; set; }
        public int so_hoadon_lienket { get; set; }
        public int so_thedv_lienket { get; set; }
        public string id_hoadon_moi_nhat { get; set; }
        public string nhan_vanhanh { get; set; }
        public string nhan_vanhanh_css { get; set; }
        public string tenkhachhang { get; set; }
        public string sdt { get; set; }
        public string dichvu { get; set; }
        public string nhanvien_thuchien { get; set; }
        public string trangthai { get; set; }
        public DateTime? ngaydat { get; set; }
        public string id_nganh_goiy { get; set; }
    }

    dbDataContext db = new dbDataContext();
    datetime_class dt_cl = new datetime_class();
    taikhoan_class tk_cl = new taikhoan_class();
    public string user, user_parent, show_add = "none";
    public Dictionary<string, lichhen_vanhanh_item> map_lichhen_vanhanh = new Dictionary<string, lichhen_vanhanh_item>();
    public lichhen_dieuphoi_tongquan dieu_phoi_homnay = new lichhen_dieuphoi_tongquan();

    #region phân trang
    public int stt = 1, current_page = 1, show = 30, total_page = 1;
    List<string> list_id_split;
    #endregion

    private bool EnsureActionAccess(string requiredPermission)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, requiredPermission);
        if (access == null)
            return false;

        user = (access.User ?? "").Trim();
        user_parent = access.OwnerAccountKey;
        return true;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "q10_1");
        if (access == null)
            return;
        user = (access.User ?? "").Trim();
        user_parent = access.OwnerAccountKey;

        if (!IsPostBack)
        {
            var list_dichvu = (from ob1 in db.web_post_tables.Where(p => p.phanloai == "ctdv" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                               select new { tendv = ob1.name, iddv = ob1.id, }
                      );
            ddl_dichvu.DataSource = list_dichvu;
            ddl_dichvu.DataTextField = "tendv";
            ddl_dichvu.DataValueField = "iddv";
            ddl_dichvu.DataBind();
            ddl_dichvu.Items.Insert(0, new ListItem("Chọn dịch vụ", ""));
            datlich_class.bind_gio_phut(ddl_giobatdau, ddl_phutbatdau, DateTime.Now);

            var list_nhanvien = (from ob1 in db.taikhoan_table_2023s.Where(p => p.trangthai == "Đang hoạt động" && p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                 select new { username = ob1.taikhoan, tennhanvien = ob1.hoten, }
                      );
            ddl_nhanvien.DataSource = list_nhanvien;
            ddl_nhanvien.DataTextField = "tennhanvien";
            ddl_nhanvien.DataValueField = "username";
            ddl_nhanvien.DataBind();
            ddl_nhanvien.Items.Insert(0, new ListItem("Chọn nhân viên", ""));

            var q = db.bspa_datlich_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString() && p.ngaydat.HasValue);
            txt_ngaydat.Text = datlich_class.return_ngay_text(DateTime.Now);
            txt_nguon.Text = "Trực tiếp";

            if (Session["current_page_lichhen"] == null)//lưu giữ trang hiện tại
                Session["current_page_lichhen"] = "1";

            if (Session["index_sapxep_lichhen"] == null)////lưu sắp xếp
            {
                DropDownList2.SelectedIndex = 1;//mặc định là giảm dần theo thời gian
                Session["index_sapxep_lichhen"] = DropDownList2.SelectedIndex.ToString();
            }
            else
                DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_lichhen"].ToString());

            if (Session["search_lichhen"] != null)//lưu tìm kiếm
                txt_search.Text = Session["search_lichhen"].ToString();
            else
                Session["search_lichhen"] = txt_search.Text;

            if (Session["show_lichhen"] == null)//lưu số dòng mặc định
            {
                txt_show.Text = "30";
                Session["show_lichhen"] = txt_show.Text;
            }
            else
                txt_show.Text = Session["show_lichhen"].ToString();

            if (Session["tungay_lichhen"] == null)
            {
                txt_tungay.Text = q.Count() != 0 ? datlich_class.return_ngay_text(q.Min(p => p.ngaydat.Value)) : datlich_class.return_ngay_text(DateTime.Now);
                Session["tungay_lichhen"] = txt_tungay.Text;
            }
            else
                txt_tungay.Text = Session["tungay_lichhen"].ToString();

            if (Session["denngay_lichhen"] == null)
            {
                txt_denngay.Text = q.Count() != 0 ? datlich_class.return_ngay_text(q.Max(p => p.ngaydat.Value)) : datlich_class.return_ngay_text(DateTime.Now);
                Session["denngay_lichhen"] = txt_denngay.Text;
            }
            else
                txt_denngay.Text = Session["denngay_lichhen"].ToString();

            if (Session["index_loc_trangthai_lichhen"] != null)//lưu lọc theo trạng thái
                DropDownList1.SelectedIndex = int.Parse(Session["index_loc_trangthai_lichhen"].ToString());
            else
                Session["index_loc_trangthai_lichhen"] = DropDownList1.SelectedValue.ToString();

            if (!string.IsNullOrWhiteSpace(Request.QueryString["sdt"]))
            {
                string _sdt_qs = HttpUtility.UrlDecode(Request.QueryString["sdt"].ToString());
                txt_sdt.Text = datlich_class.chuanhoa_sdt(_sdt_qs);
            }

            if (!string.IsNullOrWhiteSpace(Request.QueryString["tenkh"]))
            {
                string _tenkh_qs = HttpUtility.UrlDecode(Request.QueryString["tenkh"].ToString());
                txt_tenkhachhang.Text = _tenkh_qs;
            }
        }
        main();

        if (!string.IsNullOrWhiteSpace(Request.QueryString["q"]))//khi ng dùng nhấn vào nút tạo hóa đơn từ menutop --> tạo nhanh
        {
            string _q = Request.QueryString["q"].ToString().Trim();
            if (_q == "add")
                show_add = "block";
        }
    }


    public void main()
    {
        dieu_phoi_homnay = lichhen_dieuphoi_class.tai_tongquan_homnay(db, Session["chinhanh"].ToString());

        DateTime _tungay;
        if (datlich_class.try_parse_ngay(Session["tungay_lichhen"].ToString(), out _tungay) == false)
            _tungay = DateTime.Now.Date;

        DateTime _denngay;
        if (datlich_class.try_parse_ngay(Session["denngay_lichhen"].ToString(), out _denngay) == false)
            _denngay = DateTime.Now.Date;

        if (_tungay > _denngay)
        {
            DateTime _temp = _tungay;
            _tungay = _denngay;
            _denngay = _temp;
        }

        DateTime _denngay_cuoi = _denngay.Date.AddDays(1);
        var query = db.bspa_datlich_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString() && p.ngaydat.HasValue && p.ngaydat.Value >= _tungay.Date && p.ngaydat.Value < _denngay_cuoi);

        string _key = txt_search.Text.Trim();
        if (_key != "")
            query = query.Where(p => (p.tenkhachhang != null && p.tenkhachhang.Contains(_key)) || (p.sdt != null && p.sdt.Contains(_key)));

        switch (DropDownList1.SelectedValue.ToString())
        {
            case "1": query = query.Where(p => p.trangthai == datlich_class.trangthai_chua_xacnhan); break;
            case "2": query = query.Where(p => p.trangthai == datlich_class.trangthai_da_xacnhan); break;
            case "3": query = query.Where(p => p.trangthai == datlich_class.trangthai_khong_den); break;
            case "4": query = query.Where(p => p.trangthai == datlich_class.trangthai_da_den); break;
            case "5": query = query.Where(p => p.trangthai == datlich_class.trangthai_da_huy); break;
        }

        switch (Session["index_sapxep_lichhen"].ToString())
        {
            case "0": query = query.OrderBy(p => p.ngaydat); break;
            case "1": query = query.OrderByDescending(p => p.ngaydat); break;
            case "2": query = query.OrderBy(p => p.ngaytao); break;
            case "3": query = query.OrderByDescending(p => p.ngaytao); break;
            default: query = query.OrderByDescending(p => p.ngaydat); break;
        }

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 30;
        txt_show.Text = show.ToString();

        int _tongso = query.Count();
        total_page = number_of_page_class.return_total_page(_tongso, show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_lichhen"].ToString());
        if (current_page > total_page)
            current_page = total_page;
        if (current_page >= total_page)
            but_xemtiep.Enabled = false;
        else
            but_xemtiep.Enabled = true;
        if (current_page == 1)
            but_quaylai.Enabled = false;
        else
            but_quaylai.Enabled = true;

        //main
        stt = (show * current_page) - show + 1;
        var list_split_db = query.Skip(current_page * show - show).Take(show).ToList();
        nap_du_lieu_vanhanh_lich(list_split_db);
        var list_split = (from ob1 in list_split_db
                          select new
                          {
                              id = ob1.id,
                              ngaydat = ob1.ngaydat,
                              tenkhachhang = ob1.tenkhachhang,
                              sdt = ob1.sdt,
                              nguoitao = string.IsNullOrWhiteSpace(tk_cl.return_hoten(ob1.nguoitao)) ? ob1.nguoitao : tk_cl.return_hoten(ob1.nguoitao),
                              dichvu = ob1.dichvu,
                              tendv = ob1.tendichvu_taithoidiemnay,
                              nhanvien_thuchien = tk_cl.exist_user_of_userparent(ob1.nhanvien_thuchien, user_parent) == false ? ob1.nhanvien_thuchien : "<div><a class='fg-black' href='/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + ob1.nhanvien_thuchien + "'>" + tk_cl.return_object(ob1.nhanvien_thuchien).hoten + "</a></div>",
                              ghichu = ob1.ghichu,
                              trangthai = ob1.trangthai,
                              ngaytao = ob1.ngaytao,
                              nguongoc = ob1.nguongoc,
                          }).ToList();
        list_id_split = new List<string>();
        foreach (var t in list_split)
        {
            list_id_split.Add("check_" + t.id);
        }
        int _s1 = stt + list_split.Count - 1;
        if (_tongso != 0)
            lb_show.Text = "Hiển thị " + stt + "-" + _s1 + " trong số " + _tongso.ToString("#,##0") + " mục";
        else
            lb_show.Text = "Hiển thị 0-0 trong số 0";
        Repeater1.DataSource = list_split;
        Repeater1.DataBind();
    }

    public string return_ngaygio_ngan(DateTime? _value)
    {
        if (_value.HasValue == false)
            return "";

        return _value.Value.ToString("HH:mm");
    }

    private bool co_quyen_sua_lich_nhanh()
    {
        return bcorn_class.check_quyen(user, "q10_3") == "";
    }

    private bool co_quyen_tao_hoadon_nhanh()
    {
        return bcorn_class.check_quyen(user, "q7_2") == "" || bcorn_class.check_quyen(user, "n7_2") == "";
    }

    private lichhen_vanhanh_item lay_lich_vanhanh(string _id_lich)
    {
        if (string.IsNullOrWhiteSpace(_id_lich) || map_lichhen_vanhanh.ContainsKey(_id_lich) == false)
            return null;

        return map_lichhen_vanhanh[_id_lich];
    }

    private void nap_du_lieu_vanhanh_lich(List<bspa_datlich_table> _list_lich)
    {
        map_lichhen_vanhanh = new Dictionary<string, lichhen_vanhanh_item>();
        if (_list_lich == null || _list_lich.Count == 0)
            return;

        Dictionary<string, string> _map_khachhang = new Dictionary<string, string>();
        Dictionary<string, string> _map_nganh_dichvu = new Dictionary<string, string>();
        List<string> _list_sdt = _list_lich
            .Select(p => datlich_class.chuanhoa_sdt(p.sdt))
            .Where(p => string.IsNullOrWhiteSpace(p) == false)
            .Distinct()
            .ToList();
        List<string> _list_id_dichvu = _list_lich
            .Select(p => (p.dichvu ?? "").Trim())
            .Where(p => string.IsNullOrWhiteSpace(p) == false)
            .Distinct()
            .ToList();

        if (_list_sdt.Count != 0)
        {
            var q_khach = db.bspa_data_khachhang_tables
                .Where(p => p.id_chinhanh == Session["chinhanh"].ToString() && _list_sdt.Contains(p.sdt))
                .OrderByDescending(p => p.ngaytao)
                .ToList();
            foreach (bspa_data_khachhang_table _kh in q_khach)
            {
                if (_map_khachhang.ContainsKey(_kh.sdt) == false)
                    _map_khachhang.Add(_kh.sdt, _kh.id.ToString());
            }
        }

        if (_list_id_dichvu.Count != 0)
        {
            var q_dichvu = db.web_post_tables
                .Where(p => p.id_chinhanh == Session["chinhanh"].ToString() && _list_id_dichvu.Contains(p.id.ToString()))
                .Select(p => new
                {
                    id = p.id.ToString(),
                    id_nganh = p.id_nganh,
                })
                .ToList();
            foreach (var _dv in q_dichvu)
            {
                if (_map_nganh_dichvu.ContainsKey(_dv.id) == false)
                    _map_nganh_dichvu.Add(_dv.id, _dv.id_nganh);
            }
        }

        foreach (bspa_datlich_table _item in _list_lich)
        {
            List<string> _list_id_hoadon = datlich_lienket_class.lay_ds_id_hoadon_tu_ghichu(_item.ghichu);
            List<string> _list_id_thedv = datlich_lienket_class.lay_ds_id_thedv_tu_ghichu(_item.ghichu);
            string _sdt_chuan = datlich_class.chuanhoa_sdt(_item.sdt);
            bool _co_hoso = _map_khachhang.ContainsKey(_sdt_chuan);

            lichhen_vanhanh_item _vanhanh = new lichhen_vanhanh_item();
            _vanhanh.id_khachhang = _co_hoso ? _map_khachhang[_sdt_chuan] : "";
            _vanhanh.so_hoadon_lienket = _list_id_hoadon.Count;
            _vanhanh.so_thedv_lienket = _list_id_thedv.Count;
            _vanhanh.id_hoadon_moi_nhat = _list_id_hoadon.Count == 0 ? "" : _list_id_hoadon[_list_id_hoadon.Count - 1];
            _vanhanh.nhan_vanhanh = return_nhan_vanhanh(_item.trangthai, _vanhanh.so_hoadon_lienket, _vanhanh.so_thedv_lienket, _co_hoso);
            _vanhanh.nhan_vanhanh_css = return_css_nhan_vanhanh(_item.trangthai, _vanhanh.so_hoadon_lienket, _vanhanh.so_thedv_lienket, _co_hoso);
            _vanhanh.tenkhachhang = (_item.tenkhachhang ?? "").Trim();
            _vanhanh.sdt = datlich_class.chuanhoa_sdt(_item.sdt);
            _vanhanh.dichvu = (_item.dichvu ?? "").Trim();
            _vanhanh.nhanvien_thuchien = (_item.nhanvien_thuchien ?? "").Trim();
            _vanhanh.trangthai = (_item.trangthai ?? "").Trim();
            _vanhanh.ngaydat = _item.ngaydat;
            _vanhanh.id_nganh_goiy = _map_nganh_dichvu.ContainsKey(_vanhanh.dichvu) ? _map_nganh_dichvu[_vanhanh.dichvu] : "";
            map_lichhen_vanhanh[_item.id.ToString()] = _vanhanh;
        }
    }

    private string return_nhan_vanhanh(string _trangthai, int _so_hoadon, int _so_thedv, bool _co_hoso)
    {
        if (_co_hoso == false)
            return "Chưa có hồ sơ";

        switch ((_trangthai ?? "").Trim())
        {
            case "Chưa xác nhận": return "Cần xác nhận";
            case "Đã xác nhận":
                if (_so_hoadon > 0 || _so_thedv > 0)
                    return "Đang xử lý";
                return "Chờ check-in";
            case "Đã đến":
                if (_so_hoadon > 0 || _so_thedv > 0)
                    return "Đã ghi nhận giao dịch";
                return "Đã đến, chưa chốt";
            case "Không đến": return "No-show";
            case "Đã hủy": return "Đã hủy";
            default: return "Đang theo dõi";
        }
    }

    private string return_css_nhan_vanhanh(string _trangthai, int _so_hoadon, int _so_thedv, bool _co_hoso)
    {
        if (_co_hoso == false)
            return "bg-red";

        switch ((_trangthai ?? "").Trim())
        {
            case "Chưa xác nhận": return "bg-cyan";
            case "Đã xác nhận":
                if (_so_hoadon > 0 || _so_thedv > 0)
                    return "bg-green";
                return "bg-orange";
            case "Đã đến":
                if (_so_hoadon > 0 || _so_thedv > 0)
                    return "bg-magenta";
                return "bg-orange";
            case "Không đến": return "bg-orange";
            case "Đã hủy": return "bg-red";
            default: return "bg-gray";
        }
    }

    public bool co_hoso_tu_lich(string _id_lich)
    {
        if (map_lichhen_vanhanh.ContainsKey(_id_lich) == false)
            return false;

        return string.IsNullOrWhiteSpace(map_lichhen_vanhanh[_id_lich].id_khachhang) == false;
    }

    public string return_url_khachhang_tu_lich(string _id_lich)
    {
        if (co_hoso_tu_lich(_id_lich) == false)
            return "";

        string _id_khachhang = map_lichhen_vanhanh[_id_lich].id_khachhang;
        return "/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=" + HttpUtility.UrlEncode(_id_khachhang) + "&id_datlich=" + HttpUtility.UrlEncode(_id_lich);
    }

    public int return_so_hoadon_lienket_tu_lich(string _id_lich)
    {
        if (map_lichhen_vanhanh.ContainsKey(_id_lich) == false)
            return 0;

        return map_lichhen_vanhanh[_id_lich].so_hoadon_lienket;
    }

    public int return_so_thedv_lienket_tu_lich(string _id_lich)
    {
        if (map_lichhen_vanhanh.ContainsKey(_id_lich) == false)
            return 0;

        return map_lichhen_vanhanh[_id_lich].so_thedv_lienket;
    }

    public string return_url_hoadon_lienket_tu_lich(string _id_lich)
    {
        if (map_lichhen_vanhanh.ContainsKey(_id_lich) == false)
            return "";

        string _id_hoadon = map_lichhen_vanhanh[_id_lich].id_hoadon_moi_nhat;
        if (string.IsNullOrWhiteSpace(_id_hoadon))
            return "";

        return "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=" + HttpUtility.UrlEncode(_id_hoadon) + "&id_datlich=" + HttpUtility.UrlEncode(_id_lich);
    }

    public string return_url_thedv_tu_lich(string _id_lich)
    {
        if (co_hoso_tu_lich(_id_lich) == false)
            return "";

        return "/gianhang/admin/quan-ly-khach-hang/chi-tiet.aspx?id=" + HttpUtility.UrlEncode(map_lichhen_vanhanh[_id_lich].id_khachhang) + "&act=thedv&id_datlich=" + HttpUtility.UrlEncode(_id_lich);
    }

    public string return_nhan_vanhanh_tu_lich(string _id_lich)
    {
        if (map_lichhen_vanhanh.ContainsKey(_id_lich) == false)
            return "Đang theo dõi";

        return map_lichhen_vanhanh[_id_lich].nhan_vanhanh;
    }

    public string return_css_nhan_vanhanh_tu_lich(string _id_lich)
    {
        if (map_lichhen_vanhanh.ContainsKey(_id_lich) == false)
            return "bg-gray";

        return map_lichhen_vanhanh[_id_lich].nhan_vanhanh_css;
    }

    public string return_url_chinhsua_lich(string _id_lich)
    {
        if (string.IsNullOrWhiteSpace(_id_lich))
            return "/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx";

        return "/gianhang/admin/quan-ly-khach-hang/sua-lich-hen.aspx?id=" + HttpUtility.UrlEncode(_id_lich);
    }

    public bool hien_hanhdong_xacnhan_nhanh(string _id_lich)
    {
        lichhen_vanhanh_item _item = lay_lich_vanhanh(_id_lich);
        return _item != null && _item.trangthai == datlich_class.trangthai_chua_xacnhan && co_quyen_sua_lich_nhanh();
    }

    public bool co_the_xacnhan_nhanh(string _id_lich)
    {
        lichhen_vanhanh_item _item = lay_lich_vanhanh(_id_lich);
        if (_item == null || _item.trangthai != datlich_class.trangthai_chua_xacnhan)
            return false;

        return string.IsNullOrWhiteSpace(_item.dichvu) == false && string.IsNullOrWhiteSpace(_item.nhanvien_thuchien) == false;
    }

    public string return_nhan_xacnhan_nhanh(string _id_lich)
    {
        lichhen_vanhanh_item _item = lay_lich_vanhanh(_id_lich);
        if (_item == null)
            return "Xác nhận";

        if (string.IsNullOrWhiteSpace(_item.dichvu))
            return "Chọn DV";
        if (string.IsNullOrWhiteSpace(_item.nhanvien_thuchien))
            return "Gán NV";

        return "Xác nhận";
    }

    public bool hien_hanhdong_da_den_nhanh(string _id_lich)
    {
        lichhen_vanhanh_item _item = lay_lich_vanhanh(_id_lich);
        return _item != null && _item.trangthai == datlich_class.trangthai_da_xacnhan && co_quyen_sua_lich_nhanh();
    }

    public bool co_the_da_den_nhanh(string _id_lich)
    {
        lichhen_vanhanh_item _item = lay_lich_vanhanh(_id_lich);
        if (_item == null || _item.trangthai != datlich_class.trangthai_da_xacnhan || _item.ngaydat.HasValue == false)
            return false;

        return string.IsNullOrWhiteSpace(_item.dichvu) == false
            && string.IsNullOrWhiteSpace(_item.nhanvien_thuchien) == false
            && _item.ngaydat.Value <= DateTime.Now;
    }

    public string return_nhan_da_den_nhanh(string _id_lich)
    {
        lichhen_vanhanh_item _item = lay_lich_vanhanh(_id_lich);
        if (_item == null)
            return "Đã đến";

        if (string.IsNullOrWhiteSpace(_item.dichvu))
            return "Chọn DV";
        if (string.IsNullOrWhiteSpace(_item.nhanvien_thuchien))
            return "Gán NV";
        if (_item.ngaydat.HasValue && _item.ngaydat.Value > DateTime.Now)
            return "Chờ tới giờ";

        return "Đã đến";
    }

    public string return_url_hoa_don_nhanh(string _id_lich)
    {
        lichhen_vanhanh_item _item = lay_lich_vanhanh(_id_lich);
        if (_item == null)
            return "";

        string _url_hoadon_lienket = return_url_hoadon_lienket_tu_lich(_id_lich);
        if (_url_hoadon_lienket != "")
            return _url_hoadon_lienket;

        if (co_quyen_tao_hoadon_nhanh() == false)
            return "";

        return "/gianhang/admin/gianhang/tao-giao-dich.aspx?return_url="
            + HttpUtility.UrlEncode(Request.RawUrl ?? "/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx")
            + "&sdt=" + HttpUtility.UrlEncode(_item.sdt)
            + "&tenkh=" + HttpUtility.UrlEncode(_item.tenkhachhang)
            + "&idnganh=" + HttpUtility.UrlEncode(_item.id_nganh_goiy)
            + "&id_datlich=" + HttpUtility.UrlEncode(_id_lich);
    }

    public string return_nhan_hoa_don_nhanh(string _id_lich)
    {
        return return_so_hoadon_lienket_tu_lich(_id_lich) > 0 ? "Mở HĐ" : "Tạo GD";
    }

    public bool hien_hanhdong_hoa_don_nhanh(string _id_lich)
    {
        return return_url_hoa_don_nhanh(_id_lich) != "";
    }

    public bool hien_hanhdong_thedv_nhanh(string _id_lich)
    {
        return return_url_thedv_nhanh(_id_lich) != "";
    }

    public string return_url_thedv_nhanh(string _id_lich)
    {
        if (co_hoso_tu_lich(_id_lich) == false)
            return "";

        return return_url_thedv_tu_lich(_id_lich);
    }

    private datlich_input tao_input_capnhat_nhanh(bspa_datlich_table _lich, string _trangthai_moi)
    {
        datlich_input _input = new datlich_input();
        _input.tenkhachhang = (_lich.tenkhachhang ?? "").Trim();
        _input.sdt = datlich_class.chuanhoa_sdt(_lich.sdt);
        _input.ngaydat = _lich.ngaydat ?? DateTime.Now;
        _input.dichvu = (_lich.dichvu ?? "").Trim();
        _input.nhanvien_thuchien = (_lich.nhanvien_thuchien ?? "").Trim();
        _input.ghichu = (_lich.ghichu ?? "").Trim();
        _input.trangthai = _trangthai_moi;
        _input.nguongoc = (_lich.nguongoc ?? "").Trim();
        return _input;
    }

    private void capnhat_trangthai_nhanh(string _id_lich, string _trangthai_moi, string _thongbao_thanhcong)
    {
        if (!EnsureActionAccess("q10_3") || co_quyen_sua_lich_nhanh() == false)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
            return;
        }

        long _id_long;
        if (long.TryParse(_id_lich, out _id_long) == false)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lịch hẹn không hợp lệ.", "4000", "warning"), true);
            return;
        }

        var q = db.bspa_datlich_tables.Where(p => p.id == _id_long && p.id_chinhanh == Session["chinhanh"].ToString());
        if (q.Count() == 0)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không tìm thấy lịch hẹn.", "4000", "warning"), true);
            return;
        }

        bspa_datlich_table _ob = q.First();
        if (_ob.ngaydat.HasValue == false)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lịch hẹn chưa có thời gian hợp lệ.", "4000", "warning"), true);
            return;
        }

        datlich_input _input = tao_input_capnhat_nhanh(_ob, _trangthai_moi);
        string _loi_vanhanh = datlich_class.kiemtra_quy_tac_van_hanh(db, _input, Session["chinhanh"].ToString(), _id_long, true);
        if (_loi_vanhanh != "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", _loi_vanhanh, "4000", "warning"), true);
            return;
        }

        string _nhatky = khachhang_nhatky_class.tao_noidung_capnhat_lich(db, _ob, _input, _ob.id.ToString(), Session["chinhanh"].ToString());
        datlich_class.gan_du_lieu_vao_lich(db, _ob, _input, _ob.nguoitao, Session["chinhanh"].ToString(), true);
        db.SubmitChanges();

        if (_nhatky != "")
        {
            khachhang_nhatky_class.ghi_su_kien(db, _ob.sdt, Session["chinhanh"].ToString(), user, _nhatky, DateTime.Now);
            db.SubmitChanges();
        }

        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", _thongbao_thanhcong, "2500", "success"), true);
    }

    protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string _id_lich = (e.CommandArgument ?? "").ToString().Trim();
        if (_id_lich == "")
            return;

        switch ((e.CommandName ?? "").Trim())
        {
            case "quick_confirm":
                capnhat_trangthai_nhanh(_id_lich, datlich_class.trangthai_da_xacnhan, "Xác nhận lịch hẹn thành công.");
                break;
            case "quick_arrived":
                capnhat_trangthai_nhanh(_id_lich, datlich_class.trangthai_da_den, "Đã ghi nhận khách đến.");
                break;
        }
    }

    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        ApplySearchState();
    }
    protected void but_search_Click(object sender, EventArgs e)
    {
        ApplySearchState();
    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_lichhen"] = int.Parse(Session["current_page_lichhen"].ToString()) - 1;
        if (int.Parse(Session["current_page_lichhen"].ToString()) < 1)
            Session["current_page_lichhen"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_lichhen"] = int.Parse(Session["current_page_lichhen"].ToString()) + 1;
        if (int.Parse(Session["current_page_lichhen"].ToString()) > total_page)
            Session["current_page_lichhen"] = total_page;
        main();
    }

    //autocomplete sđt
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> SearchCustomers(string prefixText, int count)
    {
        dbDataContext db1 = new dbDataContext();
        return db1.bspa_data_khachhang_tables.Where(p => p.sdt.Contains(prefixText) && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).Select(p => p.sdt).ToList();
    }
    protected void txt_sdt_TextChanged(object sender, EventArgs e)
    {
        string _sdt = datlich_class.chuanhoa_sdt(txt_sdt.Text);
        txt_sdt.Text = _sdt;
        if (_sdt != "")
        {
            var q1 = db.bspa_data_khachhang_tables.Where(p => p.sdt == _sdt && p.id_chinhanh == Session["chinhanh"].ToString()).OrderByDescending(p => p.ngaytao);
            if (q1.Count() != 0)
            {
                txt_tenkhachhang.Text = q1.First().tenkhachhang;
                //txt_diachi.Text = q1.First().diachi;
            }

        }
    }
    //autocomplete tên kh
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> SearchCustomers1(string prefixText, int count)
    {
        dbDataContext db1 = new dbDataContext();
        return db1.bspa_data_khachhang_tables.Where(p => p.tenkhachhang.Contains(prefixText) && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).Select(p => p.tenkhachhang).ToList();
    }
    protected void txt_tenkhachhang_TextChanged(object sender, EventArgs e)
    {
        string _tenkh = txt_tenkhachhang.Text.Trim();
        if (_tenkh != "")
        {
            var q1 = db.bspa_data_khachhang_tables.Where(p => p.tenkhachhang == _tenkh && p.id_chinhanh == Session["chinhanh"].ToString()).OrderByDescending(p => p.ngaytao);
            if (q1.Count() != 0)
            {
                txt_sdt.Text = q1.First().sdt;
                //txt_diachi.Text = q1.First().diachi;
            }

        }
    }

    private void ApplySearchState()
    {
        Session["search_lichhen"] = txt_search.Text.Trim();
        Session["current_page_lichhen"] = "1";
        main();
    }

    #region chọn ngày nhanh
    protected void but_homqua_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = datlich_class.return_ngay_text(DateTime.Now.Date.AddDays(-1));
        txt_denngay.Text = datlich_class.return_ngay_text(DateTime.Now.Date.AddDays(-1));
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_homnay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = datlich_class.return_ngay_text(DateTime.Now.Date);
        txt_denngay.Text = datlich_class.return_ngay_text(DateTime.Now.Date);
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_tuantruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaydautuan().AddDays(-7));
        txt_denngay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaydautuan().AddDays(-1));
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_tuannay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaydautuan());
        txt_denngay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaycuoituan());
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_thangtruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaydauthangtruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()));
        txt_denngay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaycuoithangtruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()));
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_thangnay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaydauthang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()));
        txt_denngay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaycuoithang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()));
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_namtruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaydaunamtruoc(DateTime.Now.Year.ToString()));
        txt_denngay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaycuoinamtruoc(DateTime.Now.Year.ToString()));
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_namnay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaydaunam(DateTime.Now.Year.ToString()));
        txt_denngay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaycuoinam(DateTime.Now.Year.ToString()));
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_quytruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaydauquytruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()));
        txt_denngay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaycuoiquytruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()));
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_quynay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaydauquynay(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()));
        txt_denngay.Text = datlich_class.return_ngay_text(dt_cl.return_ngaycuoiquynay(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()));
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }


    #endregion


    public void reset_ss()
    {
        Session["index_sapxep_lichhen"] = null;
        Session["current_page_lichhen"] = null;
        Session["search_lichhen"] = null;
        Session["show_lichhen"] = null;
        Session["tungay_lichhen"] = null;
        Session["denngay_lichhen"] = null;

        Session["index_loc_trangthai_lichhen"] = null;
    }
    protected void Button2_Click(object sender, EventArgs e)//reset
    {
        reset_ss();
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx");
    }

    protected void Button1_Click(object sender, EventArgs e)//but lọc
    {
        DateTime _tungay;
        DateTime _denngay;
        if (datlich_class.try_parse_ngay(txt_tungay.Text, out _tungay) == false || datlich_class.try_parse_ngay(txt_denngay.Text, out _denngay) == false)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Khoảng thời gian lọc không hợp lệ.", "4000", "warning"), true);
            return;
        }

        if (_tungay > _denngay)
        {
            DateTime _temp = _tungay;
            _tungay = _denngay;
            _denngay = _temp;
        }

        Session["index_sapxep_lichhen"] = DropDownList2.SelectedIndex;
        Session["current_page_lichhen"] = "1";
        Session["search_lichhen"] = txt_search.Text.Trim();
        Session["show_lichhen"] = txt_show.Text.Trim();

        Session["tungay_lichhen"] = datlich_class.return_ngay_text(_tungay);
        Session["denngay_lichhen"] = datlich_class.return_ngay_text(_denngay);

        Session["index_loc_trangthai_lichhen"] = DropDownList1.SelectedIndex;

        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }
    protected void Button3_Click(object sender, EventArgs e)//thêm
    {
        if (EnsureActionAccess("q10_2"))
        {
            datlich_validate_result _kq = datlich_class.chuanhoa_du_lieu(
                txt_tenkhachhang.Text,
                txt_sdt.Text,
                txt_ngaydat.Text,
                ddl_giobatdau.SelectedValue,
                ddl_phutbatdau.SelectedValue,
                ddl_dichvu.SelectedValue,
                ddl_nhanvien.SelectedValue,
                txt_ghichu.Text,
                ddl_trangthai.SelectedValue,
                txt_nguon.Text,
                "Trực tiếp",
                true
            );

            if (_kq.hop_le == false)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", _kq.thongbao, "4000", "warning"), true);
            else
            {
                string _loi_vanhanh = datlich_class.kiemtra_quy_tac_van_hanh(db, _kq.dulieu, Session["chinhanh"].ToString(), null, true);
                if (_loi_vanhanh != "")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", _loi_vanhanh, "4000", "warning"), true);
                    return;
                }

                bspa_datlich_table _ob = new bspa_datlich_table();
                datlich_class.gan_du_lieu_vao_lich(db, _ob, _kq.dulieu, user, Session["chinhanh"].ToString(), false);
                db.bspa_datlich_tables.InsertOnSubmit(_ob);
                db.SubmitChanges();
                khachhang_nhatky_class.ghi_su_kien(
                    db,
                    _ob.sdt,
                    Session["chinhanh"].ToString(),
                    user,
                    khachhang_nhatky_class.tao_noidung_tao_lich(_ob.id.ToString(), _kq.dulieu, _ob.tendichvu_taithoidiemnay),
                    _ob.ngaytao
                );
                db.SubmitChanges();
                reset_ss();
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Đặt lịch hẹn thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx");
            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }
    protected void Button4_Click(object sender, EventArgs e)
    {
        if (EnsureActionAccess("q10_4"))
        {
            int _count = 0;
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    var q = db.bspa_datlich_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (q.Count() != 0)
                    {
                        bspa_datlich_table _ob = q.First();
                        db.bspa_datlich_tables.DeleteOnSubmit(_ob);
                        db.SubmitChanges();
                        _count = _count + 1;
                    }
                }
            }
            if (_count > 0)
            {
                main();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công.", "4000", "warning"), true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }

    //autocomplete dịch vụ
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> SearchCustomers2(string prefixText, int count)
    {
        dbDataContext db1 = new dbDataContext();
        return db1.bspa_datlich_tables.Where(p => p.nguongoc.Contains(prefixText) && p.id_chinhanh == System.Web.HttpContext.Current.Session["chinhanh"].ToString()).Select(p => p.nguongoc).Distinct().ToList();
    }
}
