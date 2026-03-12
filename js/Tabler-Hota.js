function format_sotien_new(textbox) { var value = parseFloat(textbox.value.replace(/\./g, '').replace(',', '.')); if (!isNaN(value)) { textbox.value = value.toLocaleString('de-DE', { minimumFractionDigits: 0, maximumFractionDigits: 3 }); } else { textbox.value = ''; } }

/*BEGIN MASK TIME*/
(function (global) {

    function setupTimeMask(input) {

        function setCaret(pos) {
            input.setSelectionRange(pos, pos);
        }
        function selectHour() {
            input.setSelectionRange(0, 2);
        }
        function selectMinute() {
            input.setSelectionRange(3, 5);
        }
        function ensureInit() {
            if (!input.value) {
                input.value = '--:--';
            }
        }

        // tránh gắn 2 lần trên cùng 1 input
        if (input.dataset.masked) return;
        input.dataset.masked = "1";

        ensureInit();

        // Focus: luôn chọn HH
        input.addEventListener('focus', function () {
            ensureInit();
            setTimeout(selectHour, 0);
        });

        // Click: trái ':' → HH, phải ':' → mm
        input.addEventListener('click', function () {
            ensureInit();
            var pos = input.selectionStart || 0;
            if (pos <= 2) {
                selectHour();
            } else {
                selectMinute();
            }
        });

        input.addEventListener('keydown', function (e) {
            var key = e.key;
            var pos = input.selectionStart;

            // Cho Tab, Arrow*
            if (key === 'Tab' || key.indexOf('Arrow') === 0) {
                return;
            }

            e.preventDefault(); // tự xử lý

            // Backspace
            if (key === 'Backspace') {
                if (pos === 0) return;

                var p = pos;
                if (p === 2) p = 1; // tránh ':'
                var chars = input.value.split('');

                p = p - 1;
                if (p === 2) p = 1;
                if (p < 0) return;

                chars[p] = '-';
                input.value = chars.join('');

                var next = p;
                if (next === 2) next = 1;
                setCaret(next);
                return;
            }

            // Gõ số
            if (key >= '0' && key <= '9') {
                var p = pos;

                if (p === 2) p = 3;   // nhảy qua ':'
                if (p > 4) return;    // quá 5 ký tự

                var chars = (input.value || '--:--').split('');
                chars[p] = key;
                input.value = chars.join('');

                var next = p + 1;
                if (next === 2) next = 3;
                setCaret(next);
                return;
            }

            // phím khác: bỏ qua
        });

        // Blur: sai format thì đưa về --:--
        input.addEventListener('blur', function () {
            if (!input.value || input.value === '--:--') return;
            var re = /^([01]\d|2[0-3]):([0-5]\d)$/;
            if (!re.test(input.value)) {
                input.value = '--:--';
            }
        });
    }

    function initTimeMasks(selector) {
        var inputs = document.querySelectorAll(selector || '.time-mask');
        inputs.forEach(function (input) {
            setupTimeMask(input);
        });
    }

    // expose ra global
    global.TimeMask = {
        init: initTimeMasks
    };

})(window);

// Khởi tạo lần đầu khi DOM ready
document.addEventListener('DOMContentLoaded', function () {
    TimeMask.init('.time-mask');
});

// Nếu dùng UpdatePanel trong WebForms thì thêm đoạn này
if (typeof Sys !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(function () {
        TimeMask.init('.time-mask');
    });
}
/*END MASK TIME*/

/*BEGINT SEARCH FOR DROPDOWNLIST (TOM-SLEECT)*/
// Hàm init cho 1 element
function initOneTomSelect(el) {
    if (!el) return;
    if (el.getAttribute("data-no-tomselect") === "1") return;

    // Nếu đã có TomSelect thì destroy trước (tránh bị đúp)
    if (el.tomselect) {
        try { el.tomselect.destroy(); } catch (e) { }
    }

    new TomSelect(el, {
        maxItems: 1,
        placeholder: el.getAttribute("data-placeholder") || "Chọn...",
        sortField: { field: "text", direction: "asc" },
        allowEmptyOption: true
    });
}

