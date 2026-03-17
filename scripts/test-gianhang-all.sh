#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

SKIP_SMOKE="${SKIP_SMOKE:-0}"

./scripts/test-gianhang-sync.sh
./scripts/test-gianhang-booking.sh
./scripts/test-gianhang-cart.sh
python3 ./scripts/check-ahashine-admin-parity.py
./scripts/test-gianhang-admin-flow.sh
./scripts/test-gianhang-webcon.sh

if [ "$SKIP_SMOKE" = "0" ]; then
  ./scripts/test-gianhang.sh
fi

echo "All GianHang tests completed."
