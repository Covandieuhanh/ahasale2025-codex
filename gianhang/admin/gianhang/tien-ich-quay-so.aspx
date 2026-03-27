<%@ Page Title="Quay số /gianhang" Language="C#" MasterPageFile="~/gianhang/admin/mp-admin.master" AutoEventWireup="true" CodeFile="tien-ich-quay-so.aspx.cs" Inherits="gianhang_admin_gianhang_tien_ich_quay_so" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .gh-spin-shell{display:grid;gap:18px;}
        .gh-spin-hero,.gh-spin-card{padding:22px;border:1px solid #efe4e5;border-radius:22px;background:#fff;box-shadow:0 16px 28px rgba(15,23,42,.05);}
        .gh-spin-hero{display:flex;justify-content:space-between;gap:16px;flex-wrap:wrap;background:radial-gradient(680px 220px at 0% 0%, rgba(255,111,72,.14), transparent 60%),linear-gradient(180deg,#fffdfc 0%,#fff 100%);}
        .gh-spin-kicker{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#fff3ee;border:1px solid #ffd8c7;color:#bb4d19;font-size:12px;font-weight:800;text-transform:uppercase;}
        .gh-spin-title{margin:10px 0 6px;color:#7f1d1d;font-size:30px;font-weight:900;line-height:1.15;}
        .gh-spin-sub{margin:0;color:#8d5d5d;font-size:14px;line-height:1.7;max-width:760px;}
        .gh-spin-actions{display:flex;gap:10px;flex-wrap:wrap;}
        .gh-spin-btn{display:inline-flex;align-items:center;justify-content:center;min-height:42px;padding:0 16px;border-radius:14px;text-decoration:none !important;font-size:13px;font-weight:800;border:1px solid transparent;}
        .gh-spin-btn--primary{background:linear-gradient(135deg,#d73a31,#ef6b41);color:#fff !important;box-shadow:0 14px 28px rgba(215,58,49,.18);}
        .gh-spin-btn--soft{background:#fff;color:#7f1d1d !important;border-color:#f2c9c9;}
        .gh-spin-grid{display:grid;grid-template-columns:minmax(0,2fr) minmax(320px,1fr);gap:16px;}
        .gh-spin-stage{display:grid;gap:16px;}
        .gh-spin-result{display:flex;align-items:center;justify-content:center;min-height:180px;border-radius:24px;background:linear-gradient(135deg,#7f1d1d,#ef6b41);color:#fff;font-size:42px;font-weight:900;text-align:center;padding:16px;line-height:1.2;box-shadow:0 24px 40px rgba(127,29,29,.18);}
        .gh-spin-toolbar{display:flex;gap:10px;flex-wrap:wrap;}
        .gh-spin-toolbar button{appearance:none;border:0;display:inline-flex;align-items:center;justify-content:center;min-height:46px;padding:0 18px;border-radius:14px;font-size:13px;font-weight:800;cursor:pointer;}
        .gh-spin-toolbar .start{background:linear-gradient(135deg,#d73a31,#ef6b41);color:#fff;box-shadow:0 14px 28px rgba(215,58,49,.18);}
        .gh-spin-toolbar .stop{background:#fff3ee;color:#bb4d19;border:1px solid #ffd8c7;}
        .gh-spin-toolbar .reset{background:#fff;border:1px solid #e6d6d6;color:#7f1d1d;}
        .gh-spin-input{width:100%;min-height:280px;padding:16px;border-radius:18px;border:1px solid #f0d3d7;background:#fff;box-shadow:inset 0 1px 2px rgba(15,23,42,.03);font-size:14px;line-height:1.7;color:#1f2937;outline:none;resize:vertical;}
        .gh-spin-note{color:#6b7280;font-size:13px;line-height:1.7;}
        .gh-spin-stat{padding:16px;border:1px solid #ebeef4;border-radius:18px;background:#f8fafc;}
        .gh-spin-stat small{display:block;color:#8d5d5d;font-size:12px;font-weight:700;text-transform:uppercase;}
        .gh-spin-stat strong{display:block;margin-top:8px;color:#111827;font-size:26px;font-weight:900;}
        .gh-spin-stat span{display:block;margin-top:6px;color:#6b7280;font-size:13px;line-height:1.7;}
        .gh-spin-list{margin:14px 0 0;padding-left:18px;color:#4b5563;line-height:1.7;}
        .gh-spin-chip{display:inline-flex;align-items:center;min-height:30px;padding:0 12px;border-radius:999px;background:#fff3ee;border:1px solid #ffd8c7;color:#a8431f;font-size:12px;font-weight:800;}
        @media (max-width: 991px){.gh-spin-grid{grid-template-columns:1fr;}.gh-spin-title{font-size:26px;}.gh-spin-result{font-size:34px;min-height:150px;}}
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="main" runat="server">
    <div class="gh-spin-shell" data-cocau-index="<%=CoCauIndex.ToString() %>">
        <section class="gh-spin-hero">
            <div>
                <div class="gh-spin-kicker">Tiện ích · quay số</div>
                <h1 class="gh-spin-title">Quay số nội bộ cho <%=WorkspaceDisplayName %></h1>
                <p class="gh-spin-sub">
                    Nhập danh sách tên theo từng dòng rồi bắt đầu quay. Công cụ này chạy mượt ngay trong admin, không cần postback liên tục.
                    <%=CurrentCoCauNote %>
                </p>
            </div>
            <div class="gh-spin-actions">
                <a class="gh-spin-btn gh-spin-btn--primary" href="<%=ConfigUrl %>">Cấu hình cơ cấu</a>
                <a class="gh-spin-btn gh-spin-btn--soft" href="<%=HubUrl %>">Về tiện ích</a>
                <a class="gh-spin-btn gh-spin-btn--soft" href="<%=PublicDrawUrl %>" target="_blank">Mở native /gianhang</a>
            </div>
        </section>

        <div class="gh-spin-grid">
            <section class="gh-spin-stage">
                <div class="gh-spin-result" id="gh-spin-result">CHƯA BẮT ĐẦU</div>
                <div class="gh-spin-toolbar">
                    <button type="button" class="start" id="gh-spin-start">Bắt đầu</button>
                    <button type="button" class="stop" id="gh-spin-stop" disabled>Dừng lại</button>
                    <button type="button" class="reset" id="gh-spin-reset">Xóa kết quả</button>
                </div>
                <div>
                    <textarea id="gh-spin-input" class="gh-spin-input" placeholder="Nhập mỗi tên trên một dòng. Ví dụ:
Nguyễn Văn A
Trần Thị B
Lê Văn C"></textarea>
                    <div class="gh-spin-note">
                        Danh sách được xử lý ngay trên trình duyệt. Nếu workspace đang có cơ cấu, kết quả dừng sẽ ưu tiên theo đúng dòng đã cấu hình.
                    </div>
                </div>
            </section>

            <aside class="gh-spin-card">
                <div class="gh-spin-stat">
                    <small>Trạng thái cơ cấu</small>
                    <strong><%=CurrentCoCauLabel %></strong>
                    <span><%=CurrentCoCauNote %></span>
                </div>
                <div class="gh-spin-stat" style="margin-top:14px;">
                    <small>Số dòng hiện nhập</small>
                    <strong id="gh-spin-count">0</strong>
                    <span>Đếm ngay theo số dòng có dữ liệu trong ô nhập.</span>
                </div>
                <div style="margin-top:14px;">
                    <span class="gh-spin-chip">Workspace scoped</span>
                </div>
                <ul class="gh-spin-list">
                    <li>Dữ liệu quay số không rời khỏi trình duyệt của mình.</li>
                    <li>Khi dừng, công cụ chỉ đọc cấu hình cơ cấu hiện tại của workspace.</li>
                    <li>Nếu cơ cấu vượt quá số dòng hiện có, hệ thống sẽ quay về chế độ ngẫu nhiên.</li>
                </ul>
            </aside>
        </div>
    </div>

    <script>
        (function () {
            var root = document.querySelector('.gh-spin-shell');
            if (!root) return;

            var input = document.getElementById('gh-spin-input');
            var result = document.getElementById('gh-spin-result');
            var count = document.getElementById('gh-spin-count');
            var start = document.getElementById('gh-spin-start');
            var stop = document.getElementById('gh-spin-stop');
            var reset = document.getElementById('gh-spin-reset');
            var timer = null;
            var current = [];
            var cocau = parseInt(root.getAttribute('data-cocau-index') || '0', 10);

            function getItems() {
                return (input.value || '')
                    .split(/\r?\n/)
                    .map(function (item) { return item.trim(); })
                    .filter(function (item) { return item.length > 0; });
            }

            function updateCount() {
                current = getItems();
                count.textContent = String(current.length);
                if (!timer && current.length === 0) {
                    result.textContent = 'CHƯA CÓ DỮ LIỆU';
                }
            }

            function pickRandom(items) {
                return items[Math.floor(Math.random() * items.length)];
            }

            input.addEventListener('input', updateCount);
            updateCount();

            start.addEventListener('click', function () {
                current = getItems();
                if (current.length === 0) {
                    result.textContent = 'VUI LÒNG NHẬP DỮ LIỆU';
                    return;
                }

                if (timer) window.clearInterval(timer);
                timer = window.setInterval(function () {
                    result.textContent = pickRandom(current);
                }, 60);
                start.disabled = true;
                stop.disabled = false;
            });

            stop.addEventListener('click', function () {
                if (timer) {
                    window.clearInterval(timer);
                    timer = null;
                }
                current = getItems();
                if (current.length === 0) {
                    result.textContent = 'CHƯA CÓ DỮ LIỆU';
                } else if (!isNaN(cocau) && cocau > 0 && cocau <= current.length) {
                    result.textContent = current[cocau - 1];
                } else {
                    result.textContent = pickRandom(current);
                }
                start.disabled = false;
                stop.disabled = true;
            });

            reset.addEventListener('click', function () {
                if (timer) {
                    window.clearInterval(timer);
                    timer = null;
                }
                result.textContent = 'CHƯA BẮT ĐẦU';
                start.disabled = false;
                stop.disabled = true;
            });
        })();
    </script>
</asp:Content>
