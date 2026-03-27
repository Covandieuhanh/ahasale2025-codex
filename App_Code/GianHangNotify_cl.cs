using System;

public static class GianHangNotify_cl
{
    public static string BuildNeedHomeMessage()
    {
        return "Ban can dang nhap bang tai khoan Home de mo khong gian gian hang.";
    }

    public static string BuildRequestCreatedMessage()
    {
        return "Da gui yeu cau mo khong gian gian hang. Vui long cho admin duyet.";
    }

    public static string BuildPendingMessage()
    {
        return "Yeu cau mo gian hang dang cho duyet. Ban chua can gui lai.";
    }

    public static string BuildAlreadyActiveMessage()
    {
        return "Tai khoan nay da co quyen vao khong gian gian hang.";
    }

    public static string BuildBlockedMessage()
    {
        return "Khong gian gian hang cua tai khoan nay dang bi khoa. Vui long lien he admin.";
    }

    public static string BuildRevokedMessage()
    {
        return "Quyen vao khong gian gian hang da bi thu hoi. Vui long gui yeu cau moi hoac lien he admin.";
    }

    public static string BuildSuccessMessage(string message)
    {
        return string.IsNullOrWhiteSpace(message)
            ? "Gian hang dang xu ly thanh cong."
            : message.Trim();
    }

    public static string BuildWarningMessage(string message)
    {
        return string.IsNullOrWhiteSpace(message)
            ? "Gian hang can them dieu kien de tiep tuc."
            : message.Trim();
    }

    public static string BuildErrorMessage(string message)
    {
        return string.IsNullOrWhiteSpace(message)
            ? "Gian hang dang gap loi he thong."
            : message.Trim();
    }
}
