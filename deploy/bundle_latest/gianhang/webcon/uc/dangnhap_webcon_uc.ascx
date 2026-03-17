<%@ Control Language="C#" AutoEventWireup="true" CodeFile="dangnhap_webcon_uc.ascx.cs" Inherits="webcon_uc_dangnhap_webcon_uc" %>
<div id='form_dangnhap' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1040!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
    <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
        <div style='position: absolute; right: 18px; top: 18px; z-index: 1040!important'>
            <a class='fg-white d-inline c-pointer' onclick='show_hide_id_form_dangnhap()' title='Đóng'>
                <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
            </a>
        </div>

        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
            <h5>Đăng nhập</h5>
            <hr />
            <asp:Panel ID="Panel2" runat="server" DefaultButton="but_login">
                <div class="mt-3">
                    <div class="fw-600">Số điện thoại</div>
                    <div>
                        <asp:TextBox ID="txt_sdt" runat="server" data-role="input"></asp:TextBox><%--autocomplete="off" --%>
                    </div>
                </div>
                <div class="mt-3">
                    <div class="fw-600">Mật khẩu</div>
                    <div>
                        <asp:TextBox TextMode="Password" ID="txt_pass" runat="server" data-role="input"></asp:TextBox>
                    </div>
                </div>

                <div class="mt-6 mb-10">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div style="float: left">
                                <%--<small><a href="/quen-mat-khau/default.aspx" class="app-bar-item fg-dark fg-red-hover">Quên mật khẩu?</a></small>--%>
                            </div>
                            <div style="float: right">
                                <asp:Button ID="but_login" runat="server" Text="ĐĂNG NHẬP" CssClass="button success rounder" OnClick="but_login_Click"/>
                            </div>
                            <div style="clear: both"></div>
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
            </asp:Panel>
        </div>

    </div>
</div>
<script>
    function show_hide_id_form_dangnhap() {
        var x = document.getElementById("form_dangnhap");
        if (x.style.display === "none") { x.style.display = "block"; }
        else { x.style.display = "none"; }
    };
</script>
