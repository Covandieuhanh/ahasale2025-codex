# AhaSale Localhost Build

Ban nay da duoc tach rieng de chay local:
- Da doi `Web.config` sang DB container (`db:1433`, DB `ahasale_local`)
- Da xoa SMTP secret production
- Da them stack Docker tu dong restore DB + chay web

## Thu muc ban localhost
`/Users/duccuongtran/Documents/Aha Sale 2025/localhost/ahasale.vn.localhost`

## Yeu cau
- Docker Desktop

## Chay tu dong (khuyen nghi)
1. Mo Terminal:
   - `cd "/Users/duccuongtran/Documents/Aha Sale 2025/localhost/ahasale.vn.localhost"`
2. Chay:
   - `./scripts/start-localhost.sh`
3. Mo trinh duyet:
   - `http://localhost`
4. Mo cong admin song song:
   - `http://localhost:8081`
   - Tai khoan: `admin`
   - Mat khau: `123`

Script tren se:
- Khoi dong SQL Server container (port host `11433`)
- Tu dong restore backup `docker/sql/backup/ahasale.bak` vao DB `ahasale_local` neu chua co
- Build web container `nginx + mono-fastcgi` va chay web tren port `80`
- Khoi dong them cong admin rieng tren port `8081`
- Tu dong dong bo du lieu tai khoan phuc vu dang nhap (`scripts/sync-account-db.sh`)
- Tu dong dat tai khoan `admin` thanh full permission trong DB local
- Tu dong chay preflight guard dong bo naming `hanhvi` (`scripts/check-hanhvi-sync.sh`)

## Kiem tra dong bo hanhvi thu cong
Co the chay doc lap bat ky luc nao:
- `./scripts/check-hanhvi-sync.sh`

Guard se:
- Quet source active de phat hien token naming cu
- Xac minh token/files bat buoc cua naming moi `hanhvi`
- Kiem tra schema DB local (neu container `ahasale_local_db` dang chay)

## Ghi chu tuong thich da ap dung cho ban local Docker
- Da bo section `system.webServer` (IIS-only) trong `Web.config`
- Da bo tham chieu `PresentationCore` trong `Web.config`
- Da bat `batch="false"` trong `Web.config` de tranh loi compile batch do source goc co nhieu page trung class
- Da giu Roslyn provider trong `Bin/roslyn` va `Microsoft.CodeDom.Providers.DotNetCompilerPlatform.dll`
- Da giu `System.Threading.Tasks.dll` trong `Bin/disabled` de tranh xung dot namespace tren Mono
- Da vo hieu hoa cap file clone `home/cho-thanh-toan - Copy.aspx(.cs)` bang duoi `.disabled`
- Da mo day du menu admin (hien tat ca module admin trong source)
- Da them che do super-admin local cho tai khoan `admin` khi chay tren `localhost`
- Da nang cap dang nhap admin/home: chap nhan `tai khoan`, `email` hoac `so dien thoai`
- Cookie dang nhap se tu dong chuyen `Secure=true` chi khi chay HTTPS (de login localhost on dinh)

## Dung he thong
- `./scripts/stop-localhost.sh`

## Module Token -> Quyen tieu dung (vi tong)
Module doc lap da duoc them de lien ket 1 vi token BSC BEP20 vao 1 tai khoan vi tong tren web:
- Endpoint nhan webhook/noi bo: `POST /admin/api/usdt-bridge-credit.aspx`
- Core xu ly: `App_Code/USDTPointBridge_cl.cs`
- Config: `App_Code/USDTBridgeConfig_cl.cs` + cac key `USDTBridge.*` trong `Web.config`
- Inbound security: `App_Code/USDTBridgeSecurity_cl.cs`
- Replay/rate-limit/audit store: `App_Code/USDTBridgeSecurityStore_cl.cs`
- Watcher quet blockchain (BSC BEP20): `scripts/usdt_bridge_watcher.py`
- Ho tro 2 chieu:
  - `IN`: token nap vao vi theo doi -> cong diem
  - `OUT`: token rut ra khoi vi theo doi -> tru diem

