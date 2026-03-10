#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CERT_DIR="$ROOT_DIR/docker/certs"
CERT_FILE="$CERT_DIR/ahasale.local.pem"
KEY_FILE="$CERT_DIR/ahasale.local-key.pem"

if ! command -v mkcert >/dev/null 2>&1; then
  echo "mkcert is not installed. Install first: brew install mkcert"
  exit 1
fi

mkdir -p "$CERT_DIR"

if ! mkcert -install >/tmp/ahasale_mkcert_install.log 2>&1; then
  echo "Warning: mkcert root CA is not trusted yet."
  echo "Run this once in your own terminal (with sudo password):"
  echo "mkcert -install"
  echo
fi

mkcert \
  -cert-file "$CERT_FILE" \
  -key-file "$KEY_FILE" \
  ahasale.local localhost 127.0.0.1 ::1

echo "HTTPS cert ready:"
echo " - $CERT_FILE"
echo " - $KEY_FILE"
echo
echo "If ahasale.local does not resolve yet, add this to /etc/hosts:"
echo "127.0.0.1 ahasale.local"
