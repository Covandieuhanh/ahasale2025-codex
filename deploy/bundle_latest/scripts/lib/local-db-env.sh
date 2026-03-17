#!/usr/bin/env bash

aha_local_root_dir() {
  if [ -n "${ROOT_DIR:-}" ]; then
    printf '%s\n' "$ROOT_DIR"
    return
  fi

  local lib_dir
  lib_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
  cd "$lib_dir/../.." && pwd
}

aha_detect_local_db_name() {
  if [ -n "${LOCAL_DB_NAME:-}" ]; then
    printf '%s\n' "$LOCAL_DB_NAME"
    return
  fi

  if [ -n "${DB_NAME:-}" ]; then
    printf '%s\n' "$DB_NAME"
    return
  fi

  local root_dir config_path
  root_dir="$(aha_local_root_dir)"
  config_path="$root_dir/Web.local.config"

  if [ -f "$config_path" ]; then
    python3 - "$config_path" <<'PY'
import re
import sys

text = open(sys.argv[1], "r", encoding="utf-8").read()
host_match = re.search(r"Data Source=([^;\"']+)", text, re.IGNORECASE)
db_match = re.search(r"Initial Catalog=([^;\"']+)", text, re.IGNORECASE)
host = host_match.group(1).strip().lower() if host_match else ""
db_name = db_match.group(1).strip() if db_match else ""

if host in ("db,1433", "db", "localhost", "127.0.0.1", ".", "(local)"):
    print(db_name or "ahasale_local")
else:
    # Safety guard: maintenance scripts still target the local Docker database
    # even when the app runtime is pointed to a shared online database.
    print("ahasale_local")
PY
    return
  fi

  printf '%s\n' 'ahasale_local'
}

aha_export_local_db_env() {
  export LOCAL_DB_NAME="${LOCAL_DB_NAME:-$(aha_detect_local_db_name)}"
  export DB_NAME="${DB_NAME:-$LOCAL_DB_NAME}"
  export DB_CONTAINER="${DB_CONTAINER:-ahasale_local_db}"
  export DB_LOGIN_TIMEOUT="${DB_LOGIN_TIMEOUT:-5}"
  export DB_QUERY_TIMEOUT="${DB_QUERY_TIMEOUT:-15}"
}
