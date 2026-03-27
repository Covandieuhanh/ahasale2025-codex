/*thông báo updateting*/
function dialog_updating() { Metro.dialog.create({ title: 'Thông báo', content: '<div>Chức năng đang được cập nhật. Vui lòng thử lại sau.</div>', closeButton: false, overlayClickClose: false, actions: [{ caption: 'OK', cls: 'js-dialog-close alert' }] }); }
/*thông báo đã sao chép thành công*/
function thongbao_dasaochep() { Metro.notify.create("Sao chép thành công.", "Thông báo", {}); }
/*định dạng phần ngàn khi nhập số vào textbox*/
function format_sotien_new(textbox) { var value = parseFloat(textbox.value.replace(/\./g, '').replace(',', '.')); if (!isNaN(value)) { textbox.value = value.toLocaleString('de-DE', { minimumFractionDigits: 0, maximumFractionDigits: 3 }); } else { textbox.value = ''; } }
/*TỰ ĐỘNG BÔI ĐEN KHI CHỌN VÀO TEXTBOX*/
function AutoSelect(textBox) { textBox.select(); }

//no enter checkbox and textsearch in datatable
$(function () { $('.table-component').each(function () { $(this).find('input').keypress(function (e) { if (e.which == 13) { return false; } }); }); });
//no enter select dropdownbox
$(function () { $('.select').each(function () { $(this).find('input').keypress(function (e) { if (e.which == 13) { return false; } }); }); });
//no enter box search
$(function () { $('#icon-search').each(function () { $(this).find('input').keypress(function (e) { if (e.which == 13) { return false; } }); }); });
//back to top
/*jQuery(document).ready(function ($) { if ($(window).scrollTop() > 200) { $('#back-to-top').fadeIn(); } else { $('#back-to-top').fadeOut(); } $(window).scroll(function () { if ($(this).scrollTop() > 200) { $('#back-to-top').fadeIn(); } else { $('#back-to-top').fadeOut(); } }); $('#back-to-top').click(function () { $("html, body").animate({ scrollTop: 0 }, 600); return false; }); });*/
$(function () { $.fn.scrollToTop = function () { $(this).hide().removeAttr("href"); if ($(window).scrollTop() != "0") { $(this).fadeIn("slow") } var scrollDiv = $(this); $(window).scroll(function () { if ($(window).scrollTop() == "0") { $(scrollDiv).fadeOut("slow") } else { $(scrollDiv).fadeIn("slow") } }); $(this).click(function () { $("html, body").animate({ scrollTop: 0 }, "slow") }) } }); $(function () { $("#back-to-top").scrollToTop(); });
//manager files ckfinder
function BrowseServer_Uploads() { var finder = new CKFinder(); finder.basePath = '/ckfinder/'; finder.popup(); };
//Copy to Clipboard
function copytoclipboard(myID) { var copyText = document.getElementById(myID); copyText.select(); copyText.setSelectionRange(0, 99999); document.execCommand("copy"); alert("Copied the text: " + copyText.value); };
//tawk.to
//var Tawk_API = Tawk_API || {}, Tawk_LoadStart = new Date(); (function () { var s1 = document.createElement("script"), s0 = document.getElementsByTagName("script")[0]; s1.async = true; s1.src = 'https://embed.tawk.to/5e9d654d35bb0c9ab2d4b3/default'; s1.charset = 'UTF-8'; s1.setAttribute('crossorigin', '*'); s0.parentNode.insertBefore(s1, s0); })();

//xử lý iframe
$(document).ready(function () {
    var w = $('.video-full').width();
    if (w > 800) {
        $('.video-full iframe').css({ "width": w });
        $('.video-full iframe').css({ "height": w / 1.91 });
    }
    else {
        $('.video-full iframe').css({ "width": w });
        $('.video-full iframe').css({ "height": w / 1.91 });
    }
    var w1 = $('.video-50').width();
    if (w1 > 800) {
        $('.video-50 iframe').css({ "width": w1 });
        $('.video-50 iframe').css({ "height": w1 / 1.91 });
    }
    else {
        $('.video-50 iframe').css({ "width": w1 });
        $('.video-50 iframe').css({ "height": w1 / 1.91 });
    }
});
$(function () {
    $(window).resize(function () {
        var w = $('.video-full').width();
        if (w > 800) {
            $('.video-full iframe').css({ "width": 800 });
            $('.video-full iframe').css({ "height": 800 / 1.91 });
        }
        else {
            $('.video-full iframe').css({ "width": w });
            $('.video-full iframe').css({ "height": w / 1.91 });
        }
        var w1 = $('.video-50').width();
        if (w1 > 800) {
            $('.video-50 iframe').css({ "width": 800 });
            $('.video-50 iframe').css({ "height": 800 / 1.91 });
        }
        else {
            $('.video-50 iframe').css({ "width": w1 });
            $('.video-50 iframe').css({ "height": w1 / 1.91 });
        }
    });
});
//xử lý iframe

//$(document).ready(function () {
//    var browserWidth = window.innerWidth;
//    //$('#nqb').css({ "width": browserWidth});
//    alert(browserWidth);
//});
//$(function () {
//    $(window).resize(function () {
//        var browserWidth = window.innerWidth;
//        //$('#nqb').css({ "width": browserWidth});
//        alert(browserWidth);
//    });
//});

