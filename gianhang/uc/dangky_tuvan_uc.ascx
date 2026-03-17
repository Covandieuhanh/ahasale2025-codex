<%@ Control Language="C#" AutoEventWireup="true" CodeFile="dangky_tuvan_uc.ascx.cs" Inherits="uc_dangky_tuvan_uc" %>
<div id='form_dangkytuvan' style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; z-index: 1001!important; overflow: auto; display: none; background-image: url('/uploads/images/bg1.png');">
    <div style='top: 0; left: 0; margin: 0 auto; max-width: 554px; opacity: 1;'>
        <div style='position: absolute; right: 18px; top: 18px; z-index: 1001!important'>
            <a  class='fg-white d-inline c-pointer' onclick='show_hide_id_form_dangkytuvan()' title='Đóng'>
                <span class='mif mif-cross mif-2x fg-red fg-darkRed-hover'></span>
            </a>
        </div>

        <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4 pt-8">
            <h5>ĐĂNG KÝ TƯ VẤN</h5>
            <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
                <div class="mt-3">
                    <div class="fw-600">Tên của bạn</div>
                    <asp:TextBox ID="txt_ten_dangky" runat="server" data-role="input" data-clear-button="true"></asp:TextBox>
                </div>
                <div class="mt-3">
                    <div class="fw-600">Số điện thoại của bạn</div>
                    <asp:TextBox ID="txt_sdt_dangky" runat="server" data-role="input" data-clear-button="true"></asp:TextBox>
                </div>
                <div class="mt-3">
                    <div class="fw-600">Nội dung cần tư vấn</div>
                    <asp:TextBox ID="txt_noidung1_dangky" data-role="textarea" runat="server" TextMode="MultiLine" data-clear-button="true"></asp:TextBox>
                </div>

                <%--<div class="mt-3">
                    <div class="fw-600">Dòng xe cần tư vấn</div>
                    <asp:DropDownList ID="DropDownList1" runat="server" data-role="select" data-filter="false">
                    </asp:DropDownList>
                </div>--%>
                <div class="mt-6 mb-10">

                    <div style="float: left">
                    </div>
                    <div style="float: right">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button1" runat="server" Text="GỌI LẠI CHO TÔI" CssClass="button success" OnClick="Button1_Click" /><%--OnClientClick="show_hide_id_form_dangkytuvan()"--%>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="UpdatePanel3">
                            <ProgressTemplate>
                                <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                                    <div style="padding-top: 45vh;">
                                        <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
                                    </div>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </div>
                    <div style="clear: both"></div>

                </div>
            </asp:Panel>
        </div>
    </div>

</div>
<script>
    function show_hide_id_form_dangkytuvan() {
        var x = document.getElementById("form_dangkytuvan");
        if (x.style.display === "none") { x.style.display = "block"; }
        else { x.style.display = "none"; }
    };
</script>
