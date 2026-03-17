using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_email_shop_Default : System.Web.UI.Page
{
    private sealed class TemplateRowVm
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
        check_login_cl.check_login_admin("none", "none");

        if (!IsPostBack)
        {
            string code = (Request.QueryString["code"] ?? "").Trim().ToLowerInvariant();
            BindAll(code);
        }
    }

    protected void but_select_template_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("none", "none");
        LinkButton button = sender as LinkButton;
        string code = button == null ? "" : (button.CommandArgument ?? "").Trim().ToLowerInvariant();
        BindAll(code);
    }

    protected void but_template_save_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("none", "none");

        string code = (hf_template_code.Value ?? "").Trim().ToLowerInvariant();
        if (!ShopEmailTemplate_cl.IsValidCode(code))
        {
            ShowDialog("Không xác định được template cần lưu.");
            return;
        }

        string name = (txt_template_name.Text ?? "").Trim();
        string subject = (txt_template_subject.Text ?? "").Trim();
        string body = txt_template_body.Text ?? "";
        bool isActive = chk_template_active.Checked;

        if (string.IsNullOrWhiteSpace(subject))
        {
            ShowDialog("Tiêu đề email không được để trống.");
            return;
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            ShowDialog("Nội dung email không được để trống.");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            ShopEmailTemplate_cl.Upsert(db, code, name, subject, body, isActive, GetCurrentAdminUser());
            ShowNotifi("Đã lưu nội dung email.");
            BindAll(code);
        }
    }

    protected void but_template_reset_Click(object sender, EventArgs e)
    {
        check_login_cl.check_login_admin("none", "none");

        string code = (hf_template_code.Value ?? "").Trim().ToLowerInvariant();
        if (!ShopEmailTemplate_cl.IsValidCode(code))
        {
            ShowDialog("Không xác định được template cần khôi phục.");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            bool ok = ShopEmailTemplate_cl.ResetToBuiltIn(db, code, GetCurrentAdminUser());
            if (!ok)
            {
                ShowDialog("Không thể khôi phục template này.");
                return;
            }

            ShowNotifi("Đã khôi phục nội dung mặc định.");
            BindAll(code);
        }
    }

    private void BindAll(string preferredCode)
    {
        using (dbDataContext db = new dbDataContext())
        {
            List<ShopEmailTemplate_cl.TemplateItem> items = ShopEmailTemplate_cl.GetAll(db);
            string selectedCode = ResolveSelection(items, preferredCode);

            rpt_templates.DataSource = items.Select(x => new TemplateRowVm
            {
                Code = x.Code,
                Name = string.IsNullOrWhiteSpace(x.Name) ? x.Code : x.Name,
                IsActive = x.IsActive
            }).ToList();
            rpt_templates.DataBind();

            BindEditor(db, selectedCode);
        }
    }

    private void BindEditor(dbDataContext db, string code)
    {
        ShopEmailTemplate_cl.TemplateItem item = ShopEmailTemplate_cl.GetEffectiveByCode(db, code);
        if (item == null)
        {
            hf_template_code.Value = "";
            txt_template_code.Text = "";
            txt_template_name.Text = "";
            txt_template_subject.Text = "";
            txt_template_body.Text = "";
            chk_template_active.Checked = true;
            lit_template_updated_info.Text = "Chưa có dữ liệu.";
            return;
        }

        hf_template_code.Value = item.Code;
        txt_template_code.Text = item.Code;
        txt_template_name.Text = item.Name;
        txt_template_subject.Text = item.Subject;
        txt_template_body.Text = item.Body;
        chk_template_active.Checked = item.IsActive;

        if (item.UpdatedAt.HasValue)
        {
            lit_template_updated_info.Text = "Cập nhật: <b>" + item.UpdatedAt.Value.ToString("dd/MM/yyyy HH:mm") +
                "</b> bởi <b>" + Server.HtmlEncode(item.UpdatedBy ?? "") + "</b>";
        }
        else
        {
            lit_template_updated_info.Text = "Đang dùng nội dung mặc định hệ thống.";
        }
    }

    private string ResolveSelection(List<ShopEmailTemplate_cl.TemplateItem> items, string preferredCode)
    {
        string code = (preferredCode ?? "").Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(code) && items.Any(x => string.Equals(x.Code, code, StringComparison.OrdinalIgnoreCase)))
            return code;
        return items.Select(x => x.Code).FirstOrDefault() ?? ShopEmailTemplate_cl.CodeOrderCreated;
    }

    private string GetCurrentAdminUser()
    {
        string encoded = Session["taikhoan"] as string;
        if (string.IsNullOrWhiteSpace(encoded)) return "";
        return mahoa_cl.giaima_Bcorn(encoded);
    }

    private void ShowDialog(string content)
    {
        ScriptManager.RegisterStartupScript(
            this.Page,
            this.GetType(),
            Guid.NewGuid().ToString(),
            thongbao_class.metro_dialog("Thông báo", content, "false", "false", "OK", "alert", ""),
            true);
    }

    private void ShowNotifi(string content)
    {
        ScriptManager.RegisterStartupScript(
            this.Page,
            this.GetType(),
            Guid.NewGuid().ToString(),
            thongbao_class.metro_notifi("Thông báo", content, "1000", "warning"),
            true);
    }
}
