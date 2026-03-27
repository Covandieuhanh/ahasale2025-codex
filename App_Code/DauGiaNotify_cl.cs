using System;

public static class DauGiaNotify_cl
{
    public static string BuildSuccessMessage(string message)
    {
        return string.IsNullOrWhiteSpace(message)
            ? "Đấu giá xử lý thành công."
            : message.Trim();
    }

    public static string BuildWarningMessage(string message)
    {
        return string.IsNullOrWhiteSpace(message)
            ? "Đấu giá cần thêm điều kiện để tiếp tục."
            : message.Trim();
    }

    public static string BuildErrorMessage(string message)
    {
        return string.IsNullOrWhiteSpace(message)
            ? "Đấu giá đang gặp lỗi hệ thống."
            : message.Trim();
    }

    public static void PushInAppNotice(
        dbDataContext db,
        string fromAccount,
        string toAccount,
        string link,
        string content)
    {
        if (db == null)
            return;

        string receiver = (toAccount ?? "").Trim().ToLowerInvariant();
        if (receiver == "")
            return;

        try
        {
            db.ExecuteCommand(@"
INSERT INTO dbo.ThongBao_tb
(
    id, nguoithongbao, nguoinhan, link, noidung, thoigian, daxem, bin
)
VALUES
(
    {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}
)
", Guid.NewGuid(),
   string.IsNullOrWhiteSpace(fromAccount) ? "admin" : fromAccount.Trim().ToLowerInvariant(),
   receiver,
   string.IsNullOrWhiteSpace(link) ? "/daugia" : link.Trim(),
   BuildSuccessMessage(content),
   AhaTime_cl.Now,
   false,
   false);
        }
        catch (Exception ex)
        {
            Log_cl.Add_Log(ex.Message, "daugia_notify", ex.StackTrace);
        }
    }
}
