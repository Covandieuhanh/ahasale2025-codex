using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

public partial class Uc_Home_TuKhoaPhoBien_UC : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            BindKeywords();
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

    private void BindKeywords()
    {
        using (dbDataContext db = new dbDataContext())
        {
            HomeTextContent_cl.TextContentItem content = HomeTextContent_cl.GetEffectiveByKey(db, HomeTextContent_cl.KeyPopularKeywords);
            lit_title_keywords.Text = Server.HtmlEncode(string.IsNullOrWhiteSpace(content.Title) ? "Các từ khóa phổ biến" : content.Title);

            List<HomeTextContent_cl.KeywordLineItem> allItems = content.IsEnabled
                ? HomeTextContent_cl.ParseKeywordLines(content.TextContent)
                : new List<HomeTextContent_cl.KeywordLineItem>();

            var col1 = new List<HomeTextContent_cl.KeywordLineItem>();
            var col2 = new List<HomeTextContent_cl.KeywordLineItem>();
            var col3 = new List<HomeTextContent_cl.KeywordLineItem>();
            var col4 = new List<HomeTextContent_cl.KeywordLineItem>();

            for (int i = 0; i < allItems.Count; i++)
            {
                int mod = i % 4;
                if (mod == 0) col1.Add(allItems[i]);
                else if (mod == 1) col2.Add(allItems[i]);
                else if (mod == 2) col3.Add(allItems[i]);
                else col4.Add(allItems[i]);
            }

            rpt_col_1.DataSource = col1;
            rpt_col_1.DataBind();
            rpt_col_2.DataSource = col2;
            rpt_col_2.DataBind();
            rpt_col_3.DataSource = col3;
            rpt_col_3.DataBind();
            rpt_col_4.DataSource = col4;
            rpt_col_4.DataBind();
        }
    }
}
