using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;
using System.Text.RegularExpressions;

public partial class home_lich_su_chuyen_diem : System.Web.UI.Page
{
    DanhMuc_cl dm_cl = new DanhMuc_cl();
    String_cl str_cl = new String_cl();
    DateTime_cl dt_cl = new DateTime_cl();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", true);

            string _tk = Session["taikhoan_home"] as string;

            if (!string.IsNullOrEmpty(_tk))
            {
                ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
            }

            set_dulieu_macdinh();
            show_main();
        }
    }

    public void set_dulieu_macdinh()
    {
        ViewState["current_page_lscd_home"] = "1";
    }

    #region main - phân trang - tìm kiếm
    public void show_main()
    {
        string _tk = ViewState["taikhoan"].ToString();
        using (dbDataContext db = new dbDataContext())
        {
            var list_all = (from ob1 in db.LichSuChuyenDiem_tbs.Where(p => p.taikhoan_chuyen == _tk || p.taikhoan_nhan == _tk)
                            select new
                            {
                                ob1.id,
                                ob1.ngay,
                                ob1.dongA,
                                ob1.taikhoan_chuyen,
                                ob1.taikhoan_nhan,
                            }).AsQueryable();

            list_all = list_all.OrderByDescending(p => p.ngay);
            int _Tong_Record = list_all.Count();

            int show = 30;
            int current_page = int.Parse(ViewState["current_page_lscd_home"].ToString());
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

            Repeater1.DataSource = list_split;
            Repeater1.DataBind();
        }
    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_lscd_home"] = int.Parse(ViewState["current_page_lscd_home"].ToString()) - 1;

        HttpCookie cookie = Request.Cookies["cookie_lscd_home"];
        if (cookie != null)
        {
            cookie["trang_hientai"] = ViewState["current_page_lscd_home"].ToString();
            cookie.Expires = AhaTime_cl.Now.AddDays(1);
            Response.Cookies.Set(cookie);
        }

        show_main();
    }

    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        ViewState["current_page_lscd_home"] = int.Parse(ViewState["current_page_lscd_home"].ToString()) + 1;

        HttpCookie cookie = Request.Cookies["cookie_lscd_home"];
        if (cookie != null)
        {
            cookie["trang_hientai"] = ViewState["current_page_lscd_home"].ToString();
            cookie.Expires = AhaTime_cl.Now.AddDays(1);
            Response.Cookies.Set(cookie);
        }

        show_main();
    }

    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        string _tk = Session["taikhoan_home"] as string;
        if (!string.IsNullOrEmpty(_tk))
        {
            _tk = mahoa_cl.giaima_Bcorn(_tk);
        }
        else
            _tk = "";
    }
    #endregion

    #region ADD - EDIT - CHI TIẾT
    public void reset_control_add_edit()
    {
        DropDownList1.DataSource = null;
        DropDownList1.DataBind();
        txt_dongA_chuyen.Text = "";
    }

    protected void but_show_form_add_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_home("none", "none", true);
        reset_control_add_edit();
        using (dbDataContext db = new dbDataContext())
        {
            var q_1 = db.taikhoan_tbs
                .Where(p => p.taikhoan != ViewState["taikhoan"].ToString())
                .Select(p => new
                {
                    taikhoan = p.taikhoan,
                    hienthi = p.taikhoan + " - " + p.hoten
                })
                .ToList();

            DropDownList1.DataSource = q_1;
            DropDownList1.DataTextField = "hienthi";
            DropDownList1.DataValueField = "taikhoan";
            DropDownList1.DataBind();

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
        decimal _dongA = 0m;
        decimal.TryParse(txt_dongA_chuyen.Text.Trim().Replace(",", ""), out _dongA);

        string _tk_nhan = DropDownList1.SelectedValue.ToString();

        if (_tk_nhan == ViewState["taikhoan"].ToString())
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Không thể tự chuyển Quyền tiêu dùng cho chính mình.", "Thông báo", true, "warning");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
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

            var q_tk_chuyen = db.taikhoan_tbs.First(p => p.taikhoan == ViewState["taikhoan"].ToString());

            decimal _tongtien = q_tk_chuyen.DongA.Value;

            if (_tongtien < _dongA)
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Quyền tiêu dùng của bạn không đủ để chuyển.", "Thông báo", true, "warning");
                return;
            }

            q_tk_chuyen.DongA = q_tk_chuyen.DongA - _dongA;

            LichSu_DongA_tb _ob22 = new LichSu_DongA_tb();
            _ob22.taikhoan = ViewState["taikhoan"].ToString();
            _ob22.dongA = _dongA;
            _ob22.ngay = AhaTime_cl.Now;
            _ob22.CongTru = false;
            _ob22.id_donhang = "";
            _ob22.ghichu = "Chuyển Quyền tiêu dùng đến " + db.taikhoan_tbs.First(p => p.taikhoan == _tk_nhan).hoten;
            _ob22.LoaiHoSo_Vi = 1;//ví tiêu dùng
            db.LichSu_DongA_tbs.InsertOnSubmit(_ob22);

            LichSuChuyenDiem_tb _ob = new LichSuChuyenDiem_tb();
            _ob.taikhoan_chuyen = ViewState["taikhoan"].ToString();
            _ob.taikhoan_nhan = _tk_nhan;
            _ob.dongA = _dongA;
            _ob.ngay = AhaTime_cl.Now;
            _ob.nap_rut = true; _ob.trangtrai_rut = "";
            db.LichSuChuyenDiem_tbs.InsertOnSubmit(_ob);

            var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _tk_nhan);
            if (q_tk != null)
            {
                q_tk.DongA = q_tk.DongA + _dongA;
            }

            LichSu_DongA_tb _ob2 = new LichSu_DongA_tb();
            _ob2.taikhoan = _tk_nhan;
            _ob2.dongA = _dongA;
            _ob2.ngay = AhaTime_cl.Now;
            _ob2.CongTru = true;
            _ob2.id_donhang = "";
            _ob2.ghichu = "Nhận Quyền tiêu dùng từ " + db.taikhoan_tbs.First(p => p.taikhoan == ViewState["taikhoan"].ToString()).hoten;
            _ob2.LoaiHoSo_Vi = 1;//ví tiêu dùng
            db.LichSu_DongA_tbs.InsertOnSubmit(_ob2);

            ThongBao_tb _ob4 = new ThongBao_tb();
            _ob4.id = Guid.NewGuid();
            _ob4.daxem = false;
            _ob4.nguoithongbao = ViewState["taikhoan"].ToString();
            _ob4.nguoinhan = _tk_nhan;
            _ob4.link = "/home/lich-su-giao-dich.aspx";
            _ob4.noidung = db.taikhoan_tbs.First(p => p.taikhoan == ViewState["taikhoan"].ToString()).hoten
                         + " vừa chuyển cho bạn " + _dongA.ToString("#,##0.##") + " Quyền tiêu dùng";
            _ob4.thoigian = AhaTime_cl.Now;
            _ob4.bin = false;
            db.ThongBao_tbs.InsertOnSubmit(_ob4);

            db.SubmitChanges();

            show_main();
            up_main.Update();

            DropDownList1.DataSource = null;
            DropDownList1.DataBind();
            txt_dongA_chuyen.Text = "";
            pn_add.Visible = !pn_add.Visible;

            Helper_Tabler_cl.ShowModal(this.Page, "Xử lý thành công.", "Thông báo", true, "success");
        }
    }
    #endregion
}
