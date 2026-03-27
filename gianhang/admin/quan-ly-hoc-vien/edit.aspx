<%@ Page Title="Chi tiết thành viên" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="edit.aspx.cs" Inherits="taikhoan_add" %><asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id='form_thanhtoan' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 600px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a class='fg-white d-inline c-pointer' onclick='show_hide_id_form_thanhtoan()' title='Đóng'>
                    <span class='mif mif-cross mif-lg fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Thanh toán</h5>
                <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel4" runat="server" DefaultButton="but_thanhtoan">
                            <div class="mt-7">
                                Ngày
                            </div>
                            <div>
                                <asp:TextBox ID="txt_ngaythanhtoan" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Hình thức giao dịch</label>
                                <asp:DropDownList ID="ddl_hinhthuc_thanhtoan" runat="server" data-role="select" data-filter="false">
                                    <asp:ListItem Text="Tiền mặt" Value="Tiền mặt"></asp:ListItem>
                                    <asp:ListItem Text="Chuyển khoản" Value="Chuyển khoản"></asp:ListItem>
                                    <asp:ListItem Text="Quẹt thẻ" Value="Quẹt thẻ"></asp:ListItem>
                                    <asp:ListItem Text="Voucher giấy" Value="Voucher giấy"></asp:ListItem>
                                    <asp:ListItem Text="E-Voucher (điểm)" Value="E-Voucher (điểm)"></asp:ListItem>
                                    <asp:ListItem Text="Ví điện tử" Value="Ví điện tử"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="mt-3">
                                <label class="fw-600">Nhập số tiền thanh toán</label>
                                <asp:TextBox ID="txt_sotien_thanhtoan_congno" data-role="input" runat="server" data-clear-button="true" onchange="format_sotien(this);"></asp:TextBox>
                            </div>

                            <div class="mt-6 mb-10">
                                <div style="float: left">
                                    <%--<span class="fg-red"><small>Mẹo: Làm mới trang sau khi thêm.</small></span>--%>
                                </div>
                                <div style="float: right" onclick='show_hide_id_form_thanhtoan()'>
                                    <asp:Button ID="but_thanhtoan" runat="server" Text="THANH TOÁN" CssClass="button success" OnClick="but_thanhtoan_Click" />
                                </div>
                                <div style="clear: both"></div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

        </div>
    </div>
    <script>
        function show_hide_id_form_thanhtoan() {
            var x = document.getElementById("form_thanhtoan");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>

    <div id="main-content" class=" mb-10">
        <ul data-role="tabs" data-expand="true">
            <li><a href="#hocvien">thành viên</a></li>
            <li><a href="#thanhtoan">Thanh toán</a></li>
        </ul>
        <div class="border bd-default no-border-top p-2 pl-4 pr-4 ">
            <div id="hocvien">
                <div class="row">
                    <div class="cell-12">
                        <asp:Panel ID="Panel1" runat="server" DefaultButton="button1">
                            <div class="row">
                                <div class="cell-lg-4 p-3-lg mt-0-lg mt-5">
                                    <h5>Thông tin thành viên</h5>
                                    <div class="mt-3 fg-red">
                                        <label class="fw-600">Họ tên</label>
                                        <div>
                                            <asp:TextBox ID="txt_hoten" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Ảnh đại diện</label>
                                        <span style="cursor: pointer" class="mif mif-info ml-1" data-role="popover" data-popover-text="<small>Kích thước chuẩn: 500x500 pixel.</small>" data-popover-hide="8000" data-close-button="false" data-popover-position="right" data-popover-trigger="click" data-cls-popover="drop-shadow"></span>
                                        <asp:FileUpload ID="FileUpload2" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div>
                                                    <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                                </div>
                                                <div style='position: absolute; bottom: 0px; left: 100px'>
                                                    <asp:Button ID="Button2" runat="server" Text="Xóa" CssClass="alert small" Visible="false" OnClick="Button2_Click" />
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Ngày sinh</label>
                                        <div>
                                            <asp:TextBox ID="txt_ngaysinh" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Xếp loại</label>
                                        <div>
                                            <asp:DropDownList ID="DropDownList3" runat="server" data-role="select" data-filter="false">
                                                <asp:ListItem Value="0" Text="Chưa học xong"></asp:ListItem>
                                                <asp:ListItem Value="A" Text="A (Giỏi)"></asp:ListItem>
                                                <asp:ListItem Value="B" Text="B (Đạt)"></asp:ListItem>
                                                <asp:ListItem Value="C" Text="C (Loại)"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Cấp bằng</label>
                                        <div>
                                            <asp:DropDownList ID="DropDownList4" runat="server" data-role="select" data-filter="false">
                                                <asp:ListItem Value="Chưa cấp bằng" Text="Chưa cấp bằng"></asp:ListItem>
                                                <asp:ListItem Value="Đã cấp bằng" Text="Đã cấp bằng"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Ngày cấp bằng</label>
                                        <div>
                                            <asp:TextBox ID="txt_ngaycapbang" runat="server" MaxLength="10" data-role="calendar-picker" data-outside="true" data-dialog-mode="true" data-week-start="1" data-locale="vi-VN" data-format="DD/MM/YYYY" data-input-format="DD/MM/YYYY" data-clear-button="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Hình ảnh cấp bằng</label>
                                        <asp:FileUpload ID="FileUpload3" runat="server" type="file" data-role="file" data-button-title="<span class='mif-file-upload'></span>" AllowMultiple="false" />
                                        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div data-role="lightbox" class="c-pointer">
                                                    <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                                                </div>
                                                <div style='position: absolute; bottom: 0px; left: 100px'>
                                                    <asp:Button ID="Button3" runat="server" Text="Xóa" CssClass="alert small" Visible="false" OnClick="Button3_Click" />
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                                <div class="cell-lg-4 p-3-lg mt-0-lg mt-5">
                                    <h5>Liên hệ</h5>
                                    <div class="mt-3">
                                        <label class="fw-600">Email</label>
                                        <div>
                                            <asp:TextBox ID="txt_email" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Điện thoại</label>
                                        <div>
                                            <asp:TextBox ID="txt_dienthoai" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Zalo</label>
                                        <div>
                                            <asp:TextBox ID="txt_zalo" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Facebook</label>
                                        <div>
                                            <asp:TextBox ID="txt_facebook" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-4 p-4 border bd-cyan bg-light" style="border-radius: 14px;">
                                        <div class="fw-700">Hồ sơ người AhaSale</div>
                                        <div class="mt-1 fg-gray">
                                            Thành viên này được liên kết Home tại module trung tâm Hồ sơ người. Gắn một lần ở đó là các vai trò cùng số điện thoại trong gian hàng sẽ tự đồng bộ.
                                        </div>
                                        <div class="mt-3">
                                            <span class="data-wrapper"><code class="<%=HttpUtility.HtmlAttributeEncode(personHubStatusCss) %>"><%=HttpUtility.HtmlEncode(personHubStatusLabel) %></code></span>
                                        </div>
                                        <div class="mt-2 fg-gray"><%=HttpUtility.HtmlEncode(personHubNote) %></div>
                                        <div class="mt-3">
                                            <span class="data-wrapper"><code class="<%=HttpUtility.HtmlAttributeEncode(personHubAdminAccessCss) %>"><%=HttpUtility.HtmlEncode(personHubAdminAccessLabel) %></code></span>
                                        </div>
                                        <div class="mt-2 fg-gray"><%=HttpUtility.HtmlEncode(personHubAdminAccessNote) %></div>
                                        <div class="mt-3">
                                            <span class="data-wrapper"><code class="<%=HttpUtility.HtmlAttributeEncode(sourceLifecycleCss) %>"><%=HttpUtility.HtmlEncode(sourceLifecycleLabel) %></code></span>
                                        </div>
                                        <div class="mt-2 fg-gray"><%=HttpUtility.HtmlEncode(sourceLifecycleNote) %></div>
                                        <div class="mt-3 p-3 border" style="border-radius: 12px; border-color: #cfe6ff!important; background: #f7fbff;">
                                            <div class="fw-700">Ngừng dùng an toàn</div>
                                            <div class="mt-1 fg-gray">
                                                Ngừng dùng thành viên sẽ không xóa lịch sử, không gỡ liên kết Home ở Hồ sơ người và không làm mất các vai trò khác cùng số điện thoại trong gian hàng này.
                                            </div>
                                            <div class="mt-3">
                                                <% if (!sourceLifecycleIsInactive) { %>
                                                <asp:Button ID="but_ngung_hocvien" runat="server" Text="NGỪNG DÙNG THÀNH VIÊN" CssClass="button warning" OnClick="but_ngung_hocvien_Click" />
                                                <% } else { %>
                                                <asp:Button ID="but_molai_hocvien" runat="server" Text="MỞ LẠI THÀNH VIÊN" CssClass="button success" OnClick="but_molai_hocvien_Click" />
                                                <% } %>
                                            </div>
                                        </div>
                                        <div class="mt-3 p-3 border" style="border-radius: 12px; border-color: #f3d6b3!important; background: #fff9f2;">
                                            <div class="fw-700"><%=HttpUtility.HtmlEncode(personHubImpactTitle) %></div>
                                            <div class="mt-1 fg-gray"><%=HttpUtility.HtmlEncode(personHubImpactNote) %></div>
                                        </div>
                                        <div class="mt-3">
                                            <a class="button success" href="<%=HttpUtility.HtmlAttributeEncode(personHubUrl) %>">Mở hồ sơ người</a>
                                        </div>
                                        <% if (!string.IsNullOrWhiteSpace(personHubRelatedRolesHtml)) { %>
                                        <div class="mt-3 pt-3 border-top bd-light">
                                            <div class="fw-700">Cùng số điện thoại này còn có thêm vai trò khác</div>
                                            <div class="mt-1 fg-gray">
                                                Đây là các hồ sơ khác trong cùng không gian đang dùng chung số điện thoại với thành viên này.
                                            </div>
                                            <div class="mt-2">
                                                <%=personHubRelatedRolesHtml %>
                                            </div>
                                        </div>
                                        <% } %>
                                    </div>
                                </div>
                                <div class="cell-lg-4 p-3-lg mt-0-lg mt-5">
                                    <h5>Ngành học</h5>
                                    <div class="mt-3">
                                        <label class="fw-600">Ngành học</label>
                                         <asp:DropDownList ID="DropDownList5" data-role="select" data-filter="true" runat="server"></asp:DropDownList>
                                        <%--<div>
                                            <asp:TextBox ID="txt_nganhhoc" runat="server" data-role="input"></asp:TextBox></div>--%>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Gói đào tạo</label>
                                        <div>
                                            <asp:DropDownList ID="DropDownList2" runat="server" data-role="select" data-filter="false">
                                                <asp:ListItem Value="Cơ bản" Text="Cơ bản"></asp:ListItem>
                                                <asp:ListItem Value="Nâng cao" Text="Nâng cao"></asp:ListItem>
                                                <asp:ListItem Value="Combo" Text="Combo"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Chuyên gia</label>
                                        <div>
                                            <asp:DropDownList ID="DropDownList1" runat="server" data-role="select" data-filter="false"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Số buổi lý thuyết</label>
                                        <div>
                                            <asp:TextBox ID="txt_sobuoi_lythuyet" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Số buổi thực hành</label>
                                        <div>
                                            <asp:TextBox ID="txt_sobuoi_thuchanh" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Số buổi trợ giảng</label>
                                        <div>
                                            <asp:TextBox ID="txt_sobuoi_trogiang" runat="server" data-role="input"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="mt-3">
                                        <label class="fw-600">Học phí</label>
                                        <div>
                                            <asp:TextBox ID="txt_hocphi" runat="server" data-role="input" onchange="format_sotien(this);"></asp:TextBox>
                                        </div>
                                    </div>


                                </div>
                            </div>

                            <div class="text-center mt-10">
                                <asp:Button OnClientClick="Metro.activity.open({type:'cycle',overlayClickClose:false})" ID="button1" runat="server" Text="CẬP NHẬT" CssClass="button success" OnClick="button1_Click1" />
                            </div>
                        </asp:Panel>
                    </div>
                </div>
            </div>
            <div id="thanhtoan">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="but_thanhtoan" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <div class="bg-white pl-2 pr-2">
                            <div>
                                <div style="float: left" class="">
                                    <ul class="h-menu ">
                                        <%if (bcorn_class.check_quyen(user, "q14_5") == "")
                                            { %>
                                        <li data-role="hint" data-hint-position="top" data-hint-text="Thanh toán"><a class="button" onclick='show_hide_id_form_thanhtoan()'><span class="mif mif-paypal"></span></a></li>
                                        <%} %>
                                        <%if (bcorn_class.check_quyen(user, "q14_5") == "")
                                            { %>
                                        <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                        <li data-role="hint" data-hint-position="top" data-hint-text="Xóa thanh toán">
                                            <asp:ImageButton ID="but_xoathanhtoan" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" OnClick="but_xoathanhtoan_Click" />
                                        </li>
                                        <%} %>
                                    </ul>
                                </div>
                                <div style="float: right" class="">
                                    <ul class="h-menu ">
                                    </ul>
                                </div>
                                <div class="clr-float"></div>
                            </div>
                            <hr />
                            <table class="table row-hover striped subcompact mt-3">
                                <tbody>
                                    <asp:Repeater ID="Repeater2" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td style="width: 1px">
                                                    <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_lichsu_thanhtoan_<%#Eval("id").ToString() %>">
                                                </td>

                                                <%--<td style="width: 50px">Lần <%=stt_tt %></td>--%>
                                                <td class="text-right" style="width: 118px"><%#Eval("thoigian","{0:dd/MM/yyyy HH:mm}").ToString() %></td>
                                                <td style="min-width: 70px"><%#Eval("hinhthuc_thanhtoan").ToString() %></td>
                                                <td class="text-right" style="width: 1px"><%#Eval("sotienthanhtoan","{0:#,##0}").ToString() %></td>
                                            </tr>
                                            <%--<%stt_tt = stt_tt + 1; %>--%>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                            <div class="p-2 text-right">
                                <asp:Label ID="Label1" runat="server" Text=""></asp:Label>

                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                    <ProgressTemplate>
                        <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                            <div style="padding-top: 50vh;">
                                <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                            </div>
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </div>
        </div>

    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>
