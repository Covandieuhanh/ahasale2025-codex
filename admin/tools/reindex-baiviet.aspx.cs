using System;
using System.Web.UI;

public partial class admin_tools_reindex_baiviet : Page
{
    private const int BatchSize = 2000;
    private const int MaxSecondsPerRun = 8;

    protected void Page_Load(object sender, EventArgs e)
    {
        AdminRolePolicy_cl.RequireSuperAdmin();

        if (!IsPostBack)
            ShowStatus();
    }

    protected void btn_run_Click(object sender, EventArgs e)
    {
        RunReindexBatch();
    }

    protected void btn_run_all_Click(object sender, EventArgs e)
    {
        RunReindexAll();
    }

    private void ShowStatus()
    {
        using (var db = new dbDataContext())
        {
            BaiVietSearchSchema_cl.EnsureSchemaSafe(db);
            int remaining = BaiVietSearchSchema_cl.CountMissing(db);
            if (remaining <= 0)
            {
                lbl_status.Text = "Đã chuẩn hoá đầy đủ. Không còn bản ghi cần reindex.";
                btn_run.Text = "Chạy lại";
                btn_run_all.Text = "Reindex toàn bộ (1 lần)";
            }
            else
            {
                lbl_status.Text = "Còn " + remaining + " bản ghi cần chuẩn hoá.";
                btn_run.Text = "Chạy reindex";
                btn_run_all.Text = "Reindex toàn bộ (1 lần)";
            }
        }
    }

    private void RunReindexBatch()
    {
        int totalUpdated = 0;
        DateTime started = DateTime.UtcNow;

        using (var db = new dbDataContext())
        {
            BaiVietSearchSchema_cl.EnsureSchemaSafe(db);

            while (true)
            {
                int updated = BaiVietSearchSchema_cl.BackfillMissing(db, BatchSize, true);
                totalUpdated += updated;
                if (updated <= 0) break;

                if ((DateTime.UtcNow - started).TotalSeconds >= MaxSecondsPerRun)
                    break;
            }

            int remaining = BaiVietSearchSchema_cl.CountMissing(db);
            if (remaining <= 0)
            {
                lbl_status.Text = "Hoàn tất. Đã cập nhật " + totalUpdated + " bản ghi.";
                btn_run.Text = "Chạy lại";
            }
            else
            {
                lbl_status.Text = "Đã cập nhật " + totalUpdated + " bản ghi. Còn " + remaining + " bản ghi chưa chuẩn hoá.";
                btn_run.Text = "Chạy tiếp";
            }
        }
    }

    private void RunReindexAll()
    {
        int totalUpdated = 0;
        using (var db = new dbDataContext())
        {
            BaiVietSearchSchema_cl.EnsureSchemaSafe(db);
            totalUpdated = BaiVietSearchSchema_cl.BackfillAll(db, BatchSize);

            int remaining = BaiVietSearchSchema_cl.CountMissing(db);
            if (remaining <= 0)
            {
                lbl_status.Text = "Hoàn tất toàn bộ. Đã cập nhật " + totalUpdated + " bản ghi.";
                btn_run.Text = "Chạy lại";
                btn_run_all.Text = "Reindex toàn bộ (1 lần)";
            }
            else
            {
                lbl_status.Text = "Đã cập nhật " + totalUpdated + " bản ghi. Còn " + remaining + " bản ghi chưa chuẩn hoá.";
                btn_run.Text = "Chạy tiếp";
                btn_run_all.Text = "Reindex toàn bộ (1 lần)";
            }
        }
    }
}
