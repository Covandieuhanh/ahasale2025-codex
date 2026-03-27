using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class event_admin_default : Page
{
    protected EventService_cl.ProgramSummaryInfo Summary = new EventService_cl.ProgramSummaryInfo();
    private string CurrentEventActor = "";
    private bool CurrentActorIsHome = false;
    private bool CurrentCanOperate = false;
    private bool CurrentCanPublish = false;
    private string CurrentAdminAccount = "";
    private bool IsSystemAdminPortal = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        IsSystemAdminPortal = IsSystemAdminRequest();
        Session["url_back"] = Request != null
            ? (Request.RawUrl ?? "/event/admin")
            : "/event/admin";

        using (dbDataContext db = new dbDataContext())
        {
            EventBootstrap_cl.EnsureSchemaSafe(db);
            EnsureEventGate(db);
            if (Response != null && Response.IsRequestBeingRedirected)
                return;

            if (IsSystemAdminPortal && !AdminManagementSpace_cl.EnsureSelectedSpaceRouteAccess(this))
                return;

            ApplyUiPermissions();

            if (!IsPostBack)
            {
                BindCreateType();
                BindFilters();
                SetCreateDefaults();
                BindAdminState(db);
            }
        }
    }

    private void EnsureEventGate(dbDataContext db)
    {
        if (IsSystemAdminPortal)
        {
            EnsureSystemAdminGate(db);
            return;
        }

        string homeUser = NormalizeAccount(ReadAccountFromSessionOrCookie("taikhoan_home", "cookie_userinfo_home_bcorn"));
        if (homeUser == "")
        {
            HomeSpaceAccess_cl.RedirectToHomeLogin(
                this,
                Request.RawUrl ?? "/event/admin",
                "Vui lòng đăng nhập tài khoản Home để truy cập Event Builder.");
            return;
        }

        taikhoan_tb homeAccount = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == homeUser);
        if (homeAccount == null || !SpaceAccess_cl.CanAccessEvent(db, homeAccount))
        {
            HomeSpaceAccess_cl.RedirectToAccessPage(this, ModuleSpace_cl.Event, Request.RawUrl ?? "/event/admin");
            return;
        }

        CurrentEventActor = homeUser;
        CurrentActorIsHome = true;
        CurrentCanOperate = EventPolicy_cl.CanOperateProgram(db, homeUser);
        CurrentCanPublish = EventPolicy_cl.CanPublishProgram(db, homeUser);
    }

    private bool IsSystemAdminRequest()
    {
        string raw = (Request.QueryString["system"] ?? "").Trim().ToLowerInvariant();
        return raw == "1" || raw == "true";
    }

    private void EnsureSystemAdminGate(dbDataContext db)
    {
        check_login_cl.check_login_admin("none", "none");
        string adminUser = AdminRolePolicy_cl.GetCurrentAdminUser();
        if (adminUser == "")
            return;

        taikhoan_tb adminAccount = db.taikhoan_tbs.FirstOrDefault(p => p.taikhoan == adminUser);
        if (!EventPolicy_cl.CanViewAdminWorkspace(db, adminUser))
        {
            Session["thongbao"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                "Bạn không có quyền truy cập quản trị sự kiện.",
                "1800",
                "warning");
            Response.Redirect("/admin/default.aspx", true);
            return;
        }

        CurrentAdminAccount = adminUser;
        CurrentEventActor = adminUser;
        CurrentActorIsHome = false;
        CurrentCanOperate = EventPolicy_cl.CanOperateProgram(db, adminUser);
        CurrentCanPublish = EventPolicy_cl.CanPublishProgram(db, adminUser);
    }

    private void ApplyUiPermissions()
    {
        bool canOperate = CurrentCanOperate;
        bool canPublish = CurrentCanPublish;

        butCreate.Visible = canOperate;
        butSaveTargets.Visible = canOperate;
        butSaveDefinition.Visible = canOperate;

        phReadOnlyHint.Visible = !canOperate;
        if (!canOperate)
        {
            if (canPublish)
                litReadOnlyHint.Text = "Tài khoản hiện tại chỉ có quyền publish/unpublish. Không thể tạo hoặc chỉnh sửa campaign definition/target.";
            else
                litReadOnlyHint.Text = "Tài khoản hiện tại đang ở chế độ chỉ xem. Cần quyền Event Operator/Designer/Owner hoặc event_admin_home để chỉnh sửa.";
        }
        else
        {
            litReadOnlyHint.Text = "";
        }
    }

    private string ReadAccountFromSessionOrCookie(string sessionKey, string cookieName)
    {
        string tkEnc = "";
        if (Session != null)
            tkEnc = Session[sessionKey] as string;
        if (string.IsNullOrEmpty(tkEnc) && Request != null && Request.Cookies != null)
        {
            HttpCookie ck = Request.Cookies[cookieName];
            if (ck != null)
                tkEnc = ck["taikhoan"] ?? "";
        }

        if (string.IsNullOrEmpty(tkEnc))
            return "";

        try
        {
            return tkEnc == "" ? "" : (mahoa_cl.giaima_Bcorn(tkEnc) ?? "");
        }
        catch
        {
            return "";
        }
    }

    private string NormalizeAccount(string raw)
    {
        return (raw ?? "").Trim().ToLowerInvariant();
    }

    private string GetCurrentActor()
    {
        if (!string.IsNullOrWhiteSpace(CurrentEventActor))
            return CurrentEventActor;

        if (IsSystemAdminPortal)
            return (CurrentAdminAccount ?? "").Trim().ToLowerInvariant();

        return NormalizeAccount(ReadAccountFromSessionOrCookie("taikhoan_home", "cookie_userinfo_home_bcorn"));
    }

    private void ShowUnauthorizedByScope()
    {
        if (IsSystemAdminPortal)
        {
            Session["thongbao"] = thongbao_class.metro_notifi_onload(
                "Thông báo",
                "Bạn không có quyền thao tác trong không gian sự kiện hiện tại.",
                "1800",
                "warning");
            Response.Redirect("/admin/default.aspx?mspace=event", true);
            return;
        }

        HomeSpaceAccess_cl.RedirectToAccessPage(this, ModuleSpace_cl.Event, Request.RawUrl ?? "/event/admin");
    }

    private void BindCreateType()
    {
        ddlCampaignType.Items.Clear();
        ddlCampaignType.Items.Add(new ListItem("Tích điểm voucher bậc thang", EventPolicy_cl.CampaignTypeVoucherTier));
        ddlCampaignType.Items.Add(new ListItem("Trả lương / thưởng bậc thang", EventPolicy_cl.CampaignTypeSalaryBonusTier));
    }

    private void BindFilters()
    {
        ddlStatusFilter.Items.Clear();
        ddlStatusFilter.Items.Add(new ListItem("Tất cả trạng thái", ""));
        ddlStatusFilter.Items.Add(new ListItem("Nháp", EventPolicy_cl.StatusDraft));
        ddlStatusFilter.Items.Add(new ListItem("Chờ duyệt", EventPolicy_cl.StatusPendingApproval));
        ddlStatusFilter.Items.Add(new ListItem("Đang chạy", EventPolicy_cl.StatusActive));
        ddlStatusFilter.Items.Add(new ListItem("Tạm dừng", EventPolicy_cl.StatusPaused));
        ddlStatusFilter.Items.Add(new ListItem("Kết thúc", EventPolicy_cl.StatusEnded));
        ddlStatusFilter.Items.Add(new ListItem("Lưu trữ", EventPolicy_cl.StatusArchived));

        ddlTypeFilter.Items.Clear();
        ddlTypeFilter.Items.Add(new ListItem("Tất cả loại", ""));
        ddlTypeFilter.Items.Add(new ListItem("Voucher tier", EventPolicy_cl.CampaignTypeVoucherTier));
        ddlTypeFilter.Items.Add(new ListItem("Salary/bonus tier", EventPolicy_cl.CampaignTypeSalaryBonusTier));
    }

    private void SetCreateDefaults()
    {
        txtStepPercent.Text = "5";
        txtMaxPercent.Text = "50";
        txtCapOccurrence.Text = "10";
        txtSelectedShopTargets.Text = "";
        txtSimulationOccurrence.Text = "1";
        txtSimulationRevenue.Text = "1000000";
        txtDefinitionNote.Text = "";
    }

    private void BindAdminState(dbDataContext db)
    {
        Summary = EventService_cl.GetSummary(db);

        string status = (ddlStatusFilter.SelectedValue ?? "").Trim();
        string type = (ddlTypeFilter.SelectedValue ?? "").Trim();
        string keyword = (txtKeyword.Text ?? "").Trim();
        List<EventService_cl.ProgramCardItem> list = EventService_cl.LoadAdminPrograms(db, status, type, keyword, 500);
        List<EventService_cl.ProgramCardItem> allPrograms = EventService_cl.LoadAdminPrograms(db, "", "", "", 500);

        rptPrograms.DataSource = list;
        rptPrograms.DataBind();
        phEmpty.Visible = list.Count == 0;

        BindSimulationPrograms(allPrograms);
        BindTargetPrograms(allPrograms);
        BindDefinitionPrograms(allPrograms);
    }

    private void BindSimulationPrograms(List<EventService_cl.ProgramCardItem> source)
    {
        string selected = (ddlSimulationProgram.SelectedValue ?? "").Trim();

        ddlSimulationProgram.Items.Clear();
        ddlSimulationProgram.Items.Add(new ListItem("-- Chọn chiến dịch --", ""));

        IEnumerable<EventService_cl.ProgramCardItem> rows = source ?? new List<EventService_cl.ProgramCardItem>();
        foreach (EventService_cl.ProgramCardItem item in rows.OrderByDescending(p => p.ID))
        {
            string label = "#" + item.ID.ToString() + " [" + BuildTypeShortLabel(item.CampaignType) + "] " + (item.CampaignName ?? "");
            ddlSimulationProgram.Items.Add(new ListItem(label, item.ID.ToString()));
        }

        if (selected != "" && ddlSimulationProgram.Items.FindByValue(selected) != null)
            ddlSimulationProgram.SelectedValue = selected;
    }

    private void BindTargetPrograms(List<EventService_cl.ProgramCardItem> source)
    {
        string selected = (ddlTargetProgram.SelectedValue ?? "").Trim();

        ddlTargetProgram.Items.Clear();
        ddlTargetProgram.Items.Add(new ListItem("-- Chọn chiến dịch --", ""));

        IEnumerable<EventService_cl.ProgramCardItem> rows = source ?? new List<EventService_cl.ProgramCardItem>();
        foreach (EventService_cl.ProgramCardItem item in rows.OrderByDescending(p => p.ID))
        {
            string label = "#" + item.ID.ToString() + " [" + BuildTypeShortLabel(item.CampaignType) + "] " + (item.CampaignName ?? "");
            ddlTargetProgram.Items.Add(new ListItem(label, item.ID.ToString()));
        }

        if (selected != "" && ddlTargetProgram.Items.FindByValue(selected) != null)
            ddlTargetProgram.SelectedValue = selected;
    }

    private void BindDefinitionPrograms(List<EventService_cl.ProgramCardItem> source)
    {
        string selected = (ddlDefinitionProgram.SelectedValue ?? "").Trim();

        ddlDefinitionProgram.Items.Clear();
        ddlDefinitionProgram.Items.Add(new ListItem("-- Chọn chiến dịch --", ""));

        IEnumerable<EventService_cl.ProgramCardItem> rows = source ?? new List<EventService_cl.ProgramCardItem>();
        foreach (EventService_cl.ProgramCardItem item in rows.OrderByDescending(p => p.ID))
        {
            string label = "#" + item.ID.ToString()
                + " [v" + item.VersionNo.ToString() + "] "
                + (item.CampaignName ?? "");
            ddlDefinitionProgram.Items.Add(new ListItem(label, item.ID.ToString()));
        }

        if (selected != "" && ddlDefinitionProgram.Items.FindByValue(selected) != null)
            ddlDefinitionProgram.SelectedValue = selected;
    }

    protected void butCreate_Click(object sender, EventArgs e)
    {
        if (!CurrentCanOperate)
        {
            ShowAdminNotice("Bạn không đủ quyền tạo campaign trong Event Platform.", "warning");
            return;
        }

        double stepPercent;
        if (!TryParseDouble(txtStepPercent.Text, out stepPercent))
        {
            ShowAdminNotice("Bậc tăng mỗi lần (%) không hợp lệ.", "warning");
            return;
        }

        double maxPercent;
        if (!TryParseDouble(txtMaxPercent.Text, out maxPercent))
        {
            ShowAdminNotice("Trần tối đa (%) không hợp lệ.", "warning");
            return;
        }

        int capOccurrence;
        if (!int.TryParse((txtCapOccurrence.Text ?? "").Trim(), out capOccurrence))
        {
            ShowAdminNotice("Số lần áp dụng trần không hợp lệ.", "warning");
            return;
        }

        DateTime? startAt;
        string dateError;
        if (!TryParseDateTimeNullable(txtStartAt.Text, out startAt, out dateError))
        {
            ShowAdminNotice(dateError, "warning");
            return;
        }

        DateTime? endAt;
        if (!TryParseDateTimeNullable(txtEndAt.Text, out endAt, out dateError))
        {
            ShowAdminNotice(dateError, "warning");
            return;
        }

        EventService_cl.CreateProgramRequest request = new EventService_cl.CreateProgramRequest
        {
            CampaignName = (txtCampaignName.Text ?? "").Trim(),
            CampaignType = (ddlCampaignType.SelectedValue ?? "").Trim(),
            Description = (txtDescription.Text ?? "").Trim(),
            ShopScope = (ddlShopScope.SelectedValue ?? "").Trim(),
            StartAt = startAt,
            EndAt = endAt,
            TierStepPercent = stepPercent,
            TierMaxPercent = maxPercent,
            TierCapOccurrence = capOccurrence,
            ActorAccount = GetCurrentActor(),
            ShopTargets = EventService_cl.ParseShopTargetsRaw(txtSelectedShopTargets.Text)
        };

        if (request.ActorAccount == "")
        {
            ShowUnauthorizedByScope();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            EventBootstrap_cl.EnsureSchemaSafe(db);
            long programId;
            string message;
            bool ok = EventService_cl.TryCreateTierProgram(db, request, out programId, out message);
            ShowAdminNotice(message, ok ? "success" : "warning");
        }
    }

    protected void butSearch_Click(object sender, EventArgs e)
    {
        using (dbDataContext db = new dbDataContext())
        {
            EventBootstrap_cl.EnsureSchemaSafe(db);
            BindAdminState(db);
        }
    }

    protected void butSimulate_Click(object sender, EventArgs e)
    {
        phSimulationResult.Visible = true;
        litSimulationResult.Text = "";

        long programId;
        if (!long.TryParse((ddlSimulationProgram.SelectedValue ?? "").Trim(), out programId) || programId <= 0)
        {
            litSimulationResult.Text = "Vui lòng chọn chiến dịch để mô phỏng.";
            return;
        }

        int occurrence;
        if (!int.TryParse((txtSimulationOccurrence.Text ?? "").Trim(), out occurrence) || occurrence <= 0)
        {
            litSimulationResult.Text = "Số lần phát sinh phải lớn hơn 0.";
            return;
        }

        double revenue;
        if (!TryParseDouble(txtSimulationRevenue.Text, out revenue))
        {
            litSimulationResult.Text = "Doanh thu mô phỏng không hợp lệ.";
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            EventBootstrap_cl.EnsureSchemaSafe(db);
            BindAdminState(db);

            EventService_cl.RewardSimulationResult result;
            string message;
            bool ok = EventService_cl.TrySimulateReward(db, programId, occurrence, revenue, out result, out message);
            if (!ok || result == null)
            {
                litSimulationResult.Text = HttpUtility.HtmlEncode(message);
                return;
            }

            string typeLabel = BuildTypeLabel(result.CampaignType);
            string rewardLabel = (result.CampaignType == EventPolicy_cl.CampaignTypeSalaryBonusTier)
                ? "Tiền thưởng/lương"
                : "Voucher tích điểm";

            litSimulationResult.Text = string.Format(
                "Chiến dịch <b>{0}</b> ({1}) -> Lần nhập {2}, lần hiệu lực {3}, tỷ lệ <b>{4}%</b>, doanh thu <b>{5}</b>, {6} nhận được <b>{7}</b>.",
                HttpUtility.HtmlEncode(result.CampaignCode ?? ""),
                HttpUtility.HtmlEncode(typeLabel),
                result.InputOccurrence.ToString("#,##0"),
                result.EffectiveOccurrence.ToString("#,##0"),
                result.RatePercent.ToString("#,##0.####"),
                FormatMoney(result.Revenue),
                HttpUtility.HtmlEncode(rewardLabel),
                FormatMoney(result.RewardValue));
        }
    }

    protected void butLoadTargets_Click(object sender, EventArgs e)
    {
        phTargetEditorHint.Visible = false;
        litTargetEditorHint.Text = "";

        long programId;
        if (!long.TryParse((ddlTargetProgram.SelectedValue ?? "").Trim(), out programId) || programId <= 0)
        {
            phTargetEditorHint.Visible = true;
            litTargetEditorHint.Text = "Vui lòng chọn chiến dịch cần cấu hình phạm vi shop.";
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            EventBootstrap_cl.EnsureSchemaSafe(db);
            EventService_cl.ProgramCardItem program = EventService_cl.GetProgramById(db, programId);
            if (program == null)
            {
                phTargetEditorHint.Visible = true;
                litTargetEditorHint.Text = "Không tìm thấy chiến dịch.";
                return;
            }

            string scope = (program.ShopScope ?? "").Trim().ToLowerInvariant();
            if (scope != "selected")
                scope = "all";

            if (ddlTargetScope.Items.FindByValue(scope) != null)
                ddlTargetScope.SelectedValue = scope;

            List<string> targets = EventService_cl.LoadProgramTargetAccounts(db, programId, 500);
            txtTargetShops.Text = string.Join("\r\n", targets);

            phTargetEditorHint.Visible = true;
            litTargetEditorHint.Text = "Đã nạp chiến dịch #" + program.ID.ToString()
                + " | Trạng thái: " + BuildStatusLabel(program.Status)
                + " | Scope: " + (scope == "selected" ? "Danh sách chỉ định" : "Tất cả shop")
                + " | Shop mục tiêu: " + program.TargetShopCount.ToString("#,##0");
        }
    }

    protected void butSaveTargets_Click(object sender, EventArgs e)
    {
        if (!CurrentCanOperate)
        {
            ShowAdminNotice("Bạn không đủ quyền cập nhật phạm vi shop cho campaign.", "warning");
            return;
        }

        long programId;
        if (!long.TryParse((ddlTargetProgram.SelectedValue ?? "").Trim(), out programId) || programId <= 0)
        {
            ShowAdminNotice("Vui lòng chọn chiến dịch cần lưu phạm vi shop.", "warning");
            return;
        }

        string scope = (ddlTargetScope.SelectedValue ?? "").Trim().ToLowerInvariant();
        if (scope != "selected")
            scope = "all";
        List<string> targets = EventService_cl.ParseShopTargetsRaw(txtTargetShops.Text);
        string actor = GetCurrentActor();
        if (actor == "")
        {
            ShowUnauthorizedByScope();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            EventBootstrap_cl.EnsureSchemaSafe(db);
            string message;
            bool ok = EventService_cl.TryUpdateProgramTargets(db, programId, scope, targets, actor, out message);
            ShowAdminNotice(message, ok ? "success" : "warning");
        }
    }

    protected void butLoadDefinition_Click(object sender, EventArgs e)
    {
        phDefinitionHint.Visible = false;
        litDefinitionHint.Text = "";

        long programId;
        if (!long.TryParse((ddlDefinitionProgram.SelectedValue ?? "").Trim(), out programId) || programId <= 0)
        {
            phDefinitionHint.Visible = true;
            litDefinitionHint.Text = "Vui lòng chọn chiến dịch cần nạp definition.";
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            EventBootstrap_cl.EnsureSchemaSafe(db);
            EventService_cl.ProgramCardItem program = EventService_cl.GetProgramById(db, programId);
            if (program == null)
            {
                phDefinitionHint.Visible = true;
                litDefinitionHint.Text = "Không tìm thấy chiến dịch.";
                return;
            }

            txtDefinitionJson.Text = program.DefinitionJson ?? "";
            txtDefinitionNote.Text = "";
            phDefinitionHint.Visible = true;
            litDefinitionHint.Text = "Đã nạp definition chiến dịch #" + program.ID.ToString()
                + " | Version hiện tại: " + program.VersionNo.ToString("#,##0")
                + " | Publish version: " + (program.PublishedVersionNo > 0 ? program.PublishedVersionNo.ToString("#,##0") : "-");
        }
    }

    protected void butSaveDefinition_Click(object sender, EventArgs e)
    {
        if (!CurrentCanOperate)
        {
            ShowAdminNotice("Bạn không đủ quyền cập nhật definition campaign.", "warning");
            return;
        }

        long programId;
        if (!long.TryParse((ddlDefinitionProgram.SelectedValue ?? "").Trim(), out programId) || programId <= 0)
        {
            ShowAdminNotice("Vui lòng chọn chiến dịch cần lưu definition.", "warning");
            return;
        }

        string actor = GetCurrentActor();
        if (actor == "")
        {
            ShowUnauthorizedByScope();
            return;
        }

        using (dbDataContext db = new dbDataContext())
        {
            EventBootstrap_cl.EnsureSchemaSafe(db);
            if (!EventPolicy_cl.CanOperateProgram(db, actor) && !EventPolicy_cl.CanAccessHomeBuilder(db, actor, ""))
            {
                ShowAdminNotice("Bạn không đủ quyền để cập nhật definition campaign.", "warning");
                return;
            }

            int versionNo;
            string message;
            bool ok = EventService_cl.TrySaveProgramDefinition(
                db,
                programId,
                txtDefinitionJson.Text,
                actor,
                txtDefinitionNote.Text,
                out versionNo,
                out message);

            if (ok)
                txtDefinitionNote.Text = "";

            ShowAdminNotice(message, ok ? "success" : "warning");
        }
    }

    protected void rptPrograms_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        long id;
        if (!long.TryParse((e.CommandArgument ?? "").ToString(), out id) || id <= 0)
            return;

        string command = (e.CommandName ?? "").Trim().ToLowerInvariant();
        string nextStatus = "";
        switch (command)
        {
            case "submit_approval":
                nextStatus = EventPolicy_cl.StatusPendingApproval;
                break;
            case "back_to_draft":
                nextStatus = EventPolicy_cl.StatusDraft;
                break;
            case "activate":
                nextStatus = EventPolicy_cl.StatusActive;
                break;
            case "pause":
                nextStatus = EventPolicy_cl.StatusPaused;
                break;
            case "end":
                nextStatus = EventPolicy_cl.StatusEnded;
                break;
            case "archive":
                nextStatus = EventPolicy_cl.StatusArchived;
                break;
            case "publish":
            case "unpublish":
                break;
            default:
                return;
        }

        string actor = GetCurrentActor();
        if (actor == "")
        {
            ShowUnauthorizedByScope();
            return;
        }
        using (dbDataContext db = new dbDataContext())
        {
            EventBootstrap_cl.EnsureSchemaSafe(db);
            string message;
            bool ok;
            if (command == "publish")
            {
                if (!CurrentCanPublish)
                {
                    ShowAdminNotice("Bạn không đủ quyền publish campaign.", "warning");
                    return;
                }

                if (!EventPolicy_cl.CanPublishProgram(db, actor))
                {
                    ShowAdminNotice("Bạn không đủ quyền publish campaign.", "warning");
                    return;
                }
                ok = EventService_cl.TryPublishProgram(db, id, actor, out message);
            }
            else if (command == "unpublish")
            {
                if (!CurrentCanPublish)
                {
                    ShowAdminNotice("Bạn không đủ quyền unpublish campaign.", "warning");
                    return;
                }

                if (!EventPolicy_cl.CanPublishProgram(db, actor))
                {
                    ShowAdminNotice("Bạn không đủ quyền unpublish campaign.", "warning");
                    return;
                }
                ok = EventService_cl.TryUnpublishProgram(db, id, actor, out message);
            }
            else
            {
                if (!CurrentCanOperate)
                {
                    ShowAdminNotice("Bạn không đủ quyền đổi trạng thái campaign.", "warning");
                    return;
                }

                if (!EventPolicy_cl.CanOperateProgram(db, actor) && !EventPolicy_cl.CanAccessHomeBuilder(db, actor, ""))
                {
                    ShowAdminNotice("Bạn không đủ quyền đổi trạng thái campaign.", "warning");
                    return;
                }
                ok = EventService_cl.TryChangeProgramStatus(db, id, nextStatus, actor, out message);
            }
            ShowAdminNotice(message, ok ? "success" : "warning");
        }
    }

    private void ShowAdminNotice(string message, string cssClass)
    {
        string notice = thongbao_class.metro_notifi_onload(
            "Thông báo",
            string.IsNullOrWhiteSpace(message) ? "Đã xử lý thao tác." : message,
            "1900",
            string.IsNullOrWhiteSpace(cssClass) ? "info" : cssClass);
        Session["thongbao"] = notice;
        Session["thongbao_home"] = notice;
        Response.Redirect(Request.RawUrl, true);
    }

    private bool TryParseDateTimeNullable(string raw, out DateTime? value, out string message)
    {
        value = null;
        message = "";
        string text = (raw ?? "").Trim();
        if (text == "")
            return true;

        DateTime parsed;
        if (DateTime.TryParseExact(text, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
        {
            value = parsed;
            return true;
        }

        if (DateTime.TryParse(text, out parsed))
        {
            value = parsed;
            return true;
        }

        message = "Định dạng ngày giờ không hợp lệ.";
        return false;
    }

    private bool TryParseDouble(string raw, out double value)
    {
        value = 0;
        string text = (raw ?? "").Trim();
        if (text == "")
            return false;

        if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            return true;
        if (double.TryParse(text, NumberStyles.Any, new CultureInfo("vi-VN"), out value))
            return true;
        if (double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out value))
            return true;
        return false;
    }

    protected string BuildTypeLabel(object typeObj)
    {
        string type = EventPolicy_cl.NormalizeCampaignType(typeObj == null ? "" : typeObj.ToString());
        if (type == EventPolicy_cl.CampaignTypeSalaryBonusTier)
            return "Trả lương / thưởng bậc thang";
        return "Tích điểm voucher bậc thang";
    }

    private string BuildTypeShortLabel(string typeRaw)
    {
        string type = EventPolicy_cl.NormalizeCampaignType(typeRaw);
        if (type == EventPolicy_cl.CampaignTypeSalaryBonusTier)
            return "SALARY";
        return "VOUCHER";
    }

    protected string BuildStatusLabel(object statusObj)
    {
        string status = EventPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        switch (status)
        {
            case EventPolicy_cl.StatusDraft:
                return "Nháp";
            case EventPolicy_cl.StatusPendingApproval:
                return "Chờ duyệt";
            case EventPolicy_cl.StatusActive:
                return "Đang chạy";
            case EventPolicy_cl.StatusPaused:
                return "Tạm dừng";
            case EventPolicy_cl.StatusEnded:
                return "Kết thúc";
            case EventPolicy_cl.StatusArchived:
                return "Lưu trữ";
            default:
                return "Không xác định";
        }
    }

    protected string BuildTierLabel(object stepObj, object maxObj, object capObj)
    {
        double step = 5;
        double max = 50;
        int cap = 10;
        double.TryParse(stepObj == null ? "5" : stepObj.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out step);
        double.TryParse(maxObj == null ? "50" : maxObj.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out max);
        int.TryParse(capObj == null ? "10" : capObj.ToString(), out cap);
        if (cap <= 0) cap = 10;
        return step.ToString("#,##0.##") + "% -> " + max.ToString("#,##0.##") + "% (cap lần " + cap.ToString("#,##0") + ")";
    }

    protected string BuildScopeLabel(object scopeObj, object targetCountObj)
    {
        string scope = (scopeObj == null ? "" : scopeObj.ToString()).Trim().ToLowerInvariant();
        int targetCount;
        if (!int.TryParse(targetCountObj == null ? "0" : targetCountObj.ToString(), out targetCount))
            targetCount = 0;

        if (scope == "selected")
            return "Chỉ định (" + targetCount.ToString("#,##0") + " shop)";
        return "Tất cả shop";
    }

    protected string BuildVersionLabel(object versionObj, object publishedVersionObj)
    {
        int version = 1;
        int published = 0;
        int.TryParse(versionObj == null ? "1" : versionObj.ToString(), out version);
        int.TryParse(publishedVersionObj == null ? "0" : publishedVersionObj.ToString(), out published);
        if (version <= 0)
            version = 1;

        if (published > 0)
            return "v" + version.ToString("#,##0") + " / pub v" + published.ToString("#,##0");
        return "v" + version.ToString("#,##0");
    }

    protected string BuildPublishLabel(object isPublishedObj, object publishedVersionObj)
    {
        bool isPublished = false;
        int publishedVersion = 0;
        bool.TryParse(isPublishedObj == null ? "false" : isPublishedObj.ToString(), out isPublished);
        int.TryParse(publishedVersionObj == null ? "0" : publishedVersionObj.ToString(), out publishedVersion);

        if (isPublished || publishedVersion > 0)
            return "Đã publish";
        return "Chưa publish";
    }

    protected string BuildRangeLabel(object startObj, object endObj)
    {
        string start = FormatDate(startObj);
        string end = FormatDate(endObj);
        if (start == "-" && end == "-")
            return "Không giới hạn";
        return start + " -> " + end;
    }

    protected bool CanSubmitApproval(object statusObj)
    {
        string status = EventPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        return CurrentCanOperate && status == EventPolicy_cl.StatusDraft;
    }

    protected bool CanActivate(object statusObj)
    {
        string status = EventPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        return CurrentCanOperate
            && (status == EventPolicy_cl.StatusDraft
            || status == EventPolicy_cl.StatusPendingApproval
            || status == EventPolicy_cl.StatusPaused);
    }

    protected bool CanPublish(object statusObj, object isPublishedObj)
    {
        string status = EventPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        bool isPublished = false;
        bool.TryParse(isPublishedObj == null ? "false" : isPublishedObj.ToString(), out isPublished);
        return CurrentCanPublish && status == EventPolicy_cl.StatusActive && !isPublished;
    }

    protected bool CanUnpublish(object isPublishedObj)
    {
        bool isPublished = false;
        bool.TryParse(isPublishedObj == null ? "false" : isPublishedObj.ToString(), out isPublished);
        return CurrentCanPublish && isPublished;
    }

    protected bool CanBackToDraft(object statusObj)
    {
        string status = EventPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        return CurrentCanOperate && status == EventPolicy_cl.StatusPendingApproval;
    }

    protected bool CanPause(object statusObj)
    {
        string status = EventPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        return CurrentCanOperate && status == EventPolicy_cl.StatusActive;
    }

    protected bool CanEnd(object statusObj)
    {
        string status = EventPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        return CurrentCanOperate
            && (status == EventPolicy_cl.StatusActive
            || status == EventPolicy_cl.StatusPaused);
    }

    protected bool CanArchive(object statusObj)
    {
        string status = EventPolicy_cl.NormalizeStatus(statusObj == null ? "" : statusObj.ToString());
        return CurrentCanOperate
            && (status == EventPolicy_cl.StatusDraft
            || status == EventPolicy_cl.StatusPendingApproval
            || status == EventPolicy_cl.StatusPaused
            || status == EventPolicy_cl.StatusEnded);
    }

    protected string FormatDate(object dateObj)
    {
        if (dateObj == null)
            return "-";
        DateTime date;
        if (!DateTime.TryParse(dateObj.ToString(), out date))
            return "-";
        return date.ToString("dd/MM/yyyy HH:mm");
    }

    protected string FormatMoney(double value)
    {
        return value.ToString("#,##0.####");
    }
}
