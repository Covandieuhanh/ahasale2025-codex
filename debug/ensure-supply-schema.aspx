<%@ Page Language="C#" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Linq" %>
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
            using (dbDataContext db = new dbDataContext())
            {
                AhaShineSupplySchema_cl.ResetCache();
                AhaShineSupplySchema_cl.EnsureSafe(db);
                int userParentCol = db.ExecuteQuery<int>("SELECT CASE WHEN COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'user_parent') IS NULL THEN 0 ELSE 1 END").FirstOrDefault();
                int idHoaDonCol = db.ExecuteQuery<int>("SELECT CASE WHEN COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'id_hoadon') IS NULL THEN 0 ELSE 1 END").FirstOrDefault();
                int giaDvCol = db.ExecuteQuery<int>("SELECT CASE WHEN COL_LENGTH('dbo.donnhap_vattu_chitiet_table', 'gia_dvsp_taithoidiemnay') IS NULL THEN 0 ELSE 1 END").FirstOrDefault();
                OutputText = string.Format("OK\nuser_parent={0}\nid_hoadon={1}\ngia_dvsp_taithoidiemnay={2}", userParentCol, idHoaDonCol, giaDvCol);
            }
        }
        catch (Exception ex)
        {
            OutputText = "ERROR\n" + ex.GetType().FullName + "\n" + ex.Message;
        }
    }
</script>
<%= OutputText %>
