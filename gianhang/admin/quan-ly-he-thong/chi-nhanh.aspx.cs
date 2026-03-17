using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class badmin_Default : System.Web.UI.Page
{
    taikhoan_class tk_cl = new taikhoan_class();
    dbDataContext db = new dbDataContext();
    string_class str_cl = new string_class();
    chinhanh_class cn_cl = new chinhanh_class();

    public string user, user_parent;
    #region phân trang
    public int stt = 1, current_page = 1, show = 50, total_page = 1;
    List<string> list_id_split;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        #region Check_Login
        string _quyen = "q16_0";
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
        user = Session["user"].ToString();
        user_parent = "admin";
        if (!IsPostBack)
        {
            Session["index_sapxep_quanlychinhanh"] = "0";
            Session["current_page_quanlychinhanh"] = "1";
        }
        main();
    }
    public void main()
    {
        //lấy dữ liệu
        var list_all = (from ob1 in db.chinhanh_tables.ToList()
                        select new
                        {
                            id = ob1.id,
                            ten = ob1.ten,
                            taikhoan_quantri = ob1.taikhoan_quantri,
                        }).ToList();

        //xử lý từ khóa
        string _key = txt_search.Text.ToLower();
        if (_key != "")
        {
            var list_search = list_all.Where(p => p.ten.ToLower().Contains(_key)).ToList();
            list_all = list_all.Intersect(list_search).ToList();
        }

        //sắp xếp
        switch (Session["index_sapxep_quanlychinhanh"].ToString())
        {
            //case ("1"): list_all = list_all.OrderBy(p => p.ngaytao_tk).ToList(); break;
            default: list_all = list_all.OrderBy(p => p.ten).ToList(); break;
        }

        //xử lý số lượng hiển thị
        string _s = txt_show.Text.Trim();
        int.TryParse(_s, out show);//nếu số mục hiển thị _s là số hợp lệ thì show = _s
        if (show <= 0)
            show = 50;
        txt_show.Text = show.ToString();

        total_page = number_of_page_class.return_total_page(list_all.Count(), show);

        //xử lý số trang        
        current_page = int.Parse(Session["current_page_quanlychinhanh"].ToString());
        if (current_page > total_page)
            current_page = total_page;
        if (current_page >= total_page)
            but_xemtiep.Enabled = false;
        else
            but_xemtiep.Enabled = true;
        if (current_page == 1)
            but_quaylai.Enabled = false;
        else
            but_quaylai.Enabled = true;

        //main
        stt = (show * current_page) - show + 1;
        var list_split = list_all.Skip(current_page * show - show).Take(show).ToList();
        list_id_split = new List<string>();
        foreach (var t in list_split)
        {
            list_id_split.Add("check_" + t.id);
        }
        int _s1 = stt + list_split.Count - 1;
        if (list_all.Count() != 0)
            lb_show.Text = "Hiển thị " + stt + "-" + _s1 + " trong số " + list_all.Count().ToString("#,##0") + " mục";
        else
            lb_show.Text = "Hiển thị 0-0 trong số 0";
        Repeater1.DataSource = list_split;
        Repeater1.DataBind();
    }
    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        Session["current_page_quanlychinhanh"] = "1";

        main();

    }
    protected void txt_show_TextChanged(object sender, EventArgs e)
    {
        Session["current_page_quanlychinhanh"] = "1";

        main();
    }
    protected void but_quaylai_Click(object sender, EventArgs e)
    {
        Session["current_page_quanlychinhanh"] = int.Parse(Session["current_page_quanlychinhanh"].ToString()) - 1;
        main();
    }
    protected void but_xemtiep_Click(object sender, EventArgs e)
    {
        Session["current_page_quanlychinhanh"] = int.Parse(Session["current_page_quanlychinhanh"].ToString()) + 1;

        main();
    }

    protected void but_form_themnhomthuchi_Click(object sender, EventArgs e)
    {
        string _user = txt_taikhoan.Text.Trim().ToLower();
        string _pass = txt_matkhau.Text.Trim();
        string _fullname = str_cl.VietHoa_ChuCai_DauTien(str_cl.remove_blank(txt_hoten.Text.Trim().ToLower()));
        string _email = txt_email.Text.ToLower().Trim();

        string _tennhom = txt_tennhom.Text.Trim();

        if (_tennhom == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tên chi nhánh.", "false", "false", "OK", "alert", ""), true);
        else
        {
            if (_user == "")
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tài khoản.", "false", "false", "OK", "alert", ""), true);
            else
            {
                if (tk_cl.exist_user(_user))
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản đã tồn tại. Vui lòng chọn tên khác.", "false", "false", "OK", "alert", ""), true);
                else
                {
                    if (!regex_class.check_user_invalid(_user) || _user.Length < 5 || _user.Length > 30)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Tài khoản không hợp lệ. Tài khoản hợp lệ phải có độ dài từ 5-30 ký tự và chỉ chứa các chữ cái từ a-z hoặc số từ 0-9.", "false", "false", "OK", "alert", ""), true);
                    else
                    {
                        if (!tk_cl.check_name_invalid(_user))//thỏa mãn a-x 0-9 nhưng trúng mấy cái tên mình k cho phép, vd như admin
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng chọn tên tài khoản khác.", "false", "false", "OK", "alert", ""), true);
                        else
                        {
                            if (_pass == "")
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập mật khẩu.", "false", "false", "OK", "alert", ""), true);
                            else
                            {
                                if (_fullname == "")
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập họ tên.", "false", "false", "OK", "alert", ""), true);
                                else
                                {
                                    if (!regex_class.check_email_invalid(_email))
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Email không hợp lệ.", "false", "false", "OK", "alert", ""), true);
                                    else
                                    {
                                        if (_email != "" && tk_cl.exist_email(_email))
                                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Email này đã được đăng ký cho một tài khoản khác.", "false", "false", "OK", "alert", ""), true);
                                        else
                                        {

                                            chinhanh_table _ob = new chinhanh_table();
                                            _ob.ten = _tennhom;
                                            _ob.taikhoan_quantri = _user;
                                            db.chinhanh_tables.InsertOnSubmit(_ob);
                                            db.SubmitChanges();
                                            txt_tennhom.Text = "";


                                            #region tao tai khoan
                                            taikhoan_table_2023 _ob1 = new taikhoan_table_2023();
                                            string _quyen_admin = "q0_1,q0_2,q0_3,q0_4,q2_1,q2_2,q2_3,q2_4,q2_5,q2_6,q2_7,q2_8,q2_9,q2_10,q3_1,q3_2,q3_3,q3_4,q3_5,q3_6,q4_1,q4_2,q4_3,q4_4,q4_5,q4_6,q7_1,q7_2,q7_3,q7_4,q7_5,q7_6,q7_7,q7_8,q7_9,q7_10,q8_1,q8_2,q8_3,q8_4,q8_5,q9_1,q9_2,q9_3,q9_4,q9_5,q9_6,q10_1,q10_2,q10_3,q10_4,q11_1,q11_2,q11_4,q11_5,q11_6,q11_7,q11_8,q12_1,q12_2,q12_3,q12_4,q12_5,q13_1,q13_2,q13_4,q13_5,q13_6,q13_7,q13_8,q13_9,q14_1,q14_2,q14_3,q14_4,q14_5,q15_1,q15_2,q15_3,q15_4,q16_1,q16_2";
                                            _ob1.permission = _quyen_admin;

                                            string _avt = "/uploads/images/macdinh.jpg";
                                            _ob1.taikhoan = _user;
                                            _ob1.matkhau = encode_class.encode_md5(encode_class.encode_sha1(_pass));
                                            _ob1.makhoiphuc = "141191";
                                            _ob1.hsd_makhoiphuc = DateTime.Parse("01/01/1991");
                                            _ob1.nguoitao = Session["user"].ToString();
                                            _ob1.ngaytao = DateTime.Now;

                                            _ob1.id_chinhanh = cn_cl.return_max_id().ToString();
                                            _ob1.id_nganh = "";
                                            _ob1.hoten = _fullname;
                                            _ob1.hoten_khongdau = str_cl.remove_vietnamchar(_fullname);
                                            _ob1.anhdaidien = _avt;
                                            _ob1.trangthai = "Đang hoạt động";
                                            _ob1.email = _email; _ob1.dienthoai = ""; _ob1.zalo = ""; _ob1.facebook = "";
                                            _ob1.ngaysinh = null;
                                            _ob1.hansudung = null;
                                            _ob1.luongcoban = 0;
                                            _ob1.songaycong = 0;
                                            _ob1.luongngay = 0;
                                            _ob1.user_parent = "admin";
                                            db.taikhoan_table_2023s.InsertOnSubmit(_ob1);
                                            db.SubmitChanges();
                                            #endregion

                                            string _tennguoigui = "", _tieude_mail = "", _noidung_mail = "";
                                            _tennguoigui = HttpContext.Current.Request.Url.Host;
                                            _tieude_mail = "Thông tin đăng nhập";
                                            _noidung_mail = _noidung_mail + "<h3>HƯỚNG DẪN ĐĂNG NHẬP</h3>";
                                            _noidung_mail = _noidung_mail + "<div>Link đăng nhập: <b>" + "https://ahashine.vn/gianhang/admin" + "</b></div>";
                                            _noidung_mail = _noidung_mail + "<div>TÊN TÀI KHOẢN: <b>" + _user + "</b></div>";
                                            _noidung_mail = _noidung_mail + "<div>MẬT KHẨU: <b>" + _pass + "</b></div>";
                                            _noidung_mail = _noidung_mail + "<div style='color:red'>Lưu ý: Đây là thư tự động được gửi từ hệ thống - Không phản hồi mail này, xin cám ơn.</div>";
                                            sendmail_class.sendmail("smtp.gmail.com", 587, _tieude_mail, _noidung_mail, _email, _tennguoigui, "");


                                            //txt_taikhoan.Text = ""; txt_hoten.Text = ""; txt_email.Text = "";
                                            //main();
                                            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Thêm chi nhánh thành công.", "4000", "alert"), true);
                                            Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Thêm chi nhánh thành công.", "2000", "alert");
                                            Response.Redirect("/gianhang/admin/quan-ly-he-thong/chi-nhanh.aspx");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
    }

    protected void but_save_Click(object sender, ImageClickEventArgs e)
    {
        for (int i = 0; i < list_id_split.Count; i++)
        {
            string _id = list_id_split[i].Replace("check_", "");
            string _tennhom = Request.Form["name_" + _id].Trim();

            var q = db.chinhanh_tables.Where(p => p.id.ToString() == _id);
            if (q.Count() != 0)
            {
                chinhanh_table _ob = q.First();
                _ob.ten = _tennhom;
                db.SubmitChanges();
            }
        }
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Lưu thành công.", "4000", "alert"), true);
        //Session["notifi"] = thongbao_class.metro_notifi_onload("Thông báo", "Lưu thành công.", "2000", "alert");
        //Response.Redirect("/gianhang/admin/quan-ly-thu-chi/nhom-thu-chi.aspx");
    }

    protected void but_xoa_Click(object sender, ImageClickEventArgs e)
    {
        for (int i = 0; i < list_id_split.Count; i++)
        {
            if (Request.Form[list_id_split[i]] == "on")
            {
                string _id = list_id_split[i].Replace("check_", "");
                var q = db.chinhanh_tables.Where(p => p.id.ToString() == _id);
                if (q.Count() != 0)
                {
                    chinhanh_table _ob = q.First();
                    db.chinhanh_tables.DeleteOnSubmit(_ob);
                    db.SubmitChanges();
                }
            }
        }
        main();
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Xóa thành công các mục đã chọn.", "4000", "alert"), true);
    }

}
