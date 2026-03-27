<%@ Page Title="Trình bày storefront" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="trinh-bay.aspx.cs" Inherits="gianhang_admin_gianhang_trinh_bay" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-present-shell { display:grid; gap:18px; }
        .gh-present-hero,.gh-present-card { border:1px solid #eadff7; border-radius:22px; background:#fff; box-shadow:0 18px 34px rgba(15,23,42,.06); }
        .gh-present-hero { padding:22px; background:radial-gradient(760px 240px at 0% 0%, rgba(124,58,237,.12), transparent 60%),linear-gradient(180deg,#fbf8ff 0%,#fff 100%); display:flex; justify-content:space-between; align-items:flex-start; gap:18px; flex-wrap:wrap; }
        .gh-present-kicker { display:inline-flex; align-items:center; min-height:30px; padding:0 12px; border-radius:999px; background:#f3e8ff; border:1px solid #e9d5ff; color:#6d28d9; font-size:12px; font-weight:800; letter-spacing:.03em; text-transform:uppercase; }
        .gh-present-title { margin:10px 0 6px; color:#4c1d95; font-size:30px; line-height:1.12; font-weight:900; }
        .gh-present-sub { margin:0; color:#6b5a8d; font-size:14px; line-height:1.6; max-width:760px; }
        .gh-present-actions { display:flex; gap:10px; flex-wrap:wrap; }
        .gh-present-btn { display:inline-flex; align-items:center; justify-content:center; min-height:42px; padding:0 16px; border-radius:14px; text-decoration:none!important; font-size:13px; font-weight:800; border:1px solid transparent; }
        .gh-present-btn--primary { background:linear-gradient(135deg,#7c3aed,#a855f7); color:#fff!important; box-shadow:0 14px 28px rgba(124,58,237,.18); }
        .gh-present-btn--soft { background:#fff; color:#4c1d95!important; border-color:#ddd6fe; }
        .gh-present-stats { display:grid; grid-template-columns:repeat(auto-fit,minmax(170px,1fr)); gap:12px; }
        .gh-present-stat { border:1px solid #ede9fe; border-radius:18px; background:#fff; padding:16px; }
        .gh-present-stat small { display:block; color:#7c3aed; font-size:12px; text-transform:uppercase; font-weight:700; }
        .gh-present-stat strong { display:block; margin-top:8px; color:#1f2937; font-size:28px; line-height:1.05; font-weight:900; }
        .gh-present-stat span { display:block; margin-top:6px; color:#6b7280; font-size:13px; line-height:1.5; }
        .gh-present-grid { display:grid; grid-template-columns:repeat(auto-fit,minmax(280px,1fr)); gap:16px; }
        .gh-present-card { padding:18px; }
        .gh-present-card h3 { margin:0 0 6px; font-size:20px; line-height:1.2; color:#1f2937; }
        .gh-present-card p { margin:0; color:#6b7280; line-height:1.6; font-size:14px; }
        .gh-present-links { display:grid; gap:10px; margin-top:16px; }
        .gh-present-link { display:flex; align-items:center; justify-content:space-between; gap:12px; padding:12px 14px; border-radius:14px; background:#faf7ff; border:1px solid #ede9fe; text-decoration:none!important; color:#1f2937; }
        .gh-present-link small { display:block; color:#7c3aed; font-size:11px; text-transform:uppercase; font-weight:700; margin-bottom:2px; }
        .gh-present-link strong { display:block; font-size:15px; line-height:1.35; }
        .gh-present-link span { display:block; margin-top:3px; color:#6b7280; font-size:13px; line-height:1.5; }
        .gh-present-link em { font-style:normal; font-weight:900; color:#7c3aed; }
        .gh-present-note { margin-top:16px; padding:14px 16px; border-radius:16px; background:#faf5ff; border:1px solid #e9d5ff; color:#6b21a8; font-size:14px; line-height:1.6; }
        @media (max-width:991px) { .gh-present-title{font-size:26px;} }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-present-shell">
        <section class="gh-present-hero">
            <div>
                <div class="gh-present-kicker">Storefront /gianhang</div>
                <h1 class="gh-present-title">Trình bày storefront</h1>
                <p class="gh-present-sub">
                    Khu này gom các phần đang quyết định storefront hiển thị thế nào: menu, slider, section, thông tin thương hiệu và các nút điều hướng công khai.
                    Mình dùng nó như một lớp wrapper để người vận hành level 2 không phải nhảy rời rạc giữa nhiều màn legacy.
                </p>
            </div>
            <div class="gh-present-actions">
                <a class="gh-present-btn gh-present-btn--primary" href="<%=StorefrontConfigUrl %>">Cấu hình storefront</a>
                <a class="gh-present-btn gh-present-btn--soft" href="<%=MenuUrl %>">Quản lý menu</a>
                <a class="gh-present-btn gh-present-btn--soft" href="<%=SliderUrl %>">Quản lý slider</a>
                <a class="gh-present-btn gh-present-btn--soft" href="<%=PublicPreviewUrl %>">Preview trong admin</a>
                <a class="gh-present-btn gh-present-btn--soft" href="<%=PublicUrl %>" target="_blank">Mở public</a>
            </div>
        </section>

        <div class="gh-present-stats">
            <div class="gh-present-stat">
                <small>Menu</small>
                <strong><%=MenuCount.ToString("#,##0") %></strong>
                <span>Các nhóm điều hướng đang dùng cho storefront.</span>
            </div>
            <div class="gh-present-stat">
                <small>Slider</small>
                <strong><%=SliderCount.ToString("#,##0") %></strong>
                <span>Ảnh hero/cover đang dùng ở lớp trình bày.</span>
            </div>
            <div class="gh-present-stat">
                <small>Section hiển thị</small>
                <strong><%=VisibleSectionCount.ToString("#,##0") %></strong>
                <span><%=TotalSectionCount.ToString("#,##0") %> section đã cấu hình trong workspace.</span>
            </div>
            <div class="gh-present-stat">
                <small>Bài viết public</small>
                <strong><%=ArticleCount.ToString("#,##0") %></strong>
                <span>Nguồn bài tư vấn/SEO đang hiển thị công khai.</span>
            </div>
            <div class="gh-present-stat">
                <small>Chế độ storefront</small>
                <strong><%=StorefrontModeLabel %></strong>
                <span>Mode lấy từ cấu hình storefront hiện tại.</span>
            </div>
            <div class="gh-present-stat">
                <small>Ghi chú thương hiệu</small>
                <strong><%=BrandNoteStatus %></strong>
                <span>Dùng để nhìn nhanh phần branding cốt lõi đã có hay chưa.</span>
            </div>
        </div>

        <div class="gh-present-grid">
            <section class="gh-present-card">
                <h3>Bố cục & section</h3>
                <p>Đây là lớp quyết định trang công khai đang hiển thị khối nào trước, khối nào sau và mỗi khối đang lấy dữ liệu từ đâu.</p>
                <div class="gh-present-links">
                    <a class="gh-present-link" href="<%=StorefrontConfigUrl %>">
                        <div>
                            <small>Section storefront</small>
                            <strong>Cấu hình storefront</strong>
                            <span>Chỉnh mode hiển thị, quick strip, hero, footer và từng section.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                    <a class="gh-present-link" href="<%=StorefrontPreviewUrl %>">
                        <div>
                            <small>Xem trong admin</small>
                            <strong>Trang công khai</strong>
                            <span>Preview ngay storefront public để đối chiếu trước/sau khi chỉnh.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                    <a class="gh-present-link" href="<%=MenuUrl %>">
                        <div>
                            <small>Điều hướng</small>
                            <strong>Quản lý menu</strong>
                            <span>Cây menu dùng chung cho bài viết, dịch vụ, sản phẩm và điều hướng public.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                </div>
            </section>

            <section class="gh-present-card">
                <h3>Thương hiệu & nhúng mã</h3>
                <p>Đây là phần tác động trực tiếp tới nhận diện storefront, social proof và các mã tích hợp ngoài như chatbot, pixel hay script chia sẻ.</p>
                <div class="gh-present-links">
                    <a class="gh-present-link" href="<%=BrandSettingsUrl %>">
                        <div>
                            <small>Thông tin thương hiệu</small>
                            <strong>Cập nhật thông tin</strong>
                            <span>Tên, địa chỉ, liên hệ, logo và các thông tin nền của storefront.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                    <a class="gh-present-link" href="<%=SocialSettingsUrl %>">
                        <div>
                            <small>Social proof</small>
                            <strong>Link social media</strong>
                            <span>Kết nối mạng xã hội để storefront có chỗ bám niềm tin tốt hơn.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                    <a class="gh-present-link" href="<%=EmbedSettingsUrl %>">
                        <div>
                            <small>Tracking / chatbot</small>
                            <strong>Nhúng mã vào website</strong>
                            <span>Điểm đặt script tích hợp cho lớp public storefront.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                </div>
            </section>

            <section class="gh-present-card">
                <h3>Media & nội dung nguồn</h3>
                <p>Ngoài bố cục, storefront còn phụ thuộc rất mạnh vào ảnh slide, bài viết public và nội dung nguồn mà khách nhìn thấy đầu tiên.</p>
                <div class="gh-present-links">
                    <a class="gh-present-link" href="<%=SliderUrl %>">
                        <div>
                            <small>Hero media</small>
                            <strong>Quản lý slider</strong>
                            <span>Ảnh cover/hero đang điều khiển điểm nhìn đầu tiên trên storefront.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                    <a class="gh-present-link" href="<%=ContentUrl %>">
                        <div>
                            <small>Sản phẩm · dịch vụ</small>
                            <strong>Nội dung /gianhang</strong>
                            <span>Nguồn sản phẩm/dịch vụ native và bridge sang admin để vận hành.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                    <a class="gh-present-link" href="<%=ArticleHubUrl %>">
                        <div>
                            <small>Bài viết public</small>
                            <strong>Bài viết /gianhang</strong>
                            <span>Nhìn lớp bài public, trạng thái hiển thị và mirror admin ngay trong workspace.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                    <a class="gh-present-link" href="<%=ArticleListUrl %>" target="_blank">
                        <div>
                            <small>Nội dung công khai</small>
                            <strong>Danh sách bài viết public</strong>
                            <span>Mở trực tiếp lớp bài viết đang ra ngoài storefront.</span>
                        </div>
                        <em>&rsaquo;</em>
                    </a>
                </div>
                <div class="gh-present-note">
                    Mục tiêu của lớp này là gom các màn legacy liên quan tới trình bày về cùng một ngữ cảnh workspace.
                    Vậy nên sau level 2, người dùng vẫn cảm thấy đang chỉnh <code>/gianhang</code> chứ không bị “rơi” sang một hệ khác.
                </div>
            </section>
        </div>
    </div>
</asp:Content>