### 1) Tao bang idempotent luu tx_hash
Chay SQL:
- `scripts/sql/create-usdt-point-bridge.sql`

Script SQL se tao:
- `USDT_Deposit_Bridge_tb` (idempotent theo `tx_hash`)
- `USDT_Bridge_Nonce_tb` (chong replay)
- `USDT_Bridge_Request_Log_tb` (audit + rate-limit window)

### 2) Cau hinh bridge key + vi tong
Co script cau hinh tu dong, khong can mo `Web.config`:
- `./scripts/configure-usdt-bridge.sh --provider rpc --deposit-address 0xVI_BSC_CUA_BAN`

Script se tu cap nhat `scripts/usdt_bridge.env` + `Web.config`, va restart `web` + `admin`.

Neu muon tu truyen key noi bo:
- `./scripts/configure-usdt-bridge.sh --provider rpc --deposit-address 0xVI_BSC_CUA_BAN --bridge-api-key KEY_NOI_BO --bridge-signing-secret SECRET_NOI_BO`

Neu muon dung provider Etherscan:
- `./scripts/configure-usdt-bridge.sh --provider etherscan --deposit-address 0xVI_BSC_CUA_BAN --etherscan-api-key KEY_V2_CUA_BAN`

Neu can debug bang log scan RPC (nang, de bi limit):
- `./scripts/configure-usdt-bridge.sh --provider rpc_logs --deposit-address 0xVI_BSC_CUA_BAN`

Neu khong truyen 2 gia tri tren, script tu sinh random secure key.

---

Trong truong hop cau hinh tay trong `Web.config`, cap nhat cac key:
- `USDTBridge.Enabled=true`
- `USDTBridge.ApiKeys=<api-key-1,api-key-2>`
- `USDTBridge.RequireHmacSignature=true`
- `USDTBridge.RequireJsonContentType=false` (de tranh loi POST JSON tren Mono/.aspx local)
- `USDTBridge.SigningSecret=<hmac-secret-bi-mat>`
- `USDTBridge.MaxClockSkewSeconds=90`
- `USDTBridge.RequireHttps=true` (production)
- `USDTBridge.EnforceSourceIpAllowList=true` (production)
- `USDTBridge.SourceIpAllowList=<ip-hoac-cidr-duoc-phep>`
- `USDTBridge.MaxRequestsPerMinutePerIp=120`
- `USDTBridge.AllowedChain=BSC`
- `USDTBridge.DepositAddress=<dia-chi-vi-token-theo-doi>`
- `USDTBridge.TokenContract=<dia-chi-hop-dong-token-BEP20>`
- `USDTBridge.TreasuryAccount=<tai-khoan-vi-tong-tren-he-thong>`
- `USDTBridge.PointRatePerToken=<ti-le-quy-doi-diem-A-tren-1-token>` (uu tien)
- `USDTBridge.PointRatePerUSDT=<fallback-tuong-thich-cu>`
- `USDTBridge.MinConfirmations=<so-xac-nhan-toi-thieu>`
- `USDTBridge.MinTokenPerTx=<nguong-nho-nhat>`
- `USDTBridge.MaxTokenPerTx=<nguong-lon-nhat>`
- `USDTBridge.StrictWalletOnlyMinting=true`

`StrictWalletOnlyMinting=true` se khoa duong chuyen diem `admin -> vi tong`, de vi blockchain la nguon tao diem duy nhat.

### 3) Chay watcher doc lap
1. Tao file env:
   - `cp scripts/usdt_bridge.env.example scripts/usdt_bridge.env`
