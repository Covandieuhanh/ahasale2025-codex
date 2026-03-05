<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Footer_uc.ascx.cs" Inherits="Uc_Home_Footer_uc" %>

<style>
    /* ===== THEME TOGGLE (SÁNG / TỐI) ===== */
    .theme-toggle .theme-btn {
        width: 34px;
        height: 34px;
        border-radius: 50%;
        border: 1px solid #e6e8eb;
        background: #fff;
        color: #6c757d;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        padding: 0;
        transition: all .15s ease;
    }

        .theme-toggle .theme-btn i {
            font-size: 18px;
            line-height: 1;
        }

        .theme-toggle .theme-btn:hover {
            background: rgba(0,0,0,.04);
            color: #111;
        }

        .theme-toggle .theme-btn.active {
            background: var(--tblr-primary);
            border-color: var(--tblr-primary);
            color: #fff;
        }

    [data-bs-theme="dark"] .theme-toggle .theme-btn {
        background: transparent;
        border-color: rgba(255,255,255,.2);
        color: rgba(255,255,255,.7);
    }

        [data-bs-theme="dark"] .theme-toggle .theme-btn.active {
            background: #fff;
            color: #000;
        }

    /* ===== FOOTER FIX ===== */
    /* Bỏ đường kẻ ngang phía trên khu vực TOP (chỗ bạn khoanh đỏ) */
    .footer .footer-top {
        border-top: 0 !important;
    }

    /* Chữ thông tin liên hệ: bình thường (không small) */
    .footer .contact-text {
        font-size: 1rem; /* cỡ chữ bình thường */
        line-height: 1.5;
    }
    /* ===== FOOTER TEXT SIZE FIX ===== */

    /* link trong footer nhỏ lại như "Blog" mẫu */
    .footer ul li a {
        font-size: .875rem; /* ~14px */
        line-height: 1.4;
    }

    /* text thường (Email, CSKH, địa chỉ) đồng bộ */
    .footer .contact-text {
        font-size: .875rem;
        line-height: 1.5;
    }

    /* tiêu đề cột giữ nguyên */
    .footer .fw-bold {
        font-size: 1rem; /* ~16px */
    }
    /* ===== FOOTER LINK HOVER - TABLER SUCCESS ===== */
    .footer a.link-secondary {
        transition: color .15s ease;
    }

        .footer a.link-secondary:hover {
            color: var(--tblr-success) !important; /* xanh lá Tabler */
            text-decoration: none;
        }

    /* Icon mạng xã hội hover nhẹ hơn */
    .footer .btn.btn-icon:hover {
        filter: brightness(1.1);
    }
</style>