// Hàm áp dụng cho toàn trang
function initAllTomSelect() {
    // lấy tất cả DropDownList có class form-select
    var selects = document.querySelectorAll("select.form-select");

    selects.forEach(function (el) {
        // Không apply cho dropdown disabled
        if (el.disabled) return;
        if (el.getAttribute("data-no-tomselect") === "1") return;

        initOneTomSelect(el);
    });
}

// Init lần đầu
document.addEventListener("DOMContentLoaded", function () {
    initAllTomSelect();
});

// Init lại sau mỗi lần UpdatePanel refresh (nếu có Sys)
if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
        initAllTomSelect();
    });
}

/*END SEARCH FOR DROPDOWNLIST (TOM-SLEECT)*/

/*BEGIN ẨN HIỆN MẬT KHẨU Ở TEXTBOX PASSWORD*/
(function () {
    if (window.__ahaPasswordToggleBound === true) {
        return;
    }

    window.__ahaPasswordToggleBound = true;

    // Gắn 1 listener duy nhất trên document
    document.addEventListener('click', function (e) {
        var toggle = e.target.closest('.js-toggle-password');
        if (!toggle) return; // không click vào nút "Hiện mật khẩu" thì thôi

        e.preventDefault();

        // Tìm input-group gần nhất
        var group = toggle.closest('.input-group');
        if (!group) return;

        // Tìm ô input có class js-password trong group đó
        var input = group.querySelector('input.js-password');

        if (!input) return;

        // Đổi type
        if (input.type === 'password') {
            input.type = 'text';

            var icon = toggle.querySelector('i');
            if (icon) {
                icon.classList.remove('ti-eye');
                icon.classList.add('ti-eye-off');
            }
        } else {
            input.type = 'password';

            var icon2 = toggle.querySelector('i');
            if (icon2) {
                icon2.classList.remove('ti-eye-off');
                icon2.classList.add('ti-eye');
            }
        }
    });
})();
/*END ẨN HIỆN MẬT KHẨU Ở TEXTBOX PASSWORD*/

