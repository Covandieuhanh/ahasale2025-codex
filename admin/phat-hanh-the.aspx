<%@ Page Title="Phát hành thẻ" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master"
    AutoEventWireup="true" CodeFile="phat-hanh-the.aspx.cs" Inherits="admin_phat_hanh_the" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <!-- POPUP THÊM MỚI -->
    <asp:UpdatePanel ID="up_add" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <asp:Panel ID="pn_add" runat="server" Visible="false" DefaultButton="but_tao_the">

                <!-- header fixed -->
                <div style="position: fixed; width: 100%; height: 52px; top: 0; left: 0; z-index: 1041!important;">
                    <div style='margin: 0 auto; max-width: 520px; opacity: 1;'>
                        <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                            <a href='#' class='fg-white d-inline' runat="server" id="close_add"
                                onserverclick="but_close_add_Click" title='Đóng'>
                                <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                            </a>
                        </div>
                        <div class="bg-white pl-4 pr-4" style="height: 52px;">
                            <div class="pt-4 text-upper text-bold">PHÁT HÀNH THẺ</div>
                            <hr />
                        </div>
                    </div>
                </div>

                <!-- overlay -->
                <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                    <div style='margin: 0 auto; max-width: 526px; opacity: 1;'>
                        <div class="bg-white border bd-transparent pl-4 pr-4" style="padding-top: 52px">

                            <div class="mt-3">
                                <label class="fw-600 fg-red">Chọn tài khoản</label>
                                <div>
                                    <asp:DropDownList ID="ddl_taikhoan" runat="server" data-role="select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mt-3">
                                <label class="fw-600 fg-red">Loại thẻ</label>
                                <div>
                                    <asp:DropDownList ID="ddl_loaithe" runat="server" data-role="select">
                                        <asp:ListItem Value="1" Text="Thẻ ưu đãi"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="Thẻ tiêu dùng"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="Thẻ gian hàng đối tác"></asp:ListItem>
                                        <asp:ListItem Value="4" Text="Thẻ đồng hành hệ sinh thái"></asp:ListItem>
                                    </asp:DropDownList>
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

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress_add" runat="server" AssociatedUpdatePanelID="up_add">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color">
                        <span class="electron"></span><span class="electron"></span><span class="electron"></span>
                    </div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <!-- MAIN LIST -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="pos-relative pb-11">
                <div id="menutop-tool-bc" style="position: fixed; top: 52px; width: 100%; z-index: 4">
                    <ul class="h-menu bg-white">
                        <li data-role="hint" data-hint-position="top" data-hint-text="Thêm mới">
                            <asp:HyperLink ID="but_show_add" runat="server">
                                <span class="mif-plus"></span>
                            </asp:HyperLink>
                        </li>

                        <li class="bd-gray border bd-default mt-2 d-block-lg d-none" style="height: 24px"></li>

                        <li class="d-block-lg d-none">
                            <a data-role="hint" data-hint-position="top" data-hint-text="Hiển thị">
                                <small>
                                    <asp:Label ID="lb_show" runat="server" /></small>
                            </a>
                        </li>
                    </ul>
                </div>
            </div>

            <div class="p-3">
                <div class="bg-white p-4 shadow-2 rounded">
                    <div class="d-flex flex-align-center mb-3">
                        <h5 class="m-0 text-bold">DANH SÁCH THẺ</h5>
                    </div>

                    <div class="table-responsive">
                        <table class="table table-border striped cell-border compact">
                            <thead class="bg-light">
                                <tr>
                                    <th class="text-center">#</th>
                                    <th>Tài khoản</th>
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
                                            <td class="text-center text-bold"><%# Container.ItemIndex + 1 %></td>

                                            <td class="text-bold fg-darkBlue"><%# Eval("taikhoan") %></td>

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