<footer class="footer mt-5">

    <!-- TOP: 4 cột -->
    <div class="footer-top">
        <div class="container py-5">
            <div class="row g-4 align-items-start">

                <!-- Cột 1 -->
                <div class="col-12 col-md-3">
                    <%--<div class="fw-bold mb-3">Tải ứng dụng AhaSale</div>

                    <div class="d-flex gap-3">
                        <div class="flex-shrink-0">
                            <img src="/uploads/images/qr-chuyen-khoan.jpg" alt="QR"
                                style="width: 96px; height: 96px; object-fit: cover; border: 1px solid #e6e8eb; border-radius: 6px;" />
                        </div>

                        <div class="d-flex flex-column gap-2">
                            <img src="/uploads/images/ios.svg" alt="App Store" style="height: 36px;" />
                            <img src="/uploads/images/android.svg" alt="Google Play" style="height: 36px;" />
                        </div>
                    </div>--%>

                    <div class="fw-bold mb-3 mt-3">Giao diện</div>

                    <div class="d-flex align-items-center gap-2 theme-toggle">
                        <button type="button" class="theme-btn" data-theme="light" title="Giao diện sáng">
                            <i class="ti ti-sun"></i>
                        </button>
                        <button type="button" class="theme-btn" data-theme="dark" title="Giao diện tối">
                            <i class="ti ti-moon"></i>
                        </button>
                    </div>
                </div>

                <!-- Cột 2 -->
                <div class="col-12 col-md-3">
                    <div class="fw-bold mb-3">Hỗ trợ khách hàng</div>
                    <ul class="list-unstyled mb-0">
                        <li class="mb-2"><a href="#" class="link-secondary text-decoration-none">Trung tâm trợ giúp</a></li>
                        <li class="mb-2"><a href="#" class="link-secondary text-decoration-none">An toàn mua bán</a></li>
                        <li class="mb-2"><a href="#" class="link-secondary text-decoration-none">Liên hệ hỗ trợ</a></li>
                    </ul>
                </div>

                <!-- Cột 3 -->
                <div class="col-12 col-md-3">
                    <div class="fw-bold mb-3">Về AhaSale</div>
                    <ul class="list-unstyled mb-0">
                        <li class="mb-2"><a href="#" class="link-secondary text-decoration-none">Giới thiệu</a></li>
                        <li class="mb-2"><a href="#" class="link-secondary text-decoration-none">Quy chế hoạt động sàn</a></li>
                        <li class="mb-2"><a href="#" class="link-secondary text-decoration-none">Chính sách bảo mật</a></li>
                        <li class="mb-2"><a href="#" class="link-secondary text-decoration-none">Giải quyết tranh chấp</a></li>
                        <li class="mb-2"><a href="#" class="link-secondary text-decoration-none">Tuyển dụng</a></li>
                        <li class="mb-2"><a href="#" class="link-secondary text-decoration-none">Truyền thông</a></li>
                        <li class="mb-2"><a href="#" class="link-secondary text-decoration-none">Blog</a></li>
                    </ul>
                </div>

                <!-- Cột 4 -->
                <div class="col-12 col-md-3">
                    <div class="fw-bold mb-3">Liên kết</div>

                    <div class="d-flex gap-2 mb-3">
                        <a href="#" class="btn btn-icon" style="background: #0a66c2; color: #fff; border: 0" title="LinkedIn">
                            <i class="ti ti-brand-linkedin"></i>
                        </a>
                        <a href="#" class="btn btn-icon" style="background: #ff0000; color: #fff; border: 0" title="YouTube">
                            <i class="ti ti-brand-youtube"></i>
                        </a>
                        <a href="#" class="btn btn-icon" style="background: #1877f2; color: #fff; border: 0" title="Facebook">
                            <i class="ti ti-brand-facebook"></i>
                        </a>
                    </div>

                    <!-- Thông tin liên hệ: chữ bình thường -->
                    <div class="text-secondary contact-text">
                        <div class="mb-2">
                            Email:
              <a href="mailto:ahasale.vn@gmail.com" class="link-secondary text-decoration-none">ahasale.vn@gmail.com</a>
                        </div>
                        <div class="mb-2">CSKH: 0868.877.686</div>
                        <div>Địa chỉ: Số 46/3, đường Võ Thị Sáu, khu phố Gò Me, Phường Trấn Biên, Tỉnh Đồng Nai, Việt Nam</div>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <!-- BOTTOM: phân cách TOP/BOTTOM -->
    <div class="border-top">
        <div class="container py-4">
            <div class="row align-items-center g-3">
                <div class="col-12 col-lg-9">
                    <div class="text-secondary small">
                        CÔNG TY CỔ PHẦN ĐÀO TẠO AHA SALE - Người đại diện theo pháp luật: Trần Đức Cường; GPKDKD: 3603907499 do Sở tài chính tỉnh Đồng Nai cấp ngày 29/03/2023;<br />
                        <%--GPMXH: 185/GP-BTTTT do Bộ Thông tin và Truyền thông cấp ngày 09/07/2024 - Chịu trách nhiệm nội dung: Trần Hoàng Ly.--%>
                        <a href="#" class="link-secondary text-success text-decoration-none">Chính sách sử dụng</a>
                    </div>
                </div>

                <%--<div class="col-12 col-lg-3 text-lg-end">
          <img src="/uploads/images/bct.png" alt="Đã đăng ký" style="height:60px;" />
        </div>--%>
            </div>
        </div>
    </div>

    <!-- Script giữ nguyên logic sáng/tối -->
    <script>
        (function () {
            const ROOT = document.documentElement;
            const STORAGE_KEY = 'theme-preference';
            const buttons = document.querySelectorAll('[data-theme]');

            function applyTheme(theme) {
                localStorage.setItem(STORAGE_KEY, theme);
                ROOT.setAttribute('data-bs-theme', theme);
                buttons.forEach(b => b.classList.toggle('active', b.dataset.theme === theme));
            }

            const saved = localStorage.getItem(STORAGE_KEY) || 'light';
            applyTheme(saved);

            buttons.forEach(btn => btn.addEventListener('click', () => applyTheme(btn.dataset.theme)));
        })();
    </script>
</footer>
