(function (window) {
    function byId(id) {
        return id ? document.getElementById(id) : null;
    }

    function bindOnce(element, key, callback) {
        if (!element || !key || typeof callback !== 'function') return;
        var marker = '__ahaFastBound_' + key;
        if (element[marker]) return;
        element[marker] = true;
        callback(element);
    }

    function setSelectValue(select, value) {
        if (!select || !value) return;
        for (var i = 0; i < select.options.length; i++) {
            if ((select.options[i].value || '') === value) {
                select.selectedIndex = i;
                return;
            }
        }
    }

    function wireEnterToButton(input, button) {
        if (!input || !button) return;
        bindOnce(input, 'enter-' + (button.id || 'button'), function (node) {
            node.addEventListener('keydown', function (event) {
                if (event.key !== 'Enter') return;
                event.preventDefault();
                button.click();
            });
        });
    }

    function fetchJson(url, callback) {
        if (!window.fetch) return;
        fetch(url, {
            credentials: 'same-origin',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        }).then(function (response) {
            return response.json();
        }).then(function (data) {
            callback(data || {});
        }).catch(function () { });
    }

    function trim(value) {
        return (value || '').replace(/^\s+|\s+$/g, '');
    }

    function initSearchSubmit(config) {
        var input = byId(config.inputId);
        var button = byId(config.buttonId);
        wireEnterToButton(input, button);
    }

    function initCustomerLookup(config) {
        var endpoint = config.endpoint || '';
        var phone = byId(config.phoneId);
        var name = byId(config.nameId);
        var address = byId(config.addressId);
        var care = byId(config.careId);
        var group = byId(config.groupId);

        function apply(item, source) {
            if (!item) return;
            if (name && item.name && (source === 'phone' || trim(name.value) === '')) {
                name.value = item.name;
            }
            if (phone && item.phone && (source === 'name' || trim(phone.value) === '')) {
                phone.value = item.phone;
            }
            if (address && item.address) {
                address.value = item.address;
            }
            setSelectValue(care, item.careUser || '');
            setSelectValue(group, item.groupId || '');
        }

        if (phone) {
            bindOnce(phone, 'customer-phone', function (node) {
                node.addEventListener('change', function () {
                    var value = trim(phone.value);
                    if (value.length < 6) return;
                    fetchJson(endpoint + '?mode=customer-phone&q=' + encodeURIComponent(value), function (data) {
                        if (data && data.ok && data.item) {
                            apply(data.item, 'phone');
                        }
                    });
                });
            });
        }

        if (name) {
            bindOnce(name, 'customer-name', function (node) {
                node.addEventListener('change', function () {
                    var value = trim(name.value);
                    if (value.length < 2 || (phone && trim(phone.value) !== '')) return;
                    fetchJson(endpoint + '?mode=customer-name&q=' + encodeURIComponent(value), function (data) {
                        if (data && data.ok && data.item) {
                            apply(data.item, 'name');
                        }
                    });
                });
            });
        }
    }

    function initItemLookup(config) {
        var endpoint = config.endpoint || '';
        var mode = config.mode || '';
        var input = byId(config.inputId);
        var price = byId(config.priceId);
        var unit = byId(config.unitId);
        var sale = byId(config.saleId);
        var performer = byId(config.performerId);

        if (!input || !mode) return;

        bindOnce(input, 'item-' + mode, function (node) {
            node.addEventListener('change', function () {
                var value = trim(input.value);
                if (value.length < 2) return;
                fetchJson(endpoint + '?mode=' + encodeURIComponent(mode) + '&q=' + encodeURIComponent(value), function (data) {
                    if (!data || !data.ok || !data.item) return;
                    var item = data.item;
                    if (price && item.priceText) {
                        price.value = item.priceText;
                    }
                    if (unit) {
                        unit.value = item.unitText || '';
                    }
                    if (sale && item.salePercentText !== undefined && item.salePercentText !== null && item.salePercentText !== '') {
                        sale.value = item.salePercentText;
                    }
                    if (performer && item.performerPercentText !== undefined && item.performerPercentText !== null && item.performerPercentText !== '') {
                        performer.value = item.performerPercentText;
                    }
                });
            });
        });
    }

    window.ahaInvoiceFast = {
        initSearchSubmit: initSearchSubmit,
        initCustomerLookup: initCustomerLookup,
        initItemLookup: initItemLookup
    };
})(window);
