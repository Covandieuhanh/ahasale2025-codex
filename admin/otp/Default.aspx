<%@ Page Title="Quản lý OTP" Language="C#" MasterPageFile="~/admin/MasterPageAdmin.master"
    AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_otp_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .otp-card {
            border-radius: 14px;
            border: 1px solid #e4eaf1;
            background: #fff;
            box-shadow: 0 10px 24px rgba(15, 23, 42, 0.08);
        }
        .otp-card h3 {
            margin: 0 0 10px 0;
            font-size: 18px;
            font-weight: 700;
        }
        .otp-grid {
            display: grid;
            gap: 16px;
            grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
        }
        .otp-section-title {
            font-weight: 700;
            text-transform: uppercase;
            font-size: 12px;
            letter-spacing: 0.08em;
            color: #64748b;
        }
        .otp-inline-tabs {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
            margin-bottom: 12px;
        }
        .otp-inline-tab {
            padding: 8px 14px;
            border-radius: 999px;
            border: 1px solid #dbe3ec;
            color: #1f2937;
            background: #f8fafc;
            text-decoration: none;
            font-weight: 600;
        }
        .otp-inline-tab.active {
            background: #16a34a;
            border-color: #16a34a;
            color: #fff;
        }
        .otp-note {
            color: #64748b;
            font-size: 13px;
        }
        .otp-status {
            font-weight: 600;
        }
        .otp-status.ok { color: #16a34a; }
        .otp-status.warn { color: #d97706; }
        .otp-status.err { color: #dc2626; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div class="p-3">
                <div class="otp-card p-4 mb-4">
                    <div class="otp-section-title mb-2">Cấu hình OTP (SMS)</div>
                    <h3>Thiết lập đơn vị gửi OTP</h3>
                    <div class="otp-grid">
                        <div>
                            <label class="fw-600">Endpoint</label>
                            <asp:TextBox ID="txt_sms_endpoint" runat="server" data-role="input" placeholder="https://api.example.com/sms"></asp:TextBox>
                        </div>
                        <div>
                            <label class="fw-600">API Key</label>
                            <asp:TextBox ID="txt_sms_apikey" runat="server" data-role="input" placeholder="API key"></asp:TextBox>
                        </div>
                        <div>
                            <label class="fw-600">Sender</label>
                            <asp:TextBox ID="txt_sms_sender" runat="server" data-role="input" placeholder="Brand name"></asp:TextBox>
                        </div>
                        <div>
                            <label class="fw-600">Template</label>
                            <asp:TextBox ID="txt_sms_template" runat="server" data-role="input" placeholder="Ma OTP AhaSale cua ban la: {OTP}. Het han sau 5 phut."></asp:TextBox>
                        </div>
                        <div>
                            <label class="fw-600">HTTP Method</label>
                            <asp:DropDownList ID="ddl_sms_method" runat="server">
                                <asp:ListItem Text="POST" Value="POST"></asp:ListItem>
                                <asp:ListItem Text="GET" Value="GET"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="mt-3">
                        <div class="otp-section-title">Tham số API (key / value)</div>
                        <div class="otp-note">Bạn có thể dùng biến: {OTP}, {phoneNumber}, {message}, {brandName}, {timestamp}, {now}.</div>
                        <div class="row mt-2">
                            <div class="cell-lg-3">
                                <asp:TextBox ID="txt_param_key_1" runat="server" placeholder="loginName"></asp:TextBox>
                            </div>
                            <div class="cell-lg-9">
                                <asp:TextBox ID="txt_param_val_1" runat="server" placeholder="Giá trị hoặc {token}"></asp:TextBox>
                            </div>
                            <div class="cell-lg-3">
                                <asp:TextBox ID="txt_param_key_2" runat="server" placeholder="sign"></asp:TextBox>
                            </div>
                            <div class="cell-lg-9">
                                <asp:TextBox ID="txt_param_val_2" runat="server" placeholder="Giá trị hoặc {token}"></asp:TextBox>
                            </div>
                            <div class="cell-lg-3">
                                <asp:TextBox ID="txt_param_key_3" runat="server" placeholder="serviceTypeId"></asp:TextBox>
                            </div>
                            <div class="cell-lg-9">
                                <asp:TextBox ID="txt_param_val_3" runat="server" placeholder="Giá trị hoặc {token}"></asp:TextBox>
                            </div>
                            <div class="cell-lg-3">
                                <asp:TextBox ID="txt_param_key_4" runat="server" placeholder="phoneNumber"></asp:TextBox>
                            </div>
                            <div class="cell-lg-9">
                                <asp:TextBox ID="txt_param_val_4" runat="server" placeholder="{phoneNumber}"></asp:TextBox>
                            </div>
                            <div class="cell-lg-3">
                                <asp:TextBox ID="txt_param_key_5" runat="server" placeholder="message"></asp:TextBox>
                            </div>
                            <div class="cell-lg-9">
                                <asp:TextBox ID="txt_param_val_5" runat="server" placeholder="{message}"></asp:TextBox>
                            </div>
                            <div class="cell-lg-3">
                                <asp:TextBox ID="txt_param_key_6" runat="server" placeholder="brandName"></asp:TextBox>
                            </div>
                            <div class="cell-lg-9">
                                <asp:TextBox ID="txt_param_val_6" runat="server" placeholder="{brandName}"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="mt-3">
                        <asp:CheckBox ID="ck_sms_dev" runat="server" Text="Bật chế độ DEV (không gửi SMS, chỉ log)" />
                    </div>
                    <div class="mt-3 text-right">
                        <asp:Button ID="but_save_config" runat="server" Text="Lưu cấu hình" CssClass="button success" OnClick="but_save_config_Click" />
                    </div>
                    <div class="otp-note mt-2">Nếu chưa có đơn vị gửi, hãy bật DEV để hệ thống vẫn tạo OTP và ghi log.</div>
                </div>

                <div class="otp-card p-4 mb-4">
                    <div class="otp-section-title mb-2">Tạo OTP thủ công</div>
                    <h3>Cấp OTP cho tài khoản</h3>
                    <asp:Panel ID="pn_manual_home" runat="server">
                        <div class="mb-2 fw-600">Home</div>
                        <div class="row">
                            <div class="cell-lg-6">
                                <label class="fw-600">Tài khoản</label>
                                <asp:DropDownList ID="ddl_home_account" runat="server"></asp:DropDownList>
                            </div>
                            <div class="cell-lg-3">
                                <label class="fw-600">Loại OTP</label>
                                <asp:DropDownList ID="ddl_home_type" runat="server"></asp:DropDownList>
                            </div>
                            <div class="cell-lg-3">
                                <label class="fw-600">Số điện thoại</label>
                                <asp:TextBox ID="txt_home_phone" runat="server" data-role="input" placeholder="Số điện thoại"></asp:TextBox>
                            </div>
                        </div>
                        <div class="mt-3 text-right">
                            <asp:Button ID="but_home_manual" runat="server" Text="Tạo OTP Home" CssClass="button dark" OnClick="but_home_manual_Click" />
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pn_manual_shop" runat="server" CssClass="mt-4">
                        <div class="mb-2 fw-600">Shop</div>
                        <div class="row">
                            <div class="cell-lg-6">
                                <label class="fw-600">Tài khoản</label>
                                <asp:DropDownList ID="ddl_shop_account" runat="server"></asp:DropDownList>
                            </div>
                            <div class="cell-lg-6">
                                <label class="fw-600">Số điện thoại</label>
                                <asp:TextBox ID="txt_shop_phone" runat="server" data-role="input" placeholder="Số điện thoại"></asp:TextBox>
                            </div>
                        </div>
                        <div class="mt-3 text-right">
                            <asp:Button ID="but_shop_manual" runat="server" Text="Tạo OTP Shop" CssClass="button dark" OnClick="but_shop_manual_Click" />
                        </div>
                    </asp:Panel>
                </div>

                <div class="otp-card p-4">
                    <div class="otp-section-title mb-2">Lịch sử OTP</div>
                    <div class="otp-inline-tabs">
                        <asp:HyperLink ID="hl_tab_home" runat="server" CssClass="otp-inline-tab">OTP Home</asp:HyperLink>
                        <asp:HyperLink ID="hl_tab_shop" runat="server" CssClass="otp-inline-tab">OTP Shop</asp:HyperLink>
                    </div>

                    <div class="mb-3" style="max-width: 300px;">
                        <asp:TextBox ID="txt_search" runat="server" data-role="input" placeholder="Tài khoản hoặc SĐT"></asp:TextBox>
                        <div class="mt-2">
                            <asp:Button ID="but_search" runat="server" Text="Tìm" CssClass="button small" OnClick="but_search_Click" />
                            <asp:Button ID="but_clear" runat="server" Text="Làm mới" CssClass="button small light" OnClick="but_clear_Click" />
                        </div>
                    </div>

                    <div class="bcorn-fix-title-table-container">
                        <table class="bcorn-fix-title-table">
                            <thead>
                                <tr>
                                    <th style="width: 1px;">ID</th>
                                    <th class="text-center">Tài khoản</th>
                                    <th class="text-center">SĐT</th>
                                    <th class="text-center">Loại</th>
                                    <th class="text-center">OTP</th>
                                    <th class="text-center">Trạng thái</th>
                                    <th class="text-center">Thời gian</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="RepeaterOtp" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="text-center"><%# Eval("id") %></td>
                                            <td class="text-center"><%# Eval("taikhoan") %></td>
                                            <td class="text-center"><%# Eval("phone") %></td>
                                            <td class="text-center"><%# Eval("type_text") %></td>
                                            <td class="text-center"><%# Eval("otp_code") %></td>
                                            <td class="text-center"><span class='otp-status <%# Eval("status_class") %>'><%# Eval("status_text") %></span></td>
                                            <td class="text-center"><%# Eval("time_text") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <asp:PlaceHolder ID="ph_empty" runat="server" Visible="false">
                        <div class="otp-note mt-3">Chưa có lịch sử OTP.</div>
                    </asp:PlaceHolder>
                </div>
            </div>
    </div>
</asp:Content>
