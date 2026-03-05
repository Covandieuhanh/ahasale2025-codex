using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class home_uc_fix_right_uc : System.Web.UI.UserControl
{
    String_cl str_cl = new String_cl();
    public string link_zalo,sdt;
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            using (dbDataContext db=new dbDataContext())
            {
                var q = db.CaiDatChung_tbs.FirstOrDefault(p=>p.phanloai_trang=="home");
                if(q!=null)
                {
                    link_zalo = q.link_zalo;
                    sdt = q.thongtin_hotline;
                }    
            }    
        }    
    }

    #region ĐĂNG KÝ TƯ VẤN
    protected void but_show_form_dangkytuvan_Click(object sender, ImageClickEventArgs e)
    {
        reset_control1();
        pn_tuvan.Visible = !pn_tuvan.Visible;
        up_tuvan.Update();
    }
    protected void but_close_form_dangkytuvan_Click(object sender, EventArgs e)
    {
        reset_control1();
        pn_tuvan.Visible = !pn_tuvan.Visible;
    }
    public void reset_control1()
    {
        txt_ten_dangky.Text = ""; txt_sdt_dangky.Text = ""; txt_noidung1_dangky.Text = "";
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        string _tenkh = str_cl.VietHoa_ChuCai_DauTien(str_cl.Remove_Blank(txt_ten_dangky.Text.Trim().ToLower()));
        string _sdtkh = txt_sdt_dangky.Text.Replace(" ", "").Replace(".", "").Replace("+", "").Replace("-", "");
        string _noidung1 = txt_noidung1_dangky.Text.Trim();
        if (_tenkh == "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập tên của bạn.", "false", "false", "OK", "alert", ""), true);
            return;
        }
        if (_sdtkh == "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập số điện thoại của bạn.", "false", "false", "OK", "alert", ""), true);
            return;
        }
        if (_noidung1 == "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("Thông báo", "Vui lòng nhập nội dung cần tư vấn.", "false", "false", "OK", "alert", ""), true);
            return;
        }
        using (dbDataContext db = new dbDataContext())
        {
            data_yeucau_tuvan_table _ob = new data_yeucau_tuvan_table();
            _ob.ten = _tenkh;
            _ob.sdt = _sdtkh;
            _ob.noidung = _noidung1;
            _ob.bin = false;
            _ob.ngay = AhaTime_cl.Now;
            _ob.kyhieu_nguon = "home";
            db.data_yeucau_tuvan_tables.InsertOnSubmit(_ob);
            db.SubmitChanges();

            // thông báo trên app
            ThongBao_tb _ob1 = new ThongBao_tb();
            _ob1.id = Guid.NewGuid();
            _ob1.daxem = false;//chưa xem
            _ob1.nguoithongbao = "admin";
            _ob1.nguoinhan = "admin";
            _ob1.link = "/admin/yeu-cau-tu-van/default.aspx";
            _ob1.noidung = _tenkh + " vừa gửi yêu cầu tư vấn tại Website. ID:" + _ob.id;
            _ob1.thoigian = AhaTime_cl.Now;
            _ob1.bin = false;
            db.ThongBao_tbs.InsertOnSubmit(_ob1);
            db.SubmitChanges();


            #region THÔNG BÁO QUA EMAIL
            // Lấy danh sách email từ bảng taikhoan_tb
            var emailAddresses = db.taikhoan_tbs
                .Where(tk => tk.taikhoan == "admin" /*|| tk.username == "bonbap"*/)
                .Select(tk => tk.email)
                .ToList();

            string _tenmien = HttpContext.Current.Request.Url.Host.ToUpper();
            string _tieude = _tenkh + " vừa gửi yêu cầu tư vấn tại " + HttpContext.Current.Request.Url.Host;
            string _noidung = "<h3>YÊU CẦU TƯ VẤN</h3>";
            _noidung = _noidung + "<div><b>HỌ TÊN: </b>" + _tenkh + "</div>";
            _noidung = _noidung + "<div><b>SỐ ĐIỆN THOẠI: </b>" + _sdtkh + "</div>";
            _noidung = _noidung + "<div><b>NỘI DUNG: </b>" + _noidung1 + "</div>";
            string _ten_nguoigui = _tenmien;
            string _link_dinhkem = "";

            //gán mail trực tiếp
            //List<string> emailAddresses = new List<string>
            //{
            //    "bcorn.net@gmail.com","mail khác",...
            //};
            foreach (var _email_nhan in emailAddresses)
            {
                guiEmail_cl.SendEmail(_email_nhan, _tieude, _noidung, _ten_nguoigui, _link_dinhkem);
            }
            #endregion

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Gửi yêu cầu thành công.", "4000", "warning"), true);
        }
        reset_control1();
        pn_tuvan.Visible = !pn_tuvan.Visible;

    }
    #endregion
}