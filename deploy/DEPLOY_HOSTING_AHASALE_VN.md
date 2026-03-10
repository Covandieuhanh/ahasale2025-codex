# Deploy AhaSale len Hosting (ahasale.vn)

Tai lieu tong hop va checklist chuan da duoc luu tai:
- `docs/host-compat/RULEBOOK.md`
- `docs/host-compat/DEPLOY_CHECKLIST_AHASALE.md`

## 1) Truoc khi upload
1. Backup code host.
2. Backup database.
3. Chuan bi goi release + danh sach SQL.

## 2) Cau hinh bat buoc
1. `site/Web.config`: dien day du gia tri `CHANGE_ME_*`.
2. Neu dung bridge watcher: cap nhat `usdt_bridge.hosting.env` dong bo key/secrets.

## 3) Upload
1. Bat `app_offline.htm`.
2. Upload full `site/` (khuyen nghi) hoac overwrite package co danh sach file ro rang.
3. Chay SQL scripts idempotent can thiet (dac biet: `ensure-shop-slug-schema.sql`, `ensure-donhang-state-schema.sql`, `ensure-hanhvi-schema.sql`, `ensure-reset-security-schema.sql` neu release co reset bao mat).
4. Recycle app pool.
5. Go `app_offline.htm`.

## 4) Smoke test toi thieu
1. `/admin/` va `/admin/login.aspx` khong loi compile.
2. `/shop/` va `/shop/login.aspx` truy cap duoc.
3. Home page, chi tiet tin, dropdown mobile hoat dong binh thuong.
4. Luong dang nhap admin/home/shop dung scope.

## 5) Loi da gap truoc day (can kiem tra lai)
1. Thieu class trong `App_Code` -> loi CS0103 o admin menu.
2. Thieu cot DB (`order_status`, `exchange_status`) -> loi runtime chi tiet tin.
3. Redirect loop o luong cho-thanh-toan -> test sau deploy.
4. Dropdown mobile trang -> deploy dong bo CSS + JS.
