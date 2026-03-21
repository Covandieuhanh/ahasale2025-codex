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

        ThongBao_tb notice = new ThongBao_tb();
        notice.id = Guid.NewGuid();
        notice.nguoithongbao = string.IsNullOrWhiteSpace(fromAccount) ? "admin" : fromAccount.Trim().ToLowerInvariant();
        notice.nguoinhan = receiver;
        notice.link = string.IsNullOrWhiteSpace(link) ? "/daugia" : link.Trim();
        notice.noidung = BuildSuccessMessage(content);
        notice.thoigian = AhaTime_cl.Now;
        notice.daxem = false;
        notice.bin = false;
        db.ThongBao_tbs.InsertOnSubmit(notice);
        db.SubmitChanges();
    }
}
