#!/usr/bin/env bash
set -euo pipefail
ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"
python3 "$ROOT_DIR/scripts/lib/switch-app-db-mode.py" local
printf '%s\n' 'Runtime app DB mode: local (db,1433 / ahasale_local)'
