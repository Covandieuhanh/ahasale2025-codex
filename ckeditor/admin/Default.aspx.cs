using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_Default : System.Web.UI.Page
{
    private const int BridgeRecentLimit = 8;
    private const string BridgeWatcherStateRelativePath = "~/scripts/usdt_bridge_state.json";

    private class BridgeHistoryRowView
    {
        public long id { get; set; }
        public string time_text { get; set; }
        public string direction { get; set; }
        public decimal token_amount { get; set; }
        public decimal points_credited { get; set; }
        public string status { get; set; }
        public string tx_hash { get; set; }
        public string tx_hash_short { get; set; }
    }

    private class BridgeWatcherBalanceView
    {
        public bool HasBalance { get; set; }
        public decimal TokenBalance { get; set; }
        public long SafeBlock { get; set; }
        public int TokenDecimals { get; set; }
        public string Note { get; set; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["url_back"] = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            check_login_cl.check_login_admin("none", "none");

            if (Session["title"] != null)
                ViewState["title"] = Session["title"].ToString();
        }

        try
        {
            LoadBridgeSummary();
        }
        catch (Exception ex)
        {
            SafeLog(ex);
            lb_bridge_status_note.Text = "Khong doc duoc du lieu bridge: " + ex.Message;
        }
    }

    private void LoadBridgeSummary()
    {
        string treasuryAccount = (USDTBridgeConfig_cl.TreasuryAccount ?? "").Trim();
        lb_bridge_treasury_account.Text = treasuryAccount;
        lb_bridge_deposit_address.Text = (USDTBridgeConfig_cl.DepositAddress ?? "").Trim();
        lb_bridge_token_contract.Text = (USDTBridgeConfig_cl.TokenContract ?? "").Trim();
        lb_bridge_enabled.Text = USDTBridgeConfig_cl.Enabled ? "Bat" : "Tat";
        lb_bridge_point_rate.Text = USDTBridgeConfig_cl.PointRatePerToken.ToString("#,##0.######");
        lb_bridge_min_confirmations.Text = USDTBridgeConfig_cl.MinConfirmations.ToString("#,##0");

        lb_bridge_treasury_name.Text = "";
        lb_bridge_points_now.Text = "0";
        lb_bridge_points_db.Text = "0";
        lb_bridge_token_balance_now.Text = "0";
        lb_bridge_safe_block.Text = "-";
        lb_bridge_status_note.Text = "";
        RepeaterBridge.DataSource = null;
        RepeaterBridge.DataBind();

        BridgeWatcherBalanceView watcherBalance = ReadBridgeWatcherBalance();
        using (dbDataContext db = new dbDataContext())
        {
            decimal dbPoints = 0m;
            var treasury = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == treasuryAccount);
            if (treasury != null)
            {
                lb_bridge_treasury_name.Text = (treasury.hoten ?? "").Trim();
                dbPoints = treasury.DongA ?? 0m;
                lb_bridge_points_db.Text = dbPoints.ToString("#,##0.##");
            }
            else
            {
                lb_bridge_treasury_name.Text = "(Khong tim thay tai khoan tong)";
                lb_bridge_points_db.Text = "0";
                AppendBridgeStatusNote("Khong tim thay tai khoan tong de doi chieu diem noi bo.");
            }

            if (watcherBalance.HasBalance)
            {
                decimal chainPoints = Math.Round(
                    watcherBalance.TokenBalance * USDTBridgeConfig_cl.PointRatePerToken,
                    2,
                    MidpointRounding.AwayFromZero);

                lb_bridge_token_balance_now.Text = watcherBalance.TokenBalance.ToString("#,##0.########");
                lb_bridge_safe_block.Text = watcherBalance.SafeBlock > 0
                    ? watcherBalance.SafeBlock.ToString("#,##0")
                    : "-";
                lb_bridge_points_now.Text = chainPoints.ToString("#,##0.##");

                if (treasury != null && Math.Abs(chainPoints - dbPoints) >= 0.01m)
                {
                    AppendBridgeStatusNote(
                        "Diem DB chua khop so du blockchain. "
                        + "Blockchain="
                        + chainPoints.ToString("#,##0.##")
                        + ", DB="
                        + dbPoints.ToString("#,##0.##"));
                }
            }
            else
            {
                lb_bridge_points_now.Text = dbPoints.ToString("#,##0.##");
                AppendBridgeStatusNote(watcherBalance.Note);
            }

            try
            {
                List<BridgeHistoryRowView> viewRows = new List<BridgeHistoryRowView>();
                if (db.Connection.State != ConnectionState.Open)
                    db.Connection.Open();

                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT TOP (@TopN)
                          id,
                          tx_hash,
                          status,
                          usdt_amount,
                          points_credited,
                          created_at,
                          credited_at
                      FROM dbo.USDT_Deposit_Bridge_tb
                      ORDER BY id DESC",
                    (SqlConnection)db.Connection))
                {
                    cmd.Parameters.AddWithValue("@TopN", BridgeRecentLimit);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime? createdAt = reader["created_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["created_at"]);
                            DateTime? creditedAt = reader["credited_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["credited_at"]);
                            string txHash = (reader["tx_hash"] ?? "").ToString();
                            decimal points = reader["points_credited"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["points_credited"]);

                            viewRows.Add(new BridgeHistoryRowView
                            {
                                id = reader["id"] == DBNull.Value ? 0L : Convert.ToInt64(reader["id"]),
                                time_text = (creditedAt ?? createdAt).HasValue
                                    ? (creditedAt ?? createdAt).Value.ToString("dd/MM/yyyy HH:mm:ss")
                                    : "",
                                direction = points < 0m ? "OUT" : "IN",
                                token_amount = reader["usdt_amount"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["usdt_amount"]),
                                points_credited = points,
                                status = (reader["status"] ?? "").ToString().Trim(),
                                tx_hash = txHash.Trim(),
                                tx_hash_short = ShortTxHash(txHash)
                            });
                        }
                    }
                }

                RepeaterBridge.DataSource = viewRows;
                RepeaterBridge.DataBind();

                if (viewRows.Count == 0)
                    AppendBridgeStatusNote("Chua co giao dich bridge nao.");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 208)
                    AppendBridgeStatusNote("Chua co bang bridge. Hay chay scripts/sql/create-usdt-point-bridge.sql truoc.");
                else
                    AppendBridgeStatusNote("Khong doc duoc lich su bridge: " + ex.Message);
            }
        }
    }

    protected void but_refresh_bridge_Click(object sender, EventArgs e)
    {
        LoadBridgeSummary();
    }

    private static string ShortTxHash(string txHash)
    {
        string s = (txHash ?? "").Trim();
        if (s == "")
            return "-";
        if (s.Length <= 20)
            return s;
        return s.Substring(0, 10) + "..." + s.Substring(s.Length - 8);
    }

    private void SafeLog(Exception ex)
    {
        try
        {
            string tk = Session["taikhoan"] as string;
            tk = !string.IsNullOrEmpty(tk) ? mahoa_cl.giaima_Bcorn(tk) : "";
            Log_cl.Add_Log(ex.Message, tk, ex.StackTrace);
        }
        catch
        {
        }
    }

    private void AppendBridgeStatusNote(string note)
    {
        if (string.IsNullOrWhiteSpace(note))
            return;

        string clean = note.Trim();
        if (string.IsNullOrWhiteSpace(lb_bridge_status_note.Text))
        {
            lb_bridge_status_note.Text = clean;
            return;
        }

        if (lb_bridge_status_note.Text.IndexOf(clean, StringComparison.OrdinalIgnoreCase) >= 0)
            return;

        lb_bridge_status_note.Text = lb_bridge_status_note.Text.Trim() + " | " + clean;
    }

    private BridgeWatcherBalanceView ReadBridgeWatcherBalance()
    {
        BridgeWatcherBalanceView result = new BridgeWatcherBalanceView
        {
            HasBalance = false,
            TokenBalance = 0m,
            SafeBlock = 0L,
            TokenDecimals = 18,
            Note = ""
        };

        try
        {
            string statePath = Server.MapPath(BridgeWatcherStateRelativePath);
            if (string.IsNullOrWhiteSpace(statePath))
            {
                result.Note = "Khong xac dinh duoc duong dan file state watcher.";
                return result;
            }

            if (!File.Exists(statePath))
            {
                result.Note = "Khong co state watcher tren host (watcher co the dang chay may khac). Dang hien thi diem theo DB.";
                return result;
            }

            string json = File.ReadAllText(statePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                result.Note = "File state watcher dang rong.";
                return result;
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> state = serializer.DeserializeObject(json) as Dictionary<string, object>;
            if (state == null)
            {
                result.Note = "State watcher sai dinh dang JSON.";
                return result;
            }

            string rawBalanceText = GetStateString(state, "last_confirmed_balance_raw");
            if (string.IsNullOrWhiteSpace(rawBalanceText))
            {
                result.Note = "Watcher chua co so du baseline tren blockchain.";
                return result;
            }

            decimal rawBalance;
            if (!decimal.TryParse(rawBalanceText, NumberStyles.Integer, CultureInfo.InvariantCulture, out rawBalance) || rawBalance < 0m)
            {
                result.Note = "Khong doc duoc last_confirmed_balance_raw tu state watcher.";
                return result;
            }

            int tokenDecimals = GetStateInt(state, "last_token_decimals", 18);
            if (tokenDecimals < 0 || tokenDecimals > 36)
                tokenDecimals = 18;

            decimal scale = Pow10(tokenDecimals);
            result.TokenBalance = scale > 0m ? rawBalance / scale : rawBalance;
            result.SafeBlock = GetStateLong(state, "last_balance_safe_block", 0L);
            result.TokenDecimals = tokenDecimals;
            result.HasBalance = true;
            return result;
        }
        catch (Exception ex)
        {
            result.Note = "Khong doc duoc state watcher: " + ex.Message;
            return result;
        }
    }

    private static string GetStateString(Dictionary<string, object> state, string key)
    {
        object value;
        if (state == null || !state.TryGetValue(key, out value) || value == null)
            return "";
        return Convert.ToString(value, CultureInfo.InvariantCulture).Trim();
    }

    private static int GetStateInt(Dictionary<string, object> state, string key, int fallback)
    {
        long value = GetStateLong(state, key, fallback);
        if (value < int.MinValue || value > int.MaxValue)
            return fallback;
        return Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private static long GetStateLong(Dictionary<string, object> state, string key, long fallback)
    {
        object value;
        if (state == null || !state.TryGetValue(key, out value) || value == null)
            return fallback;

        try
        {
            if (value is long)
                return (long)value;
            if (value is int)
                return (int)value;
            if (value is decimal)
                return Convert.ToInt64((decimal)value, CultureInfo.InvariantCulture);
            if (value is double)
                return Convert.ToInt64((double)value, CultureInfo.InvariantCulture);
            if (value is float)
                return Convert.ToInt64((float)value, CultureInfo.InvariantCulture);

            string text = Convert.ToString(value, CultureInfo.InvariantCulture);
            long parsed;
            if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed))
                return parsed;
        }
        catch
        {
        }

        return fallback;
    }

    private static decimal Pow10(int exponent)
    {
        decimal result = 1m;
        for (int i = 0; i < exponent; i++)
            result *= 10m;
        return result;
    }
}
