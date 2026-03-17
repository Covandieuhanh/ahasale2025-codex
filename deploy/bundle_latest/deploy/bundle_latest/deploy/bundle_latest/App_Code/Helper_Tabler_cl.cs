using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

public static class Helper_Tabler_cl
{
    public static void ShowToast(Page page, string message, string type = "primary",
                                 bool autohide = true, int delay = 3000, string title = "")
    {
        string msg = HttpUtility.JavaScriptStringEncode(message ?? "");
        string ttl = HttpUtility.JavaScriptStringEncode(title ?? "");
        string auto = autohide ? "true" : "false";

        // ĐỢI ĐẾN KHI show_toast CÓ MẶT (sau khi JS load xong) RỒI MỚI GỌI
        string js = string.Format(@"
(function() {{
    function _doShow() {{
        if (window.show_toast) {{
            show_toast('{0}','{1}',{2},{3},'{4}');
            return;
        }}
        if (window.Metro && Metro.notify && typeof Metro.notify.create === 'function') {{
            Metro.notify.create('{0}','{4}', {{keepOpen: false, cls: '{1}'}});
            return;
        }}
        alert(('{4}' ? '{4}' + ':\n' : '') + '{0}');
    }}
    if (document.readyState === 'complete') _doShow();
    else window.addEventListener('load', _doShow);
}})();", msg, type, auto, delay, ttl);

        var sm = ScriptManager.GetCurrent(page);
        string key = "toast_" + Guid.NewGuid().ToString("N");
        if (sm != null)
            ScriptManager.RegisterStartupScript(page, page.GetType(), key, js, true);
        else
            page.ClientScript.RegisterStartupScript(page.GetType(), key, js, true);
    }

    public static void ShowModal(Page page, string message, string title = "Thông báo",
                                 bool allowBackdropClose = true, string type = "primary")
    {
        string msg = HttpUtility.JavaScriptStringEncode(message ?? "");
        string ttl = HttpUtility.JavaScriptStringEncode(title ?? "");
        string allow = allowBackdropClose ? "true" : "false";

        string js = string.Format(@"
(function() {{
    function _doShow() {{
        if (window.show_modal) {{
            show_modal('{0}','{1}',{2},'{3}');
            return;
        }}
        if (window.Metro && Metro.dialog && typeof Metro.dialog.create === 'function') {{
            Metro.dialog.create({{
                title: '{1}',
                content: '<div>{0}</div>',
                closeButton: true,
                overlayClickClose: {2},
                actions: [{{caption: 'OK', cls: 'js-dialog-close {3}'}}]
            }});
            return;
        }}
        alert(('{1}' ? '{1}' + ':\n' : '') + '{0}');
    }}
    if (document.readyState === 'complete') _doShow();
    else window.addEventListener('load', _doShow);
}})();", msg, ttl, allow, type);

        var sm = ScriptManager.GetCurrent(page);
        string key = "modal_" + Guid.NewGuid().ToString("N");
        if (sm != null)
            ScriptManager.RegisterStartupScript(page, page.GetType(), key, js, true);
        else
            page.ClientScript.RegisterStartupScript(page.GetType(), key, js, true);
    }

    public static void ShowModalTwoButtons(Page page, string message, string title,
                                           bool allowBackdropClose, string type,
                                           string primaryText, string primaryHref,
                                           string secondaryText, string secondaryHref)
    {
        string msg = HttpUtility.JavaScriptStringEncode(message ?? "");
        string ttl = HttpUtility.JavaScriptStringEncode(title ?? "");
        string allow = allowBackdropClose ? "true" : "false";
        string pText = HttpUtility.JavaScriptStringEncode(primaryText ?? "");
        string pHref = HttpUtility.JavaScriptStringEncode(primaryHref ?? "");
        string sText = HttpUtility.JavaScriptStringEncode(secondaryText ?? "");
        string sHref = HttpUtility.JavaScriptStringEncode(secondaryHref ?? "");

        string js = string.Format(@"
(function() {{
    function _doShow() {{
        if (window.show_modal_2btn) {{
            show_modal_2btn('{0}','{1}',{2},'{3}','{4}','{5}','{6}','{7}');
        }} else {{
            setTimeout(_doShow, 100);
        }}
    }}
    if (document.readyState === 'complete') _doShow();
    else window.addEventListener('load', _doShow);
}})();", msg, ttl, allow, type, pText, pHref, sText, sHref);

        var sm = ScriptManager.GetCurrent(page);
        string key = "modal2_" + Guid.NewGuid().ToString("N");
        if (sm != null)
            ScriptManager.RegisterStartupScript(page, page.GetType(), key, js, true);
        else
            page.ClientScript.RegisterStartupScript(page.GetType(), key, js, true);
    }

    /// <summary>
    /// Đọc Session["ThongBao_Admin"] dạng: MODE|TITLE|MESSAGE|TYPE|DELAY
    /// và tự động gọi ShowToast / ShowModal.
    /// </summary>
    public static void ShowThongBaoSession(Page page)
    {
        ShowThongBaoSessionKey(page, "ThongBao_Admin");
    }

    public static void ShowThongBaoSessionShop(Page page)
    {
        if (ShowThongBaoSessionKey(page, "ThongBao_Shop"))
            return;

        var raw = page.Session["thongbao_shop"] as string;
        if (string.IsNullOrEmpty(raw))
            return;

        page.Session["thongbao_shop"] = null;
        ShowLegacyThongBaoShop(page, raw);
    }

    private static bool ShowThongBaoSessionKey(Page page, string key)
    {
        var raw = page.Session[key] as string;
        if (string.IsNullOrEmpty(raw))
            return false;

        // dùng 1 lần rồi xóa
        page.Session[key] = null;
        ShowThongBaoRaw(page, raw);
        return true;
    }

    private static void ShowThongBaoRaw(Page page, string raw)
    {
        // MODE|TITLE|MESSAGE|TYPE|DELAY
        var parts = (raw ?? "").Split('|');
        if (parts.Length < 3) return;

        string mode = parts[0];                       // "toast" hoặc "modal"
        string title = parts[1];
        string message = parts[2];
        string type = (parts.Length > 3 ? parts[3] : "primary");

        int delay = 3000;
        if (parts.Length > 4)
        {
            int.TryParse(parts[4], out delay);
            if (delay <= 0) delay = 3000;
        }

        if (string.Equals(mode, "modal", StringComparison.OrdinalIgnoreCase))
        {
            ShowModal(page, message, title, true, type);
        }
        else // mặc định toast
        {
            ShowToast(page, message, type, true, delay, title);
        }
    }

    private static void ShowLegacyThongBaoShop(Page page, string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return;

        string msg = "";
        string title = "Thông báo";
        string type = "warning";
        int delay = 2000;

        try
        {
            if (raw.IndexOf("notify.create", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var notifyMatch = Regex.Match(raw,
                    @"notify\.create\(\s*'(?<msg>.*?)'\s*,\s*'(?<title>.*?)'\s*,\s*\{cls:\s*'(?<cls>[^']*)'\}\s*\)",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (notifyMatch.Success)
                {
                    msg = UnescapeLegacy(notifyMatch.Groups["msg"].Value);
                    title = UnescapeLegacy(notifyMatch.Groups["title"].Value);
                    type = MapLegacyType(notifyMatch.Groups["cls"].Value);
                }

                var timeoutMatch = Regex.Match(raw, @"timeout\s*:\s*(?<ms>\d+)", RegexOptions.IgnoreCase);
                if (timeoutMatch.Success)
                {
                    int parsed;
                    if (int.TryParse(timeoutMatch.Groups["ms"].Value, out parsed) && parsed > 0)
                        delay = parsed;
                }

                if (string.IsNullOrEmpty(msg))
                    msg = "Vui lòng đăng nhập lại.";

                ShowToast(page, msg, type, true, delay, title);
                return;
            }

            if (raw.IndexOf("Metro.dialog.create", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var dialogMatch = Regex.Match(raw,
                    @"Metro\.dialog\.create\(\{.*?title:\s*'(?<title>.*?)'.*?content:\s*'<div>(?<msg>.*?)</div>'.*?\}\);",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (dialogMatch.Success)
                {
                    title = UnescapeLegacy(dialogMatch.Groups["title"].Value);
                    msg = UnescapeLegacy(dialogMatch.Groups["msg"].Value);
                }

                var clsMatch = Regex.Match(raw, @"cls:\s*'(?<cls>[^']*)'", RegexOptions.IgnoreCase);
                if (clsMatch.Success)
                    type = MapLegacyType(clsMatch.Groups["cls"].Value);

                if (string.IsNullOrEmpty(msg))
                    msg = "Vui lòng đăng nhập lại.";

                ShowModal(page, msg, title, true, type);
                return;
            }
        }
        catch
        {
            // ignore parsing errors, fallback below
        }

        // Fallback: hiển thị text thô nếu có
        string fallback = raw;
        fallback = Regex.Replace(fallback, @"<script.*?>.*?</script>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        fallback = fallback.Trim();
        if (fallback == "")
            fallback = "Vui lòng đăng nhập lại.";
        ShowToast(page, fallback, "warning", true, delay, title);
    }

    private static string MapLegacyType(string cls)
    {
        string v = (cls ?? "").ToLowerInvariant();
        if (v.Contains("danger") || v.Contains("alert") || v.Contains("error"))
            return "danger";
        if (v.Contains("success"))
            return "success";
        if (v.Contains("info"))
            return "info";
        if (v.Contains("primary"))
            return "primary";
        if (v.Contains("warning"))
            return "warning";
        return "warning";
    }

    private static string UnescapeLegacy(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";
        return value.Replace("\\'", "'").Replace("\\\"", "\"");
    }
}
