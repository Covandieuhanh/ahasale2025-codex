<%@ Page Title="Chi tiết hồ sơ người" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="chi-tiet.aspx.cs" Inherits="gianhang_person_hub_detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .personhub-shell {
            display: grid;
            grid-template-columns: minmax(0, 1.1fr) minmax(320px, 420px);
            gap: 20px;
        }

        .personhub-panel {
            border: 1px solid #dce7f4;
            border-radius: 18px;
            background: #fff;
            box-shadow: 0 14px 30px rgba(15, 34, 61, 0.06);
            padding: 20px;
        }

        .personhub-panel__title {
            font-size: 18px;
            font-weight: 700;
            color: #16324a;
        }

        .personhub-panel__sub {
            margin-top: 6px;
            color: #657b8d;
            line-height: 1.6;
        }

        .personhub-note {
            border: 1px solid #d7e8d8;
            border-radius: 16px;
            background: linear-gradient(180deg, #f7fff7 0%, #ffffff 100%);
            padding: 16px 18px;
        }

        .personhub-summary-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 14px;
            margin-top: 16px;
        }

        .personhub-summary-card {
            border: 1px solid #e8eef6;
            border-radius: 14px;
            padding: 14px 16px;
            background: #fafcff;
        }

        .personhub-summary-card__label {
            font-size: 11px;
            font-weight: 700;
            letter-spacing: .08em;
            text-transform: uppercase;
            color: #6e8092;
        }

        .personhub-summary-card__value {
            margin-top: 8px;
            font-size: 20px;
            font-weight: 700;
            color: #18354b;
        }

        .personhub-source-list {
            margin-top: 16px;
        }

        .personhub-source-item {
            border: 1px solid #e8eef6;
            border-radius: 14px;
            padding: 14px 16px;
            background: #fff;
        }

        .personhub-source-item + .personhub-source-item {
            margin-top: 12px;
        }

        .personhub-source-item__meta {
            margin-top: 6px;
            color: #667d90;
        }

        .home-account-search-box {
            position: relative;
        }

        .home-account-search-result {
            position: absolute;
            top: calc(100% + 6px);
            left: 0;
            right: 0;
            z-index: 30;
            background: #fff;
            border: 1px solid #d7e3f4;
            border-radius: 14px;
            box-shadow: 0 18px 40px rgba(13, 37, 63, 0.12);
            overflow: hidden;
            display: none;
        }

        .home-account-search-item {
            width: 100%;
            display: block;
            text-align: left;
            padding: 12px 14px;
            border: 0;
            background: #fff;
            cursor: pointer;
        }

        .home-account-search-item + .home-account-search-item {
            border-top: 1px solid #eef3fb;
        }

        .home-account-search-item:hover {
            background: #f7fbff;
        }

        .home-account-search-meta {
            display: block;
            margin-top: 4px;
            color: #6b7b90;
            font-size: 12px;
        }

        .home-account-search-empty {
            padding: 12px 14px;
            color: #6b7b90;
            font-size: 13px;
        }

        @media (max-width: 1024px) {
            .personhub-shell {
                grid-template-columns: 1fr;
            }

            .personhub-summary-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id="main-content" class="mb-10">
        <div class="row border-bottom bd-light">
            <div class="cell-6">
                <ul class="h-menu">
                    <li data-role="hint" data-hint-position="top" data-hint-text="Quay lại danh sách hồ sơ người">
                        <a class="button" href="/gianhang/admin/quan-ly-con-nguoi/Default.aspx"><span class="mif mif-arrow-left"></span></a>
                    </li>
                    <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                </ul>
            </div>
        </div>

        <asp:PlaceHolder ID="ph_not_found" runat="server" Visible="false">
            <div class="personhub-panel mt-5">
                <div class="personhub-panel__title">Không tìm thấy hồ sơ người</div>
                <div class="personhub-panel__sub">Hồ sơ này chưa có số điện thoại hợp lệ hoặc chưa tồn tại trong không gian hiện tại.</div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="ph_detail" runat="server" Visible="false">
            <div class="personhub-shell mt-5">
                <div class="personhub-panel">
                    <div class="personhub-panel__title"><%=HttpUtility.HtmlEncode(DisplayName) %></div>
                    <div class="personhub-panel__sub">
                        Số điện thoại chuẩn để gom vai trò: <strong><%=HttpUtility.HtmlEncode(DisplayPhone) %></strong>
                    </div>

                    <div class="personhub-summary-grid">
                        <div class="personhub-summary-card">
                            <div class="personhub-summary-card__label">Trạng thái Home</div>
                            <div class="personhub-summary-card__value">
                                <span class="data-wrapper"><code class="<%=HttpUtility.HtmlAttributeEncode(LinkCss) %>"><%=HttpUtility.HtmlEncode(LinkStatusLabel) %></code></span>
                            </div>
                        </div>
                        <div class="personhub-summary-card">
                            <div class="personhub-summary-card__label">Vai trò đã nhận diện</div>
                            <div class="personhub-summary-card__value" style="font-size:16px; line-height:1.6;"><%=RoleSummaryHtml %></div>
                        </div>
                        <div class="personhub-summary-card">
                            <div class="personhub-summary-card__label">Nguồn vai trò</div>
                            <div class="personhub-summary-card__value"><%=HttpUtility.HtmlEncode(SourceCountText) %></div>
                        </div>
                        <div class="personhub-summary-card">
                            <div class="personhub-summary-card__label">Vai trò đã gỡ</div>
                            <div class="personhub-summary-card__value"><%=HttpUtility.HtmlEncode(RemovedSourceCountText) %></div>
                        </div>
                        <div class="personhub-summary-card">
                            <div class="personhub-summary-card__label">Truy cập /gianhang/admin</div>
                            <div class="personhub-summary-card__value" style="font-size:16px; line-height:1.6;"><%=HttpUtility.HtmlEncode(AdminAccessSummaryText) %></div>
                        </div>
                        <div class="personhub-summary-card">
                            <div class="personhub-summary-card__label">Lần ghi nhận đầu</div>
                            <div class="personhub-summary-card__value" style="font-size:16px; line-height:1.6;"><%=HttpUtility.HtmlEncode(FirstSeenText) %></div>
                        </div>
                        <div class="personhub-summary-card">
                            <div class="personhub-summary-card__label">Ghi nhận gần nhất</div>
                            <div class="personhub-summary-card__value" style="font-size:16px; line-height:1.6;"><%=HttpUtility.HtmlEncode(LatestSeenText) %></div>
                        </div>
                    </div>

                    <div class="mt-4">
                        <asp:Literal ID="lit_link_status" runat="server" />
                    </div>

                    <div class="mt-5">
                        <div class="personhub-panel__title" style="font-size:16px;">Nguồn dữ liệu đang dùng chung số điện thoại này</div>
                        <div class="personhub-panel__sub">
                            Chỉ cần gắn Home một lần ở đây, tất cả nguồn bên dưới sẽ tự nhận cùng trạng thái liên kết trong không gian hiện tại.
                        </div>
                        <div class="personhub-source-list">
                            <asp:Repeater ID="RepeaterSources" runat="server">
                                <ItemTemplate>
                                    <div class="personhub-source-item">
                                        <div class="text-bold"><%#HttpUtility.HtmlEncode(Eval("SourceLabel").ToString()) %></div>
                                        <div class="mt-1">
                                            <a class="fg-cobalt" href="<%#HttpUtility.HtmlAttributeEncode(Eval("DetailUrl").ToString()) %>"><%#HttpUtility.HtmlEncode(Eval("Name").ToString()) %></a>
                                        </div>
                                        <div class="personhub-source-item__meta">
                                            Vai trò: <strong><%#HttpUtility.HtmlEncode(Eval("RoleLabel").ToString()) %></strong>
                                            • Trạng thái nguồn:
                                            <span class="data-wrapper"><code class="<%#HttpUtility.HtmlAttributeEncode(RenderSourceLifecycleCss(Eval("SourceLifecycleCss"))) %>"><%#HttpUtility.HtmlEncode(RenderSourceLifecycleLabel(Eval("SourceLifecycleLabel"))) %></code></span>
                                            • Quyền /gianhang/admin:
                                            <span class="data-wrapper"><code class="<%#HttpUtility.HtmlAttributeEncode(RenderAdminAccessCss(Eval("AdminAccessCss"))) %>"><%#HttpUtility.HtmlEncode(RenderAdminAccessLabel(Eval("AdminAccessLabel"))) %></code></span>
                                            <asp:PlaceHolder ID="phEmail" runat="server" Visible='<%# !string.IsNullOrWhiteSpace(Eval("Email").ToString()) %>'>
                                                • Email: <strong><%#HttpUtility.HtmlEncode(Eval("Email").ToString()) %></strong>
                                            </asp:PlaceHolder>
                                            <%# RenderCreatedAt(Eval("CreatedAt")) %>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:PlaceHolder ID="ph_empty_sources" runat="server" Visible="false">
                                <div class="personhub-source-item">
                                    <div class="text-bold">Hiện chưa còn vai trò nguồn nào</div>
                                    <div class="personhub-source-item__meta">
                                        Hồ sơ người trung tâm vẫn đang giữ trạng thái liên kết Home để bạn theo dõi. Nếu sau này số điện thoại này xuất hiện lại ở nhân sự, khách hàng, chuyên gia hoặc thành viên thì các vai trò mới sẽ tự nhận về đúng hồ sơ này.
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                        <div class="mt-4 personhub-note">
                            <div class="text-bold">Quy tắc giữ liên kết</div>
                            <div class="fg-gray mt-1">
                                Nếu một vai trò ở module nguồn bị xóa hoặc ngừng dùng, liên kết Home ở trung tâm vẫn được giữ nếu số điện thoại này còn xuất hiện ở vai trò khác trong cùng không gian.
                            </div>
                            <div class="fg-gray mt-1">
                                Khi không còn bất kỳ vai trò nguồn nào nữa, hồ sơ người sẽ tự trở nên rỗng và bạn có thể chủ động gỡ liên kết nếu muốn.
                            </div>
                        </div>

                        <asp:PlaceHolder ID="ph_removed_sources" runat="server" Visible="false">
                            <div class="mt-5">
                                <div class="personhub-panel__title" style="font-size:16px;">Vai trò đã bị gỡ khỏi nguồn</div>
                                <div class="personhub-panel__sub">
                                    Đây là các vai trò từng xuất hiện với cùng số điện thoại trong không gian này nhưng hiện đã bị xóa khỏi module nguồn. Hồ sơ người trung tâm vẫn giữ lịch sử để bạn tra cứu.
                                </div>
                                <div class="personhub-source-list">
                                    <asp:Repeater ID="RepeaterRemovedSources" runat="server">
                                        <ItemTemplate>
                                            <div class="personhub-source-item" style="border-color:#f3d6b3;background:#fff9f2;">
                                                <div class="text-bold"><%#HttpUtility.HtmlEncode(Eval("SourceLabel").ToString()) %></div>
                                                <div class="mt-1"><%#HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(Eval("DisplayName").ToString()) ? DisplayName : Eval("DisplayName").ToString()) %></div>
                                                <div class="personhub-source-item__meta">
                                                    Vai trò trước đây: <strong><%#HttpUtility.HtmlEncode(Eval("RoleLabel").ToString()) %></strong>
                                                    • Trạng thái:
                                                    <span class="data-wrapper"><code class="bg-orange fg-white">Đã gỡ khỏi nguồn</code></span>
                                                    • <%#RenderRemovedSourceMeta((GianHangAdminPersonHub_cl.PersonRemovedSourceRef)Container.DataItem) %>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </div>

                <div class="personhub-panel">
                    <div class="personhub-panel__title">Liên kết tài khoản Home</div>
                    <div class="personhub-panel__sub">
                        Dùng hộp tìm kiếm bên dưới nếu người này đã có tài khoản AhaSale/Home. Nếu chưa có, chỉ cần tạo trạng thái chờ liên kết theo chính số điện thoại này.
                    </div>

                    <div class="mt-4">
                        <label class="fw-600">Số điện thoại dùng làm khóa liên kết</label>
                        <div class="mt-1"><strong><%=HttpUtility.HtmlEncode(DisplayPhone) %></strong></div>
                    </div>

                    <div class="mt-4">
                        <label class="fw-600">Tìm tài khoản AhaSale/Home đã có</label>
                        <div class="home-account-search-box">
                            <asp:TextBox ID="txt_home_account_link" runat="server" CssClass="input-large mt-1" placeholder="Nhập tài khoản Home hoặc số điện thoại đã có trên AhaSale"></asp:TextBox>
                            <div id="home-account-search-result" class="home-account-search-result"></div>
                        </div>
                        <div class="fg-gray mt-1">Nếu tìm thấy tài khoản phù hợp, bấm nút liên kết để gắn ngay cho toàn bộ vai trò cùng số điện thoại.</div>
                    </div>

                    <div class="mt-4">
                        <asp:Button ID="but_link_existing" runat="server" Text="Liên kết tài khoản AhaSale đã có" CssClass="button success" OnClick="but_link_existing_Click" />
                    </div>

                    <hr class="mt-6 mb-6" />

                    <div>
                        <div class="fw-600">Tạo chờ liên kết theo số điện thoại này</div>
                        <div class="fg-gray mt-1">
                            Dùng khi người này chưa có tài khoản AhaSale. Khi tài khoản Home tương ứng đăng ký hoặc đăng nhập sau này, hệ thống sẽ tự gắn.
                        </div>
                        <div class="mt-3">
                            <asp:Button ID="but_create_pending" runat="server" Text="Tạo chờ liên kết" CssClass="button warning" OnClick="but_create_pending_Click" />
                            <asp:Button ID="but_unlink" runat="server" Text="Gỡ liên kết" CssClass="button alert ml-2" OnClick="but_unlink_Click" />
                        </div>
                    </div>

                    <asp:PlaceHolder ID="ph_internal_hint" runat="server" Visible="false">
                        <div class="mt-6 personhub-note">
                            <div class="text-bold">Vai trò nội bộ và quyền vào /gianhang/admin</div>
                            <div class="fg-gray mt-1">
                                <%=HttpUtility.HtmlEncode(InternalAccessHintHtml) %>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>

    <script>
        (function () {
            var input = document.getElementById('<%=txt_home_account_link.ClientID %>');
            var resultBox = document.getElementById('home-account-search-result');
            if (!input || !resultBox) {
                return;
            }

            var activeRequest = null;

            function closeResult() {
                resultBox.style.display = 'none';
                resultBox.innerHTML = '';
            }

            function renderItems(items) {
                if (!items || !items.length) {
                    resultBox.innerHTML = "<div class='home-account-search-empty'>Không tìm thấy tài khoản AhaSale phù hợp.</div>";
                    resultBox.style.display = 'block';
                    return;
                }

                var html = '';
                items.forEach(function (item) {
                    var label = item.name || item.account || '';
                    var meta = [];
                    if (item.account) meta.push(item.account);
                    if (item.phone) meta.push(item.phone);
                    if (item.email) meta.push(item.email);
                    html += "<button type='button' class='home-account-search-item' data-account='" + (item.account || '') + "'>"
                        + "<strong>" + label + "</strong>"
                        + "<span class='home-account-search-meta'>" + meta.join(' • ') + "</span>"
                        + "</button>";
                });

                resultBox.innerHTML = html;
                resultBox.style.display = 'block';
            }

            input.addEventListener('input', function () {
                var keyword = input.value || '';
                if (keyword.trim().length < 2) {
                    closeResult();
                    return;
                }

                if (activeRequest && activeRequest.abort) {
                    activeRequest.abort();
                }

                activeRequest = new XMLHttpRequest();
                activeRequest.open('GET', '/gianhang/admin/quan-ly-tai-khoan/tim-tai-khoan-home.ashx?q=' + encodeURIComponent(keyword), true);
                activeRequest.onreadystatechange = function () {
                    if (activeRequest.readyState !== 4) {
                        return;
                    }

                    if (activeRequest.status !== 200) {
                        closeResult();
                        return;
                    }

                    try {
                        var payload = JSON.parse(activeRequest.responseText || '{}');
                        renderItems(payload.items || []);
                    } catch (e) {
                        closeResult();
                    }
                };
                activeRequest.send();
            });

            resultBox.addEventListener('click', function (evt) {
                var button = evt.target.closest('.home-account-search-item');
                if (!button) {
                    return;
                }

                input.value = button.getAttribute('data-account') || '';
                closeResult();
            });

            document.addEventListener('click', function (evt) {
                if (!resultBox.contains(evt.target) && evt.target !== input) {
                    closeResult();
                }
            });
        })();
    </script>
</asp:Content>
