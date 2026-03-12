(function () {
    var DATA_URLS = {
        province: "/assets/data/tinh_tp.json",
        district: "/assets/data/quan_huyen.json",
        ward: "/assets/data/xa_phuong.json"
    };

    var dataPromise = null;

    function fetchJson(url) {
        return fetch(url, { cache: "force-cache" }).then(function (res) {
            if (!res.ok) {
                throw new Error("Không tải được dữ liệu địa chỉ: " + url);
            }
            return res.json();
        });
    }

    function removeDiacritics(value) {
        var s = String(value || "");
        try {
            s = s.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
        } catch (e) {
        }
        s = s.replace(/đ/g, "d").replace(/Đ/g, "D");
        return s;
    }

    function normalizeName(value) {
        var s = removeDiacritics(value).toLowerCase();
        s = s.replace(/\b(tp\.?|thanh pho|tinh|quan|huyen|phuong|xa|thi xa|thi tran)\b/g, " ");
        s = s.replace(/[^a-z0-9]+/g, "");
        if (s === "hcm" || s === "tphcm") s = "hochiminh";
        if (s === "hn" || s === "tphanoi") s = "hanoi";
        return s.trim();
    }

    function mapToList(raw) {
        var list = [];
        for (var key in raw) {
            if (!raw.hasOwnProperty(key)) continue;
            var item = raw[key] || {};
            var name = item.name || "";
            var display = item.name_with_type || name || "";
            list.push({
                code: item.code || key,
                name: name,
                display: display,
                parent_code: item.parent_code || "",
                norm: normalizeName(display),
                normName: normalizeName(name)
            });
        }
        list.sort(function (a, b) {
            return a.display.localeCompare(b.display, "vi-VN");
        });
        return list;
    }

    function buildData(provinceRaw, districtRaw, wardRaw) {
        var provinces = mapToList(provinceRaw);
        var districts = mapToList(districtRaw);
        var wards = mapToList(wardRaw);

        var districtsByProvince = {};
        for (var i = 0; i < districts.length; i++) {
            var d = districts[i];
            if (!districtsByProvince[d.parent_code]) districtsByProvince[d.parent_code] = [];
            districtsByProvince[d.parent_code].push(d);
        }

        var wardsByDistrict = {};
        for (var j = 0; j < wards.length; j++) {
            var w = wards[j];
            if (!wardsByDistrict[w.parent_code]) wardsByDistrict[w.parent_code] = [];
            wardsByDistrict[w.parent_code].push(w);
        }

        return {
            provinces: provinces,
            districts: districts,
            wards: wards,
            districtsByProvince: districtsByProvince,
            wardsByDistrict: wardsByDistrict
        };
    }

    function loadData() {
        if (dataPromise) return dataPromise;
        dataPromise = Promise.all([
            fetchJson(DATA_URLS.province),
            fetchJson(DATA_URLS.district),
            fetchJson(DATA_URLS.ward)
        ]).then(function (values) {
            return buildData(values[0], values[1], values[2]);
        });
        return dataPromise;
    }

    function buildOptions(select, items, placeholder) {
        if (!select) return;
        if (select.tomselect) {
            try { select.tomselect.destroy(); } catch (e) { }
        }
        select.innerHTML = "";
        if (placeholder) {
            select.setAttribute("data-placeholder", placeholder);
        }
        var optPlaceholder = document.createElement("option");
        optPlaceholder.value = "";
        optPlaceholder.textContent = placeholder || "--";
        select.appendChild(optPlaceholder);
        for (var i = 0; i < items.length; i++) {
            var opt = document.createElement("option");
            opt.value = items[i].code;
            opt.textContent = items[i].display;
            select.appendChild(opt);
        }

        if (select.getAttribute("data-no-tomselect") === "1") {
            return;
        }

        if (window.initOneTomSelect) {
            window.initOneTomSelect(select);
        } else if (select.tomselect && typeof select.tomselect.sync === "function") {
            select.tomselect.sync();
            if (typeof select.tomselect.refreshOptions === "function") {
                select.tomselect.refreshOptions(false);
            }
        }
    }

    function findByPart(items, part) {
        if (!part) return null;
        var norm = normalizeName(part);
        if (!norm) return null;
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            if (item.norm === norm || item.normName === norm) return item;
        }
        return null;
    }

    function guessFromRaw(raw, data) {
        var rawText = String(raw || "").trim();
        if (!rawText) return null;
        var parts = rawText.split(",").map(function (p) { return p.trim(); }).filter(Boolean);
        if (!parts.length) return null;

        var province = null;
        for (var i = parts.length - 1; i >= 0; i--) {
            province = findByPart(data.provinces, parts[i]);
            if (province) {
                parts.splice(i, 1);
                break;
            }
        }

        var district = null;
        if (province) {
            var districtList = data.districtsByProvince[province.code] || [];
            for (var j = parts.length - 1; j >= 0; j--) {
                district = findByPart(districtList, parts[j]);
                if (district) {
                    parts.splice(j, 1);
                    break;
                }
            }
        }

        var ward = null;
        if (district) {
            var wardList = data.wardsByDistrict[district.code] || [];
            for (var k = parts.length - 1; k >= 0; k--) {
                ward = findByPart(wardList, parts[k]);
                if (ward) {
                    parts.splice(k, 1);
                    break;
                }
            }
        }

        var detail = parts.join(", ").trim();
        return {
            province: province,
            district: district,
            ward: ward,
            detail: detail,
            raw: rawText
        };
    }

    function buildFullAddress(detail, wardName, districtName, provinceName) {
        var parts = [];
        if (detail) parts.push(detail);
        if (wardName) parts.push(wardName);
        if (districtName) parts.push(districtName);
        if (provinceName) parts.push(provinceName);
        return parts.join(", ").trim();
    }

    function safeGetValue(el) {
        if (!el) return "";
        return String(el.value || "").trim();
    }

    function setHidden(el, value) {
        if (!el) return;
        el.value = value || "";
    }

    function initPicker(opts) {
        if (!opts) return;
        var provinceSelect = document.getElementById(opts.provinceSelectId);
        var districtSelect = document.getElementById(opts.districtSelectId);
        var wardSelect = document.getElementById(opts.wardSelectId);
        var detailInput = document.getElementById(opts.detailInputId);
        var hiddenAddress = document.getElementById(opts.hiddenAddressId);
        var hiddenProvince = document.getElementById(opts.hiddenProvinceId);
        var hiddenDistrict = document.getElementById(opts.hiddenDistrictId);
        var hiddenWard = document.getElementById(opts.hiddenWardId);
        var rawAddressEl = opts.rawAddressId ? document.getElementById(opts.rawAddressId) : null;

        if (!provinceSelect || !districtSelect || !wardSelect) return;
        if (provinceSelect.dataset.ahaAddressBound === "1" || provinceSelect.dataset.ahaAddressBound === "pending") return;
        provinceSelect.dataset.ahaAddressBound = "pending";

        loadData().then(function (data) {
            buildOptions(provinceSelect, data.provinces, "Chọn Tỉnh/Thành");
            buildOptions(districtSelect, [], "Chọn Quận/Huyện");
            buildOptions(wardSelect, [], "Chọn Phường/Xã");

            var rawAddress = rawAddressEl ? rawAddressEl.value : "";
            var guess = guessFromRaw(rawAddress, data);
            if (guess && guess.province) {
                provinceSelect.value = guess.province.code;
                var districts = data.districtsByProvince[guess.province.code] || [];
                buildOptions(districtSelect, districts, "Chọn Quận/Huyện");
                if (guess.district) {
                    districtSelect.value = guess.district.code;
                    var wards = data.wardsByDistrict[guess.district.code] || [];
                    buildOptions(wardSelect, wards, "Chọn Phường/Xã");
                    if (guess.ward) {
                        wardSelect.value = guess.ward.code;
                    }
                }
            }

            if (detailInput) {
                if (guess && (guess.detail || guess.province || guess.district || guess.ward)) {
                    detailInput.value = guess.detail || "";
                } else if (rawAddress) {
                    detailInput.value = rawAddress;
                }
            }

            function syncHidden() {
                var provinceName = provinceSelect.value && provinceSelect.selectedOptions.length
                    ? provinceSelect.selectedOptions[0].textContent
                    : "";
                var districtName = districtSelect.value && districtSelect.selectedOptions.length
                    ? districtSelect.selectedOptions[0].textContent
                    : "";
                var wardName = wardSelect.value && wardSelect.selectedOptions.length
                    ? wardSelect.selectedOptions[0].textContent
                    : "";
                var detail = detailInput ? detailInput.value.trim() : "";

                setHidden(hiddenProvince, provinceName);
                setHidden(hiddenDistrict, districtName);
                setHidden(hiddenWard, wardName);

                var fullAddress = buildFullAddress(detail, wardName, districtName, provinceName);
                setHidden(hiddenAddress, fullAddress);
            }

            function onProvinceChange() {
                var code = safeGetValue(provinceSelect);
                var districts = code ? (data.districtsByProvince[code] || []) : [];
                buildOptions(districtSelect, districts, "Chọn Quận/Huyện");
                buildOptions(wardSelect, [], "Chọn Phường/Xã");
                syncHidden();
            }

            function onDistrictChange() {
                var code = safeGetValue(districtSelect);
                var wards = code ? (data.wardsByDistrict[code] || []) : [];
                buildOptions(wardSelect, wards, "Chọn Phường/Xã");
                syncHidden();
            }

            function onWardChange() {
                syncHidden();
            }

            provinceSelect.addEventListener("change", onProvinceChange);
            districtSelect.addEventListener("change", onDistrictChange);
            wardSelect.addEventListener("change", onWardChange);
            if (detailInput) detailInput.addEventListener("input", syncHidden);

            syncHidden();

            provinceSelect.dataset.ahaAddressBound = "1";
        }).catch(function () {
            if (provinceSelect) provinceSelect.disabled = true;
            if (districtSelect) districtSelect.disabled = true;
            if (wardSelect) wardSelect.disabled = true;
            provinceSelect.dataset.ahaAddressBound = "";
        });
    }

    function setSelectValue(select, value) {
        if (!select) return;
        var nextValue = value || "";
        if (select.tomselect && typeof select.tomselect.setValue === "function") {
            select.tomselect.setValue(nextValue, true);
            if (typeof select.tomselect.refreshOptions === "function") {
                select.tomselect.refreshOptions(false);
            }
        } else {
            select.value = nextValue;
        }
        select.dispatchEvent(new Event("change", { bubbles: true }));
    }

    function applyRaw(opts, rawAddress) {
        if (!opts) return;
        var provinceSelect = document.getElementById(opts.provinceSelectId);
        var districtSelect = document.getElementById(opts.districtSelectId);
        var wardSelect = document.getElementById(opts.wardSelectId);
        var detailInput = document.getElementById(opts.detailInputId);
        var rawAddressEl = opts.rawAddressId ? document.getElementById(opts.rawAddressId) : null;

        if (!provinceSelect || !districtSelect || !wardSelect) return;

        loadData().then(function (data) {
            buildOptions(provinceSelect, data.provinces, "Chọn Tỉnh/Thành");
            buildOptions(districtSelect, [], "Chọn Quận/Huyện");
            buildOptions(wardSelect, [], "Chọn Phường/Xã");

            var guess = guessFromRaw(rawAddress, data);
            if (guess && guess.province) {
                setSelectValue(provinceSelect, guess.province.code);
                if (guess.district) {
                    setSelectValue(districtSelect, guess.district.code);
                }
                if (guess.ward) {
                    setSelectValue(wardSelect, guess.ward.code);
                }
            } else {
                setSelectValue(provinceSelect, "");
                setSelectValue(districtSelect, "");
                setSelectValue(wardSelect, "");
            }

            if (detailInput) {
                if (guess && (guess.detail || guess.province || guess.district || guess.ward)) {
                    detailInput.value = guess.detail || "";
                } else {
                    detailInput.value = rawAddress || "";
                }
                detailInput.dispatchEvent(new Event("input", { bubbles: true }));
            }

            if (rawAddressEl) {
                rawAddressEl.value = rawAddress || "";
            }
        });
    }

    window.AhaAddressPicker = {
        init: initPicker,
        applyRaw: applyRaw
    };
})();
