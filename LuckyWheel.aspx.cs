using System;
using System.Web;
using System.Web.Services;

public partial class LuckyWheel : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e) { }

    [WebMethod] // bỏ EnableSession
    public static int GetForcedWinnerIndexAndConsume()
    {
        var obj = HttpContext.Current.Application["ForcedWinnerIndex"];
        HttpContext.Current.Application["ForcedWinnerIndex"] = null; // xóa sau khi đọc

        if (obj == null) return -1;

        int idx;
        return (int.TryParse(obj.ToString(), out idx) && idx >= 1) ? idx : -1;
    }


}
