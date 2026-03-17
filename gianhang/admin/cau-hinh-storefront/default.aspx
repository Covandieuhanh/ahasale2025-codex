<%@ Page Title="Cau hinh storefront /shop" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="gianhang_admin_cau_hinh_storefront_default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id="main-content" class="mb-10">
        <div class="row">
            <div class="cell-lg-8">
                <div class="p-4 bg-white border-radius-8 mb-4">
                    <div class="d-flex flex-justify-between flex-align-center mb-3">
                        <div>
                            <div class="h3 m-0">Cau hinh storefront /shop</div>
                            <div class="fg-gray">Toan bo text, block va CTA cua storefront /shop duoc quan tri tai day. San pham/dich vu dang trong /gianhang/admin se dong bo ra feed cong khai ma /shop dang doc.</div>
                        </div>
                        <div class="d-flex flex-wrap gap-2">
                            <a class="button primary" href="<%=shopDashboardUrl %>" target="_blank">Mo /shop</a>
                            <a class="button info outline" href="<%=shopPublicUrl %>" target="_blank">Mo trang cong khai</a>
                        </div>
                    </div>

                    <asp:Panel ID="Panel1" runat="server" DefaultButton="ButtonSave">
                        <div class="row">
                            <div class="cell-md-4">
                                <label class="fw-600">Che do hien thi /shop</label>
                                <asp:DropDownList ID="ddl_mode" runat="server" CssClass="input-large">
                                    <asp:ListItem Value="auto">Auto</asp:ListItem>
                                    <asp:ListItem Value="hybrid">Hybrid</asp:ListItem>
                                    <asp:ListItem Value="retail">Retail-first</asp:ListItem>
                                    <asp:ListItem Value="service">Service-first</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="cell-md-8">
                                <label class="fw-600">Ghi chu van hanh storefront</label>
                                <asp:TextBox ID="txt_brand_note" runat="server" data-role="input"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mt-4">
                            <div class="cell-md-6">
                                <label class="fw-600">Text menu Trang chu</label>
                                <asp:TextBox ID="txt_nav_home" runat="server" data-role="input"></asp:TextBox>
                            </div>
                            <div class="cell-md-6">
                                <label class="fw-600">Text menu trang cong khai</label>
                                <asp:TextBox ID="txt_nav_booking" runat="server" data-role="input"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mt-4">
                            <div class="cell-md-12">
                                <label class="fw-600">Quick strip /shop</label>
                                <div class="d-flex flex-align-center mb-2">
                                    <asp:CheckBox ID="chk_quickstrip_visible" runat="server" Text="Hien quick strip tren /shop" />
                                </div>
                            </div>
                            <div class="cell-md-4">
                                <asp:TextBox ID="txt_quick_service" runat="server" data-role="input" placeholder="Dich vu"></asp:TextBox>
                            </div>
                            <div class="cell-md-4">
                                <asp:TextBox ID="txt_quick_product" runat="server" data-role="input" placeholder="San pham"></asp:TextBox>
                            </div>
                            <div class="cell-md-4">
                                <asp:TextBox ID="txt_quick_article" runat="server" data-role="input" placeholder="Bai viet"></asp:TextBox>
                            </div>
                            <div class="cell-md-6 mt-3">
                                <asp:TextBox ID="txt_quick_consult" runat="server" data-role="input" placeholder="Tu van nhanh"></asp:TextBox>
                            </div>
                            <div class="cell-md-6 mt-3">
                                <asp:TextBox ID="txt_quick_booking" runat="server" data-role="input" placeholder="Lich hen"></asp:TextBox>
                            </div>
                        </div>

                        <hr class="mt-6 mb-4" />
                        <div class="h4 mb-3">Hero /shop</div>
                        <div class="mt-3">
                            <label class="fw-600">Eyebrow</label>
                            <asp:TextBox ID="txt_hero_eyebrow" runat="server" data-role="input"></asp:TextBox>
                        </div>
                        <div class="mt-3">
                            <label class="fw-600">Tieu de hero</label>
                            <asp:TextBox ID="txt_hero_title" runat="server" TextMode="MultiLine" Rows="3" data-role="textarea"></asp:TextBox>
                        </div>
                        <div class="mt-3">
                            <label class="fw-600">Mo ta hero</label>
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
                                <label class="fw-600">Nhan KPI</label>
                                <asp:TextBox ID="txt_metric_service" runat="server" data-role="input" placeholder="Metric service"></asp:TextBox>
                                <asp:TextBox ID="txt_metric_product" runat="server" data-role="input" placeholder="Metric product" CssClass="mt-2"></asp:TextBox>
                                <asp:TextBox ID="txt_metric_article" runat="server" data-role="input" placeholder="Metric article" CssClass="mt-2"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mt-4">
                            <div class="cell-md-6">
                                <label class="fw-600">Khoi top san pham</label>
                                <asp:TextBox ID="txt_stage_primary_title" runat="server" data-role="input" placeholder="Tieu de"></asp:TextBox>
                                <asp:TextBox ID="txt_stage_primary_desc" runat="server" TextMode="MultiLine" Rows="3" data-role="textarea" CssClass="mt-2"></asp:TextBox>
                            </div>
                            <div class="cell-md-6">
                                <label class="fw-600">Khoi danh sach san pham cong khai</label>
                                <asp:TextBox ID="txt_stage_secondary_title" runat="server" data-role="input" placeholder="Tieu de"></asp:TextBox>
                                <asp:TextBox ID="txt_stage_secondary_desc" runat="server" TextMode="MultiLine" Rows="3" data-role="textarea" CssClass="mt-2"></asp:TextBox>
                            </div>
                        </div>

                        <hr class="mt-6 mb-4" />
                        <div class="h4 mb-3">Footer</div>
                        <div class="mt-3">
                            <label class="fw-600">Mo ta footer</label>
                            <asp:TextBox ID="txt_footer_description" runat="server" TextMode="MultiLine" Rows="4" data-role="textarea"></asp:TextBox>
                        </div>
                        <div class="row mt-4">
                            <div class="cell-md-3"><asp:TextBox ID="txt_footer_chip1" runat="server" data-role="input" placeholder="Chip 1"></asp:TextBox></div>
                            <div class="cell-md-3"><asp:TextBox ID="txt_footer_chip2" runat="server" data-role="input" placeholder="Chip 2"></asp:TextBox></div>
                            <div class="cell-md-3"><asp:TextBox ID="txt_footer_chip3" runat="server" data-role="input" placeholder="Chip 3"></asp:TextBox></div>
                            <div class="cell-md-3"><asp:TextBox ID="txt_footer_chip4" runat="server" data-role="input" placeholder="Chip 4"></asp:TextBox></div>
                        </div>
                        <div class="row mt-4">
                            <div class="cell-md-4"><asp:TextBox ID="txt_footer_nav_title" runat="server" data-role="input" placeholder="Tieu de dieu huong"></asp:TextBox></div>
                            <div class="cell-md-4"><asp:TextBox ID="txt_footer_category_title" runat="server" data-role="input" placeholder="Tieu de danh muc"></asp:TextBox></div>
                            <div class="cell-md-4"><asp:TextBox ID="txt_footer_contact_title" runat="server" data-role="input" placeholder="Tieu de lien he"></asp:TextBox></div>
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
                            <asp:Button ID="ButtonSave" runat="server" Text="CAP NHAT STOREFRONT" CssClass="button success" OnClick="ButtonSave_Click" />
                        </div>
                    </asp:Panel>
                </div>
            </div>

            <div class="cell-lg-4">
                <div class="p-4 bg-white border-radius-8 mb-4">
                    <div class="h4 mb-3">Cac block storefront /shop</div>
                    <div class="fg-gray mb-3">Thu tu, text, CTA va trang thai hien thi cua tung block /shop duoc sua tai man edit.</div>
                    <div class="table-component compact">
                        <table class="table striped table-border cell-border w-100">
                            <thead>
                                <tr>
                                    <th>Rank</th>
                                    <th>Block</th>
                                    <th>Source</th>
                                    <th>Hien thi</th>
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
                                            <td><%# ((bool?)Eval("is_visible") ?? false) ? "ON" : "OFF" %></td>
                                            <td><a class="button small primary outline" href='/gianhang/admin/cau-hinh-storefront/edit-section.aspx?id=<%# Eval("id") %>'>Sua</a></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </div>

                <div class="p-4 bg-white border-radius-8">
                    <div class="h4 mb-3">Nguon hinh anh /shop</div>
                    <div class="fg-gray mb-3">/shop khong hardcode hinh anh. Anh dang lay tu cac module admin sau:</div>
                    <div class="d-flex flex-column gap-2">
                        <a class="button info outline mb-2" href="/gianhang/admin/cau-hinh-chung/cap-nhat-thong-tin.aspx">Logo / thuong hieu</a>
                        <a class="button info outline mb-2" href="/gianhang/admin/quan-ly-module/slide-anh/default.aspx">Slider hero</a>
                        <a class="button info outline mb-2" href="/gianhang/admin/quan-ly-menu/Default.aspx">Anh danh muc / menu</a>
                        <a class="button info outline mb-2" href="/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=dv">Anh dich vu</a>
                        <a class="button info outline" href="/gianhang/admin/quan-ly-bai-viet/Default.aspx?pl=sp">Anh san pham</a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>
