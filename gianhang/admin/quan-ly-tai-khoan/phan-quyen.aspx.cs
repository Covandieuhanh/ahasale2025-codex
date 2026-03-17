using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_Default : System.Web.UI.Page
{
    dbDataContext db = new dbDataContext(); chinhanh_class cn_cl = new chinhanh_class();
    public string notifi, user_phanquyen, user, user_parent, url_back, tenchinhanh;
    taikhoan_class tk_cl = new taikhoan_class();

    //trang chủ
    public string q0_1, q0_2, q0_3, q0_4, n0_1, n0_2, n0_3, n0_4;
    //cấu hình chung
    public string q1_1, q1_2, q1_3, q1_4, q1_5;
    //quản lý tài khoản
    public string q2_1, q2_2, q2_3, q2_4, q2_5, q2_6, q2_7, q2_8, q2_9, q2_10, n2_1, n2_2, n2_3, n2_4, n2_5, n2_6, n2_7, n2_8, n2_9, n2_10;
    //quản lý menu
    public string q3_1, q3_2, q3_3, q3_4, q3_5, q3_6;
    //quản lý bài viết
    public string q4_1, q4_2, q4_3, q4_4, q4_5, q4_6;//, q4_7;
    //quản lý module
    public string q5_1;
    //data yêu cầu tư vấn
    public string q6_1;
    //quản lý hóa đơn
    public string q7_1, q7_2, q7_3, q7_4, q7_5, q7_6, q7_7, q7_8, q7_9, q7_10, n7_1, n7_2, n7_3, n7_4, n7_5, n7_6, n7_7, n7_8, n7_9, n7_10;
    //data khách hàng
    public string q8_1, q8_2, q8_3, q8_4, q8_5, n8_1, n8_2, n8_3, n8_4, n8_5;
    //quản lý thu chi
    public string q9_1, q9_2, q9_3, q9_4, q9_5, q9_6, n9_1, n9_2, n9_3, n9_4, n9_5, n9_6;
    //lịch hẹn
    public string q10_1, q10_2, q10_3, q10_4;
    //kho hàng
    public string q11_1, q11_2, q11_3, q11_4, q11_5, q11_6, q11_7, q11_8;
    //thẻ dịch vụ
    public string q12_1, q12_2, q12_3, q12_4, q12_5, n12_1, n12_2, n12_3, n12_4, n12_5;
    //kho vật tư
    public string q13_1, q13_2, q13_3, q13_4, q13_5, q13_6, q13_7, q13_8, q13_9;
    //ql thành viên
    public string q14_1, q14_2, q14_3, q14_4, q14_5, n14_1, n14_2, n14_3, n14_4, n14_5;
    //ql Chuyên gia
    public string q15_1, q15_2, q15_3, q15_4, n15_1, n15_2, n15_3, n15_4;
    //ql hệ thống
    public string q16_0, q16_1, q16_2;

    //THÔNG BÁO
    public string tb1;

   
    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login
        string _quyen = "none";
        string _cookie_user = "", _cookie_pass = "";
        if (Request.Cookies["save_user_admin_aka_1"] != null) _cookie_user = Request.Cookies["save_user_admin_aka_1"].Value;
        if (Request.Cookies["save_pass_admin_aka_1"] != null) _cookie_pass = Request.Cookies["save_pass_admin_aka_1"].Value;
        if (Session["user"] == null) Session["user"] = ""; if (Session["notifi"] == null) Session["notifi"] = ""; if (Session["user"].ToString() == "") Response.Redirect("/gianhang/admin/f5_ss_admin.aspx");
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
        if (bcorn_class.check_quyen(user, "q2_1") == "" || bcorn_class.check_quyen(user, "n2_1") == "")
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["user"]))
            {
                user_phanquyen = Request.QueryString["user"].ToString().Trim();
                if (tk_cl.exist_user(user_phanquyen))
                {
                    if (user_phanquyen == "admin" && Session["user"].ToString() != "admin")
                    {
                        Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không thể phân quyền cho admin.", "false", "false", "OK", "alert", "");
                        Response.Redirect("/gianhang/admin");
                    }
                    else
                    {
                        var q = tk_cl.return_object(user_phanquyen);
                        tenchinhanh = cn_cl.return_name(q.id_chinhanh);

                        //SHOW CHECK QUYỀN
                        //TRANG CHỦ
                        if (bcorn_class.check_quyen(user_phanquyen, "q0_1") == "") q0_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q0_2") == "") q0_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q0_3") == "") q0_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q0_4") == "") q0_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n0_1") == "") n0_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n0_2") == "") n0_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n0_3") == "") n0_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n0_4") == "") n0_4 = "checked";

                        //CẤU HÌNH CHUNG
                        //tạo liên kết chia sẻ
                        if (bcorn_class.check_quyen(user_phanquyen, "q1_1") == "") q1_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q1_2") == "") q1_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q1_3") == "") q1_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q1_4") == "") q1_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q1_5") == "") q1_5 = "checked";

                        //QUẢN LÝ TÀI KHOẢN
                        //được phân quyền tài khoản
                        if (bcorn_class.check_quyen(user_phanquyen, "q2_1") == "") q2_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q2_2") == "") q2_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q2_3") == "") q2_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q2_4") == "") q2_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q2_5") == "") q2_5 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q2_6") == "") q2_6 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q2_7") == "") q2_7 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q2_8") == "") q2_8 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q2_9") == "") q2_9 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q2_10") == "") q2_10 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n2_1") == "") n2_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n2_2") == "") n2_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n2_3") == "") n2_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n2_4") == "") n2_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n2_5") == "") n2_5 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n2_6") == "") n2_6 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n2_7") == "") n2_7 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n2_8") == "") n2_8 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n2_9") == "") n2_9 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n2_10") == "") n2_10 = "checked";

                        //QUẢN LÝ MENU
                        if (bcorn_class.check_quyen(user_phanquyen, "q3_1") == "") q3_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q3_2") == "") q3_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q3_3") == "") q3_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q3_4") == "") q3_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q3_5") == "") q3_5 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q3_6") == "") q3_6 = "checked";

                        //QUẢN LÝ bài viết
                        if (bcorn_class.check_quyen(user_phanquyen, "q4_1") == "") q4_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q4_2") == "") q4_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q4_3") == "") q4_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q4_4") == "") q4_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q4_5") == "") q4_5 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q4_6") == "") q4_6 = "checked";
                        //if (bcorn_class.check_quyen(user_phanquyen, "q4_7") == "") q4_7 = "checked";

                        //QUẢN LÝ MODULE
                        if (bcorn_class.check_quyen(user_phanquyen, "q5_1") == "") q5_1 = "checked";//slide ảnh

                        //data yêu cầu tư vấn
                        if (bcorn_class.check_quyen(user_phanquyen, "q6_1") == "") q6_1 = "checked";

                        //QUẢN LÝ HÓA ĐƠN
                        if (bcorn_class.check_quyen(user_phanquyen, "q7_1") == "") q7_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q7_2") == "") q7_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q7_3") == "") q7_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q7_4") == "") q7_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q7_5") == "") q7_5 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q7_6") == "") q7_6 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q7_7") == "") q7_7 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q7_8") == "") q7_8 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q7_9") == "") q7_9 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q7_10") == "") q7_10 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n7_1") == "") n7_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n7_2") == "") n7_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n7_3") == "") n7_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n7_4") == "") n7_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n7_5") == "") n7_5 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n7_6") == "") n7_6 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n7_7") == "") n7_7 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n7_8") == "") n7_8 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n7_9") == "") n7_9 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n7_10") == "") n7_10 = "checked";


                        //DATA KHÁCH HÀNG
                        if (bcorn_class.check_quyen(user_phanquyen, "q8_1") == "") q8_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q8_2") == "") q8_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q8_3") == "") q8_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q8_4") == "") q8_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q8_5") == "") q8_5 = "checked";
                        //if (bcorn_class.check_quyen(user_phanquyen, "n8_1") == "") n8_1 = "checked";
                        //if (bcorn_class.check_quyen(user_phanquyen, "n8_2") == "") n8_2 = "checked";
                        //if (bcorn_class.check_quyen(user_phanquyen, "n8_3") == "") n8_3 = "checked";
                        //if (bcorn_class.check_quyen(user_phanquyen, "n8_4") == "") n8_4 = "checked";
                        //if (bcorn_class.check_quyen(user_phanquyen, "n8_5") == "") n8_5 = "checked";

                        //QUẢN LÝ THU CHI
                        if (bcorn_class.check_quyen(user_phanquyen, "q9_1") == "") q9_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q9_2") == "") q9_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q9_3") == "") q9_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q9_4") == "") q9_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q9_5") == "") q9_5 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q9_6") == "") q9_6 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n9_1") == "") n9_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n9_2") == "") n9_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n9_3") == "") n9_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n9_4") == "") n9_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n9_5") == "") n9_5 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n9_6") == "") n9_6 = "checked";

                        //LỊCH HẸN
                        if (bcorn_class.check_quyen(user_phanquyen, "q10_1") == "") q10_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q10_2") == "") q10_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q10_3") == "") q10_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q10_4") == "") q10_4 = "checked";

                        //KHO HÀNG
                        if (bcorn_class.check_quyen(user_phanquyen, "q11_1") == "") q11_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q11_2") == "") q11_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q11_3") == "") q11_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q11_4") == "") q11_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q11_5") == "") q11_5 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q11_6") == "") q11_6 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q11_7") == "") q11_7 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q11_8") == "") q11_8 = "checked";

                        //THẺ DỊCH VỤ
                        if (bcorn_class.check_quyen(user_phanquyen, "q12_1") == "") q12_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q12_2") == "") q12_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q12_3") == "") q12_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q12_4") == "") q12_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q12_5") == "") q12_5 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n12_1") == "") n12_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n12_2") == "") n12_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n12_3") == "") n12_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n12_4") == "") n12_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n12_5") == "") n12_5 = "checked";

                        // VẬT TƯ
                        if (bcorn_class.check_quyen(user_phanquyen, "q13_1") == "") q13_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q13_2") == "") q13_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q13_3") == "") q13_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q13_4") == "") q13_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q13_5") == "") q13_5 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q13_6") == "") q13_6 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q13_7") == "") q13_7 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q13_8") == "") q13_8 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q13_9") == "") q13_9 = "checked";

                        //thành viên
                        if (bcorn_class.check_quyen(user_phanquyen, "q14_1") == "") q14_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q14_2") == "") q14_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q14_3") == "") q14_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q14_4") == "") q14_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q14_5") == "") q14_5 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n14_1") == "") n14_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n14_2") == "") n14_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n14_3") == "") n14_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n14_4") == "") n14_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n14_5") == "") n14_5 = "checked";

                        //Chuyên gia
                        if (bcorn_class.check_quyen(user_phanquyen, "q15_1") == "") q15_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q15_2") == "") q15_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q15_3") == "") q15_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q15_4") == "") q15_4 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n15_1") == "") n15_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n15_2") == "") n15_2 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n15_3") == "") n15_3 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "n15_4") == "") n15_4 = "checked";

                        //QL hệ thống
                        if (bcorn_class.check_quyen(user_phanquyen, "q16_0") == "") q16_0 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q16_1") == "") q16_1 = "checked";
                        if (bcorn_class.check_quyen(user_phanquyen, "q16_2") == "") q16_2 = "checked";
                    }
                }
                else
                {
                    Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                    Response.Redirect("/gianhang/admin");
                }
            }
            else
            {
                Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Trang bạn yêu cầu không hợp lệ.", "false", "false", "OK", "alert", "");
                Response.Redirect("/gianhang/admin");
            }

            if (Request.Cookies["save_url_admin_aka_1"] != null)
                url_back = Request.Cookies["save_url_admin_aka_1"].Value;
            else
                url_back = "/gianhang/admin";
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
        #endregion
       

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        if (bcorn_class.check_quyen(Session["user"].ToString(), "q2_1") == "" || bcorn_class.check_quyen(Session["user"].ToString(), "n2_1") == "")
        {
            if (user_phanquyen == "admin" && Session["user"].ToString() != "admin")
                notifi = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không thể phân quyền cho admin.", "false", "false", "OK", "alert", "");
            else
            {
                string _q_quyen = "";

                //truy cập trang chủ
                string _q0_1, _q0_2, _q0_3, _q0_4, _n0_1, _n0_2, _n0_3, _n0_4;
                _q0_1 = Request.Form["q0_1"] == "on" ? "q0_1," : "";
                _q0_2 = Request.Form["q0_2"] == "on" ? "q0_2," : "";
                _q0_3 = Request.Form["q0_3"] == "on" ? "q0_3," : "";
                _q0_4 = Request.Form["q0_4"] == "on" ? "q0_4," : "";
                _n0_1 = Request.Form["n0_1"] == "on" ? "n0_1," : "";
                _n0_2 = Request.Form["n0_2"] == "on" ? "n0_2," : "";
                _n0_3 = Request.Form["n0_3"] == "on" ? "n0_3," : "";
                _n0_4 = Request.Form["n0_4"] == "on" ? "n0_4," : "";
                _q_quyen = _q_quyen + _q0_1 + _q0_2 + _q0_3 + _q0_4 + _n0_1 + _n0_2 + _n0_3 + _n0_4;

                //CẤU HÌNH CHUNG
                string _q1_1, _q1_2, _q1_3, _q1_4, _q1_5;
                _q1_1 = Request.Form["q1_1"] == "on" ? "q1_1," : "";
                _q1_2 = Request.Form["q1_2"] == "on" ? "q1_2," : "";
                _q1_3 = Request.Form["q1_3"] == "on" ? "q1_3," : "";
                _q1_4 = Request.Form["q1_4"] == "on" ? "q1_4," : "";
                _q1_5 = Request.Form["q1_5"] == "on" ? "q1_5," : "";
                //if (_q1_1 != "" || _q1_5 != "")
                _q_quyen = _q_quyen + _q1_1 + _q1_2 + _q1_3 + _q1_4 + _q1_5;

                //QUẢN LÝ TÀI KHOẢN
                string _q2_1, _q2_2, _q2_3, _q2_4, _q2_5, _q2_6, _q2_7, _q2_8, _q2_9, _q2_10, _n2_1, _n2_2, _n2_3, _n2_4, _n2_5, _n2_6, _n2_7, _n2_8, _n2_9, _n2_10;
                _q2_1 = Request.Form["q2_1"] == "on" ? "q2_1," : "";
                _q2_2 = Request.Form["q2_2"] == "on" ? "q2_2," : "";
                _q2_3 = Request.Form["q2_3"] == "on" ? "q2_3," : "";
                _q2_4 = Request.Form["q2_4"] == "on" ? "q2_4," : "";
                _q2_5 = Request.Form["q2_5"] == "on" ? "q2_5," : "";
                _q2_6 = Request.Form["q2_6"] == "on" ? "q2_6," : "";
                _q2_7 = Request.Form["q2_7"] == "on" ? "q2_7," : "";
                _q2_8 = Request.Form["q2_8"] == "on" ? "q2_8," : "";
                _q2_9 = Request.Form["q2_9"] == "on" ? "q2_9," : "";
                _q2_10 = Request.Form["q2_10"] == "on" ? "q2_10," : "";
                _n2_1 = Request.Form["n2_1"] == "on" ? "n2_1," : "";
                _n2_2 = Request.Form["n2_2"] == "on" ? "n2_2," : "";
                _n2_3 = Request.Form["n2_3"] == "on" ? "n2_3," : "";
                _n2_4 = Request.Form["n2_4"] == "on" ? "n2_4," : "";
                _n2_5 = Request.Form["n2_5"] == "on" ? "n2_5," : "";
                _n2_6 = Request.Form["n2_6"] == "on" ? "n2_6," : "";
                _n2_7 = Request.Form["n2_7"] == "on" ? "n2_7," : "";
                _n2_8 = Request.Form["n2_8"] == "on" ? "n2_8," : "";
                _n2_9 = Request.Form["n2_9"] == "on" ? "n2_9," : "";
                _n2_10 = Request.Form["n2_10"] == "on" ? "n2_10," : "";
                //if (_q2_1 != "" || _q2_2 != "")
                _q_quyen = _q_quyen + _q2_1 + _q2_2 + _q2_3 + _q2_4 + _q2_5 + _q2_6 + _q2_7 + _q2_8 + _q2_9 + _q2_10 + _n2_1 + _n2_2 + _n2_3 + _n2_4 + _n2_5 + _n2_6 + _n2_7 + _n2_8 + _n2_9 + _n2_10;

                //QUẢN LÝ menu
                string _q3_1, _q3_2, _q3_3, _q3_4, _q3_5, _q3_6;
                _q3_1 = Request.Form["q3_1"] == "on" ? "q3_1," : "";
                _q3_2 = Request.Form["q3_2"] == "on" ? "q3_2," : "";
                _q3_3 = Request.Form["q3_3"] == "on" ? "q3_3," : "";
                _q3_4 = Request.Form["q3_4"] == "on" ? "q3_4," : "";
                _q3_5 = Request.Form["q3_5"] == "on" ? "q3_5," : "";
                _q3_6 = Request.Form["q3_6"] == "on" ? "q3_6," : "";
                //if (_q3_1 != "" || _q3_2 != "")
                _q_quyen = _q_quyen + _q3_1 + _q3_2 + _q3_3 + _q3_4 + _q3_5 + _q3_6;

                //QUẢN LÝ bài viết
                string _q4_1, _q4_2, _q4_3, _q4_4, _q4_5, _q4_6;
                _q4_1 = Request.Form["q4_1"] == "on" ? "q4_1," : "";
                _q4_2 = Request.Form["q4_2"] == "on" ? "q4_2," : "";
                _q4_3 = Request.Form["q4_3"] == "on" ? "q4_3," : "";
                _q4_4 = Request.Form["q4_4"] == "on" ? "q4_4," : "";
                _q4_5 = Request.Form["q4_5"] == "on" ? "q4_5," : "";
                _q4_6 = Request.Form["q4_6"] == "on" ? "q4_6," : "";

                //if (_q4_1 != "" || _q4_2 != "")
                _q_quyen = _q_quyen + _q4_1 + _q4_2 + _q4_3 + _q4_4 + _q4_5 + _q4_6;

                //QUẢN LÝ MODULE
                string _q5_1;
                _q5_1 = Request.Form["q5_1"] == "on" ? "q5_1," : "";
                //if (_q4_1 != "" || _q4_2 != "")
                _q_quyen = _q_quyen + _q5_1;

                //data yêu cầu tư vấn
                string _q6_1;
                _q6_1 = Request.Form["q6_1"] == "on" ? "q6_1," : "";
                _q_quyen = _q_quyen + _q6_1;

                //QUẢN LÝ HÓA ĐƠN
                string _q7_1, _q7_2, _q7_3, _q7_4, _q7_5, _q7_6, _q7_7, _q7_8, _q7_9, _q7_10, _n7_1, _n7_2, _n7_3, _n7_4, _n7_5, _n7_6, _n7_7, _n7_8, _n7_9, _n7_10;
                _q7_1 = Request.Form["q7_1"] == "on" ? "q7_1," : "";
                _q7_2 = Request.Form["q7_2"] == "on" ? "q7_2," : "";
                _q7_3 = Request.Form["q7_3"] == "on" ? "q7_3," : "";
                _q7_4 = Request.Form["q7_4"] == "on" ? "q7_4," : "";
                _q7_5 = Request.Form["q7_5"] == "on" ? "q7_5," : "";
                _q7_6 = Request.Form["q7_6"] == "on" ? "q7_6," : "";
                _q7_7 = Request.Form["q7_7"] == "on" ? "q7_7," : "";
                _q7_8 = Request.Form["q7_8"] == "on" ? "q7_8," : "";
                _q7_9 = Request.Form["q7_9"] == "on" ? "q7_9," : "";
                _q7_10 = Request.Form["q7_10"] == "on" ? "q7_10," : "";
                _n7_1 = Request.Form["n7_1"] == "on" ? "n7_1," : "";
                _n7_2 = Request.Form["n7_2"] == "on" ? "n7_2," : "";
                _n7_3 = Request.Form["n7_3"] == "on" ? "n7_3," : "";
                _n7_4 = Request.Form["n7_4"] == "on" ? "n7_4," : "";
                _n7_5 = Request.Form["n7_5"] == "on" ? "n7_5," : "";
                _n7_6 = Request.Form["n7_6"] == "on" ? "n7_6," : "";
                _n7_7 = Request.Form["n7_7"] == "on" ? "n7_7," : "";
                _n7_8 = Request.Form["n7_8"] == "on" ? "n7_8," : "";
                _n7_9 = Request.Form["n7_9"] == "on" ? "n7_9," : "";
                _n7_10 = Request.Form["n7_10"] == "on" ? "n7_10," : "";
                _q_quyen = _q_quyen + _q7_1 + _q7_2 + _q7_3 + _q7_4 + _q7_5 + _q7_6 + _q7_7 + _q7_8 + _q7_9 + _q7_10 + _n7_1 + _n7_2 + _n7_3 + _n7_4 + _n7_5 + _n7_6 + _n7_7 + _n7_8 + _n7_9 + _n7_10;

                //DATA KHÁCH HÀNG
                string _q8_1, _q8_2, _q8_3, _q8_4, _q8_5, _n8_1, _n8_2, _n8_3, _n8_4, _n8_5;
                _q8_1 = Request.Form["q8_1"] == "on" ? "q8_1," : "";
                _q8_2 = Request.Form["q8_2"] == "on" ? "q8_2," : "";
                _q8_3 = Request.Form["q8_3"] == "on" ? "q8_3," : "";
                _q8_4 = Request.Form["q8_4"] == "on" ? "q8_4," : "";
                //_q8_5 = Request.Form["q8_5"] == "on" ? "q8_5," : "";
                //_n8_1 = Request.Form["n8_1"] == "on" ? "n8_1," : "";
                //_n8_2 = Request.Form["n8_2"] == "on" ? "n8_2," : "";
                //_n8_3 = Request.Form["n8_3"] == "on" ? "n8_3," : "";
                //_n8_4 = Request.Form["n8_4"] == "on" ? "n8_4," : "";
                //_n8_5 = Request.Form["n8_5"] == "on" ? "n8_5," : "";
                _q_quyen = _q_quyen + _q8_1 + _q8_2 + _q8_3 + _q8_4 ;

                //QUẢN LÝ THU CHI
                string _q9_1, _q9_2, _q9_3, _q9_4, _q9_5, _q9_6, _n9_1, _n9_2, _n9_3, _n9_4, _n9_5, _n9_6;
                _q9_1 = Request.Form["q9_1"] == "on" ? "q9_1," : "";
                _q9_2 = Request.Form["q9_2"] == "on" ? "q9_2," : "";
                _q9_3 = Request.Form["q9_3"] == "on" ? "q9_3," : "";
                _q9_4 = Request.Form["q9_4"] == "on" ? "q9_4," : "";
                _q9_5 = Request.Form["q9_5"] == "on" ? "q9_5," : "";
                _q9_6 = Request.Form["q9_6"] == "on" ? "q9_6," : "";
                _n9_1 = Request.Form["n9_1"] == "on" ? "n9_1," : "";
                _n9_2 = Request.Form["n9_2"] == "on" ? "n9_2," : "";
                _n9_3 = Request.Form["n9_3"] == "on" ? "n9_3," : "";
                _n9_4 = Request.Form["n9_4"] == "on" ? "n9_4," : "";
                _n9_5 = Request.Form["n9_5"] == "on" ? "n9_5," : "";
                _n9_6 = Request.Form["n9_6"] == "on" ? "n9_6," : "";
                _q_quyen = _q_quyen + _q9_1 + _q9_2 + _q9_3 + _q9_4 + _q9_5 + _q9_6 + _n9_1 + _n9_2 + _n9_3 + _n9_4 + _n9_5 + _n9_6;

                //LỊCH HẸN
                string _q10_1, _q10_2, _q10_3, _q10_4;
                _q10_1 = Request.Form["q10_1"] == "on" ? "q10_1," : "";
                _q10_2 = Request.Form["q10_2"] == "on" ? "q10_2," : "";
                _q10_3 = Request.Form["q10_3"] == "on" ? "q10_3," : "";
                _q10_4 = Request.Form["q10_4"] == "on" ? "q10_4," : "";
                _q_quyen = _q_quyen + _q10_1 + _q10_2 + _q10_3 + _q10_4;

                //KHO HÀNG
                string _q11_1, _q11_2, _q11_3, _q11_4, _q11_5, _q11_6, _q11_7, _q11_8;
                _q11_1 = Request.Form["q11_1"] == "on" ? "q11_1," : "";
                _q11_2 = Request.Form["q11_2"] == "on" ? "q11_2," : "";
                _q11_3 = Request.Form["q11_3"] == "on" ? "q11_3," : "";
                _q11_4 = Request.Form["q11_4"] == "on" ? "q11_4," : "";
                _q11_5 = Request.Form["q11_5"] == "on" ? "q11_5," : "";
                _q11_6 = Request.Form["q11_6"] == "on" ? "q11_6," : "";
                _q11_7 = Request.Form["q11_7"] == "on" ? "q11_7," : "";
                _q11_8 = Request.Form["q11_8"] == "on" ? "q11_8," : "";
                _q_quyen = _q_quyen + _q11_1 + _q11_2 + _q11_3 + _q11_4 + _q11_5 + _q11_6 + _q11_7 + _q11_8;

                //THẺ DỊCH VỤ
                string _q12_1, _q12_2, _q12_3, _q12_4, _q12_5, _n12_1, _n12_2, _n12_3, _n12_4, _n12_5;
                _q12_1 = Request.Form["q12_1"] == "on" ? "q12_1," : "";
                _q12_2 = Request.Form["q12_2"] == "on" ? "q12_2," : "";
                _q12_3 = Request.Form["q12_3"] == "on" ? "q12_3," : "";
                _q12_4 = Request.Form["q12_4"] == "on" ? "q12_4," : "";
                _q12_5 = Request.Form["q12_5"] == "on" ? "q12_5," : "";
                _n12_1 = Request.Form["n12_1"] == "on" ? "n12_1," : "";
                _n12_2 = Request.Form["n12_2"] == "on" ? "n12_2," : "";
                _n12_3 = Request.Form["n12_3"] == "on" ? "n12_3," : "";
                _n12_4 = Request.Form["n12_4"] == "on" ? "n12_4," : "";
                _n12_5 = Request.Form["n12_5"] == "on" ? "n12_5," : "";
                _q_quyen = _q_quyen + _q12_1 + _q12_2 + _q12_3 + _q12_4 + _q12_5 + _n12_1 + _n12_2 + _n12_3 + _n12_4 + _n12_5;

                //KHO VẬT TƯ
                string _q13_1, _q13_2, _q13_3, _q13_4, _q13_5, _q13_6, _q13_7, _q13_8, _q13_9;
                _q13_1 = Request.Form["q13_1"] == "on" ? "q13_1," : "";
                _q13_2 = Request.Form["q13_2"] == "on" ? "q13_2," : "";
                _q13_3 = Request.Form["q13_3"] == "on" ? "q13_3," : "";
                _q13_4 = Request.Form["q13_4"] == "on" ? "q13_4," : "";
                _q13_5 = Request.Form["q13_5"] == "on" ? "q13_5," : "";
                _q13_6 = Request.Form["q13_6"] == "on" ? "q13_6," : "";
                _q13_7 = Request.Form["q13_7"] == "on" ? "q13_7," : "";
                _q13_8 = Request.Form["q13_8"] == "on" ? "q13_8," : "";
                _q13_9 = Request.Form["q13_9"] == "on" ? "q13_9," : "";
                _q_quyen = _q_quyen + _q13_1 + _q13_2 + _q13_3 + _q13_4 + _q13_5 + _q13_6 + _q13_7 + _q13_8 + _q13_9;

                //thành viên
                string _q14_1, _q14_2, _q14_3, _q14_4, _q14_5, _n14_1, _n14_2, _n14_3, _n14_4, _n14_5;
                _q14_1 = Request.Form["q14_1"] == "on" ? "q14_1," : "";
                _q14_2 = Request.Form["q14_2"] == "on" ? "q14_2," : "";
                _q14_3 = Request.Form["q14_3"] == "on" ? "q14_3," : "";
                _q14_4 = Request.Form["q14_4"] == "on" ? "q14_4," : "";
                _q14_5 = Request.Form["q14_5"] == "on" ? "q14_5," : "";
                _n14_1 = Request.Form["n14_1"] == "on" ? "n14_1," : "";
                _n14_2 = Request.Form["n14_2"] == "on" ? "n14_2," : "";
                _n14_3 = Request.Form["n14_3"] == "on" ? "n14_3," : "";
                _n14_4 = Request.Form["n14_4"] == "on" ? "n14_4," : "";
                _n14_5 = Request.Form["n14_5"] == "on" ? "n14_5," : "";
                _q_quyen = _q_quyen + _q14_1 + _q14_2 + _q14_3 + _q14_4 + _q14_5 + _n14_1 + _n14_2 + _n14_3 + _n14_4 + _n14_5;

                //Chuyên gia
                string _q15_1, _q15_2, _q15_3, _q15_4, _n15_1, _n15_2, _n15_3, _n15_4;
                _q15_1 = Request.Form["q15_1"] == "on" ? "q15_1," : "";
                _q15_2 = Request.Form["q15_2"] == "on" ? "q15_2," : "";
                _q15_3 = Request.Form["q15_3"] == "on" ? "q15_3," : "";
                _q15_4 = Request.Form["q15_4"] == "on" ? "q15_4," : "";
                _n15_1 = Request.Form["n15_1"] == "on" ? "n15_1," : "";
                _n15_2 = Request.Form["n15_2"] == "on" ? "n15_2," : "";
                _n15_3 = Request.Form["n15_3"] == "on" ? "n15_3," : "";
                _n15_4 = Request.Form["n15_4"] == "on" ? "n15_4," : "";
                _q_quyen = _q_quyen + _q15_1 + _q15_2 + _q15_3 + _q15_4 + _n15_1 + _n15_2 + _n15_3 + _n15_4;

                //HỆ THỐNG
                string _q16_0, _q16_1, _q16_2;
                _q16_0 = Request.Form["q16_0"] == "on" ? "q16_0," : "";
                _q16_1 = Request.Form["q16_1"] == "on" ? "q16_1," : "";
                _q16_2 = Request.Form["q16_2"] == "on" ? "q16_2," : "";
                _q_quyen = _q_quyen + _q16_0 + _q16_1 + _q16_2;

                if (_q_quyen != "")
                    _q_quyen = _q_quyen.Substring(0, _q_quyen.Length - 1);//loại bỏ dấu phẩy cuối cùng

                tk_cl.update_quyen(user_phanquyen, _q_quyen);

                Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Phân quyền thành công.", "4000", "warning");
                Response.Redirect("/gianhang/admin/quan-ly-tai-khoan/tai-khoan.aspx?user=" + user_phanquyen);
            }
            
        }
        else
        {
            Session["notifi"] = thongbao_class.metro_dialog_onload("Thông báo", "Bạn không đủ quyền để truy cập hoặc thực hiện thao tác vừa rồi.", "false", "false", "OK", "alert", "");
            Response.Redirect("/gianhang/admin");
        }
    }


}