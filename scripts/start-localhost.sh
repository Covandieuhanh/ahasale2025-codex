#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

chmod +x docker/sql/init-db.sh
chmod +x scripts/ensure-admin-full.sh
chmod +x scripts/sync-account-db.sh
chmod +x scripts/ensure-shop-slug-schema.sh
chmod +x scripts/ensure-reset-security-schema.sh
chmod +x scripts/check-hanhvi-sync.sh
chmod +x scripts/setup-local-https.sh

if [ ! -f docker/certs/ahasale.local.pem ] || [ ! -f docker/certs/ahasale.local-key.pem ]; then
  echo "Preparing local HTTPS certificate..."
  ./scripts/setup-local-https.sh
fi

if ! grep -qiE '(^|[[:space:]])ahasale\.local([[:space:]]|$)' /etc/hosts; then
  echo "Warning: /etc/hosts is missing ahasale.local. Add this line for domain testing:"
  echo "127.0.0.1 ahasale.local"
fi

docker compose up -d --build db init-db web
docker compose stop admin >/dev/null 2>&1 || true

echo "Synchronizing account login data..."
./scripts/sync-account-db.sh >/tmp/ahasale_account_sync.log 2>&1 || true

echo "Ensuring admin account has full permissions..."
./scripts/ensure-admin-full.sh >/tmp/ahasale_admin_full.log 2>&1 || true

echo "Ensuring shop slug schema..."
if ! ./scripts/ensure-shop-slug-schema.sh >/tmp/ahasale_shop_slug_schema.log 2>&1; then
  echo "Shop slug schema ensure failed. See: /tmp/ahasale_shop_slug_schema.log"
  tail -n 80 /tmp/ahasale_shop_slug_schema.log || true
  exit 1
fi

echo "Ensuring reset security schema..."
if ! ./scripts/ensure-reset-security-schema.sh >/tmp/ahasale_reset_security_schema.log 2>&1; then
  echo "Reset security schema ensure failed. See: /tmp/ahasale_reset_security_schema.log"
  tail -n 80 /tmp/ahasale_reset_security_schema.log || true
  exit 1
fi

echo "Running hanhvi sync guard..."
if ! ./scripts/check-hanhvi-sync.sh >/tmp/ahasale_hanhvi_guard.log 2>&1; then
  echo "Hanhvi sync guard failed. See: /tmp/ahasale_hanhvi_guard.log"
  tail -n 80 /tmp/ahasale_hanhvi_guard.log || true
  exit 1
fi

echo "Waiting for https://ahasale.local:1313 to be ready..."
for _ in $(seq 1 30); do
  code="$(curl -k -s -o /dev/null -w "%{http_code}" --max-time 5 --resolve ahasale.local:1313:127.0.0.1 https://ahasale.local:1313 || true)"
  if [ "$code" = "200" ]; then
    break
  fi
  sleep 2
done

echo "Waiting for https://ahasale.local:1313/admin/login.aspx to be ready..."
for _ in $(seq 1 30); do
  code="$(curl -k -s -o /dev/null -w "%{http_code}" --max-time 5 --resolve ahasale.local:1313:127.0.0.1 https://ahasale.local:1313/admin/login.aspx || true)"
  if [ "$code" = "200" ]; then
    break
  fi
  sleep 2
done

echo
echo "AhaSale local is up."
echo "Open: https://ahasale.local:1313"
echo "Admin portal: https://ahasale.local:1313/admin/login.aspx"
echo "Admin account: admin / 123"
echo "DB (optional): localhost,11433 (sa / AhaSaleLocal#2026)"
