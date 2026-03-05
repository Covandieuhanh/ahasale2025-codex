<%@ control language="C#" autoeventwireup="true" inherits="home_uc_fix_right_uc, App_Web_243tslvg" %>


<asp:UpdatePanel ID="up_tuvan" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pn_tuvan" runat="server" Visible="false" DefaultButton="Button1">
            <div style="position: fixed; width: 100%; height: 52px; background-color: none; top: 0; left: 0; z-index: 1041!important;">
                <div style='top: 0; left: 0px; margin: 0 auto; max-width: 500px; opacity: 1;'>
                    <div style='position: absolute; right: 18px; top: 14px; z-index: 1040!important'>
                        <a href='#' class='fg-white d-inline' id="close_in" runat="server" onserverclick="but_close_form_dangkytuvan_Click" title='Đóng'>
                            <span class='mif mif-cross mif-2x fg-red fg-lightRed-hover'></span>
                        </a>
                    </div>
                    <div class="bg-white pl-4 pl-8-md pr-8-md pr-4" style="height: 52px;">
                        <div class="pt-4 text-upper text-bold">
                            ĐĂNG KÝ TƯ VẤN
                        </div>
                        <hr />
                    </div>
                </div>
            </div>

            <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0; overflow: auto; z-index: 1040!important; background-image: url('/uploads/images/bg1.png');">
                <div style='top: 0; left: 0; margin: 0 auto; max-width: 506px; opacity: 1;'>
                    <div class="bg-white border bd-transparent pl-4 pl-8-md pr-8-md pr-4" style="padding-top: 52px">
                        <div class="mt-3">
                            <div class="fw-600 fg-red">Tên của bạn</div>
                            <asp:TextBox ID="txt_ten_dangky" runat="server" data-role="input" data-clear-button="true"></asp:TextBox>
                        </div>
                        <div class="mt-3">
                            <div class="fw-600 fg-red">Số điện thoại của bạn</div>
                            <asp:TextBox ID="txt_sdt_dangky" runat="server" data-role="input" data-clear-button="true"></asp:TextBox>
                        </div>
                        <div class="mt-3">
                            <div class="fw-600 fg-red">Nội dung cần tư vấn</div>
                            <asp:TextBox ID="txt_noidung1_dangky" data-role="textarea" runat="server" TextMode="MultiLine" data-clear-button="true"></asp:TextBox>
                        </div>
                        <div class="mt-6 mb-20 text-right">
                            <asp:Button ID="Button1" runat="server" Text="GỌI LẠI CHO TÔI" CssClass="button alert" OnClick="Button1_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_tuvan">
    <ProgressTemplate>
        <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
            <div style="padding-top: 45vh;">
                <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
            </div>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>



<asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
    <Triggers>
        <%--<asp:AsyncPostBackTrigger ControlID="but_add" EventName="Click" />--%>
    </Triggers>
    <ContentTemplate>

        <div class="fix_but_4 text-center" data-role="hint" data-hint-position="left" data-hint-text="Chát Facebook" style="background-color: #00B2FF">
            <a href="<%=link_zalo %>" target="_blank">
                <img src="/uploads/images/icon/zalo.png" style="width: 65%" />
            </a>
        </div>

        <div class="fix_but_3 text-center pt-2" data-role="hint" data-hint-position="left" data-hint-text="Đăng ký tư vấn" style="background-color: #ffd200">
            <asp:ImageButton ID="but_show_form_dangkytuvan" OnClick="but_show_form_dangkytuvan_Click" runat="server" Width="56%" ImageUrl="~/uploads/images/icon/tuvan-den.png" />
        </div>
   
        <div class="fix_but_2 text-center" data-role="hint" data-hint-position="left" data-hint-text="Chát Zalo" style="background-color: #0068ff">
            <a href="<%=link_zalo %>" target="_blank">
                <img src="/uploads/images/icon/zalo.png" style="width: 65%" />
            </a>
        </div>


        <div class="fix_but_1 text-center" data-role="hint" data-hint-position="left" data-hint-text="Gọi ngay" style="background-color: #ce352c">
            <a href="tel:<%=sdt %>" class="ani-ring d-block">
                <img src="/uploads/images/icon/hotline.png" style="width: 50%" />
            </a>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="up_main">
    <ProgressTemplate>
        <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
            <div style="padding-top: 45vh;">
                <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true"><span class="electron"></span><span class="electron"></span><span class="electron"></span></div>
            </div>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
