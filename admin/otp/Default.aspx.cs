using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_otp_Default : System.Web.UI.Page
{
    private const string ScopeHome = "home";
    private const string ScopeShop = "shop";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            check_login_cl.check_login_admin("1", "1");
            BindConfig();
            BindTypeOptions();
            BindAccounts();
            BindTabs();
            BindLog();
        }
    }

    private string CurrentScope
    {
        get
        {
            string scope = (Request.QueryString["scope"] ?? "").Trim().ToLowerInvariant();
            return scope == ScopeShop ? ScopeShop : ScopeHome;
        }
    }

    private void BindTabs()
    {
        string baseUrl = "/admin/otp/default.aspx";
        hl_tab_home.NavigateUrl = baseUrl + "?scope=home";
        hl_tab_shop.NavigateUrl = baseUrl + "?scope=shop";

        string scope = CurrentScope;
        if (scope == ScopeShop)
        {
            hl_tab_shop.CssClass = "otp-inline-tab active";
            hl_tab_home.CssClass = "otp-inline-tab";
        }
        else
        {
            hl_tab_home.CssClass = "otp-inline-tab active";
            hl_tab_shop.CssClass = "otp-inline-tab";
        }
    }

    private void BindConfig()
    {
        using (dbDataContext db = new dbDataContext())
        {
            SmsOtpConfig cfg = OtpConfig_cl.GetSmsConfig(db) ?? new SmsOtpConfig();
            txt_sms_endpoint.Text = cfg.Endpoint ?? "";
            txt_sms_apikey.Text = cfg.ApiKey ?? "";
            txt_sms_sender.Text = cfg.Sender ?? "";
            txt_sms_template.Text = cfg.Template ?? "";
            ck_sms_dev.Checked = cfg.DevMode;
            string method = (cfg.Method ?? "").Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(method)) method = "POST";
            if (ddl_sms_method.Items.FindByValue(method) != null)
                ddl_sms_method.SelectedValue = method;
            else
                ddl_sms_method.SelectedValue = "POST";
            BindParamInputs(cfg.Params);

            EmailOtpConfig emailCfg = OtpConfig_cl.GetEmailConfig(db) ?? new EmailOtpConfig();
            txt_email_host.Text = emailCfg.Host ?? "";
            txt_email_port.Text = emailCfg.Port > 0 ? emailCfg.Port.ToString() : "";
            txt_email_user.Text = emailCfg.Username ?? "";
            txt_email_pass.Text = emailCfg.Password ?? "";
            txt_email_from_name.Text = emailCfg.FromName ?? "";
            txt_email_from.Text = emailCfg.FromEmail ?? "";
            txt_email_subject.Text = string.IsNullOrEmpty(emailCfg.SubjectTemplate) ? "[AhaSale] Mã OTP của bạn" : emailCfg.SubjectTemplate;
            txt_email_body.Text = string.IsNullOrEmpty(emailCfg.BodyTemplate)
                ? "Ma OTP cua ban la: {OTP}. Het han sau {EXPIRE} phut."
                : emailCfg.BodyTemplate;
            ck_email_ssl.Checked = emailCfg.UseSsl;
            ck_email_dev.Checked = emailCfg.DevMode;
        }
    }

    protected void but_save_config_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            SmsOtpConfig cfg = new SmsOtpConfig
            {
                Endpoint = (txt_sms_endpoint.Text ?? "").Trim(),
                ApiKey = (txt_sms_apikey.Text ?? "").Trim(),
                Sender = (txt_sms_sender.Text ?? "").Trim(),
                Template = (txt_sms_template.Text ?? "").Trim(),
                Method = (ddl_sms_method.SelectedValue ?? "POST").Trim().ToUpperInvariant(),
                Params = BuildParamLines(),
                DevMode = ck_sms_dev.Checked
            };

            string adminTk = GetCurrentAdminAccount();
            OtpConfig_cl.SaveSmsConfig(db, cfg, adminTk);
        }

        Helper_Tabler_cl.ShowModal(this.Page, "Đã lưu cấu hình OTP.", "Thông báo", true, "success");
    }

    protected void but_save_email_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            int port = 0;
            int.TryParse((txt_email_port.Text ?? "").Trim(), out port);

            EmailOtpConfig cfg = new EmailOtpConfig
            {
                Host = (txt_email_host.Text ?? "").Trim(),
                Port = port,
                Username = (txt_email_user.Text ?? "").Trim(),
                Password = (txt_email_pass.Text ?? "").Trim(),
                FromName = (txt_email_from_name.Text ?? "").Trim(),
                FromEmail = (txt_email_from.Text ?? "").Trim(),
                SubjectTemplate = (txt_email_subject.Text ?? "").Trim(),
                BodyTemplate = (txt_email_body.Text ?? "").Trim(),
                UseSsl = ck_email_ssl.Checked,
                DevMode = ck_email_dev.Checked
            };

            string adminTk = GetCurrentAdminAccount();
            OtpConfig_cl.SaveEmailConfig(db, cfg, adminTk);
        }

        Helper_Tabler_cl.ShowModal(this.Page, "Đã lưu cấu hình Email OTP.", "Thông báo", true, "success");
    }

    private void BindParamInputs(string raw)
    {
        var pairs = ParseParamLines(raw);
        if (pairs.Count == 0)
        {
            pairs.Add(new KeyValuePair<string, string>("loginName", ""));
            pairs.Add(new KeyValuePair<string, string>("sign", ""));
            pairs.Add(new KeyValuePair<string, string>("serviceTypeId", ""));
            pairs.Add(new KeyValuePair<string, string>("phoneNumber", "{phoneNumber}"));
            pairs.Add(new KeyValuePair<string, string>("message", "{message}"));
            pairs.Add(new KeyValuePair<string, string>("brandName", "{brandName}"));
        }

        for (int i = 1; i <= 6; i++)
        {
            SetParamRow(i, "", "");
        }

        int index = 1;
        foreach (var pair in pairs)
        {
            if (index > 6) break;
            SetParamRow(index, pair.Key, pair.Value);
            index++;
        }
    }

    private string BuildParamLines()
    {
        var lines = new List<string>();
        for (int i = 1; i <= 6; i++)
        {
            TextBox keyBox = GetParamKeyBox(i);
            TextBox valBox = GetParamValBox(i);
            if (keyBox == null || valBox == null) continue;

            string key = (keyBox.Text ?? "").Trim();
            string value = (valBox.Text ?? "").Trim();
            if (string.IsNullOrEmpty(key)) continue;
            lines.Add(key + "=" + value);
        }
        return string.Join("\n", lines.ToArray());
    }

    private List<KeyValuePair<string, string>> ParseParamLines(string raw)
    {
        var result = new List<KeyValuePair<string, string>>();
        if (string.IsNullOrWhiteSpace(raw)) return result;
        string[] lines = raw.Replace("\r", "").Split('\n');
        foreach (string lineRaw in lines)
        {
            string line = (lineRaw ?? "").Trim();
            if (string.IsNullOrEmpty(line)) continue;
            int idx = line.IndexOf('=');
            if (idx <= 0) continue;
            string key = line.Substring(0, idx).Trim();
            string val = line.Substring(idx + 1).Trim();
            if (string.IsNullOrEmpty(key)) continue;
            result.Add(new KeyValuePair<string, string>(key, val));
        }
        return result;
    }

    private void SetParamRow(int index, string key, string value)
    {
        TextBox keyBox = GetParamKeyBox(index);
        TextBox valBox = GetParamValBox(index);
        if (keyBox != null) keyBox.Text = key ?? "";
        if (valBox != null) valBox.Text = value ?? "";
    }

    private TextBox GetParamKeyBox(int index)
    {
        switch (index)
        {
            case 1: return txt_param_key_1;
            case 2: return txt_param_key_2;
            case 3: return txt_param_key_3;
            case 4: return txt_param_key_4;
            case 5: return txt_param_key_5;
            case 6: return txt_param_key_6;
            default: return null;
        }
    }

    private TextBox GetParamValBox(int index)
    {
        switch (index)
        {
            case 1: return txt_param_val_1;
            case 2: return txt_param_val_2;
            case 3: return txt_param_val_3;
            case 4: return txt_param_val_4;
            case 5: return txt_param_val_5;
            case 6: return txt_param_val_6;
            default: return null;
        }
    }

    private void BindTypeOptions()
    {
        ddl_home_type.Items.Clear();
        ddl_home_type.Items.Add(new System.Web.UI.WebControls.ListItem("Mật khẩu", HomeOtp_cl.TypePassword));
        ddl_home_type.Items.Add(new System.Web.UI.WebControls.ListItem("Mã PIN", HomeOtp_cl.TypePin));
    }

    private void BindAccounts()
    {
        using (dbDataContext db = new dbDataContext())
        {
            var homeAccounts = db.taikhoan_tbs
                .Select(p => new { p.taikhoan, p.hoten, p.dienthoai, p.phanloai, p.permission })
                .ToList()
                .Where(p => PortalScope_cl.CanLoginHome(p.taikhoan, p.phanloai, p.permission))
                .OrderBy(p => p.taikhoan)
                .Select(p => new { p.taikhoan, p.hoten, p.dienthoai })
                .ToList();

            ddl_home_account.Items.Clear();
            ddl_home_account.Items.Add(new System.Web.UI.WebControls.ListItem("-- Chọn tài khoản --", ""));
            foreach (var acc in homeAccounts)
            {
                string label = acc.taikhoan + (string.IsNullOrWhiteSpace(acc.hoten) ? "" : " - " + acc.hoten);
                ddl_home_account.Items.Add(new System.Web.UI.WebControls.ListItem(label, acc.taikhoan));
            }

            var shopAccounts = db.taikhoan_tbs
                .Select(p => new { p.taikhoan, p.hoten, p.dienthoai, p.phanloai, p.permission })
                .ToList()
                .Where(p => PortalScope_cl.CanLoginShop(p.taikhoan, p.phanloai, p.permission))
                .OrderBy(p => p.taikhoan)
                .Select(p => new { p.taikhoan, p.hoten, p.dienthoai })
                .ToList();

            ddl_shop_account.Items.Clear();
            ddl_shop_account.Items.Add(new System.Web.UI.WebControls.ListItem("-- Chọn tài khoản --", ""));
            foreach (var acc in shopAccounts)
            {
                string label = acc.taikhoan + (string.IsNullOrWhiteSpace(acc.hoten) ? "" : " - " + acc.hoten);
                ddl_shop_account.Items.Add(new System.Web.UI.WebControls.ListItem(label, acc.taikhoan));
            }
        }
    }

    protected void ddl_home_account_SelectedIndexChanged(object sender, EventArgs e)
    {
        string tk = ddl_home_account.SelectedValue ?? "";
        if (string.IsNullOrEmpty(tk))
        {
            txt_home_phone.Text = "";
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            var acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (acc != null)
                txt_home_phone.Text = acc.dienthoai ?? "";
        }
    }

    protected void ddl_shop_account_SelectedIndexChanged(object sender, EventArgs e)
    {
        string tk = ddl_shop_account.SelectedValue ?? "";
        if (string.IsNullOrEmpty(tk))
        {
            txt_shop_phone.Text = "";
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            var acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (acc != null)
                txt_shop_phone.Text = acc.dienthoai ?? "";
        }
    }

    protected void but_home_manual_Click(object sender, EventArgs e)
    {
        string tk = (ddl_home_account.SelectedValue ?? "").Trim();
        string phone = (txt_home_phone.Text ?? "").Trim();
        string otpType = (ddl_home_type.SelectedValue ?? "").Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(tk) || string.IsNullOrEmpty(phone))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn tài khoản và nhập số điện thoại.", "Thông báo", true, "warning");
            return;
        }

        if (!AccountAuth_cl.IsValidPhone(phone))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Số điện thoại không hợp lệ.", "Thông báo", true, "warning");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (acc == null || !PortalScope_cl.CanLoginHome(acc.taikhoan, acc.phanloai, acc.permission))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản không thuộc hệ home.", "Thông báo", true, "warning");
                return;
            }

            int requestId;
            string otp;
            string error;
            if (!HomeOtp_cl.CreateManualOtp(db, phone, tk, otpType, out requestId, out otp, out error))
            {
                Helper_Tabler_cl.ShowModal(this.Page, error, "Thông báo", true, "warning");
                return;
            }

            BindLog();
            string msg = "OTP: " + otp + " (ID " + requestId + ")";
            Helper_Tabler_cl.ShowModal(this.Page, msg, "OTP Home", true, "success");
        }
    }

    protected void but_shop_manual_Click(object sender, EventArgs e)
    {
        string tk = (ddl_shop_account.SelectedValue ?? "").Trim();
        string phone = (txt_shop_phone.Text ?? "").Trim();
        string otpType = ShopOtp_cl.TypePassword;

        if (string.IsNullOrEmpty(tk) || string.IsNullOrEmpty(phone))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Vui lòng chọn tài khoản và nhập số điện thoại.", "Thông báo", true, "warning");
            return;
        }

        if (!AccountAuth_cl.IsValidPhone(phone))
        {
            Helper_Tabler_cl.ShowModal(this.Page, "Số điện thoại không hợp lệ.", "Thông báo", true, "warning");
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == tk);
            if (acc == null || !PortalScope_cl.CanLoginShop(acc.taikhoan, acc.phanloai, acc.permission))
            {
                Helper_Tabler_cl.ShowModal(this.Page, "Tài khoản không thuộc hệ shop.", "Thông báo", true, "warning");
                return;
            }

            int requestId;
            string otp;
            string error;
            if (!ShopOtp_cl.CreateManualOtp(db, phone, tk, otpType, out requestId, out otp, out error))
            {
                Helper_Tabler_cl.ShowModal(this.Page, error, "Thông báo", true, "warning");
                return;
            }

            BindLog();
            string msg = "OTP: " + otp + " (ID " + requestId + ")";
            Helper_Tabler_cl.ShowModal(this.Page, msg, "OTP Shop", true, "success");
        }
    }

    protected void but_search_Click(object sender, EventArgs e)
    {
        BindLog();
    }

    protected void but_clear_Click(object sender, EventArgs e)
    {
        txt_search.Text = "";
        BindLog();
    }

    private void BindLog()
    {
        string scope = CurrentScope;
        string keyword = (txt_search.Text ?? "").Trim();
        DataTable table = FetchOtpLog(scope, keyword);
        RepeaterOtp.DataSource = table;
        RepeaterOtp.DataBind();
        ph_empty.Visible = table == null || table.Rows.Count == 0;
    }

    private DataTable FetchOtpLog(string scope, string keyword)
    {
        string tableName = scope == ScopeShop ? "Shop_Otp_tb" : "Home_Otp_tb";
        string search = (keyword ?? "").Trim();

        using (dbDataContext db = new dbDataContext())
        {
            if (scope == ScopeShop)
                ShopOtp_cl.EnsureSchemaSafe(db);
            else
                HomeOtp_cl.EnsureSchemaSafe(db);

            using (SqlConnection conn = new SqlConnection(db.Connection.ConnectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
            cmd.CommandText = @"
SELECT TOP 200 id, taikhoan, phone, otp_code, otp_type, status, sent_at, expires_at
FROM dbo." + tableName + @"
WHERE (@kw = '' OR taikhoan LIKE @kw OR phone LIKE @kw)
ORDER BY id DESC";
            cmd.Parameters.AddWithValue("@kw", string.IsNullOrEmpty(search) ? "" : "%" + search + "%");
            conn.Open();

            DataTable dt = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            dt.Columns.Add("type_text", typeof(string));
            dt.Columns.Add("status_text", typeof(string));
            dt.Columns.Add("status_class", typeof(string));
            dt.Columns.Add("time_text", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                string otpType = Convert.ToString(row["otp_type"] ?? "");
                int status = Convert.ToInt32(row["status"] ?? 0);
                DateTime sentAt = Convert.ToDateTime(row["sent_at"] ?? DateTime.MinValue);
                DateTime expAt = Convert.ToDateTime(row["expires_at"] ?? DateTime.MinValue);

                row["type_text"] = GetTypeText(otpType);
                row["status_text"] = GetStatusText(status);
                row["status_class"] = GetStatusClass(status);
                row["time_text"] = sentAt.ToString("dd/MM/yyyy HH:mm") + " (HSD " + expAt.ToString("dd/MM/yyyy HH:mm") + ")";
            }

                return dt;
            }
        }
    }

    private string GetTypeText(string otpType)
    {
        if (string.Equals(otpType, HomeOtp_cl.TypePin, StringComparison.OrdinalIgnoreCase))
            return "Mã PIN";
        return "Mật khẩu";
    }

    private string GetStatusText(int status)
    {
        switch (status)
        {
            case HomeOtp_cl.StatusVerified: return "Đã xác thực";
            case HomeOtp_cl.StatusConsumed: return "Đã dùng";
            case HomeOtp_cl.StatusExpired: return "Hết hạn";
            case HomeOtp_cl.StatusLocked: return "Bị khóa";
            case HomeOtp_cl.StatusSendFailed: return "Gửi thất bại";
            default: return "Đã gửi";
        }
    }

    private string GetStatusClass(int status)
    {
        switch (status)
        {
            case HomeOtp_cl.StatusVerified:
            case HomeOtp_cl.StatusConsumed:
                return "ok";
            case HomeOtp_cl.StatusExpired:
            case HomeOtp_cl.StatusLocked:
            case HomeOtp_cl.StatusSendFailed:
                return "err";
            default:
                return "warn";
        }
    }

    private string GetCurrentAdminAccount()
    {
        string taiKhoanMaHoa = Session["taikhoan"] as string;
        if (string.IsNullOrEmpty(taiKhoanMaHoa))
            return "";

        return mahoa_cl.giaima_Bcorn(taiKhoanMaHoa);
    }
}
