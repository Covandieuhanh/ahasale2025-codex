using System;
using System.Linq;
using System.Text;
using System.Web;

public partial class home_luu_danh_ba : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string user = (Request.QueryString["user"] ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(user))
        {
            Response.Redirect("/", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            taikhoan_tb acc = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == user);
            if (acc == null || acc.block == true)
            {
                Response.Redirect("/", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!PortalScope_cl.CanLoginHome(acc.taikhoan, acc.phanloai, acc.permission))
            {
                Response.Redirect("/", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            string fullName = string.IsNullOrWhiteSpace(acc.hoten) ? acc.taikhoan : acc.hoten.Trim();
            string phone = (acc.dienthoai ?? "").Trim();
            string email = (acc.email ?? "").Trim();
            string address = (acc.diachi ?? "").Trim();
            string profileUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/" + acc.taikhoan.Trim().ToLowerInvariant() + ".info";

            string vcard = BuildVCard(fullName, phone, email, address, profileUrl);

            string fileName = "ahasale-" + acc.taikhoan.Trim().ToLowerInvariant() + ".vcf";
            Response.Clear();
            Response.ContentType = "text/vcard";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(vcard);
            Response.End();
        }
    }

    private static string BuildVCard(string fullName, string phone, string email, string address, string url)
    {
        string safeName = EscapeVCard(fullName);
        string safePhone = EscapeVCard(phone);
        string safeEmail = EscapeVCard(email);
        string safeAddress = EscapeVCard(address);
        string safeUrl = EscapeVCard(url);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("BEGIN:VCARD");
        sb.AppendLine("VERSION:3.0");
        sb.AppendLine("FN:" + safeName);
        sb.AppendLine("N:" + safeName + ";;;;");
        if (!string.IsNullOrEmpty(safePhone))
            sb.AppendLine("TEL;TYPE=CELL:" + safePhone);
        if (!string.IsNullOrEmpty(safeEmail))
            sb.AppendLine("EMAIL;TYPE=INTERNET:" + safeEmail);
        if (!string.IsNullOrEmpty(safeAddress))
            sb.AppendLine("ADR;TYPE=HOME:;;" + safeAddress + ";;;;");
        if (!string.IsNullOrEmpty(safeUrl))
            sb.AppendLine("URL:" + safeUrl);
        sb.AppendLine("END:VCARD");
        return sb.ToString();
    }

    private static string EscapeVCard(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";

        string value = input.Replace("\\", "\\\\")
            .Replace(";", "\\;")
            .Replace(",", "\\,")
            .Replace("\r\n", "\\n")
            .Replace("\n", "\\n")
            .Replace("\r", "\\n");
        return value;
    }
}
