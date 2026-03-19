/**
 * @license Copyright (c) 2003-2021, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function( config ) {
    // Ngăn CKEditor tự động thêm thẻ <p>
    config.autoParagraph = false;

    // Ngăn thêm nội dung vào khối trống
    config.fillEmptyBlocks = false;

    // Chỉ định các thẻ định dạng hợp lệ
    config.format_tags = 'p;h1;h2;h3;pre';

    // Vô hiệu hóa bộ lọc nội dung nâng cao
    config.allowedContent = true;

    // Cho phép mọi thẻ và thuộc tính
    config.extraAllowedContent = '*';

    // Không xóa bất kỳ thuộc tính nào khi loại bỏ định dạng
    config.removeFormatAttributes = '';

    // Ngăn tự động chuyển đổi thẻ đơn sang thẻ đóng/mở
    config.forceSimpleAmpersand = true;

    // Ngăn chuyển đổi các ký tự HTML
    config.htmlEncodeOutput = false;
    // Cấu hình thanh công cụ
    config.toolbar = [

        { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline'] },

    ];
};
