using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_Default : System.Web.UI.Page
{
    dbDataContext db = new dbDataContext();
    datetime_class dt_cl = new datetime_class();
    public string notifi;
    private config_baotri_table EnsureBaoTri()
    {
        config_baotri_table ob = db.config_baotri_tables.FirstOrDefault();
        if (ob != null)
            return ob;

        ob = new config_baotri_table
        {
            baotri_trangthai = false,
            baotri_thoigian_batdau = null,
            baotri_thoigian_ketthuc = null,
            ghichu = ""
        };
        db.config_baotri_tables.InsertOnSubmit(ob);
        db.SubmitChanges();
        return ob;
    }
    public void main()
    {
        config_baotri_table _ob = EnsureBaoTri();
        if (!IsPostBack)
        {
            if (_ob.baotri_trangthai == true)
                DropDownList1.SelectedIndex = 1;
            else
                DropDownList1.SelectedIndex = 0;
            txt_ngay_batdau.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txt_ngay_ketthuc.Text = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
            for (int i = 23; i >= 0; i--)
            {
                if (i < 10)
                {
                    ddl_giobatdau.Items.Insert(0, new ListItem("0" + i + " giờ", "0" + i));
                    ddl_gioketthuc.Items.Insert(0, new ListItem("0" + i + " giờ", "0" + i));
                }
                else
                {
                    ddl_giobatdau.Items.Insert(0, new ListItem(i + " giờ", i.ToString()));
                    ddl_gioketthuc.Items.Insert(0, new ListItem(i + " giờ", i.ToString()));
                }
            }
            for (int i = 59; i >= 0; i--)
            {
                if (i < 10)
                {
                    ddl_phutbatdau.Items.Insert(0, new ListItem("0" + i + " phút", "0" + i));
                    ddl_phutketthuc.Items.Insert(0, new ListItem("0" + i + " phút", "0" + i));
                }
                else
                {
                    ddl_phutbatdau.Items.Insert(0, new ListItem(i + " phút", i.ToString()));
                    ddl_phutketthuc.Items.Insert(0, new ListItem(i + " phút", i.ToString()));
                }
            }
            if (_ob.baotri_trangthai == true && _ob.baotri_thoigian_batdau.HasValue && _ob.baotri_thoigian_ketthuc.HasValue)
            {
                txt_ngay_batdau.Text = _ob.baotri_thoigian_batdau.Value.ToShortDateString();
                txt_ngay_ketthuc.Text = _ob.baotri_thoigian_ketthuc.Value.ToShortDateString();
                ddl_giobatdau.SelectedIndex = _ob.baotri_thoigian_batdau.Value.Hour;
                ddl_phutbatdau.SelectedIndex = _ob.baotri_thoigian_batdau.Value.Minute;
                ddl_gioketthuc.SelectedIndex = _ob.baotri_thoigian_ketthuc.Value.Hour;
                ddl_phutketthuc.SelectedIndex = _ob.baotri_thoigian_ketthuc.Value.Minute;
            }
        }

        string _check_baotri = DropDownList1.SelectedValue.ToString();
        if (_check_baotri == "0")
            Panel2.Enabled = false;
        else
            Panel2.Enabled = true;

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login
        string _quyen = "q1_5";
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

        if (!IsPostBack)
            main();
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q1_5") == "2")
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        else
        {


            string _check_baotri = DropDownList1.SelectedValue.ToString();
            if (_check_baotri == "0")
            {
                cauhinhchung_class.update_thoigian_baotri(false, "0", "0");
                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/cau-hinh-chung/cai-dat-bao-tri.aspx");
            }
            else
            {
                string _ngay_batdau = txt_ngay_batdau.Text;
                string _gio_batdau = ddl_giobatdau.SelectedValue.ToString();
                string _phut_batdau = ddl_phutbatdau.SelectedValue.ToString();
                string _ngaygio_batdau = _ngay_batdau + " " + _gio_batdau + ":" + _phut_batdau;

                string _ngay_ketthuc = txt_ngay_ketthuc.Text;
                string _gio_ketthuc = ddl_gioketthuc.SelectedValue.ToString();
                string _phut_ketthuc = ddl_phutketthuc.SelectedValue.ToString();
                string _ngaygio_ketthuc = _ngay_ketthuc + " " + _gio_ketthuc + ":" + _phut_ketthuc;

                if (!dt_cl.check_date(_ngaygio_batdau))
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Ngày giờ bắt đầu không hợp lệ.", "4000", "warning"), true);
                else
                {
                    if (!dt_cl.check_date(_ngaygio_ketthuc))
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Ngày giờ kết thúc không hợp lệ.", "4000", "warning"), true);
                    else
                    {
                        DateTime _batdau = DateTime.Parse(_ngaygio_batdau);
                        DateTime _ketthuc = DateTime.Parse(_ngaygio_ketthuc);
                        if (_batdau >= _ketthuc)
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc.", "4000", "warning"), true);
                        else
                        {
                            cauhinhchung_class.update_thoigian_baotri(true, _ngaygio_batdau, _ngaygio_ketthuc);
                            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Cập nhật thành công.", "4000", "warning");
                            Response.Redirect("/gianhang/admin/cau-hinh-chung/cai-dat-bao-tri.aspx");
                        }
                    }
                }
            }

        }
    }
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        main();
    }
}
