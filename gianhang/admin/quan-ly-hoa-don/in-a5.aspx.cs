using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class gianhang_hoa_don_in_a5_Default : System.Web.UI.Page
{
    public string id, p;
    dbDataContext db = new dbDataContext();
    taikhoan_class tk_cl = new taikhoan_class();
    hoadon_class hd_cl = new hoadon_class();
    public string nguoixuat, logo_hoadon, user, user_parent, tencty, diachi, sdt, ngaytao,ten_kh,sdt_kh,diachi_kh,tongtien,ck,sauck,tien_dathanhtoan,tien_conlai,bangchu, km1_ghichu;
   
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
        user_parent = GianHangAdminContext_cl.ResolveCurrentOwnerAccountKey();
        string chinhanhId = (Session["chinhanh"] ?? "1").ToString();

        var q_cn = db.chinhanh_tables.Where(p => p.id.ToString() == chinhanhId);
        if (q_cn.Count() != 0)
        {
            chinhanh_table _ob1 = q_cn.First();
            tencty = _ob1.ten;
            diachi = _ob1.diachi;
            sdt = _ob1.sdt;
            logo_hoadon = _ob1.logo_hoadon;
        }

        if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
        {
            id = Request.QueryString["id"].ToString().Trim();
            if (hd_cl.exist_id(id, user_parent))
            {
                var q_taikhoan = db.taikhoan_table_2023s.Where(p => p.taikhoan == user && (string.IsNullOrWhiteSpace(chinhanhId) || p.id_chinhanh == chinhanhId));
                if (q_taikhoan.Count() != 0)
                    nguoixuat = q_taikhoan.First().hoten;

                config_thongtin_table _ob999 = db.config_thongtin_tables.FirstOrDefault();
                //tencty = _ob999.tencongty;

                var q = db.config_thongtin_tables;
                //if (q.Count() != 0)
                //{
                //    config_thongtin_table _ob1 = q.First();
                //    tencty = _ob1.tencongty;
                //    diachi = _ob1.diachi;
                //    sdt = _ob1.hotline;
                //    logo_hoadon = _ob1.logo_in_hoadon;
                //}

                bspa_hoadon_table _ob = hd_cl.return_object(id);
                if (_ob == null)
                {
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Không tìm thấy hóa đơn cần in.", "false", "false", "OK", "alert", "");
                    Response.Redirect("/gianhang/admin/Default.aspx");
                    return;
                }
                ngaytao = _ob.ngaytao.Value.ToString("dd/MM/yyyy HH:mm") + "'";
                ten_kh = _ob.tenkhachhang;
                sdt_kh = _ob.sdt;
                diachi_kh = _ob.diachi;
                tongtien = _ob.tongtien.Value.ToString("#,##0");
                ck = _ob.chietkhau.Value.ToString();
                sauck = _ob.tongsauchietkhau.Value.ToString("#,##0");
                tien_dathanhtoan = _ob.sotien_dathanhtoan.Value.ToString("#,##0");
                tien_conlai = _ob.sotien_conlai.Value.ToString("#,##0");
                bangchu = _ob.tongsauchietkhau.Value == 0 ? "0" : number_class.number_to_text_unlimit(_ob.tongsauchietkhau.Value.ToString());
                km1_ghichu = _ob.km1_ghichu;

                var list_all = (from ob1 in db.bspa_hoadon_chitiet_tables.Where(p => p.user_parent == user_parent && p.id_hoadon == id && (string.IsNullOrWhiteSpace(chinhanhId) || p.id_chinhanh == chinhanhId)).ToList()
                                //join ob2 in db.bspa_hoadon_tables.ToList() on ob1.id_hoadon equals ob2.id.ToString()
                                select new
                                {
                                    id = ob1.id,
                                    ten_dichvu_sanpham = ob1.ten_dvsp_taithoidiemnay,
                                    gia = ob1.gia_dvsp_taithoidiemnay,
                                    soluong = ob1.soluong,
                                    thanhtien = ob1.thanhtien,
                                    chietkhau = ob1.chietkhau,
                                    sauck = ob1.tongsauchietkhau,
                                });
                Repeater1.DataSource = list_all;
                Repeater1.DataBind();

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
}
