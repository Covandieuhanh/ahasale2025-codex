using System;
using System.Linq;

public partial class admin_duyet_nang_cap_level2 : System.Web.UI.Page
{
    private string GetAdminName()
    {
        string admin = Session["admin"] as string;
        if (string.IsNullOrEmpty(admin))
            admin = Session["taikhoan_admin"] as string;
        return admin ?? "";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        AdminRolePolicy_cl.RequireSuperAdmin();

        if (!IsPostBack)
        {
            txt_timkiem.Text = "";
            ddl_trangthai.SelectedValue = "";
            LoadDanhSach();
        }
    }

    private void LoadDanhSach()
    {
        using (dbDataContext db = new dbDataContext())
        {
            string keyword = txt_timkiem.Text.Trim();
            int statusValue;
            int? status = int.TryParse(ddl_trangthai.SelectedValue, out statusValue) ? (int?)statusValue : null;

            var list = ShopLevel2Request_cl.LoadRequests(db, keyword, status)
                .Select(x => new
                {
                    x.ID,
                    TaiKhoan = x.taikhoan,
                    x.TrangThai,
                    x.NgayTao,
                    x.NgayDuyet,
                    x.AdminDuyet,
                    x.GhiChuAdmin
                })
                .ToList();

            rp_level2.DataSource = list;
            rp_level2.DataBind();
            pn_empty.Visible = list.Count == 0;
        }
    }

    protected void btn_timkiem_Click(object sender, EventArgs e)
    {
        LoadDanhSach();
    }

    protected void ddl_trangthai_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadDanhSach();
    }

    protected void btn_reset_Click(object sender, EventArgs e)
    {
        txt_timkiem.Text = "";
        ddl_trangthai.SelectedValue = "";
        LoadDanhSach();
    }

    protected void btn_duyet_Click(object sender, EventArgs e)
    {
        AdminRolePolicy_cl.RequireSuperAdmin();
        int id;
        if (!int.TryParse(((System.Web.UI.WebControls.LinkButton)sender).CommandArgument, out id))
        {
            Helper_Tabler_cl.ShowToast(this, "Không xác định được yêu cầu.", "warning");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            string msg;
            if (!ShopLevel2Request_cl.ApproveRequest(db, id, GetAdminName(), out msg))
            {
                Helper_Tabler_cl.ShowToast(this, string.IsNullOrWhiteSpace(msg) ? "Không thể duyệt yêu cầu." : msg, "warning");
                LoadDanhSach();
                return;
            }
        }

        Helper_Tabler_cl.ShowToast(this, "Đã duyệt nâng cấp Level 2.", "success");
        LoadDanhSach();
    }

    protected void btn_tuchoi_Click(object sender, EventArgs e)
    {
        AdminRolePolicy_cl.RequireSuperAdmin();
        int id;
        if (!int.TryParse(((System.Web.UI.WebControls.LinkButton)sender).CommandArgument, out id))
        {
            Helper_Tabler_cl.ShowToast(this, "Không xác định được yêu cầu.", "warning");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            string msg;
            if (!ShopLevel2Request_cl.RejectRequest(db, id, GetAdminName(), "", out msg))
            {
                Helper_Tabler_cl.ShowToast(this, string.IsNullOrWhiteSpace(msg) ? "Không thể từ chối yêu cầu." : msg, "warning");
                LoadDanhSach();
                return;
            }
        }

        Helper_Tabler_cl.ShowToast(this, "Đã từ chối yêu cầu nâng cấp Level 2.", "warning");
        LoadDanhSach();
    }
}
