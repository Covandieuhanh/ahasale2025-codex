using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    // ===== QUY ƯỚC: 1 A = 1000 VNĐ =====
    private const decimal VND_PER_A = 1000m;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back_home"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_home("none", "none", false);

            using (dbDataContext db = new dbDataContext())
            {
                // lấy tài khoản
                string _tk = Session["taikhoan_home"] as string;
                if (!string.IsNullOrEmpty(_tk))
                    ViewState["taikhoan"] = mahoa_cl.giaima_Bcorn(_tk);
                else
                    ViewState["taikhoan"] = "";

                // META HOME
                var q = (from tk in db.CaiDatChung_tbs
                         where tk.phanloai_trang == "home"
                         select new
                         {
                             tk.lienket_chiase_title,
                             tk.lienket_chiase_description,
                             tk.lienket_chiase_image
                         }).FirstOrDefault();

                if (q != null)
                {
                    string title = q.lienket_chiase_title ?? "";
                    string description = q.lienket_chiase_description ?? "";
                    string imageRelativePath = q.lienket_chiase_image ?? "";

                    string imageUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, imageRelativePath);

                    string metaTags = string.Format(@"
                        <title>{0}</title>
                        <meta name='description' content='{1}' />
                        <meta property='og:title' content='{2}' />
                        <meta property='og:description' content='{3}' />
                        <meta property='og:image' content='{4}' />
                        <meta property='og:type' content='website' />
                        <meta property='og:url' content='{5}' />
                        <meta name='twitter:card' content='summary_large_image' />
                        <meta name='twitter:title' content='{6}' />
                        <meta name='twitter:description' content='{7}' />
                        <meta name='twitter:image' content='{8}' />
                    ", title, description, title, description, imageUrl, Request.Url.AbsoluteUri, title, description, imageUrl);

                    literal_meta.Text = metaTags;
                }
            }
        }
    }

    // ===== HELPER GET VIEWSTATE INT =====
    private int GetViewStateInt(string key, int defaultVal)
    {
        try
        {
            if (ViewState[key] == null) return defaultVal;
            int v;
            if (int.TryParse(ViewState[key].ToString(), out v)) return v;
            return defaultVal;
        }
        catch { return defaultVal; }
    }

    public void set_dulieu_macdinh()
    {
        ViewState["current_page_home"] = "1";
        ViewState["total_page"] = "1";
    }

    private void LoadThanhPho(dbDataContext db)
    {
        var list = db.ThanhPhos.ToList();

        ddlThanhPho.DataSource = list;
        ddlThanhPho.DataTextField = "Ten";
        ddlThanhPho.DataValueField = "Ten";
        ddlThanhPho.DataBind();

        ddlThanhPho.Items.Insert(0, new ListItem("Chọn thành phố", ""));
    }

    protected void ddlThanhPho_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewState["current_page_home"] = "1";
        using (dbDataContext db = new dbDataContext())
            HienThiTinMoi(db);
    }

    public void HienThiBanner(dbDataContext db)
    {
        var q = db.Banner_tbs.Where(p => p.show == true).OrderBy(d => d.rank).ToList();
        Repeater3.DataSource = q;
        Repeater3.DataBind();
    }

    public void HienThiDanhMuc(dbDataContext db)
    {
        var danhMucList = db.DanhMuc_tbs
            .Where(p => p.id_parent == "134" && p.bin == false)
            .OrderBy(d => d.rank)
            .ToList();

        Repeater1.DataSource = danhMucList;
        Repeater1.DataBind();
    }

    public void HienThiTinMoi(dbDataContext db)
    {
        var list_all = (from ob1 in db.BaiViet_tbs.Where(p => p.bin == false && p.phanloai == "sanpham")
                        join ob2 in db.DanhMuc_tbs on ob1.id_DanhMuc equals ob2.id.ToString() into danhMucGroup
                        from ob2 in danhMucGroup.DefaultIfEmpty()
                        join ob3 in db.DanhMuc_tbs on ob1.id_DanhMucCap2 equals ob3.id.ToString() into danhMucGroup2
                        from ob3 in danhMucGroup2.DefaultIfEmpty()
                        select new
                        {
                            ob1.id,
                            ob1.image,
                            ob1.name,
                            ob1.name_en,
                            ThanhPho = (ob1.ThanhPho ?? "Không có"),
                            ob1.ngaytao,
                            giaban = ob1.giaban, // decimal?
                            ob1.nguoitao,
                            ob1.description,
                            ob1.soluong_daban,
                            LuotTruyCap = (ob1.LuotTruyCap ?? 0),
                            TenMenu = ob2 != null ? ob2.name : "",
                            TenMenu2 = ob3 != null ? "<span class='pl-3 mif-chevron-right'></span>" + ob3.name : "",
                        }).AsQueryable();

        // search key
        string _key = (txt_search.Text ?? "").Trim();
        if (!string.IsNullOrEmpty(_key))
            list_all = list_all.Where(p => p.name.Contains(_key) || p.name_en.Contains(_key) || p.id.ToString() == _key);

        // filter theo thành phố
        string tenThanhPho = (ddlThanhPho.SelectedValue ?? "").ToString();
        if (!string.IsNullOrEmpty(tenThanhPho))
            list_all = list_all.Where(p => p.ThanhPho.Contains(tenThanhPho));

        list_all = list_all.OrderByDescending(p => p.ngaytao);

        int Tong_Record = list_all.Count();

        // ===== PHÂN TRANG (GIỮ LOGIC) =====
        int show = 40;
        if (show <= 0) show = 40;

        int current_page = GetViewStateInt("current_page_home", 1);
        int total_page = number_of_page_class.return_total_page(Tong_Record, show);
        if (total_page <= 0) total_page = 1;

        if (current_page < 1) current_page = 1;
        if (current_page > total_page) current_page = total_page;

        ViewState["current_page_home"] = current_page.ToString();
        ViewState["total_page"] = total_page.ToString();

        var list_split = list_all.Skip(current_page * show - show).Take(show).ToList();

        // ===== chia 2 repeater =====
        int totalCount = list_split.Count;
        int halfCount = (int)Math.Ceiling(totalCount / 2.0);

        var list1 = list_split.Take(halfCount).ToList();
        var list2 = list_split.Skip(halfCount).ToList();

        Repeater2.DataSource = list1;
        Repeater2.DataBind();

        Repeater4.DataSource = list2;
        Repeater4.DataBind();
    }

    // ====================== PAGING (KHÔNG LOCAL FUNCTION) ======================
    private void GeneratePageLinks(int currentPage, int totalPages)
    {
        phPageNumbers.Controls.Clear();

        if (totalPages <= 1) return;

        int sidePages = 2;

        AddPageButton(1, currentPage == 1);

        int start = Math.Max(2, currentPage - sidePages);
        int end = Math.Min(totalPages - 1, currentPage + sidePages);

        if (start > 2) AddEllipsis();

        for (int i = start; i <= end; i++)
            AddPageButton(i, i == currentPage);

        if (end < totalPages - 1) AddEllipsis();

        AddPageButton(totalPages, currentPage == totalPages);
    }

    private void AddPageButton(int page, bool isActive)
    {
        LinkButton lb = new LinkButton();
        lb.Text = page.ToString();
        lb.CommandArgument = page.ToString();
        lb.Click += new EventHandler(PageNumber_Click);
        lb.CssClass = "paging-button" + (isActive ? " active" : "");
        phPageNumbers.Controls.Add(lb);
    }

    private void AddEllipsis()
    {
        Literal ellipsis = new Literal();
        ellipsis.Text = "<span class='paging-button'>...</span>";
        phPageNumbers.Controls.Add(ellipsis);
    }

    protected void PageNumber_Click(object sender, EventArgs e)
    {
        LinkButton lb = (LinkButton)sender;
        int selectedPage = 1;
        int.TryParse(lb.CommandArgument, out selectedPage);

        ViewState["current_page_home"] = selectedPage.ToString();

        using (dbDataContext db = new dbDataContext())
            HienThiTinMoi(db);
    }

    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        int cur = GetViewStateInt("current_page_home", 1);
        cur--;
        if (cur < 1) cur = 1;
        ViewState["current_page_home"] = cur.ToString();

        using (dbDataContext db = new dbDataContext())
            HienThiTinMoi(db);
    }

    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        int cur = GetViewStateInt("current_page_home", 1);
        int total = GetViewStateInt("total_page", 1);

        cur++;
        if (cur > total) cur = total;
        ViewState["current_page_home"] = cur.ToString();

        using (dbDataContext db = new dbDataContext())
            HienThiTinMoi(db);
    }

    protected void txt_timkiem_TextChanged(object sender, EventArgs e)
    {
        ViewState["current_page_home"] = "1";
        using (dbDataContext db = new dbDataContext())
            HienThiTinMoi(db);
    }

    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        ViewState["current_page_home"] = "1";
        using (dbDataContext db = new dbDataContext())
        {
            LoadThanhPho(db);
            HienThiTinMoi(db);
        }
    }

    protected void Repeater2_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            var dataItem = e.Item.DataItem;

            Button btnBanSanPham = (Button)e.Item.FindControl("but_bansanphamnay");
            Button btnTraoDoi = (Button)e.Item.FindControl("but_traodoi");
            Button btnThemVaoGio = (Button)e.Item.FindControl("but_themvaogio");

            if (ViewState["taikhoan"] == null || string.IsNullOrEmpty(ViewState["taikhoan"].ToString()))
            {
                btnBanSanPham.Style["visibility"] = "hidden";
                btnTraoDoi.Style["visibility"] = "hidden";
                btnThemVaoGio.Style["visibility"] = "hidden";
            }
            else
            {
                string nguoitao = Convert.ToString(DataBinder.Eval(dataItem, "nguoitao"));
                if (nguoitao == ViewState["taikhoan"].ToString())
                {
                    btnBanSanPham.Style["visibility"] = "hidden";
                    btnTraoDoi.Style["visibility"] = "hidden";
                    btnThemVaoGio.Style["visibility"] = "hidden";
                }
                else
                {
                    btnBanSanPham.Style["visibility"] = "visible";
                    btnTraoDoi.Style["visibility"] = "visible";
                    btnThemVaoGio.Style["visibility"] = "visible";
                }
            }
        }
    }

    // ===================== BÁN CHÉO =====================
    protected void but_bansanphamnay_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            Button button = (Button)sender;
            string _idsp = button.CommandArgument;

            string tk = (ViewState["taikhoan"] ?? "").ToString();
            if (string.IsNullOrEmpty(tk))
                return;

            var q = db.BanSanPhamNay_tbs.FirstOrDefault(p => p.idsp == _idsp && p.taikhoan_ban == tk);
            if (q == null)
            {
                BanSanPhamNay_tb _ob = new BanSanPhamNay_tb();
                _ob.idsp = _idsp;
                _ob.ban_ngungban = true;
                _ob.ngaythem = AhaTime_cl.Now;
                _ob.taikhoan_ban = tk;
                _ob.taikhoan_goc = db.BaiViet_tbs.First(p => p.id.ToString() == _idsp).nguoitao;

                db.BanSanPhamNay_tbs.InsertOnSubmit(_ob);
                db.SubmitChanges();

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "Sản phẩm này đã được thêm vào cửa hàng của bạn.", "false", "false", "OK", "alert", ""), true);
            }
        }
    }

    // ===================== TRAO ĐỔI / ĐẶT HÀNG =====================
    protected void txt_soluong2_TextChanged(object sender, EventArgs e)
    {
        int sl = Number_cl.Check_Int((txt_soluong2.Text ?? "").Trim());
        if (sl < 0) sl = 0;

        decimal giaVND = 0m;
        if (ViewState["giaban"] != null)
            decimal.TryParse(ViewState["giaban"].ToString(), out giaVND);

        decimal tong = giaVND * sl;
        Literal11.Text = tong.ToString("#,##0");
    }

    protected void but_traodoi_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            Button button = (Button)sender;
            string _idsp = button.CommandArgument;

            var q = db.BaiViet_tbs.FirstOrDefault(p => p.id.ToString() == _idsp && p.bin == false);
            if (q != null)
            {
                ViewState["idsp_giohang"] = _idsp;

                decimal giaVND = q.giaban.HasValue ? q.giaban.Value : 0m;
                ViewState["giaban"] = giaVND.ToString();

                Literal9.Text = q.name;
                txt_soluong2.Text = "1";
                Literal10.Text = giaVND.ToString("#,##0");
                Literal11.Text = giaVND.ToString("#,##0");

                var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == (ViewState["taikhoan"] ?? "").ToString());
                if (q_tk != null)
                {
                    txt_hoten_nguoinhan.Text = q_tk.hoten;
                    txt_sdt_nguoinhan.Text = q_tk.dienthoai;
                    txt_diachi_nguoinhan.Text = q_tk.diachi;
                }

                pn_dathang.Visible = true;
                up_dathang.Update();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "Sản phẩm đã ngừng bán", "false", "false", "OK", "alert", ""), true);
            }
        }
    }

    protected void but_close_form_dathang_Click(object sender, EventArgs e)
    {
        Literal9.Text = ""; Literal10.Text = ""; Literal11.Text = "";
        txt_soluong2.Text = "1";
        ViewState["idsp_giohang"] = null;
        ViewState["giaban"] = null;
        pn_dathang.Visible = false;
    }

    protected void but_dathang_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            int sl = Number_cl.Check_Int((txt_soluong2.Text ?? "").Trim());
            if (sl <= 0)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "Số lượng không hợp lệ.", "false", "false", "OK", "alert", ""), true);
                return;
            }

            string tk = (ViewState["taikhoan"] ?? "").ToString();
            if (string.IsNullOrEmpty(tk))
                return;

            var q_tk = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            decimal soDuA = (q_tk != null && q_tk.DongA.HasValue) ? q_tk.DongA.Value : 0m;

            string idsp = (ViewState["idsp_giohang"] ?? "").ToString();
            var sp = db.BaiViet_tbs.FirstOrDefault(p => p.id.ToString() == idsp && p.bin == false);
            if (sp == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "Sản phẩm đã ngừng bán", "false", "false", "OK", "alert", ""), true);
                return;
            }

            decimal giaVND = sp.giaban.HasValue ? sp.giaban.Value : 0m;
            decimal tongVND = giaVND * sl;

            // ===== Quy đổi VNĐ -> A để trừ =====
            decimal canTraA = (decimal)Math.Ceiling((double)(tongVND / VND_PER_A));

            if (canTraA > soDuA)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo",
                        string.Format("Quyền tiêu dùng của bạn không đủ.<br/>Cần <b>{0:#,##0} A</b> (≈ {1:#,##0}đ), bạn có <b>{2:#,##0} A</b>.", canTraA, tongVND, soDuA),
                        "false", "false", "OK", "alert", ""), true);
                return;
            }

            DateTime now = AhaTime_cl.Now;

            // ===== tạo đơn hàng (lưu VNĐ) =====
            DonHang_tb dh = new DonHang_tb();
            dh.ngaydat = now;
            dh.nguoimua = tk;
            dh.nguoiban = sp.nguoitao;
            dh.order_status = DonHangStateMachine_cl.Order_DaDat;
            dh.exchange_status = DonHangStateMachine_cl.Exchange_ChuaTraoDoi;
            dh.tongtien = tongVND; // VNĐ
            dh.hoten_nguoinhan = txt_hoten_nguoinhan.Text.Trim();
            dh.sdt_nguoinhan = txt_sdt_nguoinhan.Text.Trim();
            dh.diahchi_nguoinhan = txt_diachi_nguoinhan.Text.Trim();
            dh.online_offline = true;
            dh.chothanhtoan = false;
            DonHangStateMachine_cl.SyncLegacyStatus(dh);

            db.DonHang_tbs.InsertOnSubmit(dh);
            db.SubmitChanges();

            string id_dh = dh.id.ToString();

            // ===== chi tiết đơn (lưu VNĐ) =====
            DonHang_ChiTiet_tb ct = new DonHang_ChiTiet_tb();
            ct.id_donhang = id_dh;
            ct.idsp = idsp;
            ct.nguoiban_goc = dh.nguoiban;
            ct.nguoiban_danglai = ""; // trang chủ không có đăng lại
            ct.soluong = sl;
            ct.giaban = giaVND;       // VNĐ
            ct.thanhtien = tongVND;   // VNĐ
            db.DonHang_ChiTiet_tbs.InsertOnSubmit(ct);

            // ===== lịch sử Quyền tiêu dùng (lưu A) =====
            LichSu_DongA_tb ls = new LichSu_DongA_tb();
            ls.taikhoan = tk;
            ls.dongA = canTraA; // A
            ls.ngay = now;
            ls.CongTru = false;
            ls.id_donhang = id_dh;
            ls.ghichu = "Trao đổi đơn " + id_dh + ": " + canTraA.ToString("#,##0") + "A (≈ " + tongVND.ToString("#,##0") + "đ)";
            ls.LoaiHoSo_Vi = 1;//ví tiêu dùng
            db.LichSu_DongA_tbs.InsertOnSubmit(ls);

            // ===== trừ A =====
            if (q_tk != null && q_tk.DongA.HasValue)
                q_tk.DongA = q_tk.DongA.Value - canTraA;

            // ===== thông báo =====
            ThongBao_tb tb = new ThongBao_tb();
            tb.id = Guid.NewGuid();
            tb.daxem = false;
            tb.nguoithongbao = tk;
            tb.nguoinhan = dh.nguoiban;
            tb.link = "/home/don-ban.aspx";
            tb.noidung = db.taikhoan_tbs.First(p => p.taikhoan == tk).hoten
                         + " vừa đặt hàng đến bạn. ID đơn hàng: " + id_dh;
            tb.thoigian = now;
            tb.bin = false;
            db.ThongBao_tbs.InsertOnSubmit(tb);

            db.SubmitChanges();

            // reset UI
            Literal9.Text = ""; Literal10.Text = ""; Literal11.Text = "";
            txt_soluong2.Text = "1";
            ViewState["idsp_giohang"] = null;
            ViewState["giaban"] = null;
            pn_dathang.Visible = false;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                thongbao_class.metro_dialog("Thông báo",
                    "Đặt hàng thành công.<br/>ID đơn hàng: <b>" + id_dh + "</b><br/>Đã trừ <b>" + canTraA.ToString("#,##0") + " A</b> (≈ " + tongVND.ToString("#,##0") + "đ).",
                    "false", "false", "OK", "alert", ""), true);
        }
    }

    // ===================== THÊM VÀO GIỎ =====================
    protected void but_themvaogio_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            Button button = (Button)sender;
            string _idsp = button.CommandArgument;

            var q = db.BaiViet_tbs.FirstOrDefault(p => p.id.ToString() == _idsp && p.bin == false);
            if (q != null)
            {
                ViewState["idsp_giohang"] = _idsp;
                Literal1.Text = q.name;
                txt_soluong1.Text = "1";
                pn_add_cart.Visible = true;
                up_add_cart.Update();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                    thongbao_class.metro_dialog("Thông báo", "Sản phẩm đã ngừng bán", "false", "false", "OK", "alert", ""), true);
            }
        }
    }

    protected void but_close_form_addcart_Click(object sender, EventArgs e)
    {
        Literal1.Text = "";
        txt_soluong1.Text = "1";
        ViewState["idsp_giohang"] = null;
        pn_add_cart.Visible = false;
    }

    protected void but_add_cart_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string tk = (ViewState["taikhoan"] ?? "").ToString();
            if (string.IsNullOrEmpty(tk)) return;

            string idsp = (ViewState["idsp_giohang"] ?? "").ToString();
            if (string.IsNullOrEmpty(idsp)) return;

            int slAdd = 1;
            int.TryParse((txt_soluong1.Text ?? "1").Trim(), out slAdd);
            if (slAdd <= 0) slAdd = 1;

            var q = db.GioHang_tbs.FirstOrDefault(p => p.idsp == idsp && p.taikhoan == tk);
            if (q != null)
            {
                q.soluong = q.soluong + slAdd;
                q.ngaythem = AhaTime_cl.Now;
                db.SubmitChanges();
            }
            else
            {
                GioHang_tb ob = new GioHang_tb();
                ob.ngaythem = AhaTime_cl.Now;
                ob.taikhoan = tk;
                ob.idsp = idsp;
                ob.soluong = slAdd;
                ob.nguoiban_goc = db.BaiViet_tbs.First(p => p.id.ToString() == idsp).nguoitao;
                ob.nguoiban_danglai = "";
                db.GioHang_tbs.InsertOnSubmit(ob);
                db.SubmitChanges();
            }

            Literal1.Text = "";
            txt_soluong1.Text = "1";
            ViewState["idsp_giohang"] = null;
            pn_add_cart.Visible = false;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                thongbao_class.metro_notifi("Thông báo", "Xử lý thành công.", "1000", "warning"), true);
        }
    }

    // ===================== SUGGEST =====================
    [System.Web.Services.WebMethod]
    [System.Web.Script.Services.ScriptMethod]
    public static List<string> GetSuggestions(string prefixText, int count)
    {
        using (var db = new dbDataContext())
        {
            return db.BaiViet_tbs
                .Where(p => p.bin == false
                        && p.phanloai == "sanpham"
                        && (p.name.StartsWith(prefixText) || p.name_en.StartsWith(prefixText)))
                .Select(p => p.name)
                .Distinct()
                .Take(count)
                .ToList();
        }
    }
}