/*BEGIN SHOW TOAST*/
// Singleton: không chồng toast
(function () {
    var currentToastEl = null;
    var currentHideTimer = null;

    function ensureToastContainer() {
        var c = document.getElementById('toastContainer');
        if (!c) {
            c = document.createElement('div');
            c.id = 'toastContainer';
            c.className = 'toast-container position-fixed bottom-0 end-0 p-3';
            document.body.appendChild(c);
        }
        return c;
    }

    function hideAndRemove(el) {
        if (!el) return;
        el.classList.remove('show'); // chạy transition opacity
        setTimeout(function () {
            if (el && el.parentNode) el.parentNode.removeChild(el);
            if (currentToastEl === el) currentToastEl = null;
        }, 200);
    }

    // API KHỚP HÀM C#:
    // show_toast(message, type, autohide, delay, title)
    window.show_toast = function (message, type, autohide, delay, title) {
        var container = ensureToastContainer();

        // Nếu đang có toast cũ → đóng & xoá trước khi tạo cái mới
        if (currentHideTimer) { clearTimeout(currentHideTimer); currentHideTimer = null; }
        if (currentToastEl) { hideAndRemove(currentToastEl); }

        var bg =
            type === 'success' ? 'bg-success' :
                type === 'danger' ? 'bg-danger' :
                    type === 'warning' ? 'bg-warning' :
                        type === 'info' ? 'bg-info' :
                            type === 'primary' ? 'bg-primary' : 'bg-secondary';

        var el = document.createElement('div');
        el.className = 'toast fade'; // bỏ text-white ở đây
        el.setAttribute('role', 'alert');
        el.setAttribute('aria-live', 'assertive');
        el.setAttribute('aria-atomic', 'true');

        el.innerHTML =
            '<div class="toast-header ' + bg + ' text-white">' +
            '<strong class="me-auto">' + (title || '') + '</strong>' +
            '<button type="button" class="ms-2 btn-close btn-close-white" aria-label="Close"></button>' +
            '</div>' +
            '<div class="toast-body">' + (message || '') + '</div>';


        container.appendChild(el);
        currentToastEl = el;

        // Đóng bằng nút X
        var closeBtn = el.querySelector('.btn-close');
        if (closeBtn) {
            closeBtn.addEventListener('click', function () {
                if (currentHideTimer) { clearTimeout(currentHideTimer); currentHideTimer = null; }
                hideAndRemove(el);
            });
        }

        // Hiện lên
        requestAnimationFrame(function () { el.classList.add('show'); });

        // Tự ẩn nếu autohide
        if (autohide !== false) {
            var ms = (typeof delay === 'number' ? delay : 3000);
            currentHideTimer = setTimeout(function () {
                hideAndRemove(el);
                currentHideTimer = null;
            }, ms);
        }
    };

    // Hỗ trợ UpdatePanel: đảm bảo container tồn tại sau partial postback
    if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () { ensureToastContainer(); });
    }
})();
/*END SHOW TOAST*/
/*BEGIN SHOW MODAL*/
(function () {
    var currentModalEl = null;
    var currentBackdropEl = null;

    function closeModal() {
        if (currentModalEl) {
            currentModalEl.remove();
            currentModalEl = null;
        }
        if (currentBackdropEl) {
            currentBackdropEl.remove();
            currentBackdropEl = null;
        }
        document.body.classList.remove("modal-open");
        document.body.style.removeProperty("overflow");
    }

    // API: show_modal(message, title, allowBackdropClose, type)
    window.show_modal = function (message, title, allowBackdropClose, type) {

        // Xoá modal/backdrop cũ nếu có
        closeModal();

        // Map màu
        var colorClass =
            type === "danger" ? "bg-danger text-white" :
                type === "warning" ? "bg-warning" :
                    type === "success" ? "bg-success text-white" :
                        type === "info" ? "bg-info text-white" :
                            "bg-primary text-white"; // default

        var buttonClass =
            type === "danger" ? "btn-danger" :
                type === "warning" ? "btn-warning" :
                    type === "success" ? "btn-success" :
                        type === "info" ? "btn-info" :
                            "btn-primary";

        // Tạo backdrop
        var backdrop = document.createElement("div");
        backdrop.className = "modal-backdrop fade show";
        currentBackdropEl = backdrop;
        document.body.appendChild(backdrop);

        // Tạo modal
        var html = `
            <div class="modal modal-blur show" id="dynamicModal"
                 tabindex="-1" role="dialog" style="display:block;">
              <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">

                  <div class="modal-header ${colorClass}">
                    <h5 class="modal-title">${title || ""}</h5>
                    <button type="button" class="btn-close btn-close-white" aria-label="Close"></button>
                  </div>

                  <div class="modal-body">
                    ${message || ""}
                  </div>

                  <div class="modal-footer">
                    <button type="button" class="btn ${buttonClass} btn-ok">OK</button>
                  </div>

                </div>
              </div>
            </div>
        `;

        var wrapper = document.createElement("div");
        wrapper.innerHTML = html.trim();
        var modal = wrapper.firstChild;

        currentModalEl = modal;
        document.body.appendChild(modal);

        document.body.classList.add("modal-open");
        document.body.style.overflow = "hidden";

        // Nút đóng (X)
        modal.querySelector(".btn-close").addEventListener("click", closeModal);

        // Nút OK
        modal.querySelector(".btn-ok").addEventListener("click", closeModal);

        // Click ra ngoài
        if (allowBackdropClose === true) {
            modal.addEventListener("click", function (e) {
                if (e.target === modal) closeModal();
            });
        }
    };
})();
/*BEGIN SHOW MODAL 2 BUTTONS*/
(function () {
    var currentModalEl = null;
    var currentBackdropEl = null;

    function closeModal() {
        if (currentModalEl) {
            currentModalEl.remove();
            currentModalEl = null;
        }
        if (currentBackdropEl) {
            currentBackdropEl.remove();
            currentBackdropEl = null;
        }
        document.body.classList.remove("modal-open");
        document.body.style.removeProperty("overflow");
    }

    // API: show_modal_2btn(message, title, allowBackdropClose, type, primaryText, primaryHref, secondaryText, secondaryHref)
    window.show_modal_2btn = function (message, title, allowBackdropClose, type, primaryText, primaryHref, secondaryText, secondaryHref) {
        closeModal();

        var colorClass =
            type === "danger" ? "bg-danger text-white" :
                type === "warning" ? "bg-warning" :
                    type === "success" ? "bg-success text-white" :
                        type === "info" ? "bg-info text-white" :
                            "bg-primary text-white";

        var html = `
            <div class="modal modal-blur show" id="dynamicModal2"
                 tabindex="-1" role="dialog" style="display:block;">
              <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">

                  <div class="modal-header ${colorClass}">
                    <h5 class="modal-title">${title || ""}</h5>
                    <button type="button" class="btn-close btn-close-white" aria-label="Close"></button>
                  </div>

                  <div class="modal-body">
                    ${message || ""}
                  </div>

                  <div class="modal-footer">
                    <button type="button" class="btn btn-outline-secondary btn-secondary-action">${secondaryText || "Để sau"}</button>
                    <button type="button" class="btn btn-danger btn-primary-action">${primaryText || "Đăng xuất"}</button>
                  </div>

                </div>
              </div>
            </div>
        `;

        var wrapper = document.createElement("div");
        wrapper.innerHTML = html.trim();
        var modal = wrapper.firstChild;

        currentModalEl = modal;
        var backdrop = document.createElement("div");
        backdrop.className = "modal-backdrop fade show";
        currentBackdropEl = backdrop;

        document.body.appendChild(backdrop);
        document.body.appendChild(modal);
        document.body.classList.add("modal-open");
        document.body.style.overflow = "hidden";

        modal.querySelector(".btn-close").addEventListener("click", closeModal);

        var primaryBtn = modal.querySelector(".btn-primary-action");
        if (primaryBtn) {
            primaryBtn.addEventListener("click", function () {
                if (primaryHref) {
                    window.location.href = primaryHref;
                } else {
                    closeModal();
                }
            });
        }

        var secondaryBtn = modal.querySelector(".btn-secondary-action");
        if (secondaryBtn) {
            secondaryBtn.addEventListener("click", function () {
                if (secondaryHref) {
                    window.location.href = secondaryHref;
                } else {
                    closeModal();
                }
            });
        }

        if (allowBackdropClose === true) {
            modal.addEventListener("click", function (e) {
                if (e.target === modal) closeModal();
            });
        }
    };
})();
/*END SHOW MODAL 2 BUTTONS*/
/*END SHOW MODAL*/

