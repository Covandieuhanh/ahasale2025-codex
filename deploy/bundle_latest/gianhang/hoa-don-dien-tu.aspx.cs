using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class quan_ly_ban_hang_thuong_quy_cap_duoi : System.Web.UI.Page
{
    public string id, p, meta;
    dbDataContext db = new dbDataContext();
    taikhoan_class tk_cl = new taikhoan_class();
    hoadon_class hd_cl = new hoadon_class();
    public string id_guide, nguoixuat, logo_hoadon, user, user_parent, tencty, diachi, sdt, ngaytao,ten_kh,sdt_kh,diachi_kh,tongtien,ck,sauck,tien_dathanhtoan,tien_conlai,bangchu, km1_ghichu;
   
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
        {
           

            id_guide = Request.QueryString["id"].ToString().Trim();
            var q1 = db.bspa_hoadon_tables.Where(p => p.id_guide.ToString().ToLower() == id_guide.ToLower().Trim());
            if (q1.Count()!=0)
            {
                id = q1.First().id.ToString();
                //var q_taikhoan = db.taikhoan_table_2023s.Where(p=>p.taikhoan== user);
                //if (q_taikhoan.Count() != 0)
                //    nguoixuat = q_taikhoan.First().hoten;

                config_thongtin_table _ob999 = db.config_thongtin_tables.First();
                tencty = _ob999.tencongty;

                var q = db.config_thongtin_tables;
                if (q.Count() != 0)
                {
                    config_thongtin_table _ob1 = q.First();
                    tencty = _ob1.tencongty;
                    diachi = _ob1.diachi;
                    sdt = _ob1.hotline;
                    logo_hoadon = _ob1.logo_in_hoadon;
                }

                bspa_hoadon_table _ob = q1.First();
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

                var list_all = (from ob1 in db.bspa_hoadon_chitiet_tables.Where(p => p.id_hoadon == id).ToList()
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


                #region meta
                var q123 = db.config_thongtin_tables;
                if (q123.Count() != 0)
                {
                    string _icon = "<link rel=\"shortcut icon\" type=\"image/x-icon\" href=\"" + q123.First().icon + "\" />";
                    string _appletouch = "<link rel=\"apple-touch-icon\" href=\"" + q123.First().apple_touch_icon + "\" />";

                    this.Title = "Hóa đơn điện tử";

                    string _title_op = "<meta property=\"og:title\" content=\"" + "Hóa đơn điện tử" + "\" />";
                    string _image = "<meta property=\"og:image\" content=\"" + "/uploads/images/hoa-don-dien-tu.jpg" + "\" />";
                    string _description = "<meta name=\"description\" content=\"" + "Khách hàng: " + ten_kh + "\" />";
                    string _description_op = "<meta property=\"og:description\" content=\"" + "Khách hàng: " + ten_kh + "\" />";
                    meta = _title_op + _image + _description + _description_op + _icon + _appletouch;

                    meta = _icon + _appletouch;
                }
                #endregion

                //p = "<script>window.onload = function () {window.print();};</script>";
            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect(AhaShineHomeRoutes_cl.HomeUrl);
            }
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
            Response.Redirect(AhaShineHomeRoutes_cl.HomeUrl);
        }
    }
}