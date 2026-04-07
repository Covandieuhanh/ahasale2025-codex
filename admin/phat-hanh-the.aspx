<%@ Page Title="Phát hành thẻ" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master"
    AutoEventWireup="true" CodeFile="phat-hanh-the.aspx.cs" Inherits="admin_phat_hanh_the" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .card-history-toolbar-controls {
            display: flex;
            align-items: center;
            gap: 6px;
            flex-wrap: wrap;
        }

        .card-history-toolbar-controls .card-history-search {
            min-width: 260px;
        }

        .card-history-toolbar-controls .card-history-pager {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            flex-wrap: wrap;
        }

        @media (max-width: 849.98px) {
            .card-history-toolbar-controls {
                width: 100%;
                margin-top: 8px;
            }

            .card-history-toolbar-controls .card-history-search {
                width: 100%;
                min-width: 0;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <!-- FORM PHÁT HÀNH (TRANG RIÊNG / FULL-PAGE) -->
    <asp:Panel ID="pn_add" runat="server" Visible="false" DefaultButton="but_tao_the">
        <div class="admin-fullpage-head admin-route-panel-head">
            <div class='admin-fullpage-shell admin-fullpage-head-shell admin-route-panel-shell admin-route-panel-head-shell'>
                <div class="admin-route-panel-actions">
                    <asp:HyperLink ID="lnk_back_list" runat="server" CssClass="admin-route-back-link">Quay lại danh sách</asp:HyperLink>
                </div>
                <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                    <div class="pt-4 text-upper text-bold">
                        PHÁT HÀNH THẺ
                    </div>
                    <hr />
                </div>
            </div>
        </div>
        <div class="admin-fullpage-body-wrap admin-route-panel-body-wrap">
            <div class='admin-fullpage-shell admin-fullpage-dialog admin-route-panel-shell admin-route-panel-dialog'>
                <div class="bg-white border bd-transparent admin-fullpage-body admin-route-panel-body pl-4 pl-8-md pr-8-md pr-4">
                    <div class="row">
                        <div class="cell-lg-12">
                            <div class="mt-3">
                                <label class="fw-600 fg-red">Chọn tài khoản</label>
                                <div>
                                    <asp:DropDownList ID="ddl_taikhoan" runat="server" data-role="select" data-filter="true"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mt-3">
                                <label class="fw-600 fg-red">Loại thẻ</label>
                                <div>
                                    <asp:DropDownList ID="ddl_loaithe" runat="server" CssClass="input-large">
                                        <asp:ListItem Value="" Text="-- Chọn tài khoản trước --"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Thẻ ưu đãi"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="Thẻ tiêu dùng"></asp:ListItem>
                                        <asp:ListItem Value="5" Text="Thẻ lao động"></asp:ListItem>
                                        <asp:ListItem Value="4" Text="Thẻ đồng hành hệ sinh thái"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="Thẻ gian hàng đối tác"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="mt-6 mb-20 text-right">
                        <asp:Button ID="but_tao_the" runat="server" Text="PHÁT HÀNH"
                            CssClass="button success" OnClick="but_tao_the_Click" />
                    </div>
                    <div class="mb-20"></div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- MAIN LIST -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" class="aha-admin-toolbar">
                    <ul class="h-menu bg-white">
                        <li data-role="hint" data-hint-position="top" data-hint-text="Phát hành thẻ">
                            <asp:HyperLink ID="but_show_add" runat="server" CssClass="button primary">
                                <span class="mif-plus mr-1"></span>Phát hành thẻ
                            </asp:HyperLink>
                        </li>

                        <li class="bd-gray border bd-default mt-2 d-block-lg d-none" style="height: 24px"></li>

                        <li>
                            <a data-role="hint" data-hint-position="top" data-hint-text="Hiển thị">
                                <small>
                                    <asp:Label ID="lb_show" runat="server" /></small>
                            </a>
                        </li>

                        <li class="ml-auto">
                            <div class="card-history-toolbar-controls">
                                <asp:TextBox ID="txt_search_cards" runat="server" CssClass="input-small card-history-search"
                                    placeholder="Tìm tài khoản, tên, loại thẻ, trạng thái..." />
                                <asp:LinkButton ID="but_search_cards" runat="server" CssClass="button mini primary"
                                    OnClick="but_search_cards_Click">
                                    Tìm
                                </asp:LinkButton>
                                <asp:LinkButton ID="but_clear_search_cards" runat="server" CssClass="button mini"
                                    OnClick="but_clear_search_cards_Click">
                                    Xóa lọc
                                </asp:LinkButton>
                                <asp:LinkButton ID="but_prev_cards" runat="server" CssClass="button mini"
                                    OnClick="but_prev_cards_Click">
                                    <span class="mif-arrow-left"></span> Quay lại
                                </asp:LinkButton>
                                <asp:LinkButton ID="but_next_cards" runat="server" CssClass="button mini"
                                    OnClick="but_next_cards_Click">
                                    Xem tiếp <span class="mif-arrow-right"></span>
                                </asp:LinkButton>
                                <span class="card-history-pager">
                                    <asp:Repeater ID="rpt_pager_cards" runat="server" OnItemCommand="rpt_pager_cards_ItemCommand">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnk_page_cards" runat="server"
                                                CssClass='<%# ((bool)Eval("IsCurrent")) ? "button mini primary" : "button mini" %>'
                                                CommandName="page"
                                                CommandArgument='<%# Eval("PageNumber") %>'
                                                Enabled='<%# !((bool)Eval("IsCurrent")) %>'>
                                                <%# Eval("PageNumber") %>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </span>
                            </div>
                        </li>
                    </ul>
                </div>
            </div>

            <div class="p-3 aha-admin-section">
                <div class="aha-admin-card p-4">
                    <div class="d-flex flex-align-center mb-3">
                        <h5 class="m-0 text-bold">DANH SÁCH THẺ</h5>
                    </div>

                    <div class="table-responsive">
                        <table class="table table-border striped cell-border compact">
                            <thead class="bg-light">
                                <tr>
                                    <th class="text-center">#</th>
                                    <th>Tài khoản</th>
                                    <th>Tên tài khoản home</th>
                                    <th>Loại thẻ</th>
                                    <th>Ngày phát hành</th>
                                    <th class="text-center">Trạng thái</th>
                                    <th>Ngày tạo</th>
                                    <th>Người tạo</th>
                                    <th>Lần tắt/bật cuối</th>
                                    <th class="text-center">Thao tác</th>
                                </tr>
                            </thead>

                            <tbody>
                                <asp:Repeater ID="Repeater1" runat="server" OnItemCommand="Repeater1_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="text-center text-bold"><%# Eval("Stt") %></td>

                                            <td class="text-bold fg-darkBlue"><%# Eval("taikhoan") %></td>
                                            <td><%# Eval("TenTaiKhoanHome") %></td>

                                            <td><%# Eval("TenThe") %></td>

                                            <td><%# ((DateTime)Eval("NgayPhatHanh")).ToString("dd/MM/yyyy HH:mm") %></td>

                                            <td class="text-center">
                                                <span class='<%# (bool)Eval("TrangThai") ? "fg-emerald" : "fg-red" %>'>
                                                    <%# (bool)Eval("TrangThai") ? "Hoạt động" : "Bị khóa" %>
                                                </span>
                                            </td>

                                            <td><%# ((DateTime)Eval("NgayTao")).ToString("dd/MM/yyyy HH:mm") %></td>

                                            <td><%# Eval("NguoiTao") %></td>

                                            <td>
                                                <%# Eval("NgayCapNhatTrangThai") == null
        ? ""
        : ((DateTime)Eval("NgayCapNhatTrangThai")).ToString("dd/MM/yyyy HH:mm") %>
                                            </td>

                                            <td class="text-center">
                                                <div class="dropdown-button place-right">
                                                    <button class="button small bg-transparent" type="button">
                                                        <span class="mif mif-more-horiz"></span>
                                                    </button>
                                                    <ul class="d-menu place-right" data-role="dropdown">
    <li>
        <asp:LinkButton runat="server"
            CommandName="toggle"
            CommandArgument='<%# Eval("idGuide") %>'>
            Bật / Tắt
        </asp:LinkButton>
    </li>

    <li class="divider"></li>

    <li>
        <a href="javascript:void(0)"
           onclick='copyNfcLink("<%# Eval("idGuide") %>"); return false;'>
            Copy link NFC
        </a>
    </li>
</ul>

                                                </div>
                                            </td>

                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>

                        </table>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
  <script>
      function getAllowedCardValuesByScope(scope) {
          var s = (scope || "").toLowerCase();
          if (s === "<%= PortalScope_cl.ScopeShop %>") {
              return { "3": true };
          }
          if (s === "<%= PortalScope_cl.ScopeHome %>") {
              return { "1": true, "2": true, "5": true, "4": true };
          }
          return {};
      }

      function refillCardTypeOptions() {
          var accountDdl = document.getElementById("<%= ddl_taikhoan.ClientID %>");
          var cardDdl = document.getElementById("<%= ddl_loaithe.ClientID %>");
          if (!accountDdl || !cardDdl) return;

          var selected = accountDdl.options[accountDdl.selectedIndex];
          var scope = selected ? (selected.getAttribute("data-scope") || "") : "";
          var allowed = getAllowedCardValuesByScope(scope);
          var hasAllowed = false;

          for (var i = 1; i < cardDdl.options.length; i++) {
              var op = cardDdl.options[i];
              var ok = !!allowed[op.value];
              op.disabled = !ok;
              op.hidden = !ok;
              if (ok) hasAllowed = true;
          }

          if (!hasAllowed || !allowed[cardDdl.value]) {
              cardDdl.value = "";
          }
      }

      document.addEventListener("DOMContentLoaded", function () {
          var accountDdl = document.getElementById("<%= ddl_taikhoan.ClientID %>");
          if (accountDdl) {
              accountDdl.addEventListener("change", refillCardTypeOptions);
          }
          refillCardTypeOptions();
      });

      function copyNfcLink(idGuide) {
          var link = window.location.origin + "/home/taikhoan.aspx?key=" + encodeURIComponent(idGuide);

          if (navigator.clipboard && window.isSecureContext) {
              navigator.clipboard.writeText(link).then(function () {
                  metroNotifyCopied();
              }).catch(function () {
                  copyFallback(link);
              });
          } else {
              copyFallback(link);
          }
      }

      function copyFallback(text) {
          var ta = document.createElement("textarea");
          ta.value = text;
          ta.setAttribute("readonly", "readonly");
          ta.style.position = "fixed";
          ta.style.left = "-9999px";
          ta.style.top = "-9999px";
          document.body.appendChild(ta);
          ta.select();
          document.execCommand("copy");
          document.body.removeChild(ta);
          metroNotifyCopied();
      }

      function metroNotifyCopied() {
          if (window.Metro && Metro.notify && Metro.notify.create) {
              Metro.notify.create("Đã sao chép link NFC!", "Thông báo", { keepOpen: false, cls: "success" });
          } else {
              console.log("Đã sao chép link NFC!");
          }
      }
  </script>


</asp:Content>