/*END SELECT ALL*/
document.addEventListener('change', function (e) {
    if (e.target.id === 'chkAll') {
        var checked = e.target.checked;
        document.querySelectorAll('.row-checkbox').forEach(function (cb) {
            cb.checked = checked;
        });
    }
});
/*END SELECT ALL*/
/*UPLOAD ẢNH VÀ XÓA ẢNH BẰNG HANDEL */
function uploadFile(fileInputId, messageId, previewId, textboxClientId) {
    var fileInput = document.getElementById(fileInputId);
    var messageDiv = document.getElementById(messageId);

    // reset message
    if (messageDiv) messageDiv.innerHTML = "";

    if (!fileInput || fileInput.files.length === 0) {
        if (messageDiv) messageDiv.innerHTML = "Vui lòng chọn file.";
        return;
    }

    var file = fileInput.files[0];
    var allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".heic"];
    var ext = file.name.substr(file.name.lastIndexOf(".")).toLowerCase();

    // ✅ CHECK LOẠI FILE Ở CLIENT
    if (!allowedExtensions.includes(ext)) {
        if (messageDiv) messageDiv.innerHTML = "Định dạng ảnh không hợp lệ (client).";
        return;
    }

    // ✅ CHECK KÍCH THƯỚC Ở CLIENT
    if (file.size > 10 * 1024 * 1024) {
        if (messageDiv) messageDiv.innerHTML = "Vui lòng chọn file nhỏ hơn 10 MB.";
        return;
    }

    var formData = new FormData();
    formData.append("file", file);

    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/img/Upload_Handler_Style1.ashx", true);

    xhr.onload = function () {
        // LẤY previewDiv LẠI LÚC NÀY (đảm bảo DOM sẵn sàng)
        var previewDiv = document.getElementById(previewId);
        if (!previewDiv) {
            console.error("Không tìm thấy phần tử preview với id:", previewId);
            return;
        }

        // Handler đã set StatusCode = 400 và Write("Định dạng ảnh không hợp lệ.")
        // ta đọc luôn message đó:
        if (xhr.status === 200) {
            var imageUrl = xhr.responseText;

            previewDiv.innerHTML = `
                <div id="${previewId}_wrap">
                    <img width="100" src="${imageUrl}" class="img-thumbnail" /><br/>
                    <button type="button"
                        class="btn btn-danger btn-sm mt-1"
                        onclick="removeUpload('${previewId}_wrap', '${fileInputId}', '${textboxClientId}')">
                        Xóa
                    </button>
                </div>`;

            document.getElementById(textboxClientId).value = imageUrl;
        } else {
            // ❗ HIỂN THỊ CHUẨN THÔNG ĐIỆP TỪ HANDLER
            if (messageDiv) {
                var msg = xhr.responseText || "Lỗi upload.";
                messageDiv.innerHTML = "<span class='text-danger'>" + msg + "</span>";
            }
        }
    };

    xhr.onerror = function () {
        if (messageDiv) messageDiv.innerHTML = "<span class='text-danger'>Lỗi mạng khi upload.</span>";
    };

    xhr.send(formData);
}

