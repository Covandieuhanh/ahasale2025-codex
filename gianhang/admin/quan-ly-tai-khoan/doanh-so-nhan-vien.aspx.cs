using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_quan_ly_menu_Default : System.Web.UI.Page
{
    dbDataContext db = new dbDataContext();
    taikhoan_class tk_cl = new taikhoan_class();
    datetime_class dt_cl = new datetime_class();
    nganh_class nganh_cl = new nganh_class();
    public string user, user_parent;
    public Int64 tongchot, tonglam, tongcong, tongbanthe; nganh_class ng_cl = new nganh_class();
    #region phân trang
    public int stt = 1, current_page = 1, total_page = 1, show = 30;
    List<string> list_id_split;
    #endregion

    private bool HasAnyPermission(params string[] permissionKeys)
    {
        string currentUser = (user ?? "").Trim();
        if (string.IsNullOrEmpty(currentUser))
            return false;

        foreach (string permissionKey in permissionKeys)
        {
            if (!string.IsNullOrEmpty(permissionKey) && bcorn_class.check_quyen(currentUser, permissionKey) == "")
                return true;
        }

        return false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        #region Check quyen theo nganh
        user = (access.User ?? "").Trim();
        user_parent = access.OwnerAccountKey;
        if (HasAnyPermission("q2_6", "n2_6"))
        {
            if (!IsPostBack)
            {
                var list_nganh = (from ob1 in db.nganh_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                                  select new { id = ob1.id, ten = ob1.ten, }
                                   );
                DropDownList5.DataSource = list_nganh;
                DropDownList5.DataTextField = "ten";
                DropDownList5.DataValueField = "id";
                DropDownList5.DataBind();
                DropDownList5.Items.Insert(0, new ListItem("Tất cả", ""));
                if (HasAnyPermission("q2_6"))
                {
                }
                else
                {
                    DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList5.Enabled = false;
                }

                if (Session["index_loc_dsnv"] != null)//lưu lọc theo dịch vụ-sản phẩm
                    DropDownList5.SelectedIndex = int.Parse(Session["index_loc_dsnv"].ToString());
                else
                    Session["index_loc_dsnv"] = DropDownList5.SelectedIndex.ToString();

                //var q = db.bspa_hoadon_chitiet_tables;
                if (Session["current_page_dsnv"] == null)//lưu giữ trang hiện tại
                    Session["current_page_dsnv"] = "1";

                if (Session["index_sapxep_dsnv"] == null)////lưu sắp xếp
                {
                    DropDownList2.SelectedIndex = 5;//mặc định sắp xếp theo tổng doanh số giảm dần
                    Session["index_sapxep_dsnv"] = DropDownList2.SelectedIndex.ToString();
                }
                else
                    DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_dsnv"].ToString());

                if (Session["search_dsnv"] != null)//lưu tìm kiếm
                    txt_search.Text = Session["search_dsnv"].ToString();
                else
                    Session["search_dsnv"] = txt_search.Text;

                if (Session["index_trangthai_dsnv"] != null)//lưu lọc trạng thái
                    DropDownList1.SelectedIndex = int.Parse(Session["index_trangthai_dsnv"].ToString());
                else
                    Session["index_trangthai_dsnv"] = DropDownList1.SelectedValue.ToString();

                if (Session["show_dsnv"] == null)//lưu số dòng mặc định
                {
                    txt_show.Text = "30";
                    Session["show_dsnv"] = txt_show.Text;
                }
                else
                    txt_show.Text = Session["show_dsnv"].ToString();

                if (Session["tungay_dsnv"] == null)
                {
                    txt_tungay.Text = "01/01/2023";
                    Session["tungay_dsnv"] = txt_tungay.Text;
                }
                else
                    txt_tungay.Text = Session["tungay_dsnv"].ToString();

                if (Session["denngay_dsnv"] == null)
                {
                    txt_denngay.Text = DateTime.Now.ToShortDateString();
                    Session["denngay_dsnv"] = txt_denngay.Text;
                }
                else
                    txt_denngay.Text = Session["denngay_dsnv"].ToString();
            }
            main();
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        #endregion

    }

    public Int64 tinhtong_chotsale(string _user)
    {
        var q = db.bspa_hoadon_chitiet_tables.Where(p => p.nguoichot_dvsp == _user && p.id_chinhanh == Session["chinhanh"].ToString() && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_dsnv"].ToString()) && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_dsnv"].ToString()));
        if (q.Count() != 0)
        {
            return q.Sum(p => p.tongtien_chotsale_dvsp).Value;
        }
        return 0;
    }
    public Int64 tinhtong_dichvu(string _user)
    {
        var q = db.bspa_hoadon_chitiet_tables.Where(p => p.nguoilam_dichvu == _user && p.id_chinhanh == Session["chinhanh"].ToString() && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_dsnv"].ToString()) && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_dsnv"].ToString()));
        if (q.Count() != 0)
        {
            return q.Sum(p => p.tongtien_lamdichvu).Value;
        }
        return 0;
    }
    public Int64 tinhtong_banthe(string _user)
    {
        var q = db.thedichvu_tables.Where(p => p.nguoichotsale == _user && p.id_chinhanh == Session["chinhanh"].ToString() && p.ngaytao.Value.Date >= DateTime.Parse(Session["tungay_dsnv"].ToString()) && p.ngaytao.Value.Date <= DateTime.Parse(Session["denngay_dsnv"].ToString()));
        if (q.Count() != 0)
        {
            return q.Sum(p => p.tongtien_chotsale_dvsp).Value;
        }
        return 0;
    }

    public void main()
    {
        //Intersect: lấy ra các phần tử mà cả 2 bên đều có (phần chung)
        #region lấy dữ liệu
        var list_all = (from ob1 in db.taikhoan_table_2023s.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                        select new
                        {
                            taikhoan = ob1.taikhoan,
                            hoten = ob1.hoten,
                            hoten_khongdau = ob1.hoten_khongdau,
                            avt = ob1.anhdaidien,
                            trangthai = ob1.trangthai,
                            ngaytao = ob1.ngaytao,
                            sdt = ob1.dienthoai,
                            email = ob1.email,
                            tongchot = tinhtong_chotsale(ob1.taikhoan),
                            tonglam = tinhtong_dichvu(ob1.taikhoan),
                            tongbanthe = tinhtong_banthe(ob1.taikhoan),
                            tongcong = tinhtong_chotsale(ob1.taikhoan) + tinhtong_dichvu(ob1.taikhoan) + tinhtong_banthe(ob1.taikhoan),
                            id_nganh = ob1.id_nganh,
                            tennganh = nganh_cl.return_name(ob1.id_nganh),
                        });


        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.hoten.ToLower().Contains(_key) || p.hoten_khongdau.ToLower().Contains(_key) || p.taikhoan.ToString() == _key || p.sdt.ToString() == _key || p.email.ToString() == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }
        //xử lý trạng thái

        //lọc dữ liệu
        //if (DropDownList1.SelectedValue.ToString() != "0")
        //{
        switch (DropDownList1.SelectedValue.ToString())
        {
            case ("0"): var list_0 = list_all.Where(p => p.trangthai == "Đang hoạt động").ToList(); list_all = list_all.Intersect(list_0).ToList(); break;
            case ("1"): var list_1 = list_all.Where(p => p.trangthai == "Đã bị khóa").ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
            default: var list_2 = list_all.ToList(); list_all = list_all.Intersect(list_2).ToList(); break;//tất cả
        }
        //}
        if (DropDownList5.SelectedValue.ToString() != "")//ngành
        {
            var list_1 = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList(); list_all = list_all.Intersect(list_1).ToList();
        }

        tongchot = list_all.Sum(p => p.tongchot);
        tonglam = list_all.Sum(p => p.tonglam);
        tongcong = list_all.Sum(p => p.tongcong);
        tongbanthe = list_all.Sum(p => p.tongbanthe);

        //sắp xếp
        switch (Session["index_sapxep_dsnv"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.tongchot).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.tongchot).ToList(); break;
            case ("2"): list_all = list_all.OrderBy(p => p.tonglam).ToList(); break;
            case ("3"): list_all = list_all.OrderByDescending(p => p.tonglam).ToList(); break;
            case ("4"): list_all = list_all.OrderBy(p => p.tongcong).ToList(); break;
            case ("5"): list_all = list_all.OrderByDescending(p => p.tongcong).ToList(); break;
            default: list_all = list_all.OrderByDescending(p => p.tongcong).ToList(); break;
        }
        #endregion

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 30;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_dsnv"].ToString());
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
        var list_split = list_all.Skip(current_page * show - show).Take(show).ToList();
        list_id_split = new List<string>();
        foreach (var t in list_split)
        {
            list_id_split.Add("check_" + t.taikhoan);
        }
        int _s1 = stt + list_split.Count - 1;
        if (list_all.Count() != 0)
            lb_show.Text = "Hiển thị " + stt + "-" + _s1 + " trong số " + list_all.Count().ToString("#,##0") + " mục";
        else
            lb_show.Text = "Hiển thị 0-0 trong số 0";
        Repeater1.DataSource = list_split;
        Repeater1.DataBind();
    }


    #region autopostback
    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        ApplySearchState();
    }
    protected void but_search_Click(object sender, EventArgs e)
    {
        ApplySearchState();
    }
    private void ApplySearchState()
    {
        Session["current_page_dsnv"] = "1";

        main();
    }
    //protected void txt_show_TextChanged(object sender, EventArgs e)
    //{
    //    Session["current_page_dsnv"] = "1";
    //    main();
    //}
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_dsnv"] = int.Parse(Session["current_page_dsnv"].ToString()) - 1;
        if (int.Parse(Session["current_page_dsnv"].ToString()) < 1)
            Session["current_page_dsnv"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_dsnv"] = int.Parse(Session["current_page_dsnv"].ToString()) + 1;
        if (int.Parse(Session["current_page_dsnv"].ToString()) > total_page)
            Session["current_page_dsnv"] = total_page;
        main();
    }
    #endregion


    protected void Button1_Click(object sender, EventArgs e)
    {
        Session["index_trangthai_dsnv"] = DropDownList1.SelectedIndex;
        Session["index_sapxep_dsnv"] = DropDownList2.SelectedIndex;
        Session["current_page_dsnv"] = "1";
        Session["search_dsnv"] = txt_search.Text.Trim();
        Session["show_dsnv"] = txt_show.Text.Trim();
        Session["tungay_dsnv"] = txt_tungay.Text;
        Session["denngay_dsnv"] = txt_denngay.Text;

        Session["index_locnganh_dsnv"] = DropDownList5.SelectedIndex;
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        Session["index_trangthai_dsnv"] = null;
        Session["index_sapxep_dsnv"] = null;
        Session["current_page_dsnv"] = null;
        Session["search_dsnv"] = null;
        Session["show_dsnv"] = null;
        Session["tungay_dsnv"] = null;
        Session["denngay_dsnv"] = null;

        Session["index_loc_ganh_dsnv"] = null;
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/doanh-so-nhan-vien.aspx");
    }

    #region chọn ngày nhanh
    protected void but_homqua_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = DateTime.Now.Date.AddDays(-1).ToString();
        txt_denngay.Text = DateTime.Now.Date.AddDays(-1).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_homnay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = DateTime.Now.Date.ToString();
        txt_denngay.Text = DateTime.Now.Date.ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_tuantruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydautuan().AddDays(-7).ToString();//lấy ngày đầu tuần
        txt_denngay.Text = dt_cl.return_ngaydautuan().AddDays(-1).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_tuannay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydautuan().ToString();//lấy ngày đầu tuần
        txt_denngay.Text = dt_cl.return_ngaycuoituan().ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_thangtruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauthangtruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoithangtruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_thangnay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauthang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoithang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_namtruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydaunamtruoc(DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoinamtruoc(DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_namnay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydaunam(DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoinam(DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_quytruoc_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauquytruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoiquytruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }
    protected void but_quynay_Click(object sender, EventArgs e)
    {
        txt_tungay.Text = dt_cl.return_ngaydauquynay(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoiquynay(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Chọn nhanh thành công.<br/>Hãy nhấn nút BẮT ĐẦU LỌC.", "1000", "warning"), true);
    }


    #endregion

    protected void Button5_Click(object sender, EventArgs e)
    {
        #region lấy dữ liệu
        var list_all = (from ob1 in db.taikhoan_table_2023s.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList()
                        select new
                        {
                            taikhoan = ob1.taikhoan,
                            hoten = ob1.hoten,
                            hoten_khongdau = ob1.hoten_khongdau,
                            avt = ob1.anhdaidien,
                            trangthai = ob1.trangthai,
                            ngaytao = ob1.ngaytao,
                            sdt = ob1.dienthoai,
                            email = ob1.email,
                            tongchot = tinhtong_chotsale(ob1.taikhoan),
                            tonglam = tinhtong_dichvu(ob1.taikhoan),
                            tongbanthe = tinhtong_banthe(ob1.taikhoan),
                            tongcong = tinhtong_chotsale(ob1.taikhoan) + tinhtong_dichvu(ob1.taikhoan) + tinhtong_banthe(ob1.taikhoan),
                            id_nganh = ob1.id_nganh,
                            tennganh = nganh_cl.return_name(ob1.id_nganh),
                        });


        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.hoten.ToLower().Contains(_key) || p.hoten_khongdau.ToLower().Contains(_key) || p.taikhoan.ToString() == _key || p.sdt.ToString() == _key || p.email.ToString() == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }
        //xử lý trạng thái

        //lọc dữ liệu
        //if (DropDownList1.SelectedValue.ToString() != "0")
        //{
        switch (DropDownList1.SelectedValue.ToString())
        {
            case ("0"): var list_0 = list_all.Where(p => p.trangthai == "Đang hoạt động").ToList(); list_all = list_all.Intersect(list_0).ToList(); break;
            case ("1"): var list_1 = list_all.Where(p => p.trangthai == "Đã bị khóa").ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
            default: var list_2 = list_all.ToList(); list_all = list_all.Intersect(list_2).ToList(); break;//tất cả
        }
        //}
        if (DropDownList5.SelectedValue.ToString() != "")//ngành
        {
            var list_1 = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList(); list_all = list_all.Intersect(list_1).ToList();
        }

        tongchot = list_all.Sum(p => p.tongchot);
        tonglam = list_all.Sum(p => p.tonglam);
        tongcong = list_all.Sum(p => p.tongcong);
        tongbanthe = list_all.Sum(p => p.tongbanthe);

        //sắp xếp
        switch (Session["index_sapxep_dsnv"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.tongchot).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.tongchot).ToList(); break;
            case ("2"): list_all = list_all.OrderBy(p => p.tonglam).ToList(); break;
            case ("3"): list_all = list_all.OrderByDescending(p => p.tonglam).ToList(); break;
            case ("4"): list_all = list_all.OrderBy(p => p.tongcong).ToList(); break;
            case ("5"): list_all = list_all.OrderByDescending(p => p.tongcong).ToList(); break;
            default: list_all = list_all.OrderByDescending(p => p.tongcong).ToList(); break;
        }
        #endregion

        if (check_list_excel.Items.Count == 0)
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Không có mục nào được chọn.", "4000", "warning"), true);
        else
        {
            // khởi tạo wb rỗng
            XSSFWorkbook wb = new XSSFWorkbook();

            // Tạo ra 1 sheet
            ISheet sheet = wb.CreateSheet();

            // Bắt đầu ghi lên sheet

            // Tạo row
            var row0 = sheet.CreateRow(0);
            // Merge lại row đầu 3 cột
            row0.CreateCell(0); // tạo ra cell trc khi merge
            CellRangeAddress cellMerge = new CellRangeAddress(0, 0, 0, 2);
            sheet.AddMergedRegion(cellMerge);
            row0.GetCell(0).SetCellValue("Dữ liệu của bạn xuất ngày " + DateTime.Now);

            //đếm xem có bao nhiêu cột đc chọn
            // Ghi tiêu đề cột ở row 1
            var row1 = sheet.CreateRow(1);
            int _socot = 0;
            for (int i = 0; i < check_list_excel.Items.Count; i++)//duyệt hết các phần tử trong list check
            {
                if (check_list_excel.Items[i].Selected)//nếu cột này đc chọn
                {
                    //thì tạo cột tiêu đề
                    row1.CreateCell(_socot).SetCellValue(check_list_excel.Items[i].Text);
                    _socot = _socot + 1;
                }
            }

            // bắt đầu duyệt mảng và ghi tiếp tục
            int rowIndex = 2;
            foreach (var item in list_all)
            {
                // tao row mới
                var newRow = sheet.CreateRow(rowIndex);

                // set giá trị
                int _socot1 = 0;
                for (int i = 0; i < check_list_excel.Items.Count; i++)//duyệt hết các phần tử trong list check
                {
                    if (check_list_excel.Items[i].Selected)//nếu cột này đc chọn
                    {
                        string _tencot = check_list_excel.Items[i].Value;
                        switch (_tencot)
                        {
                            case "taikhoan": newRow.CreateCell(_socot1).SetCellValue(item.taikhoan); break;
                            case "hoten": newRow.CreateCell(_socot1).SetCellValue(item.hoten); break;
                            case "tennganh": newRow.CreateCell(_socot1).SetCellValue(item.tennganh); break;
                            case "trangthai": newRow.CreateCell(_socot1).SetCellValue(item.trangthai); break;
                            case "sdt": newRow.CreateCell(_socot1).SetCellValue(item.sdt); break;
                            case "tongchot": newRow.CreateCell(_socot1).SetCellValue(item.tongchot); break;
                            case "tonglam": newRow.CreateCell(_socot1).SetCellValue(item.tonglam); break;
                            case "tongbanthe": newRow.CreateCell(_socot1).SetCellValue(item.tongbanthe); break;
                            case "tongcong": newRow.CreateCell(_socot1).SetCellValue(item.tongcong); break;
                            default: break;
                        }
                        _socot1 = _socot1 + 1;
                    }
                }

                // tăng index
                rowIndex++;
            }

            //tính tổng
            // Ghi tiêu đề cột ở row 1
            var row3 = sheet.CreateRow(rowIndex);//tạo dòng tiếp theo
            int _socot3 = 0;
            for (int i = 0; i < check_list_excel.Items.Count; i++)//duyệt hết các phần tử trong list check
            {
                if (check_list_excel.Items[i].Selected)//nếu cột này đc chọn
                {
                    //thì ghi dữ liệu
                    string _tencot = check_list_excel.Items[i].Value;
                    switch (_tencot)
                    {
                        case "tongchot": row3.CreateCell(_socot3).SetCellValue(tongchot); break;
                        case "tonglam": row3.CreateCell(_socot3).SetCellValue(tonglam); break;
                        case "tongbanthe": row3.CreateCell(_socot3).SetCellValue(tongbanthe); break;
                        case "tongcong": row3.CreateCell(_socot3).SetCellValue(tongcong); break;
                        default: break;
                    }
                    _socot3 = _socot3 + 1;
                }
            }

            // xong hết thì save file lại
            string _filename = Guid.NewGuid().ToString();
            FileStream fs = new FileStream(Server.MapPath("~/uploads/Files/" + _filename + ".xlsx"), FileMode.CreateNew);
            wb.Write(fs);
            Response.Redirect("/uploads/Files/" + _filename + ".xlsx");

        }
    }
}
