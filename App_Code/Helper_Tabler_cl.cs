using System;
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
        string js = $@"
(function() {{
    function _doShow() {{
        if (window.show_toast) {{
            show_toast('{msg}','{type}',{auto},{delay},'{ttl}');
        }} else {{
            // chưa có hàm -> chờ thêm 100ms
            setTimeout(_doShow, 100);
        }}
    }}
    if (document.readyState === 'complete') _doShow();
    else window.addEventListener('load', _doShow);
}})();";

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

        string js = $@"
(function() {{
    function _doShow() {{
        if (window.show_modal) {{
            show_modal('{msg}','{ttl}',{allow},'{type}');
        }} else {{
            setTimeout(_doShow, 100);
        }}
    }}
    if (document.readyState === 'complete') _doShow();
    else window.addEventListener('load', _doShow);
}})();";

        var sm = ScriptManager.GetCurrent(page);
        string key = "modal_" + Guid.NewGuid().ToString("N");
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
        var raw = page.Session["ThongBao_Admin"] as string;
        if (string.IsNullOrEmpty(raw))
            return;

        // dùng 1 lần rồi xóa
        page.Session["ThongBao_Admin"] = null;

        // MODE|TITLE|MESSAGE|TYPE|DELAY
        var parts = raw.Split('|');
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
}
