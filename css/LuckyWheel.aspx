<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LuckyWheel.aspx.cs" Inherits="LuckyWheel" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <title>Vòng xoay may mắn</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        :root {
            --bg: #0f172a; --muted: #94a3b8; --accent: #22d3ee; --accent2: #f472b6;
            --success: #34d399; --danger: #fb7185;
        }
        *{box-sizing:border-box}
        body{
            margin:0;font-family:"Be Vietnam Pro","Segoe UI",Arial;
            background: radial-gradient(1000px 600px at 50% -10%, #1f2937 0%, var(--bg) 55%);
            color:#e5e7eb; min-height:100vh; display:flex; align-items:center; justify-content:center; padding:16px;
        }
        .container{width:100%;max-width:1100px;display:grid;grid-template-columns:1.2fr 1fr;gap:20px}
        @media (max-width:767px){.container{grid-template-columns:1fr}}
        .card{background:linear-gradient(180deg,rgba(255,255,255,.04),rgba(255,255,255,.02));
              border:1px solid rgba(255,255,255,.06);border-radius:16px;padding:18px;box-shadow:0 10px 30px rgba(0,0,0,.35)}
        .title{margin:0 0 12px;font-size:20px;font-weight:700}
        .muted{color:var(--muted)}
        .input-wrap{display:flex;gap:10px}
        .input-wrap input{flex:1;padding:12px 14px;border-radius:10px;border:1px solid rgba(255,255,255,.12);background:#0b1220;color:#e5e7eb}
        .btn{border:0;padding:12px 16px;border-radius:10px;font-weight:700;cursor:pointer;
             background:linear-gradient(135deg,var(--accent),var(--accent2));color:#061016;box-shadow:0 8px 18px rgba(34,211,238,.25)}
        .btn.secondary{background:transparent;color:#e5e7eb;border:1px solid rgba(255,255,255,.14);box-shadow:none}
        .btn[disabled]{opacity:.6;cursor:not-allowed}
        .names{margin-top:12px;display:flex;flex-wrap:wrap;gap:8px;max-height:170px;overflow:auto}
        .chip{display:inline-flex;align-items:center;gap:8px;padding:8px 10px;border-radius:999px;font-size:14px;
              background:rgba(255,255,255,.06);border:1px solid rgba(255,255,255,.1)}
        .chip button{background:transparent;border:0;color:#fff;cursor:pointer;font-size:14px}
        .wheel-wrap{position:relative;display:grid;place-items:center}
        .canvas-wrap{position:relative;width:100%;aspect-ratio:1/1;max-width:560px}
        canvas{width:100%;height:100%;border-radius:50%}
        .pin{
            position:absolute;top:50%;right:-14px;transform:translateY(-50%);
            width:0;height:0;border-left:18px solid var(--danger);border-top:12px solid transparent;border-bottom:12px solid transparent;
        }
        @media (max-width:767px){
            .pin{left:50%;right:auto;top:-6px;transform:translate(-50%,0) rotate(90deg)}
        }
        .actions{display:flex;gap:10px;margin-top:14px;flex-wrap:wrap}
        .toast{position:fixed;inset:auto 16px 16px 16px;margin:0 auto;background:#05151a;border:1px solid rgba(255,255,255,.12);
               padding:14px 16px;border-radius:12px;max-width:520px;display:none}
        .toast.show{display:block;animation:pop .25s ease-out}
        @keyframes pop{from{transform:translateY(6px);opacity:0}to{transform:translateY(0);opacity:1}}
        #confetti{position:fixed;inset:0;pointer-events:none;z-index:50}
        .hint{font-size:13px;margin-top:10px;color:var(--muted)}
    </style>
</head>
<body>
<form id="form1" runat="server">
    <!-- Bật PageMethods để gọi WebMethod từ JS -->
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
    <asp:UpdatePanel ID="updPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="container">
                <!-- Bánh xe -->
                <div class="card">
                    <h2 class="title">🎡 Vòng xoay may mắn</h2>
                    <div class="wheel-wrap">
                        <div class="canvas-wrap">
                            <canvas id="wheel" width="900" height="900"></canvas>
                            <div class="pin"></div>
                        </div>
                        <div class="actions">
                            <button type="button" class="btn" id="btnSpin">Bắt đầu quay</button>
                            <button type="button" class="btn secondary" id="btnShuffle">Xáo trộn màu</button>
                            <button type="button" class="btn secondary" id="btnClear">Xoá tất cả</button>
                        </div>
                        <div class="hint">Nhập tên ở khung bên phải, nhấn Enter để thêm.</div>
                    </div>
                </div>

                <!-- Nhập tên -->
                <div class="card">
                    <h2 class="title">👥 Người chơi</h2>
                    <div class="input-wrap">
                        <input id="txtName" type="text" placeholder="Ví dụ: An, Bình, Chi..." />
                        <button type="button" class="btn" id="btnAdd">Thêm</button>
                    </div>
                    <div class="names" id="nameList"></div>
                    <asp:HiddenField ID="hdnNames" runat="server" />
                </div>
            </div>

            <div class="toast" id="toast"></div>
            <canvas id="confetti"></canvas>
        </ContentTemplate>
    </asp:UpdatePanel>
</form>

<script>
    /* ---------- State & refs ---------- */
    const names = [];
    let colors = [];
    let spinning = false;
    let currentAngle = 0;
    const wheel = document.getElementById('wheel');
    const ctx = wheel.getContext('2d');
    const nameListEl = document.getElementById('nameList');
    const toast = document.getElementById('toast');
    const hdnNames = document.getElementById('<%= hdnNames.ClientID %>');

    /* ---------- Math/Utils ---------- */
    const TWO_PI = Math.PI * 2;
    const mod = (x, m) => ((x % m) + m) % m;
    // 0 rad = mũi tên bên phải; 3π/2 rad = mũi tên ở trên (chỉ xuống)
    const pinAngle = () => (window.innerWidth <= 767 ? (3 * Math.PI / 2) : 0);

    const rand = (a, b) => Math.random() * (b - a) + a;
    const easeOutCubic = t => 1 - Math.pow(1 - t, 3);
    const randomPalette = n => {
        const base = Math.floor(Math.random() * 360);
        return Array.from({ length: n }, (_, i) => `hsl(${(base + i * 360 / n) % 360}deg 80% 55%)`);
    };
    const escapeHtml = s => s.replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));

    /* ---------- Toast (giữ hoài, ẩn khi quay lượt mới) ---------- */
    function hideToast() {
        toast.classList.remove('show');
        if (toast._timer) { clearTimeout(toast._timer); toast._timer = null; }
    }
    function showToast(html, duration = 0) {
        toast.innerHTML = html;
        toast.classList.add('show');
        if (toast._timer) { clearTimeout(toast._timer); toast._timer = null; }
        if (duration > 0) { toast._timer = setTimeout(() => hideToast(), duration); }
    }

    /* ---------- Confetti ---------- */
    const confettiCanvas = document.getElementById('confetti');
    const cfx = confettiCanvas.getContext('2d');
    function resizeConfetti() { confettiCanvas.width = innerWidth; confettiCanvas.height = innerHeight; }
    resizeConfetti(); addEventListener('resize', resizeConfetti);
    function runConfetti(duration = 2200) {
        const parts = Array.from({ length: 220 }, () => ({
            x: Math.random() * confettiCanvas.width, y: -10 - Math.random() * 60,
            vx: rand(-1.2, 1.2), vy: rand(2.5, 5.5), s: rand(3, 7),
            r: rand(0, TWO_PI), vr: rand(-0.2, 0.2),
            c: `hsl(${Math.floor(Math.random() * 360)}deg 85% 60%)`
        }));
        const st = performance.now();
        (function loop(t0) {
            const now = performance.now(), dt = (now - (t0 || now)) / 16;
            cfx.clearRect(0, 0, confettiCanvas.width, confettiCanvas.height);
            parts.forEach(p => {
                p.x += p.vx * dt; p.y += p.vy * dt; p.r += p.vr * dt;
                if (p.y > confettiCanvas.height + 20) p.y = -10;
                cfx.save(); cfx.translate(p.x, p.y); cfx.rotate(p.r);
                cfx.fillStyle = p.c; cfx.fillRect(-p.s / 2, -p.s / 2, p.s, p.s); cfx.restore();
            });
            if (now - st < duration) requestAnimationFrame(() => loop(now));
            else cfx.clearRect(0, 0, confettiCanvas.width, confettiCanvas.height);
        })();
    }

    /* ---------- Names UI ---------- */
    function syncHidden() { hdnNames.value = JSON.stringify(names); }
    function renderNames() {
        nameListEl.innerHTML = '';
        names.forEach((n, i) => {
            const chip = document.createElement('span');
            chip.className = 'chip';
            chip.innerHTML = `<span>${escapeHtml(n)}</span><button title="Xoá">&times;</button>`;
            chip.querySelector('button').onclick = () => { names.splice(i, 1); colors.splice(i, 1); syncHidden(); draw(); renderNames(); };
            nameListEl.appendChild(chip);
        });
    }
    function addName(v) {
        v = (v || '').trim(); if (!v) return;
        names.push(v); colors = randomPalette(names.length);
        syncHidden(); draw(); renderNames();
    }
    function clearAll() { names.length = 0; colors.length = 0; syncHidden(); draw(); renderNames(); }

    /* ---------- Wheel drawing ---------- */
    function wrapText(ctx, text, x, y, maxWidth, lineHeight) {
        const words = (text || '').split(' '); let line = '', lines = [];
        for (let w of words) {
            const test = line ? line + ' ' + w : w;
            if (ctx.measureText(test).width > maxWidth && line) { lines.push(line); line = w; }
            else line = test;
        }
        lines.push(line);
        const oy = -(lines.length - 1) * lineHeight / 2;
        lines.forEach((ln, i) => ctx.fillText(ln, x, y + oy + i * lineHeight));
    }
    function draw() {
        const N = names.length || 8, r = wheel.width / 2;
        ctx.clearRect(0, 0, wheel.width, wheel.height);
        ctx.save(); ctx.translate(r, r); ctx.rotate(currentAngle);
        for (let i = 0; i < N; i++) {
            const a0 = i * TWO_PI / N, a1 = (i + 1) * TWO_PI / N;
            ctx.beginPath(); ctx.moveTo(0, 0); ctx.arc(0, 0, r - 8, a0, a1); ctx.closePath();
            ctx.fillStyle = names.length ? colors[i] : `hsl(${(i * 360 / N) | 0}deg 20% 22%)`;
            ctx.fill(); ctx.strokeStyle = 'rgba(255,255,255,.08)'; ctx.lineWidth = 2; ctx.stroke();
            // text
            ctx.save(); const mid = (a0 + a1) / 2; ctx.rotate(mid); ctx.translate((r - 8) * 0.62, 0); ctx.rotate(Math.PI / 2);
            ctx.fillStyle = '#fff'; ctx.font = 'bold ' + Math.max(20, Math.min(34, r * 0.06)) + 'px system-ui';
            ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
            const label = names.length ? names[i] : '...';
            wrapText(ctx, label, 0, 0, (r - 60) * 0.9, Math.max(20, r * 0.05));
            ctx.restore();
        }
        // center
        ctx.beginPath(); ctx.arc(0, 0, r * 0.14, 0, TWO_PI); ctx.fillStyle = '#0b1220'; ctx.fill();
        ctx.lineWidth = 3; ctx.strokeStyle = 'rgba(255,255,255,.18)'; ctx.stroke();
        ctx.restore();
    }

    /* ---------- Application force index (PageMethods) ---------- */
    function getForcedIndexOnce() {
        return new Promise(resolve => {
            if (!window.PageMethods || !PageMethods.GetForcedWinnerIndexAndConsume) { resolve(-1); return; }
            PageMethods.GetForcedWinnerIndexAndConsume(
                res => resolve(typeof res === 'number' ? res : -1),
                _ => resolve(-1)
            );
        });
    }

    /* ---------- Spin ---------- */
    async function spin() {
        if (spinning) return;
        if (names.length < 2) { showToast('Cần ít nhất <strong>2</strong> người chơi để quay.', 5000); return; }

        hideToast(); // ẩn thông báo cũ khi quay lượt mới
        spinning = true; document.getElementById('btnSpin').disabled = true;

        const N = names.length;

        // lấy số cơ cấu (1-based) từ Application qua WebMethod, dùng 1 lần
        let forced1Based = await getForcedIndexOnce();

        let win;
        if (typeof forced1Based === 'number' && forced1Based >= 1 && forced1Based <= N) {
            // Chỉ dùng số cơ cấu khi nó hợp lệ trong danh sách
            win = forced1Based - 1;
        } else {
            // Nếu số cơ cấu không hợp lệ (<1 hoặc >N) hoặc không có -> quay ngẫu nhiên
            win = Math.floor(Math.random() * N);
        }




        const sector = TWO_PI / N;
        const mid = (win + 0.5) * sector;
        const extra = 8 + Math.floor(Math.random() * 3);
        const pinAng = pinAngle();
        const delta = extra * TWO_PI + mod(pinAng - mid - (currentAngle % TWO_PI), TWO_PI);

        const dur = 5200 + Math.random() * 800;
        const st = performance.now(); const start = currentAngle;

        return new Promise(res => {
            (function anim() {
                const now = performance.now(), t = Math.min(1, (now - st) / dur), e = easeOutCubic(t);
                currentAngle = start + delta * e; draw();
                if (t < 1) requestAnimationFrame(anim);
                else {
                    currentAngle = start + delta; draw();
                    spinning = false; document.getElementById('btnSpin').disabled = false;
                    const w = names[win];
                    showToast(`🎉 <strong>Chúc mừng</strong> <strong style="color:var(--success)">${escapeHtml(w)}</strong>!`);
                    runConfetti(2500);
                    res(w);
                }
            })();
        });
    }

    /* ---------- Events ---------- */
    document.getElementById('btnAdd').onclick = () => { const ip = txtName; addName(ip.value); ip.value = ''; ip.focus(); };
    document.getElementById('txtName').addEventListener('keydown', e => {
        if (e.key === 'Enter') { e.preventDefault(); addName(e.target.value); e.target.value = ''; }
    });
    document.getElementById('btnSpin').onclick = spin;
    document.getElementById('btnShuffle').onclick = () => { if (!names.length) return; colors = randomPalette(names.length); draw(); };
    document.getElementById('btnClear').onclick = () => { clearAll(); showToast('Đã xoá danh sách.'); };

    draw();
</script>
</body>
</html>