2. Dien cac bien moi truong trong file env (`USDT_BRIDGE_API_KEY`, `USDT_BRIDGE_SIGNING_SECRET`, `BSC_DEPOSIT_ADDRESS` bat buoc).
   - `BSC_TOKEN_CONTRACT=<dia-chi-hop-dong-token-BEP20>`
   - `CHAIN_DATA_PROVIDER=rpc` (khuyen nghi, balance mode, khong can goi API plan)
   - `BSC_RPC_URL=https://bsc-dataseed.binance.org`
   - `RPC_BLOCK_STEP=200` (chi dung khi `CHAIN_DATA_PROVIDER=rpc_logs`)
   - `RPC_MIN_BLOCK_STEP=1`
   - `RPC_REQUEST_DELAY_MS=120`
   - `BSCSCAN_API_KEY=<key>` chi can khi `CHAIN_DATA_PROVIDER=etherscan`
   - `BSCSCAN_API_URL=https://api.etherscan.io/v2/api`
   - `BSC_CHAIN_ID=56`
   - Luu y: neu dung `etherscan` provider va gap thong bao `Free API access is not supported for this chain`, chuyen sang `CHAIN_DATA_PROVIDER=rpc`.
3. Chay watcher:
   - `python3 scripts/usdt_bridge_watcher.py --env-file scripts/usdt_bridge.env`

Watcher se doc du lieu theo provider da chon:
- `rpc` (mac dinh): doc `balanceOf` tai block an toan (khong dung `eth_getLogs`, tranh loi `limit exceeded`)
- `rpc_logs`: quet `Transfer` log truc tiep tu RPC
- `etherscan`: goi Etherscan API V2 (`chainid=56`)

Sau do watcher goi endpoint bridge de cong/tru diem vao vi tong.

Theo doi diem vi tong test (doi chieu ngay sau moi lan nap/rut):
- `./scripts/check-token-bridge-status.sh`
- `./scripts/check-token-bridge-status.sh --loop --interval 3`

Co the xem truc tiep tren admin (khong can terminal):
- Vao `http://localhost/admin/default.aspx` (hoac `http://localhost:8081/admin/default.aspx`)
- O dau trang co khung `Theo doi Token Bridge (vi tong)` hien:
  - Vi tong dang lien ket
  - So diem hien tai trong vi tong
  - Vi BSC theo doi + token contract
  - 8 giao dich bridge gan nhat (IN/OUT, so token, diem, tx hash)

### 4) Test endpoint thu cong (tu chon)
```
curl -X POST "http://localhost:8081/admin/api/usdt-bridge-credit.aspx" \
  -H "Content-Type: application/json" \
  -H "X-Bridge-Key: CHANGE_ME_STRONG_KEY" \
  -H "X-Bridge-Timestamp: 1735732800" \
  -H "X-Bridge-Nonce: 7f8e67f631c9487eb9e2d9e6f98d9651" \
  -H "X-Bridge-Signature: <hmac_sha256_hex(timestamp\\nnonce\\nbody)>" \
  -d '{
    "tx_hash":"0xaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
    "chain":"BSC",
    "token_contract":"0x55d398326f99059ff775485246999027b3197955",
    "from_address":"0x1111111111111111111111111111111111111111",
    "to_address":"0xCHANGE_ME_BSC_ADDRESS",
    "confirmations": 30,
    "direction":"in",
    "token_amount":"10.5"
  }'
```

### 5) Them vi BSC moi de chay tu dong
Khi ban tao vi moi, KHONG can sua code C# hay Python. Chi can sua config:
1. `Web.config`:
   - `USDTBridge.DepositAddress=<vi_BSC_moi>`
   - `USDTBridge.TokenContract=0x55d398326f99059ff775485246999027b3197955`
   - `USDTBridge.AllowedChain=BSC`
2. `scripts/usdt_bridge.env`:
   - `BSC_DEPOSIT_ADDRESS=<vi_BSC_moi>`
   - `BSC_TOKEN_CONTRACT=<dia-chi-hop-dong-token-BEP20>`
   - `BSC_CHAIN_ID=56`
3. Restart web + watcher:
   - `docker compose restart web admin`
   - chay lai: `python3 scripts/usdt_bridge_watcher.py --env-file scripts/usdt_bridge.env`

## Truong hop dung Windows/IIS Express (tuy chon)
- Van co script: `scripts\\start-localhost.cmd`
- URL: `http://localhost:56445`

## Ghi chu
- Du lieu upload va toan bo module giu nguyen theo ban goc.
- Day la ban de xem va chuan bi nang cap; chua ap dung hardening security.
