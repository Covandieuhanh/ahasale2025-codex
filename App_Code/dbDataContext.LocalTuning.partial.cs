using System;
using System.Data.SqlClient;

public partial class dbDataContext
{
    partial void OnCreated()
    {
        try
        {
            SqlConnection sqlConnection = this.Connection as SqlConnection;
            if (sqlConnection != null)
            {
                string raw = sqlConnection.ConnectionString ?? "";
                if (raw.IndexOf("Data Source=localhost\\SQLEXPRESS", StringComparison.OrdinalIgnoreCase) >= 0
                    || raw.IndexOf("Initial Catalog=ahasale.vn_old", StringComparison.OrdinalIgnoreCase) >= 0
                    || raw.IndexOf("Initial Catalog=ahasale.vn_old", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var builder = new SqlConnectionStringBuilder(raw);
                    builder.Pooling = false;
                    builder.ConnectTimeout = 5;
                    builder.ApplicationName = "AhaSaleServer";
                    sqlConnection.ConnectionString = builder.ConnectionString;
                }
            }

            if (this.CommandTimeout <= 0 || this.CommandTimeout > 30)
                this.CommandTimeout = 30;
        }
        catch
        {
        }
    }
}


