<%@ Page Title="Trang cá nhân" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="home_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="Server">
    <asp:Literal ID="literal_meta" runat="server"></asp:Literal>
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head_sau" runat="Server">
    <style>
        .square-container { width: 100%; position: relative; overflow: hidden; border-radius: 12px; background: #f6f8fb; }
        .square-container::before { content: ""; display: block; padding-top: 100%; }
        .square-container img { position: absolute; inset: 0; width: 100%; height: 100%; object-fit: cover; }
        .text-clamp-1, .text-clamp-2 {
            display: -webkit-box;
            -webkit-box-orient: vertical;
            overflow: hidden;
            text-overflow: ellipsis;
            line-height: 18px;
        }
        .text-clamp-1 { -webkit-line-clamp: 1; }
        .text-clamp-2 { -webkit-line-clamp: 2; }
        .rounded-circle { object-fit: cover; object-position: 50% 50%; border-radius: 50%; }
        .tblr-overlay {
            position: fixed; inset: 0;
            background: rgba(0,0,0,.65);
            z-index: 99999;
            display: flex; align-items: center; justify-content: center;
        }
        .modal-body { max-height: calc(100vh - 220px); overflow: auto; }

        .bio-public-shell {
            max-width: 780px;
            margin: 16px auto 34px;
            padding: 0 12px;
        }

        .bio-hero {
            position: relative;
            border-radius: 24px;
            background:
                radial-gradient(1000px 320px at 16% -24%, rgba(25, 201, 118, .45), transparent 60%),
                radial-gradient(920px 300px at 86% -30%, rgba(22, 163, 74, .32), transparent 58%),
                linear-gradient(140deg, #0f766e, #16a34a 58%, #22c55e);
            min-height: 240px;
            box-shadow: 0 24px 50px rgba(16, 42, 67, .24);
            overflow: hidden;
            margin-bottom: 18px;
        }

        .bio-hero::after {
            content: "";
            position: absolute;
            inset: 0;
            background: linear-gradient(180deg, rgba(0,0,0,.04), rgba(0,0,0,.34));
            pointer-events: none;
        }

        .bio-hero-content {
            position: relative;
            z-index: 2;
            min-height: 240px;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: flex-end;
            text-align: center;
            color: #fff;
            padding: 18px 18px 20px;
        }

        .bio-avatar {
            width: 106px;
            height: 106px;
            border-radius: 50%;
            border: 4px solid rgba(255,255,255,.92);
            box-shadow: 0 18px 30px rgba(0,0,0,.28);
            object-fit: cover;
            background: #f4f8fd;
        }

        .bio-name {
            margin-top: 12px;
            font-size: 34px;
            line-height: 1.05;
            font-weight: 800;
            text-shadow: 0 12px 24px rgba(0,0,0,.32);
        }

        .bio-account {
            margin-top: 4px;
            font-size: 14px;
            opacity: .92;
            letter-spacing: .02em;
        }

        .bio-role {
            margin-top: 8px;
        }

        .bio-intro {
            margin-top: 10px;
            max-width: 620px;
            font-size: 15px;
            line-height: 1.6;
            color: rgba(255,255,255,.95);
        }

        .bio-actions {
            margin-top: 14px;
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
            gap: 10px;
        }

        .bio-action-btn {
            min-height: 38px;
            padding: 0 16px;
            border-radius: 999px;
            border: 1px solid rgba(255,255,255,.52);
            background: rgba(255,255,255,.16);
            color: #fff;
            font-size: 13px;
            font-weight: 700;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
            text-decoration: none;
            cursor: pointer;
            transition: .2s ease;
        }

        .bio-action-btn:hover {
            color: #fff;
            border-color: rgba(255,255,255,.78);
            background: rgba(255,255,255,.25);
            transform: translateY(-1px);
        }

        .bio-action-btn.bio-action-solid {
            border-color: #ffffff;
            background: #ffffff;
            color: #0f5132;
        }

        .bio-action-btn.bio-action-solid:hover {
            color: #0f5132;
            background: #ecfff6;
        }

        .bio-card {
            border: 1px solid #d8e4f0;
            border-radius: 18px;
            background: #fff;
            box-shadow: 0 14px 34px rgba(16, 42, 67, .08);
            margin-bottom: 14px;
        }

        .bio-card-head {
            padding: 14px 16px 8px;
            border-bottom: 1px solid #e6eef7;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            flex-wrap: wrap;
        }

        .bio-card-title {
            margin: 0;
            font-size: 20px;
            font-weight: 800;
            color: #102a43;
        }

        .bio-card-sub {
            color: #627d98;
            font-size: 13px;
        }

        .bio-card-body {
            padding: 14px 16px 16px;
        }

        .bio-contact-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(180px, 1fr));
            gap: 10px;
        }

        .bio-contact-item {
            border: 1px solid #dce7f2;
            border-radius: 12px;
            padding: 10px 12px;
            background: #f8fbff;
            display: flex;
            align-items: flex-start;
            gap: 10px;
            color: #102a43;
            min-height: 66px;
        }

        .bio-contact-item i {
            font-size: 18px;
            color: #0f766e;
            margin-top: 1px;
        }

        .bio-contact-label {
            font-size: 12px;
            color: #627d98;
            line-height: 1.1;
            margin-bottom: 4px;
        }

        .bio-contact-value {
            font-size: 14px;
            font-weight: 700;
            line-height: 1.35;
            word-break: break-word;
        }

        .bio-link-list {
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .bio-link-item {
            width: 100%;
            border: 1px solid #dce7f2;
            border-radius: 14px;
            background: #f9fcff;
            color: #102a43;
            text-decoration: none;
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 10px 12px;
            transition: .16s ease;
        }

        .bio-link-item:hover {
            color: #0b3f65;
            border-color: #b8d1ea;
            background: #f1f8ff;
            transform: translateY(-1px);
        }

        .bio-link-icon {
            width: 42px;
            height: 42px;
            border-radius: 10px;
            object-fit: cover;
            border: 1px solid #dce7f2;
            background: #fff;
        }

        .bio-link-icon.empty {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            color: #0f766e;
            font-size: 18px;
        }

        .bio-link-body {
            min-width: 0;
            flex: 1;
        }

        .bio-link-title {
            font-size: 15px;
            font-weight: 800;
            line-height: 1.2;
            margin-bottom: 4px;
        }

        .bio-link-url {
            font-size: 13px;
            color: #627d98;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .bio-link-arrow {
            color: #4f6f8f;
            font-size: 18px;
        }

        .bio-empty {
            border: 1px dashed #c4d5e7;
            border-radius: 14px;
            background: #f8fbff;
            padding: 18px;
            color: #627d98;
            text-align: center;
            font-size: 14px;
        }

        .bio-review-item {
            border: 1px solid #deebf7;
            border-radius: 14px;
            padding: 12px;
            background: #fff;
            margin-bottom: 12px;
        }

        .bio-review-head {
            display: flex;
            align-items: center;
            gap: 10px;
            margin-bottom: 8px;
        }

        .bio-review-avatar {
            width: 46px;
            height: 46px;
            border-radius: 50%;
            object-fit: cover;
            border: 1px solid #dbe7f3;
            background: #f1f5f9;
        }

        .bio-review-title {
            font-size: 15px;
            font-weight: 800;
            color: #102a43;
        }

        .bio-review-meta {
            font-size: 12px;
            color: #627d98;
        }

        .bio-review-stars {
            color: #f59f00;
            font-size: 16px;
            margin-bottom: 6px;
        }

        .bio-review-content {
            color: #334e68;
            font-size: 14px;
            line-height: 1.5;
        }

        .review-img {
            cursor: pointer;
            border-radius: 10px;
            object-fit: cover;
            margin-top: 8px;
        }

        .pagination a, .pagination .current-page {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 34px;
            height: 34px;
            padding: 0 10px;
            margin: 2px;
            border: 1px solid rgba(98,105,118,.3);
            border-radius: 8px;
            text-decoration: none;
            color: var(--tblr-body-color, #1f2937);
            background: #fff;
        }

        .pagination a:hover { background: rgba(98,105,118,.06); }

        .pagination .current-page {
            font-weight: 700;
            background: rgba(47,179,68,.12);
            border-color: rgba(47,179,68,.35);
            color: rgb(47,179,68);
            cursor: default;
        }

        @media (max-width: 767.98px) {
            .bio-public-shell {
                margin-top: 10px;
                padding: 0 10px;
            }

            .bio-hero {
                min-height: 210px;
                border-radius: 20px;
            }

            .bio-hero-content {
                min-height: 210px;
                padding: 16px 12px 16px;
            }

            .bio-avatar {
                width: 90px;
                height: 90px;
            }

            .bio-name {
                font-size: 28px;
            }

            .bio-contact-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="Server">

    <!-- ===================== MODAL: XÁC NHẬN TRAO ĐỔI ===================== -->
    <asp:UpdatePanel ID="up_dathang" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_dathang" runat="server" Visible="false" DefaultButton="but_dathang">
                <div class="modal modal-blur show" style="display:block;" tabindex="-1" role="dialog" aria-modal="true">
                    <div class="modal-dialog modal-md modal-dialog-centered" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Xác nhận trao đổi</h5>
                                <a href="#" class="btn-close" aria-label="Close" id="A1" runat="server" onserverclick="but_close_form_dathang_Click"></a>
                            </div>
                            <div class="modal-body">
                                <div class="mb-2 fw-semibold">
                                    <asp:Literal ID="Literal9" runat="server"></asp:Literal>
                                </div>
                                <div class="mb-3 text-muted">
                                    <img src="/uploads/images/dong-a.png" style="width:14px" class="me-1" />
                                    <asp:Literal ID="Literal10" runat="server"></asp:Literal>
                                </div>
                                <div class="mb-3">
                                    <span class="text-muted">Ưu đãi:</span>
                                    <span class="ms-2 text-warning fw-semibold">
                                        <asp:Literal ID="LiteralUuDaiPercent" runat="server"></asp:Literal>%
                                    </span>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label text-danger">Số lượng</label>
                                    <asp:TextBox ID="txt_soluong2" runat="server" AutoPostBack="true" OnTextChanged="txt_soluong2_TextChanged"
                                        CssClass="form-control" MaxLength="3" onfocus="AutoSelect(this)" oninput="format_sotien_new(this)"></asp:TextBox>
                                </div>

                                <div class="mb-3">
                                    <span class="text-muted">Tổng phải Trao đổi:</span>
                                    <span class="ms-2">
                                        <img src="/uploads/images/dong-a.png" style="width:14px" class="me-1" />
                                        <asp:Literal ID="Literal11" runat="server"></asp:Literal>
                                    </span>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label text-danger">Người nhận</label>
                                    <asp:TextBox ID="txt_hoten_nguoinhan" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label text-danger">Điện thoại</label>
                                    <asp:TextBox ID="txt_sdt_nguoinhan" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>

                                <div class="mb-0">
                                    <label class="form-label text-danger">Địa chỉ</label>
                                    <asp:TextBox ID="txt_diachi_nguoinhan" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <a href="#" class="btn btn-outline-secondary" id="A1b" runat="server" onserverclick="but_close_form_dathang_Click">Đóng</a>
                                <asp:Button ID="but_dathang" OnClick="but_dathang_Click" runat="server" Text="Xác nhận trao đổi" CssClass="btn btn-primary" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="up_dathang">
        <ProgressTemplate>
            <div class="tblr-overlay">
                <div class="text-center">
                    <div class="spinner-border" role="status"></div>
                    <div class="mt-3 text-white">Đang xử lý...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>


    <!-- ===================== MODAL: THÊM VÀO GIỎ ===================== -->
    <asp:UpdatePanel ID="up_add_cart" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pn_add_cart" runat="server" Visible="false" DefaultButton="but_add_cart">
                <div class="modal modal-blur show" style="display:block;" tabindex="-1" role="dialog" aria-modal="true">
                    <div class="modal-dialog modal-md modal-dialog-centered" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Thêm vào giỏ hàng</h5>
                                <a href="#" class="btn-close" aria-label="Close" id="close_add" runat="server" onserverclick="but_close_form_addcart_Click"></a>
                            </div>
                            <div class="modal-body">
                                <div class="mb-3 fw-semibold">
                                    <asp:Literal ID="Literal8" runat="server"></asp:Literal>
                                </div>
                                <div class="mb-0">
                                    <label class="form-label text-danger">Số lượng</label>
                                    <asp:TextBox ID="txt_soluong1" runat="server" CssClass="form-control"
                                        MaxLength="3" onfocus="AutoSelect(this)" oninput="format_sotien_new(this)"></asp:TextBox>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <a href="#" class="btn btn-outline-secondary" id="close_add_b" runat="server" onserverclick="but_close_form_addcart_Click">Đóng</a>
                                <asp:Button ID="but_add_cart" OnClick="but_add_cart_Click" runat="server" Text="Thêm vào giỏ hàng" CssClass="btn btn-primary" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="up_add_cart">
        <ProgressTemplate>
            <div class="tblr-overlay">
                <div class="text-center">
                    <div class="spinner-border" role="status"></div>
                    <div class="mt-3 text-white">Đang xử lý...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>


    <!-- ===================== MAIN ===================== -->
    <asp:UpdatePanel ID="up_main" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="page-body">
                <div class="bio-public-shell">
                    <div class="d-none">
                        <asp:Literal ID="Literal13" runat="server"></asp:Literal>
                    </div>

                    <section class="bio-hero">
                        <div class="bio-hero-content">
                            <img src="<%= ViewState["avt_query"] %>" alt="avatar" class="bio-avatar" />
                            <div class="bio-name"><%= ViewState["hoten_query"] %></div>
                            <div class="bio-account">@<%= ViewState["taikhoan_hienthi_query"] %></div>
                            <div class="bio-role"><%= ViewState["phanloai_query"] %></div>
                            <div class="bio-intro"><%= ViewState["gioithieu_query"] %></div>

                            <div class="bio-actions">
                                <a href="<%= ViewState["sdt_href_query"] %>" class="bio-action-btn bio-action-solid">
                                    <i class="ti ti-phone"></i>Gọi ngay
                                </a>
                                <a href="<%= ViewState["public_profile_url"] %>" target="_blank" class="bio-action-btn">
                                    <i class="ti ti-world"></i>Mở link hồ sơ
                                </a>
                                <button type="button"
                                    class="bio-action-btn"
                                    data-profile-link="<%: (ViewState["public_profile_url"] ?? "").ToString() %>"
                                    onclick="copyProfileLink(this)">
                                    <i class="ti ti-copy"></i>Sao chép link
                                </button>
                                <asp:PlaceHolder ID="phOwnerEditButton" runat="server" Visible="false">
                                    <a href="/home/edit-info.aspx" class="bio-action-btn">
                                        <i class="ti ti-edit"></i>Chỉnh sửa hồ sơ
                                    </a>
                                </asp:PlaceHolder>
                            </div>
                        </div>
                    </section>

                    <section class="bio-card">
                        <div class="bio-card-head">
                            <div>
                                <h3 class="bio-card-title">Thông tin cá nhân</h3>
                                <div class="bio-card-sub">Có <b><%= ViewState["SoLuongDanhGia"] %></b> lượt đánh giá trên hồ sơ này</div>
                            </div>
                        </div>
                        <div class="bio-card-body">
                            <div class="bio-contact-grid">
                                <div class="bio-contact-item">
                                    <i class="ti ti-phone"></i>
                                    <div>
                                        <div class="bio-contact-label">Điện thoại</div>
                                        <div class="bio-contact-value"><%= ViewState["sdt_query"] %></div>
                                    </div>
                                </div>
                                <div class="bio-contact-item">
                                    <i class="ti ti-mail"></i>
                                    <div>
                                        <div class="bio-contact-label">Email</div>
                                        <div class="bio-contact-value"><%= ViewState["email_query"] %></div>
                                    </div>
                                </div>
                                <div class="bio-contact-item">
                                    <i class="ti ti-map-pin"></i>
                                    <div>
                                        <div class="bio-contact-label">Địa chỉ</div>
                                        <div class="bio-contact-value"><%= ViewState["diachi_query"] %></div>
                                    </div>
                                </div>
                                <div class="bio-contact-item">
                                    <i class="ti ti-coin"></i>
                                    <div>
                                        <div class="bio-contact-label">Hồ sơ quyền tiêu dùng</div>
                                        <div class="bio-contact-value"><%= ViewState["DongA_query"] %></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </section>

                    <section class="bio-card">
                        <div class="bio-card-head">
                            <div>
                                <h3 class="bio-card-title">Liên kết cá nhân</h3>
                                <div class="bio-card-sub">Các kênh liên hệ và mạng xã hội đã cấu hình từ trang chỉnh sửa hồ sơ</div>
                            </div>
                        </div>
                        <div class="bio-card-body">
                            <div class="bio-link-list">
                                <asp:Repeater ID="rptMangXaHoiCN" runat="server">
                                    <ItemTemplate>
                                        <a class="bio-link-item" target="_blank" href='<%# Eval("Link") %>'>
                                            <asp:Image ID="imgIcon" runat="server"
                                                CssClass="bio-link-icon"
                                                ImageUrl='<%# Eval("Icon") %>'
                                                Visible='<%# ShouldShowIcon(Eval("Icon")) %>' />
                                            <asp:Literal ID="litIconFallback" runat="server"
                                                Text='<%# ShouldShowIcon(Eval("Icon")) ? "" : "<span class=\"bio-link-icon empty\"><i class=\"ti ti-world\"></i></span>" %>'></asp:Literal>
                                            <span class="bio-link-body">
                                                <span class="bio-link-title"><%# Eval("Ten") %></span>
                                                <span class="bio-link-url"><%# Eval("Link") %></span>
                                            </span>
                                            <i class="ti ti-arrow-up-right bio-link-arrow"></i>
                                        </a>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                            <asp:PlaceHolder ID="phNoSocialLinks" runat="server" Visible="false">
                                <div class="bio-empty">Chưa có liên kết cá nhân nào. Bạn có thể thêm tại mục chỉnh sửa hồ sơ.</div>
                            </asp:PlaceHolder>
                        </div>
                    </section>

                        <!-- ✅ BỌC TOÀN BỘ CỬA HÀNG: nếu chưa là gian hàng đối tác -> Visible=false -->
                        <asp:PlaceHolder ID="phCuaHang" runat="server" Visible="false">
                            <div class="col-lg-7">
                                <div class="card">
                                    <div class="card-body">
                                        <div class="h3 mb-2"><i class="ti ti-building-store me-1"></i>Giới thiệu cửa hàng</div>

                                        <div class="mb-3">
                                            <asp:Literal ID="Literal6" runat="server"></asp:Literal>
                                        </div>

                                        <div class="card bg-muted-lt border-0">
                                            <div class="card-body">
                                                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                                            </div>
                                        </div>

                                        <div class="mt-3 text-muted">
                                            <div class="mb-2"><i class="ti ti-phone me-1"></i><asp:Literal ID="Literal2" runat="server"></asp:Literal></div>
                                            <div><i class="ti ti-map-pin me-1"></i><asp:Literal ID="Literal3" runat="server"></asp:Literal></div>
                                        </div>

                                        <div class="mt-4">
                                            <div class="fw-semibold mb-2">Mạng xã hội cửa hàng</div>
                                            <asp:Repeater ID="rptMangXaHoiCH" runat="server">
                                                <ItemTemplate>
                                                    <div class="d-flex align-items-center mb-3">
                                                        <asp:Image ID="imgIcon" runat="server"
                                                            ImageUrl='<%# Eval("Icon") %>'
                                                            Width="42" Height="42"
                                                            Style="object-fit: cover; border-radius: 10px; margin-right: 10px;"
                                                            Visible='<%# ShouldShowIcon(Eval("Icon")) %>' />
                                                        <div style='<%# GetMarginStyle(Eval("Icon")) %>' class="flex-fill">
                                                            <div class="fw-semibold"><%# Eval("Ten") %></div>
                                                            <div class="text-muted small text-truncate"><%# Eval("Link") %></div>
                                                        </div>
                                                        <a class="btn btn-sm btn-outline-secondary" target="_blank" href='<%# Eval("Link") %>'>
                                                            <i class="ti ti-external-link"></i>
                                                        </a>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                    <!-- ✅ BỌC TOÀN BỘ SẢN PHẨM CỬA HÀNG -->
                    <asp:PlaceHolder ID="phSanPhamCuaHang" runat="server" Visible="false">

                        <!-- Products -->
                        <div class="card mt-4">
                            <div class="card-header">
                                <div>
                                    <div class="card-title">Sản phẩm đang bán</div>
                                    <div class="text-muted small">Danh sách sản phẩm của cửa hàng</div>
                                </div>

                                <div class="ms-auto d-flex align-items-center gap-2">
                                    <div class="text-muted small d-none d-md-block">
                                        <asp:Label ID="lb_show" runat="server" Text=""></asp:Label>
                                    </div>

                                    <asp:LinkButton ID="but_quaylai" OnClick="but_quaylai_Click" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                                        <i class="ti ti-chevron-left"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="but_xemtiep" OnClick="but_xemtiep_Click" runat="server" CssClass="btn btn-outline-secondary btn-sm">
                                        <i class="ti ti-chevron-right"></i>
                                    </asp:LinkButton>
                                </div>
                            </div>

                            <div class="card-body">
                                <div class="row g-2 align-items-center mb-3">
                                    <div class="col-md-6">
                                        <asp:TextBox MaxLength="50" ID="txt_search" runat="server" CssClass="form-control"
                                            AutoPostBack="true" placeholder="Nhập từ khóa..." OnTextChanged="txt_search_TextChanged"></asp:TextBox>
                                    </div>
                                    <div class="col-md-6" style="display:none">
                                        <asp:TextBox MaxLength="50" ID="txt_timkiem1" runat="server" CssClass="form-control"
                                            AutoPostBack="true" placeholder="Nhập từ khóa..." OnTextChanged="txt_timkiem_TextChanged"></asp:TextBox>
                                    </div>

                                    <!-- giữ control mobile cũ (không phá code phân trang) -->
                                    <div class="d-none">
                                        <asp:Label ID="lb_show_md" runat="server" Text=""></asp:Label>
                                        <asp:LinkButton ID="but_quaylai1" OnClick="but_quaylai_Click" runat="server" />
                                        <asp:LinkButton ID="but_xemtiep1" OnClick="but_xemtiep_Click" runat="server" />
                                    </div>
                                </div>

                                <div class="card bg-muted-lt border-0 mb-3">
                                    <div class="card-body">
                                        <div class="row">
                                            <div class="col-6">Sản phẩm: <b><asp:Literal ID="Literal7" runat="server"></asp:Literal></b></div>
                                            <div class="col-6">Đã bán: <b><asp:Literal ID="Literal12" runat="server"></asp:Literal></b></div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row g-3">

                                    <!-- Left column products -->
                                    <div class="col-lg-6">
                                        <div class="row g-3">
                                            <asp:Repeater ID="Repeater2" runat="server" OnItemDataBound="Repeater2_ItemDataBound">
                                                <ItemTemplate>
                                                    <div class="col-6">
                                                        <div class="card h-100">
                                                            <a class="text-decoration-none" href="/<%#Eval("name_en") %>-<%#Eval("id") %>.html">
                                                                <div class="square-container">
                                                                    <img src="<%# Eval("image") %>" alt="<%# Eval("name") %>" />
                                                                </div>
                                                            </a>

                                                            <div class="card-body p-2">
                                                                <div class="text-clamp-2 fw-semibold mb-1">
                                                                    <small><%# Eval("name") %></small>
                                                                </div>
                                                                <div class="text-clamp-2 text-muted">
                                                                    <small><%# Eval("description") %></small>
                                                                </div>

                                                                <div class="mt-2 fw-bold">
                                                                    <small><%# Eval("giaban", "{0:#,##0.##}") %> đ</small>
                                                                </div>

                                                                <!-- BADGE phân biệt SP -->
                                                                <div class="mt-2">
                                                                    <asp:Literal ID="lit_badge" runat="server"></asp:Literal>
                                                                </div>

                                                                <div class="mt-2 text-muted" style="font-size: 12px;">
                                                                    <div class="d-flex justify-content-between">
                                                                        <span><%# Eval("ngaytao", "{0:dd/MM/yyyy HH:mm}") %></span>
                                                                        <span>Lượt xem: <%# Eval("LuotTruyCap", "{0:#,##0}") %></span>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div class="card-footer p-2">
                                                                <div class="row g-2">
                                                                    <div class="col-6 d-none">
                                                                        <asp:Button ID="but_bansanphamnay" Width="100%" OnClick="but_bansanphamnay_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Bán chéo"
                                                                            CssClass="btn btn-sm btn-outline-warning w-100" Visible="false" />
                                                                    </div>
                                                                    <div class="col-6">
                                                                        <asp:Button ID="but_traodoi" Width="100%" OnClick="but_traodoi_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Trao đổi ngay"
                                                                            CssClass="btn btn-sm btn-primary w-100" />
                                                                    </div>
                                                                    <div class="col-12">
                                                                        <asp:Button ID="but_themvaogio" Width="100%" OnClick="but_themvaogio_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Thêm vào giỏ hàng"
                                                                            CssClass="btn btn-sm btn-outline-secondary w-100" />
                                                                    </div>

                                                                    <!-- NÚT HỦY BÁN CHÉO (chỉ hiện khi SP bán chéo & đang xem shop của chính mình) -->
                                                                    <div class="col-12">
                                                                        <asp:Button ID="but_huy_bancheo" Width="100%" OnClick="but_huy_bancheo_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Hủy bán chéo"
                                                                            CssClass="btn btn-sm btn-outline-danger w-100" Visible="false" />
                                                                    </div>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>

                                    <!-- Right column products -->
                                    <div class="col-lg-6">
                                        <div class="row g-3">
                                            <asp:Repeater ID="Repeater4" runat="server" OnItemDataBound="Repeater2_ItemDataBound">
                                                <ItemTemplate>
                                                    <div class="col-6">
                                                        <div class="card h-100">
                                                            <a class="text-decoration-none" href="/<%#Eval("name_en") %>-<%#Eval("id") %>.html">
                                                                <div class="square-container">
                                                                    <img src="<%# Eval("image") %>" alt="<%# Eval("name") %>" />
                                                                </div>
                                                            </a>

                                                            <div class="card-body p-2">
                                                                <div class="text-clamp-2 fw-semibold mb-1">
                                                                    <small><%# Eval("name") %></small>
                                                                </div>
                                                                <div class="text-clamp-2 text-muted">
                                                                    <small><%# Eval("description") %></small>
                                                                </div>

                                                                <div class="mt-2 fw-bold">
                                                                    <small><%# Eval("giaban", "{0:#,##0.##}") %> đ</small>
                                                                </div>

                                                                <!-- BADGE phân biệt SP -->
                                                                <div class="mt-2">
                                                                    <asp:Literal ID="lit_badge" runat="server"></asp:Literal>
                                                                </div>

                                                                <div class="mt-2 text-muted" style="font-size: 12px;">
                                                                    <div class="d-flex justify-content-between">
                                                                        <span><%# Eval("ngaytao", "{0:dd/MM/yyyy HH:mm}") %></span>
                                                                        <span>Lượt xem: <%# Eval("LuotTruyCap", "{0:#,##0}") %></span>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div class="card-footer p-2">
                                                                <div class="row g-2">
                                                                    <div class="col-6 d-none">
                                                                        <asp:Button ID="but_bansanphamnay" Width="100%" OnClick="but_bansanphamnay_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Bán chéo"
                                                                            CssClass="btn btn-sm btn-outline-warning w-100" Visible="false" />
                                                                    </div>
                                                                    <div class="col-6">
                                                                        <asp:Button ID="but_traodoi" Width="100%" OnClick="but_traodoi_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Trao đổi ngay"
                                                                            CssClass="btn btn-sm btn-primary w-100" />
                                                                    </div>
                                                                    <div class="col-12">
                                                                        <asp:Button ID="but_themvaogio" Width="100%" OnClick="but_themvaogio_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Thêm vào giỏ hàng"
                                                                            CssClass="btn btn-sm btn-outline-secondary w-100" />
                                                                    </div>

                                                                    <!-- NÚT HỦY BÁN CHÉO -->
                                                                    <div class="col-12">
                                                                        <asp:Button ID="but_huy_bancheo" Width="100%" OnClick="but_huy_bancheo_Click"
                                                                            CommandArgument='<%# Eval("id") %>' runat="server" Text="Hủy bán chéo"
                                                                            CssClass="btn btn-sm btn-outline-danger w-100" Visible="false" />
                                                                    </div>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>

                                </div>

                            </div>
                        </div>

                    </asp:PlaceHolder>

                    <!-- Reviews -->
                    <asp:UpdatePanel ID="Review" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="true">
                                <div class="bio-card mt-2">
                                    <div class="bio-card-head">
                                        <div>
                                            <h3 class="bio-card-title">Đánh giá người dùng</h3>
                                            <div class="bio-card-sub">Tất cả phản hồi công khai từ người dùng đã trao đổi</div>
                                        </div>
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="ListReview" runat="server" Visible="true">
                                <div class="bio-card">
                                    <div class="bio-card-body">
                                        <asp:Repeater ID="rptDanhGia" runat="server">
                                            <ItemTemplate>
                                                <div class="bio-review-item">
                                                    <div class="bio-review-head">
                                                        <asp:Image ID="imgAvatar" runat="server" ImageUrl='<%# Eval("AnhDaiDien") %>'
                                                            CssClass="bio-review-avatar" AlternateText="Ảnh đại diện" />

                                                        <div class="flex-fill">
                                                            <a href="<%#Eval("HoSoUrl")%>" class="bio-review-title text-decoration-none">
                                                                <%# Eval("TaiKhoanDanhGia") %>
                                                            </a>
                                                            <div class="bio-review-meta"><%# Eval("NgayDang", "{0:dd/MM/yyyy HH:mm}") %></div>
                                                        </div>
                                                    </div>

                                                    <div class="bio-review-stars"><%# new string('★', Convert.ToInt32(Eval("Diem"))) %></div>
                                                    <div class="bio-review-content"><%# Eval("NoiDung") %></div>

                                                    <asp:Image ID="imgReview" runat="server"
                                                        ImageUrl='<%# Eval("UrlAnh") %>'
                                                        Width="120"
                                                        CssClass="review-img"
                                                        Style="max-height:120px;"
                                                        Visible='<%# !string.IsNullOrEmpty(Eval("UrlAnh") as string) %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                        <div class="mt-3">
                                            <asp:Panel ID="pnlPaging" runat="server" CssClass="pagination"></asp:Panel>
                                        </div>
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <!-- Review image modal (Tabler/Bootstrap) -->
                    <div class="modal modal-blur fade" id="reviewImageModal" tabindex="-1" aria-hidden="true">
                        <div class="modal-dialog modal-lg modal-dialog-centered">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h5 class="modal-title">Xem ảnh</h5>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                </div>
                                <div class="modal-body text-center">
                                    <img id="reviewModalImage" src="" style="max-width:100%; max-height:75vh; border-radius: 12px;" />
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="up_main">
        <ProgressTemplate>
            <div class="tblr-overlay">
                <div class="text-center">
                    <div class="spinner-border" role="status"></div>
                    <div class="mt-3 text-white">Đang tải...</div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

<asp:Content ID="ContentFootTruoc" ContentPlaceHolderID="foot_truoc" runat="Server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="foot_sau" runat="Server">

    <!-- jQuery (phải trước jQuery UI) -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <!-- jQuery UI -->
    <link href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css" rel="stylesheet" />
    <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>

    <script type="text/javascript">
        var user_query = '<%= ViewState["user_query"] %>';

        function copyProfileLink(btn) {
            if (!btn) return;
            var link = btn.getAttribute('data-profile-link') || '';
            if (!link) return;

            function onCopied() {
                var oldHtml = btn.innerHTML;
                btn.innerHTML = '<i class="ti ti-check"></i>Đã sao chép';
                setTimeout(function () { btn.innerHTML = oldHtml; }, 1400);
            }

            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(link).then(onCopied).catch(function () {
                    window.prompt('Sao chép liên kết hồ sơ:', link);
                });
            } else {
                window.prompt('Sao chép liên kết hồ sơ:', link);
            }
        }

        // Review image -> Tabler modal
        (function () {
            function initReviewModal() {
                var modalEl = document.getElementById('reviewImageModal');
                if (!modalEl || !window.bootstrap) return;

                var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
                document.querySelectorAll('.review-img').forEach(function (img) {
                    img.addEventListener('click', function () {
                        document.getElementById('reviewModalImage').src = img.src;
                        modal.show();
                    });
                });
            }

            // init on first load
            document.addEventListener('DOMContentLoaded', initReviewModal);

            // re-init after UpdatePanel postback
            if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    initReviewModal();
                });
            }
        })();
    </script>

    <script type="text/javascript">
        $(function () {
            var txtSearch = $('#<%= txt_search.ClientID %>');
            txtSearch.autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        url: "home/Default.aspx/GetSuggestions",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify({
                            prefixText: request.term,
                            count: 10,
                            userQuery: user_query
                        }),
                        success: function (data) { response(data.d); }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    txtSearch.val(ui.item.value);
                    __doPostBack('<%= txt_search.UniqueID %>', '');
                    return false;
                }
            });

            var txtSearch1 = $('#<%= txt_timkiem1.ClientID %>');
            txtSearch1.autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        url: "home/Default.aspx/GetSuggestions",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify({
                            prefixText: request.term,
                            count: 10,
                            userQuery: user_query
                        }),
                        success: function (data) { response(data.d); }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    txtSearch1.val(ui.item.value);
                    __doPostBack('<%= txt_timkiem1.UniqueID %>', '');
                    return false;
                }
            });
        });
    </script>

</asp:Content>