function removeUpload(previewId, fileInputId, textboxClientId) {
    var textbox = document.getElementById(textboxClientId);
    if (!textbox) return;

    var imageUrl = textbox.value;
    if (imageUrl) {
        // Gọi handler để xóa ảnh trên server
        fetch("/img/Delete_Image.ashx", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ path: imageUrl })
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    console.log("Đã xóa ảnh trên server");
                } else {
                    console.error("Lỗi xóa ảnh:", data.error);
                }
            })
            .catch(err => console.error("Fetch error:", err));
    }

    // Xóa UI + reset control
    var preview = document.getElementById(previewId);
    if (preview) preview.innerHTML = "";

    var fileInput = document.getElementById(fileInputId);
    if (fileInput) fileInput.value = "";

    textbox.value = "";
}

/*END ẢNH VÀ XÓA ẢNH BẰNG HANDEL */

/*CHỐNG SUBMIT 2 LẦN*/
var isSubmitting = false;
function preventDoubleSubmit(btn) {
    if (isSubmitting) return false;
    isSubmitting = true;

    btn.disabled = true;
    btn.value = "Đang xử lý...";

    return true; // cho phép submit 1 lần
}
if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
    var prm = Sys.WebForms.PageRequestManager.getInstance();

    prm.add_endRequest(function (sender, args) {
        // Mỗi lần async postback xong thì reset trạng thái submit
        isSubmitting = false;

        var btn = document.getElementById('btnSaveShow');
        if (btn) {
            btn.disabled = false;
        }
    });
}

/*CHỐNG SUBMIT 2 LẦN*/
