<%@ Page Language="C#" AutoEventWireup="true" CodeFile="cho-thanh-toan.aspx.cs" Inherits="home_cho_thanh_toan" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AHASALE.VN</title>
    <meta charset="UTF-8" />
    <meta http-equiv="content-language" content="vi" />
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=yes" />

    <link href="/Metro-UI-CSS-master/css/metro-all.min.css" rel="stylesheet" />
    <link href="/css/home-style.css" rel="stylesheet" />
    <link href="/css/login.css?v=2026-03-02.1" rel="stylesheet" />
    <link href="/css/aha-ui-refresh.css?v=2026-03-02.1" rel="stylesheet" />
    <link href="/css/bcorn-with-metro.css" rel="stylesheet" />
    <link href="/css/fix-metro.css" rel="stylesheet" />

    <style>
        html, body {
            min-height: 100%;
        }

        .body-login-bcorn1 {
            background-color: #1e2329;
            overflow-x: hidden;
            overflow-y: auto;
            -webkit-overflow-scrolling: touch;
        }

        .bg-login-bcorn1 { background: linear-gradient(#181a20,#1e2329); }
        .fg-ahasale { color: #eaecef; }
        .fg-ahasale1 { color: #848e9c; }
        .bg-ahasale1 { background-color: #1e2329; }
        .bg-ahasale { background-color: #181a20; }

        .pay-pin-input {
            display: flex;
            align-items: center;
            width: 100%;
            min-height: 50px;
            border: 1px solid rgba(15, 23, 42, 0.2);
            border-radius: 14px;
            background: rgba(255, 255, 255, 0.96);
            overflow: hidden;
        }

        .pay-pin-input .pin-icon {
            flex: 0 0 52px;
            text-align: center;
            color: #111827;
            font-size: 18px;
            line-height: 50px;
            border-right: 1px solid rgba(15, 23, 42, 0.12);
        }

        .pay-pin-input .pay-pin-control {
            width: 100%;
            min-width: 0;
            height: 50px !important;
            border: 0 !important;
            outline: 0;
            box-shadow: none !important;
            background: transparent !important;
            color: #111827;
            padding: 0 14px !important;
            border-radius: 0 !important;
        }

        .pay-pin-input .pay-pin-control::placeholder {
            color: #6b7280;
        }

        .pay-pin-input:focus-within {
            box-shadow: 0 0 0 3px rgba(110, 231, 183, 0.28);
            border-color: rgba(16, 185, 129, 0.65);
        }
    </style>
</head>

<body class="body-login-bcorn1">
<form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True"></asp:ScriptManager>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <asp:Timer ID="Timer1" runat="server" Interval="5000" OnTick="Timer1_Tick"></asp:Timer>

            <div class="bg-login-bcorn1">
                <div class="fg-white text-center">
                    <div class="text-bold" style="font-size: 16px">
                        <div class="text-center" style="padding-top: 40px; padding-bottom: 20px;">
                            <img src="/uploads/images/logo-aha-trang.png" width="70" />
                        </div>
                        <div>
                            CHỜ Trao đổi ĐƠN HÀNG SỐ
                            <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                        </div>
                        <asp:PlaceHolder ID="ph_shop_home" runat="server" Visible="false">
                            <div class="mt-2">
                                <a href="/shop/default.aspx" class="button small rounded bg-lightBlue fg-white">Quay về Trang chủ shop</a>
                            </div>
                        </asp:PlaceHolder>

                        <div>
                            <small class="fg-yellow">Quyền tiêu dùng là đơn vị quy ước nội bộ được hiển thị và sử dụng trên nền tảng ahasale.vn, đóng vai trò làm phương tiện trao đổi giá trị.
<br />Tỷ lệ tham chiếu: 1 Quyền tiêu dùng tương đương 1000 VNĐ (Việt Nam đồng).
<br />Lưu ý: Quyền tiêu dùng không phải là tiền tệ hợp pháp và chỉ có hiệu lực trong phạm vi nền tảng.</small>
                        </div>
                    </div>
                </div>

                <div style="margin: 0 auto; max-width: 420px; z-index: 0;" class="pl-4 pr-4 pl-0-md pr-0-md">

                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="false">
                        <div class="bg-ahasale fg-ahasale text-center mt-4 p-4" style="border-radius: 20px">
                            <div>Vui lòng chạm thẻ khách hàng để Trao đổi.</div>

                            <div class="mt-1 mb-1">
                                <div class="button mini light rounded text-bold">
                                    <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                                </div>
                            </div>

                            <div class="mt-2">
                                <asp:Button ID="Button1" OnClick="Button1_Click" runat="server" Text="Hủy chờ Trao đổi" CssClass="small alert rounded" />
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="false">
                        <div class="bg-ahasale fg-ahasale text-center mt-4 p-4" style="border-radius: 20px">
                            <div>
                                Xin chào,
                                <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                            </div>

                            <div class="mt-1 mb-1">
                                <small>
                                    Bạn sắp trao đổi
                                    <span class="button mini bg-amber fg-black bg-darkAmber-hover rounded text-bold">
                                        <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                    </span>
                                    với
                                    <asp:Label ID="lb_tenshop" runat="server" Text=""></asp:Label>
                                </small>
                            </div>

                            <div class="mb-1 fg-red">
                                <small>Nếu không phải là bạn, vui lòng nhấn nút <b>Hủy</b></small>
                            </div>

                            <div class="mt-2">
                                <div class="pay-pin-input">
                                    <span class="mif-key pin-icon"></span>
                                    <asp:TextBox
                                        MaxLength="4"
                                        TextMode="Password"
                                        ID="txt_mapin"
                                        runat="server"
                                        Attributes="inputmode:numeric;pattern:[0-9]*"
                                        CssClass="pay-pin-control"
                                        placeholder="Nhập mã pin để hoàn tất Trao đổi">
                                    </asp:TextBox>
                                </div>
                            </div>

                            <div class="mt-1 fg-yellow">
                                <asp:Literal ID="lb_thongbao_the" runat="server" Visible="false"></asp:Literal>
                            </div>

                            <div class="mt-2 d-flex flex-justify-between flex-equal-item">
                                <asp:Button ID="Button2" OnClick="Button2_Click" runat="server" Text="Hủy" CssClass="alert mr-1 flat-button" Width="100%" />
                                <asp:Button ID="Button3" OnClick="Button3_Click" runat="server" Text="Trao đổi" CssClass="bg-amber fg-black bg-darkAmber-hover ml-1 flat-button" Width="100%" />
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <div class="bg-ahasale fg-ahasale text-left mt-4 p-4" style="border-radius: 20px">
                        <div class="mb-1">Chi tiết đơn hàng</div>

                        <div class="bcorn-fix-title-table-container">
                            <table class="bcorn-fix-title-table" style="width: 100%!important">
                                <thead>
                                    <tr>
                                        <th style="width: 1px;">ID</th>
                                        <th style="width: 1px;">Ảnh</th>
                                        <th class="text-left" style="min-width: 150px;">Sản phẩm</th>
                                        <th style="min-width: 1px;">SL</th>
                                        <th style="min-width: 1px;">% Ưu đãi</th>
                                        <th style="min-width: 1px;">Tổng</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <asp:Repeater ID="Repeater2" runat="server">
                                        <ItemTemplate>
                                            <tr class="fg-black">
                                                <td class="text-center"><%# Eval("id") %></td>

                                                <td class="text-center">
                                                    <div data-role="lightbox" class="c-pointer">
                                                        <img src="<%# Eval("image") %>" class="img-cover-vuong" width="60" height="60" />
                                                    </div>
                                                </td>

                                                <td class="text-left">
                                                    <%# Eval("name") %>
                                                    <div>
                                                        <%# FormatQuyen(Eval("giaban")) %> Quyền
                                                        <%# (Convert.ToInt32(Eval("PhanTramUuDai")) > 0
                                                            ? "<span class='button mini bg-amber fg-black rounded ml-1' style='height:auto; line-height:1; padding:2px 6px;'>-"
                                                              + Eval("PhanTramUuDai") + "%</span>"
                                                            : "") %>
                                                    </div>
                                                </td>

                                                <td class="text-center"><%# Eval("soluong") %></td>

                                                <td class="text-center">
                                                    <%# (Convert.ToInt32(Eval("PhanTramUuDai")) > 0 ? (Eval("PhanTramUuDai") + "%") : "-") %>
                                                </td>

                                                <td class="text-right">
                                                    <%# FormatQuyen(Eval("thanhtien")) %> Quyền
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </div>

                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div class="bg-dark fixed-top h-100 w-100" style="opacity: 0.9; z-index: 99999!important">
                <div style="padding-top: 45vh;">
                    <div class="mx-auto color-style activity-atom" data-role="activity" data-type="atom" data-style="color" data-role-activity="true">
                        <span class="electron"></span><span class="electron"></span><span class="electron"></span>
                    </div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</form>

<script src="/Metro-UI-CSS-master/js/metro.min.js"></script>
<script src="/js/aha-ui-refresh.js?v=2026-03-02.1"></script>
</body>
</html>
