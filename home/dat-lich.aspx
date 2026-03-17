<%@ Page Title="Đặt lịch dịch vụ" Language="C#" MasterPageFile="~/MasterPage/Tabler/TablerHome.master" AutoEventWireup="true" CodeFile="dat-lich.aspx.cs" Inherits="home_dat_lich" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head_truoc" runat="server">
    <style>
        :root {
            --shop-orange: #ef4444;
            --shop-orange-soft: #fee2e2;
            --shop-ink: #102a43;
            --shop-muted: #627d98;
            --shop-line: #d9e2ec;
            --shop-bg: #f5f8fb;
            --shop-card: #ffffff;
            --shop-radius: 18px;
        }

        .booking-shell {
            min-height: 100vh;
            padding: 24px 16px 40px;
            background:
                radial-gradient(1000px 280px at 16% -4%, rgba(238,77,45,.20), transparent 65%),
                radial-gradient(1000px 320px at 86% -6%, rgba(255,122,69,.18), transparent 62%),
                var(--shop-bg);
        }

        .booking-wrap {
            max-width: 960px;
            margin: 0 auto;
        }

        .booking-card,
        .booking-hero {
            background: var(--shop-card);
            border: 1px solid var(--shop-line);
            border-radius: var(--shop-radius);
            box-shadow: 0 16px 36px rgba(16, 42, 67, .08);
        }

        .booking-hero {
            padding: 24px;
            margin-bottom: 18px;
        }

        .booking-eyebrow {
            color: var(--shop-orange);
            font-weight: 800;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: .08em;
        }

        .booking-title {
            margin: 10px 0 8px;
            color: var(--shop-ink);
            font-size: 34px;
            line-height: 1.15;
            font-weight: 900;
        }

        .booking-sub {
            color: var(--shop-muted);
            font-size: 15px;
            line-height: 1.6;
            max-width: 720px;
        }

        .booking-meta {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 12px;
            margin-top: 18px;
        }

        .booking-meta-box {
            border: 1px solid var(--shop-line);
            border-radius: 14px;
            padding: 14px 16px;
            background: #fcfdff;
        }

        .booking-meta-label {
            color: var(--shop-muted);
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: .06em;
        }

        .booking-meta-value {
            margin-top: 6px;
            color: var(--shop-ink);
            font-size: 16px;
            font-weight: 800;
        }

        .booking-card {
            padding: 22px;
        }

        .booking-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 16px;
        }

        .booking-field {
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .booking-field-full {
            grid-column: 1 / -1;
        }

        .booking-label {
            color: var(--shop-ink);
            font-size: 14px;
            font-weight: 800;
        }

        .booking-input,
        .booking-select,
        .booking-textarea {
            width: 100%;
            min-height: 44px;
            border-radius: 12px;
            border: 1px solid var(--shop-line);
            padding: 0 14px;
            background: #fff;
            color: var(--shop-ink);
        }

        .service-search {
            position: relative;
        }

        .service-search-input {
            height: 48px;
            border-radius: 999px;
            padding-left: 46px;
            padding-right: 42px;
            font-size: 15px;
            border: 1px solid #dfe1e5;
            box-shadow: 0 1px 2px rgba(32, 33, 36, 0.12);
            transition: box-shadow .18s ease, border-color .18s ease;
        }

        .service-search-input:focus {
            border-color: #dfe1e5;
            box-shadow: 0 2px 6px rgba(32, 33, 36, 0.2);
            outline: none;
        }

        .service-search-icon {
            position: absolute;
            left: 16px;
            top: 50%;
            transform: translateY(-50%);
            color: #9aa0a6;
            width: 18px;
            height: 18px;
            pointer-events: none;
        }

        .service-search-clear {
            position: absolute;
            right: 12px;
            top: 50%;
            transform: translateY(-50%);
            border: 0;
            background: transparent;
            color: #5f6368;
            font-size: 18px;
            width: 28px;
            height: 28px;
            border-radius: 50%;
            display: none;
            align-items: center;
            justify-content: center;
            cursor: pointer;
        }

        .service-search-clear.is-visible {
            display: inline-flex;
        }

        .service-search-clear:hover {
            background: #f1f3f4;
        }

        .service-suggest {
            position: absolute;
            top: calc(100% + 6px);
            left: 0;
            right: 0;
            background: #ffffff;
            border: 1px solid #dfe1e5;
            border-radius: 18px;
            box-shadow: 0 10px 24px rgba(32, 33, 36, 0.18);
            max-height: 300px;
            overflow-y: auto;
            display: none;
            z-index: 20;
        }

        .service-suggest.is-open {
            display: block;
        }

        .service-suggest-item {
            padding: 10px 16px;
            cursor: pointer;
            font-weight: 600;
            color: var(--shop-ink);
        }

        .service-suggest-item strong {
            font-weight: 800;
        }

        .service-suggest-item:hover,
        .service-suggest-item.active {
            background: #f1f3f9;
        }

        .service-suggest-empty {
            padding: 10px 16px;
            color: var(--shop-muted);
        }

        .booking-textarea {
            min-height: 110px;
            padding: 12px 14px;
            resize: vertical;
        }

        .booking-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            margin-top: 20px;
        }

        .booking-btn {
            min-height: 44px;
            padding: 0 18px;
            border-radius: 12px;
            border: 0;
            font-weight: 800;
            cursor: pointer;
        }

        .booking-btn-primary {
            background: linear-gradient(135deg, #ff9a3d, #ee4d2d);
            color: #fff;
        }

        .booking-btn-secondary {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            background: #fff;
            color: var(--shop-ink);
            border: 1px solid var(--shop-line);
            text-decoration: none;
        }

        .booking-alert {
            border-radius: 14px;
            padding: 14px 16px;
            margin-bottom: 16px;
            font-weight: 700;
        }

        .booking-alert-success {
            background: #edfdf4;
            border: 1px solid #b7ebc6;
            color: #137333;
        }

        .booking-alert-warning {
            background: #fff8e1;
            border: 1px solid #ffe08a;
            color: #8a6116;
        }

        @media (max-width: 820px) {
            .booking-meta,
            .booking-grid {
                grid-template-columns: 1fr;
            }

            .booking-title {
                font-size: 28px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
    <div class="booking-shell">
        <div class="booking-wrap">
            <asp:HiddenField ID="hf_shop_account" runat="server" />
            <asp:PlaceHolder ID="ph_success" runat="server" Visible="false">
                <div class="booking-alert booking-alert-success">
                    <asp:Label ID="lb_success" runat="server"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="ph_warning" runat="server" Visible="false">
                <div class="booking-alert booking-alert-warning">
                    <asp:Label ID="lb_warning" runat="server"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <section class="booking-hero">
                <div class="booking-eyebrow">Đặt lịch dịch vụ</div>
                <h1 class="booking-title">Đặt lịch dịch vụ</h1>
                <div class="booking-sub">
                    Đặt lịch nhanh trực tiếp từ <b>/home</b>. Lịch sẽ được lưu vào hệ thống và hiển thị cho gian hàng đối tác.
                </div>

                <div class="booking-meta">
                    <div class="booking-meta-box">
                        <div class="booking-meta-label">Gian hàng</div>
                        <div class="booking-meta-value"><asp:Label ID="lb_shop_name" runat="server" Text="--"></asp:Label></div>
                    </div>
                    <div class="booking-meta-box">
                        <div class="booking-meta-label">Dịch vụ</div>
                        <div class="booking-meta-value"><asp:Label ID="lb_service_name" runat="server" Text="Chọn dịch vụ"></asp:Label></div>
                    </div>
                    <div class="booking-meta-box">
                        <div class="booking-meta-label">Nguồn đặt</div>
                        <div class="booking-meta-value">/home</div>
                    </div>
                </div>
            </section>

            <asp:Panel ID="pn_form" runat="server" CssClass="booking-card">
                <div class="booking-grid">
                    <div class="booking-field">
                        <label class="booking-label" for="<%= txt_service_search.ClientID %>">Dịch vụ</label>
                        <div class="service-search">
                            <span class="service-search-icon" aria-hidden="true">
                                <svg viewBox="0 0 24 24" width="18" height="18" fill="currentColor">
                                    <path d="M15.5 14h-.79l-.28-.27A6.5 6.5 0 1 0 14 15.5l.27.28v.79L20 21.5 21.5 20l-6-6zm-6 0a4.5 4.5 0 1 1 0-9 4.5 4.5 0 0 1 0 9z"></path>
                                </svg>
                            </span>
                            <asp:TextBox ID="txt_service_search" runat="server" CssClass="booking-input service-search-input" placeholder="Tìm dịch vụ (ví dụ: chăm sóc da)" autocomplete="off" spellcheck="false"></asp:TextBox>
                            <button type="button" class="service-search-clear" aria-label="Xóa nội dung">×</button>
                            <asp:HiddenField ID="hf_service_id" runat="server" />
                            <div id="service_suggest" class="service-suggest"></div>
                        </div>
                    </div>

                    <div class="booking-field">
                        <label class="booking-label" for="<%= txt_ngay.ClientID %>">Ngày đặt</label>
                        <asp:TextBox ID="txt_ngay" runat="server" CssClass="booking-input" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="booking-field">
                        <label class="booking-label" for="<%= txt_hoten.ClientID %>">Tên khách hàng</label>
                        <asp:TextBox ID="txt_hoten" runat="server" CssClass="booking-input" MaxLength="80"></asp:TextBox>
                    </div>

                    <div class="booking-field">
                        <label class="booking-label" for="<%= txt_sdt.ClientID %>">Số điện thoại</label>
                        <asp:TextBox ID="txt_sdt" runat="server" CssClass="booking-input" MaxLength="20"></asp:TextBox>
                    </div>

                    <div class="booking-field">
                        <label class="booking-label" for="<%= ddl_gio.ClientID %>">Giờ</label>
                        <asp:DropDownList ID="ddl_gio" runat="server" CssClass="booking-select"></asp:DropDownList>
                    </div>

                    <div class="booking-field">
                        <label class="booking-label" for="<%= ddl_phut.ClientID %>">Phút</label>
                        <asp:DropDownList ID="ddl_phut" runat="server" CssClass="booking-select"></asp:DropDownList>
                    </div>

                    <div class="booking-field booking-field-full">
                        <label class="booking-label" for="<%= txt_ghichu.ClientID %>">Ghi chú</label>
                        <asp:TextBox ID="txt_ghichu" runat="server" CssClass="booking-textarea" TextMode="MultiLine"></asp:TextBox>
                    </div>
                </div>

                <div class="booking-actions">
                    <asp:Button ID="but_datlich" runat="server" CssClass="booking-btn booking-btn-primary" Text="Đặt lịch ngay" OnClick="but_datlich_Click" />
                    <asp:HyperLink ID="lnk_back" runat="server" CssClass="booking-btn booking-btn-secondary" Text="Quay lại"></asp:HyperLink>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentFootSau" ContentPlaceHolderID="foot_sau" runat="server">
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            var input = document.getElementById('<%= txt_service_search.ClientID %>');
            var hidden = document.getElementById('<%= hf_service_id.ClientID %>');
            var suggest = document.getElementById('service_suggest');
            var label = document.getElementById('<%= lb_service_name.ClientID %>');
            var shopHolder = document.getElementById('<%= hf_shop_account.ClientID %>');
            if (!input || !hidden || !suggest) return;
            var clearBtn = input.parentElement ? input.parentElement.querySelector('.service-search-clear') : null;

            var timer = null;
            var items = [];
            var activeIndex = -1;
            var lastQuery = '';

            function closeSuggest() {
                suggest.classList.remove('is-open');
                suggest.innerHTML = '';
                activeIndex = -1;
                items = [];
            }

            function escapeHtml(text) {
                return (text || '').replace(/[&<>"']/g, function (ch) {
                    switch (ch) {
                        case '&': return '&amp;';
                        case '<': return '&lt;';
                        case '>': return '&gt;';
                        case '"': return '&quot;';
                        case "'": return '&#39;';
                        default: return ch;
                    }
                });
            }

            function highlightTerm(text, term) {
                var raw = text || '';
                var keyword = term || '';
                if (!raw || !keyword) return escapeHtml(raw);
                var lower = raw.toLowerCase();
                var keyLower = keyword.toLowerCase();
                var idx = lower.indexOf(keyLower);
                if (idx < 0) return escapeHtml(raw);
                var before = escapeHtml(raw.substring(0, idx));
                var match = escapeHtml(raw.substring(idx, idx + keyword.length));
                var after = escapeHtml(raw.substring(idx + keyword.length));
                return before + '<strong>' + match + '</strong>' + after;
            }

            function renderSuggest(list, term) {
                suggest.innerHTML = '';
                if (!list || list.length === 0) {
                    var empty = document.createElement('div');
                    empty.className = 'service-suggest-empty';
                    empty.textContent = 'Không tìm thấy dịch vụ phù hợp.';
                    suggest.appendChild(empty);
                    suggest.classList.add('is-open');
                    return;
                }
                list.forEach(function (item, idx) {
                    var row = document.createElement('div');
                    row.className = 'service-suggest-item';
                    row.innerHTML = highlightTerm(item.name || '', term);
                    row.dataset.id = item.id || '';
                    row.addEventListener('mousedown', function (ev) {
                        ev.preventDefault();
                        selectItem(idx);
                    });
                    suggest.appendChild(row);
                });
                suggest.classList.add('is-open');
            }

            function selectItem(idx) {
                if (idx < 0 || idx >= items.length) return;
                var item = items[idx];
                input.value = item.name || '';
                hidden.value = item.id || '';
                if (label) label.textContent = item.name || 'Chọn dịch vụ';
                updateClear();
                closeSuggest();
            }

            function setActive(idx) {
                var rows = suggest.querySelectorAll('.service-suggest-item');
                rows.forEach(function (row) { row.classList.remove('active'); });
                if (idx >= 0 && idx < rows.length) {
                    rows[idx].classList.add('active');
                }
            }

            function fetchSuggest(term) {
                var shop = shopHolder ? shopHolder.value : '';
                if (!window.PageMethods || !term) {
                    closeSuggest();
                    return;
                }
                lastQuery = term;
                PageMethods.SearchServices(term, shop, function (res) {
                    if (term !== lastQuery) return;
                    items = res || [];
                    activeIndex = -1;
                    renderSuggest(items, term);
                }, function () {
                    closeSuggest();
                });
            }

            function updateClear() {
                if (!clearBtn) return;
                var hasValue = (input.value || '').trim().length > 0;
                clearBtn.classList.toggle('is-visible', hasValue);
            }

            if (clearBtn) {
                clearBtn.addEventListener('click', function () {
                    input.value = '';
                    hidden.value = '';
                    if (label) label.textContent = 'Chọn dịch vụ';
                    updateClear();
                    closeSuggest();
                    input.focus();
                });
            }

            input.addEventListener('input', function () {
                hidden.value = '';
                if (label) label.textContent = 'Chọn dịch vụ';
                var term = (input.value || '').trim();
                if (timer) clearTimeout(timer);
                updateClear();
                if (term.length < 1) {
                    closeSuggest();
                    return;
                }
                timer = setTimeout(function () {
                    fetchSuggest(term);
                }, 250);
            });

            input.addEventListener('keydown', function (ev) {
                if (!suggest.classList.contains('is-open')) return;
                if (ev.key === 'ArrowDown') {
                    ev.preventDefault();
                    activeIndex = Math.min(activeIndex + 1, items.length - 1);
                    setActive(activeIndex);
                } else if (ev.key === 'ArrowUp') {
                    ev.preventDefault();
                    activeIndex = Math.max(activeIndex - 1, 0);
                    setActive(activeIndex);
                } else if (ev.key === 'Enter') {
                    if (activeIndex >= 0) {
                        ev.preventDefault();
                        selectItem(activeIndex);
                    }
                } else if (ev.key === 'Escape') {
                    closeSuggest();
                }
            });

            input.addEventListener('blur', function () {
                setTimeout(closeSuggest, 120);
            });

            input.addEventListener('focus', function () {
                var term = (input.value || '').trim();
                if (term.length >= 1) {
                    if (timer) clearTimeout(timer);
                    timer = setTimeout(function () {
                        fetchSuggest(term);
                    }, 120);
                }
            });

            document.addEventListener('click', function (ev) {
                if (ev.target === input || suggest.contains(ev.target)) return;
                closeSuggest();
            });

            updateClear();
        });
    </script>
</asp:Content>
