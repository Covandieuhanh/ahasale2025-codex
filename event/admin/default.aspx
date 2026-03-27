<%@ Page Title="Event Platform Admin" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="event_admin_default" %>

<asp:Content ID="ContentHeadTruoc" ContentPlaceHolderID="head_truoc" runat="Server">
</asp:Content>

<asp:Content ID="ContentHeadSau" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .event-admin-shell {
            padding: 22px 0 48px;
        }

        .event-admin-card {
            border-radius: 18px;
            padding: 18px;
            background: #fff;
            border: 1px solid rgba(15, 23, 42, .08);
            box-shadow: 0 16px 38px rgba(15, 23, 42, .08);
            margin-bottom: 14px;
        }

        .event-admin-stats {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 10px;
            margin-top: 12px;
        }

        .event-admin-stat {
            border: 1px solid rgba(15, 23, 42, .08);
            border-radius: 12px;
            padding: 10px 12px;
            background: #fff;
        }

        .event-admin-stat .label {
            color: #64748b;
            font-size: 12px;
        }

        .event-admin-stat .value {
            margin-top: 4px;
            font-size: 19px;
            font-weight: 700;
        }

        .event-form-grid {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 10px;
        }

        .event-form-grid .full {
            grid-column: 1 / -1;
        }

        .event-admin-table {
            width: 100%;
            border-collapse: collapse;
        }

        .event-admin-table th,
        .event-admin-table td {
            border-bottom: 1px solid rgba(15, 23, 42, .08);
            padding: 9px 8px;
            vertical-align: top;
            font-size: 13px;
        }

        .event-admin-table th {
            color: #475569;
            font-weight: 600;
            white-space: nowrap;
        }

        .event-actions {
            display: flex;
            gap: 6px;
            flex-wrap: wrap;
        }

        .event-actions .btn {
            padding: 2px 10px;
            font-size: 12px;
        }

        .event-empty {
            color: #64748b;
            font-size: 14px;
        }

        @media (max-width: 980px) {
            .event-admin-stats {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }

            .event-form-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="Server">
    <div class="container-xl event-admin-shell">
        <div class="event-admin-card">
            <div class="text-muted small">AhaSale / Event Platform</div>
            <h1 class="h4 mb-1">/event/admin - Builder chiến dịch</h1>
            <div class="text-muted">Tạo chiến dịch thực tế theo mô hình data aha: tích điểm voucher hoặc trả lương/thưởng theo bậc 5% -> 50%.</div>
            <asp:PlaceHolder ID="phReadOnlyHint" runat="server" Visible="false">
                <div class="alert alert-warning mt-3 mb-0">
                    <asp:Literal ID="litReadOnlyHint" runat="server" />
                </div>
            </asp:PlaceHolder>
            <div class="event-admin-stats">
                <div class="event-admin-stat">
                    <div class="label">Nháp</div>
                    <div class="value"><%= Summary.DraftCount.ToString("#,##0") %></div>
                </div>
                <div class="event-admin-stat">
                    <div class="label">Chờ duyệt</div>
                    <div class="value"><%= Summary.PendingApprovalCount.ToString("#,##0") %></div>
                </div>
                <div class="event-admin-stat">
                    <div class="label">Đang chạy</div>
                    <div class="value"><%= Summary.ActiveCount.ToString("#,##0") %></div>
                </div>
                <div class="event-admin-stat">
                    <div class="label">Tạm dừng</div>
                    <div class="value"><%= Summary.PausedCount.ToString("#,##0") %></div>
                </div>
            </div>
            <div class="event-admin-stats">
                <div class="event-admin-stat">
                    <div class="label">Voucher tier</div>
                    <div class="value"><%= Summary.VoucherTierCount.ToString("#,##0") %></div>
                </div>
                <div class="event-admin-stat">
                    <div class="label">Salary/bonus tier</div>
                    <div class="value"><%= Summary.SalaryBonusTierCount.ToString("#,##0") %></div>
                </div>
                <div class="event-admin-stat">
                    <div class="label">Kết thúc</div>
                    <div class="value"><%= Summary.EndedCount.ToString("#,##0") %></div>
                </div>
                <div class="event-admin-stat">
                    <div class="label">Lưu trữ</div>
                    <div class="value"><%= Summary.ArchivedCount.ToString("#,##0") %></div>
                </div>
            </div>
        </div>

        <div class="event-admin-card">
            <div class="d-flex justify-content-between align-items-center mb-2">
                <h2 class="h6 mb-0">Tạo chiến dịch bậc thang</h2>
            </div>
            <div class="text-muted mb-2">Chiến dịch được tạo ở trạng thái <b>Nháp</b>. Bạn có thể chuyển trạng thái ngay ở danh sách phía dưới.</div>
            <div class="event-form-grid">
                <div>
                    <label class="form-label">Tên chiến dịch</label>
                    <asp:TextBox ID="txtCampaignName" runat="server" CssClass="form-control form-control-sm" MaxLength="300" />
                </div>
                <div>
                    <label class="form-label">Loại chiến dịch</label>
                    <asp:DropDownList ID="ddlCampaignType" runat="server" CssClass="form-select form-select-sm" />
                </div>
                <div>
                    <label class="form-label">Phạm vi shop</label>
                    <asp:DropDownList ID="ddlShopScope" runat="server" CssClass="form-select form-select-sm">
                        <asp:ListItem Text="Tất cả shop" Value="all" />
                        <asp:ListItem Text="Danh sách shop chỉ định" Value="selected" />
                    </asp:DropDownList>
                </div>
                <div class="full">
                    <label class="form-label">Danh sách shop mục tiêu (khi chọn "Danh sách shop chỉ định")</label>
                    <asp:TextBox ID="txtSelectedShopTargets" runat="server" CssClass="form-control form-control-sm" TextMode="MultiLine" Rows="3" placeholder="Nhập tài khoản shop, mỗi dòng 1 tài khoản hoặc phân tách bằng dấu phẩy." />
                    <div class="text-muted small mt-1">Ví dụ: <code>shop_cong_ty</code>, <code>shop_demo_01</code>, <code>shop_miennam</code>.</div>
                </div>
                <div class="full">
                    <label class="form-label">Mô tả</label>
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control form-control-sm" TextMode="MultiLine" Rows="2" />
                </div>
                <div>
                    <label class="form-label">Bậc tăng mỗi lần (%)</label>
                    <asp:TextBox ID="txtStepPercent" runat="server" CssClass="form-control form-control-sm" TextMode="Number" />
                </div>
                <div>
                    <label class="form-label">Trần tối đa (%)</label>
                    <asp:TextBox ID="txtMaxPercent" runat="server" CssClass="form-control form-control-sm" TextMode="Number" />
                </div>
                <div>
                    <label class="form-label">Số lần áp dụng trần</label>
                    <asp:TextBox ID="txtCapOccurrence" runat="server" CssClass="form-control form-control-sm" TextMode="Number" />
                </div>
                <div>
                    <label class="form-label">Bắt đầu (tuỳ chọn)</label>
                    <asp:TextBox ID="txtStartAt" runat="server" CssClass="form-control form-control-sm" TextMode="DateTimeLocal" />
                </div>
                <div>
                    <label class="form-label">Kết thúc (tuỳ chọn)</label>
                    <asp:TextBox ID="txtEndAt" runat="server" CssClass="form-control form-control-sm" TextMode="DateTimeLocal" />
                </div>
                <div class="d-flex align-items-end">
                    <asp:LinkButton ID="butCreate" runat="server" CssClass="btn btn-primary btn-sm" OnClick="butCreate_Click" OnClientClick="return AhaPreventDoubleClick(this);">Tạo chiến dịch</asp:LinkButton>
                </div>
            </div>
        </div>

        <div class="event-admin-card">
            <div class="d-flex justify-content-between align-items-center mb-2">
                <h2 class="h6 mb-0">Cấu hình phạm vi shop theo chiến dịch</h2>
            </div>
            <div class="text-muted mb-2">Dùng khi cần cập nhật lại danh sách shop cho chiến dịch đã tạo. Chỉ chỉnh được ở trạng thái Nháp, Chờ duyệt hoặc Tạm dừng.</div>
            <div class="event-form-grid">
                <div>
                    <label class="form-label">Chọn chiến dịch</label>
                    <asp:DropDownList ID="ddlTargetProgram" runat="server" CssClass="form-select form-select-sm" />
                </div>
                <div class="d-flex align-items-end">
                    <asp:LinkButton ID="butLoadTargets" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="butLoadTargets_Click">Nạp dữ liệu</asp:LinkButton>
                </div>
                <div>
                    <label class="form-label">Phạm vi</label>
                    <asp:DropDownList ID="ddlTargetScope" runat="server" CssClass="form-select form-select-sm">
                        <asp:ListItem Text="Tất cả shop" Value="all" />
                        <asp:ListItem Text="Danh sách shop chỉ định" Value="selected" />
                    </asp:DropDownList>
                </div>
                <div class="full">
                    <label class="form-label">Danh sách shop mục tiêu</label>
                    <asp:TextBox ID="txtTargetShops" runat="server" CssClass="form-control form-control-sm" TextMode="MultiLine" Rows="4" placeholder="Mỗi dòng 1 tài khoản shop hoặc phân tách bằng dấu phẩy." />
                </div>
                <div class="d-flex align-items-end">
                    <asp:LinkButton ID="butSaveTargets" runat="server" CssClass="btn btn-primary btn-sm" OnClick="butSaveTargets_Click" OnClientClick="return AhaPreventDoubleClick(this);">Lưu phạm vi shop</asp:LinkButton>
                </div>
            </div>
            <asp:PlaceHolder ID="phTargetEditorHint" runat="server" Visible="false">
                <div class="alert alert-info mt-2 mb-0">
                    <asp:Literal ID="litTargetEditorHint" runat="server" />
                </div>
            </asp:PlaceHolder>
        </div>

        <div class="event-admin-card">
            <div class="d-flex justify-content-between align-items-center mb-2">
                <h2 class="h6 mb-0">Mô phỏng công thức thưởng</h2>
            </div>
            <div class="event-form-grid">
                <div>
                    <label class="form-label">Chọn chiến dịch</label>
                    <asp:DropDownList ID="ddlSimulationProgram" runat="server" CssClass="form-select form-select-sm" />
                </div>
                <div>
                    <label class="form-label">Số lần phát sinh trong tháng</label>
                    <asp:TextBox ID="txtSimulationOccurrence" runat="server" CssClass="form-control form-control-sm" TextMode="Number" />
                </div>
                <div>
                    <label class="form-label">Doanh thu</label>
                    <asp:TextBox ID="txtSimulationRevenue" runat="server" CssClass="form-control form-control-sm" />
                </div>
                <div class="d-flex align-items-end">
                    <asp:LinkButton ID="butSimulate" runat="server" CssClass="btn btn-outline-primary btn-sm" OnClick="butSimulate_Click">Mô phỏng</asp:LinkButton>
                </div>
            </div>
            <asp:PlaceHolder ID="phSimulationResult" runat="server" Visible="false">
                <div class="alert alert-info mt-2 mb-0">
                    <asp:Literal ID="litSimulationResult" runat="server" />
                </div>
            </asp:PlaceHolder>
        </div>

        <div class="event-admin-card">
            <div class="d-flex justify-content-between align-items-center mb-2">
                <h2 class="h6 mb-0">Version & Publish Definition</h2>
            </div>
            <div class="text-muted mb-2">Campaign definition được lưu theo version độc lập. Publish sẽ đẩy snapshot ra projection công khai cho các không gian hiển thị sau này.</div>
            <div class="event-form-grid">
                <div>
                    <label class="form-label">Chọn chiến dịch</label>
                    <asp:DropDownList ID="ddlDefinitionProgram" runat="server" CssClass="form-select form-select-sm" />
                </div>
                <div class="d-flex align-items-end">
                    <asp:LinkButton ID="butLoadDefinition" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="butLoadDefinition_Click">Nạp định nghĩa</asp:LinkButton>
                </div>
                <div></div>
                <div class="full">
                    <label class="form-label">Definition JSON</label>
                    <asp:TextBox ID="txtDefinitionJson" runat="server" CssClass="form-control form-control-sm" TextMode="MultiLine" Rows="8" />
                </div>
                <div class="full">
                    <label class="form-label">Ghi chú thay đổi version</label>
                    <asp:TextBox ID="txtDefinitionNote" runat="server" CssClass="form-control form-control-sm" MaxLength="300" />
                </div>
                <div class="d-flex align-items-end">
                    <asp:LinkButton ID="butSaveDefinition" runat="server" CssClass="btn btn-primary btn-sm" OnClick="butSaveDefinition_Click" OnClientClick="return AhaPreventDoubleClick(this);">Lưu definition (tăng version)</asp:LinkButton>
                </div>
            </div>
            <asp:PlaceHolder ID="phDefinitionHint" runat="server" Visible="false">
                <div class="alert alert-info mt-2 mb-0">
                    <asp:Literal ID="litDefinitionHint" runat="server" />
                </div>
            </asp:PlaceHolder>
        </div>

        <div class="event-admin-card">
            <div class="d-flex justify-content-between align-items-center mb-2">
                <h2 class="h6 mb-0">Danh sách chiến dịch</h2>
                <div class="d-flex gap-2">
                    <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control form-control-sm" placeholder="Tìm theo ID, code, tên, người tạo..." />
                    <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-select form-select-sm" />
                    <asp:DropDownList ID="ddlTypeFilter" runat="server" CssClass="form-select form-select-sm" />
                    <asp:LinkButton ID="butSearch" runat="server" CssClass="btn btn-sm btn-outline-primary" OnClick="butSearch_Click">Lọc</asp:LinkButton>
                </div>
            </div>

            <asp:PlaceHolder ID="phList" runat="server">
                <div class="table-responsive">
                    <table class="event-admin-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Chiến dịch</th>
                                <th>Loại</th>
                                <th>Trạng thái</th>
                                <th>Version</th>
                                <th>Publish</th>
                                <th>Phạm vi shop</th>
                                <th>Công thức</th>
                                <th>Thời gian</th>
                                <th>Người tạo</th>
                                <th>Thao tác</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptPrograms" runat="server" OnItemCommand="rptPrograms_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("ID") %></td>
                                        <td>
                                            <div class="fw-semibold"><%# Eval("CampaignName") %></div>
                                            <div class="text-muted small"><%# Eval("CampaignCode") %></div>
                                        </td>
                                        <td><%# BuildTypeLabel(Eval("CampaignType")) %></td>
                                        <td><%# BuildStatusLabel(Eval("Status")) %></td>
                                        <td><%# BuildVersionLabel(Eval("VersionNo"), Eval("PublishedVersionNo")) %></td>
                                        <td><%# BuildPublishLabel(Eval("IsPublished"), Eval("PublishedVersionNo")) %></td>
                                        <td><%# BuildScopeLabel(Eval("ShopScope"), Eval("TargetShopCount")) %></td>
                                        <td><%# BuildTierLabel(Eval("TierStepPercent"), Eval("TierMaxPercent"), Eval("TierCapOccurrence")) %></td>
                                        <td><%# BuildRangeLabel(Eval("StartAt"), Eval("EndAt")) %></td>
                                        <td><%# Eval("CreatedBy") %></td>
                                        <td>
                                            <div class="event-actions">
                                                <asp:LinkButton runat="server" CssClass="btn btn-outline-secondary btn-sm" CommandName="submit_approval" CommandArgument='<%# Eval("ID") %>' Visible='<%# CanSubmitApproval(Eval("Status")) %>'>Chờ duyệt</asp:LinkButton>
                                                <asp:LinkButton runat="server" CssClass="btn btn-outline-secondary btn-sm" CommandName="back_to_draft" CommandArgument='<%# Eval("ID") %>' Visible='<%# CanBackToDraft(Eval("Status")) %>'>Trả nháp</asp:LinkButton>
                                                <asp:LinkButton runat="server" CssClass="btn btn-outline-success btn-sm" CommandName="activate" CommandArgument='<%# Eval("ID") %>' Visible='<%# CanActivate(Eval("Status")) %>'>Kích hoạt</asp:LinkButton>
                                                <asp:LinkButton runat="server" CssClass="btn btn-outline-warning btn-sm" CommandName="pause" CommandArgument='<%# Eval("ID") %>' Visible='<%# CanPause(Eval("Status")) %>'>Tạm dừng</asp:LinkButton>
                                                <asp:LinkButton runat="server" CssClass="btn btn-outline-primary btn-sm" CommandName="end" CommandArgument='<%# Eval("ID") %>' Visible='<%# CanEnd(Eval("Status")) %>'>Kết thúc</asp:LinkButton>
                                                <asp:LinkButton runat="server" CssClass="btn btn-outline-success btn-sm" CommandName="publish" CommandArgument='<%# Eval("ID") %>' Visible='<%# CanPublish(Eval("Status"), Eval("IsPublished")) %>'>Publish</asp:LinkButton>
                                                <asp:LinkButton runat="server" CssClass="btn btn-outline-danger btn-sm" CommandName="unpublish" CommandArgument='<%# Eval("ID") %>' Visible='<%# CanUnpublish(Eval("IsPublished")) %>'>Unpublish</asp:LinkButton>
                                                <asp:LinkButton runat="server" CssClass="btn btn-outline-dark btn-sm" CommandName="archive" CommandArgument='<%# Eval("ID") %>' Visible='<%# CanArchive(Eval("Status")) %>'>Lưu trữ</asp:LinkButton>
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
                <div class="event-empty">Không có chiến dịch phù hợp bộ lọc.</div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
