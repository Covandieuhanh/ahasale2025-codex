<%@ Page Title="Loại thu chi" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="nhom-thu-chi.aspx.cs" Inherits="badmin_Default" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id='form_2' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
        <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
            <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
                <a href='#' class='fg-white d-inline' onclick='show_hide_id_form_2()' title='Đóng'>
                    <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
                </a>
            </div>

            <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
                <h5>Tạo loại thu chi</h5>
                <hr />
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" DefaultButton="but_form_themnhomthuchi">
                            <div class="mt-3">
                                <asp:TextBox ID="txt_tennhomdv" runat="server" data-role="input" placeholder="Nhập tên loại thu chi"></asp:TextBox><%--autocomplete="off" --%>
                            </div>
                            <div class="mt-6 mb-10 text-right">
                                <asp:Button ID="but_form_themnhomthuchi" runat="server" Text="TẠO MỚI" CssClass="button success" OnClick="but_form_themnhomthuchi_Click" />
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanel2">
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
    <script>
        function show_hide_id_form_2() {
            var x = document.getElementById("form_2");
            if (x.style.display === "none") { x.style.display = "block"; }
            else { x.style.display = "none"; }
        };
    </script>



    <div id="main-content" class="mb-10">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="but_form_themnhomthuchi" EventName="Click" />
            </Triggers>
            <ContentTemplate>
                <div class="row mt-1-minus <%--mt-0-lg-minus mt-12-minus--%>">
                    <div class="cell-md-6 order-2 order-md-1 mt-0">
                        <div class="d-flex flex-align-center">
                            <asp:TextBox ID="txt_search" runat="server" data-role="input" data-prepend="<span class='mif mif-search'></span>" placeholder="Tìm kiếm"></asp:TextBox>
                            <asp:LinkButton ID="but_search" runat="server" CssClass="button ml-2" OnClick="but_search_Click"><span class="mif mif-search"></span></asp:LinkButton>
                        </div>
                    </div>
                    <div class="cell-md-6 order-1 order-md-2 mt-0">
                        <div class="place-right">
                            <ul class="h-menu ">
                                <li data-role="hint" data-hint-position="top" data-hint-text="Tạo mới" onclick="show_hide_id_form_2()">
                                    <a class="button"><span class="mif mif-plus"></span></a></li>
                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Lưu">
                                    <asp:ImageButton ID="but_save" runat="server" ImageUrl="/uploads/images/icon-button/but-save.png" Height="32" OnClick="but_save_Click" />
                                </li>

                                <li class="bd-gray border bd-default mt-1" style="height: 28px"></li>
                                <li data-role="hint" data-hint-position="top" data-hint-text="Xóa">
                                    <asp:ImageButton ID="but_xoa" runat="server" ImageUrl="/uploads/images/icon-button/but-bin.png" Height="32" OnClick="but_xoa_Click" />
                                </li>

                            </ul>
                        </div>
                        <div class="clr-float"></div>
                    </div>
                </div>

                <asp:TextBox ID="txt_show" Visible="false" runat="server" data-role="input" data-prepend="<span title='Hiển thị' class='mif mif-eye'></span>" data-clear-button="false" placeholder="Hiển thị"></asp:TextBox>


                <div id="table-main">
                    <div style="overflow: auto" class=" mt-4">
                        <div style="overflow: auto;" class="mt-2">
                            <table class="table row-hover <%--table-border cell-border--%> <%--striped--%> <%--compact--%>">
                                <tbody>
                                    <tr style="background-color: #ecf0f5">
                                        <td style="width: 1px;" class=" text-bold text-center">#</td>
                                        <td style="width: 1px;">
                                            <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" data-caption="" data-cls-caption="fg-lightOrange" data-cls-check="bd-gray  bg-white" onclick="$('.checkbox-table input[type=checkbox]').prop('checked', this.checked)">
                                        </td>
                                        <td class="text-bold" style="min-width: 350px">Tên nhóm thu chi</td>
                                    </tr>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="text-right"><%=stt %></td>
                                                <td class="checkbox-table">
                                                    <input class="mt-1" type="checkbox" onkeypress="if (event.keyCode==13) return false;" data-role="checkbox" data-style="2" name="check_<%#Eval("id").ToString() %>">
                                                </td>
                                                <td>
                                                    <input data-role="input" data-clear-button="false" name="name_<%#Eval("id").ToString() %>" type="text" value="<%#Eval("tennhomdv").ToString() %>" onkeypress="if (event.keyCode==13) return false;" autocomplete="off" /></td>
                                            </tr>
                                            <%stt = stt + 1; %>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                        <div class="text-center mt-4 mb-20">
                            <asp:Button ID="but_quaylai" runat="server" Text="Lùi" CssClass="" OnClick="but_quaylai_Click" />
                            <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                            <asp:Button ID="but_xemtiep" runat="server" Text="Tiếp" CssClass="" OnClick="but_xemtiep_Click" />
                        </div>
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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <script src="/js/gianhang-invoice-fast.js?v=2026-03-26.2"></script>
    <script>
        (function () {
            function bindFastUi() {
                if (!window.ahaInvoiceFast) return;
                window.ahaInvoiceFast.initSearchSubmit({
                    inputId: "<%=txt_search.ClientID %>",
                    buttonId: "<%=but_search.ClientID %>"
                });
            }
            bindFastUi();
            if (window.Sys && Sys.Application) {
                Sys.Application.add_load(bindFastUi);
            }
        })();
    </script>
    <%--<%=notifi %>--%>
</asp:Content>

