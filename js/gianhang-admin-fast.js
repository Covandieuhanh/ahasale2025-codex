(function (window, document) {
    function restyleProgressOverlay(node) {
        if (!node || node.getAttribute('data-fast-overlay') === '1') return;
        if (!node.classList) return;
        if (!node.classList.contains('fixed-top') || !node.classList.contains('h-100') || !node.classList.contains('w-100')) return;
        if (!node.querySelector('.activity-atom, .activity-ring')) return;

        node.setAttribute('data-fast-overlay', '1');
        node.style.position = 'fixed';
        node.style.top = '92px';
        node.style.right = '18px';
        node.style.left = 'auto';
        node.style.width = 'auto';
        node.style.height = 'auto';
        node.style.bottom = 'auto';
        node.style.padding = '10px 16px';
        node.style.borderRadius = '999px';
        node.style.background = 'rgba(15, 23, 42, 0.92)';
        node.style.boxShadow = '0 12px 28px rgba(15, 23, 42, 0.24)';
        node.style.display = node.style.display === 'none' ? 'none' : 'inline-flex';
        node.style.alignItems = 'center';
        node.style.gap = '12px';
        node.style.opacity = '1';

        var inner = node.firstElementChild;
        if (inner) {
            inner.style.paddingTop = '0';
            inner.style.display = 'contents';
        }

        var label = node.querySelector('.aha-fast-progress-label');
        if (!label) {
            label = document.createElement('span');
            label.className = 'aha-fast-progress-label fg-white';
            label.textContent = 'Đang xử lý...';
            node.appendChild(label);
        }

        var atom = node.querySelector('.activity-atom');
        if (atom) {
            atom.setAttribute('data-type', 'ring');
            atom.classList.remove('activity-atom');
            atom.classList.add('activity-ring');
        }
    }

    function normalizeProgressOverlays() {
        var nodes = document.querySelectorAll('div.bg-dark.fixed-top.h-100.w-100');
        for (var i = 0; i < nodes.length; i++) {
            restyleProgressOverlay(nodes[i]);
        }
    }

    function bind() {
        normalizeProgressOverlays();
        if (window.MutationObserver && !window.__ahaFastProgressObserver) {
            var observer = new MutationObserver(function () {
                normalizeProgressOverlays();
            });
            observer.observe(document.body, { childList: true, subtree: true });
            window.__ahaFastProgressObserver = observer;
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', bind);
    } else {
        bind();
    }

    window.addEventListener('pageshow', bind);
    window.ahaGianHangFastUi = {
        normalizeProgressOverlays: normalizeProgressOverlays
    };
})(window, document);
