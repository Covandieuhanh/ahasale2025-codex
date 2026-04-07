using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

public partial class admin_quan_ly_noi_dung_home_bds_lien_ket_tin : System.Web.UI.Page
{
    private const bool SafeMode = false;

    private sealed class FeedLogView
    {
        public string RanAtText { get; set; }
        public string SourceLabel { get; set; }
        public int Created { get; set; }
        public int Updated { get; set; }
        public int Expired { get; set; }
        public string StatusText { get; set; }
        public string StatusBadge { get; set; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        AdminAccessGuard_cl.RequireFeatureAccess("home_bds_linked", "/admin/default.aspx?mspace=batdongsan");
        if (!IsPostBack)
        {
            if (SafeMode)
            {
                lb_result.Text = "Trang đang chạy ở chế độ an toàn để tránh timeout. Tạm thời đã tắt đọc log, danh sách nguồn và crawler ngay trên giao diện này.";
                BindSafeMode();
                return;
            }

            lb_result.Text = "Trang đã tải xong. Đồng bộ nguồn và crawl feed chỉ chạy khi bạn bấm nút thủ công để tránh treo trang quản trị.";
            try
            {
                BindSources();
                LoadLogs();
            }
            catch (Exception ex)
            {
                lb_result.Text = "Trang đã tải nhưng không đọc được log/nguồn lúc này: " + Server.HtmlEncode(ex.Message);
            }
        }
    }

    protected void btn_sync_sources_Click(object sender, EventArgs e)
    {
        if (SafeMode)
        {
            lb_result.Text = "Chế độ an toàn đang bật nên tạm thời khóa đồng bộ nguồn trên giao diện này.";
            BindSafeMode();
            return;
        }

        try
        {
            string message;
            LinkedFeedSync_cl.TryQueueSourceCatalogSync(out message);
            lb_result.Text = Server.HtmlEncode(message);
        }
        catch (Exception ex)
        {
            lb_result.Text = "Lỗi đồng bộ nguồn: " + Server.HtmlEncode(ex.Message);
        }

        BindSources();
    }

    protected void btn_refresh_Click(object sender, EventArgs e)
    {
        if (SafeMode)
        {
            lb_result.Text = "Chế độ an toàn đang bật nên tạm thời khóa crawler trên giao diện này.";
            BindSafeMode();
            return;
        }

        string source = (ddl_source.SelectedValue ?? "").Trim();
        try
        {
            string message;
            LinkedFeedSync_cl.TryQueueManualSync(source, out message);
            lb_result.Text = Server.HtmlEncode(message);
        }
        catch (Exception ex)
        {
            lb_result.Text = "Lỗi khi chạy cập nhật: " + Server.HtmlEncode(ex.Message);
        }

        LoadLogs();
    }

    protected void btn_reload_Click(object sender, EventArgs e)
    {
        if (SafeMode)
        {
            BindSafeMode();
            return;
        }

        LoadLogs();
    }

    private void BindSafeMode()
    {
        ddl_source.Items.Clear();
        ddl_source.Items.Add(new ListItem("Tạm tắt danh sách nguồn ở safe mode", ""));
        rpt_logs.DataSource = new List<FeedLogView>();
        rpt_logs.DataBind();
        pn_empty.Visible = true;
    }

    private void LoadLogs()
    {
        List<FeedLogView> logs;
        using (dbDataContext db = new dbDataContext())
        {
            logs = LinkedFeedStore_cl.TryGetRecentLogsFast(db, 20)
                .Select(x => new FeedLogView
                {
                    RanAtText = x.RanAt.ToString("dd/MM HH:mm"),
                    SourceLabel = x.SourceLabel,
                    Created = x.Created,
                    Updated = x.Updated,
                    Expired = x.Expired,
                    StatusText = x.StatusText,
                    StatusBadge = x.IsSuccess ? "badge-success" : "badge-danger"
                })
                .ToList();
        }

        rpt_logs.DataSource = logs;
        rpt_logs.DataBind();
        pn_empty.Visible = logs.Count == 0;
    }

    private void BindSources()
    {
        ddl_source.Items.Clear();
        ddl_source.Items.Add(new ListItem("Tất cả nguồn đang bật", ""));

        using (dbDataContext db = new dbDataContext())
        {
            var sources = LinkedFeedStore_cl.TryGetActiveSourcesFast(db);
            for (int i = 0; i < sources.Count; i++)
            {
                var s = sources[i];
                if (s == null || string.IsNullOrWhiteSpace(s.SourceKey))
                    continue;

                string label = string.IsNullOrWhiteSpace(s.SourceLabel) ? s.SourceKey : s.SourceLabel;
                ddl_source.Items.Add(new ListItem(label, s.SourceKey));
            }
        }
    }
}
