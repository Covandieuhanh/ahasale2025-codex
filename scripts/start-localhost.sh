#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

chmod +x docker/sql/init-db.sh
chmod +x scripts/ensure-admin-full.sh
chmod +x scripts/sync-account-db.sh
chmod +x scripts/ensure-shop-slug-schema.sh
chmod +x scripts/check-hanhvi-sync.sh

docker compose up -d --build db init-db web admin

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

echo "Running hanhvi sync guard..."
if ! ./scripts/check-hanhvi-sync.sh >/tmp/ahasale_hanhvi_guard.log 2>&1; then
  echo "Hanhvi sync guard failed. See: /tmp/ahasale_hanhvi_guard.log"
  tail -n 80 /tmp/ahasale_hanhvi_guard.log || true
  exit 1
fi

echo "Waiting for http://localhost to be ready..."
for _ in $(seq 1 30); do
  code="$(curl -s -o /dev/null -w "%{http_code}" --max-time 5 http://localhost || true)"
  if [ "$code" = "200" ]; then
    break
  fi
  sleep 2
done

echo "Waiting for http://localhost:8081/admin/login.aspx to be ready..."
for _ in $(seq 1 30); do
  code="$(curl -s -o /dev/null -w "%{http_code}" --max-time 5 http://localhost:8081/admin/login.aspx || true)"
  if [ "$code" = "200" ]; then
    break
  fi
  sleep 2
done

echo
echo "AhaSale local is up."
echo "Open: http://localhost"
echo "Admin portal: http://localhost:8081"
echo "Admin account: admin / 123"
echo "DB (optional): localhost,11433 (sa / AhaSaleLocal#2026)"
