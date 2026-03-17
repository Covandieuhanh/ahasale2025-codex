<%@ Page Language="C#" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<script runat="server">
    protected string OutputText = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        string host = (Request.Url.Host ?? "").ToLowerInvariant();
        if (!Request.IsLocal && host != "ahasale.local" && host != "localhost")
        {
            Response.StatusCode = 404;
            Response.End();
            return;
        }

        Response.ContentType = "text/plain; charset=utf-8";

        try
        {
            AhaShineSchema_cl.ResetSchemaCache();
            AhaShineContext_cl.EnsureSchemaAndDefaults();

            using (dbDataContext db = new dbDataContext())
            {
                SqlConnection connection = db.Connection as SqlConnection;
                if (connection == null)
                    throw new InvalidOperationException("db.Connection is not SqlConnection.");

                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                string sql = @"
IF OBJECT_ID('dbo.web_post_table', 'U') IS NULL
    THROW 60001, 'Missing web_post_table (AhaShine posts)', 1;

IF OBJECT_ID('dbo.bspa_datlich_table', 'U') IS NULL
    THROW 60002, 'Missing bspa_datlich_table (AhaShine booking)', 1;

IF OBJECT_ID('dbo.bspa_hoadon_table', 'U') IS NULL
    THROW 60003, 'Missing bspa_hoadon_table (AhaShine invoices)', 1;

IF OBJECT_ID('dbo.taikhoan_table_2023', 'U') IS NULL
    THROW 60004, 'Missing taikhoan_table_2023 (AhaShine staff)', 1;

IF COL_LENGTH('dbo.web_post_table', 'id_baiviet') IS NULL
    THROW 60005, 'Missing id_baiviet sync column', 1;

SELECT CAST(1 AS int);";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandTimeout = 30;
                    object scalar = command.ExecuteScalar();
                    OutputText = "ok=" + Convert.ToString(scalar ?? "0");
                }
            }
        }
        catch (Exception ex)
        {
            Response.StatusCode = 500;
            OutputText = "ERROR\n" + ex.GetType().FullName + "\n" + ex.Message;
        }
    }
</script>
<%= OutputText %>
