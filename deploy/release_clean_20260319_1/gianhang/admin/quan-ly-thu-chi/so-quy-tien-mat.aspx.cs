using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_Default : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    thuchi_class tc_cl = new thuchi_class();
    datetime_class dt_cl = new datetime_class();
    public Int64 tondauky, tonhientai;
    public int stt = 1;
    public string tienbangchu = "";

    public string user, user_parent;
   

    public void main()
    {
        if (ddl_loc1.SelectedValue.ToString() != "0")
        {
            var list_all = (from tc in db.bspa_thuchi_tables.Where(p => p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString() && p.id_nhomthuchi == ddl_loc1.SelectedValue.ToString() && p.ngay.Value.Date >= DateTime.Parse(Session["tungay_sqtm"].ToString()) && p.ngay.Value.Date <= DateTime.Parse(Session["denngay_sqtm"].ToString())).ToList()
                            join nq in db.bspa_nhomthuchi_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on tc.id_nhomthuchi equals nq.id.ToString()
                            select new
                            {
                                id = tc.id,
                                tennhom = nq.tennhom,
                                noidung = tc.noidung,
                                nguoilapphieu = tc.nguoilapphieu,
                                ngay = tc.ngay,
                                thuchi = tc.thuchi,//ký hiệu true=thu, false=chi
                                sotien = tc.sotien.Value,
                            }).ToList();

            //tính tồn đầu kỳ
            foreach (var t in db.bspa_thuchi_tables.Where(p => p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString() && p.id_nhomthuchi == ddl_loc1.SelectedValue.ToString() && p.ngay.Value.Date < DateTime.Parse(Session["tungay_sqtm"].ToString())).ToList())
            {
                if (t.thuchi == "Thu")
                    tondauky = tondauky + t.sotien.Value;
                else
                    tondauky = tondauky - t.sotien.Value;
                tonhientai = tondauky;
            }

            if (ddl_loc2.SelectedValue.ToString() != "0")
            {
                var list_1 = list_all.Where(p => p.thuchi == ddl_loc2.SelectedValue.ToString()).ToList();
                list_all = list_all.Intersect(list_1).ToList();
            }

            list_all = list_all.OrderBy(p => p.ngay).ToList();
            Repeater2.DataSource = list_all;
            Repeater2.DataBind();
        }
        else
        {
            var list_all = (from tc in db.bspa_thuchi_tables.Where(p => p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString() && p.ngay.Value.Date >= DateTime.Parse(Session["tungay_sqtm"].ToString()) && p.ngay.Value.Date <= DateTime.Parse(Session["denngay_sqtm"].ToString())).ToList()
                            join nq in db.bspa_nhomthuchi_tables.Where(p => p.id_chinhanh == Session["chinhanh"].ToString()).ToList() on tc.id_nhomthuchi equals nq.id.ToString()
                            select new
                            {
                                id = tc.id,
                                tennhom = nq.tennhom,
                                noidung = tc.noidung,
                                nguoilapphieu = tc.nguoilapphieu,
                                ngay = tc.ngay,
                                thuchi = tc.thuchi,//ký hiệu true=thu, false=chi
                                sotien = tc.sotien.Value,
                            }).ToList();

            //tính tồn đầu kỳ
            foreach (var t in db.bspa_thuchi_tables.Where(p => p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString() && p.ngay.Value.Date < DateTime.Parse(Session["tungay_sqtm"].ToString())).ToList())
            {
                if (t.thuchi == "Thu")
                    tondauky = tondauky + t.sotien.Value;
                else
                    tondauky = tondauky - t.sotien.Value;
                tonhientai = tondauky;
            }

            if (ddl_loc2.SelectedValue.ToString() != "0")
            {
                var list_1 = list_all.Where(p => p.thuchi == ddl_loc2.SelectedValue.ToString()).ToList();
                list_all = list_all.Intersect(list_1).ToList();
            }

            list_all = list_all.OrderBy(p => p.ngay).ToList();
            Repeater2.DataSource = list_all;
            Repeater2.DataBind();
        }

       

        if (tonhientai < 0)
            tienbangchu = "Âm " + number_class.number_to_text_unlimit(tonhientai.ToString().Replace("-",""));
    }
    public string tinhtonhientai(string _idthuchi)
    {
        var q = db.bspa_thuchi_tables.Where(p => p.user_parent == user_parent && p.id.ToString() == _idthuchi && p.id_chinhanh == Session["chinhanh"].ToString());
        if (q.First().thuchi == "Thu")
        {
            tonhientai = tonhientai + q.First().sotien.Value;
            return tonhientai.ToString("#,##0");
        }
        else
        {
            tonhientai = tonhientai - q.First().sotien.Value;
            return tonhientai.ToString("#,##0");
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login
        string _quyen = "none";
        string _cookie_user = "", _cookie_pass = "";
        if (Request.Cookies["save_user_admin_aka_1"] != null) _cookie_user = Request.Cookies["save_user_admin_aka_1"].Value;
        if (Request.Cookies["save_pass_admin_aka_1"] != null) _cookie_pass = Request.Cookies["save_pass_admin_aka_1"].Value;
        if (Session["user"] == null) Session["user"] = ""; if (Session["notifi"] == null) Session["notifi"] = "";if (Session["user"].ToString() == "") Response.Redirect("/gianhang/admin/f5_ss_admin.aspx");
        string _url = Request.Url.GetLeftPart(UriPartial.Authority).ToLower();
        string _kq = bcorn_class.check_login(Session["user"].ToString(), _cookie_user, _cookie_pass, _url, _quyen);
        if (_kq != "")//nếu có thông báo --> có lỗi --> reset --> bắt login lại
        {
            if (_kq == "baotri") Response.Redirect("/baotri.aspx");
            else
            {
                if (_kq == "1") Response.Redirect("/gianhang/admin/login.aspx");//hết Session, hết Cookie
                else
                {
                    if (_kq == "2")//k đủ quyền
                    {
                        Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
                        Response.Redirect("/gianhang/admin");
                    }
                    else
                    {
                        Session["notifi"] = _kq; Session["user"] = "";
                        Response.Cookies["save_user_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["save_pass_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["save_url_admin_aka_1"].Expires = DateTime.Now.AddDays(-1);
                        Response.Redirect("/gianhang/admin/login.aspx");
                    }
                }
            }
        }
        #endregion 
        user = Session["user"].ToString();
        user_parent = "admin";
        if (!IsPostBack)
        {
            Session["index_sapxep_sqtm"] = "0";
            Session["current_page_sqtm"] = "1";

            if (Session["tungay_sqtm"] != null)
                txt_tungay.Text = Session["tungay_sqtm"].ToString();
            else
            {
                txt_tungay.Text = dt_cl.return_ngaydauthang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
                Session["tungay_sqtm"] = txt_tungay.Text;
            }
            if (Session["denngay_sqtm"] != null)
                txt_denngay.Text = Session["denngay_sqtm"].ToString();
            else
            {
                txt_denngay.Text = dt_cl.return_ngaycuoithang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
                Session["denngay_sqtm"] = txt_denngay.Text;
            }
            ddl_loc1.DataSource = db.bspa_nhomthuchi_tables.Where(p => p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString()).OrderBy(p => p.tennhom).ToList();
            ddl_loc1.DataTextField = "tennhom";
            ddl_loc1.DataValueField = "id";
            ddl_loc1.DataBind();
            ddl_loc1.Items.Insert(0, new ListItem("Tất cả", "0"));
        }
        main();
    }

    public string get_hinhanh(string _id_thuchi)
    {
        string _kq;
        var q = db.bspa_hinhanhthuchi_tables.Where(p => p.user_parent == user_parent && p.id_thuchi == _id_thuchi && p.id_chinhanh == Session["chinhanh"].ToString());
        if (q.Count() != 0)
        {
            if (q.Count() == 1)
            {
                _kq = "<img src='" + q.First().hinhanh + "' data-original='" + q.First().hinhanh + "' class='img-cover-60-vuong' style='max-width: none!important'";
                return _kq;
            }
            else
            {
                string _kq2 = "<img src='" + q.First().hinhanh + "' data-original='" + q.First().hinhanh + "' class='img-cover-60-vuong' style='max-width: none!important'";
                foreach (var t in q)
                {
                    _kq2 = _kq2 + "<img class='d-none' src='" + t.hinhanh + "'>";
                }
                return _kq2;
            }
        }
        return "";
    }


    protected void but_locthoigian_Click(object sender, EventArgs e)
    {
        tonhientai = 0;tondauky = 0;
        //Button1.Style.Add("background-color", "#ff9447");
        //Button1.Style.Add("background-color", "white");
        Session["tungay_sqtm"] = txt_tungay.Text;
        Session["denngay_sqtm"] = txt_denngay.Text;
        but_loc_homnay.Style.Add("background-color", "#ebebeb");
        but_loc_homqua.Style.Add("background-color", "#ebebeb");
        but_loc_tuantruoc.Style.Add("background-color", "#ebebeb");
        but_loc_tuannay.Style.Add("background-color", "#ebebeb");
        but_loc_thangtruoc.Style.Add("background-color", "#ebebeb");
        but_loc_thangnay.Style.Add("background-color", "#ebebeb");
        but_loc_namtruoc.Style.Add("background-color", "#ebebeb");
        but_loc_namnay.Style.Add("background-color", "#ebebeb");
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thời gian thành công.", "4000", "warning"), true);
    }

    protected void but_loc_homqua_Click(object sender, EventArgs e)
    {
        tonhientai = 0; tondauky = 0;
        txt_tungay.Text = DateTime.Now.Date.AddDays(-1).ToString();
        txt_denngay.Text = DateTime.Now.Date.AddDays(-1).ToString();
        Session["tungay_sqtm"] = txt_tungay.Text;
        Session["denngay_sqtm"] = txt_denngay.Text;
        but_loc_homqua.Style.Add("background-color", "#cecece");

        but_loc_homnay.Style.Add("background-color", "#ebebeb");
        but_loc_tuantruoc.Style.Add("background-color", "#ebebeb");
        but_loc_tuannay.Style.Add("background-color", "#ebebeb");
        but_loc_thangtruoc.Style.Add("background-color", "#ebebeb");
        but_loc_thangnay.Style.Add("background-color", "#ebebeb");
        but_loc_namtruoc.Style.Add("background-color", "#ebebeb");
        but_loc_namnay.Style.Add("background-color", "#ebebeb");
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thời gian thành công.", "4000", "warning"), true);
    }

    protected void but_loc_homnay_Click(object sender, EventArgs e)
    {
        tonhientai = 0; tondauky = 0;
        txt_tungay.Text = DateTime.Now.Date.ToString();
        txt_denngay.Text = DateTime.Now.Date.ToString();
        Session["tungay_sqtm"] = txt_tungay.Text;
        Session["denngay_sqtm"] = txt_denngay.Text;
        but_loc_homnay.Style.Add("background-color", "#cecece");

        but_loc_homqua.Style.Add("background-color", "#ebebeb");
        but_loc_tuantruoc.Style.Add("background-color", "#ebebeb");
        but_loc_tuannay.Style.Add("background-color", "#ebebeb");
        but_loc_thangtruoc.Style.Add("background-color", "#ebebeb");
        but_loc_thangnay.Style.Add("background-color", "#ebebeb");
        but_loc_namtruoc.Style.Add("background-color", "#ebebeb");
        but_loc_namnay.Style.Add("background-color", "#ebebeb");
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thời gian thành công.", "4000", "warning"), true);
    }

    protected void but_loc_tuantruoc_Click(object sender, EventArgs e)
    {
        tonhientai = 0; tondauky = 0;
        txt_tungay.Text = dt_cl.return_ngaydautuan().AddDays(-7).ToString();//lấy ngày đầu tuần
        txt_denngay.Text = dt_cl.return_ngaydautuan().AddDays(-1).ToString();
        Session["tungay_sqtm"] = txt_tungay.Text;
        Session["denngay_sqtm"] = txt_denngay.Text;
        but_loc_tuantruoc.Style.Add("background-color", "#cecece");

        but_loc_homqua.Style.Add("background-color", "#ebebeb");
        but_loc_tuannay.Style.Add("background-color", "#ebebeb");
        but_loc_thangnay.Style.Add("background-color", "#ebebeb");
        but_loc_thangtruoc.Style.Add("background-color", "#ebebeb");
        but_loc_homnay.Style.Add("background-color", "#ebebeb");
        but_loc_namtruoc.Style.Add("background-color", "#ebebeb");
        but_loc_namnay.Style.Add("background-color", "#ebebeb");
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thời gian thành công.", "4000", "warning"), true);
    }

    protected void but_loc_tuannay_Click(object sender, EventArgs e)
    {
        tonhientai = 0; tondauky = 0;
        txt_tungay.Text = dt_cl.return_ngaydautuan().ToString();//lấy ngày đầu tuần
        txt_denngay.Text = dt_cl.return_ngaycuoituan().ToString();
        Session["tungay_sqtm"] = txt_tungay.Text;
        Session["denngay_sqtm"] = txt_denngay.Text;
        but_loc_tuannay.Style.Add("background-color", "#cecece");

        but_loc_homqua.Style.Add("background-color", "#ebebeb");
        but_loc_tuantruoc.Style.Add("background-color", "#ebebeb");
        but_loc_thangnay.Style.Add("background-color", "#ebebeb");
        but_loc_thangtruoc.Style.Add("background-color", "#ebebeb");
        but_loc_homnay.Style.Add("background-color", "#ebebeb");
        but_loc_namtruoc.Style.Add("background-color", "#ebebeb");
        but_loc_namnay.Style.Add("background-color", "#ebebeb");
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thời gian thành công.", "4000", "warning"), true);
    }

    protected void but_loc_thangnay_Click(object sender, EventArgs e)
    {
        tonhientai = 0; tondauky = 0;
        txt_tungay.Text = dt_cl.return_ngaydauthang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoithang(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        Session["tungay_sqtm"] = txt_tungay.Text;
        Session["denngay_sqtm"] = txt_denngay.Text;
        but_loc_thangnay.Style.Add("background-color", "#cecece");

        but_loc_homqua.Style.Add("background-color", "#ebebeb");
        but_loc_tuantruoc.Style.Add("background-color", "#ebebeb");
        but_loc_tuannay.Style.Add("background-color", "#ebebeb");
        but_loc_thangtruoc.Style.Add("background-color", "#ebebeb");
        but_loc_homnay.Style.Add("background-color", "#ebebeb");
        but_loc_namtruoc.Style.Add("background-color", "#ebebeb");
        but_loc_namnay.Style.Add("background-color", "#ebebeb");
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thời gian thành công.", "4000", "warning"), true);
    }

    protected void but_loc_thangtruoc_Click(object sender, EventArgs e)
    {
        tonhientai = 0; tondauky = 0;
        txt_tungay.Text = dt_cl.return_ngaydauthangtruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoithangtruoc(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()).ToString();
        Session["tungay_sqtm"] = txt_tungay.Text;
        Session["denngay_sqtm"] = txt_denngay.Text;
        but_loc_thangtruoc.Style.Add("background-color", "#cecece");

        but_loc_homqua.Style.Add("background-color", "#ebebeb");
        but_loc_tuantruoc.Style.Add("background-color", "#ebebeb");
        but_loc_tuannay.Style.Add("background-color", "#ebebeb");
        but_loc_thangnay.Style.Add("background-color", "#ebebeb");
        but_loc_homnay.Style.Add("background-color", "#ebebeb");
        but_loc_namtruoc.Style.Add("background-color", "#ebebeb");
        but_loc_namnay.Style.Add("background-color", "#ebebeb");
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thời gian thành công.", "4000", "warning"), true);
    }

    protected void but_loc_namnay_Click(object sender, EventArgs e)
    {
        tonhientai = 0; tondauky = 0;
        txt_tungay.Text = dt_cl.return_ngaydaunam(DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoinam(DateTime.Now.Year.ToString()).ToString();
        Session["tungay_sqtm"] = txt_tungay.Text;
        Session["denngay_sqtm"] = txt_denngay.Text;
        but_loc_namnay.Style.Add("background-color", "#cecece");

        but_loc_homqua.Style.Add("background-color", "#ebebeb");
        but_loc_tuantruoc.Style.Add("background-color", "#ebebeb");
        but_loc_tuannay.Style.Add("background-color", "#ebebeb");
        but_loc_thangnay.Style.Add("background-color", "#ebebeb");
        but_loc_homnay.Style.Add("background-color", "#ebebeb");
        but_loc_namtruoc.Style.Add("background-color", "#ebebeb");
        but_loc_thangtruoc.Style.Add("background-color", "#ebebeb");
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thời gian thành công.", "4000", "warning"), true);
    }

    protected void but_loc_namtruoc_Click(object sender, EventArgs e)
    {
        tonhientai = 0; tondauky = 0;
        txt_tungay.Text = dt_cl.return_ngaydaunamtruoc(DateTime.Now.Year.ToString()).ToString();
        txt_denngay.Text = dt_cl.return_ngaycuoinamtruoc(DateTime.Now.Year.ToString()).ToString();
        Session["tungay_sqtm"] = txt_tungay.Text;
        Session["denngay_sqtm"] = txt_denngay.Text;
        but_loc_namtruoc.Style.Add("background-color", "#cecece");

        but_loc_homqua.Style.Add("background-color", "#ebebeb");
        but_loc_tuantruoc.Style.Add("background-color", "#ebebeb");
        but_loc_tuannay.Style.Add("background-color", "#ebebeb");
        but_loc_thangnay.Style.Add("background-color", "#ebebeb");
        but_loc_homnay.Style.Add("background-color", "#ebebeb");
        but_loc_thangtruoc.Style.Add("background-color", "#ebebeb");
        but_loc_namnay.Style.Add("background-color", "#ebebeb");
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lọc thời gian thành công.", "4000", "warning"), true);
    }


    protected void but_locdulieu_Click(object sender, EventArgs e)
    {

    }
}
