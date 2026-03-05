#!/usr/bin/env bash
set -euo pipefail

if [ -n "${TZ:-}" ] && [ -f "/usr/share/zoneinfo/${TZ}" ]; then
  ln -snf "/usr/share/zoneinfo/${TZ}" /etc/localtime
  echo "${TZ}" >/etc/timezone
fi

if [ "${ADMIN_PORTAL:-false}" = "true" ]; then
  ln -sf /etc/nginx/sites-available/admin /etc/nginx/sites-enabled/default
else
  ln -sf /etc/nginx/sites-available/default /etc/nginx/sites-enabled/default
fi

mkdir -p /run/mono
chown www-data:www-data /run/mono
rm -f /run/mono/fastcgi.sock

su -s /bin/bash -c "fastcgi-mono-server4 /applications=/:/app /socket=unix:/run/mono/fastcgi.sock /printlog=True /loglevels=Standard >/tmp/mono-fastcgi.log 2>&1" www-data &

exec nginx -g "daemon off;"
