<%@ Page Title="Chỉnh sửa thông tin" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="edit-info.aspx.cs" Inherits="home_edit_info" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server">
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>
    <style>
        .wrap { max-width: 1200px; }
        .overlay-loading{
            position:fixed; inset:0; background:rgba(0,0,0,.6);
            z-index:99999; display:flex; align-items:center; justify-content:center;
        }
        .overlay-panel{
            position:fixed; inset:0; z-index:2000;
            background:rgba(0,0,0,.55);
            overflow:auto; padding:16px 12px;
        }
        .overlay-dialog{ max-width: 720px; margin:0 auto; }
        .avatar-100{ width:100px; height:100px; border-radius:999px; object-fit:cover; }
        .img-100{ width:100px; height:100px; object-fit:cover; border-radius:10px; border:1px solid rgba(98,105,118,.18); background:#fff; }
        .card-soft{ border:1px solid rgba(98,105,118,.18); }
        .muted{ color:var(--tblr-muted); }
        .social-preset-grid{ display:grid; grid-template-columns:repeat(auto-fit,minmax(120px,1fr)); gap:10px; }
        .social-preset-btn{
            border:1px solid rgba(98,105,118,.22);
            background:#fff;
            border-radius:14px;
            min-height:46px;
            padding:10px 12px;
            display:flex;
            align-items:center;
            justify-content:center;
            gap:8px;
            font-weight:700;
            color:#1f2937;
            transition:.2s ease;
        }
        .social-preset-btn:hover,
        .social-preset-btn.active{
            border-color:#16a34a;
            background:#f0fdf4;
            color:#166534;
            box-shadow:0 6px 18px rgba(22,163,74,.12);
        }
        .social-help-box{
            border:1px dashed rgba(22,163,74,.28);
            background:#f7fff9;
            border-radius:12px;
            padding:10px 12px;
            font-size:13px;
            color:#4b5563;
        }
        .social-link-card{
            border:1px solid rgba(98,105,118,.18);
            border-radius:16px;
            padding:12px;
            margin-bottom:10px;
            background:#fff;
            display:flex;
            align-items:center;
            justify-content:space-between;
            gap:12px;
        }
        .social-link-main{
            min-width:0;
            flex:1 1 auto;
            display:flex;
            align-items:center;
            gap:12px;
        }
        .social-link-order-badge{
            width:34px;
            height:34px;
            border-radius:12px;
            background:#f8fafc;
            border:1px solid rgba(98,105,118,.16);
            color:#334155;
            display:inline-flex;
            align-items:center;
            justify-content:center;
            font-weight:800;
            flex:0 0 34px;
        }
        .social-link-anchor{
            min-width:0;
            flex:1 1 auto;
            color:inherit;
            text-decoration:none;
        }
        .social-link-anchor:hover{ color:inherit; text-decoration:none; }
        .social-link-icon{
            width:56px;
            height:56px;
            object-fit:cover;
            border-radius:14px;
            border:1px solid rgba(98,105,118,.18);
            background:#fff;
            flex:0 0 56px;
        }
        .social-link-body{
            min-width:0;
        }
        .social-link-title{
            font-weight:800;
            color:#1f2937;
            line-height:1.2;
        }
        .social-link-url{
            margin-top:4px;
            color:#6b7280;
            font-style:italic;
            white-space:nowrap;
            overflow:hidden;
            text-overflow:ellipsis;
        }
        .social-link-actions{
            display:grid;
            grid-template-columns:repeat(2, 38px);
            gap:8px;
            flex:0 0 auto;
        }
        .social-link-action{
            width:38px;
            height:38px;
            border-radius:12px;
            display:inline-flex;
            align-items:center;
            justify-content:center;
            text-decoration:none;
            border:1px solid transparent;
            font-size:18px;
        }
        .social-link-action-edit{
            background:#ecfdf3;
            color:#16a34a;
            border-color:#bbf7d0;
        }
        .social-link-action-move{
            background:#eff6ff;
            color:#2563eb;
            border-color:#bfdbfe;
        }
        .social-link-action-delete{
            background:#fff1f2;
            color:#e11d48;
            border-color:#fecdd3;
        }
        .social-link-sort-note{
            margin-bottom:10px;
            color:#6b7280;
            font-size:13px;
        }
        @media (max-width: 767.98px){
            .social-link-card{
                align-items:flex-start;
            }
            .social-link-main{
                align-items:flex-start;
            }
            .social-link-order-badge{
                width:30px;
                height:30px;
                border-radius:10px;
                flex-basis:30px;
                font-size:13px;
            }
            .social-link-icon{
                width:52px;
                height:52px;
                flex-basis:52px;
            }
            .social-link-action{
                width:34px;
                height:34px;
                border-radius:10px;
                font-size:16px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <asp:TextBox ID="txtKieu" runat="server" style="visibility:hidden; height:0; width:0;" />

    <!-- ====== PANEL ADD/EDIT LINK (overlay) ====== -->
    <asp:UpdatePanel ID="up_themlink" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_themlink" runat="server" Visible="false" DefaultButton="btnLuuLink">
                <div class="overlay-panel">
                    <div class="overlay-dialog">
                        <div class="card shadow-sm">
                            <div class="card-header d-flex align-items-center justify-content-between">
                                <div class="card-title fw-bold">Thêm/sửa link mạng xã hội</div>
                                <a href="#" id="btnCloseLink" runat="server" onserverclick="btnDong_Click" title="Đóng"
                                   class="btn btn-ghost-danger btn-icon">
                                    <i class="ti ti-x"></i>
                                </a>
                            </div>

                            <div class="card-body">
                                <asp:HiddenField ID="hfIdLink" runat="server" />
                                <asp:HiddenField ID="hfSocialPreset" runat="server" />

                                <div class="mb-3">
                                    <label class="form-label">Ứng dụng phổ biến</label>
                                    <div class="social-preset-grid">
                                        <button type="button" class="social-preset-btn" data-preset="facebook" onclick="selectSocialPreset('facebook')">
                                            <i class="ti ti-brand-facebook"></i><span>Facebook</span>
                                        </button>
                                        <button type="button" class="social-preset-btn" data-preset="zalo" onclick="selectSocialPreset('zalo')">
                                            <i class="ti ti-message-circle"></i><span>Zalo</span>
                                        </button>
                                        <button type="button" class="social-preset-btn" data-preset="tiktok" onclick="selectSocialPreset('tiktok')">
                                            <i class="ti ti-brand-tiktok"></i><span>TikTok</span>
                                        </button>
                                        <button type="button" class="social-preset-btn" data-preset="youtube" onclick="selectSocialPreset('youtube')">
                                            <i class="ti ti-brand-youtube"></i><span>YouTube</span>
                                        </button>
                                        <button type="button" class="social-preset-btn" data-preset="instagram" onclick="selectSocialPreset('instagram')">
                                            <i class="ti ti-brand-instagram"></i><span>Instagram</span>
                                        </button>
                                        <button type="button" class="social-preset-btn" data-preset="telegram" onclick="selectSocialPreset('telegram')">
                                            <i class="ti ti-brand-telegram"></i><span>Telegram</span>
                                        </button>
                                        <button type="button" class="social-preset-btn" data-preset="shopee" onclick="selectSocialPreset('shopee')">
                                            <i class="ti ti-shopping-bag"></i><span>Shopee</span>
                                        </button>
                                        <button type="button" class="social-preset-btn" data-preset="website" onclick="selectSocialPreset('website')">
                                            <i class="ti ti-world-www"></i><span>Website</span>
                                        </button>
                                        <button type="button" class="social-preset-btn" data-preset="" onclick="selectSocialPreset('')">
                                            <i class="ti ti-world"></i><span>Link khác</span>
                                        </button>
                                    </div>
                                    <div id="socialPresetHelp" class="social-help-box mt-2">
                                        Chọn ứng dụng để hệ thống tự gợi ý mẫu link. Bạn chỉ cần dán username, số điện thoại hoặc link đầy đủ.
                                    </div>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Tên</label>
                                    <asp:TextBox ID="txtTen" runat="server" CssClass="form-control" placeholder="Tên" />
                                    <div class="form-hint">Có thể để trống, hệ thống sẽ tự lấy tên hồ sơ hiện tại để giảm thao tác.</div>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Link</label>
                                    <asp:TextBox ID="txtLink" runat="server" CssClass="form-control" placeholder="Ví dụ: https://www.facebook.com" />
                                    <asp:Label ID="lblLinkError" runat="server" CssClass="text-danger small" Visible="false"></asp:Label>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Icon</label>
                                    <input type="file" id="fileInput3" onchange="uploadFile3()" class="form-control" />
                                    <div id="message3" class="text-danger small mt-1"></div>
                                    <div id="uploadedFilePath3" class="mt-2"></div>

                                    <div style="display:none">
                                        <asp:TextBox ID="TxtIcon" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>

                                    <div class="mt-2">
                                        <asp:Label ID="Label5" runat="server" Text=""></asp:Label>
                                    </div>

                                    <div id="uploadedFileContainer" runat="server" visible="false" class="mt-3">
                                        <div class="small text-muted mb-1">Ảnh hiện tại:</div>
                                        <asp:Image ID="previewImage" runat="server" CssClass="img-100" />
                                        <div class="mt-2">
                                            <asp:Button ID="btnRemoveImage" runat="server" Text="Xoá ảnh"
                                                CssClass="btn btn-outline-danger btn-sm"
                                                OnClick="removeUploadedImage" />
                                        </div>
                                    </div>
                                </div>

                            </div>

                            <div class="card-footer d-flex align-items-center justify-content-between">
                                <a href="#" id="btnCloseLink2" runat="server" onserverclick="btnDong_Click"
                                   class="btn btn-link text-decoration-none">Hủy</a>
                                <asp:Button ID="btnLuuLink" runat="server" Text="Lưu" CssClass="btn btn-primary px-4" OnClientClick="return prepareSocialLinkBeforeSubmit();" OnClick="btnLuuLink_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- ====== MAIN ====== -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server" DefaultButton="but_capnhat">

                <div class="container-xl wrap py-4">
                    <div class="row g-3">

                        <!-- left profile card đã ẩn theo yêu cầu để gọn giao diện -->

                        <!-- right edit tabs -->
                        <div class="col-lg-12">
                            <div class="card card-soft shadow-sm">
                                <div class="card-body">

                                    <!-- Tabler/Bootstrap tabs -->
                                    <ul class="nav nav-tabs" data-bs-toggle="tabs" role="tablist" id="tab-kieu">
                                        <li class="nav-item" role="presentation">
                                            <a class="<%= GetTabClass(false) %>" href="#target_cn" data-bs-toggle="tab" role="tab" aria-selected="<%= GetAriaSelected(false) %>">
                                                <i class="ti ti-user me-1"></i>Cá nhân
                                            </a>
                                        </li>

                                        <!-- ✅ TAB CỬA HÀNG: CHỈ HIỆN KHI KHÔNG BỊ KHÓA -->
                                        <asp:PlaceHolder ID="phTabCuaHang" runat="server" Visible="true">
                                            <li class="nav-item" role="presentation">
                                                <a class="<%= GetTabClass(true) %>" href="#target_ch" data-bs-toggle="tab" role="tab" aria-selected="<%= GetAriaSelected(true) %>">
                                                    <i class="ti ti-building-store me-1"></i>Cửa hàng
                                                </a>
                                            </li>
                                        </asp:PlaceHolder>
                                    </ul>

                                    <div class="tab-content pt-3">

                                        <!-- ===== Cá nhân ===== -->
                                        <div class="<%= GetPaneClass(false) %>" id="target_cn" role="tabpanel">
                                            <div class="row g-3">

                                                <div class="col-12">
                                                    <div class="fw-bold"><small>Ảnh đại diện</small></div>
                                                    <input type="file" id="fileInput" onchange="uploadFile()" class="form-control" />
                                                    <div id="message" runat="server" class="text-danger small mt-1"></div>
                                                    <div id="uploadedFilePath" class="mt-2"></div>
                                                    <div style="display:none">
                                                        <asp:TextBox ID="txt_link_fileupload" runat="server"></asp:TextBox>
                                                    </div>
                                                    <div class="mt-2">
                                                        <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                                    </div>
                                                    <div class="mt-2">
                                                        <asp:Button ID="Button2" runat="server" Text="Xóa ảnh cũ"
                                                            CssClass="btn btn-outline-danger btn-sm"
                                                            Visible="false" OnClick="Button2_Click" />
                                                    </div>
                                                </div>

                                                <div class="col-lg-6">
                                                    <label class="form-label">Họ tên</label>
                                                    <asp:TextBox ID="txt_hoten" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>

                                                <div class="col-lg-6">
                                                    <label class="form-label">Điện thoại</label>
                                                    <asp:TextBox ID="txt_sdt" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>

                                                <div class="col-lg-6">
                                                    <label class="form-label">Email</label>
                                                    <asp:TextBox ID="TextBox9" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>

                                                <div class="col-lg-6"></div>

                                                <div class="col-12">
                                                    <label class="form-label">Tỉnh/Thành - Quận/Huyện - Phường/Xã</label>
                                                    <div class="row g-2">
                                                        <div class="col-md-4">
                                                            <select id="profile_tinh" class="form-select"></select>
                                                        </div>
                                                        <div class="col-md-4">
                                                            <select id="profile_quan" class="form-select"></select>
                                                        </div>
                                                        <div class="col-md-4">
                                                            <select id="profile_phuong" class="form-select"></select>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-12">
                                                    <label class="form-label">Địa chỉ chi tiết</label>
                                                    <asp:TextBox ID="txt_diachi_chitiet" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control"></asp:TextBox>
                                                </div>

                                                <asp:HiddenField ID="hf_profile_tinh" runat="server" />
                                                <asp:HiddenField ID="hf_profile_quan" runat="server" />
                                                <asp:HiddenField ID="hf_profile_phuong" runat="server" />
                                                <asp:HiddenField ID="hf_profile_raw" runat="server" />
                                                <asp:TextBox ID="txt_diachi" runat="server" CssClass="form-control d-none"></asp:TextBox>

                                                <div class="col-12">
                                                    <label class="form-label">Giới thiệu</label>
                                                    <asp:TextBox ID="txt_gioithieu" runat="server" TextMode="MultiLine" Rows="3"
                                                        CssClass="form-control" placeholder="Tối đa 60 ký tự"></asp:TextBox>
                                                </div>

                                                <div class="col-12 d-none">
                                                    <div class="card card-soft">
                                                        <div class="card-body">
                                                            <div class="fw-bold mb-1">Giao diện hồ sơ</div>
                                                            <div class="text-muted small mb-3">Chọn mẫu và bật/tắt khối hiển thị. Mặc định đã tối ưu sẵn.</div>

                                                            <div class="row g-3">
                                                                <div class="col-lg-4">
                                                                    <label class="form-label">Template</label>
                                                                    <asp:DropDownList ID="ddl_profile_template" runat="server" CssClass="form-select">
                                                                        <asp:ListItem Value="classic" Text="Classic Card"></asp:ListItem>
                                                                        <asp:ListItem Value="pro" Text="Professional Split"></asp:ListItem>
                                                                        <asp:ListItem Value="creator" Text="Creator / Shop"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                                <div class="col-lg-4">
                                                                    <label class="form-label">Màu chủ đạo</label>
                                                                    <input id="input_profile_accent" type="color" class="form-control form-control-color" value="#22c55e" />
                                                                    <asp:HiddenField ID="hf_profile_accent" runat="server" Value="#22c55e" />
                                                                </div>
                                                                <div class="col-lg-4">
                                                                    <label class="form-label">Hiển thị CTA</label>
                                                                    <div class="form-check form-switch mt-2">
                                                                        <input id="chk_profile_contact" runat="server" type="checkbox" class="form-check-input" />
                                                                        <label class="form-check-label">Nút Liên hệ</label>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-4">
                                                                    <div class="form-check form-switch mt-2">
                                                                        <input id="chk_profile_social" runat="server" type="checkbox" class="form-check-input" />
                                                                        <label class="form-check-label">Liên kết mạng xã hội</label>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-4">
                                                                    <div class="form-check form-switch mt-2">
                                                                        <input id="chk_profile_reviews" runat="server" type="checkbox" class="form-check-input" />
                                                                        <label class="form-check-label">Đánh giá người dùng</label>
                                                                    </div>
                                                                </div>

                                                                <asp:PlaceHolder ID="phProfileShopToggle" runat="server" Visible="false">
                                                                    <div class="col-lg-4">
                                                                        <div class="form-check form-switch mt-2">
                                                                            <input id="chk_profile_shop" runat="server" type="checkbox" class="form-check-input" />
                                                                            <label class="form-check-label">Giới thiệu cửa hàng</label>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-lg-4">
                                                                        <div class="form-check form-switch mt-2">
                                                                            <input id="chk_profile_products" runat="server" type="checkbox" class="form-check-input" />
                                                                            <label class="form-check-label">Sản phẩm đang bán</label>
                                                                        </div>
                                                                    </div>
                                                                </asp:PlaceHolder>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <!-- social links cá nhân -->
                                                <div class="col-12">
                                                    <div class="social-link-sort-note">Dùng mũi tên để sắp xếp thứ tự hiển thị link cá nhân.</div>
                                                    <asp:Repeater ID="rptMangXaHoiCN" runat="server" OnItemCommand="rptMangXaHoi_ItemCommand">
                                                        <ItemTemplate>
                                                            <div class="social-link-card">
                                                                <div class="social-link-main">
                                                                    <span class="social-link-order-badge"><%# Container.ItemIndex + 1 %></span>
                                                                    <asp:Image ID="imgIcon" runat="server"
                                                                        ImageUrl='<%# ResolveSocialIcon(Eval("Icon"), Eval("Link")) %>'
                                                                        CssClass="social-link-icon"
                                                                        Visible='<%# ShouldShowSocialIcon(Eval("Icon"), Eval("Link")) %>' />
                                                                    <a href='<%# Eval("Link") %>' target="_blank" class="social-link-anchor">
                                                                        <div class="social-link-body" style='<%# GetSocialIconMarginStyle(Eval("Icon"), Eval("Link")) %>'>
                                                                            <div class="social-link-title"><%# Eval("Ten") %></div>
                                                                            <div class="social-link-url"><%# Eval("Link") %></div>
                                                                        </div>
                                                                    </a>
                                                                </div>

                                                                <div class="social-link-actions">
                                                                    <asp:LinkButton ID="btnLen" runat="server" ToolTip="Đưa lên"
                                                                        CssClass="social-link-action social-link-action-move"
                                                                        CommandName="MoveUpItem"
                                                                        CausesValidation="false"
                                                                        CommandArgument='<%# Eval("ID") %>'>
                                                                        <i class="ti ti-arrow-up"></i>
                                                                        <span class="visually-hidden">Đưa lên</span>
                                                                    </asp:LinkButton>
                                                                    <asp:LinkButton ID="btnXuong" runat="server" ToolTip="Đưa xuống"
                                                                        CssClass="social-link-action social-link-action-move"
                                                                        CommandName="MoveDownItem"
                                                                        CausesValidation="false"
                                                                        CommandArgument='<%# Eval("ID") %>'>
                                                                        <i class="ti ti-arrow-down"></i>
                                                                        <span class="visually-hidden">Đưa xuống</span>
                                                                    </asp:LinkButton>
                                                                    <asp:LinkButton ID="btnSua" runat="server" ToolTip="Sửa"
                                                                        CssClass="social-link-action social-link-action-edit"
                                                                        CommandName="EditItem"
                                                                        CausesValidation="false"
                                                                        CommandArgument='<%# Eval("ID") %>'>
                                                                        <i class="ti ti-pencil"></i>
                                                                        <span class="visually-hidden">Sửa</span>
                                                                    </asp:LinkButton>
                                                                    <asp:LinkButton ID="btnXoa" runat="server" ToolTip="Xóa"
                                                                        CssClass="social-link-action social-link-action-delete"
                                                                        CommandName="DeleteItem"
                                                                        CausesValidation="false"
                                                                        CommandArgument='<%# Eval("ID") %>'>
                                                                        <i class="ti ti-x"></i>
                                                                        <span class="visually-hidden">Xóa</span>
                                                                    </asp:LinkButton>
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>

                                            </div>
                                        </div>

                                        <!-- ✅ THÔNG BÁO KHI SHOP BỊ KHÓA -->
                                        <asp:PlaceHolder ID="phShopLockedNote" runat="server" Visible="false">
                                            <div class="<%= GetPaneClass(true) %>" id="target_ch" role="tabpanel">
                                                <div class="alert alert-warning">
                                                    Tài khoản hiện tại <b>không thuộc phạm vi gian hàng đối tác</b> nên không thể chỉnh sửa thông tin cửa hàng.
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>

                                        <!-- ===== Cửa hàng (chỉ hiển thị khi không bị khóa) ===== -->
                                        <asp:PlaceHolder ID="phShopEdit" runat="server" Visible="true">
                                            <div class="<%= GetPaneClass(true) %>" id="target_ch" role="tabpanel">
                                                <div class="row g-3">

                                                    <div class="col-12">
                                                        <div class="alert alert-info d-flex flex-wrap align-items-center justify-content-between gap-2">
                                                            <div>
                                                                Hoàn thiện hồ sơ shop:
                                                                <span class="fw-semibold"><asp:Label ID="lb_shop_completion" runat="server" Text="0%"></asp:Label></span>
                                                                <div class="small text-muted">
                                                                    <asp:Label ID="lb_shop_completion_note" runat="server" Text=""></asp:Label>
                                                                </div>
                                                            </div>
                                                            <asp:HyperLink ID="hl_shop_public" runat="server" CssClass="btn btn-outline-primary btn-sm" Text="Xem trang công khai"></asp:HyperLink>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <label class="form-label">Logo</label>
                                                        <input type="file" id="fileInput1" onchange="uploadFile1()" class="form-control" />
                                                        <div id="message1" runat="server" class="text-danger small mt-1"></div>
                                                        <div id="uploadedFilePath1" class="mt-2"></div>
                                                        <div style="display:none">
                                                            <asp:TextBox ID="txt_link_fileupload1" runat="server"></asp:TextBox>
                                                        </div>
                                                        <div class="mt-2"><asp:Label ID="Label3" runat="server" Text=""></asp:Label></div>
                                                        <div class="mt-2">
                                                            <asp:Button ID="Button1" runat="server" Text="Xóa ảnh cũ"
                                                                CssClass="btn btn-outline-danger btn-sm"
                                                                Visible="false" OnClick="Button1_Click" />
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <label class="form-label">Ảnh bìa</label>
                                                        <input type="file" id="fileInput2" onchange="uploadFile2()" class="form-control" />
                                                        <div id="message2" runat="server" class="text-danger small mt-1"></div>
                                                        <div id="uploadedFilePath2" class="mt-2"></div>
                                                        <div style="display:none">
                                                            <asp:TextBox ID="txt_link_fileupload2" runat="server"></asp:TextBox>
                                                        </div>
                                                        <div class="mt-2"><asp:Label ID="Label4" runat="server" Text=""></asp:Label></div>
                                                        <div class="mt-2">
                                                            <asp:Button ID="Button3" runat="server" Text="Xóa ảnh cũ"
                                                                CssClass="btn btn-outline-danger btn-sm"
                                                                Visible="false" OnClick="Button3_Click" />
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <label class="form-label">Tên cửa hàng</label>
                                                        <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <label class="form-label">Điện thoại</label>
                                                        <asp:TextBox ID="TextBox3" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <label class="form-label">Email</label>
                                                        <asp:TextBox ID="TextBox10" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <label class="form-label">Số Zalo</label>
                                                        <asp:TextBox ID="TextBox11" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <div class="col-12">
                                                        <label class="form-label">Mô tả ngắn</label>
                                                        <asp:TextBox ID="TextBox5" runat="server" TextMode="MultiLine" Rows="3"
                                                            CssClass="form-control" placeholder="Tối đa 60 ký tự"></asp:TextBox>
                                                    </div>

                                                    <div class="col-12">
                                                        <label class="form-label">Tỉnh/Thành - Quận/Huyện - Phường/Xã</label>
                                                        <div class="row g-2">
                                                            <div class="col-md-4">
                                                                <select id="shop_tinh" class="form-select"></select>
                                                            </div>
                                                            <div class="col-md-4">
                                                                <select id="shop_quan" class="form-select"></select>
                                                            </div>
                                                            <div class="col-md-4">
                                                                <select id="shop_phuong" class="form-select"></select>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-12">
                                                        <label class="form-label">Địa chỉ chi tiết</label>
                                                        <asp:TextBox ID="txt_diachi_shop_chitiet" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control"></asp:TextBox>
                                                    </div>

                                                    <asp:HiddenField ID="hf_shop_tinh" runat="server" />
                                                    <asp:HiddenField ID="hf_shop_quan" runat="server" />
                                                    <asp:HiddenField ID="hf_shop_phuong" runat="server" />
                                                    <asp:HiddenField ID="hf_shop_raw" runat="server" />
                                                    <asp:TextBox ID="TextBox4" runat="server" CssClass="form-control d-none"></asp:TextBox>

                                                    <!-- social links cửa hàng -->
                                                    <div class="col-12">
                                                        <div class="social-link-sort-note">Dùng mũi tên để sắp xếp thứ tự hiển thị link cửa hàng.</div>
                                                        <asp:Repeater ID="rptMangXaHoiCH" runat="server" OnItemCommand="rptMangXaHoi_ItemCommand">
                                                            <ItemTemplate>
                                                                <div class="social-link-card">
                                                                    <div class="social-link-main">
                                                                        <span class="social-link-order-badge"><%# Container.ItemIndex + 1 %></span>
                                                                        <asp:Image ID="imgIcon" runat="server"
                                                                            ImageUrl='<%# ResolveSocialIcon(Eval("Icon"), Eval("Link")) %>'
                                                                            CssClass="social-link-icon"
                                                                            Visible='<%# ShouldShowSocialIcon(Eval("Icon"), Eval("Link")) %>' />

                                                                        <a href='<%# Eval("Link") %>' target="_blank" class="social-link-anchor">
                                                                            <div class="social-link-body" style='<%# GetSocialIconMarginStyle(Eval("Icon"), Eval("Link")) %>'>
                                                                                <div class="social-link-title"><%# Eval("Ten") %></div>
                                                                                <div class="social-link-url"><%# Eval("Link") %></div>
                                                                            </div>
                                                                        </a>
                                                                    </div>

                                                                    <div class="social-link-actions">
                                                                        <asp:LinkButton ID="btnLen" runat="server" ToolTip="Đưa lên"
                                                                            CssClass="social-link-action social-link-action-move"
                                                                            CommandName="MoveUpItem"
                                                                            CausesValidation="false"
                                                                            CommandArgument='<%# Eval("ID") %>'>
                                                                            <i class="ti ti-arrow-up"></i>
                                                                            <span class="visually-hidden">Đưa lên</span>
                                                                        </asp:LinkButton>
                                                                        <asp:LinkButton ID="btnXuong" runat="server" ToolTip="Đưa xuống"
                                                                            CssClass="social-link-action social-link-action-move"
                                                                            CommandName="MoveDownItem"
                                                                            CausesValidation="false"
                                                                            CommandArgument='<%# Eval("ID") %>'>
                                                                            <i class="ti ti-arrow-down"></i>
                                                                            <span class="visually-hidden">Đưa xuống</span>
                                                                        </asp:LinkButton>
                                                                        <asp:LinkButton ID="btnSua" runat="server" ToolTip="Sửa"
                                                                            CssClass="social-link-action social-link-action-edit"
                                                                            CommandName="EditItem"
                                                                            CausesValidation="false"
                                                                            CommandArgument='<%# Eval("ID") %>'>
                                                                            <i class="ti ti-pencil"></i>
                                                                            <span class="visually-hidden">Sửa</span>
                                                                        </asp:LinkButton>
                                                                        <asp:LinkButton ID="btnXoa" runat="server" ToolTip="Xóa"
                                                                            CssClass="social-link-action social-link-action-delete"
                                                                            CommandName="DeleteItem"
                                                                            CausesValidation="false"
                                                                            CommandArgument='<%# Eval("ID") %>'>
                                                                            <i class="ti ti-x"></i>
                                                                            <span class="visually-hidden">Xóa</span>
                                                                        </asp:LinkButton>
                                                                    </div>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </div>

                                                </div>
                                            </div>
                                        </asp:PlaceHolder>

                                    </div>

                                    <div class="d-flex justify-content-end gap-2 mt-4">
                                        <asp:Button ID="but_themlink" runat="server" Text="+ Thêm link"
                                            CssClass="btn btn-outline-primary"
                                            OnClick="but_themlink_Click" />
                                        <asp:Button ID="but_capnhat" OnClick="but_capnhat_Click" runat="server" Text="Cập nhật"
                                            CssClass="btn btn-primary px-4" />
                                    </div>

                                </div>
                            </div>
                        </div>

                    </div>
                </div>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="overlay-loading">
                <div class="spinner-border" role="status" aria-label="loading"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot_sau" runat="Server">
    <script src="<%= Helper_cl.VersionedUrl("~/js/aha-address-picker.js") %>"></script>
    <script>
        function setKieuFromTab() {
            var isCN = document.getElementById('target_cn')?.classList.contains('active');
            document.getElementById('<%= txtKieu.ClientID %>').value = isCN ? "Cá nhân" : "Cửa hàng";
        }

        document.addEventListener('shown.bs.tab', function () {
            setTimeout(setKieuFromTab, 50);
        });

        document.addEventListener('DOMContentLoaded', function () {
            setKieuFromTab();
            if (window.AhaAddressPicker) {
                window.AhaAddressPicker.init({
                    provinceSelectId: "profile_tinh",
                    districtSelectId: "profile_quan",
                    wardSelectId: "profile_phuong",
                    detailInputId: "<%= txt_diachi_chitiet.ClientID %>",
                    hiddenAddressId: "<%= txt_diachi.ClientID %>",
                    hiddenProvinceId: "<%= hf_profile_tinh.ClientID %>",
                    hiddenDistrictId: "<%= hf_profile_quan.ClientID %>",
                    hiddenWardId: "<%= hf_profile_phuong.ClientID %>",
                    rawAddressId: "<%= hf_profile_raw.ClientID %>"
                });
                window.AhaAddressPicker.init({
                    provinceSelectId: "shop_tinh",
                    districtSelectId: "shop_quan",
                    wardSelectId: "shop_phuong",
                    detailInputId: "<%= txt_diachi_shop_chitiet.ClientID %>",
                    hiddenAddressId: "<%= TextBox4.ClientID %>",
                    hiddenProvinceId: "<%= hf_shop_tinh.ClientID %>",
                    hiddenDistrictId: "<%= hf_shop_quan.ClientID %>",
                    hiddenWardId: "<%= hf_shop_phuong.ClientID %>",
                    rawAddressId: "<%= hf_shop_raw.ClientID %>"
                });
            }
        });

        function uploadFile() {
            var fileInput = document.getElementById("fileInput");
            var messageDiv = document.getElementById("message");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath");

            if (fileInput.files.length > 0) {
                var file = fileInput.files[0];
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) { messageDiv.innerHTML = "Định dạng ảnh không hợp lệ."; return; }
                var maxFileSize = 10 * 1024 * 1024;
                if (file.size > maxFileSize) { messageDiv.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB."; return; }

                var formData = new FormData(); formData.append("file", file);
                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        uploadedFilePathDiv.innerHTML = "<div class='small text-muted mb-1'>Ảnh mới chọn</div><img class='img-100' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= txt_link_fileupload.ClientID %>').value = xhr.responseText;
                    } else messageDiv.innerHTML = "Lỗi upload.";
                };
                xhr.send(formData);
            } else messageDiv.innerHTML = "Vui lòng chọn file.";
        }

        function uploadFile1() {
            var fileInput = document.getElementById("fileInput1");
            var messageDiv = document.getElementById("message1");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath1");

            if (fileInput && fileInput.files.length > 0) {
                var file = fileInput.files[0];
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) { messageDiv.innerHTML = "Định dạng ảnh không hợp lệ."; return; }
                var maxFileSize = 10 * 1024 * 1024;
                if (file.size > maxFileSize) { messageDiv.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB."; return; }

                var formData = new FormData(); formData.append("file", file);
                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        uploadedFilePathDiv.innerHTML = "<div class='small text-muted mb-1'>Ảnh mới chọn</div><img class='img-100' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= txt_link_fileupload1.ClientID %>').value = xhr.responseText;
                    } else messageDiv.innerHTML = "Lỗi upload.";
                };
                xhr.send(formData);
            } else if (messageDiv) messageDiv.innerHTML = "Vui lòng chọn file.";
        }

        function uploadFile2() {
            var fileInput = document.getElementById("fileInput2");
            var messageDiv = document.getElementById("message2");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath2");

            if (fileInput && fileInput.files.length > 0) {
                var file = fileInput.files[0];
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) { messageDiv.innerHTML = "Định dạng ảnh không hợp lệ."; return; }
                var maxFileSize = 10 * 1024 * 1024;
                if (file.size > maxFileSize) { messageDiv.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB."; return; }

                var formData = new FormData(); formData.append("file", file);
                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        uploadedFilePathDiv.innerHTML = "<div class='small text-muted mb-1'>Ảnh mới chọn</div><img class='img-100' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= txt_link_fileupload2.ClientID %>').value = xhr.responseText;
                    } else messageDiv.innerHTML = "Lỗi upload.";
                };
                xhr.send(formData);
            } else if (messageDiv) messageDiv.innerHTML = "Vui lòng chọn file.";
        }

        function uploadFile3() {
            var fileInput = document.getElementById("fileInput3");
            var messageDiv = document.getElementById("message3");
            var uploadedFilePathDiv = document.getElementById("uploadedFilePath3");

            if (fileInput.files.length > 0) {
                var file = fileInput.files[0];
                var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
                var fileExtension = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();
                if (allowedExtensions.indexOf(fileExtension) === -1) { messageDiv.innerHTML = "Định dạng ảnh không hợp lệ."; return; }
                var maxFileSize = 10 * 1024 * 1024;
                if (file.size > maxFileSize) { messageDiv.innerHTML = "Vui lòng chọn file có kích thước nhỏ hơn 10 MB."; return; }

                var formData = new FormData(); formData.append("file", file);
                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/uploads/Upload_Handler_Style1.ashx", true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        uploadedFilePathDiv.innerHTML = "<div class='small text-muted mb-1'>Ảnh mới chọn</div><img class='img-100' src='" + xhr.responseText + "' />";
                        document.getElementById('<%= TxtIcon.ClientID %>').value = xhr.responseText;
                    } else messageDiv.innerHTML = "Lỗi upload." + xhr.responseText;
                };
                xhr.send(formData);
            } else messageDiv.innerHTML = "Vui lòng chọn file.";
        }

        function syncProfileAccent() {
            var picker = document.getElementById('input_profile_accent');
            var hidden = document.getElementById('<%= hf_profile_accent.ClientID %>');
            if (!picker || !hidden) return;
            if (hidden.value) picker.value = hidden.value;
            picker.addEventListener('input', function () {
                hidden.value = picker.value || '#22c55e';
            });
        }

        function getSocialPresetElements() {
            return {
                hidden: document.getElementById('<%= hfSocialPreset.ClientID %>'),
                link: document.getElementById('<%= txtLink.ClientID %>'),
                name: document.getElementById('<%= txtTen.ClientID %>'),
                help: document.getElementById('socialPresetHelp'),
                buttons: document.querySelectorAll('.social-preset-btn')
            };
        }

        function getSocialPresetConfig(preset) {
            var configs = {
                facebook: {
                    placeholder: 'facebook.com/tennguoidung hoặc dán link đầy đủ',
                    help: 'Facebook: bạn chỉ cần dán link hoặc nhập tennguoidung, hệ thống sẽ tự hoàn thiện.'
                },
                zalo: {
                    placeholder: '0326824915 hoặc zalo.me/0326824915',
                    help: 'Zalo: chỉ cần nhập số điện thoại hoặc link zalo.me, hệ thống sẽ tự chuẩn hóa.'
                },
                tiktok: {
                    placeholder: '@tenkenh hoặc tiktok.com/@tenkenh',
                    help: 'TikTok: bạn có thể nhập @tenkenh hoặc link đầy đủ.'
                },
                youtube: {
                    placeholder: '@tenkenh hoặc youtube.com/@tenkenh',
                    help: 'YouTube: hỗ trợ @tenkenh, channel/... hoặc link đầy đủ.'
                },
                instagram: {
                    placeholder: 'tennguoidung hoặc instagram.com/tennguoidung',
                    help: 'Instagram: chỉ cần username, hệ thống sẽ tự ghép thành link hoàn chỉnh.'
                },
                telegram: {
                    placeholder: '@tennguoidung hoặc t.me/tennguoidung',
                    help: 'Telegram: bạn có thể nhập @tennguoidung hoặc link t.me đầy đủ.'
                },
                shopee: {
                    placeholder: 'dán link shop hoặc shop/123456',
                    help: 'Shopee: ưu tiên dán link shop đầy đủ, hoặc nhập nhanh dạng shop/123456.'
                },
                website: {
                    placeholder: 'tenmiencuaban.com',
                    help: 'Website: chỉ cần nhập tên miền, hệ thống sẽ tự thêm https:// nếu cần.'
                },
                defaultLink: {
                    placeholder: 'Ví dụ: https://www.facebook.com/tennguoidung',
                    help: 'Chọn ứng dụng để hệ thống tự gợi ý mẫu link. Bạn chỉ cần dán username, số điện thoại hoặc link đầy đủ.'
                }
            };

            return configs[preset] || configs.defaultLink;
        }

        function normalizeSocialPreset(value) {
            var preset = (value || '').toLowerCase().trim();
            if (preset === 'facebook' || preset === 'zalo' || preset === 'tiktok' || preset === 'youtube' || preset === 'instagram' || preset === 'telegram' || preset === 'shopee' || preset === 'website') {
                return preset;
            }
            return '';
        }

        function trimPrefix(value, prefix) {
            if (!value) return '';
            return value.toLowerCase().indexOf(prefix.toLowerCase()) === 0 ? value.substring(prefix.length) : value;
        }

        function buildPresetLink(preset, rawValue) {
            var raw = (rawValue || '').trim();
            if (!raw) return '';

            if (/^https?:\/\//i.test(raw)) {
                return raw;
            }

            if (preset === 'facebook') {
                var facebookValue = raw.replace(/^\/+/, '');
                facebookValue = trimPrefix(facebookValue, 'www.facebook.com/');
                facebookValue = trimPrefix(facebookValue, 'facebook.com/');
                facebookValue = facebookValue.replace(/^@/, '');
                return 'https://www.facebook.com/' + facebookValue;
            }

            if (preset === 'zalo') {
                var zaloValue = raw.replace(/^\/+/, '');
                zaloValue = trimPrefix(zaloValue, 'zalo.me/');
                zaloValue = trimPrefix(zaloValue, 'chat.zalo.me/');
                zaloValue = trimPrefix(zaloValue, 'zaloapp.com/qr/p/');
                var phone = zaloValue.replace(/\D/g, '');
                return phone ? ('https://zalo.me/' + phone) : ('https://zalo.me/' + zaloValue);
            }

            if (preset === 'tiktok') {
                var tiktokValue = raw.replace(/^\/+/, '');
                tiktokValue = trimPrefix(tiktokValue, 'www.tiktok.com/');
                tiktokValue = trimPrefix(tiktokValue, 'tiktok.com/');
                if (tiktokValue.indexOf('@') !== 0 && tiktokValue.indexOf('/') === -1) {
                    tiktokValue = '@' + tiktokValue;
                }
                return 'https://www.tiktok.com/' + tiktokValue;
            }

            if (preset === 'youtube') {
                var youtubeValue = raw.replace(/^\/+/, '');
                youtubeValue = trimPrefix(youtubeValue, 'www.youtube.com/');
                youtubeValue = trimPrefix(youtubeValue, 'youtube.com/');
                youtubeValue = trimPrefix(youtubeValue, 'youtu.be/');
                if (youtubeValue.indexOf('@') === 0 || youtubeValue.indexOf('channel/') === 0 || youtubeValue.indexOf('c/') === 0 || youtubeValue.indexOf('user/') === 0) {
                    return 'https://www.youtube.com/' + youtubeValue;
                }
                return 'https://www.youtube.com/@' + youtubeValue.replace(/^@/, '');
            }

            if (preset === 'instagram') {
                var instagramValue = raw.replace(/^\/+/, '');
                instagramValue = trimPrefix(instagramValue, 'www.instagram.com/');
                instagramValue = trimPrefix(instagramValue, 'instagram.com/');
                instagramValue = instagramValue.replace(/^@/, '');
                return 'https://www.instagram.com/' + instagramValue;
            }

            if (preset === 'telegram') {
                var telegramValue = raw.replace(/^\/+/, '');
                telegramValue = trimPrefix(telegramValue, 't.me/');
                telegramValue = trimPrefix(telegramValue, 'telegram.me/');
                telegramValue = telegramValue.replace(/^@/, '');
                return 'https://t.me/' + telegramValue;
            }

            if (preset === 'shopee') {
                var shopeeValue = raw.replace(/^\/+/, '');
                shopeeValue = trimPrefix(shopeeValue, 'www.shopee.vn/');
                shopeeValue = trimPrefix(shopeeValue, 'shopee.vn/');
                if (/^\d+$/.test(shopeeValue)) {
                    shopeeValue = 'shop/' + shopeeValue;
                }
                return 'https://shopee.vn/' + shopeeValue;
            }

            if (preset === 'website') {
                return /^https?:\/\//i.test(raw) ? raw : ('https://' + raw.replace(/^\/+/, ''));
            }

            if (/^[a-z0-9-]+(\.[a-z0-9-]+)+/i.test(raw)) {
                return 'https://' + raw;
            }

            return raw;
        }

        function syncSocialPresetUi() {
            var elements = getSocialPresetElements();
            if (!elements.link || !elements.hidden || !elements.help) return;

            var preset = normalizeSocialPreset(elements.hidden.value);
            var config = getSocialPresetConfig(preset);

            elements.link.placeholder = config.placeholder;
            elements.help.innerHTML = config.help;

            for (var i = 0; i < elements.buttons.length; i++) {
                var button = elements.buttons[i];
                var buttonPreset = normalizeSocialPreset(button.getAttribute('data-preset'));
                if (buttonPreset === preset || (!buttonPreset && !preset)) {
                    button.classList.add('active');
                } else {
                    button.classList.remove('active');
                }
            }
        }

        function selectSocialPreset(preset) {
            var elements = getSocialPresetElements();
            if (!elements.hidden) return;
            elements.hidden.value = normalizeSocialPreset(preset);
            syncSocialPresetUi();
        }

        function prepareSocialLinkBeforeSubmit() {
            var elements = getSocialPresetElements();
            if (!elements.link) return true;

            elements.link.value = buildPresetLink(normalizeSocialPreset(elements.hidden ? elements.hidden.value : ''), elements.link.value);
            return true;
        }

        document.addEventListener('DOMContentLoaded', function () {
            syncProfileAccent();
            syncSocialPresetUi();
        });

        if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                syncProfileAccent();
                syncSocialPresetUi();
            });
        }
    </script>
</asp:Content>
