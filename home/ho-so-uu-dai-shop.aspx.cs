using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_lich_su_quyen_uu_dai : System.Web.UI.Page
{
    private const decimal VND_PER_A = 1000m;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", true);

            string _tk = PortalRequest_cl.GetCurrentAccountEncrypted();
            if (!string.IsNullOrEmpty(_tk))
            {
                ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
            }

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
            decimal balance = q != null ? (q.HoSo_UuDai_ShopOnly ?? 0m) : 0m;
            lb_balance_ud.Text = balance.ToString("#,##0.##");
            lb_balance_ud_vnd.Text = (balance * VND_PER_A).ToString("#,##0");
        }
    }

    private void EnsureShopOnlySync(dbDataContext db, string tk)
    {
        if (db == null || string.IsNullOrWhiteSpace(tk)) return;

        var result = ShopOnlyLedger_cl.RecalculateBalances(db, tk, true);
        bool changed = result.Updated;

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

    private bool IsRelevantShopOnlyEntry(string note)
    {
        string normalized = (note ?? string.Empty).ToLowerInvariant();
        if (normalized.Contains(ShopOnlyLedger_cl.TagRoot.ToLowerInvariant())
            || normalized.Contains(ShopOnlyLedger_cl.TagCreditSeller.ToLowerInvariant()))
            return true;

        return false;
    }

    public void set_dulieu_macdinh()
    {
        ViewState["current_page_lsgd_home"] = HomePager_cl.ResolvePage(Request).ToString();
    }

    #region main - phân trang - tìm kiếm
    public void show_main()
    {
        using (dbDataContext db = new dbDataContext())
        {
            // ✅ Lọc ví ưu đãi
            var list_all = (from ob1 in db.LichSu_DongA_tbs.Where(p =>
                  p.taikhoan == ViewState["taikhoan"].ToString()
                  && p.LoaiHoSo_Vi == 2
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
                                ob1.id_donhang
                            }).AsEnumerable()
                              .Where(p => IsRelevantShopOnlyEntry(p.ghichu))
                              .AsQueryable();




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
            list_all = list_all.OrderByDescending(p => p.ngay);

            int _Tong_Record = list_all.Count();

            // phân trang
            int show = 30; if (show <= 0) show = 30;
            int current_page = int.Parse(ViewState["current_page_lsgd_home"].ToString());
            int total_page = number_of_page_class.return_total_page(_Tong_Record, show);
            if (total_page < 1) total_page = 1;

            if (current_page < 1) current_page = 1;
            else if (current_page > total_page) current_page = total_page;

            ViewState["total_page"] = total_page;

            litPager.Text = HomePager_cl.RenderPager(Request, current_page, total_page);
            but_xemtiep.Visible = false;
            but_xemtiep1.Visible = false;
            but_quaylai.Visible = false;
            but_quaylai1.Visible = false;

            // buttons
            bool canNext = current_page < total_page;
            bool canPrev = current_page > 1;

            but_xemtiep.Enabled = canNext;
            but_xemtiep1.Enabled = canNext;

            but_quaylai.Enabled = canPrev;
            but_quaylai1.Enabled = canPrev;

            var list_split = list_all.Skip(current_page * show - show).Take(show);

            int stt = (show * current_page) - show + 1;
            int _s1 = stt + list_split.Count() - 1;

            if (_Tong_Record != 0) lb_show.Text = stt + "-" + _s1 + " trong số " + _Tong_Record.ToString("#,##0");
            else lb_show.Text = "0-0/0";

            lb_show_md.Text = lb_show.Text;

            Repeater1.DataSource = list_split;
            Repeater1.DataBind();
            ph_empty_uudai.Visible = list_split.Count() == 0;
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
    protected void LinkButton1_Click(object sender, EventArgs e)
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
                if (q_ban != null)
                {
                    Label100.Text = q_ban.hoten;
                    Label101.Text = q_ban.dienthoai;
                    Label102.Text = q_ban.diachi;
                }
                else
                {
                    Label100.Text = "";
                    Label101.Text = "";
                    Label102.Text = "";
                }

                var q_mua = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == _nguoimua);
                if (q_mua != null)
                {
                    Label103.Text = q_mua.hoten;
                    Label104.Text = q_mua.dienthoai;
                    Label105.Text = q_mua.diachi;
                }
                else
                {
                    Label103.Text = "";
                    Label104.Text = "";
                    Label105.Text = "";
                }

                var q_ct = from ob1 in db.DonHang_ChiTiet_tbs.Where(p => p.id_donhang == _iddh)
                           join ob2 in db.BaiViet_tbs on ob1.idsp equals ob2.id.ToString() into SanPhamGroup
                           from ob2 in SanPhamGroup.DefaultIfEmpty()
                           select new
                           {
                               ob1.id,
                               ob1.id_donhang,
                               name = ob2 != null ? ob2.name : "",
                               image = ob2 != null ? ob2.image : "",
                               ob2.name_en,
                               ob1.giaban,
                               ob1.soluong,
                               ob1.thanhtien
                           };

                Repeater2.DataSource = q_ct;
                Repeater2.DataBind();

                pn_chitiet.Visible = true;
                up_chitiet.Update();
            }
        }
    }

    protected void but_close_form_chitiet_Click(object sender, EventArgs e)
    {
        Repeater2.DataSource = null;
        Repeater2.DataBind();

        pn_chitiet.Visible = false;
        up_chitiet.Update();
    }
    #endregion
}
