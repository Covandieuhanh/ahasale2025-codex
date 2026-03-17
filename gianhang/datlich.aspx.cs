using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class chitiettintuc : System.Web.UI.Page
{
    public string notifi, meta, title_web, url_back;
    dbDataContext db = new dbDataContext();
    poss_class_homeaka po_cl = new poss_class_homeaka();

    protected void Page_Load(object sender, EventArgs e)
    {
        AhaShineContext_cl.EnsureContext();
        Session["ten_tk_chinhanh"] = AhaShineContext_cl.UserParent;
        Session["id_chinhanh_webcon"] = AhaShineContext_cl.ResolveChiNhanhId();

        #region meta
        var q = db.config_lienket_chiase_tables;
        if (q.Count() != 0)
        {
            title_web = q.First().title;
            string _description = "<meta name=\"description\" content=\"" + q.First().description + "\" />";

            string _title_op = "<meta property=\"og:title\" content=\"" + "Đặt lịch hẹn" + "\" />";
            string _image = "<meta property=\"og:image\" content=\"" + q.First().image + "\" />";
            string _description_op = "<meta property=\"og:description\" content=\"" + "" + "\" />";

            meta = _title_op + _image + _description + _description_op;
        }
        #endregion

        if (Request.Cookies[app_cookie_policy_class.home_return_url_cookie] != null)
            url_back = AhaShineHomeRoutes_cl.NormalizeReturnUrl(Request.Cookies[app_cookie_policy_class.home_return_url_cookie].Value);
        else
            url_back = AhaShineHomeRoutes_cl.HomeUrl;

        if (!IsPostBack)
        {
            txt_ngay.Text = datlich_class.return_ngay_text(DateTime.Now);
            datlich_class.bind_gio_phut(ddl_giobatdau, ddl_phutbatdau, DateTime.Now);

            var list_dv = (from ob1 in db.web_post_tables
                               .Where(p => p.phanloai == "ctdv" && p.bin == false && p.id_chinhanh == AhaShineContext_cl.ResolveChiNhanhId())
                               .ToList()
                           select new
                           {
                               iddv = ob1.id,
                               tendv = ob1.name,
                           });

            DropDownList1.DataSource = list_dv;
            DropDownList1.DataTextField = "tendv";
            DropDownList1.DataValueField = "iddv";
            DropDownList1.DataBind();
            DropDownList1.Items.Insert(0, new ListItem("Chọn", ""));

            AutoFillThongTinKhachHang();

            if (!String.IsNullOrWhiteSpace(Request.QueryString["id"]))
            {
                string _id = Request.QueryString["id"];
                if (po_cl.exist_id(_id))
                {
                    string _phanloai = po_cl.return_object(_id).phanloai;

                    if (_phanloai == "ctdv")
                    {
                        datlich_class.try_select_dropdown_value(DropDownList1, _id);
                    }
                    else
                    {
                        if (_phanloai == "ctsp")
                        {
                            Session["notifi_home"] = thongbao_class.metro_dialog_onload("Thông báo", "Dịch vụ không tồn tại.", "false", "false", "OK", "success", "");
                            Response.Redirect(url_back);
                        }
                        else
                        {
                            Response.Redirect(AhaShineHomeRoutes_cl.HomeUrl);
                        }
                    }
                }
            }
        }
    }

    private void AutoFillThongTinKhachHang()
    {
        string sdt_login = "";
        string id_chinhanh = AhaShineContext_cl.ResolveChiNhanhId();

        if (Session["user_home"] != null)
            sdt_login = Session["user_home"].ToString().Trim();

        if (string.IsNullOrWhiteSpace(sdt_login) && Request.Cookies["save_sdt_home_aka"] != null)
        {
            try
            {
                string enc = Request.Cookies["save_sdt_home_aka"].Value;
                sdt_login = encode_class.decrypt(enc);
            }
            catch
            {
                sdt_login = "";
            }
        }

        if (string.IsNullOrWhiteSpace(sdt_login))
            return;

        if (string.IsNullOrWhiteSpace(txt_sdt.Text))
            txt_sdt.Text = sdt_login;

        var q_kh = db.bspa_data_khachhang_tables.Where(p => p.sdt == sdt_login && p.id_chinhanh == id_chinhanh).OrderByDescending(p => p.ngaytao);
        if (q_kh.Count() != 0 && string.IsNullOrWhiteSpace(txt_hoten.Text))
        {
            txt_hoten.Text = q_kh.First().tenkhachhang;
        }
    }

    protected void but_dathang_Click(object sender, EventArgs e)
    {
        string id_chinhanh = AhaShineContext_cl.ResolveChiNhanhId();
        string nguoitao = datlich_class.nguoitao_khach_online;
        datlich_validate_result _kq = datlich_class.chuanhoa_du_lieu(
            txt_hoten.Text,
            txt_sdt.Text,
            txt_ngay.Text,
            ddl_giobatdau.SelectedValue,
            ddl_phutbatdau.SelectedValue,
            DropDownList1.SelectedValue,
            "",
            txt_ghichu.Text,
            datlich_class.trangthai_chua_xacnhan,
            "Web",
            "Web",
            false
        );

        if (_kq.hop_le == false)
        {
            notifi = thongbao_class.metro_dialog_onload("Thông báo", _kq.thongbao, "false", "false", "OK", "primary", "");
        }
        else
        {
            string _loi_vanhanh = datlich_class.kiemtra_quy_tac_van_hanh(db, _kq.dulieu, id_chinhanh, null, true);
            if (_loi_vanhanh != "")
            {
                notifi = thongbao_class.metro_dialog_onload("Thông báo", _loi_vanhanh, "false", "false", "OK", "primary", "");
                return;
            }

            bspa_datlich_table _ob = new bspa_datlich_table();
            datlich_class.gan_du_lieu_vao_lich(db, _ob, _kq.dulieu, nguoitao, id_chinhanh, false);
            db.bspa_datlich_tables.InsertOnSubmit(_ob);
            db.SubmitChanges();

            khachhang_nhatky_class.ghi_su_kien(
                db,
                _ob.sdt,
                id_chinhanh,
                _ob.nguoitao,
                khachhang_nhatky_class.tao_noidung_tao_lich(_ob.id.ToString(), _kq.dulieu, _ob.tendichvu_taithoidiemnay),
                _ob.ngaytao
            );

            thongbao_table _ob1 = new thongbao_table();
            _ob1.id = Guid.NewGuid();
            _ob1.daxem = false;
            _ob1.nguoithongbao = "admin";
            _ob1.nguoinhan = "admin";
            _ob1.link = "/gianhang/admin/quan-ly-khach-hang/danh-sach-lich-hen.aspx";
            _ob1.noidung = _kq.dulieu.tenkhachhang + " vừa đặt lịch tại website";
            _ob1.thoigian = DateTime.Now;
            db.thongbao_tables.InsertOnSubmit(_ob1);
            db.SubmitChanges();

            string _id_datlich = _ob.id.ToString();

            Session["notifi_home"] = thongbao_class.metro_dialog_onload(
                "Đặt lịch thành công",
                "Mã đặt lịch: <b>" + _id_datlich + "</b><br/>Xin cám ơn và hẹn gặp lại.",
                "false", "false", "OK", "success", ""
            );

            Response.Redirect(AhaShineHomeRoutes_cl.HomeUrl);
        }
    }
}
