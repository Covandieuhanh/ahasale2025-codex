using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class congcu_tienich_quayso : System.Web.UI.Page
{
    public class NhanVien
    {
        public int Ma { get; set; }
        public string Ten { get; set; }
    }
    // Danh sách các tên nhân viên được chia sẻ toàn trang
    List<string> employeeNames = new List<string>();

    public string notifi = "";

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            // Lần đầu tiên trang được tải
            but_dunglai.Visible = false; // Ẩn nút "Dừng lại" khi bắt đầu
        }
    }
    protected void Timer1_Tick(object sender, EventArgs e)
    {
        var employeeNames = Session["EmployeeNames"] as List<string>;

        if (employeeNames != null && employeeNames.Count > 0)
        {
            Random rnd = new Random();
            int index = rnd.Next(0, employeeNames.Count); // Chọn ngẫu nhiên chỉ số
            Label1.Text = employeeNames[index]; // Hiển thị tên
        }
    }

    protected void but_batdau_Click(object sender, EventArgs e)
    {
        // Lấy danh sách tên từ TextBox
        string textContent = txtNhanVien.Text.ToUpper();
        string[] lines = textContent.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        employeeNames = new List<string>(lines); // Chuyển thành danh sách
        //Application["CoCau"] = 4;
        Session["EmployeeNames"] = employeeNames; // Lưu vào Session

        if (employeeNames.Count > 0)
        {
            Timer1.Enabled = true; // Bắt đầu Timer
            but_batdau.Visible = false; // Ẩn nút "Bắt đầu"
            but_dunglai.Visible = true; // Hiện nút "Dừng lại"
        }
        else
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(), thongbao_class.metro_notifi("Thông báo", "Vui lòng nhập dữ liệu.", "1000", "warning"), true);
    }

    protected void but_dunglai_Click(object sender, EventArgs e)
    {

        // CƠ CẤU
        if (Application["CoCau"] != null)
        {
            
            Timer1.Enabled = false; // Dừng Timer
            but_batdau.Visible = true; // Hiện nút "Bắt đầu"
            but_dunglai.Visible = false; // Ẩn nút "Dừng lại"
            

            var employeeNames = Session["EmployeeNames"] as List<string>;
            int CoCau = int.Parse(Application["CoCau"].ToString()) - 1;
            if (employeeNames != null && CoCau < employeeNames.Count)
            {
                Label1.Text = employeeNames[CoCau]; // Trả về phần tử dựa trên chỉ số quy định trước
            }
            else//nếu chỉ số cơ cấu vượt quá list thì coi như k cơ cấu
            {
                Timer1.Enabled = false; // Dừng Timer
                but_batdau.Visible = true; // Hiện nút "Bắt đầu"
                but_dunglai.Visible = false; // Ẩn nút "Dừng lại"
            }
            Session["EmployeeNames"] = null;
        }
        else
        {
            Timer1.Enabled = false; // Dừng Timer
            but_batdau.Visible = true; // Hiện nút "Bắt đầu"
            but_dunglai.Visible = false; // Ẩn nút "Dừng lại"
            Session["EmployeeNames"] = null;

        }
    }
}