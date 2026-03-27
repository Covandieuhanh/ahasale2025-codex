<%@ Page Title="Cấu hình trang công khai gian hàng" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="gianhang_admin_cau_hinh_storefront_default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id="main-content" class="mb-10">
        <div class="row">
            <div class="cell-lg-8">
                <div class="p-4 bg-white border-radius-8 mb-4">
                    <div class="d-flex flex-justify-between flex-align-center mb-3">
                        <div>
                            <div class="h3 m-0">Cấu hình trang công khai gian hàng</div>
                            <div class="fg-gray">Toàn bộ text, block và CTA của trang công khai được quản trị tại đây. Sản phẩm, dịch vụ và nội dung công khai của gian hàng đối tác sẽ lấy từ cấu hình và dữ liệu đang vận hành trong không gian này.</div>
                        </div>
                        <div class="d-flex flex-wrap gap-2">
                            <a class="button primary" href="<%=gianhangDashboardUrl %>" target="_blank">Mở gian hàng</a>
                            <a class="button info outline" href="<%=shopPublicUrl %>" target="_blank">Mở trang công khai</a>
                        </div>
                    </div>

                    <asp:Panel ID="Panel1" runat="server" DefaultButton="ButtonSave">
                        <div class="row">
                            <div class="cell-md-4">
                                <label class="fw-600">Chế độ hiển thị</label>
                                <asp:DropDownList ID="ddl_mode" runat="server" CssClass="input-large">
                                    <asp:ListItem Value="auto">Auto</asp:ListItem>
                                    <asp:ListItem Value="hybrid">Hybrid</asp:ListItem>
                                    <asp:ListItem Value="retail">Thiên sản phẩm</asp:ListItem>
                                    <asp:ListItem Value="service">Thiên dịch vụ</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="cell-md-8">
                                <label class="fw-600">Ghi chú vận hành</label>
                                <asp:TextBox ID="txt_brand_note" runat="server" data-role="input"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mt-4">
                            <div class="cell-md-6">
                                <label class="fw-600">Text menu Trang chủ</label>
                                <asp:TextBox ID="txt_nav_home" runat="server" data-role="input"></asp:TextBox>
                            </div>
                            <div class="cell-md-6">
                                <label class="fw-600">Text menu trang công khai</label>
                                <asp:TextBox ID="txt_nav_booking" runat="server" data-role="input"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mt-4">
                            <div class="cell-md-12">
                                <label class="fw-600">Thanh thao tác nhanh</label>
                                <div class="d-flex flex-align-center mb-2">
                                    <asp:CheckBox ID="chk_quickstrip_visible" runat="server" Text="Hiển thị thanh thao tác nhanh trên gian hàng" />
                                </div>
                            </div>
                            <div class="cell-md-4">
                                <asp:TextBox ID="txt_quick_service" runat="server" data-role="input" placeholder="Dịch vụ"></asp:TextBox>
                            </div>
                            <div class="cell-md-4">
                                <asp:TextBox ID="txt_quick_product" runat="server" data-role="input" placeholder="Sản phẩm"></asp:TextBox>
                            </div>
                            <div class="cell-md-4">
                                <asp:TextBox ID="txt_quick_article" runat="server" data-role="input" placeholder="Bài viết"></asp:TextBox>
                            </div>
                            <div class="cell-md-6 mt-3">
                                <asp:TextBox ID="txt_quick_consult" runat="server" data-role="input" placeholder="Tư vấn nhanh"></asp:TextBox>
                            </div>
                            <div class="cell-md-6 mt-3">
                                <asp:TextBox ID="txt_quick_booking" runat="server" data-role="input" placeholder="Lịch hẹn"></asp:TextBox>
                            </div>
                        </div>

                        <hr class="mt-6 mb-4" />
                        <div class="h4 mb-3">Khối giới thiệu đầu trang</div>
                        <div class="mt-3">
                            <label class="fw-600">Eyebrow</label>
                            <asp:TextBox ID="txt_hero_eyebrow" runat="server" data-role="input"></asp:TextBox>
                        </div>
                        <div class="mt-3">
                            <label class="fw-600">Tiêu đề khối giới thiệu</label>
                            <asp:TextBox ID="txt_hero_title" runat="server" TextMode="MultiLine" Rows="3" data-role="textarea"></asp:TextBox>
                        </div>
                        <div class="mt-3">
                            <label class="fw-600">Mô tả khối giới thiệu</label>
                            <asp:TextBox ID="txt_hero_description" runat="server" TextMode="MultiLine" Rows="4" data-role="textarea"></asp:TextBox>
                        </div>

                        <div class="row mt-4">
                            <div class="cell-md-6">
                                <label class="fw-600">CTA 1</label>
                                <asp:TextBox ID="txt_hero_primary_text" runat="server" data-role="input" placeholder="Text"></asp:TextBox>
                                <asp:TextBox ID="txt_hero_primary_url" runat="server" data-role="input" placeholder="URL" CssClass="mt-2"></asp:TextBox>
                            </div>
                            <div class="cell-md-6">
                                <label class="fw-600">CTA 2</label>
                                <asp:TextBox ID="txt_hero_secondary_text" runat="server" data-role="input" placeholder="Text"></asp:TextBox>
                                <asp:TextBox ID="txt_hero_secondary_url" runat="server" data-role="input" placeholder="URL" CssClass="mt-2"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row mt-4">
                            <div class="cell-md-6">
                                <label class="fw-600">CTA 3</label>
                                <asp:TextBox ID="txt_hero_tertiary_text" runat="server" data-role="input" placeholder="Text"></asp:TextBox>
                                <asp:TextBox ID="txt_hero_tertiary_url" runat="server" data-role="input" placeholder="URL" CssClass="mt-2"></asp:TextBox>
                            </div>
                            <div class="cell-md-6">
                                <label class="fw-600">Nhãn KPI</label>
                                <asp:TextBox ID="txt_metric_service" runat="server" data-role="input" placeholder="Nhãn dịch vụ"></asp:TextBox>
                                <asp:TextBox ID="txt_metric_product" runat="server" data-role="input" placeholder="Nhãn sản phẩm"></asp:TextBox>
                                <asp:TextBox ID="txt_metric_article" runat="server" data-role="input" placeholder="Nhãn nội dung" CssClass="mt-2"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mt-4">
                            <div class="cell-md-6">
                                <label class="fw-600">Khối top sản phẩm</label>
                                <asp:TextBox ID="txt_stage_primary_title" runat="server" data-role="input" placeholder="Tiêu đề"></asp:TextBox>
                                <asp:TextBox ID="txt_stage_primary_desc" runat="server" TextMode="MultiLine" Rows="3" data-role="textarea" CssClass="mt-2"></asp:TextBox>
                            </div>
                            <div class="cell-md-6">
                                <label class="fw-600">Khối danh sách tin công khai</label>
                                <asp:TextBox ID="txt_stage_secondary_title" runat="server" data-role="input" placeholder="Tiêu đề"></asp:TextBox>
                                <asp:TextBox ID="txt_stage_secondary_desc" runat="server" TextMode="MultiLine" Rows="3" data-role="textarea" CssClass="mt-2"></asp:TextBox>
                            </div>
                        </div>

                        <hr class="mt-6 mb-4" />
                        <div class="h4 mb-3">Footer</div>
                        <div class="mt-3">
                            <label class="fw-600">Mô tả footer</label>
                            <asp:TextBox ID="txt_footer_description" runat="server" TextMode="MultiLine" Rows="4" data-role="textarea"></asp:TextBox>
                        </div>
                        <div class="row mt-4">
                            <div class="cell-md-3"><asp:TextBox ID="txt_footer_chip1" runat="server" data-role="input" placeholder="Chip 1"></asp:TextBox></div>
                            <div class="cell-md-3"><asp:TextBox ID="txt_footer_chip2" runat="server" data-role="input" placeholder="Chip 2"></asp:TextBox></div>
                            <div class="cell-md-3"><asp:TextBox ID="txt_footer_chip3" runat="server" data-role="input" placeholder="Chip 3"></asp:TextBox></div>
                            <div class="cell-md-3"><asp:TextBox ID="txt_footer_chip4" runat="server" data-role="input" placeholder="Chip 4"></asp:TextBox></div>
                        </div>
                        <div class="row mt-4">
                            <div class="cell-md-4"><asp:TextBox ID="txt_footer_nav_title" runat="server" data-role="input" placeholder="Tiêu đề điều hướng"></asp:TextBox></div>
                            <div class="cell-md-4"><asp:TextBox ID="txt_footer_category_title" runat="server" data-role="input" placeholder="Tiêu đề danh mục"></asp:TextBox></div>
                            <div class="cell-md-4"><asp:TextBox ID="txt_footer_contact_title" runat="server" data-role="input" placeholder="Tiêu đề liên hệ"></asp:TextBox></div>
                        </div>
                        <div class="row mt-4">
                            <div class="cell-md-6">
                                <label class="fw-600">Footer CTA 1</label>
                                <asp:TextBox ID="txt_footer_primary_text" runat="server" data-role="input" placeholder="Text"></asp:TextBox>
                                <asp:TextBox ID="txt_footer_primary_url" runat="server" data-role="input" placeholder="URL" CssClass="mt-2"></asp:TextBox>
                            </div>
                            <div class="cell-md-6">
                                <label class="fw-600">Footer CTA 2</label>
                                <asp:TextBox ID="txt_footer_secondary_text" runat="server" data-role="input" placeholder="Text"></asp:TextBox>
                                <asp:TextBox ID="txt_footer_secondary_url" runat="server" data-role="input" placeholder="URL" CssClass="mt-2"></asp:TextBox>
                            </div>
                        </div>

                        <div class="text-center mt-8">
                            <asp:Button ID="ButtonSave" runat="server" Text="CẬP NHẬT TRANG CÔNG KHAI" CssClass="button success" OnClick="ButtonSave_Click" />
                        </div>
                    </asp:Panel>
                </div>
            </div>

            <div class="cell-lg-4">
                <div class="p-4 bg-white border-radius-8 mb-4">
                    <div class="h4 mb-3">Các block trang công khai</div>
                    <div class="fg-gray mb-3">Thứ tự, text, CTA và trạng thái hiển thị của từng block được chỉnh tại màn hình sửa block.</div>
                    <div class="table-component compact">
                        <table class="table striped table-border cell-border w-100">
                            <thead>
                                <tr>
                                    <th>Rank</th>
                                    <th>Block</th>
                                    <th>Source</th>
                                    <th>Hiển thị</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptSections" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("rank") %></td>
                                            <td>
                                                <strong><%# Eval("title") %></strong>
                                                <div class="fg-gray"><%# Eval("subtitle") %></div>
                                            </td>
                                            <td><%# Eval("source_type") %></td>
                                            <td><%# GianHangStorefrontConfig_cl.ResolveBool(Eval("is_visible"), false) ? "Bật" : "Tắt" %></td>
                                            <td><a class="button small primary outline" href='/gianhang/admin/cau-hinh-storefront/edit-section.aspx?id=<%# Eval("id") %>'>Sửa</a></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </div>

                <div class="p-4 bg-white border-radius-8">
                    <div class="h4 mb-3">Nguồn hình ảnh</div>
                    <div class="fg-gray mb-3">Gian hàng không hardcode hình ảnh. Ảnh đang lấy từ các module quản trị sau:</div>
                    <div class="d-flex flex-column gap-2">
                        <a class="button info outline mb-2" href="/gianhang/admin/cau-hinh-chung/cap-nhat-thong-tin.aspx">Logo / thương hiệu</a>
                        <a class="button info outline mb-2" href="/gianhang/admin/quan-ly-module/slide-anh/default.aspx">Slider hero</a>
                        <a class="button info outline mb-2" href="/gianhang/admin/quan-ly-menu/Default.aspx">Ảnh danh mục / menu</a>
                        <a class="button info outline mb-2" href="/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=dv">Ảnh dịch vụ</a>
                        <a class="button info outline" href="/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=sp">Ảnh sản phẩm</a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>
