using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_lich_su_giao_dich : System.Web.UI.Page
{
    private const decimal VND_PER_A = 1000m;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", true); //check tài khoản, có chuyển hướng. YÊU CẦU ĐĂNG NHẬP.

            string _tk = PortalRequest_cl.GetCurrentAccountEncrypted();

            if (!string.IsNullOrEmpty(_tk))//nếu có khách đăng nhập
            {
                ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
            }
            else
            { }

            set_dulieu_macdinh();
            BindBalance();
            show_main();
        }
    }

    private void BindBalance()
    {
        using (dbDataContext db = new dbDataContext())
        {
            string tk = (ViewState["taikhoan"] ?? "").ToString();
            EnsureShopOnlySync(db, tk);
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            decimal balance = q != null ? (q.HoSo_TieuDung_ShopOnly ?? 0m) : 0m;
            lb_balance_td.Text = balance.ToString("#,##0.##");
            lb_balance_td_vnd.Text = (balance * VND_PER_A).ToString("#,##0");
        }
    }

    private void EnsureShopOnlySync(dbDataContext db, string tk)
    {
        if (db == null || string.IsNullOrWhiteSpace(tk)) return;

        bool changed = false;
        var result = ShopOnlyLedger_cl.RecalculateBalances(db, tk, true);
        if (result.Updated) changed = true;

        string sessionKey = "shoponly_sync_" + tk;
        if (Session[sessionKey] == null)
        {
            int added = ShopOnlyLedger_cl.BackfillSellerCredits(db, tk);
            if (added > 0) changed = true;
            Session[sessionKey] = true;
        }

        if (changed)
            db.SubmitChanges();
    }

    public void set_dulieu_macdinh()
    {
        ViewState["current_page_lsgd_home"] = "1";
    }

    #region main - phân trang - tìm kiếm
    public void show_main()
    {
        using (dbDataContext db = new dbDataContext())
        {
            #region lấy dữ liệu
            var list_all = (from ob1 in db.LichSu_DongA_tbs.Where(p =>
          p.taikhoan == ViewState["taikhoan"].ToString()
          && p.LoaiHoSo_Vi == 1
          && (
              (p.ghichu != null && p.ghichu.Contains("|SHOPONLY|"))
              || (p.id_donhang != null && p.id_donhang != "")
          )
      )
                            join ob2 in db.taikhoan_tbs on ob1.taikhoan equals ob2.taikhoan into Group1
                            from ob2 in Group1.DefaultIfEmpty()
                            select new
                            {
                                ob1.id,
                                ob1.ngay,
                                ob1.dongA,
                                ob1.CongTru,
                                ob1.ghichu,
                                ob1.id_donhang,
                            }).AsQueryable();




            string _key = txt_timkiem.Text.Trim();
            if (!string.IsNullOrEmpty(_key))
            {
                list_all = list_all.Where(p => (p.id_donhang != null && p.id_donhang.Contains(_key))
                                            || (p.ghichu != null && p.ghichu.Contains(_key)));
            }
            else
            {
                string _key1 = txt_timkiem1.Text.Trim();
                if (!string.IsNullOrEmpty(_key1))
                {
                    list_all = list_all.Where(p => (p.id_donhang != null && p.id_donhang.Contains(_key1))
                                                || (p.ghichu != null && p.ghichu.Contains(_key1)));
                }
            }

            //sắp xếp
            list_all = list_all.OrderByDescending(p => p.ngay);
            int _Tong_Record = list_all.Count();
            #endregion

            #region phân trang OK, k sửa
            int show = 30; if (show <= 0) show = 30;

            int current_page = int.Parse(ViewState["current_page_lsgd_home"].ToString());
            int total_page = number_of_page_class.return_total_page(_Tong_Record, show);

            if (current_page < 1) current_page = 1;
            else if (current_page > total_page) current_page = total_page;

            ViewState["total_page"] = total_page;

            if (current_page >= total_page)
            {
                but_xemtiep.Enabled = false;
                but_xemtiep1.Enabled = false;
            }
            else
            {
                but_xemtiep.Enabled = true;
                but_xemtiep1.Enabled = true;
            }

            if (current_page == 1)
            {
                but_quaylai.Enabled = false;
                but_quaylai1.Enabled = false;
            }
            else
            {
                but_quaylai.Enabled = true;
                but_quaylai1.Enabled = true;
            }

            var list_split = list_all.Skip(current_page * show - show).Take(show);

            int stt = (show * current_page) - show + 1;
            int _s1 = stt + list_split.Count() - 1;

            if (_Tong_Record != 0) lb_show.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");
            else lb_show.Text = "0-0/0";

            lb_show_md.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");
            #endregion

            Repeater1.DataSource = list_split;
            Repeater1.DataBind();
            ph_empty_tieudung.Visible = list_split.Count() == 0;
        }
    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_lsgd_home"] = int.Parse(ViewState["current_page_lsgd_home"].ToString()) - 1;
        show_main();
    }

    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_lsgd_home"] = int.Parse(ViewState["current_page_lsgd_home"].ToString()) + 1;
        show_main();
    }

    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_lsgd_home"] = 1;
        show_main();
    }
    #endregion

    #region chi tiết đơn hàng
    protected void LinkButton1_Click(object sender, EventArgs e)//show form chi tiết đơn hàng
    {
        using (dbDataContext db = new dbDataContext())
        {
            LinkButton button = (LinkButton)sender;
            string _iddh = button.CommandArgument;

            var q = db.DonHang_tbs.FirstOrDefault(p => p.id.ToString() == _iddh);
            if (q != null)
            {
                ViewState["iddh"] = _iddh;

                var q_dh = db.DonHang_tbs.FirstOrDefault(p => p.id.ToString() == _iddh);
                string _nguoimua = q_dh.nguoimua;
                string _nguoiban = q_dh.nguoiban;

                var q_ban = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _nguoiban);
                Label100.Text = q_ban.hoten; Label101.Text = q_ban.dienthoai; Label102.Text = q_ban.diachi;

                var q_mua = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _nguoimua);
                Label103.Text = q_mua.hoten; Label104.Text = q_mua.dienthoai; Label105.Text = q_mua.diachi;

                var q_ct = from ob1 in db.DonHang_ChiTiet_tbs.Where(p => p.id_donhang == _iddh)
                           join ob2 in db.BaiViet_tbs on ob1.idsp equals ob2.id.ToString() into SanPhamGroup
                           from ob2 in SanPhamGroup.DefaultIfEmpty()
                           select new
                           {
                               ob1.id,
                               ob1.id_donhang,
                               name = ob2 != null ? ob2.name : "",
                               ob2.name_en,
                               image = ob2 != null ? ob2.image : "",
                               ob1.giaban,
                               ob1.soluong,
                               ob1.thanhtien,
                           };

                Repeater2.DataSource = q_ct;
                Repeater2.DataBind();

                pn_chitiet.Visible = !pn_chitiet.Visible;
                up_chitiet.Update();
            }
        }
    }

    protected void but_close_form_chitiet_Click(object sender, EventArgs e)
    {
        Repeater2.DataSource = null;
        Repeater2.DataBind();
        pn_chitiet.Visible = !pn_chitiet.Visible;
    }
    #endregion


    #region RÚT ĐIỂM
    public void reset_control_add_edit()
    {
        txt_dongA_chuyen.Text = "";
    }

    protected void but_show_form_add_Click(object sender, EventArgs e)
    {
        reset_control_add_edit();
        using (dbDataContext db = new dbDataContext())
        {
            var q = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());
            if (q != null)
            {
                if (q.phanloai != "Gian hàng đối tác")
                {
                    Helper_Tabler_cl.ShowModal(this.Page, "Chức năng chỉ dành cho Gian hàng đối tác.", "Thông báo", true, "warning");
                    return;
                }
            }

            pn_add.Visible = !pn_add.Visible;
            up_add.Update();
        }
    }

    protected void but_close_form_add_Click(object sender, EventArgs e)
    {
        reset_control_add_edit();
        pn_add.Visible = !pn_add.Visible;
    }

    protected void but_add_edit_Click(object sender, EventArgs e)
    {
        #region Chuẩn bị dữ liệu
        decimal _dongA = 0m;
        decimal.TryParse(txt_dongA_chuyen.Text.Trim().Replace(",", ""), out _dongA);

        string _tk_nhan = "vitonggianhangdoitac";
        #endregion

        using (dbDataContext db = new dbDataContext())
        {
            var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == ViewState["taikhoan"].ToString());

            #region Kiểm tra ngoại lệ.
            if (_dongA <= 0)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Số Quyền tiêu dùng không hợp lệ.", "Thông báo", true, "warning");
                return;
            }
            if (!taikhoan_cl.exist_taikhoan(_tk_nhan))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản nhận không hợp lệ.", "Thông báo", true, "warning");
                return;
            }

            decimal _tongtien = q_tk.HoSo_TieuDung_ShopOnly ?? 0m;
            if (_tongtien < _dongA)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Quyền tiêu dùng của bạn không đủ để rút.", "Thông báo", true, "warning");
                return;
            }
            #endregion

            LichSuChuyenDiem_tb _ob = new LichSuChuyenDiem_tb();
            _ob.taikhoan_chuyen = ViewState["taikhoan"].ToString();
            _ob.taikhoan_nhan = _tk_nhan;
            _ob.dongA = _dongA;
            _ob.ngay = AhaTime_cl.Now;
            _ob.nap_rut = false; _ob.trangtrai_rut = "Chờ xác nhận";
            db.LichSuChuyenDiem_tbs.InsertOnSubmit(_ob);
            db.SubmitChanges();

            LichSu_DongA_tb _ob2 = new LichSu_DongA_tb();
            _ob2.taikhoan = ViewState["taikhoan"].ToString();
            _ob2.dongA = _dongA;
            _ob2.ngay = AhaTime_cl.Now;
            _ob2.CongTru = false;//trừ
            _ob2.id_donhang = "";
            _ob2.ghichu = "|SHOPONLY|WITHDRAW| Đang xác nhận yêu cầu rút điểm. ID rút: " + _ob.id.ToString();
            _ob2.id_rutdiem = _ob.id.ToString();
            _ob2.LoaiHoSo_Vi = 1;//ví tiêu dùng
            db.LichSu_DongA_tbs.InsertOnSubmit(_ob2);

            q_tk.HoSo_TieuDung_ShopOnly = (q_tk.HoSo_TieuDung_ShopOnly ?? 0m) - _dongA;

            ThongBao_tb _ob4 = new ThongBao_tb();
            _ob4.id = Guid.NewGuid();
            _ob4.daxem = false;
            _ob4.nguoithongbao = ViewState["taikhoan"].ToString();
            _ob4.nguoinhan = _tk_nhan;
            _ob4.link = "/admin/lich-su-chuyen-diem/default.aspx";
            _ob4.noidung = q_tk.hoten + " yêu cầu rút " + _dongA.ToString("#,##0.##") + " Quyền tiêu dùng";
            _ob4.thoigian = AhaTime_cl.Now;
            _ob4.bin = false;
            db.ThongBao_tbs.InsertOnSubmit(_ob4);

            db.SubmitChanges();

            show_main();
            up_main.Update();

            BindBalance();

            txt_dongA_chuyen.Text = ""; ViewState["quyen_chuyendiem"] = null;
            pn_add.Visible = !pn_add.Visible;

            Helper_Tabler_cl.ShowModal(this.Page, "Xử lý thành công.", "Thông báo", true, "success");
        }
    }
    #endregion


    #region NẠP ĐIỂM
    public void reset_control_nap()
    {
        CheckBox1.Checked = false;
    }

    protected void but_show_form_nap_Click(object sender, EventArgs e)
    {
        reset_control_nap();
        using (dbDataContext db = new dbDataContext())
        {
            pn_nap.Visible = !pn_nap.Visible;
            up_nap.Update();
        }
    }

    protected void but_close_form_nap_Click(object sender, EventArgs e)
    {
        reset_control_nap();
        pn_nap.Visible = !pn_nap.Visible;
    }

    protected void but_nap_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            bool _check_ck = CheckBox1.Checked;
            string _goi = DropDownList1.SelectedValue;

            if (_goi == "")
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn gói muốn nạp.", "Thông báo", true, "warning");
                return;
            }
            if (_check_ck == false)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chuyển khoản và đánh dấu vào ô Tôi đã chuyển khoản trước khi nhấn nút NẠP ĐIỂM.", "Thông báo", true, "warning");
                return;
            }

            pn_add.Visible = !pn_add.Visible;

            Helper_Tabler_cl.ShowModal(this.Page, "Chức năng đang được xây dựng quy trình", "Thông báo", true, "warning");
        }
    }
    #endregion
}
