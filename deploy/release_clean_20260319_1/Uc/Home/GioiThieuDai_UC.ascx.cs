using System;
using System.Web.UI;

public partial class Uc_Home_GioiThieuDai_UC : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            BindAboutContent();
        }
        catch (Exception _ex)
        {
            string _tk = Session["taikhoan"] as string;
            if (!string.IsNullOrEmpty(_tk))
                _tk = mahoa_cl.giaima_Bcorn(_tk);
            else
                _tk = "";
            Log_cl.Add_Log(_ex.Message, _tk, _ex.StackTrace);
        }
    }

    private void BindAboutContent()
    {
        using (dbDataContext db = new dbDataContext())
        {
            HomeTextContent_cl.TextContentItem content = HomeTextContent_cl.GetEffectiveByKey(db, HomeTextContent_cl.KeyAboutLong);

            string title = string.IsNullOrWhiteSpace(content.Title)
                ? "AhaSale - Chợ Mua Bán, Rao Vặt Trực Tuyến Hàng Đầu Của Người Việt"
                : content.Title;

            lit_about_title.Text = Server.HtmlEncode(title);

            if (!content.IsEnabled)
            {
                lit_about_content.Text = "<p>Nội dung đang tạm ẩn.</p>";
                return;
            }

            lit_about_content.Text = HomeTextContent_cl.RenderPlainTextAsHtml(content.TextContent);
        }
    }
}
