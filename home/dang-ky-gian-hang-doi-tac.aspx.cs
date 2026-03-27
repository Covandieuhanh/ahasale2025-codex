using System;
using System.Collections.Generic;
using System.Web;

public partial class home_dang_ky_gian_hang_doi_tac : System.Web.UI.Page
{
    private string _accountKey = "";
    private string _returnUrl = "/gianhang/";
    private string _profileReturnUrl = "/home/dang-ky-gian-hang-doi-tac.aspx";
    private GianHangOnboarding_cl.ShopInfoState _state;

    protected void Page_Load(object sender, EventArgs e)
    {
        DisablePageCache();
        check_login_cl.check_login_home("none", "none", true);

        _accountKey = (PortalRequest_cl.GetCurrentAccount() ?? "").Trim().ToLowerInvariant();
        _returnUrl = GianHangRoutes_cl.NormalizeReturnUrl(Request.QueryString["return_url"], "/gianhang/");
        _profileReturnUrl = "/home/dang-ky-gian-hang-doi-tac.aspx?return_url=" + Server.UrlEncode(_returnUrl);
        if (_accountKey == "")
        {
            Response.Redirect("/dang-nhap?return_url=" + Server.UrlEncode(Request.RawUrl ?? "/home/default.aspx"), true);
            return;
        }

        if (!IsPostBack)
            BindPage();
    }

    private void DisablePageCache()
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();
        Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
        Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
    }

    protected void btn_submit_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            string message;
            bool ok = GianHangOnboarding_cl.SubmitOnboarding(
                db,
                _accountKey,
                txt_shop_name.Text,
                _accountKey,
                out message);

            Helper_Tabler_cl.ShowToast(this, message, ok ? "success" : "warning", true, 2800, ok ? "Thành công" : "Thông báo");
        }

        BindPage();
    }

    protected string GetStepCss(bool isFirstStep)
    {
        if (_state == null)
            return "gianhang-step gianhang-step--active";

        if (isFirstStep)
            return (_state.IsPending || _state.IsActive)
                ? "gianhang-step gianhang-step--done"
                : "gianhang-step gianhang-step--active";

        return (_state.IsPending || _state.IsActive)
            ? "gianhang-step gianhang-step--active"
            : "gianhang-step";
    }

    private void BindPage()
    {
        using (dbDataContext db = new dbDataContext())
        {
            _state = GianHangOnboarding_cl.LoadState(db, _accountKey);
            GianHangOnboarding_cl.ShopInfoDraft draft = _state == null ? null : _state.Draft;

            lit_account_root.Text = "Tài khoản gốc: " + Server.HtmlEncode(_accountKey);
            txt_shop_name.Text = draft == null ? "" : draft.ShopName;
            lit_contact_name.Text = Server.HtmlEncode(draft == null ? "" : draft.FullName);
            lit_contact_email.Text = Server.HtmlEncode(draft == null ? "" : draft.ContactEmail);
            lit_contact_phone.Text = Server.HtmlEncode(draft == null ? "" : draft.ContactPhone);
            lit_contact_phone_dup.Text = Server.HtmlEncode(draft == null ? "" : draft.ContactPhone);
            lit_pickup_address.Text = Server.HtmlEncode(draft == null ? "" : draft.PickupAddress).Replace("\n", "<br />");

            string homeUrl = "/home/default.aspx";
            string editInfoUrl = "/home/edit-info.aspx?return_url=" + Server.UrlEncode(_profileReturnUrl);
            lnk_edit_profile_warning.NavigateUrl = editInfoUrl;
            lnk_edit_profile_inline.NavigateUrl = editInfoUrl;
            lnk_back_home_form.NavigateUrl = homeUrl;
            lnk_back_home_waiting.NavigateUrl = homeUrl;
            lnk_back_home_done.NavigateUrl = homeUrl;

            ph_form.Visible = !_state.IsPending && !_state.IsActive;
            ph_waiting.Visible = _state.IsPending;
            ph_done.Visible = _state.IsActive;

            ph_request_feedback.Visible = (_state.IsRejected || _state.IsBlocked) && ph_form.Visible;
            lit_request_feedback_title.Text = _state.IsBlocked ? "Quyền gian hàng đang bị khóa" : "Yêu cầu gần nhất đã bị từ chối";
            lit_request_feedback_body.Text = Server.HtmlEncode(BuildRequestFeedback(_state));

            ph_profile_warning.Visible = !string.IsNullOrWhiteSpace(_state.MissingProfileMessage) && ph_form.Visible;
            lit_profile_warning.Text = Server.HtmlEncode(_state.MissingProfileMessage);
            btn_submit.Enabled = _state.CanSubmit;

            lit_waiting_status.Text = Server.HtmlEncode(_state.RequestStatusText);
            ph_waiting_time.Visible = _state.RequestedAt.HasValue;
            lit_waiting_time.Text = _state.RequestedAt.HasValue ? _state.RequestedAt.Value.ToString("dd/MM/yyyy HH:mm") : "";
            ph_waiting_note.Visible = !string.IsNullOrWhiteSpace(_state.ReviewNote);
            lit_waiting_note.Text = Server.HtmlEncode(_state.ReviewNote);

            lit_done_status.Text = Server.HtmlEncode(_state.AccessStatusText);
            lnk_open_gianhang.NavigateUrl = _returnUrl;

            List<GianHangOnboarding_cl.RequestHistoryItem> history = GianHangOnboarding_cl.LoadHistory(db, _accountKey);
            ph_history_empty.Visible = history.Count == 0;
            rp_history.DataSource = history;
            rp_history.DataBind();
        }
    }

    private static string BuildRequestFeedback(GianHangOnboarding_cl.ShopInfoState state)
    {
        if (state == null)
            return "";

        string note = (state.ReviewNote ?? "").Trim();
        if (state.IsBlocked)
        {
            if (note != "")
                return "Quyền /gianhang của tài khoản này đã từng bị khóa hoặc thu hồi. Ghi chú admin: " + note + ". Bạn có thể cập nhật lại thông tin shop rồi gửi yêu cầu mới để được xét duyệt lại.";
            return "Quyền /gianhang của tài khoản này đã từng bị khóa hoặc thu hồi. Bạn có thể cập nhật lại thông tin shop rồi gửi yêu cầu mới để được xét duyệt lại.";
        }

        if (state.IsRejected)
        {
            if (note != "")
                return "Yêu cầu mở gian hàng gần nhất chưa được duyệt. Ghi chú admin: " + note + ". Bạn có thể chỉnh lại thông tin shop rồi gửi lại.";
            return "Yêu cầu mở gian hàng gần nhất chưa được duyệt. Bạn có thể chỉnh lại thông tin shop rồi gửi lại.";
        }

        return "";
    }
}
