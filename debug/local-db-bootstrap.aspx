<%@ Page Language="C#" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Configuration" %>
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

        var lines = new System.Collections.Generic.List<string>();
        try
        {
            string requested = (Request.QueryString["run"] ?? "").Trim();
            lines.Add("Host: " + host);
            if (requested != "1")
            {
                lines.Add("Dry run. Add ?run=1 to restore ahasale_local from /var/opt/mssql/backup/ahasale.bak");
                OutputText = string.Join("\n", lines.ToArray());
                return;
            }

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["hot79334_aha_saleConnectionString"];
            if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new InvalidOperationException("Missing hot79334_aha_saleConnectionString.");

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(settings.ConnectionString);
            string targetDatabase = builder.InitialCatalog;
            builder.InitialCatalog = "master";
            builder.Pooling = false;
            builder.ConnectTimeout = 5;

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                lines.Add("Connected to server: " + connection.DataSource);
                lines.Add("Target database: " + targetDatabase);

                string databaseState = GetDatabaseState(connection, targetDatabase);
                lines.Add("Current state: " + (databaseState == "" ? "(missing)" : databaseState));

                if (databaseState == "ONLINE")
                {
                    lines.Add("Database already exists and is online. Nothing to do.");
                }
                else
                {
                    string backupPath = "/var/opt/mssql/backup/ahasale.bak";
                    string[] logicalNames = ReadLogicalNames(connection, backupPath);
                    string dataLogical = logicalNames[0];
                    string logLogical = logicalNames[1];
                    string restoreSql = string.Format(
                        "RESTORE DATABASE [{0}] FROM DISK='{1}' WITH MOVE '{2}' TO '/var/opt/mssql/data/{0}.mdf', MOVE '{3}' TO '/var/opt/mssql/data/{0}_log.ldf', REPLACE, RECOVERY;",
                        targetDatabase.Replace("]", "]]"),
                        backupPath.Replace("'", "''"),
                        dataLogical.Replace("'", "''"),
                        logLogical.Replace("'", "''"));

                    using (SqlCommand restore = new SqlCommand(restoreSql, connection))
                    {
                        restore.CommandTimeout = 0;
                        restore.ExecuteNonQuery();
                    }

                    lines.Add("Restore completed.");
                }

                using (SqlCommand verify = new SqlCommand("SELECT name FROM sys.databases WHERE name=@db", connection))
                {
                    verify.Parameters.AddWithValue("@db", targetDatabase);
                    object value = verify.ExecuteScalar();
                    lines.Add("Verified database: " + (value == null ? "(missing)" : value.ToString()));
                }
            }
        }
        catch (Exception ex)
        {
            lines.Add("Status: ERROR");
            lines.Add("Type: " + ex.GetType().FullName);
            lines.Add("Message: " + ex.Message);
            Exception inner = ex.InnerException;
            int index = 0;
            while (inner != null && index < 5)
            {
                lines.Add("Inner[" + index + "]: " + inner.GetType().FullName + " | " + inner.Message);
                inner = inner.InnerException;
                index++;
            }
        }

        OutputText = string.Join("\n", lines.ToArray());
    }

    private static bool DatabaseExists(SqlConnection connection, string databaseName)
    {
        using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name=@db", connection))
        {
            command.Parameters.AddWithValue("@db", databaseName);
            object value = command.ExecuteScalar();
            return value != null && Convert.ToInt32(value) > 0;
        }
    }

    private static string GetDatabaseState(SqlConnection connection, string databaseName)
    {
        using (SqlCommand command = new SqlCommand("SELECT state_desc FROM sys.databases WHERE name=@db", connection))
        {
            command.Parameters.AddWithValue("@db", databaseName);
            object value = command.ExecuteScalar();
            return value == null ? "" : Convert.ToString(value) ?? "";
        }
    }

    private static string[] ReadLogicalNames(SqlConnection connection, string backupPath)
    {
        string dataLogical = "";
        string logLogical = "";

        using (SqlCommand command = new SqlCommand("RESTORE FILELISTONLY FROM DISK=@path", connection))
        {
            command.Parameters.AddWithValue("@path", backupPath);
            command.CommandTimeout = 0;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string type = Convert.ToString(reader["Type"]) ?? "";
                    string logicalName = Convert.ToString(reader["LogicalName"]) ?? "";
                    if (type == "D" && dataLogical == "")
                        dataLogical = logicalName;
                    if (type == "L" && logLogical == "")
                        logLogical = logicalName;
                }
            }
        }

        if (string.IsNullOrWhiteSpace(dataLogical))
            dataLogical = "app56734_test_gpt";
        if (string.IsNullOrWhiteSpace(logLogical))
            logLogical = "app56734_test_gpt_log";

        return new[] { dataLogical, logLogical };
    }
</script>
<%= OutputText %>
