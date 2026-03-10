# DEPLOY CHECKLIST - AHA SALE HOST

## A. Truoc deploy
1. Xac nhan release package dung timestamp va dung branch.
2. Chot danh sach file thay doi (`files_changed.txt` neu co).
3. Backup code host hien tai.
4. Backup DB production.
5. Chuan bi danh sach SQL can chay.

## B. Upload + migrate
1. Bat `app_offline.htm`.
2. Upload code:
   - Release lon: upload full `site/`.
   - Hotfix: upload dung danh sach file ghi de.
3. Chay SQL idempotent scripts can thiet (toi thieu: `ensure-shop-slug-schema.sql`, `ensure-donhang-state-schema.sql`, `ensure-hanhvi-schema.sql`, `ensure-reset-security-schema.sql` neu release co reset bao mat).
4. Recycle app pool.
5. Go `app_offline.htm`.

## C. Smoke test nhanh (bat buoc)
1. `https://ahasale.vn/`
2. `https://ahasale.vn/admin/`
3. `https://ahasale.vn/admin/login.aspx`
4. `https://ahasale.vn/shop/`
5. `https://ahasale.vn/shop/login.aspx`
6. 1 link chi tiet tin (`*.html`)
7. Login admin/home/shop va kiem tra khong redirect sai he.

## D. Smoke test nghiep vu
1. Admin: vao 1 tab quan ly tai khoan, 1 tab duyet, 1 tab phat hanh/ban san pham.
2. Home: mo dropdown tai khoan desktop + mobile.
3. Shop: tao don > sang cho thanh toan > kiem tra redirect.
4. Kiem tra anh/logo/uploads tren trang chu.

## E. Loi hay gap va xu ly nhanh
1. Loi compile class khong ton tai:
   - Kiem tra file trong `App_Code` da upload du chua.
   - Kiem tra file menu/top-level co goi class optional khong.
2. Loi cot DB khong ton tai:
   - Chay lai script `ensure-*.sql` tu release.
3. Redirect loop:
   - Xoa cookie theo domain.
   - Kiem tra flow login scope (admin/home/shop).
4. Dropdown trang tren mobile:
   - Kiem tra cap CSS+JS da upload dong bo.

## F. Sau deploy
1. Luu release note: ngay gio, package, scripts, ket qua smoke test.
2. Luu backup it nhat 1 ban gan nhat de rollback nhanh.
