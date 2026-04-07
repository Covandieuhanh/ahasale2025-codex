<%@ Page Title="Tiện ích /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="tien-ich.aspx.cs" Inherits="gianhang_admin_gianhang_tien_ich_default" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-util-shell{display:grid;gap:18px;}
        .gh-util-hero{display:flex;justify-content:space-between;gap:18px;flex-wrap:wrap;padding:22px;border:1px solid #f5d8cd;border-radius:22px;background:radial-gradient(680px 220px at 0% 0%, rgba(255,111,72,.16), transparent 60%),linear-gradient(180deg,#fffdfc 0%,#fff 100%);box-shadow:0 18px 34px rgba(127,29,29,.08);}
        .gh-util-kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#fff3ee;border:1px solid #ffd8c7;color:#bb4d19;font-size:12px;font-weight:800;text-transform:uppercase;letter-spacing:.04em;}
        .gh-util-title{margin:10px 0 6px;color:#7f1d1d;font-size:30px;line-height:1.15;font-weight:900;}
        .gh-util-sub{margin:0;max-width:760px;color:#8d5d5d;font-size:14px;line-height:1.7;}
        .gh-util-actions{display:flex;gap:10px;flex-wrap:wrap;}
        .gh-util-btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;border:1px solid transparent;text-decoration:none !important;font-size:13px;font-weight:800;}
        .gh-util-btn--primary{background:linear-gradient(135deg,#d73a31,#ef6b41);color:#fff !important;box-shadow:0 14px 28px rgba(215,58,49,.18);}
        .gh-util-btn--soft{background:#fff;color:#7f1d1d !important;border-color:#f2c9c9;}
        .gh-util-kpis{display:grid;grid-template-columns:repeat(auto-fit,minmax(180px,1fr));gap:12px;}
        .gh-util-kpi{padding:16px;border:1px solid #ebeef4;border-radius:18px;background:#fff;box-shadow:0 14px 28px rgba(15,23,42,.05);}
        .gh-util-kpi small{display:block;color:#8d5d5d;font-size:12px;font-weight:700;text-transform:uppercase;}
        .gh-util-kpi strong{display:block;margin-top:8px;color:#111827;font-size:26px;font-weight:900;}
        .gh-util-kpi span{display:block;margin-top:6px;color:#6b7280;font-size:13px;line-height:1.6;}
        .gh-util-grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(260px,1fr));gap:14px;}
        .gh-util-card{padding:18px;border:1px solid #ebeef4;border-radius:18px;background:#fff;box-shadow:0 16px 30px rgba(15,23,42,.05);}
        .gh-util-card--accent{border-color:#ffd8c7;background:linear-gradient(180deg,#fff8f4 0%,#fff 100%);}
        .gh-util-card h3{margin:0 0 8px;color:#1f2937;font-size:18px;}
        .gh-util-card p{margin:0;color:#6b7280;font-size:14px;line-height:1.7;}
        .gh-util-list{margin:14px 0 0;padding-left:18px;color:#4b5563;line-height:1.7;}
        .gh-util-links{display:grid;gap:10px;margin-top:14px;}
        .gh-util-link{display:flex;align-items:center;justify-content:space-between;gap:12px;padding:12px 14px;border-radius:14px;background:#f8fafc;border:1px solid #edf2f7;color:#1f2937;text-decoration:none !important;font-weight:700;}
        .gh-util-link small{display:block;color:#8d5d5d;font-size:11px;font-weight:700;text-transform:uppercase;margin-bottom:2px;}
        .gh-util-link span{display:block;color:#6b7280;font-size:13px;font-weight:600;}
        @media (max-width: 767px){.gh-util-title{font-size:26px;}}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-util-shell">
        <section class="gh-util-hero">
            <div>
                <div class="gh-util-kicker">Tiện ích workspace</div>
                <h1 class="gh-util-title">Tiện ích /gianhang · <%=WorkspaceDisplayName %></h1>
                <p class="gh-util-sub">
                    Đây là lớp tiện ích bổ sung của chính workspace <code>/gianhang</code>, mở ngay trong <code>/gianhang/admin</code> để khi lên level 2 mình không phải nhảy ra ngoài.
                    Hai công cụ đầu tiên đã được gom vào cùng ngữ cảnh workspace là <strong>cơ cấu</strong> và <strong>quay số</strong>.
                </p>
            </div>
            <div class="gh-util-actions">
                <a class="gh-util-btn gh-util-btn--primary" href="<%=DrawUrl %>">Mở quay số</a>
                <a class="gh-util-btn gh-util-btn--soft" href="<%=ConfigUrl %>">Cấu hình cơ cấu</a>
                <a class="gh-util-btn gh-util-btn--soft" href="<%=HubUrl %>">Quay lại hub /gianhang</a>
            </div>
        </section>

        <div class="gh-util-kpis">
            <div class="gh-util-kpi">
                <small>Trạng thái cơ cấu</small>
                <strong><%=CurrentCoCauLabel %></strong>
                <span><%=CurrentCoCauNote %></span>
            </div>
            <div class="gh-util-kpi">
                <small>Phạm vi áp dụng</small>
                <strong>Riêng workspace</strong>
                <span>Không còn dùng một chỉ số cấu hình chung cho toàn hệ như trước nữa.</span>
            </div>
            <div class="gh-util-kpi">
                <small>Công cụ đã có</small>
                <strong>2</strong>
                <span>Cơ cấu kết quả và quay số ngẫu nhiên đã được gom vào admin-native.</span>
            </div>
        </div>

        <div class="gh-util-grid">
            <section class="gh-util-card gh-util-card--accent">
                <h3>Cấu hình cơ cấu</h3>
                <p>Thiết lập sẵn dòng ưu tiên khi dừng quay số. Nếu tắt cơ cấu, công cụ quay số sẽ chọn ngẫu nhiên hoàn toàn.</p>
                <ul class="gh-util-list">
                    <li>Dùng riêng cho workspace hiện tại.</li>
                    <li>Dễ bật/tắt ngay trong admin.</li>
                    <li>Giữ cùng logic với công cụ native <code>/gianhang</code>.</li>
                </ul>
                <div class="gh-util-links">
                    <a class="gh-util-link" href="<%=ConfigUrl %>">
                        <div>
                            <small>Thiết lập</small>
                            Cơ cấu kết quả
                            <span>Cài số thứ tự ưu tiên hoặc tắt hẳn cơ cấu.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-util-link" href="<%=PublicConfigUrl %>" target="_blank">
                        <div>
                            <small>Native /gianhang</small>
                            Mở công cụ gốc
                            <span>Mở trang tiện ích cũ của gian hàng với cùng ngữ cảnh workspace.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                </div>
            </section>

            <section class="gh-util-card">
                <h3>Quay số ngẫu nhiên</h3>
                <p>Nhập danh sách tên, bắt đầu quay và dừng ngay trong admin. Nếu workspace đang có cơ cấu, kết quả sẽ ưu tiên theo cấu hình đó.</p>
                <ul class="gh-util-list">
                    <li>Chạy mượt hoàn toàn bằng client-side.</li>
                    <li>Không postback liên tục như công cụ cũ.</li>
                    <li>Phù hợp để dùng nhanh trong các hoạt động nội bộ.</li>
                </ul>
                <div class="gh-util-links">
                    <a class="gh-util-link" href="<%=DrawUrl %>">
                        <div>
                            <small>Công cụ admin-native</small>
                            Quay số trong admin
                            <span>Mở công cụ quay số đã tối ưu cho trải nghiệm thao tác trong <code>/gianhang/admin</code>.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                    <a class="gh-util-link" href="<%=PublicDrawUrl %>" target="_blank">
                        <div>
                            <small>Native /gianhang</small>
                            Mở bản gốc
                            <span>Mở trang quay số cũ của gian hàng với cùng ngữ cảnh workspace.</span>
                        </div>
                        <strong>&rsaquo;</strong>
                    </a>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
