using System;
using System.Web;

public partial class CoCauTrungThuong : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e) { }

    protected void btnSet_Click(object sender, EventArgs e)
    {
        int idx;
        if (int.TryParse(txtIndex.Text, out idx) && idx >= 1)
        {
            // Lưu toàn cục cho toàn site, sẽ bị tiêu thụ ở lượt quay tiếp theo
            Application["ForcedWinnerIndex"] = idx;

            ltStatus.Text = string.Format("<div class='ok'>✅ Đã đặt cơ cấu: vị trí <strong>{0}</strong> cho lượt quay kế tiếp.</div>", idx);
        }
        else
        {
            ltStatus.Text = "<div class='err'>❌ Vui lòng nhập số nguyên ≥ 1.</div>";
        }
    }
}
