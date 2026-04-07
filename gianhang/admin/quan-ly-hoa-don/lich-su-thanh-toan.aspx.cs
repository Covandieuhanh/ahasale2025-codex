using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_Default : System.Web.UI.Page
{
    dbDataContext db = new dbDataContext();
    datetime_class dt_cl = new datetime_class(); nganh_class ng_cl = new nganh_class();
    public Int64 tongcong = 0, tienmat = 0, chuyenkhoan = 0, quetthe = 0, vouchergiay = 0, voucherdiem = 0, vidientu = 0;

    public string user, user_parent, tienbangchu;
    #region phân trang
    public int stt = 1, current_page = 1, show = 30, total_page = 1;
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
        //hoadon_class hd_cl = new hoadon_class();
        //hoadon_chitiet_class hdct = new hoadon_chitiet_class();
        //foreach (var t in db.bspa_hoadon_chitiet_tables)
        //{
        //    if (!hd_cl.exist_id(t.id_hoadon, "admin"))
        //        hdct.del(t.id.ToString());
        //}

        GianHangAdminPageGuard_cl.AccessInfo access = GianHangAdminPageGuard_cl.EnsureAccess(this, db, "none");
        if (access == null)
            return;

        #region Check quyen theo nganh
        user = (access.User ?? "").Trim();
        user_parent = access.OwnerAccountKey;
        if (HasAnyPermission("q7_7", "n7_7"))
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
                if (HasAnyPermission("q7_7"))
                {
                }
                else
                {
                    DropDownList5.SelectedIndex = ng_cl.return_index(Session["nganh"].ToString());
                    DropDownList5.Enabled = false;
                }
                if (Session["index_loc_nganh_lstt"] != null)//lưu lọc theo theo toán
                    DropDownList5.SelectedIndex = int.Parse(Session["index_loc_nganh_lstt"].ToString());
                else
                    Session["index_loc_nganh_lstt"] = DropDownList5.SelectedIndex.ToString();

                if (Session["current_page_lstt"] == null)//lưu giữ trang hiện tại
                    Session["current_page_lstt"] = "1";

                if (Session["index_sapxep_lstt"] == null)////lưu sắp xếp
                {
                    DropDownList2.SelectedIndex = 1;//mặc định là giảm dần theo thời gian
                    Session["index_sapxep_lstt"] = DropDownList2.SelectedIndex.ToString();
                }
                else
                    DropDownList2.SelectedIndex = int.Parse(Session["index_sapxep_lstt"].ToString());

                if (Session["search_lstt"] != null)//lưu tìm kiếm
                    txt_search.Text = Session["search_lstt"].ToString();
                else
                    Session["search_lstt"] = txt_search.Text;

                if (Session["show_lstt"] == null)//lưu số dòng mặc định
                {
                    txt_show.Text = "30";
                    Session["show_lstt"] = txt_show.Text;
                }
                else
                    txt_show.Text = Session["show_lstt"].ToString();

                if (Session["tungay_lstt"] == null)
                {
                    txt_tungay.Text = "01/01/2023";
                    Session["tungay_lstt"] = txt_tungay.Text;
                }
                else
                    txt_tungay.Text = Session["tungay_lstt"].ToString();

                if (Session["denngay_lstt"] == null)
                {
                    txt_denngay.Text =DateTime.Now.ToShortDateString();
                    Session["denngay_lstt"] = txt_denngay.Text;
                }
                else
                    txt_denngay.Text = Session["denngay_lstt"].ToString();

                if (Session["index_loc_httt"] != null)//lưu lọc theo Hình thức giao dịch
                    ddl_loc_thanhtoan.SelectedIndex = int.Parse(Session["index_loc_httt"].ToString());
                else
                    Session["index_loc_httt"] = ddl_loc_thanhtoan.SelectedValue.ToString();
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

    public void main()
    {
        //lấy dữ liệu
        var list_all = (from ob1 in db.bspa_lichsu_thanhtoan_tables.Where(p =>p.id_chinhanh == Session["chinhanh"].ToString()&& p.thoigian.Value.Date >= DateTime.Parse(Session["tungay_lstt"].ToString()) && p.thoigian.Value.Date <= DateTime.Parse(Session["denngay_lstt"].ToString())).ToList()
                        join ob2 in db.bspa_hoadon_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on ob1.id_hoadon equals ob2.id.ToString()
                        join ob3 in db.taikhoan_table_2023s.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on ob1.nguoithanhtoan equals ob3.taikhoan
                        select new
                        {
                            id = ob1.id,
                            thoigian = ob1.thoigian,
                            sotien = ob1.sotienthanhtoan,
                            hinhthuc = ob1.hinhthuc_thanhtoan,
                            id_hoadon = ob1.id_hoadon,
                            khachhang = ob2.tenkhachhang,
                            //diachi = ob2.diachi,
                            sdt = ob2.sdt,
                            user_thanhtoan = ob1.nguoithanhtoan,
                            nguoithanhtoan = ob3.hoten,
                            id_nganh=ob1.id_nganh,
                        });

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.khachhang.ToLower().Contains(_key) || p.nguoithanhtoan.ToLower().Contains(_key) || p.sdt.ToLower().Contains(_key) || p.id_hoadon == _key).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }
        //xử lý lọc dữ liệu
        if (ddl_loc_thanhtoan.SelectedValue.ToString() != "0")
        {
            switch (ddl_loc_thanhtoan.SelectedValue.ToString())
            {
                case ("Tiền mặt"): var list_1 = list_all.Where(p => p.hinhthuc == "Tiền mặt").ToList(); list_all = list_all.Intersect(list_1).ToList(); break;
                case ("Chuyển khoản"): var list_2 = list_all.Where(p => p.hinhthuc == "Chuyển khoản").ToList(); list_all = list_all.Intersect(list_2).ToList(); break;
                case ("Quẹt thẻ"): var list_3 = list_all.Where(p => p.hinhthuc == "Quẹt thẻ").ToList(); list_all = list_all.Intersect(list_3).ToList(); break;

                case ("Voucher giấy"): var list_5 = list_all.Where(p => p.hinhthuc == "Voucher giấy").ToList(); list_all = list_all.Intersect(list_5).ToList(); break;
                case ("E-Voucher (điểm)"): var list_4 = list_all.Where(p => p.hinhthuc == "E-Voucher (điểm)").ToList(); list_all = list_all.Intersect(list_4).ToList(); break;
                case ("Ví điện tử"): var list_6 = list_all.Where(p => p.hinhthuc == "Ví điện tử").ToList(); list_all = list_all.Intersect(list_6).ToList(); break;
                default: break;
            }
        }

        if (DropDownList5.SelectedValue.ToString() != "")//ngành
        {
            var list_1 = list_all.Where(p => p.id_nganh == DropDownList5.SelectedValue.ToString()).ToList(); list_all = list_all.Intersect(list_1).ToList();
        }

        tongcong = list_all.Sum(p => p.sotien).Value;
        tienmat = list_all.Where(p => p.hinhthuc == "Tiền mặt").Sum(p => p.sotien).Value;
        chuyenkhoan = list_all.Where(p => p.hinhthuc == "Chuyển khoản").Sum(p => p.sotien).Value;
        quetthe = list_all.Where(p => p.hinhthuc == "Quẹt thẻ").Sum(p => p.sotien).Value;
      
        vouchergiay = list_all.Where(p => p.hinhthuc == "Voucher giấy").Sum(p => p.sotien).Value;
        voucherdiem = list_all.Where(p => p.hinhthuc == "E-Voucher (điểm)").Sum(p => p.sotien).Value;
        vidientu = list_all.Where(p => p.hinhthuc == "Ví điện tử").Sum(p => p.sotien).Value;

        if (tongcong < 0)
            tienbangchu = "Âm " + number_class.number_to_text_unlimit(tongcong.ToString().Replace("-", ""));
        else
            tienbangchu = number_class.number_to_text_unlimit(tongcong.ToString().Replace("-", ""));

        //sắp xếp
        switch (Session["index_sapxep_lstt"].ToString())
        {
            case ("0"): list_all = list_all.OrderBy(p => p.thoigian).ToList(); break;
            case ("1"): list_all = list_all.OrderByDescending(p => p.thoigian).ToList(); break;
            default: list_all = list_all.OrderByDescending(p => p.thoigian).ToList(); break;
        }

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 30;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_lstt"].ToString());
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
            list_id_split.Add("check_" + t.id);
        }
        int _s1 = stt + list_split.Count - 1;
        if (list_all.Count() != 0)
            lb_show.Text = "Hiển thị " + stt + "-" + _s1 + " trong số " + list_all.Count().ToString("#,##0") + " mục";
        else
            lb_show.Text = "Hiển thị 0-0 trong số 0";
        Repeater1.DataSource = list_split;
        Repeater1.DataBind();
    }
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
        Session["current_page_lstt"] = "1";

        main();
    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_lstt"] = int.Parse(Session["current_page_lstt"].ToString()) - 1;
        if (int.Parse(Session["current_page_lstt"].ToString()) < 1)
            Session["current_page_lstt"] = 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_lstt"] = int.Parse(Session["current_page_lstt"].ToString()) + 1;
        if (int.Parse(Session["current_page_lstt"].ToString()) > total_page)
            Session["current_page_lstt"] = total_page;
        main();
    }

    protected void Button1_Click(object sender, EventArgs e)//but lọc
    {
        Session["index_sapxep_lstt"] = DropDownList2.SelectedIndex;
        Session["current_page_lstt"] = "1";
        Session["search_lstt"] = txt_search.Text.Trim();
        Session["show_lstt"] = txt_show.Text.Trim();
        Session["tungay_lstt"] = txt_tungay.Text;
        Session["denngay_lstt"] = txt_denngay.Text;

        Session["index_loc_httt"] = ddl_loc_thanhtoan.SelectedIndex;
        Session["index_loc_nganh_lstt"] = DropDownList5.SelectedIndex.ToString();
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thành công.", "4000", "warning"), true);
    }

    protected void Button2_Click(object sender, EventArgs e)//reset
    {
        reset_ss();
        Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Xử lý thành công.", "4000", "warning");
        Response.Redirect("/gianhang/admin/quan-ly-hoa-don/lich-su-thanh-toan.aspx");
    }
    public void reset_ss()
    {
        Session["index_sapxep_lstt"] = null;
        Session["current_page_lstt"] = null;
        Session["search_lstt"] = null;
        Session["show_lstt"] = null;
        Session["tungay_lstt"] = null;
        Session["denngay_lstt"] = null;

        Session["index_loc_httt"] = null;
        Session["index_loc_nganh_lstt"] = null;
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

    protected void but_xoa_thanhtoan_Click(object sender, ImageClickEventArgs e)
    {
        if (HasAnyPermission("q7_6", "n7_6"))
        {
            int _count = 0;
            for (int i = 0; i < list_id_split.Count; i++)
            {
                if (Request.Form[list_id_split[i]] == "on")
                {
                    string _id = list_id_split[i].Replace("check_", "");
                    var q = db.bspa_lichsu_thanhtoan_tables.Where(p => p.id.ToString() == _id && p.id_chinhanh == Session["chinhanh"].ToString());
                    if (q.Count() != 0)
                    {
                        Int64 _sotien_thanhtoan = q.First().sotienthanhtoan.Value;
                        string _id_hoadon = q.First().id_hoadon;
                        var q_hoadon = db.bspa_hoadon_tables.Where(p => p.id.ToString() == _id_hoadon && p.id_chinhanh == Session["chinhanh"].ToString());
                        bspa_hoadon_table _ob = q_hoadon.First();
                        _ob.sotien_dathanhtoan = _ob.sotien_dathanhtoan - _sotien_thanhtoan;
                        _ob.sotien_conlai = _ob.tongsauchietkhau - _ob.sotien_dathanhtoan;
                        db.SubmitChanges();

                        bspa_lichsu_thanhtoan_table _ob1 = q.First();
                        HoaDonThuChiSync_cl.DeleteForInvoicePayment(db, _ob1.id.ToString(), Session["chinhanh"].ToString());
                        db.bspa_lichsu_thanhtoan_tables.DeleteOnSubmit(_ob1);
                        db.SubmitChanges();
                        _count = _count + 1;
                    }
                }
                if (_count > 0)
                {
                    main();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "warning"), true);
                }
            }
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", ""), true);
    }

    //public double return_value(string _tongsl)
    //{
    //    int _tsl = int.Parse(_tongsl);
    //    return Math.Round(double.Parse(_tsl.ToString()) / double.Parse(max.ToString()) * 100, 1);
    //}
}
