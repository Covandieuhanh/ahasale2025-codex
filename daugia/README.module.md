# MODULE Đấu giá

- Module code: `DauGia`
- Route: `/daugia`
- Route trung tâm quản lý: `/daugia/admin`
- Route quản lý: `/daugia/admin/quan-ly`
- Route tạo: `/daugia/admin/tao`
- Route quản trị hệ thống: `/admin/quan-ly-dau-gia`
- Alias legacy: `/home/daugia` và `/shop/dau-gia` (redirect về `/daugia/admin`; nhánh shop giữ `shop_portal=1`)
- Legacy compat: `/daugia/quan-ly(.aspx)` và `/daugia/tao(.aspx)` chỉ chuyển hướng sang `/daugia/admin/*`
- Gate quản lý:
- Home hợp lệ được vào trực tiếp `/daugia/admin/*` để quản lý phiên của chính tài khoản đó; admin chỉ can thiệp khi cần khóa/mở lại riêng
- Shop vào qua `shop_portal=1`, chưa login thì chuyển `/shop/login.aspx`
- Admin vào `/admin/quan-ly-dau-gia` dùng session admin (`check_login_admin`), không phụ thuộc Home-space
- Source type nội bộ: `manual_asset` (độc lập), `shop_post`, `home_quyen_uu_dai`, `home_quyen_tieu_dung`
- Cơ chế khóa nguồn: module tự khóa `source_type/source_id` trong `DG_AssetLock_tb`, tự mở khi phiên kết thúc/hủy/lỗi
- Cấu hình thời gian bắt đầu: khi `start_buffer_minutes > 0` thì phải cách hiện tại tối thiểu số phút cấu hình; khi `<= 0` cho phép tạo phiên bắt đầu ngay/lùi thời gian
- Docs module: `/Users/duccuongtran/Documents/Aha Sale 2025/docs/modules/daugia`

## Ghi nhớ

- Module này được sinh ra từ bộ khung chuẩn AhaSale
- Chỉ đưa logic riêng của module vào sau khi đã điền đủ bộ docs trong `docs/modules/daugia`
- Mọi liên kết lõi phải giữ trong không gian `/daugia` trước khi ghép với module khác
