using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class thong_ke_bao_cao_inbienban : System.Web.UI.Page
{
    public string  idtc = "", user, user_parent, nhom,tenphieu;
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    nhomthuchi_class ntc_cl = new nhomthuchi_class();

    public string logo_hoadon, tencty, diachi, sdt,p;

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
        #region Check quyen theo nganh
        user = Session["user"].ToString();
        user_parent = "admin";
        if (bcorn_class.check_quyen(user, "q9_1") == "" || bcorn_class.check_quyen(user, "n9_1") == "")
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
            {
                idtc = Request.QueryString["id"];
                var q = db.bspa_thuchi_tables.Where(p => p.id.ToString() == idtc && p.user_parent == user_parent && p.id_chinhanh == Session["chinhanh"].ToString());
                if (q.Count() != 0)
                {
                    config_thongtin_table _ob999 = db.config_thongtin_tables.First();
                    tencty = _ob999.tencongty;

                    var q1 = db.config_thongtin_tables;
                    if (q1.Count() != 0)
                    {
                        config_thongtin_table _ob1 = q1.First();
                        tencty = _ob1.tencongty;
                        diachi = _ob1.diachi;
                        sdt = _ob1.hotline;
                        logo_hoadon = _ob1.logo_in_hoadon;
                    }

                    nhom = ntc_cl.return_object(q.First().id_nhomthuchi).tennhom;
                    lb_hoten.Text = q.First().nguoilapphieu;
                    lb_nguoinhan.Text = q.First().nguoinhantien == "" ? "" : tk_cl.return_object(q.First().nguoinhantien).hoten;
                    lb_noidung.Text = q.First().noidung;
                    lb_sotien.Text = q.First().sotien.Value.ToString("#,##0") + " VNĐ";
                    lb_ngaythuchi.Text = "Ngày " + q.First().ngay.Value.Day + " tháng " + q.First().ngay.Value.Month + " năm " + q.First().ngay.Value.Year;
                    lb_tienbangchu.Text = number_class.number_to_text_unlimit(q.First().sotien.Value.ToString()) + " đồng.";
                    tenphieu = q.First().thuchi == "Thu" ? "PHIẾU THU" : "PHIẾU CHI";

                    p = "<script>window.onload = function () {window.print();};</script>";
                }
                else
                {
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                    Response.Redirect("/gianhang/admin/Default.aspx");
                }
            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin/Default.aspx");
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        #endregion

        
    }
}