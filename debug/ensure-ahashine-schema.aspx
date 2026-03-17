<%@ Page Language="C#" %>
<%@ Import Namespace="System" %>
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
            OutputText = "OK";
        }
        catch (Exception ex)
        {
            OutputText = "ERROR\n" + ex.GetType().FullName + "\n" + ex.Message;
        }
    }
</script>
<%= OutputText %>
