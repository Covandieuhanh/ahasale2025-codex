<%@ Page Language="C#" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<script runat="server">
    protected string OutputHtml = "";

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

        var lines = new System.Collections.Generic.List<string>();
        try
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["hot79334_aha_saleConnectionString"];
            string rawConnectionString = settings == null ? "" : settings.ConnectionString ?? "";
            lines.Add("Host: " + host);
            lines.Add("Configured connection: " + SanitizeConnectionString(rawConnectionString));

            using (dbDataContext db = new dbDataContext())
            {
                SqlConnection connection = db.Connection as SqlConnection;
                lines.Add("Resolved connection: " + SanitizeConnectionString(connection == null ? "" : connection.ConnectionString ?? ""));

                if (connection == null)
                {
                    lines.Add("Status: db.Connection is not SqlConnection");
                }
                else
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    lines.Add("Status: OPEN");
                    lines.Add("Database: " + connection.Database);
                    lines.Add("Server: " + connection.DataSource);
                    connection.Close();
                }
            }
        }
        catch (Exception ex)
        {
            lines.Add("Status: ERROR");
            lines.Add("Type: " + ex.GetType().FullName);
            lines.Add("Message: " + ex.Message);
            Exception inner = ex.InnerException;
            int guard = 0;
            while (inner != null && guard < 5)
            {
                lines.Add("Inner[" + guard + "]: " + inner.GetType().FullName + " | " + inner.Message);
                inner = inner.InnerException;
                guard++;
            }
        }

        OutputHtml = string.Join("\n", lines.ToArray());
    }

    private static string SanitizeConnectionString(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "(empty)";

        try
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(raw);
            if (!string.IsNullOrWhiteSpace(builder.Password))
                builder.Password = "***";
            return builder.ConnectionString;
        }
        catch
        {
            return raw.Replace("Password=", "Password=***");
        }
    }
</script>
<%= OutputHtml %>
