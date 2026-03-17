#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
source "$ROOT_DIR/scripts/lib/local-db-env.sh"
aha_export_local_db_env

python3 "$ROOT_DIR/scripts/lib/run-local-sqlcmd.py" "$@"
