#!/usr/bin/env bash
set -euo pipefail
ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"
python3 "$ROOT_DIR/scripts/lib/switch-app-db-mode.py" shared-online
printf '%s\n' 'Runtime app DB mode: shared-online (112.78.2.206 / hot79334_aha_sale)'
printf '%s\n' 'Note: shared-online mode requires a working route to SQL Server from this machine.'
