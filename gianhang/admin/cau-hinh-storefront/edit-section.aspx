<%@ Page Title="Sua block storefront /shop" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="edit-section.aspx.cs" Inherits="gianhang_admin_cau_hinh_storefront_edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">
    <div id="main-content" class="mb-10">
        <div class="row">
            <div class="cell-lg-8">
                <div class="p-4 bg-white border-radius-8">
                    <div class="d-flex flex-justify-between flex-align-center mb-3">
                        <div>
                            <div class="h3 m-0">Sua block storefront /shop</div>
                            <div class="fg-gray">Dieu khien text, CTA, source va thu tu hien thi cua tung block tren storefront /shop.</div>
                        </div>
                        <a class="button default" href="/gianhang/admin/cau-hinh-storefront/default.aspx">Quay lai</a>
                    </div>

                    <asp:Panel ID="Panel1" runat="server" DefaultButton="ButtonSave">
                        <div class="row">
                            <div class="cell-md-6">
                                <label class="fw-600">Section key</label>
                                <asp:TextBox ID="txt_section_key" runat="server" data-role="input" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="cell-md-6">
                                <label class="fw-600">Source type</label>
                                <asp:DropDownList ID="ddl_source_type" runat="server" CssClass="input-large">
                                    <asp:ListItem Value="service-groups">service-groups</asp:ListItem>
                                    <asp:ListItem Value="featured-services">featured-services</asp:ListItem>
                                    <asp:ListItem Value="featured-products">featured-products</asp:ListItem>
                                    <asp:ListItem Value="featured-articles">featured-articles</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="mt-3">
                            <label class="fw-600">Subtitle / label</label>
                            <asp:TextBox ID="txt_subtitle" runat="server" data-role="input"></asp:TextBox>
                        </div>
                        <div class="mt-3">
                            <label class="fw-600">Title</label>
                            <asp:TextBox ID="txt_title" runat="server" TextMode="MultiLine" Rows="3" data-role="textarea"></asp:TextBox>
                        </div>
                        <div class="mt-3">
                            <label class="fw-600">Description</label>
                            <asp:TextBox ID="txt_description" runat="server" TextMode="MultiLine" Rows="4" data-role="textarea"></asp:TextBox>
                        </div>

                        <div class="row mt-4">
                            <div class="cell-md-4">
                                <label class="fw-600">Section badge</label>
                                <asp:TextBox ID="txt_badge" runat="server" data-role="input"></asp:TextBox>
                            </div>
                            <div class="cell-md-4">
                                <label class="fw-600">Item label</label>
                                <asp:TextBox ID="txt_item_label" runat="server" data-role="input"></asp:TextBox>
                            </div>
                            <div class="cell-md-4">
                                <label class="fw-600">Item limit</label>
                                <asp:TextBox ID="txt_item_limit" runat="server" data-role="input"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mt-4">
                            <div class="cell-md-6">
                                <label class="fw-600">CTA chinh</label>
                                <asp:TextBox ID="txt_cta_text" runat="server" data-role="input" placeholder="Text"></asp:TextBox>
                                <asp:TextBox ID="txt_cta_url" runat="server" data-role="input" placeholder="URL (optional)" CssClass="mt-2"></asp:TextBox>
                            </div>
                            <div class="cell-md-6">
                                <label class="fw-600">CTA phu</label>
                                <asp:TextBox ID="txt_secondary_cta_text" runat="server" data-role="input" placeholder="Text"></asp:TextBox>
                                <asp:TextBox ID="txt_secondary_cta_url" runat="server" data-role="input" placeholder="URL (optional)" CssClass="mt-2"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mt-4">
                            <div class="cell-md-4">
                                <label class="fw-600">Source value</label>
                                <asp:TextBox ID="txt_source_value" runat="server" data-role="input" placeholder="Vi du: 551 / 550 / 577"></asp:TextBox>
                            </div>
                            <div class="cell-md-4">
                                <label class="fw-600">Rank</label>
                                <asp:TextBox ID="txt_rank" runat="server" data-role="input"></asp:TextBox>
                            </div>
                            <div class="cell-md-4 d-flex flex-align-end">
                                <asp:CheckBox ID="chk_visible" runat="server" Text="Hien block nay" />
                            </div>
                        </div>

                        <div class="mt-4">
                            <label class="fw-600">Style variant / extra json</label>
                            <asp:TextBox ID="txt_style_variant" runat="server" data-role="input" placeholder="Style variant"></asp:TextBox>
                            <asp:TextBox ID="txt_extra_json" runat="server" TextMode="MultiLine" Rows="4" data-role="textarea" CssClass="mt-2" placeholder="Du lieu mo rong"></asp:TextBox>
                        </div>

                        <div class="text-center mt-8">
                            <asp:Button ID="ButtonSave" runat="server" Text="CAP NHAT BLOCK" CssClass="button success" OnClick="ButtonSave_Click" />
                        </div>
                    </asp:Panel>
                </div>
            </div>

            <div class="cell-lg-4">
                <div class="p-4 bg-white border-radius-8">
                    <div class="h4 mb-3">Huong dan source</div>
                    <ul class="simple-list large-bullet">
                        <li><code>service-groups</code>: block danh muc dich vu</li>
                        <li><code>featured-services</code>: block dich vu chu luc</li>
                        <li><code>featured-products</code>: block san pham chu luc</li>
                        <li><code>featured-articles</code>: block bai viet moi</li>
                    </ul>
                    <div class="mt-4 fg-gray">Source value thuong la ID menu goc. Vi du: 551 = dich vu, 550 = san pham, 577 = bai viet.</div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="Server">
    <%=notifi %>
</asp:Content>
