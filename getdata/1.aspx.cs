using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class dt : System.Web.UI.Page
{
    public string notifi = "", meta;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (dbDataContext db = new dbDataContext())
            {
                #region Favicon & icon mobile
                var q = (from tk in db.CaiDatChung_tbs
                         where tk.phanloai_trang == "home"
                         select new { tk.thongtin_icon, tk.thongtin_apple_touch_icon }).FirstOrDefault();

                if (q != null)
                {
                    string baseUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);

                    string iconUrl = string.Format("{0}{1}", baseUrl, q.thongtin_icon);
                    string appleTouchIconUrl = string.Format("{0}{1}", baseUrl, q.thongtin_apple_touch_icon);

                    string iconsHtml = string.Format(@"
                <!-- Favicon -->
                <link rel='icon' href='{0}' sizes='16x16' type='image/x-icon'>
                <link rel='icon' href='{1}' sizes='32x32' type='image/x-icon'>
                <link rel='icon' href='{2}' sizes='48x48' type='image/x-icon'>

                <!-- Apple Touch Icon -->
                <link rel='apple-touch-icon' href='{3}' sizes='180x180'>
                <link rel='apple-touch-icon' href='{4}' sizes='167x167'>
                <link rel='apple-touch-icon' href='{5}' sizes='152x152'>
                <link rel='apple-touch-icon' href='{6}' sizes='120x120'>

                <!-- Android Icons -->
                <link rel='icon' href='{7}' sizes='192x192'>
                <link rel='icon' href='{8}' sizes='144x144'>
                ", iconUrl, iconUrl, iconUrl, appleTouchIconUrl, appleTouchIconUrl, appleTouchIconUrl, appleTouchIconUrl, iconUrl, iconUrl);

                    literal_fav_icon.Text = iconsHtml;
                }
                #endregion

                //if (!IsPostBack)
                //{
                //    List<string> provinces = new List<string>
                //{
                //    "An Giang", "Bà Rịa - Vũng Tàu", "Bạc Liêu", "Bắc Kạn", "Bắc Giang",
                //    "Bắc Ninh", "Bến Tre", "Bình Dương", "Bình Định", "Bình Phước",
                //    "Bình Thuận", "Cà Mau", "Cao Bằng", "Cần Thơ", "Đà Nẵng",
                //    "Đắk Lắk", "Đắk Nông", "Điện Biên", "Đồng Nai", "Đồng Tháp",
                //    "Gia Lai", "Hà Giang", "Hà Nam", "Hà Nội", "Hà Tĩnh",
                //    "Hải Dương", "Hải Phòng", "Hậu Giang", "Hòa Bình", "Hưng Yên",
                //    "Khánh Hòa", "Kiên Giang", "Kon Tum", "Lai Châu", "Lâm Đồng",
                //    "Lạng Sơn", "Lào Cai", "Long An", "Nam Định", "Nghệ An",
                //    "Ninh Bình", "Ninh Thuận", "Phú Thọ", "Phú Yên", "Quảng Bình",
                //    "Quảng Nam", "Quảng Ngãi", "Quảng Ninh", "Quảng Trị", "Sóc Trăng",
                //    "Sơn La", "Tây Ninh", "Thái Bình", "Thái Nguyên", "Thanh Hóa",
                //    "Thừa Thiên Huế", "Tiền Giang", "Hồ Chí Minh", "Trà Vinh", "Tuyên Quang",
                //    "Vĩnh Long", "Vĩnh Phúc", "Yên Bái"
                //};
                //    DropDownList1.DataSource = provinces;
                //    DropDownList1.DataBind();
                //    // Chèn mục "Chọn" vào đầu danh sách
                //    DropDownList1.Items.Insert(0, new ListItem("Tỉnh thành", ""));
                //}
            }
        }
    }
    public String_cl str_cl = new String_cl();
    //public datetime_class dt_cl = new datetime_class();
    protected void Button1_Click(object sender, EventArgs e)
    {
        //if (TextBox1.Text == "" || TextBox2.Text == "" || TextBox3.Text == "" || DropDownList1.SelectedValue.ToString() == "" || txt_ngaysinh.Text == "" || DropDownList2.SelectedValue.ToString() == "")
        if (TextBox1.Text == "" || TextBox2.Text == "")
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập đầy đủ thông tin", "1000", "alert"), true);
        else
        {
            string _fullname = str_cl.VietHoa_ChuCai_DauTien(str_cl.Remove_Blank(TextBox1.Text.Trim().ToLower()));
            string _sdt = TextBox2.Text.Trim().Replace(" ", "").Replace("-", "").Replace("+", "").Replace(".", "");
            //string _zalo = TextBox3.Text.Trim().Replace(" ", "").Replace("-", "").Replace("+", "").Replace(".", "");
            //string _tinhthanh = DropDownList1.SelectedValue.ToString();
            //string _gioitinh = DropDownList2.SelectedValue.ToString();
            //string _ngaysinh = txt_ngaysinh.Text;
            if (!str_cl.KiemTra_SDT(_sdt))
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Số điện thoại không hợp lệ", "1000", "alert"), true);
            else
            {
                using (dbDataContext db = new dbDataContext())
                {
                    lichsu_get_data_tb _ob = new lichsu_get_data_tb();
                    _ob.hoten = _fullname;
                    _ob.sdt = _sdt;
                    _ob.zalo = "";
                    _ob.gioitinh = "";
                    _ob.ngaysinh = null;
                    _ob.tinhthanh = "";
                    _ob.ngaydangky = DateTime.Now;
                    _ob.nguon = "PHÚT VÀNG BẮT MẠCH AI";
                    db.lichsu_get_data_tbs.InsertOnSubmit(_ob);
                    db.SubmitChanges();

                    // Sau khi xử lý thành công, chạy JS để mở Zalo
                    string script = @"
        window.location.href = 'zalo://zaloapp.com/g/otoxvt910';
        setTimeout(function(){ window.location.href = 'https://zalo.me/g/otoxvt910'; }, 1500);
    ";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenZalo", script, true);

                    //TextBox1.Text = "";
                    //TextBox2.Text = "";

                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_dialog("ĐĂNG KÝ THÀNH CÔNG", "Xin chúc mừng bạn đã nhận được gói " +
                    //    "<b>Tầm Soát Ung Thư</b> chỉ với <b>375K</b>.<br/>Vui lòng chụp màn hình để giữ ưu đãi.<hr/><b>THÔNG TIN CỦA BẠN<br/></b>Họ tên: <b>" + _fullname + "</b><br/>Điện thoại: <b>" + _sdt + "</b><br/>Zalo: <b>" + _zalo + "</b>", "false", "false", "OK", "alert", ""), true);
                }
                //if (!str_cl.KiemTra_SDT(_zalo))
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Số Zalo không hợp lệ", "1000", "alert"), true);
                //else
                //{
                //    if (!dt_cl.check_date(_ngaysinh))
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Ngày sinh không hợp lệ", "1000", "alert"), true);
                //    else
                //    {

                //    }
                //}
            }

        }
    }
}