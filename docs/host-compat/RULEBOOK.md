# HOST-COMPAT RULEBOOK (AHA SALE 2025)

- Version: `2.0`
- Project: `AHA_SALE_2025`
- Owner: `AHA Sale engineering`
- Last Updated: `2026-03-09`

## 1) Muc tieu
1. Deploy len host la chay ngay, khong phat sinh loi compile/runtime do thieu phu thuoc.
2. Khong tao ra 2 logic song song local/host.
3. Moi lan release deu co quy trinh backup, migrate, smoke test va rollback ro rang.

## 2) Baseline moi truong host
1. WebForms/Web Site runtime compile tren host (IIS + .NET Framework 4.x).
2. SQL Server la nguon du lieu chinh, migration phai idempotent.
3. HTTPS la luong chay chinh tren production (`https://ahasale.vn`).

## 3) Quy tac bat buoc ve source deploy
1. Deploy theo source, KHONG phu thuoc file build trung gian tren may dev.
2. Khi module co class trong `App_Code`, phai deploy dong bo file class va file goi class do.
3. Khong deploy nua voi cach "copy 1 file code-behind" neu file do co them dependency moi.
4. Moi goi deploy phai co:
   - `site/` (hoac danh sach file ghi de)
   - `scripts/sql/*.sql` can chay
   - `README_DEPLOY.txt` ghi ro thu tu thuc hien

## 4) Quy tac code de tranh loi compile tren host
1. Menu/top-level control khong duoc phu thuoc truc tiep vao class "optional module".
2. Check permission nen dung ma quyen on dinh (vi du `q3_1`) hoac wrapper an toan.
3. Khong dung API/C# feature vuot baseline host.
4. Dynamic control phai rebind day du trong postback de tranh `Invalid postback or callback argument`.
5. Khong hardcode path may ca nhan.

## 5) Quy tac schema DB
1. Moi thay doi schema bat buoc co script SQL idempotent.
2. Script ten ro nghiep vu va dat trong `scripts/sql/`.
3. Cac script "ensure" quan trong hien tai:
   - `ensure-shop-slug-schema.sql`
   - `ensure-donhang-state-schema.sql`
   - `ensure-hanhvi-schema.sql`
   - `ensure-reset-security-schema.sql`
   - `create-home-content-cms.sql` (neu module noi dung home dang duoc su dung)
4. Tranh sua tay schema tren host ma khong cap nhat script.

## 6) Thu tu deploy chuan (bat buoc)
1. Backup code host hien tai.
2. Backup DB truoc migrate.
3. Bat `app_offline.htm` (neu co) de tranh ghi giao dich dang do.
4. Upload code (full package hoac overwrite da co danh sach ro rang).
5. Chay SQL scripts theo danh sach trong release note.
6. Recycle App Pool / restart site.
7. Remove `app_offline.htm`.
8. Warmup URL quan trong.
9. Chay smoke test.
10. Neu loi nghiem trong: rollback trong 15 phut.

## 7) Smoke test toi thieu sau deploy
1. `/admin/` va `/admin/login.aspx` khong con `Compilation Error`.
2. Login admin/home/shop dung scope (khong redirect sai he).
3. `/shop` va `/shop/login.aspx` truy cap duoc.
4. Chi tiet tin (`*.html`) khong loi cot DB.
5. Dropdown tai khoan tren mobile khong bi trang/trang thai nhay roi mat noi dung.
6. Chuc nang giao dich quan trong (ban san pham/phat hanh the/cho thanh toan) khong loi redirect loop.
7. Anh statics/uploads hien thi binh thuong.

## 8) Cac loi da gap va cach phong tranh (bat buoc nho)
1. `CS0103 HomeContentBlock_cl does not exist`:
   - Nguyen nhan: host thieu file class trong `App_Code` hoac partial deploy thieu dependency.
   - Phong tranh: deploy dong bo `App_Code` lien quan HOAC code menu dung permission constant, khong goi class optional truc tiep.
2. `Invalid column name order_status/exchange_status`:
   - Nguyen nhan: schema host chua du cot moi.
   - Phong tranh: chay `ensure-donhang-state-schema.sql` truoc khi mo traffic.
3. `Invalid postback or callback argument`:
   - Nguyen nhan: dynamic dropdown khong rebind dung lifecycle.
   - Phong tranh: bind datasource day du tren postback + event validation hop le.
4. `ERR_TOO_MANY_REDIRECTS`:
   - Nguyen nhan: xung dot flow session/cookie/redirect giua home-shop.
   - Phong tranh: test login scope + chuyen he truoc deploy, giu domain canonical duy nhat.
5. Mobile dropdown trang:
   - Nguyen nhan: mismatch JS/CSS/deploy khong dong bo.
   - Phong tranh: deploy cap file CSS+JS theo cap, tang query version cache-bust.

## 9) Quy tac release package
1. Muc tieu uu tien: full package cho ban release lon.
2. Hotfix nho: cho phep overwrite package nhung phai kem danh sach file ro rang.
3. Moi package release phai co timestamp (`YYYYMMDD_HHMMSS`).
4. Luu package trong `deploy/` de truy vet.

## 10) Rollback runbook (<= 15 phut)
1. Dat `app_offline.htm`.
2. Restore code tu backup gan nhat.
3. Restore DB neu migration lam vo logic.
4. Recycle app pool.
5. Kiem tra 3 URL: `/`, `/admin/`, `/shop/`.
6. Go `app_offline.htm`.

## 11) Definition of Done cho moi lan deploy
1. Checklist deploy PASS 100%.
2. Khong con compile/runtime error tren URL chinh.
3. SQL script da luu va co the chay lai khong vo (idempotent).
4. Co ghi chu release + danh sach file da thay doi.
5. Co goi rollback san sang.
